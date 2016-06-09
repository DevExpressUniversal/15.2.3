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
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Grid {
	[ContentProperty("Bands")]
	public abstract class BandsLayoutBase : DXFrameworkContentElement, IBandsOwner {
		public static readonly DependencyProperty ShowBandsPanelProperty;
		public static readonly DependencyProperty BandHeaderTemplateProperty;
		public static readonly DependencyProperty BandHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty BandHeaderToolTipTemplateProperty;
		public static readonly DependencyProperty AllowChangeColumnParentProperty;
		public static readonly DependencyProperty AllowChangeBandParentProperty;
		public static readonly DependencyProperty ShowBandsInCustomizationFormProperty;
		public static readonly DependencyProperty AllowBandMovingProperty;
		public static readonly DependencyProperty AllowBandResizingProperty;
		static readonly DependencyPropertyKey ColumnChooserBandsPropertyKey;
		public static readonly DependencyProperty ColumnChooserBandsProperty;
		public static readonly DependencyProperty ColumnChooserBandsSortOrderComparerProperty;
		public static readonly DependencyProperty AllowAdvancedVerticalNavigationProperty;
		public static readonly DependencyProperty AllowAdvancedHorizontalNavigationProperty;
		public static readonly DependencyProperty PrintBandHeaderStyleProperty;
		static readonly DependencyPropertyKey FixedLeftVisibleBandsPropertyKey;
		public static readonly DependencyProperty FixedLeftVisibleBandsProperty;
		static readonly DependencyPropertyKey FixedRightVisibleBandsPropertyKey;
		public static readonly DependencyProperty FixedRightVisibleBandsProperty;
		static readonly DependencyPropertyKey FixedNoneVisibleBandsPropertyKey;
		public static readonly DependencyProperty FixedNoneVisibleBandsProperty;
		static readonly DependencyPropertyKey ShowIndicatorPropertyKey;
		public static readonly DependencyProperty ShowIndicatorProperty;
		static BandsLayoutBase() {
			Type ownerType = typeof(BandsLayoutBase);
			ShowBandsPanelProperty = DependencyProperty.Register("ShowBandsPanel", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((BandsLayoutBase)d).OnShowBandsPanelChanged()));
			BandHeaderTemplateProperty = DependencyProperty.Register("BandHeaderTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((BandsLayoutBase)d).UpdateBandActualHeaderTemplateSelector()));
			BandHeaderTemplateSelectorProperty = DependencyProperty.Register("BandHeaderTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, (d, e) => ((BandsLayoutBase)d).UpdateBandActualHeaderTemplateSelector()));
			BandHeaderToolTipTemplateProperty = DependencyProperty.Register("BandHeaderToolTipTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((BandsLayoutBase)d).UpdateBandHeaderToolTipTemplate()));
			AllowChangeColumnParentProperty = DependencyProperty.Register("AllowChangeColumnParent", typeof(bool), ownerType, new PropertyMetadata(false));
			AllowChangeBandParentProperty = DependencyProperty.Register("AllowChangeBandParent", typeof(bool), ownerType, new PropertyMetadata(false));
			ShowBandsInCustomizationFormProperty = DependencyProperty.Register("ShowBandsInCustomizationForm", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((BandsLayoutBase)d).OnShowBandsInCustomizationFormChanged()));
			AllowBandMovingProperty = DependencyProperty.Register("AllowBandMoving", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((BandsLayoutBase)d).UpdateViewInfo()));
			AllowBandResizingProperty = DependencyProperty.Register("AllowBandResizing", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((BandsLayoutBase)d).UpdateViewInfo()));
			ColumnChooserBandsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ColumnChooserBands", typeof(ReadOnlyCollection<BandBase>), ownerType, new FrameworkPropertyMetadata(null));
			ColumnChooserBandsProperty = ColumnChooserBandsPropertyKey.DependencyProperty;
			ColumnChooserBandsSortOrderComparerProperty = DependencyProperty.Register("ColumnChooserBandsSortOrderComparer", typeof(IComparer<BandBase>), ownerType, new PropertyMetadata(null, (d, e) => ((BandsLayoutBase)d).RebuildColumnChooserColumns()));
			AllowAdvancedVerticalNavigationProperty = DependencyProperty.Register("AllowAdvancedVerticalNavigation", typeof(bool), ownerType, new PropertyMetadata(true));
			AllowAdvancedHorizontalNavigationProperty = DependencyProperty.Register("AllowAdvancedHorizontalNavigation", typeof(bool), ownerType, new PropertyMetadata(true));
			PrintBandHeaderStyleProperty = DependencyProperty.Register("PrintBandHeaderStyle", typeof(Style), ownerType, new PropertyMetadata(null, (d, e) => ((BandsLayoutBase)d).UpdatePrintBandHeaderStyle()));
			FixedLeftVisibleBandsPropertyKey = DependencyPropertyManager.RegisterReadOnly("FixedLeftVisibleBands", typeof(ReadOnlyCollection<BandBase>), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BandsLayoutBase)d).OnFixedLeftVisibleBandsChanged()));
			FixedLeftVisibleBandsProperty = FixedLeftVisibleBandsPropertyKey.DependencyProperty;
			FixedRightVisibleBandsPropertyKey = DependencyPropertyManager.RegisterReadOnly("FixedRightVisibleBands", typeof(ReadOnlyCollection<BandBase>), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BandsLayoutBase)d).OnFixedRightVisibleBandsChanged()));
			FixedRightVisibleBandsProperty = FixedRightVisibleBandsPropertyKey.DependencyProperty;
			FixedNoneVisibleBandsPropertyKey = DependencyPropertyManager.RegisterReadOnly("FixedNoneVisibleBands", typeof(ReadOnlyCollection<BandBase>), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BandsLayoutBase)d).OnFixedNoneVisibleBandsChanged()));
			FixedNoneVisibleBandsProperty = FixedNoneVisibleBandsPropertyKey.DependencyProperty;
			ShowIndicatorPropertyKey = DependencyProperty.RegisterReadOnly("ShowIndicator", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ShowIndicatorProperty = ShowIndicatorPropertyKey.DependencyProperty;
		}
		void OnFixedNoneVisibleBandsChanged() {
			View.Do(x => x.ViewBehavior.NotifyFixedNoneBandsChanged());
		}
		void OnFixedRightVisibleBandsChanged() {
			View.Do(x => x.ViewBehavior.NotifyFixedRightBandsChanged());
		}
		void OnFixedLeftVisibleBandsChanged() {
			View.Do(x => x.ViewBehavior.NotifyFixedLeftBandsChanged());
		}
		void UpdateBandActualHeaderTemplateSelector() {
			ForeachBand((band) => band.UpdateActualHeaderTemplateSelector());
		}
		void UpdateBandHeaderToolTipTemplate() {
			ForeachBand((band) => band.UpdateActualHeaderToolTipTemplate());
		}
		void UpdatePrintBandHeaderStyle() {
			ForeachBand((band) => band.UpdateActualPrintBandHeaderStyle());
		}
		void OnShowBandsInCustomizationFormChanged() {
			DataControl.UpdateViewActualColumnChooserTemplate();
		}
		public DataTemplate BandHeaderTemplate {
			get { return (DataTemplate)GetValue(BandHeaderTemplateProperty); }
			set { SetValue(BandHeaderTemplateProperty, value); }
		}
		public DataTemplateSelector BandHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(BandHeaderTemplateSelectorProperty); }
			set { SetValue(BandHeaderTemplateSelectorProperty, value); }
		}
		public DataTemplate BandHeaderToolTipTemplate {
			get { return (DataTemplate)GetValue(BandHeaderToolTipTemplateProperty); }
			set { SetValue(BandHeaderToolTipTemplateProperty, value); }
		}
		public bool ShowBandsPanel {
			get { return (bool)GetValue(ShowBandsPanelProperty); }
			set { SetValue(ShowBandsPanelProperty, value); }
		}
		public bool AllowChangeColumnParent {
			get { return (bool)GetValue(AllowChangeColumnParentProperty); }
			set { SetValue(AllowChangeColumnParentProperty, value); }
		}
		public bool AllowChangeBandParent {
			get { return (bool)GetValue(AllowChangeBandParentProperty); }
			set { SetValue(AllowChangeBandParentProperty, value); }
		}
		public bool ShowBandsInCustomizationForm {
			get { return (bool)GetValue(ShowBandsInCustomizationFormProperty); }
			set { SetValue(ShowBandsInCustomizationFormProperty, value); }
		}
		public bool AllowBandMoving {
			get { return (bool)GetValue(AllowBandMovingProperty); }
			set { SetValue(AllowBandMovingProperty, value); }
		}
		public bool AllowBandResizing {
			get { return (bool)GetValue(AllowBandResizingProperty); }
			set { SetValue(AllowBandResizingProperty, value); }
		}
		[Browsable(false)]
		public ReadOnlyCollection<BandBase> ColumnChooserBands {
			get { return (ReadOnlyCollection<BandBase>)GetValue(ColumnChooserBandsProperty); }
			protected set { this.SetValue(ColumnChooserBandsPropertyKey, value); }
		}
		[Browsable(false), CloneDetailMode(CloneDetailMode.Skip)]
		public IComparer<BandBase> ColumnChooserBandsSortOrderComparer {
			get { return (IComparer<BandBase>)GetValue(ColumnChooserBandsSortOrderComparerProperty); }
			set { SetValue(ColumnChooserBandsSortOrderComparerProperty, value); }
		}
		public bool AllowAdvancedVerticalNavigation {
			get { return (bool)GetValue(AllowAdvancedVerticalNavigationProperty); }
			set { SetValue(AllowAdvancedVerticalNavigationProperty, value); }
		}
		public bool AllowAdvancedHorizontalNavigation {
			get { return (bool)GetValue(AllowAdvancedHorizontalNavigationProperty); }
			set { SetValue(AllowAdvancedHorizontalNavigationProperty, value); }
		}
		public Style PrintBandHeaderStyle {
			get { return (Style)GetValue(PrintBandHeaderStyleProperty); }
			set { SetValue(PrintBandHeaderStyleProperty, value); }
		}
		[Browsable(false)]
		public ReadOnlyCollection<BandBase> FixedLeftVisibleBands {
			get { return (ReadOnlyCollection<BandBase>)GetValue(FixedLeftVisibleBandsProperty); }
			private set { this.SetValue(FixedLeftVisibleBandsPropertyKey, value); }
		}
		[Browsable(false)]
		public ReadOnlyCollection<BandBase> FixedRightVisibleBands {
			get { return (ReadOnlyCollection<BandBase>)GetValue(FixedRightVisibleBandsProperty); }
			private set { this.SetValue(FixedRightVisibleBandsPropertyKey, value); }
		}
		[Browsable(false)]
		public ReadOnlyCollection<BandBase> FixedNoneVisibleBands {
			get { return (ReadOnlyCollection<BandBase>)GetValue(FixedNoneVisibleBandsProperty); }
			private set { this.SetValue(FixedNoneVisibleBandsPropertyKey, value); }
		}
		[Browsable(false)]
		public bool ShowIndicator {
			get { return (bool)GetValue(ShowIndicatorProperty); }
			private set { this.SetValue(ShowIndicatorPropertyKey, value); }
		}
		public IBandsCollection BandsCore { get { return DataControl.BandsCore; } }
		public BandsLayoutBase() {
			VisibleBands = new List<BandBase>();
			ColumnChooserBands = new ReadOnlyCollection<BandBase>(new BandBase[0]);
			FixedLeftVisibleBands = new ReadOnlyCollection<BandBase>(new BandBase[0]);
			FixedRightVisibleBands = new ReadOnlyCollection<BandBase>(new BandBase[0]);
			FixedNoneVisibleBands = new ReadOnlyCollection<BandBase>(new BandBase[0]);
		}
		BandsHelper bandsHelper;
		DataControlBase dataControl;
		internal DataControlBase DataControl {
			get {
				return dataControl;
			}
			set {
				if(dataControl == value) return;
				if(dataControl != null && !dataControl.IsDeserializing) dataControl.ColumnsCore.Clear();
				dataControl = value;
				if(dataControl == null) return;
				bandsHelper = new BandsHelper(this, false);
				if(!dataControl.IsDeserializing)
					FillColumns();
			}
		}
		internal void OnGridControlBandsChanged(NotifyCollectionChangedEventArgs e) {
			bandsHelper.OnBandsChanged(e);
		}
		DataControlBase IBandsOwner.DataControl { get { return DataControl; } }
		public List<BandBase> VisibleBands { get; private set; }
		internal IEnumerable<BandBase> PrintableBands { get { return VisibleBands.Where(band => band.AllowPrinting); } }
		List<BandBase> IBandsOwner.VisibleBands { get { return VisibleBands; } }
		IBandsOwner IBandsOwner.FindClone(DataControlBase dataControl) {
			return dataControl.BandsLayoutCore;
		}
		void OnShowBandsPanelChanged() {
			UpdateBandsContainer(GetBandsContainerControl());
		}
		internal FrameworkElement GetBandsContainerControl() {
			if(DataControl != null) {
				if(DataControl == DataControl.GetRootDataControl())
					return DataControl.viewCore.RootBandsContainer;
				RowDataBase headersData = DataControl.DataControlParent.GetHeadersRowData();
				if(headersData != null && headersData.WholeRowElement != null)
					return LayoutHelper.FindElementByName(headersData.WholeRowElement, "PART_BandsContainer");
			}
			return null;
		}
		internal void UpdateBandsContainer(FrameworkElement bandsContainer) {
			if(bandsContainer == null)
				return;
			bandsContainer.Visibility = ShowBandsPanel ? Visibility.Visible : Visibility.Collapsed;
			if(DataControl != null)
				foreach(ColumnBase col in DataControl.ColumnsCore)
					col.RaiseHasTopElementChanged();
		}
		void IBandsOwner.OnBandsChanged(NotifyCollectionChangedEventArgs e) {
			if(DataControl != null) DataControl.ColumnsCore.BeginUpdate();
			if(e.Action == NotifyCollectionChangedAction.Reset) {
				foreach(BandBase band in BandsCore) {
					OnBandAdded(band);
				}
			} else {
				if(e.OldItems != null)
					foreach(BandBase band in e.OldItems) {
						OnBandRemoved(band);
					}
				if(e.NewItems != null)
					foreach(BandBase band in e.NewItems) {
						OnBandAdded(band);
					}
			}
			if(DataControl != null) DataControl.ColumnsCore.EndUpdate();
			UpdateBandsLayout();
		}
		DataViewBase View { get { return DataControl != null ? DataControl.viewCore : null; } }
		internal void UpdateBandsLayout() {
			if(View != null && !View.IsLockUpdateColumnsLayout) {
				View.RebuildVisibleColumns();
				View.UpdateContentLayout();
			}
		}
#if DEBUGTEST
		public int RebuildBandRowsCount { get; private set; }
#endif
		internal IList<ColumnBase> RebuildBandRows() {
#if DEBUGTEST
			RebuildBandRowsCount++;
#endif
			List<ColumnBase> visibleColumns = new List<ColumnBase>();
			if(View != null) View.UpdateVisibleIndexesLocker.Lock();
			RebuildVisibleBands(this, visibleColumns);
			if(View != null) View.UpdateVisibleIndexesLocker.Unlock();
			UpdateFixedBands();
			UpdateColumnsHasTopBottomElement();
			for(int i = 0; i < visibleColumns.Count; i++) {
				visibleColumns[i].IsFirst = i == 0;
				visibleColumns[i].IsLast = i == visibleColumns.Count - 1;
			}
			return visibleColumns;
		}
		internal void UpdateFixedBands() {
			UpdateFixedBands(new LayoutAssigner());
		}
		internal void UpdateFixedBands(LayoutAssigner layoutAssigner) {
			List<BandBase> fixedLeftBands = new List<BandBase>();
			List<BandBase> fixedRightBands = new List<BandBase>();
			List<BandBase> fixedNoneBands = new List<BandBase>();
			foreach(BandBase band in VisibleBands) {
				if(!band.Visible) continue;
				if(band.Fixed == FixedStyle.Left)
					fixedLeftBands.Add(band);
				else if(band.Fixed == FixedStyle.Right)
					fixedRightBands.Add(band);
				else
					fixedNoneBands.Add(band);
			}
			if(!ListHelper.AreEqual(FixedLeftVisibleBands, fixedLeftBands)) {
				FixedLeftVisibleBands = new ReadOnlyCollection<BandBase>(fixedLeftBands);
			}
			if(!ListHelper.AreEqual(FixedRightVisibleBands, fixedRightBands)) {
				FixedRightVisibleBands = new ReadOnlyCollection<BandBase>(fixedRightBands);
			}
			if(!ListHelper.AreEqual(FixedNoneVisibleBands, fixedNoneBands)) {
				FixedNoneVisibleBands = new ReadOnlyCollection<BandBase>(fixedNoneBands);
			}
			UpdateBandPosition(FixedLeftVisibleBands, false, true, true, false, layoutAssigner);
			UpdateBandPosition(FixedNoneVisibleBands, false, FixedLeftVisibleBands.Count == 0, true, false, layoutAssigner);
			UpdateBandPosition(FixedRightVisibleBands, false, false, true, false, layoutAssigner);
		}
		internal void RebuildColumnChooserColumns() {
			List<ColumnBase> columnChooserColumns = new List<ColumnBase>();
			List<BandBase> columnChooserBands = new List<BandBase>();
			ForeachBand(band => {
				if(!band.Visible)
					columnChooserBands.Add(band);
				foreach(ColumnBase column in band.ColumnsCore) {
					if(!column.Visible && column.ShowInColumnChooser)
						columnChooserColumns.Add(column);
				}
			});
			columnChooserColumns.Sort(View.ColumnChooserColumnsSortOrderComparer);
			if(!ListHelper.AreEqual(View.ColumnChooserColumns, columnChooserColumns)) {
				View.ColumnChooserColumns = new ReadOnlyCollection<ColumnBase>(columnChooserColumns);
			}
			if(ColumnChooserBandsSortOrderComparer != null)
				columnChooserBands.Sort(ColumnChooserBandsSortOrderComparer);
			if(!ListHelper.AreEqual(ColumnChooserBands, columnChooserBands)) {
				ColumnChooserBands = new ReadOnlyCollection<BandBase>(columnChooserBands);
			}
		}
		void RebuildVisibleBands(IBandsOwner bandsOwner, List<ColumnBase> visibleColumns) {
			bandsOwner.VisibleBands.Clear();
			bool hasFixedLeftBands = false;
			for(int i = 0; i < bandsOwner.BandsCore.Count; i++) {
				BandBase band = bandsOwner.BandsCore[i] as BandBase;
				band.index = i;
				if(!band.Visible) continue;
				bandsOwner.VisibleBands.Add(band);
				if(band.Fixed == FixedStyle.Left)
					hasFixedLeftBands = true;
			}
			bandsOwner.VisibleBands.Sort(View.VisibleComparison);
			if(ShouldPatchVisibleBands(bandsOwner))
				PatchVisibleBands(VisibleBands, hasFixedLeftBands);
			for(int i = 0; i < bandsOwner.VisibleBands.Count; i++) {
				BandBase band = bandsOwner.VisibleBands[i] as BandBase;
				band.VisibleIndex = i;
				RebuildVisibleBands(band, visibleColumns);
				RebuildBandLayout(band, visibleColumns);
			}
		}
		bool ShouldPatchVisibleBands(IBandsOwner bandsOwner) {
			return bandsOwner == this;
		}
		internal virtual void PatchVisibleBands(List<BandBase> visibleBands, bool hasFixedLeftBands) { }
		void RebuildBandLayout(BandBase band, List<ColumnBase> visibleColumns) {
			band.ActualRows.Clear();
			if(!band.Visible) return;
			if(band.VisibleBands.Count == 0) {
				if(band.ColumnsCore.Count != 0) {
					Dictionary<int, BandRow> rows = new Dictionary<int, BandRow>();
					for(int i = 0; i < band.ColumnsCore.Count; i++) {
						ColumnBase column = band.ColumnsCore[i] as ColumnBase;
						column.index = i;
						if(!column.Visible || !View.IsColumnVisibleInHeaders(column)) {
							column.BandRow = null;
							continue;
						}
						int row = BandBase.GetGridRow(column);
						if(!rows.ContainsKey(row)) {
							rows[row] = new BandRow() { Columns = new List<ColumnBase>() };
						}
						rows[row].Columns.Add(column);
						column.BandRow = rows[row];
					}
					foreach(int index in rows.Keys.OrderBy(i => i)) {
						BandRow row = rows[index];
						row.Columns.Sort(DataControl.viewCore.VisibleComparison);
						foreach(ColumnBase column in row.Columns) {
							visibleColumns.Add(column);
						}
						band.ActualRows.Add(row);
					}
				}
			}
		}
		bool UpdateBandPosition(IList<BandBase> bands, bool hasTopElement, bool ownerIsLeft, bool ownerIsRight, bool emptyLeftSibling, LayoutAssigner layoutAssigner) {
			for(int i = 0; i < bands.Count; i++) {
				BandBase band = bands[i];
				bool isLeft = i == 0 && ownerIsLeft;
				bool isRight = i == bands.Count - 1 && ownerIsRight;
				band.ColumnPosition = GetColumnPosition(isLeft);
				band.HasTopElement = hasTopElement;
				band.HasRightSibling = !isRight;
				band.HasLeftSibling = !isLeft;
				if(band.ActualRows.Count > 0) {
					UpdateColumnsPositions(band, isLeft, isRight, emptyLeftSibling, layoutAssigner);
					emptyLeftSibling = false;
				}
				else if(band.VisibleBands.Count == 0) {
					emptyLeftSibling = true;
				}
				emptyLeftSibling = UpdateBandPosition(band.VisibleBands, true, isLeft, isRight, emptyLeftSibling, layoutAssigner);
			}
			return emptyLeftSibling;
		}
		ColumnPosition GetColumnPosition(bool isLeft) {
			return isLeft && !GetShowIndicator() ? ColumnPosition.Left : ColumnPosition.Middle;
		}
		bool GetShowIndicator() {
			if(DataControl == null)
				return false;
			ITableView tableView = View as ITableView;
			return tableView != null ? tableView.ShowIndicator : false;
		}
		void UpdateColumnsPositions(BandBase band, bool ownerIsLeft, bool ownerIsRight, bool hasEmptyLeftSibling, LayoutAssigner layoutAssigner) {
			foreach(BandRow row in band.ActualRows) {
				for(int i = 0; i < row.Columns.Count; i++) {
					ColumnBase column = row.Columns[i];
					ColumnPosition columnPosition = hasEmptyLeftSibling && i == 0 ? ColumnPosition.Left : GetColumnPosition(i == 0 && ownerIsLeft);
					layoutAssigner.SetColumnPosition(column, columnPosition);
					bool hasRightSibling = !(i == row.Columns.Count - 1 && ownerIsRight);
					layoutAssigner.SetHasRightSibling(column, hasRightSibling);
					bool hasLeftSibling = !(i == 0 && ownerIsLeft);
					layoutAssigner.SetHasLeftSibling(column, hasLeftSibling);
				}
			}
		}
		void UpdateColumnsHasTopBottomElement() {
			if(DataControl == null)
				return;
			foreach(ColumnBase col in DataControl.ColumnsCore) {
				col.UpdateHasTopElement();
				col.UpdateHasBottomElement();
			}
		}
		void IBandsOwner.OnColumnsChanged(NotifyCollectionChangedEventArgs e) {
			if(e.Action == NotifyCollectionChangedAction.Reset && DataControl != null && !IsViewLockUpdateColumns)
				FillColumns();
			else {
				RemoveColumnsFromGrid(e.OldItems);
				AddColumnsToGrid(e.NewItems);
			}
		}
		void IBandsOwner.OnLayoutPropertyChanged() {
			UpdateBandsLayout();
		}
		void OnBandAdded(BandBase band) {
			if(IsViewLockUpdateColumns) return;
			band.BandsLayout = this;
			AddColumnsToGrid(band.ColumnsCore);
			AddChild(band);
			foreach(BandBase b in band.BandsCore)
				OnBandAdded(b);
		}
		void OnBandRemoved(BandBase band) {
			if(IsViewLockUpdateColumns) return;
			band.BandsLayout = null;
			RemoveColumnsFromGrid(band.ColumnsCore);
			RemoveChild(band);
			foreach(BandBase b in band.BandsCore)
				OnBandRemoved(b);
		}
		bool IsViewLockUpdateColumns { 
			get {
				if(View != null && View.IsLockUpdateColumnsLayout)
					return !dragDropLocker.IsLocked || !DataControl.DesignTimeAdorner.IsDesignTime;
				return false;
			} 
		}
		void RemoveColumnsFromGrid(IList source) {
			if(DataControl == null || IsViewLockUpdateColumns) return;
			if(source != null)
				foreach(ColumnBase column in source) {
					if(DataControl.ColumnsCore.Contains(column))
						DataControl.ColumnsCore.Remove(column);
				}
		}
		void AddColumnsToGrid(IList source) {
			if(DataControl == null || IsViewLockUpdateColumns) return;
			if(source != null)
				foreach(ColumnBase column in source) {
					if(ShouldAddColumnToDataControl(column))
						DataControl.ColumnsCore.Add(column);
				}
		}
		bool ShouldAddColumnToDataControl(ColumnBase column) {
			return !DataControl.ColumnsCore.Contains(column) && !column.IsServiceColumn();
		}
		protected override IEnumerator LogicalChildren {
			get { return DataControl != null ? new BandIterator<BandBase>(BandsCore) : (new object[0]).GetEnumerator(); }
		}
		void AddChild(BandBase band) {
			AddLogicalChild(band);
		}
		void RemoveChild(BandBase band) {
			RemoveLogicalChild(band);
		}
#if DEBUGTEST
		public int RepopulateGridColumnsCount { get; private set; }
#endif
		internal void FillColumns() {
#if DEBUGTEST
			RepopulateGridColumnsCount++;
#endif
			DataControl.ColumnsCore.BeginUpdate();
			DataControl.ColumnsCore.Clear();
			ForeachBand(band => {
				band.BandsLayout = this;
				foreach(ColumnBase column in band.ColumnsCore) {
					if(!column.IsServiceColumn())
						DataControl.ColumnsCore.Add(column);
				}
			});
			DataControl.ColumnsCore.EndUpdate();
		}
		internal virtual void ForeachBand(Action<BandBase> action) {
			BandIterator<BandBase> iterator = new BandIterator<BandBase>(BandsCore);
			foreach(BandBase band in iterator)
				action(band);
		}
		internal void ForeachVisibleBand(Action<BandBase> action) {
			ForeachVisibleBand(this, action);
		}
		internal static void ForeachVisibleBand(IEnumerable bands, Action<BandBase> action) {
			foreach(BandBase band in bands) {
				action(band);
				ForeachVisibleBand(band.VisibleBands, action);
			}
		}
		void ForeachVisibleBand(IBandsOwner owner, Action<BandBase> action) {
			ForeachVisibleBand(owner.VisibleBands, action);
		}
		int GetTargetVisibleIndex(BaseColumn column) {
			if(column is IBandsOwner) {
				return GetLastColumnVisibleIndex(DataControl.BandsLayoutCore, column as BandBase, -1) + 1;
			}
			return column.VisibleIndex;
		}
		int GetLastColumnVisibleIndex(IBandsOwner owner, BandBase band, int visibleIndex) {
			foreach(BandBase subBand in owner.BandsCore) {
				if(subBand == band) break;
				if(subBand.BandsCore.Count != 0)
					visibleIndex = GetLastColumnVisibleIndex(subBand, band, visibleIndex);
				else if(subBand.ColumnsCore.Count != 0){
					int maxIndex = -1;
					foreach(BaseColumn column in subBand.ColumnsCore) {
						maxIndex = Math.Max(maxIndex, column.VisibleIndex);
					}
					visibleIndex = maxIndex;
				}
			}
			return visibleIndex;
		}
		Locker dragDropLocker = new Locker();
		internal void DoMoveAction(Action action) {
			try {
				View.UpdateAllDependentViews(view => {
					view.BeginUpdateColumnsLayout();
					view.DataControl.BandsLayoutCore.dragDropLocker.Lock();
				});
				action();
			} finally {
				View.UpdateAllDependentViews(view => {
					view.DataControl.BandsLayoutCore.dragDropLocker.Unlock();
					view.EndUpdateColumnsLayout();
				});
			}
			View.UpdateContentLayout();
		}
		protected internal virtual void MoveColumnToBand(BaseColumn source, BandBase target, BandedViewDropPlace dropPlace, HeaderPresenterType moveFrom) {
			if(dropPlace == BandedViewDropPlace.Bottom)
				MoveColumnTo(source, target.ActualRows[0].Columns[0], BandedViewDropPlace.Top, moveFrom);
			else
				MoveColumnTo(source, target, dropPlace, moveFrom);
		}
		protected internal virtual void MoveBandTo(BandBase source, BandBase target, BandedViewDropPlace dropPlace) {
			DoMoveAction(delegate {
				BandMoveProvider.StartMovingBand();
				if(source.Visible) {
					for(int i = source.Owner.VisibleBands.IndexOf(source) + 1; i < source.Owner.VisibleBands.Count; i++)
						BandMoveProvider.SetVisibleIndex(source.Owner.VisibleBands[i], source.Owner.VisibleBands[i].VisibleIndex - 1);
				}
				int visibleIndex = target.VisibleIndex;
				if(dropPlace == BandedViewDropPlace.Right)
					visibleIndex++;
				if(dropPlace == BandedViewDropPlace.Bottom)
					visibleIndex = 0;
				BandMoveProvider.SetVisibleIndex(source, visibleIndex);
				source.Visible = true;
				if(dropPlace != BandedViewDropPlace.Bottom) {
					foreach(BandBase band in target.Owner.VisibleBands) {
						if(band != source && band.VisibleIndex >= source.VisibleIndex)
							BandMoveProvider.SetVisibleIndex(band, band.VisibleIndex + 1);
					}
				}
				if((source.Owner != target.Owner || dropPlace == BandedViewDropPlace.Bottom) && DesignerHelper.GetValue(this, AllowChangeBandParent, true)) {
					DataControl.GetOriginationDataControl().syncPropertyLocker.DoLockedAction(() => {
						Func<DataControlBase, BaseColumn> getCloneSource = source.CreateCloneAccessor();
						Func<DataControlBase, BaseColumn> getCloneTarget = target.CreateCloneAccessor();
						DataControlOriginationElementHelper.EnumerateDependentElemetsIncludingSource<DataControlBase>(DataControl,
							dc => dc,
							dc => {
								var cloneSource = (BandBase)getCloneSource(dc);
								var cloneTarget = (BandBase)getCloneTarget(dc);
								MoveBandsCore(cloneSource, cloneTarget, dropPlace);
							},
							dc => dc.BandsCore.Clear());
					});
				}
				BandMoveProvider.EndMovingBand();
			});
		}
		void MoveBandsCore(BandBase source, BandBase target, BandedViewDropPlace dropPlace) {
			BandMoveProvider.RemoveBand(source);
			if(dropPlace == BandedViewDropPlace.Bottom) {
				if(target.BandsCore.Count != 0) {
					BandMoveProvider.MoveBands(target, source);
				}
				if(target.ColumnsCore.Count != 0) {
					BandMoveProvider.MoveColumns(target, source);
				}
				BandMoveProvider.AddBand(target, ref source);
				if(source.ColumnsCore.Count != 0 && source.BandsCore.Count != 0) {
					BandBase lastVisibleBand = FindVisibleBand(source);
					if(lastVisibleBand != source)
						BandMoveProvider.MoveColumns(source, lastVisibleBand);
				}
			} else {
				BandMoveProvider.AddBand(target.Owner, ref source);
			}
		}
		BandBase FindVisibleBand(BandBase rootBand) {
			foreach(BandBase band in rootBand.BandsCore) {
				if(band.Visible)
					return FindVisibleBand(band);
			}
			return rootBand;
		}
		protected IBandMoveProvider BandMoveProvider { get { return DataControl.DesignTimeAdorner.BandMoveProvider; } }
		protected IColumnMoveToBandProvider ColumnMoveProvider { get { return DataControl.DesignTimeAdorner.ColumnMoveToBandProvider; } }
		protected virtual void OnMoveColumn(BaseColumn source, HeaderPresenterType moveFrom) {
		}
		protected internal virtual void MoveColumnTo(BaseColumn source, BaseColumn target, BandedViewDropPlace dropPlace, HeaderPresenterType moveFrom) {
			DoMoveAction(delegate {
				OnMoveColumn(source, moveFrom);
				ColumnMoveProvider.StartMoving();
				if(moveFrom == HeaderPresenterType.Headers) {
					for(int i = View.VisibleColumnsCore.IndexOf((ColumnBase)source) + 1; i < View.VisibleColumnsCore.Count; i++)
						ColumnMoveProvider.SetVisibleIndex(View.VisibleColumnsCore[i], View.VisibleColumnsCore[i].VisibleIndex - 1);
				}
				int visibleIndex = GetTargetVisibleIndex(target);
				if(dropPlace == BandedViewDropPlace.Right)
					visibleIndex++;
				if(dropPlace == BandedViewDropPlace.Top)
					visibleIndex = target.BandRow.Columns[0].VisibleIndex;
				if(dropPlace == BandedViewDropPlace.Bottom)
					visibleIndex = target.BandRow.Columns[target.BandRow.Columns.Count - 1].VisibleIndex + 1;
				ColumnMoveProvider.SetVisibleIndex(source as ColumnBase, visibleIndex);
				source.Visible = true;
				foreach(BaseColumn column in DataControl.ColumnsCore) {
					if(column != source && column.VisibleIndex >= source.VisibleIndex)
						ColumnMoveProvider.SetVisibleIndex(column as ColumnBase, column.VisibleIndex + 1);
				}
				if(source.BandRow != null && source.BandRow.Columns.Count == 1) {
					for(int i = source.ParentBandInternal.ActualRows.IndexOf(source.BandRow) + 1; i < source.ParentBandInternal.ActualRows.Count; i++) {
						foreach(BaseColumn column in source.ParentBandInternal.ActualRows[i].Columns)
							ColumnMoveProvider.SetRow(column, BandBase.GetGridRow(column) - 1);
					}
				}
				int rowIndex = BandBase.GetGridRow(target);
				if(dropPlace == BandedViewDropPlace.Top || dropPlace == BandedViewDropPlace.Bottom) {
					if(dropPlace == BandedViewDropPlace.Bottom)
						rowIndex++;
					for(int i = target.ParentBandInternal.ActualRows.IndexOf(target.BandRow); i < target.ParentBandInternal.ActualRows.Count; i++) {
						if(target.ParentBandInternal.ActualRows[i] == target.BandRow && dropPlace == BandedViewDropPlace.Bottom) continue;
						foreach(BaseColumn column in target.ParentBandInternal.ActualRows[i].Columns)
							if(column != source)
								ColumnMoveProvider.SetRow(column, BandBase.GetGridRow(column) + 1);
					}
				}
				ColumnMoveProvider.SetRow(source, rowIndex);	  
				DataControl.GetOriginationDataControl().syncPropertyLocker.DoLockedAction(() => {
					Func<DataControlBase, BaseColumn> getCloneSource = source.CreateCloneAccessor();
					Func<DataControlBase, BaseColumn> getCloneTarget = target.CreateCloneAccessor();
					DataControlOriginationElementHelper.EnumerateDependentElemetsIncludingSource<DataControlBase>(DataControl,
						dc => dc,
						dc => {
							BaseColumn cloneSource = getCloneSource(dc);
							BaseColumn cloneTarget = getCloneTarget(dc);
							ColumnMoveProvider.MoveColumnToBand(cloneSource, cloneSource.ParentBandInternal, cloneTarget.ParentBandInternal);
						},
						dc => dc.BandsCore.Clear());
				});
				ColumnMoveProvider.EndMoving();
				UpdateBandsLayout();
			});
		}
		internal void ApplyColumnVisibleIndex(BaseColumn column) {
			if(View.IsLockUpdateColumnsLayout) return;
			DoMoveAction(delegate {
				List<BaseColumn> parentCollection = null;
				BandBase band = column as BandBase;
				if(band != null) {
					parentCollection = band.Owner.VisibleBands.ToList<BaseColumn>();
				}
				else {
					if(column.BandRow == null)
						return;
					parentCollection = column.BandRow.Columns.ToList<BaseColumn>();
				}
				parentCollection.Sort((column1, column2) => Comparer<int>.Default.Compare(column1.VisibleIndex, column2.VisibleIndex));
				for(int i = 0; i < parentCollection.Count; i++) {
					parentCollection[i].VisibleIndex = i;
				}
			});
		}
		void UpdateViewInfo() {
			if(View != null)
				View.UpdateColumnsViewInfo();
		}
		internal IList GetBands(BandBase band, bool isLeft, bool skipNotFixedBands = false) {
			return GetBands(band, new List<BandBase>(), isLeft, skipNotFixedBands);
		}
		bool IsFixedBand(BandBase band) {
			return band.Owner != this || band.Fixed != FixedStyle.None;
		}
		internal bool SkipItem(int itemIndex, int currentIndex, bool isLeft) {
			if(itemIndex == -1) return false;
			if(isLeft)
				return currentIndex >= itemIndex;
			return currentIndex <= itemIndex;
		}
		IList GetBands(BandBase band, IList bands, bool isLeft, bool skipNotFixedBands) {
			if(DesignerProperties.GetIsInDesignMode(this) && band.Owner == null) return bands;
			int bandIndex = band.Owner.VisibleBands.IndexOf(band);
			for(int i = 0; i < band.Owner.VisibleBands.Count; i++) {
				if(SkipItem(bandIndex, i, isLeft)) continue;
				if(skipNotFixedBands && !IsFixedBand(band.Owner.VisibleBands[i])) continue;
				bands.Add(band.Owner.VisibleBands[i]);
			}
			if(band.Owner is BandBase)
				return GetBands((BandBase)band.Owner, bands, isLeft, skipNotFixedBands);
			return bands;
		}
		[Browsable(false)]
		public bool ShouldSerializeColumnChooserBands(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeColumnChooserBandsSortOrderComparer(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedLeftVisibleBands(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedRightVisibleBands(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedNoneVisibleBands(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		#region printing
		internal List<ColumnBase> GetVisibleColumns() {
			List<ColumnBase> visibleColumns = new List<ColumnBase>();
			ForeachVisibleBand((band) => {
				foreach(BandRow row in band.ActualRows)
					foreach(ColumnBase column in row.Columns)
						visibleColumns.Add(column);
			});
			return visibleColumns;
		}
		internal virtual BandsLayoutBase CloneAndFillEmptyBands() {
			BandsLayoutBase clone = (BandsLayoutBase)Activator.CreateInstance(this.GetType());
			clone.VisibleBands = ClonePrintBands();
			return clone;
		}
		List<BandBase> ClonePrintBands() {
			List<BandBase> cloneBands = new List<BandBase>();
			ClonePrintBands(PrintableBands, cloneBands);
			return cloneBands;
		}
		void ClonePrintBands(IEnumerable<BandBase> sourceBands, IList<BandBase> cloneBands) {
			foreach(BandBase band in sourceBands)
				cloneBands.Add(ClonePrintBand(band));
		}
		BandBase ClonePrintBand(BandBase source) {
			BandBase clone = Activator.CreateInstance(source.GetType()) as BandBase;
			clone.ActualHeaderWidth = source.ActualHeaderWidth;
			if(source.PrintableBands.Any())
				ClonePrintBands(source.PrintableBands, clone.VisibleBands);
			else if(HasColumns(source))
				CloneActualRows(source, clone);
			else
				CreateFakeColumn(source, clone);
			return clone;
		}
		bool HasColumns(BandBase band) {
			return band.ActualRows.SelectMany(row => row.Columns).Any(column => column.AllowPrinting);
		}
		void CloneActualRows(BandBase source, BandBase clones) {
			foreach(BandRow row in source.ActualRows) {
				BandRow cloneRow = new BandRow() { Columns = new List<ColumnBase>() };
				foreach(ColumnBase column in row.Columns)
					if(column.AllowPrinting)
						cloneRow.Columns.Add(column);
				clones.ActualRows.Add(cloneRow);
			}
		}
		void CreateFakeColumn(BandBase sourceBand, BandBase cloneBand) {
			BandRow cloneRow = new BandRow() { Columns = new List<ColumnBase>() };
			ColumnBase fakeColumn = DataControl.CreateColumn();
			fakeColumn.EditSettings = new DevExpress.Xpf.Editors.Settings.TextEditSettings();
			fakeColumn.HasLeftSibling = sourceBand.HasLeftSibling;
			fakeColumn.HasRightSibling = sourceBand.HasRightSibling;
			fakeColumn.UpdateActualPrintProperties(View);
			SetPrintWidth(fakeColumn, sourceBand.ActualHeaderWidth);
			cloneRow.Columns.Add(fakeColumn);
			cloneBand.ActualRows.Add(cloneRow);
		}
		protected virtual void SetPrintWidth(BaseColumn column, double width) { }
		internal BandBase GetRootBand(BandBase band) {
			BandBase ownerBand = band.Owner as BandBase;
			return ownerBand == null ? band : GetRootBand(ownerBand);
		}
		#endregion
		internal void CloneBandsCollection(IBandsCollection destination) {
			CloneBandsCollection(BandsCore, destination, CloneDetailHelper.CloneCollection<BaseColumn>);
		}
		internal void CopyBandCollection(IBandsCollection destination) {
			CloneBandsCollection(BandsCore, destination, CloneDetailHelper.CopyToCollection<BaseColumn>);
		}
		internal static BandBase CloneBand(BandBase source) {
			BandBase destination = (BandBase)CloneDetailHelper.CloneElement<BaseColumn>(source);
			CloneInnerBandsCollection(source, destination, CloneDetailHelper.CloneCollection<BaseColumn>);
			return destination;
		}
		static void CloneBandsCollection(IBandsCollection source, IBandsCollection destination, Action<IList,IList> cloneAction) {
			if(source.Count == 0)
				return;
			cloneAction(source, destination);
			for(int i = 0; i < source.Count; i++)
				CloneInnerBandsCollection((BandBase)source[i], (BandBase)destination[i], cloneAction);
		}
		static void CloneInnerBandsCollection(BandBase source, BandBase destination, Action<IList, IList> cloneAction) {
			CloneBandsCollection(source.BandsCore, destination.BandsCore, cloneAction);
			cloneAction(source.ColumnsCore, destination.ColumnsCore);
		}
		internal void UpdateShowIndicator(bool showIndicator) {
			ShowIndicator = DataControl.GetRootDataControl() == DataControl && showIndicator;
		}
	}
}
