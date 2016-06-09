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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditMailMergeItemBuilder
	public class RichEditMailMergeItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new InsertMergeFieldItem());
			items.Add(new ShowAllFieldCodesItem());
			items.Add(new ShowAllFieldResultsItem());
			items.Add(new ToggleViewMergedDataItem());
		}
	}
	#endregion
	#region InsertMergeFieldMenuItem
	public class InsertMergeFieldMenuItem : BarButtonItem {
		#region Fields
		RichEditControl control;
		string fieldName;
		#endregion
		public InsertMergeFieldMenuItem(RichEditControl control, MergeFieldName mergeFieldName)
			: base(null, mergeFieldName.DisplayName) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.fieldName = mergeFieldName.Name;
		}
		protected override void OnClick(BarItemLink link) {
			InsertMergeFieldCommand command = control.CreateCommand(RichEditCommandId.InsertMailMergeField) as InsertMergeFieldCommand;
			if (command != null) {
				command.FieldArgument = fieldName;
				command.CommandSourceType = CommandSourceType.Menu;
				command.Execute();
			}
		}
	}
	#endregion
	#region PopupMergeBasedItem
	public class PopupMergeBasedItem : RichEditCommandBarButtonItem {
		PopupMenu popupMenu = new PopupMenu();
		public PopupMergeBasedItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public PopupMergeBasedItem(BarManager manager)
			: base(manager) {
		}
		public PopupMergeBasedItem(string caption)
			: base(caption) {
		}
		public PopupMergeBasedItem()
			: base() {
		}
		#region Properties
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl { get { return popupMenu; } set { } }
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowInsertMergeFieldForm; } }
		protected PopupMenu PopupMenu { get { return popupMenu; } set { } }
		#endregion
		protected virtual void PopulatePopupMenu() { 
		}
		protected virtual void RefreshPopupMenu() {
			DeletePopupItems();
			if (RichEditControl != null)
				PopulatePopupMenu();
		}
		protected virtual void DeletePopupItems() {
			if (popupMenu == null)
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
			if (popupMenu != null)
				this.popupMenu.BeforePopup += OnBeforePopup;
		}
		protected override void UnsubscribeControlEvents() {
			if (popupMenu != null)
				this.popupMenu.BeforePopup -= OnBeforePopup;
			base.UnsubscribeControlEvents();
		}
	   protected virtual void OnBeforePopup(object sender, CancelEventArgs e) {
			RefreshPopupMenu();
	   }
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if (popupMenu != null)
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
	#region InsertMergeFieldItem
	public class InsertMergeFieldItem:  PopupMergeBasedItem {
		public InsertMergeFieldItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public InsertMergeFieldItem(BarManager manager)
			: base(manager) {
		}
		public InsertMergeFieldItem(string caption)
			: base(caption) {
		}
		public InsertMergeFieldItem()
			: base() {
		}
		#region Properties
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowInsertMergeFieldForm; } }
		#endregion
		protected override void PopulatePopupMenu() {
			if (PopupMenu == null)
				return;
			MergeFieldName[] mergeFieldNames = Control.DocumentModel.GetDatabaseFieldNames();
			Array.Sort(mergeFieldNames);
			mergeFieldNames = Control.InnerControl.RaiseCustomizeMergeFields(new CustomizeMergeFieldsEventArgs(mergeFieldNames));
			if (mergeFieldNames == null || mergeFieldNames.Length == 0)
				return;
			this.PopupMenu.BeginUpdate();
			try {
				foreach (MergeFieldName field in mergeFieldNames) {
					InsertMergeFieldMenuItem item = new InsertMergeFieldMenuItem(Control, field);
					this.PopupMenu.ItemLinks.Add(item);
				}
			}
			finally {
				this.PopupMenu.EndUpdate();
			}
		}
	}
	#endregion
	#region ToggleViewMergedDataItem
	public class ToggleViewMergedDataItem : RichEditCommandBarCheckItem {
		public ToggleViewMergedDataItem() {
		}
		public ToggleViewMergedDataItem(BarManager manager)
			: base(manager) {
		}
		public ToggleViewMergedDataItem(string caption)
			: base(caption) {
		}
		public ToggleViewMergedDataItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleViewMergedData; } }
	}
	#endregion
	#region ShowAllFieldCodesItem
	public class ShowAllFieldCodesItem: RichEditCommandBarButtonItem {
		public ShowAllFieldCodesItem() {
		}
		public ShowAllFieldCodesItem(BarManager manager)
			: base(manager) {
		}
		public ShowAllFieldCodesItem(string caption)
			: base(caption) {
		}
		public ShowAllFieldCodesItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowAllFieldCodes; } }
	}
	#endregion
	#region ShowAllFieldResultsItem
	public class ShowAllFieldResultsItem: RichEditCommandBarButtonItem {
		public ShowAllFieldResultsItem() {
		}
		public ShowAllFieldResultsItem(BarManager manager)
			: base(manager) {
		}
		public ShowAllFieldResultsItem(string caption)
			: base(caption) {
		}
		public ShowAllFieldResultsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowAllFieldResults; } }
	}
	#endregion
	#region RichEditMailMergeBarCreator
	public class RichEditMailMergeBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(MailingsRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(MailMergeRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(MailMergeBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 9; } }
		public override Bar CreateBar() {
			return new MailMergeBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditMailMergeItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new MailingsRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new MailMergeRibbonPageGroup();
		}
	}
	#endregion
	#region MailMergeBar
	public class MailMergeBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public MailMergeBar()
			: base() {
		}
		public MailMergeBar(BarManager manager)
			: base(manager) {
		}
		public MailMergeBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupMailMerge); } }
	}
	#endregion
	#region MailingsRibbonPage
	public class MailingsRibbonPage : ControlCommandBasedRibbonPage {
		public MailingsRibbonPage() {
		}
		public MailingsRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageMailings); } }
		protected override RibbonPage CreatePage() {
			return new MailingsRibbonPage();
		}
	}
	#endregion
	#region MailMergeRibbonPageGroup
	public class MailMergeRibbonPageGroup : RichEditControlRibbonPageGroup {
		public MailMergeRibbonPageGroup() {
		}
		public MailMergeRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupMailMerge); } }
	}
	#endregion
}
