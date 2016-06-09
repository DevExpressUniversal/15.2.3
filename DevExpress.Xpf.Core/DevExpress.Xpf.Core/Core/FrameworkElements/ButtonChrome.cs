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

using DevExpress.Xpf.Editors;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
namespace DevExpress.Xpf.Core.Native {
	#region ChromeBase
	[ContentProperty("Child")]
	public class ChromeBase2 : Panel {
		public static readonly DependencyProperty RenderInfoProperty;
		public static readonly DependencyProperty TemplateProviderProperty;
		public static readonly DependencyProperty ContentMarginProperty;
		public static readonly DependencyProperty ForegroundInfoProperty;
		public static readonly DependencyProperty ChildProperty;
		static ChromeBase2() {
			RenderInfoProperty = DependencyPropertyManager.Register("RenderInfo", typeof(RenderInfo), typeof(ChromeBase2), new FrameworkPropertyMetadata(null, (d, _) => ((ChromeBase2)d).OnRenderInfoChanged()));
			TemplateProviderProperty = DependencyPropertyManager.Register("TemplateProvider", typeof(ChromeStateProviderBase), typeof(ChromeBase2), new FrameworkPropertyMetadata(null, (d, e) => ((ChromeBase2)d).OnTemplateProviderChanged((ChromeStateProviderBase)e.OldValue)));
			ContentMarginProperty = DependencyPropertyManager.Register("ContentMargin", typeof(Thickness), typeof(ChromeBase2), new FrameworkPropertyMetadata(null));
			ForegroundInfoProperty = DependencyPropertyManager.Register("ForegroundInfo", typeof(ForegroundInfo), typeof(ChromeBase2), new FrameworkPropertyMetadata(null, (d, _) => ((ChromeBase2)d).OnForegroundInfoChanged()));
			ChildProperty = DependencyPropertyManager.Register("Child", typeof(UIElement), typeof(ChromeBase2), new FrameworkPropertyMetadata(null, (d, e) => ((ChromeBase2)d).OnChildChanged((UIElement)e.OldValue, (UIElement)e.NewValue)));
		}
		String state;
		readonly RenderStrategy strategy;
		protected RenderStrategy Strategy { get { return strategy; } }
		public RenderInfo RenderInfo {
			get { return (RenderInfo)GetValue(RenderInfoProperty); }
			set { SetValue(RenderInfoProperty, value); }
		}
		public String State {
			get { return state; }
			set {
				String oldValue = state;
				state = value;
				OnStateChanged();
			}
		}
		double disabledStateOpacity;
		public double DisabledStateOpacity {
			get { return disabledStateOpacity; }
			set {
				disabledStateOpacity = value;
				UpdateDisabledStateOpacity();
			}
		}
		public ChromeStateProviderBase TemplateProvider {
			get { return (ChromeStateProviderBase)GetValue(TemplateProviderProperty); }
			set { SetValue(TemplateProviderProperty, value); }
		}
		public Thickness ContentMargin {
			get { return (Thickness)GetValue(ContentMarginProperty); }
			set { SetValue(ContentMarginProperty, value); }
		}
		public ForegroundInfo ForegroundInfo {
			get { return (ForegroundInfo)GetValue(ForegroundInfoProperty); }
			set { SetValue(ForegroundInfoProperty, value); }
		}
		public UIElement Child {
			get { return (UIElement)GetValue(ChildProperty); }
			set { SetValue(ChildProperty, value); }
		}
		public ChromeBase2() {
			strategy = new RenderStrategy();
			Strategy.RequestRender += (_o_, _O_) => InvalidateVisual();
			Strategy.StateChanged += (_, e) => State = e.NewValue;
			IsEnabledChanged += OnIsEnabledChanged;
		}
		void OnRenderInfoChanged() {
			Strategy.RenderInfo = RenderInfo;
		}
		void OnStateChanged() {
			if(ForegroundInfo != null)
				ForegroundInfo.Apply(this, TemplateProvider.GetActualForegroundStateName(Strategy.GetParentStates()));
			UpdateDisabledStateOpacity();
			InvalidateVisual();
		}
		protected override void OnRender(DrawingContext dc) {
			Strategy.Render(dc);
		}
		protected virtual void OnTemplateProviderChanged(ChromeStateProviderBase oldValue) {
			Strategy.StateProvider = TemplateProvider;
		}
		void OnForegroundInfoChanged() { }
		protected virtual void UpdateDisabledStateOpacity() {
			if(!IsEnabled) {
				Opacity = DisabledStateOpacity;
			} else {
				Opacity = 1;
			}
		}
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UpdateDisabledStateOpacity();
		}
		protected override void OnVisualParentChanged(DependencyObject oldParent) {
			base.OnVisualParentChanged(oldParent);
			UpdateTemplateProviderSource();
		}
		protected virtual void UpdateTemplateProviderSource() {
			Strategy.StateSource = TemplatedParent as FrameworkElement;
		}
		protected void OnChildChanged(UIElement oldChild, UIElement newChild) {
			if(oldChild != null)
				Children.Remove(oldChild);
			if(newChild != null) {
				AddNewChild(newChild);
			}
		}
		protected virtual void AddNewChild(UIElement newChild) {
			Children.Add(newChild);
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size sz = new Size();
			if(Child != null) {
				double availableHeight = Math.Max(availableSize.Height - this.ContentMargin.Top - this.ContentMargin.Bottom, 0);
				double availableWidth = Math.Max(availableSize.Width - this.ContentMargin.Left - this.ContentMargin.Right, 0);
				Child.Measure(new Size(availableWidth, availableHeight));
				sz.Height = Math.Max(sz.Height, Child.DesiredSize.Height);
				sz.Width = Math.Max(sz.Width, Child.DesiredSize.Width);
			}
			sz.Height += ContentMargin.Top + ContentMargin.Bottom;
			sz.Width += ContentMargin.Left + ContentMargin.Right;
			return sz;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(Child != null) {
				double finalHeight = Math.Max(finalSize.Height - this.ContentMargin.Top - this.ContentMargin.Bottom, 0);
				double finalWidth = Math.Max(finalSize.Width - this.ContentMargin.Left - this.ContentMargin.Right, 0);
				Size arrangeSize = new Size(finalWidth, finalHeight);
				Child.Arrange(new Rect(new Point(ContentMargin.Left, ContentMargin.Top), arrangeSize));
			}
			Strategy.Bounds = new Rect(new Point(0, 0), finalSize);
			return finalSize;
		}
	} 
	#endregion
	#region ButtonChrome2
	public class ButtonChrome2 : ChromeBase2 {
		public ButtonChrome2() {
		}
		protected override void AddNewChild(UIElement newChild) {
			base.AddNewChild(newChild);
			Canvas.SetZIndex(newChild, 2); 
		}
	} 
	#endregion
	#region CheckEditChrome
	public class CheckEditChrome : ButtonChrome2 {
		public static readonly DependencyProperty CheckSizeProperty;
		static CheckEditChrome() {
			CheckSizeProperty = DependencyPropertyManager.Register("CheckSize", typeof(Size), typeof(CheckEditChrome), new FrameworkPropertyMetadata(new Size()));
		}
		private double checkDisabledStateOpacity;
		public bool HasContent {
			get { return (TemplateProvider as CheckEditChromeStateProvider).With(x => x.Source as CheckEditBox).With(x => x.Content).ReturnSuccess(); }
		}
		public Size CheckSize {
			get { return (Size)GetValue(CheckSizeProperty); }
			set { SetValue(CheckSizeProperty, value); }
		}
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
			HitTestResult result = base.HitTestCore(hitTestParameters);
			return result == null && IsHitTestVisible ? new PointHitTestResult(this, hitTestParameters.HitPoint) : result;
		}		
		public virtual void UpdateContentPresenterProperties() {
			if(Child != null && !HasContent) {
				Child = null;
			}
			if(Child == null && HasContent) {
				Child = new ContentPresenter() { RecognizesAccessKey = true };
			}
			ContentPresenter contentPresenter = (ContentPresenter)Child;
			if(contentPresenter == null)
				return;
			var checkBox = TemplateProvider == null ? null : TemplateProvider.Source as CheckEditBox;
			if(checkBox != null) {
				contentPresenter.HorizontalAlignment = checkBox.HorizontalContentAlignment;
				contentPresenter.VerticalAlignment = checkBox.VerticalContentAlignment;
				contentPresenter.Margin = checkBox.Padding;
				contentPresenter.SnapsToDevicePixels = checkBox.SnapsToDevicePixels;
				contentPresenter.Content = checkBox.Content;
				contentPresenter.ContentTemplate = checkBox.ContentTemplate;
			}
		}
		public double CheckDisabledStateOpacity {
			get { return checkDisabledStateOpacity; }
			set { checkDisabledStateOpacity = value; }
		}
		protected override void OnTemplateProviderChanged(ChromeStateProviderBase oldValue) {
			base.OnTemplateProviderChanged(oldValue);
			var oldProvider = oldValue as CheckEditChromeStateProvider;
			if(oldProvider != null) {
				oldProvider.CheckEditChrome = null;
			}
			var newProvider = TemplateProvider as CheckEditChromeStateProvider;
			if(newProvider != null) {
				newProvider.CheckEditChrome = this;
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size sz = new Size();
			if(Child != null) {
				double availableHeight = Math.Max(availableSize.Height - this.ContentMargin.Top - this.ContentMargin.Bottom, 0);
				double availableWidth = Math.Max(availableSize.Width - this.ContentMargin.Left - this.ContentMargin.Right, 0);
				Child.Measure(new Size(availableWidth, availableHeight));
				sz.Height = Math.Max(Child.DesiredSize.Height, this.CheckSize.Height);
				sz.Width = Child.DesiredSize.Width + this.CheckSize.Width;
			} else {
				sz.Height = Math.Max(sz.Height, this.CheckSize.Height);
				sz.Width = Math.Max(sz.Width, this.CheckSize.Width);
			}
			sz.Height += ContentMargin.Top + ContentMargin.Bottom;
			sz.Width += ContentMargin.Left + ContentMargin.Right;
			return sz;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(Child != null) {
				double finalHeight = Math.Max(finalSize.Height - this.ContentMargin.Top - this.ContentMargin.Bottom, CheckSize.Height);
				double finalWidth = Math.Max(finalSize.Width - this.ContentMargin.Left - this.ContentMargin.Right, CheckSize.Width);
				Size arrangeSize = new Size(finalWidth, finalHeight);
				Child.Arrange(new Rect(new Point(ContentMargin.Left + this.CheckSize.Width, ContentMargin.Top), arrangeSize));
				Strategy.Bounds = new Rect(new Point(0, (Math.Ceiling(arrangeSize.Height / 2d) - Math.Ceiling(CheckSize.Height / 2d))), this.CheckSize);
			} else {
				Strategy.Bounds = new Rect(new Point(0, (Math.Floor(finalSize.Height / 2d) - Math.Floor(CheckSize.Height / 2d))), CheckSize);
			}
			return finalSize;
		}
	} 
	#endregion
	#region RenderStrategy 
	public class RenderStrategy {
		String state;
		RenderInfo renderInfo;
		IEnumerable<IRenderInfo> currentRenderInfos = Enumerable.Empty<IRenderInfo>();
		ChromeStateProviderBase stateProvider;
		FrameworkElement stateSource;
		Rect bounds;
		public event EventHandler RequestRender;
		public event ValueChangedEventHandler<string> StateChanged;
		public String State {
			get { return state; }
			set {
				String oldValue = state;
				state = value;
				OnStateChanged(oldValue);
			}
		}
		public RenderInfo RenderInfo {
			get { return renderInfo; }
			set {
				renderInfo = value;
				RaiseRequestRender();
			}
		}
		public ChromeStateProviderBase StateProvider {
			get { return stateProvider; }
			set {
				if(value == stateProvider)
					return;
				ChromeStateProviderBase oldValue = stateProvider;
				stateProvider = value;
				OnStateProviderChanged(oldValue);
			}
		}
		public FrameworkElement StateSource {
			get { return stateSource; }
			set {
				if(value == stateSource)
					return;
				FrameworkElement oldValue = stateSource;
				stateSource = value;
				OnStateSourceChanged(oldValue);
			}
		}
		public Rect Bounds {
			get { return bounds; }
			set {
				if(value == bounds)
					return;
				Rect oldValue = bounds;
				bounds = value;
				OnBoundsChanged(oldValue);
			}
		}
		protected virtual void OnBoundsChanged(Rect oldValue) {
			RaiseRequestRender();
		}
		protected virtual void OnStateChanged(String oldValue) {
			if(State == null) {
				currentRenderInfos = Enumerable.Empty<IRenderInfo>();
				return;
			}
			var states = State.Split(',');
			currentRenderInfos = states.Select(x => GetRenderInfo(x)).OrderBy(x => x.ZIndex);
			if(StateChanged != null) {
				StateChanged(this, new ValueChangedEventArgs<string>(oldValue, State));
			}
			RaiseRequestRender();
		}
		protected virtual void OnStateProviderChanged(ChromeStateProviderBase oldValue) {
			if(oldValue != null) {
				oldValue.Source = null;
				oldValue.Target = null;
			}
			if(StateProvider != null) {
				StateProvider.Source = StateSource;
				StateProvider.Target = this;
			}
			RaiseRequestRender();
		}
		protected virtual void OnStateSourceChanged(FrameworkElement oldValue) {
			InitializeStateProviderSource();
			RaiseRequestRender();
		}
		protected virtual IRenderInfo GetRenderInfo(string state) {
			IRenderInfo rInfo = RenderInfo[state];
			while(state != null && (rInfo is NullRenderInfo)) {
				state = StateProvider.GetBaseState(state, State.Split(','));
				rInfo = RenderInfo[state];
			}
			return rInfo;
		}
		public void Render(DrawingContext dc) {
			Render(dc, Bounds);
		}
		public void Render(DrawingContext dc, Rect bounds) {
			if(RenderInfo == null)
				return;
			foreach(var info in currentRenderInfos)
				info.Render(dc, bounds);
		}
		protected void InitializeStateProviderSource() {
			if(StateProvider != null)
				StateProvider.Source = StateSource;
		}
		protected void RaiseRequestRender() {
			if(RequestRender == null)
				return;
			RequestRender(this, EventArgs.Empty);
		}
		protected internal IEnumerable<string> GetParentStates() {
			var states = State.Split(',');
			for(int i = 0; i < states.Count(); i++) {
				if(!RenderInfo.Contains(states[i]))
					states[i] = StateProvider.GetBaseState(states[i], states);
			}
			return states.Distinct();
		}
	} 
	#endregion
	#region ChromeStateProviderBase
	public abstract class ChromeStateProviderBase {
		FrameworkElement source;
		RenderStrategy target;
		string state;
		public FrameworkElement Source {
			get { return source; }
			set {
				if(value == source)
					return;
				FrameworkElement oldValue = source;
				source = value;
				OnSourceChanged(oldValue);
			}
		}
		public string State {
			get { return state; }
			set {
				string oldValue = state;
				state = value;
				OnStateChanged(oldValue);
			}
		}
		public RenderStrategy Target {
			get {
				return target;
			}
			set {
				if(value == target)
					return;
				RenderStrategy oldValue = target;
				target = value;
				OnTargetChanged(oldValue);
			}
		}
		void OnTargetChanged(RenderStrategy oldValue) {
			UpdateTarget();
		}
		void OnStateChanged(string oldValue) {
			UpdateTarget();
		}
		void OnSourceChanged(FrameworkElement oldValue) {
			if(oldValue != null)
				Unsubscribe(oldValue);
			if(Source != null) {
				Unsubscribe(Source);
				Subscribe(Source);
				ChangeState();
			}
		}
		void UpdateTarget() {
			if(Target != null)
				Target.State = State;
		}
		protected void ChangeState() {
			this.State = GetState() ?? "";
		}
		protected abstract string GetState();
		public abstract string GetActualForegroundStateName(IEnumerable<string> availableStates);
		public abstract string GetBaseState(string desiredState, IEnumerable<string> allStates);
		protected abstract void Subscribe(FrameworkElement source);
		protected abstract void Unsubscribe(FrameworkElement source);
	} 
	#endregion
	#region ButtonChrome2StateProvider
	public class ButtonChrome2StateProvider : ChromeStateProviderBase {
		public static readonly string DisabledStateName = "Disabled";
		public static readonly string NormalStateName = "Normal";
		public static readonly string MouseOverStateName = "MouseOver";
		public static readonly string PressedStateName = "Pressed";
		public static readonly string CheckedStateName = "Checked";
		public static readonly string UncheckedStateName = "Unchecked";
		public static readonly string IndeterminateStateName = "Indeterminate";
		public static readonly string FocusedStateName = "FocusedState";
		static readonly List<string> StringPriorities = new List<string>() {
			CheckedStateName, IndeterminateStateName, UncheckedStateName, NormalStateName, MouseOverStateName, PressedStateName, DisabledStateName, FocusedStateName
		};
		static ButtonChrome2StateProvider() {
			EventManager.RegisterClassHandler(typeof(ButtonChrome2), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnButtonChrome2MouseDown), true);
		}
		static void OnButtonChrome2MouseDown(object sender, MouseButtonEventArgs e) {
			ButtonChrome2 chrome = sender as ButtonChrome2;
			if(chrome == null || chrome.TemplateProvider as ButtonChrome2StateProvider == null)
				return;
			((ButtonChrome2StateProvider)chrome.TemplateProvider).OnSourceMouseDown(sender, e);
		}
		public override string GetActualForegroundStateName(IEnumerable<string> availableStates) {
			if(!availableStates.Any())
				return null;
			return availableStates.OrderBy(StringPriorities.IndexOf).FirstOrDefault();
		}
		public override string GetBaseState(string desiredState, IEnumerable<string> allStates) {
			var commonStates = new string[] { NormalStateName, PressedStateName, DisabledStateName, MouseOverStateName }.AsEnumerable();
			var toggleStates = new string[] { CheckedStateName, UncheckedStateName, IndeterminateStateName }.AsEnumerable();
			if(!commonStates.Contains(desiredState) && allStates.Intersect(commonStates).Any())
				return null;
			if(toggleStates.Contains(desiredState))
				return GetCommonState();
			if(desiredState == DisabledStateName)
				return NormalStateName;
			return null;
		}
		protected override void Subscribe(FrameworkElement source) {
			source.MouseEnter += OnSourceMouseEnter;
			source.MouseLeave += OnSourceMouseLeave;
			source.GotFocus += OnSourceGotFocus;
			source.GotKeyboardFocus += OnSourceGotKeyboardFocus;
			source.LostFocus += OnSourceLostFocus;
			source.LostKeyboardFocus += OnSourceLostKeyboardFocus;
			source.KeyDown += OnSourceKeyDown;
			source.KeyUp += OnSourceKeyUp;
			source.GotMouseCapture += OnCapture;
			source.LostMouseCapture += OnLostCapture;
			source.IsEnabledChanged += OnSourceIsEnabledChanged;
			if(source is ToggleButton) {
				((ToggleButton)source).Checked += OnSourceChecked;
				((ToggleButton)source).Unchecked += OnSourceUnchecked;
			}
			source.MouseMove += OnSourceMouseMove;
		}
		protected override void Unsubscribe(FrameworkElement source) {
			source.MouseEnter -= OnSourceMouseEnter;
			source.MouseLeave -= OnSourceMouseLeave;
			source.GotFocus -= OnSourceGotFocus;
			source.GotKeyboardFocus -= OnSourceGotKeyboardFocus;
			source.LostFocus -= OnSourceLostFocus;
			source.LostKeyboardFocus -= OnSourceLostKeyboardFocus;
			source.KeyDown -= OnSourceKeyDown;
			source.KeyUp -= OnSourceKeyUp;
			source.GotMouseCapture -= OnCapture;
			source.LostMouseCapture -= OnLostCapture;
			source.IsEnabledChanged -= OnSourceIsEnabledChanged;
			if(source is ToggleButton) {
				((ToggleButton)source).Checked -= OnSourceChecked;
				((ToggleButton)source).Unchecked -= OnSourceUnchecked;
			}
			source.MouseMove -= OnSourceMouseMove;
		}
		protected override string GetState() {
			return CombineState(GetToggleState() ?? GetCommonState(), GetFocusedState());
		}
		protected virtual string GetFocusedState() {
			return Source.If(x => x.IsKeyboardFocused).ReturnSuccess() ? FocusedStateName : null;
		}
		protected virtual string GetToggleState() {
			var tb = Source as ToggleButton;
			if(tb == null)
				return null;
			if(!(tb.IsChecked.HasValue))
				return IndeterminateStateName;
			if((bool)tb.IsChecked)
				return CheckedStateName;
			return UncheckedStateName;
		}
		protected virtual string GetCommonState() {
			if(Source == null)
				return null;
			if(!Source.IsEnabled)
				return DisabledStateName;
			if(Source is ButtonBase && ((ButtonBase)Source).IsPressed)
				return PressedStateName;
			if(Source.IsMouseOver)
				return MouseOverStateName;
			return NormalStateName;
		}
		protected string CombineState(string current, string additional) {
			if(string.IsNullOrEmpty(current))
				return additional;
			if(string.IsNullOrEmpty(additional))
				return current;
			return string.Format("{0},{1}", current, additional);
		}
		protected virtual void OnSourceGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			ChangeState();
		}
		protected virtual void OnSourceLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			ChangeState();
		}
		protected virtual void OnSourceUnchecked(object sender, RoutedEventArgs e) {
			ChangeState();
		}
		protected virtual void OnSourceChecked(object sender, RoutedEventArgs e) {
			ChangeState();
		}
		protected virtual void OnSourceIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(Source != null)
				ChangeState();
		}
		protected virtual void OnLostCapture(object sender, MouseEventArgs e) {
			ChangeState();
		}
		protected virtual void OnCapture(object sender, MouseEventArgs e) {
			ChangeState();
		}
		protected virtual void OnSourceKeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
			ChangeState();
		}
		protected virtual void OnSourceKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
			ChangeState();
		}
		protected virtual void OnSourceGotFocus(object sender, RoutedEventArgs e) {
			ChangeState();
		}
		protected virtual void OnSourceLostFocus(object sender, RoutedEventArgs e) {
			if(Source != null)
				ChangeState();
		}
		protected virtual void OnSourceMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			Source.Dispatcher.BeginInvoke(new Action(ChangeState));
		}
		protected virtual void OnSourceMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			Source.Dispatcher.BeginInvoke(new Action(ChangeState));
		}
		protected virtual void OnSourceMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
			ChangeState();
		}
		protected virtual void OnSourceMouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
			ChangeState();
		}
		protected virtual void OnSourceMouseMove(object sender, MouseEventArgs e) {
			ChangeState();
		}
	} 
	#endregion
	#region CheckEditChromeStateProvider
	public class CheckEditChromeStateProvider : ButtonChrome2StateProvider {
		private CheckEditChrome checkEditChrome;
		public CheckEditChrome CheckEditChrome {
			get { return checkEditChrome; }
			set {
				if(value == checkEditChrome)
					return;
				CheckEditChrome oldValue = checkEditChrome;
				checkEditChrome = value;
				UpdateChrome();
			}
		}
		protected virtual void OnCheckEditChromeChanged(CheckEditChrome oldValue) {
		}
		protected CheckEditBox CheckEditBox { get { return Source as CheckEditBox; } }
		protected override string GetState() {
			return CombineState(CombineState(GetToggleState(), GetCommonState()), GetFocusedState());
		}
		protected override void Subscribe(FrameworkElement source) {
			base.Subscribe(source);
			var checkBox = source as CheckEditBox;
			if(checkBox == null)
				return;
			checkBox.IsCheckedChanged += OnIsCheckedChanged;
			checkBox.RequestUpdate += OnRequestUpdate;
			UpdateChrome();
		}
		protected override void Unsubscribe(FrameworkElement source) {
			base.Subscribe(source);
			var checkBox = source as CheckEditBox;
			if(checkBox == null)
				return;
			checkBox.IsCheckedChanged -= OnIsCheckedChanged;
			checkBox.RequestUpdate -= OnRequestUpdate;
		}
		protected virtual void OnIsCheckedChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ChangeState();
		}
		protected virtual void OnRequestUpdate(object sender, EventArgs e) {
			UpdateChrome();
		}
		void UpdateChrome() {
			CheckEditChrome.Do(x => x.UpdateContentPresenterProperties());
		}
	} 
	#endregion
	#region render info
	[ContentProperty("Elements")]
	public class RenderInfo {
		public Dictionary<string, IRenderInfo> Elements { get; set; }
		public RenderInfo() {
			Elements = new Dictionary<string, IRenderInfo>();
		}
		public IRenderInfo this[string state] {
			get {
				IRenderInfo value = null;
				if(state == null || !Elements.TryGetValue(state, out value))
					return NullRenderInfo.Instance;
				return value;
			}
		}
		public virtual bool Contains(string stateName) {
			return Elements.ContainsKey(stateName);
		}
	}
	[ContentProperty("Brush")]
	public class BrushInfo {
		public Brush Brush { get; set; }
	}
	[ContentProperty("Elements")]
	public class BrushSet {
		public Dictionary<string, BrushInfo> Elements { get; set; }
		public BrushSet() {
			Elements = new Dictionary<string, BrushInfo>();
		}
		public void ApplyForeground(DependencyObject target, string brushName) {
			BrushInfo value = null;
			if(string.IsNullOrEmpty(brushName) || !Elements.TryGetValue(brushName, out value))
				target.ClearValue(TextElement.ForegroundProperty);
			else {
				target.SetValue(TextElement.ForegroundProperty, value.With(x => x.Brush.GetCurrentValueAsFrozen()));
			}
		}
		public Brush GetBrush(string brushName) {
			BrushInfo value = null;
			if(Elements.TryGetValue(brushName, out value))
				return value.Brush;
			return null;
		}
	}
	[ContentProperty("Elements")]
	public class ForegroundInfo {
		public Dictionary<string, Brush> Elements { get; set; }
		public ForegroundInfo() {
			Elements = new Dictionary<string, Brush>();
		}
		public void Apply(DependencyObject target, string stateName) {
			Brush value = null;
			if(stateName == null || !Elements.TryGetValue(stateName, out value))
				target.ClearValue(TextElement.ForegroundProperty);
			else {
				target.SetValue(TextElement.ForegroundProperty, value.With(x => x.GetCurrentValueAsFrozen()));
			}
		}
		public virtual bool Contains(string stateName) {
			return Elements.ContainsKey(stateName);
		}
	}
	sealed class NullRenderInfo : BaseRenderInfo {
		static NullRenderInfo instance;
		public static NullRenderInfo Instance {
			get {
				if(instance == null)
					instance = new NullRenderInfo();
				return instance;
			}
		}
		NullRenderInfo() { }
		protected override void RenderOverride(DrawingContext dc, Rect bounds) { }
	}
	[ContentProperty("Children")]
	public class RenderInfoGroup : BaseRenderInfo {
		public List<IRenderInfo> Children { get; set; }
		public RenderInfoGroup() {
			Children = new List<IRenderInfo>();
		}
		protected override void RenderOverride(DrawingContext dc, Rect bounds) {
			IEnumerable<IRenderInfo> sortedChildren = Children.OrderBy(x => x.ZIndex);
			foreach(var child in sortedChildren) {
				child.Render(dc, bounds);
			}
		}
	}
	public class PathInfo : BaseRenderInfo {
		double thickness;
		Brush fill;
		string data;
		Brush stroke;
		Geometry frozenGeometry = null;
		public Brush Stroke {
			get { return stroke; }
			set {
				stroke = value.With(x => x.GetCurrentValueAsFrozen() as Brush);
				UpdatePen();
			}
		}
		public string Data {
			get { return data; }
			set {
				data = value;
				frozenGeometry = data == null ? null : Geometry.Parse(data);
			}
		}
		public Brush Fill {
			get { return fill; }
			set { fill = value.With(x => x.GetCurrentValueAsFrozen() as Brush); }
		}
		public double Thickness {
			get { return thickness; }
			set {
				if(value == thickness)
					return;
				double oldValue = thickness;
				thickness = value;
				UpdatePen();
			}
		}
		public Pen Pen { get; private set; }
		public PathInfo() {
			Thickness = 0;
			UpdatePen();
		}
		void UpdatePen() {
			Pen = new Pen(Stroke, Thickness);
			Pen.Freeze();
		}
		protected override void RenderOverride(DrawingContext dc, Rect bounds) {
			if(frozenGeometry == null)
				return;
			var geometry = frozenGeometry.CloneCurrentValue();
			var geometryBounds = geometry.Bounds;
			var xScale = Math.Max(bounds.Width - Thickness, 0d);
			var yScale = Math.Max(bounds.Height - Thickness, 0d);
			var dX = bounds.Left + Thickness / 2d - geometryBounds.Left;
			var dY = bounds.Top + Thickness / 2d - geometryBounds.Top;
			xScale = (geometryBounds.Width > xScale * Double.Epsilon) ? (xScale / geometryBounds.Width) : 1d;
			yScale = (geometryBounds.Height > yScale * Double.Epsilon) ? (yScale / geometryBounds.Height) : 1d;
			var stretchMatrix = Matrix.Identity;
			stretchMatrix.ScaleAt(xScale, yScale, geometryBounds.Location.X, geometryBounds.Location.Y);
			stretchMatrix.Translate(dX, dY);
			geometry.Transform = new MatrixTransform(stretchMatrix);
			dc.DrawGeometry(Fill, Pen, geometry);
		}
	}
	public class RectangleInfo : BaseRenderInfo {
		Brush background;
		Brush borderBrush;
		double thickness;
		public Brush Background {
			get { return background; }
			set { background = value.With(x => x.GetCurrentValueAsFrozen()) as Brush; }
		}
		public Brush BorderBrush {
			get { return borderBrush; }
			set {
				if(value == borderBrush)
					return;
				Brush oldValue = borderBrush;
				borderBrush = value.With(x => x.GetCurrentValueAsFrozen()) as Brush;
				OnBorderBrushChanged(oldValue);
			}
		}
		public double RadiusX { get; set; }
		public double RadiusY { get; set; }
		public double Thickness {
			get { return thickness; }
			set {
				if(value == thickness)
					return;
				double oldValue = thickness;
				thickness = value;
				OnThicknessChanged(oldValue);
			}
		}
		public Pen Pen { get; private set; }
		public RectangleInfo() { }
		protected virtual void OnThicknessChanged(double oldValue) {
			UpdeteBorderPen();
		}
		protected virtual void OnBorderBrushChanged(Brush oldValue) {
			UpdeteBorderPen();
		}
		void UpdeteBorderPen() {
			Pen = new Pen(BorderBrush, Thickness);
			Pen.Freeze();
		}
		protected override void RenderOverride(DrawingContext dc, Rect bounds) {
			bounds.X += 0.5 * Thickness;
			bounds.Y += 0.5 * Thickness;
			bounds.Width -= Math.Min(bounds.Width, Thickness);
			bounds.Height -= Math.Min(bounds.Height, Thickness);
			dc.DrawRoundedRectangle(Background, Pen, bounds, RadiusX, RadiusY);
		}
	}
	static class RenderExtension {
		public static void UpdateBounds(this IRenderInfo obj, ref Rect bounds) {
			bounds.Height = Math.Max(bounds.Height - (obj.Margin.Bottom + obj.Margin.Top), 0);
			bounds.Width = Math.Max(bounds.Width - (obj.Margin.Left + obj.Margin.Right), 0);
			bounds.X += obj.Margin.Left;
			bounds.Y += obj.Margin.Top;
			var width = double.IsNaN(obj.Width) ? bounds.Width * obj.RelativeWidth : obj.Width;
			var height = double.IsNaN(obj.Height) ? bounds.Height * obj.RelativeHeight : obj.Height;
			double x = bounds.Left;
			double y = bounds.Top;
			switch(obj.HorizontalAlignment) {
				case HorizontalAlignment.Center:
					x = bounds.Left + (bounds.Width - width) / 2;
					break;
				case HorizontalAlignment.Right:
					x = bounds.Right - width;
					break;
			}
			switch(obj.VerticalAlignment) {
				case VerticalAlignment.Bottom:
					y = bounds.Bottom - height;
					break;
				case VerticalAlignment.Center:
					y = bounds.Top + (bounds.Height - height) / 2;
					break;
			}
			bounds.X = x;
			bounds.Y = y;
			bounds.Height = height;
			bounds.Width = width;
		}
	}
	public interface IRenderInfo {
		double Width { get; }
		double Height { get; }
		Thickness Margin { get; }
		double RelativeHeight { get; }
		double RelativeWidth { get; }
		int ZIndex { get; }
		HorizontalAlignment HorizontalAlignment { get; }
		VerticalAlignment VerticalAlignment { get; }
		void Render(DrawingContext dc, Rect bounds);
	}
	public abstract class BaseRenderInfo : IRenderInfo {
		public Thickness Margin { get; set; }
		public double RelativeHeight { get; set; }
		public double RelativeWidth { get; set; }
		public double Height { get; set; }
		public double Width { get; set; }
		public HorizontalAlignment HorizontalAlignment { get; set; }
		public VerticalAlignment VerticalAlignment { get; set; }
		public int ZIndex { get; set; }
		public BaseRenderInfo() {
			Width = double.NaN;
			Height = double.NaN;
			Margin = new Thickness();
			RelativeHeight = 1d;
			RelativeWidth = 1d;
			ZIndex = 0;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}
		public void Render(DrawingContext dc, Rect bounds) {
			this.UpdateBounds(ref bounds);
			RenderOverride(dc, bounds);
		}
		protected abstract void RenderOverride(DrawingContext dc, Rect bounds);
	} 
	#endregion
}
