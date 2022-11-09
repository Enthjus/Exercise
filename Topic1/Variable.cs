using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryCad;

namespace Topic1
{
    public class Variable
    {
        public static double sumDim = 0;

        public static double sumLine = 0;

        public static List<LibraryCad.Models.LayerInfo> layerInfos = new List<LibraryCad.Models.LayerInfo>();

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
                //i = 0;
                while (!r.EndOfStream)
                {
                    try
                    {
                        string line = r.ReadLine();
                        if (line.Length > 0)
                        {
                            var layerInfo = new LibraryCad.Models.LayerInfo();
                            layerInfo.Name = line.Split('|')[0];
                            layerInfo.ColorId = short.Parse(line.Split('|')[1]);
                            var status = LibraryCad.LayerFunc.CreateLayer(layerInfo);
                            if (status)
                            {
                                layerInfos.Add(layerInfo);
                            }
                            i++;
                        }
                    }
                    catch { continue; }
                }
                r.Close();
            }
        }
    }
}
