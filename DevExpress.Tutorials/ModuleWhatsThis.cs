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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
namespace DevExpress.Tutorials {
	public class ModuleWhatsThis : DevExpress.XtraEditors.XtraUserControl {
		private System.ComponentModel.IContainer components = null;
		private WhatsThisController controller;
		private Control hotTrackedControl;
		private bool parentFormActive;
		private ControlHotTrackPainterBase hotTrackPainter;
		public ModuleWhatsThis(WhatsThisController controller) {
			InitializeComponent();
			this.controller = controller;
			this.hotTrackedControl = null;
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.parentFormActive = true;
			this.hotTrackPainter = new ControlHotTrackPainterHint(this);
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.Cursor = System.Windows.Forms.Cursors.Help;
			this.Name = "ModuleWhatsThis";
			this.Click += new System.EventHandler(this.ModuleWhatsThis_Click);
			this.Load += new System.EventHandler(this.ModuleWhatsThis_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ModuleWhatsThis_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ModuleWhatsThis_MouseMove);
			this.MouseLeave += new System.EventHandler(this.ModuleWhatsThis_MouseLeave);
		}
		#endregion
		public ControlHotTrackPainterBase HotTrackPainter { get { return hotTrackPainter; } }
		public void SetHotTrackPainter(string painterName) {
			switch(painterName) {
				case "Hint":
					hotTrackPainter = new ControlHotTrackPainterHint(this);
					break;
				case "Frame":
					hotTrackPainter = new ControlHotTrackPainterFrame(this);
					break;
			}
		}
		private void ModuleWhatsThis_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			if(DesignMode) return;
			if(controller == null) return;
			Bitmap destBmp = new Bitmap(controller.WhatsThisModuleBitmap);
			Graphics tempG = Graphics.FromImage(destBmp);
			foreach(WhatsThisControlEntry entry in controller.RegisteredVisibleControls) {
				if(entry.Control == HotTrackedControl)
					HotTrackPainter.DrawHotTrackSign(tempG, entry);
			}
			e.Graphics.DrawImage(destBmp, 0, 0, destBmp.Width, destBmp.Height);
		}
		private Control HotTrackedControl {
			get { return hotTrackedControl; }
			set {
				if(hotTrackedControl == value) return;
				HotTrackControlChanging(hotTrackedControl, value);
				hotTrackedControl = value;
			}
		}
		private void HotTrackControlChanging(Control prevHotControl, Control currHotControl) {
			InvalidateControl(prevHotControl);
			InvalidateControl(currHotControl);
		}
		private void ModuleWhatsThis_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!parentFormActive) return;
			bool inControl = false;
			Control prevHotControl = hotTrackedControl;
			foreach(WhatsThisControlEntry entry in controller.RegisteredVisibleControls) {
				Rectangle controlRect = this.RectangleToClient(entry.ControlScreenRect);
				if(controlRect.Contains(e.X, e.Y)) {
					HotTrackedControl = entry.Control;
					inControl = true;
				}
			}
			if(!inControl)
				HotTrackedControl = null;
		}
		private void InvalidateControl(Control control) {
			if(control == null) return;
			foreach(WhatsThisControlEntry entry in controller.RegisteredVisibleControls) {
				if(entry.Control == control) {
					Region invalidateRegion = HotTrackPainter.GetInvalidateRegion(entry);
					Invalidate(invalidateRegion);
					return;
				}
			}
		}
		private void ModuleWhatsThis_Click(object sender, System.EventArgs e) {
			if(hotTrackedControl == null) return;
			controller.TryToShowWhatsThis(hotTrackedControl);
			InvalidateControl(hotTrackedControl);
		}
		private void ModuleWhatsThis_MouseLeave(object sender, System.EventArgs e) {
			HotTrackedControl = null;
		}
		private void AssignParentFormHandlers() {
			ParentForm.Activated += new EventHandler(ParentFormActivated);
			ParentForm.Deactivate += new EventHandler(ParentFormDeactivate);
		}
		private void ParentFormActivated(object sender, EventArgs e) {
			parentFormActive = true;
		}
		private void ParentFormDeactivate(object sender, EventArgs e) {
			parentFormActive = false;
			HotTrackedControl = null;
		}
		private void ModuleWhatsThis_Load(object sender, System.EventArgs e) {
			if(DesignMode) return;
			AssignParentFormHandlers();
		}
	}
	public abstract class ControlHotTrackPainterBase {
		protected ModuleWhatsThis module;
		public ControlHotTrackPainterBase(ModuleWhatsThis module) {
			this.module = module;
		}
		public abstract void DrawHotTrackSign(Graphics g, WhatsThisControlEntry entry);
		public abstract Region GetInvalidateRegion(WhatsThisControlEntry entry);
		protected virtual int HotTrackRegionWidth { get { return 3; } }
		protected Rectangle GetOuterRect(Rectangle rect, int offset) {
			return new Rectangle(rect.Left - offset, rect.Top - offset, 
				rect.Width + offset * 2, rect.Height + offset * 2);
		}
		public virtual string Name { get { return "Base"; } }
	}
	public class ControlHotTrackPainterHint : ControlHotTrackPainterBase {
		public ControlHotTrackPainterHint(ModuleWhatsThis module) : base(module) {}
		public override void DrawHotTrackSign(Graphics g, WhatsThisControlEntry entry) {
			Rectangle controlRect = module.RectangleToClient(entry.ControlScreenRect);
			Rectangle hintRect = GetHintRect(controlRect);
			Rectangle shadowRect = GetShadowRect(hintRect);
			g.FillRectangle(new SolidBrush(Color.FromArgb(150, SystemColors.ControlDark)), shadowRect);
			g.FillRectangle(SystemBrushes.Info, hintRect);
			hintRect.Width -= 1;
			hintRect.Height -= 1;
			g.DrawRectangle(SystemPens.WindowText, hintRect);
			g.DrawImage(entry.ControlBitmap, controlRect);
		}
		private Rectangle GetHintRect(Rectangle controlRect) {
			return GetOuterRect(controlRect, 4);
		}
		private Rectangle GetShadowRect(Rectangle hintRect) {
			return new Rectangle(hintRect.X + 3, hintRect.Y + 3, hintRect.Width, hintRect.Height);
		}
		public override Region GetInvalidateRegion(WhatsThisControlEntry entry) {
			Rectangle controlRect = module.RectangleToClient(entry.ControlScreenRect);
			Region rgn = new Region(GetHintRect(controlRect));
			rgn.Union(GetShadowRect(GetHintRect(controlRect)));
			rgn.Exclude(controlRect);
			return rgn;
		}
		public override string Name { get { return "Hint"; } }
	}
	public class ControlHotTrackPainterFrame : ControlHotTrackPainterBase {
		public ControlHotTrackPainterFrame(ModuleWhatsThis module) : base(module) {}
		public override void DrawHotTrackSign(Graphics g, WhatsThisControlEntry entry) {
			Region frameRgn = GetInvalidateRegion(entry);
			LinearGradientBrush brush = new LinearGradientBrush(frameRgn.GetBounds(g), ColorUtils.GetGradientActiveCaptionColor(), SystemColors.ActiveCaption, LinearGradientMode.ForwardDiagonal);
			g.FillRegion(brush, frameRgn);
		}
		public override Region GetInvalidateRegion(WhatsThisControlEntry entry) {
			Rectangle controlRect = module.RectangleToClient(entry.ControlScreenRect);
			Rectangle outerFrameBounds = GetOuterRect(controlRect, 3);
			Region frameRgn = new Region(outerFrameBounds);
			frameRgn.Exclude(controlRect);
			return frameRgn;		 
		}
		public override string Name { get { return "Frame"; } }
	}
	public class ImageShaderBase {
		public Bitmap ShadeBitmap(Bitmap bmp) {
			Bitmap tempBmp = new Bitmap(bmp);
			Graphics g = Graphics.FromImage(tempBmp);
			PaintShade(g, bmp);
			return tempBmp;
		}
		protected virtual void PaintShade(Graphics g, Bitmap original) {
		}
	}
	public class ImageShaderDisable : ImageShaderBase {
		protected override void PaintShade(Graphics g, Bitmap original) {
			ControlPaint.DrawImageDisabled(g, original, 0, 0, Color.Transparent);
		}
	}
	public class ImageShaderPatternBase : ImageShaderBase {
		protected override void PaintShade(Graphics g, Bitmap original) {
			Bitmap pattern = GetFillPattern();
			TextureBrush patternBrush = new TextureBrush(pattern);
			g.FillRectangle(patternBrush, 0, 0, original.Width, original.Height);
		}
		protected virtual Bitmap GetFillPattern() {
			Bitmap bmp = new Bitmap(1, 1);
			bmp.SetPixel(0, 0, Color.Transparent);
			return bmp;
		}
	}
	public class ImageShaderPatternDots : ImageShaderPatternBase {
		protected override Bitmap GetFillPattern() {
			Bitmap bmp = new Bitmap(2, 2);		 
			bmp.SetPixel(0, 0, Color.FromArgb(150, SystemColors.ControlDarkDark));
			bmp.SetPixel(0, 1, Color.Transparent);
			bmp.SetPixel(1, 0, Color.Transparent);
			bmp.SetPixel(1, 1, Color.FromArgb(150, SystemColors.ControlDarkDark));
			return bmp;
		}
	}
	public class ImageShaderHatch : ImageShaderBase {
		HatchStyle style;
		public ImageShaderHatch(HatchStyle style) {
			this.style = style;
		}
		protected override void PaintShade(Graphics g, Bitmap original) {
			HatchBrush brush = new HatchBrush(style, Color.FromArgb(150, SystemColors.ControlDarkDark), Color.Transparent);
			g.FillRectangle(brush, 0, 0, original.Width, original.Height);
		}
	}
}
