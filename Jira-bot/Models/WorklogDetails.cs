using System;

namespace Models
{
    public class WorklogDetails
    {
        public DateTime WorklogDate { get; set; }
        public TimeSpan WorklogTime {  get; set; }
        public string TicketId { get; set; }    
        public double TimeInSeconds { get; set; }

    }
}
