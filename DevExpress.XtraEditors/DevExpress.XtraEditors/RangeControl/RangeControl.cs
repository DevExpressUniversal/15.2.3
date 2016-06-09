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
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Gesture;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using System.Drawing.Drawing2D;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Regular),
	Designer("DevExpress.XtraEditors.Design.RangeControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	ToolboxTabName(AssemblyInfo.DXTabCommon), 
	SmartTagAction(typeof(RangeControlActions), "AddNumericClient", "Add Numeric Client"),
	SmartTagAction(typeof(RangeControlActions), "AddChartNumericClient", "Add Chart Numeric Client"),
	SmartTagAction(typeof(RangeControlActions), "AddChartDateTimeClient", "Add Chart Date-Time Client"),
	SmartTagFilter(typeof(RangeControlFilter)),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "RangeControl")]
	public class RangeControl : BaseStyleControl, ISupportXtraAnimation, IRangeControl, IMouseWheelSupport, ISupportInitialize {
		internal readonly object AnimationId = new object();
		private static readonly object rangeChanged = new object();
		public RangeControl() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.Selectable, true);
			OverriddenRange.InternalSetMinimum(-1.0);
			OverriddenRange.InternalSetMaximum(-1.0);
			this.normalizedRange = new RangeControlNormalizedRange() { Minimum = 0.0, Maximum = 0.0 };
			this.selectedRange = new RangeControlRange();
			this.normalizedRange.Owner = this;
			this.selectedRange.Owner = this;
			this.AllowAnimation = true;
			IsAnimating = false;
			this.allowSelection = true;
			this.showZoomScrollBar = true;
			this.showLabels = true;
			this.AllowPanMode = true;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsAnimating { get; internal set; }
		protected override Size DefaultSize {
			get {
				return new Size(420, 90);
			}
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new RangeControlViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new RangeControlPainter();
		}
		RangeControlHandler handler;
		protected RangeControlHandler Handler {
			get {
				if(handler == null)
					handler = CreateHandler();
				return handler;
			}
		}
		protected virtual RangeControlHandler CreateHandler() {
			return new RangeControlHandler(this);
		}
		protected internal List<object> Ruler { get; set; }
		protected internal bool IsClientValid { get { return Client == null || Client.IsValid; } }
		#region mouse and keyboard events
		bool AllowInteraction { get { return IsClientValid && Enabled && !IsDesignMode; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(!AllowInteraction)
				return;
			Handler.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!AllowInteraction)
				return;
			Handler.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(!AllowInteraction)
				return;
			Handler.OnMouseUp(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs e) {
			base.OnMouseWheel(e);
			if(!AllowInteraction)
				return;
			Handler.OnMouseWheel(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			if(XtraForm.ProcessSmartMouseWheel(this, e)) return;
			OnMouseWheelCore(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			if(!AllowInteraction)
				return;
			Handler.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(!AllowInteraction)
				return;
			Handler.OnMouseLeave(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(!AllowInteraction)
				return;
			Handler.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(!AllowInteraction)
				return;
			Handler.OnKeyUp(e);
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if(base.ProcessCmdKey(ref msg, keyData))
				return true;
			if(!AllowInteraction)
				return false;
			return Handler.ProcessCmdKey(ref msg, keyData);
		}
		#endregion
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			Handler.GestureHelper.TogglePressAndHold(false);
		}
		protected override void WndProc(ref Message msg) {
			if(AllowInteraction && Handler.GestureHelper.WndProc(ref msg))
				return;
			base.WndProc(ref msg);
		}
		public event RangeChangedEventHandler RangeChanged {
			add { Events.AddHandler(rangeChanged, value); }
			remove { Events.RemoveHandler(rangeChanged, value); }
		}
		bool ShouldRaiseRangeChanged { get; set; }
		protected virtual void RaiseRangeChanged() {
			if(IsInitializing) {
				ShouldRaiseRangeChanged = true;
				return;
			}
			RangeChangedEventHandler handler = Events[rangeChanged] as RangeChangedEventHandler;
			if(handler != null)
				handler(this, new RangeControlRangeEventArgs() { Range = new RangeControlRange(SelectedRange.Minimum, SelectedRange.Maximum) });
		}
		RangeControlRange selectedRange;
		bool ShouldSerializeSelectedRange() { return SelectedRange.ShouldSerialize(); }
		void ResetSelectedRange() { SelectedRange.Reset(); }
		public void SelectAll() {
			VisibleRangeScaleFactor = 1;
			VisibleRangeStartPosition = 0;
			UpdateRange(0, 1);
		}
		[Browsable(false)]
		public RangeControlRange SelectedRange {
			get { return selectedRange; }
			set {
				if(value == null)
					value = new RangeControlRange();
				if(SelectedRange == value)
					return;
				if(AnimateOnDataChange && Client != null) {
					SelectRange(Client.GetNormalizedValue(value.Minimum), Client.GetNormalizedValue(value.Maximum));
					return;
				}
				selectedRange.Owner = null;
				selectedRange = value;
				selectedRange.Owner = this;
				OnSelectedRangeChanged();
			}
		}
		protected virtual void OnSelectedRangeChanged() {
			InRangeValueChanged = true;
			try {
				if(Client != null) {
					double newMin = Client.GetNormalizedValue(SelectedRange.Minimum);
					double newMax = Client.GetNormalizedValue(SelectedRange.Maximum);
					if(newMin > NormalizedSelectedRange.Maximum) {
						NormalizedSelectedRange.Maximum = newMax;
						NormalizedSelectedRange.Minimum = newMin;
					}
					else {
						NormalizedSelectedRange.Minimum = newMin;
						NormalizedSelectedRange.Maximum = newMax;
					}
				} else OnRangeChanged(false);
			} finally {
				InRangeValueChanged = false;
			}
			RaiseRangeChanged();
			RaiseClientRangeChanged();
		}
		internal bool IsValidValue(object value) {
			if(Client == null || value == null)
				return true;
			return Client.IsValidType(value.GetType());
		}
		bool IRangeControl.IsValidValue(object value) {
			return IsValidValue(value);
		}
		protected virtual void OnRangeMinimumChanged() {
			InRangeValueChanged = true;
			try {
				if(Client != null)
					NormalizedSelectedRange.Minimum = Client.GetNormalizedValue(SelectedRange.Minimum);
				else OnRangeChanged(false);
			} finally {
				InRangeValueChanged = false;
			}
			RaiseRangeChanged();
			RaiseClientRangeChanged();
		}
		int RulerWidthInPixels { 
			get {
				if(RangeViewInfo == null)
					return 0;
				return RangeViewInfo.GetRectWidth(RangeViewInfo.ScrollBounds); 
			} 
		}
		protected virtual void OnRangeMaximumChanged() {
			InRangeValueChanged = true;
			try {
				if(Client != null)
					NormalizedSelectedRange.Maximum = Client.GetNormalizedValue(SelectedRange.Maximum);
				else
					OnRangeChanged(false);
			} finally {
				InRangeValueChanged = false;
			}
			RaiseRangeChanged();
			RaiseClientRangeChanged();
		}
		bool InRangeValueChanged { get; set; }
		[DefaultValue(false), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RangeControlAnimateOnDataChange"),
#endif
 DXCategory(CategoryName.Behavior)]
		public bool AnimateOnDataChange {
			get;
			set;
		}
		RangeControlNormalizedRange normalizedRange;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RangeControlNormalizedRange NormalizedSelectedRange {
			get { return normalizedRange; }
			set {
				if(value == null)
					value = new RangeControlNormalizedRange();
				if(NormalizedSelectedRange == value)
					return;
				value = ConstrainNormalizedRange(value);
				normalizedRange.Owner = null;
				normalizedRange = value;
				normalizedRange.Owner = this;
				OnRangeChanged(false);
			}
		}
		protected virtual RangeControlNormalizedRange ConstrainNormalizedRange(RangeControlNormalizedRange range) {
			range.Minimum = ConstrainRangeMinimum(range.Minimum);
			range.Maximum = ConstrainRangeMaximum(range.Maximum);
			return range;
		}
		double ConstrainRangeMaximumCore(double value) {
			value = Math.Max(NormalizedSelectedRange.Minimum, value);
			value = Math.Min(1.0, value);
			return value;
		}
		double IRangeControl.ConstrainRangeMaximum(double value) {
			return ConstrainRangeMaximum(value);
		}
		protected internal virtual double ConstrainRangeMaximum(double value) {
			value = ConstrainRangeMaximumCore(value);
			if(Client != null) {
				NormalizedRangeInfo info = new NormalizedRangeInfo(NormalizedSelectedRange.Minimum, value, RangeControlValidationType.Range, ChangedBoundType.Maximum);
				Client.ValidateRange(info);
				value = info.Range.Maximum;
			}
			value = ConstrainRangeMaximumCore(value);
			return value;
		}
		protected internal virtual void OnRangeChanged(bool invalidateContentImage) {
			if(IsDisposing)
				return;
			RangeViewInfo.IsContentImageReady &= !invalidateContentImage;
			if(IsLayoutLocked || !IsHandleCreated) {
				ViewInfo.IsReady = false;
				return;
			}
			ViewInfo.CalcViewInfo(null, Control.MouseButtons, PointToClient(Control.MousePosition), ClientRectangle);
			UpdateRuler();
			Invalidate();
		}
		protected virtual void RaiseClientRangeChanged() {
			if(Client != null) {
				if(!InClientRangeChanged)
					Client.OnRangeChanged(SelectedRange.Minimum, SelectedRange.Maximum);
			}
		}
		bool InUpdateRealRange { get; set; }
		protected virtual void UpdateRealRange() {
			if(InRangeValueChanged || InUpdateRealRange)
				return;
			try {
				InUpdateRealRange = true;
				if(Client != null) {
					SelectedRange.InternalSetMinimum(Client.GetValue(NormalizedSelectedRange.Minimum));
					SelectedRange.InternalSetMaximum(Client.GetValue(NormalizedSelectedRange.Maximum));
				}
				OnRangeChanged(false);
				RaiseRangeChanged();
				RaiseClientRangeChanged();
			} finally {
				InUpdateRealRange = false;
			}
		}
		protected internal virtual void UpdateRuler() {
			Ruler = null;
			if(Client != null)
				Ruler = Client.GetRuler(new RulerInfoArgs(Client.GetValue(VisibleRangeStartPosition), Client.GetValue(VisibleRangeStartPosition + VisibleRangeWidth), RulerWidthInPixels));
		}
		double ConstrainRangeMinimumCore(double value) {
			value = Math.Min(NormalizedSelectedRange.Maximum, value);
			value = Math.Max(0, value);
			return value;
		}
		double IRangeControl.ConstrainRangeMinimum(double value) {
			return ConstrainRangeMinimum(value);
		}
		protected internal virtual double ConstrainRangeMinimum(double value) {
			value = ConstrainRangeMinimumCore(value);
			if(Client != null) {
				NormalizedRangeInfo info = new NormalizedRangeInfo(value, NormalizedSelectedRange.Maximum, RangeControlValidationType.Range, ChangedBoundType.Minimum);
				Client.ValidateRange(info);
				value = info.Range.Minimum;
			}
			value = ConstrainRangeMinimumCore(value);
			return value;
		}
		Orientation orientation;
		[DefaultValue(Orientation.Horizontal), SmartTagProperty("Orientation", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public Orientation Orientation {
			get { return orientation; }
			set {
				if(Client != null && !Client.SupportOrientation(value))
					return;
				if(Orientation == value)
					return;
				orientation = value;
				OnOrientationChanged();
			}
		}
		[DefaultValue(true)]
		public bool AllowAnimation {
			get;
			set;
		}
		protected virtual void OnOrientationChanged() {
			LayoutChanged();
		}
		RangeControlSelectionType selectionType;
		[DefaultValue(RangeControlSelectionType.Thumb), SmartTagProperty("Selection Type", "")]
		public RangeControlSelectionType SelectionType {
			get { return selectionType; }
			set {
				if(selectionType == value)
					return;
				selectionType = value;
				LayoutChanged();
			}
		}
		double maximumScale = 10.0;
		[DefaultValue(10.0)]
		public double VisibleRangeMaximumScaleFactor {
			get { return maximumScale; }
			set {
				value = Math.Max(1.0, value);
				if(VisibleRangeMaximumScaleFactor == value)
					return;
				maximumScale = value;
				OnMaximumScaleChanged();
			}
		}
		protected virtual void OnMaximumScaleChanged() {
			VisibleRangeScaleFactor = Math.Min(VisibleRangeScaleFactor, VisibleRangeMaximumScaleFactor);
		}
		double scale = 1.0;
		[DefaultValue(1.0)]
		public double VisibleRangeScaleFactor {
			get { return scale; }
			set {
				value = ConstraintScale(value);
				value = Math.Min(value, VisibleRangeMaximumScaleFactor);
				if(VisibleRangeScaleFactor == value)
					return;
				scale = value;
				OnScaleChanged();
			}
		}
		protected virtual void OnScaleChanged() {
			RangeViewInfo.IsContentImageReady = false;
			LayoutChanged();
		}
		protected virtual double ConstraintScale(double value) {
			return Math.Max(1.0, value);
		}
		IRangeControlClient client;
		[DefaultValue(null), SmartTagProperty("Client", "")]
		public IRangeControlClient Client {
			get { return client; }
			set {
				if(Client == value)
					return;
				OnClientChanging();
				client = value;
				OnClientChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		RefreshProperties(RefreshProperties.All),
		 TypeConverter(typeof(RangeControlClientPropertiesTypeConverter))]
		public object ClientOptions {
			get {
				if(Client != null)
					return Client.GetOptions();
				return null;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void CenterRangeInViewPort(bool animated) {
			if (animated && IsHandleCreated) {
				BeginInvoke(new MethodInvoker(CenterViewPortAnimated));
			} else {
				double delta = (VisibleRangeWidth - (NormalizedSelectedRange.Maximum - NormalizedSelectedRange.Minimum)) / 2;
				double newVal = ConstrainViewportPosition(NormalizedSelectedRange.Minimum - delta);
				this.visibleRangeStartPosition = newVal;
			}
		}
		bool allowSelection;
		[DefaultValue(true)]
		public bool AllowSelection {
			get { return allowSelection; }
			set {
				if(AllowSelection == value)
					return;
				allowSelection = value;
				LayoutChanged();
			}
		}
		[DefaultValue(true)]
		public bool AllowPanMode {
			get;
			set;
		}
		bool showZoomScrollBar;
		[DefaultValue(true), SmartTagProperty("Show Zoom&ScrollBar", "")]
		public bool ShowZoomScrollBar {
			get { return showZoomScrollBar; }
			set{
				if(ShowZoomScrollBar == value)
					return;
				showZoomScrollBar = value;
				LayoutChanged();
			}
		}
		bool showLabels;
		[DefaultValue(true), SmartTagProperty("Show Labels", "")]
		public bool ShowLabels {
			get { return showLabels; }
			set {
				if(showLabels == value)
					return;
				showLabels = value;
				LayoutChanged();
			}
		}
		void CenterViewPortAnimated() {
			double delta = (VisibleRangeWidth - (NormalizedSelectedRange.Maximum - NormalizedSelectedRange.Minimum)) / 2;
			double newVal = ConstrainViewportPosition(NormalizedSelectedRange.Minimum - delta);
			RangeViewInfo.AddViewPortAnimation(newVal);
		}
		void StopAnimation() {
			XtraAnimator.Current.Animations.Remove(this, AnimationId);
		}
		bool InClientRangeChanged { get; set; }
		protected virtual void OnClientRangeChanged(object sender, RangeControlClientRangeEventArgs e) {
			if(AnimateOnDataChange && IsHandleCreated && XtraAnimator.Current.Get(this, AnimationId) == null) {
				SelectRange(Client.GetNormalizedValue(e.Range.Minimum), Client.GetNormalizedValue(e.Range.Maximum));
				return;
			}
			SelectedRange.InternalSetMinimum(e.Range.Minimum);
			SelectedRange.InternalSetMaximum(e.Range.Maximum);
			this.normalizedRange.InternalSetMinimum(Client.GetNormalizedValue(e.Range.Minimum));
			this.normalizedRange.InternalSetMaximum(Client.GetNormalizedValue(e.Range.Maximum));
			StopAnimation();
			if(e.MakeRangeVisible) {
				CenterRangeInViewPort(e.AnimatedViewport);
			}
			InClientRangeChanged = true;
			try {
				OnRangeChanged(e.InvalidateContent || e.MakeRangeVisible);
			} finally {
				InClientRangeChanged = false;
			}
			RaiseRangeChanged();
		}
		protected virtual void SubscribeClientEvents() {
			if(Client != null) {
				Client.RangeChanged -= new ClientRangeChangedEventHandler(OnClientRangeChanged);
				Client.RangeChanged += new ClientRangeChangedEventHandler(OnClientRangeChanged);
			}
		}
		protected virtual void OnClientChanging() {
			if(Client != null) {
				Client.OnRangeControlChanged(null);
				Client.RangeChanged -= new ClientRangeChangedEventHandler(OnClientRangeChanged);
			}
		}
		void SetClientInternal(IRangeControlClient client) {
			this.client = client;
			SubscribeClientEvents();
			LayoutChanged();
		}
		protected internal override void LayoutChanged() {
			RangeViewInfo.IsContentImageReady = false;
			OnRangeChanged(true);
			base.LayoutChanged();
		}
		protected virtual void OnClientChanged() {
			if(Client != null) {
				 if(!Client.SupportOrientation(Orientation))
					Orientation = Client.SupportOrientation(Orientation.Horizontal) ? Orientation.Horizontal : Orientation.Vertical;
				 Client.OnRangeControlChanged(this);
			}
			SubscribeClientEvents();
			LayoutChanged();
			FireChanged();
		}
		double visibleRangeStartPosition;
		[DefaultValue(0.0)]
		public double VisibleRangeStartPosition {
			get { return visibleRangeStartPosition; }
			set {
				value = ConstrainViewportPosition(value);
				if(VisibleRangeStartPosition == value)
					return;
				visibleRangeStartPosition = value;
				OnViewportPositionChanged();
			}
		}
		[Browsable(false)]
		public double VisibleRangeWidth { 
			get { return 1.0 / VisibleRangeScaleFactor; } 
		}
		[Browsable(false)]
		public double VisibleRangeMinWidth { 
			get { return 1.0 / VisibleRangeMaximumScaleFactor; } 
		}
		protected virtual void OnViewportPositionChanged() {
			RangeViewInfo.IsContentImageReady = false;
			LayoutChanged();
		}
		protected internal RangeControlViewInfo RangeViewInfo { get { return (RangeControlViewInfo)ViewInfo; } }
		protected virtual double ConstrainViewportPosition(double value) {
			value = Math.Max(0.0, value);
			if(value + VisibleRangeWidth > 1.0)
				value = 1.0 - VisibleRangeWidth;
			return value;
		}
		RangeControlNormalizedRange overriddenRange = new RangeControlNormalizedRange() { Minimum = -1.0, Maximum = -1.0 };
		internal RangeControlNormalizedRange OverriddenRange { get { return overriddenRange; } }
		protected internal virtual void UpdateRangeMinimum(Point point, bool useOverride) {
			if(useOverride) {
				OverriddenRange.Minimum = ConstrainRangeMinimumCore(RangeViewInfo.Point2Value(point));
				OnRangeChanged(false);
			} else {
				bool shouldUpdate = OverriddenRange.Minimum != -1.0;
				OverriddenRange.Minimum = -1.0;
				NormalizedSelectedRange.Minimum = RangeViewInfo.Point2Value(point);
				if(shouldUpdate) {
					UpdateRealRange();
					OnRangeChanged(false);
				}
			}
		}
		protected internal virtual void UpdateRangeMaximum(Point point, bool useOverride) {
			if(useOverride) {
				OverriddenRange.Maximum = ConstrainRangeMaximumCore(RangeViewInfo.Point2Value(point));
				OnRangeChanged(false);
			} else {
				bool shouldUpdate = OverriddenRange.Maximum != -1.0;
				OverriddenRange.Maximum = -1.0;
				NormalizedSelectedRange.Maximum = RangeViewInfo.Point2Value(point);
				if(shouldUpdate) {
					UpdateRealRange();
					OnRangeChanged(false);
				}
			}
		}
		protected internal virtual void UpdateRangePositon(Point point, bool isPreviewPoint, bool useOverride) {
			if(useOverride) {
				if(OverriddenRange.Maximum < 0.0)
					OverriddenRange.Maximum = NormalizedSelectedRange.Maximum;
				if(OverriddenRange.Minimum < 0.0)
					OverriddenRange.Minimum = NormalizedSelectedRange.Minimum;
			}
			double delta = useOverride ? OverriddenRange.Maximum - OverriddenRange.Minimum : NormalizedSelectedRange.Maximum - NormalizedSelectedRange.Minimum;
			double value = isPreviewPoint? RangeViewInfo.PreviewPoint2Value(point): RangeViewInfo.Point2Value(point);
			double position = Math.Min(1.0 - delta, Math.Max(0.0f, value));
			NormalizedRangeInfo info = new NormalizedRangeInfo(position, position + delta, RangeControlValidationType.Range, ChangedBoundType.Both);
			if(Client != null && !useOverride)
				Client.ValidateRange(info);
			position = Math.Min(1.0 - delta, Math.Max(0.0f, value));
			if(useOverride) {
				OverriddenRange.Minimum = info.Range.Minimum;
				OverriddenRange.Maximum = info.Range.Maximum;
			} else {
				this.normalizedRange.InternalSetMinimum(info.Range.Minimum);
				this.normalizedRange.InternalSetMaximum(info.Range.Maximum);
				OverriddenRange.Maximum = -1.0;
				OverriddenRange.Minimum = -1.0;
				UpdateRealRange();
			}
			OnRangeChanged(false);
		}
		protected internal virtual void UpdateViewPortPosition(Point point) {
			UpdateViewPortPosition(point, false);
		}
		protected internal virtual void UpdateViewPortPosition(Point point, bool animated) {
			double newValue = 0.0;
			if(Orientation == System.Windows.Forms.Orientation.Horizontal)
				newValue = Math.Max(0.0, Math.Min(1.0 - VisibleRangeWidth, (double)(point.X - RangeViewInfo.ScrollBarAreaBounds.X) / (RangeViewInfo.ScrollBarAreaBounds.Width)));
			else
				newValue = Math.Max(0.0, Math.Min(1.0 - VisibleRangeWidth, (double)(point.Y - RangeViewInfo.ScrollBarAreaBounds.Y) / (RangeViewInfo.ScrollBarAreaBounds.Height)));
			UpdateViewPortPosition(newValue, animated);
		}
		protected internal virtual void UpdateViewPortPosition(double newValue, bool animated) {
			newValue = Math.Max(0, Math.Min(newValue, 1.0 - VisibleRangeWidth));
			if(!animated)
				VisibleRangeStartPosition = newValue;
			else
				RangeViewInfo.AddViewPortAnimation(newValue);
		}
		protected internal virtual NormalizedRangeInfo GetValidatedInfo(double newMin, double newMax) {
			return GetValidatedInfo(newMin, newMax, RangeControlValidationType.Range);
		}
		protected internal virtual NormalizedRangeInfo GetValidatedInfo(double newMin, double newMax, RangeControlValidationType type) {
			double min = Math.Min(newMin, newMax);
			double max = Math.Max(newMin, newMax);
			max = Math.Min(1, Math.Max(0, max));
			min = Math.Min(1, Math.Max(0, min));
			NormalizedRangeInfo info = new NormalizedRangeInfo(min, max, type, ChangedBoundType.Both);
			if(Client != null)
				Client.ValidateRange(info);
			return info;
		}
		protected internal virtual void UpdateRange(double newMin, double newMax) {
			NormalizedRangeInfo info = GetValidatedInfo(newMin, newMax);
			this.normalizedRange.InternalSetMinimum(info.Range.Minimum);
			this.normalizedRange.InternalSetMaximum(info.Range.Maximum);
			UpdateRealRange();
			OnRangeChanged(false);
		}
		protected internal virtual void SelectRange(double selStart, double selEnd) {
			NormalizedRangeInfo info = GetValidatedInfo(selStart, selEnd);
			RangeViewInfo.AddRangeAnimation(info.Range.Minimum, info.Range.Maximum);
		}
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		#endregion
		internal virtual void UpdatLayout() {
			LayoutChanged();
		}
		protected internal virtual void IneternalSetViewPortPosition(double newVal) {
			this.visibleRangeStartPosition = newVal;
			RangeViewInfo.CalcRectsCore();
		}
		protected internal virtual void UpdateViewPortStart(Point point) {
			double newPosition = Math.Max(0, RangeViewInfo.PreviewPoint2Value(point));
			double delta = VisibleRangeStartPosition + VisibleRangeWidth - newPosition;
			if(delta == 0.0)
				return;
			double newScale = 1.0 / delta;
			if(newScale < 1.0)
				return;
			if (Client != null)
				newScale = Client.ValidateScale(newScale);
			delta = 1.0 / newScale;
			newPosition = Math.Min(newPosition, 1.0 - delta);
			if(newScale > VisibleRangeMaximumScaleFactor) {
				newScale = VisibleRangeMaximumScaleFactor;
				newPosition = VisibleRangeStartPosition + VisibleRangeWidth - 1.0 / VisibleRangeMaximumScaleFactor;
			}
			this.scale = newScale;
			this.visibleRangeStartPosition = newPosition;
			LayoutChanged();
		}
		protected internal virtual void UpdateViewPortEnd(Point point) {
			double newPosition = Math.Min(1.0, Math.Max(0, RangeViewInfo.PreviewPoint2Value(point)));
			double delta = newPosition - VisibleRangeStartPosition;
			if(delta == 0.0)
				return;
			double newScale = 1.0 / delta;
			if(newScale < 1.0) 
				return;
			if (Client != null)
				newScale = Client.ValidateScale(newScale);
			if(newScale > VisibleRangeMaximumScaleFactor) {
				newScale = VisibleRangeMaximumScaleFactor;
				newPosition = VisibleRangeStartPosition + 1.0 / VisibleRangeMaximumScaleFactor;
			}
			newScale = Math.Max(1.0, newScale);
			this.scale = newScale;
			LayoutChanged();
		}
		protected internal virtual void LayoutChangedCore() {
			LayoutChanged();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if (Client != null) {
				VisibleRangeScaleFactor = Client.ValidateScale(this.scale);
				Client.OnResize();
				LayoutChanged();
			}
		}
		#region IRangeControl Members
		int IRangeControl.CalcX(double normalizedValue) {
			Rectangle rect = RangeViewInfo.ScrollBounds;
			if(Orientation == System.Windows.Forms.Orientation.Vertical)
				rect = RangeViewInfo.Vertical2Horizontal(RangeViewInfo.ScrollBounds);
			return rect.X + (int)((normalizedValue - VisibleRangeStartPosition) / VisibleRangeWidth * rect.Width);
		}
		Matrix IRangeControl.NormalTransform {
			get {
				Matrix result = new Matrix();
				Rectangle rect = RangeViewInfo.ScrollBounds;
				Matrix scale = new Matrix();
				scale.Scale(1.0f / (float)VisibleRangeWidth, 1.0f);
				result.Multiply(scale);
				Matrix translate = new Matrix();
				translate.Translate(-(float)VisibleRangeStartPosition, 0.0f);
				result.Multiply(translate);
				return result;
			}
		}
		#endregion
		#region IRangeControl Members
		Color IRangeControl.BorderColor {
			get { return RangeViewInfo.BorderColor; }
		}
		Color IRangeControl.RulerColor {
			get { return RangeViewInfo.RulerColor; }
		}
		Color IRangeControl.LabelColor {
			get { return RangeViewInfo.LabelColor; }
		}
		void IRangeControl.CenterSelectedRange() {
			CenterRangeInViewPort(false);
		}
		#endregion
		void IRangeControl.OnRangeMinimumChanged(object range) {
			OnRangeMinimumChanged(range);
		}
		protected internal virtual void OnRangeMinimumChanged(object range) {
			if(range == SelectedRange) {
				OnRangeMinimumChanged();
			} else if(range == NormalizedSelectedRange) {
				OnRangeChanged(false);
			}
		}
		void IRangeControl.OnRangeMaximumChanged(object range) {
			OnRangeMaximumChanged(range);
		}
		protected internal virtual void OnRangeMaximumChanged(object range) {
			if(range == SelectedRange) {
				OnRangeMaximumChanged();
			} else if(range == NormalizedSelectedRange) {
				OnRangeChanged(false);
			}
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected bool IsInitializing { get; private set; }
		void ISupportInitialize.BeginInit() {
			IsInitializing = true;
		}
		void ISupportInitialize.EndInit() {
			IsInitializing = false;
			if(ShouldRaiseRangeChanged)
				RaiseRangeChanged();
			ShouldRaiseRangeChanged = false;
		}
	}
	[ToolboxItem(false)]
	public class RangeControlClientBase : Component, IRangeControlClient {
		private static readonly object rangeChanged = new object();
		public RangeControlClientBase() {
			this.labelFormatString = DefaultFormatString;
			this.flagFormatString = DefaultFormatString;
		}
		[Browsable(false)]
		public RangeControl RangeControl { get; set; }
		protected virtual string DefaultFormatString { get { return "{0}"; } }
		protected virtual void ResetLabelFormatString() { this.labelFormatString = DefaultFormatString; }
		protected virtual void ResetFlagFormatString() { this.flagFormatString = DefaultFormatString; }
		protected virtual bool ShouldSerializeLabelFormatString() { return LabelFormatString != DefaultFormatString; }
		protected virtual bool ShouldSerializeFlagFormatString() { return FlagFormatString != DefaultFormatString; }
		string labelFormatString;
		public string LabelFormatString {
			get { return labelFormatString; }
			set {
				if(LabelFormatString == value)
					return;
				labelFormatString = value;
				OnFormatStringChanged();
			}
		}
		protected virtual void OnFormatStringChanged() {
			if(RangeControl != null)
				RangeControl.LayoutChangedCore();
		}
		string flagFormatString;
		public string FlagFormatString {
			get { return flagFormatString; }
			set {
				if(FlagFormatString == value)
					return;
				flagFormatString = value;
				OnFormatStringChanged();
			}
		}
		#region IRangeControlClient Members
		bool IRangeControlClient.IsValidType(Type type) {
			return IsValidTypeCore(type);
		}
		protected virtual bool IsValidTypeCore(Type type) {
			return false;
		}
		protected virtual void OnClickCore(RangeControlHitInfo hitInfo) { 
		}
		void IRangeControlClient.OnClick(RangeControlHitInfo hitInfo) {
			OnClickCore(hitInfo);
		}
		protected virtual void UpdateHotInfoCore(RangeControlHitInfo hitInfo) {
		}
		void IRangeControlClient.UpdateHotInfo(RangeControlHitInfo hitInfo) {
			UpdateHotInfoCore(hitInfo);
		}
		protected virtual void UpdatePressedInfoCore(RangeControlHitInfo hitInfo) {
		}
		void IRangeControlClient.UpdatePressedInfo(RangeControlHitInfo hitInfo) {
			UpdatePressedInfoCore(hitInfo);
		}
		protected virtual double ValidateScaleCore(double newScale) { return newScale; }
		double IRangeControlClient.ValidateScale(double newScale) { return ValidateScaleCore(newScale); }
		void IRangeControlClient.OnResize() {
			OnResizeCore();
		}
		protected virtual void OnResizeCore() { }
		protected virtual string InvalidTextCore { get { return "This data type is not supported by RangeControl."; } }
		string IRangeControlClient.InvalidText { get { return InvalidTextCore; } }
		protected virtual bool IsValidCore { get { return true; } }
		bool IRangeControlClient.IsValid { get { return IsValidCore; } }
		protected virtual object GetOptionsCore() { return this; }
		object IRangeControlClient.GetOptions() { return GetOptionsCore(); }
		event ClientRangeChangedEventHandler IRangeControlClient.RangeChanged {
			add { Events.AddHandler(rangeChanged, value); }
			remove { Events.RemoveHandler(rangeChanged, value); }
		}
		protected virtual void OnRangeChangedCore(object rangeMinimum, object rangeMaximum) { 
		}
		void IRangeControlClient.OnRangeChanged(object rangeMinimum, object rangeMaximum) {
			OnRangeChangedCore(rangeMinimum, rangeMaximum);
		}
		bool IRangeControlClient.SupportOrientation(Orientation orientation) { 
			return SupportOrientationCore(orientation);
		}
		protected virtual bool SupportOrientationCore(Orientation orientation) {
			return true;
		}
		protected virtual bool DrawRulerCore(RangeControlPaintEventArgs e) {
			return false;
		}
		bool IRangeControlClient.DrawRuler(RangeControlPaintEventArgs e) {
			return DrawRulerCore(e);			
		}
		protected virtual void DrawContentCore(RangeControlPaintEventArgs e) { }
		void IRangeControlClient.DrawContent(RangeControlPaintEventArgs e) {
			DrawContentCore(e);
		}
		protected virtual int RangeBoxTopIndentCore { get { return 0; } }
		int IRangeControlClient.RangeBoxTopIndent {
			get { return RangeBoxTopIndentCore; }
		}
		protected virtual int RangeBoxBottomIndentCore { get { return 0; } }
		int IRangeControlClient.RangeBoxBottomIndent {
			get { return RangeBoxBottomIndentCore; }
		}
		protected virtual void ValidateRangeCore(NormalizedRangeInfo info) {
			object min = GetValueCore(info.Range.Minimum);
			object max = GetValueCore(info.Range.Maximum);
			info.Range.Minimum = GetNormalizedValueCore(min);
			info.Range.Maximum = GetNormalizedValueCore(max);
		}
		void IRangeControlClient.ValidateRange(NormalizedRangeInfo info) {
			ValidateRangeCore(info);
		}
		protected virtual double GetNormalizedValueCore(object value) {
			return 0.0;
		}
		double IRangeControlClient.GetNormalizedValue(object value) {
			return GetNormalizedValueCore(value);
		}
		List<object> IRangeControlClient.GetRuler(RulerInfoArgs e) {
			return null;
		}
		bool IRangeControlClient.IsCustomRuler { get { return false; } }
		object IRangeControlClient.RulerDelta {
			get { return null; }
		}
		protected virtual double NormalizedRulerDeltaCore { get { return 0.0; } }
		double IRangeControlClient.NormalizedRulerDelta {
			get {
				return NormalizedRulerDeltaCore;
			}
		}
		protected virtual string RuleToStringCore(int ruleIndex) { 
			return string.Empty; 
		}
		string IRangeControlClient.RulerToString(int ruleIndex) {
			return RuleToStringCore(ruleIndex);	
		}
		protected virtual object GetValueCore(double normalizedValue) {
			return normalizedValue;
		}
		object IRangeControlClient.GetValue(double normalizedValue) {
			return GetValueCore(normalizedValue);
		}
		void IRangeControlClient.OnRangeControlChanged(IRangeControl rangeControl) { }
		void IRangeControlClient.Calculate(Rectangle contentRect) { }
		string IRangeControlClient.ValueToString(double normalizedValue) {
			return ValueToStringCore(normalizedValue);
		}
		protected virtual string ValueToStringCore(double normalizedValue) {
			return string.Empty;
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class NumericRangeControlClient : RangeControlClientBase {
		object rulerDelta = 1;
		[DefaultValue(1),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object RulerDelta {
			get { return rulerDelta; }
			set {
				if(rulerDelta == value)
					return;
				rulerDelta = value;
				OnRulerDeltaChanged();
			}
		}
		protected virtual void OnRulerDeltaChanged() {
			if(RangeControl != null)
				RangeControl.LayoutChangedCore();
		}
		object minimum = 0;
		[DefaultValue(0),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Minimum {
			get { return minimum; }
			set {
				value = ConstrainMinimum(value);
				if(Minimum == value)
					return;
				minimum = value;
				OnMinimumChanged();
			}
		}
		object maximum = 10;
		[DefaultValue(10),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Maximum {
			get { return maximum; }
			set {
				value = ConstrainMaximum(value);
				if(Maximum == value)
					return;
				maximum = value;
				OnMaximumChanged();
			}
		}
		protected virtual object ConstrainMinimum(object value) {
			return value;
		}
		protected virtual void OnMinimumChanged() {
			if(RangeControl != null)
				RangeControl.LayoutChangedCore();
		}
		protected virtual object ConstrainMaximum(object value) {
			return value;
		}
		protected virtual void OnMaximumChanged() {
			if(RangeControl != null)
				RangeControl.LayoutChangedCore();
		}
		protected virtual object ChangeType(double value) {
			return Convert.ChangeType(value, Minimum.GetType());
		}
		protected override bool IsValidTypeCore(Type type) {
			return type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(Single) || type == typeof(float) || type == typeof(double) || type == typeof(Decimal);
		}
		protected override object GetValueCore(double normalizedValue) {
			double min = Convert.ToDouble(Minimum);
			double max = Convert.ToDouble(Maximum);
			double res = min + normalizedValue * (max - min);
			return ChangeType(res);
		}
		protected override double GetNormalizedValueCore(object value) {
			double min = Convert.ToDouble(Minimum);
			double max = Convert.ToDouble(Maximum);
			double val = Convert.ToDouble(value);
			if(min == max) return 1;
			return (val - min) / (max - min);
		}
		protected override string RuleToStringCore(int ruleIndex) {
			double ruleDelta = Convert.ToDouble(RulerDelta);
			double min = Convert.ToDouble(Minimum);
			double dval = min + ruleIndex * ruleDelta;
			object val = ChangeType(dval);
			return string.Format(LabelFormatString, val);
		}
		protected override double NormalizedRulerDeltaCore {
			get {
				double min = Convert.ToDouble(Minimum);
				double max = Convert.ToDouble(Maximum);
				double rule = Convert.ToDouble(RulerDelta);
				return Math.Abs(rule / (max - min));
			}
		}
		protected override void UpdatePressedInfoCore(RangeControlHitInfo hitInfo) {
			base.UpdatePressedInfoCore(hitInfo);
			UpdateHitInfo(hitInfo);
		}
		protected override void UpdateHotInfoCore(RangeControlHitInfo hitInfo) {
			base.UpdateHotInfoCore(hitInfo);
			UpdateHitInfo(hitInfo);
		}
		protected void UpdateHitInfo(RangeControlHitInfo hitInfo) {
		}
		protected override string ValueToStringCore(double normalizedValue) {
			return string.Format(FlagFormatString, GetValueCore(normalizedValue));
		}
	}
	public class RangeControlClientPropertiesTypeConverter : TypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) {
				return Convert.ToString(value);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(value.GetType(), attributes);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	#region date time
	#endregion
}
