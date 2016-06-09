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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Localization;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class Scale : GaugeElement, IWeakEventListener, IHitTestableElement, IModelSupported, ILayoutCalculator {
		const string StoryboardResourceKey = "storyboard";
		public static readonly DependencyProperty StartValueProperty = DependencyPropertyManager.Register("StartValue",
			typeof(double), typeof(Scale), new PropertyMetadata(0.0, UpdateElements));
		public static readonly DependencyProperty EndValueProperty = DependencyPropertyManager.Register("EndValue",
			typeof(double), typeof(Scale), new PropertyMetadata(100.0, UpdateElements));
		public static readonly DependencyProperty MajorTickmarkOptionsProperty = DependencyPropertyManager.Register("MajorTickmarkOptions",
			typeof(MajorTickmarkOptions), typeof(Scale), new PropertyMetadata(TickmarkOptionsPropertyChanged));
		public static readonly DependencyProperty MajorIntervalCountProperty = DependencyPropertyManager.Register("MajorIntervalCount",
			typeof(int), typeof(Scale), new PropertyMetadata(10, UpdateElements), MajorIntervalCountValidation);
		public static readonly DependencyProperty MinorTickmarkOptionsProperty = DependencyPropertyManager.Register("MinorTickmarkOptions",
			typeof(MinorTickmarkOptions), typeof(Scale), new PropertyMetadata(TickmarkOptionsPropertyChanged));
		public static readonly DependencyProperty MinorIntervalCountProperty = DependencyPropertyManager.Register("MinorIntervalCount",
			typeof(int), typeof(Scale), new PropertyMetadata(5, UpdateElements), MinorIntervalCountValidation);
		internal static readonly DependencyPropertyKey CustomLabelsPropertyKey = DependencyPropertyManager.RegisterReadOnly("CustomLabels",
			typeof(ScaleCustomLabelCollection), typeof(Scale), new PropertyMetadata(CustomLabelsPropertyChanged));
		public static readonly DependencyProperty CustomLabelsProperty = CustomLabelsPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey CustomElementsPropertyKey = DependencyPropertyManager.RegisterReadOnly("CustomElements",
			typeof(ScaleCustomElementCollection), typeof(Scale), new PropertyMetadata(CustomElementsPropertyChanged));
		public static readonly DependencyProperty CustomElementsProperty = CustomElementsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ShowLabelsProperty = DependencyPropertyManager.Register("ShowLabels",
		   typeof(DefaultBoolean), typeof(Scale), new PropertyMetadata(DefaultBoolean.Default, InvalidateLayout));
		public static readonly DependencyProperty ShowMajorTickmarksProperty = DependencyPropertyManager.Register("ShowMajorTickmarks",
		   typeof(DefaultBoolean), typeof(Scale), new PropertyMetadata(DefaultBoolean.Default, InvalidateLayout));
		public static readonly DependencyProperty ShowMinorTickmarksProperty = DependencyPropertyManager.Register("ShowMinorTickmarks",
		   typeof(DefaultBoolean), typeof(Scale), new PropertyMetadata(DefaultBoolean.Default, InvalidateLayout));
		public static readonly DependencyProperty LabelPresentationProperty = DependencyPropertyManager.Register("LabelPresentation",
			typeof(ScaleLabelPresentation), typeof(Scale), new PropertyMetadata(null, PresentationPropertyChanged));
		public static readonly DependencyProperty TickmarksPresentationProperty = DependencyPropertyManager.Register("TickmarksPresentation",
			typeof(TickmarksPresentation), typeof(Scale), new PropertyMetadata(null, PresentationPropertyChanged));
		public static readonly DependencyProperty LineOptionsProperty = DependencyPropertyManager.Register("LineOptions",
			typeof(ScaleLineOptions), typeof(Scale), new PropertyMetadata(null, LineOptionsPropertyChanged));
		public static readonly DependencyProperty ShowLineProperty = DependencyPropertyManager.Register("ShowLine",
			typeof(DefaultBoolean), typeof(Scale), new PropertyMetadata(DefaultBoolean.Default, InvalidateLayout));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleStartValue"),
#endif
		Category(Categories.Data)
		]
		public double StartValue {
			get { return (double)GetValue(StartValueProperty); }
			set { SetValue(StartValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleEndValue"),
#endif
		Category(Categories.Data)
		]
		public double EndValue {
			get { return (double)GetValue(EndValueProperty); }
			set { SetValue(EndValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleMajorTickmarkOptions"),
#endif
		Category(Categories.Presentation)
		]
		public MajorTickmarkOptions MajorTickmarkOptions {
			get { return (MajorTickmarkOptions)GetValue(MajorTickmarkOptionsProperty); }
			set { SetValue(MajorTickmarkOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleMajorIntervalCount"),
#endif
		Category(Categories.Presentation)
		]
		public int MajorIntervalCount {
			get { return (int)GetValue(MajorIntervalCountProperty); }
			set { SetValue(MajorIntervalCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleMinorTickmarkOptions"),
#endif
		Category(Categories.Presentation)
		]
		public MinorTickmarkOptions MinorTickmarkOptions {
			get { return (MinorTickmarkOptions)GetValue(MinorTickmarkOptionsProperty); }
			set { SetValue(MinorTickmarkOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleMinorIntervalCount"),
#endif
		Category(Categories.Presentation)
		]
		public int MinorIntervalCount {
			get { return (int)GetValue(MinorIntervalCountProperty); }
			set { SetValue(MinorIntervalCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleCustomLabels"),
#endif
		Category(Categories.Elements)
		]
		public ScaleCustomLabelCollection CustomLabels {
			get { return (ScaleCustomLabelCollection)GetValue(CustomLabelsProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleCustomElements"),
#endif
		Category(Categories.Elements)
		]
		public ScaleCustomElementCollection CustomElements {
			get { return (ScaleCustomElementCollection)GetValue(CustomElementsProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleShowLabels"),
#endif
		Category(Categories.Behavior)
		]
		public DefaultBoolean ShowLabels {
			get { return (DefaultBoolean)GetValue(ShowLabelsProperty); }
			set { SetValue(ShowLabelsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleShowMajorTickmarks"),
#endif
		Category(Categories.Behavior)
		]
		public DefaultBoolean ShowMajorTickmarks {
			get { return (DefaultBoolean)GetValue(ShowMajorTickmarksProperty); }
			set { SetValue(ShowMajorTickmarksProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleShowMinorTickmarks"),
#endif
		Category(Categories.Behavior)
		]
		public DefaultBoolean ShowMinorTickmarks {
			get { return (DefaultBoolean)GetValue(ShowMinorTickmarksProperty); }
			set { SetValue(ShowMinorTickmarksProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleLabelPresentation"),
#endif
		Category(Categories.Presentation)
		]
		public ScaleLabelPresentation LabelPresentation {
			get { return (ScaleLabelPresentation)GetValue(LabelPresentationProperty); }
			set { SetValue(LabelPresentationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleTickmarksPresentation"),
#endif
		Category(Categories.Presentation)
		]
		public TickmarksPresentation TickmarksPresentation {
			get { return (TickmarksPresentation)GetValue(TickmarksPresentationProperty); }
			set { SetValue(TickmarksPresentationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleShowLine"),
#endif
		Category(Categories.Behavior)
		]
		public DefaultBoolean ShowLine {
			get { return (DefaultBoolean)GetValue(ShowLineProperty); }
			set { SetValue(ShowLineProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleLineOptions"),
#endif
		Category(Categories.Presentation)
		]
		public ScaleLineOptions LineOptions {
			get { return (ScaleLineOptions)GetValue(LineOptionsProperty); }
			set { SetValue(LineOptionsProperty, value); }
		}
		static void CustomLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Scale scale = d as Scale;
			if (scale != null) {
				ScaleCustomLabelCollection oldCollection = e.OldValue as ScaleCustomLabelCollection;
				if (oldCollection != null)
					((INotifyCollectionChanged)oldCollection).CollectionChanged -= scale.CustomLabelsCollectionChanged;
				ScaleCustomLabelCollection newCollection = e.NewValue as ScaleCustomLabelCollection;
				if (newCollection != null)
					((INotifyCollectionChanged)newCollection).CollectionChanged += scale.CustomLabelsCollectionChanged;
			}
		}
		static void CustomElementsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Scale scale = d as Scale;
			if (scale != null) {
				ScaleCustomElementCollection oldCollection = e.OldValue as ScaleCustomElementCollection;
				if (oldCollection != null)
					((INotifyCollectionChanged)oldCollection).CollectionChanged -= scale.CustomElementsCollectionChanged;
				ScaleCustomElementCollection newCollection = e.NewValue as ScaleCustomElementCollection;
				if (newCollection != null)
					((INotifyCollectionChanged)newCollection).CollectionChanged += scale.CustomElementsCollectionChanged;
			}
		}
		static void TickmarkOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Scale scale = d as Scale;
			if (scale != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as TickmarkOptions, e.NewValue as TickmarkOptions, scale);
				scale.OnTickmarkChanged();
			}
		}
		static bool MajorIntervalCountValidation(object value) {
			return (int)value > 0;
		}
		static bool MinorIntervalCountValidation(object value) {
			return (int)value > 0;
		}
		protected static void UpdateElements(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Scale scale = d as Scale;
			if (scale != null)
				scale.UpdateElementsInfo();
		}
		protected static void InvalidateLayout(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Scale scale = d as Scale;
			if (scale != null)
				scale.Invalidate();
		}
		protected static void PresentationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Scale scale = d as Scale;
			if (scale != null && !Object.Equals(e.NewValue, e.OldValue))
				scale.UpdateModel();
		}
		protected static void LabelOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Scale scale = d as Scale;
			if (scale != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ScaleLabelOptions, e.NewValue as ScaleLabelOptions, scale);
				scale.OnLabelChanged();
			}
		}
		static void LineOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Scale scale = d as Scale;
			if (scale != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ScaleLineOptions, e.NewValue as ScaleLineOptions, scale);
				scale.OnLineOptionsChanged();
			}
		}
		readonly ScaleElementsInfo minorTickmarksInfo;
		readonly ScaleElementsInfo majorTickmarksInfo;
		readonly ScaleElementsInfo labelsInfo;
		readonly ScaleElementsInfo lineInfo;
		ScaleLayoutControl layoutControl = null;
		ScaleMapping mapping = null;
		bool shouldElementsUpdate = false;
		bool ActualShowLabels {
			get {
				if (ShowLabels == DefaultBoolean.Default && Model != null)
					return Model.ShowLabels;
				return ShowLabels != DefaultBoolean.False;
			}
		}
		bool ActualShowMajorTickmarks {
			get {
				if (ShowMajorTickmarks == DefaultBoolean.Default && Model != null)
					return Model.ShowMajorTickmarks;
				return ShowMajorTickmarks != DefaultBoolean.False;
			}
		}
		bool ActualShowMinorTickmarks {
			get {
				if (ShowMinorTickmarks == DefaultBoolean.Default && Model != null)
					return Model.ShowMinorTickmarks;
				return ShowMinorTickmarks != DefaultBoolean.False;
			}
		}
		protected bool ActualShowLine {
			get {
				if (ShowLine == DefaultBoolean.Default && Model != null)
					return Model.ShowLine;
				return ShowLine != DefaultBoolean.False;
			}
		}
		ScaleLabelPresentation ActualLabelPresentation {
			get {
				if (LabelPresentation != null)
					return LabelPresentation;
				if (Model != null && Model.LabelPresentation != null)
					return Model.LabelPresentation;
				return new DefaultScaleLabelPresentation();
			}
		}
		internal TickmarksPresentation ActualTickmarksPresentation {
			get {
				if (TickmarksPresentation != null)
					return TickmarksPresentation;
				if (Model != null && Model.TickmarksPresentation != null)
					return Model.TickmarksPresentation;
				return new DefaultTickmarksPresentation();
			}
		}
		internal abstract ScaleLinePresentation ActualLinePresentation { get; }
		protected abstract ScaleModel Model { get; }
		protected internal abstract ScaleLabelOptions ActualLabelOptions { get; }
		protected internal abstract IEnumerable<IElementInfo> Elements { get; }
		protected internal abstract IEnumerable<ValueIndicatorBase> Indicators { get; }
		internal MajorTickmarkOptions ActualMajorTickmarkOptions {
			get {
				if (MajorTickmarkOptions != null)
					return MajorTickmarkOptions;
				if (Model != null && Model.MajorTickmarkOptions != null)
					return Model.MajorTickmarkOptions;
				return new MajorTickmarkOptions();
			}
		}
		internal MinorTickmarkOptions ActualMinorTickmarkOptions {
			get {
				if (MinorTickmarkOptions != null)
					return MinorTickmarkOptions;
				if (Model != null && Model.MinorTickmarkOptions != null)
					return Model.MinorTickmarkOptions;
				return new MinorTickmarkOptions();
			}
		}
		internal ScaleLineOptions ActualLineOptions {
			get {
				if (LineOptions != null)
					return LineOptions;
				if (Model != null && Model.LineOptions != null)
					return Model.LineOptions;
				return new ScaleLineOptions();
			}
		}
		internal ScaleElementsInfo LineInfo { get { return lineInfo; } }
		internal IEnumerable<ScaleLineInfo> Lines {
			get {
				if (lineInfo != null) {
					foreach (object info in lineInfo.Elements)
						yield return (ScaleLineInfo)info;
				}
			}
		}
		internal double ValuesRange { get { return EndValue - StartValue; } }
		internal AnalogGaugeControl Gauge { get { return Owner as AnalogGaugeControl; } }
		internal ScaleMapping Mapping { get { return mapping; } }
		internal ScaleLayoutControl LayoutControl {
			get { return layoutControl; }
			set { layoutControl = value; }
		}
		internal IEnumerable<ScaleLabelInfo> Labels {
			get {
				if (labelsInfo != null) {
					foreach (object info in labelsInfo.Elements)
						if (info is ScaleLabelInfo)
							yield return (ScaleLabelInfo)info;
				}
			}
		}
		internal IEnumerable<MinorTickmarkInfo> MinorTickmarks {
			get {
				if (minorTickmarksInfo != null) {
					foreach (object info in minorTickmarksInfo.Elements)
						yield return (MinorTickmarkInfo)info;
				}
			}
		}
		internal IEnumerable<MajorTickmarkInfo> MajorTickmarks {
			get {
				if (majorTickmarksInfo != null) {
					foreach (object info in majorTickmarksInfo.Elements)
						yield return (MajorTickmarkInfo)info;
				}
			}
		}
		internal ScaleElementsInfo MinorTickmarksInfo { get { return minorTickmarksInfo; } }
		internal ScaleElementsInfo MajorTickmarksInfo { get { return majorTickmarksInfo; } }
		internal ScaleElementsInfo LabelsInfo { get { return labelsInfo; } }
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("ScalePredefinedLabelPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedLabelPresentations {
			get { return PredefinedScaleLabelPresentations.PresentationKinds; }
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("ScalePredefinedTickmarksPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedTickmarksPresentations {
			get { return DevExpress.Xpf.Gauges.Native.PredefinedTickmarksPresentations.PresentationKinds; }
		}
		public Scale() {
			lineInfo = new ScaleElementsInfo(this, ActualLineOptions.ZIndex);
			labelsInfo = new ScaleElementsInfo(this, ActualLabelOptions.ZIndex);
			this.SetValue(CustomLabelsPropertyKey, new ScaleCustomLabelCollection(this));
			this.SetValue(CustomElementsPropertyKey, new ScaleCustomElementCollection(this));
			minorTickmarksInfo = new ScaleElementsInfo(this, ActualMinorTickmarkOptions.ZIndex);
			majorTickmarksInfo = new ScaleElementsInfo(this, ActualMajorTickmarkOptions.ZIndex);
			LayoutUpdated += new EventHandler(ScaleLayoutUpdated);
		}
		#region IHitTestableElement implementation
		Object IHitTestableElement.Element { get { return this; } }
		Object IHitTestableElement.Parent { get { return null; } }
		#endregion
		#region IModelSupported implementation
		void IModelSupported.UpdateModel() {
			UpdateModel();
		}
		#endregion
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		#region ILayoutCalculator implementation
		ElementLayout ILayoutCalculator.CreateLayout(Size constraint) {
			return Mapping != null ? new ElementLayout(Mapping.Layout.InitialBounds.Width, Mapping.Layout.InitialBounds.Height) : null;
		}
		void ILayoutCalculator.CompleteLayout(ElementInfoBase elementInfo) {
			elementInfo.Layout.CompleteLayout(GetLayoutOffset(), null, null);
		}
		#endregion
		void UpdateMajorTickmarksInfo(ScaleElementsInfo tickmarksInfo) {
			if (tickmarksInfo != null) {
				tickmarksInfo.Elements.Clear();
				double majorValueStep = MajorIntervalCount > 0 ? (EndValue - StartValue) / MajorIntervalCount : EndValue - StartValue;
				double majorAlphaStep = MajorIntervalCount > 0 ? 1.0 / MajorIntervalCount : 0;
				for (int i = 0; i <= MajorIntervalCount; i++) {
					PresentationControl majorTickmarkModelControl = ActualTickmarksPresentation.CreateMajorTickPresentationControl();
					MajorTickmarkInfo majorTickmark = new MajorTickmarkInfo(majorTickmarkModelControl, ActualTickmarksPresentation, i * majorAlphaStep, StartValue + i * majorValueStep, i == 0, i == MajorIntervalCount);
					tickmarksInfo.Elements.Add(majorTickmark);
				}
			}
		}
		void UpdateMinorTickmarksInfo(ScaleElementsInfo tickmarksInfo) {
			if (tickmarksInfo != null) {
				tickmarksInfo.Elements.Clear();
				int tickCount = MajorIntervalCount * MinorIntervalCount + 1;
				double minorAngleStep = tickCount > 1 ? 1.0 / (tickCount - 1) : 1;
				for (int i = 0; i < tickCount; i++) {
					PresentationControl minorTickmarkModelControl = ActualTickmarksPresentation.CreateMinorTickPresentationControl();
					tickmarksInfo.Elements.Add(new MinorTickmarkInfo(minorTickmarkModelControl, ActualTickmarksPresentation, i * minorAngleStep, (i % MinorIntervalCount) == 0));
				}
			}
		}
		void UpdateLineInfo() {
			if (lineInfo != null) {
				lineInfo.Elements.Clear();
				lineInfo.Elements.Add(new ScaleLineInfo(ActualLinePresentation.CreateLinePresentationControl(), ActualLinePresentation));
			}
		}
		void OnTickmarkChanged() {
			if (minorTickmarksInfo != null && majorTickmarksInfo != null) {
				majorTickmarksInfo.ZIndex = ActualMajorTickmarkOptions.ZIndex;
				Invalidate();
			}
		}
		void OnLineOptionsChanged() {
			if (lineInfo != null) {
				lineInfo.ZIndex = ActualLineOptions.ZIndex;
				Invalidate();
			}
		}
		void UpdateLabelsInfo() {
			if (labelsInfo != null) {
				labelsInfo.Elements.Clear();
				foreach (MajorTickmarkInfo elementInfo in MajorTickmarks) {
					PresentationControl presentation = ActualLabelPresentation.CreateLabelPresentationControl();
					double displayValue = elementInfo.Value * ActualLabelOptions.Multiplier + ActualLabelOptions.Addend;
					labelsInfo.Elements.Add(new ScaleLabelInfo(presentation, ActualLabelPresentation, elementInfo, String.Format(ActualLabelOptions.FormatString, displayValue)));
				}
			}
		}
		void OnLabelChanged() {
			if (labelsInfo != null) {
				labelsInfo.ZIndex = ActualLabelOptions.ZIndex;
				foreach (ScaleLabelInfo elementInfo in Labels) {
					double displayValue = elementInfo.Tickmark.Value * ActualLabelOptions.Multiplier + ActualLabelOptions.Addend;
					elementInfo.Text = String.Format(ActualLabelOptions.FormatString, displayValue);
				}
				Invalidate();
			}
		}
		void CustomLabelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ScaleCustomLabelCollection collection = sender as ScaleCustomLabelCollection;
			if (collection != null) {
				if ((e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace) && e.NewItems != null)
					foreach (ScaleCustomLabel label in e.NewItems) {
						if (label != null)
							((IOwnedElement)label).Owner = this;
					}
			}
			UpdateElementsInfo();
		}
		void CustomElementsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ScaleCustomElementCollection collection = sender as ScaleCustomElementCollection;
			if (collection != null) {
				if ((e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace) && e.NewItems != null)
					foreach (ScaleCustomElement element in e.NewItems) {
						if (element != null)
							((IOwnedElement)element).Owner = this;
					}
			}
			UpdateElementsInfo();
		}
		void ScaleLayoutUpdated(object sender, EventArgs e) {
			if (shouldElementsUpdate) {
				try {
					foreach (IElementInfo elementInfo in Elements)
						elementInfo.Invalidate();
				}
				finally {
					shouldElementsUpdate = false;
				}
			}
		}
		void CalculateElementsLayout() {
			foreach (TickmarkInfo elementInfo in MinorTickmarks)
				elementInfo.Layout = ActualShowMinorTickmarks ? ActualMinorTickmarkOptions.CalculateLayout(elementInfo, Mapping) : null;
			foreach (TickmarkInfo elementInfo in MajorTickmarks)
				elementInfo.Layout = ActualShowMajorTickmarks ? ActualMajorTickmarkOptions.CalculateLayout(elementInfo, Mapping) : null;
			foreach (ScaleLabelInfo elementInfo in Labels)
				elementInfo.Layout = ActualShowLabels ? ActualLabelOptions.CalculateLayout(elementInfo, Mapping) : null;
			foreach (ScaleCustomLabel label in CustomLabels)
				label.CalculateLayout(Mapping);
			foreach (ScaleCustomElement element in CustomElements)
				element.CalculateLayout(Mapping);
			foreach (ScaleLineInfo elementInfo in Lines)
				elementInfo.Layout = CalculateLineLayout();
		}
		protected override void OwnerChanged() {
			base.OwnerChanged();
			((IModelSupported)this).UpdateModel();
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			RequestUpdate();
			return base.ArrangeOverride(arrangeBounds);
		}
		protected virtual bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if ((sender is TickmarkOptions))
					OnTickmarkChanged();
				if ((sender is ScaleLabelOptions))
					OnLabelChanged();
				if (sender is ScaleLineOptions)
					OnLineOptionsChanged();
				success = true;
			}
			return success;
		}
		protected abstract ScaleElementLayout CalculateLineLayout();
		protected virtual void UpdateModel() {
			ScaleLinePresentation linePresentation = ActualLinePresentation;
			foreach (ScaleLineInfo line in Lines) {
				line.Presentation = linePresentation;
				line.PresentationControl = linePresentation.CreateLinePresentationControl();
			}
			TickmarksPresentation tickmarksPresentation = ActualTickmarksPresentation;
			foreach (MajorTickmarkInfo majorTick in MajorTickmarks) {
				majorTick.Presentation = tickmarksPresentation;
				majorTick.PresentationControl = tickmarksPresentation.CreateMajorTickPresentationControl();
			}
			foreach (MinorTickmarkInfo minorTick in MinorTickmarks) {
				minorTick.Presentation = tickmarksPresentation;
				minorTick.PresentationControl = tickmarksPresentation.CreateMinorTickPresentationControl();
			}
			OnTickmarkChanged();
			OnLineOptionsChanged();
			ScaleLabelPresentation labelPresentation = ActualLabelPresentation;
			foreach (ScaleLabelInfo label in Labels)
				label.PresentationControl = labelPresentation.CreateLabelPresentationControl();
			OnLabelChanged();
			foreach (ValueIndicatorBase indicator in Indicators)
				((IModelSupported)indicator).UpdateModel();
		}
		protected abstract ScaleMapping CalculateMapping(Size constraint);
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new ScaleAutomationPeer(this);
		}
		protected internal virtual void UpdateElementsInfo() {
			UpdateMinorTickmarksInfo(minorTickmarksInfo);
			UpdateMajorTickmarksInfo(majorTickmarksInfo);
			UpdateLabelsInfo();
			UpdateLineInfo();
			Invalidate();
		}
		protected internal abstract void CheckIndicatorEnterLeaveRange(ValueIndicatorBase indicator, double oldValue, double newValue);
		internal void AnimateIndicators(bool shouldResetValue) {
			foreach (ValueIndicatorBase indicator in Indicators)
				indicator.Animate(shouldResetValue);
		}
		internal void AddStoryboard(Storyboard storyboard, int resourceKey) {
			if (storyboard != null && !Resources.Contains(resourceKey.ToString()))
				Resources.Add(resourceKey.ToString(), storyboard);
		}
		internal void RemoveStoryboard(int resourceKey) {
			if (Resources.Contains(resourceKey.ToString()))
				Resources.Remove(resourceKey.ToString());
		}
		internal double GetValueInPercent(double value) {
			double diff = value - StartValue;
			return (ValuesRange != 0) ? diff / ValuesRange : 0;
		}
		internal double GetLimitedValueInPercent(double value) {
			return GetValueInPercent(GetLimitedValue(value));
		}
		internal double GetLimitedValue(double value) {
			double minValue = Math.Min(StartValue, EndValue);
			double maxValue = Math.Max(StartValue, EndValue);
			if (value < minValue)
				return minValue;
			else
				if (value > maxValue)
					return maxValue;
			return value;
		}
		internal ScaleLayout CalculateLayout(Size constraint) {
			mapping = CalculateMapping(constraint);
			CalculateElementsLayout();
			RequestUpdate();
			return mapping.Layout;
		}
		internal Point GetLayoutOffset() {
			if (LayoutControl != null && Gauge != null && Gauge.BaseLayoutElement != null) {
				Rect rect = LayoutHelper.GetRelativeElementRect(LayoutControl, Gauge.BaseLayoutElement);
				return new Point(rect.X, rect.Y);
			}
			return new Point(0, 0);
		}
		internal double? GetValueByMousePosition(MouseEventArgs e) {
			Point point = e.GetPosition(LayoutControl);
			return Mapping.GetValueByPoint(point);
		}
		internal void Invalidate() {
			if (layoutControl != null)
				layoutControl.InvalidateMeasure();
		}
		internal void RequestUpdate() {
			shouldElementsUpdate = true;
		}
		internal void ClearAnimation() {
			foreach (ValueIndicatorBase indicator in Indicators) {
				indicator.ClearAnimation();
			}
		}
	}
	public class ScaleAutomationPeer : FrameworkElementAutomationPeer {
		public ScaleAutomationPeer(FrameworkElement owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			return "Scale";
		}
		protected override string GetLocalizedControlTypeCore() {
			return GaugeLocalizer.GetString(GaugeStringId.ScaleLocalizedControlType);
		}
		protected override bool IsContentElementCore() {
			return false;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			Scale scale = Owner as Scale;
			if (scale != null)
				foreach (ValueIndicatorBase indicator in scale.Indicators) {
					AutomationPeer indicatorPeer = FrameworkElementAutomationPeer.CreatePeerForElement(indicator.ElementInfo.PresentationControl);
					if (indicatorPeer != null)
						children.Add(indicatorPeer);
				}
			return children;
		}
	}
	public abstract class ScaleCollection<T> : GaugeElementCollection<T> where T : Scale {
		AnalogGaugeControl Gauge { get { return Owner as AnalogGaugeControl; } }
		public ScaleCollection(AnalogGaugeControl gauge) {
			((IOwnedElement)this).Owner = gauge;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (Gauge != null)
				Gauge.UpdateElements();
		}
	}
}
