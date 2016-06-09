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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using Model = DevExpress.Charts.Model;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public class ModelAxisLabelFormatter : IAxisLabelFormatter {
		readonly IAxisLabelFormatterCore formatter;
		public ModelAxisLabelFormatter(IAxisLabelFormatterCore formatter) {
			this.formatter = formatter;
		}
		public string GetAxisLabelText(object axisValue) {
			return formatter.GetAxisLabelText(axisValue);
		}
	}
	public abstract class AxisBaseConfigurator {
		readonly DiagrammAppearanceConfiguratorBase appearanceConfigurator;
		public AxisBaseConfigurator(DiagrammAppearanceConfiguratorBase appearanceConfigurator) {
			this.appearanceConfigurator = appearanceConfigurator;
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
		protected void ConfigureAxisBase(Model.AxisBase modelAxis, AxisBase axis, Model.IModelElementContainer container) {
			if (modelAxis != null) {
				container.Register(axis, modelAxis);
				axis.GridLines.Visible = modelAxis.GridLinesVisible;
				axis.GridLines.MinorVisible = modelAxis.GridLinesMinorVisible;
				if (modelAxis.Label != null) {
					axis.Label.Visible = modelAxis.Label.Visible;
					axis.Label.EnableAntialiasing = modelAxis.Label.EnableAntialiasing;
					if (modelAxis.Label.Formatter != null)
						axis.Label.Formatter = new ModelAxisLabelFormatter(modelAxis.Label.Formatter);
				}
				if (modelAxis.Range != null) {
					axis.WholeRange.SideMarginsValue = 0;
					if (modelAxis.Range.MinValue != null)
						axis.WholeRange.MinValue = modelAxis.Range.MinValue;
					if (modelAxis.Range.MaxValue != null)
						axis.WholeRange.MaxValue = modelAxis.Range.MaxValue;
					axis.VisualRange.SideMarginsValue = 0;
					if (modelAxis.Range.MinValue != null)
						axis.VisualRange.MinValue = modelAxis.Range.MinValue;
					if (modelAxis.Range.MaxValue != null)
						axis.VisualRange.MaxValue = modelAxis.Range.MaxValue;
				}
			}
		}
		protected abstract void ConfigureAxisCore(Model.AxisBase modelAxisBase, AxisBase axisBase, Model.IModelElementContainer container);
		protected void ConfigureAxis(Model.AxisBase modelAxis, AxisBase axis, Model.IModelElementContainer container) {
			ConfigureAxisCore(modelAxis, axis, container);
			if (modelAxis != null && modelAxis.Appearance != null)
				appearanceConfigurator.ConfigureAxis(axis, modelAxis.Appearance);
		}
	}
	public class AxesConfigurator : AxisBaseConfigurator {
		static readonly Dictionary<Model.AxisPosition, AxisAlignment> alignmentDict = new Dictionary<Model.AxisPosition, AxisAlignment>();
		static AxesConfigurator() {
			alignmentDict[Model.AxisPosition.Left] = AxisAlignment.Near;
			alignmentDict[Model.AxisPosition.Top] = AxisAlignment.Far;
			alignmentDict[Model.AxisPosition.Right] = AxisAlignment.Far;
			alignmentDict[Model.AxisPosition.Bottom] = AxisAlignment.Near;
		}
		public AxesConfigurator(DiagrammAppearanceConfiguratorBase appearanceConfigurator) : base(appearanceConfigurator) {
		}
		void ConfigurateSecondatyArgumentAxes(SecondaryAxisXCollection targetAxes, IList<Model.Axis> modelAxes, Model.IModelElementContainer container) {
			foreach (Model.Axis item in modelAxes) {
				SecondaryAxisX axisX = new SecondaryAxisX();
				ConfigureAxis(item, axisX, container);
				targetAxes.Add(axisX);
			}
		}
		void ConfigurateSecondatyValueAxes(SecondaryAxisYCollection targetAxes, IList<Model.Axis> modelAxes, Model.IModelElementContainer container) {
			foreach (Model.Axis item in modelAxes) {
				SecondaryAxisY axisY = new SecondaryAxisY();
				ConfigureAxis(item, axisY, container);
				targetAxes.Add(axisY);
			}
		}
		internal AxisAlignment CalculateAxisAlignment(Model.AxisPosition pos) {
			AxisAlignment align = AxisAlignment.Near;
			if (alignmentDict.TryGetValue(pos, out align))
				return align;
			return AxisAlignment.Near;
		}
		protected override void ConfigureAxisCore(Model.AxisBase modelAxisBase, AxisBase axisBase, Model.IModelElementContainer container) {
			ConfigureAxisBase(modelAxisBase, axisBase, container);
			Axis axis = axisBase as Axis;
			Model.Axis modelAxis = modelAxisBase as Model.Axis;
			if (modelAxis != null && axis != null) {
				axis.Visibility = DefaultBooleanUtils.ToDefaultBoolean(modelAxis.Visible);
				axis.Alignment = CalculateAxisAlignment(modelAxis.Position);
				axis.Tickmarks.Visible = modelAxis.TickmarksVisible;
				axis.Tickmarks.MinorVisible = modelAxis.TickmarksMinorVisible;
				axis.Tickmarks.CrossAxis = modelAxis.TickmarksCrossAxis;
				axis.ActualScaleOptions.AutoGrid = modelAxis.AutoGrid;
				if (!modelAxis.AutoGrid) {
					NumericScaleOptions numericOptions = axis.ActualScaleOptions as NumericScaleOptions;
					if (numericOptions != null) {
						axis.ActualScaleOptions.GridSpacing = 1.0;
						numericOptions.GridAlignment = NumericGridAlignment.Custom;
						numericOptions.CustomGridAlignment = modelAxis.GridSpacing;
						numericOptions.AggregateFunction = AggregateFunction.None;
					}
					DateTimeScaleOptions dateTimeOptions = axis.ActualScaleOptions as DateTimeScaleOptions;
					if (dateTimeOptions != null) {
						axis.ActualScaleOptions.GridSpacing = modelAxis.GridSpacing;
						dateTimeOptions.MeasureUnit = DateTimeMeasureUnit.Day;
						dateTimeOptions.GridAlignment = (DateTimeGridAlignment)modelAxis.GridAlignment;
						dateTimeOptions.AggregateFunction = AggregateFunction.None;
					}
				}
				if (modelAxis.Title != null) {
					axis.Title.Text = modelAxis.Title.Text;
					axis.Title.EnableAntialiasing = modelAxis.Title.EnableAntialiasing;
					axis.Title.Visibility = DefaultBooleanUtils.ToDefaultBoolean(modelAxis.Title.Visible);
				}
				axis.Reverse = modelAxis.Reverse;
				axis.Logarithmic = modelAxis.Logarithmic;
				if (modelAxis.Logarithmic)
					axis.LogarithmicBase = modelAxis.LogarithmicBase;
			}
		}		
		public void Configure(XYDiagram diagram, Model.CartesianChart chart, Model.IModelElementContainer container) {
			if (chart == null || diagram == null)
				return;
			ConfigureAxis(chart.ArgumentAxis, diagram.AxisX, container);
			ConfigureAxis(chart.ValueAxis, diagram.AxisY, container);
			diagram.SecondaryAxesX.Clear();
			diagram.SecondaryAxesY.Clear();
			ConfigurateSecondatyArgumentAxes(diagram.SecondaryAxesX, chart.SecondaryArgumentAxes, container);
			ConfigurateSecondatyValueAxes(diagram.SecondaryAxesY, chart.SecondaryValueAxes, container);
		}
	}
	public class RadarAxesConfigurator : AxisBaseConfigurator {
		public RadarAxesConfigurator(DiagrammAppearanceConfiguratorBase appearanceConfigurator) : base(appearanceConfigurator) {
		}
		void ConfigureAxisX(Model.RadarAxisX modelAxis, RadarAxisX axis, Model.IModelElementContainer container) {
			if (modelAxis != null) {
				ConfigureAxisBase(modelAxis, axis, container);
				axis.ActualScaleOptions.AutoGrid = modelAxis.AutoGrid;
				if (!modelAxis.AutoGrid) {
					NumericScaleOptions numericOptions = axis.ActualScaleOptions as NumericScaleOptions;
					if (numericOptions != null) {
						axis.ActualScaleOptions.GridSpacing = 1.0;
						numericOptions.GridAlignment = NumericGridAlignment.Custom;
						numericOptions.CustomGridAlignment = modelAxis.GridSpacing;
					}
				}
				axis.Logarithmic = modelAxis.Logarithmic;
				if (modelAxis.Logarithmic)
					axis.LogarithmicBase = modelAxis.LogarithmicBase;
			}
		}
		protected void ConfigureAxisY(Model.CircularAxisY modelAxis, RadarAxisY axis, Model.IModelElementContainer container) {
			if (modelAxis != null) {
				ConfigureAxisBase(modelAxis, axis, container);
				axis.Visible = modelAxis.Visible;
				axis.Tickmarks.Visible = modelAxis.TickmarksVisible;
				axis.Tickmarks.MinorVisible = modelAxis.TickmarksMinorVisible;
				axis.Tickmarks.CrossAxis = modelAxis.TickmarksCrossAxis;
				axis.ActualScaleOptions.AutoGrid = modelAxis.AutoGrid;
				if (!modelAxis.AutoGrid) {
					NumericScaleOptions numericOptions = axis.ActualScaleOptions as NumericScaleOptions;
					if (numericOptions != null) {
						axis.ActualScaleOptions.GridSpacing = 1.0;
						numericOptions.GridAlignment = NumericGridAlignment.Custom;
						numericOptions.CustomGridAlignment = modelAxis.GridSpacing;
					}
					DateTimeScaleOptions dateTimeOptions = axis.ActualScaleOptions as DateTimeScaleOptions;
					if (dateTimeOptions != null) {
						axis.ActualScaleOptions.GridSpacing = modelAxis.GridSpacing;
						dateTimeOptions.MeasureUnit = (DateTimeMeasureUnit)modelAxis.GridAlignment;
						dateTimeOptions.GridAlignment = (DateTimeGridAlignment)modelAxis.GridAlignment;
					}
				}
				axis.Logarithmic = modelAxis.Logarithmic;
				if (modelAxis.Logarithmic)
					axis.LogarithmicBase = modelAxis.LogarithmicBase;
			}
		}
		protected override void ConfigureAxisCore(Model.AxisBase modelAxisBase, AxisBase axisBase, Model.IModelElementContainer container) {
			if (axisBase is RadarAxisX)
				ConfigureAxisX((Model.RadarAxisX)modelAxisBase, (RadarAxisX)axisBase, container);
			if (axisBase is RadarAxisY)
				ConfigureAxisY((Model.CircularAxisY)modelAxisBase, (RadarAxisY)axisBase, container);
		}
		public void Configure(RadarDiagram diagram, Model.RadarChart chart, Model.IModelElementContainer container) {
			if (chart == null || diagram == null)
				return;
			ConfigureAxis(chart.ArgumentAxis, diagram.AxisX, container);
			ConfigureAxis(chart.ValueAxis, diagram.AxisY, container);
		}
	}
	public class PolarAxesConfigurator : RadarAxesConfigurator {
		public PolarAxesConfigurator(DiagrammAppearanceConfiguratorBase appearanceConfigurator) : base(appearanceConfigurator) {
		}
		void ConfigureAxisX(Model.PolarAxisX modelAxis, PolarAxisX axis, Model.IModelElementContainer container) {
				ConfigureAxisBase(modelAxis, axis, container);
		}
		protected override void ConfigureAxisCore(Model.AxisBase modelAxisBase, AxisBase axisBase, Model.IModelElementContainer container) {
			if (axisBase is RadarAxisX)
				ConfigureAxisX((Model.PolarAxisX)modelAxisBase, (PolarAxisX)axisBase, container);
			if (axisBase is RadarAxisY)
				ConfigureAxisY((Model.CircularAxisY)modelAxisBase, (RadarAxisY)axisBase, container);
		}
		public void Configure(PolarDiagram diagram, Model.PolarChart chart, Model.IModelElementContainer container) {
			if (chart == null || diagram == null)
				return;
			ConfigureAxis(chart.ArgumentAxis, (PolarAxisX)diagram.AxisX, container);
			ConfigureAxis(chart.ValueAxis, diagram.AxisY, container);
		}
	}
}
