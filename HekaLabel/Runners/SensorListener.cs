using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace HekaLabel.Runners
{
    public class SensorListener
    {
        private const string defaultFileContent = @"
[VBAI INI Variables]
ETIKET=0<<END>>";
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
                        string val = rdr.ReadToEnd();
                        rdr.Close();
                        rdr.Dispose();

                        if (Regex.IsMatch(val, "ETIKET=1"))
                        {
                            StreamWriter wr = new StreamWriter(this.TriggerPath);
                            wr.Write(defaultFileContent.Replace("\t", ""));
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
