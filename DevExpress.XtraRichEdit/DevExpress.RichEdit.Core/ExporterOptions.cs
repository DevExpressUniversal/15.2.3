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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Export;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit.Export {
	[Flags]
	[ComVisible(true)]
	public enum RtfRunBackColorExportMode {
		Chcbpat = 0,
		Highlight,
		Both,
	}
	#region RtfDocumentExporterOptions
	[ComVisible(true)]
	public class RtfDocumentExporterOptions : DocumentExporterOptions {
		#region Fields
		bool wrapContentInGroup;
		readonly RtfDocumentExporterCompatibilityOptions compatibility;
		ExportFinalParagraphMark exportFinalParagraphMark;
		RtfNumberingListExportFormat listExportFormat;
		#endregion
		public RtfDocumentExporterOptions() {
			this.compatibility = CreateCompatibilityOptions();
		}
		#region Properties
		#region WrapContentInGroup
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RtfDocumentExporterOptionsWrapContentInGroup")]
#endif
		public bool WrapContentInGroup {
			get { return wrapContentInGroup; }
			set {
				if (wrapContentInGroup == value)
					return;
				wrapContentInGroup = value;
				OnChanged("WrapContentInGroup", !value, value);
			}
		}
		protected internal virtual bool ShouldSerializeWrapContentInGroup() {
			return WrapContentInGroup != false;
		}
		protected internal virtual void ResetWrapContentInGroup() {
			WrapContentInGroup = false;
		}
		#endregion
		#region ListExportFormat
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RtfDocumentExporterOptionsListExportFormat")]
#endif
		public RtfNumberingListExportFormat ListExportFormat {
			get { return listExportFormat; }
			set {
				if (listExportFormat == value)
					return;
				RtfNumberingListExportFormat oldValue = listExportFormat;
				listExportFormat = value;
				OnChanged("ListExportFormat", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeListExportFormat() {
			return ListExportFormat != RtfNumberingListExportFormat.RtfFormat;
		}
		protected internal virtual void ResetListExportFormat() {
			ListExportFormat = RtfNumberingListExportFormat.RtfFormat;
		}
		#endregion
		#region ExportFinalParagraphMark
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RtfDocumentExporterOptionsExportFinalParagraphMark")]
#endif
		public ExportFinalParagraphMark ExportFinalParagraphMark {
			get { return exportFinalParagraphMark; }
			set {
				if (exportFinalParagraphMark == value)
					return;
				ExportFinalParagraphMark oldValue = exportFinalParagraphMark;
				exportFinalParagraphMark = value;
				OnChanged("ExportFinalParagraphMark", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeExportFinalParagraphMark() {
			return ExportFinalParagraphMark != ExportFinalParagraphMark.Always;
		}
		protected internal virtual void ResetExportFinalParagraphMark() {
			ExportFinalParagraphMark = ExportFinalParagraphMark.Always;
		}
		#endregion
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RtfDocumentExporterOptionsCompatibility"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RtfDocumentExporterCompatibilityOptions Compatibility { get { return compatibility; } }
		protected internal override Encoding ActualEncoding {
			get { return base.ActualEncoding; }
			set {
			}
		}
		protected internal override DocumentFormat Format { get { return DocumentFormat.Rtf; } }
		#endregion
		protected override Encoding GetDefaultEncoding() {
			return DevExpress.XtraRichEdit.Import.Rtf.RtfImporter.DefaultEncoding;
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			this.ListExportFormat = RtfNumberingListExportFormat.RtfFormat;
			this.WrapContentInGroup = false;
			this.ExportFinalParagraphMark = ExportFinalParagraphMark.Always;
			if (compatibility != null)
				compatibility.Reset();
		}
		public override void CopyFrom(IExporterOptions value) {
			base.CopyFrom(value);
			RtfDocumentExporterOptions options = value as RtfDocumentExporterOptions;
			if (options != null) {
				this.WrapContentInGroup = options.WrapContentInGroup;
				this.ListExportFormat = options.ListExportFormat;
				this.Compatibility.CopyFrom(options.Compatibility);
				this.ExportFinalParagraphMark = options.ExportFinalParagraphMark;
			}
		}
		protected internal virtual RtfDocumentExporterCompatibilityOptions CreateCompatibilityOptions() {
			return new RtfDocumentExporterCompatibilityOptions();
		}
	}
	#endregion
	#region RtfDocumentExporterCompatibilityOptions
	[ComVisible(true)]
	public class RtfDocumentExporterCompatibilityOptions : RichEditNotificationOptions, ISupportsCopyFrom<RtfDocumentExporterCompatibilityOptions> {
		bool duplicateObjectAsMetafile;
		RtfRunBackColorExportMode backColorExportMode;
		public RtfDocumentExporterCompatibilityOptions() {
		}
		#region Properties
		#region DuplicateObjectAsMetafile
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RtfDocumentExporterCompatibilityOptionsDuplicateObjectAsMetafile")]
#endif
		public bool DuplicateObjectAsMetafile {
			get { return duplicateObjectAsMetafile; }
			set {
				if (duplicateObjectAsMetafile == value)
					return;
				duplicateObjectAsMetafile = value;
				OnChanged("DuplicateObjectAsMetafile", !value, value);
			}
		}
		protected internal virtual bool ShouldSerializeDuplicateObjectAsMetafile() {
			return DuplicateObjectAsMetafile != false;
		}
		protected internal virtual void ResetDuplicateObjectAsMetafile() {
			DuplicateObjectAsMetafile = false;
		}
		#endregion
		#region BackColorExportMode
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RtfDocumentExporterCompatibilityOptionsBackColorExportMode")]
#endif
		public RtfRunBackColorExportMode BackColorExportMode {
			get { return backColorExportMode; }
			set {
				if (backColorExportMode == value)
					return;
				RtfRunBackColorExportMode oldValue = backColorExportMode;
				backColorExportMode = value;
				OnChanged("BackColorExportMode", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeBackColorExportMode() {
			return BackColorExportMode != RtfRunBackColorExportMode.Chcbpat;
		}
		protected internal virtual void ResetBackColorExportMode() {
			BackColorExportMode = RtfRunBackColorExportMode.Chcbpat;
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			ResetDuplicateObjectAsMetafile();
			ResetBackColorExportMode();
		}
		public virtual void CopyFrom(RtfDocumentExporterCompatibilityOptions value) {
			this.DuplicateObjectAsMetafile = value.DuplicateObjectAsMetafile;
			this.BackColorExportMode = value.BackColorExportMode;
		}
	}
	#endregion
	#region PlainTextDocumentExporterOptions
	[ComVisible(true)]
	public class PlainTextDocumentExporterOptions : DocumentExporterOptions {
		#region Fields
		static readonly string defaultFootNoteSeparator = new String('-', 60);
		static readonly string defaultEndNoteSeparator = defaultFootNoteSeparator;
		bool exportHiddenText;
		bool exportBulletsAndNumbering = true;
		string fieldCodeStartMarker = String.Empty;
		string fieldCodeEndMarker = String.Empty;
		string fieldResultEndMarker = String.Empty;
		string footNoteNumberStringFormat = String.Empty;
		string endNoteNumberStringFormat = String.Empty;
		string footNoteSeparator = String.Empty;
		string endNoteSeparator = String.Empty;
		DevExpress.XtraRichEdit.Export.PlainText.ExportFinalParagraphMark exportFinalParagraphMark = DevExpress.XtraRichEdit.Export.PlainText.ExportFinalParagraphMark.Never;
		#endregion
		#region Properties
		protected internal override DocumentFormat Format { get { return DocumentFormat.PlainText; } }
		#region Encoding
#if !SL && !DXPORTABLE
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsEncoding"),
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
		#region ExportHiddenText
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsExportHiddenText"),
#endif
 DefaultValue(false)]
		public virtual bool ExportHiddenText {
			get { return exportHiddenText; }
			set {
				if (exportHiddenText == value)
					return;
				exportHiddenText = value;
				OnChanged("ExportHiddenText", !value, value);
			}
		}
		#endregion
		#region ExportBulletsAndNumbering
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsExportBulletsAndNumbering"),
#endif
 DefaultValue(true)]
		public bool ExportBulletsAndNumbering {
			get { return exportBulletsAndNumbering; }
			set {
				if (exportBulletsAndNumbering == value)
					return;
				exportBulletsAndNumbering = value;
				OnChanged("ExportBulletsAndNumbering", !value, value);
			}
		}
		#endregion
		#region FieldCodeStartMarker
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsFieldCodeStartMarker"),
#endif
 DefaultValue("")]
		public string FieldCodeStartMarker {
			get { return fieldCodeStartMarker; }
			set {
				if (fieldCodeStartMarker == value)
					return;
				string oldValue = fieldCodeStartMarker;
				fieldCodeStartMarker = value;
				OnChanged("FieldCodeStartMarker", oldValue, value);
			}
		}
		#endregion
		#region FieldCodeEndMarker
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsFieldCodeEndMarker"),
#endif
 DefaultValue("")]
		public string FieldCodeEndMarker {
			get { return fieldCodeEndMarker; }
			set {
				if (fieldCodeEndMarker == value)
					return;
				string oldValue = fieldCodeEndMarker;
				fieldCodeEndMarker = value;
				OnChanged("FieldCodeEndMarker", oldValue, value);
			}
		}
		#endregion
		#region FieldResultEndMarker
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsFieldResultEndMarker"),
#endif
 DefaultValue("")]
		public string FieldResultEndMarker {
			get { return fieldResultEndMarker; }
			set {
				if (fieldResultEndMarker == value)
					return;
				string oldValue = fieldResultEndMarker;
				fieldResultEndMarker = value;
				OnChanged("FieldResultEndMarker", oldValue, value);
			}
		}
		#endregion
		#region FootNoteNumberStringFormat
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsFootNoteNumberStringFormat"),
#endif
 DefaultValue("")]
		public string FootNoteNumberStringFormat {
			get { return footNoteNumberStringFormat; }
			set {
				if (footNoteNumberStringFormat == value)
					return;
				string oldValue = footNoteNumberStringFormat;
				footNoteNumberStringFormat = value;
				OnChanged("FootNoteNumberStringFormat", oldValue, value);
			}
		}
		protected internal string ActualFootNoteNumberStringFormat { get { return String.IsNullOrEmpty(FootNoteNumberStringFormat) ? "[{0}]" : FootNoteNumberStringFormat; } }
		#endregion
		#region EndNoteNumberStringFormat
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsEndNoteNumberStringFormat"),
#endif
 DefaultValue("")]
		public string EndNoteNumberStringFormat {
			get { return endNoteNumberStringFormat; }
			set {
				if (endNoteNumberStringFormat == value)
					return;
				string oldValue = endNoteNumberStringFormat;
				endNoteNumberStringFormat = value;
				OnChanged("EndNoteNumberStringFormat", oldValue, value);
			}
		}
		protected internal string ActualEndNoteNumberStringFormat { get { return String.IsNullOrEmpty(EndNoteNumberStringFormat) ? "[{0}]" : EndNoteNumberStringFormat; } }
		#endregion
		#region FootNoteSeparator
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsFootNoteSeparator"),
#endif
 DefaultValue("")]
		public string FootNoteSeparator {
			get { return footNoteSeparator; }
			set {
				if (footNoteSeparator == value)
					return;
				string oldValue = footNoteSeparator;
				footNoteSeparator = value;
				OnChanged("FootNoteSeparator", oldValue, value);
			}
		}
		protected internal string ActualFootNoteSeparator { get { return String.IsNullOrEmpty(FootNoteSeparator) ? defaultFootNoteSeparator : FootNoteSeparator; } }
		#endregion
		#region EndNoteSeparator
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsEndNoteSeparator"),
#endif
 DefaultValue("")]
		public string EndNoteSeparator {
			get { return endNoteSeparator; }
			set {
				if (endNoteSeparator == value)
					return;
				string oldValue = endNoteSeparator;
				endNoteSeparator = value;
				OnChanged("EndNoteSeparator", oldValue, value);
			}
		}
		protected internal string ActualEndNoteSeparator { get { return String.IsNullOrEmpty(EndNoteSeparator) ? defaultEndNoteSeparator : EndNoteSeparator; } }
		#endregion
		#region ExportFinalParagraphMark
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PlainTextDocumentExporterOptionsExportFinalParagraphMark"),
#endif
 DefaultValue("")]
		public DevExpress.XtraRichEdit.Export.PlainText.ExportFinalParagraphMark ExportFinalParagraphMark {
			get { return exportFinalParagraphMark; }
			set {
				if (exportFinalParagraphMark == value)
					return;
				DevExpress.XtraRichEdit.Export.PlainText.ExportFinalParagraphMark oldValue = exportFinalParagraphMark;
				exportFinalParagraphMark = value;
				OnChanged("ExportFinalParagraphMark", oldValue, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			this.ExportHiddenText = false;
			this.ExportBulletsAndNumbering = true;
			this.FieldCodeStartMarker = String.Empty;
			this.FieldCodeEndMarker = String.Empty;
			this.FieldResultEndMarker = String.Empty;
			this.FootNoteNumberStringFormat = String.Empty;
			this.EndNoteNumberStringFormat = String.Empty;
			this.FootNoteSeparator = String.Empty;
			this.EndNoteSeparator = String.Empty;
			this.Encoding = DXEncoding.Default;
		}
		public override void CopyFrom(IExporterOptions value) {
			base.CopyFrom(value);
			PlainTextDocumentExporterOptions options = value as PlainTextDocumentExporterOptions;
			if (options != null) {
				ExportHiddenText = options.ExportHiddenText;
				ExportBulletsAndNumbering = options.ExportBulletsAndNumbering;
				FieldCodeStartMarker = options.FieldCodeStartMarker;
				FieldCodeEndMarker = options.FieldCodeEndMarker;
				FieldResultEndMarker = options.FieldResultEndMarker;
				FootNoteNumberStringFormat = options.FootNoteNumberStringFormat;
				EndNoteNumberStringFormat = options.EndNoteNumberStringFormat;
				FootNoteSeparator = options.FootNoteSeparator;
				EndNoteSeparator = options.EndNoteSeparator;
			}
		}
	}
	#endregion
	#region MhtDocumentExporterOptions
	[ComVisible(true)]
	public class MhtDocumentExporterOptions : HtmlDocumentExporterOptions {
		[Browsable(false)]
		public override bool EmbedImages { get { return false; } set { } }
		protected internal override DocumentFormat Format { get { return DocumentFormat.Mht; } }
	}
	#endregion
	#region HtmlDocumentExporterOptions
	[ComVisible(true)]
	public class HtmlDocumentExporterOptions : DocumentExporterOptions {
		internal const int DefaultOverrideImageResolution = 96; 
		#region Fields
		HtmlNumberingListExportFormat htmlNumberingListExportFormat;
		CssPropertiesExportType cssPropertiesExportType;
		UriExportType uriExportType;
		ExportRootTag exportRootTag;
		ExportImageSize exportImageSize;
		bool keepExternalImageSize;
		bool embedImages;
		bool underlineTocHyperlinks;
		string tabMarker = "\t";
		HtmlFontUnit fontUnit;
		int overrideImageResolution = DefaultOverrideImageResolution;
		string footNoteNumberStringFormat = String.Empty;
		string endNoteNumberStringFormat = String.Empty;
		string footNoteNamePrefix = String.Empty;
		string endNoteNamePrefix = String.Empty;
		bool disposeConvertedImagesImmediately;
		bool defaultCharacterPropertiesExportToCss;
		bool useHtml5;
		bool ignoreParagraphOutlineLevel;
		#endregion
		public HtmlDocumentExporterOptions() {
			defaultCharacterPropertiesExportToCss = true;
		}
		#region Properties
		#region HtmlNumberingListExportFormat
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsHtmlNumberingListExportFormat")]
#endif
		public HtmlNumberingListExportFormat HtmlNumberingListExportFormat {
			get { return htmlNumberingListExportFormat; }
			set {
				if (htmlNumberingListExportFormat == value)
					return;
				HtmlNumberingListExportFormat oldFormat = htmlNumberingListExportFormat;
				htmlNumberingListExportFormat = value;
				OnChanged("HtmlNumberingListExportFormat", oldFormat, value);
			}
		}
		protected internal virtual bool ShouldSerializeHtmlNumberingListExportFormat() {
			return HtmlNumberingListExportFormat != HtmlNumberingListExportFormat.HtmlFormat;
		}
		protected internal virtual void ResetHtmlNumberingListExportFormat() {
			HtmlNumberingListExportFormat = HtmlNumberingListExportFormat.HtmlFormat;
		}
		#endregion
		#region CssPropertiesExportType
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsCssPropertiesExportType")]
#endif
		public CssPropertiesExportType CssPropertiesExportType {
			get { return cssPropertiesExportType; }
			set {
				if (cssPropertiesExportType == value)
					return;
				CssPropertiesExportType oldValue = cssPropertiesExportType;
				cssPropertiesExportType = value;
				OnChanged("CssPropertiesExportType", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeCssPropertiesExportType() {
			return CssPropertiesExportType != CssPropertiesExportType.Style;
		}
		protected internal virtual void ResetCssPropertiesExportType() {
			CssPropertiesExportType = CssPropertiesExportType.Style;
		}
		#endregion
		#region UriExportType
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsUriExportType")]
#endif
		public UriExportType UriExportType {
			get { return uriExportType; }
			set {
				if (uriExportType == value)
					return;
				UriExportType oldValue = uriExportType;
				uriExportType = value;
				OnChanged("UriExportType", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeUriExportType() {
			return UriExportType != UriExportType.Relative;
		}
		protected internal virtual void ResetUriExportType() {
			UriExportType = UriExportType.Relative;
		}
		#endregion
		#region ExportRootTag
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsExportRootTag")]
#endif
		public ExportRootTag ExportRootTag {
			get { return exportRootTag; }
			set {
				if (exportRootTag == value)
					return;
				ExportRootTag oldValue = exportRootTag;
				exportRootTag = value;
				OnChanged("ExportRootTag", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeExportRootTag() {
			return ExportRootTag != ExportRootTag.Html;
		}
		protected internal virtual void ResetExportRootTag() {
			ExportRootTag = ExportRootTag.Html;
		}
		#endregion
		#region ExportImageSize
		public ExportImageSize ExportImageSize {
			get { return exportImageSize; }
			set {
				if (exportImageSize == value)
					return;
				ExportImageSize oldValue = exportImageSize;
				exportImageSize = value;
				OnChanged("ExportImageSize", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeExportImageSize() {
			return ExportImageSize != ExportImageSize.Auto;
		}
		protected internal virtual void ResetExportImageTag() {
			ExportImageSize = ExportImageSize.Auto;
		}
		#endregion
		#region KeepExternalImageSize
		[DefaultValue(false)]
		public bool KeepExternalImageSize {
			get { return keepExternalImageSize; }
			set {
				if (KeepExternalImageSize == value)
					return;
				keepExternalImageSize = value;
				OnChanged("KeepExternalImageSize", !KeepExternalImageSize, KeepExternalImageSize);
			}
		}
		#endregion
		#region Encoding
#if !SL && !DXPORTABLE
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsEncoding"),
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
		#region EmbedImages
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsEmbedImages"),
#endif
DefaultValue(false)]
		public virtual bool EmbedImages {
			get { return embedImages; }
			set {
				if (embedImages == value)
					return;
				embedImages = value;
				OnChanged("EmbedImages", !value, value);
			}
		}
		#endregion
		#region EmbedImages
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsDefaultCharacterPropertiesExportToCss"),
#endif
		DefaultValue(true)]
		public virtual bool DefaultCharacterPropertiesExportToCss {
			get { return defaultCharacterPropertiesExportToCss; }
			set {
				if (defaultCharacterPropertiesExportToCss == value)
					return;
				defaultCharacterPropertiesExportToCss = value;
				OnChanged("DefaultCharacterPropertiesExportToCss", !value, value);
			}
		}
		#endregion
		#region TabMarker
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsTabMarker"),
#endif
 DefaultValue("\t")]
		public string TabMarker {
			get { return tabMarker; }
			set {
				if (tabMarker == value)
					return;
				string oldValue = tabMarker;
				tabMarker = value;
				OnChanged("TabMarker", oldValue, value);
			}
		}
		#endregion
		#region FontUnit
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsFontUnit"),
#endif
 DefaultValue(HtmlFontUnit.Point)]
		public HtmlFontUnit FontUnit {
			get { return fontUnit; }
			set {
				if (fontUnit == value)
					return;
				HtmlFontUnit oldValue = fontUnit;
				fontUnit = value;
				OnChanged("FontUnit", oldValue, value);
			}
		}
		#endregion
		#region UnderlineTocHyperlinks
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsUnderlineTocHyperlinks"),
#endif
 DefaultValue(true)]
		public virtual bool UnderlineTocHyperlinks {
			get { return underlineTocHyperlinks; }
			set {
				if (underlineTocHyperlinks == value)
					return;
				underlineTocHyperlinks = value;
				OnChanged("UnderlineTocHyperlinks", !value, value);
			}
		}
		#endregion
		#region OverrideImageResolution
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsOverrideImageResolution"),
#endif
 NotifyParentProperty(true)]
		public int OverrideImageResolution {
			get { return overrideImageResolution; }
			set {
				if (overrideImageResolution == value)
					return;
				int oldValue = OverrideImageResolution;
				overrideImageResolution = value;
				OnChanged("OverrideImageResolution", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeOverrideImageResolution() {
			return OverrideImageResolution != DefaultOverrideImageResolution;
		}
		protected internal virtual void ResetOverrideImageResolution() {
			OverrideImageResolution = DefaultOverrideImageResolution;
		}
		#endregion
		#region FootNoteNumberStringFormat
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsFootNoteNumberStringFormat"),
#endif
 DefaultValue("")]
		public string FootNoteNumberStringFormat {
			get { return footNoteNumberStringFormat; }
			set {
				if (footNoteNumberStringFormat == value)
					return;
				string oldValue = footNoteNumberStringFormat;
				footNoteNumberStringFormat = value;
				OnChanged("FootNoteNumberStringFormat", oldValue, value);
			}
		}
		protected internal string ActualFootNoteNumberStringFormat { get { return String.IsNullOrEmpty(FootNoteNumberStringFormat) ? "[{0}]" : FootNoteNumberStringFormat; } }
		#endregion
		#region EndNoteNumberStringFormat
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsEndNoteNumberStringFormat"),
#endif
 DefaultValue("")]
		public string EndNoteNumberStringFormat {
			get { return endNoteNumberStringFormat; }
			set {
				if (endNoteNumberStringFormat == value)
					return;
				string oldValue = endNoteNumberStringFormat;
				endNoteNumberStringFormat = value;
				OnChanged("EndNoteNumberStringFormat", oldValue, value);
			}
		}
		protected internal string ActualEndNoteNumberStringFormat { get { return String.IsNullOrEmpty(EndNoteNumberStringFormat) ? "[{0}]" : EndNoteNumberStringFormat; } }
		#endregion
		#region FootNoteNamePrefix
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsFootNoteNamePrefix"),
#endif
 DefaultValue("")]
		public string FootNoteNamePrefix {
			get { return footNoteNamePrefix; }
			set {
				if (footNoteNamePrefix == value)
					return;
				string oldValue = footNoteNamePrefix;
				footNoteNamePrefix = value;
				OnChanged("FootNoteNamePrefix", oldValue, value);
			}
		}
		protected internal string ActualFootNoteNamePrefix { get { return String.IsNullOrEmpty(FootNoteNamePrefix) ? "_ftn" : FootNoteNamePrefix; } }
		#endregion
		#region UseHtml5
		[DefaultValue(false)]
		public bool UseHtml5 {
			get { return useHtml5; }
			set {
				if (useHtml5 == value)
					return;
				useHtml5 = value;
				OnChanged("UseHtml5", !useHtml5, useHtml5);
			}
		}
		#endregion
		#region IgnoreParagraphOutlineLevel
		[DefaultValue(false)]
		public bool IgnoreParagraphOutlineLevel {
			get { return ignoreParagraphOutlineLevel; }
			set {
				if (ignoreParagraphOutlineLevel == value)
					return;
				ignoreParagraphOutlineLevel = value;
				OnChanged("IgnoreParagraphOutlineLevel", !ignoreParagraphOutlineLevel, ignoreParagraphOutlineLevel);
			}
		}
		#endregion
		#region EndNoteNamePrefix
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HtmlDocumentExporterOptionsEndNoteNamePrefix"),
#endif
 DefaultValue("")]
		public string EndNoteNamePrefix {
			get { return endNoteNamePrefix; }
			set {
				if (endNoteNamePrefix == value)
					return;
				string oldValue = endNoteNamePrefix;
				endNoteNamePrefix = value;
				OnChanged("EndNoteNamePrefix", oldValue, value);
			}
		}
		protected internal string ActualEndNoteNamePrefix { get { return String.IsNullOrEmpty(EndNoteNamePrefix) ? "_endn" : EndNoteNamePrefix; } }
		#endregion
		#region DisposeConvertedImagesImmediately
		protected internal bool DisposeConvertedImagesImmediately {
			get { return disposeConvertedImagesImmediately; }
			set {
				if (disposeConvertedImagesImmediately == value)
					return;
				disposeConvertedImagesImmediately = value;
				OnChanged("FontUnit", !value, value);
			}
		}
		#endregion
		protected internal override DocumentFormat Format { get { return DocumentFormat.Html; } }
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			this.HtmlNumberingListExportFormat = HtmlNumberingListExportFormat.HtmlFormat;
			this.CssPropertiesExportType = CssPropertiesExportType.Style;
			this.ExportRootTag = ExportRootTag.Html;
			this.Encoding = Encoding.UTF8;
			this.UriExportType = UriExportType.Relative;
			this.EmbedImages = false;
			this.TabMarker = "\t";
			this.FontUnit = HtmlFontUnit.Point;
			this.UnderlineTocHyperlinks = true;
			this.OverrideImageResolution = DefaultOverrideImageResolution;
			this.FootNoteNumberStringFormat = String.Empty;
			this.EndNoteNumberStringFormat = String.Empty;
			this.DisposeConvertedImagesImmediately = true;
			this.DefaultCharacterPropertiesExportToCss = true;
			this.ExportImageSize = Html.ExportImageSize.Auto;
			this.KeepExternalImageSize = false;
			this.UseHtml5 = false;
			this.IgnoreParagraphOutlineLevel = false;
		}
		public override void CopyFrom(IExporterOptions value) {
			base.CopyFrom(value);
			HtmlDocumentExporterOptions options = value as HtmlDocumentExporterOptions;
			if (options != null) {
				this.HtmlNumberingListExportFormat = options.HtmlNumberingListExportFormat;
				this.CssPropertiesExportType = options.CssPropertiesExportType;
				this.ExportRootTag = options.ExportRootTag;
				this.UriExportType = options.UriExportType;
				this.EmbedImages = options.EmbedImages;
				this.TabMarker = options.TabMarker;
				this.FontUnit = options.FontUnit;
				this.UnderlineTocHyperlinks = options.UnderlineTocHyperlinks;
				this.OverrideImageResolution = options.OverrideImageResolution;
				this.FootNoteNumberStringFormat = options.FootNoteNumberStringFormat;
				this.EndNoteNumberStringFormat = options.EndNoteNumberStringFormat;
				this.DisposeConvertedImagesImmediately = options.DisposeConvertedImagesImmediately;
				this.DefaultCharacterPropertiesExportToCss = options.DefaultCharacterPropertiesExportToCss;
				this.ExportImageSize = options.ExportImageSize;
				this.KeepExternalImageSize = options.KeepExternalImageSize;
				this.UseHtml5 = options.UseHtml5;
				this.IgnoreParagraphOutlineLevel = options.IgnoreParagraphOutlineLevel;
			}
		}
	}
	#endregion
	#region OpenXmlDocumentExporterOptions
	[ComVisible(true)]
	public class OpenXmlDocumentExporterOptions : DocumentExporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.OpenXml; } }
		bool alternateImageFolder = false;
		bool limitBookmarkNameTo40Chars = true;
		bool limitStyleNameTo253Chars = true;
		bool limitFontNameTo31Chars = true;
		bool allowAlternateStyleNames = true;
		#region AlternateImageFolder
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("OpenXmlDocumentExporterOptionsAlternateImageFolder"),
#endif
 DefaultValue(false)]
		public bool AlternateImageFolder {
			get { return alternateImageFolder; }
			set {
				if (alternateImageFolder == value) return;
				alternateImageFolder = value;
				OnChanged("AlternateImageFolder", !value, value);
			}
		}
		#endregion
		#region LimitBookmarkNameTo40Chars
		[DefaultValue(true)]
		public bool LimitBookmarkNameTo40Chars {
			get { return limitBookmarkNameTo40Chars; }
			set {
				if (limitBookmarkNameTo40Chars == value)
					return;
				limitBookmarkNameTo40Chars = value;
				OnChanged("LimitBookmarkNameTo40Chars", !value, value);
			}
		}
		#endregion
		#region LimitStyleNameTo253Chars
		[ DefaultValue(true)]
		public bool LimitStyleNameTo253Chars {
			get { return limitStyleNameTo253Chars; }
			set {
				if (limitStyleNameTo253Chars == value)
					return;
				limitStyleNameTo253Chars = value;
				OnChanged("LimitStyleNameTo253Chars", !value, value);
			}
		}
		#endregion
		#region LimitStyleNameTo253Chars
		[ DefaultValue(true)]
		public bool LimitFontNameTo31Chars {
			get { return limitFontNameTo31Chars; }
			set {
				if (limitFontNameTo31Chars == value)
					return;
				limitFontNameTo31Chars = value;
				OnChanged("LimitFontNameTo31Chars", !value, value);
			}
		}
		#endregion
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("OpenXmlDocumentExporterOptionsAllowAlternateStyleNames"),
#endif
 DefaultValue(true)]
		public bool AllowAlternateStyleNames {
			get { return allowAlternateStyleNames; }
			set {
				if (allowAlternateStyleNames == value)
					return;
				allowAlternateStyleNames = value;
				OnChanged("AllowAlternateStyleNames", !value, value);
			}
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			this.AlternateImageFolder = false;
			this.LimitBookmarkNameTo40Chars = true;
			this.LimitStyleNameTo253Chars = true;
			this.LimitFontNameTo31Chars = true;
			this.AllowAlternateStyleNames = true;
		}
		public override void CopyFrom(IExporterOptions value) {
			base.CopyFrom(value);
			OpenXmlDocumentExporterOptions options = value as OpenXmlDocumentExporterOptions;
			if (options == null)
				return;
			this.AlternateImageFolder = options.AlternateImageFolder;
			this.LimitBookmarkNameTo40Chars = options.LimitBookmarkNameTo40Chars;
			this.LimitStyleNameTo253Chars = options.LimitStyleNameTo253Chars;
			this.LimitFontNameTo31Chars = options.LimitFontNameTo31Chars;
			this.AllowAlternateStyleNames = options.AllowAlternateStyleNames;
		}
	}
	#endregion
	#region OpenDocumentExporterOptions
	[ComVisible(true)]
	public class OpenDocumentExporterOptions : DocumentExporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.OpenDocument; } }
	}
	#endregion
	#region WordMLDocumentExporterOptions
	[ComVisible(true)]
	public class WordMLDocumentExporterOptions : DocumentExporterOptions {
		public WordMLDocumentExporterOptions() {
			this.ActualEncoding = Encoding.UTF8;
		}
		protected internal override DocumentFormat Format { get { return DocumentFormat.WordML; } }
	}
	#endregion
	#region DocDocumentExporterOptions
	[ComVisible(true)]
	public class DocDocumentExporterOptions : DocumentExporterOptions {
		readonly DocDocumentExporterCompatibilityOptions compatibility;
		public DocDocumentExporterOptions() {
			this.compatibility = CreateCompatibilityOptions();
		}
		protected internal override DocumentFormat Format { get { return DocumentFormat.Doc; } }
		[
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DocDocumentExporterCompatibilityOptions Compatibility { get { return compatibility; } }
		protected internal virtual DocDocumentExporterCompatibilityOptions CreateCompatibilityOptions() {
			return new DocDocumentExporterCompatibilityOptions();
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			if (compatibility != null)
				compatibility.Reset();
		}
		public override void CopyFrom(IExporterOptions value) {
			base.CopyFrom(value);
			DocDocumentExporterOptions options = value as DocDocumentExporterOptions;
			if (options != null)
				this.Compatibility.CopyFrom(options.Compatibility);
		}
	}
	#endregion
	#region DocDocumentExporterCompatibilityOptions
	[ComVisible(true)]
	public class DocDocumentExporterCompatibilityOptions : RichEditNotificationOptions, ISupportsCopyFrom<DocDocumentExporterCompatibilityOptions> {
		#region AllowNonLinkedListDefinitions
		bool allowNonLinkedListDefinitions;
		public bool AllowNonLinkedListDefinitions {
			get { return allowNonLinkedListDefinitions; }
			set {
				if (allowNonLinkedListDefinitions == value)
					return;
				allowNonLinkedListDefinitions = value;
				OnChanged("AllowNonLinkedListDefinitions", !value, value);
			}
		}
		protected internal virtual bool ShouldSerializeAllowNonLinkedListDefinitions() {
			return AllowNonLinkedListDefinitions != false;
		}
		protected internal virtual void ResetAllowNonLinkedListDefinitions() {
			AllowNonLinkedListDefinitions = false;
		}
		#endregion
		protected internal override void ResetCore() {
			ResetAllowNonLinkedListDefinitions();
		}
		public virtual void CopyFrom(DocDocumentExporterCompatibilityOptions value) {
			this.AllowNonLinkedListDefinitions = value.AllowNonLinkedListDefinitions;
		}
	}
	#endregion
	#region XamlDocumentExporterOptions
	public class XamlDocumentExporterOptions : DocumentExporterOptions {
		bool exportDefaultStyle;
		bool silverlightCompatible;
		public XamlDocumentExporterOptions() {
			this.ActualEncoding = Encoding.UTF8;
		}
		#region Properties
		protected internal override DocumentFormat Format { get { return DocumentFormat.Xaml; } }
		#region ExportDefaultStyle
		public bool ExportDefaultStyle {
			get { return exportDefaultStyle; }
			set {
				if (exportDefaultStyle == value)
					return;
				exportDefaultStyle = value;
				OnChanged("ExportDefaultStyle", !value, value);
			}
		}
		protected internal virtual bool ShouldSerializeExportDefaultStyle() {
			return !ExportDefaultStyle;
		}
		protected internal virtual void ResetExportRootTag() {
			ExportDefaultStyle = true;
		}
		#endregion
		internal bool SilverlightCompatible { get { return silverlightCompatible; } set { silverlightCompatible = value; } }
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			this.ExportDefaultStyle = true;
		}
		public override void CopyFrom(IExporterOptions value) {
			base.CopyFrom(value);
			XamlDocumentExporterOptions options = value as XamlDocumentExporterOptions;
			if (options != null) {
				this.ExportDefaultStyle = options.ExportDefaultStyle;
			}
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Export.PlainText {
	#region ExportFinalParagraphMark
	[ComVisible(true)]
	public enum ExportFinalParagraphMark {
		Always,
		Never
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region ExportFinalParagraphMark
	[ComVisible(true)]
	public enum ExportFinalParagraphMark {
		Always,
		Never,
		SelectedOnly
	}
	#endregion
	#region RtfNumberingListExportFormat
	[ComVisible(true)]
	public enum RtfNumberingListExportFormat {
		PlainTextFormat,
		RtfFormat
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Export.Html {
	#region HtmlNumberingListExportFormat
	[ComVisible(true)]
	public enum HtmlNumberingListExportFormat {
		PlainTextFormat,
		HtmlFormat
	}
	#endregion
	#region CssPropertiesExportType
	[ComVisible(true)]
	public enum CssPropertiesExportType {
		[Obsolete("Use the Style value instead", true), EditorBrowsable(EditorBrowsableState.Never)]
		ExportToCurrentFile = 0,
		[Obsolete("Use the Link value instead", true), EditorBrowsable(EditorBrowsableState.Never)]
		ExportToSeparateFile = 1,
		Style = 0,
		Link = 1,
		Inline = 2
	}
	#endregion
	#region HtmlFontUnit
	[ComVisible(true)]
	public enum HtmlFontUnit {
		Point,
		Pixel
	}
	#endregion
	#region ExportRootTag
	[ComVisible(true)]
	public enum ExportRootTag {
		Html = 0,
		Body = 1
	}
	#endregion
	#region ExportImageSize
	[ComVisible(true)]
	public enum ExportImageSize {
		Auto = 0,
		Always = 1
	}
	#endregion
	#region UriExportType
	[ComVisible(true)]
	public enum UriExportType {
		Relative,
		Absolute
	}
	#endregion
}
