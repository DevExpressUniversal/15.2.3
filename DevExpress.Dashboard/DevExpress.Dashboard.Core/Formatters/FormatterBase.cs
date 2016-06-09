#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using System.Globalization;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Native {
	public abstract class FormatterBase {
		public static FormatterBase CreateFormatter(CultureInfo clientCulture, ValueFormatViewModel format) {
			switch(format.DataType) {
				case ValueDataType.Numeric:
					return NumericFormatter.CreateInstance(clientCulture, format.NumericFormat);
				case ValueDataType.DateTime:
					return DateTimeFormatter.CreateInstance(clientCulture, format.DateTimeFormat);
				default:
					return new StringFormatter();
			}
		}
		public static FormatterBase CreateFormatter(ValueFormatViewModel format) {
			return CreateFormatter(Helper.CurrentCulture, format);
		}
		public abstract string FormatPattern { get; }
		public string Format(object value) {
			IList<double> array = value as IList<double>;
			if(array != null)
				value = array.Count > 0 ? (object)array[0] : null;
			if (value == null)
				return string.Empty;
			string formattedText;
			if(!DashboardSpecialValuesInternal.TryGetDisplayText(value, out formattedText))
				formattedText = FormatInternal(value);
			return formattedText;
		}
		protected abstract string FormatInternal(object value);
	}
}
