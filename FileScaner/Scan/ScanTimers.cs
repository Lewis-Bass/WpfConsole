using Common.Licenses;
using Common.Settings;
using Serilog;
using System;
using System.Threading;

namespace FileScaner.Scan
{
    public class ScanTimers
    {

        private static Timer _timer;
        private static bool _enteredAlready = false;
        public readonly string TimerName = "FileScanTimerName";

        public void StartTimer()
        {
            // we only want one timer running
            if (_timer != null)
            {
                return;
            }


            _timer = ExecuteTheTimer((o) =>
            {
                // This will catch most of the threads that want to come in here...
                if (_enteredAlready)
                {
                    return;
                }

                _enteredAlready = true;

                // License check - do not run if the license is expired
                var chk = new LicenseChecks();
                var licenseStatus = chk.GetLicenseStatus();
                if (licenseStatus != LicenseChecks.LicenseStatus.Expired)
                {

                    // ToDo: run the scan when the cpu is idle????

                    // calculate timer start to see if we can load any changed files
                    // if the current time is between the stop and start set the delay to 15 minutes
                    AutoLoadSettings settings = AutoLoadSettings.Load(true);

                    DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, settings.AutoLoadStartTime, 0, 0);
                    DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, settings.AutoLoadEndTime, 0, 0);
                   
                    if (startTime <= DateTime.Now && endTime > DateTime.Now && settings.AutoLoadNextProcess < endTime)
                    {
                        using (FileScan filescan = new FileScan(settings))
                        {
                            filescan.ExecuteScan();
                        }
                        // don't process again until this time is past
                        // needed to stop multiple runs during the same day
                        settings.AutoLoadNextProcess = (startTime < endTime) ?
                            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, settings.AutoLoadEndTime, 1, 0) :
                            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, settings.AutoLoadEndTime, 1, 0);
                        settings.WriteFile();
                    }
                }
                _enteredAlready = false;
                
            }, TimerName);

        }

        public static Timer ExecuteTheTimer(TimerCallback callback, string threadName)
        {
            try
            {
                Log.Information($"Launching the timer {threadName}");
#if DEBUG
                int delay = 20 * 1000; // FOR TESTING ONLY
#else
                int delay = 15 * 60 * 1000; // 15 minutes * 60 seconds * 1000 milliseconds
#endif

                Timer timer = new(callback, null, delay, delay);
                return timer;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in DbControlledThreading.ExecuteTheTimer(callback, {threadName})");
                return null;
            }
        }
    }
}
