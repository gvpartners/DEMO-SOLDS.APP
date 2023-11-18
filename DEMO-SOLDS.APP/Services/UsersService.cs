using Microsoft.Extensions.Configuration;
using R2.DEMO.APP.Context;
using DEMO_SOLDS.APP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace DEMO_SOLDS.APP.Services
{
    public class UserService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;


        public UserService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public List<UsersModel> GetAllUsers()
        {
            var userList = _context.AspNetUsers
                .Where(u => u.IsDeleted != true)
                .Select(u => new UsersModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    FirstLastName = u.FirstLastName,
                    SecondLastName = u.SecondLastName,
                    Email = u.Email,
                    Prefix = u.Prefix,
                    IsDeleted = u.IsDeleted,
                    IsApproved = u.IsApproved,
                    Phone = u.Phone,
                })
                .ToList();

            return userList;
        }
        public Users EditUser(Users newUser)
        {
            var findUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == newUser.Id);

            findUser.Name = newUser.Name;
            findUser.FirstLastName = newUser.FirstLastName;
            findUser.SecondLastName = newUser.SecondLastName;
            findUser.IsApproved = newUser.IsApproved;
            findUser.Prefix = newUser.Prefix;
            _context.SaveChanges();
            return findUser;
        }
        public void RemoveItem(Guid Id)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == Id);
            user.IsDeleted = true;
            _context.SaveChanges();
        }

        public UsersModel GetUserById(Guid Id)
        {
            var user = _context.AspNetUsers
                .FirstOrDefault(u => u.Id == Id);

            if (user != null)
            {
                return new UsersModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    FirstLastName = user.FirstLastName,
                    SecondLastName = user.SecondLastName,
                    Email = user.Email,
                    Phone = user.Phone,
                };
            }

            return new UsersModel();
        }
        public void UpdateUserById(Users updatedUser)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == updatedUser.Id);
            user.Name = updatedUser.Name;
            user.FirstLastName = updatedUser.FirstLastName;
            user.SecondLastName= updatedUser.SecondLastName;
            user.Phone = updatedUser.Phone;

            _context.SaveChanges();
        }

        public void UpdatePasswordById(Guid Id, string Password)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == Id);
            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
            string passwordHash = ComputeHmacSha256(Password, jwt.Key);
            user.Password = passwordHash;
            _context.SaveChanges();
        }
        private string ComputeHmacSha256(string password, string key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
