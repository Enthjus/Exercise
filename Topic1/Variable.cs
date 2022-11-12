using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryCad;
using OfficeOpenXml;

namespace Topic1
{
    public class Variable
    {
        public static double sumDim = 0;

        public static double sumLine = 0;

        /// <summary>
        /// Hàm nhập file txt
        /// </summary>
        /// <param name="i"></param>
        public static void Import_txt(int i)
        {
            //Thêm dữ liệu mới
            OpenFileDialog f = new OpenFileDialog();
            f.Title = "Chọn File Ghi Chú!!!";
            f.Filter = "Notepad (*.txt)|*.txt";
            if (f.ShowDialog() == DialogResult.OK)
            {
                string name_txt = f.FileName;
                string path = Path.GetExtension(name_txt);
                Stream s = File.Open(name_txt, FileMode.Open);
                StreamReader r = new StreamReader(s);
                while (!r.EndOfStream)
                {
                    try
                    {
                        // Đọc từng dòng
                        string line = r.ReadLine();
                        if (line.Length > 0)
                        {
                            var layerInfo = new LibraryCad.Models.LayerInfo();
                            layerInfo.Name = line.Split('|')[0];
                            layerInfo.ColorId = short.Parse(line.Split('|')[1]);
                            layerInfo.Des = "tool create layer";
                            var status = LibraryCad.LayerFunc.CreateLayer(layerInfo);
                            i++;
                        }
                    }
                    catch { continue; }
                }
                r.Close();
            }
        }

        /// <summary>
        /// Hàm xuất file excel
        /// </summary>
        /// <param name="layerObjects">List thông tin các đối tượng</param>
        public static void Export_csv(List<LibraryCad.Models.LayerObject> layerObjects)
        {
            string filePath = "";
            //tạo SaveDialog để lưu file excel
            SaveFileDialog dialog = new SaveFileDialog();

            //chỉ lọc những file có định dạng excel
            dialog.Filter = "Excel | *.xlsx | Excel 2003 | *.xls";

            //Nếu mở file và chọn nơi lưu file thành công sẽ lưu đường dẫn lại dùng
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filePath = dialog.FileName;
            }

            //nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Đường dẫn báo cáo không hợp lệ");
                return;
            }

            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    //đặt tên người tạo file
                    package.Workbook.Properties.Author = "Phúc";

                    //đặt tiêu đề cho file
                    package.Workbook.Properties.Title = "Kích thước";

                    //tạo 1 sheet để làm việc trên đó
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add("Kích thước sheet");

                    //lấy sheet vừa add để thao tác
                    //ExcelWorksheet ws = package.Workbook.Worksheets[1];

                    //đặt tên cho sheet
                    ws.Name = "Area";
                    //font size mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 14;
                    //font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Arial";

                    ////Tạo danh sách các column header
                    //string[] arrColumnHeader =
                    //{
                    //    "Tên Layer",
                    //    "Chu vi",
                    //    "Diện tích"
                    //};

                    //lấy ra số lượng cột cần dùng dựa vào số lượng header
                    var countColumnHeader = 3;

                    //merge các column lại từ column 1 đến số column header
                    ws.Cells[1, 1].Value = "Thống kê kích thước theo layer";
                    ws.Cells[1, 1, 1, countColumnHeader].Merge = true;
                    //in đậm
                    ws.Cells[1, 1, 1, countColumnHeader].Style.Font.Bold = true;
                    //cân giữa
                    ws.Cells[1, 1, 1, countColumnHeader].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    //int colIndex = 1;
                    //int rowIndex = 2;

                    // Tạo table theo list truyền vào
                    var range = ws.Cells["A2"].LoadFromCollection(layerObjects, true);
                    range.AutoFitColumns();

                    //// tạo header từ column header đã tạo từ bên trên
                    //foreach (string col in arrColumnHeader)
                    //{
                    //    var cell = ws.Cells[rowIndex, colIndex];

                    //    //set màu thành gray
                    //    var fill = cell.Style.Fill;
                    //    fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    //    //fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);

                    //    //Căn chỉnh các border
                    //    var border = cell.Style.Border;
                    //    border.Bottom.Style =
                    //        border.Top.Style =
                    //        border.Left.Style =
                    //        border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.MediumDashed;

                    //    //gán giá trị
                    //    cell.Value = col;

                    //    colIndex++;
                    //}

                    ////mỗi item trong danh sách LayerObject sẽ ghi 1 dòng
                    //foreach (var item in Variable.layerObjects)
                    //{
                    //    //bắt đầu ghi từ cột 1
                    //    colIndex = 1;

                    //    //rowindex tương ứng với dòng dữ liệu
                    //    rowIndex++;

                    //    //gán giá trị cho từng cell
                    //    ws.Cells[rowIndex, colIndex++].Value = item.LayerName;

                    //    ws.Cells[rowIndex, colIndex++].Value = item.Perimeter;

                    //    ws.Cells[rowIndex, colIndex++].Value = item.Area;
                    //}

                    //lưu file lại
                    Byte[] bin = package.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }
                MessageBox.Show("Xuất excel thành công");
            }
            catch 
            {
                MessageBox.Show("Có lỗi khi lưu file");
            }
        }
    }
}
