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
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Utils.Serializing.Helpers;
using System.ComponentModel;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Grid {
	public abstract class BandBase : BaseColumn, IBandsOwner {
		public static readonly DependencyProperty GridColumnProperty;
		public static readonly DependencyProperty GridRowProperty;
		public static readonly DependencyProperty PrintBandHeaderStyleProperty;
		public static readonly DependencyProperty ActualPrintBandHeaderStyleProperty;
		static readonly DependencyPropertyKey ActualPrintBandHeaderStylePropertyKey;
		static BandBase() {
			Type ownerType = typeof(BandBase);
			GridColumnProperty = DependencyProperty.RegisterAttached("GridColumn", typeof(int), ownerType, new PropertyMetadata(0, OnGridColumnChanged));
			GridRowProperty = DependencyProperty.RegisterAttached("GridRow", typeof(int), ownerType, new PropertyMetadata(0, OnGridRowChanged));
			PrintBandHeaderStyleProperty = DependencyProperty.Register("PrintBandHeaderStyle", typeof(Style), ownerType, new PropertyMetadata(null, (d, e) => ((BandBase)d).UpdateActualPrintBandHeaderStyle()));
			ActualPrintBandHeaderStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPrintBandHeaderStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			ActualPrintBandHeaderStyleProperty = ActualPrintBandHeaderStylePropertyKey.DependencyProperty;
			EventManager.RegisterClassHandler(ownerType, DXSerializer.FindCollectionItemEvent, new XtraFindCollectionItemEventHandler((s, e) => ((BandBase)s).OnDeserializeFindCollectionItem(e)));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.StartDeserializingEvent, new StartDeserializingEventHandler((s, e) => ((BandBase)s).OnDeserializeStart()));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.EndDeserializingEvent, new EndDeserializingEventHandler((s, e) => ((BandBase)s).OnDeserializeEnd()));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.ClearCollectionEvent, new XtraItemRoutedEventHandler((s, e) => ((BandBase)s).OnDeserializeClearCollection(e)));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.CreateCollectionItemEvent, new XtraCreateCollectionItemEventHandler((s, e) => ((BandBase)s).OnDeserializeCreateCollectionItem(e)));
			CloneDetailHelper.RegisterKnownAttachedProperty(BandBase.GridRowProperty);
		}
		public static int GetGridColumn(DependencyObject obj) {
			return (int)obj.GetValue(GridColumnProperty);
		}
		public static void SetGridColumn(DependencyObject obj, int value) {
			obj.SetValue(GridColumnProperty, value);
		}
		public static int GetGridRow(DependencyObject obj) {
			return (int)obj.GetValue(GridRowProperty);
		}
		public static void SetGridRow(DependencyObject obj, int value) {
			obj.SetValue(GridRowProperty, value);
		}
		static void OnGridColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		static void OnGridRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ColumnBase column = d as ColumnBase;
			if(column == null) return;
			if(column.OwnerControl != null && !column.OwnerControl.ColumnsCore.IsLockUpdate && column.OwnerControl.BandsLayoutCore != null)
				column.OwnerControl.BandsLayoutCore.UpdateBandsLayout();
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BandBasePrintBandHeaderStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintBandHeaderStyle {
			get { return (Style)GetValue(PrintBandHeaderStyleProperty); }
			set { SetValue(PrintBandHeaderStyleProperty, value); }
		}
		[ Category(Categories.AppearancePrint)]
		public Style ActualPrintBandHeaderStyle {
			get { return (Style)GetValue(ActualPrintBandHeaderStyleProperty); }
			private set { this.SetValue(ActualPrintBandHeaderStylePropertyKey, value); }
		}
		IBandsOwner ownerCore;
		internal IBandsOwner Owner {
			get { return ownerCore; }
			set {
				ownerCore = value;
				ParentBand = value as BandBase;
			}
		}
		DataControlBase IBandsOwner.DataControl { get { return Owner != null ? Owner.DataControl : null; } }
		internal List<BandBase> VisibleBands { get; private set; }
		internal IEnumerable<BandBase> PrintableBands { get { return VisibleBands.Where(band => band.AllowPrinting); } }
		List<BandBase> IBandsOwner.VisibleBands { get { return VisibleBands; } }
		IBandsOwner IBandsOwner.FindClone(DataControlBase dataControl) {
			return CreateCloneAccessor()(dataControl) as IBandsOwner;
		}
		BandsLayoutBase bandsLayout;
		internal BandsLayoutBase BandsLayout {
			get { return bandsLayout; }
			set {
				if(bandsLayout == value)
					return;
				bandsLayout = value;
				OnBandsLayoutChanged();
			}
		}
		internal void OnBandsLayoutChanged() {
			UpdateActualHeaderTemplateSelector();
			UpdateActualHeaderToolTipTemplate();
			UpdateActualPrintBandHeaderStyle();
		}
		internal override BandBase ParentBandInternal {
			get { return this; }
		}
		IBandsCollection bandsCore;
		internal IBandsCollection BandsCore {
			get {
				if(bandsCore == null)
					bandsCore = CreateBands();
				return bandsCore;
			} 
		}
		IBandsCollection IBandsOwner.BandsCore { get { return BandsCore; } }
		internal abstract IBandsCollection CreateBands();
		IBandColumnsCollection columnsCore;
		internal IBandColumnsCollection ColumnsCore {
			get {
				if(columnsCore == null)
					columnsCore = CreateColumns();
				return columnsCore; 
			} 
		}
		internal abstract IBandColumnsCollection CreateColumns();
		internal List<BandRow> ActualRows { get; private set; }
		ObservableCollection<BandRowDefinition> rowDefinitions;
		internal ObservableCollection<BandRowDefinition> RowDefinitions { get { return rowDefinitions; } }
		ObservableCollection<BandColumnDefinition> columnDefinitions;
		internal ObservableCollection<BandColumnDefinition> ColumnDefinitions { get { return columnDefinitions; } }
		BandsHelper bandsHelper;
		public BandBase() {
			bandsHelper = new BandsHelper(this, true);
			ColumnsCore.CollectionChanged += new NotifyCollectionChangedEventHandler(columns_CollectionChanged);
			rowDefinitions = new ObservableCollection<BandRowDefinition>();
			columnDefinitions = new ObservableCollection<BandColumnDefinition>();
			ActualRows = new List<BandRow>();
			VisibleBands = new List<BandBase>();
			HasBottomElement = true;
		}
		void columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			Action columnsChangedAction = () => {
				if(e.Action == NotifyCollectionChangedAction.Reset) {
					foreach(ColumnBase column in ColumnsCore)
						column.ParentBand = this;
				} else {
					if(e.OldItems != null) {
						foreach(ColumnBase column in e.OldItems)
							column.ParentBand = null;
					}
					if(e.NewItems != null) {
						foreach(ColumnBase column in e.NewItems)
							column.ParentBand = this;
					}
				}
				((IBandsOwner)this).OnColumnsChanged(e);
			};
			if(DataControl != null) {
				DataControl.GetOriginationDataControl().syncPropertyLocker.DoLockedAction(columnsChangedAction);
				Func<DataControlBase, BaseColumn> cloneAccessor = CreateCloneAccessor();
				DataControl.GetDataControlOriginationElement().NotifyCollectionChanged(DataControl,
					dc => ((BandBase)cloneAccessor(dc)).ColumnsCore,
					column => CloneDetailHelper.CloneElement<BaseColumn>((ColumnBase)column),
					e);
			} else
				columnsChangedAction();
		}
		void IBandsOwner.OnBandsChanged(NotifyCollectionChangedEventArgs e) {
			if(Owner != null)
				Owner.OnBandsChanged(e);
		}
		void IBandsOwner.OnColumnsChanged(NotifyCollectionChangedEventArgs e) {
			if(Owner != null)
				Owner.OnColumnsChanged(e);
		}
		void IBandsOwner.OnLayoutPropertyChanged() {
			OnLayoutPropertyChanged();
		}
		#region serialization
		BandedViewSerializationHelper BandSerializationHelper { get { return DataControl.BandSerializationHelper; } }
		void OnDeserializeStart() {
			ColumnsCore.BeginUpdate();
			BandsCore.BeginUpdate();
		}
		void OnDeserializeEnd() {
			ColumnsCore.EndUpdate();
			BandsCore.EndUpdate();
		}
		void OnDeserializeCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			switch(e.CollectionName) {
				case SerializationPropertiesNames.Columns:
					if(!BandSerializationHelper.CanRemoveOldColumns)
						DataControl.OnDeserializeCreateColumn(e);
					break;
				case SerializationPropertiesNames.Bands:
					if(!BandSerializationHelper.CanRemoveOldColumns)
						DataControl.OnDeserializeCreateBand(e);
					break;
			}
		}
		void OnDeserializeClearCollection(XtraItemRoutedEventArgs e) {
			switch(e.Item.Name) {
				case SerializationPropertiesNames.Columns:
				case SerializationPropertiesNames.Bands:
					if(!BandSerializationHelper.CanAddNewColumns)
						BandSerializationHelper.ClearCollection(e);
					break;
			}
		}
		void OnDeserializeFindCollectionItem(XtraFindCollectionItemEventArgs e) {
			switch(e.CollectionName) {
				case SerializationPropertiesNames.Columns:
					BandSerializationHelper.FindColumn(e);
					break;
				case SerializationPropertiesNames.Bands:
					BandSerializationHelper.FindBand(e, this);
					break;
			}
		}
		#endregion
		internal protected override DataTemplate GetActualTemplate() {
			if(HeaderTemplate == null && BandsLayout != null)
				return BandsLayout.BandHeaderTemplate;
			return HeaderTemplate;
		}
		internal protected override DataTemplateSelector GetActualTemplateSelector() {
			if(HeaderTemplateSelector == null && BandsLayout != null)
				return BandsLayout.BandHeaderTemplateSelector;
			return HeaderTemplateSelector;
		}
		protected internal override void UpdateActualHeaderToolTipTemplate() {
			if(HeaderToolTipTemplate != null)
				ActualHeaderToolTipTemplate = HeaderToolTipTemplate;
			else if(BandsLayout != null)
				ActualHeaderToolTipTemplate = BandsLayout.BandHeaderToolTipTemplate;
		}
		internal void UpdateActualPrintBandHeaderStyle() {
			if(PrintBandHeaderStyle != null)
				ActualPrintBandHeaderStyle = PrintBandHeaderStyle;
			else if(BandsLayout != null)
				ActualPrintBandHeaderStyle = BandsLayout.PrintBandHeaderStyle;
		}
		protected override void OnLayoutPropertyChanged() {
			if(Owner != null)
				Owner.OnLayoutPropertyChanged();
		}
		protected internal DataControlBase DataControl { get { return Owner != null ? Owner.DataControl : null; } }
		protected internal override IColumnOwnerBase ResizeOwner { get { return DataControl != null ? DataControl.DataView : null; } }
		protected internal override bool IsBand { get { return true; } }
		protected override bool OwnerAllowResizing { get { return BandsLayout != null ? BandsLayout.AllowBandResizing : true; } }
		protected override bool OwnerAllowMoving { get { return BandsLayout != null ? BandsLayout.AllowBandMoving : true; } }
		protected override void OnActualHeaderWidthChanged() {
			base.OnActualHeaderWidthChanged();
			UpdateContentLayout();
		}
		protected internal override bool CanStartDragSingleColumn {
			get {
				return Owner != DataControl.BandsLayoutCore || Fixed != FixedStyle.None || DataControl.BandsLayoutCore.FixedNoneVisibleBands.Count != 1;
			}
		}
		protected internal override bool AllowChangeParent { get { return BandsLayout.AllowChangeBandParent; } }
		protected internal override bool CanDropTo(BaseColumn target) {
			BandBase current = target.ParentBandInternal;
			while(current != null) {
				if(current == this) return false;
				current = current.Owner as BandBase;
			}
			return true;
		}
		internal override Func<DataControlBase, BaseColumn> CreateCloneAccessor() {
			return BandWalker.CreateBandCloneAccessor(this);
		}
		internal override DataControlBase GetNotifySourceControl() {
			return DataControl;
		}
	}
	class BandRow {
		public List<ColumnBase> Columns { get; set; }
	}
}
