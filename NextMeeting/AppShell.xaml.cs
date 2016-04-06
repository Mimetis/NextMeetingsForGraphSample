
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NextMeeting.Common;
using NextMeeting.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NextMeeting
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppShell : Windows.UI.Xaml.Controls.Page
    {
        // Store account-specific settings so that the app can remember that a user has already signed in.
        public static ApplicationDataContainer settings = ApplicationData.Current.RoamingSettings;

        private static AppShell current;
        public static AppShell Current
        {
            get
            {
                //if (current == null)
                //    current = new AppShell();

                return current;
            }
        }

        private ObservableCollection<NavLink> navLinks;
        public ObservableCollection<NavLink> NavLinks
        {
            get { return navLinks; }
        }

        public Windows.UI.Xaml.Controls.Page CurrentPage { get; private set; }

        public Type GetCurrentSourcePageType()
        {
            return this.ShellFrame.CurrentSourcePageType;
        }

        public AppShell()
        {
            this.InitializeComponent();

            NavigationHelper.Current.RegisterShellFrame(ShellFrame);

            // Register singleton
            this.Loaded += (s, e) =>
            {
                current = this;

                var pageType = this.ShellFrame.CurrentSourcePageType;
                if (pageType == null || pageType == typeof(DisconnectPage))
                    this.Navigate(typeof(SearchPeople));
            };


            this.navLinks = new ObservableCollection<NavLink>()
            {
                new NavLink() { Label = "Search", Symbol = Symbol.Find,
                   DestPage = typeof(SearchPeople)},
                new NavLink() { Label = "Calendar", Symbol = Symbol.Calendar,
                    DestPage = typeof(NextMeetings)},
                new NavLink() { Label = "Settings", Symbol = Symbol.Setting,
                    DestPage = typeof(Settings) },
                new NavLink() { Label = "Disconnect", Symbol = Symbol.Import,
                    DestPage = typeof(DisconnectPage) },
            };

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Window.Current.Bounds.Width < 640)
                Splitter.IsPaneOpen = false;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }
        private void NavLinksList_ItemClick(object sender, ItemClickEventArgs e)
        {
            NavLink navLink = e.ClickedItem as NavLink;

            if (navLink.Action != null)
            {
                navLink.Action();
                return;
            }

            if (navLink.DestPage != null)
            {
                Splitter.IsPaneOpen = false;

                if (navLink.DestPage != null && navLink.DestPage != this.ShellFrame.CurrentSourcePageType)
                    this.ShellFrame.Navigate(navLink.DestPage, navLink.Arguments);
            }
        }

        public void Navigate(Type page, Object args = null)
        {
            this.ShellFrame.Navigate(page, args);
        }
        private void ShellFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var nvc = e;
        }
        private void ShellFrame_Navigated(object sender, NavigationEventArgs e)
        {
            this.CurrentPage = e.Content as Windows.UI.Xaml.Controls.Page;

        }

        private void ButtonTransparentForBigFinger_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }
    }
}
