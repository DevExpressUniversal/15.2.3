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
using System.Collections.Generic;
namespace DevExpress.XtraScheduler.Native {
	#region ChangeActions
	[Flags]
	public enum ChangeActions {
		None = 0x00000000,
		RecalcClientBounds = 0x00000001,
		RepositionScrollBars = 0x00000002,
		RecalcViewBounds = 0x00000004,
		RecalcViewLayout = 0x00000008,
		MayChangeDateTimeScrollBarVisibility = 0x00000010,
		MayChangeResourceScrollBarVisibility = 0x00000020,
		MayChangeDateTimeScrollBarOrientation = 0x00000040,
		MayChangeResourceScrollBarOrientation = 0x00000080,
		UpdateDateTimeScrollBarValue = 0x00000100,
		UpdateResourceScrollBarValue = 0x00000200,
		UpdatePaintStyle = 0x00000400,
		AssignLimitInterval = 0x00000800,
		UpdateVisibleIntervals = 0x00001000,
		SynchronizeSelectionInterval = 0x00002000,
		ValidateSelectionResource = 0x00004000,
		RaiseSelectionChanged = 0x00008000,
		RaiseVisibleIntervalChanged = 0x00010000,
		SynchronizeSelectionResource = 0x00020000,
		NotifyActiveViewChanged = 0x00040000,
		CellContentScroll = 0x00080000,
		AdjustDateTimeScrollBarPosition = 0x00100000,
		RecalcDraggingAppointmentPosition = 0x00200000,
		RecalcAppointmentsLayout = 0x00400000,
		SynchronizeOrResetSelectionInterval = 0x00800000,
		ClearPreliminaryAppointmentsAndCellContainers = 0x01000000,
		RecalcPreliminaryLayout = 0x02000000
	}
	#endregion
	#region SchedulerControlChangeType
	public enum SchedulerControlChangeType {
		None,
		ActiveViewChanged,
		BorderStyleChanged,
		BoundsChanged,
		GroupTypeChanged,
		LookAndFeelChanged,
		StorageChanged,
		DateTimeScroll,
		FirstVisibleResourceIndexChanged,
		ResourcesPerPageChanged,
		ResourceScroll,
		LimitIntervalChanged,
		WeekCountChanged,
		CompressWeekendChanged,
		ShowWeekendChanged,
		TimeScaleChanged,
		TimeRulersChanged,
		VisibleTimeChanged,
		VisibleTimeSnapModeChanged,
		ScrollStartTimeChanged,
		ScrollStartTimeChangedSystemScroll,
		ShowWorkTimeOnlyChanged,
		WorkTimeChanged,
		ResourceColorSchemasChanged,
		ShowAllDayAreaChanged,
		ShowAllAppointmentsAtTimeCellsChanged,
		DayCountChanged,
		ShowFullWeekChanged,
		TimelineScalesChanged,
		SelectionBarOptionsChanged,
		AppointmentDisplayOptionsChanged,
		ShowMoreButtonsChanged,
		CellsAutoHeightChanged,
		ShowResourceHeadersChanged,
		VisibleIntervalsChanged,
		AppearanceChanged,
		SelectionChanged,
		ShowDayViewDayHeadersChanged,
		ShowMoreButtonsOnEachColumnChanged,
		ExternalAppointmentImages,
		PerformViewLayoutChanged,
		NavigationButtonVisibilityChanged,
		NavigationButtonAppointmentSearchIntervalChanged,
		StatusLineWidthChanged,
		ControlStartChanged,
		DateTimeScrollbarVisibilityChanged,
		RowHeightChanged,
		WorkDaysChanged,
		StorageAppointmentsChanged,
		DayViewActiveStorageAppointmentsChanged,
		StorageAppointmentMappingsChanged,
		StorageResourcesChanged,
		StorageResourceMappingsChanged,
		StorageDeferredNotifications,
		StorageUIObjectsChanged,
		UserPreferenceChanged,
		UserPreferenceChangedTimeline,
		SystemTimeChanged,
		EndInit,
		ResourceNavigatorVisibilityChanged,
		OptionsViewChanged,
		ViewEnabledChanged,
		OptionsCustomizationChanged,
		OptionsBehaviorChanged,
		TimelineIntervalCountChanged,
		FormatStringsChanged,
		CellContentScroll,
		ScrollBarVisibilityChanged,
		AppointmentDragging,
		ResourceExpandedChanged,
		StorageAppointmentDependenciesChanged,
		TimeIndicatorDisplayOptionsChanged,
		TimeMarkerVisibilityChanged
	}
	#endregion
	#region ChangeActionsCalculator
	public static class ChangeActionsCalculator {
		internal static Dictionary<SchedulerControlChangeType, ChangeActions> changeActionsTable = CreateChangeActionsTable();
		internal static Dictionary<SchedulerControlChangeType, ChangeActions> CreateChangeActionsTable() {
			Dictionary<SchedulerControlChangeType, ChangeActions> ht = new Dictionary<SchedulerControlChangeType, ChangeActions>();
			ht.Add(SchedulerControlChangeType.None, ChangeActions.None);
			ht.Add(SchedulerControlChangeType.ActiveViewChanged, ChangeActions.SynchronizeSelectionResource | ChangeActions.ValidateSelectionResource | ChangeActions.SynchronizeSelectionInterval | ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.AssignLimitInterval | ChangeActions.UpdateVisibleIntervals | ChangeActions.NotifyActiveViewChanged | ChangeActions.AdjustDateTimeScrollBarPosition | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ControlStartChanged, ChangeActions.SynchronizeSelectionResource | ChangeActions.ValidateSelectionResource | ChangeActions.SynchronizeSelectionInterval | ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.AssignLimitInterval | ChangeActions.UpdateVisibleIntervals | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.BorderStyleChanged, ChangeActions.RecalcClientBounds | ChangeActions.RepositionScrollBars | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue);
			ht.Add(SchedulerControlChangeType.BoundsChanged, ChangeActions.RecalcClientBounds | ChangeActions.RepositionScrollBars | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue);
			ht.Add(SchedulerControlChangeType.GroupTypeChanged, ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.ValidateSelectionResource | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.LookAndFeelChanged, ChangeActions.UpdatePaintStyle | ChangeActions.RecalcClientBounds | ChangeActions.RepositionScrollBars | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers);
			ht.Add(SchedulerControlChangeType.ResourceColorSchemasChanged, ChangeActions.RecalcViewLayout | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers);
			ht.Add(SchedulerControlChangeType.StorageChanged, ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.DateTimeScroll, ChangeActions.RecalcViewLayout | ChangeActions.RaiseSelectionChanged | ChangeActions.RaiseVisibleIntervalChanged | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.FirstVisibleResourceIndexChanged, ChangeActions.ValidateSelectionResource | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.ValidateSelectionResource | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ResourcesPerPageChanged, ChangeActions.ValidateSelectionResource | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.ValidateSelectionResource | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ResourceScroll, ChangeActions.ValidateSelectionResource | ChangeActions.RecalcViewLayout | ChangeActions.ValidateSelectionResource);
			ht.Add(SchedulerControlChangeType.LimitIntervalChanged, ChangeActions.AssignLimitInterval | ChangeActions.UpdateVisibleIntervals | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.WeekCountChanged, ChangeActions.UpdateVisibleIntervals | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcViewLayout | ChangeActions.SynchronizeSelectionInterval | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers);
			ht.Add(SchedulerControlChangeType.CompressWeekendChanged, ChangeActions.UpdateVisibleIntervals | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcViewLayout | ChangeActions.SynchronizeSelectionInterval | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ShowWeekendChanged, ChangeActions.UpdateVisibleIntervals | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcViewLayout | ChangeActions.SynchronizeSelectionInterval | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers);
			ht.Add(SchedulerControlChangeType.TimeScaleChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.SynchronizeSelectionInterval | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.VisibleTimeChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.SynchronizeSelectionInterval | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers);
			ht.Add(SchedulerControlChangeType.VisibleTimeSnapModeChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.SynchronizeSelectionInterval | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ScrollStartTimeChanged, ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.ScrollStartTimeChangedSystemScroll, ChangeActions.UpdateDateTimeScrollBarValue);
			ht.Add(SchedulerControlChangeType.ShowWorkTimeOnlyChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.SynchronizeSelectionInterval | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.WorkTimeChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.SynchronizeSelectionInterval | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ShowAllDayAreaChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.AdjustDateTimeScrollBarPosition | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers);
			ht.Add(SchedulerControlChangeType.ShowAllAppointmentsAtTimeCellsChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers);
			ht.Add(SchedulerControlChangeType.DayCountChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.SynchronizeSelectionInterval | ChangeActions.UpdateVisibleIntervals | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ShowFullWeekChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.SynchronizeSelectionInterval | ChangeActions.UpdateVisibleIntervals | ChangeActions.AssignLimitInterval | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.TimelineScalesChanged, ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.SynchronizeOrResetSelectionInterval | ChangeActions.RaiseVisibleIntervalChanged | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.SelectionBarOptionsChanged, ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.AppointmentDisplayOptionsChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ShowMoreButtonsChanged, ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.CellsAutoHeightChanged, ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.ShowResourceHeadersChanged, ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.TimeRulersChanged, ChangeActions.RecalcViewLayout | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.VisibleIntervalsChanged, ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.SynchronizeSelectionInterval | ChangeActions.RaiseVisibleIntervalChanged | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.AppearanceChanged, ChangeActions.RecalcViewLayout | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.UpdateDateTimeScrollBarValue);
			ht.Add(SchedulerControlChangeType.SelectionChanged, ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.ValidateSelectionResource | ChangeActions.SynchronizeSelectionInterval | ChangeActions.SynchronizeSelectionResource | ChangeActions.RaiseSelectionChanged);
			ht.Add(SchedulerControlChangeType.ShowDayViewDayHeadersChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ShowMoreButtonsOnEachColumnChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ExternalAppointmentImages, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue);
			ht.Add(SchedulerControlChangeType.PerformViewLayoutChanged, ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.NavigationButtonVisibilityChanged, ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.NavigationButtonAppointmentSearchIntervalChanged, ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.StatusLineWidthChanged, ChangeActions.RecalcViewLayout | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.DateTimeScrollbarVisibilityChanged, ChangeActions.RepositionScrollBars | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.RowHeightChanged, ChangeActions.RepositionScrollBars | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.WorkDaysChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.SynchronizeSelectionInterval | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.UpdateVisibleIntervals | ChangeActions.AssignLimitInterval | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.StorageAppointmentsChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers);
			ht.Add(SchedulerControlChangeType.DayViewActiveStorageAppointmentsChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.StorageAppointmentDependenciesChanged, ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.StorageResourcesChanged, ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.ValidateSelectionResource | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers);
			ht.Add(SchedulerControlChangeType.StorageDeferredNotifications, ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.ValidateSelectionResource | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.UpdateDateTimeScrollBarValue);
			ht.Add(SchedulerControlChangeType.UserPreferenceChanged, ChangeActions.UpdatePaintStyle | ChangeActions.SynchronizeSelectionInterval | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.UserPreferenceChangedTimeline, ChangeActions.UpdatePaintStyle | ChangeActions.SynchronizeSelectionInterval | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.UpdateVisibleIntervals | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.SystemTimeChanged, ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.EndInit, ChangeActions.SynchronizeSelectionResource | ChangeActions.ValidateSelectionResource | ChangeActions.SynchronizeSelectionInterval | ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.AssignLimitInterval | ChangeActions.UpdateVisibleIntervals | ChangeActions.RecalcClientBounds | ChangeActions.RecalcViewBounds | ChangeActions.UpdatePaintStyle | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ResourceNavigatorVisibilityChanged, ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue);
			ht.Add(SchedulerControlChangeType.OptionsViewChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.SynchronizeSelectionInterval | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.UpdateVisibleIntervals | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.ViewEnabledChanged, ChangeActions.None);
			ht.Add(SchedulerControlChangeType.OptionsBehaviorChanged, ChangeActions.SynchronizeSelectionInterval | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.UpdateVisibleIntervals | ChangeActions.RecalcPreliminaryLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers);
			ht.Add(SchedulerControlChangeType.OptionsCustomizationChanged, ChangeActions.RecalcAppointmentsLayout | ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.TimelineIntervalCountChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.SynchronizeSelectionInterval | ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.UpdateVisibleIntervals);
			ht.Add(SchedulerControlChangeType.StorageAppointmentMappingsChanged, ChangeActions.None); 
			ht.Add(SchedulerControlChangeType.StorageResourceMappingsChanged, ChangeActions.None); 
			ht.Add(SchedulerControlChangeType.StorageUIObjectsChanged, ChangeActions.None); 
			ht.Add(SchedulerControlChangeType.FormatStringsChanged, ChangeActions.MayChangeDateTimeScrollBarVisibility | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.SynchronizeSelectionInterval | ChangeActions.UpdateDateTimeScrollBarValue);
			ht.Add(SchedulerControlChangeType.CellContentScroll, ChangeActions.CellContentScroll);
			ht.Add(SchedulerControlChangeType.ScrollBarVisibilityChanged, ChangeActions.RecalcViewLayout | ChangeActions.AdjustDateTimeScrollBarPosition | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.AppointmentDragging, ChangeActions.RecalcDraggingAppointmentPosition);
			ht.Add(SchedulerControlChangeType.ResourceExpandedChanged, ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.ValidateSelectionResource | ChangeActions.RecalcViewBounds | ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
			ht.Add(SchedulerControlChangeType.TimeIndicatorDisplayOptionsChanged, ChangeActions.RecalcViewLayout);
			ht.Add(SchedulerControlChangeType.TimeMarkerVisibilityChanged, ChangeActions.RecalcViewLayout);
			return ht;
		}
		public static ChangeActions CalculateChangeActions(SchedulerControlChangeType change) {
			ChangeActions result;
			if (changeActionsTable.TryGetValue(change, out result))
				return result;
			else
				return ChangeActions.None;
		}
	}
	#endregion
}
