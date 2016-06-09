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

using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class DailyPrintStyle : PrintStyleWithResourceOptions {
		const string bitmapName = "dayonepage";
		TimeSlotCollection timeSlots;
		protected internal DailyPrintStyle(bool registerProperties, bool baseStyle)
			: base(registerProperties, baseStyle) {
		}
		public DailyPrintStyle(bool baseStyle)
			: this(true, baseStyle) {
		}
		public DailyPrintStyle()
			: this(true) {
		}
		#region TimeSlots
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewTimeSlots"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public TimeSlotCollection TimeSlots {
			get {
				if (timeSlots == null)
					timeSlots = CreateTimeSlotCollection();
				return timeSlots;
			}
		}
		public bool ShouldSerializeTimeSlots() {
			return !HasDefaultContent();
		}
		internal bool HasDefaultContent() {
			TimeSlotCollection defaultContent = CreateTimeSlotCollection();
			if (defaultContent.Count == 0)
				return false;
			int count = TimeSlots.Count;
			if (count != defaultContent.Count)
				return false;
			for (int i = 0; i < count; i++) {
				if (!TimeSlots[i].Equals(defaultContent[i]))
					return false;
			}
			return true;
		}
		protected virtual TimeSlotCollection CreateTimeSlotCollection() {
			TimeSlotCollection result = new TimeSlotCollection();
			result.Insert(0, new TimeSlot(TimeSpan.FromHours(2), String.Empty));
			result.Insert(0, new TimeSlot(TimeSpan.FromHours(4), String.Empty));
			return result;
		}
		#endregion
		public override SchedulerPrintStyleKind Kind { get { return SchedulerPrintStyleKind.Daily; } }
		#region properties and methods for UserInterfaceObject
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_DailyPrintStyle); } }
		protected override string GetStyleBitmapName() {
			return bitmapName;
		}
		#endregion
		#region PrintTime
		static readonly object printTimeProperty = new object();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public TimeOfDayInterval PrintTime {
			get { return (TimeOfDayInterval)GetPropertyValue(printTimeProperty); }
			set {
				TimeOfDayInterval printTime = PrintTime;
				if (Object.ReferenceEquals(printTime, value))
					return;
				SetPropertyValue(printTimeProperty, value.Clone());
			}
		}
		void PrintTimeValidationCallback(object property, object value) {
			TimeOfDayInterval interval = value as TimeOfDayInterval;
			if (interval == null)
				Exceptions.ThrowArgumentException("PrintTime", value);
			TimeOfDayIntervalValidators.ValidateMaxDuration("PrintTime", interval, TimeOfDayInterval.Day.Duration);
			TimeOfDayIntervalValidators.AttachIntervalMaxDurationValidator(interval, TimeOfDayInterval.Day.Duration);
		}
		bool ShouldSerializePrintTime() {
			return !PrintTime.IsEqual(TimeOfDayInterval.Day);
		}
		protected bool XtraShouldSerializePrintTime() {
			return !PrintTime.IsEqual(TimeOfDayInterval.Day);
		}
		void ResetPrintTime() {
			PrintTime = TimeOfDayInterval.Day;
		}
		#endregion
		#region PrintAllAppointments
		static readonly object printAllAppointmentsProperty = new object();
		[DefaultValue(true)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool PrintAllAppointments {
			get { return (bool)GetPropertyValue(printAllAppointmentsProperty); }
			set {
				SetPropertyValue(printAllAppointmentsProperty, value);
			}
		}
		#endregion
		#region UseActiveViewTimeScale
		static readonly object useActiveViewTimeScaleProperty = new object();
		[DefaultValue(false)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool UseActiveViewTimeScale {
			get { return (bool)GetPropertyValue(useActiveViewTimeScaleProperty); }
			set { SetPropertyValue(useActiveViewTimeScaleProperty, value); }
		}
		#endregion
		protected internal override void RegisterProperties() {
			base.RegisterProperties();
			TimeOfDayInterval dayInterval = (TimeOfDayInterval)TimeOfDayInterval.Day.Clone();
			RegisterProperty(printTimeProperty, PrintTimeValidationCallback, dayInterval);
			RegisterProperty(printAllAppointmentsProperty, true);
			RegisterProperty(useActiveViewTimeScaleProperty, false);
		}
		protected internal override SchedulerPrintStyle CreateInstance() {
			return new DailyPrintStyle(false, false);
		}
	}
}
