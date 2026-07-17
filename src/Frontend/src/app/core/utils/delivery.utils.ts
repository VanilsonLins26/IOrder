const EARTH_RADIUS_KM = 6371;

function toRad(deg: number): number {
  return (deg * Math.PI) / 180;
}

export function haversineDistance(
  lat1: number,
  lon1: number,
  lat2: number,
  lon2: number,
): number {
  const dLat = toRad(lat2 - lat1);
  const dLon = toRad(lon2 - lon1);
  const a =
    Math.sin(dLat / 2) ** 2 +
    Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) * Math.sin(dLon / 2) ** 2;
  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  return EARTH_RADIUS_KM * c;
}

export function calculateDeliveryFee(
  baseFee: number,
  feePerKm: number,
  storeLat: number,
  storeLon: number,
  userLat: number,
  userLon: number,
  freeRadiusKm: number = 0,
): { distanceKm: number; fee: number } {
  const distanceKm = haversineDistance(storeLat, storeLon, userLat, userLon);
  if (freeRadiusKm > 0 && distanceKm <= freeRadiusKm) {
    return { distanceKm: Math.round(distanceKm * 10) / 10, fee: 0 };
  }
  const fee = baseFee + feePerKm * distanceKm;
  return { distanceKm: Math.round(distanceKm * 10) / 10, fee: Math.round(fee * 100) / 100 };
}

export function calculateAppDeliveryFee(): number {
  return 7.99;
}

export function getDeliveryTimeRange(deliveryDate: Date): { start: string; end: string } {
  const start = deliveryDate.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
  const endDate = new Date(deliveryDate.getTime() + 30 * 60 * 1000);
  const end = endDate.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
  return { start, end };
}
