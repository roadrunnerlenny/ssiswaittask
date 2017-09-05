using System;
using System.Text.RegularExpressions;

namespace ALE.WaitTask.Helper
{
    public class Time
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public Time(int hours, int minutes)
        {
            Hours = hours;
            Minutes = minutes;
        }

        public static Time ParseHourMinutesTimeString(string time)
        {
            if (string.IsNullOrEmpty(time) || time.Trim().Length == 0) return null;
            int hours = -1;
            int minutes = -1;
            Regex timeRegex = new Regex(@"^(?<hours>[0-2]{0,1}[0-9]{1})\:{0,1}(?<minutes>[0-6]{1}[0-9]{1})$");
            if (timeRegex.IsMatch(time))
            {
                Match timeMatch = timeRegex.Match(time);
                int.TryParse(timeMatch.Groups["hours"].Value, out hours);
                int.TryParse(timeMatch.Groups["minutes"].Value, out minutes);
                if (hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59)
                {
                    return new Time(hours, minutes);
                }
            }
            throw new ApplicationException("Please enter a valid time (format hh:mm)!");
        }

        public override string ToString()
        {
            string hours = Hours.ToString();
            string minutes = Minutes.ToString();
            if (Minutes < 10) minutes = "0" + minutes;
            if (Hours < 10) hours = "0" + hours;
            return hours + ":" + minutes;
        }

        public static TimeSpan CalculateTimeToWait(Time waitUntilTime)
        {
            DateTime waitUntil = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, waitUntilTime.Hours, waitUntilTime.Minutes, 0);
            if (waitUntil < DateTime.Now)
                waitUntil = waitUntil.AddDays(1);
            TimeSpan waitTime = waitUntil.Subtract(DateTime.Now);
            return waitTime;
        }
    }
}
