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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Utils.Drawing {
	public class GraphicsInfoState : IDisposable {
		public const int InvalidDCState = 0;
		GraphicsState state;
		int dcState;
		public GraphicsInfoState(GraphicsState state, int dcState) {
			this.state = state;
			this.dcState = dcState;
		}
		public virtual void Dispose() {
			this.state = null;
			this.dcState = 0;
		}
		public GraphicsState State { get { return state; } }
		public int DCState { get { return dcState; } }
	}
	public class GraphicsClipState : IDisposable {
		Region clipRegion;
		IntPtr clipRegionAPI;
		Rectangle? savedMaximumClipBounds = null;
		public GraphicsClipState(Region clipRegion, IntPtr clipRegionAPI) : this(clipRegion, clipRegionAPI, null) { }
		public GraphicsClipState(Region clipRegion, IntPtr clipRegionAPI, Rectangle? savedMaximumClipBounds) {
			this.clipRegion = clipRegion;
			this.clipRegionAPI = clipRegionAPI;
			this.savedMaximumClipBounds = savedMaximumClipBounds;
		}
		public virtual void Dispose() {
			if(ClipRegion != null) ClipRegion.Dispose();
			if(ClipRegionAPI != IntPtr.Zero) {
				NativeMethods.DeleteObject(ClipRegionAPI);
			}
			this.clipRegion = null;
			this.clipRegionAPI = IntPtr.Zero;
		}
		public Region ClipRegion { get { return clipRegion; } }
		public IntPtr ClipRegionAPI { get { return clipRegionAPI; } }
		public Rectangle? SavedMaximumClipBounds { get { return savedMaximumClipBounds; } }
		internal void SetSavedMaximumClipBounds(Rectangle value) {
			this.savedMaximumClipBounds = value;
		}
	}
	public class GraphicsClip : IDisposable {
		bool requireAPIClipping = true;
		Rectangle maximumBounds = Rectangle.Empty;
		GraphicsCache cache;
		public GraphicsClip() {
		}
		public virtual void Dispose() {
			ReleaseGraphics();
		}
		public GraphicsCache Cache { get { return cache; } }
		public Rectangle CheckBounds(Rectangle bounds) {
			if(bounds.IsEmpty) return bounds;
			if(bounds.Y < MaximumBounds.Y) {
				bounds.Height -= (MaximumBounds.Y - bounds.Y);
				bounds.Y = MaximumBounds.Y;
			}
			if(bounds.X < MaximumBounds.X) {
				bounds.Width -= (MaximumBounds.X - bounds.X);
				bounds.X = MaximumBounds.X;
			}
			if(MaximumBounds == Rectangle.Empty) {
				return bounds;
			}
			if(bounds.Bottom > MaximumBounds.Bottom) {
				bounds.Height -= (bounds.Bottom - MaximumBounds.Bottom);
			}
			if(bounds.Right > MaximumBounds.Right) {
				bounds.Width -= (bounds.Right - MaximumBounds.Right);
			}
			return bounds;
		}
		public Rectangle MaximumBounds { get { return maximumBounds; } set { maximumBounds = value; } }
		public Graphics Graphics { get { return Cache == null ? null : Cache.Graphics; } }
		public bool RequireAPIClipping { get { return requireAPIClipping; } set { requireAPIClipping = value; } }
		public virtual void Initialize(GraphicsCache cache) {
			ReleaseGraphics();
			this.cache = cache;
			if(!RequireAPIClipping) return;
		}
		public virtual void ReleaseGraphics() {
		}
		public virtual bool IsReady { get { return Graphics != null; } }
		public GraphicsClipState SaveAndSetClip(Rectangle bounds) { return SaveAndSetClip(bounds, false); }
		public virtual GraphicsClipState SaveAndSetClip(Rectangle bounds, bool saveMaxBounds) { return SaveAndSetClip(bounds, false, saveMaxBounds); }
		public virtual GraphicsClipState SaveAndSetClip(Rectangle bounds, bool setAsMaximumBounds, bool saveMaxBounds) {
			Rectangle currentMaximumBounds = MaximumBounds;
			if(setAsMaximumBounds) MaximumBounds = bounds;
			GraphicsClipState res = SaveClip();
			SetClip(bounds);
			if(saveMaxBounds) res.SetSavedMaximumClipBounds(currentMaximumBounds);
			return res;
		}
		public GraphicsClipState SaveClip() { return SaveClip(false); }
		public virtual GraphicsClipState SaveClip(bool saveMaxBounds) {
			if(!IsReady) return null;
			Region reg = Graphics.Clip;
			IntPtr apiClip = IntPtr.Zero;
			if(RequireAPIClipping) {
				IntPtr hdc = BeginHdc(); 
				try {
					if(hdc != IntPtr.Zero) {
						apiClip = NativeMethods.CreateRectRgn(0, 0, 0, 0);
						int res = NativeMethods.GetClipRgn(hdc, apiClip);
						if(res != 1) {
							NativeMethods.DeleteObject(apiClip);
							apiClip = IntPtr.Zero;
						}
					}
				} finally {
					EndHdc(hdc);
				}
			}
			return new GraphicsClipState(reg, apiClip, saveMaxBounds ? MaximumBounds : (Rectangle?)null);
		}
		public virtual void RestoreClip(GraphicsClipState clipState) {
			if(!IsReady || clipState == null) return;
			Graphics.Clip = clipState.ClipRegion;
			if(RequireAPIClipping) {
				IntPtr hdc = BeginHdc();
				try {
					if(hdc != IntPtr.Zero) NativeMethods.SelectClipRgn(hdc, clipState.ClipRegionAPI);
				} finally {
					EndHdc(hdc);
				}
			}
			if(clipState.SavedMaximumClipBounds != null) MaximumBounds = clipState.SavedMaximumClipBounds.Value;
		}
		public void RestoreClipRelease(GraphicsClipState clipState) {
			RestoreClip(clipState);
			if(clipState != null) clipState.Dispose();
		}
		public virtual void RestoreState(GraphicsInfoState state) {
			if(state == null || !IsReady) return;
			if(state.State != null)
				Graphics.Restore(state.State);
			if(RequireAPIClipping && state.DCState != GraphicsInfoState.InvalidDCState) {
				IntPtr hdc = BeginHdc();
				try {
					if(hdc != IntPtr.Zero) NativeMethods.RestoreDC(hdc, state.DCState);
				} finally {
					EndHdc(hdc);
				}
			}
		}
		public virtual GraphicsInfoState SaveStateAPI() {
			if(!IsReady) return null;
			int dcState = GraphicsInfoState.InvalidDCState;
			if(RequireAPIClipping) {
				IntPtr hdc = BeginHdc();
				try {
					if(hdc != IntPtr.Zero) dcState = NativeMethods.SaveDC(hdc);
				} finally {
					EndHdc(hdc);
				}
			}
			return new GraphicsInfoState(null, dcState);
		}
		public virtual GraphicsInfoState SaveState() {
			if(!IsReady) return null;
			GraphicsState state = Graphics.Save();
			int dcState = GraphicsInfoState.InvalidDCState;
			if(RequireAPIClipping) {
				IntPtr hdc = BeginHdc();
				try {
					if(hdc != IntPtr.Zero) dcState = NativeMethods.SaveDC(hdc);
				} finally {
					EndHdc(hdc);
				}
			}
			return new GraphicsInfoState(state, dcState);
		}
		protected virtual void EndHdc(IntPtr hdc) {
			if(!IsReady || hdc == IntPtr.Zero) return;
			Graphics.ReleaseHdcInternal(hdc);
		}
		protected virtual IntPtr BeginHdc() {
			if(!IsReady) return IntPtr.Zero;
			try {
				return Graphics.GetHdc();
			} catch {
				return IntPtr.Zero;
			}
		}
		public virtual void SetClip(Rectangle bounds) {
			if(!IsReady) return;
			bounds = CheckBounds(bounds);
			Graphics.SetClip(bounds);
			if(RequireAPIClipping)
				APISetClip(bounds);
		}
		[ThreadStatic]
		static NativeMethods.POINT[] pointPair;
		[System.Security.SecuritySafeCritical]
		protected void APISetClip(Rectangle bounds) {
			if(bounds.Width < 1 || bounds.Height < 1) return;
			bounds = Cache.CalcClipRectangle(bounds);
			IntPtr api = IntPtr.Zero;
			IntPtr hdc = BeginHdc();
			try {
				if (pointPair == null)
					pointPair = new NativeMethods.POINT[2];
				pointPair[0] = new NativeMethods.POINT(bounds.Left, bounds.Top);
				pointPair[1] = new NativeMethods.POINT(bounds.Right, bounds.Bottom);
				NativeMethods.LPtoDP(hdc, pointPair, pointPair.Length);
				NativeMethods.POINT viewPortOrigin = new NativeMethods.POINT();
				NativeMethods.GetViewportOrgEx(hdc, ref viewPortOrigin);
				api = NativeMethods.CreateRectRgn(pointPair[0].X - viewPortOrigin.X, pointPair[0].Y - viewPortOrigin.Y, pointPair[1].X - viewPortOrigin.X, pointPair[1].Y - viewPortOrigin.Y);
				NativeMethods.ExtSelectClipRgn(hdc, api, NativeMethods.RGN_COPY);
			} finally {
				EndHdc(hdc);
			}
			NativeMethods.DeleteObject(api);
		}
		public virtual void ExcludeClip(Region region) {
			if(!IsReady || region == null) return;
			Graphics.ExcludeClip(region);
			if(RequireAPIClipping) APIExcludeClip(region);
		}
		public virtual void ExcludeClip(Rectangle bounds) {
			if(!IsReady) return;
			Graphics.ExcludeClip(bounds);
			if(RequireAPIClipping) APIExcludeClip(bounds);
		}
		protected void APIExcludeClip(Region region) {
			IntPtr apiReg = region.GetHrgn(Graphics);
			IntPtr hdc = BeginHdc();
			try {
				if(hdc != IntPtr.Zero && apiReg != IntPtr.Zero) {
					NativeMethods.ExtSelectClipRgn(hdc, apiReg, NativeMethods.RGN_XOR);
					NativeMethods.DeleteObject(apiReg);
				}
			} finally {
				EndHdc(hdc);
			}
		}
		[System.Security.SecuritySafeCritical]
		protected void APIExcludeClip(Rectangle bounds) {
			bounds = Cache.CalcClipRectangle(bounds);
			IntPtr hdc = BeginHdc();
			try {
				if(hdc != IntPtr.Zero) NativeMethods.ExcludeClipRect(hdc, bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
			} finally {
				EndHdc(hdc);
			}
		}
	}
}
