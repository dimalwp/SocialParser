using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserFacebook
{
    public class FacebookParser
    {
        private HttpWebRequest request = null;

        public string Email { get; set; }
        public string Pass { get; set; }
        public string LoginPath { get; set; }
        public string UserAgent { get; set; }
        public string Accept { get; set; }
        public string Cookie { get; set; }
        public string IdUser { get; set; }

        public FacebookParser()
        {
            this.Email = "";
            this.Pass = "";
            this.LoginPath = "https://www.facebook.com/login.php?login_attempt=1&lwv=110";
            this.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            this.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            this.Cookie = "wd=1995";
            this.IdUser = "";
        }

        public bool Login()
        {
            request = (HttpWebRequest)HttpWebRequest.Create(this.LoginPath);
            request.Method = "POST";
            request.UserAgent = this.UserAgent;
            request.Accept = this.Accept;
            //request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            //request.Headers.Add("Accept-Language", "uk,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            //request.Headers.Add("Cache-Control", "max-age=0");
            request.ContentType = "application/x-www-form-urlencoded";
            //request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add(HttpRequestHeader.Cookie, this.Cookie);
            request.AllowAutoRedirect = false;

            string sQueryString = "email=" + this.Email + "&pass=" + this.Pass;
            byte[] ByteArr = System.Text.Encoding.GetEncoding(1251).GetBytes(sQueryString);

            request.ContentLength = ByteArr.Length;
            request.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string sCookies = (String.IsNullOrEmpty(response.Headers["Set-Cookie"])) ? "" : response.Headers["Set-Cookie"];

            if (sCookies == "")
                return false;

            string[] arrCookies = sCookies.Split(new[] { ';', ',' });

            Dictionary<string, string> cookies = new Dictionary<string, string>();

            cookies.Add("sb", "");
            cookies.Add("c_user", "");
            cookies.Add("xs", "");
            cookies.Add("fr", "");
            cookies.Add("pl", "");
            cookies.Add("lu", "");

            foreach (string value in arrCookies)
            {
                switch (value.Split('=')[0])
                {
                    case "sb": cookies["sb"] = value.Split('=')[1]; break;
                    case "c_user": cookies["c_user"] = value.Split('=')[1]; break;
                    case "xs": cookies["xs"] = value.Split('=')[1]; break;
                    case "fr": cookies["fr"] = value.Split('=')[1]; break;
                    case "pl": cookies["pl"] = value.Split('=')[1]; break;
                    case "lu": cookies["lu"] = value.Split('=')[1]; break;
                }
            }

            IdUser = cookies["c_user"];

            this.Cookie = string.Format("sb={0}; c_user={1}; xs={2}; fr={3}; pl={4}; lu={5}", cookies["sb"], cookies["c_user"], cookies["xs"], cookies["fr"], cookies["pl"], cookies["lu"]);

            return true;
        }

        public void Login(string email, string pass)
        {
            this.Email = email;
            this.Pass = pass;
            this.Login();
        }

        public string GetResponseHtml(string path)
        {
            request = (HttpWebRequest)HttpWebRequest.Create(path);
            request.Method = "GET";
            request.UserAgent = this.UserAgent;
            request.Accept = this.Accept;
            request.Headers.Add("Cookie", this.Cookie);
            request.AllowAutoRedirect = false;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

            return sr.ReadToEnd();
        }

        public string GetResponseUserFullName(string path)
        {
            string html = this.GetResponseHtml(path);

            Regex reg_full_name = new Regex("(?<=<span id=\"fb-timeline-cover-name\">)(.*?)(?=</span>)", RegexOptions.Multiline);
            Match full_name = reg_full_name.Match(html);

            return full_name.Value;
        }

        public string GetResponseUserBirthday(string path)
        {
            string html = this.GetResponseHtml(this.GetPathUserInformation(path));

            Regex r = new Regex("(?<=День народження</span></div><div>)(.*?)(?=</div>)", RegexOptions.Multiline);
            Match birthday = r.Match(html);

            return birthday.Value;
        }

        public string GetResponseUserNumberPhone(string path)
        {
            string html = this.GetResponseHtml(this.GetPathUserInformation(path));

            Regex r = new Regex("(?<=Телефони</span></div><div><span dir=\"ltr\">)(.*?)(?=</span></div>)", RegexOptions.Multiline);
            Match birthday = r.Match(html);

            return birthday.Value;
        }

        public string GetResponseEmailAddress(string path)
        {
            string html = this.GetResponseHtml(this.GetPathUserInformation(path));

            Regex r = new Regex("(?<=Електронна пошта</span></div><div><a href=\"(.*?)\">)(.*?)(?=</a></div>)", RegexOptions.Multiline);
            Match birthday = r.Match(html);

            return birthday.Value.Replace("&#064;", "@");
        }

        private string GetPathUserInformation(string path)
        {
            string html = this.GetResponseHtml(path);

            // href=(.*?)data-tab-key=\"about\">Інформація | Про себе
            Regex r = new Regex("href=(.*?)data-tab-key=\"about\">", RegexOptions.RightToLeft); 
            Match res = r.Match(html);

            r = new Regex("\"(.*?)\"", RegexOptions.Multiline);
            res = r.Match(res.Value);

            string url = res.Value.Substring(1, res.Value.Length - 2);

            return url.Replace("&amp;", "&");
        }

        // User friends

        private string GetPathUserFriends(string path)
        {
            string html = this.GetResponseHtml(path);

            Regex r = new Regex("href=(.*?)data-tab-key=\"friends\">Друзі", RegexOptions.RightToLeft);
            Match res = r.Match(html);

            r = new Regex("\"(.*?)\"", RegexOptions.Multiline);
            res = r.Match(res.Value);

            string url = res.Value.Substring(1, res.Value.Length - 2);

            return url.Replace("&amp;", "&");
        }

        public Dictionary<string, string> GetUserFriends(string path)
        {
            string html = this.GetResponseHtml(GetPathUserFriends(path));

            Dictionary<string, string> friends = new Dictionary<string, string>();

            Regex reg_div = new Regex("<div class=\"fsl fwb fcb\"(.*?)</div>", RegexOptions.Multiline);

            MatchCollection divs = reg_div.Matches(html);

            Regex reg_url = new Regex("(?<=href=\")(.*?)(?=\" data-gt=\"(.*)</a></div>)", RegexOptions.RightToLeft);
            Regex reg_name = new Regex("(?<=>)(.*?)(?=</a></div>)", RegexOptions.RightToLeft);

            for ( int i = 0; i < divs.Count; i++)
                friends.Add(reg_url.Match(divs[i].Value).Value.Replace("&amp;", "&"), reg_name.Match(divs[i].Value).Value);

            return friends;
        }

        public string[] GetLinkUserFriends(string path)
        {
            return this.GetUserFriends(path).Keys.ToArray();
        }

        public string[] GetNamesUserFriends(string path)
        {
            return this.GetUserFriends(path).Values.ToArray();
        }


        // Returns profileID from page.
        // ProfileID is needed to check user`s uniqueness
        public string GetUserProfileId(string path)
        {
            string html = this.GetResponseHtml(path);

            Regex findRef = new Regex(@"<a class=""profilePicThumb"" .+profile_id=\d+""");
            Regex findId = new Regex(@"\d+""");

            string refer = findRef.Match(html).Value;
            string badId = findId.Match(refer).Value;

            string goodId = badId.TrimEnd('"');

            // link to the user`s profile 
            //string pathComplete = "https://www.facebook.com/profile.php?id=" + goodId;

            return goodId;
        }

    }
}
