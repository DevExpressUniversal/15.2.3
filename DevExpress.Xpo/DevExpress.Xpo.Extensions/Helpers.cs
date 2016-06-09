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
using System.Data.Services.Providers;
using DevExpress.Xpo.Metadata;
using System.Reflection;
namespace DevExpress.Xpo.Helpers {
	public static class ODataHelpers {
		public static object GetPropertyValue(XpoMetadata metadata, object target, ResourceProperty resourceProperty) {
			if (target == null) return null;
			ResourceType resType;
			if (metadata.TryResolveResourceTypeByType(target.GetType(), out resType)) {
				XPMemberInfo mi = resType.GetAnnotation().ClassInfo.GetMember(resourceProperty.Name);
				if (mi == null) throw new NotSupportedException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.UnknownResourceType, target.GetType()));
				object value = (mi.Converter != null) ? mi.Converter.ConvertToStorageType(mi.GetValue(target)) : mi.GetValue(target);
				if (value != null && resourceProperty.ResourceType.InstanceType.IsGenericType && resourceProperty.ResourceType.InstanceType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
					value = Convert.ChangeType(value, resourceProperty.ResourceType.InstanceType.GetGenericArguments()[0]);
				}
				return value;
			}
			StructWrapperInfo structWrapper;
			if (metadata.TryResolveStructWrapper(target.GetType(), out structWrapper)) {
				return structWrapper.GetValue(target, resourceProperty.Name);
			}
			throw new NotSupportedException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.UnknownResourceType, target.GetType()));
		}
		public static object GetPropertyValue(XpoMetadata metadata, object target, string propertyName) {			
			ResourceProperty resProp;
			if (metadata.TryResolveResourceProperty(target.GetType(), propertyName, out resProp))
				return GetPropertyValue(metadata, target, resProp);
			else
				return null;
		}
	}
}
