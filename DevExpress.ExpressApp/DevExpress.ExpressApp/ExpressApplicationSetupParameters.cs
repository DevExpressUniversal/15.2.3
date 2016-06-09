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
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Security;
namespace DevExpress.ExpressApp {
	public sealed class ExpressApplicationSetupParameters {
		private List<IObjectSpaceProvider> objectSpaceProviders;
		public ISecurityStrategyBase Security;
		public ControllersManager ControllersManager;
		public ModuleList Modules;
		public String ApplicationName;
		public String ConnectionString;
		public IEnumerable<Type> DomainComponents;
		public ExpressApplicationSetupParameters(String applicationName, IObjectSpaceProvider objectSpaceProvider, ControllersManager controllersManager, ModuleList modules)
			: this(applicationName, new SecurityDummy(), objectSpaceProvider, controllersManager, modules) {
		}
		public ExpressApplicationSetupParameters(String applicationName, ISecurityStrategyBase security, IObjectSpaceProvider objectSpaceProvider, ControllersManager controllersManager, ModuleList modules)
			: this(applicationName, security, new IObjectSpaceProvider[] { objectSpaceProvider }, controllersManager, modules) {
		}
		public ExpressApplicationSetupParameters(String applicationName, ISecurityStrategyBase security, IList<IObjectSpaceProvider> objectSpaceProviders, ControllersManager controllersManager, ModuleList modules) {
			this.Security = security;
			this.ApplicationName = applicationName;
			this.ControllersManager = controllersManager;
			this.Modules = modules;
			this.objectSpaceProviders = new List<IObjectSpaceProvider>();
			if(objectSpaceProviders != null) {
				this.objectSpaceProviders.AddRange(objectSpaceProviders);
			}
			DomainComponents = Type.EmptyTypes;
		}
		public IObjectSpaceProvider ObjectSpaceProvider {
			get {
				if(objectSpaceProviders.Count > 0) {
					return objectSpaceProviders[0];
				}
				else {
					return null;
				}
			}
			set {
				if(objectSpaceProviders.Count == 1) {
					objectSpaceProviders.Clear();
					objectSpaceProviders.Add(value);
				}
				else {
					objectSpaceProviders.Remove(value);
					objectSpaceProviders.Insert(0, value);
				}
			}
		}
		public List<IObjectSpaceProvider> ObjectSpaceProviders {
			get { return objectSpaceProviders; }
		}
		public ExpressApplicationSetupParameters Clone() {
			return this.MemberwiseClone() as ExpressApplicationSetupParameters;
		}
	}
}
