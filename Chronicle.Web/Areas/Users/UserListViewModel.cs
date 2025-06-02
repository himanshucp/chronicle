using Chronicle.Entities;

namespace Chronicle.Web.Areas.Users
{
    /// <summary>
    /// User list view model
    /// </summary>
    public class UserListViewModel
    {
        public IEnumerable<User> Users { get; set; } = new List<User>();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

}
