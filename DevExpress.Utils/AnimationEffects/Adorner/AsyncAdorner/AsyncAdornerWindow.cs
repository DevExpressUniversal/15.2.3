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
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Internal;
namespace DevExpress.Utils.Animation {
	public class BaseAsyncAdornerWindow : DXLayeredWindowEx {
		BaseAsyncAdornerElementInfo element;
		protected object syncLock = new object();
		public BaseAsyncAdornerWindow(BaseAsyncAdornerElementInfo elementCore)
			: base() {
			element = elementCore;
		}
		protected override IntPtr hWndInsertAfter {
			get { return new IntPtr(0); }
		}
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case LayeredWindowMessanger.WM_CREATE:
					element.InfoArgs.BeginAsync(Handle);
					LayeredWindowMessanger.PostInvalidate(Handle);
					break;
				case LayeredWindowMessanger.WM_INVALIDATE:
					LayeredWindowMessanger.PeekAllMessage(Handle, LayeredWindowMessanger.WM_INVALIDATE);
					Invalidate();
					break;
				case LayeredWindowMessanger.WM_DESTROY:
					element.InfoArgs.EndAsync();
					NativeMethods.PostQuitMessage(0);
					break;
			}
			base.WndProc(ref m);
		}
		protected virtual void CheckWindowRegion(IEnumerable<Rectangle> regions) {
			using(Region region = new Region()) {
				region.MakeEmpty();
				foreach(Rectangle r in regions) {
					if(r.IsEmpty) continue;
					r.Offset(GetPaintOffset());
					region.Union(r);
				}
				using(Graphics g = Graphics.FromHwndInternal(Handle)) {
					NativeMethods.SetWindowRgn(Handle, region.GetHrgn(g), false);
				}
			}
		}
		protected internal BaseAsyncAdornerElementInfo Element { get { return element; } internal set { element = value; } }
		protected override void DrawCore(GraphicsCache cache) {
			lock(syncLock) {
				var rects = element.InfoArgs.CalculateRegions(true);
				ObjectPainter.DrawObject(cache, element.OpaquePainter, element.InfoArgs.InfoArgs);
				CheckWindowRegion(rects);
			}
		}
		protected override void OnDisposing() {
			lock(syncLock) {
				base.OnDisposing();
			}
		}
	}
}
