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
using DevExpress.Design.Mvvm;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.MetaData;
using System.Reflection;
using DevExpress.Design.Mvvm.Wizards;
using DevExpress.Design.Mvvm.EntityFramework;
using System.ComponentModel.Design;
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData {
	public class ViewModelLayerService : IViewModelLayerService {
		readonly IServiceContainer serviceContainer;
		public ViewModelLayerService(IServiceContainer serviceContainer) {
			this.serviceContainer = serviceContainer;
		}
		ISolutionTypesProvider SolutionTypesProvider { get { return (ISolutionTypesProvider)this.serviceContainer.GetService(typeof(ISolutionTypesProvider)); } }
		IDataAccessLayerService DataAccesLayerService { get { return (IDataAccessLayerService)this.serviceContainer.GetService(typeof(IDataAccessLayerService)); } }
		IEntityFrameworkModel EntityFrameworkModel { get { return (IEntityFrameworkModel)serviceContainer.GetService(typeof(IEntityFrameworkModel)); } }
		IEnumerable<IViewModelInfo> GetViewModelTypesInternal() {
			IEnumerable<IDXTypeInfo> types = this.SolutionTypesProvider.GetTypes();
			foreach(IDXTypeInfo typeInfo in types) {
				bool isLocal = typeInfo.Assembly != null && typeInfo.Assembly.IsProjectAssembly;
				bool isSolution = isLocal || typeInfo.IsSolutionType;
				Type type = typeInfo.ResolveType();
				if(type.FullName == "DevExpress.Xpf.Core.Design.Wizards.Tests.Tests.SaveLayoutTests.TestCategoryViewModel")
					continue;
				IViewModelInfo info = ViewModelDataSource.GetEntityViewModelData(type, isSolution, isLocal);
				if(info == null)
					info = ViewModelDataSource.GetCollectionViewModelData(type, isSolution, isLocal);
				if(info != null)
					yield return info;
			}
		}
		public IEnumerable<IViewModelInfo> GetViewModelTypes() {
			return GetViewModelTypesInternal();
		}
		public string GetViewModelName(string typeName, ViewModelType modelType) {
			return ViewNameHelper.GetViewModelName(typeName, modelType);
		}
		public string GetViewName(string typeName, string viewModelName, ViewType viewType) {
			return ViewNameHelper.GetViewName(typeName, viewModelName, viewType);
		}
	}
	public static class ViewNameHelper {
		const string STR_Model = "Model";
		const string STR_View = "View";
		const string STR_ViewModel = STR_View + STR_Model;
		const string STR_RepositoryView = "CollectionView";
		public static string GetViewModelName(string typeName, ViewModelType modelType) {
			if(string.IsNullOrEmpty(typeName) || modelType == ViewModelType.Simple)
				return STR_ViewModel;
			if(modelType == ViewModelType.Entity)
				return typeName + STR_ViewModel;
			return typeName + "Collection" + STR_ViewModel;
		}
		public static string GetViewName(string typeName, string viewModelName, ViewType viewType) {
			if(string.IsNullOrEmpty(typeName) || viewType == ViewType.None) {
				if(string.IsNullOrEmpty(viewModelName)) return STR_View;
				if(viewModelName.EndsWith(STR_Model))
					viewModelName = viewModelName.Remove(viewModelName.Length - STR_Model.Length);
				if(!viewModelName.EndsWith(STR_View))
					viewModelName += STR_View;
				return viewModelName;
			}
			if(viewType == ViewType.Entity)
				return typeName + STR_View;
			else
				return typeName + STR_RepositoryView;
		}
	}
}
