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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Utils {
	[DomainComponent, Browsable(false)]
	public class ParametersObject {
		private ParameterList parameters;
		private ParametersObject(ParameterList parameters) {
			this.parameters = parameters;
		}
		private static ParametersObjectProvider descriptorProvider;
		public static ParametersObject CreateBoundObject(ParameterList parameters) {
			ParametersObject result = new ParametersObject(parameters);
			if(descriptorProvider == null) {
				descriptorProvider = new ParametersObjectProvider();
				TypeDescriptor.AddProvider(descriptorProvider, typeof(ParametersObject));
			}
			descriptorProvider.RegisterParameters(parameters);
			XafTypesInfo.Instance.RefreshInfo(typeof(ParametersObject));
			return result;
		}
		public DetailView CreateDetailView(IObjectSpace objectSpace, XafApplication application, Boolean isRoot) {
			IModelDetailView modelDetailView = TempDetailViewHelper.CreateTempDetailViewModel(application.Model, typeof(ParametersObject));
			for(int i = modelDetailView.Items.Count - 1; i >= 0 ; i--) {
				IModelMemberViewItem viewItem = (IModelMemberViewItem)modelDetailView.Items[i];
				IParameter parameter;
				parameters.TryGetValue(viewItem.Id, out parameter);
				if(parameter != null && parameter.Visible) {
					viewItem.Caption = parameter.Caption;
					viewItem.IsPassword = parameter.IsMasked;
					}
				else {
					viewItem.Remove();
				}
			}
			int index = 0;
			foreach(IParameter parameter in parameters.Values) {
				if(parameter.Visible) {
					modelDetailView.Items[parameter.Name].Index = index++;
				}
			}
			DetailView result = new DetailView(modelDetailView, objectSpace, this, application, isRoot);
			return result;
		}
		[Browsable(false)]
		public ParameterList Parameters {
			get {
				return parameters;
			}
		}
	}
	public static class TempDetailViewHelper {
		public static IModelDetailView CreateTempDetailViewModel(IModelApplication applicationModel, Type objectType) {
			Guard.ArgumentNotNull(applicationModel, "applicationModel");
			Guard.ArgumentNotNull(objectType, "objectType");
			ModelApplicationCreator creator = ((ModelNode)applicationModel).CreatorInstance;
			ModelApplicationBase master = creator.CreateModelApplication();
			master.AddLayerInternal(creator.CreateModelApplication());
			master.AddLayerInternal(creator.CreateModelApplication());
			List<Type> requiredTypes = new List<Type>(((IModelSources)applicationModel).BOModelTypes);
			requiredTypes.Add(objectType);
			((IModelSources)master).BOModelTypes = requiredTypes;
			((IModelSources)master).EditorDescriptors = ((IModelSources)applicationModel).EditorDescriptors;
			IModelApplication temp = (IModelApplication)master;
			temp.Options.LookupSmallCollectionItemCount = applicationModel.Options.LookupSmallCollectionItemCount;
			IModelDetailView result = temp.BOModel.GetClass(objectType).DefaultDetailView;
			return result;
		}
	}
	internal class ParameterPropertyDescriptor : PropertyDescriptor {
		private Type parameterType;
		public ParameterPropertyDescriptor(IParameter parameter)
			: base(parameter.Name, new Attribute[] { new DisplayNameAttribute(parameter.Caption) }) {
			this.parameterType = parameter.Type;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override Type ComponentType {
			get { return typeof(ParametersObject); }
		}
		public override object GetValue(object component) {
			return ((ParametersObject)component).Parameters[base.Name].CurrentValue;
		}
		public override bool IsReadOnly {
			get { return false; }
		}
		public override Type PropertyType {
			get { return parameterType; }
		}
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
			((ParametersObject)component).Parameters[base.Name].CurrentValue = value;
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
	}
	internal class ParameterListCustomTypeDescriptor : CustomTypeDescriptor {
		PropertyDescriptorCollection propertyDescriptors;
		public ParameterListCustomTypeDescriptor() {
			propertyDescriptors = new PropertyDescriptorCollection(null, false);
		}
		internal void RegisterParameters(ParameterList parameters) {
			foreach(IParameter parameter in parameters.Values) {
				if(propertyDescriptors.Find(parameter.Name, false) == null) {
					propertyDescriptors.Add(new ParameterPropertyDescriptor(parameter));
				}
			}
		}
		public override PropertyDescriptorCollection GetProperties() {
			return propertyDescriptors;
		}
	}
	internal class ParametersObjectProvider : TypeDescriptionProvider {
		private ParameterListCustomTypeDescriptor allParameters;
		protected internal void RegisterParameters(ParameterList parameters) {
			allParameters.RegisterParameters(parameters);
		}
		public ParametersObjectProvider() {
			this.allParameters = new ParameterListCustomTypeDescriptor();
		}
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
			if(typeof(ParametersObject) == objectType) {
				return allParameters;
			}
			return base.GetTypeDescriptor(objectType, instance);
		}
	}
}
