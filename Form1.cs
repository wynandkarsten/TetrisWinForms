using System;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisWinForms
{
    public partial class Form1 : Form
    {
        private int[,] grid; // The Tetris grid
        private int gridRows = 20;
        private int gridCols = 10;
        private int cellSize = 30; // Size of each cell
        private int[,] currentBlock; // Current tetromino
        private int blockRow, blockCol; // Position of the current block
        private Timer gameTimer;

        private readonly int[][][] tetrominoes = new int[][][]
        {
            new int[][] // O-shape
            {
                new int[] { 1, 1 },
                new int[] { 1, 1 }
            },
            new int[][] // I-shape
            {
                new int[] { 1, 1, 1, 1 }
            },
            new int[][] // T-shape
            {
                new int[] { 0, 1, 0 },
                new int[] { 1, 1, 1 }
            },
            new int[][] // L-shape
            {
                new int[] { 1, 0 },
                new int[] { 1, 0 },
                new int[] { 1, 1 }
            },
            new int[][] // Z-shape
            {
                new int[] { 1, 1, 0 },
                new int[] { 0, 1, 1 }
            }
        };

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(gridCols * cellSize, gridRows * cellSize);
            this.DoubleBuffered = true; // Prevent flickering during rendering
            this.Paint += Form1_Paint;
            this.KeyDown += Form1_KeyDown;

            InitializeGame();
        }

        private void InitializeGame()
        {
            grid = new int[gridRows, gridCols];
            gameTimer = new Timer
            {
                Interval = 500 // Speed of the game in milliseconds
            };
            gameTimer.Tick += GameTimer_Tick;

            SpawnBlock();
            gameTimer.Start();
        }

        private void SpawnBlock()
        {
            Random random = new Random();
            int shapeIndex = random.Next(tetrominoes.Length);
            currentBlock = GetTetromino(shapeIndex);
            blockRow = 0;
            blockCol = (gridCols - currentBlock.GetLength(1)) / 2;
        }

        private int[,] GetTetromino(int index)
        {
            int rows = tetrominoes[index].Length;
            int cols = tetrominoes[index][0].Length;
            int[,] shape = new int[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    shape[r, c] = tetrominoes[index][r][c];
                }
            }

            return shape;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw the grid
            for (int y = 0; y < gridRows; y++)
            {
                for (int x = 0; x < gridCols; x++)
                {
                    if (grid[y, x] == 1)
                    {
                        g.FillRectangle(Brushes.Blue, x * cellSize, y * cellSize, cellSize, cellSize);
                    }
                    g.DrawRectangle(Pens.Black, x * cellSize, y * cellSize, cellSize, cellSize);
                }
            }

            // Draw the current block
            for (int r = 0; r < currentBlock.GetLength(0); r++)
            {
                for (int c = 0; c < currentBlock.GetLength(1); c++)
                {
                    if (currentBlock[r, c] == 1)
                    {
                        g.FillRectangle(Brushes.Red, (blockCol + c) * cellSize, (blockRow + r) * cellSize, cellSize, cellSize);
                        g.DrawRectangle(Pens.Black, (blockCol + c) * cellSize, (blockRow + r) * cellSize, cellSize, cellSize);
                    }
                }
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MoveBlockDown();
            Refresh();
        }

        private void MoveBlockDown()
        {
            if (CanMove(blockRow + 1, blockCol))
            {
                blockRow++;
            }
            else
            {
                MergeBlock();
                CheckForCompletedRows();
                SpawnBlock();
                if (!CanMove(blockRow, blockCol))
                {
                    GameOver();
                }
            }
        }

        private bool CanMove(int newRow, int newCol)
        {
            for (int r = 0; r < currentBlock.GetLength(0); r++)
            {
                for (int c = 0; c < currentBlock.GetLength(1); c++)
                {
                    if (currentBlock[r, c] == 1)
                    {
                        int gridRow = newRow + r;
                        int gridCol = newCol + c;

                        if (gridRow < 0 || gridRow >= gridRows ||
                            gridCol < 0 || gridCol >= gridCols ||
                            grid[gridRow, gridCol] == 1)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void MergeBlock()
        {
            for (int r = 0; r < currentBlock.GetLength(0); r++)
            {
                for (int c = 0; c < currentBlock.GetLength(1); c++)
                {
                    if (currentBlock[r, c] == 1)
                    {
                        grid[blockRow + r, blockCol + c] = 1;
                    }
                }
            }
        }

        private void CheckForCompletedRows()
        {
            for (int r = gridRows - 1; r >= 0; r--)
            {
                bool isComplete = true;
                for (int c = 0; c < gridCols; c++)
                {
                    if (grid[r, c] == 0)
                    {
                        isComplete = false;
                        break;
                    }
                }

                if (isComplete)
                {
                    ClearRow(r);
                    r++; // Recheck the same row after clearing
                }
            }
        }

        private void ClearRow(int row)
        {
            for (int r = row; r > 0; r--)
            {
                for (int c = 0; c < gridCols; c++)
                {
                    grid[r, c] = grid[r - 1, c];
                }
            }

            for (int c = 0; c < gridCols; c++)
            {
                grid[0, c] = 0;
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over!");
            InitializeGame();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (CanMove(blockRow, blockCol - 1))
                        blockCol--;
                    break;

                case Keys.Right:
                    if (CanMove(blockRow, blockCol + 1))
                        blockCol++;
                    break;

                case Keys.Down:
                    if (CanMove(blockRow + 1, blockCol))
                        blockRow++;
                    break;
            }

            Refresh();
        }
    }
}
