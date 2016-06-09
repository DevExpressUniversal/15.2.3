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
namespace DevExpress.XtraScheduler.Drawing  {
	public abstract class MonthViewPainter : WeekViewPainter {
		protected MonthViewPainter() {
		}
		protected override SingleWeekPainter SelectSingleWeekPainter(SchedulerViewBase view) {
			return HorizontalSingleWeekPainter;
		}
	}
	#region Paint styles
	public class MonthViewPainterFlat : MonthViewPainter {
		public MonthViewPainterFlat() {
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
	public class MonthViewPainterUltraFlat : MonthViewPainter {
		public MonthViewPainterUltraFlat() {
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
	public class MonthViewPainterOffice2003 : MonthViewPainter {
		public MonthViewPainterOffice2003() {
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
	public class MonthViewPainterWindowsXP : MonthViewPainter {
		public MonthViewPainterWindowsXP() {
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
	public class MonthViewPainterSkin : MonthViewPainter {
		UserLookAndFeel lookAndFeel;
		public MonthViewPainterSkin(UserLookAndFeel lookAndFeel) {
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
		protected internal override AppointmentPainter CreateAppointmentPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new AppointmentSkinPainter(lookAndFeel);
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new NavigationButtonSkinPainter(lookAndFeel);
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
												   CellHeaderAppearanceSkinHelper.CreateHeaderCaptionAppearanceMonth(HorzAlignment.Far, lookAndFeel),
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
