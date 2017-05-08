
namespace EIV.Demo.Model
{
    using Base;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Windows.Input;

    //public abstract class MenuItem
    public sealed class MenuItem : EntityBase
    {
        private string name;

        // 'Header' prop in Telerik RadMenuItem
        private string text;

        private string toolTip;
        private string iconUrl;

        private bool isEnabled;
        private bool isHidden;
        private bool isSeparator;

        private ObservableCollection<MenuItem> subItems;

        private ICommand onSelected;

        private MenuItem parentMenu;

        private string className = string.Empty;
        private string classMethod = string.Empty;

        // The property 'SubItems' on type 'EIV.Demo.Model.MenuItem' returned a null value. The input stream contains collection items which cannot be added if the instance is null.\r\n",
        // OData requires a public and parameterless constructor!
        public MenuItem()
        {
            this.subItems = new ObservableCollection<MenuItem>();
        }

        // if there is no setter, then OData never shows this property on the browser
        [Required]
        public string Name { get { return this.name; } set { this.name = value;  } }

        [Required]
        public string Text { get { return this.text; } set { this.text = value; } }

        /* Proxy class does not work if we use Uri object
        public Uri IconUrl */
        public string IconUrl
        {
            get { return this.iconUrl; }
            set { this.iconUrl = value; }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
            }
        }

        public bool IsHidden
        {
            get
            {
                return this.isHidden;
            }
            set
            {
                this.isHidden = value;
            }
        }

        public bool IsSeparator
        {
            get
            {
                return this.isSeparator;
            }
            set
            {
                this.isSeparator = value;
            }
        }

        public string ToolTip
        {
            get
            {
                return this.toolTip;
            }
            set
            {
                this.toolTip = value;
            }
        }

        public string ClassName
        {
            get
            {
                return this.className;
            }
            set
            {
                this.className = value;
            }
        }

        public string ClassMethod
        {
            get
            {
                return this.classMethod;
            }
            set
            {
                this.classMethod = value;
            }
        }

        // The complex type 'EIV.Demo.Model.MenuItem' has a reference to itself through the property 'Parent'. A recursive loop of complex types is not allowed.
        // Nombre del parámetro: propertyInfo
        public MenuItem ParentMenu
        { get { return this.parentMenu; }
          set { this.parentMenu = value; }
        }

        public ICommand OnSelected
        {
            get
            {
                if (this.onSelected == null)
                {
                    this.onSelected = new MenuCommand(this.OnItemSelected, this.ItemCanBeSelected);
                }
                return this.onSelected;
            }
        }

        public ObservableCollection<MenuItem> SubItems
        {
            get
            {
                return this.subItems;
            }
        }

        public void AddSubMenu(MenuItem menuItem)
        {
            this.subItems.Add(menuItem);
        }

        public void RemoveSubMenu(MenuItem menuItem)
        {
            if (this.subItems.Contains(menuItem))
            {
                this.subItems.Remove(menuItem);
            }
        }

        // public abstract void OnItemSelected();
        public void OnItemSelected()
        { }

        public bool ItemCanBeSelected()
        {
            return true;
        }
    }
}