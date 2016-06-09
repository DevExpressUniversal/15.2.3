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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Model.Core {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] 
	public class ModelInterfacesCollector {
		static readonly Type RuntimeType = typeof(object).GetType();
		private readonly List<Type> interfacesToImplement;
		private readonly List<Type> interfacesToGenerate;
		private readonly HashSet<Type> proccessedTypes;
		private ModelInterfaceExtenders interfaceExtenders;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelInterfacesCollector() {
			interfacesToImplement = new List<Type>();
			interfacesToGenerate = new List<Type>();
			proccessedTypes = new HashSet<Type>();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CollectedInterfaces Collect(Type rootModelInterface, ModelInterfaceExtenders interfaceExtenders) {
			Guard.ArgumentNotNull(rootModelInterface, "rootModelInterface");
			Guard.ArgumentNotNull(interfaceExtenders, "interfaceExtenders");
			Type[] toImplement;
			Type[] toGenerate;
			this.interfaceExtenders = interfaceExtenders;
			try {
				LoadTypesInfo(XafTypesInfo.Instance, interfaceExtenders, rootModelInterface);
				ProcessType(rootModelInterface);
				toImplement = interfacesToImplement.ToArray();
				toGenerate = interfacesToGenerate.ToArray();
			}
			finally {
				this.interfaceExtenders = null;
				proccessedTypes.Clear();
				interfacesToImplement.Clear();
				interfacesToGenerate.Clear();
			}
			return new CollectedInterfaces(toImplement, toGenerate);
		}
		private static void LoadTypesInfo(ITypesInfo typesInfo, ModelInterfaceExtenders interfaceExtenders, Type type) {
			HashSet<Type> proccessedTypes = new HashSet<Type>();
			List<Type> modelInterfaces = new List<Type>();
			LoadTypesInfo(typesInfo, interfaceExtenders, proccessedTypes, modelInterfaces, type);
			List<Type> actualModelInterfaces = new List<Type>(modelInterfaces);
			foreach(Type modelInterface in actualModelInterfaces) {
				List<ITypeInfo> descendants = new List<ITypeInfo>(typesInfo.FindTypeInfo(modelInterface).Descendants);
				foreach(ITypeInfo descendantInfo in descendants) {
					LoadTypesInfo(typesInfo, interfaceExtenders, proccessedTypes, modelInterfaces, descendantInfo.Type);
				}
			}
		}
		private static void LoadTypesInfo(ITypesInfo typesInfo, ModelInterfaceExtenders interfaceExtenders, ICollection<Type> proccessedTypes, ICollection<Type> modelInterfaces, Type type) {
			if(!proccessedTypes.Contains(type)) {
				proccessedTypes.Add(type);
				if(ModelInterfacesHelper.IsTypeSuitableToGenerateClass(type)) {
					modelInterfaces.Add(type);
					typesInfo.LoadTypes(type.Assembly);
					foreach(Type implementedInterface in TypeHelper.GetInterfaces(type)) {
						LoadTypesInfo(typesInfo, interfaceExtenders, proccessedTypes, modelInterfaces, implementedInterface);
					}
					foreach(PropertyInfo propertyInfo in TypeHelper.GetProperties(type)) {
						LoadTypesInfo(typesInfo, interfaceExtenders, proccessedTypes, modelInterfaces, propertyInfo.PropertyType);
					}
					foreach(Type extenderType in interfaceExtenders.GetInterfaceExtenders(type)) {
						foreach(PropertyInfo propertyInfo in TypeHelper.GetProperties(extenderType)) {
							LoadTypesInfo(typesInfo, interfaceExtenders, proccessedTypes, modelInterfaces, propertyInfo.PropertyType);
						}
					}
				}
				else if(ModelInterfacesHelper.IsGenericListType(type)) {
					Type parameter = type.GetGenericArguments()[0];
					LoadTypesInfo(typesInfo, interfaceExtenders, proccessedTypes, modelInterfaces, parameter);
				}
			}
		}
		private void ProcessType(Type type) {
			if(!proccessedTypes.Contains(type)) {
				proccessedTypes.Add(type);
				if(ModelInterfacesHelper.IsTypeSuitableToGenerateClass(type)) {
					if(!IsAbstractModelInterface(type)) {
						interfacesToImplement.Add(type);
						if(IsCustomModelInterface(type)) {
							interfacesToGenerate.Add(type);
						}
					}
					ExploreModelInterface(type);
				}
				else if(ModelInterfacesHelper.IsGenericListType(type)) {
					Type parameter = type.GetGenericArguments()[0];
					ProcessType(parameter);
				}
			}
		}
		private static bool IsAbstractModelInterface(Type modelInterface) {
			return AttributeHelper.GetAttributes<ModelAbstractClassAttribute>(modelInterface, false).Length > 0;
		}
		private static bool IsCustomModelInterface(Type modelInterface) {
			return modelInterface.GetType() != RuntimeType;
		}
		private void ExploreModelInterface(Type modelInterface) {
			ExploreProperties(modelInterface);
			ExploreExtenders(modelInterface);
			ExploreImplementedInterfaces(modelInterface);
			ExploreImplementors(modelInterface);
		}
		private void ExploreProperties(Type modelInterface) {
			foreach(PropertyInfo propertyInfo in TypeHelper.GetProperties(modelInterface)) {
				ProcessType(propertyInfo.PropertyType);
			}
		}
		private void ExploreExtenders(Type modelInterface) {
			foreach(Type extenderType in interfaceExtenders.GetInterfaceExtenders(modelInterface)) {
				if(IsCustomModelInterface(extenderType)) {
					interfacesToGenerate.Add(extenderType);
				}
				ExploreProperties(extenderType);
			}
		}
		private void ExploreImplementedInterfaces(Type modelInterface) {
			foreach(Type implementedInterface in TypeHelper.GetInterfaces(modelInterface)) {
				ProcessType(implementedInterface);
			}
		}
		private void ExploreImplementors(Type modelInterface) {
			List<ITypeInfo> implementors = new List<ITypeInfo>(XafTypesInfo.Instance.FindTypeInfo(modelInterface).Implementors);
			foreach(ITypeInfo implementorInfo in implementors) {
				ProcessType(implementorInfo.Type);
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public struct CollectedInterfaces {
		private readonly Type[] interfacesToImplement;
		private readonly Type[] interfacesToGenerate;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CollectedInterfaces(Type[] interfacesToImplement, Type[] interfacesToGenerate) {
			Guard.ArgumentNotNull(interfacesToImplement, "interfacesToImplement");
			Guard.ArgumentNotNull(interfacesToGenerate, "interfacesToGenerate");
			this.interfacesToImplement = interfacesToImplement;
			this.interfacesToGenerate = interfacesToGenerate;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type[] InterfacesToImplement {
			get { return interfacesToImplement; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type[] InterfacesToGenerate {
			get { return interfacesToGenerate; }
		}
	}
}
