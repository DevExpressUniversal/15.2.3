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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Internal;
namespace DevExpress.Description.Controls.Windows {
	public class DXLayeredWindowAlt : DXLayeredWindow {
		public DXLayeredWindowAlt() {
			TransparencyKey = Color.Empty;
			Alpha = 255;
		}
		public override bool IsTransparent { get { return false; } }
		public void CleanUp() {
			if(IsCreated)
				DestroyHandle();
			if(buffer != null) buffer.Dispose();
			buffer = null;
		}
		public override void Invalidate() {
			if(Visible) Draw();
		}
		Bitmap buffer;
		protected virtual void Draw() {
			if(Size.Width < 1 || Size.Height < 1) return;
			using(Bitmap bitmap = CheckBuffer(Size.Width, Size.Height)) {
				using(Graphics g = Graphics.FromImage(bitmap)) {
					DrawBackgroundCore(g);
					UpdateLayered(bitmap);
				}
			}
		}
		private Bitmap CheckBuffer(int width, int height) {
			if(width < 1 || height < 1) return null;
			if(buffer == null) return new Bitmap(width, height);
			if(buffer.Width < width || buffer.Height < height) {
				buffer.Dispose();
				buffer = new Bitmap(width, height);
			}
			return buffer;
		}
		protected override void UpdateLayeredWindow() {
			Draw();
		}
		protected virtual void DrawBackgroundCore(Graphics g) {
			g.Clear(Color.Transparent);
			using(var brush = new SolidBrush(Color.FromArgb(1, Color.LightGray))) {
				g.FillRectangle(brush, new Rectangle(Point.Empty, Size));
			}
			PaintEventArgs e = new PaintEventArgs(g, Rectangle.Empty);
			ProcessPaint(e);
		}
		protected virtual void ProcessPaint(PaintEventArgs e) {
		}
		bool CheckVisible(Control control) {
			if(control.FindForm() == null) return false;
			while(control.Parent != null) {
				if(!control.Parent.Visible) return false;
				return CheckVisible(control.Parent);
			}
			return true;
		}
		protected override void DrawForeground(Graphics g) {
		}
		protected override void DrawBackground(Graphics g) {
		}
		protected override bool UseDoubleBuffer {
			get {
				return true;
			}
		}
		void UpdateLayered(Bitmap bmp) {
			if(bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) return;
			Rectangle bounds = ValidateBounds(Bounds);
			if(bounds.IsEmpty) return;
			IntPtr screenDc = NativeMethods.GetDC(IntPtr.Zero);
			IntPtr memDc = NativeMethods.CreateCompatibleDC(screenDc);
			IntPtr hBmp = bmp.GetHbitmap(Color.FromArgb(0));
			IntPtr tmp = NativeMethods.SelectObject(memDc, hBmp);
			var blendFunc = new NativeMethods.BLENDFUNCTION {
				BlendOp = 0,
				BlendFlags = 0,
				SourceConstantAlpha = 0xff,
				AlphaFormat = 1
			};
			var loc = new NativeMethods.POINT(bounds.Left, bounds.Top);   
			var size = new NativeMethods.SIZE(bounds.Width, bounds.Height);	
			var sourceLoc = new NativeMethods.POINT(0, 0);  
			bool flag = NativeMethods.UpdateLayeredWindow(
				this.Handle,
				screenDc,
				ref loc,   
				ref size,	
				memDc,
				ref sourceLoc,  
				0,
				ref blendFunc,
				2);
			NativeMethods.SelectObject(memDc, tmp);
			NativeMethods.DeleteObject(hBmp);
			NativeMethods.DeleteDC(memDc);
			NativeMethods.ReleaseDC(IntPtr.Zero, screenDc);
		}
	}
	public class DXGuideLayeredWindow : DXLayeredWindowAlt {
		GuideControl owner;
		public DXGuideLayeredWindow(GuideControl owner) {
			this.owner = owner;
		}
		public GuideControl Owner { get { return owner; } }
		protected override IntPtr InsertAfterWindow { get { return Owner.Root.Handle; } }
		protected override bool UseDoubleBuffer { get { return true; } }
		protected override void DrawBackground(Graphics g) {
		}
	}
	public class DXGuideLayeredWindowEx : DXGuideLayeredWindow {
		public DXGuideLayeredWindowEx(GuideControl owner) : base(owner) { }
		public new GuideControlEx Owner { get { return (GuideControlEx)base.Owner; } }
		protected override void DrawBackgroundCore(Graphics g) {
			using(GraphicsCache cache = new GraphicsCache(g)) {
				if(Owner.ActiveControl != null) DrawActiveControl(cache, Owner.ActiveControl);
				using(SolidBrush sb = new SolidBrush(Color.FromArgb(180, Color.Gray))) {
					foreach(var description in Owner.Descriptions) {
						if(!description.IsValidNow) continue;
						if(Owner.ActiveControl != description) {
							DrawInActiveControl(cache, description);
						}
					}
					foreach(var description in Owner.Descriptions) {
						if(!description.IsValidNow) continue;
						g.ExcludeClip(description.ControlBounds);
					}
					g.FillRectangle(sb, new Rectangle(Point.Empty, Bounds.Size));
				}
			}
		}
		void DrawInActiveControl(GraphicsCache cache, GuideControlDescription description) {
			Rectangle bounds = description.ControlBounds;
			cache.Graphics.FillRectangle(cache.GetSolidBrush(Color.FromArgb(90, Color.Gray)), bounds);
			DrawControlMarker(cache, description);
		}
		void DrawControlMarker(GraphicsCache cache, GuideControlDescription description) {
			Rectangle bounds = description.ControlBounds;
			Rectangle infoBounds = CalcInActiveInfoBounds(cache, description);
			if(infoBounds.IsEmpty) return;
			cache.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			cache.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			cache.Graphics.FillEllipse(cache.GetSolidBrush(Color.FromArgb(150, Color.Orange)), infoBounds);
			using(Pen pen = new Pen(Color.White, 1f)) {
				cache.Graphics.DrawEllipse(Pens.White, infoBounds);
				cache.Graphics.DrawEllipse(Pens.White, Rectangle.Inflate(infoBounds, -1, -1));
			}
			infoBounds.Inflate(-2, -2);
			FontFamily f = AppearanceObject.DefaultFont.FontFamily;
			int lineSpacing = f.GetLineSpacing(FontStyle.Regular);
			int EmHeight = f.GetEmHeight(FontStyle.Regular);
			float size = (infoBounds.Height * 0.7f) / (lineSpacing / (float)EmHeight);
			using(Font font = new Font(f, size)) {
				using(StringFormat sf = StringFormat.GenericTypographic.Clone() as StringFormat) {
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;
					cache.Graphics.DrawString((Owner.Descriptions.IndexOf(description) + 1).ToString(), font, Brushes.White, new RectangleF(infoBounds.X, infoBounds.Y, infoBounds.Width, infoBounds.Height), sf);
				}
			}
			cache.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
		}
		Rectangle CalcInActiveInfoBounds(GraphicsCache cache, GuideControlDescription description) {
			Rectangle bounds = description.ControlBounds;
			int height = Math.Max(16, Math.Min(50, bounds.Height - 8));
			if(height > bounds.Height) return Rectangle.Empty;
			if(height > bounds.Width) height = bounds.Width - 4;
			if(height < 16) return Rectangle.Empty;
			return DevExpress.Skins.RectangleHelper.GetCenterBounds(bounds, new Size(height, height));
		}
		const int ShadowSize = 4;
		protected void DrawActiveControl(GraphicsCache cache, GuideControlDescription description) {
			if(description.AssociatedControl == null || !description.ControlVisible) return;
			DrawControlMarker(cache, description);
			Rectangle bounds = description.ControlBounds;
			DrawIntersectedInActiveControls(cache, description);
			Rectangle highlightedBounds = Rectangle.Inflate(bounds, 4, 4);
			Rectangle client = Rectangle.Inflate(bounds, 2, 2);
			if(description.HighlightUseInsideBounds) {
				client = Rectangle.Inflate(bounds, -2, -2);
				highlightedBounds = bounds;
			}
			cache.DrawRectangle(cache.GetPen(Color.Yellow, 3), client);
			if(description.HighlightUseInsideBounds) {
				client = Rectangle.Inflate(client, 2, 2);
			}
			else {
				client = Rectangle.Inflate(client, 2, 2);
			}
			cache.DrawRectangle(cache.GetPen(Color.Black, 1), client);
			Brush shadow = cache.GetSolidBrush(Color.FromArgb(100, Color.Black));
			Rectangle shadowBounds = new Rectangle(client.Right, client.Top + ShadowSize, ShadowSize, client.Height);
			cache.Paint.FillRectangle(cache.Graphics, shadow, shadowBounds);
			cache.Graphics.ExcludeClip(shadowBounds);
			shadowBounds = new Rectangle(client.X + ShadowSize, client.Bottom, client.Width, ShadowSize);
			cache.Paint.FillRectangle(cache.Graphics, shadow, shadowBounds);
			cache.Graphics.ExcludeClip(shadowBounds);
			cache.Graphics.ExcludeClip(highlightedBounds);
		}
		void DrawIntersectedInActiveControls(GraphicsCache cache, GuideControlDescription description) {
			foreach(var d in Owner.Descriptions) {
				if(d == description || !d.IsValidNow) continue;
				if(!d.ControlBounds.IntersectsWith(description.ControlBounds)) continue;
				Rectangle bounds = CalcInActiveInfoBounds(cache, d);
				if(bounds.IntersectsWith(description.ControlBounds)) {
					DrawControlMarker(cache, d);
				}
			}
		}
	}
}
