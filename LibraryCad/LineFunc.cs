﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;

namespace LibraryCad
{
    public class LineFunc
    {
        /// <summary>
        /// Hàm convert selectionSet thành list Line
        /// </summary>
        /// <param name="selectSet">Các đối tượng được chọn</param>
        /// <param name="doc">Document</param>
        /// <returns>List Line</returns>
        public static List<Line> SelectionSetToListLine(Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                var lines = new List<Line>();
                var typeValue = new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start, "LINE")
                };
                var slft = new SelectionFilter(typeValue);
                var objectIds = SubFunc.GetListSelection(doc, "- Chọn các đoạn thẳng muốn tính tổng", slft);
                if (objectIds == null) return null;
                // Step through the objects in the selection set
                foreach (ObjectId objId in objectIds)
                {
                    var line = trans.GetObject(objId, OpenMode.ForRead) as Line;
                    if (line != null)
                    {
                        lines.Add(line);
                    }
                }
                return lines;
            }
        }
    }
}
