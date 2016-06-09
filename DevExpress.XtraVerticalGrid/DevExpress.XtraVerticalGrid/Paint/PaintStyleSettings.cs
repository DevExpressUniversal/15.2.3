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

namespace DevExpress.XtraVerticalGrid.Painters {
	using System;
	using System.Collections;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;
	using DevExpress.LookAndFeel;
	using DevExpress.XtraEditors.Drawing;
	using DevExpress.XtraEditors.Controls;
	using DevExpress.Skins;
	using DevExpress.Utils;
	using DevExpress.Utils.Win;
	using DevExpress.Utils.Paint;
	using DevExpress.Utils.Drawing;
	using DevExpress.XtraVerticalGrid.Events;
	using DevExpress.XtraVerticalGrid.Rows;
	using DevExpress.XtraVerticalGrid.ViewInfo;
	public class VGridPaintStyleCollection : CollectionBase {
		VGridPainter owner;
		public VGridPaintStyleCollection(VGridPainter owner) {
			this.owner = owner;
		}
		public VGridPaintStyle this[ActiveLookAndFeelStyle name] {
			get {
				string styleName = name.ToString();
				foreach(VGridPaintStyle style in InnerList) {
					if(style.Name == styleName)
						return style;
				}
				VGridPaintStyle result = CreateStyle(name);
				InnerList.Add(result);
				return result;
			}
		}
		protected virtual VGridPaintStyle CreateStyle(ActiveLookAndFeelStyle name) {
			string styleName = name.ToString();
			switch(name) {
				case ActiveLookAndFeelStyle.Office2003: return new Office2003PaintStyle(this, styleName);
				case ActiveLookAndFeelStyle.Skin: return new SkinPaintStyle(this, styleName);
				case ActiveLookAndFeelStyle.WindowsXP: return new WindowsXPPaintStyle(this, styleName);
				case ActiveLookAndFeelStyle.Style3D: return new Style3DPaintStyle(this, styleName);
			}
			return new VGridPaintStyle(this, styleName);
		}
		public VGridPainter Owner { get { return owner; } }
		protected internal UserLookAndFeel LookAndFeel { get { return Owner.EventHelper.LookAndFeel; } }
	}
	public class VGridPaintStyle {
		VGridPaintStyleCollection owner;
		string name;
		ObjectPainter openCloseButtonPainter;
		public VGridPaintStyle(VGridPaintStyleCollection owner, string name) {
			this.owner = owner;
			this.name = name;
			this.openCloseButtonPainter = CreateOpenCloseButtonPainter();
		}
		protected virtual ObjectPainter CreateOpenCloseButtonPainter() { return LookAndFeel.Painter.OpenCloseButton; }
		public virtual PaintStyleCalcHelper CreateCalcHelper(BaseViewInfo vi) { return new DotNetStyleCalcHelper(vi); }
		public virtual void DrawExplorerButton(CustomDrawTreeButtonEventArgs e) {
			Rectangle bounds = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Size.Width / 2 * 2, e.Bounds.Size.Height / 2 * 2);
			Rectangle r = Rectangle.Inflate(bounds, -2, -2);
			int x = r.X + r.Width / 2;
			int y = r.Y + r.Height / 2 - (e.Expanded ? 2 : 1);
			Pen ringPen = new Pen(ControlPaint.LightLight(e.Appearance.ForeColor));
			Pen ringPen2 = new Pen(Color.FromArgb(e.Appearance.ForeColor.A, Color.FromArgb(
				(ringPen.Color.R + e.Appearance.BackColor.R) / 2,
				(ringPen.Color.G + e.Appearance.BackColor.G) / 2,
				(ringPen.Color.B + e.Appearance.BackColor.B) / 2)));
			e.Graphics.FillEllipse(e.Appearance.GetBackBrush(e.Cache), r);
			e.Graphics.DrawEllipse(ringPen2, Rectangle.Inflate(bounds, -1, -1));
			e.Graphics.DrawEllipse(ringPen, r);
			DrawStroke(e, x, y - 2, 1);
			DrawStroke(e, x, y - 2, 0);
			DrawStroke(e, x, y + 2, 1);
			DrawStroke(e, x, y + 2, 0);
		}
		void DrawStroke(CustomDrawTreeButtonEventArgs e, int x, int startY, int delta) {
			int dx = 3, dy = 3;
			if(e.Expanded) startY += dy;
			dx -= delta; dy -= delta;
			if(e.Expanded) dy = -dy;
			e.Graphics.DrawLine(e.Appearance.GetForePen(e.Cache), new Point(x - dx, startY), new Point(x, startY + dy));
			e.Graphics.DrawLine(e.Appearance.GetForePen(e.Cache), new Point(x, startY + dy), new Point(x + dx, startY));
		}
		public virtual void DrawRowHeaderCellBackground(CustomDrawRowHeaderCellEventArgs e) {
			e.Appearance.FillRectangle(e.Cache, e.Bounds);
		}
		public virtual void DrawVertLineSeparator(CustomDrawSeparatorEventArgs e) {
			e.Appearance.FillRectangle(e.Cache, e.Bounds);
			XPaint.Graphics.FillRectangle(e.Graphics, Painter.ResourceCache.RowVertLineBrush, e.Bounds.Left + e.Bounds.Width / 2,
				e.Bounds.Top + 2, 1, e.Bounds.Height - 4);
			XPaint.Graphics.FillRectangle(e.Graphics, e.Appearance.GetBackBrush(e.Cache), e.Bounds.Left + e.Bounds.Width / 2,
				e.Bounds.Top, 1, 2);
			XPaint.Graphics.FillRectangle(e.Graphics, e.Appearance.GetBackBrush(e.Cache), e.Bounds.Left + e.Bounds.Width / 2,
				e.Bounds.Bottom - 2, 1, 2);
		}
		public virtual void DrawStyleFeatures(BaseViewInfo vi) {}
		public virtual void DrawHeader_ValuesSeparator(BaseRowViewInfo ri, int sepWidth) {}
		public virtual AppearanceDefaultInfo[] GetAppearanceDefaults() {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo(GridStyles.RowHeaderPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, HorzAlignment.Near, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.PressedRow,  new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.HideSelectionRow,  new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.Category,  new AppearanceDefault(SystemColors.WindowText, CategoryBackColor, GridLineColor, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.HorzLine,  new AppearanceDefault(Color.Empty, GridLineColor, Color.Empty, GridLineColor, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.VertLine,  new AppearanceDefault(Color.Empty, GridLineColor, Color.Empty, GridLineColor, HorzAlignment.Near, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.RecordValue, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.BandBorder, new AppearanceDefault(Color.Empty, BandBorderColor, Color.Empty, BandBorderColor, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.FocusedRow, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.FocusedRecord, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.FocusedCell, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.ExpandButton, new AppearanceDefault(SystemColors.WindowFrame, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.CategoryExpandButton, new AppearanceDefault(CategoryExpandButtonForeColor, CategoryExpandButtonBackColor, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.Empty, new AppearanceDefault(SystemColors.Window, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.DisabledRecordValue, new AppearanceDefault(DisabledText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.DisabledRow, new AppearanceDefault(DisabledText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.ReadOnlyRecordValue, new AppearanceDefault(DisabledText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.ReadOnlyRow, new AppearanceDefault(DisabledText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.ModifiedRecordValue, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Default, new Font(AppearanceObject.DefaultFont, FontStyle.Bold))),
				new AppearanceDefaultInfo(GridStyles.ModifiedRow, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.FixedLine, new AppearanceDefault(Color.Empty, FixedLineColor, Color.Empty, FixedLineColor, HorzAlignment.Center, VertAlignment.Center))
			};
		}
		public virtual PaintStyleParams GetPaintStyleParams() { return new PaintStyleParams(); }
		public virtual Color GetSizeGripColor(Color baseColor) { return LookAndFeelHelper.GetSystemColor(LookAndFeel, baseColor); }
		protected virtual Color CategoryBackColor { get { return SystemColors.Control; } }
		protected virtual Color CategoryExpandButtonBackColor { get { return SystemColors.Control; } }
		protected virtual Color CategoryExpandButtonForeColor { get { return SystemColors.WindowFrame; } }
		protected virtual Color GridLineColor { get { return SystemColors.ControlDark; } }
		protected virtual Color BandBorderColor { get { return SystemColors.ControlDarkDark; } }
		protected virtual Color FixedLineColor { get { return SystemColors.ControlDarkDark; } }
		protected virtual Color DisabledText { get { return SystemColors.GrayText; } }
		protected UserLookAndFeel LookAndFeel { get { return owner.LookAndFeel; } }
		protected VGridPainter Painter { get { return owner.Owner; } }
		protected Graphics Graphics { get { return Painter.Graphics; } }
		public string Name { get { return name; } }
		public ObjectPainter OpenCloseButtonPainter { get { return openCloseButtonPainter; } }
		public virtual BorderPainter BorderPainter { get { return BorderHelper.GetPainter(Painter.EventHelper.BorderStyle, LookAndFeel); } }
		public virtual Size ExplorerButtonCollapseSize { get { return new Size(19, 19); } }
		public virtual Size ExplorerButtonExpandSize { get { return new Size(19, 19); } }
	}
	public class Style3DPaintStyle : VGridPaintStyle {
		public Style3DPaintStyle(VGridPaintStyleCollection owner, string name) : base(owner, name) {}
		public override AppearanceDefaultInfo[] GetAppearanceDefaults() {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo(GridStyles.RowHeaderPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.PressedRow, new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.HideSelectionRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.Category, new AppearanceDefault(Color.DarkMagenta, CategoryBackColor, GridLineColor, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.HorzLine, new AppearanceDefault(Color.Empty, GridLineColor, Color.Empty, GridLineColor, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.VertLine, new AppearanceDefault(Color.Empty, GridLineColor, Color.Empty, GridLineColor, HorzAlignment.Near, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.RecordValue, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.BandBorder, new AppearanceDefault(Color.Empty, BandBorderColor, Color.Empty, BandBorderColor, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.FocusedRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.FocusedRecord, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.FocusedCell, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.ExpandButton, new AppearanceDefault(SystemColors.WindowFrame, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.CategoryExpandButton, new AppearanceDefault(SystemColors.WindowFrame, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.Empty, new AppearanceDefault(SystemColors.Control, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.DisabledRecordValue, new AppearanceDefault(SystemColors.GrayText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.DisabledRow, new AppearanceDefault(SystemColors.GrayText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.ReadOnlyRecordValue, new AppearanceDefault(SystemColors.GrayText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.ReadOnlyRow, new AppearanceDefault(SystemColors.GrayText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.ModifiedRecordValue, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Default, new Font(AppearanceObject.DefaultFont, FontStyle.Bold))),
				new AppearanceDefaultInfo(GridStyles.ModifiedRow, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.FixedLine, new AppearanceDefault(Color.Empty, FixedLineColor, Color.Empty, FixedLineColor, HorzAlignment.Center, VertAlignment.Center))
			};
		}
		public override PaintStyleCalcHelper CreateCalcHelper(BaseViewInfo vi) { return new Style3DCalcHelper(vi); }
		public override void DrawVertLineSeparator(CustomDrawSeparatorEventArgs e) {
			e.Appearance.FillRectangle(e.Cache, e.Bounds);
			ControlPaint.DrawBorder3D(e.Graphics, e.Bounds.Left + e.Bounds.Width / 2 - 1,
				e.Bounds.Top + 2, 2, e.Bounds.Height - 4, Border3DStyle.Etched, Border3DSide.Left);
			XPaint.Graphics.FillRectangle(e.Graphics, e.Appearance.GetBackBrush(e.Cache), e.Bounds.Left + e.Bounds.Width / 2 - 1,
				e.Bounds.Top, 2, 2);
			XPaint.Graphics.FillRectangle(e.Graphics, e.Appearance.GetBackBrush(e.Cache), e.Bounds.Left + e.Bounds.Width / 2 - 1,
				e.Bounds.Bottom - 2, 2, 2);
		}
		public override void DrawHeader_ValuesSeparator(BaseRowViewInfo ri, int sepWidth) {
			Rectangle r = new Rectangle(ri.HeaderInfo.HeaderRect.Right, ri.HeaderInfo.HeaderRect.Top, 
				ri.ValuesRect.Left - ri.HeaderInfo.HeaderRect.Right, ri.HeaderInfo.HeaderRect.Height + sepWidth);
			if(Painter.NeedsRedraw(r))
				ControlPaint.DrawBorder3D(Graphics, r, Border3DStyle.Etched, Border3DSide.Left);
		}
		public override void DrawStyleFeatures(BaseViewInfo vi) {
			Painter.DrawLines(vi.FocusLinesInfo, vi.ViewRects.Client);
		}
		public override PaintStyleParams GetPaintStyleParams() {
			PaintStyleParams result = base.GetPaintStyleParams();
			result.SeparatorWidth = 2;
			return result;
		}
	}
	public class Office2003PaintStyle : VGridPaintStyle {
		public Office2003PaintStyle(VGridPaintStyleCollection owner, string name) : base(owner, name) {}
		AppearanceDefault Get(Office2003GridAppearance app) {
			return Office2003Colors.Default[app].Clone() as AppearanceDefault;
		}
		protected override Color DisabledText { get { return Office2003Colors.Default[Office2003Color.TextDisabled]; } }
		protected override Color CategoryExpandButtonForeColor { get { return ControlPaint.Dark(Office2003Colors.Default[Office2003Color.Button2]); } }
		protected override Color CategoryExpandButtonBackColor { get { return SystemColors.Window; } }
		protected override Color GridLineColor { get { return Get(Office2003GridAppearance.GridLine).BackColor; } }
		protected override Color BandBorderColor { get { return Get(Office2003GridAppearance.GroupPanel).BackColor; } }
		protected override Color FixedLineColor { get { return Get(Office2003GridAppearance.GroupPanel).BackColor; } }
		protected override Color CategoryBackColor { get { return Get(Office2003GridAppearance.GroupRow).BackColor; } }
	}
	public class WindowsXPPaintStyle : VGridPaintStyle {
		public WindowsXPPaintStyle(VGridPaintStyleCollection owner, string name) : base(owner, name) {}
		public override void DrawExplorerButton(CustomDrawTreeButtonEventArgs e) {
			int part = e.Expanded ? 6 : 7;
			NativeControlAdvPaintArgs pArgs =  new NativeControlAdvPaintArgs(e.Graphics, e.Bounds, ButtonPredefines.Glyph, ButtonStates.None,
				e.Appearance.GetBackBrush(e.Cache),	true, "explorerbar", part, 1, false); 
			DevExpress.Utils.WXPaint.Painter.Draw(pArgs);
		}
		public override Size ExplorerButtonCollapseSize { get { return GetExplorerButtonSize(7); } }
		public override Size ExplorerButtonExpandSize { get { return GetExplorerButtonSize(6); } }
		Size GetExplorerButtonSize(int part) {
			NativeControlAdvPaintArgs ne = new NativeControlAdvPaintArgs(null, Rectangle.Empty, ButtonPredefines.Glyph, ButtonStates.None,
				SystemBrushes.Control,	true, "explorerbar", part, 1, false);
			return DevExpress.Utils.WXPaint.Painter.CalcButtonMinSize(ne);
		}
	}
	public class SkinPaintStyle : VGridPaintStyle {
		SkinHeaderObjectPainter headerPainter;
		BorderPainter borderPainter;
		public SkinPaintStyle(VGridPaintStyleCollection owner, string name) : base(owner, name) {
			this.headerPainter = CreateHeaderPainter();
			this.borderPainter = CreateBorderPainter();
		}
		protected override ObjectPainter CreateOpenCloseButtonPainter() {
			return new SkinOpenCloseButtonObjectPainter(LookAndFeel);
		}
		protected virtual BorderPainter CreateBorderPainter() {
			return new VGridSkinBorderPainter(LookAndFeel);
		}
		protected virtual SkinHeaderObjectPainter CreateHeaderPainter() {
			return new SkinHeaderObjectPainter(LookAndFeel);
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaults() {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo(GridStyles.RowHeaderPanel, UpdateStyleDefaults(Skin[VGridSkins.SkinRow], new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.PressedRow, UpdateSystemColors(new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.HideSelectionRow, UpdateSystemColors(new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, HorzAlignment.Default))),
				new AppearanceDefaultInfo(GridStyles.Category, UpdateStyleDefaults(Skin[VGridSkins.SkinCategory], new AppearanceDefault(SystemColors.WindowText, CategoryBackColor, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.HorzLine, UpdateStyleDefaults(Skin[VGridSkins.SkinGridLine], new AppearanceDefault(Color.Empty, GridLineColor, Color.Empty, GridLineColor, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.VertLine, UpdateStyleDefaults(Skin[VGridSkins.SkinGridLine], new AppearanceDefault(Color.Empty, GridLineColor, Color.Empty, GridLineColor, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.RecordValue, UpdateStyleDefaultsOrUpdateSystemColors(Skin[VGridSkins.SkinRecordValue], new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.BandBorder, UpdateStyleDefaults(Skin[VGridSkins.SkinBandBorder], new AppearanceDefault(Color.Empty, BandBorderColor, HorzAlignment.Center, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.FocusedRow, UpdateSystemColors(new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.FocusedRecord, UpdateStyleDefaultsOrUpdateSystemColors(Skin[VGridSkins.SkinRecordValue], new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.FocusedCell, UpdateSystemColors(new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.ExpandButton, new AppearanceDefault(SystemColors.WindowFrame, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.CategoryExpandButton, new AppearanceDefault(CategoryExpandButtonForeColor, CategoryExpandButtonBackColor, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(GridStyles.Empty, UpdateSystemColors(new AppearanceDefault(SystemColors.Window, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.DisabledRecordValue, UpdateStyleDefaultsOrUpdateSystemColors(Skin[VGridSkins.SkinRecordValue], new AppearanceDefault(DisabledText, Color.Empty, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.DisabledRow, UpdateStyleDefaults(Skin[VGridSkins.SkinRowDisabled], new AppearanceDefault(DisabledText, Color.Empty, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.ReadOnlyRecordValue, UpdateStyleDefaultsOrUpdateSystemColors(Skin[VGridSkins.SkinRecordValueReadOnly], new AppearanceDefault(DisabledText, Color.Empty, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.ReadOnlyRow, UpdateStyleDefaultsOrUpdateSystemColors(Skin[VGridSkins.SkinRowReadOnly], new AppearanceDefault(DisabledText, Color.Empty, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.ModifiedRecordValue, UpdateStyleDefaultsOrUpdateSystemColors(Skin[VGridSkins.SkinRecordValue], new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Default, new Font(AppearanceObject.DefaultFont, FontStyle.Bold)))),
				new AppearanceDefaultInfo(GridStyles.ModifiedRow, UpdateStyleDefaults(Skin[VGridSkins.SkinRow], new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(GridStyles.FixedLine, UpdateStyleDefaults(Skin[VGridSkins.SkinFixedLine], new AppearanceDefault(Color.Empty, FixedLineColor, HorzAlignment.Center, VertAlignment.Center))),
			};
		}
		protected AppearanceDefault UpdateStyleDefaultsOrUpdateSystemColors(SkinElement element, AppearanceDefault defs) {
			if (element == null)
				return UpdateSystemColors(defs);
			return UpdateStyleDefaults(element, defs);
		}
		protected AppearanceDefault UpdateStyleDefaults(SkinElement element, AppearanceDefault defs) {
			if(element == null)
				return defs;
			defs = element.ApplyForeColorAndFont(defs);
			if (element.Color.GetBackColor() != Color.Empty) {
				defs.BackColor = element.Color.GetBackColor();
			}
			if (element.Color.GetBackColor2() != Color.Empty) {
				defs.BackColor2 = element.Color.GetBackColor2();
			}
			if (element.Color.GradientMode != LinearGradientMode.Horizontal)
				defs.GradientMode = element.Color.GradientMode;
			if (element.Border.All != Color.Empty)
				defs.BorderColor = element.Border.All;
			return defs;
		}
		protected AppearanceDefault UpdateSystemColors(AppearanceDefault defs) {
			defs.ForeColor = CommonSkins.GetSkin(LookAndFeel).TranslateColor(defs.ForeColor);
			defs.BackColor = CommonSkins.GetSkin(LookAndFeel).TranslateColor(defs.BackColor);
			return defs;
		}
		public override void DrawExplorerButton(CustomDrawTreeButtonEventArgs e) {
			SkinElementInfo info = CreateExplorerButtonInfo(e.Expanded);
			info.Cache = e.Cache;
			info.Bounds = e.Bounds;
			SkinElementPainter.Default.DrawObject(info);
		}
		SkinElementInfo CreateExplorerButtonInfo(bool expanded) {
			SkinElementInfo result = new SkinElementInfo(Skin[VGridSkins.SkinCategoryButton]);
			result.ImageIndex = expanded ? 0 : 3;
			return result;
		}
		public override Size ExplorerButtonCollapseSize { get { return GetExplorerButtonSize(false); } }
		public override Size ExplorerButtonExpandSize { get { return GetExplorerButtonSize(true); } }
		Size GetExplorerButtonSize(bool expanded) {
			SkinElementInfo info = CreateExplorerButtonInfo(expanded);
			ImageCollection ic = null;
			if(info.Element != null && info.Element.Image != null) {
				ic = info.Element.Image.GetImages();
			}
			return (ic == null ? Size.Empty : ic.ImageSize);
		}
		protected Skin GridSkin { get { return GridSkins.GetSkin(LookAndFeel); } }
		protected Skin Skin { get { return VGridSkins.GetSkin(LookAndFeel); } }
		protected SkinHeaderObjectPainter HeaderPainter { get { return headerPainter; } }
		public override BorderPainter BorderPainter {
			get {
				if(Painter.EventHelper.BorderStyle == BorderStyles.NoBorder) return base.BorderPainter;
				return borderPainter;
			}
		}
	}
	public class VGridSkinBorderPainter : SkinBorderPainter {
		public  VGridSkinBorderPainter(ISkinProvider provider) : base(provider) {}
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			return new SkinElementInfo(VGridSkins.GetSkin(Provider)[VGridSkins.SkinBorder]);
		}
	}
}
