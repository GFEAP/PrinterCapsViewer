// -----------------------------------------------------------------------
// <copyright file="RichTextBoxBindingHelper.cs" company="GFEAP GmbH">
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
    /// Provides an attached property that enables data binding of a <see cref="FlowDocument"/>
    /// to a <see cref="RichTextBox"/> control, which does not natively support binding its
    /// <see cref="RichTextBox.Document"/> property.
    /// </summary>
    public static class RichTextBoxBindingHelper
    {
        /// <summary>
        /// Identifies the BindableDocument attached dependency property, which allows a
        /// <see cref="FlowDocument"/> to be bound to a <see cref="RichTextBox"/>.
        /// </summary>
        public static readonly DependencyProperty BindableDocumentProperty =
            DependencyProperty.RegisterAttached(
                "BindableDocument",
                typeof(FlowDocument),
                typeof(RichTextBoxBindingHelper),
                new PropertyMetadata(null, OnBindableDocumentChanged));

        /// <summary>
        /// Gets the value of the BindableDocument attached property for the specified object.
        /// </summary>
        /// <param name="obj">The dependency object from which to read the property value.</param>
        /// <returns>The <see cref="FlowDocument"/> currently assigned to the object.</returns>
        public static FlowDocument GetBindableDocument(DependencyObject obj) => (FlowDocument)obj.GetValue(BindableDocumentProperty);

        /// <summary>
        /// Sets the value of the BindableDocument attached property for the specified object.
        /// </summary>
        /// <param name="obj">The dependency object on which to set the property value.</param>
        /// <param name="value">The <see cref="FlowDocument"/> to assign to the object.</param>
        public static void SetBindableDocument(DependencyObject obj, FlowDocument value) => obj.SetValue(BindableDocumentProperty, value);

        /// <summary>
        /// Called when the BindableDocument attached property changes. Assigns the new
        /// <see cref="FlowDocument"/> value to the <see cref="RichTextBox.Document"/> property
        /// of the target <see cref="RichTextBox"/>.
        /// </summary>
        /// <param name="d">The dependency object whose property changed. Expected to be a <see cref="RichTextBox"/>.</param>
        /// <param name="e">Event data containing the old and new values of the property.</param>
        private static void OnBindableDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox rtb)
            {
                rtb.Document = e.NewValue as FlowDocument;
            }
        }
    }     
}

