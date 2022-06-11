using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom19_QuanLyLuuTruDienTu.models
{
    public class IndexVM
    {
        public List<File> Files { get; set; }
        public List<Folder> Folders { get; set; }
    }
}