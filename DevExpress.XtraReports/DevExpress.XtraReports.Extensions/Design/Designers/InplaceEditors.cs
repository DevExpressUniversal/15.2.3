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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Utils.Internal;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design {
	public abstract class InplaceTextEditorBase {
		#region static
		static bool TryGetDragData(IDataObject dataObject, out DataInfo[] data) {
			if(dataObject.GetDataPresent(typeof(DataInfo[]))) {
				data =  dataObject.GetData(typeof(DataInfo[])) as DataInfo[];
				return true;
			}
			data = null;
			return false;
		}
		protected static Color GetValidColor(Color color) {
			return color.A == 255 ? color : DevExpress.Utils.DXColor.Blend(color, Color.White);
		}
		#endregion
		IDesignerHost designerHost;
		string initialText;
		protected ZoomService zoomService;
		protected XRFieldEmbeddableControl xrControl;
		Control textControl;
		protected abstract string CurrentText { get; }
		protected IComponentChangeService ComponentChangeService {
			get { return (IComponentChangeService)designerHost.GetService(typeof(IComponentChangeService)); }
		}
		protected IDesignerHost DesignerHost {
			get { return designerHost; }
		}
		public System.Windows.Forms.Control Control {
			get { return textControl; }
		}
		public string Text {
			get { return textControl.Text; }
		}
		protected InplaceTextEditorBase(IDesignerHost designerHost, XRFieldEmbeddableControl control, string text, bool selectAll) {
			this.designerHost = designerHost;
			this.xrControl = control;
			zoomService = ZoomService.GetInstance(designerHost);
			textControl = CreateTextEditor();
			UpdateProperties(null);
			UpdateText();
			initialText = CurrentText;
			if(text.Length > 0) {
				textControl.Text = text;
				SelectionStart = text.Length;
			}
			InitTextBox(selectAll);
		}
		public virtual bool CanUndo { get { return false; } }
		public virtual bool CanRedo { get { return false; } }
		public virtual void Undo() { }
		public virtual void Redo() { }
		public abstract void Cut();
		public abstract string SelectedText { get; set; }
		public abstract int SelectionStart { get; set; }
		public abstract int SelectionLength { get; set; }
		abstract protected int GetCharIndexFromPoint(Point point);
		protected virtual void InitTextBox(bool selectAll) {
			SetTextBoxBounds();
			textControl.AllowDrop = true;
			textControl.DragEnter += new DragEventHandler(OnDragEnter);
			textControl.DragOver += new DragEventHandler(OnDragOver);
			textControl.DragDrop += new DragEventHandler(OnDragDrop);
			textControl.TextChanged += OnTextChanged;
		}
		void SetTextBoxBounds() {
			XRTextControlBaseDesigner designer = designerHost.GetDesigner(xrControl) as XRTextControlBaseDesigner;
			Rectangle rect = Rectangle.Round(designer.GetEditorScreenBounds());
			rect.Size = zoomService.AdjustInplaceEditorSize(rect.Size);
			rect = RectHelper.InflateRect(rect, 0, 0, 1, 1);
			IBandViewInfoService svc = (IBandViewInfoService)designerHost.GetService(typeof(IBandViewInfoService));
			Control editorParent = svc.View.Parent;
			textControl.Bounds = editorParent.RectangleToClient(rect);
			textControl.Parent = editorParent;
			textControl.BringToFront();
		}
		void OnDragEnter(object sender, DragEventArgs e) {
			DataInfo[] data;
			if(TryGetDragData(e.Data, out data) && data.Length == 1)
				e.Effect = DragDropEffects.Copy;
			if(!textControl.Focused)
				textControl.Focus();
		}
		void OnDragOver(object sender, DragEventArgs e) {
			DataInfo[] data;
			if(TryGetDragData(e.Data, out data) && data.Length == 1) {
				e.Effect = DragDropEffects.Copy;
				SetSelection(textControl.Text, GetCharIndexFromPoint(new Point(e.X, e.Y)));
			}
		}
		void SetSelection(string text, int charIndex) {
			int selectionStart;
			int selectionLength;
			EmbeddedFieldsHelper.GetSelectionProperties(text, charIndex, out selectionStart, out selectionLength);
			SetSelection(selectionStart, selectionLength);
		}
		public virtual void SetSelection(int start, int length) {
			SelectionStart = start;
			SelectionLength = length;
		}
		protected virtual void OnDragDrop(object sender, DragEventArgs e) {
			DataInfo[] data;
			if(TryGetDragData(e.Data, out data) && data.Length == 1) {
				object dataSource = data[0].Source;
				string dataMember = xrControl.Report.GetFieldDisplayName(data[0].Member);
				if(dataSource is DevExpress.XtraReports.Native.Parameters.ParametersDataSource)
					dataMember = DevExpress.XtraReports.Native.Parameters.ParametersReplacer.GetParameterFullName(dataMember);
				string s = MailMergeFieldInfo.WrapColumnInfoInBrackets(dataMember, String.Empty).ToString();
				SetSelection(textControl.Text, GetCharIndexFromPoint(new Point(e.X, e.Y)));
				SelectedText = s;
			}
		}
		bool closed;
		public virtual void Close(bool commit) {
			if(closed)
				return;
			closed = true;
			textControl.DragEnter -= new DragEventHandler(OnDragEnter);
			textControl.DragOver -= new DragEventHandler(OnDragOver);
			textControl.DragDrop -= new DragEventHandler(OnDragDrop);
			textControl.TextChanged -= OnTextChanged;
			if(commit && initialText != CurrentText)
				CommitChanges();
			textControl.Dispose();
			textControl = null;
		}
		public virtual void UpdateProperties(string propertyName) {
			textControl.BackColor = GetValidColor(xrControl.GetEffectiveBackColor());
			textControl.ForeColor = GetValidColor(xrControl.GetEffectiveForeColor());
		}
		protected virtual void UpdateText() {
			textControl.Text = xrControl.GetDisplayPropertyWithDisplayColumnNames();
		}
		protected abstract Control CreateTextEditor();
		protected abstract void CommitChanges();
#if DEBUGTEST
		public string GetCurrentText() { return CurrentText; }
		public string GetInitialText() { return initialText; }
#endif
		void OnTextChanged(object sender, EventArgs e) {
			MenuCommandHandler menuCommandHandler = (MenuCommandHandler)DesignerHost.GetService(typeof(MenuCommandHandler));
			menuCommandHandler.UpdateCommandStatus();
		}
		protected void SetPropertyRealValue(string propertyName, string value){
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(xrControl)[propertyName];
			if(propertyDescriptor != null)
				propertyDescriptor.SetValue(xrControl, ((IDisplayNamePropertyContainer)xrControl).GetRealPropertyValue(value));
		}
	}
	public class InplaceTextEditor : InplaceTextEditorBase {
		protected TextBox TextBox { get { return (TextBox)Control; } }
		XRControl Owner { get { return xrControl; } }
		protected override string CurrentText { get { return TextBox.Text; } }
		public override bool CanUndo { get { return TextBox.CanUndo; } }
		public override void Undo() { TextBox.Undo(); }
		public override void Cut() { TextBox.Cut(); }
		public override string SelectedText {
			get { return TextBox.SelectedText; }
			set { TextBox.SelectedText = value; }
		}
		public override int SelectionStart {
			get { return TextBox.SelectionStart; }
			set { TextBox.SelectionStart = value; }
		}
		public override int SelectionLength {
			get { return TextBox.SelectionLength; }
			set { TextBox.SelectionLength = value; }
		}
		protected override void InitTextBox(bool selectAll) {
			TextBox.Multiline = true;
			TextBox.BorderStyle = zoomService.GetInplaceEditorBorderStyle();
			TextBox.HideSelection = false;
			TextBox.AcceptsTab = true;
			if(selectAll) TextBox.SelectAll();
			base.InitTextBox(selectAll);
		}
		public InplaceTextEditor(IDesignerHost designerHost, XRFieldEmbeddableControl control, string text, bool selectAll)
			: base(designerHost, control, text, selectAll) {
		}
		protected override void CommitChanges() {
			var label = Owner as XRLabel;
			if(label != null && !label.Multiline && TextBox.Lines.Length > 1)
				label.Multiline = true;
			DesignerTransaction trans = DesignerHost.CreateTransaction(String.Format("Change {0}.Text", xrControl.Name));
			try {
				SetPropertyRealValue("Text", Control.Text);
			} catch {
				trans.Cancel();
			} finally {
				trans.Commit();
			}
		}
		protected override int GetCharIndexFromPoint(Point point) {
			Point p = TextBox.PointToClient(point);
			IntPtr result = DevExpress.XtraPrinting.Native.Win32.SendMessage(new System.Runtime.InteropServices.HandleRef(TextBox, TextBox.Handle), DevExpress.XtraPrinting.Native.Win32.EM_CHARFROMPOS, 0, DevExpress.XtraPrinting.Native.Win32.MakeLParam(p.X, p.Y));
			return DevExpress.XtraPrinting.Native.Win32.LoWord(result);
		}
		public override void UpdateProperties(string propertyName) {
			base.UpdateProperties(propertyName);
			Control.Font = zoomService.AdjustInplaceEditorFont(xrControl.GetEffectiveFont());
			switch(xrControl.TextAlignment) {
				case TextAlignment.BottomCenter:
				case TextAlignment.MiddleCenter:
				case TextAlignment.TopCenter:
					TextBox.TextAlign = HorizontalAlignment.Center;
					return;
				case TextAlignment.BottomLeft:
				case TextAlignment.MiddleLeft:
				case TextAlignment.TopLeft:
					TextBox.TextAlign = HorizontalAlignment.Left;
					return;
				case TextAlignment.BottomRight:
				case TextAlignment.MiddleRight:
				case TextAlignment.TopRight:
					TextBox.TextAlign = HorizontalAlignment.Right;
					return;
			}
		}
		protected override Control CreateTextEditor() {
			return new TextBox();
		}
	}
	public class InplaceSingleTextEditor : InplaceTextEditor {
		public InplaceSingleTextEditor(IDesignerHost designerHost, XRFieldEmbeddableControl control, string text, bool selectAll)
			: base(designerHost, control, text, selectAll) {
		}
		string GetInputString() {
			return string.Concat(TextBox.Lines);
		}
		protected override void CommitChanges() {
			try {
				SetPropertyRealValue("Text", GetInputString());
			} catch { }
		}
	}
	public abstract class InplaceRichTextEditorBase : InplaceTextEditorBase {
		protected abstract event EventHandler SelectionChanged;
		protected XRRichTextBase XRRichText {
			get { return (XRRichTextBase)xrControl; }
		}
		protected override string CurrentText { 
			get { return Rtf; }
		}
		public abstract string Rtf { get; set; }
		public abstract Color SelectionBackColor { get; set; }
		public abstract Color SelectionColor { get; set; }
		public abstract FontSurrogate SelectionFont { get; set; }
		public abstract HorizontalAlignmentEx SelectionAlignment { get; set; }
		protected abstract float ZoomFactor { get; set; }
		protected abstract Font Font { get; set; }
		public InplaceRichTextEditorBase(IDesignerHost designerHost, XRFieldEmbeddableControl control, string text, bool selectAll)
			: base(designerHost, control, text, selectAll) {
		}
		public abstract void Clear();
		protected override void InitTextBox(bool selectAll) {
			base.InitTextBox(selectAll);
			SelectionChanged += OnSelectionChanged;
		}
		public override void Close(bool commit) {
			SelectionChanged -= OnSelectionChanged;
			base.Close(commit);
		}
		protected void UpdateFontService() {
			FontServiceBase fontServiceBase = DesignerHost.GetService(typeof(FontServiceBase)) as FontServiceBase;
			if(fontServiceBase != null)
				fontServiceBase.UpdateFontControls(SelectionFont);
		}
		protected override Control CreateTextEditor() {
			return new RichTextBoxEx();
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			UpdateFontService();
			UpdateCommandStatus();
		}
		protected void UpdateCommandStatus() {
			MenuCommandHandler menuCommandHandler = (MenuCommandHandler)DesignerHost.GetService(typeof(MenuCommandHandler));
			if(menuCommandHandler != null) menuCommandHandler.UpdateCommandStatus();
		}
		protected override void UpdateText() {
			Font = XRRichText.Font;
			Rtf = XRRichText.GetDisplayPropertyWithDisplayColumnNames();
		}
		protected override void CommitChanges() {
			DesignerTransaction trans = DesignerHost.CreateTransaction(String.Format("Change {0}.Rtf", xrControl.Name));
			try {
				XRControlDesignerBase.RaiseComponentChanging(ComponentChangeService, xrControl, "RtfText");
				SerializableString serializableString = new SerializableString();
				serializableString.Value = Rtf;
				object oldValue = XRRichText.RtfText;
				XRRichText.RtfText = serializableString;
				XRControlDesignerBase.RaiseComponentChanged(ComponentChangeService, xrControl, "RtfText", oldValue, serializableString);
			} catch {
				trans.Cancel();
			} finally {
				trans.Commit();
			}
		}
		public override void UpdateProperties(string propertyName) {
			base.UpdateProperties(propertyName);
			ZoomFactor = zoomService.GetInplaceEditorZoomFactor();
			if(propertyName == "Text" || propertyName == "Lines")
				Rtf = XRRichText.Rtf;
		}
	}
	public class InplaceRichTextEditor : InplaceRichTextEditorBase {
		protected override event EventHandler SelectionChanged {
			add { RichTextBox.SelectionChanged += value; }
			remove { RichTextBox.SelectionChanged -= value; }
		}
		RichTextBoxEx RichTextBox {
			get { return (RichTextBoxEx)Control; }
		}
		public override string Rtf {
			get { return RichTextBox.Rtf; }
			set { RichTextBox.Rtf = value; }
		}
		public override Color SelectionBackColor {
			get { return RichTextBox.SelectionBackColor; }
			set { RichTextBox.SelectionBackColor = value; }
		}
		public override Color SelectionColor {
			get { return RichTextBox.SelectionColor; }
			set { RichTextBox.SelectionColor = value; }
		}
		public override FontSurrogate SelectionFont {
			get { return FontSurrogate.FromFont(RichTextBox.SelectionFont); }
			set { RichTextBox.SelectionFont = FontSurrogate.ToFont(value); }
		}
		public override HorizontalAlignmentEx SelectionAlignment {
			get { return RichTextBox.SelectionAlignmentEx; }
			set { RichTextBox.SelectionAlignmentEx = value; }
		}		
		protected override float ZoomFactor {
			get { return RichTextBox.ZoomFactor; }
			set { RichTextBox.ZoomFactor = value; }
		}
		protected override Font Font {
			get { return RichTextBox.Font; }
			set { RichTextBox.Font = value; }
		}
		public override bool CanUndo {
			get { return RichTextBox.CanUndo; }
		}
		public override bool CanRedo {
			get { return RichTextBox.CanRedo; }
		}
		public override string SelectedText {
			get { return RichTextBox.SelectedText; }
			set { RichTextBox.SelectedText = value; }
		}
		public override int SelectionStart {
			get { return RichTextBox.SelectionStart; }
			set { RichTextBox.SelectionStart = value; }
		}
		public override int SelectionLength {
			get { return RichTextBox.SelectionLength; }
			set { RichTextBox.SelectionLength = value; }
		}
		public InplaceRichTextEditor(IDesignerHost designerHost, XRFieldEmbeddableControl control, string text, bool selectAll)
			: base(designerHost, control, text, selectAll) {
		}
		public override void Undo() {
			RichTextBox.Undo();
		}
		public override void Redo() {
			RichTextBox.Redo();
		}
		public override void Cut() {
			RichTextBox.Cut();
		}
		public override void Clear() {
			RichTextBox.Clear();
		}
		protected override void InitTextBox(bool selectAll) {
			RichTextBox.Multiline = true;
			RichTextBox.BorderStyle = zoomService.GetInplaceEditorBorderStyle();
			RichTextBox.HideSelection = false;
			RichTextBox.AcceptsTab = true;
			if(selectAll) RichTextBox.SelectAll();
			base.InitTextBox(selectAll);
			UpdateFontService();
		}
		protected override int GetCharIndexFromPoint(Point point) {
			Point pt = RichTextBox.PointToClient(point);
			int index = RichTextBox.GetCharIndexFromPosition(pt);
			if(index == RichTextBox.TextLength - 1) {
				Point p1 = RichTextBox.GetPositionFromCharIndex(index);
				Point p2 = RichTextBox.GetPositionFromCharIndex(index + 1);
				if(Math.Abs(pt.X - p2.X) < Math.Abs(pt.X - p1.X))
					index++;
			}
			return index;
		}
		protected override void UpdateText() {
			base.UpdateText();
			if(XRRichText is XRRichTextBoxBase)
				RichTextBox.DetectUrls = ((XRRichTextBoxBase)xrControl).DetectUrls;
		}
	}
}
namespace DevExpress.XtraReports.Design {
	using DevExpress.XtraRichEdit;
	using DevExpress.XtraRichEdit.API.Native;
	using DevExpress.XtraRichEdit.SyntaxEdit;
	using DevExpress.Utils.Win;
	using DevExpress.Office.Internal;
	using DevExpress.XtraEditors;
	public class InplaceDXRichTextEditor : InplaceRichTextEditorBase {
		protected override event EventHandler SelectionChanged {
			add { RichEdit.InnerSelectionChanged += value; }
			remove { RichEdit.InnerSelectionChanged -= value; }
		}
		DevExpress.XtraRichEdit.SyntaxEdit.SimpleEditControl RichEdit {
			get { return (DevExpress.XtraRichEdit.SyntaxEdit.SimpleEditControl)Control; }
		}
		public override string Rtf {
			get { return RichEdit.RtfText; }
			set { RichEdit.RtfText = value; }
		}
		DocumentRange Selection {
			get {
				return RichEdit.Document.Selection;
			}
			set {
				RichEdit.Document.Selection = value;
			}
		}
		DocumentRange EffectiveSelection {
			get {
				if(Selection.Length > 0)
					return Selection;
				int start = RichEdit.Document.CaretPosition.ToInt() - 1;
				return RichEdit.Document.CreateRange(Math.Max(0, start), 1);
			}
		}
		public override Color SelectionBackColor {
			get {
				return CharacterPropertiesGetter<Color>(EffectiveSelection, (props, defaultProps) => {
					return props.BackColor.HasValue ? props.BackColor.Value : defaultProps.BackColor.Value; 
				});
			}
			set {
				CharacterPropertiesSetter(EffectiveSelection, props => props.BackColor = value);
			}
		}
		public override Color SelectionColor {
			get {
				return CharacterPropertiesGetter<Color>(EffectiveSelection, (props, defaultProps) => {
					return props.ForeColor.HasValue ? props.ForeColor.Value : defaultProps.ForeColor.Value;
				});
			}
			set {
				CharacterPropertiesSetter(EffectiveSelection, props => props.ForeColor = value);
			}
		}
		public override FontSurrogate SelectionFont {
			get {
				return CharacterPropertiesGetter<FontSurrogate>(EffectiveSelection, (props, defaultProps) => {
					return CreateFont(props, defaultProps);
				});
			}
			set {
				CharacterPropertiesSetter(EffectiveSelection, props => {
					props.FontName = value.Name;
					props.FontSize = value.Size;
					props.Bold = value.Bold;
					props.Italic = value.Italic;
					props.Strikeout = value.Strikeout ? StrikeoutType.Single : StrikeoutType.None;
					props.Underline = value.Underline ? UnderlineType.Single : UnderlineType.None;
				});
			}
		}
		public override HorizontalAlignmentEx SelectionAlignment {
			get {
				return ParagraphPropertiesGetter<HorizontalAlignmentEx>(EffectiveSelection, (props, defaultProps) => {
					return props.Alignment.HasValue ? ParagraphToHorizontalAlignment(props.Alignment.Value) : 
						ParagraphToHorizontalAlignment(defaultProps.Alignment.Value);
				});
			}
			set {
				ParagraphPropertiesSetter(EffectiveSelection, props => {
					props.Alignment = ParagraphFromHorizontalAlignment(value);
				});
			}
		}
		protected override float ZoomFactor {
			get { return RichEdit.ActiveView.ZoomFactor; }
			set { RichEdit.ActiveView.ZoomFactor = value; } 
		}
		protected override Font Font { get; set; }
		public override bool CanUndo {
			get { return RichEdit.CanUndo; }
		}
		public override bool CanRedo {
			get { return RichEdit.CanRedo; }
		}
		public override string SelectedText {
			get { return RichEdit.Document.GetText(Selection); }
			set { RichEdit.Document.Replace(Selection, value); }
		}
		public override int SelectionStart {
			get { return Selection.Start.ToInt(); }
			set { SetSelection(value, 0); }
		}
		public override int SelectionLength {
			get { return Selection.Length; }
			set { SetSelection(Selection.Start.ToInt(), value); }
		}
		public override void SetSelection(int start, int length) {
			Selection = RichEdit.Document.CreateRange(start, length);
		}
		public InplaceDXRichTextEditor(IDesignerHost designerHost, XRRichText control, string text, bool selectAll)
			: base(designerHost, control, text, selectAll) { 
		}
		protected override void InitTextBox(bool selectAll) {
			if(selectAll && !string.IsNullOrEmpty(Rtf))
				RichEdit.SelectAllReversed();
			base.InitTextBox(selectAll);
			RichEdit.LayoutUnit = DocumentLayoutUnit.Twip;
			RichEdit.ActiveView.BackColor = GetValidColor(xrControl.GetEffectiveBackColor());
			UpdateFontService();
			RichEdit.ContentChanged += OnContentChanged;
		}
		public override void Close(bool commit) {
			RichEdit.ContentChanged -= OnContentChanged;
			base.Close(commit);
		}
		void OnContentChanged(object sender, EventArgs e) {
			UpdateFontService();
			UpdateCommandStatus();
		}
		T ParagraphPropertiesGetter<T>(DocumentRange range, Func<ParagraphPropertiesBase, ParagraphPropertiesBase, T> func) {
			ParagraphProperties props = RichEdit.Document.BeginUpdateParagraphs(range);
			try {
				return func(props, RichEdit.Document.DefaultParagraphProperties);
			} finally {
				RichEdit.Document.EndUpdateParagraphs(props);
			}
		}
		void ParagraphPropertiesSetter(DocumentRange range, Action<ParagraphPropertiesBase> action) {
			ParagraphProperties props = RichEdit.Document.BeginUpdateParagraphs(range);
			try {
				action(props);
			} finally {
				RichEdit.Document.EndUpdateParagraphs(props);
			}
		}
		T CharacterPropertiesGetter<T>(DocumentRange range, Func<CharacterPropertiesBase, CharacterPropertiesBase, T> func) {
			CharacterProperties props = RichEdit.Document.BeginUpdateCharacters(range);
			try {
				return func(props, RichEdit.Document.DefaultCharacterProperties);
			} finally {
				RichEdit.Document.EndUpdateCharacters(props);
			}
		}
		void CharacterPropertiesSetter(DocumentRange range, Action<CharacterPropertiesBase> action) {
			CharacterProperties props = RichEdit.Document.BeginUpdateCharacters(range);
			try {
				action(props);
			} finally {
				RichEdit.Document.EndUpdateCharacters(props);
			}
		}
		static FontSurrogate CreateFont(CharacterPropertiesBase props, CharacterPropertiesBase defaultProps) {
			return new FontSurrogate() {
				Name = !string.IsNullOrEmpty(props.FontName) ? props.FontName : defaultProps.FontName,
				Size = props.FontSize.HasValue ? props.FontSize.Value : defaultProps.FontSize.Value,
				Style = CreateFontStyle(props, defaultProps)
			};
		}
		static FontStyle CreateFontStyle(CharacterPropertiesBase props, CharacterPropertiesBase defaultProps) {
			FontStyle fontStyle = FontStyle.Regular;
			fontStyle = AdjustStyle(fontStyle, FontStyle.Bold, props.Bold.HasValue ? props.Bold.Value : defaultProps.Bold.Value);
			fontStyle = AdjustStyle(fontStyle, FontStyle.Italic, props.Italic.HasValue ? props.Italic.Value : defaultProps.Italic.Value);
			fontStyle = AdjustStyle(fontStyle, FontStyle.Strikeout, props.Strikeout.HasValue ? props.Strikeout.Value.Equals(StrikeoutType.Single) : 
				defaultProps.Strikeout.Value.Equals(StrikeoutType.Single));
			fontStyle = AdjustStyle(fontStyle, FontStyle.Underline, props.Underline.HasValue ? props.Underline.Value.Equals(UnderlineType.Single) : 
				defaultProps.Underline.Value.Equals(UnderlineType.Single));
			return fontStyle;
		}
		static FontStyle AdjustStyle(FontStyle style, FontStyle flag, bool flagExisted) {
			return flagExisted ? style | flag : style & ~flag;
		} 
		static HorizontalAlignmentEx ParagraphToHorizontalAlignment(ParagraphAlignment aligment) {
			switch(aligment) {
				case ParagraphAlignment.Center:
					return HorizontalAlignmentEx.Center;
				case ParagraphAlignment.Justify:
					return HorizontalAlignmentEx.Justify;
				case ParagraphAlignment.Right:
					return HorizontalAlignmentEx.Right;
				default:
					return HorizontalAlignmentEx.Left;
			}
		}
		static ParagraphAlignment ParagraphFromHorizontalAlignment(HorizontalAlignmentEx aligment) {
			switch(aligment) {
				case HorizontalAlignmentEx.Center:
					return ParagraphAlignment.Center;
				case HorizontalAlignmentEx.Justify:
					return ParagraphAlignment.Justify;
				case HorizontalAlignmentEx.Right:
					return ParagraphAlignment.Right;
				default:
					return ParagraphAlignment.Left;
			}
		}
		protected override Control CreateTextEditor() {
			return new InplaceEditControl();
		}
		public override void Undo() {
			RichEdit.Undo();
		}
		public override void Redo() {
			RichEdit.Redo();
		}
		public override void Cut() {
			RichEdit.Cut();
		}
		public override void Clear() {
			RichEdit.CreateNewDocument();
		}
		protected override int GetCharIndexFromPoint(Point point) {
			return RichEdit.GetCharIndexFromPoint(point);
		}
		protected override void CommitChanges() {
			DesignerTransaction trans = DesignerHost.CreateTransaction(String.Format("Change {0}.Rtf", XRRichText.Name));
			try {
				PropertyDescriptor rtfStringPropertyDescriptor = TypeDescriptor.GetProperties(XRRichText)["Rtf"];
				if(rtfStringPropertyDescriptor != null)
					rtfStringPropertyDescriptor.SetValue(XRRichText, Rtf);
			} catch {
				trans.Cancel();
			} finally {
				trans.Commit();
			}
		}
	}
	class InplaceEditControl : SimpleEditControl, IFormSubstitute {
		public InplaceEditControl()
			: base(true) {
			Initialize();
			Options.CopyPaste.InsertOptions = InsertOptions.KeepSourceFormatting;
		}
		protected override IOfficeScrollbar CreateScrollBarCore(ScrollBarBase scrollBar) {
			scrollBar.Cursor = System.Windows.Forms.Cursors.Default;
			ScrollBarBase.ApplyUIMode(scrollBar, ScrollUIMode.Touch);
			this.Controls.Add(scrollBar);
			scrollBar.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			return new WinFormsOfficeScrollbar(scrollBar);
		}
		bool IFormSubstitute.FormIsCreated {
			get { return true; }
		}
		bool IFormSubstitute.FormIsHandleCreated {
			get { return true; }
		}
	}
}
