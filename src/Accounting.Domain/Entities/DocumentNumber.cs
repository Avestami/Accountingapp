using System;

namespace Accounting.Domain.Entities
{
    public class DocumentNumber : BaseEntity
    {
        public string DocumentType { get; set; }
        public string Company { get; set; }
        public string Prefix { get; set; }
        public int CurrentNumber { get; set; }
        public int PadLength { get; set; }
        public string Suffix { get; set; }
        public DateTime? ResetDate { get; set; }
        public ResetPeriod ResetPeriod { get; set; }
        public bool IsActive { get; set; }
        
        public DocumentNumber()
        {
            CurrentNumber = 0;
            PadLength = 6;
            IsActive = true;
            ResetPeriod = ResetPeriod.Never;
        }
        
        public string GetNextNumber()
        {
            if (!IsActive)
                throw new InvalidOperationException("Document number sequence is not active");
                
            CheckForReset();
            
            CurrentNumber++;
            
            var numberPart = CurrentNumber.ToString().PadLeft(PadLength, '0');
            return $"{Prefix}{numberPart}{Suffix}";
        }
        
        private void CheckForReset()
        {
            if (ResetPeriod == ResetPeriod.Never || !ResetDate.HasValue)
                return;
                
            var now = DateTime.UtcNow;
            var shouldReset = false;
            
            switch (ResetPeriod)
            {
                case ResetPeriod.Daily:
                    shouldReset = now.Date > ResetDate.Value.Date;
                    break;
                case ResetPeriod.Monthly:
                    shouldReset = now.Year > ResetDate.Value.Year || 
                                 (now.Year == ResetDate.Value.Year && now.Month > ResetDate.Value.Month);
                    break;
                case ResetPeriod.Yearly:
                    shouldReset = now.Year > ResetDate.Value.Year;
                    break;
            }
            
            if (shouldReset)
            {
                CurrentNumber = 0;
                ResetDate = now;
            }
        }
        
        public void Reset()
        {
            CurrentNumber = 0;
            ResetDate = DateTime.UtcNow;
        }
    }
    
    public enum ResetPeriod
    {
        Never = 0,
        Daily = 1,
        Monthly = 2,
        Yearly = 3
    }
}