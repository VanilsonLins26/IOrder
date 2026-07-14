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
