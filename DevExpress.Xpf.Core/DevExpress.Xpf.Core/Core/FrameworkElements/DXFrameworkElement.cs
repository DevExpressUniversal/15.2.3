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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
#if !SL
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Core {
	public class DXFrameworkElement : FrameworkElement
#if SL
		, IDependencyPropertyChangeListener 
#endif
	{
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Double ActualHeight {
			get { return base.ActualHeight; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Double ActualWidth {
			get { return base.ActualWidth; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean AllowDrop {
			get { return base.AllowDrop; }
			set { base.AllowDrop = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new CacheMode CacheMode {
			get { return base.CacheMode; }
			set { base.CacheMode = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Geometry Clip {
			get { return base.Clip; }
			set { base.Clip = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Cursor Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Size DesiredSize {
			get { return base.DesiredSize; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Dispatcher Dispatcher {
			get { return base.Dispatcher; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Effect Effect {
			get { return base.Effect; }
			set { base.Effect = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FlowDirection FlowDirection {
			get { return base.FlowDirection; }
			set { base.FlowDirection = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Double Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlignment HorizontalAlignment {
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsHitTestVisible {
			get { return base.IsHitTestVisible; }
			set { base.IsHitTestVisible = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new XmlLanguage Language {
			get { return base.Language; }
			set { base.Language = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness Margin {
			get { return base.Margin; }
			set { base.Margin = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Double MaxHeight {
			get { return base.MaxHeight; }
			set { base.MaxHeight = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Double MaxWidth {
			get { return base.MaxWidth; }
			set { base.MaxWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Double MinHeight {
			get { return base.MinHeight; }
			set { base.MinHeight = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Double MinWidth {
			get { return base.MinWidth; }
			set { base.MinWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Double Opacity {
			get { return base.Opacity; }
			set { base.Opacity = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush OpacityMask {
			get { return base.OpacityMask; }
			set { base.OpacityMask = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DependencyObject Parent {
			get { return base.Parent; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform RenderTransform {
			get { return base.RenderTransform; }
			set { base.RenderTransform = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Point RenderTransformOrigin {
			get { return base.RenderTransformOrigin; }
			set { base.RenderTransformOrigin = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ResourceDictionary Resources {
			get { return base.Resources; }
			set { base.Resources = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style Style {
			get { return base.Style; }
			set { base.Style = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new TriggerCollection Triggers {
			get { return base.Triggers; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean UseLayoutRounding {
			get { return base.UseLayoutRounding; }
			set { base.UseLayoutRounding = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlignment VerticalAlignment {
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Visibility Visibility {
			get { return base.Visibility; }
			set { base.Visibility = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Double Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
#if SL
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new Projection Projection {
			get { return base.Projection; }
			set { base.Projection = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Size RenderSize {
			get { return base.RenderSize; }
		}
#else // WPF
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean AreAnyTouchesCaptured {
			get { return base.AreAnyTouchesCaptured; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean AreAnyTouchesCapturedWithin {
			get { return base.AreAnyTouchesCapturedWithin; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean AreAnyTouchesDirectlyOver {
			get { return base.AreAnyTouchesDirectlyOver; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean AreAnyTouchesOver {
			get { return base.AreAnyTouchesOver; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BindingGroup BindingGroup {
			get { return base.BindingGroup; }
			set { base.BindingGroup = value; }
		}
#pragma warning disable 0618
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BitmapEffect BitmapEffect {
			get { return base.BitmapEffect; }
			set { base.BitmapEffect = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BitmapEffectInput BitmapEffectInput {
			get { return base.BitmapEffectInput; }
			set { base.BitmapEffectInput = value; }
		}
#pragma warning restore 0618
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean ClipToBounds {
			get { return base.ClipToBounds; }
			set { base.ClipToBounds = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new CommandBindingCollection CommandBindings {
			get { return base.CommandBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ContextMenu ContextMenu {
			get { return base.ContextMenu; }
			set { base.ContextMenu = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DependencyObjectType DependencyObjectType {
			get { return base.DependencyObjectType; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean Focusable {
			get { return base.Focusable; }
			set { base.Focusable = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style FocusVisualStyle {
			get { return base.FocusVisualStyle; }
			set { base.FocusVisualStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean ForceCursor {
			get { return base.ForceCursor; }
			set { base.ForceCursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean HasAnimatedProperties {
			get { return base.HasAnimatedProperties; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new InputBindingCollection InputBindings {
			get { return base.InputBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new InputScope InputScope {
			get { return base.InputScope; }
			set { base.InputScope = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsArrangeValid {
			get { return base.IsArrangeValid; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsEnabled {
			get { return base.IsEnabled; }
			set { base.IsEnabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsFocused {
			get { return base.IsFocused; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsInitialized {
			get { return base.IsInitialized; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsInputMethodEnabled {
			get { return base.IsInputMethodEnabled; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsKeyboardFocused {
			get { return base.IsKeyboardFocused; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsKeyboardFocusWithin {
			get { return base.IsKeyboardFocusWithin; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsLoaded {
			get { return base.IsLoaded; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsManipulationEnabled {
			get { return base.IsManipulationEnabled; }
			set { base.IsManipulationEnabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsMeasureValid {
			get { return base.IsMeasureValid; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsMouseCaptured {
			get { return base.IsMouseCaptured; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsMouseCaptureWithin {
			get { return base.IsMouseCaptureWithin; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsMouseDirectlyOver {
			get { return base.IsMouseDirectlyOver; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsMouseOver {
			get { return base.IsMouseOver; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsSealed {
			get { return base.IsSealed; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsStylusCaptured {
			get { return base.IsStylusCaptured; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsStylusCaptureWithin {
			get { return base.IsStylusCaptureWithin; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsStylusDirectlyOver {
			get { return base.IsStylusDirectlyOver; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsStylusOver {
			get { return base.IsStylusOver; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean IsVisible {
			get { return base.IsVisible; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform LayoutTransform {
			get { return base.LayoutTransform; }
			set { base.LayoutTransform = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean OverridesDefaultStyle {
			get { return base.OverridesDefaultStyle; }
			set { base.OverridesDefaultStyle = value; }
		}
#pragma warning disable 0618
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int PersistId {
			get { return base.PersistId; }
		}
#pragma warning restore 0618
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Size RenderSize {
			get { return base.RenderSize; }
			set { base.RenderSize = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Boolean SnapsToDevicePixels {
			get { return base.SnapsToDevicePixels; }
			set { base.SnapsToDevicePixels = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DependencyObject TemplatedParent {
			get { return base.TemplatedParent; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Object ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new IEnumerable<TouchDevice> TouchesCaptured {
			get { return base.TouchesCaptured; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new IEnumerable<TouchDevice> TouchesCapturedWithin {
			get { return base.TouchesCapturedWithin; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new IEnumerable<TouchDevice> TouchesDirectlyOver {
			get { return base.TouchesDirectlyOver; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new IEnumerable<TouchDevice> TouchesOver {
			get { return base.TouchesOver; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new String Uid {
			get { return base.Uid; }
			set { base.Uid = value; }
		}
#endif
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DragEventHandler DragEnter {
			add { base.DragEnter += value; }
			remove { base.DragEnter -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DragEventHandler DragLeave {
			add { base.DragLeave += value; }
			remove { base.DragLeave -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DragEventHandler DragOver {
			add { base.DragOver += value; }
			remove { base.DragOver -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DragEventHandler Drop {
			add { base.Drop += value; }
			remove { base.Drop -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event RoutedEventHandler GotFocus {
			add { base.GotFocus += value; }
			remove { base.GotFocus -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event KeyEventHandler KeyDown {
			add { base.KeyDown += value; }
			remove { base.KeyDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event KeyEventHandler KeyUp {
			add { base.KeyUp += value; }
			remove { base.KeyUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler LayoutUpdated {
			add { base.LayoutUpdated += value; }
			remove { base.LayoutUpdated -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event RoutedEventHandler Loaded {
			add { base.Loaded += value; }
			remove { base.Loaded -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event RoutedEventHandler LostFocus {
			add { base.LostFocus += value; }
			remove { base.LostFocus -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseEventHandler LostMouseCapture {
			add { base.LostMouseCapture += value; }
			remove { base.LostMouseCapture -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<ManipulationCompletedEventArgs> ManipulationCompleted {
			add { base.ManipulationCompleted += value; }
			remove { base.ManipulationCompleted -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<ManipulationDeltaEventArgs> ManipulationDelta {
			add { base.ManipulationDelta += value; }
			remove { base.ManipulationDelta -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<ManipulationStartedEventArgs> ManipulationStarted {
			add { base.ManipulationStarted += value; }
			remove { base.ManipulationStarted -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseEventHandler MouseEnter {
			add { base.MouseEnter += value; }
			remove { base.MouseEnter -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseEventHandler MouseLeave {
			add { base.MouseLeave += value; }
			remove { base.MouseLeave -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler MouseLeftButtonDown {
			add { base.MouseLeftButtonDown += value; }
			remove { base.MouseLeftButtonDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler MouseLeftButtonUp {
			add { base.MouseLeftButtonUp += value; }
			remove { base.MouseLeftButtonUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseEventHandler MouseMove {
			add { base.MouseMove += value; }
			remove { base.MouseMove -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler MouseRightButtonDown {
			add { base.MouseRightButtonDown += value; }
			remove { base.MouseRightButtonDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler MouseRightButtonUp {
			add { base.MouseRightButtonUp += value; }
			remove { base.MouseRightButtonUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseWheelEventHandler MouseWheel {
			add { base.MouseWheel += value; }
			remove { base.MouseWheel -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event SizeChangedEventHandler SizeChanged {
			add { base.SizeChanged += value; }
			remove { base.SizeChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event TextCompositionEventHandler TextInput {
			add { base.TextInput += value; }
			remove { base.TextInput -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event RoutedEventHandler Unloaded {
			add { base.Unloaded += value; }
			remove { base.Unloaded -= value; }
		}
#if SL
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<ValidationErrorEventArgs> BindingValidationError {
			add { base.BindingValidationError += value; }
			remove { base.BindingValidationError -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event TextCompositionEventHandler TextInputStart {
			add { base.TextInputStart += value; }
			remove { base.TextInputStart -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event TextCompositionEventHandler TextInputUpdate {
			add { base.TextInputUpdate += value; }
			remove { base.TextInputUpdate -= value; }
		}
#else // WPF
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event ContextMenuEventHandler ContextMenuClosing {
			add { base.ContextMenuClosing += value; }
			remove { base.ContextMenuClosing -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event ContextMenuEventHandler ContextMenuOpening {
			add { base.ContextMenuOpening += value; }
			remove { base.ContextMenuOpening -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler FocusableChanged {
			add { base.FocusableChanged += value; }
			remove { base.FocusableChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event GiveFeedbackEventHandler GiveFeedback {
			add { base.GiveFeedback += value; }
			remove { base.GiveFeedback -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event KeyboardFocusChangedEventHandler GotKeyboardFocus {
			add { base.GotKeyboardFocus += value; }
			remove { base.GotKeyboardFocus -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseEventHandler GotMouseCapture {
			add { base.GotMouseCapture += value; }
			remove { base.GotMouseCapture -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler GotStylusCapture {
			add { base.GotStylusCapture += value; }
			remove { base.GotStylusCapture -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<TouchEventArgs> GotTouchCapture {
			add { base.GotTouchCapture += value; }
			remove { base.GotTouchCapture -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler Initialized {
			add { base.Initialized += value; }
			remove { base.Initialized -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsEnabledChanged {
			add { base.IsEnabledChanged += value; }
			remove { base.IsEnabledChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsHitTestVisibleChanged {
			add { base.IsHitTestVisibleChanged += value; }
			remove { base.IsHitTestVisibleChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsKeyboardFocusedChanged {
			add { base.IsKeyboardFocusedChanged += value; }
			remove { base.IsKeyboardFocusedChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsKeyboardFocusWithinChanged {
			add { base.IsKeyboardFocusWithinChanged += value; }
			remove { base.IsKeyboardFocusWithinChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsMouseCapturedChanged {
			add { base.IsMouseCapturedChanged += value; }
			remove { base.IsMouseCapturedChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsMouseCaptureWithinChanged {
			add { base.IsMouseCaptureWithinChanged += value; }
			remove { base.IsMouseCaptureWithinChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsMouseDirectlyOverChanged {
			add { base.IsMouseDirectlyOverChanged += value; }
			remove { base.IsMouseDirectlyOverChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsStylusCapturedChanged {
			add { base.IsStylusCapturedChanged += value; }
			remove { base.IsStylusCapturedChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsStylusCaptureWithinChanged {
			add { base.IsStylusCaptureWithinChanged += value; }
			remove { base.IsStylusCaptureWithinChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsStylusDirectlyOverChanged {
			add { base.IsStylusDirectlyOverChanged += value; }
			remove { base.IsStylusDirectlyOverChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DependencyPropertyChangedEventHandler IsVisibleChanged {
			add { base.IsVisibleChanged += value; }
			remove { base.IsVisibleChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event KeyboardFocusChangedEventHandler LostKeyboardFocus {
			add { base.LostKeyboardFocus += value; }
			remove { base.LostKeyboardFocus -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler LostStylusCapture {
			add { base.LostStylusCapture += value; }
			remove { base.LostStylusCapture -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<TouchEventArgs> LostTouchCapture {
			add { base.LostTouchCapture += value; }
			remove { base.LostTouchCapture -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<ManipulationBoundaryFeedbackEventArgs> ManipulationBoundaryFeedback {
			add { base.ManipulationBoundaryFeedback += value; }
			remove { base.ManipulationBoundaryFeedback -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<ManipulationInertiaStartingEventArgs> ManipulationInertiaStarting {
			add { base.ManipulationInertiaStarting += value; }
			remove { base.ManipulationInertiaStarting -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<ManipulationStartingEventArgs> ManipulationStarting {
			add { base.ManipulationStarting += value; }
			remove { base.ManipulationStarting -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler MouseDown {
			add { base.MouseDown += value; }
			remove { base.MouseDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler MouseUp {
			add { base.MouseUp += value; }
			remove { base.MouseUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DragEventHandler PreviewDragEnter {
			add { base.PreviewDragEnter += value; }
			remove { base.PreviewDragEnter -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DragEventHandler PreviewDragLeave {
			add { base.PreviewDragLeave += value; }
			remove { base.PreviewDragLeave -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DragEventHandler PreviewDragOver {
			add { base.PreviewDragOver += value; }
			remove { base.PreviewDragOver -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event DragEventHandler PreviewDrop {
			add { base.PreviewDrop += value; }
			remove { base.PreviewDrop -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event GiveFeedbackEventHandler PreviewGiveFeedback {
			add { base.PreviewGiveFeedback += value; }
			remove { base.PreviewGiveFeedback -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event KeyboardFocusChangedEventHandler PreviewGotKeyboardFocus {
			add { base.PreviewGotKeyboardFocus += value; }
			remove { base.PreviewGotKeyboardFocus -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event KeyEventHandler PreviewKeyDown {
			add { base.PreviewKeyDown += value; }
			remove { base.PreviewKeyDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event KeyEventHandler PreviewKeyUp {
			add { base.PreviewKeyUp += value; }
			remove { base.PreviewKeyUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event KeyboardFocusChangedEventHandler PreviewLostKeyboardFocus {
			add { base.PreviewLostKeyboardFocus += value; }
			remove { base.PreviewLostKeyboardFocus -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler PreviewMouseDown {
			add { base.PreviewMouseDown += value; }
			remove { base.PreviewMouseDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler PreviewMouseLeftButtonDown {
			add { base.PreviewMouseLeftButtonDown += value; }
			remove { base.PreviewMouseLeftButtonDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler PreviewMouseLeftButtonUp {
			add { base.PreviewMouseLeftButtonUp += value; }
			remove { base.PreviewMouseLeftButtonUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseEventHandler PreviewMouseMove {
			add { base.PreviewMouseMove += value; }
			remove { base.PreviewMouseMove -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler PreviewMouseRightButtonDown {
			add { base.PreviewMouseRightButtonDown += value; }
			remove { base.PreviewMouseRightButtonDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler PreviewMouseRightButtonUp {
			add { base.PreviewMouseRightButtonUp += value; }
			remove { base.PreviewMouseRightButtonUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseButtonEventHandler PreviewMouseUp {
			add { base.PreviewMouseUp += value; }
			remove { base.PreviewMouseUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event MouseWheelEventHandler PreviewMouseWheel {
			add { base.PreviewMouseWheel += value; }
			remove { base.PreviewMouseWheel -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event TextCompositionEventHandler PreviewTextInput {
			add { base.PreviewTextInput += value; }
			remove { base.PreviewTextInput -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event QueryContinueDragEventHandler PreviewQueryContinueDrag {
			add { base.PreviewQueryContinueDrag += value; }
			remove { base.PreviewQueryContinueDrag -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusButtonEventHandler PreviewStylusButtonDown {
			add { base.PreviewStylusButtonDown += value; }
			remove { base.PreviewStylusButtonDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusButtonEventHandler PreviewStylusButtonUp {
			add { base.PreviewStylusButtonUp += value; }
			remove { base.PreviewStylusButtonUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusDownEventHandler PreviewStylusDown {
			add { base.PreviewStylusDown += value; }
			remove { base.PreviewStylusDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler PreviewStylusInAirMove {
			add { base.PreviewStylusInAirMove += value; }
			remove { base.PreviewStylusInAirMove -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler PreviewStylusInRange {
			add { base.PreviewStylusInRange += value; }
			remove { base.PreviewStylusInRange -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler PreviewStylusMove {
			add { base.PreviewStylusMove += value; }
			remove { base.PreviewStylusMove -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler PreviewStylusOutOfRange {
			add { base.PreviewStylusOutOfRange += value; }
			remove { base.PreviewStylusOutOfRange -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusSystemGestureEventHandler PreviewStylusSystemGesture {
			add { base.PreviewStylusSystemGesture += value; }
			remove { base.PreviewStylusSystemGesture -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler PreviewStylusUp {
			add { base.PreviewStylusUp += value; }
			remove { base.PreviewStylusUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<TouchEventArgs> PreviewTouchDown {
			add { base.PreviewTouchDown += value; }
			remove { base.PreviewTouchDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<TouchEventArgs> PreviewTouchMove {
			add { base.PreviewTouchMove += value; }
			remove { base.PreviewTouchMove -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<TouchEventArgs> PreviewTouchUp {
			add { base.PreviewTouchUp += value; }
			remove { base.PreviewTouchUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event QueryContinueDragEventHandler QueryContinueDrag {
			add { base.QueryContinueDrag += value; }
			remove { base.QueryContinueDrag -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event QueryCursorEventHandler QueryCursor {
			add { base.QueryCursor += value; }
			remove { base.QueryCursor -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event RequestBringIntoViewEventHandler RequestBringIntoView {
			add { base.RequestBringIntoView += value; }
			remove { base.RequestBringIntoView -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<DataTransferEventArgs> SourceUpdated {
			add { base.SourceUpdated += value; }
			remove { base.SourceUpdated -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusButtonEventHandler StylusButtonDown {
			add { base.StylusButtonDown += value; }
			remove { base.StylusButtonDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusButtonEventHandler StylusButtonUp {
			add { base.StylusButtonUp += value; }
			remove { base.StylusButtonUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusDownEventHandler StylusDown {
			add { base.StylusDown += value; }
			remove { base.StylusDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler StylusEnter {
			add { base.StylusEnter += value; }
			remove { base.StylusEnter -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler StylusInAirMove {
			add { base.StylusInAirMove += value; }
			remove { base.StylusInAirMove -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler StylusInRange {
			add { base.StylusInRange += value; }
			remove { base.StylusInRange -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler StylusLeave {
			add { base.StylusLeave += value; }
			remove { base.StylusLeave -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler StylusMove {
			add { base.StylusMove += value; }
			remove { base.StylusMove -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler StylusOutOfRange {
			add { base.StylusOutOfRange += value; }
			remove { base.StylusOutOfRange -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusSystemGestureEventHandler StylusSystemGesture {
			add { base.StylusSystemGesture += value; }
			remove { base.StylusSystemGesture -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event StylusEventHandler StylusUp {
			add { base.StylusUp += value; }
			remove { base.StylusUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<DataTransferEventArgs> TargetUpdated {
			add { base.TargetUpdated += value; }
			remove { base.TargetUpdated -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event ToolTipEventHandler ToolTipClosing {
			add { base.ToolTipClosing += value; }
			remove { base.ToolTipClosing -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event ToolTipEventHandler ToolTipOpening {
			add { base.ToolTipOpening += value; }
			remove { base.ToolTipOpening -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<TouchEventArgs> TouchDown {
			add { base.TouchDown += value; }
			remove { base.TouchDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<TouchEventArgs> TouchEnter {
			add { base.TouchEnter += value; }
			remove { base.TouchEnter -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<TouchEventArgs> TouchLeave {
			add { base.TouchLeave += value; }
			remove { base.TouchLeave -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<TouchEventArgs> TouchMove {
			add { base.TouchMove += value; }
			remove { base.TouchMove -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler<TouchEventArgs> TouchUp {
			add { base.TouchUp += value; }
			remove { base.TouchUp -= value; }
		}
#endif
#if SL
		#region IDependencyPropertyChangeListener Members
		void IDependencyPropertyChangeListener.OnPropertyAssigning(DependencyProperty dp, object value) {
		}
		void IDependencyPropertyChangeListener.OnPropertyChanged(SLDependencyPropertyChangedEventArgs e) {
			OnPropertyChanged(e);
		}
		protected virtual void OnPropertyChanged(SLDependencyPropertyChangedEventArgs e) { }
		#endregion
#endif
	}
}
