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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
namespace DevExpress.Snap.Core.Native.LayoutUI {
	public interface ISnapHotZonePainter : IDisposable {
		void WithHighQualityPixelOffsetMode(Painter painter, Action action);
		void DrawHotZone(HotZone hotZone);
	}
	public abstract class SnapHotZonePainter : ISnapHotZonePainter {
		readonly Color backgroundHoverColor = Color.FromArgb(0x00, 0xB9, 0xF2);
		readonly Brush backgroundHoverBrush;
		readonly Painter painter;
		protected SnapHotZonePainter(Painter painter) {
			Guard.ArgumentNotNull(painter, "painter");
			this.painter = painter;
			this.backgroundHoverBrush = new SolidBrush(backgroundHoverColor);
		}
		protected Painter Painter { get { return painter; } }
		protected Color BackgroundHoverColor { get { return backgroundHoverColor; } }
		protected Brush BackgroundHoverBrush { get { return backgroundHoverBrush; } }
		public void WithHighQualityPixelOffsetMode(Painter painter, Action action) {
			painter.PushPixelOffsetMode(true);
			action();
			painter.PopPixelOffsetMode();
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				BackgroundHoverBrush.Dispose(); ;
			}
		}
		~SnapHotZonePainter() {
			Dispose(false);
		}
		#endregion
		public abstract void DrawHotZone(HotZone hotZone);
	}
}
