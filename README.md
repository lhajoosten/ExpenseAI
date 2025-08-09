# ExpenseAI 🧾✨

> AI-powered financial insights that understand your business

[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-20-red)](https://angular.io/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.9.1-blue)](https://www.typescriptlang.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Azure](https://img.shields.io/badge/Azure-Ready-0078d4)](https://azure.microsoft.com/)

ExpenseAI revolutionizes expense management through intelligent automation, natural language processing, and AI-powered business insights. Built with modern Clean Architecture principles and cutting-edge AI integration.

## 🚀 Features

### 🤖 AI-Powered Intelligence
- **Smart Document Processing**: OCR + LLM analysis for automatic receipt/invoice data extraction
- **Intelligent Categorization**: AI learns from your behavior to auto-categorize expenses
- **Natural Language Queries**: Ask questions like "Show me all marketing expenses over $500 last quarter"
- **Conversational Analytics**: Chat with your financial data to discover insights
- **Fraud Detection**: AI identifies suspicious patterns and duplicate expenses

### 💼 Business Management
- **Invoice Generation**: Create professional invoices from simple descriptions
- **Multi-Currency Support**: Handle international transactions seamlessly  
- **Tax Optimization**: AI-powered deduction suggestions and compliance checks
- **Cash Flow Forecasting**: Predictive analytics for financial planning
- **Client Portal**: Secure client access for invoice viewing and payments

### 📊 Advanced Analytics
- **Real-time Dashboards**: Interactive financial visualizations
- **Trend Analysis**: Identify spending patterns and budget variances
- **Custom Reporting**: Generate detailed reports with AI-powered insights
- **Budget Tracking**: Smart alerts and recommendations for budget management
- **ROI Analysis**: Track return on investment for business expenses

## 🏗️ Architecture

ExpenseAI follows Clean Architecture principles with Domain-Driven Design patterns:

```
src/
├── backend/
│   ├── ExpenseAI.Api/              # Web API & Controllers
│   ├── ExpenseAI.Application/      # Business Logic & CQRS
│   │   ├── Commands/               # Write operations
│   │   ├── Queries/                # Read operations
│   │   ├── Services/               # Application services
│   │   └── Interfaces/             # Application contracts
│   ├── ExpenseAI.Domain/           # Domain Models & Business Rules
│   │   ├── Entities/               # Domain entities
│   │   ├── ValueObjects/           # Domain value objects
│   │   ├── Aggregates/             # Domain aggregates
│   │   └── Specifications/         # Business rules
│   └── ExpenseAI.Infrastructure/   # External Concerns
│       ├── Data/                   # Entity Framework Core
│       ├── AI/                     # LLM integrations
│       ├── Storage/                # File storage
│       └── External/               # Third-party APIs
├── frontend/
│   ├── src/app/
│   │   ├── core/                   # Singleton services
│   │   ├── shared/                 # Reusable components
│   │   ├── features/               # Feature modules
│   │   │   ├── expenses/           # Expense management
│   │   │   ├── invoices/           # Invoice generation
│   │   │   ├── analytics/          # AI-powered insights
│   │   │   └── ai-chat/            # Conversational interface
│   │   └── layout/                 # Application shell
└── shared/
    └── ExpenseAI.Contracts/        # Shared DTOs & contracts
```

## 💻 Technology Stack

### Backend (.NET 9)
- **Framework**: ASP.NET Core 8 Web API
- **ORM**: Entity Framework Core with SQL Server
- **Patterns**: CQRS with MediatR, Repository & Specification patterns
- **Authentication**: ASP.NET Core Identity with JWT tokens
- **Validation**: FluentValidation for business rules
- **Mapping**: AutoMapper for object transformations
- **Caching**: Redis for distributed caching
- **Logging**: Serilog with structured logging
- **Testing**: xUnit, Moq, TestContainers for integration tests

### Frontend (Angular 20)
- **Framework**: Angular 20 with TypeScript 5.9.1
- **State Management**: NgRx for complex state scenarios
- **UI Components**: Angular Material + custom design system
- **Forms**: Reactive Forms with custom validators
- **Charts**: Chart.js for data visualizations
- **HTTP**: Interceptors for authentication and error handling
- **PWA**: Service workers for offline capabilities
- **Testing**: Jasmine, Karma, Cypress for E2E testing

### AI & Machine Learning
- **LLM Integration**: OpenAI GPT-4, Anthropic Claude
- **Document Processing**: Azure Cognitive Services OCR
- **ML Models**: ML.NET for predictive analytics
- **Vector Storage**: Azure Cognitive Search for semantic search
- **Prompt Management**: Custom prompt engineering framework

### Infrastructure & DevOps
- **Cloud Platform**: Microsoft Azure
- **Containerization**: Docker with multi-stage builds
- **Orchestration**: Azure Container Instances / Kubernetes
- **Database**: Azure SQL Database with automated backups
- **Storage**: Azure Blob Storage for documents
- **CDN**: Azure CDN for static assets
- **Monitoring**: Azure Application Insights
- **CI/CD**: GitHub Actions with Azure DevOps

## 🛠️ Getting Started

### Prerequisites
- [.NET88 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)

### Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/expense-ai-platform.git
   cd expense-ai-platform
   ```

2. **Backend Setup**
   ```bash
   cd src/backend
   dotnet restore
   dotnet build
   
   # Setup database
   dotnet ef database update --project ExpenseAI.Infrastructure
   
   # Run the API
   dotnet run --project ExpenseAI.Api
   ```

3. **Frontend Setup**
   ```bash
   cd src/frontend
   npm install
   ng serve
   ```

4. **Docker Development**
   ```bash
   # Run full stack with Docker Compose
   docker-compose -f docker/docker-compose.dev.yml up
   ```

### Environment Configuration

Create `appsettings.Development.json` in the API project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ExpenseAI;Trusted_Connection=true;MultipleActiveResultSets=true",
    "Redis": "localhost:6379"
  },
  "AI": {
    "OpenAI": {
      "ApiKey": "your-openai-key",
      "Model": "gpt-4-turbo"
    },
    "Azure": {
      "CognitiveServicesKey": "your-cognitive-key",
      "CognitiveServicesEndpoint": "your-endpoint"
    }
  },
  "JWT": {
    "Key": "your-super-secret-key-that-is-long-enough",
    "Issuer": "ExpenseAI",
    "Audience": "ExpenseAI-Users"
  }
}
```

## 🧪 Testing

### Running Tests
```bash
# Backend tests
cd src/backend
dotnet test

# Frontend tests
cd src/frontend
npm test

# E2E tests
npm run e2e

# Integration tests with TestContainers
dotnet test --filter "Category=Integration"
```

### Test Coverage
- **Backend**: xUnit with comprehensive unit and integration tests
- **Frontend**: Jasmine/Karma with 90%+ coverage target
- **E2E**: Cypress for critical user journeys
- **AI Components**: Dedicated tests for prompt engineering and AI integrations

## 📦 Deployment

### Azure Deployment

1. **Infrastructure as Code**
   ```bash
   # Deploy Azure resources
   az deployment group create \
     --resource-group expense-ai-rg \
     --template-file azure/main.bicep
   ```

2. **Container Deployment**
   ```bash
   # Build and push containers
   docker build -t expenseai-api:latest -f docker/Dockerfile.api .
   docker build -t expenseai-web:latest -f docker/Dockerfile.web .
   
   # Deploy to Azure Container Instances
   az container create --resource-group expense-ai-rg \
     --name expenseai-api --image expenseai-api:latest
   ```

3. **GitHub Actions CI/CD**
   - Automated testing on PR
   - Container builds and pushes
   - Blue-green deployments to Azure

### Docker Production
```bash
# Production build
docker-compose -f docker/docker-compose.prod.yml up -d
```

## 🔐 Security

- **Authentication**: JWT tokens with refresh mechanism
- **Authorization**: Role-based access control (RBAC)
- **Data Protection**: Encryption at rest and in transit
- **Input Validation**: Comprehensive validation at all layers
- **SQL Injection Prevention**: Parameterized queries only
- **XSS Protection**: Content Security Policy and sanitization
- **Rate Limiting**: API throttling and DDoS protection
- **Audit Logging**: Complete audit trail for financial data

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow our coding standards (see `.editorconfig`)
4. Write tests for your changes
5. Run the test suite (`npm test` and `dotnet test`)
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

### Development Guidelines
- Follow Clean Architecture principles
- Use SOLID design principles
- Write meaningful tests
- Document public APIs
- Use conventional commits
- Follow our code style (PascalCase for C#, camelCase for TypeScript)

## 📋 Roadmap

### Version 1.0 - Core Platform (Q1 2026)
- [x] Basic expense tracking and categorization
- [x] AI-powered document processing
- [x] Simple invoice generation
- [ ] Multi-user support with role management
- [ ] Basic reporting and analytics

### Version 1.1 - AI Enhancement (Q2 2026)
- [ ] Natural language query interface
- [ ] Conversational analytics chatbot
- [ ] Advanced fraud detection
- [ ] Predictive cash flow modeling
- [ ] Smart tax optimization suggestions

### Version 2.0 - Enterprise Features (Q4 2026)
- [ ] Multi-company support
- [ ] Advanced workflow automation
- [ ] API for third-party integrations
- [ ] White-label customization
- [ ] Advanced compliance features

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙋‍♂️ Support

- 📧 Email: support@expenseai.com
- 💬 Discussions: [GitHub Discussions](https://github.com/lhajoosten/expense-ai-platform/discussions)
- 🐛 Issues: [GitHub Issues](https://github.com/lhajoosten/expense-ai-platform/issues)
- 📖 Documentation: [Wiki](https://github.com/lhajoosten/expense-ai-platform/wiki)

## 🌟 Acknowledgments

- OpenAI and Anthropic for LLM capabilities
- Microsoft Azure for cloud infrastructure
- The .NET and Angular communities
- Contributors and early adopters

---

**Made with ❤️ by the ExpenseAI Team**
