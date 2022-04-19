using System.Web;
using System.Web.Mvc;

namespace Nhom19_QuanLyLuuTruDienTu
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
