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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking {
	public class LayoutControlItem : ContentItem, IUIElement {
		#region static
		public static readonly DependencyProperty CaptionToControlDistanceProperty;
		public static readonly DependencyProperty ControlProperty;
		static readonly DependencyPropertyKey ControlPropertyKey;
		public static readonly DependencyProperty ShowControlProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty DesiredSizeInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HasDesiredSizeProperty;
		internal static readonly DependencyPropertyKey HasDesiredSizePropertyKey;
		public static readonly DependencyProperty ActualCaptionMarginProperty;
		static readonly DependencyPropertyKey ActualCaptionMarginPropertyKey;
		public static readonly DependencyProperty HasControlProperty;
		internal static readonly DependencyPropertyKey HasControlPropertyKey;
		public static readonly DependencyProperty ControlHorizontalAlignmentProperty;
		public static readonly DependencyProperty ControlVerticalAlignmentProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty HasVisibleCaptionProperty;
		static LayoutControlItem() {
			var dProp = new DependencyPropertyRegistrator<LayoutControlItem>();
			dProp.OverrideMetadata(AllowFloatProperty, false);
			dProp.OverrideMetadata(AllowDockProperty, false);
			dProp.OverrideMetadata(AllowCloseProperty, false);
			dProp.OverrideMetadata(ItemHeightProperty, new GridLength(1, GridUnitType.Auto));
			dProp.RegisterReadonly("Control", ref ControlPropertyKey, ref ControlProperty, (UIElement)null,
				(dObj, e) => ((LayoutControlItem)dObj).OnControlChanged((UIElement)e.NewValue, (UIElement)e.OldValue));
			dProp.Register("ShowControl", ref ShowControlProperty, true,
				(dObj, e) => ((LayoutControlItem)dObj).OnShowControlChanged((bool)e.NewValue));
			dProp.RegisterReadonly("HasControl", ref HasControlPropertyKey, ref HasControlProperty, false);
			dProp.Register("CaptionToControlDistance", ref CaptionToControlDistanceProperty, double.NaN,
				(dObj, e) => ((LayoutControlItem)dObj).OnCaptionToControlDistanceChanged((double)e.NewValue),
				(dObj, value) => ((LayoutControlItem)dObj).CoerceCaptionToControlDistance((double)value));
			dProp.Register("DesiredSizeInternal", ref DesiredSizeInternalProperty, Size.Empty,
				(dObj, e) => ((LayoutControlItem)dObj).OnDesiredSizeChanged((Size)e.NewValue),
				(dObj, value) => ((LayoutControlItem)dObj).CoerceDesiredSize((Size)value));
			dProp.RegisterReadonly("HasDesiredSize", ref HasDesiredSizePropertyKey, ref HasDesiredSizeProperty, false);
			dProp.RegisterReadonly("ActualCaptionMargin", ref ActualCaptionMarginPropertyKey, ref ActualCaptionMarginProperty, new Thickness(double.NaN), null,
				(dObj, value) => ((LayoutControlItem)dObj).CoerceActualCaptionMargin((Thickness)value));
			dProp.Register("ControlHorizontalAlignment", ref ControlHorizontalAlignmentProperty, HorizontalAlignment.Stretch);
			dProp.Register("ControlVerticalAlignment", ref ControlVerticalAlignmentProperty, VerticalAlignment.Stretch);
			dProp.Register("HasVisibleCaption", ref HasVisibleCaptionProperty, false,
				(dObj, e) => ((LayoutControlItem)dObj).OnHasVisibleCaptionChanged((bool)e.NewValue));
		}
		#endregion static
		public LayoutControlItem() {
			if(!isInDesignTime)
				CoerceValue(CaptionToControlDistanceProperty);
			CoerceValue(ActualCaptionMarginProperty);
			GotFocus += new RoutedEventHandler(Control_GotFocus);
		}
		protected virtual void OnControlChanged(UIElement control, UIElement oldControl) {
			SetValue(HasControlPropertyKey, control != null);
			if(oldControl != null) {
				RemoveLogicalChild(oldControl);
				oldControl.ClearValue(DockLayoutManager.LayoutItemProperty);
			}
			if(control != null) {
				AddLogicalChild(control);
				control.SetValue(DockLayoutManager.LayoutItemProperty, this);
			}
		}
		protected override Size CalcMinSizeValue(Size value) {
			if(DevExpress.Xpf.Core.Native.SizeHelper.IsZero(value)) {
				if(HasDesiredSize && DesiredSizeHelper.CanUseDesiredSizeAsMinSize(Control))
					return MathHelper.MeasureMinSize(new Size[] { DesiredSizeInternal, value });
			}
			return base.CalcMinSizeValue(value);
		}
		void Control_GotFocus(object sender, RoutedEventArgs e) {
			if(Manager != null) {
				if(!IsActive)
					ActivateItemCore();
			}
		}
		protected virtual void ActivateItemCore() {
			if(!Manager.IsDisposing)
				Manager.LayoutController.Activate(this, false);
		}
		protected override double CoerceActualCaptionWidth(double value) {
			return HasDesiredCaptionWidth ? CaptionAlignHelper.GetActualCaptionWidth(this) : value;
		}
		protected override void OnCaptionAlignModeChanged(CaptionAlignMode oldValue, CaptionAlignMode value) {
			CoerceValue(ActualCaptionWidthProperty);
			CoerceValue(ActualCaptionMarginProperty);
			CaptionAlignHelper.UpdateAffectedItems(this, oldValue, value);
		}
		protected override void OnCaptionWidthChanged(double value) {
			CoerceValue(ActualCaptionWidthProperty);
			CoerceValue(ActualCaptionMarginProperty);
		}
		protected override void OnDesiredCaptionWidthChanged(double value) {
			base.OnDesiredCaptionWidthChanged(value);
			CaptionAlignHelper.UpdateAffectedItems(this, CaptionAlignMode);
		}
		protected override void OnParentChanged() {
			base.OnParentChanged();
			CoerceValue(ActualCaptionWidthProperty);
		}
		protected internal override void OnParentItemsChanged() {
			base.OnParentItemsChanged();
			CoerceValue(ActualCaptionWidthProperty);
		}
		Size desiredSizeCore;
		void SetDesiredSize(Size value) {
			if(desiredSizeCore == value) return;
			desiredSizeCore = value;
		}
		protected virtual object CoerceDesiredSize(Size value) {
			if(HasDesiredSize) return desiredSizeCore;
			return value;
		}
		protected virtual double CoerceCaptionToControlDistance(double value) {
			if(!double.IsNaN(value)) return value;
			switch(CaptionLocation) {
				case CaptionLocation.Top:
					return DockLayoutManagerParameters.CaptionToControlDistanceTop;
				case CaptionLocation.Right:
					return DockLayoutManagerParameters.CaptionToControlDistanceRight;
				case CaptionLocation.Bottom:
					return DockLayoutManagerParameters.CaptionToControlDistanceBottom;
				case CaptionLocation.Left:
				default:
					return DockLayoutManagerParameters.CaptionToControlDistanceLeft;
			}
		}
		protected virtual void OnCaptionToControlDistanceChanged(double value) {
			CoerceValue(ActualCaptionMarginProperty);
		}
		protected override void OnCaptionLocationChanged(CaptionLocation value) {
			CoerceValue(CaptionToControlDistanceProperty);
			CoerceValue(ActualCaptionMarginProperty);
			CoerceValue(ActualCaptionWidthProperty);
			ClearValue(HasDesiredSizePropertyKey);
		}
		protected virtual Thickness CoerceActualCaptionMargin(Thickness value) {
			if(!HasVisibleCaption) return new Thickness();
			if(!MathHelper.AreEqual(value, new Thickness(double.NaN))) return value;
			double captionToControlDistance = double.IsNaN(CaptionToControlDistance) ? 0 : CaptionToControlDistance;
			switch(CaptionLocation) {
				case CaptionLocation.Top:
					return new Thickness(0, 0, 0, captionToControlDistance);
				case CaptionLocation.Right:
					return new Thickness(captionToControlDistance, 0, 0, 0);
				case CaptionLocation.Bottom:
					return new Thickness(0, captionToControlDistance, 0, 0);
			}
			return new Thickness(0, 0, captionToControlDistance, 0);
		}
		protected override string CoerceCaptionFormat(string captionFormat) {
			if(!string.IsNullOrEmpty(captionFormat)) return captionFormat;
			return DockLayoutManagerParameters.LayoutControlItemCaptionFormat;
		}
		protected virtual void OnShowControlChanged(bool showControl) {
			ClearValue(DesiredSizeInternalProperty);
		}
		protected override void OnShowCaptionChanged(bool value) {
			base.OnShowCaptionChanged(value);
			ClearValue(DesiredSizeInternalProperty);
		}
		protected virtual void OnDesiredSizeChanged(Size value) {
			SetDesiredSize(value);
			SetValue(HasDesiredSizePropertyKey, !value.IsEmpty);
			CoerceValue(ActualMinSizeProperty);
		}
		protected virtual void ClearContent(object oldContent) {
			ClearValue(ControlPropertyKey);
			ClearValue(IsDataBoundPropertyKey);
		}
		protected virtual void CheckContent(object content) {
			UIElement uiElement = content as UIElement;
			if(uiElement != null) {
				SetValue(ControlPropertyKey, content);
				return;
			}
			SetValue(IsDataBoundPropertyKey, content != null);
		}
		protected override void OnContentChanged(object content, object oldContent) {
			base.OnContentChanged(content, oldContent);
			ClearContent(oldContent);
			CheckContent(content);
		}
		protected override void OnIsDataBoundChanged(bool value) {
			base.OnIsDataBoundChanged(value);
			if(value) SetValue(ControlPropertyKey, GetDataBoundContainer());
		}
		protected override void OnCaptionChanged(object oldValue, object newValue) {
			base.OnCaptionChanged(oldValue, newValue);
			UpdateHasVisibleCaptionProperty();
		}
		protected override void OnCaptionTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			base.OnCaptionTemplateChanged(oldValue, newValue);
			UpdateHasVisibleCaptionProperty();
		}
		protected override void OnCaptionTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) {
			base.OnCaptionTemplateSelectorChanged(oldValue, newValue);
			UpdateHasVisibleCaptionProperty();
		}
		protected override void OnCaptionImageChanged(System.Windows.Media.ImageSource value) {
			base.OnCaptionImageChanged(value);
			UpdateHasVisibleCaptionProperty();
		}
		void UpdateHasVisibleCaptionProperty() {
			bool hasVisibleCaption = Caption != null || CaptionTemplate != null || CaptionTemplateSelector != null || CaptionImage != null;
			SetValue(HasVisibleCaptionProperty, hasVisibleCaption);
		}
		protected virtual void OnHasVisibleCaptionChanged(bool newValue) {
			CoerceValue(ActualCaptionMarginProperty);
		}
		protected override LayoutItemType GetLayoutItemTypeCore() {
			return LayoutItemType.ControlItem;
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return new Bars.SingleLogicalChildEnumerator(Control); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutControlItemControl"),
#endif
 Category("Control")]
		public UIElement Control {
			get { return (UIElement)GetValue(ControlProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutControlItemShowControl"),
#endif
		XtraSerializableProperty, Category("Control")]
		public bool ShowControl {
			get { return (bool)GetValue(ShowControlProperty); }
			set { SetValue(ShowControlProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutControlItemHasControl")]
#endif
		public bool HasControl {
			get { return (bool)GetValue(HasControlProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutControlItemCaptionToControlDistance"),
#endif
		XtraSerializableProperty]
		public double CaptionToControlDistance {
			get { return (double)GetValue(CaptionToControlDistanceProperty); }
			set { SetValue(CaptionToControlDistanceProperty, value); }
		}
		internal Size DesiredSizeInternal {
			get { return (Size)GetValue(DesiredSizeInternalProperty); }
			set { SetValue(DesiredSizeInternalProperty, value); }
		}
		internal bool HasDesiredSize {
			get { return (bool)GetValue(HasDesiredSizeProperty); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Thickness ActualCaptionMargin {
			get { return (Thickness)GetValue(ActualCaptionMarginProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutControlItemControlHorizontalAlignment"),
#endif
 XtraSerializableProperty, Category("Control")]
		public HorizontalAlignment ControlHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(ControlHorizontalAlignmentProperty); }
			set { SetValue(ControlHorizontalAlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutControlItemControlVerticalAlignment"),
#endif
 XtraSerializableProperty, Category("Control")]
		public VerticalAlignment ControlVerticalAlignment {
			get { return (VerticalAlignment)GetValue(ControlVerticalAlignmentProperty); }
			set { SetValue(ControlVerticalAlignmentProperty, value); }
		}
		internal bool HasVisibleCaption {
			get { return (bool)GetValue(HasVisibleCaptionProperty); }
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return DockLayoutManager.GetUIScope(this); } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion IUIElement
	}
}
