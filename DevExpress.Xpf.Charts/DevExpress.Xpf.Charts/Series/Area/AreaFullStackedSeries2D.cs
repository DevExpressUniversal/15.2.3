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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[
	TemplatePart(Name = "PART_AdditionalGeometryHolder", Type = typeof(ChartContentPresenter)),
	TemplatePart(Name = "PART_PointsPanel", Type = typeof(SimplePanel))
	]
	public class AreaFullStackedSeries2D : AreaStackedSeries2D {
		public static readonly DependencyProperty PercentOptionsProperty = DependencyPropertyManager.RegisterAttached("PercentOptions",
				typeof(PercentOptions), typeof(AreaFullStackedSeries2D), new PropertyMetadata(PointOptions.PercentOptionsPropertyChanged));
		[Obsolete(ObsoleteMessages.PercentOptionsProperty), EditorBrowsable(EditorBrowsableState.Never)]
		public static PercentOptions GetPercentOptions(PointOptions pointOptions) {
			return (PercentOptions)pointOptions.GetValue(PercentOptionsProperty);
		}
		[Obsolete(ObsoleteMessages.PercentOptionsProperty), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetPercentOptions(PointOptions pointOptions, PercentOptions value) {
			pointOptions.SetValue(PercentOptionsProperty, value);
		}
		protected internal override bool IsLabelConnectorItemVisible { get { return false; } }
		protected internal override ResolveOverlappingMode LabelsResolveOverlappingMode {
			get {
				if (ActualLabel.ResolveOverlappingMode == ResolveOverlappingMode.JustifyAllAroundPoint ||
					ActualLabel.ResolveOverlappingMode == ResolveOverlappingMode.JustifyAroundPoint)
					return ResolveOverlappingMode.Default;
				return ActualLabel.ResolveOverlappingMode;
			}
		}
		protected internal override ToolTipPointDataToStringConverter ToolTipPointValuesConverter { get { return new ToolTipFullStackedValueToStringConverter(this); } }
		protected override Type PointInterfaceType { get { return typeof(IFullStackedPoint); } }
		protected override string DefaultLabelTextPattern { get { return "{" + PatternUtils.PercentValuePlaceholder + ":G4}"; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ColorEach {
			get { return base.ColorEach; }
			set { base.ColorEach = value; }
		}
		public AreaFullStackedSeries2D() {
			DefaultStyleKey = typeof(AreaFullStackedSeries2D);
		}
		protected override Series CreateObjectForClone() {
			return new AreaFullStackedSeries2D();
		}
		protected override XYSeriesLabel2DLayout CreateSeriesLabelLayout(SeriesLabelItem labelItem, PaneMapping mapping, Transform transform) {
			IStackedPoint point = labelItem.RefinedPoint;
			if (point != null) {
				Point? anchorPoint = RangeArea2DHelper.CalculateLabelAnchorPoint(this, mapping, labelItem, new MinMaxValues(point.MinValue, point.MaxValue), transform);
				if (anchorPoint.HasValue)
					return new XYSeriesLabel2DLayout(labelItem, mapping, anchorPoint.Value, GRect2D.Empty);
			}
			return null;
		}
		protected internal override Point CalculateToolTipPoint(SeriesPointItem pointItem, PaneMapping mapping, Transform transform, bool inLabel) {
			IFullStackedPoint stackedPoint = pointItem.RefinedPoint as IFullStackedPoint;
			MinMaxValues values = stackedPoint == null ? new MinMaxValues(stackedPoint.Value) : new MinMaxValues(stackedPoint.MinValue, stackedPoint.MaxValue);
			double centerValue = MathUtils.CalculateCenterValueByRange(((IAxisData)ActualAxisY).VisualRange, values);
			return Double.IsNaN(centerValue) ? new Point() : transform.Transform(mapping.GetRoundedDiagramPoint(stackedPoint.Argument, centerValue));
		}
		protected override SeriesContainer CreateContainer() {
			return new FullStackedInteractionContainer(this, true);
		}
		protected internal override string ConstructValuePattern(PointOptionsContainerBase pointOptionsContainer, ScaleType valueScaleType) {
			PercentOptions percentOptions = pointOptionsContainer.GetPercentOptions(PercentOptionsProperty);
			return pointOptionsContainer.ConstructValuePatternFromPercentOptions(percentOptions, valueScaleType);
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.PercentViewPointPatterns;
		}
	}
}
