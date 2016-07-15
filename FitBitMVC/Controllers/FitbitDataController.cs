using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FitBitMVC.Models;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OAuth;

namespace FitBitMVC.Controllers
{
    public class FitbitDataController : Controller
    {
        // GET: FitbitData
        public ActionResult Index()
        {
            return View();
        }

        public string Index2()
        {
            return "This is a <b>test</b> </br> <button content=\"hi\">";
        }

        // GET: FitbitData/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FitbitData/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FitbitData/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FitbitData/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FitbitData/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FitbitData/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FitbitData/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public void UpdateFitbitDataModel(FitbitCredentials user)
        {
            FitbitDataModel model = new FitbitDataModel();
            model.UpdatePariticpant(user);
        }

        public string GetData()
        {
            string results = "";
            using (var client = new HttpClient())
            {
                //var uri = new Uri("https://api.fitbit.com");
                ////var authzHeader = 



                //var response = await client.GetAsync(uri);


                //string result = await response.Content.ReadAsStringAsync();

                FitbitDataModel model = new FitbitDataModel();
                FitbitCredentials user = model.GetCredentials("4QTKJN");
                //if (user.TimeStamp + new TimeSpan(1, 0, 0) < DateTime.Now)
                //{
                    try
                    {
                        using (var wb = new WebClient())
                        {
                            var token = Base64Encode("227VF3:a62f5ae60b91c3a9f9ebeb6af1c8d0c8");
                            var data = new NameValueCollection();
                            data["grant_type"] = "refresh_token";
                            //data["client_id"] = "227VF3";
                            //data["redirect_uri"] = "http://derp.ronboxlocal.com:4136/signin-fitbit";
                            data["refresh_token"] = user.RefreshToken;

                            wb.Headers["Authorization"] = "Basic " + token;

                            var response = wb.UploadValues("https://api.fitbit.com/oauth2/token", "POST", data);
                            var responseString = Encoding.ASCII.GetString(response);

                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof (FitbitCredentials));

                            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
                            {
                                user = (FitbitCredentials) ser.ReadObject(stream);
                            }

                        }
        
                        model.UpdatePariticpant(user);
                    }
                    catch (Exception ex)
                    {
                        results += " refresh token failed: " + ex.Message + " </br>";
                    }


                //}

                string urlworkout = "https://api.fitbit.com/1/user/" + user.UserID + "/activities/date/2016-07-22.json";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlworkout);
                request.Method = "GET";
                request.Headers["Authorization"] = "Bearer " + user.AccessToken;
                request.Accept = "application/json";

                WebResponse myResponse;
                //string results = "";
                try
                {
                    myResponse = request.GetResponse();
                    StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream());
                    results = httpwebStreamReader.ReadToEnd();

                    myResponse.Close();
                    httpwebStreamReader.Close();

                }
                catch (Exception ex)
                {
                    results += "request failed: " + ex.Message + "</br>";
                }

                
            }

            return "here's the data: </br>" + results + " </br> and the end of the data. ";
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
