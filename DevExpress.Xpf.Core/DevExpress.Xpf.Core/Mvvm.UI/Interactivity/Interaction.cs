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

#if !MVVM
using DevExpress.Xpf.Core.Native;
#endif
using System;
using System.Linq;
using System.Collections.Generic;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
#endif
#if FREE
using DevExpress.Mvvm.UI.Native;
#endif
namespace DevExpress.Mvvm.UI.Interactivity {
	public static class Interaction {
		#region Dependency Properties
#if SILVERLIGHT || NETFX_CORE
		const string BehaviorsPropertyName = "Behaviors";
		const string BehaviorsTemplatePropertyName = "BehaviorsTemplate";
		const string TriggersPropertyName = "Triggers";
#else
		const string BehaviorsPropertyName = "BehaviorsInternal";
		const string BehaviorsTemplatePropertyName = "BehaviorsTemplate";
		const string TriggersPropertyName = "TriggersInternal";
#endif
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty BehaviorsProperty =
			DependencyProperty.RegisterAttached(BehaviorsPropertyName, typeof(BehaviorCollection), typeof(Interaction), new PropertyMetadata(null, OnCollectionChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty BehaviorsTemplateProperty =
			DependencyProperty.RegisterAttached(BehaviorsTemplatePropertyName, typeof(DataTemplate), typeof(Interaction), new PropertyMetadata(null, OnBehaviorsTemplateChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty BehaviorsTemplateItemsProperty = 
			DependencyProperty.RegisterAttached("BehaviorsTemplateItems", typeof(IList<Behavior>), typeof(Interaction), new PropertyMetadata(null));
#if !NETFX_CORE
		[IgnoreDependencyPropertiesConsistencyChecker]
		[Obsolete("This property is obsolete. Use the Behaviors property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty TriggersProperty =
			DependencyProperty.RegisterAttached(TriggersPropertyName, typeof(TriggerCollection), typeof(Interaction), new PropertyMetadata(null, OnCollectionChanged));
#endif
		#endregion
		public static BehaviorCollection GetBehaviors(DependencyObject d) {
			BehaviorCollection behaviors = (BehaviorCollection)d.GetValue(BehaviorsProperty);
			if(behaviors == null) {
				behaviors = new BehaviorCollection();
				d.SetValue(BehaviorsProperty, behaviors);
			}
			return behaviors;
		}
		public static DataTemplate GetBehaviorsTemplate(DependencyObject d) {
			return (DataTemplate)d.GetValue(BehaviorsProperty);
		}
		public static void SetBehaviorsTemplate(DependencyObject d, DataTemplate template) {
			d.SetValue(BehaviorsTemplateProperty, template);
		}
#if !NETFX_CORE
		[Obsolete("This method is obsolete. Use the GetBehaviors method instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static TriggerCollection GetTriggers(DependencyObject d) {
			TriggerCollection triggers = (TriggerCollection)d.GetValue(TriggersProperty);
			if(triggers == null) {
				triggers = new TriggerCollection();
				d.SetValue(TriggersProperty, triggers);
			}
			return triggers;
		}
#endif
		static void OnCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			IAttachableObject oldValue = (IAttachableObject)e.OldValue;
			IAttachableObject newValue = (IAttachableObject)e.NewValue;
			if(object.ReferenceEquals(oldValue, newValue)) return;
			if(oldValue != null && oldValue.AssociatedObject != null)
				oldValue.Detach();
			if(newValue != null && d != null) {
				if(newValue.AssociatedObject != null)
					throw new InvalidOperationException();
				newValue.Attach(d);
			}
		}
		static void OnBehaviorsTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BehaviorCollection objectBehaviors = GetBehaviors(d);
			IList<Behavior> oldItems = d.GetValue(BehaviorsTemplateItemsProperty) as IList<Behavior>;
			DataTemplate newValue = e.NewValue as DataTemplate;
			if(oldItems != null) {
				foreach(Behavior behavior in oldItems)
					if(objectBehaviors.Contains(behavior))
						objectBehaviors.Remove(behavior);
			}
			if(newValue == null) {
				d.SetValue(BehaviorsTemplateItemsProperty, null);
				return;
			}
#if !NETFX_CORE && !SILVERLIGHT
			if(!newValue.IsSealed)
				newValue.Seal();
#endif
			IList<Behavior> newItems;
			DependencyObject content = newValue.LoadContent();
			if(content is ContentControl) {
				newItems = new List<Behavior>();
				var behavior = ((ContentControl)content).Content as Behavior;
				((ContentControl)content).Content = null;
				if(behavior != null)
					newItems.Add(behavior);
			} else if(content is ItemsControl) {
				var ic = content as ItemsControl;
				newItems = ic.Items.OfType<Behavior>().ToList();
				ic.Items.Clear();
				ic.ItemsSource = null;
			} else
				throw new InvalidOperationException("Use ContentControl or ItemsControl in the template to specify Behaviors.");
			d.SetValue(BehaviorsTemplateItemsProperty, newItems.Count > 0 ? newItems : null);
			foreach(Behavior behavior in newItems)
				objectBehaviors.Add(behavior);
		}
	}
}
