using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NextMeeting.Common
{
    public class NavigationHelper
    {
        private static NavigationHelper current;
        private SystemNavigationManager shellSystemNavigationManager;
        private SystemNavigationManager rootSystemNavigationManager;

        public static NavigationHelper Current
        {
            get
            {
                if (current == null)
                    current = new NavigationHelper();

                return current;
            }
        }

        /// <summary>
        /// Entry page. Cant go back from this entry page.
        /// </summary>
        public Type EntryPage { get; private set; }

        /// <summary>
        /// Root frame
        /// </summary>
        public Frame RootFrame { get; private set; }

        /// <summary>
        /// Splitter frame
        /// </summary>
        public Frame ShellFrame { get; private set; }
        public NavigationHelper()
        {

        }
        public void RegisterRootFrame(Frame rootFrame)
        {
            if (rootFrame == null)
                return;

            this.RootFrame = rootFrame;
            this.rootSystemNavigationManager = SystemNavigationManager.GetForCurrentView();

            // Register root navigation to handle back button
            this.RootFrame.Navigated += (s, e) =>
                this.rootSystemNavigationManager.AppViewBackButtonVisibility =
                    this.RootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

            this.RootFrame.NavigationFailed += OnNavigationFailed;

            this.rootSystemNavigationManager.BackRequested += OnBackRequestedFromRootFrame;
        }
        public void RegisterShellFrame(Frame shellFrame)
        {
            if (shellFrame == null)
                return;

            this.ShellFrame = shellFrame;
            this.shellSystemNavigationManager = SystemNavigationManager.GetForCurrentView();

            // Register root navigation to handle back button
            this.ShellFrame.Navigated += (s, e) =>
                this.shellSystemNavigationManager.AppViewBackButtonVisibility =
                    this.ShellFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

            this.RootFrame.NavigationFailed += OnNavigationFailed;

            this.shellSystemNavigationManager.BackRequested += OnBackRequestedFromShellFrame;
        }
        public void RegisterEntryPage(Type entryPage)
        {
            this.EntryPage = entryPage;
        }


        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// back from Shell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackRequestedFromShellFrame(object sender, BackRequestedEventArgs e)
        {
            if (this.ShellFrame.CanGoBack)
            {
                e.Handled = true;
                this.ShellFrame.GoBack();
            }
        }
        /// <summary>
        /// Back Request from root frame
        /// Just check if we are at the entry page, so dont go back, just exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackRequestedFromRootFrame(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Check if we are on the entry page and try to go back
                var backTypePage = rootFrame.BackStack[rootFrame.BackStackDepth - 1];

                if (this.EntryPage != null && backTypePage.SourcePageType == this.EntryPage)
                {
                    e.Handled = false;
                    rootFrame.BackStack.Clear();
                }
                else
                {
                    e.Handled = true;
                    rootFrame.GoBack();
                }
            }
        }
    }
}
