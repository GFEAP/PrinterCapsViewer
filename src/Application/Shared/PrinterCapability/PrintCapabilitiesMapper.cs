// -----------------------------------------------------------------------
// <copyright file="PrintCapabilitiesMapper.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using System.Collections;
using System.Printing;

namespace PrinterCapsViewer.Shared.PrinterCapability
{
    public static class PrintTicketCapabilityMapper
    {
        /// <summary>
        /// Prüft, ob ein bestimmter Capability-Wert im angegebenen <see cref="PrintTicket"/> aktiv ist.
        /// </summary>
        /// <remarks>
        /// Implementiert für Container-Werte (z. B. <c>ReadOnlyCollection&lt;T&gt;</c>), die
        /// <see cref="IEnumerable"/> implementieren, aber keine <see cref="string"/>-Instanzen sind,
        /// eine rekursive Prüfung: Es wird jedes Element einzeln geprüft, und die Methode liefert
        /// <see langword="true"/> zurück, sobald mindestens ein Element im Ticket aktiv ist.
        /// Für einzelne Werte wird der konkrete Capability-Typ (z. B. <see cref="Duplexing"/>, 
        /// <see cref="PageOrientation"/>, <see cref="PageMediaSize"/> usw.) per Pattern Matching
        /// erkannt und mit der entsprechenden Eigenschaft des Tickets verglichen.
        /// </remarks>
        /// <param name="ticket">
        /// Das <see cref="PrintTicket"/>, gegen das der Capability-Wert geprüft werden soll.
        /// </param>
        /// <param name="capabilityValue">
        /// Der zu prüfende Capability-Wert. Kann ein einzelner Wert oder eine
        /// <see cref="IEnumerable"/>-Sammlung von Werten sein.
        /// </param>
        /// <returns>
        /// <see langword="true"/>, wenn der angegebene Capability-Wert (bzw. mindestens ein Element
        /// einer übergebenen Sammlung) im <paramref name="ticket"/> aktiv ist; andernfalls
        /// <see langword="false"/>. Ist <paramref name="ticket"/> oder <paramref name="capabilityValue"/>
        /// <see langword="null"/>, wird ebenfalls <see langword="false"/> zurückgegeben.
        /// </returns>
        public static bool IsActiveInTicket(
            PrintTicket ticket,
            object capabilityValue)
        {
            if (ticket == null || capabilityValue == null)
                return false;

            // Container-Capabilities (ReadOnlyCollection<T>) rekursiv prüfen
            if (capabilityValue is IEnumerable enumerable && capabilityValue is not string)
            {
                foreach (var item in enumerable)
                {
                    if (IsActiveInTicket(ticket, item))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (capabilityValue is Collation collation)
            {
                return ticket.Collation == collation ||
                       (ticket.Collation != null && ticket.Collation.Value == collation);
            }

            return capabilityValue switch
            {
                Duplexing d =>
                    ticket.Duplexing == d,
                PageOrientation o =>
                    ticket.PageOrientation == o,
                OutputColor c =>
                    ticket.OutputColor == c,
                Collation q =>
                    ticket.Collation == q,
                DeviceFontSubstitution device =>
                    ticket.DeviceFontSubstitution == device,
                InputBin input =>
                    ticket.InputBin == input,
                OutputQuality outputquality =>
                    ticket.OutputQuality == outputquality,
                PageBorderless pageBorderless =>
                    ticket.PageBorderless == pageBorderless,
                PageMediaSize pageMediaSize =>
                     ticket.PageMediaSize != null &&
                     ticket.PageMediaSize.PageMediaSizeName == pageMediaSize.PageMediaSizeName,
                PageMediaType pageMediaType =>
                    ticket.PageMediaType == pageMediaType,
                PageOrder pageOrder =>
                    ticket.PageOrder == pageOrder,
                PageResolution pageResolution =>
                    ticket.PageResolution != null &&
                    ticket.PageResolution.QualitativeResolution == pageResolution.QualitativeResolution,
                PagesPerSheetDirection pagesPerSheetDirection =>
                    ticket.PagesPerSheetDirection == pagesPerSheetDirection,
                PhotoPrintingIntent photoPrintingIntent =>
                    ticket.PhotoPrintingIntent == photoPrintingIntent,
                Stapling stapling =>
                    ticket.Stapling == stapling,
                TrueTypeFontMode trueTypeFontMode =>
                    ticket.TrueTypeFontMode == trueTypeFontMode,
                _ => false
            };
        }
    }
}

