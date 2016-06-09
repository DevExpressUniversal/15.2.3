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
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using SessionDictCore = System.Collections.Generic.Dictionary<System.Guid, DevExpress.Web.Office.Internal.WorkSessionBase>;
namespace DevExpress.Web.Office.Internal {
	public interface ICustomerCustomersInteractionStrategy {
		WorkSessionDictionary WorkSessionDictionary { get; }
		Guid GetOpenedWorkSessionID(Guid sessionGuid, string documentPath);
		void CloseCustomerWorkSessions(HttpSessionState session);
		bool CanReuseWorkSession { get; }
	}
	public class SingleCustomerStrategy : ICustomerCustomersInteractionStrategy {
		WorkSessionDictionary sessionDictionary;
		readonly object locker = new object();
		public bool CanReuseWorkSession {
			get { return true; }
		}
		public WorkSessionDictionary WorkSessionDictionary {
			get {
				if (sessionDictionary == null)
					sessionDictionary = new CurrentUserSessionBasedWorkSessionDictionary();
				return sessionDictionary;
			}
		}
		public virtual Guid GetOpenedWorkSessionID(Guid myCurrentSessionID, string documentPathOrID) {
			Guid documentSessionID_fromAnyoneSessions = WorkSessions.GetDocumentWorkSessionID(documentPathOrID);
			bool itsMySessionID = false;
			lock (locker)
				itsMySessionID = CurrentUserWorkSessions.WorkSessions.ContainsKey(documentSessionID_fromAnyoneSessions);
			if (itsMySessionID)
				return documentSessionID_fromAnyoneSessions;
			bool documentIsAnothers = documentSessionID_fromAnyoneSessions != Guid.Empty && documentSessionID_fromAnyoneSessions != myCurrentSessionID;
			if (documentIsAnothers)
				throw new DocumentAlreadyOpenedException();
			return Guid.Empty;
		}
		public virtual void CloseCustomerWorkSessions(HttpSessionState session) {
			SessionDictCore currentWorkSessions = CurrentUserWorkSessions.GetCurrentWorkSessions(session);
			foreach (Guid workSessionID in currentWorkSessions.Keys.ToList()) {
				WorkSessions.CloseWorkSession(workSessionID);
				currentWorkSessions.Remove(workSessionID); 
			}
		}
	}
	public class MultiCustomerCollaborationStrategy : ICustomerCustomersInteractionStrategy {
		WorkSessionDictionary sessionDictionary;
		public WorkSessionDictionary WorkSessionDictionary {
			get {
				if (sessionDictionary == null)
					sessionDictionary = new WorkSessionDictionary();
				return sessionDictionary;
			}
		}
		public bool CanReuseWorkSession {
			get { return false; }
		}
		public Guid GetOpenedWorkSessionID(Guid sessionGuid, string documentPathOrID) {
			Guid documentSessionID = WorkSessions.GetDocumentWorkSessionID(documentPathOrID);
			return documentSessionID;
		}
		public void CloseCustomerWorkSessions(HttpSessionState session) { }
	}
	class CurrentUserSessionBasedWorkSessionDictionary : WorkSessionDictionary {
		protected override void OnSetting(Guid key, WorkSessionBase value) {
			if (CurrentUserWorkSessions.WorkSessions != null)
				CurrentUserWorkSessions.WorkSessions[key] = value;
		}
		protected override void OnAdding(Guid key, WorkSessionBase value) {
			if (CurrentUserWorkSessions.WorkSessions != null)
				CurrentUserWorkSessions.WorkSessions.Add(key, value);
		}
		protected override void OnRemoving(Guid key) {
			if (CurrentUserWorkSessions.WorkSessions != null)
				CurrentUserWorkSessions.WorkSessions.Remove(key);
		}
		protected override void OnClearing() {
			if (CurrentUserWorkSessions.WorkSessions != null)
				CurrentUserWorkSessions.WorkSessions.Clear();
		}
	}
	static class CurrentUserWorkSessions {
		const string sessionKeyName = "dxssUserSessions";
		public static SessionDictCore WorkSessions { 
			get {
				if (HttpContext.Current != null)
					return GetCurrentWorkSessions(HttpContext.Current.Session);
				return null;
			}
		}
		public static SessionDictCore GetCurrentWorkSessions(HttpSessionState session) {
			if (session[sessionKeyName] == null)
				session[sessionKeyName] = new SessionDictCore();
			return (SessionDictCore)session[sessionKeyName];
		}
	}
}
