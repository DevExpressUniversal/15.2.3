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
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum PieLabelPosition {
		Inside,
		Outside,
		TwoColumns
	}
	public abstract class PieSeries : Series {
		#region Dependency properties
		static readonly DependencyPropertyKey TitlesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Titles",
			typeof(TitleCollection), typeof(PieSeries), new PropertyMetadata());
		public static readonly DependencyProperty TitlesProperty = TitlesPropertyKey.DependencyProperty;
		public static readonly DependencyProperty HoleRadiusPercentProperty = DependencyPropertyManager.Register("HoleRadiusPercent",
			typeof(double), typeof(PieSeries), new FrameworkPropertyMetadata(50.0, OnHoleRadiusPercentPrpertyChanged), new ValidateValueCallback(HoleRadiusPercentValidation));
		public static readonly DependencyProperty ExplodedDistanceProperty = DependencyPropertyManager.RegisterAttached("ExplodedDistance",
			typeof(double), typeof(PieSeries), new FrameworkPropertyMetadata(OnExplodedDistancePropertyChanged), new ValidateValueCallback(ExplodedDistanceValidation));
		public static readonly DependencyProperty LabelPositionProperty = DependencyPropertyManager.RegisterAttached("LabelPosition",
			typeof(PieLabelPosition), typeof(PieSeries), new FrameworkPropertyMetadata(PieLabelPosition.Outside, ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty PercentOptionsProperty = DependencyPropertyManager.RegisterAttached("PercentOptions",
			typeof(PercentOptions), typeof(PieSeries), new FrameworkPropertyMetadata(PointOptions.PercentOptionsPropertyChanged));
		public static readonly DependencyProperty LabelsResolveOverlappingMinIndentProperty = DependencyPropertyManager.Register("LabelsResolveOverlappingMinIndent",
			typeof(int), typeof(PieSeries), new PropertyMetadata(-1, ChartElementHelper.UpdateWithClearDiagramCache));
		#endregion
		[Category(Categories.Layout),
		XtraSerializableProperty]
		public static double GetExplodedDistance(SeriesPoint point) {
			return (double)point.GetValue(ExplodedDistanceProperty);
		}
		[Category(Categories.Layout),
		XtraSerializableProperty]
		public static PieLabelPosition GetLabelPosition(SeriesLabel label) {
			return (PieLabelPosition)label.GetValue(LabelPositionProperty);
		}
		public static void SetExplodedDistance(SeriesPoint point, double distance) {
			point.SetValue(ExplodedDistanceProperty, distance);
		}
		public static void SetLabelPosition(SeriesLabel label, PieLabelPosition position) {
			label.SetValue(LabelPositionProperty, position);
		}
		[Obsolete(ObsoleteMessages.PercentOptionsProperty), EditorBrowsable(EditorBrowsableState.Never)]
		public static PercentOptions GetPercentOptions(PointOptions pointOptions) {
			return (PercentOptions)pointOptions.GetValue(PercentOptionsProperty);
		}
		[Obsolete(ObsoleteMessages.PercentOptionsProperty), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetPercentOptions(PointOptions pointOptions, PercentOptions value) {
			pointOptions.SetValue(PercentOptionsProperty, value);
		}
		static bool ExplodedDistanceValidation(object explodedDistance) {
			return (double)explodedDistance >= 0 && (double)explodedDistance <= 1;
		}
		static bool HoleRadiusPercentValidation(object holeRadiusPercent) {
			return (double)holeRadiusPercent >= 0 && (double)holeRadiusPercent <= 100;
		}
		static void OnExplodedDistancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PieSeries pieOrNestedDonut = ((SeriesPoint)d).Series as PieSeries;
			if (pieOrNestedDonut != null) {
				ChartElementHelper.Update(d, new PropertyUpdateInfo(pieOrNestedDonut, "ExplodedDistance"));
			}
		}
		static void OnHoleRadiusPercentPrpertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PieSeries pieOrNestedDonut = (PieSeries)d;
			ChartElementHelper.Update(d, new PropertyUpdateInfo(pieOrNestedDonut, "HoleRadius"), ChartElementChange.ClearDiagramCache);
		}
		protected override CompatibleViewType CompatibleViewType {
			get { return CompatibleViewType.SimpleView; }
		}
		protected override bool ShouldSortPoints { get { return false; } }
		protected override Type PointInterfaceType {
			get { return typeof(IValuePoint); }
		}
		protected override string DefaultLegendTextPattern { get { return "{" + PatternUtils.PercentValuePlaceholder + ":G2}"; } }
		protected override string DefaultLabelTextPattern { get { return "{" + PatternUtils.PercentValuePlaceholder + ":G2}"; } }
		internal bool LabelsResolveOverlapping {
			get {
				SeriesLabel label = ActualLabel;
				if (label == null || GetLabelPosition(label) == PieLabelPosition.Inside)
					return false;
				return label.ResolveOverlappingMode != ResolveOverlappingMode.None;
			}
		}
		protected internal override bool LabelsResolveOverlappingSupported {
			get { return true; }
		}
		protected internal override bool ActualColorEach {
			get { return true; }
		}
		protected internal override ToolTipPointDataToStringConverter ToolTipPointValuesConverter {
			get { return new ToolTipSimpleDiagramValueToStringConverter(this); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PieSeriesTitles"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public TitleCollection Titles {
			get { return (TitleCollection)GetValue(TitlesProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PieSeriesHoleRadiusPercent"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty]
		public double HoleRadiusPercent {
			get { return (double)GetValue(HoleRadiusPercentProperty); }
			set { SetValue(HoleRadiusPercentProperty, value); }
		}
		[Category(Categories.Behavior),
		XtraSerializableProperty]
		public int LabelsResolveOverlappingMinIndent {
			get { return (int)GetValue(LabelsResolveOverlappingMinIndentProperty); }
			set { SetValue(LabelsResolveOverlappingMinIndentProperty, value); }
		}
		public PieSeries() {
			this.SetValue(TitlesPropertyKey, ChartElementHelper.CreateInstance<TitleCollection>(this));
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			PieSeries pieSeries = series as PieSeries;
			if (pieSeries != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, pieSeries, LabelsResolveOverlappingMinIndentProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, pieSeries, HoleRadiusPercentProperty);
				if (Label != null && pieSeries.Label != null)
					PieSeries.SetLabelPosition(Label, PieSeries.GetLabelPosition(pieSeries.Label));
				if (pieSeries.Titles != null && pieSeries.Titles.Count > 0) {
					Titles.Clear();
					foreach (Title title in pieSeries.Titles) {
						Title newTitle = new Title();
						newTitle.Assign(title);
						Titles.Add(newTitle);
					}
				}
			}
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IValuePoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IValuePoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IValuePoint)point).Value);
		}
		protected override SeriesContainer CreateContainer() {
			return new SimpleSeriesContainer(this);
		}
		protected override PatternDataProvider GetDataProvider(PatternConstants patternConstant) {
			switch (patternConstant) {
				case PatternConstants.Argument:
				case PatternConstants.Value:
				case PatternConstants.PercentValue:
				case PatternConstants.PointHint:
					return new SimplePointPatternDataProvider(patternConstant);
				case PatternConstants.Series:
				case PatternConstants.SeriesGroup:
					return new SeriesPatternDataProvider(patternConstant);
			}
			return null;
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		internal double GetMaxExplodedDistance() {
			INestedDoughnutSeriesView nestDonutView = this as INestedDoughnutSeriesView;
			if (nestDonutView != null && !nestDonutView.IsOutside.Value)
				return 0;
			double distance = 0;
			foreach (SeriesPoint point in Points)
				distance = Math.Max(distance, (double)point.GetValue(ExplodedDistanceProperty));
			return distance;
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
