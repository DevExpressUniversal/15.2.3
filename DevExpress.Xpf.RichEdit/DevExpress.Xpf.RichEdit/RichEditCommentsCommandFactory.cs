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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Utils;
#if WPF
using DevExpress.Xpf.RichEdit;
#else
using DevExpress.XtraRichEdit;
#endif
namespace DevExpress.XtraRichEdit.Services.Implementation {
	#region RichEditCommentCommandFactoryServiceBase (abstract class)
	public abstract class RichEditCommentCommandFactoryServiceBase : RichEditCommandFactoryService {
		readonly RichEditCommandFactoryService service;
		readonly InnerCommentControl commentControl;
		readonly RichEditControl control;
		static readonly Type[] constructorParametersInterface = new Type[] { typeof(InnerCommentControl) };
		protected RichEditCommentCommandFactoryServiceBase(InnerCommentControl commentControl, RichEditControl control, RichEditCommandFactoryService service)
			: base(commentControl) {
			Guard.ArgumentNotNull(commentControl, "commentControl");
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(service, "service");
			this.commentControl = commentControl;
			this.control = control;
			this.service = service;
		}
		protected InnerCommentControl CommentControl { get { return commentControl; } }
		protected RichEditControl MainControl { get { return control; } }
		protected RichEditCommandFactoryService Service { get { return service; } }
		protected internal override ConstructorInfo GetConstructorInfo(Type commandType) {
			ConstructorInfo ci = base.GetConstructorInfo(commandType);
			if (ci == null)
				ci = commandType.GetConstructor(constructorParametersInterface);
			return ci;
		}
		protected internal override void PopulateConstructorTable(RichEditCommandConstructorTable table) {
			AddCommandConstructor(table, RichEditCommandId.ShowPasteSpecialForm, typeof(RichEditCommentShowPasteSpecialFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowFontForm, typeof(RichEditCommentShowFontFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowParagraphForm, typeof(RichEditCommentShowParagraphFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowNumberingListForm, typeof(RichEditCommentShowNumberingListFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowHyperlinkForm, typeof(RichEditCommentShowHyperlinkFormCommand));
			AddCommandConstructor(table, RichEditCommandId.CreateHyperlink, typeof(RichEditCommentCreateHyperlinkContextMenuItemCommand));
			AddCommandConstructor(table, RichEditCommandId.EditHyperlink, typeof(RichEditCommentEditHyperlinkCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowBookmarkForm, typeof(RichEditCommentShowBookmarkFormCommand));
			AddCommandConstructor(table, RichEditCommandId.CreateBookmark, typeof(RichEditCommentCreateBookmarkContextMenuItemCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowEditStyleForm, typeof(RichEditCommentShowEditStyleFormCommand));
			AddCommandConstructor(table, RichEditCommandId.Find, typeof(RichEditCommentFindCommand));
			AddCommandConstructor(table, RichEditCommandId.Replace, typeof(RichEditCommentReplaceCommand));
			AddCommandConstructor(table, RichEditCommandId.InnerReplace, typeof(RichEditCommentReplaceInnerCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertPicture, typeof(RichEditCommentInsertPictureCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertPictureInner, typeof(RichEditCommentInsertPictureInnerCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTable, typeof(RichEditCommentInsertTableCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableInner, typeof(RichEditCommentInsertTableInnerCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowSymbolForm, typeof(RichEditCommentShowSymbolFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowTablePropertiesForm, typeof(RichEditCommentShowTablePropertiesFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowTablePropertiesFormMenuItem, typeof(RichEditCommentShowTablePropertiesFormMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowDeleteTableCellsForm, typeof(RichEditCommentShowDeleteTableCellsFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowDeleteTableCellsFormMenuItem, typeof(RichEditCommentShowDeleteTableCellsFormMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowSplitTableCellsForm, typeof(RichEditCommentShowSplitTableCellsFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowSplitTableCellsFormMenuItem, typeof(RichEditCommentShowSplitTableCellsFormMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFirstRow, typeof(RichEditCommentToggleFirstRowCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleLastRow, typeof(RichEditCommentToggleLastRowCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleBandedRows, typeof(RichEditCommentToggleBandedRowsCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleFirstColumn, typeof(RichEditCommentToggleFirstColumnCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleLastColumn, typeof(RichEditCommentToggleLastColumnCommand));
			AddCommandConstructor(table, RichEditCommandId.ToggleBandedColumns, typeof(RichEditCommentToggleBandedColumnsCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableStyle, typeof(RichEditCommentChangeTableStyleCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableCellsBorderColor, typeof(RichEditCommentChangeCurrentBorderRepositoryItemColorCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableCellsBorderLineStyle, typeof(RichEditCommentChangeCurrentBorderRepositoryItemLineStyleCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableCellsBorderLineWeight, typeof(RichEditCommentChangeCurrentBorderRepositoryItemLineThicknessCommand));
			AddCommandConstructor(table, RichEditCommandId.ChangeTableCellsShading, typeof(RichEditCommentChangeTableCellsShadingCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectTableColumns, typeof(SelectTableColumnsCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectTableCell, typeof(SelectTableCellCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectTableRow, typeof(SelectTableRowCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectTable, typeof(SelectTableCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowTableOptionsForm, typeof(RichEditCommentShowTableOptionsFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ImeUndo, typeof(RichEditCommentImeUndoCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteNonEmptySelection, typeof(RichEditCommentDeleteNonEmptySelectionCommand));
			AddCommandConstructor(table, RichEditCommandId.ResetMerging, typeof(RichEditCommentResetMergingCommand));
		}
		public override RichEditCommand CreateCommand(RichEditCommandId commandId) {
			if (commandId == RichEditCommandId.FileNew)
				return new CreateEmptyDocumentCommand(MainControl);
			if (commandId == RichEditCommandId.FileOpen)
				return new LoadDocumentCommand(MainControl);
			if (commandId == RichEditCommandId.FileSave)
				return new SaveDocumentCommand(MainControl);
			if (commandId == RichEditCommandId.FileSaveAs)
				return new SaveDocumentAsCommand(MainControl);
			if (commandId == RichEditCommandId.Print)
				return new PrintCommand(MainControl);
			if (commandId == RichEditCommandId.QuickPrint)
				return new QuickPrintCommand(MainControl);
			if (commandId == RichEditCommandId.PrintPreview)
				return new PrintPreviewCommand(MainControl);
			if (commandId == RichEditCommandId.NewComment)
				return new NewCommentCommand(MainControl);
			if (commandId == RichEditCommandId.DeleteCommentsPlaceholder)
				return new DeleteCommentCommand(MainControl);
			if (commandId == RichEditCommandId.DeleteOneComment)
				return new DeleteOneCommentCommand(MainControl);
			if (commandId == RichEditCommandId.DeleteAllCommentsShown)
				return new DeleteAllCommentsShownCommand(MainControl);
			if (commandId == RichEditCommandId.DeleteAllComments)
				return new DeleteAllCommentsCommand(MainControl);
			RichEditCommand command = base.CreateCommandCore(commandId);
			if (command != null)
				return command;
			if (PredicateCreateRichEditCommentDisableBaseCommand(commandId))
				return new RichEditCommentDisableBaseCommand(CommentControl, MainControl, commandId, Service);
			return CreateSharedCommand(commandId);
		}
		public virtual RichEditCommand CreateSharedCommand(RichEditCommandId commandId) {
			return null;
		}
		protected bool PredicateCreateRichEditCommentDisableBaseCommand(RichEditCommandId commandId) {
			return (commandId == RichEditCommandId.InsertTextBox || commandId == RichEditCommandId.InsertFloatingPicture || commandId == RichEditCommandId.ChangeSectionPageMargins ||
					commandId == RichEditCommandId.ChangeSectionPageOrientation || commandId == RichEditCommandId.SetSectionColumnsPlaceholder ||
					commandId == RichEditCommandId.InsertBreak || commandId == RichEditCommandId.ChangeSectionPaperKindPlaceholder || commandId == RichEditCommandId.ChangeSectionLineNumbering ||
					commandId == RichEditCommandId.ChangePageColor || commandId == RichEditCommandId.UpdateTableOfContents || commandId == RichEditCommandId.AddParagraphsToTableOfContents ||
					commandId == RichEditCommandId.UpdateTableOfFigures || commandId == RichEditCommandId.InsertMailMergeField || commandId == RichEditCommandId.InsertMailMergeFieldPlaceholder ||
					commandId == RichEditCommandId.ToggleViewMergedData || commandId == RichEditCommandId.ShowAllFieldCodes || commandId == RichEditCommandId.ShowAllFieldResults ||
					commandId == RichEditCommandId.LastDataRecord || commandId == RichEditCommandId.FirstDataRecord || commandId == RichEditCommandId.NextDataRecord || 
					commandId == RichEditCommandId.PreviousDataRecord || commandId == RichEditCommandId.MailMergeSaveDocumentAs ||
					commandId == RichEditCommandId.SwitchToSimpleView || commandId == RichEditCommandId.SwitchToDraftView || commandId == RichEditCommandId.SwitchToPrintLayoutView ||
					commandId == RichEditCommandId.GoToNextHeaderFooter || commandId == RichEditCommandId.GoToPreviousHeaderFooter || commandId == RichEditCommandId.ToggleDifferentFirstPage ||
					commandId == RichEditCommandId.ToggleDifferentOddAndEvenPages || commandId == RichEditCommandId.ResizeInlinePicture);
		}
	}
	#endregion
	#region RichEditCommentBaseCommand (abstract class)
	public abstract class RichEditCommentBaseCommand : RichEditCommand {
		RichEditCommand commentCommand;
		RichEditCommand command;
		InnerCommentControl commentControl;
		RichEditControl control;
		protected RichEditCommentBaseCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(control) {
			this.commentControl = commentControl;
			this.control = control;
			this.commentCommand = CreateCommandCore(commandId, commentControl, service);
			this.command = CreateCommandCore(commandId, control, service);
		}
		protected internal RichEditCommand CommentCommand { get { return commentCommand; } }
		protected internal RichEditCommand Command { get { return command; } }
		protected InnerCommentControl CommentControl { get { return commentControl; } }
		protected RichEditControl MainControl { get { return control; } }
		private RichEditCommand CreateCommandCore(RichEditCommandId commandId, IRichEditControl control, RichEditCommandFactoryService service) {
			ConstructorInfo ci;
			if (service.CommandConstructorTable.TryGetValue(commandId, out ci))
				return (RichEditCommand)ci.Invoke(new object[] { control });
			else
				return null;
		}
	}
	#endregion
	#region RichEditCommentDisableBaseCommand
	public class RichEditCommentDisableBaseCommand : RichEditCommentBaseCommand {
		public RichEditCommentDisableBaseCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(commentControl, control, commandId, service) {
		}
		protected DocumentModel MainDocumentModel { get { return MainControl.ActiveView.DocumentModel; } }
		public override DevExpress.XtraRichEdit.Localization.XtraRichEditStringId DescriptionStringId {
			get { return Command.DescriptionStringId; }
		}
		public override DevExpress.XtraRichEdit.Localization.XtraRichEditStringId MenuCaptionStringId {
			get { return Command.MenuCaptionStringId; }
		}
		public override string ImageName {
			get { return Command.ImageName; }
		}
		public override void ForceExecute(ICommandUIState state) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = true;
			state.Enabled = false;
		}
	}
	#endregion
	#region RichEditWithCommentsCommandFactoryService
	public class RichEditWithCommentsCommandFactoryService : RichEditCommentCommandFactoryServiceBase {
		public RichEditWithCommentsCommandFactoryService(InnerCommentControl commentControl, RichEditControl control, RichEditCommandFactoryService service)
			: base(commentControl, control, service) {
		}
		public override RichEditCommand CreateCommand(RichEditCommandId commandId) {
#if !WPF
			if (MainControl.IsDisposed || CommentControl.IsDisposed)
				return null;
#endif
			if (commandId == RichEditCommandId.InsertText || commandId == RichEditCommandId.OvertypeText)
				return new RichEditWithCommentsInsertTextCommand(CommentControl, MainControl, commandId, Service);
			if (commandId == RichEditCommandId.InsertSymbol)
				return new RichEditWithCommentsInsertSymbolCommand(CommentControl, MainControl, commandId, Service);
			if (commandId == RichEditCommandId.Undo)
				return new RichEditWithCommentsUndoCommand(CommentControl, MainControl, commandId, Service);
			if (commandId == RichEditCommandId.Redo)
				return new RichEditWithCommentsRedoCommand(CommentControl, MainControl, commandId, Service);
			if (commandId == RichEditCommandId.ResizeInlinePicture)
				return new RichEditWithCommentsResizeInlinePictureCommand(CommentControl, MainControl, commandId, Service);
			if (!MainControl.DocumentModel.ActivePieceTable.IsComment) {
				return Service.CreateCommand(commandId);
			}
			else {
				return base.CreateCommand(commandId);
			}
		}
		public override RichEditCommand CreateSharedCommand(RichEditCommandId commandId) {
			return new RichEditWithCommentsSharedCommand(CommentControl, MainControl, commandId, Service);
		}
	}
	#endregion
	#region RichEditWithCommentsSharedCommand
	public class RichEditWithCommentsSharedCommand : RichEditCommentBaseCommand {
		public RichEditWithCommentsSharedCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(commentControl, control, commandId, service) {
		}
		protected DocumentModel MainDocumentModel { get { return MainControl.ActiveView.DocumentModel; } }
		public override DevExpress.XtraRichEdit.Localization.XtraRichEditStringId DescriptionStringId {
			get { return Command.DescriptionStringId; }
		}
		public override DevExpress.XtraRichEdit.Localization.XtraRichEditStringId MenuCaptionStringId {
			get { return Command.MenuCaptionStringId; }
		}
		public override string ImageName {
			get { return Command.ImageName; }
		}
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			CommentControl.LockSelectionChangedEvent();
			Command.ForceExecute(state);
			if (MainDocumentModel.ActivePieceTable.IsComment) {
				CommentCommand.ForceExecute(state);
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(CommentControl, MainControl);
				helper.SetSelectionInCommentControl();
			}
			CommentControl.UnlockSelectionChangedEvent();
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			Command.UpdateUIState(state);
		}
		public override DevExpress.Utils.Commands.ICommandUIState CreateDefaultCommandUIState() {
			return Command.CreateDefaultCommandUIState();
		}
		protected RichEditCommand CreateCommandCore(RichEditCommandId commandId, IRichEditControl control, RichEditCommandFactoryService service) {
			ConstructorInfo ci;
			if (service.CommandConstructorTable.TryGetValue(commandId, out ci))
				return (RichEditCommand)ci.Invoke(new object[] { control });
			else
				return null;
		}
		public override void Execute() {
			CommentControl.LockSelectionChangedEvent();
			Command.Execute();
			if (MainDocumentModel.ActivePieceTable.IsComment) {
				CommentCommand.Execute();
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(CommentControl, MainControl);
				helper.SetSelectionInCommentControl();
			}
			CommentControl.UnlockSelectionChangedEvent();
		}
	}
	#endregion
	#region RichEditWithCommentsInsertTextCommand
	public class RichEditWithCommentsInsertTextCommand : RichEditWithCommentsSharedCommand, IInsertTextCommand {
		public RichEditWithCommentsInsertTextCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(commentControl, control, commandId, service) {
		}
		public override CommandSourceType CommandSourceType {
			get { return ((IInsertTextCommand)Command).CommandSourceType; }
			set {
				((IInsertTextCommand)CommentCommand).CommandSourceType = value;
				((IInsertTextCommand)Command).CommandSourceType = value;
			}
		}
		public string Text {
			get {
				return ((IInsertTextCommand)Command).Text;
			}
			set {
				((IInsertTextCommand)CommentCommand).Text = value;
				((IInsertTextCommand)Command).Text = value;
			}
		}
		public override void Execute() {
			CommentControl.LockSelectionChangedEvent();
			base.Execute();
			CommentControl.UnlockSelectionChangedEvent();
		}
	}
	#endregion
	#region RichEditWithCommentsInsertSymbolCommand
	public class RichEditWithCommentsInsertSymbolCommand : RichEditWithCommentsSharedCommand, IInsertSymbolCommand {
		public RichEditWithCommentsInsertSymbolCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(commentControl, control, commandId, service) {
		}
		public SymbolProperties SymbolInfo {
			get {
				return ((IInsertSymbolCommand)Command).SymbolInfo;
			}
			set {
				((IInsertSymbolCommand)CommentCommand).SymbolInfo = value;
				((IInsertSymbolCommand)Command).SymbolInfo = value;
			}
		}
	}
	#endregion
	#region RichEditWithCommentsUndoRedoCommandHelper
	public class RichEditWithCommentsUndoRedoCommandHelper {
		internal bool IsCommentUndoRedoCore(HistoryItem current) {
			if (current == null)
				return false;
			if (IsCommentItem(current))
				return true;
			RichEditCompositeHistoryItem richEditCompositeHistoryItem = current as RichEditCompositeHistoryItem;
			if (richEditCompositeHistoryItem != null) {
				int countItems = richEditCompositeHistoryItem.Count;
				for (int i = countItems - 1; i >= 0; i--) {
					RichEditHistoryItem subItem = richEditCompositeHistoryItem.Items[i] as RichEditHistoryItem;
					if (subItem != null && subItem.PieceTable.IsComment)
						return true;
					if (IsCommentItem(subItem))
						return true;
				}
			}
			IPieceTableHistoryItem richEditItem = current as IPieceTableHistoryItem;
			if (richEditItem != null && richEditItem.PieceTable.IsComment)
				return true;
			HistoryItem item = current as HistoryItem;
			if (item != null && ((DocumentModel)item.DocumentModel).ActivePieceTable.IsComment)
				return true;
			return false;
		}
		bool IsCommentItem(HistoryItem item) {
			if (item is InsertCommentHistoryItem)
				return true;
			if (item is DeleteCommentHistoryItem)
				return true;
			if (item is ChangeCommentPropertyHistoryItem)
				return true;
			return false;
		}
	}
	#endregion
	#region RichEditWithCommentsUndoCommand
	public class RichEditWithCommentsUndoCommand : RichEditWithCommentsSharedCommand {
		public RichEditWithCommentsUndoCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(commentControl, control, commandId, service) {
		}
		public override void Execute() {
			CommentControl.UnsubscribeShowReviewingPaneEvent();
			CommentControl.LockSelectionChangedEvent();
			if (IsCommentUndo())
				CommentCommand.Execute();
			Command.Execute();
			CommentControl.UnlockSelectionChangedEvent();
			CommentControl.SubscribeShowReviewingPaneEvent();
		}
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			CommentControl.UnsubscribeShowReviewingPaneEvent();
			CommentControl.LockSelectionChangedEvent();
			if (IsCommentUndo())
				CommentCommand.ForceExecute(state);
			Command.ForceExecute(state);
			CommentControl.UnlockSelectionChangedEvent();
			CommentControl.SubscribeShowReviewingPaneEvent();
		}
		bool IsCommentUndo() {
			DocumentModel documentModel = Control.InnerControl.DocumentModel;
			HistoryItem current = documentModel.History.Current;
			RichEditWithCommentsUndoRedoCommandHelper helper = new RichEditWithCommentsUndoRedoCommandHelper();
			return helper.IsCommentUndoRedoCore(current);
		}
	}
	#endregion
	#region RichEditWithCommentsRedoCommand
	public class RichEditWithCommentsRedoCommand : RichEditWithCommentsSharedCommand {
		public RichEditWithCommentsRedoCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(commentControl, control, commandId, service) {
		}
		public override void Execute() {
			CommentControl.UnsubscribeShowReviewingPaneEvent();
			CommentControl.LockSelectionChangedEvent();
			if (Command.CanExecute()) {
				if (IsCommentRedo())
					CommentCommand.Execute();
				Command.Execute();
			}
			CommentControl.UnlockSelectionChangedEvent();
			CommentControl.SubscribeShowReviewingPaneEvent();
		}
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			CommentControl.UnsubscribeShowReviewingPaneEvent();
			CommentControl.LockSelectionChangedEvent();
			if (IsCommentRedo())
				CommentCommand.ForceExecute(state);
			Command.ForceExecute(state);
			CommentControl.UnlockSelectionChangedEvent();
			CommentControl.SubscribeShowReviewingPaneEvent();
		}
		bool IsCommentRedo() {
			DocumentModel documentModel = Control.InnerControl.DocumentModel;
			int currentIndex = documentModel.History.CurrentIndex + 1;
			HistoryItem current = documentModel.History.Items[currentIndex];
			RichEditWithCommentsUndoRedoCommandHelper helper = new RichEditWithCommentsUndoRedoCommandHelper();
			return helper.IsCommentUndoRedoCore(current);
		}
	}
	#endregion
	#region  RichEditWithCommentsResizeInlinePictureCommand
	public class RichEditWithCommentsResizeInlinePictureCommand : ResizeInlinePictureCommand
	{
		InnerCommentControl commentControl;
		ResizeInlinePictureCommand commentCommand;
		ResizeInlinePictureCommand command;
		public RichEditWithCommentsResizeInlinePictureCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(control) 
		{
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		#region Properties
		protected RichEditControl RichEditControl { get { return commentControl.RichEditControl; } }
		public override Rectangle ObjectActualBounds
		{
			get
			{
				return command.ObjectActualBounds;
			}
			set
			{
				command.ObjectActualBounds = value;
			}
		}
		public override IRectangularObject RectangularObject
		{
			get
			{
				return commentCommand.RectangularObject;
			}
			set
			{
				command.RectangularObject = value;
			}
		}
		#endregion
		private ResizeInlinePictureCommand CreateCommandCore(RichEditControl control)
		{
			return new ResizeInlinePictureCommand(control);
		}
		public override void Execute()
		{
			commentControl.LockSelectionChangedEvent();
			command.Execute();
			commentControl.GenerateMainDocumentComments(true);		  
			commentControl.UnlockSelectionChangedEvent();
		}
	}
	#endregion
	#region RichEditCommentCommandFactoryService
	public class RichEditCommentCommandFactoryService : RichEditCommentCommandFactoryServiceBase {
		public RichEditCommentCommandFactoryService(InnerCommentControl commentControl, RichEditControl control, RichEditCommandFactoryService service)
			: base(commentControl, control, service) {
		}
		#region IRichEditCommandFactoryService Members
		public override RichEditCommand CreateCommand(RichEditCommandId commandId) {
#if !WPF
			if (MainControl.IsDisposed || CommentControl.IsDisposed)
				return null;
#endif
			if (commandId == RichEditCommandId.InsertText || commandId == RichEditCommandId.OvertypeText)
				return new RichEditCommentInsertTextCommand(CommentControl, MainControl, commandId, Service);
			return base.CreateCommand(commandId);
		}
		public override RichEditCommand CreateSharedCommand(RichEditCommandId commandId) {
			return new RichEditCommentSharedCommand(CommentControl, MainControl, commandId, Service);
		}
		#endregion
	}
	#endregion
	#region RichEditCommentCommandHelper
	public class RichEditCommentCommandHelper {
		InnerCommentControl commentControl;
		RichEditControl control;
		public RichEditCommentCommandHelper(InnerCommentControl commentControl, RichEditControl control) {
			this.commentControl = commentControl;
			this.control = control;
		}
		DocumentModel CommentDocumentModel { get { return commentControl.DocumentModel; } }
		DocumentModel MainDocumentModel { get { return control.ActiveView.DocumentModel; } }
		internal void SetSelection() {
			Bookmark bookmark = commentControl.GetBookmarkFromSelection();
			Comment comment = commentControl.FindCommentFromSelection(bookmark);
			if (comment != null) {
				ChangeActivePieceTable(comment);
				SetSelectionCore(bookmark);
				control.ScrollToCaret(-1);
			}
		}
		internal void ChangeActivePieceTable(Comment comment) {
			SectionIndex sectionIndex = MainDocumentModel.FindSectionIndex(comment.End);
			ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(control, comment.Content.PieceTable, MainDocumentModel.Sections[sectionIndex], 0);
			command.Execute();
		}
		void SetSelectionCore(Bookmark bookmark) {
			control.BeginUpdate();
			Selection selection = MainDocumentModel.Selection;
			MainDocumentModel.BeginUpdate();
			selection.ClearMultiSelection();
			Selection sourceSelection = commentControl.DocumentModel.Selection;
			int sourceSelectionItemsCount = sourceSelection.Items.Count;
			selection.Items.Clear();
			for (int i = 0; i < sourceSelectionItemsCount; i++) {
				SelectionItem item = new SelectionItem(selection.PieceTable);
				item.Start = sourceSelection.Items[i].Start - (bookmark.Start - DocumentLogPosition.Zero);
				item.End = sourceSelection.Items[i].End - (bookmark.Start - DocumentLogPosition.Zero);
				selection.Items.Add(item);
			}
			if (sourceSelection.SelectedCells.IsNotEmpty)
				selection.SelectedCells = commentControl.DocumentModel.Selection.SelectedCells.CloneWithShift(selection.PieceTable, -(bookmark.Start - DocumentLogPosition.Zero));
			MainDocumentModel.ApplyChangesCore(MainDocumentModel.MainPieceTable, DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw, RunIndex.DontCare, RunIndex.DontCare);
			MainDocumentModel.EndUpdate();
			control.EndUpdate();
		}
		internal void SetSelectionInCommentControl() {
			if (MainDocumentModel.ActivePieceTable.IsComment)
				SetSelectionInCommentControlCore();
			else
				commentControl.ClearSelectionInCommentControl();
		}
		void SetSelectionInCommentControlCore() {
			CommentCaretPosition position = control.ActiveView.CaretPosition as CommentCaretPosition;
			if (position != null)
				commentControl.SetCursor(control.InnerControl.GetCommentViewInfoFromCaretPosition(position), false, MainDocumentModel.Selection.Start, MainDocumentModel.Selection.End);
		}
		internal void RestoreSelectionInCommentControlAndControl(DocumentLogPosition start, DocumentLogPosition end) {
			RestoreSelectionInCommentControl(start, end);
			SetSelection();
		}
		void RestoreSelectionInCommentControl(DocumentLogPosition start, DocumentLogPosition end) {
			Selection selection = CommentDocumentModel.Selection;
			CommentDocumentModel.BeginUpdate();
			selection.Start = start;
			selection.End = end;
			CommentDocumentModel.EndUpdate();
		}
		internal void CopyFromReviewingPaneCore(Bookmark bookmark, Comment comment, PieceTable sourcePieceTable) {
			DocumentModel intermediateModel = sourcePieceTable.DocumentModel.CreateNew(false, false);
			DocumentLogPosition startLogPosition = bookmark.Start;
			DocumentLogPosition endLogPosition = bookmark.End - 1;
			DocumentModelCopyOptions options = new DocumentModelCopyOptions(startLogPosition, endLogPosition - startLogPosition + 1);
			DocumentModelCopyCommand copyCommand = commentControl.DocumentModel.CreateDocumentModelCopyCommand(sourcePieceTable, intermediateModel, options);
			copyCommand.SuppressFieldsUpdate = true;
			copyCommand.Execute();
			intermediateModel.ActivePieceTable.RangePermissions.Clear();
			PieceTable commentPieceTable = comment.Content.PieceTable;
			DocumentLogPosition start = commentPieceTable.DocumentStartLogPosition;
			int length = commentPieceTable.DocumentEndLogPosition - start;
			commentPieceTable.DeleteContent(start, length, start + length >= commentPieceTable.DocumentEndLogPosition);
			PieceTableInsertContentConvertedToDocumentModelCommand command = new PieceTableInsertContentConvertedToDocumentModelCommand(commentPieceTable, intermediateModel, start, false);
			command.Execute();
		}
		internal void CopyTablePropertiesFromCommentControl() {
			Bookmark bookmark = commentControl.GetBookmarkFromSelection();
			if (bookmark != null) {
				Comment comment = commentControl.FindCommentFromSelection(bookmark);
				if (comment != null) {
					DocumentModel targetModel = comment.Content.PieceTable.DocumentModel;
					Selection targetSelection = targetModel.Selection;
					DocumentModel sourceModel = commentControl.DocumentModel;
					Selection sourceSelection = sourceModel.Selection;
					if (sourceSelection.SelectedCells.IsNotEmpty)
						sourceSelection.SelectedCells.CopyProperties(targetSelection.SelectedCells);
				}
			}
		}
	}
	#endregion
	#region RichEditCommentSharedCommand
	public class RichEditCommentSharedCommand : RichEditCommentBaseCommand {
		public RichEditCommentSharedCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(commentControl, control, commandId, service) {
		}
		public override DevExpress.XtraRichEdit.Localization.XtraRichEditStringId DescriptionStringId {
			get { return CommentCommand.DescriptionStringId; }
		}
		public override DevExpress.XtraRichEdit.Localization.XtraRichEditStringId MenuCaptionStringId {
			get { return CommentCommand.MenuCaptionStringId; }
		}
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			CommentControl.LockSelectionChangedEvent();
			CommentCommand.ForceExecute(state);
			Command.ForceExecute(state);
			RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(CommentControl, MainControl);
			helper.SetSelection();
			CommentControl.UnlockSelectionChangedEvent();
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			CommentCommand.UpdateUIState(state);
		}
		public override DevExpress.Utils.Commands.ICommandUIState CreateDefaultCommandUIState() {
			return CommentCommand.CreateDefaultCommandUIState();
		}
		public override void Execute() {
			ICommandUIState state = CreateDefaultCommandUIState();
			UpdateUIState(state);
			if (state.Visible && state.Enabled) {
				CommentControl.LockSelectionChangedEvent();
				CommentCommand.Execute();
				Command.Execute();
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(CommentControl, MainControl);
				helper.SetSelection();
				CommentControl.UnlockSelectionChangedEvent();
			}
		}
	}
	#endregion
	#region RichEditCommentInsertTextCommand
	public class RichEditCommentInsertTextCommand : RichEditCommentSharedCommand, IInsertTextCommand {
		public RichEditCommentInsertTextCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(commentControl, control, commandId, service) {
		}
		public string Text {
			get {
				return ((IInsertTextCommand)CommentCommand).Text;
			}
			set {
				((IInsertTextCommand)CommentCommand).Text = value;
				((IInsertTextCommand)Command).Text = value;
			}
		}
		public override CommandSourceType CommandSourceType {
			get { return ((IInsertTextCommand)Command).CommandSourceType; }
			set {
				((IInsertTextCommand)CommentCommand).CommandSourceType = value;
				((IInsertTextCommand)Command).CommandSourceType = value;
			}
		}
		public override void Execute() {
			CommentControl.LockSelectionChangedEvent();
			base.Execute();
			CommentControl.UnlockSelectionChangedEvent();
		}
	}
	#endregion
	#region RichEditCommentInsertSymbolCommand
	public class RichEditCommentInsertSymbolCommand : RichEditCommentSharedCommand, IInsertSymbolCommand {
		public RichEditCommentInsertSymbolCommand(InnerCommentControl commentControl, RichEditControl control, RichEditCommandId commandId, RichEditCommandFactoryService service)
			: base(commentControl, control, commandId, service) {
		}
		public SymbolProperties SymbolInfo {
			get {
				return ((IInsertSymbolCommand)Command).SymbolInfo;
			}
			set {
				((IInsertSymbolCommand)CommentCommand).SymbolInfo = value;
				((IInsertSymbolCommand)Command).SymbolInfo = value;
			}
		}
	}
	#endregion
	#region RichEditCommentShowSymbolFormCommand
	public class RichEditCommentShowSymbolFormCommand : ShowSymbolFormCommand {
		InnerCommentControl commentControl;
		ShowSymbolFormCommand commentCommand;
		public RichEditCommentShowSymbolFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ShowSymbolFormCommand CreateCommandCore(InnerCommentControl control) {
			return new ShowSymbolFormCommand(control);
		}
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			commentCommand.ForceExecute(state);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return commentCommand.CreateDefaultCommandUIState();
		}
	}
	#endregion
	#region RichEditCommentShowPasteSpecialFormCommand
	public class RichEditCommentShowPasteSpecialFormCommand : ShowPasteSpecialFormCommand { 
		InnerCommentControl commentControl;
		ShowPasteSpecialFormCommand commentCommand;
		ShowPasteSpecialFormCommand command;
		public RichEditCommentShowPasteSpecialFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ShowPasteSpecialFormCommand CreateCommandCore(IRichEditControl control) {
			return new ShowPasteSpecialFormCommand(control);
		}
		protected internal override void ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
			commentControl.ShowPasteSpecialForm(properties, callback, callbackData);
		}
		protected internal override void ShowPasteSpecialFormCallback(PasteSpecialInfo properties, object callbackData) {
			DocumentLogPosition start = commentControl.DocumentModel.Selection.Start;
			DocumentLogPosition end = commentControl.DocumentModel.Selection.End;
			commentControl.LockSelectionChangedEvent();
			commentCommand.ShowPasteSpecialFormCallback(properties, callbackData);
			RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
			helper.RestoreSelectionInCommentControlAndControl(start, end);
			command.ShowPasteSpecialFormCallback(properties, callbackData);
			commentControl.UnlockSelectionChangedEvent();
		}
		public override DevExpress.Utils.Commands.ICommandUIState CreateDefaultCommandUIState() {
			return commentCommand.CreateDefaultCommandUIState();
		}
	}
	#endregion
	#region RichEditCommentShowFontFormCommand
	public class RichEditCommentShowFontFormCommand : ShowFontFormCommand {
		InnerCommentControl commentControl;
		ShowFontFormCommand commentCommand;
		ShowFontFormCommand command;
		public RichEditCommentShowFontFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ShowFontFormCommand CreateCommandCore(IRichEditControl control) {
			return new ShowFontFormCommand(control);
		}
		protected internal override void ShowFontForm(DevExpress.XtraRichEdit.Model.MergedCharacterProperties characterProperties, ShowFontFormCallback callback, object callbackData) {
			commentControl.ShowFontForm(characterProperties, callback, callbackData);
		}
		protected internal override void ShowFontFormCallback(DevExpress.XtraRichEdit.Model.MergedCharacterProperties properties, object callbackData) {
			commentCommand.ShowFontFormCallback(properties, callbackData);
			command.ShowFontFormCallback(properties, callbackData);
		}
	}
	#endregion
	#region RichEditCommentShowParagraphFormCommand
	public class RichEditCommentShowParagraphFormCommand : ShowParagraphFormCommand {
		InnerCommentControl commentControl;
		ShowParagraphFormCommand commentCommand;
		ShowParagraphFormCommand command;
		public RichEditCommentShowParagraphFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ShowParagraphFormCommand CreateCommandCore(IRichEditControl control) {
			return new ShowParagraphFormCommand(control);
		}
		internal override void ShowParagraphForm(MergedParagraphProperties paragraphProperties, ShowParagraphFormCallback callback, object callbackData) {
			commentControl.ShowParagraphForm(paragraphProperties, callback, callbackData);
		}
		protected internal override void ShowParagraphFormCallback(MergedParagraphProperties properties, object callbackData) {
			commentCommand.ShowParagraphFormCallback(properties, callbackData);
			command.ShowParagraphFormCallback(properties, callbackData);
		}
		public override DevExpress.Utils.Commands.ICommandUIState CreateDefaultCommandUIState() {
			return commentCommand.CreateDefaultCommandUIState();
		}
	}
	#endregion
	#region RichEditCommentShowNumberingListFormCommand
	public class RichEditCommentShowNumberingListFormCommand : ShowNumberingListFormCommand {
		InnerCommentControl commentControl;
		ShowNumberingListFormCommand commentCommand;
		public RichEditCommentShowNumberingListFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ShowNumberingListFormCommand CreateCommandCore(RichEditControl control) {
			return new ShowNumberingListFormCommand(control);
		}
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			commentCommand.ForceExecute(state);
		}
		public override void Execute() {
			commentControl.DocumentModel.BeginUpdate();
			try {
				commentControl.LockSelectionChangedEvent();
				commentControl.UnsubscribeShowReviewingPaneEvent();
				DocumentLogPosition start = commentControl.DocumentModel.Selection.Start;
				DocumentLogPosition end = commentControl.DocumentModel.Selection.End;
				base.Execute();
				Bookmark bookmark = commentControl.GetBookmarkFromSelection();
				if (bookmark != null) {
					Comment comment = commentControl.FindCommentFromSelection(bookmark);
					if (comment != null) {
						PieceTable sourcePieceTable = commentControl.DocumentModel.MainPieceTable;
						RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
						helper.CopyFromReviewingPaneCore(bookmark, comment, sourcePieceTable);
						NumberingListsCopy(comment.Content.PieceTable.DocumentModel, sourcePieceTable);
						helper.RestoreSelectionInCommentControlAndControl(start, end);
					}
				}
				commentControl.UnlockSelectionChangedEvent();
				commentControl.SubscribeShowReviewingPaneEvent();
			}
			finally {
				commentControl.DocumentModel.EndUpdate();
			}
		}
		public override DevExpress.Utils.Commands.ICommandUIState CreateDefaultCommandUIState() {
			return commentCommand.CreateDefaultCommandUIState();
		}
		public override void UpdateUIState(DevExpress.Utils.Commands.ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		void NumberingListsCopy(DocumentModel targetModel, PieceTable sourcePieceTable) {
			targetModel.BeginUpdate();
			NumberingListCopy(targetModel, sourcePieceTable);
			AbstractNumberingListCopy(targetModel, sourcePieceTable);
			targetModel.EndUpdate();
		}
		void NumberingListCopy(DocumentModel targetModel, PieceTable sourcePieceTable) {
			for (NumberingListIndex index = new NumberingListIndex(0); index < new NumberingListIndex(targetModel.NumberingLists.Count); index++) {
				NumberingList list = targetModel.NumberingLists[index];
				for (NumberingListIndex sourceIndex = new NumberingListIndex(0); sourceIndex < new NumberingListIndex(sourcePieceTable.DocumentModel.NumberingLists.Count); sourceIndex++) {
					if (targetModel.NumberingLists[index].Id == sourcePieceTable.DocumentModel.NumberingLists[sourceIndex].Id)
						list.CopyFromViaHistory(sourcePieceTable.DocumentModel.NumberingLists[sourceIndex]);
				}
			}
		}
		void AbstractNumberingListCopy(DocumentModel targetModel, PieceTable sourcePieceTable) {
			for (AbstractNumberingListIndex index = new AbstractNumberingListIndex(0); index < new AbstractNumberingListIndex(targetModel.AbstractNumberingLists.Count); index++) {
				AbstractNumberingList list = targetModel.AbstractNumberingLists[index];
				for (AbstractNumberingListIndex sourceIndex = new AbstractNumberingListIndex(0); sourceIndex < new AbstractNumberingListIndex(sourcePieceTable.DocumentModel.AbstractNumberingLists.Count); sourceIndex++) {
					if (targetModel.AbstractNumberingLists[index].Id == sourcePieceTable.DocumentModel.AbstractNumberingLists[sourceIndex].Id)
						list.CopyFromViaHistory(sourcePieceTable.DocumentModel.AbstractNumberingLists[sourceIndex]);
				}
			}
		}
	}
	#endregion
	#region RichEditCommentShowHyperlinkFormCommand
	public class RichEditCommentShowHyperlinkFormCommand : ShowHyperlinkFormCommand {
		InnerCommentControl commentControl;
		ShowHyperlinkFormCommand commentCommand;
		public RichEditCommentShowHyperlinkFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ShowHyperlinkFormCommand CreateCommandCore(InnerCommentControl commentControl) {
			return new ShowHyperlinkFormCommand(commentControl);
		}
		protected internal override void CreateCommands() {
			Commands.Add(new SelectTextForHyperlinkCommand(Control));
			Commands.Add(new RichEditCommentShowHyperlinkFormCoreCommand((InnerCommentControl)Control));
		}
	}
	#endregion
	#region RichEditCommentEditHyperlinkCommand
	public class RichEditCommentEditHyperlinkCommand : RichEditCommentShowHyperlinkFormCommand {
		public RichEditCommentEditHyperlinkCommand(InnerCommentControl commentControl)
			: base(commentControl) {
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.EditHyperlink; } }
	}
	#endregion
	#region RichEditCommentCreateHyperlinkContextMenuItemCommand
	public class RichEditCommentCreateHyperlinkContextMenuItemCommand :  RichEditCommentShowHyperlinkFormCommand {
		public RichEditCommentCreateHyperlinkContextMenuItemCommand(InnerCommentControl commentControl)
			: base(commentControl) {
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.CreateHyperlink; } }
	}
	#endregion
	#region RichEditCommentShowHyperlinkFormCoreCommand
	public class RichEditCommentShowHyperlinkFormCoreCommand : ShowHyperlinkFormCoreCommand {
		InnerCommentControl commentControl;
		RichEditControl control;
		ShowHyperlinkFormCoreCommand commentCommand;
		ShowHyperlinkFormCoreCommand command;
		public RichEditCommentShowHyperlinkFormCoreCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.control = commentControl.RichEditControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(control);
		}
		private ShowHyperlinkFormCoreCommand CreateCommandCore(RichEditControl control) {
			return new ShowHyperlinkFormCoreCommand(control);
		}
		protected internal override void CreateHyperlink(HyperlinkInfo hyperlinkInfo, TextToDisplaySource source, RunInfo runInfo, string text) {
			command.CreateHyperlink(hyperlinkInfo, source, control.DocumentModel.Selection.Interval, text);
			commentCommand.CreateHyperlink(hyperlinkInfo, source, runInfo, text);
		}
		protected internal override void ChangeHyperlink(HyperlinkInfo hyperlinkInfo, TextToDisplaySource source, RunInfo runInfo, string text) {
			command.ChangeHyperlink(hyperlinkInfo, source, control.DocumentModel.Selection.Interval, text);
			commentCommand.ChangeHyperlink(hyperlinkInfo, source, runInfo, text);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
	}
	#endregion
	#region RichEditCommentShowBookmarkFormCommand
	public class RichEditCommentShowBookmarkFormCommand : ShowBookmarkFormCommand {
		InnerCommentControl commentControl;
		ShowBookmarkFormCommand commentCommand;
		public RichEditCommentShowBookmarkFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		protected RichEditControl RichEditControl { get { return commentControl.RichEditControl; } }
		private ShowBookmarkFormCommand CreateCommandCore(InnerCommentControl commentControl) {
			return new ShowBookmarkFormCommand(commentControl);
		}
		protected internal override void ExecuteCore() {
			RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentControl.LockSelectionChangedEvent();
				commentControl.UnsubscribeShowReviewingPaneEvent();
				DocumentLogPosition start = commentControl.DocumentModel.Selection.Start;
				DocumentLogPosition end = commentControl.DocumentModel.Selection.End;
				commentCommand.ExecuteCore();
				Bookmark bookmark = commentControl.GetBookmarkFromSelection();
				if (bookmark != null) {
					Comment comment = commentControl.FindCommentFromSelection(bookmark);
					if (comment != null) {
						PieceTable sourcePieceTable = commentControl.DocumentModel.MainPieceTable;
						RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
						helper.CopyFromReviewingPaneCore(bookmark, comment, sourcePieceTable);
						helper.RestoreSelectionInCommentControlAndControl(start, end);
					}
				}
				commentControl.UnlockSelectionChangedEvent();
				commentControl.SubscribeShowReviewingPaneEvent();
				SetBookmarkOptionsVisibility();
			}
			finally {
				RichEditControl.DocumentModel.EndUpdate();
			}
		}
		void SetBookmarkOptionsVisibility() {
			commentControl.DocumentModel.BookmarkOptions.Visibility = RichEditBookmarkVisibility.Visible;
			RichEditControl.DocumentModel.BookmarkOptions.Visibility = RichEditBookmarkVisibility.Visible;
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
	}
	#endregion
	#region RichEditCommentCreateBookmarkContextMenuItemCommand
	public class RichEditCommentCreateBookmarkContextMenuItemCommand :  RichEditCommentShowBookmarkFormCommand {
		public RichEditCommentCreateBookmarkContextMenuItemCommand(InnerCommentControl commentControl)
			: base(commentControl) {
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.CreateBookmark; } }
	}
	#endregion
	#region RichEditCommentShowEditStyleFormCommand
	public class RichEditCommentShowEditStyleFormCommand : ShowEditStyleFormCommand {
		InnerCommentControl commentControl;
		ShowEditStyleFormCommand commentCommand;
		ShowEditStyleFormCommand command;
		public RichEditCommentShowEditStyleFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		protected RichEditControl RichEditControl { get { return commentControl.RichEditControl; } }
		private ShowEditStyleFormCommand CreateCommandCore(RichEditControl commentControl) {
			return new ShowEditStyleFormCommand(commentControl);
		}
		public override void ForceExecute(ICommandUIState state) {
			RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ForceExecute(state);
				Bookmark bookmark = commentControl.GetBookmarkFromSelection();
				if (bookmark != null) {
					Comment comment = commentControl.FindCommentFromSelection(bookmark);
					if (comment != null) {
						DocumentModel targetModel = comment.Content.PieceTable.DocumentModel;
						DocumentModel sourceModel = commentControl.DocumentModel;
						DocumentModelCopyCommand.CopyStyles(targetModel, sourceModel, true);
					}
				}
			}
			finally {
				RichEditControl.DocumentModel.EndUpdate();
			}
		}
		protected internal override void ShowEditStyleFormCallback(IStyle sourceStyle, IStyle targetStyle) {
			commentCommand.ShowEditStyleFormCallback(sourceStyle, targetStyle);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
	}
	#endregion
	#region RichEditCommentsFindCommand
	public class RichEditCommentFindCommand : FindCommand {
		InnerCommentControl commentControl;
		FindCommand commentCommand;
		FindCommand command;
		public RichEditCommentFindCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		protected RichEditControl RichEditControl { get { return commentControl.RichEditControl; } }
		private FindCommand CreateCommandCore(RichEditControl commentControl) {
			return new FindCommand(commentControl);
		}
		protected override void ShowForm(string searchString) {
#if WPF
			commentControl.ShowSearchForm(searchString);
#else
			commentControl.ShowSearchForm();
#endif
		}
	}
	#endregion
	#region RichEditCommentsReplaceCommand
	public class RichEditCommentReplaceCommand : ReplaceCommand {
		InnerCommentControl commentControl;
		ReplaceCommand commentCommand;
		public RichEditCommentReplaceCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		protected RichEditControl RichEditControl { get { return commentControl.RichEditControl; } }
		private ReplaceCommand CreateCommandCore(InnerCommentControl commentControl) {
			return new ReplaceCommand(commentControl);
		}
		protected override void ShowForm(string searchString) {
			commentControl.ShowReplaceForm();
		}
	}
	#endregion
	#region RichEditCommentsReplaceInnerCommand
	public class RichEditCommentReplaceInnerCommand : ReplaceInnerCommand {
		InnerCommentControl commentControl;
		ReplaceInnerCommand commentCommand;
		ReplaceInnerCommand command;
		public RichEditCommentReplaceInnerCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		#region Properties
		protected RichEditControl RichEditControl { get { return commentControl.RichEditControl; } }
		public override DocumentLogPosition StartPosition {
			get {
				return commentCommand.StartPosition;
			}
			set {
				commentCommand.StartPosition = value;
				command.StartPosition = value;
			}
		}
		public override int Length {
			get {
				return commentCommand.Length;
			}
			set {
				commentCommand.Length = value;
				command.Length = value;
			}
		}
		public override string Replacement {
			get {
				return commentCommand.Replacement;
			}
			set {
				commentCommand.Replacement = value;
				command.Replacement = value;
			}
		}
		#endregion
		private ReplaceInnerCommand CreateCommandCore(RichEditControl commentControl) {
			return new ReplaceInnerCommand(commentControl);
		}
		public override void Execute() {
			commentControl.LockSelectionChangedEvent();
			RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
			helper.RestoreSelectionInCommentControlAndControl(commentCommand.StartPosition, commentCommand.StartPosition);
			commentCommand.Execute();
			command.StartPosition = RichEditControl.DocumentModel.Selection.Start;
			command.Execute();
			commentControl.UnlockSelectionChangedEvent();
		}
	}
	#endregion
	#region RichEditCommentsInsertPictureCommand
	public class RichEditCommentInsertPictureCommand : InsertPictureCommand {
		InnerCommentControl commentControl;
		InsertPictureCommand commentCommand;
		public RichEditCommentInsertPictureCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private InsertPictureCommand CreateCommandCore(InnerCommentControl commentControl) {
			return new InsertPictureCommand(commentControl);
		}
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			commentCommand.ForceExecuteCore(state);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertPictureCoreCommand(Control);
		}
	}
	#endregion
	#region RichEditCommentsInsertPictureInnerCommand
	public class RichEditCommentInsertPictureInnerCommand : InsertPictureInnerCommand {
		InnerCommentControl commentControl;
		InsertPictureInnerCommand commentCommand;
		InsertPictureInnerCommand command;
		public RichEditCommentInsertPictureInnerCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		#region Properties
		protected RichEditControl RichEditControl { get { return commentControl.RichEditControl; } }
		public override DocumentLogPosition StartPosition {
			get {
				return commentCommand.StartPosition;
			}
			set {
				commentCommand.StartPosition = value;
				command.StartPosition = value;
			}
		}
		public override int Scale {
			get {
				return commentCommand.Scale;
			}
			set {
				commentCommand.Scale = value;
				command.Scale = value;
			}
		}
		public override OfficeImage NewImage {
			get {
				return commentCommand.NewImage;
			}
			set {
				commentCommand.NewImage = value;
				command.NewImage = value;
			}
		}
		#endregion
		private InsertPictureInnerCommand CreateCommandCore(RichEditControl control) {
			return new InsertPictureInnerCommand(control);
		}
		public override void Execute() {
			commentControl.LockSelectionChangedEvent();
			commentCommand.Execute();
			command.StartPosition = RichEditControl.DocumentModel.Selection.End;
			command.Execute();
			commentControl.UnlockSelectionChangedEvent();
		}
	}
	#endregion
	#region RichEditCommentsInsertTableCommand
	public class RichEditCommentInsertTableCommand : InsertTableCommand {
		InnerCommentControl commentControl;
		InsertTableCommand commentCommand;
		public RichEditCommentInsertTableCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private InsertTableCommand CreateCommandCore(InnerCommentControl commentControl) {
			return new InsertTableCommand(commentControl);
		}
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertTableCoreCommand((InnerCommentControl)Control);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentCommand.ForceExecute(state);
		}
		public override void InsertTable(int rows, int columns) {
			commentCommand.InsertTable(rows, columns);
		}
	}
	#endregion
	#region RichEditCommentsInsertTableInnerCommand
	public class RichEditCommentInsertTableInnerCommand : InsertTableInnerCommand {
		InnerCommentControl commentControl;
		InsertTableInnerCommand commentCommand;
		InsertTableInnerCommand command;
		public RichEditCommentInsertTableInnerCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		#region Properties
		protected RichEditControl RichEditControl { get { return commentControl.RichEditControl; } }
		public override DocumentLogPosition StartPosition {
			get {
				return commentCommand.StartPosition;
			}
			set {
				commentCommand.StartPosition = value;
				command.StartPosition = value;
			}
		}
		public override int RowCount {
			get {
				return commentCommand.RowCount;
			}
			set {
				commentCommand.RowCount = value;
				command.RowCount = value;
			}
		}
		public override int ColumnCount {
			get {
				return commentCommand.ColumnCount;
			}
			set {
				commentCommand.ColumnCount = value;
				command.ColumnCount = value;
			}
		}
		public override int ColumnWidth {
			get {
				return commentCommand.ColumnWidth;
			}
			set {
				commentCommand.ColumnWidth = value;
				command.ColumnWidth = value;
			}
		}
		#endregion
		private InsertTableInnerCommand CreateCommandCore(RichEditControl control) {
			return new InsertTableInnerCommand(control);
		}
		public override void Execute() {
			commentControl.LockSelectionChangedEvent();
			commentCommand.Execute();
			command.StartPosition = RichEditControl.DocumentModel.Selection.End;
			command.Execute();
			commentControl.UnlockSelectionChangedEvent();
		}
	}
	#endregion
	#region RichEditCommentShowTablePropertiesFormCommand
	public class RichEditCommentShowTablePropertiesFormCommand : ShowTablePropertiesFormCommand {
		InnerCommentControl commentControl;
		ShowTablePropertiesFormCommand commentCommand;
		public RichEditCommentShowTablePropertiesFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		protected RichEditControl RichEditControl { get { return commentControl.RichEditControl; } }
		private ShowTablePropertiesFormCommand CreateCommandCore(RichEditControl control) {
			return new ShowTablePropertiesFormCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		protected internal override void ExecuteCore() {
			RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ExecuteCore();
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.CopyTablePropertiesFromCommentControl();
			}
			finally {
				RichEditControl.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region RichEditCommentShowTablePropertiesFormMenuCommand
	public class RichEditCommentShowTablePropertiesFormMenuCommand : ShowTablePropertiesFormMenuCommand {
		InnerCommentControl commentControl;
		ShowTablePropertiesFormMenuCommand commentCommand;
		public RichEditCommentShowTablePropertiesFormMenuCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		protected RichEditControl RichEditControl { get { return commentControl.RichEditControl; } }
		private ShowTablePropertiesFormMenuCommand CreateCommandCore(RichEditControl control) {
			return new ShowTablePropertiesFormMenuCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		protected internal override void ExecuteCore() {
			RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ExecuteCore();
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.CopyTablePropertiesFromCommentControl();
			}
			finally {
				RichEditControl.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region RichEditCommentShowDeleteTableCellsFormCommand
	public class RichEditCommentShowDeleteTableCellsFormCommand : ShowDeleteTableCellsFormCommand {
		InnerCommentControl commentControl;
		ShowDeleteTableCellsFormCommand commentCommand;
		ShowDeleteTableCellsFormCommand command;
		public RichEditCommentShowDeleteTableCellsFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ShowDeleteTableCellsFormCommand CreateCommandCore(RichEditControl control) {
			return new ShowDeleteTableCellsFormCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		protected internal override void ShowInsertDeleteTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			commentControl.ShowDeleteTableCellsForm(parameters, callback, callbackData);
		}
		protected internal override void ShowInsertDeleteTableCellsFormCallback(TableCellsParameters parameters, object callbackData) {
			commentCommand.ShowInsertDeleteTableCellsFormCallback(parameters, callbackData);
			command.ShowInsertDeleteTableCellsFormCallback(parameters, callbackData);
		}
	}
	#endregion
	#region RichEditCommentShowDeleteTableCellsFormMenuCommand
	public class RichEditCommentShowDeleteTableCellsFormMenuCommand : ShowDeleteTableCellsFormMenuCommand {
		InnerCommentControl commentControl;
		ShowDeleteTableCellsFormMenuCommand commentCommand;
		ShowDeleteTableCellsFormMenuCommand command;
		public RichEditCommentShowDeleteTableCellsFormMenuCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ShowDeleteTableCellsFormMenuCommand CreateCommandCore(RichEditControl control) {
			return new ShowDeleteTableCellsFormMenuCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		protected internal override void ShowInsertDeleteTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			commentControl.ShowDeleteTableCellsForm(parameters, callback, callbackData);
		}
		protected internal override void ShowInsertDeleteTableCellsFormCallback(TableCellsParameters parameters, object callbackData) {
			commentCommand.ShowInsertDeleteTableCellsFormCallback(parameters, callbackData);
			command.ShowInsertDeleteTableCellsFormCallback(parameters, callbackData);
		}
	}
	#endregion
	#region RichEditCommentShowSplitTableCellsFormCommand
	public class RichEditCommentShowSplitTableCellsFormCommand : ShowSplitTableCellsFormCommand {
		InnerCommentControl commentControl;
		ShowSplitTableCellsFormCommand commentCommand;
		ShowSplitTableCellsFormCommand command;
		public RichEditCommentShowSplitTableCellsFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ShowSplitTableCellsFormCommand CreateCommandCore(RichEditControl control) {
			return new ShowSplitTableCellsFormCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		protected internal override void ShowSplitTableCellsForm(SplitTableCellsParameters parameters, ShowSplitTableCellsFormCallback callback, object callbackData) {
			commentControl.ShowSplitTableCellsForm(parameters, callback, callbackData);
		}
		protected internal override void ShowSplitTableCellsFormCallback(SplitTableCellsParameters parameters, object callbackData) {
			commentCommand.ShowSplitTableCellsFormCallback(parameters, callbackData);
			command.ShowSplitTableCellsFormCallback(parameters, callbackData);
		}
	}
	#endregion
	#region RichEditCommentShowSplitTableCellsFormMenuCommand
	public class RichEditCommentShowSplitTableCellsFormMenuCommand : ShowSplitTableCellsFormMenuCommand {
		InnerCommentControl commentControl;
		ShowSplitTableCellsFormMenuCommand commentCommand;
		ShowSplitTableCellsFormMenuCommand command;
		public RichEditCommentShowSplitTableCellsFormMenuCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ShowSplitTableCellsFormMenuCommand CreateCommandCore(RichEditControl control) {
			return new ShowSplitTableCellsFormMenuCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		protected internal override void ShowSplitTableCellsForm(SplitTableCellsParameters parameters, ShowSplitTableCellsFormCallback callback, object callbackData) {
			commentControl.ShowSplitTableCellsForm(parameters, callback, callbackData);
		}
		protected internal override void ShowSplitTableCellsFormCallback(SplitTableCellsParameters parameters, object callbackData) {
			commentCommand.ShowSplitTableCellsFormCallback(parameters, callbackData);
			command.ShowSplitTableCellsFormCallback(parameters, callbackData);
		}
	}
	#endregion
	#region RichEditCommentChangeCurrentBorderRepositoryItemColorCommand
	public class RichEditCommentChangeCurrentBorderRepositoryItemColorCommand : ChangeCurrentBorderRepositoryItemColorCommand {
		InnerCommentControl commentControl;
		ChangeCurrentBorderRepositoryItemColorCommand commentCommand;
		ChangeCurrentBorderRepositoryItemColorCommand command;
		public RichEditCommentChangeCurrentBorderRepositoryItemColorCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ChangeCurrentBorderRepositoryItemColorCommand CreateCommandCore(RichEditControl control) {
			return new ChangeCurrentBorderRepositoryItemColorCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentCommand.ForceExecute(state);
			command.ForceExecute(state);
		}
	}
	#endregion
	#region RichEditCommentChangeCurrentBorderRepositoryItemLineStyleCommand
	public class RichEditCommentChangeCurrentBorderRepositoryItemLineStyleCommand : ChangeCurrentBorderRepositoryItemLineStyleCommand {
		InnerCommentControl commentControl;
		ChangeCurrentBorderRepositoryItemLineStyleCommand commentCommand;
		ChangeCurrentBorderRepositoryItemLineStyleCommand command;
		public RichEditCommentChangeCurrentBorderRepositoryItemLineStyleCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ChangeCurrentBorderRepositoryItemLineStyleCommand CreateCommandCore(RichEditControl control) {
			return new ChangeCurrentBorderRepositoryItemLineStyleCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentCommand.ForceExecute(state);
			command.ForceExecute(state);
		}
	}
	#endregion
	#region RichEditCommentChangeCurrentBorderRepositoryItemLineThicknessCommand
	public class RichEditCommentChangeCurrentBorderRepositoryItemLineThicknessCommand : ChangeCurrentBorderRepositoryItemLineThicknessCommand {
		InnerCommentControl commentControl;
		ChangeCurrentBorderRepositoryItemLineThicknessCommand commentCommand;
		ChangeCurrentBorderRepositoryItemLineThicknessCommand command;
		public RichEditCommentChangeCurrentBorderRepositoryItemLineThicknessCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ChangeCurrentBorderRepositoryItemLineThicknessCommand CreateCommandCore(RichEditControl control) {
			return new ChangeCurrentBorderRepositoryItemLineThicknessCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentCommand.ForceExecute(state);
			command.ForceExecute(state);
		}
	}
	#endregion
	#region RichEditCommentChangeTableCellsShadingCommand
	public class RichEditCommentChangeTableCellsShadingCommand : ChangeTableCellsShadingCommand {
		InnerCommentControl commentControl;
		ChangeTableCellsShadingCommand commentCommand;
		ChangeTableCellsShadingCommand command;
		public RichEditCommentChangeTableCellsShadingCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ChangeTableCellsShadingCommand CreateCommandCore(RichEditControl control) {
			return new ChangeTableCellsShadingCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentCommand.ForceExecute(state);
			command.ForceExecute(state);
		}
	}
	#endregion
	#region RichEditCommentChangeTableStyleCommand
	public class RichEditCommentChangeTableStyleCommand : ChangeTableStyleCommand {
		InnerCommentControl commentControl;
		ChangeTableStyleCommand commentCommand;
		public RichEditCommentChangeTableStyleCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ChangeTableStyleCommand CreateCommandCore(RichEditControl control) {
			return new ChangeTableStyleCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			CopyStyles();
			commentControl.RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ForceExecute(state);
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.CopyTablePropertiesFromCommentControl();
			}
			finally {
				commentControl.RichEditControl.DocumentModel.EndUpdate();
			}
		}
		void CopyStyles() {
			DocumentModel targetModel = commentControl.DocumentModel;
			DocumentModel sourceModel = commentControl.RichEditControl.DocumentModel;
			targetModel.BeginUpdate();
			DocumentModelCopyCommand.CopyStylesCore(targetModel.TableStyles, sourceModel.TableStyles, true);
			targetModel.EndUpdate();
		}
	}
	#endregion
	#region RichEditCommentToggleFirstRowCommand
	public class RichEditCommentToggleFirstRowCommand : ToggleFirstRowCommand {
		InnerCommentControl commentControl;
		ToggleFirstRowCommand commentCommand;
		public RichEditCommentToggleFirstRowCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ToggleFirstRowCommand CreateCommandCore(RichEditControl control) {
			return new ToggleFirstRowCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentControl.RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ForceExecute(state);
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.CopyTablePropertiesFromCommentControl();
			}
			finally {
				commentControl.RichEditControl.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region RichEditCommentToggleLastRowCommand
	public class RichEditCommentToggleLastRowCommand : ToggleLastRowCommand {
		InnerCommentControl commentControl;
		ToggleLastRowCommand commentCommand;
		public RichEditCommentToggleLastRowCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ToggleLastRowCommand CreateCommandCore(RichEditControl control) {
			return new ToggleLastRowCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentControl.RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ForceExecute(state);
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.CopyTablePropertiesFromCommentControl();
			}
			finally {
				commentControl.RichEditControl.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region RichEditCommentToggleBandedRowsCommand
	public class RichEditCommentToggleBandedRowsCommand : ToggleBandedRowsCommand {
		InnerCommentControl commentControl;
		ToggleBandedRowsCommand commentCommand;
		public RichEditCommentToggleBandedRowsCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ToggleBandedRowsCommand CreateCommandCore(RichEditControl control) {
			return new ToggleBandedRowsCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentControl.RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ForceExecute(state);
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.CopyTablePropertiesFromCommentControl();
			}
			finally {
				commentControl.RichEditControl.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region RichEditCommentToggleFirstColumnCommand
	public class RichEditCommentToggleFirstColumnCommand : ToggleFirstColumnCommand {
		InnerCommentControl commentControl;
		ToggleFirstColumnCommand commentCommand;
		public RichEditCommentToggleFirstColumnCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ToggleFirstColumnCommand CreateCommandCore(RichEditControl control) {
			return new ToggleFirstColumnCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentControl.RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ForceExecute(state);
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.CopyTablePropertiesFromCommentControl();
			}
			finally {
				commentControl.RichEditControl.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region RichEditCommentToggleLastColumnCommand
	public class RichEditCommentToggleLastColumnCommand : ToggleLastColumnCommand {
		InnerCommentControl commentControl;
		ToggleLastColumnCommand commentCommand;
		public RichEditCommentToggleLastColumnCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ToggleLastColumnCommand CreateCommandCore(RichEditControl control) {
			return new ToggleLastColumnCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentControl.RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ForceExecute(state);
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.CopyTablePropertiesFromCommentControl();
			}
			finally {
				commentControl.RichEditControl.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region RichEditCommentToggleBandedColumnsCommand
	public class RichEditCommentToggleBandedColumnsCommand : ToggleBandedColumnsCommand {
		InnerCommentControl commentControl;
		ToggleBandedColumnsCommand commentCommand;
		public RichEditCommentToggleBandedColumnsCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ToggleBandedColumnsCommand CreateCommandCore(RichEditControl control) {
			return new ToggleBandedColumnsCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			commentControl.RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ForceExecute(state);
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.CopyTablePropertiesFromCommentControl();
			}
			finally {
				commentControl.RichEditControl.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region RichEditCommentShowTableOptionsFormCommand
	public class RichEditCommentShowTableOptionsFormCommand : ShowTableOptionsFormCommand {
		InnerCommentControl commentControl;
		ShowTableOptionsFormCommand commentCommand;
		public RichEditCommentShowTableOptionsFormCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
		}
		private ShowTableOptionsFormCommand CreateCommandCore(RichEditControl control) {
			return new ShowTableOptionsFormCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		protected internal override void ExecuteCore() {
			commentControl.RichEditControl.DocumentModel.BeginUpdate();
			try {
				commentCommand.ExecuteCore();
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.CopyTablePropertiesFromCommentControl();
			}
			finally {
				commentControl.RichEditControl.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region RichEditCommentImeUndoCommand
	public class RichEditCommentImeUndoCommand : ImeUndoCommand {
		InnerCommentControl commentControl;
		ImeUndoCommand commentCommand;
		ImeUndoCommand command;
		public RichEditCommentImeUndoCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ImeUndoCommand CreateCommandCore(RichEditControl control) {
			return new ImeUndoCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void Execute() {
			commentControl.UnsubscribeShowReviewingPaneEvent();
			commentControl.LockSelectionChangedEvent();
			if (commentControl.RichEditControl.DocumentModel.ActivePieceTable.IsComment)
				commentCommand.Execute();
			command.Execute();
			commentControl.UnlockSelectionChangedEvent();
			commentControl.SubscribeShowReviewingPaneEvent();
		}
		bool IsCommentUndo() {
			DocumentModel documentModel = commentControl.RichEditControl.InnerControl.DocumentModel;
			HistoryItem current = documentModel.History.Current;
			RichEditWithCommentsUndoRedoCommandHelper helper = new RichEditWithCommentsUndoRedoCommandHelper();
			return helper.IsCommentUndoRedoCore(current);
		}
	}
#endregion
#region RichEditCommentDeleteNonEmptySelectionCommand
	public class RichEditCommentDeleteNonEmptySelectionCommand : DeleteNonEmptySelectionCommand {
		InnerCommentControl commentControl;
		DeleteNonEmptySelectionCommand commentCommand;
		DeleteNonEmptySelectionCommand command;
		public RichEditCommentDeleteNonEmptySelectionCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private DeleteNonEmptySelectionCommand CreateCommandCore(RichEditControl control) {
			return new DeleteNonEmptySelectionCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void Execute() {
			commentControl.LockSelectionChangedEvent();
			command.Execute();
			if (commentControl.RichEditControl.DocumentModel.ActivePieceTable.IsComment) {
				commentCommand.Execute();
				RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(commentControl, commentControl.RichEditControl);
				helper.SetSelectionInCommentControl();
			}
			commentControl.UnlockSelectionChangedEvent();
		}
	}
#endregion
#region RichEditCommentResetMergingCommand
	public class RichEditCommentResetMergingCommand : ResetMergingCommand {
		InnerCommentControl commentControl;
		ResetMergingCommand commentCommand;
		ResetMergingCommand command;
		public RichEditCommentResetMergingCommand(InnerCommentControl commentControl)
			: base(commentControl) {
			this.commentControl = commentControl;
			this.commentCommand = CreateCommandCore(commentControl);
			this.command = CreateCommandCore(commentControl.RichEditControl);
		}
		private ResetMergingCommand CreateCommandCore(RichEditControl control) {
			return new ResetMergingCommand(control);
		}
		public override void UpdateUIState(ICommandUIState state) {
			commentCommand.UpdateUIState(state);
		}
		public override void Execute() {
			commentControl.UnsubscribeShowReviewingPaneEvent();
			commentControl.LockSelectionChangedEvent();
			if (commentControl.RichEditControl.DocumentModel.ActivePieceTable.IsComment)
				commentCommand.Execute();
			command.Execute();
			commentControl.UnlockSelectionChangedEvent();
			commentControl.SubscribeShowReviewingPaneEvent();
		}
	}
#endregion
}
