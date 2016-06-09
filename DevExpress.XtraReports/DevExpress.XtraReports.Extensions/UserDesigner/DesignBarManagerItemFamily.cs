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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Preview;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.Design.Commands;
namespace DevExpress.XtraReports.UserDesigner {
	#region menu items
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BarReportTabButtonItem : BarButtonItem, ISupportReportCommand {
		ReportCommand command;
		public ReportCommand Command { get { return command; }
		}
		public BarReportTabButtonItem(BarManager manager, bool privateItem, ReportCommand command) : base() {
			fIsPrivateItem = privateItem;
			Manager = manager;
			this.command = command;
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BarReportTabButtonsListItem : BarLinkContainerItem {
		BarReportTabButtonItem item;
		XRDesignBarManager XRDesignBarManager { get { return Manager as XRDesignBarManager; } 
		}
		public BarReportTabButtonsListItem(bool isPrivateItem, BarManager manager) : this() {
			fIsPrivateItem = true;
			Manager = manager;
			OnGetItemData();
		}
		public BarReportTabButtonsListItem() {
			this.item = new BarReportTabButtonItem(null, true, ReportCommand.None);
			this.item.ItemClick += new ItemClickEventHandler(OnItemClick);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(item != null) {
					item.ItemClick -= new ItemClickEventHandler(OnItemClick);
					item.Dispose();
				}
				item = null;
			}
			base.Dispose(disposing);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public override LinksInfo LinksPersistInfo { 
			get { return null; } 
			set { 
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden), Browsable(false)]
		public override BarItemLinkCollection ItemLinks {
			get { return base.ItemLinks; }
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if(item == null) return;
			item.Manager = Manager;
			if(Manager != null) AddItem(item);
		}
		protected override void OnGetItemData() {
			ReportTabControl tabControl = null;
			if(XRDesignBarManager != null && XRDesignBarManager.XRDesignPanel != null && XRDesignBarManager.XRDesignPanel.View != null)
				tabControl = XRDesignBarManager.XRDesignPanel.GetService(typeof(ReportTabControl)) as ReportTabControl;
			BeginUpdate();
			try {
				ClearLinks();
				if(tabControl != null)
					for(int i = 0; i < tabControl.Pages.Count; i++) {
						if(tabControl.Pages[i].Visible)
							AddTabItem(tabControl.Pages[i].Text, i, tabControl.Pages[i].Command);
					}
			}
			finally {
				CancelUpdate();
			}
			base.OnGetItemData();
		}
		void AddTabItem(string caption, int tabIndex, ReportCommand command) {
			BarReportTabButtonItem btnItem = new BarReportTabButtonItem(Manager, true, command);
			btnItem.Caption = caption;
			btnItem.ButtonStyle = BarButtonStyle.Check;
			btnItem.Down = false;
			btnItem.Hint = String.Format(ReportLocalizer.GetString(ReportStringId.UD_Hint_ViewTabs), caption); 
			if(Manager is IDesignControl) {
				btnItem.Down = ((IDesignControl)Manager).XRDesignPanel.SelectedTabIndex == tabIndex;
				btnItem.Glyph = ((IDesignControl)Manager).XRDesignPanel.View.Pages[tabIndex].Image;
			} 
			btnItem.ItemClick += new ItemClickEventHandler(OnItemClick);
			AddItem(btnItem);
		}
		void OnItemClick(object sender, ItemClickEventArgs e) {
			if(Manager is IDesignControl && e.Item is BarReportTabButtonItem) {
				((IDesignControl)Manager).XRDesignPanel.ExecCommand(((BarReportTabButtonItem)e.Item).Command);
			}
		}
	}
	[
	ToolboxItem(false),
	]
	public class XRBarToolbarsListItem : BarToolbarsListItem {
		protected override void OnGetItemData() {
			base.OnGetItemData();
			foreach(BarItemLink itemLink in ItemLinks) {
				object tag = itemLink.Item.Tag;
				if(tag is DockPanel)
					itemLink.Item.Hint = String.Format(ReportLocalizer.GetString(ReportStringId.UD_Hint_ViewDockPanels), ((DockPanel)tag).Text); 
				if(tag is Bar)
					itemLink.Item.Hint = String.Format(ReportLocalizer.GetString(ReportStringId.UD_Hint_ViewBars), ((Bar)tag).Text); 
			}
		}
	}
	[
	ToolboxItem(false),
	]
	public class BarDockPanelsListItem : XRBarToolbarsListItem {
		XRDesignDockManager XRDesignDockManager { get { return Manager.DockManager as XRDesignDockManager; } 
		}
		public BarDockPanelsListItem() {
			ShowDockPanels = true;
			ShowToolbars = false;
			ShowCustomizationItem = false;
		}
		protected override void OnGetItemData() {
			base.OnGetItemData();
			foreach(BarItemLink link in ItemLinks) {
				DockPanel panel = link.Item.Tag as DockPanel;
				if(panel == null) continue;
				if(panel is TypedDesignDockPanel) {
					link.Visible = ((TypedDesignDockPanel)panel).EUDVisibility != EUDVisibility.Hidden;
				}
				if(0 <= panel.ImageIndex && panel.ImageIndex < XRDesignDockManager.Images.Count)
					link.Item.Glyph = XRDesignDockManager.Images[panel.ImageIndex];
			}
		}
	}
	[
	ToolboxItem(false),
	DesignTimeVisible(false),
	]
	#endregion
	#region repository items
	public class DesignRepositoryItemComboBox : RepositoryItemComboBox {
	}
	public class RecentlyUsedItemsComboBox : RepositoryItemFontEdit {
		public RecentlyUsedItemsComboBox() {
			AppearanceDropDown.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		}
		public void RememberRecentItem(String item) {
			base.StoreRecentItem(item);
		}
	}
	#endregion
	#region CommandBarItems
	public interface ISupportReportCommand {
		ReportCommand Command { get; }
	}
	public class CommandBarCheckItem : BarCheckItem, ISupportCommand, ISupportReportCommand {
		CommandContainer checkedCommandHolder;
		CommandContainer uncheckedCommandHolder;
		public CommandBarCheckItem() {
			checkedCommandHolder = new CommandContainer();
			uncheckedCommandHolder = new CommandContainer();
		}
		public CommandID CommandID { get{ return Checked ? checkedCommandHolder.CommandID : uncheckedCommandHolder.CommandID; } 
		}
		public ReportCommand Command {
			get { return Checked ? checkedCommandHolder.Command : uncheckedCommandHolder.Command; }
		}
		public ReportCommand CheckedCommand {
			get { return this.checkedCommandHolder.Command; }
			set { this.checkedCommandHolder.Command = value; }
		}
		public ReportCommand UncheckedCommand {
			get { return this.uncheckedCommandHolder.Command; }
			set { this.uncheckedCommandHolder.Command = value; }
		}
	}
	public class CommandBarItem : DevExpress.XtraBars.BarButtonItem, ISupportCommand, ISupportReportCommand {
		CommandContainer commandHolder;
		public CommandBarItem() {
			commandHolder = new CommandContainer();
		}
		#region obsolete
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("The CommandIDID property is now obsolete. Use the Command property instead."),
		]
		public int CommandIDID {
			get { return commandHolder.CommandIDID; } 
			set { commandHolder.CommandIDID = value; } 
		}
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("The CommandIDGuid property is now obsolete. Use the Command property instead."),
		]
		public Guid CommandIDGuid {
			get { return this.commandHolder.CommandIDGuid; }
			set { this.commandHolder.CommandIDGuid = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The CommandID property is now obsolete. Use the Command property instead."),
		]
		public CommandID CommandID { get{ return this.commandHolder.CommandID; } 
		}
		#endregion
		[
		DefaultValue(ReportCommand.None),
		SRCategory(ReportStringId.CatBehavior),
		]
		public virtual ReportCommand Command {
			get { return this.commandHolder.Command; }
			set { this.commandHolder.Command = value; }
		}
	}
	[
	ToolboxItem(false),
	DesignTimeVisible(false),
	]
	public class CommandColorBarItem : CommandBarItem {
		ColorPopupControlContainer colorPopupControlContainer = new ColorPopupControlContainer();
		XRDesignItemsLogicBase xrDesignItemsLogic;
		public ColorPopupControlContainer ColorPopupControlContainer { get { return colorPopupControlContainer; }
		}
		internal XRDesignItemsLogicBase DesignItemsLogic {
			set { xrDesignItemsLogic = value; }
		}
		public CommandColorBarItem() {
			CloseSubMenuOnClick = false;
			colorPopupControlContainer.Visible = false;
			ButtonStyle = BarButtonStyle.DropDown;
			DropDownControl = colorPopupControlContainer;
			colorPopupControlContainer.Item = this;
			colorPopupControlContainer.Popup += new EventHandler(OnColorPopup);
			colorPopupControlContainer.CloseUp += new EventHandler(OnColorCloseUp);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && colorPopupControlContainer != null) {
				colorPopupControlContainer.Popup -= new EventHandler(OnColorPopup);
				colorPopupControlContainer.CloseUp -= new EventHandler(OnColorCloseUp);
				colorPopupControlContainer.Dispose();
				colorPopupControlContainer = null;
			}
			base.Dispose(disposing);
		}
		protected override void OnClick(BarItemLink link) {
			ExecuteColorCommand();
		}
		void ExecuteColorCommand() {
			System.Diagnostics.Debug.Assert(xrDesignItemsLogic != null);
			System.Diagnostics.Debug.Assert(xrDesignItemsLogic.XRDesignPanel != null);
			xrDesignItemsLogic.XRDesignPanel.ExecCommand(Command, new object[] { colorPopupControlContainer.ResultColor });
		}
		void OnColorPopup(object sender, EventArgs e) {
			colorPopupControlContainer.Manager = Manager;
		}
		void OnColorCloseUp(object sender, EventArgs e) {
			ExecuteColorCommand();
		}
	}
	public class CommandBarItemHashTable {
		Hashtable table = new Hashtable();
		public CommandBarItem this [ReportCommand command] { get { return table[CommandIDReportCommandConverter.GetCommandID(command)] as CommandBarItem; } set { table[CommandIDReportCommandConverter.GetCommandID(command)] = value; } 
		}
		public int Count { get { return table.Count; }
		}
		public void Add(ReportCommand command, CommandBarItem barItem) {
			table.Add(CommandIDReportCommandConverter.GetCommandID(command), barItem);
		}
		public void Clear() {
			table.Clear();
		}
	}
	#endregion //CommandBarItems
	#region zoom item
	public class XRZoomBarEditItem : ZoomBarEditItemBase, ISupportReportCommand {
		ZoomService zoomService;
		protected override int MinZoomFactor { get { return ZoomService.MinZoomInPercents; } 
		}
		protected override int MaxZoomFactor { get { return ZoomService.MaxZoomInPercents; } 
		}
		protected override string ZoomStringFormat { get { return ZoomService.ZoomStringFormat; } 
		}
		protected override float CurrentZoom { get { return zoomService != null ? zoomService.ZoomFactor : 1.0f; } 
		}
		internal void SetZoomService(ZoomService value) {
			if(zoomService != null)
				UnsubscribeEvents();
			zoomService = value;
			if(zoomService != null)
				SubscribeEvents();
			base.EditValue = GetEditValue();
		}
		ReportCommand ISupportReportCommand.Command { get { return ReportCommand.Zoom; }
		}
		public XRZoomBarEditItem() {
		}
		protected override object[] CreateItems() {
			ArrayList items = new ArrayList();
			foreach(int zoomFactor in ZoomService.PredefinedZoomFactorsInPercents) {
				items.Add(new ZoomComboBoxItemBase(zoomFactor + "%"));
			}
			return (object[])items.ToArray(typeof(object));
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (zoomService != null) {
					UnsubscribeEvents();
					zoomService = null;
				}
			}
			base.Dispose(disposing);
		}
		void SubscribeEvents() {
			zoomService.ZoomChanged += new EventHandler(OnZoomChanged);
		}
		void UnsubscribeEvents() {
			zoomService.ZoomChanged -= new EventHandler(OnZoomChanged);
		}
		protected override void ExecZoomCommandCore(float zoomFactor) {
			UnsubscribeEvents();
			zoomService.ZoomFactor = zoomFactor;
			SubscribeEvents();
		}
		protected override bool CanExecZoomCommand() {
			return zoomService != null;
		}
		protected override void ApplyNewEditValue() {
			ExecZoomCommand(EditValue);
		}
		void OnZoomChanged(object sender, EventArgs e) {
			base.EditValue = GetEditValue();
		}
	}
	public class ZoomRuntimeCommandBarItem : CommandBarItem {
		int zoomPercent;
		ZoomService zoomService;
		public ZoomRuntimeCommandBarItem(int zoomPercent)
			: base() {
			Command = ReportCommand.Zoom;
			Caption = String.Format("{0}%", zoomPercent);
			this.zoomPercent = zoomPercent;
		}
		protected override void OnClick(BarItemLink link) {
			base.OnClick(link);
			if(zoomService != null)
				zoomService.ZoomFactor = (float)zoomPercent / 100f;
		}
		internal void SetZoomService(ZoomService zoomService) {
			this.zoomService = zoomService;
		}
	}
	public class ScriptsCommandBarItem : CommandBarItem {
		public override bool Down { get { return Command != ReportCommand.ShowScriptsTab; } set { } }
		public ScriptsCommandBarItem()
			: base() {
			Command = ReportCommand.ShowScriptsTab;
			ButtonStyle = BarButtonStyle.Check;
		}
		internal void UpdateCommand(ReportCommand command) {
			Command = command;
		}
	}
	#endregion
}
