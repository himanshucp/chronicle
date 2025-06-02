namespace Chronicle.Web.Code
{
    public class BaseModel
    {
        // Base class to all viewmodels

        private readonly IHttpContextAccessor _httpContextAccessor =
            ServiceLocator.Resolve<IHttpContextAccessor>();

        public string Referer
        {
            get => _httpContextAccessor.PostedReferer() ??
                                       _httpContextAccessor.Referer();
        }
    }
}
