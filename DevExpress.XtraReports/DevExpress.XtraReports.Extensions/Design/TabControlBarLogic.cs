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
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraBars;
using System.Windows.Forms;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraBars.Ribbon;
using System.Collections;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.Design {
	class TabControlBarLogic : TabControlLogic {
		#region static
		static BarButtonItem CreateBarButtonItem(string text, Bar bar, int index, bool beginGroup) {
			BarButtonItem barButtonItem = new BarButtonItem(bar.Manager, text);
			barButtonItem.ButtonStyle = BarButtonStyle.Check;
			barButtonItem.PaintStyle = BarItemPaintStyle.CaptionGlyph;
			barButtonItem.ImageIndex = index;
			barButtonItem.GroupIndex = 1;
			bar.ItemLinks[0].BeginGroup = true;
			bar.InsertItem(bar.ItemLinks[index], barButtonItem).BeginGroup = beginGroup;
			return barButtonItem;
		}
		static bool IsZoomItem(BarItem item) {
			IStatusPanel panel = item as IStatusPanel;
			return (item is ZoomTrackBarEditItem) || (panel != null && panel.StatusPanelID == StatusPanelID.ZoomFactor);
		}
		static void InitDesignStaticItem(DesignBarStaticItem item) {
			item.LeftIndent = 1;
			item.RightIndent = 1;
			item.Visible = false;
		}
		#endregion
		#region inner classes
		class DesignPrintBarManagerConfigurator : PrintBarManagerConfigurator {
			public DesignPrintBarManagerConfigurator(PrintBarManager manager)
				: base(manager) {
			}
			protected override Bar AddMainMenuBar(string barName, int dockCol, int dockRow, XtraBars.BarDockStyle barDockStyle, string text) {
				return null;
			}
		}
		class BarButtonTabPage : XtraTabControl.TabPage {
			private BarButtonItem barButtonItem;
			public override bool Selected {
				get { return base.Selected; }
				set {
					if(base.Selected != value)
						barButtonItem.Down = value;
					base.Selected = value;
				}
			}
			public override bool Visible {
				get { return base.Visible; }
				set {
					base.Visible = value;
					barButtonItem.Visibility = value ? BarItemVisibility.Always : BarItemVisibility.Never;
				}
			}
			public override Image Image {
				get { return barButtonItem.Glyph; }
				set { barButtonItem.Glyph = value; }
			}
			public BarButtonTabPage(Control control, string text, BarButtonItem button, ReportCommand command)
				: base(control, text, command) {
				barButtonItem = button;
				SubscribeEvents();
			}
			public override void Dispose() {
				UnSubscribeEvents();
			}
			void OnItemClick(object sender, ItemClickEventArgs e) {
				OnPageActivate(EventArgs.Empty);
			}
			void SubscribeEvents() {
				barButtonItem.ItemClick += new XtraBars.ItemClickEventHandler(OnItemClick);
			}
			void UnSubscribeEvents() {
				barButtonItem.ItemClick -= new XtraBars.ItemClickEventHandler(OnItemClick);
			}
		}
		#endregion
		List<BarItemLink> previewStaticItems = new List<BarItemLink>();
		BarStatusItem designStatusItem;
		BarStaticItem designZoomItem;
		PrintBarManager printBarManager = new PrintBarManager();
		bool barManagerActivated;
		IServiceProvider serviceProvider;
#if DEBUGTEST
		public override BarStaticItem DesignZoomItem {
			get { return designZoomItem; }
		}
#endif
		internal override BarManager BarManager {
			get { return printBarManager; }
		}
		bool ShouldActivateBarManager {
			get { return !barManagerActivated && !tabControl.ClientSize.IsEmpty; }
		}
		Bar StatusBar {
			get { return printBarManager.StatusBar; }
		}
		public TabControlBarLogic(ReportTabControl tabControl, IServiceProvider servProvider)
			: base(tabControl, servProvider) {
		}
		public override void Dispose() {
			BarAndDockingController controller = null;
			if(printBarManager != null) {
				controller = printBarManager.Controller;
				printBarManager.Controller = null;
				printBarManager.Dispose();
				printBarManager = null;
			}
			if(controller != null) {
				controller.Dispose();
			}
		}
		protected BarButtonItem CreateButton(ReportStringId id, bool beginGroup) {
			string text = ReportLocalizer.GetString(id);
			return CreateBarButtonItem(text, this.StatusBar, tabControl.Pages.Count, beginGroup);
		}
		public override void AddPage(Control control, ReportStringId id, ReportCommand command, bool beginGroup) {
			XtraTabControl.TabPage page = new BarButtonTabPage(control, GetStringById(id), CreateButton(id,beginGroup), command);
			tabControl.AddPage(control, page);
		}
		public override void AddPage(ReportStringId id, ReportCommand command, bool beginGroup) {
			XtraTabControl.TabPage page = new BarButtonTabPage(null, GetStringById(id), CreateButton(id, beginGroup), command);
			tabControl.AddPage(page);
		}
		public override void Initialize(IServiceProvider serviceProvider, bool isEUD) {
			this.serviceProvider = serviceProvider;
			printBarManager.Form = tabControl;
			printBarManager.AllowShowToolbarsPopup = false;
			DesignPrintBarManagerConfigurator configurator = new DesignPrintBarManagerConfigurator(printBarManager);
			configurator.Configure();
			System.Diagnostics.Debug.Assert(previewStaticItems.Count == 0);
			foreach(BarItemLink link in StatusBar.ItemLinks)
				previewStaticItems.Add(link);
			CreateDesignStaticItems();
			DesignLookAndFeelHelper.SetLookAndFeel(printBarManager, serviceProvider);
			if(!isEUD) {
				BarEditItem zoomItem = (BarEditItem)printBarManager.GetBarItemByCommand(PrintingSystemCommand.Zoom);
				((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)zoomItem.Edit).TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			}
		}
		public override void UpdateStatus(string status) {
			designStatusItem.UpdateStatus(status);
		}
		public override void OnDesignerVisible() {
			base.OnDesignerVisible();
			printBarManager.PreviewBar.Visible = false;
			SetDesignBarStaticItemsVisible(true);
			SetPreviewStatusVisibility(false);
			ZoomEditItem.Visibility = BarItemVisibility.Always;
			EnableZoomItems(true);
		}
		public override void OnPreviewVisible() {
			base.OnPreviewVisible();
			printBarManager.PreviewBar.Visible = tabControl.ShowPrintPreviewBar;
			SetDesignBarStaticItemsVisible(false);
			SetPreviewStatusVisibility(true);
			SetPreviewStatusVisibility(true, StatusPanelID.PageOfPages);
			EnableZoomItems(true);
		}
		public override void OnScriptsVisible() {
			base.OnScriptsVisible();
			printBarManager.PreviewBar.Visible = false;
			SetDesignBarStaticItemsVisible(false);
			SetPreviewStatusVisibility(false);
			EnableZoomItems(false);
		}
		public override void OnBrowserVisible() {
			base.OnBrowserVisible();
			printBarManager.PreviewBar.Visible = false;
			SetDesignBarStaticItemsVisible(false);
			SetPreviewStatusVisibility(true);
			SetPreviewStatusVisibility(false, StatusPanelID.PageOfPages);
			EnableZoomItems(false);
		}
		public override void Activate() {
			if(ShouldActivateBarManager)
				tabControl.BeginInvoke(new MethodInvoker(ActivateBarManager));
		}
		public override void OnPrintControlCreated() {
			printBarManager.PrintControl = tabControl.PreviewControl;
		}
		protected override ZoomTrackBarEditItem CreateZoomTrackBar() {
			foreach(BarItemLink link in previewStaticItems)
				if(link.Item is ZoomTrackBarEditItem)
					return link.Item as ZoomTrackBarEditItem;
			return printBarManager.GetBarItemByCommand(PrintingSystemCommand.ZoomTrackBar) as ZoomTrackBarEditItem;
		}
		protected override ZoomService CreateZoomService() {
			return serviceProvider.GetService(typeof(ZoomService)) as ZoomService;
		}
		protected override BarStaticItem GetZoomFactorTextStaticItem() {
			return designZoomItem;
		}
		protected override void SetBarItemLocked(BarItem item, bool locked) {
			printBarManager.SetBarItemLocked(item, locked);
		}
		void SetPreviewStatusVisibility(bool visible, params StatusPanelID[] ids) {
			SetLinksVisibility(previewStaticItems, visible, ids);
		}
		protected override void EnableCommand(PrintingSystemCommand command, bool enabled) {
			printBarManager.EnableCommand(command, enabled);
		}
		void SetLinksVisibility(IList itemLinks, bool visible, params StatusPanelID[] ids) {
			foreach(BarItemLink link in itemLinks) {
				IStatusPanel panel = link.Item as IStatusPanel;
				if(!IsZoomItem(link.Item) && panel != null && Array.IndexOf(ids, panel.StatusPanelID) >= 0)
					link.Visible = visible;
			}
		}
		void SetPreviewStatusVisibility(bool visible) {
			foreach(BarItemLink link in previewStaticItems) {
				if(!IsZoomItem(link.Item))
					link.Visible = visible;
			}
		}
		void SetDesignBarStaticItemsVisible(bool visible) {
			designStatusItem.Visible = visible;
			designZoomItem.Visibility = BarItemVisibility.Always;
			ZoomEditItem.Visibility = BarItemVisibility.Always;
			ZoomEditItem.Refresh();
		}
		void ActivateBarManager() {
			if(!barManagerActivated) {
				barManagerActivated = true;
				((IBarManagerControl)printBarManager).Activate();
			}
		}
		BarStaticItem GetDesignZoomItem() {
			foreach(BarItemLink link in previewStaticItems) {
				IStatusPanel panel = link.Item as IStatusPanel;
				if(panel != null && (panel.StatusPanelID == StatusPanelID.ZoomFactor))
					return link.Item as BarStaticItem;
			}
			return new BarZoomItem(serviceProvider);
		}
		void CreateDesignStaticItems() {
			designStatusItem = new BarStatusItem();
			InitDesignStaticItem(designStatusItem);
			StatusBar.AddItem(designStatusItem).BeginGroup = true;
			designZoomItem = GetDesignZoomItem();
			StatusBar.RemoveLink(designZoomItem.Links[0]);
			StatusBar.RemoveLink(ZoomEditItem.Links[0]);
			StatusBar.AddItem(designZoomItem).BeginGroup = false;
			StatusBar.AddItem(ZoomEditItem);
			ZoomEditItem.Visibility = BarItemVisibility.Always;
			designZoomItem.Visibility = BarItemVisibility.Always;
		}
		void EnableZoomItems(bool enabled) {
			ZoomEditItem.Enabled = enabled;
			designZoomItem.Enabled = enabled;
		}
	}
}
