# RickRoll z Appwrite, Twilio i .NET
Jakiś czas temu natrafiłem na ciekawy [artykuł](https://dev.to/adityaoberai/rick-roll-your-friends-using-appwrite-twilio-and-net-4180) w którym autor opisał jak za pomocą Appwrite, Twilio i .NET stworzyć aplikację, która dzwoni na wybrany numer i puszcza Rick Rolla. 

## Efekt końcowy
Ze względu na darmowe konto Twilio tuż przed uruchomieniem kodu prezentowany jest komunikat.


https://github.com/kjurkiew/RickRoll/assets/35111684/bfc4d3f3-678d-4c37-a0b9-41eb487c7ffd



## Warunki wstępne
**Serwer** - Postawiłem na [mikr.us](https://mikr.us/?r=c9545d98) i serwer 3.0. Wymogiem do sprawnego działania Appwrite jest co najmniej 2 GB ramu
**Docker** - Zainstalowałem go korzystając z [dokumentacji dockera](https://docs.docker.com/engine/install/ubuntu/)
**Appwrite** - Cały poradnik jest na [stronie Appwrite](https://appwrite.io/docs/self-hosting#installWithDocker), w dużym skrócie wystarczy wpisać:
```
docker run -it --rm \
    --volume /var/run/docker.sock:/var/run/docker.sock \
    --volume "$(pwd)"/appwrite:/usr/src/code/appwrite:rw \
    --entrypoint="install" \
    appwrite/appwrite:1.3.8
```
Instalując Appwrite, wybrałem domyślne porty a następnie podpiąłem serwer pod darmową subdomene mikur.us - bieda.it.
Po zainstalowaniu należy wejść na naszą stronę i stworzyć nowe konto admina
![Appwrite](https://github.com/kjurkiew/RickRoll/assets/35111684/488435c4-0807-461b-8f4f-67936f5cc852)

Aby zainstalować CLI Appwrite wystarczy wpisać:
```
curl -sL https://appwrite.io/cli/install.sh | bash
```
Cała dokumentacją CLI znajduje się [tutaj](https://appwrite.io/docs/command-line)

**Ważne!**
Aby włączyć środowisko .NET dla Appwrite należy wejść do katalogu Appwrite, edytować plik .env np. za pomocą edytora nano, znaleźć w pliku .env zmienną środowiskową '_APP_FUNCTIONS_RUNTIMES' i do listy oddzielonej przecinkami dodać 'dotnet-6.0'.
Aby wgrać nową konfigurację należy skorzystać z polecenia:
```
docker-compose up -d
```
***Twilio*** - należy zarejestrować się na stronie [Twilio](https://www.twilio.com/try-twilio) gdzie otrzymamy darmowe konto oraz numer telefonu. Z darmowego konta możemy dzwonić tylko na zweryfikowane numery telefonów. Trzeba też pamiętać aby pozwolić aplikacji dzwonić do naszego kraju. Całość można ustawić [tutaj](https://www.twilio.com/console/voice/settings/geo-permissions)

## Konfigurowanie Appwrite
Aby utworzyć funkcję w Appwrite, która pozwoli nam na RickRollowanie telefonu, potrzebujemy najpierw zalogować się do interfejsu CLI Appwrite za pomocą polecenia:
```
appwrite login
? Enter your email adres@mailowy.com
? Enter your password ********
✓ Success
```
Następnie tworzymy nowy projekt Appwrite za pomocą polecenia:
```
appwrite init project
? How would you like to start? Create a new Appwrite project
? What would you like to name your project? Projekt Rick
? What ID would you like to have for your project? unique()
✓ Success
```
Inicjujemy funkcję wybierając środowisko wykonawcze .NET 6.0:
```
appwrite init function
? What would you like to name your function? Roll
? What ID would you like to have for your function? unique()
? What runtime would you like to use? .NET (dotnet-6.0)
✓ Success
```

## Tworzenie funkcji
Odwiedź utworzony katalog functions/Roll. Struktura plików wygląda w nim następująco:

```
Roll
├── Function.csproj 
|
└── src
    └── Index.cs
```
Edytuj plik src/Index.cs i wprowadź w nim następujący kod:
```
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

public async Task<RuntimeResponse> Main(RuntimeRequest req, RuntimeResponse res)
{
  // Konwertuj ładunek JSON na słownik i odczytaj numer telefonu do wywołania.
  var payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(req.Payload);
  var toPhoneNumber = payload["phoneNumber"];

  // Pobierz identyfikator konta (SID), token uwierzytelniający (Auth Token) oraz numer telefonu Twilio z zmiennych środowiskowych.
  var accountSID = req.Variables["TWILIO_ACCOUNTSID"];
  var authToken = req.Variables["TWILIO_AUTHTOKEN"];
  var twilioPhoneNumber = req.Variables["TWILIO_PHONENUMBER"];

  //Inicjalizuj bibliotekę SDK Twilio.
  TwilioClient.Init(accountSID, authToken);

  // Stwórz połączenie telefoniczne przy użyciu interfejsu API głosowego Twilio
  var call = CallResource.Create(
                to: new PhoneNumber(toPhoneNumber),
                from: new PhoneNumber(twilioPhoneNumber),
                twiml: new Twiml("<Response><Play>https://demo.twilio.com/docs/classic.mp3</Play></Response>") 
             );

  // Zwróć odpowiedź z biblioteki SDK Twilio.
  return res.Json(new()
  {
    { "twilioResponse", call }
  });
}
```

Aby mieć pewność, że biblioteka *Newtonsoft.Json* i Twilio będzie zainstalowana przez Appwrite musimy uwzględnić je w pliku *Function.csproj*

```
<ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
    <PackageReference Include="Twilio" Version="5.75.2" />
</ItemGroup>
```

Funkcję możemy wdrożyć za pomocą konsoli i polecenia
```
appwrite deploy function
```
Bądź manualnie. W tym celu otwieramy katalog w którym znajduje się nasza funkcja *Roll* i pakujemy ją za pomocą polecenia:
```
tar -czf code.tar.gz --exclude code.tar.gz .
```
Wchodzimy na naszą stronę, wybieramy projekt *Rick*, funkcję *Roll* i klikamy "+Create deployment"
![2](https://github.com/kjurkiew/RickRoll/assets/35111684/34368493-c7c2-4a3d-aca8-4533c3c7e460)
W Entrypoint wpisujemy:
```
src/Index.cs
```
Wrzucamy spakowany plik code.targ.gz
Zaznaczamy *Activate deployment after build*

Ostatnią rzeczą, którą musimy zrobić, zanim będziemy mogli przetestować funkcję, jest dodanie niezbędnych zmiennych środowiskowych do strony Ustawienia funkcji. Zrobimy to klikając w *Settings*
*TWILIO_ACCOUNTSID:* Identyfikator SID konta Twilio
*TWILIO_AUTHTOKEN:* Token uwierzytelniania Twilio
*TWILIO_PHONENUMBER:* Numer telefonu Twilio
![3](https://github.com/kjurkiew/RickRoll/assets/35111684/69f0f09e-01b0-4fa2-8683-c90b5d56fcb4)

Wszystko już gotowe!

## Uruchomienie funkcji
Wybieramy *Execute now* i wpisujemy
```
{
    "phoneNumber": "+48123456789"
}
```
I możemy cieszyć się Rick Rollem :D

Odpowiedź z Twilio SDK znajdziemy w logach funkcji


Oczywiście, mogę dodać entery, aby poprawić czytelność tego tekstu. Oto on z dodanymi entrami:


{
  "twilioResponse": {
    "Sid": "CA1f810e5fd5e83cc193467892abaf9323",
    "DateCreated": null,
    "DateUpdated": null,
    "ParentCallSid": null,
    "AccountSid": "AC1b5daa43642bf69ad6dc906acfa2ae40",
    "To": "+48791774028",
    "ToFormatted": "+48791774028",
    "From": "+12184525884",
    "FromFormatted": "(218) 452-5884",
    "PhoneNumberSid": "PN587d187e81ce586d7cad161ff2d5598d",
    "Status": [],
    "StartTime": null,
    "EndTime": null,
    "Duration": null,
    "Price": null,
    "PriceUnit": "USD",
    "Direction": "outbound-api",
    "AnsweredBy": null,
    "ApiVersion": "2010-04-01",
    "ForwardedFrom": null,
    "GroupSid": null,
    "CallerName": null,
    "QueueTime": "0",
    "TrunkSid": null,
    "Uri": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/CA1f810e5fd5e83cc193467892abaf9323.json",
    "SubresourceUris": {
      "feedback": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/CA1f810e5fd5e83cc193467892abaf9323/Feedback.json",
      "user_defined_messages": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/CA1f810e5fd5e83cc193467892abaf9323/UserDefinedMessages.json",
      "notifications": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/CA1f810e5fd5e83cc193467892abaf9323/Notifications.json",
      "recordings": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/CA1f810e5fd5e83cc193467892abaf9323/Recordings.json",
      "streams": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/CA1f810e5fd5e83cc193467892abaf9323/Streams.json",
      "payments": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/CA1f810e5fd5e83cc193467892abaf9323/Payments.json",
      "user_defined_message_subscriptions": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/CA1f810e5fd5e83cc193467892abaf9323/UserDefinedMessageSubscriptions.json",
      "siprec": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/CA1f810e5fd5e83cc193467892abaf9323/Siprec.json",
      "events": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/CA1f810e5fd5e83cc193467892abaf9323/Events.json",
      "feedback_summaries": "/2010-04-01/Accounts/AC1b5daa43642bf69ad6dc906acfa2ae40/Calls/FeedbackSummary.json"
    }
  }
}


