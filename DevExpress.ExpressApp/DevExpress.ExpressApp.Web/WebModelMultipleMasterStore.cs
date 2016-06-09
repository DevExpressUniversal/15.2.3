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
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.Web {
	public class WebModelMultipleMasterStore : ModelMultipleMasterStore {
		private const string WebModelMultipleMasterStoreName = "MultipleMasterStore";
		private Dictionary<ModelNode, ModelMultipleMasterStoreItem> testItems;
		private static object SessionModelsLockObject = new object();
		private static Dictionary<string, Dictionary<ModelNode, ModelMultipleMasterStoreItem>> SessionModels = new Dictionary<string, Dictionary<ModelNode, ModelMultipleMasterStoreItem>>();
		public WebModelMultipleMasterStore() {
			this.testItems = new Dictionary<ModelNode, ModelMultipleMasterStoreItem>();
		}
		public static void RemoveMasterStore(HttpSessionState session) {
			lock(SessionModelsLockObject)
				if(SessionModels.ContainsKey(session.SessionID))
					SessionModels.Remove(session.SessionID);
		}
		protected Dictionary<ModelNode, ModelMultipleMasterStoreItem> TestItems {
			get {
				return testItems;
			}
		}
		protected override Dictionary<ModelNode, ModelMultipleMasterStoreItem> MasterStore {
			get {
				HttpContext context = HttpContext.Current;
				HttpSessionState session = context != null ? context.Session : null;
				if(session != null) {
					Dictionary<ModelNode, ModelMultipleMasterStoreItem> result = null;
					lock(SessionModelsLockObject)
						if(SessionModels.ContainsKey(session.SessionID))
							result = SessionModels[session.SessionID];
						else {
							result = new Dictionary<ModelNode, ModelMultipleMasterStoreItem>();
							SessionModels[session.SessionID] = result;
						}
					return result;
				}
				return testItems;
			}
		}
	}
}
