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
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class TimelineViewPainter : ViewPainterBase {
		TimelinePainter timelinePainter;
		SelectionBarPainter selectionBarPainter;
		TimeIndicatorPainter timeIndicatorPainter;
		protected TimelineViewPainter() {
			this.timelinePainter = CreateTimelinePainter();
			this.selectionBarPainter = CreateSelectionBarPainter();
		}
		internal TimelinePainter TimelinePainter { get { return timelinePainter; } } 
		public SelectionBarPainter SelectionBarPainter { get { return selectionBarPainter; } }
		public override bool HideSelection {
			get {
				return base.HideSelection;
			}
			set {
				base.HideSelection = value;
				timelinePainter.HideSelection = value;
				selectionBarPainter.HideSelection = value;
			}
		}
		public override void Initialize() {
			base.Initialize();
			this.timeIndicatorPainter = CreateTimeIndicatorPainter();
		}
		protected internal TimeIndicatorPainter TimeIndicatorPainter { get { return timeIndicatorPainter; } }
		protected abstract TimeIndicatorPainter CreateTimeIndicatorPainter();
		protected virtual SelectionBarPainter CreateSelectionBarPainter() {
			return new SelectionBarPainter();
		}
		protected virtual TimelinePainter CreateTimelinePainter() {
			return new TimelinePainter();
		}
		protected internal override AppointmentPainter CreateAppointmentPainter() {
			return new TimelineAppointmentPainter();
		}
		protected internal override ViewInfoPainterBase SelectCellsLayoutPainter() {
			return HorizontalHeaderPainter;
		}
		public override void DrawHeaders(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			TimelineViewInfo timelineViewInfo = (TimelineViewInfo)viewInfo;
			SchedulerControl control = viewInfo.View.Control;
			DrawScaleHeaders(cache, timelineViewInfo.ScaleLevels, control);
			HorizontalHeaderPainter.DrawHeaders(cache, timelineViewInfo.Corners, control);
			if (viewInfo.View.GroupType != SchedulerGroupType.None) {
				VerticalHeaderPainter.DrawHeaders(cache, timelineViewInfo.ResourceHeaders, control);
				VerticalHeaderPainter.DrawHeaders(cache, timelineViewInfo.GroupSeparators, control);
			}
		}
		protected override void DrawTimeIndicator(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			TimelineViewInfo dayViewInfo = (TimelineViewInfo)viewInfo;
			SchedulerControl control = viewInfo.View.Control;
			TimeIndicatorPainter.DrawTimeIndicator(cache, dayViewInfo.TimeIndicator, control);
		}
		protected virtual void DrawScaleHeaders(GraphicsCache cache, SchedulerHeaderLevelCollection levels, SchedulerControl control) {
			int count = levels.Count;
			for (int i = 0; i < count; i++)
				HorizontalHeaderPainter.DrawHeaders(cache, levels[i].Headers, control);
		}
		protected internal override void DrawCellContainers(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			TimelineViewInfo timelineViewInfo = (TimelineViewInfo)viewInfo;
			SchedulerControl control = viewInfo.View.Control;
			DrawSelectionBar(cache, timelineViewInfo.SelectionBar, control);
			DrawTimelines(cache, timelineViewInfo.Timelines, control);
		}
		protected virtual void DrawSelectionBar(GraphicsCache cache, SelectionBar selectionBar, SchedulerControl control) {
			selectionBarPainter.Draw(cache, selectionBar, control);
		}
		protected virtual void DrawTimelines(GraphicsCache cache, SchedulerViewCellContainerCollection timelines, SchedulerControl control) {
			int count = timelines.Count;
			for (int i = 0; i < count; i++)
				timelinePainter.Draw(cache, (Timeline)timelines[i], control);
		}
	}
	public class SelectionBarPainter : TimeCellPainter {
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public SelectionBarPainter() {
		}
		public virtual void Draw(GraphicsCache cache, SelectionBar viewInfo, SchedulerControl control) {
			DrawCells(cache, viewInfo.Cells, control);
		}
	}
	public class TimelinePainter : TimeCellPainter {
		public TimelinePainter() {
		}
		public virtual void Draw(GraphicsCache cache, TimelineBase viewInfo, ISupportCustomDraw customDrawProvider) {
			DrawCells(cache, viewInfo.Cells, customDrawProvider);
		}
	}
	#region Paint styles
	public class TimelineViewPainterFlat : TimelineViewPainter {
		public TimelineViewPainterFlat() {
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
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorFlatPainter();
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
												   HeaderAppearanceFlatHelper.CreateSelectionAppearance(HorzAlignment.Center),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class TimelineViewPainterUltraFlat : TimelineViewPainter {
		public TimelineViewPainterUltraFlat() {
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
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorUltraFlatPainter();
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
												   HeaderAppearanceUltraFlatHelper.CreateSelectionAppearance(HorzAlignment.Center),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonUltraFlatAppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonUltraFlatAppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class TimelineViewPainterOffice2003 : TimelineViewPainter {
		public TimelineViewPainterOffice2003() {
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
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorOffice2003Painter();
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
												   HeaderAppearanceOffice2003Helper.CreateSelectionAppearance(HorzAlignment.Center),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonOffice2003AppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonOffice2003AppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class TimelineViewPainterWindowsXP : TimelineViewPainter {
		public TimelineViewPainterWindowsXP() {
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
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorWindowsXPPainter();
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
												   HeaderAppearanceFlatHelper.CreateSelectionAppearance(HorzAlignment.Center),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class TimelineViewPainterSkin : TimelineViewPainter {
		UserLookAndFeel lookAndFeel;
		public TimelineViewPainterSkin(UserLookAndFeel lookAndFeel) {
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
			return new TimelineAppointmentSkinPainter(lookAndFeel);
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new TimeIndicatorSkinPainter(lookAndFeel);
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
												   HeaderAppearanceSkinHelper.CreateSelectionAppearance(lookAndFeel, HorzAlignment.Center),
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
	public class TimelineAppointmentSkinPainter : AppointmentSkinPainter {
		public TimelineAppointmentSkinPainter(UserLookAndFeel lookAndFeel)
			: base(lookAndFeel) {
		}
		public override int LeftPadding { get { return 0; } }
		public override int RightPadding { get { return 0; } }
	}
	public class TimelineAppointmentPainter : AppointmentPainter {
		public TimelineAppointmentPainter()
			: base() {
		}
		public override int LeftPadding { get { return 0; } }
		public override int RightPadding { get { return 0; } }
	}
	#endregion
}
