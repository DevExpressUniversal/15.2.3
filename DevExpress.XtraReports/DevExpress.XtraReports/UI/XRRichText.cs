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

using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting.Native.RichText;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.Utils.Serializing;
using DevExpress.XtraRichEdit;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	#region XRRichText
	[
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRRichText.bmp"),
	XRDesigner("DevExpress.XtraReports.Design.XRRichTextDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRRichTextDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRRichText", "RichText"),
	XRToolboxSubcategoryAttribute(0, 2),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRRichText.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRRichText.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRRichText : XRRichTextBase, ISupportInitialize, IDisplayNamePropertyContainer {
		class CustomDocumentModelHelper : DocumentModelHelper {
			XRRichText richText;
			public CustomDocumentModelHelper(XRRichText richText)
				: base(richText.DocumentModel) {
				this.richText = richText;
			}
			protected override void Load(Stream data, string sourceUri, XRRichTextStreamType streamType) {
				base.Load(data, sourceUri, streamType);
				richText.UpdateInternalRtf();
			}
			protected override void LoadPlainText(Stream data) {
				base.LoadPlainText(data);
				richText.SetContentFont();
			}
		}
		#region RichTextBox-similar methods
		public void LoadFile(string path) {
			new CustomDocumentModelHelper(this).Load(path);
		}
		public void LoadFile(string path, XRRichTextStreamType streamType) {
			new CustomDocumentModelHelper(this).Load(path, streamType);
		}
		public void LoadFile(Stream data, XRRichTextStreamType streamType) {
			new CustomDocumentModelHelper(this).Load(data, streamType);
		}
		public void SaveFile(string path) {
			SaveFile(path, XRRichTextStreamType.RtfText);
		}
		public void SaveFile(string path, XRRichTextStreamType streamType) {
			new CustomDocumentModelHelper(this).Save(path, streamType);
		}
		public void SaveFile(Stream data, XRRichTextStreamType streamType) {
			new CustomDocumentModelHelper(this).Save(data, streamType);
		}
		public void Clear() {
			XtraRichTextEditHelper.ClearDocumentModel(documentModel);
			modelIsValid = true;
		}
		#endregion
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeString(PropertyNames.Rtf, Rtf);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			Rtf = serializer.DeserializeString(PropertyNames.Rtf, "");
		}
		#endregion
		#region overriden properties
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextKeepTogether"),
#endif
 DefaultValue(false)]
		public override bool KeepTogether {
			get { return base.KeepTogether; }
			set { base.KeepTogether = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextCanGrow"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Browsable(true),
		XtraSerializableProperty,
		]
		public override bool CanGrow {
			get { return base.CanGrow; }
			set { base.CanGrow = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextCanShrink"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Browsable(true),
		XtraSerializableProperty,
		]
		public override bool CanShrink {
			get { return base.CanShrink; }
			set { base.CanShrink = value; }
		}
		internal override bool HasDisplayDataBinding {
			get {
				return DataBindings[PropertyNames.Html] != null || DataBindings[PropertyNames.Rtf] != null;
			}
		}
		#endregion
		#region fields & properties
		DocumentModel documentModel;
		GraphicsDocumentPrinter printer;
		bool loading;
		bool modelIsValid;
		internal DocumentModel DocumentModel {
			get {
				if(!modelIsValid) {
					XtraRichTextEditHelper.ImportRtfTextToDocManager(Rtf, documentModel);
					modelIsValid = true;
				}
				return documentModel;
			}
		}
		[
		Browsable(false),
		Bindable(true),
		DefaultValue(""),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public string Html {
			get { return string.Empty; }
			set {
				using(MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value == null ? string.Empty : value))) {
					XtraRichTextEditHelper.ImportHtmlTextStreamToDocManager(stream, null, DocumentModel, true);
					UpdateInternalRtf();
				}
			}
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRRichTextRtf")]
#endif
		public override string Rtf {
			get {
				return base.Rtf;
			}
			set {
				if(!string.IsNullOrEmpty(value) && !RtfTags.IsRtfContent(value)) {
					XtraRichTextEditHelper.ImportPlainTextToDocManager(value, documentModel);
					value = XtraRichTextEditHelper.GetRtfFromDocManager(documentModel);
					base.Rtf = value;
				}
				if(base.Rtf != value) {
					modelIsValid = false;
					base.Rtf = value;
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextText"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Bindable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Text {
			get { return XtraRichTextEditHelper.GetTextFromDocManager(DocumentModel); }
			set {
				if(!string.IsNullOrEmpty(value) && RtfTags.IsRtfContent(value))
					XtraRichTextEditHelper.ImportRtfTextToDocManager(value, DocumentModel);
				else
					XtraRichTextEditHelper.ImportPlainTextToDocManager(value == null ? string.Empty : value, DocumentModel);
				SetContentFont();
				SetContentForeColor();
				UpdateInternalRtf();
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextLines"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRRichText.Lines"),
		SRCategory(ReportStringId.CatData),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.Utils.UI.StringArrayEditor," + AssemblyInfo.SRAssemblyUtilsUIFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public string[] Lines {
			get { return XtraRichTextEditHelper.GetTextLinesFromDocManager(DocumentModel); }
			set {
				XtraRichTextEditHelper.ImportPlainTextLinesToDocManager(value, DocumentModel, GetEffectiveFont());
				SetContentFont();
				UpdateInternalRtf();
			}
		}
		#endregion
		public XRRichText()
			: base() {
			documentModel = XtraRichTextEditHelper.CreateDocumentModel();
			printer = new BreaksFreeDocumentPrinter(documentModel);
		}
		public override Font GetEffectiveFont() {
			return this.Font;
		}
		public override Color GetEffectiveForeColor() {
			return this.ForeColor;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				base.Dispose(disposing);
				if(printer != null) {
					printer.Dispose();
					printer = null;
				}
				if(documentModel != null) {
					documentModel.Dispose();
					documentModel = null;
				}
			} else
				base.Dispose(disposing);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new RichTextBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			RichTextBrick richTextBrick = (RichTextBrick)brick;
			if(DesignMode) {
				if(EmbeddedFieldsHelper.HasEmbeddedFieldNames(this))
					richTextBrick.RtfText = GetDisplayPropertyWithDisplayColumnNames();
				else if(ShowBindingString(Text))
					richTextBrick.Text = GetDisplayPropertyBindingString();
				else
					richTextBrick.SetDocManagerAndPrinter(DocumentModel, printer);
			} else
				richTextBrick.RtfText = !IsEmptyValue(Rtf) ? Rtf : NullValueText;
		}
		protected internal override void GetStateFromBrick(VisualBrick brick) {
			base.GetStateFromBrick(brick);
			Rtf = ((RichTextBrick)brick).RtfText;
		}
		protected override float CalculateBrickHeight(VisualBrick brick) {
			RectangleF rect = new RectangleF(0, 0, 0, ((RichTextBrick)brick).EffectiveHeight);
			rect = brick.Style.InflateBorderWidth(rect, GraphicsDpi.Document);
			float heightInDoc = brick.Padding.InflateHeight(rect.Height, GraphicsDpi.Document);
			float heightInControlDpi = XRConvert.Convert(heightInDoc, GraphicsDpi.Document, Dpi);
			return UpdateAutoHeight(brick, (int)Math.Ceiling(heightInControlDpi));
		}
		protected internal override MailMergeFieldInfosCalculator GetMailMergeFieldInfosCalculator() {
			return RichTextMailMergeFieldInfosCalculator.Instance;
		}
		internal override void SetDisplayPropertyFromTextWithDisplayColumnNames(string source) {
			if(string.IsNullOrEmpty(source) || (!string.IsNullOrEmpty(source) && !RtfTags.IsRtfContent(source))) {
				XtraRichTextEditHelper.ImportPlainTextToDocManager(source, documentModel);
				source = XtraRichTextEditHelper.GetRtfFromDocManager(documentModel);
			}
			base.SetDisplayPropertyFromTextWithDisplayColumnNames(source);
		}
		protected override void OnFontChanged() {
			SetContentFont();
			UpdateInternalRtf();
		}
		protected override void OnForeColorChanged() {
			SetContentForeColor();
			UpdateInternalRtf();
		}
		void SetContentFont() {
			if(!loading)
				XtraRichTextEditHelper.SetContentFont(DocumentModel, GetEffectiveFont());
		}
		void SetContentForeColor() {
			if(!loading)
				XtraRichTextEditHelper.SetContentForeColor(DocumentModel, GetEffectiveForeColor());
		}
#if DEBUGTEST
		internal
#endif
 void UpdateInternalRtf() {
			base.Rtf = new CustomDocumentModelHelper(this).GetRtf();
		}
		protected override TextEditMode TextEditMode {
			get {
				return TextEditMode.None;
			}
		}
		#region ISupportInitialize implmentation
		public void BeginInit() {
			loading = true;
		}
		public void EndInit() {
			loading = false;
		}
		#endregion
		#region IDisplayNamePropertyContainer implmentation
		string IDisplayNamePropertyContainer.GetRealPropertyValue(string source) {
			MailMergeFieldInfosCalculator calculator = !string.IsNullOrEmpty(source) && !RtfTags.IsRtfContent(source) ? PlainTextMailMergeFieldInfosCalculator.Instance : GetMailMergeFieldInfosCalculator();
			return EmbeddedFieldsHelper.GetTextFromTextWithDisplayColumnNames(Report, calculator, source);
		}
		#endregion
	}
	#endregion // XRRichText
}
