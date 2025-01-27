﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TetrisWinForms;


namespace TetrisWinForms
{
    enum Tetromino
    {
        OShape,
        IShape,
        TShape,
        LShape,
        ZShape,
        SShape
    }

    enum TetrominoColor
    {
        Blue,
        Red,
        Green,
        Yellow,
        Purple,
        Cyan
    }

    public partial class Form1 : Form
    {
        private int[,] grid; // The Tetris grid
        private int gridRows = 20;
        private int gridCols = 10;
        private int cellSize = 30; // Size of each cell
        private int[,] currentBlock; // Current tetromino
        private int blockRow, blockCol; // Position of the current block
        private Timer gameTimer;
        private int blockType;
        private int score = 0;

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
            },
            new int[][] // S-shape
            {
                new int[] { 0, 1, 1 },
                new int[] { 1, 1, 0 }
            },
            new int[][] // Reverse L-Shape
            {
                new int[] { 0, 1 },
                new int[] { 0, 1 },
                new int[] { 1, 1 }
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
            score = 0;
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
            score += 5;
            Random random = new Random();
            int shapeIndex = random.Next(tetrominoes.Length);
            blockType = shapeIndex;
            currentBlock = GetTetromino(shapeIndex);
            blockRow = 0;

            //blockCol = (gridCols - currentBlock.GetLength(1)) / 2;
            Random randomCol = new Random();
            blockCol = randomCol.Next(1, gridCols - currentBlock.GetLength(1) - 1);

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
            Console.WriteLine(shape);
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
                        switch (blockType)
                        {   // O-Piece
                            case 0:
                                g.FillRectangle(Brushes.Blue, (blockCol + c) * cellSize, (blockRow + r) * cellSize, cellSize, cellSize);
                                break;
                            // I Piece
                            case 1:
                                g.FillRectangle(Brushes.Red, (blockCol + c) * cellSize, (blockRow + r) * cellSize, cellSize, cellSize);
                                break;
                            // T Piece
                            case 2:
                                g.FillRectangle(Brushes.Yellow, (blockCol + c) * cellSize, (blockRow + r) * cellSize, cellSize, cellSize);
                                break;
                            // L-Piece
                            case 3:
                                g.FillRectangle(Brushes.Purple, (blockCol + c) * cellSize, (blockRow + r) * cellSize, cellSize, cellSize);
                                break;
                            // Z Piece
                            case 4:
                                g.FillRectangle(Brushes.DarkCyan, (blockCol + c) * cellSize, (blockRow + r) * cellSize, cellSize, cellSize);
                                break;
                            // S Piece
                            case 5:
                                g.FillRectangle(Brushes.Green, (blockCol + c) * cellSize, (blockRow + r) * cellSize, cellSize, cellSize);
                                break;
                            // Reverse L-Piece
                            case 6:
                                g.FillRectangle(Brushes.White, (blockCol + c) * cellSize, (blockRow + r) * cellSize, cellSize, cellSize);
                                break;
                            default:
                                g.FillRectangle(Brushes.Gray, (blockCol + c) * cellSize, (blockRow + r) * cellSize, cellSize, cellSize);
                                break;
                        }
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
            int clearedRows = 0;
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
                    clearedRows++;
                    r++; // Recheck the same row after clearing
                }
            }
            if (clearedRows == 1)
            {
                score += 100 * clearedRows;
            }
            else if (clearedRows == 2)
            {
                score += 200 * clearedRows;
            }
            else if (clearedRows == 3)
            {
                score += 500 * clearedRows;
            }
            else if (clearedRows == 4)
            {
                score += 1000 * clearedRows;
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
            MessageBox.Show($"Your score: {score}. Press any key to restart.");
            InitializeGame();
        }

        private void GameOver_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'y' || e.KeyChar == 'Y')
            {
                // Restart the game
                InitializeGame();

                // Detach the event handler to avoid duplicate handling
                this.KeyPress -= GameOver_KeyPress;
            }
            else if (e.KeyChar == 'n' || e.KeyChar == 'N')
            {
                // Quit the game
                this.KeyPress -= GameOver_KeyPress;
                this.Close(); // Closes the application
            }
        }

        private bool isPaused = false; // Track whether the game is paused

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isPaused && e.KeyCode != Keys.Escape)
            {
                // Ignore all keys except ESC when the game is paused
                return;
            }

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

                case Keys.Up:
                    RotateBlock();
                    break;

            }

            Refresh(); // Repaint the game board
        }

        private void GamePause()
        {
            if (!isPaused)
            {
                isPaused = true; // Set the game state to paused
                gameTimer.Stop(); // Stop the timer
                MessageBox.Show("Game Paused. Press ESC to resume."); // Provide a clear message
            }
            else
            {
                ResumeGame();
            }
        }

        private void ResumeGame()
        {
            isPaused = false; // Set the game state to running
            gameTimer.Start(); // Restart the timer
        }

        private void RotateBlock()
        {
            int[,] rotatedBlock = RotateMatrixClockwise(currentBlock);

            // Check if the rotated block fits in the grid
            if (CanRotate(rotatedBlock))
            {
                currentBlock = rotatedBlock;
            }
            else
            {
                // Optionally, play a sound or indicate rotation failure (if desired)
            }

            Refresh(); // Redraw the game to show the rotated block
        }

        private int[,] RotateMatrixClockwise(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] rotated = new int[cols, rows];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    rotated[c, rows - 1 - r] = matrix[r, c];
                }
            }

            return rotated;
        }

        private bool CanRotate(int[,] rotatedBlock)
        {
            for (int r = 0; r < rotatedBlock.GetLength(0); r++)
            {
                for (int c = 0; c < rotatedBlock.GetLength(1); c++)
                {
                    if (rotatedBlock[r, c] == 1)
                    {
                        int newRow = blockRow + r;
                        int newCol = blockCol + c;

                        // Check bounds
                        if (newRow < 0 || newRow >= gridRows || newCol < 0 || newCol >= gridCols)
                        {
                            return false;
                        }

                        // Check collision with the grid
                        if (grid[newRow, newCol] == 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

    }
}
