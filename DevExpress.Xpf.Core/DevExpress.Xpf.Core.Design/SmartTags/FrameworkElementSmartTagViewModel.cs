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

extern alias Platform;
using System;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Design.SmartTags;
using DevExpress.Utils.Design;
using System.Windows;
using System.Windows.Controls;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#if !SL
using DevExpress.Xpf.Core.Design.Services;
using DevExpress.Mvvm;
#endif
using Microsoft.Windows.Design.Model;
using System.Windows.Input;
using DevExpress.Xpf.CreateLayoutWizard;
using DevExpress.Utils.About;
using System.Diagnostics;
using DevExpress.Xpf.Core.Design.Utils;
using DevExpress.Design.UI;
namespace DevExpress.Xpf.Core.Design {
	public class FrameworkElementSmartTagViewModel {
		public static bool IsPropertiesVisible { get; set; }
		public static bool IsBehaviorsVisible { get; set; }
		public static bool IsTabsVisible { get; private set; }
		public WpfDelegateCommand<Type> OpenHelp { get; set; }
		public WpfDelegateCommand CloseSmartTag { get; set; }
		static FrameworkElementSmartTagViewModel() {
			IsPropertiesVisible = true;
			IsTabsVisible = FrameworkElementSmartTagAdorner.VSVersion > 10;
		}
		public FrameworkElementSmartTagViewModel(IModelItem selectedItem) {
			ModelItem = selectedItem;
			PropertiesViewModel = new FrameworkElementSmartTagPropertiesViewModel(selectedItem);
#if !SL
			if(IsTabsVisible && ModelItem.Properties.FindProperty(DXServicesModelItemExtensions.DXBehaviorsProperty.Name, DXServicesModelItemExtensions.DXBehaviorsProperty.DeclaringType) != null)
				ServiceProviderViewModel = new ServiceProvider(ModelItem);
#endif
			OpenHelp = new WpfDelegateCommand<Type>(OnOpenHelp);
			CloseSmartTag = new WpfDelegateCommand(OnCloseSmartTag);
		}
		internal IModelItem ModelItem { get; set; }
		public FrameworkElementSmartTagPropertiesViewModel PropertiesViewModel { get; private set; }
#if !SL
		public ServiceProvider ServiceProviderViewModel { get; private set; }
#endif
		private void OnCloseSmartTag() {
			string messageBoxText = "Click OK to disable the DevExpress Smart Tags and Tasks panel for standard controls. You can enable this feature again via the DEVEXPRESS | Smart Tags main menu.";
			if(MessageBox.Show(messageBoxText, "DevExpress Smart Tags", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK) {
				DisabledSmartTagForStandardControls();
				HideSmartTag();
#if !SL && !DEBUG
#endif
			}
		}
		private void OnOpenHelp(Type itemType) {
			Uri uri = DocumentationUriHelper.GetUri(itemType);
			var procInfo = new ProcessStartInfo(uri.AbsoluteUri);
			try {
				Process.Start(procInfo);
			} catch(Exception) { }
		}
#if !SL
#endif
		void DisabledSmartTagForStandardControls() {
			var data = SharedMemoryDataHelper.GetSharedData();
			data.IsStandardSmartTagsEnabled = false;
			SharedMemoryDataHelper.UpdateSharedData(data);
		}
		void HideSmartTag() {
			SmartTagDesignService service = XpfModelItem.ToModelItem(ModelItem).Context.Services.GetService<SmartTagDesignService>();
			if(service != null)
				service.IsSmartTagButtonPressed = false;
		}
	}
}
