// -----------------------------------------------------------------------
// <copyright file="FlowDocHelper.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace PrinterCapsViewer.Shared.FlowDoc;

/// <summary>
/// Provides helper methods to build a <see cref="FlowDocument"/> that visually
/// highlights printer ticket options (e.g. PSK/PSF options) within an XML string.
/// </summary>
public partial class FlowDocHelper
{
    /// <summary>
    /// Gets or sets an index that maps option names (e.g. "psk:Portrait") to their
    /// corresponding <see cref="TextPointer"/> position within the generated document.
    /// </summary>
    public Dictionary<string, TextPointer> XmlOptionIndex { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    private HashSet<string>? _activeOptions;
    private FontFamily _fontFamily { get; set; } = new FontFamily("Consolas");
    private double _fontSize { get; set; } = 12.0;

    /// <summary>
    /// Creates a new instance of <see cref="FlowDocHelper"/>.
    /// </summary>
    /// <returns>A new <see cref="FlowDocHelper"/> instance ready for further configuration.</returns>
    public static FlowDocHelper Build() { 
        return new FlowDocHelper();
    }

    /// <summary>
    /// Specifies the set of option names that should be highlighted as "active"
    /// in the generated document.
    /// </summary>
    /// <param name="activeOptions">The set of active option names (case-insensitive).</param>
    /// <returns>The current <see cref="FlowDocHelper"/> instance for method chaining.</returns>
    public FlowDocHelper WithActiveOptions(HashSet<string> activeOptions)
    {
        _activeOptions = activeOptions ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        return this;
    }

    /// <summary>
    /// Specifies the font family to use for the generated document.
    /// </summary>
    /// <param name="fontFamily">The desired font family. Defaults to "Consolas" if null.</param>
    /// <returns>The current <see cref="FlowDocHelper"/> instance for method chaining.</returns>
    public FlowDocHelper WithFontFamily(FontFamily fontFamily)
    {
        _fontFamily = fontFamily ?? new FontFamily("Consolas");
        return this;
    }

    /// <summary>
    /// Specifies the font size to use for the generated document.
    /// </summary>
    /// <param name="fontSize">The desired font size. Must be greater than zero, otherwise defaults to 12.0.</param>
    /// <returns>The current <see cref="FlowDocHelper"/> instance for method chaining.</returns>
    public FlowDocHelper WithFontSize(double fontSize)
    {
        _fontSize = fontSize > 0 ? fontSize : 12.0;
        return this;
    }

    /// <summary>
    /// Builds a <see cref="FlowDocument"/> from the given XML string, highlighting
    /// recognized PSK/PSF options and indexing their positions in <see cref="XmlOptionIndex"/>.
    /// </summary>
    /// <param name="xml">The raw XML ticket content to render and highlight.</param>
    /// <returns>A <see cref="FlowDocument"/> containing the highlighted content.</returns>
    public FlowDocument BuildHighlightedTicketDocument(string xml)
    {
        XmlOptionIndex.Clear();

        var doc = new FlowDocument
        {
            FontFamily = _fontFamily,
            FontSize = _fontSize
        };

        var paragraph = new Paragraph();
        var lines = xml.Split(["\r\n", "\n"], StringSplitOptions.None);

        var optionRegex = FindOptionRegex();

        foreach (var line in lines)
        {
            var match = optionRegex.Match(line);
            Run run;

            if (match.Success)
            {
                var optionName = match.Groups[1].Value; // e.g. psk:Portrait
                run = new Run(line + Environment.NewLine);

                bool isActive = _activeOptions.Contains(optionName);
                if (isActive)
                {
                    run.Background = Brushes.LightGreen;
                    run.FontWeight = FontWeights.Bold;
                }

                if (optionName.StartsWith("ns"))
                {
                    run.Background = Brushes.Orange;
                }

                // ✅ INDEXIEREN
                XmlOptionIndex[optionName] = run.ContentStart;
            }
            else
            {
                run = new Run(line + Environment.NewLine);
            }

            paragraph.Inlines.Add(run);
        }

        doc.Blocks.Add(paragraph);
        return doc;
    }

    /// <summary>
    /// Gets a compiled regular expression that matches PSF option elements
    /// (e.g. &lt;psf:Option name="..."/&gt;) and captures the option name.
    /// </summary>
    [GeneratedRegex(@"<psf:Option\s+name=""([^""]+)""")]
    private static partial Regex FindOptionRegex();

    /// <summary>
    /// Gets a compiled regular expression that matches PSK option elements
    /// (e.g. &lt;psk:Option name="..."/&gt;) and captures the option name.
    /// </summary>
    //psk:JobBindAllDocuments
    [GeneratedRegex(@"<psk:Option\s+name=""([^""]+)""")]
    private static partial Regex FindPskRegex();
}
