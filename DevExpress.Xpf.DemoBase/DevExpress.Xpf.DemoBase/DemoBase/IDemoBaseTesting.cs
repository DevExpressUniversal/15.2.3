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
using System.Reflection;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.DemoBase.Helpers;
namespace DevExpress.Xpf.DemoBase {
	public interface IDemoBaseTesting {
		Assembly DemoAssembly { get; }
		bool IsReady { get; }
		int GetCurrentModuleSourcesCount();
		object SubscribeToModuleAppear(EventHandler handler);
		void UnsubscribeFromModuleAppear(object handler);
		void LoadModule(ModuleDescription module, bool reloadIfNeeded);
		IList<ModuleDescription> Modules { get; }
		FrameworkElement ResetFocusElement { get; }
		FrameworkElement CurrentDemoModule { get; }
		Exception DemoModuleException { get; }
		ModuleDescription CurrentModule { get; }
		string DemoModuleDescription { get; }
		void ShowDemoModuleDescription();
		void HideDemoModuleDescription();
		bool IsDemoModuleDescriptionHidden { get; }
		void ShowCode();
		void ShowDemo();
		void ShowCodeFile(int index);
		string CodeFileName { get; }
		string CodeText { get; }
		bool IsThemeSelectorHidden { get; }
		void ShowThemeSelector();
		bool IsDemoOpen { get; }
		bool IsCodeOpen { get; }
	}
}
