using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;

namespace ConsultantDashboard.Services.IImplement
{
    public interface ILocationService
    {
        Task<LocationDTOs?> GetLocationAsync();
        Task SaveLocationAsync(LocationDTOs dto);
    }
}
