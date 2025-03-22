using System.Collections.Generic;
using System.Threading.Tasks;
using PhapClinicX.Models;

public interface IMenuService
{
    Task<List<Menu>> GetActiveMenusAsync(); // Lấy danh sách menu hoạt động
}
