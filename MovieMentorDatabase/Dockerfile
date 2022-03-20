FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MovieMentorDatabase/MovieMentorDatabase.csproj", "MovieMentorDatabase/"]
RUN dotnet restore "MovieMentorDatabase/MovieMentorDatabase.csproj"
COPY . .
WORKDIR "/src/MovieMentorDatabase"
RUN dotnet build "MovieMentorDatabase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MovieMentorDatabase.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieMentorDatabase.dll"]
