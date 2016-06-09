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
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraCharts {
	public abstract class TitleBase : ChartElement, ITextAppearance, ISupportTextAntialiasing {
		static readonly Color DefaultTextColor = Color.Empty;
		Font font;
		Color textColor = DefaultTextColor;
		bool visible;
		DefaultBoolean enableAntialiasing = DefaultBoolean.Default;
		bool ActualAntialiasing { get { return DefaultBooleanUtils.ToBoolean(enableAntialiasing, DefaultAntialiasing); } }
		protected abstract bool DefaultVisible { get; }
		protected abstract Font DefaultFont { get; } 
		protected abstract bool DefaultAntialiasing { get; }
		protected virtual bool Rotated { get { return false; } }
		protected internal virtual Color ActualTextColor { 
			get { 
				if (!textColor.IsEmpty)
					return textColor;
				IChartAppearance appearance = CommonUtils.GetActualAppearance(this);
				return appearance == null ? Color.Empty : GetTextColor(appearance);
			}
		}
		protected internal virtual bool ActualVisibility { get { return Visible; } }
		protected internal virtual ChartElement BackElement { get { return this; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TitleBaseVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TitleBase.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category("Appearance"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return visible; }
			set {
				if(value == visible) return;
				SendNotification(new ElementWillChangeNotification(this));
				visible = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TitleBaseFont"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TitleBase.Font"),
		TypeConverter(typeof(FontTypeConverter)),
		Category("Appearance"),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Font Font {
			get { return font; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectFont));
				if (font != value) {
					SendNotification(new ElementWillChangeNotification(this));
					font = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TitleBaseTextColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TitleBase.TextColor"),
		Category("Appearance"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color TextColor {
			get { return textColor; }
			set {
				if(value == textColor) 
					return;
				SendNotification(new ElementWillChangeNotification(this));
				textColor = value;
				RaiseControlChanged();
			}
		}
		[
		Obsolete("This property is now obsolete. Use the EnableAntialiasing property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty
		]
		public bool Antialiasing {
			get { return ActualAntialiasing; }
			set { EnableAntialiasing = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TitleBaseEnableAntialiasing"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TitleBase.EnableAntialiasing"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public DefaultBoolean EnableAntialiasing {
			get { return enableAntialiasing; }
			set {
				if (enableAntialiasing == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				enableAntialiasing = value;
				RaiseControlChanged();
			}
		}
		protected TitleBase(ChartElement obj) : base(obj) {
			visible = DefaultVisible;
			font = DefaultFont;
		}
		#region ISupportTextAntialiasing implementation
		bool ISupportTextAntialiasing.DefaultAntialiasing { get { return DefaultAntialiasing; } }
		Color ISupportTextAntialiasing.TextBackColor { get { return Color.Empty; } }
		bool ISupportTextAntialiasing.Rotated { get { return Rotated; } }
		RectangleFillStyle ISupportTextAntialiasing.TextBackFillStyle { get { return RectangleFillStyle.Empty; } }
		ChartElement ISupportTextAntialiasing.BackElement { get { return BackElement; } }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "Visible")
				return ShouldSerializeVisible();
			if(propertyName == "Font")
				return ShouldSerializeFont();
			if(propertyName == "TextColor")
				return ShouldSerializeTextColor();
			if(propertyName == "Antialiasing")
				return false;
			if(propertyName == "EnableAntialiasing")
				return ShouldSerializeEnableAntialiasing();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeVisible() {
			return visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
		}
		bool ShouldSerializeFont() {
			return this.font != null && !this.font.Equals(DefaultFont);
		}
		void ResetFont() {
			Font = DefaultFont;
		}
		bool ShouldSerializeTextColor() {
			return this.textColor != DefaultTextColor;
		}
		void ResetTextColor() {
			TextColor = DefaultTextColor;
		}
		bool ShouldSerializeAntialiasing() {
			return false;
		}
		bool ShouldSerializeEnableAntialiasing() {
			return this.enableAntialiasing != DefaultBoolean.Default;
		}
		void ResetEnableAntialiasing() {
			EnableAntialiasing = DefaultBoolean.Default;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeVisible() ||
				ShouldSerializeFont() ||
				ShouldSerializeTextColor() ||
				ShouldSerializeEnableAntialiasing();
		}
		#endregion
		protected virtual Color GetTextColor(IChartAppearance actualAppearance) {
			return Color.Empty;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			TitleBase title = obj as TitleBase;
			if (title != null) {
				visible = title.visible;
				font = title.Font;
				textColor = title.textColor;
				enableAntialiasing = title.enableAntialiasing;
			}
		}
	}
	public abstract class Title : TitleBase {
		string text;
		protected abstract string DefaultText { get; }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TitleText"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Title.Text"),
		Category("Behavior"),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public string Text {
			get { return text; }
			set {
				if(value == text) return;
				SendNotification(new ElementWillChangeNotification(this));
				text = value;
				RaiseControlChanged();
			}
		}
		protected Title(ChartElement obj) : base(obj) {
			text = DefaultText;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "Text")
				return ShouldSerializeText();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		protected bool ShouldSerializeText() {
			return this.text != DefaultText;
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeText();
		}
		#endregion
		internal void SetText(string text) {
			if(this.text == DefaultText)
				this.text = text;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Title title = obj as Title;
			if(title == null)
				return;
			this.text = title.text;
		}
	}
	public abstract class MultilineTitle : Title {
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MultilineTitleLines"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MultilineTitle.Lines"),
		Category("Behavior"),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.XtraCharts.Design.StringCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public string[] Lines { 
			get { return StringUtils.StringToStringArray(Text); } 
			set { Text = StringUtils.StringArrayToString(value); }
		}
		bool ShouldSerializeLines() {
			return ShouldSerializeText();
		}
		void ResetLines() {
			ResetText();
		}
		protected MultilineTitle(ChartElement obj) : base(obj) { }
	}
}
