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

#if !MVVM
using DevExpress.Xpf.Core.Native;
#endif
using DevExpress.Mvvm.UI.Interactivity;
using System.Windows;
using System;
using DevExpress.Mvvm.Native;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#if FREE
using DevExpress.Mvvm.UI.Native;
#endif
#else
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Navigation;
using DevExpress.Mvvm.UI.Native;
#endif
namespace DevExpress.Mvvm.UI {
#if !SILVERLIGHT && !NETFX_CORE
	[RuntimeNameProperty("Name")]
#endif
	public abstract class ServiceBase : Behavior<FrameworkElement> {
#if !NETFX_CORE
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(ServiceBase), new PropertyMetadata(null));
		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
#endif
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ServicesClientInternalProperty = DependencyProperty.Register("ServicesClientInternal", typeof(object), typeof(ServiceBase), new PropertyMetadata(null,
			(d, e) => ((ServiceBase)d).OnServicesClientInternalChanged(e.OldValue as ISupportServices, e.NewValue as ISupportServices)));
		public bool YieldToParent { get; set; }
		internal bool ShouldInject { get; set; }
		protected ServiceBase() {
			ShouldInject = true;
		}
		protected override void OnAttached() {
			base.OnAttached();
			SetInjectBinding();
		}
		protected override void OnDetaching() {
#if SILVERLIGHT || NETFX_CORE
			ClearValue(ServicesClientInternalProperty);
#else
			BindingOperations.ClearBinding(this, ServicesClientInternalProperty);
#endif
			base.OnDetaching();
		}
		void SetInjectBinding() {
			if(ShouldInject)
				BindingOperations.SetBinding(this, ServicesClientInternalProperty, new Binding() { Path = new PropertyPath("DataContext"), Source = AssociatedObject });
		}
#if NETFX_CORE
		protected override void OnDataContextChange(object dataContext) {
			base.OnDataContextChange(dataContext);
			SetInjectBinding();
		}
#endif
		protected ISupportServices GetServicesClient() {
			return GetValue(ServicesClientInternalProperty) as ISupportServices;
		}
#if !SILVERLIGHT && !NETFX_CORE
		protected Uri GetBaseUri() {
			Uri baseUri = BaseUriHelper.GetBaseUri(this);
			if(baseUri != null || AssociatedObject == null) return baseUri;
			return BaseUriHelper.GetBaseUri(AssociatedObject);
		}
#endif
		void OnServicesClientInternalChanged(ISupportServices oldServiceClient, ISupportServices newServiceClient) {
			oldServiceClient.Do(x => x.ServiceContainer.UnregisterService(this));
			newServiceClient.Do(x => x.ServiceContainer.RegisterService(Name, this, YieldToParent));
		}
	}
}
#if !FREE && !NETFX_CORE
namespace DevExpress.Mvvm.UI.Native {
	public static class AssignableServiceHelper<TOwner, TService> 
		where TOwner : FrameworkElement 
		where TService : class 
	{
		public static DependencyProperty RegisterServiceProperty(string name) {
			return DependencyProperty.Register(name, typeof(TService), typeof(TOwner), new PropertyMetadata(null, OnServicePropertyChanged));
		}
		static void OnServicePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			(e.OldValue as ServiceBase).Do(x => x.Detach());
			(e.NewValue as ServiceBase).Do(x => {
				x.ShouldInject = false;
				x.Attach(d); 
			});
		}
		public static TService GetService(TOwner owner, DependencyProperty property, object templateKey) {
			var service = (TService)owner.GetValue(property);
			if(service != null)
				return service;
			service = LoadServiceFromTemplate(owner, templateKey);
#if SILVERLIGHT  || NETFX_CORE
			owner.SetValue(property, service);
#else
			owner.SetCurrentValue(property, service);
#endif
			return service;
		}
		static TService LoadServiceFromTemplate(TOwner owner, object templateKey) {
			var template = (DataTemplate)DevExpress.Xpf.Core.ResourceHelper.FindResource(owner, templateKey);
			return TemplateHelper.LoadFromTemplate<TService>(template);
		}
	}
	public static class AssignableServiceHelper2<TOwner, TService>
		where TService : class {
		public static DependencyProperty RegisterServiceTemplateProperty(string name) {
			return RegisterServiceTemplateProperty<DependencyObject>(name, null);
		}
		public static DependencyProperty RegisterServiceTemplateProperty<T>(string name, Action<T> onChanged) {
			var metadata = onChanged == null ? new PropertyMetadata(null) : new PropertyMetadata(null, (d, e) => onChanged((T)(object)d));
			return DependencyProperty.Register(name, typeof(DataTemplate), typeof(TOwner), metadata);
		}
		public static void DoServiceAction(DependencyObject owner, DataTemplate template, Action<TService> action) {
			TService service = TemplateHelper.LoadFromTemplate<TService>(template);
			(service as ServiceBase).Do(x => {
				x.ShouldInject = false;
				x.Attach(owner);
			});
			try {
				action(service);
			} finally {
				(service as ServiceBase).Do(x => x.Detach());
			}
		}
		public static void DoServiceAction(DependencyObject owner, TService service, Action<TService> action) {
			(service as ServiceBase).Do(x => {
				x.ShouldInject = false;
				x.Attach(owner);
			});
			try {
				action(service);
			} finally {
				(service as ServiceBase).Do(x => x.Detach());
			}
		}
	}
}
#endif
