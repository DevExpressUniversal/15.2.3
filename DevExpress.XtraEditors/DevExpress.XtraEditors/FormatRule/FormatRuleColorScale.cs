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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
namespace DevExpress.XtraEditors {
	public class FormatConditionRule2ColorScale : FormatConditionRuleMinMaxBase, IFormatRuleAppearance, IFormatConditionRule2ColorScale {
		internal static readonly
			Color default2MinimumColor = Color.FromArgb(99, 190, 123),
			default2MaximumColor = Color.FromArgb(248, 105, 107);
		Color minimumColor, maximumColor;
		string predefinedName = "";
		public FormatConditionRule2ColorScale() {
			minimumColor = Color.Empty;
			maximumColor = Color.Empty;
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRule2ColorScale;
			if(source == null) return;
			MaximumColor = source.MaximumColor;
			MinimumColor = source.MinimumColor;
			PredefinedName = source.PredefinedName;
		}
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRule2ColorScale();
		}
		[XtraSerializableProperty]
		[DefaultValue("")]
		[RefreshProperties(System.ComponentModel.RefreshProperties.Repaint)]
		[Editor(typeof(DevExpress.XtraEditors.Design.FormatPredefinedColorScaleUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string PredefinedName { 
			get { return predefinedName; }
			set {
				if(PredefinedName == value) return;
				if(value == null) value = "";
				predefinedName = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		bool ShouldSerializeMinimumColor() { return MinimumColor != Color.Empty; }
		void ResetMinimumColor() { MinimumColor = Color.Empty; }
		[XtraSerializableProperty]
		public Color MinimumColor {
			get { return minimumColor; }
			set {
				if(MinimumColor == value) return;
				minimumColor = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		bool ShouldSerializeMaximumColor() { return MaximumColor != Color.Empty; }
		void ResetMaximumColor() { MaximumColor = Color.Empty; }
		[XtraSerializableProperty]
		public Color MaximumColor {
			get { return maximumColor; }
			set {
				if(MaximumColor == value) return;
				maximumColor = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		protected void ResetPredefinedName() {
			this.predefinedName = string.Empty;
		}
		protected virtual Color GetMaximumColor() {
			if(MaximumColor != Color.Empty) return MaximumColor;
			var p = GetPredefinedScale();
			return p == null ? DefaultMaximumColor : p.MaximumColor;
		}
		protected virtual Color GetMinimumColor() {
			if(MinimumColor != Color.Empty) return MinimumColor;
			var p = GetPredefinedScale();
			return p == null ? DefaultMinimumColor : p.MinimumColor;
		}
		protected virtual Color DefaultMinimumColor { get { return default2MinimumColor; } }
		protected virtual Color DefaultMaximumColor { get { return default2MaximumColor; } }
		protected virtual FormatPredefinedColorScale GetPredefinedScale() {
			if(string.IsNullOrEmpty(PredefinedName)) return null;
			return FormatPredefinedColorScales.Default.Find(Owner == null ? DevExpress.LookAndFeel.UserLookAndFeel.Default : Owner.LookAndFeel, PredefinedName);
		}
		#region IFormatRuleAppearance Members
		AppearanceObjectEx IFormatRuleAppearance.QueryAppearance(FormatRuleAppearanceArgs e) {
			return QueryApperanceCore(e);
		}
		protected virtual AppearanceObjectEx QueryApperanceCore(FormatRuleAppearanceArgs e) {
			var res = new AppearanceObjectEx();
			var state = e.ValueProvider.GetState(this);
			decimal? value = ConvertToNumeric(e.CellValue);
			decimal min, max;
			if(value == null || !GetMinMaxRange(e.ValueProvider, out min, out max)) return res;
			res.BackColor = GetColor(GetMinimumColor(), Color.Empty, GetMaximumColor(), min, 0, max, value.Value);
			return res;
		}
		protected override FormatConditionRuleState GetQueryKindStateCore() {
			FormatConditionRuleState res = base.GetQueryKindStateCore();
			res.SetValue(MinimumColorValue, GetMinimumColor());
			res.SetValue(MaximumColorValue, GetMaximumColor());
			return res;
		}
		internal const string MinimumColorValue = "MinimumColor", MaximumColorValue = "MaximumColorValue", MiddleColorValue = "MiddleColorValue";
		#endregion
		#region color helpers
		protected Color GetColor(Color minColor, Color middleColor, Color maxColor, decimal min, decimal mid, decimal max, decimal value) {
			if(middleColor != Color.Empty) {
				if(value < mid) {
					maxColor = middleColor;
					max = mid;
				}
				else {
					minColor = middleColor;
					min = mid;
				}
			}
			return CalcColorCore(minColor, maxColor, min, max, value);
		}
		static byte GetScaleValue(decimal ratio, decimal low, decimal high) {
			return (byte)Math.Round((low + (high - low) * ratio));
		}
		static Color CalcColorCore(Color colorLow, Color colorHigh, decimal min, decimal max, decimal value) {
			decimal ratio = GetRatio(min, max, value);
			var color = Color.FromArgb(
				GetScaleValue(ratio, colorLow.A, colorHigh.A),
				GetScaleValue(ratio, colorLow.R, colorHigh.R),
				GetScaleValue(ratio, colorLow.G, colorHigh.G),
				GetScaleValue(ratio, colorLow.B, colorHigh.B)
			);
			return color;
		}
		internal static decimal GetRatio(decimal min, decimal max, decimal value) {
			return min == max ? 1 : (value - min) / (max - min);
		}
		#endregion
		protected override void DrawPreviewCore(Utils.Drawing.GraphicsCache cache, FormatConditionDrawPreviewArgs e) {
			Color min = GetMinimumColor(), max = GetMaximumColor();
			for(int n = 0; n < e.Bounds.Width; n++) {
				Color c = GetColor(min, GetPreviewMiddleColor(), max, 0, e.Bounds.Width / 2, e.Bounds.Width, n);
				using(var sb = new SolidBrush(c)) {
					e.Graphics.FillRectangle(sb, new Rectangle(n + e.Bounds.Left, e.Bounds.Top, 1, e.Bounds.Height));
				}
			}
		}
		protected virtual Color GetPreviewMiddleColor() { return Color.Empty; }
		#region IFormatConditionRule2ColorScale
		Color IFormatConditionRuleColorScaleBase.MaxColor {
			get { return GetMaximumColor(); }
		}
		Color IFormatConditionRuleColorScaleBase.MinColor {
			get { return GetMinimumColor(); }
		}
		XlCondFmtValueObjectType IFormatConditionRuleMinMaxBase.MaxType {
			get { return MaximumType == FormatConditionValueType.Number ? XlCondFmtValueObjectType.Number : XlCondFmtValueObjectType.Percent; }
		}
		object IFormatConditionRuleMinMaxBase.MaxValue {
			get { return Maximum; }
		}
		XlCondFmtValueObjectType IFormatConditionRuleMinMaxBase.MinType {
			get { return MinimumType == FormatConditionValueType.Number ? XlCondFmtValueObjectType.Number : XlCondFmtValueObjectType.Percent; }
		}
		object IFormatConditionRuleMinMaxBase.MinValue {
			get { return Minimum; }
		}
		#endregion
	}
	public class FormatConditionRule3ColorScale : FormatConditionRule2ColorScale, IFormatConditionRule3ColorScale {
		static readonly Color
			default3MinimumColor = Color.FromArgb(248, 105, 107),
			default3MiddleColor = Color.FromArgb(255, 235, 132),
			default3MaximumColor = Color.FromArgb(99, 190, 123);
		Color middleColor;
		FormatConditionValueType middleType = FormatConditionValueType.Automatic;
		decimal middle;
		public FormatConditionRule3ColorScale() {
			this.middleColor = Color.Empty;
		}
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRule3ColorScale();
		}
		protected bool GetMiddleValue(IFormatConditionRuleValueProvider valueProvider, out decimal middle) {
			middle = 0;
			decimal min, max;
			if(!GetMinMaxRange(valueProvider, out min, out max)) return false;
			decimal? mid = Middle;
			if(MiddleType == FormatConditionValueType.Automatic) {
				mid = GetPercentValue(min, max, 50);
			}
			if(MiddleType == FormatConditionValueType.Percent) {
				mid = GetPercentValue(min, max, mid.Value);
			}
			if(mid == null) return false;
			middle = mid.Value;
			return true;
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRule3ColorScale;
			if(source == null) return;
			MiddleColor = source.MiddleColor;
			Middle = source.Middle;
			MiddleType = source.MiddleType;
		}
		[XtraSerializableProperty, DefaultValue(FormatConditionValueType.Automatic)]
		public FormatConditionValueType MiddleType {
			get { return middleType; }
			set {
				if(MiddleType == value) return;
				middleType = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		void ResetMiddle() { Middle = 0; }
		bool ShouldSerializeMiddle() { return Middle != 0; }
		[XtraSerializableProperty]
		public decimal Middle {
			get { return middle; }
			set {
				if(Middle == value) return;
				middle = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		bool ShouldSerializeMiddleColor() { return MiddleColor != Color.Empty; }
		void ResetMiddleColor() { MiddleColor = Color.Empty; }
		[XtraSerializableProperty]
		public Color MiddleColor {
			get { return middleColor; }
			set {
				if(MiddleColor == value) return;
				middleColor = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		protected override Color DefaultMinimumColor { get { return default3MinimumColor; } }
		protected override Color DefaultMaximumColor { get { return default3MaximumColor; } }
		protected virtual Color GetMiddleColor() {
			if(MiddleColor != Color.Empty) return MiddleColor;
			var p = GetPredefinedScale();
			return p == null ? default3MiddleColor : p.MiddleColor;
		}
		protected override AppearanceObjectEx QueryApperanceCore(FormatRuleAppearanceArgs e) {
			var res = new AppearanceObjectEx();
			decimal? value = ConvertToNumeric(e.CellValue);
			decimal min, max, mid;
			if(value == null || !GetMinMaxRange(e.ValueProvider, out min, out max)) return res;
			var state = e.ValueProvider.GetState(this);
			Color middleColor = GetMiddleColor();
			if(!GetMiddleValue(e.ValueProvider, out mid)) middleColor = Color.Empty;
			res.BackColor = GetColor(GetMinimumColor(), middleColor, GetMaximumColor(), min, mid, max, value.Value);
			return res;
		}
		protected override FormatConditionRuleState GetQueryKindStateCore() {
			FormatConditionRuleState res = base.GetQueryKindStateCore();
			res.SetValue(MinimumColorValue, GetMinimumColor());
			res.SetValue(MaximumColorValue, GetMaximumColor());
			res.SetValue(MiddleColorValue, GetMiddleColor());
			return res;
		}
		protected override Color GetPreviewMiddleColor() { return GetMiddleColor(); }
		#region IFormatConditionRule3ColorScale
		Color IFormatConditionRule3ColorScale.MidpointColor {
			get { return GetMiddleColor(); }
		}
		XlCondFmtValueObjectType IFormatConditionRule3ColorScale.MidpointType {
			get { return MiddleType == FormatConditionValueType.Number ? XlCondFmtValueObjectType.Number : XlCondFmtValueObjectType.Percent; }
		}
		object IFormatConditionRule3ColorScale.MidpointValue {
			get { return Middle; }
		}
		Color IFormatConditionRuleColorScaleBase.MaxColor {
			get { return GetMaximumColor(); }
		}
		Color IFormatConditionRuleColorScaleBase.MinColor {
			get { return GetMinimumColor(); }
		}
		XlCondFmtValueObjectType IFormatConditionRuleMinMaxBase.MaxType {
			get { return MaximumType == FormatConditionValueType.Number ? XlCondFmtValueObjectType.Number : XlCondFmtValueObjectType.Percent; }
		}
		object IFormatConditionRuleMinMaxBase.MaxValue {
			get { return Maximum; }
		}
		XlCondFmtValueObjectType IFormatConditionRuleMinMaxBase.MinType {
			get { return MinimumType == FormatConditionValueType.Number ? XlCondFmtValueObjectType.Number : XlCondFmtValueObjectType.Percent; }
		}
		object IFormatConditionRuleMinMaxBase.MinValue {
			get { return Minimum; }
		}
		#endregion
	}
}
