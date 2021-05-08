// ============================
// CoWin service main Interface
// ============================

using System.Threading.Tasks;

namespace CoWinService.Domain.Interfaces
{
    public interface ICoWinServer
    {
        // Gets the vaccine center data by pincode and date
        Task SearchByPincode(string pincode, string date);

        // Gets the vaccine center data by district and date
        Task SearchByDistrict(string districtCode, string date);
    }
}
