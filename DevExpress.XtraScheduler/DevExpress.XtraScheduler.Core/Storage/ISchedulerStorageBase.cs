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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Data;
using DevExpress.XtraScheduler.Exchange;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System.Runtime.InteropServices;
using DevExpress.Data.Helpers;
using System.Security;
using System.Diagnostics.Contracts;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler {
	public interface ISchedulerStorageBase : IBatchUpdateable, ISupportReminders, ISupportTimeZoneEngine, IDisposable, IDisposeState {
		[Browsable(false)]
		bool SupportsRecurrence { get; }
		[Browsable(false)]
		bool SupportsReminders { get; }
		[Browsable(false)]
		bool ResourceSharing { get; }
		[Browsable(false)]
		bool RemindersEnabled { get; }
		bool EnableReminders { get; set; }
		[Browsable(false)]
		bool UnboundMode { get; }
		string TimeZoneId { get; set; }
		bool EnableTimeZones { get; set; }
		[Browsable(false), DXBrowsable(false)]
		new bool IsUpdateLocked { get; }
		IAppointmentStorageBase Appointments { get; }
		IAppointmentDependencyStorage AppointmentDependencies { get; }
		IResourceStorageBase Resources { get; }
		int RemindersCheckInterval { get; set; }
		AppointmentBaseCollection GetAppointments(TimeInterval interval);
		AppointmentBaseCollection GetAppointments(TimeIntervalCollection intervals);
		AppointmentBaseCollection GetAppointments(DateTime start, DateTime end);
		AppointmentBaseCollection GetAppointmentsCore(TimeInterval interval, object callerObject);
		Resource GetResourceById(object resourceId);
		void ImportFromOutlook();
		void ExportToOutlook();
		void SynchronizeStorageWithOutlook(string outlookEntryIdFieldName);
		void SynchronizeOutlookWithStorage(string outlookEntryIdFieldName);
		AppointmentImporter CreateOutlookImporter();
		AppointmentExporter CreateOutlookExporter();
		AppointmentImportSynchronizer CreateOutlookImportSynchronizer();
		AppointmentExportSynchronizer CreateOutlookExportSynchronizer();
		void ImportFromVCalendar(string path);
		void ImportFromVCalendar(Stream stream);
		void ExportToVCalendar(string path);
		void ExportToVCalendar(Stream stream);
		void ImportFromICalendar(string path);
		void ImportFromICalendar(Stream stream);
		void ExportToICalendar(string path);
		void ExportToICalendar(Stream stream);
		void RegisterClient(object tcaller);
		void UnregisterClient(object tcaller);
		void RefreshData();
		Appointment CreateAppointment(AppointmentType type);
		Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end);
		Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration);
		Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end, string subject);
		Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration, string subject);
		Resource CreateResource(object resourceId);
		Resource CreateResource(object resourceId, string resourceCaption);
		AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId);
		AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId, AppointmentDependencyType type);
		Reminder CreateReminder(Appointment appointment);
		object GetObjectRow(IPersistentObject obj);
		object GetObjectValue(IPersistentObject obj, string columnName);
		void SetObjectValue(IPersistentObject obj, string columnName, object val);
		void SetAppointmentFactory(IAppointmentFactory appointmentFactory);
		void SetResourceFactory(IResourceFactory resourceFactory);
		void SetAppointmentDependencyFactory(IAppointmentDependencyFactory appointmentDependencyFactory);
		event PersistentObjectCancelEventHandler AppointmentInserting;
		event PersistentObjectsEventHandler AppointmentsInserted;
		event PersistentObjectCancelEventHandler AppointmentChanging;
		event PersistentObjectsEventHandler AppointmentsChanged;
		event PersistentObjectCancelEventHandler AppointmentDeleting;
		event PersistentObjectsEventHandler AppointmentsDeleted;
		event CancelListChangedEventHandler AppointmentCollectionAutoReloading;
		event EventHandler AppointmentCollectionCleared;
		event EventHandler AppointmentCollectionLoaded;
		event FetchAppointmentsEventHandler FetchAppointments;
		event PersistentObjectCancelEventHandler FilterAppointment;
		event PersistentObjectCancelEventHandler FilterResource;
		event PersistentObjectCancelEventHandler FilterDependency;
		event EventHandler<ReminderCancelEventArgs> FilterReminderAlert;
		event ReminderEventHandler ReminderAlert;
		event PersistentObjectCancelEventHandler ResourceChanging;
		event CancelListChangedEventHandler ResourceCollectionAutoReloading;
		event EventHandler ResourceCollectionCleared;
		event EventHandler ResourceCollectionLoaded;
		event PersistentObjectCancelEventHandler ResourceDeleting;
		event PersistentObjectCancelEventHandler ResourceInserting;
		event PersistentObjectsEventHandler ResourcesChanged;
		event PersistentObjectsEventHandler ResourcesDeleted;
		event PersistentObjectsEventHandler ResourcesInserted;
		event CancelListChangedEventHandler AppointmentDependencyCollectionAutoReloading;
		event EventHandler AppointmentDependencyCollectionCleared;
		event EventHandler AppointmentDependencyCollectionLoaded;
		event PersistentObjectCancelEventHandler AppointmentDependencyDeleting;
		event PersistentObjectCancelEventHandler AppointmentDependencyInserting;
		event PersistentObjectCancelEventHandler AppointmentDependencyChanging;
		event PersistentObjectsEventHandler AppointmentDependenciesChanged;
		event PersistentObjectsEventHandler AppointmentDependenciesDeleted;
		event PersistentObjectsEventHandler AppointmentDependenciesInserted;
	}
}
