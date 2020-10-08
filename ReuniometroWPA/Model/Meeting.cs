using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Xml;

namespace ReuniometroWPA.Model
{
    public enum MeetingStatus
    {
        Pending,
        Running,
        Paused,
        Finished
    }
    public class Meeting
    {
        private List<Attendee>_Attendees = new List<Attendee>();
        public ReadOnlyCollection<Attendee> Attendees { get; private set; }
        public decimal Cost { get; private set; } = 0;
        //public string CostFormated => Cost.ToString("C");
        public decimal TotalCostPerHour => Attendees.Sum(x => x.TotalCost);
        public decimal NumberAttendees => Attendees.Sum(x => x.Number);

        public MeetingStatus Status = MeetingStatus.Pending; 
        
        private TimeSpan _MeetingStep= TimeSpan.FromMilliseconds(250);
        private TimeSpan _Duration = new TimeSpan();
        private System.Timers.Timer _Timer;

        public Meeting()
        {
            Attendees = new ReadOnlyCollection<Attendee>(_Attendees);
        }

        public event Action OnCostChanged;

        public void AddAtendee()
        {
            if (_Attendees.Count == 0)
                _Attendees.Add(new Attendee());
            else
            {
                var attendee = _Attendees.Last();
                if (!String.IsNullOrWhiteSpace(attendee.Name) || attendee.Number != 0 || attendee.CostPerHour != 0)
                    _Attendees.Add(new Attendee());
            }
        }

        public void RemoveAtendee(Attendee attendee)
        {
            _Attendees.Remove(attendee);
        }

        public void StartMeeting ()
        {
            Cost = 0;
            _Timer = new System.Timers.Timer(_MeetingStep.TotalMilliseconds);
            _Timer.Elapsed += (o, e) =>
            {
                _Duration = _Duration.Add(_MeetingStep);
                Cost += (decimal)_Timer.Interval * GetCostPerMillisecond();
                OnCostChanged?.Invoke();
            };
            _Timer.Start();
            Status = MeetingStatus.Running;
            
        }

        private decimal GetCostPerMillisecond()
        {
            return Attendees.Sum(a => a.TotalCost / (60 * 60 * 1000));
        }

        public void Pause()
        {
            if (Status == MeetingStatus.Running)
            {
                _Timer.Stop();
                Status = MeetingStatus.Paused;
            }
            else if (Status == MeetingStatus.Paused)
            {
                _Timer.Start();
                Status = MeetingStatus.Running;
            }            
        }

        public void Stop()
        {
            _Timer.Stop();
            Status = MeetingStatus.Finished;
        }



    }
}
