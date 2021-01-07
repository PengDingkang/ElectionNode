using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebNode.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        static readonly HttpClient httpClient = new HttpClient();
        private static int leader = 0;
        private static int trust = 0;
        private static List<Node> voter = new List<Node>();
        private static bool startFlag = false;
        private static bool leaderFlag = false;
        [Route("api/start")]
        [HttpPost]
        public IActionResult Start()
        {
            var starter = new
            {
                starter = GlobalVars.NodeNumber
            };

            var json = JsonConvert.SerializeObject(starter);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            foreach (string node in GlobalVars.OtherNodes)
            {
                try
                {
                    httpClient.PostAsync($"{node}/api/startdash", data);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
            return Ok("start");
        }

        [Route("api/startdash")]
        [HttpPost]
        public IActionResult StartDash([FromBody] object jsonStr)
        {
            int starter;
            try
            {
                JObject jo = JObject.Parse(jsonStr.ToString());
                starter = jo.Value<int>("starter");
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Invalid input" });
            }
            if (starter == GlobalVars.NodeNumber)
            {
                leaderFlag = true;
            }
            trust = starter;
            startFlag = true;
            return Ok($"{GlobalVars.NodeNumber} Started");
        }

        [Route("api/heartbeat")]
        [HttpGet]
        public IActionResult GetHeartBeart([FromQuery] int client)
        {
            int voters = 0;
            bool newNode = true;

            foreach(Node node in voter)
            {
                if(node.clientId == client)
                {
                    node.Refresh();
                    newNode = false;
                }
                if (!node.Expired())
                {
                    voters++;
                }
            }
            if (newNode)
            {
                Node addNode = new(client);
                voter.Add(addNode);
                voters++;
            }
            if (voters > GlobalVars.NodeAmount / 2)
            {
                ApiController.leader = GlobalVars.NodeNumber;
                leaderFlag = true;
                var leader = new
                {
                    node = GlobalVars.NodeNumber
                };

                var json = JsonConvert.SerializeObject(leader);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                foreach (string node in GlobalVars.OtherNodes)
                {
                    try
                    {
                        httpClient.PostAsync($"{node}/api/setleader", data);
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }
                }
            }
            Console.WriteLine($"Get heartbeat from {client}");
            return Ok($"success form {GlobalVars.NodeNumber}");
        }

        [Route("api/setleader")]
        [HttpPost]
        public IActionResult SetLeader([FromBody] object jsonStr)
        {
            try
            {
                JObject jo = JObject.Parse(jsonStr.ToString());
                ApiController.leader = jo.Value<int>("node");
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Invalid input" });
            }
            if (!startFlag)
            {
                startFlag = true;
                trust = leader;
            }
            return Ok($"Node {GlobalVars.NodeNumber} set leader {leader}");
        }

        [NonAction]
        public static Task Run()
        {
            while (true)
            {
                if (leaderFlag)
                {
                    continue;
                }
                else if (startFlag)
                {
                    HeartBeat();
                }
                Thread.Sleep(10000);
            }
        }

        [NonAction]
        private async static void HeartBeat()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{GlobalVars.OtherNodes[trust]}/api/heartbeat?client={GlobalVars.NodeNumber}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException)
            {
                trust = leader + 1;
                if (trust >= GlobalVars.NodeAmount)
                {
                    trust = 0;
                }

                Console.WriteLine($"Leader {leader} failed, vote to next node {trust}");
            }
        }
    }

    public class Node
    {
        public int clientId { get; set; }
        public DateTime setTime { get; set; }
        public DateTime expireTime { get; set; }
        public Node(int clientId)
        {
            this.clientId = clientId;
            Refresh();
        }
        public bool Expired()
        {
            if (expireTime > setTime)
            {
                return false;
            }
            else return true;
        }
        public void Refresh()
        {
            setTime = DateTime.Now;
            expireTime = setTime.AddSeconds(10);
        }
    }
}
