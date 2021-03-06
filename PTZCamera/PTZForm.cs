﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTZ_Controller
{
    public partial class PTZForm : Form
    {
        public int speed = 4;
        LoginForm clientForm;
        public int nLoginID;

        public PTZForm(LoginForm loginForm, int log)
        {
            clientForm = loginForm;
            InitializeComponent();
            nLoginID = log;
            this.Select();  // Activate user control to the PTZ form
        }

        private void PTZControl(int nCommand, bool bStop, int nSpeed)
        {
            //DEV_INFO m_devinfo = clientForm.devInfo;
            //int nLoginID = m_devinfo.lLoginID;
            int nChannel = 0;
            XMSDK.H264_DVR_PTZControl(nLoginID, nChannel, (int)nCommand, bStop, nSpeed);
        }

        /*private void PTZForm_Load(object sender, EventArgs e)
        {


            this.KeyDown += new KeyEventHandler(PTZForm_KeyDown);
            this.KeyUp += new KeyEventHandler(PTZForm_KeyUp);

            // Error possibility from simultaneous key presses
        }*/

        void PTZForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // handle up/down/right/left
                case Keys.Up:
                    PTZControl((int)PTZ_ControlType.TILT_UP, false, speed);
                    break;
                case Keys.Down:
                    PTZControl((int)PTZ_ControlType.TILT_DOWN, false, speed);
                    break;
                case Keys.Right:
                    PTZControl((int)PTZ_ControlType.PAN_RIGHT, false, speed);
                    break;
                case Keys.Left:
                    PTZControl((int)PTZ_ControlType.PAN_LEFT, false, speed);
                    break;
                case Keys.Z:    // zoom IN
                    PTZControl((int)PTZ_ControlType.ZOOM_IN, false, speed);
                    break;
                case Keys.X:    // zoom OUT
                    PTZControl((int)PTZ_ControlType.ZOOM_OUT, false, speed);
                    break;


                // set PTZ speed from 1-8 (Low 1 to High 8)
                case Keys.D1:
                case Keys.NumPad1: speed = 1; break;
                case Keys.D2:
                case Keys.NumPad2: speed = 2; break;
                case Keys.D3:
                case Keys.NumPad3: speed = 3; break;
                case Keys.D4:
                case Keys.NumPad4: speed = 4; break;
                case Keys.D5:
                case Keys.NumPad5: speed = 5; break;
                case Keys.D6:
                case Keys.NumPad6: speed = 6; break;
                case Keys.D7:
                case Keys.NumPad7: speed = 7; break;
                case Keys.D8:
                case Keys.NumPad8: speed = 8; break;

                default: return;  // ignore other keys
            }

            // set speed dialog on Form2 to display updated speed
        }

        private void PTZForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // handle up/down/right/left STOP
                case Keys.Down:
                    PTZControl((int)PTZ_ControlType.TILT_UP, true, speed);
                    break;
                case Keys.Up:
                    PTZControl((int)PTZ_ControlType.TILT_DOWN, true, speed);
                    break;
                case Keys.Left:
                    PTZControl((int)PTZ_ControlType.PAN_RIGHT, true, speed);
                    break;
                case Keys.Right:
                    PTZControl((int)PTZ_ControlType.PAN_LEFT, true, speed);
                    break;
                case Keys.Z:    // zoom IN STOP
                    PTZControl((int)PTZ_ControlType.ZOOM_IN, true, speed);
                    break;
                case Keys.X:    // zoom OUT STOP
                    PTZControl((int)PTZ_ControlType.ZOOM_OUT, true, speed);
                    break;
                default: return;    // ignore other keys
            }
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            if (textBoxCmd.Text.Trim() != "" && textBoxCmd.Text.Trim().Length > 1)
            {
                string cmd = textBoxCmd.Text.Trim();
                int ctrl;   // Set control type
                int dinc = 15;  // 15 degree speed increments (120 deg/s max)
                double steps;  // Number of frame steps
                int deg;    // Degrees of rotation
                int tinc;   // Time increment for PTZ

                switch (cmd.Substring(0, 2))
                {
                    case "PL": case "pl": case "Pl": case "pL":
                        ctrl = (int)PTZ_ControlType.PAN_RIGHT;
                        break;
                    case "PR": case "pr": case "Pr": case "pR":
                        ctrl = (int)PTZ_ControlType.PAN_LEFT;
                        break;
                    case "TU": case "tu": case "Tu": case "tU":
                        ctrl = (int)PTZ_ControlType.TILT_DOWN;
                        break;
                    case "TD": case "td": case "Td": case "tD":
                        ctrl = (int)PTZ_ControlType.TILT_UP;
                        break;
                    case "ZI": case "zi": case "Zi": case "zI":
                        ctrl = (int)PTZ_ControlType.ZOOM_IN;
                        if (cmd.Length == 2)
                        {
                            PTZControl(ctrl, false, 4); // Control Zoom with speed = 4
                           // await Task.Delay(1000);
                            System.Threading.Thread.Sleep(1000);
                            PTZControl(ctrl, true, 4);  // Stop
                            return;
                        }
                        break;
                    case "ZO": case "zo": case "Zo": case "zO":
                        ctrl = (int)PTZ_ControlType.ZOOM_OUT;
                        if (cmd.Length == 2)
                        {
                            PTZControl(ctrl, false, 4); // Control Zoom with speed = 4
                            //await Task.Delay(1000);
                            System.Threading.Thread.Sleep(1000);
                            PTZControl(ctrl, true, 4);  // Stop
                            return;
                        }
                        break;
                    default:
                        MessageBox.Show("Invalid Input");
                        return;
                } 

                // Check if degree input is an integer and is below 360 degrees          
                if (int.TryParse(cmd.Substring(2, cmd.Length - 2), out deg))
                {
                    if (deg <= 360)
                    {
                        steps = deg / dinc;

                        if (ctrl == (int)PTZ_ControlType.TILT_UP || ctrl == (int)PTZ_ControlType.TILT_DOWN)
                        {
                            tinc = Convert.ToInt32(1950 * steps);
                        }
                        else
                        {
                            tinc = Convert.ToInt32(1143 * steps);
                        }

                        // tinc = Convert.ToInt32(1000 * steps);
                        PTZControl(ctrl, false, 2); // Control PT with speed = 1 (15 deg/s)
                        //await Task.Delay(tinc);
                        System.Threading.Thread.Sleep(tinc);
                        PTZControl(ctrl, true, 2);  // Stop
                    }
                    else
                    {
                        MessageBox.Show("Invalid Input: Rotation > 360 degrees");
                    }
                    return;
                }
                else
                {
                    MessageBox.Show("Invalid Input: Rotation not int");
                }
                return;
            }
            MessageBox.Show("Invalid Input: Empty or Single Character");
            return;
        }

        private void buttonSwitch_Click(object sender, EventArgs e)
        {
            textBoxCmd.Enabled = !textBoxCmd.Enabled;
        }
    }
}
