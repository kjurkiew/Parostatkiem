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
                twiml: new Twiml("<Response><Play>https://magenta-iguana-8031.twil.io/assets/01%20Krzysztof%20Krawczyk%20-%20Parostatek.mp3</Play></Response>") 
             );

  // Zwróć odpowiedź z biblioteki SDK Twilio.
  return res.Json(new()
  {
    { "twilioResponse", call }
  });
}
