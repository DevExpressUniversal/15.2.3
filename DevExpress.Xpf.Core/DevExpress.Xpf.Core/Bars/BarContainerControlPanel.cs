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
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public class BarContainerControlPanel : Panel, IMultipleElementRegistratorSupport {
		#region static
		static BarContainerControlPanel() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarContainerControlPanel), new FrameworkPropertyMetadata(typeof(BarContainerControlPanel)));
			EventManager.RegisterClassHandler(typeof(DragWidget), DragWidget.DragStartedEvent, new DragStartedEventHandler(OnDragWidgetDragStarted));
			EventManager.RegisterClassHandler(typeof(DragWidget), DragWidget.DragCompletedEvent, new DragCompletedEventHandler(OnDragWidgetDragCompleted));
			EventManager.RegisterClassHandler(typeof(DragWidget), DragWidget.DragDeltaEvent, new DragDeltaEventHandler(OnDragWidgetDragDelta));
		}
		static void OnDragWidgetDragDelta(object sender, DragDeltaEventArgs e) {
			foreach(BarContainerControlPanel panel in GetPanels(sender)) {
				panel.LayoutCalculator.OnBarControlDrag(GetLayoutInfo(sender), e);
			}
		}
		static void OnDragWidgetDragCompleted(object sender, DragCompletedEventArgs e) {
			foreach (BarContainerControlPanel panel in GetPanels(sender)) {
				panel.LayoutCalculator.OnBarControlDragCompleted(GetLayoutInfo(sender), e);
			}
		}
		static void OnDragWidgetDragStarted(object sender, DragStartedEventArgs e) {
			foreach (BarContainerControlPanel panel in GetPanels(sender)) {
				panel.LayoutCalculator.OnBarControlDragStart(GetLayoutInfo(sender), e);
			}
		}
		static IBarLayoutTableInfo GetLayoutInfo(object sender) {
			var bc = LayoutHelper.FindParentObject<BarControl>((DragWidget)sender);
			if (bc == null) return null;
			return bc.TemplatedParent as IBarLayoutTableInfo ?? bc;
		}
		static IEnumerable<BarContainerControlPanel> GetPanels(object sender) {
			return BarNameScope
				.GetService<IElementRegistratorService>((DragWidget)sender)
				.GetElements<IFrameworkInputElement>(ScopeSearchSettings.Ancestors | ScopeSearchSettings.Descendants | ScopeSearchSettings.Local)
				.OfType<BarContainerControlPanel>().ToList();
		}
		#endregion
		BaseBarLayoutCalculator layoutCalculator;
		public BarContainerControlPanel() {
		}
		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
			return new BarControlCollection(this, logicalParent);
		}
		BarContainerControl owner;
		public BarContainerControl Owner { 
			get {
				if(owner == null)
					owner = BarManagerHelper.FindContainerControl(this);
				return owner; 
			}
			set { owner = value; }
		}
		public double ColumnIndent { get { return Owner == null ? 0.0 : Owner.BarHorzIndent; } }
		public double RowIndent { get { return Owner == null ? 0.0 : Owner.BarVertIndent; } }
		protected internal BaseBarLayoutCalculator LayoutCalculator {
			get {
				if(layoutCalculator == null)
					layoutCalculator = CreateLayoutCalculator();
				return layoutCalculator;
			}
		}
		Orientation orientation;
		public Orientation Orientation {
			get { return orientation; }
			set {
				if (value == orientation) return;
				Orientation oldValue = orientation;
				orientation = value;
				OnOrientationChanged(oldValue);
			}
		}
		protected virtual void OnOrientationChanged(Orientation oldValue) {
			LayoutCalculator.Orientation = Orientation;
		}		
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			if (VisualChildrenChanged != null) {
				var args = new NotifyCollectionChangedEventArgs(visualAdded == null ? NotifyCollectionChangedAction.Remove : NotifyCollectionChangedAction.Add, visualAdded ?? visualRemoved);
				VisualChildrenChanged(this, args);
			}
		}
		public event NotifyCollectionChangedEventHandler VisualChildrenChanged;
		protected virtual BaseBarLayoutCalculator CreateLayoutCalculator() {
			if (Owner.Return(x => x.IsFloating, () => false))
				return new FloatingBarLayoutCalculator(this);
			return new BarLayoutCalculator2(this);			
		}				
		protected override Size MeasureOverride(Size constraint) {			
			return LayoutCalculator.MeasureContainer(constraint);
		}
		protected override Size ArrangeOverride(Size finalSize) {							
			return LayoutCalculator.ArrangeContainer(finalSize);
		}
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys { get { yield return typeof(IFrameworkInputElement); } }
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) { return string.Empty; }
	}
}
