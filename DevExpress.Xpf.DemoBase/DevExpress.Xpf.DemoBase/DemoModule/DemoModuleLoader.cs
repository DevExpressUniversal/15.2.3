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
using DevExpress.DemoData.Helpers;
using System.Windows.Data;
using DevExpress.Xpf.DemoBase.Helpers;
namespace DevExpress.Xpf.DemoBase {
	class DemoModuleLoader : DependencyObject {
		#region Dependency Properties
		public static readonly DependencyProperty CurrentDemoModuleHelperProperty;
		public static readonly DependencyProperty NextDemoModuleHelperProperty;
		public static readonly DependencyProperty IsPopupContentInvisibleProperty;
		static DemoModuleLoader() {
			Type ownerType = typeof(DemoModuleLoader);
			CurrentDemoModuleHelperProperty = DependencyProperty.Register("CurrentDemoModuleHelper", typeof(DemoModuleHelper), ownerType, new PropertyMetadata(null));
			NextDemoModuleHelperProperty = DependencyProperty.Register("NextDemoModuleHelper", typeof(DemoModuleHelper), ownerType, new PropertyMetadata(null));
			IsPopupContentInvisibleProperty = DependencyProperty.Register("IsPopupContentInvisible", typeof(bool), ownerType, new PropertyMetadata(true));
		}
		#endregion
		DemoModuleHelper demoModule1Helper;
		DemoModuleHelper demoModule2Helper;
		Type nextDemoModuleType;
		public DemoModuleLoader(ContentPresenter demoModule1Presenter, ContentPresenter demoModule2Presenter, Style demoModuleControlStyle) {
			this.demoModule1Helper = new DemoModuleHelper(demoModule1Presenter, demoModuleControlStyle);
			this.demoModule2Helper = new DemoModuleHelper(demoModule2Presenter, demoModuleControlStyle);
			BindingOperations.SetBinding(this.demoModule1Helper, DemoModuleHelper.IsPopupContentInvisibleProperty, new Binding("IsPopupContentInvisible") { Source = this, Mode = BindingMode.OneWay });
			BindingOperations.SetBinding(this.demoModule2Helper, DemoModuleHelper.IsPopupContentInvisibleProperty, new Binding("IsPopupContentInvisible") { Source = this, Mode = BindingMode.OneWay });
			CurrentDemoModuleHelper = this.demoModule1Helper;
			NextDemoModuleHelper = this.demoModule2Helper;
			LoadDemoModule(CurrentDemoModuleHelper);
			ShowDemoModule(CurrentDemoModuleHelper);
			HideDemoModule(NextDemoModuleHelper);
			UnloadDemoModule(NextDemoModuleHelper);
		}
		public DemoModuleHelper CurrentDemoModuleHelper { get { return (DemoModuleHelper)GetValue(CurrentDemoModuleHelperProperty); } private set { SetValue(CurrentDemoModuleHelperProperty, value); } }
		public DemoModuleHelper NextDemoModuleHelper { get { return (DemoModuleHelper)GetValue(NextDemoModuleHelperProperty); } private set { SetValue(NextDemoModuleHelperProperty, value); } }
		public bool IsPopupContentInvisible { get { return (bool)GetValue(IsPopupContentInvisibleProperty); } set { SetValue(IsPopupContentInvisibleProperty, value); } }
		public event EventHandler NextDemoModuleLoaded;
		public event EventHandler DemoUnloaded;
		public void BeginLoadNextDemoModule(Type demoModuleType) {
			this.nextDemoModuleType = demoModuleType;
			CurrentDemoModuleHelper.BeginDisappearDemoModule(OnBeginDisappearCompleted);
		}
		public void ReplaceCurrentDemoModuleByNext() {
			CurrentDemoModuleHelper.EndDispappearDemoModule();
			HideDemoModule(CurrentDemoModuleHelper);
			UnloadDemoModule(CurrentDemoModuleHelper);
			SwitchDemoModules();
			ShowDemoModule(CurrentDemoModuleHelper);
		}
		public void AppearCurrentDemoModule() {
			CurrentDemoModuleHelper.EndAppearDemoModule();
		}
		void OnBeginAppearCompleted(DemoModuleHelper helper) {
			if(NextDemoModuleLoaded != null)
				NextDemoModuleLoaded(this, EventArgs.Empty);
		}
		void OnBeginDisappearCompleted(DemoModuleHelper helper) {
			NextDemoModuleHelper.InitialOptionsView = CurrentDemoModuleHelper.OptionsExpanded ? (ToolbarSidebarView?)CurrentDemoModuleHelper.DemoModuleOptionsView : null;
			LoadDemoModule(NextDemoModuleHelper);
			NextDemoModuleHelper.BeginAppearDemoModule(this.nextDemoModuleType, OnBeginAppearCompleted);
		}
		void ShowDemoModule(DemoModuleHelper demoModuleHelper) {
			demoModuleHelper.DemoModulePresenter.Opacity = 1.0;
		}
		void HideDemoModule(DemoModuleHelper demoModuleHelper) {
			demoModuleHelper.DemoModulePresenter.Opacity = 0.0;
		}
		void LoadDemoModule(DemoModuleHelper demoModuleHelper) {
			demoModuleHelper.DemoModulePresenter.Visibility = Visibility.Visible;
		}
		void UnloadDemoModule(DemoModuleHelper demoModuleHelper) {
			demoModuleHelper.DemoModulePresenter.Visibility = Visibility.Collapsed;
			if(DemoUnloaded != null) {
				DemoUnloaded(this, EventArgs.Empty);
			}
		}
		void SwitchDemoModules() {
			DemoModuleHelper helper = NextDemoModuleHelper;
			NextDemoModuleHelper = CurrentDemoModuleHelper;
			CurrentDemoModuleHelper = helper;
		}
	}
}
