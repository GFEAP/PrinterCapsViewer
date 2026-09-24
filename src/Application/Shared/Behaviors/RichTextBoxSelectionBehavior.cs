// -----------------------------------------------------------------------
// <copyright file="RichTextBoxSelectionBehavior.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrinterCapsViewer.Shared.Behaviors
{
    /// <summary>
    /// Bereitgestellt eine angehängte Eigenschaft, mit der die Cursorposition bzw. Auswahl
    /// eines <see cref="RichTextBox"/>-Steuerelements über Bindings (z. B. aus einem ViewModel)
    /// gesteuert werden kann.
    /// </summary>
    public static class RichTextBoxSelectionBehavior
    {
        /// <summary>
        /// Angehängte Eigenschaft, die eine <see cref="TextPointer"/>-Position innerhalb eines
        /// <see cref="RichTextBox"/>-Steuerelements festlegt. Beim Setzen wird die Auswahl an dieser
        /// Position platziert, der Fokus auf das Steuerelement gesetzt und es wird an die Position gescrollt.
        /// </summary>
        public static readonly DependencyProperty SelectionPositionProperty =
            DependencyProperty.RegisterAttached(
                "SelectionPosition",
                typeof(TextPointer),
                typeof(RichTextBoxSelectionBehavior),
                new PropertyMetadata(null, OnSelectionPositionChanged));

        /// <summary>
        /// Ruft den Wert der <see cref="SelectionPositionProperty"/>-Eigenschaft für das angegebene Objekt ab.
        /// </summary>
        /// <param name="obj">Das Zielobjekt, üblicherweise ein <see cref="RichTextBox"/>-Steuerelement.</param>
        /// <returns>Die aktuell zugewiesene <see cref="TextPointer"/>-Position.</returns>
        public static TextPointer GetSelectionPosition(DependencyObject obj) => (TextPointer)obj.GetValue(SelectionPositionProperty);

        /// <summary>
        /// Legt den Wert der <see cref="SelectionPositionProperty"/>-Eigenschaft für das angegebene Objekt fest.
        /// </summary>
        /// <param name="obj">Das Zielobjekt, üblicherweise ein <see cref="RichTextBox"/>-Steuerelement.</param>
        /// <param name="value">Die zu setzende <see cref="TextPointer"/>-Position.</param>
        public static void SetSelectionPosition(DependencyObject obj, TextPointer value) => obj.SetValue(SelectionPositionProperty, value);

        /// <summary>
        /// Wird aufgerufen, wenn sich der Wert der <see cref="SelectionPositionProperty"/>-Eigenschaft ändert.
        /// Setzt den Fokus auf das <see cref="RichTextBox"/>-Steuerelement, positioniert die Auswahl an der
        /// neuen <see cref="TextPointer"/>-Position und scrollt das Steuerelement, damit die Position sichtbar wird.
        /// </summary>
        /// <param name="d">Das Abhängigkeitsobjekt, an dem die Änderung aufgetreten ist (erwartet wird ein <see cref="RichTextBox"/>).</param>
        /// <param name="e">Die Ereignisdaten mit dem alten und neuen Eigenschaftswert.</param>
        private static void OnSelectionPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox rtb && e.NewValue is TextPointer position)
            {
                rtb.Focus();
                rtb.Selection.Select(position1: position, position2: position);
                rtb.ScrollToVerticalOffset(rtb.VerticalOffset + 100);
            }
        }
    }
}
