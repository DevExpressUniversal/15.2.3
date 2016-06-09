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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Reports.UserDesigner.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public sealed class FontDialogService : WindowAwareServiceBase, ICustomDialogService {
		protected override void OnActualWindowChanged(Window oldWindow) { }
		bool ICustomDialogService.ShowDialog(string title, object viewModel) {
			var editorValue = (UITypeEditorValue)viewModel;
			var dialog = new System.Windows.Forms.FontDialog();
			dialog.Font = (System.Drawing.Font)editorValue.OriginalValue;
			bool result = dialog.ShowDialog(GetWin32Window(ActualWindow)) == System.Windows.Forms.DialogResult.OK;
			if(result)
				editorValue.Value = dialog.Font;
			return result;
		}
		interface IWin32Window : System.Windows.Forms.IWin32Window, System.Windows.Interop.IWin32Window { }
		IWin32Window GetWin32Window(Window window) {
			return window.With(x => new Win32Window(x));
		}
		sealed class Win32Window : IWin32Window {
			readonly Window owner;
			public Win32Window(Window window) {
				this.owner = window;
			}
			public IntPtr Handle { get { return owner.With(x => new WindowInteropHelper(x)).Return(x => x.Handle, () => IntPtr.Zero); } }
		}
	}
}
