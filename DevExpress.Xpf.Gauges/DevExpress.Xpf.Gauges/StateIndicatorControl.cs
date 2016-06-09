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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Gauges.Localization;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	[DXToolboxBrowsableAttribute]
	public class StateIndicatorControl : Control, IModelSupported, ILayoutCalculator {
		const double defaultWidth = 250.0;
		const double defaultHeight = 250.0;
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model",
			typeof(StateIndicatorModel), typeof(StateIndicatorControl), new PropertyMetadata(null, ModelPropertyChanged));
		public static readonly DependencyProperty StateIndexProperty = DependencyPropertyManager.Register("StateIndex",
			typeof(int), typeof(StateIndicatorControl), new PropertyMetadata(0, StateIndexPropertyChanged));
		public static readonly DependencyProperty DefaultStateProperty = DependencyPropertyManager.Register("DefaultState",
			typeof(State), typeof(StateIndicatorControl), new PropertyMetadata(null, DefaultStatePropertyChanged));
		static readonly DependencyPropertyKey StatePropertyKey = DependencyPropertyManager.RegisterReadOnly("State",
			typeof(State), typeof(StateIndicatorControl), new PropertyMetadata(null));
		public static readonly DependencyProperty StateProperty = StatePropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey AdditionalStatesPropertyKey = DependencyPropertyManager.RegisterReadOnly("AdditionalStates",
			typeof(StateCollection), typeof(StateIndicatorControl), new PropertyMetadata(null));
		public static readonly DependencyProperty AdditionalStatesProperty = AdditionalStatesPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey ActualModelPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualModel",
			typeof(StateIndicatorModel), typeof(StateIndicatorControl), new PropertyMetadata(null, ActualModelPropertyChanged));
		public static readonly DependencyProperty ActualModelProperty = ActualModelPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey StateCountPropertyKey = DependencyPropertyManager.RegisterReadOnly("StateCount",
			typeof(int), typeof(StateIndicatorControl), new PropertyMetadata(0));
		public static readonly DependencyProperty StateCountProperty = StateCountPropertyKey.DependencyProperty;
		[
		Category(Categories.Presentation)
		]
		public StateIndicatorModel Model {
			get { return (StateIndicatorModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		[
		Category(Categories.Data)
		]
		public int StateIndex {
			get { return (int)GetValue(StateIndexProperty); }
			set { SetValue(StateIndexProperty, value); }
		}
		[
		Category(Categories.Data)
		]
		public State DefaultState {
			get { return (State)GetValue(DefaultStateProperty); }
			set { SetValue(DefaultStateProperty, value); }
		}
		[
		Category(Categories.Presentation)
		]
		public StateCollection AdditionalStates {
			get { return (StateCollection)GetValue(AdditionalStatesProperty); }
		}
		[
		Category(Categories.Presentation)
		]
		public State State {
			get { return (State)GetValue(StateProperty); }
		}
		[
		Category(Categories.Presentation)
		]
		public StateIndicatorModel ActualModel {
			get { return (StateIndicatorModel)GetValue(ActualModelProperty); }
		}
		[
		Category(Categories.Data)
		]
		public int StateCount {
			get { return (int)GetValue(StateCountProperty); }
		}
		static StateIndicatorControl() {
		}
		static void ModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			StateIndicatorControl stateIndicator = d as StateIndicatorControl;
			if (stateIndicator != null) {
				StateIndicatorModel model = e.NewValue as StateIndicatorModel;
				if (model == null)
					stateIndicator.SetValue(StateIndicatorControl.ActualModelPropertyKey, new EmptyStateIndicatorModel());
				else
					stateIndicator.SetValue(StateIndicatorControl.ActualModelPropertyKey, model);
			}
		}
		static void ActualModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			IModelSupported obj = d as IModelSupported;
			IOwnedElement model = e.NewValue as IOwnedElement;
			if (model != null)
				model.Owner = d as StateIndicatorControl;
			if (obj != null)
				obj.UpdateModel();
		}
		static void StateIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			StateIndicatorControl stateIndicator = d as StateIndicatorControl;
			if (stateIndicator != null)
				stateIndicator.UpdateActualState();
		}
		static void DefaultStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			StateIndicatorControl stateIndicator = d as StateIndicatorControl;
			if (stateIndicator != null) {
				if (stateIndicator.StateIndex < 0 || stateIndicator.StateIndex >= stateIndicator.StateCount)
					stateIndicator.UpdateActualState();
			}
		}
		internal static void ValueIndicatorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			StateIndicatorControl stateIndicator = d as StateIndicatorControl;
			ValueIndicatorBase oldValueIndicator = e.OldValue as ValueIndicatorBase;
			if (oldValueIndicator != null)
				oldValueIndicator.StateIndicator = null;
			ValueIndicatorBase newValueIndicator = e.NewValue as ValueIndicatorBase;			
			if (newValueIndicator != null)
				newValueIndicator.StateIndicator = stateIndicator;			
			if (stateIndicator != null) {
				stateIndicator.SubscribeRangeCollectionEvents(newValueIndicator != null ? newValueIndicator.Scale : null,
					oldValueIndicator != null ? oldValueIndicator.Scale : null);
				stateIndicator.UpdateStateIndexByValueIndicator(AnalogGaugeControl.GetValueIndicator(stateIndicator));				
			}			
		}
		IEnumerable<StateInfo> StatesInfo {
			get {
				foreach (State state in actualStates)
					yield return state.ElementInfo;
			}
		}
		public static List<PredefinedElementKind> PredefinedModels {
			get { return PredefinedStateIndicatorModels.ModelKinds; }
		}
		readonly List<State> actualStates = new List<State>();
		IList currentRangeCollection = null;
		State ActualDefaultState { get { return DefaultState != null ? DefaultState : ActualModel.DefaultState; } }
		public StateIndicatorControl() {
			DefaultStyleKey = typeof(StateIndicatorControl);
			this.SetValue(AdditionalStatesPropertyKey, new StateCollection(this));
			this.SetValue(ActualModelPropertyKey, new EmptyStateIndicatorModel());
		}
		#region IModelSupported implementation
		void IModelSupported.UpdateModel() {
			UpdateStates();
		}
		#endregion
		#region ILayoutCalculator implementation
		ElementLayout ILayoutCalculator.CreateLayout(Size constraint) {
			return new ElementLayout();
		}
		void ILayoutCalculator.CompleteLayout(ElementInfoBase elementInfo) {
			if (elementInfo.Layout != null)
				elementInfo.Layout.CompleteLayout(new Point(0, 0), null, null);
		}
		#endregion
		internal void SubscribeRangeCollectionEvents(Scale newScale, Scale oldScale) {
			if (oldScale != null) {
				IList oldRanges = oldScale is ArcScale ? (IList)((ArcScale)oldScale).Ranges : (IList)((LinearScale)oldScale).Ranges;
				INotifyCollectionChanged collection = oldRanges as INotifyCollectionChanged;
				if (collection != null)
					collection.CollectionChanged -= new NotifyCollectionChangedEventHandler(RangeCollectionChanged);
				foreach (object o in oldRanges)
					UnsubscribeRangeEvents(o as RangeBase);
			}
			if (newScale != null) {
				currentRangeCollection = newScale is ArcScale ? (IList)((ArcScale)newScale).Ranges : (IList)((LinearScale)newScale).Ranges;
				INotifyCollectionChanged collection = currentRangeCollection as INotifyCollectionChanged;
				if (collection != null)
					collection.CollectionChanged += new NotifyCollectionChangedEventHandler(RangeCollectionChanged);
				foreach (object o in currentRangeCollection)
					SubscribeRangeEvents(o as RangeBase);
			}
			else
				currentRangeCollection = null;
		}
		void SubscribeRangeEvents(RangeBase range) {
			if (range != null) {
				range.IndicatorEnter += new IndicatorEnterEventHandler(RangeIndicatorEnter);
				range.IndicatorLeave += new IndicatorLeaveEventHandler(RangeIndicatorLeave);
			}
		}
		void UnsubscribeRangeEvents(RangeBase range) {
			if (range != null) {
				range.IndicatorEnter -= new IndicatorEnterEventHandler(RangeIndicatorEnter);
				range.IndicatorLeave -= new IndicatorLeaveEventHandler(RangeIndicatorLeave);
			}
		}
		void RangeIndicatorLeave(object sender, IndicatorLeaveEventArgs e) {
			ValueIndicatorBase indicator = AnalogGaugeControl.GetValueIndicator(this);
			if (currentRangeCollection != null && indicator != null)
				if (object.ReferenceEquals(indicator, e.Indicator)&& StateIndex == currentRangeCollection.IndexOf(sender))
					UpdateStateIndexByValueIndicator(AnalogGaugeControl.GetValueIndicator(this));
		}
		void RangeIndicatorEnter(object sender, IndicatorEnterEventArgs e) {
			ValueIndicatorBase indicator = AnalogGaugeControl.GetValueIndicator(this);
			if (currentRangeCollection != null && indicator != null)
				if (object.ReferenceEquals(indicator, e.Indicator))
					UpdateStateIndexByValueIndicator(AnalogGaugeControl.GetValueIndicator(this));
		}
		void RangeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null && (e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace))
				foreach (object o in e.OldItems)
					UnsubscribeRangeEvents(o as RangeBase);
			if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace))
				foreach (object o in e.NewItems)
					SubscribeRangeEvents(o as RangeBase);
			UpdateStateIndexByValueIndicator(AnalogGaugeControl.GetValueIndicator(this));
		}
		void UpdateActualState() {
			if (StateIndex >= 0 && StateIndex < actualStates.Count)
				this.SetValue(StateIndicatorControl.StatePropertyKey, actualStates[StateIndex]);
			else
				this.SetValue(StateIndicatorControl.StatePropertyKey, ActualDefaultState);
			if (State.ElementInfo == null && State.Presentation != null)
				State.ElementInfo = new StateInfo(this, 0, State.Presentation.CreateStatePresentationControl(), State.Presentation);
			if (State.Presentation != null)
				State.ElementInfo.Invalidate();
		}
		internal void UpdateStateIndexByValueIndicator(ValueIndicatorBase indicator) {
			bool isRangeAssigned = false;
			if (currentRangeCollection != null && indicator != null)
				foreach (object o in currentRangeCollection) {
					RangeBase range = o as RangeBase;
					if (indicator.Value >= Math.Min(range.EndValue.Value, range.StartValue.Value) && 
						indicator.Value <= Math.Max(range.EndValue.Value, range.StartValue.Value)) {
						this.SetValue(StateIndexProperty, currentRangeCollection.IndexOf(range));
						isRangeAssigned = true;
						break;
					}
				}
			if (currentRangeCollection == null || !isRangeAssigned)
				this.SetValue(StateIndexProperty, -1);
		}
		internal void UpdateStates() {
			actualStates.Clear();
			if (ActualModel != null)
				foreach (State state in ActualModel.PredefinedStates)
					actualStates.Add(state);
			if (AdditionalStates != null)
				foreach (State state in AdditionalStates)
					actualStates.Add(state);
			this.SetValue(StateIndicatorControl.StateCountPropertyKey, actualStates.Count);
			UpdateActualState();
		}
		protected override Size MeasureOverride(Size constraint) {
			double constraintWidth = double.IsInfinity(constraint.Width) ? defaultWidth : constraint.Width;
			double constraintHeight = double.IsInfinity(constraint.Height) ? defaultHeight : constraint.Height;
			return base.MeasureOverride(new Size(constraintWidth, constraintHeight));
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new StateIndicatorControlAutomationPeer(this);
		}
	}
	public class StateIndicatorControlAutomationPeer : FrameworkElementAutomationPeer {
		StateIndicatorControl StateIndicator {
			get { return Owner as StateIndicatorControl; }
		}
		public StateIndicatorControlAutomationPeer(FrameworkElement owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			return "StateIndicatorControl";
		}
		protected override string GetLocalizedControlTypeCore() {
			return GaugeLocalizer.GetString(GaugeStringId.StateIndicatorLocalizedControlType);
		}
		protected override bool IsContentElementCore() {
			return false;
		}
		protected override string GetHelpTextCore() {
			string helpTextBase = base.GetHelpTextCore();
			if (String.IsNullOrEmpty(helpTextBase))
				return GaugeLocalizer.GetString(GaugeStringId.StateIndicatorAutomationPeerHelpText);
			else
				return helpTextBase;
		}
	}
}
