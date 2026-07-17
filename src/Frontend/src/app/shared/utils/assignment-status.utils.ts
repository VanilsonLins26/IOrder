import { AssignmentStatusDto } from '../../core/models';

const LABELS: Record<number, string> = {
  [AssignmentStatusDto.Pending]: 'Pendente',
  [AssignmentStatusDto.Accepted]: 'Aceita',
  [AssignmentStatusDto.Rejected]: 'Rejeitada',
  [AssignmentStatusDto.PickedUp]: 'Coletado',
  [AssignmentStatusDto.InTransit]: 'Em Trânsito',
  [AssignmentStatusDto.Delivered]: 'Entregue',
  [AssignmentStatusDto.Failed]: 'Falhou',
};

const STATUS_CLASSES: Record<number, string> = {
  [AssignmentStatusDto.Pending]: 'status--pending',
  [AssignmentStatusDto.Accepted]: 'status--accepted',
  [AssignmentStatusDto.Rejected]: 'status--rejected',
  [AssignmentStatusDto.PickedUp]: 'status--picked-up',
  [AssignmentStatusDto.InTransit]: 'status--in-transit',
  [AssignmentStatusDto.Delivered]: 'status--delivered',
  [AssignmentStatusDto.Failed]: 'status--failed',
};

export function getAssignmentStatusLabel(status: number): string {
  return LABELS[status] ?? 'Desconhecido';
}

export function getAssignmentStatusClass(status: number): string {
  return STATUS_CLASSES[status] ?? '';
}

export function canAccept(status: number): boolean {
  return status === AssignmentStatusDto.Pending;
}

export function canReject(status: number): boolean {
  return status === AssignmentStatusDto.Pending;
}

export function canPickup(status: number): boolean {
  return status === AssignmentStatusDto.Accepted;
}

export function canStartTransit(status: number): boolean {
  return status === AssignmentStatusDto.PickedUp;
}

export function canDeliver(status: number): boolean {
  return status === AssignmentStatusDto.PickedUp || status === AssignmentStatusDto.InTransit;
}
