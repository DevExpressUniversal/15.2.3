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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using DevExpress.XtraScheduler;
using System.ComponentModel;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.UI;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Data;
namespace DevExpress.Xpf.Scheduler.UI {
	public interface IValidatableControl {
		void ValidateValues(ValidationArgs args);
		void CheckForWarnings(ValidationArgs args);
	}
	public class WeekOfMonthTypeTemplateSelector : DataTemplateSelector {
		public DataTemplate WeekOfMonthTemplate { get; set; }
		public DataTemplate DayNumberTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			NamedElement el = item as NamedElement;
			if (el == null)
				return null;
			WeekOfMonth range = (WeekOfMonth)el.Id;
			switch (range) {
				case WeekOfMonth.None:
					return DayNumberTemplate;
				default:
					return WeekOfMonthTemplate;
			}
		}
	}
	public class ResourcesTemplateSelector : DataTemplateSelector {
		public DataTemplate ResourceTemplate { get; set; }
		public DataTemplate ResourcesTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			AppointmentFormController controller = item as AppointmentFormController;
			if (controller == null)
				return null;
			return controller.ResourceSharing ? ResourcesTemplate : ResourceTemplate;
		}
	}
	public class RecurrenceTypeElement {
		RecurrenceType? recurrenceType;
		string description;
		public RecurrenceTypeElement(RecurrenceType? recurrenceType) {
			this.recurrenceType = recurrenceType;
			this.description = CalculateDescription(recurrenceType);
		}
		public RecurrenceType? RecurrenceType { get { return recurrenceType; } }
		public string Description { get { return description; } }
		string CalculateDescription(RecurrenceType? recurrenceType) {
			if (!recurrenceType.HasValue)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_NoneRecurrence);
			switch (recurrenceType.Value) {
				case XtraScheduler.RecurrenceType.Daily:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeDaily);
				case XtraScheduler.RecurrenceType.Hourly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeHourly);
				case XtraScheduler.RecurrenceType.Minutely:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeMinutely);
				case XtraScheduler.RecurrenceType.Monthly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeMonthly);
				case XtraScheduler.RecurrenceType.Weekly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeWeekly);
				case XtraScheduler.RecurrenceType.Yearly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeYearly);
			}
			return String.Empty;
		}
		public override string ToString() {
			return Description;
		}
	}
}
