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
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Design {
	public class EFDesignTimeTypeInfoHelper {
		public const string DbContextTypeName = "System.Data.Entity.DbContext";
		public const string ObjectContextTypeName = "System.Data.Entity.Core.Objects.ObjectContext";
		public static bool IsEntityContextType(Type dbContextType) {
			Type baseType = dbContextType.BaseType;
			while(baseType != null) {
				if(baseType.FullName == DbContextTypeName) {
					return true;
				}
				if(baseType.FullName == ObjectContextTypeName) {
					return true;
				}
				baseType = baseType.BaseType;
			}
			return false;
		}
		public static bool IsEntitySetType(Type type) {
			Type baseType = type;
			while(baseType != null) {
				if(baseType.FullName.StartsWith("System.Data.Entity.DbSet")) {
					return true;
				}
				if(baseType.FullName.StartsWith("System.Data.Entity.Core.Objects.ObjectSet")) {
					return true;
				}
				if(baseType.FullName.StartsWith("System.Data.Entity.Core.Objects.IObjectSet")) {
					return true;
				}
				baseType = baseType.BaseType;
			}
			return false;
		}
		public static IEnumerable<Type> GetEntityContextTypes(ITypeDiscoveryService typeDiscoveryService) {
			List<Type> contextTypes = new List<Type>();
			if(typeDiscoveryService != null) {
				foreach(Type localType in typeDiscoveryService.GetTypes(typeof(object), true)) {
					if(IsEntityContextType(localType)) {
						contextTypes.Add(localType);
					}
				}
			}
			return contextTypes;
		}
		public static void ForceInitialize(ITypeDiscoveryService typeDiscoveryService) {
			IEnumerable<Type> contextTypes = GetEntityContextTypes(typeDiscoveryService);
			ForceInitialize(contextTypes);
		}
		public static void ForceInitialize(IEnumerable<Type> contextTypes) {
			TypesInfo typesInfo = XafTypesInfo.Instance as TypesInfo;
			if(typesInfo == null) return;
			lock(typesInfo) {
				InitTypesInfo(typesInfo, contextTypes);
				EFDesignTimeTypeInfoSource efTypeInfoSource = null;
				foreach(IEntityStore entityStore in typesInfo.EntityStores) {
					if((entityStore is EFDesignTimeTypeInfoSource)) {
						efTypeInfoSource = entityStore as EFDesignTimeTypeInfoSource;
					}
				}
				if(efTypeInfoSource == null) {
					efTypeInfoSource = CreateEntityStore(typesInfo, contextTypes);
					typesInfo.AddEntityStore(efTypeInfoSource);
				}
				foreach(Type contextType in contextTypes) {
					efTypeInfoSource.LoadTypesFromContext(contextType);
				}
			}
		}
		public static EFDesignTimeTypeInfoSource CreateEntityStore(ITypesInfo typesInfo, IEnumerable<Type> contextTypes) {
			EFDesignTimeTypeInfoSource efTypeInfoSource = new EFDesignTimeTypeInfoSource(typesInfo);
			foreach(Type contextType in contextTypes) {
				efTypeInfoSource.LoadTypesFromContext(contextType);
			}
			return efTypeInfoSource;
		}
		public static EFDesignTimeTypeInfoSource CreateEntityStore(ITypesInfo typesInfo, ITypeDiscoveryService typeDiscoveryService) {
			IEnumerable<Type> contextTypes = GetEntityContextTypes(typeDiscoveryService);
			return CreateEntityStore(typesInfo, contextTypes);
		}
		public static void InitTypesInfo(ITypesInfo typesInfo, IEnumerable<Type> contextTypes) {
			if(contextTypes != null && typesInfo != null) {
				foreach(Type type in contextTypes) {
					typesInfo.RefreshInfo(type);
				}
			}
		}
		public static void InitTypesInfo(ITypesInfo typesInfo, ITypeDiscoveryService typeDiscoveryService) {
			if(typeDiscoveryService != null && typesInfo != null) {
				IEnumerable<Type> contextTypes = GetEntityContextTypes(typeDiscoveryService);
				InitTypesInfo(typesInfo, contextTypes);
			}
		}
	}
}
