// -----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using CommunityToolkit.Mvvm.ComponentModel;
using PrinterCapsViewer.Shared.FlowDoc;
using PrinterCapsViewer.Shared.PrinterCapability;
using PrinterCapsViewer.Shared.PrintTicketHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace PrinterCapsViewer.ApplicationMain;

/// <summary>
/// ViewModel für das Hauptfenster der Anwendung. Verwaltet die Liste der verfügbaren
/// Druckwarteschlangen, lädt und zeigt die Druckerfähigkeiten (Print Capabilities) des
/// ausgewählten Druckers an, ermöglicht das Filtern des Fähigkeitenbaums und stellt das
/// zugehörige Print-Ticket sowohl als hervorgehobenes <see cref="FlowDocument"/> als auch
/// als bereinigtes XML dar.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// Interner Speicher für den aktuellen Filtertext (Backing Field für <see cref="FilterText"/>).
    /// </summary>
    private string _filterText;

    /// <summary>
    /// Ordnet den Namen einer Ticket-Option der zugehörigen Position (<see cref="TextPointer"/>)
    /// im dargestellten XML-Dokument zu. Wird verwendet, um bei Auswahl eines Knotens im
    /// Fähigkeitenbaum zur entsprechenden Stelle im XML zu springen.
    /// </summary>
    private Dictionary<string, TextPointer> _xmlOptionIndex { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initialisiert eine neue Instanz von <see cref="MainViewModel"/>.
    /// Ermittelt den lokalen Druckserver, füllt die Liste der verfügbaren
    /// <see cref="PrintQueue"/>-Objekte und registriert den Handler für
    /// Eigenschaftsänderungen.
    /// </summary>
    public MainViewModel()
    {
        var server = new LocalPrintServer();
        foreach (var queue in server.GetPrintQueues())
        {
            PrintQueues.Add(queue);
        }
        PropertyChanged += MainViewModel_PropertyChanged;
    }

    /// <summary>
    /// Reagiert auf Änderungen der beobachteten Eigenschaften dieses ViewModels.
    /// Lädt bei Auswahl eines neuen Druckers dessen Fähigkeiten, wendet bei Änderung
    /// des Filtertexts den Filter auf den Fähigkeitenbaum an und aktualisiert die
    /// Cursorposition im XML-Dokument bei Auswahl eines Knotens.
    /// </summary>
    /// <param name="sender">Die Quelle des Ereignisses.</param>
    /// <param name="e">Die Ereignisargumente mit dem Namen der geänderten Eigenschaft.</param>
    private void MainViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedPrintQueue))
        {
            LoadCapabilitiesForSelectedPrinter();
        }

        if (e.PropertyName == nameof(FilterText))
        {
            if (Root != null)
            {
                ApplyFilter(Root, _filterText);
            }
        }
        if(e.PropertyName == nameof(SelectedNode))
        {
            if (SelectedNode?.Value != null)
            {
                if (_xmlOptionIndex.TryGetValue(SelectedNode.Value.ToString(), out var position))
                {
                    SelectedPosition = position;
                }
            }
        }
    }

    /// <summary>
    /// Gibt an, ob der Fähigkeitenbaum standardmäßig aufgeklappt angezeigt wird.
    /// </summary>
    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    /// <summary>
    /// Liste der auf dem lokalen Druckserver verfügbaren Druckwarteschlangen.
    /// </summary>
    public ObservableCollection<PrintQueue> PrintQueues { get; } = [];


    /// <summary>
    /// Der Wurzelknoten des Baums der Druckerfähigkeiten (Print Capabilities) des
    /// aktuell ausgewählten Druckers.
    /// </summary>
    [ObservableProperty]
    public partial CapabilityNode Root { get; set; }


    /// <summary>
    /// Die aktuell im UI ausgewählte Druckwarteschlange.
    /// </summary>
    [ObservableProperty]
    public partial PrintQueue SelectedPrintQueue { get; set; }


    /// <summary>
    /// Menge der Namen der Optionen, die im aktuellen Print-Ticket aktiv gesetzt sind.
    /// </summary>
    [ObservableProperty]
    public partial HashSet<string> ActiveOptions { get; set; }

    /// <summary>
    /// Der vom Benutzer eingegebene Text zum Filtern des Fähigkeitenbaums.
    /// </summary>
    [ObservableProperty]
    public partial string FilterText { set; get; }

    /// <summary>
    /// Der aktuell im Fähigkeitenbaum ausgewählte Knoten.
    /// </summary>
    [ObservableProperty]
    public partial CapabilityNode SelectedNode { get; set; }

    /// <summary>
    /// Das <see cref="FlowDocument"/>, das den Inhalt des Print-Tickets mit
    /// hervorgehobenen aktiven Optionen zur Anzeige bereitstellt.
    /// </summary>
    [ObservableProperty]
    public partial FlowDocument TicketFlowDocument { get; set; }

    /// <summary>
    /// Der Wert des aktuell ausgewählten Knotens als Zeichenkette, oder "&lt;null&gt;",
    /// falls kein Knoten ausgewählt ist bzw. dieser keinen Wert besitzt.
    /// </summary>
    public string SelectedValue => SelectedNode?.Value?.ToString() ?? "<null>";

    /// <summary>
    /// Das bereinigte XML des aktuellen Print-Tickets als reiner Text.
    /// </summary>
    [ObservableProperty]
    public partial string TicketXml { get; set; }

    /// <summary>
    /// Die Position im <see cref="TicketFlowDocument"/>, die dem aktuell ausgewählten
    /// Knoten des Fähigkeitenbaums entspricht, verwendet zum Scrollen/Hervorheben im XML.
    /// </summary>
    [ObservableProperty]
    public partial TextPointer SelectedPosition { get; set; }

    /// <summary>
    /// Lädt die Druckerfähigkeiten und das effektive Print-Ticket für die aktuell
    /// ausgewählte Druckwarteschlange und aktualisiert den Fähigkeitenbaum, das
    /// Ticket-XML sowie das hervorgehobene <see cref="FlowDocument"/> entsprechend.
    /// Zeigt bei Fehlern eine Fehlermeldung an.
    /// </summary>
    private void LoadCapabilitiesForSelectedPrinter()
    {
        if (SelectedPrintQueue == null)
            return;

        try
        {
            var baseTicket =
                SelectedPrintQueue.UserPrintTicket
                ?? SelectedPrintQueue.DefaultPrintTicket
                ?? new PrintTicket();

            // ✅ Capabilities korrekt laden
            var capabilities =
                SelectedPrintQueue.GetPrintCapabilities(baseTicket);

            Root = PrinterCapabilityHelpers.BuildNode(capabilities, "PrintCapabilities");
            Root.IsExpanded = true;

            // ✅ Effektives Ticket (RICHTIG!)
            var effectiveTicket =
                GetEffectivePrintTicket(SelectedPrintQueue);

            var rawXml = PrintTicketXmlHelper.PrintTicketToXml(effectiveTicket);
            TicketXml = PrintTicketXmlHelper.CleanPrintTicketXml(rawXml);
            var activeOptions = PrintTicketXmlHelper.ExtractActiveOptions(effectiveTicket);


            var helper = FlowDocHelper.Build()
                .WithActiveOptions(activeOptions)
                .WithFontSize(12)
                .WithFontFamily(new FontFamily("Consolas"));
            TicketFlowDocument = helper.BuildHighlightedTicketDocument(TicketXml);
            
            _xmlOptionIndex = helper.XmlOptionIndex;

            // ✅ Mapping anwenden
            PrinterCapabilityHelpers.CompareWithTicket(Root, effectiveTicket);

            ApplyFilter(Root, FilterText);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to load capabilities:\n{ex.Message}",
                "Printer Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Wendet rekursiv einen Filter auf einen Knoten des Fähigkeitenbaums und dessen
    /// untergeordnete Knoten an. Ein Knoten gilt als Treffer, wenn sein Name oder Typ
    /// den Filtertext enthält oder mindestens ein untergeordneter Knoten einen Treffer
    /// darstellt. Bei aktivem Filtertext und Übereinstimmung in Kindknoten wird der
    /// Knoten automatisch aufgeklappt.
    /// </summary>
    /// <param name="node">Der zu prüfende Knoten des Fähigkeitenbaums.</param>
    /// <param name="filter">Der anzuwendende Filtertext.</param>
    /// <returns><see langword="true"/>, wenn der Knoten oder einer seiner Nachfahren dem Filter entspricht, andernfalls <see langword="false"/>.</returns>
    private bool ApplyFilter(CapabilityNode node, string filter)
    {
        if (node != null)
        {
            bool selfMatch =
                string.IsNullOrWhiteSpace(filter) ||
                node.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                node.Type.Contains(filter, StringComparison.OrdinalIgnoreCase);

            bool childMatch = false;

            foreach (var child in node.Children)
            {
                if (ApplyFilter(child, filter))
                {
                    childMatch = true;
                }
            }

            node.MatchesFilter = selfMatch || childMatch;

            // ⭐ HIER passiert das Auto-Expand ⭐
            node.IsExpanded = !string.IsNullOrWhiteSpace(filter) && childMatch;

            return node.MatchesFilter;
        }

        return false;
    }

    /// <summary>
    /// Ermittelt das effektive, validierte Print-Ticket für die angegebene Druckwarteschlange,
    /// indem das Basis-Ticket (Benutzer- oder Standard-Ticket) mit einem leeren Delta-Ticket
    /// zusammengeführt und validiert wird.
    /// </summary>
    /// <param name="queue">Die Druckwarteschlange, für die das effektive Ticket ermittelt werden soll.</param>
    /// <returns>Das validierte, effektive <see cref="PrintTicket"/>.</returns>
    private PrintTicket GetEffectivePrintTicket(PrintQueue queue)
    {
        var baseTicket =
            queue.UserPrintTicket
            ?? queue.DefaultPrintTicket
            ?? new PrintTicket();

        var deltaTicket = new PrintTicket();


        // ✅ Ticket validieren
        var result = queue.MergeAndValidatePrintTicket(
            baseTicket,
           deltaTicket);

        return result.ValidatedPrintTicket;
    }

    /// <summary>
    /// Aktion, die beim Schließen des zugehörigen Fensters ausgeführt wird.
    /// </summary>
    private Action SetCloseWindowAction;

    /// <summary>
    /// Registriert die Aktion, die zum Schließen des Fensters aufgerufen werden soll.
    /// </summary>
    /// <param name="closeAction">Die auszuführende Schließen-Aktion.</param>
    public void SetCloseWindowsAction(Action closeAction)
    {
        SetCloseWindowAction = closeAction;
    }
}

