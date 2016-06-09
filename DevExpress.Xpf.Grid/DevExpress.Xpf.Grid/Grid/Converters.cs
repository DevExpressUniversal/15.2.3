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

using System.Windows.Data;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System;
using System.Globalization;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
using System.Windows.Controls;
using System.ComponentModel;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class GroupDetailNameConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value == null)
				return null;
			return string.Format(GridControlLocalizer.GetString(GridControlStringId.GroupPanelDisplayFormatStringForMasterDetail), value.ToString());
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DetailLevelToObjectConverter : IValueConverter {
		public object NotDetailValue { get; set; }
		public object DetailValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			object result = value is int && (int)value > 0 ? DetailValue : NotDetailValue;
			return TypeDescriptor.GetConverter(targetType).ConvertFrom(null, CultureInfo.InvariantCulture, result);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DetailMarginVisibilityConverter : IValueConverter {
		internal static Visibility GetDetailMarginControlVisibility(IList<DetailIndent> detailIndents, Side marginSide) {
			if(detailIndents == null)
				return Visibility.Collapsed;
			double width = 0;
			switch(marginSide) {
				case Side.Left:
					foreach(var indent in detailIndents)
						width += indent.Width;
					return width > 0 ? Visibility.Visible : Visibility.Collapsed;
				case Side.Right:
					foreach(var indent in detailIndents)
						width += indent.WidthRight;
					return width > 0 ? Visibility.Visible : Visibility.Collapsed;
				default:
					return Visibility.Collapsed;
			}
		}
		public Side MarginSide { get; set; }
		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return GetDetailMarginControlVisibility(value as IList<DetailIndent>, MarginSide);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	class SelectionModeHelper {
		public static MultiSelectMode ConvertToMultiSelectMode(TableViewSelectMode selectMode) {
			switch(selectMode) {
				case TableViewSelectMode.Row:
					return MultiSelectMode.Row;
				case TableViewSelectMode.Cell:
					return MultiSelectMode.Cell;
				default:
					return MultiSelectMode.None;
			}
		}
#if !SL
		public static MultiSelectMode ConvertToMultiSelectMode(CardViewSelectMode selectMode) {
			switch(selectMode) {
				case CardViewSelectMode.Row:
					return MultiSelectMode.Row;
				default:
					return MultiSelectMode.None;
			}
		}
#endif
	}
	public class FootersPanelMarginConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(!(value is double))
				return new Thickness();
			return new Thickness(0, 0, -(double)value, 0);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DoubleToVisibilityConverter : IValueConverter {
		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value is double)
				return (double)value == 0 ? Visibility.Collapsed : Visibility.Visible;
			return Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DoubleToVisibilityInverseConverter : DoubleToVisibilityConverter {
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (Visibility)base.Convert(value, targetType, parameter, culture) == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
		}
	}
	public class NavigationRowConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(values == null || values.Length == 0 || values[0] == null)
				return null;
			DataViewBase view = values[0] as DataViewBase;
			if(view != null && view.DataControl != null) {
				LocalizationDescriptor descriptor = view.LocalizationDescriptor;
				string format = descriptor.GetValue("NavigationRecord");
				int all = view.DataControl.VisibleRowCount;
				int rowHandle = view.FocusedRowHandle;
				if(rowHandle == DataControlBase.InvalidRowHandle || rowHandle == DataControlBase.AutoFilterRowHandle)
					return string.Format(format, 0, all);
				if(rowHandle == DataControlBase.NewItemRowHandle) {
					if(!(view is TableView))
						return string.Format(format, 0, all);
					if(((TableView)view).NewItemRowPosition != NewItemRowPosition.Bottom)
						return string.Format(format, 0, all);
				}
				int rowVisibleIndex = view.DataControl.GetRowVisibleIndexByHandleCore(rowHandle);
				return string.Format(format, rowVisibleIndex + 1, all);
			}
			return null;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DisplayedNavigationConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			NavigatorButtonType typeButton = (NavigatorButtonType)Enum.Parse(typeof(NavigatorButtonType), parameter.ToString());
			NavigatorButtonType displayedButtons = (NavigatorButtonType)value;	 
			switch(typeButton) {			  
				case NavigatorButtonType.AddNewRow:
					return (displayedButtons & NavigatorButtonType.AddNewRow) == NavigatorButtonType.AddNewRow ? Visibility.Visible : Visibility.Collapsed;
				case NavigatorButtonType.DeleteFocusedRow:
					return (displayedButtons & NavigatorButtonType.DeleteFocusedRow) == NavigatorButtonType.DeleteFocusedRow ? Visibility.Visible : Visibility.Collapsed;
				case NavigatorButtonType.EditFocusedRow:
					return (displayedButtons & NavigatorButtonType.EditFocusedRow) == NavigatorButtonType.EditFocusedRow ? Visibility.Visible : Visibility.Collapsed;
				case NavigatorButtonType.MoveFirstRow:
					return (displayedButtons & NavigatorButtonType.MoveFirstRow) == NavigatorButtonType.MoveFirstRow ? Visibility.Visible : Visibility.Collapsed;
				case NavigatorButtonType.MoveLastRow:
					return (displayedButtons & NavigatorButtonType.MoveLastRow) == NavigatorButtonType.MoveLastRow ? Visibility.Visible : Visibility.Collapsed;
				case NavigatorButtonType.MoveNextPage:
					return (displayedButtons & NavigatorButtonType.MoveNextPage) == NavigatorButtonType.MoveNextPage ? Visibility.Visible : Visibility.Collapsed;
				case NavigatorButtonType.MoveNextRow:
					return (displayedButtons & NavigatorButtonType.MoveNextRow) == NavigatorButtonType.MoveNextRow ? Visibility.Visible : Visibility.Collapsed;
				case NavigatorButtonType.MovePrevPage:
					return (displayedButtons & NavigatorButtonType.MovePrevPage) == NavigatorButtonType.MovePrevPage ? Visibility.Visible : Visibility.Collapsed;
				case NavigatorButtonType.MovePrevRow:
					return (displayedButtons & NavigatorButtonType.MovePrevRow) == NavigatorButtonType.MovePrevRow ? Visibility.Visible : Visibility.Collapsed;
				default:
					throw new NotImplementedException();
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
#if !SL
	public class CardViewTopContrlsIsVisibileToRowSpanCoverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is bool) || !(values[1] is bool))
				return 4;
			return (bool)values[0] || (bool)values[1] ? 4 : 2;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalScrollBarGridColumnSpanConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is bool) || !(values[1] is double) || !(values[2] is ScrollBarMode) || !(values[3] is bool))
				return 1;
			if((bool)values[0] && (((double)values[1] == 0) ||(bool)values[3]))
				return 2;
			if((ScrollBarMode)values[2] == ScrollBarMode.TouchOverlap && (double)values[1] != 0)
				return 2;
			return 1;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class FitContentContainerConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is ScrollBarMode) || !(values[1] is bool) || !(values[2] is double) || !(values[3] is bool) || !(values[4] is Visibility))
				return Visibility.Visible;
			return ConvertCore((ScrollBarMode)values[0], (bool)values[1], (double)values[2], (bool)values[3], (Visibility)values[4]);
		}
		Visibility ConvertCore(ScrollBarMode scrollBarMode, bool showTotalSummary, double fixedRightContentWidth, bool extendScrollBarToFixedColumns, Visibility comutedVerticalScrollBarVisibility) {
			if(showTotalSummary && (fixedRightContentWidth == 0 || extendScrollBarToFixedColumns))
				return Visibility.Collapsed;
			return comutedVerticalScrollBarVisibility;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class ScrollCornerCardViewConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is ScrollBarMode) || !(values[1] is Visibility))
				return Visibility.Visible;
			return (ScrollBarMode)values[0] == ScrollBarMode.TouchOverlap ? Visibility.Collapsed : values[1];
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class VerticalScrollBarRowSpanConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is ScrollBarMode) || !(values[1] is bool))
				return 2;
			if((bool)values[1])
				return 2;
			return (ScrollBarMode)values[0] == ScrollBarMode.TouchOverlap ? 4 : 2;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class ScrollViewerContentRowSpanConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is ScrollBarMode) || !(values[1] is bool))
				return 1;
			if((bool)values[1])
				return 1;
			return (ScrollBarMode)values[0] == ScrollBarMode.TouchOverlap ? 3 : 1;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalScrollBarMarginConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is bool) || !(values[1] is double) || !(values[2] is Thickness) || !(values[3] is bool))
				return new Thickness();
			Thickness themeMargin = (Thickness)values[2];
			if((bool)values[3] || !(bool)values[0])
				return themeMargin;
			return new Thickness(themeMargin.Left + (double)values[1], themeMargin.Top, themeMargin.Right, themeMargin.Bottom);
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalScrollBarLeftCornerBorderThicknessConverter : IMultiValueConverter {
		public bool UseVerticalBorder { get; set; }
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is bool) || !(values[1] is double) || !(values[2] is bool) || !(values[3] is bool))
				return new Thickness();
			return new Thickness(0, UseVerticalBorder && !(bool)values[0] ? 1 : 0, (UseVerticalBorder || !(bool)values[0]) && ((bool)values[2] || (double)values[1] == 0 || (bool)values[3]) ? 1 : 0, 0);
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalScrollBarIndicatorVisibilityConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is bool) || !(values[1] is bool) || !(values[2] is double) || !(values[3] is bool) || !(values[4] is bool)|| !(values[5] is ScrollBarMode))
				return Visibility.Collapsed;
			if(!(bool)values[0])
				return Visibility.Collapsed;
			if(!(bool)values[1])
				return Visibility.Visible;
			return !((bool)values[3]) && (double)values[2] != 0 && !((bool)values[4] && (ScrollBarMode)values[5] != ScrollBarMode.TouchOverlap) ? Visibility.Visible : Visibility.Collapsed;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalScrollBarFixedLeftThumbVisibilityConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is bool) || !(values[1] is double) || !(values[2] is bool) || !((values[3]) is bool) || !((values[4]) is ScrollBarMode))
				return Visibility.Collapsed;
			if((ScrollBarMode)values[4] == ScrollBarMode.Standard && (bool)values[3])
				return Visibility.Collapsed;
			return !(bool)values[2] && (double)values[1] != 0 ? Visibility.Visible : Visibility.Collapsed;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalScrollBarFixedLeftThumbWidthConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is double) || !(values[1] is double) || !(values[2] is bool) || !(values[3] is double))
				return 0d;
			return (double)values[0] + (double)values[1] + ((bool)values[2] ? (double)values[3] : 0);
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalScrollBarRowConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is ScrollBarMode) || !(values[1] is bool))
				return 3;
			if((bool)values[1]) {
				return (ScrollBarMode)values[0] == ScrollBarMode.TouchOverlap ? 2 : 3;
			}
			else {
				return (ScrollBarMode)values[0] == ScrollBarMode.TouchOverlap ? 1 : 3;
			}
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalScrollBarRowSpanConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is ScrollBarMode) || !(values[1] is bool))
				return 3;
			if((bool)values[1]) {
				return (ScrollBarMode)values[0] == ScrollBarMode.TouchOverlap ? 2 : 1;
			}
			else {
				return (ScrollBarMode)values[0] == ScrollBarMode.TouchOverlap ? 3 : 1;
			}
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalScrollBarColumnSpanConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is ScrollBarMode) || !(values[1] is Orientation?))
				return 2;
			if((ScrollBarMode)values[0] != ScrollBarMode.TouchOverlap || values[1] == null)
				return 2;
			return ((Orientation?)values[1]).Value == Orientation.Horizontal ? 2 : 1;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class VerticalScrollBarRowSpanCardViewConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[0] is ScrollBarMode) || !(values[1] is Orientation?))
				return 1;
			if((ScrollBarMode)values[0] != ScrollBarMode.TouchOverlap || values[1] == null)
				return 1;
			return ((Orientation?)values[1]).Value == Orientation.Vertical ? 2 : 1;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class TotalSummaryMarginConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return new Thickness((double)value, 0, 0, 0);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class InverseBooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {		   
			return !(bool)value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return !(bool)value;
		}
	}
	public class AddNewRowVisibleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value is TableView)
				return Visibility.Visible;
			return Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return !(bool)value;
		}
	}
	public class NavigationStyleToBoolean : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value is GridViewNavigationStyle && (GridViewNavigationStyle)value == GridViewNavigationStyle.None)
				return false;
			return true;		 
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
#endif
}
