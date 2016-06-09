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

using DevExpress.Utils;
namespace DevExpress.DashboardCommon.ViewModel {
	public enum ValueDataType { String, Numeric, DateTime }
	public class ValueFormatViewModel {
		ValueDataType dataType;
		NumericFormatViewModel numericFormat;
		DateTimeFormatViewModel dateTimeFormat;
		public ValueDataType DataType {
			get { return dataType; }
			set { dataType = value; }
		}
		public NumericFormatViewModel NumericFormat { 
			get { return numericFormat; }
			set { numericFormat = value; }
		}
		public DateTimeFormatViewModel DateTimeFormat { 
			get { return dateTimeFormat; }
			set { dateTimeFormat = value; }
		}
		public ValueFormatViewModel() {
			dataType = ValueDataType.String;
		}
		public ValueFormatViewModel(NumericFormatViewModel numericFormat) {
			Guard.ArgumentNotNull(numericFormat, "numericFormat");
			this.numericFormat = numericFormat;
			dataType = ValueDataType.Numeric;
		}
		public ValueFormatViewModel(DateTimeFormatViewModel dateTimeFormat) {
			Guard.ArgumentNotNull(dateTimeFormat, "dateTimeFormat");
			this.dateTimeFormat = dateTimeFormat;
			dataType = ValueDataType.DateTime;
		}
		public bool EqualsViewModel(ValueFormatViewModel format) {
			if(format == null)
				return false;
			bool numericFormatEquals = numericFormat == null && format.numericFormat == null || numericFormat != null && numericFormat.EqualsViewModel(format.numericFormat);
			bool dateTimeFormatEquals = dateTimeFormat == null && format.dateTimeFormat == null || dateTimeFormat != null && dateTimeFormat.EqualsViewModel(format.dateTimeFormat);
			return dataType == format.dataType && numericFormatEquals && dateTimeFormatEquals;
		}
	}
}
