using System;

namespace Accounting.Domain.ValueObjects
{
    public class DateRange : IEquatable<DateRange>
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        
        public DateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be greater than end date");
                
            StartDate = startDate.Date;
            EndDate = endDate.Date;
        }
        
        public static DateRange Create(DateTime startDate, DateTime endDate)
        {
            return new DateRange(startDate, endDate);
        }
        
        public static DateRange Today()
        {
            var today = DateTime.Today;
            return new DateRange(today, today);
        }
        
        public static DateRange ThisWeek()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);
            return new DateRange(startOfWeek, endOfWeek);
        }
        
        public static DateRange ThisMonth()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            return new DateRange(startOfMonth, endOfMonth);
        }
        
        public static DateRange ThisYear()
        {
            var today = DateTime.Today;
            var startOfYear = new DateTime(today.Year, 1, 1);
            var endOfYear = new DateTime(today.Year, 12, 31);
            return new DateRange(startOfYear, endOfYear);
        }
        
        public static DateRange LastMonth()
        {
            var today = DateTime.Today;
            var firstDayOfLastMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
            var lastDayOfLastMonth = firstDayOfLastMonth.AddMonths(1).AddDays(-1);
            return new DateRange(firstDayOfLastMonth, lastDayOfLastMonth);
        }
        
        public static DateRange LastYear()
        {
            var today = DateTime.Today;
            var startOfLastYear = new DateTime(today.Year - 1, 1, 1);
            var endOfLastYear = new DateTime(today.Year - 1, 12, 31);
            return new DateRange(startOfLastYear, endOfLastYear);
        }
        
        public TimeSpan Duration => EndDate - StartDate;
        
        public int DaysCount => (int)Duration.TotalDays + 1;
        
        public bool Contains(DateTime date)
        {
            var dateOnly = date.Date;
            return dateOnly >= StartDate && dateOnly <= EndDate;
        }
        
        public bool Overlaps(DateRange other)
        {
            if (other == null) return false;
            return StartDate <= other.EndDate && EndDate >= other.StartDate;
        }
        
        public DateRange Intersect(DateRange other)
        {
            if (other == null || !Overlaps(other))
                return null;
                
            var intersectionStart = StartDate > other.StartDate ? StartDate : other.StartDate;
            var intersectionEnd = EndDate < other.EndDate ? EndDate : other.EndDate;
            
            return new DateRange(intersectionStart, intersectionEnd);
        }
        
        public DateRange Extend(int days)
        {
            return new DateRange(StartDate.AddDays(-days), EndDate.AddDays(days));
        }
        
        public DateRange ExtendToEndOfMonth()
        {
            var endOfMonth = new DateTime(EndDate.Year, EndDate.Month, DateTime.DaysInMonth(EndDate.Year, EndDate.Month));
            return new DateRange(StartDate, endOfMonth);
        }
        
        public DateRange ExtendToStartOfMonth()
        {
            var startOfMonth = new DateTime(StartDate.Year, StartDate.Month, 1);
            return new DateRange(startOfMonth, EndDate);
        }
        
        public bool IsValid()
        {
            return StartDate <= EndDate;
        }
        
        public bool Equals(DateRange other)
        {
            if (other is null) return false;
            return StartDate == other.StartDate && EndDate == other.EndDate;
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as DateRange);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(StartDate, EndDate);
        }
        
        public override string ToString()
        {
            return $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
        }
        
        public static bool operator ==(DateRange left, DateRange right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }
        
        public static bool operator !=(DateRange left, DateRange right) => !(left == right);
    }
}