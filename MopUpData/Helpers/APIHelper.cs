using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using MopUpData.Models;
using Task = MopUpData.Models.Task;
using Object = MopUpData.Models.Object;
using System.Configuration;

namespace MopUpData.Helpers
{

    public class APIHelper
    {
        public string apiResponse;
        private string _district;
        private string _status;
        private int _taskCount = 0;
        
        private List<Task> tasks;
        private int _tasksToUpdate = 0;
        const string sbLink = "https://fse-na-sb-int01";
        const string prodLink = "https://fse-na-int01";

        public async Task<string> CallFSE(string username, string password, string district, string status, bool isSandBox)
        {

            _district = district;
            _status = status;

            var client = new RestClient($"https://fse-na-sb-int01.cloud.clicksoftware.com/SO/api/objects/Task?$filter=District/Name eq '{district}' and Status/Name eq '{status}'"); ;
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {

                tasks = JsonConvert.DeserializeObject<List<Task>>(response.Content);
                _tasksToUpdate = tasks.Count;
                apiResponse = "\n-------------" + DateTime.Now.ToString() + "------------ \n" + "------- Recieved " + _tasksToUpdate + " Tasks for district " + _district + " in Status " + _status;
                WriteLogs(apiResponse);

                return response.Content;
            }
            else
            {
                apiResponse = response.StatusDescription.ToString();
                WriteLogs(apiResponse);
                throw new Exception(apiResponse);
            }
        }
        public async Task<string> UpdateTasks(string username, string password)
        {
            var client = new RestClient($"https://fse-na-sb-int01.cloud.clicksoftware.com/so/api/Services/Integration/ServiceOptimization/ExecuteMultipleOperations"); ;
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

            JObject jsonToSend = JObject.Parse(JSONBuilder(tasks));
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                apiResponse = "\n------- Updated " + _taskCount + " Tasks for district " + _district + " in Status " + _status;
                WriteLogs(apiResponse);

                if (_taskCount < tasks.Count)
                {
                    UpdateTasks(username, password);
                }
                WriteLogs(response.StatusDescription.ToString());
                return response.Content;
            }
            else
            {
                WriteLogs(response.ErrorMessage.ToString());
                throw new Exception(response.ResponseStatus.ToString());
            }
        }

        private void WriteLogs(string logs)
        {
            string path = @"C:\data\TaskList.txt";
            using (StreamWriter sw = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
            {
                sw.WriteLine(logs);
            }
         }


        private string JSONBuilder(List<Task> tasks)
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
             if(count < 99 && count < tasks.Count)
                {
                    var operation = new Operation()
                    {
                        Action = "CreateOrUpdate",
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
            _taskCount = _taskCount + count;
            var tasksToUpdate = JsonConvert.SerializeObject(root);
            return tasksToUpdate;

        }
    }
    
}



