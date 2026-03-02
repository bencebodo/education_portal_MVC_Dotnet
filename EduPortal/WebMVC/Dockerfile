FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["WebMVC/EduPortal.WebMVC.csproj", "WebMVC/"]
COPY ["EduPortal.Data/EduPortal.Data.csproj", "EduPortal.Data/"]
COPY ["EduPortal.Logic/EduPortal.Logic.csproj", "EduPortal.Logic/"]
COPY ["EduPortal.Repo/EduPortal.Repo.csproj", "EduPortal.Repo/"]

RUN dotnet restore "WebMVC/EduPortal.WebMVC.csproj"

COPY . .

WORKDIR "/src/WebMVC"
RUN dotnet publish "EduPortal.WebMVC.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "EduPortal.WebMVC.dll"]