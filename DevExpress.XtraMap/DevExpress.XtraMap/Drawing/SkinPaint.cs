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

using DevExpress.Skins;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraMap.Drawing {
	public class MapDefaultSkinProvider : ISkinProvider {
		static readonly MapDefaultSkinProvider instance = new MapDefaultSkinProvider();
		public static MapDefaultSkinProvider Default { get { return instance; } }
		#region ISkinProvider Members
		string ISkinProvider.SkinName {
			get { return "DevExpress Style"; }
		}
		#endregion
	}
	public static class SkinPainterHelper {
		#region Nested class
		class TextOriginDpiProvider : DpiProvider {
			static TextOriginDpiProvider instance = new TextOriginDpiProvider();
			public static TextOriginDpiProvider Instance { get { return instance; } }
			TextOriginDpiProvider() { }
			protected override float GetDpiScale() {
				return 1.0f;
			}
		}
		#endregion
		static Color DefaultPushpinNormalColor = Color.FromArgb(136, 102, 174);
		static Color DefaultPushpinHighlightedColor = Color.FromArgb(5, 87, 199);
		static Color DefaultPushpinSelectedColor = Color.FromArgb(255, 196, 8);
		static SkinPainterHelper() {
		}
		static void UpdateTextOrigin(PushpinStyle pushpinStyle, ISkinProvider skinProvider) {
			Skin skin = MapSkins.GetSkin(skinProvider);
			IDpiProvider tempProvider = skin.DpiProvider;
			skin.DpiProvider = TextOriginDpiProvider.Instance;
			pushpinStyle.TextOrigin = new Point(
				SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinPushpin, MapSkins.PropTextOriginX),
				SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinPushpin, MapSkins.PropTextOriginY));
			skin.DpiProvider = tempProvider;
		}
		static Image ChangeImageBrightness(Image originalImage, float brightnessFactor) {
			Bitmap bmp = originalImage.Clone() as Bitmap;
			for(int x = 0; x < bmp.Width; x++)
				for(int y = 0; y < bmp.Height; y++) {
					Color cur = bmp.GetPixel(x, y);
					int R = Math.Min((int)(cur.R * brightnessFactor), 255);
					int G = Math.Min((int)(cur.G * brightnessFactor), 255);
					int B = Math.Min((int)(cur.B * brightnessFactor), 255);
					Color newColor = Color.FromArgb(cur.A, R, G, B);
					bmp.SetPixel(x, y, newColor);
				}
			return bmp;
		}
		public static SkinElementInfo UpdateObjectInfoArgs(ISkinProvider skinProvider, string skinElementName) {
			return UpdateObjectInfoArgs(skinProvider, skinElementName, Rectangle.Empty);
		}
		public static SkinElementInfo UpdateObjectInfoArgs(ISkinProvider skinProvider, string skinElementName, Rectangle bounds) {
			SkinElement skinEl = GetSkinElement(skinProvider, skinElementName);
			return new SkinElementInfo(skinEl, bounds);
		}
		public static SkinElement GetSkinElement(ISkinProvider skinProvider, string skinElementName) {
			Skin skin = MapSkins.GetSkin(skinProvider);
			return skin != null ? skin[skinElementName] : null;
		}
		public static Color GetSkinForeColor(ISkinProvider skinProvider, string skinElementName) {
			SkinElement skinEl = GetSkinElement(skinProvider, skinElementName);
			return skinEl != null && skinEl.Color != null ? skinEl.Color.GetForeColor() : Color.Empty;
		}
		public static Color GetSkinBackColor(ISkinProvider skinProvider, string skinElementName) {
			SkinElement skinEl = GetSkinElement(skinProvider, skinElementName);
			return skinEl != null && skinEl.Color != null ? skinEl.Color.GetBackColor() : Color.Empty;
		}
		public static Color GetPredefinedColorizerColor(ISkinProvider skinProvider, string colorPropertyName) {
			Skin skin = MapSkins.GetSkin(skinProvider);
			return skin != null && skin.Colors != null ? skin.Colors.GetColor(colorPropertyName) : Color.Empty;
		}
		public static Color GetSkinAlphaColorProperty(ISkinProvider skinProvider, string colorName, string alphaName) {
			Color c = GetSkinColorProperty(skinProvider, colorName);
			int alpha = GetSkinIntegerProperty(skinProvider, alphaName);
			return Color.FromArgb(alpha, c.R, c.G, c.B);
		}
		public static Color GetSkinColorProperty(ISkinProvider skinProvider, string name) {
			return GetSkinColorProperty(skinProvider, name, Color.Empty);
		}
		public static Color GetSkinColorProperty(ISkinProvider skinProvider, string name, Color defaultColor) {
			Skin skin = MapSkins.GetSkin(skinProvider);
			return skin != null && skin.Properties != null ? skin.Properties.GetColor(name, defaultColor) : defaultColor;
		}
		public static int GetSkinIntegerProperty(ISkinProvider skinProvider, string name) {
			Skin skin = MapSkins.GetSkin(skinProvider);
			return skin != null && skin.Properties != null ? skin.Properties.GetInteger(name) : 0;
		}
		public static Color GetSkinElementColor(ISkinProvider skinProvider, string element, string colorName, Color defaultColor) {
			SkinElement skinEl = GetSkinElement(skinProvider, element);
			return skinEl != null && skinEl.Properties != null ? skinEl.Properties.GetColor(colorName, defaultColor) : defaultColor;
		}
		public static Color GetSkinElementAlphaColor(ISkinProvider skinProvider, string element, string colorName, string alphaName) {
			Color c = GetSkinElementColor(skinProvider, element, colorName, Color.Empty);
			int alpha = GetSkinElementInteger(skinProvider, element, alphaName);
			return Color.FromArgb(alpha, c);
		}
		public static int GetSkinElementInteger(ISkinProvider skinProvider, string element, string alphaName) {
			SkinElement skinEl = GetSkinElement(skinProvider, element);
			return skinEl != null && skinEl.Properties != null ? skinEl.Properties.GetInteger(alphaName) : 0;
		}
		public static Padding GetContentPadding(ISkinProvider skinProvider, string element) {
			SkinElement skinEl = GetSkinElement(skinProvider, element);
			SkinPaddingEdges padding = skinEl != null ? skinEl.ContentMargins : new SkinPaddingEdges();
			return new Padding(padding.Left, padding.Top, padding.Right, padding.Bottom);
		}
		public static SkinPaddingEdges GetSkinPaddingEdges(ISkinProvider skinProvider, string element, string name) {
			SkinElement skinEl = GetSkinElement(skinProvider, element);
			return skinEl != null ? skinEl.Properties.GetPadding(name, new SkinPaddingEdges()) : new SkinPaddingEdges();
		}
		static Image GetImageFromSkinElement(SkinElement element) {
			if(element != null) {
				SkinImage image = element.Image;
				return image != null ? image.Image : null;
			}
			return null;
		}
		public static Image GetElementImage(ISkinProvider skinProvider, string elementName) {
			SkinElement skinEl = GetSkinElement(skinProvider, elementName);
			return GetImageFromSkinElement(skinEl);
		}
		public static List<Image> GetPushpinImages(ISkinProvider skinProvider) {
			SkinElement skinEl = GetSkinElement(skinProvider, MapSkins.SkinPushpin);
			if(skinEl == null)
				skinEl = GetSkinElement(MapDefaultSkinProvider.Default, MapSkins.SkinPushpin);
			if(skinEl != null && skinEl.Image != null && skinEl.Image.Image != null) {
				Image sourceImage = skinEl.Image.Image;
				List<Image> newImages = new List<Image>();
				newImages.Add(sourceImage.Clone() as Image);
				newImages.Add(ChangeImageBrightness(sourceImage, 1.1f));
				newImages.Add(ChangeImageBrightness(sourceImage, 0.9f));
				return newImages;
			}
			return new List<Image>() { MapUtils.DefaultPushpinImage, MapUtils.DefaultHighlightedPushpinImage, MapUtils.DefaultSelectedPushpinImage };  
		}
		public static void UpdateTextStyle(MapItemTextStyle style, ISkinProvider skinProvider, string elementName) {
			style.TextColor = GetSkinElementColor(skinProvider, elementName, MapSkins.PropTextColor, MapItemStyleProvider.DefaultTextColor);
			style.TextGlowColor = GetSkinElementColor(skinProvider, elementName, MapSkins.PropTextGlowColor, MapItemStyleProvider.DefaultTextGlowColor);
		}
		public static void UpdateShapeTitleStyle(MapItemTextStyle style, ISkinProvider skinProvider) {
			UpdateTextStyle(style, skinProvider, MapSkins.SkinCustomElement);
		}
		public static void UpdateShapeStyle(MapItemStyle shapeStyle, ISkinProvider skinProvider) {
			UpdateShapeTitleStyle(shapeStyle, skinProvider);
			shapeStyle.Fill = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinShape, MapSkins.PropElementBackColor, Color.Empty);
			shapeStyle.Stroke = SkinPainterHelper.GetSkinElementAlphaColor(skinProvider, MapSkins.SkinShape, MapSkins.PropElementBorderColor, MapSkins.PropElementBorderColorAlpha);
			shapeStyle.StrokeWidth = SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinShape, MapSkins.PropBorderWidth);
		}
		public static void UpdateSelectedShapeStyle(MapItemStyle selectedShapeStyle, ISkinProvider skinProvider) {
			selectedShapeStyle.Fill = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinShape, MapSkins.PropElementSelectedColor, Color.Empty);
			selectedShapeStyle.Stroke = SkinPainterHelper.GetSkinElementAlphaColor(skinProvider, MapSkins.SkinShape, MapSkins.PropSelectedBorderColor, MapSkins.PropSelectedBorderColorAlpha);
			selectedShapeStyle.StrokeWidth = SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinShape, MapSkins.PropSelectedBorderWidth);
		}
		public static void UpdateHighlightedShapeStyle(MapItemStyle highlightedShapeStyle, ISkinProvider skinProvider) {
			highlightedShapeStyle.Fill = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinShape, MapSkins.PropElementHighlightedColor, Color.Empty);
			highlightedShapeStyle.Stroke = SkinPainterHelper.GetSkinElementAlphaColor(skinProvider, MapSkins.SkinShape, MapSkins.PropHighlightedBorderColor, MapSkins.PropHighlightedBorderColorAlpha);
			highlightedShapeStyle.StrokeWidth = SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinShape, MapSkins.PropHighlightedBorderWidth);
		}
		public static void UpdateCustomElementStyle(MapPointerStyle style, ISkinProvider skinProvider) {
			UpdateTextStyle(style, skinProvider, MapSkins.SkinCustomElement);
			style.ContentPadding = SkinPainterHelper.GetContentPadding(skinProvider, MapSkins.SkinCustomElement);
			style.BackgroundElement = SkinPainterHelper.GetSkinElement(skinProvider, MapSkins.SkinCustomElement);
			style.BaseColor = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinCustomElement, MapSkins.PropBaseColor, MapPointerStyle.DefaultBaseColor);
			style.HighlightedBaseColor = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinCustomElement, MapSkins.PropHighlightedBaseColor, MapCalloutStyle.DefaultBaseColor);
			style.SelectedBaseColor = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinCustomElement, MapSkins.PropSelectedBaseColor, MapCalloutStyle.DefaultBaseColor);
			style.HighlightedEffectiveArea = SkinPainterHelper.GetSkinPaddingEdges(skinProvider, MapSkins.SkinCustomElement, MapSkins.PropHighlightingEffectiveArea);
		}
		public static void UpdateLineStyle(MapItemStyle lineStyle, ISkinProvider skinProvider) {
			UpdateShapeTitleStyle(lineStyle, skinProvider);
			lineStyle.Stroke = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinShape, MapSkins.PropElementBackColor, Color.Empty);
			lineStyle.StrokeWidth = SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinShape, MapSkins.PropBorderWidth);
		}
		public static void UpdatePolylineStyle(MapItemStyle polylineSegmentStyle, ISkinProvider skinProvider) {
			UpdateShapeTitleStyle(polylineSegmentStyle, skinProvider);
			polylineSegmentStyle.Stroke = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinShape, MapSkins.PropElementBackColor, Color.Empty);
			polylineSegmentStyle.StrokeWidth = SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinShape, MapSkins.PropBorderWidth);
		}
		public static void UpdatePolygonSegmentStyle(MapItemStyle polygonSegmentStyle, ISkinProvider skinProvider) {
			UpdateShapeTitleStyle(polygonSegmentStyle, skinProvider);
			polygonSegmentStyle.Fill = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinShape, MapSkins.PropElementBackColor, Color.Empty);
			polygonSegmentStyle.Stroke = SkinPainterHelper.GetSkinElementAlphaColor(skinProvider, MapSkins.SkinShape, MapSkins.PropElementBorderColor, MapSkins.PropElementBorderColorAlpha);
			polygonSegmentStyle.StrokeWidth = SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinShape, MapSkins.PropBorderWidth);
		}
		public static void UpdatePushpinStyle(PushpinStyle pushpinStyle, ISkinProvider skinProvider) {
			UpdateTextStyle(pushpinStyle, skinProvider, MapSkins.SkinPushpin);
			List<Image> images = SkinPainterHelper.GetPushpinImages(skinProvider);
			pushpinStyle.Image = images[0];
			pushpinStyle.HighlightedImage = images[1];
			pushpinStyle.SelectedImage = images[2];
			UpdateTextOrigin(pushpinStyle, skinProvider);
			pushpinStyle.ImageOrigin = PushpinPainter.DefaultSkinImageOrigin;
		}	  
		public static void UpdateSelectedRegionStyle(BorderedElementStyle selectedRegionStyle, ISkinProvider skinProvider) {
			selectedRegionStyle.Fill = SkinPainterHelper.GetSkinElementAlphaColor(skinProvider, MapSkins.SkinSelectedRegion, MapSkins.PropElementBackColor, MapSkins.PropElementBackColorAlpha);
			selectedRegionStyle.Stroke = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinSelectedRegion, MapSkins.PropElementBorderColor, Color.Empty);
		}
		public static void UpdateCalloutStyle(MapCalloutStyle style, ISkinProvider skinProvider) {
			UpdateTextStyle(style, skinProvider, MapSkins.SkinCallout);
			style.TextOrigin = new Point(
				SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinCallout, MapSkins.PropTextOriginX),
				SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinCallout, MapSkins.PropTextOriginY));
			style.ContentPadding = SkinPainterHelper.GetContentPadding(skinProvider, MapSkins.SkinCallout);
			style.BaseOffset = new Point(
			   SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinCallout, MapSkins.PropPointerX),
			   SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinCallout, MapSkins.PropPointerY));
			style.PointerSize = new Size(0,
				SkinPainterHelper.GetSkinElementInteger(skinProvider, MapSkins.SkinCallout, MapSkins.PropPointerHeight));
			style.BackgroundElement = SkinPainterHelper.GetSkinElement(skinProvider, MapSkins.SkinCallout);
			style.BaseColor = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinCallout, MapSkins.PropBaseColor, MapCalloutStyle.DefaultBaseColor);
			style.HighlightedBaseColor = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinCallout, MapSkins.PropHighlightedBaseColor, MapCalloutStyle.DefaultBaseColor);
			style.SelectedBaseColor = SkinPainterHelper.GetSkinElementColor(skinProvider, MapSkins.SkinCallout, MapSkins.PropSelectedBaseColor, MapCalloutStyle.DefaultBaseColor);
			style.HighlightedEffectiveArea = SkinPainterHelper.GetSkinPaddingEdges(skinProvider, MapSkins.SkinCallout, MapSkins.PropHighlightingEffectiveArea);
		}
		public static void UpdateMapBorderStyle(MapBorderStyle style, ISkinProvider skinProvider) {
			SkinElement borderElement = CommonSkins.GetSkin(skinProvider)[CommonSkins.SkinTextBorder];
			style.BorderElement = borderElement;
		}
	}
}
