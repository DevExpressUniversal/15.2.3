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
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace DevExpress.Persistent.Base {
	public class SimpleTypes {
		static List<Type> registeredTypes = new List<Type>();
		static SimpleTypes() {
			registeredTypes.Add(typeof(String));
			registeredTypes.Add(typeof(DateTime));
			registeredTypes.Add(typeof(Int16));
			registeredTypes.Add(typeof(Int32));
			registeredTypes.Add(typeof(Int64));
			registeredTypes.Add(typeof(UInt16));
			registeredTypes.Add(typeof(UInt32));
			registeredTypes.Add(typeof(UInt64));
			registeredTypes.Add(typeof(Single));
			registeredTypes.Add(typeof(Double));
			registeredTypes.Add(typeof(Char));
			registeredTypes.Add(typeof(Boolean));
			registeredTypes.Add(typeof(Decimal));
			registeredTypes.Add(typeof(Enum));
			registeredTypes.Add(typeof(Guid));
			registeredTypes.Add(typeof(TimeSpan));
			registeredTypes.Add(typeof(System.Drawing.Image));
		}
		public static bool IsSimpleType(string typeName) {
			return IsSimpleType(ReflectionHelper.FindType(typeName));
		}
		public static bool IsSimpleType(Type type) {
			if(registeredTypes.Contains(type)) {
				return true;
			}
			else {
				if(type != null) {
					return type.IsValueType || (type.IsEnum && type.IsGenericType);
				}
				else {
					Tracing.Tracer.LogWarning("The '{0}' type is not found", type.FullName);
				}
			}
			return false;
		}
		public static bool IsClass(Type type) {
			return type.IsClass && !IsSimpleType(type.FullName);
		}
		public static ReadOnlyCollection<Type> GetSimpleTypes {
			get {
				return registeredTypes.AsReadOnly();
			}
		}
	}
}
