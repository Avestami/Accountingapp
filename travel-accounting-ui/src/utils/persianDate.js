import moment from 'moment-jalaali';

// Configure moment-jalaali
moment.loadPersian({ dialect: 'persian-modern' });

/**
 * Persian Date Utility Class
 * Provides comprehensive Persian calendar functionality
 */
export class PersianDate {
  constructor(date = null) {
    this.moment = date ? moment(date) : moment();
  }

  // Static factory methods
  static now() {
    return new PersianDate();
  }

  static from(date) {
    return new PersianDate(date);
  }

  static parse(dateString, format = 'jYYYY/jMM/jDD') {
    return new PersianDate(moment(dateString, format));
  }

  // Formatting methods
  format(format = 'jYYYY/jMM/jDD') {
    return this.moment.format(format);
  }

  formatLong() {
    return this.moment.format('jYYYY/jMM/jDD - HH:mm');
  }

  formatShort() {
    return this.moment.format('jYY/jMM/jDD');
  }

  formatTime() {
    return this.moment.format('HH:mm');
  }

  formatDateTime() {
    return this.moment.format('jYYYY/jMM/jDD HH:mm:ss');
  }

  // Persian month and day names
  formatWithNames() {
    const persianMonths = [
      'فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور',
      'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'
    ];
    
    const persianDays = [
      'یکشنبه', 'دوشنبه', 'سه‌شنبه', 'چهارشنبه', 'پنج‌شنبه', 'جمعه', 'شنبه'
    ];

    const jYear = this.moment.jYear();
    const jMonth = this.moment.jMonth();
    const jDate = this.moment.jDate();
    const dayOfWeek = this.moment.day();

    return `${persianDays[dayOfWeek]}، ${jDate} ${persianMonths[jMonth]} ${jYear}`;
  }

  // Conversion methods
  toGregorian() {
    return this.moment.toDate();
  }

  toISOString() {
    return this.moment.toISOString();
  }

  toJalali() {
    return {
      year: this.moment.jYear(),
      month: this.moment.jMonth() + 1, // moment uses 0-based months
      day: this.moment.jDate()
    };
  }

  // Manipulation methods
  add(amount, unit) {
    return new PersianDate(this.moment.clone().add(amount, unit));
  }

  subtract(amount, unit) {
    return new PersianDate(this.moment.clone().subtract(amount, unit));
  }

  startOf(unit) {
    return new PersianDate(this.moment.clone().startOf(unit));
  }

  endOf(unit) {
    return new PersianDate(this.moment.clone().endOf(unit));
  }

  // Comparison methods
  isBefore(date) {
    return this.moment.isBefore(date instanceof PersianDate ? date.moment : date);
  }

  isAfter(date) {
    return this.moment.isAfter(date instanceof PersianDate ? date.moment : date);
  }

  isSame(date, unit = 'day') {
    return this.moment.isSame(date instanceof PersianDate ? date.moment : date, unit);
  }

  isBetween(start, end, unit = 'day') {
    const startMoment = start instanceof PersianDate ? start.moment : start;
    const endMoment = end instanceof PersianDate ? end.moment : end;
    return this.moment.isBetween(startMoment, endMoment, unit);
  }

  // Utility methods
  isValid() {
    return this.moment.isValid();
  }

  clone() {
    return new PersianDate(this.moment.clone());
  }

  valueOf() {
    return this.moment.valueOf();
  }

  toString() {
    return this.format();
  }

  // Static utility methods
  static getMonthNames() {
    return [
      'فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور',
      'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'
    ];
  }

  static getDayNames() {
    return ['یکشنبه', 'دوشنبه', 'سه‌شنبه', 'چهارشنبه', 'پنج‌شنبه', 'جمعه', 'شنبه'];
  }

  static getShortDayNames() {
    return ['ی', 'د', 'س', 'چ', 'پ', 'ج', 'ش'];
  }

  // Date range utilities
  static getDateRange(start, end) {
    const startDate = start instanceof PersianDate ? start : new PersianDate(start);
    const endDate = end instanceof PersianDate ? end : new PersianDate(end);
    const dates = [];
    const current = startDate.clone();

    while (current.isBefore(endDate) || current.isSame(endDate)) {
      dates.push(current.clone());
      current.add(1, 'day');
    }

    return dates;
  }

  // Fiscal year utilities (Persian calendar year)
  static getCurrentFiscalYear() {
    const now = new PersianDate();
    return now.moment.jYear();
  }

  static getFiscalYearStart(year = null) {
    const fiscalYear = year || PersianDate.getCurrentFiscalYear();
    return PersianDate.parse(`${fiscalYear}/01/01`);
  }

  static getFiscalYearEnd(year = null) {
    const fiscalYear = year || PersianDate.getCurrentFiscalYear();
    return PersianDate.parse(`${fiscalYear}/12/29`); // Persian year can have 29 or 30 days in last month
  }
}

// Export utility functions
export const formatPersianDate = (date, format = 'jYYYY/jMM/jDD') => {
  return new PersianDate(date).format(format);
};

export const parsePersianDate = (dateString, format = 'jYYYY/jMM/jDD') => {
  return PersianDate.parse(dateString, format);
};

export const getCurrentPersianDate = () => {
  return new PersianDate();
};

export const convertToPersian = (gregorianDate) => {
  return new PersianDate(gregorianDate);
};

export const convertToGregorian = (persianDateString, format = 'jYYYY/jMM/jDD') => {
  return PersianDate.parse(persianDateString, format).toGregorian();
};

// Persian number conversion
export const toPersianNumbers = (str) => {
  const persianDigits = ['۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹'];
  return str.toString().replace(/\d/g, (digit) => persianDigits[parseInt(digit)]);
};

export const toEnglishNumbers = (str) => {
  const englishDigits = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
  const persianDigits = ['۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹'];
  
  let result = str.toString();
  persianDigits.forEach((persian, index) => {
    result = result.replace(new RegExp(persian, 'g'), englishDigits[index]);
  });
  
  return result;
};

export default PersianDate;