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
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.Printing {
	public class PivotPrintHeaders {
		PivotArea area;
		PivotGridData data;
		PivotGridOptionsPrint optionsPrint;
		PivotGridBestFitterBase bestFitter;
		List<PivotFieldItemBase> fields = new List<PivotFieldItemBase>();
		List<int> widths = new List<int>();
		int height;
		PivotVisualItemsBase VisualItems { get { return data.VisualItems; } }
		PivotGridOptionsPrint OptionsPrint { get { return optionsPrint; } }
		PivotGridOptionsViewBase OptionsView { get { return data.OptionsView; } }
		CellSizeProvider CellSizeProvider { get { return bestFitter.CellSizeProvider; } }
		public PivotFieldItemBase this[int index] {
			get { return fields[index]; }
		}
		public int Count {
			get { return fields.Count; }
		}
		public int Height {
			get {
				return Width > 0 && fields.Count > 0 ? height : 0;
			}
		}
		public int Width {
			get {
				int width = 0;
				for(int i = 0; i < widths.Count; i++)
					width += widths[i];
				return width;
			}
		}
		public PivotPrintHeaders(PivotArea area, PivotGridData data, PivotGridBestFitterBase bestFitter, PivotGridOptionsPrint optionsPrint) {
			this.area = area;
			this.data = data;
			this.bestFitter = bestFitter;
			this.optionsPrint = optionsPrint;
			Calculate();
		}
		public int GetItemWidth(int index) {
			return widths[index];
		}
		public int GetItemWidthOffset(int index) {
			int width = 0;
			for(int i = 0; i < index; i++)
				width += widths[i];
			if(area == PivotArea.ColumnArea)
				width += CellSizeProvider.GetWidthDifference(false, 0, VisualItems.GetItemsCreator(false).GetUnpagedItems().LevelCount);
			return width;
		}
		void Calculate() {
			fields.Clear();
			widths.Clear();
			if(OptionsPrint.GetPrintHeaders(area) == DefaultBoolean.Default)
				if(!OptionsView.GetShowHeaders(area))
					return;
			if(OptionsPrint.GetPrintHeaders(area) == DefaultBoolean.False)
				return;
			GetFields();
			SetWidths();
			FitWidth();
			CalculateHeight();
		}
		void CalculateHeight() {
			height = fields.Count > 0 ? CellSizeProvider.DefaultFieldValueHeight : 0;
			for(int i = 0; i < fields.Count; i++)
				height = Math.Max(height, CellSizeProvider.CalculateHeaderHeight(fields[i]));
		}
		void GetFields() {
			List<PivotFieldItemBase> baseFields = data.FieldItems.GetFieldItemsByArea(area, true);
			for(int i = 0; i < baseFields.Count; i++) {
				if(!OptionsPrint.PrintUnusedFilterFields && area == PivotArea.FilterArea && baseFields[i].FilterValuesCount == 0)
					continue;
				fields.Add(baseFields[i]);
			}
		}
		void SetWidths() {
			for(int i = 0; i < fields.Count; i++)
				if(area == PivotArea.RowArea && OptionsView.RowTotalsLocation != PivotRowTotalsLocation.Tree)
					widths.Add(fields[i].Width);
				else
					widths.Add(CellSizeProvider.CalculateHeaderWidth(fields[i]));
		}
		void FitWidth() {
			if(area == PivotArea.FilterArea || area == PivotArea.ColumnArea || area == PivotArea.RowArea && OptionsView.RowTotalsLocation != PivotRowTotalsLocation.Tree)
				return;
			int rowAreaWidth = CellSizeProvider.GetWidthDifference(false, 0, VisualItems.GetItemsCreator(false).GetUnpagedItems().LevelCount);
			if(Width <= rowAreaWidth)
				return;
			int averageWidth = rowAreaWidth / fields.Count;
			int noNeedCorrectionWidth = 0;
			int noNeedCorrectionCount = 0;
			for(int i = 0; i < fields.Count; i++) {
				if(widths[i] < averageWidth) {
					noNeedCorrectionWidth += widths[i];
					noNeedCorrectionCount++;
				}
			}
			averageWidth = (rowAreaWidth - noNeedCorrectionWidth) / (fields.Count - noNeedCorrectionCount);
			for(int i = 0; i < fields.Count; i++) {
				if(widths[i] > averageWidth)
					widths[i] = averageWidth;
			}
		}
	}
}
