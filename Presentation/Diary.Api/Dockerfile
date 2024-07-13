FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/Diary.Api/Diary.Api.csproj", "Presentation/Diary.Api/"]
COPY ["Core/Diary.Domain/Diary.Domain.csproj", "Core/Diary.Domain/"]
COPY ["Infrastructure/Diary.DAL/Diary.DAL.csproj", "Infrastructure/Diary.DAL/"]
COPY ["Core/Diary.Application/Diary.Application.csproj", "Core/Diary.Application/"]
COPY ["Infrastructure/Diary.Producer/Diary.Producer.csproj", "Infrastructure/Diary.Producer/"]
COPY ["Infrastructure/Diary.Consumer/Diary.Consumer.csproj", "Infrastructure/Diary.Consumer/"]
RUN dotnet restore "Presentation/Diary.Api/Diary.Api.csproj"
COPY . .
WORKDIR "/src/Presentation/Diary.Api"
RUN dotnet build "Diary.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Diary.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Diary.Api.dll"]
