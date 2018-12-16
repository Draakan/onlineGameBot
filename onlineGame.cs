using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium;
using System.Timers;
using System.IO;

namespace XOR
{
    public static class MyLog
    {
        public static void LogToFile(string str)
        {
            string filePath = @"D:\onlineGameLog.txt";

            using (FileStream fs = new FileStream(filePath, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(str + "\n");
                }
            }
        }
    }

    class Program
    {
        static PhantomJSDriver driver = new PhantomJSDriver();
        static bool isMutated = true, isPatruling = false, haveTainiki = true;
        static Timer timer;
        static int countOfFights = 0;

        static void Login()
        {
            driver.Navigate().GoToUrl("http://v.time2play.mobi/game");

            driver.FindElementById("session_login").SendKeys("asfedit");
            driver.FindElementById("session_password").SendKeys("maksim333");
            driver.FindElementByXPath("//input[@value='Войти']").Click();

            Console.Clear();
        }

        static void Main(string[] args)
        {
            Login();

            MyLog.LogToFile($"starting program => {DateTime.Now}");

            while (true)
            {
                try
                {
                    try
                    {
                        driver.Navigate().GoToUrl("http://v.time2play.mobi/game/witch/access_ritual?class=button&amp;id=experience");

                        driver.FindElementByClassName("button_medium").Click();
                        driver.FindElementByXPath("//div[@class='center']/div/a").Click();

                        MyLog.LogToFile($"mutation was carried out => {DateTime.Now}");

                        isMutated = true;
                    }
                    catch
                    {
                        try
                        {
                            if (driver.FindElementByXPath("//div[@class='partial_time_ritual']/div[@class='block']").Text.Substring(0, 13).Equals("действует еще"))
                            {
                                isMutated = true;
                                Console.WriteLine("действует еще");
                            }
                            else if (driver.FindElementByXPath("//div[@class='partial_time_ritual']/div[@class='block']").Text.Substring(0, 14).Equals("доступна через"))
                            {
                                isMutated = false;
                                Console.WriteLine("доступна через");
                            }
                        }
                        catch { }
                    }

                    if (isMutated && !isPatruling)
                    {
                        try
                        {
                            driver.Navigate().GoToUrl("http://v.time2play.mobi/game/mine");
                            driver.FindElementByXPath("//a[@class='button_medium'][2]").Click();

                            MyLog.LogToFile($"mining - true => {DateTime.Now}");

                            System.Threading.Thread.Sleep(3000);
                        }
                        catch
                        {
                            string startSkip = "/game/mine/skip?drid=";

                            string oboroten = driver.PageSource.Substring(driver.PageSource.IndexOf(startSkip) + 21, 5);

                            for (int i = 0; i < 5; i++)
                                if (Char.IsDigit(oboroten[i]))
                                    startSkip += oboroten[i];

                            if (startSkip.Equals("/game/mine/skip?drid=") || startSkip.Equals("/game/mine/skip?drid=0"))
                                haveTainiki = false;
                            else
                            {
                                driver.Navigate().GoToUrl("http://v.time2play.mobi" + startSkip);
                                System.Threading.Thread.Sleep(65000);
                            }
                        }

                        driver.Navigate().GoToUrl("http://v.time2play.mobi/game/fight/find?level=25%2C26%2C27");

                        string count = driver.FindElementByXPath("//div[@class='footer_icons']/span[10]/a[2]").Text;

                        countOfFights = int.Parse(count[0].ToString());

                        if (countOfFights > 0)
                        {
                            try
                            {
                                if (int.Parse(driver.FindElementByXPath("//div[@class='opponent_info']/div/span").Text) < 800)
                                {
                                    driver.FindElementByXPath("//div[@class='opponent_block']/div[@class='center']/div[@class='block']/a").Click();

                                    MyLog.LogToFile($"hit an enemy => {DateTime.Now}");

                                    System.Threading.Thread.Sleep(65000);
                                }
                            }
                            catch
                            {
                                driver.Navigate().GoToUrl("http://v.time2play.mobi/game");

                                System.Threading.Thread.Sleep(65000);
                            }
                        }
                        else
                        {
                            if (!isPatruling && (countOfFights == 0) && (haveTainiki == false))
                            {
                                try
                                {
                                    driver.Navigate().GoToUrl("http://v.time2play.mobi/game/patrol");
                                    driver.FindElementByXPath("//select[@id='time']/option[@value='14400']").Click();
                                    driver.FindElementByXPath("//input[@value='Служба']").Click();

                                    MyLog.LogToFile($"start patruling => {DateTime.Now}");

                                    isPatruling = true;

                                    timer = new Timer();
                                    timer.Interval = 660000;
                                    timer.Elapsed += OnTimeElapsed;
                                    timer.AutoReset = false;
                                    timer.Enabled = true;
                                }
                                catch { }
                            }

                            System.Threading.Thread.Sleep(65000);
                        }
                    }
                    else if (isMutated && isPatruling)
                    {
                        MyLog.LogToFile($"patruling true => {DateTime.Now}");
                        System.Threading.Thread.Sleep(300000);
                    }
                    else if (!isMutated)
                    {
                        Console.WriteLine("not mutated");
                        System.Threading.Thread.Sleep(300000);
                    }
                }
                catch(Exception e)
                {
                    MyLog.LogToFile($"{e.Message} => {DateTime.Now}");
                    System.Threading.Thread.Sleep(65000);
                    driver.Navigate().GoToUrl("http://v.time2play.mobi/game");
                }
            }
        }

        private static void OnTimeElapsed(object sender, ElapsedEventArgs e)
        {
            isPatruling = false;
            timer.Enabled = false;

            MyLog.LogToFile($"patruling is over => {DateTime.Now}");
        }
    }
}