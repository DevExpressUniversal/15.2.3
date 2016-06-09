﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Reflection;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Native {
	public class RuntimeSolutionTypesProvider : SolutionTypesProviderBase {		
		readonly IEnumerable<Type> typesEnumerable;
		List<Type> typeList;
		public RuntimeSolutionTypesProvider(Func<IEnumerable<Type>> typesSelector) {
			typesEnumerable = typesSelector();
		}
		protected override IEnumerable<Type> GetActiveProjectTypes() {
			if(typeList == null)
				CreateTypeList();
			return typeList;
		}
		protected override IDXAssemblyInfo GetAssemblyCore(string assemblyName) {
			Assembly asm = Assembly.Load(assemblyName);		 
			return new DXAssemblyInfo(asm, string.Equals(assemblyName, GetActiveProjectAssemblyFullName(), StringComparison.Ordinal), false, null, asm.GetTypes());
		}
		protected override string GetActiveProjectAssemblyFullName() {
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if(entryAssembly == null)
				return string.Empty;
			return entryAssembly.FullName;
		}
		protected override string[] GetProjectOutputs() {
			return null;
		}
		protected override string GetOutputDir() {
			return null;
		}
		protected override IEnumerable<string> GetSolutionAssemblyFullNames() {
			return Enumerable.Empty<string>();
		}
		void CreateTypeList() {
			typeList = new List<Type>();
			typeList.AddRange(typesEnumerable);
		}
	}
}
