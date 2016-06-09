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
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export.Html;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Export {
	#region HtmlDocumentExporter
	public class HtmlDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_HtmlFiles), new string[] { "htm", "html" });
		#region IDocumentExporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.Html; } }
		public IExporterOptions SetupSaving() {
			return new HtmlDocumentExporterOptions();
		}
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.SaveDocumentHtmlContent(stream, (HtmlDocumentExporterOptions)options);
			return true;
		}
		#endregion
	}
	#endregion
	#region HtmlDocumentExporterOptions
	[ComVisible(true)]
	public class HtmlDocumentExporterOptions : DocumentExporterOptions {
		#region Fields
		CssPropertiesExportType cssPropertiesExportType;
		UriExportType uriExportType;
		ExportRootTag exportRootTag;
		bool exportImages = true;
		bool embedImages;
		bool underlineTocHyperlinks;
		string tabMarker = "\t";
		HtmlFontUnit fontUnit;
		int overrideImageResolution = (int)DocumentModel.Dpi;
		bool disposeConvertedImagesImmediately;
		string range = String.Empty;
		int sheetIndex;
		bool addClipboardHtmlFragmentTags;
		bool useColumnGroupTag = true;
		bool useSpanTagForIndentation = true;
		bool useCssForWidthAndHeight = true;
		#endregion
		public HtmlDocumentExporterOptions() {
		}
		#region Properties
		#region CssPropertiesExportType
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsCssPropertiesExportType")]
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
	[DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsUriExportType")]
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
	[DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsExportRootTag")]
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
		#region Encoding
#if !SL && !DXPORTABLE
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsEncoding"),
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
		#region ExportImages
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsExportImages"),
#endif
DefaultValue(true)]
		public virtual bool ExportImages {
			get { return exportImages; }
			set {
				if (exportImages == value)
					return;
				exportImages = value;
				OnChanged("ExportImages", !value, value);
			}
		}
		#endregion
		#region EmbedImages
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsEmbedImages"),
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
		#region TabMarker
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsTabMarker"),
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
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsFontUnit"),
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
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsUnderlineTocHyperlinks"),
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
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsOverrideImageResolution"),
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
			return OverrideImageResolution != (int)DocumentModel.Dpi;
		}
		protected internal virtual void ResetOverrideImageResolution() {
			OverrideImageResolution = (int)DocumentModel.Dpi;
		}
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
		#region Range
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsRange"),
#endif
		DefaultValue("")]
		public string Range
		{
			get { return this.range; }
			set
			{
				if (range == value)
					return;
				string prev = range;
				this.range = value;
				OnChanged("Range", prev, value);
			}
		}
		#endregion
		#region SheetIndex
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsSheetIndex"),
#endif
		DefaultValue(0)]
		public int SheetIndex
		{
			get { return sheetIndex; }
			set
			{
				if (sheetIndex == value)
					return;
				int prevSheetIndex = sheetIndex;
				sheetIndex = value;
				OnChanged("SheetIndex", prevSheetIndex, value);
			}
		}
		#endregion
		#region UseColumnGroupTag
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsUseColumnGroupTag"),
#endif
 DefaultValue(true)]
		public bool UseColumnGroupTag {
			get { return useColumnGroupTag; }
			set {
				if (useColumnGroupTag == value)
					return;
				useColumnGroupTag = value;
				OnChanged("UseColumnGroupTag", !value, value);
			}
		}
		#endregion
		#region UseSpanTagForIndentation
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsUseSpanTagForIndentation"),
#endif
 DefaultValue(true)]
		public bool UseSpanTagForIndentation {
			get { return useSpanTagForIndentation; }
			set {
				if (useSpanTagForIndentation == value)
					return;
				useSpanTagForIndentation = value;
				OnChanged("UseSpanTagForIndentation", !value, value);
			}
		}
		#endregion
		#region UseCssForWidthAndHeight
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("HtmlDocumentExporterOptionsUseCssForWidthAndHeight"),
#endif
 DefaultValue(true)]
		public bool UseCssForWidthAndHeight {
			get { return useCssForWidthAndHeight; }
			set {
				if (useCssForWidthAndHeight == value)
					return;
				useCssForWidthAndHeight = value;
				OnChanged("UseCssForWidthAndHeight", !value, value);
			}
		}
		#endregion
		internal bool AddClipboardHtmlFragmentTags {
			get { return this.addClipboardHtmlFragmentTags; }
			set {
				if (addClipboardHtmlFragmentTags == value)
					return;
				bool prevAddClipboardHtmlFragmentTags = addClipboardHtmlFragmentTags;
				addClipboardHtmlFragmentTags = value;
				OnChanged("AddClipboardHtmlFragmentTags", prevAddClipboardHtmlFragmentTags, value);
			}
		}
		protected internal override DocumentFormat Format { get { return DocumentFormat.Html; } }
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			this.CssPropertiesExportType = CssPropertiesExportType.Style;
			this.ExportRootTag = ExportRootTag.Html;
			this.Encoding = Encoding.UTF8;
			this.UriExportType = UriExportType.Relative;
			this.ExportImages = true;
			this.EmbedImages = false;
			this.TabMarker = "\t";
			this.FontUnit = HtmlFontUnit.Point;
			this.UnderlineTocHyperlinks = true;
			this.OverrideImageResolution = (int)DocumentModel.Dpi;
			this.DisposeConvertedImagesImmediately = true;
			this.SheetIndex = 0;
			this.addClipboardHtmlFragmentTags = false;
			this.Range = String.Empty;
			this.UseColumnGroupTag = true;
			this.UseSpanTagForIndentation = true;
			this.UseCssForWidthAndHeight = true;
		}
		public override void CopyFrom(IExporterOptions value) {
			base.CopyFrom(value);
			HtmlDocumentExporterOptions options = value as HtmlDocumentExporterOptions;
			if (options != null) {
				this.CssPropertiesExportType = options.CssPropertiesExportType;
				this.ExportRootTag = options.ExportRootTag;
				this.UriExportType = options.UriExportType;
				this.ExportImages = options.ExportImages;
				this.EmbedImages = options.EmbedImages;
				this.TabMarker = options.TabMarker;
				this.FontUnit = options.FontUnit;
				this.UnderlineTocHyperlinks = options.UnderlineTocHyperlinks;
				this.OverrideImageResolution = options.OverrideImageResolution;
				this.DisposeConvertedImagesImmediately = options.DisposeConvertedImagesImmediately;
				this.SheetIndex = options.SheetIndex;
				this.Range = options.Range;
				this.AddClipboardHtmlFragmentTags = options.AddClipboardHtmlFragmentTags;
				this.UseColumnGroupTag = options.UseColumnGroupTag;
				this.UseSpanTagForIndentation = options.UseSpanTagForIndentation;
				this.UseCssForWidthAndHeight = options.UseCssForWidthAndHeight;
			}
		}
	}
	#endregion
}
