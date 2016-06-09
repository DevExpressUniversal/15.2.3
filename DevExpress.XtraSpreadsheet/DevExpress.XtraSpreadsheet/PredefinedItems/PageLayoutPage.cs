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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.Office;
using DevExpress.Office.API.Internal;
using DevExpress.Office.Design.Internal;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetPageSetupItemBuilder
	public class SpreadsheetPageSetupItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem marginsSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.PageSetupMarginsCommandGroup);
			SpreadsheetCommandBarCheckItem checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PageSetupMarginsNormal);
			checkItem.CaptionDependOnUnits = true;
			marginsSubItem.AddBarItem(checkItem);
			checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PageSetupMarginsWide);
			checkItem.CaptionDependOnUnits = true;
			marginsSubItem.AddBarItem(checkItem);
			checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PageSetupMarginsNarrow);
			checkItem.CaptionDependOnUnits = true;
			marginsSubItem.AddBarItem(checkItem);
			SpreadsheetCommandBarButtonItem buttomItem = SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PageSetupCustomMargins);
			marginsSubItem.AddBarItem(buttomItem);
			items.Add(marginsSubItem);
			SpreadsheetCommandBarSubItem orientationSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.PageSetupOrientationCommandGroup);
			orientationSubItem.AddBarItem(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PageSetupOrientationPortrait));
			orientationSubItem.AddBarItem(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PageSetupOrientationLandscape));
			items.Add(orientationSubItem);
			items.Add(new PageSetupPaperKindItem());
			SpreadsheetCommandBarSubItem printAreaSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.PageSetupPrintAreaCommandGroup);
			printAreaSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PageSetupSetPrintArea));
			printAreaSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PageSetupClearPrintArea));
			printAreaSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PageSetupAddPrintArea));
			items.Add(printAreaSubItem);
		}
	}
	#endregion
	#region SpreadsheetPageSetupShowItemBuilder
	public class SpreadsheetPageSetupShowItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarCheckItem checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.ViewShowGridlines);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
			checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.ViewShowHeadings);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
		}
	}
	#endregion
	#region SpreadsheetPageSetupPrintItemBuilder
	public class SpreadsheetPageSetupPrintItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarCheckItem checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PageSetupPrintGridlines);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
			checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PageSetupPrintHeadings);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
		}
	}
	#endregion
	#region SpreadsheetArrangeItemBuilder
	public class SpreadsheetArrangeItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			PopulateArrangeItems(items);
		}
		public static void PopulateArrangeItems(List<BarItem> items) {
			SpreadsheetCommandBarSubItem bringForwardSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.ArrangeBringForwardCommandGroup);
			bringForwardSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ArrangeBringForward));
			bringForwardSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ArrangeBringToFront));
			items.Add(bringForwardSubItem);
			SpreadsheetCommandBarSubItem sendBackwardSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.ArrangeSendBackwardCommandGroup);
			sendBackwardSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ArrangeSendBackward));
			sendBackwardSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ArrangeSendToBack));
			items.Add(sendBackwardSubItem);
		}
	}
	#endregion
	#region SpreadsheetPageSetupBarCreator
	public class SpreadsheetPageSetupBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PageLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PageSetupRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(PageSetupBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new PageSetupBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPageSetupItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PageLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PageSetupRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetPageSetupShowBarCreator
	public class SpreadsheetPageSetupShowBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PageLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PageSetupShowRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(PageSetupShowBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 4; } }
		public override Bar CreateBar() {
			return new PageSetupShowBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPageSetupShowItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PageSetupShowRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetPageSetupPrintBarCreator
	public class SpreadsheetPageSetupPrintBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PageLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PageSetupPrintRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(PageSetupPrintBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 4; } }
		public override Bar CreateBar() {
			return new PageSetupPrintBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPageSetupPrintItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PageSetupPrintRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetArrangeBarCreator
	public class SpreadsheetArrangeBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PageLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ArrangeRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ArrangeBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new ArrangeBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetArrangeItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PageLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ArrangeRibbonPageGroup();
		}
	}
	#endregion
	#region PageLayoutRibbonPage
	public class PageLayoutRibbonPage : ControlCommandBasedRibbonPage {
		public PageLayoutRibbonPage() {
		}
		public PageLayoutRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PagePageLayout); } }
	}
	#endregion
	#region PageSetupBar
	public class PageSetupBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PageSetupBar() {
		}
		public PageSetupBar(BarManager manager)
			: base(manager) {
		}
		public PageSetupBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupPageSetup); } }
	}
	#endregion
	#region PageSetupShowBar
	public class PageSetupShowBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PageSetupShowBar() {
		}
		public PageSetupShowBar(BarManager manager)
			: base(manager) {
		}
		public PageSetupShowBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupShow); } }
	}
	#endregion
	#region PageSetupPrintBar
	public class PageSetupPrintBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PageSetupPrintBar() {
		}
		public PageSetupPrintBar(BarManager manager)
			: base(manager) {
		}
		public PageSetupPrintBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupPrint); } }
	}
	#endregion
	#region ArrangeBar
	public class ArrangeBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ArrangeBar() {
		}
		public ArrangeBar(BarManager manager)
			: base(manager) {
		}
		public ArrangeBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupArrange); } }
	}
	#endregion
	#region PageSetupRibbonPageGroup
	public class PageSetupRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PageSetupRibbonPageGroup() {
		}
		public PageSetupRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupPageSetup); } }
		public override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.PageSetup; } }
	}
	#endregion
	#region PageSetupShowRibbonPageGroup
	public class PageSetupShowRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PageSetupShowRibbonPageGroup() {
		}
		public PageSetupShowRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupShow); } }
	}
	#endregion
	#region PageSetupPrintRibbonPageGroup
	public class PageSetupPrintRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PageSetupPrintRibbonPageGroup() {
		}
		public PageSetupPrintRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupPrint); } }
		public override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.PageSetupSheet; } }
	}
	#endregion
	#region ArrangeRibbonPageGroup
	public class ArrangeRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ArrangeRibbonPageGroup() {
		}
		public ArrangeRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupArrange); } }
	}
	#endregion
	#region PageSetupPaperKindItem
	public class PageSetupPaperKindItem : SpreadsheetCommandBarButtonItem {
		PopupMenu popupMenu = new PopupMenu();
		public PageSetupPaperKindItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public PageSetupPaperKindItem(BarManager manager)
			: base(manager) {
		}
		public PageSetupPaperKindItem(string caption)
			: base(caption) {
		}
		public PageSetupPaperKindItem()
			: base() {
		}
		#region Properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsDropDown { get { return true; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl { get { return popupMenu; } set { } }
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.PageSetupPaperKindCommandGroup; } }
		#endregion
		protected virtual void PopulatePopupMenu() {
			if (this.popupMenu == null)
				return;
			IList<PaperKind> paperKindList = PageSetupSetPaperKindCommand.DefaultPaperKindList;
			this.popupMenu.BeginUpdate();
			try {
				foreach (PaperKind paperKind in paperKindList) {
					PageSetupSetPaperKindMenuItem item = new PageSetupSetPaperKindMenuItem(Control, paperKind);
					this.popupMenu.ItemLinks.Add(item);
				}
				this.popupMenu.ItemLinks.Add(new ShowPageSetupPaperSizesFormItem(Control));
			}
			finally {
				this.popupMenu.EndUpdate();
			}
		}
		protected virtual void RefreshPopupMenu() {
			DeletePopupItems();
			if (Control != null)
				PopulatePopupMenu();
		}
		protected virtual void DeletePopupItems() {
			if (this.popupMenu == null)
				return;
			BarItemLinkCollection itemLinks = this.popupMenu.ItemLinks;
			this.popupMenu.BeginUpdate();
			try {
				while (itemLinks.Count > 0)
					itemLinks[0].Item.Dispose();
			}
			finally {
				this.popupMenu.EndUpdate();
			}
		}
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			if (this.popupMenu != null)
				this.popupMenu.BeforePopup += OnBeforePopup;
		}
		protected override void UnsubscribeControlEvents() {
			if (this.popupMenu != null)
				this.popupMenu.BeforePopup -= OnBeforePopup;
			base.UnsubscribeControlEvents();
		}
		protected virtual void OnBeforePopup(object sender, CancelEventArgs e) {
			RefreshPopupMenu();
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if (this.popupMenu != null)
				this.popupMenu.Manager = this.Manager;
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				if (popupMenu != null) {
					DeletePopupItems();
					popupMenu.Dispose();
					popupMenu = null;
				}
			}
		}
		#endregion
	}
	#endregion
	#region PageSetupSetPaperKindMenuItem
	public class PageSetupSetPaperKindMenuItem : BarCheckItem {
		#region Fields
		readonly SpreadsheetControl control;
		readonly PaperKind paperKind;
		#endregion
		public PageSetupSetPaperKindMenuItem(SpreadsheetControl control, PaperKind paperKind) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.paperKind = paperKind;
			this.Caption = CreateCaption();
			PageSetupSetPaperKindCommand command = CreateCommand();
			ICommandUIState uiState = command.CreateDefaultCommandUIState();
			command.UpdateUIState(uiState);
			BarCheckItemUIState state = new BarCheckItemUIState(this);
			state.Visible = uiState.Visible;
			state.Enabled = uiState.Enabled;
			state.Checked = uiState.Checked;
			state.EditValue = uiState.EditValue;
		}
		protected override void OnClick(BarItemLink link) {
			PageSetupSetPaperKindCommand command = CreateCommand();
			command.Execute();
		}
		protected internal virtual PageSetupSetPaperKindCommand CreateCommand() {
			PageSetupSetPaperKindCommand command = new PageSetupSetPaperKindCommand(control, paperKind);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected internal virtual string CreateCaption() {
			Size paperSizeInTwips = PaperSizeCalculator.CalculatePaperSize(paperKind);
			DocumentModel documentModel = control.InnerControl.DocumentModel;
			DocumentUnit unit = control.InnerControl.Unit == DocumentUnit.Document ? DocumentUnit.Inch : control.InnerControl.Unit;
			UnitConverter unitConverter = documentModel.InternalAPI.UnitConverters[unit];
			UIUnit width = new UIUnit(unitConverter.FromUnits(documentModel.UnitConverter.TwipsToModelUnits(paperSizeInTwips.Width)), unit, UnitPrecisionDictionary.DefaultPrecisions);
			UIUnit height = new UIUnit(unitConverter.FromUnits(documentModel.UnitConverter.TwipsToModelUnits(paperSizeInTwips.Height)), unit, UnitPrecisionDictionary.DefaultPrecisions);
			return String.Format("{0}\r\n{1} x {2}", paperKind, width.ToString(), height.ToString());
		}
	}
	#endregion
	#region ShowPageSetupPaperSizesFormItem
	public class ShowPageSetupPaperSizesFormItem : BarButtonItem {
		#region Fields
		readonly SpreadsheetControl control;
		#endregion
		public ShowPageSetupPaperSizesFormItem(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			ShowPageSetupMorePaperSizesCommand command = CreateCommand();
			ICommandUIState uiState = command.CreateDefaultCommandUIState();
			command.UpdateUIState(uiState);
			BarButtonItemUIState state = new BarButtonItemUIState(this);
			state.Visible = uiState.Visible;
			state.Enabled = uiState.Enabled;
			state.Checked = uiState.Checked;
			state.EditValue = uiState.EditValue;
			this.Caption = XtraSpreadsheetLocalizer.GetString(command.MenuCaptionStringId);
		}
		protected override void OnClick(BarItemLink link) {
			ShowPageSetupMorePaperSizesCommand command = CreateCommand();
			command.Execute();
		}
		protected internal virtual ShowPageSetupMorePaperSizesCommand CreateCommand() {
			ShowPageSetupMorePaperSizesCommand command = new ShowPageSetupMorePaperSizesCommand(control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	#endregion
}
