using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private QrsList qrs;

        private DB() {
            this.dbPath = GetPath();
            var data = LoadData();
            qrs = new QrsList(data);
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

        public ObservableCollection<Qr> GetQrs()
        {
            return qrs;    
        }
        public void ShowCodes(bool isShowCodes) {
            qrs.ShowCodes(isShowCodes);
        }

        public void AddQr(Qr qr) 
        {
            lock (_lock)
            {
                if (qrs.AddQr(qr))
                {
                    Save();
                }
             }
        }
        public void RemoveQr(Qr qr) 
        {
            lock (_lock)
            {
                if (qrs.RemoveQr(qr))
                {
                    Save();
                }
            }
        }

        private void Save() {
                File.WriteAllLines(dbPath, qrs.Select(x=>x.Url));
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
