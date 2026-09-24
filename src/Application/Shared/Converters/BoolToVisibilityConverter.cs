// -----------------------------------------------------------------------
// <copyright file="BoolToVisibilityConverter.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PrinterCapsViewer.Shared.Converters;

/// <summary>
/// Konvertiert einen <see cref="bool"/>-Wert in einen <see cref="Visibility"/>-Wert
/// zur Verwendung in WPF-Bindings.
/// </summary>
/// <remarks>
/// Ist der gebundene Wert <c>true</c>, wird <see cref="Visibility.Visible"/> zurückgegeben.
/// Andernfalls wird standardmäßig <see cref="Visibility.Collapsed"/> zurückgegeben,
/// sofern über den <c>ConverterParameter</c> nicht <see cref="Visibility.Hidden"/>
/// angegeben wurde.
/// </remarks>
public class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Konvertiert einen booleschen Wert in einen entsprechenden <see cref="Visibility"/>-Wert.
    /// </summary>
    /// <param name="value">
    /// Der zu konvertierende Wert. Es wird erwartet, dass es sich um einen
    /// <see cref="bool"/> handelt; andere Werte werden als <c>false</c> behandelt.
    /// </param>
    /// <param name="targetType">Der Typ der Bindungsziel-Eigenschaft (wird nicht verwendet).</param>
    /// <param name="parameter">
    /// Optionaler Parameter vom Typ <see cref="Visibility"/>, der den Zustand festlegt,
    /// der zurückgegeben wird, wenn <paramref name="value"/> <c>false</c> ist
    /// (z. B. <see cref="Visibility.Hidden"/> statt des Standardwerts
    /// <see cref="Visibility.Collapsed"/>). <see cref="Visibility.Hidden"/> reserviert
    /// weiterhin Platz im Layout und verschiebt nachfolgende Elemente nicht.
    /// </param>
    /// <param name="culture">Die zu verwendende Kultur im Konverter (wird nicht verwendet).</param>
    /// <returns>
    /// <see cref="Visibility.Visible"/>, wenn <paramref name="value"/> <c>true</c> ist;
    /// andernfalls der über <paramref name="parameter"/> angegebene Zustand oder
    /// standardmäßig <see cref="Visibility.Collapsed"/>.
    /// </returns>
    public virtual object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        bool isVisible = value is bool b && b;

        // Optionaler ConverterParameter, um Hidden statt Collapsed zu nutzen
        // (reserviert Platz im Layout, verschiebt nachfolgende Elemente nicht)
        var hiddenState = parameter as Visibility? ?? Visibility.Collapsed;

        return isVisible ? Visibility.Visible : hiddenState;
    }

    /// <summary>
    /// Nicht implementiert. Diese Konvertierungsrichtung wird von diesem Konverter
    /// nicht unterstützt.
    /// </summary>
    /// <param name="value">Der zu konvertierende Wert (wird nicht verwendet).</param>
    /// <param name="targetType">Der Zieltyp der Konvertierung (wird nicht verwendet).</param>
    /// <param name="parameter">Ein optionaler Parameter (wird nicht verwendet).</param>
    /// <param name="culture">Die zu verwendende Kultur (wird nicht verwendet).</param>
    /// <returns>Wird nie zurückgegeben.</returns>
    /// <exception cref="NotImplementedException">Wird immer ausgelöst.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

