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
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.LayoutControl {
	public enum GroupBoxButtonKind { Minimize, Unminimize, Maximize, Unmaximize }
	[TemplatePart(Name = MinimizeElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = UnminimizeElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = MaximizeElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = UnmaximizeElementName, Type = typeof(FrameworkElement))]
	public class GroupBoxButton : DXButton {
		private GroupBoxButtonKind _Kind;
		public GroupBoxButton() {
			DefaultStyleKey = typeof(GroupBoxButton);
		}
		public GroupBoxButtonKind Kind {
			get { return _Kind; }
			set {
				if(_Kind != value) {
					_Kind = value;
					UpdateTemplate();
				}
			}
		}
		#region Template
		const string MinimizeElementName = "MinimizeElement";
		const string UnminimizeElementName = "UnminimizeElement";
		const string MaximizeElementName = "MaximizeElement";
		const string UnmaximizeElementName = "UnmaximizeElement";
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			MinimizeElement = (FrameworkElement)GetTemplateChild(MinimizeElementName);
			UnminimizeElement = (FrameworkElement)GetTemplateChild(UnminimizeElementName);
			MaximizeElement = (FrameworkElement)GetTemplateChild(MaximizeElementName);
			UnmaximizeElement = (FrameworkElement)GetTemplateChild(UnmaximizeElementName);
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if(MinimizeElement != null)
				MinimizeElement.SetVisible(Kind == GroupBoxButtonKind.Minimize);
			if(UnminimizeElement != null)
				UnminimizeElement.SetVisible(Kind == GroupBoxButtonKind.Unminimize);
			if(MaximizeElement != null)
				MaximizeElement.SetVisible(Kind == GroupBoxButtonKind.Maximize);
			if(UnmaximizeElement != null)
				UnmaximizeElement.SetVisible(Kind == GroupBoxButtonKind.Unmaximize);
		}
		protected FrameworkElement MinimizeElement { get; set; }
		protected FrameworkElement UnminimizeElement { get; set; }
		protected FrameworkElement MaximizeElement { get; set; }
		protected FrameworkElement UnmaximizeElement { get; set; }
		#endregion Template
		public new GroupBoxButtonController Controller { get { return (GroupBoxButtonController)base.Controller; } }
		protected override ControlControllerBase CreateController() {
			return new GroupBoxButtonController(this);
		}
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.LayoutControl.UIAutomation.GroupBoxButtonAutomationPeer(this);
		}
		#endregion
	}
	public class GroupBoxButtonController : DXButtonController {
		public GroupBoxButtonController(IControl control) : base(control) {
		}
		internal void InvokeClick() {
			OnClick();
		}
	}
	public enum GroupBoxShadowVisibility { Never, WhenHasMouse, Always }
	public enum GroupBoxState { Normal, Minimized, Maximized }
	public enum GroupBoxDisplayMode { Default, Normal, Light }
	public interface IGroupBox : IControl {
		bool DesignTimeClick(DXMouseButtonEventArgs args);
		void UpdateShadowVisibility();
		Rect MinimizeElementBounds { get; }
	}
	[TemplatePart(Name = BorderElementName, Type = typeof(Border))]
	[TemplatePart(Name = MaximizeElementName, Type = typeof(GroupBoxButton))]
	[TemplatePart(Name = MinimizeElementName, Type = typeof(GroupBoxButton))]
	[TemplateVisualState(Name = NormalLayoutState, GroupName = LayoutStates)]
	[TemplateVisualState(Name = MinimizedLayoutState, GroupName = LayoutStates)]
	[TemplateVisualState(Name = MaximizedLayoutState, GroupName = LayoutStates)]
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNameNavigationAndLayout)]
#if !SILVERLIGHT
#endif
	public class GroupBox : MaximizableHeaderedContentControlBase, IGroupBox, IMaximizableElement {
		#region Dependency Properties
		private bool _IsChangingState;
		public static readonly DependencyProperty CornerRadiusProperty =
			DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GroupBox), null);
		public static readonly DependencyProperty MaximizeElementVisibilityProperty =
			DependencyProperty.Register("MaximizeElementVisibility", typeof(Visibility), typeof(GroupBox),
				new PropertyMetadata(Visibility.Collapsed));
		public static readonly DependencyProperty MinimizeElementVisibilityProperty =
			DependencyProperty.Register("MinimizeElementVisibility", typeof(Visibility), typeof(GroupBox),
				new PropertyMetadata(Visibility.Collapsed));
		public static readonly DependencyProperty MinimizationDirectionProperty =
			DependencyProperty.Register("MinimizationDirection", typeof(Orientation), typeof(GroupBox), null);
		public static readonly DependencyProperty SeparatorBrushProperty =
			DependencyProperty.Register("SeparatorBrush", typeof(Brush), typeof(GroupBox), null);
		public static readonly DependencyProperty ShadowOffsetProperty =
			DependencyProperty.Register("ShadowOffset", typeof(double), typeof(GroupBox), null);
		public static readonly DependencyProperty ShadowVisibilityProperty =
			DependencyProperty.Register("ShadowVisibility", typeof(Visibility), typeof(GroupBox), null);
		public static readonly DependencyProperty ShowShadowProperty =
			DependencyProperty.Register("ShowShadow", typeof(GroupBoxShadowVisibility), typeof(GroupBox),
				new PropertyMetadata((o, e) => ((GroupBox)o).UpdateShadowVisibility()));
		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(GroupBoxState), typeof(GroupBox),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (GroupBox)o;
						if (control._IsChangingState)
							return;
						control._IsChangingState = true;
						if (control.OnStateChanging((GroupBoxState)e.OldValue, (GroupBoxState)e.NewValue))
							control.OnStateChanged((GroupBoxState)e.OldValue, (GroupBoxState)e.NewValue);
						else
							o.SetValue(e.Property, e.OldValue);
						control._IsChangingState = false;
					}));
		public static readonly DependencyProperty TitleBackgroundProperty =
			DependencyProperty.Register("TitleBackground", typeof(Brush), typeof(GroupBox), null);
		public static readonly DependencyProperty TitleForegroundProperty =
			DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(GroupBox), null);
		public static readonly DependencyProperty TitleVisibilityProperty =
			DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(GroupBox), new PropertyMetadata(Visibility.Visible));
		public static readonly DependencyProperty NormalTemplateProperty =
			DependencyProperty.Register("NormalTemplate", typeof(ControlTemplate), typeof(GroupBox),
				new PropertyMetadata(null, (d, e) => ((GroupBox)d).UpdateCurrentTemplate()));
		public static readonly DependencyProperty LightTemplateProperty =
			DependencyProperty.Register("LightTemplate", typeof(ControlTemplate), typeof(GroupBox),
				new PropertyMetadata(null, (d, e) => ((GroupBox)d).UpdateCurrentTemplate()));
		public static readonly DependencyProperty DisplayModeProperty =
			DependencyProperty.Register("DisplayMode", typeof(GroupBoxDisplayMode), typeof(GroupBox),
				new PropertyMetadata(GroupBoxDisplayMode.Default, (d, e) => ((GroupBox)d).UpdateCurrentTemplate()));
		#endregion Dependency Properties
		private object _StoredMinSizePropertyValue;
		private object _StoredSizePropertyValue;
		public GroupBox() {
			DefaultStyleKey = typeof(GroupBox);
			UpdateShadowVisibility();
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxCornerRadius")]
#endif
		public CornerRadius CornerRadius {
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxMaximizeElementVisibility")]
#endif
		public Visibility MaximizeElementVisibility {
			get { return (Visibility)GetValue(MaximizeElementVisibilityProperty); }
			set { SetValue(MaximizeElementVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxMinimizationDirection")]
#endif
		public Orientation MinimizationDirection {
			get { return (Orientation)GetValue(MinimizationDirectionProperty); }
			set { SetValue(MinimizationDirectionProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxMinimizeElementVisibility")]
#endif
		public Visibility MinimizeElementVisibility {
			get { return (Visibility)GetValue(MinimizeElementVisibilityProperty); }
			set { SetValue(MinimizeElementVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxSeparatorBrush")]
#endif
		public Brush SeparatorBrush {
			get { return (Brush)GetValue(SeparatorBrushProperty); }
			set { SetValue(SeparatorBrushProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxShadowOffset")]
#endif
		public double ShadowOffset {
			get { return (double)GetValue(ShadowOffsetProperty); }
			set { SetValue(ShadowOffsetProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxShowShadow")]
#endif
		public GroupBoxShadowVisibility ShowShadow {
			get { return (GroupBoxShadowVisibility)GetValue(ShowShadowProperty); }
			set { SetValue(ShowShadowProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxState")]
#endif
		public GroupBoxState State {
			get { return (GroupBoxState)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxTitleBackground")]
#endif
		public Brush TitleBackground {
			get { return (Brush)GetValue(TitleBackgroundProperty); }
			set { SetValue(TitleBackgroundProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxTitleForeground")]
#endif
		public Brush TitleForeground {
			get { return (Brush)GetValue(TitleForegroundProperty); }
			set { SetValue(TitleForegroundProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("GroupBoxTitleVisibility")]
#endif
		public Visibility TitleVisibility {
			get { return (Visibility)GetValue(TitleVisibilityProperty); }
			set { SetValue(TitleVisibilityProperty, value); }
		}
		public ControlTemplate NormalTemplate {
			get { return (ControlTemplate)GetValue(NormalTemplateProperty); }
			set { SetValue(NormalTemplateProperty, value); }
		}
		public ControlTemplate LightTemplate {
			get { return (ControlTemplate)GetValue(LightTemplateProperty); }
			set { SetValue(LightTemplateProperty, value); }
		}
		public GroupBoxDisplayMode DisplayMode {
			get { return (GroupBoxDisplayMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		public event ValueChangedEventHandler<GroupBoxState> StateChanged;
		#region Template
		const string BorderElementName = "BorderElement";
		const string MaximizeElementName = "MaximizeElement";
		const string MinimizeElementName = "MinimizeElement";
		const string LayoutStates = "LayoutStates";
		const string NormalLayoutState = "NormalLayout";
		const string MinimizedLayoutState = "MinimizedLayout";
		const string MaximizedLayoutState = "MaximizedLayout";
		public override void OnApplyTemplate() {
			if (MaximizeElement != null)
				MaximizeElement.Click -= OnButtonClick;
			if (MinimizeElement != null)
				MinimizeElement.Click -= OnButtonClick;
			base.OnApplyTemplate();
			BorderElement = GetTemplateChild(BorderElementName) as Border;
			MaximizeElement = GetTemplateChild(MaximizeElementName) as GroupBoxButton;
			MinimizeElement = GetTemplateChild(MinimizeElementName) as GroupBoxButton;
			if (MaximizeElement != null)
				MaximizeElement.Click += OnButtonClick;
			if (MinimizeElement != null)
				MinimizeElement.Click += OnButtonClick;
			UpdateButtons();
		}
		protected string GetLayoutState(GroupBoxState state) {
			if (state == GroupBoxState.Normal)
				return NormalLayoutState;
			else
				if (state == GroupBoxState.Minimized)
					return MinimizedLayoutState;
				else
					return MaximizedLayoutState;
		}
		protected Border BorderElement { get; set; }
		protected GroupBoxButton MaximizeElement { get; set; }
		protected GroupBoxButton MinimizeElement { get; set; }
		private void OnButtonClick(object sender, RoutedEventArgs e) {
			OnButtonClick((GroupBoxButton)sender);
		}
		#endregion Template
		#region XML Storage
		protected internal virtual void ReadCustomizablePropertiesFromXML(XmlReader xml) {
			this.ReadPropertyFromXML(xml, StateProperty, "State", typeof(GroupBoxState));
		}
		protected internal virtual void WriteCustomizablePropertiesToXML(XmlWriter xml) {
			this.WritePropertyToXML(xml, StateProperty, "State");
		}
		#endregion XML Storage
		protected override ControlControllerBase CreateController() {
			return new GroupBoxController(this);
		}
		protected virtual Visibility GetShadowVisibility() {
			if(ShowShadow == GroupBoxShadowVisibility.Always ||
				ShowShadow == GroupBoxShadowVisibility.WhenHasMouse && Controller.IsMouseEntered)
				return Visibility.Visible;
			else
				return Visibility.Collapsed;
		}
		protected virtual void OnButtonClick(GroupBoxButton button) {
			switch(button.Kind) {
				case GroupBoxButtonKind.Minimize:
					State = GroupBoxState.Minimized;
					break;
				case GroupBoxButtonKind.Unminimize:
				case GroupBoxButtonKind.Unmaximize:
					State = GroupBoxState.Normal;
					break;
				case GroupBoxButtonKind.Maximize:
					State = GroupBoxState.Maximized;
					break;
			}
		}
		protected virtual bool OnDesignTimeClick(DXMouseButtonEventArgs args) {
			if (MinimizeElementVisibility == Visibility.Visible && MinimizeElement != null && MinimizeElement.Contains(args.GetPosition(null))) {
				OnButtonClick(MinimizeElement);
				return true;
			}
			else
				return false;
		}
		protected virtual bool OnStateChanging(GroupBoxState oldValue, GroupBoxState newValue) {
			return newValue != GroupBoxState.Maximized || Parent is IMaximizingContainer;
		}
		protected virtual void OnStateChanged(GroupBoxState oldValue, GroupBoxState newValue) {
			if (oldValue == GroupBoxState.Maximized &&
				Parent is IMaximizingContainer && ((IMaximizingContainer)Parent).MaximizedElement == this)
				((IMaximizingContainer)Parent).MaximizedElement = null;
			UpdateButtons();
			UpdateState(true);
			UpdateSize(oldValue, newValue);
			IsMaximizedCore = newValue == GroupBoxState.Maximized;
			if (StateChanged != null)
				StateChanged(this, new ValueChangedEventArgs<GroupBoxState>(oldValue, newValue));
			if (newValue == GroupBoxState.Maximized)
				((IMaximizingContainer)Parent).MaximizedElement = this;
		}
		protected virtual void UpdateButtons() {
			if(MinimizeElement != null)
				MinimizeElement.Kind = State == GroupBoxState.Minimized ? GroupBoxButtonKind.Unminimize : GroupBoxButtonKind.Minimize;
			if(MaximizeElement != null)
				MaximizeElement.Kind = State == GroupBoxState.Maximized ? GroupBoxButtonKind.Unmaximize : GroupBoxButtonKind.Maximize;
		}
		protected void UpdateLayoutState(bool useTransitions) {
			GoToState(GetLayoutState(State), useTransitions);
#if !SILVERLIGHT
			UIElement element = BorderElement;
			while (element != null) {
				element.InvalidateMeasure();
				if (element == this)
					break;
				element = (UIElement)VisualTreeHelper.GetParent(element);
			}
#endif
		}
		protected void UpdateShadowVisibility() {
			SetValue(ShadowVisibilityProperty, GetShadowVisibility());
		}
		protected virtual void UpdateSize(GroupBoxState oldState, GroupBoxState newState) {
			DependencyProperty sizeProperty = MinimizationDirection == Orientation.Horizontal ? WidthProperty : HeightProperty;
			DependencyProperty minSizeProperty = MinimizationDirection == Orientation.Horizontal ? MinWidthProperty : MinHeightProperty;
			if (newState == GroupBoxState.Minimized) {
				if (!double.IsNaN((double)GetValue(sizeProperty))) {
					_StoredSizePropertyValue = this.StorePropertyValue(sizeProperty);
					SetValue(sizeProperty, double.NaN);
				}
				if ((double)GetValue(minSizeProperty) != 0) {
					_StoredMinSizePropertyValue = this.StorePropertyValue(minSizeProperty);
					SetValue(minSizeProperty, 0.0);
				}
			}
			if (oldState == GroupBoxState.Minimized) {
				if (_StoredSizePropertyValue != null) {
					this.RestorePropertyValue(sizeProperty, _StoredSizePropertyValue);
					_StoredSizePropertyValue = null;
				}
				if (_StoredMinSizePropertyValue != null) {
					this.RestorePropertyValue(minSizeProperty, _StoredMinSizePropertyValue);
					_StoredMinSizePropertyValue = null;
				}
			}
		}
		protected override void UpdateState(bool useTransitions) {
			base.UpdateState(useTransitions);
			UpdateLayoutState(useTransitions);
		}
		protected virtual void UpdateCurrentTemplate() {
			if(NormalTemplate == null || LightTemplate == null)
				return;
			ControlTemplate template;
			switch(DisplayMode) {
				case GroupBoxDisplayMode.Light:
					template = LightTemplate;
					break;
				case GroupBoxDisplayMode.Normal:
				case GroupBoxDisplayMode.Default:
				default:
					template = NormalTemplate;
					break;
			}
			if(System.Windows.DependencyPropertyHelper.GetValueSource(this, TemplateProperty).BaseValueSource == BaseValueSource.Default)
				SetCurrentValue(TemplateProperty, template);
		}
		#region IGroupBox
		bool IGroupBox.DesignTimeClick(DXMouseButtonEventArgs args) {
			return OnDesignTimeClick(args);
		}
		void IGroupBox.UpdateShadowVisibility() {
			UpdateShadowVisibility();
		}
		Rect IGroupBox.MinimizeElementBounds {
			get {
				return MinimizeElement != null && MinimizeElement.IsInVisualTree() ? MinimizeElement.GetBounds(this) : Rect.Empty;
			}
		}
		#endregion
		#region IMaximizableElement
		void IMaximizableElement.AfterNormalization() {
			if (State == GroupBoxState.Maximized)
				State = GroupBoxState.Normal;
		}
		void IMaximizableElement.BeforeMaximization() {
			State = GroupBoxState.Maximized;
		}
		#endregion
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.LayoutControl.UIAutomation.GroupBoxAutomationPeer(this);
		}
		#endregion
	}
	public class GroupBoxController : ControlControllerBase {
		public GroupBoxController(IGroupBox control) : base(control) { }
		public IGroupBox IGroupBox { get { return IControl as IGroupBox; } }
		#region Keyboard and Mouse Handling
		protected override void OnMouseEnter(DXMouseEventArgs e) {
			base.OnMouseEnter(e);
			IGroupBox.UpdateShadowVisibility();
		}
		protected override void OnMouseLeave(DXMouseEventArgs e) {
			base.OnMouseLeave(e);
			IGroupBox.UpdateShadowVisibility();
		}
		#endregion Keyboard and Mouse Handling
	}
	public class GroupBoxShadow : DevExpress.Xpf.Core.Container {
		#region Dependency Properties
		public static readonly DependencyProperty CornerRadiusProperty =
			DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GroupBoxShadow),
				new PropertyMetadata((o, e) => ((GroupBoxShadow)o).OnCornerRadiusChanged()));
		public static readonly DependencyProperty OffsetProperty =
			DependencyProperty.Register("Offset", typeof(double), typeof(GroupBoxShadow),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						if((double)e.NewValue < 0.0)
							o.SetValue(e.Property, 0.0);
						else
							((GroupBoxShadow)o).OnOffsetChanged();
					}));
		#endregion Dependency Properties
		public GroupBoxShadow() {
			IsHitTestVisible = false;
		}
		public CornerRadius CornerRadius {
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		protected double StartOpacity = 0.07;
		protected double EndOpacity = 0.35;
		protected virtual FrameworkElement CreateElement(double margin, double opacity) {
			var result = new Border();
			result.Background = new SolidColorBrush(Colors.Black);
			result.SetBinding(Border.CornerRadiusProperty, new Binding("CornerRadius") { Source = this });
			result.Margin = new Thickness(margin);
			result.Opacity = opacity;
			return result;
		}
		protected virtual void CreateElements() {
			Children.Clear();
			double transparency = 1.0;
			for (int i = 0; i < Offset; i++) {
				double opacity = StartOpacity + (0.5 + i) * (EndOpacity - StartOpacity) / Offset;
				opacity = 1 - (1 - opacity) / transparency;
				transparency *= 1 - opacity;
				Children.Add(CreateElement(i, opacity));
			}
		}
		protected virtual void OnCornerRadiusChanged() {
		}
		protected virtual void OnOffsetChanged() {
			RenderTransform = new TranslateTransform { X = Offset, Y = Offset };
			CreateElements();
		}
	}
	[TemplatePart(Name = HorizontalRootElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = VerticalRootElementName, Type = typeof(FrameworkElement))]
	public class GroupSeparator : ControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(GroupSeparator),
				new PropertyMetadata(Orientation.Horizontal, (o, e) => ((GroupSeparator)o).OnOrientationChanged()));
		#endregion
		public GroupSeparator() {
			DefaultStyleKey = typeof(GroupSeparator);
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		#region Template
		const string HorizontalRootElementName = "HorizontalRootElement";
		const string VerticalRootElementName = "VerticalRootElement";
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			HorizontalRootElement = GetTemplateChild(HorizontalRootElementName) as FrameworkElement;
			VerticalRootElement = GetTemplateChild(VerticalRootElementName) as FrameworkElement;
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if (HorizontalRootElement != null)
				HorizontalRootElement.SetVisible(Orientation == Orientation.Horizontal);
			if (VerticalRootElement != null)
				VerticalRootElement.SetVisible(Orientation == Orientation.Vertical);
		}
		protected FrameworkElement HorizontalRootElement { get; private set; }
		protected FrameworkElement VerticalRootElement { get; private set; }
		#endregion
		protected virtual void OnOrientationChanged() {
			UpdateTemplate();
		}
	}
}
