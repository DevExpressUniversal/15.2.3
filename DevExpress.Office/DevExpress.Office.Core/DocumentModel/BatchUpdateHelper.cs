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
using DevExpress.Utils;
namespace DevExpress.Office {
	#region BatchUpdateHelper<T>
	public class BatchUpdateHelper<T> : BatchUpdateHelper {
		T deferredNotifications;
		bool fakeAssignDetected;
		int suppressDirectNotificationsCount;
		int suppressIndexRecalculationOnEndInitCount;
		public BatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public T DeferredNotifications { get { return deferredNotifications; } set { deferredNotifications = value; } }
		public bool FakeAssignDetected { get { return fakeAssignDetected; } set { fakeAssignDetected = value; } }
		public bool IsDirectNotificationsEnabled { get { return suppressDirectNotificationsCount == 0; } }
		public void SuppressDirectNotifications() {
			suppressDirectNotificationsCount++;
		}
		public void ResumeDirectNotifications() {
			suppressDirectNotificationsCount--;
		}
		public bool IsIndexRecalculationOnEndInitEnabled { get { return suppressIndexRecalculationOnEndInitCount == 0; } }
		public void SuppressIndexRecalculationOnEndInit() {
			suppressIndexRecalculationOnEndInitCount++;
		}
		public void ResumeIndexRecalculationOnEndInit() {
			suppressIndexRecalculationOnEndInitCount--;
		}
	}
	public class BatchInitHelper<T> : BatchUpdateHelper<T> {
		class InnerBatchUpdateHandler : IBatchUpdateHandler {
			IBatchInitHandler batchInitHandler;
			public InnerBatchUpdateHandler(IBatchInitHandler batchInitHandler) {
				Guard.ArgumentNotNull(batchInitHandler, "batchInitHandler");
				this.batchInitHandler = batchInitHandler;
			}
			public IBatchInitHandler BatchInitHandler { get { return batchInitHandler; } }
			#region IBatchUpdateHandler Members
			public void OnBeginUpdate() {
				batchInitHandler.OnBeginInit();
			}
			public void OnCancelUpdate() {
				batchInitHandler.OnCancelInit();
			}
			public void OnEndUpdate() {
				batchInitHandler.OnEndInit();
			}
			public void OnFirstBeginUpdate() {
				batchInitHandler.OnFirstBeginInit();
			}
			public void OnLastCancelUpdate() {
				batchInitHandler.OnLastCancelInit();
			}
			public void OnLastEndUpdate() {
				batchInitHandler.OnLastEndInit();
			}
			#endregion
		}
		public BatchInitHelper(IBatchInitHandler handler)
			: base(new InnerBatchUpdateHandler(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((InnerBatchUpdateHandler)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region BatchUpdateHelper<TInfo, TOptions>
	public class BatchUpdateHelper<TInfo, TOptions> : BatchUpdateHelper {
		TInfo deferredInfoNotifications;
		TOptions deferredOptionsNotifications;
		public BatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public TInfo DeferredInfoNotifications { get { return deferredInfoNotifications; } set { deferredInfoNotifications = value; } }
		public TOptions DeferredOptionsNotifications { get { return deferredOptionsNotifications; } set { deferredOptionsNotifications = value; } }
	}
	#endregion
	#region BatchInitAdapter
	public class BatchInitAdapter : IBatchUpdateHandler {
		readonly IBatchInitHandler batchInitHandler;
		public BatchInitAdapter(IBatchInitHandler batchInitHandler) {
			Guard.ArgumentNotNull(batchInitHandler, "batchInitHandler");
			this.batchInitHandler = batchInitHandler;
		}
		public IBatchInitHandler BatchInitHandler { get { return batchInitHandler; } }
		#region IBatchUpdateHandler Members
		public void OnBeginUpdate() {
			batchInitHandler.OnBeginInit();
		}
		public void OnCancelUpdate() {
			batchInitHandler.OnCancelInit();
		}
		public void OnEndUpdate() {
			batchInitHandler.OnEndInit();
		}
		public void OnFirstBeginUpdate() {
			batchInitHandler.OnFirstBeginInit();
		}
		public void OnLastCancelUpdate() {
			batchInitHandler.OnLastCancelInit();
		}
		public void OnLastEndUpdate() {
			batchInitHandler.OnLastEndInit();
		}
		#endregion
	}
	#endregion
}
