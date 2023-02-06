using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.Models;
using LibraryCad.ObjectsFunc.DimensionObject;
using LibraryCad.ObjectsFunc.PolylineObject;
using LibraryCad.Sub;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using acAp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadProject.AcadManip.WorkWithObject
{
    public class WorkWithPolyline
    {
        [CommandMethod("loadjson")]
        public void LoadJson()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            using (StreamReader r = new StreamReader(@"D:\c#\ATC\AddinLearning\appsetting.json"))
            {
                string json = r.ReadToEnd();
                List<AppSetting> items = JsonConvert.DeserializeObject<List<AppSetting>>(json);
                foreach (AppSetting item in items)
                {
                    doc.Editor.WriteMessage("\nData: " + item.Data + "\nStatus: " + item.Status);
                }
            }
        }
    }
}
