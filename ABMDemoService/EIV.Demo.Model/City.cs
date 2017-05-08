
namespace EIV.Demo.Model
{
    using EIV.Demo.Model.Base;

    public class City : EntityBase
    {
        private string name = string.Empty;
        private string postalCode = string.Empty;
        private State state = null;

        public City()
        {
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

        public virtual string PostalCode
        {
            get
            {
                return this.postalCode;
            }

            set
            {
                this.postalCode = value;
            }
        }

        public virtual State State
        {
            get
            {
                return this.state;
            }
        }

        public void SetState(State state)
        {
            if (state == null)
            {
                return;
            }
            // Some validations go here
            this.state = state;
            this.state.AddCity(this);
        }

        #endregion
    }
}