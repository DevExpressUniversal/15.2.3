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

using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Utils {
	public class PaperKindToDisplayNameConverter : MarkupExtension, IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			var isRollPaper = (bool)values[1];
			return isRollPaper ? string.Format("{0} ({1})", values[0], values[1]) : values[0];
		}
		object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class GraphicsUnitsToIncrementConverter : MarkupExtension, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var unit = (GraphicsUnit)value;
			if (unit != GraphicsUnit.Inch && unit != GraphicsUnit.Millimeter)
				throw new InvalidOperationException();
			return (GraphicsUnit)value == GraphicsUnit.Inch ? 0.01f : 0.1f;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class GraphicsUnitsToMaskConverter : MarkupExtension, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var unit = (GraphicsUnit)value;
			if (unit != GraphicsUnit.Inch && unit != GraphicsUnit.Millimeter)
				throw new InvalidOperationException();
			return (GraphicsUnit)value == GraphicsUnit.Inch ? "##0.00" : "##0.0";
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class PaperKindToSizeStringConverter : MarkupExtension, IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			var actualUnit = (GraphicsUnit)values[2];
			var expectedUnit = (GraphicsUnit)values[3];
			if (actualUnit != GraphicsUnit.Inch && actualUnit != GraphicsUnit.Millimeter
				|| expectedUnit != GraphicsUnit.Inch && expectedUnit != GraphicsUnit.Millimeter)
				throw new InvalidOperationException();
			var width = ConvertFloat((float)values[0], actualUnit, expectedUnit);
			var height = ConvertFloat((float)values[1], actualUnit, expectedUnit);
			return string.Format("{0} x {1}", width, height);
		}
		float ConvertFloat(float value, GraphicsUnit actual, GraphicsUnit expected) {
			return (float)Math.Round(GraphicsUnitConverter.Convert(value, actual, expected), expected == GraphicsUnit.Inch ? 2 : 1);
		}
		object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
}
