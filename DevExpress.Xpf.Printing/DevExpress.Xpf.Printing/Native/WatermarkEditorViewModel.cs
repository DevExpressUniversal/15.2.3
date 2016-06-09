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
using System.Linq.Expressions;
using DevExpress.Xpf.Core;
using DevExpress.XtraPrinting.Design;
using DevExpress.XtraPrinting.Drawing;
using System.Windows;
using DevExpress.Utils.Internal;
using System.Linq;
using DevExpress.XtraPrinting.Native;
using System.Windows.Input;
using System.IO;
using DevExpress.XtraPrinting.XamlExport;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
#if SL
using DevExpress.Xpf.Drawing.Printing;
using DevExpress.Xpf.Drawing;
using FontStyle = DevExpress.Xpf.Drawing.FontStyle;
using System.Windows.Media;
#else
using System.Drawing.Printing;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Color = System.Windows.Media.Color;
using FontStyle = System.Drawing.FontStyle;
using System.Drawing;
using DevExpress.Printing.Native;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class WatermarkEditorViewModel : INotifyPropertyChanged {
		struct TransparencyRange {
			public double Min;
			public double Max;
			public double GetRange() { return Max - Min; }
		}
		#region Fields and Properties
		string pageRange = string.Empty;
		static string[] textStandardValues = WMTextConverter.StandardValues;
		LocalizedEnumWrapper<DirectionMode> textDirectionMode;
		static LocalizedEnumWrapper<DirectionMode>[] directionModeStandardValues;
		static string[] fontNameStandardValues;
		static string[] fontSizeStandardValues;
		TransparencyRange textTransparencyRange;
		LocalizedEnumWrapper<ImageViewMode> pictureViewMode;
		static LocalizedEnumWrapper<ImageViewMode>[] pictureViewModeStandardValues;
		LocalizedEnumWrapper<HorizontalAlignment> pictureHorizontalAlignment;
		static LocalizedEnumWrapper<HorizontalAlignment>[] pictureHorizontalAlignmentStandardValues;
		LocalizedEnumWrapper<VerticalAlignment> pictureVerticalAlignment;
		static LocalizedEnumWrapper<VerticalAlignment>[] pictureVerticalAlignmentStandardValues;
		TransparencyRange pictureTransparencyRange;
		bool isPageRange;
		readonly DelegateCommand<object> loadImageCommand;
		readonly DelegateCommand<object> clearAllCommand;
		FrameworkElement watermarkPreview;
		bool needToRestartBackgroundWorker;
		DevExpress.XtraPrinting.Page page;
		int pageCount;
		readonly IBackgroundWorkerWrapper backgroundWatermarkPreviewMaker;
		XpfWatermark watermark;
		int suppressUpdateWatermarkPreview;
		string pictureFileName;
		readonly TransparencyRange watermarkTransparencyRange = new TransparencyRange() { Max = 255, Min = 0 };
		const int textTransparencySteps = 10;
		const int textTransparencyLargeSteps = 5;
		const int pictureTransparencySteps = 10;
		const int pictureTransparencyLargeSteps = 5;
		const int maxFontSize = 3500;
		public IDialogService DialogService { get; set; }
		public string Text {
			get {
				return watermark.Text;
			}
			set {
				if(watermark.Text == value)
					return;
				watermark.Text = value;
				RaisePropertyChanged(() => Text);
				RaisePropertyChanged(() => TextIsNotEmpty);
				UpdateWatermarkPreview();
			}
		}
		public static string[] TextStandardValues {
			get {
				return textStandardValues;
			}
		}
		public LocalizedEnumWrapper<DirectionMode> TextDirectionMode {
			get {
				return textDirectionMode;
			}
			set {
				if(textDirectionMode == value)
					return;
				textDirectionMode = value;
				watermark.TextDirection = textDirectionMode.Value;
				RaisePropertyChanged(() => TextDirectionMode);
				UpdateWatermarkPreview();
			}
		}
		public static LocalizedEnumWrapper<DirectionMode>[] TextDirectionModeStandardValues {
			get {
				return directionModeStandardValues;
			}
		}
		public Color TextColor {
			get {
#if SL
				Color color = watermark.ForeColor;
#else
				Color color = Color.FromArgb(watermark.ForeColor.A, watermark.ForeColor.R, watermark.ForeColor.G, watermark.ForeColor.B);
#endif
				return color;
			}
			set {
#if SL
				Color color = value;
#else
				System.Drawing.Color color = System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B);
#endif
				if(watermark.ForeColor == color)
					return;
				watermark.ForeColor = color;
				RaisePropertyChanged(() => TextColor);
				UpdateWatermarkPreview();
			}
		}
		public string FontName {
			get {
				return watermark.Font.Name;
			}
			set {
				if(watermark.Font.Name == value)
					return;
				watermark.Font = new Font(value, watermark.Font.Size, watermark.Font.Style);
				RaisePropertyChanged(() => FontName);
				UpdateWatermarkPreview();
			}
		}
		public float FontSize {
			get {
				return watermark.Font.Size;
			}
			set {
				if(watermark.Font.Size == value)
					return;
				watermark.Font = new Font(watermark.Font.Name, value, watermark.Font.Style);
				RaisePropertyChanged(() => FontSize);
				UpdateWatermarkPreview();
			}
		}
		public static string[] FontSizeStandardValues {
			get { return fontSizeStandardValues; }
		}
		public static string[] FontNameStandardValues {
			get {
				return fontNameStandardValues;
			}
		}
		public bool IsTextItalic {
			get {
#if SL
				return watermark.Font.Style.IsItalic();
#else
				return watermark.Font.Italic;
#endif
			}
			set {
#if SL
				if(watermark.Font.Style.IsItalic() == value)
#else
				if(watermark.Font.Italic == value)
#endif
					return;
				FontStyle fontStyle = FontStyle.Regular;
				if(value)
					fontStyle = FontStyle.Italic;
				if(IsTextBold)
					fontStyle |= FontStyle.Bold;
				watermark.Font = new Font(watermark.Font.Name, watermark.Font.Size, fontStyle);
				RaisePropertyChanged(() => IsTextItalic);
				UpdateWatermarkPreview();
			}
		}
		public bool IsTextBold {
			get {
#if SL
				return watermark.Font.Style.IsBold();
#else
				return watermark.Font.Bold;
#endif
			}
			set {
#if SL
				if(watermark.Font.Style.IsBold() == value)
#else
				if(watermark.Font.Bold == value)
#endif
					return;
				FontStyle fontStyle = FontStyle.Regular;
				if(value)
					fontStyle = FontStyle.Bold;
				if(IsTextItalic)
					fontStyle |= FontStyle.Italic;
				watermark.Font = new Font(watermark.Font.Name, watermark.Font.Size, fontStyle);
				RaisePropertyChanged(() => IsTextBold);
				UpdateWatermarkPreview();
			}
		}
		public int MinTextTransparency {
			get {
				return (int)textTransparencyRange.Min;
			}
			set {
				if((int)textTransparencyRange.Min == value || value >= MaxTextTransparency)
					return;
				textTransparencyRange.Min = value;
				RaisePropertyChanged(() => MinTextTransparency);
			}
		}
		public int MaxTextTransparency {
			get {
				return (int)textTransparencyRange.Max;
			}
			set {
				if((int)textTransparencyRange.Max == value || value <= MinTextTransparency)
					return;
				textTransparencyRange.Max = value;
				RaisePropertyChanged(() => MaxTextTransparency);
			}
		}
		public double TextTransparency {
			get {
				return RecalculateTransparencyRange(watermark.TextTransparency, watermarkTransparencyRange, textTransparencyRange);
			}
			set {
				if(TextTransparency == value || value < MinTextTransparency || value > MaxTextTransparency)
					return;
				watermark.TextTransparency = (int)RecalculateTransparencyRange(value, textTransparencyRange, watermarkTransparencyRange);
				BeginUpdate();
				RaisePropertyChanged(() => TextTransparency);
				EndUpdate();
			}
		}
		public LocalizedEnumWrapper<ImageViewMode> PictureViewMode {
			get {
				return pictureViewMode;
			}
			set {
				if(pictureViewMode == value)
					return;
				pictureViewMode = value;
				watermark.ImageViewMode = pictureViewMode.Value;
				RaisePropertyChanged(() => PictureViewMode);
				UpdateWatermarkPreview();
			}
		}
		public static LocalizedEnumWrapper<ImageViewMode>[] PictureViewModeStandardValues {
			get {
				return pictureViewModeStandardValues;
			}
		}
		public bool IsPictureTiled {
			get {
				return watermark.ImageTiling;
			}
			set {
				if(watermark.ImageTiling == value)
					return;
				watermark.ImageTiling = value;
				RaisePropertyChanged(() => IsPictureTiled);
				UpdateWatermarkPreview();
			}
		}
		public LocalizedEnumWrapper<HorizontalAlignment> PictureHorizontalAlignment {
			get {
				return pictureHorizontalAlignment;
			}
			set {
				if(pictureHorizontalAlignment == value)
					return;
				pictureHorizontalAlignment = value;
				if(pictureHorizontalAlignment != null && pictureVerticalAlignment != null)
					watermark.ImageAlign = (ContentAlignment)ContentAlignmentHelper.ContentAlignmentFromAlignments(pictureHorizontalAlignment.Value, pictureVerticalAlignment.Value);
				RaisePropertyChanged(() => PictureHorizontalAlignment);
				UpdateWatermarkPreview();
			}
		}
		public static LocalizedEnumWrapper<HorizontalAlignment>[] PictureHorizontalAlignmentStandardValues {
			get {
				return pictureHorizontalAlignmentStandardValues;
			}
		}
		public LocalizedEnumWrapper<VerticalAlignment> PictureVerticalAlignment {
			get {
				return pictureVerticalAlignment;
			}
			set {
				if(pictureVerticalAlignment == value)
					return;
				pictureVerticalAlignment = value;
				if(pictureHorizontalAlignment != null && pictureVerticalAlignment != null)
					watermark.ImageAlign = (ContentAlignment)ContentAlignmentHelper.ContentAlignmentFromAlignments(
						pictureHorizontalAlignment.Value, pictureVerticalAlignment.Value);
				RaisePropertyChanged(() => PictureVerticalAlignment);
				UpdateWatermarkPreview();
			}
		}
		public static LocalizedEnumWrapper<VerticalAlignment>[] PictureVerticalAlignmentStandardValues {
			get {
				return pictureVerticalAlignmentStandardValues;
			}
		}
		public double PictureTransparency {
			get {
				return RecalculateTransparencyRange(watermark.ImageTransparency, watermarkTransparencyRange, pictureTransparencyRange);
			}
			set {
				if(PictureTransparency == value || value < MinPictureTransparency || value > MaxPictureTransparency)
					return;
				watermark.ImageTransparency = (int)RecalculateTransparencyRange(value, pictureTransparencyRange, watermarkTransparencyRange);
				BeginUpdate();
				RaisePropertyChanged(() => PictureTransparency);
				EndUpdate();
			}
		}
		public int MinPictureTransparency {
			get {
				return (int)pictureTransparencyRange.Min;
			}
			set {
				if((int)pictureTransparencyRange.Min == value || value >= MaxPictureTransparency)
					return;
				pictureTransparencyRange.Min = value;
				RaisePropertyChanged(() => MinPictureTransparency);
			}
		}
		public int MaxPictureTransparency {
			get {
				return (int)pictureTransparencyRange.Max;
			}
			set {
				if((int)pictureTransparencyRange.Max == value || value <= MinPictureTransparency)
					return;
				pictureTransparencyRange.Max = value;
				RaisePropertyChanged(() => MaxPictureTransparency);
			}
		}
		public bool ShowBehind {
			get {
				return watermark.ShowBehind;
			}
			set {
				watermark.ShowBehind = value;
				RaisePropertyChanged(() => ShowBehind);
				UpdateWatermarkPreview();
			}
		}
		public string PageRange {
			get {
				return !string.IsNullOrEmpty(pageRange) ? pageRange : pageRange = watermark.PageRange;
			}
			set {
				pageRange = value;
				watermark.PageRange = PageRangeParser.ValidateString(value);
				if(!string.IsNullOrEmpty(watermark.PageRange))
					IsPageRange = true;
				else
					IsPageRange = false;
				RaisePropertyChanged(() => PageRange);
			}
		}
		public bool IsPageRange {
			get {
				return isPageRange;
			}
			set {
				if(isPageRange == value)
					return;
				isPageRange = value;
				if(!value)
					PageRange = "";
				RaisePropertyChanged(() => IsPageRange);
			}
		}
		public byte[] Picture {
			get {
				return watermark.ImageArray;
			}
			set {
				if(watermark.ImageArray == value)
					return;
				try {
					watermark.ImageArray = value;
				} catch {
					watermark.ImageArray = null;
					DialogService.ShowError(PrintingLocalizer.GetString(PrintingStringId.WatermarkImageLoadError), PrintingLocalizer.GetString(PrintingStringId.Error));
				}
				BeginUpdate();
				RaisePropertyChanged(() => Picture);
				RaisePropertyChanged(() => Bitmap);
				RaisePropertyChanged(() => IsPictureLoaded);
				EndUpdate();
			}
		}
		public Bitmap Bitmap {
			get {
				return (Bitmap)watermark.Image;
			}
		}
		public bool IsPictureLoaded {
			get {
				return watermark.Image != null;
			}
		}
		public ICommand LoadImageCommand {
			get {
				return loadImageCommand;
			}
		}
		public DelegateCommand<object> ClearAllCommand {
			get {
				return clearAllCommand;
			}
		}
		public FrameworkElement WatermarkPreview {
			get {
				return watermarkPreview;
			}
		}
		public DevExpress.XtraPrinting.Page Page {
			get {
				return page;
			}
			set {
				if(page == value)
					return;
				page = value;
				RaisePropertyChanged(() => Page);
				UpdateWatermarkPreview();
			}
		}
		public int PageCount {
			get {
				return pageCount;
			}
			set {
				if(pageCount == value)
					return;
				pageCount = value;
				RaisePropertyChanged(() => PageCount);
			}
		}
		public int[] PageIndexes {
			get {
				int[] pageIndexes;
				if(IsPageRange) {
					pageIndexes = PageRangeParser.GetIndices(watermark.PageRange, PageCount);
				} else
					pageIndexes = PageRangeParser.GetIndices("", PageCount);
				return pageIndexes;
			}
		}
		public XpfWatermark Watermark {
			get { return watermark; }
			set {
				if(watermark == value)
					return;
				BeginUpdate();
				watermark = value;
				PictureViewMode = new LocalizedEnumWrapper<ImageViewMode>(watermark.ImageViewMode, WatermarkLocalizers.LocalizeImageViewMode);
				ContentAlignment pictureAlignment = watermark.ImageAlign;
				PictureHorizontalAlignment = new LocalizedEnumWrapper<HorizontalAlignment>(
					ContentAlignmentHelper.HorizontalAlignmentFromContentAlignment(pictureAlignment), WatermarkLocalizers.LocalizeHorizontalAlignment);
				PictureVerticalAlignment = new LocalizedEnumWrapper<VerticalAlignment>(
					ContentAlignmentHelper.VerticalAlignmentFromContentAlignment(pictureAlignment), WatermarkLocalizers.LocalizeVerticalAlignment);
				TextDirectionMode = new LocalizedEnumWrapper<DirectionMode>(watermark.TextDirection, WatermarkLocalizers.LocalizeDirectionMode);
				PageRange = watermark.PageRange;
				ShowBehind = watermark.ShowBehind;
				RaisePropertyChanged(() => Text);
				RaisePropertyChanged(() => IsTextBold);
				RaisePropertyChanged(() => IsTextItalic);
				RaisePropertyChanged(() => TextColor);
				RaisePropertyChanged(() => FontName);
				RaisePropertyChanged(() => FontSize);
				RaisePropertyChanged(() => TextTransparency);
				RaisePropertyChanged(() => IsPictureTiled);
				RaisePropertyChanged(() => PictureTransparency);
				RaisePropertyChanged(() => IsPictureLoaded);
				EndUpdate();
			}
		}
		public string PictureFileName {
			get {
				return pictureFileName;
			}
			set {
				if(pictureFileName == value)
					return;
				pictureFileName = value;
				RaisePropertyChanged(() => PictureFileName);
			}
		}
		public int TextTransparencyStep {
			get {
				return (int)textTransparencyRange.GetRange() / textTransparencySteps;
			}
		}
		public int TextTransparencyLargeStep {
			get {
				return (int)textTransparencyRange.GetRange() / textTransparencyLargeSteps;
			}
		}
		public int PictureTransparencyStep {
			get {
				return (int)pictureTransparencyRange.GetRange() / pictureTransparencySteps;
			}
		}
		public int PictureTransparencyLargeStep {
			get {
				return (int)pictureTransparencyRange.GetRange() / pictureTransparencyLargeSteps;
			}
		}
		public bool TextIsNotEmpty {
			get { return !string.IsNullOrEmpty(Text); }
		}
		#endregion
		#region ctor
		public WatermarkEditorViewModel() {
			watermark = new XpfWatermark();
			page = new PSPage(new ReadonlyPageData(
				new Margins(10, 10, 10, 10), new Margins(), PaperKind.Letter, PageSizeInfo.GetPageSize(PaperKind.Letter), false));
			needToRestartBackgroundWorker = false;
			backgroundWatermarkPreviewMaker = CreateBackgroundWorkerWrapper();
			backgroundWatermarkPreviewMaker.DoWork += backgroundWatermarkPreviewMaker_DoWork;
			backgroundWatermarkPreviewMaker.RunWorkerCompleted += backgroundWatermarkPreviewMaker_RunWorkerCompleted;
			ClearAll(null);
			loadImageCommand = DelegateCommandFactory.Create<object>(LoadImage, CanLoadImage, false);
			clearAllCommand = DelegateCommandFactory.Create<object>(ClearAll, false);
		}
		#endregion
		#region Methods
		void LoadImage(object parameter) {
#if SL
			string filter = "Image files (*.jpg,*.png)|*.jpg;*.png";
#else
			string filter = "Image files (*.bmp,*.jpg,*.gif,*.png,*.dib,*.tif)|*.bmp;*.jpg;*.gif;*.png;*.dib;*.tif";
#endif
			string fileName = "";
			using(Stream stream = DialogService.ShowOpenFileDialog("", filter, out fileName)) {
				if(stream != null) {
					byte[] buffer = new byte[stream.Length];
					try {
						stream.Read(buffer, 0, (int)stream.Length);
						BeginUpdate();
						Picture = buffer;
						PictureFileName = fileName;
					} catch(IOException) {
						BeginUpdate();
						Picture = null;
						PictureFileName = "";
						DialogService.ShowError(
							PrintingLocalizer.GetString(PrintingStringId.WatermarkImageLoadError), PrintingLocalizer.GetString(PrintingStringId.Error));
					} finally {
						EndUpdate();
					}
				}
			}
		}
		bool CanLoadImage(object parameter) {
			return true;
		}
		void ClearAll(object parameter) {
			BeginUpdate();
			textTransparencyRange = new TransparencyRange() { Max = 100, Min = 0 };
			pictureTransparencyRange = new TransparencyRange() { Max = 100, Min = 0 };
			TextTransparency = 20;
			directionModeStandardValues = new LocalizedEnumWrapper<DirectionMode>[] {				 
				new LocalizedEnumWrapper<DirectionMode>(DirectionMode.Horizontal, WatermarkLocalizers.LocalizeDirectionMode),
				new LocalizedEnumWrapper<DirectionMode>(DirectionMode.Vertical, WatermarkLocalizers.LocalizeDirectionMode),
				new LocalizedEnumWrapper<DirectionMode>(DirectionMode.ForwardDiagonal, WatermarkLocalizers.LocalizeDirectionMode),
				new LocalizedEnumWrapper<DirectionMode>(DirectionMode.BackwardDiagonal, WatermarkLocalizers.LocalizeDirectionMode)			
			};
			fontNameStandardValues = FontManager.GetFontFamilyNames().ToArray();			
			int[] fontSizes = FontManager.GetPredefinedFontSizes().ToArray();
			fontSizeStandardValues = new string[fontSizes.Length];
			for(int i = 0; i < fontSizes.Length; i++) {
				fontSizeStandardValues[i] = fontSizes[i].ToString();
			}
			pictureViewModeStandardValues = new LocalizedEnumWrapper<ImageViewMode>[] {
				new LocalizedEnumWrapper<ImageViewMode>(ImageViewMode.Clip,WatermarkLocalizers.LocalizeImageViewMode),
				new LocalizedEnumWrapper<ImageViewMode>(ImageViewMode.Stretch,WatermarkLocalizers.LocalizeImageViewMode),
				new LocalizedEnumWrapper<ImageViewMode>(ImageViewMode.Zoom,WatermarkLocalizers.LocalizeImageViewMode)				
			};
			pictureHorizontalAlignmentStandardValues = new LocalizedEnumWrapper<HorizontalAlignment>[] {
				new LocalizedEnumWrapper<HorizontalAlignment>(HorizontalAlignment.Left, WatermarkLocalizers.LocalizeHorizontalAlignment),
				new LocalizedEnumWrapper<HorizontalAlignment>(HorizontalAlignment.Center, WatermarkLocalizers.LocalizeHorizontalAlignment),
				new LocalizedEnumWrapper<HorizontalAlignment>(HorizontalAlignment.Right, WatermarkLocalizers.LocalizeHorizontalAlignment)				
			};
			pictureVerticalAlignmentStandardValues = new LocalizedEnumWrapper<VerticalAlignment>[] {
				new LocalizedEnumWrapper<VerticalAlignment>(VerticalAlignment.Top,  WatermarkLocalizers.LocalizeVerticalAlignment),
				new LocalizedEnumWrapper<VerticalAlignment>(VerticalAlignment.Center, WatermarkLocalizers.LocalizeVerticalAlignment),
				new LocalizedEnumWrapper<VerticalAlignment>(VerticalAlignment.Bottom, WatermarkLocalizers.LocalizeVerticalAlignment)				
			};
			pictureVerticalAlignment = pictureVerticalAlignmentStandardValues[0];
			pictureVerticalAlignment = new LocalizedEnumWrapper<VerticalAlignment>(ContentAlignmentHelper.VerticalAlignmentFromContentAlignment(watermark.ImageAlign),
				WatermarkLocalizers.LocalizeVerticalAlignment);
			pictureHorizontalAlignment = new LocalizedEnumWrapper<HorizontalAlignment>(ContentAlignmentHelper.HorizontalAlignmentFromContentAlignment(watermark.ImageAlign),
				WatermarkLocalizers.LocalizeHorizontalAlignment);
			textDirectionMode = new LocalizedEnumWrapper<DirectionMode>(watermark.TextDirection, WatermarkLocalizers.LocalizeDirectionMode);
			pictureViewMode = new LocalizedEnumWrapper<ImageViewMode>(watermark.ImageViewMode, WatermarkLocalizers.LocalizeImageViewMode);
			Watermark = new XpfWatermark();
			PictureFileName = "";
			RaisePropertyChanged(() => Page);
			RaisePropertyChanged(() => TextIsNotEmpty);
			EndUpdate();
		}
		public static bool ValidatePageRange(string pageRange) {
			return pageRange == PageRangeParser.ValidateString(pageRange);
		}
		public static bool ValidateFontSize(string fontSize) {
			int size;
			if(!int.TryParse(fontSize, out size))
				return false;
			return size > 0 && size < maxFontSize;
		}
		void UpdateWatermarkPreview() {
			if(suppressUpdateWatermarkPreview > 0)
				return;
			if(!backgroundWatermarkPreviewMaker.IsBusy) {
				backgroundWatermarkPreviewMaker.RunWorkerAsync();
			} else
				needToRestartBackgroundWorker = true;
		}
		void backgroundWatermarkPreviewMaker_DoWork(object sender, DoWorkEventArgs e) {
#if SL
			page.AssignWatermark(watermark);
#else
			XpfWatermark copy = new XpfWatermark();
			copy.CopyFrom(watermark);
			page.AssignWatermark(copy);
#endif
			BrickPageVisualizer brickPageVisualizer = new BrickPageVisualizer(TextMeasurementSystem.NativeXpf);
			Stream stream = brickPageVisualizer.SaveToStream(page, 0, 0);
			e.Result = (object)stream;
		}
		void backgroundWatermarkPreviewMaker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if(needToRestartBackgroundWorker) {
				UpdateWatermarkPreview();
			} else {
				LoadWatermarkPreview(e.Result as Stream);
			}
			needToRestartBackgroundWorker = false;
		}
#if DEBUGTEST
		protected virtual
#endif
		void LoadWatermarkPreview(Stream stream) {
			watermarkPreview = XamlReaderHelper.Load(stream);
			RaisePropertyChanged(() => WatermarkPreview);
		}
#if DEBUGTEST
		protected virtual
#endif
		IBackgroundWorkerWrapper CreateBackgroundWorkerWrapper() {
			return new BackgroundWorkerWrapper();
		}
		public void BeginUpdate() {
			suppressUpdateWatermarkPreview++;
		}
		public void EndUpdate() {
			suppressUpdateWatermarkPreview--;
			UpdateWatermarkPreview();
		}
		static double RecalculateTransparencyRange(double transparency, TransparencyRange currentRange, TransparencyRange newRange) {
			return Math.Round(((transparency - currentRange.Min) / currentRange.GetRange()) * newRange.GetRange() + newRange.Min);
		}
		#endregion
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged<T>(Expression<Func<T>> property) {
			PropertyExtensions.RaisePropertyChanged(this, PropertyChanged, property);
		}
		#endregion
	}
}
