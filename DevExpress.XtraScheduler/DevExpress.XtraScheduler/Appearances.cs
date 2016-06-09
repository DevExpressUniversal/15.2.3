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
using DevExpress.Utils;
using DevExpress.XtraScheduler.Drawing;
namespace DevExpress.XtraScheduler {
	#region BaseHeaderAppearance
	public class BaseHeaderAppearance : BaseAppearanceCollection {
		#region Fields
		AppearanceObject headerCaption;
		AppearanceObject headerCaptionLine;
		AppearanceObject alternateHeaderCaption;
		AppearanceObject alternateHeaderCaptionLine;
		AppearanceObject selection;
		#endregion
		protected override void CreateAppearances() {
			this.headerCaption = CreateAppearance(BaseHeaderAppearanceNames.HeaderCaption);
			this.headerCaptionLine = CreateAppearance(BaseHeaderAppearanceNames.HeaderCaptionLine);
			this.alternateHeaderCaption = CreateAppearance(BaseHeaderAppearanceNames.AlternateHeaderCaption);
			this.alternateHeaderCaptionLine = CreateAppearance(BaseHeaderAppearanceNames.AlternateHeaderCaptionLine);
			this.selection = CreateAppearance(BaseHeaderAppearanceNames.Selection);
		}
		#region Properties
		#region HeaderCaption
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("BaseHeaderAppearanceHeaderCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderCaption { get { return headerCaption; } protected set { headerCaption = value; } }
		bool ShouldSerializeHeaderCaption() { return HeaderCaption.ShouldSerialize(); }
		void ResetHeaderCaption() { HeaderCaption.Reset(); }
		#endregion
		#region HeaderCaptionLine
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("BaseHeaderAppearanceHeaderCaptionLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderCaptionLine { get { return headerCaptionLine; } protected set { headerCaptionLine = value; } }
		bool ShouldSerializeHeaderCaptionLine() { return HeaderCaptionLine.ShouldSerialize(); }
		void ResetHeaderCaptionLine() { HeaderCaptionLine.Reset(); }
		#endregion
		#region AlternateHeaderCaption
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("BaseHeaderAppearanceAlternateHeaderCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AlternateHeaderCaption { get { return alternateHeaderCaption; } protected set { alternateHeaderCaption = value; } }
		bool ShouldSerializeAlternateHeaderCaption() { return AlternateHeaderCaption.ShouldSerialize(); }
		void ResetAlternateHeaderCaption() { AlternateHeaderCaption.Reset(); }
		#endregion
		#region AlternateHeaderCaptionLine
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("BaseHeaderAppearanceAlternateHeaderCaptionLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AlternateHeaderCaptionLine { get { return alternateHeaderCaptionLine; } protected set { alternateHeaderCaptionLine = value; } }
		bool ShouldSerializeAlternateHeaderCaptionLine() { return AlternateHeaderCaptionLine.ShouldSerialize(); }
		void ResetAlternateHeaderCaptionLine() { AlternateHeaderCaptionLine.Reset(); }
		#endregion
		#region Selection
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("BaseHeaderAppearanceSelection"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Selection { get { return selection; } protected set { selection = value; } }
		bool ShouldSerializeSelection() { return Selection.ShouldSerialize(); }
		void ResetSelection() { Selection.Reset(); }
		#endregion
		#endregion
	}
	#endregion
	#region BaseViewAppearance
	public class BaseViewAppearance : BaseHeaderAppearance {
		#region Fields
		AppearanceObject appointment;
		AppearanceObject navigationButton;
		AppearanceObject navigationButtonDisabled;
		AppearanceObject resourceHeaderCaption;
		AppearanceObject resourceHeaderCaptionLine;
		#endregion
		#region Properties
		#region Appointment
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("BaseViewAppearanceAppointment"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appointment { get { return appointment; } }
		bool ShouldSerializeAppointment() { return Appointment.ShouldSerialize(); }
		void ResetAppointment() { Appointment.Reset(); }
		#endregion
		#region NavigationButton
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("BaseViewAppearanceNavigationButton"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject NavigationButton { get { return navigationButton; } }
		bool ShouldSerializeNavigationButton() { return NavigationButton.ShouldSerialize(); }
		void ResetNavigationButton() { NavigationButton.Reset(); }
		#endregion
		#region NavigationButtonDisabled
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("BaseViewAppearanceNavigationButtonDisabled"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject NavigationButtonDisabled { get { return navigationButtonDisabled; } }
		bool ShouldSerializeNavigationButtonDisabled() { return NavigationButtonDisabled.ShouldSerialize(); }
		void ResetNavigationButtonDisabled() { NavigationButtonDisabled.Reset(); }
		#endregion
		#region ResourceHeaderCaption
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("BaseViewAppearanceResourceHeaderCaption"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ResourceHeaderCaption { get { return resourceHeaderCaption; } }
		bool ShouldSerializeResourceHeaderCaption() { return ResourceHeaderCaption.ShouldSerialize(); }
		void ResetResourceHeaderCaption() { ResourceHeaderCaption.Reset(); }
		#endregion
		#region ResourceHeaderCaptionLine
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("BaseViewAppearanceResourceHeaderCaptionLine"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ResourceHeaderCaptionLine { get { return resourceHeaderCaptionLine; } }
		bool ShouldSerializeResourceHeaderCaptionLine() { return ResourceHeaderCaptionLine.ShouldSerialize(); }
		void ResetResourceHeaderCaptionLine() { ResourceHeaderCaptionLine.Reset(); }
		#endregion
		#endregion
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.appointment = CreateAppearance(SchedulerAppearanceNames.Appointment);
			this.navigationButton = CreateAppearance(SchedulerAppearanceNames.NavigationButton);
			this.navigationButtonDisabled = CreateAppearance(SchedulerAppearanceNames.NavigationButtonDisabled);
			this.resourceHeaderCaption = CreateAppearance(SchedulerAppearanceNames.ResourceHeaderCaption);
			this.resourceHeaderCaptionLine = CreateAppearance(SchedulerAppearanceNames.ResourceHeaderCaptionLine);
		}
	}
	#endregion
	#region SchedulerAppearance
	public class SchedulerAppearance : BaseViewAppearance {
	}
	#endregion
	#region DayViewAppearance
	public class DayViewAppearance : BaseViewAppearance {
		#region Appearance fields
		AppearanceObject timeRuler;
		AppearanceObject timeRulerLine;
		AppearanceObject timeRulerHourLine;
		AppearanceObject timeRulerNowLine;
		AppearanceObject timeRulerNowArea;
		AppearanceObject allDayArea;
		AppearanceObject allDayAreaSeparator;
		AppearanceObject selectedAllDayArea;
		#endregion
		#region Appearance properties
		#region AllDayArea
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAppearanceAllDayArea"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AllDayArea { get { return allDayArea; } }
		bool ShouldSerializeAllDayArea() { return AllDayArea.ShouldSerialize(); }
		void ResetAllDayArea() { AllDayArea.Reset(); }
		#endregion
		#region AllDayAreaSeparator
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAppearanceAllDayAreaSeparator"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AllDayAreaSeparator { get { return allDayAreaSeparator; } }
		bool ShouldSerializeAllDayAreaSeparator() { return AllDayAreaSeparator.ShouldSerialize(); }
		void ResetAllDayAreaSeparator() { AllDayAreaSeparator.Reset(); }
		#endregion
		#region SelectedAllDayArea
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAppearanceSelectedAllDayArea"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SelectedAllDayArea { get { return selectedAllDayArea; } }
		bool ShouldSerializeSelectedAllDayArea() { return SelectedAllDayArea.ShouldSerialize(); }
		void ResetSelectedAllDayArea() { SelectedAllDayArea.Reset(); }
		#endregion
		#region TimeRuler
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAppearanceTimeRuler"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject TimeRuler { get { return timeRuler; } }
		bool ShouldSerializeTimeRuler() { return TimeRuler.ShouldSerialize(); }
		void ResetTimeRuler() { TimeRuler.Reset(); }
		#endregion
		#region TimeRulerLine
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAppearanceTimeRulerLine"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject TimeRulerLine { get { return timeRulerLine; } }
		bool ShouldSerializeTimeRulerLine() { return TimeRulerLine.ShouldSerialize(); }
		void ResetTimeRulerLine() { TimeRulerLine.Reset(); }
		#endregion
		#region TimeRulerHourLine
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAppearanceTimeRulerHourLine"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject TimeRulerHourLine { get { return timeRulerHourLine; } }
		bool ShouldSerializeTimeRulerHourLine() { return TimeRulerHourLine.ShouldSerialize(); }
		void ResetTimeRulerHourLine() { TimeRulerHourLine.Reset(); }
		#endregion
		#region TimeRulerNowLine
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAppearanceTimeRulerNowLine"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject TimeRulerNowLine { get { return timeRulerNowLine; } }
		bool ShouldSerializeTimeRulerNowLine() { return TimeRulerNowLine.ShouldSerialize(); }
		void ResetTimeRulerNowLine() { TimeRulerNowLine.Reset(); }
		#endregion
		#region TimeRulerNowArea
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAppearanceTimeRulerNowArea"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject TimeRulerNowArea { get { return timeRulerNowArea; } }
		bool ShouldSerializeTimeRulerNowArea() { return TimeRulerNowArea.ShouldSerialize(); }
		void ResetTimeRulerNowArea() { TimeRulerNowArea.Reset(); }
		#endregion
		#endregion
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.allDayArea = CreateAppearance(DayViewAppearanceNames.AllDayArea);
			this.allDayAreaSeparator = CreateAppearance(DayViewAppearanceNames.AllDayAreaSeparator);
			this.selectedAllDayArea = CreateAppearance(DayViewAppearanceNames.SelectedAllDayArea);
			this.timeRuler = CreateAppearance(DayViewAppearanceNames.TimeRuler);
			this.timeRulerLine = CreateAppearance(DayViewAppearanceNames.TimeRulerLine);
			this.timeRulerHourLine = CreateAppearance(DayViewAppearanceNames.TimeRulerHourLine);
			this.timeRulerNowLine = CreateAppearance(DayViewAppearanceNames.TimeRulerNowLine);
			this.timeRulerNowArea = CreateAppearance(DayViewAppearanceNames.TimeRulerNowArea);
		}
	}
	#endregion
	#region WorkWeekViewAppearance
	public class WorkWeekViewAppearance : DayViewAppearance {
	}
	#endregion
	#region WeekViewAppearance
	public class WeekViewAppearance : BaseViewAppearance {
		#region Appearance fields
		AppearanceObject cellHeaderCaption;
		AppearanceObject cellHeaderCaptionLine;
		AppearanceObject todayCellHeaderCaption;
		AppearanceObject todayCellHeaderCaptionLine;
		BaseHeaderAppearanceContainer singleWeekCellHeaderAppearance;
		#endregion
		#region Appearance properties
		#region CellHeaderCaption
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekViewAppearanceCellHeaderCaption"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CellHeaderCaption { get { return cellHeaderCaption; } }
		bool ShouldSerializeCellHeaderCaption() { return CellHeaderCaption.ShouldSerialize(); }
		void ResetCellHeaderCaption() { CellHeaderCaption.Reset(); }
		#endregion
		#region CellHeaderCaptionLine
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekViewAppearanceCellHeaderCaptionLine"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CellHeaderCaptionLine { get { return cellHeaderCaptionLine; } }
		bool ShouldSerializeCellHeaderCaptionLine() { return CellHeaderCaptionLine.ShouldSerialize(); }
		void ResetCellHeaderCaptionLine() { CellHeaderCaptionLine.Reset(); }
		#endregion
		#region TodayCellHeaderCaption
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekViewAppearanceTodayCellHeaderCaption"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject TodayCellHeaderCaption { get { return todayCellHeaderCaption; } }
		bool ShouldSerializeTodayCellHeaderCaption() { return TodayCellHeaderCaption.ShouldSerialize(); }
		void ResetTodayCellHeaderCaption() { TodayCellHeaderCaption.Reset(); }
		#endregion
		#region TodayCellHeaderCaptionLine
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekViewAppearanceTodayCellHeaderCaptionLine"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject TodayCellHeaderCaptionLine { get { return todayCellHeaderCaptionLine; } }
		bool ShouldSerializeTodayCellHeaderCaptionLine() { return TodayCellHeaderCaptionLine.ShouldSerialize(); }
		void ResetTodayCellHeaderCaptionLine() { TodayCellHeaderCaptionLine.Reset(); }
		#endregion
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new AppearanceObject AlternateHeaderCaption { get { return base.AlternateHeaderCaption; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new AppearanceObject AlternateHeaderCaptionLine { get { return base.AlternateHeaderCaptionLine; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		internal BaseHeaderAppearanceContainer SingleWeekCellHeaderAppearance { get { return singleWeekCellHeaderAppearance; } }
		#endregion
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.cellHeaderCaption = CreateAppearance(WeekViewAppearanceNames.CellHeaderCaption);
			this.cellHeaderCaptionLine = CreateAppearance(WeekViewAppearanceNames.CellHeaderCaptionLine);
			this.todayCellHeaderCaption = CreateAppearance(WeekViewAppearanceNames.TodayCellHeaderCaption);
			this.todayCellHeaderCaptionLine = CreateAppearance(WeekViewAppearanceNames.TodayCellHeaderCaptionLine);
			this.singleWeekCellHeaderAppearance = new BaseHeaderAppearanceContainer(this);
		}
	}
	#endregion
	#region MonthViewAppearance
	public class MonthViewAppearance : WeekViewAppearance {
	}
	#endregion
	#region TimelineViewAppearance
	public class TimelineViewAppearance : BaseViewAppearance {
	}
	#endregion
	#region GanttViewAppearance
	public class GanttViewAppearance : TimelineViewAppearance {
		#region Appearance fields
		AppearanceObject dependency;
		AppearanceObject selectedDependency;
		#endregion
		#region Appearance properties
		#region Dependency
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("GanttViewAppearanceDependency"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Dependency { get { return dependency; } }
		bool ShouldSerializeDependency() { return Dependency.ShouldSerialize(); }
		void ResetDependency() { Dependency.Reset(); }
		#endregion
		#region SelectedDependency
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("GanttViewAppearanceSelectedDependency"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SelectedDependency { get { return selectedDependency; } }
		bool ShouldSerializeSelectedDependency() { return SelectedDependency.ShouldSerialize(); }
		void ResetSelectedDependency() { SelectedDependency.Reset(); }
		#endregion
		#endregion
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.dependency = CreateAppearance(GanttViewAppearanceNames.Dependency);
			this.selectedDependency = CreateAppearance(GanttViewAppearanceNames.SelectedDependency);
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Drawing {
	public static class SchedulerAppearanceNames { 
		public const string Appointment = "Appointment";
		public const string NavigationButton = "NavigationButton";
		public const string NavigationButtonDisabled = "NavigationButtonDisabled";
		public const string ResourceHeaderCaption = "ResourceHeaderCaption";
		public const string ResourceHeaderCaptionLine = "ResourceHeaderCaptionLine";
	}
	#region BaseHeaderAppearanceNames
	public static class BaseHeaderAppearanceNames {
		public const string HeaderCaption = "HeaderCaption";
		public const string HeaderCaptionLine = "HeaderCaptionLine";
		public const string AlternateHeaderCaption = "AlternateHeaderCaption";
		public const string AlternateHeaderCaptionLine = "AlternateHeaderCaptionLine";
		public const string Selection = "Selection";
	}
	#endregion
	#region BaseHeaderAppearanceContainer
	public class BaseHeaderAppearanceContainer : BaseHeaderAppearance {
		public BaseHeaderAppearanceContainer(WeekViewAppearance appearance)
			: base() {
			this.HeaderCaption = appearance.CellHeaderCaption;
			this.HeaderCaptionLine = appearance.CellHeaderCaptionLine;
			this.AlternateHeaderCaption = appearance.TodayCellHeaderCaption;
			this.AlternateHeaderCaptionLine = appearance.TodayCellHeaderCaptionLine;
			this.Selection = appearance.Selection;
			this.Names[BaseHeaderAppearanceNames.HeaderCaption] = HeaderCaption;
			this.Names[BaseHeaderAppearanceNames.HeaderCaptionLine] = HeaderCaptionLine;
			this.Names[BaseHeaderAppearanceNames.AlternateHeaderCaption] = AlternateHeaderCaption;
			this.Names[BaseHeaderAppearanceNames.AlternateHeaderCaptionLine] = AlternateHeaderCaptionLine;
			this.Names[BaseHeaderAppearanceNames.Selection] = Selection;
		}
		protected override void CreateAppearances() {
		}
	}
	#endregion
	#region DayViewAppearanceNames
	public static class DayViewAppearanceNames {
		public const string TimeRuler = "TimeRuler";
		public const string TimeRulerLine = "TimeRulerLine";
		public const string TimeRulerHourLine = "TimeRulerHourLine";
		public const string TimeRulerNowLine = "TimeRulerNowLine";
		public const string TimeRulerNowArea = "TimeRulerNowArea";
		public const string AllDayArea = "AllDayArea";
		public const string AllDayAreaSeparator = "AllDayAreaSeparator";
		public const string SelectedAllDayArea = "SelectedAllDayArea";
	}
	#endregion
	#region WeekViewAppearanceNames
	public static class WeekViewAppearanceNames {
		public const string CellHeaderCaption = "CellHeaderCaption";
		public const string CellHeaderCaptionLine = "CellHeaderCaptionLine";
		public const string TodayCellHeaderCaption = "TodayCellHeaderCaption";
		public const string TodayCellHeaderCaptionLine = "TodayCellHeaderCaptionLine";
	}
	#endregion
	#region GanttViewAppearanceNames
	public static class GanttViewAppearanceNames {
		public const string Dependency = "Dependency";
		public const string SelectedDependency = "SelectedDependency";
	}
	#endregion
}
