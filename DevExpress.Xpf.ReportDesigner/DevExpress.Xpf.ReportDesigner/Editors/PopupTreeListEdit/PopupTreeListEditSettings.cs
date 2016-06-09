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
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public interface IPopupTreeListEditItemsProviderOwner {
		object ItemsSource { get; }
		string DisplayMember { get; }
		string ValueMember { get; }
		string ChildNodesPath { get; }
		IHierarchicalPathProvider HierarchicalPathProvider { get; }
	}
	public class PopupTreeListEditSettings : PopupBaseEditSettings, IPopupTreeListEditItemsProviderOwner {
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ValueMemberProperty;
		public static readonly DependencyProperty DisplayMemberProperty;
		public static readonly DependencyProperty ChildNodesPathProperty;
		public static readonly DependencyProperty HierarchicalPathProviderProperty;
		public static readonly DependencyProperty PopupSelectionValidatorProperty;
		public static readonly DependencyProperty PopupDisplayMemberProperty;
		public static readonly DependencyProperty TreeListCellTemplateProperty;
		PopupTreeListEditItemsProvider itemsProvider;
		protected internal PopupTreeListEditItemsProvider ItemsProvider {
			get {
				return itemsProvider ?? (itemsProvider = new PopupTreeListEditItemsProvider(this));
			}
		}
		public object ItemsSource { 
			get {
				return GetValue(ItemsSourceProperty);
			}
			set {
				SetValue(ItemsSourceProperty, value);
			}
		}
		public string ValueMember {
			get {
				return (string)GetValue(ValueMemberProperty);
			}
			set {
				SetValue(ValueMemberProperty, value);
			}
		}
		public string DisplayMember {
			get {
				return (string)GetValue(DisplayMemberProperty);
			}
			set {
				SetValue(DisplayMemberProperty, value);
			}
		}
		public string ChildNodesPath {
			get {
				return (string)GetValue(ChildNodesPathProperty);
			}
			set {
				SetValue(ChildNodesPathProperty, value);
			}
		}
		public IHierarchicalPathProvider HierarchicalPathProvider {
			get {
				return (IHierarchicalPathProvider)GetValue(HierarchicalPathProviderProperty);
			}
			set {
				SetValue(HierarchicalPathProviderProperty, value);
			}
		}
		public IPopupSelectionValidator PopupSelectionValidator {
			get {
				return (IPopupSelectionValidator)GetValue(PopupSelectionValidatorProperty);
			}
			set {
				SetValue(PopupSelectionValidatorProperty, value);
			}
		}
		public string PopupDisplayMember {
			get {
				return (string)GetValue(PopupDisplayMemberProperty);
			}
			set {
				SetValue(PopupDisplayMemberProperty, value);
			}
		}
		public DataTemplate TreeListCellTemplate {
			get {
				return (DataTemplate)GetValue(TreeListCellTemplateProperty);
			}
			set {
				SetValue(TreeListCellTemplateProperty, value);
			}
		}
		internal Locker AssignToEditLocker {
			get { return AssignToEditCoreLocker; }
		}
		static PopupTreeListEditSettings() {
			DependencyPropertyRegistrator<PopupTreeListEditSettings>.New()
				.Register(d => d.ItemsSource, out ItemsSourceProperty, null, (d, e) => d.OnItemsSourceChanged(e.NewValue))
				.Register(d => d.ValueMember, out ValueMemberProperty, null, (d, e) => d.OnValueMemberChanged((string)e.NewValue))
				.Register(d => d.DisplayMember, out DisplayMemberProperty, null, (d, e) => d.OnDisplayMemberChanged((string)e.NewValue))
				.Register(d => d.ChildNodesPath, out ChildNodesPathProperty, null, (d, e) => d.ChildNodesPathChanged((string)e.NewValue))
				.Register(d => d.HierarchicalPathProvider, out HierarchicalPathProviderProperty, null, (d, e) => d.HierarchicalPathProviderChanged((IHierarchicalPathProvider)e.NewValue))
				.Register(d => d.PopupSelectionValidator, out PopupSelectionValidatorProperty, null, (d, e) => d.PopupSelectionValidatorChanged((IPopupSelectionValidator)e.NewValue))
				.Register(d => d.PopupDisplayMember, out PopupDisplayMemberProperty, null, (d, e) => d.PopupDisplayMemberChanged((string)e.NewValue))
				.Register(d => d.TreeListCellTemplate, out TreeListCellTemplateProperty, null, (d, e) => d.TreeListCellTemplateChanged((DataTemplate)e.NewValue))
			;
			Type ownerType = typeof(PopupTreeListEditSettings);
			PopupMinHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(PopupTreeListEdit.DefaultPopupMinHeight));
			PopupMinWidthProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(PopupTreeListEdit.DefaultPopupMinWidth));
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(PopupTreeListEdit), typeof(PopupTreeListEditSettings), () => new PopupTreeListEdit(), () => new PopupTreeListEditSettings());
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			PopupTreeListEdit cb = edit as PopupTreeListEdit;
			if(cb == null)
				return;
			SetValueFromSettings(ItemsSourceProperty, () => cb.ItemsSource = ItemsSource);
			SetValueFromSettings(ValueMemberProperty, () => cb.ValueMember = ValueMember);
			SetValueFromSettings(DisplayMemberProperty, () => cb.DisplayMember = DisplayMember);
			SetValueFromSettings(ChildNodesPathProperty, () => cb.ChildNodesPath = ChildNodesPath);
			SetValueFromSettings(HierarchicalPathProviderProperty, () => cb.HierarchicalPathProvider = HierarchicalPathProvider);
			SetValueFromSettings(PopupSelectionValidatorProperty, () => cb.PopupSelectionValidator = PopupSelectionValidator);
			SetValueFromSettings(PopupDisplayMemberProperty, () => cb.PopupDisplayMember = PopupDisplayMember);
			SetValueFromSettings(TreeListCellTemplateProperty, () => cb.TreeListCellTemplate = TreeListCellTemplate);
		}
		protected virtual void OnItemsSourceChanged(object newValue) {
		}
		protected virtual void OnValueMemberChanged(string newValue) {
		}
		protected virtual void OnDisplayMemberChanged(string newValue) {
		}
		protected virtual void ChildNodesPathChanged(string newValue) {
		}
		protected virtual void HierarchicalPathProviderChanged(IHierarchicalPathProvider newValue) {
		}
		protected virtual void PopupSelectionValidatorChanged(IPopupSelectionValidator newValue) {
		}
		protected virtual void PopupDisplayMemberChanged(string newValue) {
		}
		protected virtual void TreeListCellTemplateChanged(DataTemplate newValue) {
		}
	}
}
