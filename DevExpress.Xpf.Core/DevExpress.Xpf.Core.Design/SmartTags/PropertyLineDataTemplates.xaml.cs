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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Globalization;
using DevExpress.Design.SmartTags;
using DevExpress.Design.UI;
#if SL
using DevExpress.Xpf.Core.Design.CoreUtils;
using System.Windows.Input;
#else
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
#endif
namespace DevExpress.Xpf.Core.Design {
	public partial class PropertyLineDataTemplates : ResourceDictionary {
		public PropertyLineDataTemplates() {
			InitializeComponent();
			LoadPlatformResources();
		}
		private void LoadPlatformResources() {
			ResourceDictionary resources = new ResourceDictionary();
			string assemblyName = GetType().Assembly.GetName().Name;
#if SL
			string platformFolder = "SL";
#else
			string platformFolder = "WPF";
#endif
			resources.Source = new Uri(string.Format("/{0};component/SmartTags/{1}/Resources.xaml", assemblyName, platformFolder), UriKind.Relative);
			MergedDictionaries.Add(resources);
		}
		private void OnTextBlockSizeChanged(object sender, SizeChangedEventArgs e) {
			FrameworkElement elem = sender as FrameworkElement;
			if(elem == null) return;
			TextBlockService.SetIsTextTrimmed(elem, TextBlockService.GetIsActualTextBlockTrimmed(elem as TextBlock));
		}
	}
	public class NullableToVisibilityConverter : IValueConverter {
		DevExpress.Design.UI.BooleanToVisibilityConverter booleanToVisibility;
		public NullableToVisibilityConverter() {
			booleanToVisibility = new DevExpress.Design.UI.BooleanToVisibilityConverter();
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return booleanToVisibility.Convert(value != null, targetType, parameter, culture);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return booleanToVisibility.ConvertBack(value, targetType, parameter, culture);
		}
	}
	public class ActionListPropertyLineViewModelStringConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string str = value as string;
			if(string.IsNullOrEmpty(str))
				return "";
			else return str + " ";
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
#if SL
	public class ImageSourcePropertyLineViewModel { }
	public class ImageSourcePropertyLineControl : UserControl { }
#endif
}
