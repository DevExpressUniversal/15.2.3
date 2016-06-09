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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using DevExpress.Diagram.Core;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.ViewInfo;
using Orientation = System.Windows.Controls.Orientation;
namespace DevExpress.XtraDiagram.Paint {
	#region PaintArgs
	public abstract class DiagramRulerObjectInfoArgsBase : ObjectInfoArgs {
		MeasureUnit measureUnit;
		double rulerOffset, zoomLevel;
		bool transparentRulers;
		Point labelOffset;
		int rulerMargin;
		Rectangle clientRect;
		Rectangle rulerContentRect;
		int tickSize;
		DiagramAppearanceObject paintAppearance;
		UserLookAndFeel lookAndFeel;
		public DiagramRulerObjectInfoArgsBase() {
			this.paintAppearance = null;
			this.lookAndFeel = null;
		}
		public virtual void Initialize(DiagramControlViewInfo viewInfo) {
			this.measureUnit = viewInfo.MeasureUnit;
			this.zoomLevel = viewInfo.ZoomFactor;
			this.rulerMargin = viewInfo.RulerMargin;
			this.clientRect = viewInfo.ClientRect;
			this.lookAndFeel = viewInfo.LookAndFeel;
			this.tickSize = viewInfo.RulerTickSize;
			this.transparentRulers = viewInfo.AllowTransparentRulerBackground;
		}
		public virtual void Clear() {
			this.paintAppearance = null;
			this.lookAndFeel = null;
		}
		protected void SetRulerOffset(double rulerOffset) {
			this.rulerOffset = rulerOffset;
		}
		protected void SetLabelOffset(Point labelOffset) {
			this.labelOffset = labelOffset;
		}
		protected void SetRulerContentBounds(Rectangle bounds) {
			this.rulerContentRect = bounds;
		}
		protected void SetAppearance(DiagramAppearanceObject appearance) {
			this.paintAppearance = appearance;
		}
		public abstract DiagramLineDrawArgs GetBorderLineDrawArgs();
		public abstract DiagramLineDrawArgs GetTickDrawArgs(double position, double weight);
		public abstract Rectangle GetTickMarkBounds(double position, string text);
		public string GetLabelText(double value) {
			return string.Format(CultureInfo.CurrentCulture, "{0:0.##}", value);
		}
		public bool IsTinyTick(double weight) {
			return weight < 0.45;
		}
		public int GetTickSize(double weight) {
			return (int)(this.tickSize * weight);
		}
		public MeasureUnit MeasureUnit { get { return measureUnit; } }
		public double RulerOffset { get { return rulerOffset; } }
		public double ZoomLevel { get { return zoomLevel; } }
		public Point LabelOffset { get { return labelOffset; } }
		public Rectangle RulerContentBounds { get { return rulerContentRect; } }
		public Rectangle ClientRect { get { return clientRect; } }
		public int RulerMargin { get { return rulerMargin; } }
		public bool TransparentRulers { get { return transparentRulers; } }
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		public DiagramAppearanceObject PaintAppearance { get { return paintAppearance; } }
	}
	public class DiagramHRulerObjectInfoArgs : DiagramRulerObjectInfoArgsBase {
		public DiagramHRulerObjectInfoArgs() {
		}
		public override void Initialize(DiagramControlViewInfo viewInfo) {
			base.Initialize(viewInfo);
			this.Bounds = viewInfo.HRulerBounds;
			SetAppearance(viewInfo.HRulerPaintAppearance);
			SetRulerOffset(viewInfo.HRulerOffset);
			SetLabelOffset(viewInfo.HRulerLabelOffset);
			SetRulerContentBounds(viewInfo.HRulerContentBounds);
		}
		public override DiagramLineDrawArgs GetBorderLineDrawArgs() {
			Rectangle rulerBounds = RulerContentBounds;
			int xStartPt = ClientRect.X + RulerMargin;
			return new DiagramLineDrawArgs(xStartPt, rulerBounds.Bottom - 1, rulerBounds.Right, rulerBounds.Bottom - 1);
		}
		public override DiagramLineDrawArgs GetTickDrawArgs(double position, double weight) {
			Rectangle bounds = RulerContentBounds;
			int xPos = bounds.X + (int)Math.Round(position);
			int yStartPos = bounds.Bottom - 1 - GetTickSize(weight);
			return new DiagramLineDrawArgs(xPos, yStartPos, xPos, bounds.Bottom - 1);
		}
		public override Rectangle GetTickMarkBounds(double position, string text) {
			Rectangle markBounds = RulerContentBounds;
			Point pos = new Point(markBounds.X + (int)(position + 0.5), markBounds.Y);
			pos.Offset(LabelOffset);
			Size sz = PaintAppearance.CalcTextSizeInt(Cache, text, 0);
			pos.Offset(-sz.Width / 2 - 1 - 1, 0);
			return new Rectangle(pos.X, pos.Y, sz.Width + 1, sz.Height + 1);
		}
	}
	public class DiagramVRulerObjectInfoArgs : DiagramRulerObjectInfoArgsBase {
		public DiagramVRulerObjectInfoArgs() {
		}
		public override void Initialize(DiagramControlViewInfo viewInfo) {
			base.Initialize(viewInfo);
			this.Bounds = viewInfo.VRulerBounds;
			SetAppearance(viewInfo.VRulerPaintAppearance);
			SetRulerOffset(viewInfo.VRulerOffset);
			SetLabelOffset(viewInfo.VRulerLabelOffset);
			SetRulerContentBounds(viewInfo.VRulerContentBounds);
		}
		public override DiagramLineDrawArgs GetBorderLineDrawArgs() {
			Rectangle rulerBounds = RulerContentBounds;
			int yStartPt = ClientRect.Y + RulerMargin;
			return new DiagramLineDrawArgs(rulerBounds.Right - 1, yStartPt, rulerBounds.Right - 1, rulerBounds.Bottom);
		}
		public override DiagramLineDrawArgs GetTickDrawArgs(double position, double weight) {
			Rectangle bounds = RulerContentBounds;
			int yPos = bounds.Y + (int)Math.Round(position);
			int xStartPos = bounds.Right - 1 - GetTickSize(weight);
			return new DiagramLineDrawArgs(xStartPos, yPos, bounds.Right - 1, yPos);
		}
		public override Rectangle GetTickMarkBounds(double position, string text) {
			Point pos = new Point(RulerContentBounds.X, RulerContentBounds.Y + (int)position);
			pos.Offset(LabelOffset);
			Size sz = PaintAppearance.CalcTextSizeInt(Cache, text, 0);
			pos.Offset(0, -sz.Height / 2 - text.Length);
			Rectangle markBounds = new Rectangle(pos.X, pos.Y, sz.Height + 1, sz.Width + 1);
			return Rectangle.Inflate(markBounds, 0, 2);
		}
	}
	#endregion
	#region Painters
	public abstract class DiagramRulerPainterBase : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramRulerObjectInfoArgsBase args = (DiagramRulerObjectInfoArgsBase)e;
			base.DrawObject(e);
			DrawRuler(args);
			DrawBorder(args);
		}
		protected virtual void DrawRuler(DiagramRulerObjectInfoArgsBase args) {
			GraphicsClipState clip = args.Cache.ClipInfo.SaveAndSetClip(args.RulerContentBounds);
			try {
				RulerRenderHelper.DrawRuler(
					args.MeasureUnit,
					RulerKind,
					args.RulerContentBounds.Size.ToPlatformSize(),
					args.RulerOffset,
					args.ZoomLevel,
					(position, weight) => DrawTick(args, position, weight),
					(position, weight) => DrawTickMark(args, position, weight));
			}
			finally {
				args.Cache.ClipInfo.RestoreClipRelease(clip);
			}
		}
		protected virtual void DrawBorder(DiagramRulerObjectInfoArgsBase args) {
			DiagramLineDrawArgs lineArgs = args.GetBorderLineDrawArgs();
			args.Graphics.DrawLine(args.Cache.GetPen(args.PaintAppearance.ForeColor), lineArgs.X1, lineArgs.Y1, lineArgs.X2, lineArgs.Y2);
		}
		protected virtual void DrawTick(DiagramRulerObjectInfoArgsBase args, double position, double weight) {
			if(args.IsTinyTick(weight)) return; 
			DiagramLineDrawArgs lineArgs = args.GetTickDrawArgs(position, weight);
			args.Graphics.DrawLine(args.Cache.GetPen(args.PaintAppearance.ForeColor), lineArgs.X1, lineArgs.Y1, lineArgs.X2, lineArgs.Y2);
		}
		protected abstract Orientation RulerKind { get; }
		protected abstract void DrawTickMark(DiagramRulerObjectInfoArgsBase args, double position, double label);
	}
	public class DiagramHRulerPainter : DiagramRulerPainterBase {
		protected override void DrawTickMark(DiagramRulerObjectInfoArgsBase args, double position, double value) {
			string text = args.GetLabelText(value);
			args.PaintAppearance.DrawString(args.Cache, text, args.GetTickMarkBounds(position, text), StringFormat.GenericTypographic);
		}
		protected override Orientation RulerKind { get { return Orientation.Horizontal; } }
	}
	public class DiagramVRulerPainter : DiagramRulerPainterBase {
		protected override void DrawTickMark(DiagramRulerObjectInfoArgsBase args, double position, double value) {
			string text = args.GetLabelText(value);
			args.PaintAppearance.DrawVString(args.Cache, text, args.PaintAppearance.Font, args.Cache.GetSolidBrush(args.PaintAppearance.ForeColor), args.GetTickMarkBounds(position, text), StringFormat.GenericTypographic, 270);
		}
		protected override Orientation RulerKind { get { return Orientation.Vertical; } }
	}
	public abstract class DiagramRulerBackgroundPainterBase : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawBackground(e);
		}
		protected virtual void DrawBackground(ObjectInfoArgs e) {
			DrawObject(e.Cache, SkinElementPainter.Default, GetBackgroundElementInfo(e));
		}
		protected virtual ObjectInfoArgs GetBackgroundElementInfo(ObjectInfoArgs e) {
			DiagramRulerObjectInfoArgsBase args = (DiagramRulerObjectInfoArgsBase)e;
			return new SkinElementInfo(GetBackgroundElement(args), e.Bounds);
		}
		protected abstract SkinElement GetBackgroundElement(DiagramRulerObjectInfoArgsBase e);
	}
	public class DiagramHRulerBackgroundPainter : DiagramRulerBackgroundPainterBase {
		protected override SkinElement GetBackgroundElement(DiagramRulerObjectInfoArgsBase e) {
			if(e.TransparentRulers) {
				return RichEditSkins.GetSkin(e.LookAndFeel)[RichEditSkins.SkinHorizontalRulerBackground];
			}
			return ReportsSkins.GetSkin(e.LookAndFeel)[ReportsSkins.SkinRulerSection];
		}
	}
	public class DiagramVRulerBackgroundPainter : DiagramRulerBackgroundPainterBase {
		protected override SkinElement GetBackgroundElement(DiagramRulerObjectInfoArgsBase e) {
			if(e.TransparentRulers) {
				return RichEditSkins.GetSkin(e.LookAndFeel)[RichEditSkins.SkinVerticalRulerBackground];
			}
			return ReportsSkins.GetSkin(e.LookAndFeel)[ReportsSkins.SkinRulerSection];
		}
	}
	#endregion
}
