import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { UserAddressApiService } from '../../../../core/services/api/user-address-api.service';
import { ViacepApiService } from '../../../../core/services/api/viacep-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import type { UserAddressResponse, UserAddressRequest } from '../../../../core/models';

@Component({
  selector: 'app-addresses-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './addresses-page.component.html',
  styleUrl: './addresses-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AddressesPageComponent implements OnInit {
  private readonly addressApi = inject(UserAddressApiService);
  private readonly viacepApi = inject(ViacepApiService);
  private readonly toast = inject(ToastService);

  readonly addresses = signal<UserAddressResponse[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly loadingCep = signal(false);
  readonly showForm = signal(false);

  readonly formName = signal('');
  readonly formZipCode = signal('');
  readonly formStreet = signal('');
  readonly formNumber = signal('');
  readonly formComplement = signal('');
  readonly formNeighborhood = signal('');
  readonly formCity = signal('');
  readonly formState = signal('');

  ngOnInit() {
    this.loadAddresses();
  }

  loadAddresses() {
    this.loading.set(true);
    this.addressApi.getAll().subscribe({
      next: (addresses) => {
        this.addresses.set(addresses);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  onCepBlur() {
    const cep = this.formZipCode().replace(/\D/g, '');
    if (cep.length !== 8) return;

    this.loadingCep.set(true);
    this.viacepApi.getByCep(cep).subscribe({
      next: (result) => {
        if (result) {
          this.formStreet.set(result.street);
          this.formNeighborhood.set(result.neighborhood);
          this.formCity.set(result.city);
          this.formState.set(result.state);
        }
        this.loadingCep.set(false);
      },
      error: () => {
        this.loadingCep.set(false);
      },
    });
  }

  openForm() {
    this.resetForm();
    this.showForm.set(true);
  }

  cancelForm() {
    this.showForm.set(false);
    this.resetForm();
  }

  saveAddress() {
    if (!this.formName() || !this.formZipCode() || !this.formStreet() || !this.formNumber() || !this.formNeighborhood() || !this.formCity() || !this.formState()) {
      this.toast.error('Preencha todos os campos obrigatórios.');
      return;
    }

    this.saving.set(true);
    const dto: UserAddressRequest = {
      name: this.formName(),
      zipCode: this.formZipCode().replace(/\D/g, ''),
      street: this.formStreet(),
      number: this.formNumber(),
      complement: this.formComplement(),
      neighborhood: this.formNeighborhood(),
      city: this.formCity(),
      state: this.formState(),
    };

    this.addressApi.add(dto).subscribe({
      next: (created) => {
        this.addresses.update(list => [...list, created]);
        this.showForm.set(false);
        this.resetForm();
        this.saving.set(false);
        this.toast.success('Endereço salvo com sucesso.');
      },
      error: (err) => {
        this.saving.set(false);
        this.toast.error(err.error?.errors?.[0] || 'Erro ao salvar endereço.');
      },
    });
  }

  setDefault(id: string) {
    this.addressApi.setDefault(id).subscribe({
      next: () => {
        this.addresses.update(list =>
          list.map(a => ({ ...a, isDefault: a.id === id }))
        );
        this.toast.success('Endereço padrão atualizado.');
      },
      error: () => this.toast.error('Erro ao definir endereço padrão.'),
    });
  }

  deleteAddress(id: string) {
    if (!confirm('Deseja remover este endereço?')) return;

    this.addressApi.delete(id).subscribe({
      next: () => {
        this.addresses.update(list => list.filter(a => a.id !== id));
        this.toast.success('Endereço removido.');
      },
      error: () => this.toast.error('Erro ao remover endereço.'),
    });
  }

  private resetForm() {
    this.formName.set('');
    this.formZipCode.set('');
    this.formStreet.set('');
    this.formNumber.set('');
    this.formComplement.set('');
    this.formNeighborhood.set('');
    this.formCity.set('');
    this.formState.set('');
  }
}
