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
using System.Text;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraScheduler.Reporting {
	public class ReportDayViewAppointmentDisplayOptions : DayViewAppointmentDisplayOptions {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ShowShadows {
			get { return false; }
			set {
			}
		}
		protected internal override AppointmentTimeDisplayType DefaultTimeDisplayType { get { return AppointmentTimeDisplayType.Text; } }
	}
	public class ReportAppointmentDisplayOptions : AppointmentDisplayOptions {
		protected internal override AppointmentTimeDisplayType DefaultTimeDisplayType { get { return AppointmentTimeDisplayType.Text; } }
	}
	public class ReportWeekViewAppointmentDisplayOptions : WeekViewAppointmentDisplayOptions {
		protected internal override AppointmentTimeDisplayType DefaultTimeDisplayType { get { return AppointmentTimeDisplayType.Text; } }
	}
	public class ReportMonthViewAppointmentDisplayOptions : MonthViewAppointmentDisplayOptions {
		protected internal override AppointmentTimeDisplayType DefaultTimeDisplayType { get { return AppointmentTimeDisplayType.Text; } }
	}
	#region PrintColorSchemaOptions
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PrintColorSchemaOptions : SchedulerNotificationOptions {
		public const PrintColorSchema DefaultPrintColorSchema = PrintColorSchema.Default;
		PrintColorSchema content = PrintColorSchema.Default;
		[DefaultValue(DefaultPrintColorSchema), NotifyParentProperty(true)]
		public PrintColorSchema Content { get { return content; } set { content = value; } } 
		protected internal override void ResetCore() {
			Content = PrintColorSchema.Default;
		}
	}
	#endregion
	#region TimeCellsPrintColorSchemaOptions
	public class TimeCellsPrintColorSchemaOptions : PrintColorSchemaOptions {
		PrintColorSchema appointment = PrintColorSchema.Default;
		[DefaultValue(PrintColorSchema.Default), NotifyParentProperty(true)]
		public PrintColorSchema Appointment { get { return appointment; } set { appointment = value; } }
		protected internal override void ResetCore() {
			base.ResetCore();
			Appointment = DefaultPrintColorSchema;
		}
	}
	#endregion
	#region DayViewTimeCellsPrintColorSchemaOptions
	public class DayViewTimeCellsPrintColorSchemaOptions: TimeCellsPrintColorSchemaOptions {
		PrintColorSchema allDayArea = PrintColorSchema.Default;
		PrintColorSchema allDayAppointmentStatus = PrintColorSchema.Default;
		public DayViewTimeCellsPrintColorSchemaOptions() {
			AllDayAppointmentStatus = DefaultPrintColorSchema;
		}
		[DefaultValue(PrintColorSchema.Default), NotifyParentProperty(true)]
		public PrintColorSchema AllDayArea { get { return allDayArea; } set { allDayArea = value; } }
		[DefaultValue(PrintColorSchema.Default), NotifyParentProperty(true)]
		public PrintColorSchema AllDayAppointmentStatus { get { return allDayAppointmentStatus; } set { allDayAppointmentStatus = value; } }
		protected internal override void ResetCore() {
			base.ResetCore();
			AllDayArea = DefaultPrintColorSchema;
		}
	}
	#endregion
	#region ExtraCellsOptions
	[DXToolboxItem(false)]
	public class ExtraCellsOptions : SchedulerNotificationOptions {
		#region Constants
		internal const bool DefaultVisible = true;
		internal const int DefaultMinCount = 3;
		#endregion
		#region Fields
		bool visible;
		int minCount;
		#endregion
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ExtraCellsOptionsVisible"),
#endif
XtraSerializableProperty(), DefaultValue(DefaultVisible)]
		public bool Visible { get { return visible; } set { if (visible == value) return; visible = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ExtraCellsOptionsMinCount"),
#endif
XtraSerializableProperty(), DefaultValue(DefaultMinCount)]
		public int MinCount { get { return minCount; } set { if (minCount == value) return; minCount = value; } }
		#endregion
		protected internal override void ResetCore() {
			this.visible = DefaultVisible;
			this.minCount = DefaultMinCount;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				ExtraCellsOptions cellsOptions = options as ExtraCellsOptions;
				if (cellsOptions == null)
					return;
				Visible = cellsOptions.Visible;
				MinCount = cellsOptions.MinCount;
			}
			finally {
				EndUpdate();
			}
		}
		public override string ToString() {
			return String.Empty;
		}
	}
	#endregion
	#region Corners
	[DXToolboxItem(false)]
	public class ControlCornersOptions : SchedulerNotificationOptions {
		#region Constants
		internal const int DefaultTop = 0;
		internal const int DefaultBottom = 0;
		#endregion
		#region Fields
		int top;
		int bottom;
		#endregion
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ControlCornersOptionsTop"),
#endif
XtraSerializableProperty(), DefaultValue(DefaultTop)]
		public int Top { get { return top; } set { if (top == value) return; top = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ControlCornersOptionsBottom"),
#endif
XtraSerializableProperty(), DefaultValue(DefaultBottom)]
		public int Bottom { get { return bottom; } set { if (bottom == value) return; bottom = value; } }
		#endregion
		protected internal override void ResetCore() {
			this.top = DefaultTop;
			this.bottom = DefaultBottom;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				ControlCornersOptions corners = options as ControlCornersOptions;
				if (corners == null)
					return;
				Top = corners.Top;
				Bottom = corners.Bottom;
			}
			finally {
				EndUpdate();
			}
		}
		public override string ToString() {
			return String.Empty;
		}
	}
	#endregion
	public class ReportResourceHeaderOptions : SchedulerResourceHeaderOptions {
		#region Height
		[
DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int Height {
			get { return 0; }
			set {				
			}
		}
		#endregion
	}
}
