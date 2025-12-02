using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpaApp.Models
{
    public class Subscription : INotifyPropertyChanged
    {
        private bool _isInComparison;

        public int Id { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public string FacilityName { get; set; }
        public string Description { get; set; }
        public string Features { get; set; }
        public int DurationDays { get; set; }
        public int VisitsCount { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public bool IsInComparison
        {
            get => _isInComparison;
            set
            {
                if (_isInComparison != value)
                {
                    _isInComparison = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}