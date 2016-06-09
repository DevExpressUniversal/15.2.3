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
namespace DevExpress.Export.Xl {
	public enum XlDataValidationType {
		None = 0,
		Whole = 1,
		Decimal = 2,
		List = 3,
		Date = 4,
		Time = 5,
		TextLength = 6,
		Custom = 7,
	}
	public enum XlDataValidationOperator {
		Between = 0,
		NotBetween = 1,
		Equal = 2,
		NotEqual = 3,
		LessThan = 5,
		LessThanOrEqual = 7,
		GreaterThan = 4,
		GreaterThanOrEqual = 6,
	}
	public enum XlDataValidationErrorStyle {
		Stop = 0,
		Warning = 1,
		Information = 2,
	}
	public enum XlDataValidationImeMode {
		NoControl = 0,
		On = 1,
		Off = 2,
		Disabled = 3,
		Hiragana = 4,
		FullKatakana = 5,
		HalfKatakana = 6,
		FullAlpha = 7,
		HalfAlpha = 8,
		FullHangul = 9,
		HalfHangul = 10,
	}
	public class XlDataValidation {
		readonly List<XlCellRange> ranges = new List<XlCellRange>();
		readonly List<XlVariantValue> listValues = new List<XlVariantValue>();
		XlValueObject criteria1 = XlValueObject.Empty;
		XlValueObject criteria2 = XlValueObject.Empty;
		public XlDataValidation() {
			AllowBlank = true;
			ErrorMessage = string.Empty;
			ErrorTitle = string.Empty;
			InputPrompt = string.Empty;
			PromptTitle = string.Empty;
			ShowDropDown = true;
			ShowErrorMessage = true;
			ShowInputMessage = true;
		}
		#region Properties
		public XlDataValidationType Type { get; set; }
		public IList<XlCellRange> Ranges { get { return ranges; } }
		public bool AllowBlank { get; set; }
		public XlDataValidationImeMode ImeMode { get; set; }
		public XlDataValidationOperator Operator { get; set; }
		public XlDataValidationErrorStyle ErrorStyle { get; set; }
		public string ErrorTitle { get; set; }
		public string ErrorMessage { get; set; }
		public string InputPrompt { get; set; }
		public string PromptTitle { get; set; }
		public bool ShowDropDown { get; set; }
		public bool ShowErrorMessage { get; set; }
		public bool ShowInputMessage { get; set; }
		public XlValueObject Criteria1 {
			get { return criteria1; }
			set {
				if(value == null)
					criteria1 = XlValueObject.Empty;
				else
					criteria1 = value;
			}
		}
		public XlValueObject Criteria2 {
			get { return criteria2; }
			set {
				if(value == null)
					criteria2 = XlValueObject.Empty;
				else
					criteria2 = value;
			}
		}
		public IList<XlVariantValue> ListValues { get { return listValues; } }
		public XlCellRange ListRange { get { return Criteria1.RangeValue; } set { Criteria1 = value; } }
		#endregion
		protected internal bool IsExtended {
			get {
				return (criteria1.IsRange && !string.IsNullOrEmpty(criteria1.RangeValue.SheetName)) ||
					(criteria2.IsRange && !string.IsNullOrEmpty(criteria2.RangeValue.SheetName));
			}
		}
	}
}
