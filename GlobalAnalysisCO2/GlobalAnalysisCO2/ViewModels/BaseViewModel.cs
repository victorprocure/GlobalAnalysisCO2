using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalAnalysisCO2.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private string viewTitle;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string control)
        {
            var handler = this.PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(control));
        }

        public string ViewTitle
        {
            get
            {
                return string.Format("Global Analysis CO2 - {0}", this.viewTitle);
            }
        }

        protected void ChangeTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
            {
                throw new ArgumentNullException(nameof(newTitle));
            }

            this.viewTitle = newTitle;
        }

        public virtual void SuspendView()
        {
        }

        public virtual void StartView()
        {
        }
    }
}