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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid.Customization;
using DevExpress.XtraGrid.Views.Grid.Customization;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraEditors;
using DevExpress.Export;
namespace DevExpress.XtraGrid.Views.BandedGrid {
	[Designer("DevExpress.XtraGrid.Design.BandedGridViewDesignTimeDesigner, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.ComponentModel.Design.IDesigner))]
	public class BandedGridView : GridView {
		BandedGridState _bandedState;
		GridBandCollection bands;
		int bandPanelRowHeight, minBandPanelRowCount;
		private static readonly object customDrawBandHeader = new object();
		private static readonly object bandWidthChanged = new object();
		public BandedGridView(GridControl ownerGrid) : this() {
			SetGridControl(ownerGrid);
		}
		public BandedGridView() {
			this._bandedState = BandedGridState.Normal;
			this.bandPanelRowHeight = -1;
			this.minBandPanelRowCount = 1;
			this.bands = CreateBands();
			Bands.CollectionChanged += new CollectionChangeEventHandler(OnBandCollectionChanged);
		}
		protected virtual GridBandCollection CreateBands() {
			return new GridBandCollection(this, null);
		}
		protected override void CheckCheckboxSelector() { }
		protected override BaseViewInfo CreateNullViewInfo() { return new NullBandedGridViewInfo(this); }
		protected override GridOptionsCustomization CreateOptionsCustomization() { return new BandedGridOptionsCustomization(); }
		protected override ViewPrintOptionsBase CreateOptionsPrint() { return new BandedGridOptionsPrint(); }
		protected override GridOptionsHint CreateOptionsHint() { return new BandedGridOptionsHint(); }
		protected override ColumnViewOptionsView CreateOptionsView() { return new BandedGridOptionsView(); }
		protected override void OnOptionChanged(object sender, BaseOptionChangedEventArgs e) {
			base.OnOptionChanged(sender, e);
			if(sender == OptionsCustomization) {
				RecreateColumnsCustomization();
			}
		}
		protected override BaseViewPrintInfo CreatePrintInfoInstance(PrintInfoArgs args) {
			return new BandedGridViewPrintInfo(args);
		}
		protected internal override void InitializeNew() {
			this.Bands.Add();
			base.InitializeNew();
		}
		protected override bool IsSupportRightToLeft {
			get {
				return true;
			}
		}
		protected override void DesignerMakeColumnsVisible() {
			bool needUpdate = false;
			foreach(BandedGridColumn column in Columns) {
				if(column.OwnerBand == null) needUpdate = true;
			}
			if(!needUpdate || Bands.Count != 1) return;
			BeginUpdate();
			try {
				foreach(BandedGridColumn column in Columns) {
					column.Visible = true;
					column.OwnerBand = Bands[0];
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override void OnLoaded() {
			BeginUpdate();
			try {
				CheckOwners();
				Bands.SetViewCore(this);
				base.OnLoaded();
			} finally {
				EndUpdateCore(true);
			}
		}
		void CheckOwners() {
			foreach(BandedGridColumn column in Columns) {
				if(column.OwnerBand != null && column.OwnerBand.View != this) {
					Bands.Add(column.OwnerBand);
				}
			}
		}
		bool bandedDestroying = false;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				fViewDisposing = true;
				if(bandedDestroying) return;
				BeginUpdate();
				bandedDestroying = true;
				DestroyBands();
			}
			base.Dispose(disposing);
		}
		protected virtual void DestroyBands() {
			Bands.DestroyBands();
		}
		protected override string ViewName { get { return "BandedGridView"; } }
		protected new BandedGridPainter Painter { get { return base.Painter as BandedGridPainter; } }
		protected new BandedGridViewInfo ViewInfo { get { return base.ViewInfo as BandedGridViewInfo; } }
		protected internal override ViewDrawArgs CreateDrawArgs(DXPaintEventArgs e, GraphicsCache cache) {
			if(cache == null) cache = new GraphicsCache(e, Painter.Paint);
			return new BandedGridViewDrawArgs(cache, ViewInfo as BandedGridViewInfo, ViewInfo.ViewRects.Bounds);
		}
		protected internal virtual void FireChangedBands() {
			if(IsDeserializing || GridControl == null || !IsDesignMode) return;
			foreach(GridBand band in Bands) {
				GridControl.FireChanged(band);
			}
		}
		protected internal virtual void OnBandColumnCollectionChanged(object sender, CollectionChangeEventArgs e) {
			GridBand band = sender as GridBand;
			if(band == null || IsLoading) return;
			BandedGridColumn col = e.Element as BandedGridColumn;
			if(e.Action == CollectionChangeAction.Add) {
				if(col.View != this) Columns.Add(col);
				if(col != null && !col.WidthLocked && col.Visible) {
					if(band.Columns.Count == 1) 
						band.Width = col.width;
					else
						band.Width += col.Width;
				}
			}
			if(band.ReallyVisible) OnPropertiesChanged();
		}
		protected override ToolTipControlInfo GetToolTipObjectInfoCore(GraphicsCache cache, Point p) {
			BandedGridHitInfo ht = GetHintObjectInfo() as BandedGridHitInfo;
			if(ht != null && ht.InBandPanel && ht.Band != null) {
				if(GridControl.IsDesignMode) return new ToolTipControlInfo(ht.Band, GetToolTipText(ht.Band, p));
				if(!OptionsHint.ShowBandHeaderHints) return null;
				if(ht.Band.ToolTip != string.Empty) return new ToolTipControlInfo(ht.Band, GetToolTipText(ht.Band, p));
				if(!ht.Band.OptionsBand.ShowCaption) return null;
				GridBandInfoArgs bi = ViewInfo.BandsInfo.FindBand(ht.Band);
				if(bi != null) {
					bool fit = Painter.ElementsPainter.Band.IsCaptionFit(ViewInfo.GInfo.Cache, bi);
					if(!fit) return new ToolTipControlInfo(ht.Band, GetToolTipText(ht.Band, p));
				}
				return null;
			}
			return base.GetToolTipObjectInfoCore(cache, p);
		}
		protected internal override string GetToolTipText(object hintObject, Point p) {
			GridBand band = hintObject as GridBand;
			if(band != null) {
				if(GridControl.IsDesignMode) {
					return GridDesignTimeHints.DTBand;
				}
				return band.ToolTip != string.Empty ? band.ToolTip : band.GetTextCaption();
			}
			return base.GetToolTipText(hintObject, p);
		}
		protected override bool IsFirstVisibleColumn(GridColumn column) {
			BandedGridColumn bc = column as BandedGridColumn;
			if(bc.ColVIndex != 0 || !bc.Visible) return false;
			if(bc.OwnerBand == null) return false;
			GridBand band = bc.OwnerBand;
			while(band != null) {
				if(band.VisibleIndex > 0 || !band.Visible) return false;
				band = band.ParentBand;
			}
			return true;
		}
		bool requireRefreshBands = false;
		protected internal virtual void OnBandCollectionChanged(object sender, CollectionChangeEventArgs e) {
			GridBandCollection coll = sender as GridBandCollection;
			GridBand band = e.Element as GridBand;
			if(band == null) return;
			switch(e.Action) {
				case CollectionChangeAction.Add :
					if(coll != null) coll.SetViewCore(this);
					OnBandAdded(band);
					if(band.Collection == null) return;
					break;
				case CollectionChangeAction.Remove:
					if(ViewDisposing || (GridControl != null && GridControl.GridDisposing)) return;
					OnBandDeleted(band);
					break;
			}
			if(IsLoading || IsDeserializing) return;
			if(sender == Bands) {
				if(!IsLockUpdate)
				RefreshBandList();
				else {
					requireRefreshBands = true;
				}
			}
			OnPropertiesChanged();
			FireChanged();
		}
		protected override void OnAfterDeserializeCollection(object property, XtraItemEventArgs e) {
			base.OnAfterDeserializeCollection(property, e);
			if(e.Collection == Bands) RefreshBandList();
		}
		protected virtual void OnBandAdded(GridBand band) {
			if(IsLoading && !IsDeserializing) return;
			if(IsLoading) return;
		}
		protected virtual void OnBandDeleted(GridBand band) {
		}
		protected override CustomizationForm CreateCustomizationForm() {
			return new BandedCustomizationForm(this);
		}
		protected override GridColumnCollection CreateColumnCollection() {
			return new BandedGridColumnCollection(this);
		}
		protected override bool CanUseFixedStyle { get { return false; } } 
		protected override BaseViewAppearanceCollection CreateAppearancesPrint() { return new BandedViewPrintAppearances(this); }
		void ResetAppearancePrint() { AppearancePrint.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridViewAppearancePrint"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public new BandedViewPrintAppearances AppearancePrint { get { return base.AppearancePrint as BandedViewPrintAppearances; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BandedViewAppearances PaintAppearance { get { return base.PaintAppearance as BandedViewAppearances; } }
		protected override BaseViewAppearanceCollection CreateAppearances() { return new BandedViewAppearances(this); }
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridViewAppearance"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public new BandedViewAppearances Appearance { get { return base.Appearance as BandedViewAppearances; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdColumns)]
		public new BandedGridColumnCollection Columns { get { return base.Columns as BandedGridColumnCollection; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridViewMinBandPanelRowCount"),
#endif
 DefaultValue(1), XtraSerializableProperty()]
		public virtual int MinBandPanelRowCount {
			get { return minBandPanelRowCount; }
			set {
				if(value < 1) value = 1;
				if(MinBandPanelRowCount == value) return;
				minBandPanelRowCount = value;
				OnPropertiesChanged();
			}
		}
		internal void XtraClearBands(XtraItemEventArgs e) {	Bands.ClearBandItems(e); }
		internal object XtraCreateBandsItem(XtraItemEventArgs e) { return Bands.CreateBandItem(e); }
		internal object XtraFindBandsItem(XtraItemEventArgs e) { return Bands.FindBandItem(e); }
		internal void XtraSetIndexBandsItem(XtraSetItemIndexEventArgs e) { Bands.SetBandItemIndex(e); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 1, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdColumns)]
		public GridBandCollection Bands { get { return bands; } }
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridViewOptionsView"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new BandedGridOptionsView OptionsView {
			get { return base.OptionsView as BandedGridOptionsView; }
		}
		bool ShouldSerializeOptionsHint() { return OptionsHint.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridViewOptionsHint"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new BandedGridOptionsHint OptionsHint {
			get { return base.OptionsHint as BandedGridOptionsHint; }
		}
		bool ShouldSerializeOptionsPrint() { return OptionsPrint.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridViewOptionsPrint"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new BandedGridOptionsPrint OptionsPrint {
			get { return base.OptionsPrint as BandedGridOptionsPrint; }
		}
		bool ShouldSerializeOptionsCustomization() { return OptionsCustomization.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridViewOptionsCustomization"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new BandedGridOptionsCustomization OptionsCustomization {
			get { return base.OptionsCustomization as BandedGridOptionsCustomization; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridViewBandPanelRowHeight"),
#endif
 DefaultValue(-1), XtraSerializableProperty()]
		public virtual int BandPanelRowHeight {
			get { return bandPanelRowHeight; }
			set {
				if(value < 0) value = -1;
				if(value > 100) value = 100;
				if(BandPanelRowHeight == value) return;
				bandPanelRowHeight = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public override bool IsDraggingState { get { return State == BandedGridState.BandDragging || base.IsDraggingState; } }
		[Browsable(false)]
		public override bool IsSizingState { get { return State == BandedGridState.BandSizing || base.IsSizingState; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BandedGridState State {
			get { return _bandedState; }
		}
		protected internal override int StateInt { get { return (int)_bandedState; } }
		internal void SetStateCore(BandedGridState val) { SetState((int)val); }
		const int BandedGridStateStart = 100;
		protected override void SetStateInt(int newState) {
			this._bandedState = (BandedGridState)newState;
			if(newState >= BandedGridStateStart) newState = (int)GridState.Unknown;
			base.SetStateInt(newState);
		}
		protected override void OnStateChanged() {
			if(State == BandedGridState.BandDown) return;
			base.OnStateChanged();
		}
		protected internal virtual Image GetMaxHeightBandImage() {
			Image res = null;
			for(int n = 0; n < Math.Min(ColumnViewInfo.AutoHeightCalculateMaxColumnCount, Bands.Count); n++) {
				GridBand band = Bands[n];
				if(res == null || (band.Image != null && band.Image.Height > res.Height)) res = band.Image;
			}
			return res;
		}
		protected internal virtual int GetColumnVisibleIndex(BandedGridColumn column) {
			if(column == null || column.OwnerBand == null) return -1;
			int index = 0;
			foreach(BandedGridColumn col in column.OwnerBand.Columns) {
				if(col == column) return index;
				if(col.Visible) index ++;
			}
			return -1;
		}
		protected internal virtual void SetColumnVisibleIndex(BandedGridColumn column, int colVIndex) {
			if(column.OwnerBand == null) return;
			if(colVIndex < 0) {
				column.Visible = false;
				return;
			}
			int curIndex = GetColumnVisibleIndex(column);
			if(curIndex == colVIndex && column.Visible) return;
			BeginUpdate();
			try {
				column.Visible = true;
				if(colVIndex == 0) {
					column.OwnerBand.Columns.MoveTo(0, column);
					return;
				}
				curIndex = 0;
				for(int n = 0; n < column.OwnerBand.Columns.Count; n++) {
					BandedGridColumn col = column.OwnerBand.Columns[n];
					if(col == column) {
						continue;
					}
					if(col.Visible) curIndex ++;
					if(curIndex > colVIndex) {
						column.OwnerBand.Columns.MoveTo(n, column);
						return;
					}
				}
				column.OwnerBand.Columns.MoveTo(column.OwnerBand.Columns.Count, column);
			} finally {
				EndUpdate();
			}
		}
		protected override void AssignColumns(ColumnView cv, bool synchronize) {
			BandedGridView bv = cv as BandedGridView;
			if(bv == null) {
				base.AssignColumns(cv, synchronize);
				return;
			}
			if(synchronize) {
				Columns.Synchronize(bv.Columns);
				Bands.SynchronizeCore(bv.Bands);
			} else {
				Columns.Assign(bv.Columns);
				Bands.AssignCore(bv.Bands);
			}
		}
		public override void SynchronizeVisual(BaseView viewSource) {
			if(viewSource == null) return;
			BeginSynchronization();
			BeginUpdate();
			try {
				base.SynchronizeVisual(viewSource);
				BandedGridView bv = viewSource as BandedGridView;
				if(bv == null) return;
				SyncBGridProperties(bv);
			}
			finally {
				EndUpdate();
				EndSynchronization();
			}
		}
		void SyncBGridProperties(BandedGridView bv) {
			this.bandPanelRowHeight = bv.BandPanelRowHeight;
			this.minBandPanelRowCount = bv.MinBandPanelRowCount;
		}
		public override void Assign(BaseView v, bool copyEvents) {
			if(v == null) return;
			BeginUpdate();
			try {
				base.Assign(v, copyEvents);
				BandedGridView bv = v as BandedGridView;
				if(bv != null) {
					SyncBGridProperties(bv);
					if(copyEvents) {
						Events.AddHandler(customDrawBandHeader, bv.Events[customDrawBandHeader]);
						Events.AddHandler(bandWidthChanged, bv.Events[bandWidthChanged]);
					}
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override void EndSizing() {
			Painter.HideSizerLine();
			if(Painter.StartSizerPos != Painter.CurrentSizerPos && Painter.ReSizingObject != null) {
				if(State == BandedGridState.BandSizing) {
					EndBandSizing();
				}
			}
			base.EndSizing();
		}
		protected virtual void EndBandSizing() {
			GridBand band = Painter.ReSizingObject as GridBand;
			GridBandInfoArgs info = ViewInfo.BandsInfo.FindBand(band);
			if(info != null) {
				int newSize = 0;
				if(IsRightToLeft) {
					newSize = info.Bounds.Right - Painter.CurrentSizerPos;
					if(newSize < 0) newSize = Painter.CurrentSizerPos - info.Bounds.Right;
				}
				else {
					newSize = Painter.CurrentSizerPos - info.Bounds.Left;
					if(newSize < 0) return;
				}
				newSize = Math.Max(newSize, GetBandMinWidth(band));
				band.Resize(newSize);
			}
		}
		protected internal override void OnColumnSizeChanged(GridColumn column) {
			if(IsDeserializing) return;
			BandedGridColumn col = column as BandedGridColumn;
			if(col.VisibleIndex > -1 && col.OwnerBand != null) {
				ViewInfo.RecalcOnColumnWidthChanged(col);
				OnPropertiesChanged();
			}
		}
		protected internal virtual GridBandRowCollection GetBandRows(GridBand band) { return GetBandRows(band, false); }
		protected internal virtual GridBandRowCollection GetBandRows(GridBand band, bool includeNonVisible) { 
			GridBandRowCollection rows = new GridBandRowCollection();
			if(band.Columns.Count == 0) return rows;
			GridBandRow row = new GridBandRow();
			row.Columns.AddRangeCore(band.Columns);
			rows.Add(row);
			return rows;
		}
		protected internal virtual void UpdateBandColumnsRowValues(GridBand band) {
		}
		protected internal virtual void OnBandChanged(GridBand band) { 
			OnPropertiesChanged();
		}
		protected internal virtual void OnBandWidthChanged(GridBand band) { 
			RaiseBandWidthChanged(new BandEventArgs(band));
		}
		protected internal virtual void OnBandSizeChanged(GridBand band) { 
			if(IsDeserializing || IsLockUpdate) return;
			ViewInfo.RecalcOnBandWidthChanged(band);
			OnPropertiesChanged();
		}
		protected internal virtual int GetBandMinWidth(GridBand band) {
			int r = 0;
			if(band.VisibleIndex == 0) {
				if(IsShowDetailButtons) 
					r += ViewInfo.DetailButtonSize.Width;
				r += SortInfo.GroupCount * (ViewInfo.LevelIndent + 2);
			}
			return Math.Max(band.MinWidth, r);
		}
		public virtual bool CanDragBand(GridBand band) {
			if(GridControl.IsDesignMode || ForcedDesignMode) return true;
			if(!OptionsCustomization.AllowBandMoving || !band.OptionsBand.AllowMove) return false;
			return true;
		}
		public virtual bool CanResizeBand(GridBand band) {
			return OptionsCustomization.AllowBandResizing && band.OptionsBand.AllowSize;
		}
		public new BandedGridHitInfo CalcHitInfo(Point pt) { return base.CalcHitInfo(pt) as BandedGridHitInfo; }
		public new BandedGridHitInfo CalcHitInfo(int x, int y) { return CalcHitInfo(new Point(x, y)); }
		public virtual void InvalidateBandHeader(GridBand band) {
			if(!ViewInfo.IsReady) return;
			ViewInfo.PaintAnimatedItems = false;
			InvalidateRect(ViewInfo.ViewRects.BandPanelActual);
		}
		public override void InvalidateHitObject(DevExpress.XtraGrid.Views.Base.ViewInfo.BaseHitInfo hitInfo) {
			BandedGridHitInfo hit = hitInfo as BandedGridHitInfo;
			ViewInfo.PaintAnimatedItems = false;
			if(hit.InBandPanel) {
				InvalidateBandHeader(null);
				return;
			}
			base.InvalidateHitObject(hitInfo);
		}
		protected void CheckRefreshBandList() {
			if(this.requireRefreshBands) {
				this.requireRefreshBands = false;
				RefreshBandList();
			}
		}
		protected virtual void RefreshBandList() {
			if(IsLoading) return;
			GridBand[] bands = new GridBand[Bands.Count];
			if(bands.Length != 0) {
				(Bands as IList).CopyTo(bands, 0);
				Array.Sort(bands, new BandVisibleIndexSorter());
				if(!Bands.EqualsCore(bands)) {
					Bands.InternalCopy(bands);
				}
			}
			if(ViewInfo != null) {
				ViewInfo.UpdateFixedColumnInfo();
			}
		}
		protected internal virtual int CalcBandColIndex(BandedGridColumn column) { 
			if(IsLoading) return -1;
			return column.InternalColIndexCore;
		}
		protected internal override bool CanShowColumnInCustomizationForm(GridColumn col) {
			bool res = base.CanShowColumnInCustomizationForm(col);
			BandedGridColumn bcol = col as BandedGridColumn;
			if(res || (col.OptionsColumn.ShowInCustomizationForm && bcol.OwnerBand == null)) return true;
			return false;
		}
		protected internal virtual void OnColumnRowIndexChanged(BandedGridColumn column) {
		}
		protected virtual void OnColumnDeleted(GridBand band, BandedGridColumn column) {
			while(band.Columns.Contains(column)) {
				band.Columns.Remove(column);
			}
			foreach(GridBand bnd in band.Children) {
				OnColumnDeleted(bnd, column);
			}
		}
		protected override void OnColumnDeleted(GridColumn column) {
			BeginUpdate();
			try {
				foreach(GridBand band in Bands) {
					OnColumnDeleted(band, column as BandedGridColumn);
				}
				base.OnColumnDeleted(column);
			}
			finally {
				EndUpdate();
			}
		}
		protected override void OnEndUpdate() {
			CheckRefreshBandList();
			base.OnEndUpdate();
		}
		protected override void RefreshVisibleColumnsIndexes() {
		}
		protected internal override void RefreshVisibleColumnsList() {
			if(IsLoading) return;
			ArrayList tempList = new ArrayList();
			VisibleColumnsCore.ClearCore();
			Hashtable tempHash = new Hashtable();
			foreach(BandedGridColumn column in Columns) {
				if(column.OwnerBand == null || !column.OwnerBand.ReallyVisible) continue;
				tempHash[column.OwnerBand] = column;
			}
			if(tempHash.Count == 0) return;
			GridBand[] bands = new GridBand[tempHash.Count];
			tempHash.Keys.CopyTo(bands, 0);
			Array.Sort(bands, new RootBandIndexSorter());
			CreateVisibleListOnBands(bands);
		}
		protected virtual void CreateVisibleListOnBands(GridBand[] bands) {
			for(int n = 0; n < bands.Length; n++) {
				GridBand band = bands[n];
				foreach(BandedGridColumn column in band.Columns) {
					if(column.Visible) VisibleColumnsCore.Show(column, VisibleColumnsCore.Count);
					else VisibleColumnsCore.Hide(column);
				}
				if(GridControl == null) return;
				GridBandRowCollection rows = GetBandRows(band, true);
				ApplyBandRowValues(band, rows);
			}
		}
		protected internal virtual void ApplyBandRowValues(GridBand band, GridBandRowCollection rows) {
			if(rows == null || band == null) return;
			for(int n = 0; n < rows.Count; n++) {
				GridBandRow row = rows[n];
				for(int c = 0; c < row.Columns.Count; c++) {
					BandedGridColumn column = row.Columns[c];
					column.SetRowIndexCore(n);
					column.SeColIndexCore(c);
				}
			}
		}
		protected class RootBandIndexSorter : IComparer {
			int IComparer.Compare(object a, object b) {
				GridBand b1 = (GridBand)a, b2 = (GridBand)b;
				if(b1 == b2) return 0;
				int res = Comparer.Default.Compare(b1.RootBand.Index, b2.RootBand.Index);
				if(res != 0) return res;
				if(b1.ParentBand == b2.ParentBand) return Comparer.Default.Compare(b1.Index, b2.Index);
				res = CompareRoot(b1, b2);
				if(res != 0) return res;
					return Comparer.Default.Compare(b1.Index, b2.Index);
			}
			int CompareRoot(GridBand b1, GridBand b2) {
				while(b1.ParentBand != null) {
					GridBand b2_band = b2;
					while(b2_band != null) {
						if(b2_band.ParentBand == b1.ParentBand) {
							return Comparer.Default.Compare(b1.Index, b2_band.Index);
						}
						b2_band = b2_band.ParentBand;
					}
					b1 = b1.ParentBand;
				}
				return 0;
			}
		}
		protected override void DoLeftCoordChanged() {
			base.DoLeftCoordChanged();
			if(fLockUpdate != 0) return;
			ViewInfo.PaintAnimatedItems = false;
			InvalidateBandHeader(null);
		}
		protected override void InitializeVisualParameters() {
			base.InitializeVisualParameters();
			RefreshBandList();
		}
		protected override void OnColumnPopulate(GridColumn column, int visibleIndex) {
			if(Bands.Count == 0) return;
			BandedGridColumn col = column as BandedGridColumn;
			col.SetVisibleCore(true);
			Bands[0].Columns.Add(col);
		}
		protected internal virtual void SetBandFixedStyle(GridBand band, FixedStyle newValue) {
			if(IsLoading) return;
			RefreshBandList();
			OnPropertiesChanged();
		}
		protected class BandVisibleIndexSorter : IComparer {
			public BandVisibleIndexSorter() {
			}
			int IComparer.Compare(object a, object b) {
				GridBand b1 = a as GridBand, b2 = b as GridBand;
				if(b1 == b2) return 0;
				if(b1 == null) return -1;
				if(b2 == null) return 1;
				if(b1.Fixed != b2.Fixed) {
					if(b1.Fixed == FixedStyle.Left) return -1;
					if(b2.Fixed == FixedStyle.Left) return 1;
					if(b1.Fixed == FixedStyle.Right) return 1;
					if(b2.Fixed == FixedStyle.Right) return -1;
				}
				return Comparer.Default.Compare(b1.Index, b2.Index);
			}
		}
		public override DevExpress.XtraGrid.Export.BaseExportLink CreateExportLink(DevExpress.XtraExport.IExportProvider provider) {
#pragma warning disable 618
			return new DevExpress.XtraGrid.Export.BandedViewExportLink(this, provider);
#pragma warning restore 618
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridViewCustomDrawBandHeader"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event BandHeaderCustomDrawEventHandler CustomDrawBandHeader {
			add { this.Events.AddHandler(customDrawBandHeader, value); }
			remove { this.Events.RemoveHandler(customDrawBandHeader, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridViewBandWidthChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event BandEventHandler BandWidthChanged {
			add { this.Events.AddHandler(bandWidthChanged, value); }
			remove { this.Events.RemoveHandler(bandWidthChanged, value); }
		}
		protected internal virtual void RaiseCustomDrawBandHeader(EventArgs e) { 
			BandHeaderCustomDrawEventHandler handler = (BandHeaderCustomDrawEventHandler)this.Events[customDrawBandHeader];
			if(handler != null) handler(this, e as BandHeaderCustomDrawEventArgs);
		}
		protected virtual void RaiseBandWidthChanged(BandEventArgs e) {
			BandEventHandler handler = (BandEventHandler)this.Events[bandWidthChanged];
			if(handler != null) handler(this, e);
		}
		protected override bool SetDataAwareClipboardData() {
			return false;
		}
		protected override GridOptionsClipboard CreateOptionsClipboard() {
			return new GridOptionsClipboard(false);
		}
	}
	public enum BandedGridState {Normal, ColumnSizing, Editing, 
		ColumnDragging, ColumnDown, ColumnFilterDown, RowDetailSizing,
		FilterCloseButtonPressed, ColumnButtonDown , RowSizing, IncrementalSearch, Selection,
		FilterPanelActiveButtonPressed, FilterPanelTextPressed, FilterPanelMRUButtonPressed, FilterPanelCustomizeButtonPressed, CellSelection, Scrolling,
		Unknown,
		BandSizing = 100, BandDragging = 101, BandDown = 102
	};
}
