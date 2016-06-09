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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using DevExpress.Entity.Model;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Mvvm.UI.Native.ViewGenerator {
	public class ReflectionEntityProperties : IEntityProperties {
		static Type GetUnderlyingType(PropertyDescriptor property) {
			return Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
		}
		readonly IEdmPropertyInfo[] properties;
		public ReflectionEntityProperties(IEnumerable<PropertyDescriptor> properties, Type ownerType, bool includeReadonly = false) {
			PropertyDescriptor[] writeableProperties = properties.Where(x => includeReadonly || !x.IsReadOnly).ToArray();			
			this.properties = writeableProperties
				.Select(x => new EdmPropertyInfo(x,DataColumnAttributesProvider.GetAttributes(x,ownerType), IsNavigationProperty(x), false))
				.ToArray();
		}
		bool IsNavigationProperty(PropertyDescriptor property) {
			Type underlyingType = GetUnderlyingType(property);
			TypeCode typeCode = Type.GetTypeCode(underlyingType);
			return typeCode == TypeCode.Object && !typeof(IEnumerable).IsAssignableFrom(underlyingType);
		}
		IEnumerable<IEdmPropertyInfo> IEntityProperties.AllProperties {
			get { return properties; }
		}
	}
	public sealed class ReflectionEntityTypeInfo : ReflectionEntityProperties, IEntityTypeInfo {
		public IEdmPropertyInfo GetForeignKey(IEdmPropertyInfo dependentProperty) {
			return null;
		}
		public IEdmPropertyInfo GetDependentProperty(IEdmPropertyInfo foreignKey) {
			return null;
		}
		readonly Type entityType;
		public ReflectionEntityTypeInfo(Type entityType, bool includeReadonly = false)
			: base(TypeDescriptor.GetProperties(entityType).Cast<PropertyDescriptor>(), entityType, includeReadonly) {
			this.entityType = entityType;
		}
		IEnumerable<IEdmPropertyInfo> IEntityTypeInfo.KeyMembers {
			get { yield break; }
		}
		Type IEntityTypeInfo.Type {
			get { return entityType; }
		}
		IEnumerable<IEdmAssociationPropertyInfo> IEntityTypeInfo.LookupTables {
			get { yield break; }
		}		
	}
}
