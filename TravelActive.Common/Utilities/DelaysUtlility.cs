using System;

namespace TravelActive.Common.Utilities
{
    public static class DelaysUtlility
    {
        public static string DelayToStringWithSeconds(int delay)
        {
            string delayStr = delay.ToString("000000");
            return $"{delayStr.Substring(0, 2)}:{delayStr.Substring(2, 2)}:{delayStr.Substring(4, 2)}";
        }
        //Format
        // 9 минути
        // 1 час и 10 минути
        public static string DelayToInformationString(int delay)
        {
            if (delay < 10000)
            {
                return $"{delay / 100} минути";
            }
            else
            {
                return $"{delay / 10000} часа и {delay / 100 % 100} минути";
            }
        }
        public static string DelayToStringWithoutSeconds(int delay)
        {
            string delayStr = delay.ToString("000000");
            return $"{delayStr.Substring(0, 2)}:{delayStr.Substring(2, 2)}";
        }

        public static int DelaysSubstract(int delay1, int delay2)
        {
            int delay1Seconds = delay1 % 100;
            int delay2Seconds = delay2 % 100;
            int delay1Minutes = delay1 / 100 % 100;
            int delay2Minutes = delay2 / 100 % 100;
            int delay1Hours = delay1 / 10_000;
            int delay2Hours = delay2 / 10_000;
            int resultSeconds = 0;
            int resultMinutes = 0;
            int resultHours = 0;
            int substract = 0;
            if (delay1Seconds - delay2Seconds < 0)
            {
                resultSeconds = delay1Seconds - delay2Seconds + 60;
                substract = 1;
            }
            else
            {
                resultSeconds = delay1Seconds - delay2Seconds;
            }

            if (delay1Minutes - delay2Minutes - substract < 0)
            {
                resultMinutes = delay1Minutes - delay2Minutes - substract + 60;

                substract = 1;
            }
            else
            {
                resultMinutes = delay1Minutes - delay2Minutes - substract;
                substract = 0;
            }

            if (delay1Hours - delay2Hours - substract < 0)
            {
                resultHours = delay1Hours - delay2Hours - substract + 24;
            }
            else
            {
                resultHours = delay1Hours - delay2Hours - substract;
            }

            return resultHours * 10_000 + resultMinutes * 100 + resultSeconds;
        }

        public static int DelayAddition(int delay1, int delay2)
        {
            int delay1Seconds = delay1 % 100;
            int delay2Seconds = delay2 % 100;
            int delay1Minutes = delay1 / 100 % 100;
            int delay2Minutes = delay2 / 100 % 100;
            int delay1Hours = delay1 / 10_000;
            int delay2Hours = delay2 / 10_000;
            int resultSeconds = 0;
            int resultMinutes = 0;
            int resultHours = 0;
            int add = 0;
            if (delay1Seconds + delay2Seconds < 60)
            {
                resultSeconds = delay1Seconds + delay2Seconds;
            }
            else
            {
                resultSeconds = delay1Seconds + delay2Seconds - 60;
                add = 1;
            }

            if (delay1Minutes + delay2Minutes + add < 60)
            {
                resultMinutes = delay1Minutes + delay2Minutes + add;
                add = 0;
            }
            else
            {
                resultMinutes = delay1Minutes + delay2Minutes + add - 60;
                add = 1;
            }

            if (delay1Hours + delay2Hours + add < 24)
            {
                resultHours = delay1Hours + delay2Hours + add;
            }
            else
            {
                resultHours = delay1Hours + delay2Hours + add - 24;
            }

            return resultHours * 10_000 + resultMinutes * 100 + resultSeconds;
        }
        // Divide and conquer should be the fastest solution for the problem
        public static int Length(int delay)
        {
            // 4 or less
            if (delay < 10000)
            {
                //3 or less
                if (delay < 1000)
                {
                    // 2 or 1
                    if (delay < 100)
                    {
                        if (delay < 10)
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    return 4;
                }
            }
            // 5 or 6 
            else
            {
                if (delay < 100000)
                {
                    return 5;
                }
                else
                {
                    return 6;
                }
            }
        }

        public static int ConvertDateTimeToInt(DateTime now)
        {
            return now.Hour * 10000 + now.Minute * 100 + now.Second;
        }
        /// <summary>
        /// Parses string in format 00:00:00 or 00:00
        /// </summary>
        /// <param name="delayString"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static int ParseDelay(string delayString, string format)
        {
            if (delayString == "")
            {
                return 0;
            }
            if (format == "00:00")
            {
                return (int)(delayString[0] - 48) * 1000 + (int)(delayString[1] - 48) * 100 + (int)(delayString[3] - 48) * 10 + (int)delayString[4];
            }
            else
            {
                return (delayString[0] - 48) * 100_000 + (delayString[1] - 48) * 10_000 + (delayString[3] - 48) * 1000 +
                       (delayString[4] - 48) * 100 + (delayString[6] - 48) * 100 + (delayString[7] - 48);
            }
        }
    }
}