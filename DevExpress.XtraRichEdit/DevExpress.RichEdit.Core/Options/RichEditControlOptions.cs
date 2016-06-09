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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Import;
using DevExpress.Utils.Serializing;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit {
	#region RichEditControlOptionsBase (abstract class)
	[ComVisible(true)]
	public abstract class RichEditControlOptionsBase : RichEditNotificationOptions, IDisposable {
		internal static readonly string ShowHiddenTextOptions = "ShowHiddenText";
		#region Fields
		readonly HyperlinkOptions hyperlinkOptions;
		readonly InnerRichEditDocumentServer documentServer;
		VerticalRulerOptions verticalRuler;
		HorizontalRulerOptions horizontalRuler;
		#endregion
		protected RichEditControlOptionsBase(InnerRichEditDocumentServer documentServer) {
			Guard.ArgumentNotNull(documentServer, "documentServer");
			this.documentServer = documentServer;
			this.hyperlinkOptions = new HyperlinkOptions();
			this.verticalRuler = new VerticalRulerOptions();
			this.horizontalRuler = new HorizontalRulerOptions();
			SubscribeInnerOptionsEvents();
		}
		#region Properties
		protected internal InnerRichEditDocumentServer DocumentServer { get { return documentServer; } }
		#region ShowHiddenText
		[NotifyParentProperty(true), DefaultValue(false)]
		internal bool ShowHiddenText {
			get { return FormattingMarkVisibility.ShowHiddenText; }
			set {
				if (ShowHiddenText == value)
					return;
				FormattingMarkVisibility.ShowHiddenText = value;
				OnChanged(ShowHiddenTextOptions, !value, value);
			}
		}
		#endregion
		#region Fields
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseFields"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FieldOptions Fields {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.FieldOptions;
			}
		}
		#endregion
		#region MailMerge
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseMailMerge"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditMailMergeOptions MailMerge {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.MailMergeOptions;
			}
		}
		#endregion
		#region DocumentCapabilities
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseDocumentCapabilities"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DocumentCapabilitiesOptions DocumentCapabilities {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.DocumentCapabilities;
			}
		}
		#endregion
		#region RichEditLayoutOptions
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseLayout"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditLayoutOptions Layout {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.LayoutOptions;
			}
		}
		#endregion
		#region BehaviorOptions
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseBehavior"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditBehaviorOptions Behavior {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.BehaviorOptions; ;
			}
		}
		#endregion
		#region Export
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseExport"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditDocumentExportOptions Export {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.DocumentExportOptions;
			}
		}
		#endregion
		#region Import
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseImport"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditDocumentImportOptions Import {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.DocumentImportOptions;
			}
		}
		#endregion
		#region Hyperlinks
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseHyperlinks"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HyperlinkOptions Hyperlinks { get { return hyperlinkOptions; } }
		#endregion
		#region Bookmarks
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseBookmarks"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BookmarkOptions Bookmarks {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.BookmarkOptions;
			}
		}
		#endregion
		#region RangePermissions
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseRangePermissions"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RangePermissionOptions RangePermissions {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.RangePermissionOptions;
			}
		}
		#endregion
		#region Comments
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseComments"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CommentOptions Comments {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.CommentOptions;
			}
		}
		#endregion
		#region Search
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseSearch"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DocumentSearchOptions Search
		{
			get
			{
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.SearchOptions;
			}
		}
		#endregion
		#region DocumentSaveOptions
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseDocumentSaveOptions"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DocumentSaveOptions DocumentSaveOptions {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.DocumentSaveOptions;
			}
		}
		#endregion
		#region FormattingMarkVisibility
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseFormattingMarkVisibility"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FormattingMarkVisibilityOptions FormattingMarkVisibility {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.FormattingMarkVisibilityOptions;
			}
		}
		#endregion
		#region TableOptions
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseTableOptions"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TableOptions TableOptions {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.TableOptions;
			}
		}
		#endregion
		#region VerticalRuler
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseVerticalRuler"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public VerticalRulerOptions VerticalRuler { get { return verticalRuler; } }
		#endregion
		#region HorizontalRuler
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseHorizontalRuler"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HorizontalRulerOptions HorizontalRuler { get { return horizontalRuler; } }
		#endregion
		#region Authentication
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseAuthentication"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AuthenticationOptions Authentication {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.AuthenticationOptions;
			}
		}
		#endregion
		#region AutoCorrect
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseAutoCorrect"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AutoCorrectOptions AutoCorrect {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.AutoCorrectOptions;
			}
		}
		#endregion
		#region Printing
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBasePrinting"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PrintingOptions Printing {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.PrintingOptions;
			}
		}
		#endregion
		#region CopyPaste
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseCopyPaste"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CopyPasteOptions CopyPaste
		{
			get
			{
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.CopyPasteOptions;
			}
		}
		#endregion
		#region SpellChecker
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditControlOptionsBaseSpellChecker"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpellCheckerOptions SpellChecker {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.SpellCheckerOptions;
			}
		} 
		#endregion
		#endregion
		protected internal virtual void SubscribeInnerOptionsEvents() {
			if (Fields != null)
				Fields.Changed += OnInnerOptionsChanged;
			if (MailMerge != null)
				MailMerge.Changed += OnInnerOptionsChanged;
			if (DocumentCapabilities != null)
				DocumentCapabilities.Changed += OnInnerOptionsChanged;
			if (Behavior != null)
				Behavior.Changed += OnInnerOptionsChanged;
			if (Bookmarks != null)
				Bookmarks.Changed += OnInnerOptionsChanged;
			if (Search != null)
				Search.Changed += OnInnerOptionsChanged;
			if (FormattingMarkVisibility != null)
				FormattingMarkVisibility.Changed += OnInnerOptionsChanged;
			if (Hyperlinks != null)
				Hyperlinks.Changed += OnInnerOptionsChanged;
			if (VerticalRuler != null)
				VerticalRuler.Changed += OnInnerOptionsChanged;
			if (HorizontalRuler != null)
				HorizontalRuler.Changed += OnInnerOptionsChanged;
			if (Authentication != null)
				Authentication.Changed += OnInnerOptionsChanged;
			if (Layout != null)
				Layout.Changed += OnInnerOptionsChanged;
			if (Printing != null)
				Printing.Changed += OnInnerOptionsChanged;
			if (RangePermissions != null)
				RangePermissions.Changed += OnInnerOptionsChanged;
			if (Comments != null)
				Comments.Changed += OnInnerOptionsChanged;
			if (SpellChecker != null)
				SpellChecker.Changed += OnInnerOptionsChanged;
		}
		protected internal virtual void UnsubscribeInnerOptionsEvents() {
			if (Fields != null)
				Fields.Changed -= OnInnerOptionsChanged;
			if (MailMerge != null)
				MailMerge.Changed -= OnInnerOptionsChanged;
			if (DocumentCapabilities != null)
				DocumentCapabilities.Changed -= OnInnerOptionsChanged;
			if (Behavior != null)
				Behavior.Changed -= OnInnerOptionsChanged;
			if (Bookmarks != null)
				Bookmarks.Changed -= OnInnerOptionsChanged;
			if (Search != null)
				Search.Changed -= OnInnerOptionsChanged;
			if (FormattingMarkVisibility != null)
				FormattingMarkVisibility.Changed -= OnInnerOptionsChanged;
			if (Hyperlinks != null)
				Hyperlinks.Changed -= OnInnerOptionsChanged;
			if (VerticalRuler != null)
				VerticalRuler.Changed -= OnInnerOptionsChanged;
			if (HorizontalRuler != null)
				HorizontalRuler.Changed -= OnInnerOptionsChanged;
			if (Authentication != null)
				Authentication.Changed -= OnInnerOptionsChanged;
			if (Layout != null)
				Layout.Changed -= OnInnerOptionsChanged;
			if (Printing != null)
				Printing.Changed -= OnInnerOptionsChanged;
			if (RangePermissions != null)
				RangePermissions.Changed -= OnInnerOptionsChanged;
			if (Comments != null)
				Comments.Changed -= OnInnerOptionsChanged;
			if (SpellChecker != null)
				SpellChecker.Changed -= OnInnerOptionsChanged;
		}
		protected internal virtual void OnInnerOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged(e);
		}
		protected internal override void ResetCore() {
			if (DocumentSaveOptions != null)
				DocumentSaveOptions.Reset();
			if (Fields != null)
				Fields.Reset();
			if (MailMerge != null)
				MailMerge.Reset();
			if (Export != null)
				Export.Reset();
			if (Import != null)
				Import.Reset();
			if (Hyperlinks != null)
				Hyperlinks.Reset();
			if (DocumentCapabilities != null)
				DocumentCapabilities.Reset();
			if (Behavior != null)
				Behavior.Reset();
			if (Bookmarks != null)
				Bookmarks.Reset();
			if (Search != null)
				Search.Reset();
			if (FormattingMarkVisibility != null)
				FormattingMarkVisibility.Reset();
			if (VerticalRuler != null)
				VerticalRuler.Reset();
			if (HorizontalRuler != null)
				HorizontalRuler.Reset();
			if (Authentication != null)
				Authentication.Reset();
			if (Layout != null)
				Layout.Reset();
			if (Printing != null)
				Printing.Reset();
			if (SpellChecker != null)
				SpellChecker.Reset();
		}
		#region IDisposable Members
		public void Dispose() {
			UnsubscribeInnerOptionsEvents();
		}
		#endregion
	}
	#endregion
	#region RichEditControlCompatibility
	public static class RichEditControlCompatibility {
		const string defaultFontNameValue = "Calibri";
		const float defaultFontSizeValue = 11f;
		static string defaultFontName = defaultFontNameValue;
		static float defaultFontSize = defaultFontSizeValue;
		[DefaultValue(defaultFontNameValue), NotifyParentProperty(true)]
		public static string DefaultFontName {
			get { return defaultFontName; }
			set {
				if (defaultFontName == value)
					return;
				if (String.IsNullOrEmpty(value))
					Exceptions.ThrowArgumentException("DefaultFontName", value);
				defaultFontName = value;
			}
		}
		[DefaultValue(defaultFontSizeValue), NotifyParentProperty(true)]
		public static float DefaultFontSize {
			get {
				return defaultFontSize;
			}
			set {
				if (defaultFontSize == value)
					return;
				if (value <= 0)
					Exceptions.ThrowArgumentException("DefaultFontSize", value);
				defaultFontSize = value;
			}
		}
		internal static int DefaultDoubleFontSize {
			get { return (int)Math.Round(defaultFontSize * 2); }
		}
		[DefaultValue(false)]
		public static bool CompatibleRtfImportForRotatedImages { get; set; }
	}
	#endregion
}
