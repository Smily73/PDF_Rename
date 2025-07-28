# PDF Rename

Eine moderne WPF-Desktop-Anwendung zum automatischen Umbenennen von PDF-Dateien basierend auf den Metadaten des Dokuments.

## Features

### Moderne Bootstrap-inspirierte Benutzeroberfläche
- **Kopfbereich** mit Anwendungstitel und Statusanzeige
- **Drag & Drop** Unterstützung für PDF-Dateien
- **Responsive Design** das sich an die Fenstergröße anpasst
- **Bootstrap-Styling** für alle UI-Elemente (Buttons, ListView, ComboBox, etc.)

### Drei Betriebsmodi
1. **Automatisch umbenennen**: PDF-Dateien werden sofort beim Hinzufügen umbenannt
2. **Simulations-Modus**: Zeigt die Umbenennungen an, ohne die Dateien tatsächlich zu ändern
3. **Namen vorher bearbeiten**: Ermöglicht die manuelle Bearbeitung der neuen Dateinamen vor dem Umbenennen

### PDF-Metadaten Extraktion
- Verwendet **PdfSharp** (kostenlose Open-Source-Bibliothek)
- Liest den Titel aus den PDF-Metadaten aus
- Wandelt den Titel in einen gültigen Dateinamen um
- Fallback auf den ursprünglichen Dateinamen wenn keine Metadaten vorhanden sind

### Intelligente Dateinamensbereinigung
- Entfernt ungültige Zeichen
- Ersetzt mehrfache Leerzeichen durch Unterstriche
- Begrenzt die Dateinamenlänge
- Stellt sicher, dass der Dateiname nicht leer ist

### Benutzerfreundliche Funktionen
- **Status-Anzeige** für jede Datei (Pending, Processing, Success, Error)
- **Checkboxes** zur Auswahl einzelner Dateien (im Bearbeitungsmodus)
- **Bearbeiten-Button** für jeden Eintrag (im Bearbeitungsmodus)
- **Entfernen-Button** für jeden Eintrag
- **Liste leeren** Funktion
- **Progress-Bar** während der Verarbeitung

## Technische Details

### Architektur
- **WPF** mit .NET 8.0
- **MVVM-Pattern** mit Microsoft MVVM Toolkit
- **Dependency Injection** für Services
- **Asynchrone Verarbeitung** für bessere Benutzererfahrung

### Dependencies
- **CommunityToolkit.Mvvm** (8.2.2) - Für MVVM-Pattern
- **PdfSharp** (6.0.0) - Für PDF-Metadaten-Extraktion

### Projektstruktur
```
PDFRename/
├── Models/
│   ├── Enums.cs - ProcessingMode und FileStatus Enumerationen
│   └── PdfFileItem.cs - Datenmodell für PDF-Dateien
├── Services/
│   ├── PdfMetadataService.cs - PDF-Metadaten-Extraktion
│   └── FileRenameService.cs - Datei-Umbenennungslogik
├── ViewModels/
│   └── MainViewModel.cs - Haupt-ViewModel mit MVVM Toolkit
├── Views/
│   └── EditFileNameDialog.xaml/.cs - Dialog zum Bearbeiten von Dateinamen
├── Converters/
│   └── ProcessingModeConverter.cs - Converter für Enum-zu-String
├── Styles/
│   └── BootstrapStyles.xaml - Bootstrap-inspirierte UI-Styles
├── MainWindow.xaml/.cs - Hauptfenster
└── App.xaml/.cs - Application-Einstiegspunkt
```

## Verwendung

1. **Anwendung starten**
2. **Modus auswählen** aus der Dropdown-Liste
3. **PDF-Dateien** per Drag & Drop in die Liste ziehen
4. Je nach Modus:
   - **Automatisch**: Dateien werden sofort umbenannt
   - **Simulation**: Vorschau der Umbenennungen ohne tatsächliche Änderungen
   - **Bearbeiten**: Namen können vor dem Umbenennen angepasst werden

## Entwicklung

### Projekt erstellen und ausführen
```bash
dotnet restore
dotnet build
dotnet run
```

### Neue Features hinzufügen
Die Anwendung ist vollständig modular aufgebaut. Neue Features können einfach durch:
- Erweitern der Services
- Hinzufügen neuer ViewModels
- Erstellen zusätzlicher Views
- Anpassen der Styles

## Lizenz

Dieses Projekt verwendet:
- **PdfSharp** (MIT License)
- **CommunityToolkit.Mvvm** (MIT License)

Die Anwendung selbst ist Open Source.
