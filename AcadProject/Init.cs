using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using LibraryCad.DocumentManager;
using LibraryCad.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecureDongle;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(AcadProject.Init))]
namespace AcadProject
{
    enum SDCmd : ushort
    {
        SD_FIND = 1,//Find Dongle
        SD_FIND_NEXT = 2,//Find Next Dongle
        SD_OPEN = 3,//Open Dongle
        SD_CLOSE = 4,//Close Dongle
        SD_READ = 5,//Read Dongle
        SD_WRITE = 6,//Write Dongle
        SD_RANDOM = 7,//Generate Random Number
        SD_SEED = 8,//Generate Seed Code
        SD_WRITE_USERID = 9,//Write User ID
        SD_READ_USERID = 10,//Read User ID
        SD_SET_MODULE = 11,//Set Module
        SD_CHECK_MODULE = 12,//Check Module
        SD_WRITE_ARITHMETIC = 13,//Write Arithmetic
        SD_CALCULATE1 = 14,//Calculate 1
        SD_CALCULATE2 = 15,//Calculate 2
        SD_CALCULATE3 = 16,//Calculate 3
        SD_DECREASE = 17,//Decrease Module Unit
        SD_SET_COUNTER = 20,//set counter
        SD_GET_COUNTER = 21,//get counter
        SD_DEC_COUNTER = 22,
        SD_SET_TIMER = 23,//set timer
        SD_GET_TIMER = 24,//get timer
        SD_ADJUST_TIMER = 25,//adjust timer
        SD_SET_RSAKEY_N = 29,//write RSA N
        SD_SET_RSAKEY_D = 30,//write RSA D
        SD_UPDATE_GEN_HEADER = 31,//generate encrypted file header
        SD_UPDATE_GEN = 32,//create encrypted file content
        SD_UPDATE_CHECK = 33,//update cipher file
        SD_UPDATE = 34,//update cipher file
        SD_SET_DES_KEY = 41,//Set DES key
        SD_DES_ENC = 42,//DES encryption
        SD_DES_DEC = 43,//DES decryption
        SD_RSA_ENC = 44,//RSA encryption
        SD_RSA_DEC = 45,//RSA decryption
        SD_READ_EX = 46,//read dongle memory
        SD_WRITE_EX = 47,//write dongle memory
        SD_SET_COUNTER_EX = 0xA0,//set counter value type changed from WORD to DWORD
        SD_GET_COUNTER_EX = 0xA1,//get counter, value type changed from WORD to DWORD
        SD_SET_TIMER_EX = 0xA2,//set timer time value type changed from WORD to DWORD
        SD_GET_TIMER_EX = 0xA3,//get timer time value type changed from WORD to DWORD
        SD_ADJUST_TIMER_EX = 0xA4,//adjust timer, time value type changed from WORD to DWORD
        SD_UPDATE_GEN_HEADER_EX = 0xA5,//generate update file header specialize in updating RSA key pair
        SD_UPDATE_GEN_EX = 0xA6,//generate update file content specialize in updating RSA key pair
        SD_UPDATE_CHECK_EX = 0xA7,//update file checking specialize in updating RSA key pair
        SD_UPDATE_EX = 0xA8,//update cipher file specialize in updating RSA key pair
        SD_SET_UPDATE_KEY = 0xA9,//set update RSA key pair
        SD_ADD_UPDATE_HEADER = 0xAA,//fill head of authorization file
        SD_ADD_UPDATE_CONTENT = 0xAB,//fill content of authorization file
        SD_GET_TIME_DWORD = 0xAC,//get value(DWORD type) based on 2006.1.1.0.0.0
        SD_VERSION = 100,//get COS Version
    };

    enum SDErrCode : uint
    {
        ERR_SUCCESS = 0,							//No error
        ERR_NO_PARALLEL_PORT = 0x80300001,		//(0x80300001)No parallel port
        ERR_NO_DRIVER,							//(0x80300002)No drive
        ERR_NO_DONGLE,							//(0x80300003)No SecureDongle
        ERR_INVALID_pWORD,					//(0x80300004)Invalid pword
        ERR_INVALID_pWORD_OR_ID,				//(0x80300005)Invalid pword or ID
        ERR_SETID,								//(0x80300006)Set id error
        ERR_INVALID_ADDR_OR_SIZE,				//(0x80300007)Invalid address or size
        ERR_UNKNOWN_COMMAND,					//(0x80300008)Unkown command
        ERR_NOTBELEVEL3,						//(0x80300009)Inner error
        ERR_READ,								//(0x8030000A)Read error
        ERR_WRITE,								//(0x8030000B)Write error
        ERR_RANDOM,								//(0x8030000C)Generate random error
        ERR_SEED,								//(0x8030000D)Generate seed error
        ERR_CALCULATE,							//(0x8030000E)Calculate error
        ERR_NO_OPEN,							//(0x8030000F)The SecureDongle is not opened
        ERR_OPEN_OVERFLOW,						//(0x80300010)Open SecureDongle too more(>16)
        ERR_NOMORE,								//(0x80300011)No more SecureDongle
        ERR_NEED_FIND,							//(0x80300012)Need Find before FindNext
        ERR_DECREASE,							//(0x80300013)Dcrease error
        ERR_AR_BADCOMMAND,						//(0x80300014)Band command
        ERR_AR_UNKNOWN_OPCODE,					//(0x80300015)Unkown op code
        ERR_AR_WRONGBEGIN,						//(0x80300016)There could not be constant in first instruction in arithmetic 
        ERR_AR_WRONG_END,						//(0x80300017)There could not be constant in last instruction in arithmetic 
        ERR_AR_VALUEOVERFLOW,					//(0x80300018)The constant in arithmetic overflow
        ERR_UNKNOWN = 0x8030ffff,					//(0x8030FFFF)Unkown error
        ERR_RECEIVE_NULL = 0x80300100,			//(0x80300100)Receive null
        ERR_PRNPORT_BUSY = 0x80300101				//(0x80300101)Parallel port busy
    };

    //-----------------------------------------------------------------------

    public class Init : IExtensionApplication
    {
        static AcadProject.UserControl1 _uc;

        //Declare variable
        byte[] buffer = new byte[1024];
        object obbuffer = new object();
        ushort handle = 0;
        ushort p1 = 0;
        ushort p2 = 0;
        ushort p3 = 0;
        ushort p4 = 0;
        uint lp1 = 0;
        uint lp2 = 0;
        ulong ret = 1;
        UInt32 value = 0;

        SecureDonglecom SD = new SecureDonglecom();

        public static int remainder = 0;
        public static int triangleStatus = 0;

        public void Initialize()
        {
            Application.DisplayingOptionDialog += new TabbedDialogEventHandler(Application_DisplayingOptionDialog);

            DocumentManager.CreateDocument();
            //throw new System.NotImplementedException();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            using (StreamReader r = new StreamReader(@"D:\c#\ATC\AddinLearning\appsetting.json"))
            {
                string json = r.ReadToEnd();
                List<AppSetting> items = JsonConvert.DeserializeObject<List<AppSetting>>(json);
                foreach (AppSetting item in items)
                {
                    if(item.Data == "Remainder calculator")
                    {
                        remainder = item.Status;
                    }
                    else if(item.Data == "Triangle status")
                    {
                        triangleStatus = item.Status;
                    }
                }
            }

            ////Open SecureDongle
            ////SecureDongle Password
            //p1 = 0x1C76;
            //p2 = 0x8078;
            //p3 = 0x8058;    //advance password. Must set to 0 for end user application
            //p4 = 0x5D6A;    //advance password. Must set to 0 for end user application

            //ret = SD.SecureDongle((ushort)SDCmd.SD_FIND, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, ref obbuffer);

            //if (ret != 0)
            //{
            //    ed.WriteMessage(string.Format("\nNo SecureDongle found, error code: {0}", ret));
            //    return;
            //}
            //else
            //{
            //    ed.WriteMessage(string.Format("\nFound SecureDongle. HID: {0:X8}", lp1));
            //}

            //ret = SD.SecureDongle((ushort)SDCmd.SD_OPEN, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, ref obbuffer);

            //if (ret != 0)
            //{
            //    ed.WriteMessage(string.Format("\nOpen SecureDongle failed, error code: {0}", ret));
            //    return;
            //}
            //else
            //{
            //    ed.WriteMessage(string.Format("\nOpen SecureDongle ({0:X8})", lp1));
            //}
            //value = UInt32.Parse("10");
            //obbuffer = (object)value;
            //ret = SD.SecureDongle((ushort)SDCmd.SD_SET_COUNTER_EX, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, ref obbuffer);
            ////Get Counter Value
            //p1 = 5; // Which Counter?
            //ret = SD.SecureDongle((ushort)SDCmd.SD_GET_COUNTER_EX, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, ref obbuffer);
            //if (ret != 0)
            //{
            //    if (ret == 228)
            //    {
            //        ed.WriteMessage(string.Format("Cannot find any valid value in this module."));
            //        return;
            //    }
            //    ed.WriteMessage(string.Format("Counter decrease failed, error code: {0}", ret));
            //    return;
            //}
            //else
            //{
            //    value = (UInt32)obbuffer;
            //    ed.WriteMessage(string.Format("Counter Value: {0}", value));
            //}


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
            Application.DisplayingOptionDialog -= new TabbedDialogEventHandler(Application_DisplayingOptionDialog);
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
            Utilities_Add_Ribbon.Menu_Dimension(Tab);
            Utilities_Add_Ribbon.Menu_Line(Tab);
            Utilities_Add_Ribbon.Menu_Extend(Tab);
        }

        //string _AppName = "[AutoCAD.net] Phuc Ribbon";
        //string _Decreption = "[AutoCAD.net] Ribbon for v.NET Tools @ Author: Phuc";

        private static void ButtonPressedMessage(string name)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage(name + " button pressed.\n");
        }

        private static void ShowSettings()
        {
            if (_uc != null)
            {
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                //ed.WriteMessage("Settings were: checkbox contains {0}," + " combobox contains {1}\n", _uc.checkBox1.Checked, _uc.comboBox1.Text);
            }
        }

        private static void OnOK()
        {
            ButtonPressedMessage("OK");
            ShowSettings();
        }

        private static void OnCancel()
        {
            ButtonPressedMessage("Cancel");
        }

        private static void OnHelp()
        {
            ButtonPressedMessage("Help");
        }


        private static void OnApply()
        {
            //System.Windows.Forms.RadioButton button = new System.Windows.Forms.RadioButton();
            //var uc = new UserControl1(button);
            //_ = button.Text == "Remainder" ? remainder = 1 : remainder = 0;

            remainder = (_uc.rbNotRemainder.Checked) ? 1 : 0;
            triangleStatus = (_uc.comboBoxTriangleStatus.Text == "Inscribed") ? 1 : 0;

            List<AppSetting> appsetting = new List<AppSetting>
            {
                new AppSetting("Remainder calculator", remainder),
                new AppSetting("Triangle status", triangleStatus)
            };

            var jsonSerialiser = new JavaScriptSerializer();
            string jsonString = jsonSerialiser.Serialize(appsetting);
            File.WriteAllText(@"D:\c#\ATC\AddinLearning\appsetting.json", jsonString);

            //// write JSON directly to a file
            //using (StreamWriter file = File.CreateText(@"D:\c#\ATC\AddinLearning\appsetting.json"))
            //using (JsonTextWriter writer = new JsonTextWriter(file))
            //{
            //    appsetting.WriteTo(writer);
            //}
        }

        private static void Application_DisplayingOptionDialog(object sender, TabbedDialogEventArgs e)
        {
            if (_uc == null) _uc = new UserControl1();
            TabbedDialogExtension tde = new TabbedDialogExtension(_uc, new TabbedDialogAction(OnOK), new TabbedDialogAction(OnCancel), new TabbedDialogAction(OnHelp), new TabbedDialogAction(OnApply));
            e.AddTab("My Custom Settings", tde);
        }
    }

    #region Các hàm bổ trợ
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
    #endregion

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

        #region Menu hình tròn
        public static void Menu_Circle(RibbonTab Tab)
        {
            // Create Ribbon PANELS
            RibbonPanelSource panel3Panel = Utilities_Sub_Function.PanelSource_Subfun(Tab, "Thao tác với hình tròn");

            // Create Ribbon BUTTON
            string P3T_btn1 = "Vẽ hình\ntròn";
            RibbonButton pan3button1 = Utilities_Sub_Function.RibbonButton_Subfun(P3T_btn1, "DrawCircle ", true, Properties.Resources.draw_circle, Orientation.Vertical, RibbonItemSize.Large);
            string P3T_btn2 = "Tam giác nội tiếp đường tròn";
            RibbonButton pan3button2 = Utilities_Sub_Function.RibbonButton_Subfun(P3T_btn2, "TriangleInscribedWithCircle ", true, Properties.Resources.triangle_in_circle);
            string P3T_btn3 = "Tam giác ngoại tiếp đường tròn";
            RibbonButton pan3button3 = Utilities_Sub_Function.RibbonButton_Subfun(P3T_btn3, "TriangleCircumscribedAboutCircle ", true, Properties.Resources.triangle_out_circle);

            RibbonRowPanel rowPanel = new RibbonRowPanel();
            rowPanel.Items.Add(pan3button2);
            rowPanel.Items.Add(new RibbonRowBreak());
            rowPanel.Items.Add(pan3button3);

            panel3Panel.Items.Add(pan3button1);
            panel3Panel.Items.Add(rowPanel);
        }
        #endregion

        #region Menu đường kích thước
        public static void Menu_Dimension(RibbonTab Tab)
        {
            // Create Ribbon PANELS
            RibbonPanelSource panel4Panel = Utilities_Sub_Function.PanelSource_Subfun(Tab, "Thao tác với đường kích thước");

            // Create Ribbon BUTTON
            string P4T_btn1 = "Cộng tổng các\nđường kích thước";
            RibbonButton pan4button1 = Utilities_Sub_Function.RibbonButton_Subfun(P4T_btn1, "DimensionSum ", true, Properties.Resources.dimension_sum, Orientation.Vertical, RibbonItemSize.Large);
            string P4T_btn2 = "Tạo đường kích thước trên nhiều đoạn thẳng";
            RibbonButton pan4button2 = Utilities_Sub_Function.RibbonButton_Subfun(P4T_btn2, "DimMLine ", true, Properties.Resources.dimension_multi_line);
            string P4T_btn3 = "Tạo đường kích thước trên nhiều đường đa tuyến";
            RibbonButton pan4button3 = Utilities_Sub_Function.RibbonButton_Subfun(P4T_btn3, "Dimpolyline ", true, Properties.Resources.dimension_multi_polyline);
            string P4T_btn4 = "Tạo đường kích thước trên nhiều đường cong";
            RibbonButton pan4button4 = Utilities_Sub_Function.RibbonButton_Subfun(P4T_btn4, "DimMArc ", true, Properties.Resources.arc_dimension);

            RibbonRowPanel rowPanel = new RibbonRowPanel();
            rowPanel.Items.Add(pan4button2);
            rowPanel.Items.Add(new RibbonRowBreak());
            rowPanel.Items.Add(pan4button3);
            rowPanel.Items.Add(new RibbonRowBreak());
            rowPanel.Items.Add(pan4button4);

            panel4Panel.Items.Add(pan4button1);
            panel4Panel.Items.Add(rowPanel);
        }
        #endregion

        #region Menu đoạn thẳng
        public static void Menu_Line(RibbonTab Tab)
        {
            // Create Ribbon PANELS
            RibbonPanelSource panel5Panel = Utilities_Sub_Function.PanelSource_Subfun(Tab, "Thao tác với đường thẳng");

            // Create Ribbon BUTTON
            string P5T_btn1 = "Tổng kích thước đường thẳng";
            RibbonButton pan5button1 = Utilities_Sub_Function.RibbonButton_Subfun(P5T_btn1, "SumMultiLine ", true, Properties.Resources.sum_line);
            string P5T_btn2 = "Tạo đường thẳng";
            RibbonButton pan5button2 = Utilities_Sub_Function.RibbonButton_Subfun(P5T_btn2, "AddLine ", true, Properties.Resources.create_line);

            RibbonRowPanel rowPanel = new RibbonRowPanel();
            rowPanel.Items.Add(pan5button1);
            rowPanel.Items.Add(new RibbonRowBreak());
            rowPanel.Items.Add(pan5button2);

            panel5Panel.Items.Add(rowPanel);
        }
        #endregion

        #region Menu mở rộng
        public static void Menu_Extend(RibbonTab Tab)
        {
            // Create Ribbon PANELS
            RibbonPanelSource panel6Panel = Utilities_Sub_Function.PanelSource_Subfun(Tab, "Chức năng mở rộng");

            // Create Ribbon BUTTON
            string P6T_btn1 = "Thao tác layer";
            RibbonButton pan6button1 = Utilities_Sub_Function.RibbonButton_Subfun(P6T_btn1, "LayerController ", true, Properties.Resources.layer_controller);
            string P6T_btn2 = "Sắp xếp các chuỗi thành dạng bảng";
            RibbonButton pan6button2 = Utilities_Sub_Function.RibbonButton_Subfun(P6T_btn2, "SortFormController ", true, Properties.Resources.sort_text);

            RibbonRowPanel rowPanel = new RibbonRowPanel();
            rowPanel.Items.Add(pan6button1);
            rowPanel.Items.Add(new RibbonRowBreak());
            rowPanel.Items.Add(pan6button2);

            panel6Panel.Items.Add(rowPanel);
        }
        #endregion
    }

}
