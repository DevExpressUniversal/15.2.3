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
	public class ModelAxisLabelFormatter : IAxisLabelFormatter {
		readonly IAxisLabelFormatterCore formatter;
		public ModelAxisLabelFormatter(IAxisLabelFormatterCore formatter) {
			this.formatter = formatter;
		}
		public string GetAxisLabelText(object axisValue) {
			return formatter.GetAxisLabelText(axisValue);
		}
	}
	public abstract class AxisBaseConfigurator : ConfiguratorBase {
		protected abstract DiagrammAppearanceConfiguratorBase AppearanceConfigurator { get; }
		public AxisBaseConfigurator(Model.Chart chartModel, Model.IModelElementContainer container)
			: base(chartModel, container) {
		}
		NumericFormat ConvertNumericFormat(NumericOptionsFormat format) {
			switch (format) {
				case NumericOptionsFormat.Currency:
					return NumericFormat.Currency;
				case NumericOptionsFormat.FixedPoint:
					return NumericFormat.FixedPoint;
				case NumericOptionsFormat.Number:
					return NumericFormat.Number;
				case NumericOptionsFormat.Percent:
					return NumericFormat.Percent;
				case NumericOptionsFormat.Scientific:
					return NumericFormat.Scientific;
				default:
					return NumericFormat.General;
			}
		}
		protected void ConfigureAxisBase(AxisBase axis, Model.AxisBase modelAxis) {
			Container.Register(axis, modelAxis);
			axis.GridLinesVisible = modelAxis.GridLinesVisible;
			axis.GridLinesMinorVisible = modelAxis.GridLinesMinorVisible;
			if (modelAxis.Label != null) {
				axis.ActualLabel.Visible = modelAxis.Label.Visible;
				if (modelAxis.Label.Formatter != null)
					axis.ActualLabel.Formatter = new ModelAxisLabelFormatter(modelAxis.Label.Formatter);
			}
		}
		protected abstract void ConfigureAxisCore(AxisBase axis, Model.AxisBase modelAxisBase);
		protected void ConfigureAxis(AxisBase axis, Model.AxisBase modelAxis) {
			ConfigureAxisCore(axis, modelAxis);
			if (modelAxis != null && modelAxis.Appearance != null)
				AppearanceConfigurator.ConfigureAxis(axis, modelAxis.Appearance);
		}
	}
	public class CartesianAxesConfigurator : AxisBaseConfigurator {
		static readonly Dictionary<Model.AxisPosition, AxisAlignment> alignmentDict = new Dictionary<Model.AxisPosition, AxisAlignment>();
		static CartesianAxesConfigurator() {
			alignmentDict[Model.AxisPosition.Left] = AxisAlignment.Near;
			alignmentDict[Model.AxisPosition.Top] = AxisAlignment.Far;
			alignmentDict[Model.AxisPosition.Right] = AxisAlignment.Far;
			alignmentDict[Model.AxisPosition.Bottom] = AxisAlignment.Near;
		}
		Model.CartesianChart ChartModel { get { return ModelElement as Model.CartesianChart; } }
		protected override DiagrammAppearanceConfiguratorBase AppearanceConfigurator {
			get { return new CartesianDiagrammAppearanceConfigurator(Container); }
		}
		public CartesianAxesConfigurator(Model.CartesianChart chartModel, Model.IModelElementContainer container)
			: base(chartModel, container) {
		}
		void ConfigurateSecondaryArgumentAxes(SecondaryAxisXCollection targetAxes, IList<Model.Axis> modelAxes) {
			foreach (Model.Axis item in modelAxes) {
				SecondaryAxisX2D axisX = new SecondaryAxisX2D();
				ConfigureAxis(axisX, item);
				targetAxes.Add(axisX);
			}
		}
		void ConfigurateSecondaryValueAxes(SecondaryAxisYCollection targetAxes, IList<Model.Axis> modelAxes) {
			foreach (Model.Axis item in modelAxes) {
				SecondaryAxisY2D axisY = new SecondaryAxisY2D();
				ConfigureAxis(axisY, item);
				targetAxes.Add(axisY);
			}
		}
		internal AxisAlignment CalculateAxisAlignment(Model.AxisPosition pos) {
			AxisAlignment align = AxisAlignment.Near;
			if (alignmentDict.TryGetValue(pos, out align))
				return align;
			return AxisAlignment.Near;
		}
		protected override void ConfigureAxisCore(AxisBase axis, Model.AxisBase modelAxisBase) {
			Axis2D axis2D = axis as Axis2D;
			Model.Axis modelAxis = modelAxisBase as Model.Axis;
			if (modelAxis != null && axis2D != null) {
				double gridSpacing = modelAxis.AutoGrid ? double.NaN : modelAxis.GridSpacing;
				if (axis is AxisX2D) {
					AxisX2D axisX2D = (AxisX2D)axis2D;
					axisX2D.NumericScaleOptions = new ContinuousNumericScaleOptions() { AutoGrid = modelAxis.AutoGrid, GridSpacing = gridSpacing };
					axisX2D.DateTimeScaleOptions = new ContinuousDateTimeScaleOptions() {
						AutoGrid = modelAxis.AutoGrid,
						GridSpacing = gridSpacing,
						GridAlignment = (DateTimeGridAlignment)modelAxis.GridAlignment
					};
				}
				if (axis is AxisY2D) {
					AxisY2D axisY2D = (AxisY2D)axis2D;
					axisY2D.NumericScaleOptions = new ContinuousNumericScaleOptions() { AutoGrid = modelAxis.AutoGrid, GridSpacing = gridSpacing };
					axisY2D.DateTimeScaleOptions = new ContinuousDateTimeScaleOptions() {
						AutoGrid = modelAxis.AutoGrid,
						GridSpacing = gridSpacing,
						GridAlignment = (DateTimeGridAlignment)modelAxis.GridAlignment
					};
				}
				axis2D.Visible = modelAxisBase.Visible;
				ConfigureAxisBase(axis, modelAxis);
				axis2D.Alignment = CalculateAxisAlignment(modelAxis.Position);
				axis2D.TickmarksVisible = modelAxis.TickmarksVisible;
				axis2D.TickmarksMinorVisible = modelAxis.TickmarksMinorVisible;
				axis2D.TickmarksCrossAxis = modelAxis.TickmarksCrossAxis;
				if (modelAxis.Title != null) {
					axis2D.Title = new AxisTitle();
					axis2D.Title.Content = modelAxis.Title.Text;
					axis2D.Title.Visible = modelAxis.Title.Visible;
				}
				if (modelAxis.Range != null) {
					axis2D.WholeRange = new Range();
					axis2D.WholeRange.SideMarginsValue = 0;
					if (modelAxis.Range.MinValue != null)
						axis2D.WholeRange.MinValue = modelAxis.Range.MinValue;
					if (modelAxis.Range.MaxValue != null)
						axis2D.WholeRange.MaxValue = modelAxis.Range.MaxValue;
				}
				axis2D.Reverse = modelAxis.Reverse;
				axis2D.Logarithmic = modelAxis.Logarithmic;
				if (modelAxis.Logarithmic)
					axis2D.LogarithmicBase = modelAxis.LogarithmicBase;
			}
		}
		public void Configure(XYDiagram2D diagram) {
			diagram.AxisX = new AxisX2D();
			diagram.AxisY = new AxisY2D();
			ConfigureAxis(diagram.AxisX, ChartModel.ArgumentAxis);
			ConfigureAxis(diagram.AxisY, ChartModel.ValueAxis);
			diagram.SecondaryAxesX.Clear();
			diagram.SecondaryAxesY.Clear();
			ConfigurateSecondaryArgumentAxes(diagram.SecondaryAxesX, ChartModel.SecondaryArgumentAxes);
			ConfigurateSecondaryValueAxes(diagram.SecondaryAxesY, ChartModel.SecondaryValueAxes);
		}
	}
}
