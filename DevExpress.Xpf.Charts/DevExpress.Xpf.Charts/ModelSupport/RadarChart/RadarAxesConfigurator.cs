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

using System.Collections.Generic;
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using Model = DevExpress.Charts.Model;
namespace DevExpress.Xpf.Charts.ModelSupport {
	public class RadarAxesConfigurator : AxisBaseConfigurator {
		Model.RadarChart ChartModel { get { return ModelElement as Model.RadarChart; } }
		protected override DiagrammAppearanceConfiguratorBase AppearanceConfigurator {
			get { return new CircularDiagramAppearanceConfigurator(Container); }
		}
		protected RadarAxesConfigurator(Model.CircularChart chartModel, Model.IModelElementContainer container)
			: base(chartModel, container) {
		}
		public RadarAxesConfigurator(Model.RadarChart chartModel, Model.IModelElementContainer container)
			: base(chartModel, container) {
		}
		void ConfigureAxisX(RadarAxisX2D axis, Model.RadarAxisX modelAxis) {
			if (modelAxis != null) {
				double gridSpacing = modelAxis.AutoGrid ? double.NaN : modelAxis.GridSpacing;
				axis.NumericScaleOptions = new ContinuousNumericScaleOptions() { AutoGrid = modelAxis.AutoGrid, GridSpacing = gridSpacing };
				axis.DateTimeScaleOptions = new ContinuousDateTimeScaleOptions() {
					AutoGrid = modelAxis.AutoGrid,
					GridSpacing = gridSpacing,
					GridAlignment = (DateTimeGridAlignment)modelAxis.GridAlignment
				};
				ConfigureAxisBase(axis, modelAxis);
				if (modelAxis.Range != null) {
					axis.WholeRange = new Range();
					if (modelAxis.Range.MinValue != null)
						axis.WholeRange.MinValue = modelAxis.Range.MinValue;
					if (modelAxis.Range.MaxValue != null)
						axis.WholeRange.MaxValue = modelAxis.Range.MaxValue;
				}				
				axis.Logarithmic = modelAxis.Logarithmic;
				if (modelAxis.Logarithmic)
					axis.LogarithmicBase = modelAxis.LogarithmicBase;
			}
		}
		protected void ConfigureAxisY(CircularAxisY2D axis, Model.CircularAxisY modelAxis) {
			if (modelAxis != null) {
				double gridSpacing = modelAxis.AutoGrid ? double.NaN : modelAxis.GridSpacing;
				axis.NumericScaleOptions = new ContinuousNumericScaleOptions() { AutoGrid = modelAxis.AutoGrid, GridSpacing = gridSpacing };
				axis.DateTimeScaleOptions = new ContinuousDateTimeScaleOptions() {
					AutoGrid = modelAxis.AutoGrid,
					GridSpacing = gridSpacing,
					GridAlignment = (DateTimeGridAlignment)modelAxis.GridAlignment
				};
				axis.Visible = modelAxis.Visible;
				ConfigureAxisBase(axis, modelAxis);
				axis.TickmarksVisible = modelAxis.TickmarksVisible;
				axis.TickmarksMinorVisible = modelAxis.TickmarksMinorVisible;
				axis.TickmarksCrossAxis = modelAxis.TickmarksCrossAxis;
				if (modelAxis.Range != null) {
					axis.WholeRange = new Range();
					if (modelAxis.Range.MinValue != null)
						axis.WholeRange.MinValue = modelAxis.Range.MinValue;
					if (modelAxis.Range.MaxValue != null)
						axis.WholeRange.MaxValue = modelAxis.Range.MaxValue;
				}
				axis.Logarithmic = modelAxis.Logarithmic;
				if (modelAxis.Logarithmic)
					axis.LogarithmicBase = modelAxis.LogarithmicBase;
			}
		}
		protected override void ConfigureAxisCore(AxisBase axis, Model.AxisBase modelAxisBase) {
			if (axis is RadarAxisX2D)
				ConfigureAxisX((RadarAxisX2D)axis, (Model.RadarAxisX)modelAxisBase);
			if (axis is CircularAxisY2D)
				ConfigureAxisY((CircularAxisY2D)axis, (Model.CircularAxisY)modelAxisBase);
		}
		public void Configure(RadarDiagram2D diagram) {
			diagram.AxisX = new RadarAxisX2D();
			diagram.AxisY = new RadarAxisY2D();
			ConfigureAxis(diagram.AxisX, ChartModel.ArgumentAxis);
			ConfigureAxis(diagram.AxisY, ChartModel.ValueAxis);
		}
	}
}
