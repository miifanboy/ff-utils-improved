﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ff_utils_winforms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.logTbox = logTbox;
            CheckForIllegalCrossThreadCalls = false;
            InitCombox(createMp4Enc, 0);
            InitCombox(createMp4Crf, 1);
            InitCombox(createMp4Fps, 2);
            InitCombox(loopTimesLossless, 0);
            InitCombox(loopEncTimes, 0);
            InitCombox(loopEnc, 0);
            InitCombox(loopCrf, 1);
            InitCombox(encVidCodec, 0);
            InitCombox(encVidCrf, 1);
            InitCombox(encAudCodec, 1);
            InitCombox(encAudBitrate, 4);
            InitCombox(changeSpeedCombox, 0);
            InitCombox(comparisonEnc, 0);
            InitCombox(comparisonCrf, 1);
            InitCombox(comp2enc, 0);
            InitCombox(comp2crf, 1);
        }

        void InitCombox(ComboBox cbox, int index)
        {
            cbox.SelectedIndex = index;
            cbox.Text = cbox.Items[index].ToString();
        }

        private void extractFramesDropPanel_DragEnter(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }

        private void extractFramesDropPanel_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if(extractFramesTabcontrol.SelectedIndex == 0)
            {
                foreach (string file in files)
                    FFmpegCommands.VideoToFrames(file, tonemapHdrCbox2.Checked, extractAllDelSrcCbox.Checked);
            }
            if (extractFramesTabcontrol.SelectedIndex == 1)
            {
                int frameNum = frameNumTbox.GetInt();
                foreach (string file in files)
                    FFmpegCommands.ExtractSingleFrame(file, frameNum, tonemapHdrCbox2.Checked, extractSingleDelSrcCbox.Checked);
            }
        }

        private void createVidDropPanel_DragEnter(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }

        private void createVidDropPanel_DragDrop(object sender, DragEventArgs e)
        {
            string[] dirs = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (createVidTabControl.SelectedIndex == 0) // Create MP4
            {
                bool h265 = createMp4Enc.SelectedIndex == 1;
                int crf = createMp4Crf.GetInt();
                int fps = createMp4Fps.GetInt();
                foreach (string dir in dirs)
                    FFmpegCommands.FramesToMp4(dir, h265, crf, fps, createMp4Prefix.Text.Trim(), framesToMp4DelSrc.Checked);
            }
            if (createVidTabControl.SelectedIndex == 1) // Create APNG
            {
                bool optimize = createApngOpti.Checked;
                int fps = createApngFps.GetInt();
                foreach (string dir in dirs)
                    FFmpegCommands.FramesToApng(dir, optimize, fps, createApngPrefix.Text.Trim(), createApngDelSrc.Checked);
            }
            if (createVidTabControl.SelectedIndex == 2) // Create GIF
            {
                bool optimize = createGifOpti.Checked;
                int fps = createGifFps.GetInt();
                foreach (string dir in dirs)
                    FFmpegCommands.FramesToGif(dir, optimize, fps, createGifPrefix.Text.Trim(), createGifDelSrc.Checked);
            }
        }

        private void loopDropPanel_DragEnter(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }

        private void loopDropPanel_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            if (loopTabControl.SelectedIndex == 0) // Lossless
            {
                int times = loopTimesLossless.GetInt();
                foreach (string file in files)
                    FFmpegCommands.LoopVideo(file, times, loopEncDelSrc.Checked);
            }
            if (loopTabControl.SelectedIndex == 1) // With Re-Encoding
            {
                int times = int.Parse(loopEncTimes.Text.Trim());
                bool h265 = loopEnc.SelectedIndex == 1;
                int crf = int.Parse(loopCrf.Text.Trim());
                foreach(string file in files)
                    FFmpegCommands.LoopVideoEnc(file, times, h265, crf, loopEncDelSrc.Checked);
            }
        }

        private void encodeDropPanel_DragEnter(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }

        private void encodeDropPanel_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach(string file in files)
                EncodeTabHelper.Run(file, encVidCodec, encAudCodec, encVidCrf, encAudBitrate, encDelSrc);
        }

        private void speedDropPanel_DragEnter(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }

        private void speedDropPanel_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (speedTabControl.SelectedIndex == 0) // Lossless
            {
                int times = changeSpeedCombox.GetInt();
                foreach (string file in files)
                    FFmpegCommands.ChangeSpeed(file, times, loopEncDelSrc.Checked);
            }
        }

        private void compDropPanel_DragEnter(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }

        private void compDropPanel_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            string vcodec = "libx264";
            

            if (compTabControl.SelectedIndex == 0) // Side By Side
            {
                if (comparisonEnc.SelectedIndex == 1)
                    vcodec = "libx265";
                FFmpegCommands.CreateComparison(files[0], files[1], false, vcodec, comparisonCrf.GetInt(), comparisonDelSrc.Checked);
            }
                

            if (compTabControl.SelectedIndex == 1) // Over-Under
            {
                if (comp2enc.SelectedIndex == 1)
                    vcodec = "libx265";
                FFmpegCommands.CreateComparison(files[0], files[1], true, vcodec, comp2crf.GetInt(), comp2delSrc.Checked);
            }
        }
    }
}
