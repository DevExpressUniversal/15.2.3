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
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Forms {
	#region CellOptionsFormController
	public class TableCellOptionsFormController : FormController {
		#region Fields
		readonly List<TableCell> sourceCells;
		bool cellMarginsSameAsTable;
		int? leftMargin;
		int? rightMargin;
		int? topMargin;
		int? bottomMargin;
		bool? noWrap;
		bool? fitText;
		#endregion
		public TableCellOptionsFormController(List<TableCell> cells) {
			Guard.ArgumentNotNull(cells, "cells");
			Guard.ArgumentPositive(cells.Count, "cells count");
			this.sourceCells = cells;
			InitializeController();
		}
		#region Properties
		public bool CellMarginsSameAsTable { get { return cellMarginsSameAsTable; } set { cellMarginsSameAsTable = value; } }
		public int? LeftMargin { get { return leftMargin; } set { leftMargin = value; } }
		public int? RightMargin { get { return rightMargin; } set { rightMargin = value; } }
		public int? TopMargin { get { return topMargin; } set { topMargin = value; } }
		public int? BottomMargin { get { return bottomMargin; } set { bottomMargin = value; } }
		public bool? NoWrap { get { return noWrap; } set { noWrap = value; } }
		public bool? FitText { get { return fitText; } set { fitText = value; } }
		#endregion
		protected internal virtual void InitializeController() {
			InitializeCellMargins();
			CellMarginsSameAsTable = IsIdenticalCellMarginsAsTableMargins();
			NoWrap = GetTotalNoWrap();
			FitText = GetTotalFitText();
		}
		private void InitializeCellMargins() {
			TableCell firstCell = sourceCells[0];
			MarginUnitBase firstCellLeftMargin = firstCell.GetActualLeftMargin();
			MarginUnitBase firstCellRightMargin = firstCell.GetActualRightMargin();
			MarginUnitBase firstCellTopMargin = firstCell.GetActualTopMargin();
			MarginUnitBase firstCellBottomMargin = firstCell.GetActualBottomMargin();
			bool identicalLeftMargins = true;
			bool identicalRightMargins = true;
			bool identicalTopMargins = true;
			bool identicalBottomMargins = true;
			int cellsCount = sourceCells.Count;
			for (int i = 1; i < cellsCount; i++) {
				TableCell currentCell = sourceCells[i];
				identicalLeftMargins &= EqualsWidthUnit(firstCellLeftMargin, currentCell.GetActualLeftMargin());
				identicalRightMargins &= EqualsWidthUnit(firstCellRightMargin, currentCell.GetActualRightMargin());
				identicalTopMargins &= EqualsWidthUnit(firstCellTopMargin, currentCell.GetActualTopMargin());
				identicalBottomMargins &= EqualsWidthUnit(firstCellBottomMargin, currentCell.GetActualBottomMargin());
			}
			LeftMargin = identicalLeftMargins ? firstCellLeftMargin.Value : (int?)null;
			RightMargin = identicalRightMargins ? firstCellRightMargin.Value : (int?)null;
			TopMargin = identicalTopMargins ? firstCellTopMargin.Value : (int?)null;
			BottomMargin = identicalBottomMargins ? firstCellBottomMargin.Value : (int?)null;
		}
		protected internal virtual bool EqualsWidthUnit(WidthUnit value1, WidthUnit value2) {
			if (value1 == null && value2 == null)
				return true;
			if (value1 == null || value2 == null)
				return false;
			return value1.Type == value2.Type && value1.Value == value2.Value;
		}
		bool IsIdenticalCellMarginsAsTableMargins() {
			int cellsCount = sourceCells.Count;
			for (int i = 0; i < cellsCount; i++) {
				TableCell currentCell = sourceCells[i];
				bool useLeftInnerMargin = GetUseCellInnerMargin(LeftMarginUnit.PropertyAccessor, currentCell);
				bool useRightInnerMargin = GetUseCellInnerMargin(RightMarginUnit.PropertyAccessor, currentCell);
				bool useTopInnerMargin = GetUseCellInnerMargin(TopMarginUnit.PropertyAccessor, currentCell);
				bool useBottomInnerMargin = GetUseCellInnerMargin(BottomMarginUnit.PropertyAccessor, currentCell);
				if (useLeftInnerMargin || useRightInnerMargin || useTopInnerMargin || useBottomInnerMargin)
					return false;
			}
			return true;
		}
		protected internal virtual bool GetUseCellInnerMargin(MarginUnitBase.MarginPropertyAccessorBase accessor, TableCell cell) {
			TableCellProperties cellProperties = cell.Properties;
			if (cellProperties.GetUse(accessor.CellPropertiesMask))
				return true;
			TableCellProperties tableStyleCellProperties = cell.Table.TableStyle.TableCellProperties;
			if (tableStyleCellProperties.GetUse(accessor.CellPropertiesMask))
				return true;
			return false;
		}
		protected internal virtual bool? GetTotalNoWrap() {
			int cellsCount = sourceCells.Count;
			bool firstCellNoWrap = sourceCells[0].NoWrap;
			for (int i = 1; i < cellsCount; i++) {
				TableCell currentCell = sourceCells[i];
				if (firstCellNoWrap != currentCell.NoWrap)
					return null;
			}
			return firstCellNoWrap;
		}
		protected internal virtual bool? GetTotalFitText() {
			int cellsCount = sourceCells.Count;
			bool firstCellFitText = sourceCells[0].FitText;
			for (int i = 1; i < cellsCount; i++) {
				TableCell currentCell = sourceCells[i];
				if (firstCellFitText != currentCell.FitText)
					return null;
			}
			return firstCellFitText;
		}
		public override void ApplyChanges() {
			DocumentModel documentModel = sourceCells[0].DocumentModel;
			documentModel.BeginUpdate();
			try {
				ApplyChangesCore(documentModel);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal virtual void ApplyChangesCore(DocumentModel documentModel) {
			CellMargins defaultCellMargins = documentModel.DefaultTableCellProperties.CellMargins;
			int cellsCount = sourceCells.Count;
			for (int i = 0; i < cellsCount; i++) {
				TableCell currentCell = sourceCells[i];
				TableCellProperties currentCellProperties = currentCell.Properties;
				if (NoWrap.HasValue && currentCellProperties.NoWrap != NoWrap.Value)
					currentCellProperties.NoWrap = NoWrap.Value;
				if (FitText.HasValue && currentCellProperties.FitText != FitText.Value)
					currentCellProperties.FitText = FitText.Value;
				CellMargins currentCellMargins = currentCellProperties.CellMargins;
				if (CellMarginsSameAsTable)
					ResetCellMargins(defaultCellMargins, currentCellMargins, currentCellProperties);
				else
					ApplyCellMargins(currentCell, currentCellMargins);
			}
		}
		protected internal virtual void ResetCellMargins(CellMargins defaultCellMargins, CellMargins cellMargins, TableCellProperties cellProperties) {
			cellMargins.Left.CopyFrom(defaultCellMargins.Left);
			cellMargins.Right.CopyFrom(defaultCellMargins.Right);
			cellMargins.Top.CopyFrom(defaultCellMargins.Top);
			cellMargins.Bottom.CopyFrom(defaultCellMargins.Bottom);
			cellProperties.ResetUse(TableCellPropertiesOptions.Mask.UseLeftMargin);
			cellProperties.ResetUse(TableCellPropertiesOptions.Mask.UseRightMargin);
			cellProperties.ResetUse(TableCellPropertiesOptions.Mask.UseTopMargin);
			cellProperties.ResetUse(TableCellPropertiesOptions.Mask.UseBottomMargin);
		}
		protected internal virtual void ApplyCellMargins(TableCell cell, CellMargins cellMargins) {
			if (LeftMargin.HasValue) {
				WidthUnitInfo newLeftMargin = new WidthUnitInfo(WidthUnitType.ModelUnits, LeftMargin.Value);
				if (!EqualsWidthUnit(newLeftMargin, cell.GetActualLeftMargin()))
					cellMargins.Left.CopyFrom(newLeftMargin);
			}
			if (RightMargin.HasValue) {
				WidthUnitInfo newRightMargin = new WidthUnitInfo(WidthUnitType.ModelUnits, RightMargin.Value);
				if (!EqualsWidthUnit(newRightMargin, cell.GetActualRightMargin()))
					cellMargins.Right.CopyFrom(newRightMargin);
			}
			if (TopMargin.HasValue) {
				WidthUnitInfo newTopMargin = new WidthUnitInfo(WidthUnitType.ModelUnits, TopMargin.Value);
				if (!EqualsWidthUnit(newTopMargin, cell.GetActualTopMargin()))
					cellMargins.Top.CopyFrom(newTopMargin);
			}
			if (BottomMargin.HasValue) {
				WidthUnitInfo newBottomMargin = new WidthUnitInfo(WidthUnitType.ModelUnits, BottomMargin.Value);
				if (!EqualsWidthUnit(newBottomMargin, cell.GetActualBottomMargin()))
					cellMargins.Bottom.CopyFrom(newBottomMargin);
			}
		}
		protected internal virtual bool EqualsWidthUnit(WidthUnitInfo value1, WidthUnit value2) {
			if (value1 == null || value2 == null)
				return false;
			return value1.Type == value2.Type && value1.Value == value2.Value;
		}
	}
	#endregion
	#region TableCellOptionsFormDefaults
	public static class TableCellOptionsFormDefaults {
		public const int MaxTopMarginByDefault = 22 * 1440;
		public const int MinTopMarginByDefault = 0;
		public const int MaxRightMarginByDefault = 22 * 1440;
		public const int MinRightMarginByDefault = 0;
		public const int MaxBottomMarginByDefault = 22 * 1440;
		public const int MinBottomMarginByDefault = 0;
		public const int MaxLeftMarginByDefault = 22 * 1440;
		public const int MinLeftMarginByDefault = 0;
	}
	#endregion
}
