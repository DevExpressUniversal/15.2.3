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
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(RegressionLineTypeConverter))
	]
	public class RegressionLine : SingleLevelIndicator {
		public override string IndicatorName { get { return ChartLocalizer.GetString(ChartStringId.IndRegressionLine); } }
		public RegressionLine(string name, ValueLevel valueLevel) 
			: base(name, valueLevel) { }
		public RegressionLine(string name) 
			: base(name) { }
		public RegressionLine(ValueLevel valueLevel) 
			: this(string.Empty, valueLevel) { }
		public RegressionLine() 
			: this(string.Empty) { }
		protected override ChartElement CreateObjectForClone() {
			return new RegressionLine();
		}
		protected override IndicatorBehavior CreateBehavior() {
			return new RegressionLineBehavior(this);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class RegressionLineBehavior : IndicatorBehavior {
		GRealPoint2D p1, p2;
		RegressionLineCalculator calculator = new RegressionLineCalculator();
		RegressionLine RegressionLine { get { return (RegressionLine)Indicator; } }
		internal GRealPoint2D P1 { get { return p1; } }
		internal GRealPoint2D P2 { get { return p2; } }
		public RegressionLineBehavior(RegressionLine regressionLine) 
			: base(regressionLine) { }
		protected override void Calculate(IRefinedSeries refinedSeries) {
			Visible = false;
			if (!RegressionLine.Visible)
				return;
			double x1 = refinedSeries.MinArgument;
			double x2 = refinedSeries.MaxArgument;
			Tuple<GRealPoint2D, GRealPoint2D> linePoints = calculator.Calculate(refinedSeries, x1, x2, (ValueLevelInternal)RegressionLine.ValueLevel);
			if (linePoints != null) {
				p1 = linePoints.Item1;
				p2 = linePoints.Item2;
				Visible = true;
			}
			else
				Visible = false;
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!RegressionLine.Visible) {
				Visible = false;
				return null;
			}
			double yMin = RegressionLine.View.ActualAxisY.VisualRangeData.Min;
			double yMax = RegressionLine.View.ActualAxisY.VisualRangeData.Max;
			Visible = !((p1.Y < yMin && p2.Y < yMin) || (p1.Y > yMax && p2.Y > yMax));
			if (!Visible)
				return null;
			GRealLine2D? line = TruncatedLineCalculator.Truncate(diagramMapping.GetScreenPointNoRound(p1.X, p1.Y),
				diagramMapping.GetScreenPointNoRound(p2.X, p2.Y), diagramMapping.Bounds, LineKind.Line);
			return line.HasValue ? new LineIndicatorLayout(RegressionLine, line.Value) : null;
		}
	}
}
