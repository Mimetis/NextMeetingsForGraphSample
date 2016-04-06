using NextMeeting.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace NextMeeting.ViewModels
{
    public class SearchPeopleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        public SearchPeopleViewModel()
        {

        }

        public async Task SearchClick(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.QueryText))
            {
                try
                {
                    IsLoading = true;
                    Users.Clear();

                    var allusers = await GraphService.GetUsersLikeAsync(args.QueryText);

                    foreach (var user in allusers)
                        Users.Add(new UserViewModel(user));

                    IsLoading = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception " + ex.Message);
                }

            }
        }

        private bool isLoading;
        private ObservableCollection<UserViewModel> users = new ObservableCollection<UserViewModel>();

        public ObservableCollection<UserViewModel> Users
        {
            get
            {
                return users;
            }

            set
            {
                users = value;
                RaisePropertyChanged(nameof(Users));
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

    }
}
