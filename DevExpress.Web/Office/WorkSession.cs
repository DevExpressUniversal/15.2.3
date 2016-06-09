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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.Web.Office;
using SessionDictCore = System.Collections.Generic.Dictionary<System.Guid, DevExpress.Web.Office.Internal.WorkSessionBase>;
namespace DevExpress.Web.Office.Internal {
	public interface IWorkSession {
		Guid ID { get; }
		WorkSessionState State { get; }
		bool Modified { get; }
		DateTime? StartModifyTime { get; }
		DateTime LastTimeActivity { get; }
		bool EnableAutoSave { get; }
		TimeSpan AutoSaveTimeout { get; set; }
		void AutoSave();
		HibernationContainer GetHibernationContainer();
		void OnHibernated();
		void WakeUp(HibernationContainer hibernationContainer);
	}
	public abstract class WorkSessionBase : IWorkSession {
		public string DocumentPathOrID { get; set; }
		public Guid ID { get; set; } 
		public WorkSessionState State { get { return StateController.State; } }
		protected WorkSessionStateController StateController = new WorkSessionStateController();
		IDocumentInfo documentInfo;
		public IDocumentInfo DocumentInfo {
			get {
				if(documentInfo == null)
					documentInfo = GetCreateDocumentInfo();
				return documentInfo;
			}
		}
		protected abstract IDocumentInfo GetCreateDocumentInfo();
		protected WorkSessionBase(Guid id) : this(null, id) {
		}
		protected WorkSessionBase(DocumentContentContainer documentContentContainer, Guid id) {
			RefreshLastTimeActivity();
			DocumentPathOrID = documentContentContainer != null ? documentContentContainer.PathOrID : string.Empty;
			ID = id;
			WorkSessionAdminTools.RaiseCreated(this);
		}
		public DateTime LastTimeActivity { get; internal set; } 
		internal bool IsOlderThen(int seconds, DateTime now) {
			return (now - LastTimeActivity).TotalSeconds >= seconds;
		}
		protected void RefreshLastTimeActivity() {
			LastTimeActivity = DateTime.Now;
		}
		DateTime? lastModifyTime = null;
		public DateTime? LastModifyTime { get { return this.lastModifyTime; } }
		protected void RefreshLastModifyTime() {
			this.lastModifyTime = DateTime.Now;
		}
		DateTime? startModifyTime = null;
		public DateTime? StartModifyTime { get { return this.startModifyTime; } }
		void RefreshStartModifyTime() {
			this.startModifyTime = Modified ? DateTime.Now : (DateTime?)null;
		}
		protected void OnModifiedChanged(bool value) {
			ModifiedInternal = value;
			RefreshStartModifyTime();
			WorkSessionAdminTools.RaiseModifiedChanged(this);
		}
		public bool Modified { 
			get { 
				if(DocumentControlExists)
					return GetModified();
				else
					return ModifiedInternal;
			} 
			set { 
				SetModified(value);
				ModifiedInternal = value;
			}
		}
		bool ModifiedInternal { get; set; }
		protected abstract bool GetModified();
		protected abstract void SetModified(bool value);
		protected abstract bool DocumentControlExists { get; }
		public virtual bool EnableAutoSave { get; private set; }
		public virtual TimeSpan AutoSaveTimeout { get; set; }
		public void AutoSave() {
			var args = DocumentManager.RaiseAutoSaving(DocumentInfo, MultiUserConflict.None);
			if(args.Handled)
				Modified = false;
			if(!args.Handled)
				SaveInTheSameFile();
			WorkSessionAdminTools.RaiseAutoSaved(this);
		}
		public void SaveInTheSameFile() {
			EnsureDocumentPathBeforeSaving();
			SaveInTheSameFileCore();
		}
		public void EnsureDocumentPathBeforeSaving() {
			bool documentPathIsKnown = !string.IsNullOrEmpty(GetCurrentDocumentFilePath());
			if(!documentPathIsKnown)
				throw new CantSaveToEmptyPathException();
		}
		protected abstract string GetCurrentDocumentFilePath();
		protected internal abstract void CreateNewDocument();
		protected internal abstract void LoadDocument(DocumentContentContainer documentContentContainer);
		protected abstract void SaveInTheSameFileCore();
		protected internal abstract void SaveAs(DocumentContentContainer saveAsDocumentContentContainer);
		protected internal abstract WorkSessionBase Clone(Guid newWorkSessionID, string documentPathOrIDs);
		internal DocumentHandlerResponse ProcessRequest(NameValueCollection nameValueCollection) {
			OnBeforeDocumentAccess();
			RefreshLastTimeActivity();
			return ProcessRequestCore(nameValueCollection);
		}
		protected abstract DocumentHandlerResponse ProcessRequestCore(NameValueCollection nameValueCollection);
		public void OnNewDocumentCreated() {
			RefreshLastTimeActivity();
			DocumentPathOrID = string.Empty;
		}
		public void OnNewDocumentLoaded(DocumentContentContainer documentContentContainer) {
			RefreshLastTimeActivity();
			DocumentPathOrID = documentContentContainer.PathOrID;
		}
		public void OnDocumentSaved() {
			RefreshLastTimeActivity();
		}
		public void OnNewClientAttached() {
			RefreshLastTimeActivity();
		}
		protected internal void Close() {
			WorkSessionAdminTools.RaiseDisposed(this);
			CloseCore();
		}
		protected abstract void CloseCore();
		public void SyncSettingWithControl(IAutoSaveControl autoSaveControl) {
			if(autoSaveControl.AutoSaveMode == AutoSaveMode.On)
				EnableAutoSave = true;
			if(autoSaveControl.AutoSaveTimeout < AutoSaveTimeout || AutoSaveTimeout.Ticks == 0)
				AutoSaveTimeout = autoSaveControl.AutoSaveTimeout;
		}
		public HibernationContainer GetHibernationContainer() {
			if(State != WorkSessionState.Loaded)
				return null;
			HibernationContainer hibernationContainer = null;
			hibernationContainer = SaveToHibernationContainer();
			return hibernationContainer;
		}
		public void OnHibernated() {
			UnloadDocumentFromMemory();
			StateController.OnHibernated();
			WorkSessionAdminTools.RaiseHibernated(this);
		}
		protected HibernationContainer SaveToHibernationContainer() {
			var descriptor = new HibernationChamberDescriptor(DocumentPathOrID, GetWorkSessionDocumentTypeName(), LastTimeActivity, Modified);
			var chamber = SaveToHibernationChamber();
			var hibernationContainer = new HibernationContainer(descriptor, chamber);
			return hibernationContainer;
		}
		protected abstract HibernationChamber SaveToHibernationChamber();
		protected abstract void UnloadDocumentFromMemory();
		public void OnBeforeDocumentAccess() {
			if(State == WorkSessionState.Hibernated) {
				((IWorkSessionHibernateProvider)WorkSessionProcessing.WorkSessionHibernateProviderSingleton).WakeUp(this);
			}
		}
		public void WakeUp(HibernationContainer hibernationContainer) {
			if(State == WorkSessionState.Hibernated) {
				StateController.OnBeforeWakeUp();
				var success = RestoreFromHibernationContainer(hibernationContainer);
				if(success) {
					StateController.OnAfterWakeUp();
					RefreshLastTimeActivity();
					WorkSessionAdminTools.RaiseWokeUp(this);
				}
			}
		}
		protected bool RestoreFromHibernationContainer(HibernationContainer hibernationContainer) {
			var success = RestoreFromHibernationChamber(hibernationContainer.Chamber);
			if(success) {
				DocumentPathOrID = hibernationContainer.Descriptor.DocumentPathOrID;
				Modified = hibernationContainer.Descriptor.Modified;
			}
			return success;
		}
		protected abstract bool RestoreFromHibernationChamber(HibernationChamber hibernationChamber);
		protected abstract string GetWorkSessionDocumentTypeName();
		internal void OnInitializedHibernated() {
			StateController.OnInitializedHibernated();
		}
	}
	public abstract class WorkSessionBase<T> : WorkSessionBase {
		public WorkSessionBase(DocumentContentContainer documentContentContainer, Guid id, T settings)
			: base(documentContentContainer, id) {
			CreateModelCore(documentContentContainer, settings);
			StateController.OnModelCreated();
		}
		protected abstract void CreateModelCore(DocumentContentContainer documentContentContainer, T settings);
	}
	public class WorkSessionDictionary : IDictionary<Guid, WorkSessionBase> {
		public SessionDictCore Sessions = new SessionDictCore();
		protected virtual void OnSetting(Guid key, WorkSessionBase value) { }
		protected virtual void OnAdding(Guid key, WorkSessionBase value) { }
		protected virtual void OnRemoving(Guid key) { }
		protected virtual void OnClearing() { }
		#region IDictionary<Guid,WorkSessionBase> Members
		public ICollection<Guid> Keys { get { return Sessions.Keys; } }
		public ICollection<WorkSessionBase> Values { get { return Sessions.Values; } }
		public WorkSessionBase this[Guid key] {
			get { return Sessions[key]; }
			set {
				OnSetting(key, value);
				Sessions[key] = value;
			}
		}
		public void Add(Guid key, WorkSessionBase value) {
			OnAdding(key, value);
			Sessions.Add(key, value);
		}
		public bool ContainsKey(Guid key) {
			return Sessions.ContainsKey(key);
		}
		public bool Remove(Guid key) {
			OnRemoving(key);
			return Sessions.Remove(key);
		}
		public bool TryGetValue(Guid key, out WorkSessionBase value) {
			return Sessions.TryGetValue(key, out value);
		}
		#endregion
		#region ICollection<KeyValuePair<Guid,WorkSessionBase>> Members
		public int Count { get { return Sessions.Count; } }
		public bool IsReadOnly { get { return ((ICollection<KeyValuePair<Guid, WorkSessionBase>>)Sessions).IsReadOnly; } }
		public void Add(KeyValuePair<Guid, WorkSessionBase> item) {
			OnAdding(item.Key, item.Value);
			((ICollection<KeyValuePair<Guid, WorkSessionBase>>)Sessions).Add(item);
		}
		public void Clear() {
			OnClearing();
			Sessions.Clear();
		}
		public bool Contains(KeyValuePair<Guid, WorkSessionBase> item) {
			return Sessions.Contains(item);
		}
		public void CopyTo(KeyValuePair<Guid, WorkSessionBase>[] array, int arrayIndex) {
			((ICollection<KeyValuePair<Guid, WorkSessionBase>>)Sessions).CopyTo(array, arrayIndex);
		}
		public bool Remove(KeyValuePair<Guid, WorkSessionBase> item) {
			OnRemoving(item.Key);
			return ((ICollection<KeyValuePair<Guid, WorkSessionBase>>)Sessions).Remove(item);
		}
		#endregion
		#region IEnumerable<KeyValuePair<Guid,WorkSessionBase>> Members
		public IEnumerator<KeyValuePair<Guid, WorkSessionBase>> GetEnumerator() {
			return Sessions.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return Sessions.GetEnumerator();
		}
		#endregion
	}
	public static class WorkSessions {
		static object locker = new object();
		static ICustomerCustomersInteractionStrategy customerInteactionStrategy = GetNewDefaultCustomersInteractionStrategy();
		static WorkSessionDictionary SessionDictionary { get { return customerInteactionStrategy.WorkSessionDictionary; } }
		public static void SetCustomersInteractionStrategy(ICustomerCustomersInteractionStrategy strategy) {
			customerInteactionStrategy = strategy;
		}
		public static void ResetCustomersInteractionStrategyToDefault() {
			SetCustomersInteractionStrategy(GetNewDefaultCustomersInteractionStrategy());
		}
		public static ICustomerCustomersInteractionStrategy GetNewDefaultCustomersInteractionStrategy() {
			return new MultiCustomerCollaborationStrategy();
		}
		public static Guid SaveAsWorkSession(Guid currentWorkSessionID, DocumentContentContainer saveAsDocumentContentContainer) {
			lock(locker) {
				var canReuseCurrentWorkSession = customerInteactionStrategy.CanReuseWorkSession;
				if(canReuseCurrentWorkSession) {
					WorkSessionBase myCurrentWorkSessionToReUse = Get(currentWorkSessionID, true);
					myCurrentWorkSessionToReUse.SaveAs(saveAsDocumentContentContainer);
					Guid changedWorkSessionID = RenewWorkSessionID(currentWorkSessionID);
					return changedWorkSessionID;
				} else {
					WorkSessionBase currentWorkSession = Get(currentWorkSessionID, true);
					Guid newGuid = GenerateNewGuid();
					WorkSessionBase newWorkSession = currentWorkSession.Clone(newGuid, saveAsDocumentContentContainer.PathOrID);
					newWorkSession.SaveAs(saveAsDocumentContentContainer);
					AddWorkSessionToDictionary(newWorkSession);
					return newGuid;
				}
			}
		}
		public static Guid OpenWorkSession(Guid currentWorkSessionID, DocumentContentContainer documentContentContainer, Func<Guid, WorkSessionBase> createNewWorkSession) {
			Guid openedDocumentWorkSessionID = GetWorkSessionGuidIfExist(currentWorkSessionID, documentContentContainer.PathOrID);
			if(openedDocumentWorkSessionID != Guid.Empty) {
				WorkSessionBase ws = Get(openedDocumentWorkSessionID, true);
				ws.OnNewClientAttached();
				return openedDocumentWorkSessionID;
			}
			return OpenNewWorkSession(currentWorkSessionID, documentContentContainer, createNewWorkSession);
		}
		static Guid GetWorkSessionGuidIfExist(Guid currentWorkSessionID, string documentPathOrID) {
			Guid openedWorkSession = Guid.Empty;
			if(!string.IsNullOrEmpty(documentPathOrID)) {
				documentPathOrID = documentPathOrID.ToLower();
				openedWorkSession = customerInteactionStrategy.GetOpenedWorkSessionID(currentWorkSessionID, documentPathOrID);
			}
			return openedWorkSession;
		}
		static Guid OpenNewWorkSession(Guid currentWorkSessionID, DocumentContentContainer documentContentContainer, Func<Guid, WorkSessionBase> createNewWorkSession) {
			Guid newGuid = GenerateNewGuid(); 
			lock(locker) {
				bool canReuseWorkSession = customerInteactionStrategy.CanReuseWorkSession;
				WorkSessionBase newWorkSession = CreateNewOrReuseExistingWorkSession(currentWorkSessionID, documentContentContainer, createNewWorkSession, newGuid , canReuseWorkSession);
				AddWorkSessionToDictionary(newWorkSession);
				if(currentWorkSessionID != Guid.Empty && canReuseWorkSession) {
					SessionDictionary.Remove(currentWorkSessionID);
				}
			}
			return newGuid; 
		}
		static WorkSessionBase CreateNewOrReuseExistingWorkSession(Guid currentSessionGuid, DocumentContentContainer documentContentContainer, Func<Guid, WorkSessionBase> createNewWorkSession, Guid newGuid, bool canReuseWorkSession) {
			WorkSessionBase reusedWorkSession = null;
			bool sessionAlreadyExists = currentSessionGuid != Guid.Empty;
			if(sessionAlreadyExists && canReuseWorkSession)
				reusedWorkSession = ReuseWorkSessionIfExist_ForOpening(currentSessionGuid, newGuid, documentContentContainer);
			return reusedWorkSession ?? createNewWorkSession(newGuid);
		}
		static WorkSessionBase ReuseWorkSessionIfExist_ForOpening(Guid currentSessionGuid, Guid newGuid, DocumentContentContainer documentContentContainer) {
			WorkSessionBase currentSession = null;
			lock(locker) {
					currentSession = SessionDictionary[currentSessionGuid];
			}
			if(currentSession != null) {
				lock(currentSession) {
					currentSession.LoadDocument(documentContentContainer);
					currentSession.ID = newGuid;
				}
			}
			return currentSession;
		}
		public static WorkSessionBase Get(Guid workSessionID, bool ensureDocumentLoaded) {
			lock(locker) {
				if(SessionDictionary.ContainsKey(workSessionID)){
					var workSession = SessionDictionary[workSessionID];
					if(ensureDocumentLoaded)
						workSession.OnBeforeDocumentAccess();
					return workSession;
				}
				return null;
			}
		}
		public static Guid GetDocumentWorkSessionID(string documentPathOrID) {
			lock(locker) {
				KeyValuePair<Guid, WorkSessionBase> existsWorkSession = SessionDictionary.FirstOrDefault(s => s.Value.DocumentPathOrID.ToLower() == documentPathOrID.ToLower());
				return existsWorkSession.Key;
			}
		}
		public static void CloseCustomerSession(HttpSessionState session) {
			customerInteactionStrategy.CloseCustomerWorkSessions(session);
		}
		public static void CloseWorkSession(Guid workSessionID) {
			lock(locker) {
				if(!SessionDictionary.ContainsKey(workSessionID))
					return;
				WorkSessionBase currentWorkSession = SessionDictionary[workSessionID];
				lock(currentWorkSession) {
					currentWorkSession.Close();
				}
				SessionDictionary.Remove(workSessionID);
			}
		}
		public static void CloseAllWorkSessions() {
			lock(locker) {
				foreach(var workSessionID in SessionDictionary.Keys.ToList())
					CloseWorkSession(workSessionID);
			}
		}
		public static Guid GetTheOldestSession() {
			Guid oldestWorkSessionID = Guid.Empty;
			lock(locker) {
				DateTime now = DateTime.Now;
				double maxAge = 0;
				foreach(var workSessionID in SessionDictionary.Keys.ToList()) {
					WorkSessionBase sswSession = Get(workSessionID, false);
					double age = (now - sswSession.LastTimeActivity).TotalSeconds;
					if(maxAge < age) {
						maxAge = age;
						oldestWorkSessionID = workSessionID;
					}
				}
				return oldestWorkSessionID;
			}
		}
		public static void CloseWorkSessionOlderThen(int seconds) {
			lock(locker) {
				DateTime now = DateTime.Now;
				WorkSessionAdminTools.ForEachWorkSession(delegate(WorkSessionBase workSession) {
					if(workSession.IsOlderThen(seconds, now)) {
						CloseWorkSession(workSession.ID);
					}
				});
			}
		}
		public static Guid GenerateNewGuid() {
			lock(locker) {
				Guid newGuid;
				do {
					newGuid = Guid.NewGuid();
				}
				while(SessionDictionary.ContainsKey(newGuid));
				return newGuid;
			}
		}
		public static void ForEachWorkSession(Action<WorkSessionBase> action) {
				foreach (var workSessionID in SessionDictionary.Keys.ToList()) {
					WorkSessionBase sswSession = Get(workSessionID, false);
					if(sswSession != null) { 
						lock (sswSession)
							action(sswSession);
					}
				}
		}
		static void AddWorkSessionToDictionary(WorkSessionBase workSession) {
			if(workSession != null && !SessionDictionary.Keys.Contains(workSession.ID))
				SessionDictionary.Add(workSession.ID, workSession);
		}
		static Guid RenewWorkSessionID(Guid oldWorkSessionID) {
			WorkSessionBase workSession = SessionDictionary[oldWorkSessionID];
			if(workSession != null) {
				SessionDictionary.Remove(oldWorkSessionID);
				Guid newWorkSessionGuid = GenerateNewGuid();
				workSession.ID = newWorkSessionGuid;
				AddWorkSessionToDictionary(workSession);
				return newWorkSessionGuid;
			}
			return Guid.Empty;
		}
		public static MultiUserConflict DetectMultiUserSavingConflict(Guid currentWorkSessionID, string documentPathOrID) {
			Guid alreadyOpenedWorkSessionID = GetDocumentWorkSessionID(documentPathOrID);
			bool alreadyOpened = alreadyOpenedWorkSessionID != Guid.Empty;
			bool alreadyOpenedNotByMe = alreadyOpened && alreadyOpenedWorkSessionID != currentWorkSessionID;
			MultiUserConflict multiUserConflict = alreadyOpenedNotByMe ? MultiUserConflict.OtherUserDocumentOverride : MultiUserConflict.None;
			return multiUserConflict;
		}
		public static void CheckDocumentIsUnique(string documentId) {
			lock(locker) {
				ForEachWorkSession(delegate(WorkSessionBase workSession) {
					if(string.Equals(workSession.DocumentPathOrID, documentId))
						throw new DocumentIdIsnotUniqueException();
				});
			}
		}
		public static void OnControlDetachFromWorkSession(Guid workSessionID) {
			if(workSessionID == Guid.Empty) return;
			if (!SessionDictionary.ContainsKey(workSessionID)) return;
			WorkSessionBase workSession = SessionDictionary[workSessionID];
			if(workSession != null) {
				bool workSessionCantBeUsedByOtherControl = string.IsNullOrEmpty(workSession.DocumentPathOrID);
				if(workSessionCantBeUsedByOtherControl)
					CloseWorkSession(workSessionID);
			}
		}
		internal static void AddHibernatedSession(WorkSessionBase workSession, Guid workSessionID, HibernationChamberDescriptor hibernationChamberDescriptor) {
			workSession.ID = workSessionID;
			workSession.DocumentPathOrID = hibernationChamberDescriptor.DocumentPathOrID;
			workSession.LastTimeActivity = hibernationChamberDescriptor.LastActivityTime;
			workSession.OnInitializedHibernated();
			AddWorkSessionToDictionary(workSession);
		}
	}
	public class DocumentContentContainer {
		enum ContainerType { Undefined, File, Array, Stream }
		ContainerType containerType = ContainerType.Undefined;
		string pathToDocument;
		string documentID;
		string formatName;
		public DocumentContentContainer(string pathToDocument) : this(pathToDocument, string.Empty) { }
		public DocumentContentContainer(string pathToDocument, string formatName) {
			this.containerType = ContainerType.File;
			this.pathToDocument = pathToDocument;
			this.formatName = formatName;
		}
		public DocumentContentContainer(byte[] content, string formatName, string documentID) {
			this.containerType = ContainerType.Array;
			this.documentID = documentID;
			this.Array = content;
			this.formatName = formatName;
		}
		public DocumentContentContainer(Stream content, string formatName, string documentID) {
			this.containerType = ContainerType.Stream;
			this.documentID = documentID;
			this.Stream = content;
			this.formatName = formatName;
		}
		DocumentContentContainer(ContainerType containerType, string formatName) {
			this.containerType = containerType;
			this.formatName = formatName;
		}
		public string PathOrID {
			get {
				if(IsFile)
					return pathToDocument;
				else if(IsArray || IsStream)
					return documentID;
				else
					return null;
			}
		}
		public bool IsEmpty { get { return IsFile && string.IsNullOrEmpty(PathOrID); } }
		public bool IsFile { get { return this.containerType == ContainerType.File; } }
		public bool IsArray { get { return this.containerType == ContainerType.Array; } }
		public bool IsStream { get { return this.containerType == ContainerType.Stream; } }
		public string FormatName { get { return this.formatName; } }
		public static DocumentContentContainer Empty = new DocumentContentContainer(string.Empty);
		public byte[] Array { get; set; }
		public Stream Stream { get; private set; }
		public static DocumentContentContainer GetEmptyArrayContainer(string formatName) {
			return new DocumentContentContainer(ContainerType.Array, formatName);
		}
	}
	public class CantSaveToAlreadyOpenedFileException : Exception {
		public override string Message {
			get {
				return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Documents_CantSaveToAlreadyOpenedFileMessage);
			}
		}
	}
	public class DocumentAlreadyOpenedException : InvalidOperationException {
		const string message = "Document is already opened by another user";  
		public DocumentAlreadyOpenedException() : base(message) {
		}
	}
	public class CantSaveToEmptyPathException : InvalidOperationException {
		const string message = "Cannot save a document opened not from a file. Handle the Saving event to process custom saving";  
		public CantSaveToEmptyPathException() : base(message) { }
	}
	public class DocumentIdIsnotUniqueException : ArgumentException {
		const string message = "Document with the same DocumentId is already opened";  
		public DocumentIdIsnotUniqueException() : base(message) { }
	}
	public class DocumentIdCannotBeEmptyException : ArgumentException {
		const string message = "Can't find a document with unsigned documentId. Assign the documentId first";  
		public DocumentIdCannotBeEmptyException() : base(message) { }
	}
}
