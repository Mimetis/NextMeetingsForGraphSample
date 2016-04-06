//using Microsoft.Graph;
using Microsoft.Graph;
using NextMeeting.Common;
using NextMeeting.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NextMeeting.ViewModels
{
    public class EventViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private int index;
        private bool? isAllDay = false;
        internal string organizerEmail;
        private string organizerFriendlyName;
        private string subject;
        private string bodyPreview;
        private string location;
        private DateTimeOffset startingDate;
        private DateTimeOffset endingDate;
        private TimeSpan startingHour;
        private TimeSpan endingHour;
        private UserViewModel organizer;
        private TaskScheduler uiScheduler;
        private ImageSource photo = ImageHelper.UnknownPersonImage;
        private bool isLoadedPhoto = false;
        private bool isLoading = false;
        private RoutedEventHandler saveCommand;
        private RoutedEventHandler deleteCommand;

        public bool IsLoadedPhoto
        {
            get
            {
                return isLoadedPhoto;
            }
            private set
            {
                isLoadedPhoto = value;

                RaisePropertyChanged(nameof(IsLoadedPhoto));
            }
        }
        public ImageSource Photo
        {
            get
            {
                if (!IsLoadedPhoto)
                    this.UpdatePhotoAsync();

                return photo;
            }
            set
            {

                this.photo = value;

                RaisePropertyChanged(nameof(Photo));
            }
        }
        public async Task UpdatePhotoAsync()
        {
            if (IsLoadedPhoto)
                return;

            if (String.IsNullOrEmpty(this.organizerEmail))
                return;

            var tuple = await GraphService.GetPhotoAsync(this.organizerEmail);

            if (tuple != null)
            {
                this.Photo = tuple.Item1;
                // this.PhotoUri = tuple.Item2;

            }

            IsLoadedPhoto = true;
        }
        public String Body { get; set; }
        public String Id { get; set; }
        public string BodyPreview
        {
            get
            {
                return bodyPreview;
            }

            set
            {
                bodyPreview = value;
                RaisePropertyChanged(nameof(BodyPreview));
            }
        }
        public String Subject
        {
            get
            {
                return subject;
            }

            set
            {
                subject = value;
                RaisePropertyChanged(nameof(Subject));
            }
        }
        public DateTimeOffset StartingDate
        {
            get
            {
                return startingDate;
            }

            set
            {
                if (startingDate == value)
                    return;

                startingDate = value;
                RaisePropertyChanged(nameof(StartingDate));
            }
        }
        public DateTimeOffset EndingDate
        {
            get
            {
                return endingDate;
            }

            set
            {
                if (endingDate == value)
                    return;

                endingDate = value;
                RaisePropertyChanged(nameof(EndingDate));
            }
        }
        public String StartingDateString
        {
            get
            {
                return StartingDate.DateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            }


        }
        public String EndingDateString
        {
            get
            {
                return EndingDate.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            }


        }
        public String TimeDelta
        {
            get
            {
                if (this.IsAllDay.HasValue && this.IsAllDay.Value)
                    return "All day event";

                return this.StartingHourString + "-" + this.EndingHourString;
            }
        }
        public String StartingHourString
        {
            get
            {
                var hour = StartingDate.Hour < 10 ? "0" + StartingDate.Hour.ToString() : StartingDate.Hour.ToString();
                var min = StartingDate.Minute < 10 ? "0" + StartingDate.Minute.ToString() : StartingDate.Minute.ToString();
                return string.Format("{0}:{1}", hour, min);
            }
        }
        public String EndingHourString
        {
            get
            {
                var hour = EndingDate.Hour < 10 ? "0" + EndingDate.Hour.ToString() : EndingDate.Hour.ToString();
                var min = EndingDate.Minute < 10 ? "0" + EndingDate.Minute.ToString() : EndingDate.Minute.ToString();
                return string.Format("{0}:{1}", hour, min);
            }
        }
        public UserViewModel Organizer
        {
            get
            {
                return organizer;
            }

            set
            {
                organizer = value;
                RaisePropertyChanged(nameof(Organizer));

                if (Organizer != null && Organizer.DisplayName != this.OrganizerFriendlyName)
                    this.OrganizerFriendlyName = Organizer.DisplayName;
            }
        }
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
        public string OrganizerFriendlyName
        {
            get
            {
                return organizerFriendlyName;
            }

            set
            {
                organizerFriendlyName = value;
                RaisePropertyChanged(nameof(OrganizerFriendlyName));
            }
        }
        public bool? IsAllDay
        {
            get
            {
                return isAllDay;
            }

            set
            {
                isAllDay = value;
                RaisePropertyChanged(nameof(IsAllDay));
            }
        }
        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
                RaisePropertyChanged(nameof(Index));
            }
        }
        public TimeSpan StartingHour
        {
            get
            {
                return startingHour;
            }

            set
            {
                startingHour = value;
                RaisePropertyChanged(nameof(StartingHour));
            }
        }
        public TimeSpan EndingHour
        {
            get
            {
                return endingHour;
            }

            set
            {
                endingHour = value;
                RaisePropertyChanged(nameof(EndingHour));
            }
        }
        public RoutedEventHandler SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RoutedEventHandler(async (sender, e) =>
                    {
                        Event _event = new Event();

                        Recipient organizer = null;
                        organizer = new Recipient();
                        organizer.EmailAddress = new EmailAddress();

                        if (String.IsNullOrEmpty(this.Id))
                        {
                            var me = await GraphService.GetMe();
                            organizer.EmailAddress.Address = me.Mail;
                            organizer.EmailAddress.Name = me.DisplayName;

                        }
                        else
                        {
                            organizer.EmailAddress.Address = this.organizerEmail;
                            organizer.EmailAddress.Name = this.organizerFriendlyName;
                            _event.Id = this.Id;
                        }
                        _event.Organizer = organizer;

                        _event.Body = new ItemBody
                        {
                            Content = this.BodyPreview,
                            ContentType = BodyType.Text
                        };
                        _event.Subject = this.Subject;

                        DateTimeTimeZone start = new DateTimeTimeZone();
                        start.TimeZone = "UTC";
                        start.DateTime = this.StartingDate.Date.AddTicks(this.StartingHour.Ticks).ToUniversalTime().ToString("G");
                        _event.Start = start;

                        DateTimeTimeZone end = new DateTimeTimeZone();
                        end.TimeZone = "UTC";
                        end.DateTime = this.EndingDate.Date.AddTicks(this.EndingHour.Ticks).ToUniversalTime().ToString("G");
                        _event.End = end;

                        await GraphService.SaveEventAsync(_event);
                        AppShell.Current.Navigate(typeof(NextMeetings));

                    });

                }
                return saveCommand;
            }
        }

        public RoutedEventHandler DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    deleteCommand = new RoutedEventHandler(async (sender, e) =>
                    {
                        if (!String.IsNullOrEmpty(this.Id))
                            await GraphService.DeleteEventAsync(this.Id);

                        AppShell.Current.Navigate(typeof(NextMeetings));

                    });

                }
                return deleteCommand;
            }

        }
        public EventViewModel()
        {
            this.StartingDate = new DateTimeOffset(DateTime.Now);
            this.EndingDate = new DateTimeOffset(DateTime.Now.AddHours(1));
            this.StartingHour = this.StartingDate.TimeOfDay;
            this.EndingHour = this.EndingDate.TimeOfDay;

        }
        public EventViewModel(Event ev)
        {
            this.IsLoading = true;
            this.uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            this.Id = ev.Id;
            this.Subject = ev.Subject;

            this.StartingDate = ev.Start == null ? new DateTimeOffset(DateTime.Now) : new DateTimeOffset(DateTime.Parse(ev.Start.DateTime).ToLocalTime());
            this.EndingDate = ev.End == null ? new DateTimeOffset(DateTime.Now.AddHours(1)) : new DateTimeOffset(DateTime.Parse(ev.End.DateTime).ToLocalTime());
            this.StartingHour = ev.Start == null ? DateTime.Now.TimeOfDay : this.StartingDate.TimeOfDay;
            this.EndingHour = ev.End == null ? DateTime.Now.AddHours(1).TimeOfDay : this.EndingDate.TimeOfDay;

            this.BodyPreview = ev.BodyPreview;
            this.IsAllDay = ev.IsAllDay;
            if (ev.Organizer != null)
            {
                this.organizerEmail = ev.Organizer.EmailAddress.Address;
                this.OrganizerFriendlyName = ev.Organizer.EmailAddress.Name;
            }
            this.IsLoading = false;

        }

    }
}
