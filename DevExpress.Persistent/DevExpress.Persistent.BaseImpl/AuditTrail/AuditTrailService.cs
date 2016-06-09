#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Persistent.Base;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.Persistent.AuditTrail {
	public enum AuditTrailStrategy { OnObjectLoaded, OnObjectChanged };
	public enum ObjectAuditingMode { Full, Lightweight, CreationOnly };
	public class CustomizeAuditTrailSettingsEventArgs : EventArgs {
		private AuditTrailSettings auditTrailSettings;
		public AuditTrailSettings AuditTrailSettings {
			get { return auditTrailSettings; }
			set { auditTrailSettings = value; }
		}
		public CustomizeAuditTrailSettingsEventArgs(AuditTrailSettings auditTrailSettings) {
			this.auditTrailSettings = auditTrailSettings;
		}
	}
	public class SaveAuditTrailDataEventArgs : EventArgs {
		private Session session;
		public Session Session {
			get { return session; }
			set { session = value; }
		}
		private bool handled;
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		private List<AuditDataItem> auditTrailDataItems;
		public List<AuditDataItem> AuditTrailDataItems {
			get { return auditTrailDataItems; }
		}
		public SaveAuditTrailDataEventArgs(Session session, List<AuditDataItem> auditTrailDataItems) {
			this.session = session;
			this.auditTrailDataItems = auditTrailDataItems;
		}
	}
	public class CustomCreateObjectAuditProcessorsFactoryEventArgs : HandledEventArgs {
		private IObjectAuditProcessorsFactory factory;
		public IObjectAuditProcessorsFactory Factory {
			get { return factory; }
			set { factory = value; }
		}
	}
	public class CustomizeSessionAuditingOptionsEventArgs : EventArgs {
		private Session session;
		private AuditTrailStrategy auditTrailStrategy;
		private ObjectAuditingMode objectAuditingMode;
		public CustomizeSessionAuditingOptionsEventArgs(Session session, AuditTrailStrategy auditTrailStrategy, ObjectAuditingMode objectAuditingMode) {
			this.session = session;
			this.auditTrailStrategy = auditTrailStrategy;
			this.objectAuditingMode = objectAuditingMode;
		}
		public Session Session {
			get { return session; }
		}
		public AuditTrailStrategy AuditTrailStrategy {
			get { return auditTrailStrategy; }
			set { auditTrailStrategy = value; }
		}
		public ObjectAuditingMode ObjectAuditingMode {
			get { return objectAuditingMode; }
			set { objectAuditingMode = value; }
		}
	}
	public delegate void CustomizeAuditSettingsEventHandler(object sender, CustomizeAuditTrailSettingsEventArgs e);
	public delegate void SaveAuditTrailDataEventHandler(object sender, SaveAuditTrailDataEventArgs e);
	public class AuditTrailService {
		private static BaseAuditDataStore auditDataStore;
		protected Dictionary<Session, ObjectAuditProcessor> objectAuditProcessors;
		private IObjectAuditProcessorsFactory objectAuditProcessorFactory;
		private static AuditTrailSettings settings;
		private static object locker = new object();
		private ObjectAuditingMode objectAuditingMode = ObjectAuditingMode.Full;
		private void RaiseCustomizeAuditTrailSettings() {
			CustomizeAuditTrailSettingsEventArgs args = new CustomizeAuditTrailSettingsEventArgs(settings);
			if(CustomizeAuditTrailSettings != null) {
				CustomizeAuditTrailSettings(this, args);
			}
		}
		private void CreateObjectAuditProcessorsFactory() {
			CustomCreateObjectAuditProcessorsFactoryEventArgs args = new CustomCreateObjectAuditProcessorsFactoryEventArgs();
			if(CustomCreateObjectAuditProcessorsFactory != null) {
				CustomCreateObjectAuditProcessorsFactory(this, args);
			}
			if(args.Handled && args.Factory != null) {
				objectAuditProcessorFactory = args.Factory;
			}
			else {
				objectAuditProcessorFactory = new ObjectAuditProcessorsFactory();
			}
		}
		private void InternalSaveAuditData(Session session, List<AuditDataItem> itemsToSave) {
			SaveAuditTrailDataEventArgs args = new SaveAuditTrailDataEventArgs(session, itemsToSave);
			if(SaveAuditTrailData != null) {
				SaveAuditTrailData(this, args);
			}
			if(!args.Handled) {
				SaveAuditDataByDefault(session, itemsToSave);
			}
		}
		private IAuditTimestampStrategy timestampStrategy = new LocalAuditTimestampStrategy();
		protected static AuditTrailService instance;
		static AuditTrailService() { }
		protected AuditTrailService() {
			objectAuditProcessors = new Dictionary<Session, ObjectAuditProcessor>();
		}
		protected virtual void SaveAuditDataByDefault(Session session, List<AuditDataItem> itemsToSave) {
			if(AuditDataStore != null) {
				AuditDataStore.Save(session, itemsToSave, TimestampStrategy, auditDataStore_QueryCurrentUserName(this, new QueryCurrentUserNameEventArgs()));
			}
			else {
				throw new ArgumentNullException("AuditDataStore");
			}
		}
		public static AuditTrailService Instance {
			get {
				if(instance == null) {
					instance = new AuditTrailService();
				}
				return instance;
			}
		}
		public void SetXPDictionary(XPDictionary xpDictionary) {
			List<XPDictionary> dictionaries = new List<XPDictionary>() { xpDictionary };
			SetXPDictionaries(dictionaries);
		}
		public void SetXPDictionaries(IList<XPDictionary> dictionaries) {
			lock(locker) {
				settings = new AuditTrailSettings();
				settings.SetXPDictionaries(dictionaries);
				settings.Clear();
				settings.AddAllTypes();
				RaiseCustomizeAuditTrailSettings();
				CreateObjectAuditProcessorsFactory();
			}
		}
		public bool IsSessionAudited(Session session) {
			lock(objectAuditProcessors) {
				return (objectAuditProcessors.ContainsKey(session));
			}
		}
		public ObjectAuditProcessor BeginSessionAudit(Session session, AuditTrailStrategy strategy) {
			return BeginSessionAudit(session, strategy, objectAuditingMode);
		}
		public ObjectAuditProcessor BeginSessionAudit(Session session, AuditTrailStrategy strategy, ObjectAuditingMode auditingMode) {
			Guard.ArgumentNotNull(objectAuditProcessorFactory, "ObjectAuditProcessorsFactory");
			lock(objectAuditProcessors) {
				CustomizeSessionAuditingOptionsEventArgs args = new CustomizeSessionAuditingOptionsEventArgs(session, strategy, auditingMode);
				if(CustomizeSessionAuditingOptions != null) {
					CustomizeSessionAuditingOptions(this, args);
				}
				if(IsSessionAudited(session) && !objectAuditProcessorFactory.IsSuitableAuditProcessor(objectAuditProcessors[session], args.ObjectAuditingMode)) {
					EndSessionAudit(session);
				}
				if(!IsSessionAudited(session)) {
					objectAuditProcessors[session] = objectAuditProcessorFactory.CreateAuditProcessor(args.ObjectAuditingMode, session, settings);
					session.Disposed += new EventHandler(session_Disposed);
				}
				objectAuditProcessors[session].Strategy = args.AuditTrailStrategy;
			}
			return objectAuditProcessors[session];
		}
		void session_Disposed(object sender, EventArgs e) {
			Session session = sender as Session;
			session.Disposed -= new EventHandler(session_Disposed);
			EndSessionAudit(session);
		}
		public void BeginObjectsAudit(Session session, params object[] alreadyLoadedObjects) {
			BeginObjectsAudit(session, objectAuditingMode, alreadyLoadedObjects);
		}
		public void BeginObjectsAudit(Session session, ObjectAuditingMode auditingMode, params object[] alreadyLoadedObjects) {
			Tracing.Tracer.LogLockedSectionEntering(GetType(), "BeginObjectsAudit", objectAuditProcessors);
			lock(objectAuditProcessors) {
				Tracing.Tracer.LogLockedSectionEntered();
				BeginSessionAudit(session, AuditTrailStrategy.OnObjectChanged, auditingMode);
				if(alreadyLoadedObjects != null) {
					foreach(object obj in alreadyLoadedObjects) {
						objectAuditProcessors[session].BeginObjectAudit(obj);
					}
				}
			}
		}
		public AuditTrailStrategy? GetStrategy(Session session) {
			if(IsSessionAudited(session)) {
				return objectAuditProcessors[session].Strategy;
			}
			return null;
		}
		public void EndSessionAudit(Session session) {
			Tracing.Tracer.LogLockedSectionEntering(GetType(), "EndSessionAudit", objectAuditProcessors);
			lock(objectAuditProcessors) {
				Tracing.Tracer.LogLockedSectionEntered();
				ObjectAuditProcessor processor = null;
				if(objectAuditProcessors.TryGetValue(session, out processor) && processor != null) {
					objectAuditProcessors.Remove(session);
					processor.Dispose();
				}
			}
		}
		public virtual void SaveAuditData(Session session) {
			Tracing.Tracer.LogLockedSectionEntering(GetType(), "SaveAuditData", objectAuditProcessors);
			lock(objectAuditProcessors) {
				Tracing.Tracer.LogLockedSectionEntered();
				if(objectAuditProcessors.ContainsKey(session)) {
					ObjectAuditProcessor objectAuditProcessor = objectAuditProcessors[session];
					DiffCollection allDiffs = objectAuditProcessor.GetAllDiffs();
					List<AuditDataItem> items = allDiffs.All();
					InternalSaveAuditData(session, items);
					objectAuditProcessors.Remove(session);
					objectAuditProcessor.Dispose();
				}
			}
		}
		public void AddCustomAuditData(Session session, AuditDataItem auditDataItem) {
			Tracing.Tracer.LogLockedSectionEntering(GetType(), "AddCustomAuditData", objectAuditProcessors);
			lock(objectAuditProcessors) {
				Tracing.Tracer.LogLockedSectionEntered();
				if(!IsSessionAudited(session)) {
					BeginSessionAudit(session, AuditTrailStrategy.OnObjectChanged);
				}
				objectAuditProcessors[session].AddAuditDataItem(auditDataItem);
			}
		}
		public BaseAuditDataStore AuditDataStore {
			get {
				return auditDataStore;
			}
			set {
				SetDataStore(value);
			}
		}
		private void SetDataStore(BaseAuditDataStore newAuditDataStore) {
			auditDataStore = newAuditDataStore;
		}
		string auditDataStore_QueryCurrentUserName(object sender, QueryCurrentUserNameEventArgs e) {
			if(QueryCurrentUserName != null) {
				QueryCurrentUserName(this, e);
			}
			return e.CurrentUserName;
		}
		public IAuditTimestampStrategy TimestampStrategy {
			get { return timestampStrategy; }
			set { timestampStrategy = value; }
		}
#if DEBUG
		public ObjectAuditProcessor GetSessionAuditProcessor(Session session) {
			if(objectAuditProcessors.ContainsKey(session)) {
				return objectAuditProcessors[session];
			}
			return null;
		}
		public static void DEBUG_Clear() {
			settings = null;
		}
#endif
		public AuditTrailSettings Settings {
			get {
				return settings;
			}
		}
		public ObjectAuditingMode ObjectAuditingMode {
			get { return objectAuditingMode; }
			set { objectAuditingMode = value; }
		}
		public IObjectAuditProcessorsFactory ObjectAuditProcessorsFactory {
			get { return objectAuditProcessorFactory; }
		}
		public event CustomizeAuditSettingsEventHandler CustomizeAuditTrailSettings;
		public event SaveAuditTrailDataEventHandler SaveAuditTrailData;
		public event QueryCurrentUserNameEventHandler QueryCurrentUserName;
		public event EventHandler<CustomCreateObjectAuditProcessorsFactoryEventArgs> CustomCreateObjectAuditProcessorsFactory;
		public event EventHandler<CustomizeSessionAuditingOptionsEventArgs> CustomizeSessionAuditingOptions;
	}
}
