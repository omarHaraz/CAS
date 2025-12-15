using CAS.Models.Users;
using System;

namespace CAS.Factories
{
    public class UserFactory : IUserFactory
    {
        public User CreateUser(string userType)
        {
            switch (userType.ToLower())
            {
                case "doctor":
                    return new Doctor
                    {
                        Role = "Doctor"
                    };

                case "patient":
                    return new Patient
                    {
                        Role = "Patient"
                    };

                case "admin":
                    return new Admin
                    {
                        Role = "Admin"
                    };

                default:
                    throw new ArgumentException("Invalid user type. Please choose Doctor, Patient, or Admin.");
            }
        }
    }
}