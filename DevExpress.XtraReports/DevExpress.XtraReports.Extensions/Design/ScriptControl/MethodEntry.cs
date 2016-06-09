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

using System.Collections.Generic;
using DevExpress.CodeParser;
namespace DevExpress.XtraReports.Design {
	class MethodEntry {
		static List<string> GetParamTypes(Method method) {
			if(method == null)
				return null;
			LanguageElementCollection ps = method.Parameters;
			List<string> paramTypes = new List<string>();
			int count = ps.Count;
			for(int i = 0; i < count; i++) {
				Param par = ps[i] as Param;
				if(par != null)
					paramTypes.Add(par.ParamType);
			}
			return paramTypes;
		}
		static bool EqualTypes(string type1, string type2) {
			if(type1 != type2)
				return type1.IndexOf(type2) != -1 || type2.IndexOf(type1) != -1;
			return true;
		}
		Method method;
		List<string> parameterTypes;
		public MethodEntry(Method method) {
			this.method = method;
			this.parameterTypes = GetParamTypes(method);
		}
		public string Name {
			get { return method.Name; }
		}
		public Method Method {
			get { return method; }
		}
		public bool EqualSignature(MethodEntry method) {
			if(method.parameterTypes.Count != parameterTypes.Count)
				return false;
			for(int i = 0; i < parameterTypes.Count; i++)
				if(!EqualTypes(method.parameterTypes[i], parameterTypes[i]))
					return false;
			return true;
		}
	}
}
