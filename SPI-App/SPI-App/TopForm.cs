


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FT_HANDLE = System.UInt32;

namespace ExpChargeTechTool
{
    public partial class TopForm : Form
    {
        FTDI ftdi = new FTDI();
        //private bool countUp = true;

        class registerRow
        {
            public Label address;
            public Label name;
            public Label data;
            public NumericUpDown hexData;
            public Button read;
            public Button write;
        }

        registerRow[] registerRows;

        public TopForm()
        {
            InitializeComponent();
        }

        // Programmatically create the read and write buttons
        private void TopForm_Load(object sender, EventArgs e)
        {
            int i;

            registerRows = new registerRow[10];

            try
            {
                for (i = 0; i < 8; i++)
                {
                    registerRows[i] = new registerRow();

                    registerRows[i].address = new Label();
                    registerRows[i].address.Location = new System.Drawing.Point(10, 20 + (24 * i));
                    registerRows[i].address.Size = new System.Drawing.Size(20, 20);
                    registerRows[i].address.Text = String.Format("{0:00}", i);
                    grpRegisters.Controls.Add(registerRows[i].address);

                    registerRows[i].name = new Label();
                    registerRows[i].name.Location = new System.Drawing.Point(30, 20 + (24 * i));
                    registerRows[i].name.Size = new System.Drawing.Size(80, 20);
                    registerRows[i].name.Text = String.Format("Status register");
                    grpRegisters.Controls.Add(registerRows[i].name);

                    registerRows[i].data = new Label();
                    registerRows[i].data.Location = new System.Drawing.Point(120, 20 + (24 * i));
                    registerRows[i].data.Size = new System.Drawing.Size(50, 20);
                    registerRows[i].data.BorderStyle = BorderStyle.Fixed3D;
                    registerRows[i].data.Text = "0x00";
                    grpRegisters.Controls.Add(registerRows[i].data);

                    registerRows[i].read = new Button();
                    registerRows[i].read.Location = new System.Drawing.Point(180, 20 + (24 * i));
                    registerRows[i].read.Size = new System.Drawing.Size(60, 20);
                    registerRows[i].read.Text = String.Format("Read");
                    registerRows[i].read.Tag = i;
                    registerRows[i].read.Click += new System.EventHandler(btnReadClick);
                    grpRegisters.Controls.Add(registerRows[i].read);

                    registerRows[i].hexData = new NumericUpDown();
                    registerRows[i].hexData.Location = new System.Drawing.Point(250, 20 + (24 * i));
                    registerRows[i].hexData.Size = new System.Drawing.Size(40, 20);
                    registerRows[i].hexData.Value = 0;
                    registerRows[i].hexData.Maximum = 255;
                    registerRows[i].hexData.Hexadecimal = true;
                    grpRegisters.Controls.Add(registerRows[i].hexData);

                    registerRows[i].write = new Button();
                    registerRows[i].write.Location = new System.Drawing.Point(300, 20 + (24 * i));
                    registerRows[i].write.Size = new System.Drawing.Size(60, 20);
                    registerRows[i].write.Text = String.Format("Write");
                    registerRows[i].write.Tag = i;
                    registerRows[i].write.Click += new System.EventHandler(btnWriteClick);
                    grpRegisters.Controls.Add(registerRows[i].write);
                }
            }
            catch (Exception ex)
            {

            }
        }

        // Read a byte from the specified register
        // We do write a byte, but byte written is zero
        private void btnReadClick(object sender, EventArgs e)
        {
            byte address;
            byte data;

            //MessageBox.Show(String.Format("Read {0:00}", ((Button)sender).Tag));
            try
            {
                address = (byte)(0x80 + (int)((Button)sender).Tag);
                SPI_XferByte((byte)(0x80 + (int)((Button)sender).Tag), (byte)0, out data);
                registerRows[(int)((Button)sender).Tag].data.Text = String.Format("0x{0:X02}", data);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "ExpChargeTechTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Write a byte to the specified register
        // We do read a byte back, but it is discarded
        private void btnWriteClick(object sender, EventArgs e)
        {
            byte address;
            byte data;

            //MessageBox.Show(String.Format("Write {0:00}", ((Button)sender).Tag));
            try
            {
                byte toWrite = 0;
                address = (byte)(0x00 + (int)((Button)sender).Tag);
                toWrite = (byte)registerRows[(int)((Button)sender).Tag].hexData.Value;
                SPI_XferByte(address, toWrite, out data);
                //registerRows[(int)((Button)sender).Tag].data.Text = String.Format("0x{0:X02}", data);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "ExpChargeTechTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Transit the byte, msb first, clock rises to indicate valid data
        // There is an SPI engine in the MSSPE, but it sounds like hard work to use
        private bool SPI_XferByte(byte address, byte toWrite, out byte readByte)
        {
            const byte SCLK = 0x01;
            const byte SO = 0x02;
            const byte SI = 0x04;
            const byte CS = 0x08;
            bool retval = true;
            byte outByte = CS;
            byte inByte = 0;
            int i;
            UInt32 index = 0;
            byte[] outData = new byte[100];
            byte[] inData;

            readByte = 0;

            // Start with CS high, SO and CLK high
            outData[index++] = (byte)(CS | SO | SCLK);

            // Assert CS low, SO and CLK high
            outData[index++] = unchecked((byte)~CS);

            for (i = 0; i < 8; i++)
            {
                // Write next bit of address and low edge on clock
                outByte = 0x00;
                if((address & 0x80) == 0x80)
                    outByte |= SO;
                outData[index++] = outByte;

                // Rising edge on clock
                outByte |= SCLK;
                outData[index++] = outByte;

                address = (byte)(address << 1);
            }
            for (i = 0; i < 8; i++)
            {
                // Write next bit of data and low edge on clock
                outByte = 0x00;
                if ((toWrite & 0x80) == 0x80)
                    outByte |= SO;
                outData[index++] = outByte;

                // Rising edge on clock
                outByte |= SCLK;
                outData[index++] = outByte;

                toWrite = (byte)(toWrite << 1);
            }
            // Negate CS
            outData[index++] = CS;

            // 4 byte just makes the bitCounter reach 1
            // 18 bytes just makes the bitCounter reach 8
            // 35 bytes is sixteen bits plus two setup bytes and one final byte
            ftdi.WriteAndReadBytes(outData, out inData, 35);

            for (i = 20; i < 35; i += 2)
            {
                readByte = (byte)(readByte << 1);
                if ((inData[i] & SI) == SI)
                {
                    readByte |= 0x01;
                }
            }

            return (retval);
        }

        private void cmbDevices_DropDown(object sender, EventArgs e)
        {
            int i;
            int numDevices = ftdi.CreateDeviceList();
            FTDI.DeviceData data = new FTDI.DeviceData();

            cmbDevices.Items.Clear();
            for (i = 0; i < numDevices; i++)
            {
                ftdi.GetDeviceInfoDetail(i, ref data);
                cmbDevices.Items.Add(data.strSerialNo);
            }
        }

        private void cmbDevices_DropDownClosed(object sender, EventArgs e)
        {
            if ((cmbDevices.Items.Count > 0) && (cmbDevices.Text != ""))
            {
                if (!ftdi.OpenDevice(cmbDevices.Text))
                {
                    MessageBox.Show("Error opening FTDI device", "Expedition Charger Tech Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tmr100_Tick(object sender, EventArgs e)
        {

        }

        private void nudValue_ValueChanged(object sender, EventArgs e)
        {
            //ftdi.WriteByte((uint)nudValue.Value);
        }


    }
}
