using Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace UseCases.Facturacion;

/// <summary>
/// Spec 020: representación impresa (PDF) simple del CFDI 4.0 timbrado. El proyecto no tenía
/// previamente ninguna librería de generación de PDF/tickets térmicos, así que se incorpora
/// QuestPDF (licencia Community, gratuita para uso comercial con ingresos anuales menores al
/// umbral publicado por QuestPDF; ver https://www.questpdf.com/license/). No implementa el
/// diseño oficial completo de "representación impresa" del SAT (logotipos, cadena original
/// completa en el pie, RFC del PAC certificador, etc.), pero incluye los datos fiscales
/// mínimos indispensables: emisor, receptor, conceptos, impuestos, totales, UUID y sellos.
/// </summary>
public static class FacturaPdfGenerator
{
    static FacturaPdfGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] Generar(FacturaVenta factura, EmpresaConfiguracionPAC? emisor)
    {
        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("Comprobante Fiscal Digital por Internet (CFDI 4.0)").FontSize(16).Bold();
                    col.Item().Text($"Serie {factura.Serie}  Folio {factura.Folio}");
                });

                page.Content().Column(col =>
                {
                    col.Spacing(8);

                    col.Item().Text("Emisor").Bold();
                    col.Item().Text($"RFC: {emisor?.RfcEmisor}");
                    col.Item().Text($"Razón Social: {emisor?.RazonSocialEmisor}");
                    col.Item().Text($"Régimen Fiscal: {emisor?.RegimenFiscalEmisor}   Lugar de Expedición: {emisor?.LugarExpedicionCP}");

                    col.Item().PaddingTop(5).Text("Receptor").Bold();
                    col.Item().Text($"RFC: {factura.RfcReceptor}");
                    col.Item().Text($"Nombre / Razón Social: {factura.NombreReceptor}");
                    col.Item().Text($"Uso CFDI: {factura.UsoCfdi}   Régimen Fiscal: {factura.RegimenFiscalReceptor}   CP: {factura.CodigoPostalReceptor}");

                    col.Item().PaddingTop(5).Text("Conceptos").Bold();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Descripción").Bold();
                            header.Cell().Text("Cant.").Bold();
                            header.Cell().Text("V. Unitario").Bold();
                            header.Cell().Text("Importe").Bold();
                        });

                        foreach (var d in factura.Detalles)
                        {
                            table.Cell().Text(d.Descripcion);
                            table.Cell().Text(d.Cantidad.ToString("F2"));
                            table.Cell().Text(d.ValorUnitario.ToString("C2"));
                            table.Cell().Text(d.Importe.ToString("C2"));
                        }
                    });

                    col.Item().PaddingTop(10).AlignRight().Column(totales =>
                    {
                        totales.Item().Text($"Subtotal: {factura.Subtotal:C2}");
                        totales.Item().Text($"Descuento: {factura.Descuento:C2}");
                        totales.Item().Text($"IVA: {factura.Iva:C2}");
                        totales.Item().Text($"Total: {factura.Total:C2}").Bold();
                    });

                    col.Item().PaddingTop(10).Text("Datos del Timbre Fiscal Digital").Bold();
                    col.Item().Text($"UUID: {factura.UUID}");
                    col.Item().Text($"Fecha de Timbrado: {factura.FechaTimbrado:yyyy-MM-dd HH:mm:ss}");
                    col.Item().Text($"No. Certificado SAT: {factura.NoCertificadoSat}");
                    col.Item().Text("Sello Digital del CFDI:").FontSize(7);
                    col.Item().Text(factura.SelloDigitalEmisor ?? string.Empty).FontSize(6);
                    col.Item().Text("Sello del SAT:").FontSize(7);
                    col.Item().Text(factura.SelloDigitalSat ?? string.Empty).FontSize(6);
                });

                page.Footer().AlignCenter().Text("Este documento es una representación impresa de un CFDI (Spec 020 - MesaFacil).").FontSize(8);
            });
        });

        return documento.GeneratePdf();
    }
}
