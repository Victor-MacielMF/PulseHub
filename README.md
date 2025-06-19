# PulseHub – Plataforma de Sincronização de Estoques Multicanal

🚀 Estrutura – Clean Architecture + DDD

---

## 🧠 Visão Geral

O PulseHub é uma plataforma backend desenvolvida com o objetivo de simular e resolver o desafio real de sincronização de cadastros, produtos e estoques entre múltiplos canais de venda e marketplaces.

Este projeto foi idealizado para demonstrar domínio técnico nas principais tecnologias do mercado, além da capacidade de entregar soluções escaláveis, robustas e bem arquitetadas, utilizando práticas modernas como Clean Architecture, SOLID e Domain-Driven Design (DDD).

---

## 🎯 O que este projeto demonstra

- ✔️ Backend robusto com **.NET 5 / C#**
- ✔️ Banco de dados relacional com **SQL Server**, modelado e otimizado
- ✔️ Mensageria assíncrona via **RabbitMQ** (simulando AWS SQS)
- ✔️ API RESTful limpa, bem estruturada e documentada com **Swagger**
- ✔️ Frontend com **Angular + Angular Material** (painel administrativo)
- ✔️ Arquitetura modular com **Clean Architecture + SOLID + DDD**
- ✔️ Observabilidade: logs estruturados (**Serilog**), health checks e monitoramento
- ✔️ Desacoplamento total entre camadas, seguindo práticas de microserviços
- ✔️ Padrões de design aplicados: **Repository, Unit of Work, Mediator, Factory, Strategy**
- ✔️ GitFlow aplicado no versionamento
- ✔️ Testes de integração implementados para repositórios, garantindo qualidade e segurança na persistência dos dados
- ✔️ Testes unitários implementados para os serviços da camada Application

---

## 🚀 Funcionalidades do MVP

- 🛍️ Cadastro de produtos: nome, descrição, preço e estoque
- 📦 Controle de estoque: ajuste de quantidades e visualização do saldo
- 🔄 Sincronização automática: cada alteração gera eventos simulando integração com marketplaces
- 🔗 API RESTful: pronta para consumo externo
- 📤 Fila de mensagens: atualizações processadas de forma assíncrona via RabbitMQ
- 🔍 Monitoramento: health checks, logs estruturados e feedback visual via frontend
- 🖥️ Painel administrativo (Angular): CRUD de produtos, gestão de estoque e visualização dos eventos de sincronização

---

## 📂 Estrutura do Projeto


```
/PulseHub
├── PulseHub.API → Camada de apresentação (Controllers, Startup, Program)
├── PulseHub.Application → Camada de aplicação (Services, DTOs, Mappings, Interfaces)
│ ├── DTOs → Definição dos contratos de dados (Request e Response)
│ ├── Mappings
│ │ ├── Extensions → Mapeamentos manuais entre DTOs e Entidades
│ │ └── Profiles → Configurações do AutoMapper para mapeamento automático
│ └── Services
│ ├── Interfaces → Contratos dos serviços
│ └── Implementations → Implementação das regras de negócio
├── PulseHub.Domain → Camada de domínio (Entities, Aggregates, Interfaces dos Repositórios)
├── PulseHub.Infrastructure → Camada de infraestrutura (EF Core, Repositórios, Acesso a Dados)
├── PulseHub.Infrastructure.Tests → Testes de integração dos repositórios
├── PulseHub.Application.Tests → Testes unitários da camada Application
│ ├── Services
│ ├── Mocks
│ └── TestHelpers
├── docs → Diagramas de Arquitetura e Modelagem de Entidades
└── PulseHub.sln → Arquivo da solução

```

---

## 🔀 Mapeamento (DTO ↔️ Entidades)

O projeto adota duas abordagens para mapear os dados entre as camadas de API e domínio:

- ✔️ **Mapeamento Manual (Extensions):**  
Utilizado nos casos em que é necessário ter mais controle sobre a transformação dos dados, especialmente nas operações de entrada (**Request**).

- ✔️ **Mapeamento Automático (AutoMapper - Profiles):**  
Aplicado principalmente na saída (**Response**), onde o mapeamento é direto e não exige transformações complexas.

### 📁 Estrutura dos mapeamentos:

```
PulseHub.Application
└── Mappings
├── Extensions → Métodos de mapeamento manual (ex.: ProductMappingExtensions.cs)
└── Profiles → Configuração do AutoMapper (ex.: ProductProfile.cs)
```

Essa combinação permite o equilíbrio entre controle e produtividade, onde demonstro domínio sobre ambas as abordagens, aplicando a melhor solução para cada contexto.

## 🔧 Como executar as Migrations (Entity Framework)

Para criar o banco de dados e aplicar a estrutura definida no projeto, execute os comandos abaixo utilizando o Entity Framework Core CLI.

### ✔️ Passo 1 – Gerar uma Migration (se necessário)

Acesse a pasta `PulseHub.Infrastructure` e execute o seguinte comando:
```
dotnet ef migrations add NomeDaMigration --startup-project ../PulseHub.API
```
Observação: Esse comando cria uma nova migration. Caso você já tenha a migration chamada `InitialCreate`, não precisa executar esse comando novamente.

---

### ✔️ Passo 2 – Aplicar as Migrations no Banco de Dados

Ainda dentro da pasta `PulseHub.Infrastructure`, execute:

```
dotnet ef database update --startup-project ../PulseHub.API
```
Esse comando cria o banco de dados e aplica toda a estrutura de tabelas, constraints e relacionamentos automaticamente.

---

### ✔️ Observações importantes

- O parâmetro `--startup-project ../PulseHub.API` é necessário porque o projeto PulseHub.API contém as configurações de ambiente, como a connection string no arquivo `appsettings.json`. Porém, quem fornece o DbContext para as migrations em tempo de desenvolvimento (design-time) é a classe `PulseHubDbContextFactory`, localizada no projeto PulseHub.Infrastructure.

---

### ✔️ Pré-requisitos para rodar as migrations

- Ter o SQL Server instalado e configurado localmente ou na nuvem.
- A connection string deve estar corretamente configurada no arquivo `appsettings.json` dentro do projeto `PulseHub.API`.

Exemplo de connection string no arquivo `appsettings.json`:

```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=PulseHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```


---

## 🧪 Testes de Integração

O projeto conta com testes de integração dos repositórios, garantindo que as operações de persistência estejam funcionando corretamente.

### ✔️ Executando os testes de integração

Acesse a raiz do projeto de testes:
```
cd PulseHub.Infrastructure.Tests
```

Execute os testes:
```
dotnet test
```

### ✔️ O que é testado:

- Operações de CRUD dos repositórios:
  - **ProductRepository**
  - **SyncEventRepository**
  - **QueueMessageRepository**
- Cada teste cria um banco isolado em memória.
- Usa FluentAssertions para validações legíveis.
- Garante que cada repositório funciona corretamente antes de avançar para outras camadas.

---
## 🧪 Testes de Integração

O projeto conta com testes de integração dos repositórios, garantindo que as operações de persistência estejam funcionando corretamente.

### ✔️ Estrutura dos testes de integração:
```
PulseHub.Infrastructure.Tests
├── Services → Testes dos repositórios
├── TestHelpers → Builders, dados fake, utilitários
└── Mocks → (opcional) mocks auxiliares
```

### ✔️ Executando os testes de integração

Acesse a raiz do projeto de testes:

```
cd PulseHub.Infrastructure.Tests
```

Execute os testes:

```
dotnet test
```

---

## 🏗️ Diagrama de Arquitetura

![Diagrama de Arquitetura](./docs/architecture-diagram.png)

---

## 🗃️ Modelagem das Entidades

![Modelagem das Entidades](./docs/entities-model.png)

---

## 🛠️ Tecnologias e Ferramentas

- **Backend:** .NET 5, ASP.NET Web API, Entity Framework, MediatR, Serilog
- **Banco de Dados:** SQL Server
- **Mensageria:** RabbitMQ (simulando AWS SQS)
- **Frontend:** Angular + Angular Material + RxJS
- **Versionamento:** Git + GitFlow
- **Documentação:** Swagger (OpenAPI)
- **Testes:** xUnit, FluentAssertions, Moq,EF InMemory

---

## 🚧 Observações

- Este repositório está em desenvolvimento ativo.
- O PulseHub foi desenvolvido para demonstrar domínio em arquitetura de software, backend em .NET, mensageria, APIs RESTful e integração com frontend Angular.
- Projeto idealizado para resolver desafios de integração, sincronização de cadastros, produtos e estoques entre múltiplos canais de venda e marketplaces.