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
using System.Drawing.Text;
using System.Runtime.InteropServices;
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {
	public class NativeFont : IDisposable {
		System.Drawing.Font gdiPlusFont;
		bool isDisposed = false;
		public System.Drawing.Font GdiPlusFont { get { return gdiPlusFont; } }
		internal bool IsDisposed { get { return isDisposed; } }
		public NativeFont(System.Drawing.Font gdiPlusFont) {
			this.gdiPlusFont = gdiPlusFont;
		}
		public virtual void Dispose() {
			this.isDisposed = true;
		}
		protected internal void SetGdiPlusFont(System.Drawing.Font gdiPlusFont) {
			this.gdiPlusFont = gdiPlusFont;
		}
	}
	public class NativeFontDisposable : NativeFont {
		public NativeFontDisposable(System.Drawing.Font gdiPlusFont) : base(gdiPlusFont) {
		}
		public NativeFontDisposable(FontFamily family, float emSize) : base(null) {
			SetGdiPlusFont(new Font(family, emSize));
		}
		public override void Dispose() {
			if(GdiPlusFont != null) {
				GdiPlusFont.Dispose();
				SetGdiPlusFont(null);
			}
			base.Dispose();
		}
	}
}
