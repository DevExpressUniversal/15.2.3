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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Editors.DateNavigator;
using System.ComponentModel.Design;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.DateNavigator.Internal;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
#if SL
using FrameworkContentElement = System.Windows.FrameworkElement;
#endif
namespace DevExpress.Xpf.Editors.DateNavigator {
	public abstract class DateNavigatorStyleSettingsBase : FrameworkContentElement, IServiceContainer
		#if SL
		, ISupportInitialize
		#endif
	{
		protected DateNavigator Navigator { get; private set; }
		protected IServiceContainer ServiceContainer { get { return (IServiceContainer)this; } }
		Dictionary<Type, object> RegisteredServices { get; set; }
		public DateNavigatorStyleSettingsBase() {
			RegisteredServices = new Dictionary<Type, object>();
		}
		protected virtual void RegisterDefaultServices() {
			RegisterDefaultService(typeof(IValueEditingService), new ValueEditingStrategy(Navigator));
			RegisterDefaultService(typeof(INavigationService), CreateNavigationService());
			RegisterDefaultService(typeof(INavigationCallbackService), new DummyNavigationCallbackService());
			RegisterDefaultService(typeof(IValueValidatingService), new ValueValidatingStrategy(Navigator));
			RegisterDefaultService(typeof(IDateCalculationService), new DateNavigatorWorkdayCalculator(Navigator));
		}
		protected void RegisterService(Type serviceType, object serviceInstance) {
			ServiceContainer.RemoveService(serviceType);
			ServiceContainer.AddService(serviceType, serviceInstance);
		}
		protected void RegisterDefaultService(Type serviceType, object serviceInstance) {
			if (RegisteredServices.ContainsKey(serviceType))
				return;
			RegisteredServices.Add(serviceType, serviceInstance);
		}
		protected abstract INavigationService CreateNavigationService();
		protected internal virtual void Initialize(DateNavigator navigator) {
			Navigator = navigator;
			RegisterDefaultServices();
		}
		protected internal virtual void RegisterNavigationService() {
			RegisterService(typeof(INavigationService), CreateNavigationService());
		}
		#region IServiceContainer Members
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			ServiceContainer.AddService(serviceType, callback);
		}
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback) {
			ServiceContainer.AddService(serviceType, callback(this, serviceType));
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) {
			ServiceContainer.AddService(serviceType, serviceInstance);
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance) {
			RegisteredServices.Add(serviceType, serviceInstance);
		}
		void IServiceContainer.RemoveService(Type serviceType, bool promote) {
			ServiceContainer.RemoveService(serviceType);
		}
		void IServiceContainer.RemoveService(Type serviceType) {
			if (RegisteredServices.ContainsKey(serviceType))
				RegisteredServices.Remove(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		internal T GetService<T>() {
			return (T)ServiceContainer.GetService(typeof(T));
		}
		object IServiceProvider.GetService(Type serviceType) {
			return GetServiceInternal(serviceType);
		}
		protected virtual object GetServiceInternal(Type serviceType) {
			object result;
			RegisteredServices.TryGetValue(serviceType, out result);
			return result;
		}
		#endregion
#if SL
		#region ISupportInitialize Members
		public virtual void BeginInit() { }
		public virtual void EndInit() { }
		#endregion
#endif
	}
}
