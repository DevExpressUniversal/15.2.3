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
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraSpreadsheet.Model;
using System.ComponentModel;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Native;
using System.Collections.Generic;
using DevExpress.Export.Xl;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraSpreadsheet.Forms {
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1301")]
	#region FormatCellsForm
	public partial class FormatCellsForm : XtraForm {
		#region Fields
		readonly FormatCellsViewModel viewModel;
		#endregion
		FormatCellsForm() {
			InitializeComponent();
		}
		public FormatCellsForm(FormatCellsViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			SetBindingsNumber();
			SetBindingsAlignment();
			SetBindingsFont();
			SetBindingsBorder();
			SetBindingFill();
			SetBindingsProtection();
			InitializeFontPreviewStyle();
			SetActiveTabPage(viewModel.InitialTabPage);
			this.formatBorderControl.InitializeBorderPainters(viewModel.Control);
			this.lbBorderLineStyle.InitializeViewModel(viewModel);
			this.viewModel.PropertyChanged += OnPropertyChanged;
			UpdateControlVisibility();
			UpdateNumberTypeSource();
			this.drawPatternType.Paint += OnPatternTypePaint;
			this.edtBackColor.EditValueChanging += OnBackColorChanging;
		}
		#region Properties
		public FormatCellsViewModel ViewModel { get { return viewModel; } }
		#endregion
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "NumberCategory") {
				UpdateControlVisibility();
				UpdateNumberTypeSource();
			}
		}
		void UpdateControlVisibility() {
			this.lblNumberDescription.Visible = ViewModel.NumberDescriptionVisible;
			this.formatNumberDecimalControl.Visible = ViewModel.NumberDecimalControlVisible;
			this.formatNumberTypeControl.Visible = ViewModel.NumberTypeControlVisible;
			this.formatNumberDecimalControl.UseThousandSeparatorVisible = ViewModel.UseThousandSeparatorVisible;
			this.formatNumberDecimalControl.SymbolListVisible = ViewModel.SymbolListVisible;
			this.formatNumberDecimalControl.NegativeNumbersVisible = ViewModel.NegativeNumbersVisible;
			this.formatNumberCustomControl.Visible = ViewModel.NumberCustomControlVisible;
			this.xtraTabProtection.PageVisible = !ViewModel.IsProtected;
		}
		void UpdateNumberTypeSource() {
			this.formatNumberTypeControl.SetTypeSource(ViewModel.TypeCaptions);
		}
		void InitializeFontPreviewStyle() {
			DevExpress.XtraRichEdit.API.Native.Document doc = edtFontPreview.Document;
			ParagraphStyle defaultParagraphStyle = doc.ParagraphStyles["Normal"];
			(defaultParagraphStyle as ParagraphPropertiesBase).Alignment = ParagraphAlignment.Center;
		}
		protected internal void SetBindingsNumber() {
			this.lBoxNumberCategory.DataBindings.Add("DataSource", viewModel, "NumberCategories");
			this.lBoxNumberCategory.DataBindings.Add("SelectedValue", viewModel, "NumberCategory", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblNumberSample.DataBindings.Add("Text", viewModel, "NumberSampleText", false, DataSourceUpdateMode.OnPropertyChanged);
			this.formatNumberDecimalControl.DataBindings.Add("DecimalPlaces", viewModel, "DecimalPlaces", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatNumberTypeControl.DataBindings.Add("SelectedTypeIndex", viewModel, "SelectedTypeIndex", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatNumberCustomControl.DataBindings.Add("CustomFormatCode", viewModel, "CustomFormatCode", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblNumberDescription.DataBindings.Add("Text", viewModel, "NumberDescription", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lblCategoryDescription.DataBindings.Add("Text", viewModel, "CategoryDescription", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		protected internal void SetBindingsAlignment() {
			this.edtHorizontalAlignment.Properties.DataSource = viewModel.HorizontalAlignments;
			this.edtHorizontalAlignment.DataBindings.Add("EditValue", viewModel, "HorizontalAlignment", true, DataSourceUpdateMode.OnPropertyChanged, String.Empty);
			this.edtVerticalAlignment.Properties.DataSource = viewModel.VerticalAlignments;
			this.edtVerticalAlignment.DataBindings.Add("EditValue", viewModel, "VerticalAlignment", true, DataSourceUpdateMode.OnPropertyChanged, String.Empty);
			this.edtTextDirection.Properties.DataSource = viewModel.TextDirections;
			this.edtTextDirection.DataBindings.Add("EditValue", viewModel, "TextDirection", true, DataSourceUpdateMode.OnPropertyChanged, String.Empty);
			this.edtIndent.DataBindings.Add("EditValue", viewModel, "Indent", true, DataSourceUpdateMode.OnPropertyChanged);
			this.grpOrientation.DataBindings.Add("Enabled", viewModel, "OrientationEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkWrapText.Properties.AllowGrayed = viewModel.WrapTextIndeterminate;
			this.chkWrapText.DataBindings.Add("CheckState", viewModel, "WrapText", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShrinkToFit.Properties.AllowGrayed = viewModel.ShrinkToFitIndeterminate;
			this.chkShrinkToFit.DataBindings.Add("CheckState", viewModel, "ShrinkToFit", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShrinkToFit.DataBindings.Add("Enabled", viewModel, "ShrinkToFitEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkMergeCells.Properties.AllowGrayed = viewModel.MergeCellsIndeterminate;
			this.chkMergeCells.DataBindings.Add("CheckState", viewModel, "MergeCells", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkJustifyDistributed.Properties.AllowGrayed = viewModel.JustifyDistributedIndeterminate;
			this.chkJustifyDistributed.DataBindings.Add("CheckState", viewModel, "JustifyDistributed", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkJustifyDistributed.DataBindings.Add("Enabled", viewModel, "JustifyDistributedEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		protected internal void SetBindingsFont() {
			this.lBoxFont.DataBindings.Add("DataSource", viewModel, "FontNames");
			this.lBoxFont.DataBindings.Add("SelectedItem", viewModel, "FontName", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnFontNameBindingComplete;
			this.edtFont.DataBindings.Add("Text", viewModel, "FontName", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lBoxFont.DataBindings.Add("SelectedValue", edtFont, "EditValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lBoxFontStyle.DataBindings.Add("DataSource", viewModel, "FontStyles");
			this.lBoxFontStyle.DataBindings.Add("SelectedItem", viewModel, "FontStyle", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnFontStyleBindingComplete;
			this.edtFontStyle.DataBindings.Add("Text", viewModel, "FontStyle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lBoxFontStyle.DataBindings.Add("SelectedValue", edtFontStyle, "EditValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lBoxSize.DataBindings.Add("DataSource", viewModel, "FontSizes");
			this.lBoxSize.DataBindings.Add("SelectedItem", viewModel, "Size", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnFontSizeBindingComplete;
			this.edtSize.DataBindings.Add("Text", viewModel, "Size", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lBoxSize.DataBindings.Add("SelectedValue", edtSize, "EditValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtUnderline.Properties.DataSource = viewModel.Underlines;
			this.edtUnderline.DataBindings.Add("EditValue", viewModel, "Underline", true, DataSourceUpdateMode.OnPropertyChanged, String.Empty).BindingComplete += OnUnderlineBindingComplete;
			this.edtColorFont.DataBindings.Add("Color", viewModel, "ColorFont", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnFontColorBindingComplete;
			this.chkStrikeThrough.Properties.AllowGrayed = viewModel.StrikeThroughIndeterminate;
			this.chkStrikeThrough.DataBindings.Add("CheckState", viewModel, "StrikeThrough", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnStrikeThroughBindingComplete;
			this.chkSuperscript.Properties.AllowGrayed = viewModel.SuperscriptIndeterminate;
			this.chkSuperscript.DataBindings.Add("CheckState", viewModel, "Superscript", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnSuperscriptBindingComplete;
			this.chkSubscript.DataBindings.Add("Checked", viewModel, "Subscript", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnSubscriptBindingComplete;
		}
		#region Font's Event
		void OnFontNameBindingComplete(object sender, BindingCompleteEventArgs e) {
			DevExpress.XtraRichEdit.API.Native.Document doc = edtFontPreview.Document;
			CharacterProperties properties = doc.BeginUpdateCharacters(doc.Range);
			properties.FontName = viewModel.FontName;
			doc.EndUpdateCharacters(properties);
		}
		void OnFontStyleBindingComplete(object sender, BindingCompleteEventArgs e) {
			DevExpress.XtraRichEdit.API.Native.Document doc = edtFontPreview.Document;
			CharacterProperties properties = doc.BeginUpdateCharacters(doc.Range);
			if (viewModel.FontStyle == "Regular") {
				properties.Bold = false;
				properties.Italic = false;
			}
			else if (viewModel.FontStyle == "Italic") {
				properties.Italic = true;
				properties.Bold = false;
			}
			else if (viewModel.FontStyle == "Bold") {
				properties.Bold = true;
				properties.Italic = false;
			}
			else {
				properties.Italic = true;
				properties.Bold = true;
			}
			doc.EndUpdateCharacters(properties);
		}
		void OnFontSizeBindingComplete(object sender, BindingCompleteEventArgs e) {
			DevExpress.XtraRichEdit.API.Native.Document doc = edtFontPreview.Document;
			CharacterProperties properties = doc.BeginUpdateCharacters(doc.Range);
			ParagraphStyle defaultParagraphStyle = doc.ParagraphStyles["Normal"];
			properties.FontSize = Convert.ToInt32(viewModel.Size);
			float fontSize = properties.FontSize.Value;
			ParagraphPropertiesBase paragraphProperties = (defaultParagraphStyle as ParagraphPropertiesBase);
			FontSizeToRichEditParagraphSpacingBeforeCalculator calculator = new FontSizeToRichEditParagraphSpacingBeforeCalculator();
			float calculatedSpacing;
			if (calculator.TryCalculateSpacingBefore(fontSize, out calculatedSpacing)) {
				paragraphProperties.SpacingBefore = calculatedSpacing;
			}
			doc.EndUpdateCharacters(properties);
		}
		void OnFontColorBindingComplete(object sender, BindingCompleteEventArgs e) {
			DevExpress.XtraRichEdit.API.Native.Document doc = edtFontPreview.Document;
			CharacterProperties properties = doc.BeginUpdateCharacters(doc.Range);
			properties.ForeColor = viewModel.ColorFont;
			doc.EndUpdateCharacters(properties);
		}
		void OnUnderlineBindingComplete(object sender, BindingCompleteEventArgs e) {
			DevExpress.XtraRichEdit.API.Native.Document doc = edtFontPreview.Document;
			CharacterProperties properties = doc.BeginUpdateCharacters(doc.Range);
			if (viewModel.IsUnderlineNone())
				properties.Underline = UnderlineType.None;
			else if (viewModel.IsUnderlineSingle())
				properties.Underline = UnderlineType.Single;
			else if (viewModel.IsUnderlineDouble())
				properties.Underline = UnderlineType.Double;
			doc.EndUpdateCharacters(properties);
		}
		void OnStrikeThroughBindingComplete(object sender, BindingCompleteEventArgs e) {
			DevExpress.XtraRichEdit.API.Native.Document doc = edtFontPreview.Document;
			CharacterProperties properties = doc.BeginUpdateCharacters(doc.Range);
			if (viewModel.StrikeThrough == CheckState.Checked)
				properties.Strikeout = StrikeoutType.Single;
			else
				properties.Strikeout = StrikeoutType.None;
			doc.EndUpdateCharacters(properties);
		}
		void OnSuperscriptBindingComplete(object sender, BindingCompleteEventArgs e) {
			DevExpress.XtraRichEdit.API.Native.Document doc = edtFontPreview.Document;
			CharacterProperties properties = doc.BeginUpdateCharacters(doc.Range);
			this.edtFontPreview.BeginUpdate();
			properties.Superscript = viewModel.Superscript == CheckState.Checked;
			this.edtFontPreview.EndUpdate();
			doc.EndUpdateCharacters(properties);
		}
		void OnSubscriptBindingComplete(object sender, BindingCompleteEventArgs e) {
			DevExpress.XtraRichEdit.API.Native.Document doc = edtFontPreview.Document;
			CharacterProperties properties = doc.BeginUpdateCharacters(doc.Range);
			this.edtFontPreview.BeginUpdate();
			properties.Subscript = viewModel.Subscript;
			this.edtFontPreview.EndUpdate();
			doc.EndUpdateCharacters(properties);
		}
		#endregion
		protected internal void SetBindingsBorder() {
			this.lbBorderLineStyle.DataSource = Enum.GetValues(typeof(XlBorderLineStyle));
			this.lbBorderLineStyle.DataBindings.Add("SelectedValue", viewModel, "LineStyle", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtColorBorder.DataBindings.Add("Color", viewModel, "Color", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lbBorderLineStyle.DataBindings.Add("ForeColor", viewModel, "Color", false, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("LeftColor", viewModel, "LeftColor", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("LeftLineStyle", viewModel, "LeftLineStyle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("RightColor", viewModel, "RightColor", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("RightLineStyle", viewModel, "RightLineStyle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("TopColor", viewModel, "TopColor", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("TopLineStyle", viewModel, "TopLineStyle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("BottomColor", viewModel, "BottomColor", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("BottomLineStyle", viewModel, "BottomLineStyle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("DiagonalColor", viewModel, "DiagonalColor", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("DiagonalDownLineStyle", viewModel, "DiagonalDownLineStyle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("DiagonalUpLineStyle", viewModel, "DiagonalUpLineStyle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("HorizontalColor", viewModel, "HorizontalColor", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("HorizontalLineStyle", viewModel, "HorizontalLineStyle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("VerticalColor", viewModel, "VerticalColor", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("VerticalLineStyle", viewModel, "VerticalLineStyle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("IsLeftBorderChecked", viewModel, "ApplyLeftBorder", false, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("IsRightBorderChecked", viewModel, "ApplyRightBorder", false, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("IsTopBorderChecked", viewModel, "ApplyTopBorder", false, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("IsBottomBorderChecked", viewModel, "ApplyBottomBorder", false, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("IsDiagonalUpBorderChecked", viewModel, "ApplyDiagonalUpBorder", false, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("IsDiagonalDownBorderChecked", viewModel, "ApplyDiagonalDownBorder", false, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("IsInsideVerticalBorderChecked", viewModel, "ApplyVerticalBorder", false, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("IsInsideHorizontalBorderChecked", viewModel, "ApplyHorizontalBorder", false, DataSourceUpdateMode.OnPropertyChanged);
			this.formatBorderControl.DataBindings.Add("RangeType", viewModel, "RangeType", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnRangeTypeBindingComplete;
		}
		#region Border's Event
		void OnRangeTypeBindingComplete(object sender, BindingCompleteEventArgs e) {
			if (this.formatBorderControl.RangeType != SelectedRangeTypeForBorderPreview.Cell)
				this.btnInside.Enabled = true;
		}
		void OutlineOnClick(object sender, EventArgs e) {
			viewModel.IsOutline = true;
			viewModel.RightColor = viewModel.Color;
			viewModel.TopColor = viewModel.Color;
			viewModel.BottomColor = viewModel.Color;
			viewModel.LeftColor = viewModel.Color;
			viewModel.LeftLineStyle = viewModel.LineStyle;
			viewModel.RightLineStyle = viewModel.LineStyle;
			viewModel.BottomLineStyle = viewModel.LineStyle;
			viewModel.TopLineStyle = viewModel.LineStyle;
			if (viewModel.LineStyle == XlBorderLineStyle.None)
				this.formatBorderControl.InitializeOutlineBorder(false);
			else
				this.formatBorderControl.InitializeOutlineBorder(true);
			this.formatBorderControl.Refresh();
			viewModel.IsOutline = false;
		}
		void InsideOnClick(object sender, EventArgs e) {
			viewModel.IsInside = true;
			if (this.formatBorderControl.RangeType == SelectedRangeTypeForBorderPreview.Row || this.formatBorderControl.RangeType == SelectedRangeTypeForBorderPreview.Table) {
				viewModel.VerticalColor = viewModel.Color;
				viewModel.VerticalLineStyle = viewModel.LineStyle;
				viewModel.ApplyVerticalBorder = true;
			}
			if (this.formatBorderControl.RangeType == SelectedRangeTypeForBorderPreview.Column || this.formatBorderControl.RangeType == SelectedRangeTypeForBorderPreview.Table) {
				viewModel.HorizontalColor = viewModel.Color;
				viewModel.HorizontalLineStyle = viewModel.LineStyle;
				viewModel.ApplyHorizontalBorder = true;
			}
			if (viewModel.LineStyle == XlBorderLineStyle.None) {
				this.formatBorderControl.IsInsideVerticalBorderChecked = false;
				this.formatBorderControl.IsInsideHorizontalBorderChecked = false;
			}
			this.formatBorderControl.Refresh();
			viewModel.IsInside = false;
		}
		void NoneOnClick(object sender, EventArgs e) {
			viewModel.IsNoBorder = true;
			viewModel.RightColor = DXColor.Empty;
			viewModel.TopColor = DXColor.Empty;
			viewModel.BottomColor = DXColor.Empty;
			viewModel.LeftColor = DXColor.Empty;
			viewModel.DiagonalColor = DXColor.Empty;
			viewModel.VerticalColor = DXColor.Empty;
			viewModel.HorizontalColor = DXColor.Empty;
			viewModel.LeftLineStyle = XlBorderLineStyle.None;
			viewModel.RightLineStyle = XlBorderLineStyle.None;
			viewModel.BottomLineStyle = XlBorderLineStyle.None;
			viewModel.TopLineStyle = XlBorderLineStyle.None;
			viewModel.DiagonalUpLineStyle = XlBorderLineStyle.None;
			ViewModel.DiagonalDownLineStyle = XlBorderLineStyle.None;
			viewModel.VerticalLineStyle = XlBorderLineStyle.None;
			viewModel.HorizontalLineStyle = XlBorderLineStyle.None;
			this.formatBorderControl.InitializeNoBorderBorder(false);
			this.formatBorderControl.Refresh();
			viewModel.IsNoBorder = false;
		}
		#endregion
		protected internal void SetBindingFill() {
			this.edtPatternStyle.Properties.Items.AddEnum(typeof(XlPatternType));
			this.edtPatternStyle.Properties.Items.RemoveAt(0);
			this.edtPatternStyle.DataBindings.Add("EditValue", viewModel, "PatternType", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnPatternStyleBindingComplete;
			this.chkNoColor.DataBindings.Add("Checked", viewModel, "IsFillColorEmpty", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtBackColor.DataBindings.Add("Color", viewModel, "FillBackColor", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnBackColorBindingComplete;
			this.edtPatternColor.DataBindings.Add("Color", viewModel, "FillForeColor", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnPatternColorBindingComplete;
			InitializeFillsColors();
		}
		#region Fill's Events
		void ConvertFillsSolidPatternType() {
			Color? temp = new Color();
			if (viewModel.PatternType.HasValue && viewModel.PatternType == XlPatternType.Solid) {
				temp = viewModel.FillForeColor;
				viewModel.FillForeColor = viewModel.FillBackColor;
				viewModel.FillBackColor = temp;
			}
		}
		void InitializeFillsColors() {
			ConvertFillsSolidPatternType();
		}
		void OnPatternTypePaint(object sender, PaintEventArgs e) {
			HatchStyle hatchStyle;
			DevExpress.XtraSpreadsheet.Forms.Design.PatternStyleComboBoxEditColor.DrawComboBoxEditColor = ViewModel.FillForeColor.HasValue ? GetNotEmptyColor(ViewModel.FillForeColor.Value) : Color.Black;
			viewModel.BrushHatchStyle.TryGetValue(viewModel.PatternType.GetValueOrDefault(), out hatchStyle);
			if (viewModel.PatternType.HasValue && viewModel.PatternType != XlPatternType.None && viewModel.FillForeColor.HasValue && viewModel.FillBackColor.HasValue) {
				if (viewModel.PatternType != XlPatternType.Solid)
					e.Graphics.FillRectangle(new HatchBrush(hatchStyle, GetNotEmptyColor(viewModel.FillForeColor.Value), viewModel.FillBackColor.Value), drawPatternType.ClientRectangle);
				else
					e.Graphics.FillRectangle(new SolidBrush(viewModel.FillBackColor.Value), drawPatternType.ClientRectangle);
			}
		}
		private Color GetNotEmptyColor(Color color) {
			if (color.IsEmpty)
				return Color.Black;
			return color;
		}
		void OnBackColorChanging(object sender, ChangingEventArgs e) {
			if (ViewModel.PatternType.HasValue && ViewModel.PatternType == XlPatternType.None)
				ViewModel.PatternType = XlPatternType.Solid;
			this.drawPatternType.Refresh();
		}
		void OnPatternStyleBindingComplete(object sender, BindingCompleteEventArgs e) {
			this.edtPatternStyle.Refresh();
			this.drawPatternType.Refresh();
		}
		void OnBackColorBindingComplete(object sender, BindingCompleteEventArgs e) {
			this.edtPatternStyle.Refresh();
			this.drawPatternType.Refresh();
		}
		void OnPatternColorBindingComplete(object sender, BindingCompleteEventArgs e) {
			this.edtPatternStyle.Refresh();
			this.drawPatternType.Refresh();
		}
		#endregion
		protected internal void SetBindingsProtection() {
			this.chkLocked.Properties.AllowGrayed = viewModel.LockedIndeterminate;
			this.chkHidden.Properties.AllowGrayed = viewModel.HiddenIndeterminate;
			this.chkLocked.DataBindings.Add("CheckState", viewModel, "Locked", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkHidden.DataBindings.Add("CheckState", viewModel, "Hidden", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		protected internal void SetActiveTabPage(FormatCellsFormInitialTabPage intitialTabPage) {
			tabControl.SelectedTabPageIndex = (int)intitialTabPage;
		}
		void btnOk_Click(object sender, EventArgs e) {
			if (ViewModel.CheckNumberFormatIsValid())
				DialogResult = DialogResult.OK;
		}
	}
	#endregion
	#region FontSizeToRichEditParagraphSpacingBeforeCalculator
	public class FontSizeToRichEditParagraphSpacingBeforeCalculator {
		internal static List<FontSizeToSpacingBeforeConversaionItem> fontSizeToSpacingBeforeConversationTable =
			CreateFontSizeToSpacingBeforeConversationTable();
		static List<FontSizeToSpacingBeforeConversaionItem> CreateFontSizeToSpacingBeforeConversationTable() {
			List<FontSizeToSpacingBeforeConversaionItem> result = new List<FontSizeToSpacingBeforeConversaionItem>();
			result.Add(new FontSizeToSpacingBeforeConversaionItem(10, 380, (fontSize) => { return 380; }));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(15, 380));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(20, 375));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(30, 370));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(36, 365));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(40, 360));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(45, 355));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(50, 350));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(53, 340));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(56, 330));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(60, 320));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(63, 310));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(66, 300));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(70, 290));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(74, 280));
			result.Add(new FontSizeToSpacingBeforeConversaionItem(100, 280, (fontSize) => Math.Max(0, Convert.ToInt32(280 - (fontSize + fontSize / 5)))));
			return result;
		}
		const float minimumAllowedFontSize = 1.0f;
		public bool TryCalculateSpacingBefore(float fontSize, out float calculatedSpacing) {
			calculatedSpacing = 0;
			if (fontSize < minimumAllowedFontSize)
				return false;
			int index = Algorithms.BinarySearch<FontSizeToSpacingBeforeConversaionItem>(fontSizeToSpacingBeforeConversationTable, new FontSizeToSpacingComparable(fontSize));
			if (index < 0)
				index = ~index;
			bool result = index >= 0 && index < fontSizeToSpacingBeforeConversationTable.Count;
			if (result)
				calculatedSpacing = fontSizeToSpacingBeforeConversationTable[index].CalculateSpacingBefore(fontSize);
			return result;
		}
	}
	delegate float FontSizeToParagraphSpacingCalculationDelegate(float fontSize);
	class FontSizeToSpacingBeforeConversaionItem {
		float fontSizeEnd;
		FontSizeToParagraphSpacingCalculationDelegate doCalculate = null;
		public FontSizeToSpacingBeforeConversaionItem(float fontSizeEnd, int baseSpacingBefore)
			: this(fontSizeEnd, baseSpacingBefore, (fontSize) => { return baseSpacingBefore - fontSize; }) {
		}
		public FontSizeToSpacingBeforeConversaionItem(float fontSizeEnd, int baseSpacingBefore, FontSizeToParagraphSpacingCalculationDelegate calculateMethod) {
			this.fontSizeEnd = fontSizeEnd;
			this.doCalculate = calculateMethod;
		}
		public float FontSizeEnd { get { return fontSizeEnd; } }
		public virtual float CalculateSpacingBefore(float fontSize) {
			return doCalculate(fontSize);
		}
	}
	internal class FontSizeToSpacingComparable : IComparable<FontSizeToSpacingBeforeConversaionItem> {
		readonly float fontSize;
		public FontSizeToSpacingComparable(float fontSize) {
			this.fontSize = fontSize;
		}
		public int CompareTo(FontSizeToSpacingBeforeConversaionItem other) {
			return other.FontSizeEnd.CompareTo(fontSize);
		}
	}
	#endregion
}
