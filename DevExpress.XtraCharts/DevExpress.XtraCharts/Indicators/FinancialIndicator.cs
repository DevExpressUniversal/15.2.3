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
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FinancialIndicatorPoint : ChartElement {
		const ValueLevel DefaultValueLevel = ValueLevel.Value;
		object argument;
		string argumentSerializable;
		ValueLevel valueLevel = DefaultValueLevel;
		internal FinancialIndicator FinancialIndicator { get { return (FinancialIndicator)Owner; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FinancialIndicatorPointArgument"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FinancialIndicatorPoint.Argument"),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(FinancialIndicatorPointArgumentTypeConverter))
		]
		public object Argument {
			get { return argument; }
			set {
				if (value != argument) {
					if (value == null)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgNullFinancialIndicatorArgument));
					SendNotification(new ElementWillChangeNotification(this));
					argument = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string ArgumentSerializable {
			get { return SerializingUtils.ConvertToSerializable(argument); }
			set { 
				Argument = value; 
				if (Loading)
					argumentSerializable = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FinancialIndicatorPointValueLevel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FinancialIndicatorPoint.ValueLevel"),
		NonTestableProperty,
		XtraSerializableProperty,
		TypeConverter(typeof(ValueLevelTypeConterter))
		]
		public ValueLevel ValueLevel {
			get { return valueLevel; }
			set {
				if (value != valueLevel) {
					if (!Loading) {
						XYDiagram2DSeriesViewBase view = FinancialIndicator.View;
						if (view != null)
							view.CheckValueLevel(value);
					}
					SendNotification(new ElementWillChangeNotification(this));
					valueLevel = value;
					RaiseControlChanged();
				}
			}
		}
		internal FinancialIndicatorPoint(FinancialIndicator financialIndicator) : base(financialIndicator) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeValueLevel() {
			return valueLevel != DefaultValueLevel;
		}
		void ResetValueLevel() {
			ValueLevel = DefaultValueLevel;
		}
		protected internal override bool ShouldSerialize() {
			return true;
		} 
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "ValueLevel" ? ShouldSerializeValueLevel() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		internal void Validate(XYDiagram2DSeriesViewBase view, IRefinedSeries refinedSeries, bool validateAsMaxValue) {
			try {
				view.CheckValueLevel(valueLevel);
			}
			catch {
				valueLevel = view.DefaultValueLevel;
			}
			Axis2D axisX = view.ActualAxisX;
			if (axisX != null) {
				if (argument == null) {
					Scale scaleType = (Scale)axisX.ScaleType;
					RefinedPoint pointForValidate = null;
					if (refinedSeries != null) {
						if (validateAsMaxValue) {
							double maxArgument = Double.MinValue;
							foreach (RefinedPoint refinedPoint in refinedSeries.Points)
								if (!refinedPoint.IsEmpty) {
									double internalArgument = refinedPoint.Argument;
									if (internalArgument > maxArgument) {
										maxArgument = internalArgument;
										pointForValidate = refinedPoint;
									}
								}
						}
						else {
							double minArgument = Double.MaxValue;
							foreach (RefinedPoint refinedPoint in refinedSeries.Points)
								if (!refinedPoint.IsEmpty) {
									double internalArgument = refinedPoint.Argument;
									if (internalArgument < minArgument) {
										minArgument = internalArgument;
										pointForValidate = refinedPoint;
									}
								}
						}
					}
					if (pointForValidate == null)
						switch (scaleType) {
							case Scale.Numerical:
								argument = 0.0;
								break;
							case Scale.DateTime:
								argument = DateTimeUtils.Floor(DateTime.Now, (DateTimeMeasureUnitNative)axisX.DateTimeScaleOptions.MeasureUnit);
								break;
							default:
								argument = String.Empty;
								break;
						}
					else
						argument = pointForValidate.SeriesPoint.UserArgument;
				}
				else {
					CultureInfo culture = (argumentSerializable != null && argumentSerializable.Equals(argument)) ? 
						CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
					argument = ((IAxisData)axisX).AxisScaleTypeMap.ConvertValue(argument, culture);
					argumentSerializable = null;
				}
			}
		}
		protected override ChartElement CreateObjectForClone() {
			return new FinancialIndicatorPoint(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FinancialIndicatorPoint financialIndicatorPoint = obj as FinancialIndicatorPoint;
			if (financialIndicatorPoint != null) {
				argument = financialIndicatorPoint.argument;			
				argumentSerializable = null;
				valueLevel = financialIndicatorPoint.valueLevel;
			}
		}
	}
	public abstract class FinancialIndicator : Indicator {
		readonly FinancialIndicatorPoint point1;
		readonly FinancialIndicatorPoint point2;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FinancialIndicatorPoint1"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FinancialIndicator.Point1"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		NestedTagProperty
		]
		public FinancialIndicatorPoint Point1 { get { return point1; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FinancialIndicatorPoint2"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FinancialIndicator.Point2"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		NestedTagProperty
		]
		public FinancialIndicatorPoint Point2 { get { return point2; } }
		protected FinancialIndicator() : this(String.Empty) {
		}
		protected FinancialIndicator(string name) : base(name) {
			point1 = new FinancialIndicatorPoint(this);
			point2 = new FinancialIndicatorPoint(this);
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializePoint1() {
			return point1.ShouldSerialize();
		}
		bool ShouldSerializePoint2() {
			return point2.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializePoint1() || ShouldSerializePoint2();
		}
		#endregion                
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Point1":
					return ShouldSerializePoint1();
				case "Point2":
					return ShouldSerializePoint2();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected internal override void Validate(XYDiagram2DSeriesViewBase view, IRefinedSeries refinedSeries) {
			base.Validate(view, refinedSeries);
			if (!Loading) {
				point1.Validate(view, refinedSeries, false);
				point2.Validate(view, refinedSeries, true);
			}
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FinancialIndicator financialIndicator = obj as FinancialIndicator;
			if (financialIndicator != null) {
				point1.Assign(financialIndicator.point1);
				point2.Assign(financialIndicator.point2);
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public abstract class FinancialIndicatorBehavior : IndicatorBehavior {
		DiagramPoint minPoint;
		DiagramPoint maxPoint;
		protected DiagramPoint MinPoint { get { return minPoint; } }
		protected DiagramPoint MaxPoint { get { return maxPoint; } }
		public FinancialIndicatorBehavior(FinancialIndicator financialIndicator) : base(financialIndicator) {
		}
		protected override void Calculate(IRefinedSeries seriesInfo) {
			Visible = false;
			FinancialIndicator financialIndicator = Indicator as FinancialIndicator;
			if (financialIndicator != null && financialIndicator.Visible) {
				FinancialIndicatorPoint point1 = financialIndicator.Point1;
				FinancialIndicatorPoint point2 = financialIndicator.Point2;
				AxisScaleTypeMap map = ((IAxisData)financialIndicator.View.ActualAxisX).AxisScaleTypeMap;
				object argument1 = point1.Argument;
				object argument2 = point2.Argument;
				if (map.IsCompatible(argument1) && map.IsCompatible(argument2)) {
					double refinedArgument1 = map.NativeToRefined(argument1);
					double refinedArgument2 = map.NativeToRefined(argument2);
					if (refinedArgument1 != refinedArgument2 && !double.IsNaN(refinedArgument1) && !double.IsNaN(refinedArgument2)) {
						RefinedPoint minRefinedPoint;
						RefinedPoint maxRefinedPoint;
						ValueLevel minValueLevel;
						ValueLevel maxValueLevel;
						if (refinedArgument1 < refinedArgument2) {
							minRefinedPoint = seriesInfo.GetMinPoint(refinedArgument1);
							maxRefinedPoint = seriesInfo.GetMaxPoint(refinedArgument2);
							minValueLevel = point1.ValueLevel;
							maxValueLevel = point2.ValueLevel;
						}
						else {
							minRefinedPoint = seriesInfo.GetMinPoint(refinedArgument2);
							maxRefinedPoint = seriesInfo.GetMaxPoint(refinedArgument1);
							minValueLevel = point2.ValueLevel;
							maxValueLevel = point1.ValueLevel;
						}
						if (minRefinedPoint != null && maxRefinedPoint != null) {
							minPoint = new DiagramPoint(map.RefinedToInternal(minRefinedPoint.Argument),
								minRefinedPoint.GetValue((ValueLevelInternal)minValueLevel));
							maxPoint = new DiagramPoint(map.RefinedToInternal(maxRefinedPoint.Argument),
								maxRefinedPoint.GetValue((ValueLevelInternal)maxValueLevel));
							Visible = true;
						}
					}
				}
			}
		}
	}
}
