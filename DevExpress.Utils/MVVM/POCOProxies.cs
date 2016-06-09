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

namespace DevExpress.Utils.MVVM {
	using System;
	public interface IViewModelSource {
		object Create(Type viewModelType, params object[] parameters);
	}
	public interface IPOCOInterfaces {
		object GetParentViewModel(object viewModel);
		void SetParentViewModel(object viewModel, object parentViewModel);
		void SetParameter(object viewModel, object parameter);
		object GetDefaultServiceContainer();
		object GetServiceContainer(object viewModel);
		TService GetService<TService>(object serviceContainer, params object[] parameters) where TService : class;
		void RegisterService(object serviceContainer, params object[] parameters);
		void UnregisterService(object serviceContainer, object service);
		void SetDocumentOwner(object viewModel, object documentOwner);
		void OnClose(object viewModel, System.ComponentModel.CancelEventArgs e);
		void OnDestroy(object viewModel);
		object GetTitle(object viewModel);
	}
	sealed class POCOViewModelSourceProxy : IViewModelSource {
		internal static IViewModelSource Instance = new POCOViewModelSourceProxy();
		POCOViewModelSourceProxy() { }
		object IViewModelSource.Create(Type viewModelType, params object[] parameters) {
			var viewModelSourceType = MVVMTypesResolver.Instance.GetViewModelSourceType();
			return ViewModelSourceProxy.Create(viewModelSourceType, viewModelType, parameters);
		}
	}
	sealed class POCOInterfacesProxy : IPOCOInterfaces {
		internal static IPOCOInterfaces Instance = new POCOInterfacesProxy(MVVMTypesResolver.Instance);
		readonly IMVVMTypesResolver typesResolver;
		public POCOInterfacesProxy(IMVVMTypesResolver typesResolver) {
			this.typesResolver = typesResolver;
		}
		object IPOCOInterfaces.GetParentViewModel(object viewModel) {
			return Return(typesResolver.GetSupportParentViewModelType(), viewModel, supportParentViewModelType =>
				InterfacesProxy.GetParentViewModel(supportParentViewModelType, viewModel));
		}
		void IPOCOInterfaces.SetParentViewModel(object viewModel, object parentViewModel) {
			Do(typesResolver.GetSupportParentViewModelType(), viewModel, supportParentViewModelType =>
				InterfacesProxy.SetParentViewModel(supportParentViewModelType, viewModel, parentViewModel));
		}
		void IPOCOInterfaces.SetParameter(object viewModel, object parameter) {
			Do(typesResolver.GetSupportParameterType(), viewModel, supportParameterType =>
				InterfacesProxy.SetParameter(supportParameterType, viewModel, parameter));
		}
		object IPOCOInterfaces.GetDefaultServiceContainer() {
			var defaultServiceContainerType = typesResolver.GetDefaultServiceContainerType();
			return InterfacesProxy.GetDefaultServiceContainer(defaultServiceContainerType);
		}
		object IPOCOInterfaces.GetServiceContainer(object viewModel) {
			return Return(typesResolver.GetSupportServicesType(), viewModel, supportServicesType =>
				InterfacesProxy.GetServiceContainer(supportServicesType, viewModel));
		}
		TService IPOCOInterfaces.GetService<TService>(object serviceContainer, params object[] parameters) {
			return Return(typesResolver.GetServiceContainerType(), serviceContainer, serviceContainerType =>
				InterfacesProxy.GetService<TService>(serviceContainerType, serviceContainer, parameters));
		}
		void IPOCOInterfaces.RegisterService(object serviceContainer, params object[] parameters) {
			Do(typesResolver.GetServiceContainerType(), serviceContainer, serviceContainerType =>
				InterfacesProxy.RegisterService(serviceContainerType, serviceContainer, parameters));
		}
		void IPOCOInterfaces.UnregisterService(object serviceContainer, object service) {
			Do(typesResolver.GetServiceContainerType(), serviceContainer, serviceContainerType =>
				InterfacesProxy.UnregisterService(serviceContainerType, serviceContainer, service));
		}
		void IPOCOInterfaces.SetDocumentOwner(object viewModel, object documentOwner) {
			Do(GetIDocumentContentType(), viewModel, documentContentType =>
				Do(GetIDocumentOwnerType(), documentOwner, documentOwnerType =>
					InterfacesProxy.SetDocumentOwner(documentContentType, documentOwnerType, viewModel, documentOwner)));
		}
		void IPOCOInterfaces.OnClose(object viewModel, System.ComponentModel.CancelEventArgs e) {
			Do(GetIDocumentContentType(), viewModel, documentContentType =>
				InterfacesProxy.OnClose(documentContentType, viewModel, e));
		}
		void IPOCOInterfaces.OnDestroy(object viewModel) {
			Do(GetIDocumentContentType(), viewModel, documentContentType =>
				InterfacesProxy.OnDestroy(documentContentType, viewModel));
		}
		object IPOCOInterfaces.GetTitle(object viewModel) {
			return Return(GetIDocumentContentType(), viewModel, documentContentType =>
				 InterfacesProxy.GetTitle(documentContentType, viewModel));
		}
		static void Do(Type type, object obj, Action<Type> @do) {
			if((type != null) && (obj != null) && type.IsAssignableFrom(obj.GetType())) @do(type);
		}
		static T Return<T>(Type type, object obj, Func<Type, T> @result) {
			if((type != null) && (obj != null) && type.IsAssignableFrom(obj.GetType()))
				return @result(type);
			return default(T);
		}
		Type GetIDocumentContentType() {
			IMVVMServiceTypesResolver servicesTypeResolver = typesResolver as IMVVMServiceTypesResolver;
			return (servicesTypeResolver != null) ? servicesTypeResolver.GetIDocumentContentType() : null;
		}
		Type GetIDocumentOwnerType() {
			IMVVMServiceTypesResolver servicesTypeResolver = typesResolver as IMVVMServiceTypesResolver;
			return (servicesTypeResolver != null) ? servicesTypeResolver.GetIDocumentOwnerType() : null;
		}
	}
}
