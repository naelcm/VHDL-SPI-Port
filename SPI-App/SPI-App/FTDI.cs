// If wanting to use the SPI port directly, try this:
// https://www.eevblog.com/forum/projects/ftdi-2232h-in-mpsse-spi-mode-toil-and-trouble-example-code-needed/


using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using FT_HANDLE = System.UInt32;

namespace ExpChargeTechTool
{

/// <summary>
/// Class to encapsulate an FTDI based Controller
/// </summary>
public class FTDI
{
 //   public int iNumDevices = 0;         // Number of FTDI devices found attached to this computer
    public bool bDeviceOpen = false;    // True when device is open

    public const int UP = 0x01;
    public const int DOWN = 0x02;
    public const int RIGHT = 0x04;
    public const int LEFT = 0x08;
    public const int STOP = 0x00;
    public const int SLOW = 0x40;

    enum FT_STATUS
    {
        FT_OK = 0,
        FT_INVALID_HANDLE,
        FT_DEVICE_NOT_FOUND,
        FT_DEVICE_NOT_OPENED,
        FT_IO_ERROR,
        FT_INSUFFICIENT_RESOURCES,
        FT_INVALID_PARAMETER,
        FT_INVALID_BAUD_RATE,
        FT_DEVICE_NOT_OPENED_FOR_ERASE,
        FT_DEVICE_NOT_OPENED_FOR_WRITE,
        FT_FAILED_TO_WRITE_DEVICE,
        FT_EEPROM_READ_FAILED,
        FT_EEPROM_WRITE_FAILED,
        FT_EEPROM_ERASE_FAILED,
        FT_EEPROM_NOT_PRESENT,
        FT_EEPROM_NOT_PROGRAMMED,
        FT_INVALID_ARGS,
        FT_OTHER_ERROR
    };

    enum FT_OPEN_FLAGS
    {
        FT_OPEN_BY_SERIAL_NUMBER = 1,
        FT_OPEN_BY_DESCRIPTION = 2,
        FT_OPEN_BY_LOCATION = 4
    };

    enum FT_PURGE_FLAGS
    {
        FT_PURGE_RX = 1,
        FT_PURGE_TX = 2
    };

    public struct DeviceData
    {
        public UInt32 u32Flag;
        public UInt32 u32Type;
        public UInt32 u32DeviceID;
        public UInt32 u32LocationID;
        public String strSerialNo;
        public String strDescription;
        public FT_HANDLE handle;
    }


    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_ListDevices(void* pvArg1, void* pvArg2, UInt32 dwFlags);	// FT_ListDevices by number only
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_ListDevices(UInt32 pvArg1, void* pvArg2, UInt32 dwFlags);	// FT_ListDevcies by serial number or description by index only
    [DllImport("FTD2XX.dll")]
    static extern FT_STATUS FT_Open(UInt32 uiPort, ref FT_HANDLE ftHandle);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_OpenEx(void* pvArg1, UInt32 dwFlags, ref FT_HANDLE ftHandle);
    [DllImport("FTD2XX.dll")]
    static extern FT_STATUS FT_Close(FT_HANDLE ftHandle);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_Read(FT_HANDLE ftHandle, void* lpBuffer, UInt32 dwBytesToRead, ref UInt32 lpdwBytesReturned);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_Write(FT_HANDLE ftHandle, void* lpBuffer, UInt32 dwBytesToRead, ref UInt32 lpdwBytesWritten);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetBaudRate(FT_HANDLE ftHandle, UInt32 dwBaudRate);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetDataCharacteristics(FT_HANDLE ftHandle, byte uWordLength, byte uStopBits, byte uParity);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetFlowControl(FT_HANDLE ftHandle, char usFlowControl, byte uXon, byte uXoff);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetDtr(FT_HANDLE ftHandle);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_ClrDtr(FT_HANDLE ftHandle);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetRts(FT_HANDLE ftHandle);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_ClrRts(FT_HANDLE ftHandle);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_GetModemStatus(FT_HANDLE ftHandle, ref UInt32 lpdwModemStatus);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetChars(FT_HANDLE ftHandle, byte uEventCh, byte uEventChEn, byte uErrorCh, byte uErrorChEn);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_Purge(FT_HANDLE ftHandle, UInt32 dwMask);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetTimeouts(FT_HANDLE ftHandle, UInt32 dwReadTimeout, UInt32 dwWriteTimeout);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_GetQueueStatus(FT_HANDLE ftHandle, ref UInt32 lpdwAmountInRxQueue);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetBreakOn(FT_HANDLE ftHandle);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetBreakOff(FT_HANDLE ftHandle);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_GetStatus(FT_HANDLE ftHandle, ref UInt32 lpdwAmountInRxQueue, ref UInt32 lpdwAmountInTxQueue, ref UInt32 lpdwEventStatus);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetEventNotification(FT_HANDLE ftHandle, UInt32 dwEventMask, void* pvArg);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_ResetDevice(FT_HANDLE ftHandle);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetDivisor(FT_HANDLE ftHandle, char usDivisor);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_GetLatencyTimer(FT_HANDLE ftHandle, ref byte pucTimer);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetLatencyTimer(FT_HANDLE ftHandle, byte ucTimer);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_GetBitMode(FT_HANDLE ftHandle, ref byte pucMode);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetBitMode(FT_HANDLE ftHandle, byte ucMask, byte ucEnable);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_SetUSBParameters(FT_HANDLE ftHandle, UInt32 dwInTransferSize, UInt32 dwOutTransferSize);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_CreateDeviceInfoList(void* lpdwNumDevs);
    [DllImport("FTD2XX.dll")]
    static extern unsafe FT_STATUS FT_GetDeviceInfoDetail(UInt32 dwIndex, void* lpdwFlags, void* lpdwType, void* lpdwID, void* lpdwLocId, void* pcSerialNumber, void* pcDescription, FT_HANDLE* ftHandle);

    public uint u_dp_pos;
    bool b_diagnostics = false;
    private FT_HANDLE ftHandle;
    //   protected UInt32 m_hPort;

    public FTDI()
    {
        u_dp_pos = 0;
        ftHandle = 0;
    }

    // Create the device info list
    public unsafe int CreateDeviceList()
    {
        FT_STATUS ftStatus = FT_STATUS.FT_OTHER_ERROR;
        int iNum = 0;

        ftStatus = FT_CreateDeviceInfoList(&iNum);
        if (ftStatus != FT_STATUS.FT_OK)
        {
            iNum = 0;
        }
        return iNum;
    }

    // Retrieve an  entry in the device info list
    public unsafe bool GetDeviceInfoDetail(int iIndex, ref FTDI.DeviceData sDeviceData)
    {
        FT_STATUS ftStatus = FT_STATUS.FT_OTHER_ERROR;
        UInt32 u32Flag;
        UInt32 u32Type;
        UInt32 u32DeviceID;
        UInt32 u32LocationID;
        byte[] abySerialNo = new byte[256];
        byte[] abyDescription = new byte[256];
        FT_HANDLE ftHandle;
        int uCharsToNull = 0;

        fixed (byte* pbySerialNo = abySerialNo)
        fixed (byte* pbyDescription = abyDescription)
            ftStatus = FT_GetDeviceInfoDetail((uint)iIndex, &u32Flag, &u32Type, &u32DeviceID, &u32LocationID, pbySerialNo, pbyDescription, &ftHandle);
        if (ftStatus == FT_STATUS.FT_OK)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;


            sDeviceData.u32Flag = u32Flag;
            sDeviceData.u32Type = u32Type;
            sDeviceData.u32DeviceID = u32DeviceID;
            sDeviceData.u32LocationID = u32LocationID;
            while (abySerialNo[uCharsToNull] != 0)
            {
                uCharsToNull++;
            }
            sDeviceData.strSerialNo = enc.GetString(abySerialNo, 0, uCharsToNull);
            uCharsToNull = 0;
            while (abyDescription[uCharsToNull] != 0)
            {
                uCharsToNull++;
            }
            sDeviceData.strDescription = enc.GetString(abyDescription, 0, uCharsToNull);
            sDeviceData.handle = ftHandle;
        }
        return (ftStatus == FT_STATUS.FT_OK);
    }

    // Open the device with the specified serial No.
    public unsafe bool OpenDevice(string strSerialNo)
    {
        FT_STATUS ftStatus = FT_STATUS.FT_OTHER_ERROR;
        bool b_retval = true;
        System.Text.Encoding enc = System.Text.Encoding.ASCII;
        byte[] abyBuffer = new byte[strSerialNo.Length + 1];

        enc.GetBytes(strSerialNo, 0, strSerialNo.Length, abyBuffer, 0);
        abyBuffer[strSerialNo.Length] = 0;      // Null terminate
        fixed (byte* pbySerialNo = abyBuffer)
            ftStatus = FT_OpenEx(pbySerialNo, (uint)FT_OPEN_FLAGS.FT_OPEN_BY_SERIAL_NUMBER, ref ftHandle);
        b_retval = (ftStatus == FT_STATUS.FT_OK);
        if (!b_retval && b_diagnostics)
        {
            MessageBox.Show("Failed to open port" + Convert.ToString(ftStatus));
        }
        if (b_retval)
        {
            // Set up the port
            ftStatus = FT_SetBaudRate(ftHandle, 10000);
            b_retval = (ftStatus == FT_STATUS.FT_OK);
        }
        if (!b_retval && b_diagnostics)
        {
            MessageBox.Show("Failed to set baud rate" + Convert.ToString(ftStatus));
        }
        if (b_retval)
        {
            // Set port to Synchronous Bit-Bang mode
            ftStatus = FT_SetBitMode(ftHandle, 0xFB, 0x04);
            b_retval = (ftStatus == FT_STATUS.FT_OK);
        }
        if (!b_retval && b_diagnostics)
        {
            MessageBox.Show("Failed to set bit mode" + Convert.ToString(ftStatus));
        }
        bDeviceOpen = b_retval;
        return (b_retval);
    }

    public void CloseDevice()
    {
        FT_Close(ftHandle);
        ftHandle = 0;
        u_dp_pos = 0;
        bDeviceOpen = false;
    }

    /// <summary>
    /// Call before writing to the device
    /// Checks the device status and disconnects it properly if it is not OK
    /// </summary>
    /// <returns></returns>
    public bool CheckConnect()
    {
        UInt32 lpdwAmountInRxQueue = 0;
        UInt32 lpdwAmountInTxQueue = 0;
        UInt32 lpdwEventStatus = 0;
        bool b_retval = true;

        if (FT_GetStatus(ftHandle, ref lpdwAmountInRxQueue, ref lpdwAmountInTxQueue, ref lpdwEventStatus) != FT_STATUS.FT_OK)
        {
            u_dp_pos = 0;		// Maybe we have been disconnected
            b_retval = false;
            CloseDevice();
        }
        return b_retval;
    }

    public unsafe bool WriteAndReadBytes(byte[] outData, out byte[] inData, UInt32 bytes)
    {
        FT_STATUS ftStatus = FT_STATUS.FT_OTHER_ERROR;
        UInt32 inByteCount = 0;
        UInt32 outByteCount = 0;

        inData = new byte[bytes];

        if (CheckConnect())
        {
            // Empty input buffer before trying to read
            ftStatus = FT_Purge(ftHandle, (UInt32)(FT_PURGE_FLAGS.FT_PURGE_RX | FT_PURGE_FLAGS.FT_PURGE_TX));

            if (ftStatus == FT_STATUS.FT_OK)
            {
                fixed (byte* pbyBuffer = outData)
                    ftStatus = FT_Write(ftHandle, pbyBuffer, bytes, ref outByteCount);
                if (ftStatus == FT_STATUS.FT_OK)
                {
                    fixed (byte* pbyBuffer = inData)
                        ftStatus = FT_Read(ftHandle, pbyBuffer, bytes, ref inByteCount);
                }
            }
        }
        return ((ftStatus == FT_STATUS.FT_OK) && (inByteCount == bytes) && (outByteCount == bytes));
    }
}
}