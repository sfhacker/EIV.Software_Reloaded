
namespace EIV.TrackableEntities
{
    using System;
    public sealed class TrackableEntity
    {
        public enum Operation
        {
            Unknown = 0,
            Insert,
            Update,
            Delete,
        };

        private Operation operationType;

        // Applicable to Insert method only!
        private string itemName = string.Empty;

        private object item;
        public TrackableEntity(object entity, Operation type = Operation.Unknown, string entityName = null)
        {
            if (entity == null)
            {
                return;
            }

            // OData entities do NOT implement this interface!
            //if (entity is IComparable)
            this.item = entity;
            this.operationType = type;
            this.itemName = entityName;
        }

        public Operation OperationType
        {
            get
            {
                return this.operationType;
            }
        }

        public object Entity
        {
            get
            {
                return this.item;
            }
        }

        // When the class name (e.g. State) and the controller name (e.g. States)
        // do not match!
        public string EntityName
        {
            get
            {
                return this.itemName;
            }
        }

        // OData entities do NOT implement 'IComparable' interface!
        // Should I use 'TrackableEntity' instead of 'object'
        public bool CompareTo(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            // should we throw an exception?
            if (entity.GetType() != this.item.GetType())
            {
                return false;
            }

            // what about operation type?
            bool areEqual = entity.Equals(this.item);

            return areEqual;
        }
    }
}