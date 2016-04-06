using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using NextMeeting.ViewModels;

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Graph;

namespace NextMeeting.Common
{
    /// <summary>
    /// Extensions when Microsoft.Graph wrapper is not enough
    /// </summary>
    public static class GraphService
    {

        private static User me;
        public async static Task<User> GetMe()
        {
            if (me != null)
                return me;
            try
            {
                GraphServiceClient graphService = new GraphServiceClient(AuthenticationHelper.Current);
                var userMe = await graphService.Me.Request().GetAsync();
                GraphService.me = userMe;
                return GraphService.me;
            }
            catch (Microsoft.Graph.ServiceException serviceException)
            {
                Debug.WriteLine(serviceException.Error.Code + " : " + serviceException.Error.Message);
            }

            return null;
        }

        
        public async static Task DeleteEventAsync(string id)
        {
            GraphServiceClient graphService = new GraphServiceClient(AuthenticationHelper.Current);
            try
            {

                await graphService.Me.Events[id].Request().DeleteAsync();

            }
            catch (ServiceException ex)
            {
                Debug.WriteLine(ex.Error.Message);
            }
        }
        public async static Task SaveEventAsync(Event _event)
        {
            GraphServiceClient graphService = new GraphServiceClient(AuthenticationHelper.Current);
            try
            {
                if (!String.IsNullOrEmpty(_event.Id))
                {
                    await graphService.Me.Events[_event.Id].Request().UpdateAsync(_event);
                }
                else
                {
                    await graphService.Me.Events.Request().AddAsync(_event);
                }

            }
            catch (ServiceException ex)
            {
                Debug.WriteLine(ex.Error.Message);
            }
        }

        public async static Task<List<Event>> GetEventsAsync()
        {
            GraphServiceClient graphService = new GraphServiceClient();
            try
            {
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


            }
            catch (ServiceException ex)
            {
                Debug.WriteLine(ex.Error.Message);
                return null;
            }
        }

        public async static Task<Tuple<BitmapImage, Uri>> GetPhotoAsync(string identifier)
        {

            var fileName = identifier + ".png";

            var tuple = await ImageHelper.GetImageFromCache(fileName);

            if (tuple != null)
                return tuple;


            GraphServiceClient graphService = new GraphServiceClient(AuthenticationHelper.Current);

            try
            {
                var photoInfo = await graphService.Users[identifier].Photo.Request().GetAsync(); //users/{1}/photo
                var photoStream = await graphService.Users[identifier].Photo.Content.Request().GetAsync(); //users/{1}/photo/$value

                byte[] photoByte = new byte[photoStream.Length];
                photoStream.Read(photoByte, 0, (int)photoStream.Length);

                var bitmapImage = await ImageHelper.SaveImageToCacheAndGetImage(photoByte, fileName);
                var imgUri = new Uri("ms-appdata:///local/" + fileName);

                return new Tuple<BitmapImage, Uri>(bitmapImage, imgUri);
            }
            catch (ServiceException ex)
            {
                Debug.WriteLine(ex.Error.Message);
                return null;
            }


        }


        public async static Task<List<User>> GetUsersLikeAsync(string id)
        {
            List<User> users = new List<User>();

            GraphServiceClient graphService = new GraphServiceClient(AuthenticationHelper.Current);

            try
            {
                var usersQuery = await graphService.Users.Request().Filter("startswith(displayName, '" + id + "')").GetAsync();


                var hasMorePages = true;
                while (hasMorePages)
                {
                    foreach (var u in usersQuery.CurrentPage.ToList())
                        users.Add(u);

                    hasMorePages = usersQuery.NextPageRequest != null;
                    if (hasMorePages)
                        usersQuery = await usersQuery.NextPageRequest.GetAsync();
                }

                return users;
            }
            catch (ServiceException ex)
            {
                Debug.WriteLine(ex.Error.Message);
                return null;
            }

        }





    }
}
