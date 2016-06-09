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
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(ExpandableObjectConverter))
	]
	public class CrosshairAxisLabelOptions : ChartElement {
		const DefaultBoolean DefaultVisibility = DefaultBoolean.Default;
		const string DefaultPattern = null;
		static readonly Font DefaultFont = null;
		static readonly Color DefaultBackColor = Color.Empty;
		static readonly Color DefaultTextColor = Color.Empty;
		string pattern = DefaultPattern;
		Color backColor = DefaultBackColor;
		Color textColor = DefaultTextColor;
		Font font;
		DefaultBoolean visibility = DefaultVisibility;
		internal Axis2D Axis {
			get {
				return (Axis2D)Owner;
			}
		}
		[Browsable(false)]
		public bool ActualVisibility {
			get {
				if (Visibility != DefaultBoolean.Default)
					return Visibility == DefaultBoolean.False ? false : true;
				if (Axis.Diagram == null || Axis.Diagram.Chart == null)
					return false;
				CrosshairOptions options = Axis.Diagram.Chart.CrosshairOptions;
				return Axis.IsValuesAxis ? options.ShowValueLabels : options.ShowArgumentLabels;
			}
		}
		[Browsable(false)]
		public Font ActualFont {
			get { return Font != null ? Font : Axis.Label.Font; }
		}
		[Browsable(false)]
		public Color ActualBackColor {
			get {
				if (!BackColor.IsEmpty)
					return BackColor;
				if (Axis.Diagram != null && Axis.Diagram.Chart != null) {
					CrosshairOptions crosshairOptions = Axis.Diagram.Chart.CrosshairOptions;
					return Axis.IsValuesAxis ? crosshairOptions.ValueLineColor : crosshairOptions.ArgumentLineColor;
				}
				return Color.FromArgb(0xDE, 0x39, 0xCD);
			}
		}
		[Browsable(false)]
		public Color ActualTextColor {
			get { return TextColor.IsEmpty ? Color.White : TextColor; }
		}
		[Browsable(false)]
		public string ActualPattern {
			get { return string.IsNullOrEmpty(pattern) ? string.Empty : pattern; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairAxisLabelOptionsVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairAxisLabelOptions.Visibility"),
		Category(Categories.Behavior),
		TypeConverter(typeof(DefaultBooleanConverter)),
		XtraSerializableProperty
		]
		public DefaultBoolean Visibility {
			get {
				return visibility;
			}
			set {
				if (value != visibility) {
					SendNotification(new ElementWillChangeNotification(this));
					visibility = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairAxisLabelOptionsPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairAxisLabelOptions.Pattern"),
		Editor("DevExpress.XtraCharts.Design.CrosshairAxisLabelPatternEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public string Pattern {
			get {
				return pattern;
			}
			set {
				if (value != pattern) {
					SendNotification(new ElementWillChangeNotification(this));
					pattern = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairAxisLabelOptionsBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairAxisLabelOptions.BackColor"),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All),
		NotifyParentProperty(true)
		]
		public Color BackColor {
			get {
				return backColor;
			}
			set {
				if (value != backColor) {
					SendNotification(new ElementWillChangeNotification(this));
					backColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairAxisLabelOptionsTextColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairAxisLabelOptions.TextColor"),
		XtraSerializableProperty
		]
		public Color TextColor {
			get {
				return textColor;
			}
			set {
				if (value != textColor) {
					SendNotification(new ElementWillChangeNotification(this));
					textColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairAxisLabelOptionsFont"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairAxisLabelOptions.Font"),
		TypeConverter(typeof(FontTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public Font Font {
			get {
				return font;
			}
			set {
				if (value != font) {
					SendNotification(new ElementWillChangeNotification(this));
					font = value;
					RaiseControlChanged();
				}
			}
		}
		internal CrosshairAxisLabelOptions(Axis2D axis) : base(axis) {
			font = DefaultFont;
		}
		#region XtraSeralizing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Pattern":
					return ShouldSerializePattern();
				case "BackColor":
					return ShouldSerializeBackColor();
				case "TextColor":
					return ShouldSerializeTextColor();
				case "Font":
					return ShouldSerializeFont();
				case "Visibility":
					return ShouldSerializeVisibility();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializePattern() {
			return !String.IsNullOrEmpty(pattern);
		}
		void ResetPattern() {
			Pattern = DefaultPattern;
		}
		bool ShouldSerializeBackColor() {
			return backColor != DefaultBackColor;
		}
		void ResetBackColor() {
			BackColor = DefaultBackColor;
		}
		bool ShouldSerializeTextColor() {
			return textColor != DefaultTextColor;
		}
		void ResetTextColor() {
			TextColor = DefaultTextColor;
		}
		bool ShouldSerializeFont() {
			return !Object.Equals(font, DefaultFont);
		}
		void ResetFont() {
			Font = DefaultFont;
		}
		bool ShouldSerializeVisibility() {
			return visibility != DefaultVisibility;
		}
		void ResetVisibility() {
			Visibility = DefaultVisibility;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializePattern() ||
				ShouldSerializeBackColor() ||
				ShouldSerializeTextColor() ||
				ShouldSerializeFont() ||
				ShouldSerializeVisibility();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new CrosshairAxisLabelOptions(Axis);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			CrosshairAxisLabelOptions options = obj as CrosshairAxisLabelOptions;
			if (options != null) {
				pattern = options.pattern;
				backColor = options.backColor;
				textColor = options.textColor;
				font = options.font;
				visibility = options.visibility;
			}
		}
	}
}
