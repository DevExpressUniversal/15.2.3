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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Forms {
	#region TableOptionsFormControllerParameters
	public class TableOptionsFormControllerParameters : FormControllerParameters {
		#region Fields
		readonly Table table;
		#endregion
		public TableOptionsFormControllerParameters(IRichEditControl control, Table table)
			: base(control) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
		}
		#region Properties
		internal Table Table { get { return table; } }
		#endregion
	}
	#endregion
	#region TableOptionsFormController
	public class TableOptionsFormController : FormController {
		#region Fields
		readonly Table sourceTable;
		int? leftMargin;
		int? rightMargin;
		int? topMargin;
		int? bottomMargin;
		int? cellSpacing;
		bool allowCellSpacing;
		bool resizeToFitContent;
		#endregion
		public TableOptionsFormController(TableOptionsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.sourceTable = controllerParameters.Table;
			InitializeController();
		}
		#region Properties
		public int? LeftMargin { get { return leftMargin; } set { leftMargin = value; } }
		public int? RightMargin { get { return rightMargin; } set { rightMargin = value; } }
		public int? TopMargin { get { return topMargin; } set { topMargin = value; } }
		public int? BottomMargin { get { return bottomMargin; } set { bottomMargin = value; } }
		public int? CellSpacing { get { return cellSpacing; } set { cellSpacing = value; } }
		public bool AllowCellSpacing { get { return allowCellSpacing; } set { allowCellSpacing = value; } }
		public bool ResizeToFitContent { get { return resizeToFitContent; } set { resizeToFitContent = value; } }
		protected internal DocumentModel documentModel { get { return sourceTable.DocumentModel; } }
		#endregion
		protected internal virtual void InitializeController() {
			InitializeMargins();
			CellSpacing = GetCellSpacing() * 2;
			AllowCellSpacing = CellSpacing != 0;
			ResizeToFitContent = sourceTable.TableLayout == TableLayoutType.Autofit;
		}
		protected internal void InitializeMargins() {
			TableRowCollection rows = sourceTable.Rows;
			TableRow firstRow = rows.First;
			MarginUnitBase firstRowLeftCellMargin = firstRow.GetLeftCellMarginConsiderExceptions();
			MarginUnitBase firstRowRightCellMargin = firstRow.GetRightCellMarginConsiderExceptions();
			MarginUnitBase firstRowTopCellMargin = firstRow.GetTopCellMarginConsiderExceptions();
			MarginUnitBase firstRowBottomCellMargin = firstRow.GetBottomCellMarginConsiderExceptions();
			bool identicalLeftMargins = true;
			bool identicalRightMargins = true;
			bool identicalTopMargins = true;
			bool identicalBottomMargins = true;
			int rowsCount = rows.Count;
			for (int i = 1; i < rowsCount; i++) {
				TableRow currentRow = rows[i];
				MarginUnitBase currentLeftMargins = currentRow.GetLeftCellMarginConsiderExceptions();
				identicalLeftMargins &= EqualsWidthUnit(firstRowLeftCellMargin, currentLeftMargins);
				MarginUnitBase currentRightMargins = currentRow.GetRightCellMarginConsiderExceptions();
				identicalRightMargins &= EqualsWidthUnit(firstRowRightCellMargin, currentRightMargins);
				MarginUnitBase currentTopMargins = currentRow.GetTopCellMarginConsiderExceptions();
				identicalTopMargins &= EqualsWidthUnit(firstRowTopCellMargin, currentTopMargins);
				MarginUnitBase currentBottomMargins = currentRow.GetBottomCellMarginConsiderExceptions();
				identicalBottomMargins &= EqualsWidthUnit(firstRowBottomCellMargin, currentBottomMargins);
			}
			LeftMargin = identicalLeftMargins ? firstRowLeftCellMargin.Value : (int?)null;
			RightMargin = identicalRightMargins ? firstRowRightCellMargin.Value : (int?)null;
			TopMargin = identicalTopMargins ? firstRowTopCellMargin.Value : (int?)null;
			BottomMargin = identicalBottomMargins ? firstRowBottomCellMargin.Value : (int?)null;
		}
		protected internal virtual int? GetCellSpacing() {
			TableRowCollection rows = sourceTable.Rows;
			int rowsCount = rows.Count;
			WidthUnit firstRowCellSpacing = rows.First.CellSpacing;
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = rows[i];
				if (!EqualsWidthUnit(firstRowCellSpacing, currentRow.CellSpacing))
					return null;
			}
			return firstRowCellSpacing.Type == WidthUnitType.ModelUnits ? firstRowCellSpacing.Value : 0;
		}
		protected internal virtual bool EqualsWidthUnit(WidthUnit value1, WidthUnit value2) {
			return value1.Type == value2.Type && value1.Value == value2.Value;
		}
		public override void ApplyChanges() {
			documentModel.BeginUpdate();
			try {
				ApplyChangesCore();
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal void ApplyChangesCore() {
			ApplyMargins();
			sourceTable.TableLayout = ResizeToFitContent ? TableLayoutType.Autofit : TableLayoutType.Fixed;
			ApplyCellSpacing();
		}
		protected internal virtual void ApplyMargins() {
			CellMargins sourceCellMargins = sourceTable.TableProperties.CellMargins;
			if (LeftMargin.HasValue) {
				WidthUnitInfo newLeftMargin = new WidthUnitInfo(WidthUnitType.ModelUnits, LeftMargin.Value);
				if (!EqualsWidthUnit(newLeftMargin, sourceCellMargins.Left))
					sourceCellMargins.Left.CopyFrom(newLeftMargin);
			}
			if (RightMargin.HasValue) {
				WidthUnitInfo newRightMargin = new WidthUnitInfo(WidthUnitType.ModelUnits, RightMargin.Value);
				if (!EqualsWidthUnit(newRightMargin, sourceCellMargins.Right))
					sourceCellMargins.Right.CopyFrom(newRightMargin);
			}
			if (TopMargin.HasValue) {
				WidthUnitInfo newTopMargin = new WidthUnitInfo(WidthUnitType.ModelUnits, TopMargin.Value);
				if (!EqualsWidthUnit(newTopMargin, sourceCellMargins.Top))
					sourceCellMargins.Top.CopyFrom(newTopMargin);
			}
			if (BottomMargin.HasValue) {
				WidthUnitInfo newBottomMargin = new WidthUnitInfo(WidthUnitType.ModelUnits, BottomMargin.Value);
				if (!EqualsWidthUnit(newBottomMargin, sourceCellMargins.Bottom))
					sourceCellMargins.Bottom.CopyFrom(newBottomMargin);
			}
			ResetPropertiesException();
		}
		protected internal virtual bool EqualsWidthUnit(WidthUnitInfo value1, WidthUnit value2) {
			if (value1 == null || value2 == null)
				return false;
			return value1.Type == value2.Type && value1.Value == value2.Value;
		}
		void ResetPropertiesException() {
			CellMargins defaultCellMargins = documentModel.DefaultTableProperties.CellMargins;
			TableRowCollection rows = sourceTable.Rows;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				TableProperties propertiesException = rows[i].TablePropertiesException;
				CellMargins cellMargins = propertiesException.CellMargins;
				if (LeftMargin.HasValue && propertiesException.GetUse(TablePropertiesOptions.Mask.UseLeftMargin)) {
					cellMargins.Left.CopyFrom(defaultCellMargins.Left);
					propertiesException.ResetUse(TablePropertiesOptions.Mask.UseLeftMargin);
				}
				if (RightMargin.HasValue && propertiesException.GetUse(TablePropertiesOptions.Mask.UseRightMargin)) {
					cellMargins.Right.CopyFrom(defaultCellMargins.Right);
					propertiesException.ResetUse(TablePropertiesOptions.Mask.UseRightMargin);
				}
				if (TopMargin.HasValue && propertiesException.GetUse(TablePropertiesOptions.Mask.UseTopMargin)) {
					cellMargins.Top.CopyFrom(defaultCellMargins.Top);
					propertiesException.ResetUse(TablePropertiesOptions.Mask.UseTopMargin);
				}
				if (BottomMargin.HasValue && propertiesException.GetUse(TablePropertiesOptions.Mask.UseBottomMargin)) {
					cellMargins.Bottom.CopyFrom(defaultCellMargins.Bottom);
					propertiesException.ResetUse(TablePropertiesOptions.Mask.UseBottomMargin);
				}
			}
		}
		protected internal virtual void ApplyCellSpacing() {
			if (!CellSpacing.HasValue)
				return;
			if (AllowCellSpacing && CellSpacing.Value != 0)
				ApplyCellSpacingCore();
			else
				ResetCellSpacing();
		}
		void ApplyCellSpacingCore() {
			WidthUnitInfo newCellSpacing = new WidthUnitInfo(WidthUnitType.ModelUnits, CellSpacing.Value / 2);
			WidthUnit tablePropertiesCellSpacing = sourceTable.TableProperties.CellSpacing;
			if (!EqualsWidthUnit(newCellSpacing, tablePropertiesCellSpacing))
				tablePropertiesCellSpacing.CopyFrom(newCellSpacing);
			TableRowCollection rows = sourceTable.Rows;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				WidthUnit rowPropertiesCellSpacing = rows[i].Properties.CellSpacing;
				if (!EqualsWidthUnit(newCellSpacing, rowPropertiesCellSpacing))
					rowPropertiesCellSpacing.CopyFrom(newCellSpacing);
			}
		}
		void ResetCellSpacing() {
			WidthUnit defaultCellSpacing = documentModel.DefaultTableProperties.CellSpacing;
			sourceTable.TableProperties.CellSpacing.CopyFrom(defaultCellSpacing);
			sourceTable.TableProperties.ResetUse(TablePropertiesOptions.Mask.UseCellSpacing);
			TableRowCollection rows = sourceTable.Rows;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				TableRowProperties currentRowProperties = rows[i].Properties;
				if (currentRowProperties.GetUse(TableRowPropertiesOptions.Mask.UseCellSpacing)) {
					currentRowProperties.CellSpacing.CopyFrom(defaultCellSpacing);
					currentRowProperties.ResetUse(TableRowPropertiesOptions.Mask.UseCellSpacing);
				}
			}
		}
	}
	#endregion
	#region TableOptionsFormDefaults
	public static class TableOptionsFormDefaults {
		public const int MaxTopMarginByDefault = 22 * 1440;
		public const int MinTopMarginByDefault = 0;
		public const int MaxRightMarginByDefault = 22 * 1440;
		public const int MinRightMarginByDefault = 0;
		public const int MaxBottomMarginByDefault = 22 * 1440;
		public const int MinBottomMarginByDefault = 0;
		public const int MaxLeftMarginByDefault = 22 * 1440;
		public const int MinLeftMarginByDefault = 0;
		public const int MaxCellSpacingByDefault = 7920; 
		public const int MinCellSpacingByDefault = 0;
		public const int MinCellSpacing = 20;
	}
	#endregion
}
