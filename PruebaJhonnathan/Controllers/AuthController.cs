using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.IO;
using System.Text;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PruebaJhonnathan.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
           return View();
        }

        public IActionResult Logout()
        {
            return View("../Auth/Login");
        }

        public async Task<ActionResult> Index(string User, string Password, string Ip, string UserAgent)
        {
            var jsonFormat = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient()) {
                var urlApi = "https://bpoamericas.co/Prueba/SecurityAPI/Security/LoginUser";
                var urlToken = "https://bpoamericas.co/Prueba/SecurityAPI/Security";
                var tokenUser = "T3stU53r!";
                var tokenPass = "T35tP455w0rd#";
                var token = "";
                using (var resquestMessage = new HttpRequestMessage(HttpMethod.Get, urlToken))
                {
                    resquestMessage.Headers.Add("TokenUser", tokenUser);
                    resquestMessage.Headers.Add("TokenPassword", tokenPass);
                    var Response = await httpClient.SendAsync(resquestMessage);
                    var ContentString = await Response.Content.ReadAsStringAsync();
                    var responseJson = JsonSerializer.Deserialize<dynamic>(ContentString);
                    var token2 = responseJson.GetProperty("token");
                    token = ""+token2;
                }

                string cadenaBase64User = string.Empty;
                byte[] encryted = System.Text.Encoding.UTF8.GetBytes(User);
                cadenaBase64User = Convert.ToBase64String(encryted);

                string cadenaBase64Pass = string.Empty;
                byte[] encryted2 = System.Text.Encoding.UTF8.GetBytes(Password);
                cadenaBase64Pass = Convert.ToBase64String(encryted2);

                var values = new Dictionary<string, string>
                {
                    { "UserName", cadenaBase64User },
                    { "UserPassword", cadenaBase64Pass },
                    { "ClientIP", Ip },
                    { "UserAgent", UserAgent }
                };

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, urlApi);
                request.Content = new StringContent("{\"UserName\":\"" + cadenaBase64User + "\",\"UserPassword\":\"" + cadenaBase64Pass + "\",\"ClientIP\":\"" + Ip + "\",\"UserAgent\":\"" + UserAgent + "\"}",
                                                    Encoding.UTF8,
                                                    "application/json");

                var responseLogin = await client.SendAsync(request);
                var responseString = await responseLogin.Content.ReadAsStringAsync();
                var responseLoginJson = JsonSerializer.Deserialize<dynamic>(responseString);
                if (((int)responseLogin.StatusCode) == 200)
                {
                    return View("../Home/Index");
                } else
                {
                    return View("Login");
                }

            }
        }
    }
}
