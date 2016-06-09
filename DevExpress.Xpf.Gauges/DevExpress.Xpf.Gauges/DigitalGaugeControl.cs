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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Gauges.Localization;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public enum TextHorizontalAlignment { 
		Left, 
		Center,
		Right
	}
	public enum TextVerticalAlignment {
		Top,
		Center,
		Bottom
	}
	public enum TextDirection { 
		LeftToRight,
		RightToLeft
	}
	[DXToolboxBrowsableAttribute]
	public class DigitalGaugeControl : GaugeControlBase {
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model",
			typeof(DigitalGaugeModel), typeof(DigitalGaugeControl), new PropertyMetadata(null, ModelProperytChanged));
		internal static readonly DependencyPropertyKey LayersPropertyKey = DependencyPropertyManager.RegisterReadOnly("Layers",
			typeof(DigitalGaugeLayerCollection), typeof(DigitalGaugeControl), new PropertyMetadata());
		public static readonly DependencyProperty LayersProperty = LayersPropertyKey.DependencyProperty;
		public static readonly DependencyProperty TextProperty = DependencyPropertyManager.Register("Text",
			typeof(string), typeof(DigitalGaugeControl), new PropertyMetadata("00.000", TextPropertyChanged));
		public static readonly DependencyProperty SymbolCountProperty = DependencyPropertyManager.Register("SymbolCount",
			typeof(int), typeof(DigitalGaugeControl), new PropertyMetadata(0, SymbolCountPropertyChanged), CountValidation);
		public static readonly DependencyProperty SymbolViewProperty = DependencyPropertyManager.Register("SymbolView",
			typeof(SymbolViewBase), typeof(DigitalGaugeControl), new PropertyMetadata(null, SymbolViewPropertyChanged));
		public static readonly DependencyProperty TextHorizontalAlignmentProperty = DependencyPropertyManager.Register("TextHorizontalAlignment",
			typeof(TextHorizontalAlignment), typeof(DigitalGaugeControl), new PropertyMetadata(TextHorizontalAlignment.Center, TextAlignmentPropertyChanged));
		public static readonly DependencyProperty TextVerticalAlignmentProperty = DependencyPropertyManager.Register("TextVerticalAlignment",
			typeof(TextVerticalAlignment), typeof(DigitalGaugeControl), new PropertyMetadata(TextVerticalAlignment.Center, TextAlignmentPropertyChanged));
		public static readonly DependencyProperty TextDirectionProperty = DependencyPropertyManager.Register("TextDirection",
			typeof(TextDirection), typeof(DigitalGaugeControl), new PropertyMetadata(TextDirection.LeftToRight, TextDirectionPropertyChanged));
		static readonly DependencyPropertyKey ActualModelPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualModel",
			typeof(DigitalGaugeModel), typeof(DigitalGaugeControl), new PropertyMetadata(null, ActualModelProperytChanged));
		public static readonly DependencyProperty ActualModelProperty = ActualModelPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey ActualSymbolViewPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualSymbolView",
			typeof(SymbolViewBase), typeof(DigitalGaugeControl), new PropertyMetadata(null, ActualSymbolViewPropertyChanged));
		public static readonly DependencyProperty ActualSymbolViewProperty = ActualSymbolViewPropertyKey.DependencyProperty;
		[
		Category(Categories.Presentation)
		]
		public DigitalGaugeModel Model {
			get { return (DigitalGaugeModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		[		
		Category(Categories.Elements)
		]
		public DigitalGaugeLayerCollection Layers {
			get { return (DigitalGaugeLayerCollection)GetValue(LayersProperty); }
		}
		[Category(Categories.Data)]
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		[Category(Categories.Presentation)]
		public int SymbolCount {
			get { return (int)GetValue(SymbolCountProperty); }
			set { SetValue(SymbolCountProperty, value); }
		}
		[Category(Categories.Presentation)]
		public SymbolViewBase SymbolView {
			get { return (SymbolViewBase)GetValue(SymbolViewProperty); }
			set { SetValue(SymbolViewProperty, value); }
		}
		[Category(Categories.Behavior)]
		public TextDirection TextDirection {
			get { return (TextDirection)GetValue(TextDirectionProperty); }
			set { SetValue(TextDirectionProperty, value); }
		}
		[Category(Categories.Layout)]
		public TextHorizontalAlignment TextHorizontalAlignment {
			get { return (TextHorizontalAlignment)GetValue(TextHorizontalAlignmentProperty); }
			set { SetValue(TextHorizontalAlignmentProperty, value); }
		}
		[Category(Categories.Layout)]
		public TextVerticalAlignment TextVerticalAlignment {
			get { return (TextVerticalAlignment)GetValue(TextVerticalAlignmentProperty); }
			set { SetValue(TextVerticalAlignmentProperty, value); }
		}		
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public SymbolViewBase ActualSymbolView {
			get { return (SymbolViewBase)GetValue(ActualSymbolViewProperty); }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DigitalGaugeModel ActualModel {
			get { return (DigitalGaugeModel)GetValue(ActualModelProperty); }
		}
		static DigitalGaugeControl() {
		}
		static void TextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DigitalGaugeControl gauge = d as DigitalGaugeControl;
			if (gauge != null) 
				gauge.InvalidateLayout();			
		}
		static void ModelProperytChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DigitalGaugeControl gauge = d as DigitalGaugeControl;
			if (gauge != null) {
				DigitalGaugeModel model = e.NewValue as DigitalGaugeModel;
				if (model == null)
					gauge.SetValue(DigitalGaugeControl.ActualModelPropertyKey, new DigitalDefaultModel());
				else
					gauge.SetValue(DigitalGaugeControl.ActualModelPropertyKey, model);
			}
		}
		static void ActualModelProperytChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			IModelSupported obj = d as IModelSupported;
			IOwnedElement model = e.NewValue as IOwnedElement;
			if (model != null)
				model.Owner = d as DigitalGaugeControl;
			if (obj != null)
				obj.UpdateModel();
		}
		static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DigitalGaugeControl gauge = d as DigitalGaugeControl;
			if (gauge != null)
				if (gauge.AutoSymbolsGeniration)
					gauge.UpdateSymbols();
				else {
					gauge.SymbolViewInternal.UpdateSymbols();
					gauge.Animate();
				}
		}
		static void TextDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DigitalGaugeControl gauge = d as DigitalGaugeControl;
			if (gauge != null)
				gauge.UpdateSymbols();
		}
		static void SymbolCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DigitalGaugeControl gauge = d as DigitalGaugeControl;
			if (gauge != null)
				gauge.UpdateSymbols();
		}
		static void SymbolViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DigitalGaugeControl gauge = d as DigitalGaugeControl;
			if (gauge != null) {
				SymbolViewBase view = e.NewValue as SymbolViewBase;
				if (view == null)
					gauge.SetValue(ActualSymbolViewPropertyKey, new SevenSegmentsView());
				else
					gauge.SetValue(ActualSymbolViewPropertyKey, view);
			}
		}
		static void ActualSymbolViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DigitalGaugeControl gauge = d as DigitalGaugeControl;
			if (gauge != null && !Object.ReferenceEquals(e.NewValue, e.OldValue)) {
				SymbolViewBase options = e.NewValue as SymbolViewBase;
				if (options != null) {
					gauge.SymbolViewInternal = options.CreateInternalView();
					((IOwnedElement)gauge.SymbolViewInternal).Owner = gauge;
					((IOwnedElement)options).Owner = gauge;
				}
				gauge.UpdateSymbols();
			}
		}
		static bool CountValidation(object value) {
			return (int)value > -1;
		}
		public static List<PredefinedElementKind> PredefinedModels {
			get { return PredefinedDigitalGaugeModels.ModelKinds; }
		}
		const string StoryboardResourceKey = "Storyboard";
		SymbolsLayout symbolslayout = null;
		Storyboard storyboard = null;
		SymbolViewInternal symbolViewInternal = null;
		Panel ElementsPanel { get { return CommonUtils.GetChildPanel(GetTemplateChild("PART_Elements") as ItemsControl); } }
		internal bool AutoSymbolsGeniration { get { return SymbolCount < 1; } }
		internal List<string> TextBySymbols { get { return SymbolViewInternal.SeparateTextToSymbols(Text); } }
		internal SymbolViewInternal SymbolViewInternal {
			get { return symbolViewInternal; }
			private set { symbolViewInternal = value; }
		}
		internal int ActualSymbolCount { 
			get {
				if (!AutoSymbolsGeniration)
					return SymbolCount;
				int count = TextBySymbols.Count;
				return count > 0 ? count : 1;
			} 
		}
		internal SymbolsLayout SymbolsLayout { 
			get { return symbolslayout; } 
			set { 
				symbolslayout = value;
				InvalidateLayout();
			} 
		}
		internal Storyboard Storyboard {
			get {
				if (storyboard == null) {
					storyboard = new Storyboard();
					storyboard.Completed += new EventHandler(OnStoryboardCompleted);
					Resources.Add(StoryboardResourceKey, storyboard);
				}
				return storyboard;
			}
		}
		public DigitalGaugeControl() {
			DefaultStyleKey = typeof(DigitalGaugeControl);
			this.SetValue(ActualModelPropertyKey, new DigitalDefaultModel());
			this.SetValue(ActualSymbolViewPropertyKey, new SevenSegmentsView());
			this.SetValue(LayersPropertyKey, new DigitalGaugeLayerCollection(this));
			UpdateSymbols();
		}
		void OnStoryboardCompleted(object sender, EventArgs e) {
			SymbolViewInternal.Replay();
		}
		void UpdateSymbols() {
			SymbolViewInternal.RecreateSymbols(ActualSymbolCount);
			UpdateElements();
			InvalidateLayout();
			Animate();
		}
		internal void InvalidateLayout() {
			foreach (IElementInfo elementInfo in Elements)
				elementInfo.Invalidate();
			if (SymbolsLayout != null)
				SymbolsLayout.Invalidate();
			if (ElementsPanel != null)
				ElementsPanel.InvalidateMeasure();			
		}
		internal void ViewOptionsChanged() {
			SymbolViewInternal.OnOptionsChanged();
		}
		internal void UpdateViewSymbols() {
			SymbolViewInternal.UpdateSymbols();
		}
		internal void UpdateSymbolsModels() {
			SymbolViewInternal.UpdateModel();
		}
		protected internal override void Animate() {
			SymbolViewInternal.Animate();
		}
		protected override IEnumerable<IElementInfo> GetElements() {
			if (Layers != null)
				foreach (DigitalGaugeLayer layer in Layers)
					yield return layer.ElementInfo;
			foreach (SymbolInfo info in SymbolViewInternal.Symbols)
				yield return info;
		}
		protected override void UpdateModel() {
			if(SymbolViewInternal != null)
				((IModelSupported)SymbolViewInternal).UpdateModel();
			if (Layers != null)
				foreach (DigitalGaugeLayer layer in Layers)
					((IModelSupported)layer).UpdateModel();
		}
		protected override void GaugeLoaded(object sender, RoutedEventArgs e) {
			Dispatcher.BeginInvoke(new Action(delegate { Animate(); }));
			base.GaugeLoaded(sender, e);
		}
		protected override void GaugeUnloaded(object sender, RoutedEventArgs e) {
			base.GaugeUnloaded(sender, e);
			if (storyboard != null) {
				storyboard.Stop();
				storyboard.Completed -= OnStoryboardCompleted;
				Resources.Remove(StoryboardResourceKey);
				storyboard = null;
			}
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new DigitalGaugeControlAutomationPeer(this);
		}
	}
	public class DigitalGaugeControlAutomationPeer : FrameworkElementAutomationPeer, IValueProvider {
		DigitalGaugeControl Gauge {
			get { return Owner as DigitalGaugeControl; }
		}
		public DigitalGaugeControlAutomationPeer(FrameworkElement owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			return "DigitalGaugeControl";
		}
		protected override string GetLocalizedControlTypeCore() {
			return GaugeLocalizer.GetString(GaugeStringId.DigitalGaugeLocalizedControlType);
		}
		protected override bool IsContentElementCore() {
			return false;
		}
		protected override string GetHelpTextCore() {
			string helpTextBase = base.GetHelpTextCore();
			if (String.IsNullOrEmpty(helpTextBase))
				return GaugeLocalizer.GetString(GaugeStringId.DigitalGaugeAutomationPeerHelpText);
			else
				return helpTextBase;
		}
		#region IValueProvider implementation
		bool IValueProvider.IsReadOnly {
			get { return false; }
		}
		string IValueProvider.Value {
			get {
				if (Gauge != null)
					return Gauge.Text;
				else
					return String.Empty;
			}
		}
		void IValueProvider.SetValue (string value) {
			if (Gauge != null)
				Gauge.Text = value;
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			if (patternInterface == PatternInterface.Value)
				return this;
			return base.GetPattern(patternInterface);
		}
	}
}
