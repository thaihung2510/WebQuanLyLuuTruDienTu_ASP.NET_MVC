//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nhom19_QuanLyLuuTruDienTu.models
{
    using System;
    using System.Collections.Generic;
    
    public partial class File
    {
        public int FileID { get; set; }
        public int AccountID { get; set; }
        public Nullable<int> TagID { get; set; }
        public int FileTypeID { get; set; }
        public Nullable<int> FolderID { get; set; }
        public int TimeID { get; set; }
        public string Location { get; set; }
        public string FileName { get; set; }
        public Nullable<double> Size { get; set; }
        public Nullable<double> SizeInMB
        {
            get { return Math.Round((double)(Size / 1048576),2); } //convert to MB
        }
        public string Description { get; set; }
        public Nullable<bool> Status { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual Account Account1 { get; set; }
        public virtual FileType FileType { get; set; }
        public virtual FileType FileType1 { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual Folder Folder1 { get; set; }
        public virtual TagName TagName { get; set; }
        public virtual TimeKeep TimeKeep { get; set; }
        public virtual TimeKeep TimeKeep1 { get; set; }
    }
}
