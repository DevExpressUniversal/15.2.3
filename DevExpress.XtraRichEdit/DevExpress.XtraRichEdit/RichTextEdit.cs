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
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Controls.Rtf;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.Rtf;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Utils.Paint;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
namespace DevExpress.XtraEditors.Repository {
	public class RichTextEditHorizontalScrollbarOptions : HorizontalScrollbarOptions {
		protected internal override bool ShouldSerializeVisibility() { 
			return Visibility != RichEditScrollbarVisibility.Hidden; 
		}
		protected internal override void ResetVisibility() {
			Visibility = RichEditScrollbarVisibility.Hidden;
		}
	}
	public class RichTextEditBehaviorOptions : RichEditBehaviorOptions {
		protected internal override bool ShouldSerializeSave() {
			return Save != DocumentCapability.Disabled; 
		}
		protected internal override bool ShouldSerializeSaveAs() {
			return SaveAs != DocumentCapability.Disabled; 
		}
		protected internal override bool ShouldSerializePrinting() { 
			return Printing != DocumentCapability.Disabled; 
		}
		protected internal override bool ShouldSerializeOpen() { 
			return Open != DocumentCapability.Disabled; 
		}
		protected internal override bool ShouldSerializeCreateNew() { 
			return CreateNew != DocumentCapability.Disabled; 
		}
		protected internal override bool ShouldSerializeZooming() { 
			return Zooming != DocumentCapability.Disabled; 
		}
		protected internal override void ResetSave() { 
			Save = DocumentCapability.Disabled; 
		}
		protected internal override void ResetSaveAs() { 
			SaveAs = DocumentCapability.Disabled; 
		}
		protected internal override void ResetPrinting() { 
			Printing = DocumentCapability.Disabled; 
		}
		protected internal override void ResetOpen() { 
			Open = DocumentCapability.Disabled; 
		}
		protected internal override void ResetCreateNew() {
			CreateNew = DocumentCapability.Disabled; 
		}
		protected internal override void ResetZooming() { 
			Zooming = DocumentCapability.Disabled;
		}
	}
	#region RepositoryItemRichTextEdit
	public class RepositoryItemRichTextEdit : RepositoryItem {
		int horzIndent, vertIndent, maxHeight, customHeight;
		DocumentFormat documentFormat;
		Encoding encoding;
		bool acceptsTab;
		bool showCaretInReadOnly;
		bool useGdiPlus;
		RepositoryItemRichTextEditFontCacheManager fontCacheManager;
		readonly RichTextEditDocumentExportOptions optionsExport;
		readonly RichTextEditDocumentImportOptions optionsImport;
		readonly RichTextEditBehaviorOptions optionsBehavior;
		readonly RichTextEditHorizontalScrollbarOptions optionsHorizontalScrollbar;
		readonly VerticalScrollbarOptions optionsVerticalScrollbar;
		public RepositoryItemRichTextEdit() {
			horzIndent = 4;
			vertIndent = 0;
			maxHeight = 250;
			customHeight = -1;
			BestFitWidth = 200;
			documentFormat = DocumentFormat.Undefined;
			encoding = Encoding.Default;
			fontCacheManager = new RepositoryItemRichTextEditFontCacheManager(DocumentLayoutUnitConverter.Create(DevExpress.Office.DocumentLayoutUnit.Pixel, DevExpress.Office.DocumentModelDpi.Dpi));
			optionsExport = new RichTextEditDocumentExportOptions();
			optionsImport = new RichTextEditDocumentImportOptions();
			optionsBehavior = new RichTextEditBehaviorOptions();
			optionsHorizontalScrollbar = new RichTextEditHorizontalScrollbarOptions();
			optionsVerticalScrollbar = new VerticalScrollbarOptions();
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (fontCacheManager != null) {
						fontCacheManager.ForceReleaseFontCache();
						fontCacheManager = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemRichTextEdit source = item as RepositoryItemRichTextEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if (source == null)
					return;
				this.horzIndent = source.HorizontalIndent;
				this.vertIndent = source.VerticalIndent;
				this.maxHeight = source.MaxHeight;
				this.customHeight = source.CustomHeight;
				this.encoding = source.Encoding;
				this.documentFormat = source.DocumentFormat;
				this.acceptsTab = source.acceptsTab;
				this.showCaretInReadOnly = source.ShowCaretInReadOnly;
				this.useGdiPlus = source.useGdiPlus;
				this.optionsExport.CopyFrom(source.OptionsExport);
				this.optionsImport.CopyFrom(source.OptionsImport);
				this.optionsBehavior.CopyFrom(source.OptionsBehavior);
				this.optionsHorizontalScrollbar.CopyFrom(source.optionsHorizontalScrollbar);
				this.optionsVerticalScrollbar.CopyFrom(source.optionsVerticalScrollbar);
			}
			finally {
				EndUpdate();
			}
		}
		#region Properties
		[Browsable(false)]
		public override string EditorTypeName { get { return "RichTextEdit"; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override bool AutoHeight { get { return false; } }
		[Category(CategoryName.Appearance), 
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditHorizontalIndent"),
#endif
 DefaultValue(4)]
		public int HorizontalIndent {
			get { return horzIndent; }
			set {
				if (HorizontalIndent == value) return;
				horzIndent = value;
				if (horzIndent < 0) horzIndent = 0;
				OnPropertiesChanged();
			}
		}
		[Category(CategoryName.Appearance), 
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditVerticalIndent"),
#endif
 DefaultValue(0)]
		public int VerticalIndent {
			get { return vertIndent; }
			set {
				if (VerticalIndent == value) return;
				vertIndent = value;
				if (vertIndent < 0) vertIndent = 0;
				OnPropertiesChanged();
			}
		}
		[Category(CategoryName.Appearance), 
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditCustomHeight"),
#endif
 DefaultValue(-1)]
		public int CustomHeight {
			get { return customHeight; }
			set {
				if (CustomHeight == value) return;
				customHeight = value;
				if (customHeight < -1) customHeight = -1;
				OnPropertiesChanged();
			}
		}
		[Category(CategoryName.Appearance), 
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditMaxHeight"),
#endif
 DefaultValue(250)]
		public int MaxHeight {
			get { return maxHeight; }
			set {
				if (MaxHeight == value) return;
				maxHeight = value;
				if (maxHeight < 0) maxHeight = 0;
				OnPropertiesChanged();
			}
		}
		[Category(CategoryName.Behavior), 
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditAcceptsTab"),
#endif
 DefaultValue(false)]
		public bool AcceptsTab {
			get { return acceptsTab; }
			set {
				if (AcceptsTab == value) return;
				acceptsTab = value;
				OnPropertiesChanged();
			}
		}
		[Category(CategoryName.Behavior),  DefaultValue(false)]
		internal bool UseGdiPlus {
			get {
				return useGdiPlus;
			}
			set {
				if (useGdiPlus == value)
					return;
				useGdiPlus = value;
				OnPropertiesChanged();
			}
		}
		[Category(CategoryName.Behavior), 
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditShowCaretInReadOnly"),
#endif
 DefaultValue(true)]
		public bool ShowCaretInReadOnly {
			get { return showCaretInReadOnly; }
			set {
				if (ShowCaretInReadOnly == value) return;
				showCaretInReadOnly = value;
				OnPropertiesChanged();
			}
		}
		internal RepositoryItemRichTextEditFontCacheManager FontCacheManager { get { return fontCacheManager; } }
		#region DocumentFormat
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditDocumentFormat")]
#endif
		public DocumentFormat DocumentFormat {
			get { return documentFormat; }
			set {
				if (value == documentFormat)
					return;
				documentFormat = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual void ResetDocumentFormat() {
			this.DocumentFormat = DocumentFormat.Undefined;
		}
		protected internal virtual bool ShouldSerializeDocumentFormat() {
			return DocumentFormat != DocumentFormat.Undefined;
		}
		#endregion
		#region Encoding
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditEncoding"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.Repaint),
		TypeConverter(typeof(DevExpress.Office.Design.EncodingConverter))
		]
		public Encoding Encoding {
			get { return encoding; }
			set {
				if (encoding == value)
					return;
				encoding = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual void ResetEncoding() {
			this.Encoding = Encoding.Default;
		}
		protected internal virtual bool ShouldSerializeEncoding() {
			return !Object.Equals(Encoding, Encoding.Default);
		}
		#endregion
		#region EncodingWebName
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string EncodingWebName {
			get {
				if (Encoding != null)
					return Encoding.WebName;
				else
					return String.Empty;
			}
			set { this.Encoding = GetEncodingByWebName(value); }
		}
		protected internal virtual bool ShouldSerializeEncodingWebName() {
			return !Object.Equals(Encoding, Encoding.Default);
		}
		#endregion
		#region OptionsExport
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditOptionsExport"),
#endif
NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichTextEditDocumentExportOptions OptionsExport { get { return optionsExport; } }
		#endregion
		#region OptionsImport
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditOptionsImport"),
#endif
NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichTextEditDocumentImportOptions OptionsImport { get { return optionsImport; } }
		#endregion
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditOptionsBehavior"),
#endif
NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichTextEditBehaviorOptions OptionsBehavior { get { return optionsBehavior; } }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditOptionsVerticalScrollbar"),
#endif
NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public VerticalScrollbarOptions OptionsVerticalScrollbar { get { return optionsVerticalScrollbar; } }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RepositoryItemRichTextEditOptionsHorizontalScrollbar"),
#endif
NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichTextEditHorizontalScrollbarOptions OptionsHorizontalScrollbar { get { return optionsHorizontalScrollbar; } }
		#endregion
		protected internal virtual Encoding GetEncodingByWebName(string value) {
			EncodingInfo[] encodingInfos = Encoding.GetEncodings();
			int count = encodingInfos.Length;
			for (int i = 0; i < count; i++) {
				Encoding encoding = encodingInfos[i].GetEncoding();
				if (String.Compare(encoding.WebName, value, true) == 0)
					return encoding;
			}
			return Encoding.Default;
		}
		static void RegisterRepositoryItemRichTextEdit() {
			if (EditorRegistrationInfo.Default.Editors.Contains("RichTextEdit"))
				return;
			EditorClassInfo ci = new EditorClassInfo("RichTextEdit", typeof(RichTextEdit), typeof(RepositoryItemRichTextEdit), typeof(RichTextEditViewInfo), new RichTextEditPainter(), true, null, typeof(DevExpress.Accessibility.BaseEditAccessible));
			EditorRegistrationInfo.Default.Editors.Add(ci);
		}
		static RepositoryItemRichTextEdit() {
			RegisterRepositoryItemRichTextEdit();
		}
		public static void Register() { }
		protected override object GetEditValueForExport(object editValue, ExportTarget exportTarget) {
			if(exportTarget == ExportTarget.Csv || exportTarget == ExportTarget.Xls || exportTarget == ExportTarget.Xlsx) return ConvertEditValueToPlainText(editValue);
			return base.GetEditValueForExport(editValue, exportTarget);
		}
		public override DevExpress.XtraPrinting.IVisualBrick GetBrick(PrintCellHelperInfo info) {
			RichTextBrick brick = new RichTextBrick();
			SetCommonBrickProperties(brick, info);
			brick.Style = CreateBrickStyle(info, "rtf");
			brick.Hint = String.Empty;
			string text = info.DisplayText;
			if (!String.IsNullOrEmpty(text) && text.StartsWith("{\\rtf"))
				brick.RtfText = ConvertEditValueToRtfText(text);
			else {
				if (ShouldUseEditValue(info))
					brick.RtfText = ConvertEditValueToRtfText(info.EditValue);
				else
					brick.Text = text;
			}
			brick.BaseFont = info.Appearance.Font;
			if(brick.Text.Contains(Environment.NewLine)) brick.Style.StringFormat = BrickStringFormat.Create(brick.Style.TextAlignment, true);
			return brick;
		}
		protected override BrickStyle CreateBrickStyle(PrintCellHelperInfo info, string type) {
			BrickStyle style = base.CreateBrickStyle(info, type);
			SetupTextBrickStyleProperties(info, style);
			style.Padding = new PaddingInfo(this.HorizontalIndent, this.HorizontalIndent, this.VerticalIndent, this.VerticalIndent);
			return style;
		}
		protected internal virtual bool ShouldUseEditValue(PrintCellHelperInfo info) {
			return Object.Equals(info.EditValue, info.DisplayText) || (info.EditValue != null && Object.Equals(info.EditValue.ToString(), info.DisplayText));
		}
		protected internal virtual DocumentModel CreateDocumentModel() {
			DocumentModel result = new DocumentModel(RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies());
			result.ShouldApplyAppearanceProperties = true;
			result.LayoutUnit = DevExpress.Office.DocumentLayoutUnit.Pixel;
			result.FontCacheManager = fontCacheManager;
			result.DocumentExportOptions.CopyFrom(OptionsExport);
			result.DocumentImportOptions.CopyFrom(OptionsImport);
			return result;
		}
		public virtual string ConvertEditValueToPlainText(object editValue) {
			return ConvertEditValueSpecificFormatString(editValue, DocumentFormat.PlainText);
		}
		protected internal virtual string ConvertEditValueToRtfText(object editValue) {
			return ConvertEditValueSpecificFormatString(editValue, DocumentFormat.Rtf);
		}
		protected internal virtual string ConvertEditValueSpecificFormatString(object editValue, DocumentFormat format) {
			using (DocumentModel documentModel = CreateDocumentModel()) {
				EditValueToDocumentModelConverter toDocumentModel = CreateEditValueToDocumentModelConverter(editValue);
				toDocumentModel.ConvertToDocumentModel(documentModel, editValue);
				DocumentModelToStringConverter toRtfString = new DocumentModelToStringConverter(format, Encoding.Default);
				return toRtfString.ConvertToEditValue(documentModel) as String;
			}
		}
		protected internal virtual bool ShouldUseByteArrayConverter(object editValue) {
			if (DocumentFormat == DocumentFormat.Undefined)
				return editValue != null && editValue is byte[];
			return DocumentFormat == DocumentFormat.OpenDocument || DocumentFormat == DocumentFormat.OpenXml;
		}
		protected internal virtual EditValueToDocumentModelConverter CreateEditValueToDocumentModelConverter(object editValue) {
			if (ShouldUseByteArrayConverter(editValue))
				return new ByteArrayEditValueToDocumentModelConverter(DocumentFormat, Encoding);
			else
				return new StringEditValueToDocumentModelConverter(DocumentFormat, Encoding);
		}
		protected internal virtual DocumentModelToEditValueConverter CreateDocumentModelToEditValueConverter(object oldEditValue) {
			if (ShouldUseByteArrayConverter(oldEditValue))
				return new DocumentModelToByteArrayConverter(DocumentFormat, Encoding);
			else
				return new DocumentModelToStringConverter(DocumentFormat, Encoding);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
	public class RepositoryItemRichTextEditFontCacheManager : GdiPlusFontCacheManager {
		DevExpress.Office.Drawing.FontCache sharedFontCache;
		public RepositoryItemRichTextEditFontCacheManager(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		public override DevExpress.Office.Drawing.FontCache CreateFontCache() {
			if (sharedFontCache == null)
				sharedFontCache = base.CreateFontCache();
			return sharedFontCache;
		}
		public override void ReleaseFontCache(DevExpress.Office.Drawing.FontCache cache) {
		}
		public void ForceReleaseFontCache() {
			if (sharedFontCache != null) {
				base.ReleaseFontCache(sharedFontCache);
				sharedFontCache = null;
			}
		}
	}
}
namespace DevExpress.XtraRichEdit.Export {
	#region RichTextEditDocumentExportOptions
	public class RichTextEditDocumentExportOptions : RichEditDocumentExportOptions {
		public RichTextEditDocumentExportOptions() {
		}
		protected internal override RtfDocumentExporterOptions CreateRtfOptions() {
			return new RichTextEditRtfDocumentExporterOptions();
		}
	}
	#endregion
	#region RichTextEditRtfDocumentExporterOptions
	public class RichTextEditRtfDocumentExporterOptions : RtfDocumentExporterOptions {
		protected internal override RtfDocumentExporterCompatibilityOptions CreateCompatibilityOptions() {
			return new RichTextEditRtfDocumentExporterCompatibilityOptions();
		}
	}
	#endregion
	#region RichTextEditRtfDocumentExporterCompatibilityOptions
	public class RichTextEditRtfDocumentExporterCompatibilityOptions : RtfDocumentExporterCompatibilityOptions {
		#region DuplicateObjectAsMetafile
		protected internal override bool ShouldSerializeDuplicateObjectAsMetafile() {
			return DuplicateObjectAsMetafile != false;
		}
		protected internal override void ResetDuplicateObjectAsMetafile() {
			DuplicateObjectAsMetafile = false;
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Import {
	#region RichTextEditDocumentImportOptions
	public class RichTextEditDocumentImportOptions : RichEditDocumentImportOptions {
	}
	#endregion
}
namespace DevExpress.XtraEditors {
	#region RichTextEdit
	[DXToolboxItem(false),
	ToolboxBitmap(typeof(RichEditControl), DevExpress.Utils.ControlConstants.BitmapPath + "RichTextEdit.bmp"),
	 Description("Provides advanced text entry and editing features.")
	]
	public class RichTextEdit : BaseEdit {
		#region Fields
		InplaceRichEditControl innerControl;
		#endregion
		public RichTextEdit() {
		}
		#region Properties
		[Browsable(false)]
		public override string EditorTypeName { get { return "RichTextEdit"; } }
		[Category(CategoryName.Properties),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemRichTextEdit Properties { get { return base.Properties as RepositoryItemRichTextEdit; } }
		protected internal new RichTextEditViewInfo ViewInfo { get { return base.ViewInfo as RichTextEditViewInfo; } }
		public override object EditValue {
			get { return base.EditValue; }
			set {
				if (Object.Equals(EditValue, value))
					return;
				if (innerControl != null && (value is String || value is byte[])) {
					AssignInnerControlContent(value);
				}
				else if (innerControl != null && (Object.ReferenceEquals(value, null) || Object.Equals(DBNull.Value, value))) {
					AssignInnerControlContent(String.Empty);
				}
				else
					base.EditValue = value;
			}
		}
		[Browsable(false)]
		public override bool IsEditorActive {
			get {
				IContainerControl container = GetContainerControl();
				if (container == null)
					return EditorContainsFocus;
				return container.ActiveControl == this || (innerControl != null && container.ActiveControl == innerControl);
			}
		}
		[Browsable(false)]
		public override bool IsNeedFocus {
			get {
				return true;
			}
		}
		protected internal override Control InnerControl { get { return innerControl; } }
		#endregion
		protected override void OnHandleCreated(EventArgs e) {
			CreateInnerRichEdit();
			base.OnHandleCreated(e);
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			DestroyInnerRichEdit();
			base.OnHandleDestroyed(e);
		}
		protected internal virtual void AssignInnerControlContent(object editValue) {
			EditValueToDocumentModelConverter converter = Properties.CreateEditValueToDocumentModelConverter(editValue);
			converter.ConvertToDocumentModel(innerControl.DocumentModel, editValue);
			innerControl.ApplyFontAndForeColor();
		}
		protected internal virtual object CalculateEditValue() {
			if (innerControl.DocumentModel.IsEmpty && !innerControl.DocumentModel.Modified) {
				object editValue = EditValue;
				if (Object.Equals(editValue, lastEmptyEditValue) || Object.ReferenceEquals(editValue, null) || Object.Equals(DBNull.Value, editValue) || Object.Equals(editValue, String.Empty))
					return editValue;
			}
			int oldIndex = innerControl.GetDefaultStyleCharacterPropertiesIndex();
			try {
				innerControl.SetDefaultStyleCharacterPropertiesIndexSilently(innerControl.DefaultStyleCharacterPropertiesIndex);
				DocumentModelToEditValueConverter converter = Properties.CreateDocumentModelToEditValueConverter(this.EditValue);
				return converter.ConvertToEditValue(innerControl.DocumentModel);
			}
			finally {
				innerControl.SetDefaultStyleCharacterPropertiesIndexSilently(oldIndex);
			}
		}
		object lastEmptyEditValue;
		protected internal override void SetEmptyEditValue(object emptyEditValue) {
			this.lastEmptyEditValue = emptyEditValue;
			base.SetEmptyEditValue(emptyEditValue);
		}
		protected internal void CreateInnerRichEdit() {
			this.innerControl = new InplaceRichEditControl(Properties.UseGdiPlus || RichTextEditPainter.ShouldUseGdiPlus(null));
			this.innerControl.LayoutUnit = DocumentLayoutUnit.Pixel;
			DocumentModel documentModel = this.innerControl.DocumentModel;
			documentModel.BeginUpdate();
			try {
				documentModel.FontCacheManager = Properties.FontCacheManager;
			}
			finally {
				documentModel.EndUpdate();
			}
			documentModel.ShouldApplyAppearanceProperties = true;
			documentModel.DocumentExportOptions.CopyFrom(Properties.OptionsExport);
			documentModel.DocumentImportOptions.CopyFrom(Properties.OptionsImport);
			SubscribeInnerControlUIEvents();
			this.innerControl.Dock = DockStyle.Fill;
			this.innerControl.BorderStyle = BorderStyles.NoBorder;
			this.innerControl.ActiveViewType = RichEditViewType.Simple;
			this.innerControl.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			this.innerControl.MenuManager = this.MenuManager;
			ApplyProperties();
			this.innerControl.Options.Behavior.CopyFrom(this.Properties.OptionsBehavior);
			this.innerControl.Options.HorizontalScrollbar.CopyFrom(this.Properties.OptionsHorizontalScrollbar);
			this.innerControl.Options.VerticalScrollbar.CopyFrom(this.Properties.OptionsVerticalScrollbar);
			this.innerControl.AcceptsTab = this.Properties.AcceptsTab;
			this.innerControl.ShowCaretInReadOnly = this.Properties.ShowCaretInReadOnly;
			this.PropertiesChanged += OnPropertiesChanged;
			this.Controls.Clear();
			this.Controls.Add(innerControl);
			UpdatePaintAppearance();
			AssignInnerControlContent(this.EditValue);
			UpdatePaintAppearance();
			this.innerControl.ContentChanged += OnInnerControlContentChanged;
		}
		void UpdatePaintAppearance() {
			if (ViewInfo != null) {
				if (!ViewInfo.IsReady)
					ViewInfo.UpdatePaintAppearance();
				this.innerControl.Appearance.Text.Font = ViewInfo.PaintAppearance.Font;
				this.innerControl.Appearance.Text.Options.UseFont = true;
				this.innerControl.ForeColor = ViewInfo.PaintAppearance.ForeColor;
				this.innerControl.ActiveView.BackColor = ViewInfo.PaintAppearance.BackColor;
			}
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			if (innerControl != null){
				ToolTipControlInfo info = ((IToolTipControlClient)innerControl).GetObjectInfo(point);
				if (info != null)
					return info;
			}
			return base.GetToolTipInfo(point);
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			if (innerControl == null)
				return;
			ApplyProperties();
		}
		protected internal virtual void ApplyProperties() {
			this.innerControl.Views.SimpleView.Padding = new Padding(Properties.HorizontalIndent, Properties.VerticalIndent, 0, 0);
			this.innerControl.ReadOnly = Properties.ReadOnly;
			this.innerControl.AcceptsTab = Properties.AcceptsTab;
			this.innerControl.ShowCaretInReadOnly = Properties.ShowCaretInReadOnly;
		}
		public override bool IsNeededKey(KeyEventArgs e) {
			if (e.KeyCode == Keys.Escape)
				return false;
			if (e.KeyCode == Keys.Enter && e.Control)
				return false;
			if (!Properties.AcceptsTab && e.KeyCode == Keys.Tab)
				return false;
			return true;
		}
		#region Events forwarding
		void OnInnerControlKeyPress(object sender, KeyPressEventArgs e) { OnKeyPress(e);  }
		void OnInnerControlKeyUp(object sender, KeyEventArgs e) { OnKeyUp(e);  }
		void OnInnerControlKeyDown(object sender, KeyEventArgs e) { OnKeyDown(e); }
		protected virtual void OnInnerControlGotFocus(object sender, EventArgs e) { OnGotFocus(e); }
		protected virtual void OnInnerControlLostFocus(object sender, EventArgs e) { OnLostFocus(e); }
		protected virtual void OnInnerControlMouseEnter(object sender, EventArgs e) { OnMouseEnter(e); }
		protected virtual void OnInnerControlMouseLeave(object sender, EventArgs e) {
			if (!Bounds.Contains(MousePosition))
				OnMouseLeave(e);
		}
		protected virtual void OnInnerControlMouseUp(object sender, MouseEventArgs e) {
			OnMouseUp(ConvertToInnerControl(e));
		}
		protected virtual void OnInnerControlMouseDown(object sender, MouseEventArgs e) {
			OnMouseDown(ConvertToInnerControl(e));
		}
		protected virtual void OnInnerControlMouseMove(object sender, MouseEventArgs e) {
			OnMouseMove(ConvertToInnerControl(e));
		}
		protected virtual void OnInnerControlClick(object sender, EventArgs e) { OnClick(e); }
		protected virtual void OnInnerControlDoubleClick(object sender, EventArgs e) { OnDoubleClick(e); }
		MouseEventArgs ConvertToInnerControl(MouseEventArgs e) {
			return new MouseEventArgs(e.Button, e.Clicks, e.X + innerControl.Left, e.Y + innerControl.Top, e.Delta);
		}
		#endregion
		protected internal virtual void SubscribeInnerControlUIEvents() {
			innerControl.KeyDown += OnInnerControlKeyDown;
			innerControl.KeyUp += OnInnerControlKeyUp;
			innerControl.KeyPress += OnInnerControlKeyPress;
			innerControl.GotFocus += OnInnerControlGotFocus;
			innerControl.LostFocus += OnInnerControlLostFocus;
			innerControl.MouseEnter += OnInnerControlMouseEnter;
			innerControl.MouseLeave += OnInnerControlMouseLeave;
			innerControl.MouseDown += OnInnerControlMouseDown;
			innerControl.MouseUp += OnInnerControlMouseUp;
			innerControl.MouseMove += OnInnerControlMouseMove;
			innerControl.DoubleClick += OnInnerControlDoubleClick;
			innerControl.Click += OnInnerControlClick;
		}
		protected internal virtual void UnsubscribeInnerControlUIEvents() {
			innerControl.KeyDown -= OnInnerControlKeyDown;
			innerControl.KeyUp -= OnInnerControlKeyUp;
			innerControl.KeyPress -= OnInnerControlKeyPress;
			innerControl.GotFocus -= OnInnerControlGotFocus;
			innerControl.LostFocus -= OnInnerControlLostFocus;
			innerControl.MouseEnter -= OnInnerControlMouseEnter;
			innerControl.MouseLeave -= OnInnerControlMouseLeave;
			innerControl.MouseDown -= OnInnerControlMouseDown;
			innerControl.MouseUp -= OnInnerControlMouseUp;
			innerControl.MouseMove -= OnInnerControlMouseMove;
			innerControl.DoubleClick -= OnInnerControlDoubleClick;
			innerControl.Click -= OnInnerControlClick;
		}
		protected internal void DestroyInnerRichEdit() {
			if (innerControl != null) {
				if (!Properties.IsDisposed)
					base.EditValue = CalculateEditValue();
				UnsubscribeInnerControlUIEvents();
				innerControl.ContentChanged -= OnInnerControlContentChanged;
				this.PropertiesChanged -= OnPropertiesChanged;
				innerControl.Dispose();
				innerControl = null;
				this.Controls.Clear();
			}
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			if (innerControl != null) {
				if (ContainsFocus && !innerControl.ContainsFocus)
					this.innerControl.Focus();
			}
		}
		public override void SelectAll() {
			base.SelectAll();
			if (innerControl != null) {
				SelectAllCommand command = new SelectAllCommand(innerControl);
				command.Execute();
			}
		}
		public override void DeselectAll() {
			base.DeselectAll();
			if (innerControl != null) {
				innerControl.BeginUpdate();
				try {
					DocumentModel documentModel = innerControl.DocumentModel;
					documentModel.BeginUpdate();
					try {
						Selection selection = documentModel.Selection;
						selection.BeginUpdate();
						try {
							DocumentLogPosition pos = Algorithms.Min(documentModel.ActivePieceTable.DocumentEndLogPosition, selection.End);
							selection.End = pos;
							selection.Start = pos;
						}
						finally {
							selection.EndUpdate();
						}
					}
					finally {
						documentModel.EndUpdate();
					}
				}
				finally {
					innerControl.EndUpdate();
				}
			}
		}
		public override void Reset() {
			base.Reset();
			DeselectAll();
		}
		void OnInnerControlContentChanged(object sender, EventArgs e) {
			base.EditValue = CalculateEditValue();
		}
		public void LoadFile(string fileName) {
			if (Properties.DocumentFormat == DocumentFormat.Undefined)
				EditValue = GetStringFromFile(fileName);
			else
				innerControl.LoadDocument(fileName, Properties.DocumentFormat);
		}
		public void LoadText(string text) {
			EditValue = text;
		}
		string GetStringFromFile(string fileName) {
			try {
				using (TextReader streamReader = new StreamReader(fileName)) {
					return streamReader.ReadToEnd();
				}
			}
			catch {
				return String.Empty;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanShowDialog { get { return true; } }
	}
	#endregion
	[DXToolboxItem(false)]
	internal class InplaceRichEditControl : RichEditControl {
		int defaultStyleCharacterPropertiesIndex;
		internal InplaceRichEditControl(bool useGdiPlus)
			: base(useGdiPlus) {
		}
		internal int DefaultStyleCharacterPropertiesIndex { get { return defaultStyleCharacterPropertiesIndex; } }
		protected override void RegisterToolTipClient() {
		}
		protected internal override void RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			if ((changeActions & DocumentModelChangeActions.RaiseEmptyDocumentCreated) != 0 || (changeActions & DocumentModelChangeActions.RaiseDocumentLoaded) != 0)
				this.defaultStyleCharacterPropertiesIndex = GetDefaultStyleCharacterPropertiesIndex();
			base.RaiseDeferredEvents(changeActions);
		}
		protected internal int GetDefaultStyleCharacterPropertiesIndex() {
			if (DocumentModel == null)
				return -1;
			if (DocumentModel.CharacterStyles.Count <= 0)
				return -1;
			return DocumentModel.CharacterStyles[0].CharacterProperties.Index;
		}
		protected internal int SetDefaultStyleCharacterPropertiesIndexSilently(int value) {
			if (value < 0)
				return -1;
			if (DocumentModel == null)
				return -1;
			if (DocumentModel.CharacterStyles.Count <= 0)
				return -1;
			int result = DocumentModel.CharacterStyles[0].CharacterProperties.Index;
			DocumentModel.CharacterStyles[0].CharacterProperties.SetIndexInitial(value);
			return result;
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	#region RichTextEditViewInfo
	public class RichTextEditViewInfo : BaseEditViewInfo, IHeightAdaptable {
		RtfViewer viewer;
		public RichTextEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		protected internal RtfViewer Viewer {
			get {
				if (viewer == null)
					viewer = new RtfViewer(Item);
				return viewer;
			}
		}
		public override bool RequireClipping {
			get {
				return true;
			}
		}
		public RtfViewer GetViewer() {
			Viewer.CheckReady();
			return Viewer;
		}
		public Rectangle DocumentRect {
			get {
				Rectangle ret = new Rectangle(ClientRect.Location, ClientRect.Size);
				ret.Inflate(-Item.HorizontalIndent, -Item.VerticalIndent);
				return ret;
			}
		}
		internal void Update() {
			Viewer.Printer.ClearColumns();
		}
		internal Size GetDocumentSize(Graphics g) {
			DocumentLayoutUnitConverter unitConverter = Viewer.DocumentModel.LayoutUnitConverter;
			Rectangle ret = DocumentRect;
			return new Size(
				unitConverter.PixelsToLayoutUnits(ret.Width, g.DpiX),
				unitConverter.PixelsToLayoutUnits(ret.Height, g.DpiY));
		}
		internal Rectangle GetDocumentRectangle(Graphics g) {
			DocumentLayoutUnitConverter unitConverter = Viewer.DocumentModel.LayoutUnitConverter;
			Rectangle ret = DocumentRect;
			Size sf = GetDocumentSize(g);
			return new Rectangle(
				unitConverter.PixelsToLayoutUnits(ret.X, g.DpiX),
				unitConverter.PixelsToLayoutUnits(ret.Y, g.DpiY),
				sf.Width, sf.Height);
		}
		public new RepositoryItemRichTextEdit Item { get { return base.Item as RepositoryItemRichTextEdit; } }
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			Viewer.EditValue = EditValue;
		}
		int IHeightAdaptable.CalcHeight(DevExpress.Utils.Drawing.GraphicsCache cache, int width) {
			if (Item.CustomHeight >= 0) return Item.CustomHeight;
			int ret = Viewer.GetEditorHeight(cache.Graphics, width - Item.HorizontalIndent * 2 - 1, Item.MaxHeight, 1) + Item.VerticalIndent * 2 + 1;
			return ret;
		}
		public int CalcHeight(int width) {
			int ret = 0;
			GraphicsInfo.Default.AddGraphics(null);
			try {
				ret = Viewer.GetEditorHeight(GraphicsInfo.Default.Graphics, width - Item.HorizontalIndent * 2 - 1, Item.MaxHeight, 1) + Item.VerticalIndent * 2 + 1;
			}
			finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
			return ret;
		}
		public void LoadText(string text) {
			Viewer.EditValue = text;
		}
		public override void SetDisplayText(string newText, object editValue) {
			if (editValue != null && editValue.GetType() == typeof(byte[]) && newText == editValue.ToString())
				Viewer.EditValue = editValue;
			else
				Viewer.EditValue = newText;
		}
		public override void Dispose() {
			try {
				if (viewer != null) {
					viewer.Dispose();
					viewer = null;
				}
			}
			finally {
				base.Dispose();
			}
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			Viewer.ApplyAppearance(PaintAppearance);
		}
		public override AppearanceObject PaintAppearance {
			get { return base.PaintAppearance; }
			set {
				base.PaintAppearance = value;
			}
		}
		protected override void OnPaintAppearanceChanged() {
			Viewer.ApplyAppearance(PaintAppearance);
		}
		public override string UpdateGroupValueDisplayText(string groupValueText, object value) {
			RepositoryItemRichTextEdit repositoryItem = Viewer.RepositoryItem;
			if (repositoryItem.DocumentFormat == DocumentFormat.PlainText)
				return groupValueText;
			StringEditValueToDocumentModelConverter converter = new StringEditValueToDocumentModelConverter(repositoryItem.DocumentFormat, repositoryItem.Encoding);
			if (converter.CalculateActualDocumentFormat(groupValueText) == DocumentFormat.PlainText)
				return groupValueText;
			return repositoryItem.ConvertEditValueSpecificFormatString(groupValueText, DocumentFormat.PlainText);
		}
		public override Size CalcBestFit(Graphics g) {
			DocumentModel documentModel = Viewer.DocumentModel;
			FontInfo fontInfo = documentModel.FontCache[documentModel.MainPieceTable.Runs.Last.FontCacheIndex];
			return new Size(Item.BestFitWidth, fontInfo.LineSpacing);
		}
	}
	#endregion
}
namespace DevExpress.XtraEditors.Drawing {
	#region RichTextEditPainter
	public class RichTextEditPainter : BaseEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawRTF(info.ViewInfo as RichTextEditViewInfo, info.Cache);
		}
		public static void DrawRTF(RichTextEditViewInfo vi, GraphicsCache cache) {
			if (vi == null)
				return;
			using(GraphicsClipState clipState = cache.ClipInfo.SaveClip()) {			
				try {
					RectangleF clipBounds = cache.Graphics.ClipBounds;
					clipBounds.Intersect(vi.Bounds);
					cache.ClipInfo.SetClip(Rectangle.Ceiling(clipBounds));
					Graphics gr = cache.Graphics;
					RtfViewer rtfViewer = vi.GetViewer();
					using (GraphicsToLayoutUnitsModifier modifier = new GraphicsToLayoutUnitsModifier(gr, rtfViewer.DocumentModel.LayoutUnitConverter)) {
						using (MeasurementAndDrawingStrategy strategy = CreateMeasurementAndDrawingStrategy(cache, rtfViewer.DocumentModel, vi.Item.UseGdiPlus)) {
							strategy.Initialize();
							using (Painter gdiPainter = strategy.CreateDocumentPainter(new GraphicsCacheDrawingSurface(cache))) {
								WinFormsGraphicsDocumentLayoutExporterAdapter adapter = new WinFormsGraphicsDocumentLayoutExporterAdapter();
								using (GraphicsDocumentLayoutExporter painter = new GraphicsDocumentLayoutExporter(rtfViewer.DocumentModel, gdiPainter, adapter, vi.GetDocumentRectangle(gr), true, TextColors.Defaults)) {
									rtfViewer.Printer.Export2(gr, painter, vi.GetDocumentSize(gr));
								}
							}
						}
					}
				}
				finally {
					cache.ClipInfo.RestoreClip(clipState);
				}
			}
		}
		protected internal static MeasurementAndDrawingStrategy CreateMeasurementAndDrawingStrategy(GraphicsCache cache, DocumentModel documentModel, bool forceUseGdiPlus) {
			if (forceUseGdiPlus || ShouldUseGdiPlus(cache))
				return new GdiPlusMeasurementAndDrawingStrategy(documentModel);
			else
				return new GdiMeasurementAndDrawingStrategy(documentModel);
		}
		internal static bool ShouldUseGdiPlus(GraphicsCache cache) {
			XPaint painter = (cache == null) ? XPaint.Graphics : cache.Paint;
			return !(painter is XPaintMixed || painter is XPaintTextRender);
		}
	}
	#endregion
}
namespace DevExpress.XtraEditors.Controls.Rtf {
	#region VirtualDocumentPrinter
	public class VirtualDocumentPrinter : SimpleDocumentPrinter {
		public VirtualDocumentPrinter(DocumentModel documentModel)
			: base(documentModel) {
		}
		public Size VirtualColumnSize {
			get { return ColumnSize; }
			set {
				if (ColumnSize.Equals(value)) return;
				ColumnSize = value;
			}
		}
		protected internal override DocumentPrinterController CreateDocumentPrinterController() {
			return new PlatformDocumentPrinterController();
		}
		protected internal override DevExpress.XtraRichEdit.Layout.Engine.BoxMeasurer CreateMeasurer(Graphics gr) {
			if (RichTextEditPainter.ShouldUseGdiPlus(null))
				return new GdiPlusBoxMeasurer(DocumentModel, gr);
			else
				return new GdiBoxMeasurer(DocumentModel, gr);
		}
		public void Export2(Graphics graphics, IDocumentLayoutExporter exporter, Size columnSize) {
			int width = ColumnSize.Width;
			VirtualColumnSize = columnSize;
			if (width != ColumnSize.Width) {
				if (isFormatPerformed)
					Format(graphics, ColumnSize.Height);
				else
					Format();
			}
			Export(exporter);
		}
		protected override void Export(IDocumentLayoutExporter exporter) {
			IPageFloatingObjectExporter floatingObjectExporter = exporter as IPageFloatingObjectExporter;
			if (floatingObjectExporter != null && Controller.PageController.Pages.Count > 0) {
				floatingObjectExporter.ExportPageAreaCore(Controller.PageController.Pages[0], Controller.DocumentModel.MainPieceTable, page => ExportCore(exporter));
			}
			else
				base.Export(exporter);
		}
		void ExportCore(IDocumentLayoutExporter exporter) {
			base.Export(exporter);
		}
		bool isFormatPerformed = false;
		public bool IsFormatPerformed {
			get {
				return isFormatPerformed;
			}
			set {
				if (isFormatPerformed == value)
					return;
				isFormatPerformed = value;
			}
		}
		internal int GetEditorHeight(Graphics graphics, int width, int maxHeight, float scaleFactor) {
			DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
			using (GraphicsToLayoutUnitsModifier modifier = new GraphicsToLayoutUnitsModifier(graphics, unitConverter)) {
				Size newColumnSize = unitConverter.PixelsToLayoutUnits(new Size(width, maxHeight), graphics.DpiX, graphics.DpiY);
				base.ColumnSize = newColumnSize;
				int actualHeight = this.Format(graphics, newColumnSize.Height);
				isFormatPerformed = true;
				if (Controller.PageController.Pages.Count > 0) {
					DevExpress.XtraRichEdit.Layout.Page page = Controller.PageController.Pages[0];
					foreach (FloatingObjectBox box in page.GetSortedNonBackgroundFloatingObjects()) {
						actualHeight = Math.Max(box.ExtendedBounds.Bottom, actualHeight);
					}
				}
				return unitConverter.LayoutUnitsToPixels(actualHeight, graphics.DpiY);
			}
		}
		internal void ClearColumns() {
			VirtualColumnSize = Size.Empty;
		}
	}
	#endregion
	#region RtfViewer
	public class RtfViewer : IDisposable {
		#region Fields
		AppearanceObject appearance;
		DocumentModel documentModel;
		RtfImporter importer;
		VirtualDocumentPrinter printer;
		bool isReady;
		object editValue;
		readonly RepositoryItemRichTextEdit repositoryItem;
		#endregion
		public RtfViewer(RepositoryItemRichTextEdit repositoryItem) {
			Guard.ArgumentNotNull(repositoryItem, "repositoryItem");
			this.repositoryItem = repositoryItem;
			this.documentModel = repositoryItem.CreateDocumentModel();
			CreateSubsidiaryElements();
		}
		#region Properties
		public bool IsReady {
			get { return isReady; }
			set {
				isReady = value;
				if (!isReady)
					printer.IsFormatPerformed = false;
			}
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public VirtualDocumentPrinter Printer { get { return printer; } }
		public object EditValue {
			get { return editValue; }
			set {
				if (value == DBNull.Value) value = null;
				if (EditValue == value) return;
				editValue = value;
				this.IsReady = false;
			}
		}
		protected internal RepositoryItemRichTextEdit RepositoryItem { get { return repositoryItem; } }
		#endregion
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (printer != null) {
					printer.Dispose();
					printer = null;
				}
				if (importer != null) {
					importer.Dispose();
					importer = null;
				}
				if (documentModel != null) {
					documentModel.Dispose();
					documentModel = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public void Clear() {
			this.IsReady = false;
			DocumentModel.Reinitialize();
			CreateSubsidiaryElements();
		}
		void CreateSubsidiaryElements() {
			this.importer = new RtfImporter(DocumentModel, new RtfDocumentImporterOptions());
			this.printer = new VirtualDocumentPrinter(DocumentModel);
		}
		public void CheckReady() {
			if (IsReady) return;
			Clear();
			if (EditValue == null)
				return;
			LoadContent(EditValue);
		}
		public int GetEditorHeight(Graphics graphics, int width, int maxHeight, float scaleFactor) {
			CheckReady();
			return Printer.GetEditorHeight(graphics, width, maxHeight, scaleFactor);
		}
		protected void LoadContent(object value) {
			LoadContentCore(value);
			ApplyAppearance(appearance);
			this.IsReady = true;
		}
		protected void LoadContentCore(object content) {
			EditValueToDocumentModelConverter converter = repositoryItem.CreateEditValueToDocumentModelConverter(content);
			converter.ConvertToDocumentModel(DocumentModel, content);
		}
		protected internal void ApplyAppearance(AppearanceObject appearance) {
			if (appearance == null)
				return;
			this.appearance = appearance;
			CharacterProperties characterProperties = documentModel.CharacterStyles[0].CharacterProperties;
			if (ShouldApplyFont())
				CharacterPropertiesFontAssignmentHelper.AssignFont(characterProperties, appearance.Font);
			if (ShouldApplyForeColor())
				characterProperties.ForeColor = appearance.ForeColor;
		}
		protected internal virtual bool ShouldApplyForeColor() {
			switch (repositoryItem.OptionsBehavior.ForeColorSource) {
				default:
				case RichEditBaseValueSource.Auto:
					return true;
				case RichEditBaseValueSource.Control:
					return true;
				case RichEditBaseValueSource.Document:
					return false;
			}
		}
		protected internal virtual bool ShouldApplyFont() {
			switch (repositoryItem.OptionsBehavior.FontSource) {
				default:
				case RichEditBaseValueSource.Auto:
					return true;
				case RichEditBaseValueSource.Control:
					return true;
				case RichEditBaseValueSource.Document:
					return false;
			}
		}
	}
	#endregion
	#region DocumentModelToEditValueConverter (abstract class)
	public abstract class DocumentModelToEditValueConverter {
		readonly DocumentFormat documentFormat;
		readonly Encoding encoding;
		protected DocumentModelToEditValueConverter(DocumentFormat documentFormat, Encoding encoding) {
			if (encoding == null)
				encoding = Encoding.Default;
			if (documentFormat == DocumentFormat.Undefined)
				documentFormat = DocumentFormat.Rtf;
			this.documentFormat = documentFormat;
			this.encoding = encoding;
		}
		public DocumentFormat DocumentFormat { get { return documentFormat; } }
		public Encoding Encoding { get { return encoding; } }
		public abstract object ConvertToEditValue(DocumentModel documentModel);
	}
	#endregion
	#region EditValueToDocumentModelConverter
	public abstract class EditValueToDocumentModelConverter {
		readonly DocumentFormat documentFormat;
		readonly Encoding encoding;
		protected EditValueToDocumentModelConverter(DocumentFormat documentFormat, Encoding encoding) {
			if (encoding == null)
				encoding = Encoding.Default;
			this.documentFormat = documentFormat;
			this.encoding = encoding;
		}
		public DocumentFormat DocumentFormat { get { return documentFormat; } }
		public Encoding Encoding { get { return encoding; } }
		public abstract void ConvertToDocumentModel(DocumentModel documentModel, object value);
	}
	#endregion
	#region DocumentModelToStringConverter
	public class DocumentModelToStringConverter : DocumentModelToEditValueConverter {
		public DocumentModelToStringConverter(DocumentFormat documentFormat, Encoding encoding)
			: base(documentFormat, encoding) {
		}
		public override object ConvertToEditValue(DocumentModel documentModel) {
			using (MemoryStream stream = new MemoryStream()) {
				documentModel.SaveDocument(stream, DocumentFormat, String.Empty, Encoding);
				return Encoding.GetString(stream.GetBuffer(), 0, (int)stream.Length);
			}
		}
	}
	#endregion
	#region StringEditValueToDocumentModelConverter
	public class StringEditValueToDocumentModelConverter : EditValueToDocumentModelConverter {
		public StringEditValueToDocumentModelConverter(DocumentFormat documentFormat, Encoding encoding)
			: base(documentFormat, encoding) {
		}
		public override void ConvertToDocumentModel(DocumentModel documentModel, object value) {
			if (value != null)
				value = value.ToString();
			string contentString = value as String;
			if (String.IsNullOrEmpty(contentString)) {
				documentModel.InternalAPI.SetDocumentPlainTextContent(String.Empty);
				return;
			}
			ConvertToDocumentModelCore(documentModel, contentString, CalculateActualDocumentFormat(contentString));
		}
		protected internal DocumentFormat CalculateActualDocumentFormat(string contentString) {
			if (DocumentFormat != DocumentFormat.Undefined)
				return DocumentFormat;
			if (IsRtfText(contentString))
				return DocumentFormat.Rtf;
			else
				return DocumentFormat.PlainText;
		}
		void ConvertToDocumentModelCore(DocumentModel documentModel, string contentString, DocumentFormat documentFormat) {
			documentModel.BeforeImport += OnBeforeImport;
			try {
				using (Stream stream = CreateContentStream(contentString, Encoding)) {
					documentModel.LoadDocument(stream, documentFormat, String.Empty, Encoding);
				}
			}
			catch {
			}
			finally {
				documentModel.BeforeImport -= OnBeforeImport;
			}
		}
		void OnBeforeImport(object sender, BeforeImportEventArgs e) {
			HtmlDocumentImporterOptions options = e.Options as HtmlDocumentImporterOptions;
			if (options != null)
				options.AsyncImageLoading = false;
		}
		bool IsRtfText(string text) {
			return text.IndexOf("{\\rtf") == 0;
		}
		Stream CreateContentStream(string text, Encoding encoding) {
			MemoryStream stream = new MemoryStream();
			byte[] bytes = encoding.GetBytes(text);
			stream.Write(bytes, 0, bytes.Length);
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}
	}
	#endregion
	#region DocumentModelToByteArrayConverter
	public class DocumentModelToByteArrayConverter : DocumentModelToEditValueConverter {
		public DocumentModelToByteArrayConverter(DocumentFormat documentFormat, Encoding encoding)
			: base(documentFormat, encoding) {
		}
		public override object ConvertToEditValue(DocumentModel documentModel) {
			using (MemoryStream stream = new MemoryStream()) {
				documentModel.SaveDocument(stream, DocumentFormat, String.Empty, Encoding);
				byte[] result = new byte[(int)stream.Length];
				Array.Copy(stream.GetBuffer(), result, result.Length);
				return result;
			}
		}
	}
	#endregion
	#region ByteArrayEditValueToDocumentModelConverter
	public class ByteArrayEditValueToDocumentModelConverter : EditValueToDocumentModelConverter {
		public ByteArrayEditValueToDocumentModelConverter(DocumentFormat documentFormat, Encoding encoding)
			: base(documentFormat, encoding) {
		}
		public override void ConvertToDocumentModel(DocumentModel documentModel, object value) {
			byte[] contentBytes = value as byte[];
			if (contentBytes == null || contentBytes.Length <= 0) {
				documentModel.InternalAPI.SetDocumentPlainTextContent(String.Empty);
				return;
			}
			using (MemoryStream stream = new MemoryStream(contentBytes)) {
				documentModel.LoadDocument(stream, CalculateActualDocumentFormat(contentBytes), String.Empty, Encoding);
			}
		}
		protected internal DocumentFormat CalculateActualDocumentFormat(byte[] contentBytes) {
			if (DocumentFormat == DocumentFormat.Undefined) {
				if (IsRtfBytes(contentBytes))
					return DocumentFormat.Rtf;
				else
					return DocumentFormat.PlainText;
			}
			else
				return DocumentFormat;
		}
		protected internal bool IsRtfBytes(byte[] bytes) {
			if (bytes.Length < 5)
				return false;
			return
				bytes[0] == (byte)'{' &&
				bytes[1] == (byte)'\\' &&
				bytes[2] == (byte)'r' &&
				bytes[3] == (byte)'t' &&
				bytes[4] == (byte)'f';
		}
	}
	#endregion
}
