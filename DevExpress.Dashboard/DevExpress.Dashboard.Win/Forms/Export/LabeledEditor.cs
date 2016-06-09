#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardWin.Native.Printing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.DashboardWin.Forms.Export {
	public class ExportDocumenInfoEventArgs : EventArgs {
		public IOptionsEditor Initiator { get; set; }
	}
	public interface IOptionsEditor {
		void Set(ExtendedReportOptions options);
		void Apply(ExtendedReportOptions options);
		IEnumerable<LabelAndEditor> GetControls();
	}
	public class LabelAndEditor {
		public Control Label { get; private set; }
		public Control Editor { get; private set; }
		public LabelAndEditor(Control label, Control editor) {
			Label = label;
			Editor = editor;
		}
	}
	public enum PageLayout {
		Auto,
		Portrait,
		Landscape
	}
	public enum ExportImageFormat {
		Png,
		Jpeg,
		Gif
	}
	public class ComboBoxItem {
		object value;
		string stringValue;
		public ComboBoxItem(object value, string stringValue) {
			this.value = value;
			this.stringValue = stringValue;
		}
		public object Value { get { return value; } }
		public override string ToString() {
			return stringValue;
		}
	}
	public static class ComboBoxRadioGroupItemsHelper {
		public static ComboBoxItem CreateComboBoxItem(object value) {
			return new ComboBoxItem(value, ComboBoxRadioGroupItemsHelper.GetStringValue(value));
		}
		public static RadioGroupItem CreateRadioGroupItem(object value) {
			return new RadioGroupItem(value, ComboBoxRadioGroupItemsHelper.GetStringValue(value));
		}
		static string GetPageLayoutStringValue(PageLayout value) {
			switch(value) {
				case PageLayout.Auto:
					return DashboardLocalizer.GetString(DashboardStringId.PageLayoutAuto);
				case PageLayout.Landscape: 
					return DashboardLocalizer.GetString(DashboardStringId.PageLayoutLandscape);
				case PageLayout.Portrait: 
					return DashboardLocalizer.GetString(DashboardStringId.PageLayoutPortrait);
				default:
					throw new Exception();
			}
		}
		static string GetPaperKindStringValue(PaperKind value) {
			switch(value) {	 
				case PaperKind.Letter:
					return DashboardLocalizer.GetString(DashboardStringId.PaperKindLetter);
				case PaperKind.Legal:
					return DashboardLocalizer.GetString(DashboardStringId.PaperKindLegal);
				case PaperKind.Executive:
					return DashboardLocalizer.GetString(DashboardStringId.PaperKindExecutive);
				case PaperKind.A5: 
					return DashboardLocalizer.GetString(DashboardStringId.PaperKindA5);
				case PaperKind.A4: 
					return DashboardLocalizer.GetString(DashboardStringId.PaperKindA4);
				case PaperKind.A3: 
					return DashboardLocalizer.GetString(DashboardStringId.PaperKindA3);
				default:
					throw new Exception();
		   }
		}
		static string GetScaleModeStringValue(ExtendedScaleMode value) {
		   switch(value) {
			   case ExtendedScaleMode.None: 
					return DashboardLocalizer.GetString(DashboardStringId.ScaleModeNone);
			   case ExtendedScaleMode.UseScaleFactor:
					return DashboardLocalizer.GetString(DashboardStringId.ScaleModeUseScaleFactor);
			   case ExtendedScaleMode.AutoFitToPageWidth: 
					return DashboardLocalizer.GetString(DashboardStringId.ScaleModeAutoFitToPageWidth);
			   default:
					throw new Exception();
		   }
		}
		static string GetFilterStatePresentationStringValue(FilterStatePresentation value) {
			switch(value) { 
				case FilterStatePresentation.None:
					return DashboardLocalizer.GetString(DashboardStringId.FilterStatePresentationNone);
				case FilterStatePresentation.After: 
					return DashboardLocalizer.GetString(DashboardStringId.FilterStatePresentationAfter);
				case FilterStatePresentation.AfterAndSplitPage: 
					return DashboardLocalizer.GetString(DashboardStringId.FilterStatePresentationAfterAndSplitPage);
				default:
					throw new Exception();
			}
		}
		static string GetSizeModeStringValue(ItemSizeMode value) {
			switch(value) {
				case ItemSizeMode.None:
					return DashboardLocalizer.GetString(DashboardStringId.SizeModeNone);
				case ItemSizeMode.Stretch: 
					return DashboardLocalizer.GetString(DashboardStringId.SizeModeStretch);
				case ItemSizeMode.Zoom: 
					return DashboardLocalizer.GetString(DashboardStringId.SizeModeZoom);
				default:
					throw new Exception();
			}
		}
		static string GetImageFormatStringValue(ExportImageFormat value) {
			switch(value) {
				case ExportImageFormat.Jpeg:
					return "JPEG";
				case ExportImageFormat.Gif:
					return "GIF";
				default:
					return "PNG";
			}
		}
		static string GetDataFormatStringValue(ExcelFormat value) {
			switch(value) {
				case ExcelFormat.Xls:
					return "XLS";
				case ExcelFormat.Xlsx:
					return "XLSX";
				default:
					return "CSV";
			}
		}
		static string GetStringValue(object value) {
			if(value is PageLayout)
				return GetPageLayoutStringValue((PageLayout)value);
			else if(value is PaperKind)
				return GetPaperKindStringValue((PaperKind)value);
			else if(value is ExtendedScaleMode)
				return GetScaleModeStringValue((ExtendedScaleMode)value);
			else if(value is FilterStatePresentation) 
				return GetFilterStatePresentationStringValue((FilterStatePresentation)value);
			else if(value is ItemSizeMode)
				return GetSizeModeStringValue((ItemSizeMode)value);
			else if(value is ExportImageFormat)
				return GetImageFormatStringValue((ExportImageFormat)value);
			else if(value is ExcelFormat)
				return GetDataFormatStringValue((ExcelFormat)value);
			else
				throw new Exception();
		}
	}
	public abstract class LabeledEditor: IDisposable, IOptionsEditor {
		readonly LabelControl label = new LabelControl {
			Margin = new Padding(0)
		};
		Control Label { get { return label; } }
		protected abstract Control Editor { get; }
		protected LabeledEditor(string labelText) {
			this.label.Text = String.Format("{0}:", labelText);
		}
		public IEnumerable<LabelAndEditor> GetControls() {
			return new List<LabelAndEditor> { new LabelAndEditor(Label, Editor) };
		}
		public void Set(ExtendedReportOptions opts) {
			SetInternal(opts);
		}
		public abstract void Apply(ExtendedReportOptions opts);
		public void SetEnabled(bool enabled) {
			Label.Enabled = enabled;
			Editor.Enabled = enabled;
		}
		public void SetVisibility(bool visible) {
			Label.Visible = visible;
			Editor.Visible = visible;
		}
		public event EventHandler<ExportDocumenInfoEventArgs> ValueChanged;
		protected void OnValueChanged(ExportDocumenInfoEventArgs e) {
			if(ValueChanged != null)
				ValueChanged(this, e);
		}
		protected abstract void SetInternal(ExtendedReportOptions opts);
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				Label.Dispose();
				Editor.Dispose();
			}
		}
		protected static CheckEdit CreateCheckEdit(Action<bool> onCheckedChanged) {
			CheckEdit checkEdit = new CheckEdit {
				Text = String.Empty
			};
			checkEdit.CheckedChanged += (s, e) => {
				onCheckedChanged(checkEdit.Checked);
			};
			return checkEdit;
		}
		protected static CheckEdit CreateCheckEdit() {
			return CreateCheckEdit((v) => { });
		}
		protected static ComboBoxEdit CreateComboBoxEdit<T>(T[] values, Action<T> onEditValueChanged) where T : struct {
			ComboBoxEdit comboBoxEdit = new ComboBoxEdit {
				Width = 150
			};
			comboBoxEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			foreach(T value in values) {
				comboBoxEdit.Properties.Items.Add(ComboBoxRadioGroupItemsHelper.CreateComboBoxItem(value));
			}
			comboBoxEdit.EditValueChanged += (s, e) => {
				onEditValueChanged((T)((ComboBoxItem)comboBoxEdit.SelectedItem).Value);
			};
			comboBoxEdit.CustomDisplayText += (o, args) => comboBoxEdit.ToolTip = args.DisplayText;
			return comboBoxEdit;
		}
		protected static ComboBoxEdit CreateComboBoxEdit<T>(T[] values) where T : struct {
			return CreateComboBoxEdit(values, (v) => { });
		}
		protected static RadioGroup CreateRadioGroup<T>(T[] values, Action<T> onEditValueChanged) where T : struct {
			RadioGroup radioEdit = new RadioGroup {
				BackColor = Color.Transparent,
				Width = 150,
				Height = 60
			};
			radioEdit.Properties.BorderStyle = BorderStyles.NoBorder;
			foreach(T value in values) {
				radioEdit.Properties.Items.Add(ComboBoxRadioGroupItemsHelper.CreateRadioGroupItem(value));
			}
			radioEdit.EditValueChanged += (s, e) => {
				onEditValueChanged((T)radioEdit.EditValue);
			};
			return radioEdit;
		}
		protected static RadioGroup CreateRadioGroup<T>(T[] values) where T : struct {
			return CreateRadioGroup(values, (v) => { });
		}
		protected static SpinEdit CreateSpinEdit(bool isFloatValue, Action<decimal> onEditValueChanged) {
			SpinEdit spinEdit = new SpinEdit {
				Properties = {
					IsFloatValue = isFloatValue
				},
				Width = 50
			};
			spinEdit.EditValueChanged += (s, e) => {
				onEditValueChanged(spinEdit.Value);
			};
			return spinEdit;
		}
		protected static SpinEdit CreateSpinEdit(bool isFloatValue) {
			return CreateSpinEdit(isFloatValue, (v) => { });
		}
		protected static TextEdit CreateTextEdit() {
			return new TextEdit {
				Width = 150
			};
		}
	}
	public class SimplyPageLayoutEditor : LabeledEditor {
		readonly RadioGroup pageLayoutEdit;
		public SimplyPageLayoutEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.PageLayout)) {
			pageLayoutEdit = CreateRadioGroup<PageLayout>(new PageLayout[] { PageLayout.Portrait, PageLayout.Landscape });
		}
		protected override Control Editor { get { return pageLayoutEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			pageLayoutEdit.EditValue = opts.PaperOptions.Landscape ? PageLayout.Landscape : PageLayout.Portrait;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.PaperOptions.Landscape = (PageLayout)pageLayoutEdit.EditValue == PageLayout.Landscape;
		}
	}
	public abstract class AutoPageLayoutEditor : LabeledEditor {
		RadioGroup pageLayoutEdit;
		protected RadioGroup PageLayoutEdit {
			get { return pageLayoutEdit; }
			set { pageLayoutEdit = value; }
		}
		protected AutoPageLayoutEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.PageLayout)) {
		}
		protected override Control Editor { get { return pageLayoutEdit; } }
		public override void Apply(ExtendedReportOptions opts) {
			if((PageLayout)pageLayoutEdit.EditValue != PageLayout.Auto)
				opts.PaperOptions.Landscape = (PageLayout)pageLayoutEdit.EditValue == PageLayout.Landscape;			
		}
	}
	public class DashboardAutoPageLayoutEditor : AutoPageLayoutEditor {
		public DashboardAutoPageLayoutEditor()
			: base() {
				PageLayoutEdit = CreateRadioGroup<PageLayout>(new PageLayout[] { PageLayout.Portrait, PageLayout.Landscape, PageLayout.Auto }, (value) => { OnValueChanged(new ExportDocumenInfoEventArgs { Initiator = this }); });
		}
		protected override void SetInternal(ExtendedReportOptions opts) {
			PageLayout userLayout = opts.PaperOptions.Landscape ? PageLayout.Landscape : PageLayout.Portrait;
			PageLayoutEdit.EditValue = (opts.AutoPageOptions.AutoFitToPageSize && opts.AutoPageOptions.AutoRotate) ? PageLayout.Auto : userLayout;
		}
		public override void Apply(ExtendedReportOptions opts) {
			base.Apply(opts);
			bool autoLayout = (PageLayout)(PageLayoutEdit.EditValue) == PageLayout.Auto;
			if(autoLayout)
				opts.ScalingOptions.ScaleMode = ExtendedScaleMode.None;
			opts.AutoPageOptions.AutoFitToPageSize = autoLayout;
			opts.AutoPageOptions.AutoRotate = autoLayout;
		}
	}
	public class ItemAutoPageLayoutEditor : AutoPageLayoutEditor {
		public ItemAutoPageLayoutEditor()
			: base() {
			PageLayoutEdit = CreateRadioGroup<PageLayout>(new PageLayout[] { PageLayout.Portrait, PageLayout.Landscape, PageLayout.Auto });
		}
		protected override void SetInternal(ExtendedReportOptions opts) {
			PageLayout userLayout = opts.PaperOptions.Landscape ? PageLayout.Landscape : PageLayout.Portrait;
			PageLayoutEdit.EditValue = opts.AutoPageOptions.AutoRotate ? PageLayout.Auto : userLayout;
		}
		public override void Apply(ExtendedReportOptions opts) {
			base.Apply(opts);
			opts.AutoPageOptions.AutoRotate = (PageLayout)(PageLayoutEdit.EditValue) == PageLayout.Auto;
		}
	}
	public class PaperKindEditor : LabeledEditor {
		readonly ComboBoxEdit paperKindEdit;
		public PaperKindEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.PaperKind)) {
			paperKindEdit = CreateComboBoxEdit<PaperKind>(new PaperKind[] { PaperKind.Letter, PaperKind.Legal, PaperKind.Executive, PaperKind.A5, PaperKind.A4, PaperKind.A3 });
		}
		protected override Control Editor { get { return paperKindEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			paperKindEdit.EditValue = ComboBoxRadioGroupItemsHelper.CreateComboBoxItem(opts.PaperOptions.PaperKind);
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.PaperOptions.PaperKind = (PaperKind)(((ComboBoxItem)paperKindEdit.SelectedItem).Value);
		}
	}
	public class PDFFilterStatePresentationEditor : LabeledEditor {
		readonly ComboBoxEdit filterStatePresentationEdit;
		public PDFFilterStatePresentationEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.FilterStatePresentation)) {
			filterStatePresentationEdit = CreateComboBoxEdit<FilterStatePresentation>(new FilterStatePresentation[] { FilterStatePresentation.None, FilterStatePresentation.After, FilterStatePresentation.AfterAndSplitPage });
		}
		protected override Control Editor { get { return filterStatePresentationEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			filterStatePresentationEdit.EditValue = ComboBoxRadioGroupItemsHelper.CreateComboBoxItem(opts.DocumentContentOptions.FilterStatePresentation);
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.DocumentContentOptions.FilterStatePresentation = (FilterStatePresentation)(((ComboBoxItem)filterStatePresentationEdit.SelectedItem).Value);
		}
	}
	public class ImageFilterStatePresentationEditor : LabeledEditor {
		readonly ComboBoxEdit filterStatePresentationEdit;
		public ImageFilterStatePresentationEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.FilterStatePresentation)) {
			filterStatePresentationEdit = CreateComboBoxEdit<FilterStatePresentation>(new FilterStatePresentation[] { FilterStatePresentation.None, FilterStatePresentation.After });
		}
		protected override Control Editor { get { return filterStatePresentationEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			FilterStatePresentation value = opts.DocumentContentOptions.FilterStatePresentation == FilterStatePresentation.AfterAndSplitPage ? FilterStatePresentation.After : opts.DocumentContentOptions.FilterStatePresentation;
			filterStatePresentationEdit.EditValue = ComboBoxRadioGroupItemsHelper.CreateComboBoxItem(value);
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.DocumentContentOptions.FilterStatePresentation = (FilterStatePresentation)(((ComboBoxItem)filterStatePresentationEdit.SelectedItem).Value);
		}
	}
	public class ShowTitleEditor : LabeledEditor {
		readonly CheckEdit showTitleEdit;
		public ShowTitleEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.ShowTitle)) {
			showTitleEdit = CreateCheckEdit((value) => { OnValueChanged(new ExportDocumenInfoEventArgs { Initiator = this }); });
		}
		protected override Control Editor { get { return showTitleEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			showTitleEdit.Checked = opts.DocumentContentOptions.ShowTitle;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.DocumentContentOptions.ShowTitle = showTitleEdit.Checked;
		}
	}
	public class TitleEditor : LabeledEditor {
		readonly TextEdit titleEdit;
		public TitleEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.Title)) {
			titleEdit = CreateTextEdit();
		}
		protected override Control Editor { get { return titleEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			titleEdit.Text = opts.DocumentContentOptions.Title;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.DocumentContentOptions.Title = titleEdit.Text;
		}
	}
	public class ScaleModeEditor : LabeledEditor {
		readonly ComboBoxEdit scaleModeEdit;
		public ScaleModeEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.ScaleMode)) {
				scaleModeEdit = CreateComboBoxEdit(new ExtendedScaleMode[] { ExtendedScaleMode.None, ExtendedScaleMode.UseScaleFactor, ExtendedScaleMode.AutoFitToPageWidth }, (value) => { OnValueChanged(new ExportDocumenInfoEventArgs { Initiator = this }); });
		}
		protected override Control Editor { get { return scaleModeEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			scaleModeEdit.EditValue = ComboBoxRadioGroupItemsHelper.CreateComboBoxItem(opts.ScalingOptions.ScaleMode);
		}
		public override void Apply(ExtendedReportOptions opts) {
			if(scaleModeEdit.SelectedItem == null)
				return;
			opts.ScalingOptions.ScaleMode = (ExtendedScaleMode)(((ComboBoxItem)scaleModeEdit.SelectedItem).Value);
		}
	}
	public class ScaleFactorEditor : LabeledEditor {
		readonly SpinEdit scaleFactorEdit;
		public ScaleFactorEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.ScaleFactor)) {
			scaleFactorEdit = CreateSpinEdit(true, ValidateValue);
		}
		protected override Control Editor { get { return scaleFactorEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			scaleFactorEdit.Value = (decimal)opts.ScalingOptions.ScaleFactor;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.ScalingOptions.ScaleFactor = scaleFactorEdit.Enabled ? (float)scaleFactorEdit.Value : 1;
		}
		void ValidateValue(decimal editValue) {
			if(editValue <= 0)
				scaleFactorEdit.EditValue = 1;
			else if(editValue > 999) {
				scaleFactorEdit.EditValue = 999;
			}
		}
	}
	public class AutoFitPageCountEditor : LabeledEditor {
		readonly SpinEdit autoFitPageCountEdit;
		public AutoFitPageCountEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.AutoFitPageCount)) {
			autoFitPageCountEdit = CreateSpinEdit(false, ValidateValue);
		}
		protected override Control Editor { get { return autoFitPageCountEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			autoFitPageCountEdit.Value = opts.ScalingOptions.AutoFitPageCount;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.ScalingOptions.AutoFitPageCount = autoFitPageCountEdit.Enabled ? (int)autoFitPageCountEdit.Value : 1;
		}
		void ValidateValue(decimal editValue) {
			if(editValue < 1)
				autoFitPageCountEdit.EditValue = 1;
			else if(editValue > 999) {
				autoFitPageCountEdit.EditValue = 999;
			}
		}
	}	
	public class PrintHeadersOnEveryPageEditor : LabeledEditor {
		readonly CheckEdit printHeadersOnEveryPageEdit;
		public PrintHeadersOnEveryPageEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.PrintHeadersOnEveryPage)) {
			printHeadersOnEveryPageEdit = CreateCheckEdit();
		}
		protected override Control Editor { get { return printHeadersOnEveryPageEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			printHeadersOnEveryPageEdit.Checked = opts.ItemContentOptions.HeadersOptions.PrintHeadersOnEveryPage;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.ItemContentOptions.HeadersOptions.PrintHeadersOnEveryPage = printHeadersOnEveryPageEdit.Checked;
		}
	}
	public class FitToPageWidthEditor : LabeledEditor {
		readonly CheckEdit fitToPageWidthEdit;
		public FitToPageWidthEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.FitToPageWidth)) {
			fitToPageWidthEdit = CreateCheckEdit((value) => { OnValueChanged(new ExportDocumenInfoEventArgs { Initiator = this }); });
		}
		protected override Control Editor { get { return fitToPageWidthEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			fitToPageWidthEdit.Checked = opts.ItemContentOptions.SizeMode == ItemSizeMode.FitWidth;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.ItemContentOptions.SizeMode = fitToPageWidthEdit.Checked ? ItemSizeMode.FitWidth : ItemSizeMode.None;
		}
	}
	public class AutoArrangeContentEditor : LabeledEditor {
		readonly CheckEdit autoArrangeContentEdit;
		public AutoArrangeContentEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.AutoArrangeContent)) {
			autoArrangeContentEdit = CreateCheckEdit((value) => { OnValueChanged(new ExportDocumenInfoEventArgs { Initiator = this }); });
		}
		protected override Control Editor { get { return autoArrangeContentEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			autoArrangeContentEdit.Checked = opts.ItemContentOptions.ArrangerOptions.AutoArrangeContent;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.ItemContentOptions.ArrangerOptions.AutoArrangeContent = autoArrangeContentEdit.Checked;
		}
	}
	public class SizeModeTwoItemsEditor : LabeledEditor {
		readonly RadioGroup sizeModeEdit;
		public SizeModeTwoItemsEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.SizeMode))	{
				sizeModeEdit = CreateRadioGroup<ItemSizeMode>(new ItemSizeMode[] { ItemSizeMode.None, ItemSizeMode.Zoom });
		}
		protected override Control Editor { get { return sizeModeEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			sizeModeEdit.EditValue = opts.ItemContentOptions.SizeMode;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.ItemContentOptions.SizeMode = (ItemSizeMode)sizeModeEdit.EditValue;
		}
	}
	public class SizeModeThreeItemsEditor : LabeledEditor {
		readonly RadioGroup sizeModeEdit;
		public SizeModeThreeItemsEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.SizeMode)) {
				sizeModeEdit = CreateRadioGroup<ItemSizeMode>(new ItemSizeMode[] { ItemSizeMode.None, ItemSizeMode.Stretch, ItemSizeMode.Zoom });
		}
		protected override Control Editor { get { return sizeModeEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			sizeModeEdit.EditValue = opts.ItemContentOptions.SizeMode;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.ItemContentOptions.SizeMode = (ItemSizeMode)sizeModeEdit.EditValue;
		}
	}
	public class ImageFormatEditor : LabeledEditor {
		readonly ComboBoxEdit imageFormatEdit;
		public ImageFormatEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.ImageFormat)) {
			imageFormatEdit = CreateComboBoxEdit<ExportImageFormat>(new ExportImageFormat[] { ExportImageFormat.Png, ExportImageFormat.Jpeg, ExportImageFormat.Gif });
		}
		protected override Control Editor { get { return imageFormatEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			imageFormatEdit.EditValue = ComboBoxRadioGroupItemsHelper.CreateComboBoxItem(ConvertImageFormatToExportImageFormat(opts.FormatOptions.ImageOptions.Format));
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.FormatOptions.ImageOptions.Format = ConvertExportImageFormatToImageFormat((ExportImageFormat)(((ComboBoxItem)(imageFormatEdit.SelectedItem)).Value));
		}
		static ExportImageFormat ConvertImageFormatToExportImageFormat(ImageFormat imageFormat) {
			switch(imageFormat.ToString()) {
				case "Jpeg":
					return ExportImageFormat.Jpeg;
				case "Gif":
					return ExportImageFormat.Gif;
				default:
					return ExportImageFormat.Png;
			}
		}
		static ImageFormat ConvertExportImageFormatToImageFormat(ExportImageFormat imageFormat) {
			switch(imageFormat) {
				case ExportImageFormat.Jpeg:
					return ImageFormat.Jpeg;
				case ExportImageFormat.Gif:
					return ImageFormat.Gif;
				default:
					return ImageFormat.Png;
			}
		}
	}
	public class ResolutionEditor : LabeledEditor {
		readonly SpinEdit resolutionEdit;
		public ResolutionEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.Resolution)) {
			resolutionEdit = CreateSpinEdit(false, ValidateValue);
		}
		protected override Control Editor { get { return resolutionEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			resolutionEdit.Value = opts.FormatOptions.ImageOptions.Resolution;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.FormatOptions.ImageOptions.Resolution = (int)resolutionEdit.Value;
		}
		void ValidateValue(decimal editValue) {
			if(editValue < 1)
				resolutionEdit.EditValue = 1;
			else if(editValue > 999) {
				resolutionEdit.EditValue = 999;
			}
		}
	}
}
