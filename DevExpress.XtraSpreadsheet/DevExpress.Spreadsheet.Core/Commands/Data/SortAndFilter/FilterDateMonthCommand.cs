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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FilterDateMonthCommandBase (abstract class)
	public abstract class FilterDateMonthCommandBase : FilterDynamicCommandBase {
		protected FilterDateMonthCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterMonth; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterMonthDescription; } }
		public override string MenuCaption { get { return String.Format(base.MenuCaption, MonthName); } }
		public override string Description { get { return String.Format(base.Description, MonthName); } }
		string MonthName {
			get {
				int month = Math.Max(0, Math.Min(11, FilterType - DynamicFilterType.M1));
				return CultureInfo.CurrentUICulture.DateTimeFormat.MonthNames[month];
			}
		}
		#endregion
	}
	#endregion
	#region FilterDateMonthJanuaryCommand
	public class FilterDateMonthJanuaryCommand : FilterDateMonthCommandBase {
		public FilterDateMonthJanuaryCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthJanuary; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M1; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthFebruaryCommand
	public class FilterDateMonthFebruaryCommand : FilterDateMonthCommandBase {
		public FilterDateMonthFebruaryCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthFebruary; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M2; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthMarchCommand
	public class FilterDateMonthMarchCommand : FilterDateMonthCommandBase {
		public FilterDateMonthMarchCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthMarch; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M3; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthAprilCommand
	public class FilterDateMonthAprilCommand : FilterDateMonthCommandBase {
		public FilterDateMonthAprilCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthApril; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M4; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthMayCommand
	public class FilterDateMonthMayCommand : FilterDateMonthCommandBase {
		public FilterDateMonthMayCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthMay; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M5; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthJuneCommand
	public class FilterDateMonthJuneCommand : FilterDateMonthCommandBase {
		public FilterDateMonthJuneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthJune; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M6; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthJulyCommand
	public class FilterDateMonthJulyCommand : FilterDateMonthCommandBase {
		public FilterDateMonthJulyCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthJuly; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M7; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthAugustCommand
	public class FilterDateMonthAugustCommand : FilterDateMonthCommandBase {
		public FilterDateMonthAugustCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthAugust; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M8; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthSeptemberCommand
	public class FilterDateMonthSeptemberCommand : FilterDateMonthCommandBase {
		public FilterDateMonthSeptemberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthSeptember; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M9; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthOctoberCommand
	public class FilterDateMonthOctoberCommand : FilterDateMonthCommandBase {
		public FilterDateMonthOctoberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthOctober; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M10; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthNovemberCommand
	public class FilterDateMonthNovemberCommand : FilterDateMonthCommandBase {
		public FilterDateMonthNovemberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthNovember; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M11; } }
		#endregion
	}
	#endregion
	#region FilterDateMonthDecemberCommand
	public class FilterDateMonthDecemberCommand : FilterDateMonthCommandBase {
		public FilterDateMonthDecemberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterMonthDecember; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.M12; } }
		#endregion
	}
	#endregion
}
