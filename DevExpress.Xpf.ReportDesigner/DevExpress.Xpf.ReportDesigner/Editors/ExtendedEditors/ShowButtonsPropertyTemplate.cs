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
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	partial class ButtonEditEx {
		public static readonly DependencyProperty ShowEditorButtonsAlwaysProperty =
			DependencyProperty.Register("ShowEditorButtonsAlways", typeof(bool), typeof(ButtonEditEx), new PropertyMetadata(false));
		public bool ShowEditorButtonsAlways {
			get { return (bool)GetValue(ShowEditorButtonsAlwaysProperty); }
			set { SetValue(ShowEditorButtonsAlwaysProperty, value); }
		}
		protected override void SetShowEditorButtons(bool show) {
			base.SetShowEditorButtons(show || ShowEditorButtonsAlways);
		}
	}
	partial class ButtonEditSettingsEx {
		public static readonly DependencyProperty ShowEditorButtonsAlwaysProperty =
			DependencyProperty.Register("ShowEditorButtonsAlways", typeof(bool), typeof(ButtonEditSettingsEx), new PropertyMetadata(false));
		public bool ShowEditorButtonsAlways {
			get { return (bool)GetValue(ShowEditorButtonsAlwaysProperty); }
			set { SetValue(ShowEditorButtonsAlwaysProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			var target = edit as ButtonEditEx;
			if(target == null) return;
			SetValueFromSettings(ShowEditorButtonsAlwaysProperty, () => target.ShowEditorButtonsAlways = ShowEditorButtonsAlways);
		}
	}
	partial class ComboBoxEditEx {
		public static readonly DependencyProperty ShowPopupOnEmptyItemsSourceProperty = 
			DependencyProperty.Register("ShowPopupOnEmptyItemsSource", typeof(bool), typeof(ComboBoxEditEx), new PropertyMetadata(false));
		public static readonly DependencyProperty ShowEditorButtonsAlwaysProperty =
			DependencyProperty.Register("ShowEditorButtonsAlways", typeof(bool), typeof(ComboBoxEditEx), new PropertyMetadata(false));
		public bool ShowPopupOnEmptyItemsSource {
			get { return (bool)GetValue(ShowPopupOnEmptyItemsSourceProperty); }
			set { SetValue(ShowPopupOnEmptyItemsSourceProperty, value); }
		}
		public bool ShowEditorButtonsAlways {
			get { return (bool)GetValue(ShowEditorButtonsAlwaysProperty); }
			set { SetValue(ShowEditorButtonsAlwaysProperty, value); }
		}
		protected override void SetShowEditorButtons(bool show) {
			base.SetShowEditorButtons(show || ShowEditorButtonsAlways);
		}
		protected override bool CanShowPopupCore() {
			return ShowPopupOnEmptyItemsSource ? ItemsSource!=null && ItemsProvider!=null : base.CanShowPopupCore();
		}
	}
	partial class ComboBoxEditSettingsEx {
		public static readonly DependencyProperty ShowEditorButtonsAlwaysProperty =
			DependencyProperty.Register("ShowEditorButtonsAlways", typeof(bool), typeof(ComboBoxEditSettingsEx), new PropertyMetadata(false));
		public static readonly DependencyProperty ShowPopupOnEmptyItemsSourceProperty =
			DependencyProperty.Register("ShowPopupOnEmptyItemsSource", typeof(bool), typeof(ComboBoxEditSettingsEx), new PropertyMetadata(false));
		public bool ShowEditorButtonsAlways {
			get { return (bool)GetValue(ShowEditorButtonsAlwaysProperty); }
			set { SetValue(ShowEditorButtonsAlwaysProperty, value); }
		}
		public bool ShowPopupOnEmptyItemsSource {
			get { return (bool)GetValue(ShowPopupOnEmptyItemsSourceProperty); }
			set { SetValue(ShowPopupOnEmptyItemsSourceProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			var target = edit as ComboBoxEditEx;
			if(target == null) return;
			SetValueFromSettings(ShowEditorButtonsAlwaysProperty, () => target.ShowEditorButtonsAlways = ShowEditorButtonsAlways);
			SetValueFromSettings(ShowPopupOnEmptyItemsSourceProperty, () => target.ShowPopupOnEmptyItemsSource = ShowPopupOnEmptyItemsSource);
		}
	}
}
