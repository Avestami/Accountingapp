using System;
using System.Collections.Generic;
using System.Linq;

namespace Accounting.Application.Common.Models
{
    public class Result
    {
        protected Result(bool isSuccess, IEnumerable<string> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors?.ToList() ?? new List<string>();
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<string> Errors { get; }
        public string Error => Errors.FirstOrDefault();

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, new[] { error });
        public static Result Failure(IEnumerable<string> errors) => new(false, errors);

        public static Result<T> Success<T>(T value) => new(value, true, null);
        public static Result<T> Failure<T>(string error) => new(default, false, new[] { error });
        public static Result<T> Failure<T>(IEnumerable<string> errors) => new(default, false, errors);

        public static implicit operator Result(string error) => Failure(error);
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        protected internal Result(T value, bool isSuccess, IEnumerable<string> errors)
            : base(isSuccess, errors)
        {
            _value = value;
        }

        public T Value
        {
            get
            {
                if (IsFailure)
                    throw new InvalidOperationException("Cannot access value of a failed result.");
                return _value;
            }
        }

        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(string error) => Failure<T>(error);
    }
}