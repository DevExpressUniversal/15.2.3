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
using DevExpress.XtraGauges.Core.Base;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Drawing {
	public interface IStringPainter {
		void SetGraphics(Graphics g, Brush brush);
		Utils_StringInfo Calculate(Graphics g, BaseTextAppearance appearance, string text, Rectangle bounds);
		void DrawString(Graphics g, Utils_StringInfo info);
	}
	public class BaseShapePainter : BaseObject {
		GaugeStringPainter stringPainterCore;
		protected override void OnCreate() {
			this.stringPainterCore = new GaugeStringPainter();
		}
		protected override void OnDispose() {
			Ref.Dispose(ref stringPainterCore);
		}
		public virtual void Draw(PaintInfo info, BaseShapeCollection shapes) {
			if(info == null || shapes.Count == 0) return;
			foreach(BaseShape shape in shapes)
				DrawCore(info, shape);
		}
		public virtual void Draw(PaintInfo info, BaseShape shape) {
			if(info == null || shape.IsEmpty) return;
			DrawCore(info, shape);
		}
		protected void DrawCore(PaintInfo info, BaseShape shape) {
			shape.Render(info.Graphics, StringPainter);
		}
		GaugeStringPainter StringPainter {
			get { return stringPainterCore; }
		}
		class GaugeStringPainter : Utils_StringPainter, IStringPainter, IDisposable {
			public void SetGraphics(Graphics g, Brush brush) {
				this.StringBrush = brush;
			}
			public void Dispose() { 
				this.StringBrush = null; 
			}
			Utils_StringInfo IStringPainter.Calculate(Graphics graphics, BaseTextAppearance appearance, string text, Rectangle bounds) {
				return Calculate(new Utils_StringCalculateArgs(graphics, text, bounds, this) { Appearance = appearance });
			}
		}
	}
	public class ShapeTextPainterHelper : IDisposable {
		Graphics savedGraphics;
		GraphicsState savedState;
		public ShapeTextPainterHelper(Graphics g, BaseShape shape) {
			savedState = g.Save();
			savedGraphics = g;
			if(!shape.Transform.IsIdentity) {
				g.MultiplyTransform(shape.Transform);
			}
		}
		public void Dispose() {
			savedGraphics.Restore(savedState);
			savedGraphics = null;
			savedState = null;
		}
	}
	public class ShapeImageIndicatorPainterHelper : IDisposable {
		Graphics savedGraphics;
		GraphicsState savedState;
		public ShapeImageIndicatorPainterHelper(Graphics g, BaseShape shape) {
			savedState = g.Save();
			savedGraphics = g;
			bool Transformed = false;
			if(shape is ImageIndicatorShape && (shape as ImageIndicatorShape).ImageLayoutMode == ImageLayoutMode.Default) {
				if(g.Transform.Elements[0] > 1 && g.Transform.Elements[3] > 1) {
					Matrix defaultSizeMatrix = new Matrix(1 / g.Transform.Elements[0], 0, 0, 1 / g.Transform.Elements[3], shape.Transform.Elements[4], shape.Transform.Elements[5]);
					Matrix graphicsMatrix = g.Transform;
					graphicsMatrix.Multiply(defaultSizeMatrix);
					Matrix resultMatrix = new Matrix(1, 0, 0, 1, (float)Math.Round(graphicsMatrix.Elements[4], 0), (float)Math.Round(graphicsMatrix.Elements[5], 0));
					g.Transform = resultMatrix;
					Transformed = true;
				}
			}
			if(!shape.Transform.IsIdentity && !Transformed) {
				g.MultiplyTransform(shape.Transform);
			}
		}
		public void Dispose() {
			savedGraphics.Restore(savedState);
			savedGraphics = null;
			savedState = null;
		}
	}
}
