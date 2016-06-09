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
using DevExpress.DemoData.Helpers;
using System.Windows;
using DevExpress.Xpf.DemoBase.Internal;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Reflection;
using DevExpress.Internal.DXWindow;
namespace DevExpress.Xpf.DemoBase {
	class LinkClickEventArgs : EventArgs {
		public LinkClickEventArgs(string uriString, string target) {
			UriString = uriString;
			Target = target;
		}
		public string UriString { get; private set; }
		public string Target { get; private set; }
	}
	class ModuleAppearEventArgs : EventArgs {
		public ModuleAppearEventArgs(FrameworkElement demoModule, Exception demoModuleException) {
			DemoModule = demoModule;
			DemoModuleException = demoModuleException;
		}
		public FrameworkElement DemoModule { get; private set; }
		public Exception DemoModuleException { get; private set; }
	}
	sealed class DemoBaseControl : PartsControl {
		#region Dependency Properties
		public static readonly DependencyProperty DataProperty;
		public static readonly DependencyProperty ActualPageProperty;
		public static readonly DependencyProperty AllowRunAnotherDemoProperty;
		static DemoBaseControl() {
			Type ownerType = typeof(DemoBaseControl);
			DataProperty = DependencyProperty.Register("Data", typeof(DemoBaseControlData), ownerType, new PropertyMetadata(null, RaiseDataChanged));
			ActualPageProperty = DependencyProperty.Register("ActualPage", typeof(DemoBaseControlPage), ownerType, new PropertyMetadata(DemoBaseControlPage.None, RaiseActualPageChanged));
			AllowRunAnotherDemoProperty = DependencyProperty.Register("AllowRunAnotherDemo", typeof(bool), ownerType, new PropertyMetadata(false, RaiseAllowRunAnotherDemoChanged));
		}
		DemoBaseControlData dataValue = null;
		DemoBaseControlPage actualPageValue = DemoBaseControlPage.None;
		static void RaiseDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoBaseControl)d).dataValue = (DemoBaseControlData)e.NewValue;
		}
		static void RaiseActualPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoBaseControl)d).actualPageValue = (DemoBaseControlPage)e.NewValue;
			((DemoBaseControl)d).RaiseActualPageChanged(e);
		}
		static void RaiseAllowRunAnotherDemoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoBaseControl)d).RaiseAllowRunAnotherDemoChanged(e);
		}
		#endregion
		ProductDescription actualProduct;
		DemoBaseControlMainPart mainPart;
		StartupBase startup;
		public DemoBaseControl(string platform, DataLoaderActions actions, string productId, Assembly demoAssembly, string moduleName, StartupBase startup) {
			this.startup = startup;
			FocusHelper.SetFocusable(this, false);
			InitiallyLoadedProductName = productId;
			InitiallyLoadedDemoAssembly = demoAssembly;
			InitiallyLoadedModuleName = moduleName;
			Data = new DemoBaseControlData(platform, actions, productId);
		}
		public string InitiallyLoadedProductName { get; private set; }
		public string InitiallyLoadedModuleName { get; private set; }
		public Assembly InitiallyLoadedDemoAssembly { get; private set; }
		public DemoBaseControlData Data { get { return dataValue; } private set { SetValue(DataProperty, value); } }
		public DemoBaseControlPage ActualPage { get { return actualPageValue; } private set { SetValue(ActualPageProperty, value); } }
		public ProductDescription ActualProduct { get { return actualProduct; } private set { actualProduct = value; } }
		public bool AllowRunAnotherDemo { get { return (bool)GetValue(AllowRunAnotherDemoProperty); } set { SetValue(AllowRunAnotherDemoProperty, value); } }
		public Exception DemoModuleException { get; private set; }
		public FrameworkElement CurrentDemoModule { get; private set; }
		public DemoBaseControlPagesContainer PagesContainer { get { return mainPart.PagesContainer; } }
		public ToolbarView DemoModuleView {
			get { return mainPart.PagesContainer.MainPage.ModuleWrapper.DemoModuleView; }
			set { mainPart.PagesContainer.MainPage.ModuleWrapper.DemoModuleView = value; }
		}
		public ToolbarSidebarView DemoModuleOptionsView {
			get { return mainPart.PagesContainer.MainPage.ModuleWrapper.OptionsView; }
			set { mainPart.PagesContainer.MainPage.ModuleWrapper.OptionsView = value; }
		}
		public bool IsLoading { get { return mainPart.PagesContainer.MainPage.ModuleWrapper.LoadingInProgress; } }
		public FlowDirection DemoFlowDirection { get { return mainPart.PagesContainer.MainPage.ModuleWrapper.DemoFlowDirection; } set { mainPart.PagesContainer.MainPage.ModuleWrapper.DemoFlowDirection = value; } }
		public FlowDirection WrapperViewFlowDirection { get { return mainPart.PagesContainer.MainPage.ModuleWrapper.View.FlowDirection; } }
		public event DepPropertyChangedEventHandler StateChanged;
		public event EventHandler<ModuleAppearEventArgs> ModuleAppear;
		public event ThePropertyChangedEventHandler<ProductDescription> ActualProductChanged;
		public event EventHandler BackButtonClick;
		public event EventHandler BuyNowButtonClick;
		public event EventHandler ModuleReload;
		public event DepPropertyChangedEventHandler ActualPageChanged;
		public event DepPropertyChangedEventHandler AllowRunAnotherDemoChanged;
		public void ReloadModule() {
			if(ModuleReload != null)
				ModuleReload(this, EventArgs.Empty);
		}
		protected override void RegisterViews() {
			RegisterView(typeof(DemoBaseControlMainPart), typeof(DemoBaseControlMainPartView));
			RegisterView(typeof(DemoBaseControlPagesContainer), typeof(DemoBaseControlPagesContainerView));
			RegisterView(typeof(DemoBaseControlProductsPage), typeof(DemoBaseControlProductsPageView));
			RegisterView(typeof(DemoBaseControlModulesPage), typeof(DemoBaseControlModulesPageView));
			RegisterView(typeof(DemoBaseControlMainPage), typeof(DemoBaseControlMainPageView));
			RegisterView(typeof(DemoBaseControlModuleSelector), typeof(DemoBaseControlModuleSelectorView));
			RegisterView(typeof(DemoBaseControlModuleWrapper), typeof(DemoBaseControlModuleWrapperView));
		}
		protected override PartsControlPart CreateMainPart() {
			mainPart = new DemoBaseControlMainPart(this, startup);
			mainPart.PagesContainer.NavigateToProductsPage();
			mainPart.ModuleAppear += OnMainPartModuleAppear;
			mainPart.BackButtonClick += OnMainPartBackButtonClick;
			mainPart.BuyNowButtonClick += OnMainPartBuyNowButtonClick;
			mainPart.PagesContainer.ActualPageChanged += OnPagesContainerActualPageChanged;
			mainPart.PagesContainer.ActualProductChanged += OnPagesContainerActualProductChanged;
			return mainPart;
		}
		void RaiseAllowRunAnotherDemoChanged(DependencyPropertyChangedEventArgs e) {
			if(AllowRunAnotherDemoChanged != null)
				AllowRunAnotherDemoChanged(this, new DepPropertyChangedEventArgs(e));
		}
		void RaiseActualPageChanged(DependencyPropertyChangedEventArgs e) {
			if(ActualPageChanged != null)
				ActualPageChanged(this, new DepPropertyChangedEventArgs(e));
		}
		void OnPagesContainerActualPageChanged(object sender, ThePropertyChangedEventArgs<DemoBaseControlPage> e) {
			ActualPage = e.NewValue;
		}
		void OnPagesContainerActualProductChanged(object sender, ThePropertyChangedEventArgs<ProductDescription> e) {
			ActualProduct = e.NewValue;
			if(ActualProductChanged != null)
				ActualProductChanged(sender, e);
		}
		void OnMainPartBackButtonClick(object sender, EventArgs e) {
			if(BackButtonClick != null)
				BackButtonClick(this, EventArgs.Empty);
		}
		void OnMainPartBuyNowButtonClick(object sender, EventArgs e) {
			if(BuyNowButtonClick != null)
				BuyNowButtonClick(this, EventArgs.Empty);
		}
		void RaiseStateChanged(DependencyPropertyChangedEventArgs e) {
			if(StateChanged != null)
				StateChanged(this, new DepPropertyChangedEventArgs(e));
		}
		void OnMainPartModuleAppear(object sender, ModuleAppearEventArgs e) {
			CurrentDemoModule = e.DemoModule;
			DemoModuleException = e.DemoModuleException;
			if(ModuleAppear != null)
				ModuleAppear(this, e);
		}
	}
}
