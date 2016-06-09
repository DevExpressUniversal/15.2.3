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

namespace DevExpress.DashboardCommon.ViewModel {
	public enum NumericFormatType {
		General = DataItemNumericFormatType.General,
		Number = DataItemNumericFormatType.Number,
		Currency = DataItemNumericFormatType.Currency,
		Scientific = DataItemNumericFormatType.Scientific,
		Percent = DataItemNumericFormatType.Percent,
	}
	public class NumericFormatViewModel {
		NumericFormatType formatType;
		int precision;
		DataItemNumericUnit unit;
		bool includeGroupSeparator;
		bool forcePlusSign;
		int significatntDigits;
		string currencyCulture;
		public NumericFormatType FormatType { 
			get { return formatType; }
			set { formatType = value; }
		}
		public int Precision { 
			get { return precision; } 
			set { precision = value; }
		}
		public DataItemNumericUnit Unit { 
			get { return unit; }
			set { unit = value; }
		}
		public bool IncludeGroupSeparator { 
			get { return includeGroupSeparator; }
			set { includeGroupSeparator = value; }
		}
		public bool ForcePlusSign { 
			get { return forcePlusSign; }
			set { forcePlusSign = value; }
		}
		public int SignificantDigits { 
			get { return significatntDigits; }
			set { significatntDigits = value; }
		}
		public string CurrencyCulture { 
			get { return currencyCulture; }
			set { currencyCulture = value; }
		}
		public NumericFormatViewModel() {
		}
		public NumericFormatViewModel(NumericFormatType formatType, int precision, DataItemNumericUnit unit, bool includeGroupSeparator, bool forcePlusSign, int significantDigits, string currencyCulture) {
			this.formatType = formatType;
			this.precision = precision;
			this.unit = unit;
			this.includeGroupSeparator = includeGroupSeparator;
			this.forcePlusSign = forcePlusSign;
			this.significatntDigits = significantDigits;
			this.currencyCulture = currencyCulture;
		}
		public bool EqualsViewModel(NumericFormatViewModel format) {
			return format != null &&
				format.formatType == formatType &&
				format.precision == precision &&
				format.unit == unit &&
				format.includeGroupSeparator == includeGroupSeparator &&
				format.forcePlusSign == forcePlusSign &&
				format.significatntDigits == significatntDigits &&
				format.currencyCulture == currencyCulture;
		}
	}
}
