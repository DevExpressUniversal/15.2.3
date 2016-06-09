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
namespace DevExpress.Xpf.Docking.VisualElements {
	public class MultiTemplateControl : psvControl {
		#region static
		public static readonly DependencyProperty EmptyTemplateProperty;
		public static readonly DependencyProperty PanelTemplateProperty;
		public static readonly DependencyProperty ControlItemTemplateProperty;
		public static readonly DependencyProperty DocumentTemplateProperty;
		public static readonly DependencyProperty GroupTemplateProperty;
		public static readonly DependencyProperty TabPanelTemplateProperty;
		public static readonly DependencyProperty DocumentPanelTemplateProperty;
		public static readonly DependencyProperty MDIDocumentTemplateProperty;
		public static readonly DependencyProperty FloatDocumentTemplateProperty;
		public static readonly DependencyProperty FloatingWindowTemplateProperty;
		public static readonly DependencyProperty FloatingAdornerTemplateProperty;
		public static readonly DependencyProperty SplitterControlTemplateProperty;
		public static readonly DependencyProperty EmptySpaceControlTemplateProperty;
		public static readonly DependencyProperty LabelControlTemplateProperty;
		public static readonly DependencyProperty SeparatorControlTemplateProperty;
		public static readonly DependencyProperty AutoHideGroupTemplateProperty;
		public static readonly DependencyProperty LayoutItemProperty;
		static MultiTemplateControl() {
			var dProp = new DependencyPropertyRegistrator<MultiTemplateControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("LayoutItem", ref LayoutItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((MultiTemplateControl)dObj).OnLayoutItemChanged((BaseLayoutItem)e.NewValue));
			dProp.Register("EmptyTemplate", ref EmptyTemplateProperty, (ControlTemplate)null);
			dProp.Register("ControlItemTemplate", ref ControlItemTemplateProperty, (ControlTemplate)null);
			dProp.Register("PanelTemplate", ref PanelTemplateProperty, (ControlTemplate)null);
			dProp.Register("DocumentTemplate", ref DocumentTemplateProperty, (ControlTemplate)null);
			dProp.Register("MDIDocumentTemplate", ref MDIDocumentTemplateProperty, (ControlTemplate)null);
			dProp.Register("FloatDocumentTemplate", ref FloatDocumentTemplateProperty, (ControlTemplate)null);
			dProp.Register("GroupTemplate", ref GroupTemplateProperty, (ControlTemplate)null);
			dProp.Register("TabPanelTemplate", ref TabPanelTemplateProperty, (ControlTemplate)null);
			dProp.Register("DocumentPanelTemplate", ref DocumentPanelTemplateProperty, (ControlTemplate)null);
			dProp.Register("FloatingWindowTemplate", ref FloatingWindowTemplateProperty, (ControlTemplate)null);
			dProp.Register("FloatingAdornerTemplate", ref FloatingAdornerTemplateProperty, (ControlTemplate)null);
			dProp.Register("SplitterControlTemplate", ref SplitterControlTemplateProperty, (ControlTemplate)null);
			dProp.Register("EmptySpaceControlTemplate", ref EmptySpaceControlTemplateProperty, (ControlTemplate)null);
			dProp.Register("LabelControlTemplate", ref LabelControlTemplateProperty, (ControlTemplate)null);
			dProp.Register("SeparatorControlTemplate", ref SeparatorControlTemplateProperty, (ControlTemplate)null);
			dProp.Register("AutoHideGroupTemplate", ref AutoHideGroupTemplateProperty, (ControlTemplate)null);
		}
		#endregion static
		#region Templates
		public ControlTemplate EmptyTemplate {
			get { return (ControlTemplate)GetValue(EmptyTemplateProperty); }
			set { SetValue(EmptyTemplateProperty, value); }
		}
		public ControlTemplate ControlItemTemplate {
			get { return (ControlTemplate)GetValue(ControlItemTemplateProperty); }
			set { SetValue(ControlItemTemplateProperty, value); }
		}
		public ControlTemplate PanelTemplate {
			get { return (ControlTemplate)GetValue(PanelTemplateProperty); }
			set { SetValue(PanelTemplateProperty, value); }
		}
		public ControlTemplate DocumentTemplate {
			get { return (ControlTemplate)GetValue(DocumentTemplateProperty); }
			set { SetValue(DocumentTemplateProperty, value); }
		}
		public ControlTemplate MDIDocumentTemplate {
			get { return (ControlTemplate)GetValue(MDIDocumentTemplateProperty); }
			set { SetValue(MDIDocumentTemplateProperty, value); }
		}
		public ControlTemplate FloatDocumentTemplate {
			get { return (ControlTemplate)GetValue(FloatDocumentTemplateProperty); }
			set { SetValue(FloatDocumentTemplateProperty, value); }
		}
		public ControlTemplate GroupTemplate {
			get { return (ControlTemplate)GetValue(GroupTemplateProperty); }
			set { SetValue(GroupTemplateProperty, value); }
		}
		public ControlTemplate TabPanelTemplate {
			get { return (ControlTemplate)GetValue(TabPanelTemplateProperty); }
			set { SetValue(TabPanelTemplateProperty, value); }
		}
		public ControlTemplate DocumentPanelTemplate {
			get { return (ControlTemplate)GetValue(DocumentPanelTemplateProperty); }
			set { SetValue(DocumentPanelTemplateProperty, value); }
		}
		public ControlTemplate FloatingWindowTemplate {
			get { return (ControlTemplate)GetValue(FloatingWindowTemplateProperty); }
			set { SetValue(FloatingWindowTemplateProperty, value); }
		}
		public ControlTemplate FloatingAdornerTemplate {
			get { return (ControlTemplate)GetValue(FloatingAdornerTemplateProperty); }
			set { SetValue(FloatingAdornerTemplateProperty, value); }
		}
		public ControlTemplate SplitterControlTemplate {
			get { return (ControlTemplate)GetValue(SplitterControlTemplateProperty); }
			set { SetValue(SplitterControlTemplateProperty, value); }
		}
		public ControlTemplate EmptySpaceControlTemplate {
			get { return (ControlTemplate)GetValue(EmptySpaceControlTemplateProperty); }
			set { SetValue(EmptySpaceControlTemplateProperty, value); }
		}
		public ControlTemplate LabelControlTemplate {
			get { return (ControlTemplate)GetValue(LabelControlTemplateProperty); }
			set { SetValue(LabelControlTemplateProperty, value); }
		}
		public ControlTemplate SeparatorControlTemplate {
			get { return (ControlTemplate)GetValue(SeparatorControlTemplateProperty); }
			set { SetValue(SeparatorControlTemplateProperty, value); }
		}
		public ControlTemplate AutoHideGroupTemplate {
			get { return (ControlTemplate)GetValue(AutoHideGroupTemplateProperty); }
			set { SetValue(AutoHideGroupTemplateProperty, value); }
		}
		#endregion Templates
		public MultiTemplateControl() {
		}
		public BaseLayoutItem LayoutItem {
			get { return (BaseLayoutItem)GetValue(LayoutItemProperty); }
			set { SetValue(LayoutItemProperty, value); }
		}
		protected override void OnDispose() {
			IDisposable templateChild = LayoutItemsHelper.GetChild<DependencyObject>(this) as IDisposable;
			Ref.Dispose(ref templateChild);
			ClearValue(LayoutItemProperty);
			ClearValue(EmptyTemplateProperty);
			ClearValue(ControlItemTemplateProperty);
			ClearValue(PanelTemplateProperty);
			ClearValue(DocumentTemplateProperty);
			ClearValue(MDIDocumentTemplateProperty);
			ClearValue(FloatDocumentTemplateProperty);
			ClearValue(GroupTemplateProperty);
			ClearValue(TabPanelTemplateProperty);
			ClearValue(DocumentPanelTemplateProperty);
			ClearValue(FloatingWindowTemplateProperty);
			ClearValue(FloatingAdornerTemplateProperty);
			ClearValue(SplitterControlTemplateProperty);
			ClearValue(EmptySpaceControlTemplateProperty);
			ClearValue(LabelControlTemplateProperty);
			ClearValue(SeparatorControlTemplateProperty);
			ClearValue(AutoHideGroupTemplateProperty);
			ClearValue(TemplateProperty);
			DockLayoutManager.Release(this);
			base.OnDispose();
		}
		public void ClearTemplateIfNeeded(BaseLayoutItem item) {
			var template = SelectTemplateCore(item);
			if(template == Template) return;
			LayoutItem = null;
		}
		protected virtual void OnLayoutItemChanged(BaseLayoutItem item) {
			DependencyObject templateChild = LayoutItemsHelper.GetChild<DependencyObject>(this);
			if(item != null) {
				SetValue(DockLayoutManager.LayoutItemProperty, item);
				SelectTemplate(item);
			}
			else {
				if(templateChild is IDisposable)
					((IDisposable)templateChild).Dispose();
				ClearValue(TemplateProperty);
				DockLayoutManager.Release(this);
				if(templateChild != null)
					DockLayoutManager.Release(templateChild);
			}
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			SelectTemplate(LayoutItem);
		}
		protected override void OnPrepareContainerForItemComplete() {
			base.OnPrepareContainerForItemComplete();
			SelectTemplate(LayoutItem);
		}
		protected virtual void SelectTemplate(BaseLayoutItem item) {
			if(IsDisposing || !IsInitialized) return;
			Template = SelectTemplateCore(item);
		}
		ControlTemplate SelectTemplateCore(BaseLayoutItem item) {
			if(item == null)
				return EmptyTemplate;
			ControlTemplate template = null;
			switch(item.ItemType) {
				case LayoutItemType.ControlItem: template = ControlItemTemplate; break;
				case LayoutItemType.Panel: template = PanelTemplate; break;
				case LayoutItemType.Group: template = GroupTemplate; break;
				case LayoutItemType.FloatGroup: template = GetFloatGroupTemplate(); break;
				case LayoutItemType.TabPanelGroup: template = TabPanelTemplate; break;
				case LayoutItemType.Document: template = GetDocumentTemplate(); break;
				case LayoutItemType.DocumentPanelGroup: template = DocumentPanelTemplate; break;
				case LayoutItemType.LayoutSplitter: template = SplitterControlTemplate; break;
				case LayoutItemType.EmptySpaceItem: template = EmptySpaceControlTemplate; break;
				case LayoutItemType.Label: template = LabelControlTemplate; break;
				case LayoutItemType.Separator: template = SeparatorControlTemplate; break;
				case LayoutItemType.AutoHideGroup: template = AutoHideGroupTemplate; break;
			}
			return template;
		}
		ControlTemplate GetFloatGroupTemplate() {
			ControlTemplate result = null;
			DockLayoutManager container = DockLayoutManager.Ensure(this);
			if(container != null) {
				result = (container.GetRealFloatingMode() == Core.FloatingMode.Window) ?
					FloatingWindowTemplate : FloatingAdornerTemplate;
			}
			return result;
		}
		ControlTemplate GetDocumentTemplate() {
			DocumentPanel document = (DocumentPanel)LayoutItem;
			if(document.IsFloatingRootItem) return FloatDocumentTemplate;
			return document.IsMDIChild ? MDIDocumentTemplate : DocumentTemplate;
		}
	}
}
