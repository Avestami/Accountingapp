using System;
using System.Collections.Generic;

namespace Accounting.Domain.ValueObjects
{
    public class Money : IEquatable<Money>
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        
        public Money(decimal amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));
                
            Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
            Currency = currency.ToUpperInvariant();
        }
        
        public static Money Zero(string currency) => new Money(0, currency);
        
        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException($"Cannot add different currencies: {left.Currency} and {right.Currency}");
                
            return new Money(left.Amount + right.Amount, left.Currency);
        }
        
        public static Money operator -(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException($"Cannot subtract different currencies: {left.Currency} and {right.Currency}");
                
            return new Money(left.Amount - right.Amount, left.Currency);
        }
        
        public static Money operator *(Money money, decimal multiplier)
        {
            return new Money(money.Amount * multiplier, money.Currency);
        }
        
        public static Money operator /(Money money, decimal divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException("Cannot divide money by zero");
                
            return new Money(money.Amount / divisor, money.Currency);
        }
        
        public static bool operator ==(Money left, Money right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }
        
        public static bool operator !=(Money left, Money right) => !(left == right);
        
        public static bool operator >(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException($"Cannot compare different currencies: {left.Currency} and {right.Currency}");
                
            return left.Amount > right.Amount;
        }
        
        public static bool operator <(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException($"Cannot compare different currencies: {left.Currency} and {right.Currency}");
                
            return left.Amount < right.Amount;
        }
        
        public static bool operator >=(Money left, Money right) => left > right || left == right;
        public static bool operator <=(Money left, Money right) => left < right || left == right;
        
        public bool Equals(Money other)
        {
            if (other is null) return false;
            return Amount == other.Amount && Currency == other.Currency;
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as Money);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }
        
        public override string ToString()
        {
            return $"{Amount:N2} {Currency}";
        }
        
        public Money ConvertTo(string targetCurrency, decimal exchangeRate)
        {
            if (Currency == targetCurrency)
                return this;
                
            return new Money(Amount * exchangeRate, targetCurrency);
        }
        
        public bool IsZero() => Amount == 0;
        public bool IsPositive() => Amount > 0;
        public bool IsNegative() => Amount < 0;
        
        public Money Abs() => new Money(Math.Abs(Amount), Currency);
        public Money Negate() => new Money(-Amount, Currency);
    }
}