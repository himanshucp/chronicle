using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace Chronicle.Lookups
{
    /// <summary>
    /// Interface for UI dropdown lookups
    /// </summary>
    public interface ILookup
    {
        // Company lookups
        List<SelectListItem> GetCompanyItems(bool includeEmpty = true, string emptyText = "-- Select Company --");
        List<SelectListItem> GetCompanyItemsByLocation(string location = null, bool includeEmpty = true);
        List<SelectListItem> GetCompanyLocationItems(bool includeEmpty = true);
        List<SelectListItem> GetActiveCompanyItems(bool includeEmpty = true);

        List<SelectListItem> GetDisciplineItems(bool includeEmpty = true, string emptyText = "-- Select Discipline --");

        List<SelectListItem> GetCompanyRoleItems(bool includeEmpty = true, string emptyText = "-- Select Company Role --");

        List<SelectListItem> GetHierarchyLevelsItems(bool includeEmpty = true, string emptyText = "-- Select Hierarchy Levels --");


        List<SelectListItem> StatusItems { get; }

        // Can be extended with additional lookup methods
    }
}
