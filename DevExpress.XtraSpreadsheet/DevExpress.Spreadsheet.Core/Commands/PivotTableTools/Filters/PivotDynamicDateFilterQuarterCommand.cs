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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableDateFilterQuarterCommandBase
	public abstract class PivotTableDateFilterQuarterCommandBase : PivotTableFiltersCommandBase {
		protected PivotTableDateFilterQuarterCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterQuarter; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterQuarterDescription; } }
		public override string MenuCaption { get { return String.Format(base.MenuCaption, GetQuarterNumber()); } }
		public override string Description { get { return String.Format(base.Description, GetQuarterNumber()); } }
		#endregion
		int GetQuarterNumber() {
			return FilterType - PivotFilterType.FirstQuarter + 1;
		}
	}
	#endregion
	#region PivotTableDateFilterFirstQuarterCommand
	public class PivotTableDateFilterFirstQuarterCommand : PivotTableDateFilterQuarterCommandBase {
		public PivotTableDateFilterFirstQuarterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterFirstQuarter; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.FirstQuarter; } }
	}
	#endregion
	#region PivotTableDateFilterSecondQuarterCommand
	public class PivotTableDateFilterSecondQuarterCommand : PivotTableDateFilterQuarterCommandBase {
		public PivotTableDateFilterSecondQuarterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterSecondQuarter; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.SecondQuarter; } }
	}
	#endregion
	#region PivotTableDateFilterThirdQuarterCommand
	public class PivotTableDateFilterThirdQuarterCommand : PivotTableDateFilterQuarterCommandBase {
		public PivotTableDateFilterThirdQuarterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterThirdQuarter; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ThirdQuarter; } }
	}
	#endregion
	#region PivotTableDateFilterFourthQuarterCommand
	public class PivotTableDateFilterFourthQuarterCommand : PivotTableDateFilterQuarterCommandBase {
		public PivotTableDateFilterFourthQuarterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterFourthQuarter; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.FourthQuarter; } }
	}
	#endregion
}
