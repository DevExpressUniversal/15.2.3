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
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.ComponentModel.Design;
using DevExpress.Services.Internal;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler.Services.Internal;
namespace DevExpress.XtraScheduler.Reporting {
	public interface ISchedulerPrintAdapterPropertiesBase : ISchedulerPropertiesBase, ISupportInitialize, IDisposable {
		TimeInterval TimeInterval { get; set; }
		TimeOfDayInterval WorkTime { get; set; }
		FirstDayOfWeek FirstDayOfWeek { get; set; }
		string ClientTimeZoneId { get; set; }
		bool EnableSmartSync { get; set; }
		ISmartSyncOptions SmartSyncOptions { get; }
		ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> ResourceColorSchemas { get; }
		TimeZoneHelper TimeZoneHelper { get; }
		void UpdateTimeZoneEngine(string tzId);
	}
	public class SchedulerPrintAdapterPropertiesBase : SchedulerPropertiesBase, ISchedulerPrintAdapterPropertiesBase {
		#region Fields
		NotificationTimeInterval timeInterval = NotificationTimeInterval.Empty;
		FirstDayOfWeek firstDayOfWeek = SchedulerPrintAdapter.DefaultFirstDayOfWeek;
		TimeOfDayInterval workTime = WorkTimeInterval.WorkTime;
		TimeZoneHelper timeZoneEngine;
		ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> resourceColorSchemas;
		ICollectionChangedListener resourceColorsListener;
		bool enableSmartSync;
		ISmartSyncOptions smartSyncOptions;
		#endregion
		public SchedulerPrintAdapterPropertiesBase() {
			this.timeZoneEngine = new TimeZoneHelper(new TimeZoneEngine());
			this.timeZoneEngine.ClientTimeZone = TimeZoneInfo.FindSystemTimeZoneById(SchedulerPrintAdapter.GetDefaultTimeZoneId());
			this.smartSyncOptions = CreateSmartSyncOptions();
			this.resourceColorSchemas = CreateResourceColorSchemas();
			this.resourceColorsListener = CreateResourceColorsListener();
			SubscribePropertiesEvents();
		}
		#region Properties
		protected internal TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } }
		protected internal ICollectionChangedListener ResourceColorsListener { get { return resourceColorsListener; } }
		#region TimeInterval
		public TimeInterval TimeInterval {
			get { return timeInterval; }
			set {
				if (value == null)
					value = SchedulerPrintAdapter.DefaultTimeInterval;
				if (object.Equals(TimeInterval, value))
					return;
				NotificationTimeInterval oldValue = this.timeInterval;
				NotificationTimeInterval newValue = new NotificationTimeInterval(value.Start, value.Duration);
				UnsubscribeTimeIntervalEvents();
				this.timeInterval = newValue;
				SubscribeTimeIntervalEvents();
				RaisePropertyChanged("TimeInterval", oldValue, newValue);
			}
		}
		#endregion
		#region WorkTime
		public TimeOfDayInterval WorkTime {
			get { return workTime; }
			set {
				if (value == null)
					value = SchedulerPrintAdapter.DefaultWorkTime;
				if (workTime.IsEqual(value))
					return;
				TimeOfDayInterval oldValue = workTime;
				TimeOfDayInterval newValue = new WorkTimeInterval(value.Start, value.End);
				UnsubscribeWorkTimeEvents();
				this.workTime = newValue;
				SubscribeWorkTimeEvents();
				RaisePropertyChanged("WorkTime", oldValue, newValue);
			}
		}
		#endregion
		#region FirstDayOfWeek
		public FirstDayOfWeek FirstDayOfWeek {
			get { return firstDayOfWeek; }
			set {
				if (firstDayOfWeek == value)
					return;
				FirstDayOfWeek oldValue = firstDayOfWeek;
				firstDayOfWeek = value;
				RaisePropertyChanged("FirstDayOfWeek", oldValue, value);
			}
		}
		#endregion
		#region ClientTimeZoneId
		public string ClientTimeZoneId {
			get { return TimeZoneHelper.ClientTimeZone.Id; }
			set {
				if (ClientTimeZoneId == value)
					return;
				string oldValue = TimeZoneHelper.ClientTimeZone.Id;
				UpdateTimeZoneEngineCore(value);
				RaisePropertyChanged("ClientTimeZoneId", oldValue, value);
			}
		}
		#endregion
		#region EnableSmartSync
		public bool EnableSmartSync {
			get { return enableSmartSync; }
			set {
				if (enableSmartSync == value)
					return;
				enableSmartSync = value;
				RaisePropertyChanged("EnableSmartSync", !value, value);
			}
		}
		#endregion
		public ISmartSyncOptions SmartSyncOptions { get { return smartSyncOptions; } }
		public ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> ResourceColorSchemas { get { return resourceColorSchemas; } }
		#endregion
		#region Events
		protected internal virtual void SubscribePropertiesEvents() {
			SubscribeTimeIntervalEvents();
			SubscribeWorkTimeEvents();
			SubscribeSmartSyncOptionsEvents();
			SubscribeResourceColorsEvents();
		}
		protected internal virtual void UnsubscribePropertiesEvents() {
			UnsubscribeSmartSyncOptionsEvents();
			UnsubscribeTimeIntervalEvents();
			UnsubscribeWorkTimeEvents();
			UnsubscribeResourceColorsEvents();
		}
		#region SubscribeTimeIntervalEvents
		protected internal virtual void SubscribeTimeIntervalEvents() {
			this.timeInterval.Changed += new EventHandler(OnTimeIntervalChanged);
		}
		#endregion
		#region UnsubscribeTimeIntervalEvents
		protected internal virtual void UnsubscribeTimeIntervalEvents() {
			this.timeInterval.Changed -= new EventHandler(OnTimeIntervalChanged);
		}
		#endregion
		protected internal virtual void SubscribeWorkTimeEvents() {
			workTime.Changed += new EventHandler(OnWorkTimeChanged);
		}
		protected internal virtual void UnsubscribeWorkTimeEvents() {
			workTime.Changed -= new EventHandler(OnWorkTimeChanged);
		}
		protected internal virtual void SubscribeSmartSyncOptionsEvents() {
			SmartSyncOptions.PropertyChanged += OnSmartSyncOptionsChanged;
		}
		protected internal virtual void UnsubscribeSmartSyncOptionsEvents() {
			SmartSyncOptions.PropertyChanged -= OnSmartSyncOptionsChanged;
		}
		protected internal virtual void SubscribeResourceColorsEvents() {
			resourceColorsListener.Changed += new EventHandler(OnResourceColorsChanged);
		}
		protected internal virtual void UnsubscribeResourceColorsEvents() {
			resourceColorsListener.Changed -= new EventHandler(OnResourceColorsChanged);
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribePropertiesEvents();
				if (this.timeZoneEngine != null) {
					this.timeZoneEngine = null;
				}
				if (this.smartSyncOptions != null) {
					this.smartSyncOptions = null;
				}
				if (this.resourceColorsListener != null) {
					this.resourceColorsListener.Dispose();
					this.resourceColorsListener = null;
				}
			}
			base.Dispose(disposing);
		}
		#region ISchedulerPrintAdapterPropertiesBase
		void ISchedulerPrintAdapterPropertiesBase.UpdateTimeZoneEngine(string tzId) {
			UpdateTimeZoneEngineCore(tzId);
		}
		TimeZoneHelper ISchedulerPrintAdapterPropertiesBase.TimeZoneHelper { get { return TimeZoneHelper; } }
		#endregion
		protected virtual ISmartSyncOptions CreateSmartSyncOptions() {
			return new SmartSyncOptions();
		}
		#region ISupportInitialize Members
		void ISupportInitialize.BeginInit() {
			BeginInit();
		}
		void ISupportInitialize.EndInit() {
			EndInit();
		}
		#endregion
		protected virtual ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> CreateResourceColorSchemas() {
			return new SchedulerColorSchemaCollectionBase<SchedulerColorSchemaBase>();
		}
		protected virtual ICollectionChangedListener CreateResourceColorsListener() {
			return new ResourceColorSchemasChangedListenerCore(resourceColorSchemas);
		}
		protected internal virtual void BeginInit() {
			UnsubscribeResourceColorsEvents();
			ResourceColorSchemas.Clear();
		}
		protected internal virtual void EndInit() {
			if (ResourceColorSchemas.Count <= 0)
				ResourceColorSchemas.LoadDefaults();
			SubscribeResourceColorsEvents();
		}
		protected virtual void UpdateTimeZoneEngineCore(string tzId) {
			this.timeZoneEngine.ClientTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
		}
		protected internal virtual void OnTimeIntervalChanged(object sender, EventArgs e) {
			RaisePropertyChanged("TimeInterval", TimeInterval, TimeInterval);
		}
		protected internal virtual void OnWorkTimeChanged(object sender, EventArgs e) {
			RaisePropertyChanged("WorkTime", WorkTime, WorkTime);
		}
		protected internal virtual void OnSmartSyncOptionsChanged(object sender, PropertyChangedEventArgs e) {
			RaisePropertyChanged("SmartSyncOptions", SmartSyncOptions, SmartSyncOptions);
		}
		protected internal virtual void OnResourceColorsChanged(object sender, EventArgs args) {
			RaisePropertyChanged("ResourceColorSchemas", ResourceColorSchemas, ResourceColorSchemas);
		}
	}
}
