// -----------------------------------------------------------------------
// <copyright file="PrintTicketXmlHelper.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Xml.Linq;

namespace PrinterCapsViewer.Shared.PrintTicketHelper;

/// <summary>
/// Stellt Hilfsmethoden zur Konvertierung und Analyse von <see cref="PrintTicket"/>-Objekten
/// als XML bereit, basierend auf dem Print Schema Framework von Microsoft.
/// </summary>
public static class PrintTicketXmlHelper
{
    /// <summary>
    /// Extrahiert die Namen aller aktiven "Option"-Elemente aus dem XML eines <see cref="PrintTicket"/>.
    /// </summary>
    /// <param name="ticket">Das <see cref="PrintTicket"/>, dessen aktive Optionen ermittelt werden sollen. Kann <see langword="null"/> sein.</param>
    /// <returns>
    /// Eine <see cref="HashSet{T}"/> mit den Namen aller gefundenen Optionen (ohne Berücksichtigung der Groß-/Kleinschreibung).
    /// Ist <paramref name="ticket"/> <see langword="null"/>, wird eine leere Menge zurückgegeben.
    /// </returns>
    public static HashSet<string> ExtractActiveOptions(PrintTicket ticket)
    {
        if (ticket == null)
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var xml = PrintTicketToXml(ticket);
        var doc = XDocument.Parse(xml);

        XNamespace psf =
            "http://schemas.microsoft.com/windows/2003/08/printing/printschemaframework";

        return doc
            .Descendants(psf + "Option")
            .Select(o => o.Attribute("name")?.Value)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Bereinigt ein PrintTicket-XML, indem alle "ParameterInit"-Elemente mit dem Namen
    /// "ns0000:PageDevmodeSnapshot" entfernt werden. Dies wird verwendet, um redundante oder
    /// treiberspezifische Snapshot-Daten aus dem XML zu filtern.
    /// </summary>
    /// <param name="xml">Der zu bereinigende PrintTicket-XML-String.</param>
    /// <returns>Der bereinigte XML-String ohne "PageDevmodeSnapshot"-Parameter.</returns>
    public static string CleanPrintTicketXml(string xml)
    {
        var doc = XDocument.Parse(xml);

        XNamespace psf =
            "http://schemas.microsoft.com/windows/2003/08/printing/printschemaframework";

        // Alle PageDevmodeSnapshot-Parameter entfernen
        var nodesToRemove = doc
            .Descendants(psf + "ParameterInit")
            .Where(e =>
                (string)e.Attribute("name") == "ns0000:PageDevmodeSnapshot")
            .ToList();

        foreach (var node in nodesToRemove)
            node.Remove();

        return doc.ToString();
    }

    /// <summary>
    /// Konvertiert ein <see cref="PrintTicket"/>-Objekt in seine XML-Darstellung als String.
    /// </summary>
    /// <param name="ticket">Das zu konvertierende <see cref="PrintTicket"/>. Kann <see langword="null"/> sein.</param>
    /// <returns>
    /// Der XML-Inhalt des <see cref="PrintTicket"/> als String, oder <c>"&lt;null&gt;"</c>,
    /// falls <paramref name="ticket"/> <see langword="null"/> ist.
    /// </returns>
    public static string PrintTicketToXml(PrintTicket ticket)
    {

        if (ticket == null)
            return "<null>";

        using var ms = new MemoryStream();

        // ✅ EINZIG KORREKTER AUFRUF
        ticket.SaveTo(ms);

        ms.Position = 0;

        using var reader = new StreamReader(ms, Encoding.UTF8);
        return reader.ReadToEnd();

    }

}
