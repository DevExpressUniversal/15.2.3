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

using DevExpress.Design.UI;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Windows.Design;
namespace DevExpress.Xpf.Ribbon.Design {
	public partial class RibbonControlInfoElement : UserControl, INotifyPropertyChanged {
		public ICommand CreateBarManager {
			get { return createBarManager; }
			private set {
				if(createBarManager == value) return;
				createBarManager = value;
				OnCreateBarManagerChanged();
			}
		}
		public Action<object> CreateBarManagerAction {
			get { return createBarManagerAction; }
			set {
				if(createBarManagerAction == value) return;
				createBarManagerAction = value;
				OnCreateBarManagerActionChanged();
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public RibbonControlInfoElement() {			
			InitializeComponent();
		}
		protected void RaisePropertyChanged(string propertyName) { if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
		void OnCreateBarManagerChanged() {
			RaisePropertyChanged("CreateBarManager");
		}
		void OnCreateBarManagerActionChanged() {
			RaisePropertyChanged("CreateBarManagerAction");
			InitializeCreateBarManagerCommand();
		}
		void InitializeCreateBarManagerCommand() {
			CreateBarManager = CreateBarManagerAction == null ? null : (ICommand)new WpfDelegateCommand<object>(CreateBarManagerAction, false);
		}
		ICommand createBarManager;
		Action<object> createBarManagerAction;
	}   
}
