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
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel;
using DevExpress.Xpf.Core.MvvmSample.Helpers;
namespace DevExpress.Xpf.Core.MvvmSample {
	public class ModuleDescription {
		public Type ModuleType { get; set; }
		public Type ViewType { get; set; }
		public Type NavigationParentModuleType { get; set; }
	}
	public class ModuleDescriptionCollection : List<ModuleDescription>, IViewsListProvider, IModulesListProvider {
		Type mainModuleType = null;
		public Type GetMainModuleType() {
			if(mainModuleType == null)
				mainModuleType = GetMainModuleTypeCore();
			return mainModuleType;
		}
		Type GetMainModuleTypeCore() {
			return (from t in this where t.ModuleType != null && t.ModuleType.IsSubclassOf(typeof(MainModule)) select t.ModuleType).SingleOrDefault();
		}
		Type IModulesListProvider.GetNavigationParentModuleType(Type moduleType) {
			if(moduleType == null || moduleType == GetMainModuleType()) return null;
			Type navigationParentModuleType = (from t in this where t.ModuleType == moduleType select t.NavigationParentModuleType).SingleOrDefault();
			return navigationParentModuleType ?? GetMainModuleType();
		}
		Type IViewsListProvider.GetViewType(Type moduleType) {
			if(moduleType == null) return null;
			return (from t in this where t.ModuleType == moduleType select t.ViewType).SingleOrDefault();
		}
	}
	[ContentProperty("Modules")]
	public class MvvmRoot : Control, ISupportInitialize {
		#region Dependency Properties
		public static readonly DependencyProperty StoryboardProperty;
		public static readonly DependencyProperty StoryboardsProperty;
		public static readonly DependencyProperty MainModuleProperty;
		public static readonly DependencyProperty SplashScreenProviderProperty;
		static MvvmRoot() {
			Type ownerType = typeof(MvvmRoot);
			StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(string), ownerType, new PropertyMetadata(null));
			StoryboardsProperty = DependencyProperty.Register("Storyboards", typeof(StoryboardCollection), ownerType, new PropertyMetadata(null));
			MainModuleProperty = DependencyProperty.Register("MainModule", typeof(MainModule), ownerType, new PropertyMetadata(null));
			SplashScreenProviderProperty = DependencyProperty.Register("SplashScreenProvider", typeof(ISplashScreenProvider), ownerType, new PropertyMetadata(null, RaiseSplashScreenProviderChanged));
		}
		static void RaiseSplashScreenProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((MvvmRoot)d).RaiseSplashScreenProviderChanged(e);
		}
		#endregion
		public MvvmRoot() {
			DefaultStyleKey = typeof(MvvmRoot);
			Modules = new ModuleDescriptionCollection();
			ModulesManager = new ModulesManager(new ViewsManager(Modules), Modules);
			Storyboards = new StoryboardCollection();
		}
		public IModulesManager ModulesManager { get; private set; }
		public ModuleDescriptionCollection Modules { get; private set; }
		public ISplashScreenProvider SplashScreenProvider { get { return (ISplashScreenProvider)GetValue(SplashScreenProviderProperty); } set { SetValue(SplashScreenProviderProperty, value); } }
		public string Storyboard { get { return (string)GetValue(StoryboardProperty); } set { SetValue(StoryboardProperty, value); } }
		public StoryboardCollection Storyboards { get { return (StoryboardCollection)GetValue(StoryboardsProperty); } set { SetValue(StoryboardsProperty, value); } }
		public MainModule MainModule { get { return (MainModule)GetValue(MainModuleProperty); } private set { SetValue(MainModuleProperty, value); } }
#if SL
		public virtual void BeginInit() {
#else
		public override void BeginInit() {
			base.BeginInit();
#endif
		}
#if SL
		public virtual void EndInit() {
#else
		public override void EndInit() {
			base.EndInit();
#endif
			Type mainModuleType = Modules.GetMainModuleType();
			MainModule = mainModuleType == null ? null : (MainModule)ModulesManager.CreateModule(mainModuleType, null, null);
		}
		void RaiseSplashScreenProviderChanged(DependencyPropertyChangedEventArgs e) {
			ModulesManager.SplashScreenWrapper.SplashScreenProvider = (ISplashScreenProvider)e.NewValue;
		}
	}
}
