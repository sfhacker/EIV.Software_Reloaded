
namespace EIV.Demo.Data.Repository
{
    using EIV.Demo.Data.Base;
    using EIV.Demo.Data.Interface;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class UserRepository : Repository<User>, IUserRepository
    {
        private IList<User> Usuarios = null;
        public UserRepository()
        {
            this.Reset();
            this.PopulateClients();
        }

        public override void Add(User entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            if (this.Usuarios == null)
            {
                throw new InvalidOperationException();
            }

            this.Usuarios.Add(
                new User()
                {
                    // 'Id' should be random or calculated!
                    Id = entity.Id,
                    Name = entity.Name
                }
            );
        }

        public override IList<User> GetAll()
        {
            return this.Usuarios;
        }

        public override User GetById(int id)
        {
            if (id < 1)
            {
                throw new ArgumentNullException();
            }

            if (this.Usuarios == null)
            {
                throw new InvalidOperationException();
            }

            return this.Usuarios.Where(x => x.Id == id).SingleOrDefault();
        }

        public User GetByName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException();
            }

            if (this.Usuarios == null)
            {
                throw new InvalidOperationException();
            }

            return this.Usuarios.Where(x => x.Name.Equals(userName)).SingleOrDefault();
        }

        public User Authenticate(string userName, string userPassword)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }
            if (string.IsNullOrEmpty(userPassword))
            {
                return null;
            }

            User thisUser = this.Usuarios.Where(x => x.Name.Equals(userName)).SingleOrDefault();
            if (thisUser == null)
            {
                return null;
            }

            bool rst = thisUser.ComparePassword(userPassword);
            if (!rst)
            {
                return null;
            }

            return thisUser;
        }

        private void Reset()
        {
            if (this.Usuarios == null)
            {
                this.Usuarios = new List<User>();
            }
        }

        private void PopulateClients()
        {
            if (this.Usuarios == null)
            {
                return;
            }
            if (this.Usuarios.Count != 0)
            {
                return;
            }

            MenuItem adminFileMenu = new MenuItem();
            adminFileMenu.Name = "_File";
            adminFileMenu.Text = "File";

            MenuItem adminNewMenu = new MenuItem();
            adminNewMenu.Name = "_New";
            adminNewMenu.Text = "New";

            MenuItem adminOpenMenu = new MenuItem();
            adminOpenMenu.Name = "_Open";
            adminOpenMenu.Text = "Open";

            MenuItem adminNewWebSiteMenu = new MenuItem();
            adminNewWebSiteMenu.Name = "_Website";
            adminNewWebSiteMenu.Text = "Website";

            User adminUser = new User()
            {
                Id = 1,
                Name = "Administrador",
            };
            adminUser.SetPassword("Admin1234");

            adminFileMenu.IconUrl = "C:\\Software\\logo.png"; // new Uri("C:\\Software\\logo.png");

            adminNewMenu.AddSubMenu(adminNewWebSiteMenu);

            adminFileMenu.AddSubMenu(adminNewMenu);
            adminFileMenu.AddSubMenu(adminOpenMenu);

            adminUser.AddMenuItem(adminFileMenu);

            User user1 = new User()
            {
                Id = 2,
                Name = "Usuario 1"
            };

            user1.SetPassword("User1234");

            this.Usuarios.Add(adminUser);
            this.Usuarios.Add(user1);
        }
    }
}