using System;
using System.Drawing;
using System.Windows.Forms;

namespace demo_TTNT
{
    public partial class Form1 : Form
    {
        private const int Cols = 9;
        private const int Rows = 10;
        private const int Cell = 64;
        private const int Margin = 20;

        private Piece?[,] board = new Piece[Rows, Cols];
        private bool redTurn = true;
        private (int row, int col)? selected;

        private Font pieceFont = new Font("Microsoft YaHei", 22, FontStyle.Bold);
        private Font riverFont = new Font("Microsoft YaHei", 26, FontStyle.Bold);

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            boardPanel.Width = Cols * Cell + Margin * 2;
            boardPanel.Height = Rows * Cell + Margin * 2;
            StartNewGame();
        }

        private void StartNewGame()
        {
            selected = null;
            redTurn = true;
            SetupInitialPosition();
            UpdateStatus();
            boardPanel.Invalidate();
        }

        private void SetupInitialPosition()
        {
            // clear
            board = new Piece[Rows, Cols];

            // Black (top) back row - row 0
            board[0, 0] = new Piece(PieceType.Chariot, false);
            board[0, 1] = new Piece(PieceType.Horse, false);
            board[0, 2] = new Piece(PieceType.Elephant, false);
            board[0, 3] = new Piece(PieceType.Advisor, false);
            board[0, 4] = new Piece(PieceType.General, false);
            board[0, 5] = new Piece(PieceType.Advisor, false);
            board[0, 6] = new Piece(PieceType.Elephant, false);
            board[0, 7] = new Piece(PieceType.Horse, false);
            board[0, 8] = new Piece(PieceType.Chariot, false);

            // Black cannons
            board[2, 1] = new Piece(PieceType.Cannon, false);
            board[2, 7] = new Piece(PieceType.Cannon, false);

            // Black soldiers
            for (int c = 0; c <= 8; c += 2)
                board[3, c] = new Piece(PieceType.Soldier, false);

            // Red (bottom) back row - row 9
            board[9, 0] = new Piece(PieceType.Chariot, true);
            board[9, 1] = new Piece(PieceType.Horse, true);
            board[9, 2] = new Piece(PieceType.Elephant, true);
            board[9, 3] = new Piece(PieceType.Advisor, true);
            board[9, 4] = new Piece(PieceType.General, true);
            board[9, 5] = new Piece(PieceType.Advisor, true);
            board[9, 6] = new Piece(PieceType.Elephant, true);
            board[9, 7] = new Piece(PieceType.Horse, true);
            board[9, 8] = new Piece(PieceType.Chariot, true);

            // Red cannons
            board[7, 1] = new Piece(PieceType.Cannon, true);
            board[7, 7] = new Piece(PieceType.Cannon, true);

            // Red soldiers
            for (int c = 0; c <= 8; c += 2)
                board[6, c] = new Piece(PieceType.Soldier, true);
        }

        private void UpdateStatus()
        {
            lblStatus.Text = (redTurn ? "Red's turn" : "Black's turn") + Environment.NewLine + "Click a piece to select, click destination to move.";
        }

        private void boardPanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // background
            using var bg = new SolidBrush(Color.Beige);
            g.FillRectangle(bg, 0, 0, boardPanel.Width, boardPanel.Height);

            // draw grid
            Pen gridPen = Pens.Black;
            for (int r = 0; r < Rows; r++)
            {
                int y = Margin + r * Cell;
                g.DrawLine(gridPen, Margin, y, Margin + (Cols - 1) * Cell, y);
            }
            for (int c = 0; c < Cols; c++)
            {
                int x = Margin + c * Cell;
                g.DrawLine(gridPen, x, Margin, x, Margin + (Rows - 1) * Cell);
            }

            // Draw river text between rows 4 and 5
            string left = "楚河";
            string right = "漢界";
            SizeF leftSize = g.MeasureString(left, riverFont);
            SizeF rightSize = g.MeasureString(right, riverFont);
            float centerX = Margin + (Cols - 1) * Cell / 2f;
            float riverY = Margin + 4 * Cell + Cell / 2f - leftSize.Height / 2f;
            g.DrawString(left, riverFont, Brushes.DarkBlue, centerX - 8 - leftSize.Width, riverY);
            g.DrawString(right, riverFont, Brushes.DarkBlue, centerX + 8, riverY);

            // draw pieces
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    var p = board[r, c];
                    if (p != null)
                    {
                        DrawPiece(g, r, c, p);
                    }
                }
            }

            // highlight selection
            if (selected.HasValue)
            {
                var s = selected.Value;
                int x = Margin + s.col * Cell;
                int y = Margin + s.row * Cell;
                Rectangle rect = new Rectangle(x - Cell / 2 + Cell / 2, y - Cell / 2 + Cell / 2, Cell, Cell);
                using var selPen = new Pen(Color.Magenta, 3);
                g.DrawRectangle(selPen, Margin + s.col * Cell - Cell / 2 + Cell / 2, Margin + s.row * Cell - Cell / 2 + Cell / 2, Cell, Cell);
            }
        }

        private void DrawPiece(Graphics g, int row, int col, Piece p)
        {
            int cx = Margin + col * Cell;
            int cy = Margin + row * Cell;
            int radius = (int)(Cell * 0.8);
            Rectangle rect = new Rectangle(cx - radius / 2, cy - radius / 2, radius, radius);

            Color fill = p.IsRed ? Color.LightCoral : Color.LightGray;
            Color border = Color.Black;
            using var brush = new SolidBrush(fill);
            using var pen = new Pen(border, 2);
            g.FillEllipse(brush, rect);
            g.DrawEllipse(pen, rect);

            string text = p.Symbol;
            var textBrush = p.IsRed ? Brushes.DarkRed : Brushes.Black;
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(text, pieceFont, textBrush, rect, sf);
        }

        private void boardPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            var hit = ScreenToCell(e.Location);
            if (hit == null) return;
            int r = hit.Value.row, c = hit.Value.col;

            var clickedPiece = board[r, c];
            if (selected == null)
            {
                // select piece if exists and belongs to current player
                if (clickedPiece != null && clickedPiece.IsRed == redTurn)
                {
                    selected = (r, c);
                    boardPanel.Invalidate();
                }
            }
            else
            {
                var s = selected.Value;
                // if clicked own piece -> switch selection
                if (clickedPiece != null && clickedPiece.IsRed == redTurn)
                {
                    selected = (r, c);
                    boardPanel.Invalidate();
                    return;
                }

                // perform move (no legality checking in this demo)
                board[r, c] = board[s.row, s.col];
                board[s.row, s.col] = null;
                selected = null;
                redTurn = !redTurn;
                UpdateStatus();
                boardPanel.Invalidate();
            }
        }

        private (int row, int col)? ScreenToCell(Point p)
        {
            int x = p.X - Margin;
            int y = p.Y - Margin;
            if (x < -Cell / 2 || y < -Cell / 2) return null;
            int col = (int)Math.Round(x / (double)Cell);
            int row = (int)Math.Round(y / (double)Cell);
            if (row < 0 || row >= Rows || col < 0 || col >= Cols) return null;
            return (row, col);
        }

        private void btnReset_Click(object? sender, EventArgs e)
        {
            StartNewGame();
        }
    }

    enum PieceType
    {
        General,
        Advisor,
        Elephant,
        Horse,
        Chariot,
        Cannon,
        Soldier
    }

    class Piece
    {
        public PieceType Type { get; }
        public bool IsRed { get; }
        public string Symbol => GetSymbol();

        public Piece(PieceType type, bool isRed)
        {
            Type = type;
            IsRed = isRed;
        }

        private string GetSymbol()
        {
            // red / black Chinese characters
            return Type switch
            {
                PieceType.General => IsRed ? "帥" : "將",
                PieceType.Advisor => IsRed ? "仕" : "士",
                PieceType.Elephant => IsRed ? "相" : "象",
                PieceType.Horse => "馬",
                PieceType.Chariot => "車",
                PieceType.Cannon => "砲",
                PieceType.Soldier => IsRed ? "兵" : "卒",
                _ => "?"
            };
        }
    }
}