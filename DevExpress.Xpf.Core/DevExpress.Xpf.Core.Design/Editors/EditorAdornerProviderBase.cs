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

using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System;
namespace DevExpress.Xpf.Core.Design {
	public abstract class EditorAdornerProviderBase : PrimarySelectionAdornerProvider {
		AdornerPanel panel;
		public ModelItem AdornedItem { get; private set; }
		public ICommand Command { get; private set; }
		protected virtual bool IsVisible { get { return true; } }
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			AdornedItem = item;
			if(!IsVisible) return;
			panel = new AdornerPanel();
			EditorSmartTagContentControl control = new EditorSmartTagContentControl();
			Command = CreateCommand();
			control.DataContext = this;
			AdornerPanel.SetAdornerHorizontalAlignment(control, AdornerHorizontalAlignment.Right);
			AdornerPanel.SetAdornerVerticalAlignment(control, AdornerVerticalAlignment.OutsideTop);
			AdornerPanel.SetAdornerMargin(control, new Thickness(0, 1, 14, 0));
			panel.Children.Add(control);
#if SILVERLIGHT
#endif
			Adorners.Add(panel);
		}
		protected abstract ICommand CreateCommand();
		protected override void Deactivate() {
			AdornedItem = null;
			if(panel != null) {
				((EditorSmartTagContentControl)panel.Children[0]).DataContext = null;
				Adorners.Remove(panel);
				panel = null;
			}
			base.Deactivate();
		}
	}
	public static class DesignDialogHelper {
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetActiveWindow();
		public static bool? ShowDialog(Window window) {
			System.Windows.Interop.WindowInteropHelper windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(window);
			windowInteropHelper.Owner = GetActiveWindow();
			return window.ShowDialog();
		}
	}
}
