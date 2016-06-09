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
namespace DevExpress.Office.Utils {
	#region ValueInfo
	public class ValueInfo {
		static readonly ValueInfo empty = new ValueInfo();
		public static ValueInfo Empty { get { return empty; } }
		#region Fields
		string unit;
		float value;
		bool isValidNumber = false;
		#endregion
		public ValueInfo() {
			this.unit = String.Empty;
		}
		public ValueInfo(string unit) {
			this.isValidNumber = false;
			this.unit = unit;
		}
		public ValueInfo(float value, string unit) {
			this.isValidNumber = true;
			this.unit = unit;
			this.value = value;
		}
		#region Properties
		public string Unit { get { return unit; } }
		public float Value { get { return value; } }
		public bool IsValidNumber { get { return isValidNumber; } }
		public bool IsEmpty { get { return this == ValueInfo.Empty; } }
		#endregion
	}
	#endregion
	#region StringValueParser
	public static class StringValueParser {
		static char[] valueChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
		internal struct SplitResult {
			public string Value;
			public string Unit;
			public SplitResult(string value, string unit) {
				this.Value = value;
				this.Unit = unit;
			}
		}
		public static ValueInfo TryParse(string inputString) {
			if(String.IsNullOrEmpty(inputString))
				return ValueInfo.Empty;
			SplitResult result = SplitUnitFromValue(inputString);
			return Parse(result);
		}
		internal static SplitResult SplitUnitFromValue(string inputString) {
			string value = string.Empty;
			string unit = inputString;
			int pos = inputString.LastIndexOfAny(valueChars);
			if(pos != -1) {
				value = inputString.Substring(0, pos + 1);
				unit = inputString.Substring(pos + 1);
			}
			return new SplitResult(value, unit);
		}
		internal static ValueInfo Parse(SplitResult result) {
			double value;
			if(Double.TryParse(result.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
				return new ValueInfo((float)value, result.Unit);
			else
				return new ValueInfo(result.Unit);
		}
	}
	#endregion
}
