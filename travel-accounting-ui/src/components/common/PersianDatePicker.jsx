import React, { useState, useEffect, useRef } from 'react';
import { PersianDate } from '../../utils/persianDate';
import './PersianDatePicker.css';

const PersianDatePicker = ({ 
  value, 
  onChange, 
  placeholder = 'انتخاب تاریخ',
  disabled = false,
  className = '',
  showTime = false,
  format = 'jYYYY/jMM/jDD'
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [displayValue, setDisplayValue] = useState('');
  const [currentMonth, setCurrentMonth] = useState(new PersianDate());
  const [selectedDate, setSelectedDate] = useState(null);
  const [timeValue, setTimeValue] = useState({ hour: '00', minute: '00' });
  const containerRef = useRef(null);
  const inputRef = useRef(null);

  useEffect(() => {
    if (value) {
      const persianDate = new PersianDate(value);
      setSelectedDate(persianDate);
      setDisplayValue(persianDate.format(format));
      setCurrentMonth(persianDate.clone().startOf('month'));
      
      if (showTime) {
        setTimeValue({
          hour: persianDate.format('HH'),
          minute: persianDate.format('mm')
        });
      }
    } else {
      setSelectedDate(null);
      setDisplayValue('');
    }
  }, [value, format, showTime]);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (containerRef.current && !containerRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleInputClick = () => {
    if (!disabled) {
      setIsOpen(!isOpen);
    }
  };

  const handleDateSelect = (date) => {
    let finalDate = date.clone();
    
    if (showTime) {
      finalDate = finalDate
        .add(parseInt(timeValue.hour), 'hours')
        .add(parseInt(timeValue.minute), 'minutes');
    }
    
    setSelectedDate(finalDate);
    setDisplayValue(finalDate.format(format));
    
    if (onChange) {
      onChange(finalDate.toGregorian());
    }
    
    if (!showTime) {
      setIsOpen(false);
    }
  };

  const handleTimeChange = (type, value) => {
    const newTimeValue = { ...timeValue, [type]: value };
    setTimeValue(newTimeValue);
    
    if (selectedDate) {
      const updatedDate = selectedDate.clone()
        .startOf('day')
        .add(parseInt(newTimeValue.hour), 'hours')
        .add(parseInt(newTimeValue.minute), 'minutes');
      
      setSelectedDate(updatedDate);
      setDisplayValue(updatedDate.format(format));
      
      if (onChange) {
        onChange(updatedDate.toGregorian());
      }
    }
  };

  const navigateMonth = (direction) => {
    setCurrentMonth(prev => prev.add(direction, 'month'));
  };

  const navigateYear = (direction) => {
    setCurrentMonth(prev => prev.add(direction, 'year'));
  };

  const renderCalendar = () => {
    const monthStart = currentMonth.clone().startOf('month');
    const monthEnd = currentMonth.clone().endOf('month');
    const startDate = monthStart.clone().startOf('week');
    const endDate = monthEnd.clone().endOf('week');
    
    const days = [];
    const current = startDate.clone();
    
    while (current.isBefore(endDate) || current.isSame(endDate, 'day')) {
      days.push(current.clone());
      current.add(1, 'day');
    }
    
    const weeks = [];
    for (let i = 0; i < days.length; i += 7) {
      weeks.push(days.slice(i, i + 7));
    }
    
    return weeks;
  };

  const isToday = (date) => {
    return date.isSame(new PersianDate(), 'day');
  };

  const isSelected = (date) => {
    return selectedDate && date.isSame(selectedDate, 'day');
  };

  const isCurrentMonth = (date) => {
    return date.isSame(currentMonth, 'month');
  };

  const monthNames = PersianDate.getMonthNames();
  const dayNames = PersianDate.getShortDayNames();

  return (
    <div className={`persian-datepicker ${className}`} ref={containerRef}>
      <div 
        className={`persian-datepicker-input ${disabled ? 'disabled' : ''}`}
        onClick={handleInputClick}
        ref={inputRef}
      >
        <input
          type="text"
          value={displayValue}
          placeholder={placeholder}
          readOnly
          disabled={disabled}
          className="persian-datepicker-field"
        />
        <span className="persian-datepicker-icon">📅</span>
      </div>
      
      {isOpen && (
        <div className="persian-datepicker-dropdown">
          <div className="persian-datepicker-header">
            <div className="persian-datepicker-nav">
              <button 
                type="button" 
                onClick={() => navigateYear(-1)}
                className="persian-datepicker-nav-btn"
              >
                ≪
              </button>
              <button 
                type="button" 
                onClick={() => navigateMonth(-1)}
                className="persian-datepicker-nav-btn"
              >
                ‹
              </button>
              
              <div className="persian-datepicker-title">
                {monthNames[currentMonth.moment.jMonth()]} {currentMonth.moment.jYear()}
              </div>
              
              <button 
                type="button" 
                onClick={() => navigateMonth(1)}
                className="persian-datepicker-nav-btn"
              >
                ›
              </button>
              <button 
                type="button" 
                onClick={() => navigateYear(1)}
                className="persian-datepicker-nav-btn"
              >
                ≫
              </button>
            </div>
          </div>
          
          <div className="persian-datepicker-calendar">
            <div className="persian-datepicker-weekdays">
              {dayNames.map((day, index) => (
                <div key={index} className="persian-datepicker-weekday">
                  {day}
                </div>
              ))}
            </div>
            
            <div className="persian-datepicker-days">
              {renderCalendar().map((week, weekIndex) => (
                <div key={weekIndex} className="persian-datepicker-week">
                  {week.map((day, dayIndex) => (
                    <button
                      key={dayIndex}
                      type="button"
                      className={`
                        persian-datepicker-day
                        ${isCurrentMonth(day) ? 'current-month' : 'other-month'}
                        ${isToday(day) ? 'today' : ''}
                        ${isSelected(day) ? 'selected' : ''}
                      `}
                      onClick={() => handleDateSelect(day)}
                    >
                      {day.moment.jDate()}
                    </button>
                  ))}
                </div>
              ))}
            </div>
          </div>
          
          {showTime && (
            <div className="persian-datepicker-time">
              <div className="persian-datepicker-time-inputs">
                <div className="persian-datepicker-time-group">
                  <label>ساعت</label>
                  <select 
                    value={timeValue.hour} 
                    onChange={(e) => handleTimeChange('hour', e.target.value)}
                    className="persian-datepicker-time-select"
                  >
                    {Array.from({ length: 24 }, (_, i) => (
                      <option key={i} value={i.toString().padStart(2, '0')}>
                        {i.toString().padStart(2, '0')}
                      </option>
                    ))}
                  </select>
                </div>
                
                <div className="persian-datepicker-time-separator">:</div>
                
                <div className="persian-datepicker-time-group">
                  <label>دقیقه</label>
                  <select 
                    value={timeValue.minute} 
                    onChange={(e) => handleTimeChange('minute', e.target.value)}
                    className="persian-datepicker-time-select"
                  >
                    {Array.from({ length: 60 }, (_, i) => (
                      <option key={i} value={i.toString().padStart(2, '0')}>
                        {i.toString().padStart(2, '0')}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            </div>
          )}
          
          <div className="persian-datepicker-footer">
            <button 
              type="button" 
              onClick={() => {
                const today = new PersianDate();
                handleDateSelect(today);
              }}
              className="persian-datepicker-today-btn"
            >
              امروز
            </button>
            
            <button 
              type="button" 
              onClick={() => setIsOpen(false)}
              className="persian-datepicker-close-btn"
            >
              بستن
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default PersianDatePicker;