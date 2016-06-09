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
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.Web.ASPxSpreadsheet {
	#region Chart Axes command
	public class SRChartAxesCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartAxesCommandGroup;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartPrimaryHorizontalAxisCommand());
			Items.Add(new SRChartPrimaryVerticalAxisCommand());
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
	public class SRChartPrimaryHorizontalAxisCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalAxisCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartHidePrimaryHorizontalAxisCommand());
			Items.Add(new SRChartPrimaryHorizontalAxisLeftToRightCommand());
			Items.Add(new SRChartPrimaryHorizontalAxisHideLabelsCommand());
			Items.Add(new SRChartPrimaryHorizontalAxisRightToLeftCommand());
		}
	}
	public class SRChartPrimaryVerticalAxisCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartHidePrimaryVerticalAxisCommand());
			Items.Add(new SRChartPrimaryVerticalAxisDefaultCommand());
			Items.Add(new SRChartPrimaryVerticalAxisScaleThousandsCommand());
			Items.Add(new SRChartPrimaryVerticalAxisScaleMillionsCommand());
			Items.Add(new SRChartPrimaryVerticalAxisScaleBillionsCommand());
			Items.Add(new SRChartPrimaryVerticalAxisScaleLogarithmCommand());
		}
	}
	public class SRChartHidePrimaryHorizontalAxisCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartHidePrimaryHorizontalAxis;
			}
		}
	}
	public class SRChartPrimaryHorizontalAxisLeftToRightCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalAxisLeftToRight;
			}
		}
	}
	public class SRChartPrimaryHorizontalAxisHideLabelsCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalAxisHideLabels;
			}
		}
	}
	public class SRChartPrimaryHorizontalAxisRightToLeftCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalAxisRightToLeft;
			}
		}
	}
	public class SRChartHidePrimaryVerticalAxisCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartHidePrimaryVerticalAxis;
			}
		}
	}
	public class SRChartPrimaryVerticalAxisDefaultCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisDefault;
			}
		}
	}
	public class SRChartPrimaryVerticalAxisScaleThousandsCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleThousands;
			}
		}
	}
	public class SRChartPrimaryVerticalAxisScaleMillionsCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleMillions;
			}
		}
	}
	public class SRChartPrimaryVerticalAxisScaleBillionsCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleBillions;
			}
		}
	}
	public class SRChartPrimaryVerticalAxisScaleLogarithmCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleLogarithm;
			}
		}
	}
	#endregion
	#region Chart Gridlines command
	public class SRChartGridlinesCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartGridlinesCommandGroup;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartPrimaryHorizontalGridlinesCommand());
			Items.Add(new SRChartPrimaryVerticalGridlinesCommand());
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
	public class SRChartPrimaryHorizontalGridlinesCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartPrimaryHorizontalGridlinesNoneCommand());
			Items.Add(new SRChartPrimaryHorizontalGridlinesMajorCommand());
			Items.Add(new SRChartPrimaryHorizontalGridlinesMinorCommand());
			Items.Add(new SRChartPrimaryHorizontalGridlinesMajorAndMinorCommand());
		}
	}
	public class SRChartPrimaryVerticalGridlinesCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalGridlinesCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRChartPrimaryVerticalGridlinesNoneCommand());
			Items.Add(new SRChartPrimaryVerticalGridlinesMajorCommand());
			Items.Add(new SRChartPrimaryVerticalGridlinesMinorCommand());
			Items.Add(new SRChartPrimaryVerticalGridlinesMajorAndMinorCommand());
		}
	}
	public class SRChartPrimaryHorizontalGridlinesNoneCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesNone;
			}
		}
	}
	public class SRChartPrimaryHorizontalGridlinesMajorCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMajor;
			}
		}
	}
	public class SRChartPrimaryHorizontalGridlinesMinorCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMinor;
			}
		}
	}
	public class SRChartPrimaryHorizontalGridlinesMajorAndMinorCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMajorAndMinor;
			}
		}
	}
	public class SRChartPrimaryVerticalGridlinesNoneCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalGridlinesNone;
			}
		}
	}
	public class SRChartPrimaryVerticalGridlinesMajorCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMajor;
			}
		}
	}
	public class SRChartPrimaryVerticalGridlinesMinorCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMinor;
			}
		}
	}
	public class SRChartPrimaryVerticalGridlinesMajorAndMinorCommand : SRChartLargeDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMajorAndMinor;
			}
		}
	}
	#endregion
}
