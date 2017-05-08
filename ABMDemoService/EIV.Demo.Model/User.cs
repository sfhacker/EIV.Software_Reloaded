
namespace EIV.Demo.Model
{
    using EIV.Demo.Model.Base;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.OData.Builder;

    public sealed class User : EntityBase
    {
        private string userName = string.Empty;
        private string userPassword = string.Empty;
        private DateTime fechaAlta = default(DateTime);
        private DateTime fechaBaja = default(DateTime);

        private ObservableCollection<MenuItem> menuItems;

        public User()
        {
            this.menuItems = new ObservableCollection<MenuItem>();
        }

        // Should i put this here on in the WebService project?
        [Required]
        public string Name
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
            }
        }

        // Not shown on $metadata
        public string Password
        {
            set
            {
                this.userPassword = value;
            }
        }

        /*
        public DateTime FechaAlta
        {
            get
            {
                return this.fechaAlta;
            }
            set
            {
                this.fechaAlta = value;
            }
        }

        public DateTime FechaBaja
        {
            get
            {
                return this.fechaBaja;
            }
            set
            {
                this.fechaBaja = value;
            }
        }*/

        // OData requires both setter AND getter ????
        // Otherwise, it cannot be used in a POST method
        // However, I should use 'AddMenuItem' method
        // and create an Action for it!
        [Contained]
        public ObservableCollection<MenuItem> MenuItems
        {
            get
            {
                return this.menuItems;
            }
            set
            {
                this.menuItems = value;
            }
        }

        // Always hashed passwords, please!
        // Should create an Action for the 
        // password property
        public void SetPassword(string password)
        {
            this.userPassword = password;

        }

        public bool ComparePassword(string password)
        {
            return this.userPassword.Equals(password);

        }
        public void AddMenuItem(MenuItem menuItem)
        {
            // some validation here
            this.menuItems.Add(menuItem);
        }

        public void RemoveMenuItem(MenuItem menuItem)
        {
            if (this.menuItems.Contains(menuItem))
            {
                this.menuItems.Remove(menuItem);
            }
        }

    }
}