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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model.Core {
	public sealed class ModelApplicationCreatorProperties {
		static readonly Type DefaultModelApplicationNodeBaseInterface = typeof(IModelApplication);
		static readonly Type DefaultModelApplicationNodeBaseClass = typeof(ModelApplicationBase);
		readonly Type modelApplicationNodeBaseInterface;
		readonly Type modelApplicationNodeBaseClass;
		readonly CustomLogics customLogics;
		readonly ModelInterfaceExtenders interfaceExtenders;
		string assemblyFileAbsolutePath;
		Type[] interfacesToImplement;
		Type[] interfacesToGenerate;
		internal static ModelApplicationCreatorProperties CreateDefault() {
			return new ModelApplicationCreatorProperties(DefaultModelApplicationNodeBaseInterface, DefaultModelApplicationNodeBaseClass);
		}
		private ModelApplicationCreatorProperties(ModelApplicationCreatorProperties source) {
			interfaceExtenders = source.InterfacesExtenders.Clone();
			modelApplicationNodeBaseInterface = source.ModelApplicationNodeBaseInterface;
			modelApplicationNodeBaseClass = source.ModelApplicationNodeBaseClass;
			customLogics = source.CustomLogics.Clone();
			assemblyFileAbsolutePath = source.AssemblyFileAbsolutePath;
		}
		internal ModelApplicationCreatorProperties(Type modelApplicationNodeBaseInterface, Type modelApplicationNodeBaseClass) {
			Guard.ArgumentNotNull(modelApplicationNodeBaseInterface, "ModelApplicationNodeBaseInterface");
			Guard.ArgumentNotNull(modelApplicationNodeBaseClass, "modelApplicationNodeBaseClass");
			if(!modelApplicationNodeBaseInterface.IsInterface) {
				throw new ArgumentException(string.Format("The '{0}' type is not an interface.", modelApplicationNodeBaseInterface.FullName), "ModelApplicationNodeBaseInterface");
			}
			if(!modelApplicationNodeBaseClass.IsClass) {
				throw new ArgumentException(string.Format("The '{0}' type is not a class.", modelApplicationNodeBaseClass.FullName), "modelApplicationNodeBaseClass");
			}
			this.modelApplicationNodeBaseInterface = modelApplicationNodeBaseInterface;
			this.modelApplicationNodeBaseClass = modelApplicationNodeBaseClass;
			customLogics = new CustomLogics();
			interfaceExtenders = new ModelInterfaceExtenders();
		}
		public Type ModelApplicationNodeBaseInterface { get { return modelApplicationNodeBaseInterface; } }
		public Type ModelApplicationNodeBaseClass { get { return modelApplicationNodeBaseClass; } }
		public CustomLogics CustomLogics { get { return customLogics; } }
		public ModelInterfaceExtenders InterfacesExtenders { get { return interfaceExtenders; } }
		public string AssemblyFileAbsolutePath {
			get { return assemblyFileAbsolutePath; }
			set { assemblyFileAbsolutePath = value; }
		}
		internal void CollectInterfaces() {
			CollectedInterfaces result = new ModelInterfacesCollector().Collect(ModelApplicationNodeBaseInterface, InterfacesExtenders);
			interfacesToImplement = result.InterfacesToImplement;
			interfacesToGenerate = result.InterfacesToGenerate;
		}
		public IList<Type> GetInterfacesToGenerate() {
			return Array.AsReadOnly<Type>(interfacesToGenerate);
		}
		public IList<Type> GetInterfacesToImplement() {
			return Array.AsReadOnly<Type>(interfacesToImplement);
		}
		public ModelApplicationCreatorProperties Clone() {
			return new ModelApplicationCreatorProperties(this);
		}
		public override bool Equals(object obj) {
			if(this != obj) {
				ModelApplicationCreatorProperties properties = obj as ModelApplicationCreatorProperties;
				return properties != null
					&& ModelApplicationNodeBaseInterface == properties.ModelApplicationNodeBaseInterface
					&& ModelApplicationNodeBaseClass == properties.ModelApplicationNodeBaseClass
					&& CustomLogics.Equals(properties.CustomLogics)
					&& InterfacesExtenders.Equals(properties.InterfacesExtenders)
					&& assemblyFileAbsolutePath == properties.AssemblyFileAbsolutePath;
			}
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public static class ModelApplicationCreatorPropertiesHelper {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ModelApplicationCreatorProperties Create() {
			return ModelApplicationCreatorProperties.CreateDefault();
		}
	}
}
