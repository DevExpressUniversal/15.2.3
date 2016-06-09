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
using System.Windows.Data;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class PopupTreeListEdit : PopupBaseEdit {
		public const double DefaultPopupMinHeight = 300d, DefaultPopupMinWidth = 600d;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ValueMemberProperty;
		public static readonly DependencyProperty DisplayMemberProperty;
		public static readonly DependencyProperty ChildNodesPathProperty;
		public static readonly DependencyProperty HierarchicalPathProviderProperty;
		public static readonly DependencyProperty PopupSelectionValidatorProperty;
		public static readonly DependencyProperty PopupDisplayMemberProperty;
		public static readonly DependencyProperty TreeListCellTemplateProperty;
		#region Properties
		new PopupTreeListEditStrategy EditStrategy {
			get {
				return (PopupTreeListEditStrategy)base.EditStrategy;
			}
		}
		protected override Type StyleSettingsType {
			get {
				return typeof(PopupTreeListEditStyleSettings);
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
		#endregion
		#region ctor
		static PopupTreeListEdit() {
			Type ownerType = typeof(PopupTreeListEdit);
			EditorSettingsProvider.Default.RegisterUserEditor(ownerType, typeof(PopupTreeListEditSettings), () => new PopupTreeListEdit(), () => new PopupTreeListEditSettings());
			DependencyPropertyRegistrator<PopupTreeListEdit>.New()
				.Register(d => d.ItemsSource, out ItemsSourceProperty, null, (d, e) => d.OnItemsSourceChanged(e.NewValue))
				.Register(d => d.ValueMember, out ValueMemberProperty, null, (d, e) => d.OnValueMemberChanged((string)e.NewValue))
				.Register(d => d.DisplayMember, out DisplayMemberProperty, null, (d, e) => d.OnDisplayMemberChanged((string)e.NewValue))
				.Register(d => d.ChildNodesPath, out ChildNodesPathProperty, null, (d, e) => d.OnChildNodesPathChanged((string)e.NewValue))
				.Register(d => d.HierarchicalPathProvider, out HierarchicalPathProviderProperty, null, (d, e) => d.OnHierarchicalPathProviderChanged((IHierarchicalPathProvider)e.NewValue))
				.Register(d => d.PopupSelectionValidator, out PopupSelectionValidatorProperty, null, (d, e) => d.OnPopupSelectionValidatorChanged((IPopupSelectionValidator)e.NewValue))
				.Register(d => d.PopupDisplayMember, out PopupDisplayMemberProperty, null, (d, e) => d.OnPopupDisplayMemberChanged((string)e.NewValue))
				.Register(d => d.TreeListCellTemplate, out TreeListCellTemplateProperty, null, (d, e) => d.OnTreeListCellTemplateChanged((DataTemplate)e.NewValue))
			;
			PopupMinHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(DefaultPopupMinHeight));
			PopupMinWidthProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(DefaultPopupMinWidth));
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
		public PopupTreeListEdit() {
		}
		#endregion
		protected override EditStrategyBase CreateEditStrategy() {
			return new PopupTreeListEditStrategy(this);
		}
		protected override VisualClientOwner CreateVisualClient() {
			return new PopupTreeListEditVisualClient(this);
		}
		protected override void BeforePopupOpened() {
			base.BeforePopupOpened();
			EditStrategy.SetInitialPopupSize();
		}
		protected virtual void OnItemsSourceChanged(object newValue) {
			EditStrategy.ItemsSourceChanged(newValue);
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void OnValueMemberChanged(string newValue) {
			EditStrategy.ValueMemberChanged(newValue);
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void OnDisplayMemberChanged(string newValue) {
			EditStrategy.DisplayMemberChanged(newValue);
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void OnChildNodesPathChanged(string newValue) {
			EditStrategy.ChildNodesPathChanged(newValue);
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void OnHierarchicalPathProviderChanged(IHierarchicalPathProvider newValue) {
			EditStrategy.HierarchicalPathProviderChanged(newValue);
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void OnPopupSelectionValidatorChanged(IPopupSelectionValidator newValue) {
			EditStrategy.PopupSelectionValidatorChanged(newValue);
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void OnPopupDisplayMemberChanged(string newValue) {
			EditStrategy.PopupDisplayMemberChanged(newValue);
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void OnTreeListCellTemplateChanged(DataTemplate newValue) {
			EditStrategy.TreeListCellTemplateChanged(newValue);
		}
		protected override void AcceptPopupValue() {
			base.AcceptPopupValue();
			EditStrategy.AcceptPopupValue();
		}
		protected override void DestroyPopupContent(EditorPopupBase popup) {
			base.DestroyPopupContent(popup);
			EditStrategy.PopupDestroyed();
		}
		internal void SetInitialPopupSizeInternal() {
			if(double.IsNaN(PopupWidth)) {
				PopupWidth = PopupMinWidth;
			}
			if(double.IsNaN(PopupHeight)) {
				PopupHeight = PopupMinHeight;
			}
		}
	}
}
