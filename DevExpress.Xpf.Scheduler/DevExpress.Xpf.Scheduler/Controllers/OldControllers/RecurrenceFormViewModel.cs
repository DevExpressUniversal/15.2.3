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
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Xpf.Scheduler.UI {
	public class StandaloneRecurrenceVisualController : RecurrenceVisualController {
		public StandaloneRecurrenceVisualController(AppointmentFormController controller)
			: base(controller) {
		}
		public StandaloneRecurrenceVisualController(AppointmentFormController controller, XtraScheduler.RecurrenceType[] availableRecurrenceTypes)
			: base(controller, availableRecurrenceTypes) {
		}
		protected override void InitializeRecurrenceType(AppointmentFormController controller) {
			EnableRecurrence = true;
			RecurrenceType = controller.PatternCopy.RecurrenceInfo.Type;
		}
	}
	public class RecurrenceVisualController : DependencyObject, INotifyPropertyChanged {
		internal static RecurrenceTypeElement RecurrenceNone = new RecurrenceTypeElement(null);
		internal static readonly RecurrenceType[] RecurrenceTypeOrder = new RecurrenceType[] {XtraScheduler.RecurrenceType.Minutely, 
																			XtraScheduler.RecurrenceType.Hourly, 
																			XtraScheduler.RecurrenceType.Daily, 
																			XtraScheduler.RecurrenceType.Weekly,
																			XtraScheduler.RecurrenceType.Monthly,
																			XtraScheduler.RecurrenceType.Yearly
																			};
		internal static RecurrenceType[] AvailableRecurrenceTypesByDefault = new RecurrenceType[] { XtraScheduler.RecurrenceType.Daily, 
																							XtraScheduler.RecurrenceType.Weekly,
																							XtraScheduler.RecurrenceType.Monthly,
																							XtraScheduler.RecurrenceType.Yearly};
		List<RecurrenceTypeElement> recurrenceElements;
		List<IRecurrenceInfo> recurrenceInfos;
		RecurrenceType? recurrenceType;
		List<RecurrenceType> availableRecurrenceTypes;
		AppointmentFormController controller;
		bool enableNoneRecurrence = true;
		bool enableRecurrence = false;
		bool internalUpdate = false;
		public RecurrenceVisualController(AppointmentFormController controller)
			: this(controller, AvailableRecurrenceTypesByDefault) {
		}
		public RecurrenceVisualController(AppointmentFormController controller, XtraScheduler.RecurrenceType[] availableRecurrenceTypes) {
			this.availableRecurrenceTypes = new List<RecurrenceType>();
			this.availableRecurrenceTypes.AddRange(availableRecurrenceTypes);
			this.controller = controller;
			RecreateSources();
			InitializeRecurrenceType(controller);
			Binding srcBinding = InnerBindingHelper.CreateOneWayPropertyBinding(controller, "Start");
			BindingOperations.SetBinding(this, StartProperty, srcBinding);
		}
		#region Start
		public DateTime Start { get { return (DateTime)GetValue(StartProperty); } set { SetValue(StartProperty, value); } }
		public static readonly DependencyProperty StartProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceVisualController, DateTime>("Start", DateTime.MinValue, (d, e) => d.OnStartChanged(e.OldValue, e.NewValue));
		private void OnStartChanged(DateTime oldStart, DateTime newStart) {
			SynchronizeRecurrenceInfoStart(Controller.TimeZoneHelper.FromClientTime(newStart, Controller.TimeZoneId));
		}
		protected virtual void SynchronizeRecurrenceInfoStart(DateTime start) {
			if(RecurrenceInfo != null && RecurrenceInfo.Start != start) {
				RecurrenceInfo.Start = Start;
			}
		}
		#endregion
		public bool ShouldShowRecurrence { get { return !Controller.SourceAppointment.IsOccurrence && Controller.ShouldShowRecurrenceButton; } }
		public bool EnableRecurrence {
			get { return enableRecurrence; }
			set {
				if(enableRecurrence == value)
					return;
				BeginInternalUpdate();
				enableRecurrence = value;
				if(value == true) {
					if(!RecurrenceType.HasValue) {
						foreach(RecurrenceType type in RecurrenceTypeOrder)
							if(AvailableRecurrenceTypes.Contains(type)) {
								RecurrenceType = type;
								break;
							}
					}
				}
				SyncRecurrenceTypeWithOtherProperties();
				RaisePropertiesUpdated();
				EndInternalUpdate();
			}
		}
		public AppointmentFormController Controller { get { return controller; } }
		List<RecurrenceType> AvailableRecurrenceTypes { get { return availableRecurrenceTypes; } }
		public List<RecurrenceTypeElement> RecurrenceElements { get { return recurrenceElements; } }
		#region RecurrenceElement
		public RecurrenceTypeElement RecurrenceElement {
			get { return (RecurrenceTypeElement)GetValue(RecurrenceElementProperty); }
			set { SetValue(RecurrenceElementProperty, value); }
		}
		public static readonly DependencyProperty RecurrenceElementProperty = CreateElementProperty();
		static DependencyProperty CreateElementProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceVisualController, RecurrenceTypeElement>("RecurrenceElement", null, (d, e) => d.OnRecurrenceElementChanged(e.OldValue, e.NewValue), null);
		}
		void OnRecurrenceElementChanged(RecurrenceTypeElement oldValue, RecurrenceTypeElement newValue) {
			if(InternalUpdate)
				return;
			if(newValue != null && newValue.RecurrenceType.HasValue && AvailableRecurrenceTypes.Contains(newValue.RecurrenceType.Value)) {
				this.enableRecurrence = true;
				RecurrenceType = newValue.RecurrenceType.Value;
			} else {
				this.enableRecurrence = false;
				RecurrenceType = null;
			}
			SynchronizeRecurrenceInfoStart(controller.TimeZoneHelper.FromClientTime(Start, controller.TimeZoneId));
			RaisePropertiesUpdated();
		}
		#endregion
		public List<IRecurrenceInfo> RecurrenceInfos { get { return recurrenceInfos; } }
		public RecurrenceInfo RecurrenceInfo {
			get {
				if(!RecurrenceType.HasValue || !EnableRecurrence)
					return null;
				foreach(RecurrenceInfo item in RecurrenceInfos)
					if(item.Type == RecurrenceType.Value)
						return item;
				return null;
			}
		}
		public RecurrenceType? RecurrenceType {
			get { return recurrenceType; }
			set {
				recurrenceType = value;
				SyncRecurrenceTypeWithOtherProperties();
				RaisePropertiesUpdated();
			}
		}
		public bool IsDailyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Daily; } }
		public bool IsWeeklyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Weekly; } }
		public bool IsMonthlyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Monthly; } }
		public bool IsYearlyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Yearly; } }
		public bool IsHourlyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Hourly; } }
		public bool IsMinutelyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Minutely; } }
		public bool IsRecurrence { get { return RecurrenceType.HasValue; } }
		public bool EnableDailyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Daily); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Daily, value); }
		}
		public bool EnableWeeklyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Weekly); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Weekly, value); }
		}
		public bool EnableMonthlyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Monthly); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Monthly, value); }
		}
		public bool EnableYearlyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Yearly); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Yearly, value); }
		}
		public bool EnableHourlyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Hourly); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Hourly, value); }
		}
		public bool EnableMinutelyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Minutely); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Minutely, value); }
		}
		public bool EnableNoneRecurrence {
			get { return enableNoneRecurrence; }
			set {
				if(EnableNoneRecurrence == value)
					return;
				BeginInternalUpdate();
				enableNoneRecurrence = value;
				RecreateSources();
				SyncRecurrenceTypeWithOtherProperties();
				RaisePropertiesUpdated();
				EndInternalUpdate();
			}
		}
		bool InternalUpdate {
			get { return internalUpdate; }
		}
		protected virtual void InitializeRecurrenceType(AppointmentFormController controller) {
			if(controller.PatternRecurrenceInfo != null) {
				EnableRecurrence = ShouldShowRecurrence;
				RecurrenceType = controller.PatternRecurrenceInfo.Type;
			} else {
				EnableRecurrence = false;
				RecurrenceType = null;
			}
		}
		void BeginInternalUpdate() {
			this.internalUpdate = true;
		}
		void EndInternalUpdate() {
			this.internalUpdate = false;
		}
		void EnableRecurrenceType(RecurrenceType recurrenceType, bool value) {
			BeginInternalUpdate();
			if(AvailableRecurrenceTypes.Contains(recurrenceType) == value)
				return;
			if(value == true)
				AvailableRecurrenceTypes.Add(recurrenceType);
			else
				AvailableRecurrenceTypes.Remove(recurrenceType);
			RecreateSources();
			SyncRecurrenceTypeWithOtherProperties();
			RaisePropertiesUpdated();
			EndInternalUpdate();
		}
		void RecreateSources() {
			this.recurrenceElements = CreateRecurrenceElements();
			this.recurrenceInfos = CreateRecurrenceInfos(Controller.PatternCopy);
			RaiseSourcePropertiesUpdated();
		}
		void SyncRecurrenceTypeWithOtherProperties() {
			RecurrenceTypeElement result = RecurrenceNone;
			if(!EnableNoneRecurrence)
				result = RecurrenceElements[0];
			if(RecurrenceType.HasValue && EnableRecurrence) {
				foreach(RecurrenceTypeElement item in RecurrenceElements) {
					if(item.RecurrenceType == RecurrenceType.Value) {
						result = item;
						break;
					}
				}
			}
			RecurrenceElement = result;
		}
		void RaisePropertiesUpdated() {
			RaiseOnPropertyChanged("IsDailyRecurrence");
			RaiseOnPropertyChanged("IsWeeklyRecurrence");
			RaiseOnPropertyChanged("IsMonthlyRecurrence");
			RaiseOnPropertyChanged("IsYearlyRecurrence");
			RaiseOnPropertyChanged("IsRecurrence");
			RaiseOnPropertyChanged("RecurrenceInfo");
			RaiseOnPropertyChanged("EnableRecurrence");
		}
		void RaiseSourcePropertiesUpdated() {
			RaiseOnPropertyChanged("RecurrenceElements");
			RaiseOnPropertyChanged("RecurrenceInfos");
		}
		List<RecurrenceTypeElement> CreateRecurrenceElements() {
			List<RecurrenceTypeElement> elements = new List<RecurrenceTypeElement>();
			if(EnableNoneRecurrence)
				elements.Add(RecurrenceNone);
			int count = RecurrenceTypeOrder.Length;
			for(int i = 0; i < count; i++) {
				RecurrenceType currentType = RecurrenceTypeOrder[i];
				if(AvailableRecurrenceTypes.Contains(currentType))
					elements.Add(new RecurrenceTypeElement(currentType));
			}
			return elements;
		}
		List<IRecurrenceInfo> CreateRecurrenceInfos(Appointment patternCopy) {
			List<IRecurrenceInfo> result = new List<IRecurrenceInfo>();
			int count = RecurrenceTypeOrder.Length;
			for(int i = 0; i < count; i++) {
				RecurrenceType currentType = RecurrenceTypeOrder[i];
				if(AvailableRecurrenceTypes.Contains(currentType))
					result.Add(CreateRecurrenceInfo(patternCopy, currentType));
			}
			return result;
		}
		protected IRecurrenceInfo CreateRecurrenceInfo(Appointment patternCopy, RecurrenceType type) {
			IRecurrenceInfo patternRecurrenceInfo = patternCopy.RecurrenceInfo;
			IRecurrenceInfo ri = new RecurrenceInfo();
			if(patternRecurrenceInfo != null)
				ri.Assign(patternRecurrenceInfo);
			ri.Type = type;
			((IInternalRecurrenceInfo)ri).UpdateRange(ri.Start, ri.End, ri.Range, ri.OccurrenceCount, patternCopy);
			return ri;
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected void RaiseOnPropertyChanged(string propertyName) {
			if(onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		public void ApplyRecurrence() {
			if(RecurrenceInfo != null) {
				Appointment patternCopy = Controller.PatternCopy;
				patternCopy.Start = Controller.TimeZoneHelper.FromClientTime(Controller.Start, Controller.TimeZoneId);
				patternCopy.End = Controller.TimeZoneHelper.FromClientTime(Controller.End, controller.TimeZoneId);
				patternCopy.RecurrenceInfo.Assign(RecurrenceInfo);
				Controller.ApplyRecurrence(patternCopy);
			} else
				Controller.RemoveRecurrence();
		}
		bool CalcCanChangeAllDay() {
			return !RecurrenceType.HasValue;
		}
	}
}
