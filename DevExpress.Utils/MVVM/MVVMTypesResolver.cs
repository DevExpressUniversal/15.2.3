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
	using System.Collections.Generic;
	public interface IMVVMTypesResolver {
		Type GetViewModelSourceType();
		Type GetSupportParentViewModelType();
		Type GetSupportParameterType();
		Type GetSupportServicesType();
		Type GetServiceContainerType();
		Type GetUICommandType();
		Type GetAsyncCommandType();
		Type GetCommandBaseType();
		Type GetCommandAttributeType();
		Type GetCommandParameterAttributeType();
		Type GetBindablePropertyAttributeType();
		Type GetDefaultServiceContainerType();
		Type GetMetadataHelperType();
		Type GetAttributeType(string attributeType);
		Type GetIPreviewModelWrapperType();
	}
	public interface IMVVMServiceTypesResolver {
		Type GetIDispatcherServiceType();
		Type GetIDialogServiceType();
		Type GetIMessageBoxServiceType();
		Type GetIDocumentManagerServiceType();
		Type GetIDocumentContentType();
		Type GetIDocumentOwnerType();
		Type GetIDocumentType();
		Type GetIDocumentInfoType();
		Type GetIMessageButtonLocalizerType();
		Type GetINotificationType();
		Type GetINotificationServiceType();
		Type GetNotificationResultType();
		Type GetISplashScreenServiceType();
		Type GetILayoutSerializationServiceType();
		Type GetIPrintableControlPreviewServiceType();
	}
	sealed class MVVMTypesResolver : IMVVMTypesResolver, IMVVMServiceTypesResolver {
		readonly internal static IMVVMTypesResolver Instance = new MVVMTypesResolver();
		MVVMTypesResolver() { }
		static Type viewModelSourceType;
		Type IMVVMTypesResolver.GetViewModelSourceType() {
			return MVVMAssemblyProxy.GetMvvmType(ref viewModelSourceType, "POCO.ViewModelSource");
		}
		static Type supportParentViewModelType;
		Type IMVVMTypesResolver.GetSupportParentViewModelType() {
			return MVVMAssemblyProxy.GetMvvmType(ref supportParentViewModelType, "ISupportParentViewModel");
		}
		static Type supportParameterType;
		Type IMVVMTypesResolver.GetSupportParameterType() {
			return MVVMAssemblyProxy.GetMvvmType(ref supportParameterType, "ISupportParameter");
		}
		static Type supportServicesType;
		Type IMVVMTypesResolver.GetSupportServicesType() {
			return MVVMAssemblyProxy.GetMvvmType(ref supportServicesType, "ISupportServices");
		}
		static Type serviceContainerType;
		Type IMVVMTypesResolver.GetServiceContainerType() {
			return MVVMAssemblyProxy.GetMvvmType(ref serviceContainerType, "IServiceContainer");
		}
		static Type uiCommandType;
		Type IMVVMTypesResolver.GetUICommandType() {
			return MVVMAssemblyProxy.GetMvvmType(ref uiCommandType, "UICommand");
		}
		static Type commandBaseType;
		Type IMVVMTypesResolver.GetCommandBaseType() {
			return MVVMAssemblyProxy.GetMvvmType(ref commandBaseType, "CommandBase");
		}
		static Type commandAttributeType;
		Type IMVVMTypesResolver.GetCommandAttributeType() {
			return MVVMAssemblyProxy.GetMvvmType(ref commandAttributeType, "DataAnnotations.CommandAttribute");
		}
		static Type commandParameterAttributeType;
		Type IMVVMTypesResolver.GetCommandParameterAttributeType() {
			return MVVMAssemblyProxy.GetMvvmType(ref commandParameterAttributeType, "DataAnnotations.CommandParameterAttribute");
		}
		static Type bindablePropertyAttributeType;
		Type IMVVMTypesResolver.GetBindablePropertyAttributeType() {
			return MVVMAssemblyProxy.GetMvvmType(ref bindablePropertyAttributeType, "DataAnnotations.BindablePropertyAttribute");
		}
		static Type asyncCommandType;
		Type IMVVMTypesResolver.GetAsyncCommandType() {
			return MVVMAssemblyProxy.GetMvvmType(ref asyncCommandType, "Native.IAsyncCommand");
		}
		static Type defaultServiceContainerType;
		Type IMVVMTypesResolver.GetDefaultServiceContainerType() {
			return MVVMAssemblyProxy.GetMvvmType(ref defaultServiceContainerType, "ServiceContainer");
		}
		static Type metadataHelperType;
		Type IMVVMTypesResolver.GetMetadataHelperType() {
			return MVVMAssemblyProxy.GetMvvmType(ref metadataHelperType, "Native.MetadataHelper");
		}
		static IDictionary<string, Type> attributeTypes = new Dictionary<string, Type>();
		Type IMVVMTypesResolver.GetAttributeType(string attributeTypeName) {
			Type attributeType;
			if(!attributeTypes.TryGetValue(attributeTypeName, out attributeType)) {
				MVVMAssemblyProxy.GetMvvmType(ref attributeType, attributeTypeName);
				attributeTypes.Add(attributeTypeName, attributeType);
			}
			return attributeType;
		}
		static Type previewModelWrapperType;
		Type IMVVMTypesResolver.GetIPreviewModelWrapperType() {
			return MVVMAssemblyProxy.GetMvvmType(ref previewModelWrapperType, "IPreviewModelWrapper");
		}
		static Type messageBoxServiceType;
		Type IMVVMServiceTypesResolver.GetIMessageBoxServiceType() {
			return MVVMAssemblyProxy.GetMvvmType(ref messageBoxServiceType, "IMessageBoxService");
		}
		static Type documentManagerServiceType;
		Type IMVVMServiceTypesResolver.GetIDocumentManagerServiceType() {
			return MVVMAssemblyProxy.GetMvvmType(ref documentManagerServiceType, "IDocumentManagerService");
		}
		static Type dispatcherServiceType;
		Type IMVVMServiceTypesResolver.GetIDispatcherServiceType() {
			return MVVMAssemblyProxy.GetMvvmType(ref dispatcherServiceType, "IDispatcherService");
		}
		static Type dialogServiceType;
		Type IMVVMServiceTypesResolver.GetIDialogServiceType() {
			return MVVMAssemblyProxy.GetMvvmType(ref dialogServiceType, "IDialogService");
		}
		static Type documentContentType;
		Type IMVVMServiceTypesResolver.GetIDocumentContentType() {
			return MVVMAssemblyProxy.GetMvvmType(ref documentContentType, "IDocumentContent");
		}
		static Type documentOwnerType;
		Type IMVVMServiceTypesResolver.GetIDocumentOwnerType() {
			return MVVMAssemblyProxy.GetMvvmType(ref documentOwnerType, "IDocumentOwner");
		}
		static Type documentType;
		Type IMVVMServiceTypesResolver.GetIDocumentType() {
			return MVVMAssemblyProxy.GetMvvmType(ref documentType, "IDocument");
		}
		static Type documentInfoType;
		Type IMVVMServiceTypesResolver.GetIDocumentInfoType() {
			return MVVMAssemblyProxy.GetMvvmType(ref documentInfoType, "IDocumentInfo");
		}
		static Type messageButtonLocalizerType;
		Type IMVVMServiceTypesResolver.GetIMessageButtonLocalizerType() {
			return MVVMAssemblyProxy.GetMvvmType(ref messageButtonLocalizerType, "IMessageButtonLocalizer");
		}
		static Type notificationType;
		Type IMVVMServiceTypesResolver.GetINotificationType() {
			return MVVMAssemblyProxy.GetMvvmType(ref notificationType, "INotification");
		}
		static Type notificationServiceType;
		Type IMVVMServiceTypesResolver.GetINotificationServiceType() {
			return MVVMAssemblyProxy.GetMvvmType(ref notificationServiceType, "INotificationService");
		}
		static Type notificationResultType;
		Type IMVVMServiceTypesResolver.GetNotificationResultType() {
			return MVVMAssemblyProxy.GetMvvmType(ref notificationResultType, "NotificationResult");
		}
		static Type splashScreenServiceType;
		Type IMVVMServiceTypesResolver.GetISplashScreenServiceType() {
			return MVVMAssemblyProxy.GetMvvmType(ref splashScreenServiceType, "ISplashScreenService");
		}
		static Type layoutSerializationServiceType;
		Type IMVVMServiceTypesResolver.GetILayoutSerializationServiceType() {
			return MVVMAssemblyProxy.GetMvvmType(ref layoutSerializationServiceType, "ILayoutSerializationService");
		}
		static Type printableControlServiceType;
		Type IMVVMServiceTypesResolver.GetIPrintableControlPreviewServiceType() {
			return MVVMAssemblyProxy.GetMvvmType(ref printableControlServiceType, "IPrintableControlPreviewService");
		}
		internal static void Reset() {
			MVVMAssemblyProxy.Reset();
			attributeTypes.Clear();
		}
	}
}
