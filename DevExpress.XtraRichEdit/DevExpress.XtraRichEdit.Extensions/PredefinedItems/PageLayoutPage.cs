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
using DevExpress.Office;
using DevExpress.Office.API.Internal;
using DevExpress.Office.UI;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Design.Internal;
using System.Resources;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditPageLayoutPageSetupItemBuilder
	public class RichEditPageLayoutPageSetupItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			ChangeSectionPageMarginsItem marginsItem = new ChangeSectionPageMarginsItem();
			items.Add(marginsItem);
			IBarSubItem marginsSubItem = marginsItem;
			marginsSubItem.AddBarItem(new SetNormalSectionPageMarginsItem());
			marginsSubItem.AddBarItem(new SetNarrowSectionPageMarginsItem());
			marginsSubItem.AddBarItem(new SetModerateSectionPageMarginsItem());
			marginsSubItem.AddBarItem(new SetWideSectionPageMarginsItem());
			marginsSubItem.AddBarItem(new ShowPageMarginsSetupFormItem());
			ChangeSectionPageOrientationItem orientationItem = new ChangeSectionPageOrientationItem();
			items.Add(orientationItem);
			IBarSubItem orientationSubItem = orientationItem;
			orientationSubItem.AddBarItem(new SetPortraitPageOrientationItem());
			orientationSubItem.AddBarItem(new SetLandscapePageOrientationItem());
			items.Add(new ChangeSectionPaperKindItem());
			ChangeSectionColumnsItem columnsItem = new ChangeSectionColumnsItem();
			items.Add(columnsItem);
			columnsItem.AddBarItem(new SetSectionOneColumnItem());
			columnsItem.AddBarItem(new SetSectionTwoColumnsItem());
			columnsItem.AddBarItem(new SetSectionThreeColumnsItem());
			columnsItem.AddBarItem(new ShowColumnsSetupFormItem());
			InsertBreakItem insertBreak = new InsertBreakItem();
			items.Add(insertBreak);
			insertBreak.AddBarItem(new InsertPageBreakItem());
			insertBreak.AddBarItem(new InsertColumnBreakItem());
			insertBreak.AddBarItem(new InsertSectionBreakNextPageItem());
			insertBreak.AddBarItem(new InsertSectionBreakEvenPageItem());
			insertBreak.AddBarItem(new InsertSectionBreakOddPageItem());
			ChangeSectionLineNumberingItem lineNumberingItem = new ChangeSectionLineNumberingItem();
			items.Add(lineNumberingItem);
			lineNumberingItem.AddBarItem(new SetSectionLineNumberingNoneItem());
			lineNumberingItem.AddBarItem(new SetSectionLineNumberingContinuousItem());
			lineNumberingItem.AddBarItem(new SetSectionLineNumberingRestartNewPageItem());
			lineNumberingItem.AddBarItem(new SetSectionLineNumberingRestartNewSectionItem());
			lineNumberingItem.AddBarItem(new ToggleParagraphSuppressLineNumbersItem());
			lineNumberingItem.AddBarItem(new ShowLineNumberingFormItem());
		}
	}
	#endregion
	#region ChangeSectionPageMarginsItem
	public class ChangeSectionPageMarginsItem : RichEditCommandBarSubItem {
		public ChangeSectionPageMarginsItem() {
		}
		public ChangeSectionPageMarginsItem(BarManager manager)
			: base(manager) {
		}
		public ChangeSectionPageMarginsItem(string caption)
			: base(caption) {
		}
		public ChangeSectionPageMarginsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeSectionPageMargins; } }
	}
	#endregion
	#region UnitDependentRichEditFormattingCheckItemBase (abstract class)
	public abstract class UnitDependentRichEditFormattingCheckItemBase : RichEditCommandBarCheckItem {
		protected UnitDependentRichEditFormattingCheckItemBase() {
		}
		protected UnitDependentRichEditFormattingCheckItemBase(string caption)
			: base(caption) {
		}
		protected UnitDependentRichEditFormattingCheckItemBase(BarManager manager)
			: base(manager) {
		}
		protected UnitDependentRichEditFormattingCheckItemBase(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			IRichEditDocumentServer server = RichEditControl;
			server.UnitChanging += OnUnitChanging;
			server.UnitChanged += OnUnitChanged;
		}
		protected override void UnsubscribeControlEvents() {
			IRichEditDocumentServer server = RichEditControl;
			server.UnitChanging -= OnUnitChanging;
			server.UnitChanged -= OnUnitChanged;
			base.UnsubscribeControlEvents();
		}
		bool wasDefaultCaption;
		void OnUnitChanging(object sender, EventArgs e) {
			this.wasDefaultCaption = !ShouldSerializeCaption();
		}
		void OnUnitChanged(object sender, EventArgs e) {
			if (wasDefaultCaption)
				ResetCaption();
			wasDefaultCaption = false;
		}
	}
	#endregion
	#region SetNormalSectionPageMarginsItem
	public class SetNormalSectionPageMarginsItem : UnitDependentRichEditFormattingCheckItemBase {
		public SetNormalSectionPageMarginsItem() {
		}
		public SetNormalSectionPageMarginsItem(BarManager manager)
			: base(manager) {
		}
		public SetNormalSectionPageMarginsItem(string caption)
			: base(caption) {
		}
		public SetNormalSectionPageMarginsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetNormalSectionPageMargins; } }
	}
	#endregion
	#region SetNarrowSectionPageMarginsItem
	public class SetNarrowSectionPageMarginsItem : UnitDependentRichEditFormattingCheckItemBase {
		public SetNarrowSectionPageMarginsItem() {
		}
		public SetNarrowSectionPageMarginsItem(BarManager manager)
			: base(manager) {
		}
		public SetNarrowSectionPageMarginsItem(string caption)
			: base(caption) {
		}
		public SetNarrowSectionPageMarginsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetNarrowSectionPageMargins; } }
	}
	#endregion
	#region SetModerateSectionPageMarginsItem
	public class SetModerateSectionPageMarginsItem : UnitDependentRichEditFormattingCheckItemBase {
		public SetModerateSectionPageMarginsItem() {
		}
		public SetModerateSectionPageMarginsItem(BarManager manager)
			: base(manager) {
		}
		public SetModerateSectionPageMarginsItem(string caption)
			: base(caption) {
		}
		public SetModerateSectionPageMarginsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetModerateSectionPageMargins; } }
	}
	#endregion
	#region SetWideSectionPageMarginsItem
	public class SetWideSectionPageMarginsItem : UnitDependentRichEditFormattingCheckItemBase {
		public SetWideSectionPageMarginsItem() {
		}
		public SetWideSectionPageMarginsItem(BarManager manager)
			: base(manager) {
		}
		public SetWideSectionPageMarginsItem(string caption)
			: base(caption) {
		}
		public SetWideSectionPageMarginsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetWideSectionPageMargins; } }
	}
	#endregion
	#region ShowPageMarginsSetupFormItem
	public class ShowPageMarginsSetupFormItem: RichEditCommandBarButtonItem {
		public ShowPageMarginsSetupFormItem() {
		}
		public ShowPageMarginsSetupFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowPageMarginsSetupFormItem(string caption)
			: base(caption) {
		}
		public ShowPageMarginsSetupFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowPageMarginsSetupForm; } }
	}
	#endregion
	#region SetSectionPaperKindMenuItem
	public class SetSectionPaperKindMenuItem : BarCheckItem {
		#region Fields
		readonly RichEditControl control;
		readonly PaperKind paperKind;
		#endregion
		public SetSectionPaperKindMenuItem(RichEditControl control, PaperKind paperKind, string paperKindName) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.paperKind = paperKind;
			this.Caption = CreateCaption(paperKindName);
			ChangeSectionPaperKindCommand command = CreateCommand();
			BarCheckItemUIState state = new BarCheckItemUIState(this);
			command.UpdateUIState(state);
		}
		protected override void OnClick(BarItemLink link) {
			ChangeSectionPaperKindCommand command = CreateCommand();
			command.Execute();
		}
		protected internal virtual ChangeSectionPaperKindCommand CreateCommand() {
			ChangeSectionPaperKindCommand command = new ChangeSectionPaperKindCommand(control, paperKind);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected internal virtual string CreateCaption(string paperKindName) {
			Size paperSizeInTwips = PaperSizeCalculator.CalculatePaperSize(paperKind);
			DocumentModel documentModel = control.InnerControl.DocumentModel;
			DocumentUnit unit = control.InnerControl.Unit == DocumentUnit.Document ? DocumentUnit.Inch : control.InnerControl.Unit;
			UnitConverter unitConverter = documentModel.InternalAPI.UnitConverters[unit];
			UIUnit width = new UIUnit(unitConverter.FromUnits(documentModel.UnitConverter.TwipsToModelUnits(paperSizeInTwips.Width)), unit, UnitPrecisionDictionary.DefaultPrecisions);
			UIUnit height = new UIUnit(unitConverter.FromUnits(documentModel.UnitConverter.TwipsToModelUnits(paperSizeInTwips.Height)), unit, UnitPrecisionDictionary.DefaultPrecisions);
			return String.Format("{0}\r\n{1} x {2}", paperKindName, width.ToString(), height.ToString());
		}
	}
	#endregion
	#region ShowPagePaperSetupFormItem
	public class ShowPagePaperSetupFormItem: RichEditCommandBarButtonItem {
		public ShowPagePaperSetupFormItem(RichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.RichEditControl = control;
			this.UpdateItemCaption();
			Command command = CreateCommand();
			BarButtonItemUIState state = new BarButtonItemUIState(this);
			command.UpdateUIState(state);
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowPagePaperSetupForm; } }
		protected override void OnClick(BarItemLink link) {
			Command command = CreateCommand();
			command.Execute();
		}
	}
	#endregion
	#region ChangeSectionPaperKindItem
	public class ChangeSectionPaperKindItem: RichEditCommandBarButtonItem {
		PopupMenu popupMenu = new PopupMenu();
		public ChangeSectionPaperKindItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public ChangeSectionPaperKindItem(BarManager manager)
			: base(manager) {
		}
		public ChangeSectionPaperKindItem(string caption)
			: base(caption) {
		}
		public ChangeSectionPaperKindItem()
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
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeSectionPaperKindPlaceholder; } }
		#endregion
		protected virtual void PopulatePopupMenu() {
			if (this.popupMenu == null)
				return;
			Type resourceFinder = typeof(DevExpress.Data.ResFinder);
			string resourceFileName = DXDisplayNameAttribute.DefaultResourceFile;
			ResourceManager resourceManager = new System.Resources.ResourceManager(string.Concat(resourceFinder.Namespace, ".", resourceFileName), resourceFinder.Assembly);
			IList<PaperKind> paperKindList = ChangeSectionPaperKindCommand.DefaultPaperKindList;
			this.popupMenu.BeginUpdate();
			try {
				foreach (PaperKind paperKind in paperKindList) {
					SetSectionPaperKindMenuItem item = new SetSectionPaperKindMenuItem(RichEditControl, paperKind, resourceManager.GetString(DevExpress.XtraRichEdit.Forms.PageSetupForm.GetResourceName(paperKind)));
					this.popupMenu.ItemLinks.Add(item);
				}
				this.popupMenu.ItemLinks.Add(new ShowPagePaperSetupFormItem(RichEditControl));
			}
			finally {
				this.popupMenu.EndUpdate();
			}
		}
		protected virtual void RefreshPopupMenu() {
			DeletePopupItems();
			if (RichEditControl != null)
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
	#region ChangeSectionPageOrientationItem
	public class ChangeSectionPageOrientationItem : RichEditCommandBarSubItem {
		public ChangeSectionPageOrientationItem() {
		}
		public ChangeSectionPageOrientationItem(BarManager manager)
			: base(manager) {
		}
		public ChangeSectionPageOrientationItem(string caption)
			: base(caption) {
		}
		public ChangeSectionPageOrientationItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeSectionPageOrientation; } }
	}
	#endregion
	#region SetPortraitPageOrientationItem
	public class SetPortraitPageOrientationItem : RichEditCommandBarCheckItem {
		public SetPortraitPageOrientationItem() {
		}
		public SetPortraitPageOrientationItem(BarManager manager)
			: base(manager) {
		}
		public SetPortraitPageOrientationItem(string caption)
			: base(caption) {
		}
		public SetPortraitPageOrientationItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetPortraitPageOrientation; } }
	}
	#endregion
	#region SetLandscapePageOrientationItem
	public class SetLandscapePageOrientationItem : RichEditCommandBarCheckItem {
		public SetLandscapePageOrientationItem() {
		}
		public SetLandscapePageOrientationItem(BarManager manager)
			: base(manager) {
		}
		public SetLandscapePageOrientationItem(string caption)
			: base(caption) {
		}
		public SetLandscapePageOrientationItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetLandscapePageOrientation; } }
	}
	#endregion
	#region ChangeSectionColumnsItem
	public class ChangeSectionColumnsItem : RichEditCommandBarSubItem {
		public ChangeSectionColumnsItem() {
		}
		public ChangeSectionColumnsItem(BarManager manager)
			: base(manager) {
		}
		public ChangeSectionColumnsItem(string caption)
			: base(caption) {
		}
		public ChangeSectionColumnsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetSectionColumnsPlaceholder; } }
	}
	#endregion
	#region SetSectionOneColumnItem
	public class SetSectionOneColumnItem : RichEditCommandBarCheckItem {
		public SetSectionOneColumnItem() {
		}
		public SetSectionOneColumnItem(BarManager manager)
			: base(manager) {
		}
		public SetSectionOneColumnItem(string caption)
			: base(caption) {
		}
		public SetSectionOneColumnItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetSectionOneColumn; } }
	}
	#endregion
	#region SetSectionTwoColumnsItem
	public class SetSectionTwoColumnsItem : RichEditCommandBarCheckItem {
		public SetSectionTwoColumnsItem() {
		}
		public SetSectionTwoColumnsItem(BarManager manager)
			: base(manager) {
		}
		public SetSectionTwoColumnsItem(string caption)
			: base(caption) {
		}
		public SetSectionTwoColumnsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetSectionTwoColumns; } }
	}
	#endregion
	#region SetSectionThreeColumnsItem
	public class SetSectionThreeColumnsItem : RichEditCommandBarCheckItem {
		public SetSectionThreeColumnsItem() {
		}
		public SetSectionThreeColumnsItem(BarManager manager)
			: base(manager) {
		}
		public SetSectionThreeColumnsItem(string caption)
			: base(caption) {
		}
		public SetSectionThreeColumnsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetSectionThreeColumns; } }
	}
	#endregion
	#region ShowColumnsSetupFormItem
	public class ShowColumnsSetupFormItem: RichEditCommandBarButtonItem {
		public ShowColumnsSetupFormItem() {
		}
		public ShowColumnsSetupFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowColumnsSetupFormItem(string caption)
			: base(caption) {
		}
		public ShowColumnsSetupFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowColumnsSetupForm; } }
	}
	#endregion
	#region ChangeSectionPaperKindItem
	public class ChangeSectionPagePaperKindItem : RichEditCommandBarSubItem {
		public ChangeSectionPagePaperKindItem() {
		}
		public ChangeSectionPagePaperKindItem(BarManager manager)
			: base(manager) {
		}
		public ChangeSectionPagePaperKindItem(string caption)
			: base(caption) {
		}
		public ChangeSectionPagePaperKindItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeSectionPaperKindPlaceholder; } }
	}
	#endregion
	#region ChangeSectionPaperKindListItem
	public class ChangeSectionPaperKindListItem : BarListItem {
		public ChangeSectionPaperKindListItem() {
		}
		protected override void CreateItems() {
			base.CreateItems();
			IList<PaperKind> paperKindList = ChangeSectionPaperKindCommand.DefaultPaperKindList;
			int count = paperKindList.Count;
			for (int i = 0; i < count; i++) {
				SetSectionPagePaperKindItem item = new SetSectionPagePaperKindItem(Manager);
				item.PaperKind = paperKindList[i];
			}
		}
	}
	#endregion
	#region SetSectionPaperKindItem
	public class SetSectionPagePaperKindItem: RichEditCommandBarButtonItem {
		PaperKind paperKind;
		public SetSectionPagePaperKindItem() {
		}
		public SetSectionPagePaperKindItem(BarManager manager)
			: base(manager) {
		}
		public SetSectionPagePaperKindItem(string caption)
			: base(caption) {
		}
		public SetSectionPagePaperKindItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeSectionPaperKind; } }
		public PaperKind PaperKind { get { return paperKind; } set { paperKind = value; } }
		protected override Command CreateCommand() {
			Command result = base.CreateCommand();
			ChangeSectionPaperKindCommand command = (ChangeSectionPaperKindCommand)result;
			command.PaperKind = PaperKind;
			command.CommandSourceType = CommandSourceType.Menu;
			return result;
		}
	}
	#endregion
	#region InsertBreakItem
	public class InsertBreakItem : RichEditCommandBarSubItem {
		public InsertBreakItem() {
		}
		public InsertBreakItem(BarManager manager)
			: base(manager) {
		}
		public InsertBreakItem(string caption)
			: base(caption) {
		}
		public InsertBreakItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertBreak; } }
	}
	#endregion
	#region InsertColumnBreakItem
	public class InsertColumnBreakItem: RichEditCommandBarButtonItem {
		public InsertColumnBreakItem() {
		}
		public InsertColumnBreakItem(BarManager manager)
			: base(manager) {
		}
		public InsertColumnBreakItem(string caption)
			: base(caption) {
		}
		public InsertColumnBreakItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertColumnBreak; } }
	}
	#endregion
	#region InsertSectionBreakNextPageItem
	public class InsertSectionBreakNextPageItem: RichEditCommandBarButtonItem {
		public InsertSectionBreakNextPageItem() {
		}
		public InsertSectionBreakNextPageItem(BarManager manager)
			: base(manager) {
		}
		public InsertSectionBreakNextPageItem(string caption)
			: base(caption) {
		}
		public InsertSectionBreakNextPageItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertSectionBreakNextPage; } }
	}
	#endregion
	#region InsertSectionBreakContinuousItem
	public class InsertSectionBreakContinuousItem: RichEditCommandBarButtonItem {
		public InsertSectionBreakContinuousItem() {
		}
		public InsertSectionBreakContinuousItem(BarManager manager)
			: base(manager) {
		}
		public InsertSectionBreakContinuousItem(string caption)
			: base(caption) {
		}
		public InsertSectionBreakContinuousItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertSectionBreakContinuous; } }
	}
	#endregion
	#region InsertSectionBreakEvenPageItem
	public class InsertSectionBreakEvenPageItem: RichEditCommandBarButtonItem {
		public InsertSectionBreakEvenPageItem() {
		}
		public InsertSectionBreakEvenPageItem(BarManager manager)
			: base(manager) {
		}
		public InsertSectionBreakEvenPageItem(string caption)
			: base(caption) {
		}
		public InsertSectionBreakEvenPageItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertSectionBreakEvenPage; } }
	}
	#endregion
	#region InsertSectionBreakOddPageItem
	public class InsertSectionBreakOddPageItem: RichEditCommandBarButtonItem {
		public InsertSectionBreakOddPageItem() {
		}
		public InsertSectionBreakOddPageItem(BarManager manager)
			: base(manager) {
		}
		public InsertSectionBreakOddPageItem(string caption)
			: base(caption) {
		}
		public InsertSectionBreakOddPageItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertSectionBreakOddPage; } }
	}
	#endregion
	#region ChangeSectionLineNumberingItem
	public class ChangeSectionLineNumberingItem : RichEditCommandBarSubItem {
		public ChangeSectionLineNumberingItem() {
		}
		public ChangeSectionLineNumberingItem(BarManager manager)
			: base(manager) {
		}
		public ChangeSectionLineNumberingItem(string caption)
			: base(caption) {
		}
		public ChangeSectionLineNumberingItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeSectionLineNumbering; } }
	}
	#endregion
	#region SetSectionLineNumberingNoneItem
	public class SetSectionLineNumberingNoneItem : RichEditCommandBarCheckItem {
		public SetSectionLineNumberingNoneItem() {
		}
		public SetSectionLineNumberingNoneItem(BarManager manager)
			: base(manager) {
		}
		public SetSectionLineNumberingNoneItem(string caption)
			: base(caption) {
		}
		public SetSectionLineNumberingNoneItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetSectionLineNumberingNone; } }
	}
	#endregion
	#region SetSectionLineNumberingContinuousItem
	public class SetSectionLineNumberingContinuousItem : RichEditCommandBarCheckItem {
		public SetSectionLineNumberingContinuousItem() {
		}
		public SetSectionLineNumberingContinuousItem(BarManager manager)
			: base(manager) {
		}
		public SetSectionLineNumberingContinuousItem(string caption)
			: base(caption) {
		}
		public SetSectionLineNumberingContinuousItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetSectionLineNumberingContinuous; } }
	}
	#endregion
	#region SetSectionLineNumberingRestartNewPageItem
	public class SetSectionLineNumberingRestartNewPageItem : RichEditCommandBarCheckItem {
		public SetSectionLineNumberingRestartNewPageItem() {
		}
		public SetSectionLineNumberingRestartNewPageItem(BarManager manager)
			: base(manager) {
		}
		public SetSectionLineNumberingRestartNewPageItem(string caption)
			: base(caption) {
		}
		public SetSectionLineNumberingRestartNewPageItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetSectionLineNumberingRestartNewPage; } }
	}
	#endregion
	#region SetSectionLineNumberingRestartNewSectionItem
	public class SetSectionLineNumberingRestartNewSectionItem : RichEditCommandBarCheckItem {
		public SetSectionLineNumberingRestartNewSectionItem() {
		}
		public SetSectionLineNumberingRestartNewSectionItem(BarManager manager)
			: base(manager) {
		}
		public SetSectionLineNumberingRestartNewSectionItem(string caption)
			: base(caption) {
		}
		public SetSectionLineNumberingRestartNewSectionItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetSectionLineNumberingRestartNewSection; } }
	}
	#endregion
	#region ToggleParagraphSuppressLineNumbersItem
	public class ToggleParagraphSuppressLineNumbersItem : RichEditCommandBarCheckItem {
		public ToggleParagraphSuppressLineNumbersItem() {
		}
		public ToggleParagraphSuppressLineNumbersItem(BarManager manager)
			: base(manager) {
		}
		public ToggleParagraphSuppressLineNumbersItem(string caption)
			: base(caption) {
		}
		public ToggleParagraphSuppressLineNumbersItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleParagraphSuppressLineNumbers; } }
	}
	#endregion
	#region ShowLineNumberingFormItem
	public class ShowLineNumberingFormItem: RichEditCommandBarButtonItem {
		public ShowLineNumberingFormItem() {
		}
		public ShowLineNumberingFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowLineNumberingFormItem(string caption)
			: base(caption) {
		}
		public ShowLineNumberingFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowLineNumberingForm; } }
	}
	#endregion
	#region RichEditPageBackgroundItemBuilder
	public class RichEditPageBackgroundItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ChangePageColorItem());
		}
	}
	#endregion
	#region ChangePageColorItem
	public class ChangePageColorItem : ChangeColorItemBase<RichEditControl, RichEditCommandId> {
		public ChangePageColorItem() {
		}
		public ChangePageColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangePageColorItem(string caption)
			: base(caption) {
		}
		public ChangePageColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsDropDown { get { return true; } set { } }
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.None; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.All; } }
		protected override string DefaultColorButtonCaption { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_NoColor); } }
		#endregion
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(RichEditCommandId.ChangePageColor);
			if (command != null) {
				command.CommandSourceType = CommandSourceType.Menu;
				return command;
			}
			return null;
		}
		protected override void DrawColorRectangle(Image image, Rectangle rect) {
		}
		protected override void InvokeCommand() {
		}
	}
	#endregion
	#region RichEditPageSetupBarCreator
	public class RichEditPageSetupBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PageLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PageSetupRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(PageSetupBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new PageSetupBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditPageLayoutPageSetupItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PageLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PageSetupRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditPageBackgroundBarCreator
	public class RichEditPageBackgroundBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PageLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PageBackgroundRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(PageBackgroundBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 4; } }
		public override Bar CreateBar() {
			return new PageBackgroundBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditPageBackgroundItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PageLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PageBackgroundRibbonPageGroup() { AllowTextClipping = false };
		}
	}
	#endregion
	#region PageSetupBar
	public class PageSetupBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public PageSetupBar()
			: base() {
		}
		public PageSetupBar(BarManager manager)
			: base(manager) {
		}
		public PageSetupBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupPageSetup); } }
	}
	#endregion
	#region PageBackgroundBar
	public class PageBackgroundBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public PageBackgroundBar()
			: base() {
		}
		public PageBackgroundBar(BarManager manager)
			: base(manager) {
		}
		public PageBackgroundBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupPageBackground); } }
	}
	#endregion
	#region PageLayoutRibbonPage
	public class PageLayoutRibbonPage : ControlCommandBasedRibbonPage {
		public PageLayoutRibbonPage() {
		}
		public PageLayoutRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PagePageLayout); } }
		protected override RibbonPage CreatePage() {
			return new PageLayoutRibbonPage();
		}
	}
	#endregion
	#region PageSetupRibbonPageGroup
	public class PageSetupRibbonPageGroup : RichEditControlRibbonPageGroup {
		public PageSetupRibbonPageGroup() {
		}
		public PageSetupRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupPageSetup); } }
		public override RichEditCommandId CommandId { get { return RichEditCommandId.ShowPageSetupForm; } }
	}
	#endregion
	#region PageBackgroundRibbonPageGroup
	public class PageBackgroundRibbonPageGroup : RichEditControlRibbonPageGroup {
		public PageBackgroundRibbonPageGroup() {
		}
		public PageBackgroundRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupPageBackground); } }
	}
	#endregion
}
