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
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public class TextElementStyle : MapElementStyleBase {
		Color textColor = Color.Empty;
		Font font = AppearanceObject.DefaultFont;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("TextElementStyleTextColor"),
#endif
		Category(SRCategoryNames.Appearance), NotifyParentProperty(true)]
		public Color TextColor {
			get { return textColor; }
			set {
				if (textColor == value)
					return;
				textColor = value;
				StyleChanged();
			}
		}
		void ResetTextColor() { TextColor = Color.Empty; }
		bool ShouldSerializeTextColor() { return TextColor != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("TextElementStyleFont"),
#endif
		Category(SRCategoryNames.Appearance), NotifyParentProperty(true)]
		public Font Font {
			get { return font; }
			set {
				if (font == value)
					return;
				font = value;
				StyleChanged();
			}
		}
		void ResetFont() { Font = AppearanceObject.DefaultFont; }
		bool ShouldSerializeFont() { return Font != AppearanceObject.DefaultFont; }
		internal TextElementStyle() {
		}
		protected override void Merge(MapElementStyleBase style, MapElementStyleBase destinationStyle) {
			base.Merge(style, destinationStyle);
			TextElementStyle textStyle = style as TextElementStyle;
			TextElementStyle destinationTextStyle = destinationStyle as TextElementStyle;
			if (textStyle == null || destinationTextStyle == null)
				return;
			destinationTextStyle.textColor = MapUtils.IsColorEmpty(this.textColor) ? textStyle.textColor : this.textColor;
			destinationTextStyle.font = this.font == AppearanceObject.DefaultFont ? textStyle.font : this.font;
		}
		public override string ToString() {
			return "(TextElementStyle)";
		}
	}
	public class BackgroundStyle : MapElementStyleBase {
		internal static readonly Color DefaultOverlayBackColor = Color.FromArgb(0xB0, 0x0, 0x0, 0x0);
		Color fill = Color.Empty;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BackgroundStyleFill"),
#endif
		Category(SRCategoryNames.Appearance), NotifyParentProperty(true)]
		public Color Fill {
			get { return fill; }
			set {
				if (fill == value)
					return;
				fill = value;
				StyleChanged();
			}
		}
		void ResetFill() { Fill = Color.Empty; }
		bool ShouldSerializeFill() { return Fill != Color.Empty; }
		internal BackgroundStyle() {
		}
		protected override void Merge(MapElementStyleBase style, MapElementStyleBase destinationStyle) {
			base.Merge(style, destinationStyle);
			BackgroundStyle backStyle = style as BackgroundStyle;
			BackgroundStyle destinationBackStyle = destinationStyle as BackgroundStyle;
			if (backStyle == null || destinationBackStyle == null)
				return;
			destinationBackStyle.fill = MapUtils.IsColorEmpty(this.fill) ? backStyle.fill : this.fill;
		}
		public override string ToString() {
			return "(BackgroundStyle)";
		}
	}
	public class BorderedElementStyle : BackgroundStyle {
		Color stroke = Color.Empty;
		[
		Category(SRCategoryNames.Appearance), NotifyParentProperty(true)]
		public Color Stroke {
			get { return stroke; }
			set {
				if (stroke == value)
					return;
				stroke = value;
				StyleChanged();
			}
		}
		void ResetStroke() { Stroke = Color.Empty; }
		bool ShouldSerializeStroke() { return Stroke != Color.Empty; }
		protected override void Merge(MapElementStyleBase style, MapElementStyleBase destinationStyle) {
			base.Merge(style, destinationStyle);
			BorderedElementStyle borderedStyle = style as BorderedElementStyle;
			BorderedElementStyle destinationBackStyle = destinationStyle as BorderedElementStyle;
			if (borderedStyle == null || destinationBackStyle == null)
				return;
			destinationBackStyle.Stroke = MapUtils.IsColorEmpty(this.Stroke) ? borderedStyle.Stroke : this.Stroke;
		}
		public override string ToString() {
			return "(BorderedElementStyle)";
		}
	}
	public class MapElementStyle : TextElementStyle {
		Color fill = Color.Empty;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapElementStyleFill"),
#endif
		Category(SRCategoryNames.Appearance), NotifyParentProperty(true)]
		public Color Fill {
			get { return fill; }
			set {
				if (fill == value)
					return;
				fill = value;
				StyleChanged();
			}
		}
		void ResetFill() { Fill = Color.Empty; }
		bool ShouldSerializeFill() { return Fill != Color.Empty; }
		internal MapElementStyle() {
		}
		protected override void Merge(MapElementStyleBase style, MapElementStyleBase destinationStyle) {
			base.Merge(style, destinationStyle);
			MapElementStyle elementStyle = style as MapElementStyle;
			MapElementStyle destinationElementStyle = destinationStyle as MapElementStyle;
			if (elementStyle == null || destinationElementStyle == null)
				return;
			destinationElementStyle.fill = MapUtils.IsColorEmpty(this.fill) ? elementStyle.fill : this.fill;
		}
		public override string ToString() {
			return "(MapElementStyle)";
		}
	}
}
namespace DevExpress.XtraMap.Drawing {
	public interface IViewInfoStyleProvider {
		NavigationPanelAppearance DefaultNavigationPanelAppearance { get; }
		LegendAppearance DefaultLegendAppearance { get; }
		ErrorPanelAppearance DefaultErrorPanelAppearance { get; }
		SearchPanelAppearance DefaultSearchPanelAppearance { get; }
		CustomOverlayAppearance DefaultCustomOverlayAppearance { get; }
		OverlayTextAppearance DefaultOverlayTextAppearance { get; }
	}
	public static class MapAppearanceNames {
		public const string NavigationPanelBackground = "NavigationPanelBackground";
		public const string NavigationPanelItem = "NavigationPanelHotItem";
		public const string NavigationPanelHotTrackedItem = "NavigationPanelHotTracked";
		public const string NavigationPanelScaleInfoArea = "NavigationPanelScaleInfo";
		public const string NavigationPanelPressedItem = "NavigationPanelPressed";
		public const string NavigationPanelCoordinates = "NavigationPanelCoordinates";
		public const string LegendBackground = "LegendBackground";
		public const string LegendItem = "LegendItem";
		public const string LegendHeader = "LegendHeader";
		public const string LegendDescription = "LegendDescription";
		public const string ErrorPanelBackground = "ErrorPanelBackground";
		public const string ErrorPanelText = "ErrorPanelText";
		public const string SearchPanelBackground = "SearchPanelBackground";
		public const string CustomOverlayBackground = "CustomOverlayBackground";
		public const string OverlayItemBackground = "OverlayItemBackground";
		public const string OverlayHotTrackedItem = "OverlayHotTrackedItem";
		public const string OverlayItemText = "OverlayItemText";
	}
	public abstract class MapStyleCollection : Dictionary<string, MapElementStyleBase> {
		readonly IMapStyleOwner stylesOwner;
		protected MapStyleCollection(IMapStyleOwner stylesOwner) {
			this.stylesOwner = stylesOwner;
			CreateStyles();
		}
		protected void AddStyle(string name, MapElementStyleBase style) {
			Add(name, style);
			MapUtils.SetOwner(style, stylesOwner);
		}
		protected virtual void CreateStyles() { }
	}
	public class EmptyAppearance : MapStyleCollection {
		static readonly EmptyAppearance instance = new EmptyAppearance();
		public static EmptyAppearance Instance { get { return instance; } }
		EmptyAppearance()
			: base(null) {
		}
	}
	public class NavigationPanelAppearance : MapStyleCollection {
		internal static readonly Color DefaultElementColor = Color.FromArgb(0xFF, 0xAD, 0xDF, 0xFF);
		internal static readonly Color DefaultHotTrackColor = Color.White;
		internal static readonly Color DefaultPressedColor = Color.FromArgb(90, Color.White);
		BackgroundStyle backgroundStyle;
		BackgroundStyle itemStyle;
		BackgroundStyle hotTrackedItemStyle;
		BackgroundStyle pressedItemStyle;
		TextElementStyle scaleStyle;
		TextElementStyle coordinatesStyle;
		public BackgroundStyle BackgroundStyle { get { return backgroundStyle; } }
		public BackgroundStyle ItemStyle { get { return itemStyle; } }
		public BackgroundStyle HotTrackedItemStyle { get { return hotTrackedItemStyle; } }
		public BackgroundStyle PressedItemStyle { get { return pressedItemStyle; } }
		public TextElementStyle ScaleStyle { get { return scaleStyle; } }
		public TextElementStyle CoordinatesStyle { get { return coordinatesStyle; } }
		public NavigationPanelAppearance(IMapStyleOwner stylesOwner)
			: base(stylesOwner) {
		}
		protected override void CreateStyles() {
			this.backgroundStyle = new BackgroundStyle();
			AddStyle(MapAppearanceNames.NavigationPanelBackground, backgroundStyle);
			this.itemStyle = new BackgroundStyle();
			AddStyle(MapAppearanceNames.NavigationPanelItem, itemStyle);
			this.hotTrackedItemStyle = new BackgroundStyle();
			AddStyle(MapAppearanceNames.NavigationPanelHotTrackedItem, hotTrackedItemStyle);
			this.scaleStyle = new TextElementStyle();
			AddStyle(MapAppearanceNames.NavigationPanelScaleInfoArea, scaleStyle);
			this.pressedItemStyle = new BackgroundStyle();
			AddStyle(MapAppearanceNames.NavigationPanelPressedItem, pressedItemStyle);
			this.coordinatesStyle = new TextElementStyle();
			AddStyle(MapAppearanceNames.NavigationPanelCoordinates, coordinatesStyle);
		}
	}
	public class LegendAppearance : MapStyleCollection {
		internal static readonly Color DefaultElementColor = Color.FromArgb(0xFF, 0xAD, 0xDF, 0xFF);
		BackgroundStyle backgroundStyle;
		MapElementStyle itemStyle;
		TextElementStyle headerStyle;
		TextElementStyle descriptionStyle;
		public BackgroundStyle BackgroundStyle { get { return backgroundStyle; } }
		public MapElementStyle ItemStyle { get { return itemStyle; } }
		public TextElementStyle HeaderStyle { get { return headerStyle; } }
		public TextElementStyle DescriptionStyle { get { return descriptionStyle; } }
		public LegendAppearance(IMapStyleOwner stylesOwner)
			: base(stylesOwner) {
		}
		protected override void CreateStyles() {
			this.backgroundStyle = new BackgroundStyle();
			AddStyle(MapAppearanceNames.LegendBackground, backgroundStyle);
			this.itemStyle = new MapElementStyle();
			AddStyle(MapAppearanceNames.LegendItem, itemStyle);
			this.headerStyle = new TextElementStyle();
			AddStyle(MapAppearanceNames.LegendHeader, headerStyle);
			this.descriptionStyle = new TextElementStyle();
			AddStyle(MapAppearanceNames.LegendDescription, descriptionStyle);
		}
	}
	public class ErrorPanelAppearance : MapStyleCollection {
		internal static readonly Color DefaultBackgroundColor = Color.FromArgb(191, 221, 0, 0);
		internal static readonly Color DefaultTextColor = Color.White;
		BackgroundStyle backgroundStyle;
		TextElementStyle textStyle;
		public BackgroundStyle BackgroundStyle { get { return backgroundStyle; } }
		public TextElementStyle TextStyle { get { return textStyle; } }
		public ErrorPanelAppearance(IMapStyleOwner stylesOwner)
			: base(stylesOwner) {
		}
		protected override void CreateStyles() {
			this.backgroundStyle = new BackgroundStyle();
			AddStyle(MapAppearanceNames.ErrorPanelBackground, backgroundStyle);
			this.textStyle = new TextElementStyle();
			AddStyle(MapAppearanceNames.ErrorPanelText, textStyle);
		}
	}
	public class SearchPanelAppearance : MapStyleCollection {
		BackgroundStyle backgroundStyle;
		public BackgroundStyle BackgroundStyle { get { return backgroundStyle; } }
		public SearchPanelAppearance(IMapStyleOwner stylesOwner)
			: base(stylesOwner) {
		}
		protected override void CreateStyles() {
			this.backgroundStyle = new BackgroundStyle();
			AddStyle(MapAppearanceNames.SearchPanelBackground, backgroundStyle);
		}
	}
	public class CustomOverlayAppearance : MapStyleCollection {
		BackgroundStyle backgroundStyle;
		public BackgroundStyle BackgroundStyle { get { return backgroundStyle; } }
		public CustomOverlayAppearance(IMapStyleOwner stylesOwner)
			: base(stylesOwner) {
		}
		protected override void CreateStyles() {
			base.CreateStyles();
			this.backgroundStyle = new BackgroundStyle();
			AddStyle(MapAppearanceNames.CustomOverlayBackground, backgroundStyle);
		}
	}
	public class OverlayItemAppearance : MapStyleCollection {
		BackgroundStyle itemStyle;
		BackgroundStyle hotTrackedItemStyle;
		public BackgroundStyle ItemStyle { get { return itemStyle; } }
		public BackgroundStyle HotTrackedItemStyle { get { return hotTrackedItemStyle; } }
		public OverlayItemAppearance(IMapStyleOwner stylesOwner)
			: base(stylesOwner) {
		}
		protected override void CreateStyles() {
			base.CreateStyles();
			this.itemStyle = new BackgroundStyle();
			this.hotTrackedItemStyle = new BackgroundStyle();
			AddStyle(MapAppearanceNames.OverlayItemBackground, itemStyle);
			AddStyle(MapAppearanceNames.OverlayHotTrackedItem, hotTrackedItemStyle);
		}
	}
	public class OverlayTextAppearance : OverlayItemAppearance {
		internal static readonly Color DefaultTextColor = Color.FromArgb(0xFF, 0xAD, 0xDF, 0xFF);
		TextElementStyle textStyle;
		public TextElementStyle TextStyle { get { return textStyle; } }
		public OverlayTextAppearance(IMapStyleOwner stylesOwner)
			: base(stylesOwner) {
		}
		protected override void CreateStyles() {
			base.CreateStyles();
			this.textStyle = new TextElementStyle();
			AddStyle(MapAppearanceNames.OverlayItemText, textStyle);
		}
	}
}
