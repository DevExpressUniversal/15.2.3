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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.BinderSettings;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc {
	public class DevExpressEditorsModelBinderDictionary : ModelBinderDictionary {
		public DevExpressEditorsModelBinderDictionary(DevExpressEditorsBinder binder) {
			foreach(Type type in ModelBinders.Binders.Keys)
				Add(type, ModelBinders.Binders[type]);
			DefaultBinder = binder;
		}
	}
	public class DevExpressEditorsBinder : DefaultModelBinder {
		public DevExpressEditorsBinder()
			: base() {
			DevExpressEditorsBinder defaultBinder = this.Binders.DefaultBinder as DevExpressEditorsBinder;
			Binders = new DevExpressEditorsModelBinderDictionary(defaultBinder != null ? defaultBinder : this);
			ExtensionValueProvidersFactory = defaultBinder != null ? defaultBinder.ExtensionValueProvidersFactory : new ExtensionValueProvidersFactory();
		}
		public HtmlEditorBinderSettings HtmlEditorBinderSettings {
			get { return (HtmlEditorBinderSettings)ExtensionValueProvidersFactory.GetValueProviderSettings(typeof(HtmlEditorValueProvider)); }
		}
		public UploadControlBinderSettings UploadControlBinderSettings {
			get { return (UploadControlBinderSettings)ExtensionValueProvidersFactory.GetValueProviderSettings(typeof(UploadControlValueProvider)); }
		}
		protected ExtensionValueProvidersFactory ExtensionValueProvidersFactory { get; set; }
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			MvcUtils.ModelBinderProcessing = true;
			try {
				object value;
				if(TryGetDxExtensionValue(bindingContext, controllerContext, out value))
					return value;
				if(IsGridViewBatchEditModel(bindingContext.ModelType))
					PrepareBindingContextForBatchEditMode(bindingContext, controllerContext);
				return base.BindModel(controllerContext, bindingContext);
			} finally {
				MvcUtils.ModelBinderProcessing = false;
			}
		}
		bool IsGridViewBatchEditModel(Type modelType) {
			return modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(MVCxGridViewBatchUpdateValues<,>);
		}
		void PrepareBindingContextForBatchEditMode(ModelBindingContext bindingContext, ControllerContext controllerContext) {
			bindingContext.ValueProvider = MVCxGridBatchEditHelperAdapter.CreateValueProvider(bindingContext);
			if(bindingContext.Model == null)
				bindingContext.ModelMetadata.Model = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);
			ReflectionUtils.SetNonPublicInstanceFieldValue(bindingContext.Model, "modelState", bindingContext.ModelState);
		}
		protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor) {
			TrySetDxValueProvider(bindingContext, propertyDescriptor);
			base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
			ResetValueProvider(bindingContext);
		}
		protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext,
			PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder) {
			ResetValueProvider(bindingContext);
			object value;
			if(TryGetDxExtensionValue(bindingContext, controllerContext, out value))
				return value;
			return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
		}
		void TrySetDxValueProvider(ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor) {
			string fullModelName = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
			ExtensionValueProviderBase exValueProvider = ExtensionValueProvidersFactory.GetValueProvider(bindingContext, fullModelName);
			string propertyPostfix = exValueProvider != null ? exValueProvider.PropertyPostfix : null;
			bool isBatchEditModel = IsGridViewBatchEditModel(bindingContext.ModelType);
			if(bindingContext.ValueProvider is IUnvalidatedValueProvider && (isBatchEditModel || !string.IsNullOrEmpty(propertyPostfix)))
				bindingContext.ValueProvider = new MvcPostDataCollection(bindingContext.ValueProvider, isBatchEditModel, propertyPostfix);
		}
		void ResetValueProvider(ModelBindingContext bindinigContext) {
			var dxValueProvider = bindinigContext.ValueProvider as MvcPostDataCollection;
			if(dxValueProvider != null && !dxValueProvider.IsBatchEditCollection)
				bindinigContext.ValueProvider = dxValueProvider.ValueProvider;
		}
		protected bool TryGetDxExtensionValue(ModelBindingContext bindingContext, ControllerContext controllerContext,  out object value) {
			value = null;
			var exValueProvider = ExtensionValueProvidersFactory.GetValueProvider(bindingContext, bindingContext.ModelName);
			if(exValueProvider == null)
				return false;
			value = exValueProvider.GetValue(bindingContext, controllerContext);
			return true;
		}
	}
}
