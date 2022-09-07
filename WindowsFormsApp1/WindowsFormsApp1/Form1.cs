using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void CompareWeather_Click(object sender, EventArgs e)
        {
            Time t = new Time();
            t.time();
        }
    }
    class Time
    {
        public void time()
        {
            string T1H_t, T1H_y = string.Empty; // 오늘 기온
            int Date_t = int.Parse(DateTime.Now.ToString("yyyyMMdd")); // 오늘 날짜
            string Date_Month = DateTime.Now.ToString("MM"); // 오늘의 달(월)
            string Date_Day = DateTime.Now.ToString("dd"); // 오늘의 일
            string Time_t = DateTime.Now.ToString("HH" + "00"); // 오늘 시간 (프로그램이 실행된 시간)
            int Minute = int.Parse(DateTime.Now.ToString("mm"));
            int Date_y = 0; // 어제 날짜(비교 대상)
            string Time_y = string.Empty; // 어제 시간(비교 대상)

            API_weather api = new API_weather();
            CorrectClock c = new CorrectClock();
            Compare CP = new Compare();

            Date_y = c.Date(Date_t, Date_Month, Date_Day);
            if (Time_t == "0000" && Minute < 40)
                Date_t = Date_y;
            Time_t = c.Time(Time_t, Minute);
            Time_y = c.Time(int.Parse(Time_t));

            T1H_t = api.data(Date_t, Time_t, 60, 127); // 오늘 날씨의 기온을 체크
            T1H_y = api.data(Date_y, Time_y, 60, 127); // 어제 날씨의 기온을 체크

            CP.compare(T1H_t, T1H_y);
        }
    }

    class Compare // 비교 함수
    {
        public void compare(string T1H_t, string T1H_y)
        {
            double T1H_a = Convert.ToDouble(T1H_t);
            double T1H_b = Convert.ToDouble(T1H_y);
            string select = string.Empty;
            double result;
            if (T1H_a > T1H_b)
            {
                select = "어제보다 오늘이 따뜻합니다.";
                result = T1H_a - T1H_b;
            }
            else
            {
                select = "어제보다 오늘이 더 춥습니다.";
                result = T1H_b - T1H_a;
            }
            string Print = select + "어제의 온도는 " + T1H_y + "이고, 오늘의 온도는" + T1H_t + " 입니다. 온도차는" + result + "ºC 입니다.";

            Form2 f2 = new Form2();
            f2.Print(Print);
            f2.ShowDialog();
        }
    }

    class CorrectClock // 비교할 수 있도록 어제의 날짜와 날씨로 바꾸어주는 클래스
    {
        public int Date(int Date_t, string Date_Month, string Date_Day)
        {
            int Date_y = 0;
            if (Date_Day == "01") // 1일 일 때,
            {
                if (Date_Month == "01" || Date_Month == "02" || Date_Month == "04" || Date_Month == "06" || Date_Month == "08" || Date_Month == "09" || Date_Month == "11") // 1, 2, 4, 6, 8, 9, 11 월인 경우에는 31일로 바꾸어 줌 1일 => 31일
                {
                    if (Date_Month == "01") // 1월인 경우 12월이 되고(역으로 숫자가 커짐), 연도가 적어지며, 1일에서 31일이 되므로 -10000 + 1100 + 30
                    {
                        Date_y = Date_t - 8870;
                    }
                    else // 그 외의 경우 모두 월(month)에만 영향을 주고, 1일에서 31일이 되므로 -100 + 30
                    {
                        Date_y = Date_t - 70;
                    }
                }
                else if (Date_Month == "05" || Date_Month == "07" || Date_Month == "10" || Date_Month == "12") // 5, 7, 10, 12 월인 경우에는 30일로 바꾸어 줌 1일 => 30일
                { // - 100 + 29
                    Date_y = Date_t - 71;
                }
                else if (Date_Month == "03") // 3월인 경우에는 28일로 바꾸어 줌 1일 => 28일
                { // -100 + 27
                    Date_y = Date_t - 73;
                }
            }
            else
            {
                Date_y = Date_t - 1;
            }
            return Date_y;
        }


        public string Time(string Time_t, int Minute)
        {
            string[] array = { "0000", "0100", "0200", "0300", "0400", "0500", "0600", "0700", "0800", "0900", "1000",
                "1100", "1200", "1300", "1400", "1500", "1600", "1700", "1800", "1900", "2000", "2100", "2200", "2300" };
            string Time = string.Empty;
            if (Minute < 40)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (Time_t == array[i])
                    {
                        if (Time_t == "0000")
                        {
                            Time = "2300";
                        }
                        else
                            Time = array[i - 1];
                        break;
                    }
                }
            }
            else
                Time = Time_t;
            return Time;
        }

        public string Time(int Time) // 어제의 시간대를 맞춰주는 기능
        {
            string Time_y = string.Empty;
            if (0 <= Time && 200 > Time || 2300 <= Time) //23시 ~ 2시 전까지는
            {
                Time_y = "0200"; // 2시로 제출한다.
            }
            else if (200 <= Time && 500 > Time)
            {
                Time_y = "0500";
            }
            else if (500 <= Time && 800 > Time)
            {
                Time_y = "0800";
            }
            else if (800 <= Time && 1100 > Time)
            {
                Time_y = "1100";
            }
            else if (1100 <= Time && 1400 > Time)
            {
                Time_y = "1400";
            }
            else if (1400 <= Time && 1700 > Time)
            {
                Time_y = "1700";
            }
            else if (1700 <= Time && 2000 > Time)
            {
                Time_y = "2000";
            }
            else if (2000 <= Time && 2300 > Time)
            {
                Time_y = "2300";
            }
            return Time_y;
        }
    }

    class API_weather
    {
        public string data(int base_date, string base_time, int nx, int ny)
        {
            string T1H = string.Empty;
            string Servicekey = "PPTx52BYkVcriTkyPy7xQRLbGM2EjghUwuirUloaERoGWD5XQUYYzD7bB4it3GzR18VgsKSW7gQa1brJQMRYsQ%3D%3D";
            string WeatherAddress = "http://newsky2.kma.go.kr/service/SecndSrtpdFrcstInfoService2/ForecastGrib?" // 초단기 실황 조회 (오늘)
                + "serviceKey=" + Servicekey + "&base_date=" + base_date + "&base_time=" + base_time + "&nx=" + nx + "&ny=" + ny + "&numOfRows=4";

            XmlDocument xml = new XmlDocument();
            xml.Load(WeatherAddress);
            XmlNodeList xmlList = xml.GetElementsByTagName("item");

            int i = 0;
            foreach (XmlNode XNL in xmlList)
            {
                if (i == 3)
                {
                    T1H = Convert.ToString(XNL["obsrValue"].InnerText);
                }
                i++;
            }
            return T1H;
        }
    }
}
