using NextMeeting.Common;
using NextMeeting.ViewModels;
using NextMeeting.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NextMeeting.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DisconnectPage : Page
    {

        private async System.Threading.Tasks.Task Disconnect()
        {


            await AuthenticationHelper.Current.SignOutAsync();

            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(LoginPage));
        }

      
        public DisconnectPage()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await AuthenticationHelper.Current.SignOutAsync();

            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(LoginPage));
        }

    }
}
