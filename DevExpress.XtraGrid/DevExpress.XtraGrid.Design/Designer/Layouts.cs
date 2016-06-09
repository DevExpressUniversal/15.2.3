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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors.Frames;
using DevExpress.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.BandedGrid;
using System.Reflection;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class Layouts : LayoutsBase{
		Timer formTimer = new Timer();
		public Layouts() : base(6) {
			this.Resize += new EventHandler(Layouts_Resize);
			formTimer.Interval = 100;
			formTimer.Tick += new EventHandler(formTimer_Tick);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing && baseView is IGridDesignTime)
				(baseView as IGridDesignTime).ForceDesignMode = false;
		}
		protected override string DescriptionText { get { return DevExpress.XtraGrid.Design.Properties.Resources.LayoutsDescription; } }
		protected BaseView EditingView { get { return EditingObject as BaseView; } }
		DevExpress.XtraGrid.GridControl grid = null;
		BaseView baseView;
		protected override Control CreatePreviewControl() {
			grid = DevExpress.XtraGrid.Design.GridAssign.CreateGridControlAssign(EditingView.GridControl, EditingView);
			grid.Tag = "Design";
			baseView = grid.MainView;
			if(dataSet != null) 
				grid.DataSource = dataSet.Tables[tableName];
			else
				grid.DataSource = EditingView.DataSource;
			InitView();
			grid.MainView.Layout += new EventHandler(View_Layout);
			if(View != null)
				View.LeftCoordChanged += new EventHandler(View_LeftCoordChanged);
			if(baseView is IGridDesignTime)
				(baseView as IGridDesignTime).ForceDesignMode = true;
			return grid;
		}
		protected override void ShowColumnsCustomization() {
			View.ColumnsCustomization();
		}
		protected override void HideColumnsCustomization() {
			View.DestroyCustomization();
		}
		protected override void SetColumnSelectorText(bool showing) {
			BandedGridView bView = View as BandedGridView;
			if(bView != null) 
				SetColumnSelectorCaption(showing ? DevExpress.XtraGrid.Design.Properties.Resources.HideColumnsBandsSelectorCaption : DevExpress.XtraGrid.Design.Properties.Resources.ShowColumnsBandsSelectorCaption);
			else
				base.SetColumnSelectorText(showing);
		}
		protected override void ApplyLayouts() {
			System.IO.MemoryStream str = new System.IO.MemoryStream();
			baseView.SaveLayoutToStream(str, OptionsLayoutBase.FullLayout);
			str.Seek(0, System.IO.SeekOrigin.Begin);
			EditingView.RestoreLayoutFromStream(str, OptionsLayoutBase.FullLayout);
			str.Close();
		}
		protected override void RestoreLayoutFromXml(string fileName) {
			baseView.RestoreLayoutFromXml(fileName, OptionsLayoutBase.FullLayout);
		}
		protected override void SaveLayoutToXml(string fileName) {
			try {
			baseView.SaveLayoutToXml(fileName, OptionsLayoutBase.FullLayout);
		}
			catch(Exception ex) {
				DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
			}
		}
		protected override void SetControlDataSource(DataView dataView) {
			grid.DataSource = dataView;
			if(((ColumnView)EditingView).Columns.Count == 0)
				baseView.PopulateColumns();
			else SetLayoutChanged(false);
		}
		ComponentCollection GetCurrentComponents() {
			if(EditingView.GridControl.Container != null) return EditingView.GridControl.Container.Components;
			if(EditingView.Container != null) return EditingView.Container.Components;
			return null;
		}
		protected override DBAdapter CreateDBAdapter() {
			ArrayList adapters = new ArrayList();
			ComponentCollection components = GetCurrentComponents();
			if(components == null) return null;
			foreach(object comp in components)
				adapters.Add(comp);
			return new DBAdapter(adapters, EditingView.GridControl.DataSource, EditingView.GridControl.DataMember);
		}
		protected override DataTable CreateDataTableAdapter() {
			if(EditingView.GridControl.DataSource == null)  return null;
			try {
				CurrencyManager manager = EditingView.GridControl.BindingContext[EditingView.GridControl.DataSource, EditingView.GridControl.DataMember] as CurrencyManager;
				if(manager == null) return null;
				DataView dv = manager.List as DataView;
				if(dv != null) return dv.Table;
			}
			catch {
			}
			return null;
		}
		protected override void OnFillGrid() {
			if(baseView is GridView)
				((GridView)baseView).ExpandAllGroups();
		}
		private GridView View {
			get {
				if(grid.MainView is GridView) return grid.MainView as GridView;
				return null;
			}
		}
		void InitView() {
			ChangeColumnSelectorButtonVisibility(View != null);
			if(View != null) {
				View.ShowCustomizationForm += new System.EventHandler(this.view_ShowCustomizationForm);
				View.HideCustomizationForm += new System.EventHandler(this.view_HideCustomizationForm);
				View.GridMenuItemClick += new GridMenuItemClickEventHandler(View_GridMenuItemClick);
			}
		}
		void View_GridMenuItemClick(object sender, GridMenuItemClickEventArgs e) {
			if(e.MenuType == GridMenuType.Summary)
				SetLayoutChanged(true);
		}
		void view_ShowCustomizationForm(object sender, System.EventArgs e) {
			OnShowCustomizationForm();
		}
		void view_HideCustomizationForm(object sender, System.EventArgs e) {
			OnHideCustomizationForm();
		}
		void View_Layout(object sender, EventArgs e) {
			SetLayoutChanged(true);
			formTimer.Start();
		}
		protected override bool AllowShowColumnSortIndex {
			get {
				return View != null;
			}
		}
		protected override void ShowColumnsSortIndices() {
			if(AdornerManager == null) {
				AdornerManager = new AdornerUIManager();
				AdornerManager.Owner = this;
			}
			AdornerManager.BeginUpdate();
			for(int i = AdornerManager.Elements.Count - 1; i >= 0; i--)
				AdornerManager.Elements[i].Dispose();
			AdornerManager.Elements.Clear();
			if(ShowColumnsSortIndex) {
				PropertyInfo pi = View.GetType().GetProperty("ViewInfo", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic, null, typeof(GridViewInfo), new Type[0], null);
				GridViewInfo info = pi.GetValue(View, null) as GridViewInfo;
				foreach(GridColumn col in View.Columns) {
					if(col.SortIndex > -1 && info.ColumnsInfo[col] != null) {
						Badge element = new Badge();
						AdornerManager.Elements.Add(element);
						element.TargetElement = grid;
						element.Appearance.BackColor = Color.FromArgb(51, 153, 255);
						element.Properties.Text = col.SortIndex.ToString();
						Rectangle r = info.ColumnsInfo[col].Bounds;
						element.Properties.Offset = new Point(r.X + r.Width - Math.Min(30, col.Width), r.Y);
					}
				}
			}
			AdornerManager.EndUpdate();
		}
		void Layouts_Resize(object sender, EventArgs e) {
			formTimer.Start();
		}
		void View_LeftCoordChanged(object sender, EventArgs e) {
			formTimer.Start();
		}
		void formTimer_Tick(object sender, EventArgs e) {
			formTimer.Stop();
			if(ShowColumnsSortIndex) ShowColumnsSortIndices();
		}
	}
}
