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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.XtraPrinting;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class BorderSideToBooleanConverter : IValueConverter {
		public BorderSide Border { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value == null ? false : ((BorderSide)value & Border) != 0 ? true : false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class ViewKindToVisibilityConverter : MarkupExtension, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var currentValue = (ReportDesignerDocumentViewKind)value;
			var expectedParameter = (ReportDesignerDocumentViewKind)parameter;
			return currentValue == expectedParameter
				? Visibility.Visible
				: Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class ViewKindToBooleanConverter : MarkupExtension, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var currentValue = (ReportDesignerDocumentViewKind)value;
			var expectedParameter = (ReportDesignerDocumentViewKind)parameter;
			return currentValue == expectedParameter;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if(targetType == null)
				return null;
			var currentValue = (bool)value;
			var expectedParameter = (ReportDesignerDocumentViewKind)parameter;
			object defaultValue = null;
			return currentValue ? expectedParameter : defaultValue;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
}
