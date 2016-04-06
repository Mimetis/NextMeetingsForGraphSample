using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NextMeeting.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NextMeeting
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {

 
        public LoginPage()
        {
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }



        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;

            for (int i = 0; i < pivot.Items.Count; i++)
            {
                if (i == pivot.SelectedIndex)
                {
                    PivotItem selectedPivotItem = pivot.SelectedItem as PivotItem;

                    (selectedPivotItem.Header as Ellipse).Fill = new SolidColorBrush(Color.FromArgb(255, 0, 112, 192));

                }
                else
                {
                    PivotItem unselectedPivotItem = pivot.Items[i] as PivotItem;
                    (unselectedPivotItem.Header as Ellipse).Fill = new SolidColorBrush(Colors.LightGray);
                }
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (sender as Button).IsEnabled = false;

                var me = await GraphService.GetMe();

                (sender as Button).IsEnabled = true;

                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(AppShell));

            }
            catch (ServiceException ex)
            {
                (sender as Button).IsEnabled = true;

            }

        }

    }
}
