using System;
using System.IO;
using System.Text.RegularExpressions;

namespace GoogleHosts
{
    public class Hosts
    {
        private const string HostFileUrl = "http://www.360kb.com/kb/2_122.html";
        private const string HostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";

        private Log _log = new Log();

        public string Message { get; private set; }

        public void MainProcess()
        {
            Message = "执行";
            _log.Write(Message);

            EasyWebRequest request = new EasyWebRequest();
            string html = "";
            string file = "";
            try
            {
                html = request.Request(HostFileUrl, RequestMethod.GET);
            }
            catch(Exception ex)
            {
                Message = "获取网页源码时异常-" + ex.Message;
                _log.Write(Message);
                return;
            }

            try
            {
                using (FileStream fileStream = new FileStream(HostsFilePath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fileStream))
                    {
                        file = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Message = "读取本机hosts文件时异常" + ex.Message;
                _log.Write(Message);
                return;
            }

            if (!string.IsNullOrWhiteSpace(html))
            {
                DateTime? dtr, dtl;

                bool update = CheckUpdate(html, file, out dtr, out dtl);
                if (update)
                {
                    const string pattern = @"#google hosts[\s\S]+#google hosts[\s\S]+?end";
                    string hostsr = Regex.Match(html, pattern).ToString();
                    if (!string.IsNullOrWhiteSpace(hostsr))
                    {
                        string hostsl = Regex.Match(file, pattern).ToString();
                        if (string.IsNullOrWhiteSpace(hostsl))
                        {
                            using (StreamWriter sw = new StreamWriter(HostsFilePath, true))
                            {
                                sw.WriteLine();
                                sw.WriteLine();
                                sw.WriteLine(hostsr);
                            }
                            Message = string.Format("追加hosts信息成功,hosts日期:{0}", dtr == null ? "" : ((DateTime) dtr).ToShortDateString());
                            _log.Write(Message);
                        }
                        else
                        {
                            using (StreamWriter sw = new StreamWriter(HostsFilePath, false))
                            {
                                sw.Write(file.Replace(hostsl, hostsr));
                            }
                            Message = string.Format("更新hosts信息成功,hosts日期:{0}", dtr == null ? "" : ((DateTime) dtr).ToShortDateString());
                            _log.Write(Message);
                        }
                    }
                    else
                    {
                        Message = "获取网页源码中的hosts信息失败";
                        _log.Write(Message);
                    }
                }
                else
                {
                    Message = "相关Google hosts已经是最新,不需要更新";
                    _log.Write(Message);
                }
            }
            else
            {
                Message = "获取网页源码为空";
                _log.Write(Message);
            }
        }

        /// <summary>
        /// 检查是否可更新
        /// </summary>
        /// <param name="html">远端网页源码</param>
        /// <param name="file">本地hosts文件内容</param>
        /// <param name="dt1">返回远端hosts日期</param>
        /// <param name="dt2">返回本地hosts日期</param>
        /// <returns></returns>
        private bool CheckUpdate(string html, string file, out DateTime? dt1, out DateTime? dt2)
        {
            dt1 = null;
            dt2 = null;

            const string pattern = @"#google hosts (.+)";
            MatchCollection matches = Regex.Matches(html, pattern);
            if (matches.Count > 0)
            {
                string tmp = matches[0].Groups[1].Value;
                DateTime dtr = new DateTime();
                if (DateTime.TryParse(tmp, out dtr))
                {
                    dt1 = dtr;
                    matches = Regex.Matches(file, pattern);
                    if (matches.Count > 0)
                    {
                        tmp = matches[0].Groups[1].Value;
                        DateTime dtl = new DateTime();
                        if (DateTime.TryParse(tmp, out dtl))
                        {
                            dt2 = dtl;
                            if (dtr > dtl)
                            {
                                return true;
                            }
                            return false;
                        }
                        _log.Write("转换本机hosts中的日期为DateTime时出错");
                        return false;
                    }
                    return true;
                }
                _log.Write("转换网页源码中的日期为DateTime时出错");
                return false;
            }
            _log.Write("解析网页源码中的日期时异常");
            return false;
        }
    }
}