# ApiRest

![CI](https://github.com/pabloquirino/ApiRest/actions/workflows/ci.yml/badge.svg)
![CD](https://github.com/pabloquirino/ApiRest/actions/workflows/cd.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green)

REST API escalável construída com **.NET 8**, **Clean Architecture** e **CQRS**,
com autenticação JWT + Refresh Token, deploy contínuo via GitHub Actions e Railway.

**[→ Swagger UI (produção)](https://apirest-production-d659.up.railway.app/swagger)**

---

## Funcionalidades

- Autenticação completa: Register, Login, Refresh Token, Revoke
- CRUD de produtos com controle de estoque
- Gestão de pedidos com ciclo de vida (Pending → Confirmed → Cancelled)
- Autorização por roles: Customer e Admin
- Validação automática via FluentValidation + MediatR Pipeline Behavior
- Respostas de erro padronizadas (middleware global)
- Health check endpoint em `/health`

## Stack

| Camada         | Tecnologia                              |
|----------------|-----------------------------------------|
| Framework      | ASP.NET Core 8                          |
| ORM            | Entity Framework Core 8 + PostgreSQL    |
| Auth           | JWT Bearer + Refresh Token (rotation)   |
| CQRS           | MediatR                                 |
| Validação      | FluentValidation                        |
| Testes         | xUnit + Moq + FluentAssertions          |
| CI/CD          | GitHub Actions + Railway                |
| Documentação   | Swagger / Swashbuckle                   |

## Arquitetura

O projeto segue **Clean Architecture** com 4 camadas isoladas:

```
src/
├── ApiRest.Domain/          # Entidades, regras de negócio, interfaces
├── ApiRest.Application/     # CQRS (Commands/Queries/Handlers), DTOs, Validators
├── ApiRest.Infrastructure/  # EF Core, JWT, repositórios concretos
└── ApiRest.API/             # Controllers, Middlewares, Program.cs
```

A direção das dependências nunca aponta para fora:
`API → Application → Domain ← Infrastructure`

## Endpoints

### Auth
| Método | Rota                  | Auth     | Descrição               |
|--------|-----------------------|----------|-------------------------|
| POST   | `/api/auth/register`  | Público  | Criar conta             |
| POST   | `/api/auth/login`     | Público  | Login → tokens          |
| POST   | `/api/auth/refresh`   | Público  | Renovar access token    |
| POST   | `/api/auth/revoke`    | Público  | Revogar refresh token   |

### Products
| Método | Rota                   | Auth     | Descrição               |
|--------|------------------------|----------|-------------------------|
| GET    | `/api/products`        | Público  | Listar produtos ativos  |
| GET    | `/api/products/{id}`   | Público  | Buscar produto          |
| POST   | `/api/products`        | Admin    | Criar produto           |
| PUT    | `/api/products/{id}`   | Admin    | Atualizar produto       |
| DELETE | `/api/products/{id}`   | Admin    | Desativar produto       |

### Orders
| Método | Rota                            | Auth      | Descrição            |
|--------|---------------------------------|-----------|----------------------|
| GET    | `/api/orders`                   | Customer  | Meus pedidos         |
| GET    | `/api/orders/{id}`              | Customer  | Buscar pedido        |
| POST   | `/api/orders`                   | Customer  | Criar pedido         |
| POST   | `/api/orders/{id}/confirm`      | Customer  | Confirmar pedido     |
| POST   | `/api/orders/{id}/cancel`       | Customer  | Cancelar pedido      |

## Rodando localmente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)

### Setup

```bash
# 1. Clonar o repositório
git clone https://github.com/SEU_USUARIO/ApiRest.git
cd ApiRest

# 2. Subir o banco de dados
docker compose up -d

# 3. Aplicar migrations
dotnet ef database update \
  --project src/ApiRest.Infrastructure \
  --startup-project src/ApiRest.API

# 4. Rodar a API
dotnet run --project src/ApiRest.API
```

Acesse o Swagger em: `https://localhost:5001/swagger`

### Variáveis de ambiente

Crie o arquivo `src/ApiRest.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=portfolio_db;Username=portfolio_user;Password=supersenha123"
  },
  "JwtSettings": {
    "SecretKey": "SUA-CHAVE-SECRETA-DE-32-CHARS-MINIMO",
    "Issuer": "ApiRest",
    "Audience": "ApiRest.Client",
    "ExpiryMinutes": 15,
    "RefreshExpiryDays": 7
  }
}
```

## Testes

```bash
# Todos os testes
dotnet test

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

A suíte cobre 3 camadas:
- **Domain.Tests** — regras de negócio puras (sem dependências)
- **Application.Tests** — handlers com repositórios mockados (Moq)
- **Integration.Tests** — pipeline HTTP completo com banco InMemory

## Deploy

O deploy é automático via GitHub Actions:
- **Push para `develop`** → roda CI (build + testes)
- **Merge para `main`** → roda CI + deploy automático na Railway

## Licença

MIT — veja [LICENSE](LICENSE) para detalhes.
