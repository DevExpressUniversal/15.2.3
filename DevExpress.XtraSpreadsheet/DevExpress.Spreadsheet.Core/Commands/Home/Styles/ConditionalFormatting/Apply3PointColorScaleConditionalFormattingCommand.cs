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
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Commands {
	#region Apply3PointColorScaleConditionalFormattingCommand (abstract class)
	public abstract class Apply3PointColorScaleConditionalFormattingCommand : ApplyConditionalFormattingCommand {
		protected Apply3PointColorScaleConditionalFormattingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected Color Red { get { return DXColor.FromArgb(248, 105, 107); } }
		protected Color Yellow { get { return DXColor.FromArgb(255, 235, 132); } }
		protected Color Green { get { return DXColor.FromArgb(99, 190, 123); } }
		protected Color Blue { get { return DXColor.FromArgb(90, 138, 198); } }
		protected Color White { get { return DXColor.White; } }
		protected abstract Color ColorLow { get; }
		protected abstract Color ColorMiddle { get; }
		protected abstract Color ColorHigh { get; }
		protected override ConditionalFormatting CreateConditionalFormatting(Worksheet sheet, CellRangeBase range) {
			ColorScaleConditionalFormatting result = new ColorScaleConditionalFormatting(sheet);
			result.LowPointColor = ColorLow;
			result.LowPointValue = new ConditionalFormattingValueObject(sheet, ConditionalFormattingValueObjectType.Min, 0);
			if(!DXColor.IsTransparentOrEmpty(ColorMiddle)) {
				result.MiddlePointColor = ColorMiddle;
				result.MiddlePointValue = new ConditionalFormattingValueObject(sheet, ConditionalFormattingValueObjectType.Percent, 50);
			}
			result.HighPointColor = ColorHigh;
			result.HighPointValue = new ConditionalFormattingValueObject(sheet, ConditionalFormattingValueObjectType.Max, 0);
			result.SetCellRange(range);
			return result;
		}
	}
	#endregion
}
