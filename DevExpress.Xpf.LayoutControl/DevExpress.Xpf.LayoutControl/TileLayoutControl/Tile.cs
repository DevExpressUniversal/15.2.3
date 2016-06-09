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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows.Data;
#else
using System.Collections.Generic;
#endif
namespace DevExpress.Xpf.LayoutControl {
	public enum TileSize { ExtraSmall, Small, Large, ExtraLarge }
	public interface ITile {
		void Click();
		TileSize Size { get; }
	}
	[TemplatePart(Name = ContentChangeStoryboardName, Type = typeof(Storyboard))]
	[DXToolboxBrowsable]
	public class Tile : MaximizableHeaderedContentControlBase, ITile, IMaximizableElement, IClickable {
		public const int DefaultContentChangeInterval = 5;  
		public const double ExtraLargeSize = 310;
		public const double ExtraSmallSize = 70;
		public const double LargeWidth = 310;
		public const double LargeHeight = 150;
		public const double SmallWidth = 150;
		public const double SmallHeight = 150;
		#region Dependency Properties
		private bool _IsChangingIsMaximized;
		public static readonly DependencyProperty CalculatedBackgroundProperty =
			DependencyProperty.Register("CalculatedBackground", typeof(Brush), typeof(Tile), null);
		public static readonly DependencyProperty CalculatedHeaderVisibilityProperty =
			DependencyProperty.Register("CalculatedHeaderVisibility", typeof(Visibility), typeof(Tile), null);
		public static readonly DependencyProperty PreviousContentProperty =
			DependencyProperty.Register("PreviousContent", typeof(object), typeof(Tile), null);
		public static readonly DependencyProperty AnimateContentChangeProperty =
			DependencyProperty.Register("AnimateContentChange", typeof(bool), typeof(Tile), new PropertyMetadata(true));
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(Tile), null);
		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.Register("CommandParameter", typeof(object), typeof(Tile), null);
		public static readonly DependencyProperty ContentChangeIntervalProperty =
			DependencyProperty.Register("ContentChangeInterval", typeof(TimeSpan), typeof(Tile),
				new PropertyMetadata(TimeSpan.FromSeconds(DefaultContentChangeInterval), (o, e) => ((Tile)o).OnContentChangeIntervalChanged()));
		public static readonly DependencyProperty ContentSourceProperty =
			DependencyProperty.Register("ContentSource", typeof(IEnumerable), typeof(Tile),
				new PropertyMetadata((o, e) => ((Tile)o).OnContentSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue)));
		public static readonly DependencyProperty HorizontalHeaderAlignmentProperty =
			DependencyProperty.Register("HorizontalHeaderAlignment", typeof(HorizontalAlignment), typeof(Tile), null);
		public static readonly DependencyProperty IsMaximizedProperty =
			DependencyProperty.Register("IsMaximized", typeof(bool), typeof(Tile),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (Tile)o;
						if (control._IsChangingIsMaximized)
							return;
						control._IsChangingIsMaximized = true;
						if ((bool)e.NewValue && !(control.Parent is IMaximizingContainer))
							o.SetValue(e.Property, e.OldValue);
						else
							control.OnIsMaximizedChanged();
						control._IsChangingIsMaximized = false;
					}));
		public static readonly DependencyProperty SizeProperty =
			DependencyProperty.Register("Size", typeof(TileSize), typeof(Tile), new PropertyMetadata(TileSize.Large, (o, e) => ((Tile)o).OnSizeChanged()));
		public static readonly DependencyProperty VerticalHeaderAlignmentProperty =
			DependencyProperty.Register("VerticalHeaderAlignment", typeof(VerticalAlignment), typeof(Tile), null);
		private static readonly DependencyProperty BackgroundListener = RegisterPropertyListener("Background");
		#endregion Dependency Properties
		public Tile() {
			DefaultStyleKey = typeof(Tile);
			CalculateHeaderVisibility();
			AttachPropertyListener("Background", BackgroundListener);
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileAnimateContentChange")]
#endif
		public bool AnimateContentChange {
			get { return (bool)GetValue(AnimateContentChangeProperty); }
			set { SetValue(AnimateContentChangeProperty, value); }
		}
		public Brush CalculatedBackground {
			get { return (Brush)GetValue(CalculatedBackgroundProperty); }
			private set { SetValue(CalculatedBackgroundProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileCommand")]
#endif
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileCommandParameter")]
#endif
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileContentChangeInterval")]
#endif
		public TimeSpan ContentChangeInterval {
			get { return (TimeSpan)GetValue(ContentChangeIntervalProperty); }
			set { SetValue(ContentChangeIntervalProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileContentSource")]
#endif
		public IEnumerable ContentSource {
			get { return (IEnumerable)GetValue(ContentSourceProperty); }
			set { SetValue(ContentSourceProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileController")]
#endif
		public new TileController Controller { get { return (TileController)base.Controller; } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileHorizontalHeaderAlignment")]
#endif
		public HorizontalAlignment HorizontalHeaderAlignment {
			get { return (HorizontalAlignment)GetValue(HorizontalHeaderAlignmentProperty); }
			set { SetValue(HorizontalHeaderAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileIsMaximized")]
#endif
		public bool IsMaximized {
			get { return (bool)GetValue(IsMaximizedProperty); }
			set { SetValue(IsMaximizedProperty, value); }
		}
		public object PreviousContent {
			get { return (object)GetValue(PreviousContentProperty); }
			private set { SetValue(PreviousContentProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileSize")]
#endif
		public TileSize Size {
			get { return (TileSize)GetValue(SizeProperty); }
			set { SetValue(SizeProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileVerticalHeaderAlignment")]
#endif
		public VerticalAlignment VerticalHeaderAlignment {
			get { return (VerticalAlignment)GetValue(VerticalHeaderAlignmentProperty); }
			set { SetValue(VerticalHeaderAlignmentProperty, value); }
		}
		public event EventHandler Click;
		public event EventHandler IsMaximizedChanged;
		#region Template
		const string ContentChangeStoryboardName = "ContentChangeStoryboard";
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ContentChangeStoryboard = GetTemplateChild(ContentChangeStoryboardName) as Storyboard;
		}
		protected Storyboard ContentChangeStoryboard { get; private set; }
		protected bool IsContentChangeStoryboardActive { get; private set; }
		#endregion Template
		#region Layout
		protected override Size OnMeasure(Size availableSize) {
			if (IsMaximized)
				return base.OnMeasure(availableSize);
			var result = new Size();
			switch (Size) {
				case TileSize.ExtraSmall:
					result.Width = result.Height = ExtraSmallSize;
					break;
				case TileSize.Small:
					result.Width = SmallWidth;
					result.Height = SmallHeight;
					break;
				case TileSize.Large:
					result.Width = LargeWidth;
					result.Height = LargeHeight;
					break;
				case TileSize.ExtraLarge:
					result.Width = result.Height = ExtraLargeSize;
					break;
			}
			if (!double.IsNaN(Width))
				result.Width = Width;
			if (!double.IsNaN(Height))
				result.Height = Height;
			base.OnMeasure(result);
			return result;
		}
		#endregion Layout
		protected override ControlControllerBase CreateController() {
			return new TileController(this);
		}
		protected virtual Color CalculateGradientEndColor(Color startColor) {
			return Color.FromArgb(
				startColor.A,
				CalculateGradientEndColorChannel(startColor.R),
				CalculateGradientEndColorChannel(startColor.G),
				CalculateGradientEndColorChannel(startColor.B));
		}
		protected virtual byte CalculateGradientEndColorChannel(byte startColorChannel) {
			if (startColorChannel < 8 || startColorChannel > 247)
				return startColorChannel;
			int delta;
			if (startColorChannel < 128)
				delta = startColorChannel / 4 - 1;
			else
				delta = 63 - startColorChannel / 4;
			return (byte)(startColorChannel + delta);
		}
		protected void CalculateHeaderVisibility() {
			SetValue(CalculatedHeaderVisibilityProperty, IsHeaderVisible() ? Visibility.Visible : Visibility.Collapsed);
		}
		protected virtual bool CanAnimateContentChange() {
			return AnimateContentChange && !IsMaximized && (!this.IsInDesignTool() || IsContentChangeActive);
		}
		protected virtual Brush GetCalculatedBackground() {
			var background = Background as SolidColorBrush;
			if (background == null)
				return null;
			Color color1 = background.Color;
			Color color2 = CalculateGradientEndColor(color1);
			if (color2 == color1)
				return null;
			else
				return new LinearGradientBrush(
					new GradientStopCollection {
						new GradientStop { Offset = 0, Color = color1 },
						new GradientStop { Offset = 1, Color = color2 } },
					0);
		}
		protected virtual bool IsHeaderVisible() {
			return Size != TileSize.ExtraSmall;
		}
		protected virtual void OnBackgroundChanged() {
			UpdateCalculatedBackground();
		}
		protected virtual void OnClick() {
			if (Click != null)
				Click(this, EventArgs.Empty);
			if (Command != null && Command.CanExecute(CommandParameter))
				Command.Execute(CommandParameter);
		}
		protected override void OnContentChanged(object oldValue, object newValue) {
			base.OnContentChanged(oldValue, newValue);
#if SILVERLIGHT
			var contentPresenter = GetTemplateChild("content") as ContentPresenter;
			if (contentPresenter != null) {
				PreviousContent = null;
				contentPresenter.SetBinding(ContentPresenter.ContentProperty, new Binding("Content") { Source = this });
			}
#endif
			PreviousContent = oldValue;
			if (CanAnimateContentChange() && IsLoaded && this.IsInVisualTree() && ContentChangeStoryboard != null) {
				if (IsContentChangeStoryboardActive)
					ContentChangeStoryboard.SkipToFill();
				ContentChangeStoryboard.Begin();
				IsContentChangeStoryboardActive = true;
			}
		}
		protected virtual void OnContentChangeIntervalChanged() {
			UpdateContentChangeInterval();
		}
		protected virtual void OnContentSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
#if !SILVERLIGHT
			if (oldValue is ICollection)
				foreach (object content in oldValue)
					RemoveLogicalChild(content);
			if (newValue is ICollection)
				foreach (object content in newValue)
					AddLogicalChild(content);
#endif
			if (ContentSource != null)
				StartContentChange();
			else
				StopContentChange();
		}
		protected virtual void OnIsMaximizedChanged() {
			if (!IsMaximized && Parent is IMaximizingContainer && ((IMaximizingContainer)Parent).MaximizedElement == this)
				((IMaximizingContainer)Parent).MaximizedElement = null;
			IsMaximizedCore = IsMaximized;
			if (IsMaximizedChanged != null)
				IsMaximizedChanged(this, EventArgs.Empty);
			if (IsMaximized)
				((IMaximizingContainer)Parent).MaximizedElement = this;
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			UpdateCalculatedBackground();
			UpdateContentChangeState();
		}
		protected override void OnPropertyChanged(DependencyProperty propertyListener, object oldValue, object newValue) {
			base.OnPropertyChanged(propertyListener, oldValue, newValue);
			if (propertyListener == BackgroundListener)
				OnBackgroundChanged();
		}
		protected virtual void OnSizeChanged() {
			CalculateHeaderVisibility();
			InvalidateMeasure();
			if (Parent is ITileLayoutControl)
				((ITileLayoutControl)Parent).OnTileSizeChanged(this);
		}
		protected override void OnUnloaded() {
			base.OnUnloaded();
			UpdateContentChangeState();
		}
		protected void UpdateCalculatedBackground() {
			CalculatedBackground = GetCalculatedBackground();
		}
#if !SILVERLIGHT
		protected override IEnumerator LogicalChildren {
			get {
				if (!(ContentSource is ICollection))
					return base.LogicalChildren;
				var logicalChildren = new List<object>();
				IEnumerator baseLogicalChildren = base.LogicalChildren;
				if (baseLogicalChildren != null)
					while (baseLogicalChildren.MoveNext())
						logicalChildren.Add(baseLogicalChildren.Current);
				foreach (object content in ContentSource)
					logicalChildren.Add(content);
				return logicalChildren.GetEnumerator();
			}
		}
#endif
		#region Content Change
		private void ChangeContent() {
			if (!ContentEnumerator.MoveNext()) {
				ContentEnumerator.Reset();
				if (!ContentEnumerator.MoveNext()) {
					Content = null;
					return;
				}
			}
			Content = ContentEnumerator.Current;
		}
		private void StartContentChange() {
			if (IsContentChangeActive)
				StopContentChange();
			ContentEnumerator = ContentSource.GetEnumerator();
			ChangeContent();
			ContentChangeTimer = new DispatcherTimer();
			ContentChangeTimer.Interval = ContentChangeInterval;
			ContentChangeTimer.Tick += (o, e) => ChangeContent();
			UpdateContentChangeState();
		}
		private void StopContentChange() {
			if (!IsContentChangeActive)
				return;
			ContentChangeTimer.Stop();
			ContentChangeTimer = null;
			ContentEnumerator = null;
			ClearValue(ContentProperty);
		}
		private void UpdateContentChangeInterval() {
			if (IsContentChangeActive)
				ContentChangeTimer.Interval = ContentChangeInterval;
		}
		private void UpdateContentChangeState() {
			if (!IsContentChangeActive)
				return;
			if (IsLoaded)
				ContentChangeTimer.Start();
			else
				ContentChangeTimer.Stop();
		}
		private DispatcherTimer ContentChangeTimer { get; set; }
		private IEnumerator ContentEnumerator { get; set; }
		private bool IsContentChangeActive { get { return ContentEnumerator != null; } }
		#endregion Content Change
		#region ITile
		void ITile.Click() {
			OnClick();
			if (Parent is ITileLayoutControl)
				((ITileLayoutControl)Parent).TileClick(this);
		}
		#endregion ITile
		#region IMaximizableElement
		void IMaximizableElement.AfterNormalization() {
			IsMaximized = false;
		}
		void IMaximizableElement.BeforeMaximization() {
			IsMaximized = true;
		}
		#endregion
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.LayoutControl.UIAutomation.TileAutomationPeer(this);
		}
		#endregion
	}
	public class TileController : ControlControllerBase {
		public TileController(IControl control) : base(control) { }
		public ITile ITile { get { return Control as ITile; } }
		internal void InvokeClick() {
			OnClick();
		}
		void OnClick() {
			ITile.Click();
		}
		#region Keyboard and Mouse Handling
		protected override void OnMouseLeftButtonUp(DXMouseButtonEventArgs e) {
			bool isClick = IsMouseLeftButtonDown;
			base.OnMouseLeftButtonUp(e);
			if (isClick) {
				OnClick();
				e.Handled = true;
			}
		}
		#endregion Keyboard and Mouse Handling
	}
}
