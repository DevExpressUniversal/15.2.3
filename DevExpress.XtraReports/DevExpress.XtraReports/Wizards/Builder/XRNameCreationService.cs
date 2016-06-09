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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
namespace DevExpress.XtraReports.Serialization {
	public class XRNameCreationService : INameCreationService {
		#region static
		public static bool HasWrongCharacters(string name) {
			if(name == null || name.Length == 0 || !(Char.IsLetter(name[0]) || name[0] == '_'))
				return true;
			foreach(char ch in name) {
				if(!Char.IsLetterOrDigit(ch) && ch != '_')
					return true;
			}
			return false;
		}
		public static bool RootComponentHasMember(object rootComponent, string name) {
			if(rootComponent == null)
				return false;
			Type rootComponentType = rootComponent.GetType();
			System.Reflection.MemberInfo[] members = rootComponentType.GetMember(name);
			return members.Length != 0 && Array.Exists(members, m => !(m is System.Reflection.FieldInfo));
		}
		public static string GetDefaultBaseName(string typeName) {
			string baseResult = typeName.Length < 3 ? typeName : Char.ToLower(typeName[0]) + typeName.Substring(1);
			baseResult = baseResult.Replace("`", "_");
			if(baseResult.Length >= 3) {
				string prefix = typeName.Substring(0, 2);
				if(String.Compare(prefix, "xr", true) == 0) {
					string result = baseResult;
					result = result.Substring(2);
					result = result.Substring(0, 1).ToLower() + result.Substring(1);
					if(!HasWrongCharacters(result))
						return result;
				}
			}
			return baseResult;
		}
		#endregion
		IDesignerHost fHost;
		public XRNameCreationService(IDesignerHost host) {
			this.fHost = host;
		}
		protected virtual string GetValidName(string typeName) {
			return GetDefaultBaseName(typeName);
		}
		public string CreateNameByType(string[] names, Type dataType) {
			string name = GetValidName(dataType.Name);
			int number = 1;
			while(Array.IndexOf<string>(names,  name + number) >= 0)
				number++;
			return name + number;
		}
		public string CreateNameByType(ComponentCollection components, Type dataType) {
			string name = GetValidName(dataType.Name);
			int number = 1;
			while(components[name + number] != null)
				number++;
			return name + number;
		}
		public string CreateName(IContainer container, Type dataType) {
			return container == null ? GetValidName(dataType.Name) :
				CreateNameByType(container.Components, dataType);
		}
		public bool IsValidName(string name) {
			return !HasWrongCharacters(name) && !ContainsName(fHost.Container, name) && !RootComponentHasMember(fHost.Loading ? null : fHost.RootComponent, name);
		}
		bool ContainsName(IContainer container, string name) {
			foreach(IComponent comp in container.Components)
				if(comp.Site.Name.ToUpper() == name.ToUpper())
					return true;
			return false;
		}
		public void ValidateName(string name) {
			if(!IsValidName(name))
				throw new System.Exception("Invalid name " + name);
		}
	}
}
