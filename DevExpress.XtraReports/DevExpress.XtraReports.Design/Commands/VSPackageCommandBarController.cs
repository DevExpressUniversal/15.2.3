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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.VisualStudio.Shell;
using DevExpress.Utils.Design;
using Microsoft.VisualStudio.Shell.Interop;
using OLEInterop = Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;
using DevExpress.XtraReports.VSPackage;
namespace DevExpress.XtraReports.Design.Commands {
	  public class VSPackageCommandBarController : ICommandBarController {
		IServiceProvider serviceProvider;
		IControllerCallbacks callBacks;
		static IVsPackage package;
		MenuCommand packageToDesignMenuCommand;
		IMenuCommandService MenuService {
			get {
				return (IMenuCommandService)serviceProvider.GetService(typeof(IMenuCommandService));
			}
		}
		MenuCommandService PackageMenuService {
			get {
				IServiceProvider provider = GetPackage(serviceProvider) as IServiceProvider;
				return (MenuCommandService)provider.GetService(typeof(IMenuCommandService));
			}
		}
		public VSPackageCommandBarController(IServiceProvider serviceProvider, IControllerCallbacks callBacks) {
			this.serviceProvider = serviceProvider;
			this.callBacks = callBacks;
			packageToDesignMenuCommand = OleMenuCommandHelper.CreateOleMenuCommand(serviceProvider, new EventHandler(this.PackageToDesign), PackageInteractionCmdIDList.PackageToDesign);
		}
		public static bool IsPackagePresent(IServiceProvider serviceProvider) {
			return GetPackage(serviceProvider) != null;
		}
		static IVsPackage GetPackage(IServiceProvider serviceProvider) {
			if(package == null) {
				EnvDTE.DTE dte = serviceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
				if(dte.Edition.Contains("Express"))
					return null;
				IVsShell shell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
				if(shell == null) 
					return null;
				Guid xrVsPackageGuid = SharedGuidList.guidPackage;
				shell.LoadPackage(ref xrVsPackageGuid, out package);
			}
			return package;
		}
		void DoPackageAction(InteractionAction action, object parameter) {
			object o = new object[] { action, parameter };
			PackageMenuService.GlobalInvoke(PackageInteractionCmdIDList.DesignToPackage, o);
		}
		void Initizlize() {
			string[][] lists = {
				CommandBarHelper.GetFontFamilies(),
				CommandBarHelper.GetFontSizes(),
				CommandBarHelper.GetZoomFactors(), 
			};
			DoPackageAction(InteractionAction.FillComboBoxes, lists);
		}
		void PackageToDesign(object sender, EventArgs e) {
			object[] parameters = (object[])OleMenuCommandHelper.GetInValue(e);
			InteractionAction action = (InteractionAction)parameters[0];
			object parameter = parameters[1];
			switch(action) {
				case InteractionAction.ZoomIn:
					callBacks.ZoomIn();
					break;
				case InteractionAction.ZoomOut:
					callBacks.ZoomOut();
					break;
				case InteractionAction.ZoomChanged:
					callBacks.ZoomChanged((string)parameter);
					break;
				case InteractionAction.FontNameChanged:
					callBacks.FontNameChanged((string)parameter);
					break;
				case InteractionAction.FontSizeChanged:
					callBacks.FontSizeChanged((string)parameter);
					break;
			}
		}
		public void Activate() {
			Initizlize();
			SetCommandBarEnabled(true);
		}
		public void ToggleCommandBarVisibility() {
		}
		public void Deactivate() {
			SetCommandBarEnabled(false);
		}
		public void Close() {
		}
		public void Dispose() {
			UnsubscribeToPackage();
		}
		public void SetZoomFactorsText(string text) {
			DoPackageAction(InteractionAction.SetZoomFactorsText, text);
		}
		public void OnDesignerActivated() {
			SubscribeToPackage();
		}
		public void OnDesignerDeactivated() {
			UnsubscribeToPackage();
		}
		public void UpdateFontControls(string fontName, string size) {
			DoPackageAction(InteractionAction.UpdateFont, new string[] { fontName, size });
		}
		public void SetFontControlsVisibility(bool enabled) {
			DoPackageAction(InteractionAction.SetFontControlsVisibility, enabled);
		}
		public void ResetFontControls() {
			DoPackageAction(InteractionAction.ResetFont, null);
		}
		void SetCommandBarEnabled(bool enabled) {
			DoPackageAction(InteractionAction.SetCommandBarEnabled, enabled);
		}
		void SubscribeToPackage() {
			MenuService.AddCommand(packageToDesignMenuCommand);
		}
		void UnsubscribeToPackage() {
			MenuService.RemoveCommand(packageToDesignMenuCommand);
		}
	}
}
