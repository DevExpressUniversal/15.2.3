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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Drawing {
	public class MapBorderStyle : MapElementStyleBase {
		SkinElement borderElement;
		public SkinElement BorderElement {
			get { return borderElement; }
			set {
				if(Object.Equals(value, BorderElement))
					return;
				borderElement = value;
				StyleChanged();
			}
		}
	}
	public interface IMapItemStyleProvider {
		MapItemStyle ShapeStyle { get; }
		MapItemStyle SelectedShapeStyle { get; }
		MapItemStyle HighlightedShapeStyle { get; }
		MapPointerStyle CustomElementStyle { get; }
		MapItemStyle SelectedCustomElementStyle { get; }
		MapItemStyle HighlightedCustomElementStyle { get; }
		MapItemStyle LineStyle { get; }
		MapItemStyle PolylineSegmentStyle { get; }
		MapItemStyle PolygonSegmentStyle { get; }
		MapItemStyle PieSegmentStyle { get; }
		PushpinStyle PushpinStyle { get; }
		BorderedElementStyle SelectedRegionStyle { get; }
		MapCalloutStyle CalloutStyle { get; }
 		MapCalloutStyle SelectedCalloutStyle { get; }
		MapCalloutStyle HighlightedCalloutStyle { get; }
		MapBorderStyle MapBorderStyle { get; }
	}
	public class MapItemStyleProvider : IMapItemStyleProvider {
		#region Consts
		internal const int SelectedCustomElementAlpha = 128;
		internal const int HighlightedCustomElementAlpha = 90;
		internal const int SelectedItemStrokeAlpha = 200;
		internal const int HighlightedItemStrokeAlpha = 120;
		internal const int DefaultStrokeWidth = 2;
		internal readonly static Font DefaultFont = new Font(AppearanceObject.DefaultFont, FontStyle.Bold);
		internal readonly static Color DefaultTextColor = Color.Black;
		internal readonly static Color DefaultTextGlowColor = Color.White;
		static Color DefaultFill = Color.White;
		static Color DefaultStroke = Color.Empty;
		static Color DefaultSelectedStroke = Color.Black;
		static Color DefaultPathStroke = Color.Black;
		static Color DefaultSelectedRegionStroke = Color.FromArgb(255, 51, 153, 255);
		static Color DefaultSelectedRegionFill = Color.FromArgb(128, 0, 114, 174);
		#endregion
		#region Static
		internal static readonly MapItemStyleProvider Default = new MapItemStyleProvider();
		static readonly MapItemStyle shapeStyle = new MapItemStyle() {
			Fill = DefaultFill,
			Stroke = DefaultStroke,
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			StrokeWidth = DefaultStrokeWidth
		};
		static readonly MapItemStyle selectedShapeStyle = new MapItemStyle() {
			Fill = Color.Empty,
			Stroke = Color.FromArgb(SelectedItemStrokeAlpha, DefaultSelectedStroke),
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			StrokeWidth = DefaultStrokeWidth
		};
		static readonly MapItemStyle highlightedShapeStyle = new MapItemStyle() {
			Fill = Color.Empty,
			Stroke = Color.FromArgb(HighlightedItemStrokeAlpha, DefaultSelectedStroke),
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			StrokeWidth = DefaultStrokeWidth
		};
		static readonly MapPointerStyle customElementStyle = new MapPointerStyle() {
			Fill = Color.Empty,
			Stroke = Color.Empty,
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			StrokeWidth = 0,
			ContentPadding = CustomElementPainter.DefaultPadding
		};
		static readonly MapItemStyle selectedCustomElementStyle = new MapItemStyle() {
			Fill = Color.FromArgb(SelectedCustomElementAlpha, DefaultFill),
			Stroke = Color.FromArgb(SelectedCustomElementAlpha, DefaultSelectedStroke),
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			StrokeWidth = DefaultStrokeWidth
		};
		static readonly MapItemStyle highlightedCustomElementStyle = new MapItemStyle() {
			Fill = Color.FromArgb(HighlightedCustomElementAlpha, DefaultFill),
			Stroke = Color.FromArgb(HighlightedCustomElementAlpha, DefaultSelectedStroke),
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			StrokeWidth = DefaultStrokeWidth
		};
		static readonly MapItemStyle lineStyle = new MapItemStyle() {
			Fill = Color.Empty,
			Stroke = DefaultFill,
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			StrokeWidth = DefaultStrokeWidth
		};
		static readonly MapItemStyle polylineSegmentStyle = new MapItemStyle() {
			Fill = Color.Empty,
			Stroke = DefaultPathStroke,
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			StrokeWidth = DefaultStrokeWidth
		};
		static readonly MapItemStyle polygonSegmentStyle = new MapItemStyle() {
			Fill = DefaultFill,
			Stroke = Color.Empty,
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			StrokeWidth = DefaultStrokeWidth
		};
		static readonly MapItemStyle pieSegmentStyle = new MapItemStyle() {
			Fill = DefaultFill,
			Stroke = Color.Transparent,
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			StrokeWidth = 0
		};
		static readonly PushpinStyle pushpinStyle = new PushpinStyle() {
			Image = MapUtils.DefaultPushpinImage,
			SelectedImage = MapUtils.DefaultSelectedPushpinImage,
			HighlightedImage = MapUtils.DefaultHighlightedPushpinImage,
			TextOrigin = MapUtils.DefaultPushpinTextOrigin,
			ImageOrigin = TextImageItemPainterBase.DefaultRenderOrigin,
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor
		};
		static readonly BorderedElementStyle selectedRegionStyle = new BorderedElementStyle() {
			Fill = DefaultSelectedRegionFill,
			Stroke = DefaultSelectedRegionStroke
		};
		static readonly MapCalloutStyle calloutStyle = new MapCalloutStyle() {
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			TextOrigin = Point.Empty,
			ContentPadding = CalloutPainter.DefaultPadding,
			BaseOffset = CalloutPainter.DefaultBaseOffset,
			PointerSize = CalloutPainter.DefaultCursorSize,
			Stroke = MapCalloutStyle.DefaultBorderColor,
			StrokeWidth = MapCalloutStyle.DefaultStrokeWidth,
			Fill = MapCalloutStyle.DefaultFillColor,
			HighlightedEffectiveArea = new SkinPaddingEdges(0, 0, 0, CalloutPainter.DefaultCursorSize.Height)
		};
		static readonly MapCalloutStyle selectedCalloutStyle = new MapCalloutStyle() {
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			TextOrigin = Point.Empty,
			ContentPadding = CalloutPainter.DefaultPadding,
			BaseOffset = CalloutPainter.DefaultBaseOffset,
			PointerSize = CalloutPainter.DefaultCursorSize,
			Stroke = MapCalloutStyle.DefaultSelectedBorderColor,
			StrokeWidth = MapCalloutStyle.DefaultStrokeWidth,
			Fill = MapCalloutStyle.DefaultSelectedFillColor
		};
		static readonly MapCalloutStyle highlightedCalloutStyle = new MapCalloutStyle() {
			TextColor = DefaultTextColor,
			TextGlowColor = DefaultTextGlowColor,
			TextOrigin = Point.Empty,
			ContentPadding = CalloutPainter.DefaultPadding,
			BaseOffset = CalloutPainter.DefaultBaseOffset,
			PointerSize = CalloutPainter.DefaultCursorSize,
			Stroke = MapCalloutStyle.DefaultHighlightedBorderColor,
			StrokeWidth = MapCalloutStyle.DefaultStrokeWidth,
			Fill = MapCalloutStyle.DefaultHighlightedFillColor	
		};
		static readonly MapBorderStyle mapBorderStyle = new MapBorderStyle();
		#endregion
		public MapItemStyle ShapeStyle { get { return shapeStyle; } }
		public MapItemStyle SelectedShapeStyle { get { return selectedShapeStyle; } }
		public MapItemStyle HighlightedShapeStyle { get { return highlightedShapeStyle; } }
		public MapPointerStyle CustomElementStyle { get { return customElementStyle; } }
		public MapItemStyle SelectedCustomElementStyle { get { return selectedCustomElementStyle; } }
		public MapItemStyle HighlightedCustomElementStyle { get { return highlightedCustomElementStyle; } }
		public MapItemStyle LineStyle { get { return lineStyle; } }
		public MapItemStyle PolylineSegmentStyle { get { return polylineSegmentStyle; } }
		public MapItemStyle PolygonSegmentStyle { get { return polygonSegmentStyle; } }
		public MapItemStyle PieSegmentStyle { get { return pieSegmentStyle; } }
		public PushpinStyle PushpinStyle { get { return pushpinStyle; } }
		public BorderedElementStyle SelectedRegionStyle { get { return selectedRegionStyle; } }
		public MapCalloutStyle CalloutStyle { get { return calloutStyle; } }
		public MapCalloutStyle SelectedCalloutStyle { get { return selectedCalloutStyle; } }
		public MapCalloutStyle HighlightedCalloutStyle { get { return highlightedCalloutStyle; } }
		public MapBorderStyle MapBorderStyle { get { return mapBorderStyle; } }
	}
	public class PushpinStyle : MapItemTextStyle {
		Image image;
		Image selectedImage;
		Image highlightedImage;
		Point textOrigin;
		MapPoint imageOrigin;
		public Image Image {
			get { return image; }
			set {
				if(Object.Equals(value, image))
					return;
				image = value;
				StyleChanged();
			}
		}
		public Image SelectedImage {
			get { return selectedImage; }
			set {
				if(Object.Equals(value, selectedImage))
					return;
				selectedImage = value;
				StyleChanged();
			}
		}
		public Image HighlightedImage {
			get { return highlightedImage; }
			set {
				if(Object.Equals(value, highlightedImage))
					return;
				highlightedImage = value;
				StyleChanged();
			}
		}
		public Point TextOrigin {
			get { return textOrigin; }
			set {
				if(textOrigin == value)
					return;
				textOrigin = value;
				StyleChanged();
			}
		}
		public MapPoint ImageOrigin {
			get { return imageOrigin; }
			set {
				if(imageOrigin == value)
					return;
				imageOrigin = value;
				StyleChanged();
			}
		}
	}
	public class MapPointerStyle : MapItemStyle {
		public static readonly Color DefaultBaseColor = Color.White;
		Color baseColor;
		Color highlightedBaseColor;
		Color selectedBaseColor;
		Padding contentPadding;
		SkinElement backgroundElement;
		SkinPaddingEdges highlightedEffectiveArea = new SkinPaddingEdges();
		public Padding ContentPadding {
			get { return contentPadding; }
			set {
				if(contentPadding == value)
					return;
				contentPadding = value;
				StyleChanged();
			}
		}
		public SkinElement BackgroundElement {
			get { return backgroundElement; }
			set {
				if(Object.Equals(value, BackgroundElement))
					return;
				backgroundElement = value;
				StyleChanged();
			}
		}
		public Color BaseColor {
			get { return baseColor; }
			set {
				if(Object.Equals(value, baseColor))
					return;
				baseColor = value;
				StyleChanged();
			}
		}
		public Color HighlightedBaseColor {
			get { return highlightedBaseColor; }
			set {
				if (Object.Equals(value, highlightedBaseColor))
					return;
				highlightedBaseColor = value;
				StyleChanged();
			}
		}
		public Color SelectedBaseColor {
			get { return selectedBaseColor; }
			set {
				if (Object.Equals(value, selectedBaseColor))
					return;
				selectedBaseColor = value;
				StyleChanged();
			}
		}
		public SkinPaddingEdges HighlightedEffectiveArea {
			get { return highlightedEffectiveArea; }
			set {
				if(Object.Equals(value, highlightedEffectiveArea))
					return;
				highlightedEffectiveArea = value;
				StyleChanged();
			}
		}
	}
	public class MapCalloutStyle : MapPointerStyle {
		public const int DefaultStrokeWidth = 1;
		public static readonly Color DefaultBorderColor = Color.Black;
		public static readonly Color DefaultSelectedBorderColor = Color.FromArgb(128, DefaultBorderColor);
		public static readonly Color DefaultHighlightedBorderColor = Color.FromArgb(90, DefaultBorderColor);
		public static readonly Color DefaultFillColor = Color.FromArgb(228, 228, 232);
		public static readonly Color DefaultSelectedFillColor = Color.White;
		public static readonly Color DefaultHighlightedFillColor = Color.FromArgb(237, 227, 248);
		Point textOrigin;
		Point baseOffset;
		Size pointerSize;
		public Point TextOrigin {
			get { return textOrigin; }
			set {
				if(textOrigin == value)
					return;
				textOrigin = value;
				StyleChanged();
			}
		}
		public Point BaseOffset {
			get { return baseOffset; }
			set {
				if(baseOffset == value)
					return;
				baseOffset = value;
				StyleChanged();
			}
		}
		public Size PointerSize {
			get { return pointerSize; }
			set {
				if(pointerSize == value)
					return;
				pointerSize = value;
				StyleChanged();
			}
		}
	}
	public class SkinMapItemStyleProvider : IMapItemStyleProvider { 
		ISkinProvider skinProvider;
		MapItemStyle shapeStyle;
		MapItemStyle selectedShapeStyle;
		MapItemStyle highlightedShapeStyle;
		MapPointerStyle customElementStyle;
		MapItemStyle lineStyle;
		MapItemStyle polylineSegmentStyle;
		MapItemStyle pieSegmentStyle;
		MapItemStyle polygonSegmentStyle;
		PushpinStyle pushpinStyle;
		BorderedElementStyle selectedRegionStyle;
		MapCalloutStyle calloutStyle;
		MapBorderStyle mapBorderStyle;
		public MapItemStyle ShapeStyle { get { return shapeStyle; } }
		public MapItemStyle SelectedShapeStyle { get { return selectedShapeStyle; } }
		public MapItemStyle HighlightedShapeStyle { get { return highlightedShapeStyle; } }
		public MapPointerStyle CustomElementStyle { get { return customElementStyle; } }
		public MapItemStyle SelectedCustomElementStyle { get { return customElementStyle; } }
		public MapItemStyle HighlightedCustomElementStyle { get { return customElementStyle; } }
		public MapItemStyle LineStyle { get { return lineStyle; } }
		public MapItemStyle PolylineSegmentStyle { get { return polylineSegmentStyle; } }
		public MapItemStyle PieSegmentStyle { get { return pieSegmentStyle; } }
		public MapItemStyle PolygonSegmentStyle { get { return polygonSegmentStyle; } }
		public PushpinStyle PushpinStyle { get { return pushpinStyle; } }
		public BorderedElementStyle SelectedRegionStyle { get { return selectedRegionStyle; } }
		public MapCalloutStyle CalloutStyle { get { return calloutStyle; } }
 		public MapCalloutStyle SelectedCalloutStyle { get { return calloutStyle; } }
		public MapCalloutStyle HighlightedCalloutStyle { get { return calloutStyle; } }
		public MapBorderStyle MapBorderStyle { get { return mapBorderStyle; } }
		public SkinMapItemStyleProvider(ISkinProvider skinProvider) {
			Guard.ArgumentNotNull(skinProvider, "skinProvider");
			this.skinProvider = skinProvider;
			shapeStyle = new MapItemStyle();
			selectedShapeStyle = new MapItemStyle();
			highlightedShapeStyle = new MapItemStyle();
			customElementStyle = new MapPointerStyle();
			lineStyle = new MapItemStyle();
			polylineSegmentStyle = new MapItemStyle();
			pieSegmentStyle = new MapItemStyle();
			polygonSegmentStyle = new MapItemStyle();
			pushpinStyle = new PushpinStyle();
			selectedRegionStyle = new BorderedElementStyle();
			calloutStyle = new MapCalloutStyle();
			mapBorderStyle = new MapBorderStyle();
			UpdateStyles();
		}
		void UpdateStyles() {
			SkinPainterHelper.UpdateShapeStyle(shapeStyle, skinProvider);
			SkinPainterHelper.UpdateSelectedShapeStyle(selectedShapeStyle, skinProvider);
			SkinPainterHelper.UpdateHighlightedShapeStyle(highlightedShapeStyle, skinProvider);
			SkinPainterHelper.UpdateLineStyle(lineStyle, skinProvider);
			SkinPainterHelper.UpdatePolylineStyle(polylineSegmentStyle, skinProvider);
			SkinPainterHelper.UpdatePolygonSegmentStyle(polygonSegmentStyle, skinProvider);
			UpdatePieSegmentStyle(polygonSegmentStyle);
			SkinPainterHelper.UpdatePushpinStyle(pushpinStyle, skinProvider);
			SkinPainterHelper.UpdateCalloutStyle(calloutStyle, skinProvider);
			SkinPainterHelper.UpdateCustomElementStyle(customElementStyle, skinProvider);
			SkinPainterHelper.UpdateSelectedRegionStyle(selectedRegionStyle, skinProvider);
			SkinPainterHelper.UpdateMapBorderStyle(mapBorderStyle, skinProvider);
		}
		void UpdatePieSegmentStyle(MapItemStyle polylineSegmentStyle) {
			pieSegmentStyle.Fill = polylineSegmentStyle.Fill;
			pieSegmentStyle.Stroke = Color.Transparent;
			pieSegmentStyle.StrokeWidth = 0;
		}
	}
	public abstract class ViewInfoStyleProviderBase : MapDisposableObject {
		NavigationPanelAppearance navigationPanelAppearance;
		LegendAppearance legendAppearance;
		ErrorPanelAppearance errorPanelAppearance;
		SearchPanelAppearance searchPanelAppearance;
		CustomOverlayAppearance customOverlayAppearance;
		OverlayTextAppearance overlayTextAppearance;
		public virtual NavigationPanelAppearance DefaultNavigationPanelAppearance { get { return navigationPanelAppearance; } }
		public virtual LegendAppearance DefaultLegendAppearance { get { return legendAppearance; } }
		public virtual ErrorPanelAppearance DefaultErrorPanelAppearance { get { return errorPanelAppearance; } }
		public virtual SearchPanelAppearance DefaultSearchPanelAppearance { get { return searchPanelAppearance; } }
		public virtual CustomOverlayAppearance DefaultCustomOverlayAppearance { get { return customOverlayAppearance; } }
		public virtual OverlayTextAppearance DefaultOverlayTextAppearance { get { return overlayTextAppearance; } }
		protected ViewInfoStyleProviderBase() {
			Update();
		}
		void DisposeAppearance() {
			if(this.navigationPanelAppearance != null){
				this.navigationPanelAppearance.CoordinatesStyle.Font.Dispose();
				this.navigationPanelAppearance.ScaleStyle.Font.Dispose();
				this.navigationPanelAppearance = null;
			}
			if(this.legendAppearance != null){
				this.legendAppearance.ItemStyle.Font.Dispose();
				this.legendAppearance.HeaderStyle.Font.Dispose();
				this.legendAppearance.DescriptionStyle.Font.Dispose();
				this.legendAppearance = null;
			}
			if(this.overlayTextAppearance != null) {
				this.overlayTextAppearance.TextStyle.Font.Dispose();
				this.overlayTextAppearance = null;
			}
		}
		public virtual void Update() {
			DisposeAppearance();
			this.navigationPanelAppearance = CreateNavPanelDefaultAppearance();
			this.legendAppearance = CreateLegendDefaultAppearance();
			this.errorPanelAppearance = CreateErrorPanelAppearance();
			this.searchPanelAppearance = CreateSearchPanelAppearance();
			this.customOverlayAppearance = CreateCustomOverlayAppearance();
			this.overlayTextAppearance = CreateOverlayTextAppearance();
		}
		protected abstract NavigationPanelAppearance CreateNavPanelDefaultAppearance();
		protected abstract LegendAppearance CreateLegendDefaultAppearance();
		protected abstract ErrorPanelAppearance CreateErrorPanelAppearance();
		protected abstract SearchPanelAppearance CreateSearchPanelAppearance();
		protected abstract CustomOverlayAppearance CreateCustomOverlayAppearance();
		protected abstract OverlayTextAppearance CreateOverlayTextAppearance();
		protected override void DisposeOverride() {
			base.DisposeOverride();
			DisposeAppearance();
		}
	}
	public class SkinViewInfoStyleProvider : ViewInfoStyleProviderBase {
		ISkinProvider provider;
		public SkinViewInfoStyleProvider(ISkinProvider provider) {
			this.provider = provider;
			Update();
		}
		protected override NavigationPanelAppearance CreateNavPanelDefaultAppearance() {
			NavigationPanelAppearance navAp = new NavigationPanelAppearance(null);
			navAp.BackgroundStyle.Fill = SkinPainterHelper.GetSkinAlphaColorProperty(this.provider, MapSkins.PropPanelBackColor, MapSkins.PropPanelBackColorAlpha);
			Color textColor = SkinPainterHelper.GetSkinColorProperty(this.provider, MapSkins.PropPanelTextColor);
			navAp.ItemStyle.Fill = textColor;
			navAp.ScaleStyle.TextColor = textColor;
			navAp.ScaleStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 8, FontStyle.Bold);
			navAp.HotTrackedItemStyle.Fill = SkinPainterHelper.GetSkinColorProperty(this.provider, MapSkins.PropPanelHotTrackedTextColor);
			navAp.PressedItemStyle.Fill = SkinPainterHelper.GetSkinAlphaColorProperty(this.provider, MapSkins.PropPanelPressedTextColor, MapSkins.PropPanelPressedTextColorAlpha);
			navAp.CoordinatesStyle.TextColor = textColor;
			navAp.CoordinatesStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 16);
			return navAp;
		}
		protected override LegendAppearance CreateLegendDefaultAppearance() {
			LegendAppearance appearance = new LegendAppearance(null);
			appearance.BackgroundStyle.Fill = SkinPainterHelper.GetSkinAlphaColorProperty(this.provider, MapSkins.PropPanelBackColor, MapSkins.PropPanelBackColorAlpha);
			Color textColor = SkinPainterHelper.GetSkinColorProperty(this.provider, MapSkins.PropPanelTextColor);
			Color itemColor = SkinPainterHelper.GetSkinElementColor(this.provider, MapSkins.SkinShape, MapSkins.PropElementBackColor, Color.Empty);
			appearance.ItemStyle.Fill = itemColor;
			appearance.ItemStyle.TextColor = textColor;
			appearance.ItemStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 10);
			appearance.HeaderStyle.TextColor = textColor;
			appearance.HeaderStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 16);
			appearance.DescriptionStyle.TextColor = textColor;
			appearance.DescriptionStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 12);
			return appearance;
		}
		protected override ErrorPanelAppearance CreateErrorPanelAppearance() {
			ErrorPanelAppearance appearance = new ErrorPanelAppearance(null);
			appearance.BackgroundStyle.Fill = SkinPainterHelper.GetSkinAlphaColorProperty(this.provider, MapSkins.PropPanelBackColor, MapSkins.PropPanelBackColorAlpha);
			appearance.TextStyle.TextColor = SkinPainterHelper.GetSkinColorProperty(this.provider, MapSkins.PropPanelTextColor);
			return appearance;
		}
		protected override SearchPanelAppearance CreateSearchPanelAppearance() {
			SearchPanelAppearance appearance = new SearchPanelAppearance(null);
			appearance.BackgroundStyle.Fill = SkinPainterHelper.GetSkinAlphaColorProperty(this.provider, MapSkins.PropPanelBackColor, MapSkins.PropPanelBackColorAlpha);
			return appearance;
		}
		protected override CustomOverlayAppearance CreateCustomOverlayAppearance() {
			CustomOverlayAppearance appearance = new CustomOverlayAppearance(null);
			appearance.BackgroundStyle.Fill = SkinPainterHelper.GetSkinAlphaColorProperty(this.provider, MapSkins.PropOverlayBackColor, MapSkins.PropOverlayBackColorAlpha);
			return appearance;
		}
		protected override OverlayTextAppearance CreateOverlayTextAppearance() {
			OverlayTextAppearance appearance = new OverlayTextAppearance(null);
			appearance.TextStyle.TextColor = SkinPainterHelper.GetSkinColorProperty(this.provider, MapSkins.PropOverlayTextColor);
			appearance.TextStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 9);
			return appearance;
		}
	}
	public class ViewInfoStyleProvider : ViewInfoStyleProviderBase {
		protected override NavigationPanelAppearance CreateNavPanelDefaultAppearance() {
			NavigationPanelAppearance appearance = new NavigationPanelAppearance(null);
			appearance.BackgroundStyle.Fill = BackgroundStyle.DefaultOverlayBackColor;
			appearance.ItemStyle.Fill = NavigationPanelAppearance.DefaultElementColor;
			appearance.ScaleStyle.TextColor = NavigationPanelAppearance.DefaultElementColor;
			appearance.ScaleStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 8, FontStyle.Bold);
			appearance.HotTrackedItemStyle.Fill = NavigationPanelAppearance.DefaultHotTrackColor;
			appearance.PressedItemStyle.Fill = NavigationPanelAppearance.DefaultPressedColor;
			appearance.CoordinatesStyle.TextColor = NavigationPanelAppearance.DefaultElementColor;
			appearance.CoordinatesStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 16);
			return appearance;
		}
		protected override LegendAppearance CreateLegendDefaultAppearance() {
			LegendAppearance appearance = new LegendAppearance(null);
			appearance.BackgroundStyle.Fill = BackgroundStyle.DefaultOverlayBackColor;
			appearance.ItemStyle.Fill = LegendAppearance.DefaultElementColor;
			appearance.ItemStyle.TextColor = LegendAppearance.DefaultElementColor;
			appearance.ItemStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 10);
			appearance.HeaderStyle.TextColor = LegendAppearance.DefaultElementColor;
			appearance.HeaderStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 16);
			appearance.DescriptionStyle.TextColor = LegendAppearance.DefaultElementColor;
			appearance.DescriptionStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 12);
			return appearance;
		}
		protected override ErrorPanelAppearance CreateErrorPanelAppearance() {
			ErrorPanelAppearance appearance = new ErrorPanelAppearance(null);
			appearance.BackgroundStyle.Fill = ErrorPanelAppearance.DefaultBackgroundColor;
			appearance.TextStyle.TextColor = ErrorPanelAppearance.DefaultTextColor;
			return appearance;
		}
		protected override SearchPanelAppearance CreateSearchPanelAppearance() {
			SearchPanelAppearance appearance = new SearchPanelAppearance(null);
			appearance.BackgroundStyle.Fill = BackgroundStyle.DefaultOverlayBackColor;
			return appearance;
		}
		protected override CustomOverlayAppearance CreateCustomOverlayAppearance() {
			CustomOverlayAppearance appearance = new CustomOverlayAppearance(null);
			appearance.BackgroundStyle.Fill = BackgroundStyle.DefaultOverlayBackColor;
			return appearance;
		}
		protected override OverlayTextAppearance CreateOverlayTextAppearance() {
			OverlayTextAppearance appearance = new OverlayTextAppearance(null);
			appearance.ItemStyle.Fill = BackgroundStyle.DefaultOverlayBackColor;
			appearance.HotTrackedItemStyle.Fill = BackgroundStyle.DefaultOverlayBackColor;
			appearance.TextStyle.TextColor = OverlayTextAppearance.DefaultTextColor;
			appearance.TextStyle.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 9);
			return appearance;
		}
	}
}
