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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public abstract class XRDiagramItemBase {
		static readonly DependencyPropertyKey DiagramPropertyKey;
		public static readonly DependencyProperty DiagramProperty;
		static readonly DependencyPropertyKey PropertiesModelPropertyKey;
		public static readonly DependencyProperty PropertiesModelProperty;
		public static readonly DependencyProperty OnIsSelectedChangedActionProperty;
		public static readonly DependencyProperty EditablePropertiesProperty;
		public static readonly DependencyProperty ShowBordersProperty;
		public static readonly DependencyProperty ShowBindingIconProperty;
		public static readonly DependencyProperty BandCollapsedProperty;
		public static readonly DependencyProperty BandContentVisibleProperty;
		public static readonly DependencyProperty BandHeaderVisibleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty LogicalChildrenProperty;
		public static readonly DependencyProperty BeforeDeleteCallbackProperty;
		public static readonly DependencyProperty OnAttachItemCallbackProperty;
		public static readonly DependencyProperty OnDetachItemCallbackProperty;
		public static readonly DependencyProperty ContextMenuTemplateProperty;
		public static readonly DependencyProperty ContextMenuProperty;
		static XRDiagramItemBase() {
			DependencyPropertyRegistrator<XRDiagramItemBase>.New()
				.RegisterAttachedReadOnly((DiagramItem d) => GetDiagram(d), out DiagramPropertyKey, out DiagramProperty, null, OnDiagramChanged)
				.RegisterAttachedReadOnly((DiagramItem d) => GetPropertiesModel(d), out PropertiesModelPropertyKey, out PropertiesModelProperty, null)
				.RegisterAttached((DiagramItem d) => GetOnIsSelectedChangedAction(d), out OnIsSelectedChangedActionProperty, null)
				.RegisterAttached((DiagramItem d) => GetEditableProperties(d), out EditablePropertiesProperty, null)
				.RegisterAttached((DiagramItem d) => GetShowBorders(d), out ShowBordersProperty, false)
				.RegisterAttached((DiagramItem d) => GetShowBindingIcon(d), out ShowBindingIconProperty, false)
				.RegisterAttached((DiagramItem d) => GetLogicalChildren(d), out LogicalChildrenProperty, null)
				.RegisterAttached((DiagramItem d) => GetBeforeDeleteCallback(d), out BeforeDeleteCallbackProperty, null)
				.RegisterAttached((DiagramItem d) => GetOnAttachItemCallback(d), out OnAttachItemCallbackProperty, null, d => GetDiagram(d).With(x => GetOnAttachItemCallback(d)).Do(y => y()))
				.RegisterAttached((DiagramItem d) => GetOnDetachItemCallback(d), out OnDetachItemCallbackProperty, null, d => GetDiagram(d).With(x => GetOnDetachItemCallback(d)).Do(y => y()))
				.RegisterAttached((DiagramItem d) => GetContextMenu(d), out ContextMenuProperty, null, OnContextMenuChanged)
				.RegisterAttached((DiagramItem d) => GetContextMenuTemplate(d), out ContextMenuTemplateProperty, null, OnContextMenuTemplateChanged)
				.RegisterAttached((DiagramItem d) => GetBandCollapsed(d), out BandCollapsedProperty, false, OnBandCollapsedPropertyChanged)
				.RegisterAttached((DiagramItem d) => GetBandContentVisible(d), out BandContentVisibleProperty, true, FrameworkPropertyMetadataOptions.Inherits)
				.RegisterAttached((DiagramItem d) => GetBandHeaderVisible(d), out BandHeaderVisibleProperty, true, FrameworkPropertyMetadataOptions.Inherits)
			;
		}
		static void OnBandCollapsedPropertyChanged(DiagramItem d) {
			bool bandCollapsed = GetBandCollapsed(d);
			IEnumerable<DiagramItem> childrenDiagramItems = d.GetChildren().Cast<DiagramItem>();
			childrenDiagramItems.ForEach(x => InvalidateChildDiagramItem(d, x));
			if(bandCollapsed)
				SetBandContentVisible(d, false);
			else
				d.ClearValue(BandContentVisibleProperty);
		}
		public static void InvalidateChildDiagramItem(DiagramItem bandDiagramItem, DiagramItem diagramItem) {
			bool bandCollapsed = GetBandCollapsed(bandDiagramItem);
			if(bandCollapsed)
				SetBandHeaderVisible(diagramItem, false);
			else
				diagramItem.ClearValue(BandHeaderVisibleProperty);
		}
		public static XRDiagramControl GetDiagram(DiagramItem d) { return (XRDiagramControl)d.GetValue(DiagramProperty); }
		internal static void SetDiagram(DiagramItem d, XRDiagramControl v) { d.SetValue(DiagramPropertyKey, v); }
		public static SelectionModel<IDiagramItem> GetPropertiesModel(DiagramItem d) { return (SelectionModel<IDiagramItem>)d.GetValue(PropertiesModelProperty); }
		static void SetPropertiesModel(DiagramItem d, SelectionModel<IDiagramItem> v) { d.SetValue(PropertiesModelPropertyKey, v); }
		public static Action GetOnIsSelectedChangedAction(DiagramItem d) { return (Action)d.GetValue(OnIsSelectedChangedActionProperty); }
		public static void SetOnIsSelectedChangedAction(DiagramItem d, Action v) { d.SetValue(OnIsSelectedChangedActionProperty, v); }
		public static IEnumerable<PropertyDescriptor> GetEditableProperties(DiagramItem d) { return (IEnumerable<PropertyDescriptor>)d.GetValue(EditablePropertiesProperty); }
		public static void SetEditableProperties(DiagramItem d, IEnumerable<PropertyDescriptor> v) { d.SetValue(EditablePropertiesProperty, v); }
		public static bool GetShowBorders(DiagramItem d) { return (bool)d.GetValue(ShowBordersProperty); }
		public static void SetShowBorders(DiagramItem d, bool v) { d.SetValue(ShowBordersProperty, v); }
		public static bool GetShowBindingIcon(DiagramItem d) { return (bool)d.GetValue(ShowBindingIconProperty); }
		public static void SetShowBindingIcon(DiagramItem d, bool v) { d.SetValue(ShowBindingIconProperty, v); }
		public static bool GetBandCollapsed(DiagramItem d) { return (bool)d.GetValue(BandCollapsedProperty); }
		public static void SetBandCollapsed(DiagramItem d, bool v) { d.SetValue(BandCollapsedProperty, v); }
		public static bool GetBandContentVisible(DiagramItem d) { return (bool)d.GetValue(BandContentVisibleProperty); }
		public static void SetBandContentVisible(DiagramItem d, bool v) { d.SetValue(BandContentVisibleProperty, v); }
		public static bool GetBandHeaderVisible(DiagramItem d) { return (bool)d.GetValue(BandHeaderVisibleProperty); }
		public static void SetBandHeaderVisible(DiagramItem d, bool v) { d.SetValue(BandHeaderVisibleProperty, v); }
		public static Action<Transaction> GetBeforeDeleteCallback(DiagramItem d) { return (Action<Transaction>)d.GetValue(BeforeDeleteCallbackProperty); }
		public static void SetBeforeDeleteCallback(DiagramItem d, Action<Transaction> v) { d.SetValue(BeforeDeleteCallbackProperty, v); }
		public static Action GetOnAttachItemCallback(DiagramItem d) { return (Action)d.GetValue(OnAttachItemCallbackProperty); }
		public static void SetOnAttachItemCallback(DiagramItem d, Action v) { d.SetValue(OnAttachItemCallbackProperty, v); }
		public static Action GetOnDetachItemCallback(DiagramItem d) { return (Action)d.GetValue(OnDetachItemCallbackProperty); }
		public static void SetOnDetachItemCallback(DiagramItem d, Action v) { d.SetValue(OnDetachItemCallbackProperty, v); }
		public static IAdorner CreateDragPreviewAdorner(IDiagramControl diagram) {
			return ((XRDiagramControl)diagram).CreateAdorner(new Border { BorderThickness = new Thickness(2), BorderBrush = Brushes.Black });
		}
		static void OnDiagramChanged(DiagramItem d) {
			SetPropertiesModel(d, (SelectionModel<IDiagramItem>)d.GetDiagram().Controller.CreateMultiModel(typeof(SelectionModel<>), typeof(SelectionModel<>), true, new[] { d }));
		}
		static void OnContextMenuTemplateChanged(DiagramItem d, DependencyPropertyChangedEventArgs e) {
			var template = (DataTemplate)e.NewValue;
			SetContextMenu(d, template.With(x => XamlTemplateHelper.CreateObjectFromTemplate<PopupMenu>(x)));
		}
		static void OnContextMenuChanged(DiagramItem d, DependencyPropertyChangedEventArgs e) {
			var oldValue = (PopupMenu)e.OldValue;
			var newValue = (PopupMenu)e.NewValue;
			if(oldValue != null)
				RemoveLogicalChild(d, oldValue);
			if(newValue != null) {
				newValue.DataContext = null;
				AddLogicalChild(d, newValue);
			}
		}
		public static PopupMenu GetContextMenu(DiagramItem d) { return (PopupMenu)d.GetValue(ContextMenuProperty); }
		public static void SetContextMenu(DiagramItem d, PopupMenu menu) { d.SetValue(ContextMenuProperty, menu); }
		public static DataTemplate GetContextMenuTemplate(DiagramItem d) { return (DataTemplate)d.GetValue(ContextMenuTemplateProperty); }
		public static void SetContextMenuTemplate(DiagramItem d, DataTemplate menuTemplate) { d.SetValue(ContextMenuTemplateProperty, menuTemplate); }
		public static void AddLogicalChild(DiagramItem d, object child) {
			GetLogicalChildren(d).Add(child);
		}
		public static void RemoveLogicalChild(DiagramItem d, object child) {
			GetLogicalChildren(d).Remove(child);
		}
		public static IEnumerator GetLogicalChildrenEnumerator(DiagramItem d, IEnumerator baseChildren) {
			return new MergedEnumerator(baseChildren, GetLogicalChildren(d).GetEnumerator());
		}
		static List<object> GetLogicalChildren(DiagramItem d) {
			var list = (List<object>)d.GetValue(LogicalChildrenProperty);
			if(list == null) {
				list = new List<object>();
				d.SetValue(LogicalChildrenProperty, list);
			}
			return list;
		}
	}
}
