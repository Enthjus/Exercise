using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace Topic1.WorkWithStyle
{
    public class AtcTableStyle
    {
        [CommandMethod("CTWS")]
        static public void CreateTableWithStyle()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptPointResult pointResult = ed.GetPoint("\nEnter table insertion point: ");
            if (pointResult.Status == PromptStatus.OK)
            {
                using (Transaction trans = doc.TransactionManager.StartTransaction())
                {
                    // Tạo style tùy chỉnh
                    const string styleName = "Garish Table Style";
                    ObjectId tableStyleId = ObjectId.Null;
                    DBDictionary styleDic = trans.GetObject(db.TableStyleDictionaryId, OpenMode.ForRead) as DBDictionary;
                    // Sử dụng style nếu đã tồn tại
                    if (styleDic.Contains(styleName))
                    {
                        tableStyleId = styleDic.GetAt(styleName);
                    }
                    else
                    {
                        // Nếu chưa tồn tại thì tạo mới
                        TableStyle tableStyle = new TableStyle();
                        tableStyle.SetBackgroundColor(Color.FromColorIndex(ColorMethod.ByAci, 1), (int)(RowType.HeaderRow | RowType.TitleRow));
                        tableStyle.SetBackgroundColor(Color.FromColorIndex(ColorMethod.ByAci, 2), (int)RowType.DataRow);
                        tableStyle.SetColor(Color.FromColorIndex(ColorMethod.ByAci, 6), (int)(RowType.HeaderRow | RowType.TitleRow | RowType.DataRow));
                        styleDic.UpgradeOpen();
                        tableStyleId = styleDic.SetAt(styleName, tableStyle);
                        trans.AddNewlyCreatedDBObject(tableStyle, true);
                        styleDic.DowngradeOpen();
                    }
                    BlockTable blockTable = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    Table table = new Table();
                    // Sử dụng table style
                    if (tableStyleId == ObjectId.Null) table.TableStyle = db.Tablestyle;
                    else table.TableStyle = tableStyleId;
                    table.NumRows = 5;
                    table.NumColumns = 3;
                    table.SetRowHeight(3);
                    table.SetColumnWidth(15);
                    table.Position = pointResult.Value;
                    // Create a 2-dimensional array of our table contents
                    string[,] str = new string[5, 4];
                    str[0, 0] = "Part No.";
                    str[0, 1] = "Name ";
                    str[0, 2] = "Material ";
                    str[1, 0] = "1876-1";
                    str[1, 1] = "Flange";
                    str[1, 2] = "Perspex";
                    str[2, 0] = "0985-4";
                    str[2, 1] = "Bolt";
                    str[2, 2] = "Steel";
                    str[3, 0] = "3476-K";
                    str[3, 1] = "Tile";
                    str[3, 2] = "Ceramic";
                    str[4, 0] = "8734-3";
                    str[4, 1] = "Kean";
                    str[4, 2] = "Mostly water";
                    // Use a nested loop to add and format each cell
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            table.SetTextHeight(i, j, 1);
                            table.SetTextString(i, j, str[i, j]);
                            table.SetAlignment(i, j, CellAlignment.MiddleCenter);
                        }
                    }
                    table.GenerateLayout();
                    BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    tableRec.AppendEntity(table);
                    trans.AddNewlyCreatedDBObject(table, true);
                    trans.Commit();
                }
            }
        }
    }
}
