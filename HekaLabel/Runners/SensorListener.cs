using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HekaLabel.Runners
{
    public class SensorListener
    {
        public string TriggerPath { get; set; }

        public delegate void PrintOrderArrived();
        public event PrintOrderArrived OnPrintOrderArrived;

        private bool _runCheck = false;
        private Task _runTask = null;
        public void Dispose()
        {
            Stop();
        }

        public void Run()
        {
            _runCheck = true;
            try
            {
                _runTask = Task.Run(RunnerLoop);
            }
            catch (Exception)
            {

            }
        }

        public void Stop()
        {
            _runCheck = false;
            try
            {
                if (_runTask != null)
                    _runTask.Dispose();
            }
            catch (Exception)
            {

            }
        }

        private async Task RunnerLoop()
        {
            while (_runCheck)
            {
                try
                {
                    if (!string.IsNullOrEmpty(this.TriggerPath) && File.Exists(this.TriggerPath))
                    {
                        StreamReader rdr = new StreamReader(this.TriggerPath);
                        string val = rdr.ReadLine().ToString();
                        rdr.Close();
                        rdr.Dispose();

                        if (val.StartsWith("1"))
                        {
                            StreamWriter wr = new StreamWriter(this.TriggerPath);
                            wr.WriteLine("0");
                            wr.Flush();
                            wr.Close();
                            wr.Dispose();

                            OnPrintOrderArrived?.Invoke();
                        }
                    }
                }
                catch (Exception)
                {

                }

                await Task.Delay(50);
            }
        }
    }
}
