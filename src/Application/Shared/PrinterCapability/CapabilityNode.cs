// -----------------------------------------------------------------------
// <copyright file="CapabilityNode.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace PrinterCapsViewer.Shared.PrinterCapability;

/// <summary>
/// Repräsentiert einen Knoten in der Drucker-Fähigkeiten-Hierarchie (Printer Capability),
/// wie er beispielsweise aus einer PrintCapabilities-XML-Struktur abgeleitet wird.
/// Ein Knoten kann untergeordnete Knoten enthalten und unterstützt Änderungsbenachrichtigungen
/// über <see cref="ObservableObject"/> für die Bindung an die Benutzeroberfläche.
/// </summary>
public partial class CapabilityNode : ObservableObject
{
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="CapabilityNode"/>-Klasse.
    /// Setzt die Standardwerte für <see cref="IsSupported"/> und <see cref="Children"/>
    /// explizit im Konstruktor, um ein verlässliches Verhalten sicherzustellen
    /// (anstelle von Property-Initializern), und registriert den internen
    /// PropertyChanged-Handler.
    /// </summary>
    public CapabilityNode()
    {
        IsSupported = true; //  verlässlich statt Property-Initializer
        Children = []; //  verlässlicher als Property-Initializer        
    }
    
    /// <summary>
    /// Wird automatisch aufgerufen, wenn sich der Wert von <see cref="IsSupported"/> ändert.
    /// Aktualisiert <see cref="IsIncompatible"/> als logisches Gegenteil des neuen Werts.
    /// </summary>
    /// <param name="value">Der neue Wert von <see cref="IsSupported"/>.</param>
    partial void OnIsSupportedChanged(bool value)
    {
        IsIncompatible = !value;
    }

    /// <summary>
    /// Ruft den Namen der Fähigkeit ab oder legt diesen fest (z. B. der Name der Eigenschaft
    /// oder des Features im Drucker-Fähigkeitenbaum).
    /// </summary>
    [ObservableProperty]
    public partial string Name { get; set; }

    /// <summary>
    /// Ruft den Datentyp der Fähigkeit ab oder legt diesen fest.
    /// </summary>
    [ObservableProperty]
    public partial string Type { get; set; }

    /// <summary>
    /// Ruft den aktuellen Wert der Fähigkeit ab oder legt diesen fest.
    /// </summary>
    [ObservableProperty]
    public partial object Value { get; set; }

    /// <summary>
    /// Ruft die für die Benutzeroberfläche aufbereitete Anzeigedarstellung
    /// des Werts ab oder legt diese fest.
    /// </summary>
    [ObservableProperty]
    public partial string DisplayValue { get; set; }

    /// <summary>
    /// Ruft die Liste der untergeordneten <see cref="CapabilityNode"/>-Instanzen ab
    /// oder legt diese fest, wodurch die hierarchische Struktur der Fähigkeiten abgebildet wird.
    /// </summary>
    [ObservableProperty]
    public partial List<CapabilityNode> Children { get; set; } 

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob dieser Knoten dem aktuell angewendeten
    /// Filterkriterium entspricht, oder legt diesen fest.
    /// </summary>
    [ObservableProperty]
    public partial bool MatchesFilter { get; set; }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob dieser Knoten in der Benutzeroberfläche
    /// erweitert (aufgeklappt) dargestellt wird, oder legt diesen fest.
    /// </summary>
    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob dieser Knoten Teil des aktuellen
    /// Support-Tickets ist, oder legt diesen fest.
    /// </summary>
    [ObservableProperty]
    public partial bool IsInTicket { get; set; }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob diese Fähigkeit vom Drucker unterstützt wird,
    /// oder legt diesen fest. Eine Änderung dieses Werts aktualisiert automatisch
    /// <see cref="IsIncompatible"/> (siehe <see cref="OnIsSupportedChanged(bool)"/>).
    /// </summary>
    [ObservableProperty]
    public partial bool IsSupported { get; set; }   // kein "= true" mehr am Property

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob diese Fähigkeit inkompatibel ist
    /// (d. h. das logische Gegenteil von <see cref="IsSupported"/>), oder legt diesen fest.
    /// </summary>
    [ObservableProperty]
    public partial bool IsIncompatible { get; set; }
}

