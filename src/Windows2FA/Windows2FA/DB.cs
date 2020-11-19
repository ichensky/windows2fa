using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Windows2FA
{
    public class DB
    {
        private static readonly object _lock = new object();
        private readonly string dbPath;
        private static DB db;

        public ConcurrentBag<string> Data { get; }

        private DB() {
            this.dbPath = GetPath();
            this.Data = new ConcurrentBag<string>(LoadData());
        }

        public static DB Instance
        {
            get
            {
                if (db == null)
                {
                    lock (_lock)
                    {
                        if (db == null)
                        {
                            db = new DB();
                        }
                    }
                }
                return db;
            }
        }

        public void Save() {
            lock (_lock)
            {
                File.WriteAllLines(dbPath, Data);
            }
        }

        private IEnumerable<string> LoadData() =>
            File.ReadAllLines(dbPath)
            .Where(x=>!string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim());

        private string GetPath() {
            var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dir = Path.Combine(systemPath, "Windows2FA");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var dbPath = Path.Combine(dir, "db.txt");
            if (!File.Exists(dbPath))
            {
                using var file = File.Create(dbPath);
            }
            return dbPath;
        }
    }
}
