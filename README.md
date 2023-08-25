# RickRoll
Jakiś czas temu natrafiłem na ciekawy [artykuł](https://dev.to/adityaoberai/rick-roll-your-friends-using-appwrite-twilio-and-net-4180) w którym autor opisał jak za pomocą Appwrite, Twilio i .NET stworzyć aplikację, która dzwoni na wybrany numer i puszcza Rick Rolla.

## Efekt końcowy
Ze względu na darmowe konto Twilio tuż przed uruchomieniem kodu prezentowany jest komunikat.


https://github.com/kjurkiew/RickRoll/assets/35111684/bfc4d3f3-678d-4c37-a0b9-41eb487c7ffd



## Warunki wstępne
**Serwer** - Postawiłem na [mikr.us](https://mikr.us/?r=c9545d98) i serwer 3.0. Wymogiem do sprawnego działania Appwrite jest co najmniej 2 GB ramu
**Docker** - Zainstalowałem go korzystając z [dokumentacji dockera](https://docs.docker.com/engine/install/ubuntu/)
**Appwrite** - Cały poradnik jest na ich [stronie](https://appwrite.io/docs/self-hosting#installWithDocker), w dużym skrócie wystarczy wpisać:
```
docker run -it --rm \
    --volume /var/run/docker.sock:/var/run/docker.sock \
    --volume "$(pwd)"/appwrite:/usr/src/code/appwrite:rw \
    --entrypoint="install" \
    appwrite/appwrite:1.3.8
```
Instalując Appwrite, wybrałem domyślne porty a następnie podpiąłem serwer pod darmową subdomene mikur.us - bieda.it.

**Ważne!**
Aby włączyć środowisko .NET dla Appwrite należy wejść do katalogu Appwrite, edytować plik .env np. za pomocą edytora nano, znaleźć w pliku .env zmienną środowiskową '_APP_FUNCTIONS_RUNTIMES' i do listy oddzielonej przecinkami dodać 'dotnet-6.0'.
Aby wgrać nową konfigurację należy skorzystać z polecenia:
```
docker-compose up -d
```
