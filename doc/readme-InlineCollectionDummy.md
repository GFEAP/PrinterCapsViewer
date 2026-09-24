# InlineCollectionDummy

**Datei:** `src/Shared/TreeView/InlineCollectionDummy.cs`  
**Namespace:** `PrintCapsDebug.Shared.TreeView`

## Übersicht

`InlineCollectionDummy` ist eine einfache Hilfsklasse, die von `List<Inline>` erbt und keine zusätzliche Logik oder Member hinzufügt.
```csharp
namespace PrintCapsDebug.Shared.TreeView { public class InlineCollectionDummy : List<Inline> { } }
```

## Zweck

`Inline` (`System.Windows.Documents.Inline`) ist die Basisklasse für Inline-Flow-Content-Elemente in WPF (z. B. `Run`, `Bold`, `Italic`, `Hyperlink`), die z. B. in `TextBlock` oder `FlowDocument` zur formatierten Textdarstellung verwendet werden.

Da WPF/XAML generische Typen (wie `List<Inline>`) nur eingeschränkt unterstützt, dient `InlineCollectionDummy` als konkreter, nicht-generischer Ableitungstyp von `List<Inline>`. Dadurch kann dieser Typ:

- als Rückgabetyp einer Eigenschaft oder Methode verwendet werden, die eine Sammlung von `Inline`-Elementen liefert (z. B. für formatierten Text in einer `TreeView`),
- problemlos in XAML referenziert werden, ohne dass generische Typangaben nötig sind,
- als Zieltyp für Daten-Templates oder Value-Konverter dienen, die formatierte Inline-Inhalte (z. B. hervorgehobener Text, Hyperlinks) für Baumknoten in der `TreeView`-Ansicht der Anwendung bereitstellen.

## Verwendung im Projekt

Im aktuellen Workspace wurden keine weiteren direkten Verwendungsstellen dieser Klasse gefunden. Sie steht vermutlich als vorbereitete Erweiterung oder für zukünftige Anpassungen der `TreeView`-Darstellung im `Shared`-Bereich des Projekts bereit, in dem `Inline`-Elemente (z. B. formatierte Texte, Icons, Hyperlinks) innerhalb von Baumknoten dargestellt werden sollen.

## Hinweis

Da die Klasse aktuell keinen eigenen Code enthält, hat sie funktional identisches Verhalten wie `List<Inline>` selbst. Ihr Mehrwert liegt ausschließlich in der konkreten Typdefinition für den Einsatz in WPF-Kontexten (z. B. XAML-Bindungen, DataTemplates).

