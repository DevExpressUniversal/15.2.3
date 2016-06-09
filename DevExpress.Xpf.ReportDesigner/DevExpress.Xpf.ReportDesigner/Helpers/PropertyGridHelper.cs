﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Windows.Threading;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.PropertyGrid;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public abstract class PropertyGridHelper {
		public static readonly DependencyProperty SelectedObjectAsyncProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ImmediateActionsManagerProperty;
		static PropertyGridHelper() {
			DependencyPropertyRegistrator<PropertyGridHelper>.New()
				.RegisterAttached((PropertyGridControl d) => GetSelectedObjectAsync(d), out SelectedObjectAsyncProperty, null, (d) => OnPropertyChanged(d))
				.RegisterAttached((PropertyGridControl d) => GetImmediateActionsManager(d), out ImmediateActionsManagerProperty, null);
		}
		static void OnPropertyChanged(PropertyGridControl propertyGrid) {
			var immediateActionsManager = GetImmediateActionsManager(propertyGrid);
			if(immediateActionsManager == null)
				SetImmediateActionsManager(propertyGrid, immediateActionsManager = new ImmediateActionsManagerWithTimer());
			immediateActionsManager.ExecuteAction(() => propertyGrid.SelectedObject = GetSelectedObjectAsync(propertyGrid));
		}
		public static object GetSelectedObjectAsync(PropertyGridControl d) { return d.GetValue(SelectedObjectAsyncProperty); }
		public static void SetSelectedObjectAsync(PropertyGridControl d, object v) { d.SetValue(SelectedObjectAsyncProperty, v); }
		static ImmediateActionsManagerWithTimer GetImmediateActionsManager(PropertyGridControl d) { return (ImmediateActionsManagerWithTimer)d.GetValue(ImmediateActionsManagerProperty); }
		static void SetImmediateActionsManager(PropertyGridControl d, ImmediateActionsManagerWithTimer v) { d.SetValue(ImmediateActionsManagerProperty, v); }
	}
}
