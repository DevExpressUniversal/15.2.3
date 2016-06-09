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
using System.IO;
using System.Linq;
using System.Text;
using System.Web.SessionState;
using DevExpress.Web.Office;
using DevExpress.Web.Office.Internal;
namespace DevExpress.Web.Office {
	public interface OfficeWorkSessionControl { 
		Guid GetWorkSessionID();
		void AttachToWorkSession(Guid workSessionID);
	}
	public class DocumentManager {
		static event DocumentAutoSavingEventHandler AutoSavingCore;
		public static event DocumentAutoSavingEventHandler AutoSaving {
			add {
				if(!value.Method.IsStatic)
					throw new NotSupportedException("Only Static event handlers are allowed for the DocumentManager.AutoSaving event");
				if(AutoSavingCore == null || !AutoSavingCore.GetInvocationList().Contains(value))
					AutoSavingCore += value;
			} remove {
				AutoSavingCore -= value;
			}
		}
		internal static DocumentSavingEventArgs RaiseAutoSaving(IDocumentInfo documentInfo, MultiUserConflict multiUserConflict) {
			DocumentSavingEventArgs e = new DocumentSavingEventArgs(documentInfo.DocumentId, multiUserConflict);
			if(AutoSavingCore != null)
				AutoSavingCore(documentInfo, e);
			return e;
		}
		public static IDocumentInfo FindDocument(string documentId) {
			if(string.IsNullOrEmpty(documentId))
				throw new DocumentIdCannotBeEmptyException();
			IDocumentInfo documentInfo = null;
			WorkSessionAdminTools.ForEachWorkSession(delegate(WorkSessionBase workSessionBase) {
				if(string.Equals(workSessionBase.DocumentPathOrID, documentId))
					documentInfo = workSessionBase.DocumentInfo;
			});
			return documentInfo;
		}
		public static IEnumerable<IDocumentInfo> GetAllDocuments() {
			List<IDocumentInfo> documentInfos = new List<IDocumentInfo>();
			WorkSessionAdminTools.ForEachWorkSession(delegate(WorkSessionBase workSessionBase) {
				documentInfos.Add(workSessionBase.DocumentInfo);
			});
			return documentInfos;
		}
		public static void CloseAllDocuments() {
			WorkSessionAdminTools.CloseAllWorkSessions();
		}
		public static void CloseDocument(string documentId) {
			IDocumentInfo docInfo = FindDocument(documentId);
			if(docInfo != null)
				docInfo.Close();
		}
		[System.ComponentModel.DefaultValue(false)] 
		public static bool EnableHibernation { get { return WorkSessionProcessing.EnableHibernation; } set { WorkSessionProcessing.EnableHibernation = value; } }
		public static string HibernationStoragePath { get { return WorkSessionProcessing.HibernationStoragePath; } set { WorkSessionProcessing.HibernationStoragePath = value; } }
		public static TimeSpan HibernateTimeout { get { return WorkSessionProcessing.HibernateTimeout; } set { WorkSessionProcessing.HibernateTimeout = value; } }
		public static TimeSpan HibernatedDocumentsDisposeTimeout { get { return WorkSessionProcessing.HibernatedDocumentsDisposeTimeout; } set { WorkSessionProcessing.HibernatedDocumentsDisposeTimeout = value; } }
		public static bool HibernateAllDocumentsOnApplicationEnd { get { return WorkSessionProcessing.HibernateAllDocumentsOnApplicationEnd; } set { WorkSessionProcessing.HibernateAllDocumentsOnApplicationEnd = value; }  }
		public static void HibernateAllDocuments() {
			WorkSessionAdminTools.HibernateAllDocuments();
		}
	}
	public interface IDocumentInfo {
		string DocumentId { get; }
		DateTime LastAccessTime { get; }
		DateTime? LastModifyTime { get; }
		bool Modified { get; }
		byte[] SaveCopy();
		void SaveCopy(Stream stream);
		void SaveCopy(string filePath);
		void Close();
	}
}
namespace DevExpress.Web.Office.Internal {
	public abstract class OfficeDocumentBase<T> : IDocumentInfo where T : WorkSessionBase {
		public OfficeDocumentBase(T workSession) {
			this.WorkSession = workSession;
		}
		protected internal T WorkSession { get; private set; }
		public string DocumentId {
			get { 
				return WorkSession.DocumentPathOrID;
			}
		}
		public DateTime LastAccessTime {
			get { return WorkSession.LastTimeActivity; }
		}
		public virtual DateTime? LastModifyTime {
			get { return WorkSession.LastTimeActivity; }
		}
		public void Close() {
			WorkSessionAdminTools.CloseWorkSession(WorkSession.ID);
		}
		public abstract bool Modified { get; }
		public abstract byte[] SaveCopy();
		public abstract void SaveCopy(Stream stream);
		public abstract void SaveCopy(string filePath);
	}
	public static class WorkSessionAdminTools {
		static object locker = new object();
		public static event DocumentDiagnosticEventHandler AutoSaved;
		public static event DocumentDiagnosticEventHandler Hibernated;
		public static event DocumentDiagnosticEventHandler WokeUp;
		public static event DocumentDiagnosticEventHandler ModifiedChanged;
		public static event DocumentDiagnosticEventHandler Created;
		public static event DocumentDiagnosticEventHandler Disposed;
		internal static void RaiseAutoSaved(WorkSessionBase workSession) {
			DocumentDiagnosticEventArgs e = new DocumentDiagnosticEventArgs(workSession.DocumentInfo);
			if(AutoSaved != null)
				AutoSaved(workSession, e);
		}
		internal static void RaiseHibernated(WorkSessionBase workSession) {
			DocumentDiagnosticEventArgs e = new DocumentDiagnosticEventArgs(workSession.DocumentInfo);
			if(Hibernated != null)
				Hibernated(workSession, e);
		}
		internal static void RaiseWokeUp(WorkSessionBase workSession) {
			DocumentDiagnosticEventArgs e = new DocumentDiagnosticEventArgs(workSession.DocumentInfo);
			if(WokeUp != null)
				WokeUp(workSession, e);
		}
		internal static void RaiseModifiedChanged(WorkSessionBase workSession) {
			DocumentDiagnosticEventArgs e = new DocumentDiagnosticEventArgs(workSession.DocumentInfo);
			if(ModifiedChanged != null)
				ModifiedChanged(workSession, e);
		}
		internal static void RaiseCreated(WorkSessionBase workSession) {
			DocumentDiagnosticEventArgs e = new DocumentDiagnosticEventArgs(workSession.DocumentInfo);
			if(Created != null)
				Created(workSession, e);
		}
		internal static void RaiseDisposed(WorkSessionBase workSession) {
			DocumentDiagnosticEventArgs e = new DocumentDiagnosticEventArgs(workSession.DocumentInfo);
			if(Disposed != null)
				Disposed(workSession, e);
		}
		public static void CloseWorkSession(Guid workSessionID) {
			lock (locker) {
				WorkSessions.CloseWorkSession(workSessionID);
			}
		}
		public static void CloseAllWorkSessions() {
			lock (locker) {
				WorkSessions.CloseAllWorkSessions();
			}
		}
		public static void CloseWorkSessionOlderThen(int seconds) {
			lock (locker) {
				WorkSessions.CloseWorkSessionOlderThen(seconds);
			}
		}
		public static void CloseCustomerSession(HttpSessionState session) {
			lock (locker) {
				WorkSessions.CloseCustomerSession(session);
			}
		}
		public static void ForEachWorkSession(Action<WorkSessionBase> action) {
			lock (locker) {
				WorkSessions.ForEachWorkSession(action);
			}
		}
		public static void SetCustomersInteractionStrategy(ICustomerCustomersInteractionStrategy strategy) {
			WorkSessions.SetCustomersInteractionStrategy(strategy);
		}
		static IWorkSession GetWorkSession(Guid wsID) {
			IWorkSession workSession = null;
			WorkSessionAdminTools.ForEachWorkSession(
				ws => { 
					if(ws.ID == wsID) 
						workSession = ws; 
				});
			return workSession;
		}
		public static void HibernateWorkSession(Guid workSessionID) {
			IWorkSession workSession = GetWorkSession(workSessionID);
			((IWorkSessionHibernateProvider)WorkSessionProcessing.WorkSessionHibernateProviderSingleton).Hibernate(workSession);
		}
		public static void WakeUpSession(Guid workSessionID) {
			IWorkSession workSession = GetWorkSession(workSessionID);
			((IWorkSessionHibernateProvider)WorkSessionProcessing.WorkSessionHibernateProviderSingleton).WakeUp(workSession);
		}
		public static void HibernateAllDocuments() {
			WorkSessionAdminTools.ForEachWorkSession(
				workSession => {
					HibernateWorkSession(workSession.ID);
				}
			);
		}
	}
	public delegate void DocumentDiagnosticEventHandler(IWorkSession workSession, DocumentDiagnosticEventArgs e);
	public class DocumentDiagnosticEventArgs : EventArgs {
		public DocumentDiagnosticEventArgs(IDocumentInfo documentInfo) {
			DocumentInfo = documentInfo;
		}
		public IDocumentInfo DocumentInfo { get; private set; }
	}
}
