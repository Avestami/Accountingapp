import jsPDF from 'jspdf';
import * as XLSX from 'xlsx';

// Export Service for PDF and Excel functionality
export class ExportService {
  
  // Export data to PDF
  static exportToPDF(data, filename = 'export', title = 'گزارش') {
    try {
      const doc = new jsPDF();
      
      // Add Persian font support (placeholder - would need actual font file)
      doc.setFont('helvetica');
      doc.setFontSize(16);
      
      // Add title
      doc.text(title, 20, 20);
      
      // Add date
      const currentDate = new Date().toLocaleDateString('fa-IR');
      doc.setFontSize(12);
      doc.text(`تاریخ: ${currentDate}`, 20, 35);
      
      // Add data (simple text format)
      let yPosition = 50;
      
      if (Array.isArray(data)) {
        data.forEach((item, index) => {
          if (yPosition > 280) {
            doc.addPage();
            yPosition = 20;
          }
          
          const text = typeof item === 'object' ? JSON.stringify(item, null, 2) : String(item);
          const lines = doc.splitTextToSize(text, 170);
          
          lines.forEach(line => {
            doc.text(line, 20, yPosition);
            yPosition += 7;
          });
          
          yPosition += 5; // Extra space between items
        });
      } else {
        const text = typeof data === 'object' ? JSON.stringify(data, null, 2) : String(data);
        const lines = doc.splitTextToSize(text, 170);
        
        lines.forEach(line => {
          if (yPosition > 280) {
            doc.addPage();
            yPosition = 20;
          }
          doc.text(line, 20, yPosition);
          yPosition += 7;
        });
      }
      
      // Save the PDF
      doc.save(`${filename}.pdf`);
      
      return { success: true, message: 'فایل PDF با موفقیت ایجاد شد' };
    } catch (error) {
      console.error('PDF Export Error:', error);
      return { success: false, message: 'خطا در ایجاد فایل PDF' };
    }
  }
  
  // Export data to Excel
  static exportToExcel(data, filename = 'export', sheetName = 'Sheet1') {
    try {
      let worksheetData;
      
      if (Array.isArray(data) && data.length > 0) {
        // Convert array of objects to worksheet format
        if (typeof data[0] === 'object') {
          worksheetData = data;
        } else {
          // Convert simple array to object array
          worksheetData = data.map((item, index) => ({ 'ردیف': index + 1, 'مقدار': item }));
        }
      } else if (typeof data === 'object') {
        // Convert single object to array
        worksheetData = [data];
      } else {
        // Convert primitive value to object array
        worksheetData = [{ 'مقدار': data }];
      }
      
      // Create worksheet
      const worksheet = XLSX.utils.json_to_sheet(worksheetData);
      
      // Create workbook
      const workbook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(workbook, worksheet, sheetName);
      
      // Save the Excel file
      XLSX.writeFile(workbook, `${filename}.xlsx`);
      
      return { success: true, message: 'فایل Excel با موفقیت ایجاد شد' };
    } catch (error) {
      console.error('Excel Export Error:', error);
      return { success: false, message: 'خطا در ایجاد فایل Excel' };
    }
  }
  
  // Export sales documents to PDF
  static exportSalesDocumentsToPDF(documents, filename = 'sales-documents') {
    const formattedData = documents.map(doc => ({
      'شماره سند': doc.documentNo,
      'وضعیت': this.getStatusText(doc.status),
      'نوع سرویس': this.getServiceTypeText(doc.serviceType),
      'تاریخ فروش': doc.saleDate,
      'تاریخ پرواز': doc.flightDate,
      'خریدار': doc.buyer.name,
      'مسافر': doc.passenger.name,
      'مبلغ فروش': this.formatCurrency(doc.finance.salePrice),
      'مبلغ خرید': this.formatCurrency(doc.finance.costPrice),
      'سود': this.formatCurrency(doc.finance.profit)
    }));
    
    return this.exportToPDF(formattedData, filename, 'گزارش اسناد فروش');
  }
  
  // Export sales documents to Excel
  static exportSalesDocumentsToExcel(documents, filename = 'sales-documents') {
    const formattedData = documents.map(doc => ({
      'شماره سند': doc.documentNo,
      'وضعیت': this.getStatusText(doc.status),
      'نوع سرویس': this.getServiceTypeText(doc.serviceType),
      'تاریخ فروش': doc.saleDate,
      'تاریخ پرواز': doc.flightDate,
      'خریدار': doc.buyer.name,
      'مسافر': doc.passenger.name,
      'شماره پاسپورت': doc.passenger.passportNo,
      'مبدا': doc.route.origin.name,
      'مقصد': doc.route.destination.name,
      'ایرلاین': doc.route.airline.name,
      'شماره پرواز': doc.route.flightNo,
      'مبلغ فروش': doc.finance.salePrice,
      'مبلغ خرید': doc.finance.costPrice,
      'سود': doc.finance.profit,
      'ارز': doc.finance.currency,
      'ایجاد کننده': doc.createdBy.name,
      'تاریخ ایجاد': doc.createdAt
    }));
    
    return this.exportToExcel(formattedData, filename, 'اسناد فروش');
  }
  
  // Export vouchers to PDF
  static exportVouchersToPDF(vouchers, filename = 'vouchers') {
    const formattedData = vouchers.map(voucher => ({
      'شماره سند': voucher.voucherNo,
      'تاریخ': voucher.date,
      'نوع': this.getVoucherTypeText(voucher.type),
      'شرح': voucher.description,
      'مبلغ': this.formatCurrency(voucher.amount),
      'طرف حساب': voucher.counterparty.name,
      'بانک': voucher.bank.name
    }));
    
    return this.exportToPDF(formattedData, filename, 'گزارش اسناد حسابداری');
  }
  
  // Export vouchers to Excel
  static exportVouchersToExcel(vouchers, filename = 'vouchers') {
    const formattedData = vouchers.map(voucher => ({
      'شماره سند': voucher.voucherNo,
      'تاریخ': voucher.date,
      'نوع': this.getVoucherTypeText(voucher.type),
      'شرح': voucher.description,
      'مبلغ': voucher.amount,
      'ارز': voucher.currency,
      'طرف حساب': voucher.counterparty.name,
      'بانک': voucher.bank.name,
      'شماره حساب': voucher.bank.accountNumber,
      'ایجاد کننده': voucher.createdBy.name,
      'تاریخ ایجاد': voucher.createdAt
    }));
    
    return this.exportToExcel(formattedData, filename, 'اسناد حسابداری');
  }
  
  // Helper methods
  static getStatusText(status) {
    const statusMap = {
      'unissued': 'صادر نشده',
      'issued': 'صادر شده',
      'canceled': 'لغو شده'
    };
    return statusMap[status] || status;
  }
  
  static getServiceTypeText(serviceType) {
    const serviceMap = {
      'air': 'هواپیما',
      'train': 'قطار',
      'bus': 'اتوبوس',
      'hotel': 'هتل',
      'tour': 'تور',
      'mixed': 'ترکیبی'
    };
    return serviceMap[serviceType] || serviceType;
  }
  
  static getVoucherTypeText(type) {
    const typeMap = {
      'receipt': 'دریافت',
      'payment': 'پرداخت',
      'transfer': 'انتقال'
    };
    return typeMap[type] || type;
  }
  
  static formatCurrency(amount, currency = 'IRR') {
    if (currency === 'IRR') {
      return new Intl.NumberFormat('fa-IR').format(amount) + ' ریال';
    } else {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: currency
      }).format(amount);
    }
  }
}

// Default export
export default ExportService;