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
		static List<Module> Create_DXScheduler_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "DateNavigator",
					displayName: @"Date Navigator",
					group: "Calendar Features",
					type: "SchedulerDemo.DateNavigator",
					shortDescription: @"Use the DateNavigator control to select the visible date interval of the scheduler.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to use the DateNavigator control to select the visible date interval of the scheduler. Change the demo's options to specify whether dates with appointments are drawn in bold, and whether the Today button and week numbers are shown in the DateNavigator.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Date Navigator",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument10025"),
						new WpfModuleLink(
							title: "How to: Use a Date Navigator in a Scheduling Application",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9897")
					}
				),
				new WpfModule(demo,
					name: "TimeZones",
					displayName: @"Time Zones",
					group: "Calendar Features",
					type: "SchedulerDemo.TimeZones",
					shortDescription: @"Set the scheduler's time zone and add time rulers to display the time in different time zones.",
					description: @"
                        <Paragraph>
                        This example demonstrates time zone support. The SchedulerControl.OptionsBehavior.ClientTimeZoneId property specifies the Scheduler time zone. All-day appoitments can belong to the ""floating"" time zone that ignores time zone setting.
                        </Paragraph>
                        <Paragraph>
                        Use combobox to specify the ClientTimeZoneId property. The Scheduler displays appointments using the start and end values converted to the time zone specified by the ClientTimeZoneId setting. By default, it is the same as the time zone of the host system.
                        </Paragraph>
                        <Paragraph>
                        Check the Floating Time for All-day Events box to display all-day appointment using the ""floating"" time zone. ""Floating"" means that the time is independent of the client time zone. A typical example of a ""floating"" all-day appointment would be New Year's Day. It starts at midnight on January 1st in any time zone. Therefore, it does not start simultaneously throughout the world - it starts at different times, according to the time zone where the client is located.
                        </Paragraph>
                        <Paragraph>Right-click the Time Ruler and select Customize Time Ruler in the popup menu to change the Time Ruler time zone setting.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Time Zones",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WindowsForms/CustomDocument5041"),
						new WpfModuleLink(
							title: "Time Ruler",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8726")
					}
				),
				new WpfModule(demo,
					name: "Reminders",
					displayName: @"Reminders",
					group: "Calendar Features",
					type: "SchedulerDemo.Reminders",
					shortDescription: @"Create an appointment with reminder and see an alert invoked.",
					description: @"
                        <Paragraph>
                        This example demonstrates appointment reminders. Click the 'Create Appointment with Reminder' button to create an appointment with a reminder due in 5 minutes time. By default, all reminder alerts are checked every 15000 milliseconds, as specified by the RemindersCheckInterval property. Then, a reminder alert will be invoked to provide a notification about the event. You can open the appointment that triggered an alert or dismiss it. Another option is to hit the Snooze button, to be reminded again after a specified time interval.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Reminders for Appointments",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8736")
					}
				),
				new WpfModule(demo,
					name: "EndUserRestrictions",
					displayName: @"End-User Restrictions",
					group: "Calendar Features",
					type: "SchedulerDemo.EndUserRestrictions",
					shortDescription: @"Specify what actions end-users can perform with appointments.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to set end-user restrictions in the Scheduler. Use the 'Allow Appointment' control group to specify what actions end-users can perform with the scheduled appointments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "How to: Set End-User Restrictions",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8682")
					}
				),
				new WpfModule(demo,
					name: "RangeControl",
					displayName: @"Range Control",
					group: "Calendar Features",
					type: "SchedulerDemo.RangeControl",
					shortDescription: @"How to use range control with scheduler.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to use range control with scheduler..
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DragDropData",
					displayName: @"Drag And Drop Data",
					group: "Calendar Features",
					type: "SchedulerDemo.DragDropData",
					shortDescription: @"Explore the drag and drop functionality by dragging tasks from the grid.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to drag appointments data from any Windows Forms control onto the Scheduler Control. In this demo you're able to choose a task to add in the grid below, then drag-and-drop it on the selected resource. Then, a new appointment with the corresponding data will be added to the storage and displayed in the Scheduler control.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "ReportTemplates",
					displayName: @"Report Templates",
					group: "Reporting",
					type: "SchedulerDemo.ReportTemplates",
					shortDescription: @"",
					description: @"
                        <Paragraph>
                        This demo illustrates the use of report templates for creating reports. You can load any previously saved template and instantly preview a report filled with data retrieved from the current Scheduler control.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DocumentPreview",
					displayName: @"Document Preview",
					group: "Reporting",
					type: "SchedulerDemo.DocumentPreview",
					shortDescription: @"",
					description: @"
                        <Paragraph>
                        This demo illustrates how to create a report of the Scheduler.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "HorizontalResourceHeaderStyles",
					displayName: @"Horizontal Resource Header Style",
					group: "Styles",
					type: "SchedulerDemo.HorizontalResourceHeaderStyles",
					shortDescription: @"Modify the visual appearance of the horizontal resource headers on the fly.",
					description: @"
                        <Paragraph>
                        This example demonstrates the use of custom templates defined in XAML to modify the visual content and layout of horizontal resource headers. The header template can be used to change font and background, and the content template allows showing pictures of resources.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Styles and Templates Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9187")
					}
				),
				new WpfModule(demo,
					name: "VerticalResourceHeaderStyles",
					displayName: @"Vertical Resource Header Style",
					group: "Styles",
					type: "SchedulerDemo.VerticalResourceHeaderStyles",
					shortDescription: @"Modify the visual appearance of the vertical resource headers on the fly.",
					description: @"
                        <Paragraph>
                        This example demonstrates the use of custom templates defined in XAML to modify visual content and layout of vertical resource headers. The header template can be used to change font and background, and the content template allows showing pictures of resources.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Styles and Templates Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9187")
					}
				),
				new WpfModule(demo,
					name: "AppointmentStyles",
					displayName: @"Appointment Style",
					group: "Styles",
					type: "SchedulerDemo.AppointmentStyles",
					shortDescription: @"Modify the visual appearance of appointments by using styles.",
					description: @"
                        <Paragraph>
                        This example demonstrates the use of custom templates defined in XAML to modify an appointment visual content and layout. Horizontal appointment templates are used in WeekView, MonthView, and in the all-day area of the Day and WorkWeek views. Vertical appointment templates are used in the Day and WorkWeek views to represent appointments located in time cells.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Styles and Templates Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9187")
					}
				),
				new WpfModule(demo,
					name: "CellStyles",
					displayName: @"Cell Style",
					group: "Styles",
					type: "SchedulerDemo.CellStyles",
					shortDescription: @"Customize cell styles via templates.",
					description: @"
                        <Paragraph>
                        This example demonstrates the use of custom templates defined in XAML to modify the color of time cells.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Styles and Templates Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9187")
					}
				),
				new WpfModule(demo,
					name: "VisualElementStyles",
					displayName: @"Visual Element Styles",
					group: "Styles",
					type: "SchedulerDemo.VisualElementStyles",
					shortDescription: @"Customize the appearance of the scheduler's visual elements using templates.",
					description: @"
                        <Paragraph>
                        This example demonstrates the use of custom templates defined in XAML to change how visual elements of the Scheduler, such as navigation and 'more' buttons, look.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Styles and Templates Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9187")
					}
				),
				new WpfModule(demo,
					name: "DateHeaderStyleModule",
					displayName: @"Date Header Style",
					group: "Styles",
					type: "SchedulerDemo.DateHeaderStyleModule",
					shortDescription: @"Customize headers via styles.",
					description: @"
                        <Paragraph>
                        This example demonstrates the use of custom templates defined in XAML to modify the visual appearance of date headers and the format used to display time intervals.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Styles and Templates Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9187")
					}
				),
				new WpfModule(demo,
					name: "BoundMode",
					displayName: @"Bound Mode",
					group: "Data Binding",
					type: "SchedulerDemo.BoundMode",
					shortDescription: @"Bind a scheduler to an external data source.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Scheduler bound to data.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "How to: Bind a Scheduler to Data",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8653"),
						new WpfModuleLink(
							title: "How to: Provide Resources for Appointments",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8654")
					}
				),
				new WpfModule(demo,
					name: "ListBoundMode",
					displayName: @"List Bound Mode",
					group: "Data Binding",
					type: "SchedulerDemo.ListBoundMode",
					shortDescription: @"Bind a scheduler to data exposed via the IBindingList interface.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Scheduler bound to an object with the IBindingList interface.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "EntityBoundMode",
					displayName: @"Entity Bound Mode",
					group: "Data Binding",
					type: "SchedulerDemo.EntityBoundMode",
					shortDescription: @"Bind a scheduler to data exposed via the entity framework.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Scheduler bound to an object with the entity framework.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ObjectDataProvider",
					displayName: @"ObjectDataProvider",
					group: "Data Binding",
					type: "SchedulerDemo.ObjectDataProvider",
					shortDescription: @"Use the ObjectDataProvider to bind the data source directly in the XAML markup.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Scheduler bound to data in XAML via the ObjectDataProvider.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "How to: Bind a Scheduler to Data via ObjectDataProvider",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9812")
					}
				),
				new WpfModule(demo,
					name: "UnboundMode",
					displayName: @"Unbound Mode",
					group: "Data Binding",
					type: "SchedulerDemo.UnboundMode",
					shortDescription: @"Use the Scheduler without data source.",
					description: @"
                        <Paragraph>
                        This example demonstrates that the Scheduler can be used in unbound mode. The data for appointments and resources are loaded from external xml files into the Scheduler Storage.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "iCalendarExchange",
					displayName: @"iCalendar Exchange",
					group: "Data Exchange",
					type: "SchedulerDemo.iCalendarExchange",
					shortDescription: @"Explore the functionality that allows you to export/import data in the iCalendar format.",
					description: @"
                        <Paragraph>
                        Click the 'Export' button to load data from the scheduler to the text box converted to iCalendar format. You can modify text box contents or paste any iCalendar data in it and click the 'Import' button to load data into the scheduler.
                        </Paragraph>
                        <Paragraph>
                        You are encouraged to experiment with iCalendar data to explore the opportunities that the support for iCalendar format provides.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "iCalendar Support",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument11212"),
						new WpfModuleLink(
							title: "How to: Import Data from iCalendar",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument11206"),
						new WpfModuleLink(
							title: "How to: Export Data to iCalendar",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument11207")
					}
				),
				new WpfModule(demo,
					name: "ImportFromOutlook",
					displayName: @"Import from Outlook",
					group: "Data Exchange",
					type: "SchedulerDemo.ImportFromOutlook",
					shortDescription: @"Transfer data from the Microsoft Outlook calendar to the scheduler.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to import data from Microsoft Outlook to the Scheduler Storage. If you have MS Outlook installed on your machine, you can choose which appointments should be imported, and click the 'Import' button to start importing. Note that it's also possible to export data from the Scheduler Storage to MS Outlook. However, to prevent your data from being removed, it can't be done in this demo. The OnException event is handled so if an error occurs while importing, no appointments are added to the scheduler.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SynchronizeWithOutlook",
					displayName: @"Synchronize with Outlook",
					group: "Data Exchange",
					type: "SchedulerDemo.SynchronizeWithOutlook",
					shortDescription: @"Perform the scheduler synchronization with the Microsoft Outlook calendar.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to synchronize current data in the Scheduler Storage with appointments stored in Microsoft Outlook. If you have MS Outlook installed on your machine, you can first manually add some appointments to the Scheduler, then specify synchronization options and click the 'Synchronize' button to start synchronization. Note that it's also possible to synchronize current data in MS Outlook with appointments stored in the Scheduler Storage. However, to prevent your data from being removed, it can't be done in this demo. The OnException event is handled so if an error occurs while importing, no appointments are added to the scheduler.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "AppointmentFiltering",
					displayName: @"Appointment Filtering",
					group: "Grouping and Filtering",
					type: "SchedulerDemo.AppointmentFiltering",
					shortDescription: @"Apply filter on resource and label and display only appointments that meet the criteria.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to filter appointments currently being displayed with the Scheduler Control according to certain conditions. In this demo, you're able to show only appointments associated with a certain sport (represented by appointment label).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "GroupByDate",
					displayName: @"Group by Date",
					group: "Grouping and Filtering",
					type: "SchedulerDemo.GroupByDate",
					shortDescription: @"Group appointments first by date and then by resource.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to group scheduled appointments by dates. Date headers and resource headers appear in the view. Note the Resource Navigator located at the bottom right. Use the radio buttons to see how this grouping looks in different views.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Resources",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8710"),
						new WpfModuleLink(
							title: "How to: Group Appointments by Dates",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9248")
					}
				),
				new WpfModule(demo,
					name: "GroupByResource",
					displayName: @"Group by Resource",
					group: "Grouping and Filtering",
					type: "SchedulerDemo.GroupByResource",
					shortDescription: @"Group appointments first by resource and then by date.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to group scheduled appointments by resources. Resource headers appear in the view. Note the Resource Navigator located at the bottom right. Use the radio buttons to see how this grouping looks in different views.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Resources",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8710"),
						new WpfModuleLink(
							title: "How to: Group Appointments by Resources",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8918")
					}
				),
				new WpfModule(demo,
					name: "ResourceSharing",
					displayName: @"Resource Sharing",
					group: "Grouping and Filtering",
					type: "SchedulerDemo.ResourceSharing",
					shortDescription: @"Schedule appointments for different resources so each appointment has several resources assigned.",
					description: @"
                        <Paragraph>
                        This example demonstrates appointments with shared resources (multi-resource appointments). Drag an appointment to observe its other resource projections moving together. To enable resource sharing, set the SchedulerStorage.ResourceSharing property to true and ensure that the mapped Appointments.ResourceID data field can hold xml string data.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Resources",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8710")
					}
				),
				new WpfModule(demo,
					name: "DayView",
					displayName: @"Day View",
					group: "View Types",
					type: "SchedulerDemo.DayView",
					shortDescription: @"Display the most detailed view of appointments for selected days.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Day View, which provides the most detailed view of the appointments for a particular day. Change the options to specify the number of days shown in the view at one time, to display working hours only, and to hide or show the all-day area and day headers. Resize an appointment to examine different Snap-To-Cell modes. See how the appointment status line can be used to visualize its time interval.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Day View",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8674")
					}
				),
				new WpfModule(demo,
					name: "WorkWeekView",
					displayName: @"Work Week View",
					group: "View Types",
					type: "SchedulerDemo.WorkWeekView",
					shortDescription: @"Display appointments for the working days in a week.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Work Week View, which displays appointments for the working days in a particular week. Use the checkboxes to specify the working days to display. You can enable all-day area scrolling to show appointments which do not fit in the area.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Work-Week View",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8675")
					}
				),
				new WpfModule(demo,
					name: "WeekView",
					displayName: @"Week View",
					group: "View Types",
					type: "SchedulerDemo.WeekView",
					shortDescription: @"Display appointments for a weekly period.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Week View, which displays appointments for a weekly period. You can change the first day of the week shown. Use the appointment options to specify whether the appointment's start and end times are displayed as clocks or as text, and whether the start-end time is shown at all. See how the appointment status line can be used to visualize its time interval.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Week View",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8676")
					}
				),
				new WpfModule(demo,
					name: "FullWeekView",
					displayName: @"Full Week View",
					group: "View Types",
					type: "SchedulerDemo.FullWeekView",
					shortDescription: @"Display appointments for a weekly period.",
					description: @"
                        <Paragraph>
                        This example demonstrates use of the Scheduler control's Full Week View. It displays appointments for all days within a specific week.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "MonthView",
					displayName: @"Month View",
					group: "View Types",
					type: "SchedulerDemo.MonthView",
					shortDescription: @"Provide an overview of appointments by displaying several weeks simultaneously.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Month View, which is designed to provide end-users with an overview of appointments. Change the demo's options to set the number of weeks shown in the view at one time, to specify the first day of the week and whether weekends (two fixed days - Saturday and Sunday) are displayed in one box. Use appointment options to select display modes for appointment start and end times. See how appointment status line can be used to visualize its time interval.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Month View",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8677")
					}
				),
				new WpfModule(demo,
					name: "TimeLineView",
					displayName: @"Timeline View",
					group: "View Types",
					type: "SchedulerDemo.TimeLineView",
					shortDescription: @"Show resources with the appointments assigned to them over time. Different levels of detail can be enabled.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Timeline View, which displays appointments as horizontal bars along the timescales, and provides end-users with a clearer overview for scheduling purposes. You can specify the number of days shown at once and select time scales to display.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Timeline View",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8678")
					}
				),
				new WpfModule(demo,
					name: "CustomMenu",
					displayName: @"Custom Menu",
					group: "Customization",
					type: "SchedulerDemo.CustomMenu",
					shortDescription: @"Customize the default popup menu by adding new items, removing unneeded and moving existing items.",
					description: @"
                        <Paragraph>
                        This example demonstrates that the scheduler's context menu can be customized to include arbitrary actions. Right-click the scheduler's area to invoke the menu that enables you to create appointments with predefined characteristics.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "How to: Customize the Popup Menu",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9180")
					}
				),
				new WpfModule(demo,
					name: "CustomAppointmentForm",
					displayName: @"Custom Appointment Form",
					group: "Customization",
					type: "SchedulerDemo.CustomAppointmentForm",
					shortDescription: @"See how the editing appointment form can be tailored to suit the requirements.",
					description: @"
                        <Paragraph>
                        This example demonstrates simple modifications of a standard appointment editing form. Double-click any appointment or create a new one to see a custom form in place of the standard form. Note that the form allows editing of the appointment custom properties obtained via custom mappings.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "How to: Create a Custom Edit Appointment Form",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8655"),
						new WpfModuleLink(
							title: "How to: Create a Custom Appointment Recurrence Form",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument11183")
					}
				),
				new WpfModule(demo,
					name: "AppointmentCustomization",
					displayName: @"Appointment Customization",
					group: "Customization",
					type: "SchedulerDemo.AppointmentCustomization",
					shortDescription: @"Modify text and pictures displayed in the appointment body.",
					description: @"
                        <Paragraph>
                        This example demonstrates how you can modify the visual elements which constitute a body of an appointment. Specify custom images, change display text and colors.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Styles and Templates Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9187")
					}
				),
				new WpfModule(demo,
					name: "MVVMAppointmentForm",
					displayName: @"MVVM Appointment Forms",
					group: "Customization",
					type: "SchedulerDemo.MVVMAppointmentForm",
					shortDescription: @"Use the MVVM design pattern to customize default appointment editing forms.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to substitute the standard <Bold>Edit Appointment</Bold> and  <Bold>Appointment Recurrence</Bold> forms with custom forms using the MVVM approach. The new technique is based on a service provided by a data template. This service allows displaying a form in a separate window.
                        </Paragraph>
                        <Paragraph>
                        Double-click any appointment or create a new one to display a custom appointment editing form. This form allows editing the appointment custom properties obtained via custom mappings. Click the <Bold>Recurrence</Bold> button within the form to invoke the tailored <Bold>Appointment Recurrence</Bold> dialog.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "How to: Create a Custom Edit Appointment Form Using the MVVM Pattern",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument16994")
					}
				),
				new WpfModule(demo,
					name: "CustomWorkTime",
					displayName: @"Custom Work Time",
					group: "Customization",
					type: "SchedulerDemo.CustomWorkTime",
					shortDescription: @"Define a different working time for different days.",
					description: @"
                        <Paragraph>
                        This example demonstrates the ability to specify custom work time intervals. To accomplish this, handle the <Bold>SchedulerControl.QueryWorkTime</Bold> event, which provides information about the current resource and date for which the work time interval is requested. You can specify different work times for different days. Toggle the <Bold>Custom Work Time</Bold> checkbox and observe changes in time cell coloring, indicating work time changes for specific days.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CustomInplaceEditor",
					displayName: @"Custom In-place Editor",
					group: "Customization",
					type: "SchedulerDemo.CustomInplaceEditor",
					shortDescription: @"Replace the standard in-place editor with your own.",
					description: @"
                        <Paragraph>
                        This example demonstrates the use of a custom in-place editor instead of a standard one. The InplaceEditorShowing event of the SchedulerControl is handled to specify a custom form to show. The custom editor form is a descendant of the AppointmentInplaceEditorBase class.
                        </Paragraph>
                        <Paragraph>
                        Select an empty time cell within the SchedulerControl and press Enter to invoke a custom in-place editor allowing you to specify Subject, Label and Description fields.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "How to: Customize the In-place Editor",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9289")
					}
				),
				new WpfModule(demo,
					name: "SchedulerBars",
					displayName: @"Scheduler Toolbar UI Commands",
					group: "Bars",
					type: "SchedulerDemo.SchedulerBars",
					shortDescription: @"See how the basic functionality for switching views, grouping, navigation and filtering can be implemented in command bars.",
					description: @"
                        <Paragraph>
                        This example demonstrates Bar UI that you can create for SchedulerControl to provide your end-users with the basic functionality for switching views, view navigation, and grouping data. To implement these bars in your application, right-click SchedulerControl at design time and select the View Navigator, View Selector or Group By item from the Create Bars submenu of the invoked context menu.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "How to: Provide Bars UI for a Scheduler",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8703")
					}
				),
				new WpfModule(demo,
					name: "SchedulerRibbonBars",
					displayName: @"Scheduler Ribbon UI Commands",
					group: "Bars",
					type: "SchedulerDemo.SchedulerRibbonBars",
					shortDescription: @"See how the basic functionality for switching views and view navigation can be implemented in Ribbon interface.",
					description: @"
                        <Paragraph>
                        This example demonstrates Ribbon UI that you can create for SchedulerControl to provide your end-users with the basic functionality for switching views, view navigation, and grouping data. To implement these Ribbon bars in your application, right-click SchedulerControl at design time and select the View Navigator, View Selector or Group By item from the Create Ribbon Items submenu of the invoked context menu.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "How to: Provide Ribbon UI for a Scheduler",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8691")
					}
				)
			};
		}
	}
}
