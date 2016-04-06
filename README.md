# NextMeetingsForGraphSample
Microsoft Graph SDK sample application.

This sample implements the **Microsoft Graph .Net SDK**, with an **ADAL** authentication provider in an **UWP** application


## Useful links before get into the code !

- https://github.com/microsoftgraph
- https://www.nuget.org/packages/Microsoft.Graph
- https://github.com/OfficeDev/Microsoft-Graph-UWP-Connect-Library
- https://github.com/azuread


## Demo application

this **UWP** application is provided with 3 main features:
- Authentication through ADAL to your O365 Tenant.
- Get users and photos profiles through Microsoft.Graph.
- Get, set and remove events through Microsoft.Graph.

![](https://msdnshared.blob.core.windows.net/media/2016/04/NextMeeting01.png) 

## GraphServiceClient et Authentification

Here is a part of the **AuthenticationHelper** class, which implements **IAuthenticationProvider** :

    public class AuthenticationHelper : IAuthenticationProvider
    {


        public async Task<AuthenticationResult> GetAccessTokenAsync()
        {
            if (authContext == null)
                authContext = new AuthenticationContext(Authority);

            AuthenticationResult authResult = null;

            authResult = await authContext.AcquireTokenSilentAsync(GraphResourceId, ClientId);

            if (authResult.Status != AuthenticationStatus.Success && authResult.Error == "failed_to_acquire_token_silently")
                authResult = await authContext.AcquireTokenAsync(GraphResourceId, ClientId, AppRedirectURI, PromptBehavior.Auto);

            if (authResult.Status != AuthenticationStatus.Success)
            {
                if (authResult.Error != "authentication_canceled")
                {
                    MessageDialog dialog = new MessageDialog(string.Format("If the error continues, please contact your administrator.\n\nError: {0}\n\n Error Description:\n\n{1}", authResult.Error, authResult.ErrorDescription), "Sorry, an error occurred while signing you in.");
                    await dialog.ShowAsync();
                }
            }


            return authResult;

        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var authResult = await GetAccessTokenAsync();

            if (authResult.Status != AuthenticationStatus.Success)
            {
                throw new ServiceException(new Error
                {
                     Code = authResult.Error,
                     Message = authResult.ErrorDescription
                });
            }

            request.Headers.Add("Authorization", "Bearer " + authResult.AccessToken); // + token
        }
    }


**Note : You should implement a real async mechanism. DON'T use a MessageDialog to manage your errors. Use a CancellationToken instead :)**

## Microsoft.Graph get and push informations to Office 365

### Getting Informations
 
Getting my profile informations (notice the **Request()** method involved)

    GraphServiceClient graphService = new GraphServiceClient(AuthenticationHelper.Current);
    return await graphService.Me.Request().GetAsync();
    
Getting photos from a particular user (notice the **Content** property used to get the /$value request) :

      // var photoInfo = await graphService.Users[identifier].Photo.Request().GetAsync(); //users/{1}/photo : Getting info about the photo, not the photo itself
      var photoStream = await graphService.Users[identifier].Photo.Content.Request().GetAsync(); //users/{1}/photo/$value : Getting photo stream

      byte[] photoByte = new byte[photoStream.Length];
      photoStream.Read(photoByte, 0, (int)photoStream.Length);

      var bitmapImage = await ImageHelper.SaveImageToCacheAndGetImage(photoByte, fileName);

      return bitmapImage;

Getting events from your calendar view (notice the **QueryOption** parameters) :

    var start = DateTime.UtcNow.ToString("o");
    var end = DateTime.UtcNow.AddMonths(1).ToString("o");

    QueryOption optionStart = new QueryOption("startDateTime", start);
    QueryOption optionEnd = new QueryOption("endDateTime", end);
    var calendarView = await graphService.Me.CalendarView.Request(new[] { optionStart, optionEnd }).GetAsync(); // suffit pas

    var hasMorePages = true;
    var events = new List<Event>();
    while (hasMorePages)
    {
        foreach (var ev in calendarView.CurrentPage.ToList())
            events.Add(ev);
        hasMorePages = calendarView.NextPageRequest != null;
        if (hasMorePages)
            calendarView = await calendarView.NextPageRequest.GetAsync();
    }
    return events;

Delete an event :

    GraphServiceClient graphService = new GraphServiceClient(AuthenticationHelper.Current);
    try
    {
        await graphService.Me.Events[id].Request().DeleteAsync();
    }
    catch (ServiceException ex)
    {
        Debug.WriteLine(ex.Error.Message);
    }

Adding or Updating an event :

    GraphServiceClient graphService = new GraphServiceClient(AuthenticationHelper.Current);
    try
    {
        if (!String.IsNullOrEmpty(_event.Id))
            await graphService.Me.Events[_event.Id].Request().UpdateAsync(_event);
        else
            await graphService.Me.Events.Request().AddAsync(_event);
    }
    catch (ServiceException ex)
    {
        Debug.WriteLine(ex.Error.Message);
    }

Enjoy !