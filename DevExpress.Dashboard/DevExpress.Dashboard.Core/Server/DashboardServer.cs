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
using DevExpress.Utils.Controls;
namespace DevExpress.DashboardCommon.Server {
	public class DashboardServer : IDashboardServer, IDisposable {
		protected class SessionRepositoryRecord {
			public DashboardSession Session { get; set; }
			public DateTime LastAccessTime { get; set; }
			public TimeSpan Timeout { get; set; }
		}
		readonly Dictionary<Guid, SessionRepositoryRecord> repository = new Dictionary<Guid, SessionRepositoryRecord>();
		readonly object repositorySyncObject = new object();
		protected virtual DateTime CurrentTime { get { return DateTime.Now; } }
		protected SessionRepositoryRecord this[Guid sessionId] { get { return repository[sessionId]; } }
		public void Dispose() {
			lock (repositorySyncObject) {
				List<DashboardSession> sessionsToDestroy = new List<DashboardSession>();
				foreach (SessionRepositoryRecord record in repository.Values)
					sessionsToDestroy.Add(record.Session);
				repository.Clear();
				DestroySessions(sessionsToDestroy);
			}
		}
		SessionRepositoryRecord AddSession(Guid sessionId, TimeSpan timeout) {
			SessionRepositoryRecord record = new SessionRepositoryRecord {
				Session = new DashboardSession(),
				LastAccessTime = CurrentTime,
				Timeout = timeout
			};
			repository[sessionId] = record;
			return record;
		}
		void PerformTimeout() {
			IEnumerable<KeyValuePair<Guid, SessionRepositoryRecord>> sessionsToRemove = repository
				.Where(pair => {
					SessionRepositoryRecord record = pair.Value;
					return record.Timeout <= TimeSpan.Zero || CurrentTime - record.LastAccessTime >= record.Timeout;
				}).ToList();
			foreach (var pair in sessionsToRemove)
				repository.Remove(pair.Key);
			DestroySessions(sessionsToRemove.Select(p => p.Value.Session));
		}
		void DestroySessions(IEnumerable<DashboardSession> sessions) {
			foreach(DashboardSession session in sessions)
				session.Dispose();
		}
		#region IDashboardServer
		DashboardServerResult IDashboardServer.GetSession(Guid? sessionId, TimeSpan timeout) {
			lock (repositorySyncObject) {
				PerformTimeout();
				Guid id;
				SessionRepositoryRecord record;
				if (sessionId == null) {
					id = Guid.NewGuid();
					record = AddSession(id, timeout);
					return new DashboardServerResult(id, record.Session);
				}
				else {
					id = sessionId.Value;
					if (repository.ContainsKey(id))
						record = repository[id];
					else
						record = AddSession(id, timeout);
					record.LastAccessTime = CurrentTime;
					return new DashboardServerResult(id, record.Session);
				}
			}
		}
		#endregion
	}
}
