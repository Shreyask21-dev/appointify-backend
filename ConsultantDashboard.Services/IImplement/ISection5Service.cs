using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;

namespace ConsultantDashboard.Services.IImplement
{
    public interface ISection5Service
    {
        Task<Section5ContentDto> GetContentAsync();
        Task<bool> SaveOrUpdateContentAsync(Section5ContentDto dto);
    }
}
