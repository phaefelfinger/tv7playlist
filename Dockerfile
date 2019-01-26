FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy everything else and build
COPY . ./
RUN dotnet restore Tv7Playlist.sln
RUN dotnet publish -c Release -o out Tv7Playlist.sln

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime

WORKDIR /app
RUN mkdir /data

COPY --from=build-env /app/Tv7Playlist/out .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

VOLUME [ "/data" ]

ENTRYPOINT ["dotnet", "Tv7Playlist.dll"]
