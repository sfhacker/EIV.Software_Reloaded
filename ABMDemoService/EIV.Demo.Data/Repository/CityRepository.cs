
namespace EIV.Demo.Data.Repository
{
    using EIV.Demo.Data.Base;
    using EIV.Demo.Data.Interface;
    using EIV.Demo.Model;
    using System.Collections.Generic;

    public sealed class CityRepository : Repository<City>, ICityRepository
    {
        private IList<City> cities = null;
        public CityRepository()
        {
            this.Reset();
            this.PopulateCities();
        }

        private void Reset()
        {
            if (this.cities == null)
            {
                this.cities = new List<City>();
            }
        }

        private void PopulateCities()
        {
            if (this.cities == null)
            {
                return;
            }
            if (this.cities.Count != 0)
            {
                return;
            }

            State thisState = new State() { Id = 12, Name = "Santa Fe" };

            this.cities.Add(
                new City()
                {
                    Id = 89,
                    Name = "Rosario"
                }
            );

            this.cities[0].SetState(thisState);
        }
    }
}