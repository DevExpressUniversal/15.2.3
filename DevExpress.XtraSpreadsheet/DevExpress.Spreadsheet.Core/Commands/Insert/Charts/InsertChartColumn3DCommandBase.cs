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
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertChartColumn3DCommandBase (abstract class)
	public abstract class InsertChartColumn3DCommandBase : InsertChartColumn2DCommandBase {
		protected InsertChartColumn3DCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract BarShape Shape { get; }
		protected override ChartLayoutModifier Preset {
			get {
				if (Grouping == BarChartGrouping.Clustered || Grouping == BarChartGrouping.Standard)
					return ChartColumnClusteredPresets.Instance.DefaultModifier;
				else
					return ChartColumnStackedPresets.Instance.DefaultModifier;
			}
		}
		protected override ChartViewType ViewType { get { return ChartViewType.Bar3D; } }
		protected internal override IChartView CreateChartView(IChart parent) {
			Bar3DChartView view = new Bar3DChartView(parent);
			view.Grouping = Grouping;
			view.BarDirection = BarDirection;
			view.Shape = Shape;
			return view;
		}
		protected override void SetupView3D(View3DOptions view3D) {
			view3D.BeginUpdate();
			try {
				view3D.RightAngleAxes = true;
				view3D.XRotation = 15;
				view3D.YRotation = 20;
			}
			finally {
				view3D.EndUpdate();
			}
		}
		protected internal override bool IsCompatibleView(IChartView chartView) {
			Bar3DChartView view = chartView as Bar3DChartView;
			return view != null && view.Grouping == this.Grouping && view.BarDirection == this.BarDirection && view.Shape == this.Shape;
		}
	}
	#endregion
}
