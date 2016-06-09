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
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.Design {
	public class DockTypeEditor : Control {
		#region static
		public static readonly RoutedEvent EditValueChangedEvent;
		public static readonly RoutedEvent ActivatingEvent;
		static DockTypeEditor() {
			var dProp = new DependencyPropertyRegistrator<DockTypeEditor>();
			dProp.RegisterDirectEvent<DockTypeEditorValueChangedEventHandler>("EditValueChanged", ref EditValueChangedEvent);
			dProp.RegisterDirectEvent<DockTypeEditorActivatingEventHandler>("Activating", ref ActivatingEvent);
		}
		#endregion static
		DockButton PART_BottomButton, PART_TopButton, PART_LeftButton, PART_RightButton, PART_CenterButton, PART_CenterLeftButton, PART_CenterTopButton, PART_CenterRightButton, PART_CenterBottomButton;
		public DockTypeEditor() {
			IsVisibleChanged += DockTypeEditor_IsVisibleChanged;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			EnsureButtons();
			RaiseInitializing();
		}
		bool isReady;
		void EnsureButtons() {
			EnsureTemplatedButton("PART_BottomButton", ref PART_BottomButton);
			EnsureTemplatedButton("PART_TopButton", ref PART_TopButton);
			EnsureTemplatedButton("PART_LeftButton", ref PART_LeftButton);
			EnsureTemplatedButton("PART_RightButton", ref PART_RightButton);
			EnsureTemplatedButton("PART_CenterButton", ref PART_CenterButton);
			EnsureTemplatedButton("PART_CenterLeftButton", ref  PART_CenterLeftButton);
			EnsureTemplatedButton("PART_CenterTopButton", ref PART_CenterTopButton);
			EnsureTemplatedButton("PART_CenterRightButton", ref PART_CenterRightButton);
			EnsureTemplatedButton("PART_CenterBottomButton", ref PART_CenterBottomButton);
			isReady = true;
		}
		void EnsureTemplatedButton(string name, ref DockButton button) {
			button = GetTemplateChild(name) as DockButton;
			button.Click += new RoutedEventHandler(DockButtonClicked);
		}
		void DockTypeEditor_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			RaiseInitializing();
		}
		void RaiseInitializing() {
			if(!IsVisible || !isReady) return;
			DockTypeEditorActivatingEventArgs newInitializingEventArgs = new DockTypeEditorActivatingEventArgs();
			RaiseEvent(newInitializingEventArgs);
			PART_BottomButton.IsEnabled = PART_TopButton.IsEnabled = PART_LeftButton.IsEnabled = PART_RightButton.IsEnabled =
				newInitializingEventArgs.AreSideButtonsEnabled;
			PART_CenterButton.IsEnabled = PART_CenterLeftButton.IsEnabled = PART_CenterTopButton.IsEnabled = PART_CenterRightButton.IsEnabled = PART_CenterBottomButton.IsEnabled =
				newInitializingEventArgs.AreCenterButtonsEnabled;
			PART_CenterButton.IsEnabled = newInitializingEventArgs.IsFillButtonEnabled;
		}
		public event DockTypeEditorValueChangedEventHandler EditValueChanged {
			add { AddHandler(EditValueChangedEvent, value); }
			remove { RemoveHandler(EditValueChangedEvent, value); }
		}
		public event DockTypeEditorActivatingEventHandler Activating {
			add { AddHandler(ActivatingEvent, value); }
			remove { RemoveHandler(ActivatingEvent, value); }
		}
		void DockButtonClicked(object sender, RoutedEventArgs e) {
			DockButton dockButton = sender as DockButton;
			if(dockButton == null) return;
			DockTypeValue value = new DockTypeValue() { DockType = dockButton.DockType, IsCenter = dockButton.IsCenter };
			RaiseEvent(new DockTypeEditorValueChangedEventArgs(null, value));
			e.Handled = true;
		}
	}
	public class DockButton : Button {
		public DockType DockType { get; set; }
		public bool IsCenter { get; set; }
	}
	public class DockTypeValue {
		public bool IsCenter { get; set; }
		public DockType DockType { get; set; }
	}
	public class DockTypeEditorActivatingEventArgs : RoutedEventArgs {
		public bool IsFillButtonEnabled { get; set; }
		public bool AreCenterButtonsEnabled { get; set; }
		public bool AreSideButtonsEnabled { get; set; }
		public DockTypeEditorActivatingEventArgs()
			: base(DockTypeEditor.ActivatingEvent) {
			AreCenterButtonsEnabled = true;
			AreSideButtonsEnabled = true;
			IsFillButtonEnabled = true;
		}
	}
	public class DockTypeEditorValueChangedEventArgs : RoutedEventArgs {
		public object NewValue { get; private set; }
		public object OldValue { get; private set; }
		public DockTypeEditorValueChangedEventArgs(object oldValue, object newValue)
			: base(DockTypeEditor.EditValueChangedEvent) {
				OldValue = oldValue;
				NewValue = newValue;
		}
	}
	public delegate void DockTypeEditorActivatingEventHandler(
			object sender, DockTypeEditorActivatingEventArgs e
	);
	public delegate void DockTypeEditorValueChangedEventHandler(
			object sender, DockTypeEditorValueChangedEventArgs e
	);
	public static class DockTypeEditorFactory {
		static readonly string dockTypeEditorStyle = "DockTypeEditorStyle";
		static ResourceDictionary dockTypeEditorTemplates;
		static DockTypeEditorFactory() {
			string uri = string.Format("/DevExpress.Xpf.Docking.v{0}.Design;component/DockTypeEditor/DockTypeEditor.xaml", AssemblyInfo.VersionShort);
			dockTypeEditorTemplates = new ResourceDictionary() { Source = new Uri(uri, UriKind.Relative) };
		}
		public static DockTypeEditor CreateDockTypeEditor() {
			DockTypeEditor editor = new DockTypeEditor { Style = dockTypeEditorTemplates[dockTypeEditorStyle] as Style };
			return editor;
		}
	}
}
