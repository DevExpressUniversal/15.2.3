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
using System.IO;
using System.ComponentModel;
using DevExpress.XtraScheduler.Exchange;
using DevExpress.XtraScheduler.Outlook.Native;
using DevExpress.XtraScheduler.Outlook.Interop;
using System.Collections.Generic;
using System.Collections;
namespace DevExpress.XtraScheduler.Outlook {
	public interface IGetAppointmentForeignId {
		object GetForeignId(Appointment apt);
	}
	public interface ISupportCalendarFolders {
		string CalendarFolderName { get; set; }
	}
	[CLSCompliant(false)]
	public interface IOutlookCalendarProvider {
		_Application GetOutlookApplication();
		_Items GetCalendarItems(_Application app, string folderPath);
		List<_AppointmentItem> PrepareItemsForExchange(_Items items);
	}
	public class OutlookCalendarFolder {
		string name;
		string path;
		string fullPath;
		public OutlookCalendarFolder(string name, string path, string fullPath) {
			this.name = name;
			this.path = path;
			this.fullPath = path;
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("OutlookCalendarFolderName")]
#endif
		public string Name { get { return name; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("OutlookCalendarFolderPath")]
#endif
		public string Path { get { return path; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("OutlookCalendarFolderFullPath")]
#endif
		public string FullPath { get { return fullPath; } }
	}
	public static class OutlookExchangeHelper {
		public static string[] GetOutlookCalendarNames() {
			List<OutlookCalendarFolder> folders = GetOutlookCalendarFolders();
			List<string> names = new List<string>();
			for (int i = 0; i < folders.Count; i++) {
				names.Add(folders[i].Name);
			}
			return names.ToArray();
		}
		public static string[] GetOutlookCalendarPaths() {
			List<OutlookCalendarFolder> folders = GetOutlookCalendarFolders();
			List<string> names = new List<string>();
			for (int i = 0; i < folders.Count; i++) {
				names.Add(folders[i].Path);
			}
			return names.ToArray();
		}
		public static List<OutlookCalendarFolder> GetOutlookCalendarFolders() {
			return OutlookUtils.GetOutlookCalendarFolders();
		}
	}
	[CLSCompliant(false)]
	public class OutlookImport : AppointmentImporter, ISupportCalendarFolders {
		OutlookExchangeManager manager;
		string calendarFolderName = string.Empty;
		public OutlookImport(ISchedulerStorageBase storage)
			: base(storage) {
			this.manager = CreateExchangeManager();
		}
		#region Properties
		protected internal OutlookExchangeManager Manager { get { return manager; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("OutlookImportCalendarFolderName")]
#endif
		public string CalendarFolderName { get { return calendarFolderName; } set { calendarFolderName = value; } }
		#endregion
		public void SetCalendarProvider(IOutlookCalendarProvider provider) {
			Manager.CalendarProvider = provider;
		}
		protected internal virtual OutlookExchangeManager CreateExchangeManager() {
			return new ImportManager(this);
		}
		public override void Import(string path) {
			if (string.IsNullOrEmpty(path)) {
				base.Import(Stream.Null);
				return;
			}
			DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("path", path);
		}
		protected internal override void ImportCore(Stream stream) {
			try {
				Manager.Exchange();
			} catch (System.Exception e) {
				RaiseOnException(new ExchangeExceptionEventArgs(e));
			}
		}
		protected internal override int CalculateSourceObjectCount() {
			return Manager.CalculateSourceObjectCount();
		}
		protected internal virtual OutlookExchangeInfo CreateImportInfo(Appointment apt, _AppointmentItem olApt) {
			OutlookAppointmentImportingEventArgs args = new OutlookAppointmentImportingEventArgs(apt, olApt);
			RaiseOnAppointmentImporting(args);
			return new OutlookImportInfo(args.Cancel, IsTermination);
		}
		protected internal virtual void AfterImport(Appointment apt, _AppointmentItem olApt) {
			OutlookAppointmentImportedEventArgs args = new OutlookAppointmentImportedEventArgs(apt, olApt);
			RaiseOnAppointmentImported(args);
		}
		protected internal void HandleException(System.Exception e, Appointment apt, _AppointmentItem olApt) {
			RaiseOnException(new OutlookExchangeExceptionEventArgs(e, apt, olApt));
		}
	}
	[CLSCompliant(false)]
	public class OutlookExport : AppointmentExporter, ISupportCalendarFolders, IOutlookItemExchanger {
		OutlookExchangeManager manager;
		string calendarFolderName = string.Empty;
		public OutlookExport(ISchedulerStorageBase storage)
			: base(storage) {
			this.manager = CreateExchangeManager();
		}
		#region Properties
		protected internal OutlookExchangeManager Manager { get { return manager; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("OutlookExportCalendarFolderName")]
#endif
		public string CalendarFolderName { get { return calendarFolderName; } set { calendarFolderName = value; } }
		#endregion
		#region Events
		[Obsolete("You should use the 'AppointmentExported' instead.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event AppointmentExportedEventHandler OutlookAppointmentItemAdded { add { } remove { } }
		#endregion
		public void SetCalendarProvider(IOutlookCalendarProvider provider) {
			Manager.CalendarProvider = provider;
		}
		protected internal virtual OutlookExchangeManager CreateExchangeManager() {
			return new ExportManager(this);
		}
		protected internal override void ExportCore(Stream stream) {
			try {
				Manager.Exchange();
			} catch (System.Exception e) {
				RaiseOnException(new ExchangeExceptionEventArgs(e));
			}
		}
		#region IOutlookExportExchanger (implementation)
		OutlookExchangeInfo IOutlookItemExchanger.CreateExchangeInfo(Appointment apt, _AppointmentItem olApt, SynchronizeOperation defaultOperation) {
			return CreateExchangeInfo(apt, olApt, defaultOperation);
		}
		void IOutlookItemExchanger.AfterExchangeItem(Appointment apt, _AppointmentItem olApt) {
			AfterExchangeItem(apt, olApt);
		}
		void IOutlookItemExchanger.HandleException(System.Exception e, Appointment apt, _AppointmentItem olApt) {
			RaiseOnException(new OutlookExchangeExceptionEventArgs(e, apt, olApt));
		}
		#endregion
		protected virtual OutlookExchangeInfo CreateExchangeInfo(Appointment apt, _AppointmentItem olApt, SynchronizeOperation defaultOperation) {
			OutlookAppointmentExportingEventArgs args = new OutlookAppointmentExportingEventArgs(apt, olApt);
			RaiseOnAppointmentExporting(args);
			return new OutlookExportInfo(args.Cancel, IsTermination);
		}
		protected virtual void AfterExchangeItem(Appointment apt, _AppointmentItem olApt) {
			OutlookAppointmentExportedEventArgs args = new OutlookAppointmentExportedEventArgs(apt, olApt);
			RaiseOnAppointmentExported(args);
		}
		protected virtual void HandleException(System.Exception e, Appointment apt, _AppointmentItem olApt) {
			RaiseOnException(new OutlookExchangeExceptionEventArgs(e, apt, olApt));
		}
	}
	[CLSCompliant(false)]
	public class OutlookExportSynchronizer : AppointmentExportSynchronizer, IGetAppointmentForeignId, ISupportCalendarFolders, IOutlookItemExchanger {
		OutlookExchangeManager manager;
		string calendarFolderName = string.Empty;
		public OutlookExportSynchronizer(ISchedulerStorageBase storage)
			: base(storage) {
			this.manager = CreateExchangeManager();
		}
		#region Properties
		protected internal OutlookExchangeManager Manager { get { return manager; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("OutlookExportSynchronizerCalendarFolderName")]
#endif
		public string CalendarFolderName { get { return calendarFolderName; } set { calendarFolderName = value; } }
		#endregion
		protected internal virtual OutlookExchangeManager CreateExchangeManager() {
			return new ExportSynchronizeManager(this);
		}
		public void SetCalendarProvider(IOutlookCalendarProvider provider) {
			Manager.CalendarProvider = provider;
		}
		protected internal override void SynchronizeCore() {
			try {
				Manager.Exchange();
			} catch (System.Exception e) {
				RaiseOnException(new ExchangeExceptionEventArgs(e));
			}
		}
		#region IGetAppointmentForeignId implementation
		object IGetAppointmentForeignId.GetForeignId(Appointment apt) {
			return GetAppointmentForeignIdCore(apt);
		}
		#endregion
		protected virtual object GetAppointmentForeignIdCore(Appointment apt) {
			ExchangeAppointmentEventArgs args = new ExchangeAppointmentEventArgs(apt);
			args.Id = apt.GetValue(Storage, ForeignIdFieldName);
			RaiseGetAppointmentForeignId(args);
			return args.Id;
		}
		protected internal virtual void SetAppointmentForeignId(Appointment apt, _AppointmentItem olApt) {
			OutlookSyncHelper.SetAppointmentForeignId(apt, olApt, Storage, ForeignIdFieldName);
		}
		#region IOutlookExportExchanger (implementation)
		OutlookExchangeInfo IOutlookItemExchanger.CreateExchangeInfo(Appointment apt, _AppointmentItem olApt, SynchronizeOperation defaultOperation) {
			return CreateExchangeInfo(apt, olApt, defaultOperation);
		}
		void IOutlookItemExchanger.AfterExchangeItem(Appointment apt, _AppointmentItem olApt) {
			AfterExchangeItem(apt, olApt);
		}
		void IOutlookItemExchanger.HandleException(System.Exception e, Appointment apt, _AppointmentItem olApt) {
			HandleException(e, apt, olApt);
		}
		#endregion
		protected internal virtual OutlookExchangeInfo CreateExchangeInfo(Appointment apt, _AppointmentItem olApt, SynchronizeOperation defaultOperation) {
			OutlookAppointmentSynchronizingEventArgs args = OutlookSyncHelper.CreateSynchronizingEventArgs(apt, olApt, defaultOperation);
			RaiseOnAppointmentSynchronizing(args);
			return OutlookSyncHelper.CreateSynchronizeInfo(args, defaultOperation, IsTermination);
		}
		protected internal virtual void AfterExchangeItem(Appointment apt, _AppointmentItem olApt) {
			SetAppointmentForeignId(apt, olApt);
			RaiseOnAppointmentSynchronized(OutlookSyncHelper.CreateSynchronizedEventArgs(apt, olApt));
		}
		protected internal virtual void HandleException(System.Exception e, Appointment apt, _AppointmentItem olApt) {
			RaiseOnException(new OutlookExchangeExceptionEventArgs(e, apt, olApt));
		}
	}
	[CLSCompliant(false)]
	public class OutlookImportSynchronizer : AppointmentImportSynchronizer, IGetAppointmentForeignId, ISupportCalendarFolders {
		OutlookExchangeManager manager;
		string calendarFolderName = string.Empty;
		public OutlookImportSynchronizer(ISchedulerStorageBase storage)
			: base(storage) {
			this.manager = CreateExchangeManager();
		}
		#region Properties
		protected internal OutlookExchangeManager Manager { get { return manager; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("OutlookImportSynchronizerCalendarFolderName")]
#endif
		public string CalendarFolderName { get { return calendarFolderName; } set { calendarFolderName = value; } }
		#endregion
		protected internal virtual OutlookExchangeManager CreateExchangeManager() {
			return new ImportSynchronizeManager(this);
		}
		public void SetCalendarProvider(IOutlookCalendarProvider provider) {
			Manager.CalendarProvider = provider;
		}
		protected internal override void SynchronizeCore() {
			try {
				Manager.Exchange();
			} catch (System.Exception e) {
				RaiseOnException(new ExchangeExceptionEventArgs(e));
			}
		}
		protected internal override int CalculateSourceObjectCount() {
			return Manager.CalculateSourceObjectCount();
		}
		protected internal virtual OutlookSynchronizeInfo CreateSynchronizeInfo(Appointment apt, _AppointmentItem olApt, SynchronizeOperation defaultOperation) {
			OutlookAppointmentSynchronizingEventArgs args = OutlookSyncHelper.CreateSynchronizingEventArgs(apt, olApt, defaultOperation);
			RaiseOnAppointmentSynchronizing(args);
			return OutlookSyncHelper.CreateSynchronizeInfo(args, defaultOperation, IsTermination);
		}
		protected internal virtual void AfterSynchronize(Appointment apt, _AppointmentItem olApt) {
			SetAppointmentForeignId(apt, olApt);
			RaiseOnAppointmentSynchronized(OutlookSyncHelper.CreateSynchronizedEventArgs(apt, olApt));
		}
		protected internal void HandleException(System.Exception e, Appointment apt, _AppointmentItem olApt) {
			RaiseOnException(new OutlookExchangeExceptionEventArgs(e, apt, olApt));
		}
		#region IGetAppointmentForeignId implementation
		object IGetAppointmentForeignId.GetForeignId(Appointment apt) {
			return GetAppointmentForeignIdCore(apt);
		}
		#endregion
		protected virtual object GetAppointmentForeignIdCore(Appointment apt) {
			ExchangeAppointmentEventArgs args = new ExchangeAppointmentEventArgs(apt);
			args.Id = apt.GetValue(Storage, ForeignIdFieldName);
			RaiseGetAppointmentForeignId(args);
			return args.Id;
		}
		protected internal virtual void SetAppointmentForeignId(Appointment apt, _AppointmentItem olApt) {
			OutlookSyncHelper.SetAppointmentForeignId(apt, olApt, Storage, ForeignIdFieldName);
		}
	}
	[CLSCompliant(false)]
	public class OutlookCalendarProvider : IOutlookCalendarProvider {
		protected virtual _Application GetOutlookApplication() {
			return OutlookUtils.GetOutlookApplication();
		}
		protected virtual _Items GetCalendarItems(_Application app, string folderPath) {
			return OutlookUtils.GetCalendarItems(app, folderPath);
		}
		protected virtual List<_AppointmentItem> PrepareItemsForExchange(_Items items) {
			List<_AppointmentItem> result = new List<_AppointmentItem>();
			if (items == null)
				return result;
			IEnumerator en = items.GetEnumerator();
			while (en.MoveNext()) {
				_AppointmentItem item = en.Current as _AppointmentItem;
				if (item != null)
					result.Add(item);
			}
			return result;
		}
		#region IOutlookCalendarProvider Members
		_Application IOutlookCalendarProvider.GetOutlookApplication() {
			return GetOutlookApplication();
		}
		_Items IOutlookCalendarProvider.GetCalendarItems(_Application app, string folderPath) {
			return GetCalendarItems(app, folderPath);
		}
		List<_AppointmentItem> IOutlookCalendarProvider.PrepareItemsForExchange(_Items items) {
			return PrepareItemsForExchange(items);
		}
		#endregion
	}
}
