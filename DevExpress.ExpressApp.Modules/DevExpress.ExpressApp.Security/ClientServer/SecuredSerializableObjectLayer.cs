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
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.DB;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public static class SecuredSerializableObjectLayerHelper {
		public static void CreateObjectTypeRecords(IDataLayer dataLayer) {
			Session session = new Session(dataLayer);
			Console.WriteLine("Creating XPObjectType records...");
			List<XPClassInfo> list = new List<XPClassInfo>();
			foreach(XPClassInfo ci in dataLayer.Dictionary.Classes) {
				if(ci.IsPersistent) {
					list.Add(ci);
				}
			}
			session.CreateObjectTypeRecords(list.ToArray());
		}
		public static void CreateObjectTypeRecords(string connectionString) {
			CreateObjectTypeRecords(XpoDefault.GetDataLayer(
				connectionString, DevExpress.ExpressApp.Xpo.XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary, AutoCreateOption.SchemaAlreadyExists));
		}
	}
	public class SecuredSerializableObjectLayer : SecuredSerializableObjectLayerBase {
		private readonly bool allowICommandChannelDoWithSecurityContext;
		private readonly IDataLayer dataLayer; 
		private readonly IRequestSecurityStrategyProvider securityStrategyProvider;
		internal SerializableObjectLayer GetSerializableObjectLayerCore(IClientInfo clientInfo, out UnitOfWork result_parentSession, bool allowICommandChannelDoWithSecurityContext) {
			UnitOfWork parentSession;
			ISecurityRule securityRule;
			SerializableObjectLayer layer = CreateSerializableObjectLayerCore(clientInfo, dataLayer, securityStrategyProvider, out parentSession, out securityRule, allowICommandChannelDoWithSecurityContext);
			result_parentSession = parentSession;
			return layer;
		}
		protected virtual SerializableObjectLayer CreateSerializableObjectLayerCore(IClientInfo clientInfo, IDataLayer dataLayer, IRequestSecurityStrategyProvider securityStrategyProvider, out UnitOfWork result_parentSession, out ISecurityRule securityRule, bool allowICommandChannelDoWithSecurityContext) {
			ISelectDataSecurity security = new EmptySelectDataSecurity();
			ISelectDataSecurityProvider selectSecurityProvider = securityStrategyProvider.CreateAndLogonSecurity(clientInfo);
			if(selectSecurityProvider != null) {
				security = selectSecurityProvider.CreateSelectDataSecurity();
			}
			UnitOfWork directSession = new UnitOfWork(dataLayer);
			SecurityRuleProvider securityRuleProvider = new SecurityRuleProvider(dataLayer.Dictionary, security);
			SecuredSessionObjectLayer sessionObjectLayer = new SecuredSessionObjectLayer(allowICommandChannelDoWithSecurityContext, directSession, true, null, securityRuleProvider, null);
			result_parentSession = new UnitOfWork(sessionObjectLayer);
			securityRule = securityRuleProvider.securityRule;
			return new SerializableObjectLayer(result_parentSession, true);
		}
		protected override void FinalizeSession(IClientInfo clientInfo) {
		}
		protected override SerializableObjectLayer GetSerializableObjectLayer(IClientInfo clientInfo, out UnitOfWork result_parentSession) {
			return GetSerializableObjectLayerCore(clientInfo, out result_parentSession, allowICommandChannelDoWithSecurityContext);
		}
		public SecuredSerializableObjectLayer(IDataLayer dataLayer, IRequestSecurityStrategyProvider securityStrategyProvider, bool allowICommandChannelDoWithSecurityContext) {
			Guard.ArgumentNotNull(dataLayer, "dataLayer");
			Guard.ArgumentNotNull(securityStrategyProvider, "securityStrategyProvider");
			this.dataLayer = dataLayer;
			this.securityStrategyProvider = securityStrategyProvider;
			this.allowICommandChannelDoWithSecurityContext = allowICommandChannelDoWithSecurityContext;
		}
		public SecuredSerializableObjectLayer(IDataLayer dataLayer, IRequestSecurityStrategyProvider securityStrategyProvider, TimeSpan sessionTimeout)
			: this(dataLayer, securityStrategyProvider, false) {
		}
		public SecuredSerializableObjectLayer(string connectionString, XPDictionary dictionary, IRequestSecurityStrategyProvider securityStrategyProvider)
			: this(XpoDefault.GetDataLayer(connectionString, dictionary, AutoCreateOption.SchemaAlreadyExists), securityStrategyProvider, false) {
		}
		[Obsolete("Use 'SecuredSerializableObjectLayerHelper.CreateObjectTypeRecords' instead.")]
		public static void CreateObjectTypeRecords(IDataLayer dataLayer) {
			SecuredSerializableObjectLayerHelper.CreateObjectTypeRecords(dataLayer);
		}
		[Obsolete("Use 'SecuredSerializableObjectLayerHelper.CreateObjectTypeRecords' instead.")]
		public static void CreateObjectTypeRecords(string connectionString) {
			SecuredSerializableObjectLayerHelper.CreateObjectTypeRecords(connectionString);
		}
	}
}
