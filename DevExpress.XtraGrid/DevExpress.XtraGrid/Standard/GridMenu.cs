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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.Data;
using DevExpress.XtraGrid.Blending;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using System.Collections;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraExport;
using DevExpress.XtraGrid.Export;
namespace DevExpress.XtraGrid.Menu {
	[Obsolete("")]
	public class GridAppearanceMenu : LookAndFeelMenu {
		XtraGridBlending blending;
		MenuItem miBlending, miOptions, miMixedXP, miSelectionOptions, miExport;
		BaseView view;
		BaseView exportView;
		public override void Dispose() {
			base.Dispose();
			this.about = null;
			this.blending = null;
			this.miMixedXP = null;
			this.miBlending = null;
			this.miOptions = null;
			this.miSelectionOptions = null;
			this.miExport = null;
			this.menu = null;
		}
		public GridAppearanceMenu(Form form, XtraGridBlending blending, DefaultLookAndFeel lookAndFeel, string about) : this(form, blending, lookAndFeel, about, null, false) {}
		public GridAppearanceMenu(Form form, XtraGridBlending blending, DefaultLookAndFeel lookAndFeel, string about, BaseView view) : this(form, blending, lookAndFeel, about, view, false) {}
		public GridAppearanceMenu(Form form, XtraGridBlending blending, DefaultLookAndFeel lookAndFeel, string about, bool setMixed) : this(form, blending, lookAndFeel, about, null, setMixed) {}
		public GridAppearanceMenu(Form form, XtraGridBlending blending, DefaultLookAndFeel lookAndFeel, string about, bool setMixed, BaseView exportView) : this(form, blending, lookAndFeel, about, null, setMixed, exportView) {}
		public GridAppearanceMenu(Form form, XtraGridBlending blending, DefaultLookAndFeel lookAndFeel, string about, BaseView view, bool setMixed) : this(form, blending, lookAndFeel, about, view, setMixed, null) {}
		public GridAppearanceMenu(Form form, XtraGridBlending blending, DefaultLookAndFeel lookAndFeel, string about, BaseView view, bool setMixed, BaseView exportView) : base(form, lookAndFeel, about) {
			this.blending = blending;
			this.exportView = exportView;
			miBlending = new MenuItem("Alpha &Blending");
			miBlending.MenuItems.Add(new AppearanceItem("&Using", new EventHandler(miUsing_Click)));	
			miBlending.MenuItems.Add(new AppearanceItem("&Adjustment...", new EventHandler(miAdjustment_Click)));
			miOptions = new MenuItem("View &Options");
			miSelectionOptions = new MenuItem("&Selection Options");
			miMixedXP = new AppearanceItem("&Mixed XP", new EventHandler(miMixedXP_Click));
			if(DevExpress.Utils.WXPaint.Painter.ThemesEnabled) {
				if(setMixed) {
					SetMixedXP(setMixed);
				}
				miLookAndFeel.MenuItems.Add(new MenuItem("-"));
				miLookAndFeel.MenuItems.Add(miMixedXP);
			}
			miExport = new MenuItem("&Export To...");
			miExport.MenuItems.Add(new AppearanceItem("Export To HTML Document", new EventHandler(miExportHTML_Click)));
			miExport.MenuItems.Add(new AppearanceItem("Export To XML Document", new EventHandler(miExportXML_Click)));
			miExport.MenuItems.Add(new AppearanceItem("Export To Microsoft Excel", new EventHandler(miExportXLS_Click)));
			miExport.MenuItems.Add(new AppearanceItem("Export To Text File", new EventHandler(miExportTXT_Click)));
			AddOptionsMenu(view);
			AddOptionsMenu(miSelectionOptions, SelectionOptions,new EventHandler(miSelectionViewOptions_Click));
			AddItems();
		}
		void HideXP(MenuItem item) {
			foreach(MenuItem i in item.MenuItems)
				i.Visible = (i.Text.IndexOf("Native") < 0 && i.Text.IndexOf("Office") < 0);
		}
		protected override void AddItems() {
			if(menu == null || miBlending == null) return;
			menu.MenuItems.Add(miBlending);
			menu.MenuItems.Add(miLookAndFeel);
			menu.MenuItems.Add(miOptions);
			menu.MenuItems.Add(miSelectionOptions);
			menu.MenuItems.Add(miExport);
			miExport.Visible = exportView != null; 
			if(about != "" && about != null) menu.MenuItems.Add(new AppearanceItem("&About", new EventHandler(miAbout_Click)));	
			form.Menu = menu;
			InitBlendingMenu(); 
			InitLookAndFeelMenu();
		}
		protected MenuItem ExportMenuItem { get { return miExport; } }
		public BaseView ExportView {
			get { return exportView; }
			set {
				exportView = value;
				miExport.Visible = exportView != null;
			}
		}
		private void AddOptionsMenu(MenuItem miItem, object options, EventHandler handler) {
			miItem.MenuItems.Clear();
			if(options != null) {
				ArrayList arr = DevExpress.Utils.SetOptions.GetOptionNames(options);
				for(int i = 0; i < arr.Count; i++)
					miItem.MenuItems.Add(new AppearanceItem(arr[i].ToString(), handler));
			}
			miItem.Visible = options != null; 
			InitOptionsMenu(miItem, options);
		}
		private void AddOptionsMenu(BaseView view) {
			this.view = view;
			AddOptionsMenu(miOptions, ViewOptions, new EventHandler(miViewOptions_Click));			
		}
		public void RefreshOptionsMenu(BaseView view) {
			this.view = view;
			AddOptionsMenu(view);
			AddOptionsMenu(miSelectionOptions, SelectionOptions,new EventHandler(miSelectionViewOptions_Click));
		}
		private object ViewOptions {
			get {
				if(view is GridView) return ((GridView)view).OptionsView;
				if(view is CardView) return ((CardView)view).OptionsView;
				return null;
			}
		}
		private object SelectionOptions {
			get {
				if(view is GridView) return ((GridView)view).OptionsSelection;
				return null;
			}
		}
		public void InitOptionsMenu(MenuItem miItem, object options) {
			if(view == null) return;
			for(int i = 0; i < miItem.MenuItems.Count; i++)
				miItem.MenuItems[i].Checked = DevExpress.Utils.SetOptions.OptionValueByString(miItem.MenuItems[i].Text, options);
		}
		public void InitBlendingMenu() {
			InitBlendingMenu(blending);
		}
		public void InitBlendingMenu(XtraGridBlending blending) {
			this.blending = blending;
			miBlending.Visible = blending != null;
			if(blending != null) 
				miBlending.MenuItems[0].Checked = miBlending.MenuItems[1].Enabled = blending.Enabled;
		}
		private void miUsing_Click(object sender, System.EventArgs e) {
			MenuItem item = sender as MenuItem;
			if(blending != null) {
				item.Checked = !item.Checked;
				blending.Enabled = item.Checked;
				((MenuItem)item.Parent).MenuItems[1].Enabled = item.Checked;
			}
		}
		private void miAdjustment_Click(object sender, System.EventArgs e) {
			MenuItem item = sender as MenuItem;
			if(blending != null) {
				blending.ShowDialog();
			}
		}
		private void miViewOptions_Click(object sender, System.EventArgs e) {
			MenuItem item = sender as MenuItem;
			if(view != null) {
				DevExpress.Utils.SetOptions.SetOptionValueByString(item.Text, ViewOptions, !item.Checked);
				InitOptionsMenu(miOptions, ViewOptions);
			}
		}
		private void miSelectionViewOptions_Click(object sender, System.EventArgs e) {
			MenuItem item = sender as MenuItem;
			if(view != null) {
				DevExpress.Utils.SetOptions.SetOptionValueByString(item.Text, SelectionOptions, !item.Checked);
				InitOptionsMenu(miSelectionOptions, SelectionOptions);
			}
		}
		private void SetViewStyle(GridControl grid, string styleName) {
			grid.SwitchPaintStyle(styleName);
		}
		public void FindGridControls(Control control, bool setMixed) {
			foreach(Control cntrl in control.Controls) {
				if(cntrl is GridControl && ((GridControl)cntrl).LookAndFeel.UseDefaultLookAndFeel) 
					SetViewStyle((GridControl)cntrl, (setMixed ? "MixedXP" : "Default"));
				else FindGridControls(cntrl, setMixed);
			}
		}
		protected override bool Mixed { get { return miMixedXP.Checked; }}
		protected override void InitLookAndFeelEnabled() {
			base.InitLookAndFeelEnabled();
			miMixedXP.Enabled = DevExpress.Utils.WXPaint.Painter.ThemesEnabled;
		}
		public virtual void SetMixedXP(bool setMixed) {
			if(DevExpress.Utils.WXPaint.Painter.ThemesEnabled) {
				if(setMixed && LookAndFeel != null) LookAndFeel.LookAndFeel.UseWindowsXPTheme = true;
				miMixedXP.Checked = setMixed;
				FindGridControls(form, setMixed);
				if(setMixed) InitLookAndFeelMenu();
			}
		}
		private void miMixedXP_Click(object sender, System.EventArgs e) {
			SetMixedXP(true);
		}
		protected override void OnSwitchStyle_Click(object sender, System.EventArgs e) {
			SetMixedXP(false);
			base.OnSwitchStyle_Click(sender, e);
		}
		void miExportHTML_Click(object sender, System.EventArgs e) {
			string fileName = ShowSaveFileDialog("HTML Document", "HTML Documents|*.html");
			if(fileName != "") {
				ExportTo(new ExportHtmlProvider(fileName));
				OpenFile(fileName);
			}
		}
		void miExportXML_Click(object sender, System.EventArgs e) {
			string fileName = ShowSaveFileDialog("XML Document", "XML Documents|*.xml");
			if(fileName != "") { 
				ExportTo(new ExportXmlProvider(fileName));
				OpenFile(fileName);
			}
		}
		void miExportXLS_Click(object sender, System.EventArgs e) {
			string fileName = ShowSaveFileDialog("Microsoft Excel Document", "Microsoft Excel|*.xls");
			if(fileName != "") {
				ExportTo(new ExportXlsProvider(fileName));
				OpenFile(fileName);
			}
		}
		void miExportTXT_Click(object sender, System.EventArgs e) {
			string fileName = ShowSaveFileDialog("Text Document", "Text Files|*.txt");
			if(fileName != "") {
				ExportTo(new ExportTxtProvider(fileName));
				OpenFile(fileName);
			}
		}
		private void OpenFile(string fileName) {
			if(XtraMessageBox.Show("Do you want to open this file?", "Export To...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
				try {
					System.Diagnostics.Process process = new System.Diagnostics.Process();
					process.StartInfo.FileName = fileName;
					process.StartInfo.Verb = "Open";
					process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
					process.Start();
				} catch {
					XtraMessageBox.Show("Cannot find an application on your system suitable for openning the file with exported data.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); 
				}
			}
		}
		private void ExportTo(IExportProvider provider) {
			StartExport();		
			Cursor currentCursor = view.GetCursor();
			view.SetCursor(Cursors.WaitCursor);
			BaseExportLink link = exportView.CreateExportLink(provider);
			link.Progress += new DevExpress.XtraGrid.Export.ProgressEventHandler(Export_Progress);
			link.ExportTo(true);
			provider.Dispose();
			link.Progress -= new DevExpress.XtraGrid.Export.ProgressEventHandler(Export_Progress);
			view.SetCursor(currentCursor);
			EndExport();
		}
		protected virtual void StartExport() {
			if(this.form != null) form.Update();
		}
		protected virtual void EndExport() {}
		protected virtual void Export_Progress(object sender, DevExpress.XtraGrid.Export.ProgressEventArgs e) {
		}
		private string ShowSaveFileDialog(string title, string filter) {
			SaveFileDialog dlg = new SaveFileDialog();
			string name = Application.ProductName;
			int n = name.LastIndexOf(".") + 1;
			if(n > 0) name = name.Substring(n, name.Length - n);
			dlg.Title = "Export To " + title;
			dlg.FileName = name;
			dlg.Filter = filter;
			if(dlg.ShowDialog() == DialogResult.OK) return dlg.FileName;
			return "";
		} 
	}
}
namespace DevExpress.XtraGrid.Views.Grid {
	public enum GridMenuType { User, Summary, Column, Group, Row };
	public delegate void GridMenuItemClickEventHandler(object sender, GridMenuItemClickEventArgs e);
	public class GridMenuItemClickEventArgs : EventArgs {
		DXMenuItem dxMenuItem;
		GridColumn column;
		GridSummaryItemCollection items;
		GridSummaryItem item;
		SummaryItemType type;
		GridMenuType mtype;
		MenuItem menuItem;
		string format;
		bool handled = false;
		public GridMenuItemClickEventArgs(GridColumn column, GridSummaryItemCollection items, GridSummaryItem item, SummaryItemType type, string format, GridMenuType mtype, DXMenuItem menuItem) {
			this.column = column;
			this.items = items;
			this.item = item;
			this.SummaryType = type;
			this.SummaryFormat = format;
			this.mtype = mtype;
			this.dxMenuItem = menuItem;
		}
		public GridMenuItemClickEventArgs(GridColumn column, GridSummaryItemCollection items, GridSummaryItem item, SummaryItemType type, string format, GridMenuType mtype, MenuItem menuItem) {
			this.column = column;
			this.items = items;
			this.item = item;
			this.SummaryType = type;
			this.SummaryFormat = format;
			this.mtype = mtype;
			this.menuItem = menuItem;
		}
		public GridSummaryItemCollection SummaryItems { get { return items; } }
		public GridSummaryItem SummaryItem { get { return item; } }
		public GridColumn Column { get { return column; } }
		public GridMenuType MenuType { get { return mtype; } }
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete(ObsoleteText.SRObsoleteMenuItem)]
		public MenuItem MenuItem { get { return menuItem; } }
		public DXMenuItem DXMenuItem { get { return dxMenuItem; } }
		public SummaryItemType SummaryType { 
			get { return type; }  
			set { type = value; }
		}
		public string SummaryFormat { 
			get { return format; }  
			set { format = value; }
		}
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
}
