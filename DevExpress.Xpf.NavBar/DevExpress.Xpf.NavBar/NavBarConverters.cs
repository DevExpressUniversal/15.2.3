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
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.NavBar {
	public class SourceToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (parameter.ToString() == "Visible")
				return value == null ? Visibility.Visible : Visibility.Collapsed;
			return value == null ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class EnabledToOpacityConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (bool)value ? 1 : 0.35;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class BooleanToOpacityConverter : MarkupExtension, IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (bool)value ? 1.0 : 0.0;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class OrientationToCursorConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (Orientation)value == Orientation.Vertical ? Cursors.SizeNS : Cursors.SizeWE;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class NavBarViewKindToBooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (NavBarViewKind)value == NavBarViewKind.NavigationPane ? true : false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();			
		}
	}
	public class NavBarViewKindToScrollBarVisibilityConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (NavBarViewKind)value == NavBarViewKind.ExplorerBar ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class NavBarViewKindToObjectConverter : IValueConverter {
		public object ExplorerBarValue { get; set; }
		public object NavigationPaneValue { get; set; }
		public object SideBarValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value is NavBarViewKind) {
				NavBarViewKind kind = (NavBarViewKind)value;
				switch(kind) {
					case NavBarViewKind.ExplorerBar:
						return ExplorerBarValue;
					case NavBarViewKind.NavigationPane:
						return NavigationPaneValue;
					case NavBarViewKind.SideBar:
						return SideBarValue;
				}
			} return null;			
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DoubleInvertConverter : IValueConverter {
		public double MaxValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return ConvertCore(value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return ConvertCore(value);
		}
		private object ConvertCore(object value) {
			if(value is double) {
				double dVal = ((double)value);
				return MaxValue - dVal;
			}
			return -1;
		}
	}
	public class NavBarViewToAllowScrollingConverter : IValueConverter
	,IMultiValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value is NavigationPaneView) return true;
			if(value is ExplorerBarView) {
				ExplorerBarView view = value as ExplorerBarView;
				return view.Orientation != view.ItemsPanelOrientation;
			}
			return false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(values[0] is NavigationPaneView) return true;
			if(values[0] is ExplorerBarView) {
				ExplorerBarView view = values[0] as ExplorerBarView;
				return view.Orientation != view.ItemsPanelOrientation;
			}
			return false;
		}
		object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DoubleDivisionConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { return ((double)value) / ((double)parameter); }
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { return ((double)value) * ((double)parameter); }
	}
	public class FrameworkElementInfoSLCompatibilityConverterExtension : MarkupExtension, IValueConverter {
		public bool ConvertToInfo { get; set; }
		public bool IgnoreWrongValue { get; set; }
		public FrameworkElementInfoSLCompatibilityConverterExtension() {
			ConvertToInfo = true;
			IgnoreWrongValue = false;
		}
		public object Convert(object value) {
			return Convert(value, typeof(object), null, null);
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		public object ConvertBack(object value) {
			return ConvertBack(value, typeof(object), null, null);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class NavBarGroupCollectionConverterExtension : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class NavBarItemToSourceObjectConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (value as NavBarItem).With(x => x.SourceObject) ?? value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class NavBarItemToSourceObjectConverterExtension : MarkupExtension {
		static Lazy<NavBarItemToSourceObjectConverter> default_;
		protected static NavBarItemToSourceObjectConverter Default { get { return default_.Value; } }
		static NavBarItemToSourceObjectConverterExtension() {
			default_ = new Lazy<NavBarItemToSourceObjectConverter>(() => new NavBarItemToSourceObjectConverter(), true);
		}
		public NavBarItemToSourceObjectConverterExtension() { }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return Default;
		}
	}
}
