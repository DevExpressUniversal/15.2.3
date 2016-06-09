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
using System.IO;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Data;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Mask;
using DevExpress.XtraPrinting.Helpers;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting {
	[
	TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.ExportOptions"),
	SerializationContext(typeof(PrintingSystemSerializationContext))
	]
	public class ExportOptions : IXtraSupportShouldSerialize {
		#region fields
		List<ExportOptionKind> hiddenOptions = new List<ExportOptionKind>();
		protected Dictionary<Type, ExportOptionsBase> options;
		EmailOptions emailOptions = new EmailOptions();
		PrintPreviewOptions printPreviewOptions = new PrintPreviewOptions();
		PrintingSystemXmlSerializer serializer = null;
		#endregion
		public ExportOptions() {
			options = new Dictionary<Type, ExportOptionsBase>();
			options.Add(typeof(PdfExportOptions), new PdfExportOptions());
			options.Add(typeof(XlsExportOptions), new XlsExportOptions());
			options.Add(typeof(TextExportOptions), new TextExportOptions());
			options.Add(typeof(CsvExportOptions), new CsvExportOptions());
			options.Add(typeof(ImageExportOptions), new ImageExportOptions());
			options.Add(typeof(HtmlExportOptions), new HtmlExportOptions());
			options.Add(typeof(MhtExportOptions), new MhtExportOptions());
			options.Add(typeof(RtfExportOptions), new RtfExportOptions());
			options.Add(typeof(NativeFormatOptions), new NativeFormatOptions());
			options.Add(typeof(XlsxExportOptions), new XlsxExportOptions());
			options.Add(typeof(MailMessageExportOptions), new MailMessageExportOptions());
#if SILVERLIGHT
			options.Add(typeof(XpsExportOptions), new XpsExportOptions());
			hiddenOptions.Add(ExportOptionKind.HtmlEmbedImagesInHTML);
#endif
		}
		#region properties
		PrintingSystemXmlSerializer Serializer {
			get {
				if(serializer == null)
					serializer = new PrintingSystemXmlSerializer();
				return serializer;
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsPdf"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PdfExportOptions Pdf { get { return (PdfExportOptions)options[typeof(PdfExportOptions)]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsXls"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),]
		public XlsExportOptions Xls { get { return (XlsExportOptions)options[typeof(XlsExportOptions)]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsXlsx"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public XlsxExportOptions Xlsx { get { return (XlsxExportOptions)options[typeof(XlsxExportOptions)]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsText"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public TextExportOptions Text { get { return (TextExportOptions)options[typeof(TextExportOptions)]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsCsv"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public CsvExportOptions Csv { get { return (CsvExportOptions)options[typeof(CsvExportOptions)]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ImageExportOptions Image { get { return (ImageExportOptions)options[typeof(ImageExportOptions)]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsHtml"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public HtmlExportOptions Html { get { return (HtmlExportOptions)options[typeof(HtmlExportOptions)]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsMht"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public MhtExportOptions Mht { get { return (MhtExportOptions)options[typeof(MhtExportOptions)]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsRtf"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public RtfExportOptions Rtf { get { return (RtfExportOptions)options[typeof(RtfExportOptions)]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsNativeFormat"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public NativeFormatOptions NativeFormat { get { return (NativeFormatOptions)options[typeof(NativeFormatOptions)]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsEmail"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public EmailOptions Email { get { return emailOptions; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ExportOptionsPrintPreview"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PrintPreviewOptions PrintPreview { get { return printPreviewOptions; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public MailMessageExportOptions MailMessage { get { return (MailMessageExportOptions)options[typeof(MailMessageExportOptions)]; } }
		internal Dictionary<Type, ExportOptionsBase> Options { get { return options; } }
		internal List<ExportOptionKind> HiddenOptions { get { return hiddenOptions; } }
		#endregion
		#region Save Restore
		public void SaveToXml(string xmlFile) {
			SaveCore(Serializer, xmlFile);
		}
		public void RestoreFromXml(string xmlFile) {
			RestoreCore(Serializer, xmlFile);
		}
		public void SaveToRegistry(string path) {
			SaveCore(new RegistryXtraSerializer(), path);
		}
		public void RestoreFromRegistry(string path) {
			RestoreCore(new RegistryXtraSerializer(), path);
		}
		public void SaveToStream(Stream stream) {
			long position = 0L;
			if(stream.CanSeek)
				position = stream.Position;
			SaveCore(Serializer, stream);
			if(stream.CanSeek)
				stream.Position = position;
		}
		public void RestoreFromStream(Stream stream) {
			long position = 0L;
			if(stream.CanSeek)
				position = stream.Position;
			RestoreCore(Serializer, stream);
			if(stream.CanSeek)
				stream.Position = position;
		}
		void SaveCore(XtraSerializer serializer, object path) {
			serializer.SerializeObject(this, path, "ExportOptions");
		}
		void RestoreCore(XtraSerializer serializer, object path) {
			this.Assign(new ExportOptions());
			serializer.DeserializeObject(this, path, "ExportOptions");
		}
		#endregion
		public void Assign(ExportOptions source) {
			foreach(Type type in source.options.Keys) {
				options[type].Assign(source.options[type]);
			}
			emailOptions.Assign(source.Email);
			printPreviewOptions.Assign(source.PrintPreview);
			foreach(ExportOptionKind optionKind in EnumExtensions.GetValues(typeof(ExportOptionKind))) {
				SetOptionVisibility(optionKind, source.GetOptionVisibility(optionKind));
			}
		}
		public void SetOptionVisibility(ExportOptionKind optionKind, bool visible) {
			if(visible && !GetOptionVisibility(optionKind))
				hiddenOptions.Remove(optionKind);
			if(!visible && GetOptionVisibility(optionKind))
				hiddenOptions.Add(optionKind);
		}
		public void SetOptionsVisibility(ExportOptionKind[] optionKinds, bool visible) {
			foreach(ExportOptionKind optionKind in optionKinds) {
				SetOptionVisibility(optionKind, visible);
			}
		}
		public bool GetOptionVisibility(ExportOptionKind optionKind) {
			return !hiddenOptions.Contains(optionKind);
		}
		internal void UpdateDefaultFileName(string oldValue, string newValue) {
			if(oldValue == printPreviewOptions.DefaultFileName) {
				printPreviewOptions.DefaultFileName = newValue;
			}
			if(oldValue == Html.Title)
				Html.Title = newValue;
			if(oldValue == Mht.Title)
				Mht.Title = newValue;
			if(oldValue == MailMessage.Title)
				MailMessage.Title = newValue;
		}
		bool ShouldSerializePdf() {
			return Pdf.ShouldSerialize();
		}
		bool ShouldSerializeXls() {
			return Xls.ShouldSerialize();
		}
		bool ShouldSerializeXlsx() {
			return Xlsx.ShouldSerialize();
		}
		bool ShouldSerializeText() {
			return Text.ShouldSerialize();
		}
		bool ShouldSerializeCsv() {
			return Csv.ShouldSerialize();
		}
		bool ShouldSerializeImage() {
			return Image.ShouldSerialize();
		}
		bool ShouldSerializeHtml() {
			return Html.ShouldSerialize();
		}
		bool ShouldSerializeMht() {
			return Mht.ShouldSerialize();
		}
		bool ShouldSerializeRtf() {
			return Rtf.ShouldSerialize();
		}
		bool ShouldSerializeNativeFormat() {
			return NativeFormat.ShouldSerialize();
		}
		bool ShouldSerializeEmail() {
			return Email.ShouldSerialize();
		}
		bool ShouldSerializePrintPreview() {
			return PrintPreview.ShouldSerialize();
		}
		bool ShouldSerializeMailMessage() {
			return MailMessage.ShouldSerialize();
		}
		internal bool ShouldSerialize() {
			return ShouldSerializeCsv() || ShouldSerializeEmail() || ShouldSerializeHtml() || ShouldSerializeImage() ||
				ShouldSerializeMht() || ShouldSerializeNativeFormat() || ShouldSerializePdf() || ShouldSerializePrintPreview() ||
				ShouldSerializeRtf() || ShouldSerializeText() || ShouldSerializeXls() || ShouldSerializeXlsx() ||
				ShouldSerializeMailMessage();
		}
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			switch(propertyName) {
				case "Pdf":
					return ShouldSerializePdf();
				case "Xls":
					return ShouldSerializeXls();
				case "Xlsx":
					return ShouldSerializeXlsx();
				case "Text":
					return ShouldSerializeText();
				case "Csv":
					return ShouldSerializeCsv();
				case "Image":
					return ShouldSerializeImage();
				case "Html":
					return ShouldSerializeHtml();
				case "Mht":
					return ShouldSerializeMht();
				case "Rtf":
					return ShouldSerializeRtf();
				case "NativeFormat":
					return ShouldSerializeNativeFormat();
				case "Email":
					return ShouldSerializeEmail();
				case "PrintPreview":
					return ShouldSerializePrintPreview();
				case "MailMessage":
					return ShouldSerializeMailMessage();
			}
			return true;
		}
	}
}
