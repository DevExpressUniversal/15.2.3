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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Reports.UserDesigner.Editors.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System.Collections;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Diagram;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using System.ComponentModel;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class FormattingRuleSheetUITypeEditor : SingleSelectionCollectionEditor {
		public static readonly DependencyProperty ReportModelProperty;
		static FormattingRuleSheetUITypeEditor() {
			DependencyPropertyRegistrator<FormattingRuleSheetUITypeEditor>.New()
				.Register(d => d.ReportModel, out ReportModelProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		public FormattingRuleSheetUITypeEditor() {
			clearAllFormattingRulesCommand = DelegateCommandFactory.Create(RemoveAllItems, CanExecuteRemoveItemCommand, true);
			clearUnusedFormattingRulesCommand = DelegateCommandFactory.Create(ClearUnusedFormattingRules);
		}
		readonly ICommand clearAllFormattingRulesCommand;
		public ICommand ClearAllFormattingRulesCommand { get { return clearAllFormattingRulesCommand; } }
		readonly ICommand clearUnusedFormattingRulesCommand;
		public ICommand ClearUnusedFormattingRulesCommand { get { return clearUnusedFormattingRulesCommand; } }
		public XtraReportModelBase ReportModel {
			get { return (XtraReportModelBase)GetValue(ReportModelProperty); }
			set { SetValue(ReportModelProperty, value); }
		}
		void ClearUnusedFormattingRules() {
			DoWithEditorItems(x => {
				var formattingRule = (FormattingRule)XRModelBase.GetXRModel(SelectionModelHelper<IDiagramItem, DiagramItem>.GetUnderlyingItem((IMultiModel)Items[x])).XRObject;
				if(!ReportModel.XRObject.IsAttachedRule(formattingRule))
					controller.RemoveAt(x);
			});
		}
		public override object CreateItem() {
			return (XRFormattingRuleDiagramItem)ReportModel.DiagramItem.Diagram.ItemFactory(new FormattingRule());
		}
		public override Func<IMultiModel, bool> IsEditorItem { get { return item => SelectionModelHelper<IDiagramItem, DiagramItem>.GetUnderlyingItem(item) is XRFormattingRuleDiagramItem; } }
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class DrawingColorToMediaBrushConverter : IValueConverter {
		public string ItemType { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			System.Drawing.Color color = (System.Drawing.Color)value;
			return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			Color color = ((SolidColorBrush)value).Color;
			return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
	}
	public class PaddingInfoToThicknessConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var paddingInfo = (PaddingInfo)value;
			return new Thickness((double)paddingInfo.Left, (double)paddingInfo.Top, (double)paddingInfo.Right, (double)paddingInfo.Bottom);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			var thickness = (Thickness)value;
			return new PaddingInfo((int)thickness.Left, (int)thickness.Right, (int)thickness.Top, (int)thickness.Bottom);
		}
	}
	public class TextAlignmentToHorizontalAlignmentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var textAlignment = (XtraPrinting.TextAlignment)value;
			var right = XtraPrinting.TextAlignment.BottomRight | XtraPrinting.TextAlignment.MiddleRight | XtraPrinting.TextAlignment.TopRight;
			var center = XtraPrinting.TextAlignment.BottomCenter | XtraPrinting.TextAlignment.MiddleCenter | XtraPrinting.TextAlignment.TopCenter;
			var justify = XtraPrinting.TextAlignment.BottomJustify | XtraPrinting.TextAlignment.MiddleJustify | XtraPrinting.TextAlignment.TopJustify;
			if((textAlignment & right) != 0)
				return System.Windows.HorizontalAlignment.Right;
			if((textAlignment & center) != 0)
				return System.Windows.HorizontalAlignment.Center;
			if((textAlignment & justify) != 0)
				return System.Windows.HorizontalAlignment.Stretch;
			return System.Windows.HorizontalAlignment.Left;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new Exception();
		}
	}
	public class TextAlignmentToVerticalAlignmentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var textAlignment = (XtraPrinting.TextAlignment)value;
			var top = XtraPrinting.TextAlignment.TopCenter | XtraPrinting.TextAlignment.TopJustify | XtraPrinting.TextAlignment.TopLeft | XtraPrinting.TextAlignment.TopRight;
			var center = XtraPrinting.TextAlignment.MiddleCenter | XtraPrinting.TextAlignment.MiddleJustify | XtraPrinting.TextAlignment.MiddleLeft | XtraPrinting.TextAlignment.MiddleRight;
			if((textAlignment & top) != 0)
				return System.Windows.VerticalAlignment.Top;
			if((textAlignment & center) != 0)
				return System.Windows.VerticalAlignment.Center;
			return System.Windows.VerticalAlignment.Bottom;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new Exception();
		}
	}
	public class WidthToThicknessConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value.ToString() + "," + value.ToString() + "," + value.ToString() + "," + value.ToString();
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new Exception();
		}
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class DrawingColorToMediaBrushConverterExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new DrawingColorToMediaBrushConverter();
		}
	}
	public class PaddingInfoToThicknessConverterExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new PaddingInfoToThicknessConverter();
		}
	}
	public class TextAlignmentToHorizontalAlignmentConverterExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new TextAlignmentToHorizontalAlignmentConverter();
		}
	}
	public class TextAlignmentToVerticalAlignmentConverterExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new TextAlignmentToVerticalAlignmentConverter();
		}
	}
	public class WidthToThicknessConverterExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new WidthToThicknessConverter();
		}
	}
}
