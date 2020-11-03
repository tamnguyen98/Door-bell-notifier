using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WindowsFormsApp1
{
    public class ThreadedCommunication
    {
        public void ConnectTo(string url)
        {
            using (var client = new HttpClient())
            {
                while (true)
                {
                    var response = client.GetAsync("http://192.168.1.129/").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content;

                        // by calling .Result you are synchronously reading the result
                        string responseString = responseContent.ReadAsStringAsync().Result;

                        Console.WriteLine(responseString);
                    }
                }
            }
        }

        // Show the notification on the desktop (Main display)
        public void Alert(string msg, Form_Alert.enmType type)
        {
            Form_Alert frm = new Form_Alert();
            frm.showAlert(msg, type);
        }
    }
}
