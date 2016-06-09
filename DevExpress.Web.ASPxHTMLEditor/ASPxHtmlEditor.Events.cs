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
using System.ComponentModel;
using DevExpress.XtraSpellChecker;
namespace DevExpress.Web.ASPxHtmlEditor {
	public partial class ASPxHtmlEditor {
		static readonly object HtmlChangedEventKey = new object();
		static readonly object HtmlCorrectingEventKey = new object();
		static readonly object EventValidation = new object();
		static readonly object ImageSelectorCustomThumbnailEventKey = new object();
		static readonly object ImageSelectorFolderCreatingEventKey = new object();
		static readonly object ImageSelectorItemRenamingEventKey = new object();
		static readonly object ImageSelectorItemDeletingEventKey = new object();
		static readonly object ImageSelectorItemMovingEventKey = new object();
		static readonly object ImageSelectorItemCopyingEventKey = new object();
		static readonly object ImageSelectorFileUploadingEventKey = new object();
		static readonly object ImageFileSavingEventKey = new object();
		static readonly object FlashSelectorCustomThumbnailEventKey = new object();
		static readonly object FlashSelectorFolderCreatingEventKey = new object();
		static readonly object FlashSelectorItemRenamingEventKey = new object();
		static readonly object FlashSelectorItemDeletingEventKey = new object();
		static readonly object FlashSelectorItemMovingEventKey = new object();
		static readonly object FlashSelectorItemCopyingEventKey = new object();
		static readonly object FlashSelectorFileUploadingEventKey = new object();
		static readonly object FlashFileSavingEventKey = new object();
		static readonly object VideoSelectorCustomThumbnailEventKey = new object();
		static readonly object VideoSelectorFolderCreatingEventKey = new object();
		static readonly object VideoSelectorItemRenamingEventKey = new object();
		static readonly object VideoSelectorItemDeletingEventKey = new object();
		static readonly object VideoSelectorItemMovingEventKey = new object();
		static readonly object VideoSelectorItemCopyingEventKey = new object();
		static readonly object VideoSelectorFileUploadingEventKey = new object();
		static readonly object VideoFileSavingEventKey = new object();
		static readonly object AudioSelectorCustomThumbnailEventKey = new object();
		static readonly object AudioSelectorFolderCreatingEventKey = new object();
		static readonly object AudioSelectorItemRenamingEventKey = new object();
		static readonly object AudioSelectorItemDeletingEventKey = new object();
		static readonly object AudioSelectorItemMovingEventKey = new object();
		static readonly object AudioSelectorItemCopyingEventKey = new object();
		static readonly object AudioSelectorFileUploadingEventKey = new object();
		static readonly object AudioFileSavingEventKey = new object();
		static readonly object DocumentSelectorCustomThumbnailEventKey = new object();
		static readonly object DocumentSelectorFolderCreatingEventKey = new object();
		static readonly object DocumentSelectorItemRenamingEventKey = new object();
		static readonly object DocumentSelectorItemDeletingEventKey = new object();
		static readonly object DocumentSelectorItemMovingEventKey = new object();
		static readonly object DocumentSelectorItemCopyingEventKey = new object();
		static readonly object DocumentSelectorFileUploadingEventKey = new object();
		static readonly object SpellCheckerWordAddedEventKey = new object();
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorHtmlChanged"),
#endif
		Category("Action")]
		public event EventHandler<EventArgs> HtmlChanged {
			add { Events.AddHandler(HtmlChangedEventKey, value); }
			remove { Events.RemoveHandler(HtmlChangedEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorHtmlCorrecting"),
#endif
		Category("Action")]
		public event EventHandler<HtmlCorrectingEventArgs> HtmlCorrecting {
			add { Events.AddHandler(HtmlCorrectingEventKey, value); }
			remove { Events.RemoveHandler(HtmlCorrectingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorCustomDataCallback"),
#endif
		Category("Action")]
		public event CustomDataCallbackEventHandler CustomDataCallback {
			add { Events.AddHandler(EventCustomDataCallback, value); }
			remove { Events.RemoveHandler(EventCustomDataCallback, value); }
		}
#if !SL
	[DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorValidation"),
#endif
		Category("Action")]
		public event EventHandler<HtmlEditorValidationEventArgs> Validation {
			add { Events.AddHandler(EventValidation, value); }
			remove { Events.RemoveHandler(EventValidation, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageSelectorCustomThumbnail"),
#endif
		Category("Action")]
		public event FileManagerThumbnailCreateEventHandler ImageSelectorCustomThumbnail {
			add { Events.AddHandler(ImageSelectorCustomThumbnailEventKey, value); }
			remove { Events.RemoveHandler(ImageSelectorCustomThumbnailEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageSelectorFolderCreating"),
#endif
		Category("Action")]
		public event FileManagerFolderCreateEventHandler ImageSelectorFolderCreating {
			add { Events.AddHandler(ImageSelectorFolderCreatingEventKey, value); }
			remove { Events.RemoveHandler(ImageSelectorFolderCreatingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageSelectorItemRenaming"),
#endif
		Category("Action")]
		public event FileManagerItemRenameEventHandler ImageSelectorItemRenaming {
			add { Events.AddHandler(ImageSelectorItemRenamingEventKey, value); }
			remove { Events.RemoveHandler(ImageSelectorItemRenamingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageSelectorItemDeleting"),
#endif
		Category("Action")]
		public event FileManagerItemDeleteEventHandler ImageSelectorItemDeleting {
			add { Events.AddHandler(ImageSelectorItemDeletingEventKey, value); }
			remove { Events.RemoveHandler(ImageSelectorItemDeletingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageSelectorItemMoving"),
#endif
		Category("Action")]
		public event FileManagerItemMoveEventHandler ImageSelectorItemMoving {
			add { Events.AddHandler(ImageSelectorItemMovingEventKey, value); }
			remove { Events.RemoveHandler(ImageSelectorItemMovingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageSelectorItemCopying"),
#endif
		Category("Action")]
		public event FileManagerItemCopyEventHandler ImageSelectorItemCopying {
			add { Events.AddHandler(ImageSelectorItemCopyingEventKey, value); }
			remove { Events.RemoveHandler(ImageSelectorItemCopyingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageSelectorFileUploading"),
#endif
		Category("Action")]
		public event FileManagerFileUploadEventHandler ImageSelectorFileUploading {
			add { Events.AddHandler(ImageSelectorFileUploadingEventKey, value); }
			remove { Events.RemoveHandler(ImageSelectorFileUploadingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageFileSaving"),
#endif
		Category("Action")]
		public event FileSavingEventHandler ImageFileSaving {
			add { Events.AddHandler(ImageFileSavingEventKey, value); }
			remove { Events.RemoveHandler(ImageFileSavingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorFlashSelectorCustomThumbnail"),
#endif
		Category("Action")]
		public event FileManagerThumbnailCreateEventHandler FlashSelectorCustomThumbnail {
			add { Events.AddHandler(FlashSelectorCustomThumbnailEventKey, value); }
			remove { Events.RemoveHandler(FlashSelectorCustomThumbnailEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorFlashSelectorFolderCreating"),
#endif
		Category("Action")]
		public event FileManagerFolderCreateEventHandler FlashSelectorFolderCreating {
			add { Events.AddHandler(FlashSelectorFolderCreatingEventKey, value); }
			remove { Events.RemoveHandler(FlashSelectorFolderCreatingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorFlashSelectorItemRenaming"),
#endif
		Category("Action")]
		public event FileManagerItemRenameEventHandler FlashSelectorItemRenaming {
			add { Events.AddHandler(FlashSelectorItemRenamingEventKey, value); }
			remove { Events.RemoveHandler(FlashSelectorItemRenamingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorFlashSelectorItemDeleting"),
#endif
		Category("Action")]
		public event FileManagerItemDeleteEventHandler FlashSelectorItemDeleting {
			add { Events.AddHandler(FlashSelectorItemDeletingEventKey, value); }
			remove { Events.RemoveHandler(FlashSelectorItemDeletingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorFlashSelectorItemMoving"),
#endif
		Category("Action")]
		public event FileManagerItemMoveEventHandler FlashSelectorItemMoving {
			add { Events.AddHandler(FlashSelectorItemMovingEventKey, value); }
			remove { Events.RemoveHandler(FlashSelectorItemMovingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorFlashSelectorItemCopying"),
#endif
		Category("Action")]
		public event FileManagerItemCopyEventHandler FlashSelectorItemCopying {
			add { Events.AddHandler(FlashSelectorItemCopyingEventKey, value); }
			remove { Events.RemoveHandler(FlashSelectorItemCopyingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorFlashSelectorFileUploading"),
#endif
		Category("Action")]
		public event FileManagerFileUploadEventHandler FlashSelectorFileUploading {
			add { Events.AddHandler(FlashSelectorFileUploadingEventKey, value); }
			remove { Events.RemoveHandler(FlashSelectorFileUploadingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorFlashFileSaving"),
#endif
		Category("Action")]
		public event FileSavingEventHandler FlashFileSaving {
			add { Events.AddHandler(FlashFileSavingEventKey, value); }
			remove { Events.RemoveHandler(FlashFileSavingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorVideoSelectorCustomThumbnail"),
#endif
		Category("Action")]
		public event FileManagerThumbnailCreateEventHandler VideoSelectorCustomThumbnail {
			add { Events.AddHandler(VideoSelectorCustomThumbnailEventKey, value); }
			remove { Events.RemoveHandler(VideoSelectorCustomThumbnailEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorVideoSelectorFolderCreating"),
#endif
		Category("Action")]
		public event FileManagerFolderCreateEventHandler VideoSelectorFolderCreating {
			add { Events.AddHandler(VideoSelectorFolderCreatingEventKey, value); }
			remove { Events.RemoveHandler(VideoSelectorFolderCreatingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorVideoSelectorItemRenaming"),
#endif
		Category("Action")]
		public event FileManagerItemRenameEventHandler VideoSelectorItemRenaming {
			add { Events.AddHandler(VideoSelectorItemRenamingEventKey, value); }
			remove { Events.RemoveHandler(VideoSelectorItemRenamingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorVideoSelectorItemDeleting"),
#endif
		Category("Action")]
		public event FileManagerItemDeleteEventHandler VideoSelectorItemDeleting {
			add { Events.AddHandler(VideoSelectorItemDeletingEventKey, value); }
			remove { Events.RemoveHandler(VideoSelectorItemDeletingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorVideoSelectorItemMoving"),
#endif
		Category("Action")]
		public event FileManagerItemMoveEventHandler VideoSelectorItemMoving {
			add { Events.AddHandler(VideoSelectorItemMovingEventKey, value); }
			remove { Events.RemoveHandler(VideoSelectorItemMovingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorVideoSelectorItemCopying"),
#endif
		Category("Action")]
		public event FileManagerItemCopyEventHandler VideoSelectorItemCopying {
			add { Events.AddHandler(VideoSelectorItemCopyingEventKey, value); }
			remove { Events.RemoveHandler(VideoSelectorItemCopyingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorVideoSelectorFileUploading"),
#endif
		Category("Action")]
		public event FileManagerFileUploadEventHandler VideoSelectorFileUploading {
			add { Events.AddHandler(VideoSelectorFileUploadingEventKey, value); }
			remove { Events.RemoveHandler(VideoSelectorFileUploadingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorVideoFileSaving"),
#endif
		Category("Action")]
		public event FileSavingEventHandler VideoFileSaving {
			add { Events.AddHandler(VideoFileSavingEventKey, value); }
			remove { Events.RemoveHandler(VideoFileSavingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAudioSelectorCustomThumbnail"),
#endif
		Category("Action")]
		public event FileManagerThumbnailCreateEventHandler AudioSelectorCustomThumbnail {
			add { Events.AddHandler(AudioSelectorCustomThumbnailEventKey, value); }
			remove { Events.RemoveHandler(AudioSelectorCustomThumbnailEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAudioSelectorFolderCreating"),
#endif
		Category("Action")]
		public event FileManagerFolderCreateEventHandler AudioSelectorFolderCreating {
			add { Events.AddHandler(AudioSelectorFolderCreatingEventKey, value); }
			remove { Events.RemoveHandler(AudioSelectorFolderCreatingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAudioSelectorItemRenaming"),
#endif
		Category("Action")]
		public event FileManagerItemRenameEventHandler AudioSelectorItemRenaming {
			add { Events.AddHandler(AudioSelectorItemRenamingEventKey, value); }
			remove { Events.RemoveHandler(AudioSelectorItemRenamingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAudioSelectorItemDeleting"),
#endif
		Category("Action")]
		public event FileManagerItemDeleteEventHandler AudioSelectorItemDeleting {
			add { Events.AddHandler(AudioSelectorItemDeletingEventKey, value); }
			remove { Events.RemoveHandler(AudioSelectorItemDeletingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAudioSelectorItemMoving"),
#endif
		Category("Action")]
		public event FileManagerItemMoveEventHandler AudioSelectorItemMoving {
			add { Events.AddHandler(AudioSelectorItemMovingEventKey, value); }
			remove { Events.RemoveHandler(AudioSelectorItemMovingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAudioSelectorItemCopying"),
#endif
		Category("Action")]
		public event FileManagerItemCopyEventHandler AudioSelectorItemCopying {
			add { Events.AddHandler(AudioSelectorItemCopyingEventKey, value); }
			remove { Events.RemoveHandler(AudioSelectorItemCopyingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAudioSelectorFileUploading"),
#endif
		Category("Action")]
		public event FileManagerFileUploadEventHandler AudioSelectorFileUploading {
			add { Events.AddHandler(AudioSelectorFileUploadingEventKey, value); }
			remove { Events.RemoveHandler(AudioSelectorFileUploadingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAudioFileSaving"),
#endif
		Category("Action")]
		public event FileSavingEventHandler AudioFileSaving {
			add { Events.AddHandler(AudioFileSavingEventKey, value); }
			remove { Events.RemoveHandler(AudioFileSavingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorDocumentSelectorCustomThumbnail"),
#endif
		Category("Action")]
		public event FileManagerThumbnailCreateEventHandler DocumentSelectorCustomThumbnail {
			add { Events.AddHandler(DocumentSelectorCustomThumbnailEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorCustomThumbnailEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorDocumentSelectorFolderCreating"),
#endif
		Category("Action")]
		public event FileManagerFolderCreateEventHandler DocumentSelectorFolderCreating {
			add { Events.AddHandler(DocumentSelectorFolderCreatingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorFolderCreatingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorDocumentSelectorItemRenaming"),
#endif
		Category("Action")]
		public event FileManagerItemRenameEventHandler DocumentSelectorItemRenaming {
			add { Events.AddHandler(DocumentSelectorItemRenamingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorItemRenamingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorDocumentSelectorItemDeleting"),
#endif
		Category("Action")]
		public event FileManagerItemDeleteEventHandler DocumentSelectorItemDeleting {
			add { Events.AddHandler(DocumentSelectorItemDeletingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorItemDeletingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorDocumentSelectorItemMoving"),
#endif
		Category("Action")]
		public event FileManagerItemMoveEventHandler DocumentSelectorItemMoving {
			add { Events.AddHandler(DocumentSelectorItemMovingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorItemMovingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorDocumentSelectorItemCopying"),
#endif
		Category("Action")]
		public event FileManagerItemCopyEventHandler DocumentSelectorItemCopying {
			add { Events.AddHandler(DocumentSelectorItemCopyingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorItemCopyingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorDocumentSelectorFileUploading"),
#endif
		Category("Action")]
		public event FileManagerFileUploadEventHandler DocumentSelectorFileUploading {
			add { Events.AddHandler(DocumentSelectorFileUploadingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorFileUploadingEventKey, value); }
		}
		[Category("Action")]
		public event WordAddedEventHandler SpellCheckerWordAdded {
			add { Events.AddHandler(SpellCheckerWordAddedEventKey, value); }
			remove { Events.RemoveHandler(SpellCheckerWordAddedEventKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseImageSelectorCustomThumbnail(FileManagerThumbnailCreateEventArgs args) {
			RaiseEvent(ImageSelectorCustomThumbnailEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseImageSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			RaiseEvent(ImageSelectorFolderCreatingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseImageSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			RaiseEvent(ImageSelectorItemRenamingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseImageSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			RaiseEvent(ImageSelectorItemDeletingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseImageSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			RaiseEvent(ImageSelectorItemMovingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseImageSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			RaiseEvent(ImageSelectorItemCopyingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseImageSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			RaiseEvent(ImageSelectorFileUploadingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseImageFileSaving(FileSavingEventArgs args) {
			RaiseEvent(ImageFileSavingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseFlashSelectorCustomThumbnail(FileManagerThumbnailCreateEventArgs args) {
			RaiseEvent(FlashSelectorCustomThumbnailEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseFlashSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			RaiseEvent(FlashSelectorFolderCreatingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseFlashSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			RaiseEvent(FlashSelectorItemRenamingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseFlashSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			RaiseEvent(FlashSelectorItemDeletingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseFlashSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			RaiseEvent(FlashSelectorItemMovingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseFlashSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			RaiseEvent(FlashSelectorItemCopyingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseFlashSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			RaiseEvent(FlashSelectorFileUploadingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseFlashFileSaving(FileSavingEventArgs args) {
			RaiseEvent(FlashFileSavingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseVideoSelectorCustomThumbnail(FileManagerThumbnailCreateEventArgs args) {
			RaiseEvent(VideoSelectorCustomThumbnailEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseVideoSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			RaiseEvent(VideoSelectorFolderCreatingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseVideoSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			RaiseEvent(VideoSelectorItemRenamingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseVideoSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			RaiseEvent(VideoSelectorItemDeletingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseVideoSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			RaiseEvent(VideoSelectorItemMovingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseVideoSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			RaiseEvent(VideoSelectorItemCopyingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseVideoSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			RaiseEvent(VideoSelectorFileUploadingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseVideoFileSaving(FileSavingEventArgs args) {
			RaiseEvent(VideoFileSavingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAudioSelectorCustomThumbnail(FileManagerThumbnailCreateEventArgs args) {
			RaiseEvent(AudioSelectorCustomThumbnailEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAudioSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			RaiseEvent(AudioSelectorFolderCreatingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAudioSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			RaiseEvent(AudioSelectorItemRenamingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAudioSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			RaiseEvent(AudioSelectorItemDeletingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAudioSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			RaiseEvent(AudioSelectorItemMovingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAudioSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			RaiseEvent(AudioSelectorItemCopyingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAudioSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			RaiseEvent(AudioSelectorFileUploadingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAudioFileSaving(FileSavingEventArgs args) {
			RaiseEvent(AudioFileSavingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseDocumentSelectorCustomThumbnail(FileManagerThumbnailCreateEventArgs args) {
			RaiseEvent(DocumentSelectorCustomThumbnailEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseDocumentSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			RaiseEvent(DocumentSelectorFolderCreatingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseDocumentSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			RaiseEvent(DocumentSelectorItemRenamingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseDocumentSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			RaiseEvent(DocumentSelectorItemDeletingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseDocumentSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			RaiseEvent(DocumentSelectorItemMovingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseDocumentSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			RaiseEvent(DocumentSelectorItemCopyingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseDocumentSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			RaiseEvent(DocumentSelectorFileUploadingEventKey, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		void RaiseSpellCheckerWordAdded(WordAddedEventArgs args) {
			RaiseEvent(SpellCheckerWordAddedEventKey, args);
		}
		protected virtual void OnHtmlChanged(EventArgs e) {
			RaiseEvent(HtmlChangedEventKey, e);
		}
		protected internal virtual void OnHtmlCorrecting(HtmlCorrectingEventArgs e) {
			RaiseEvent(HtmlCorrectingEventKey, e);
		}
		protected virtual void OnValidation(HtmlEditorValidationEventArgs e) {
			RaiseEvent(EventValidation, e);
		}
		protected void RaiseEvent(object key, EventArgs args) {
			Delegate handler = Events[key];
			if(handler != null)
				handler.DynamicInvoke(this, args);
		}
	}
}
