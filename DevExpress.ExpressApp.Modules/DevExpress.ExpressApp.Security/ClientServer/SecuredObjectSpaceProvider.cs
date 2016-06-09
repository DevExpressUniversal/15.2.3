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
using System.ComponentModel;
using System.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Security.Adapters;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Utils;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public class SecuredSessionObjectLayer : SessionObjectLayer {
		private bool allowICommandChannelDoWithSecurityContext;
		public SecuredSessionObjectLayer(bool allowICommandChannelDoWithSecurityContext, Session parentSession, bool throughCommitMode, IGenericSecurityRule genericSecurityRule, ISecurityRuleProvider securityDictionary, object securityCustomContext)
			: base(parentSession, throughCommitMode, genericSecurityRule, securityDictionary, securityCustomContext) {
			this.allowICommandChannelDoWithSecurityContext = allowICommandChannelDoWithSecurityContext;
		}
		protected override bool AllowICommandChannelDoWithSecurityContext {
			get {
				return allowICommandChannelDoWithSecurityContext;
			}
		}
	}
	public class SecuredObjectSpaceProvider : XPObjectSpaceProvider {
		[DefaultValue(false)]
		[Browsable(false)]
		public static bool AllowReloadPermissions { get; set; }
		private bool allowICommandChannelDoWithSecurityContext;
		private ISelectDataSecurityProvider selectDataSecurityProvider;
		private void Initialize(ISelectDataSecurityProvider selectDataSecurityProvider) {
			Guard.ArgumentNotNull(selectDataSecurityProvider, "selectDataSecurityProvider");
			this.selectDataSecurityProvider = selectDataSecurityProvider;
		}
		protected override UnitOfWork CreateUnitOfWork(IDataLayer dataLayer) {
			UnitOfWork directBaseUow = new UnitOfWork(dataLayer);
			SecurityRuleProvider securityRuleProvider = new SecurityRuleProvider(XPDictionary, selectDataSecurityProvider.CreateSelectDataSecurity());
			SessionObjectLayer currentObjectLayer = new SecuredSessionObjectLayer(allowICommandChannelDoWithSecurityContext, directBaseUow, true, null, securityRuleProvider, null);
			return new UnitOfWork(currentObjectLayer, directBaseUow);
		}
		public SecuredObjectSpaceProvider(ISelectDataSecurityProvider selectDataSecurityProvider, IXpoDataStoreProvider dataStoreProvider, ITypesInfo typesInfo, XpoTypeInfoSource xpoTypeInfoSource, Boolean threadSafe)
			: base(dataStoreProvider, typesInfo, xpoTypeInfoSource, threadSafe) {
			Initialize(selectDataSecurityProvider);
		}
		public SecuredObjectSpaceProvider(ISelectDataSecurityProvider selectDataSecurityProvider, IXpoDataStoreProvider dataStoreProvider, Boolean threadSafe)
			: base(dataStoreProvider, threadSafe) {
			Initialize(selectDataSecurityProvider);
		}
		public SecuredObjectSpaceProvider(ISelectDataSecurityProvider selectDataSecurityProvider, string databaseConnectionString, IDbConnection connection, Boolean threadSafe)
			: base(databaseConnectionString, connection, threadSafe) {
			Initialize(selectDataSecurityProvider);
		}
		public SecuredObjectSpaceProvider(ISelectDataSecurityProvider selectDataSecurityProvider, IXpoDataStoreProvider dataStoreProvider, ITypesInfo typesInfo, XpoTypeInfoSource xpoTypeInfoSource)
			: this(selectDataSecurityProvider, dataStoreProvider, typesInfo, xpoTypeInfoSource, true) {
		}
		public SecuredObjectSpaceProvider(ISelectDataSecurityProvider selectDataSecurityProvider, IXpoDataStoreProvider dataStoreProvider)
			: this(selectDataSecurityProvider, dataStoreProvider, true) {
		}
		public SecuredObjectSpaceProvider(ISelectDataSecurityProvider selectDataSecurityProvider, string databaseConnectionString, IDbConnection connection)
			: this(selectDataSecurityProvider, databaseConnectionString, connection, true) {
		}
		public bool AllowICommandChannelDoWithSecurityContext {
			get { return allowICommandChannelDoWithSecurityContext; }
			set { allowICommandChannelDoWithSecurityContext = true; }
		}
		protected override IObjectSpace CreateObjectSpaceCore() {
			IObjectSpace objectSpace = null;
			DelayedPermissionsProvider delayedPermissionsProvider = new DelayedPermissionsProvider();
			objectSpace = new XPObjectSpace(TypesInfo, XpoTypeInfoSource, () => CreateExtensionUnitOfWork(delayedPermissionsProvider, DataLayer));
			objectSpace.Reloaded += (s, e) => {
				InitializePermissions(objectSpace, delayedPermissionsProvider);
			};
			InitializePermissions(objectSpace, delayedPermissionsProvider);
			return objectSpace;
		}
		private static void InitializePermissions(IObjectSpace objectSpace, DelayedPermissionsProvider delayedPermissionsProvider) {
			ISecurityAdapter securityAdapter = IsGrantedAdapter.GetOrCreateSecurityAdapter(objectSpace, SecuritySystem.UserType, SecuritySystem.CurrentUserId);
			if(securityAdapter != null && securityAdapter is ISupportPermissions) {
					delayedPermissionsProvider.Permissions = ((ISupportPermissions)securityAdapter).PermissionDictionary;				
			}
		}
		private void OnObjectSpaceCreated(IObjectSpace objectSpace) {
			if(ObjectSpaceCreated != null) {
				ObjectSpaceCreated(objectSpace, EventArgs.Empty);
			}
		}
		private UnitOfWork CreateExtensionUnitOfWork(DelayedPermissionsProvider delayedPermissionsProvider, IDataLayer dataLayer) {
			UnitOfWork directBaseUow = new UnitOfWork(dataLayer);
			SecurityRuleProvider securityRuleProvider = new SecurityRuleProvider(XPDictionary, selectDataSecurityProvider.CreateSelectDataSecurity(), delayedPermissionsProvider);
			SessionObjectLayer currentObjectLayer = new SecuredSessionObjectLayer(allowICommandChannelDoWithSecurityContext, directBaseUow, true, null, securityRuleProvider, null);
			return new UnitOfWork(currentObjectLayer, directBaseUow);
		}
		public event EventHandler<EventArgs> ObjectSpaceCreated;
	}
	public class DelayedPermissionsProvider {
		public IPermissionDictionary Permissions { get; set; }
	}
}
