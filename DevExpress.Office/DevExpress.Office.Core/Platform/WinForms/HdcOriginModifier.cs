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
	#region HdcOriginModifier
	public class HdcOriginModifier : IDisposable {
		public enum Mode {
			Replace,
			Combine
		}
		Graphics gr;
		Win32.POINT oldOrigin;
		public HdcOriginModifier(Graphics gr, Point newOrigin, float zoomFactor)
			: this(gr, newOrigin, zoomFactor, Mode.Replace) {
		}
		public HdcOriginModifier(Graphics gr, Point newOrigin, float zoomFactor, Mode mode) {
			this.gr = gr;
			SetHDCOrigin(newOrigin, zoomFactor, mode);
		}
		protected internal virtual void SetHDCOrigin(Point newOrigin, float zoomFactor, Mode mode) {
			IntPtr hdc = gr.GetHdc();
			try {
				Win32.POINT point = new Win32.POINT();
				Win32.GetWindowOrgEx(hdc, out oldOrigin);
				int newX = -(int)Math.Round(newOrigin.X / zoomFactor);
				int newY = -(int)Math.Round(newOrigin.Y / zoomFactor);
				if (mode == Mode.Combine) {
					newX += (int)Math.Round(oldOrigin.X / zoomFactor);
					newY += (int)Math.Round(oldOrigin.Y / zoomFactor);
				}
				Win32.SetWindowOrgEx(hdc, newX, newY, ref point);
			}
			finally {
				gr.ReleaseHdc();
			}
		}
		protected internal virtual void RestoreHDC() {
			IntPtr hdc = gr.GetHdc();
			try {
				Win32.POINT point = new Win32.POINT();
				Win32.SetWindowOrgEx(hdc, oldOrigin.X, oldOrigin.Y, ref point);
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
