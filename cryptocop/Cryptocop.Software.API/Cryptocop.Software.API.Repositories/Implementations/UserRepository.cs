using System;
using System.Linq;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.DTOs;
using Cryptocop.Software.API.Models.Entities;
using Cryptocop.Software.API.Repositories.Contexts;
using System.Text;
using System.Collections.Generic;

namespace Cryptocop.Software.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly CryptocopDbContext _dbContext;
        private string _salt = "00209b47-08d7-475d-a0fb-20abf0872ba0";

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
            var password = HashPassword(register.Password);
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
                u.HashedPassword == HashPassword(login.Password));
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

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: CreateSalt(),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        }

        private byte[] CreateSalt() =>
            Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(_salt)));
    }
}