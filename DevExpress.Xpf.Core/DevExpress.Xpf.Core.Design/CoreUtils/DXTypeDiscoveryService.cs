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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Design;
using DevExpress.Design.SmartTags;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design.Model;
using System.Reflection;
using Microsoft.Windows.Design.Metadata;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Data.Utils;
using Guard = Platform::DevExpress.Utils.Guard;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Design {
	public static class DXTypeDiscoveryServiceFactory {
		public static IDXTypeDiscoveryService Create(IEditingContext editingContext) {
			IMarkupAccessService markupAccessService = editingContext.Services.GetService<IMarkupAccessService>();
			IMarkupAccessService2012 markupAccessService2012 = markupAccessService as IMarkupAccessService2012;
			if(markupAccessService2012 != null)
				return new DXVS2012TypeDiscoveryService(markupAccessService2012, editingContext);
			IMarkupAccessService2010 markupAccessService2010 = markupAccessService as IMarkupAccessService2010;
			if(markupAccessService2010 != null)
				return new DXVS2010TypeDiscoveryService(markupAccessService2010, editingContext);
			throw new InvalidOperationException();
		}
	}
	public class DXVS2010TypeDiscoveryService : IDXTypeDiscoveryService {
		IVS2010MetadataContext metadataContext;
		public DXVS2010TypeDiscoveryService(IMarkupAccessService2010 markupAccessService, IEditingContext editingContext) {
			IModelItem modelItem = editingContext.Services.GetService<IModelService>().Root;
			metadataContext = markupAccessService.GetModelItem(modelItem).Host.Metadata;
		}
		public IEnumerable<IDXTypeMetadata> GetTypes(Func<IDXAssemblyMetadata, bool> assemblyPredicate, Func<IDXTypeMetadata, bool> typePredicate, bool fromActiveProjectOnly) {
			IEnumerable<IVS2010AssemblyMetadata> assemblies = fromActiveProjectOnly ? new IVS2010AssemblyMetadata[] { metadataContext.LocalAssembly } : metadataContext.Assemblies;
			return assemblies
				.Select(a => DXVS2010AssemblyMetadata.Get(a))
				.Where(assemblyPredicate)
				.Cast<DXVS2010AssemblyMetadata>().SelectMany(a => a.Value.Types)
				.Select(t => DXVS2010TypeMetadata.Get(t))
				.Where(typePredicate);
		}
	}
	public class DXVS2012TypeDiscoveryService : IDXTypeDiscoveryService {
		IVS2012TypeResolver typeResolver;
		public DXVS2012TypeDiscoveryService(IMarkupAccessService2012 markupAccessService, IEditingContext editingContext) {
			IModelItem modelItem = editingContext.Services.GetService<IModelService>().Root;
			typeResolver = markupAccessService.GetModelItem(modelItem).SceneNode.ProjectContext.Metadata;
		}
		public IEnumerable<IDXTypeMetadata> GetTypes(Func<IDXAssemblyMetadata, bool> assemblyPredicate, Func<IDXTypeMetadata, bool> typePredicate, bool fromActiveProjectOnly) {
			IEnumerable<IVS2012AssemblyMetadata> assemblies = fromActiveProjectOnly ? new IVS2012AssemblyMetadata[] { typeResolver.ProjectAssembly } : typeResolver.AssemblyReferences;
			return assemblies
				.Select(a => DXVS2012AssemblyMetadata.Get(a))
				.Where(assemblyPredicate)
				.Cast<DXVS2012AssemblyMetadata>().SelectMany(a => typeResolver.GetTypes(a.Value))
				.Select(t => DXVS2012TypeMetadata.Get(t, typeResolver))
				.Where(typePredicate);
		}
	}
	public class DXVS2010AssemblyMetadata : RuntimeBase<IDXAssemblyMetadata, IVS2010AssemblyMetadata>, IDXAssemblyMetadata {
		public static IDXAssemblyMetadata Get(IVS2010AssemblyMetadata assemblyMetadata) {
			return assemblyMetadata == null ? null : new DXVS2010AssemblyMetadata(assemblyMetadata);
		}
		protected DXVS2010AssemblyMetadata(IVS2010AssemblyMetadata assemblyMetadata) : base(assemblyMetadata) { }
		public string Name { get { return Value.Name; } }
		public string FullName { get { return Value.FullName; } }
	}
	public class DXVS2012AssemblyMetadata : RuntimeBase<IDXAssemblyMetadata, IVS2012AssemblyMetadata>, IDXAssemblyMetadata {
		public static IDXAssemblyMetadata Get(IVS2012AssemblyMetadata assemblyMetadata) {
			return assemblyMetadata == null ? null : new DXVS2012AssemblyMetadata(assemblyMetadata);
		}
		protected DXVS2012AssemblyMetadata(IVS2012AssemblyMetadata assemblyMetadata) : base(assemblyMetadata) { }
		public string Name { get { return Value.Name; } }
		public string FullName { get { return Value.FullName; } }
	}
	public class DXVS2010TypeMetadata : RuntimeBase<IDXTypeMetadata, IVS2010TypeMetadata>, IDXTypeMetadata {
		public static IDXTypeMetadata Get(IVS2010TypeMetadata typeMetadata) {
			return typeMetadata == null ? null : new DXVS2010TypeMetadata(typeMetadata);
		}
		protected DXVS2010TypeMetadata(IVS2010TypeMetadata typeMetadata) : base(typeMetadata) { }
		public string Name { get { return Value.Name; } }
		public string FullName { get { return Value.FullName; } }
		public string Namespace { get { return Value.NamespaceName; } }
		public bool IsAbstract { get { return Value.IsAbstract; } }
		public bool IsArray { get { return Value.IsArray; } }
		public bool IsEnum { get { return Value.IsEnum; } }
		public bool IsInterface { get { return Value.IsInterface; } }
		public bool IsValueType { get { return Value.IsValueType; } }
		public bool IsGenericType { get { return Value.IsGenericType; } }
		public bool IsPocoViewModel { get { return ViewModelSourceHelper.IsPOCOViewModelType(Value.GetRuntimeType()); } }
		public IDXAssemblyMetadata Assembly { get { return DXVS2010AssemblyMetadata.Get(Value.Assembly); } }
		public IDXTypeMetadata BaseType { get { return DXVS2010TypeMetadata.Get(Value.BaseType); } }
		public bool HasDefaultConstructor {
			get {
				IVS2010ConstructorMetadata constructor = Value.GetConstructor();
				return constructor != null && constructor.IsVisible;
			}
		}
		public Type GetRuntimeType() { return Value.GetRuntimeType(); }
		public bool IsVisible { get { return Value.IsVisible; } }
		public bool ImplementsInterface(Type interfaceType) {
			string interfaceTypeFullName = interfaceType.FullName;
			return Value.Interfaces.Where(i => string.Equals(i.FullName, interfaceTypeFullName, StringComparison.Ordinal)).Any();
		}
	}
	public class DXVS2012TypeMetadata : RuntimeBase<IDXTypeMetadata, IVS2012TypeMetadata>, IDXTypeMetadata {
		public static IDXTypeMetadata Get(IVS2012TypeMetadata typeMetadata, IVS2012TypeResolver typeResolver) {
			return typeMetadata == null ? null : new DXVS2012TypeMetadata(typeMetadata, typeResolver);
		}
		IVS2012TypeResolver typeResolver;
		protected DXVS2012TypeMetadata(IVS2012TypeMetadata typeMetadata, IVS2012TypeResolver typeResolver)
			: base(typeMetadata) {
			this.typeResolver = typeResolver;
		}
		public string Name { get { return Value.Name; } }
		public string FullName { get { return Value.FullName; } }
		public string Namespace { get { return Value.Namespace; } }
		public bool IsAbstract { get { return Value.IsAbstract; } }
		public bool IsArray { get { return Value.IsArray; } }
		public bool IsEnum { get { return Value.RuntimeType.IsEnum; } }
		public bool IsInterface { get { return Value.IsInterface; } }
		public bool IsValueType { get { return Value.RuntimeType.IsValueType; } }
		public bool IsGenericType { get { return Value.IsGenericType; } }
		public bool IsPocoViewModel { get { return ViewModelSourceHelper.IsPOCOViewModelType(Value.RuntimeType); } }
		public IDXAssemblyMetadata Assembly { get { return DXVS2012AssemblyMetadata.Get(Value.RuntimeAssembly); } }
		public IDXTypeMetadata BaseType { get { return DXVS2012TypeMetadata.Get(Value.BaseType, this.typeResolver); } }
		public bool HasDefaultConstructor { get { return Value.HasDefaultConstructor; } }
		public Type GetRuntimeType() { return Value.RuntimeType; }
		public bool IsVisible {
			get {
				if(Value.IsPublic) return true;
				IVS2012AssemblyMetadata typeAssembly = Value.RuntimeAssembly;
				IVS2012AssemblyMetadata projectAssembly = this.typeResolver.ProjectAssembly;
				return typeAssembly != null && projectAssembly != null && typeAssembly.FullName == projectAssembly.FullName;
			}
		}
		public bool ImplementsInterface(Type interfaceType) {
			IVS2012TypeMetadata interfaceTypeMetadata = this.typeResolver.GetType(interfaceType);
			return interfaceTypeMetadata != null && interfaceTypeMetadata.IsAssignableFrom(Value);
		}
	}
}
