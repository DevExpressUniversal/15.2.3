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
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Utils;
using System.Windows.Markup;
using DevExpress.Office.Design.Internal;
using DevExpress.Office;
using DevExpress.Office.Layout;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit;
#if SL
using PlatformIndependentColor = System.Windows.Media.Color;
using DevExpress.Xpf.Core.WPFCompatibility;
using FrameworkContentElement = System.Windows.FrameworkElement;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#else
using PlatformIndependentColor = System.Drawing.Color;
using DevExpress.Xpf.Utils;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Layout.TableLayout;
#endif
namespace DevExpress.Xpf.RichEdit.UI {
	#region RichEditBorderLineComboBoxEdit
	[DXToolboxBrowsableAttribute(false)]
	public class RichEditBorderLineComboBoxEdit : ComboBoxEdit {
		static RichEditBorderLineComboBoxEdit() {
			RichEditBorderLineComboBoxEditSettings.RegisterEditor();
		}
		public RichEditBorderLineComboBoxEdit()
			: base() {
			IsTextEditable = false;
			ApplyItemTemplateToSelectedItem = true;
			this.ValueMember = "BorderStyle";
		}
		RichEditBorderLineComboBoxEditSettings InnerSettings { get { return Settings as RichEditBorderLineComboBoxEditSettings; } }
		public IRichEditControl RichEditControl {
			get {
				if (InnerSettings != null)
					return InnerSettings.RichEditControl;
				else
					return null;
			}
			set {
				if (InnerSettings != null)
					InnerSettings.RichEditControl = value;
			}
		}
		protected override BaseEditSettings CreateEditorSettings() {
			return new RichEditBorderLineComboBoxEditSettings();
		}
		public BorderLineStyle Value {
			get {
				object value = base.EditValue;
				if (value != null && value is BorderLineStyle)
					return (BorderLineStyle)value;
				else
					return BorderLineStyle.None;
			}
			set {
				base.EditValue = value;
			}
		}
	}
	#endregion
	#region RichEditBorderLineComboBoxEditSettings
	public class RichEditBorderLineComboBoxEditSettings : ComboBoxEditSettings, IRichEditControlDependencyPropertyOwner {
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(IRichEditControl), typeof(RichEditBorderLineComboBoxEditSettings), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditBorderLineComboBoxEditSettings instance = d as RichEditBorderLineComboBoxEditSettings;
			if (instance != null)
				instance.OnRichEditControlChanged((RichEditControl)e.OldValue, (RichEditControl)e.NewValue);
		}
		public IRichEditControl RichEditControl {
			get { return (IRichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		protected internal virtual void OnRichEditControlChanged(RichEditControl oldValue, RichEditControl newValue) {
			UnsubscribeEvents(oldValue);
			Populate();
			SubscribeEvents(newValue);
		}
		static RichEditBorderLineComboBoxEditSettings() {
			RegisterEditor();
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(RichEditBorderLineComboBoxEdit), typeof(RichEditBorderLineComboBoxEditSettings), delegate() { return new RichEditBorderLineComboBoxEdit(); }, delegate() { return new RichEditBorderLineComboBoxEditSettings(); });
		}
		protected internal virtual void SubscribeEvents(RichEditControl control) {
			if (control == null)
				return;
			control.DocumentLoaded += OnDocumentRecreated;
			control.EmptyDocumentCreated += OnDocumentRecreated;
			control.DocumentModel.DocumentCleared += OnDocumentRecreated;
		}
		protected internal virtual void UnsubscribeEvents(RichEditControl control) {
			if (control == null)
				return;
			control.DocumentLoaded -= OnDocumentRecreated;
			control.EmptyDocumentCreated -= OnDocumentRecreated;
			control.DocumentModel.DocumentCleared -= OnDocumentRecreated;
		}
		void OnDocumentRecreated(object sender, EventArgs e) {
			Populate();
		}
		void Populate() {
			Items.Clear();
			if (RichEditControl != null) {
				DocumentModel documentModel = RichEditControl.InnerControl.DocumentModel;
				List<BorderInfo> borderInfos = documentModel.TableBorderInfoRepository.Items;
				int count = borderInfos.Count;
				for (int i = 0; i < count; i++)
					AddItem(documentModel, borderInfos[i]);
			}
		}
		void AddItem(DocumentModel documentModel, BorderInfo borderInfo) {
			RichEditBorderLineComboBoxEditItem item = new RichEditBorderLineComboBoxEditItem();
			item.BorderInfo = borderInfo;
			item.Content = GenerateItemContent(documentModel, borderInfo);
			if (item.Content != null)
				Items.Add(item);
		}
		protected internal static FrameworkElement GenerateItemContent(DocumentModel documentModel, BorderInfo borderInfo) {
			Grid grid = new Grid() { Margin = new Thickness(3, 0, 3, 0) };
			TextBlock textBlock = new TextBlock();
			textBlock.Text = " ";
			grid.Children.Add(textBlock);
			DocumentLayoutUnitConverter converter = new DocumentLayoutUnitPixelsConverter(documentModel.ScreenDpi);
			XpfSlowPainter painter = new XpfSlowPainter(converter, grid.Children);
			painter.ZoomFactor = 1;
			RichEditPatternLinePainter horizontalLinePainter = new RichEditHorizontalPatternLinePainter(painter, converter);
			RichEditPatternLinePainter verticalLinePainter = new RichEditVerticalPatternLinePainter(painter, converter);
			GraphicsDocumentLayoutExporterTableBorder exporter = new GraphicsDocumentLayoutExporterTableBorder(documentModel, painter, horizontalLinePainter, verticalLinePainter);
			DocumentModelUnitToLayoutUnitConverter toLayoutUnitConverter = documentModel.UnitConverter.CreateConverterToLayoutUnits(DevExpress.Office.DocumentLayoutUnit.Pixel);
			TableBorderPainter borderPainter = exporter.GetBorderPainter(borderInfo, toLayoutUnitConverter);
			if (borderPainter != null)
				borderPainter.DrawHorizontalBorder(borderInfo.Color, new PointF(0, 0), 150);
			else
				return null;
			foreach (UIElement child in grid.Children) {
				FrameworkElement element = child as FrameworkElement;
				if (element != null)
					element.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			}
			return grid;
		}
		#region IRichEditControlDependencyPropertyOwner Members
		DependencyProperty IRichEditControlDependencyPropertyOwner.DependencyProperty { get { return RichEditControlProperty; } }
		#endregion
	}
	#endregion
	#region RichEditBorderLineComboBoxEditItem
	public class RichEditBorderLineComboBoxEditItem : ContentControl {
		BorderInfo borderInfo;
		public BorderInfo BorderInfo { get { return borderInfo; } set { borderInfo = value; } }
		public BorderLineStyle BorderStyle { get { return BorderInfo.Style; } }
	}
	#endregion
	#region RichEditBorderLineWidthEdit
	[DXToolboxBrowsableAttribute(false)]
	public class RichEditBorderLineWidthEdit : ComboBoxEdit {
		static RichEditBorderLineWidthEdit() {
			RichEditBorderLineWidthEditSettings.RegisterEditor();
		}
		public RichEditBorderLineWidthEdit() {
			DefaultStyleKey = typeof(RichEditBorderLineWidthEdit);
		}
	}
	#endregion
	#region RichEditBorderLineWidthEditSettings
	public class RichEditBorderLineWidthEditSettings : ComboBoxEditSettings {
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(RichEditBorderLineWidthEdit), typeof(RichEditBorderLineWidthEditSettings), () => new RichEditBorderLineWidthEdit(), () => new RichEditBorderLineWidthEditSettings());
		}
		public RichEditBorderLineWidthEditSettings() {
			Populate();
		}
		void Populate() {
			Items.Clear();
			Items.Add((double)0.25);
			Items.Add((double)0.5);
			Items.Add((double)0.75);
			Items.Add((double)1.0);
			Items.Add((double)1.5);
			Items.Add((double)2.25);
			Items.Add((double)3.0);
			Items.Add((double)4.5);
			Items.Add((double)6.0);
		}
	}
	#endregion
	#region BorderWidthDisplayTextConverter
	public class BorderWidthDisplayTextConverter : MarkupExtension, IValueConverter {
		BorderWidthDisplayTextConverter instance = null;
		public BorderWidthDisplayTextConverter() {
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null) {
				instance = new BorderWidthDisplayTextConverter();
			}
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (!(value is double))
				return String.Empty;
			double width = (double)value;
			return String.Format("{0}{1}", width, UIUnit.GetTextAbbreviation(DocumentUnit.Point));
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	#endregion
	#region PointsToPixelsConverter
	public class PointsToPixelsConverter : MarkupExtension, IValueConverter {
		PointsToPixelsConverter instance = null;
		public PointsToPixelsConverter() {
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null) {
				instance = new PointsToPixelsConverter();
			}
			return instance;
		}
		#region IValueConverter Members
		[System.Security.SecuritySafeCritical]
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (!(value is double))
				return 0;
			double width = (double)value;
#if !SL
			double dpiy;
			if (Application.Current != null && Application.Current.MainWindow != null) {
				Matrix matrix = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
				dpiy = matrix.M22 * 96;
			}
			else {
				using (var source = new System.Windows.Interop.HwndSource(new System.Windows.Interop.HwndSourceParameters())) {
					Matrix matrix = source.CompositionTarget.TransformToDevice;
					dpiy = matrix.M22 * 96;
				}
			}
#else
			double dpiy = 96;
#endif
			return Math.Max(1, (double)width * dpiy / 72);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	#endregion
}
