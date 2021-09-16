using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.IO;
using System.Net;
using MopUpData.Models;
using TaskFSE = MopUpData.Models.TaskFSE;
using Object = MopUpData.Models.Object;
using System.Threading;

namespace MopUpData.Helpers
{

    public class APIHelper
    {
        public string apiResponse;
        private string _district;
        private string _status = null;
        //private int _taskCount = 0;

        private int startNumber = 0;
        private int topNumber =500;
        private int overalCounts;
        private string _username;
        private string _password;
        private bool _isSandBox;

        private List<TaskFSE> tasksOveralAmount;
        
        const string sbLink = "https://fse-na-sb-int01.cloud.clicksoftware.com";
        const string prodLink = "https://fse-na.cloud.clicksoftware.com";
        private string requestString;
        private string link = prodLink;

        public void CallFSE(string username, string password, string status, bool isSandBox)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            _status = status;
            _username = username;
            _password = password;
            _isSandBox = isSandBox;
            tasksOveralAmount = new List<TaskFSE>();

            overalCounts = 0;

            requestString = $"/SO/api/objects/Task?&$filter=TaskColor/Key eq -1 and Status/Name eq '{_status}'&$select=CallID,Number&$count=true&$skip={startNumber}&$top={topNumber}";

            if (_isSandBox)
            {
                link = sbLink;
            }

            overalCounts = GetTotalTaskCount();

            if (overalCounts < 0)
            {
                throw new Exception("No Tasks for update");
            }

            ProgressBarModel report = new ProgressBarModel();
            List<Task<string>> asyncTasks = new List<Task<string>>();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            while (startNumber < overalCounts)
            {
                var tasks = CreateTaskList(startNumber);
                tasksOveralAmount.AddRange(tasks);
                startNumber += topNumber;

                if (startNumber > overalCounts)
                {
                    startNumber = overalCounts;
                }
            }

            var timeExecution = watch.ElapsedMilliseconds;
            TimeSpan t = TimeSpan.FromMilliseconds(timeExecution);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);

            apiResponse = "\n-------------" + DateTime.Now.ToString() + "------------ \n" + "------- Recieved Tasks for update: " + overalCounts + " \n Server respons:  ";
            WriteLogs(apiResponse);
        }

        private List<TaskFSE> CreateTaskList(int startNumber)
        {
            List<TaskFSE> tasks = new List<TaskFSE>();
            var tasksResponse = GetTask(startNumber);

            string items = JObject.Parse(tasksResponse.Result).SelectToken("Items").ToString();
            tasks = JsonConvert.DeserializeObject<List<TaskFSE>>(items);
            
            apiResponse = "\n-------------" + DateTime.Now.ToString() + "------------ \n" + "------- Sending request for " + startNumber + " out of " + overalCounts;
            WriteLogs(apiResponse);

            return tasks;
        }


        public async Task<string> UpdateTasksInParallel()
        {
            if(overalCounts <= 0)
            {
                throw new Exception("No Tasks recieved from FSE");
            }
            //string response = null;
            int _taskCount = 0;
            List<Task<string>> asyncTasks = new List<Task<string>>();

            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (_taskCount < overalCounts)
            {
                var task = UpdateTaskAsync(tasksOveralAmount, _taskCount);
                asyncTasks.Add(task);
                _taskCount += 99;
            }
            var results = await Task.WhenAll(asyncTasks);

            var timeExecution = watch.ElapsedMilliseconds;
            TimeSpan t = TimeSpan.FromMilliseconds(timeExecution);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);

            apiResponse = "\n-------------Time:  " + t + "------------ \n" + _taskCount + "------- Tasks were sent for update the TaskColor to  " + _status + " updated ";
            WriteLogs(apiResponse);
            return results[0];
        }
        private int GetTotalTaskCount()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new RestClient($"{link}{requestString}");
            client.Timeout = -1;

            var request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}")));
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                overalCounts = Int32.Parse(JObject.Parse(response.Content).SelectToken("Count").ToString());

                apiResponse = "\n-------------" + DateTime.Now.ToString() + "------------ \n" + "------- Overal Tasks coount " + overalCounts + " \n Server respons:  " + response.StatusCode;
                WriteLogs(apiResponse);
            }
            else
            {
                overalCounts = 0;
                apiResponse = "\n-------------" + DateTime.Now.ToString() + "------------ \n" + "------- Overal Tasks coount " + overalCounts + " \n Server respons:  " + response.StatusCode;
                WriteLogs(apiResponse);
            }
                

            return overalCounts;
        }

        private async Task<string> GetTask(int startNumber)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new RestClient($"{link}/SO/api/objects/Task?&$filter=TaskColor/Key eq -1 and Status/Name eq '{_status}'&$select=CallID,Number&$count=true&$skip={startNumber}&$top={topNumber}");
            client.Timeout = -1;

            var request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}")));

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {                
                apiResponse = "\n-------------" + DateTime.Now.ToString() + "------------ \n" + "------- Overal Tasks coount " + overalCounts + " \n Server respons:  " + response.StatusCode;
                WriteLogs(apiResponse);
                return response.Content;
            }
            else
            {
                apiResponse = "\n-------------" + DateTime.Now.ToString() + "------------ \n" + "------- Overal Tasks coount " + overalCounts + " \n Server respons:  " + response.StatusCode;
                throw new Exception("Server response error: " + response.Content);
            }

        }


        private async Task<string> UpdateTaskAsync(List<TaskFSE> tasks, int _taskCount) {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            JObject jsonToSend = JObject.Parse(JSONBuilder(tasks, _taskCount));

            var client = new RestClient($"{link}/so/api/Services/Integration/ServiceOptimization/ExecuteMultipleOperations");
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}")));
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);

            var cancellationTokenSource = new CancellationTokenSource();
            var restResponse = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            apiResponse = "\n-------------" + DateTime.Now.ToString() + "------------ \n" + "------- Server respons " + restResponse.StatusCode + " updated " + _taskCount;
            WriteLogs(apiResponse);

            return restResponse.Content;

        }        

        private void WriteLogs(string logs)
        {
            string path = @"C:\data\TaskList.txt";
            using (StreamWriter sw = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
            {
                sw.WriteLine(logs);
            }
         }


        private string JSONBuilder(List<TaskFSE> tasks, int _taskCount)
        {
            int count = 0;

            Root root = new Root()
            {
                Operations = new List<Operation>(),
                OneTransaction = true,
                ErrorOnNonExistingDictionaries = true,
                ContinueOnError = true,
            };

            for (int i = _taskCount; i < tasks.Count; i++)
            {
                if (count < 99 && _taskCount < tasks.Count)
                {
                    var operation = new Operation()
                    {
                        Action = "Update",
                        Object = new Object()
                        {
                            ObjectType = "Task",
                            CreateOrUpdate = true,
                            CallID = tasks[i].CallID,
                            Number = tasks[i].Number,
                            TaskColor = new TaskColor()
                            {
                                Name = _status,
                            },
                        },
                    };
                    root.Operations.Add(operation);
                    count++;
                }

            }
            var tasksToUpdate = JsonConvert.SerializeObject(root);
            return tasksToUpdate;

        }
    }

}



