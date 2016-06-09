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
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Docking.Paint {
	public class DockElementsOffice2000Painter : DockElementsPainter {
		public DockElementsOffice2000Painter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		protected override void CreateElementPainters() {
			fHideBarPainter = new HideBarOffice2000Painter(this);
			fTabPanelPainter = new TabPanelOffice2000Painter(this);
			fButtonPainter = new ButtonOffice2000Painter(this);
			fWindowPainter = new WindowOffice2000Painter(this);
		}
		protected override ObjectPainter CreateButtonsPanelPainter() {
			return new Docking2010.ButtonPanelOffice2000Painter();
		}
	}
	public class HideBarOffice2000Painter : HideBarPainter {
		public HideBarOffice2000Painter(DockElementsOffice2000Painter painter) : base(painter) { }
	}
	public class TabPanelOffice2000Painter : TabPanelPainter {
		public TabPanelOffice2000Painter(DockElementsOffice2000Painter painter) : base(painter) { }
		protected override void DrawTabCore(DrawTabArgs args) {
			if(args.IsActive) {
				args.DrawBackground();
				EdgesPaintHelper.DrawColorEdgeCore(args.Graphics, args.Bounds, Pens.White, Pens.Black, GetActiveTabEdgePositions(args.Position));
			}
			else if(args.TabIndex != args.ActiveChildIndex - 1) {
				if(args.IsVertical)
					args.Graphics.DrawLine(args.Appearance.GetForePen(args.Cache), args.Bounds.Left + 4, args.Bounds.Bottom, args.Bounds.Right - 4, args.Bounds.Bottom);
				else
					args.Graphics.DrawLine(args.Appearance.GetForePen(args.Cache), args.Bounds.Right, args.Bounds.Top + 4, args.Bounds.Right, args.Bounds.Bottom - 4);
			}
			DrawTabContent(args);
		}
		public override int TabVertBackIndent { get { return 1; } }
	}
	public class ButtonOffice2000Painter : ButtonPainter {
		const int borderIndent = 1, borderWidth = 1;
		public ButtonOffice2000Painter(DockElementsOffice2000Painter painter) : base(painter) { }
		protected override void DrawButtonBackground(DrawApplicationCaptionArgs captionArgs, DockPanelCaptionButton args) {
			if(!IsHotOrPressed(args.ButtonState)) return;
			DrawButtonBounds(captionArgs.Cache, args);
		}
		protected override void DrawButtonBounds(GraphicsCache cache, DockPanelCaptionButton args) {
			Brush light = SystemBrushes.ActiveCaptionText, dark = SystemBrushes.ControlText;
			if((args.ButtonState & ObjectState.Pressed) != 0 && (args.ButtonState & ObjectState.Hot) != 0) {
				Brush tmp = light;
				light = dark;
				dark = tmp;
			}
			DrawSingleBorderFrame(cache.Graphics, args.Bounds, light, dark);
		}
		protected void DrawSingleBorderFrame(Graphics g, Rectangle bounds, Brush light, Brush dark) {
			g.FillRectangle(light, new Rectangle(bounds.Left, bounds.Top, bounds.Width, 1));
			g.FillRectangle(light, new Rectangle(bounds.Left, bounds.Top + 1, 1, bounds.Height - 2));
			g.FillRectangle(dark, new Rectangle(bounds.Left, bounds.Bottom - 1, bounds.Width, 1));
			g.FillRectangle(dark, new Rectangle(bounds.Right - 1, bounds.Top + 1, 1, bounds.Height - 2));
		}
		public override Size ButtonSize { get { return new Size(ImageSize.Width + borderWidth + borderIndent, ImageSize.Height + borderWidth + borderIndent); } }
	}
	public class WindowOffice2000Painter : WindowPainter {
		public WindowOffice2000Painter(DockElementsOffice2000Painter painter) : base(painter) { }
		protected override void DrawWindowCaptionBackground(DrawWindowCaptionArgs e) {
			base.DrawWindowCaptionBackground(e);
			if(!e.ActiveCaption)
				DrawCaptionFrame(e);
		}
		protected virtual void DrawCaptionFrame(DrawArgs args) {
			args.Graphics.FillRectangle(SystemBrushes.ControlDark, new Rectangle(args.Bounds.Left + 2, args.Bounds.Top + 1, args.Bounds.Width - 4, 1));
			args.Graphics.FillRectangle(SystemBrushes.ControlDark, new Rectangle(args.Bounds.Left + 2, args.Bounds.Bottom - 2, args.Bounds.Width - 4, 1));
			args.Graphics.FillRectangle(SystemBrushes.ControlDark, new Rectangle(args.Bounds.Left + 1, args.Bounds.Top + 2, 1, args.Bounds.Height - 4));
			args.Graphics.FillRectangle(SystemBrushes.ControlDark, new Rectangle(args.Bounds.Right - 2, args.Bounds.Top + 2, 1, args.Bounds.Height - 4));
		}
		protected override void DrawApplicationBorder(DrawBorderArgs args) {
			ControlPaint.DrawBorder3D(args.Graphics, args.Bounds, Border3DStyle.Raised);
			args.BorderArgs.Bounds = Rectangle.Inflate(args.Bounds, -1, -1);
			Rectangle r = BorderPainter.GetObjectClientRectangle(args.BorderArgs);
			args.Paint.DrawRectangle(args.Graphics, args.BorderPen, r);
		}
		protected override BorderPainter BorderPainter { 
			get {
				if(BorderStyle == BorderStyles.Default) return BorderHelper.GetPainter(BorderStyles.HotFlat, LookAndFeel);
				return base.BorderPainter;
			}
		}
		protected internal override int GetImageIndex(int index, ObjectState state, CaptionButtonStatus status) {
			if(status == CaptionButtonStatus.ActiveApplicationButton || status == CaptionButtonStatus.InactiveApplicationButton ||
				status == CaptionButtonStatus.ActiveWindowButton) return (index + ButtonPainter.HotIndexOffset);
			return index;
		}
	}
}
