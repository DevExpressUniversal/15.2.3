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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using FrameworkContentElement = System.Windows.FrameworkElement;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PlatformIndependentColor = System.Windows.Media.Color;
#else
using DevExpress.Xpf.Utils;
using PlatformIndependentColor = System.Drawing.Color;
#endif
namespace DevExpress.Xpf.Office.UI {
	#region StringDelimitedWithCRLFAndTabsToPartConverter (abstract class)
	public abstract class StringDelimitedWithCRLFAndTabsToPartConverter : IValueConverter {
		protected string GetSubstring(string entireString, int y, int x) {
			if (String.IsNullOrEmpty(entireString))
				return String.Empty;
			entireString = entireString.Replace("\t\t", "\t");
			entireString = entireString.Replace("\r\n", "\n");
			string[] lines = entireString.Split('\n');
			if (y >= lines.Length)
				return String.Empty;
			string[] columns = lines[y].Split('\t');
			if (x >= columns.Length)
				return String.Empty;
			return columns[x];
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType != typeof(string))
				return value;
			string result = GetSubstring(value as string, Y, X);
			return String.IsNullOrEmpty(result) ? value : result;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		protected abstract int X { get; }
		protected abstract int Y { get; }
	}
	#endregion
	#region MarginsStringToNameConverter
	public class MarginsStringToNameConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 0; } }
		protected override int Y { get { return 0; } }
	}
	#endregion
	#region MarginsStringToTopCaptionConverter
	public class MarginsStringToTopCaptionConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 0; } }
		protected override int Y { get { return 1; } }
	}
	#endregion
	#region MarginsStringToTopValueConverter
	public class MarginsStringToTopValueConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 1; } }
		protected override int Y { get { return 1; } }
	}
	#endregion
	#region MarginsStringToBottomCaptionConverter
	public class MarginsStringToBottomCaptionConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 2; } }
		protected override int Y { get { return 1; } }
	}
	#endregion
	#region MarginsStringToBottomValueConverter
	public class MarginsStringToBottomValueConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 3; } }
		protected override int Y { get { return 1; } }
	}
	#endregion
	#region MarginsStringToLeftCaptionConverter
	public class MarginsStringToLeftCaptionConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 0; } }
		protected override int Y { get { return 2; } }
	}
	#endregion
	#region MarginsStringToLeftValueConverter
	public class MarginsStringToLeftValueConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 1; } }
		protected override int Y { get { return 2; } }
	}
	#endregion
	#region MarginsStringToRightCaptionConverter
	public class MarginsStringToRightCaptionConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 2; } }
		protected override int Y { get { return 2; } }
	}
	#endregion
	#region MarginsStringToRightValueConverter
	public class MarginsStringToRightValueConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 3; } }
		protected override int Y { get { return 2; } }
	}
	#endregion
	#region PaperKindStringToCaptionConverter
	public class PaperKindStringToCaptionConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 0; } }
		protected override int Y { get { return 0; } }
	}
	#endregion
	#region PaperKindStringToValueConverter
	public class PaperKindStringToValueConverter : StringDelimitedWithCRLFAndTabsToPartConverter {
		protected override int X { get { return 0; } }
		protected override int Y { get { return 1; } }
	}
	#endregion
	public class GalleryStyleItemForeColorConverter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType != typeof(System.Windows.Media.Brush))
				return value;
			System.Windows.Media.Color foreColor = (System.Windows.Media.Color)value;
			return new SolidColorBrush(foreColor);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return ((SolidColorBrush)value).Color;
		}
	}
	#region OfficeDefaultBarItemDataTemplates
	public class OfficeDefaultBarItemDataTemplates : Control {
		public static readonly DependencyProperty MarginBarItemContentTemplateProperty = DependencyPropertyManager.Register("MarginBarItemContentTemplate", typeof(DataTemplate), typeof(OfficeDefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public DataTemplate MarginBarItemContentTemplate {
			get { return (DataTemplate)GetValue(MarginBarItemContentTemplateProperty); }
			set { SetValue(MarginBarItemContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty PaperKindBarItemContentTemplateProperty = DependencyPropertyManager.Register("PaperKindBarItemContentTemplate", typeof(DataTemplate), typeof(OfficeDefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public DataTemplate PaperKindBarItemContentTemplate {
			get { return (DataTemplate)GetValue(PaperKindBarItemContentTemplateProperty); }
			set { SetValue(PaperKindBarItemContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty CheckEditTemplateProperty = DependencyPropertyManager.Register("CheckEditTemplate", typeof(DataTemplate), typeof(OfficeDefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public DataTemplate CheckEditTemplate {
			get { return (DataTemplate)GetValue(CheckEditTemplateProperty); }
			set { SetValue(CheckEditTemplateProperty, value); }
		}
		public OfficeDefaultBarItemDataTemplates() {
			this.DefaultStyleKey = typeof(OfficeDefaultBarItemDataTemplates);
		}
	}
	#endregion
}
