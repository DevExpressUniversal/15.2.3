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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace DevExpress.Utils.Design {
	public static class DXAssemblyHelper {
		public static Type GetTypeFromAssembly(string typeName, string nameSpace, string assemblyName) {
			string patchedAssemblyName = PatchAssemblyName(assemblyName);
			Type type = TryGetType(typeName, nameSpace, patchedAssemblyName);
			if(type == null)
				throw new ArgumentException(string.Format("Could not load type '{0}'", typeName));
			return type;
		}
		static string PatchAssemblyName(string assemblyName) {
			if(assemblyName.Contains(" "))
				return assemblyName;
			string publicKeyToken = GetPublicKeyToken();
			string fullAssemblyVersionExtension = ", Version=" + AssemblyInfo.Version + ", Culture=neutral, PublicKeyToken=" + publicKeyToken;
			return assemblyName + fullAssemblyVersionExtension;
		}
		static string GetPublicKeyToken() {
			return Assembly.GetExecutingAssembly().GetName().GetPublicKeyToken().Aggregate(string.Empty, (result, item) => (result += item.ToString("X2"))).ToLower();
		}
		static Type TryGetType(string typeName, string nameSpace, string assemblyName) {
			StringBuilder fullTypeName = new StringBuilder();
			fullTypeName.Append(nameSpace);
			fullTypeName.Append(".");
			fullTypeName.Append(typeName);
			fullTypeName.Append(", ");
			fullTypeName.Append(assemblyName);
			Type type = Type.GetType(fullTypeName.ToString());
			return type;
		}
	}
}
