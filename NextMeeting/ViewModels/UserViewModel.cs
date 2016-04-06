
using Microsoft.Graph;
using NextMeeting.Common;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace NextMeeting.ViewModels
{

    public class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        private ImageSource photo = ImageHelper.UnknownPersonImage;

        private string id;

        private bool? accountEnabled;

        private string city;

        private string companyName;

        private string country;

        private string department;

        private string displayName;

        private string givenName;

        private string jobTitle;

        private string mail;

        private string mailNickname;

        private string mobilePhone;

        private string officeLocation;

        private string postalCode;

        private string preferredLanguage;

        private string state;

        private string streetAddress;

        private string surname;

        private string usageLocation;

        private string userPrincipalName;

        private string userType;

        private string aboutMe;

        private DateTimeOffset birthday;

        private DateTimeOffset hireDate;

        private string preferredName;

        private bool isLoadedPhoto = false;


        public UserViewModel(User user)
        {
            this.DisplayName = user.DisplayName;
            this.GivenName = user.GivenName;
            this.JobTitle = user.JobTitle;
            this.Mail = user.Mail;
            this.MailNickname = user.MailNickname;
            this.MobilePhone = user.MobilePhone;
            this.OfficeLocation = user.OfficeLocation;
            this.PostalCode = user.PostalCode;
            this.PreferredLanguage = user.PreferredLanguage;
            this.State = user.State;
            this.StreetAddress = user.StreetAddress;
            this.Surname = user.Surname;
            this.UsageLocation = user.UsageLocation;
            this.UserPrincipalName = user.UserPrincipalName;
            this.UserType = user.UserType;
            this.AboutMe = user.AboutMe;
            this.PreferredName = user.PreferredName;
        }


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
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        public bool? AccountEnabled
        {
            get
            {
                return accountEnabled;
            }

            set
            {
                accountEnabled = value;
                RaisePropertyChanged(nameof(AccountEnabled));
            }
        }

        public string City
        {
            get
            {
                return city;
            }

            set
            {
                city = value;
                RaisePropertyChanged(nameof(City));
            }
        }

        public string CompanyName
        {
            get
            {
                return companyName;
            }

            set
            {
                companyName = value;
                RaisePropertyChanged(nameof(CompanyName));
            }
        }

        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
                RaisePropertyChanged(nameof(Country));
            }
        }

        public string Department
        {
            get
            {
                return department;
            }

            set
            {
                department = value;
                RaisePropertyChanged(nameof(Department));
            }
        }

        public string DisplayName
        {
            get
            {
                return displayName;
            }

            set
            {
                displayName = value;
                RaisePropertyChanged(nameof(DisplayName));
            }
        }

        public string GivenName
        {
            get
            {
                return givenName;
            }

            set
            {
                givenName = value;
                RaisePropertyChanged(nameof(GivenName));
            }
        }

        public string JobTitle
        {
            get
            {
                return jobTitle;
            }

            set
            {
                jobTitle = value;
                RaisePropertyChanged(nameof(JobTitle));
            }
        }

        public string Mail
        {
            get
            {
                return mail;
            }

            set
            {
                mail = value;
                RaisePropertyChanged(nameof(Mail));
            }
        }

        public string MailNickname
        {
            get
            {
                return mailNickname;
            }

            set
            {
                mailNickname = value;
                RaisePropertyChanged(nameof(MailNickname));
            }
        }

        public string MobilePhone
        {
            get
            {
                return mobilePhone;
            }

            set
            {
                mobilePhone = value;
                RaisePropertyChanged(nameof(MobilePhone));
            }
        }



        public string OfficeLocation
        {
            get
            {
                return officeLocation;
            }

            set
            {
                officeLocation = value;
                RaisePropertyChanged(nameof(OfficeLocation));
            }
        }

        public string PostalCode
        {
            get
            {
                return postalCode;
            }

            set
            {
                postalCode = value;
                RaisePropertyChanged(nameof(PostalCode));
            }
        }

        public string PreferredLanguage
        {
            get
            {
                return preferredLanguage;
            }

            set
            {
                preferredLanguage = value;
                RaisePropertyChanged(nameof(PreferredLanguage));
            }
        }

        public string State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
                RaisePropertyChanged(nameof(State));
            }
        }

        public string StreetAddress
        {
            get
            {
                return streetAddress;
            }

            set
            {
                streetAddress = value;
                RaisePropertyChanged(nameof(StreetAddress));
            }
        }

        public string Surname
        {
            get
            {
                return surname;
            }

            set
            {
                surname = value;
                RaisePropertyChanged(nameof(Surname));
            }
        }

        public string UsageLocation
        {
            get
            {
                return usageLocation;
            }

            set
            {
                usageLocation = value;
                RaisePropertyChanged(nameof(UsageLocation));
            }
        }

        public string UserPrincipalName
        {
            get
            {
                return userPrincipalName;
            }

            set
            {
                userPrincipalName = value;
                RaisePropertyChanged(nameof(UserPrincipalName));
            }
        }

        public string UserType
        {
            get
            {
                return userType;
            }

            set
            {
                userType = value;
                RaisePropertyChanged(nameof(UserType));
            }
        }

        public string AboutMe
        {
            get
            {
                return aboutMe;
            }

            set
            {
                aboutMe = value;
                RaisePropertyChanged(nameof(AboutMe));
            }
        }

        public DateTimeOffset Birthday
        {
            get
            {
                return birthday;
            }

            set
            {
                birthday = value;
                RaisePropertyChanged(nameof(Birthday));
            }
        }

        public DateTimeOffset HireDate
        {
            get
            {
                return hireDate;
            }

            set
            {
                hireDate = value;
                RaisePropertyChanged(nameof(HireDate));
            }
        }

        public string PreferredName
        {
            get
            {
                return preferredName;
            }

            set
            {
                preferredName = value;
                RaisePropertyChanged(nameof(PreferredName));
            }
        }


        public async Task UpdatePhotoAsync()
        {
            if (IsLoadedPhoto)
                return;

            if (String.IsNullOrEmpty(this.UserPrincipalName))
                return;

            var tuple = await GraphService.GetPhotoAsync(this.UserPrincipalName);

            if (tuple != null)
            {
                this.Photo = tuple.Item1;
                // this.PhotoUri = tuple.Item2;

            }

            IsLoadedPhoto = true;
        }
    }

}
