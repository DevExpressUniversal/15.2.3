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
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Media.Animation;
namespace DevExpress.Xpf.Scheduler.ThemeKeys {
	#region SchedulerControlThemeKeys
	public enum SchedulerControlThemeKeys {
		Template,
		Padding,
		ResourceColorSchemaTemplate,
		BorderTemplate,
		InvisibleBorderTemplate,
		InplaceEditTemplate,
		ResourceNavigatorHorizontalStyle,
		ResourceNavigatorVerticalStyle,
		ScrollingCornerStyle,
		DefaultBorderBrush,
		SchedulerRoot,
		DefaultAppointmentHeight,
		DialogWindowStyle
	}
	#endregion
	#region SchedulerControlThemeKeyExtension
	public class SchedulerControlThemeKeyExtension : ThemeKeyExtensionBase<SchedulerControlThemeKeys> {
		public SchedulerControlThemeKeyExtension() {
		}
	}
	#endregion
	#region ResourceNavigatorThemeKeys
	public enum ResourceNavigatorThemeKeys {
		HorizontalButtonStyle,
		VerticalButtonStyle,
		HorizontalResourceNavigatorStyle,
		VerticalResourceNavigatorStyle,
		HorizontalResourceNavigatorTemplate,
		VerticalResourceNavigatorTemplate,
	}
	#endregion
	#region ResourceNavigatorThemeKeyExtension
	public class ResourceNavigatorThemeKeyExtension : ThemeKeyExtensionBase<ResourceNavigatorThemeKeys> {
		public ResourceNavigatorThemeKeyExtension() {
		}
	}
	#endregion
	#region RangeControlThemeKeys
	public enum RangeControlThemeKeys {
		RulerHeaderControlTemplate,
		RulerHeaderContentTemplate,
		RulerCellControlTemplate,
		ThumbnailControlTemplate,
		ThumbnailGroupControlTemplate
	}
	#endregion
	#region RangeControlThemeKeyExtension
	public class RangeControlThemeKeyExtension : ThemeKeyExtensionBase<RangeControlThemeKeys> {
	}
	#endregion
	#region DayViewThemeKeys
	public enum DayViewThemeKeys {
		TopHeadersStyle,
		BottomHeadersStyle,
		GroupByNoneHeadersStyle,
		ScrollViewerTemplate,
		CellStyle,
		AllDayAreaCellStyle,
		DateHeaderTemplate,
		DateHeaderStyle,
		AllDayAreaBestFitTemplate,
		DayViewColumnTemplate,
		ResourceDayViewHeaderTemplate,
		DayViewGroupByNone,
		DayViewContentTemplateSelector,
		AllDayAreaTemplate,
		GroupByResourceDayViewHeadersTemplate,
		AllDayTemplate,
		AllDayAreaWithScrollTemplate,
		DayViewHeadersTemplate,
		ResourceDayViewHeadersTemplate,
		GroupByDateResourceHeadersTemplate,
		GroupByDateDayViewHeadersTemplate,
		GroupByDateDayHeaderTemplate,
		AllDayTemplateConverter,		
		DayViewGroupByDate,
		TimeRulerHeaderTemplate,
		TimeRulerTemplate,
		CurrentTimeIndicatorStyle,
		GroupByDateDayViewResourceDaysTemplate,
		DayViewResourceDaysTemplate,
		DayViewGroupByResource,
		DayViewHorizontalCellContentTemplate,
		DayViewScrollViewerTemplate,
		TimeRulerMinuteItemTemplate,
		TimeRulerFontStyle,
		TimeRulerHeaderSeparatorTemplate,
		TimeRulerHourItemTemplate,
		DayViewVerticalCellContentTemplate,
		ContentStyleSelector,
		SchedulerScrollViewer,
		DayViewGroupByDateNavigationButtonsTemplate,
		GroupByResourceResourceHeaderTemplate,
		GroupByResourceDayHeaderTemplate,
		GroupByDateDayHeadersVisibleHeaderTemplate,
		GroupByDateDayHeadersInvisibleHeaderTemplate,
		GroupByDateHeaderTemplateConverter,
		VerticalContentContainerStyle,
		AllDayAreaContentContainerStyle,
		TimeRulerMargin,
		TimeRulersMinWidth,
		TimeRulerHeadersContainerStyle,
		TimeRulersContainerStyle,		
		TopHeadersElementPosition,
		NonTopHeadersElementPosition,
		MoreButtonContainerStyle,
		AllDayAreaElementPosition,
		TopRightHeaderCornerPosition,
		NavigationButtonPairGroupByDateStyle,
		NavigationButtonPairGroupByResourceStyle,
		NavigationButtonPairGroupByNoneStyle,
		VerticalScrollBarMargin,
		HorizontalScrollBarMargin,
		ResourceNavigatorMargin,
		AllDayAreaBottomBorderBackground,
		HorizontalTimeMarker
	}
	#endregion
	#region DayViewThemeKeyExtension
	public class DayViewThemeKeyExtension : ThemeKeyExtensionBase<DayViewThemeKeys> {
	}
	#endregion
	#region SchedulerViewThemeKeys
	public enum SchedulerViewThemeKeys {
		AppointmentTemplate,
		VerticalAppointmentTemplate,
		VerticalAppointmentContentTemplate,
		HorizontalAppointmentTemplate,
		HorizontalAppointmentSameDayContentTemplate,
		HorizontalAppointmentLongerThanADayContentTemplate,
		HorizontalAppointmentSameDayStyle, 
		HorizontalAppointmentLongerThanADayStyle,
		VerticalAppointmentStyle,
		HeaderNormalTemplate,
		HeaderAlternateTemplate,
		VerticalHeaderNormalTemplate,
		NavigationButtonPrevStyle,
		NavigationButtonNextStyle,
		HorizontalResourceHeaderStyle,
		VerticalResourceHeaderStyle,
		SelectionTemplate,
		AppointmentToolTipContentTemplate,
		MoreButtonUpStyle,
		MoreButtonDownStyle,
		NavigationButtonsPair,
		TemplateCore,
		ColumnHeaderFadeInStoryboard,
		ColumnHeaderFadeOutStoryboard,
		VerticalLineBrush,
		HeaderNormalGradient,
		HeaderNormalGlare,
		HeaderHotGradient,
		HeaderHotGlare,
		HeaderPressedGradient,
		HeaderPressedGlare,
		MoreButtonAppointmentTemplate,
		MoreButtonCellTemplate,
		ViewTemplate,
		HeaderControlColors,
		VerticalHeaderControlTemplate,
		VerticalHeaderControlContentTemplate,
		DragDropHoverTimeCellsStyle,
		HeaderCornerVisibility,
		HorizontalHeadersContainerStyle,
		DayViewHorizontalHeadersContainerStyle,
		VerticalHeadersContainerStyle,
		HeaderForegroundColor,
		AppointmentForegroundColor
	}
	#endregion
	#region SchedulerViewThemeKeyExtension
	public class SchedulerViewThemeKeyExtension : ThemeKeyExtensionBase<SchedulerViewThemeKeys> {
	}
	#endregion
	#region WorkWeekViewThemeKeys
	public enum WorkWeekViewThemeKeys {
		WorkWeekViewContentTemplateSelector,
		ContentStyleSelector
	}
	#endregion
	#region WorkWeekViewThemeKeyExtension
	public class WorkWeekViewThemeKeyExtension : ThemeKeyExtensionBase<WorkWeekViewThemeKeys> {
	}
	#endregion
	#region FullWeekViewThemeKeys
	public enum FullWeekViewThemeKeys {
		FullWeekViewContentTemplateSelector,
		ContentStyleSelector
	}
	#endregion
	#region FullWeekViewThemeKeyExtension
	public class FullWeekViewThemeKeyExtension : ThemeKeyExtensionBase<FullWeekViewThemeKeys> {
	}
	#endregion
	#region WeekViewThemeKeys
	public enum WeekViewThemeKeys {
		MoreButtonStyle,
		HorizontalWeekCellStyle,
		VerticalWeekCellStyle,
		DayOfWeekHeaderStyle,
		HorizontalWeekDateHeaderStyle,
		VerticalWeekDateHeaderStyle,
		WeekViewDayContentTemplate,
		CellHeaderNormalTemplate,
		CellHeaderAlternateTemplate,
		VerticalCellHeaderNormalTemplate,
		VerticalCellHeaderAlternateTemplate,
		WeekCellNormalTemplate,
		WeekCellAlternateTemplate,
		VerticalWeekCellNormalTemplate,
		VerticalWeekCellAlternateTemplate,
		WeekViewGroupByDate,
		WeekViewMonthVerticalTemplate,
		WeekViewGroupByNone,
		WeekViewMonthTemplate,
		WeekViewVerticalWeekTemplate,
		WeekViewHorizontalWeekTemplate,
		WeekViewGroupByResource,
		ResourceHeaderTemplate,
		NavigationButtonsPairGroupByResource,
		WeekViewGroupByNoneContentContainerStyle,
		WeekViewGroupByResourceContentContainerStyle,
		WeekViewGroupByDateContentContainerStyle,
		ContentStyleSelector,
		DayOfWeekHeadersGroupByDateElementPosition,
		ResourceHeadersGroupByDateElementPosition,
		GroupByNoneElementPosition,
		GroupByResourceElementPosition,
		ResourceHeadersGroupByResourceElementPosition,
		TopRightHeaderCornerGroupByResourceElementPosition,
		TopLeftCornerGroupByDateElementPosition,
		TopRightCornerGroupByDateElementPosition,
		BottomLeftCornerGroupByDateElementPosition,
		CellBorderBrush,
		NavigationButtonPairGroupByDateStyle,
		NavigationButtonPairGroupByResourceStyle,
		NavigationButtonPairGroupByNoneStyle,
		GroupByNoneWeekScrollBarMargin,
		GroupByDateBottomLeftCornerMargin,
		GroupByDateWeekScrollBarMargin,
		GroupByDateWeekResourceNavigatorMargin,
		ScrollBarStyle,
		DayOfWeekHeadersGroupByDateRightCornerSeparatorPosition
	}
	#endregion
	#region WeekViewThemeKeyExtension
	public class WeekViewThemeKeyExtension : ThemeKeyExtensionBase<WeekViewThemeKeys> {
	}
	#endregion
	#region MonthViewThemeKeys
	public enum MonthViewThemeKeys {
		CellStyle,
		HorizontalWeekCellStyle,
		DayOfWeekHeaderStyle,
		DateHeaderStyle,
		CellContentTemplate,
		MonthVerticalTemplate,
		MonthHorizontalTemplate,
		GroupByResourceMonthHorizontalTemplate,
		GroupByResourceHeaderTemplate,
		MonthViewGroupByDate,
		MonthViewGroupByNone,
		MonthViewGroupByResource,
		MonthViewMonthHorizontalResourceHeaderTemplate,
		MonthViewWeekTemplate,
		ContentStyleSelector,
		MonthViewContainerStyle,
		DayOfWeekHeadersGroupByNoneElementPosition,
		ResourceHeadersGroupByResourceElementPosition,
		GroupByResourceDayOfWeekHeadersTemplate,
		DayOfWeekHeadersGroupByResourceElementPosition,
		TopRightHeaderCornerGroupByNoneElementPosition,
		TopRightHeaderCornerGroupByResourceElementPosition,
		MiddleRightHeaderCornerGroupByResourceElementPosition,
		HorizontalWeekAppointmentPanelMargin,
		NavigationButtonPairGroupByDateStyle,
		NavigationButtonPairGroupByResourceStyle,
		NavigationButtonPairGroupByNoneStyle,
		WeekScrollBarMargin,
		ResourceNavigatorMargin
	}
	#endregion
	#region MonthViewThemeKeyExtension
	public class MonthViewThemeKeyExtension : ThemeKeyExtensionBase<MonthViewThemeKeys> {
	}
	#endregion
	#region TimelineViewThemeKeys
	public enum TimelineViewThemeKeys {
		CellStyle,
		DateHeaderStyle,
		TimelineViewCellContentTemplate,		
		TimelineViewGroupByDate,
		TimelineHeaderGroupByDate,
		TimelineViewTimelineTemplate,		
		TimelineHeaderWithScroll,		
		TimelineColumnHeaderTemplateSelector,
		TimelineColumnHeaderTemplateSelectorGroupByDate,
		TimelineHeader,
		TimelineScaleTopLevelHeaderGroupByDate,
		TimelineScaleTopLevelHeader,
		TimelineScaleOtherLevelHeader,
		TimelineScaleOtherLevelHeaderGroupByDate,
		TimelineViewContainerTemplate,
		TimelineViewGroupByNone,		
		TimelineViewContentTemplateSelector,
		TimelineViewGroupByNoneContainerStyle,
		TimelineViewGroupByResourceContainerStyle,
		TimelineTemplateConverter,
		ContentStyleSelector,
		SelectionBarTemplate,
		TimelineViewSelectionBarCellStyle,
		ResourceHeadersElementPosition,
		SelectionBarContanerGroupByNonePosition,
		TopLeftCornerGroupByDateElementPosition,
		TopRightCornerGroupByDateElementPosition,
		ContentElementPosition,
		BottomLeftCornerGroupByDateElementPosition,
		NavigationButtonsPair,
		NavigationButtonsPairContainerStyle,
		NavigationButtonPairGroupByDateStyle,
		NavigationButtonPairGroupByResourceStyle,
		NavigationButtonPairGroupByNoneStyle,
		GroupByNoneTimelineScrollBarMargin,
		GroupByDateTimelineScrollBarMargin,
		GroupByDateResourceNavigatorMargin,
		ScaleTopLevelHeaderElementPosition,
		ScaleTopLevelHeaderGroupByDateDefaultElementPosition,
		ScaleTopLevelHeaderGroupByDateRightCornerSeparatorPosition,
		ScaleOtherLevelHeaderElementPosition,
		ScaleOtherLevelHeaderGroupByDateDefaultElementPosition,
		ScaleOtherLevelHeaderGroupByDateRightCornerSeparatorPosition,
		ScaleTopLevelHeaderConverter,
		ScaleOtherLevelHeaderConverter,
		ResourceNavigatorVisibilityMarginResolver,
		GroupByDateContainerStyle,
		AdornedBorderStyle,
		VerticalTimeMarker
	}
	#endregion
	#region TimelineViewThemeKeyExtension
	public class TimelineViewThemeKeyExtension : ThemeKeyExtensionBase<TimelineViewThemeKeys> {
	}
	#endregion
	#region GanttViewThemeKeys
	public enum GanttViewThemeKeys {
		GanttViewGroupByDate,
		ContentStyleSelector,
		GanttViewGroupByDateContainerStyle,
		AdornedBorderStyle
	}
	#endregion
	#region GanttViewThemeKeyExtension
	public class GanttViewThemeKeyExtension : ThemeKeyExtensionBase<GanttViewThemeKeys> {
	}
	#endregion
	#region SchedulerUIControlThemeKeys
	public enum SchedulerUIControlThemeKeys {
		UserInterfaceObjectItemTemplate,
		UserInterfaceObjectNonEditableTemplate,
		SchedulerResourceItemTemplate,
		SchedulerResourceNonEditableTemplate,
		ColorMarkerStyle,
		TextBlockStyle,
		ItemTextBlockStyle
	}
	#endregion
	#region SchedulerUIControlThemeKeyExtension
	public class SchedulerUIControlThemeKeyExtension : ThemeKeyExtensionBase<SchedulerUIControlThemeKeys> {
	}
	#endregion
	#region SchedulerColorConvertThemeKeys
	public enum SchedulerColorConvertThemeKeys {
		NormalGradientStop1,
		NormalGradientStop2,
		NormalGradientStop3,
		NormalGlareStop1,
		NormalGlareStop2,
		NormalGlareStop3,
		NormalGlareStop4,
		HotGradientStop1,
		HotGradientStop2,
		HotGradientStop3,
		HotGlareStop1,
		HotGlareStop2,
		HotGlareStop3,
		HotGlareStop4,
		PressedGradientStop1,
		PressedGradientStop2,
		PressedGlareStop1,
		PressedGlareStop2,
		PressedGlareStop3,
		HeadersVerticalLineBrush
	}
	#endregion
	#region SchedulerColorConvertThemeKeyExtension
	public class SchedulerColorConvertThemeKeyExtension : ThemeKeyExtensionBase<SchedulerColorConvertThemeKeys> {
	}
	#endregion
	#region SchedulerControlGenericThemeKeys
	public enum SchedulerControlGenericThemeKeys {
		DayViewTimeCellAttachedPropertiesCalculator,		
		ElementPositionAttachedPropertySetterTemplate
	}
	#endregion
	#region SchedulerControlGenericKeyExtension
	public class SchedulerControlGenericThemeKeyExtension : ThemeKeyExtensionBase<SchedulerControlGenericThemeKeys> {
		public SchedulerControlGenericThemeKeyExtension() {
		}
	}
	#endregion
	#region DateNavigatorThemeKeys
	public enum DateNavigatorThemeKeys {
		Template
	}
	#endregion
	#region DateNavigatorThemeKeyExtension
	public class DateNavigatorThemeKeyExtension : ThemeKeyExtensionBase<DateNavigatorThemeKeys> {
		public DateNavigatorThemeKeyExtension() {
		}
	}
	#endregion
	public enum StyleSelectorThemeKeys {
		HorizontalAppointmentStyle,
		VerticalAppointmentStyle
	}
	public class StyleSelectorThemeKeyExtension : ThemeKeyExtensionBase<StyleSelectorThemeKeys> { 
	}
	public enum AppointmentColorConvertThemeKeys {
		BackBorder,
		MiddleBorderGradient,
		AppointmentBackgroundGradientStart,
		AppointmentBackgroundGradientEnd,
		AppointmentSelection
	}
	public class AppointmentColorConvertThemeKeyExtension : ThemeKeyExtensionBase<AppointmentColorConvertThemeKeys> {
		public AppointmentColorConvertThemeKeyExtension() {
		}
	}
	public enum Office2007ThemeKeys {
		DefaultBorderColor,
		ResourceNavigatorGlyphBrush,
		MoreButtonDefaultStateBackground,
		NavigationButtonDefaultStateBackground,
		MoreButtonMouseOverStateBackground,
		NavigationButtonMouseOverStateBackground,
		NavigationButtonPressedStateBackground,
		MoreButtonPressedStateBackground,
		DefaultBackgroundBrush,
		HeaderBaseColors,
		TimeRulerTextForeground,
		HorizontalResourceNavigatorBackground,
		VerticalResourceNavigatorBackground
	}
	public class Office2007ThemeKeyExtension : ThemeKeyExtensionBase<Office2007ThemeKeys> {
		public Office2007ThemeKeyExtension() {
		}
	}
	public class SchedulerFormThemeKeyExtension : ThemeKeyExtensionBase<SchedulerFormThemeKeys> { 
	}
	public enum SchedulerFormThemeKeys { 
		DefaultAppointmentFormWidth,
		DefaultAppointmentFormHeight,
		DefaultRecurrentAppointmentFormWidth, 
		DefaultRecurrentAppointmentFormHeight,
		DefaultAppointmentRecurrenceFormStyle,
		DefaultAppointmentFormStyle
	}
}
