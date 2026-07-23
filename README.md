<div align="center">
  <h1>IOrder Platform</h1>
  <p><strong>Plataforma de encomendas personalizadas com delivery, chat em tempo real e pagamentos integrados</strong></p>

  <p>
    <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&style=for-the-badge" alt=".NET" />
    <img src="https://img.shields.io/badge/Angular-20-DD0031?logo=angular&style=for-the-badge" alt="Angular" />
    <img src="https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql&style=for-the-badge" alt="MySQL" />
    <img src="https://img.shields.io/badge/Redis-7.0-DC382D?logo=redis&style=for-the-badge" alt="Redis" />
    <img src="https://img.shields.io/badge/RabbitMQ-3.13-FF6600?logo=rabbitmq&style=for-the-badge" alt="RabbitMQ" />
    <img src="https://img.shields.io/badge/Kafka-3.9-231F20?logo=apachekafka&style=for-the-badge" alt="Kafka" />
    <img src="https://img.shields.io/badge/Stripe-Payments-635BFF?logo=stripe&style=for-the-badge" alt="Stripe" />
    <img src="https://img.shields.io/badge/Auth0-OAuth2-EB5424?logo=auth0&style=for-the-badge" alt="Auth0" />
    <img src="https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&style=for-the-badge" alt="Docker" />
  </p>
</div>

---

**IOrder** e uma plataforma de encomendas personalizadas inspirada no modelo de grandes apps de delivery, com foco em **encomendas de todos os tipos** (comidas, roupas, itens personalizados). O diferencial e o fluxo de aprovacao e negociacao direta entre cliente e lojista atraves de um **chat em tempo real**.

> **Projeto de portfolio** com foco em boas praticas de engenharia de software: Clean Architecture, DDD, testes automatizados e integracoes assincronas.

---

## Funcionalidades

### Backend (ASP.NET Core Web API - C#)

| Modulo | Descricao |
|:---|:---|
| **Catalogo** | CRUD de Produtos, Lojas e Categorias com paginacao dinamica e ordenacao. |
| **Customizacao de Produtos** | Grupos de opcoes estruturadas (SingleChoice / MultipleChoice) com precificacao dinamica. |
| **Carrinho de Compras** | Redis-based com persistencia por usuario, aplicacao de cupons e calculo automatico. |
| **Pedidos** | Fluxo completo: criacao, negociacao, aprovacao, preparo, entrega e avaliacao. |
| **Chat em Tempo Real** | SignalR integrado ao fluxo de pedidos com indicadores de digitacao e recibos de leitura. |
| **Pagamentos** | Stripe (Cartao, PIX, Boleto) com Payment Intents, Webhooks e cartoes salvos. |
| **Delivery** | Módulo de entregas com broadcast de ofertas, aceitacao, rastreamento geoespacial e auto-busca de entregadores. |
| **Avaliacoes** | Sistema de rating para lojas e entregadores apos entrega. |
| **Cupons** | Geracao e aplicacao de cupons de desconto (percentual ou valor fixo). |
| **Enderecos** | Gerenciamento de enderecos do usuario com endereco padrao. |
| **Mensageria** | Domain Events via Kafka + fila de chat via RabbitMQ com dedup via Redis. |
| **Notificacoes** | E-mail (SMTP/MailHog) e WhatsApp (Evolution API) para eventos acionaveis. |
| **Cache Distribuido** | Cache-Aside via Decorator Pattern no Redis com invalidacao automatica. |
| **Background Workers** | Carrinhos abandonados, sincronizacao de promocoes e auto-busca de entregadores. |

### Frontend (Angular 20)

| Modulo | Descricao |
|:---|:---|
| **Catalogo** | Home page, listagem de lojas com filtros, detalhes do produto com customizacao. |
| **Carrinho** | Carrinho com NgRx Signals, aplicacao de cupons e checkout. |
| **Pedidos** | Acompanhamento de status em tempo real com timeline. |
| **Chat** | Integracao SignalR com indicadores de digitacao e recibos de leitura. |
| **Painel Admin** | Dashboard, gestao de loja, produtos, categorias, cupons e pedidos. |
| **App Entregador** | Layout dedicado com mapa interativo (Leaflet) e acoes de entrega. |
| **Pagamento** | Stripe Elements (Payment Element) embutido no checkout. |
| **Perfil** | Gestao de perfil e enderecos do usuario. |

### Infraestrutura & DevOps

| Componente | Descricao |
|:---|:---|
| **Docker Compose** | 6 servicos: MySQL, Redis, RabbitMQ, Kafka (KRaft), MailHog, Kafka UI. |
| **CI/CD** | Azure Pipelines: build, testes com cobertura (Coverlet). |
| **Testes** | 445+ testes: unitarios (xUnit + Moq + Bogus), validacao (FluentValidation) e integracao (TestContainers). |
| **Migrations** | 32 migrations Entity Framework Core com auto-migracao no startup. |

---

## Arquitetura

```
src/
├── Backend/
│   ├── IOrder.API                  # Controllers, Filtros, Swagger, SignalR Hub
│   ├── IOrder.Application          # Use Cases (CQRS), Validadores, Servicos de Aplicacao
│   ├── IOrder.Domain               # Entidades, Value Objects, Domain Events, Interfaces de Repositorio
│   └── IOrder.infrastructure       # EF Core, Repositorios, Redis, Workers, Consumers, Integracoes
├── Shared/
│   ├── IOrder.Communication        # DTOs (Request/Response/Enums)
│   └── IOrder.Exceptions           # Excecoes customizadas
└── Frontend/                       # Angular 20 SPA
    └── src/app/
        ├── core/                   # Auth, Guards, Services, Stores, Models
        ├── features/               # Home, Catalog, Cart, Orders, Admin, Courier, Profile
        └── shared/                 # Componentes reutilizaveis
```

### Camadas

| Camada | Responsabilidade |
|:---|:---|
| **IOrder.API** | Controllers HTTP, Filtros globais, Swagger/OpenAPI, Configuracao SignalR e DI. |
| **IOrder.Application** | Casos de uso (Commands/Queries), Validadores (FluentValidation), Servicos de aplicacao (Permissao, Pagamento, Cache, Mapeamento). |
| **IOrder.Domain** | Entidades ricas, Aggregates, Value Objects, Domain Events, Interfaces de Repositorio. **Zero dependencias externas.** |
| **IOrder.infrastructure** | Implementacao concreta: EF Core + MySQL, Redis, Cloudinary, Workers, Consumers (Kafka/RabbitMQ), Integracoes (Stripe, Evolution API, Nominatim). |

---

## Destaques Tecnicos

### Cache-Aside Distribuido (Redis)

Decorator Pattern nos repositórios de leitura e escrita. Consultas frequentes sao cacheadas no Redis com TTL configuravel. Em operacoes de escrita, o cache e invalidado automaticamente, garantindo alta performance sem poluir a camada de aplicacao.

```
Request → CachedRepository → [Cache Hit?] → Redis → Response
                            → [Cache Miss?] → EF Core → MySQL → Redis (save) → Response
```

### Mensageria e Eventos de Dominio

- **Apache Kafka**: Orquestra 19 Domain Events (`OrderCreated`, `PaymentApproved`, `CartAbandoned`, etc.) de forma resiliente.
- **RabbitMQ**: Processa mensagens do chat em alta velocidade com dedup via Redis (30s TTL).
- **Consumers**: `KafkaDomainEventConsumer` (notificacoes) e `ChatConsumer` (broadcast SignalR).

### Pagamentos com Stripe

Fluxo hibrido com `Payment Intents API` no Backend e `Stripe Elements` no Frontend. Suporta Cartao (com parcelamento), PIX e Boleto. Webhooks atualizam o status do pagamento e notificam o frontend via SignalR (`PaymentStatusChanged`). Cartoes sao salvos para compras futuras com dedup por fingerprint.

### Delivery com Rastreamento Geoespacial

- `BroadcastDeliveryOffer`: notifica entregadores elegiveis via SignalR.
- `CourierAutoSearchService`: worker que busca entregadores automaticamente a cada 1 minuto.
- `CourierLocation`: entidade com NTS Point (SRID 4326) para consultas espaciais.
- Mapa interativo (Leaflet) no frontend para rastreamento em tempo real.

### Auto-Migracao e Seed Data

O banco de dados e criado e as migrations aplicadas automaticamente na inicializacao da API. Seed data inclui 10 categorias de loja, 3 lojas (Sao Paulo), 5 categorias de menu e 5 produtos. Script adicional (`scripts/seed-fortaleza.sql`) popula 50 lojas em Fortaleza com coordenadas GPS reais.

---

## Como Executar

### Pre-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 22+](https://nodejs.org/)
- [Angular CLI](https://angular.dev/tools/cli) (`npm install -g @angular/cli`)

### 1. Subir os Servicos (Docker Compose)

```bash
docker compose up -d
```

Isso sobe 6 servicos:

| Servico | Porta | Descricao |
|:---|:---|:---|
| MySQL | 2552 | Banco de dados transacional |
| Redis | 6379 | Cache distribuido |
| RabbitMQ | 5672 / 15672 | Fila de mensagens do chat |
| Kafka | 9092 | Domain Events (KRaft, sem Zookeeper) |
| MailHog | 1025 (SMTP) / 8025 (UI) | Teste de e-mails |
| Kafka UI | 8080 | Dashboard de topics |

### 2. Configurar o Backend

Crie o arquivo `src/Backend/IOrder.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=2552;userid=root;password=@Password123;database=iorderdb"
  },
  "RedisConnection": "localhost:6379",
  "Authentication": {
    "Authority": "https://seu-tenant.us.auth0.com/",
    "Audience": "https://api.iorder.com",
    "ClientId": "seu-client-id"
  },
  "Cloudinary": {
    "CloudName": "seu-cloud-name",
    "ApiKey": "sua-api-key",
    "ApiSecret": "seu-api-secret"
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_...",
    "PublishableKey": "pk_test_..."
  }
}
```

### 3. Executar o Backend

```bash
cd src/Backend/IOrder.API
dotnet run
```

O Swagger estara disponivel em `https://localhost:7023/swagger`.

> O banco de dados e criado e as migrations aplicadas automaticamente na inicializacao.

### 4. Executar o Frontend

```bash
cd src/Frontend
npm install
ng serve
```

A aplicacao estara disponivel em `http://localhost:4200`.

---

## Testes

```bash
# Testes de unidade (Casos de Uso)
dotnet test tests/UseCases.Test

# Testes de unidade (Validators)
dotnet test tests/Validators.Tests

# Testes de integracao (requer Docker - TestContainers)
dotnet test tests/WebApi.Test

# Todos os testes
dotnet test
```

| Projeto | Tipo | Descricao |
|:---|:---|:---|
| `UseCases.Test` | Unitario | Testes dos casos de uso da Application layer com mocks (Moq + Bogus). |
| `Validators.Tests` | Unitario | Testes dos validadores FluentValidation. |
| `WebApi.Test` | Integracao | Testes end-to-end com TestContainers (MySQL real efemero). |
| `CommomTestUtilities` | Utilitarios | Builders, mocks e fakes compartilhados. |

> Os testes de integracao utilizam **TestContainers** para criar e destruir um container MySQL real a cada execucao, garantindo fidelidade com o ambiente de producao.

---

## Superficie da API

| Controller | Endpoints | Descricao |
|:---|:---|:---|
| `Store` | 10 | CRUD de lojas, endereco, horarios, imagem, geocodificacao. |
| `Product` | 7 | CRUD de produtos, promocoes, imagem. |
| `Category` | 8 | CRUD de categorias, associacao a produtos, ordenacao. |
| `Cart` | 6 | Carrinho de compras (Redis), cupons, quantidades. |
| `Order` | 8 | Criacao, status, negociacao, mensagens, entrega. |
| `Payment` | 8 | Stripe (criacao, webhook, cartoes salvos, sincronizacao). |
| `Delivery` | 11 | Broadcast, aceitacao, rejeicao, pickup, transit, entrega, localizacao. |
| `Chat` | 3 | Conversas, mensagens, marcacao como lida. |
| `Reviews` | 3 | Criacao de avaliacao, reviews por loja e por pedido. |
| `Profile` | 2 | Perfil do usuario. |
| `UserAddress` | 4 | Enderecos do usuario com padrao. |
| `Coupon` | 3 | Criacao, listagem ativa e admin de cupons. |
| `Dashboard` | 1 | Metricas do lojista. |
| `StoreCategory` | 1 | Categorias de loja (pre-definidas). |
| `Upload` | 1 | Upload de imagens (Cloudinary). |

---

## Estrutura do Repositorio

```
IOrderNew/
├── src/
│   ├── Backend/
│   │   ├── IOrder.API              # Presentation Layer
│   │   ├── IOrder.Application      # Use Cases / CQRS
│   │   ├── IOrder.Domain           # Entities / Core
│   │   └── IOrder.infrastructure   # EF Core / Redis / Workers / Integracoes
│   ├── Shared/
│   │   ├── IOrder.Communication    # DTOs (Request/Response)
│   │   └── IOrder.Exceptions       # Custom Exceptions
│   └── Frontend/                   # Angular 20 SPA
│       └── src/app/
│           ├── core/               # Auth, Guards, Services, Stores, Models
│           ├── features/           # Home, Catalog, Cart, Orders, Admin, Courier, Profile
│           └── shared/             # Componentes reutilizaveis
├── tests/
│   ├── CommomTestUtilities         # Builders / Mocks / Fakes
│   ├── UseCases.Test               # Testes de unidade (casos de uso)
│   ├── Validators.Tests            # Testes de unidade (validators)
│   └── WebApi.Test                 # Testes de integracao (TestContainers)
├── scripts/
│   └── seed-fortaleza.sql          # Seed data (50 lojas Fortaleza)
├── docker-compose.yml              # 6 servicos (MySQL, Redis, RabbitMQ, Kafka, MailHog, Kafka UI)
├── azure-pipelines.yml             # CI/CD (build + testes + cobertura)
├── coverlet.runsettings            # Configuracao de cobertura de codigo
└── IOrderNew.slnx                  # Solution file
```

---

## Licenca

Projeto pessoal de portfolio. Todos os direitos reservados.
