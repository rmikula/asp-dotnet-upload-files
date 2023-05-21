# Poznámky k upload souborů

Při http requestu se soubory rovnou posílají. Pokud soubory budou příliš veliké, pak POST request spadne.

```
java.net.SocketException: Broken pipe
```

Musíme povolit větší velikost souborů použitím atributů.

```
[RequestSizeLimit(MaxFileSize)]
[RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
```

Upload souborů spustíme **"scriptem"** ``PostFile.http``. 