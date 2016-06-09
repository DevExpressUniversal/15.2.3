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

using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Internal {
	public interface IInternalSchedulerStorageBase {
		TimeZoneEngine TimeZoneEngine { get; set; }
		SchedulerStorageDeferredChanges DeferredChanges { get; }
		bool UnboundMode { get; }
		bool DesignModeProtected { get; }
		ReminderEngine RemindersEngine { get; }
		AppointmentMultiClientCache AppointmentCache { get; }
		bool AppointmentCacheEnabled { get; set; }
		void RaiseInternalResourceVisibilityChanged();
		int CalcTotalAppointmentCountForExchange();
		AppointmentBaseCollection GetNonFilteredAppointments(TimeInterval interval, object callerObject);
		bool RaiseFilterAppointment(Appointment apt);
		bool RaiseFilterDependency(AppointmentDependency dependency);
		bool RaiseFilterResource(Resource resource);
		ResourceBaseCollection GetNonFilteredResourcesCore();
		void SetAppointmentStorage(IAppointmentStorageBase appointmentStorage);
		void SetResourceStorage(IResourceStorageBase resourceStorage);
		bool IsDependencyValid(AppointmentDependency dependency);
		bool IsDependencyIntersectsInterval(AppointmentDependency dependency, TimeInterval visibleInternval);
		AppointmentDependencyBaseCollection FilterDependencies(AppointmentDependencyBaseCollection aptDependencies);
		void OnAppointmentsAutoReloading(object sender, CancelListChangedEventArgs e);
		void OnAppointmentInserting(object sender, PersistentObjectCancelEventArgs e);
		void RaiseAppointmentsInserted(AppointmentBaseCollection appointments);
		void OnAppointmentChanging(object sender, PersistentObjectCancelEventArgs e);
		void RaiseAppointmentsChanged(AppointmentBaseCollection appointments);
		void OnAppointmentDeleting(object sender, PersistentObjectCancelEventArgs e);
		void RaiseAppointmentsDeleted(AppointmentBaseCollection appointments);
		void OnResourceInserting(object sender, PersistentObjectCancelEventArgs e);
		void RaiseResourcesInserted(ResourceBaseCollection resources);
		void OnResourceChanging(object sender, PersistentObjectCancelEventArgs e);
		void RaiseResourcesChanged(ResourceBaseCollection resources);
		void OnResourceDeleting(object sender, PersistentObjectCancelEventArgs e);
		void RaiseResourcesDeleted(ResourceBaseCollection resources);
		void RaiseResourceCollectionLoaded();
		void RaiseResourceCollectionCleared();
		void OnResourcesAutoReloading(object sender, CancelListChangedEventArgs e);
		bool RaiseFetchAppointments(TimeInterval interval);
		void RaiseAfterFetchAppointments();
		void RaiseAppointmentCollectionLoaded();
		void RaiseAppointmentCollectionCleared();
		void OnRemindersAlerted(object sender, ReminderEventArgs e);
		Dictionary<TimeInterval, AppointmentBaseCollection> GetFilteredAppointmentByIntervals(TimeIntervalCollection searchIntervals, AppointmentResourcesMatchFilter filter, object callerObject);
		AppointmentBaseCollection GetNonRecurringAppointments();
		IPredicate<Appointment> CreateAppointmentExternalFilterPredicate();
		IList<DateTime> GetAppointmentDates(TimeInterval interval, object callerObject, AppointmentResourcesMatchFilter filter);
		IPredicate<Appointment> CreateAppointmentExternalFilter();
		IPredicate<OccurrenceInfo> CreateOccurrenceInfoExternalFilter();
		Dictionary<object, Appointment> FindNearestAppointmentInterval(TimeInterval searchInterval, AppointmentResourcesMatchFilter filter, bool findPrevApt);
		ResourceBaseCollection GetFilteredResources(bool useResourcesTreeFilter);
		ResourceBaseCollection GetVisibleResources(bool useResourcesTreeFilter);
		ResourceBaseCollection GetResourcesTree(bool useResourcesTreeFilter);
		AppointmentDependencyBaseCollection GetFilteredDependencies(TimeInterval interval, object callerObject);
		event EventHandler AfterFetchAppointments;
		event EventHandler InternalAppointmentCollectionCleared;
		event EventHandler InternalAppointmentCollectionLoaded;
		event PersistentObjectsEventHandler InternalAppointmentsInserted;
		event PersistentObjectsEventHandler InternalAppointmentsChanged;
		event PersistentObjectsEventHandler InternalAppointmentsDeleted;
		event EventHandler InternalAppointmentMappingsChanged;
		event EventHandler InternalAppointmentUIObjectsChanged;
		event EventHandler InternalResourceCollectionLoaded;
		event EventHandler InternalResourceCollectionCleared;
		event PersistentObjectsEventHandler InternalResourcesInserted;
		event PersistentObjectsEventHandler InternalResourcesChanged;
		event PersistentObjectsEventHandler InternalResourcesDeleted;
		event EventHandler InternalResourceMappingsChanged;
		event EventHandler InternalResourceVisibilityChanged;
		event EventHandler InternalResourceSortedColumnsChange;
		event EventHandler InternalDependencyCollectionLoaded;
		event EventHandler InternalDependencyCollectionCleared;
		event PersistentObjectsEventHandler InternalDependenciesInserted;
		event PersistentObjectsEventHandler InternalDependenciesChanged;
		event PersistentObjectsEventHandler InternalDependenciesDeleted;
		event EventHandler InternalDependencyMappingsChanged;
		event EventHandler InternalDependencyUIObjectsChanged;
		event EventHandler InternalDeferredNotifications;
		event EventHandler BeforeDispose;
		event EventHandler UpdateUI;
		event ReminderEventHandler InternalReminderAlert;
	}
}
