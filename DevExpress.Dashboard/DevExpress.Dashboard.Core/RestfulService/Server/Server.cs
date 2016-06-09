#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {
	public abstract class Server<TSessionKey, TSession> : IDisposable {
		static TimeSpan DefaultSessionTimeout = TimeSpan.FromMinutes(5);
		readonly object repositorySyncObject = new object();
		readonly Dictionary<string, SessionRepositoryRecord<TSession>> repository = new Dictionary<string, SessionRepositoryRecord<TSession>>();
		protected virtual DateTime CurrentTime { get { return DateTime.Now; } }
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				lock (repositorySyncObject) {
					CloseSessions(repository);
					repository.Clear();
				}
			}
		}
		protected virtual void OpenSession(TSession session) {
		}
		protected virtual void CloseSession(TSession session) {
			IDisposable sessionDisp = session as IDisposable;
			if (sessionDisp != null)
				sessionDisp.Dispose();
		}
		protected abstract TSession CreateSessionInstance(TSessionKey key);
		protected ServerResult<TSession> GetSession(TSessionKey key) {
			return GetSession(key, DefaultSessionTimeout);
		}
		protected ServerResult<TSession> GetSession(TSessionKey key, TimeSpan timeout) {
			lock (repositorySyncObject) {
				PerformTimeout();
				SessionRepositoryRecord<TSession> record = null;
				string sessionId = key.ToString();
				if (repository.ContainsKey(sessionId))
					record = repository[sessionId];
				else {
					TSession session = CreateSessionInstance(key);
					OpenSession(session);
					record = new SessionRepositoryRecord<TSession> {
						Session = session,
						LastAccessTime = CurrentTime,
						Timeout = timeout
					};
					repository[sessionId] = record;
				}
				record.LastAccessTime = CurrentTime;
				return new ServerResult<TSession>(sessionId, record.Session);
			}
		}
		void PerformTimeout() {
			var sessionsToRemove = repository
				.Where(pair => pair.Value.Timeout <= TimeSpan.Zero || CurrentTime - pair.Value.LastAccessTime >= pair.Value.Timeout)
				.ToList();
			CloseSessions(sessionsToRemove);
			foreach (var pair in sessionsToRemove)
				repository.Remove(pair.Key);
		}
		void CloseSessions(IEnumerable<KeyValuePair<string, SessionRepositoryRecord<TSession>>> pairs) {
			foreach (TSession session in pairs.Select(pair => pair.Value.Session))
				CloseSession(session);
		}
		#region IDisposable
		void IDisposable.Dispose() {
			Dispose(true);
		}
		#endregion
	}
}
