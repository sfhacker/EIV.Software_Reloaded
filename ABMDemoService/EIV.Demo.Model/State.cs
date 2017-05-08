

namespace EIV.Demo.Model
{
    using EIV.Demo.Model.Base;
    using System.Collections.Generic;

    public class State : EntityBase
    {
        private string name = string.Empty;
        private int countryId = -1;

        private Country country = null;
        private IList<City> cities = null;

        // OData requires a parameterless constructor (and public)
        public State()
        {
            this.cities = new List<City>();
        }

        #region Properties

        public virtual string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        public virtual int CountryId
        {
            get
            {
                return this.countryId;
            }
            set
            {
                this.countryId = value;
            }
        }
        //[Association("Country", "CountryId", "")]
        //[Contained]
        public virtual Country Country
        {
            get
            {
                return this.country;
            }
        }

        public virtual IList<City> Cities
        {
            get
            {
                return this.cities;
            }
        }

        #endregion

        internal void AddCity(City city)
        {
            if (city == null)
            {
                return;
            }

            // Some validations go here
            // e.g. duplications
            this.cities.Add(city);
        }

        public void SetCountry(Country country)
        {
            if (country == null)
            {
                return;
            }
            // Some validations go here
            this.country = country;
            //this.CountryId = country.Id;
            this.country.AddState(this);
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.name))
            {
                return false;
            }
            if (this.country == null)
            {
                return false;
            }

            return true;
        }
    }
}