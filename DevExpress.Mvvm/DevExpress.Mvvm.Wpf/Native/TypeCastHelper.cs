#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using DevExpress.Mvvm.Native;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
#if !NETFX_CORE
using System.Windows.Threading;
#else
using Windows.UI.Xaml;
using Windows.UI.Core;
#endif
namespace DevExpress.Mvvm.Native {
	public static class TypeCastHelper {
		public static object TryCast(object value, Type targetType) {
			Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
#if !NETFX_CORE
			if(underlyingType.IsEnum && value is string) {
#else
			if(underlyingType.IsEnum() && value is string) {
#endif
				value = Enum.Parse(underlyingType, (string)value, false);
			} else if(
#if !NETFX_CORE
				value is IConvertible && 
#else 
				IsConvertableType(value) &&
#endif
				!targetType.IsAssignableFrom(value.GetType())) {
				value = Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
			}
#if !NETFX_CORE
			if(value == null && targetType.IsValueType)
#else
			if(value == null && targetType.IsValueType())
#endif
				value = Activator.CreateInstance(targetType);
			return value;
		}
#if NETFX_CORE
		static readonly Type[] convertableTypes = new Type[] { 
					typeof(System.Decimal), typeof(System.Decimal?),
					typeof(System.Single), typeof(System.Single?),
					typeof(System.Double), typeof(System.Double?),
					typeof(System.Int16), typeof(System.Int16?),
					typeof(System.Int32), typeof(System.Int32?),
					typeof(System.Int64), typeof(System.Int64?),
					typeof(System.UInt16), typeof(System.UInt16?),
					typeof(System.UInt32), typeof(System.UInt32?),
					typeof(System.UInt64), typeof(System.UInt64?),
					typeof(System.Byte), typeof(System.Byte?),
					typeof(System.SByte), typeof(System.SByte?),
					typeof(System.String), typeof(DateTime)
		};
		static bool IsConvertableType(object parameter) {
			return parameter != null && Array.IndexOf<Type>(convertableTypes, parameter.GetType()) >= 0;
		}
#endif
	}
}
