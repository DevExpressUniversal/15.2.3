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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(RangeArea3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RangeArea3DSeriesView : Area3DSeriesView {
		const int valueCount = 2;
		protected override Type PointInterfaceType {
			get {
				return typeof(IRangePoint);
			}
		}
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnRangeArea3D); } }
		protected internal override int PointDimension { get { return valueCount; } }
		protected internal override ValueLevel[] SupportedValueLevels {
			get { return new ValueLevel[] { ValueLevel.Value_1, ValueLevel.Value_2 }; }
		}
		protected override byte DefaultOpacity { get { return ConvertBetweenOpacityAndTransparency(0); } }
		public RangeArea3DSeriesView() : base() {
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			Line3DDrawOptions lineDrawOptions = seriesPointDrawOptions as Line3DDrawOptions;
			if (lineDrawOptions == null)
				return;
			renderer.EnablePolygonAntialiasing(true);
			StripsUtils.Render(renderer, RangeAreaSeriesViewPainter.CreateRangeAreaLegendMarkerStrip(this, bounds, false),
				LegendFillStyle.Options, lineDrawOptions.Color, lineDrawOptions.ActualColor2, new SeriesHitTestState(), null, selectionState);
			renderer.RestorePolygonAntialiasing();
		}
		protected internal override MinMaxValues GetSeriesPointValues(RefinedPoint refinedPoint) {
			double value1 = ((IRangePoint)refinedPoint).Value1;
			double value2 = ((IRangePoint)refinedPoint).Value2;
			return new MinMaxValues(value1, value2);
		}
		protected internal override PointOptions CreatePointOptions() {
			return new RangeAreaPointOptions();
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new RangeArea3DSeriesLabel();
		}
		protected override ChartElement CreateObjectForClone() {
			return new RangeArea3DSeriesView();
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new RangeAreaGeometryStripCreator();
		}
		public override string GetValueCaption(int index) {
			if (index >= PointDimension)
				throw new IndexOutOfRangeException();
			return ChartLocalizer.GetString(ChartStringId.ValueMember) + " " + (index + 1).ToString();
		}
	}
}
