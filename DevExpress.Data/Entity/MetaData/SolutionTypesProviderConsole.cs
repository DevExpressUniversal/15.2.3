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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
namespace DevExpress.Entity.ProjectModel {
	public class SolutionTypesProviderConsole : SolutionTypesProviderBase {
		List<Type> typeList = new List<Type>();
		string activeProjectAssemblyFullName;
		IEnumerable<string> solutionAssemblyFullNames;
		public SolutionTypesProviderConsole() {
		}
		public SolutionTypesProviderConsole(string activeProjectAssemblyFullName) {
			this.activeProjectAssemblyFullName = activeProjectAssemblyFullName;
		}
		public SolutionTypesProviderConsole(string activeProjectAssemblyFullName, IEnumerable<string> solutionAssemblyFullNames)
			: this(activeProjectAssemblyFullName) {
			this.solutionAssemblyFullNames = solutionAssemblyFullNames;
		}
		protected override IEnumerable<Type> GetActiveProjectTypes() {
			return typeList;
		}
		protected override IDXAssemblyInfo GetAssemblyCore(string assemblyName) {
			Assembly asm = Assembly.Load(assemblyName);
			return new DXAssemblyInfo(asm, false, false, null, asm.GetTypes());
		}
		public void Add(Type type) {
			if (!typeList.Contains(type))
				typeList.Add(type);
		}
		public void AddRange(IEnumerable<Type> types) {
			foreach (Type item in types)
				Add(item);
		}
		protected override string GetActiveProjectAssemblyFullName() {
			return activeProjectAssemblyFullName;
		}
		protected override string[] GetProjectOutputs() {
			return null;
		}
		protected override string GetOutputDir() {
			return null;
		}
		protected override IEnumerable<string> GetSolutionAssemblyFullNames() {
			return solutionAssemblyFullNames;
		}
	}
}
