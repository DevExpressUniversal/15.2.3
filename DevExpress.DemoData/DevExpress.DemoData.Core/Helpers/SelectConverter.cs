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
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace DevExpress.DemoData.Helpers {
	public abstract class ValueConverterBase : MarkupExtension {
		public string S { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return XamlReaderHelper.CreateObjectFromXaml(GetTargetType(), S);
		}
		protected abstract Type GetTargetType();
	}
	public class DoubleValueExtension : ValueConverterBase {
		protected override Type GetTargetType() {
			return typeof(double);
		}
	}
	public class BrushValueExtension : ValueConverterBase {
		protected override Type GetTargetType() {
			return typeof(SolidColorBrush);
		}
	}
	public class VisibilityValueExtension : ValueConverterBase {
		protected override Type GetTargetType() {
			return typeof(Visibility);
		}
	}
	public class BoolValueExtension : ValueConverterBase {
		protected override Type GetTargetType() {
			return typeof(bool);
		}
	}
	public class IntValueExtension : ValueConverterBase {
		protected override Type GetTargetType() {
			return typeof(int);
		}
	}
	public class ThicknessValueExtension : ValueConverterBase {
		protected override Type GetTargetType() {
			return typeof(Thickness);
		}
	}
	[ContentProperty("Value")]
	public class SelectState : DependencyObjectExt {
		#region Dependency Properties
		public static readonly DependencyProperty ValueProperty;
		static SelectState() {
			Type ownerType = typeof(SelectState);
			ValueProperty = DependencyProperty.Register("Value", typeof(object), ownerType, new PropertyMetadata(null, RaiseValueChanged));
		}
		object valueValue = null;
		static void RaiseValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SelectState)d).valueValue = e.NewValue;
		}
		#endregion
		public SelectState() { }
		public string Key { get; set; }
		public object Value { get { return valueValue; } set { SetValue(ValueProperty, value); } }
	}
	[ContentProperty("Storyboard")]
	public class SelectTransition : DependencyObjectExt {
		#region Dependency Properties
		public static readonly DependencyProperty StoryboardProperty;
		static SelectTransition() {
			Type ownerType = typeof(SelectTransition);
			StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(Storyboard), ownerType, new PropertyMetadata(null, RaiseStoryboardChanged));
		}
		Storyboard storyboardValue = null;
		static void RaiseStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SelectTransition)d).storyboardValue = (Storyboard)e.NewValue;
		}
		#endregion
		public SelectTransition() {
			Storyboard = new Storyboard();
		}
		public string From { get; set; }
		public string To { get; set; }
		public Storyboard Storyboard { get { return storyboardValue; } set { SetValue(StoryboardProperty, value); } }
	}
	public class SelectStateCollection : ObservableCollection<SelectState> { }
	public class SelectTransitionCollection : ObservableCollection<SelectTransition> { }
	[ContentProperty("States")]
	public class SelectConverter : DependencyObjectExt {
		#region Dependency Properties
		public static readonly DependencyProperty DefaultValueProperty;
		public static readonly DependencyProperty StatesProperty;
		public static readonly DependencyProperty TransitionsProperty;
		static SelectConverter() {
			Type ownerType = typeof(SelectConverter);
			DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(object), ownerType, new PropertyMetadata(null, RaiseDefaultValueChanged));
			StatesProperty = DependencyProperty.Register("States", typeof(SelectStateCollection), ownerType, new PropertyMetadata(null, RaiseStatesChanged));
			TransitionsProperty = DependencyProperty.Register("Transitions", typeof(SelectTransitionCollection), ownerType, new PropertyMetadata(null, RaiseTransitionsChanged));
		}
		object defaultValueValue = null;
		SelectStateCollection statesValue = null;
		SelectTransitionCollection transitionsValue = null;
		static void RaiseDefaultValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SelectConverter)d).defaultValueValue = e.NewValue;
		}
		static void RaiseStatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SelectConverter)d).statesValue = (SelectStateCollection)e.NewValue;
		}
		static void RaiseTransitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SelectConverter)d).transitionsValue = (SelectTransitionCollection)e.NewValue;
		}
		#endregion
		public SelectConverter() {
			States = new SelectStateCollection();
			Transitions = new SelectTransitionCollection();
		}
		public object DefaultValue { get { return defaultValueValue; } set { SetValue(DefaultValueProperty, value); } }
		public SelectStateCollection States { get { return statesValue; } set { SetValue(StatesProperty, value); } }
		public SelectTransitionCollection Transitions { get { return transitionsValue; } set { SetValue(TransitionsProperty, value); } }
		public SelectTransition FindTransition(string from, string to) {
			SelectTransition tr1 = null;
			SelectTransition tr2 = null;
			SelectTransition tr3 = null;
			foreach(SelectTransition transition in Transitions) {
				if(string.IsNullOrEmpty(transition.To) && transition.From == from) {
					tr3 = transition;
				} else if(string.IsNullOrEmpty(transition.From) && transition.To == to) {
					tr2 = transition;
				} else if(transition.From == from && transition.To == to) {
					tr1 = transition;
					break;
				}
			}
			return tr1 != null ? tr1 : tr2 != null ? tr2 : tr3;
		}
		public SelectState FindState(string to) {
			foreach(SelectState state in States) {
				if(state.Key == to) return state;
			}
			return null;
		}
	}
	public class NullStoryboardTarget : DependencyObject { }
	public class SelectBinding : MarkupExtension {
		static int intermediatePropertyIndex = 0;
		static LinkedList<DependencyProperty> intermediateProperties = new LinkedList<DependencyProperty>();
		WeakReference targetObject;
		DependencyProperty targetProperty;
		object defaultValue = new object();
		DependencyProperty intermediateProperty;
		DependencyProperty useTransitionValueProperty;
		string currentValue;
		Storyboard currentAnimation;
		EventHandler currentAnimationCompleted;
		Dictionary<SelectTransition, Storyboard> transitions = new Dictionary<SelectTransition,Storyboard>();
		public SelectBinding() {
			intermediateProperty = GetIntermediateProperty(RaiseValueChanged);
			useTransitionValueProperty = GetIntermediateProperty(RaiseUseTransitionValueChanged);
			UseTransition = true;
		}
		~SelectBinding() {
			ReleaseIntermediateProperty(intermediateProperty);
			ReleaseIntermediateProperty(useTransitionValueProperty);
		}
		public SelectConverter Converter { get; set; }
		public BindingBase Binding { get; set; }
		public bool UseTransition { get; set; }
		public BindingBase UseTransitionBinding { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			IProvideValueTarget target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
			DependencyObject targetObject = target.TargetObject as DependencyObject;
			targetProperty = target.TargetProperty as DependencyProperty;
			if(targetObject == null || targetProperty == null)
				throw new InvalidOperationException();
			this.targetObject = new WeakReference(targetObject);
			BindingOperations.SetBinding(targetObject, intermediateProperty, Binding);
			if(UseTransitionBinding != null)
				BindingOperations.SetBinding(targetObject, useTransitionValueProperty, UseTransitionBinding);
			return GetTargetPropertyValue();
		}
		object GetTargetPropertyValue() {
			DependencyObject targetObject = (DependencyObject)this.targetObject.Target;
			if(targetObject == null) return null;
			return targetObject.GetValue(targetProperty);
		}
		void SetTargetPropertyValue(object v) {
			DependencyObject targetObject = (DependencyObject)this.targetObject.Target;
			if(targetObject == null) return;
			targetObject.SetValue(targetProperty, v);
		}
		void RaiseValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			string oldState = currentValue;
			currentValue = e.NewValue == null ? "NULL" : e.NewValue.ToString();
			bool useTransition = UseTransition && e.OldValue != defaultValue;
			BeginUpdateState(!useTransition, oldState, currentValue);
		}
		void RaiseUseTransitionValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UseTransition = (bool)e.NewValue;
		}
		void ReleaseIntermediateProperty(DependencyProperty intermediateProperty) {
			lock(intermediateProperties) {
				intermediateProperties.AddLast(intermediateProperty);
			}
		}
		DependencyProperty GetIntermediateProperty(PropertyChangedCallback valueChanged) {
			lock(intermediateProperties) {
				LinkedListNode<DependencyProperty> last = intermediateProperties.Last;
				if(last != null) {
					intermediateProperties.RemoveLast();
					return last.Value;
				}
				++intermediatePropertyIndex;
				string propertyName = string.Format("SelectBinding{0}Property", intermediatePropertyIndex);
				return DependencyProperty.RegisterAttached(propertyName, typeof(object), typeof(SelectBinding), new PropertyMetadata(defaultValue, valueChanged));
			}
		}
		void BeginUpdateState(bool doNotUseTransition, string oldState, string newState) {
			DependencyObject targetObject = (DependencyObject)this.targetObject.Target;
			if(targetObject == null) return;
			Storyboard transition = null;
			if(!doNotUseTransition)
				transition = FindTransition(oldState, newState);
			if(transition == null) {
				EndUpdateState(oldState, newState);
			} else {
				StopAnimation();
				currentAnimation = transition;
				currentAnimationCompleted = (s, e) => {
					currentAnimation.Completed -= currentAnimationCompleted;
					EndUpdateState(oldState, newState);
				};
				currentAnimation.Completed += currentAnimationCompleted;
				Storyboard.SetTarget(currentAnimation, targetObject);
				currentAnimation.Begin();
			}
		}
		Storyboard FindTransition(string oldState, string newState) {
			SelectTransition t = Converter.FindTransition(oldState, newState);
			if(t == null) return null;
			Storyboard sb;
			if(!transitions.TryGetValue(t, out sb)) {
				sb = StoryboardCloneHelper.CopyStoryboard(t.Storyboard);
				transitions.Add(t, sb);
			}
			return sb;
		}
		void EndUpdateState(string oldState, string newState) {
			StopAnimation();
			currentAnimation = null;
			SelectState state = Converter.FindState(newState);
			SetTargetPropertyValue(state == null ? Converter.DefaultValue : state.Value);
		}
		void StopAnimation() {
			if(currentAnimation != null) {
				currentAnimation.Completed -= currentAnimationCompleted;
				currentAnimationCompleted = null;
				currentAnimation.SkipToFill();
				currentAnimation.Stop();
				Storyboard.SetTarget(currentAnimation, new NullStoryboardTarget());
			}
		}
	}
	public static class StoryboardCloneHelper {
		abstract class PropertyCopyManagerBase {
			public abstract void Clone(object source, object dest);
		}
		class PropertyCopyManager<TSource> : PropertyCopyManagerBase where TSource : class {
			public PropertyCopyManager(Func<TSource, object> getter, Action<TSource, object> setter) {
				Getter = getter;
				Setter = setter;
			}
			public Func<TSource, object> Getter { get; private set; }
			public Action<TSource, object> Setter { get; private set; }
			public override void Clone(object source, object dest) {
				TSource t = source as TSource;
				if(t == null) return;
				Setter((TSource)dest, Getter(t));
			}
		}
		static List<PropertyCopyManagerBase> properties = new List<PropertyCopyManagerBase>();
		static StoryboardCloneHelper() {
			properties.Add(new PropertyCopyManager<Timeline>(o => Storyboard.GetTargetProperty(o), (o, v) => { if(v != null) Storyboard.SetTargetProperty(o, (PropertyPath)v); }));
		}
		public static Storyboard CopyStoryboard(Storyboard src) {
			if(src == null) return null;
			return src.Clone();
		}
		public static T Clone<T>(T source) {
			T cloned = (T)Activator.CreateInstance(source.GetType());
			foreach(PropertyInfo propertyInfo in source.GetType().GetProperties()) {
				MethodInfo getMethod = propertyInfo.GetGetMethod();
				MethodInfo setMethod = propertyInfo.GetSetMethod();
				if(getMethod == null || setMethod == null) continue;
				if(propertyInfo.Name != "Item") {
					object value = getMethod.Invoke(source, null);
					DependencyObject d = value as DependencyObject;
					if(d != null)
						value = Clone(d);
					setMethod.Invoke(cloned, new object[] { value });
				} else {
					int count = (int)propertyInfo.ReflectedType.GetProperty("Count").GetValue(source, null);
					for(int i = 0; i < count; ++i) {
						object value = getMethod.Invoke(source, new object[] { i });
						DependencyObject d = value as DependencyObject;
						if(d != null)
							value = Clone(d);
						propertyInfo.ReflectedType.GetMethod("Add").Invoke(cloned, new object[] { value });
					}
				}
			}
			foreach(PropertyCopyManagerBase property in properties) {
				property.Clone(source, cloned);
			}
			return cloned;
		}
	}
}
