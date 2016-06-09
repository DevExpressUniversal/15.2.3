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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
namespace DevExpress.Xpf.Charts.Native {
	public static class ColorUtils {
		static readonly Color defaultColor = Color.FromArgb(255, 128, 128, 128);
		static bool IsNormalizedOffset(double offset) {
			return offset >= 0 && offset <= 1;
		}
		static bool IsVerticalGradientBrush(LinearGradientBrush gradientBrush) {
			return gradientBrush != null ? gradientBrush.StartPoint.X == gradientBrush.EndPoint.X : false;
		}
		static LinearGradientBrush NormalizeVerticalGradientBrush(LinearGradientBrush gradientBrush) {
			if (!IsVerticalGradientBrush(gradientBrush))
				return null;
			double gradientAxisLength = gradientBrush.EndPoint.Y - gradientBrush.StartPoint.Y;
			LinearGradientBrush noramlizedGradientBrush = new LinearGradientBrush();
			noramlizedGradientBrush.StartPoint = new Point(gradientBrush.StartPoint.X, 0);
			noramlizedGradientBrush.EndPoint = new Point(gradientBrush.EndPoint.X, 1);
			foreach (GradientStop gradientStop in gradientBrush.GradientStops) {
				double offset = gradientBrush.StartPoint.Y + gradientStop.Offset * gradientAxisLength;
				noramlizedGradientBrush.GradientStops.Add(new GradientStop() { Offset = offset, Color = gradientStop.Color });
			}
			return noramlizedGradientBrush;
		}
		public static LinearGradientBrush InterpolateVerticalGradientBrush(LinearGradientBrush gradientBrush, double startOffset, double endOffset) {
			if (gradientBrush == null || !IsVerticalGradientBrush(gradientBrush) || !IsNormalizedOffset(startOffset) || !IsNormalizedOffset(endOffset))
				return null;
			LinearGradientBrush normalizedGradientBrush = NormalizeVerticalGradientBrush(gradientBrush);
			if (normalizedGradientBrush != null) {
				normalizedGradientBrush.StartPoint = new Point(normalizedGradientBrush.StartPoint.X, normalizedGradientBrush.StartPoint.Y - startOffset / (endOffset - startOffset));
				normalizedGradientBrush.EndPoint = new Point(normalizedGradientBrush.EndPoint.X, normalizedGradientBrush.EndPoint.Y + (1 - endOffset) / (endOffset - startOffset));
			}
			return normalizedGradientBrush;
		}
		public static SolidColorBrush GetTransparentBrush(SolidColorBrush brush, double transparensy) {
			return Double.IsNaN(transparensy) ? brush :
				new SolidColorBrush(Color.FromArgb((byte)(Byte.MaxValue * (1 - transparensy)), brush.Color.R, brush.Color.G, brush.Color.B));
		}
		public static SolidColorBrush GetNotTransparentBrush(SolidColorBrush brush) {
			return new SolidColorBrush(Color.FromArgb(byte.MaxValue, brush.Color.R, brush.Color.G, brush.Color.B));
		}
		public static SolidColorBrush MixWithDefaultColor(Color targetColor) {
			return (SolidColorBrush)ColorHelper.MixColors(new SolidColorBrush(defaultColor), targetColor);
		}
		public static void MixBrushes(Model3D model, SolidColorBrush targetBrush, MaterialBrushCache brushesCache) {
			if (targetBrush == null)
				return;
			if (model is Model3DGroup) {
				Model3DGroup group = (Model3DGroup)model;
				foreach (Model3D child in group.Children)
					MixBrushes(child, targetBrush, brushesCache);
			}
			else if (model is GeometryModel3D) {
				Graphics3DUtils.SetMaterialBrush(((GeometryModel3D)model).Material, targetBrush, brushesCache, true);
			}
		}
		public static void MixBrushes(Model3D model, SolidColorBrush targetBrush) {
			MixBrushes(model, targetBrush, null);
		}
	}
}
