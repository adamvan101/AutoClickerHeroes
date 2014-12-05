using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoClicker
{   
    public partial class Form1 : Form
    {
        // Define our mouse event constants
        const int MOUSEEVENTF_LEFTDOWN = 2;
        const int MOUSEEVENTF_LEFTUP = 4;

        // This is a constant for our input type
        const int INPUT_MOUSE = 0;

        // A struct that holds the Mouse Input information
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        // My screen coords for Treebeast
        Point _hero = new Point(267, 592);

        // A struct of input information to pass to the "click" call
        public struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        };

        // A reference to the hotkey for our code
        const int F6_HOTKEY_ID = 1;
        const int F7_HOTKEY_ID = 2;
        const int F8_HOTKEY_ID = 3;
        const int F8_HOTKEY_ID_SHIFT = 4;
        const int F9_HOTKEY_ID = 5;
        const int F9_HOTKEY_ID_SHIFT = 6;
        const int F6_HOTKEY_ID_SHIFT = 7;
        
        // Some private variables to store data
        Point mousePosition = new Point(0, 0);
        List<Point> alternatePoints = new List<Point>();
        Timer setPoint = new Timer();
        Timer clickTimer = new Timer();
        Timer keyTimer = new Timer();
        Timer levelTimer = new Timer();
        int countdown;
        int index = 0;
        bool _lockMouse;
        bool executingAlts = false;

        public Form1()
        {
            InitializeComponent();

            // Timers
            setPoint.Tick += setPoint_Tick;
            clickTimer.Tick += clickTimer_Tick;
            keyTimer.Tick += keyTimer_Tick;
            levelTimer.Tick += levelTimer_Tick;

            // misc.
            comboBox1.SelectedIndex = 0;
            numericUpDown1.Value = 1;
            button1.Enabled = false;

            // Hotkeys
            RegisterHotKey(this.Handle, F6_HOTKEY_ID, 0, (int)Keys.F6);
            RegisterHotKey(this.Handle, F6_HOTKEY_ID_SHIFT, 4, (int)Keys.F6);
            RegisterHotKey(this.Handle, F7_HOTKEY_ID, 0, (int)Keys.F7);
            RegisterHotKey(this.Handle, F8_HOTKEY_ID, 0, (int)Keys.F8);
            RegisterHotKey(this.Handle, F8_HOTKEY_ID_SHIFT, 4, (int)Keys.F8);
            RegisterHotKey(this.Handle, F9_HOTKEY_ID, 0, (int)Keys.F9);
            RegisterHotKey(this.Handle, F9_HOTKEY_ID_SHIFT, 4, (int)Keys.F9);

            try
            {
                // Read file
                System.Xml.Serialization.XmlSerializer reader =
                    new System.Xml.Serialization.XmlSerializer(typeof(List<Point>));
                var desktopFodler = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var fullName = Path.Combine(desktopFodler, "List.txt");

                System.IO.StreamReader file = new System.IO.StreamReader(
                 fullName);
                alternatePoints = (List<Point>)reader.Deserialize(file);
                label4.Text = alternatePoints.Count.ToString();
            }
            catch (Exception e)
            {
                // Suppress
            }
        }

        void levelTimer_Tick(object sender, EventArgs e)
        {
            if (clickTimer.Enabled && checkbox_level.Checked)
            {
                levels();
            }
        }

        void levels()
        {
            Point p = Cursor.Position;
            executingAlts = true;
            System.Threading.Thread.Sleep(100);
            ClickPoint(_hero);
            System.Threading.Thread.Sleep(100);
            executingAlts = false;
            Cursor.Position = p;
        }

        void Keyboard()
        {
            System.Windows.Forms.SendKeys.Send("A");
            System.Threading.Thread.Sleep(40);

            System.Windows.Forms.SendKeys.Send("8");
            System.Threading.Thread.Sleep(40);
            System.Windows.Forms.SendKeys.Send("7");
            System.Threading.Thread.Sleep(40);
            System.Windows.Forms.SendKeys.Send("9");
            System.Threading.Thread.Sleep(40);
            System.Windows.Forms.SendKeys.Send("1");
            System.Threading.Thread.Sleep(40);
            System.Windows.Forms.SendKeys.Send("2");
            System.Threading.Thread.Sleep(40);
            System.Windows.Forms.SendKeys.Send("3");
            System.Threading.Thread.Sleep(40);
            System.Windows.Forms.SendKeys.Send("4");
            System.Threading.Thread.Sleep(40);
            System.Windows.Forms.SendKeys.Send("5");
            System.Threading.Thread.Sleep(40);
            System.Windows.Forms.SendKeys.Send("6");
            System.Threading.Thread.Sleep(40);
        }


        void keyTimer_Tick(object sender, EventArgs e)
        {
            if (clickTimer.Enabled && checkbox_keyboard.Checked)
            {
                Keyboard();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == F6_HOTKEY_ID_SHIFT)
            {
                mousePosition = Cursor.Position;
                StartClicking(false);
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == F6_HOTKEY_ID)
            {
                mousePosition = Cursor.Position;
                StartClicking(true);
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == F7_HOTKEY_ID)
            {
                StopClicking();
                this.WindowState = FormWindowState.Minimized;
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == F8_HOTKEY_ID)
            {
                ExecuteAlternatePoints();
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == F8_HOTKEY_ID_SHIFT)
            {
                ExecuteNextPoint();
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == F9_HOTKEY_ID)
            {
                IncrementIndex(1);
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == F9_HOTKEY_ID_SHIFT)
            {
                IncrementIndex(-1);
            }
            base.WndProc(ref m);
        }

        void clickTimer_Tick(object sender, EventArgs e)
        {
            if (executingAlts)
            {
                return;
            }

            //if (_lockMouse)
            //{
                Point p = Cursor.Position;
                ClickPoint(mousePosition);
                Cursor.Position = p;
            //}
            //else
            //{
                //ClickPoint(Cursor.Position);
            //}
        }

        void ClickPoint(Point p)
        {
            // Move our cursor
            Cursor.Position = p;
            // Set our structs
            INPUT i = new INPUT();
            i.type = INPUT_MOUSE;
            i.mi.dx = 0;
            i.mi.dy = 0;
            i.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            i.mi.dwExtraInfo = IntPtr.Zero;
            i.mi.mouseData = 0;
            i.mi.time = 0;
            // Send our mouse down input
            SendInput(1, ref i, Marshal.SizeOf(i));
            // Now send mouse up input
            i.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            SendInput(1, ref i, Marshal.SizeOf(i));
        }

        void setPoint_Tick(object sender, EventArgs e)
        {
            if (countdown == 0)
            {
                mousePosition = Cursor.Position;
                //show the location on window title
                this.label2.Text = "Position Set";
                setPoint.Stop();
                button1.Enabled = true;
            }
            else
            {
                setPoint.Interval = 1000;
                this.label2.Text = "Setting Position In " + countdown.ToString();
                countdown--;
            }
        }

        // Dlls used to handle input. Are they different?
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);
        //[DllImport("user32.dll")]
        //public static extern IntPtr SetCapture(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.SuppressKeyPress = true; // do this to 'eat' the keystroke
            if (e.KeyCode == Keys.Escape)
            {
                StopClicking();
            }
            base.OnKeyDown(e);
        }

        private void start_Click(object sender, EventArgs e)
        {
            if (numericUpDown1.Value <= 0)
            {
                numericUpDown1.Value = 1;
            }
            switch (comboBox1.SelectedIndex)
            {
                default:
                case 0:
                    clickTimer.Interval = (int)((float)numericUpDown1.Value);
                    break;
                case 1:
                    clickTimer.Interval = (int)((float)numericUpDown1.Value * 1000);
                    break;
                case 2: 
                    clickTimer.Interval = (int)((float)numericUpDown1.Value * 1000 * 60);
                    break;
            }
            if (!clickTimer.Enabled && button1.Enabled)
            {
                StartClicking(true);
            }
            else
            {
                StopClicking();
            }
        }

        private void StopClicking()
        {
            clickTimer.Stop();
            this.label2.Text = "Stopped";
            button1.Text = "Start";
            numericUpDown1.Enabled = true;
            setPos.Enabled = true;
            comboBox1.Enabled = true;
        }

        private void StartClicking(bool lockMouse)
        {
            _lockMouse = lockMouse;
            clickTimer.Start();
            this.label2.Text = "Clicking";
            button1.Text = "Stop";
            numericUpDown1.Enabled = false;
            setPos.Enabled = false;
            comboBox1.Enabled = false;

            if (lockMouse)
            {
                // The keyboard timer
                keyTimer.Interval = 1000 * 60 * 30;
                keyTimer.Start();
                Keyboard();

                // The level timer
                levelTimer.Interval = 1000 * 60;
                levelTimer.Start();
                levels();
            }
        }

        private void setPos_Click(object sender, EventArgs e)
        {
            countdown = 3;
            setPoint.Interval = 1;
            setPoint.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Add point
            alternatePoints.Add(Cursor.Position);
            label4.Text = alternatePoints.Count.ToString();

            try
            {
                System.Xml.Serialization.XmlSerializer writer =
                    new System.Xml.Serialization.XmlSerializer(typeof(List<Point>));
                var desktopFodler = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var fullName = Path.Combine(desktopFodler, "List.txt");

                System.IO.StreamWriter file = new System.IO.StreamWriter(
                 fullName);
                writer.Serialize(file, alternatePoints);
                file.Close();
            }
            catch
            {
                // suppress for now
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Clear points
            alternatePoints.Clear();
            label4.Text = alternatePoints.Count.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Execute points
            ExecuteAlternatePoints();
        }

        private void ExecuteAlternatePoints()
        {
            executingAlts = true;

            foreach (Point p in alternatePoints)
            {
                ClickPoint(p);
                System.Threading.Thread.Sleep(600);
            }

            executingAlts = false;
        }

        private void ExecuteNextPoint()
        {
            if (alternatePoints.Count == 0)
            {
                return;
            }

            ClickPoint(alternatePoints[index]);
        }

        private void IncrementIndex(int i)
        {
            index += i;
            if (index >= alternatePoints.Count)
            {
                index = 0;
            }
            else if (index < 0)
            {
                index = alternatePoints.Count - 1;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //ExecuteNextPoint();
            _hero = Cursor.Position;
        }
    }
}
