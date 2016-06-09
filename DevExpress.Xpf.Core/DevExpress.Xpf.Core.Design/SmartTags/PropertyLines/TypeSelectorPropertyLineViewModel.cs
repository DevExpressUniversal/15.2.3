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

using DevExpress.Design;
using DevExpress.Design.SmartTags;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public abstract class TypeSelectorPropertyLineViewModelBase : PropertyLineWithPopupEditorViewModel {
		public TypeSelectorPropertyLineViewModelBase(IPropertyLineContext context, string propertyName, IPropertyLinePlatformInfo platformInfo)
			: base(context, propertyName, null, platformInfo) {
			IsReadOnly = true;
		}
	}
	public class TypeSelectorPropertyLineViewModel : TypeSelectorPropertyLineViewModelBase {
		public Func<IDXTypeMetadata, bool> TypePredicate { get; set; }
		public TypeSelectorPropertyLineViewModel(IPropertyLineContext context, string propertyName, IPropertyLinePlatformInfo platformInfo, Func<IDXTypeMetadata, bool> typePredicate)
			: base(context, propertyName, platformInfo) {
			TypePredicate = typePredicate;
		}
		protected override PropertyLineWithPopupEditorPopupViewModel CreatePopup() {
			return new TypeSelectorPropertyLinePopupViewModel(this);
		}
	}
	public abstract class TypeSelectorPropertyLinePopupViewModelBase : TreeViewPropertyLinePopupViewModelBase {
		readonly DataContextPropertyLinePopupViewModelDataProvider solutionDataProviderCore;
		readonly DataContextPropertyLinePopupViewModelDataProvider projectDataProviderCore;
		INodeSelectorDataProvider solutionDataProvider;
		INodeSelectorDataProvider projectDataProvider;
		public TypeSelectorPropertyLinePopupViewModelBase(TypeSelectorPropertyLineViewModelBase propertyLine)
			: base(propertyLine) {
			solutionDataProviderCore = new DataContextPropertyLinePopupViewModelDataProvider(GetSolutionViewModels, GetDefaultSelectedType);
			projectDataProviderCore = new DataContextPropertyLinePopupViewModelDataProvider(GetProjectViewModels, GetDefaultSelectedType);
		}
		public INodeSelectorDataProvider SolutionDataProvider {
			get { return solutionDataProvider; }
			set { SetProperty(ref solutionDataProvider, value, () => SolutionDataProvider); }
		}
		public INodeSelectorDataProvider ProjectDataProvider {
			get { return projectDataProvider; }
			set { SetProperty(ref projectDataProvider, value, () => ProjectDataProvider); }
		}
		protected override void OnPropertyLineIsPopupOpenChanged(object sender, EventArgs e) {
			base.OnPropertyLineIsPopupOpenChanged(sender, e);
			if(PropertyLine.IsPopupOpen) {
				SolutionDataProvider = solutionDataProviderCore;
				ProjectDataProvider = projectDataProviderCore;
			} else {
				SolutionDataProvider = null;
				ProjectDataProvider = null;
				solutionDataProviderCore.Reset();
				projectDataProviderCore.Reset();
			}
		}
		protected override void OnSelectedNodeChanged() {
			base.OnSelectedNodeChanged();
			ViewModelEditorTypeInfo typeInfo = SelectedNode as ViewModelEditorTypeInfo;
			if(typeInfo == null) return;
			IDXTypeMetadata type = typeInfo.Type;
			Type runtimeType = type.GetRuntimeType();
			PropertyLine.PropertyValue = runtimeType;
		}
		protected virtual Type GetDefaultSelectedType() {
			return PropertyLine.PropertyValueType;
		}
		protected abstract IEnumerable<IDXTypeMetadata> GetProjectViewModels();
		protected abstract IEnumerable<IDXTypeMetadata> GetSolutionViewModels();
	}
	public class TypeSelectorPropertyLinePopupViewModel: TypeSelectorPropertyLinePopupViewModelBase {
		public TypeSelectorPropertyLinePopupViewModel(TypeSelectorPropertyLineViewModel propertyLine)
			: base(propertyLine) {
		}
		protected override IEnumerable<IDXTypeMetadata> GetProjectViewModels() {
			return GetAvailableTypes(true, PropertyLine.SelectedItem);
		}
		protected override IEnumerable<IDXTypeMetadata> GetSolutionViewModels() {
			return GetAvailableTypes(false, PropertyLine.SelectedItem);
		}
		protected virtual Func<IDXTypeMetadata, bool> GetTypePredicate() {
			return ((TypeSelectorPropertyLineViewModel)PropertyLine).TypePredicate;
		}
		IEnumerable<IDXTypeMetadata> GetAvailableTypes(bool fromActiveProjectsOnly, IModelItem modelItem) {
			IDXTypeDiscoveryService typeDiscoveryService = PropertyLine.SelectedItem.Context.Services.GetService(typeof(IDXTypeDiscoveryService)) as IDXTypeDiscoveryService;
			if(typeDiscoveryService == null)
				return Enumerable.Empty<IDXTypeMetadata>();
			var typePredicate = GetTypePredicate();
			var availableTypes = typeDiscoveryService.GetTypes(
				a => ViewModelHelper.ViewModelAssemblyPredicate(a, null),
				t => typePredicate(t), fromActiveProjectsOnly);
			return availableTypes;
		}
	}
}
