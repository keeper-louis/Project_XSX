using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace KEEPER.K3.WebService
{
    /// <summary>
    /// 日志类
    /// 在类声明中使用sealed可防止其它类继承此类；在方法声明中使用sealed修饰符可防止扩充类重写此方法
    /// </summary>
    public sealed class LogHelper
    {
        private static LogHelper _LogHelper;
        private static readonly object _LockObj = new object();
        public static LogHelper Instance
        {
            get
            {
                lock (_LockObj)
                {
                    if (_LogHelper == null)
                    {
                        _LogHelper = new LogHelper();
                    }
                    return _LogHelper;
                }
            }
        }

        private string logPath = string.Empty;
        private bool _bWriteLog = true;
        private readonly object m_lock = new object();

        /// <summary>
        /// 保存日志的文件夹
        /// </summary>
        private string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    logPath = Path.GetDirectoryName(HttpContext.Current.Request.PhysicalApplicationPath) + @"\log\";
                }
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                return logPath;
            }
            set
            {

                logPath = value;
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
            }
        }

        /// <summary>
        /// 是否写日志标志
        /// </summary>
        public bool bWriteLog
        {
            get
            {
                return _bWriteLog;
            }
            set
            {
                _bWriteLog = value;
            }
        }

        /// <summary>
        /// 写跟踪日志
        /// </summary>
        public void WriteTraceLog(string strMsg)
        {
            lock (m_lock)
            {
                if (_bWriteLog)
                {
                    try
                    {
                        System.IO.StreamWriter sw = System.IO.File.AppendText(LogPath + "TrainLog" + " " + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                        StringBuilder sb = new StringBuilder();
                        sb.Append(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]: "));
                        sb.Append("调试信息:" + strMsg);
                        sw.WriteLine(sb.ToString());
                        sw.Close();
                    }
                    catch (System.IO.IOException IoEx)
                    {
                        WriteErrorLog(IoEx.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        public void WriteErrorLog(string strError)
        {
            lock (m_lock)
            {
                if (_bWriteLog)
                {
                    try
                    {
                        System.IO.StreamWriter sw = System.IO.File.AppendText(LogPath + "ErrorLog" + " " + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                        StringBuilder sb = new StringBuilder();
                        sb.Append(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]: "));
                        sb.Append("错误信息:" + strError);
                        sw.WriteLine(sb.ToString());
                        sw.Close();
                    }
                    catch
                    {

                    }
                }
            }
        }

        /// <summary>
        /// 写警告日志
        /// </summary>
        public void WriteWarningLog(string strError)
        {
            lock (m_lock)
            {
                if (_bWriteLog)
                {
                    try
                    {
                        System.IO.StreamWriter sw = System.IO.File.AppendText(LogPath + "WarnLog" + " " + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                        StringBuilder sb = new StringBuilder();
                        sb.Append(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]: "));
                        sb.Append("警告信息:" + strError);
                        sw.WriteLine(sb.ToString());
                        sw.Close();
                    }
                    catch (System.IO.IOException IoEx)
                    {
                        WriteErrorLog(IoEx.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 写异常日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="detailMessage">错误描述</param>
        public void WriteExceptionLog(Exception ex, string detailMessage)
        {
            lock (m_lock)
            {
                if (_bWriteLog)
                {
                    try
                    {
                        System.IO.StreamWriter sw = System.IO.File.AppendText(LogPath + "ExceptionLog" + " " + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                        StringBuilder sb = new StringBuilder();
                        sb.Append(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]: "));
                        if (!string.IsNullOrEmpty(detailMessage))
                            detailMessage = detailMessage + "      异常信息:" + ex.Message;
                        else
                            detailMessage = "异常信息:" + ex.Message;
                        sb.Append(detailMessage);
                        sw.WriteLine(sb.ToString());
                        sw.Close();
                    }
                    catch
                    {

                    }
                }
            }
        }

        public void WriteTraceLog(string stacktrace, string strMsg)
        {
            string strDugInfo = string.Format("调试：{0}，错误信息:{1}", stacktrace, strMsg);
            this.WriteErrorLog(strDugInfo);
        }
        

    }
}