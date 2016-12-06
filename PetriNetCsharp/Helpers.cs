﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace PetriNetCsharp
{
    public static class Helpers
    {
        public static void FillWholeDgvWithZeros(DataGridView target)
        {
            try
            {
                for (int i = 0; i < target.RowCount; i++)
                {
                    for (int j = 0; j < target.ColumnCount; j++)
                    {
                        target.Rows[i].Cells[j].Value = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Raport(ex);
            }

        }

        public static void FillNullsInDgvWithZeros(DataGridView target)
        {

            for (int i = 0; i < target.RowCount; i++)
            {
                for (int j = 0; j < target.ColumnCount; j++)
                {
                    if (target.Rows[i].Cells[j].Value == null)
                    {
                        target.Rows[i].Cells[j].Value = 0;
                    }
                }
            }
        }


        public static List<int> AddMatrixElementWise(List<int> a, List<int> b)
        {
            try
            {
                List<int> result = new List<int>(a.Count);
                for (int i = 0; i < a.Count; i++)
                {
                    result[i] = a[i] + b[i];
                }
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public static void ImportMatrix2DToDataGridView(List<List<int>> source, DataGridView target)
        {
            try
            {
                for (int i = 0; i < source.Count; i++)
                {
                    for (int j = 0; j < source[0].Count; j++)
                    {
                        target.Rows[i].Cells[j].Value = source[i][j].ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                Raport(ex);
            }
        }

        public static void FillListWithValue(List<int> target, int val)
        {
            for (int i = 0; i < target.Capacity; i++)
            {
                target.Add(val);
            }
        }
        public static void FillListWithValue(List<List<int>> target, List<int> val)
        {
            for (int i = 0; i < target.Capacity; i++)
            {
                target.Add(val);
            }
        }
        public static List<List<int>> Matrix2DMultiply(List<List<int>> A, List<List<int>> B)
        {
            try
            {
                int rowsA = A.Count;
                int colsA = A[0].Count;
                int rowsB = B.Count;
                int colsB = B[0].Count;
                if (colsA != rowsB)
                {
                    throw new System.InvalidOperationException("Liczba kolumna Macierzy A\n rozna od liczby wierszy macierzy B");
                }
                List<int> tmp1D = new List<int>(colsB);
                FillListWithValue(tmp1D, 0);
                List<List<int>> result = new List<List<int>>(rowsA);
                FillListWithValue(result, tmp1D);

                for (int row = 0; row < rowsA; row++)
                {
                    for (int col = 0; col < colsB; col++)
                    {
                        // Multiply the row of A by the column of B to get the row, column of product.  
                        for (int inner = 0; inner < rowsB; inner++)
                        {
                            result[row][col] += A[row][inner] * B[inner][col];
                        }
                    }
                }
                return result;
            }
            catch (InvalidOperationException ex)
            {
                Raport(ex);
                return null;
            }
            catch (Exception ex)
            {
                Raport(ex);
                throw;
            }

        }

        public static List<int> AddElementWise(List<int> a, List<int> b)
        {
            try
            {
                List<int> result = new List<int>(a.Count);
                for (int i = 0; i < a.Count; i++)
                {
                    result.Add(a[i] + b[i]);
                }
                return result;
            }
            catch (Exception ex)
            {
                Raport(ex); return null;
            }

        }
        public static List<List<int>> AddElementWise(List<List<int>> a, List<List<int>> b)
        {
            try
            {
                int numRows = a.Count;
                int numCols = a[0].Count;


                List<List<int>> result = new List<List<int>>(numRows);
                List<int> resultRow = new List<int>(numCols);
                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        resultRow.Add(a[i].ElementAt(j) + b[i].ElementAt(j));
                    }
                    result.Add(resultRow);
                    resultRow = new List<int>(numCols);
                }
                return result;
            }
            catch (Exception ex)
            {
                Raport(ex); return null;
            }

        }

        public static List<int> SubtractElementWise(List<int> a, List<int> b)
        {
            try
            {
                List<int> result = new List<int>(a.Count);
                for (int i = 0; i < a.Count; i++)
                {
                    result.Add(a[i] - b[i]);
                }
                return result;
            }
            catch (Exception ex)
            {
                Raport(ex); return null;
            }

        }
        public static List<List<int>> SubtractElementWise(List<List<int>> a, List<List<int>> b)
        {
            try
            {
                int numRows = a.Count;
                int numCols = a[0].Count;


                List<List<int>> result = new List<List<int>>(numRows);
                List<int> resultRow = new List<int>(numCols);
                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        resultRow.Add(a[i].ElementAt(j) - b[i].ElementAt(j));
                    }
                    result.Add(resultRow);
                    resultRow = new List<int>(numCols);
                }
                return result;
            }
            catch (Exception ex)
            {
                Raport(ex); return null;
            }

        }


        public static void Raport(Exception ex)
        {
            MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
        }
    }
}