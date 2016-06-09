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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid.Native;
#if SL
using DXFrameworkContentElement = DevExpress.Xpf.Core.DXFrameworkElement;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridControlBandsLayout : BandsLayoutBase {
		public ObservableCollectionCore<GridControlBand> Bands { get { return (ObservableCollectionCore<GridControlBand>)BandsCore; } }
		GridControl GridControl { get { return DataControl as GridControl; } }
		ITableView TableView { get { return GridControl.Return(grid => grid.View as ITableView, () => null); } }
		public GridControlBandsLayout() : base() {
			CheckBoxSelectorBand = CreateCheckBoxSelectorBand();
		}
		protected override void  OnMoveColumn(BaseColumn source, HeaderPresenterType moveFrom) {
			base.OnMoveColumn(source, moveFrom);
			GridColumn column = source as GridColumn;
			if(column.IsGrouped && moveFrom == HeaderPresenterType.GroupPanel)
				GridControl.SortInfo.UngroupByColumn(column.FieldName);
		}
		internal override void ForeachBand(Action<BandBase> action) {
			if(IsCheckBoxSelectorBandVisible)
				action(CheckBoxSelectorBand);
			base.ForeachBand(action);
		}
		protected override void SetPrintWidth(BaseColumn column, double width) {
			GridPrintingHelper.SetPrintColumnWidth(column, width);
		}
		internal override BandsLayoutBase CloneAndFillEmptyBands() {
			BandsLayoutBase clone = base.CloneAndFillEmptyBands();
			clone.UpdateFixedBands(new PrintLayoutAssigner());
			return clone;
		}
		#region CheckBoxSelectorColumn
		internal GridControlBand CheckBoxSelectorBand { get; private set; }
		bool IsCheckBoxSelectorBandVisible { get { return TableView != null && TableView.IsCheckBoxSelectorColumnVisible; } }
		internal override void PatchVisibleBands(List<BandBase> visibleBands, bool hasFixedLeftBands) {
			if(!IsCheckBoxSelectorBandVisible)
				return;
			UpdateCheckBoxSelectorBand(hasFixedLeftBands);
			visibleBands.Insert(0, CheckBoxSelectorBand);
		}
		void UpdateCheckBoxSelectorBand(bool hasFixedLeftBands) {
			UpdateIsCheckBoxSelectorBandFixed(hasFixedLeftBands);
			UpdateCheckBoxSelectorBandColumns();
			CheckBoxSelectorBand.OnBandsLayoutChanged();
		}
		void UpdateIsCheckBoxSelectorBandFixed(bool hasFixedLeftBands) {
			if(hasFixedLeftBands)
				CheckBoxSelectorBand.Fixed = FixedStyle.Left;
			else
				CheckBoxSelectorBand.Fixed = FixedStyle.None;
		}
		void UpdateCheckBoxSelectorBandColumns() {
			GridColumn checkBoxSelectorColumn = null;
			if(DataControl != null && DataControl.DataView is TableView)
				checkBoxSelectorColumn = ((TableView)DataControl.DataView).CheckBoxSelectorColumn;
			if(checkBoxSelectorColumn == null || CheckBoxSelectorBand.Columns.Contains(checkBoxSelectorColumn))
				return;
			if(CheckBoxSelectorBand.Columns.Count != 0)
				CheckBoxSelectorBand.Columns.Clear();
			CheckBoxSelectorBand.Columns.Add(checkBoxSelectorColumn);
		}
		GridControlBand CreateCheckBoxSelectorBand() {
			var band = new GridControlBand();
			band.AllowMoving = DevExpress.Utils.DefaultBoolean.False;
			band.AllowResizing = DevExpress.Utils.DefaultBoolean.False;
			band.Owner = this;
			band.BandsLayout = this;
			band.AllowPrinting = false;
			return band;
		}
		#endregion
	}
	public class TreeListControlBandsLayout : BandsLayoutBase {
		public ObservableCollectionCore<TreeListControlBand> Bands { get { return (ObservableCollectionCore<TreeListControlBand>)BandsCore; } }
		protected override void SetPrintWidth(BaseColumn column, double width) {
			GridPrintingHelper.SetPrintColumnWidth(column, width);
		}
		internal override BandsLayoutBase CloneAndFillEmptyBands() {
			BandsLayoutBase clone = base.CloneAndFillEmptyBands();
			clone.UpdateFixedBands(new PrintLayoutAssigner());
			return clone;
		}
	}
	[ContentProperty("Columns")]
	public class GridControlBand : BandBase, IDetailElement<BaseColumn> {
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlBandBands"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Data), XtraSerializableProperty(true, true, true), GridStoreAlwaysProperty]
		public ObservableCollectionCore<GridControlBand> Bands { get { return (ObservableCollectionCore<GridControlBand>)BandsCore; } }
		internal override IBandsCollection CreateBands() {
			return new BandCollection<GridControlBand>();
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlBandColumns"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Data), XtraSerializableProperty(true, true, true), GridStoreAlwaysProperty]
		public ObservableCollectionCore<GridColumn> Columns { get { return (ObservableCollectionCore<GridColumn>)ColumnsCore; } }
		internal override IBandColumnsCollection CreateColumns() {
			return new BandColumnCollection<GridColumn>();
		}
		internal override bool IsServiceColumn() {
			GridControlBandsLayout bandsLayout = BandsLayout as GridControlBandsLayout;
			return bandsLayout != null && bandsLayout.CheckBoxSelectorBand == this;
		}
		#region IDetailElement
		BaseColumn IDetailElement<BaseColumn>.CreateNewInstance(params object[] args) {
			return (BaseColumn)Activator.CreateInstance(this.GetType());
		}
		#endregion
	}
	[ContentProperty("Columns")]
	public class TreeListControlBand : BandBase {
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListControlBandBands"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Data), XtraSerializableProperty(true, true, true), GridStoreAlwaysProperty]
		public ObservableCollectionCore<TreeListControlBand> Bands { get { return (ObservableCollectionCore<TreeListControlBand>)BandsCore; } }
		internal override IBandsCollection CreateBands() {
			return new BandCollection<TreeListControlBand>();
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListControlBandColumns"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Data), XtraSerializableProperty(true, true, true), GridStoreAlwaysProperty]
		public ObservableCollectionCore<TreeListColumn> Columns { get { return (ObservableCollectionCore<TreeListColumn>)ColumnsCore; } }
		internal override IBandColumnsCollection CreateColumns() {
			return new BandColumnCollection<TreeListColumn>();
		}
	}
}
