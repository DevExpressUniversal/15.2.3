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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using DevExpress.Data;
using DevExpress.Data.IO;
using DevExpress.Export.Xl;
using DevExpress.Web.Data;
using DevExpress.Web.Internal;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.Web {
	public abstract class GridFormatConditionCollection : DevExpress.Web.Collection<GridFormatConditionBase> {
		DevExpress.Web.Collection<FormatConditionSummary> summaries;
		public GridFormatConditionCollection()
			: this(null, null) {
		}
		protected internal GridFormatConditionCollection(IWebControlObject owner, Action formatSummaryChangedHandler)
			: base(owner) {
			FormatSummaryChanged = formatSummaryChangedHandler;
		}
		protected IEnumerable<GridFormatConditionBase> ActiveConditions { get { return this.Where(c => c.Enabled); } }
		protected internal IEnumerable<GridFormatConditionBase> GetActiveColumnCellConditions(IWebGridDataColumn column) {
			return ActiveConditions.Where(c => c.FieldName == column.FieldName && !c.ApplyToItem);
		}
		protected internal IEnumerable<GridFormatConditionBase> GetActiveItemConditions() {
			return ActiveConditions.Where(c => c.ApplyToItem);
		}
		protected Action FormatSummaryChanged { get; private set; }
		protected internal void OnFormatSummaryChanged(){
			if(IsLoading || Owner == null || FormatSummaryChanged == null)
				return;
			this.summaries = null;
			FormatSummaryChanged();
		}
		protected override void OnChanged() {
			base.OnChanged();
			OnFormatSummaryChanged();
		}
		protected internal DevExpress.Web.Collection<FormatConditionSummary> Summaries {
			get {
				if(summaries == null)
					summaries = CreateFormatConditionSummaries();
				return summaries;
			}
		}
		Collection<FormatConditionSummary> CreateFormatConditionSummaries() {
			Collection<FormatConditionSummary> summaries = new Collection<FormatConditionSummary>();
			for(int i = 0; i < Count; i++){
				var conditionSummaries = this[i].CreateSummaries();
				foreach(var item in conditionSummaries){
					if(!summaries.Any(s => s.FieldName == item.FieldName && s.SummaryType == item.SummaryType))
						summaries.Add(item);
				}
			}
			return summaries;
		}
		protected internal byte[] SaveCacheState(WebDataProxy dataProxy) {
			using(MemoryStream stream = new MemoryStream())
			using(var writer = new TypedBinaryWriter(stream)) {
				for(int i = 0; i < Count; i++) {
					this[i].EnshureCache(dataProxy);
					this[i].SaveCache(writer);
				}
				return stream.ToArray();
			}
		}
		protected internal void LoadCacheState(byte[] data) {
			if(data == null || data.Length < 1)
				return;
			using(MemoryStream stream = new MemoryStream(data))
			using(var reader = new TypedBinaryReader(stream)) {
				for(int i = 0; i < Count; i++)
					this[i].LoadCache(reader);
			}
		}
		protected internal void ResetCache() {
			for(int i = 0; i < Count; i++)
				this[i].ResetCache();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class GridFormatConditionBase : CollectionItem {
		protected static ValueComparer ValueComparer = new ValueComparer();
		protected internal abstract bool IsFitToRule(WebDataProxy dataProxy, int visibleIndex);
		protected internal abstract AppearanceStyle GetItemStyle(WebDataProxy dataProxy, int visibleIndex);
		protected internal abstract AppearanceStyle GetItemCellStyle(WebDataProxy dataProxy, int visibleIndex);
		protected internal abstract IFormatConditionRuleBase GetExportRule();
		protected internal string FieldName {
			get { return GetStringProperty("FieldName", string.Empty); }
			set {
				if(FieldName == value) return;
				SetStringProperty("FieldName", string.Empty, value);
				Changed();
			}
		}
		protected internal bool ApplyToItem {
			get { return GetBoolProperty("ApplyToItem", false); }
			set { SetBoolProperty("ApplyToItem", false, value); }
		}
		protected internal bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set {
				if(Enabled == value) return;
				SetBoolProperty("Enabled", true, value);
				Changed();
			}
		}
		protected internal new GridFormatConditionCollection Collection { get { return (GridFormatConditionCollection)base.Collection; } }
		protected IEnumerable<FormatConditionSummary> ColumnSummaries {
			get {
				if(Collection == null)
					return Enumerable.Empty<FormatConditionSummary>();
				return Collection.Summaries.Where(i => i.FieldName == FieldName);
			}
		}
		protected internal virtual IEnumerable<FormatConditionSummary> CreateSummaries() {
			return Enumerable.Empty<FormatConditionSummary>();
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var condition = source as GridFormatConditionBase;
			if(condition != null) {
				FieldName = condition.FieldName;
				ApplyToItem = condition.ApplyToItem;
				Enabled = condition.Enabled;
			}
		}
		protected virtual string ConditionName { get { return string.Empty; } }
		protected override string GetDesignTimeCaption() {
			if(string.IsNullOrEmpty(ConditionName))
				return base.GetDesignTimeCaption();
			if(string.IsNullOrEmpty(FieldName))
				return ConditionName;
			return string.Format("{0} ({1})", ConditionName, FieldName);
		}
		protected static int CompareValues(object val1, object val2) {
			if(!IsNullValue(val1) && !IsNullValue(val2)) {
				val1 = DataUtils.ConvertToDecimalValue(val1);
				val2 = DataUtils.ConvertToDecimalValue(val2);
			}
			return ValueComparer.Compare(val1, val2);
		}
		protected static bool IsNullValue(object value) {
			return value == null || value == DBNull.Value;
		}
		protected virtual void Changed(){
			ResetCache();
			if(Collection != null)
				Collection.OnFormatSummaryChanged();
		}
		protected internal virtual void ResetCache() { }
		protected internal virtual void EnshureCache(WebDataProxy dataProxy) { }
		protected internal virtual void SaveCache(TypedBinaryWriter writer) { }
		protected internal virtual void LoadCache(TypedBinaryReader reader) { }
	}
	public enum GridConditionRule { None, Equal, NotEqual, Between, NotBetween, Less, Greater, GreaterOrEqual, LessOrEqual, Expression }
	public abstract class GridFormatConditionHighlight : GridFormatConditionExpressionBase {
		public GridFormatConditionHighlight() {
			ExpressionCache = new GridExpressionFormatConditionCache();
		}
		protected internal GridConditionRule Rule {
			get { return (GridConditionRule)GetEnumProperty("Rule", GridConditionRule.Expression); }
			set { SetEnumProperty("Rule", GridConditionRule.Expression, value); }
		}
		protected internal object Value1 {
			get { return GetObjectProperty("Value1", null); }
			set { SetObjectProperty("Value1", null, value); }
		}
		protected internal object Value2 {
			get { return GetObjectProperty("Value2", null); }
			set { SetObjectProperty("Value2", null, value); }
		}
		protected internal string Expression {
			get { return GetStringProperty("Expression", string.Empty); }
			set { SetStringProperty("Expression", string.Empty, value); }
		}
		protected GridExpressionFormatConditionCache ExpressionCache { get; private set; }
		protected override string ConditionName { get { return "Highlight condition"; } }
		protected internal override bool IsFitToRule(WebDataProxy dataProxy, int visibleIndex) {
			if(!Enabled || Rule == GridConditionRule.None)
				return false;
			if(Rule == GridConditionRule.Expression)
				return IsExpressionFitToRule(dataProxy, visibleIndex);
			object value = dataProxy.GetRowValue(visibleIndex, FieldName);
			switch(Rule) {
				case GridConditionRule.Equal:
					return CompareValues(value, Value1) == 0;
				case GridConditionRule.NotEqual:
					return CompareValues(value, Value1) != 0;
				case GridConditionRule.Less:
					return CompareValues(value, Value1) < 0;
				case GridConditionRule.Greater:
					return CompareValues(value, Value1) > 0;
				case GridConditionRule.GreaterOrEqual:
					return CompareValues(value, Value1) >= 0;
				case GridConditionRule.LessOrEqual:
					return CompareValues(value, Value1) <= 0;
				case GridConditionRule.Between:
					return CompareValues(value, Value1) > 0 && CompareValues(value, Value2) < 0;
				case GridConditionRule.NotBetween:
					return CompareValues(value, Value1) <= 0 || CompareValues(value, Value2) >= 0;
			}
			return false;
		}
		bool IsExpressionFitToRule(WebDataProxy dataProxy, int visibleIndex) {
			if(!ExpressionCache.IsReady)
				return dataProxy.IsExpressionFitToRule(Expression, visibleIndex);
			return ExpressionCache.IsFitToRule(visibleIndex - dataProxy.VisibleStartIndex);
		}
		protected internal override IFormatConditionRuleBase GetExportRule() {
			return new GridExportConditionValueRule(this);
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var condition = source as GridFormatConditionHighlight;
			if(condition != null) {
				Rule = condition.Rule;
				Value1 = condition.Value1;
				Value2 = condition.Value2;
				Expression = condition.Expression;
			}
		}
		protected internal override void ResetCache() {
			ExpressionCache.Reset();
		}
		protected internal override void EnshureCache(WebDataProxy dataProxy) {
			if(dataProxy.UseEndlessPaging || Rule != GridConditionRule.Expression) {
				ExpressionCache.Reset();
				return;
			}
			if(ExpressionCache.IsReady)
				return;
			ExpressionCache.VisibleItemCountOnPage = dataProxy.VisibleRowCountOnPage;
			for(int i = 0; i < ExpressionCache.VisibleItemCountOnPage; i++) {
				int visibleIndex = dataProxy.VisibleStartIndex + i;
				bool isFit = dataProxy.IsExpressionFitToRule(Expression, visibleIndex);
				ExpressionCache.CacheRuleResult(i, isFit);
			}
		}
		protected internal override void SaveCache(TypedBinaryWriter writer) {
			ExpressionCache.Serialize(writer);
		}
		protected internal override void LoadCache(TypedBinaryReader reader) {
			ExpressionCache.Deserialize(reader);
		}
	}
	public enum GridTopBottomRule { TopItems, TopPercent, BottomItems, BottomPercent, AboveAverage, BelowAverage }
	public abstract class GridFormatConditionTopBottom : GridFormatConditionExpressionBase {
		protected const decimal DefaultThreshold = 10;
		protected internal decimal Threshold {
			get { return GetDecimalProperty("Threshold", DefaultThreshold); }
			set { SetDecimalProperty("Threshold", DefaultThreshold, value); }
		}
		protected internal GridTopBottomRule Rule {
			get { return (GridTopBottomRule)GetEnumProperty("Rule", GridTopBottomRule.AboveAverage); }
			set {
				if(Rule == value) return;
				SetEnumProperty("Rule", GridTopBottomRule.AboveAverage, value);
				Changed();
			}
		}
		protected override string ConditionName { get { return "Top/Bottom condition"; } }
		internal bool IsAboveOrBelowAverageRule { get { return Rule == GridTopBottomRule.AboveAverage || Rule == GridTopBottomRule.BelowAverage; } }
		internal bool IsPercentRule { get { return Rule == GridTopBottomRule.TopPercent || Rule == GridTopBottomRule.BottomPercent; } }
		internal bool IsTopRule { get { return Rule == GridTopBottomRule.TopItems || Rule == GridTopBottomRule.TopPercent; } }
		FormatConditionSummary AverageSummary {
			get { return ColumnSummaries != null ? ColumnSummaries.FirstOrDefault(s => s.SummaryType == SummaryItemType.Average) : null; }
		}
		FormatConditionSummary SortedIndicesSummary {
			get { return ColumnSummaries != null ? ColumnSummaries.FirstOrDefault(s => s.SummaryType == SummaryItemType.Custom) : null; }
		}
		protected internal override IEnumerable<FormatConditionSummary> CreateSummaries() {
			var summary = new FormatConditionSummary();
			summary.FieldName = FieldName;
			summary.SummaryType = IsAboveOrBelowAverageRule ? SummaryItemType.Average : SummaryItemType.Custom;
			return new FormatConditionSummary[] { summary };
		}
		protected internal override IFormatConditionRuleBase GetExportRule() {
			if(IsAboveOrBelowAverageRule)
				return new GridExportConditionAboveBelowAverageRule(this);
			return new GridExportConditionTopBottomRule(this);
		}
		protected internal override bool IsFitToRule(WebDataProxy dataProxy, int visibleIndex) {
			if(IsAboveOrBelowAverageRule)
				return IsFitToAboveOrBelowAverageRule(dataProxy, visibleIndex);
			EnshureCache(dataProxy);
			int cmp = ValueComparer.Compare(dataProxy.GetRowValue(visibleIndex, FieldName), TopBottomThresholdCacheValue);
			return IsTopRule ? cmp >= 0 : cmp <= 0;
		}
		bool IsFitToAboveOrBelowAverageRule(WebDataProxy dataProxy, int visibleIndex) {
			decimal? value = DataUtils.ConvertToDecimalValue(dataProxy.GetRowValue(visibleIndex, FieldName));
			decimal? averageValue = DataUtils.ConvertToDecimalValue(dataProxy.GetFormatConditionSummaryValue(AverageSummary));
			if(!value.HasValue || !averageValue.HasValue)
				return false;
			return ValueComparer.Compare(value, averageValue) * GetAboveOrBelowAverageRuleSign() > 0;
		}
		int GetAboveOrBelowAverageRuleSign(){
			if(!IsAboveOrBelowAverageRule)
				throw new InvalidOperationException();
			return Rule == GridTopBottomRule.AboveAverage ? 1 : -1;
		}
		object CalculateTopBottomThresholdValue(WebDataProxy dataProxy) {
			int[] sortedIndices = dataProxy.GetFormatConditionSummaryValue(SortedIndicesSummary) as int[];
			if(sortedIndices == null || sortedIndices.Length == 0)
				return null;
			int topBottomItemsCount = GetTopBottomItemsCount(sortedIndices.Length);
			int listIndex = IsTopRule ? sortedIndices.Length - topBottomItemsCount : topBottomItemsCount - 1;
			return dataProxy.GetListSourceRowValue(sortedIndices[listIndex], FieldName);
		}
		int GetTopBottomItemsCount(int dataItemsCount) {
			decimal topBottomItemsCount = IsPercentRule ? Math.Max(1, Math.Floor(dataItemsCount * (Threshold / 100))) : Math.Min(int.MaxValue, Threshold);
			return (int)Math.Min(dataItemsCount, topBottomItemsCount);
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var condition = source as GridFormatConditionTopBottom;
			if(condition != null){
				Threshold = condition.Threshold;
				Rule = condition.Rule;
			}
		}
		object TopBottomThresholdCacheValue { get; set; }
		protected internal override void ResetCache() {
			TopBottomThresholdCacheValue = null;
		}
		protected internal override void EnshureCache(WebDataProxy dataProxy) {
			if(IsAboveOrBelowAverageRule) {
				ResetCache();
				return;
			}
			if(TopBottomThresholdCacheValue == null)
				TopBottomThresholdCacheValue = CalculateTopBottomThresholdValue(dataProxy);
		}
		protected internal override void LoadCache(TypedBinaryReader reader) {
			TopBottomThresholdCacheValue = reader.ReadTypedObject();
		}
		protected internal override void SaveCache(TypedBinaryWriter writer) {
			writer.WriteTypedObject(TopBottomThresholdCacheValue);
		}
	}
	public enum GridConditionHighlightFormat { LightRedFillWithDarkRedText, YellowFillWithDarkYellowText, GreenFillWithDarkGreenText, LightRedFill,
											   LightGreenFill, RedText, GreenText, BoldText, ItalicText, StrikethroughText, Custom }
	public abstract class GridFormatConditionExpressionBase : GridFormatConditionBase {
		static Color
			RedTextColor = ColorTranslator.FromHtml("#FF9C0006"),
			YellowTextColor = ColorTranslator.FromHtml("#FF9C6500"),
			GreenTextColor = ColorTranslator.FromHtml("#FF006100"),
			RedBackgroundColor = ColorTranslator.FromHtml("#FFFFC7CE"),
			YellowBackgroundColor = ColorTranslator.FromHtml("#FFFFEB9C"),
			GreenBackgroundColor = ColorTranslator.FromHtml("#FFC6EFCE");
		static IDictionary<GridConditionHighlightFormat, AppearanceStyle> PredefinedFormats = new Dictionary<GridConditionHighlightFormat, AppearanceStyle> {
			{ GridConditionHighlightFormat.LightRedFillWithDarkRedText, new AppearanceStyle { ForeColor = RedTextColor, BackColor = RedBackgroundColor } },
			{ GridConditionHighlightFormat.YellowFillWithDarkYellowText, new AppearanceStyle { ForeColor = YellowTextColor, BackColor = YellowBackgroundColor } },
			{ GridConditionHighlightFormat.GreenFillWithDarkGreenText, new AppearanceStyle { ForeColor = GreenTextColor, BackColor = GreenBackgroundColor } },
			{ GridConditionHighlightFormat.LightRedFill, new AppearanceStyle { BackColor = RedBackgroundColor } },
			{ GridConditionHighlightFormat.LightGreenFill, new AppearanceStyle { BackColor = GreenBackgroundColor } },
			{ GridConditionHighlightFormat.RedText, new AppearanceStyle { ForeColor = RedTextColor } },
			{ GridConditionHighlightFormat.GreenText, new AppearanceStyle { ForeColor = GreenTextColor } },
			{ GridConditionHighlightFormat.BoldText, CreateBoldTextStyle() },
			{ GridConditionHighlightFormat.ItalicText, CreateItalicTextStyle() },
			{ GridConditionHighlightFormat.StrikethroughText, CreateStrikethroughTextStyle() }
		};
		static AppearanceStyle CreateBoldTextStyle() {
			var style = new AppearanceStyle();
			style.Font.Bold = true;
			return style;
		}
		static AppearanceStyle CreateItalicTextStyle() {
			var style = new AppearanceStyle();
			style.Font.Italic = true;
			return style;
		}
		static AppearanceStyle CreateStrikethroughTextStyle() {
			var style = new AppearanceStyle();
			style.Font.Strikeout = true;
			return style;
		}
		public GridFormatConditionExpressionBase() {
			ItemStyle = CreateItemStyle();
			ItemCellStyle = CreateItemCellStyle();
		}
		protected internal GridConditionHighlightFormat Format {
			get { return (GridConditionHighlightFormat)GetEnumProperty("Format", GridConditionHighlightFormat.LightRedFillWithDarkRedText); }
			set { SetEnumProperty("Format", GridConditionHighlightFormat.LightRedFillWithDarkRedText, value); }
		}
		protected internal AppearanceStyle ItemStyle { get; private set; }
		protected internal AppearanceStyle ItemCellStyle { get; private set; }
		protected abstract AppearanceStyle CreateItemStyle();
		protected abstract AppearanceStyle CreateItemCellStyle();
		protected internal override AppearanceStyle GetItemStyle(WebDataProxy dataProxy, int visibleIndex) {
			if(Format == GridConditionHighlightFormat.Custom)
				return ItemStyle;
			return PredefinedFormats[Format];
		}
		protected internal override AppearanceStyle GetItemCellStyle(WebDataProxy dataProxy, int visibleIndex) {
			if(Format == GridConditionHighlightFormat.Custom)
				return ItemCellStyle;
			return PredefinedFormats[Format];
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ItemStyle, ItemCellStyle });
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var condition = source as GridFormatConditionExpressionBase;
			if(condition != null) {
				Format = condition.Format;
				ItemCellStyle.Assign(condition.ItemCellStyle);
				ItemStyle.Assign(condition.ItemStyle);
			}
		}
	}
	public enum GridConditionColorScaleFormat { GreenYellowRed, RedYellowGreen, GreenWhiteRed, RedWhiteGreen, BlueWhiteRed, RedWhiteBlue,
												 RedWhite, WhiteRed, GreenWhite, WhiteGreen, GreenYellow, YellowGreen, Custom };
	public abstract class GridFormatConditionColorScale : GridFormatConditionIndicatorBase {
		static Color
			Red = ColorTranslator.FromHtml("#FFF8696B"),
			Yellow = ColorTranslator.FromHtml("#FFFFEB84"),
			Green = ColorTranslator.FromHtml("#FF63BE7B"),
			Blue = ColorTranslator.FromHtml("#FF5A8AC6"),
			White = Color.White;
		static IDictionary<GridConditionColorScaleFormat, Color[]> PredefinedFormats = new Dictionary<GridConditionColorScaleFormat, Color[]> {
			{ GridConditionColorScaleFormat.GreenYellowRed, new Color[] { Green, Yellow, Red } },
			{ GridConditionColorScaleFormat.RedYellowGreen, new Color[] { Red, Yellow, Green } },
			{ GridConditionColorScaleFormat.GreenWhiteRed, new Color[] { Green, White, Red } },
			{ GridConditionColorScaleFormat.RedWhiteGreen, new Color[] { Red, White, Green } },
			{ GridConditionColorScaleFormat.BlueWhiteRed, new Color[] { Blue, White, Red } },
			{ GridConditionColorScaleFormat.RedWhiteBlue, new Color[] { Red, White, Blue } },
			{ GridConditionColorScaleFormat.RedWhite, new Color[] { Red, White } },
			{ GridConditionColorScaleFormat.WhiteRed, new Color[] { White, Red } },
			{ GridConditionColorScaleFormat.GreenWhite, new Color[] { Green, White } },
			{ GridConditionColorScaleFormat.WhiteGreen, new Color[] { White, Green } },
			{ GridConditionColorScaleFormat.GreenYellow, new Color[] { Green, Yellow } },
			{ GridConditionColorScaleFormat.YellowGreen, new Color[] { Yellow, Green } }
		};
		protected internal Color MinimumColor {
			get { return GetColorProperty("MinimumColor", Color.Empty); }
			set { SetColorProperty("MinimumColor", Color.Empty, value); }
		}
		protected internal Color MiddleColor {
			get { return GetColorProperty("MiddleColor", Color.Empty); }
			set { SetColorProperty("MiddleColor", Color.Empty, value); }
		}
		protected internal Color MaximumColor {
			get { return GetColorProperty("MaximumColor", Color.Empty); }
			set { SetColorProperty("MaximumColor", Color.Empty, value); }
		}
		protected internal GridConditionColorScaleFormat Format {
			get { return (GridConditionColorScaleFormat)GetEnumProperty("Format", GridConditionColorScaleFormat.GreenYellowRed); }
			set {
				if(Format == value) return;
				SetEnumProperty("Format", GridConditionColorScaleFormat.GreenYellowRed, value);
				Changed();
			}
		}
		protected override string ConditionName { get { return "Color scale condition"; } }
		protected internal override AppearanceStyle GetItemStyle(WebDataProxy dataProxy, int visibleIndex) {
			return null;
		}
		protected internal override AppearanceStyle GetItemCellStyle(WebDataProxy dataProxy, int visibleIndex) {
			return new AppearanceStyle { BackColor = GetColor(dataProxy, visibleIndex) };
		}
		protected internal override IFormatConditionRuleBase GetExportRule() {
			if(MiddleColor == Color.Empty)
				return new GridExportConditionRule2ColorScale(this);
			return new GridExportConditionRule3ColorScale(this);
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var condition = source as GridFormatConditionColorScale;
			if(condition != null) {
				MinimumColor = condition.MinimumColor;
				MiddleColor = condition.MiddleColor;
				MaximumColor = condition.MaximumColor;
				Format = condition.Format;
			}
		}
		protected internal Color GetColor(WebDataProxy dataProxy, int visibleIndex) {
			decimal? minValue = GetMinimumValue(dataProxy);
			decimal? maxValue = GetMaximumValue(dataProxy);
			decimal? value = DataUtils.ConvertToDecimalValue(dataProxy.GetRowValue(visibleIndex, FieldName));
			if(minValue == null || maxValue == null || value == null)
				return Color.Empty;
			Color lowColor = GetMinimumColor();
			Color highColor = GetMaximumColor();
			Color middleColor = GetMiddleColor();
			if(middleColor != Color.Empty) {
				decimal average = (minValue.Value + maxValue.Value) / 2;
				if(value.Value < average) {
					highColor = middleColor;
					maxValue = average;
				} else {
					lowColor = middleColor;
					minValue = average;
				}
			}
			return GetColorByRange(value.Value, minValue.Value, lowColor, maxValue.Value, highColor);
		}
		static Color GetColorByRange(decimal value, decimal minValue, Color lowColor, decimal maxValue, Color highColor) {
			decimal scale = minValue != maxValue ? (value - minValue) / (maxValue - minValue) : 1;
			return Color.FromArgb(
				GetScaledColorChanelValue(scale, lowColor.A, highColor.A),
				GetScaledColorChanelValue(scale, lowColor.R, highColor.R),
				GetScaledColorChanelValue(scale, lowColor.G, highColor.G),
				GetScaledColorChanelValue(scale, lowColor.B, highColor.B)
			);
		}
		static byte GetScaledColorChanelValue(decimal scale, byte lowChanel, byte highChanel) {
			return (byte)Math.Round((lowChanel + (highChanel - lowChanel) * scale));
		}
		protected internal Color GetMaximumColor() {
			if(Format == GridConditionColorScaleFormat.Custom)
				return MaximumColor;
			return PredefinedFormats[Format][0];
		}
		protected internal Color GetMiddleColor() {
			if(Format == GridConditionColorScaleFormat.Custom)
				return MiddleColor;
			var colors = PredefinedFormats[Format];
			return colors.Count() == 3 ? colors[1] : Color.Empty;
		}
		protected internal Color GetMinimumColor() {
			if(Format == GridConditionColorScaleFormat.Custom)
				return MinimumColor;
			var colors = PredefinedFormats[Format];
			return colors.Count() == 3 ? colors[2] : colors[1];
		}
	}
	public enum GridConditionIconSetFormat {
		Arrows3Colored, Arrows3Gray, Triangles3, Arrows4Colored, Arrows4Gray, Arrows5Colored, Arrows5Gray,
		TrafficLights3Unrimmed, TrafficLights3Rimmed, Signs3, TrafficLights4, RedToBlack4,
		Symbols3Circled, Symbols3Uncircled, Flags3,
		Stars3, Ratings4, Ratings5, Quarters5, Boxes5,
		PositiveNegativeArrowsColored, PositiveNegativeArrowsGray, PositiveNegativeTriangles
	}
	public abstract class GridFormatConditionIconSet : GridFormatConditionIndicatorBase {
		static IDictionary<GridConditionIconSetFormat, GridFormatConditionIconCollection> PredefinedFormats = new Dictionary<GridConditionIconSetFormat, GridFormatConditionIconCollection> {
			{ GridConditionIconSetFormat.Arrows3Colored, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Arrows3, 3)  },
			{ GridConditionIconSetFormat.Arrows3Gray, new GridFormatConditionIconCollection(XlCondFmtIconSetType.ArrowsGray3, 3)  },
			{ GridConditionIconSetFormat.Triangles3, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Triangles3, 3)  },
			{ GridConditionIconSetFormat.Arrows4Colored, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Arrows4, 4)  },
			{ GridConditionIconSetFormat.Arrows4Gray, new GridFormatConditionIconCollection(XlCondFmtIconSetType.ArrowsGray4, 4)  },
			{ GridConditionIconSetFormat.Arrows5Colored, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Arrows5, 5)  },
			{ GridConditionIconSetFormat.Arrows5Gray, new GridFormatConditionIconCollection(XlCondFmtIconSetType.ArrowsGray5, 5)  },
			{ GridConditionIconSetFormat.TrafficLights3Unrimmed, new GridFormatConditionIconCollection(XlCondFmtIconSetType.TrafficLights3, 3)  },
			{ GridConditionIconSetFormat.TrafficLights3Rimmed, new GridFormatConditionIconCollection(XlCondFmtIconSetType.TrafficLights3Black, 3)  },
			{ GridConditionIconSetFormat.Signs3, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Signs3, 3)  },
			{ GridConditionIconSetFormat.TrafficLights4, new GridFormatConditionIconCollection(XlCondFmtIconSetType.TrafficLights4, 4)  },
			{ GridConditionIconSetFormat.RedToBlack4, new GridFormatConditionIconCollection(XlCondFmtIconSetType.RedToBlack4, 4)  },
			{ GridConditionIconSetFormat.Symbols3Circled, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Symbols3Circled, 3)  },
			{ GridConditionIconSetFormat.Symbols3Uncircled, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Symbols3, 3)  },
			{ GridConditionIconSetFormat.Flags3, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Flags3, 3)  },
			{ GridConditionIconSetFormat.Stars3, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Stars3, 3)  },
			{ GridConditionIconSetFormat.Ratings4, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Rating4, 4)  },
			{ GridConditionIconSetFormat.Ratings5, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Rating5, 5)  },
			{ GridConditionIconSetFormat.Quarters5, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Quarters5, 5)  },
			{ GridConditionIconSetFormat.Boxes5, new GridFormatConditionIconCollection(XlCondFmtIconSetType.Boxes5, 5)  },
			{ GridConditionIconSetFormat.PositiveNegativeTriangles, new GridFormatConditionPositiveNegativeIcons(XlCondFmtIconSetType.Triangles3)  },
			{ GridConditionIconSetFormat.PositiveNegativeArrowsGray, new GridFormatConditionPositiveNegativeIcons(XlCondFmtIconSetType.ArrowsGray3)  },
			{ GridConditionIconSetFormat.PositiveNegativeArrowsColored, new GridFormatConditionPositiveNegativeIcons(XlCondFmtIconSetType.Arrows3)  }
		};
		protected internal GridConditionIconSetFormat Format {
			get { return (GridConditionIconSetFormat)GetEnumProperty("Format", GridConditionIconSetFormat.Arrows3Colored); }
			set { 
				if(Format == value) return;
				SetEnumProperty("Format", GridConditionIconSetFormat.Arrows3Colored, value);
				Changed();
			}
		}
		protected override string ConditionName { get { return "Icon set condition"; } }
		protected internal GridFormatConditionIconCollection Icons { get { return PredefinedFormats[Format]; } }
		protected List<GridFormatConditionIcon> GetSortedIcons() {
			var sortedIcons = Icons.ToList();
			sortedIcons.Sort((a, b) => {
				int cmp = decimal.Compare(a.Threshold, b.Threshold);
				if(cmp != 0)
					return cmp * -1;
				if(a.ComparisonType == b.ComparisonType)
					return 0;
				if(a.ComparisonType == GridFormatConditionComparisonType.Greater)
					return -1;
				return 1;
			});
			return sortedIcons;
		}
		protected internal override AppearanceStyle GetItemStyle(WebDataProxy dataProxy, int visibleIndex) {
			return null;
		}
		protected internal override AppearanceStyle GetItemCellStyle(WebDataProxy dataProxy, int visibleIndex) {
			AppearanceStyle style = new AppearanceStyle();
			decimal state = GetIconState(dataProxy, visibleIndex);
			if(state > 0)
				style.CssClass = GridConditionFormatingIconsHelper.GetIconCssClassName(Icons.FormatName, state);
			return style;
		}
		protected internal decimal GetIconState(WebDataProxy dataProxy, int visibleIndex) {
			decimal? minValue = GetMinimumValue(dataProxy);
			decimal? maxValue = GetMaximumValue(dataProxy);
			decimal? value = DataUtils.ConvertToDecimalValue(dataProxy.GetRowValue(visibleIndex, FieldName));
			if(minValue == null || maxValue == null || value == null)
				return -1;
			if(Icons.ThresholdType == GridFormatConditionThresholdType.Percent) {
				value = minValue != maxValue ? (value.Value - minValue.Value) / (maxValue.Value - minValue.Value) : 1;
				value *= 100;
			}
			List<GridFormatConditionIcon> sortedIcons = GetSortedIcons();
			for(int i = sortedIcons.Count - 1; i >= 0; i--) {
				if(IsValueBelongToIcon(sortedIcons[i], value.Value) && (i == 0 || !IsValueBelongToIcon(sortedIcons[i - 1], value.Value)))
					return sortedIcons[i].Visible ? i + 1 : -1;
			}
			return -1;
		}
		static bool IsValueBelongToIcon(GridFormatConditionIcon element, decimal value) {
			return element.ComparisonType == GridFormatConditionComparisonType.GreaterOrEqual ? value >= element.Threshold : value > element.Threshold;
		}
		protected internal override IFormatConditionRuleBase GetExportRule() {
			return new GridExportConditionRuleIconSet(this);
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var condition = source as GridFormatConditionIconSet;
			if(condition != null)
				Format = condition.Format;
		}
	}
	public abstract class GridFormatConditionIndicatorBase : GridFormatConditionBase {
		protected internal object MinimumValue {
			get { return GetObjectProperty("MinimumValue", null); }
			set {
				if(MinimumValue == value) return;
				SetObjectProperty("MinimumValue", null, value);
				Changed();
			}
		}
		protected internal object MaximumValue {
			get { return GetObjectProperty("MaximumValue", null); }
			set {
				if(MaximumValue == value) return;
				SetObjectProperty("MaximumValue", null, value);
				Changed();
			}
		}
		protected decimal? GetMinimumValue(WebDataProxy dataProxy) {
			object minValue = IsNullValue(MinimumValue) && MinSummary != null ? dataProxy.GetFormatConditionSummaryValue(MinSummary) : MinimumValue;
			return DataUtils.ConvertToDecimalValue(minValue);
		}
		protected decimal? GetMaximumValue(WebDataProxy dataProxy) {
			var maxValue = IsNullValue(MaximumValue) && MaxSummary != null ? dataProxy.GetFormatConditionSummaryValue(MaxSummary) : MaximumValue;
			return DataUtils.ConvertToDecimalValue(maxValue);
		}
		protected FormatConditionSummary MinSummary {
			get { return ColumnSummaries != null ? ColumnSummaries.FirstOrDefault(s => s.SummaryType == SummaryItemType.Min) : null; }
		}
		protected FormatConditionSummary MaxSummary {
			get { return ColumnSummaries != null ? ColumnSummaries.FirstOrDefault(s => s.SummaryType == SummaryItemType.Max) : null; }
		}
		protected internal override IEnumerable<FormatConditionSummary> CreateSummaries() {
			List<FormatConditionSummary> summaries = new List<FormatConditionSummary>();
			if(MaximumValue == null)
				summaries.Add(new FormatConditionSummary { FieldName = FieldName, SummaryType = SummaryItemType.Max });
			if(MinimumValue == null)
				summaries.Add(new FormatConditionSummary { FieldName = FieldName, SummaryType = SummaryItemType.Min });
			return summaries;
		}
		protected internal override bool IsFitToRule(WebDataProxy dataProxy, int visibleIndex) {
			object value = dataProxy.GetRowValue(visibleIndex, FieldName);
			return CompareValues(GetMinimumValue(dataProxy), value) <= 0 && CompareValues(value, GetMaximumValue(dataProxy)) <= 0;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var condition = source as GridFormatConditionIndicatorBase;
			if(condition != null) {
				MinimumValue = condition.MinimumValue;
				MaximumValue = condition.MaximumValue;
			}
		}
	}
}
namespace DevExpress.Web.Internal {
	public class FormatConditionSummary : ASPxSummaryItemBase { }
	public enum GridFormatConditionThresholdType { Number, Percent }
	public enum GridFormatConditionComparisonType { Greater, GreaterOrEqual }
	public class GridFormatConditionIcon {
		public GridFormatConditionIcon() {
			Visible = true;
			ComparisonType = GridFormatConditionComparisonType.GreaterOrEqual;
		}
		public decimal Threshold { get; set; }
		public GridFormatConditionComparisonType ComparisonType { get; set; }
		public bool Visible { get; set; }
	}
	public sealed class GridFormatConditionPositiveNegativeIcons : GridFormatConditionIconCollection {
		public GridFormatConditionPositiveNegativeIcons(XlCondFmtIconSetType iconSet)
			: base(iconSet, -1) {
		}
		public override int StateCount { get { return Count; } }
		public override GridFormatConditionThresholdType ThresholdType { get { return GridFormatConditionThresholdType.Number; } }
		protected override void PopulateElements() {
			AddRange(new GridFormatConditionIcon[]{
				new GridFormatConditionIcon { Threshold = Decimal.MinValue },
				new GridFormatConditionIcon { Threshold = 0, Visible = false },
				new GridFormatConditionIcon { Threshold = 0, ComparisonType = GridFormatConditionComparisonType.Greater }
			});
		}
	}
	public class GridFormatConditionIconCollection : List<GridFormatConditionIcon> {
		public GridFormatConditionIconCollection(XlCondFmtIconSetType iconSet, int stateCount) {
			StateCount = stateCount;
			IconSet = iconSet;
			PopulateElements();
		}
		public string FormatName { get { return IconSet.ToString(); } }
		public virtual int StateCount { get; private set; }
		public XlCondFmtIconSetType IconSet { get; private set; }
		public virtual GridFormatConditionThresholdType ThresholdType {
			get { return GridFormatConditionThresholdType.Percent; }
		}
		protected virtual void PopulateElements() {
			for(int i = 0; i < StateCount; i++) {
				decimal threshold = Math.Round(100 * (StateCount - 1 - i) / (decimal)StateCount);
				Add(new GridFormatConditionIcon {
					Threshold = threshold, ComparisonType = GridFormatConditionComparisonType.GreaterOrEqual
				});
			}
		}
	}
	public class GridConditionFormatingIconsHelper {
		protected internal const string IconSetSpriteName = "FCISprite";
		const string
			IconSetSpriteResourceName = ASPxWebControl.WebCssResourcePath + IconSetSpriteName + ".css",
			FormatConditionRuleClassName = "dxFCRule";
		public static string GetIconCssClassName(string formatName, decimal state) {
			string iconClassName = string.Format("dxWeb_fcIcons_{0}_{1}", formatName, state);
			return FormatConditionRuleClassName + " " + iconClassName;
		}
		public static void RegisterSpriteCssFile(Page page, GridFormatConditionCollection formatConditions) {
			if(RequireRegisterIconsSprite(formatConditions))
				ResourceManager.RegisterCssResource(page, typeof(ASPxWebControl), IconSetSpriteResourceName);
		}
		static bool RequireRegisterIconsSprite(GridFormatConditionCollection formatConditions) {
			if(MvcUtils.RenderMode != MvcRenderMode.None)
				return true;
			return formatConditions != null && formatConditions.OfType<GridFormatConditionIconSet>().Any();
		}
	}
	public class GridExpressionFormatConditionCache {
		int visibleItemCountOnPage = -1;
		public bool IsReady { get { return VisibleItemCountOnPage > -1; } }
		public int VisibleItemCountOnPage {
			get { return visibleItemCountOnPage; }
			set {
				visibleItemCountOnPage = value;
				if(visibleItemCountOnPage > -1)
					FitExpression = new bool?[visibleItemCountOnPage];
			}
		}
		protected bool?[] FitExpression { get; private set; }
		public bool IsConditionCache(int index) {
			return IsValidIndex(index) && FitExpression[index].HasValue;
		}
		public void CacheRuleResult(int index, bool isFit) {
			if(!IsValidIndex(index)) return;
			FitExpression[index] = isFit;
		}
		public bool IsFitToRule(int index) {
			return FitExpression[index].Value;
		}
		public void Reset() {
			FitExpression = null;
			VisibleItemCountOnPage = -1;
		}
		protected bool IsValidIndex(int index) {
			return index >= 0 && index < VisibleItemCountOnPage;
		}
		public void Serialize(TypedBinaryWriter writer) {
			writer.WriteObject(VisibleItemCountOnPage);
			for(int i = 0; i < VisibleItemCountOnPage; i++) {
				bool value = FitExpression[i].HasValue ? FitExpression[i].Value : false;
				writer.Write(value);
			}
		}
		public void Deserialize(TypedBinaryReader reader) {
			VisibleItemCountOnPage = reader.ReadObject<int>();
			for(int i = 0; i < VisibleItemCountOnPage; i++)
				FitExpression[i] = reader.ReadBoolean();
		}
	}
}
