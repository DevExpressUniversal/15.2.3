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

using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using ChartsModel = DevExpress.Charts.Model;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region ReverseSeriesVisitor
	public class IsReverseSeriesVisitor : IChartViewVisitor {
		bool result;
		public bool Result { get { return result; } }
		void SetDefault() {
			result = false;
		}
		#region IChartViewVisitor Members
		public void Visit(Area3DChartView view) {
			result = view.Grouping == ChartGrouping.Standard;
		}
		public void Visit(AreaChartView view) {
			SetDefault();
		}
		public void Visit(Bar3DChartView view) {
			BarChartGrouping grouping = view.Grouping;
			result = (grouping == BarChartGrouping.Standard) || (grouping == BarChartGrouping.Clustered && view.BarDirection == BarChartDirection.Bar);
		}
		public void Visit(BarChartView view) {
			SetDefault();
		}
		public void Visit(BubbleChartView view) {
			SetDefault();
		}
		public void Visit(DoughnutChartView view) {
			SetDefault();
		}
		public void Visit(Line3DChartView view) {
			result = true;
		}
		public void Visit(LineChartView view) {
			SetDefault();
		}
		public void Visit(OfPieChartView view) {
			SetDefault();
		}
		public void Visit(Pie3DChartView view) {
			SetDefault();
		}
		public void Visit(PieChartView view) {
			SetDefault();
		}
		public void Visit(RadarChartView view) {
			SetDefault();
		}
		public void Visit(ScatterChartView view) {
			SetDefault();
		}
		public void Visit(StockChartView view) {
			SetDefault();
		}
		public void Visit(Surface3DChartView view) {
			SetDefault();
		}
		public void Visit(SurfaceChartView view) {
			SetDefault();
		}
		#endregion
	}
	#endregion
	#region ChartElement (enum)
	public enum ChartElement {
		Axis, 
		AxisTitle, 
		ChartArea, 
		ChartTitle, 
		DataLabels, 
		DataTable, 
		DownBars, 
		Floor,
		Legend, 
		MajorGridlines, 
		MinorGridlines, 
		OtherLines,
		PlotArea2D, 
		PlotArea3D, 
		TrendlineLabels, 
		UpBars, 
		Walls,  
		FillsForDataPoints2D,
		FillsForDataPoints3D,
		LinesForDataPoints,
		MarkersForDataPoints, 
	}
	#endregion
	#region ChartAppearanceColorInfo
	public struct ChartAppearanceColorInfo {
		#region Static Members 
		static SchemeColorValues[,] patternedSchemeColors = new SchemeColorValues[,] { 
			{ SchemeColorValues.Dark1, SchemeColorValues.Dark1, SchemeColorValues.Dark1, SchemeColorValues.Dark1, SchemeColorValues.Dark1, SchemeColorValues.Dark1 },
			{ SchemeColorValues.Accent1, SchemeColorValues.Accent2, SchemeColorValues.Accent3, SchemeColorValues.Accent4, SchemeColorValues.Accent5, SchemeColorValues.Accent6 }, 
			{ SchemeColorValues.Accent1, SchemeColorValues.Accent2, SchemeColorValues.Accent3, SchemeColorValues.Accent4, SchemeColorValues.Accent5, SchemeColorValues.Accent6 },
			{ SchemeColorValues.Dark1, SchemeColorValues.Dark1, SchemeColorValues.Dark1, SchemeColorValues.Dark1, SchemeColorValues.Dark1, SchemeColorValues.Dark1 } };
		static float[,] patternedTints = new float[,] { 
			{ 0.885f, 0.55f, 0.78f, 0.925f, 0.7f, 0.3f }, 
			{ 0, 0, 0, 0, 0, 0 }, 
			{-0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f}, 
			{ 0.05f, 0.55f, 0.78f, 0.15f, 0.7f, 0.3f }
		};
		static void CheckPattern(int pattern) {
			ValueChecker.CheckValue(pattern, minPattern, maxPattern, "pattern");
		}
		static ChartAppearanceColorInfo Create() {
			ChartAppearanceColorInfo result = new ChartAppearanceColorInfo();
			result.hasColor = true;
			return result;
		}
		public static ChartAppearanceColorInfo Create(SchemeColorValues schemeColor) {
			ChartAppearanceColorInfo result = Create();
			result.schemeColor = schemeColor;
			return result;
		}
		public static ChartAppearanceColorInfo Create(SchemeColorValues schemeColor, float tint) {
			ChartAppearanceColorInfo result = Create(schemeColor);
			result.tint = tint;
			return result;
		}
		public static ChartAppearanceColorInfo CreateFade(SchemeColorValues schemeColor) {
			ChartAppearanceColorInfo result = Create(schemeColor);
			result.isFade = true;
			return result;
		}
		public static ChartAppearanceColorInfo Create(int pattern) {
			CheckPattern(pattern);
			ChartAppearanceColorInfo result = Create();
			result.pattern = pattern;
			return result;
		}
		#endregion
		#region Fields
		const int patternedSeriesCount = 6;
		const int minPattern = 1;
		const int maxPattern = 4;
		SchemeColorValues schemeColor;
		bool hasColor;
		float tint;
		int pattern;
		bool isFade;
		#endregion
		#region Properties
		public SchemeColorValues SchemeColor { get { return schemeColor; } }
		public bool HasColor { get { return hasColor; } }
		public float Tint { get { return tint; } }
		public int Pattern { get { return pattern; } }
		public bool IsFade { get { return isFade; } }
		public bool HasPattern { get { return pattern >= minPattern; } }
		#endregion
		public Color ToRgb(ThemeDrawingColorCollection colors) {
			if (!hasColor)
				return DXColor.Empty;
			Color color = colors.GetColor(schemeColor);
			return ApplyTint(color, Tint);
		}
		public Color GetSeriesColor(int seriesIndex, int highestSeriesIndex, ThemeDrawingColorCollection themeColors) {
			if (!IsFade && !HasPattern)
				return ToRgb(themeColors);
			CheckIndexes(seriesIndex, highestSeriesIndex);
			Color baseSeriesColor = GetBaseSeriesColor(seriesIndex, themeColors);
			float tint = GetSeriesTint(seriesIndex, highestSeriesIndex);
			return ApplyTint(baseSeriesColor, tint);
		}
		public Color GetBaseSeriesColor(int seriesIndex, ThemeDrawingColorCollection themeColors) {
			ChartAppearanceColorInfo colorBaseInfo = Create(GetBaseSchemeColor(seriesIndex), GetBaseTint(seriesIndex));
			return colorBaseInfo.ToRgb(themeColors);
		}
		public float GetSeriesTint(int seriesIndex, int highestSeriesIndex) {
			return IsFade ? GetFadeTint(seriesIndex, highestSeriesIndex) : GetTintForPattern(seriesIndex, highestSeriesIndex);
		}
		#region Internal
		Color ApplyTint(Color color, float tint) {
			return tint == 0 ? color : GetTintColorTransform(tint).ApplyTransform(color);
		}
		ColorTransformBase GetTintColorTransform(float tint) {
			int percentage = (int)(tint * DrawingValueConstants.ThousandthOfPercentage);
			if (percentage > 0)
				return new TintColorTransform(percentage);
			return new ShadeColorTransform(-percentage);
		}
		SchemeColorValues GetBaseSchemeColor(int seriesIndex) {
			return IsFade ? schemeColor : patternedSchemeColors[pattern - 1, seriesIndex % patternedSeriesCount];
		}
		float GetBaseTint(int seriesIndex) {
			return IsFade ? 0 : GetBasePatternedTint(seriesIndex % patternedSeriesCount);
		}
		float GetBasePatternedTint(int seriesIndex) {
			return patternedTints[pattern - 1, seriesIndex];
		}
		void CheckIndexes(int seriesIndex, int highestSeriesIndex) {
			Guard.ArgumentNonNegative(seriesIndex, "seriesIndex");
			Guard.ArgumentNonNegative(highestSeriesIndex, "highestSeriesIndex");
			if (seriesIndex > highestSeriesIndex)
				Exceptions.ThrowInternalException();
		}
		float GetFadeTint(int seriesIndex, int highestSeriesIndex) {
			return PercentToTint(GetFadePersent(seriesIndex, highestSeriesIndex));
		}
		float PercentToTint(float percent) {
			return percent == 0 ? 0 : percent > 0 ? 1 - percent : -1 - percent;
		}
		float GetFadePersent(int zeroBasedIndex, int maxZeroBasedIndex) {
			zeroBasedIndex++;
			maxZeroBasedIndex++;
			return -0.7f + 1.4f * ((float)zeroBasedIndex / ((float)maxZeroBasedIndex + 1));
		}
		float GetTintForPattern(int seriesIndex, int highestSeriesIndex) {
			if (highestSeriesIndex < patternedSeriesCount)
				return 0;
			int maxIndex = highestSeriesIndex / patternedSeriesCount;
			if (highestSeriesIndex % patternedSeriesCount == 5)
				maxIndex += 1;
			return GetFadeTint(seriesIndex / patternedSeriesCount, maxIndex);
		}
		#endregion
	}
	#endregion
	#region ChartAppearanceElementBuilderFactory
	public static class ChartAppearanceElementBuilderFactory {
		static Dictionary<ChartElement, ChartAppearanceElementBuilder> buildersInstanceTable = GetBuildersInstanceTable();
		static Dictionary<ChartElement, ChartAppearanceElementBuilder> GetBuildersInstanceTable() {
			Dictionary<ChartElement, ChartAppearanceElementBuilder> result = new Dictionary<ChartElement, ChartAppearanceElementBuilder>();
			result.Add(ChartElement.Axis, new ChartAppearanceAxisBuilder());
			result.Add(ChartElement.AxisTitle, new ChartAppearanceElementBuilder()); 
			result.Add(ChartElement.ChartArea, new ChartAppearanceChartAreaBuilder());
			result.Add(ChartElement.ChartTitle, new ChartAppearanceElementBuilder()); 
			result.Add(ChartElement.DataLabels, new ChartAppearanceElementBuilder()); 
			result.Add(ChartElement.DataTable, new ChartAppearanceDataTableBuilder());
			result.Add(ChartElement.DownBars, new ChartAppearanceDownBarsBuilder());
			result.Add(ChartElement.Floor, new ChartAppearanceFloorBuilder());
			result.Add(ChartElement.Legend, new ChartAppearanceElementBuilder()); 
			result.Add(ChartElement.MajorGridlines, new ChartAppearanceMajorGridlinesBuilder());  
			result.Add(ChartElement.MinorGridlines, new ChartAppearanceMinorGridlinesBuilder());
			result.Add(ChartElement.OtherLines, new ChartAppearanceOtherLinesBuilder());
			result.Add(ChartElement.PlotArea2D, new ChartAppearancePlotArea2DBuilder());
			result.Add(ChartElement.PlotArea3D, new ChartAppearanceElementBuilder()); 
			result.Add(ChartElement.TrendlineLabels, new ChartAppearanceElementBuilder()); 
			result.Add(ChartElement.UpBars, new ChartAppearanceUpBarsBuilder());
			result.Add(ChartElement.Walls, new ChartAppearanceWallsBuilder());
			result.Add(ChartElement.FillsForDataPoints2D, new ChartAppearanceFillsForDataPoints2DBuilder());
			result.Add(ChartElement.FillsForDataPoints3D, new ChartAppearanceFillsForDataPoints3DBuilder());
			result.Add(ChartElement.LinesForDataPoints, new ChartAppearanceLinesForDataPointsBuilder());
			result.Add(ChartElement.MarkersForDataPoints, new ChartAppearanceMarkersForDataPointsBuilder());
			return result;
		}
		internal static ChartAppearanceElementBuilder GetInstanceBuilder(ChartElement element, int style) {
			ChartAppearanceElementBuilder result = buildersInstanceTable[element];
			result.Style = style;
			return result;
		}
		internal static ChartAppearanceElementBuilder GetBuilder(ChartElement element, int style) {
			ChartAppearanceElementBuilder result = GetInstanceBuilder(element, style);
			result.CalculateAllProperties();
			return result;
		}
	}
	#endregion
	#region ChartAppearanceElementBuilder
	public class ChartAppearanceElementBuilder {
		#region Fields
		StyleMatrixElementType themedLine;
		StyleMatrixElementType themedFill;
		StyleMatrixElementType themedEffect;
		ChartAppearanceColorInfo lineColor;
		ChartAppearanceColorInfo fillColor;
		ChartAppearanceColorInfo effectColor;
		int style;
		#endregion
		#region Properties
		protected internal StyleMatrixElementType ThemedLine { get { return themedLine; } protected set { themedLine = value; } }
		protected internal StyleMatrixElementType ThemedFill { get { return themedFill; } }
		protected internal StyleMatrixElementType ThemedEffect { get { return themedEffect; }  }
		protected internal ChartAppearanceColorInfo LineColor { get { return lineColor; } protected set { lineColor = value; } }
		protected internal ChartAppearanceColorInfo FillColor { get { return fillColor; } }
		protected internal ChartAppearanceColorInfo EffectColor { get { return effectColor; } }
		protected internal int Style { get { return style; } set { style = value; } }
		#endregion
		protected internal void CalculateAllProperties() {
			CalculateLineProperties();
			themedFill = GetThemedFill();
			fillColor = GetFillColorInfo();
			themedEffect = GetThemedEffect();
			effectColor = GetEffectColorInfo();
		}
		protected virtual void CalculateLineProperties() {
			themedLine = GetThemedLine();
			lineColor = GetLineColorInfo();
		}
		protected internal virtual StyleMatrixElementType GetThemedLine() {
			return StyleMatrixElementType.None;
		}
		protected internal virtual ChartAppearanceColorInfo GetLineColorInfo() {
			return new ChartAppearanceColorInfo();
		}
		protected internal virtual StyleMatrixElementType GetThemedFill() {
			return StyleMatrixElementType.None;
		}
		protected internal virtual ChartAppearanceColorInfo GetFillColorInfo() {
			return new ChartAppearanceColorInfo();
		}
		protected internal virtual StyleMatrixElementType GetThemedEffect() {
			return StyleMatrixElementType.None;
		}
		protected internal virtual ChartAppearanceColorInfo GetEffectColorInfo() {
			return new ChartAppearanceColorInfo();
		}
	}
	#endregion
	#region ChartAppearanceAxisBuilder
	public class ChartAppearanceAxisBuilder : ChartAppearanceElementBuilder {
		protected internal override StyleMatrixElementType GetThemedLine() {
			return StyleMatrixElementType.Subtle;
		}
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			return ChartAppearanceColorInfo.Create(GetSchemeColorValues());
		}
		SchemeColorValues GetSchemeColorValues() {
			if (Style > 40)
				return SchemeColorValues.Light1; 
			return Style > 32 ? SchemeColorValues.Dark1 : SchemeColorValues.Text1;
		}
	}
	#endregion
	#region ChartAppearanceDataTableBuilder
	public class ChartAppearanceDataTableBuilder : ChartAppearanceElementBuilder {
		#region Static Members
		internal static ChartAppearanceColorInfo CreateLineColorInfo(int style) {
			if (style < 33)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Text1, 0.75f);
			if (style < 35)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, 0.75f);
			if (style < 41)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, 0.75f);
			return ChartAppearanceColorInfo.Create(SchemeColorValues.Light1);
		}
		#endregion 
		protected internal override StyleMatrixElementType GetThemedLine() {
			return StyleMatrixElementType.Subtle;
		}
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			return CreateLineColorInfo(Style);
		}
	}
	#endregion
	#region ChartAppearanceChartAreaBuilder
	public class ChartAppearanceChartAreaBuilder : ChartAppearanceElementBuilder {
		#region Static Members
		internal static StyleMatrixElementType CreateThemedLine(int style) {
			return style < 41 ? StyleMatrixElementType.Subtle : StyleMatrixElementType.None;
		}
		#endregion 
		protected internal override StyleMatrixElementType GetThemedLine() {
			return CreateThemedLine(Style);
		}
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			return ChartAppearanceDataTableBuilder.CreateLineColorInfo(Style);
		}
		protected internal override StyleMatrixElementType GetThemedFill() {
			return StyleMatrixElementType.Subtle;
		}
		protected internal override ChartAppearanceColorInfo GetFillColorInfo() {
			return ChartAppearanceColorInfo.Create(GetFillSchemeColorValues());
		}
		SchemeColorValues GetFillSchemeColorValues() {
			if (Style < 33)
				return SchemeColorValues.Background1;
			if (Style > 40)
				return SchemeColorValues.Dark1;
			return SchemeColorValues.Light1;
		}
	}
	#endregion
	#region ChartAppearanceDownBarsBuilder
	public class ChartAppearanceDownBarsBuilder : ChartAppearanceElementBuilder {
		#region Static Members
		static Dictionary<int, ChartAppearanceColorInfo> shadeColorInfoTable = GetShadeColorInfoTable();
		static Dictionary<int, ChartAppearanceColorInfo> GetShadeColorInfoTable() {
			Dictionary<int, ChartAppearanceColorInfo> result = new Dictionary<int, ChartAppearanceColorInfo>();
			result.Add(1, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent1, -0.25f));
			result.Add(2, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent2, -0.25f));
			result.Add(3, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent3, -0.25f));
			result.Add(4, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent4, -0.25f));
			result.Add(5, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent5, -0.25f));
			result.Add(6, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent6, -0.25f));
			return result;
		}
		#endregion
		protected internal override StyleMatrixElementType GetThemedLine() {
			return Style < 17 || (Style > 32 && Style < 41) ? StyleMatrixElementType.Subtle : StyleMatrixElementType.None;
		}
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			if (Style < 17)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Text1);
			if (Style < 35 && Style > 32)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1);
			if (Style > 34 && Style < 41)
				return shadeColorInfoTable[Style - 34];
			return new ChartAppearanceColorInfo();
		}
		protected internal override StyleMatrixElementType GetThemedFill() {
			return Style < 17 || (Style > 32 && Style < 41) ? StyleMatrixElementType.Subtle : StyleMatrixElementType.Intense;
		}
		protected internal override ChartAppearanceColorInfo GetFillColorInfo() {
			if (Style % 8 == 1)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, 0.85f);
			if (Style == 42)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1);
			if (Style % 8 == 2)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, 0.95f);
			return shadeColorInfoTable[(Style - 2) % 8];
		}
		protected internal override StyleMatrixElementType GetThemedEffect() {
			if (Style > 8 && Style < 17)
				return StyleMatrixElementType.Subtle;
			if (Style > 16 && Style < 25)
				return StyleMatrixElementType.Moderate;
			if ((Style > 24 && Style < 33) || Style > 40)
				return StyleMatrixElementType.Intense;
			return StyleMatrixElementType.None;
		}
		protected internal override ChartAppearanceColorInfo GetEffectColorInfo() {
			return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1);
		}
	}
	#endregion
	#region ChartAppearanceUpBarsBuilder
	public class ChartAppearanceUpBarsBuilder : ChartAppearanceDownBarsBuilder {
		#region Static Members
		static Dictionary<int, ChartAppearanceColorInfo> tintColorInfoTable = GetTintColorInfoTable();
		static Dictionary<int, ChartAppearanceColorInfo> GetTintColorInfoTable() {
			Dictionary<int, ChartAppearanceColorInfo> result = new Dictionary<int, ChartAppearanceColorInfo>();
			result.Add(1, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent1, 0.25f));
			result.Add(2, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent2, 0.25f));
			result.Add(3, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent3, 0.25f));
			result.Add(4, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent4, 0.25f));
			result.Add(5, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent5, 0.25f));
			result.Add(6, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent6, 0.25f));
			return result;
		}
		#endregion
		protected internal override ChartAppearanceColorInfo GetFillColorInfo() {
			if (Style < 33) { 
				if (Style % 8 == 1)
					return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, 0.25f);
				if (Style % 8 == 2)
					return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, 0.05f);
				return tintColorInfoTable[(Style - 2) % 8];
			}
			if (Style < 41 || Style == 42)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Light1);
			if (Style == 41)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, 0.25f);
			return tintColorInfoTable[Style - 42];
		}
	}
	#endregion
	#region ChartAppearanceWallsBuilder
	public class ChartAppearanceWallsBuilder : ChartAppearanceElementBuilder {
		#region Static Members
		static Dictionary<int, ChartAppearanceColorInfo> fillColorInfoTable = GetFillColorInfoTable();
		static Dictionary<int, ChartAppearanceColorInfo> GetFillColorInfoTable() {
			Dictionary<int, ChartAppearanceColorInfo> result = new Dictionary<int, ChartAppearanceColorInfo>();
			result.Add(1, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent1, 0.2f));
			result.Add(2, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent2, 0.2f));
			result.Add(3, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent3, 0.2f));
			result.Add(4, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent4, 0.2f));
			result.Add(5, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent5, 0.2f));
			result.Add(6, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent6, 0.2f));
			return result;
		}
		#endregion
		protected internal override StyleMatrixElementType GetThemedFill() {
			return Style > 32 ? StyleMatrixElementType.Subtle : StyleMatrixElementType.None;
		}
		protected internal override ChartAppearanceColorInfo GetFillColorInfo() {
			if (Style < 33)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Background1);
			if (Style < 35)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, 0.2f);
			if (Style > 34 && Style < 41)
				return fillColorInfoTable[Style - 34];
			return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, 0.95f);
		}
	}
	#endregion
	#region ChartAppearanceFloorBuilder
	public class ChartAppearanceFloorBuilder : ChartAppearanceWallsBuilder {
		protected internal override StyleMatrixElementType GetThemedLine() {
			return ChartAppearanceChartAreaBuilder.CreateThemedLine(Style);
		}
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			return ChartAppearanceDataTableBuilder.CreateLineColorInfo(Style);
		}
	}
	#endregion
	#region ChartAppearanceMajorGridlinesBuilder
	public class ChartAppearanceMajorGridlinesBuilder : ChartAppearanceElementBuilder {
		protected internal override StyleMatrixElementType GetThemedLine() {
			return StyleMatrixElementType.Subtle;
		}
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			return ChartAppearanceColorInfo.Create(Style > 32 ? SchemeColorValues.Dark1 : SchemeColorValues.Text1, 0.75f);
		}
	}
	#endregion
	#region ChartAppearanceMinorGridlinesBuilder
	public class ChartAppearanceMinorGridlinesBuilder : ChartAppearanceAxisBuilder {
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			return ChartAppearanceColorInfo.Create(SchemeColorValues.Text1, Style < 41 ? 0.5f : 0.9f);
		}
	}
	#endregion
	#region ChartAppearanceOtherLinesBuilder
	public class ChartAppearanceOtherLinesBuilder : ChartAppearanceAxisBuilder {
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			if (Style > 34 && Style < 41)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, -0.25f);
			return ChartAppearanceColorInfo.Create(GetLineSchemeColor());
		}
		SchemeColorValues GetLineSchemeColor() {
			if (Style < 33)
				return SchemeColorValues.Text1;
			if (Style < 35)
				return SchemeColorValues.Dark1;
			return SchemeColorValues.Light1;
		}
	}
	#endregion
	#region ChartAppearancePlotArea2DBuilder
	public class ChartAppearancePlotArea2DBuilder : ChartAppearanceWallsBuilder {
		protected internal override StyleMatrixElementType GetThemedFill() {
			return StyleMatrixElementType.Subtle;
		}
	}
	#endregion
	#region ChartAppearanceLinesForDataPointsBuilder
	public class ChartAppearanceLinesForDataPointsBuilder : ChartAppearanceElementBuilder {
		#region Static Members
		static Dictionary<int, ChartAppearanceColorInfo> fadeColorInfoTable = GetFadeColorInfoTable();
		static Dictionary<int, ChartAppearanceColorInfo> GetFadeColorInfoTable() {
			Dictionary<int, ChartAppearanceColorInfo> result = new Dictionary<int, ChartAppearanceColorInfo>();
			result.Add(1, ChartAppearanceColorInfo.CreateFade(SchemeColorValues.Accent1));
			result.Add(2, ChartAppearanceColorInfo.CreateFade(SchemeColorValues.Accent2));
			result.Add(3, ChartAppearanceColorInfo.CreateFade(SchemeColorValues.Accent3));
			result.Add(4, ChartAppearanceColorInfo.CreateFade(SchemeColorValues.Accent4));
			result.Add(5, ChartAppearanceColorInfo.CreateFade(SchemeColorValues.Accent5));
			result.Add(6, ChartAppearanceColorInfo.CreateFade(SchemeColorValues.Accent6));
			return result;
		}
		internal static ChartAppearanceColorInfo GetColorInfo(int style) {
			if (style % 8 == 1)
				return ChartAppearanceColorInfo.Create(style == 41 ? 4 : 1);
			if (style % 8 == 2)
				return ChartAppearanceColorInfo.Create(2);
			return fadeColorInfoTable[(style - 2) % 8];
		}
		#endregion
		int widthLine;
		protected internal int WidthLine { get { return widthLine; } }
		protected override void CalculateLineProperties() {
			base.CalculateLineProperties();
			widthLine = GetWidthLine();
		}
		protected internal int GetWidthLine() {
			if (Style < 9)
				return 3;
			if (Style > 24 && Style < 33)
				return 7;
			return 5;
		}
		protected internal override StyleMatrixElementType GetThemedLine() {
			return StyleMatrixElementType.Subtle;
		}
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			return GetColorInfo(Style);
		}
	}
	#endregion
	#region ChartAppearanceMarkersForDataPointsBuilder
	public class ChartAppearanceMarkersForDataPointsBuilder : ChartAppearanceElementBuilder {
		#region Static Members
		internal static StyleMatrixElementType CreateThemedEffect(int style) {
			if (style > 8 && style < 17)
				return StyleMatrixElementType.Subtle;
			if (style > 16 && style < 25)
				return StyleMatrixElementType.Moderate;
			if ((style > 24 && style < 33) || style > 40)
				return StyleMatrixElementType.Intense;
			return StyleMatrixElementType.None;
		}
		#endregion
		protected internal override StyleMatrixElementType GetThemedLine() {
			return StyleMatrixElementType.Subtle;
		}
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			return ChartAppearanceLinesForDataPointsBuilder.GetColorInfo(Style);
		}
		protected internal override StyleMatrixElementType GetThemedFill() {
			return Style < 17 || (Style > 32 && Style < 41) ? StyleMatrixElementType.Subtle : StyleMatrixElementType.Intense;
		}
		protected internal override ChartAppearanceColorInfo GetFillColorInfo() {
			return ChartAppearanceLinesForDataPointsBuilder.GetColorInfo(Style);
		}
		protected internal override StyleMatrixElementType GetThemedEffect() {
			return CreateThemedEffect(Style);
		}
		protected internal override ChartAppearanceColorInfo GetEffectColorInfo() {
			return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1);
		}
	}
	#endregion
	#region ChartAppearanceFillsForDataPoints2DBuilder
	public class ChartAppearanceFillsForDataPoints2DBuilder : ChartAppearanceMarkersForDataPointsBuilder {
		#region Static Members
		static Dictionary<int, ChartAppearanceColorInfo> shadeColorInfoTable = GetShadeColorInfoTable();
		static Dictionary<int, ChartAppearanceColorInfo> GetShadeColorInfoTable() {
			Dictionary<int, ChartAppearanceColorInfo> result = new Dictionary<int, ChartAppearanceColorInfo>();
			result.Add(1, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent1, -0.5f));
			result.Add(2, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent2, -0.5f));
			result.Add(3, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent3, -0.5f));
			result.Add(4, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent4, -0.5f));
			result.Add(5, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent5, -0.5f));
			result.Add(6, ChartAppearanceColorInfo.Create(SchemeColorValues.Accent6, -0.5f));
			return result;
		}
		#endregion
		protected internal override StyleMatrixElementType GetThemedLine() {
			return (Style > 8 && Style < 17) || (Style > 32 && Style < 41) ? StyleMatrixElementType.Subtle : StyleMatrixElementType.None;
		}
		protected internal override ChartAppearanceColorInfo GetLineColorInfo() {
			if (Style > 8 && Style < 17)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Light1);
			if (Style == 33)
				return ChartAppearanceColorInfo.Create(SchemeColorValues.Dark1, -0.5f);
			if (Style == 34)
				return ChartAppearanceColorInfo.Create(3);
			if (Style > 34 && Style < 41)
				return shadeColorInfoTable[Style - 34];
			return new ChartAppearanceColorInfo();
		}
	}
	#endregion
	#region ChartAppearanceFillsForDataPoints3DBuilder
	public class ChartAppearanceFillsForDataPoints3DBuilder : ChartAppearanceFillsForDataPoints2DBuilder {
		protected internal override StyleMatrixElementType GetThemedFill() {
			return Style == 18 || Style == 26 || Style == 42 ? StyleMatrixElementType.Intense : StyleMatrixElementType.Subtle;
		}
	}
	#endregion
	#region IChartAppearance
	public interface IChartAppearance {
		DrawingFillType GetFillType(ChartElement element, int index, int maxIndex);
		Color GetFillForegroundColor(ChartElement element, int index, int maxIndex);
		Color GetFillBackgroundColor(ChartElement element, int index, int maxIndex);
		DrawingFillType GetOutlineFillType(ChartElement element, int index, int maxIndex);
		Color GetOutlineColor(ChartElement element, int index, int maxIndex);
		int GetOutlineWidth(ChartElement element, int index, int maxIndex);
		OutlineDashing GetOutlineDashing(ChartElement element, int index, int maxIndex);
		DrawingPatternType GetOutlineFillPatternType(ChartElement element, int index, int maxIndex);
	}
	#endregion
	#region Partial Chart (ChartAppearanceCalculator)
	public partial class Chart : IChartAppearance {
		#region Static Members
		static FillStyleFactory fillStyleFactory = new FillStyleFactory();
		static FillPaletteEntryFactory paletteEntryFactory = new FillPaletteEntryFactory();
		static FillColorsFactory fillColorsFactory = FillPaletteEntryFactory.FillColorsFactory;
		static ShadowFactory shadowFactory = new ShadowFactory();
		static ChartsModel.ColorARGB transparentColor = new ChartsModel.ColorARGB(0x00, 0xff, 0xff, 0xff);
		static ChartsModel.ColorARGB ConvertColor(Color color) {
			return new ChartsModel.ColorARGB(color.A, color.R, color.G, color.B);
		}
		#endregion
		protected void SetupChartStyle() {
			if (this.modelChart == null)
				return;
			SetupChartStyleCore();
			ResetCachedImage();
		}
		protected internal void MakeAxisTransparent(ChartsModel.AxisBase modelAxis) {
			if (modelAxis.Appearance == null)
				modelAxis.Appearance = new ChartsModel.AxisAppearance();
			ChartsModel.AxisAppearance appearance = modelAxis.Appearance;
			appearance.Color = transparentColor; 
			SetupAxesAppearanceFromStyleCore(appearance);
			SetupAxisTitleAppearance(appearance, CalculateTextColor());
		}
		#region Internal
		void SetupChartStyleCore() {
			ChartsModel.Palette palette = new ChartsModel.Palette(this.modelChart);
			ChartsModel.ChartAppearanceOptions appearance = new ChartsModel.ChartAppearanceOptions();
			ChartsModel.ColorARGB textColor = CalculateTextColor();
			SetupChartAppearance(appearance, textColor);
			SetupDiagrammAppearance(appearance, textColor);
			SetupSeriesAppearance(appearance, palette, textColor);
			modelChart.Appearance = appearance;
			if (palette.Entries.Count > 0)
				this.modelChart.Palette = palette;
		}
		#region SetupChartAppearance
		void SetupChartAppearance(ChartsModel.ChartAppearanceOptions options, ChartsModel.ColorARGB textColor) {
			if (options.ChartAppearance == null)
				options.ChartAppearance = new ChartsModel.ChartAppearance();
			ChartsModel.ChartAppearance appearance = options.ChartAppearance;
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(ChartElement.ChartArea);
			SetupChartAppearanceBorder(appearance, builder);
			SetupColor(builder, ShapeProperties.Fill, SetChartAppearanceFillStyle, SetChartAppearanceBackColor, SetChartAppearanceColor2, appearance);
			SetupLegendAppearance(options, textColor);
			if (appearance.TitleAppearance == null)
				appearance.TitleAppearance = new ChartsModel.ChartTitleAppearance();
			appearance.TitleAppearance.TextColor = GetSolidTextColor(Title.TextProperties.Paragraphs, Title.Text,  textColor);
		}
		ChartsModel.ColorARGB GetSolidTextColor(TextProperties textProperties, ChartsModel.ColorARGB defaultColor) {
			if (textProperties.IsDefault)
				return defaultColor;
			return GetSolidTextColor(textProperties.Paragraphs, defaultColor);
		}
		ChartsModel.ColorARGB GetSolidTextColor(DrawingTextParagraphCollection paragraphs, IChartText text, ChartsModel.ColorARGB defaultColor) {
			if (text.TextType == ChartTextType.Auto || text.TextType == ChartTextType.Ref)
				return GetSolidTextColor(paragraphs, defaultColor);
			if (text.TextType == ChartTextType.Rich)
				return GetSolidTextColor(text as ChartRichText, defaultColor);
			return defaultColor;
		}
		ChartsModel.ColorARGB GetSolidTextColor(IDrawingFill fill, ChartsModel.ColorARGB defaultColor) {
			DrawingSolidFill solidFill = fill as DrawingSolidFill;
			return solidFill != null ? ConvertColor(solidFill.Color.FinalColor) : defaultColor;
		}
		ChartsModel.ColorARGB GetSolidTextColor(DrawingTextParagraphCollection paragraphs, ChartsModel.ColorARGB defaultColor) {
			if (paragraphs.Count > 0) {
				DrawingTextParagraph paragraph = paragraphs[0];
				DrawingTextParagraphProperties paragraphProperties = paragraph.ParagraphProperties;
				if (paragraph.ApplyParagraphProperties && paragraphProperties.ApplyDefaultCharacterProperties) 
					return GetSolidTextColor(paragraphProperties.DefaultCharacterProperties.Fill, defaultColor);
			}
			return defaultColor;
		}
		ChartsModel.ColorARGB GetSolidTextColor(ChartRichText richText, ChartsModel.ColorARGB defaultColor) {
			return richText == null ? defaultColor : GetRichTextColor(richText.Paragraphs, defaultColor);
		}
		ChartsModel.ColorARGB GetRichTextColor(DrawingTextParagraphCollection paragraphs, ChartsModel.ColorARGB defaultColor) {
			if (paragraphs.Count > 0) {
				DrawingTextParagraph paragraph = paragraphs[0];
				if (paragraph.Runs.Count > 0) {
					IDrawingTextRun run = paragraph.Runs[0];
					return GetSolidTextColor(run.RunProperties.Fill, defaultColor);
				}
				DrawingTextParagraphProperties paragraphProperties = paragraph.ParagraphProperties;
				if (paragraph.ApplyParagraphProperties && paragraphProperties.ApplyDefaultCharacterProperties)
					return GetSolidTextColor(paragraphProperties.DefaultCharacterProperties.Fill, defaultColor);
			}
			return defaultColor;
		}
		void SetupChartAppearanceBorder(ChartsModel.ChartAppearance appearance, ChartAppearanceElementBuilder builder) {
			Outline outline = ShapeProperties.Outline;
			appearance.Border = outline.IsDefault ? GetBorder(builder) : GetBorder(outline);
		}
		void SetupLegendAppearance(ChartsModel.ChartAppearanceOptions options, ChartsModel.ColorARGB textColor) {
			if (options.LegendAppearance == null)
				options.LegendAppearance = new ChartsModel.LegendAppearance();
			ChartsModel.LegendAppearance appearance = options.LegendAppearance;
			appearance.TextColor = GetSolidTextColor(Legend.TextProperties, textColor);
			ShapeProperties shapeProperties = Legend.ShapeProperties;
			IDrawingFill fill = shapeProperties.Fill;
			if (fill.FillType != DrawingFillType.Automatic && fill.FillType != DrawingFillType.Picture && fill.FillType != DrawingFillType.Pattern)
				SetColorFromShapeProperties(fill, SetLegendAppearanceFillStyle, SetLegendAppearanceBackColor, SetLegendAppearanceColor2, appearance);
			else {
				appearance.BackColor = transparentColor;
			}
			SetupLegendAppearanceBorder(appearance, shapeProperties.Outline);
			appearance.Shadow = shadowFactory.Create(shapeProperties.EffectStyle, DXColor.Empty);
		}
		void SetupLegendAppearanceBorder(ChartsModel.LegendAppearance appearance, Outline outline) {
			appearance.Border = outline.IsDefault ? GetTransparentBorder() : GetBorder(outline);
		}
		#endregion
		#region SetupDiagrammAppearance
		void SetupDiagrammAppearance(ChartsModel.ChartAppearanceOptions options, ChartsModel.ColorARGB textColor) {
			if (options.DiagrammAppearance == null)
				options.DiagrammAppearance = new ChartsModel.DiagrammAppearance();
			ChartsModel.DiagrammAppearance appearance = options.DiagrammAppearance;
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(ChartElement.PlotArea2D);
			ShapeProperties shapeProperties = plotArea.ShapeProperties;
			if(Is3DChart && shapeProperties.Fill.FillType == DrawingFillType.None && Views[0].ViewType != ChartViewType.Pie3D)
				SetupColor(builder, DrawingFill.Automatic, SetDiagrammAppearanceFillStyle, SetDiagrammAppearanceBackColor, SetDiagrammAppearanceColor2, appearance);
			else
				SetupColor(builder, shapeProperties.Fill, SetDiagrammAppearanceFillStyle, SetDiagrammAppearanceBackColor, SetDiagrammAppearanceColor2, appearance);
			SetupDiagrammAppearanceBorder(appearance, shapeProperties.Outline);
			appearance.Shadow = shadowFactory.Create(shapeProperties.EffectStyle, DXColor.Empty);
			SetupAxesAppearance(appearance, textColor);
		}
		void SetupDiagrammAppearanceBorder(ChartsModel.DiagrammAppearance appearance, Outline outline) {
			if (!outline.IsDefault) {
				appearance.BorderVisible = true;
				appearance.BorderColor = GetLineColor(outline, DXColor.Empty);
			}
		}
		void SetupAxesAppearance(ChartsModel.DiagrammAppearance appearance, ChartsModel.ColorARGB textColor) {
			if (appearance.AxesAppearance == null)
				appearance.AxesAppearance = new ChartsModel.AxisAppearance();
			SetupAxesAppearance(appearance.AxesAppearance, textColor);
		}
		void SetupAxesAppearance(ChartsModel.AxisAppearance appearance, ChartsModel.ColorARGB textColor) {
			SetupAxesAppearanceFromStyle(appearance);
			appearance.LabelTextColor = textColor;
			SetupAxisTitleAppearance(appearance, textColor);
		}
		void SetupAxesAppearanceFromStyleCore(ChartsModel.AxisAppearance appearance) {
			appearance.Thickness = 1;
			SetupLines(ChartElement.MajorGridlines, SetMajorGridLinesColor, SetMajorGridLinesLineStyle, appearance);
			SetupLines(ChartElement.MinorGridlines, SetMinorGridLinesColor, SetMinorGridLinesLineStyle, appearance);
		}
		void SetupAxesAppearanceFromStyle(ChartsModel.AxisAppearance appearance) {
			SetupAxesAppearanceFromStyleCore(appearance);
			appearance.Color = appearance.GridLinesColor;
		}
		void SetupAxisTitleAppearance(ChartsModel.AxisAppearance axesAppearance, ChartsModel.ColorARGB color) {
			if (axesAppearance.TitleAppearance == null)
				axesAppearance.TitleAppearance = new ChartsModel.AxisTitleAppearance();
			axesAppearance.TitleAppearance.TextColor = color; 
		}
		#endregion
		#region SetupSeriesAppearance
		void SetupSeriesAppearance(ChartsModel.ChartAppearanceOptions options, ChartsModel.Palette palette, ChartsModel.ColorARGB textColor) {
			if (options.SeriesAppearance == null)
				options.SeriesAppearance = new ChartsModel.SeriesAppearance();
			SetupSeriesAppearance(options.SeriesAppearance, palette, textColor);
		}
		void SetupSeriesAppearance(ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette, ChartsModel.ColorARGB textColor) {
			SetupLabelAppearance(appearance, textColor); 
			if (HasVaryColor())
				SetupSeriesAppearanceForVaryColor(appearance, palette);
			else
				SetupSeriesAppearanceCore(appearance, palette);
		}
		void SetupSeriesAppearanceForVaryColor(ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette) {
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(ChartElement.FillsForDataPoints2D);
			appearance.Border = GetSeriesBorder(builder);
			paletteEntryFactory.Prepare(views[0], palette);
			SetupSeriesFillForVaryColor(GetFill(builder), builder.GetFillColorInfo(), appearance, palette);
			SetupLineStyle(appearance);
			SetupSeriesShadowForVaryColor(appearance);
		}
		#region SetupSeriesShadowForVaryColor
		void SetupSeriesShadowForVaryColor(ChartsModel.SeriesAppearance appearance) {
			int viewCount = Views.Count;
			for (int i = 0; i < viewCount; i++) {
				SeriesCollection seriesCollection = Views[i].Series;
				int seriesCount = seriesCollection.Count;
				for (int j = 0; j < seriesCount; j++) {
					ISeries series = seriesCollection[j];
					ChartElement chartElement = GetChartElementForVaryColor(series);
					ChartAppearanceElementBuilder builder = GetInstanceBuilder(chartElement);
					ShapeProperties shapeProperties = GetSeriesShapePropertiesForVaryColor(series);
					SetupSeriesShadow(shapeProperties.EffectStyle, appearance, builder);
				}
			}
		}
		ChartElement GetChartElementForVaryColor(ISeries series) {
			if (IsMarkerOnlySeries(series))
				return ChartElement.MarkersForDataPoints;
			if (HasLineSeries(series))
				return ChartElement.LinesForDataPoints;
			return ChartElement.FillsForDataPoints2D;
		}
		ShapeProperties GetSeriesShapePropertiesForVaryColor(ISeries series) {
			return IsMarkerOnlySeries(series) ? ((ISeriesWithMarker)series).Marker.ShapeProperties : series.ShapeProperties;
		}
		#endregion
		void SetupSeriesAppearanceCore(ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette) {
			int viewCount = Views.Count;
			for (int i = 0; i < viewCount; i++) {
				IChartView view = Views[i];
				List<ISeries> seriesCollection = GetOrderedSeries(view);
				int highestSeriesIndex = GetHighestSeriesIndex(seriesCollection);
				paletteEntryFactory.Prepare(view, palette);
				int seriesCount = seriesCollection.Count;
				for (int j = 0; j < seriesCount; j++) {
					ISeries series = seriesCollection[j];
					if (IsMarkerOnlySeries(series))
						SetupSeriesForMarker(series, appearance, palette, highestSeriesIndex);
					else if (HasLineSeries(series)) 
						SetupSeriesAppearanceCoreForLineSeries(series, appearance, palette, highestSeriesIndex, seriesCount);
					else
						SetupSeriesForFill(series, appearance, palette, highestSeriesIndex);
				}
			}
		}
		void SetupSeriesAppearanceCoreForLineSeries(ISeries series, ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette, int highestSeriesIndex, int seriesCount) {
			SetupSeriesForLine(series, appearance, palette, highestSeriesIndex);
			if (seriesCount > 1) {
				ChartAppearanceLinesForDataPointsBuilder builder = GetInstanceBuilder(ChartElement.LinesForDataPoints) as ChartAppearanceLinesForDataPointsBuilder;
				SetupLineStyleForLines(builder, appearance);
			}
		}
		void SetupSeriesForMarker(ISeries series, ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette, int highestSeriesIndex) {
			ChartAppearanceMarkersForDataPointsBuilder builder = GetInstanceBuilder(ChartElement.MarkersForDataPoints) as ChartAppearanceMarkersForDataPointsBuilder;
			ShapeProperties shapeProperties = ((ISeriesWithMarker)series).Marker.ShapeProperties;
			SetupSeriesFill(shapeProperties.Fill, appearance, palette, builder, series.Index, highestSeriesIndex);
			SetupSeriesShadow(shapeProperties.EffectStyle, appearance, builder);
		}
		void SetupSeriesForLine(ISeries series, ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette, int highestSeriesIndex) {
			ChartAppearanceLinesForDataPointsBuilder builder = GetInstanceBuilder(ChartElement.LinesForDataPoints) as ChartAppearanceLinesForDataPointsBuilder;
			ShapeProperties shapeProperties = series.ShapeProperties;
			SetupSeriesOutline(shapeProperties.Outline, appearance, palette, builder, series.Index, highestSeriesIndex);
			SetupSeriesShadow(shapeProperties.EffectStyle, appearance, builder);
		}
		void SetupSeriesForFill(ISeries series, ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette, int highestSeriesIndex) {
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(ChartElement.FillsForDataPoints2D);
			ShapeProperties shapeProperties = series.ShapeProperties;
			SetupSeriesFill(shapeProperties.Fill, appearance, palette, builder, series.Index, highestSeriesIndex);
			SetupSeriesBorder(shapeProperties.Outline, appearance, builder);
			SetupSeriesShadow(shapeProperties.EffectStyle, appearance, builder);
		}
		#region SetupSeriesFillForVaryColor
		void SetupSeriesFillForVaryColor(IDrawingFill fill, ChartAppearanceColorInfo fillColorInfo, ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette) {
			SetupSeriesFillStyle(appearance, fill);
			SetupSeriesColorsForVaryColor(fill, palette, fillColorInfo);
		}
		void SetupSeriesColorsForVaryColor(IDrawingFill fill, ChartsModel.Palette palette, ChartAppearanceColorInfo fillColorInfo) {
			if (fillColorInfo.IsFade)
				SetupSeriesColorForVaryColor(palette, 0, fill, fillColorInfo);
			if (fillColorInfo.HasPattern)
				for (int i = 0; i < 6; i++)
					SetupSeriesColorForVaryColor(palette, i, fill, fillColorInfo);
		}
		void SetupSeriesColorForVaryColor(ChartsModel.Palette palette, int baseIndex, IDrawingFill fill, ChartAppearanceColorInfo fillColorInfo) {
			Color color = fillColorInfo.GetBaseSeriesColor(baseIndex, Theme.Colors);
			AddColor(palette, fill, color);
		}
		#endregion 
		#region SetupSeriesOutline
		void SetupSeriesOutline(Outline currentSeriesOutline, ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette, ChartAppearanceLinesForDataPointsBuilder builder, int seriesIndex, int highestSeriesIndex) {
			if (currentSeriesOutline.Fill.FillType != DrawingFillType.Automatic && currentSeriesOutline.Fill.FillType != DrawingFillType.Pattern)
				SetupSeriesOutline(builder, currentSeriesOutline, appearance, palette);
			else
				SetupSeriesOutline(builder, currentSeriesOutline, appearance, palette, seriesIndex, highestSeriesIndex);
		}
		void SetupSeriesOutline(ChartAppearanceLinesForDataPointsBuilder builder, Outline currentSeriesOutline, ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette) {
			SetupLineStyleForLines(builder, appearance, currentSeriesOutline);  
			AddColor(palette, currentSeriesOutline.Fill, DXColor.Empty);
		}
		void SetupSeriesOutline(ChartAppearanceLinesForDataPointsBuilder builder, Outline currentSeriesOutline, ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette, int seriesIndex, int highestSeriesIndex) {
			SetupLineStyleForLines(builder, appearance, currentSeriesOutline);  
			ChartAppearanceColorInfo colorInfo = builder.GetLineColorInfo();
			Color color = colorInfo.GetSeriesColor(seriesIndex, highestSeriesIndex, Theme.Colors);
			Outline outline = GetOutline(builder);
			AddColor(palette, outline.Fill, color);
		}
		#endregion
		#region SetupSeriesFill
		void SetupSeriesFill(IDrawingFill fill, ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette, ChartAppearanceElementBuilder builder, int seriesIndex, int highestSeriesIndex) {
			if (fill.FillType != DrawingFillType.Automatic && fill.FillType != DrawingFillType.Picture && fill.FillType != DrawingFillType.Pattern)
				SetupSeriesFill(appearance, palette, fill);
			else
				SetupSeriesFill(GetFill(builder), builder.GetFillColorInfo(), appearance, palette, seriesIndex, highestSeriesIndex);
		}
		void SetupSeriesFill(ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette, IDrawingFill fill) {
			SetupSeriesFillStyle(appearance, fill);
			AddColor(palette, fill, DXColor.Empty);
		}
		void SetupSeriesFill(IDrawingFill fill, ChartAppearanceColorInfo colorInfo, ChartsModel.SeriesAppearance appearance, ChartsModel.Palette palette, int seriesIndex, int highestSeriesIndex) {
			SetupSeriesFillStyle(appearance, fill);
			Color color = colorInfo.GetSeriesColor(seriesIndex, highestSeriesIndex, Theme.Colors);
			AddColor(palette, fill, color);
		}
		void SetupSeriesFillStyle(ChartsModel.SeriesAppearance appearance, IDrawingFill fill) {
			appearance.FillStyle = fillStyleFactory.Create(fill);
		}
		void AddColor(ChartsModel.Palette palette, IDrawingFill fill, Color styleColor) {
			paletteEntryFactory.AddColor(palette, fill, styleColor);
		}
		#endregion
		#region SetupSeriesBorder
		void SetupSeriesBorder(Outline currentSeriesOutline, ChartsModel.SeriesAppearance appearance, ChartAppearanceElementBuilder builder) {
			appearance.Border = currentSeriesOutline.IsDefault ? GetSeriesBorder(builder) : GetBorder(currentSeriesOutline);
		}
		#region GetSeriesBorder
		ChartsModel.Border GetSeriesBorder(ChartAppearanceElementBuilder builder) {
			return ShouldUseDefaultSeriesBorder(builder.Style) ? null : GetBorder(builder); 
		}
		bool ShouldUseDefaultSeriesBorder(int style) {
			return style >= 33 && style <= 40; 
		}
		#endregion
		#endregion
		#region SetupSeriesShadow
		void SetupSeriesShadow(DrawingEffectStyle currentSeriesEffectStyle, ChartsModel.SeriesAppearance appearance, ChartAppearanceElementBuilder builder) {
			if (currentSeriesEffectStyle.IsDefault)
				SetupSeriesShadow(appearance, builder);
			else
				appearance.Shadow = shadowFactory.Create(currentSeriesEffectStyle, DXColor.Empty);
		}
		void SetupSeriesShadow(ChartsModel.SeriesAppearance appearance, ChartAppearanceElementBuilder builder) {
			StyleMatrixElementType themedEffect = builder.GetThemedEffect();
			if (themedEffect != StyleMatrixElementType.None) 
				appearance.Shadow = CreateSeriesShadow(themedEffect, builder.GetEffectColorInfo());
			else
				appearance.Shadow = null;
		}
		ChartsModel.Shadow CreateSeriesShadow(StyleMatrixElementType themedEffect, ChartAppearanceColorInfo effectColorInfo) {
			DrawingEffectStyle style = Theme.FormatScheme.GetEffectStyle(themedEffect);
			Color styleColor = effectColorInfo.ToRgb(Theme.Colors);
			return shadowFactory.Create(style, styleColor);
		}
		#endregion
		#region SetupLabelAppearance
		void SetupLabelAppearance(ChartsModel.SeriesAppearance appearance, ChartsModel.ColorARGB textColor) {
			if (appearance.LabelAppearance == null)
				appearance.LabelAppearance = new ChartsModel.SeriesLabelAppearance();
			ChartsModel.Border border = new ChartsModel.Border();
			border.Thickness = 0;
			appearance.LabelAppearance.Border = border;
			appearance.LabelAppearance.LineVisible = IsBestFitDataLabelPosition();
			appearance.LabelAppearance.TextColor = textColor;
			appearance.LabelAppearance.BackColor = transparentColor;
		}
		bool IsBestFitDataLabelPosition() {
			if (Views.Count == 0 || Views[0].Series.Count == 0)
				return false;
			ChartViewWithDataLabels view = Views[0] as ChartViewWithDataLabels;
			SeriesWithDataLabelsAndPoints series = Views[0].Series[0] as SeriesWithDataLabelsAndPoints;
			if (view == null || series == null)
				return false;
			if (!DataLabelsHelper.IsDataLabelsVisible(view.DataLabels, series.DataLabels))
				return false;
			DataLabelPosition position = DataLabelsHelper.GetDataLabelsPosition(view.DataLabels, series.DataLabels, view.DefaultDataLabelPosition);
			return position == DataLabelPosition.BestFit;
		}
		#endregion
		#region SetupLineStyle
		void SetupLineStyle(ChartsModel.SeriesAppearance appearance) {
			int viewCount = Views.Count;
			for (int i = 0; i < viewCount; i++) {
				SeriesCollection seriesCollection = views[i].Series;
				int seriesCount = seriesCollection.Count;
				for (int j = 0; j < seriesCount; j++)
					if (HasLineSeries(seriesCollection[j])) {
						ChartAppearanceLinesForDataPointsBuilder builder = GetInstanceBuilder(ChartElement.LinesForDataPoints) as ChartAppearanceLinesForDataPointsBuilder;
						SetupLineStyleForLines(builder, appearance);
						return;
					}
			}
		}
		bool HasLineSeries(ISeries series) {
			return series.SeriesType == ChartSeriesType.Radar ||
				   series.SeriesType == ChartSeriesType.Scatter ||
				   series.SeriesType == ChartSeriesType.Surface ||
				   (series.SeriesType == ChartSeriesType.Line && !series.View.Is3DView);
		}
		bool IsMarkerOnlySeries(ISeries series) {
			if (series.SeriesType != ChartSeriesType.Scatter)
				return false;
			ScatterSeries scatterSeries = series as ScatterSeries;
			ScatterChartStyle scatterStyle = scatterSeries.GetActualScatterStyle();
			return scatterStyle == ScatterChartStyle.Marker;
		}
		void SetupLineStyleForLines(ChartAppearanceLinesForDataPointsBuilder builder, ChartsModel.SeriesAppearance appearance) {
			StyleMatrixElementType themedLine = builder.GetThemedLine();
			Outline outline = Theme.FormatScheme.GetOutline(themedLine);
			int themedWidth = builder.GetWidthLine();
			int thickness = GetLineThickness(outline.Width * themedWidth);
			if (thickness == 0)
				thickness = 1;
			ChartsModel.DashStyle dashStyle = GetDashStyle(outline.Dashing);
			appearance.LineStyle = GetLineStyle(thickness, dashStyle);
		}
		void SetupLineStyleForLines(ChartAppearanceLinesForDataPointsBuilder builder, ChartsModel.SeriesAppearance appearance, Outline curentSeriesOutline) {
			StyleMatrixElementType themedLine = builder.GetThemedLine();
			Outline themedOutline = Theme.FormatScheme.GetOutline(themedLine);
			int themedWidth = builder.GetWidthLine();
			int thickness = GetSeriesThickness(themedOutline, curentSeriesOutline, themedWidth);
			ChartsModel.DashStyle dashStyle = GetSeriesDashStyle(themedOutline, curentSeriesOutline);
			appearance.LineStyle = GetLineStyle(thickness, dashStyle);
		}
		ChartsModel.DashStyle GetSeriesDashStyle(Outline themedOutline, Outline curentSeriesOutline) {
			return GetDashStyle(curentSeriesOutline.HasDashing ? curentSeriesOutline.Dashing : themedOutline.Dashing);
		}
		int GetSeriesThickness(Outline themedOutline, Outline curentSeriesOutline, int themedWidth) {
			return GetLineThickness(curentSeriesOutline.HasWidth ? curentSeriesOutline.Width : themedOutline.Width * themedWidth);
		}
		#endregion
		#endregion
		#region Helper methods
		ChartsModel.ColorARGB CalculateTextColor() {
			return GetStyleLineColor(ChartElement.Axis);
		}
		ChartsModel.ColorARGB GetStyleLineColor(ChartElement chartElement) {
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(chartElement);
			ChartAppearanceColorInfo colorInfo = builder.GetLineColorInfo();
			return ConvertColor(colorInfo.ToRgb(Theme.Colors));
		}
		Outline GetOutline(ChartAppearanceElementBuilder builder) {
			StyleMatrixElementType themedLine = builder.GetThemedLine();
			return Theme.FormatScheme.GetOutline(themedLine);
		}
		#region GetLineColor
		ChartsModel.ColorARGB GetLineColor(StyleMatrixElementType themedLine, ChartAppearanceColorInfo lineColorInfo) {
			Outline outline = Theme.FormatScheme.GetOutline(themedLine);
			return GetLineColor(outline, lineColorInfo);
		}
		ChartsModel.ColorARGB GetLineColor(Outline outline, ChartAppearanceColorInfo lineColorInfo) {
			Color styleColor = lineColorInfo.ToRgb(Theme.Colors);
			return GetLineColor(outline, styleColor);
		}
		ChartsModel.ColorARGB GetLineColor(Outline outline, Color styleColor) {
			fillColorsFactory.Prepare(outline.Fill, styleColor);
			return ConvertColor(fillColorsFactory.FirstColor);
		}
		#endregion
		IDrawingFill GetFill(ChartAppearanceElementBuilder builder) {
			StyleMatrixElementType themedFill = builder.GetThemedFill();
			if (themedFill != StyleMatrixElementType.None)
				return Theme.FormatScheme.GetFill(themedFill);
			return DrawingFill.Automatic;
		}
		void PrepareFillColorFactory(IDrawingFill fill, ChartAppearanceElementBuilder builder) {
			ChartAppearanceColorInfo fillColorInfo = builder.GetFillColorInfo();
			Color styleColor = fillColorInfo.ToRgb(Theme.Colors);
			fillColorsFactory.Prepare(fill, styleColor);
		}
		ChartAppearanceElementBuilder GetInstanceBuilder(ChartElement chartElement) {
			return ChartAppearanceElementBuilderFactory.GetInstanceBuilder(chartElement, style);
		}
		#region SetupColor
		void SetupColor<T>(ChartAppearanceElementBuilder builder, IDrawingFill fill, Action<T, ChartsModel.FillStyle> fillStyleSetter, Action<T, ChartsModel.ColorARGB> firstColorSetter, Action<T, ChartsModel.ColorARGB> secondColorSetter, T appearance) {
			if (fill.FillType != DrawingFillType.Automatic && fill.FillType != DrawingFillType.Picture && fill.FillType != DrawingFillType.Pattern)
				SetColorFromShapeProperties(fill, fillStyleSetter, firstColorSetter, secondColorSetter, appearance);
			else
				SetColorFromStyle(builder, fillStyleSetter, firstColorSetter, secondColorSetter, appearance);
		}
		void SetColorFromShapeProperties<T>(IDrawingFill fill, Action<T, ChartsModel.FillStyle> fillStyleSetter, Action<T, ChartsModel.ColorARGB> firstColorSetter, Action<T, ChartsModel.ColorARGB> secondColorSetter, T appearance) {
			SetFillStyleCore(fill, fillStyleSetter, appearance);
			fillColorsFactory.Prepare(fill, DXColor.Transparent);
			SetColorCore(fill.FillType, firstColorSetter, secondColorSetter, appearance);
		}
		void SetColorFromStyle<T>(ChartAppearanceElementBuilder builder, Action<T, ChartsModel.FillStyle> fillStyleSetter, Action<T, ChartsModel.ColorARGB> firstColorSetter, Action<T, ChartsModel.ColorARGB> secondColorSetter, T appearance) {
			IDrawingFill fill = GetFill(builder);
			SetFillStyleCore(fill, fillStyleSetter, appearance);
			PrepareFillColorFactory(fill, builder);
			SetColorCore(fill.FillType, firstColorSetter, secondColorSetter, appearance);
		}
		void SetFillStyleCore<T>(IDrawingFill fill, Action<T, ChartsModel.FillStyle> fillStyleSetter, T appearance) {
			ChartsModel.FillStyle fillStyle = fillStyleFactory.Create(fill);
			fillStyleSetter(appearance, fillStyle);
		}
		void SetColorCore<T>(DrawingFillType fillType, Action<T, ChartsModel.ColorARGB> firstColorSetter, Action<T, ChartsModel.ColorARGB> secondColorSetter, T appearance) {
			SetFirstColor(firstColorSetter, appearance);
			if (fillType == DrawingFillType.Gradient)
				SetSecondColor(secondColorSetter, appearance);
		}
		void SetFirstColor<T>(Action<T, ChartsModel.ColorARGB> colorSetter, T appearance) {
			ChartsModel.ColorARGB firstColor = ConvertColor(fillColorsFactory.FirstColor);
			colorSetter(appearance, firstColor);
		}
		void SetSecondColor<T>(Action<T, ChartsModel.ColorARGB> secondColorSetter, T appearance) {
			ChartsModel.ColorARGB secondColor = ConvertColor(fillColorsFactory.SecondColor);
			secondColorSetter(appearance, secondColor);
		}
		#endregion
		#region SetupLines
		void SetupLines<T>(ChartElement chartElement, Action<T, ChartsModel.ColorARGB> colorSetter, Action<T, ChartsModel.LineStyle> lineStyleSetter, T appearance) {
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(chartElement);
			StyleMatrixElementType themedLine = builder.GetThemedLine();
			if (themedLine != StyleMatrixElementType.None) {
				ChartAppearanceColorInfo colorInfo = builder.GetLineColorInfo();
				colorSetter(appearance, GetLineColor(themedLine, colorInfo));
				Outline outline = Theme.FormatScheme.GetOutline(themedLine);
				if (outline.HasDashing) {
					int thickness = GetLineThickness(outline.Width);
					ChartsModel.DashStyle dashStyle = GetDashStyle(outline.Dashing);
					lineStyleSetter(appearance, GetLineStyle(thickness, dashStyle));
				}
			}
		}
		ChartsModel.LineStyle GetLineStyle(int thickness, ChartsModel.DashStyle style) {
			ChartsModel.LineStyle result = new ChartsModel.LineStyle();
			result.Thickness = thickness;
			result.DashStyle = style;
			return result;
		}
		int GetLineThickness(int outlineModelUnits) {
			return Math.Max(1, (int)DocumentModel.UnitConverter.ModelUnitsToPixelsF(outlineModelUnits, DocumentModel.Dpi));
		}
		ChartsModel.DashStyle GetDashStyle(OutlineDashing dashing) {
			if (dashing == OutlineDashing.SystemDash || dashing == OutlineDashing.Dash || dashing == OutlineDashing.LongDash)
				return ChartsModel.DashStyle.Dash;
			if (dashing == OutlineDashing.SystemDot || dashing == OutlineDashing.Dot)
				return ChartsModel.DashStyle.Dot;
			if (dashing == OutlineDashing.SystemDashDot || dashing == OutlineDashing.DashDot || dashing == OutlineDashing.LongDashDot)
				return ChartsModel.DashStyle.DashDot;
			if (dashing == OutlineDashing.SystemDashDotDot || dashing == OutlineDashing.LongDashDotDot)
				return ChartsModel.DashStyle.DashDotDot;
			return ChartsModel.DashStyle.Solid;
		}
		#endregion
		#region GetBorder
		ChartsModel.Border GetBorder(Outline outline) {
			if (outline.Fill.FillType == DrawingFillType.None)
				return GetTransparentBorder();
			int thickness = GetLineThickness(outline.Width);
			ChartsModel.ColorARGB color = GetLineColor(outline, DXColor.Empty);
			return GetBorder(thickness, color);
		}
		ChartsModel.Border GetBorder(ChartAppearanceElementBuilder builder) {
			StyleMatrixElementType borderThemedLine = builder.GetThemedLine();
			if (borderThemedLine != StyleMatrixElementType.None) {
				Outline outline = Theme.FormatScheme.GetOutline(borderThemedLine);
				if (outline.Fill.FillType == DrawingFillType.None)
					return GetTransparentBorder();
				int thickness = GetLineThickness(outline.Width);
				ChartsModel.ColorARGB color = GetLineColor(outline, builder.GetLineColorInfo());
				return GetBorder(thickness, color);
			}
			return GetTransparentBorder();
		}
		ChartsModel.Border GetBorder(int thickness, ChartsModel.ColorARGB color) {
			ChartsModel.Border result = new ChartsModel.Border();
			result.Thickness = thickness;
			result.Color = color;
			return result;
		}
		ChartsModel.Border GetTransparentBorder() {
			ChartsModel.Border result = new ChartsModel.Border();
			result.Thickness = 0;
			return result;
		}
		#endregion
		List<ISeries> GetOrderedSeries(IChartView view) {
			List<ISeries> result = view.Series.ByOrder();
			if (view.ViewType == ChartViewType.Bar3D) {
				BarChartViewBase barView = view as BarChartViewBase;
				System.Diagnostics.Debug.Assert(barView != null);
				if (barView.Grouping == BarChartGrouping.Standard)
					result.Reverse();
			}
			return result;
		}
		int GetHighestSeriesIndex(List<ISeries> series) {
			int seriesCount = series.Count;
			if (seriesCount == 0)
				return 0;
			int result = series[0].Index;
			for (int i = 0; i < seriesCount; i++) {
				int seriesIndex = series[i].Index;
				if (seriesIndex > result)
					result = seriesIndex;
			}
			return result;
		}
		bool HasVaryColor() {
			int viewCount = Views.Count;
			for (int i = 0; i < viewCount; i++) {
				ChartViewWithVaryColors view = views[i] as ChartViewWithVaryColors;
				if (view != null && view.VaryColors)
					return true;
			}
			return false;
		}
		#endregion
		#endregion
		#region SetPropertyAccessors
		void SetChartAppearanceFillStyle(ChartsModel.ChartAppearance appearance, ChartsModel.FillStyle style) {
			appearance.FillStyle = style;
		}
		void SetChartAppearanceBackColor(ChartsModel.ChartAppearance appearance, ChartsModel.ColorARGB color) {
			appearance.BackColor = color;
		}
		void SetChartAppearanceColor2(ChartsModel.ChartAppearance appearance, ChartsModel.ColorARGB color) {
			appearance.FillStyle.Options.Color2 = color;
		}
		void SetLegendAppearanceFillStyle(ChartsModel.LegendAppearance appearance, ChartsModel.FillStyle style) {
			appearance.FillStyle = style;
		}
		void SetLegendAppearanceBackColor(ChartsModel.LegendAppearance appearance, ChartsModel.ColorARGB color) {
			appearance.BackColor = color;
		}
		void SetLegendAppearanceColor2(ChartsModel.LegendAppearance appearance, ChartsModel.ColorARGB color) {
			appearance.FillStyle.Options.Color2 = color;
		}	 
		void SetDiagrammAppearanceFillStyle(ChartsModel.DiagrammAppearance appearance, ChartsModel.FillStyle style) {
			appearance.FillStyle = style;
		}
		void SetDiagrammAppearanceBackColor(ChartsModel.DiagrammAppearance appearance, ChartsModel.ColorARGB color) {
			appearance.BackColor = color;
		}
		void SetDiagrammAppearanceColor2(ChartsModel.DiagrammAppearance appearance, ChartsModel.ColorARGB color) {
			appearance.FillStyle.Options.Color2 = color;
		}
		void SetMajorGridLinesColor(ChartsModel.AxisAppearance appearance, ChartsModel.ColorARGB color) {
			appearance.GridLinesColor = color;
		}
		void SetMinorGridLinesColor(ChartsModel.AxisAppearance appearance, ChartsModel.ColorARGB color) {
			appearance.GridLinesMinorColor = color;
		}
		void SetMajorGridLinesLineStyle(ChartsModel.AxisAppearance appearance, ChartsModel.LineStyle style) {
			appearance.GridLinesLineStyle = style;
		}
		void SetMinorGridLinesLineStyle(ChartsModel.AxisAppearance appearance, ChartsModel.LineStyle style) {
			appearance.GridLinesMinorLineStyle = style;
		}
		#endregion
		#region Factories
		#region FillStyleFactory (class)
		class FillStyleFactory : IDrawingFillVisitor {
			ChartsModel.FillStyle style;
			public ChartsModel.FillStyle Create(IDrawingFill fill) {
				fill.Visit(this);
				ChartsModel.FillStyle result = style;
				style = null;
				return result;
			}
			#region Internal 
			void CreateFillStyle(ChartsModel.FillMode fillMode) {
				style = new ChartsModel.FillStyle();
				style.FillMode = fillMode;
				CreateDefaultFillOptions();
			}
			void CreateDefaultFillOptions() {
				if (style.Options == null)
					style.Options = new ChartsModel.FillOptions();
				style.Options.GradientMode = ChartsModel.GradientMode.BottomToTop; 
			}
			void SetLinearGradientMode(int positiveFixedAngle) {
				ChartsModel.FillOptions options = style.Options;
				int degree = positiveFixedAngle / DrawingValueConstants.OnePositiveFixedAngle;
				if ((degree >= 315 && degree <= 360) || degree < 45)
					options.GradientMode = ChartsModel.GradientMode.LeftToRight;
				if (degree >= 45 && degree < 135)
					options.GradientMode = ChartsModel.GradientMode.TopToBottom;
				if (degree >= 135 && degree < 225)
					options.GradientMode = ChartsModel.GradientMode.RightToLeft;
			}
			#endregion
			#region IDrawingFillVisitor Members
			public void Visit(DrawingFill fill) {
				CreateFillStyle(ChartsModel.FillMode.Empty);
			}
			public void Visit(DrawingSolidFill fill) {
				CreateFillStyle(ChartsModel.FillMode.Solid);
			}
			public void Visit(DrawingPatternFill fill) {
				CreateFillStyle(ChartsModel.FillMode.Empty);
			}
			public void Visit(DrawingGradientFill fill) {
				CreateFillStyle(ChartsModel.FillMode.Gradient);
				if (fill.GradientType == GradientType.Linear)
					SetLinearGradientMode(fill.Angle);
			}
			public void Visit(DrawingBlipFill fill) {
				CreateFillStyle(ChartsModel.FillMode.Empty);
			}
			#endregion
		}
		#endregion
		#region FillColorsFactory (class)
		class FillColorsFactory : IDrawingFillVisitor {
			Color styleColor;
			#region Properties
			public Color FirstColor { get; set; }
			public Color SecondColor { get; set; }
			#endregion
			public void Prepare(IDrawingFill fill, Color styleColor) {
				this.styleColor = styleColor;
				fill.Visit(this);
			}
			#region Internal
			void CreateDefaultColor() {
				FirstColor = styleColor;
			}
			Color GetFirstGradientColor(GradientStopCollection gradientStops, Color styleColor) {
				return gradientStops[0].Color.ToRgb(styleColor); 
			}
			Color GetLastGradientColor(GradientStopCollection gradientStops, Color styleColor) {
				return gradientStops[gradientStops.Count - 1].Color.ToRgb(styleColor); 
			}
			#endregion
			#region IDrawingFillVisitor Members
			public void Visit(DrawingFill fill) {
				CreateDefaultColor();
			}
			public void Visit(DrawingSolidFill fill) {
				FirstColor = fill.Color.ToRgb(styleColor);
			}
			public void Visit(DrawingPatternFill fill) {
				CreateDefaultColor();
			}
			public void Visit(DrawingGradientFill fill) {
				if (fill.GradientStops.Count < 2)
					CreateDefaultColor();
				else {
					GradientStopCollection gradientStops = fill.GradientStops;
					FirstColor = GetFirstGradientColor(gradientStops, styleColor);
					SecondColor = GetLastGradientColor(gradientStops, styleColor);
				}
			}
			public void Visit(DrawingBlipFill fill) {
				CreateDefaultColor();
			}
			#endregion
		}
		#endregion
		#region FillPaletteEntryFactory (class)
		class FillPaletteEntryFactory : IDrawingFillVisitor {
			#region Fields
			internal static FillColorsFactory FillColorsFactory = new FillColorsFactory();
			internal static IsReverseSeriesVisitor isReverseSeriesVisitor = new IsReverseSeriesVisitor();
			ChartsModel.PaletteEntry result;
			int insertIndex;
			bool isReverseSeries;
			#endregion
			public void Prepare(IChartView view, ChartsModel.Palette palette) {
				view.Visit(isReverseSeriesVisitor);
				isReverseSeries = isReverseSeriesVisitor.Result;
				if (isReverseSeries) 
					insertIndex = palette.Entries.Count;
			}
			public void AddColor(ChartsModel.Palette palette, IDrawingFill fill, Color styleColor) {
				if (isReverseSeries)
					palette.Entries.Insert(insertIndex, Create(fill, styleColor));
				else
					palette.Entries.Add(Create(fill, styleColor));
			}
			#region Internal
			ChartsModel.PaletteEntry Create(IDrawingFill fill, Color styleColor) {
				FillColorsFactory.Prepare(fill, styleColor);
				fill.Visit(this);
				return result;
			}
			void CreateDefaultPaletteEntry() {
				result = new ChartsModel.PaletteEntry(ConvertColor(FillColorsFactory.FirstColor));
			}
			#endregion
			#region IDrawingFillVisitor Members
			public void Visit(DrawingFill fill) {
				FillColorsFactory.Visit(fill);
				CreateDefaultPaletteEntry();
			}
			public void Visit(DrawingSolidFill fill) {
				FillColorsFactory.Visit(fill);
				CreateDefaultPaletteEntry();
			}
			public void Visit(DrawingPatternFill fill) {
				FillColorsFactory.Visit(fill);
				CreateDefaultPaletteEntry();
			}
			public void Visit(DrawingGradientFill fill) {
				FillColorsFactory.Visit(fill);
				if (fill.GradientStops.Count < 2)
					CreateDefaultPaletteEntry();
				else
					result = new ChartsModel.PaletteEntry(ConvertColor(FillColorsFactory.FirstColor), ConvertColor(FillColorsFactory.SecondColor));
			}
			public void Visit(DrawingBlipFill fill) {
				FillColorsFactory.Visit(fill);
				CreateDefaultPaletteEntry();
			}
			#endregion
		}
		#endregion
		#region ShadowFactory (class)
		class ShadowFactory : IDrawingEffectVisitor {
			#region Fields
			Color styleColor;
			ChartsModel.Shadow shadow;
			#endregion
			public ChartsModel.Shadow Create(DrawingEffectStyle style, Color styleColor) {
				PrepareCore(styleColor);
				style.ApplyEffects(this);
				return GetResult();
			}
			void PrepareCore(Color styleColor) {
				shadow = null;
				this.styleColor = styleColor;
			}
			ChartsModel.Shadow GetResult() {
				ChartsModel.Shadow result = shadow;
				shadow = null;
				return result;
			}
			#region IDrawingEffectVisitor Members
			public void AlphaCeilingEffectVisit() {
			}
			public void AlphaFloorEffectVisit() {
			}
			public void GrayScaleEffectVisit() {
			}
			public void Visit(AlphaBiLevelEffect drawingEffect) {
			}
			public void Visit(AlphaInverseEffect drawingEffect) {
			}
			public void Visit(AlphaModulateEffect drawingEffect) {
			}
			public void Visit(AlphaModulateFixedEffect drawingEffect) {
			}
			public void Visit(AlphaOutsetEffect drawingEffect) {
			}
			public void Visit(AlphaReplaceEffect drawingEffect) {
			}
			public void Visit(BiLevelEffect drawingEffect) {
			}
			public void Visit(BlendEffect drawingEffect) {
			}
			public void Visit(BlurEffect drawingEffect) {
			}
			public void Visit(ColorChangeEffect drawingEffect) {
			}
			public void Visit(ContainerEffect drawingEffect) {
			}
			public void Visit(DuotoneEffect drawingEffect) {
			}
			public void Visit(Effect drawingEffect) {
			}
			public void Visit(FillEffect drawingEffect) {
			}
			public void Visit(FillOverlayEffect drawingEffect) {
			}
			public void Visit(GlowEffect drawingEffect) {
			}
			public void Visit(HSLEffect drawingEffect) {
			}
			public void Visit(InnerShadowEffect drawingEffect) {
			}
			public void Visit(LuminanceEffect drawingEffect) {
			}
			public void Visit(OuterShadowEffect drawingEffect) {
				shadow = new ChartsModel.Shadow();
				shadow.Color = ConvertColor(drawingEffect.Color.ToRgb(styleColor));
			}
			public void Visit(PresetShadowEffect drawingEffect) {
			}
			public void Visit(ReflectionEffect drawingEffect) {
			}
			public void Visit(RelativeOffsetEffect drawingEffect) {
			}
			public void Visit(SoftEdgeEffect drawingEffect) {
			}
			public void Visit(SolidColorReplacementEffect drawingEffect) {
			}
			public void Visit(TintEffect drawingEffect) {
			}
			public void Visit(TransformEffect drawingEffect) {
			}
			#endregion
		}
		#endregion
		#endregion
		#region IChartAppearance Members
		#region Internal
		void PrepareFillColorFactory(IDrawingFill fill, ChartAppearanceColorInfo colorInfo, int index, int maxIndex) {
			Color styleColor = colorInfo.GetSeriesColor(index, maxIndex, Theme.Colors);
			fillColorsFactory.Prepare(fill, styleColor);
		}
		void PrepareFillColorFactory(ChartElement element, int index, int maxIndex) {
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(element);
			IDrawingFill fill = GetFill(builder);
			PrepareFillColorFactory(fill, builder.GetFillColorInfo(), index, maxIndex);
		}
		Outline GetOutline(ChartElement element) {
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(element);
			return GetOutline(builder);
		}
		#endregion
		public DrawingFillType GetFillType(ChartElement element, int index, int maxIndex) {
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(element);
			StyleMatrixElementType themedFill = builder.GetThemedFill();
			if (themedFill != StyleMatrixElementType.None)
				return Theme.FormatScheme.GetFill(themedFill).FillType;
			return DrawingFillType.None;
		}
		public Color GetFillForegroundColor(ChartElement element, int index, int maxIndex) {
			PrepareFillColorFactory(element, index, maxIndex);
			return fillColorsFactory.FirstColor;
		}
		public Color GetFillBackgroundColor(ChartElement element, int index, int maxIndex) {
			PrepareFillColorFactory(element, index, maxIndex);
			return fillColorsFactory.SecondColor;
		}
		public DrawingFillType GetOutlineFillType(ChartElement element, int index, int maxIndex) {
			Outline outline = GetOutline(element);
			return outline != null ? outline.Fill.FillType : DrawingFillType.None;
		}
		public Color GetOutlineColor(ChartElement element, int index, int maxIndex) {
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(element);
			if (element == ChartElement.LinesForDataPoints || element == ChartElement.MarkersForDataPoints)
				return builder.GetLineColorInfo().GetSeriesColor(index, maxIndex, Theme.Colors);
			Outline outline = GetOutline(builder);
			if (outline == null)
				return DXColor.Empty;
			PrepareFillColorFactory(outline.Fill, builder.GetLineColorInfo(), index, maxIndex);
			return fillColorsFactory.FirstColor;
		}
		public int GetOutlineWidth(ChartElement element, int index, int maxIndex) {
			ChartAppearanceElementBuilder builder = GetInstanceBuilder(element);
			Outline outline = GetOutline(builder);
			if (outline == null)
				return 0;
			int result = outline.Width;
			ChartAppearanceLinesForDataPointsBuilder linesBuilder = builder as ChartAppearanceLinesForDataPointsBuilder;
			return linesBuilder != null ? result * linesBuilder.GetWidthLine() : result;
		}
		public OutlineDashing GetOutlineDashing(ChartElement element, int index, int maxIndex) {
			Outline outline = GetOutline(element);
			return outline != null ? outline.Dashing : OutlineDashing.Solid;
		}
		public DrawingPatternType GetOutlineFillPatternType(ChartElement element, int index, int maxIndex) {
			Outline outline = GetOutline(element);
			if (outline == null)
				Exceptions.ThrowInternalException();
			DrawingPatternFill fill = outline.Fill as DrawingPatternFill;
			if (fill == null)
				Exceptions.ThrowInternalException();
			return fill.PatternType;
		}
		#endregion
	}
	#endregion 
}
