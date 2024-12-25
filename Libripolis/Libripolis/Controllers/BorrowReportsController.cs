using Microsoft.AspNetCore.Mvc;
using Libripolis.Data;
using Libripolis.Models;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Fonts;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Libripolis.Controllers
{
    public class BorrowReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowReportsController(ApplicationDbContext context)
        {
            _context = context;

            // Rejestracja CustomFontResolver jako dostawcy czcionek
            GlobalFontSettings.FontResolver = new CustomFontResolver();
        }

        public async Task<IActionResult> Index()
        {
            var borrowReports = await _context.Borrow.Include(b => b.Book).Include(b => b.user).ToListAsync();
            return View(borrowReports);
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePdf()
        {
            var borrows = await _context.Borrow
                                        .Include(b => b.Book)
                                        .Include(b => b.user)
                                        .ToListAsync();

            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Borrow Report";
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont headerFont = new XFont("Arial", 14, XFontStyle.Bold);
                XFont font = new XFont("Arial", 12, XFontStyle.Regular);

                // Ustawienia tabeli
                double margin = 10;
                double yPosition = 40;
                double rowHeight = 20;
                double columnWidth = page.Width / 4 - margin;

                // Rysowanie nagłówka dokumentu
                gfx.DrawString("Borrow Report", new XFont("Arial", 16, XFontStyle.Bold), XBrushes.Black, margin, yPosition);
                yPosition += 30;

                // Rysowanie nagłówków tabeli
                gfx.DrawRectangle(XPens.Black, margin, yPosition, columnWidth, rowHeight);
                gfx.DrawString("Book Title", headerFont, XBrushes.Black, new XRect(margin, yPosition, columnWidth, rowHeight), XStringFormats.Center);

                gfx.DrawRectangle(XPens.Black, margin + columnWidth, yPosition, columnWidth, rowHeight);
                gfx.DrawString("User", headerFont, XBrushes.Black, new XRect(margin + columnWidth, yPosition, columnWidth, rowHeight), XStringFormats.Center);

                gfx.DrawRectangle(XPens.Black, margin + 2 * columnWidth, yPosition, columnWidth, rowHeight);
                gfx.DrawString("Start Date", headerFont, XBrushes.Black, new XRect(margin + 2 * columnWidth, yPosition, columnWidth, rowHeight), XStringFormats.Center);

                gfx.DrawRectangle(XPens.Black, margin + 3 * columnWidth, yPosition, columnWidth, rowHeight);
                gfx.DrawString("End Date", headerFont, XBrushes.Black, new XRect(margin + 3 * columnWidth, yPosition, columnWidth, rowHeight), XStringFormats.Center);

                yPosition += rowHeight;

                // Wypełnianie tabeli danymi
                foreach (var borrow in borrows)
                {
                    gfx.DrawRectangle(XPens.Black, margin, yPosition, columnWidth, rowHeight);
                    gfx.DrawString(borrow.Book.Title, font, XBrushes.Black, new XRect(margin, yPosition, columnWidth, rowHeight), XStringFormats.CenterLeft);

                    gfx.DrawRectangle(XPens.Black, margin + columnWidth, yPosition, columnWidth, rowHeight);
                    gfx.DrawString(borrow.user?.Email ?? "Unknown", font, XBrushes.Black, new XRect(margin + columnWidth, yPosition, columnWidth, rowHeight), XStringFormats.CenterLeft);

                    gfx.DrawRectangle(XPens.Black, margin + 2 * columnWidth, yPosition, columnWidth, rowHeight);
                    gfx.DrawString(borrow.Start.ToShortDateString(), font, XBrushes.Black, new XRect(margin + 2 * columnWidth, yPosition, columnWidth, rowHeight), XStringFormats.Center);

                    gfx.DrawRectangle(XPens.Black, margin + 3 * columnWidth, yPosition, columnWidth, rowHeight);
                    gfx.DrawString(borrow.End.ToShortDateString(), font, XBrushes.Black, new XRect(margin + 3 * columnWidth, yPosition, columnWidth, rowHeight), XStringFormats.Center);

                    yPosition += rowHeight;

                    // Przejście do nowej strony, jeśli brakuje miejsca
                    if (yPosition + rowHeight > page.Height - margin)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPosition = margin;
                    }
                }

                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf", "BorrowReport.pdf");
            }
        }
    }
}
