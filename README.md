# The Dish

A next-generation restaurant and hotel review platform with intelligent dietary preference adaptation, community-verified certifications, and comprehensive business tools.

## Architecture

- **Backend**: .NET Core 8 Microservices with Clean Architecture
- **Frontend**: Next.js 14 (Web) + Expo React Native (Mobile) + React Admin Panel
- **Database**: PostgreSQL 15 + PostGIS
- **Infrastructure**: AWS Cloud-Native with Kubernetes
- **Caching**: Redis 7
- **Search**: Elasticsearch 8
- **Messaging**: RabbitMQ 3

## Project Structure

```
the-dish-web/
├── backend/          # .NET Core microservices
│   ├── src/
│   │   ├── ApiGateway/
│   │   ├── Services/
│   │   │   ├── User/
│   │   │   ├── Place/
│   │   │   ├── Review/
│   │   │   ├── Dietary/
│   │   │   ├── Social/
│   │   │   └── Business/
│   │   └── Common/
│   └── tests/
├── web/              # Next.js 14 web app
├── mobile/           # Expo React Native
├── admin/            # React admin panel
├── docs/             # Documentation
├── infrastructure/   # Terraform/IaC
└── .github/          # CI/CD workflows
```

## Getting Started

### Prerequisites

- Docker Desktop
- .NET 8 SDK
- Node.js 18+ and npm/yarn
- Git

### Development Setup

1. Clone the repository:
```bash
git clone <repository-url>
cd the-dish-web
```

2. Start development services with Docker Compose:
```bash
docker-compose up -d
```

This will start:
- PostgreSQL 15 with PostGIS
- Redis 7
- Elasticsearch 8
- RabbitMQ 3
- pgAdmin (http://localhost:5050)
- Redis Commander (http://localhost:8081)

3. Set up backend services:
```bash
cd backend
dotnet restore
dotnet build
```

4. Set up web application:
```bash
cd web
npm install
npm run dev
```

5. Set up mobile application:
```bash
cd mobile
npm install
npx expo start
```

## Development Workflow

1. Create a feature branch from `develop`:
```bash
git checkout -b feature/your-feature-name
```

2. Make your changes and commit with semantic messages:
```bash
git commit -m "feat: add user authentication"
```

3. Push and create a pull request to `develop`

4. After review, squash merge to `develop`

## Branch Strategy

- `main` - Production-ready code
- `develop` - Integration branch for features
- `feature/*` - Feature development branches
- `hotfix/*` - Critical production fixes
- `release/*` - Release preparation branches

## Code Standards

- **Backend**: Follow Clean Architecture principles, 80%+ test coverage
- **Frontend**: TypeScript strict mode, ESLint + Prettier
- **API**: RESTful conventions, versioned endpoints (`/api/v1/`)
- **Git**: Semantic commit messages (feat, fix, docs, style, refactor, test, chore)

## Testing

Run all tests:
```bash
# Backend
cd backend
dotnet test

# Frontend
cd web
npm test

# Mobile
cd mobile
npm test
```

## Documentation

- [Business Logic Part 1](Docs/Business Logic Part 1.md)
- [Business Logic Part 2](Docs/Business Logic Part 2.md)
- [Technical Stack User Docs](Docs/Technical Stack User Docs.md)
- [Technical Stack Admin Docs](Docs/Technical Stack Admin Docs.md)
- [Production Plan](the-dish-production-plan-73f6e5.plan.md)

## Contributing

1. Read the development standards in the plan document
2. Follow the code quality guidelines
3. Write tests for new features
4. Update documentation as needed
5. Ensure all CI checks pass

## License

[To be determined]

