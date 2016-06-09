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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public static class ObjectHandleHelper {
		private static void ThrowInvalidHandleException(String handle) {
			throw new ArgumentException(String.Format("Invalid handle '{0}'.", handle), "handle");
		}
		public static String CreateObjectHandle(ITypesInfo typesInfo, Type objectType, String objectKeyAsString) {
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			Guard.ArgumentNotNull(objectType, "objectType");
			Guard.ArgumentNotNull(objectKeyAsString, "objectKeyAsString");
			ITypeInfo objectTypeInfo = typesInfo.FindTypeInfo(objectType);
			return String.Format("{0}({1})", objectTypeInfo.FullName, objectKeyAsString);
		}
		public static void ParseObjectHandle(ITypesInfo typesInfo, String handle, out Type objectType, out String objectKeyAsString) {
			Guard.ArgumentNotNullOrEmpty(handle, "handle"); 
			if(!TryParseObjectHandle(typesInfo, handle, out objectType, out objectKeyAsString)) {
				ThrowInvalidHandleException(handle);
			}
		}
		public static bool TryParseObjectHandle(ITypesInfo typesInfo, String handle, out Type objectType, out String objectKeyAsString) {
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			objectType = null;
			objectKeyAsString = null;
			if(String.IsNullOrEmpty(handle)) {
				return false;
			}
			Int32 separatorIndex = handle.IndexOf('(');
			if((separatorIndex <= 0) || (separatorIndex > handle.Length - 2)) {
				return false;
			}
			String objectTypeFullName = handle.Substring(0, separatorIndex);
			ITypeInfo objectTypeInfo = typesInfo.FindTypeInfo(objectTypeFullName);
			if(objectTypeInfo == null) {
				return false;
			}
			objectType = objectTypeInfo.Type;
			objectKeyAsString = handle.Substring(objectTypeFullName.Length + 1, handle.Length - objectTypeFullName.Length - 2);
			return true;
		}
	}
}
