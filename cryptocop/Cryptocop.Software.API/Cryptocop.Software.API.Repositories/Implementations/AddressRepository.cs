using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.DTOs;
using Cryptocop.Software.API.Models.Exceptions;
using Cryptocop.Software.API.Repositories.Contexts;
using Cryptocop.Software.API.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Cryptocop.Software.API.Repositories.Implementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly CryptocopDbContext _dbContext;
        private IMapper _mapper;

        public AddressRepository(CryptocopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddAddress(string email, AddressInputModel address)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            var newAddress = new Address
            {
                UserId = user.Id,
                StreetName = address.StreetName,
                HouseNumber = address.HouseNumber,
                ZipCode = address.ZipCode,
                Country = address.Country,
                City = address.City
            };
            _dbContext.Addresses.Add(newAddress);
            _dbContext.SaveChanges();
        }

        public IEnumerable<AddressDto> GetAllAddresses(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var addresses = _mapper.Map<IEnumerable<AddressDto>>(_dbContext.Addresses.Where(a => a.UserId == user.Id));
            return addresses;
        }

        public void DeleteAddress(string email, int addressId)
        {
            // Find the user
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            var address = _dbContext.Addresses.FirstOrDefault(a => a.Id == addressId && a.UserId == user.Id);
            if (address == null)
            {
                { throw new ResourceNotFoundException($"Address with id {addressId} not found."); };
            }
            _dbContext.Addresses.Remove(address);
            _dbContext.SaveChanges();
        }
    }
}