# ApiRest

![CI](https://github.com/pabloquirino/ApiRest/actions/workflows/ci.yml/badge.svg)
![CD](https://github.com/pabloquirino/ApiRest/actions/workflows/cd.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green)

REST API escalável construída com **.NET 8**, **Clean Architecture** e **CQRS**,
com autenticação JWT + Refresh Token, deploy contínuo via GitHub Actions e Railway.

[→ Swagger UI (produção)](https://apirest-production-d659.up.railway.app/swagger)

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
| Documentação   | Swagger (via Swashbuckle)               |

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

## Deploy

O deploy é automático via GitHub Actions:
- **Push para `develop`** → roda CI (build + testes)
- **Merge para `main`** → roda CI + deploy automático na Railway

## Rodando localmente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)

### Setup
```bash
# 1. Clonar o repositório
git clone https://github.com/pabloquirino/ApiRest.git
cd ApiRest

# 2. Configurar variáveis de ambiente
Crie o arquivo `src/ApiRest.API/appsettings.Development.json`:
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=clone_db;Username=clone_user;Password=supersenha123"
  },
  "JwtSettings": {
    "SecretKey": "SUA-CHAVE-SECRETA-DE-32-CHARS-MINIMO",
    "Issuer": "ApiRest",
    "Audience": "ApiRest.Client",
    "ExpiryMinutes": 15,
    "RefreshExpiryDays": 7
  }
}

cp .env.example .env

# 3. Subir o banco de dados
docker compose up -d

# 4. Rodar restore do NuGet
dotnet restore

# 5. Aplicar migrations
dotnet ef database update --project src/ApiRest.Infrastructure --startup-project src/ApiRest.API

# 6. Rodar a API
dotnet run --project src/ApiRest.API
```
Acesse o Swagger em: `http://localhost:5277/swagger`

## Licença

MIT — veja [LICENSE](LICENSE) para detalhes.
