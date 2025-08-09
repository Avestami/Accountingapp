using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Services
{
    public interface IDocumentNumberService
    {
        Task<string> GetNextNumberAsync(string documentType, string company, CancellationToken cancellationToken = default);
    }

    public class DocumentNumberService : IDocumentNumberService
    {
        private readonly IAccountingDbContext _context;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public DocumentNumberService(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetNextNumberAsync(string documentType, string company, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            
            try
            {
                const int maxRetries = 3;
                for (int attempt = 0; attempt < maxRetries; attempt++)
                {
                    try
                    {
                        var docNumber = await _context.DocumentNumbers
                            .FirstOrDefaultAsync(d => d.DocumentType == documentType && d.Company == company, cancellationToken);

                        if (docNumber == null)
                        {
                            docNumber = new DocumentNumber
                            {
                                DocumentType = documentType,
                                Company = company,
                                Prefix = GetDefaultPrefix(documentType),
                                CurrentNumber = 1,
                                PadLength = 6,
                                Suffix = "",
                                ResetPeriod = Domain.Entities.ResetPeriod.Yearly,
                                CreatedAt = DateTime.UtcNow
                            };
                            _context.DocumentNumbers.Add(docNumber);
                        }
                        else
                        {
                            // Check if reset is needed
                            if (ShouldReset(docNumber))
                            {
                                docNumber.CurrentNumber = 1;
                                docNumber.ResetDate = DateTime.UtcNow;
                            }
                            else
                            {
                                docNumber.CurrentNumber++;
                            }
                            docNumber.UpdatedAt = DateTime.UtcNow;
                        }

                        await _context.SaveChangesAsync(cancellationToken);

                        return FormatDocumentNumber(docNumber);
                    }
                    catch (DbUpdateConcurrencyException) when (attempt < maxRetries - 1)
                    {
                        // Retry on concurrency conflict
                        await Task.Delay(100 * (attempt + 1), cancellationToken);
                        continue;
                    }
                }

                throw new InvalidOperationException("Failed to generate document number after multiple attempts");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private string GetDefaultPrefix(string documentType)
        {
            return documentType.ToUpper() switch
            {
                "TICKET" => "TKT",
                "VOUCHER" => "VCH",
                "COST" => "CST",
                "INCOME" => "INC",
                "TRANSFER" => "TRF",
                _ => "DOC"
            };
        }

        private bool ShouldReset(DocumentNumber docNumber)
        {
            if (docNumber.ResetDate == null) return false;

            var now = DateTime.UtcNow;
            return docNumber.ResetPeriod switch
            {
                Domain.Entities.ResetPeriod.Daily => docNumber.ResetDate.Value.Date < now.Date,
                Domain.Entities.ResetPeriod.Monthly => docNumber.ResetDate.Value.Month != now.Month || docNumber.ResetDate.Value.Year != now.Year,
                Domain.Entities.ResetPeriod.Yearly => docNumber.ResetDate.Value.Year != now.Year,
                _ => false
            };
        }

        private string FormatDocumentNumber(DocumentNumber docNumber)
        {
            var paddedNumber = docNumber.CurrentNumber.ToString().PadLeft(docNumber.PadLength, '0');
            return $"{docNumber.Prefix}{paddedNumber}{docNumber.Suffix}";
        }
    }
}