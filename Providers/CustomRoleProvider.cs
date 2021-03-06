﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using telegramBod.Models;

namespace telegramBod.Providers
{
    public class CustomRoleProvider:RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            string[] roles = new string[] { };
            using (botEntities3 db = new botEntities3())
            {
                // Получаем пользователя
                Users user = db.Users.FirstOrDefault(u => u.Email == username);
                if (user != null && user.Roles != null)
                {
                    // получаем роль
                    roles = new string[] { user.Roles.Names };
                }
                return roles;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (botEntities3 db = new botEntities3())
            {
                Users user;
                // Получаем пользователя
                try
                {
                  user = db.Users.FirstOrDefault(u => u.Email == username);
                }
                catch
                {
                    user = null;
                }

                if (user != null && user.Roles != null && user.Roles.Names == roleName)
                    return true;
                else
                    return false;
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
