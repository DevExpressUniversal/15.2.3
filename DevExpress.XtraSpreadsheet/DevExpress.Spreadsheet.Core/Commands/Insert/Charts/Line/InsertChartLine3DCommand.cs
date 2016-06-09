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
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertChartLine3DCommand
	public class InsertChartLine3DCommand : InsertChartLineCommandBase {
		public InsertChartLine3DCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertChartLine3D; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertChartLine3D; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertChartLine3DDescription; } }
		public override string ImageName { get { return "CreateLine3DChart"; } }
		protected override ChartGrouping Grouping { get { return ChartGrouping.Standard; } }
		protected override bool ShowMarkers { get { return false; } }
		protected override ChartViewType ViewType { get { return ChartViewType.Line3D; } }
		#endregion
		protected internal override IChartView CreateChartView(IChart parent) {
			return new Line3DChartView(parent);
		}
		protected override void CreateAxes(Chart chart) {
			CreateThreePrimaryAxes(chart);
		}
		protected override void SetupView3D(View3DOptions view3D) {
			view3D.BeginUpdate();
			try {
				view3D.RightAngleAxes = false;
				view3D.XRotation = 15;
				view3D.YRotation = 20;
				view3D.Perspective = 30;
			}
			finally {
				view3D.EndUpdate();
			}
		}
		protected internal override bool IsCompatibleView(IChartView chartView) {
			Line3DChartView view = chartView as Line3DChartView;
			return view != null && view.Grouping == this.Grouping && AreSeriesHaveMarkers(view.Series) == ShowMarkers;
		}
	}
	#endregion
}
