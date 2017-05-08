/// <summary>
/// 
/// </summary>
namespace EIV.Demo.Model
{
    using Base;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Client : EntityBase
    {
        public enum EnumClientType
        {
            Unknown = -1,
            Basic,
            Premier
        };

        private string firstName;
        private string lastName;
        private DateTime dateOfBirth;
        private City city = null;
        private EnumClientType clientType;
        
        public Client()
        {
        }

        #region Properties

        public virtual string FirstName
        {
            get
            {
                return this.firstName;
            }

            set
            {
                this.firstName = value;
            }
        }

        /// <summary>
        /// Gets or sets Last Name.
        /// </summary>
        public virtual string LastName
        {
            get
            {
                return this.lastName;
            }

            set
            {
                this.lastName = value;
            }
        }

        public virtual DateTime DOB
        {
            get
            {
                return this.dateOfBirth;
            }

            set
            {
                this.dateOfBirth = value;
            }
        }

        //[ForeignKey("CityId")]
        public virtual City City
        {
            get
            {
                return this.city;
            }
        }

        public EnumClientType ClientType
        {
            get
            {
                return this.clientType;
            }
            set
            {
                this.clientType = value;
            }
        }
        #endregion Properties

        public void SetCity(City city)
        {
            if (city == null)
            {
                return;
            }

            this.city = city;
        }
    }
}