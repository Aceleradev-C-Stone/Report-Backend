using Newtonsoft.Json;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Report.IntegrationTests.Helpers
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

            DataFileNames.Add(typeof(User), usersFileName);
            DataFileNames.Add(typeof(UserResponse), usersFileName);
            DataFileNames.Add(typeof(CreateUserRequest), usersFileName);
            DataFileNames.Add(typeof(LoginUserRequest), usersFileName);
            DataFileNames.Add(typeof(RegisterUserRequest), usersFileName);

            DataFileNames.Add(typeof(Log), logsFileName);
            DataFileNames.Add(typeof(LogResponse), logsFileName);
            DataFileNames.Add(typeof(CreateLogRequest), logsFileName);
            DataFileNames.Add(typeof(UpdateLogRequest), logsFileName);
        }

        public List<T> Get<T>()
        {
            string content = File.ReadAllText(FileName<T>());
            return JsonConvert.DeserializeObject<List<T>>(content);
        }
    }
}
