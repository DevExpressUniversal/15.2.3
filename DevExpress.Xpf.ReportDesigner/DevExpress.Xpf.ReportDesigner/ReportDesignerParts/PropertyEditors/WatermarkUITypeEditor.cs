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
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class BooleanTemplateSelector : DataTemplateSelector {
		public DataTemplate FalseValueTemplate { get; set; }
		public DataTemplate TrueValueTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			return ((BooleanViewModel)item).Value ? TrueValueTemplate : FalseValueTemplate;
		}
	}
	public class WatermarkUITypeEditor : Control {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty ReportProperty;
		static readonly Action<WatermarkUITypeEditor, Action<IOpenFileDialogService>> openFileDialogServiceAccessor;
		public static readonly DependencyProperty OpenFileDialogServiceTemplateProperty;
		static readonly Action<WatermarkUITypeEditor, Action<IMessageBoxService>> messageBoxServiceAccessor;
		public static readonly DependencyProperty MessageBoxServiceTemplateProperty;
		static readonly DependencyPropertyKey ImagePropertyKey;
		public static readonly DependencyProperty ImageProperty;
		static readonly DependencyPropertyKey ImageFileNamePropertyKey;
		public static readonly DependencyProperty ImageFileNameProperty;
		public static readonly DependencyProperty ImageViewModeProperty;
		public static readonly DependencyProperty ImageHorizontalAlignmentProperty;
		public static readonly DependencyProperty ImageVerticalAlignmentProperty;
		public static readonly DependencyProperty ImageIsTiledProperty;
		public static readonly DependencyProperty ImageTransparencyProperty;
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty TextDirectionProperty;
		public static readonly DependencyProperty TextFontFamilyProperty;
		public static readonly DependencyProperty TextFontSizeProperty;
		public static readonly DependencyProperty TextForegroundProperty;
		public static readonly DependencyProperty TextIsItalicProperty;
		public static readonly DependencyProperty TextIsBoldProperty;
		public static readonly DependencyProperty TextTransparencyProperty;
		public static readonly DependencyProperty ShowBehindProperty;
		public static readonly DependencyProperty IsPageRangeProperty;
		public static readonly DependencyProperty PageRangeProperty;
		static readonly DependencyPropertyKey WatermarkPreviewPropertyKey;
		public static readonly DependencyProperty WatermarkPreviewProperty;
		static WatermarkUITypeEditor() {
			DependencyPropertyRegistrator<WatermarkUITypeEditor>.New()
				.Register(d => d.EditValue, out EditValueProperty, null, d => d.OnEditValueChanged(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.Register(d => d.Report, out ReportProperty, null, d => d.UpdatePreview())
				.RegisterServiceTemplateProperty(d => d.OpenFileDialogServiceTemplate, out OpenFileDialogServiceTemplateProperty, out openFileDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.MessageBoxServiceTemplate, out MessageBoxServiceTemplateProperty, out messageBoxServiceAccessor)
				.RegisterReadOnly(d => d.ImageFileName, out ImageFileNamePropertyKey, out ImageFileNameProperty, null)
				.RegisterReadOnly(d => d.Image, out ImagePropertyKey, out ImageProperty, null, d => d.UpdatePreview())
				.Register(d => d.ImageViewMode, out ImageViewModeProperty, ImageViewMode.Clip, d => d.UpdatePreview())
				.Register(d => d.ImageHorizontalAlignment, out ImageHorizontalAlignmentProperty, HorizontalAlignment.Center, d => d.UpdatePreview())
				.Register(d => d.ImageVerticalAlignment, out ImageVerticalAlignmentProperty, VerticalAlignment.Center, d => d.UpdatePreview())
				.Register(d => d.ImageIsTiled, out ImageIsTiledProperty, false, d => d.UpdatePreview())
				.Register(d => d.ImageTransparency, out ImageTransparencyProperty, 0.0, d => d.UpdatePreview())
				.Register(d => d.Text, out TextProperty, null, d => d.UpdatePreview())
				.Register(d => d.TextDirection, out TextDirectionProperty, DirectionMode.ForwardDiagonal, d => d.UpdatePreview())
				.Register(d => d.TextFontFamily, out TextFontFamilyProperty, null, d => d.UpdatePreview())
				.Register(d => d.TextFontSize, out TextFontSizeProperty, 12, d => d.UpdatePreview())
				.Register(d => d.TextForeground, out TextForegroundProperty, Colors.Red, d => d.UpdatePreview())
				.Register(d => d.TextIsItalic, out TextIsItalicProperty, false, d => d.UpdatePreview())
				.Register(d => d.TextIsBold, out TextIsBoldProperty, false, d => d.UpdatePreview())
				.Register(d => d.TextTransparency, out TextTransparencyProperty, 20.0, d => d.UpdatePreview())
				.Register(d => d.ShowBehind, out ShowBehindProperty, false, d => d.UpdatePreview())
				.Register(d => d.IsPageRange, out IsPageRangeProperty, false, d => d.UpdatePreview())
				.Register(d => d.PageRange, out PageRangeProperty, null, d => d.UpdatePreview())
				.RegisterReadOnly(d => d.WatermarkPreview, out WatermarkPreviewPropertyKey, out WatermarkPreviewProperty, null)
				.OverrideDefaultStyleKey();
		}
		public SelectionModel<XRWatermark> EditValue {
			get { return (SelectionModel<XRWatermark>)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public XtraReport Report {
			get { return (XtraReport)GetValue(ReportProperty); }
			set { SetValue(ReportProperty, value); }
		}
		protected void DoWithOpenFileDialogService(Action<IOpenFileDialogService> action) { openFileDialogServiceAccessor(this, action); }
		public DataTemplate OpenFileDialogServiceTemplate {
			get { return (DataTemplate)GetValue(OpenFileDialogServiceTemplateProperty); }
			set { SetValue(OpenFileDialogServiceTemplateProperty, value); }
		}
		protected void DoWithMessageBoxService(Action<IMessageBoxService> action) { messageBoxServiceAccessor(this, action); }
		public DataTemplate MessageBoxServiceTemplate {
			get { return (DataTemplate)GetValue(MessageBoxServiceTemplateProperty); }
			set { SetValue(MessageBoxServiceTemplateProperty, value); }
		}
		DelegateCommand openImageCommand;
		public ICommand OpenImageCommand {
			get {
				if(openImageCommand == null)
					openImageCommand = new DelegateCommand(OpenImage, false);
				return openImageCommand;
			}
		}
		void OpenImage() {
			DoWithOpenFileDialogService(dialog => {
				dialog.Filter = "Image files (*.jpg,*.png)|*.jpg;*.png";
				var dialogResult = dialog.ShowDialog(e => {
					try {
						System.Drawing.Image.FromFile(Path.Combine(dialog.File.DirectoryName, dialog.File.Name));
					} catch(OutOfMemoryException) {
						e.Cancel = true;
						DoWithMessageBoxService(x => x.ShowMessage(PrintingLocalizer.GetString(PrintingStringId.WatermarkImageLoadError), PrintingLocalizer.GetString(PrintingStringId.Error), MessageButton.OK, MessageIcon.Exclamation));
					}
				});
				if(dialogResult) {
					ImageFileName = dialog.File.Name;
					Image = System.Drawing.Image.FromFile(Path.Combine(dialog.File.DirectoryName, dialog.File.Name));
				}
			});
		}
		public string ImageFileName {
			get { return (string)GetValue(ImageFileNameProperty); }
			private set { SetValue(ImageFileNamePropertyKey, value); }
		}
		public System.Drawing.Image Image {
			get { return (System.Drawing.Image)GetValue(ImageProperty); }
			private set { SetValue(ImagePropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageViewMode ImageViewMode {
			get { return (ImageViewMode)GetValue(ImageViewModeProperty); }
			set { SetValue(ImageViewModeProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public HorizontalAlignment ImageHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(ImageHorizontalAlignmentProperty); }
			set { SetValue(ImageHorizontalAlignmentProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public VerticalAlignment ImageVerticalAlignment {
			get { return (VerticalAlignment)GetValue(ImageVerticalAlignmentProperty); }
			set { SetValue(ImageVerticalAlignmentProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ImageIsTiled {
			get { return (bool)GetValue(ImageIsTiledProperty); }
			set { SetValue(ImageIsTiledProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public double ImageTransparency {
			get { return (double)GetValue(ImageTransparencyProperty); }
			set { SetValue(ImageTransparencyProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DirectionMode TextDirection {
			get { return (DirectionMode)GetValue(TextDirectionProperty); }
			set { SetValue(TextDirectionProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string TextFontFamily {
			get { return (string)GetValue(TextFontFamilyProperty); }
			set { SetValue(TextFontFamilyProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int TextFontSize {
			get { return (int)GetValue(TextFontSizeProperty); }
			set { SetValue(TextFontSizeProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Color TextForeground {
			get { return (Color)GetValue(TextForegroundProperty); }
			set { SetValue(TextForegroundProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool TextIsItalic {
			get { return (bool)GetValue(TextIsItalicProperty); }
			set { SetValue(TextIsItalicProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool TextIsBold {
			get { return (bool)GetValue(TextIsBoldProperty); }
			set { SetValue(TextIsBoldProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public double TextTransparency {
			get { return (double)GetValue(TextTransparencyProperty); }
			set { SetValue(TextTransparencyProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowBehind {
			get { return (bool)GetValue(ShowBehindProperty); }
			set { SetValue(ShowBehindProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsPageRange {
			get { return (bool)GetValue(IsPageRangeProperty); }
			set { SetValue(IsPageRangeProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string PageRange {
			get { return (string)GetValue(PageRangeProperty); }
			set { SetValue(PageRangeProperty, value); }
		}
		public FrameworkElement WatermarkPreview {
			get { return (FrameworkElement)GetValue(WatermarkPreviewProperty); }
			private set { SetValue(WatermarkPreviewPropertyKey, value); }
		}
		DelegateCommand clearAllCommad;
		public ICommand ClearAllCommand {
			get {
				if(clearAllCommad == null)
					clearAllCommad = new DelegateCommand(ResetProperties, false);
				return clearAllCommad;
			}
		}
		DelegateCommand saveCommand;
		public ICommand SaveCommand {
			get {
				if(saveCommand == null)
					saveCommand = new DelegateCommand(Save, false);
				return saveCommand;
			}
		}
		readonly Lazy<IEnumerable<BooleanViewModel>> pageRanges = BooleanViewModel.CreateList("All", "Pages", false);
		public IEnumerable<BooleanViewModel> PageRanges { get { return pageRanges.Value; } }
		readonly Lazy<IEnumerable<BooleanViewModel>> positions = BooleanViewModel.CreateList("In front", "Behind", false);
		public IEnumerable<BooleanViewModel> Positions { get { return positions.Value; } }
		void Save() {
			if(EditValue == null) return;
			EditValue.SetPropertyValue(x => x.Text, Text);
			EditValue.SetPropertyValue(x => x.Font, TextFont);
			EditValue.SetPropertyValue(x => x.TextDirection, TextDirection);
			EditValue.SetPropertyValue(x => x.ImageViewMode, ImageViewMode);
			EditValue.SetPropertyValue(x => x.ImageTiling, ImageIsTiled);
			EditValue.SetPropertyValue(x => x.ImageAlign, ImageAlignment);
			EditValue.SetPropertyValue(x => x.TextTransparency, (int)TextTransparency);
			EditValue.SetPropertyValue(x => x.ImageTransparency, (int)ImageTransparency);
			EditValue.SetPropertyValue(x => x.ShowBehind, ShowBehind);
			EditValue.SetPropertyValue(x => x.PageRange, IsPageRange ? PageRange : string.Empty);
			var properties = PropertyDescriptorHelper.GetPropertyDescriptors(EditValue);
			properties[ExpressionHelper.GetPropertyName((Watermark x) => x.ForeColor)].SetValue(EditValue, TextForeground);
			properties[ExpressionHelper.GetPropertyName((Watermark x) => x.Image)].SetValue(EditValue, Image.With(WinImageHelper.GetImageSource));
		}
		void ResetProperties() {
			updatePreviewLock.DoLockedAction(() => {
				ClearValue(TextProperty);
				ClearValue(TextFontFamilyProperty);
				ClearValue(TextFontSizeProperty);
				ClearValue(TextIsBoldProperty);
				ClearValue(TextIsItalicProperty);
				ClearValue(TextDirectionProperty);
				ClearValue(ImageViewModeProperty);
				ClearValue(ImageIsTiledProperty);
				ClearValue(ImageHorizontalAlignmentProperty);
				ClearValue(ImageVerticalAlignmentProperty);
				ClearValue(TextTransparencyProperty);
				ClearValue(ImageTransparencyProperty);
				ClearValue(ShowBehindProperty);
				ClearValue(PageRangeProperty);
				ClearValue(IsPageRangeProperty);
				ClearValue(TextForegroundProperty);
				ClearValue(ImagePropertyKey);
			});
			UpdatePreview();
		}
		void OnEditValueChanged() {
			if(EditValue == null) {
				ResetProperties();
				return;
			}
			updatePreviewLock.DoLockedAction(() => {
				Text = EditValue.GetPropertyValue(x => x.Text);
				TextFont = EditValue.GetPropertyValue(x => x.Font);
				TextDirection = EditValue.Return(value => value.GetPropertyValue(x => x.TextDirection), () => DirectionMode.Horizontal);
				ImageViewMode = EditValue.Return(value => value.GetPropertyValue(x => x.ImageViewMode), () => ImageViewMode.Clip);
				ImageIsTiled = EditValue.Return(value => value.GetPropertyValue(x => x.ImageTiling), () => false);
				ImageAlignment = EditValue.Return(value => value.GetPropertyValue(x => x.ImageAlign), () => System.Drawing.ContentAlignment.TopLeft);
				TextTransparency = EditValue.Return(value => value.GetPropertyValue(x => x.TextTransparency), () => 20.0);
				ImageTransparency = EditValue.Return(value => value.GetPropertyValue(x => x.ImageTransparency), () => 0.0);
				ShowBehind = EditValue.Return(value => value.GetPropertyValue(x => x.ShowBehind), () => false);
				PageRange = EditValue.GetPropertyValue(x => x.PageRange);
				IsPageRange = !string.IsNullOrEmpty(PageRange);
				var properties = PropertyDescriptorHelper.GetPropertyDescriptors(EditValue);
				TextForeground = (Color)properties[ExpressionHelper.GetPropertyName((Watermark x) => x.ForeColor)].GetValue(EditValue);
				Image = ((ImageSource)properties[ExpressionHelper.GetPropertyName((Watermark x) => x.Image)].GetValue(EditValue)).With(WinImageHelper.GetImage);
			});
			UpdatePreview();
		}
		readonly Locker updatePreviewLock = new Locker();
		void UpdatePreview() {
			updatePreviewLock.DoIfNotLocked(() => {
				var watermark = new Watermark();
				watermark.Text = Text;
				watermark.Image = Image;
				watermark.ImageAlign = ImageAlignment;
				watermark.ImageViewMode = ImageViewMode;
				watermark.ImageTiling = ImageIsTiled;
				watermark.ImageTransparency = (int)ImageTransparency;
				watermark.TextTransparency = (int)TextTransparency;
				watermark.TextDirection = TextDirection;
				watermark.ForeColor = TextColor;
				watermark.ShowBehind = ShowBehind;
				watermark.PageRange = IsPageRange ? PageRange : string.Empty;
				watermark.Font = TextFont;
				WatermarkPreview = Report.With(x => x.GetReportWatermarkPreview(watermark));
			});
		}
		System.Drawing.ContentAlignment ImageAlignment {
			get { return ContentAlignmentHelper.ContentAlignmentFromAlignments(ImageHorizontalAlignment, ImageVerticalAlignment); }
			set {
				ImageHorizontalAlignment = ContentAlignmentHelper.HorizontalAlignmentFromContentAlignment(value);
				ImageVerticalAlignment = ContentAlignmentHelper.VerticalAlignmentFromContentAlignment(value);
			}
		}
		System.Drawing.Color TextColor { get { return System.Drawing.Color.FromArgb(TextForeground.A, TextForeground.R, TextForeground.G, TextForeground.B); } }
		System.Drawing.Font TextFont {
			get {
				var fontStyle = System.Drawing.FontStyle.Regular;
				fontStyle = WinFontHelper.ConvertToFontStyle(TextIsBold, System.Drawing.FontStyle.Bold, fontStyle);
				fontStyle = WinFontHelper.ConvertToFontStyle(TextIsItalic, System.Drawing.FontStyle.Italic, fontStyle);
				return new System.Drawing.Font(TextFontFamily, TextFontSize, fontStyle, System.Drawing.GraphicsUnit.Point);
			}
			set {
				TextFontFamily = value.With(x => x.Name);
				TextFontSize = value.Return(x => (int)x.Size, () => 12);
				TextIsBold = value.Return(x => x.Style.IsBold(), () => false);
				TextIsItalic = value.Return(x => x.Style.IsItalic(), () => false);
			}
		}
	}
}
