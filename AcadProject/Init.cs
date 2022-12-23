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
namespace AcadProject
{
    public class Init : IExtensionApplication
    {
        public void Initialize()
        {
            DocumentManager.CreateDocument();
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

            Utilities_Add_Ribbon.Menu_Text(Tab);
            Utilities_Add_Ribbon.Menu_Block(Tab);
            Utilities_Add_Ribbon.Menu_Circle(Tab);
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
        #region Menu text
        public static void Menu_Text(RibbonTab Tab)
        {
            // Create Ribbon PANELS
            RibbonPanelSource panel1Panel = Utilities_Sub_Function.PanelSource_Subfun(Tab, "Thông Số");

            // Create Ribbon BUTTON
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
        #endregion

        #region Menu khối
        public static void Menu_Block(RibbonTab Tab)
        {
            // Create Ribbon PANELS
            RibbonPanelSource panel2Panel = Utilities_Sub_Function.PanelSource_Subfun(Tab, "Thao tác với khối");

            // Create Ribbon BUTTON
            string P2T_btn1 = "Copy khối qua file khác";
            RibbonButton pan2button1 = Utilities_Sub_Function.RibbonButton_Subfun(P2T_btn1, "CopyBlocksBetweenDatabases ", true, Properties.Resources.copy_block);
            string P2T_btn2 = "Copy đối tượng qua file khác";
            RibbonButton pan2button2 = Utilities_Sub_Function.RibbonButton_Subfun(P2T_btn2, "CopyObjectsBetweenDatabases ", true, Properties.Resources.copy_object);
            string P2T_btn3 = "Lấy block từ file";
            RibbonButton pan2button3 = Utilities_Sub_Function.RibbonButton_Subfun(P2T_btn3, "GetBlockController ", true, Properties.Resources.get_block);
            string P2T_btn4 = "Thêm đối tượng vào khối";
            RibbonButton pan2button4 = Utilities_Sub_Function.RibbonButton_Subfun(P2T_btn4, "AddEntToBlock ", true, Properties.Resources.add_ent_to_block);
            string P2T_btn5 = "Xóa khỏi khối";
            RibbonButton pan2button5 = Utilities_Sub_Function.RibbonButton_Subfun(P2T_btn5, "DeleteEntityInBlock ", true, Properties.Resources.delete_ent_in_block);
            string P2T_btn6 = "Đổi layer trong khối";
            RibbonButton pan2button6 = Utilities_Sub_Function.RibbonButton_Subfun(P2T_btn6, "EditEntityInBlock", true, Properties.Resources.edit_block);

            RibbonRowPanel rowPanel1 = new RibbonRowPanel();
            rowPanel1.Items.Add(pan2button1);
            rowPanel1.Items.Add(new RibbonRowBreak());
            rowPanel1.Items.Add(pan2button2);
            rowPanel1.Items.Add(new RibbonRowBreak());
            rowPanel1.Items.Add(pan2button3);

            RibbonRowPanel rowPanel2 = new RibbonRowPanel();
            rowPanel2.Items.Add(pan2button4);
            rowPanel2.Items.Add(new RibbonRowBreak());
            rowPanel2.Items.Add(pan2button5);
            rowPanel2.Items.Add(new RibbonRowBreak());
            rowPanel2.Items.Add(pan2button6);

            panel2Panel.Items.Add(rowPanel1);
            panel2Panel.Items.Add(rowPanel2);
        }
        #endregion

        public static void Menu_Circle(RibbonTab Tab)
        {
            // Create Ribbon PANELS
            RibbonPanelSource panel3Panel = Utilities_Sub_Function.PanelSource_Subfun(Tab, "Thao tác với hình tròn");

            // Create Ribbon BUTTON
            string P3T_btn1 = "Vẽ hình tròn";
            RibbonButton pan3button1 = Utilities_Sub_Function.RibbonButton_Subfun(P3T_btn1, "DrawCircle ", true, Properties.Resources.draw_circle);
            string P3T_btn2 = "Tam giác nội tiếp đường tròn";
            RibbonButton pan3button2 = Utilities_Sub_Function.RibbonButton_Subfun(P3T_btn2, "TriangleInscribedWithCircle ", true, Properties.Resources.triangle_in_circle);
            string P3T_btn3 = "Tam giác ngoại tiếp đường tròn";
            RibbonButton pan3button3 = Utilities_Sub_Function.RibbonButton_Subfun(P3T_btn3, "TriangleCircumscribedAboutCircle ", true, Properties.Resources.triangle_out_circle);

            RibbonRowPanel rowPanel = new RibbonRowPanel();
            rowPanel.Items.Add(pan3button1);
            rowPanel.Items.Add(new RibbonRowBreak());
            rowPanel.Items.Add(pan3button2);
            rowPanel.Items.Add(new RibbonRowBreak());
            rowPanel.Items.Add(pan3button3);

            panel3Panel.Items.Add(rowPanel);
        }

        public static void Menu_Dimension(RibbonTab Tab)
        {
            // Create Ribbon PANELS
            RibbonPanelSource panel4Panel = Utilities_Sub_Function.PanelSource_Subfun(Tab, "Thao tác với hình tròn");

            // Create Ribbon BUTTON
            string P4T_btn1 = "Vẽ hình tròn";
            RibbonButton pan4button1 = Utilities_Sub_Function.RibbonButton_Subfun(P4T_btn1, "DrawCircle ", true, Properties.Resources.draw_circle);
            string P4T_btn2 = "Tam giác nội tiếp đường tròn";
            RibbonButton pan4button2 = Utilities_Sub_Function.RibbonButton_Subfun(P4T_btn2, "TriangleInscribedWithCircle ", true, Properties.Resources.triangle_in_circle);
            string P4T_btn3 = "Tam giác ngoại tiếp đường tròn";
            RibbonButton pan4button3 = Utilities_Sub_Function.RibbonButton_Subfun(P4T_btn3, "TriangleCircumscribedAboutCircle ", true, Properties.Resources.triangle_out_circle);

            RibbonRowPanel rowPanel = new RibbonRowPanel();
            rowPanel.Items.Add(pan4button1);
            rowPanel.Items.Add(new RibbonRowBreak());
            rowPanel.Items.Add(pan4button2);
            rowPanel.Items.Add(new RibbonRowBreak());
            rowPanel.Items.Add(pan4button3);

            panel4Panel.Items.Add(rowPanel);
        }
    }
}
