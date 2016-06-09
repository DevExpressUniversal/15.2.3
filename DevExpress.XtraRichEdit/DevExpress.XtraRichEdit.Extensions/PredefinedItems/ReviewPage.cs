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
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraRichEdit.PredefinedItems;
using DevExpress.Utils;
using System.Collections.ObjectModel;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditDocumentProtectionItemBuilder
	public class RichEditDocumentProtectionItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ProtectDocumentItem());
			items.Add(new ChangeRangeEditingPermissionsItem());
			items.Add(new UnprotectDocumentItem());
		}
	}
	#endregion
	#region ProtectDocumentItem
	public class ProtectDocumentItem : RichEditCommandBarButtonItem {
		public ProtectDocumentItem() {
		}
		public ProtectDocumentItem(BarManager manager)
			: base(manager) {
		}
		public ProtectDocumentItem(string caption)
			: base(caption) {
		}
		public ProtectDocumentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ProtectDocument; } }
	}
	#endregion
	#region UnprotectDocumentItem
	public class UnprotectDocumentItem : RichEditCommandBarButtonItem {
		public UnprotectDocumentItem() {
		}
		public UnprotectDocumentItem(BarManager manager)
			: base(manager) {
		}
		public UnprotectDocumentItem(string caption)
			: base(caption) {
		}
		public UnprotectDocumentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.UnprotectDocument; } }
	}
	#endregion
	#region EditPermissionRangeItem
	public class ChangeRangeEditingPermissionsItem : RichEditCommandBarButtonItem {
		public ChangeRangeEditingPermissionsItem() {
		}
		public ChangeRangeEditingPermissionsItem(BarManager manager)
			: base(manager) {
		}
		public ChangeRangeEditingPermissionsItem(string caption)
			: base(caption) {
		}
		public ChangeRangeEditingPermissionsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowRangeEditingPermissionsForm; } }
	}
	#endregion
	#region RichEditDocumentProofingItemBuilder
	public class RichEditDocumentProofingItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new CheckSpellingItem());
		}
	}
	#endregion
	#region CheckSpellingItem
	public class CheckSpellingItem : RichEditCommandBarButtonItem {
		public CheckSpellingItem() {
		}
		public CheckSpellingItem(BarManager manager)
			: base(manager) {
		}
		public CheckSpellingItem(string caption)
			: base(caption) {
		}
		public CheckSpellingItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.CheckSpelling; } }
	}
	#endregion
	#region ToggleSpellCheckAsYouType
	public class ToggleSpellCheckAsYouType : RichEditCommandBarCheckItem {
		public ToggleSpellCheckAsYouType() {
		}
		public ToggleSpellCheckAsYouType(BarManager manager)
			: base(manager) {
		}
		public ToggleSpellCheckAsYouType(string caption)
			: base(caption) {
		}
		public ToggleSpellCheckAsYouType(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleSpellCheckAsYouType; } }
	}
	#endregion
	#region RichEditDocumentLangugeItemBuilder
	public class RichEditDocumentLanguageItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ChangeLanguageItem());
		}
	}
	#endregion
	#region ChangeLanguageItem
	public class ChangeLanguageItem : RichEditCommandBarButtonItem {
		public ChangeLanguageItem() {
		}
		public ChangeLanguageItem(BarManager manager)
			: base(manager) {
		}
		public ChangeLanguageItem(string caption)
			: base(caption) {
		}
		public ChangeLanguageItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowLanguageForm; } }
	}
	#endregion
	#region RichEditDocumentCommentItemBuilder
	public class RichEditDocumentCommentItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new NewCommentItem());
			DeleteCommentsItem deleteCommentsItem = new DeleteCommentsItem();
			items.Add(deleteCommentsItem);
			deleteCommentsItem.AddBarItem(new DeleteOneCommentItem());
			deleteCommentsItem.AddBarItem(new DeleteAllCommentsShownItem());
			deleteCommentsItem.AddBarItem(new DeleteAllCommentsItem());
			items.Add(new PreviousCommentItem());
			items.Add(new NextCommentItem());
		}
	}
	#endregion
	#region NewCommentItem
	public class NewCommentItem : RichEditCommandBarButtonItem {
		public NewCommentItem() {
		}
		public NewCommentItem(BarManager manager)
			: base(manager) {
		}
		public NewCommentItem(string caption)
			: base(caption) {
		}
		public NewCommentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.NewComment; }
		}
	}
	#endregion
	#region DeleteCommentItem
	public class DeleteCommentsItem : RichEditCommandBarSubItem { 
	public DeleteCommentsItem() {
		}
		public DeleteCommentsItem(BarManager manager)
			: base(manager) {
		}
		public DeleteCommentsItem(string caption)
			: base(caption) {
		}
		public DeleteCommentsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.DeleteCommentsPlaceholder; }
		}
	}
	#endregion
	#region DeleteOneCommentItem
	public class DeleteOneCommentItem : RichEditCommandBarButtonItem {
		public DeleteOneCommentItem() {
		}
		public DeleteOneCommentItem(BarManager manager)
			: base(manager) {
		}
		public DeleteOneCommentItem(string caption)
			: base(caption) {
		}
		public DeleteOneCommentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.DeleteOneComment; }
		}
	}
	#endregion
	#region DeleteAllCommentsShown
	public class DeleteAllCommentsShownItem : RichEditCommandBarButtonItem {
		public DeleteAllCommentsShownItem() {
		}
		public DeleteAllCommentsShownItem(BarManager manager)
			: base(manager) {
		}
		public DeleteAllCommentsShownItem(string caption)
			: base(caption) {
		}
		public DeleteAllCommentsShownItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.DeleteAllCommentsShown; }
		}
	}
	#endregion
	#region DeleteAllCommentsItem
	public class DeleteAllCommentsItem : RichEditCommandBarButtonItem {
		public DeleteAllCommentsItem() {
		}
		public DeleteAllCommentsItem(BarManager manager)
			: base(manager) {
		}
		public DeleteAllCommentsItem(string caption)
			: base(caption) {
		}
		public DeleteAllCommentsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.DeleteAllComments; }
		}
	}
	#endregion
	#region PreviousCommentItem
	public class PreviousCommentItem : RichEditCommandBarButtonItem {
		public PreviousCommentItem() {
		}
		public PreviousCommentItem(BarManager manager)
			: base(manager) {
		}
		public PreviousCommentItem(string caption)
			: base(caption) {
		}
		public PreviousCommentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.PreviousComment; }
		}
	}
	#endregion
	#region NextCommentItem
	public class NextCommentItem : RichEditCommandBarButtonItem {
		public NextCommentItem() {
		}
		public NextCommentItem(BarManager manager)
			: base(manager) {
		}
		public NextCommentItem(string caption)
			: base(caption) {
		}
		public NextCommentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.NextComment; }
		}
	}
	#endregion
	#region RichEditDocumentTrackingItemBuilder
	public class RichEditDocumentTrackingItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ChangeCommentItem());
			items.Add(new ReviewersItem());
			items.Add(new ReviewingPaneItem());
		}
	}
	#endregion
	#region ToggleAuthorVisibilityItem
	public class ToggleAuthorVisibilityItem : BarCheckItem {
		#region Fields
		RichEditControl control;
		string author;
		const string allAuthors = "All Authors";
		#endregion
		public ToggleAuthorVisibilityItem(RichEditControl control, string author)
			: base(null, true) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.author = author;			
		}
		public override string Caption { get { return author; } set {} }
		protected override void RaiseCheckedChanged() {
		}
		protected override void OnClick(BarItemLink link) {
			this.Checked = !this.Checked;
			if (this.Caption == allAuthors) {
				control.Options.Comments.ShowAllAuthors = this.Checked;
				if (this.Checked) 
					control.Options.Comments.Visibility = RichEditCommentVisibility.Visible;
				else 
					control.Options.Comments.Visibility = RichEditCommentVisibility.Hidden;
				if (!control.ActiveView.DocumentModel.CommentOptions.ShowAllAuthors) {
					ObservableCollection<string> visibleAuthors = control.Options.Comments.VisibleAuthors;
						visibleAuthors.Clear();
					return;
				}
				List<string> authors = control.DocumentModel.GetAuthors();
				if (authors.Count > 0) {
					ObservableCollection<string> visibleAuthors = control.Options.Comments.VisibleAuthors;
					foreach (string author in authors) { 
					if (!visibleAuthors.Contains(author))
						visibleAuthors.Add(author);
					}
				}
			}
			else {
				ObservableCollection<string> visibleAuthors = control.Options.Comments.VisibleAuthors;
				if (this.Checked) {
					control.DocumentModel.UnsubscribeCommentOptions();
					control.Options.Comments.Visibility = RichEditCommentVisibility.Visible;
					control.DocumentModel.SubscribeCommentOptions();
					if (!visibleAuthors.Contains(link.Item.Caption))
						visibleAuthors.Add(link.Item.Caption);
				}
				else {
					control.Options.Comments.ShowAllAuthors = false;  
					visibleAuthors.Remove(link.Item.Caption);
				}
			}
		}
	}
	#endregion
	#region ShowCommentItem
	public class ChangeCommentItem : RichEditCommandBarCheckItem {
		public ChangeCommentItem() { 
		}
		public ChangeCommentItem(BarManager manager)
			: base(manager) { 
		}
		public ChangeCommentItem(string caption)
			: base(caption) {
		}
		public ChangeCommentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.ViewComments; }
		}
	}
	#endregion
	#region ReviewersItem
	public class ReviewersItem : PopupMergeBasedItem {
		public ReviewersItem() { 
		}
		public ReviewersItem(BarManager manager)
			: base(manager) { 
		}
		public ReviewersItem(string caption)
			: base(caption) {
		}
		public ReviewersItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.Reviewers; }
		}
		protected override void PopulatePopupMenu() {
			if (PopupMenu == null)
				return;
			List<string> authors = Control.DocumentModel.GetAuthors();
			if (authors == null || authors.Count == 0)
				return;
			this.PopupMenu.BeginUpdate();
			try {
				ToggleAuthorVisibilityItem item = new ToggleAuthorVisibilityItem(Control, "All Authors");
				bool showAllAuthors = Control.ActiveView.DocumentModel.CommentOptions.ShowAllAuthors;
				item.Checked = showAllAuthors;
				this.PopupMenu.ItemLinks.Add(item).BeginGroup = true;
				foreach (string author in authors) {
					item = new ToggleAuthorVisibilityItem(Control, author);
					if (showAllAuthors)
						item.Checked = true;
					else
						item.Checked = CheckedAuthors(Control, author);					
					this.PopupMenu.ItemLinks.Add(item);
				}
			}
			finally {
				this.PopupMenu.EndUpdate();
			}
		}
		bool CheckedAuthors(RichEditControl control, string author) {
			ObservableCollection<string> visibleAuthors = control.Options.Comments.VisibleAuthors;
			return (visibleAuthors.Contains(author));
		}
	}
	#endregion
	#region ReviewingPaneItem
	public class ReviewingPaneItem : RichEditCommandBarCheckItem {
		public ReviewingPaneItem() { 
		}
		public ReviewingPaneItem(BarManager manager)
			: base(manager) { 
		} 
		public ReviewingPaneItem(string caption)
			: base(caption) {
		}
		public ReviewingPaneItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.ToggleReviewingPane; }
		}
	}
	#endregion
	#region RichEditDocumentProtectionBarCreator
	public class RichEditDocumentProtectionBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ReviewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(DocumentProtectionRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(DocumentProtectionBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 7; } }
		public override Bar CreateBar() {
			return new DocumentProtectionBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditDocumentProtectionItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ReviewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new DocumentProtectionRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditDocumentProofingBarCreator
	public class RichEditDocumentProofingBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ReviewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(DocumentProofingRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(DocumentProofingBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 5; } }
		public override Bar CreateBar() {
			return new DocumentProofingBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditDocumentProofingItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ReviewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new DocumentProofingRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditDocumentLanguageBarCreator
	public class RichEditDocumentLanguageBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ReviewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(DocumentProofingRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(DocumentProofingBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 6; } }
		public override Bar CreateBar() {
			return new DocumentLanguageBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditDocumentLanguageItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ReviewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new DocumentLanguageRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditDocumentCommentBarCreator
	public class RichEditDocumentCommentBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ReviewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(DocumentCommentRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(DocumentCommentBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 8; } }
		public override Bar CreateBar() {
			return new DocumentCommentBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditDocumentCommentItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ReviewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new DocumentCommentRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditDocumentTrackingBarCreator
	public class RichEditDocumentTrackingBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ReviewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(DocumentTrackingRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(DocumentTrackingBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 9; } }
		public override Bar CreateBar() {
			return new DocumentTrackingBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditDocumentTrackingItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ReviewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new DocumentTrackingRibbonPageGroup();
		}
	}
	#endregion
	#region DocumentProtectionBar
	public class DocumentProtectionBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public DocumentProtectionBar()
			: base() {
		}
		public DocumentProtectionBar(BarManager manager)
			: base(manager) {
		}
		public DocumentProtectionBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentProtection); } }
	}
	#endregion
	#region DocumentProofingBar
	public class DocumentProofingBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public DocumentProofingBar()
			: base() {
		}
		public DocumentProofingBar(BarManager manager)
			: base(manager) {
		}
		public DocumentProofingBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentProofing); } }
	}
	#endregion
	#region DocumentLanguageBar
	public class DocumentLanguageBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public DocumentLanguageBar()
			: base() {
		}
		public DocumentLanguageBar(BarManager manager)
			: base(manager) {
		}
		public DocumentLanguageBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentLanguage); } }
	}
	#endregion
	#region DocumentCommentBar
	public class DocumentCommentBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public DocumentCommentBar()
			: base() {
		}
		public DocumentCommentBar(BarManager manager)
			: base(manager) {
		}
		public DocumentCommentBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentComment); } }
	}
	#endregion
	#region DocumentTrackingBar
	public class DocumentTrackingBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public DocumentTrackingBar()
			: base() {
		}
		public DocumentTrackingBar(BarManager manager)
			: base(manager) {
		}
		public DocumentTrackingBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentTracking); } }
	}
	#endregion
	#region ReviewRibbonPage
	public class ReviewRibbonPage : ControlCommandBasedRibbonPage {
		public ReviewRibbonPage() {
		}
		public ReviewRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageReview); } }
		protected override RibbonPage CreatePage() {
			return new ReviewRibbonPage();
		}
	}
	#endregion
	#region DocumentProtectionRibbonPageGroup
	public class DocumentProtectionRibbonPageGroup : RichEditControlRibbonPageGroup {
		public DocumentProtectionRibbonPageGroup() {
		}
		public DocumentProtectionRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentProtection); } }
	}
	#endregion
	#region DocumentProofingRibbonPageGroup
	public class DocumentProofingRibbonPageGroup : RichEditControlRibbonPageGroup {
		public DocumentProofingRibbonPageGroup() {
		}
		public DocumentProofingRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentProofing); } }
	}
	#endregion
	#region DocumentLanguageRibbonPageGroup
	public class DocumentLanguageRibbonPageGroup : RichEditControlRibbonPageGroup {
		public DocumentLanguageRibbonPageGroup() {
		}
		public DocumentLanguageRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentLanguage); } }
	}
	#endregion
	#region DocumentCommentRibbonPageGroup
	public class DocumentCommentRibbonPageGroup : RichEditControlRibbonPageGroup {
		public DocumentCommentRibbonPageGroup() { 
		}
		public DocumentCommentRibbonPageGroup(string text)
			: base(text) { 
		}
		public override string DefaultText {
			get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentComment); }
		}
	}
	#endregion
	#region DocumentTrackingRibbonPageGroup
	public class DocumentTrackingRibbonPageGroup : RichEditControlRibbonPageGroup {
		public DocumentTrackingRibbonPageGroup() {
		}
		public DocumentTrackingRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText {
			get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentTracking); }
		}
	}
	#endregion
}
