import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface ViaCepResponse {
  logradouro: string;
  bairro: string;
  localidade: string;
  uf: string;
  erro?: boolean;
}

@Injectable({ providedIn: 'root' })
export class ViacepApiService {
  private readonly http = inject(HttpClient);

  getByCep(cep: string): Observable<{ street: string; neighborhood: string; city: string; state: string } | null> {
    const cleaned = cep.replace(/\D/g, '');
    if (cleaned.length !== 8) return new Observable(sub => sub.next(null));

    return this.http.get<ViaCepResponse>(`https://viacep.com.br/ws/${cleaned}/json/`).pipe(
      map(res => {
        if (res.erro) return null;
        return {
          street: res.logradouro,
          neighborhood: res.bairro,
          city: res.localidade,
          state: res.uf,
        };
      })
    );
  }
}
