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
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class SymbolOptions : GaugeDependencyObject {
		public static readonly DependencyProperty MarginProperty = DependencyPropertyManager.Register("Margin",
			typeof(Thickness), typeof(SymbolOptions), new PropertyMetadata(new Thickness(0.0), NotifyPropertyChanged));
		public static readonly DependencyProperty SkewAngleXProperty = DependencyPropertyManager.Register("SkewAngleX",
			typeof(double), typeof(SymbolOptions), new PropertyMetadata(0.0, NotifyPropertyChanged));
		public static readonly DependencyProperty SkewAngleYProperty = DependencyPropertyManager.Register("SkewAngleY",
			typeof(double), typeof(SymbolOptions), new PropertyMetadata(0.0, NotifyPropertyChanged));
		[Category(Categories.Layout)]
		public Thickness Margin {
			get { return (Thickness)GetValue(MarginProperty); }
			set { SetValue(MarginProperty, value); }
		}
		[Category(Categories.Layout)]
		public double SkewAngleX {
			get { return (double)GetValue(SkewAngleXProperty); }
			set { SetValue(SkewAngleXProperty, value); }
		}
		[Category(Categories.Layout)]
		public double SkewAngleY {
			get { return (double)GetValue(SkewAngleYProperty); }
			set { SetValue(SkewAngleYProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SymbolOptions();
		}
	}
	public abstract class SymbolViewBase : GaugeDependencyObject, IOwnedElement, IWeakEventListener {
		public static readonly DependencyProperty AnimationProperty = DependencyPropertyManager.Register("Animation",
			typeof(SymbolsAnimation), typeof(SymbolViewBase), new PropertyMetadata(null, AnimationPropertyChanged));
		public static readonly DependencyProperty OptionsProperty = DependencyPropertyManager.Register("Options",
			typeof(SymbolOptions), typeof(SymbolViewBase), new PropertyMetadata(OptionsPropertyChanged));
		public static readonly DependencyProperty HeightProperty = DependencyPropertyManager.Register("Height",
			typeof(SymbolLength), typeof(SymbolViewBase), new PropertyMetadata(new SymbolLength(SymbolLengthType.Auto), SymbolSizePropertyChanged));
		public static readonly DependencyProperty WidthProperty = DependencyPropertyManager.Register("Width",
			typeof(SymbolLength), typeof(SymbolViewBase), new PropertyMetadata(new SymbolLength(SymbolLengthType.Auto), SymbolSizePropertyChanged));
		internal static readonly DependencyPropertyKey CustomSymbolMappingPropertyKey = DependencyPropertyManager.RegisterReadOnly("CustomSymbolMapping",
			typeof(SymbolDictionary), typeof(SymbolViewBase), new PropertyMetadata());
		public static readonly DependencyProperty CustomSymbolMappingProperty = CustomSymbolMappingPropertyKey.DependencyProperty;
		[Category(Categories.Presentation)]
		public SymbolsAnimation Animation {
			get { return (SymbolsAnimation)GetValue(AnimationProperty); }
			set { SetValue(AnimationProperty, value); }
		}
		[Category(Categories.Presentation)]
		public SymbolOptions Options {
			get { return (SymbolOptions)GetValue(OptionsProperty); }
			set { SetValue(OptionsProperty, value); }
		}
		[Category(Categories.Layout)]
		public SymbolLength Width {
			get { return (SymbolLength)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		[Category(Categories.Layout)]
		public SymbolLength Height {
			get { return (SymbolLength)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		[Category(Categories.Data)]
		public SymbolDictionary CustomSymbolMapping {
			get { return (SymbolDictionary)GetValue(CustomSymbolMappingProperty); }
		}
		static void AnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SymbolViewBase symbolView = d as SymbolViewBase;
			if (symbolView != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SymbolsAnimation, e.NewValue as SymbolsAnimation, symbolView);
				symbolView.OnAnimationChanged();
			}
		}
		static void OptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SymbolViewBase symbolView = d as SymbolViewBase;
			if (symbolView != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SymbolOptions, e.NewValue as SymbolOptions, symbolView);
				symbolView.OnOptionsChanged();
			}
		}
		static void SymbolSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SymbolViewBase symbolView = d as SymbolViewBase;
			if (symbolView != null && symbolView.Gauge != null)
				symbolView.Gauge.InvalidateLayout();
		}
		protected static void PresentationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SymbolViewBase view = d as SymbolViewBase;
			if (view != null && view.Gauge != null && !Object.Equals(e.NewValue, e.OldValue))
				view.Gauge.UpdateSymbolsModels();
		}
		object owner;
		internal DigitalGaugeControl Gauge { get { return owner as DigitalGaugeControl; } }		
		public SymbolViewBase() {
			this.SetValue(CustomSymbolMappingPropertyKey, new SymbolDictionary(this));
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set { owner = value; }
		}
		#endregion
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		void OnAnimationChanged() {
			if (Gauge != null)
				Gauge.Animate();
		}
		protected void OnOptionsChanged() {
			if (Gauge != null)
				Gauge.ViewOptionsChanged();
		}
		protected virtual bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if (sender is SymbolsAnimation) {
					OnAnimationChanged();
					success = true;
				}
				else if ((sender is GaugeDependencyObject)) {
					OnOptionsChanged();
					success = true;
				}
			}
			return success;
		}
		protected internal abstract SymbolViewInternal CreateInternalView();
	}
	public abstract class SegmentsView : SymbolViewBase {
		internal static readonly DependencyPropertyKey SymbolMappingPropertyKey = DependencyPropertyManager.RegisterReadOnly("SymbolMapping",
			typeof(SymbolDictionary), typeof(SegmentsView), new PropertyMetadata());
		public static readonly DependencyProperty SymbolMappingProperty = SymbolMappingPropertyKey.DependencyProperty;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public SymbolDictionary SymbolMapping {
			get { return (SymbolDictionary)GetValue(SymbolMappingProperty); }
		}
		public SegmentsView() {
			this.SetValue(SymbolMappingPropertyKey, new SymbolDictionary(this));
		}
	}
}
namespace DevExpress.Xpf.Gauges.Native {
	public abstract class SymbolViewInternal : IOwnedElement, IModelSupported, IAnimatableElement, ILayoutCalculator {
		readonly List<SymbolInfo> symbols = new List<SymbolInfo>();
		object owner;
		bool animationInProgress = false;
		IntegerAnimationProgress progress = null;
		List<SymbolState> actualSymbolsState = null;
		SymbolsAnimation ActualAnimation {
			get {
				if (Animation != null)
					return Animation;
				return Gauge != null && Gauge.EnableAnimation ? GetDefaultAnimation() : null;
			}
		}
		protected SymbolViewBase View { get { return Gauge.ActualSymbolView; } }
		protected SymbolDictionary CustomSymbolMapping { get { return View.CustomSymbolMapping; } }
		protected SymbolOptions Options { get { return View.Options; } }
		protected SymbolsAnimation Animation { get { return View.Animation; } }
		internal bool ShouldAnimate {
			get {
				if (Gauge == null)
					return false;
				return Animation != null ? Animation.Enable : Gauge.EnableAnimation;
			}
		}
		internal IntegerAnimationProgress Progress { get { return progress; } }
		internal SymbolOptions ActualOptions {
			get {
				if (Options != null)
					return Options;
				if (ModelBase != null && ModelBase.Options != null)
					return ModelBase.Options;
				return new SymbolOptions();
			}
		}
		internal DigitalGaugeControl Gauge { get { return owner as DigitalGaugeControl; } }
		internal List<SymbolInfo> Symbols { get { return symbols; } }
		protected abstract SymbolPresentation ActualPresentation { get; }
		protected abstract SymbolsModelBase ModelBase { get; }
		internal abstract double DefaultHeightToWidthRatio { get; }
		protected SymbolViewInternal() {
			progress = new IntegerAnimationProgress(this);
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set { owner = value; }
		}
		#endregion
		#region IModelSupported implementation
		void IModelSupported.UpdateModel() {
			UpdateModel();
		}
		#endregion
		#region ILayoutCalculator implementation
		ElementLayout ILayoutCalculator.CreateLayout(Size constraint) {
			Size size = Gauge != null && Gauge.SymbolsLayout != null ? Gauge.SymbolsLayout.SymbolSize : new Size(0, 0);
			return new ElementLayout(size.Width, size.Height);
		}
		void ILayoutCalculator.CompleteLayout(ElementInfoBase elementInfo) {
			SymbolInfo symbolInfo = elementInfo as SymbolInfo;
			if (symbolInfo != null && symbolInfo.Layout != null) {
				Point location = Gauge == null || Gauge.SymbolsLayout == null ? new Point(0, 0) :
					Gauge.SymbolsLayout.GetSymbolLocation(Gauge.BaseLayoutElement, symbolInfo);
				Rect rect = Gauge == null || Gauge.SymbolsLayout == null ? new Rect(0, 0, 0, 0) :
					Gauge.SymbolsLayout.GetClipBounds(Gauge.BaseLayoutElement);
				symbolInfo.Layout.CompleteLayout(location, null, new RectangleGeometry() { Rect = new Rect(rect.Left - location.X, rect.Top - location.Y, rect.Width, rect.Height) });
			}
		}
		#endregion
		#region IAnimatableElement implementation
		bool IAnimatableElement.InProgress { get { return animationInProgress; } }
		void IAnimatableElement.ProgressChanged() {
			ApplyStatesToSymbols();
		}
		#endregion
		void Animate(bool firstPlay) {
			Gauge.Storyboard.Stop();
			Gauge.Storyboard.Children.Clear();
			ActualAnimation.Prepare(Gauge.Storyboard, this, actualSymbolsState, firstPlay);
			animationInProgress = true;
			progress.Start();
			if (Gauge.IsLoaded)
				Gauge.Storyboard.Begin();
		}
		void ApplyStatesToSymbols() {
			if (actualSymbolsState != null) {
				SymbolsAnimation animation = ActualAnimation;
				List<SymbolState> states = animation != null && animation.Enable ? animation.AnimateSymbolsStates(actualSymbolsState, this) : actualSymbolsState;
				for (int i = 0, j = states.Count - 1; i < symbols.Count; i++, j--) {
					int symbolIndex = Gauge.TextDirection == TextDirection.LeftToRight ? i : symbols.Count - 1 - i;
					int stateIndex = (Gauge.TextDirection == TextDirection.LeftToRight ? i : j);
					symbols[symbolIndex].SymbolState = states[stateIndex % states.Count];
					symbols[symbolIndex].DisplayText = states[stateIndex % states.Count].Symbol;
				}
			}
		}
		List<SymbolState> GetSymbolsStateByDisplayText(int symbolCount, List<string> textBySymbols) {
			List<SymbolState> symbolsState = GetSymbolsStateByDisplayText(Gauge.ActualSymbolCount, textBySymbols);
			return symbolsState;
		}
		void UpdateSymbolsStates() {
			List<string> textBySymbols = SeparateTextToSymbols(Gauge.Text);
			actualSymbolsState = GetSymbolsStateByDisplayText(textBySymbols);
			while (actualSymbolsState.Count < symbols.Count)
				if (Gauge.TextDirection == TextDirection.LeftToRight)
					actualSymbolsState.Add(GetEmptySymbolState());
				else
					actualSymbolsState.Insert(0, GetEmptySymbolState());
		}
		internal void UpdateModel() {
			foreach (SymbolInfo info in Symbols) {
				SymbolPresentation presentation = ActualPresentation;
				info.Presentation = presentation;
				info.PresentationControl = presentation.CreateSymbolPresentationControl();
			}
			OnOptionsChanged();
		}
		internal void OnOptionsChanged() {
			for (int i = 0; i < Symbols.Count; i++) {
				SymbolInfo info = Symbols[i];
				info.Margin = ActualOptions.Margin;
				info.RenderTransform = new SkewTransform() { AngleX = ActualOptions.SkewAngleX, AngleY = ActualOptions.SkewAngleY };
			}
		}
		protected SymbolSegmentsMapping GetCustomSegmentsMapping(char symbol) {
			foreach (SymbolSegmentsMapping mapping in CustomSymbolMapping)
				if (mapping.Symbol == symbol)
					return mapping;
			return null;
		}
		internal void Animate() {
			if (Gauge != null) {
				UpdateSymbolsStates();
				StopAnimation();
				if (ShouldAnimate)
					Animate(true);
			}
		}
		internal void Replay() {
			if (Gauge != null) {
				StopAnimation();
				if (ShouldAnimate && ActualAnimation.ShouldReplay) {
					UpdateSymbolsStates();
					Animate(false);
				}
				else {
					if (ActualAnimation != null)
						Gauge.Dispatcher.BeginInvoke(new Action(delegate { ActualAnimation.RaiseCompletedEvent(); }));
				}
			}
		}
		internal void StopAnimation() {
			animationInProgress = false;
		}
		internal void RecreateSymbols(int symbolCount) {
			if (actualSymbolsState != null)
				actualSymbolsState.Clear();
			foreach (SymbolInfo symbol in Symbols) {
				symbol.Presentation = null;
				symbol.SymbolState = null;
				symbol.PresentationControl = null;
			}
			Symbols.Clear();
			SymbolPresentation presentation = ActualPresentation;
			for (int symbolIndex = 0; symbolIndex < symbolCount; symbolIndex++)
				Symbols.Add(new SymbolInfo(this, presentation.CreateSymbolPresentationControl(), presentation, symbolIndex));
			UpdateSymbols();
		}
		internal void UpdateSymbols() {
			if (Gauge != null) {
				UpdateSymbolsStates();
				ApplyStatesToSymbols();
				OnOptionsChanged();
			}
		}
		protected abstract List<SymbolState> GetSymbolsStateByDisplayText(List<string> textBySymbols);
		protected abstract SymbolsAnimation GetDefaultAnimation();
		protected internal virtual SymbolState GetEmptySymbolState() {
			return new SymbolState(false);
		}
		protected internal abstract List<string> SeparateTextToSymbols(string text);
	}
	public abstract class SegmentsViewInternal : SymbolViewInternal {
		SymbolDictionary SymbolMapping { get { return ((SegmentsView)View).SymbolMapping; } }
		internal override double DefaultHeightToWidthRatio { get { return 2.0; } }
		SymbolSegmentsMapping GetSegmentsMapping(char symbol) {
			SymbolSegmentsMapping mapping = GetCustomSegmentsMapping(symbol);
			if (mapping != null)
				return mapping;
			else
				foreach (SymbolSegmentsMapping symbolMapping in SymbolMapping) {
					char upperSymbol = char.ToUpper(symbol);
					if (symbolMapping.Symbol == upperSymbol)
						return symbolMapping;
				}
			return null;
		}
		SymbolState GetSymbolStateBySymbolText(string symbolText) {
			SymbolState result = null;
			foreach (char symbol in symbolText) {
				SymbolSegmentsMapping mapping = GetSegmentsMapping(symbol);
				if (mapping != null)
					if (result == null)
						result = new SymbolState(symbol.ToString(), mapping.SegmentsStates.States);
					else
						result = result.Unite(new SymbolState(symbol.ToString(), mapping.SegmentsStates.States));
				else
					result = GetEmptySymbolState();
			}
			return result;
		}
		protected override SymbolsAnimation GetDefaultAnimation() {
			return new CreepingLineAnimation();
		}
		protected override List<SymbolState> GetSymbolsStateByDisplayText(List<string> textBySymbols) {
			List<SymbolState> result = new List<SymbolState>();
			foreach (string symbolText in textBySymbols)
				result.Add(GetSymbolStateBySymbolText(symbolText));
			return result;
		}
		protected internal override List<string> SeparateTextToSymbols(string text) {
			List<string> symbols = new List<string>();
			if (text != null) {
				string current = "";
				for (int i = 0; i < text.Length; i++) {
					SymbolSegmentsMapping mapping = GetSegmentsMapping(text[i]);
					if (mapping != null) {
						if (mapping.SymbolType == SymbolType.Main) {
							if (current != "")
								symbols.Add(current);
							current = text[i].ToString();
						}
						else
							if (current != "")
								current += text[i];
							else
								current = text[i].ToString();
					}
					else {
						if (current != "")
							symbols.Add(current);
						current = text[i].ToString();
					}
				}
				if (current != "")
					symbols.Add(current);
			}
			return symbols;
		}
	}
}
