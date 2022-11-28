using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topic1.WorkWithObject
{
    public class WorkWithFile
    {
        private static string[] filepaths = new string[]
        {
            "E:\\BanVe\\DWG\\02. B2F ~ Block B ~ General Note.dwg",
            "E:\\BanVe\\DWG\\03. B2F ~ Block B ~ Lower slab at B2F_Segment.dwg",
            "E:\\BanVe\\DWG\\04. B2F ~ Block B ~ General Layout Sec 1-3.dwg"
        };

        [CommandMethod("SaveMultiFile")]
        public static void SaveMultiFile()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            foreach (string path in filepaths)
            {
                Database newDb = new Database(false, true);
                using (newDb)
                {
                    try
                    {
                        newDb.ReadDwgFile(path, FileOpenMode.OpenForReadAndAllShare, false, null);
                        newDb.SaveAs(path, DwgVersion.Current);
                    }
                    catch
                    {
                        doc.Editor.WriteMessage("Không tìm thấy file!");
                    }
                }
            }
        }
    }
}
