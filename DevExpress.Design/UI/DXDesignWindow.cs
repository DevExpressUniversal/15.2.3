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
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using DevExpress.Utils;
using DevExpress.Utils.Design;
namespace DevExpress.Design.UI {
	public class DXDesignWindow : Window, IDXDesignWindow {
		static DXDesignWindow() {
			var dProp = new DependencyPropertyRegistrator<DXDesignWindow>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		public DXDesignWindow() {
			ApplyOptions();
		}
		protected virtual void ApplyOptions() {
			ShowInTaskbar = false;
			WindowStyle = System.Windows.WindowStyle.None;
			AllowsTransparency = true;
			WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			ResizeMode = System.Windows.ResizeMode.NoResize;
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if(!e.Handled) DragMove();
		}
		public static bool? ShowModal(DXDesignWindow wizardWindow) {
			return ShowModal(wizardWindow, (bool?)null);
		}
		public static bool? ShowModal(DXDesignWindow wizardWindow, bool? isVS2013) {
			var vsUIShell = DevExpress.Utils.Design.DTEHelper.GetVsUIShell();
			bool allowModeless = !isVS2013.HasValue || !isVS2013.Value;
			if(allowModeless) {
				if(vsUIShell != null)
					vsUIShell.EnableModeless(0);
			}
			try {
				if(vsUIShell != null) {
					System.IntPtr dlgOwner;
					vsUIShell.GetDialogOwnerHwnd(out dlgOwner);
					new System.Windows.Interop.WindowInteropHelper(wizardWindow) { Owner = dlgOwner };
				}
				return wizardWindow.ShowDialog();
			}
			finally {
				if(allowModeless) {
					if(vsUIShell != null)
						vsUIShell.EnableModeless(1);
				}
			}
		}
		public static bool? ShowModal(DXDesignWindow wizardWindow, Func<DXDesignWindow, IntPtr> requestHandleCallback) {
			Guard.ArgumentNotNull(requestHandleCallback, "requestHandleCallback");
			var vsUIShell = GetVsUIShell();
			if(vsUIShell != null) {
				return ShowModal(wizardWindow);
			}
			IntPtr dlgOwner = requestHandleCallback(wizardWindow);
			new WindowInteropHelper(wizardWindow) { Owner = dlgOwner };
			return wizardWindow.ShowDialog();
		}
		static Microsoft.VisualStudio.Shell.Interop.IVsUIShell GetVsUIShell() {
			if(DTEHelper.IsVisualStudioExpressVersion)
				return null;
			return DTEHelper.GetVsUIShell();
		}
	}
}
