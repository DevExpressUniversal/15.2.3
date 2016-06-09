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

namespace DevExpress.Utils.MVVM.Internal {
	using System;
	sealed class POCOTypesResolver : IMVVMTypesResolver {
		readonly internal static IMVVMTypesResolver Instance = new POCOTypesResolver();
		Type IMVVMTypesResolver.GetViewModelSourceType() {
			return typeof(POCOSource);
		}
		Type IMVVMTypesResolver.GetSupportParentViewModelType() {
			return null;
		}
		Type IMVVMTypesResolver.GetSupportParameterType() {
			return null;
		}
		Type IMVVMTypesResolver.GetSupportServicesType() {
			return null;
		}
		Type IMVVMTypesResolver.GetServiceContainerType() {
			return null;
		}
		Type IMVVMTypesResolver.GetUICommandType() {
			return null;
		}
		Type IMVVMTypesResolver.GetAsyncCommandType() {
			return typeof(IAsyncCommand);
		}
		Type IMVVMTypesResolver.GetCommandBaseType() {
			return typeof(CommandBase);
		}
		Type IMVVMTypesResolver.GetCommandAttributeType() {
			return null;
		}
		Type IMVVMTypesResolver.GetCommandParameterAttributeType() {
			return null;
		}
		Type IMVVMTypesResolver.GetBindablePropertyAttributeType() {
			return null;
		}
		Type IMVVMTypesResolver.GetDefaultServiceContainerType() {
			return null;
		}
		Type IMVVMTypesResolver.GetMetadataHelperType() {
			return null;
		}
		Type IMVVMTypesResolver.GetAttributeType(string attributeType) {
			return null;
		}
		public Type GetIPreviewModelWrapperType() {
			return null;
		}
	}
}
