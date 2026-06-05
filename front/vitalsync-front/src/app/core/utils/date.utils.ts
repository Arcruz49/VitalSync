function pad(n: number): string {
  return String(n).padStart(2, '0');
}

export function isoToDisplayDateTime(iso: string): string {
  const d = new Date(iso);
  return `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()} ${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

export function isoToInputDate(iso: string): string {
  const d = new Date(iso);
  return `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()}`;
}

export function isoToInputDateTime(iso: string): string {
  const d = new Date(iso);
  return `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()} ${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

export function nowInputDateTime(): string {
  const d = new Date();
  return `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()} ${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

// Parses "dd/mm/yyyy" → ISO UTC string (date only, noon to avoid DST edge cases)
export function inputDateToISO(dmy: string): string {
  const [dd, mm, yyyy] = dmy.split('/');
  if (!dd || !mm || !yyyy || yyyy.length < 4) return '';
  const d = new Date(+yyyy, +mm - 1, +dd, 12, 0, 0);
  return isNaN(d.getTime()) ? '' : d.toISOString();
}

// Parses "dd/mm/yyyy HH:mm" → ISO UTC string (local time → UTC)
export function inputDateTimeToISO(dmyHm: string): string {
  const [datePart = '', timePart = '00:00'] = dmyHm.trim().split(' ');
  const [dd, mm, yyyy] = datePart.split('/');
  const [hh, min] = timePart.split(':');
  if (!dd || !mm || !yyyy || yyyy.length < 4) return '';
  const d = new Date(+yyyy, +mm - 1, +dd, +(hh ?? 0), +(min ?? 0), 0);
  return isNaN(d.getTime()) ? '' : d.toISOString();
}

// ── NATIVE PICKER HELPERS ────────────────────────────
// type="date" value format: "YYYY-MM-DD" (local)
export function isoToPickerDate(iso: string): string {
  const d = new Date(iso);
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
}

// type="datetime-local" value format: "YYYY-MM-DDTHH:mm" (local)
export function isoToPickerDateTime(iso: string): string {
  const d = new Date(iso);
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

export function nowPickerDateTime(): string {
  return isoToPickerDateTime(new Date().toISOString());
}

// "YYYY-MM-DD" → ISO UTC (noon local to avoid DST issues)
export function pickerDateToISO(ymd: string): string {
  if (!ymd) return '';
  const d = new Date(ymd + 'T12:00:00');
  return isNaN(d.getTime()) ? '' : d.toISOString();
}

// "YYYY-MM-DDTHH:mm" → ISO UTC (browser treats no-TZ as local)
export function pickerDateTimeToISO(ymdhm: string): string {
  if (!ymdhm) return '';
  const d = new Date(ymdhm);
  return isNaN(d.getTime()) ? '' : d.toISOString();
}

// Auto-masks a date input: "ddmmyyyy" → "dd/mm/yyyy"
export function maskDate(value: string): string {
  const digits = value.replace(/\D/g, '').slice(0, 8);
  if (digits.length <= 2) return digits;
  if (digits.length <= 4) return `${digits.slice(0, 2)}/${digits.slice(2)}`;
  return `${digits.slice(0, 2)}/${digits.slice(2, 4)}/${digits.slice(4)}`;
}

// Auto-masks a datetime input: "ddmmyyyyHHMM" → "dd/mm/yyyy HH:mm"
export function maskDateTime(value: string): string {
  const digits = value.replace(/\D/g, '').slice(0, 12);
  if (digits.length <= 2) return digits;
  if (digits.length <= 4) return `${digits.slice(0, 2)}/${digits.slice(2)}`;
  if (digits.length <= 8) return `${digits.slice(0, 2)}/${digits.slice(2, 4)}/${digits.slice(4)}`;
  if (digits.length <= 10) return `${digits.slice(0, 2)}/${digits.slice(2, 4)}/${digits.slice(4, 8)} ${digits.slice(8)}`;
  return `${digits.slice(0, 2)}/${digits.slice(2, 4)}/${digits.slice(4, 8)} ${digits.slice(8, 10)}:${digits.slice(10)}`;
}
