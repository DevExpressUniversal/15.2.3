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
using DevExpress.Xpf.Utils.Themes;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class DXPivotGridThemesLoader : DXThemesLoaderBase {
		const string ShowBorderGroupName = "ShowBorder";
		const string ShowBorderStateName = "ShownBorder";
		const string HideBorderStateName = "HidenBorder";
		const string ShowWaitIndicatorGroupName = "ShowIndicator";
		const string ShowIndicatorStateName = "ShownIndicator";
		const string HideIndicatorStateName = "HidenIndicator";
		public static readonly DependencyProperty ShowBorderProperty;
		public static readonly DependencyProperty ShowIndicatorProperty;
		static DXPivotGridThemesLoader() {
			Type ownerType = typeof(DXPivotGridThemesLoader);
			ShowBorderProperty = DependencyProperty.Register("ShowBorder", typeof(bool), ownerType, new PropertyMetadata(true, (d,e) => ((DXPivotGridThemesLoader)d).OnPropertyChanged()));
			ShowIndicatorProperty = DependencyProperty.Register("ShowIndicator", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((DXPivotGridThemesLoader)d).OnPropertyChanged()));
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		public bool ShowIndicator {
			get { return (bool)GetValue(ShowIndicatorProperty); }
			set { SetValue(ShowIndicatorProperty, value); }
		}
		protected override Type TargetType {
			get { return typeof(PivotGridControl); }
		}
		public DXPivotGridThemesLoader() {
			Loaded += OnLoaded;
		}
		void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			EnsureVisualState();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			EnsureVisualState();
		}
		void OnPropertyChanged() {
			EnsureVisualState();
		}
		void EnsureVisualState() {
			VisualStateManager.GoToState(this, ShowBorder ? ShowBorderStateName : HideBorderStateName, true);
			VisualStateManager.GoToState(this, ShowIndicator ? ShowIndicatorStateName : HideIndicatorStateName, true);
		}
	}
}
