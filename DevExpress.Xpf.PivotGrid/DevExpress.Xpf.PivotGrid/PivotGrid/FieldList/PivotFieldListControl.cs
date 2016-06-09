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
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid {
	[
	TemplateVisualState(GroupName = PivotFieldListControl.EnableDragOverGroupName, Name = PivotFieldListControl.EnableDragOverStateName),
	TemplateVisualState(GroupName = PivotFieldListControl.EnableDragOverGroupName, Name = PivotFieldListControl.DisableDragOverStateName),
	]
	[DXToolboxBrowsable]
	public class PivotFieldListControl : FieldListControlBase {
		const string EnableDragOverGroupName = "EnableDragOver", EnableDragOverStateName = "Enable", DisableDragOverStateName = "DisableD";
		int diff = 20;
		#region static stuff
		public static readonly DependencyProperty ShowBorderProperty =
	DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(PivotFieldListControl), new PropertyMetadata(false, (d, e) => ((PivotFieldListControl)d).OnShowBorderChanged()));
		public static readonly DependencyProperty LabelVisibilityProperty =
DependencyPropertyManager.Register("LabelVisibility", typeof(Visibility), typeof(PivotFieldListControl), new PropertyMetadata(Visibility.Collapsed, (d, e) => ((PivotFieldListControl)d).LabelVisibilityChanged()));
		public static readonly DependencyProperty EmptyTextVisibilityProperty =
DependencyPropertyManager.Register("EmptyTextVisibility", typeof(Visibility), typeof(PivotFieldListControl), new PropertyMetadata(Visibility.Collapsed, (d, e) => ((PivotFieldListControl)d).OnShowBorderChanged()));
		public static readonly DependencyProperty AreaProperty =
	DependencyPropertyManager.Register("Area", typeof(FieldListArea), typeof(PivotFieldListControl), new PropertyMetadata(FieldListArea.All));
		public static readonly DependencyProperty OrientationProperty =
	DependencyPropertyManager.Register("Orientation", typeof(FieldListOrientation), typeof(PivotFieldListControl), new PropertyMetadata(FieldListOrientation.Vertical, (d, e) => ((PivotFieldListControl)d).OnOrientationChanged()));
		static readonly DependencyPropertyKey ActualOrientationPropertyKey =
	   DependencyPropertyManager.RegisterReadOnly("ActualOrientation", typeof(FieldListOrientation), typeof(PivotFieldListControl), new PropertyMetadata(FieldListOrientation.Vertical));
		public static readonly DependencyProperty ActualOrientationProperty = ActualOrientationPropertyKey.DependencyProperty;
		#endregion
		public Visibility LabelVisibility {
			get { return (Visibility)GetValue(LabelVisibilityProperty); }
			set { SetValue(LabelVisibilityProperty, value); }
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		public Visibility EmptyTextVisibility {
			get { return (Visibility)GetValue(EmptyTextVisibilityProperty); }
			set { SetValue(EmptyTextVisibilityProperty, value); }
		}
		public FieldListArea Area {
			get { return (FieldListArea)GetValue(AreaProperty); }
			set { SetValue(AreaProperty, value); }
		}
		public FieldListOrientation Orientation {
			get { return (FieldListOrientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public FieldListOrientation ActualOrientation {
			get { return (FieldListOrientation)GetValue(ActualOrientationProperty); }
			private set { this.SetValue(ActualOrientationPropertyKey, value); }
		}
		public PivotFieldListControl() {
			this.SetDefaultStyleKey(typeof(PivotFieldListControl));
			SizeChanged += OnSizeChanged;
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if(Orientation == FieldListOrientation.Auto)
				SelectOrientation();
		}
		void OnOrientationChanged() {
			if(Orientation != FieldListOrientation.Auto) {
				ActualOrientation = Orientation;
				return;
			}
			SelectOrientation();
		}
		void SelectOrientation() {
			if(GroupFields || IncludeVisibleFields) {
				ActualOrientation = FieldListOrientation.Vertical;
				return;
			}
			if(Orientation != FieldListOrientation.Auto) {
				ActualOrientation = Orientation;
				return;
			}
			if(ActualHeight == 0 || ActualWidth == 0)
				return;
			if(ActualOrientation == FieldListOrientation.Horizontal && ActualWidth < ActualHeight - diff)
				ActualOrientation = FieldListOrientation.Vertical;
			if(ActualOrientation == FieldListOrientation.Vertical && ActualHeight < ActualWidth - diff)
				ActualOrientation = FieldListOrientation.Horizontal;
		}
		protected override Size MeasureOverride(Size constraint) {
			return base.MeasureOverride(constraint);
		}
		void OnShowBorderChanged() {
			BorderThickness = new Thickness(ShowBorder ? 1 : 0);
		}
		void LabelVisibilityChanged() {
			VisualStateManager.GoToState(this, LabelVisibility == Visibility.Visible ? EnableDragOverStateName : DisableDragOverStateName, false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			LabelVisibilityChanged();
		}
		protected override void OnGroupFieldsChanged() {
			base.OnGroupFieldsChanged();
			SelectOrientation();
		}
		protected override void OnIncludeVisibleFieldsChanged() {
			base.OnIncludeVisibleFieldsChanged();
			SelectOrientation();
		}
	}
	public abstract class FieldListControlBase : ColumnChooserControlBase {
		bool isExternal = true;
		#region static stuff
		public static readonly DependencyProperty GroupFieldsProperty =
			DependencyPropertyManager.Register("GroupFields", typeof(bool), typeof(FieldListControlBase), new PropertyMetadata(false, (d, e) => ((FieldListControlBase)d).OnGroupFieldsChanged()));
		public static readonly DependencyProperty IncludeVisibleFieldsProperty =
			DependencyPropertyManager.Register("IncludeVisibleFields", typeof(bool), typeof(FieldListControlBase), new PropertyMetadata(false, (d, e) => ((FieldListControlBase)d).OnIncludeVisibleFieldsChanged()));
		protected virtual void OnGroupFieldsChanged() { }
		protected virtual void OnIncludeVisibleFieldsChanged() { }
		#endregion
		public FieldListControlBase() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			if(PivotGrid != null) {
				PivotGrid.UnregisteredFieldListControl(this);
			}
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			if(PivotGrid != null) {
				PivotGrid.RegisteredFieldListControl(this);
			}
		}
		public bool GroupFields {
			get { return (bool)GetValue(GroupFieldsProperty); }
			set { SetValue(GroupFieldsProperty, value); }
		}
		public bool IncludeVisibleFields {
			get { return (bool)GetValue(IncludeVisibleFieldsProperty); }
			set { SetValue(IncludeVisibleFieldsProperty, value); }
		}
		protected PivotGridControl PivotGrid { get { return Owner as PivotGridControl; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsExternal {
			get { return isExternal; }
			set {
				if(isExternal == value) return;
				isExternal = value;
				if(PivotGrid == null) return;
				if(!value)
					DecreaseExternalFieldListCount(PivotGrid);
				else {
					IncreaseExternalFieldListCount(PivotGrid);
#if !SL
					if(LogicalTreeHelper.GetParent(this) == PivotGrid && PivotGrid != null)
						PivotGrid.RemoveChild(this);
#endif
				}
			}
		}
		protected override void OnOwnerChanged(ILogicalOwner oldOwner, ILogicalOwner newOwner) {
			OnPivotGridChanged((PivotGridControl)oldOwner, (PivotGridControl)newOwner);
			base.OnOwnerChanged(oldOwner, newOwner);
#if !SL
			if(IsExternal && LogicalTreeHelper.GetParent(this) == PivotGrid && PivotGrid != null)
				PivotGrid.RemoveChild(this);
#endif
		}
		protected void OnPivotGridChanged(PivotGridControl oldPivot, PivotGridControl newPivot) {
			PivotGridControl.SetPivotGrid(this, newPivot);
			if(oldPivot != null)
				oldPivot.UnregisteredFieldListControl(this);
			if(!IsExternal) return;
			if(oldPivot != null)
				DecreaseExternalFieldListCount(oldPivot);
			if(newPivot != null) {
				PivotGridControl.SetPivotGrid(this, newPivot);
				IncreaseExternalFieldListCount(newPivot);
			}
		}
		void IncreaseExternalFieldListCount(PivotGridControl pivot) {
			pivot.ExternalFieldListCount++;
		}
		void DecreaseExternalFieldListCount(PivotGridControl pivot) {
			if(pivot.ExternalFieldListCount <= 0) return;
			pivot.ExternalFieldListCount--;
		}
#if !SL
		protected override void ExecutedRoutedEventHandler(object sender, ExecutedRoutedEventArgs e) {
			if(e.Handled)
				return;
			base.ExecutedRoutedEventHandler(sender, e);
			if(e.Command as RoutedCommand != null)
				e.Handled = true;
		}
#endif
	}
}
