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
using System.Linq;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using System.ComponentModel.Design;
using DevExpress.Utils;
using DevExpress.Data.Helpers;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraGrid {
	[Designer("DevExpress.XtraGrid.Design.GridSplitContainerDesigner, " + AssemblyInfo.SRAssemblyGridDesign, typeof(IDesigner)),
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabData),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "GridSplitContainer"),
	Description("Allows a Grid Control to be split horizontally or vertically.")]
	public class GridSplitContainer : SplitContainerControl {
		private static readonly object splitViewCreated = new object();
		private static readonly object splitViewShown = new object();
		private static readonly object splitViewHidden = new object();
		GridControl grid, splitChildGrid;
		bool isSplitViewVisible = false;
		DefaultBoolean synchronizeScrolling = DefaultBoolean.Default;
		DefaultBoolean synchronizeFocusedRow = DefaultBoolean.Default;
		DefaultBoolean synchronizeExpandCollapse = DefaultBoolean.Default;
		DefaultBoolean synchronizeViews = DefaultBoolean.Default;
		bool disposeChildDataSource = false;
		DevExpress.XtraEditors.Repository.InternalPersistentRepository editorsRepository;
		public GridSplitContainer() {
			base.BorderStyle = BorderStyles.NoBorder;
			base.Horizontal = false;
			base.PanelVisibility = SplitPanelVisibility.Panel1;
			base.FixedPanel = SplitFixedPanel.None;
			base.SplitterPosition = 0;
			this.editorsRepository = new DevExpress.XtraEditors.Repository.InternalPersistentRepository();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				CheckDestroyDataSource();
				DestroyEditorsRepository();
			}
			base.Dispose(disposing);
		}
		void DestroyEditorsRepository() {
			if(editorsRepository.Items.Count > 0) {
				editorsRepository.Items.Clear();
			}
			editorsRepository.Dispose();
		}
		protected void CheckDestroyDataSource() {
			if(!disposeChildDataSource) return;
			this.disposeChildDataSource = false;
			if(SplitChildGrid == null || SplitChildGrid.DataSource == null) return;
			IDisposable ds = SplitChildGrid.DataSource as IDisposable;
			SplitChildGrid.DataSource = null;
			if(ds != null) ds.Dispose();
		}
		public void Initialize() {
			if(grid != null) return;
			Grid = CreateGridControl();
			this.grid.Dock = DockStyle.Fill;
			Panel1.Controls.Add(Grid);
			this.grid.ForceInitialize();
		}
		protected void EnsureSplitGrid() {
			Initialize();
			if(SplitChildGrid != null) return;
			this.splitChildGrid = CreateGridControl();
			this.splitChildGrid.Dock = DockStyle.Fill;
			Panel2.Controls.Add(SplitChildGrid);
			SplitChildGrid.ExternalRepository = this.editorsRepository;
		}
		protected void EnsureSplitMainView() {
			EnsureSplitGrid();
			if(View == null) return;
			Type type = View.GetType();
			if(SplitChildView == null || !SplitChildView.GetType().Equals(type)) {
				CreateSplitView(View);
			}
		}
		protected virtual void CreateSplitView(BaseView view) {
			SplitChildGrid.ViewCollection.Clear();
			BaseView child = view.CreateInstance();
			SplitChildGrid.ViewCollection.Add(child);
			SplitChildGrid.MainView = child;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseView View { get { return Grid == null ? null : Grid.MainView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseView SplitChildView { get { return SplitChildGrid == null ? null : SplitChildGrid.MainView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override SplitPanelVisibility PanelVisibility {
			get { return base.PanelVisibility; }
			set { }
		}
		[DXCategory(CategoryName.Split)]
		public event EventHandler SplitViewShown {
			add { Events.AddHandler(splitViewShown, value); }
			remove { Events.RemoveHandler(splitViewShown, value);}
		}
		[DXCategory(CategoryName.Split)]
		public event EventHandler SplitViewCreated {
			add { Events.AddHandler(splitViewCreated, value); }
			remove { Events.RemoveHandler(splitViewCreated, value); }
		}
		[DXCategory(CategoryName.Split)]
		public event EventHandler SplitViewHidden {
			add { Events.AddHandler(splitViewHidden, value); }
			remove { Events.RemoveHandler(splitViewHidden, value); }
		}
		[DefaultValue(false), DXCategory(CategoryName.Split)]
		public override bool Horizontal {
			get { return base.Horizontal; } 
			set {
				if(Horizontal == value) return;
				base.Horizontal = value;
				ReshowSplitView();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SplitCollapsePanel CollapsePanel {
			get {return base.CollapsePanel; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Collapsed {
			get { return base.Collapsed; }
			set { base.Collapsed = false; }
		}
		[ DXCategory(CategoryName.Layout), DefaultValue(0)]
		public override int SplitterPosition {
			get { return base.SplitterPosition; }
			set { base.SplitterPosition = value; }
		}
		void ReshowSplitView() {
			if(!IsSplitViewVisible) return;
			HideSplitView();
			ShowSplitView();
		}
		[DXCategory(CategoryName.Split), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean SynchronizeViews {
			get { return synchronizeViews; }
			set { synchronizeViews = value; }
		}
		[DXCategory(CategoryName.Split), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean SynchronizeFocusedRow {
			get { return synchronizeFocusedRow; }
			set { synchronizeFocusedRow = value; }
		}
		[DXCategory(CategoryName.Split), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean SynchronizeExpandCollapse {
			get { return synchronizeExpandCollapse; }
			set { synchronizeExpandCollapse = value; }
		}
		[DXCategory(CategoryName.Split), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean SynchronizeScrolling {
			get { return synchronizeScrolling; }
			set { synchronizeScrolling = value; }
		}
		internal bool GetSynchronizeFocusedRow() {
			return SynchronizeFocusedRow == DefaultBoolean.True || (SynchronizeFocusedRow == DefaultBoolean.Default && Horizontal) ;
		}
		internal bool GetSynchronizeViews() {
			return SynchronizeViews != DefaultBoolean.False;
		}
		internal bool GetSynchronizeExpandCollapse() {
			return SynchronizeExpandCollapse != DefaultBoolean.False;
		}
		internal bool GetSynchronizeScrolling() {
			return SynchronizeScrolling != DefaultBoolean.False;
		}
		[DefaultValue(SplitFixedPanel.None)]
		public override SplitFixedPanel FixedPanel {
			get { return base.FixedPanel; }
			set { base.FixedPanel = value; }
		}
		[DXCategory(CategoryName.Split), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public GridControl Grid { 
			get {  return grid; }
			set { 
				if(DesignMode) {
					if(value == null) return;
					if(Grid != null) return;
				}
				if(Grid == value) return;
				if(grid != null) {
					grid.Disposed -= OnGridDisposed;
					grid.DataSourceChanged -= OnGridDataSourceChanged;
				}
				grid = value;
				if(grid != null) {
					grid.Disposed += OnGridDisposed;
					grid.DataSourceChanged += OnGridDataSourceChanged;
				}
			}
		}
		void OnGridDataSourceChanged(object sender, EventArgs e) {
			if(IsSplitViewVisible) {
				SplitChildGrid.DataMember = Grid.DataMember; 
				AssignChildGridDataSource();
			}
		} 
		void OnGridDisposed(object sender, EventArgs e) {
			if(DesignMode && Site != null) {
				IDesignerHost host = (IDesignerHost)Site.GetService(typeof(IDesignerHost));
				if(host != null) host.DestroyComponent(this);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridControl SplitChildGrid { get { return splitChildGrid; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new BorderStyles BorderStyle {
			get { return base.BorderStyle; }
			set { }
		}
		protected override void OnSplitterPositionChanged() {
			base.OnSplitterPositionChanged();
			if(SplitterPosition < 2 && IsSplitViewVisible) {
				HideSplitView();
			}
		}
		protected virtual GridControl CreateGridControl() {
			return new GridControl();
		}
		protected override Size DefaultSize { get { return new Size(400, 400); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsSplitViewVisible { get { return isSplitViewVisible; }}
		public void ShowSplitView() {
			if(IsSplitViewVisible) return;
			EnsureSplitMainView();
			SyncrhonizeViews();
			if(Horizontal) {
				if(ScaledSplitterPosition > Width || SplitterPosition < 10) ScaledSplitterPosition = Width / 2;
			} else {
				if(ScaledSplitterPosition > Height || SplitterPosition < 10) ScaledSplitterPosition = Height / 2;
			}
			base.PanelVisibility = SplitPanelVisibility.Both;
			this.isSplitViewVisible = true;
			RaiseSplitViewShown();
		}
		public void HideSplitView() {
			if(!IsSplitViewVisible) return;
			base.PanelVisibility = SplitPanelVisibility.Panel1;
			this.isSplitViewVisible = false;
			if(View != null) View.RestoreMasterSplitElements();
			if(SplitChildGrid != null) {
				CheckDestroyDataSource();
				SplitChildGrid.DataSource = null;
			}
			RaiseSplitViewHidden();
		}
		protected virtual bool CloneDXDataSource { get { return true; } }
		protected virtual bool SyncrhonizeEvents { get { return true; } }
		protected internal void SyncrhonizeViews() {
			if(View == null || SplitChildView == null) return;
			this.editorsRepository.Items.Clear();
			this.editorsRepository.Items.AddRange(Grid.EditorHelper.InternalRepository.Items.OfType<RepositoryItem>().ToArray());
			((ColumnView)View).OptionsView.AllowAssignSplitOptions = true;
			((ColumnView)SplitChildView).OptionsView.AllowAssignSplitOptions = true;
			SplitChildView.AllowSynchronization = View.AllowSynchronization = false;
			((ColumnView)SplitChildView).DisableCurrencyManager = true;
			SplitChildView.BeginDataUpdate();
			try {
				SplitChildView.ResetEvents();
				SplitChildView.Assign(View, SyncrhonizeEvents);
				SplitChildGrid.DataMember = Grid.DataMember;
				AssignChildGridDataSource();
				SplitChildView.SetupChildSplitElements(View);
				View.SetupMasterSplitElements();
			}
			finally {
				SplitChildView.EndDataUpdate();
			}
			RaiseSplitViewCreated();
			SplitChildView.AllowSynchronization = View.AllowSynchronization = GetSynchronizeViews();
			((ColumnView)View).OptionsView.AllowAssignSplitOptions = false;
			((ColumnView)SplitChildView).OptionsView.AllowAssignSplitOptions = false;
		}
		void AssignChildGridDataSource() {
			if(!CloneDXDataSource) {
				SplitChildGrid.DataSource = Grid.DataSource;
				return;
			}
			object data = Grid.DataSource;
			if(object.ReferenceEquals(SplitChildGrid.DataSource, data)) return;
			CheckDestroyDataSource();
			object clonedData = GridControl.GetClonedDataSource(data);
			if(clonedData != null) {
				SplitChildGrid.DataSource = clonedData;
				this.disposeChildDataSource = true;
			}
			else {
				SplitChildGrid.DataSource = data;
			}
		}
		protected virtual void RaiseSplitViewCreated() {
			EventHandler handler = (EventHandler)Events[splitViewCreated];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseSplitViewShown() {
			EventHandler handler = (EventHandler)Events[splitViewShown];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseSplitViewHidden() {
			EventHandler handler = (EventHandler)Events[splitViewHidden];
			if(handler != null) handler(this, EventArgs.Empty);
		}
	}
	public class SplitHelperGridView {
		public static void ShowGroupPanel(GridView view, bool value) {
			SetSplitOptionValue(view, view.OptionsView, view.SplitOtherView == null ? null : ((GridView)view.SplitOtherView).OptionsView, "ShowGroupPanel", value, true);
		}
		public static void ShowAutoFilterRow(GridView view, bool value) {
			SetSplitOptionValue(view, view.OptionsView, view.SplitOtherView == null ? null : ((GridView)view.SplitOtherView).OptionsView, "ShowAutoFilterRow", value, true);
		}
		public static void ShowFooter(GridView view, bool value) {
			SetSplitOptionValue(view, view.OptionsView, view.SplitOtherView == null ? null : ((GridView)view.SplitOtherView).OptionsView, "ShowFooter", value, false);
		}
		static void SetSplitOptionValue(BaseView view, object viewOptions, object childViewOptions, string name, object value, bool reverse) {
			if(childViewOptions != null && view.IsSplitView) {
				if(view.IsVerticalSplit) {
					SetOption(reverse ? viewOptions : childViewOptions, name, value);
				}
				else {
					SetOption(viewOptions, name, value);
					SetOption(childViewOptions, name, value);
				}
			}
			else {
				SetOption(viewOptions, name, value);
			}
		}
		static void SetOption(object options, string name, object value) {
			OptionsHelper.SetOptionValue(options, name, value);
		}
	}
}
