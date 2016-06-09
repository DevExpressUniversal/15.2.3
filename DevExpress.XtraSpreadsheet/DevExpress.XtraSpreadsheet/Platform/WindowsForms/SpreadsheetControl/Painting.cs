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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl {
		bool isPainted; 
		SpreadsheetControlPainter painter;
		SpreadsheetViewPainter viewPainter;
		SpreadsheetViewBackgroundPainter backgroundPainter;
		protected internal bool IsPainted { get { return isPainted; } }
		protected internal SpreadsheetControlPainter Painter { get { return painter; } }
		protected internal SpreadsheetViewPainter ViewPainter { get { return viewPainter; } }
		protected internal SpreadsheetViewBackgroundPainter BackgroundPainter { get { return backgroundPainter; } }
		protected override void OnPaint(PaintEventArgs e) {
			if (IsUpdateLocked && !IsMessageBoxShown)
				return;
			InnerControl.OnBeginPaint();
			try {
				isPainted = true;
				Painter.Draw(e.Graphics );
				base.OnPaint(e);
			}
			finally {
				InnerControl.OnEndPaint();
			}
		}
		protected override void OnPaintBackground(PaintEventArgs pevent) {
			using (GraphicsCache cache = new GraphicsCache(pevent.Graphics)) {
				BackgroundPainter.Draw(cache, BackgroundBounds);
			}
		}
		protected internal virtual SpreadsheetControlPainter CreatePainter() {
			return new SpreadsheetControlPainter(this);
		}
		protected internal virtual void Redraw() {
			if (IsUpdateLocked)
				ControlDeferredChanges.Redraw = true;
			else
				CustomRefresh();
		}
		protected internal virtual void CustomRefresh() {
			Refresh();
		}
		#region Background Painter
		protected internal virtual SpreadsheetViewBackgroundPainterFactory CreateViewBackgroundPainterFactory() {
			switch (LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Flat:
					return new SpreadsheetViewBackgroundPainterFlatFactory();
				case ActiveLookAndFeelStyle.UltraFlat:
					return new SpreadsheetViewBackgroundPainterUltraFlatFactory();
				case ActiveLookAndFeelStyle.Style3D:
					return new SpreadsheetViewBackgroundPainterStyle3DFactory();
				case ActiveLookAndFeelStyle.Office2003:
					return new SpreadsheetViewBackgroundPainterOffice2003Factory();
				case ActiveLookAndFeelStyle.WindowsXP:
					return new SpreadsheetViewBackgroundPainterWindowsXPFactory();
				case ActiveLookAndFeelStyle.Skin:
				default:
					return new SpreadsheetViewBackgroundPainterSkinFactory();
			}
		}
		protected internal virtual void CreateBackgroundPainter(SpreadsheetView view) {
			SpreadsheetViewBackgroundPainterFactory factory = CreateViewBackgroundPainterFactory();
			this.backgroundPainter = factory.CreatePainter(view);
		}
		protected internal virtual void DisposeBackgroundPainter() {
			if (backgroundPainter != null) {
				this.backgroundPainter.Dispose();
				this.backgroundPainter = null;
			}
		}
		protected internal virtual void RecreateBackgroundPainter(SpreadsheetView view) {
			DisposeBackgroundPainter();
			CreateBackgroundPainter(view);
		}
		#endregion
		#region View Painter
		protected internal virtual SpreadsheetViewPainterFactory CreateViewPainterFactory() {
			switch (LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Flat:
					return new SpreadsheetViewPainterFlatFactory();
				case ActiveLookAndFeelStyle.UltraFlat:
					return new SpreadsheetViewPainterUltraFlatFactory();
				case ActiveLookAndFeelStyle.Style3D:
					return new SpreadsheetViewPainterStyle3DFactory();
				case ActiveLookAndFeelStyle.Office2003:
					return new SpreadsheetViewPainterOffice2003Factory();
				case ActiveLookAndFeelStyle.WindowsXP:
					return new SpreadsheetViewPainterWindowsXPFactory();
				case ActiveLookAndFeelStyle.Skin:
				default:
					return new SpreadsheetViewPainterSkinFactory();
			}
		}
		protected internal virtual void CreateViewPainter(SpreadsheetView view) {
			SpreadsheetViewPainterFactory factory = CreateViewPainterFactory();
			this.viewPainter = factory.CreatePainter(view);
		}
		protected internal virtual void DisposeViewPainter() {
			if (viewPainter != null) {
				this.viewPainter.Dispose();
				this.viewPainter = null;
			}
		}
		protected internal virtual void RecreateViewPainter(SpreadsheetView view) {
			DisposeViewPainter();
			CreateViewPainter(view);
		}
		#endregion
	}
}
