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
using System.Collections;
using DevExpress.LookAndFeel;
using DevExpress.Data;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using System.Windows.Forms;
using DevExpress.Utils.Text;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Skins;
namespace DevExpress.XtraGrid.Drawing {
	public class DrawOutlookButtonArgs {
		public Graphics  Graphics;
		public Rectangle Rect;
		public bool	  SimpleStyle;
		public bool		 Disabled;
		public bool		 Collapsed;
		public Brush	 BackBrush, ForeBrush;
		public DrawOutlookButtonArgs(Graphics g) : this(g, Rectangle.Empty, false, SystemBrushes.Control, SystemBrushes.ControlText) {}
		public DrawOutlookButtonArgs(Graphics g, Rectangle rect, bool collapsed,
			Brush backBrush, Brush foreBrush) : 
			this(g, rect, collapsed, false, false, backBrush, foreBrush) { }
		public DrawOutlookButtonArgs(Graphics g, Rectangle rect, Brush backBrush, bool collapsed, bool simpleStyle) :
			this(g, rect, collapsed, simpleStyle, false) { }
		public DrawOutlookButtonArgs(Graphics g, Rectangle rect, bool collapsed, 
			bool simpleStyle, bool disabled) : 
			this(g, rect, collapsed, simpleStyle, disabled, SystemBrushes.Control, SystemBrushes.ControlText) {}
		public DrawOutlookButtonArgs(Graphics g, Rectangle rect, bool collapsed, 
			bool simpleStyle, bool disabled, Brush backBrush, Brush foreBrush) {
			Graphics = g;
			Rect = rect;
			Collapsed = collapsed;
			SimpleStyle = simpleStyle;
			Disabled = disabled;
			BackBrush = backBrush;
			ForeBrush = foreBrush;
		}
	}
	public class DetailButtonObjectInfoArgs : ObjectInfoArgs {
		bool expanded;
		public DetailButtonObjectInfoArgs() : this(Rectangle.Empty, false, true) { }
		public DetailButtonObjectInfoArgs(Rectangle bounds, bool expanded, bool enabled) {
			this.Bounds = bounds;
			this.expanded = expanded;
			State = enabled ? ObjectState.Normal : ObjectState.Disabled;
		}
		public bool Expanded { get { return expanded; } set { expanded = value; } }
	}
	public class DetailButtonObjectPainter : ObjectPainter {
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 11, 11);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DetailButtonObjectInfoArgs ee = e as DetailButtonObjectInfoArgs;
			Brush plusBrush, fillColor;
			Rectangle r = e.Bounds;
			Graphics g = e.Graphics;
			Pen borderPen;
			if(e.State != ObjectState.Disabled) {
				plusBrush = SystemBrushes.ControlLightLight;
				fillColor = SystemBrushes.ControlLight;
				borderPen = SystemPens.ControlLightLight;
			}
			else {
				plusBrush = SystemBrushes.ControlDarkDark;
				fillColor = SystemBrushes.ControlLightLight;
				borderPen = SystemPens.ControlDark;
			}
			XPaint.Graphics.DrawRectangle(g, borderPen, r);
			r.Inflate(-1, -1);
			e.Cache.Paint.FillRectangle(g, fillColor, r);
			e.Cache.Paint.FillRectangle(g, plusBrush, new Rectangle(r.Left + 2, r.Top + 4, 5, 1));
			if(!ee.Expanded) 
				e.Cache.Paint.FillRectangle(g, plusBrush, new Rectangle(r.Left + 4, r.Top + 2, 1, 5));
		}
	}
	public class GridDrawing {
		public static bool PtInRect(Rectangle r, Point pt) {
			return (!r.IsEmpty && r.Contains(pt));
		}
	}
	public class GridButtonPainter : StyleObjectPainter {
		ObjectPainter buttonPainter;
		public GridButtonPainter(ObjectPainter btPainter) {
			this.buttonPainter = btPainter;
		}
		public virtual ObjectPainter ButtonPainter { get { return buttonPainter; } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return ButtonPainter.GetObjectClientRectangle(e);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return ButtonPainter.CalcObjectBounds(e);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return ButtonPainter.CalcBoundsByClientRectangle(e, client);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return ButtonPainter.CalcObjectMinBounds(e);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ButtonPainter.DrawObject(e);
		}
	}
	public class GridColumnButtonPainter : GridButtonPainter {
		public GridColumnButtonPainter(ObjectPainter painter) : base(painter) {}
	}
	public class SkinGridSpecialTopRowIndentPainter : GridSpecialTopRowIndentPainter {
		ISkinProvider provider;
		public SkinGridSpecialTopRowIndentPainter(ISkinProvider provider) {
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
	public class SkinGridGroupRowPainter : GridGroupRowPainter {
		ISkinProvider provider;
		public SkinGridGroupRowPainter(ISkinProvider provider, GridSkinElementsPainter painter)
			: base(painter) {
				this.provider = provider;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle r = base.CalcObjectMinBounds(e);
			return ObjectPainter.CalcBoundsByClientRectangle(e.Graphics, SkinElementPainter.Default, UpdateInfo(e), r);
		}
		public override Rectangle CalcObjectDefaultBounds(ObjectInfoArgs e) {
			Rectangle r = base.CalcObjectDefaultBounds(e);
			return ObjectPainter.CalcBoundsByClientRectangle(e.Graphics, SkinElementPainter.Default, UpdateInfo(e), r);
		}
		SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(GridSkins.GetSkin(this.provider)[GridSkins.SkinGridGroupRow], e.Bounds);
			info.RightToLeft = ((GridGroupRowInfo)e).RightToLeft;
			info.Cache = e.Cache;
			return info;
		}
	}
	public class SkinGridDetailButtonPainter : SkinCustomPainter {
		public SkinGridDetailButtonPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			DetailButtonObjectInfoArgs ee = (DetailButtonObjectInfoArgs)e;
			SkinElementInfo res = new SkinElementInfo(GridSkins.GetSkin(Provider)[GridSkins.SkinPlusMinusEx]);
			int delta = ee.State == ObjectState.Disabled ? 0 : 2;
			res.ImageIndex = delta + (ee.Expanded ? 1 : 0);
			return res;
		}
	}
	public class SkinGridViewCaptionPainter : GridViewCaptionPainter {
		public SkinGridViewCaptionPainter(BaseView view) : base(view) { }
		protected override ObjectPainter GetBackgroundPainter(ObjectInfoArgs e) {
			if(e is SkinElementInfo) return SkinElementPainter.Default;
			return base.GetBackgroundPainter(e);
		}
		protected override ObjectInfoArgs UpdateInfo(ObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(GridSkins.GetSkin(ElementsLookAndFeel)[GridSkins.SkinViewCaption], e.Bounds);
			if(info.Element == null) return e;
			info.Cache = e.Cache;
			return info;
		}
		protected override Size GetIndent(ObjectInfoArgs e) {
			if(e is SkinElementInfo) return Size.Empty;
			return base.GetIndent(e);
		}
	}
	public class SkinGridFilterPanelPainter : GridFilterPanelPainter {
		ISkinProvider provider;
		public SkinGridFilterPanelPainter(ISkinProvider provider) : base(new SkinEditorButtonPainter(provider), new SkinCheckObjectPainter(provider)) { 
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
		public override  void DrawBackground(ObjectInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, UpdateInfo(e));
		}
	}
	public enum GridColumnLocation { Left, Center, Right, Single};
	public class GridColumnInfo {
		bool allowBottomBorder, allowRightBorder, bottomColumn, validateCoord, allowEffects, inGroup;
		int rowCount, startRow;
		int cellIndex;
		GridColumnLocation location;
		HorzAlignment defaultValueAlignment;
		GridColumn column;
		public GridColumnInfo(GridColumn column) {
			this.column = column;
			this.bottomColumn = true;
			this.cellIndex = -1;
			this.allowBottomBorder = this.allowRightBorder = true;
			this.startRow = 0;
			this.rowCount = 1;
			this.allowEffects = true;
			this.inGroup = false;
			this.defaultValueAlignment = HorzAlignment.Default;
			this.validateCoord = true;
			this.location = GridColumnLocation.Center;
		}
		public GridColumnLocation Location { get { return location; } set { location = value; } }
		public int CellIndex { get { return cellIndex; } set { cellIndex = value; } }
		public int StartRow { get { return startRow; } set { startRow = value; } }
		public int RowCount { get { return rowCount; } set { rowCount = value; } }
		public bool BottomColumn { get { return bottomColumn; } set { bottomColumn = value; } }
		public bool AllowBottomBorder { get { return allowBottomBorder; } set { allowBottomBorder = value; } }
		public bool AllowRightBorder { get { return allowRightBorder; } set { allowRightBorder = value; } }
		public bool AllowEffects { get { return allowEffects; } set { allowEffects = value; } }
		public bool ValidateCoord { get { return validateCoord; } set { validateCoord = value; } }
		public bool InGroup { get { return inGroup; } set { inGroup = value; } }
		public GridColumn Column { 
			get { return column; } 
			set { column = value; }
		}
		public HorzAlignment DefaultValueAlignment {
			get { 
				if(defaultValueAlignment == HorzAlignment.Default) {
					if(Column != null) defaultValueAlignment = Column.DefaultValueAlignment;
				}
				return defaultValueAlignment; 
			}
			set { defaultValueAlignment = value; }
		}
	}
	public class GridColumnInfoArgs : HeaderObjectInfoArgs {
		GridColumn column;
		GridColumnInfoType type;
		GridColumnInfo info;
		object tag;
		bool customizationForm;
		public GridColumnInfoArgs(GridColumn column) : this(null, column) { }
		public GridColumnInfoArgs(GraphicsCache cache, GridColumn column) { 
			this.customizationForm = false;
			this.Cache = cache;
			this.tag = null;
			this.type = GridColumnInfoType.Column;
			this.column = column;
			this.info = new GridColumnInfo(column);
			CreateInnerCollection();
			UpdateCaption();
		}
		void UpdateHtmlDrawInfo() {
			GridView view = (column == null ? null : column.View) as GridView;
			if(view == null) return;
			UseHtmlTextDraw = view.OptionsView.AllowHtmlDrawHeaders;
		}
		protected internal void UpdateCaption() {
			UpdateHtmlDrawInfo();
			if(Column == null) {
				Caption = string.Empty;
				return;
			}
			if(CustomizationForm) 
				Caption = Column.GetCustomizationCaption();  
			else
				Caption = Column.OptionsColumn.ShowCaption ? Column.GetCaption() : "";
		}
		public bool CustomizationForm { 
			get { return customizationForm; } 
			set { 
				customizationForm = value; 
				UpdateCaption();
			} 
		}
		protected GridView View { 
			get { 
				if(Column == null) return null;
				return Column.View as GridView;
			}
		}
		protected virtual GridElementsPainter ElementsPainter {
			get { 
				if(View == null || View.Painter == null) return null;
				return (View.Painter as GridPainter).ElementsPainter;
			}
		}
		public Rectangle TrueBounds { get; set; }
		public virtual void CreateInnerCollection() {
			InnerElements.Clear();
			if(Column == null || View == null) return;
			if(View.OptionsView.AllowGlyphSkinning)
				InnerElements.Add(new DrawElementInfo(new SkinnedGlyphElementPainter(), new SkinnedGlyphElementInfoArgs(Column.Images, Column.ImageIndex, Column.Image), Column.ImageAlignment));
			else
				InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), new GlyphElementInfoArgs(Column.Images, Column.ImageIndex, Column.Image), Column.ImageAlignment));
			if(Column.SortOrder != ColumnSortOrder.None) {
				InnerElements.Add(ElementsPainter.SortedShape, new SortedShapeObjectInfoArgs());
			}
			if(View.CanShowFilterButton(Column)) {
				if(View.OptionsView.GetHeaderFilterButtonShowMode() == FilterButtonShowMode.SmartTag) {
					DrawElementInfo di = new DrawElementInfo(ElementsPainter.SmartFilterButton, new GridFilterButtonInfoArgs(), StringAlignment.Far);
					di.ElementInterval = 0;
					di.RequireTotalBounds = true;
					InnerElements.Add(di);
				}
				else {
					InnerElements.Add(ElementsPainter.FilterButton, new GridFilterButtonInfoArgs());
				}
			}
			Caption = Column.GetCaption();
		}
		public GridColumnInfo Info { get { return info; } }
		public virtual object Tag { get { return tag; } set { tag = value; } }
		public GridColumnInfoType Type {
			get { return type; }
			set { type = value; }
		}
		public virtual GridColumn Column { 
			get { return column; } 
			set { 
				column = value; 
				info.Column = value;
			}
		}
		public bool IsEquals(object obj) {
			GridColumnInfoArgs args = obj as GridColumnInfoArgs;
			if(args == null) return false;
			if(args == this) return true;
			if(args.Column != this.Column) return false;
			return args.Bounds.Y == this.Bounds.Y;
		}
	}
	public class GridFilterPanelInfoArgs : FilterPanelInfoArgsBase { }
	public class GridFilterPanelPainter : FilterPanelPainterBase {
		public GridFilterPanelPainter(EditorButtonPainter buttonPainter, CheckObjectPainter activeButtonPainter) : base(buttonPainter, activeButtonPainter) {
		}
	}
	public class GridObjectPainter : ObjectPainter {
		GridElementsPainter elementsPainter;
		public GridObjectPainter(GridElementsPainter elementsPainter) {
			this.elementsPainter = elementsPainter;
		}
		public GridElementsPainter ElementsPainter { get { return elementsPainter; } }
	}
	public class GridGroupRowPainter : GridObjectPainter {
		const int Office2003GroupIndent = 18;
		public GridGroupRowPainter(GridElementsPainter elementsPainter) : base(elementsPainter) {
		}
		protected virtual bool GetIsOffice2003Mode(ObjectInfoArgs e) {
			GridRowInfo ri = e as GridRowInfo;
			if(ri != null) return ri.ViewInfo.GetGroupDrawMode() == GroupDrawMode.Office2003;
			return false;
		}
		protected virtual bool GetIsOfficeMode(ObjectInfoArgs e) {
			GridRowInfo ri = e as GridRowInfo;
			if(ri != null) return ri.ViewInfo.GetGroupDrawMode() == GroupDrawMode.Office;
			return false;
		}
		public virtual Rectangle CalcObjectDefaultBounds(ObjectInfoArgs e) {
			Rectangle res = CalcObjectMinBounds(e);
			if(GetIsOffice2003Mode(e)) {
				res.Height += Office2003GroupIndent;
			}
			return res;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			GridGroupRowInfo info = e as GridGroupRowInfo;
			info.CreateEditorInfo(Rectangle.Empty, Color.Black);
			Rectangle res = ObjectPainter.CalcObjectMinBounds(e.Graphics, info.CreatePainter(), info.EditorInfo);
			if(info.SelectorInfo != null) res.Height = Math.Max(res.Height, ObjectPainter.CalcObjectMinBounds(e.Graphics, info.SelectorPainter, info.SelectorInfo).Height);
			res.Height += 2; 
			return res;
		}
		public virtual int CalcLevelIndent(GridRowInfo ri, int level) {
			if(GetIsOffice2003Mode(ri) || GetIsOfficeMode(ri)) {
				if(!ri.IsGroupRow) {
					if(level < 2) return 0;
					level --;
				}
			}
			return level * ri.ViewInfo.LevelIndent;
		}
		public virtual AppearanceObject GetGroupColorAppearance(ObjectInfoArgs e) {
			GridGroupRowInfo ee = e as GridGroupRowInfo;
			AppearanceObject appearance = ee.Appearance;
			if(GetIsOffice2003Mode(e) && (ee.RowState & (GridRowCellState.Focused | GridRowCellState.Selected)) == 0) {
				appearance = ee.ViewInfo.PaintAppearance.Row;
			}
			return appearance;
		}
		[ThreadStatic]
		static Image defaultGroupRowMoreRowsIcon;
		protected static Image DefaultGroupRowMoreRowsIcon {
			get {
				if(defaultGroupRowMoreRowsIcon == null) {
					defaultGroupRowMoreRowsIcon = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraGrid.Images.GroupRowMoreRows.png", typeof(GridGroupRowPainter).Assembly);
				}
				return defaultGroupRowMoreRowsIcon;
			}
		}
		protected virtual Image GetGroupRowMoreRowsIcon(GridGroupRowInfo e) {
			return DefaultGroupRowMoreRowsIcon;
		}
		public virtual void DrawGroupRowMoreRowsIcon(ObjectInfoArgs e) {
			GridGroupRowInfo ee = e as GridGroupRowInfo;
			if(!ee.DrawMoreIcons) return;
			Image i = GetGroupRowMoreRowsIcon(ee);
			Rectangle r = GetObjectClientRectangle(e);
			Rectangle image = RectangleHelper.GetCenterBounds(r, i.Size);
			if(ee.RightToLeft)
				image.X = r.X + 2;
			else
				image.X = r.Right - (image.Width + 2);
			e.Paint.DrawImage(e.Graphics, i, image);
		}
		public virtual void DrawGroupRowBackground(ObjectInfoArgs e) {
			GridGroupRowInfo ee = e as GridGroupRowInfo;
			AppearanceObject appearance = GetGroupColorAppearance(ee);
			appearance.FillRectangle(ee.Cache, ee.DataBounds);
		}
		public virtual void DrawGroupRowIndent(GridRowInfo e, GraphicsCache cache, AppearanceObject appearance, Rectangle bounds) {
			appearance.DrawBackground(cache, bounds);
		}
		public virtual int GetGridLineHeight(GridGroupRowInfo e) { 
			int res = GetIsOffice2003Mode(e) ? 2 : 1;
			if(e.ViewInfo.AllowPartialGroups && !e.GroupExpanded) return res + e.ViewInfo.GetGroupDividerHeight() - 1;
			return res;
		}
		protected virtual AppearanceObject GetHorzLineAppearance(GridGroupRowInfo e) {
			if(GetIsOffice2003Mode(e)) {
				if(e.ViewInfo.View.LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
					return new AppearanceObject(new AppearanceDefault(Color.Empty, Office2003Colors.Default[Office2003Color.GroupRowSeparatorEx]));
			}
			if(e.ViewInfo.AllowPartialGroups && !e.GroupExpanded) return e.ViewInfo.GetGroupDividerAppearance();
			if(GetIsOfficeMode(e)) {
				return e.ViewInfo.PaintAppearance.Row;
			}
			return e.ViewInfo.PaintAppearance.HorzLine;
		}
		public virtual void CalcRowLines(GridGroupRowInfo e) {
			if(!e.ViewInfo.View.GetShowHorizontalLines()) return;
			int indent = GetIsOffice2003Mode(e) || GetIsOfficeMode(e) ? 0 : (e.GroupExpanded ? e.ViewInfo.LevelIndent - 1 : 0);
			bool rightToLeft = e.ViewInfo.IsRightToLeft;
			e.AddLineInfo(null, e.DataBounds.Left + (rightToLeft ? 0 : indent), e.DataBounds.Bottom - GetGridLineHeight(e),
				e.DataBounds.Width - indent, GetGridLineHeight(e), GetHorzLineAppearance(e));
			if(!GetIsOffice2003Mode(e) && !GetIsOfficeMode(e)) {
				if(indent > 0) {
					e.AddLineInfo(null, new Rectangle((rightToLeft ? e.DataBounds.Right  - indent: e.DataBounds.Left), e.DataBounds.Bottom - 1, indent, 1),
						e.ViewInfo.View.GetLevelStyle(e.Level, false), true);
				}
			}
			Rectangle r = e.DataBounds; r.Height -= GetGridLineHeight(e);
			e.DataBounds = r;
		}
		public virtual Rectangle GetGroupClientBounds(ObjectInfoArgs e) {
			GridGroupRowInfo ee = e as GridGroupRowInfo;
			Rectangle res = Rectangle.Inflate(ee.DataBounds, -1, -1);
			if(!ee.ViewInfo.ShowGroupButtons) { res.X += 2; res.Width -= 2; }
			if(GetIsOffice2003Mode(e)) {
				int indentDelta = Office2003GroupIndent;
				if(ee.ViewInfo.GroupRowDefaultHeight != ee.DataBounds.Height) {
					indentDelta += ee.DataBounds.Height - ee.ViewInfo.GroupRowDefaultHeight;
				}
				if(indentDelta > 0) {
					res.Height -= indentDelta;
					res.Y += indentDelta;
				}
			}
			return res;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			GridGroupRowInfo ee = e as GridGroupRowInfo;
			CalcRowLines(ee);
			Rectangle client = GetGroupClientBounds(e);
			if(ee.ViewInfo.ShowGroupButtons) {
				Rectangle buttonBounds = new Rectangle(client.X + 4, client.Top + (client.Height - ee.ViewInfo.PlusMinusButtonSize.Height) / 2,
					ee.ViewInfo.PlusMinusButtonSize.Width, ee.ViewInfo.PlusMinusButtonSize.Height);
				if(ee.RightToLeft) {
					buttonBounds.X = client.Right - (4 + buttonBounds.Width);
			}
				ee.ButtonBounds  = buttonBounds;
			}
			if(ee.View.IsShowCheckboxSelectorInGroupRow) {
				ee.SelectorInfo = CreateSelectorInfo();
				Rectangle sbounds = client;
				sbounds.Size = ObjectPainter.CalcObjectMinBounds(e.Graphics, ee.SelectorPainter, ee.SelectorInfo).Size;
				if(ee.ButtonBounds.Width > 0) {
					if(ee.RightToLeft)
						sbounds.X = ee.ButtonBounds.X - SelectorIndent - sbounds.Width;
					else
						sbounds.X = ee.ButtonBounds.Right + SelectorIndent;
				}
				sbounds.Y = RectangleHelper.GetCenterBounds(client, sbounds.Size).Y;
				ee.SelectorInfo.Bounds = sbounds;
				ObjectPainter.CalcObjectBounds(e.Graphics, ee.SelectorPainter, ee.SelectorInfo);
			}
			return ee.Bounds;
		}
		protected virtual int SelectorIndent { get { return 4; } }
		protected virtual GroupRowCheckboxSelectorInfoArgs CreateSelectorInfo() {
			return new GroupRowCheckboxSelectorInfoArgs();
		}
		public override void DrawObject(ObjectInfoArgs e) {
			GridGroupRowInfo ee = e as GridGroupRowInfo;
			Rectangle r = ee.DataBounds;
			bool drawFocusRect = ((ee.RowState & GridRowCellState.FocusedAndGridFocused) == GridRowCellState.FocusedAndGridFocused && ee.ViewInfo.View.FocusRectStyle != DrawFocusRectStyle.None); 
			if(drawFocusRect) {
				e.Cache.Paint.DrawFocusRectangle(e.Graphics, r, ee.Appearance.ForeColor, ee.Appearance.BackColor);
			}
			r = GetGroupClientBounds(e);
			if(!ee.ButtonBounds.IsEmpty) {
				ElementsPainter.OpenCloseButton.DrawObject(new OpenCloseButtonInfoArgs(e.Cache, ee.ButtonBounds, ee.GroupExpanded, ee.AppearanceGroupButton, ObjectState.Normal) { RightToLeft = ee.RightToLeft });
				if(ee.RightToLeft) {
					r.Width = (ee.ButtonBounds.X - 5) - r.X;
				}
				else {
				r.X = ee.ButtonBounds.Right + 5;
				r.Width = ee.Bounds.Right - r.X;
			}
			}
			if(ee.SelectorInfo != null && !ee.SelectorInfo.Bounds.IsEmpty) {
				CheckUpdateSelector(ee);
				ObjectPainter.DrawObject(e.Cache, ee.SelectorPainter, ee.SelectorInfo);
				if(ee.RightToLeft) {
					r.X = ee.Bounds.X;
					r.Width = (ee.SelectorInfo.Bounds.X - SelectorIndent) - r.X;
				}
				else {
					r.X = ee.SelectorInfo.Bounds.Right + SelectorIndent;
					r.Width = ee.Bounds.Right - r.X;
				}
			}
			ee.CreateEditorInfo(r, ee.Appearance.GetForeColor());
			GraphicsClipState state = null;
			if(ee.ViewInfo.IsAlignGroupRowSummariesUnderColumns) {
				var fi = ee.ViewInfo.ColumnsInfo.FirstNonGroupColumnInfo;
				if(fi != null) {
					int clipX = fi.Bounds.X;
					if(ee.ViewInfo.FixedLeftColumn != null) {
						fi = ee.ViewInfo.ColumnsInfo[ee.ViewInfo.FixedLeftColumn];
						if(fi != null) clipX = fi.Bounds.Right;
					}
					state = ee.Cache.ClipInfo.SaveAndSetClip(new Rectangle(ee.Bounds.X, ee.Bounds.Top, clipX - ee.Bounds.X, ee.Bounds.Height), false, false);
				}
			}
			ObjectPainter.DrawObject(e.Cache, ee.CreatePainter(), ee.EditorInfo);
			if(state != null) ee.Cache.ClipInfo.RestoreClipRelease(state);
			if(ee.ViewInfo.IsAlignGroupRowSummariesUnderColumns) {
				DrawAlignedGroupSummaries(e.Cache, ee);
			}
			DrawGroupRowMoreRowsIcon(e);
		}
		void DrawAlignedGroupSummaries(GraphicsCache cache, GridGroupRowInfo ee) {
			GridViewInfo viewInfo = ee.ViewInfo;
			Rectangle clipBounds = Rectangle.Empty;
			for(int n = 0; n < ee.ViewInfo.ColumnsInfo.Count; n++) {
				var col = viewInfo.ColumnsInfo[n];
				if(col.Column == null) continue;
				if(col.Column.GroupIndex >= 0) {
					if(ee.ViewInfo.FixedLeftColumn == col.Column) {
						Rectangle bounds = ee.Bounds;
						bounds.X = col.Bounds.Right;
						bounds.Width = ee.View.FixedLineWidth;
						viewInfo.PaintAppearance.FixedLine.FillRectangle(cache, bounds);
						clipBounds = new Rectangle(bounds.Right, ee.Bounds.Top, ee.Bounds.Right - bounds.Right, ee.Bounds.Height);
					}
					continue;
				}
				GraphicsClipState state = null;
				if(!clipBounds.IsEmpty) state = cache.ClipInfo.SaveAndSetClip(clipBounds, false, true);
				try {
					DrawAlignedSummaryCell(cache, ee, viewInfo, col);
				}
				finally {
					if(state != null) cache.ClipInfo.RestoreClipRelease(state);
				}
			}
		}
		protected virtual void DrawAlignedSummaryCell(GraphicsCache cache, GridGroupRowInfo ee, GridViewInfo viewInfo, GridColumnInfoArgs col) {
			AppearanceObject app = viewInfo.PaintAppearance.GroupFooter;
			Rectangle bounds = ee.Bounds;
			if(ee.View.GetShowVerticalLines()) {
				bounds.X = col.Bounds.X - 1;
				bounds.Width = 1;
				viewInfo.PaintAppearance.VertLine.FillRectangle(cache, bounds);
			}
			if(!ee.HasColumnSummaryItems) return;
			var item = ee.GetColumnSummaryItem(col.Column);
			if(item == null) return;
			bounds = viewInfo.GetGroupRowCellBounds(ee, col);
			Rectangle captionBounds = bounds;
			captionBounds.Inflate(-2, 0);
			captionBounds.X += viewInfo.CellPadding.Left;
			captionBounds.Width -= (viewInfo.CellPadding.Horizontal);
			var summaryText = item.SummaryItem.GetDisplayText(item.Value, false);
			var drawArgs = new RowGroupRowCellEventArgs(cache, bounds, captionBounds, ee, app.Clone() as AppearanceObject, summaryText, item.Value, item.SummaryItem);
			ee.View.RaiseCustomDrawGroupRowCell(drawArgs);
			if(!drawArgs.Handled) drawArgs.Appearance.DrawString(cache, drawArgs.DisplayText, captionBounds);
		}
		void CheckUpdateSelector(GridGroupRowInfo ee) {
			if(ee.SelectorInfo.IsDirty) {
				ee.UpdateSelectorState();
			}
		}
	}
	public class GridViewCaptionPainter : StyleObjectPainter {
		BaseView view;
		public GridViewCaptionPainter(BaseView view) {
			this.view = view;
		}
		protected UserLookAndFeel ElementsLookAndFeel { get { return view.ElementsLookAndFeel; } }
		protected virtual ObjectInfoArgs UpdateInfo(ObjectInfoArgs e) { return e; }
		protected string GetCaption(ObjectInfoArgs e) { return view.GetViewCaption(); }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			ObjectPainter painter = GetBackgroundPainter(e);
			Rectangle client = painter.GetObjectClientRectangle(e);
			string text = GetCaption(e);
			StyleObjectInfoArgs se = (StyleObjectInfoArgs)e;
			Rectangle bounds = new Rectangle(Point.Empty, CalcTextSize(se, client));
			bounds.Inflate(GetIndent(e));
			return painter.CalcBoundsByClientRectangle(e, bounds);
		}
		protected Size CalcTextSize(StyleObjectInfoArgs ee, Rectangle bounds) {
			return StringPainter.Default.Calculate(ee.Graphics, ee.Appearance, GetCaption(ee), bounds.Width).Bounds.Size;
		}
		protected virtual Size GetIndent(ObjectInfoArgs e) {
			return new Size(3, 3);
		}
		protected virtual ObjectPainter GetBackgroundPainter(ObjectInfoArgs e) {
			return LookAndFeelPainterHelper.GetPainter(ElementsLookAndFeel, ElementsLookAndFeel.ActiveStyle).Button;
		}
		public override void DrawObject(ObjectInfoArgs ee) {
			ObjectInfoArgs e = UpdateInfo(ee);
			ObjectPainter painter = GetBackgroundPainter(e);
			painter.DrawObject(e);
			Rectangle client = painter.GetObjectClientRectangle(e);
			client.Inflate(-GetIndent(e).Width, -GetIndent(e).Height);
			DrawText(ee, client);
		}
		protected virtual void DrawText(ObjectInfoArgs e, Rectangle client) {
			StyleObjectInfoArgs se = (StyleObjectInfoArgs)e;
			StringPainter.Default.DrawString(e.Cache, se.Appearance, GetCaption(e), client);
		}
	}
	public class GridRowPreviewPainter : GridObjectPainter {
		public const int PreviewTextIndent = 2, PreviewTextVIndent = 1;
		public GridRowPreviewPainter(GridElementsPainter elementsPainter) : base(elementsPainter) {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			GridDataRowInfo ee = e as GridDataRowInfo;
			Rectangle r = ee.PreviewBounds;
			if(r.IsEmpty) return;
			ee.AppearancePreview.DrawBackground(e.Cache, r);
			r.Inflate(-PreviewTextIndent, -PreviewTextVIndent);
			if(!View.IsRightToLeft) r.X += ee.PreviewIndent;
			r.Width -= ee.PreviewIndent;
			TextEditViewInfo te = new TextEditViewInfo(View.GetColumnDefaultRepositoryItem(null));
			te.RightToLeft = View.IsRightToLeft;
			te.SetDisplayText(ee.PreviewText);
			te.UseHighlightSearchAppearance = true;
			te.MatchedStringUseContains = true;
			te.MatchedString = string.Empty;
			te.PaintAppearance = ee.AppearancePreview;
			te.FillBackground = false;
			te.Bounds = r;
			te.SetMaskBoxRect(r);
			if(View.OptionsFind.SearchInPreview && View.OptionsFind.HighlightFindResults) {
				te.MatchedString = View.GetFindMatchedText(GridView.PreviewUnboundColumnName, ee.PreviewText);
			}
			te.Painter.Draw(new ControlGraphicsInfoArgs(te, e.Cache, r));
		}
		GridView View { get { return ElementsPainter.View; } }
	}
	public class GridFooterCellInfoArgs : FooterCellInfoArgs {
		GridColumnInfoArgs columnInfo;
		GridSummaryItem summaryItem;
		public GridFooterCellInfoArgs() : this(null) { }
		public GridFooterCellInfoArgs(GraphicsCache cache) : base(cache) {
			this.columnInfo = null;
		}
		public GridColumn Column { get { return ColumnInfo == null ? null : ColumnInfo.Column; } }
		public GridColumnInfoArgs ColumnInfo { get { return columnInfo; } set { columnInfo = value; } }
		public GridSummaryItem SummaryItem { get { return summaryItem; } set { summaryItem = value; } }
	}
	public class GridGroupFooterPanelPainter : FooterPanelPainter {
		public GridGroupFooterPanelPainter(ObjectPainter panelButtonPainter) : base(panelButtonPainter) {
		}
	}
	public class GridGroupFooterCellPainter : FooterCellPainter {
		public GridGroupFooterCellPainter(ObjectPainter cellBorderPainter) : base(cellBorderPainter) {
		}
	}
	public class GridBandInfoArgs : HeaderObjectInfoArgs {
		GridColumnsInfo columns;
		GridBandInfoCollection children;
		GridBand band;
		FixedStyle _fixed;
		GridColumnInfoType type;
		bool allowEffects, validateCoord, customizationForm;
		public GridBandInfoArgs(GridBand band) : this(band, null) { }
		public GridBandInfoArgs(GridBand band, GraphicsCache cache) {
			this.Cache = cache;
			this._fixed = FixedStyle.None;
			this.columns = null;
			this.children = null;
			this.allowEffects = true;
			this.band = band;
			this.customizationForm = false;
			this.type = GridColumnInfoType.Column;
			this.validateCoord = true;
			CreateInnerCollection();
			UpdateCaption();
		}
		void UpdateHtmlDrawInfo() {
			GridView view = (Band == null ? null : Band.View) as GridView;
			if(view == null) return;
			UseHtmlTextDraw = view.OptionsView.AllowHtmlDrawHeaders;
		}
		protected internal void UpdateCaption() {
			UpdateHtmlDrawInfo();
			if(band == null) {
				Caption = "";
				return;
			}
			if(CustomizationForm) 
				Caption = band.GetCustomizationCaption();
			else
				Caption = band.OptionsBand.ShowCaption ? band.Caption : "";
		}
		public bool CustomizationForm { 
			get { return customizationForm; } 
			set { 
				customizationForm = value; 
				UpdateCaption();
			} 
		}
		protected internal virtual void AddColumn(GridColumnInfoArgs ci) {
			if(columns == null) columns = new GridColumnsInfo();
			Columns.Add(ci);
		}
		protected internal virtual void AddChild(GridBandInfoArgs info) {
			if(children == null) children = new GridBandInfoCollection();
			Children.Add(info);
		}
		public virtual void CreateInnerCollection() {
			InnerElements.Clear();
			if(Band == null) return;
			if(Band.View != null && Band.View.OptionsView.AllowGlyphSkinning)
				InnerElements.Add(new DrawElementInfo(new SkinnedGlyphElementPainter(), new SkinnedGlyphElementInfoArgs(Band.Images, Band.ImageIndex, Band.Image), Band.ImageAlignment));
			else
				InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), new GlyphElementInfoArgs(Band.Images, Band.ImageIndex, Band.Image), Band.ImageAlignment));
		}
		public virtual FixedStyle Fixed { get { return _fixed; } set { _fixed = value; } }
		public virtual bool HasChildren { get { return Children != null && Children.Count > 0; } }
		public virtual bool HasColumns { get { return Columns != null && Columns.Count > 0; } }
		public GridBandInfoCollection Children { get { return children; } }
		public GridColumnsInfo Columns { get { return columns; } }
		public virtual GridBand Band { get { return band; } internal set { band = value; } }
		public virtual GridColumnInfoType Type { get { return type; } set { type = value; } }
		public bool AllowEffects { get { return allowEffects; } set { allowEffects = value; } }
		public bool ValidateCoord { get { return validateCoord; } set { validateCoord = value; } }
	}
	public class Office2003GridGroupRowPainter : GridGroupRowPainter {
		public Office2003GridGroupRowPainter(GridElementsPainter elementsPainter) : base(elementsPainter) { }
		public override void DrawGroupRowIndent(GridRowInfo e, GraphicsCache cache, AppearanceObject appearance, Rectangle bounds) {
			Color backColor = appearance.GetBackColor();
			if(GetIsOffice2003Mode(e)) backColor = Office2003Colors.Default[Office2003Color.GroupRowEx];
			cache.Paint.FillRectangle(cache.Graphics, cache.GetSolidBrush(backColor), bounds);
		}
		public override void DrawGroupRowBackground(ObjectInfoArgs e) {
			if(!GetIsOffice2003Mode(e)) {
				GridGroupRowInfo ee = e as GridGroupRowInfo;
				ee.Appearance.DrawBackground(e.Cache, ee.DataBounds);
				return;
			}
			base.DrawGroupRowBackground(e);
		}
	}
	public class Office2003GridFilterPanelPainter : GridFilterPanelPainter {
		public Office2003GridFilterPanelPainter() : base(new Office2003FilterCloseButtonPainter(), new Office2003CheckObjectPainter()) { }
		protected override ObjectPainter CreatePanelPainter() {
			return new Office2003FooterPanelObjectPainter();
		}
		public override void DrawBackground(ObjectInfoArgs e) {
			PanelPainter.DrawObject(e);
		}
	}
	public class Office2003FilterCloseButtonPainter : EditorButtonPainter {
		public Office2003FilterCloseButtonPainter() : base(EditorButtonHelper.GetPainter(BorderStyles.Office2003)) { }
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			Rectangle r = e.Bounds;
			if(e.State == ObjectState.Normal) {
				e.Paint.DrawRectangle(e.Graphics, SystemPens.Highlight, r);
			}
		}
	}
	public class GridSpecialTopRowIndentPainter : StyleObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			AppearanceObject header = GetStyle(e);
			AppearanceObject app = new AppearanceObject();
			app.GradientMode = LinearGradientMode.Vertical;
			app.BackColor = ControlPaint.Light(header.BackColor);
			app.BackColor2 = ControlPaint.Dark(header.BackColor);
			app.DrawBackground(e.Cache, e.Bounds);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle r = new Rectangle(0, 0, 10, 5);
			return r;
		}
	}
	public class GridSpecialTopRowIndentUltraFlatPainter : GridSpecialTopRowIndentPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			new GridUltraFlatButtonPainter().DrawObject(e);
		}
	}
	public class GridTopNewItemRowIndentOffice2003Painter : GridSpecialTopRowIndentPainter {
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
}
