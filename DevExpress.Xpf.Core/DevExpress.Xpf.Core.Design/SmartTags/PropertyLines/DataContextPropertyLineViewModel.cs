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

extern alias Platform;
using DevExpress.Design;
using DevExpress.Design.SmartTags;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Guard = Platform::DevExpress.Utils.Guard;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public class DataContextPropertyLineViewModel : TypeSelectorPropertyLineViewModelBase {
		public DataContextPropertyLineViewModel(IPropertyLineContext context, string propertyName)
			: base(context, propertyName, context.PlatformInfoFactory.ForStandardProperty(FrameworkElement.DataContextProperty.Name)) {
		}
		protected override PropertyLineWithPopupEditorPopupViewModel CreatePopup() {
			return new DataContextPropertyLinePopupViewModel(this);
		}
	}
	public class DataContextPropertyLinePopupViewModel : TypeSelectorPropertyLinePopupViewModelBase {
		public DataContextPropertyLinePopupViewModel(TypeSelectorPropertyLineViewModelBase propertyLine)
			: base(propertyLine) {
		}
		protected override void OnSelectedNodeChanged() {
			base.OnSelectedNodeChanged();
			ViewModelEditorTypeInfo typeInfo = SelectedNode as ViewModelEditorTypeInfo;
			if(typeInfo == null) return;
			IDXTypeMetadata type = typeInfo.Type;
			Type runtimeType = type.GetRuntimeType();
			if(runtimeType == null) return;
			Type currentDataContextType = DataContextInfoHelper.GetDataContextType(PropertyLine.SelectedItem);
			if(currentDataContextType != runtimeType)
				DataContextInfoHelper.SetDataContext(PropertyLine.SelectedItem, type);
		}
		protected override IEnumerable<IDXTypeMetadata> GetProjectViewModels() {
			return ViewModelHelper.GetViewModels(true, PropertyLine.SelectedItem);
		}
		protected override IEnumerable<IDXTypeMetadata> GetSolutionViewModels() {
			return ViewModelHelper.GetViewModels(false, PropertyLine.SelectedItem);
		}
	}
	public class DataContextPropertyLinePopupViewModelDataProvider : TreeViewPropertyLinePopupViewModelDataProvider {
		readonly Func<IEnumerable<IDXTypeMetadata>> loadDataCallback;
		public DataContextPropertyLinePopupViewModelDataProvider(Func<IEnumerable<IDXTypeMetadata>> loadDataCallback, Func<Type> getDefaultSelectedTypeCallback) {
			this.loadDataCallback = loadDataCallback;
			UpdateAsyncFunc = AsyncHelper.Create<bool, Type>(reset => getDefaultSelectedTypeCallback(), UpdateAsync);
		}
		IEnumerable<ManualResetEvent> UpdateAsync(bool reset, Type defaultSelectedType, CancellationToken cancellationToken) {
			if(reset) {
				Root = null;
				return null;
			}
			if(cancellationToken.IsCancellationRequested) return null;
			ViewModelEditorRootInfo root = new ViewModelEditorRootInfo();
			ViewModelEditorTypeInfo defaultSelectedItem = null;
			foreach(IDXTypeMetadata viewModel in loadDataCallback == null ? new IDXTypeMetadata[] { } : loadDataCallback()) {
				ViewModelEditorTypeInfo addedItem = root.AddType(viewModel);
				if(addedItem == null) continue;
				if(defaultSelectedItem == null && defaultSelectedType != null && defaultSelectedType.FullName == addedItem.Type.FullName && defaultSelectedType == addedItem.Type.GetRuntimeType())
					defaultSelectedItem = addedItem;
			}
			Root = root;
			DefaultSelectedItem = defaultSelectedItem;
			return null;
		}
	}
	public class ViewModelEditorRootInfo : INodeSelectorItem {
		readonly SortedDictionary<string, ViewModelEditorAssemblyInfo> assemblies = new SortedDictionary<string, ViewModelEditorAssemblyInfo>();
		public INodeSelectorItem Parent { get { return null; } }
		public string Name { get { return string.Empty; } }
		public ImageSource Icon { get { return null; } }
		bool INodeSelectorItem.CanBeSelected { get { return false; } }
		public IEnumerable<INodeSelectorItem> GetChildren() { return assemblies.Values; }
		public ViewModelEditorTypeInfo AddType(IDXTypeMetadata type) {
			IDXAssemblyMetadata typeAssembly = type.Assembly;
			if(typeAssembly == null) return null;
			string assemblyName = typeAssembly.Name;
			if(string.IsNullOrEmpty(assemblyName)) return null;
			ViewModelEditorAssemblyInfo assemblyInfo;
			if(!assemblies.TryGetValue(assemblyName, out assemblyInfo)) {
				assemblyInfo = new ViewModelEditorAssemblyInfo(assemblyName, this);
				assemblies.Add(assemblyName, assemblyInfo);
			}
			return assemblyInfo.AddType(type);
		}
	}
	public class ViewModelEditorAssemblyInfo : INodeSelectorItem {
		readonly SortedDictionary<string, ViewModelEditorNamespaceInfo> namespaces = new SortedDictionary<string, ViewModelEditorNamespaceInfo>();
		public ViewModelEditorAssemblyInfo(string assemblyName, ViewModelEditorRootInfo parent) {
			Parent = parent;
			Name = assemblyName;
		}
		public INodeSelectorItem Parent { get; private set; }
		public ImageSource Icon { get { return ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(ViewModelEditorAssemblyInfo).Assembly, "Images/MVVMAssistant/assembly.png")); } }
		public string Name { get; private set; }
		bool INodeSelectorItem.CanBeSelected { get { return false; } }
		public IEnumerable<INodeSelectorItem> GetChildren() { return namespaces.Values; }
		public ViewModelEditorTypeInfo AddType(IDXTypeMetadata type) {
			string typeNamespace = type.Namespace;
			if(string.IsNullOrEmpty(typeNamespace)) return null;
			ViewModelEditorNamespaceInfo namespaceInfo;
			if(!namespaces.TryGetValue(typeNamespace, out namespaceInfo)) {
				namespaceInfo = new ViewModelEditorNamespaceInfo(typeNamespace, this);
				namespaces.Add(typeNamespace, namespaceInfo);
			}
			return namespaceInfo.AddType(type);
		}
	}
	public class ViewModelEditorNamespaceInfo : INodeSelectorItem {
		readonly SortedDictionary<string, ViewModelEditorTypeInfo> types = new SortedDictionary<string, ViewModelEditorTypeInfo>();
		public ViewModelEditorNamespaceInfo(string name, ViewModelEditorAssemblyInfo parent) {
			Parent = parent;
			Name = name;
		}
		public INodeSelectorItem Parent { get; private set; }
		public ImageSource Icon { get { return ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(ViewModelEditorAssemblyInfo).Assembly, "Images/MVVMAssistant/namespace.png")); } }
		public string Name { get; private set; }
		bool INodeSelectorItem.CanBeSelected { get { return false; } }
		public IEnumerable<INodeSelectorItem> GetChildren() { return types.Values; }
		public ViewModelEditorTypeInfo AddType(IDXTypeMetadata type) {
			string typeName = type.Name;
			if(string.IsNullOrEmpty(typeName)) return null;
			ViewModelEditorTypeInfo typeInfo = new ViewModelEditorTypeInfo(typeName, type, this);
			types[typeName] = typeInfo;
			return typeInfo;
		}
	}
	public class ViewModelEditorTypeInfo : INodeSelectorItem {
		public ViewModelEditorTypeInfo(string name, IDXTypeMetadata type, ViewModelEditorNamespaceInfo parent) {
			Guard.ArgumentNotNull(type, "type");
			Parent = parent;
			Name = name;
			Type = type;
		}
		public INodeSelectorItem Parent { get; private set; }
		public IDXTypeMetadata Type { get; private set; }
		public ImageSource Icon { get { return ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(ViewModelEditorAssemblyInfo).Assembly, "Images/MVVMAssistant/class.png")); } }
		public string Name { get; private set; }
		bool INodeSelectorItem.CanBeSelected { get { return true; } }
		public IEnumerable<INodeSelectorItem> GetChildren() { return new INodeSelectorItem[] { }; }
		#region Equality
		public override int GetHashCode() {
			return Type.GetHashCode();
		}
		public static bool operator ==(ViewModelEditorTypeInfo i1, ViewModelEditorTypeInfo i2) {
			bool i1IsNull = (object)i1 == null;
			bool i2IsNull = (object)i2 == null;
			if(i1IsNull && i2IsNull) return true;
			if(i1IsNull || i2IsNull) return false;
			return i1.Type == i2.Type;
		}
		public static bool operator !=(ViewModelEditorTypeInfo i1, ViewModelEditorTypeInfo i2) {
			return !(i1 == i2);
		}
		public override bool Equals(object obj) {
			return this == obj as ViewModelEditorTypeInfo;
		}
		#endregion
	}
}
