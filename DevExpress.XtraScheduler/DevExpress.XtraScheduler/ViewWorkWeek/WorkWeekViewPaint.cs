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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class WorkWeekViewPainter : DayViewPainter {
		protected WorkWeekViewPainter() {
		}
	}
	#region Paint styles
	public class WorkWeekViewPainterFlat : WorkWeekViewPainter {
		public WorkWeekViewPainterFlat() {
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderFlatPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalFlatPainter();
		}
		protected override DayViewColumnPainter CreateColumnPainter() {
			return new DayViewColumnFlatPainter();
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			return new TimeRulerFlatPainter();
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorFlatPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonFlatPainter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRuler, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, SystemColors.ControlDark, SystemColors.Control)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerHourLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowLine, new AppearanceDefault(SystemColors.ControlText, Color.Red, Color.Transparent, Color.Red)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowArea, new AppearanceDefault(SystemColors.ControlText, Color.LightGoldenrodYellow, Color.Transparent, Color.Orange, System.Drawing.Drawing2D.LinearGradientMode.Vertical)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayArea, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayAreaSeparator, new AppearanceDefault(SystemColors.ControlText, SystemColors.WindowText, Color.Transparent, SystemColors.WindowText)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.SelectedAllDayArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, Color.Transparent, SystemColors.Window)),
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
	public class WorkWeekViewPainterUltraFlat : WorkWeekViewPainter {
		public WorkWeekViewPainterUltraFlat() {
		}
		public override int ViewAndScrollbarVerticalSeparatorWidth { get { return 1; } }
		public override int ViewAndScrollbarHorizontalSeparatorHeight { get { return 1; } }
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderUltraFlatPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalUltraFlatPainter();
		}
		protected override DayViewColumnPainter CreateColumnPainter() {
			return new DayViewColumnUltraFlatPainter();
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			return new TimeRulerUltraFlatPainter();
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorUltraFlatPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonUltraFlatPainter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRuler, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, SystemColors.ControlDark, SystemColors.Control)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerHourLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowLine, new AppearanceDefault(SystemColors.ControlText, Color.Red, Color.Transparent, Color.Red)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowArea, new AppearanceDefault(SystemColors.ControlText, Color.LightGoldenrodYellow, Color.Transparent, Color.Orange, System.Drawing.Drawing2D.LinearGradientMode.Vertical)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayArea, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayAreaSeparator, new AppearanceDefault(SystemColors.ControlText, SystemColors.WindowText, Color.Transparent, SystemColors.WindowText)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.SelectedAllDayArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, Color.Transparent, SystemColors.Window)),
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
	public class WorkWeekViewPainterOffice2003 : WorkWeekViewPainter {
		public WorkWeekViewPainterOffice2003() {
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderOffice2003Painter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalOffice2003Painter();
		}
		protected override DayViewColumnPainter CreateColumnPainter() {
			return new DayViewColumnOffice2003Painter();
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			return new TimeRulerOffice2003Painter();
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorOffice2003Painter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonOffice2003Painter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRuler, new AppearanceDefault(SystemColors.WindowText, ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Header]), ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]), ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Header2]))),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerLine, new AppearanceDefault(SystemColors.ControlText, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]), Color.Transparent, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]))),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerHourLine, new AppearanceDefault(SystemColors.ControlText, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]), Color.Transparent, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]))),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowLine, new AppearanceDefault(SystemColors.ControlText, Color.Red, Color.Transparent, Color.Red)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowArea, new AppearanceDefault(SystemColors.ControlText, Color.LightGoldenrodYellow, Color.Transparent, Color.Orange, System.Drawing.Drawing2D.LinearGradientMode.Vertical)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayArea, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayAreaSeparator, new AppearanceDefault(SystemColors.ControlText, SystemColors.WindowText, Color.Transparent, SystemColors.WindowText)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.SelectedAllDayArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, Color.Transparent, SystemColors.Window)),
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
	public class WorkWeekViewPainterWindowsXP : WorkWeekViewPainter {
		public WorkWeekViewPainterWindowsXP() {
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderWindowsXPPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalWindowsXPPainter();
		}
		protected override DayViewColumnPainter CreateColumnPainter() {
			return new DayViewColumnWindowsXPPainter();
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			return new TimeRulerWindowsXPPainter();
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorWindowsXPPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonWindowsXPPainter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRuler, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, SystemColors.ControlDark, SystemColors.Control)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerHourLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowLine, new AppearanceDefault(SystemColors.ControlText, Color.Red, Color.Transparent, Color.Red)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowArea, new AppearanceDefault(SystemColors.ControlText, Color.LightGoldenrodYellow, Color.Transparent, Color.Orange, System.Drawing.Drawing2D.LinearGradientMode.Vertical)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayArea, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayAreaSeparator, new AppearanceDefault(SystemColors.ControlText, SystemColors.WindowText, Color.Transparent, SystemColors.WindowText)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.SelectedAllDayArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, Color.Transparent, SystemColors.Window)),
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
	public class WorkWeekViewPainterSkin : WorkWeekViewPainter {
		UserLookAndFeel lookAndFeel;
		public WorkWeekViewPainterSkin(UserLookAndFeel lookAndFeel) {
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
		protected override DayViewColumnPainter CreateColumnPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new DayViewColumnSkinPainter(lookAndFeel);
		}
		protected internal override AppointmentPainter CreateAllDayAppointmentPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new AppointmentSkinPainter(lookAndFeel);
		}
		protected internal override AppointmentPainter CreateTimeCellsAppointmentPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new DayViewTimeCellAppointmentSkinPainter(lookAndFeel);
		}
		protected override MoreButtonPainter CreateMoreButtonPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new MoreButtonSkinPainter(lookAndFeel);
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new NavigationButtonSkinPainter(lookAndFeel);
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new TimeRulerSkinPainter(lookAndFeel);
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new TimeIndicatorSkinPainter(lookAndFeel);
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRuler, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinRuler, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, Color.Transparent, SystemColors.Control), lookAndFeel)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerLine, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinRulerMinLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark), lookAndFeel)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerHourLine, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinRulerHourLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark), lookAndFeel)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowLine, new AppearanceDefault(SystemColors.ControlText, Color.Red, Color.Transparent, Color.Red)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowArea, new AppearanceDefault(SystemColors.ControlText, Color.LightGoldenrodYellow, Color.Transparent, Color.Orange, System.Drawing.Drawing2D.LinearGradientMode.Vertical)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayArea, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinAllDayArea, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark), lookAndFeel)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayAreaSeparator, new AppearanceDefault(SystemColors.ControlText, SystemColors.WindowText, Color.Transparent, SystemColors.WindowText)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.SelectedAllDayArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, Color.Transparent, SystemColors.Window)),
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
	#endregion
}
