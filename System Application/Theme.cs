using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Corner_Application
{
    /// <summary>
    /// Central "warm coffee" visual theme for the whole application.
    ///
    /// Rather than hand-editing hundreds of properties across every WinForms
    /// designer, each form calls <see cref="Apply"/> once (right after
    /// InitializeComponent). Apply walks the control tree and restyles controls by
    /// type — giving a consistent, modern, flat look (cream backgrounds, caramel
    /// flat buttons, Segoe UI, styled grids) across all screens. On Windows 11 it
    /// also tints the native title bar espresso via DWM.
    /// </summary>
    public static class Theme
    {
        // Palette
        public static readonly Color Espresso = ColorTranslator.FromHtml("#3E2723"); // headers, title bar, grid header
        public static readonly Color Mocha    = ColorTranslator.FromHtml("#6F4E37"); // button hover
        public static readonly Color Caramel  = ColorTranslator.FromHtml("#C8894B"); // primary accent / buttons
        public static readonly Color Cream    = ColorTranslator.FromHtml("#F5EFE6"); // window background
        public static readonly Color Card     = ColorTranslator.FromHtml("#FFFDF9"); // inputs / cards
        public static readonly Color TextDark = ColorTranslator.FromHtml("#2B211C"); // body text
        public static readonly Color Border   = ColorTranslator.FromHtml("#E3D9CC"); // subtle borders
        public static readonly Color Danger   = ColorTranslator.FromHtml("#B03A2E"); // delete/withdraw actions

        public static readonly string FontFamily = "Segoe UI";

        /// <summary>Apply the theme to a form and every control it contains.</summary>
        public static void Apply(Form form)
        {
            if (form == null) return;
            form.BackColor = Cream;
            form.ForeColor = TextDark;
            try { form.Font = new Font(FontFamily, 9.75f, FontStyle.Regular); } catch { }

            StyleChildren(form);

            // Tint the Windows 11 title bar once the handle exists.
            form.HandleCreated += (s, e) => TrySetTitleBar(form.Handle, Espresso, Cream);
            if (form.IsHandleCreated) TrySetTitleBar(form.Handle, Espresso, Cream);
        }

        private static void StyleChildren(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                StyleOne(c);
                if (c.HasChildren) StyleChildren(c);
            }
        }

        private static void StyleOne(Control c)
        {
            var btn = c as Button;
            if (btn != null)
            {
                bool isDanger = LooksLikeDanger(btn.Text);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.BackColor = isDanger ? Danger : Caramel;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = isDanger ? ControlPaint.Dark(Danger) : Mocha;
                btn.FlatAppearance.MouseDownBackColor = Espresso;
                btn.UseVisualStyleBackColor = false;
                btn.Cursor = Cursors.Hand;
                try { btn.Font = new Font(FontFamily, 9.75f, FontStyle.Bold); } catch { }
                return;
            }

            var tb = c as TextBox;
            if (tb != null)
            {
                tb.BorderStyle = BorderStyle.FixedSingle;
                tb.BackColor = Card;
                tb.ForeColor = TextDark;
                return;
            }

            var cb = c as ComboBox;
            if (cb != null)
            {
                cb.FlatStyle = FlatStyle.Flat;
                cb.BackColor = Card;
                cb.ForeColor = TextDark;
                return;
            }

            var nud = c as NumericUpDown;
            if (nud != null)
            {
                nud.BorderStyle = BorderStyle.FixedSingle;
                nud.BackColor = Card;
                nud.ForeColor = TextDark;
                return;
            }

            var lbl = c as Label;
            if (lbl != null)
            {
                lbl.BackColor = Color.Transparent;
                // Bigger labels read as headings -> espresso; body text -> dark.
                lbl.ForeColor = (lbl.Font != null && lbl.Font.Size >= 14f) ? Espresso : TextDark;
                return;
            }

            var gb = c as GroupBox;
            if (gb != null)
            {
                gb.ForeColor = Espresso;
                return;
            }

            var grid = c as DataGridView;
            if (grid != null)
            {
                StyleGrid(grid);
                return;
            }

            var panel = c as Panel;
            if (panel != null)
            {
                // Keep panels on the cream surface unless they carry a background image.
                if (panel.BackgroundImage == null) panel.BackColor = Cream;
                return;
            }
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = Cream;
            grid.GridColor = Border;
            grid.RowHeadersVisible = false;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Espresso;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Cream;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Espresso;
            try { grid.ColumnHeadersDefaultCellStyle.Font = new Font(FontFamily, 9.75f, FontStyle.Bold); } catch { }
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            grid.ColumnHeadersHeight = 32;

            grid.DefaultCellStyle.BackColor = Card;
            grid.DefaultCellStyle.ForeColor = TextDark;
            grid.DefaultCellStyle.SelectionBackColor = Caramel;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Cream;
            grid.RowTemplate.Height = 28;
        }

        private static bool LooksLikeDanger(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            string t = text.ToLowerInvariant();
            return t.Contains("delete") || t.Contains("remove") || t.Contains("withdraw")
                || t.Contains("cancel") || t.Contains("حذف") || t.Contains("مسح")
                || t.Contains("سحب") || t.Contains("الغاء") || t.Contains("إلغاء");
        }

        // ---- Windows 11 title-bar tint (best-effort; no-op on older Windows) ----
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

        private static void TrySetTitleBar(IntPtr hwnd, Color caption, Color text)
        {
            try
            {
                int cap = ToColorRef(caption);
                int txt = ToColorRef(text);
                DwmSetWindowAttribute(hwnd, DWMWA_CAPTION_COLOR, ref cap, sizeof(int));
                DwmSetWindowAttribute(hwnd, DWMWA_TEXT_COLOR, ref txt, sizeof(int));
            }
            catch
            {
                // Pre-Windows-11: attribute unsupported, ignore.
            }
        }

        private static int ToColorRef(Color c)
        {
            // COLORREF = 0x00BBGGRR
            return (c.B << 16) | (c.G << 8) | c.R;
        }
    }
}
