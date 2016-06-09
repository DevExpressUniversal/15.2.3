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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Grid {
	public class BestFitRowControl : RowControl {
		public GridColumnData CellData { get; private set; }
		public BestFitRowControl(RowData rowData, GridColumnData cellData)
			: base(rowData) {
			CellData = cellData;
		}
		protected override void CreateDefaultContent() {
			CellsControl = CreateAndInitFixedNoneCellsControl(0, x => new List<GridColumnData> { CellData }, x => null);
		}
		protected override CellsControl CreateCellsControl(Func<RowData, IList<GridColumnData>> getCellDataFunc, Func<BandsLayoutBase, IList<BandBase>> getBandsFunc) {
			return new BestFitCellsControl(this, getCellDataFunc, getBandsFunc);
		}
		protected override void UpdateOffsetPresenterLevel() { }
		protected override bool ShowDetails { get { return false; } }
		protected override bool AllowTreeIndentScrolling { get { return false; } }
	}
	public class LightweightBestFitControl : BestFitControlBase {
		public LightweightBestFitControl(DataViewBase view, ColumnBase column)
			: base(view, column) {
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Content = CreateBestFitRowControl(); 
		}
		protected virtual BestFitRowControl CreateBestFitRowControl() {
			return new BestFitRowControl(RowData, CellData);
		}
	}
	public class BestFitCellsControl : CellsControl {
		public BestFitCellsControl(RowControl rowControl, Func<RowData, IList<GridColumnData>> getCellDataFunc, Func<BandsLayoutBase, IList<BandBase>> getBandsFunc)
			: base(rowControl, getCellDataFunc, getBandsFunc) { }
		protected override void UpdateElementWidth(FrameworkElement element, GridCellData cellData) { }
		protected override void ValidateElementCore(FrameworkElement element, GridCellData cellData) {
			base.ValidateElementCore(element, cellData);
			((LightweightCellEditor)element).IsFocusedCell = RowControl.rowData.View.GetIsCellFocused(RowControl.rowData.RowHandle.Value, cellData.Column);
		}
		protected internal override void UpdatePanel() { }
		protected override FrameworkElement CreateChildCore(GridCellData cellData) {
			return new BestFitCellEditor(this) { RowData = cellData.RowData };
		}
	}
	public class BestFitCellEditor : LightweightCellEditor {
		protected override bool CanRefreshContentCore { get { return true; } }
		public BestFitCellEditor(CellsControl cellsControl) : base(cellsControl) { }
		protected override Size MeasureOverride(Size constraint) {
			Size size = base.MeasureOverride(constraint);
			size.Width++;
			return size;
		}
	}
}
