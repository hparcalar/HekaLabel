using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharp7;

namespace HekaLabel
{
    public class PlcTransfer : IDisposable
    {
        public delegate void PrintL1(string modelCode);
        public delegate void PrintL2(string modelCode);

        public event PrintL1 OnPrintL1;
        public event PrintL2 OnPrintL2;

        private S7Client _plc;
        private Task _listener;
        private bool _runListener;

        public PlcTransfer()
        {
            _plc = new S7Client();
        }

        public void Run()
        {
            this.Stop();

            _runListener = true;
            _listener = Task.Run(this.ListenLoop);
            
        }
        public void Stop()
        {
            if (_listener != null && !_listener.IsCompleted)
                try
                {
                    _runListener = false;
                    _listener.Dispose();
                }
                catch (Exception)
                {

                }
        }

        private async Task ListenLoop()
        {
            while (_runListener)
            {
                try
                {
                    bool printL1 = false;
                    bool printL2 = false;

                    int conResult = this._plc.ConnectTo("192.168.0.1", 0, 0);
                    if (conResult == 0)
                    {
                        S7MultiVar Reader = new S7MultiVar(this._plc);
                        S7MultiVar Writer = new S7MultiVar(this._plc);

                        byte[] DB_A = new byte[1024];
                        byte[] DB_B = new byte[1024];

                        byte[] DB_W = new byte[1];

                        int DBNumber_A = 15; // DB15

                        Reader.Add(S7Consts.S7AreaDB, S7Consts.S7WLByte, DBNumber_A, 0, 1, ref DB_A);
                        Reader.Add(S7Consts.S7AreaDB, S7Consts.S7WLByte, DBNumber_A, 2, 10, ref DB_B);
                        Writer.Add(S7Consts.S7AreaDB, S7Consts.S7WLByte, DBNumber_A, 0, 1, ref DB_W);

                        int Result = Reader.Read();

                        printL1 = S7.GetBitAt(DB_A, 0, 0);
                        printL2 = S7.GetBitAt(DB_A, 0, 1);

                        var printModel = S7.GetStringAt(DB_B, 0);

                        if (printL1)
                        {
                            S7.SetBitAt(ref DB_W, 0, 0, false);
                            S7.SetBitAt(ref DB_W, 0, 2, true);
                        }

                        if (printL2)
                        {
                            S7.SetBitAt(ref DB_W, 0, 1, false);
                            S7.SetBitAt(ref DB_W, 0, 3, true);
                        }

                        if (printL1 || printL2)
                        {
                            int writeResult = Writer.Write();
                        }

                        this._plc.Disconnect();

                        if (printL1)
                        {
                            OnPrintL1?.Invoke(printModel);
                        }

                        if (printL2)
                        {
                            OnPrintL2?.Invoke(printModel);
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                await Task.Delay(100);
            }
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
