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
using DevExpress.XtraReports.UserDesigner.Native;
using System.Drawing.Design;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Localization;
using System.Drawing;
using DevExpress.Utils;
using System.Reflection;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraScheduler.Reporting.Design {
	#region SchedulerReportToolboxService
	public class SchedulerReportToolboxService : XRToolboxService {
		#region Static
		static Type[] filteredXRControlTypes = {
			typeof(DevExpress.XtraReports.UI.XRSubreport),
			typeof(DevExpress.XtraReports.UI.XRCrossBandBox),
			typeof(DevExpress.XtraReports.UI.XRCrossBandLine)
		};
		static Type[] schedulerReportingControlTypes = { 
			typeof(SchedulerStoragePrintAdapter),
			typeof(SchedulerControlPrintAdapter),
			typeof(DayViewTimeCells),
			typeof(HorizontalWeek),
			typeof(FullWeek),
			typeof(TimelineCells),
			typeof(HorizontalResourceHeaders),
			typeof(VerticalResourceHeaders),
			typeof(HorizontalDateHeaders),
			typeof(DayOfWeekHeaders),
			typeof(TimelineScaleHeaders),
			typeof(CalendarControl),
			typeof(TimeIntervalInfo),
			typeof(FormatTimeIntervalInfo),
			typeof(ResourceInfo),
			typeof(DayViewTimeRuler),
			typeof(ReportDayView),
			typeof(ReportWeekView),
			typeof(ReportMonthView),
			typeof(ReportTimelineView)
		};
		static ToolboxItem[] schedulerReportingToolboxItems;
		public static ToolboxItem[] SchedulerReportingToolboxItems {
			get {
				if (schedulerReportingToolboxItems == null)
					schedulerReportingToolboxItems = CreateToolboxItems(CreateToolboxItem);
				return schedulerReportingToolboxItems;
			}
		}
		static ToolboxItem[] CreateToolboxItems(Function<ToolboxItem, Type> itemCreator) {
			ToolboxItem[] items = new ToolboxItem[schedulerReportingControlTypes.Length];
			for (int i = 0; i < schedulerReportingControlTypes.Length; i++) {
				items[i] = itemCreator.Invoke(schedulerReportingControlTypes[i]);
			}
			return items;
		}
		static ToolboxItem CreateToolboxItem(Type type) {
			return new ToolboxItem(type);
		}
		public static Type[] FilteredXRControlTypes { get { return filteredXRControlTypes; } }
		#endregion
		List<Type> toolBoxControlTypes;
		List<string> filteredToolBoxTypeNames;
		public SchedulerReportToolboxService() {
			AddToolBoxImages();
		}
		public List<Type> ToolBoxControlTypes { 
			get {
				if (toolBoxControlTypes == null) {
					toolBoxControlTypes = new List<Type>();
					PopulateToolBoxControlTypes();
				}
				return toolBoxControlTypes; 
			} 
		}
		protected List<string> FilteredToolBoxTypeNames {
			get {
				if (filteredToolBoxTypeNames == null) {
					filteredToolBoxTypeNames = new List<string>();
					PopulateFilteredToolBoxTypeNames();
				}
				return filteredToolBoxTypeNames;
			}
		}
		protected internal virtual void PopulateFilteredToolBoxTypeNames() {
			int count = FilteredXRControlTypes.Length;
			for (int i = 0; i < count; i++) {
				FilteredToolBoxTypeNames.Add(FilteredXRControlTypes[i].FullName);
			}
		}
		protected internal virtual void PopulateToolBoxControlTypes() {
			int count = schedulerReportingControlTypes.Length;
			for (int i = 0; i < count; i++) {
				ToolBoxControlTypes.Add(schedulerReportingControlTypes[i]);
			}
		}
		public override string DefaultCategoryName {
			get {
				return SchedulerLocalizer.GetString(SchedulerStringId.UD_SchedulerReportsToolboxCategoryName);
			}
		}	   
		protected override void AddToolboxItems() {
			AddSchedulerToolboxItems();
			base.AddToolboxItems();
		}
		protected internal virtual void AddSchedulerToolboxItems() {
			ToolboxItem[] schedulerItems = GetSchedulerToolboxItems();
			for (int i = 0; i < schedulerItems.Length; i++) {
				AddToolboxItem(schedulerItems[i], DefaultCategoryName);
			}
		}
		void AddToolBoxImages() {
			string toolBoxImagesName = "DevExpress.XtraScheduler.Reporting.Extensions.Images.UserDesignerToolbox.png";
			ImageCollection imageCollection = ImageHelper.CreateImageCollectionFromResources(toolBoxImagesName, Assembly.GetExecutingAssembly(), new Size(24, 24));
			for(int i = 0; i < ToolBoxControlTypes.Count; i++)
				this.AddToolBoxImage(ToolBoxControlTypes[i], imageCollection.Images[i + 1]);
		}
		public virtual ToolboxItem[] GetSchedulerToolboxItems() {
			List<ToolboxItem> result = new List<ToolboxItem>();
			for (int i = 0; i < ToolBoxControlTypes.Count; i++) {
				result.Add(new LocalizableToolboxItem(ToolBoxControlTypes[i]));
			}
			return result.ToArray();
		}
		protected override bool CanAddToolBoxItem(ToolboxItem toolboxItem) {
			return !FilteredToolBoxTypeNames.Contains(toolboxItem.TypeName);
		}
	}
	#endregion
}
