# PrinterCapsViewer

**PrinterCapsViewer** ist ein WPF-Desktoptool (.NET 8) zur Anzeige und Analyse der
Fähigkeiten (*Print Capabilities*) und Einstellungen (*Print Tickets*) lokal installierter
Drucker unter Windows.

## Funktionen

- Anzeige aller verfügbaren Druckwarteschlangen (`PrintQueue`) des lokalen Druckservers
- Laden und Darstellen der Druckerfähigkeiten als durchsuchbaren Baum
- Filtern des Fähigkeitenbaums per Freitextsuche
- Anzeige des zugehörigen Print-Tickets als hervorgehobenes, formatiertes XML-Dokument
- Synchronisation zwischen ausgewähltem Baumknoten und der entsprechenden Position im XML

## Voraussetzungen

- Windows-Betriebssystem
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022/2026 mit installierter Workload **.NET-Desktopentwicklung**

## Erste Schritte

1. Repository klonen:
```
git clone https://github.com/GFEAP/PrinterCapsViewer.git
```
2. Projekt in Visual Studio öffnen
```
src\PrinterCapsViewer.slnx
```
3. Projekt `PrinterCapsViewer` als Startprojekt festlegen und ausführen (**F5**).

## Projektstruktur

| Verzeichnis              | Beschreibung                                                    |
|------------------------------|------------------------------------------------------------------|
| `src/ApplicationMain`        | Hauptanwendung, View und ViewModel des Hauptfensters             |
| `src/Converters`             | Wertkonverter für die WPF-Datenbindung (z. B. Texthervorhebung) |
| `src/Shared`                 | Wiederverwendbare Hilfskomponenten (z. B. TreeView-Erweiterungen)|

## Technologie-Stack

- .NET 10.0 / WPF
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)

## Lizenz

Dieses Projekt steht unter der [MIT-Lizenz](LICENSE).

## Autor

Michael Friedl – GFEAP GmbH, Bensheim, Deutschland