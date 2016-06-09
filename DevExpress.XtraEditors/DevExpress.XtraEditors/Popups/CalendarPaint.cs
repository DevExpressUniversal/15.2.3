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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.Skins;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Drawing.Animation;
using System.Collections.ObjectModel;
namespace DevExpress.XtraEditors.Calendar {
	public delegate void CustomDrawDayNumberCellEventHandler(object sender, CustomDrawDayNumberCellEventArgs e);
	public class CustomDrawDayNumberCellEventArgs : EventArgs {
		public CustomDrawDayNumberCellEventArgs(GraphicsCache cache, CalendarCellViewInfo viewInfo, SkinElementInfo backgroundInfo) {
			ViewInfo = viewInfo;
			BackgroundElementInfo = backgroundInfo;
			Cache = cache;
			Date = ViewInfo.Date;
			State = ViewInfo.State;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CalendarCellViewInfo ViewInfo { get; private set; }
		public DateEditCalendarViewType View { get { return ViewInfo.Calendar.View; } }
		public GraphicsCache Cache { get; private set; }
		public Graphics Graphics { get { return Cache.Graphics; } }
		public Rectangle Bounds { get { return ViewInfo.Bounds; } }
		public Rectangle ContentBounds { get { return ViewInfo.ContentBounds; } }
		public AppearanceObject Style {
			get { return ViewInfo.Appearance; }
			set { ViewInfo.Appearance = value; } 
		}
		public DateTime Date { get; set; }
		public bool Selected { get { return ViewInfo.Selected; } }
		public bool Highlighted { get { return ViewInfo.IsHighlighted; } }
		public bool IsPressed { get { return ViewInfo.IsPressed; } }
		public bool Disabled { get { return ViewInfo.IsDisabled; } }
		public bool Inactive { get { return !ViewInfo.IsActive; } }
		public bool Today { get { return ViewInfo.Today; } }
		public bool Holiday { get { return ViewInfo.Holiday; } }
		public bool IsSpecial { get { return ViewInfo.IsSpecial; } }
		public bool Handled { get; set; }
		public SkinElementInfo BackgroundElementInfo { get; set; }
		public ObjectState State { get; set; }
	}
	public delegate void DisableCalendarDateEventHandler(object sender, DisableCalendarDateEventArgs e);
	public delegate void SpecialCalendarDateEventHandler(object sender, SpecialCalendarDateEventArgs e);
	public class DisableCalendarDateEventArgs : SpecialCalendarBaseDateEventArgs {
		bool disabled;
		public DisableCalendarDateEventArgs(DateTime date, bool disabled, DateEditCalendarViewType view) : base(date, view) {
			this.disabled = disabled;
		}
		public bool IsDisabled { get { return disabled; } set { disabled = value; } }
	}
	public class SpecialCalendarDateEventArgs : SpecialCalendarBaseDateEventArgs {
		public SpecialCalendarDateEventArgs(DateTime date, bool isSpecial, DateEditCalendarViewType view) : base(date, view) {
			IsSpecial = isSpecial;
		}
		public virtual bool IsSpecial { get; set; }
	}
	public class SpecialCalendarBaseDateEventArgs : EventArgs {
		DateTime date;
		DateEditCalendarViewType view;
		public SpecialCalendarBaseDateEventArgs(DateTime date, DateEditCalendarViewType view) {
			this.date = date;
			this.view = view;
		}
		public DateTime Date { get { return date; } set { date = value; } }
		public DateEditCalendarViewType View { get { return view; } }
	}
	public class SkinCalendarCellPainter : CalendarCellPainter {
		public override SkinElementInfo GetButtonElementInfo(CalendarCellObjectInfoArgs info) {
			SkinElementInfo skinInfo = new SkinElementInfo(CommonSkins.GetSkin(info.ViewInfo.CalendarInfo.LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinHighlightedItem], info.ViewInfo.Bounds);
			if(info.ViewInfo.Selected && info.ViewInfo.Calendar.HighlightSelection) skinInfo.ImageIndex = 1;
			skinInfo.Cache = info.Cache;
			return skinInfo;
		}
		protected override void DrawTodayFrame(CalendarCellObjectInfoArgs info, CustomDrawDayNumberCellEventArgs e) {
			SkinElementInfo sinfo = GetButtonElementInfo(info);
			sinfo.ImageIndex = 1;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, sinfo);
		}
	}
	public class CalendarCellPainter : ObjectPainter {
		protected virtual void RaiseCustomDrawDayNumberCell(CalendarCellObjectInfoArgs info, CustomDrawDayNumberCellEventArgs e) {
			info.Calendar.RaiseCustomDrawDayNumberCell(e);
		}
		public virtual SkinElementInfo GetButtonElementInfo(CalendarCellObjectInfoArgs info) {
			return null;
		}
		protected virtual CustomDrawDayNumberCellEventArgs CreateCellEventArgs(CalendarCellObjectInfoArgs info) {
			return new CustomDrawDayNumberCellEventArgs(info.Cache, info.ViewInfo, GetButtonElementInfo(info));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			CalendarCellObjectInfoArgs info = (CalendarCellObjectInfoArgs)e;
			if(DrawAnimationFrame(info))
				return;
			CustomDrawDayNumberCellEventArgs ee = CreateCellEventArgs(info);
			RaiseCustomDrawDayNumberCell(info, ee);
			if(ee.Handled)
				return;
			if(info.ViewInfo.Today && info.ViewInfo.CalendarInfo.HighlightTodayCell)
				DrawTodayFrame(info, ee);
			DrawCellBackground(info, ee);
			DrawCellForeground(info, ee);
			DrawContextItems(info, ee);
			if(info.ViewInfo.IsDisabled)
				DrawDisabledLine(info, ee);
		}
		protected virtual void DrawContextItems(CalendarCellObjectInfoArgs info, CustomDrawDayNumberCellEventArgs ee) {
			new ContextItemCollectionPainter().Draw(new ContextItemCollectionInfoArgs(info.ViewInfo.ContextButtonsViewInfo, info.Cache, info.ViewInfo.Bounds));
		}
		protected virtual void DrawDisabledLine(CalendarCellObjectInfoArgs info, CustomDrawDayNumberCellEventArgs e) {
			info.Graphics.DrawLine(info.Cache.GetPen(info.ViewInfo.Appearance.ForeColor), 
				new Point(info.ViewInfo.Bounds.X, info.ViewInfo.Bounds.Y + info.ViewInfo.Bounds.Height / 2), 
				new Point(info.ViewInfo.Bounds.Right, info.ViewInfo.Bounds.Y + info.ViewInfo.Bounds.Height / 2));
		}
		protected virtual void DrawCellForeground(CalendarCellObjectInfoArgs info, CustomDrawDayNumberCellEventArgs e) {
			info.ViewInfo.Appearance.DrawString(info.Cache, info.ViewInfo.Text, info.ViewInfo.TextBounds);
			if(!string.IsNullOrEmpty(info.ViewInfo.Description))
				info.ViewInfo.DescriptionAppearance.DrawString(info.Cache, info.ViewInfo.Description, info.ViewInfo.DescriptionBounds);
		}
		protected virtual void DrawCellBackground(CalendarCellObjectInfoArgs info, CustomDrawDayNumberCellEventArgs e) {
			info.ViewInfo.Appearance.DrawBackground(info.Cache, info.ViewInfo.Bounds);
			if(info.ViewInfo.Image != null)
				info.Graphics.DrawImage(info.ViewInfo.Image, info.ViewInfo.ImageBounds);
		}
		protected virtual bool DrawAnimationFrame(CalendarCellObjectInfoArgs info) {
			if(info.CalendarInfo.CalendarInfo.LockAnimation != 0)
				return false;
			return XtraAnimator.Current.DrawFrame(info.Cache, info.Calendar, info.ViewInfo);
		}
		protected virtual void DrawTodayFrame(CalendarCellObjectInfoArgs info, CustomDrawDayNumberCellEventArgs e) {
			Rectangle rect = info.Bounds;
			rect.Width--;
			rect.Height--;
			e.Graphics.DrawRectangle(info.Cache.GetPen(info.ViewInfo.CalendarInfo.TodayFrameColor), rect);
		}
	}
	public class CalendarObjectPainterBase : StyleObjectPainter {
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			CalendarControlObjectInfoArgs info = e as CalendarControlObjectInfoArgs;
			return new Rectangle(Point.Empty, info.ViewInfo.CalcBestSize(e.Cache));
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			CalendarControlObjectInfoArgs info = e as CalendarControlObjectInfoArgs;
			info.ViewInfo.LayoutCalendar(info.ViewInfo.Bounds);
			return info.ViewInfo.Bounds;
		}
		public virtual void DrawBackground(CalendarControlInfoArgs e, Rectangle bounds) {
			e.ViewInfo.PaintAppearance.FillRectangle(e.Cache, bounds);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DrawBackground(((CalendarControlObjectInfoArgs)e).CalendarInfoArgs, e.Bounds);
			DrawHeader((CalendarControlObjectInfoArgs)e);
			DrawContent((CalendarControlObjectInfoArgs)e);
		}
		protected virtual void DrawContent(CalendarControlObjectInfoArgs e) {
			DrawWeekdaysAbbreviation(e);
			DrawWeekdaysAbbreviationSeparator(e);
			DrawDayCells(e);
			DrawCellLines(e);
			DrawCalendarUnderline(e);
			if(e.Calendar.View == DateEditCalendarViewType.MonthInfo) {
				DrawWeekNumbers(e);
				DrawWeekNumbersSeparator(e);
			}
		}
		protected virtual void DrawCellLines(CalendarControlObjectInfoArgs e) {
			if(!e.Calendar.DrawCellLines)
				return;
			for(int i = 0; i <= e.ViewInfo.RowCount; i++) { 
				e.Graphics.DrawLine(e.Cache.GetPen(e.CalendarInfo.PaintStyle.WeekNumbersSeparatorColor, 1), 
					new Point(e.ViewInfo.DayCellsBounds.X, e.ViewInfo.DayCellsBounds.Y + i * e.ViewInfo.ActualCellSize.Height),
						new Point(e.ViewInfo.DayCellsBounds.Right, e.ViewInfo.DayCellsBounds.Y + i * e.ViewInfo.ActualCellSize.Height));
			}
			for(int i = 0; i <= e.ViewInfo.ColumnCount; i++) {
				e.Graphics.DrawLine(e.Cache.GetPen(e.CalendarInfo.PaintStyle.WeekNumbersSeparatorColor, 1),
					new Point(e.ViewInfo.DayCellsBounds.X + i * e.ViewInfo.ActualCellSize.Width, e.ViewInfo.DayCellsBounds.Y),
						new Point(e.ViewInfo.DayCellsBounds.X + i * e.ViewInfo.ActualCellSize.Width, e.ViewInfo.DayCellsBounds.Bottom));
			}
		}
		protected virtual void DrawHeader(CalendarControlObjectInfoArgs e) {
		}
		protected virtual void DrawHeaderButton(CalendarControlObjectInfoArgs e) {
		}
		protected virtual void DrawWeekNumbersSeparator(CalendarControlObjectInfoArgs e) {
			CalendarObjectViewInfoBase viewInfo = (CalendarObjectViewInfoBase)e.ViewInfo;
			if(!viewInfo.ShowWeekNumbers || viewInfo.Calendar.DrawCellLines)
				return;
			e.Graphics.DrawLine(e.Cache.GetPen(viewInfo.WeekNumbersSeparatorColor), viewInfo.WeekNumbersSeparatorStartPoint, viewInfo.WeekNumbersSeparatorEndPoint);
		}
		protected virtual void DrawWeekdaysAbbreviationSeparator(CalendarControlObjectInfoArgs e) {
			CalendarObjectViewInfoBase viewInfo = (CalendarObjectViewInfoBase)e.ViewInfo;
			if(viewInfo.Calendar.DrawCellLines)
				return;
			if(viewInfo.View != DateEditCalendarViewType.MonthInfo)
				return;
			e.Graphics.DrawLine(e.Cache.GetPen(viewInfo.WeekDaysSeparatorColor), viewInfo.WeekDaysSeparatorStartPoint, viewInfo.WeekDaysSeparatorEndPoint);
		}
		protected void DrawWeekNumbers(CalendarControlObjectInfoArgs e) {
			int count = e.ViewInfo.WeekNumberCells.Count;
			for(int i = 0; i < count; i++) {
				CalendarCellViewInfo cell = e.ViewInfo.WeekNumberCells[i];
				DrawWeekNumberCell(e, cell);
			}
		}
		protected virtual void DrawWeekDayAbbreviation(CalendarControlObjectInfoArgs e, CalendarCellViewInfo cell) {
			cell.Appearance.DrawString(e.Cache, cell.Text, cell.Bounds);
		}
		protected virtual void DrawWeekdaysAbbreviation(CalendarControlObjectInfoArgs e) {
			foreach(CalendarCellViewInfo wa in e.ViewInfo.WeekAbbreviationCells) {
				DrawWeekDayAbbreviation(e, wa);
			}
		}
		protected virtual void DrawWeekNumbersLine(CalendarControlObjectInfoArgs e) {
		}
		protected virtual void DrawCalendarUnderline(CalendarControlObjectInfoArgs e) {
		}
		protected virtual void DrawUnderline(CalendarControlObjectInfoArgs info, int y) {
		}
		protected virtual void DrawLine(CalendarControlObjectInfoArgs info, Rectangle r) {
		}
		protected virtual void DrawDayCells(CalendarControlObjectInfoArgs e) {
			int count = e.ViewInfo.DayCells.Count;
			for (int i = 0; i < count; i++) {
				CalendarCellViewInfo cell = e.ViewInfo.DayCells[i];
				DrawDayCell(e, cell);
			}
		}
		protected virtual void DrawDayCell(CalendarControlObjectInfoArgs e, CalendarCellViewInfo cell) {
			e.CalendarInfo.PaintStyle.DayCellPainter.DrawObject(new CalendarCellObjectInfoArgs(e.Cache, e, cell));
		}
		protected virtual void DrawWeekNumberCell(CalendarControlObjectInfoArgs e, CalendarCellViewInfo cell) {
			e.Cache.DrawString(cell.Text, cell.Appearance.Font, e.Cache.GetSolidBrush(cell.Appearance.ForeColor), cell.Bounds, cell.Appearance.GetStringFormat());
		}
	}
	public static class CalendarHeaderHelper {
		public static ObjectPainter GetPainter(UserLookAndFeel lookAndFeel) {
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new CalendarHeaderSkinPainer(lookAndFeel);
			return new CalendarHeaderPainter();
		}
		public static ObjectPainter GetArrowButtonPainter(UserLookAndFeel lookAndFeel) {
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Flat)
				return new EditorButtonPainter(new FlatTabHeaderButtonPainter());
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003)
				return new EditorButtonPainter(new Office2003TabHeaderButtonPainter());
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new SkinCalendarNavigationButtonPainter(lookAndFeel);
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Style3D)
				return EditorButtonHelper.GetPainter(BorderStyles.Style3D);
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.UltraFlat)
				return EditorButtonHelper.GetPainter(BorderStyles.UltraFlat);
			return EditorButtonHelper.GetWindowsXPPainter();
		}
	}
	public class SkinCalendarNavigationButtonPainter : ButtonObjectPainter {
		public SkinCalendarNavigationButtonPainter(ISkinProvider provider) {
			Provider = provider;
		}
		protected ISkinProvider Provider { get; private set; }
		public override void DrawObject(ObjectInfoArgs e) {
			CalendarNavigationButtonInfoArgs info = e as CalendarNavigationButtonInfoArgs;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, GetBackgroundInfo(info));
			info.ViewInfo.PaintAppearance.DrawString(info.Cache, info.ViewInfo.Text, info.ViewInfo.ContentBounds);
		}
		protected virtual SkinElementInfo GetBackgroundInfo(CalendarNavigationButtonInfoArgs info) {
			SkinElement elem = info.ViewInfo.BackgroundElement != null? info.ViewInfo.BackgroundElement: EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinEditorButton];
			SkinElementInfo res = new SkinElementInfo(elem, info.Bounds);
			res.ImageIndex = -1;
			res.State = info.State;
			return res;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			CalendarNavigationButtonInfoArgs info = e as CalendarNavigationButtonInfoArgs;
			return ObjectPainter.CalcBoundsByClientRectangle(info.Graphics, SkinElementPainter.Default, GetBackgroundInfo(info), client);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			CalendarNavigationButtonInfoArgs info = e as CalendarNavigationButtonInfoArgs;
			return CalcBoundsByClientRectangle(e, info.ViewInfo.ContentBounds);
		}
	}
	public class CalendarHeaderPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			StyleObjectInfoArgs se = e as StyleObjectInfoArgs;
			if(se == null) return;
			se.Appearance.FillRectangle(se.Cache, se.Bounds);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle res = client;
			res.Inflate(0, 4);
			return res;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			StyleObjectInfoArgs se = e as StyleObjectInfoArgs;
			if(se == null)
				return base.CalcObjectMinBounds(e);
			return new Rectangle(Point.Empty, se.Appearance.CalcTextSize(e.Graphics, "Wg", 0).ToSize());
		}
	}
	public class CalendarHeaderSkinPainer : CalendarHeaderPainter {
		ISkinProvider provider;
		public CalendarHeaderSkinPainer(ISkinProvider provider) {
			this.provider = provider;
		}
		public ISkinProvider Provider { get { return provider; } }
		public override void DrawObject(ObjectInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetHeaderInfo(e, e.Bounds));
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return ObjectPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, GetHeaderInfo(e, e.Bounds));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle rect = ObjectPainter.CalcBoundsByClientRectangle(e.Graphics, SkinElementPainter.Default, GetHeaderInfo(e, client), client);
			rect.Inflate(0, 4);
			return rect;
		}
		protected SkinElementInfo GetHeaderInfo(ObjectInfoArgs e, Rectangle bounds) {
			StyleObjectInfoArgs ee = e as StyleObjectInfoArgs;
			SkinElementInfo res =  new SkinElementInfo(CommonSkins.GetSkin(Provider)[CommonSkins.SkinGroupPanelNoBorder], bounds);
			res.BackAppearance = ee.BackAppearance;
			return res;
		}
	}
	public class CalendarHitInfo {
		public CalendarHitInfo(Point hitPoint) : this(hitPoint, ObjectState.Normal) { }
		public CalendarHitInfo(Point hitPoint, ObjectState hitInfoType) {
			Clear();
			HitPoint = hitPoint;
			HitInfoType = hitInfoType;
		}
		public void Clear() {
			HitTest = CalendarHitInfoType.Unknown;
			HitDate = CalendarObjectViewInfoBase.InvalidDate;
			HitPoint = Point.Empty;
		}
		public bool IsEquals(CalendarHitInfo hitInfo) {
			return HitTest == hitInfo.HitTest && HitObject == hitInfo.HitObject;
		}
		public bool ContainsSet(Rectangle bounds, CalendarHitInfoType hitTest) {
			if(bounds.Contains(HitPoint)) {
				HitTest = hitTest;
				Bounds = bounds;
				return true;
			}
			return false;
		}
		public ObjectState HitInfoType { get; set; }
		public CalendarObjectViewInfoBase CalendarInfo { get { return Cell == null? null: Cell.ViewInfo; } }
		public CalendarViewInfoBase ViewInfo { get { return CalendarInfo == null ? null : CalendarInfo.CalendarInfo; } }
		[Obsolete("Use the HitTest property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CalendarHitInfoType InfoType { get { return HitTest; } set { HitTest = value; } }
		public CalendarHitInfoType HitTest { get; set; }
		public DateTime HitDate { get; set; }
		[Obsolete("Use the HitPoint property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Point Pt { get { return HitPoint; } set { HitPoint = value; } }
		public Point HitPoint { get; set; }
		public object HitObject { get; set; }
		public CalendarHeaderViewInfoBase Header { get; internal set; }
		public CalendarFooterViewInfoBase Footer { get; internal set; }
		public CalendarAreaViewInfoBase RightArea { get; internal set; }
		public bool IsInCell { get { return Cell != null; } }
		public bool IsInHeader { get { return Header != null; } }
		public bool IsInFooter { get { return Footer != null; } }
		public bool IsInRightArea { get { return RightArea != null; } }
		public CalendarCellViewInfo Cell { get { return HitObject as CalendarCellViewInfo; } }
		public Rectangle Bounds { get; set; }
		public void SetCell(CalendarCellViewInfo cell) {
			Bounds = cell.Bounds;
			HitDate = cell.Date;
			HitObject = cell;
			HitTest = CalendarHitInfoType.Cell;
		}
	}
	public class CalendarCellObjectInfoArgs : ObjectInfoArgs {
		public CalendarCellObjectInfoArgs(GraphicsCache cache, CalendarControlObjectInfoArgs calendarInfo, CalendarCellViewInfo viewInfo) : base(cache) {
			ViewInfo = viewInfo;
			CalendarInfo = calendarInfo;
		}
		public CalendarControlObjectInfoArgs CalendarInfo { get; private set; } 
		public CalendarCellViewInfo ViewInfo { get; private set; }
		public CalendarControlBase Calendar { get { return CalendarInfo.Calendar; } }
		public CalendarPaintStyle PaintStyle { get { return CalendarInfo.ViewInfo.CalendarInfo.PaintStyle; } }
		public override Rectangle Bounds {
			get { return ViewInfo.Bounds; }
			set { }
		}
		public override ObjectState State {
			get { return ViewInfo.State; }
			set { ViewInfo.State = value; }
		}
	}
	public class CalendarCellViewInfo : ISupportContextItems {
		public CalendarCellViewInfo(DateTime date, CalendarObjectViewInfoBase viewInfo) {
			Date = date;
			ViewInfo = viewInfo;
			Appearance = CreateAppearance();
		}
		public CalendarObjectViewInfoBase ViewInfo { get; private set; }
		public CalendarViewInfoBase CalendarInfo { get { return ViewInfo.CalendarInfo; } }
		public CalendarControlBase Calendar { get { return ViewInfo.CalendarInfo.Calendar; } }
		public DateTime Date { get; set; }
		public bool Selected { get; set; }
		public bool IsHighlighted { get; set; }
		public bool IsPressed { get; set; }
		public bool IsActive { get; set; }
		public bool IsDisabled { get; set; }
		public bool IsSpecial { get; set; }
		public bool Holiday { get; set; }
		public bool Today { get; set; }
		public string Text { get; set; }
		public Rectangle ContentBounds { get; set; }
		public Rectangle TextBounds { get; set; }
		public Rectangle DescriptionBounds { get; set; }
		public Rectangle Bounds { get; set; }
		public AppearanceObject Appearance { get; set; }
		public Image Image { get; set; }
		public SkinPaddingEdges ImageMargins { get; set; }
		public ImageScaleMode ImageSizeMode { get; set; }
		public ImageAlignmentMode ImageAlignment { get; set; }
		public Rectangle ImageBounds { get; set; }
		public string Description { get; set; }
		public AppearanceObject DescriptionAppearance { get; private set; }
		protected virtual AppearanceObject CreateAppearance() { return new AppearanceObject(); }
		public ObjectState State { get; set; }
		protected virtual AppearanceObject CreateNormalCellAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.DayCellAppearance.Clone();
		}
		protected CalendarPaintStyle PaintStyle { get { return CalendarInfo.PaintStyle; } }
		protected virtual AppearanceObject CreateNormalCellSpecialAppearance() {
			return (AppearanceObject)PaintStyle.DayCellSpecialAppearance.Clone();
		}
		protected virtual AppearanceObject CreateInactiveCellAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.InactiveDayCellAppearance.Clone();
		}
		protected virtual AppearanceObject CreateTodayCellAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.TodayCellAppearance.Clone();
		}
		protected virtual AppearanceObject CreateSelectedCellAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.SelectedDayCellAppearance.Clone();
		}
		protected virtual AppearanceObject CreateSelectedCellSpecialAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.SelectedDayCellSpecialAppearance.Clone();
		}
		protected virtual AppearanceObject CreateHolidayCellAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.HolidayCellAppearance.Clone();
		}
		protected virtual AppearanceObject CreateDisabledCellAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.DisabledDayCellAppearance.Clone();
		}
		protected virtual AppearanceObject CreateHighlightCellAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.HighlightDayCellAppearance.Clone();
		}
		protected virtual AppearanceObject CreatePressedCellAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.PressedDayCellAppearance.Clone();
		}
		protected virtual AppearanceObject CreateHighlightCellSpecialAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.HighlightDayCellSpecialAppearance.Clone();
		}
		protected virtual AppearanceObject CreatePressedCellSpecialAppearance() {
			return (AppearanceObject)CalendarInfo.PaintStyle.PressedDayCellSpecialAppearance.Clone();
		}
		protected internal virtual void UpdateAppearance() {
			if(IsDisabled) {
				Appearance = CreateDisabledCellAppearance();
			}
			else if(IsHighlighted) {
				Appearance = IsSpecial ? CreateHighlightCellSpecialAppearance() : CreateHighlightCellAppearance();
			}
			else if(IsPressed) {
				Appearance = IsSpecial ? CreatePressedCellSpecialAppearance() : CreatePressedCellAppearance();
			}
			else if(Selected && Calendar.HighlightSelection) {
				Appearance = IsSpecial? CreateSelectedCellSpecialAppearance(): CreateSelectedCellAppearance();
			}
			else if(!IsActive) {
				Appearance = CreateInactiveCellAppearance();
			}
			else if(Today) {
				Appearance = CreateTodayCellAppearance();
			}
			else if(IsSpecial) {
				Appearance = CreateNormalCellSpecialAppearance();
			}
			else if(CalendarInfo.HighlightHolidays && Holiday) {
				Appearance = CreateHolidayCellAppearance();
			}
			else 
				Appearance = IsSpecial? CreateNormalCellSpecialAppearance(): CreateNormalCellAppearance();
			UpdateStyleFromProvider();
		}
		protected virtual CalendarCellStyle GetCellStyle() {
			CalendarCellStyle info = new CalendarCellStyle() {
				Bounds = Bounds,
				Active = IsActive,
				Appearance = Appearance,
				Date = Date,
				Holiday = Holiday,
				IsSpecial = IsSpecial,
				State = State,
				View = ViewInfo.View,
				PaintStyle = PaintStyle,
				Image = Image,
				ImageSizeMode = ImageSizeMode,
				ImageAlignment = ImageAlignment,
				ImageMargins = CalendarInfo.GetCellPadding()
			};
			return info;
		}
		protected virtual void UpdateStyleFromProvider() {
			if(Calendar.CellStyleProvider == null)
				return;
			CalendarCellStyle info = GetCellStyle();
			Calendar.CellStyleProvider.UpdateAppearance(info);
			Appearance = info.Appearance;
			Image = info.Image;
			ImageSizeMode = info.ImageSizeMode;
			ImageAlignment = info.ImageAlignment;
			ImageMargins = info.ImageMargins;
			ImageBounds = CalcImageBounds();
			Description = info.Description;
			if(!string.IsNullOrEmpty(Description)) {
				DescriptionAppearance = info.DescriptionAppearance;
				if(DescriptionAppearance == null)
					DescriptionAppearance = (AppearanceObject)Appearance.Clone();
			}
		}
		protected virtual Rectangle CalcImageBounds() {
			if(Image == null) { 
				return Rectangle.Empty;
			}
			Rectangle rect = Bounds;
			rect.X += ImageMargins.Left;
			rect.Width -= ImageMargins.Width;
			rect.Y += ImageMargins.Top;
			rect.Height -= ImageMargins.Height;
			return ImageLayoutHelper.GetImageBounds(rect, Image.Size, ImageSizeMode, ImageAlignment);
		}
		protected virtual void UpdateState() {
			IsActive = ViewInfo.IsDateActive(this);
			if(Calendar.DisabledDateProvider != null) {
				IsDisabled = Calendar.DisabledDateProvider.IsDisabledDate(Date, ViewInfo.View);
			}
			else {
				DisableCalendarDateEventArgs e = new DisableCalendarDateEventArgs(Date, false, ViewInfo.View);
				Calendar.RaiseDisableCalendarDate(e);
				IsDisabled = e.IsDisabled;
			}
			if(Calendar.SpecialDateProvider != null) {
				IsSpecial = Calendar.SpecialDateProvider.IsSpecialDate(Date, ViewInfo.View);
			}
			else {
				SpecialCalendarDateEventArgs se = new SpecialCalendarDateEventArgs(Date, false, ViewInfo.View);
				Calendar.RaiseSpecialCalendarDate(se);
				IsSpecial = se.IsSpecial;
			}
			Selected = ViewInfo.IsDateSelected(this);
			IsHighlighted = ViewInfo.IsHighlightedDate(this);
			IsPressed = ViewInfo.IsPressedDate(this);
			Today = ViewInfo.IsToday(this);
			Holiday = ViewInfo.IsHolidayDate(this);
			State = CalcCellState();
		}
		private ObjectState CalcCellState() {
			if(IsDisabled)
				return ObjectState.Disabled;
			if(IsPressed)
				return ObjectState.Pressed;
			if(IsHighlighted)
				return ObjectState.Hot;
			if(Selected)
				return ObjectState.Selected;
			return ObjectState.Normal;
		}
		public virtual void UpdateVisualState() {
			UpdateState();
			UpdateAppearance();
		}
		protected internal virtual bool UpdateVisualInfo(CalendarHitInfo info) {
			if(ViewInfo.CalendarInfo.SuppressStateChangeAnimation)
				return true;
			CalendarControlInfoArgs infoArgs = new CalendarControlInfoArgs(ViewInfo.CalendarInfo, CalendarInfo.GInfo.Cache, Bounds);
			CalendarCellObjectInfoArgs cellInfo = new CalendarCellObjectInfoArgs(CalendarInfo.GInfo.Cache, new CalendarControlObjectInfoArgs(infoArgs, ViewInfo, CalendarInfo.GInfo.Cache), this);
			CalendarAnimationFramePainter painter = new CalendarAnimationFramePainter(infoArgs, CalendarInfo.PaintStyle.DayCellPainter, cellInfo);
			Bitmap from = XtraAnimator.Current.CreateBitmap(painter, cellInfo, true);
			UpdateVisualState();
			Bitmap to = XtraAnimator.Current.CreateBitmap(painter, cellInfo, true);
			XtraAnimator.Current.AddAnimation(new BitmapAnimationInfo(ViewInfo.Calendar, this, from, to, Bounds, CalendarViewInfoBase.StateChangeAnimationLength));
			return true;
		}
		ContextItemCollectionViewInfo contextButtonsViewInfo;
		protected internal ContextItemCollectionViewInfo ContextButtonsViewInfo {
			get { 
				if(contextButtonsViewInfo == null)
					contextButtonsViewInfo = new ContextItemCollectionViewInfo(((ISupportContextItems)this).ContextItems, ((ISupportContextItems)this).Options, this);
				return contextButtonsViewInfo;
			}
		}
		ContextItemCollectionHandler contextButtonsHandler;
		protected internal ContextItemCollectionHandler ContextButtonsHandler {
			get {
				if(contextButtonsHandler == null)
					contextButtonsHandler = new ContextItemCollectionHandler(ContextButtonsViewInfo);
				return contextButtonsHandler;
			}
		}
		Rectangle ISupportContextItems.ActivationBounds {
			get { return Bounds; }
		}
		bool ISupportContextItems.CloneItems {
			get { return true; }
		}
		ContextItemCollection ISupportContextItems.ContextItems {
			get { return Calendar.ContextButtons; }
		}
		Control ISupportContextItems.Control {
			get { return Calendar; }
		}
		bool ISupportContextItems.DesignMode {
			get { return Calendar.IsDesignMode; }
		}
		Rectangle ISupportContextItems.DisplayBounds {
			get { return Bounds; }
		}
		Rectangle ISupportContextItems.DrawBounds {
			get { return Bounds; }
		}
		ItemHorizontalAlignment ISupportContextItems.GetCaptionHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemVerticalAlignment ISupportContextItems.GetCaptionVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		ItemHorizontalAlignment ISupportContextItems.GetGlyphHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemLocation ISupportContextItems.GetGlyphLocation(ContextButton btn) {
			return ItemLocation.Left;
		}
		int ISupportContextItems.GetGlyphToCaptionIndent(ContextButton btn) {
			return 3;
		}
		ItemVerticalAlignment ISupportContextItems.GetGlyphVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		UserLookAndFeel ISupportContextItems.LookAndFeel {
			get { return Calendar.LookAndFeel.ActiveLookAndFeel; }
		}
		ContextItemCollectionOptions ISupportContextItems.Options {
			get { return Calendar.ContextButtonOptions; }
		}
		void ISupportContextItems.RaiseContextItemClick(ContextItemClickEventArgs e) {
			e.DataItem = this;
			Calendar.RaiseContextButtonClick(e);
		}
		void ISupportContextItems.RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
		}
		void ISupportContextItems.RaiseCustomizeContextItem(ContextItem item) {
			Calendar.RaiseContextButtonCustomize(new CalendarContextButtonCustomizeEventArgs() { Cell = GetCellStyle(), Item = item });
		}
		void ISupportContextItems.Redraw(Rectangle rect) {
			Calendar.Invalidate(rect);
		}
		void ISupportContextItems.Redraw() {
			Calendar.Invalidate();
		}
		bool ISupportContextItems.ShowOutsideDisplayBounds {
			get { return false; }
		}
		void ISupportContextItems.Update() {
			Calendar.Update();
		}
		protected Items2Panel ItemsPanel {
			get;
			set;
		}
		protected internal virtual void CalcTextBounds() {
			if(string.IsNullOrEmpty(Description)) {
				TextBounds = ContentBounds;
				return;
			}
			Size descSize = DescriptionAppearance.CalcTextSize(ViewInfo.CalendarInfo.GInfo.Graphics, Description, ContentBounds.Width).ToSize();
			UpdateItemsPanel();
			Size textSize = Appearance.CalcTextSize(CalendarInfo.GInfo.Graphics, Text, ContentBounds.Width).ToSize();
			Size contentSize = ItemsPanel.CalcBestSize(textSize, descSize);
			Rectangle textRect = Rectangle.Empty, descRect = Rectangle.Empty;
			Rectangle content = ArrangeContent(contentSize);
			ItemsPanel.ArrangeItems(content, textSize, descSize, ref textRect, ref descRect);
			TextBounds = textRect;
			DescriptionBounds = descRect;
		}
		protected virtual Rectangle ArrangeContent(Size contentSize) {
			int x = 0, y = 0;
			if(Appearance.TextOptions.HAlignment == HorzAlignment.Near)
				x = ContentBounds.X;
			if(Appearance.TextOptions.HAlignment == HorzAlignment.Far)
				x = ContentBounds.Right - contentSize.Width;
			if(Appearance.TextOptions.HAlignment == HorzAlignment.Center)
				x = ContentBounds.X + (ContentBounds.Width - contentSize.Width) / 2;
			if(x < ContentBounds.X) x = ContentBounds.X;
			if(Appearance.TextOptions.VAlignment == VertAlignment.Top)
				y = ContentBounds.Y;
			if(Appearance.TextOptions.VAlignment == VertAlignment.Center)
				y = ContentBounds.Y + (ContentBounds.Height - contentSize.Height) / 2;
			if(Appearance.TextOptions.VAlignment == VertAlignment.Bottom)
				y = ContentBounds.Bottom - contentSize.Height;
			if(y < ContentBounds.Y) y = ContentBounds.Y;
			return new Rectangle(x, y, Math.Min(ContentBounds.Width, contentSize.Width), Math.Min(ContentBounds.Height, contentSize.Height));
		}
		protected virtual void UpdateItemsPanel() {
			ItemsPanel = new Items2Panel();
			ItemsPanel.Content1HorizontalAlignment = ItemsPanel.GetHorizontalAlignment(Appearance.TextOptions.HAlignment);
			ItemsPanel.Content1VerticalAlignment = ItemsPanel.GetVerticalAlignment(Appearance.TextOptions.VAlignment);
			ItemsPanel.Content2HorizontalAlignment = ItemsPanel.GetHorizontalAlignment(DescriptionAppearance.TextOptions.HAlignment);
			ItemsPanel.Content2VerticalAlignment = ItemsPanel.GetVerticalAlignment(DescriptionAppearance.TextOptions.VAlignment);
			ItemsPanel.Content1Location = ItemLocation.Top;
		}
	}
	public class DayNumberCellInfoCollection : Collection<CalendarCellViewInfo> { }
	public enum CalendarHitInfoType { Unknown, MonthNumber, DecMonth, IncMonth, DecYear, IncYear, EditMonth, WeekNumber, LeftArrow, RightArrow, LeftArrow2, RightArrow2, Cell, Caption, CurrentDate, Clear, Ok, Cancel, Today, LeftScrollArea, RightScrollArea }
	public class CalendarControlObjectInfoArgs : ObjectInfoArgs {
		public CalendarControlObjectInfoArgs(CalendarControlInfoArgs calendarInfo, CalendarObjectViewInfoBase viewInfo) : base() {
			ViewInfo = viewInfo;
			CalendarInfoArgs = calendarInfo;
		}
		public CalendarControlObjectInfoArgs(CalendarControlInfoArgs calendarInfo, CalendarObjectViewInfoBase viewInfo, GraphicsCache cache) : base(cache) {
			ViewInfo = viewInfo;
			CalendarInfoArgs = calendarInfo;
		}
		public CalendarControlInfoArgs CalendarInfoArgs { get; set; }
		public CalendarObjectViewInfoBase ViewInfo { get; private set; }
		public CalendarViewInfoBase CalendarInfo { get { return ViewInfo.CalendarInfo; } }
		public CalendarControlBase Calendar { get { return CalendarInfo.Calendar; } }
	}
	public abstract class CalendarObjectViewInfoBase {
		public int PenaltyIndex;
		public bool FillBackground;
		DateTime firstVisibleDate;
		DateTime currentMonth;
		DayNumberCellInfoCollection dayCells, weekCells, weekAbbrCells;
		public CalendarObjectViewInfoBase(CalendarControlBase calendar) {
			Calendar = calendar;
			this.dayCells = new DayNumberCellInfoCollection();
			this.weekCells = new DayNumberCellInfoCollection();
			this.weekAbbrCells = new DayNumberCellInfoCollection();
			PenaltyIndex = 0;
			FillBackground = true;
			this.firstVisibleDate = Calendar.GetTodayDate();
			this.currentMonth = firstVisibleDate;
		}
		protected internal virtual void UpdateContent() { }
		CalendarObjectHeaderViewInfoBase header;
		public CalendarObjectHeaderViewInfoBase Header {
			get {
				if(header == null)
					header = CreateHeader();
				return header;
			}
		}
		public virtual bool ShowHeader { get; protected internal set; }
		protected virtual CalendarObjectHeaderViewInfoBase CreateHeader() {
			return new CalendarObjectHeaderViewInfo(this);
		}
		protected virtual void CheckRtlLayout(CalendarObjectViewInfoBase info) {
			if(info.IsRightToLeft) {
				foreach(CalendarCellViewInfo cell in info.DayCells) {
					InvertCellBounds(info, cell);
				}
				foreach(CalendarCellViewInfo cell in info.WeekNumberCells) {
					InvertCellBounds(info, cell);
				}
				foreach(CalendarCellViewInfo cell in info.WeekAbbreviationCells) {
					InvertCellBounds(info, cell);
				}
			}
		}
		protected void InvertCellBounds(CalendarObjectViewInfoBase info, CalendarCellViewInfo cell) {
			cell.Bounds = InvertBounds(cell.Bounds, info.Bounds);
			cell.ContentBounds = InvertBounds(cell.ContentBounds, info.Bounds);
		}
		protected Rectangle InvertBounds(Rectangle rect, Rectangle content) {
			rect.X = content.Left + content.Right - rect.Right;
			return rect;
		}
		public virtual CalendarViewInfoBase CalendarInfo { get; internal set; }
		public DayNumberCellInfoCollection DayCells { get { return dayCells; } }
		public DayNumberCellInfoCollection WeekNumberCells { get { return weekCells; } }
		public DayNumberCellInfoCollection WeekAbbreviationCells { get { return weekAbbrCells; }}
		public DateTime CurrentDate { get { return currentMonth; } set { currentMonth = value; } }
		public abstract UserLookAndFeel LookAndFeel { get; }
		public abstract bool ShowWeekNumbers { get; } 
		public abstract DateTimeFormatInfo DateFormat { get; }
		public abstract DayOfWeek FirstDayOfWeek { get; }
		public abstract DateTime DateTime { get; }
		public abstract DateTime MinValue { get; }
		public abstract DateTime MaxValue { get; }
		public virtual void Reset() {
			HeaderBounds = ContentBounds = Bounds = Rectangle.Empty;
		}
		public CalendarControlBase Calendar { get; private set; }
		protected Size ScaleMetrics(Size size) {
			if(LookAndFeel.ActiveLookAndFeel.Style == LookAndFeelStyle.Skin && LookAndFeel.GetTouchUI()) {
				SkinElement elem = CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinHighlightedItem];
				return ObjectPainter.CalcBoundsByClientRectangle(CalendarInfo.GInfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(elem, Rectangle.Empty), new Rectangle(Point.Empty, size)).Size;
			}
			return size;
		}
		protected virtual void UpdateDayCellsState() {
			for(int i = 0; i < DayCells.Count; i++) {
				CalendarCellViewInfo cell = DayCells[i];
				cell.UpdateVisualState();
			}
		}
		protected virtual void UpdateWeekDayAbbreviationCellsState() {
			for(int i = 0; i < WeekAbbreviationCells.Count; i++) {
				UpdateWeekDayAppearance(WeekAbbreviationCells[i]);
			}	
		}
		protected virtual void UpdateWeekDayAppearance(CalendarCellViewInfo cell) {
			cell.Appearance = (AppearanceObject)CalendarInfo.PaintStyle.WeekDayAppearance.Clone();
		}
		protected virtual void UpdateWeekNumberCellsState() {
			foreach(CalendarCellViewInfo info in WeekNumberCells) {
				UpdateWeekNumberAppearance(info);
			}
		}
		protected virtual void UpdateWeekNumberAppearance(CalendarCellViewInfo info) {
			info.Appearance = (AppearanceObject)CalendarInfo.PaintStyle.WeekNumberAppearance.Clone();
		}
		public void UpdateExistingCellsState() {
			UpdateDayCellsState();
			UpdateWeekDayAbbreviationCellsState();
			UpdateWeekNumberCellsState();
		}
		protected internal virtual bool IsToday(CalendarCellViewInfo cell) {
			if(View != DateEditCalendarViewType.MonthInfo)
				return false;
			return DrawingDatesAreEqual(cell.Date, Calendar.GetTodayDate());
		}
		protected internal virtual bool IsHighlightedDate(CalendarCellViewInfo cell) {
			if(cell.IsDisabled)
				return false;
			else
				return CalendarInfo.HoverInfo.IsInCell && CalendarInfo.HoverInfo.Cell == cell;
		}
		protected virtual CalendarCellViewInfo CreateDayCell(DateTime date) {
			return new CalendarCellViewInfo(date, this);
		}
		public Size ActualCellSize { get; protected set; }
		protected virtual Size CalcActualCellSize() {
			return new Size(DayCellsBounds.Width / GetCellColumnCount(), DayCellsBounds.Height / GetCellRowCount());				
		}
		protected virtual Rectangle CalcCellBounds(int x, int y) {
			return new Rectangle(new Point(x, y), ActualCellSize);
		}
		protected internal bool IsRightToLeft {
			get { return WindowsFormsSettings.GetIsRightToLeft(Calendar); }
		}
		protected virtual Rectangle CalcWeekCellBounds(string text, int x, int y) {
			return new Rectangle(x, y, CalendarInfo.WeekNumberSize.Width, ActualCellSize.Height);
		}
		protected internal virtual bool IsDateSelected(CalendarCellViewInfo cell) {
			return DrawingDatesAreEqual(cell.Date, DateTime);
		}
		protected internal virtual bool IsDateActive(CalendarCellViewInfo cell) {
			if(View == DateEditCalendarViewType.MonthInfo)
				return (cell.Date.Month == CurrentDate.Month);
			if(View == DateEditCalendarViewType.YearsInfo) {
				int startYear = (CurrentDate.Year / 10) * 10;
				int endYear = startYear + 9;
				return cell.Date.Year >= startYear && cell.Date.Year <= endYear;
			}
			if(View == DateEditCalendarViewType.YearsGroupInfo) {
				int startYear = (CurrentDate.Year / 100) * 100;
				int endYear = startYear + 99;
				return cell.Date.Year >= startYear && cell.Date.Year <= endYear;
			}
			return true;
		}
		protected internal virtual bool IsHolidayDate(CalendarCellViewInfo cell) {
			if(View != DateEditCalendarViewType.MonthInfo)
				return false;
			DayOfWeek dayOfWeek = cell.Date.DayOfWeek;
			return dayOfWeek == DayOfWeek.Sunday || dayOfWeek == DayOfWeek.Saturday;
		}
		protected virtual bool CanAddDate(DateTime date) {
			DateTime endDate = GetCellEndDate(date);
			DateTime startDate = GetCellStartDate(date);
			if(endDate < MinValue) return false;
			if(startDate > MaxValue) return false;
			return true;
		}
		protected virtual System.DateTime GetCellStartDate(System.DateTime date) {
			return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
		}
		protected virtual System.DateTime GetCellEndDate(System.DateTime date) {
			DateTime start = GetCellStartDate(date);
			if(View == DateEditCalendarViewType.QuarterInfo) { 
				start = start.AddMonths(2);
				start = start.AddDays(DateTime.DaysInMonth(start.Year, start.Month) - 1);
			}
			else if(View == DateEditCalendarViewType.YearInfo) {
				start = start.AddDays(DateTime.DaysInMonth(start.Year, start.Month) - 1);
			}
			else if(View == DateEditCalendarViewType.YearsInfo) {
				start = start.AddMonths(11);
				start = start.AddDays(DateTime.DaysInMonth(start.Year, start.Month) - 1);
			}
			else if(View == DateEditCalendarViewType.YearsGroupInfo) {
				start = start.AddYears(9);
				start = start.AddMonths(11);
				start = start.AddDays(DateTime.DaysInMonth(start.Year, start.Month) - 1);
			}
			return new DateTime(start.Year, start.Month, start.Day, 23, 59, 59);
		}
		protected static bool DrawingDatesAreEqual(DateTime dt1, DateTime dt2) {
			return (dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day);
		}
		protected virtual Rectangle CalcDayCellContentBounds(Rectangle r) {
			SkinPaddingEdges p = CalendarInfo.GetCellPadding();
			r.X += p.Left;
			r.Y += p.Top;
			r.Width -= p.Width;
			r.Height -= p.Height;
			return r;
		}
		protected virtual void CalcFirstVisibleDate() {
			SetFirstVisibleDate(GetFirstVisibleDate(CurrentDate));
		}
		public virtual void SetFirstVisibleDate(DateTime firstVisibleDate) {
			PenaltyIndex = 0;
			FirstVisibleDate = new DateTime(firstVisibleDate.Ticks, firstVisibleDate.Kind);
			if(View != DateEditCalendarViewType.MonthInfo)
				return;
			if(FirstVisibleDate == DateTime.MinValue)
				PenaltyIndex = GetFirstDayOffset(FirstVisibleDate) - (FirstVisibleDate - FirstVisibleDate).Days;
		}
		public virtual DateTime GetFirstVisibleDate(DateTime editDate) {
			DateTime firstMonthDate = GetFirstMonthDate(editDate);
			TimeSpan delta = TimeSpan.FromDays(-GetFirstDayOffset(firstMonthDate));
			if (firstMonthDate.Ticks + delta.Ticks < 0)
				return DateTime.MinValue;
			else {
				try {
					return firstMonthDate + delta;
				}
				catch(ArgumentOutOfRangeException) { 
					return MinValue; 
				}
			}
		}
		static internal DateTime GetFirstMonthDate(DateTime date) { return new DateTime(date.Year, date.Month, 1, date.Hour, date.Minute, date.Second, date.Millisecond, date.Kind); }
		protected int GetFirstDayOffset(DateTime firstMonthDate) {
			return (FirstDayOfWeek == firstMonthDate.DayOfWeek ? 7 : (7 + firstMonthDate.DayOfWeek - FirstDayOfWeek) % 7);
		}
		protected virtual int? GetWeekNumber(DateTime date) {
			int weekNumber = Calendar.DateFormat.Calendar.GetWeekOfYear(date, GetWeekNumberRule(), FirstDayOfWeek);
			CustomWeekNumberEventArgs e = new CustomWeekNumberEventArgs() { Date = date, WeekNumber = weekNumber };
			Calendar.RaiseCustomWeekNumber(e);
			return e.WeekNumber;
		}
		protected virtual CalendarWeekRule GetWeekNumberRule() {
			return Calendar.DateFormat.CalendarWeekRule;
		}
		public virtual CalendarHitInfo GetHitInfo(Point location) {
			CalendarHitInfo result = new CalendarHitInfo(location, ObjectState.Normal);
			if(DayCellsBounds.Contains(location)) {
				MonthNumberHitTest(result, location);
				if(result.HitTest != CalendarHitInfoType.Unknown)
					return result;
			}
			WeekNumberHitTest(result, location);
			return result;
		}
		public virtual CalendarHitInfo GetHitInfo(MouseEventArgs e) {
			return GetHitInfo(e.Location);
		}
		void DayNumberCollectionHitTest(CalendarHitInfo hit, Point pt, DayNumberCellInfoCollection coll, CalendarHitInfoType type) {
			int count = coll.Count;
			for(int i = 0; i < count; i++) {
				if (coll[i].Bounds.Contains(pt)) {
					hit.HitTest = type;
					hit.HitDate = coll[i].Date;
					hit.HitObject = coll[i];
					return;
				}
			}
		}
		protected virtual void MonthNumberHitTest(CalendarHitInfo hit, Point pt) {
			DayNumberCollectionHitTest(hit, pt, DayCells, CalendarHitInfoType.MonthNumber);
		}
		protected virtual void WeekNumberHitTest(CalendarHitInfo hit, Point pt) {
			DayNumberCollectionHitTest(hit, pt, WeekNumberCells, CalendarHitInfoType.WeekNumber);
		}
		public virtual int GetDateVisibleIndex(DateTime dt) {
			try { return (dt - FirstVisibleDate).Days + PenaltyIndex; }
			catch {}
			return -1;
		}
		public Rectangle HeaderBounds { get; set; }
		public Rectangle ContentBounds { get; set; }
		public DateTime FirstVisibleDate { get { return firstVisibleDate; } set { firstVisibleDate = value; } }
		public static DateTime InvalidDate { get { return DateTime.MinValue; } }
		internal string CalculateInsufficientPrefixForLength(string[] names, int length) {
			IDictionary q = new Hashtable();
			for(int i = 0; i < names.Length; ++i) {
				string currentNm = names[i];
				if(currentNm.Length > length) {
					string supposedPrefix = currentNm.Substring(0, length);
					if(q.Contains(supposedPrefix)) {
						int cnt = (int)q[supposedPrefix];
						q[supposedPrefix] = cnt + 1;
					}
					else {
						q.Add(supposedPrefix, 1);
					}
				}
			}
			foreach(DictionaryEntry de in q) {
				if((int)de.Value >= 5)
					return (string)de.Key;
			}
			return null;
		}
		internal string CalculateInsufficientPrefix(string[] names) {
			string result = string.Empty;
			for(int len = 1; ; ++len) {
				string suspect = CalculateInsufficientPrefixForLength(names, len);
				if(suspect == null)
					return result;
				result = suspect;
			}
		}
		protected internal virtual string GetAbbreviatedDayName(string abbrDayName, string insufficientPrefix) {
			if(abbrDayName.Length > insufficientPrefix.Length && abbrDayName.StartsWith(insufficientPrefix))
				abbrDayName = abbrDayName.Substring(insufficientPrefix.Length);
			return abbrDayName;
		}
		protected Size DayCellsSize { get; private set; }
		protected Size WeekDaysAbbreviationsSize { get; private set; }
		protected Size WeekNumbersSize { get; private set; }
		protected internal virtual Size CalcBestSize(GraphicsCache cache) {
			DayCellsSize = CalcDayCellsSize();
			WeekDaysAbbreviationsSize = CalcWeekDaysAbbreviationSize();
			WeekNumbersSize = CalcWeekNumbersSize();
			Size bottomAreaSize = CalcBottomAreaSize();
			Size res = DayCellsSize;
			res.Width = Math.Max(res.Width, WeekDaysAbbreviationsSize.Width);
			res.Height += WeekDaysAbbreviationsSize.Height + WeekDayAbbreviationToDayCellIndent;
			if(!bottomAreaSize.IsEmpty) {
				res.Width = Math.Max(bottomAreaSize.Width, res.Width);
				res.Height += bottomAreaSize.Height + DayCellsToBottomAreaSizeIndent;
			}
			if(!WeekNumbersSize.IsEmpty) {
				res.Height = Math.Max(res.Height, WeekNumbersSize.Height);
				res.Width += WeekNumbersSize.Width + WeekNumbersToDayCellsIndent;
			}
			if(ShowHeader) { 
				Size headerSize = Header.CalcBestSize();
				res.Height += headerSize.Height;
			}
			return res;
		}
		protected virtual Size CalcBottomAreaSize() {
			return new Size(0, 5);
		}
		protected internal virtual DateEditCalendarViewType View {
			get;
			set;
		}
		protected virtual int WeekDayAbbreviationToDayCellIndent { get { return 5; } }
		protected virtual int DayCellsToBottomAreaSizeIndent { get { return 5; } }
		protected virtual int WeekNumbersToDayCellsIndent { get { return 7; } }
		protected virtual Size CalcWeekNumbersSize() {
			if(!ShowWeekNumbers)
				return Size.Empty;
			return new Size(CalendarInfo.WeekNumberSize.Width, CalendarInfo.WeekNumberSize.Height * GetCellRowCount());
		}
		protected virtual Size CalcWeekDaysAbbreviationSize() {
			return new Size(CalendarInfo.MonthInfoCellSize.Width * GetCellColumnCount(), CalendarInfo.WeekDayTextSize.Height + CalendarInfo.WeekDayVertIndent * 2);
		}
		protected virtual Size CalcDayCellsSize() {
			return new Size(CalendarInfo.MonthInfoCellSize.Width * GetCellColumnCount(), CalendarInfo.MonthInfoCellSize.Height * GetCellRowCount());
		}
		protected int GetCellColumnCount() {
			return CalendarInfo.GetCalendarCellColumnCount(View); 
		}
		protected int GetCellRowCount() {
			return CalendarInfo.GetCalendarCellRowCount(View); 
		}
		public Rectangle WeekDayAbbreviationsBounds { get; private set; }
		public Rectangle DayCellsBounds { get; private set; }
		public Rectangle WeekNumbersBounds { get; private set; }
		public virtual void LayoutCalendar(Rectangle bounds) {
			CalcBestSize(CalendarInfo.GInfo.Cache);
			Bounds = bounds;
			ContentBounds = CalcContentBounds();
			CalcFirstVisibleDate();
			WeekDayAbbreviationsBounds = CalcWeekDaysAbbreviationBounds();
			WeekNumbersBounds = CalcWeekNumbersBounds();
			DayCellsBounds = CalcDayCellsBounds();
			ActualCellSize = CalcActualCellSize();
			DayCellsBounds = UpdateDayCellsBounds();
			CalcWeekDayAbbreviations();
			CalcDayCells();
			CalcWeekNumbers();
			UpdateExistingCellsState();
		}
		protected virtual Rectangle CalcContentBounds() {
			return Bounds;
		}
		protected virtual void CalcWeekNumbers() {
			WeekNumberCells.Clear();
			if(!ShowWeekNumbers)
				return;
			int x = WeekNumbersBounds.X, y = WeekNumbersBounds.Y;
			DateTime cur = MinValue;
			int rowCount = GetCellRowCount();
			int columnCount = GetCellColumnCount();
			for(int row = 0; row < rowCount; row++) {
				try { 
					cur = FirstVisibleDate.AddDays(row * columnCount - PenaltyIndex); }
				catch {
					break;
				}
				CalendarCellViewInfo weekCell = CreateDayCell(cur);
				int? weekNumber = GetWeekNumber(cur);
				weekCell.Text = weekNumber.HasValue ? weekNumber.ToString() : "";
				weekCell.Bounds = CalcWeekCellBounds(weekCell.Text, x, y);
				WeekNumberCells.Add(weekCell);
				y += ActualCellSize.Height;
			}
		}
		protected virtual void CalcDayCells() {
			DayCells.Clear();
			int x = DayCellsBounds.X, y = DayCellsBounds.Y;
			DateTime date = MinValue;
			RowCount = GetCellRowCount();
			ColumnCount = GetCellColumnCount();
			bool isRtl = WindowsFormsSettings.GetIsRightToLeftLayout(Calendar);
			for(int row = 0; row < RowCount; row++) {
				x = isRtl? DayCellsBounds.Right - ActualCellSize.Width: DayCellsBounds.X;
				for(int column = 0; column < ColumnCount; column++) {
					bool correctDate = true;
					if(row == 0 && column < PenaltyIndex) {
						x += isRtl ? -ActualCellSize.Width : ActualCellSize.Width;
						continue;
					}
					try {
						date = CalcDate(row, column, out correctDate);  
					}
					catch {
						row = RowCount;
						break;
					}
					if(correctDate && CanAddDate(date)) {
						CalendarCellViewInfo cell = CreateDayCell(date);
						cell.UpdateVisualState();
						cell.Bounds = CalcCellBounds(x, y);
						cell.Text = GetCellText(date, row, column);
						cell.ContentBounds = CalcDayCellContentBounds(cell.Bounds);
						cell.CalcTextBounds();
						UpdateDayCell(cell);
						DayCells.Add(cell);
						CalendarInfo.AddCellToNavigationGrid(cell, Row, Column, row, column);
					}
					x += isRtl? -ActualCellSize.Width: ActualCellSize.Width;
				}
				y += ActualCellSize.Height;
			}
		}
		public int Row { get; internal set; }
		public int Column { get; internal set; }
		private System.DateTime CalcDate(int row, int column, out bool correctDate) {
			correctDate = true;
			if(View == DateEditCalendarViewType.QuarterInfo)
				return new DateTime(CurrentDate.Year, row * 6 + column * 3 + 1, 1);
			if(View == DateEditCalendarViewType.YearInfo)
				return new DateTime(CurrentDate.Year, 1 + row * 4 + column, 1);
			if(View == DateEditCalendarViewType.YearsInfo) {
				int beginYear = (CurrentDate.Year / 10) * 10 - 1;
				int currYear = beginYear + row * 4 + column;
				if(currYear <= 0 || currYear >= 10000) {
					correctDate = false;
					return DateTime.MinValue;
				}
				return new DateTime(currYear, 1, 1);
			}
			if(View == DateEditCalendarViewType.YearsGroupInfo) {
				int beginYearGroup = (CurrentDate.Year / 100) * 100 - 10;
				int currYearGroup = beginYearGroup + (row * 4 + column) * 10;
				if(currYearGroup < 0 || currYearGroup >= 10000) {
					correctDate = false;
					return DateTime.MinValue;
				}
				int endYearGroup = currYearGroup + 9;
				if(currYearGroup == 0) currYearGroup = 1;
				return new DateTime(currYearGroup, 1, 1);
			}
			return FirstVisibleDate.AddDays(row * GetCellColumnCount() + column - PenaltyIndex);
		}
		protected bool IsLocalizedDateValid(DateTime date) {
			return date >= CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime && date <= CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime;
		}
		protected virtual string GetCellText(System.DateTime date, int row, int column) {
			if(View == DateEditCalendarViewType.QuarterInfo)
				return (row * 2 + column + 1).ToString();
			if(View == DateEditCalendarViewType.YearInfo)
				return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);
			if(View == DateEditCalendarViewType.YearsInfo) {
				if(!IsLocalizedDateValid(date))
					return "";
				return date.ToString("yyyy", CultureInfo.CurrentCulture);
			}
			if(View == DateEditCalendarViewType.YearsGroupInfo){
				int endYearsGroup = date.Year + 9;
				DateTime endYear = new DateTime(endYearsGroup, 1, 1);
				if(!IsLocalizedDateValid(date) || !IsLocalizedDateValid(endYear))
					return "";
				return date.ToString("yyyy", CultureInfo.CurrentCulture) + "-\n" + endYear.ToString("yyyy", CultureInfo.CurrentCulture);
			}
			return date.Day.ToString();
		}
		protected virtual void UpdateDayCell(CalendarCellViewInfo cell) {
		}
		protected virtual void CalcWeekDayAbbreviations() {
			WeekAbbreviationCells.Clear();
			if(View != DateEditCalendarViewType.MonthInfo)
				return;
			int columnCount = GetCellColumnCount();
			for(int day = 0; day < columnCount; day++) {
				string abbrDayName = CalendarInfo.GetAbbreviatedDayName(day);
				CalendarCellViewInfo info = CreateDayCell(DateTime.Now);
				info.Appearance = (AppearanceObject)CalendarInfo.PaintStyle.WeekDayAppearance.Clone();
				info.Text = abbrDayName;
				int cellPosition = WindowsFormsSettings.GetIsRightToLeftLayout(Calendar) ? columnCount - 1 - day : day;
				info.Bounds = new Rectangle(WeekDayAbbreviationsBounds.X + cellPosition * CalendarInfo.CellSize.Width, WeekDayAbbreviationsBounds.Y, CalendarInfo.CellSize.Width, WeekDayAbbreviationsBounds.Height);
				info.ContentBounds = info.Bounds;
				WeekAbbreviationCells.Add(info);
			}
		}
		protected virtual Rectangle UpdateDayCellsBounds() {
			return new Rectangle(DayCellsBounds.X, DayCellsBounds.Y, ActualCellSize.Width * GetCellColumnCount(), ActualCellSize.Height * GetCellRowCount());
		}
		protected virtual Rectangle CalcDayCellsBounds() {
			Rectangle rect = ContentBounds;
			if(View == DateEditCalendarViewType.MonthInfo) {
				rect.Y += WeekDayAbbreviationsBounds.Height + WeekDayAbbreviationToDayCellIndent;
				rect.Height -= WeekDayAbbreviationsBounds.Height + WeekDayAbbreviationToDayCellIndent;
			}
			if(ShowWeekNumbers && View == DateEditCalendarViewType.MonthInfo) {
				rect.X += CalendarInfo.WeekNumberSize.Width + WeekNumbersToDayCellsIndent;
				rect.Width -= CalendarInfo.WeekNumberSize.Width + WeekNumbersToDayCellsIndent;
			}
			return rect;			
		}
		protected virtual Rectangle CalcWeekNumbersBounds() {
			if(!ShowWeekNumbers)
				return Rectangle.Empty;
			int height = Math.Max(DayCellsSize.Height, WeekNumbersSize.Height);
			return new Rectangle(ContentBounds.X, ContentBounds.Y + WeekDaysAbbreviationsSize.Height + WeekDayAbbreviationToDayCellIndent, WeekNumbersSize.Width, height);
		}
		protected virtual Rectangle CalcWeekDaysAbbreviationBounds() {
			int width = Math.Max(DayCellsSize.Width, WeekDaysAbbreviationsSize.Width);
			return new Rectangle(ContentBounds.Right - width, ContentBounds.Y, width, WeekDaysAbbreviationsSize.Height);
		}
		public Rectangle Bounds { get; private set; }
		public virtual void CalcHitInfo(CalendarHitInfo hitInfo) {
			if(Calendar.ReadOnly)
				return;
			foreach(CalendarCellViewInfo cell in DayCells) {
				if(cell.Bounds.Contains(hitInfo.HitPoint)) {
					hitInfo.SetCell(cell);
					break;
				}
			}
		}
		protected internal virtual bool IsPressedDate(CalendarCellViewInfo cell) {
			return CalendarInfo.PressedInfo.IsInCell && CalendarInfo.PressedInfo.Cell.Date == cell.Date;
		}
		protected internal virtual void UpdateVisualInfo() {
			UpdateExistingCellsState();
		}
		public virtual Color WeekNumbersSeparatorColor {
			get { return CalendarInfo.PaintStyle.WeekNumbersSeparatorColor; }
		}
		public Point WeekNumbersSeparatorEndPoint { get { return new Point(WeekNumbersBounds.Right + WeekNumbersToDayCellsIndent / 2, WeekNumbersBounds.Top); } }
		public Point WeekNumbersSeparatorStartPoint { get { return new Point(WeekNumbersBounds.Right + WeekNumbersToDayCellsIndent / 2, WeekNumbersBounds.Bottom); } }
		public Color WeekDaysSeparatorColor { get { return CalendarInfo.PaintStyle.WeekDaySeparatorColor; } }
		public Point WeekDaysSeparatorStartPoint { get { return new Point(WeekDayAbbreviationsBounds.Left, WeekDayAbbreviationsBounds.Bottom + WeekDayAbbreviationToDayCellIndent / 2); } }
		public Point WeekDaysSeparatorEndPoint { get { return new Point(WeekDayAbbreviationsBounds.Right, WeekDayAbbreviationsBounds.Bottom + WeekDayAbbreviationToDayCellIndent / 2); } }
		public int RowCount { get; protected set; }
		public int ColumnCount { get; protected set; }
	}
}
