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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.SystemModule;
using System.Reflection;
using DevExpress.ExpressApp.Security.ClientServer;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Security.Adapters {
	public enum ReloadPermissionStrategy { None, NoCache, CacheOnFirstAccess }
	public interface ISecurityAdapter {
		bool IsGranted(IRequestSecurity requestSecurity, IObjectSpace objectSpace, Type objectType, object targetObject, string memberName, string operation, out bool result);
	}
	public interface ISupportExternalRequestCache {
		void SetRequestCache(Dictionary<string, bool> requestCache);
	}
	public interface ISupportLogonEvent {
		event EventHandler<LoggingOnEventArgs> LoggingOn;
		event EventHandler<EventArgs> LoggingOff;
	}
	public interface ISecurityAdapterProvider {
		int GetScopeKey(IObjectSpace objectSpace);
		ISecurityAdapter CreateSecurityAdapter();
		bool CanHandle(IObjectSpace objectSpace);
	}
	public interface ISupportObjectSpaceProvider {
		IObjectSpaceProvider ObjectSpaceProvider { get; set; }
	}
	public interface ISupportPermissions {
		IPermissionDictionary PermissionDictionary { get; set; }
	}
	public interface ISupportCreatePermissions {
		IPermissionDictionary CreatePermissionDictionary(IObjectSpace objectSpace, Type userType, object userID);
	}
	public static class IsGrantedAdapter {
#if DebugTest
		public static ISecurityAdapterProvider GetHandleSecurityAdapterProviderTest(IObjectSpace objectSpace, IList<ISecurityAdapterProvider> securityAdapterProviders) {
			return FindHandleSecurityAdapterProvider(objectSpace, securityAdapterProviders);
		}
#endif
		private static ReloadPermissionStrategy reloadPermissionStrategy = ReloadPermissionStrategy.None;
		private static bool isInitialized = false;
		private static object lockObject = new object();
		private static Dictionary<int, ISecurityAdapter> securityAdapterDictionary = new Dictionary<int, ISecurityAdapter>();
		private static List<ISecurityAdapterProvider> SecurityAdapterProviders = new List<ISecurityAdapterProvider>();
		private static IPermissionDictionary LogonPermissions {
			get {
				return ValueManager.GetValueManager<IPermissionDictionary>("Logon_IPermissionDictionary").Value;
			}
			set {
				ValueManager.GetValueManager<IPermissionDictionary>("Logon_IPermissionDictionary").Value = value;
			}
		}
		private static Dictionary<string, bool> LogonRequestCache {
			get {
				return ValueManager.GetValueManager<Dictionary<string, bool>>("Logon_RquestCache").Value;
			}
			set {
				ValueManager.GetValueManager<Dictionary<string, bool>>("Logon_RquestCache").Value = value;
			}
		}
		private static void DelayedInitializeSettings(IRequestSecurity reqestSecurity) {
			if(reloadPermissionStrategy == ReloadPermissionStrategy.CacheOnFirstAccess) {
				ISupportLogonEvent supportLogonEvent = reqestSecurity as ISupportLogonEvent;
				if(supportLogonEvent != null) {
					supportLogonEvent.LoggingOff += supportLogonEvent_LoggingOff;
				}
			}
		}
		private static void supportLogonEvent_LoggingOff(object sender, EventArgs e) {
			ClearPermissionCache();
		}
		private static void CustomHasPermissionTo(object sender, CustomHasPermissionToEventArgs e) {
			bool result;
			IRequestSecurity reqestSecurity = null;
			if(SecuritySystem.Instance != null) {
				reqestSecurity = SecuritySystem.Instance as IRequestSecurity;
			}
			else {
				DevExpress.Persistent.Base.Tracing.Tracer.LogWarning(typeof(IsGrantedAdapter).Name + ".CustomHasPermissionTo - SecuritySystem.Instance is not initialized.");
			}
			if(reqestSecurity == null) {
				DevExpress.Persistent.Base.Tracing.Tracer.LogWarning(typeof(IsGrantedAdapter).Name + ".CustomHasPermissionTo - SecuritySystem.Instance is not support IRequestSecurity interface");
				e.Handled = true;
				e.Result = true;
				return;
			}
			if(SecurityAdapterProviders != null) {
				e.Handled = IsGranted(reqestSecurity, e.ObjectType, e.TargetObject, e.MemberName, e.Operation, e.ObjectSpace, SecuritySystem.UserType, SecuritySystem.CurrentUserId, out result);
				e.Result = result;
			}
			else {
				DevExpress.Persistent.Base.Tracing.Tracer.LogWarning(typeof(IsGrantedAdapter).Name + ".CustomHasPermissionTo - SecurityAdapterProviders is not found. Please make sure that you are using the adapter.");
				e.Handled = false;
			}
		}
		private static IPermissionDictionary GetOrCreatePermissionDictionary(ReloadPermissionStrategy reloadPermissionStrategy, Func<IPermissionDictionary> permissionProvider) {
			IPermissionDictionary result;
			switch(reloadPermissionStrategy) {
				case ReloadPermissionStrategy.None:
					throw new InvalidOperationException();
				case ReloadPermissionStrategy.NoCache:
					result = permissionProvider();
					break;
				case ReloadPermissionStrategy.CacheOnFirstAccess:
					if(LogonPermissions == null) {
						LogonPermissions = permissionProvider();
					}
					result = LogonPermissions;
					break;
				default:
					throw new InvalidOperationException();
			}
			return result;
		}
		private static ISecurityAdapterProvider FindHandleSecurityAdapterProvider(IObjectSpace objectSpace, IList<ISecurityAdapterProvider> securityAdapterProviders) {
			Guard.ArgumentNotNull(objectSpace, "objectSpace");
			Guard.ArgumentNotNull(securityAdapterProviders, "securityAdapterProviders");
			ISecurityAdapterProvider securityAdapterProvider = null;
			foreach(ISecurityAdapterProvider AdapterProvider in securityAdapterProviders) {
				if(AdapterProvider.CanHandle(objectSpace)) {
					securityAdapterProvider = AdapterProvider;
					break;
				}
			}
			return securityAdapterProvider;
		}
		public static ISecurityAdapter GetOrCreateSecurityAdapter(IObjectSpace objectSpace, Type userType, object userID) {
			Guard.ArgumentNotNull(objectSpace, "objectSpace");
			ISecurityAdapterProvider securityAdapterProvider = FindHandleSecurityAdapterProvider(objectSpace, SecurityAdapterProviders);
			if(securityAdapterProvider == null) {
				DevExpress.Persistent.Base.Tracing.Tracer.LogWarning(typeof(IsGrantedAdapter).Name + ".IsGranted - Compatible securityAdapterProvider is not found");
				return null;
			}
			int scopeKey = securityAdapterProvider.GetScopeKey(objectSpace);
			ISecurityAdapter securityAdapter;
			lock(lockObject) {
				if(!securityAdapterDictionary.TryGetValue(scopeKey, out securityAdapter)) {
					securityAdapter = securityAdapterProvider.CreateSecurityAdapter();
					ISupportCreatePermissions supportCreatePermissions = securityAdapterProvider as ISupportCreatePermissions;
					ISupportPermissions supportCachedPermissions = securityAdapter as ISupportPermissions;
					if(supportCreatePermissions != null && supportCachedPermissions != null) {
						IPermissionDictionary permissionDictionary;
						if(reloadPermissionStrategy == ReloadPermissionStrategy.CacheOnFirstAccess) {
							if(LogonPermissions == null) {
								LogonPermissions = supportCreatePermissions.CreatePermissionDictionary(objectSpace, userType, userID);
							}
							permissionDictionary = LogonPermissions;
						}
						else {
							permissionDictionary = supportCreatePermissions.CreatePermissionDictionary(objectSpace, userType, userID);
						}
						if(permissionDictionary == null) {
							return null;
						}
						supportCachedPermissions.PermissionDictionary = permissionDictionary;
					}
					ISupportExternalRequestCache externalRequestCache = securityAdapter as ISupportExternalRequestCache;
					if(externalRequestCache != null && reloadPermissionStrategy == ReloadPermissionStrategy.CacheOnFirstAccess) {
						if(LogonRequestCache == null) {
							LogonRequestCache = new Dictionary<string, bool>();
						}
						externalRequestCache.SetRequestCache(LogonRequestCache);
					}
					new SecurityAdapterDictionaryEntryLifetimeManager(objectSpace, scopeKey, securityAdapterDictionary).Start();
					securityAdapterDictionary.Add(scopeKey, securityAdapter);
				}
			}
			return securityAdapter;
		}
		public static void Enable(ISecurityAdapterProvider securityAdapterProvider) {
			Enable(new[] { securityAdapterProvider });
		}
		public static void Enable(IEnumerable<ISecurityAdapterProvider> securityAdapterProviders) {
			Enable(securityAdapterProviders, ReloadPermissionStrategy.NoCache);
		}
		public static void Enable(ISecurityAdapterProvider securityAdapterProviders, ReloadPermissionStrategy reloadPermissionStrategy) {
			Enable(new[] { securityAdapterProviders }, reloadPermissionStrategy);
		}
		public static void Enable(IEnumerable<ISecurityAdapterProvider> securityAdapterProviders, ReloadPermissionStrategy reloadPermissionStrategy) {
			if(reloadPermissionStrategy != ReloadPermissionStrategy.None && !isInitialized) {
				IsGrantedAdapter.reloadPermissionStrategy = reloadPermissionStrategy;
				SecurityAdapterProviders.AddRange(securityAdapterProviders);
				SecuritySystem.AllowReloadPermissions = false;
				ServerSecurityClient.CanUseCache = true;
				ServerPermissionRequestProcessor.CanUseCache = false;
				SecuritySystem.CustomIsGranted -= CustomHasPermissionTo;
				SecuritySystem.CustomIsGranted += CustomHasPermissionTo;
			}
		}
		public static void DisableRefreshPermissions() {
			IsGrantedAdapter.reloadPermissionStrategy = ReloadPermissionStrategy.None;
			ServerSecurityClient.CanUseCache = false;
			ServerPermissionRequestProcessor.CanUseCache = true;
			SecuredObjectSpaceProvider.AllowReloadPermissions = false;
			SecuritySystem.AllowReloadPermissions = true;
			isInitialized = false;
			SecurityAdapterProviders.Clear();
			LogonRequestCache = null;
			LogonPermissions = null;
			SecuritySystem.CustomIsGranted -= CustomHasPermissionTo;
		}
		public static void ClearPermissionCache() {
			LogonPermissions = null;
			LogonRequestCache = null;
		}
		public static bool IsGranted(IRequestSecurity requestSecurity, Type objectType, object targetObject, string memberName, string operation, IObjectSpace objectSpace, Type userType, object userID, out bool result) {
			result = false;
			if(userID == null || userType == null || objectSpace == null || objectType == null || targetObject is string || requestSecurity == null) {
				return false;
			}
			ITypeInfo targetObjectType = objectSpace.TypesInfo.FindTypeInfo(objectType);
			if(targetObjectType == null || !targetObjectType.IsPersistent) {
				return false;
			}
			if(!isInitialized && reloadPermissionStrategy != ReloadPermissionStrategy.None) {
				DelayedInitializeSettings(requestSecurity);
				isInitialized = true;
			}
			ISecurityAdapter securityAdapter = GetOrCreateSecurityAdapter(objectSpace, userType, userID);
			if(securityAdapter == null) {
				return false;
			}
			return securityAdapter.IsGranted(requestSecurity, objectSpace, objectType, targetObject, memberName, operation, out result);
		}
	}
}
