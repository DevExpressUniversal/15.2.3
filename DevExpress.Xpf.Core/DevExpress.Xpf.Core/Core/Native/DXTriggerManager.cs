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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Reflection;
#if SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Core.Native {
	public interface ITargetPropertyResolverFactory {
		TargetPropertyUpdater CreateTargetPropertyResolver(FrameworkElement owner);
	}
	internal enum SituationTrigger {
		Style,
		Template,
		Resolver,
		NotDefine
	}
	class BindingSource {
		public BindingSource(string elementName, RelativeSource relativeSource) {
			this.ElementName = elementName;
			this.RelativeSource = relativeSource;
		}
		public string ElementName { get; set; }
		public Binding NewBinding { get; set; }
		public RelativeSource RelativeSource { get; set; }
	}
	public class DXTrigger {
		Control templatedParent;
		public DXTrigger(UIElement owner, string visualState, string visualStateNormal, DXConditionCollection conditions, TargetPropertyUpdater resolver)
			: this(owner, visualState, visualStateNormal, conditions, resolver, null) { }
		internal DXTrigger(UIElement owner, string visualState, string visualStateNormal, DXConditionCollection conditions, TargetPropertyUpdater resolver, DXSetterCollection setters) {
			this.Resolver = resolver;
			VisualState = visualState;
			VisualStateNormal = visualStateNormal;
			this.Setters = setters;
			Initialize(owner, conditions);
		}		
		public DXSetterCollection Setters { get; set; }
		public string VisualState { get; set; }
		public string VisualStateNormal { get; set; }
		protected internal DXConditionCollection Conditions { get; set; }
		protected internal bool IsActive { get; set; }
		protected internal UIElement Owner { get; set; }
		protected internal TargetPropertyUpdater Resolver { get; set; }
		protected internal Collection<DXTriggerCondition> TriggerConditions { get; set; }
		SituationTrigger TriggerSituation { get; set; }
		protected internal Control TemplatedParent {
			get { return templatedParent ?? (templatedParent = DXTriggerManager.GetTemplatedParent(Owner) as Control); }
		}
		void Initialize(UIElement owner, DXConditionCollection conditions) {
			this.Owner = owner;
			this.Conditions = conditions;
			TriggerSituation = Resolver != null ? SituationTrigger.Resolver : DefineSituationTrigger();
			InitializeBySituationTrigger(TriggerSituation);
		}
		void InitializeBySituationTrigger(SituationTrigger situationTrigger) {
			switch(situationTrigger) {
				case SituationTrigger.Style:
					TemplatedParent.LayoutUpdated += DXTrigger_LayoutUpdated;
					break;
				case SituationTrigger.Template:
					(Owner as FrameworkElement).LayoutUpdated += DXTrigger_LayoutUpdated;
					break;
				case SituationTrigger.Resolver:
					Resolver.AnimationUpdated += Resolver_AnimationUpdated;
					break;
				default:
					InitializeCore(Owner, Conditions);
					break;
			}
		}
		SituationTrigger DefineSituationTrigger() {
			TriggerSituation = Native.SituationTrigger.NotDefine;
			if (TemplatedParent == null)
				TriggerSituation = SituationTrigger.Template;
			if (TemplatedParent != null && TemplatedParent.Template == null)
				TriggerSituation = SituationTrigger.Style;
			return TriggerSituation;
		}
		void DXTrigger_LayoutUpdated(object sender, EventArgs e) {
			InitializeCore(Owner, Conditions);
#if SL
			DXTriggerManager.InitializedTriggersCount++;
			if (DXTriggerManager.InitializedTriggersCount < DXTriggerManager.GetTriggers(Owner).Count)
				return;
#endif
			if (TriggerSituation == Native.SituationTrigger.Template)
				(Owner as FrameworkElement).LayoutUpdated -= DXTrigger_LayoutUpdated;
			else
				TemplatedParent.LayoutUpdated -= DXTrigger_LayoutUpdated;
		}
		void InitializeCore(UIElement owner, DXConditionCollection conditions) {
			TriggerConditions = new Collection<DXTriggerCondition>();
			foreach (DXCondition condition in conditions) {
				DXTriggerCondition triggerCondition = new DXTriggerCondition(owner, condition);
				triggerCondition.ActualValueChanged += triggerCondition_ActualValueChanged;
				TriggerConditions.Add(triggerCondition);
			}
			ActivateTrigger();
		}
		void ActivateTrigger() {
			foreach (DXTriggerCondition condition in TriggerConditions) {
				condition.ActivateCondition();
			}
		}
		protected internal bool IsSetValue() {
			bool flag = TriggerConditions.Count > 0;
			for (int i = 0; flag && (i < TriggerConditions.Count); i++) {
				flag = (bool)TriggerConditions[i].IsEntryCondition;
			}
			return flag;
		}
		void Resolver_AnimationUpdated(object sender, EventArgs e) {
			InitializeBySituationTrigger(DefineSituationTrigger());
			Resolver.AnimationUpdated -= Resolver_AnimationUpdated;
		}
		void triggerCondition_ActualValueChanged(object sender, EventArgs e) {
			PerformAction();
		}
		protected internal virtual void PerformAction() {
			if (TemplatedParent == null)
				return;
			try {
				if (IsSetValue()) {
					IsActive = VisualStateManager.GoToState(TemplatedParent, VisualState, false);
				} else {
					if (IsActive && !String.IsNullOrEmpty(VisualStateNormal)) {
						IsActive = !(VisualStateManager.GoToState(TemplatedParent, VisualStateNormal, false));
					}
				}
			} catch (InvalidOperationException ex) {
				Debug.WriteLine("!DXTriggerError: " + ex.Message);
			}
		}
	}
	internal class DXTriggerBySetter : DXTrigger {
		public DXTriggerBySetter(UIElement owner, DXConditionCollection conditions, TargetPropertyUpdater resolver, DXSetterCollection setters)
			: base(owner, string.Empty, string.Empty, conditions, resolver, setters) { }
		Collection<object> PreviousValues = new Collection<object>();
		protected internal override void PerformAction() {
			if(TriggerConditions[0].TemplatedParent == null)
				return;
			bool isSetValue = IsSetValue();
			for (int i = 0; i < Setters.Count; i++) {
				IsActive = SetOrResetValue(isSetValue, i);
			}
		}
		bool SetOrResetValue(bool isSetValue, int i) {
			UIElement element = Owner.GetElementByName(Setters[i].TargetName) as FrameworkElement;
			if (element == null)
				return IsActive;
			if (isSetValue) {
				PreviousValues.Add(element.GetValue(GetProperty(Setters[i].TargetProperty)));
				element.SetValue(GetProperty(Setters[i].TargetProperty), Setters[i].Value);
				return true;
			}
			if (!IsActive)
				return false;
			element.SetValue(GetProperty(Setters[i].TargetProperty), PreviousValues[i]);
			return Setters.Count - 1 != i;
		}
		DependencyProperty GetProperty(string strProperty) {
			Type targetType = null;
			string targetProperty = null;
			if (strProperty.Contains(".")) {
				string[] parts = strProperty.Split('.');
#if SL
				targetType = Type.GetType("System.Windows.Controls." + parts[0] +
					", System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
#else
				targetType = Type.GetType("System.Windows.Controls." + parts[0] + 
					", PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
#endif
				targetProperty = parts[1];
			} else {
				targetType = TriggerConditions[0].TemplatedParent.GetType();
				targetProperty = strProperty;
			}
			BindingFlags dpFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
			FieldInfo[] sourceFields = targetType.GetFields(dpFlags);
			FieldInfo targetDependencyPropertyField =
				sourceFields.First(i => i.Name == targetProperty + "Property");
			DependencyProperty targetDependencyProperty =
				targetDependencyPropertyField.GetValue(null) as DependencyProperty;
			return targetDependencyProperty;
		}
	}
	public class DXTriggerCondition : FrameworkElement {
		public event EventHandler ActualValueChanged;
		public static readonly DependencyProperty ActualValueProperty;
		static DXTriggerCondition() {
			ActualValueProperty = DependencyProperty.Register("ActualValue", typeof(object), typeof(DXTriggerCondition), new PropertyMetadata(null, (d, e) => ((DXTriggerCondition)d).OnActualValueChanged()));
		}
		public DXTriggerCondition(UIElement owner, DXCondition condition) {
			Owner = owner;
			Binding = condition.Binding;
			TriggerValue = condition.TriggerValue;			
		}
		public object ActualValue {
			get { return GetValue(ActualValueProperty); }
			set { SetValue(ActualValueProperty, value); }
		}
		BindingSource bindingSource;
		FrameworkElement templatedParent;
		public UIElement Owner { get; private set; }
		public Binding Binding { get; private set; }
		public object TriggerValue { get; private set; }
		public bool IsEntryCondition { get { return object.Equals(ActualValue, TriggerValue); } }
		protected internal 
#if !SL
			new 
#endif
			FrameworkElement TemplatedParent {
			get { return templatedParent ?? (templatedParent = DXTriggerManager.GetTemplatedParent(Owner)); }
		}
		void OnActualValueChanged() {
#if DEBUGTEST
			RaiseActualValueChanged();
#else
			Dispatcher.BeginInvoke((Action)(() => RaiseActualValueChanged()));
#endif
		}
		void RaiseActualValueChanged() {
			if (ActualValueChanged != null)
				ActualValueChanged(this, EventArgs.Empty);
		}
		public void ActivateCondition() {
			if (Owner == null || Binding == null)
				return;
			CreateBinding(Binding);
			if (bindingSource.NewBinding.Source != null || bindingSource.NewBinding.RelativeSource != null)
				SetBinding();
			if (TriggerValue == null)
				RaiseActualValueChanged();
		}
		void SetBinding() {
			BindingOperations.SetBinding(this, DXTriggerCondition.ActualValueProperty, bindingSource.NewBinding);
		}
		void CreateBinding(Binding binding) {
			bindingSource = new BindingSource(binding.ElementName, binding.RelativeSource);
			bindingSource.NewBinding = new Binding();
#if SL
			if (binding.Path.Path.Contains(':'))
				bindingSource.NewBinding = BindingHelper.GetBindingOnAtachedProperty(null, binding.Path.Path, null);
			else
				bindingSource.NewBinding.Path = binding.Path;
#else
			bindingSource.NewBinding.Path = binding.Path;
#endif
			bindingSource.NewBinding.Source = binding.Source ?? GetSource(bindingSource);
			bindingSource.NewBinding.Converter = binding.Converter;
			bindingSource.NewBinding.ConverterCulture = binding.ConverterCulture;
			bindingSource.NewBinding.ConverterParameter = binding.ConverterParameter;
			bindingSource.NewBinding.Mode = binding.Mode;
		}
		object GetSource(BindingSource bindingSource) {
			if(TemplatedParent == null) 
				return null;
			if (String.IsNullOrEmpty(bindingSource.ElementName))
				return bindingSource.RelativeSource == null ? TemplatedParent.DataContext : GetSourceByRelativeSource(bindingSource.RelativeSource);
			return GetElementByName(bindingSource.ElementName);
		}
		object GetSourceByRelativeSource(RelativeSource relativeSource) {
			switch (relativeSource.Mode) {
				case RelativeSourceMode.Self: return Owner;
				case RelativeSourceMode.TemplatedParent: return TemplatedParent;
				default: return null;
			}
		}
		UIElement GetElementByName(string elementName) {
#if !SL
			return Owner.GetElementByName(elementName);
#else
			return Owner.GetElementByName(elementName);
#endif
		}
	}
	[ContentProperty("TriggerValue")]
	public class DXCondition {
		public Binding Binding { get; set; }
		public object TriggerValue { get; set; }
	}
	public class DXConditionCollection : Collection<DXCondition> { }
	public class DXMultiTriggerInfo : DXTriggerInfoBase {
		public DXMultiTriggerInfo() {
			Conditions = new DXConditionCollection();
		}
		public DXConditionCollection Conditions { get { return TriggersConditions; } set { TriggersConditions = value; } }
	}
	public class DXSetter {
		public object Value { get; set; }
		public string TargetProperty { get; set; }
		public string TargetName { get; set; }
	}
	[ContentProperty("TriggerValue")]
	public class DXTriggerInfo : DXTriggerInfoBase {
		object triggerValue;
		Binding binding;
		public DXTriggerInfo() {
			TriggersConditions = new DXConditionCollection() { new DXCondition() };
		}
		public object TriggerValue {
			get { return triggerValue; }
			set {
				triggerValue = value;
				TriggersConditions[0].TriggerValue = value;
			}
		}
		public Binding Binding {
			get { return binding; }
			set {
				binding = value;
				TriggersConditions[0].Binding = value;
			}
		}
	}
	public abstract class DXTriggerInfoBase : FrameworkElement {
		public ITargetPropertyResolverFactory TargetPropertyResolverFactory { get; set; }
		internal DXConditionCollection TriggersConditions { get; set; }
		public string VisualState { get; set; }
		public string VisualStateNormal { get; set; }
		public DXSetterCollection Setters { get; set; }
	}
	public class DXTriggerCollection : ObservableCollection<DXTrigger> {
		public DXTriggerCollection() { }
		public DXTriggerCollection(IEnumerable<DXTriggerInfoBase> collection, UIElement owner) {
			this.Owner = owner;
			if (collection == null)
				return;
			foreach (DXTriggerInfoBase trigger in collection) {
				Items.Add(CreateNewTrigger(trigger));
			}
		}
		public UIElement Owner { get; set; }
		DXTrigger CreateNewTrigger(DXTriggerInfoBase triggerInfo) {
			TargetPropertyUpdater resolver = null;
			if (triggerInfo.TargetPropertyResolverFactory != null)
				resolver = triggerInfo.TargetPropertyResolverFactory.CreateTargetPropertyResolver(Owner as FrameworkElement);
			if (triggerInfo.Setters != null)
				return new DXTriggerBySetter(Owner, triggerInfo.TriggersConditions, resolver, triggerInfo.Setters);
			return new DXTrigger(Owner, triggerInfo.VisualState, triggerInfo.VisualStateNormal, triggerInfo.TriggersConditions, resolver);
		}
	}
	public class DXTriggerInfoCollection : Collection<DXTriggerInfoBase> { }
	public class DXSetterCollection : Collection<DXSetter> { }
	public class DXTriggerManager : DependencyObject {
		public static readonly DependencyProperty TriggersProperty;
		public static readonly DependencyProperty TriggersInfoProperty;
		static DXTriggerManager() {
			TriggersInfoProperty = DependencyProperty.RegisterAttached("TriggersInfo", typeof(DXTriggerInfoCollection), typeof(DXTriggerManager), new PropertyMetadata(null, OnTriggersInfoChanged));
			TriggersProperty = DependencyProperty.RegisterAttached("Triggers", typeof(DXTriggerCollection), typeof(DXTriggerManager), new PropertyMetadata(null));
		}
		internal static int InitializedTriggersCount { get; set; }
		public static DXTriggerCollection GetTriggers(DependencyObject obj) {
			DXTriggerCollection triggers = (DXTriggerCollection)obj.GetValue(TriggersProperty);
			if (triggers == null) {
				triggers = new DXTriggerCollection();
				SetTriggers(obj, triggers);
			}
			return triggers;
		}
		public static DXTriggerInfoCollection GetTriggersInfo(DependencyObject obj) {
			DXTriggerInfoCollection triggers = (DXTriggerInfoCollection)obj.GetValue(TriggersInfoProperty);
			if (triggers == null) {
				triggers = new DXTriggerInfoCollection();
				SetTriggersInfo(obj, triggers);
			}
			return triggers;
		}
		static void OnTriggersInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (e.NewValue == null)
				return;
			InitializedTriggersCount = 0;
			DXTriggerCollection triggers = new DXTriggerCollection(e.NewValue as IEnumerable<DXTriggerInfoBase>, d as UIElement);
			SetTriggers(d, triggers);
		}
		public static void SetTriggers(DependencyObject obj, DXTriggerCollection value) {
			obj.SetValue(TriggersProperty, value);
		}
		public static void SetTriggersInfo(DependencyObject obj, Collection<DXTriggerInfoBase> value) {
			obj.SetValue(TriggersInfoProperty, value);
		}
		internal static FrameworkElement GetTemplatedParent(UIElement d) {
			return d is Control ? d as FrameworkElement : (d as FrameworkElement).GetTemplatedParent() as FrameworkElement;
		}
	}
	public abstract class TargetPropertyUpdater {
		FrameworkElement templatedParent;
		FrameworkElement owner;
		public event EventHandler AnimationUpdated;
		public TargetPropertyUpdater(FrameworkElement owner) {
			this.Owner = owner;
		}
		public bool IsAnimationUpdated { get; private set; }
		public FrameworkElement Owner {
			get { return owner; }
			private set {
				if (owner == value)
					return;
				owner = value;
				UpdateOwner();
			}
		}
		internal FrameworkElement Target { get { return templatedParent ?? (templatedParent = DXTriggerManager.GetTemplatedParent(Owner)); } }
		void Owner_LayoutUpdated(object sender, EventArgs e) {
			UpdateAnimation();
			Owner.LayoutUpdated -= Owner_LayoutUpdated;
		}
		void RaisAnimationUpdated() {
			if (AnimationUpdated != null)
				AnimationUpdated(this, EventArgs.Empty);
		}
		void UpdateAnimation() {
			IList groups = GetVisualStateGroups();
			if (Target == null)
				return;
			foreach (VisualStateGroup group in groups) {
				foreach (VisualState state in group.States) {
					if (state.Storyboard == null)
						continue;
					foreach (Timeline timeLine in state.Storyboard.Children) {
						PropertyPath propertyPath = Storyboard.GetTargetProperty(timeLine);
						if (!propertyPath.Path.Contains("(0)")) {
							string path = GetCorrectPath(propertyPath);
							DependencyProperty prop = GetDependencyPropertyByPath(path);
							if (prop != null)
								Storyboard.SetTargetProperty(timeLine, new PropertyPath(prop));
						}
						if (String.IsNullOrEmpty(Storyboard.GetTargetName(timeLine)))
							Storyboard.SetTarget(timeLine, Target);
					}
				}
			}
			IsAnimationUpdated = true;
			RaisAnimationUpdated();
		}
		void UpdateOwner() {
			if (owner != null) {
				Owner.LayoutUpdated += Owner_LayoutUpdated;
			}
		}
		string GetCorrectPath(PropertyPath propertyPath) {
			return propertyPath.Path.Contains(':') ? propertyPath.Path.Split(':')[1].Split(')')[0] : String.Empty;
		}
		IList GetVisualStateGroups() {
			IList groups = VisualStateManager.GetVisualStateGroups(Owner);
			if (groups.Count != 0)
				return groups;
			if (VisualTreeHelper.GetChildrenCount(Owner) == 0)
				return groups;
			FrameworkElement elem = VisualTreeHelper.GetChild(Owner, 0) as FrameworkElement;
			if (elem == null)
				return groups;
			groups = VisualStateManager.GetVisualStateGroups(elem);
			return groups;
		}
		protected abstract DependencyProperty GetDependencyPropertyByPath(string path);
	}
	public abstract class TargetPropertyUpdaterFactory : ITargetPropertyResolverFactory {
		public abstract TargetPropertyUpdater CreateTargetPropertyResolver(FrameworkElement owner);
	}
}
