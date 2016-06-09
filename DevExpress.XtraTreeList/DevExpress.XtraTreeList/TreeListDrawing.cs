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
using System.Collections;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Skins;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Text;
namespace DevExpress.XtraTreeList.Painter {
	public abstract class ElementPainters {
		protected PaintLink fPaintLink;
		HeaderObjectPainter headerPainter;
		IndicatorObjectPainter indicatorPainter;
		ObjectPainter openCloseButtonPainter;
		FooterPanelPainter footerPanelPainter;
		FooterCellPainter footerCellPainter;
		ObjectPainter sortedShapePainter;
		ObjectPainter filterButton, smartFilterButton;
		CheckObjectPainter checkPainter;
		ObjectPainter specialRowSeparatorPainter;
		TreeListFilterPanelPainter filterPanel;
		HeaderObjectPainter bandPainter;
		ObjectPainter captionPainter;
		protected ElementPainters(PaintLink paintLink) {
			this.fPaintLink = paintLink;
			this.headerPainter = CreateHeaderPainter();
			this.bandPainter = CreateBandPainter();
			this.indicatorPainter = CreateIndicatorPainter();
			this.openCloseButtonPainter = CreateOpenCloseButtonPainter();
			this.footerPanelPainter = CreateFooterPanelPainter();
			this.footerCellPainter = CreateFooterCellPainter();
			this.sortedShapePainter = CreateSortedShapePainter();
			this.checkPainter = CreateCheckPainter();
			this.specialRowSeparatorPainter = CreateSpecialRowSeparatorPainter();
			this.filterPanel = CreateFilterPanelPainter();
			this.filterButton = CreateHeaderFilterButtonPainter();
			this.smartFilterButton = CreateHeaderSmartFilterButtonPainter();
			this.captionPainter = CreateCaptionPainter();
		}
		protected virtual HeaderObjectPainter CreateHeaderPainter() { return LookAndFeel.Painter.Header; }
		protected virtual HeaderObjectPainter CreateBandPainter() { return LookAndFeel.Painter.Header; } 
		protected virtual ObjectPainter CreateOpenCloseButtonPainter() { return LookAndFeel.Painter.OpenCloseButton; }
		protected virtual FooterPanelPainter CreateFooterPanelPainter() { return LookAndFeel.Painter.FooterPanel; }
		protected virtual FooterCellPainter CreateFooterCellPainter() { return LookAndFeel.Painter.FooterCell; }
		protected virtual ObjectPainter CreateSortedShapePainter() { return LookAndFeel.Painter.SortedShape; }
		protected virtual CheckObjectPainter CreateCheckPainter() { return CheckPainterHelper.GetPainter(LookAndFeel); }
		protected virtual ObjectPainter CreateCaptionPainter() { return new TreeListCaptionPainter(fPaintLink.TreeList); }
		protected abstract IndicatorObjectPainter CreateIndicatorPainter();
		protected UserLookAndFeel LookAndFeel { get { return fPaintLink.LookAndFeel; } }
		public virtual BorderObjectInfoArgs GetBorderPainterInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject border) {
			return new BorderObjectInfoArgs(cache, bounds, border, fPaintLink.BorderStyle == BorderStyles.UltraFlat ? ObjectState.Disabled : ObjectState.Normal);
		}
		protected virtual ObjectPainter CreateSpecialRowSeparatorPainter() { return new TreeListSpecialTopSeparatorPainter(); }
		protected virtual TreeListFilterPanelPainter CreateFilterPanelPainter() { return new TreeListFilterPanelPainter(EditorButtonHelper.GetPainter(BorderStyles.Default, LookAndFeel), CheckPainterHelper.GetPainter(LookAndFeel)); }
		protected virtual ObjectPainter CreateHeaderSmartFilterButtonPainter() { return new GridSmartFlatFilterButtonPainter(); }
		protected virtual ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(BorderStyles.Flat)); } 
		public virtual AppearanceDefaultInfo[] GetAppearanceDefault() {
			AppearanceDefaultInfo[] result = new AppearanceDefaultInfo[] {
					new AppearanceDefaultInfo(AppearanceName.HeaderPanel, new AppearanceDefault(HeaderForeColor, HeaderBackColor)),
					new AppearanceDefaultInfo(AppearanceName.HeaderPanelBackground, new AppearanceDefault(HeaderForeColor, SystemColors.ControlDark)),
					new AppearanceDefaultInfo(AppearanceName.BandPanel, new AppearanceDefault(HeaderForeColor, HeaderBackColor)),
					new AppearanceDefaultInfo(AppearanceName.FooterPanel, new AppearanceDefault(FooterCellColor, SystemColors.Control)),
					new AppearanceDefaultInfo(AppearanceName.Row, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window)),
					new AppearanceDefaultInfo(AppearanceName.EvenRow, new AppearanceDefault(SystemColors.WindowText, Color.LightSkyBlue)),
					new AppearanceDefaultInfo(AppearanceName.OddRow, new AppearanceDefault(SystemColors.WindowText, Color.LightSalmon)),
					new AppearanceDefaultInfo(AppearanceName.HorzLine, new AppearanceDefault(GridLineColor, GridLineColor)),
					new AppearanceDefaultInfo(AppearanceName.VertLine, new AppearanceDefault(GridLineColor, GridLineColor)),
					new AppearanceDefaultInfo(AppearanceName.Preview, new AppearanceDefault(Color.Blue, SystemColors.Window, HorzAlignment.Near, VertAlignment.Top)),
					new AppearanceDefaultInfo(AppearanceName.FocusedRow, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight)),
					new AppearanceDefaultInfo(AppearanceName.FocusedCell, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window)),
					new AppearanceDefaultInfo(AppearanceName.GroupButton, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
					new AppearanceDefaultInfo(AppearanceName.TreeLine, new AppearanceDefault(TreeLineColor, TreeLineColor)),
					new AppearanceDefaultInfo(AppearanceName.GroupFooter, new AppearanceDefault(FooterCellColor, SystemColors.Control)),
					new AppearanceDefaultInfo(AppearanceName.Empty, new AppearanceDefault(SystemColors.Window, SystemColors.Window)),
					new AppearanceDefaultInfo(AppearanceName.SelectedRow, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight)),
					new AppearanceDefaultInfo(AppearanceName.HideSelectionRow, new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption)),
					new AppearanceDefaultInfo(AppearanceName.FixedLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDarkDark)),
					new AppearanceDefaultInfo(AppearanceName.CustomizationFormHint, new AppearanceDefault(SystemColors.ControlText, Color.Empty)),
					new AppearanceDefaultInfo(AppearanceName.FilterPanel, new AppearanceDefault(SystemColors.ControlLight, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
					new AppearanceDefaultInfo(AppearanceName.Caption, GetCaptionAppearanceDefault()),
			};
			return result;
		}
		protected virtual AppearanceDefault GetCaptionAppearanceDefault() {
			return new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center);
		}
		protected virtual Color GridLineColor { get { return SystemColors.Control; } }
		protected virtual Color HeaderBackColor { get { return SystemColors.Control; } }
		protected virtual Color HeaderForeColor { get { return SystemColors.ControlText; } }
		protected virtual Color FooterCellColor { get { return SystemColors.ControlText; } }
		protected virtual Color TreeLineColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		public virtual Color SizeGripColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control); } }
		public virtual object IndicatorImageCollection { get { return fPaintLink.IndicatorImages; } }
		public virtual Size IndicatorImageSize { get { return fPaintLink.IndicatorImages.ImageSize; } }
		public HeaderObjectPainter HeaderPainter { get { return headerPainter; } }
		public HeaderObjectPainter BandPainter { get { return bandPainter; } } 
		public IndicatorObjectPainter IndicatorPainter { get { return indicatorPainter; } }
		public ObjectPainter OpenCloseButtonPainter { get { return openCloseButtonPainter; } }
		public virtual BorderPainter BorderPainter { get { return BorderHelper.GetPainter(fPaintLink.BorderStyle, LookAndFeel);; } }
		public FooterPanelPainter FooterPanelPainter { get { return footerPanelPainter; } }
		public FooterCellPainter FooterCellPainter { get { return footerCellPainter; } }
		public ObjectPainter SortedShapePainter { get { return sortedShapePainter; } }
		public CheckObjectPainter CheckPainter { get { return checkPainter; } }
		public ObjectPainter SpecialRowSeparatorPainter { get { return specialRowSeparatorPainter; } }
		public TreeListFilterPanelPainter FilterPanel { get { return filterPanel; } }
		public ObjectPainter SmartFilterButton { get { return smartFilterButton; } }
		public ObjectPainter FilterButton { get { return filterButton; } }
		public ObjectPainter CaptionPainter { get { return captionPainter; } }
		public virtual bool IsSkin { get { return false; } }
	}
	public class FlatElementPainters : ElementPainters {
		public FlatElementPainters(PaintLink paintLink) : base(paintLink) {}
		protected override IndicatorObjectPainter CreateIndicatorPainter() {
			return new IndicatorObjectPainter(new FlatButtonObjectPainter());
		}
	}
	public class UltraFlatElementPainters : ElementPainters {
		public UltraFlatElementPainters(PaintLink paintLink) : base(paintLink) {}
		protected override IndicatorObjectPainter CreateIndicatorPainter() {
			return new UltraFlatIndicatorObjectPainter();
		}
		protected override ObjectPainter CreateSpecialRowSeparatorPainter() {
			return new TreeListSpecialRowSeparatorUltraFlatPainter();
		}
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { 
			return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(BorderStyles.UltraFlat)); 
		} 
	}
	public class Style3DElementPainters : ElementPainters {
		public Style3DElementPainters(PaintLink paintLink) : base(paintLink) {}
		protected override IndicatorObjectPainter CreateIndicatorPainter() {
			return new Style3DIndicatorObjectPainter();
		}
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(BorderStyles.Style3D)); } 
 	}
	public class XPElementPainters : ElementPainters {
		public XPElementPainters(PaintLink paintLink) : base(paintLink) {}
		protected override IndicatorObjectPainter CreateIndicatorPainter() {
			return new XPIndicatorObjectPainter();
		}
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { 
			return new GridFilterButtonPainter(new WindowsXPEditorButtonPainter()); 
		} 
	}
	public class Office2003ElementPainters : ElementPainters {
		public Office2003ElementPainters(PaintLink paintLink) : base(paintLink) {}
		protected override IndicatorObjectPainter CreateIndicatorPainter() {
			return new Office2003IndicatorObjectPainter();
		}
		protected override ObjectPainter CreateSpecialRowSeparatorPainter() {
			return new TreeListSpecialRowSeparatorOffice2003Painter(); 
		}
		protected override TreeListFilterPanelPainter CreateFilterPanelPainter() { 
			return new Office2003TreeListFilterPanelPainter(); 
		}
		protected override ObjectPainter CreateHeaderSmartFilterButtonPainter() { 
			return new GridSmartOffice2003FilterButtonPainter(); 
		}
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { 
			return new Office2003GridFilterButtonPainter(); 
		} 
		AppearanceDefault Get(Office2003GridAppearance app) {
			return Office2003Colors.Default[app].Clone() as AppearanceDefault;
		}
		protected override AppearanceDefault GetCaptionAppearanceDefault() {
			AppearanceDefault res = Get(Office2003GridAppearance.Header);
			res.HAlignment = HorzAlignment.Center;
			return res;
		}
		protected override Color HeaderForeColor { get { return Office2003Colors.Default[Office2003Color.Text]; } }
		protected override Color HeaderBackColor { get { return Get(Office2003GridAppearance.Header).BackColor; } }
		protected override Color GridLineColor { get { return Get(Office2003GridAppearance.GridLine).BackColor; } }
	}
	public class SkinElementPainters : ElementPainters {
		BorderPainter borderPainter;
		public SkinElementPainters(PaintLink paintLink) : base(paintLink) {
			this.borderPainter = CreateBorderPainter();
		}
		protected override HeaderObjectPainter CreateHeaderPainter() {
			return new SkinHeaderObjectPainter(LookAndFeel);
		}
		protected override HeaderObjectPainter CreateBandPainter() {
			return new SkinHeaderObjectPainter(LookAndFeel);
		}
		protected override IndicatorObjectPainter CreateIndicatorPainter() {
			return new SkinIndicatorObjectPainter(LookAndFeel);
		}
		protected override ObjectPainter CreateOpenCloseButtonPainter() {
			return new SkinOpenCloseButtonObjectPainter(LookAndFeel);
		}
		protected override FooterPanelPainter CreateFooterPanelPainter() {
			return new SkinFooterPanelPainter(LookAndFeel);
		}
		protected override FooterCellPainter CreateFooterCellPainter() {
			return new SkinFooterCellPainter(LookAndFeel);
		}
		protected override ObjectPainter CreateSortedShapePainter() {
			return new SkinSortedShapeObjectPainter(LookAndFeel);
		}
		protected virtual BorderPainter CreateBorderPainter() { 
			return new TreeListSkinBorderPainter(LookAndFeel);
		}
		protected override ObjectPainter CreateSpecialRowSeparatorPainter() {
			return new SkinTreeListSpecialRowSeparatorPainter(LookAndFeel);
		}
		protected override TreeListFilterPanelPainter CreateFilterPanelPainter() { 
			return new SkinTreeListFilterPanelPainter(LookAndFeel); 
		}
		protected override ObjectPainter CreateHeaderSmartFilterButtonPainter() { 
			return new GridSmartSkinFilterButtonPainter(LookAndFeel); 
		}
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { 
			return new SkinGridFilterButtonPainter(LookAndFeel); 
		}
		protected override ObjectPainter CreateCaptionPainter() {
			return new SkinTreeListCaptionPainter(fPaintLink.TreeList);
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefault() {
			AppearanceDefaultInfo[] result = new AppearanceDefaultInfo[] {
					new AppearanceDefaultInfo(AppearanceName.HeaderPanel, UpdateAppearanceEx(GridSkins.SkinHeader, new AppearanceDefault(HeaderForeColor, HeaderBackColor, HorzAlignment.Near, VertAlignment.Center))),
					new AppearanceDefaultInfo(AppearanceName.HeaderPanelBackground, UpdateAppearanceEx(GridSkins.SkinHeader, new AppearanceDefault(HeaderForeColor, HeaderBackColor, HorzAlignment.Near, VertAlignment.Center))),
					new AppearanceDefaultInfo(AppearanceName.BandPanel, UpdateAppearanceEx(GridSkins.SkinHeader, new AppearanceDefault(HeaderForeColor, HeaderBackColor, HorzAlignment.Near, VertAlignment.Center))),
					new AppearanceDefaultInfo(AppearanceName.FooterPanel, UpdateAppearanceEx(GridSkins.SkinFooterPanel, new AppearanceDefault(FooterCellColor, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center))),
					new AppearanceDefaultInfo(AppearanceName.Row, UpdateAppearanceEx(GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window))),
					new AppearanceDefaultInfo(AppearanceName.EvenRow, UpdateAppearanceEx(GridSkins.SkinGridEvenRow, new AppearanceDefault(Color.Empty, Color.Empty))),
					new AppearanceDefaultInfo(AppearanceName.OddRow, UpdateAppearanceEx(GridSkins.SkinGridOddRow, new AppearanceDefault(Color.Empty, Color.Empty))),
					new AppearanceDefaultInfo(AppearanceName.HorzLine, UpdateAppearanceEx(GridSkins.SkinGridLine, new AppearanceDefault(GridLineColor, GridLineColor))),
					new AppearanceDefaultInfo(AppearanceName.VertLine, UpdateAppearanceEx(GridSkins.SkinGridLine, new AppearanceDefault(GridLineColor, GridLineColor))),
					new AppearanceDefaultInfo(AppearanceName.Preview,  UpdateAppearanceEx(GridSkins.SkinGridPreview, new AppearanceDefault(Color.Blue, SystemColors.Window, HorzAlignment.Near, VertAlignment.Top))),
					new AppearanceDefaultInfo(AppearanceName.FocusedRow, UpdateSystemColors(new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight))),
					new AppearanceDefaultInfo(AppearanceName.FocusedCell, UpdateAppearanceEx(GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window))),
					new AppearanceDefaultInfo(AppearanceName.GroupButton, UpdateSystemColors(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control))),
					new AppearanceDefaultInfo(AppearanceName.TreeLine, new AppearanceDefault(TreeLineColor, TreeLineColor)),
					new AppearanceDefaultInfo(AppearanceName.GroupFooter, UpdateAppearanceEx(GridSkins.SkinFooterPanel, new AppearanceDefault(FooterCellColor, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center))),
					new AppearanceDefaultInfo(AppearanceName.Empty, UpdateAppearanceEx(GridSkins.SkinGridEmptyArea, new AppearanceDefault(SystemColors.Window, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
					new AppearanceDefaultInfo(AppearanceName.SelectedRow, UpdateSystemColors(new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight))),
					new AppearanceDefaultInfo(AppearanceName.HideSelectionRow, UpdateSystemColors(new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption))),
					new AppearanceDefaultInfo(AppearanceName.FixedLine, UpdateAppearanceEx(GridSkins.SkinGridFixedLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDarkDark))),
					new AppearanceDefaultInfo(AppearanceName.CustomizationFormHint, UpdateAppearanceEx(CommonSkins.SkinLabel, new AppearanceDefault(SystemColors.ControlText, Color.Empty))),
					new AppearanceDefaultInfo(AppearanceName.FilterPanel, UpdateAppearance(GridSkins.SkinGridFilterPanel, new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLight, LinearGradientMode.ForwardDiagonal, HorzAlignment.Near, VertAlignment.Center))),
					new AppearanceDefaultInfo(AppearanceName.Caption, UpdateAppearanceEx(GridSkins.SkinViewCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center))),
				};
			return result;
		}
		protected AppearanceDefault UpdateAppearance(string elementName, AppearanceDefault info) {
			SkinElement element = Skin[elementName] ??  CommonSkins.GetSkin(LookAndFeel)[elementName];
			if(element == null) return info;
			if(element.Color.GetBackColor() != Color.Empty) {
				info.BackColor = element.Color.GetBackColor();
				info.BackColor2 = element.Color.GetBackColor2();
				info.GradientMode = element.Color.GradientMode;
			}
			if(element.Color.FontBold) {
				info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
			}
			if(element.Color.GetForeColor() != Color.Empty) {
				info.ForeColor = element.Color.GetForeColor();
			}
			return info;
		}
		protected AppearanceDefault UpdateSystemColors(AppearanceDefault info) {
			info.ForeColor = CommonSkins.GetSkin(LookAndFeel).TranslateColor(info.ForeColor);
			info.BackColor = CommonSkins.GetSkin(LookAndFeel).TranslateColor(info.BackColor);
			return info;
		}
		protected AppearanceDefault UpdateAppearanceEx(string elementName, AppearanceDefault info) {
			info = UpdateSystemColors(info);
			return UpdateAppearance(elementName, info);
		}
		protected override Color GridLineColor { get { return Skin[GridSkins.SkinGridLine].Color.BackColor; } }
		protected Skin Skin { get { return GridSkins.GetSkin(LookAndFeel); } }
		ImageCollection GetIndicatorImageCollection() {
			SkinElement element = Skin[GridSkins.SkinIndicatorImages];
			if(element != null && element.Image != null) return element.Image.GetImages();
			return null;
		}
		public override object IndicatorImageCollection {
			get {
				object result = GetIndicatorImageCollection();
				return (result == null ? base.IndicatorImageCollection : result); 
			}
		}
		public override Size IndicatorImageSize { 
			get {
				ImageCollection ic = IndicatorImageCollection as ImageCollection;
				if(ic != null) return ic.ImageSize;
				return base.IndicatorImageSize;
			}
		}
		public override BorderPainter BorderPainter { 
			get {
				if(fPaintLink.BorderStyle == BorderStyles.NoBorder) return base.BorderPainter;
				return borderPainter;
			} 
		}
		public override bool IsSkin { get { return true; } }
	}
	public class TreeListSkinBorderPainter : SkinBorderPainter {
		public TreeListSkinBorderPainter(ISkinProvider provider) : base(provider) {}
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			return new SkinElementInfo(GridSkins.GetSkin(Provider)[GridSkins.SkinBorder]);
		}
	}
	#region special row separator
	public class TreeListSpecialTopSeparatorPainter : StyleObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			AppearanceObject header = GetStyle(e);
			AppearanceObject app = new AppearanceObject();
			app.GradientMode = LinearGradientMode.Vertical;
			app.BackColor = ControlPaint.Light(header.BackColor);
			app.BackColor2 = ControlPaint.Dark(header.BackColor);
			app.DrawBackground(e.Cache, e.Bounds);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) { return new Rectangle(0, 0, 10, 5); }
	}
	public class TreeListSpecialRowSeparatorUltraFlatPainter : TreeListSpecialTopSeparatorPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			new GridUltraFlatButtonPainter().DrawObject(e);
		}
	}
	public class TreeListSpecialRowSeparatorOffice2003Painter : TreeListSpecialTopSeparatorPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			AppearanceObject header = GetStyle(e);
			if(header.BackColor != SystemColors.Control) {
				base.DrawObject(e);
				return;
			}
			AppearanceObject app = new AppearanceObject();
			app.GradientMode = LinearGradientMode.Vertical;
			app.BackColor = Office2003Colors.Default[Office2003Color.Header];
			app.BackColor2 = Office2003Colors.Default[Office2003Color.Header2];
			app.DrawBackground(e.Cache, e.Bounds);
		}
	}
	public class SkinTreeListSpecialRowSeparatorPainter : TreeListSpecialTopSeparatorPainter {
		ISkinProvider provider;
		public SkinTreeListSpecialRowSeparatorPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, UpdateInfo(e));
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return ObjectPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, UpdateInfo(e));
		}
		SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(GridSkins.GetSkin(this.provider)[GridSkins.SkinGridSpecialRowIndent], e.Bounds);
			info.Cache = e.Cache;
			return info;
		}
	}
	#endregion
	#region filter panel
	public class TreeListFilterPanelInfoArgs : FilterPanelInfoArgsBase { }
	public class TreeListFilterPanelPainter : FilterPanelPainterBase {
		public TreeListFilterPanelPainter(EditorButtonPainter buttonPainter, CheckObjectPainter activeButtonPainter)
			: base(buttonPainter, activeButtonPainter) {
		}
	}
	public class Office2003TreeListFilterPanelPainter : TreeListFilterPanelPainter {
		public Office2003TreeListFilterPanelPainter() : base(new Office2003FilterPanelButtonPainter(), new Office2003CheckObjectPainter()) { }
		protected override ObjectPainter CreatePanelPainter() {
			return new Office2003FooterPanelObjectPainter();
		}
		public override void DrawBackground(ObjectInfoArgs e) {
			PanelPainter.DrawObject(e);
		}
	}
	public class Office2003FilterPanelButtonPainter : EditorButtonPainter {
		public Office2003FilterPanelButtonPainter() : base(EditorButtonHelper.GetPainter(BorderStyles.Office2003)) { }
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			Rectangle r = e.Bounds;
			if(e.State == ObjectState.Normal) {
				e.Paint.DrawRectangle(e.Graphics, SystemPens.Highlight, r);
			}
		}
	}
	public class SkinTreeListFilterPanelPainter : TreeListFilterPanelPainter {
		ISkinProvider provider;
		public SkinTreeListFilterPanelPainter(ISkinProvider provider)
			: base(new SkinEditorButtonPainter(provider), new SkinCheckObjectPainter(provider)) {
			this.provider = provider;
		}
		protected override ObjectPainter CreatePanelPainter() {
			return SkinElementPainter.Default;
		}
		protected override ObjectInfoArgs UpdateInfo(ObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(GridSkins.GetSkin(this.provider)[GridSkins.SkinGridFilterPanel], e.Bounds);
			info.Cache = e.Cache;
			return info;
		}
		public override void DrawBackground(ObjectInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, UpdateInfo(e));
		}
	}
	#endregion
#region view caption
	public class TreeListCaptionPainter : StyleObjectPainter {
		protected const int DefaultIndent = 3;
		public TreeListCaptionPainter(TreeList treeList) {
			TreeList = treeList;
		}
		protected TreeList TreeList { get; private set; }
		protected UserLookAndFeel ElementsLookAndFeel { get { return TreeList.ElementsLookAndFeel; } }
		protected virtual int GetIndent(ObjectInfoArgs e) { return DefaultIndent;  }
		protected virtual ObjectPainter GetBackgroundPainter(ObjectInfoArgs e) {
			return LookAndFeelPainterHelper.GetPainter(ElementsLookAndFeel, ElementsLookAndFeel.ActiveStyle).Button;
		}
		protected virtual string GetCaption() { return TreeList.Caption; }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			ObjectPainter painter = GetBackgroundPainter(e);
			Rectangle rect = painter.GetObjectClientRectangle(e);
			rect.Height = CalcTextSize((StyleObjectInfoArgs)e, rect.Width).Height + 2 * GetIndent(e);
			return painter.CalcBoundsByClientRectangle(e, rect);
		}
		protected Size CalcTextSize(StyleObjectInfoArgs ee, int maxWidth) {
			return StringPainter.Default.Calculate(ee.Graphics, ee.Appearance, GetCaption(), maxWidth).Bounds.Size;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ObjectInfoArgs ee = UpdateObjectInfo(e);
			DrawBackground(ee);
			DrawText(e);
		}
		protected virtual ObjectInfoArgs UpdateObjectInfo(ObjectInfoArgs e) { return e; }
		protected virtual void DrawBackground(ObjectInfoArgs e) {
			GetBackgroundPainter(e).DrawObject(e);
		}
		protected virtual void DrawText(ObjectInfoArgs e) {
			StringPainter.Default.DrawString(e.Cache, ((StyleObjectInfoArgs)e).Appearance, GetCaption(), GetObjectClientRectangle(e));
		}
	}
	public class SkinTreeListCaptionPainter : TreeListCaptionPainter {
		public SkinTreeListCaptionPainter(TreeList treeList) : base(treeList) { }
		protected override ObjectPainter GetBackgroundPainter(ObjectInfoArgs e) {
			if(e is SkinElementInfo) return SkinElementPainter.Default;
			return base.GetBackgroundPainter(e);
		}
		protected override ObjectInfoArgs UpdateObjectInfo(ObjectInfoArgs e) {
			SkinElement element = GridSkins.GetSkin(ElementsLookAndFeel)[GridSkins.SkinViewCaption];
			if(element == null) return base.UpdateObjectInfo(e);
			return new SkinElementInfo(element, e.Bounds) { Cache = e.Cache };
		}
		protected override int GetIndent(ObjectInfoArgs e) {
			if(e is SkinElementInfo) return 0;
			return base.GetIndent(e);
		}
	}
#endregion
}
