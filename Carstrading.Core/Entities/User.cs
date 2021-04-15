using System;
namespace Carstrading.Core.Entities
{
    public class User: BaseEntity
    {
        public User()
        {
        }

        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime LastLogged { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string ResetPasswordToken { get; set; }
    }
}
