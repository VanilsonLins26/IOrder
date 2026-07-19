import type { OpeningHourResponse } from '../models/store.model';

/**
 * Checks if a given date and time falls within a store's opening hours.
 * @param selectedDate The selected Date object to check.
 * @param openingHours The store's configured opening hours.
 * @returns boolean true if the store is open at the given date/time, false otherwise.
 */
export function isDateTimeWithinOpeningHours(selectedDate: Date, openingHours: OpeningHourResponse[]): boolean {
  if (!openingHours || openingHours.length === 0) {
    return true; // Se a loja não configurou horários, assumimos que está aberta (não bloqueia vendas)
  }

  const dayOfWeek = selectedDate.getDay(); // 0 = Sunday, 1 = Monday, etc. (Matches C# DayOfWeek)
  const hoursForDay = openingHours.filter(h => h.dayOfWeek === dayOfWeek);

  if (hoursForDay.length === 0) {
    return false; // No hours configured for this day = closed
  }

  const selectedMinutes = selectedDate.getHours() * 60 + selectedDate.getMinutes();

  for (const range of hoursForDay) {
    const openMinutes = parseTimeStringToMinutes(range.openHour);
    let closeMinutes = parseTimeStringToMinutes(range.closeHour);

    if (closeMinutes < openMinutes) {
      // The range goes past midnight (e.g. 20:00 to 02:00)
      if (selectedMinutes >= openMinutes || selectedMinutes <= closeMinutes) {
        return true;
      }
    } else {
      // Normal range (e.g. 08:00 to 18:00)
      if (selectedMinutes >= openMinutes && selectedMinutes <= closeMinutes) {
        return true;
      }
    }
  }

  return false;
}

export function parseTimeStringToMinutes(timeString: string): number {
  if (!timeString) return 0;
  const parts = timeString.split(':');
  const hours = parseInt(parts[0] || '0', 10);
  const minutes = parseInt(parts[1] || '0', 10);
  return hours * 60 + minutes;
}

export function generateAvailableDates(openingHours: OpeningHourResponse[], daysAhead: number = 7): { date: string, label: string }[] {
  const dates: { date: string, label: string }[] = [];
  const today = new Date();
  
  const daysOfWeekNames = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'];

  for (let i = 0; i < daysAhead; i++) {
    const d = new Date(today.getFullYear(), today.getMonth(), today.getDate() + i);
    const dayOfWeek = d.getDay();
    
    // Check if store is open on this day
    if (!openingHours || openingHours.length === 0 || openingHours.some(h => h.dayOfWeek === dayOfWeek)) {
      const yyyy = d.getFullYear();
      const mo = String(d.getMonth() + 1).padStart(2, '0');
      const da = String(d.getDate()).padStart(2, '0');
      const dateStr = `${yyyy}-${mo}-${da}`;
      
      let label = ``;
      if (i === 0) {
        label = `Hoje - ` + daysOfWeekNames[dayOfWeek];
      } else if (i === 1) {
        label = `Amanhã - ` + daysOfWeekNames[dayOfWeek];
      } else {
        const dd = String(d.getDate()).padStart(2, '0');
        const mm = String(d.getMonth() + 1).padStart(2, '0');
        label = `${dd}/${mm} - ` + daysOfWeekNames[dayOfWeek];
      }

      dates.push({ date: dateStr, label });
    }
  }

  return dates;
}

export interface TimeSlot {
  value: string;
  label: string;
}

export function generateTimeSlots(dateStr: string, openingHours: OpeningHourResponse[], intervalMin: number = 30): TimeSlot[] {
  if (!dateStr) return [];

  const parts = dateStr.split('-');
  if (parts.length !== 3) return [];
  const selectedDate = new Date(parseInt(parts[0], 10), parseInt(parts[1], 10) - 1, parseInt(parts[2], 10));
  const dayOfWeek = selectedDate.getDay();

  const hoursForDay = (!openingHours || openingHours.length === 0) 
    ? [{ openHour: '08:00', closeHour: '22:00', dayOfWeek }] // default fallback if no config
    : openingHours.filter(h => h.dayOfWeek === dayOfWeek);

  if (hoursForDay.length === 0) return [];

  const slotsMap = new Map<string, TimeSlot>();
  const now = new Date();
  
  const yyyy = now.getFullYear();
  const mo = String(now.getMonth() + 1).padStart(2, '0');
  const da = String(now.getDate()).padStart(2, '0');
  const nowStr = `${yyyy}-${mo}-${da}`;
  
  const isToday = nowStr === dateStr;
  const currentMinutes = now.getHours() * 60 + now.getMinutes();
  const bufferMinutes = 30; // Min time to prepare an order

  for (const range of hoursForDay) {
    const openMinutes = parseTimeStringToMinutes(range.openHour);
    let closeMinutes = parseTimeStringToMinutes(range.closeHour);

    if (closeMinutes <= openMinutes) {
      closeMinutes += 24 * 60; // Next day
    }

    let start = openMinutes;
    // Align start to the next interval (e.g. 08:00, 08:30)
    if (start % intervalMin !== 0) {
      start += intervalMin - (start % intervalMin);
    }

    for (let m = start; m <= closeMinutes; m += intervalMin) {
      const mInDay = m % (24 * 60);
      
      if (isToday) {
        if (m < currentMinutes + bufferMinutes) {
          continue;
        }
      }

      const hh = String(Math.floor(mInDay / 60)).padStart(2, '0');
      const mm = String(mInDay % 60).padStart(2, '0');
      
      const nextM = m + intervalMin;
      const nextMInDay = nextM % (24 * 60);
      const nextHh = String(Math.floor(nextMInDay / 60)).padStart(2, '0');
      const nextMm = String(nextMInDay % 60).padStart(2, '0');

      const value = `${hh}:${mm}`;
      const label = `${hh}:${mm} a ${nextHh}:${nextMm}`;

      if (!slotsMap.has(value)) {
        slotsMap.set(value, { value, label });
      }
    }
  }

  return Array.from(slotsMap.values()).sort((a, b) => parseTimeStringToMinutes(a.value) - parseTimeStringToMinutes(b.value));
}
