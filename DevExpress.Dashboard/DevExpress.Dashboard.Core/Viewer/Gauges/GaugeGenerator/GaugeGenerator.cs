#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Presets;
using DevExpress.XtraGauges.Presets.PresetManager;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.DashboardExport;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class GaugeGenerator : IDisposable {
		class IndicatorLabel : LabelComponent {
			IndicatorStringPainter painter;
			public IndicatorLabel(IndicatorStringPainter painter, string name)
				: base(name) {
				this.painter = painter;
			}
			protected override bool RaiseCustomDrawElement(XtraGauges.Core.Primitive.IRenderingContext context) {
				TextShape.Render(context.Graphics, painter);
				return true;
			}
		}
		internal class FlatBackgroundShader : BaseColorShader {
			readonly Color replaced;
			readonly Color source;
			public FlatBackgroundShader(Color replaced, Color source) {
				this.replaced = replaced;
				this.source = source;
			}
			protected override void OnCreate() { }
			protected override void ProcessCore(ref Color sourceColor) {
				if(sourceColor == replaced)
					sourceColor = source;
			}
			protected override BaseObject CloneCore() {
				return new FlatBackgroundShader(replaced, source);
			}
			protected override string GetShaderTypeTag() {
				return "Empty";
			}
			protected override string GetShaderDataTag() {
				return string.Empty;
			}
			protected override void Assign(string shaderData) { }
		}
		const int LabelPadding = 3;
		const string NewLineSeparatorString = "<br>";
		const string SizeFormatString = "<size={0}>{1}</size>";
		const string ColorFormatString = "<color={0}>{1}</color>";
		const string IndicatorImageFormatString = "<image={0};size={2},{2}>{1}";
		const string LabelFontName = "Tahoma";
		const int MainSeriesLabelHeight = 24;
		const int CommonSeriesLabelHeight = 18;
		const int ImageSize = 14;
		static string GetColorString(Color color) {
			return string.Format("{0},{1},{2}", color.R, color.G, color.B);
		}
		public static GaugeGenerator Create(GaugeViewType viewType, GaugeTheme gaugeTheme, DeltaColorsGetter deltaColorsGetter) {
			switch(viewType) {
				case GaugeViewType.CircularFull:
					return new CircularFullGaugeGenerator(viewType, gaugeTheme, deltaColorsGetter);
				case GaugeViewType.CircularHalf:
					return new CircularHalfGaugeGenerator(viewType, gaugeTheme, deltaColorsGetter);
				case GaugeViewType.CircularQuarterLeft:
					return new CircularQuarterGaugeGenerator(viewType, gaugeTheme, deltaColorsGetter);
				case GaugeViewType.CircularQuarterRight:
					return new CircularQuarterGaugeGenerator(viewType, gaugeTheme, deltaColorsGetter);
				case GaugeViewType.CircularThreeFourth:
					return new CircularThreeFourGaugeGenerator(viewType, gaugeTheme, deltaColorsGetter);
				default:
					return new LinearGaugeGenerator(viewType, gaugeTheme, deltaColorsGetter);
			}
		}
		public static string GetPresetNameByGaugeType(GaugeViewType viewType, GaugeTheme theme) {
			string presetPath;
			switch(viewType) {
				case GaugeViewType.CircularHalf:
					presetPath = "{0}CircularHalf.Style{1}{2}";
					break;
				case GaugeViewType.CircularQuarterLeft:
					presetPath = "{0}CircularQuarter.Style{1}.Left{2}";
					break;
				case GaugeViewType.CircularQuarterRight:
					presetPath = "{0}CircularQuarter.Style{1}.Right{2}";
					break;
				case GaugeViewType.CircularThreeFourth:
					presetPath = "{0}CircularThreeFourth.Style{1}{2}";
					break;
				case GaugeViewType.LinearHorizontal:
					presetPath = "{0}LinearHorizontal.Style{1}{2}";
					break;
				case GaugeViewType.LinearVertical:
					presetPath = "{0}LinearVertical.Style{1}{2}";
					break;
				default:
					presetPath = "{0}CircularFull.Style{1}{2}";
					break;
			}
			return string.Format(presetPath, "DevExpress.XtraGauges.Presets.Resources.", (int)theme, ".preset");
		}
		public static GaugeTheme GetLightTheme(GaugeViewType viewType) {
			return GaugeTheme.FlatLight;
		}
		public static GaugeTheme GetDarkTheme(GaugeViewType viewType) {
			return GaugeTheme.FlatDark;
		}
		readonly GaugeViewType viewType;
		readonly GaugeTheme theme;
		readonly DeltaColorsGetter deltaColorsGetter;
		readonly IndicatorStringPainter indicatorPainter;
		readonly Dictionary<IGauge, LabelComponent> SeriesLabels = new Dictionary<IGauge, LabelComponent>();
		readonly Dictionary<IGauge, LabelComponent> ValueLabels = new Dictionary<IGauge, LabelComponent>();
		int seriesLabelHeight;
		int valueLabelHeight;
		float defaultModelFontSize;
		float defaultMajorTickmarkTextOffset;
		bool isDarkBackgroundSkin;
		Color dashboardGaugeForeColor;
		Color dashboardGaugeBackColor;
		Font valueFont;
		Font seriesLabelFont;
		int mainSeriesFontSize = 0;
		int commonSeriesLabelFontSize = 0;
		int MainSeriesFontSize {
			get {
				if(mainSeriesFontSize == 0)
					mainSeriesFontSize = DashboardStringHelper.GetFontSizeByLineHeight(LabelFontName, MainSeriesLabelHeight);
				return mainSeriesFontSize;
			}
		}
		int CommonSeriesLabelFontSize {
			get {
				if(commonSeriesLabelFontSize == 0)
					commonSeriesLabelFontSize = DashboardStringHelper.GetFontSizeByLineHeight(LabelFontName, CommonSeriesLabelHeight);
				return commonSeriesLabelFontSize;
			}
		}
		protected GaugeViewType ViewType { get { return viewType; } }
		protected GaugeTheme Theme { get { return theme; } }
		protected int SeriesLabelHeight { get { return seriesLabelHeight; } private set { seriesLabelHeight = value; } }
		protected int ValueLabelHeight { get { return valueLabelHeight; } private set { valueLabelHeight = value; } }
		protected Color DashboardGaugeForeColor { get { return dashboardGaugeForeColor; } }		
		protected Color DashboardGaugeBackColor { get { return dashboardGaugeBackColor; } }
		public abstract int GaugeMinWidth { get; }
		public DeltaColorsGetter DeltaColorsGetter { get { return deltaColorsGetter; } }
		protected GaugeGenerator(GaugeViewType viewType, GaugeTheme theme, DeltaColorsGetter deltaColorsGetter) {
			this.viewType = viewType;
			this.theme = theme;
			this.deltaColorsGetter = deltaColorsGetter;
			indicatorPainter = new IndicatorStringPainter(deltaColorsGetter);
			valueFont = new Font(LabelFontName, MainSeriesFontSize);
			seriesLabelFont = new Font(LabelFontName, CommonSeriesLabelFontSize);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				indicatorPainter.Dispose();
				if(seriesLabelFont != null) {
					seriesLabelFont.Dispose();
					seriesLabelFont = null;
				}
				if(valueFont != null) {
					valueFont.Dispose();
					valueFont = null;
				}
			}
		}
		void ApplyLabelSettings(BaseGaugeWin baseGauge, string[] caption, bool isTop, Color? color, IndicatorType deltaIndicatorType, bool deltaIsGood) {
			LabelComponent label = null;
			if(isTop)
				SeriesLabels.TryGetValue(baseGauge, out label);
			else
				ValueLabels.TryGetValue(baseGauge, out label);
			Size modelSize = GetGaugeModelSize(false);
			if(caption.Length != 0 && label == null)
				label = CreateLabel(baseGauge, isTop);
			if(caption.Length == 0 && label != null) {
				if(isTop)
					SeriesLabels.Remove(baseGauge);
				else
					ValueLabels.Remove(baseGauge);
				baseGauge.Labels.Remove(label);
			}
			float height = isTop ? SeriesLabelHeight : ValueLabelHeight;
			if(label != null) {
				BrushObject brush = GetLabelBrush(baseGauge, isTop, color);
				label.Text = GetLabelCaption(caption, isTop, deltaIndicatorType, deltaIsGood);
				label.Size = GetLabelSize(isTop);
				label.Position = GetLabelPosition(isTop, modelSize, height);
				label.AppearanceText.TextBrush = brush;
				label.AppearanceText.Format.Alignment = GetLabelHorizontalTextAlign(isTop);
				if(!isTop) {
					label.AppearanceText.Format.Trimming = StringTrimming.EllipsisCharacter;
					label.AppearanceText.Format.FormatFlags = StringFormatFlags.NoWrap;
				}
			}
		}
		LabelComponent CreateLabel(BaseGaugeWin baseGauge, bool isTop) {
			LabelComponent labelComponent;
			if(isTop) {
				labelComponent = baseGauge.AddLabel();
			}
			else {
				string[] names = new string[baseGauge.Labels.Count];
				int i = 0;
				baseGauge.Labels.Accept(
						delegate(LabelComponent l) { names[i++] = l.Name; }
					);
				labelComponent = new IndicatorLabel(indicatorPainter, UniqueNameHelper.GetUniqueName("GaugeLabel", names, baseGauge.Labels.Count + 1));
				BaseGaugeExtension.InitializeLabelDefault(labelComponent);
				baseGauge.Labels.Add(labelComponent);
			}
			ICircularGauge cGauge = baseGauge as ICircularGauge;
			if(cGauge != null) {
				labelComponent.ZOrder = cGauge.Needles[0].ZOrder + 1;
			}
			labelComponent.AllowHTMLString = true;
			if(isTop)
				SeriesLabels.Add(baseGauge, labelComponent);
			else
				ValueLabels.Add(baseGauge, labelComponent);
			labelComponent.AppearanceText.Font = isTop ? seriesLabelFont : valueFont;
			return labelComponent;
		}
		void UpdateScaleFont(IDiscreteScale scale, GaugeModel model) {
			if(ViewType != GaugeViewType.LinearHorizontal && ViewType != GaugeViewType.LinearVertical) {
				string tickMarkFormatString = scale.MajorTickmark.FormatString;
				int len = 0;
				GaugeRangeModel range = model.Range;
				double tick = (range.MaxRangeValue - range.MinRangeValue) / (range.MajorTickCount - 1);
				for(double i = range.MinRangeValue; i <= range.MaxRangeValue; i += tick) {
					len = Math.Max(len, String.Format(tickMarkFormatString, i).Length);
				}
				Font current = scale.MajorTickmark.TextShape.AppearanceText.Font;
				float usableFontSize, usableTextOffset;
				if(len < 4) {
					usableFontSize = defaultModelFontSize;
					usableTextOffset = defaultMajorTickmarkTextOffset + GetSmallScaleTextOffset();
				}
				else {
					usableFontSize = defaultModelFontSize - 1;
					usableTextOffset = defaultMajorTickmarkTextOffset + GetLargeScaleTextOffset();
				}
				if(current.Size != usableFontSize)
					scale.MajorTickmark.TextShape.AppearanceText.Font = new Font(current.FontFamily, usableFontSize);
				scale.MajorTickmark.TextOffset = usableTextOffset;
				if(dashboardGaugeForeColor != Color.Empty)
					scale.MajorTickmark.TextShape.AppearanceText.TextBrush = new SolidBrushObject(dashboardGaugeForeColor);
			}
		}
		BrushObject GetLabelBrush(BaseGaugeWin baseGauge, bool isTop, Color? color) {
			return new SolidBrushObject(color ?? deltaColorsGetter.ActualValueColor);
		}
		protected string GetLabelCaption(string[] caption, bool isTop, IndicatorType deltaIndicatorType, bool deltaIsGood) {
			StringBuilder builder = new StringBuilder();
			if(caption.Length > 0) {
				if(isTop) {
					builder.Append(String.Format(SizeFormatString, MainSeriesFontSize, caption[0]));
					if(caption.Length > 1)
						builder.Append(NewLineSeparatorString).Append(string.Format(ColorFormatString, GetColorString(deltaColorsGetter.Neutral), caption[1]));
				} else if(deltaIndicatorType != IndicatorType.None)
					builder.Append(String.Format(IndicatorImageFormatString, IndicatorStringPainter.IndicatorTypeToString(deltaIndicatorType, deltaIsGood), caption[0], ImageSize));
				else
					builder.Append(caption[0]);
			}
			return builder.ToString();
		}
		protected bool IsLabelsUpdated(IGauge gauge, string topCaption, string bottomCaption) {
			LabelComponent seriesLabel = null;
			SeriesLabels.TryGetValue(gauge, out seriesLabel);
			string gaugeTopCaption = seriesLabel == null ? null : seriesLabel.Text;
			LabelComponent valueLabel = null;
			ValueLabels.TryGetValue(gauge, out valueLabel);
			string gaugeBottomCaption = valueLabel == null ? null : valueLabel.Text;
			if(bottomCaption != gaugeBottomCaption || topCaption != gaugeTopCaption)
				return false;
			return true;
		}
		protected void ApplyRangeSettings<T>(T scale, GaugeRangeModel range) where T : IDiscreteScale, IScale {
			scale.MinValue = Convert.ToSingle(range.MinRangeValue);
			scale.MaxValue = Convert.ToSingle(range.MaxRangeValue);
			scale.MajorTickCount = range.MajorTickCount;
			scale.MinorTickCount = range.MinorTickCount;
			NumericFormatter formatter = NumericFormatter.CreateInstance(Thread.CurrentThread.CurrentCulture, range.MinRangeValue, range.MaxRangeValue);
			scale.MajorTickmark.FormatString = String.Format("{{0:{0}}}", formatter.GetFormatPattern(range.MinRangeValue - range.MinRangeValue));
		}
		protected float CoerceValue(GaugeRangeModel range, float value) {
			float minValue = Convert.ToSingle(Math.Min(range.MinRangeValue, range.MaxRangeValue));
			float maxValue = Convert.ToSingle(Math.Max(range.MinRangeValue, range.MaxRangeValue));
			if(value < minValue)
				value = minValue - GetValueLimit(Math.Abs(maxValue - minValue));
			if(value > maxValue)
				value = maxValue + GetValueLimit(Math.Abs(maxValue - minValue));
			return value;
		}
		protected void KeepOneListItem(IList list) {
			while(list.Count > 1)
				list.RemoveAt(1);
		}
		protected virtual XtraGauges.Core.Base.PointF2D GetLabelPosition(bool isTop, Size modelSize, float height) {
			return new XtraGauges.Core.Base.PointF2D(modelSize.Width / 2, 
				isTop ? 
				-(height - LabelPadding) / 2 - LabelPadding : 
				LabelPadding + modelSize.Height - GetGaugeHeightDiff() + (height - LabelPadding) / 2);
		}
		protected virtual int GetLargeScaleTextOffset() {
			return 0;
		}
		protected virtual int GetSmallScaleTextOffset() {
			return 0;
		}
		protected virtual int GetGaugeHeightDiff() {
			return 0;
		}
		protected string GetLabelCaption(string[] caption, bool isTop) {
			return GetLabelCaption(caption, isTop, IndicatorType.None, false);
		}
		protected virtual StringAlignment GetLabelHorizontalTextAlign(bool isTop) {
			return StringAlignment.Center;
		}
		protected virtual SizeF GetLabelSize(bool isTop) {
			return new SizeF(GetGaugeModelSize(false).Width, (isTop ? SeriesLabelHeight : ValueLabelHeight) - LabelPadding);
		}
		protected virtual float GetDefaultFontSize(IDiscreteScale scale) {
			return scale.MajorTickmark.TextShape.AppearanceText.Font.Size;
		}
		protected abstract IDiscreteScale GetScale(IGauge gauge);
		protected abstract void ApplyGaugeSettingsCore(IGauge gauge, GaugeModel model);
		public abstract Size GetGaugeModelSize(bool includeLabelSize);
		protected abstract float GetValueLimit(float range);
		public virtual int GetTopPadding() {
			return SeriesLabelHeight;
		}
		public bool GetIsCurrent(GaugeViewType viewType, GaugeTheme theme) {
			return this.viewType == viewType && this.theme == theme;
		}
		[SecuritySafeCritical]
		public bool Measure(IEnumerable<GaugeModel> gaugeModels, bool newIsDarkBackgroundSkin, Color newDashboardGaugeForeColor, Color newDashboardGaugeBackColor) {
			int newSeriesLabelHeight = 0;
			int newValueLabelHeight = 0;
			using(Graphics graphics = Graphics.FromHwndInternal(IntPtr.Zero)) {
				using(BaseTextAppearance appearance = new BaseTextAppearance()) {
					Utils_StringPainter sp = new Utils_StringPainter();
					foreach(GaugeModel model in gaugeModels) {
						appearance.Font = seriesLabelFont;
						appearance.Format.Alignment = GetLabelHorizontalTextAlign(true);
						int gaugeModelWidth = Convert.ToInt32(GetLabelSize(true).Width);
						if(model.SeriesLabel.Length > 0) {
							string caption = GetLabelCaption(model.SeriesLabel, true);
							newSeriesLabelHeight = Math.Max(newSeriesLabelHeight, sp.Calculate(new Utils_StringCalculateArgs(graphics, caption, new Rectangle(0, 0, gaugeModelWidth, 0), null) { Appearance = appearance }).Bounds.Height);
							caption = GetLabelCaption(model.ValueLabel, false, model.DeltaIndicatorType, model.DeltaIsGood);
						}
						appearance.Format.Alignment = GetLabelHorizontalTextAlign(false);
						gaugeModelWidth = Convert.ToInt32(GetLabelSize(false).Width);
						appearance.Font = valueFont;
						newValueLabelHeight = Math.Max(newValueLabelHeight, valueFont.Height + 1);
					}
					newSeriesLabelHeight += newSeriesLabelHeight % 2 + LabelPadding;
					newValueLabelHeight += newValueLabelHeight % 2 + LabelPadding;
				}
			}
			bool shouldUpdate = SeriesLabelHeight != newSeriesLabelHeight || ValueLabelHeight != newValueLabelHeight || isDarkBackgroundSkin != newIsDarkBackgroundSkin || dashboardGaugeForeColor != newDashboardGaugeForeColor || dashboardGaugeBackColor != newDashboardGaugeBackColor;
			if(!shouldUpdate)
				return false;
			SeriesLabelHeight = newSeriesLabelHeight;
			ValueLabelHeight = newValueLabelHeight;
			isDarkBackgroundSkin = newIsDarkBackgroundSkin;
			dashboardGaugeForeColor = newDashboardGaugeForeColor;
			dashboardGaugeBackColor = newDashboardGaugeBackColor;
			return true;
		}
		public virtual IGauge CreateGauge() {
			IGaugeContainer tempGauge = new GaugeContainer();
			BaseGaugePreset preset = PresetLoader.LoadFromResources(GetPresetNameByGaugeType(viewType, theme));
			if(preset == null || preset.LayoutInfo == null)
				preset = PresetLoader.LoadFromResources(GetPresetNameByGaugeType(viewType, GaugeGenerator.GetLightTheme(viewType)));
			using(Stream stream = new MemoryStream(preset.LayoutInfo))
				new BinaryXtraSerializer().DeserializeObject(tempGauge, stream, "IGaugeContainer");
			IGauge gauge = tempGauge.Gauges[0];
			tempGauge.Gauges.Remove(gauge);
			IDiscreteScale scale = GetScale(gauge);
			scale.Ranges.Clear();
			defaultModelFontSize = GetDefaultFontSize(scale);
			defaultMajorTickmarkTextOffset = scale.MajorTickmark.TextOffset;
			return gauge;
		}
		public void ApplyGaugeSettings(IGauge gauge, GaugeModel model) {
			ApplyGaugeSettingsCore(gauge, model);
			BaseGaugeWin baseGaugeWin = (BaseGaugeWin)gauge;
			ApplyLabelSettings(baseGaugeWin, model.SeriesLabel, true, null, IndicatorType.None, false);
			ApplyLabelSettings(baseGaugeWin, model.ValueLabel, false, model.ValueColor, model.DeltaIndicatorType, model.DeltaIsGood);
			UpdateScaleFont(GetScale(gauge), model);
		}
		public void Clear() {
			SeriesLabels.Clear();
			ValueLabels.Clear();
		}
	}
}
