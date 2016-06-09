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

namespace DevExpress.Xpf.Charts.Native {
	public static class ObsoleteMessages {
		public const string
			RangePropertyCircularAxis = "The Range property is obsolete now. Use the WholeRange property instead.",
			ScrollingRangeProperty = "This property is obsolete now. To specify a custom range, use the Axis2D.VisualRange and AxisBase.WholeRange properties instead. For more information, see the corresponding topic in the documentation.",
			RangeProperty = "This property is obsolete now. To specify a custom range, use the Axis2D.VisualRange and AxisBase.WholeRange properties instead. For more information, see the corresponding topic in the documentation.",
			ActualScrollingRangeProperty = "This property is obsolete now. To specify a custom range, use the AxisBase.ActualWholeRange and Axis2D.ActualVisualRange properties instead. For more information, see the corresponding topic in the documentation.",
			ActualRangeProperty = "This property is obsolete now. To specify a custom range, use the AxisBase.ActualWholeRange and Axis2D.ActualVisualRange properties instead. For more information, see the corresponding topic in the documentation.",
			AxisRangeClass = "The AxisRange class is obsolete now. Use the Range class instead.",
			SeriesLabelVisibleProperty = "The SeriesLabel.Visible property is obsolete now. Use the Series.LabelsVisibility property instead.",
			Axis2DCrosshairLabelPatternProperty = "The Axis2D.CrosshairLabelPattern property is obsolete now. Use the CrosshairAxisLabelOptions.Pattern property instead.",
			Axis2DCrosshairLabelVisibilityProperty = "The Axis2D.CrosshairLabelVisibility property is obsolete now. Use the CrosshairAxisLabelOptions.Visibility property instead.",
			EnableAnimationProperty = "The ChartControl.EnableAnimation property is obsolete now. Use the ChartControl.AnimationMode property instead.",
			AxisLabelBeginTextProperty = "The AxisLabel.BeginText property is obsolete now. Use the AxisLabel.TextPattern property instead.",
			AxisLabelEndTextProperty = "The AxisLabel.EndText property is obsolete now. Use the AxisLabel.TextPattern property instead.",
			AxisNumericOptionsProperty = "The AxisBase.NumericOptions property is obsolete now. Use the AxisLabel.TextPattern property instead.",
			AxisDateTimeOptionsProperty = "The Axis.DateTimeOptions property is obsolete now. Use the AxisLabel.TextPattern property instead.",
			CircularAxisY2DDateTimeOptionsProperty = "The CircularAxisY2D.DateTimeOptions property is obsolete now. Use the AxisLabel.TextPattern property instead.",
			RadarAxisX2DDateTimeOptionsProperty = "The RadarAxisX2D.DateTimeOptions property is obsolete now. Use the AxisLabel.TextPattern property instead.",
			LegendPointOptionsProperty = "The Series.LegendPointOptions property is obsolete now. Use the Series.LegendTextPattern property instead.",
			PointOptionsProperty = "The Series.PointOptions property is obsolete now. Use the SeriesLabel.TextPattern property instead.",
			PercentOptionsProperty = "The PercentOptions propery is obsolete now. Use the SeriesLabel.TextPattern property instead.",
			ValueToDisplayProperty = "The ValueToDisplay propery is obsolete now. Use the SeriesLabel.TextPattern property instead.",
			AxisGridSpacingProperty = "The Axis.GridSpacing property is now obsolete.  Use the Axis.ContinuousNumericScaleOptions.GridSpacing, AxisX.ManualNumericScaleOptions.GridSpacing, Axis.ContinuousDateTimeScaleOptions.GridSpacing, or AxisX.ManualDateTimeScaleOptions.GridSpacing property instead.",
			AxisDateTimeMeasureUnitProperty = "The Axis.DateTimeMeasureUnit property is now obsolete. Use the AxisX.ManualDateTimeScaleOptions.MeasureUnit property instead.",
			AxisDateTimeGridAlignmentProperty = "The DateTimeGridAlignment property is now obsolete. Use the GridAlignment property of the ContinuousDateTimeScaleOptions or ManualDateTimeScaleOptions object assigned to the DateTimeScaleOptions property instead of the DateTimeGridAlignment property.",
			CrosshairElementsProperty = "The CrosshairElements propery is obsolete now. Use the CrosshairElementGroup.CrosshairElements property instead.",
			CrosshairGroupHeaderElementsProperty = "The CrosshairGroupHeaderElements propery is obsolete now. Use the CrosshairElementGroup.HeaderElement property instead.";
	}
}
