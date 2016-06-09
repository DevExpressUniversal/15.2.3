#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Diagnostics;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionMInverse
	public class FunctionMInverse : WorksheetFunctionSingleArgumentBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array));
			return collection;
		}
		#endregion
		public override string Name { get { return "MINVERSE"; } }
		public override int Code { get { return 0x00A4; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Array; } }
		protected override VariantValue EvaluateArgument(VariantValue argument, WorkbookDataContext context) {
			if (argument.IsBoolean || argument.IsEmpty || argument.IsText)
				return VariantValue.ErrorInvalidValueInFunction;
			if (argument.IsError)
				return argument;
			return GetResult(argument);
		}
		VariantValue GetResult(VariantValue value) {
			if (value.IsArray && value.ArrayValue.Height != value.ArrayValue.Width)
				return VariantValue.ErrorInvalidValueInFunction;
			return GetResultCore(new SquareMatrix(value));
		}
		VariantValue GetResultCore(SquareMatrix matrix) {
			return matrix.GetInverseMatrix();
		}
	}
	#endregion
	#region SquareMatrix
	public class SquareMatrix {
		#region Static Members
		public static SquareMatrix GetIdentityMatrix(int dimension) {
			SquareMatrix result = new SquareMatrix(dimension);
			for (int i = 0; i < dimension; i++)
				result[i, i] = 1;
			return result;
		}
		public static double[,] TryGetInverse(double[,] sourceArray) {
			if (sourceArray == null)
				return null;
			int dimension = sourceArray.GetLength(0);
			if (dimension == 0 || dimension != sourceArray.GetLength(1))
				return null;
			SquareMatrix matrix = new SquareMatrix(sourceArray);
			SquareMatrix result;
			return matrix.TryGetInverseMatrixCore(out result) ? result.sourceArray : null;
		}
		#endregion
		#region Fields
		double[,] sourceArray;
		VariantValue error;
		#endregion
		public SquareMatrix(double[,] sourceArray) {
			Guard.ArgumentNotNull(sourceArray, "sourceArray");
			Guard.ArgumentPositive(sourceArray.Length, "dimension");
			Debug.Assert(sourceArray.GetLength(0) == sourceArray.GetLength(1));
			this.sourceArray = sourceArray;
		}
		public SquareMatrix(int dimension) {
			Guard.ArgumentPositive(dimension, "dimension");
			CreateZeroArray(dimension);
		}
		public SquareMatrix(VariantValue value) {
			Guard.ArgumentNotNull(value, "value");
			CreateSourceArray(value);
		}
		#region Properties
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023")]
		public double this[int row, int column] { get { return sourceArray[row, column]; } set { sourceArray[row, column] = value; } }
		public int Dimension { get { return sourceArray.GetLength(0); } }
		public VariantValue Error { get { return error; } }
		#endregion
		#region RowsOperations
		bool SwapRows(int rowIndex1, int rowIndex2) {
			if (rowIndex1 == rowIndex2)
				return false;
			for (int column = 0; column < Dimension; column++) {
				double value = this[rowIndex1, column];
				this[rowIndex1, column] = this[rowIndex2, column];
				this[rowIndex2, column] = value;
			}
			return true;
		}
		bool SwapRows(int rowIndex1, int rowIndex2, SquareMatrix clone) {
			if (rowIndex1 == rowIndex2)
				return false;
			for (int column = 0; column < Dimension; column++) {
				double value = this[rowIndex1, column];
				this[rowIndex1, column] = this[rowIndex2, column];
				this[rowIndex2, column] = value;
				value = clone[rowIndex1, column];
				clone[rowIndex1, column] = clone[rowIndex2, column];
				clone[rowIndex2, column] = value;
			}
			return true;
		}
		int GetMaxValueIndexFromColumn(int column, out double maxValue) {
			maxValue = 0;
			int result = -1;
			for (int row = column; row < Dimension; row++) {
				double value = Math.Abs(this[row, column]);
				if (value > maxValue) {
					maxValue = value;
					result = row;
				}
			}
			return result;
		}
		void RowDivideScalar(int row, int startColumn, double value) {
			for (int column = startColumn; column < Dimension; column++)
				this[row, column] /= value;
		}
		void RowDivideScalar(int row, int startColumn, double value, SquareMatrix clone) {
			for (int column = startColumn; column < Dimension; column++) {
				this[row, column] /= value;
				clone[row, column] /= value;
			}
		}
		void ModifyRowsAdvance(int factor) {
			for (int row = factor + 1; row < Dimension; row++) {
				double value = this[row, factor];
				for (int column = factor; column < Dimension; column++)
					this[row, column] -= this[factor, column] * value;
			}
		}
		void ModifyRowsAdvance(int factor, SquareMatrix clone) {
			for (int row = 0; row < Dimension; row++) {
				if (row == factor)
					continue;
				double value = this[row, factor];
				for (int column = 0; column < Dimension; column++) {
					this[row, column] -= this[factor, column] * value;
					clone[row, column] -= clone[factor, column] * value;
				}
			}
		}
		void ModifyRowsReverse(int factor, SquareMatrix clone) {
			for (int row = Dimension - 1; row >= 0; row--) {
				if (row == factor)
					continue;
				double value = this[row, factor];
				for (int column = Dimension - 1; column >= 0; column--) {
					this[row, column] -= this[factor, column] * value;
					clone[row, column] -= clone[factor, column] * value;
				}
			}
		}
		#endregion
		#region MatrixOperations
		#region Determinant
		public VariantValue GetDeterminant() {
			if (error.IsError)
				return error;
			if (Dimension == 1)
				return this[0, 0];
			if (Dimension == 2)
				return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
			if (Dimension == 3)
				return this[0, 0] * (this[1, 1] * this[2, 2] - this[2, 1] * this[1, 2]) -
					   this[0, 1] * (this[1, 0] * this[2, 2] - this[2, 0] * this[1, 2]) +
					   this[0, 2] * (this[1, 0] * this[2, 1] - this[2, 0] * this[1, 1]);
			SquareMatrix matrix = this.Clone();
			return matrix.GetDeterminantCore();
		}
		double GetDeterminantCore() {
			double result = 1;
			bool flag = false;
			for (int row = 0; row < Dimension; row++) {
				double maxValue;
				int maxValueIndex = GetMaxValueIndexFromColumn(row, out maxValue);
				if (maxValue == 0)
					return 0;
				flag = flag ? !SwapRows(maxValueIndex, row) : SwapRows(maxValueIndex, row);
				double value = this[row, row];
				result *= value;
				RowDivideScalar(row, row, value);
				ModifyRowsAdvance(row);
			}
			return flag ? -result : result;
		}
		#endregion
		#region InverseMatrix
		public VariantValue GetInverseMatrix() {
			if (error.IsError)
				return error;
			if (Dimension == 1)
				return GetInverseNumber();
			SquareMatrix sourceMatrix = this.Clone();
			SquareMatrix result;
			if (!sourceMatrix.TryGetInverseMatrixCore(out result))
				return VariantValue.ErrorNumber;
			return result.GetArrayValue();
		}
		VariantValue GetInverseNumber() {
			VariantArray array = new VariantArray();
			double number = this[0, 0];
			if (number == 0)
				return VariantValue.ErrorNumber;
			array.SetValues(new VariantValue[,] { { 1.0 / number } });
			return VariantValue.FromArray(array);
		}
		bool TryGetInverseMatrixCore(out SquareMatrix result) {
			result = SquareMatrix.GetIdentityMatrix(Dimension);
			for (int row = 0; row < Dimension; row++) {
				double maxValue;
				int maxValueIndex = GetMaxValueIndexFromColumn(row, out maxValue);
				if (maxValue == 0)
					return false;
				SwapRows(maxValueIndex, row, result);
				RowDivideScalar(row, 0, this[row, row], result);
				ModifyRowsAdvance(row, result);
			}
			for (int row = Dimension - 1; row >= 0; row--)
				ModifyRowsReverse(row, result);
			return true;
		}
		#endregion
		#endregion
		#region Internal
		void CreateZeroArray(int dimension) {
			sourceArray = new double[dimension, dimension];
		}
		#region CreateSourceArray
		void CreateSourceArray(VariantValue value) {
			if (value.IsNumeric)
				CreateSourceArray(value.NumericValue);
			if (value.IsArray)
				CreateSourceArray(value.ArrayValue);
			if (!value.IsNumeric && !value.IsArray)
				error = VariantValue.ErrorInvalidValueInFunction;
		}
		void CreateSourceArray(double number) {
			CreateZeroArray(1);
			sourceArray[0, 0] = number;
		}
		void CreateSourceArray(IVariantArray array) {
			Debug.Assert(array.Width == array.Height);
			Debug.Assert(array.Width != 0);
			CreateZeroArray(array.Width);
			for (int row = 0; row < Dimension; row++)
				for (int column = 0; column < Dimension; column++) {
					VariantValue variantValue = CheckErrorValue(array, row, column);
					if (variantValue.IsError) {
						error = variantValue;
						return;
					}
					sourceArray[row, column] = variantValue.NumericValue;
				}
		}
		#endregion
		VariantValue CheckErrorValue(IVariantArray array, int y, int x) {
			VariantValue value = array.GetValue(y, x);
			if (value.IsError)
				return value;
			if (!value.IsNumeric)
				return VariantValue.ErrorInvalidValueInFunction;
			return value;
		}
		VariantValue GetArrayValue() {
			VariantArray result = VariantArray.Create(Dimension, Dimension);
			for (int i = 0; i < Dimension; i++)
				for (int j = 0; j < Dimension; j++)
					result.SetValue(i, j, this[i, j]);
			return VariantValue.FromArray(result);
		}
		SquareMatrix Clone() {
			SquareMatrix result = new SquareMatrix(Dimension);
			for (int i = 0; i < Dimension; i++)
				for (int j = 0; j < Dimension; j++)
					result[i, j] = this[i, j];
			return result;
		}
		#endregion
	}
	#endregion
}
