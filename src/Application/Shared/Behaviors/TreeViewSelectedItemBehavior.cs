// -----------------------------------------------------------------------
// <copyright file="TreeViewSelectedItemBehavior.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;

namespace PrinterCapsViewer.Shared.Behaviors;

/// <summary>
/// Stellt eine angehängte Eigenschaft ("Attached Property") bereit, die eine bidirektionale
/// Bindung des <see cref="TreeView.SelectedItem"/> an eine ViewModel-Eigenschaft ermöglicht,
/// da <see cref="TreeView.SelectedItem"/> selbst schreibgeschützt ist und daher nicht direkt
/// gebunden werden kann.
/// </summary>
public static class TreeViewSelectedItemBehavior
{
    // WICHTIG: Defaultwert ist new object() (nicht null),
    // da WPF sonst das PropertyChanged-Callback beim ersten Setzen nicht auslöst.
    // Siehe: https://stackoverflow.com/questions/18396345/treeview-selecteditem-binding-doesnt-work
    /// <summary>
    /// Identifiziert die angehängte "SelectedItem"-Abhängigkeitseigenschaft, die es ermöglicht,
    /// das ausgewählte Element eines <see cref="TreeView"/> über Data-Binding zu lesen und zu setzen.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.RegisterAttached(
            "SelectedItem",
            typeof(object),
            typeof(TreeViewSelectedItemBehavior),
            new FrameworkPropertyMetadata(new object(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

    /// <summary>
    /// Ruft den Wert der angehängten "SelectedItem"-Eigenschaft für das angegebene Abhängigkeitsobjekt ab.
    /// </summary>
    /// <param name="obj">Das <see cref="DependencyObject"/>, in der Regel ein <see cref="TreeView"/>, dessen Wert abgerufen werden soll.</param>
    /// <returns>Das aktuell ausgewählte Element.</returns>
    public static object GetSelectedItem(DependencyObject obj) => obj.GetValue(SelectedItemProperty);

    /// <summary>
    /// Legt den Wert der angehängten "SelectedItem"-Eigenschaft für das angegebene Abhängigkeitsobjekt fest.
    /// </summary>
    /// <param name="obj">Das <see cref="DependencyObject"/>, in der Regel ein <see cref="TreeView"/>, dessen Wert festgelegt werden soll.</param>
    /// <param name="value">Das Element, das als ausgewählt markiert werden soll.</param>
    public static void SetSelectedItem(DependencyObject obj, object value) => obj.SetValue(SelectedItemProperty, value);

    /// <summary>
    /// Wird aufgerufen, wenn sich der Wert der angehängten "SelectedItem"-Eigenschaft ändert
    /// (z. B. durch Setzen aus dem ViewModel). Registriert den <see cref="TreeView.SelectedItemChanged"/>-Ereignishandler
    /// am zugehörigen <see cref="TreeView"/>, um Änderungen der TreeView-internen Auswahl
    /// wieder zurück in die Abhängigkeitseigenschaft zu synchronisieren.
    /// </summary>
    /// <param name="d">Das Abhängigkeitsobjekt, an dem sich die Eigenschaft geändert hat.</param>
    /// <param name="e">Die Ereignisdaten mit altem und neuem Wert der Eigenschaft.</param>
    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TreeView treeView)
        {
            treeView.SelectedItemChanged -= TreeView_SelectedItemChanged;
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }
    }

    /// <summary>
    /// Behandelt das <see cref="TreeView.SelectedItemChanged"/>-Ereignis und übernimmt das neu
    /// ausgewählte Element in die angehängte "SelectedItem"-Abhängigkeitseigenschaft, damit
    /// die Änderung per Data-Binding an ein gebundenes ViewModel weitergegeben wird.
    /// </summary>
    /// <param name="sender">Der Auslöser des Ereignisses, in der Regel der betroffene <see cref="TreeView"/>.</param>
    /// <param name="e">Die Ereignisdaten mit dem zuvor und dem neu ausgewählten Element.</param>
    private static void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is TreeView treeView)
        {
            SetSelectedItem(treeView, e.NewValue);
        }
    }
}

