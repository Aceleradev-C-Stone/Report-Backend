using Report.Core.Enums;
using Report.Core.Models;
using Report.Infra.Contexts;
using System;
using System.Collections.Generic;

namespace Report.IntegrationTests.Helpers
{
    public class Fakes
    {
        private static readonly DataFiles _data = new DataFiles();

        public static void InitializeDbForTesting(DataContext db)
        {
            db.Users.AddRange(Get<User>());
            db.Logs.AddRange(Get<Log>());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTesting(DataContext db)
        {
            db.RemoveRange(db.Logs);
            db.RemoveRange(db.Users);
            InitializeDbForTesting(db);
        }

        public static List<T> Get<T>()
        {
            return _data.Get<T>();
        }
    }
}
