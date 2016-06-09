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
using System.ServiceModel;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Service.Native.Services {
	public class PollingService : IPollingService {
		readonly TimeSpan timeout = TimeSpan.FromSeconds(10);
		readonly IThreadFactoryService threadFactoryService;
		protected virtual TimeSpan Timeout {
			get { return timeout; }
		}
		public PollingService(IThreadFactoryService threadFactoryService) {
			Guard.ArgumentNotNull(threadFactoryService, "threadFactoryService");
			this.threadFactoryService = threadFactoryService;
		}
		#region ITimeoutService Members
		public virtual void Do(Func<bool> conditionCheck) {
			Do(Timeout, conditionCheck);
		}
		public virtual void Do(TimeSpan timeout, Func<bool> conditionCheck) {
			Guard.ArgumentNotNull(conditionCheck, "conditionCheck");
			var start = DateTime.Now;
			while(true) {
				threadFactoryService.Sleep();
				if(conditionCheck()) {
					break;
				}
				if(DateTime.Now - start > timeout) {
					throw new FaultException(Messages.Timeout);
				}
			}
		}
		#endregion
	}
}
