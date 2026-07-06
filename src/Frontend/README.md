# IOrderApp — Frontend

Frontend Angular do **IOrder**, plataforma de encomendas personalizadas.

## Stack

- Angular 20 (standalone components, signals)
- Auth0 (`@auth0/auth0-angular`)
- `@ngrx/signals` (state management)

## Rotas

### Públicas
| Rota | Componente | Descrição |
|---|---|---|
| `/` | HomePage | Landing page com listagem de lojas |
| `/stores` | StoresList | Catálogo de lojas |
| `/stores/:id` | StoreDetail | Detalhes da loja e produtos |

### Admin (requer autenticação + role `ShopKeeper`)
| Rota | Componente | Descrição |
|---|---|---|
| `/admin/setup-store` | SetupStore | Criar loja (primeiro acesso) |
| `/admin/dashboard` | Dashboard | Painel principal |
| `/admin/products` | ProductManagement | CRUD de produtos |
| `/admin/categories` | CategoryManagement | CRUD de categorias |
| `/admin/store` | StoreSettings | Configurações da loja |

## Desenvolvimento

```bash
npm install
ng serve
```

Acesse `http://localhost:4200`. O proxy reverso redireciona chamadas `/api/*` para o backend em `https://localhost:7023` (configurado em `proxy.conf.json`).

## Build

```bash
ng build
```
