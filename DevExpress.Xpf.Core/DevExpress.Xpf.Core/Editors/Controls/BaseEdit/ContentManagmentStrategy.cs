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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.IO;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using DevExpress.Utils;
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
using DevExpress.Xpf.Printing.Native;
#if !SL
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using IInputElement = System.Windows.UIElement;
using Keyboard = DevExpress.Xpf.Editors.WPFCompatibility.SLKeyboard;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public abstract class ContentManagementStrategyBase {
		WeakReference edit;
		protected BaseEdit Edit { get { return (BaseEdit)edit.Target; } }
		protected ContentManagementStrategyBase(BaseEdit edit) {
			this.edit = new WeakReference(edit);
		}
		public abstract void OnEditorApplyTemplate();
		public virtual void UpdateErrorPresenter() { }
		public abstract Size MeasureOverride(Size constraint);
		public abstract Size ArrangeOverride(Size arrangeSize);
#if !SL
		public abstract Visual GetVisualChild(int index);
		public abstract int VisualChildrenCount { get; }
#endif
		public abstract void UpdateButtonPanels();
	}
#if !SL
	public class InplaceContentManagementStrategy : ContentManagementStrategyBase {
		public InplaceContentManagementStrategy(BaseEdit edit)
			: base(edit) {
		}
		public override void OnEditorApplyTemplate() {
			Edit.EditCore = Edit.GetTemplateChildInternal<FrameworkElement>("PART_Editor");
			Updater = new DataContextUpdater(Edit, Edit.EditCore);
			Updater.AttachDataContext();
			UpdateButtonPanels();
		}
		DataContextUpdater Updater { get; set; }
		public override void UpdateErrorPresenter() {
			Edit.UpdateInplaceErrorPresenter();
		}
		public override Visual GetVisualChild(int index) {
			return Edit.GetVisualChildInplaceMode(index);
		}
		public override int VisualChildrenCount { get { return Edit.VisualChildrenCountInplaceMode; } }
		public override Size MeasureOverride(Size constraint) {
			if(Edit.BorderRenderer.CanRenderBorder)
				return Edit.BorderRenderer.MeasureOverride(constraint);
			return Edit.MeasureOverrideInplaceMode(constraint);
		}
		public override Size ArrangeOverride(Size arrangeSize) {
			if(Edit.BorderRenderer.CanRenderBorder)
				return Edit.BorderRenderer.ArrangeOverride(arrangeSize);
			return Edit.ArrangeOverrideInplaceMode(arrangeSize);
		}
		public override void UpdateButtonPanels() {
			Edit.UpdateButtonPanelsInplaceMode();
		}
		class DataContextUpdater : DependencyObject {
			public static readonly DependencyProperty DataContextProperty;
			static DataContextUpdater() {
				Type ownerType = typeof(DataContextUpdater);
				DataContextProperty = DependencyProperty.Register("DataContext", typeof(object), ownerType,
					new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, DataContextPropertyChanged));
			}
			static void DataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
				((DataContextUpdater)d).UpdateDataContext(e.NewValue as DependencyObject);
			}
			bool isDataContextChanged;
			public FrameworkElement Target { get; private set; }
			public BaseEdit Editor { get; private set; }
			public DataContextUpdater(BaseEdit editor, FrameworkElement target) {
				Target = target;
				Editor = editor;
			}
			public void AttachDataContext() {
				ApplyBinding();
			}
			void UpdateDataContext(DependencyObject d) {
				isDataContextChanged = true;
				if(Target != null)
					if(d == null)
						Target.DataContext = Editor;
					else
						Target.ClearValue(FrameworkElement.DataContextProperty);
				if(d != null)
					Editor.EditStrategy.UpdateDataContext(d);
			}
			void ApplyBinding() {
				Binding binding = new Binding() {
					Source = Editor,
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
					Path = new PropertyPath(FrameworkElement.DataContextProperty)
				};
				isDataContextChanged = false;
				BindingOperations.SetBinding(this, DataContextProperty, binding);
				if(!isDataContextChanged)
					UpdateDataContext(null);
			}
		}
	}
#endif
	public class StandaloneContentManagementStrategy : ContentManagementStrategyBase {
		EditorControl ContentCore { get; set; }
		public StandaloneContentManagementStrategy(BaseEdit edit)
			: base(edit) {
		}
		public override void OnEditorApplyTemplate() {
			ContentCore = Edit.GetTemplateChildInternal<EditorControl>("PART_Content");
			if(ContentCore != null) {
				ContentCore.Owner = Edit;
				ContentCore.DataContext = Edit;
			}
			SubscribeEditEvents(null);
			Edit.ErrorPresenterStandalone = Edit.GetTemplateChildInternal<ContentControl>("PART_ErrorPresenter");
		}
		internal void OnApplyContentTemplate(EditorControl content) {
			SubscribeEditEvents(content);
		}
		void SubscribeEditEvents(EditorControl content) {
			if(content == null)
				content = ContentCore;
			if(content == null)
				return;
			Edit.EditCore = content.GetEditCore();
#if SL
			BaseEdit.SetOwnerEdit(content, Edit);
#endif
		}
		public override void UpdateErrorPresenter() {
			if(Edit.ErrorPresenterStandalone != null) {
				Edit.ErrorPresenterStandalone.Content = Edit.ValidationError;
				Edit.ErrorPresenterStandalone.Visibility = Edit.HasValidationError && Edit.ShowError ? Visibility.Visible : Visibility.Collapsed;
#if SL
				Edit.UpdateErrorPresenterVisualState();
#endif
			}
		}
		public override Size MeasureOverride(Size constraint) {
			return Edit.MeasureOverrideStandaloneMode(constraint);
		}
		public override Size ArrangeOverride(Size arrangeSize) {
			return Edit.ArrangeOverrideStandaloneMode(arrangeSize);
		}
#if !SL
		public override Visual GetVisualChild(int index) {
			return Edit.GetVisualChildStandaloneMode(index);
		}
		public override int VisualChildrenCount { get { return Edit.VisualChildrenCountStandaloneMode; } }
#endif
		public override void UpdateButtonPanels() {
		}
	}
}
