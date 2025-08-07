using System;

namespace Accounting.Domain.Entities
{
    public class FxConsumption : BaseEntity
    {
        public decimal Amount { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal GainLoss { get; set; }
        public DateTime ConsumptionDate { get; set; }
        
        // Foreign Keys
        public int BuyTransactionId { get; set; }
        public int SellTransactionId { get; set; }
        
        // Navigation Properties
        public virtual FxTransaction BuyTransaction { get; set; }
        public virtual FxTransaction SellTransaction { get; set; }
        
        public FxConsumption()
        {
            ConsumptionDate = DateTime.UtcNow;
        }
        
        public void CalculateGainLoss()
        {
            // Calculate FX gain/loss based on the difference between buy and sell rates
            var buyRate = BuyTransaction.ExchangeRate;
            var sellRate = SellTransaction.ExchangeRate;
            
            // Gain/Loss = (Sell Rate - Buy Rate) * Amount
            GainLoss = (sellRate - buyRate) * Amount;
        }
    }
}