# 🛒 IOrder

![Build Status](https://img.shields.io/badge/build-Azure%20DevOps-blue?logo=azure-devops)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-20-DD0031?logo=angular)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql)
![Redis](https://img.shields.io/badge/Redis-7.0-DC382D?logo=redis)
![Auth0](https://img.shields.io/badge/Auth0-OAuth2-EB5424?logo=auth0)

**IOrder** é uma plataforma de encomendas personalizadas inspirada no modelo de grandes apps de delivery, mas com um diferencial: foco em **encomendas de todos os tipos** (comidas, roupas, itens personalizados, etc.) com fluxo de aprovação e negociação direta entre cliente e lojista.

O usuário escolhe a loja ou busca por categoria, personaliza seus produtos, escolhe a data e hora desejada para entrega/retirada e envia a solicitação. Um **chat em tempo real** é aberto entre cliente e lojista, que pode analisar, aprovar o pedido e dar seguimento (pagamento, preparo, entrega).

> Projeto pessoal de portfólio com foco em boas práticas de engenharia de software, arquitetura limpa e tecnologias escaláveis.

---

## 📌 Roadmap

### Backend (ASP.NET Core Web API — C#)

- ✅ Estruturação Clean Architecture + DDD com SeedWork
- ✅ CRUD de Produtos (Read/Write segregation)
- ✅ CRUD de Lojas (endereço, horários, upload de imagens)
- ✅ CRUD de Categorias por loja (com ordenação)
- ✅ Categorias de Loja (StoreCategory — categorias pré-definidas)
- ✅ Filtros dinâmicos, paginação e ordenação centralizados
- ✅ Tratamento global de exceções (ExceptionFilter)
- ✅ Background Service para sincronização de preços promocionais
- ✅ Autenticação JWT via Auth0 (roles: Cliente, ShopKeeper)
- ✅ Upload de imagens para Cloudinary
- ✅ Redis Cache para carrinho de compras
- ✅ Seed Data populado (lojas, categorias, produtos)
- ✅ Testes de unidade + integração com TestContainers
- 🔄 Carrinho de Compras (use cases e controllers em finalização)
- ⬜ Módulo de Chat via SignalR
- ⬜ Integração com Mercado Pago
- ⬜ Integração com EvolutionAPI (WhatsApp)
- ⬜ RabbitMQ (fila de comandos)
- ⬜ Apache Kafka (log de eventos — opcional)

### Frontend (Angular 20)

- ✅ Home page com listagem de lojas
- ✅ Catálogo de lojas com detalhes
- ✅ Painel Admin (lazy loading) com:
  - ✅ Setup de Loja
  - ✅ Dashboard
  - ✅ Gerenciamento de Produtos
  - ✅ Gerenciamento de Categorias
  - ✅ Configurações da Loja
- ✅ Guards de autenticação (Auth0) e roles
- ✅ Guard de verificação de loja
- ✅ Layouts separados (Cliente + Admin)
- ⬜ Integração com carrinho (após finalização do backend)
- ⬜ Chat em tempo real

### Infraestrutura

- ✅ MySQL 8.0 via Docker
- ✅ Redis via Docker
- ✅ CI/CD com Azure Pipelines (build + testes + cobertura)
- ✅ Entity Framework Core Migrations (10 migrations)
- ✅ TestContainers para testes de integração
- ⬜ MongoDB (chat)
- ⬜ Deploy em produção

---

## 🏗️ Arquitetura

```
┌─────────────────────────────────────────────────────────┐
│                 IOrder.API (Presentation)                │
│  Controllers · Filters · Program.cs · Swagger / OpenAPI │
└──────────────────────┬──────────────────────────────────┘
                       │ depende de
┌──────────────────────▼──────────────────────────────────┐
│             IOrder.Application (Use Cases / CQRS)        │
│  Commands · Queries · Validators (FluentValidation)      │
│  Services (LoggedUser, StorePermission, Mapper)          │
└──────────────────────┬──────────────────────────────────┘
                       │ depende de
┌──────────────────────▼──────────────────────────────────┐
│               IOrder.Domain (Core Business)              │
│  Entities · Aggregates · Value Objects · Enums           │
│  Repository Interfaces · Domain Events · SeedWork        │
└──────────────────────┬──────────────────────────────────┘
                       │ implementa
┌──────────────────────▼──────────────────────────────────┐
│           IOrder.Infrastructure (Data Access / External) │
│  EF Core · DbContext · Migrations · Repositories         │
│  Cloudinary (Storage) · Redis (Cache) · Workers          │
└──────────────────────────────────────────────────────────┘

┌─────────────────────┐  ┌────────────────────────────────┐
│ IOrder.Communication │  │    IOrder.Exceptions           │
│ (Request/Response)   │  │ (Custom Exceptions + .resx)    │
└──────────────────────┘  └────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│  Tests (Unit + Integration)                              │
│  UseCases.Test · Validators.Tests · WebApi.Test          │
│  CommomTestUtilities (Builders · Mocks · Fakes)          │
└─────────────────────────────────────────────────────────┘
```

### Camadas

| Camada | Responsabilidade |
|---|---|
| **IOrder.API** | Controllers, filtros globais, configuração DI, Swagger, autenticação |
| **IOrder.Application** | Casos de uso (Commands/Queries), validadores, serviços de aplicação, mapeamento |
| **IOrder.Domain** | Entidades, aggregates, interfaces de repositório, eventos de domínio — **zero dependências externas** |
| **IOrder.Infrastructure** | Implementação concreta: EF Core + MySQL, Redis, Cloudinary, Workers |
| **IOrder.Communication** | DTOs compartilhados (Request/Response/Enums) |
| **IOrder.Exceptions** | Exceções customizadas e mensagens de erro em português |

---

## 🛠️ Tecnologias

| Categoria | Tecnologia | Versão | Status |
|---|---|---|---|
| **Runtime** | .NET | 10.0 | ✅ |
| **Web Framework** | ASP.NET Core Web API | 10.0 | ✅ |
| **ORM** | EF Core + Pomelo MySQL | 9.0 | ✅ |
| **Database** | MySQL | 8.0 | ✅ |
| **Cache** | Redis (StackExchange.Redis) | — | ✅ |
| **Storage** | Cloudinary | — | ✅ |
| **Auth** | Auth0 (OAuth 2.0 / JWT Bearer) | — | ✅ |
| **Mapping** | Mapster | 10.0 | ✅ |
| **Validation** | FluentValidation | 12.1 | ✅ |
| **API Docs** | Swagger / Swashbuckle | 10.2 | ✅ |
| **Frontend** | Angular (standalone, signals) | 20 | ✅ |
| **Test Runner** | xUnit | — | ✅ |
| **Assertion** | Shouldly | — | ✅ |
| **Fake Data** | Bogus | — | ✅ |
| **Mocking** | Moq | — | ✅ |
| **Integration Tests** | TestContainers.MySql | — | ✅ |
| **CI/CD** | Azure Pipelines | — | ✅ |
| **Real-time** | SignalR | — | 🚧 |
| **Messaging** | RabbitMQ | — | 🚧 |
| **Event Log** | Apache Kafka | — | 📝 |
| **Chat DB** | MongoDB | — | 🚧 |
| **Payments** | Mercado Pago | — | 🚧 |
| **WhatsApp** | EvolutionAPI | — | 🚧 |

---

## 🚀 Como Executar

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 22+](https://nodejs.org/)
- [Angular CLI](https://angular.dev/tools/cli) (`npm install -g @angular/cli`)

### 1. Suba os containers

```bash
docker run -d --name mysql-iorder -p 2552:3306 ^
  -e MYSQL_ROOT_PASSWORD=@Password123 ^
  -e MYSQL_DATABASE=iorderdb ^
  mysql:8.0

docker run -d --name redis-iorder -p 6379:6379 redis:7-alpine
```

> ⚠️ A porta `2552` é intencional para evitar conflito com outras instâncias MySQL locais.

### 2. Configure o `appsettings.Development.json`

```json
{
  "Authentication": {
    "Authority": "https://seu-tenant.us.auth0.com/",
    "Audience": "https://api.iorder.com",
    "ClientId": "seu-client-id"
  },
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=2552;userid=root;password=@Password123;database=iorderdb"
  },
  "Cloudinary": {
    "CloudName": "seu-cloud-name",
    "ApiKey": "sua-api-key",
    "ApiSecret": "seu-api-secret"
  },
  "RedisConnection": "localhost:6379"
}
```

### 3. Execute o backend

```bash
cd src/Backend/IOrder.API
dotnet run
```

O Swagger estará disponível em `https://localhost:7023/swagger`.

> O banco de dados é criado e as migrations aplicadas automaticamente na inicialização.

### 4. Execute o frontend

```bash
cd src/Frontend
npm install
ng serve
```

A aplicação estará disponível em `http://localhost:4200`.

---

## 🧪 Testes

```bash
# Testes de unidade (Casos de Uso)
dotnet test tests/UseCases.Test

# Testes de unidade (Validators)
dotnet test tests/Validators.Tests

# Testes de integração (requer Docker — sobe MySQL automaticamente via TestContainers)
dotnet test tests/WebApi.Test

# Todos os testes de uma vez
dotnet test
```

Os testes de integração utilizam **TestContainers** para criar e destruir um container MySQL real a cada execução, garantindo fidelidade com o ambiente de produção.

---

## 🔄 CI/CD (Azure Pipelines)

O pipeline é acionado em pushes para a branch `develop` e executa:

1. Instala .NET SDK 10.x
2. Restaura pacotes NuGet
3. Compila em modo Release
4. Executa **todos os testes** com cobertura de código (coverlet)
5. Publica o relatório de cobertura no Azure DevOps

> Consulte [`azure-pipelines.yml`](azure-pipelines.yml) para detalhes da configuração.

---

## 📁 Estrutura do Projeto

```
IOrderNew/
├── src/
│   ├── Backend/
│   │   ├── IOrder.API              # Presentation Layer
│   │   ├── IOrder.Application       # Use Cases / CQRS
│   │   ├── IOrder.Domain            # Entities / Core
│   │   └── IOrder.Infrastructure    # EF Core / Redis / Cloudinary
│   ├── Shared/
│   │   ├── IOrder.Communication     # DTOs (Request/Response)
│   │   └── IOrder.Exceptions        # Custom Exceptions
│   └── Frontend/                    # Angular 20 SPA
│       └── src/app/
│           ├── core/                # Auth, guards, services
│           ├── features/            # Home, Store Catalog, Admin
│           └── layouts/             # Client + Admin layouts
├── tests/
│   ├── CommomTestUtilities          # Builders / Mocks / Fakes
│   ├── UseCases.Test                # Testes de unidade (casos de uso)
│   ├── Validators.Tests             # Testes de unidade (validators)
│   └── WebApi.Test                  # Testes de integração (TestContainers)
├── azure-pipelines.yml
├── coverlet.runsettings
└── IOrderNew.slnx
```

---

## 📄 Licença

Projeto pessoal para portfólio. Todos os direitos reservados.
