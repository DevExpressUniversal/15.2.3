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
using System.Drawing.Text;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Native {
	public class AntialiasingSupport : ISupportTextAntialiasing {
		static TextRenderingHint GetAntialiasingRenderHint(bool transparentBackground, bool rotated) {
			return transparentBackground || rotated ? TextRenderingHint.AntiAlias : TextRenderingHint.SystemDefault;
		}
		static TextRenderingHint GetRenderHint(bool antialias, bool transparentBackground, bool rotated) {
			return antialias ? GetAntialiasingRenderHint(transparentBackground, rotated) : TextRenderingHint.SingleBitPerPixelGridFit;
		}
		static bool GetActualAntialiasing(DefaultBoolean enableAntialiasing, bool defaultAntialiasing, bool transparentBackground, bool rotated) {
			if (enableAntialiasing == DefaultBoolean.Default)
				return !transparentBackground || defaultAntialiasing || rotated;
			else
				return enableAntialiasing == DefaultBoolean.True;
		}
		static bool IsTransparentColor(Color color) {
			return color.A < 255;
		}
		static bool IsTransparentFill(RectangleFillStyle fillStyle) {
			FillOptionsColor2Base options = fillStyle.Options as FillOptionsColor2Base;
			return (options != null) && IsTransparentColor(options.Color2);
		}
		static bool IsTransparentBackground(ISupportTextAntialiasing antialiasingProvider) {
			bool isTransparent = IsTransparentColor(antialiasingProvider.TextBackColor) || IsTransparentFill(antialiasingProvider.TextBackFillStyle);
			ChartElement backElement = antialiasingProvider.BackElement;
			if (isTransparent && backElement != null) {
				Chart chart = CommonUtils.FindOwnerChart(backElement);
				bool is3DDiagram = chart.Diagram is Diagram3D;
				return is3DDiagram || IsTransparentColor(chart.ActualBackColor) || IsTransparentFill(chart.FillStyle);
			}
			return isTransparent;
		}
		internal static TextRenderingHint GetRenderHint(ISupportTextAntialiasing antialiasingProvider) {
			bool transparentBackground = IsTransparentBackground(antialiasingProvider);
			bool rotated = antialiasingProvider.Rotated;
			bool actualAntialiasing = GetActualAntialiasing(antialiasingProvider.EnableAntialiasing, antialiasingProvider.DefaultAntialiasing, transparentBackground, rotated);
			return GetRenderHint(actualAntialiasing, transparentBackground, rotated);
		}
		internal static bool GetAntialiasingEnabled(ISupportTextAntialiasing antialiasingProvider) {
			TextRenderingHint renderingHint = GetRenderHint(antialiasingProvider);
			return renderingHint == TextRenderingHint.AntiAlias;
		}
		readonly bool defaultAnitaliasing;
		readonly ISupportTextAntialiasing antialiasingSource;
		public AntialiasingSupport(bool defaultAnitaliasing, ISupportTextAntialiasing antialiasingSource) {
			this.defaultAnitaliasing = defaultAnitaliasing;
			this.antialiasingSource = antialiasingSource;
		}
		#region ISupportTextAntialiasing implementation
		DefaultBoolean ISupportTextAntialiasing.EnableAntialiasing { get { return DefaultBoolean.Default; } }
		bool ISupportTextAntialiasing.DefaultAntialiasing { get { return defaultAnitaliasing; } }
		bool ISupportTextAntialiasing.Rotated { get { return antialiasingSource.Rotated; } }
		Color ISupportTextAntialiasing.TextBackColor { get { return antialiasingSource.TextBackColor; } }
		RectangleFillStyle ISupportTextAntialiasing.TextBackFillStyle { get { return antialiasingSource.TextBackFillStyle; } }
		ChartElement ISupportTextAntialiasing.BackElement { get { return antialiasingSource.BackElement; } }
		#endregion
	}
}
