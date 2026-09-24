// -----------------------------------------------------------------------
// <copyright file="RichTextBoxScrollBehavior.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;

namespace PrinterCapsViewer.Shared.Behaviors
{
    /// <summary>
    /// Provides an attached behavior that enables binding a <see cref="RichTextBox"/>'s
    /// vertical scroll position through a dependency property, since
    /// <see cref="RichTextBox"/> does not natively support data binding for scrolling.
    /// </summary>
    public static class RichTextBoxScrollBehavior
    {
        /// <summary>
        /// Identifies the VerticalOffset attached dependency property, which allows
        /// the vertical scroll position of a <see cref="RichTextBox"/> to be set or bound.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "VerticalOffset",
                typeof(double),
                typeof(RichTextBoxScrollBehavior),
                new PropertyMetadata(0.0, OnVerticalOffsetChanged));

        /// <summary>
        /// Gets the value of the <see cref="VerticalOffsetProperty"/> attached property for the specified element.
        /// </summary>
        /// <param name="obj">The dependency object to read the value from.</param>
        /// <returns>The current vertical offset value.</returns>
        public static double GetVerticalOffset(DependencyObject obj) => (double)obj.GetValue(VerticalOffsetProperty);

        /// <summary>
        /// Sets the value of the <see cref="VerticalOffsetProperty"/> attached property on the specified element.
        /// </summary>
        /// <param name="obj">The dependency object to set the value on.</param>
        /// <param name="value">The vertical offset value to apply.</param>
        public static void SetVerticalOffset(DependencyObject obj, double value) => obj.SetValue(VerticalOffsetProperty, value);

        /// <summary>
        /// Handles changes to the <see cref="VerticalOffsetProperty"/> attached property.
        /// If the target <see cref="RichTextBox"/> is already loaded, scrolls it immediately.
        /// Otherwise, defers scrolling until the control's <see cref="FrameworkElement.Loaded"/>
        /// event fires, ensuring the scroll position is applied correctly once it is available.
        /// </summary>
        /// <param name="d">The dependency object whose property changed.</param>
        /// <param name="e">Event data containing the old and new values of the property.</param>
        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox rtb && rtb.IsLoaded)
            {
                rtb.ScrollToVerticalOffset((double)e.NewValue);
            }
            else if (d is RichTextBox rtb2)
            {
                rtb2.Loaded += (s, ev) =>
                {
                    rtb2.ScrollToVerticalOffset((double)e.NewValue);
                };
            }
        }
    }
}
