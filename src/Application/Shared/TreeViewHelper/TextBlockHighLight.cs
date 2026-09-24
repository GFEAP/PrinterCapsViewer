// -----------------------------------------------------------------------
// <copyright file="TextBlockHighLight.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrinterCapsViewer.Shared.TreeViewHelper
{
    /// <summary>
    /// Stellt eine angehängte Eigenschaft (Attached Property) für <see cref="TextBlock"/> bereit,
    /// mit der eine Sammlung von <see cref="Inline"/>-Elementen (z. B. zur Hervorhebung von Text)
    /// über Bindings gesetzt werden kann. Dies wird benötigt, da <see cref="TextBlock.Inlines"/>
    /// selbst nicht bindbar ist.
    /// </summary>
    public static class TextBlockHighlight
    {
        /// <summary>
        /// Ruft den Wert der angehängten <see cref="InlinesProperty"/>-Eigenschaft für das
        /// angegebene Abhängigkeitsobjekt ab.
        /// </summary>
        /// <param name="obj">Das Abhängigkeitsobjekt, dessen Eigenschaftswert abgerufen werden soll.</param>
        /// <returns>Die Sammlung von <see cref="Inline"/>-Elementen, die dem Objekt zugewiesen sind.</returns>
        public static IEnumerable<Inline> GetInlines(DependencyObject obj)
        {
            return (IEnumerable<Inline>)obj.GetValue(InlinesProperty);
        }

        /// <summary>
        /// Legt den Wert der angehängten <see cref="InlinesProperty"/>-Eigenschaft für das
        /// angegebene Abhängigkeitsobjekt fest.
        /// </summary>
        /// <param name="obj">Das Abhängigkeitsobjekt, dessen Eigenschaftswert festgelegt werden soll.</param>
        /// <param name="value">Die Sammlung von <see cref="Inline"/>-Elementen, die zugewiesen werden soll.</param>
        public static void SetInlines(DependencyObject obj, IEnumerable<Inline> value)
        {
            obj.SetValue(InlinesProperty, value);
        }

        /// <summary>
        /// Identifiziert die <c>Inlines</c>-Abhängigkeitseigenschaft, die es ermöglicht, eine
        /// Sammlung von <see cref="Inline"/>-Elementen an einen <see cref="TextBlock"/> zu binden.
        /// Bei Änderung des Werts wird <see cref="OnInlinesChanged"/> aufgerufen, um die
        /// <see cref="TextBlock.Inlines"/>-Auflistung entsprechend zu aktualisieren.
        /// </summary>
        public static readonly DependencyProperty InlinesProperty =
            DependencyProperty.RegisterAttached(
                "Inlines",
                typeof(IEnumerable<Inline>),
                typeof(TextBlockHighlight),
                new PropertyMetadata(null, OnInlinesChanged));

        /// <summary>
        /// Wird aufgerufen, wenn sich der Wert der <see cref="InlinesProperty"/> ändert.
        /// Leert die vorhandene <see cref="TextBlock.Inlines"/>-Auflistung des Ziel-<see cref="TextBlock"/>
        /// und fügt die neuen <see cref="Inline"/>-Elemente hinzu.
        /// </summary>
        /// <param name="d">Das Abhängigkeitsobjekt, dessen Eigenschaft sich geändert hat. Muss ein <see cref="TextBlock"/> sein.</param>
        /// <param name="e">Die Ereignisdaten mit dem alten und neuen Eigenschaftswert.</param>
        private static void OnInlinesChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBlock textBlock)
                return;

            textBlock.Inlines.Clear();

            if (e.NewValue is IEnumerable<Inline> inlines)
            {
                foreach (var inline in inlines)
                    textBlock.Inlines.Add(inline);
            }
        }
    }
}
