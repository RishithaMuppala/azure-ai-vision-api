FROM mcr.microsoft.com/dotnet/aspnet:6.0.1-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src/IntelligentDocumentAnalysisAPI

COPY ["IntelligentDocumentAnalysisAPI.csproj", "./"]

RUN dotnet restore "IntelligentDocumentAnalysisAPI.csproj"
COPY . .

# WORKDIR "/src/IntelligentDocumentAnalysisAPI"
RUN dotnet build "IntelligentDocumentAnalysisAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IntelligentDocumentAnalysisAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntelligentDocumentAnalysisAPI.dll"]