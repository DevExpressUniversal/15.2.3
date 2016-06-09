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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ApplyIconSetConditionalFormattingCommand (abstract class)
	public abstract class ApplyIconSetConditionalFormattingCommand : ApplyConditionalFormattingCommand {
		static readonly int[] threePoints = new int[] { 0, 33, 67 };
		static readonly int[] fourPoints = new int[] { 0, 25, 50, 75 };
		static readonly int[] fivePoints = new int[] { 0, 20, 40, 60, 80 };
		protected ApplyIconSetConditionalFormattingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract IconSetType IconSetType { get; }
		protected abstract int IconsCount { get; }
		protected int[] GetPercents() {
			switch (IconsCount) {
				default:
				case 3:
					return threePoints;
				case 4:
					return fourPoints;
				case 5:
					return fivePoints;
			}
		}
		protected ConditionalFormattingValueObject GetPredefinedValue(Worksheet sheet, int index) {
			int[] percents = GetPercents();
			return new ConditionalFormattingValueObject(sheet, ConditionalFormattingValueObjectType.Percent, percents[index], true);
		}
		protected override ConditionalFormatting CreateConditionalFormatting(Worksheet sheet, CellRangeBase range) {
			IconSetConditionalFormatting result = new IconSetConditionalFormatting(sheet);
			result.IconSet = IconSetType;
			for (int i = 0; i < IconsCount; i++)
				result.SetPointValue(i, GetPredefinedValue(sheet, i));
			result.SetCellRange(range);
			return result;
		}
	}
	#endregion
}
