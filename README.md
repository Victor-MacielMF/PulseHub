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
- ✔️ Respostas padronizadas para toda API usando `ApiResponse`
- ✔️ Middleware global para tratamento de exceções
- ✔️ Testes de integração implementados para repositórios
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

## 📬 Infraestrutura de Mensageria – RabbitMQ (Simulando Amazon SQS)

O projeto PulseHub implementa uma arquitetura orientada a eventos utilizando mensageria assíncrona com **RabbitMQ**, simulando o comportamento do **Amazon SQS**.

---

### 🚀 Como funciona?

- Cada vez que um produto é criado, atualizado ou excluído, um **evento de sincronização** é publicado em uma fila chamada `sync-events-queue`.
- Esse evento representa uma tentativa de sincronizar aquela ação com um marketplace externo (no MVP, futuramente Mercado Livre, etc.).
- O RabbitMQ armazena essas mensagens na fila de forma **durável e persistente**, garantindo que nenhuma atualização seja perdida mesmo que haja falhas na API.
- Um serviço separado (**Consumer Service**) é responsável por consumir as mensagens dessa fila e processá-las.

---

### 🏗️ Estrutura da Mensageria

- ✔️ **Fila:** `sync-events-queue`  
→ Responsável por armazenar todos os eventos de sincronização de produtos.

- ✔️ **Publisher:**  
→ Implementado na própria API. Toda vez que um produto é alterado (**Create, Update, Delete**), a API publica uma mensagem nessa fila.

- ✔️ **Consumer:**  
→ Serviço separado, desenvolvido em .NET, que escuta essa fila, consome os eventos e executa ações como simular integrações com marketplaces ou atualizar status no banco.

---

### 🎯 Por que RabbitMQ?

- O RabbitMQ está sendo utilizado para simular um cenário real que, em produção, poderia ser facilmente migrado para serviços como **AWS SQS**, **Azure Service Bus**, **Google Pub/Sub**, entre outros.
- A decisão de utilizar o RabbitMQ local tem como objetivo simplificar a configuração, acelerar o desenvolvimento local e reduzir custos de infraestrutura.

---

### 🔧 Configuração da Fila

- **Nome da fila:** `sync-events-queue`
- **Durabilidade:** ✔️ **Durable** (Persistente)
- **Auto Delete:** ❌ Desabilitado
- **Exclusive:** ❌ Desabilitado

---

### 🌐 Acesso ao Painel do RabbitMQ

Após instalar e habilitar o plugin de gerenciamento, você pode acessar o painel administrativo via navegador:


```
http://localhost:15672
```


- **Usuário:** guest  
- **Senha:** guest  

---

### ✅ Funcionamento Resumido do Fluxo


```
	[ API (.NET) ]
		↓ (Publica eventos)
	[ RabbitMQ (sync-events-queue) ]
		↓ (Consumer escuta)
	[ Consumer (.NET) ]
		↓ (Processamento)
	[ SQL Server + Simulação Marketplace ]
```

---

Essa abordagem desacopla os processos de escrita e leitura, melhora a escalabilidade, permite resiliência em caso de falhas e simula um ambiente realista de microsserviços, adotando uma estratégia robusta de mensageria assíncrona.

---

## 📂 Estrutura do Projeto

```
/PulseHub
├── PulseHub.API → Camada de apresentação (Controllers, Middlewares, Startup, Program)
│ ├── Controllers → Endpoints da API
│ ├── Middlewares → Tratamento global de exceções
├── PulseHub.Application → Camada de aplicação (Services, DTOs, Mappings, Interfaces)
│ ├── DTOs → Definição dos contratos de dados (Request e Response + ApiResponse)
│ ├── Mappings
│ │ ├── Extensions → Mapeamentos manuais entre DTOs e Entidades
│ │ └── Profiles → Configurações do AutoMapper para mapeamento automático
│ └── Services
│ ├── Interfaces → Contratos dos serviços
│ └── Implementations → Implementação das regras de negócio
├── PulseHub.Domain → Camada de domínio (Entities, Aggregates, Interfaces dos Repositórios)
├── PulseHub.Infrastructure → Camada de infraestrutura (EF Core, Repositórios, UnitOfWork, Acesso a Dados)
├── PulseHub.Infrastructure.Tests → Testes de integração dos repositórios
├── PulseHub.Application.Tests → Testes unitários da camada Application
│ ├── Services → Testes dos serviços
│ └── TestHelpers → Builders, dados fake, utilitários
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
└─────── Mappings
├── Extensions → Métodos de mapeamento manual (ex.: ProductMappingExtensions.cs)
└── Profiles → Configuração do AutoMapper (ex.: ProductProfile.cs)
```


Essa combinação permite o equilíbrio entre controle e produtividade, onde demonstro domínio sobre ambas as abordagens, aplicando a melhor solução para cada contexto.

---

## 🔧 Como executar as Migrations (Entity Framework)

Para criar o banco de dados e aplicar a estrutura definida no projeto, execute os comandos abaixo utilizando o Entity Framework Core CLI.

### ✔️ Passo 1 – Gerar uma Migration (se necessário)

Acesse a pasta `PulseHub.Infrastructure` e execute o seguinte comando:

```
dotnet ef migrations add NomeDaMigration --startup-project ../PulseHub.API
```


> Observação: Esse comando cria uma nova migration. Caso você já tenha a migration chamada `InitialCreate`, não precisa executar esse comando novamente.

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

## 🧪 Testes Unitários da Camada Application

O projeto conta com testes unitários para os serviços da camada de Application, garantindo que as regras de negócio estejam funcionando corretamente.

### ✔️ Estrutura dos testes unitários:

```
PulseHub.Application.Tests
├── Services → Testes dos serviços
└── TestHelpers → Builders, dados fake, utilitários
```


### ✔️ Executando os testes

Acesse a raiz do projeto de testes:

```
cd PulseHub.Application.Tests
```


Execute os testes:

```
dotnet test
```


### ✔️ O que é testado:

- Operações dos serviços da camada Application:
  - **ProductService**
  - **SyncEventService**
  - **QueueMessageService**
- Verificações como:
  - Busca por ID
  - Listagem de registros
  - Exclusão
  - Comportamento esperado quando não encontrar registros
- Uso de **Moq** para mocks de repositórios e unit of work
- **FluentAssertions** para garantir clareza nos asserts
- Builders criados para dados consistentes nos testes

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
- **Testes:** xUnit, FluentAssertions, Moq, EF InMemory

---

## 🚧 Observações

- Este repositório está em desenvolvimento ativo.
- O PulseHub foi desenvolvido para demonstrar domínio em arquitetura de software, backend em .NET, mensageria, APIs RESTful e integração com frontend Angular.
- Projeto idealizado para resolver desafios de integração, sincronização de cadastros, produtos e estoques entre múltiplos canais de venda e marketplaces.
