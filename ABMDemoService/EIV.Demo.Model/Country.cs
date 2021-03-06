﻿
namespace EIV.Demo.Model
{
    using EIV.Demo.Model.Base;
    using System.Collections.Generic;

    public class Country : EntityBase
    {
        private string name;
        private string ddi;

        private IList<State> states = null;

        public Country()
        {
            this.states = new List<State>();
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

        public virtual string DDI
        {
            get
            {
                return this.ddi;
            }

            set
            {
                this.ddi = value;
            }
        }

        // Is this a good idea?
        // I want an immutable list
        // Client should NOT use this property
        // to add states
        public virtual IList<State> States
        {
            get
            {
                return this.states;
            }
        }
        #endregion

        // internal before!
        internal void AddState(State state)
        {
            if (state == null)
            {
                return;
            }

            //Some validations here
            this.states.Add(state);
        }

    }
}