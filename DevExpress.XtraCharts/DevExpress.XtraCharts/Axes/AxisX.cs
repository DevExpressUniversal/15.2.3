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
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum DateTimeScaleMode {
		Manual,
		AutomaticAverage,
		AutomaticIntegral
	}
	public abstract class AxisXBase : Axis {
		protected override int DefaultMinorCount { get { return Constants.AxisXDefaultMinorCount; } }
		protected override int GridSpacingFactor { get { return Constants.AxisXGridSpacingFactor; } }
		protected internal override bool IsValuesAxis { get { return false; } }
		protected internal override bool IsVertical { get { return ((XYDiagram)Diagram).Rotated; } }
		protected internal override ScaleMode ActualDateTimeScaleMode { get { return DateTimeScaleOptions.ScaleMode; } }
		internal bool ScrollingZoomingEnabled { get { return AxisNavigationUtils.IsScrollingZoomingEnabled(this); } }
		[
		Obsolete("This property is obsolete now. Use DateTimeScaleOptions.ScaleMode instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public virtual DateTimeScaleMode DateTimeScaleMode {
			get {
				if (DateTimeScaleOptions.ScaleMode == ScaleMode.Automatic) {
					switch (DateTimeScaleOptions.AggregateFunction) {
						case AggregateFunction.Average:
							return DateTimeScaleMode.AutomaticAverage;
						case AggregateFunction.Sum:
							return DateTimeScaleMode.AutomaticIntegral;
					}
				}
				return DateTimeScaleMode.Manual;
			}
			set {
				switch (value) {
					case DateTimeScaleMode.Manual:
						DateTimeScaleOptions.ScaleMode = ScaleMode.Manual;
						DateTimeScaleOptions.ProcessMissingPoints = ProcessMissingPointsMode.Skip;
						break;
					case DateTimeScaleMode.AutomaticAverage:
					case DateTimeScaleMode.AutomaticIntegral:
						DateTimeScaleOptions.ScaleMode = ScaleMode.Automatic;
						DateTimeScaleOptions.AggregateFunction = AggregateFunction.Average;
						DateTimeScaleOptions.ProcessMissingPoints = ProcessMissingPointsMode.InsertZeroValues;
						break;
				}
			}
		}
		protected AxisXBase(string name, XYDiagram diagram) : base(name, diagram) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeDateTimeScaleMode() {
			return false;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeDateTimeScaleMode();
		}
		#endregion
		#region XtraShouldSerialize
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "DateTimeScaleMode" ? ShouldSerializeDateTimeScaleMode() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected override Tickmarks CreateTickmarks() {
			return new TickmarksX(this);
		}
		protected override AxisTitle CreateAxisTitle() {
			return new AxisTitleX(this);
		}
		protected override AxisRange CreateAxisRange(RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange) {
			return new AxisXRange(this);
		}
		protected override IEnumerable<double> GetInitialValuesForAutomaticScaleBreakCalculation(ISeries series, RefinedPoint refinedPoint) {
			return new double[] { refinedPoint.Argument };
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(AxisXTypeConverter))
	]
	[RuntimeObject]
	public class AxisX : AxisXBase {
		static readonly string defaultName = ChartLocalizer.GetString(ChartStringId.PrimaryAxisXName);
		protected override AxisAlignment DefaultAlignment { get { return AxisAlignment.Near; } }
		protected override ChartElementVisibilityPriority Priority { get { return ChartElementVisibilityPriority.AxisX; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		internal AxisX(XYDiagram diagram) : base(defaultName, diagram) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisX((XYDiagram)null);
		}
		protected override GridLines CreateGridLines() {
			return new GridLinesX(this);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public static class AxisNavigationUtils {
		public static bool IsScrollingZoomingEnabled(XYDiagram2D diagram) {
			if (diagram.DefaultPane.ActualEnableAxisXScrolling || diagram.DefaultPane.ActualEnableAxisXZooming)
				return true;
			foreach (XYDiagramPaneBase pane in diagram.Panes)
				if (pane.ActualEnableAxisXScrolling || pane.ActualEnableAxisXZooming)
					return true;
			return false;
		}
		public static bool IsScrollingZoomingEnabled(AxisXBase axis) {
			XYDiagram diagram = axis.XYDiagram;
			if (diagram == null || diagram.AxisPaneRepository == null)
				return false;
			return diagram.AxisPaneRepository.IsScrollingZoomingEnabledForAxis(axis);
		}
	}
}
