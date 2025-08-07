using System;

namespace Accounting.Domain.ValueObjects
{
    public class ExchangeRate : IEquatable<ExchangeRate>
    {
        public string FromCurrency { get; private set; }
        public string ToCurrency { get; private set; }
        public decimal Rate { get; private set; }
        public DateTime EffectiveDate { get; private set; }
        
        public ExchangeRate(string fromCurrency, string toCurrency, decimal rate, DateTime effectiveDate)
        {
            if (string.IsNullOrWhiteSpace(fromCurrency))
                throw new ArgumentException("From currency cannot be null or empty", nameof(fromCurrency));
                
            if (string.IsNullOrWhiteSpace(toCurrency))
                throw new ArgumentException("To currency cannot be null or empty", nameof(toCurrency));
                
            if (rate <= 0)
                throw new ArgumentException("Exchange rate must be positive", nameof(rate));
                
            FromCurrency = fromCurrency.ToUpperInvariant();
            ToCurrency = toCurrency.ToUpperInvariant();
            Rate = Math.Round(rate, 6, MidpointRounding.AwayFromZero);
            EffectiveDate = effectiveDate;
        }
        
        public static ExchangeRate Create(string fromCurrency, string toCurrency, decimal rate)
        {
            return new ExchangeRate(fromCurrency, toCurrency, rate, DateTime.UtcNow);
        }
        
        public Money Convert(Money money)
        {
            if (money.Currency != FromCurrency)
                throw new InvalidOperationException($"Cannot convert {money.Currency} using exchange rate from {FromCurrency} to {ToCurrency}");
                
            return money.ConvertTo(ToCurrency, Rate);
        }
        
        public ExchangeRate Invert()
        {
            if (Rate == 0)
                throw new InvalidOperationException("Cannot invert exchange rate with zero rate");
                
            return new ExchangeRate(ToCurrency, FromCurrency, 1 / Rate, EffectiveDate);
        }
        
        public bool IsExpired(TimeSpan validityPeriod)
        {
            return DateTime.UtcNow > EffectiveDate.Add(validityPeriod);
        }
        
        public bool IsSameCurrencyPair(string fromCurrency, string toCurrency)
        {
            return FromCurrency.Equals(fromCurrency, StringComparison.OrdinalIgnoreCase) &&
                   ToCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase);
        }
        
        public bool IsIdentity()
        {
            return FromCurrency == ToCurrency || Rate == 1.0m;
        }
        
        public string GetCurrencyPair()
        {
            return $"{FromCurrency}/{ToCurrency}";
        }
        
        public bool Equals(ExchangeRate other)
        {
            if (other is null) return false;
            return FromCurrency == other.FromCurrency &&
                   ToCurrency == other.ToCurrency &&
                   Rate == other.Rate &&
                   EffectiveDate == other.EffectiveDate;
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as ExchangeRate);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(FromCurrency, ToCurrency, Rate, EffectiveDate);
        }
        
        public override string ToString()
        {
            return $"{FromCurrency}/{ToCurrency} = {Rate:F6} (as of {EffectiveDate:yyyy-MM-dd})";
        }
        
        public static bool operator ==(ExchangeRate left, ExchangeRate right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }
        
        public static bool operator !=(ExchangeRate left, ExchangeRate right) => !(left == right);
    }
}