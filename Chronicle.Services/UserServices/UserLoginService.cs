using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories.UsersRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.UserServices
{
    public class UserLoginService : IUserLoginService
    {
        private readonly IUserLoginRepository _userLoginRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserLoginService(
            IUserLoginRepository userLoginRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userLoginRepository = userLoginRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<UserLogin>> GetUserLoginByIdAsync(int userLoginId, int tenantId)
        {
            try
            {
                var userLogin = await _userLoginRepository.GetByIdWithUserAsync(userLoginId, tenantId);
                if (userLogin == null)
                {
                    return ServiceResult<UserLogin>.FailureResult("User login not found");
                }

                return ServiceResult<UserLogin>.SuccessResult(userLogin);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserLogin>.FailureResult($"Error retrieving user login: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UserLogin>> GetByProviderAsync(string loginProvider, string providerKey, int tenantId)
        {
            try
            {
                var userLogin = await _userLoginRepository.GetByProviderAsync(loginProvider, providerKey, tenantId);
                if (userLogin == null)
                {
                    return ServiceResult<UserLogin>.FailureResult("User login not found");
                }

                return ServiceResult<UserLogin>.SuccessResult(userLogin);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserLogin>.FailureResult($"Error retrieving user login by provider: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserLogin>>> GetByUserIdAsync(int userId, int tenantId)
        {
            try
            {
                var userLogins = await _userLoginRepository.GetByUserIdAsync(userId, tenantId);
                return ServiceResult<IEnumerable<UserLogin>>.SuccessResult(userLogins);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserLogin>>.FailureResult($"Error retrieving user logins: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserLogin>>> GetByLoginProviderAsync(string loginProvider, int tenantId)
        {
            try
            {
                var userLogins = await _userLoginRepository.GetByLoginProviderAsync(loginProvider, tenantId);
                return ServiceResult<IEnumerable<UserLogin>>.SuccessResult(userLogins);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserLogin>>.FailureResult($"Error retrieving user logins by provider: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UserLogin>> GetUserLoginAsync(int userId, string loginProvider, int tenantId)
        {
            try
            {
                var userLogin = await _userLoginRepository.GetUserLoginAsync(userId, loginProvider, tenantId);
                if (userLogin == null)
                {
                    return ServiceResult<UserLogin>.FailureResult("User login not found");
                }

                return ServiceResult<UserLogin>.SuccessResult(userLogin);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserLogin>.FailureResult($"Error retrieving user login: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateUserLoginAsync(UserLogin userLogin, int tenantId)
        {
            try
            {
                // Set tenant ID
                userLogin.TenantID = tenantId;

                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userLogin.UserID, tenantId);
                if (user == null)
                {
                    return ServiceResult<int>.FailureResult("User not found");
                }

                // Validate login provider
                var providerValidation = await ValidateLoginProviderAsync(userLogin.LoginProvider, userLogin.ProviderKey);
                if (!providerValidation.Success)
                {
                    return ServiceResult<int>.FailureResult(providerValidation.Message);
                }

                // Check if login already exists
                var existingLogin = await _userLoginRepository.GetByProviderAsync(userLogin.LoginProvider, userLogin.ProviderKey, tenantId);
                if (existingLogin != null)
                {
                    return ServiceResult<int>.FailureResult("Login provider with this key already exists");
                }

                // Check if user already has this login provider
                var userProviderLogin = await _userLoginRepository.GetUserLoginAsync(userLogin.UserID, userLogin.LoginProvider, tenantId);
                if (userProviderLogin != null)
                {
                    return ServiceResult<int>.FailureResult("User already has a login for this provider");
                }

                // Set default values
                userLogin.CreatedDate = DateTime.UtcNow;
                userLogin.ModifiedDate = DateTime.UtcNow;
                userLogin.IsActive = true;

                _unitOfWork.BeginTransaction();

                int userLoginId = await _userLoginRepository.InsertAsync(userLogin);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(userLoginId, "User login created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating user login: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(UserLogin userLogin, int tenantId)
        {
            try
            {
                // Ensure tenant ID matches
                userLogin.TenantID = tenantId;

                // Check if user login exists
                var existingUserLogin = await _userLoginRepository.GetByIdAsync(userLogin.UserLoginID, tenantId);
                if (existingUserLogin == null)
                {
                    return ServiceResult<bool>.FailureResult("User login not found");
                }

                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userLogin.UserID, tenantId);
                if (user == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                // Validate login provider
                var providerValidation = await ValidateLoginProviderAsync(userLogin.LoginProvider, userLogin.ProviderKey);
                if (!providerValidation.Success)
                {
                    return ServiceResult<bool>.FailureResult(providerValidation.Message);
                }

                // Check if another login exists with same provider and key
                var duplicateLogin = await _userLoginRepository.GetByProviderAsync(userLogin.LoginProvider, userLogin.ProviderKey, tenantId);
                if (duplicateLogin != null && duplicateLogin.UserLoginID != userLogin.UserLoginID)
                {
                    return ServiceResult<bool>.FailureResult("Login provider with this key already exists");
                }

                userLogin.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _userLoginRepository.UpdateAsync(userLogin);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User login updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating user login: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int userLoginId, int tenantId)
        {
            try
            {
                // Check if user login exists
                var existingUserLogin = await _userLoginRepository.GetByIdAsync(userLoginId, tenantId);
                if (existingUserLogin == null)
                {
                    return ServiceResult<bool>.FailureResult("User login not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _userLoginRepository.DeleteAsync(userLoginId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User login deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting user login: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteByUserAndProviderAsync(int userId, string loginProvider, int tenantId)
        {
            try
            {
                // Check if user login exists
                var userLogin = await _userLoginRepository.GetUserLoginAsync(userId, loginProvider, tenantId);
                if (userLogin == null)
                {
                    return ServiceResult<bool>.FailureResult("User login not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _userLoginRepository.DeleteByUserAndProviderAsync(userId, loginProvider, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User login removed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error removing user login: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AddLoginToUserAsync(int userId, string loginProvider, string providerKey, string providerDisplayName, int tenantId)
        {
            try
            {
                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userId, tenantId);
                if (user == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                // Validate login provider
                var providerValidation = await ValidateLoginProviderAsync(loginProvider, providerKey);
                if (!providerValidation.Success)
                {
                    return ServiceResult<bool>.FailureResult(providerValidation.Message);
                }

                // Check if login already exists
                var existingLogin = await _userLoginRepository.GetByProviderAsync(loginProvider, providerKey, tenantId);
                if (existingLogin != null)
                {
                    return ServiceResult<bool>.FailureResult("Login provider with this key already exists");
                }

                // Check if user can add this login
                var canAddResult = await CanAddLoginToUserAsync(userId, loginProvider, tenantId);
                if (!canAddResult.Success || !canAddResult.Data)
                {
                    return ServiceResult<bool>.FailureResult("User already has a login for this provider");
                }

                var userLogin = new UserLogin
                {
                    UserID = userId,
                    TenantID = tenantId,
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey,
                    ProviderDisplayName = providerDisplayName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    IsActive = true
                };

                _unitOfWork.BeginTransaction();

                await _userLoginRepository.InsertAsync(userLogin);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Login added to user successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error adding login to user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RemoveLoginFromUserAsync(int userId, string loginProvider, int tenantId)
        {
            try
            {
                var result = await DeleteByUserAndProviderAsync(userId, loginProvider, tenantId);
                return result;
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error removing login from user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ExistsAsync(string loginProvider, string providerKey, int tenantId)
        {
            try
            {
                bool exists = await _userLoginRepository.ExistsAsync(loginProvider, providerKey, tenantId);
                return ServiceResult<bool>.SuccessResult(exists);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error checking login existence: {ex.Message}");
            }
        }

        public async Task<ServiceResult<User>> FindUserByLoginAsync(string loginProvider, string providerKey, int tenantId)
        {
            try
            {
                var user = await _userLoginRepository.FindUserByLoginAsync(loginProvider, providerKey, tenantId);
                if (user == null)
                {
                    return ServiceResult<User>.FailureResult("User not found for this login");
                }

                return ServiceResult<User>.SuccessResult(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.FailureResult($"Error finding user by login: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<string>>> GetLoginProvidersAsync(int tenantId)
        {
            try
            {
                var providers = await _userLoginRepository.GetLoginProvidersAsync(tenantId);
                return ServiceResult<IEnumerable<string>>.SuccessResult(providers);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<string>>.FailureResult($"Error retrieving login providers: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetUserLoginCountAsync(int userId, int tenantId)
        {
            try
            {
                int count = await _userLoginRepository.GetUserLoginCountAsync(userId, tenantId);
                return ServiceResult<int>.SuccessResult(count);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.FailureResult($"Error getting user login count: {ex.Message}");
            }
        }

        public async Task<IEnumerable<UserLogin>> GetUserLoginsAsync(int tenantId)
        {
            return await _userLoginRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<UserLogin>>> GetPagedUserLoginsAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var userLogins = await _userLoginRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<UserLogin>>.SuccessResult(userLogins);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<UserLogin>>.FailureResult($"Error retrieving user logins: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ValidateLoginProviderAsync(string loginProvider, string providerKey)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(loginProvider))
                {
                    return ServiceResult<bool>.FailureResult("Login provider cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(providerKey))
                {
                    return ServiceResult<bool>.FailureResult("Provider key cannot be empty");
                }

                if (loginProvider.Length > 128)
                {
                    return ServiceResult<bool>.FailureResult("Login provider cannot exceed 128 characters");
                }

                if (providerKey.Length > 128)
                {
                    return ServiceResult<bool>.FailureResult("Provider key cannot exceed 128 characters");
                }

                // Validate allowed providers (you can customize this list)
                var allowedProviders = new[] { "Google", "Microsoft", "Facebook", "Twitter", "GitHub", "LinkedIn" };
                if (!allowedProviders.Contains(loginProvider, StringComparer.OrdinalIgnoreCase))
                {
                    return ServiceResult<bool>.FailureResult($"Login provider '{loginProvider}' is not supported");
                }

                return ServiceResult<bool>.SuccessResult(true, "Login provider is valid");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error validating login provider: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CanAddLoginToUserAsync(int userId, string loginProvider, int tenantId)
        {
            try
            {
                // Check if user already has this login provider
                var existingLogin = await _userLoginRepository.GetUserLoginAsync(userId, loginProvider, tenantId);
                if (existingLogin != null)
                {
                    return ServiceResult<bool>.SuccessResult(false, "User already has a login for this provider");
                }

                return ServiceResult<bool>.SuccessResult(true, "User can add this login provider");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error checking if user can add login: {ex.Message}");
            }
        }
    }
}
