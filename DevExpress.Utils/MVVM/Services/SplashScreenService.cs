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

namespace DevExpress.Utils.MVVM.Services {
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;
	public interface ISplashScreenServiceProvider {
		void ShowSplashScreen(Func<Control> createFormFunction);
		void SetSplashScreenProgress(double progress, double maxProgress);
		void SetSplashScreenState(object state);
		void HideSplashScreen();
		bool IsSplashScreenActive { get; }
	}
	public class SplashScreenService : ViewServiceBase {
		ISplashScreenServiceProvider serviceProviderCore;
		protected SplashScreenService(ISplashScreenServiceProvider serviceProvider) {
			this.serviceProviderCore = serviceProvider;
		}
		public void ShowSplashScreen(string documentType) {
			if(serviceProviderCore == null) return;
			serviceProviderCore.ShowSplashScreen(CreateControl(documentType));
		}
		Func<Control> CreateControl(string documentType) {
			if(String.IsNullOrEmpty(documentType)) return null;
			else return () => base.CreateAndInitializeView(documentType, null, null, null);
		}
		public void SetSplashScreenProgress(double progress, double maxProgress) {
			if(serviceProviderCore == null) return;
			serviceProviderCore.SetSplashScreenProgress(progress, maxProgress);
		}
		public void SetSplashScreenState(object state) {
			if(serviceProviderCore == null) return;
			serviceProviderCore.SetSplashScreenState(state);
		}
		public void HideSplashScreen() {
			if(serviceProviderCore == null) return;
			serviceProviderCore.HideSplashScreen();
		}
		public bool IsSplashScreenActive {
			get {
				if(serviceProviderCore == null) return false;
				return serviceProviderCore.IsSplashScreenActive;
			}
		}
		public static SplashScreenService Create(ISplashScreenServiceProvider serviceProvider) {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			return DynamicServiceSource.Create<SplashScreenService, ISplashScreenServiceProvider>(
				typesResolver.GetISplashScreenServiceType(), serviceProvider);
		}
		internal static void Register() {
			IPOCOInterfaces pocoInterfaces = POCOInterfacesProxy.Instance;
			object serviceContainer = pocoInterfaces.GetDefaultServiceContainer();
			var service = SplashScreenService.Create(CreateServiceProvider());
			pocoInterfaces.RegisterService(serviceContainer, service);
		}
		static ISplashScreenServiceProvider CreateServiceProvider() {
			Type splashScreenType = null;
			var editorsAssembly = AssemblyHelper.GetLoadedAssembly(AssemblyInfo.SRAssemblyEditors) ?? AssemblyHelper.LoadDXAssembly(AssemblyInfo.SRAssemblyEditors);
			if(editorsAssembly != null)
				splashScreenType = editorsAssembly.GetType("DevExpress.XtraSplashScreen.SplashScreenManager");
			return CreateSplashScreenServiceProvider(splashScreenType);
		}
		static ISplashScreenServiceProvider CreateSplashScreenServiceProvider(Type splashScreenType) {
			return splashScreenType == null ? null : Activator.CreateInstance(splashScreenType) as ISplashScreenServiceProvider;
		}
	}
}
