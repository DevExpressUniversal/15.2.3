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

using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.Printing {
	class PivotPrintFieldValueAreaBestFitProvider : IFieldValueAreaBestFitProvider {
		PivotGridData data;
		PivotGridPrinterBase printer;
		bool isColumn;
		public PivotPrintFieldValueAreaBestFitProvider(PivotGridData data, PivotGridPrinterBase printer, bool isColumn) {
			this.data = data;
			this.printer = printer;
			this.isColumn = isColumn;
		}
		void IFieldValueAreaBestFitProvider.BeginBestFitCalculcations() { }
		void IFieldValueAreaBestFitProvider.EndBestFitCalculcations() { }
		int IFieldValueAreaBestFitProvider.ChildCount {
			get { return data.visualItems.GetItemsCreator(isColumn).UnpagedItemsCount; }
		}
		bool IFieldValueAreaBestFitProvider.IsColumn {
			get { return isColumn; }
		}
		IPivotFieldValueItem IFieldValueAreaBestFitProvider.this[int index] {
			get {
				PivotFieldValueItem item = data.VisualItems.GetItemsCreator(isColumn).GetUnpagedItem(index);
				return new PivotPrintFieldValueItem(item, PrintCellSizeProvider.MeasureAppearance(printer.GetValueAppearance(item.ValueType, item.Field), item.DisplayText, printer).Width + 15);
			}
		}
	}
	class PivotPrintFieldValueItem : IPivotFieldValueItem {
		PivotFieldValueItem item;
		int bestWidth;
		PivotFieldValueItem IPivotFieldValueItem.Item {
			get { return item; }
		}
		public PivotPrintFieldValueItem(PivotFieldValueItem item, int bestWidth) {
			this.item = item;
			this.bestWidth = bestWidth;
		}
		int IPivotFieldValueItem.GetBestWidth() {
			return bestWidth;
		}
	}
}
