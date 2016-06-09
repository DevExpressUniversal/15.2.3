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

namespace DevExpress.XtraSpreadsheet.Model {
	#region RemoveShiftLeftSharedFormulaRPNLogic
	public class RemoveShiftLeftSharedFormulaRPNLogic : RemoveShiftLeftRPNLogic {
		public RemoveShiftLeftSharedFormulaRPNLogic(CellRange modyfingRange)
			: base(modyfingRange) {
		}
		protected internal override bool IsResizingRange(CellRange range) {
			CellRange intersection = ModifyingRange.Intersection(range);
			return intersection != null && (intersection.Height == range.Height || intersection.Width == range.Width);
		}
		protected internal override bool IsMovingRange(CellRange range) {
			if (ModifyingRange.TopLeft.Column > range.TopLeft.Column)
				return false;
			CellRange intersection = GetChangingRange().Intersection(range);
			return intersection != null && intersection.Height == range.Height;
		}
	}
	#endregion    
	#region RemoveShiftUpSharedFormulaRPNLogic 
	public class RemoveShiftUpSharedFormulaRPNLogic : RemoveShiftUpRPNLogic {
		public RemoveShiftUpSharedFormulaRPNLogic(CellRange modyfingRange)
			: base(modyfingRange) {
		}
		protected internal override bool IsMovingRange(CellRange range) {
			if (ModifyingRange.Intersects(range))
				return true;
			CellRange prohibitedRange = new CellRange(range.Worksheet, new CellPosition(range.TopLeft.Column, 0), new CellPosition(range.BottomRight.Column, range.TopLeft.Row - 1));
			if (prohibitedRange.Intersects(ModifyingRange))
				return ModifyingRange.IntersectionWith(prohibitedRange).CellRangeValue.Width == prohibitedRange.Width;
			return false;
		}
	}
	#endregion    
	#region InsertShiftRightSharedFormulaRPNLogic
	public class InsertShiftRightSharedFormulaRPNLogic : InsertShiftRightRPNLogic {
		public InsertShiftRightSharedFormulaRPNLogic(CellRange modyfingRange)
			: base(modyfingRange) {
		}
		protected internal override bool IsResizingRange(CellRange range) {
			if (ModifyingRange.Intersects(range) && ModifyingRange.TopLeft.Column >= range.TopLeft.Column + 1)
				return true;
			CellRange prohibitedRange = new CellRange(range.Worksheet, new CellPosition(0, range.TopLeft.Row), new CellPosition(range.TopLeft.Column + 1, range.BottomRight.Row));
			if (prohibitedRange.Intersects(ModifyingRange))
				return ModifyingRange.IntersectionWith(prohibitedRange).CellRangeValue.Height != prohibitedRange.Height;
			return false;
		}
		protected internal override bool IsMovingRange(CellRange range) {
			if (ModifyingRange.Includes(range))
				return true;
			CellRange prohibitedRange = new CellRange(range.Worksheet, new CellPosition(0, range.TopLeft.Row), new CellPosition(range.TopLeft.Column, range.BottomRight.Row));
			if (prohibitedRange.Intersects(ModifyingRange))
				return ModifyingRange.IntersectionWith(prohibitedRange).CellRangeValue.Height == prohibitedRange.Height;
			return false;
		}
	}
	#endregion
	#region InsertShiftDownSharedFormulaRPNLogic 
	public class InsertShiftDownSharedFormulaRPNLogic : InsertShiftDownRPNLogic {
		public InsertShiftDownSharedFormulaRPNLogic(CellRange modyfingRange)
			: base(modyfingRange) {
		}
		protected internal override bool IsResizingRange(CellRange range) {
			if (ModifyingRange.TopLeft.Row <= range.TopLeft.Row)
				return false;
			CellRange intersection = GetChangingRange().Intersection(range);
			return intersection != null && intersection.Width == range.Width;
		}
		protected internal override bool IsMovingRange(CellRange range) {
			if (ModifyingRange.Includes(range))
				return true;
			CellRange prohibitedRange = new CellRange(range.Worksheet, new CellPosition(range.TopLeft.Column, 0), new CellPosition(range.BottomRight.Column, range.TopLeft.Row));
			if (prohibitedRange.Intersects(ModifyingRange))
				return ModifyingRange.IntersectionWith(prohibitedRange).CellRangeValue.Width == prohibitedRange.Width;
			return false;
		}
	}
	#endregion
}
