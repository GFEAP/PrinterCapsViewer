// -----------------------------------------------------------------------
// <copyright file="InverseBoolToVisibilityConverter.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Globalization;

namespace PrinterCapsViewer.Shared.Converters;

/// <summary>
/// Ein Value-Converter, der einen booleschen Wert invertiert und ihn
/// anschließend in einen <see cref="System.Windows.Visibility"/>-Wert
/// umwandelt, indem die Konvertierungslogik der Basisklasse
/// <see cref="BoolToVisibilityConverter"/> genutzt wird.
/// </summary>
/// <remarks>
/// Dieser Converter ist nützlich, um Steuerelemente in XAML abhängig
/// von einem invertierten booleschen Zustand ein- oder auszublenden,
/// z. B. wenn ein Element nur sichtbar sein soll, wenn eine
/// entsprechende Eigenschaft <c>false</c> ist.
/// </remarks>
public class InverseBoolToVisibilityConverter
    : BoolToVisibilityConverter
{
    /// <summary>
    /// Invertiert den übergebenen booleschen Wert und konvertiert ihn
    /// anschließend mithilfe der Basisklasse in einen
    /// <see cref="System.Windows.Visibility"/>-Wert.
    /// </summary>
    /// <param name="value">
    /// Der zu konvertierende Wert. Es wird erwartet, dass es sich um
    /// einen <see cref="bool"/> oder <see cref="Nullable{Boolean}"/>
    /// handelt. Ist der Wert <c>null</c> oder kein boolescher Wert,
    /// wird <c>false</c> angenommen (und somit zu <c>true</c> invertiert).
    /// </param>
    /// <param name="targetType">Der Zieltyp der Bindung.</param>
    /// <param name="parameter">
    /// Ein optionaler Konverterparameter, der an die Basisklasse
    /// weitergereicht wird.
    /// </param>
    /// <param name="culture">
    /// Die für die Konvertierung zu verwendende Kultur.
    /// </param>
    /// <returns>
    /// Ein <see cref="System.Windows.Visibility"/>-Wert, der auf dem
    /// invertierten booleschen Eingabewert basiert.
    /// </returns>
    public override object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        return base.Convert(!(value as bool? ?? false),
            targetType, parameter, culture);
    }
}
