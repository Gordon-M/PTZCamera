﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PTZ_Controller
{
    public partial class VideoForm : Form
    {
        // int m_nIndex;   //index	
        // bool m_bRecord; //is recording or not
        bool m_bSound;  //sound is off/on

        public int m_iPlayhandle;   //play handle
        public int m_lLogin; //login handle
        public int m_iChannel; //play channel
        // public int m_iTalkhandle;

        public VideoForm()
        {
            InitializeComponent();
        }

        public int ConnectRealPlay(ref DEV_INFO pDev, int nChannel)
        {
            H264_DVR_CLIENTINFO playstru = new H264_DVR_CLIENTINFO();

            playstru.nChannel = nChannel;
            playstru.nStream = 0;
            playstru.nMode = 0;
            playstru.hWnd = this.Handle;
            m_iPlayhandle = XMSDK.H264_DVR_RealPlay(pDev.lLoginID, ref playstru);

            return m_iPlayhandle;
        }

        public int GetLoginHandle()
        {
            return m_lLogin;
        }

        public void OnDisconnect()
        {
            if (m_iPlayhandle > 0)
            {
                XMSDK.H264_DVR_StopRealPlay(m_iPlayhandle, (uint)this.Handle);
                m_iPlayhandle = -1;

            }
            if (m_bSound)
            {
                OnCloseSound();
            }
            m_lLogin = -1;
        }

        public bool OnOpenSound()
        {
            if (XMSDK.H264_DVR_OpenSound(m_iPlayhandle))
            {
                m_bSound = true;
                return true;
            }
            return false;
        }

        public bool OnCloseSound()
        {
            if (XMSDK.H264_DVR_CloseSound(m_iPlayhandle))
            {
                m_bSound = false;
                return true;
            }
            return false;
        }

        /*public void VideoExit()
        {
            ((Form)this.TopLevelControl).Close();
        }*/
    }
}
