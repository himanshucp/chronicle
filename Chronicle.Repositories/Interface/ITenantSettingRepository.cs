using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface ITenantSettingRepository : IRepository<TenantSetting,int>
    {
        Task<TenantSetting> GetSettingAsync(int tenantId, string settingKey);
        Task<IEnumerable<TenantSetting>> GetAllSettingsForTenantAsync(int tenantId);
        Task<string> GetSettingValueAsync(int tenantId, string settingKey, string defaultValue = null);
        Task UpdateSettingAsync(int tenantId, string settingKey, string settingValue);
    }
}
