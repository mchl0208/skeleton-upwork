using System;
using System.Collections.Generic;
using Carstrading.Core.Entities;

namespace Carstrading.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        User GetUser(int id);
        User Authenticate(string username, string password);
        User Create(User user, string password);
        void InsertUser(User user);
        void UpdateUser(User user, string password);
        void DeleteUser(int id);
        User GetByUsername(string username);
        User GetByUsernameAndResetPassToken(string emailAddress, string resetPasswordToken);
        User GetUserById(int id);
    }
}
