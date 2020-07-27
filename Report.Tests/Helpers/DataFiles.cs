using Newtonsoft.Json;
using Report.Core.Dto.Requests;
using Report.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Report.Tests.Helpers
{
    class DataFiles
    {
        private Dictionary<Type, string> DataFileNames { get; } =
            new Dictionary<Type, string>();
        private string FileName<T>() { return DataFileNames[typeof(T)]; }

        private char Slash = Path.DirectorySeparatorChar;

        public DataFiles()
        {
            var usersFileName = $"TestData{Slash}Users.json";
            var logsFileName = $"TestData{Slash}Logs.json";

            // Services
            DataFileNames.Add(typeof(User), usersFileName);
            DataFileNames.Add(typeof(CreateUserRequest), usersFileName);
            DataFileNames.Add(typeof(UpdateUserRequest), usersFileName);
            DataFileNames.Add(typeof(LoginUserRequest), usersFileName);
            DataFileNames.Add(typeof(RegisterUserRequest), usersFileName);

            DataFileNames.Add(typeof(Log), logsFileName);
            DataFileNames.Add(typeof(CreateLogRequest), logsFileName);
            DataFileNames.Add(typeof(UpdateLogRequest), logsFileName);

            // Controllers
            DataFileNames.Add(typeof(Api.Dto.Requests.CreateUserRequest), usersFileName);
            DataFileNames.Add(typeof(Api.Dto.Requests.UpdateUserRequest), usersFileName);
            DataFileNames.Add(typeof(Api.Dto.Requests.LoginUserRequest), usersFileName);
            DataFileNames.Add(typeof(Api.Dto.Requests.RegisterUserRequest), usersFileName);

            DataFileNames.Add(typeof(Api.Dto.Requests.CreateLogRequest), logsFileName);
            DataFileNames.Add(typeof(Api.Dto.Requests.UpdateLogRequest), logsFileName);
        }

        public List<T> Get<T>()
        {
            string content = File.ReadAllText(FileName<T>());
            return JsonConvert.DeserializeObject<List<T>>(content);
        }
    }
}
