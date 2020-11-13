using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Services.Interfaces;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.DTOs;

namespace Cryptocop.Software.API.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;

        public AccountService(IUserRepository userRepository, ITokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
        }

        public UserDto CreateUser(RegisterInputModel register)
        {
            return _userRepository.CreateUser(register);
        }

        public UserDto AuthenticateUser(LoginInputModel login)
        {
            return _userRepository.AuthenticateUser(login);
        }

        public void Logout(int tokenId)
        {
            // Void the JWT token
            _tokenRepository.VoidToken(tokenId);
        }
    }
}