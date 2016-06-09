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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.DemoData.DemoParts;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.DemoBase.Internal {
	public partial class DemoBaseControlModulesPageView : DemoBaseControlPartView {
		public DemoBaseControlModulesPageView() {
			InitializeComponent();
			Loaded += DemoBaseControlModulesPageView_Loaded;
		}
		DemoWindow DemoWindow {
			get { return this.VisualParents().OfType<DemoWindow>().FirstOrDefault(); }
		}
		void DemoBaseControlModulesPageView_Loaded(object sender, RoutedEventArgs e) {
			DemoWindow.Do(w => w.CtrlFClicked += (o, args) => filter.FocusEditor());
		}
		void OnClearFilterFocus(object sender, MouseButtonEventArgs e) {
			FocusManager.SetFocusedElement(DemoWindow, DemoWindow);
		}
		void Filter_OnClearFocus(object sender, EventArgs e) {
			OnClearFilterFocus(null, null);
		}
	}
}
