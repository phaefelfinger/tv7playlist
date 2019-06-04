# TV7-Playlist

## Summary

This little application is used to rewrite the TV7 multicast channel list by fiber7 m3u.
The updated list will proxy the multicast stream through udpxy and builds a stream that plex can handle.

Others have changed the code in telly. As I did not want to change any external source, I just
wrote this little application.

There are more features than just changing the URL:

- Resorting of the channel list
- Enable or disable a channel
- Enable or disable multiple channels at once
- Override the channel number -> better EPG Detection support in plex / emby
- Override the channel name -> better EPG Detection support in plex / emby

This is licensed under GPLv2. See License file.

## Docker

### Run the application

You may run this application using docker. If you want to persist the database when doing some updates, create a volume and use this as data storage.

```shell
docker volume create tv7playlist_data
```

Next you have to create and run the docker container.

```shell
docker run -t --name="tv7playlist" -p 8000:80 -e "UdpxyUrl=http://your.host.ip.of.udpxy:4022/udp" -v tv7playlist_data:/data --restart=unless-stopped phaefelfinger/tv7playlist:latest
```

### Environment variables

- SourceType: "M3U" or "Xspf"
- SqLiteConnectionString: "Data Source=/data/playlist.db"
- TV7Url: "https://api.init7.net/tvchannels.m3u" or "https://api.init7.net/tvchannels.xspf" or any other provider
- UdpxyUrl: "http://your.host.ip.of.udpxy:4022/udp" or empty
- DownloadFileName: "PlaylistTV7udpxy.m3u" or any name that should be sent as filename while downloading the list
