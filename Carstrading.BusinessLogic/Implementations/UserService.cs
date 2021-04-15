using System;
using System.Collections.Generic;
using System.Linq;
using Carstrading.BusinessLogic.Helpers;
using Carstrading.BusinessLogic.Interfaces;
using Carstrading.Core;
using Carstrading.Core.Entities;

namespace Carstrading.BusinessLogic.Implementations
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public User Create(User user, string password)
        {
            var validEmail = Common.EmailIsValid(user.EmailAddress);

            if (!validEmail) throw new AppException("Format email invalid");

            Common.ValidatePassword(password, out string passwordValidationError);

            if (!string.IsNullOrEmpty(passwordValidationError)) throw new AppException(passwordValidationError);

            if (_userRepository.GetAll().Any(x => x.EmailAddress == user.EmailAddress))
                throw new AppException("Username \"" + user.EmailAddress + "\" is already taken");

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _userRepository.Insert(user);
            _userRepository.SaveChanges();

            return user;
        }

        public void DeleteUser(int id)
        {
            var repo = _userRepository.Get(id);

            if (repo != null)
                _userRepository.Delete(repo);
        }

        public User GetUser(int id)
        {
            return _userRepository.Get(id);
        }

        public User GetByUsername(string username)
        {
            return _userRepository.GetAll().Where(u => u.EmailAddress == username).FirstOrDefault();
        }

        public IEnumerable<User> GetUsers()
        {
            return _userRepository.GetAll();
        }

        public void InsertUser(User user)
        {
            _userRepository.Insert(user);
        }

        public void UpdateUser(User userParam, string password = null)
        {
            var user = _userRepository.Get(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.EmailAddress != user.EmailAddress)
            {
                // username has changed so check if the new username is already taken
                if (_userRepository.GetAll().Any(x => x.EmailAddress == userParam.EmailAddress))
                    throw new AppException("Username " + userParam.EmailAddress + " is already taken");
            }

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _userRepository.Update(user);
            _userRepository.SaveChanges();
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _userRepository.GetAll().SingleOrDefault(x => x.EmailAddress == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public User GetByUsernameAndResetPassToken(string emailAddress, string resetPasswordToken)
        {
            return _userRepository.GetAll()
                .Where(u => u.EmailAddress == emailAddress && u.ResetPasswordToken == resetPasswordToken)
                .FirstOrDefault();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public User GetUserById(int id)
        {
            return _userRepository.GetAll().Where(x => x.Id == id).First();
        }

        public User GetUserByEmailAsync(string email)
        {
            return _userRepository.GetAll().Where(x => x.EmailAddress.ToLower().Equals(email)).First();
        }
    }
}
