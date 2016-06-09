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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraScheduler {
	#region TimeSlot
	public class TimeSlot : UserInterfaceObject {
		TimeSpan val;
		public TimeSlot()
			: this(DateTimeHelper.FiveMinutesSpan, String.Empty) {
		}
		public TimeSlot(TimeSpan value, string displayName)
			: this(value, displayName, displayName) {
		}
		public TimeSlot(TimeSpan value, string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
			if (value.Ticks <= 0)
				Exceptions.ThrowArgumentException("value", value);
			this.val = value;
			ShouldSerializeHelper.RegisterXtraShouldSerializeMethod("Value", XtraShouldSerializeValue);
		}
		#region Properties
		#region Value
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeSlotValue"),
#endif
NotifyParentProperty(true), AutoFormatDisable(), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TimeSpan Value {
			get { return val; }
			set {
				if (val == value)
					return;
				TimeSpan oldValue = val;
				val = value;
				OnChanged("Value", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeValue() {
			return Value != DateTimeHelper.FiveMinutesSpan;
		}
		protected internal virtual bool XtraShouldSerializeValue() {
			return ShouldSerializeValue();
		}
		#endregion
		#endregion
		protected internal override void Assign(UserInterfaceObject source) {
			base.Assign(source);
			TimeSlot timeSlot = source as TimeSlot;
			if (timeSlot != null)
				Value = timeSlot.Value;
		}
		protected internal override bool ShouldSerializeDisplayName() {
			return DisplayName.Length != 0;
		}
		protected internal override bool ShouldSerializeMenuCaption() {
			return MenuCaption.Length != 0 && base.ShouldSerializeMenuCaption();
		}
		public override string ToString() {
			return String.Format("{0} ({1})", DisplayName, Value);
		}
		public override bool Equals(object obj) {
			if ( !(base.Equals(obj)) )
				return false;
			TimeSlot timeSlot = (TimeSlot)obj;
			return Value == timeSlot.Value;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
	#region TimeSlotComparer
	public class TimeSlotComparer : IComparer<TimeSlot>, IComparer {
		int IComparer.Compare(object x, object y) {
			TimeSlot valX = (TimeSlot)x;
			TimeSlot valY = (TimeSlot)y;
			return CompareCore(valX, valY);
		}
		public int Compare(TimeSlot x, TimeSlot y) {
			return CompareCore(x, y);
		}
		protected internal int CompareCore(TimeSlot x, TimeSlot y) {
			return -x.Value.CompareTo(y.Value);
		}
	}
	#endregion
	#region TimeSlotCollection
	[ListBindable(BindableSupport.No)]
	public class TimeSlotCollection : UserInterfaceObjectCollection<TimeSlot> {
		public TimeSlotCollection()
			: this(true) {
		}
		TimeSlotCollection(bool loadDefaults) {
			if (loadDefaults)
				LoadDefaults();
		}
		protected override TimeSlot GetItem(int index) {
			return InnerList[index];
		}
		public int Add(TimeSpan val, string displayName) {
			return Add(new TimeSlot(val, displayName));
		}
		public int Add(TimeSpan val, string displayName, string menuCaption) {
			return Add(new TimeSlot(val, displayName, menuCaption));
		}
		protected internal override UserInterfaceObjectCollection<TimeSlot> CreateDefaultContent() {
			TimeSlotCollection defaultContent = new TimeSlotCollection(false);
			defaultContent.Add(DateTimeHelper.HourSpan, SchedulerLocalizer.GetString(SchedulerStringId.Caption_60Minutes), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_60Minutes));
			defaultContent.Add(DateTimeHelper.HalfHourSpan, SchedulerLocalizer.GetString(SchedulerStringId.Caption_30Minutes), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_30Minutes));
			defaultContent.Add(DateTimeHelper.FifteenMinutesSpan, SchedulerLocalizer.GetString(SchedulerStringId.Caption_15Minutes), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_15Minutes));
			defaultContent.Add(DateTimeHelper.TenMinutesSpan, SchedulerLocalizer.GetString(SchedulerStringId.Caption_10Minutes), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_10Minutes));
			defaultContent.Add(DateTimeHelper.SixMinutesSpan, SchedulerLocalizer.GetString(SchedulerStringId.Caption_6Minutes), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_6Minutes));
			defaultContent.Add(DateTimeHelper.FiveMinutesSpan, SchedulerLocalizer.GetString(SchedulerStringId.Caption_5Minutes), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_5Minutes));
			return defaultContent;
		}
		protected internal override TimeSlot CreateItemInstance(object id, string displayName, string menuCaption) {
			return new TimeSlot(DateTimeHelper.FiveMinutesSpan, displayName, menuCaption);
		}
		protected internal override object ProvideDefaultId() {
			return null;
		}
		protected internal virtual void Assign(TimeSlotCollection source) {
			if (source == null)
				return;
			BeginUpdate();
			try {
				Clear();
				for (int i = 0; i < source.Count; i++) {
					Add(CloneItem(source[i]));
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override TimeSlot CloneItem(TimeSlot item) {
			TimeSlot result = new TimeSlot();
			result.Assign(item);
			return result;
		}
		public new TimeSlot CreateItem(object id, string displayName, string menuCaption) {
			return base.CreateItem(id, displayName, menuCaption);
		}
		[Obsolete("You should not use this method.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public TimeSpan GetMinValue() {
			TimeSpan result = TimeSpan.MaxValue;
			for (int i = 0; i < Count; i++)
				if (this[i].Value < result)
					result = this[i].Value;
			return result;
		}
		[Obsolete("You should not use this method.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public TimeSpan GetMaxValue() {
			TimeSpan result = TimeSpan.MinValue;
			for (int i = 0; i < Count; i++)
				if (this[i].Value > result)
					result = this[i].Value;
			return result;
		}
		protected internal virtual TimeSlot GetTimeSlotsMinValue() {
			return this[this.Count - 1];
		}
		protected internal virtual TimeSlot GetTimeSlotsMaxValue() {
			return this[0];
		}
		protected internal virtual TimeSpan FindNearestTimeSlotValue(TimeSpan val) {
			TimeSlot slot = FindNearestTimeSlot(val);
			if (slot != null)
				return slot.Value;
			else
				return val;
		}
		protected internal virtual TimeSlot FindNearestTimeSlot(TimeSpan val) {
			int index = FindNearestTimeSlotIndex(val);
			if (index >= 0)
				return this[index];
			else
				return null;
		}
		protected internal virtual int FindNearestTimeSlotIndex(TimeSpan val) {
			int count = this.Count;
			if (count <= 0)
				return -1;
			TimeSlot minSlot = GetTimeSlotsMinValue();
			if (val <= minSlot.Value)
				return count - 1;
			TimeSlot maxSlot = GetTimeSlotsMaxValue();
			if (val >= maxSlot.Value)
				return 0;
			long min = long.MaxValue;
			int result = 0;
			for (int i = 0; i < count; i++) {
				long testValue = Math.Abs(val.Ticks - this[i].Value.Ticks);
				if (testValue < min) {
					min = testValue;
					result = i;
				}
			}
			return result;
		}
	}
	#endregion
}
