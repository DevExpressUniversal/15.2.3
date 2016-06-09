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

using System.Collections.Generic;
using System.Reflection;
namespace DevExpress.ExpressApp.DC {
	public class DcAssemblyInfo : BaseInfo, IAssemblyInfo {
		private readonly Assembly assembly;
		private readonly Dictionary<TypeInfo, ITypeInfo> types;
		private bool allTypesLoaded;
		public DcAssemblyInfo(Assembly assembly, TypesInfo store)
			: base(store) {
			types = new Dictionary<TypeInfo, ITypeInfo>();
			this.assembly = assembly;
		}
		public void LoadTypes() {
			Store.LoadTypes(this);
		}
		public void AddTypeInfo(TypeInfo info) {
			if(!types.ContainsKey(info)) {
				types.Add(info, info);
			}
		}
		public string FullName {
			get { return assembly.FullName; }
		}
		public IEnumerable<TypeInfo> Types {
			get { return types.Keys; }
		}
		public Assembly Assembly {
			get { return assembly; }
		}
		public bool AllTypesLoaded {
			get { return allTypesLoaded; }
			set { allTypesLoaded = value; }
		}
		#region IAssemblyInfo Members
		void IAssemblyInfo.LoadTypes() {
			LoadTypes();
		}
		Assembly IAssemblyInfo.Assembly {
			get { return Assembly; }
		}
		string IAssemblyInfo.FullName {
			get { return FullName; }
		}
		IEnumerable<ITypeInfo> IAssemblyInfo.Types {
			get { return types.Values; }
		}
		bool IAssemblyInfo.AllTypesLoaded {
			get { return AllTypesLoaded; }
		}
		#endregion
	}
}
