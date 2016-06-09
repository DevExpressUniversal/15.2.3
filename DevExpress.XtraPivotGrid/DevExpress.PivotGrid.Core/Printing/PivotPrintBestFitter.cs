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
	public class PivotPrintBestFitter : PivotGridBestFitterBase {
		PivotGridPrinterBase printer;
		PivotPrintCellsBestFitProvider cellsBestFitProvider;
		PivotPrintFieldValueAreaBestFitProvider columnArea;
		PivotPrintFieldValueAreaBestFitProvider rowArea;
		protected PivotGridPrinterBase Printer { get { return printer; } }
		protected override IFieldValueAreaBestFitProvider RowAreaFields {
			get {
				if(rowArea == null)
					rowArea = new PivotPrintFieldValueAreaBestFitProvider(Data, printer, false);
				return rowArea;
			}
		}
		protected override IFieldValueAreaBestFitProvider ColumnAreaFields {
			get {
				if(columnArea == null)
					columnArea = new PivotPrintFieldValueAreaBestFitProvider(Data, printer, true);
				return columnArea;
			}
		}
		protected internal override ICellsBestFitProvider CellsArea {
			get {
				if(cellsBestFitProvider == null)
					cellsBestFitProvider = CreateCellsBestFitProvider();
				return cellsBestFitProvider;
			}
		}
		protected virtual PivotPrintCellsBestFitProvider CreateCellsBestFitProvider() {
			return new PivotPrintCellsBestFitProvider(Data, printer);
		}
		public PivotPrintBestFitter(PivotGridData data, PivotGridPrinterBase printer)
			: this(data, printer, new PrintCellSizeProvider(data, data.visualItems, printer)) {
		}
		public PivotPrintBestFitter(PivotGridData data, PivotGridPrinterBase printer, PrintCellSizeProvider cellSizeProvider)
			: base(data) {
			this.printer = printer;
			SetSizeProvider(cellSizeProvider);
		}
	}
}
