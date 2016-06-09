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

using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxSpreadsheet {
	#region Chart Title command
	public class SRChartTitleCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartTitleCommandGroup;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartTitleNoneCommand());
			Items.Add(new SRChartTitleCenteredOverlayCommand());
			Items.Add(new SRChartTitleAboveCommand());
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
	}
	public class SRChartTitleNoneCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartTitleNone;
			}
		}
	}
	public class SRChartTitleCenteredOverlayCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartTitleCenteredOverlay;
			}
		}
	}
	public class SRChartTitleAboveCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartTitleAbove;
			}
		}
	}
	#endregion Chart Title command
	#region Axis Titles command
	public class SRChartAxisTitlesCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartAxisTitlesCommandGroup;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartPrimaryHorizontalAxisTitleCommand());
			Items.Add(new SRChartPrimaryVerticalAxisTitleCommand());
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
	}
	public class SRChartPrimaryHorizontalAxisTitleCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleCommandGroup;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartPrimaryHorizontalAxisTitleNone());
			Items.Add(new SRChartPrimaryHorizontalAxisTitleBelow());
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
	}
	public class SRChartPrimaryVerticalAxisTitleCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleCommandGroup;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartPrimaryVerticalAxisTitleNone());
			Items.Add(new SRChartPrimaryVerticalAxisTitleRotated());
			Items.Add(new SRChartPrimaryVerticalAxisTitleVertical());
			Items.Add(new SRChartPrimaryVerticalAxisTitleHorizontal());
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
	}
	public class SRChartPrimaryHorizontalAxisTitleNone : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartHidePrimaryHorizontalAxis;
			}
		}
	}
	public class SRChartPrimaryHorizontalAxisTitleBelow : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleBelow;
			}
		}
	}
	public class SRChartPrimaryVerticalAxisTitleNone : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleNone;
			}
		}
	}
	public class SRChartPrimaryVerticalAxisTitleRotated : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleRotated;
			}
		}
	}
	public class SRChartPrimaryVerticalAxisTitleVertical : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleVertical;
			}
		}
	}
	public class SRChartPrimaryVerticalAxisTitleHorizontal : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleHorizontal;
			}
		}
	}
	#endregion Axis Titles command
	#region Chart Legend command
	public class SRChartLegendCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartLegendCommandGroup;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartLegendNoneCommand());
			Items.Add(new SRChartLegendAtRightCommand());
			Items.Add(new SRChartLegendAtTopCommand());
			Items.Add(new SRChartLegendAtLeftCommand());
			Items.Add(new SRChartLegendAtBottomCommand());
			Items.Add(new SRChartLegendOverlayAtRightCommand());
			Items.Add(new SRChartLegendOverlayAtLeftCommand());
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
	}
	public class SRChartLegendNoneCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartLegendNone;
			}
		}
	}
	public class SRChartLegendAtRightCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartLegendAtRight;
			}
		}
	}
	public class SRChartLegendAtTopCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartLegendAtTop;
			}
		}
	}
	public class SRChartLegendAtLeftCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartLegendAtLeft;
			}
		}
	}
	public class SRChartLegendAtBottomCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartLegendAtBottom;
			}
		}
	}
	public class SRChartLegendOverlayAtRightCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartLegendOverlayAtRight;
			}
		}
	}
	public class SRChartLegendOverlayAtLeftCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartLegendOverlayAtLeft;
			}
		}
	}
	#endregion Chart Legend command
	#region Chart Data Labels command
	public class SRChartDataLabelsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartDataLabelsCommandGroup;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartDataLabelsNoneCommand());
			Items.Add(new SRChartDataLabelsCenterCommand());
			Items.Add(new SRChartDataLabelsInsideEndCommand());
			Items.Add(new SRChartDataLabelsInsideBaseCommand());
			Items.Add(new SRChartDataLabelsOutsideEndCommand());
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
	}
	public class SRChartDataLabelsNoneCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartDataLabelsNone;
			}
		}
	}
	public class SRChartDataLabelsCenterCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartDataLabelsCenter;
			}
		}
	}
	public class SRChartDataLabelsInsideEndCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartDataLabelsInsideEnd;
			}
		}
	}
	public class SRChartDataLabelsInsideBaseCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartDataLabelsInsideBase;
			}
		}
	}
	public class SRChartDataLabelsOutsideEndCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartDataLabelsOutsideEnd;
			}
		}
	}
	#endregion Chart Data Labels command
}
