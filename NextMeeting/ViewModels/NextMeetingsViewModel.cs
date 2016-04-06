//using Microsoft.Graph;
using NextMeeting.Common;
using NextMeeting.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NextMeeting.ViewModels
{
    public class NextMeetingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
        private ItemClickEventHandler clickCommand;
        private RoutedEventHandler addCommand;
        public NextMeetingsViewModel()
        {
        }


        public async Task LoadNextMeetingsAsync()
        {
            try
            {
                IsLoading = true;
                Events.Clear();

                var allevents = await GraphService.GetEventsAsync();

                foreach (var ev in allevents)
                    Events.Add(new EventViewModel(ev));

                IsLoading = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception " + ex.Message);
            }
        }

        private ObservableCollection<EventViewModel> events = new ObservableCollection<EventViewModel>();

        public ObservableCollection<EventViewModel> Events
        {
            get
            {
                return events;
            }

            set
            {
                events = value;
                RaisePropertyChanged(nameof(Events));
            }
        }
        private bool isLoading;

        public bool IsLoading
        {
            get
            {
                return isLoading;
            }

            set
            {
                isLoading = value;
                RaisePropertyChanged(nameof(IsLoading));
            }
        }

        public ItemClickEventHandler ClickCommand
        {
            get
            {
                if (clickCommand == null)
                {
                    clickCommand = new ItemClickEventHandler((sender, e) =>
                    {
                        EventViewModel eventClicked = e.ClickedItem as EventViewModel;
                        AppShell.Current.Navigate(typeof(NextMeetingDetails), eventClicked);

                    });
                }
                return clickCommand;
            }


        }

        public RoutedEventHandler AddCommand
        {
            get
            {
                if (addCommand == null)
                {

                    addCommand = new RoutedEventHandler((sender, e) =>
                    {

                        AppShell.Current.Navigate(typeof(NextMeetingDetails), new EventViewModel());

                    });
                }
                return addCommand;
            }


        }
    }
}
