FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["TheDish.AI.ReviewAnalysis.API/TheDish.AI.ReviewAnalysis.API.csproj", "TheDish.AI.ReviewAnalysis.API/"]
COPY ["TheDish.AI.ReviewAnalysis.Application/TheDish.AI.ReviewAnalysis.Application.csproj", "TheDish.AI.ReviewAnalysis.Application/"]
COPY ["TheDish.AI.ReviewAnalysis.Infrastructure/TheDish.AI.ReviewAnalysis.Infrastructure.csproj", "TheDish.AI.ReviewAnalysis.Infrastructure/"]
COPY ["TheDish.AI.ReviewAnalysis.Domain/TheDish.AI.ReviewAnalysis.Domain.csproj", "TheDish.AI.ReviewAnalysis.Domain/"]

RUN dotnet restore "TheDish.AI.ReviewAnalysis.API/TheDish.AI.ReviewAnalysis.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/TheDish.AI.ReviewAnalysis.API"
RUN dotnet build "TheDish.AI.ReviewAnalysis.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TheDish.AI.ReviewAnalysis.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheDish.AI.ReviewAnalysis.API.dll"]




