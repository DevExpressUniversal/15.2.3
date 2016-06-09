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

namespace DevExpress.Design.Filtering {
	using System;
	using System.Collections.Generic;
	public interface ILocalizableEUFObject<TCodeName>
		where TCodeName : struct {
		string Name { get; }
		string Description { get; }
		TCodeName GetCodeName();
	}
	abstract class LocalizableEndUserFilteringObject<TCodeName> : ILocalizableEUFObject<TCodeName>
		where TCodeName : struct {
		readonly TCodeName codeName;
		protected LocalizableEndUserFilteringObject(TCodeName codeName) {
			this.codeName = codeName;
		}
		public string Name {
			get { return CodeNamesResolver<TCodeName>.GetName(codeName); }
		}
		public string Description {
			get { return CodeNamesResolver<TCodeName>.GetDescription(codeName); }
		}
		public sealed override string ToString() {
			return Name;
		}
		TCodeName ILocalizableEUFObject<TCodeName>.GetCodeName() {
			return codeName;
		}
		public sealed override int GetHashCode() {
			return GetHash(codeName);
		}
		public sealed override bool Equals(object obj) {
			ILocalizableEUFObject<TCodeName> source = obj as ILocalizableEUFObject<TCodeName>;
			if(object.ReferenceEquals(source, null)) return false;
			return object.Equals(codeName, source.GetCodeName());
		}
		protected abstract int GetHash(TCodeName codeName);
		#region CodeNamesResolver
		const string CodeNameSuffix = "CodeName";
		internal readonly static string StringIdNameSuffix;
		internal readonly static string StringIdDescriptionSuffix;
		static LocalizableEndUserFilteringObject() {
			string codeNameTypeName = typeof(TCodeName).Name;
			string name = codeNameTypeName.Remove(codeNameTypeName.Length - CodeNameSuffix.Length);
			StringIdNameSuffix = name + "Name";
			StringIdDescriptionSuffix = name + "Description";
		}
		static class CodeNamesResolver<CodeName>
			where CodeName : struct {
			static IDictionary<CodeName, string> names = new Dictionary<CodeName, string>();
			internal static string GetName(CodeName name) {
				string result;
				if(!names.TryGetValue(name, out result))
					result = LoadName(name);
				return result;
			}
			static string LoadName(CodeName name) {
				string strId = name.ToString() + StringIdNameSuffix;
				string result = FilteringModelConfiguratorLocalizer.GetString((FilteringModelConfiguratorLocalizerStringId)Enum.Parse(typeof(FilteringModelConfiguratorLocalizerStringId), strId));
				names.Add(name, result);
				return result;
			}
			static IDictionary<CodeName, string> descriptions = new Dictionary<CodeName, string>();
			internal static string GetDescription(CodeName name) {
				string result;
				if(!descriptions.TryGetValue(name, out result))
					result = LoadDescription(name);
				return result;
			}
			static string LoadDescription(CodeName name) {
				string strId = name.ToString() + StringIdDescriptionSuffix;
				string result = FilteringModelConfiguratorLocalizer.GetString((FilteringModelConfiguratorLocalizerStringId)Enum.Parse(typeof(FilteringModelConfiguratorLocalizerStringId), strId));
				descriptions.Add(name, result);
				return result;
			}
		}
		#endregion CodeNamesResolver
	}
}
