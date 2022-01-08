using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace P
{
    public static class Constant
    {
        public const String APP_DIRECTORY = @"C:\Users\Public\Parental Control\";
        public const String SCREENSHOT_DIRECTORY = @"C:\Users\Public\Parental Control\Screenshots\";
        public const String CONFIG_FILE = "config.txt";
        public const int TIME_BETWEEN_SAVE = 1; //10 minutes
    }

    public class Time
    {
        public int Hour;
        public int Minute;

        public Time()
        {
            Hour = 0;
            Minute = 0;
        }

        public Time(int _hour, int _minute)
        {
            Hour = _hour;
            Minute = _minute;
        }

        public Time(string timeString) //format: hh:mm
        {
            string[] components = timeString.Split(':');
            Hour = Int32.Parse(components[0]);
            Minute = Int32.Parse(components[1]);
        }

        public static bool operator ==(Time a, Time b)
        {
            return a.Hour == b.Hour && a.Minute == b.Minute;
        }

        public static bool operator !=(Time a, Time b)
        {
            return a.Hour != b.Hour || a.Minute != b.Minute;
        }

        public static bool operator >(Time a, Time b)
        {
            return a.Hour > b.Hour || ((a.Hour == b.Hour) && (a.Minute > b.Minute));
        }

        public static bool operator <(Time a, Time b)
        {
            return a.Hour < b.Hour || ((a.Hour == b.Hour) && (a.Minute < b.Minute));
        }

        public static bool operator <=(Time a, Time b)
        {
            return a < b || a == b;
        }

        public static bool operator >=(Time a, Time b)
        {
            return a > b || a == b;
        }

        public static Time operator +(Time a, Time b)
        {
            Time sum = new Time();
            sum.Hour = a.Hour + b.Hour;
            sum.Hour += (a.Minute + b.Minute) / 60;
            sum.Minute = (a.Minute + b.Minute) % 60;
            return sum;
        }

        public static Time operator -(Time a, Time b)
        {
            Time sub = new Time();
            sub.Hour = a.Hour - b.Hour;
            if (a.Minute >= b.Minute)
            {
                sub.Minute = a.Minute - b.Minute;
            }
            else
            {
                sub.Hour -= 1;
                sub.Minute = a.Minute - b.Minute + 60;
            }
            return sub;
        }

        public static Time Now()
        {
            DateTime dateTime = DateTime.Now;
            return new Time(dateTime.Hour, dateTime.Minute);
        }

        public int ToSeconds()
        {
            return Hour * 3600 + Minute * 60;
        }

        public int ToMinutes()
        {
            return Hour * 60 + Minute;
        }

        public static Time MinuteToTime(int minute)
        {
            return new Time(minute / 60, minute % 60);
        }

        public override string ToString()
        {
            string timeString = "";
            if (Hour < 10)
            {
                timeString += "0" + Hour.ToString();
            }
            else
            {
                timeString += Hour.ToString();
            }
            timeString += ":";
            if (Minute < 10)
            {
                timeString += "0" + Minute.ToString();
            }
            else
            {
                timeString += Minute.ToString();
            }
            return timeString;
        }
    }

    public class Phase
    {
        public string From { get; set; } = "00:00";
        public string To { get; set; } = "00:00";
        public int Duration { get; set; } = 0;
        public int InterruptTime { get; set; } = 0;
        public int Sum { get; set; } = 0;

        public Phase() { }

        public Phase(string from, string to, int duration, int interruptTime, int sum)
        {
            if (checkTimeFormat(from))
            {
                From = from;
            }
            else
            {
                From = "00:00";
            }
            if (checkTimeFormat(to))
            {
                To = to;
            }
            else
            {
                To = "00:00";
            }
            Duration = duration;
            InterruptTime = interruptTime;
            Sum = sum;
        }

        public Phase(string phaseString)
        {
            string[] components = phaseString.Split(' ');
            foreach (string component in components)
            {
                char index = component[0];
                string value = component.Substring(1);
                if (index == 'F')
                {
                    From = value;
                }
                if (index == 'T')
                {
                    To = value;
                }
                if (index == 'D')
                {
                    Duration = Int32.Parse(value);
                }
                if (index == 'I')
                {
                    InterruptTime = Int32.Parse(value);
                }
                if (index == 'S')
                {
                    Sum = Int32.Parse(value);
                }
            }
        }

        public override string ToString()
        {
            string phaseString = "";
            if (Duration != 0)
            {
                phaseString += "\nThời gian mỗi quãng: " + Duration.ToString() + " phút";
            }
            if (InterruptTime != 0)
            {
                phaseString += "\nThời gian nghỉ giữa quãng: " + InterruptTime.ToString() + " phút";
            }
            if (Sum != 0)
            {
                phaseString += "\nTổng thời gian được sử dụng: " + Sum.ToString() + " phút";
            }
            return phaseString;
        }

        public static bool checkTimeFormat(string timeString)
        {
            string[] components = timeString.Split(':');
            if (components.Length != 2)
            {
                return false;
            }

            try
            {
                int hour = Int32.Parse(components[0]);
                if (hour < 0 || hour > 23)
                {
                    return false;
                }
            } catch (FormatException)
            {
                return false;
            }

            try
            {
                int minute = Int32.Parse(components[1]);
                if (minute < 0 || minute > 59)
                {
                    return false;
                }
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }
    }

    public class Phases
    {
        public List<Phase> phaseList { get; set; } = GetPhases();
        public static List<Phase> GetPhases()
        {
            List<Phase> phaseList = new List<Phase>();

            try
            {
                string[] lines = System.IO.File.ReadAllLines(@$"{Constant.APP_DIRECTORY}{Constant.CONFIG_FILE}");
                foreach (string line in lines)
                {
                    Phase newPhase = new Phase(line);
                    phaseList.Add(newPhase);
                }

                return phaseList;
            }
            catch
            {
                return GetPhases();
            }
        }
    }

    public partial class App : Application
    {
    }
}
