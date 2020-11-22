using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace Windows2FA
{
    public class QrsList : ObservableCollection<Qr>
    {
        public QrsList(IEnumerable<string> data) : 
            base(data.Select(x => new Qr(x))
                           .OrderBy(x => x.Issuer)) {}

        public bool AddQr(Qr qr)
        {
            if (!this.Contains(qr))
            {
                int i = 0;
                for (; i < this.Count; i++)
                {
                    if (this[i].CompareTo(qr) > 0)
                    {
                        break;
                    }
                }
                this.Insert(i, qr);
                return true;
            }
            return false;
        }
        public bool RemoveQr(Qr qr)
        {
            if (this.Contains(qr))
            {
                this.Remove(qr);
                return true;
            }
            return false;
        }

        public void ShowCodes(bool isShowCodes)
        {
            foreach (var item in this)
            {
                item.ShowCodes(isShowCodes);
            }
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        public void UpdateReminingSeconds() {
            foreach (var item in this)
            {
                item.UpdateReminingSeconds();
            }
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
    }
}
