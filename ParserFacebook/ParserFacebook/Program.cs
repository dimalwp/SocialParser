using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParserFacebook
{
    class Program
    {
        static void Main(string[] args)
        {
            FacebookParser fp = new FacebookParser();

            //fp.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            //fp.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

            
            fp.Login("email", "pass");
            if (fp.IdUser == "")
            {
                Console.WriteLine("NOT LOGIN!!");
                Console.ReadKey();
            }
            //for (int i = 0; i < 5; i++)
            //{
            //    if (fp.IdUser != "") break;
            //    fp.Login();
            //    if (i == 9) Console.WriteLine("Not Login");
            //    Console.WriteLine("...");
            //}

            //string html = fp.GetResponseHtml("https://www.facebook.com/profile.php?id=100003197080384");

            //string name = fp.GetResponseUserName("https://www.facebook.com/profile.php?id=100017316760657");

            //Console.WriteLine(fp.Cookie);

            //Dictionary<string, string> friends = fp.GetUserFriends("https://www.facebook.com/profile.php?id=100004674946665");

            //foreach (var friend in friends)
            //    Console.WriteLine("name: {0}  url: {1} ", friend.Value, friend.Key);

            //string name = fp.GetResponseUserName(friends.ElementAt(0).Key);

            string[] arr = fp.GetLinkUserFriends("https://www.facebook.com/profile.php?id=100004674946665");

            foreach (var v in arr)
                Console.WriteLine(v);

            Console.WriteLine(fp.GetResponseUserFullName("https://www.facebook.com/profile.php?id=100017368845593"));
            Console.WriteLine(fp.GetResponseEmailAddress("https://www.facebook.com/profile.php?id=100017368845593"));

            Console.WriteLine(fp.GetResponseUserFullName("https://www.facebook.com/oleg.kotkevych"));
            Console.WriteLine(fp.GetResponseUserBirthday("https://www.facebook.com/oleg.kotkevych"));
            Console.WriteLine(fp.GetResponseUserNumberPhone("https://www.facebook.com/oleg.kotkevych"));

           
            //StreamWriter sw = new StreamWriter("D:\\f.txt");
            //sw.Write(arr);
            //sw.Close();

            //Console.WriteLine(fp.GetResponseUserName(arr[0]));
            Console.ReadKey();
        }
    }
}
