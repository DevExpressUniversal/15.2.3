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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public enum ChartHitTest {
		None,
		Chart,
		ChartTitle,
		Axis,
		ConstantLine,
		Diagram,
		Legend,
		Series,
		SeriesLabel,
		SeriesTitle,
		NonDefaultPane,
		[Obsolete("This field is obsolete now. Use the ChartHitTest.Indicator field instead.")]
		TrendLine,
		[Obsolete("This field is obsolete now. Use the ChartHitTest.Indicator field instead.")]
		FibonacciIndicator,
		[Obsolete("This field is obsolete now. Use the ChartHitTest.Indicator field instead.")]
		RegressionLine, 
		Annotation,
		Indicator
	}
	public class ChartHitInfo {
		readonly Point hitPoint;
		readonly object hitObject;
		readonly ChartHitTest hitTest;
		readonly IList<HitTestParams> hitTestParams;
		public Point HitPoint { get { return hitPoint; } }
		public object HitObject { get { return hitObject; } }
		public ChartHitTest HitTest { get { return hitTest; } }
		public object[] HitObjects {
			get {
				object[] hitObjects = new object[hitTestParams.Count];
				for (int i = 0; i < hitTestParams.Count; i++)
					hitObjects[i] = GetHitObjectByParams(hitTestParams[i]);
				return hitObjects;
			}
		}
		public bool InChart { get { return InHitObject(typeof(IChartContainer)); } }
		public bool InChartTitle { get { return InHitObject(typeof(ChartTitle)); } }
		public bool InAxis { get { return InHitObject(typeof(AxisBase)); } }
		public bool InConstantLine { get { return InHitObject(typeof(ConstantLine)); } }
		public bool InDiagram { get { return InHitObject(typeof(Diagram)); } }
		public bool InLegend { get { return InHitObject(typeof(Legend)); } }
		public bool InSeries { get { return InHitObject(typeof(SeriesBase)); } }
		public bool InSeriesLabel { get { return InHitObject(typeof(SeriesLabelBase)); } }
		public bool InSeriesTitle { get { return InHitObject(typeof(SeriesTitle)); } }
		public bool InNonDefaultPane { get { return InHitObject(typeof(XYDiagramPane)); } }
		public bool InIndicator { get { return InHitObject(typeof(Indicator)); } }
		public bool InAnnotation { get { return InHitObject(typeof(Annotation)); } }
		public bool InHyperlink { get { return InAdditionalHitObject(typeof(HyperlinkSource)); ; } }
		[Obsolete("This property is now obsolete. Use the InIndicator property instead.")]
		public bool InTrendLine { get { return InHitObject(typeof(TrendLine)); } }
		[Obsolete("This property is now obsolete. Use the InIndicator property instead.")]
		public bool InFibonacciIndicator { get { return InHitObject(typeof(FibonacciIndicator)); } }
		[Obsolete("This property is now obsolete. Use the InIndicator property instead.")]
		public bool InRegressionLine { get { return InHitObject(typeof(RegressionLine));} }
		public IChartContainer Chart { get { return GetHitObjectByType(typeof(IChartContainer)) as IChartContainer; } }
		public ChartTitle ChartTitle { get { return GetHitObjectByType(typeof(ChartTitle)) as ChartTitle; } }		
		public AxisBase Axis { get { return GetHitObjectByType(typeof(AxisBase)) as AxisBase; } }
		public ConstantLine ConstantLine {
			get {
				ConstantLine constantLine = GetHitObjectByType(typeof(ConstantLine)) as ConstantLine;
				return constantLine != null ? constantLine : GetLegendItemSource<ConstantLine>();
			}
		}
		public Diagram Diagram { get { return GetHitObjectByType(typeof(Diagram)) as Diagram; } }
		public Legend Legend { get { return GetHitObjectByType(typeof(Legend)) as Legend; } }
		public SeriesBase Series { 
			get {
				SeriesBase series = SeriesInsideDiagram;
				return series != null ? series : GetLegendItemSource<SeriesBase>();
			} 
		}
		public SeriesLabelBase SeriesLabel { get { return GetHitObjectByType(typeof(SeriesLabelBase)) as SeriesLabelBase; } }
		public SeriesTitle SeriesTitle { get { return GetHitObjectByType(typeof(SeriesTitle)) as SeriesTitle; } }
		public XYDiagramPane NonDefaultPane { get { return GetHitObjectByType(typeof(XYDiagramPane)) as XYDiagramPane; } }
		public string Hyperlink {
			get {
				HyperlinkSource linkSource = GetAdditionalHitObjectByType(typeof(HyperlinkSource)) as HyperlinkSource;
				return linkSource != null ? linkSource.Hyperlink : null;
			}
		}
		public Indicator Indicator {
			get {
				Indicator indicator = GetHitObjectByType(typeof(Indicator)) as Indicator;
				return indicator != null ? indicator : GetLegendItemSource<Indicator>();
			}
		}
		public Annotation Annotation { get { return GetHitObjectByType(typeof(Annotation)) as Annotation; } }
		internal SeriesBase SeriesInsideDiagram {
			get {
				SeriesBase series = GetHitObjectByType(typeof(SeriesBase)) as SeriesBase;
				if (series != null)
					return series;
				SeriesLabelBase label = GetHitObjectByType(typeof(SeriesLabelBase)) as SeriesLabelBase;
				return label == null ? null : label.SeriesBase;
			}
		}
		internal RefinedPoint RefinedPointInsideDiagram {
			get {
				HitTestParams hitParams = GetHitParamsByType(typeof(SeriesBase));
				if (hitParams == null)
					hitParams = GetHitParamsByType(typeof(SeriesLabelBase));
				if (hitParams == null)
					hitParams = GetHitParamsByType(typeof(Annotation));
				return hitParams == null ? null : hitParams.AdditionalObj as RefinedPoint;
			}
		}
		public SeriesPoint SeriesPoint {
			get {
				RefinedPoint refinedPoint = RefinedPointInsideDiagram;
				if (refinedPoint == null)
					refinedPoint = GetLegendItemSource<RefinedPoint>();
				return refinedPoint != null ? SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint) : null;				
			}
		}
		public AxisLabelItemBase AxisLabelItem {
			get {
				HitTestParams hitParams = GetHitParamsByType(typeof(AxisBase));
				return hitParams == null ? null : (hitParams.AdditionalObj as AxisLabelItemBase);
			}
		}
		public AxisTitle AxisTitle {
			get {
				HitTestParams hitParams = GetHitParamsByType(typeof(Axis));
				return hitParams == null ? null : (hitParams.AdditionalObj as AxisTitle);
			}
		}
		[Obsolete("This property is now obsolete. Use the Indicator property instead.")]
		public TrendLine TrendLine { get { return GetHitObjectByType(typeof(TrendLine)) as TrendLine; } }
		[Obsolete("This property is now obsolete. Use the Indicator property instead.")]
		public FibonacciIndicator FibonacciIndicator { get { return GetHitObjectByType(typeof(FibonacciIndicator)) as FibonacciIndicator; } }
		[Obsolete("This property is now obsolete. Use the Indicator property instead.")]
		public RegressionLine RegressionLine { get { return GetHitObjectByType(typeof(RegressionLine)) as RegressionLine; } }
		internal ChartHitInfo(Point hitPoint, IList<HitTestParams> hitTestParams) {
			this.hitPoint = hitPoint;
			this.hitTestParams = hitTestParams;
			hitObject = hitTestParams.Count > 0 ? GetHitObjectByParams(hitTestParams[0]) : null;
			hitTest = GetChartHitTest(hitObject);
		}
		object GetHitObjectByParams(HitTestParams hitParams) {
			IHitTest hitElement = hitParams.Object;
			object hitObject = hitElement.Object;
			Chart chart = hitObject as Chart;
			if (chart != null)
				hitObject = chart.ChartContainer;
			return hitObject;
		}
		TSource GetLegendItemSource<TSource>() where TSource : class {
			HitTestParams hitParams = GetHitParamsByType(typeof(Legend));
			return hitParams != null ? hitParams.AdditionalObj as TSource : null;
		}
		HitTestParams GetHitParamsByType(Type hitType) {
			foreach (HitTestParams hitParams in hitTestParams) {
				object hitObject = GetHitObjectByParams(hitParams);
				if (hitType.IsAssignableFrom(hitObject.GetType()))
					return hitParams;
			}
			return null;
		}
		object GetHitObjectByType(Type hitType) {
			HitTestParams hitParams = GetHitParamsByType(hitType);
			return hitParams == null ? null : GetHitObjectByParams(hitParams);
		}
		object GetAdditionalHitObjectByType(Type hitType) {
			foreach (HitTestParams hitParams in hitTestParams) {
				object hitObject = hitParams.AdditionalObj;
				if (hitObject != null && hitType.IsAssignableFrom(hitObject.GetType()))
					return hitObject;
			}
			return null;
		}
		bool InHitObject(Type hitType) {
			return GetHitObjectByType(hitType) != null;
		}
		bool InAdditionalHitObject(Type hitType) {
			return GetAdditionalHitObjectByType(hitType) != null;
		}
		ChartHitTest GetChartHitTest(object hitObject) {
			if (hitObject == null)
				return ChartHitTest.None;
			if (hitObject is IChartContainer)
				return ChartHitTest.Chart;
			if (hitObject is ChartTitle)
				return ChartHitTest.ChartTitle;
			if ((hitObject is Axis) || (hitObject is RadarAxis))
				return ChartHitTest.Axis;
			if (hitObject is ConstantLine)
				return ChartHitTest.ConstantLine;
			if (hitObject is Diagram)
				return ChartHitTest.Diagram;
			if (hitObject is Legend)
				return ChartHitTest.Legend;
			if (hitObject is SeriesBase)
				return ChartHitTest.Series;
			if (hitObject is SeriesLabelBase)
				return ChartHitTest.SeriesLabel;
			if (hitObject is SeriesTitle)
				return ChartHitTest.SeriesTitle;
			if (hitObject is XYDiagramPane)
				return ChartHitTest.NonDefaultPane;
			if (hitObject is Annotation)
				return ChartHitTest.Annotation;
			if (hitObject is Indicator)
				return ChartHitTest.Indicator;
			return ChartHitTest.None;
		}
	}
}
