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
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Drawing.Design;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public abstract class XRDesignItemsLogicBase : IDisposable {
		#region static
		static readonly ReportCommand[] checkedCommands = new ReportCommand[] {ReportCommand.FontBold, ReportCommand.FontItalic, ReportCommand.FontUnderline,
			ReportCommand.JustifyCenter, ReportCommand.JustifyJustify, ReportCommand.JustifyLeft, ReportCommand.JustifyRight };
		static readonly ReportCommand[] contextCommands = { ReportCommand.SelectAll, ReportCommand.Copy, ReportCommand.Cut };
		static internal bool IsColorPopupCommand(ReportCommand command) {
			return object.Equals(command, ReportCommand.ForeColor) || object.Equals(command, ReportCommand.BackColor);
		}
		internal static BarItem[] GetBarItemsByReportCommand(BarManager manager, ReportCommand command) {
			ArrayList items = new ArrayList();
			foreach(BarItem item in manager.Items) {
				if(item is ISupportReportCommand && ((ISupportReportCommand)item).Command == command)
					items.Add(item);
			}
			return (BarItem[])items.ToArray(typeof(BarItem));
		}
		#endregion
		#region InnerClasses
		internal class ToolBarFontService : DevExpress.XtraReports.Design.FontServiceBase {
			RootToolBarFontService rootoolBarFontService;
			bool enabled;
			FontSurrogate font;
			public bool IsActive { get; private set; }
			public FontSurrogate Font { get { return font; } }
			public ToolBarFontService(IServiceProvider serviceProvider, RootToolBarFontService rootoolBarFontService)
				: base(serviceProvider) {
				this.rootoolBarFontService = rootoolBarFontService;
				this.rootoolBarFontService.Register(this);
				Subscribe(serviceProvider);
			}
			public override void Dispose() {
				Unsubscribe();
				this.rootoolBarFontService.Unregister(this);
				base.Dispose();
			}
			void Subscribe(IServiceProvider serviceProvider) {
				IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
				host.Activated += new EventHandler(host_Activated);
				host.Deactivated += new EventHandler(host_Deactivated);
			}
			void Unsubscribe() {
				IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
				host.Activated -= new EventHandler(host_Activated);
				host.Deactivated -= new EventHandler(host_Deactivated);
			}
			void host_Activated(object sender, EventArgs e) {
				IsActive = true;
				rootoolBarFontService.SetFontControlsVisibility(enabled);
			}
			void host_Deactivated(object sender, EventArgs e) {
				IsActive = false;
			}
			protected override void SetFontControlsVisibility(bool enabled) {
				this.enabled = enabled;
				rootoolBarFontService.SetFontControlsVisibility(enabled);
			}
			protected override void ResetFontControls() {
				rootoolBarFontService.ResetFontControls();
			}
			public override void UpdateFontControls(FontSurrogate font) {
				this.font = font;
				rootoolBarFontService.UpdateFontControls(font);
			}
			public void FontNameEntered(string item) {
				ExecuteFontNameCommand(item);
			}
			public void FontSizeEntered(string item) {
				ExecuteFontSizeCommand(item);
			}
		}
		internal class RootToolBarFontService : IDisposable {
			#region static
			static void SetComboBoxText(RepositoryItemComboBox comboBox, XtraBars.BarEditItem barEditItem, object val) {
				if(barEditItem == null || comboBox == null || !comboBox.Enabled)
					return;
				for(int i = 0; i < comboBox.Items.Count; i++) {
					if(comboBox.Items[i].Equals(val)) {
						barEditItem.EditValue = comboBox.Items[i];
						return;
					}
				}
				barEditItem.EditValue = val.ToString();
			}
			#endregion
			BarEditItem nullEditItem = new BarEditItem();
			List<ToolBarFontService> services = new List<ToolBarFontService>();
			bool subscribed;
			public ToolBarFontService GetActiveService() {
				foreach(ToolBarFontService service in services)
					if(service.IsActive)
						return service;
				return null;
			}
			XtraEditors.Repository.RepositoryItemComboBox FontSizeBox {
				get { return designItemsLogic.FontSizeBox; }
			}
			XtraBars.BarEditItem FontSizeEdit {
				get { return designItemsLogic != null && designItemsLogic.FontSizeEdit != null ? designItemsLogic.FontSizeEdit : nullEditItem; }
			}
			XtraEditors.Repository.RepositoryItemComboBox FontNameBox {
				get { return designItemsLogic.FontNameBox; }
			}
			XtraBars.BarEditItem FontNameEdit {
				get { return designItemsLogic != null & designItemsLogic.FontNameEdit != null ? designItemsLogic.FontNameEdit : nullEditItem; }
			}
			XRDesignItemsLogicBase designItemsLogic;
			public RootToolBarFontService(XRDesignItemsLogicBase designItemsLogic) {
				this.designItemsLogic = designItemsLogic;
				SubscribeEvents();
			}
			public void Dispose() {
				UnsubscribeEvents();
			}
			private void SubscribeEvents() {
				if(subscribed) return;
				subscribed = true;
				FontNameEdit.EditValueChanged += new EventHandler(OnFontNameChanged);
				FontSizeEdit.EditValueChanged += new EventHandler(OnFontSizeChanged);
			}
			private void UnsubscribeEvents() {
				if(!subscribed) return;
				subscribed = false;
				FontNameEdit.EditValueChanged -= new EventHandler(OnFontNameChanged);
				FontSizeEdit.EditValueChanged -= new EventHandler(OnFontSizeChanged);
			}
			private void OnFontNameChanged(object sender, EventArgs e) {
				ToolBarFontService activeService = GetActiveService();
				if(activeService != null) {
					activeService.FontNameEntered(this.FontNameEdit.EditValue.ToString());
					if(designItemsLogic.RecentlyItemsBox != null)
						designItemsLogic.RecentlyItemsBox.RememberItem(FontNameEdit.EditValue.ToString());
				}
			}
			private void OnFontSizeChanged(object sender, EventArgs e) {
				ToolBarFontService activeService = GetActiveService();
				if(activeService != null)
					activeService.FontSizeEntered(this.FontSizeEdit.EditValue.ToString());
			}
			public void SetFontControlsVisibility(bool enabled) {
				UnsubscribeEvents();
				if(enabled) {
					ToolBarFontService activeService = GetActiveService();
					if(activeService != null)
						UpdateFontControls(activeService.Font);
				} else {
					ResetFontControls();
				}
				SubscribeEvents();
				FontNameEdit.Enabled = enabled;
				FontSizeEdit.Enabled = enabled;
			}
			public void ResetFontControls() {
				FontNameEdit.EditValue = "";
				FontSizeEdit.EditValue = "";
			}
			public void UpdateFontControls(FontSurrogate font) {
				UnsubscribeEvents();
				ResetFontControls();
				if(font != null && !font.IsEmpty) {
					SetComboBoxText(FontNameBox, FontNameEdit, font.Name);
					SetComboBoxText(FontSizeBox, FontSizeEdit, font.Size);
					if(designItemsLogic.RecentlyItemsBox != null)
						designItemsLogic.RecentlyItemsBox.RememberItem(font.Name);
				}
				SubscribeEvents();
			}
			public void Register(ToolBarFontService service) {
				if(!services.Contains(service))
					services.Add(service);
			}
			public void Unregister(ToolBarFontService service) {
				services.Remove(service);
			}
		}
		#endregion
		BarManager manager;
		XtraBars.BarEditItem fontNameEdit;
		RepositoryItemComboBox fontNameBox;
		XtraBars.BarEditItem fontSizeEdit;
		RepositoryItemComboBox fontSizeBox;
		XRDesignPanel xrDesignPanel;
		RootToolBarFontService rootfontService;
		BarStaticItem hintStaticItem;
		ReportCommandServiceBase reportCommandService;
		IServiceProvider serviceProvider;
		internal DevExpress.XtraEditors.IRecentlyUsedItems RecentlyItemsBox;
		#region properties
		ReportCommandServiceBase ReportCommandService {
			get {
				if(reportCommandService == null) {
					reportCommandService = serviceProvider.GetService(typeof(ReportCommandServiceBase)) as ReportCommandServiceBase;
					if(reportCommandService != null)
						reportCommandService.CommandChanged += new ReportCommandEventHandler(reportCommandService_CommandChanged);
				}
				return reportCommandService;
			}
		}
		protected BarManager Manager {
			get { return manager; }
		}
		internal XtraBars.BarEditItem FontNameEdit {
			get { return this.fontNameEdit; }
			set { this.fontNameEdit = value; }
		}
		internal RepositoryItemComboBox FontNameBox {
			get { return this.fontNameBox; }
			set { this.fontNameBox = value; 
				RecentlyItemsBox = (DevExpress.XtraEditors.IRecentlyUsedItems)value; 
			}
		}
		internal XtraBars.BarEditItem FontSizeEdit {
			get { return this.fontSizeEdit; }
			set { this.fontSizeEdit = value; }
		}
		internal RepositoryItemComboBox FontSizeBox {
			get { return this.fontSizeBox; }
			set { this.fontSizeBox = value; }
		}
		internal BarStaticItem HintStaticItem {
			get { return hintStaticItem; }
			set { hintStaticItem = value; }
		}
		internal XRDesignPanel XRDesignPanel {
			get { return this.xrDesignPanel; }
			set {
				if(xrDesignPanel != null) {
					UnsubscribeDesignPanelEvents();
				}
				xrDesignPanel = value;
				if(xrDesignPanel != null) {
					SubscribeDesignPanelEvents();
				}
				UpdateBarItems();
			}
		}
		protected ZoomService ZoomService {
			get {
				IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
				return designerHost != null ? (ZoomService)designerHost.GetService(typeof(ZoomService)) : null;
			}
		}
		protected IToolboxService ToolboxService { 
			get { 
				return GetService(typeof(IToolboxService)) as IToolboxService; 
			} 
		}
		#endregion
		#region IDisposable implementaion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(xrDesignPanel != null) {
					UnsubscribeDesignPanelEvents();
					xrDesignPanel = null;
				}
				if(reportCommandService != null) {
					reportCommandService.CommandChanged -= new ReportCommandEventHandler(reportCommandService_CommandChanged);
					reportCommandService = null;
				}
				UnsubscribeBarManagerEvents();
				ClearContent();
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~XRDesignItemsLogicBase() {
			Dispose(false);
		}
		#endregion
		protected XRDesignItemsLogicBase(BarManager manager, IServiceProvider serviceProvider) {
			this.manager = manager;
			SubscribeBarManagerEvents();
			this.rootfontService = new RootToolBarFontService(this);
			this.serviceProvider = serviceProvider;
		}
		internal void EndInit() {
			EndInitCore();
			UpdateBarItems();
		}
		protected virtual void EndInitCore() {
			if(!Manager.IsDesignMode) {
				InitZoomItem();
				InitFontControls();
			}
			InitColorItems();
		}
		protected abstract void InitZoomItem();
		internal BarItem[] GetBarItemsByReportCommand(ReportCommand command) {
			return GetBarItemsByReportCommand(manager, command);
		}
		internal void ClearContent() {
			if(xrDesignPanel != null)
				UnsubscribeDesignPanelEvents();
		}
		void UpdateBarItemByCommand(ReportCommand command) {
			BarItem[] items = GetBarItemsByReportCommand(command);
			foreach(BarItem item in items) {
				SetBarItemData(item, command);
			}
		}
		internal void UpdateBarItems() {
			foreach(BarItem barItem in manager.Items) {
				ReportCommand command = ReportCommand.None;
				if(barItem is ISupportReportCommand)
					command = ((ISupportReportCommand)barItem).Command;
				if(command != ReportCommand.None) {
					SetBarItemData(barItem, command);
				}
			}
		}
		void SetBarItemData(BarItem barItem, ReportCommand command) {
			if(xrDesignPanel != null) {
				CommandVisibility visibility = xrDesignPanel.GetCommandVisibility(command);
				barItem.Visibility = ToBarItemVisibility(visibility);
				barItem.Enabled = xrDesignPanel.GetCommandEnabled(command);
			} else if(ReportCommandService != null) {
				CommandVisibility visibility = ReportCommandService.GetCommandVisibility(command);
				barItem.Visibility = ToBarItemVisibility(visibility);
				barItem.Enabled = ReportCommandService.CanHandleCommand(command);
			} else {
				barItem.Visibility = ToBarItemVisibility(CommandVisibility.All);
				barItem.Enabled = false;
			}
			if(Array.IndexOf<ReportCommand>(checkedCommands, command) >= 0) {
				ReportCommandService reportCommandService = GetService(typeof(ReportCommandService)) as ReportCommandService;
				if(reportCommandService != null)
					SetButtonDown(barItem, reportCommandService.GetCommandChecked(command));
			}
		}
		static void SetButtonDown(BarItem item, bool down) {
			BarButtonItem barButtonItem = item as BarButtonItem;
			if(barButtonItem != null) {
				barButtonItem.ButtonStyle = BarButtonStyle.Check;
				barButtonItem.Down = down;
			}
		}
		static BarItemVisibility ToBarItemVisibility(CommandVisibility visibility) {
			return (visibility & CommandVisibility.Toolbar) > 0 ? BarItemVisibility.Always : BarItemVisibility.Never;
		}
		void OnManagerItemClick(object sender, ItemClickEventArgs e) {
			if(e.Item is ISupportReportCommand) {
				ReportCommand command = ((ISupportReportCommand)e.Item).Command;
				ExecuteCommand(command);
			}
		}
		void ExecuteCommand(ReportCommand command) {
			if(IsColorPopupCommand(command))
				return;
			if(xrDesignPanel != null)
				xrDesignPanel.ExecCommand(command);
			else if(ReportCommandService != null)
				ReportCommandService.HandleCommand(command, null);
		}
		protected object GetService(Type type) {
			if(xrDesignPanel != null)
				return xrDesignPanel.GetService(type);
			return null;
		}
		protected virtual void InitFontControls() {
			XtraEditors.FontServiceBase.FillRepositoryItemComboBox(FontSizeBox, FormattingCommands.FontSizeSet);
			XtraEditors.FontServiceBase.FillRepositoryItemComboBox(FontNameBox, XtraEditors.FontServiceBase.GetFontFamiliesNames(false));
		}
		void InitColorItems() {
			XRLabel control = new XRLabel();
			SetDefaultItemColorAndLogic(ReportCommand.ForeColor, control.ForeColor);
			SetDefaultItemColorAndLogic(ReportCommand.BackColor, control.BackColor);
			control.Dispose();
		}
		void SetDefaultItemColorAndLogic(ReportCommand command, Color color) {
			BarItem[] items = GetBarItemsByReportCommand(command);
			foreach(BarItem item in items) {
				if(item is CommandColorBarItem) {
					((CommandColorBarItem)item).ColorPopupControlContainer.ResultColor = color;
					((CommandColorBarItem)item).DesignItemsLogic = this;
				}
			}
		}
		void OnReportStateChanged(object sender, ReportStateEventArgs e) {
			if(e.ReportState == ReportState.Opened || e.ReportState == ReportState.None)
				UpdateBarItems();
		}
		void OnDesignPanelDisposed(object sender, EventArgs e) {
			XRDesignPanel = null;
		}
		protected virtual void xrDesignPanel_SelectedTabIndexChanged(object sender, EventArgs e) {
		}
		protected virtual void XRDesignPanel_Activated(object sender, EventArgs e) {
			InitZoomItem();
			xrDesignPanel.SelectedToolboxItemUsed += xrDesignPanel_SelectedToolboxItemUsed;
		}
		protected virtual void OnDesignerDeactivated(object sender, EventArgs e) {
			UnsubscribeSelectionServiceEvents();
			UpdateBarItems();
			xrDesignPanel.SelectedToolboxItemUsed -= xrDesignPanel_SelectedToolboxItemUsed;
		}
		void OnDesignerHostLoadComplete(object sender, EventArgs e) {
			SubscribeSelectionServiceEvents();
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			if(designerHost != null) {
				FontServiceBase fontService = (FontServiceBase)GetService(typeof(FontServiceBase));
				if(fontService != null) {
					designerHost.RemoveService(typeof(FontServiceBase));
					fontService.Dispose();
				}
				designerHost.AddService(typeof(FontServiceBase), new ToolBarFontService(xrDesignPanel, rootfontService)); 
			}
			InitZoomItem();
		}
		void reportCommandService_CommandChanged(object sender, ReportCommandEventArgs e) {
			if(xrDesignPanel == null)
				UpdateBarItemByCommand(e.Command);
		}
		void OnCommandChanged(object sender, ReportCommandEventArgs e) {
			UpdateBarItemByCommand(e.Command);
		}
		void OnSelectionChanging(object sender, EventArgs e) {
			manager.BeginUpdate();
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			manager.EndUpdate();
			ISelectionService selelectionService = xrDesignPanel.GetService(typeof(ISelectionService)) as ISelectionService;
			if(selelectionService != null)
				UpdateColorControls(selelectionService.PrimarySelection as XRControl);
		}
		void OnHighlightedLinkChanged(object sender, HighlightedLinkChangedEventArgs e) {
			if(hintStaticItem == null)
				return;
			hintStaticItem.Caption = "";
			if(e.Link != null && !String.IsNullOrEmpty(e.Link.Item.Hint)) {
				hintStaticItem.Caption = e.Link.Item.Hint;
			}
		}
		void OnEnter(object sender, EventArgs e) {
			SetContextCommandsEnabled(true);
		}
		void OnLeave(object sender, EventArgs e) {
			SetContextCommandsEnabled(false);
		}
		void SetContextCommandsEnabled(bool enabled) {
			foreach(ReportCommand command in contextCommands) {
				foreach(BarItem item in GetBarItemsByReportCommand(command)) {
					if(item.ItemShortcut.IsExist)
						item.Enabled = enabled && xrDesignPanel.GetCommandEnabled(command);
				}
			}
		}
		void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			XRControl control = e.Component as XRControl;
			if(control != null && e.Member != null &&
				(e.Member.Name.Equals(XRComponentDesigner.GetStylePropertyName(XRComponentPropertyNames.ForeColor)) ||
				e.Member.Name.Equals(XRComponentDesigner.GetStylePropertyName(XRComponentPropertyNames.BackColor)))
			)
				UpdateColorControls(control);
		}
		void SubscribeDesignPanelEvents() {
			xrDesignPanel.Activated += XRDesignPanel_Activated;
			xrDesignPanel.SelectedTabIndexChanged += xrDesignPanel_SelectedTabIndexChanged;
			xrDesignPanel.ReportStateChanged += new ReportStateEventHandler(OnReportStateChanged);
			xrDesignPanel.Disposed += new EventHandler(OnDesignPanelDisposed);
			xrDesignPanel.Deactivated += new EventHandler(OnDesignerDeactivated);
			xrDesignPanel.LoadComplete += new EventHandler(OnDesignerHostLoadComplete);
			xrDesignPanel.CommandChanged += new ReportCommandEventHandler(OnCommandChanged);
			xrDesignPanel.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
			xrDesignPanel.Enter += new EventHandler(OnEnter);
			xrDesignPanel.Leave += new EventHandler(OnLeave);
			SetContextCommandsEnabled(xrDesignPanel.ContainsFocus);
		}
		void UnsubscribeDesignPanelEvents() {
			xrDesignPanel.Activated -= XRDesignPanel_Activated;
			xrDesignPanel.SelectedTabIndexChanged -= xrDesignPanel_SelectedTabIndexChanged;
			xrDesignPanel.ReportStateChanged -= new ReportStateEventHandler(OnReportStateChanged);
			xrDesignPanel.Disposed -= new EventHandler(OnDesignPanelDisposed);
			xrDesignPanel.Deactivated -= new EventHandler(OnDesignerDeactivated);
			xrDesignPanel.LoadComplete -= new EventHandler(OnDesignerHostLoadComplete);
			xrDesignPanel.CommandChanged -= new ReportCommandEventHandler(OnCommandChanged);
			xrDesignPanel.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
			xrDesignPanel.Enter -= new EventHandler(OnEnter);
			xrDesignPanel.Leave -= new EventHandler(OnLeave);
			UnsubscribeSelectionServiceEvents();
		}
		void SubscribeSelectionServiceEvents() {
			ISelectionService selSvc = xrDesignPanel.GetService(typeof(ISelectionService)) as ISelectionService;
			if(selSvc == null) return;
			selSvc.SelectionChanging += new EventHandler(OnSelectionChanging);
			selSvc.SelectionChanged += new EventHandler(OnSelectionChanged);
		}
		void UnsubscribeSelectionServiceEvents() {
			ISelectionService selSvc = xrDesignPanel.GetService(typeof(ISelectionService)) as ISelectionService;
			if(selSvc == null) return;
			selSvc.SelectionChanging -= new EventHandler(OnSelectionChanging);
			selSvc.SelectionChanged -= new EventHandler(OnSelectionChanged);
		}
		void SubscribeBarManagerEvents() {
			manager.PressedLinkChanged += Manager_PressedLinkChanged;
			manager.ItemDoubleClick += Manager_ItemDoubleClick;
			manager.ItemClick += new ItemClickEventHandler(OnManagerItemClick);
			manager.HighlightedLinkChanged += new HighlightedLinkChangedEventHandler(OnHighlightedLinkChanged);
		}
		void UnsubscribeBarManagerEvents() {
			manager.PressedLinkChanged -= Manager_PressedLinkChanged;
			manager.ItemDoubleClick -= Manager_ItemDoubleClick;
			manager.ItemClick -= new ItemClickEventHandler(OnManagerItemClick);
			manager.HighlightedLinkChanged -= new HighlightedLinkChangedEventHandler(OnHighlightedLinkChanged);
		}
		void UpdateColorControls(XRControl control) {
			if(control == null) return;
			UpdateColorControl(control, ReportCommand.ForeColor, control.GetEffectiveForeColor());
			UpdateColorControl(control, ReportCommand.BackColor, control.GetEffectiveBackColor());
		}
		void UpdateColorControl(XRControl control, ReportCommand command, Color color) {
			BarItem[] items = GetBarItemsByReportCommand(command);
			foreach(BarItem item in items) {
				CommandColorBarItem colorItem = item as CommandColorBarItem;
				if(colorItem != null && colorItem.ColorPopupControlContainer != null && colorItem.ColorPopupControlContainer.ResultColor != color)
					colorItem.ColorPopupControlContainer.ResultColor = color;
			}
		}
		protected virtual void Manager_PressedLinkChanged(object sender, HighlightedLinkChangedEventArgs e) { }
		protected abstract void PressCursorBarItem();
		protected void Manager_ItemDoubleClick(object sender, ItemClickEventArgs e) {
			ToolboxItem item = e.Item.Tag as ToolboxItem;
			if(item == null) {
				PressCursorBarItem();
				return;
			}
			if(XRDesignPanel != null)
				XRDesignPanel.ToolPicked(item);
		}
		void xrDesignPanel_SelectedToolboxItemUsed(object sender, EventArgs e) {
			PressCursorBarItem();
			if(ToolboxService != null)
				ToolboxService.SetSelectedToolboxItem(null);
		}
	}
}
