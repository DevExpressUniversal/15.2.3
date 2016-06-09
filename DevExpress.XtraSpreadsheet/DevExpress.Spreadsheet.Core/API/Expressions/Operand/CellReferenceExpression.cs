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
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using DevExpress.XtraSpreadsheet.Utils;
using System.ComponentModel;
using Model = DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Spreadsheet.Formulas {
	public enum PositionType {
		Relative = 0,
		Absolute = 1,
	}
	#region CellReferencePosition
	public struct CellReferencePosition : IEquatable<CellReferencePosition> {
		static readonly CellReferencePosition invalidValue = new CellReferencePosition(Model.CellPosition.InvalidValue);
		public static CellReferencePosition InvalidValue { get { return invalidValue; } }
		#region Fields
		Model.CellPosition modelPosition;
		#endregion
		public CellReferencePosition(int column, int row, PositionType columnType, PositionType rowType) {
			if (!IndicesChecker.CheckIsColumnIndexValid(column))
				Exceptions.ThrowArgumentException("column", column);
			if (!IndicesChecker.CheckIsRowIndexValid(row))
				Exceptions.ThrowArgumentException("row", row);
			modelPosition = new Model.CellPosition(column, row, (Model.PositionType)columnType, (Model.PositionType)rowType);
		}
		public CellReferencePosition(int column, int row) {
			if (!IndicesChecker.CheckIsColumnIndexValid(column))
				Exceptions.ThrowArgumentException("column", column);
			if (!IndicesChecker.CheckIsRowIndexValid(row))
				Exceptions.ThrowArgumentException("row", row);
			modelPosition = new Model.CellPosition(column, row);
		}
		internal CellReferencePosition(Model.CellPosition modelPosition) {
			this.modelPosition = modelPosition;
		}
		#region Properties
		public PositionType ColumnType { get { return (PositionType)modelPosition.ColumnType; } }
		public PositionType RowType { get { return (PositionType)modelPosition.RowType; } }
		public int Column { get { return modelPosition.Column; } }
		public int Row { get { return modelPosition.Row; } }
		internal Model.CellPosition ModelPosition { get { return modelPosition; } set { modelPosition = value; } }
		#endregion
		public static CellReferencePosition TryCreate(string reference) {
			Model.CellPosition modelPosition = Model.CellPosition.TryCreate(reference);
			if (modelPosition.IsValid)
				return new CellReferencePosition(modelPosition);
			return InvalidValue;
		}
		public override string ToString() {
			return modelPosition.ToString();
		}
		internal string ToString(Model.WorkbookDataContext context) {
			return modelPosition.ToString(context);
		}
		public override int GetHashCode() {
			return modelPosition.GetHashCode();
		}
		#region IEquatable<CellReferencePosition> Members
		public bool Equals(CellReferencePosition other) {
			return modelPosition.Equals(other.modelPosition);
		}
		public override bool Equals(object obj) {
			if (!(obj is CellReferencePosition))
				return false;
			return Equals((CellReferencePosition)obj);
		}
		#endregion
	}
	#endregion
	public class CellArea : ICloneable<CellArea>, ISupportsCopyFrom<CellArea> {
		#region Fields
		Model.CellPosition topLeft;
		Model.CellPosition bottomRight;
		#endregion
		public CellArea(int leftColumnIndex, int topRowIndex, int rightColumnIndex, int bottomRowIndex) {
			this.topLeft = new Model.CellPosition(leftColumnIndex, topRowIndex);
			this.bottomRight = new Model.CellPosition(rightColumnIndex, bottomRowIndex);
		}
		public CellArea(Range sourceRange) {
			Guard.ArgumentNotNull(sourceRange, "sourceRange");
			this.topLeft = new Model.CellPosition(sourceRange.LeftColumnIndex, sourceRange.TopRowIndex);
			this.bottomRight = new Model.CellPosition(sourceRange.RightColumnIndex, sourceRange.BottomRowIndex);
		}
		public CellArea(CellReferencePosition topLeft, CellReferencePosition bottomRight) {
			Guard.ArgumentNotNull(topLeft, "topLeft");
			Guard.ArgumentNotNull(bottomRight, "bottomRight");
			this.topLeft = topLeft.ModelPosition;
			this.bottomRight = bottomRight.ModelPosition;
		}
		internal CellArea(Model.CellPosition topLeft, Model.CellPosition bottomRight) {
			this.topLeft = topLeft;
			this.bottomRight = bottomRight;
		}
		internal CellArea(Model.CellRange cellRange) {
			Guard.ArgumentNotNull(cellRange, "cellRange");
			this.topLeft = cellRange.TopLeft;
			this.bottomRight = cellRange.BottomRight;
		}
		CellArea() {
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellAreaLeftColumnIndex")]
#endif
		public int LeftColumnIndex {
			get { return topLeft.Column; }
			set {
				topLeft = new Model.CellPosition(value, topLeft.Row, topLeft.ColumnType, topLeft.RowType);
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellAreaTopRowIndex")]
#endif
		public int TopRowIndex {
			get { return topLeft.Row; }
			set {
				topLeft = new Model.CellPosition(topLeft.Column, value, topLeft.ColumnType, topLeft.RowType);
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellAreaRightColumnIndex")]
#endif
		public int RightColumnIndex {
			get { return bottomRight.Column; }
			set {
				bottomRight = new Model.CellPosition(value, bottomRight.Row, bottomRight.ColumnType, bottomRight.RowType);
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellAreaBottomRowIndex")]
#endif
		public int BottomRowIndex {
			get { return bottomRight.Row; }
			set {
				bottomRight = new Model.CellPosition(bottomRight.Column, value, bottomRight.ColumnType, bottomRight.RowType);
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellAreaColumnCount")]
#endif
		public int ColumnCount { get { return RightColumnIndex - LeftColumnIndex + 1; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellAreaRowCount")]
#endif
		public int RowCount { get { return BottomRowIndex - TopRowIndex + 1; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellAreaTopLeft")]
#endif
		public CellReferencePosition TopLeft {
			get { return new CellReferencePosition(topLeft); }
			set {
				topLeft = value.ModelPosition;
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellAreaBottomRight")]
#endif
		public CellReferencePosition BottomRight {
			get { return new CellReferencePosition(bottomRight); }
			set {
				bottomRight = value.ModelPosition;
			}
		}
		internal Model.CellRange ModelRange { get { return new Model.CellRange(null, topLeft, bottomRight); } }
		#endregion
		#region ICloneable<RangeArea> Members
		public CellArea Clone() {
			CellArea clone = new CellArea();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<RangeArea> Members
		public void CopyFrom(CellArea value) {
			this.topLeft = value.topLeft;
			this.bottomRight = value.bottomRight;
		}
		#endregion
		internal string ToString(Model.WorkbookDataContext context) {
			Model.CellRange modelRange = ModelRange.TryConvertToCellInterval();
			return modelRange.ToString(context);
		}
		public override string ToString() {
			Model.CellRange modelRange = ModelRange.TryConvertToCellInterval();
			return modelRange.ToString();
		}
		public void Normalize() {
			Model.CellRange modelRange = ModelRange;
			modelRange.Normalize();
			topLeft = modelRange.TopLeft;
			bottomRight = modelRange.BottomRight;
		}
	}
	#region ReferenceExpression(abstract class)
	public abstract class ReferenceExpression : Expression {
		#region Fields
		SheetReference sheetReference;
		#endregion
		protected ReferenceExpression(SheetReference sheetReference) {
			this.sheetReference = sheetReference;
		}
		protected ReferenceExpression() {
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ReferenceExpressionSheetReference")]
#endif
		public SheetReference SheetReference { get { return sheetReference; } set { sheetReference = value; } }
		#endregion
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			if (sheetReference != null)
				sheetReference.BuildString(result, context);
		}
		protected void CopyFrom(ReferenceExpression value) {
			base.CopyFrom(value);
			if (value.sheetReference == null)
				sheetReference = null;
			else
				sheetReference = value.SheetReference.Clone();
		}
	}
	#endregion
	#region CellReferenceExpression
	public class CellReferenceExpression : ReferenceExpression, ISupportsCopyFrom<CellReferenceExpression>, ICloneable<CellReferenceExpression> {
		#region Fields
		CellArea cellArea;
		#endregion
		public CellReferenceExpression(CellArea rangeArea, SheetReference sheetReference)
			: base(sheetReference) {
			this.cellArea = rangeArea;
		}
		public CellReferenceExpression(CellArea rangeArea) {
			this.cellArea = rangeArea;
		}
		protected CellReferenceExpression() {
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellReferenceExpressionCellArea")]
#endif
		public CellArea CellArea {
			get { return cellArea; }
			set {
				if (cellArea == null)
					throw new ArgumentNullException("CellArea can not be null");
				cellArea = value;
			}
		}
		#endregion
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			base.BuildExpressionStringCore(result, workbook, context);
			result.Append(cellArea.ToString(context));
		}
		public override void Visit(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}
		#region ISupportsCopyFrom<CellReferenceExpression> Members
		public void CopyFrom(CellReferenceExpression value) {
			base.CopyFrom(value);
			this.cellArea = value.CellArea.Clone();
		}
		#endregion
		#region ICloneable<CellReferenceExpression> Members
		public CellReferenceExpression Clone() {
			CellReferenceExpression result = new CellReferenceExpression();
			result.CopyFrom(this);
			return result;
		}
		protected override IExpression CloneCore() {
			return Clone();
		}
		#endregion
	}
	#endregion
	#region CellReferenceErrorExpression(=Sheet1!#REF!)
	public class CellErrorReferenceExpression : ReferenceExpression, ISupportsCopyFrom<CellErrorReferenceExpression>, ICloneable<CellErrorReferenceExpression> {
		public CellErrorReferenceExpression(SheetReference sheetReference)
			: base(sheetReference) {
		}
		public CellErrorReferenceExpression() {
		}
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			base.BuildExpressionStringCore(result, workbook, context);
			result.Append(Model.CellErrorFactory.GetErrorName(Model.ReferenceError.Instance, context));
		}
		public override void Visit(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}
		#region ISupportsCopyFrom<CellErrorReferenceExpression> Members
		public void CopyFrom(CellErrorReferenceExpression value) {
			base.CopyFrom(value);
		}
		#endregion
		#region ICloneable<CellErrorReferenceExpression> Members
		public CellErrorReferenceExpression Clone() {
			CellErrorReferenceExpression result = new CellErrorReferenceExpression();
			result.CopyFrom(this);
			return result;
		}
		protected override IExpression CloneCore() {
			return Clone();
		}
		#endregion
	}
	#endregion
}
