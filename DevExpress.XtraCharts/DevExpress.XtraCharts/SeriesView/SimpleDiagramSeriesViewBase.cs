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
using System.Drawing;
using System.Drawing.Design;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraCharts {
	public abstract class SimpleDiagramSeriesViewBase : SeriesViewBase, ISupportTextAntialiasing {
		protected static PointF CalculateBasePoint(RectangleF bounds) {
			return new PointF(bounds.X + bounds.Width / 2.0f, bounds.Y + bounds.Height / 2.0f);
		}
		SeriesTitleCollection titles;
		protected internal override bool DateTimeValuesSupported { get { return false; } }
		protected internal override bool ActualColorEach { get { return true; } }
		protected internal virtual int ActualBorderThickness { get { return 0; } }
		protected internal override string DefaultPointToolTipPattern {
			get {
				return PatternParser.FullStackedToolTipPattern(GetDefaultArgumentFormat(), GetDefaultFormat(Series.ValueScaleType));				
			}
		}
		internal Color ExclamationTextColor {
			get {
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this);
				return actualAppearance == null ? Color.Black : actualAppearance.WholeChartAppearance.TitleColor;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SimpleDiagramSeriesViewBaseTitles"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SimpleDiagramSeriesViewBase.Titles"),
		Category(Categories.Elements),
		TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.SeriesTitleEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public SeriesTitleCollection Titles { get { return titles; } }
		protected SimpleDiagramSeriesViewBase() : base() {
			titles = new SeriesTitleCollection(this);
		}
		#region ISupportTextAntialiasing implementation
		DefaultBoolean ISupportTextAntialiasing.EnableAntialiasing { get { return DefaultBoolean.Default; } }
		bool ISupportTextAntialiasing.DefaultAntialiasing { get { return true; } }
		bool ISupportTextAntialiasing.Rotated { get { return false; } }
		Color ISupportTextAntialiasing.TextBackColor { get { return Color.Empty; } }
		RectangleFillStyle ISupportTextAntialiasing.TextBackFillStyle { get { return RectangleFillStyle.Empty; } }
		ChartElement ISupportTextAntialiasing.BackElement { get { return this; } }
		#endregion
		#region XtraSerializing
		protected override void SetIndexCollectionItemOnXtraDeserializing(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == "Titles")
				Titles.Add((SeriesTitle)e.Item.Value);
			else
				base.SetIndexCollectionItemOnXtraDeserializing(propertyName, e);
		}
		protected override object CreateCollectionItemOnXtraDeserializing(string propertyName, XtraItemEventArgs e) {
			if(propertyName == "Titles")
				return new SeriesTitle();
			return base.CreateCollectionItemOnXtraDeserializing(propertyName, e);
		}
		#endregion
		#region ShouldSerialize & Reset
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				Titles.Count > 0;
		}
		#endregion
		protected override PatternDataProvider GetDataProvider(PatternConstants patternConstant) {
			if (Series != null)
				switch (patternConstant) {
					case PatternConstants.Argument:
					case PatternConstants.Value:
					case PatternConstants.PercentValue:
					case PatternConstants.PointHint:
						return CreatePatternDataProvider(patternConstant);
					case PatternConstants.Series:
					case PatternConstants.SeriesGroup:
						return new SeriesPatternDataProvider(patternConstant);
				}
			return null;
		}
		protected virtual PatternDataProvider CreatePatternDataProvider(PatternConstants patternConstant) {
			return new SimplePointPatternDataProvider(patternConstant);
		}
		protected override SeriesContainer CreateContainer() {
			return new SimpleSeriesContainer(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				titles.Dispose();
			base.Dispose(disposing);
		}
		protected abstract DiagramPoint? CalculateAnnotationAchorPoint(ISimpleDiagramDomain domain, SeriesPointLayout pointLayout);
		protected abstract NormalizedValuesCalculator CreateNormalizedCalculator(ISeries series); 
		protected override Rectangle CorrectLegendMarkerBounds(Rectangle bounds) {
			bounds.X++;
			bounds.Y++;
			bounds.Width -= 2;
			bounds.Height -= 2;
			return bounds;
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		protected internal virtual bool CanDrag(ChartHitInfo hitInfo) {
			return false;
		}
		protected internal virtual bool PerformDragging(RefinedPoint refinedPoint, int dx, int dy) {
			return false;
		}
		protected internal virtual string GetDesignerHint() {
			return String.Empty;
		}
		protected internal override bool Contains(object obj) {
			return base.Contains(obj) || titles.Contains(obj);
		}
		protected internal override ToolTipPointDataToStringConverter CreateToolTipValueToStringConverter() {
			return new ToolTipSimpleDiagramValueToStringConverter(Series);
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.PercentViewPointPatterns;
		}
		protected internal abstract bool CalculateBounds(RefinedSeriesData seriesData, Rectangle outerBounds, out Rectangle innerBounds, out Rectangle labelBounds);
		protected internal abstract IEnumerable<SeriesPointLayout> CalculatePointsLayout(ISimpleDiagramDomain domain, RefinedSeriesData seriesData);
		protected internal List<AnnotationLayout> CalculateAnnotationsAchorPointsLayout(ISimpleDiagramDomain domain, SeriesPointLayout pointLayout) {
			List<AnnotationLayout> result = new List<AnnotationLayout>();
			SeriesPoint seriesPoint = pointLayout.SeriesPoint as SeriesPoint;
			if (seriesPoint != null && seriesPoint.Annotations.Count > 0) {
				DiagramPoint? anchorPoint = CalculateAnnotationAchorPoint(domain, pointLayout);
				if (anchorPoint.HasValue)
					foreach (Annotation annotation in seriesPoint.Annotations) {
						if (annotation.ScrollingSupported || domain.Bounds.Contains((Point)anchorPoint.Value))
							result.Add(new AnnotationLayout(annotation, anchorPoint.Value, pointLayout.PointData.RefinedPoint));
					}
			}
			return result;
		}
		public override string GetValueCaption(int index) {
			if (index > 0)
				throw new IndexOutOfRangeException();
			return ChartLocalizer.GetString(ChartStringId.ValueMember);
		}
		public override void Assign(ChartElement element) {
			base.Assign(element);
			SimpleDiagramSeriesViewBase view = element as SimpleDiagramSeriesViewBase;
			if(view == null)
				return;
			this.titles.Assign(view.titles);
		}
	}
}
