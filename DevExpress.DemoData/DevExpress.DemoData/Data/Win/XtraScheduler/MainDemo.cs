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
namespace DevExpress.DemoData.Model {
	public static partial class Repository {
		static List<Module> Create_XtraScheduler_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraScheduler %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraScheduler.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "DayViewModule",
					displayName: @"Day View",
					group: "View Types",
					type: "DevExpress.XtraScheduler.Demos.DayViewModule",
					description: @"This example demonstrates the scheduler control's Day View, which provides the most detailed view of the appointments for a particular day. Change the demo's options to display working hours only, to enable or disable shadows for appointments, to hide or show the ""all-day"" area and day headers, and to specify the number of days shown in the view at one time. Examine different Snap-To-Cells modes. See how appointment status lines can be used to visualize time intervals. Use CTRL + mouse wheel for zooming.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\DayView.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\DayView.vb"
					}
				),
				new SimpleModule(demo,
					name: "WorkWeekViewModule",
					displayName: @"Work Week View",
					group: "View Types",
					type: "DevExpress.XtraScheduler.Demos.WorkWeekViewModule",
					description: @"This example demonstrates the scheduler control's Work Week View, which displays appointments for the working days in a particular week. Use the checkboxes to specify the working days to display. Use CTRL + mouse wheel for zooming.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\WorkDaysView.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\WorkDaysView.vb"
					}
				),
				new SimpleModule(demo,
					name: "FullWeekViewModule",
					displayName: @"Full Week View",
					group: "View Types",
					type: "DevExpress.XtraScheduler.Demos.FullWeekViewModule",
					description: @"This example demonstrates use of the Scheduler control's Full Week View. It displays appointments for all days within a specific week.",
					addedIn: KnownDXVersion.V142,
					updatedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\FullWeekView.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\FullWeekView.vb"
					}
				),
				new SimpleModule(demo,
					name: "PerformanceTestModule",
					displayName: @"Performance Test",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.PerformanceTestModule",
					description: @"Check for yourself a significant performance improvement achieved by adjusting the way the layout is calculated. Specify the number of visible appointments and observe how the Scheduler performs by scrolling the view and navigating to another date. To minimize the occasional 'freezing' that can affect the scheduler, calculations are now executed in multiple threads. This advanced asynchronous mode can be switched off if desired, however, even in synchronous mode the layout calculations are faster than before.",
					addedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\PerformanceTest.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\PerformanceTest.vb"
					}
				),
				new SimpleModule(demo,
					name: "WeekViewModule",
					displayName: @"Week View",
					group: "View Types",
					type: "DevExpress.XtraScheduler.Demos.WeekViewModule",
					description: @"This example demonstrates the scheduler control's Week View, which displays appointments for a weekly period. Change the demo's options to specify whether the appointment start and end times are shown as clocks or as text, and whether the end time is shown at all. See how appointment status lines can be used to visualize time intervals.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\WeekView.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\WeekView.vb"
					}
				),
				new SimpleModule(demo,
					name: "MonthViewModule",
					displayName: @"Month View",
					group: "View Types",
					type: "DevExpress.XtraScheduler.Demos.MonthViewModule",
					description: @"This example demonstrates the scheduler control's Month View, which is designed to provide end-users with an overview of appointments. Change the demo's options to specify whether weekends (two days - Saturday and Sunday) are compressed (displayed in one box), select display modes for appointment start and end times, and set the number of weeks shown in the view at once. Observe how appointment status lines can be used to visualize time intervals. Use CTRL + mouse wheel for zooming.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\MonthView.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\MonthView.vb"
					}
				),
				new SimpleModule(demo,
					name: "TimelineViewModule",
					displayName: @"Timeline View",
					group: "View Types",
					type: "DevExpress.XtraScheduler.Demos.TimelineViewModule",
					description: @"This example demonstrates the scheduler control's Timeline View, which displays appointments as horizontal bars along the timescales, and provides end-users with a clearer overview for scheduling purposes. Right-click the scheduler control and use the popup menu to enable and show certain time scales. To change captions or add a new time scale with a fixed interval, click the 'Edit TimeScales...' button. The slider enables you to change the interval width, providing zoom functionality. You can also use CTRL + mouse wheel.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\TimeLineView.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\TimeLineView.vb"
					}
				),
				new SimpleModule(demo,
					name: "GanttViewModule",
					displayName: @"Gantt View",
					group: "View Types",
					type: "DevExpress.XtraScheduler.Demos.GanttViewModule",
					description: @"This demo introduces the Gantt View to display appointment data. Its key features include hierarchical resources, auto height of resources, four types of dependencies between appointments, and multiple parent and child appointment relations.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\GanttView.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\GanttView.vb"
					}
				),
				new SimpleModule(demo,
					name: "ResourcesTreeModule",
					displayName: @"Resources Tree",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.ResourcesTreeModule",
					description: @"This example demonstrates how to display hierarchically ordered resources in a tree view using our ResourcesTree control. The control synchronize its visible resources with the resources displayed within the bound Scheduler control.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\ResourcesTree.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\ResourcesTree.vb"
					}
				),
				new SimpleModule(demo,
					name: "GroupByDateModule",
					displayName: @"By Date",
					group: "Grouping",
					type: "DevExpress.XtraScheduler.Demos.GroupByDateModule",
					description: @"This example demonstrates how to group scheduled appointments by dates. Switch the Scheduler's view option, to see how this grouping looks in different views.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\GroupByDate.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\GroupByDate.vb"
					}
				),
				new SimpleModule(demo,
					name: "GroupByResourceModule",
					displayName: @"By Resource",
					group: "Grouping",
					type: "DevExpress.XtraScheduler.Demos.GroupByResourceModule",
					description: @"This example demonstrates how to group the scheduled appointments by resources. Switch the Scheduler's view option, to see how this grouping looks in different views.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\GroupByResource.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\GroupByResource.vb"
					}
				),
				new SimpleModule(demo,
					name: "BoundModeModule",
					displayName: @"MS Access",
					group: "Data Binding",
					type: "DevExpress.XtraScheduler.Demos.BoundModeModule",
					description: @"This example demonstrates how to bind the scheduler to MS Access database using TableAdapters.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\BoundMode.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\BoundMode.vb"
					}
				),
				new SimpleModule(demo,
					name: "EntityBoundModeModule",
					displayName: @"Entity Framework",
					group: "Data Binding",
					type: "DevExpress.XtraScheduler.Demos.EntityBoundModeModule",
					description: @"This example demonstrates how you can bind the scheduler to Entity Framework data. Entities are stored and retrieved from a SQLite backend database. The scheduler is bound to the DbSet<TEntity>.Local property of the entity collection with appointment and resource data contained in a DbContext object. The Entity Framework data source eliminates the need to handle SchedulerStorage events to update appointment data.",
					addedIn: KnownDXVersion.V142,
					featuredPriority: 0,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\EntityBoundMode.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\EntityBoundMode.vb"
					}
				),
				new SimpleModule(demo,
					name: "XPOBoundModeModule",
					displayName: @"XPO",
					group: "Data Binding",
					type: "DevExpress.XtraScheduler.Demos.XPOBoundModeModule",
					description: @"This example demonstrates how to bind the scheduler to the XPO data source using the XPCollection objects.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\XPOBoundMode.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\XPOBoundMode.vb"
					}
				),
				new SimpleModule(demo,
					name: "ListBoundModeModule",
					displayName: @"BindingList",
					group: "Data Binding",
					type: "DevExpress.XtraScheduler.Demos.ListBoundModeModule",
					description: @"This example demonstrates how to bind the scheduler at runtime to the data source which exposes the IBindingList interface.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\ListBoundMode.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\ListBoundMode.vb"
					}
				),
				new SimpleModule(demo,
					name: "UnboundModeModule",
					displayName: @"Unbound Mode",
					group: "Data Binding",
					type: "DevExpress.XtraScheduler.Demos.UnboundModeModule",
					description: @"This example demonstrates the unbound mode of the scheduler. The data for appointments and resources are initially loaded from external xml files.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\UnboundMode.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\UnboundMode.vb"
					}
				),
				new SimpleModule(demo,
					name: "OutlookImportModule",
					displayName: @"Import from Outlook",
					group: "Data Exchange",
					type: "DevExpress.XtraScheduler.Demos.OutlookImportModule",
					description: @"This example demonstrates how to import data from Microsoft Outlook to the Scheduler Storage. If you have MS Outlook installed on your machine, you can choose which appointments should be imported, and click the 'Import' button to start importing. Note that the data can be exported from the Scheduler Storage to MS Outlook, but this functionality is not implemented in this demo. The OnException event is handled so if an error occurs while importing, no appointments are added to the scheduler.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\OutlookImport.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\OutlookImport.vb"
					}
				),
				new SimpleModule(demo,
					name: "OutlookSynchronizationModule",
					displayName: @"Synchronize with Outlook",
					group: "Data Exchange",
					type: "DevExpress.XtraScheduler.Demos.OutlookSynchronizationModule",
					description: @"This example demonstrates how to synchronize current data in the Scheduler Storage with appointments stored in Microsoft Outlook. If you have MS Outlook installed on your machine, you can first manually add some appointments to the Scheduler, then specify synchronization options and click the 'Synchronize' button to start one-way synchronization. Appointments contained in MS Outlook folder are not modified. The OnException event is handled so if an error occurs while importing, no appointments are added to the scheduler.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\OutlookSynchronization.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\OutlookSynchronization.vb"
					}
				),
				new SimpleModule(demo,
					name: "iCalendarExportModule",
					displayName: @"iCalendar export",
					group: "Data Exchange",
					type: "DevExpress.XtraScheduler.Demos.iCalendarExportModule",
					description: @"This example demonstrates the XtraScheduler support for the iCalendar data exchange format. It facilitates data transfer between applications that use XtraScheduler and other applications, such as Apple iCal, Google Calendar, Microsoft Exchange Server, Microsoft Office Outlook 2007, Novell GroupWise, and Windows Calendar. In this demo, you can save appointments into an iCalendar file with an .ics extension. A combo box enables you to select whether recurring or non-recurring appointments should be excluded from export. This example creates an iCalendarExporter class instance, and calls its Export method. It handles the AppointmentExporting event, to cancel processing of specific appointments.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\iCalendarExport.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\iCalendarExport.vb"
					}
				),
				new SimpleModule(demo,
					name: "iCalendarImportModule",
					displayName: @"iCalendar import",
					group: "Data Exchange",
					type: "DevExpress.XtraScheduler.Demos.iCalendarImportModule",
					description: @"This example demonstrates the XtraScheduler support for the iCalendar data exchange format. It facilitates data transfer between different scheduling applications and platforms. In this demo, you can load appointments from the iCalendar file. A combo box enables you to select whether recurring or non-recurring appointments are ignored. A checkbox allows clearing the scheduler by deleting existing appointments before import. The OnException event is handled so if an error occurs while importing, no appointments are added to the scheduler.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\iCalendarImport.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\iCalendarImport.vb"
					}
				),
				new SimpleModule(demo,
					name: "AppearanceModule",
					displayName: @"Custom Appearance",
					group: "Customization",
					type: "DevExpress.XtraScheduler.Demos.AppearanceModule",
					description: @"This example demonstrates how to change the appearances of the scheduler control and its view. Choose whether you want to customize the scheduler or current view's appearance, or resource color schemas and click the ""Edit..."" button to open a dialog and change the demo's appearance.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\CustomAppearance.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\CustomAppearance.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomDrawModule",
					displayName: @"Custom Draw",
					group: "Customization",
					type: "DevExpress.XtraScheduler.Demos.CustomDrawModule",
					description: @"This example demonstrates how custom drawing can be used to modify or enhance the visual appearance of the scheduler. Change the demo's options to switch the scheduler's view and grouping mode. Use checkboxes to specify which elements of the scheduler should be drawn in a custom manner. The module handles CustomDraw* events of the SchedulerControl visual elements and employs System.Drawing.Graphics and DevExpress.Utils.Drawing.GraphicsCache objects.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\CustomDraw.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\CustomDraw.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomAppointmentsModule",
					displayName: @"Custom Appointments",
					group: "Customization",
					type: "DevExpress.XtraScheduler.Demos.CustomAppointmentsModule",
					description: @"This example demonstrates how you can modify the elements which constitute a visual representation of an appointment. Change its display options, specify custom images, change display text and the tooltip appearance. This module handles the ActiveViewChanged, InitAppointmentImages, InitAppointmentDisplayText events of the SchedulerControl object.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\CustomAppointments.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\CustomAppointments.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomWorkTimeModule",
					displayName: @"Custom Work Time",
					group: "Customization",
					type: "DevExpress.XtraScheduler.Demos.CustomWorkTimeModule",
					description: @"This example demonstrates the ability to specify custom work time intervals. To accomplish this, handle the SchedulerControl.QueryWorkTime event, which provides information about the current resource and date for which the work time interval is requested. You can specify different work times for different days. Toggle the ""Custom Work Time"" checkbox and observe changes in time cell coloring, indicating work time changes for specific days.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\CustomWorkTime.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\CustomWorkTime.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomEditAppointmentFormModule",
					displayName: @"Custom Appointment Form",
					group: "Customization",
					type: "DevExpress.XtraScheduler.Demos.CustomEditAppointmentFormModule",
					description: @"This example demonstrates simple modifications of a standard appointment editing form. Double-click any appointment or create a new one to see a custom form in place of the standard form. Note that the form allows editing of the appointment custom properties obtained via custom mappings. This module handles the EditAppointmentFormShowing event of the SchedulerControl object.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\CustomEditAppointmentForm.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\CustomEditAppointmentForm.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomInplaceEditorModule",
					displayName: @"Custom In-place Editor",
					group: "Customization",
					type: "DevExpress.XtraScheduler.Demos.CustomInplaceEditorModule",
					description: @"This example demonstrates how to replace the standard in-place editor with a custom one. Select empty time cells within the SchedulerControl and press Enter to invoke a custom in-place editor which enables you to specify Subject, Label and Description. This module handles the InplaceEditorShowing event of the SchedulerControl object.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\CustomInplaceEditor.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\CustomInplaceEditor.vb"
					}
				),
				new SimpleModule(demo,
					name: "ResourceHeadersModule",
					displayName: @"Resource Headers",
					group: "Customization",
					type: "DevExpress.XtraScheduler.Demos.ResourceHeadersModule",
					description: @"This example demonstrates how to customize resource headers. You have a choice to display a text (resource caption), an image, or both. Explore the demo by changing layout options to see how they work. Double-click any resource header to load an image file and display it in the header.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 3,
					isFeatured: true,
					updatedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\ResourceHeaders.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\ResourceHeaders.vb"
					}
				),
				new SimpleModule(demo,
					name: "AppointmentFilteringModule",
					displayName: @"Appointment Filtering",
					group: "Filtering",
					type: "DevExpress.XtraScheduler.Demos.AppointmentFilteringModule",
					description: @"This example demonstrates how to filter appointments and resources currently being displayed with the Scheduler Control according to certain conditions. In this demo, you're able to select appointments associated with specified sports (represented by appointment labels) and TV channels (represented by resources). Only appointments that meet the criteria will be visible in the current TV program. This module handles the FilterAppointment event of the SchedulerStorage object.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\AppointmentFiltering.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\AppointmentFiltering.vb"
					}
				),
				new SimpleModule(demo,
					name: "UserDefinedFilterModule",
					displayName: @"User-defined Filter",
					group: "Filtering",
					type: "DevExpress.XtraScheduler.Demos.UserDefinedFilterModule",
					description: @"This example demonstrates how to take advantage of the FilterControl (included in DevExpress.XtraEditors assembly) to display appointments according to criteria specified by the end-user. You can construct arbitrary complex logical combinations, based on the available appointment fields.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\UserDefinedFilter.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\UserDefinedFilter.vb"
					}
				),
				new SimpleModule(demo,
					name: "DateNavigatorModule",
					displayName: @"Date Navigator",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.DateNavigatorModule",
					description: @"This example demonstrates how to use the DateNavigator control to select the visible date interval of the scheduler. Change the demo's options to specify whether dates with appointments are drawn in bold, and whether the Today button and week numbers are shown in the DateNavigator. To switch the Scheduler view when the dates are selected in the DateNavigator, handle the DateNavigatorQueryActiveViewType event of the SchedulerControl.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\DateNavigator.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\DateNavigator.vb"
					}
				),
				new SimpleModule(demo,
					name: "RangeControlModule",
					displayName: @"Range Control",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.RangeControlModule",
					description: @"This demo demonstrates how to use RangeControl to scroll and navigate through different time periods within SchedulerControl. Depending on the selected schedule type, RangeControl displays corresponding time scales. Change the demo options to enable or disable the automatic changing of the scheduler view to the most appropriate display type for showing the time range selected within RangeControl, to switch on or switch off auto format for scale captions of RangeControl, to specify whether appointment data should be displayed as appointment thumbnails or numbers of appointments within RangeControl, and to set a maximum number of intervals that can be selected within RangeControl. Use a slider under RangeControl to change the number of visible intervals in RangeControl and scroll the RangeControl area.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 1,
					newUpdatedPriority: 0,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\RangeControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\RangeControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "RemindersModule",
					displayName: @"Reminders",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.RemindersModule",
					description: @"This example demonstrates appointment reminders in the scheduler control. Click the ""Go To Today"" and ""Create Appointment with reminder"" buttons to create an appointment with a reminder due in 5 minutes time. Then, a reminder alert will be invoked to provide a notification about the event.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reminders.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reminders.vb"
					}
				),
				new SimpleModule(demo,
					name: "RestrictionsModule",
					displayName: @"End-User Restrictions",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.RestrictionsModule",
					description: @"This example demonstrates how to set end-user restrictions in the scheduler control. Change the demo's options to specify what actions end-users can perform with the scheduled appointments.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 5,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Restrictions.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Restrictions.vb"
					}
				),
				new SimpleModule(demo,
					name: "ResourceSharingModule",
					displayName: @"Resource Sharing",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.ResourceSharingModule",
					description: @"This example demonstrates how to share resources between appointments to create multi-resource appointments. To assign any appointment to several resources, open its Edit Appointment form, click on the Resource drop-down icon and choose the required resources in the drop-down list. Note: Resource sharing is not enabled by default. To use this feature you should set the SchedulerStorage.ResourceSharing property to true. Make sure that your data table structure is appropriate for resource sharing, since the mapped Appointments.ResourceID data field will hold xml string data.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\ResourceSharing.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\ResourceSharing.vb"
					}
				),
				new SimpleModule(demo,
					name: "DragDropDataModule",
					displayName: @"Drag&Drop Data",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.DragDropDataModule",
					description: @"This example demonstrates how to drag appointments data from any Windows Forms control onto the Scheduler Control. In this demo you're able to choose a task to add in the grid below, then drag&drop it on the selected resource. Then, a new appointment with the corresponding data will be added to the Scheduler Storage and displayed in the Scheduler Control.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 4,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\DragDropData.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\DragDropData.vb"
					}
				),
				new SimpleModule(demo,
					name: "HolidaysModule",
					displayName: @"Holidays",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.HolidaysModule",
					description: @"This example demonstrates how to add statutory holidays to the Scheduler Storage. You may load them from the previously created *.xml file (e.g. holidays.xml), or from the OUTLOOK.HOL file which is shipped with Microsoft Outlook (for MS Office 2007 the default location is ""[MS Office 2007 Directory]\Office12\1033\""). After holiday dates are loaded, select the country (location) and click the ""Generate Appointments"" button to create all-day appointment for every item in a holiday list.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Holidays.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Holidays.vb"
					}
				),
				new SimpleModule(demo,
					name: "TimeZonesModule",
					displayName: @"Time Zones",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.TimeZonesModule",
					description: @"This example demonstrates time zone support. Use the combo box to change the  SchedulerControl.OptionsBehavior.ClientTimeZoneId property that specifies the Scheduler time zone. Right-click a time ruler and select Customize Time Ruler in the popup menu to change the Time Ruler time zone.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\TimeZones.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\TimeZones.vb"
					}
				),
				new SimpleModule(demo,
					name: "SplitAppointmentToolModule",
					displayName: @"Appointment Splitting",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.SplitAppointmentToolModule",
					description: @"This example illustrates the use of an Appointment Split tool allowing end-users to split an appointment in two by simply dragging a splitter line. Select the Split command from the appointment's context menu, drag the splitter to the required time and click the left mouse button. You can use the mouse wheel to adjust the time scale step which determines the splitter precision.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\SplitAppointmentTool.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\SplitAppointmentTool.vb"
					}
				),
				new SimpleModule(demo,
					name: "SchedulerBarsModule",
					displayName: @"Scheduler Bars",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.SchedulerBarsModule",
					description: @"This example demonstrates Bar UI allowing end-users to perform basic operations in SchedulerControl. To implement these bars in your application, add BarManager onto a form and create required bars using corresponding items of SchedulerControl’s smart tag panel at design time.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\SchedulerBars.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\SchedulerBars.vb"
					}
				),
				new SimpleModule(demo,
					name: "SchedulerRibbonBarsModule",
					displayName: @"Scheduler Ribbon Bars",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.SchedulerRibbonBarsModule",
					description: @"This example demonstrates Ribbon UI allowing end-users to perform basic operations in SchedulerControl. To implement these Ribbon bars in your application, add RibbonControl onto a form and create required bars using corresponding items of SchedulerControl’s smart tag panel at design time.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\SchedulerRibbonBars.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\SchedulerRibbonBars.vb"
					}
				),
				new SimpleModule(demo,
					name: "AppointmentFormStylesModule",
					displayName: @"Appointment Form Styles",
					group: "Calendar Features",
					type: "DevExpress.XtraScheduler.Demos.AppointmentFormStylesModule",
					description: @"This example demonstrates two bonus Appointment Forms shipped with the XtraScheduler suite, to mimic the Microsoft Outlook and Microsoft Office Ribbon UI styles. Simply choose an appropriate button and create a new appointment to see them in action.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\AppointmentFormStyles.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\AppointmentFormStyles.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.DailyStyle",
					displayName: @"Daily Style",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.DailyStyle.PreviewControl",
					description: @"This demo illustrates how to create a report which corresponds to the Day View of the Scheduler, and mimics the Daily print style. You can specify the reported time interval in days, the visible time of day, and the number of resources displayed in a column allocated for one day. Another option, related to the column arrangement, is the number of days per column group. Appointments outside the visible time of day can be printed in the extra cells which are located below the scheduling area. Check the Print All Appointments checkbox to accomplish this. The Edit button invokes the End-User Designer which enables you to fully customize the report.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\DailyStyle\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\DailyStyle\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.WeeklyStyle",
					displayName: @"Weekly Style",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.WeeklyStyle.PreviewControl",
					description: @"This demo illustrates how to create a report which corresponds to the Week View of the Scheduler and mimics the Weekly Top to Bottom print style. You can specify the reported time interval in days, and the number of resources displayed in a column allocated for one week. A week can be fitted into a single page to save space, or extended to several pages, providing more detailed view. The Edit button invokes the End-User Designer which enables you to fully customize the report.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\WeeklyStyle\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\WeeklyStyle\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.MonthlyStyle",
					displayName: @"Monthly Style",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.MonthlyStyle.PreviewControl",
					description: @"This demo illustrates how to create a report which corresponds to the Month View of the Scheduler and mimics the Monthly print style. You can specify the number of resources per page. A month can be fitted into a single page to save space, or extended to several pages providing more detailed view. Other options include hiding weekends or printing them compressed, so two days occupy a single cell. Also, you can enhance the layout by placing exactly one month on a page, hiding the appointments scheduled outside the current month. The Edit button invokes the End-User Designer, which enables you to fully customize the report.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\MonthlyStyle\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\MonthlyStyle\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.TimelineStyle",
					displayName: @"Timeline Style",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.TimelineStyle.PreviewControl",
					description: @"This demo illustrates how to create a report which corresponds to the Timeline View of the Scheduler. You can specify the number of resources and the number of scale intervals displayed in a column. Notice that you can imitate zooming in and out by selecting different time scales for the report, to present your data in the most effective manner. The Edit button invokes the End-User Designer which enables you to fully customize the report.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\TimelineStyle\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\TimelineStyle\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.GroupType",
					displayName: @"Group Type",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.GroupType.PreviewControl",
					description: @"This demo illustrates how the grouping mode affects the report layout. Any grouping results in iterations performed over the specified time interval and the collection of resources. Notice that the iteration order changes when you change the grouping type. The Edit button invokes the End-User Designer which enables you to fully customize the report.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\GroupType\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\GroupType\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.ReportTemplates",
					displayName: @"Report Templates",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.ReportTemplates.ReportTemplatesModule",
					description: @"This demo illustrates the use of report templates for creating reports. You can load any previously saved template and instantly preview a report filled with data retrieved from the current Scheduler control.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 6,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\ReportTemplates\ReportTemplates.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\ReportTemplates\ReportTemplates.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.MultiColumn",
					displayName: @"Multi-Column",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.MultiColumn.PreviewControl",
					description: @"This demo illustrates how the report is rendered into columns. A week can be split into several columns, and you can specify whether the number of days in each column ascend or descend in number. In this example with four columns, it can be 1-1-2-2 (ascending) or 2-2-1-1 (descending). The Edit button invokes the End-User Designer which enables you to fully customize the report.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\MultiColumn\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\MultiColumn\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.AutoHeight",
					displayName: @"Auto Height",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.AutoHeight.PreviewControl",
					description: @"This demo illustrates the AutoHeight feature that enables the time cell to automatically adjust its size to accommodate the appointments it contains. The CanShrink property allows you to avoid empty space wasted by cells with no appointments, while the CanGrow property can be used to resize a cell to show all appointments instead of printing the ""More items"" link.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\AutoHeight\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\AutoHeight\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.SmartSyncPrinting",
					displayName: @"SmartSync Printing",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.SmartSyncPrinting.PreviewControl",
					description: @"This demo illustrates the SmartSync Printing feature used to create a Tri-fold Printing Style report. If several Scheduler Report controls are placed on the same report, the Scheduler Adapter can coordinate how controls iterate through data. This feature can be turned on via the EnableSmartSync property of the Scheduler Print Adapter (by default it is off). The SmartSyncOptions property enables you to specify the group type, i.e. the first parameter through which the controls iterate (date or resource).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\SmartSyncPrinting\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\SmartSyncPrinting\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.AppointmentCustomization",
					displayName: @"Appointment Customization",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.AppointmentCustomization.PreviewControl",
					description: @"This example demonstrates how you can selectively modify the elements which constitute a visual representation of an appointment in the report. Unlike the AppointmentDisplayOptions properties which applies to all appointments, the event-based mechanism employed in this demo enables you to decide how to display a particular appointment. This module handles the AppointmentViewInfoCustomizing, InitAppointmentImages, InitAppointmentDisplayText events of the DayViewTimeCells control.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\AppointmentCustomization\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\AppointmentCustomization\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.VisibleWeekDays",
					displayName: @"Visible Week Days",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.VisibleWeekDays.PreviewControl",
					description: @"This demo illustrates how you can hide certain weekdays in a report for the Day and Month views. The Edit button invokes the End-User Designer, which enables you to fully customize the report. NOTE: Scheduler reporting functionality requires the XtraReports Suite (available as a separate product or as part of DXperience subscription packages).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\VisibleWeekDays\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\VisibleWeekDays\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.DateFormatting",
					displayName: @"Date Formatting",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.DateFormatting.PreviewControl",
					description: @"This example demonstrates how to modify formats applied to header captions and start/end indications within appointments. The demo uses the service substitution technique for the HeaderCaptionService and the AppointmentFormatStringService of the Scheduler.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\DateFormatting\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\DateFormatting\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.DataValidation",
					displayName: @"Print Data Validation",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.DataValidation.PreviewControl",
					description: @"This demo illustrates how you can filter or re-shape data before the report is generated. You can modify the collection of resources, set the criteria for appointments and select the days to print. The Edit button invokes the End-User Designer, which enables you to fully customize the report.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\DataValidation\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\DataValidation\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.Appearance",
					displayName: @"Appearance",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.Appearance.PreviewControl",
					description: @"This demo illustrates how you can change the appearance settings of the view elements and the resource color schemes. You can also select the print color schema, which specifies how the entire report or its elements should be printed: in color, grayscale or black-and-white. All-day area, all-day appointments, appointments and time cell content can have different print color settings. Note that these changes affect only the report, and not the original Scheduler.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\Appearance\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\Appearance\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Reporting.CustomDraw",
					displayName: @"Custom Drawing",
					group: "Reporting",
					type: "DevExpress.XtraScheduler.Demos.Reporting.CustomDraw.PreviewControl",
					description: @"This example demonstrates how to paint the scheduler report objects manually to enhance their visual appearance. Use checkboxes to specify which elements of the scheduler should be drawn in a custom manner. The module handles CustomDraw* events of the XtraSchedulerReport controls.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SchedulerMainDemo\Modules\Reporting\CustomDraw\PreviewControl.cs",
						@"\WinForms\VB\SchedulerMainDemo\Modules\Reporting\CustomDraw\PreviewControl.vb"
					}
				)
			};
		}
	}
}
