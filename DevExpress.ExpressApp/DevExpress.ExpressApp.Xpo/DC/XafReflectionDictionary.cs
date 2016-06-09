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
using DevExpress.Xpo.Metadata;
namespace DevExpress.ExpressApp.DC.Xpo {
	public class XafReflectionDictionary : ReflectionDictionary {
		private Dictionary<Type, Type> extendedPersistentInterfaces;
		private Boolean IsDomainComponent(Type interfaceType) {
			if((interfaceType == null) || !interfaceType.IsInterface) {
				return false;
			}
			return interfaceType.IsDefined(typeof(DomainComponentAttribute), false);
		}
		internal Boolean IsPersistentInterface(Type interfaceType) {
			if((interfaceType == null) || !interfaceType.IsInterface) {
				return false;
			}
			return extendedPersistentInterfaces.ContainsKey(interfaceType);
		}
		protected override XPClassInfo ResolveClassInfoByType(Type classType) {
			Type resultType;
			if(extendedPersistentInterfaces.TryGetValue(classType, out resultType) && (resultType != null)) {
				return QueryClassInfo(resultType);
			}
			return base.ResolveClassInfoByType(classType);
		}
		protected internal void ExtendPersistentInterfaces(Type interfaceType) {
			ExtendPersistentInterfaces(interfaceType, null);
		}
		protected internal void ExtendPersistentInterfaces(Type interfaceType, Type mapType) {
			if(interfaceType.IsInterface) {
				if(mapType != null || !IsPersistentInterface(interfaceType)) {
					ExtendPersistentInterfacesCore(interfaceType, mapType);
				}
				foreach(Type baseInterface in interfaceType.GetInterfaces()) {
					if(!IsPersistentInterface(baseInterface) && IsDomainComponent(baseInterface)) {
						ExtendPersistentInterfacesCore(baseInterface, null);
					}
				}
			}
		}
		private void ExtendPersistentInterfacesCore(Type interfaceType, Type mapType) {
			classesByName.Remove(interfaceType.FullName);
			classesByType.Remove(interfaceType);
			extendedPersistentInterfaces[interfaceType] = mapType;
		}
		public XafReflectionDictionary() {
			extendedPersistentInterfaces = new Dictionary<Type, Type>();
		}
		public override bool CanGetClassInfoByType(Type classType) {
			if(IsPersistentInterface(classType)) {
				return true;
			}
			return base.CanGetClassInfoByType(classType);
		}
	}
}
