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

using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
namespace DevExpress.XtraMap {
	public interface IMapStyleOwner {
		void OnStyleChanged();
	}
	public abstract class MapElementStyleBase : IOwnedElement {
		public static void MergeStyles(MapElementStyleBase style1, MapElementStyleBase style2, MapElementStyleBase destinationStyle) {
			style1.Merge(style2, destinationStyle);
		}
		object owner;
		#region IOwnedElementImplementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				owner = value;
			}
		}
		#endregion
		protected MapElementStyleBase() {
		}
		protected void StyleChanged() {
			IMapStyleOwner styleOwner = owner as IMapStyleOwner;
			if(styleOwner != null)
				styleOwner.OnStyleChanged();
		}
		protected virtual void Merge(MapElementStyleBase style, MapElementStyleBase destinationStyle) {
		}
		protected internal virtual void Reset() {
		}
	}
	public class MapItemTextStyle : MapElementStyleBase {
		Color textColor = Color.Empty;
		Color textGlowColor = Color.Empty;
		Font font = MapItemStyleProvider.DefaultFont;
		[Category(SRCategoryNames.Appearance),
		Description("")]
		public Color TextColor {
			get { return textColor; }
			set {
				if(textColor == value)
					return;
				textColor = value;
				StyleChanged();
			}
		}
		void ResetTextColor() { TextColor = Color.Empty; }
		bool ShouldSerializeTextColor() { return TextColor != Color.Empty; }
		[Category(SRCategoryNames.Appearance),
		Description("")]
		public Color TextGlowColor {
			get { return textGlowColor; }
			set {
				if(textGlowColor == value)
					return;
				textGlowColor = value;
				StyleChanged();
			}
		}
		void ResetTextGlowColor() { TextGlowColor = Color.Empty; }
		bool ShouldSerializeTextGlowColor() { return TextGlowColor != Color.Empty; }
		[Category(SRCategoryNames.Appearance),
		Description("")]
		public Font Font {
			get { return font; }
			set {
				if(font == value)
					return;
				font = value;
				StyleChanged();
			}
		}
		void ResetFont() { Font = MapItemStyleProvider.DefaultFont; }
		bool ShouldSerializeFont() { return Font != MapItemStyleProvider.DefaultFont; }
		protected override void Merge(MapElementStyleBase style, MapElementStyleBase destinationStyle) {
			base.Merge(style, destinationStyle);
			MapItemTextStyle itemStyle = style as MapItemTextStyle;
			MapItemTextStyle destinationItemStyle = destinationStyle as MapItemTextStyle;
			if(itemStyle == null || destinationItemStyle == null)
				return;
			destinationItemStyle.textColor = MapUtils.IsColorEmpty(this.textColor) ? itemStyle.textColor : this.textColor;
			destinationItemStyle.textGlowColor = MapUtils.IsColorEmpty(this.textGlowColor) ? itemStyle.textGlowColor : this.textGlowColor;
			destinationItemStyle.font = this.font == MapItemStyleProvider.DefaultFont ? itemStyle.font : this.font;
		}
		public override string ToString() {
			return "(MapItemTextStyle)";
		}
		protected internal override void Reset() {
			ResetFont();
			ResetTextColor();
			ResetTextGlowColor();
		}
	}
	public class MapItemStyle : MapItemTextStyle, IRenderItemStyle {
		internal const int EmptyStrokeWidth = -1;
		Color fill = Color.Empty;
		Color stroke = new Color();
		int strokeWidth = EmptyStrokeWidth;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemStyleFill"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Color Fill {
			get { return fill; }
			set {
				if(fill == value)
					return;
				fill = value;
				StyleChanged();
			}
		}
		void ResetFill() { Fill = Color.Empty; }
		bool ShouldSerializeFill() { return Fill != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemStyleStroke"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Color Stroke {
			get { return stroke; }
			set {
				if(stroke == value)
					return;
				stroke = value;
				StyleChanged();
			}
		}
		void ResetStroke() { Stroke = Color.Empty; }
		bool ShouldSerializeStroke() { return Stroke != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemStyleStrokeWidth"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(EmptyStrokeWidth)]
		public int StrokeWidth {
			get { return strokeWidth; }
			set {
				if(strokeWidth == value)
					return;
				strokeWidth = Math.Max(value, EmptyStrokeWidth);
				StyleChanged();
			}
		}
		public MapItemStyle() {
		}
		protected override void Merge(MapElementStyleBase style, MapElementStyleBase destinationStyle) {
			base.Merge(style, destinationStyle);
			MapItemStyle destinationItemStyle = destinationStyle as MapItemStyle;
			if(destinationItemStyle == null)
				return;
			MapItemColorStyle simpleStyle = style as MapItemColorStyle;
			if(simpleStyle != null) {
				if(simpleStyle.IsPriorityStyleColorEmpty(this))
					simpleStyle.ApplyColorTo(destinationItemStyle);
				return;
			}
			MapItemStyle itemStyle = style as MapItemStyle;
			if(itemStyle != null) {
				destinationItemStyle.fill = MapUtils.IsColorEmpty(this.fill) ? itemStyle.fill : this.fill;
				destinationItemStyle.stroke = MapUtils.IsColorEmpty(this.stroke) ? itemStyle.stroke : this.stroke;
				destinationItemStyle.strokeWidth = this.strokeWidth < 0 ? itemStyle.strokeWidth : this.strokeWidth;
			}
		}
		public override string ToString() {
			return "(MapItemStyle)";
		}
		protected internal override void Reset() {
			base.Reset();
			ResetFill();
			ResetStroke();
			StrokeWidth = EmptyStrokeWidth;
		}
	}
}
namespace DevExpress.XtraMap.Drawing {
	public abstract class MapItemColorStyle : MapElementStyleBase {
		Color color;
		protected internal Color Color {
			get { return color; }
			set {
				if(color == value)
					return;
				color = value;
			}
		}
		public abstract bool IsPriorityStyleColorEmpty(MapItemStyle style);
		public abstract void ApplyColorTo(MapItemStyle style);
	}
	public class MapItemFillStyle : MapItemColorStyle {
		public Color Fill {
			get { return base.Color; }
			set {
				base.Color = value;
			}
		}
		public override bool IsPriorityStyleColorEmpty(MapItemStyle style) {
			return MapUtils.IsColorEmpty(style.Fill);
		}
		public override void ApplyColorTo(MapItemStyle style) {
			style.Fill = Fill;
		}
	}
	public class MapItemStrokeStyle : MapItemColorStyle {
		public Color Stroke {
			get { return base.Color; }
			set {
				base.Color = value;
			}
		}
		public override bool IsPriorityStyleColorEmpty(MapItemStyle style) {
			return MapUtils.IsColorEmpty(style.Stroke);
		}
		public override void ApplyColorTo(MapItemStyle style) {
			style.Stroke = Stroke;
		}
	}
	public class MapItemTextColorStyle : MapItemColorStyle {
		public Color TextColor {
			get { return base.Color; }
			set {
				base.Color = value;
			}
		}
		public override bool IsPriorityStyleColorEmpty(MapItemStyle style) {
			return MapUtils.IsColorEmpty(style.TextColor);
		}
		public override void ApplyColorTo(MapItemStyle style) {
			style.TextColor = TextColor;
		}
	}
	public class MapItemTextGlowColorStyle : MapItemColorStyle {
		public Color TextGlowColor {
			get { return base.Color; }
			set {
				base.Color = value;
			}
		}
		public override bool IsPriorityStyleColorEmpty(MapItemStyle style) {
			return MapUtils.IsColorEmpty(style.TextGlowColor);
		}
		public override void ApplyColorTo(MapItemStyle style) {
			style.TextGlowColor = TextGlowColor;
		}
	}
}
