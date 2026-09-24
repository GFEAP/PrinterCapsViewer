// -----------------------------------------------------------------------
// <copyright file="InlineCollectionDummy.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Windows.Documents;

namespace PrinterCapsViewer.Shared.TreeViewHelper;

/// <summary>
/// Stellt eine Hilfsklasse dar, die als typisierte Auflistung von <see cref="Inline"/>-Elementen dient.
/// Wird verwendet, um Sammlungen von Inline-Elementen (z. B. für XAML-Bindungen oder Attached Properties
/// in TreeView-Steuerelementen) als eigenständigen Typ zu deklarieren.
/// </summary>
public class InlineCollectionDummy : List<Inline>
{
}

