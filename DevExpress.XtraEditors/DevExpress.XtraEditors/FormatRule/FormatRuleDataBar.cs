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
using DevExpress.Utils.Drawing;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
namespace DevExpress.XtraEditors {
	public class FormatConditionRuleDataBar : FormatConditionRuleMinMaxBase, IAppearanceOwner, IFormatRuleDraw, IFormatConditionRuleDataBar {
		AppearanceObjectEx appearance, appearanceNegative;
		static AppearanceDefault defaultColors = new AppearanceDefault() { BackColor = Color.LightBlue, BorderColor = Color.Blue, ForeColor = Color.Black };
		static AppearanceDefault defaultColorsNegative = new AppearanceDefault() { BackColor = Color.FromArgb(255,198,198), BorderColor = Color.Red, ForeColor = Color.Black };
		bool showBarOnly = false, drawAxis = true, allowNegativeAxis = true, drawAxisAtMiddle = false;
		DefaultBoolean rightToLeft = DefaultBoolean.Default;
		static Color defaultAxisColor = Color.Black;
		Color axisColor = Color.Empty;
		string predefinedName;
		public FormatConditionRuleDataBar() {
			this.appearance = new AppearanceObjectEx(this);
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.appearanceNegative = new AppearanceObjectEx(this);
			this.appearanceNegative.Changed += new EventHandler(OnAppearanceChanged);
		}
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRuleDataBar();
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleDataBar;
			if(source == null) return;
			Appearance.Assign(source.Appearance);
			AppearanceNegative.Assign(source.AppearanceNegative);
			AxisColor = source.AxisColor;
			ShowBarOnly = source.ShowBarOnly;
			DrawAxisAtMiddle = source.DrawAxisAtMiddle;
			DrawAxis = source.DrawAxis;
			RightToLeft = source.RightToLeft;
			AllowNegativeAxis = source.AllowNegativeAxis;
			PredefinedName = source.PredefinedName;
		}
		bool ShouldSerializeAxisColor() { return AxisColor != Color.Empty; }
		void ResetAxisColor() { AxisColor = Color.Empty; }
		[XtraSerializableProperty]
		public Color AxisColor {
			get { return axisColor; }
			set {
				if(AxisColor == value) return;
				axisColor = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		[DefaultValue(false), XtraSerializableProperty]
		public bool ShowBarOnly {
			get { return showBarOnly; }
			set {
				if(ShowBarOnly == value) return;
				showBarOnly = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		[DefaultValue(true), XtraSerializableProperty]
		public bool DrawAxis {
			get { return drawAxis; }
			set {
				if(DrawAxis == value) return;
				drawAxis = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		[DefaultValue(true), XtraSerializableProperty]
		public bool AllowNegativeAxis {
			get { return allowNegativeAxis; }
			set {
				if(AllowNegativeAxis == value) return;
				allowNegativeAxis = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		[DefaultValue(false), XtraSerializableProperty]
		public bool DrawAxisAtMiddle {
			get { return drawAxisAtMiddle; }
			set {
				if(DrawAxisAtMiddle == value) return;
				drawAxisAtMiddle = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty]
		public DefaultBoolean RightToLeft {
			get { return rightToLeft; }
			set {
				if(RightToLeft == value) return;
				rightToLeft = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnModified(FormatConditionRuleChangeType.UI);
		}
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("FormatConditionRuleDataBarAppearance"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.FormatConditionRuleDataBar.Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public AppearanceObjectEx Appearance { get { return appearance; } }
		bool ShouldSerializeAppearanceNegative() { return AppearanceNegative.ShouldSerialize(); }
		void ResetAppearanceNegative() { AppearanceNegative.Reset(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("FormatConditionRuleDataBarAppearanceNegative"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.FormatConditionRuleDataBar.AppearanceNegative"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public AppearanceObjectEx AppearanceNegative { get { return appearanceNegative; } }
		[DefaultValue("")]
		[XtraSerializableProperty]
		[RefreshProperties(System.ComponentModel.RefreshProperties.Repaint)]
		[Editor(typeof(DevExpress.XtraEditors.Design.FormatPredefinedDataBarSchemesUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string PredefinedName { 
			get { return predefinedName; }
			set {
				if(PredefinedName == value) return;
				if(value == null) value = "";
				predefinedName = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		protected virtual void OnAssignPredefinedDataBar(FormatPredefinedDataBarScheme db) {
			AppearanceNegative.Reset();
			Appearance.Reset();
			if(db.Negative != null) AppearanceNegative.Assign(db.Negative);
			if(db.Positive != null) Appearance.Assign(db.Positive);
			AxisColor = db.AxisColor;
		}
		protected override bool IsAllowOutOfBoundsValue() {
			return true;
		}
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading { get { return false; } } 
		#endregion
		#region IFormatRuleDraw Members
		void IFormatRuleDraw.DrawOverlay(FormatRuleDrawArgs e) {
			decimal min, max;
			if(!GetMinMaxRange(e.ValueProvider, out min, out max)) return;
			if(min > max) return;
			decimal? value = CheckQueryNumericValue(e.ValueProvider);
			if(value == null) return;
			decimal actualValue = value.Value;
			if(GetRightToLeft()) {
				min = min * -1;
				max = max * -1;
				var tempMax = max;
				max = min;
				min = tempMax;
				actualValue = actualValue * -1;
			}
			DrawCore(e, min, max, actualValue);
		}
		int AxisPadding { get { return 2; } }
		bool GetRightToLeft() {
			if(RightToLeft == DefaultBoolean.True) return true;
			if(RightToLeft == DefaultBoolean.False) return false;
			return GetIsRightToLeft();
		}
		void DrawCore(FormatRuleDrawArgs e, decimal min, decimal max, decimal actualValue) {
			decimal percent = FindPercentValue(min, max, actualValue);
			if(percent < 0) return;
			Rectangle bounds = Rectangle.Inflate(e.Bounds, -2, -2);
			Rectangle boundsNegative = Rectangle.Empty, boundsPositive = Rectangle.Empty;
			int availableWidth = bounds.Width;
			bool allPositive = true, allNegative = false;
			if(AllowNegativeAxis) {
				if(min < 0 || max < 0) { 
					allPositive = false;
					if(min < 0 && max <= 0) { 
						allNegative = true;
						boundsNegative = bounds;
						boundsNegative.Width = (int)((decimal)boundsNegative.Width * ((100 - percent) / 100));
						boundsNegative.X += bounds.Width - boundsNegative.Width;
					}
					else {
						decimal zero = FindPercentValue(min, max, 0);
						if(DrawAxisAtMiddle) zero = 50;
						availableWidth = (int)(bounds.Width * (zero / 100));
						if(!allPositive && DrawAxis) availableWidth -= AxisPadding + 1;
						if(actualValue < 0) {
							boundsNegative = bounds;
							boundsNegative.Width = (int)(availableWidth * ((100 - FindPercentValue(min, 0, actualValue)) / 100));
							boundsNegative.X += availableWidth - boundsNegative.Width;
						}
						availableWidth = bounds.Width - (availableWidth + ((!allPositive && DrawAxis) ? AxisPadding * 2 + 1 : 0));
					}
				}
			}
			if(!allPositive && !allNegative && AllowNegativeAxis) {
				if(DrawAxis) {
					using(Pen pen = new Pen(GetAxisColor())) {
						pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
						int dashX = bounds.Right - (availableWidth + AxisPadding + 1);
						e.Cache.Graphics.DrawLine(pen, new Point(dashX, bounds.Y), new Point(dashX, bounds.Bottom));
					}
				}
			}
			if(min > 0 || max > 0 || allPositive) {
				if(allPositive && min > 0 && max > 0) {
					boundsPositive = bounds;
					boundsPositive.Width = (int)((decimal)boundsPositive.Width * (percent / 100));
				}
				else {
					if(actualValue >= 0) {
						boundsPositive = bounds;
						boundsPositive.X += (bounds.Width - availableWidth);
						boundsPositive.Width = (int)(availableWidth * (FindPercentValue(0, max, actualValue)) / 100);
					}
				}
			}
			DrawBar(e, boundsNegative, boundsPositive);
		}
		void DrawBar(FormatRuleDrawArgs e, Rectangle boundsNegative, Rectangle boundsPositive) {
			if(GetRightToLeft()) {
				if(!boundsNegative.IsEmpty) DrawCore(e, boundsNegative, Appearance, GetDefaultColors());
				if(!boundsPositive.IsEmpty) DrawCore(e, boundsPositive, AppearanceNegative, GetDefaultColorsNegative());
			}
			else {
				if(!boundsNegative.IsEmpty) DrawCore(e, boundsNegative, AppearanceNegative, GetDefaultColorsNegative());
				if(!boundsPositive.IsEmpty) DrawCore(e, boundsPositive, Appearance, GetDefaultColors());
			}
		}
		protected virtual AppearanceDefault GetDefaultColors() {
			var predefined = GetPredefined();
			if(predefined != null) return predefined.Positive;
			return defaultColors;
		}
		protected virtual AppearanceDefault GetDefaultColorsNegative() {
			var predefined = GetPredefined();
			if(predefined != null) return predefined.Negative;
			return defaultColorsNegative;
		}
		protected virtual Color GetAxisColor() { 
			if(AxisColor != Color.Empty) return AxisColor;
			var predefined = GetPredefined();
			if(predefined == null) return defaultAxisColor;
			return predefined.AxisColor;
		}
		protected FormatPredefinedDataBarScheme GetPredefined() {
			if(string.IsNullOrEmpty(PredefinedName)) return null;
			return FormatPredefinedDataBarSchemes.Default.Find(LookAndFeel, PredefinedName);
		}
		void DrawCore(FormatRuleDrawArgs e, Rectangle bounds, AppearanceObjectEx appearance, AppearanceDefault defaultColors) {
			if(bounds.Width < 1 || bounds.Height < 1) return;
			AppearanceObject app = new AppearanceObject();
			AppearanceObject source = appearance;
			AppearanceDefault sourceDefault = defaultColors;
			if(source.GetBackColor() != Color.Empty) sourceDefault = null;
			AppearanceHelper.Combine(app, new AppearanceObject[] { source }, sourceDefault);
			e.Cache.DrawRectangle(e.Cache.GetPen(app.BorderColor), bounds);
			Rectangle fillBounds = Rectangle.Inflate(bounds, -1, -1);
			app.FillRectangle(e.Cache, fillBounds);
			if(!ShowBarOnly && e.OriginalContentAppearance != null && e.OriginalContentPainter != null && app.GetForeColor() != Color.Empty) {
				var original = e.OriginalContentAppearance;
				AppearanceObject appNew = original.Clone() as AppearanceObject;
				appNew.ForeColor = app.GetForeColor();
				var state = e.Cache.ClipInfo.SaveAndSetClip(bounds);
				e.OriginalContentPainter(e.Cache, appNew);
				e.Cache.ClipInfo.RestoreClipRelease(state);
				e.Cache.ClipInfo.ExcludeClip(bounds);
			}
		}
		bool IFormatRuleDraw.AllowDrawValue { get { return !ShowBarOnly; } }
		protected override void DrawPreviewCore(GraphicsCache cache, FormatConditionDrawPreviewArgs e) {
			Rectangle bounds = e.Bounds;
			bounds.Width = bounds.Width / 2;
			FormatRuleDrawArgs de = new FormatRuleDrawArgs(cache, Rectangle.Inflate(bounds, -2, 0), new FormatConditionEmptyValueProvider());
			e.Appearance.DrawString(cache, e.Text, Rectangle.Inflate(de.Bounds, -4, 0));
			de.OriginalContentAppearance = e.Appearance;
			de.OriginalContentPainter = (c, appearance) => {
				appearance.DrawString(c, e.Text, Rectangle.Inflate(de.Bounds, -4, 0));
			};
			DrawCore(de, -100, 1, -90);
			bounds.Width += 4;
			bounds.X += bounds.Width - 8;
			de.SetBounds(Rectangle.Inflate(bounds, 0, 0));
			e.Appearance.DrawString(cache, e.Text, Rectangle.Inflate(de.Bounds, -4, 0));
			DrawCore(de, 0, 100, 50);
		}
		#endregion
		#region IFormatConditionRuleDataBar
		bool IFormatConditionRuleDataBar.AllowNegativeAxis {
			get { return AllowNegativeAxis; }
		}
		Color IFormatConditionRuleDataBar.AxisColor {
			get { return GetAxisColor(); }
		}
		Color IFormatConditionRuleDataBar.BorderColor {
			get {
				if(!Appearance.BorderColor.IsEmpty) return Appearance.BorderColor;
				else return GetDefaultColors().BorderColor;
			}
		} 
		int IFormatConditionRuleDataBar.Direction {
			get { return (int)RightToLeft; }
		}	   
		bool IFormatConditionRuleDataBar.DrawAxis {
			get { return DrawAxis; }
		}
		bool IFormatConditionRuleDataBar.DrawAxisAtMiddle {
			get { return DrawAxisAtMiddle; }
		}
		Color IFormatConditionRuleDataBar.FillColor {
			get {
				if(!Appearance.BackColor.IsEmpty) return Appearance.BackColor;
				return GetDefaultColors().BackColor;
			}
		}
		bool IFormatConditionRuleDataBar.GradientFill {
			get {
				if(string.IsNullOrEmpty(PredefinedName)) return false;
				return PredefinedName.EndsWith("Gradient");
			}
		}	
		Color IFormatConditionRuleDataBar.NegativeBorderColor {
			get {
				if(!AppearanceNegative.BorderColor.IsEmpty) return AppearanceNegative.BorderColor;
				else return GetDefaultColorsNegative().BorderColor;
			}
		}
		Color IFormatConditionRuleDataBar.NegativeFillColor {
			get {
				if(!AppearanceNegative.BackColor.IsEmpty) return AppearanceNegative.BackColor;
				else return GetDefaultColorsNegative().BackColor2;
			}
		}
		string IFormatConditionRuleDataBar.PredefinedName {
			get { return PredefinedName; }
		}
		bool IFormatConditionRuleDataBar.ShowBarOnly {
			get { return ShowBarOnly; }
		}
		object IFormatConditionRuleMinMaxBase.MaxValue {
			get { return Maximum; }
		}
		object IFormatConditionRuleMinMaxBase.MinValue {
			get { return Minimum; }
		}
		XlCondFmtValueObjectType IFormatConditionRuleMinMaxBase.MaxType {
			get { return MaximumType == FormatConditionValueType.Number ? XlCondFmtValueObjectType.Number : XlCondFmtValueObjectType.Percent; }
		}
		XlCondFmtValueObjectType IFormatConditionRuleMinMaxBase.MinType {
			get { return MinimumType == FormatConditionValueType.Number ? XlCondFmtValueObjectType.Number : XlCondFmtValueObjectType.Percent; }
		}
		#endregion
	}
}
