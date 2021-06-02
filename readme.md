# Prototype 'Configuration aus der Datenbank'

Diese Solution stellt einen Prototyp dar, wie die Configuration einer ASP.NET Core Applikation aus der Datenbank geladen werden kann.

## TODO

Derzeit funktioniert der reload der settings noch nicht. Es ist zu überprüfen ob die IConfiguration die neuen inhalte bekommt, oder nur die IOptions<T> nicht modifiziert sind.

## Datenbank

Eine Mongoinstanz ist im zugehörigen docker-compose file angehängt. 
Zum Testen, eine neue Datenbank (Settings) und eine Collection (Service1) anlegen.
in die Collection ein neues Objekt anlegen, ich hab dafür die appsettings.Docker aus dem Identity Service genommen:

```JSON
  "Redis": {
    "Host": "servicegrid-redis"
  },
  "SgSystemConfiguration": {
    "DatabaseUri": "mongodb://service-grid-persistence:27017",
    "AdminUser": "root",
    "AdminUserPassword": "Secret#1"
  },
  "HostingInformationConfiguration": {
    "Uri": "https://identity-service:9430"
  },
  "MqttClientConfig": {
    "Certificate": {
      "Path": "/app/certs/ccb.scadacert",
      "Alias": ""
    }
  }
```

