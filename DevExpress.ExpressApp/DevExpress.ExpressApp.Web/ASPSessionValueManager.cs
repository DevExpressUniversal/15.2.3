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
using System.Web;
using System.Web.SessionState;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web {
	public class ASPSessionValueManager<ValueType> : ASPSessionValueManagerBase, IValueManager<ValueType> {
		private string sessionEntryName;
		private HttpSessionState GetSession() {
			HttpContext context = HttpContext.Current;
			if(context == null) {
				return DisposedSessionSession;
			}
			return context.Session;
		}
		public ASPSessionValueManager(string key) {
			sessionEntryName = key;
		}
		private string GetSessionID() {
			HttpSessionState session = GetSession();
			if(session != null) {
				return session.SessionID;
			}
			return "";
		}
		public ValueType Value {
			get {
				string sessionID = GetSessionID();
				Dictionary<string, object> sessionStore;
				object value;
				if(!string.IsNullOrEmpty(sessionID) && store.TryGetValue(sessionID, out sessionStore) && sessionStore.TryGetValue(sessionEntryName, out value)) {
					return (ValueType)value;
				}
				return default(ValueType);
			}
			set {
				string sessionID = GetSessionID();
				if(!string.IsNullOrEmpty(sessionID)) {
					Dictionary<string, object> sessionStore;
					if(!store.TryGetValue(sessionID, out sessionStore)) {
						sessionStore = new Dictionary<string, object>();
						store[sessionID] = sessionStore;
					}
					sessionStore[sessionEntryName] = value;
				}
				else {
					throw new InvalidOperationException("HttpContext.Current.Session is null");
				}
			}
		}
		public bool CanManageValue {
			get {
				if(DisposedSessionSession != null) {
					return true;
				}
				HttpContext context = HttpContext.Current;
				if(context == null) {
					return false;
				}
				if(context.Session == null) {
					return false;
				}
				return true;
			}
		}
		public void Clear() {
			Value = default(ValueType);
		}
	}
}
