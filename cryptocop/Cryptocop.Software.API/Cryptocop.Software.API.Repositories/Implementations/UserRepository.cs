using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.DTOs;
using Cryptocop.Software.API.Models.Entities;
using Cryptocop.Software.API.Repositories.Contexts;
using Cryptocop.Software.API.Repositories.Helpers;
using System;
using System.Linq;
using System.Text;

namespace Cryptocop.Software.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly CryptocopDbContext _dbContext;
        public UserRepository(CryptocopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public UserDto CreateUser(RegisterInputModel register)
        {
            // Check if user already exists
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == register.Email);
            if (user != null)
            {
                throw new Exception("User already exists.");
            }

            // Hash the password and create a new token
            var password = HashingHelper.HashPassword(register.Password);
            var token = new JwtToken();
            _dbContext.JwtTokens.Add(token);
            _dbContext.SaveChanges();

            var newUser = new User
            {
                Email = register.Email,
                FullName = register.FullName,
                HashedPassword = password,
            };
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            var createdUser = _dbContext.Users.FirstOrDefault(u => u.Email == register.Email);

            return new UserDto
            {
                Id = createdUser.Id,
                Email = createdUser.Email,
                FullName = createdUser.FullName,
                TokenId = token.Id
            };
        }

        public UserDto AuthenticateUser(LoginInputModel login)
        {
            var user = _dbContext.Users.FirstOrDefault(u =>
                u.Email == login.Email &&
                u.HashedPassword == HashingHelper.HashPassword(login.Password));
            if (user == null) { return null; }

            var token = new JwtToken();
            _dbContext.JwtTokens.Add(token);
            _dbContext.SaveChanges();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                TokenId = token.Id
            };
        }
    }
}