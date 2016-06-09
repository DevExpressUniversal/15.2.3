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
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FilterDateQuarterCommandBase (abstract class)
	public abstract class FilterDateQuarterCommandBase : FilterDynamicCommandBase {
		protected FilterDateQuarterCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterQuarter; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterQuarterDescription; } }
		public override string MenuCaption {
			get {
				string formatString = base.MenuCaption;
				return String.Format(formatString, FilterType - DynamicFilterType.Q1 + 1);
			}
		}
		public override string Description {
			get {
				string formatString = base.Description;
				return String.Format(formatString, FilterType - DynamicFilterType.Q1 + 1);
			}
		}
		#endregion
	}
	#endregion
	#region FilterDateQuarter1Command
	public class FilterDateQuarter1Command : FilterDateQuarterCommandBase {
		public FilterDateQuarter1Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterQuarter1; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.Q1; } }
		#endregion
	}
	#endregion
	#region FilterDateQuarter2Command
	public class FilterDateQuarter2Command : FilterDateQuarterCommandBase {
		public FilterDateQuarter2Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterQuarter2; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.Q2; } }
		#endregion
	}
	#endregion
	#region FilterDateQuarter3Command
	public class FilterDateQuarter3Command : FilterDateQuarterCommandBase {
		public FilterDateQuarter3Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterQuarter3; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.Q3; } }
		#endregion
	}
	#endregion
	#region FilterDateQuarter4Command
	public class FilterDateQuarter4Command : FilterDateQuarterCommandBase {
		public FilterDateQuarter4Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterQuarter4; } }
		protected override DynamicFilterType FilterType { get { return DynamicFilterType.Q4; } }
		#endregion
	}
	#endregion
}
