FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ExamSystem/ExamSystem.csproj", "ExamSystem/"]
RUN dotnet restore "ExamSystem/ExamSystem.csproj"
COPY . .
WORKDIR "/src/ExamSystem"
RUN dotnet build "ExamSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExamSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExamSystem.dll"]
