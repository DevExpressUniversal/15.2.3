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
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler {
}
namespace DevExpress.XtraScheduler.Drawing  {
	public abstract class WeekViewPainter : ViewPainterBase {
		SingleWeekPainter horizontalSingleWeekPainter;
		SingleWeekPainter verticalSingleWeekPainter;
		protected WeekViewPainter() {
		}
		public override bool HideSelection {
			get {
				return base.HideSelection;
			}
			set {
				base.HideSelection = value;
				horizontalSingleWeekPainter.CellPainter.HideSelection = value;
				horizontalSingleWeekPainter.HeaderPainter.HideSelection = value;
				verticalSingleWeekPainter.CellPainter.HideSelection = value;
				verticalSingleWeekPainter.HeaderPainter.HideSelection = value;
			}
		}
		public override void Initialize() {
			base.Initialize();
			this.horizontalSingleWeekPainter = CreateHorizontalSingleWeekPainter();
			this.verticalSingleWeekPainter = CreateVerticalSingleWeekPainter();
		}
		protected internal SingleWeekPainter HorizontalSingleWeekPainter { get { return horizontalSingleWeekPainter; } }
		protected internal SingleWeekPainter VerticalSingleWeekPainter { get { return verticalSingleWeekPainter; } }
		protected internal override AppointmentPainter CreateAppointmentPainter() {
			return new AppointmentPainter();
		}
		protected internal override ViewInfoPainterBase SelectCellsLayoutPainter() {
			return HorizontalHeaderPainter;
		}
		bool ShouldUseHorizontalWeek(SchedulerViewBase view) {
			return view.FactoryHelper.CalcActualGroupType(view) == SchedulerGroupType.Date;
		}
		protected virtual SingleWeekPainter SelectSingleWeekPainter(SchedulerViewBase view) {
			return (ShouldUseHorizontalWeek(view)) ? HorizontalSingleWeekPainter : VerticalSingleWeekPainter;
		}
		protected virtual SingleWeekPainter CreateHorizontalSingleWeekPainter() {
			return new SingleWeekPainter(new HorizontalSingleWeekHeaderPainter(HorizontalHeaderPainter)); 
		}
		protected virtual SingleWeekPainter CreateVerticalSingleWeekPainter() {
			return new SingleWeekPainter(HorizontalHeaderPainter); 
		}
		public override void DrawHeaders(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			WeekViewInfo weekViewInfo = (WeekViewInfo)viewInfo;
			SchedulerControl control = viewInfo.View.Control;
			HorizontalHeaderPainter.DrawHeaders(cache, weekViewInfo.WeekDaysHeaders, control);
			HorizontalHeaderPainter.DrawHeaders(cache, weekViewInfo.Corners, control);
			SchedulerViewBase view = viewInfo.View;
			if (ShouldUseHorizontalWeek(view)) {
				VerticalHeaderPainter.DrawHeaders(cache, weekViewInfo.ResourceHeaders, control);
				VerticalHeaderPainter.DrawHeaders(cache, weekViewInfo.GroupSeparators, control);
			}
			else {
				HorizontalHeaderPainter.DrawHeaders(cache, weekViewInfo.ResourceHeaders, control);
				HorizontalHeaderPainter.DrawHeaders(cache, weekViewInfo.GroupSeparators, control);
			}
		}
		protected internal override void DrawCellContainers(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			SingleWeekPainter painter = SelectSingleWeekPainter(viewInfo.View);
			DrawWeeks(cache, viewInfo.CellContainers, painter, viewInfo.View.Control);
		}
		protected virtual void DrawWeeks(GraphicsCache cache, SchedulerViewCellContainerCollection weeks, SingleWeekPainter painter, SchedulerControl control) {
			int count = weeks.Count;
			for(int i = 0; i < count; i++)
				painter.Draw(cache, (SingleWeekViewInfo)weeks[i], control);
		}
	}
	public class SingleWeekPainter : ViewInfoPainterBase {
		SingleWeekCellPainter cellPainter;
		SchedulerHeaderPainter headerPainter;
		public SingleWeekPainter(SchedulerHeaderPainter headerPainter) {
			this.headerPainter = headerPainter;
			this.cellPainter = CreateCellPainter();
		}
		internal SingleWeekCellPainter CellPainter { get { return cellPainter; } }
		internal SchedulerHeaderPainter HeaderPainter { get { return headerPainter; } }
		protected virtual SingleWeekCellPainter CreateCellPainter() {
			return new SingleWeekCellPainter();
		}
		public virtual void Draw(GraphicsCache cache, SingleWeekViewInfo viewInfo, ISupportCustomDraw customDrawProvider) {
			SchedulerViewCellBaseCollection cells = viewInfo.Cells;
			int count = cells.Count;
			for (int i = 0; i < count; i++)
				cellPainter.Draw(cache, (SingleWeekCellBase)cells[i], this.headerPainter, customDrawProvider);
		}
	}
	public class SingleWeekCellPainter : SchedulerViewCellPainter { 
		public SingleWeekCellPainter() {
		}
		public virtual void Draw(GraphicsCache cache, SingleWeekCellBase viewInfo, SchedulerHeaderPainter painter, ISupportCustomDraw customDrawProvider) {
			DrawContent(cache, viewInfo, customDrawProvider);
			DrawHeader(cache, viewInfo, painter, customDrawProvider);
		}
		protected virtual void DrawContent(GraphicsCache cache, SingleWeekCellBase viewInfo, ISupportCustomDraw customDrawProvider) {
			DefaultDrawDelegate defaultDraw = delegate() { DrawContentCore(cache, viewInfo); };
			if (viewInfo.RaiseCustomDrawEvent(cache, customDrawProvider, defaultDraw))
				return;
			defaultDraw();
		}
		protected virtual void DrawContentCore(GraphicsCache cache, SingleWeekCellBase viewInfo) {
			DrawBackground(cache, viewInfo);
			DrawBorders(cache, viewInfo);
		}
		protected virtual void DrawBackground(GraphicsCache cache, SingleWeekCellBase viewInfo) {
			viewInfo.Appearance.FillRectangle(cache, viewInfo.Bounds);
		}
		protected virtual void DrawHeader(GraphicsCache cache, SingleWeekCellBase viewInfo, SchedulerHeaderPainter painter, ISupportCustomDraw customDrawProvider) {
			painter.Draw(cache, viewInfo.Header, customDrawProvider);
		}
	}
	public class HorizontalSingleWeekHeaderPainter : SchedulerHeaderPainter {
		SchedulerHeaderPainter wrappedPainter;
		public HorizontalSingleWeekHeaderPainter(SchedulerHeaderPainter wrappedPainter) {
			this.wrappedPainter = wrappedPainter;
		}
		internal SchedulerHeaderPainter WrappedPainter { get { return wrappedPainter; } }
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected internal override void DrawBackground(GraphicsCache cache, SchedulerHeader header) {
			if (header.Alternate)
				wrappedPainter.DrawBackground(cache, header);
			else {
				bool isSelected = header.Selected && !HideSelection;
				if (isSelected)
					base.DrawBackground(cache, header);
			}
		}
		protected internal override void DrawUnderline(GraphicsCache cache, SchedulerHeader header) {
			if (header.Alternate)
				wrappedPainter.DrawUnderline(cache, header);
		}
		protected internal override AppearanceObject GetActualCaptionAppearance(SchedulerHeader header) {
			if (header.Alternate)
				return wrappedPainter.GetActualCaptionAppearance(header);
			else
				return base.GetActualCaptionAppearance(header);
		}
	}
	#region Paint styles
	public class WeekViewPainterFlat : WeekViewPainter {
		public WeekViewPainterFlat() {
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderFlatPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalFlatPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonFlatPainter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   HeaderAppearanceFlatHelper.CreateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateSelectionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceFlatHelper.CreateHeaderCaptionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceFlatHelper.CreateHeaderCaptionLineAppearance(),
												   CellHeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class WeekViewPainterUltraFlat : WeekViewPainter {
		public WeekViewPainterUltraFlat() {
		}
		public override int ViewAndScrollbarVerticalSeparatorWidth { get { return 1; } }
		public override int ViewAndScrollbarHorizontalSeparatorHeight { get { return 1; } }
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderUltraFlatPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalUltraFlatPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonUltraFlatPainter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   HeaderAppearanceUltraFlatHelper.CreateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceUltraFlatHelper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceUltraFlatHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceUltraFlatHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceUltraFlatHelper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceUltraFlatHelper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceUltraFlatHelper.CreateSelectionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceUltraFlatHelper.CreateHeaderCaptionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceUltraFlatHelper.CreateHeaderCaptionLineAppearance(),
												   CellHeaderAppearanceUltraFlatHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceUltraFlatHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonUltraFlatAppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonUltraFlatAppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class WeekViewPainterOffice2003 : WeekViewPainter {
		public WeekViewPainterOffice2003() {
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderOffice2003Painter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalOffice2003Painter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonOffice2003Painter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   HeaderAppearanceOffice2003Helper.CreateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceOffice2003Helper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceOffice2003Helper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceOffice2003Helper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceOffice2003Helper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceOffice2003Helper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceOffice2003Helper.CreateSelectionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceOffice2003Helper.CreateHeaderCaptionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceOffice2003Helper.CreateHeaderCaptionLineAppearance(),
												   CellHeaderAppearanceOffice2003Helper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceOffice2003Helper.CreateAlternateHeaderCaptionLineAppearance(),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonOffice2003AppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonOffice2003AppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class WeekViewPainterWindowsXP : WeekViewPainter {
		public WeekViewPainterWindowsXP() {
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderWindowsXPPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalWindowsXPPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonWindowsXPPainter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   HeaderAppearanceFlatHelper.CreateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateSelectionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceFlatHelper.CreateHeaderCaptionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceFlatHelper.CreateHeaderCaptionLineAppearance(),
												   CellHeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Far),
												   CellHeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class WeekViewPainterSkin : WeekViewPainter {
		UserLookAndFeel lookAndFeel;
		public WeekViewPainterSkin(UserLookAndFeel lookAndFeel) {
			if (lookAndFeel == null)
				Exceptions.ThrowArgumentException("lookAndFeel", lookAndFeel);
			this.lookAndFeel = lookAndFeel;
		}
		public override int ViewAndScrollbarVerticalSeparatorWidth { get { return SkinPainterHelper.GetViewAndScrollbarVerticalSeparatorWidth(lookAndFeel); } }
		public override int ViewAndScrollbarHorizontalSeparatorHeight { get { return SkinPainterHelper.GetViewAndScrollbarHorizontalSeparatorHeight(lookAndFeel); } }
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new SchedulerHeaderSkinPainter(lookAndFeel);
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new SchedulerHeaderVerticalSkinPainter(lookAndFeel);
		}
		protected override MoreButtonPainter CreateMoreButtonPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new MoreButtonSkinPainter(lookAndFeel);
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new NavigationButtonSkinPainter(lookAndFeel);
		}
		protected internal override AppointmentPainter CreateAppointmentPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new AppointmentSkinPainter(lookAndFeel);
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   HeaderAppearanceSkinHelper.CreateHeaderCaptionAppearance(HorzAlignment.Center, lookAndFeel),
												   HeaderAppearanceSkinHelper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceSkinHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center, lookAndFeel),
												   HeaderAppearanceSkinHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceSkinHelper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center, lookAndFeel),
												   HeaderAppearanceSkinHelper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceSkinHelper.CreateSelectionAppearance(lookAndFeel, HorzAlignment.Far),
												   CellHeaderAppearanceSkinHelper.CreateHeaderCaptionAppearance(HorzAlignment.Far, lookAndFeel),
												   CellHeaderAppearanceSkinHelper.CreateHeaderCaptionLineAppearance(),
												   CellHeaderAppearanceSkinHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Far, lookAndFeel),
												   CellHeaderAppearanceSkinHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonSkinAppearancesHelper.CreateNavigationButtonAppearance(HorzAlignment.Center, lookAndFeel),
												   NavigationButtonSkinAppearancesHelper.CreateNavigationButtonDisabledAppearance(HorzAlignment.Center, lookAndFeel)
											   };
		}
		#endregion
		public override void DrawViewAndScrollBarSeparator(GraphicsCache cache, Rectangle bounds) {
			SkinPainterHelper.DrawViewAndDateTimeSeparator(cache, lookAndFeel, bounds);
		}
	}
	#endregion
}
