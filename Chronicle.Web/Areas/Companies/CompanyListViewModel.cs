using Chronicle.Entities;

namespace Chronicle.Web.Areas.Companies
{
    public class CompanyListViewModel
    {
        public IEnumerable<Company> Companies { get; set; } = new List<Company>();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    }
}
