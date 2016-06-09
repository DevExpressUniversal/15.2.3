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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Styles;
namespace DevExpress.XtraBars.Docking.Paint {
	public class DockElementsOffice2003Painter : DockElementsPainter {
		public DockElementsOffice2003Painter(BarManagerOffice2003PaintStyle paintStyle) : base(paintStyle) { }
		protected override void CreateElementPainters() {
			fHideBarPainter = new HideBarOffice2003Painter(this);
			fTabPanelPainter = new TabPanelOffice2003Painter(this);
			fButtonPainter = new ButtonOffice2003Painter(this);
			fWindowPainter = new WindowOffice2003Painter(this);
		}
		protected override ObjectPainter CreateButtonsPanelPainter() {
			return new Docking2010.ButtonPanelOffice2003Painter();
		}
	}
	public class HideBarOffice2003Painter : HideBarPainter {
		public HideBarOffice2003Painter(DockElementsOffice2003Painter painter) : base(painter) { }
	}
	public class TabPanelOffice2003Painter : TabPanelPainter {
		public TabPanelOffice2003Painter(DockElementsOffice2003Painter painter) : base(painter) { }
		protected override void DrawTabCore(DrawTabArgs args) {
			if(args.IsActive) {
				args.DrawBackground();
				EdgesPaintHelper.DrawColorEdge(args.Cache, args.Bounds, args.Appearance.BorderColor, EdgesType.Standard,
					GetActiveTabEdgePositions(args.Position));
			}
			else if(args.TabIndex != args.ActiveChildIndex - 1) {
				DirectionRectangle dRect = new DirectionRectangle(args.Bounds, !args.IsVertical);
				DirectionRectangle r = new DirectionRectangle(dRect.GetPrevRectangle(new DirectionSize(new Size(2, dRect.Height - 6), !args.IsVertical).DirectSize, 0), args.IsVertical);
				EdgesPaintHelper.DrawColorEdge(args.Cache, r.Bounds, args.Appearance.BorderColor, EdgesType.Standard, args.IsVertical ? EdgePositions.Top : EdgePositions.Left);
				ControlPaint.DrawBorder3D(args.Graphics, r.Bounds, Border3DStyle.SunkenInner, args.IsVertical ? Border3DSide.Bottom : Border3DSide.Right);
			}
			DrawTabContent(args);
		}
	}
	public class ButtonOffice2003Painter : ButtonPainter {
		public ButtonOffice2003Painter(DockElementsOffice2003Painter painter) : base(painter) { }
		protected override void DrawButtonBounds(GraphicsCache cache, DockPanelCaptionButton args) {
			if(!IsHotOrPressed(args.ButtonState)) return;
			Brush backBrush = null;
			if((args.ButtonState & ObjectState.Pressed) != 0 && (args.ButtonState & ObjectState.Hot) != 0)
				backBrush = cache.GetGradientBrush(args.Bounds, Office2003Colors.Default[Office2003Color.Button1Pressed], Office2003Colors.Default[Office2003Color.Button2Pressed], LinearGradientMode.Vertical);
			else
				backBrush = cache.GetGradientBrush(args.Bounds, Office2003Colors.Default[Office2003Color.Button1Pressed], Office2003Colors.Default[Office2003Color.Button1Hot], LinearGradientMode.Vertical);
			cache.Graphics.FillRectangle(backBrush, args.Bounds);
			Brush borderBrush = cache.GetSolidBrush(Office2003Colors.Default[Office2003Color.Border]);
			cache.Graphics.FillRectangle(borderBrush, args.Bounds.Left, args.Bounds.Top, args.Bounds.Width, 1);
			cache.Graphics.FillRectangle(borderBrush, args.Bounds.Left, args.Bounds.Bottom - 1, args.Bounds.Width, 1);
			cache.Graphics.FillRectangle(borderBrush, args.Bounds.Left, args.Bounds.Top + 1, 1, args.Bounds.Height - 2);
			cache.Graphics.FillRectangle(borderBrush, args.Bounds.Right - 1, args.Bounds.Top + 1, 1, args.Bounds.Height - 2);
		}
	}
	public class WindowOffice2003Painter : WindowPainter {
		public WindowOffice2003Painter(DockElementsOffice2003Painter painter) : base(painter) { }
		public override void DrawBorder(DrawBorderArgs args) {
			args.BorderArgs.State = ObjectState.Hot;
			base.DrawBorder(args);
		}
		protected override void DrawInnerApplicationBorderFrame(DrawBorderArgs args, Rectangle bounds) {
			args.Paint.DrawRectangle(args.Graphics, args.BorderPen, bounds);
		}
		protected internal override int GetImageIndex(int index, ObjectState state, CaptionButtonStatus status) {
			return (index + ButtonPainter.HotIndexOffset);
		}
	}
}
