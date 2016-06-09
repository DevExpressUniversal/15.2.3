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
namespace DevExpress.Charts.NotificationCenter.Native {
	public class ObserverManager {
		readonly Dictionary<NotificationFilterLevel, ObserverFilter> filters;
		public ObserverManager() {
			this.filters = new Dictionary<NotificationFilterLevel, ObserverFilter>();
			foreach (NotificationFilterLevel level in Enum.GetValues(typeof(NotificationFilterLevel)))
				this.filters.Add(level, new ObserverFilter());
		}
		public void Register(NotificationFilterCredentials credentials, INotificationObserver observer) {
			filters[credentials.Level].Register(credentials, observer);
		}
		public void Unregister(NotificationFilterCredentials credentials, INotificationObserver observer) {
			filters[credentials.Level].Unregister(credentials, observer);
		}
		public int GetCount(NotificationFilterCredentials credentials) {
			return filters[credentials.Level].FindObservers(credentials).Count;
		}
		public int GetCount(NotificationFilterLevel level) {
			return filters[level].GetObserversCount();
		}
		public void SelectObserver(List<INotificationObserver> observersBuffer, NotificationFilterCredentials credentials) {
			observersBuffer.Clear();
			for (int level = (int)credentials.Level; level >= (int)NotificationFilterLevel.None; level--) {
				NotificationFilterLevel currentLevel = (NotificationFilterLevel)level;
				IList<INotificationObserver> observersAtLevel = filters[currentLevel].FindObservers(credentials.ShiftToLevel(currentLevel));
				if (observersAtLevel != null)
					observersBuffer.AddRange(observersAtLevel);
			}
		}
	}
}
