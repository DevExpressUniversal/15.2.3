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

using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Grid.Native {
	class DisplayCriteriaHelper : IDisplayCriteriaGeneratorNamesSource {
		DataViewBase View { get; set; }
		public DisplayCriteriaHelper(DataViewBase view) {
			View = view;
		}
		#region IDisplayCriteriaGeneratorNamesSource Members
		public string GetDisplayPropertyName(OperandProperty property) {
			ColumnBase column = View.ColumnsCore[property.PropertyName];
			return ReplaceEscapeSequences(ColumnBase.GetDisplayName(column, property.PropertyName));
		}
		public string GetValueScreenText(OperandProperty property, object value) {
			if(ReferenceEquals(property, null))
				return GetValueString(value);
			ColumnBase column = View.ColumnsCore[property.PropertyName];
			if(column != null && column.ColumnFilterMode != ColumnFilterMode.DisplayText)
				return View.GetColumnDisplayText(value, column);
			else
				return GetValueString(value);
		}
		string GetValueString(object value) {
			return value != null ? value.ToString() : string.Empty;
		}
		#endregion
		string ReplaceEscapeSequences(string input) {
			input = input.Replace("\n", " ");
			input = input.Replace("\t", " ");
			input = input.Replace("\r", " ");
			input = System.Text.RegularExpressions.Regex.Replace(input, @"\s{2,}", " ");
			return input;
		}
	}
}
