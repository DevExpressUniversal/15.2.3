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
using System.Windows.Controls;
using DevExpress.Xpf.Core.MvvmSample.Helpers;
namespace DevExpress.Xpf.Core.MvvmSample {
	public class ModuleView : UserControl, IView, IModuleView {
		#region Dependency Properties
		public static readonly DependencyProperty ViewIsReadyToAppearProperty;
		public static readonly DependencyProperty ViewIsVisibleProperty;
		static ModuleView() {
			Type ownerType = typeof(ModuleView);
			ViewIsReadyToAppearProperty = DependencyProperty.Register("ViewIsReadyToAppear", typeof(bool), ownerType, new PropertyMetadata(false, RaiseViewIsReadyToAppearChanged));
			ViewIsVisibleProperty = DependencyProperty.Register("ViewIsVisible", typeof(bool), ownerType, new PropertyMetadata(false, RaiseViewIsVisibleChanged));
		}
		static void RaiseViewIsReadyToAppearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ModuleView)d).RaiseViewIsReadyToAppearChanged(e);
		}
		static void RaiseViewIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ModuleView)d).RaiseViewIsVisibleChanged(e);
		}
		#endregion
		public bool ViewIsReadyToAppear { get { return (bool)GetValue(ViewIsReadyToAppearProperty); } private set { SetValue(ViewIsReadyToAppearProperty, value); } }
		public bool ViewIsVisible { get { return (bool)GetValue(ViewIsVisibleProperty); } private set { SetValue(ViewIsVisibleProperty, value); } }
		public event EventHandler ViewIsReadyToAppearChanged;
		public event EventHandler ViewIsVisibleChanged;
		public event EventHandler BeforeViewDisappear;
		public event EventHandler AfterViewDisappear;
		void IView.SetViewIsVisible(bool v) {
			ViewIsVisible = v;
		}
		void IModuleView.SetViewIsReadyToAppear(bool v) {
			ViewIsReadyToAppear = v;
		}
		void IView.RaiseBeforeViewDisappear() {
			if(BeforeViewDisappear != null)
				BeforeViewDisappear(this, EventArgs.Empty);
		}
		void IView.RaiseAfterViewDisappear() {
			if(AfterViewDisappear != null)
				AfterViewDisappear(this, EventArgs.Empty);
		}
		void RaiseViewIsReadyToAppearChanged(DependencyPropertyChangedEventArgs e) {
			if(ViewIsReadyToAppearChanged != null)
				ViewIsReadyToAppearChanged(this, EventArgs.Empty);
		}
		void RaiseViewIsVisibleChanged(DependencyPropertyChangedEventArgs e) {
			if(ViewIsVisibleChanged != null)
				ViewIsVisibleChanged(this, EventArgs.Empty);
		}
	}
}
