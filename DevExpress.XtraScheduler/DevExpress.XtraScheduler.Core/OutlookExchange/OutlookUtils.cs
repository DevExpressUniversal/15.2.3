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
using System.Security;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Outlook.Interop;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Outlook.Native {
	public static class SRNames {
		public const string OutlookProgId = "Outlook.Application";
		public const string OutlookNamespaceType = "MAPI";
		public const string AppointmentSetTypeMethod = "SetTypeCore";
	}
	#region OutlookUtils
	[CLSCompliant(false)]
	public static class OutlookUtils {
		const string RootFolderPrefix = @"\\";
		const string PathSeparator = "\\";
		#region static data
		static Hashtable olBusyStatusHash = new Hashtable();
		static Hashtable appointmentStatusTypeHash = new Hashtable();
		static OutlookUtils() {
			olBusyStatusHash[AppointmentStatusType.Busy] = OlBusyStatus.olBusy;
			olBusyStatusHash[AppointmentStatusType.Free] = OlBusyStatus.olFree;
			olBusyStatusHash[AppointmentStatusType.OutOfOffice] = OlBusyStatus.olOutOfOffice;
			olBusyStatusHash[AppointmentStatusType.Tentative] = OlBusyStatus.olTentative;
			olBusyStatusHash[AppointmentStatusType.WorkingElsewhere] = OlBusyStatus.olWorkingElsewhere;
			olBusyStatusHash[AppointmentStatusType.Custom] = OlBusyStatus.olFree;
			appointmentStatusTypeHash[OlBusyStatus.olBusy] = AppointmentStatusType.Busy;
			appointmentStatusTypeHash[OlBusyStatus.olFree] = AppointmentStatusType.Free;
			appointmentStatusTypeHash[OlBusyStatus.olOutOfOffice] = AppointmentStatusType.OutOfOffice;
			appointmentStatusTypeHash[OlBusyStatus.olTentative] = AppointmentStatusType.Tentative;
			appointmentStatusTypeHash[OlBusyStatus.olWorkingElsewhere] = AppointmentStatusType.WorkingElsewhere;
		}
		#endregion
		[SecuritySafeCritical]
		public static void ReleaseOutlookObject(object obj) {
			if (obj != null) Marshal.ReleaseComObject(obj);
		}
		public static OlBusyStatus ConvertToOlBusyStatus(AppointmentStatusType status) {
			return (OlBusyStatus)olBusyStatusHash[status];
		}
		public static AppointmentStatusType ConvertToAppointmentStatus(OlBusyStatus status) {
			return (AppointmentStatusType)appointmentStatusTypeHash[status];
		}
		public static string GetRecurrenceEntryId(string entryId, int recurrenceIndex) {
			return entryId + recurrenceIndex.ToString().Trim();
		}
		public static DateTime CalculateOccurrenceStart(IRecurrenceInfo rinfo, int recurrenceIndex) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(rinfo);
			return calc.CalcOccurrenceStartTime(recurrenceIndex);
		}
		public static void SetAppointmentType(Appointment apt, AppointmentType type) {
			((IInternalAppointment)apt).SetTypeCore(type);
		}
		[SecuritySafeCritical]
		public static _Application GetOutlookApplication() {
			_Application app = GetRunningOutlookApplication();
			if (app != null)
				return app;
			Type type = Type.GetTypeFromProgID(SRNames.OutlookProgId);
			return Activator.CreateInstance(type) as _Application;
		}
		[SecuritySafeCritical]
		internal static _Application GetRunningOutlookApplication() {
			try {
				object appInterface = Marshal.GetActiveObject(SRNames.OutlookProgId);
				_Application app = appInterface as _Application;
				return app;
			}
			catch {
				return null;
			}
		}
		static bool FolderIsCalendar(MAPIFolder folder) {
			try {
				return folder.DefaultItemType == OlItemType.olAppointmentItem;
			}
			catch {
				return false; 
			}
		}
		static bool IsFolderPathValid(MAPIFolder folder, string path) {
			string folderStr = ShouldCompareFolderByFullPath(path) ? folder.FullFolderPath : folder.Name;
			return (String.Compare(path, folderStr, true) == 0);
		}
		static bool ShouldCompareFolderByFullPath(string path) {
			return path.Contains(PathSeparator);
		}
		static MAPIFolder GetSubFolderByName(_Folders oFolders, string name) {
			if (oFolders == null)
				return null;
			try {
				return oFolders[name];
			}
			catch {
				return null;
			}
		}
		static _Folders GetNamespaceFolders(_NameSpace olNamespace) {
			try {
				return olNamespace.Folders;
			}
			catch {
				return null;
			}
		}
		static MAPIFolder GetRootSubFolderByName(_NameSpace olNamespace, string name) {
			try {
				_Folders oFolders = GetNamespaceFolders(olNamespace);
				if (oFolders == null)
					return null;
				try {
					return oFolders[name];
				}
				finally {
					ReleaseOutlookObject(oFolders);
					oFolders = null;
				}
			}
			catch {
				return null;
			}
		}
		public static List<OutlookCalendarFolder> GetOutlookCalendarFolders() {
			List<OutlookCalendarFolder> result = new List<OutlookCalendarFolder>();
			_Application app = GetOutlookApplication();
			try {
				if (app == null) return result;
				_NameSpace olNamespace = app.GetNamespace(SRNames.OutlookNamespaceType);
				try {
					PopulateCalendarFolders(olNamespace, result);
					return result;
				}
				finally {
					ReleaseOutlookObject(olNamespace);
					olNamespace = null;
				}
			}
			finally {
				ReleaseOutlookObject(app);
				app = null;
			}
		}
		private static void PopulateCalendarFolders(_NameSpace olNamespace, List<OutlookCalendarFolder> result) {
			if (olNamespace == null || result == null)
				return;
			PopulateNamespaceCalendarFolders(olNamespace, result);
			if (result.Count == 0) {
				PopulateTypedCalendarFolders(olNamespace, OlDefaultFolders.olFolderCalendar, result);
				PopulateTypedCalendarFolders(olNamespace, OlDefaultFolders.olPublicFoldersAllPublicFolders, result);
			}
		}
		static void PopulateTypedCalendarFolders(_NameSpace olNamespace, OlDefaultFolders folderType, List<OutlookCalendarFolder> result) {
			MAPIFolder folder = GetTypedCalendarFolder(olNamespace, folderType);
			PopulateCalendarsInfoByFolder(folder, result);
		}
		static MAPIFolder GetTypedCalendarFolder(_NameSpace olNamespace, OlDefaultFolders folderType) {
			try {
				return olNamespace.GetDefaultFolder(folderType);
			}
			catch {
				return null;
			}
		}
		static void PopulateNamespaceCalendarFolders(_NameSpace olNamespace, List<OutlookCalendarFolder> result) {
			_Folders olFolders = GetNamespaceFolders(olNamespace);
			if (olFolders == null)
				return;
			int count = olFolders.Count;
			for (int i = 1; i <= count; i++) {
				MAPIFolder folder = GetRootSubFolderByName(olNamespace, olNamespace.Folders[i].Name);
				PopulateCalendarsInfoByFolder(folder, result);
			}
		}
		static bool CanAddCalendarFolder(List<OutlookCalendarFolder> destination, MAPIFolder folder) {
			if (folder != null && destination != null) {
				for (int i = 0; i < destination.Count; i++) {
					if (destination[i].FullPath == folder.FullFolderPath)
						return false;
				}
			}
			return true;
		}
		static void PopulateCalendarsInfoByFolder(MAPIFolder folder, List<OutlookCalendarFolder> result) {
			if (folder == null)
				return;
			try {
				if (FolderIsCalendar(folder) && CanAddCalendarFolder(result, folder))
					result.Add(CreateCalendarInfo(folder));
				try {
					PopulateCalendarsInfoRecursive(folder.Folders, result);
				}
				catch { 
					;
				}
			}
			finally {
				ReleaseOutlookObject(folder);
				folder = null;
			}
		}
		static void PopulateCalendarsInfoRecursive(_Folders folders, List<OutlookCalendarFolder> info) {
			if (folders == null || info == null)
				return;
			try {
				foreach (MAPIFolder folder in folders) {
					try {
						if (FolderIsCalendar(folder) && CanAddCalendarFolder(info, folder))
							info.Add(CreateCalendarInfo(folder));
						try {
							PopulateCalendarsInfoRecursive(folder.Folders, info);
						}
						catch {
							; 
						}
					}
					finally {
						ReleaseOutlookObject(folder);
					}
				}
			}
			finally {
				ReleaseOutlookObject(folders);
				folders = null;
			}
		}
		static OutlookCalendarFolder CreateCalendarInfo(MAPIFolder folder) {
			return new OutlookCalendarFolder(folder.Name, folder.FolderPath, folder.FullFolderPath);
		}
		public static _Items GetCalendarItems(_Application app, string folderPath) {
			_NameSpace olNamespace = app.GetNamespace(SRNames.OutlookNamespaceType);
			try {
				_Items items = GetDefaultCalendarItems(olNamespace, folderPath);
				if (items != null) return items;
				items = GetNamespaceCalendarItems(olNamespace, folderPath);
				if (items != null) return items;
				items = GetAllPublicCalendarItems(olNamespace, folderPath);
				if (items != null) return items;
				throw new ArgumentException(String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Msg_OutlookCalendarNotFound), folderPath));
			}
			finally {
				ReleaseOutlookObject(olNamespace);
				olNamespace = null;
			}
		}
		static _Items GetDefaultCalendarItems(_NameSpace olNamespace, string searchFolderPath) {
			return GetCalendarItemsCore(searchFolderPath, GetTypedCalendarFolder(olNamespace, OlDefaultFolders.olFolderCalendar));
		}
		static _Items GetAllPublicCalendarItems(_NameSpace olNamespace, string searchFolderPath) {
			return GetCalendarItemsCore(searchFolderPath, GetTypedCalendarFolder(olNamespace, OlDefaultFolders.olPublicFoldersAllPublicFolders));
		}
		static _Items GetCalendarItemsCore(string searchFolderPath, MAPIFolder folder) {
			if (folder == null)
				return null;
			try {
				if (string.IsNullOrEmpty(searchFolderPath) || IsFolderPathValid(folder, searchFolderPath))
					return folder.Items;
				else {
					MAPIFolder subFolder = null;
					try {
						subFolder = FindCalendarFolderRecursive(folder.Folders, searchFolderPath);
						if (subFolder != null)
							return subFolder.Items;
					}
					finally {
						ReleaseOutlookObject(subFolder);
						subFolder = null;
					}
				}
			}
			catch {
				return null;
			}
			finally {
				ReleaseOutlookObject(folder);
				folder = null;
			}
			return null;
		}
		static _Items GetNamespaceCalendarItems(_NameSpace olNamespace, string folderPath) {
			_Folders olFolders = GetNamespaceFolders(olNamespace);
			if (olFolders == null)
				return null;
			int count = olFolders.Count;
			if (count == 0)
				return null;
			string rootFolderName = CalculateRootFolderName(folderPath);
			for (int i = 1; i <= count; i++) {
				string folderName = olFolders[i].Name;
				if (!MatchRootFolder(rootFolderName, folderName))
					continue;
				MAPIFolder rootFolder = GetRootSubFolderByName(olNamespace, folderName);
				if (rootFolder != null) {
					_Items items = GetRootFolderCalendarItems(rootFolder, folderPath);
					if (items != null)
						return items;
				}
			}
			return null;
		}
		internal static bool MatchRootFolder(string rootFolderName, string folderName) {
			if (String.IsNullOrEmpty(rootFolderName))
				return true;
			return rootFolderName.CompareTo(folderName) == 0;
		}
		internal static string CalculateRootFolderName(string folderPath) {
			int lenght = RootFolderPrefix.Length;
			if (folderPath.Length <= lenght)
				return String.Empty;
			if (folderPath.StartsWith(RootFolderPrefix)) {
				int pos = folderPath.IndexOf(PathSeparator, lenght);
				if (pos > 0)
					return folderPath.Substring(lenght, pos - lenght);
				else
					return folderPath.Replace(RootFolderPrefix, string.Empty);
			}
			return string.Empty;
		}
		static _Items GetRootFolderCalendarItems(MAPIFolder rootFolder, string folderPath) {
			if (rootFolder == null)
				return null;
			try {
				MAPIFolder calendar = FindCalendarFolderRecursive(rootFolder.Folders, folderPath);
				if (calendar == null)
					return null;
				try {
					return calendar.Items;
				}
				finally {
					ReleaseOutlookObject(calendar);
					calendar = null;
				}
			}
			catch {
				return null;
			}
			finally {
				ReleaseOutlookObject(rootFolder);
				rootFolder = null;
			}
		}
		static MAPIFolder FindCalendarFolderRecursive(_Folders folders, string path) {
			if (folders == null || string.IsNullOrEmpty(path))
				return null;
			try {
				int count = folders.Count;
				if (count <= 0) return null;
				MAPIFolder folder = null;
				if (!ShouldCompareFolderByFullPath(path)) {
					folder = GetSubFolderByName(folders, path);
					if (folder != null && FolderIsCalendar(folder))
						return folder;
				}
				for (int i = 1; i <= count; i++) {
					folder = folders[i];
					if (folder != null && FolderIsCalendar(folder) && IsFolderPathValid(folder, path))
						return folder;
					try {
						folder = FindCalendarFolderRecursive(folders[i].Folders, path);
					}
					catch {
						continue;  
					}
					if (folder != null) return folder;
				}
			}
			catch {
				return null;
			}
			finally {
				ReleaseOutlookObject(folders);
				folders = null;
			}
			return null;
		}
	}
	#endregion
	#region AppointmentBinder
	public class AppointmentBinder {
		#region Fields
		IGetAppointmentForeignId foreignIdAccessor;
		Hashtable outlookEntryIdTable = new Hashtable();
		AppointmentBaseCollection nonLinkedAppointments = new AppointmentBaseCollection();
		#endregion
		public AppointmentBinder(IGetAppointmentForeignId foreignIdAccessor) {
			this.foreignIdAccessor = foreignIdAccessor;
		}
		#region Properties
		protected internal AppointmentBaseCollection NonLinkedAppointments { get { return nonLinkedAppointments; } }
		protected internal Hashtable OutlookEntryIdTable { get { return outlookEntryIdTable; } }
		protected IGetAppointmentForeignId ForeignIdAccessor { get { return foreignIdAccessor; } }
		#endregion
		public virtual void CreateOutlookEntryIdTable(AppointmentBaseCollection apts, bool ignoreExceptions) {
			Clear();
			FillEntryIdTable(apts, nonLinkedAppointments, ignoreExceptions);
		}
		public void AddLink(string entryId, Appointment apt) {
			OutlookEntryIdTable[entryId] = apt;
		}
		public void RemoveLink(string entryId) {
			if (OutlookEntryIdTable.ContainsKey(entryId))
				OutlookEntryIdTable.Remove(entryId);
		}
		public void RemoveAppointmentLinks(Appointment apt) {
			NonLinkedAppointments.Remove(apt);
			if (OutlookEntryIdTable.ContainsValue(apt)) {
				IDictionaryEnumerator en = OutlookEntryIdTable.GetEnumerator();
				while (en.MoveNext()) {
					if (Object.Equals(apt, en.Value)) {
						OutlookEntryIdTable.Remove(en.Key);
						break;
					}
				}
			}
		}
		protected internal virtual void Clear() {
			outlookEntryIdTable.Clear();
			nonLinkedAppointments.Clear();
		}
		protected internal virtual void FillEntryIdTable(ICollection appointments, AppointmentBaseCollection nonLinkedAppointments, bool ignoreExceptions) {
			foreach (Appointment apt in appointments) {
				BindAppointment(apt, nonLinkedAppointments);
				if (!ignoreExceptions && apt.HasExceptions)
					FillEntryIdTable(apt.GetExceptions(), nonLinkedAppointments, false);
			}
		}
		protected internal virtual void BindAppointment(Appointment apt, AppointmentBaseCollection nonLinkedAppointments) {
			object entryId = ForeignIdAccessor.GetForeignId(apt);
			if (entryId != null && entryId != DBNull.Value && !IsEmptyString(entryId))
				outlookEntryIdTable[entryId] = apt;
			else
				nonLinkedAppointments.Add(apt);
		}
		protected internal bool IsEmptyString(object entryId) {
			string s = Convert.ToString(entryId);
			return String.IsNullOrEmpty(s);
		}
		public Appointment FindLinkedAppointment(string entryId) {
			return OutlookEntryIdTable[entryId] as Appointment;
		}
		public AppointmentBaseCollection GetUnboundAppointments() {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			result.AddRange(nonLinkedAppointments);
			result.AddRange(outlookEntryIdTable.Values); 
			return result;
		}
	}
	#endregion
	#region AppointmentsDeleteRuleComparer
	public class AppointmentsDeleteRuleComparer : IComparer<Appointment>, IComparer {
		int IComparer.Compare(object x, object y) {
			Appointment xApt = (Appointment)x;
			Appointment yApt = (Appointment)y;
			return CompareCore(xApt, yApt);
		}
		public int Compare(Appointment x, Appointment y) {
			return CompareCore(x, y);
		}
		protected internal virtual int CompareCore(Appointment xApt, Appointment yApt) {
			if (xApt.IsBase && yApt.IsException)
				return 1;
			if (xApt.IsException && yApt.IsBase)
				return -1;
			return 0;
		}
	}
	#endregion
	[CLSCompliant(false)]
	public static class OutlookSyncHelper {
		public static OutlookAppointmentSynchronizingEventArgs CreateSynchronizingEventArgs(Appointment apt, _AppointmentItem olApt, SynchronizeOperation defaultOperation) {
			OutlookAppointmentSynchronizingEventArgs args = CreateSynchronizingEventArgs(apt, olApt);
			args.Operation = defaultOperation;
			return args;
		}
		public static OutlookSynchronizeInfo CreateSynchronizeInfo(OutlookAppointmentSynchronizingEventArgs args, SynchronizeOperation defaultOperation, bool isTermination) {
			OutlookSynchronizeInfo result = new OutlookSynchronizeInfo(defaultOperation, args.Cancel, isTermination);
			result.Operation = args.Operation;
			return result;
		}
		static OutlookAppointmentSynchronizingEventArgs CreateSynchronizingEventArgs(Appointment apt, _AppointmentItem olApt) {
			return apt != null ? new OutlookAppointmentSynchronizingEventArgs(apt, olApt) : new OutlookAppointmentSynchronizingEventArgs(olApt);
		}
		public static OutlookAppointmentSynchronizedEventArgs CreateSynchronizedEventArgs(Appointment apt, _AppointmentItem olApt) {
			return apt != null ? new OutlookAppointmentSynchronizedEventArgs(apt, olApt) : new OutlookAppointmentSynchronizedEventArgs(olApt);
		}
		public static void SetAppointmentForeignId(Appointment apt, _AppointmentItem olApt, ISchedulerStorageBase storage, string foreignIdFieldName) {
			if (olApt == null || apt == null)
				return;
			string entryId = olApt.EntryID;
			if (apt.IsException) {
				entryId = OutlookUtils.GetRecurrenceEntryId(entryId, apt.RecurrenceIndex);
			}
			apt.SetValue(storage, foreignIdFieldName, entryId);
		}
	}
	#region OutlookExchangeInfo (abstract class)
	public abstract class OutlookExchangeInfo {
		bool termination;
		bool cancel;
		protected OutlookExchangeInfo() {
		}
		protected OutlookExchangeInfo(bool cancel, bool termination) {
			this.cancel = cancel;
			this.termination = termination;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public bool Termination { get { return termination; } }
	}
	#endregion
	#region OutlookExportInfo
	public class OutlookExportInfo : OutlookExchangeInfo {
		public OutlookExportInfo() {
		}
		public OutlookExportInfo(bool cancel, bool termination)
			: base(cancel, termination) {
		}
	}
	#endregion
	#region OutlookImportInfo
	public class OutlookImportInfo : OutlookExchangeInfo {
		public OutlookImportInfo() {
		}
		public OutlookImportInfo(bool cancel, bool termination)
			: base(cancel, termination) {
		}
	}
	#endregion
	#region OutlookSynchronizeInfo
	public class OutlookSynchronizeInfo : OutlookExchangeInfo {
		SynchronizeOperation operation = SynchronizeOperation.Create;
		SynchronizeOperation defaultOperation = SynchronizeOperation.Create;
		public OutlookSynchronizeInfo(SynchronizeOperation defaultOperation, bool cancel, bool termination)
			: base(cancel, termination) {
			this.defaultOperation = defaultOperation;
		}
		public SynchronizeOperation Operation { get { return operation; } set { operation = value; } }
		public SynchronizeOperation DefaultOperation { get { return defaultOperation; } }
	}
	#endregion
}
