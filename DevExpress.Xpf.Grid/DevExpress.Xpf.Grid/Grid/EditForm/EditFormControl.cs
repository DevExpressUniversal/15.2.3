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
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Grid.EditForm;
namespace DevExpress.Xpf.Grid {
	public class EditFormControl : CachedItemsControl {
		public static readonly DependencyProperty LayoutSettingsProperty;
		static EditFormControl() {
			DependencyPropertyRegistrator<EditFormControl>.New()
				.Register(d => d.LayoutSettings, out LayoutSettingsProperty, EditFormLayoutSettings.Empty, d => d.UpdateGridPanel());
			Type ownerType = typeof(EditFormControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
		public EditFormLayoutSettings LayoutSettings {
			get { return (EditFormLayoutSettings)GetValue(LayoutSettingsProperty); }
			set { SetValue(LayoutSettingsProperty, value); }
		}
		System.Windows.Controls.Grid GridPanel { get { return Panel as System.Windows.Controls.Grid; } }
#if DEBUGTEST
		internal System.Windows.Controls.Grid GridPanelForTests { get { return GridPanel; } }
#endif
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateGridPanel();
		}
		void UpdateGridPanel() {
			if(GridPanel == null)
				return;
			GridPanel.ColumnDefinitions.Clear();
			GridPanel.RowDefinitions.Clear();
			for(int i = 0; i < LayoutSettings.ColumnCount; i++)
				GridPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, i % 2 == 0 ? GridUnitType.Auto : GridUnitType.Star) });
			for(int i = 0; i < LayoutSettings.RowCount; i++)
				GridPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
		}
		protected override void ValidateElement(FrameworkElement element, object item) {
			base.ValidateElement(element, item);
			var data = item as EditFormCellDataBase;
			if(data != null) {
				System.Windows.Controls.Grid.SetColumn(element, data.Column);
				System.Windows.Controls.Grid.SetRow(element, data.Row);
				System.Windows.Controls.Grid.SetColumnSpan(element, data.ColumnSpan);
				System.Windows.Controls.Grid.SetRowSpan(element, data.RowSpan);
			}
		}
	}
	public class PopupEditFormContainer : ContentControl {
		public PopupEditFormContainer() {
#if DEBUGTEST
			if(LoadedCallbackForTests != null)
				Loaded += OnLoaded;
#endif
		}
#if DEBUGTEST
		public static Action<PopupEditFormContainer> LoadedCallbackForTests;
		void OnLoaded(object sender, RoutedEventArgs e) {
			LoadedCallbackForTests(this);
			LoadedCallbackForTests = null;
		}
#endif
	}
	public class EditFormContainer : ContentControl {
		public static readonly DependencyProperty ShowModeProperty = DependencyProperty.Register("ShowMode", typeof(EditFormShowMode), typeof(EditFormContainer), new PropertyMetadata(EditFormShowMode.None));
		public EditFormShowMode ShowMode {
			get { return (EditFormShowMode)GetValue(ShowModeProperty); }
			set { SetValue(ShowModeProperty, value); }
		}
		static EditFormContainer() {
			Type ownerType = typeof(EditFormContainer);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
	}
}
