using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;

namespace LibraryCad
{
    public class DimensionFunc
    {
        /// <summary>
        /// Hàm lấy dimension trong các đối tượng được chọn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static List<Dimension> GetDimensions(Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                try
                {
                    List<Dimension> dimensions = new List<Dimension>();
                    var typeValues = new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start, "DIMENSION")
                    };
                    var slft = new SelectionFilter(typeValues);
                    var selSet = SubFunc.GetListSelection(doc, "- Chọn các dimension mà bạn muốn tính tồng: ", slft);
                    if (selSet == null) return null;
                    foreach (ObjectId sel in selSet)
                    {
                        Dimension dimension = trans.GetObject(sel, OpenMode.ForRead) as Dimension;
                        if (dimension != null)
                        {
                            dimensions.Add(dimension);
                        }
                    }
                    trans.Commit();
                    return dimensions;
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage(ex.Message);
                    trans.Abort();
                    return null;
                }
            }
            return null;
        }
    }
}
