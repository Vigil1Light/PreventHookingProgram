using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreventHookingProgram
{
    public partial class Form1 : Form
    {
        bool status = false;
        // Import the FindWindow function from user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Import the SendMessage function from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const uint WM_CLOSE = 0x0010;

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            status = !status;
            if (status)
            {
                button1.Text = "Stop";
                timer1.Enabled = true;
            }
            else
            {
                button1.Text = "Start";
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string windowName = "OrderCapture";

            // Call FindWindow to get the handle of the window
            IntPtr hwnd = FindWindow(null, windowName);

            if (hwnd != IntPtr.Zero)
            {
                MessageBox.Show("Detected hooking program");
                // Close the window by sending the WM_CLOSE message
                SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

                // Alternatively, you can use the Process class to terminate the process
                // Get the process ID associated with the window
                uint processId;
                GetWindowThreadProcessId(hwnd, out processId);

                if (processId != 0)
                {
                    try
                    {
                        // Find and terminate the corresponding process
                        Process process = Process.GetProcessById((int)processId);
                        process.Kill();

                        Console.WriteLine("Process terminated.");
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Process ID not found.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error terminating process: " + ex.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("Window not found.");
            }
        }
    }
}
