using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ConsoleTool
{
    public class WebTunnelProvider : SingletonManager<WebTunnelProvider>
    {
        #region Fields and Properities
        internal Thread ListeningThread = null;
        private HttpListener listener = null;
        private Encoding encoding = null;
        private int tunnelPort = 24402;
        private JsonSerializerSettings jsonSetting;
        private readonly Regex rOperation = new Regex("[/](?<Data>.*?)[?][&]?.*");

        #endregion

        #region Methods
        public void InitialAcceptor(int tunnelPort)
        {
            this.tunnelPort = tunnelPort;
            encoding = Encoding.Default;
            ListeningThread = new Thread(new ThreadStart(StartListening))
            {
                IsBackground = Environment.OSVersion.Platform != PlatformID.Unix
            };
            ListeningThread.Start();
        }

        public override void Init()
        {
            base.Init();
            jsonSetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            IsoDateTimeConverter java = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff"
            };
            jsonSetting.Converters.Add(java);
        }

        public void StartListening()
        {
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add("http://+:" + tunnelPort + "/");
                //listener.Prefixes.Add("http://");
                listener.Start();
                Console.WriteLine($"启动主机成功");
                while (true)
                {
                    IAsyncResult result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
                    result.AsyncWaitHandle.WaitOne();
                }
            }
            catch (Exception e)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"启动主机出错。错误信息：{e}");
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            try
            {

                HttpListener listener = (HttpListener)result.AsyncState;
                HttpListenerContext context = listener.EndGetContext(result);
                ProcessRequest(context);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            try
            {
                string url = HttpUtility.UrlDecode(request.RawUrl);
                Match match = rOperation.Match(request.RawUrl);
                PadResponseData responseData = new PadResponseData
                {
                    Success = false
                };
                if (request.HttpMethod.ToLower() == "post")
                {
                    try
                    {
                        StreamReader streamReader = new StreamReader(request.InputStream);
                        string paramsString = streamReader.ReadToEnd();
                        paramsString = HttpUtility.UrlDecode(paramsString);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        //Console.WriteLine($"收到数据{paramsString}");
                        Console.WriteLine($"收到数据");
                        responseData.Success = true;
                        //responseData.Data =new Dictionary<string, object>() {["shoudao"] =$"收到数据{paramsString}" } ;
                        responseData.Data = new Dictionary<string, object>() { ["shoudao"] = $"收到数据" };
                    }
                    catch
                    {
                    }
                }
                
                if (responseData.Success)
                {
                    Success(context, responseData);
                }
                else
                {
                    Fail(context);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("出错。。。。");
                    //context.Response.StatusCode = 1234;
                    //context.Response.Close();
                }
                #region MyRegion
                //Match match = rOperation.Match(request.RawUrl);
                //if (match.Success)
                //{
                //    if (match.Groups["Data"].Value == "SendEvaluateData")
                //    {
                //        PadResponseData responseData = new PadResponseData
                //        {
                //            Success = false
                //        };
                //        if (request.HttpMethod.ToLower() == "post")
                //        {
                //            try
                //            {
                //                StreamReader streamReader = new StreamReader(request.InputStream);
                //                string paramsString = streamReader.ReadToEnd();
                //                paramsString = HttpUtility.UrlDecode(paramsString);
                //                responseData.Success = true;
                //            }
                //            catch
                //            {
                //            }
                //        }
                //        Success(context, responseData);
                //    }
                //}
                //else
                //{
                //    Fail(context);
                //    //context.Response.StatusCode = 1234;
                //    //context.Response.Close();
                //} 
                #endregion
            }
            catch (Exception e)
            {

            }
            finally
            {
            }
        }

        private void ProcessPhotoResponse(HttpListenerContext context, byte[] buffer)
        {
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.ContentType = "image/jpg";
            context.Response.ContentLength64 = buffer.Length;
            var output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private void Success(HttpListenerContext context, object data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //context.Response.Headers.Add("Access-Control-Allow-Headers", ",Content-Type, Content-Length, Authorization, Accept, X-Requested-With , yourHeaderFeild");
            //context.Response.Headers.Add("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS");
            ////context.Response.Headers.Add("X-Powered-By", "3.2.1");
            //context.Response.Headers.Add("Content-Type", "application/json;charset=utf-8");
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.Close();
        }

        private void Fail(HttpListenerContext context)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("不匹配哦");
            //context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);          
            context.Response.Close();
        }

        private void NotFound(HttpListenerContext context)
        {
            context.Response.StatusCode = 404;
            context.Response.Close();
        }

        internal void AbortManage()
        {
            try
            {
                if (listener != null)
                {
                    listener.Stop();
                }
            }
            catch
            {
            }
            try
            {
                if (ListeningThread != null)
                    ListeningThread.Abort();
            }
            catch
            {
            }
            ListeningThread = null;
            listener = null;
        }
        #endregion
    }
}
