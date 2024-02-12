using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DataHelper
/// </summary>
namespace WebApplication
{
    public class DataHelper
    {

        public static List<FileSystemData> CreateDataSource(string GCRecord, string user, string tablename)
        {
            MyDataModule cs = new MyDataModule();
            List<FileSystemData> list = new List<FileSystemData>();
            DataTable dt = new DataTable();
            SqlParameter[] param = { new SqlParameter("@GCRecord", GCRecord.ToString()),
        new SqlParameter("@username", user.ToString()),
        new SqlParameter("@tablename", tablename.ToString())};
            dt = cs.GetRelatedResources("spGetFileSystem", param);
            dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
            list = (from DataRow dr in dt.Rows
                    select new FileSystemData()
                    {
                        Id = Convert.ToInt32(dr["ID"]),
                        ParentId = Convert.ToInt32(dr["ParentId"]),
                        IsFolder = (bool)dr["IsFolder"],
                        Name = dr["Name"].ToString(),
                        Data = !string.IsNullOrEmpty(dr["Data"].ToString()) ? (byte[])dr["Data"] : null,
                        LastWriteTime = Convert.ToDateTime(dr["LastWriteTime"]),
                        GCRecord = dr["GCRecord"].ToString(),
                        LastUpdateBy = dr["LastUpdateBy"].ToString()
                    }).ToList();

            return list;
        }
        public static List<FileSystemData> Init(){
                        List<FileSystemData> list = new List<FileSystemData>();

        FileSystemData item = new FileSystemData();
        item.Id = 0;
            item.ParentId = -1;
            item.Name = "Folder";
            item.IsFolder = true;
            item.LastWriteTime = null;
            item.LastUpdateBy = null;
            list.Add(item);
            return list;
        }
    }
    public class FileSystemData
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public bool IsFolder { get; set; }
        public Byte[] Data { get; set; }
        public DateTime? LastWriteTime { get; set; }
        public string GCRecord { get; set; }
        public string LastUpdateBy { get; set; }
    }
}