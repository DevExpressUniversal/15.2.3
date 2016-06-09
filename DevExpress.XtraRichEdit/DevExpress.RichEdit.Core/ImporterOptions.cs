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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit.Import {
	#region RtfDocumentImporterOptions
	[ComVisible(true)]
	public class RtfDocumentImporterOptions : DocumentImporterOptions {
		bool suppressLastParagraphDelete;
		bool copySingleCellAsText;
		bool pasteFromIe;
		bool ignoreDeletedText;
		protected internal override Encoding ActualEncoding {
			get { return base.ActualEncoding; }
			set {
			}
		}
		protected internal override DocumentFormat Format { get { return DocumentFormat.Rtf; } }
		protected internal bool SuppressLastParagraphDelete { get { return suppressLastParagraphDelete; } set { suppressLastParagraphDelete = value; } }
		protected internal bool CopySingleCellAsText { get { return copySingleCellAsText; } set { copySingleCellAsText = value; } }
		protected internal bool PasteFromIE { get { return pasteFromIe; } set { pasteFromIe = value; } }
		#region IgnoreDeletedText
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RtfDocumentImporterOptionsIgnoreDeletedText"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool IgnoreDeletedText {
			get { return ignoreDeletedText; }
			set {
				if (value == ignoreDeletedText)
					return;
				ignoreDeletedText = value;
				OnChanged("IgnoreDeletedText", !ignoreDeletedText, ignoreDeletedText);
			}
		}
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			SuppressLastParagraphDelete = false;
			CopySingleCellAsText = false;
			PasteFromIE = false;
			IgnoreDeletedText = false;
		}
		public override void CopyFrom(IImporterOptions value) {
			base.CopyFrom(value);
			RtfDocumentImporterOptions options = value as RtfDocumentImporterOptions;
			if (options != null) {
				this.SuppressLastParagraphDelete = options.SuppressLastParagraphDelete;
				this.CopySingleCellAsText = options.CopySingleCellAsText;
				this.PasteFromIE = options.PasteFromIE;
				this.IgnoreDeletedText = options.IgnoreDeletedText;
			}
		}
	}
	#endregion
	#region PlainTextDocumentImporterOptions
	[ComVisible(true)]
	public class PlainTextDocumentImporterOptions : DocumentImporterOptions {
		bool autoDetectEncoding;
		#region Encoding
#if !SL && !DXPORTABLE
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentImporterOptionsEncoding"),
#endif
TypeConverter(typeof(DevExpress.Office.Design.EncodingConverter))]
#endif
		public Encoding Encoding { get { return ActualEncoding; } set { ActualEncoding = value; } }
		protected internal virtual bool ShouldSerializeEncoding() {
			return !Object.Equals(DXEncoding.Default, Encoding);
		}
		protected internal virtual void ResetEncoding() {
			Encoding = DXEncoding.Default;
		}
		#endregion
		#region AutoDetectEncoding
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentImporterOptionsAutoDetectEncoding"),
#endif
DefaultValue(true)]
		public bool AutoDetectEncoding {
			get { return autoDetectEncoding; }
			set {
				if (value == autoDetectEncoding)
					return;
				autoDetectEncoding = value;
				OnChanged("AutoDetectEncoding", !value, value);
			}
		}
		#endregion
		#region UpdateField
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new UpdateFieldOptions UpdateField { get { return base.UpdateField; } }
		#endregion
		protected internal override DocumentFormat Format { get { return DocumentFormat.PlainText; } }
		protected internal override void ResetCore() {
			base.ResetCore();
			AutoDetectEncoding = true;
		}
		public override void CopyFrom(IImporterOptions value) {
			base.CopyFrom(value);
			PlainTextDocumentImporterOptions options = value as PlainTextDocumentImporterOptions;
			if (options != null)
				this.AutoDetectEncoding = options.AutoDetectEncoding;
		}
	}
	#endregion
	#region HtmlDocumentImporterOptions
	[ComVisible(true)]
	public class HtmlDocumentImporterOptions : DocumentImporterOptions {
		const int defaultTableCellSpacingDefaultValue = 15;
		const int defaultTableCellMargingDefaultValue = 15;
		bool defaultAsyncImageLoading = true;
		bool asyncImageLoading;
		bool autoDetectEncoding;
		bool ignoreMetaCharset;
		bool replaceSpaceWithNonBreakingSpaceInsidePre;
		bool ignoreFloatProperty;
		int defaultTableCellSpacing;
		int defaultTableCellMarging;
		#region Properties
		#region UpdateField
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new UpdateFieldOptions UpdateField { get { return base.UpdateField; } }
		#endregion
		#region Encoding
#if !SL && !DXPORTABLE
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentImporterOptionsEncoding"),
#endif
TypeConverter(typeof(DevExpress.Office.Design.EncodingConverter))]
#endif
		public Encoding Encoding { get { return ActualEncoding; } set { ActualEncoding = value; } }
		protected internal virtual bool ShouldSerializeEncoding() {
			return !Object.Equals(Encoding.UTF8, Encoding);
		}
		protected internal virtual void ResetEncoding() {
			Encoding = Encoding.UTF8;
		}
		#endregion
		#region AutoDetectEncoding
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentImporterOptionsAutoDetectEncoding"),
#endif
 DefaultValue(true)]
		public bool AutoDetectEncoding {
			get { return autoDetectEncoding; }
			set {
				if (value == autoDetectEncoding)
					return;
				autoDetectEncoding = value;
				OnChanged("AutoDetectEncoding", !value, value);
			}
		}
		#endregion
		#region ReplaceSpaceWithNonBreakingSpaceInsidePre
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentImporterOptionsReplaceSpaceWithNonBreakingSpaceInsidePre"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool ReplaceSpaceWithNonBreakingSpaceInsidePre {
			get { return replaceSpaceWithNonBreakingSpaceInsidePre; }
			set {
				if (value == replaceSpaceWithNonBreakingSpaceInsidePre)
					return;
				replaceSpaceWithNonBreakingSpaceInsidePre = value;
				OnChanged("ReplaceSpaceWithNonBreakingSpaceInsidePre", !value, value);
			}
		}
		#endregion
		#region IgnoreMetaCharset
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentImporterOptionsIgnoreMetaCharset"),
#endif
		DefaultValue(false)]
		public bool IgnoreMetaCharset {
			get { return ignoreMetaCharset; }
			set {
				if (ignoreMetaCharset == value)
					return;
				ignoreMetaCharset = value;
				OnChanged("IgnoreMetaCharset", !value, value);
			}
		}
		#endregion
		#region IgnoreFloatProperty
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentImporterOptionsIgnoreFloatProperty"),
#endif
		DefaultValue(false)]
		public bool IgnoreFloatProperty {
			get { return ignoreFloatProperty; }
			set {
				if (ignoreFloatProperty == value)
					return;
				ignoreFloatProperty = value;
				OnChanged("IgnoreFloatProperty", !value, value);
			}
		}
		#endregion
		internal bool DefaultAsyncImageLoading { get { return defaultAsyncImageLoading; } set { defaultAsyncImageLoading = value; } }
		#region AsyncImageLoading
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("HtmlDocumentImporterOptionsAsyncImageLoading")]
#endif
		public bool AsyncImageLoading {
			get { return asyncImageLoading; }
			set {
				if (asyncImageLoading == value)
					return;
				asyncImageLoading = value;
				OnChanged("AsyncImageLoading", !value, value);
			}
		}
		protected internal virtual bool ShouldSerializeAsyncImageLoading() {
			return AsyncImageLoading != DefaultAsyncImageLoading;
		}
		protected internal virtual void ResetAsyncImageLoading() {
			AsyncImageLoading = DefaultAsyncImageLoading;
		}
		#endregion
		protected internal override DocumentFormat Format { get { return DocumentFormat.Html; } }
		#region DefaultTableCellSpacing
		[DefaultValue(defaultTableCellSpacingDefaultValue)]
		protected internal int DefaultTableCellSpacing {
			get { return defaultTableCellSpacing; }
			set {
				if (defaultTableCellSpacing == value)
					return;
				int oldValue = defaultTableCellSpacing;
				defaultTableCellSpacing = value;
				OnChanged("DefaultTableCellSpacing", oldValue, value);
			}
		}
		#endregion
		#region DefaultTableCellMarging
		[DefaultValue(defaultTableCellMargingDefaultValue)]
		protected internal int DefaultTableCellMarging {
			get { return defaultTableCellMarging; }
			set {
				if (defaultTableCellMarging == value)
					return;
				int oldValue = defaultTableCellMarging;
				defaultTableCellMarging = value;
				OnChanged("DefaultTableCellMarging", oldValue, value);
			}
		}
		#endregion
		#endregion
		public override void CopyFrom(IImporterOptions value) {
			base.CopyFrom(value);
			HtmlDocumentImporterOptions options = value as HtmlDocumentImporterOptions;
			if (options != null) {
				this.DefaultAsyncImageLoading = options.DefaultAsyncImageLoading;
				this.AsyncImageLoading = options.AsyncImageLoading;
				this.AutoDetectEncoding = options.AutoDetectEncoding;
				this.IgnoreMetaCharset = options.IgnoreMetaCharset;
				this.DefaultTableCellSpacing = options.DefaultTableCellSpacing;
				this.DefaultTableCellMarging = options.DefaultTableCellMarging;
				this.ReplaceSpaceWithNonBreakingSpaceInsidePre = options.ReplaceSpaceWithNonBreakingSpaceInsidePre;
				this.IgnoreFloatProperty = options.IgnoreFloatProperty;
			}
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			Encoding = Encoding.UTF8;
			AutoDetectEncoding = DefaultAsyncImageLoading;
			IgnoreMetaCharset = false;
			AsyncImageLoading = true;
			DefaultTableCellSpacing = defaultTableCellSpacingDefaultValue;
			DefaultTableCellMarging = defaultTableCellMargingDefaultValue;
			ReplaceSpaceWithNonBreakingSpaceInsidePre = false;
			IgnoreFloatProperty = false;
		}
	}
	#endregion
	#region MhtDocumentImporterOptions
	[ComVisible(true)]
	public class MhtDocumentImporterOptions : HtmlDocumentImporterOptions {
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MhtDocumentImporterOptionsSourceUri")]
#endif
		public override string SourceUri {
			get { return String.Empty; }
			set {
			}
		}
		protected internal override DocumentFormat Format { get { return DocumentFormat.Mht; } }
		protected internal override bool ShouldSerializeEncoding() {
			return !Object.Equals(EmptyEncoding.Instance, Encoding);
		}
		protected internal override void ResetEncoding() {
			Encoding = EmptyEncoding.Instance;
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			Encoding = EmptyEncoding.Instance;
		}
	}
	#endregion
	#region OpenXmlDocumentImporterOptions
	[ComVisible(true)]
	public class OpenXmlDocumentImporterOptions : XmlBasedDocumentImporterOptions {
		bool ignoreDeletedText = true;
		bool ignoreInsertedText = true;
		protected internal override DocumentFormat Format { get { return DocumentFormat.OpenXml; } }
		#region IgnoreDeletedText
		[
		DefaultValue(true), NotifyParentProperty(true)]
		public bool IgnoreDeletedText {
			get { return ignoreDeletedText; }
			set {
				if (value == ignoreDeletedText)
					return;
				ignoreDeletedText = value;
				OnChanged("IgnoreDeletedText", !value, value);
			}
		}
		#endregion
		#region IgnoreInsertedText
		[
		DefaultValue(true), NotifyParentProperty(true)]
		public bool IgnoreInsertedText {
			get { return ignoreInsertedText; }
			set {
				if (value == ignoreInsertedText)
					return;
				ignoreInsertedText = value;
				OnChanged("IgnoreInsertedText", !value, value);
			}
		}
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			IgnoreDeletedText = true;
			IgnoreInsertedText = true;
		}
		public override void CopyFrom(IImporterOptions value) {
			base.CopyFrom(value);
			OpenXmlDocumentImporterOptions options = value as OpenXmlDocumentImporterOptions;
			if (options != null) {
				this.IgnoreDeletedText = options.IgnoreDeletedText;
				this.IgnoreInsertedText = options.ignoreInsertedText;
			}
		}
	}
	#endregion
	#region WordMLDocumentImporterOptions
	[ComVisible(true)]
	public class WordMLDocumentImporterOptions : XmlBasedDocumentImporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.WordML; } }
		protected internal override void ResetCore() {
			base.ResetCore();
			ActualEncoding = Encoding.UTF8;
		}
	}
	#endregion
	#region OpenDocumentImporterOptions
	[ComVisible(true)]
	public class OpenDocumentImporterOptions : DocumentImporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.OpenDocument; } }
	}
	#endregion
	#region XamlDocumentImporterOptions
	public class XamlDocumentImporterOptions : XmlBasedDocumentImporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.Xaml; } }
		#region UpdateField
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new UpdateFieldOptions UpdateField { get { return base.UpdateField; } }
		#endregion
	}
	#endregion
	#region DocDocumentImporterOptions
	[ComVisible(true)]
	public class DocDocumentImporterOptions : DocumentImporterOptions {
		bool ignoreDeletedText = true;
		protected internal override DocumentFormat Format { get { return DocumentFormat.Doc; } }
		#region IgnoreDeletedText
		[
		DefaultValue(true), NotifyParentProperty(true)]
		public bool IgnoreDeletedText {
			get { return ignoreDeletedText; }
			set {
				if (value == ignoreDeletedText)
					return;
				ignoreDeletedText = value;
				OnChanged("IgnoreDeletedText", !value, value);
			}
		}
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			IgnoreDeletedText = true;
		}
		public override void CopyFrom(IImporterOptions value) {
			base.CopyFrom(value);
			DocDocumentImporterOptions options = value as DocDocumentImporterOptions;
			if (options != null) {
				this.IgnoreDeletedText = options.IgnoreDeletedText;
			}
		}
	}
	#endregion
}
