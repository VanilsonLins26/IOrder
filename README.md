# IOrder

**IOrder** é uma plataforma de encomendas personalizadas inspirada no modelo de grandes apps de delivery, mas com um diferencial: foco em **encomendas de todos os tipos** (comidas, roupas, itens personalizados, etc.) com fluxo de aprovação e negociação direta.

O usuário escolhe a loja ou busca por categoria, personaliza seus produtos, escolhe a data e hora desejada para a entrega/retirada e envia a solicitação. A partir desse momento, um **chat em tempo real** é aberto entre o cliente e o lojista. O lojista pode analisar, aprovar o pedido e, após a aprovação, o fluxo segue normalmente (pagamento, preparo, entrega/retirada).

---

## 🚧 Status do Projeto
**Em Desenvolvimento (Work in Progress)**

**O que já foi feito (Backend):**
- [x] Estruturação do projeto em **Clean Architecture** e conceitos de **DDD**.
- [x] Criação do `SeedWork` (Classes base de Domínio e Paginação).
- [x] Implementação do **CRUD de Produtos** com repositórios baseados em *Read/Write Segregation*.
- [x] Filtros dinâmicos, paginação e ordenação centralizados em uma única Query.
- [x] Tratamento global de exceções (Global Exception Filter) e padronização de retornos de erro (ex: `ErrorOnValidationException`, `NotFoundException`).
- [x] Implementação de **Background Service (Worker)** para sincronização assíncrona de preços promocionais, adotando padrões de *Read Model / Desnormalização*.
- [x] **Excelente cobertura de testes automatizados**, abrangendo tanto **Testes de Unidade** focados nas regras de negócio (Casos de Uso e Validações), quanto **Testes de Integração** avançados que utilizam **TestContainers** para validar o fluxo completo contra um banco de dados real rodando em Docker.
- [ ] Módulo de Lojas e Categorias.
- [ ] Fluxo de Pedidos e Carrinho.
- [ ] Módulo de Chat via SignalR.
- [ ] Integração com Mercado Pago e EvolutionAPI.

---

## 🛠️ Tecnologias e Arquitetura Planejada

### 1. Frontend (Interface do Usuário)
* **Tecnologia:** Angular
* **Papel:** Consumir as APIs, fornecer uma interface fluida para o cliente final e um painel administrativo para os lojistas.

### 2. Backend (Core Business)
* **Tecnologia:** ASP.NET Core Web API (C#)
* **Arquitetura:** Clean Architecture + Domain-Driven Design (DDD). Isolamento rigoroso de regras de negócio das integrações de infraestrutura.
* **Qualidade de Código:** Testes de Unidade e **Testes de Integração** com **xUnit**, **Shouldly** e **Bogus** (para geração de dados fakes estruturados). Validações robustas via FluentValidation.
* **Processamento Assíncrono:** Uso de `BackgroundService` (Workers) nativos para executar tarefas em segundo plano (como varredura e ativação de promoções em tempo real) sem penalizar a performance das buscas na API.
* **CI/CD:** Pipelines automatizados no GitHub/DevOps para execução de testes e cálculo de Cobertura de Código em cada Push.

### 3. Autenticação e Segurança
* **Tecnologia:** Auth0 (OAuth 2.0 / JWT)
* **Papel:** Gerenciar login, cadastro e perfis de acesso (Cliente, Lojista, Entregador), garantindo segurança via tokens JWT.

### 4. Bancos de Dados (Persistência Poliglota)
* **Relacional (MySQL via Docker):** Responsável pelo core do negócio (Lojas, Produtos, Pedidos, Transações financeiras) garantindo consistência ACID.
* **NoSQL (MongoDB):** Exclusivo para o módulo de Chat, garantindo alta performance para leitura/escrita massiva de mensagens desestruturadas sem onerar o banco principal.

### 5. Comunicação em Tempo Real
* **Tecnologia:** SignalR (WebSockets)
* **Papel:** Manter um túnel de comunicação bidirecional para o **Chat**. Notifica o cliente em tempo real quando o lojista digita ou responde.

### 6. Mensageria e Eventos (Assincronismo)
* **RabbitMQ (Fila de Comandos):** Para processamento resiliente (com retry automático). Exemplo: Processamento de Webhooks do Mercado Pago e disparo de notificações no WhatsApp via Workers, evitando prender a requisição principal do usuário.
* **Apache Kafka (Log de Eventos - Opcional):** Registro do histórico completo do ciclo de vida da encomenda (Criado -> Aprovado -> Preparando), abrindo portas para microsserviços paralelos de auditoria ou analytics.

### 7. Integrações Externas
* **Mercado Pago:** Pagamentos integrados (geração de Pix/Cartão) e recebimento de Webhooks de confirmação de pagamento.
* **EvolutionAPI:** Automação de WhatsApp consumida via Workers (RabbitMQ) para notificar clientes sobre o andamento do pedido caso não estejam online no app.

---

## 🚀 Como Executar o Backend (Desenvolvimento)

1. Clone este repositório.
2. Certifique-se de ter o [.NET SDK](https://dotnet.microsoft.com/download) instalado.
3. Navegue até a pasta do projeto `API`:
   ```bash
   cd src/Backend/IOrder.API
   ```
4. Execute o projeto:
   ```bash
   dotnet run
   ```
5. Acesse o **Swagger** pelo navegador na porta indicada no terminal (ex: `https://localhost:7023/swagger`) para testar os endpoints disponíveis.

---

*Projeto pessoal desenvolvido com foco em portfólio, boas práticas de engenharia de software, arquitetura limpa e tecnologias escaláveis, visando deploy em ambiente de produção real.*
