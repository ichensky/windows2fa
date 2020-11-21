using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Windows2FA
{
    public class DB
    {
        private static readonly object _lock = new object();
        private readonly string dbPath;
        private static DB db;

        private List<string> data;

        private DB() {
            this.dbPath = GetPath();
            this.data = LoadData().ToList();
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

        public void OpenDbPath() {
            var path = Path.GetDirectoryName(dbPath);
            Process.Start("explorer.exe",path);
        }

        public IEnumerable<Qr> GetQrs(bool isShowCodes)
        {
            lock (_lock)
            {
                return data.Select(x => new Qr(x, isShowCodes))
                           .OrderBy(x => x.Issuer).ToList();
            }
        }

        public void AddQr(Qr qr) 
        {
            lock (_lock)
            {
                if (!data.Contains(qr.Url))
                {
                    data.Add(qr.Url);
                    Save();
                }
            }
        }
        public void RemoveQr(Qr qr) 
        {
            lock (_lock)
            {
                if (data.Contains(qr.Url))
                {
                    data.Remove(qr.Url);
                    Save();
                }
            }
        }

        private void Save() {
                File.WriteAllLines(dbPath, data);
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
