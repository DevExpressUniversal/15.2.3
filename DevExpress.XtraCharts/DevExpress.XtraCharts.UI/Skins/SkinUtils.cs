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

using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Skins;
namespace DevExpress.XtraCharts.Native {
	public static class FillStyleSkinUtils {
		static void LoadFromSkinColor(FillStyle2D fillStyle, SkinColor skinColor) {
			Color color2 = skinColor.GetBackColor2();
			fillStyle.FillMode = ((color2.IsEmpty || color2 == skinColor.GetBackColor()) ? FillMode.Solid : FillMode.Gradient);
			bool reversed = fillStyle.FillMode == FillMode.Gradient && color2 == Color.White;
			if (fillStyle.Options != null)
				FillOptionsSkinUtils.LoadFromSkinColor(fillStyle.Options, skinColor, reversed);
		}
		static void LoadFromSkinColor(FillStyle3D fillStyle, SkinColor skinColor) {
			Color color2 = skinColor.GetBackColor2();
			fillStyle.FillMode = ((color2.IsEmpty || color2 == skinColor.GetBackColor()) ? FillMode3D.Solid : FillMode3D.Gradient);
			bool reversed = fillStyle.FillMode == FillMode3D.Gradient && color2 == Color.White;
			if (fillStyle.Options != null)
				FillOptionsSkinUtils.LoadFromSkinColor(fillStyle.Options, skinColor, reversed);
		}
		public static void LoadFromSkinColor(FillStyleBase fillStyle, SkinColor skinColor) {
			if (fillStyle is FillStyle2D)
				LoadFromSkinColor((FillStyle2D)fillStyle, skinColor);
			else if (fillStyle is FillStyle3D)
				LoadFromSkinColor((FillStyle3D)fillStyle, skinColor);
		}
	}
	public static class FillOptionsSkinUtils {
		static void LoadFromSkinColor(PolygonGradientFillOptions fillOptions, SkinColor skinColor, bool reversed) {
			fillOptions.Color2 = skinColor.GetBackColor2();
			switch (skinColor.GradientMode) {
				case LinearGradientMode.Vertical:
					fillOptions.GradientMode = reversed ? PolygonGradientMode.TopToBottom : PolygonGradientMode.BottomToTop;
					break;
				case LinearGradientMode.BackwardDiagonal:
					fillOptions.GradientMode = reversed ? PolygonGradientMode.TopRightToBottomLeft : PolygonGradientMode.BottomLeftToTopRight;
					break;
				case LinearGradientMode.ForwardDiagonal:
					fillOptions.GradientMode = reversed ? PolygonGradientMode.TopLeftToBottomRight : PolygonGradientMode.BottomRightToTopLeft;
					break;
				default:
					fillOptions.GradientMode = reversed ? PolygonGradientMode.LeftToRight : PolygonGradientMode.RightToLeft;
					break;
			}
		}
		static void LoadFromSkinColor(RectangleGradientFillOptions fillOptions, SkinColor skinColor, bool reversed) {
			fillOptions.Color2 = skinColor.GetBackColor2();
			switch (skinColor.GradientMode) {
				case LinearGradientMode.Vertical:
					fillOptions.GradientMode = reversed ? RectangleGradientMode.TopToBottom : RectangleGradientMode.BottomToTop;
					break;
				case LinearGradientMode.BackwardDiagonal:
					fillOptions.GradientMode = reversed ? RectangleGradientMode.TopRightToBottomLeft : RectangleGradientMode.BottomLeftToTopRight;
					break;
				case LinearGradientMode.ForwardDiagonal:
					fillOptions.GradientMode = reversed ? RectangleGradientMode.TopLeftToBottomRight : RectangleGradientMode.BottomRightToTopLeft;
					break;
				default:
					fillOptions.GradientMode = reversed ? RectangleGradientMode.LeftToRight : RectangleGradientMode.RightToLeft;
					break;
			}
		}
		static void LoadFromSkinColor(HatchFillOptions fillOptions, SkinColor skinColor) {
			fillOptions.Color2 = skinColor.GetBackColor2();
		}
		public static void LoadFromSkinColor(FillOptionsBase fillOptions, SkinColor skinColor, bool reversed) {
			if (fillOptions is RectangleGradientFillOptions)
				LoadFromSkinColor((RectangleGradientFillOptions)fillOptions, skinColor, reversed);
			else if (fillOptions is PolygonGradientFillOptions)
				LoadFromSkinColor((PolygonGradientFillOptions)fillOptions, skinColor, reversed);
			else if (fillOptions is HatchFillOptions)
				LoadFromSkinColor((HatchFillOptions)fillOptions, skinColor);
		}		
	}
	public static class ChartSkinUtils {
		static void LoadFromSkin(WholeChartAppearance appearance, Skin skin) {
			Color color = skin.Colors.GetColor(ChartSkins.ColorChartTitle);
			if (!color.IsEmpty)
				appearance.TitleColor = color;
			SkinElement skinElement = skin[ChartSkins.SkinBackground];
			if (skinElement != null) {
				color = skinElement.Border.GetLeft();
				if (!color.IsEmpty)
					appearance.BorderColor = color;
				color = skinElement.Color.GetBackColor();
				if (!color.IsEmpty)
					appearance.BackColor = color;
				SkinImage image = skinElement.Image;
				if (image != null) {
					BackgroundImage backImage = ChartElementFactory.CreateBackgroundImage();
					backImage.Image = image.Image;
					backImage.Stretch = image.Stretch == SkinImageStretch.Stretch;
					appearance.BackImage = backImage;
				}
			}
		}
		static void LoadFromSkin(LegendAppearance appearance, Skin skin) {
			SkinElement skinElement = skin[ChartSkins.SkinLegend];
			if (skinElement != null) {
				SkinPaddingEdges skinPadding = skinElement.ContentMargins;
				appearance.Padding.Left = skinPadding.Left;
				appearance.Padding.Top = skinPadding.Top;
				appearance.Padding.Right = skinPadding.Right;
				appearance.Padding.Bottom = skinPadding.Bottom;				
				Color color = skinElement.Color.GetForeColor();
				if (!color.IsEmpty)
					appearance.TextColor = color;
				color = skinElement.Color.GetBackColor();
				if (!color.IsEmpty)
					appearance.BackColor = color;
				FillStyleSkinUtils.LoadFromSkinColor(appearance.FillStyle, skinElement.Color);
				color = skinElement.Border.GetLeft();
				if (!color.IsEmpty)
					appearance.BorderColor = color;
				SkinImage image = skinElement.Image;
				if (image != null) {
					BackgroundImage backImage = ChartElementFactory.CreateBackgroundImage();
					backImage.Image = image.Image;
					backImage.Stretch = image.Stretch == SkinImageStretch.Stretch;
					appearance.BackImage = backImage;
				}
			}
		}
		static void LoadFromSkin(DiagramAppearance appearance, Skin skin, string diagramSkinName, string interlacedSkinName) {
			SkinElement skinElement = skin[diagramSkinName];
			if (skinElement != null) {
				Color color = skinElement.Color.GetBackColor();
				if (!color.IsEmpty)
					appearance.BackColor = color;
				FillStyleSkinUtils.LoadFromSkinColor(appearance.FillStyle, skinElement.Color);
				color = (Color)skinElement.Properties.GetColor(ChartSkins.ColorAxis);
				if (!color.IsEmpty)
					appearance.AxisColor = color;
				color = (Color)skinElement.Properties.GetColor(ChartSkins.ColorAxisLabel);
				if (!color.IsEmpty)
					appearance.LabelsColor = color;
				color = (Color)skinElement.Properties.GetColor(ChartSkins.ColorGridLines);
				if (!color.IsEmpty)
					appearance.GridLinesColor = color;
				color = (Color)skinElement.Properties.GetColor(ChartSkins.ColorMinorGridLines);
				if (!color.IsEmpty)
					appearance.MinorGridLinesColor = color;
				SkinElement interlacedSkinElement = skinElement.Owner[interlacedSkinName];
				if (interlacedSkinElement != null) {
					color = interlacedSkinElement.Color.GetBackColor();
					if (!color.IsEmpty)
						appearance.InterlacedColor = color;
					FillStyleSkinUtils.LoadFromSkinColor(appearance.InterlacedFillStyle, interlacedSkinElement.Color);
				}
			}
		}
		static void LoadFromSkin(XYDiagramAppearance appearance, Skin skin) {
			LoadFromSkin(appearance, skin, ChartSkins.SkinXYDiagram, ChartSkins.SkinXYDiagramInterlaced);
			SkinElement skinElement = skin[ChartSkins.SkinXYDiagram];
			if (skin != null) {
				Color color = skinElement.Color.GetForeColor();
				if (!color.IsEmpty)
					appearance.TextColor = color;
				color = (Color)skinElement.Properties.GetColor(ChartSkins.ColorAxisTitle);
				if (!color.IsEmpty)
					appearance.AxisTitleColor = color;
				color = (Color)skinElement.Properties.GetColor(ChartSkins.ColorPaneBounds);
				if (!color.IsEmpty)
					appearance.BorderColor = color;
			}
		}
		static void LoadFromSkin(RadarDiagramAppearance appearance, Skin skin) {
			LoadFromSkin(appearance, skin, ChartSkins.SkinRadarDiagram, ChartSkins.SkinRadarDiagramInterlaced);
		}
		static void LoadFromSkin(XYDiagram3DAppearance appearance, Skin skin) {
			LoadFromSkin(appearance, skin, ChartSkins.SkinXYDiagram3D, ChartSkins.SkinXYDiagram3DInterlaced);
		}
		static void LoadFromSkin(ConstantLineAppearance appearance, Skin skin) {
			Color lineColor = skin.Colors.GetColor(ChartSkins.ColorConstantLine);
			if (!lineColor.IsEmpty)
				appearance.Color = lineColor;
			appearance.DashStyle = DevExpress.XtraCharts.DashStyle.Solid;
			lineColor = skin.Colors.GetColor(ChartSkins.ColorConstantLineTitle);
			if (!lineColor.IsEmpty)
				appearance.TitleColor = lineColor;
		}
		static void LoadFromSkin(StripAppearance appearance, Skin skin) {
			SkinElement skinElement = skin[ChartSkins.SkinStrip];
			if (skinElement != null) {
				Color stripColor = skinElement.Color.GetBackColor();
				if (!stripColor.IsEmpty)
					appearance.Color = stripColor;
				FillStyleSkinUtils.LoadFromSkinColor(appearance.FillStyle, skinElement.Color);
			}
		}
		static void LoadFromSkin(AnnotationAppearance appearance, Skin skin, string skinName) {
			SkinElement skinElement = skin[skinName];
			if (skinElement != null) {
				Color color = skinElement.Color.GetBackColor();
				if (!color.IsEmpty)
					appearance.BackColor = color;
				color = skinElement.Border.GetLeft();
				if (!color.IsEmpty)
					appearance.BorderColor = color;
				FillStyleSkinUtils.LoadFromSkinColor(appearance.FillStyle, skinElement.Color);
			}
		}
		static void LoadFromSkin(TextAnnotationAppearance appearance, Skin skin) {
			LoadFromSkin(appearance, skin, ChartSkins.SkinTextAnnotation);
			SkinElement skinElement = skin[ChartSkins.SkinTextAnnotation];
			Color color = skinElement.Color.GetForeColor();
			if (!color.IsEmpty)
				appearance.TextColor = color;
		}
		static void LoadFromSkin(ImageAnnotationAppearance appearance, Skin skin) {
			LoadFromSkin(appearance, skin, ChartSkins.SkinImageAnnotation);
		}
		static void LoadFromSkin(BarSeriesViewAppearance appearance, Skin skin) {
			SkinElement skinElement = skin[ChartSkins.SkinBar];
			if (skinElement != null) {
				if (skinElement.Properties.ContainsProperty(ChartSkins.ShowBarBorder))
					appearance.ShowBorder = (bool)skinElement.Properties.GetBoolean(ChartSkins.ShowBarBorder);
				Color color = skinElement.Border.GetLeft();
				if (!color.IsEmpty)
					appearance.BorderColor = color;
				RectangleFillStyle fillStyle = appearance.FillStyle;
				FillStyleSkinUtils.LoadFromSkinColor(fillStyle, skinElement.Color);
				if (fillStyle.FillMode == DevExpress.XtraCharts.FillMode.Gradient)
					((GradientFillOptionsBase)fillStyle.Options).Color2 = Color.Empty;
			}
		}
		static void LoadFromSkin(AreaSeriesViewAppearance appearance, Skin skin) {
			SkinElement skinElement = skin[ChartSkins.SkinArea];
			if (skinElement != null) {
				Color color = skinElement.Border.GetLeft();
				if (!color.IsEmpty)
					appearance.BorderColor = color;
				PolygonFillStyle fillStyle = appearance.FillStyle;
				FillStyleSkinUtils.LoadFromSkinColor(fillStyle, skinElement.Color);
				if (fillStyle.FillMode == DevExpress.XtraCharts.FillMode.Gradient)
					((GradientFillOptionsBase)fillStyle.Options).Color2 = Color.Empty;
			}
		}
		static void LoadFromSkin(PieSeriesViewAppearance appearance, Skin skin) {
			SkinElement skinElement = skin[ChartSkins.SkinPie];
			if (skinElement != null) {
				Color color = skinElement.Border.GetLeft();
				if (!color.IsEmpty)
					appearance.BorderColor = color;
				PolygonFillStyle fillStyle = appearance.FillStyle;
				FillStyleSkinUtils.LoadFromSkinColor(fillStyle, skinElement.Color);
				if (fillStyle.FillMode == DevExpress.XtraCharts.FillMode.Gradient)
					((GradientFillOptionsBase)fillStyle.Options).Color2 = Color.Empty;
			}
		}
		static void LoadFromSkin(FunnelSeriesViewAppearance appearance, Skin skin) {
			SkinElement skinElement = skin[ChartSkins.SkinFunnel];
			if (skinElement != null) {
				Color color = skinElement.Border.GetLeft();
				if (!color.IsEmpty)
					appearance.BorderColor = color;
				PolygonFillStyle fillStyle = appearance.FillStyle;
				FillStyleSkinUtils.LoadFromSkinColor(fillStyle, skinElement.Color);
				if (fillStyle.FillMode == DevExpress.XtraCharts.FillMode.Gradient)
					((GradientFillOptionsBase)fillStyle.Options).Color2 = Color.Empty;
			}
		}
		static void LoadFromSkin(Bar3DSeriesViewAppearance appearance, Skin skin) {
			SkinElement skinElement = skin[ChartSkins.SkinBar3D];
			if (skinElement != null) {
				RectangleFillStyle3D fillStyle = appearance.FillStyle;
				FillStyleSkinUtils.LoadFromSkinColor(fillStyle, skinElement.Color);
			}
		}
		static void LoadFromSkin(Pie3DSeriesViewAppearance appearance, Skin skin) {
			SkinElement skinElement = skin[ChartSkins.SkinPie3D];
			if (skinElement != null) {
				PolygonFillStyle3D fillStyle = appearance.FillStyle;
				FillStyleSkinUtils.LoadFromSkinColor(fillStyle, skinElement.Color);
			}
		}
		static void LoadFromSkin(MarkerAppearance appearance, Skin skin) {
			SkinElement skinElement = skin[ChartSkins.SkinMarker];
			if (skinElement != null) {
				PolygonFillStyle fillStyle = appearance.FillStyle;
				FillStyleSkinUtils.LoadFromSkinColor(fillStyle, skinElement.Color);
				if (fillStyle.FillMode == DevExpress.XtraCharts.FillMode.Gradient)
					((GradientFillOptionsBase)fillStyle.Options).Color2 = Color.Empty;
				Color color = skinElement.Border.GetLeft();
				if (!color.IsEmpty)
					appearance.BorderColor = color;
			}
		}
		static void LoadFromSkin(SeriesLabelAppearance appearance, Skin skin, string skinName) {
			SkinElement skinElement = skin[skinName];
			if (skinElement != null) {
				Color color = skinElement.Color.GetBackColor();
				if (!color.IsEmpty)
					appearance.BackColor = color;
				color = skinElement.Color.GetForeColor();
				if (!color.IsEmpty)
					appearance.TextColor = color;
				if (skinElement.Properties.ContainsProperty(ChartSkins.ShowSeriesLabelBorder))
					appearance.ShowBorder = (bool)skinElement.Properties.GetBoolean(ChartSkins.ShowSeriesLabelBorder);
				color = skinElement.Border.GetLeft();
				if (!color.IsEmpty)
					appearance.BorderColor = color;
				if (skinElement.Properties.ContainsProperty(ChartSkins.ShowSeriesLabelConnector))
					appearance.ShowConnector = (bool)skinElement.Properties.GetBoolean(ChartSkins.ShowSeriesLabelConnector);
				color = (Color)skinElement.Properties.GetColor(ChartSkins.ColorSeriesLabelConnector);
				if (!color.IsEmpty)
					appearance.ConnectorColor = color;
			}
		}
		static void LoadFromSkin(SeriesLabel2DAppearance appearance, Skin skin) {
			LoadFromSkin(appearance, skin, ChartSkins.SkinSeriesLabel2D);
			SkinElement skinElement = skin[ChartSkins.SkinSeriesLabel2D];
			if (skinElement  != null) {
				if (skinElement.Properties.ContainsProperty(ChartSkins.ShowBubbleSeriesLabelConnector))
					appearance.ShowBubbleConnector = (bool)skinElement.Properties.GetBoolean(ChartSkins.ShowBubbleSeriesLabelConnector);
			}
		}
		static void LoadFromSkin(SeriesLabel3DAppearance appearance, Skin skin) {
			LoadFromSkin(appearance, skin, ChartSkins.SkinSeriesLabel3D);
		}
		static void LoadFromSkin(ScrollBarAppearance appearance, Skin skin) {
			SkinElement skinElement = skin[ChartSkins.SkinScrollBar];
			if (skinElement != null) {
				Color color = skinElement.Color.GetBackColor();
				if (!color.IsEmpty)
					appearance.BackColor = color;
				color = skinElement.Color.GetForeColor();
				if (!color.IsEmpty)
					appearance.BarColor = color;
				color = skinElement.Border.GetLeft();
				if (!color.IsEmpty)
					appearance.BorderColor = color;
			}
		}
		public static void LoadFromSkin(IChartAppearance appearance, ISkinProvider skinProvider) {
			appearance.ReadFromXml();
			if (skinProvider != null) {
				Skin skin = ChartSkins.GetSkin(skinProvider);
				if (skin != null) {
					LoadFromSkin(appearance.WholeChartAppearance, skin);
					LoadFromSkin(appearance.LegendAppearance, skin);
					LoadFromSkin(appearance.XYDiagramAppearance, skin);
					LoadFromSkin(appearance.RadarDiagramAppearance, skin);
					LoadFromSkin(appearance.XYDiagram3DAppearance, skin);
					LoadFromSkin(appearance.ConstantLineAppearance, skin);
					LoadFromSkin(appearance.StripAppearance, skin);
					LoadFromSkin(appearance.TextAnnotationAppearance, skin);
					LoadFromSkin(appearance.ImageAnnotationAppearance, skin);
					LoadFromSkin(appearance.BarSeriesViewAppearance, skin);
					LoadFromSkin(appearance.AreaSeriesViewAppearance, skin);
					LoadFromSkin(appearance.PieSeriesViewAppearance, skin);
					LoadFromSkin(appearance.FunnelSeriesViewAppearance, skin);
					LoadFromSkin(appearance.Bar3DSeriesViewAppearance, skin);
					LoadFromSkin(appearance.Pie3DSeriesViewAppearance, skin);
					LoadFromSkin(appearance.MarkerAppearance, skin);
					LoadFromSkin(appearance.SeriesLabel2DAppearance, skin);
					LoadFromSkin(appearance.SeriesLabel3DAppearance, skin);
					LoadFromSkin(appearance.ScrollBarAppearance, skin);
				}
			}
		}
	}
}
