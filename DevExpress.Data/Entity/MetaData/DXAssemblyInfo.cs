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
using System.Reflection;
using System.Collections.Generic;
namespace DevExpress.Entity.ProjectModel {
	public class DXAssemblyInfo : HasTypesCacheBase, IDXAssemblyInfo {
		string assemblyFullName;		
		EdmxResources edmxResources;
		ResourceOptions resourceOptions;
		string name;
		public DXAssemblyInfo(string assemblyFullName, bool isProjectAssembly, bool isSolutionAssembly, IResourceOptions resourceOptions)
		{
			ResourceOptions = resourceOptions;
			this.assemblyFullName = assemblyFullName;
			IsProjectAssembly = isProjectAssembly;
			IsSolutionAssembly = isSolutionAssembly || isProjectAssembly;
		}
		public DXAssemblyInfo(Assembly assembly, bool isProjectAssembly, bool isSolutionAssembly, IResourceOptions resourceOptions, params Type[] type) {
			ResourceOptions = resourceOptions;
			Assembly = assembly;
			IsProjectAssembly = isProjectAssembly;
			IsSolutionAssembly = isSolutionAssembly || isProjectAssembly;
			AddTypes(type);
		}
		public DXAssemblyInfo(IDXAssemblyInfo dxAssemblyInfo) {
			if(dxAssemblyInfo == null)
				return;
			this.resourceOptions = (dxAssemblyInfo as DXAssemblyInfo).resourceOptions;
			this.assemblyFullName = dxAssemblyInfo.AssemblyFullName;
			this.name = dxAssemblyInfo.Name;
			DXAssemblyInfo classAsmInfo = dxAssemblyInfo as DXAssemblyInfo;
			if(classAsmInfo != null)
				Assembly = classAsmInfo.Assembly;
			IsProjectAssembly = dxAssemblyInfo.IsProjectAssembly;
			IsSolutionAssembly = dxAssemblyInfo.IsProjectAssembly || dxAssemblyInfo.IsSolutionAssembly;
		}
		EdmxResources EdmxResources {
			get {
				if(edmxResources == null)
					edmxResources = new EdmxResources(Assembly, resourceOptions);
				return edmxResources;
			}
		}
		public EdmxResource GetEdmxResource(IDXTypeInfo typeInfo) {
			if(typeInfo == null)
				return null;
			return EdmxResources.GetEdmxResource(typeInfo.Name);
		}
		void AddTypes(IEnumerable<Type> types) {
			if(types == null)
				return;
			foreach(Type type in types)
				Add(new DXTypeInfo(type));
		}		
		public override void Add(IDXTypeInfo typeInfo) {
			if(typeInfo is DXTypeInfo)
				((DXTypeInfo)typeInfo).Assembly = this;
			base.Add(typeInfo);
		}	   
		public Assembly Assembly {
			get;
			private set;
		}
		public string AssemblyFullName {
			get {
				if(!String.IsNullOrEmpty(assemblyFullName))
					return assemblyFullName;
				if(Assembly != null)
					assemblyFullName = Assembly.FullName;
				return assemblyFullName;
			}
		}
		public bool IsProjectAssembly {
			get;
			private set;
		}
		public IResourceOptions ResourceOptions { get; private set; }
		public bool IsSolutionAssembly {
			get;
			private set;
		}
		public string Name {
			get {
				if(this.Assembly != null)
					return this.Assembly.GetName().Name;
				return name;
			}
		}
	}
}
