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

using System.Collections.Generic;
namespace DevExpress.Charts.NotificationCenter.Native {
	[System.Diagnostics.DebuggerStepThrough]
	public class ObserverFilter {
		readonly Dictionary<NotificationFilterCredentials, List<INotificationObserver>> clusters;
		public ObserverFilter() {
			this.clusters = new Dictionary<NotificationFilterCredentials, List<INotificationObserver>>();
		}
		public void Register(NotificationFilterCredentials credentials, INotificationObserver observer) {
			List<INotificationObserver> cluster;
			if (!clusters.TryGetValue(credentials, out cluster)) {
				cluster = new List<INotificationObserver>();
				clusters.Add(credentials, cluster);
			}
			cluster.Add(observer);
		}
		public void Unregister(NotificationFilterCredentials credentials, INotificationObserver observer) {
			List<INotificationObserver> cluster;
			if (clusters.TryGetValue(credentials, out cluster))
				cluster.Remove(observer);
		}
		public IList<INotificationObserver> FindObservers(NotificationFilterCredentials credentials) {
			List<INotificationObserver> cluster;
			if (clusters.TryGetValue(credentials, out cluster))
				return cluster;
			return null;
		}
		public int GetObserversCount() {
			int count = 0;
			foreach (var cluster in clusters)
				count += cluster.Value.Count;
			return count;
		}
	}
}
