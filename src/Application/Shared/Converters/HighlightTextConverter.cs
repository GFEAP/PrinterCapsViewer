// -----------------------------------------------------------------------
// <copyright file="HighlightTextConverter.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using PrinterCapsViewer.Shared.TreeViewHelper;

namespace PrinterCapsViewer.Shared.Converters
{
    /// <summary>
    /// Ein <see cref="IMultiValueConverter"/>, der einen Text anhand eines
    /// Filterbegriffs in <see cref="Run"/>-Elemente aufteilt und die
    /// Fundstellen optisch hervorhebt (z. B. für Suchfunktionen in der UI).
    /// Nicht übereinstimmende Textteile werden im Normalstil dargestellt,
    /// übereinstimmende Teile im Hervorhebungsstil (Hintergrundfarbe und
    /// Schriftgewicht).
    /// </summary>
    public class HighlightTextConverter : IMultiValueConverter
    {
        // Farben können optional auch über XAML gesetzt werden
        /// <summary>
        /// Hintergrundfarbe, mit der übereinstimmende Textteile
        /// hervorgehoben werden. Standardwert: <see cref="Brushes.LightGoldenrodYellow"/>.
        /// </summary>
        public Brush HighlightBackground { get; set; }
            = Brushes.LightGoldenrodYellow;

        /// <summary>
        /// Vordergrundfarbe (Schriftfarbe), die sowohl für normale als auch
        /// für hervorgehobene Textteile verwendet wird.
        /// Standardwert: <see cref="Brushes.Black"/>.
        /// </summary>
        public Brush NormalForeground { get; set; }
            = Brushes.Black;

        /// <summary>
        /// Schriftgewicht für nicht hervorgehobene Textteile.
        /// Standardwert: <see cref="FontWeights.SemiBold"/>.
        /// </summary>
        public FontWeight NormalWeight { get; set; }
            = FontWeights.SemiBold;

        /// <summary>
        /// Schriftgewicht für hervorgehobene (dem Filter entsprechende)
        /// Textteile. Standardwert: <see cref="FontWeights.Bold"/>.
        /// </summary>
        public FontWeight HighlightWeight { get; set; }
            = FontWeights.Bold;

        /// <summary>
        /// Wandelt einen Ausgangstext und einen Filterbegriff in eine
        /// Sammlung von <see cref="Run"/>-Elementen um, wobei alle
        /// Vorkommen des Filterbegriffs (Groß-/Kleinschreibung wird
        /// ignoriert) hervorgehoben werden.
        /// </summary>
        /// <param name="values">
        /// Ein Array mit mindestens zwei Werten:
        /// <c>values[0]</c> ist der anzuzeigende Text,
        /// <c>values[1]</c> ist der Filterbegriff, nach dem gesucht wird.
        /// </param>
        /// <param name="targetType">Der Zieltyp der Bindung (wird nicht verwendet).</param>
        /// <param name="parameter">Ein optionaler Konverterparameter (wird nicht verwendet).</param>
        /// <param name="culture">Die zu verwendende Kultur (wird nicht verwendet).</param>
        /// <returns>
        /// Eine <see cref="InlineCollectionDummy"/>, die abwechselnd normale
        /// und hervorgehobene <see cref="Run"/>-Elemente enthält, oder
        /// <c>null</c>, wenn <paramref name="values"/> <c>null</c> ist oder
        /// weniger als zwei Elemente enthält. Ist der Filterbegriff leer
        /// oder besteht nur aus Leerzeichen, wird der gesamte Text als
        /// einzelner normaler Run zurückgegeben.
        /// </returns>
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return null!;

            string text = values[0]?.ToString() ?? string.Empty;
            string filter = values[1]?.ToString() ?? string.Empty;

            var inlines = new InlineCollectionDummy();

            // Kein Filter → kompletter Text mit Normal-Style
            if (string.IsNullOrWhiteSpace(filter))
            {
                inlines.Add(CreateNormalRun(text));
                return inlines;
            }

            int index = 0;
            int hitIndex;

            while ((hitIndex = text.IndexOf(
                filter,
                index,
                StringComparison.OrdinalIgnoreCase)) >= 0)
            {
                // Text vor dem Treffer
                if (hitIndex > index)
                {
                    inlines.Add(
                        CreateNormalRun(
                            text.Substring(index, hitIndex - index)));
                }

                // Treffer
                inlines.Add(
                    CreateHighlightRun(
                        text.Substring(hitIndex, filter.Length)));

                index = hitIndex + filter.Length;
            }

            // Restlicher Text
            if (index < text.Length)
            {
                inlines.Add(
                    CreateNormalRun(text.Substring(index)));
            }

            return inlines;
        }

        /// <summary>
        /// Erstellt einen <see cref="Run"/> für einen nicht hervorgehobenen
        /// Textabschnitt unter Verwendung von <see cref="NormalForeground"/>
        /// und <see cref="NormalWeight"/>.
        /// </summary>
        /// <param name="text">Der darzustellende Textabschnitt.</param>
        /// <returns>Ein neuer <see cref="Run"/> mit Normalstil.</returns>
        private Run CreateNormalRun(string text)
        {
            return new Run(text)
            {
                Foreground = NormalForeground,
                FontWeight = NormalWeight
            };
        }

        /// <summary>
        /// Erstellt einen <see cref="Run"/> für einen hervorgehobenen
        /// Textabschnitt (Filtertreffer) unter Verwendung von
        /// <see cref="HighlightBackground"/> und <see cref="HighlightWeight"/>.
        /// </summary>
        /// <param name="text">Der hervorzuhebende Textabschnitt.</param>
        /// <returns>Ein neuer <see cref="Run"/> mit Hervorhebungsstil.</returns>
        private Run CreateHighlightRun(string text)
        {
            return new Run(text)
            {
                Background = HighlightBackground,
                FontWeight = HighlightWeight,
                Foreground = NormalForeground
            };
        }

        /// <summary>
        /// Diese Rückkonvertierung wird nicht unterstützt, da es sich um
        /// eine reine Anzeige-Konvertierung handelt.
        /// </summary>
        /// <param name="value">Der Wert, der zurückkonvertiert werden soll.</param>
        /// <param name="targetTypes">Die Zieltypen der Quellbindungen.</param>
        /// <param name="parameter">Ein optionaler Konverterparameter.</param>
        /// <param name="culture">Die zu verwendende Kultur.</param>
        /// <exception cref="NotSupportedException">
        /// Wird immer ausgelöst, da die Rückkonvertierung nicht unterstützt wird.
        /// </exception>
        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
