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
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraReports.UserDesigner;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.Utils.UI.Localization;
using System.Windows.Forms.Design;
namespace DevExpress.XtraReports.Design {
	[System.ComponentModel.ToolboxItem(false)]
	public class PropertyGridUserControl : XtraUserControl {
		static void SetBarButtonToolTip(DevExpress.XtraBars.BarButtonItem barButton, UtilsUIStringId id) {
			SetBarButtonToolTip(barButton, UtilsUILocalizer.GetString(id));
		}
		static void SetBarButtonToolTip(DevExpress.XtraBars.BarButtonItem barButton, string value) {
			DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
			DevExpress.Utils.ToolTipTitleItem toolTipTitleItem = new DevExpress.Utils.ToolTipTitleItem();
			toolTipTitleItem.Text = value;
			superToolTip.Items.Add(toolTipTitleItem);
			barButton.SuperTip = superToolTip;
		}
		class CustomPropertyGridControl : DevExpress.XtraVerticalGrid.PropertyGridControl {
			List<RepositoryItem> items = new List<RepositoryItem>();
			public CustomPropertyGridControl() {
				this.CustomRecordCellEdit += new GetCustomRowCellEditEventHandler(OnCustomRecordCellEdit);
			}
			void OnCustomRecordCellEdit(object sender, GetCustomRowCellEditEventArgs e) {
				IDesignerHost host = this.ServiceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				object obj;
				PropertyDescriptor pd = GetPropertyDescriptor(e.Row, out obj);
				if(host == null || !(obj is Parameter) || pd == null)
					return;
				IExtensionsProvider rootComponent = (IExtensionsProvider)host.RootComponent;
				EditingContext editingContext = new EditingContext(rootComponent.Extensions[DataEditorService.Guid], rootComponent);
				RepositoryItem item = DataEditorService.GetRepositoryItem(pd.PropertyType, obj, editingContext);
				ClearItems();
				if(item != null) {
					items.Add(item);
					RepositoryItems.Add(item);
					e.RepositoryItem = item;
				}
			}
			void ClearItems() { 
				foreach(RepositoryItem item in items) {
					if(RepositoryItems.Contains(item))
						RepositoryItems.Remove(item);
				}
				items.Clear();
			}
			protected override void InitializeCore() {
			}
			protected override void ProcessUIEditingException(Exception e) {
				IUIService serv = this.ServiceProvider.GetService(typeof(IUIService)) as IUIService;
				if(serv != null)
					serv.ShowError(e);
				else
					base.ProcessUIEditingException(e);
			}
		}
		private System.ComponentModel.IContainer components = null;
		private DevExpress.XtraBars.BarManager barManager;
		private DevExpress.XtraBars.Bar commandBar;
		private DevExpress.XtraBars.BarButtonItem barButtonCategorized;
		private DevExpress.XtraBars.BarButtonItem barButtonAlphabetical;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		DevExpress.XtraVerticalGrid.PropertyGridControl propertyGridControl;
		DevExpress.XtraVerticalGrid.PropertyDescriptionControl propertyDescriptionControl;
		SplitterControl splitter;
		bool allowGlyphSkinning;
		public bool AllowGlyphSkinning {
			get { return this.allowGlyphSkinning; }
			set {
				if(value != allowGlyphSkinning) {
					this.allowGlyphSkinning = value;
					UpdateGlyphSkinning();
				}
			}
		}
		public bool ShowCategories {
			get { return this.barButtonCategorized.Down; }
			set { this.barButtonCategorized.Down = value; }
		}
		public bool ShowDescription {
			get { return this.propertyDescriptionControl.Visible; }
			set {
				this.splitter.Visible = value;
				this.propertyDescriptionControl.Visible = value;
			}
		}
		public DevExpress.XtraVerticalGrid.PropertyGridControl PropertyGridControl {
			get { return propertyGridControl; }
		}
		public object SelectedObject {
			get { return propertyGridControl.SelectedObject; }
			set { propertyGridControl.SelectedObject = value; }
		}
		public object[] SelectedObjects {
			get { return propertyGridControl.SelectedObjects; }
			set { propertyGridControl.SelectedObjects = value; }
		}
		public IServiceProvider ServiceProvider {
			get { return propertyGridControl.ServiceProvider; }
			set { propertyGridControl.ServiceProvider = value; }
		}
		public PropertyGridUserControl() {
			InitializeComponent();
			propertyGridControl.MenuManager = barManager;
			this.barButtonCategorized.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barButton_DownChanged);
			this.barButtonAlphabetical.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barButton_DownChanged);
			SetBarButtonToolTip(this.barButtonAlphabetical, UtilsUIStringId.PropGrid_TTip_Alphabetical);
			SetBarButtonToolTip(this.barButtonCategorized, UtilsUIStringId.PropGrid_TTip_Categorized);
			UpdateGlyphSkinning();
			this.ShowCategories = true;
		}		
		public void SetLookAndFeel(IServiceProvider servProvider) {
			DesignLookAndFeelHelper.SetParentLookAndFeel(this.PropertyGridControl, servProvider);
			DesignLookAndFeelHelper.SetLookAndFeel(this.barManager, servProvider);
			DesignLookAndFeelHelper.SetParentLookAndFeel(this, servProvider);
		}
		public PropertyDescriptor GetPropertyDescriptor(DevExpress.XtraVerticalGrid.Rows.BaseRow row) {
			return this.propertyGridControl.GetPropertyDescriptor(row);
		}
		void barButton_DownChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			if(e.Item == barButtonAlphabetical)
				SetBarButtonDown(this.barButtonCategorized, !this.barButtonAlphabetical.Down);
			else
				SetBarButtonDown(this.barButtonAlphabetical, !this.barButtonCategorized.Down);
			UpdatePropertyGrid();
		}
		void SetBarButtonDown(DevExpress.XtraBars.BarButtonItem barButton, bool value) {
			barButton.DownChanged -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButton_DownChanged);
			barButton.Down = value;
			barButton.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barButton_DownChanged);
		}
		void UpdateGlyphSkinning() {
			string imageNamePostfix = AllowGlyphSkinning ? "_Monochrome" : string.Empty;
			this.propertyGridControl.OptionsView.AllowGlyphSkinning = AllowGlyphSkinning;
			this.barButtonCategorized.AllowGlyphSkinning = this.barButtonAlphabetical.AllowGlyphSkinning = AllowGlyphSkinning ? DefaultBoolean.True : DefaultBoolean.False;
			this.barButtonCategorized.Glyph = ResourceImageHelper.CreateImageFromResources("Images.Categorized"+ imageNamePostfix +".png", typeof(DevExpress.Utils.UI.ResFinder));
			this.barButtonAlphabetical.Glyph = ResourceImageHelper.CreateImageFromResources("Images.Alphabetical"+ imageNamePostfix +".png", typeof(DevExpress.Utils.UI.ResFinder));
		}
		void UpdatePropertyGrid() {
			this.propertyGridControl.OptionsView.ShowRootCategories = this.barButtonCategorized.Down;
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		public void ExpandProperty(string propertyName) {
			DevExpress.XtraVerticalGrid.Rows.BaseRow row = propertyGridControl.Rows["row" + propertyName];
			if(row != null)
				row.Expanded = true;
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.barManager = new RuntimeBarManager(this.components);
			this.commandBar = new DevExpress.XtraBars.Bar();
			this.barButtonCategorized = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonAlphabetical = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.propertyGridControl = new CustomPropertyGridControl();
			this.propertyDescriptionControl = new DevExpress.XtraVerticalGrid.PropertyDescriptionControl();
			this.splitter = new SplitterControl();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGridControl)).BeginInit();
			this.SuspendLayout();
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.commandBar});
			this.barManager.AllowCustomization = false;
			this.barManager.AllowShowToolbarsPopup = false;
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.barButtonCategorized,
			this.barButtonAlphabetical});
			this.barManager.MaxItemId = 2;
			this.barManager.Controller = new DevExpress.XtraBars.BarAndDockingController();
			this.barManager.Controller.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			this.commandBar.BarName = "Tools";
			this.commandBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
			this.commandBar.DockCol = 0;
			this.commandBar.DockRow = 0;
			this.commandBar.OptionsBar.AllowQuickCustomization = false;
			this.commandBar.OptionsBar.DisableCustomization = true;
			this.commandBar.OptionsBar.DrawDragBorder = false;
			this.commandBar.OptionsBar.UseWholeRow = true;
			this.commandBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.commandBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonCategorized),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonAlphabetical)});
			this.commandBar.Text = "Tools";
			this.barButtonCategorized.Caption = "Categorized";
			this.barButtonCategorized.Id = 0;
			this.barButtonCategorized.Name = "barButtonCategorized";
			this.barButtonCategorized.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
			this.barButtonAlphabetical.Caption = "Alphabetical";
			this.barButtonAlphabetical.Id = 1;
			this.barButtonAlphabetical.Name = "barButtonAlphabetical";
			this.barButtonAlphabetical.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
			this.propertyGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridControl.Name = "propertyGridControl";
			this.propertyGridControl.AutoGenerateRows = true;
			this.propertyGridControl.OptionsMenu.EnableContextMenu = true;
			this.propertyGridControl.OptionsBehavior.ResizeRowValues = false;
			this.propertyGridControl.OptionsBehavior.ResizeRowHeaders = false;
			this.propertyGridControl.Size = new System.Drawing.Size(250, 200);
			this.propertyGridControl.ServiceProvider = null;
			this.propertyGridControl.DefaultEditors.Add(typeof(System.Drawing.Color), new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit());
			this.propertyDescriptionControl.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.propertyDescriptionControl.Name = "propertyDescriptionControl";
			this.propertyDescriptionControl.PropertyGrid = this.propertyGridControl;
			this.propertyDescriptionControl.Size = new System.Drawing.Size(250, 50);
			this.propertyDescriptionControl.TabStop = false;
			this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter.Name = "splitter";
			this.splitter.Size = new System.Drawing.Size(250, 3);
			this.splitter.TabStop = false;
			this.Controls.Add(this.propertyGridControl);
			this.Controls.Add(this.splitter);
			this.Controls.Add(this.propertyDescriptionControl);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "UserControl1";
			this.Size = new System.Drawing.Size(273, 254);
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGridControl)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
	}
}
