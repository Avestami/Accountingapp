using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System.Text.Json;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Accounting.Application.Features.Reports.Services
{
    public class ExportService : IExportService
    {
        public async Task<Result<byte[]>> ExportToPdfAsync<T>(T data, ExportOptionsDto options, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                options.Validate();
                
                using var stream = new MemoryStream();
                using var writer = new PdfWriter(stream);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf);

                if (options.UseRtlLayout)
                {
                    document.SetProperty(Property.BASE_DIRECTION, BaseDirection.RIGHT_TO_LEFT);
                }

                if (options.IncludeHeader)
                {
                    AddPdfHeader(document, options);
                }

                await AddPdfContentAsync(document, data, options, cancellationToken);

                if (options.IncludeFooter)
                {
                    AddPdfFooter(document, options);
                }

                document.Close();
                return Result.Success(stream.ToArray());
            }
            catch (Exception ex)
            {
                return Result.Failure<byte[]>($"PDF export failed: {ex.Message}");
            }
        }

        public async Task<Result<byte[]>> ExportToExcelAsync<T>(T data, ExportOptionsDto options, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                options.Validate();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add(options.WorksheetName);

                await AddExcelContentAsync(worksheet, data, options, cancellationToken);
                ApplyExcelFormatting(worksheet, options);

                return Result.Success(package.GetAsByteArray());
            }
            catch (Exception ex)
            {
                return Result.Failure<byte[]>($"Excel export failed: {ex.Message}");
            }
        }

        public async Task<Result<byte[]>> ExportToCsvAsync<T>(T data, ExportOptionsDto options, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                options.Validate();
                var csv = new StringBuilder();

                if (options.IncludeHeader)
                {
                    csv.AppendLine($"{options.Title}");
                    csv.AppendLine($"Generated: {DateTime.Now.ToString(options.DateFormat)}");
                    csv.AppendLine();
                }

                await AddCsvContentAsync(csv, data, options, cancellationToken);

                return Result.Success(Encoding.UTF8.GetBytes(csv.ToString()));
            }
            catch (Exception ex)
            {
                return Result.Failure<byte[]>($"CSV export failed: {ex.Message}");
            }
        }

        public async Task<Result<byte[]>> ExportToJsonAsync<T>(T data, ExportOptionsDto options, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                options.Validate();
                
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var wrapper = new
                {
                    Title = options.Title,
                    GeneratedAt = DateTime.Now,
                    Data = data
                };

                var json = JsonSerializer.Serialize(wrapper, jsonOptions);
                return Result.Success(Encoding.UTF8.GetBytes(json));
            }
            catch (Exception ex)
            {
                return Result.Failure<byte[]>($"JSON export failed: {ex.Message}");
            }
        }

        public async Task<Result<byte[]>> ExportSalesReportAsync(SalesReportDto report, ExportOptionsDto options, CancellationToken cancellationToken = default)
        {
            return options.Format.ToLower() switch
            {
                "pdf" => await ExportToPdfAsync(report, options, cancellationToken),
                "excel" => await ExportToExcelAsync(report, options, cancellationToken),
                "csv" => await ExportToCsvAsync(report, options, cancellationToken),
                "json" => await ExportToJsonAsync(report, options, cancellationToken),
                _ => Result.Failure<byte[]>($"Unsupported format: {options.Format}")
            };
        }

        public async Task<Result<byte[]>> ExportFinancialReportAsync(FinancialReportDto report, ExportOptionsDto options, CancellationToken cancellationToken = default)
        {
            return options.Format.ToLower() switch
            {
                "pdf" => await ExportToPdfAsync(report, options, cancellationToken),
                "excel" => await ExportToExcelAsync(report, options, cancellationToken),
                "csv" => await ExportToCsvAsync(report, options, cancellationToken),
                "json" => await ExportToJsonAsync(report, options, cancellationToken),
                _ => Result.Failure<byte[]>($"Unsupported format: {options.Format}")
            };
        }

        public async Task<Result<byte[]>> ExportProfitLossReportAsync(ProfitLossReportDto report, ExportOptionsDto options, CancellationToken cancellationToken = default)
        {
            return options.Format.ToLower() switch
            {
                "pdf" => await ExportToPdfAsync(report, options, cancellationToken),
                "excel" => await ExportToExcelAsync(report, options, cancellationToken),
                "csv" => await ExportToCsvAsync(report, options, cancellationToken),
                "json" => await ExportToJsonAsync(report, options, cancellationToken),
                _ => Result.Failure<byte[]>($"Unsupported format: {options.Format}")
            };
        }

        public async Task<Result<byte[]>> ExportBalanceSheetReportAsync(BalanceSheetReportDto report, ExportOptionsDto options, CancellationToken cancellationToken = default)
        {
            return options.Format.ToLower() switch
            {
                "pdf" => await ExportToPdfAsync(report, options, cancellationToken),
                "excel" => await ExportToExcelAsync(report, options, cancellationToken),
                "csv" => await ExportToCsvAsync(report, options, cancellationToken),
                "json" => await ExportToJsonAsync(report, options, cancellationToken),
                _ => Result.Failure<byte[]>($"Unsupported format: {options.Format}")
            };
        }

        public async Task<Result<byte[]>> ExportMultipleReportsAsync(List<object> reports, ExportOptionsDto options, CancellationToken cancellationToken = default)
        {
            try
            {
                if (options.Format.ToLower() == "excel")
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using var package = new ExcelPackage();

                    for (int i = 0; i < reports.Count; i++)
                    {
                        var worksheetName = $"Report_{i + 1}";
                        var worksheet = package.Workbook.Worksheets.Add(worksheetName);
                        await AddExcelContentAsync(worksheet, reports[i], options, cancellationToken);
                        ApplyExcelFormatting(worksheet, options);
                    }

                    return Result.Success(package.GetAsByteArray());
                }
                else
                {
                    return await ExportToJsonAsync(reports, options, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<byte[]>($"Multiple reports export failed: {ex.Message}");
            }
        }

        public async Task<Result<byte[]>> ExportWithTemplateAsync<T>(T data, string templatePath, ExportOptionsDto options, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                if (!File.Exists(templatePath))
                    return Result.Failure<byte[]>("Template file not found");

                return await ExportToExcelAsync(data, options, cancellationToken);
            }
            catch (Exception ex)
            {
                return Result.Failure<byte[]>($"Template export failed: {ex.Message}");
            }
        }

        private void AddPdfHeader(Document document, ExportOptionsDto options)
        {
            var header = new Paragraph(options.Title)
                .SetFontSize(18)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER);

            document.Add(header);

            if (!string.IsNullOrEmpty(options.Subtitle))
            {
                var subtitle = new Paragraph(options.Subtitle)
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER);
                document.Add(subtitle);
            }

            var dateInfo = new Paragraph($"Generated: {DateTime.Now.ToString(options.DateFormat)}")
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.RIGHT);
            document.Add(dateInfo);

            document.Add(new Paragraph("\n"));
        }

        private void AddPdfFooter(Document document, ExportOptionsDto options)
        {
            if (!string.IsNullOrEmpty(options.FooterText))
            {
                var footer = new Paragraph(options.FooterText)
                    .SetFontSize(8)
                    .SetTextAlignment(TextAlignment.CENTER);
                document.Add(footer);
            }
        }

        private async Task AddPdfContentAsync<T>(Document document, T data, ExportOptionsDto options, CancellationToken cancellationToken) where T : class
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            var content = new Paragraph(json)
                .SetFontSize(10);
            document.Add(content);
        }

        private async Task AddExcelContentAsync<T>(ExcelWorksheet worksheet, T data, ExportOptionsDto options, CancellationToken cancellationToken) where T : class
        {
            worksheet.Cells[1, 1].Value = options.Title;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;

            worksheet.Cells[2, 1].Value = $"Generated: {DateTime.Now.ToString(options.DateFormat)}";
            worksheet.Cells[2, 1].Style.Font.Size = 10;

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            worksheet.Cells[4, 1].Value = json;
        }

        private async Task AddCsvContentAsync<T>(StringBuilder csv, T data, ExportOptionsDto options, CancellationToken cancellationToken) where T : class
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            csv.AppendLine(json);
        }

        private void ApplyExcelFormatting(ExcelWorksheet worksheet, ExportOptionsDto options)
        {
            if (options.AutoFitColumns)
            {
                worksheet.Cells.AutoFitColumns();
            }

            if (options.FreezePanes)
            {
                worksheet.View.FreezePanes(4, 1);
            }

            if (options.UseRtlLayout)
            {
                worksheet.View.RightToLeft = true;
            }
        }
    }
}