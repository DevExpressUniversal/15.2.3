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
namespace DevExpress.Web.Office.Internal {
	using DevExpress.Web.Internal;
	using WorkSessionFactory = System.Func<System.Guid, DevExpress.Web.Office.Internal.WorkSessionBase>;
	public sealed class WorkSessionFactories {
		class WorkSessionFactoryCollection : System.Collections.Generic.Dictionary<string, WorkSessionFactory> {}
		private static volatile WorkSessionFactories instance;
		private static object syncRoot = new Object();
		private WorkSessionFactoryCollection factories;
		private WorkSessionFactories() { }
		private static WorkSessionFactories Instance {
			get {
				if(instance == null) {
					lock(syncRoot) {
						if(instance == null)
							instance = new WorkSessionFactories();
					}
				}
				return instance;
			}
		}
		private WorkSessionFactoryCollection Factories {
			get { 
				if(factories == null)
					factories = new WorkSessionFactoryCollection();
				return factories; 
			}
		}
		public static void Register(string workSessionTypeName, WorkSessionFactory createNewWorkSession) {
			lock(syncRoot) {
				if(!Instance.Factories.ContainsKey(workSessionTypeName))
					Instance.Factories.Add(workSessionTypeName, createNewWorkSession);
			}
		}
		public static WorkSessionBase Produce(string workSessionTypeName, Guid workSessionId) {
			lock(syncRoot) {
				try{
					var factory = Instance.Factories[workSessionTypeName];
					if(factory != null)
						return factory(workSessionId);
				} catch(Exception exc) {
					CommonUtils.RaiseCallbackErrorInternal(null, exc);
				}
				return null;
			}
		}
	}
	public abstract class WorkSessionRegistrationAttributeBase : Attribute {
		public WorkSessionRegistrationAttributeBase() {
			RegisterWorkSessionFactory();
		}
		protected abstract void RegisterWorkSessionFactory();
	}
}
