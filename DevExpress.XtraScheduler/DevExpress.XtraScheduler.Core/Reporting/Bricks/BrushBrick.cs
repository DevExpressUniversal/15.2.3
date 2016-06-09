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
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraScheduler.Printing {
	[BrickExporter(typeof(BrushBrickExporter))]
	public class BrushBrick : VisualBrick, IDisposable {
		Brush brush;
		public BrushBrick(Brush brush) {
			this.brush = (Brush)brush.Clone();
		}
		internal Brush Brush { get { return brush; } }
		public override void Dispose() {
			if (brush != null)
				brush.Dispose();
			brush = null;
			base.Dispose();
		}
	}
	public class BrushBrickExporter : VisualBrickExporter {
		LinearGradientBrush GetLinearGradientBrush(RectangleF rect, LinearGradientBrush brush) {
			LinearGradientBrush result = (LinearGradientBrush)brush.Clone();
			double sx = rect.Width / brush.Rectangle.Width;
			double sy = rect.Height / brush.Rectangle.Height;
			result.TranslateTransform((float)((rect.X / sx - brush.Rectangle.X)), (float)((rect.Y / sy - brush.Rectangle.Y)), MatrixOrder.Append);
			result.ScaleTransform((float)sx, (float)sy, MatrixOrder.Append);
			return result;
		}
		TextureBrush GetTextureBrush(TextureBrush brush) {
			return (TextureBrush)brush.Clone();
		}
		Brush GetBrush(RectangleF rect) {
			Brush brush = ((BrushBrick)this.Brick).Brush;
			LinearGradientBrush linearGradientBrush = brush as LinearGradientBrush;
			if (linearGradientBrush != null)
				return GetLinearGradientBrush(rect, linearGradientBrush);
			TextureBrush textureBrush = brush as TextureBrush;
			if (textureBrush != null)
				return GetTextureBrush(textureBrush);
			return (Brush)brush.Clone();
		}
		protected override void DrawObject(IGraphics gr, RectangleF rect) {
			if (rect.Width != 0 && rect.Height != 0) {
				Brush modifedBrush = GetBrush(rect);
				gr.FillRectangle(modifedBrush, rect);
				modifedBrush.Dispose();
			}
		}
		public override void Draw(Graphics gr, RectangleF rect) {
			if (rect.Width != 0 && rect.Height != 0) {
				Brush modifedBrush = GetBrush(rect);
				gr.FillRectangle(modifedBrush, rect);
				modifedBrush.Dispose();
			}
		}
	}
}
