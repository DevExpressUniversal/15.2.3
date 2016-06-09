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
using System.Text;
using System.Windows.Media;
using System.Windows;
namespace DevExpress.Xpf.Charts.Native {
	[Flags]
	public enum VisualSelectionType {
		None = 0x00,
		Hatch = 0x01,
		Size = 0x02,
		Brightness = 0x04
	}
	public static class VisualSelectionHelper {
		const int SelectedLineThicknessMultiplier = 2;
		const int SelectedLegendLineThicknessIncrement = 1;
		const byte SeriesSelectionAlpha = 191;
		const byte LegendItemSelectionAlpha = 150;
		const double SelectedPointColorBrightnessPercentage = 0.6;
		const double HighlightedPointColorBrightnessPercentage = 0.7;
		static readonly Thickness selectedLegendMarkerMargin = new Thickness(-2);
		static readonly Point opacityMaskStartPoint = new Point(8, 8);
		static readonly Color customModelSelectionColor = Colors.Orange;
		static readonly Color selectionColor3D = Color.FromArgb(0x40, 0x4A, 0xCB, 0xFC);
		static readonly double selectionRectInflate = 4;
		static readonly double selectionRectCornerRadius = 4;
		public static LinearGradientBrush SelectionOpacityMask { get { return CreateItemOpacityMask(opacityMaskStartPoint); } }
		public static LinearGradientBrush InvertedSelectionOpacityMask { get { return CreateItemOpacityMask(new Point(8, -8)); } }
		public static LinearGradientBrush LegendMarkerSelectionOpacityMask { get { return CreateLegendItemOpacityMask(new Point(3, 3)); } }
		public static Thickness SelectedLegendMarkerMargin { get { return selectedLegendMarkerMargin; } }
		public static Color CustomModelSelectionColor { get { return customModelSelectionColor; } }
		public static Color SelectionColor3D { get { return selectionColor3D; } }
		public static double SelectionRectInflate { get { return selectionRectInflate; } }
		public static double SelectionRectCornerRadius { get { return selectionRectCornerRadius; } }
		static LinearGradientBrush CreateOpacityMask(Point endPoint, byte alpha) {
			LinearGradientBrush result = new LinearGradientBrush() { SpreadMethod = GradientSpreadMethod.Repeat, EndPoint = endPoint, MappingMode=BrushMappingMode.Absolute };
			result.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(alpha, 0, 0, 0), Offset = 0.5 });
			result.GradientStops.Add(new GradientStop() { Color = Colors.Black, Offset = 0.5 });
			return result;
		}
		static LinearGradientBrush CreateItemOpacityMask(Point endPoint) {
			return CreateOpacityMask(endPoint, SeriesSelectionAlpha);
		}
		static LinearGradientBrush CreateLegendItemOpacityMask(Point endPoint) {
			return CreateOpacityMask(endPoint, LegendItemSelectionAlpha);
		}
		static Color ChangeColorBrightness(Color color, double percentage) {
			byte newA = Convert.ToByte(Math.Min(color.A + 255, 255));
			byte newR = Convert.ToByte(color.R + (255 - color.R) * percentage);
			byte newG = Convert.ToByte(color.G + (255 - color.G) * percentage);
			byte newB = Convert.ToByte(color.B + (255 - color.B) * percentage);
			return Color.FromArgb(newA, newR, newG, newB);
		}
		public static LinearGradientBrush GetOpacityMask(Transform transform) {
			LinearGradientBrush mask = SelectionOpacityMask;
			if (!transform.IsIdentity())
				mask.Transform = transform;
			return mask;
		}
		public static int GetLineThickness(int thickness, bool isSelected) {
			return isSelected ? thickness * SelectedLineThicknessMultiplier : thickness;
		}
		public static Color GetSelectedPointColor(Color color) {
			return ChangeColorBrightness(color, SelectedPointColorBrightnessPercentage);
		}
		public static Color GetHighlightedPointColor(Color color) {
			return ChangeColorBrightness(color, HighlightedPointColorBrightnessPercentage);
		}
		public static int GetLegendLineThickness(int thickness, bool isSelected) {
			return isSelected ? thickness + SelectedLegendLineThicknessIncrement: thickness;
		}
		public static bool SupportsHatchSelection(VisualSelectionType selectionType) {
			return ((selectionType & VisualSelectionType.Hatch) != VisualSelectionType.None);
		}
		public static bool SupportsSizeSelection(VisualSelectionType selectionType) {
			return ((selectionType & VisualSelectionType.Size) != VisualSelectionType.None);
		}
		public static bool SupportsBrightnessSelection(VisualSelectionType selectionType) {
			return ((selectionType & VisualSelectionType.Brightness) != VisualSelectionType.None);
		}
	}
}
