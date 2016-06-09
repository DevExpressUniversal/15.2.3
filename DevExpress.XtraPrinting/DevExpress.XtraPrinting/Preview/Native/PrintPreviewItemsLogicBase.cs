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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Control;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Controls;
using System.Reflection;
using System.Collections;
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Control.Native;
namespace DevExpress.XtraPrinting.Preview.Native {
	public abstract class PreviewItemsLogicBase : IDisposable {
		#region inner classes
		protected class PrintPreviewPopupMenu : PopupMenu {
			protected PreviewItemsLogicBase logic;
			public PrintPreviewPopupMenu(PreviewItemsLogicBase logic, int groupIndex, BarItem[] buttons, IContainer container)
				: base(logic.Manager) {
				container.Add(this);
				this.logic = logic;
				bool beginGroup = false;
				foreach(BarItem button in buttons) {
					if(button == null) {
						beginGroup = true;
						continue;
					}
					button.ItemClick += new ItemClickEventHandler(OnItemClick);
					if(button is BarBaseButtonItem)
						((BarBaseButtonItem)button).GroupIndex = groupIndex;
					ItemLinks.Add(button, beginGroup);
					beginGroup = false;
				}
			}
			protected override void Dispose(bool disposing) {
				foreach(BarItemLink barItemLink in ItemLinks) {
					barItemLink.Item.ItemClick -= new ItemClickEventHandler(OnItemClick);
				}
				base.Dispose(disposing);
			}
			void OnItemClick(object sender, ItemClickEventArgs e) {
				logic.ExecCommand(e.Item as ISupportPrintingSystemCommand);
			}
		}
		class PrintPreviewItemCommandHandler : ICommandHandler, IDisposable {
			protected PrintingSystemCommand handledCommand;
			PopupControl popupControl;
			public PrintPreviewItemCommandHandler(PopupControl popupControl) {
				handledCommand = PrintingSystemCommand.None;
				this.popupControl = popupControl;
				if(popupControl != null) {
					popupControl.CloseUp += new EventHandler(OnCloseUp);
					popupControl.Popup += new EventHandler(OnPopup);
				}
			}
			public bool CanHandleCommand(PrintingSystemCommand command, IPrintControl printControl) {
				return handledCommand == command;
			}
			public virtual void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl, ref bool handled) {
				if(args.Length != 1 || (args[0] as PrintPreviewBarItem) == null) {
					handled = false;
					return;
				}
				handled = true;
			}
			public void Dispose() {
				if(popupControl != null) {
					popupControl.CloseUp -= new EventHandler(OnCloseUp);
					popupControl.Popup -= new EventHandler(OnPopup);
					popupControl = null;
				}
			}
			protected virtual void OnCloseUp(object sender, EventArgs e) {
			}
			protected virtual void OnPopup(object sender, EventArgs e) {
			}
		}
		class MultiplePagesCommandHandler : PrintPreviewItemCommandHandler {
			public MultiplePagesCommandHandler(PopupControl popupControl)
				: base(popupControl) {
				handledCommand = PrintingSystemCommand.MultiplePages;
			}
			protected override void OnCloseUp(object sender, EventArgs e) {
				base.OnCloseUp(sender, e);
				MultiplePagesControlContainer container = sender as MultiplePagesControlContainer;
				if(container.Commited) {
					container.PrintControl.SetPageView(container.SelectedColumns, container.SelectedRows);
				}
			}
		}
		class FillBackgroundCommandHandler : PrintPreviewItemCommandHandler {
			public FillBackgroundCommandHandler(PopupControl popupControl)
				: base(popupControl) {
				handledCommand = PrintingSystemCommand.FillBackground;
			}
			protected override void OnCloseUp(object sender, EventArgs e) {
				base.OnCloseUp(sender, e);
				ColorPopupControlContainer container = sender as ColorPopupControlContainer;
				container.PrintControl.PrintingSystem.Graph.PageBackColor = container.ResultColor;
			}
			protected override void OnPopup(object sender, EventArgs e) {
				ColorPopupControlContainer container = sender as ColorPopupControlContainer;
				container.ResultColor = container.PrintControl.PrintingSystem.Graph.PageBackColor;
				base.OnPopup(sender, e);
			}
		}
		class ScaleCommandHandler : PrintPreviewItemCommandHandler {
			public ScaleCommandHandler(PopupControl popupControl)
				: base(popupControl) {
				handledCommand = PrintingSystemCommand.Scale;
			}
			protected override void OnCloseUp(object sender, EventArgs e) {
				base.OnCloseUp(sender, e);
				ScaleControlContainer container = sender as ScaleControlContainer;
				if(container.Committed) {
					if(container.FitToPageWidth) {
						container.Document.AutoFitToPagesWidth = container.FitToWidthPagesCount;
					} else {
						container.Document.ScaleFactor = container.ScaleFactor;
					}
				}
			}
		}
		class DocumentMapCommandHandler : PrintPreviewItemCommandHandler {
			PreviewItemsLogicBase logic;
			public DocumentMapCommandHandler(PreviewItemsLogicBase logic)
				: base(null) {
				this.logic = logic;
				handledCommand = PrintingSystemCommand.DocumentMap;
			}
			public override void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl, ref bool handled) {
				logic.ButtonDocumentMap.Down = (bool)args[0];
			}
		}
		class ThumbnailsCommandHandler : PrintPreviewItemCommandHandler {
			PreviewItemsLogicBase logic;
			public ThumbnailsCommandHandler(PreviewItemsLogicBase logic)
				: base(null) {
				this.logic = logic;
				handledCommand = PrintingSystemCommand.Thumbnails;
			}
			public override void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl, ref bool handled) {
				logic.ButtonThumbnails.Down = (bool)args[0];
			}
		}
		class FindCommandHandler : PrintPreviewItemCommandHandler {
			PreviewItemsLogicBase logic;
			public FindCommandHandler(PreviewItemsLogicBase logic)
				: base(null) {
				this.logic = logic;
				handledCommand = PrintingSystemCommand.Find;
			}
			public override void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl, ref bool handled) {
				logic.ButtonFind.Down = (bool)args[0];
			}
		}
		class ParametersCommandHandler : PrintPreviewItemCommandHandler {
			PreviewItemsLogicBase logic;
			public ParametersCommandHandler(PreviewItemsLogicBase logic)
				: base(null) {
				this.logic = logic;
				handledCommand = PrintingSystemCommand.Parameters;
			}
			public override void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl, ref bool handled) {
				logic.ButtonParameters.Down = (bool)args[0];
			}
		}
		#endregion inner classes
		#region static
		internal const string ZoomStringFormat = "{0:###}%";
		static CommandSet defaultCommandSet;
		static PreviewItemsLogicBase() {
			defaultCommandSet = new CommandSet();
		}
		public static BarItem GetBarItemByCommand(IEnumerable items, PrintingSystemCommand command) {
			foreach(BarItem item in items) {
				if(item is ISupportPrintingSystemCommand && ((ISupportPrintingSystemCommand)item).Command == command)
					return item;
			}
			return null;
		}
		static BarItem GetBarItemByStatusPanelID(IEnumerable items, StatusPanelID statusPanelID) {
			foreach(BarItem item in items) {
				if(item is IStatusPanel && ((IStatusPanel)item).StatusPanelID == statusPanelID)
					return item;
			}
			return null;
		}
		public static BarItem GetBarItemByStatusPanelID(BarManager manager, StatusPanelID statusPanelID) {
			return GetBarItemByStatusPanelID(manager.Items, statusPanelID);
		}
		internal static BarItem GetBarItemByCommand(BarManager manager, PrintingSystemCommand command, object contextSpecifier) {
			return GetBarItemByCommand(ContextHelper.GetSameContextEnumerable<BarItem>(manager.Items, contextSpecifier), command);
		}
		internal static string ZoomFactorToString(float zoomFactor) {
			return String.Format(ZoomStringFormat, zoomFactor * 100);
		}
		#endregion static
		bool isInitialized;
		PopupControl exportMenu;
		PopupControl sendMenu;
		PopupMenu popupContextMenu;
		BarManager manager;
		PrintControl printControl;
		List<PrintPreviewItemCommandHandler> commandHandlers;
		const int popupControlContainersCount = 3;
		IPrintPreviewPopupControlContainer[] popupControlContainers = new IPrintPreviewPopupControlContainer[popupControlContainersCount];
		PrintPreviewStaticItem dummyBarItem = new PrintPreviewStaticItem();
		protected PrintPreviewBarItem dummyItem = new PrintPreviewBarItem();
		List<BarItem> lockedItems = new List<BarItem>();
		protected IContainer components;
		#region properties
		internal PrintingSystemBase PrintingSystem {
			get { return printControl != null ? printControl.PrintingSystem : null; }
		}
		internal PrintingSystemCommand DefaultExportFormat {
			get {
				return PrintingSystem != null ? PrintingSystem.ExportOptions.PrintPreview.DefaultExportFormat : PrintingSystemCommand.ExportPdf;
			}
			set {
				PrintingSystem.ExportOptions.PrintPreview.DefaultExportFormat = value;
			}
		}
		internal PrintingSystemCommand DefaultSendFormat {
			get {
				return PrintingSystem != null ? PrintingSystem.ExportOptions.PrintPreview.DefaultSendFormat : PrintingSystemCommand.SendPdf;
			}
			set {
				PrintingSystem.ExportOptions.PrintPreview.DefaultSendFormat = value;
			}
		}
		protected BarManager Manager { get { return manager; } }
		public PrintControl PrintControl {
			get { return printControl; }
			set {
				if(printControl != null) {
					if(printControl.ProgressReflector != null)
						printControl.ProgressReflector.Reset();
					UnsubscribePrintControlEvents();
					printControl.ProgressReflector = null;
				}
				printControl = value;
				InitZoomItems();
				if(printControl != null) {
					SubscribePrintControlEvents();
					InitializeProgressReflector();
				}
				UpdateCommands();
				UpdatePreviewStatusPanels();
				UpdateContainerControls();
			}
		}
		protected virtual void InitializeProgressReflector() {
			printControl.ProgressReflector = new ReflectorBarItem(manager);
		}
		protected internal abstract ZoomBarEditItem ZoomItem { get; }
		ZoomTrackBarEditItem ZoomTrackItem {
			get {
				ZoomTrackBarEditItem zoomTrackBarEditItem = (ZoomTrackBarEditItem)GetBarItemByCommand(manager.Items, PrintingSystemCommand.ZoomTrackBar);
				if(zoomTrackBarEditItem != null && zoomTrackBarEditItem.RepositoryItem != null)
					zoomTrackBarEditItem.RepositoryItem.AllowKeyboardNavigation = false;
				return zoomTrackBarEditItem;
			}
		}
		PrintPreviewBarItem ButtonPointer { get { return GetButtonByCommand(PrintingSystemCommand.Pointer); } }
		protected PrintPreviewBarItem ButtonHandTool { get { return GetButtonByCommand(PrintingSystemCommand.HandTool); } }
		protected PrintPreviewBarItem ButtonMagnifier { get { return GetButtonByCommand(PrintingSystemCommand.Magnifier); } }
		PrintPreviewBarItem ButtonDocumentMap { get { return GetButtonByCommand(PrintingSystemCommand.DocumentMap); } }
		PrintPreviewBarItem ButtonThumbnails { get { return GetButtonByCommand(PrintingSystemCommand.Thumbnails); } }
		PrintPreviewBarItem ButtonParameters { get { return GetButtonByCommand(PrintingSystemCommand.Parameters); } }
		PrintPreviewBarItem ButtonExportDocument { get { return GetButtonByCommand(PrintingSystemCommand.ExportFile); } }
		PrintPreviewBarItem ButtonSendDocument { get { return GetButtonByCommand(PrintingSystemCommand.SendFile); } }
		PrintPreviewBarItem ButtonFind { get { return GetButtonByCommand(PrintingSystemCommand.Find); } }
		PrintPreviewSubItem PageLayoutItem { get { return GetBarItemByCommand(PrintingSystemCommand.PageLayout) as PrintPreviewSubItem; } }
		PrintPreviewStaticItem PanelPageOfPages { get { return GetPanelByID(StatusPanelID.PageOfPages); } }
		PrintPreviewStaticItem PanelZoomFactorText { get { return GetPanelByID(StatusPanelID.ZoomFactorText); } }
#if DEBUGTEST
		internal PrintPreviewStaticItem PanelZoomFactor { get { return GetPanelByID(StatusPanelID.ZoomFactor); } }
#else
		PrintPreviewStaticItem PanelZoomFactor { get { return GetPanelByID(StatusPanelID.ZoomFactor); } }
#endif
		protected internal CommandSet CommandSet { get { return PrintControl != null ? PrintControl.CommandSet : defaultCommandSet; } }
		#endregion properties
		protected PreviewItemsLogicBase(BarManager manager) {
			components = new Container();
			commandHandlers = new List<PrintPreviewItemCommandHandler>();
			this.manager = manager;
			SubscribeBarManagerEvents();
		}
		public void SetItemLocked(BarItem item, bool locked) {
			if(locked && !lockedItems.Contains(item))
				lockedItems.Add(item);
			else if(!locked)
				lockedItems.Remove(item);
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(PageLayoutItem != null) {
					PageLayoutItem.Popup -= new EventHandler(OnPageLayoutItemPopup);
				}
				if(printControl != null) {
					UnsubscribePrintControlEvents();
					printControl = null;
				}
				if(popupContextMenu != null) {
					popupContextMenu.Dispose();
					popupContextMenu = null;
				}
				if(dummyBarItem != null) {
					dummyBarItem.Dispose();
					dummyBarItem  = null;
				}
				if(dummyItem != null) {
					dummyItem.Dispose();
					dummyItem  = null;
				}
				foreach(PrintPreviewItemCommandHandler commandHandler in commandHandlers)
					commandHandler.Dispose();
				commandHandlers.Clear();
				UnsubscribeBarManagerEvents();
				DisposeControlContainers();
				if(components != null) {
					components.Dispose();
					components = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~PreviewItemsLogicBase() {
			Dispose(false);
		}
		#endregion
		CommandSetItem[] GetCommands() {
			return CommandSet.GetAllCommands();
		}
		public virtual BarItem GetBarItemByCommand(PrintingSystemCommand command) {
			BarItem item = GetBarItemByCommand(manager.Items, command);
			return item != null ? item : dummyItem;
		}
		PrintPreviewBarItem GetButtonByCommand(PrintingSystemCommand command) {
			PrintPreviewBarItem item = GetBarItemByCommand(command) as PrintPreviewBarItem;
			if(item != null)
				return item;
			return dummyItem;
		}
		void OnPageLayoutItemPopup(object sender, EventArgs e) {
			if(PrintControl == null)
				return;
			BarButtonItem itemContinuous = GetButtonByCommand(PrintingSystemCommand.PageLayoutContinuous);
			if(itemContinuous != null)
				itemContinuous.Down = PrintControl.Continuous;
			BarButtonItem itemFacing = GetButtonByCommand(PrintingSystemCommand.PageLayoutFacing);
			if(itemFacing != null)
				itemFacing.Down = !PrintControl.Continuous;
		}
		internal void ExecCommand(ISupportPrintingSystemCommand commandItem) {
			DefaultExportFormat = commandItem.Command;
			DefaultSendFormat = commandItem.Command;
			if(commandItem != null) {
				object arg = commandItem is ISupportParametrizedPrintingSystemCommand ? ((ISupportParametrizedPrintingSystemCommand)commandItem).CommandParameter : null;
				ExecCommand(commandItem.Command, arg);
			}
		}
		internal void ExecCommand(PrintingSystemCommand command, object arg) {
			if(printControl != null && IsCommandEnabled(command))
				printControl.ExecCommand(command, new object[] { arg });
		}
		internal void EndInit() {
			if(isInitialized)
				return;
			InitDropDownMenus();
			InitZoomItems();
			InitPageLayoutItem();
			if(!Manager.IsDesignMode)
				InitPopupControlContainers();
			isInitialized = true;
		}
		void InitDropDownMenus() {
#if DEBUGTEST
			if(sendMenu != null && exportMenu != null)
				throw new Exception("Repeated initialization");
#endif
			sendMenu = CreateDropDownMenu(ButtonSendDocument, PSCommandHelper.SendCommands, PrintingSystemCommand.SendFile);
			exportMenu = CreateDropDownMenu(ButtonExportDocument, PSCommandHelper.ExportCommands, PrintingSystemCommand.ExportFile);
		}
		PopupControl CreateDropDownMenu(PrintPreviewBarItem item, PrintingSystemCommand[] commands, PrintingSystemCommand parentCommand) {
			if(item != null && item != dummyItem) {
				item.DropDownControl = CreateExportMenuPopupControl(commands, parentCommand);
				PopupControl exportMenu = item.DropDownControl;
				System.Diagnostics.Debug.Assert(exportMenu != null);
				return exportMenu;
			}
			return null;
		}
		protected abstract PopupControl CreateExportMenuPopupControl(PrintingSystemCommand[] commands, PrintingSystemCommand parentCommand);
		void InitZoomItems() {
			if(ZoomItem != null)
				ZoomItem.Init(printControl);
			if(ZoomTrackItem != null && printControl != null) {
				ZoomTrackItem.CommandExecuter = printControl.ExecCommand;
				ZoomTrackItem.ApplyZoom(printControl.Zoom);
			}
		}
		void InitPageLayoutItem() {
			if(PageLayoutItem != null) {
				PageLayoutItem.Popup += new EventHandler(OnPageLayoutItemPopup);
			}
		}
		void HandleItemClick(PrintPreviewBarItem barItem) {
			PrintingSystemCommand command = barItem.Command;
			switch(command) {
				case PrintingSystemCommand.Print:
				case PrintingSystemCommand.PageSetup:
				case PrintingSystemCommand.Customize:
				case PrintingSystemCommand.EditPageHF:
				printControl.ExecCommand(command);
				barItem.Down = false;
				break;
				case PrintingSystemCommand.SendFile:
				printControl.ExecCommand(DefaultSendFormat);
				break;
				case PrintingSystemCommand.ExportFile:
				printControl.ExecCommand(DefaultExportFormat);
				break;
				case PrintingSystemCommand.DocumentMap:
				case PrintingSystemCommand.Parameters:
				case PrintingSystemCommand.Thumbnails:
				case PrintingSystemCommand.Find:
				printControl.ExecCommand(command, new object[] { barItem.Down });
				break;
				case PrintingSystemCommand.Pointer:
				case PrintingSystemCommand.HandTool:
				case PrintingSystemCommand.Magnifier:
				HandleToolCommand(command, barItem.Down);
				break;
				case PrintingSystemCommand.MultiplePages:
				case PrintingSystemCommand.FillBackground:
				case PrintingSystemCommand.Scale:
				case PrintingSystemCommand.PaperSize:
				case PrintingSystemCommand.PageOrientation:
				case PrintingSystemCommand.PageMargins:
				case PrintingSystemCommand.Zoom:
				break;
				default:
				printControl.ExecCommand(command);
				break;
			}
		}
		protected abstract void HandleToolCommand(PrintingSystemCommand command, bool down);
		void HandleItemPress(PrintPreviewBarItem barItem) {
			if(barItem.DropDownControl is IPrintPreviewPopupControlContainer)
				printControl.ExecCommand(barItem.Command, new object[] { barItem });
		}
		void SubscribeBarManagerEvents() {
			manager.ItemClick += new ItemClickEventHandler(bm_ItemClick);
			manager.ItemPress += new ItemClickEventHandler(bm_ItemPress);
		}
		void UnsubscribeBarManagerEvents() {
			manager.ItemClick -= new ItemClickEventHandler(bm_ItemClick);
			manager.ItemPress -= new ItemClickEventHandler(bm_ItemPress);
		}
		void SubscribePrintControlEvents() {
			printControl.SelectedPageChanged += new EventHandler(pc_SelectedPageChanged);
			printControl.CommandChanged += new EventHandler(pc_CommandChanged);
			printControl.DocumentChanged += new EventHandler(pc_DocumentChanged);
			printControl.ZoomChanged += new EventHandler(pc_ZoomChanged);
			printControl.Disposed += new EventHandler(pc_Disposed);
			printControl.CommandExecute += new CommandExecuteEventHandler(pc_CommandExecute);
			printControl.MouseDown += new MouseEventHandler(pc_MouseDown);
			printControl.DockManagerCreated += new EventHandler(pc_DockManagerCreated);
			printControl.RightToLeftChanged += new EventHandler(pc_RightToLeftChanged);
		}
		void UnsubscribePrintControlEvents() {
			printControl.SelectedPageChanged -= new EventHandler(pc_SelectedPageChanged);
			printControl.CommandChanged -= new EventHandler(pc_CommandChanged);
			printControl.DocumentChanged -= new EventHandler(pc_DocumentChanged);
			printControl.ZoomChanged -= new EventHandler(pc_ZoomChanged);
			printControl.Disposed -= new EventHandler(pc_Disposed);
			printControl.CommandExecute -= new CommandExecuteEventHandler(pc_CommandExecute);
			printControl.MouseDown -= new MouseEventHandler(pc_MouseDown);
			printControl.DockManagerCreated -= new EventHandler(pc_DockManagerCreated);
			printControl.RightToLeftChanged -= new EventHandler(pc_RightToLeftChanged);
		}
		void bm_ItemClick(object sender, ItemClickEventArgs e) {
			if(e.Item is PrintPreviewBarItem && CanHandleEvent((PrintPreviewBarItem)e.Item))
				HandleItemClick((PrintPreviewBarItem)e.Item);
			if(e.Item is PrintPreviewStaticItem && ((PrintPreviewStaticItem)e.Item).StatusPanelID == StatusPanelID.PageOfPages && PrintControl != null)
				printControl.ExecCommand(PrintingSystemCommand.GoToPage);
		}
		void bm_ItemPress(object sender, ItemClickEventArgs e) {
			PrintPreviewBarItem barItem = e.Item as PrintPreviewBarItem;
			if(CanHandleEvent(barItem))
				HandleItemPress(barItem);
		}
		protected virtual bool CanHandleEvent(PrintPreviewBarItem barItem) {
			return barItem != null && PrintControl != null;
		}
		void pc_ZoomChanged(object sender, System.EventArgs e) {
			if(this.ZoomItem != null)
				this.ZoomItem.OnZoomChanged(printControl, EventArgs.Empty);
			if(this.ZoomTrackItem != null)
				this.ZoomTrackItem.ApplyZoom(printControl.Zoom);
			UpdatePreviewStatusPanels();
		}
		void pc_CommandChanged(object sender, System.EventArgs e) {
			UpdateCommands();
		}
		void pc_DocumentChanged(object sender, System.EventArgs e) {
			UpdatePreviewStatusPanels();
			UpdateCommands();
			if(printControl != null) {
				SetItemDown(ButtonDocumentMap, printControl.GetPanelVisibility(PrintingSystemCommand.DocumentMap));
				SetItemDown(ButtonParameters, printControl.GetPanelVisibility(PrintingSystemCommand.Parameters));
				SetItemDown(ButtonThumbnails, printControl.GetPanelVisibility(PrintingSystemCommand.Thumbnails));
			}
		}
		static void SetItemDown(PrintPreviewBarItem item, bool value) {
			if(item.Visibility == BarItemVisibility.Always)
				item.Down = value;
		}
		void pc_Disposed(object sender, System.EventArgs e) {
			PrintControl = null;
		}
		void pc_CommandExecute(object sender, CommandExecuteEventArgs e) {
			foreach(PrintPreviewItemCommandHandler commandHandler in commandHandlers)
				HandleCommand(commandHandler, e);
		}
		void pc_SelectedPageChanged(object sender, EventArgs e) {
			UpdatePreviewStatusPanels();
		}
		void pc_MouseDown(object sender, MouseEventArgs e) {
			if(e.Button == System.Windows.Forms.MouseButtons.Right && CommandSet.IsCommandEnabled(PrintingSystemCommand.Copy)) {
				if(popupContextMenu == null)
					popupContextMenu = CreatePopupMenu();
				if(popupContextMenu.ItemLinks.Count > 0)
					popupContextMenu.ShowPopup(System.Windows.Forms.Control.MousePosition);
			}
		}
		void pc_DockManagerCreated(object sender, EventArgs e) {
			if(!Manager.IsDesignMode 
				&& printControl != null 
				&& printControl.DockManager != null 
				&& printControl.DockManager.MenuManager == null
				)
				printControl.DockManager.MenuManager = Manager;
		}
		PopupMenu CreatePopupMenu() {
			PopupMenu popupMenu = new PopupMenu(manager);
			popupMenu.ClearLinks();
			popupMenu.BeginInit();
			try {
				PrintPreviewBarItem itemCopy = new PrintPreviewBarItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_Copy), PrintingSystemCommand.Copy);
				itemCopy.Glyph = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraPrinting.Images.Copy_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
				popupMenu.AddItem(itemCopy);
				PrintPreviewBarItem itemPrint = new PrintPreviewBarItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_PrintSelection), PrintingSystemCommand.PrintSelection);
				itemPrint.Glyph = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraPrinting.Images.Print_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
				popupMenu.AddItem(itemPrint);
			} finally {
				popupMenu.EndInit();
			}
			return popupMenu;
		}
		void HandleCommand(PrintPreviewItemCommandHandler handler, CommandExecuteEventArgs e) {
			if(handler.CanHandleCommand(e.Command, printControl)) {
				bool handled = false;
				handler.HandleCommand(e.Command, e.Args, printControl, ref handled);
				e.Handled = handled;
			}
		}
		public virtual void UpdateCommands() {
			CommandSetItem[] items = GetCommands();
			if(manager.Items.Count == 0)
				return;
			if(printControl != null) {
				ButtonHandTool.Down = printControl.HandTool;
				ButtonMagnifier.Down = printControl.AutoZoom;
				ButtonPointer.Down = !printControl.AutoZoom && !printControl.HandTool;
				ButtonDocumentMap.Down = printControl.DocumentMapVisible;
				ButtonThumbnails.Down = printControl.ThumbnailsVisible;
				ButtonFind.Down = printControl.FindPanelVisible;
			}
			foreach(CommandSetItem item in items) {
				UpdateBarItem(GetBarItemByCommand(item.Command), item);
			}
		}
		void UpdateBarItem(BarItem barItem, CommandSetItem commandSetItem) {
			if(barItem != null && !lockedItems.Contains(barItem)) {
				if(PrintingSystem != null)
					barItem.Visibility = GetBarItemVisibility(commandSetItem);
				ZoomTrackBarEditItem zoomItem = barItem as ZoomTrackBarEditItem;
				if(zoomItem != null && zoomItem.Locked)
					return;
				barItem.Enabled = barItem is PrintPreviewSubItem || IsCommandSetEnabled(commandSetItem);
			}
		}
		internal bool IsCommandEnabled(PrintingSystemCommand command) {
			return CommandSet[command] != null && IsCommadSetItemEnabled(CommandSet[command]);
		}
		bool IsCommandSetEnabled(CommandSetItem item) {
			bool enabled = IsCommadSetItemEnabled(item);
			return PrintingSystem == null || PrintControl == null ? enabled :
				(enabled &&
				(printControl.CanExecCommand(item.Command) ||
				item.Command == PrintingSystemCommand.SendFile ||
				item.Command == PrintingSystemCommand.ExportFile));
		}
		BarItemVisibility GetBarItemVisibility(CommandSetItem commandSetItem) {
			if(IsCommandVisible(commandSetItem))
				return BarItemVisibility.Always;
			else
				return BarItemVisibility.Never;
		}
		protected virtual bool IsCommadSetItemEnabled(CommandSetItem commandSetItem) {
			return commandSetItem.Enabled;
		}
		protected internal virtual bool IsCommandVisible(CommandSetItem commandSetItem) {
			return IsCommandVisible(commandSetItem.Command);
		}
		internal bool IsCommandVisible(PrintingSystemCommand command) {
			return CommandSet.GetCommandVisibility(command) != CommandVisibility.None;
		}
		internal void UpdatePreviewStatusPanels() {
			if(printControl != null && printControl.Document != null) {
				UpdatePreviewStatusPanels(printControl.Zoom, printControl.SelectedPageIndex + 1,
					printControl.Document.Pages.Count);
			} else
				UpdatePreviewStatusPanels(1, -1, 0);
		}
		void UpdatePreviewStatusPanels(float zoom, int pageNo, int pageCount) {
			SetPanelText(PanelZoomFactor, ZoomFactorToString(zoom), string.Empty);
			SetPanelText(PanelZoomFactorText, ZoomFactorToString(zoom), string.Empty);
			string pageOfPagesHint = IsCommandVisible(PrintingSystemCommand.GoToPage) && pageCount > 0 ? PreviewLocalizer.GetString(PreviewStringId.SB_PageOfPagesHint) : string.Empty;
			SetPanelText(PanelPageOfPages, GetPageOfPagesText(pageNo, pageCount), pageOfPagesHint);
		}
		static string GetPageOfPagesText(int pageNo, int pageCount) {
			return pageCount <= 0 ? PreviewLocalizer.GetString(PreviewStringId.SB_PageNone) :
				string.Format(PreviewLocalizer.GetString(PreviewStringId.SB_PageOfPages), Math.Max(0, pageNo), pageCount);
		}
		void SetPanelText(PrintPreviewStaticItem barItem, string text, string hint) {
			if(barItem != null && barItem.StatusPanelID != StatusPanelID.None && !lockedItems.Contains(barItem)) {
				string value = PrintPreviewStaticItemFactory.GetCaption(barItem) + text;
				if(barItem.Caption != value)
					barItem.Caption = value;
				barItem.Hint = hint;
			}
		}
		public static System.Windows.Forms.Control GetStatusControl(BarManager barManager) {
			RibbonBarManager ribbonBarManager = barManager as RibbonBarManager;
			if(ribbonBarManager != null) {
				RibbonControl ribbonControl = GetEffectiveRibbonControl(ribbonBarManager.Ribbon);
				return ribbonControl != null ? ribbonControl.StatusBar : null;
			}
			foreach(BarDockControl item in barManager.DockControls) {
				if(item.DockStyle == BarDockStyle.Bottom)
					return item;
			}
			return null;
		}
		static RibbonControl GetEffectiveRibbonControl(RibbonControl ribbonControl) {
			return ribbonControl != null && ribbonControl.MergeOwner != null ?
				ribbonControl.MergeOwner :
				ribbonControl;
		}
		internal PrintPreviewStaticItem GetPanelByID(StatusPanelID panelID) {
			foreach(BarItem barItem in manager.Items) {
				PrintPreviewStaticItem printPreviewBarItem = barItem as PrintPreviewStaticItem;
				if(printPreviewBarItem != null && printPreviewBarItem.StatusPanelID == panelID)
					return printPreviewBarItem;
			}
			return dummyBarItem;
		}
		protected virtual void InitPopupControlContainers() {
			MultiplePagesControlContainer multiplePagesControlContainer = new DevExpress.XtraPrinting.Preview.MultiplePagesControlContainer();
			multiplePagesControlContainer.Controls.Add(multiplePagesControlContainer.Panel);
			multiplePagesControlContainer.Size = new System.Drawing.Size(0, 0);
			AssignDropDownControl(PrintingSystemCommand.MultiplePages, multiplePagesControlContainer);
			ColorPopupControlContainer colorPopupControlContainer = new ColorPopupControlContainer();
			colorPopupControlContainer.Visible = false;
			colorPopupControlContainer.DrawColorRectangle = false;
			AssignDropDownControl(PrintingSystemCommand.FillBackground, colorPopupControlContainer);
			colorPopupControlContainer.Item = GetButtonByCommand(PrintingSystemCommand.FillBackground);
			ScaleControlContainer scaleControlContainer = new ScaleControlContainer();
			AssignDropDownControl(PrintingSystemCommand.Scale, scaleControlContainer);
			popupControlContainers[0] = multiplePagesControlContainer;
			popupControlContainers[1] = colorPopupControlContainer;
			popupControlContainers[2] = scaleControlContainer;
			BindManagerToPopupControlContainers();
			UpdateContainerControls();
			commandHandlers.AddRange(new PrintPreviewItemCommandHandler[] {
				new MultiplePagesCommandHandler(multiplePagesControlContainer),
				new FillBackgroundCommandHandler(colorPopupControlContainer),
				new ScaleCommandHandler(scaleControlContainer),
				new DocumentMapCommandHandler(this),
				new ParametersCommandHandler(this),
				new ThumbnailsCommandHandler(this),
				new FindCommandHandler(this),
			});
		}
		void BindManagerToPopupControlContainers() {
			foreach(PopupControlContainer container in popupControlContainers) {
				if(container != null)
					container.Manager = manager;
			}
		}
		void DisposeControlContainers() {
			foreach(PopupControlContainer container in popupControlContainers)
				if(container != null)
					container.Dispose();
		}
		protected void AssignDropDownControl(PrintingSystemCommand command, PopupControl container) {
			PrintPreviewBarItem item = GetButtonByCommand(command);
			if(item != null) {
				item.DropDownControl = container;
				item.ActAsDropDown = true;
			}
		}
		void UpdateContainerControls() {
			foreach(IPrintPreviewPopupControlContainer container in popupControlContainers)
				if(container != null)
					container.PrintControl = printControl;
		}
		void pc_RightToLeftChanged(object sender, EventArgs e) {
			foreach(PopupControlContainer container in popupControlContainers)
				if(container != null)
					container.RightToLeft = (sender as PrintControl).RightToLeft;
		}
	}
}
