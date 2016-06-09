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

#if SILVERLIGHT
extern alias Platform;
#endif
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Linq;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Design.Policies;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.Windows.Design.Services;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Interop;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
using DevExpress.Design.UI;
#if SILVERLIGHT
using DependencyObject = Platform::System.Windows.DependencyObject;
using DependencyProperty = Platform::System.Windows.DependencyProperty;
using PropertyMetadata = Platform::System.Windows.PropertyMetadata;
using Point = Platform::System.Windows.Point;
using RoutedEventHandler = Platform::System.Windows.RoutedEventHandler;
using RoutedEventArgs = Platform::System.Windows.RoutedEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using FrameworkElement = Platform::System.Windows.FrameworkElement;
using ToggleStateButton = DevExpress.Xpf.Core.Design.CoreUtils.ToggleStateButton;
using UIElement = Platform::System.Windows.FrameworkElement;
using LayoutHelper = Platform::DevExpress.Xpf.Core.Native.LayoutHelper;
using PivotGridFieldCollection =Platform::DevExpress.Xpf.PivotGrid.PivotGridFieldCollection;
using HitTestResult =  Platform::DevExpress.Xpf.Core.HitTestResult;
using HitTestResultBehavior = Platform::DevExpress.Xpf.Core.HitTestResultBehavior;
using HitTestFilterCallback = Platform::DevExpress.Xpf.Core.HitTestFilterCallback;
using HitTestResultCallback = Platform::DevExpress.Xpf.Core.HitTestResultCallback;
using HitTestFilterBehavior = Platform::DevExpress.Xpf.Core.HitTestFilterBehavior;
using PointHitTestParameters = Platform::DevExpress.Xpf.Core.PointHitTestParameters;
using FieldHeader = Platform::DevExpress.Xpf.PivotGrid.Internal.FieldHeader;
using PivotGridControl = Platform::DevExpress.Xpf.PivotGrid.PivotGridControl;
using PivotGridField = Platform::DevExpress.Xpf.PivotGrid.PivotGridField;
using VisualTreeHelper = Platform::System.Windows.Media.VisualTreeHelper;
using DevExpress.Xpf.Core.Design.CoreUtils;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Data;
using Platform::DevExpress.Xpf.Bars.Helpers;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Utils;
using Platform::DevExpress.Xpf.Core.Commands;
using Platform::DevExpress.Xpf.Core.WPFCompatibility;
using IDesignTimeAdorner = Platform::DevExpress.Xpf.PivotGrid.Internal.IDesignTimeAdorner;
using PivotGridHelper = Platform::DevExpress.Xpf.PivotGrid.Internal.PivotGridHelper;
using FieldHeaderContentControl = Platform::DevExpress.Xpf.PivotGrid.Internal.FieldHeaderContentControl;
#else
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Data;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Commands;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#endif
namespace DevExpress.Xpf.PivotGrid.Design {
	[FeatureConnector(typeof(DataControlAdornerFeatureConnector))]
	[UsesItemPolicy(typeof(DataControlPolicy))]
	public class PivotGridControlAdornerProvider : DXAdornerProviderBase, IDesignTimeAdorner {
		UserControl PivotGridControlAdornerPanel { get { return (UserControl)hookPanel; } }
		protected PivotGridFieldCollection Fields { get { return PivotGrid.Fields; } }
		internal static bool CanSelectField {
			get {
#if !SILVERLIGHT
				return !BlendHelper.IsInBlend;
#else
				return true;
#endif
			}
		}
		List<FieldHeader> highlightedHeaders;
		PivotGridControl pivotGrid;
		internal PivotGridControl PivotGrid {
			get {
				if(this.pivotGrid == null)
					this.pivotGrid = this.platformObject as PivotGridControl ?? AdornedElement.GetCurrentValue() as PivotGridControl;
				return this.pivotGrid;
			}
		}
		public PivotGridControlAdornerProvider() {
			PivotGridControlAdornerPanel.MouseLeftButtonDown += new MouseButtonEventHandler(GridControlAdornerPanel_MouseLeftButtonDown);
			PivotGridControlAdornerPanel.MouseMove += new MouseEventHandler(GridControlAdornerPanel_MouseMove);
			PivotGridControlAdornerPanel.MouseLeftButtonUp += new MouseButtonEventHandler(GridControlAdornerPanel_MouseLeftButtonUp);
		}
		ModelItem GetFieldModelItem(PivotGridField field) {
			int index = Fields.IndexOf(field);
			ModelItemCollection fields = GetPivotGridFields();
			return 0 <= index && index < fields.Count ? fields[index] : null;
		}
		ModelItemCollection GetPivotGridFields() {
			return PivotGridDesignTimeHelper.GetPivotGridFieldsCollection(AdornedElement);
		}
		void GridControlAdornerPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			PivotGridField hitField = GetHitField(GetPivotGridMousePosition(e));
			bool reRaiseEvent = true;
			if(hitField != null) {
				ModelItem fieldItem = GetFieldModelItem(hitField);
				if(fieldItem != null && PivotGridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context) != fieldItem) {
					SelectionOperations.Select(AdornedElement.Context, fieldItem);
					reRaiseEvent = false;
				}
			} else {
				SelectionOperations.Select(AdornedElement.Context, AdornedElement);
			}
			if(reRaiseEvent) {
#if !SILVERLIGHT
				ReraiseEventHelper.ReraiseEvent<MouseButtonEventArgs>(e, GetHitTestElement(e), FrameworkElement.PreviewMouseDownEvent, FrameworkElement.MouseDownEvent, ReraiseEventHelper.CloneMouseButtonEventArgs);
#else
#endif
			}
			e.Handled = true;
		}
		PivotGridField GetHitField(Point point) {
			if(!CanSelectField)
				return null;
		  UIElement hitTestElement = GetHitTestElement(point);		 
		  if(hitTestElement == null)
				return null;
		  FieldHeader header = LayoutHelper.FindParentObject<FieldHeader>(hitTestElement);
		  return header == null ? null : header.Field;
		}
		void GridControlAdornerPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
#if !SILVERLIGHT
			ReraiseEventHelper.ReraiseEvent<MouseButtonEventArgs>(e, GetHitTestElement(e), FrameworkElement.PreviewMouseUpEvent, FrameworkElement.MouseUpEvent, ReraiseEventHelper.CloneMouseButtonEventArgs);
#else
#endif
			e.Handled = true;
		}
		Point GetSilverlightHitTestPoint(Point point) {
#if !SILVERLIGHT
			return point;
#else
			Point location = Platform::DevExpress.Xpf.Core.Native.RectHelper.TopLeft(LayoutHelper.GetRelativeElementRect(PivotGrid, LayoutHelper.FindRoot(PivotGrid) as UIElement));
			Platform::DevExpress.Xpf.Core.Native.PointHelper.Offset(ref location, point.X, point.Y);
			return location;
#endif
		}
		void GridControlAdornerPanel_MouseMove(object sender, MouseEventArgs e) {
#if !SILVERLIGHT
			ReraiseEventHelper.ReraiseEvent<MouseEventArgs>(e, GetHitTestElement(e), FrameworkElement.PreviewMouseMoveEvent, FrameworkElement.MouseMoveEvent, ReraiseEventHelper.CloneMouseEventArgs);
#else
#endif
			e.Handled = true;
		}
		UIElement GetHitTestElement(MouseEventArgs e) {
			return GetHitTestElement(GetPivotGridMousePosition(e));
		}
		UIElement GetHitTestElement(Point point) {
			HitTestResult hitResult = null;
			HitTestResultCallback resultCallback = delegate(HitTestResult result) {
				if(LayoutHelper.FindParentObject<FieldHeader>(result.VisualHit) == null)
					return HitTestResultBehavior.Continue;
				hitResult = result;
				return HitTestResultBehavior.Stop;
			};
#if !SILVERLIGHT
			VisualTreeHelper.HitTest(PivotGrid, HitTestFilter, resultCallback, new PointHitTestParameters(point));
#else
			HitTestHelper.HitTest(PivotGrid, HitTestFilter, resultCallback, new PointHitTestParameters(GetSilverlightHitTestPoint(point)), false, false);
#endif
			if(hitResult == null)
				return null;
			DependencyObject visualHit = hitResult.VisualHit as DependencyObject;
			return visualHit != null ? LayoutHelper.FindParentObject<UIElement>(visualHit) : null;
		}
		HitTestFilterBehavior HitTestFilter(DependencyObject potentialHitTestTarget) {
			FrameworkElement element = potentialHitTestTarget as FrameworkElement;
			if(element == null || !UIElementHelper.IsVisibleInTree(element))
				return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
			return HitTestFilterBehavior.Continue;
		}
		Point GetPivotGridMousePosition(MouseEventArgs e) {
			Point position = DesignHelper.ConvertToPlatformPoint(e.GetPosition(hookPanel));
			return position;
		}
		void DestroyHeaderAdorner() {
			if(highlightedHeaders != null) {
				highlightedHeaders.ForEach(header => header.IsSelectedInDesignTime = false);
				highlightedHeaders = null;
			}
		}
		protected override Control CreateHookPanel() {
			return new UserControl() { Content = new Border() { Background = new SolidColorBrush(Colors.Transparent) } };
		}
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			AdornedElement.Context.Items.Subscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
			SubscribePivotGridEvents();
		}
		protected override void Deactivate() {
			UnsubscribePivotGridEvents();
			AdornedElement.Context.Items.Unsubscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
			base.Deactivate();
		}
		void SubscribePivotGridEvents() {
			if(PivotGrid == null)
				return;
			PivotGridHelper.SetDesignTimeAdorner(PivotGrid, this);
			PivotGrid.Unloaded += new RoutedEventHandler(OnGridControlUnloaded);
		}
		void UnsubscribePivotGridEvents() {
			if(PivotGrid == null)
				return;
			PivotGrid.Unloaded -= new RoutedEventHandler(OnGridControlUnloaded);
			PivotGridHelper.SetDesignTimeAdorner(PivotGrid, null);
		}
		void OnGridControlUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribePivotGridEvents();
			RefreshPlatformObject();
			SubscribePivotGridEvents();
		}
		void OnSelectionChanged(Selection newSelection) {
			RefreshFieldAdorner();
		}
		void UpdateFieldProperty(ModelItem item, DependencyProperty property) {
			if(item == null)
				return;
			ModelProperty modelProperty = item.Properties[DesignHelper.GetPropertyName(property)];
			PerformEditAction(string.Format(SR.ChangeFieldProperty, PivotGridDesignTimeHelper.SplitString(DesignHelper.GetPropertyName(property))), delegate {
				modelProperty.SetValue(modelProperty.ComputedValue);
			});
		}
		void RefreshFieldAdorner() {
			DestroyHeaderAdorner();
			if(AdornedElement == null)
				return;
			ModelItem primarySelection = PivotGridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context);
			if(IsFieldSelected(primarySelection)) {
				PivotGridField field = GetPivotGridFieldBytModelItem(primarySelection);
				if(field != null) {
					highlightedHeaders = new List<FieldHeader>();
					FieldHeader headerElements = PivotGridDesignTimeHelper.GetFieldHeaderElements(PivotGrid, field);
					if(headerElements != null) {
							highlightedHeaders.Add(headerElements);
							headerElements.IsSelectedInDesignTime = true;						}
				}
			}
		}
		PivotGridField GetPivotGridFieldBytModelItem(ModelItem fieldItem) {
			int index = GetPivotGridFields().IndexOf(fieldItem);
			return 0 <= index && index < Fields.Count ? Fields[index] : null;
		}
		bool IsFieldSelected(ModelItem primarySelection) {
			return primarySelection != null && IsOwnFieldItem(primarySelection);
		}
		bool IsOwnFieldItem(ModelItem fieldItem) {
			return GetPivotGridFields().Contains(fieldItem);
		}
		protected void PerformEditAction(string description, Action editAction) {
			using(ModelEditingScope batchedChangeRoot = AdornedElement.BeginEdit(description)) {
				editAction();
				batchedChangeRoot.Complete();
			}
		}
		void IDesignTimeAdorner.PerformChangeSortOrder(PivotGridField field) {
			if(field == null)
				return;
			UpdateFieldProperty(GetFieldModelItem(field), PivotGridField.SortOrderProperty);
		}
		void IDesignTimeAdorner.PerformDragDrop() {
			PerformEditAction(SR.OrderField, () => {
				foreach(ModelItem item in GetPivotGridFields()) {
					UpdateModelItemProperty(item, PivotGridField.AreaIndexProperty);
					UpdateModelItemProperty(item, PivotGridField.AreaProperty);
				}
			});
			RefreshFieldAdorner();
		}
		IModelItem IDesignTimeAdorner.GetPivotGridModelItem() {
		   return XpfModelItem.FromModelItem(AdornedElement);
		}
		void UpdateModelItemProperty(ModelItem item, DependencyProperty property) {
			ModelProperty modelProperty = item.Properties[DesignHelper.GetPropertyName(property)];
			if(!modelProperty.IsSet && modelProperty.ComputedValue == modelProperty.DefaultValue)
				return;
			modelProperty.SetValue(modelProperty.ComputedValue);
		}
		void IDesignTimeAdorner.PerformChangeUnboundExpression(PivotGridField field) {
			if(field == null)
				return;
			UpdateFieldProperty(GetFieldModelItem(field), PivotGridField.UnboundExpressionProperty);
		}
		public bool IsDesignTime { get { return true; } }
	}
}
