export function parseDate(dStr: string): Date {
    if (/^\d{8}$/.test(dStr)) {
      return new Date(`${dStr.slice(0, 4)}-${dStr.slice(4, 6)}-${dStr.slice(6, 8)}`);
    }

    return new Date(dStr);
  }