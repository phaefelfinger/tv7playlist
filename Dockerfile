FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# Copy everything else and build
COPY . ./
RUN dotnet restore Tv7Playlist.sln
RUN dotnet publish -c Release -o out Tv7Playlist.sln

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app
RUN mkdir /data

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

VOLUME [ "/data" ]

ENTRYPOINT ["dotnet", "Tv7Playlist.dll"]
