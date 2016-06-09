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
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Office.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Office.PInvoke;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Drawing {
	public delegate void DrawDelegate(GraphicsCache cache);
	#region SpreadsheetControlPainter
	public class SpreadsheetControlPainter : IDisposable {
		readonly SpreadsheetControl control;
		readonly List<DrawDelegate> deferredDraws = new List<DrawDelegate>();
		public SpreadsheetControlPainter(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		public SpreadsheetControl Control { get { return control; } }
		public UserLookAndFeel LookAndFeel { get { return Control.LookAndFeel; } }
		protected internal virtual SpreadsheetView ActiveView { get { return Control.ActiveView; } }
		#endregion
		public virtual void Draw(Graphics gr) {
			if (Control.InnerControl == null)
				return;
			using (GraphicsCache cache = new GraphicsCache(gr)) {
				try {
					GraphicsClipState clipState = cache.ClipInfo.SaveAndSetClip(Control.ClientBounds);
					try {
						PerformRenderingInUnits(cache, RenderDocument);
						int count = deferredDraws.Count;
						if (count > 0) {
							for (int i = 0; i < count; i++) {
								PerformRenderingInUnits(cache, deferredDraws[i]);
							}
							deferredDraws.Clear();
						}
					}
					finally {
						cache.ClipInfo.RestoreClipRelease(clipState);
					}
				}
				finally {
				}
				Color backGroundCollor = LookAndFeelHelper.GetSystemColor(Control.LookAndFeel, SystemColors.Control);
				cache.FillRectangle(backGroundCollor, control.CornerBounds);
				if (control.RightHeaderBounds.Top > 1) {
					Rectangle uppeRightCornerBounds = new Rectangle(control.RightHeaderBounds.X, 0, control.RightHeaderBounds.Width, control.RightHeaderBounds.Top);
					cache.FillRectangle(backGroundCollor, uppeRightCornerBounds);
				}
				Control.ViewPainter.HeaderPainter.DrawRightHeader(cache, control.RightHeaderBounds);
				BorderPainter borderPainter = BorderHelper.GetGridPainter(Control.BorderStyle, Control.LookAndFeel);
				borderPainter.DrawObject(new BorderObjectInfoArgs(cache, Control.ClientRectangle, null, ObjectState.Normal));
			}
		}
		public void RegisterDeferredDraw(DrawDelegate draw) {
			deferredDraws.Add(draw);
		}
		public void DeferredDraw(Page page, DrawAtPageDelegate drawAtPage) {
			DrawDelegate draw = delegate(GraphicsCache cache) {
				Control.ViewPainter.DrawAtPageCore(cache, page, drawAtPage);
			};
			RegisterDeferredDraw(draw);
		}
		public void DrawReversibleHorizontalLine(int y) {
			DrawDelegate draw = delegate(GraphicsCache graphicsCache) {
				DrawReversibleHorizontalLineCore(graphicsCache, y);
			};
			DrawReversibleCore(draw);
		}
		public void DrawReversibleVerticalLine(int x) {
			DrawDelegate draw = delegate(GraphicsCache graphicsCache) {
				DrawReversibleVerticalLineCore(graphicsCache, x);
			};
			DrawReversibleCore(draw);
		}
		public void DrawReversibleFrame(Rectangle bounds) {
			DrawDelegate draw = delegate(GraphicsCache graphicsCache) {
				DrawReversibleFrameCore(graphicsCache, bounds);
			};
			DrawReversibleCore(draw);
		}
		void DrawReversibleCore(DrawDelegate draw) {
			IntPtr hdc = Win32.GetWindowDC(Control.Handle);
			try {
				using (Graphics gr = Graphics.FromHdc(hdc)) {
					using (GraphicsCache cache = new GraphicsCache(gr)) {
						GraphicsClipState clipState = cache.ClipInfo.SaveAndSetClip(Control.ClientBounds);
						try {
							PerformRenderingInUnits(cache, draw);
						}
						finally {
							cache.ClipInfo.RestoreClipRelease(clipState);
						}
					}
				}
			}
			finally {
				Win32.ReleaseDC(Control.Handle, hdc);
			}
		}
		internal virtual void PerformRenderingInUnits(GraphicsCache cache, DrawDelegate draw) {
			Graphics gr = cache.Graphics;
			using (GraphicsToLayoutUnitsModifier modifier = new GraphicsToLayoutUnitsModifier(gr, Control.DocumentModel.LayoutUnitConverter)) {
				Rectangle viewBounds = Control.ViewBounds;
				gr.TranslateTransform(viewBounds.Left, viewBounds.Top);
				using (HdcOriginModifier hdcOriginModifier = new HdcOriginModifier(gr, viewBounds.Location, 1.0f)) {
					draw(cache);
				}
			}
		}
		protected internal virtual void RenderDocument(GraphicsCache cache) {
			ActiveView.SelectionLayout.Update();
			HeaderPage headerPage = Control.InnerControl.DesignDocumentLayout.HeaderPage;
			if (headerPage != null)
				headerPage.Update();
			Control.ViewPainter.Draw(cache);
		}
		protected internal virtual void DrawReversibleHorizontalLineCore(GraphicsCache cache, int y) {
			Rectangle rect = GetActualBounds(Control.ClientBounds);
			Control.ViewPainter.DrawReversibleHorizontalLineAtPage(cache, y, rect);
		}
		protected internal virtual void DrawReversibleVerticalLineCore(GraphicsCache cache, int x) {
			Rectangle rect = GetActualBounds(Control.ClientBounds);
			Control.ViewPainter.DrawReversibleVerticalLineAtPage(cache, x, rect);
		}
		Rectangle GetActualBounds(Rectangle originalBounds) {
			originalBounds = control.DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(originalBounds, DocumentModel.DpiX, DocumentModel.DpiY);
			float zoomFactor = control.ActiveView.ZoomFactor;
			if (zoomFactor == 1.0f)
				return originalBounds;
			int x = originalBounds.X;
			int y = originalBounds.Y;
			int width = ApplyZoomFactor(originalBounds.Width, zoomFactor);
			int height = ApplyZoomFactor(originalBounds.Height, zoomFactor);
			return new Rectangle(x, y, width, height);
		}
		int ApplyZoomFactor(float value, float zoomFactor) {
			return (int)Math.Round(value * 1 / zoomFactor);
		}
		protected internal virtual void DrawReversibleFrameCore(GraphicsCache cache, Rectangle bounds) {
			Control.ViewPainter.DrawReversibleFrameAtPage(cache, bounds);
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region EmptySpreadsheetControlPainter
	public class EmptySpreadsheetControlPainter : SpreadsheetControlPainter {
		public EmptySpreadsheetControlPainter(SpreadsheetControl control)
			: base(control) {
		}
		public override void Draw(Graphics gr) {
		}
	}
	#endregion
}
