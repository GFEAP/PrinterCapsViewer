// -----------------------------------------------------------------------
// <copyright file="PrinterCapabilityHelpers.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Linq;
using System.Printing;
using System.Reflection;

namespace PrinterCapsViewer.Shared.PrinterCapability
{
    /// <summary>
    /// Stellt Hilfsmethoden zur Verarbeitung und Anzeige von Druckerfähigkeiten
    /// (<see cref="PrintCapabilities"/>) und Druckaufträgen (<see cref="PrintTicket"/>) bereit,
    /// unter anderem zum Aufbau einer hierarchischen Knotenstruktur (<see cref="CapabilityNode"/>)
    /// mittels Reflektion sowie zum Abgleich mit einem Print-Ticket.
    /// </summary>
    public class PrinterCapabilityHelpers
    {
        /// <summary>
        /// Ermittelt den Anzeigenamen einer Fähigkeits-Option, indem versucht wird,
        /// die Eigenschaft "Name" des angegebenen Objekts per Reflektion auszulesen.
        /// </summary>
        /// <param name="value">Das Objekt, dessen "Name"-Eigenschaft ausgelesen werden soll.</param>
        /// <returns>
        /// Der Wert der "Name"-Eigenschaft als <see cref="string"/>, oder <see langword="null"/>,
        /// wenn <paramref name="value"/> <see langword="null"/> ist, keine "Name"-Eigenschaft besitzt
        /// oder diese nicht vom Typ <see cref="string"/> ist.
        /// </returns>
        public static string GetCapabilityOptionName(object value)
        {
            var prop = value?.GetType().GetProperty("Name");
            return prop?.GetValue(value) as string;
        }

        /// <summary>
        /// Gleicht rekursiv einen <see cref="CapabilityNode"/>-Baum mit einem <see cref="PrintTicket"/> ab
        /// und markiert jeden Knoten (<see cref="CapabilityNode.IsInTicket"/>) danach, ob sein Wert
        /// im übergebenen Print-Ticket aktiv gesetzt ist.
        /// </summary>
        /// <param name="node">
        /// Der zu prüfende Knoten. Wenn <see langword="null"/>, wird die Methode ohne Aktion beendet.
        /// </param>
        /// <param name="ticket">Das Print-Ticket, gegen das der Knoten abgeglichen wird.</param>
        /// <remarks>
        /// Nachdem alle Kindknoten rekursiv verarbeitet wurden, wird ein Elternknoten zusätzlich
        /// als im Ticket aktiv markiert, wenn mindestens einer seiner direkten Kindknoten aktiv ist.
        /// </remarks>
        public static void CompareWithTicket(
            CapabilityNode node,
            PrintTicket ticket)
        {
            if (node == null)
            {
                return;
            }

            node.IsInTicket =
                PrintTicketCapabilityMapper
                    .IsActiveInTicket(ticket, node.Value);

            foreach (var child in node.Children)
            {
                CompareWithTicket(child, ticket);
            }

            // Optional: Parent aktiv, wenn ein Child aktiv ist
            if (!node.IsInTicket)
            {
                node.IsInTicket = node.Children.Any(c => c.IsInTicket);
            }
        }

        /// <summary>
        /// Erzeugt eine lesbare Anzeigedarstellung für einen beliebigen Wert, abhängig von dessen Typ,
        /// z. B. für Enums, Auflistungen, <see cref="PageMediaSize"/>, Objekte mit einer "Name"-Eigenschaft,
        /// primitive Typen und Zeichenfolgen.
        /// </summary>
        /// <param name="value">Der darzustellende Wert.</param>
        /// <returns>
        /// Eine für die Anzeige geeignete Zeichenfolgendarstellung des Werts. Gibt "&lt;null&gt;" zurück,
        /// wenn <paramref name="value"/> <see langword="null"/> ist, oder den Typnamen als Fallback,
        /// wenn keine spezifischere Darstellung ermittelt werden kann.
        /// </returns>
        private static string GetDisplayValue(object value)
        {
            if (value == null)
            {
                return "<null>";
            }

            // Enums
            if (value is Enum val)
            {
                return val.ToString();
            }

            // ReadOnlyCollection / IEnumerable
            if (value is IEnumerable enumerable && value is not string)
            {
                int count = 0;
                foreach (var _ in enumerable)
                {
                    count++;
                }

                return $"{count} item(s)";
            }

            // PageMediaSize
            if (value is PageMediaSize pms)
            {
                return $"{pms.PageMediaSizeName} ({pms.Width} x {pms.Height})";
            }

            // Name-Property (PrintCapabilityOption etc.)
            var type = value.GetType();
            var nameProp = type.GetProperty("Name");
            if (nameProp != null)
            {
                var name = nameProp.GetValue(value);
                if (name != null)
                {
                    return name.ToString();
                }
            }

            // Primitive & string
            if (value is string str)
            {
                return str;
            }

            if (type.IsPrimitive)
            {
                return value.ToString();
            }

            // Fallback
            return type.Name;
        }

        /// <summary>
        /// Baut rekursiv per Reflektion einen <see cref="CapabilityNode"/>-Baum aus einem beliebigen Objekt auf.
        /// Primitive Typen, Zeichenfolgen und Enums werden als Blattknoten behandelt, Auflistungen
        /// (<see cref="IEnumerable"/>) werden elementweise als Kindknoten aufgelöst, und für alle anderen
        /// Objekte werden die öffentlichen Instanzeigenschaften rekursiv als Kindknoten hinzugefügt.
        /// </summary>
        /// <param name="obj">Das Objekt, das in einen Knotenbaum umgewandelt werden soll.</param>
        /// <param name="name">Der Anzeigename, der dem erzeugten Knoten zugewiesen wird.</param>
        /// <returns>
        /// Der erzeugte <see cref="CapabilityNode"/>, der <paramref name="obj"/> und dessen
        /// gegebenenfalls vorhandene Kindstruktur repräsentiert.
        /// </returns>
        /// <remarks>
        /// Wird beim Auslesen einer Eigenschaft eine Ausnahme ausgelöst, so wird anstelle des Werts
        /// ein Fehlerknoten mit Typ "Error" und der Ausnahmemeldung als Anzeigewert hinzugefügt,
        /// und <see cref="CapabilityNode.IsSupported"/> wird auf <see langword="false"/> gesetzt.
        /// </remarks>
        public static CapabilityNode BuildNode(object obj, string name)
        {
            var node = new CapabilityNode
            {
                Name = name,
                Type = obj?.GetType().Name ?? "null",
                Value = obj,
                DisplayValue = GetDisplayValue(obj)
            };

            if (obj == null)
            {
                return node;
            }

            var type = obj.GetType();

            // Primitive / Enum → Leaf
            if (type.IsPrimitive || obj is string || obj is Enum)
            {
                return node;
            }

            // IEnumerable
            if (obj is IEnumerable enumerable && obj is not string)
            {
                int index = 0;
                foreach (var item in enumerable)
                {
                    node.Children.Add(
                        BuildNode(item, $"[{index++}]"));
                }
                return node;
            }

            // Properties
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    var value = prop.GetValue(obj);
                    node.Children.Add(BuildNode(value, prop.Name));
                }
                catch (Exception ex)
                {
                    node.Children.Add(new CapabilityNode
                    {
                        Name = prop.Name,
                        Type = "Error",
                        DisplayValue = $"<exception>{ex.Message}",
                        IsSupported = false   // ✅ nur hier wirklich "nicht unterstützt"
                    });
                }
            }

            return node;
        }
    }
}
