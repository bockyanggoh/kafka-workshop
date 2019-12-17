FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app
COPY ./ .
RUN dotnet restore KafkaPublisherContainer.sln
RUN dotnet build */KafkaPublisherContainer.csproj
RUN dotnet publish  -c Release -o /app/KafkaPublisherContainer/out
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ENV ASPNETCORE_ENVIRONMENT="Testing"
WORKDIR /app
COPY --from=build /app/KafkaPublisherContainer/out .
ENTRYPOINT ["dotnet", "KafkaPublisherContainer.dll"]
