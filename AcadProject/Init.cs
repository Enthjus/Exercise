using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using LibraryCad.DocumentManager;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;

//[assembly: CommandMethod(In)]
namespace Topic1
{
    public class Init : IExtensionApplication
    {
        public void Initialize()
        {
            DocumentManager document = new DocumentManager();
            //throw new System.NotImplementedException();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            ed.WriteMessage("Load thanh cong");
            if (ComponentManager.Ribbon == null)
            {
                //Load Ribbon khi khởi động AutoCAD
                //Ghi nhớ 1 sự kiện khi chưa tìm thấy Ribbon, đợi thực thi
                ComponentManager.ItemInitialized += new EventHandler<RibbonItemEventArgs>(ComponentManager_ItemInitialized);
            }
            else
            {
                //Tự động Load Ribbon khi thực hiện lệnh NETLOAD
                Ribbon_Utilities();
            }
        }

        public void Terminate()
        {
            //throw new System.NotImplementedException();
            // Cac ham su ly khi thoat CAD.
        }

        public void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {
            if (ComponentManager.Ribbon != null)
            {
                //Tạo Ribbon
                Ribbon_Utilities();
                //Loại bỏ sự kiện chờ
                ComponentManager.ItemInitialized -= new EventHandler<RibbonItemEventArgs>(ComponentManager_ItemInitialized);
            }
        }

        [CommandMethod("Ribbon_Phuc_Tools")]
        public static void Ribbon_Utilities()
        {
            RibbonControl ribbonControl = ComponentManager.Ribbon;
            RibbonTab Tab = new RibbonTab();
            Tab.Title = "Phuc Tools";
            Tab.Id = "Ribbon_Utilities_ID";
            ribbonControl.Tabs.Add(Tab);
            Tab.IsActive = true;

            Utilities_Add_Ribbon.Menu_Thongso(Tab);
        }

        //string _AppName = "[AutoCAD.net] Phuc Ribbon";
        //string _Decreption = "[AutoCAD.net] Ribbon for v.NET Tools @ Author: Phuc";


    }

    public class Utilities_Sub_Function
    {
        // Hàm con tạo Panel
        public static RibbonPanelSource PanelSource_Subfun(RibbonTab Rtab, string Panel)
        {
            RibbonPanelSource PanelSource = new RibbonPanelSource();
            PanelSource.Title = Panel;
            RibbonPanel RPanel = new RibbonPanel();
            RPanel.Source = PanelSource;
            Rtab.Panels.Add(RPanel);
            return PanelSource;
        }

        // Hàm con tạo Button
        public static RibbonButton RibbonButton_Subfun(string Text, string Command, bool showtext, Bitmap bmp,
            Orientation canhchu = Orientation.Horizontal, RibbonItemSize size = RibbonItemSize.Standard)
        {
            RibbonButton btn = new RibbonButton();
            btn.Text = Text;
            btn.CommandParameter = Command;
            btn.ShowText = showtext;
            btn.ShowImage = true;
            btn.Image = getBitmap(bmp);
            btn.LargeImage = getBitmap(bmp);
            btn.Orientation = canhchu;
            btn.Size = size;
            btn.CommandHandler = new RibbonCommandHandler();
            return btn;
        }

        public static RibbonButton RibbonButton_Subfun(string Command, Bitmap bmp, RibbonItemSize size = RibbonItemSize.Standard)
        {
            RibbonButton btn = new RibbonButton();
            btn.CommandParameter = Command;
            btn.ShowImage = true;
            btn.Image = getBitmap(bmp);
            btn.LargeImage = getBitmap(bmp);
            btn.Size = size;
            btn.CommandHandler = new RibbonCommandHandler();
            return btn;
        }

        // Hàm con lấy file hình
        public static BitmapImage getBitmap(Bitmap image)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.EndInit();

            return bmp;
        }

        public class RibbonCommandHandler : System.Windows.Input.ICommand
        {
            public event System.EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                Document doc = acadApp.DocumentManager.MdiActiveDocument;
                RibbonButton button = parameter as RibbonButton;
                if (button != null)
                {
                    doc.SendStringToExecute((String)button.CommandParameter, true, false, true);
                }
            }
        }
    }

    public class Utilities_Add_Ribbon
    {
        public static void Menu_Thongso(RibbonTab Tab)
        {
            // Create Ribbon PANELS
            RibbonPanelSource panel1Panel = Utilities_Sub_Function.PanelSource_Subfun(Tab, "Thông Số");

            // Create Ribbon BOTTON
            string P1T_btn1 = "Tạo nhiều" + "\n" + "text";
            RibbonButton pan1button1 = Utilities_Sub_Function.RibbonButton_Subfun(P1T_btn1, "CreateMText ", true, Properties.Resources.text, Orientation.Vertical, RibbonItemSize.Large);
            string P1T_btn2 = "Đọc text";
            RibbonButton pan1button2 = Utilities_Sub_Function.RibbonButton_Subfun(P1T_btn2, "ReadText ", true, Properties.Resources.open_book);
            string P1T_btn3 = "Gộp text";
            RibbonButton pan1button3 = Utilities_Sub_Function.RibbonButton_Subfun(P1T_btn3, "MergeString ", true, Properties.Resources.merge);

            RibbonRowPanel rowPanel = new RibbonRowPanel();
            rowPanel.Items.Add(pan1button2);
            rowPanel.Items.Add(new RibbonRowBreak());
            rowPanel.Items.Add(pan1button3);

            panel1Panel.Items.Add(pan1button1);
            panel1Panel.Items.Add(rowPanel);
        }
    }
}
