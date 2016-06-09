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
using System;
namespace DevExpress.XtraCharts.Native {
	public class ChartBitmapCache : IDisposable {
		readonly Rectangle bounds;
		Bitmap image;
		Bitmap imageMiddle;
		Bitmap imageAbove;
		Graphics graphics = null;
		Graphics graphicsMiddle = null;
		Graphics graphicsAbove = null;
		public Graphics Graphics {
			get {
				if (graphics == null)
					graphics = Graphics.FromImage(image);
				return graphics;
			}
		}
		public Graphics GraphicsMiddle {
			get {
				if (graphicsMiddle == null)
					graphicsMiddle = imageMiddle != null ? Graphics.FromImage(imageMiddle) : null;
				return graphicsMiddle;
			}
		}
		public Graphics GraphicsAbove {
			get {
				if (graphicsAbove == null)
					graphicsAbove = Graphics.FromImage(imageAbove);
				return graphicsAbove;
			}
		}
		public bool IsEmpty { get { return graphicsAbove == null && graphics == null && graphicsMiddle == null; } }
		public ChartBitmapCache(Rectangle bounds, bool createMiddleImage) {
			this.bounds = bounds;
			image = new Bitmap(bounds.Width, bounds.Height);
			if (createMiddleImage)
				imageMiddle = new Bitmap(bounds.Width, bounds.Height);
			else
				imageMiddle = null;
			imageAbove = new Bitmap(bounds.Width, bounds.Height);
		}
		void DisposeGraphics(Graphics gr) {
			if (gr != null)
				gr.Dispose();
		}
		public void Dispose() {
			DisposeGraphics();
			if (image != null) {
				image.Dispose();
				image = null;
			}
			if (imageMiddle != null) {
				imageMiddle.Dispose();
				imageMiddle = null;
			}
			if (imageAbove != null) {
				imageAbove.Dispose();
				imageAbove = null;
			}
		}
		public void Render(IRenderer renderer) {
			renderer.DrawImage(image, bounds.Location);
		}
		public void RenderMiddle(IRenderer renderer) {
			renderer.DrawImage(imageMiddle, bounds.Location);
		}
		public void RenderAbove(IRenderer renderer) {
			renderer.DrawImage(imageAbove, bounds.Location);
		}
		public void DisposeGraphics() {
			DisposeGraphics(graphics);
			graphics = null;
			DisposeGraphics(graphicsMiddle);
			graphicsMiddle = null;
			DisposeGraphics(graphicsAbove);
			graphicsAbove = null;
		}
	}
}
