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
using DevExpress.Office.PInvoke;
namespace DevExpress.Office.Utils {
	#region HdcZoomModifier
	public class HdcZoomModifier : IDisposable {
		Graphics gr;
		Win32.SIZE oldWindowExtent;
		public HdcZoomModifier(Graphics gr, float zoomFactor) {
			this.gr = gr;
			ZoomHDC(zoomFactor);
		}
		protected internal virtual void ZoomHDC(float zoomFactor) {
			IntPtr hdc = gr.GetHdc();
			try {
				Win32.SIZE size = new Win32.SIZE(0, 0);
				Win32.GetWindowExtEx(hdc, out oldWindowExtent);
				Win32.SetWindowExtEx(hdc, (int)Math.Round(oldWindowExtent.Width / zoomFactor), (int)Math.Round(oldWindowExtent.Height / zoomFactor), ref size);
			}
			finally {
				gr.ReleaseHdc();
			}
		}
		protected internal virtual void RestoreHDC() {
			IntPtr hdc = gr.GetHdc();
			try {
				Win32.SIZE size = new Win32.SIZE(0, 0);
				Win32.SetWindowExtEx(hdc, oldWindowExtent.Width, oldWindowExtent.Height, ref size);
			}
			finally {
				gr.ReleaseHdc();
			}
		}
		public void Dispose() {
			RestoreHDC();
		}
	}
	#endregion
}
