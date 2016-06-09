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
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
namespace DevExpress.Xpf.Core {
	public class ConditionalBinding : Behavior<DependencyObject> {
		public static readonly DependencyProperty ConditionProperty =
			DependencyProperty.Register("Condition", typeof(bool), typeof(ConditionalBinding), new PropertyMetadata(false, OnConditionChanged));
		static void OnConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ConditionalBinding)d).UpdateTargetBinding();
		}
		void UpdateTargetBinding() {
			if(AssociatedObject == null)
				return;
			BindingBase binding = Condition ? ConditionTrueBinding : ConditionFalseBinding;
			if(binding == null)
				return;
			FieldInfo dPropertyFieldInfo = AssociatedObject.GetType().GetField(Property + "Property", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			if(dPropertyFieldInfo == null)
				return;
			DependencyProperty dependencyProperty = dPropertyFieldInfo.GetValue(null) as DependencyProperty;
			if(dependencyProperty == null)
				return;
			BindingOperations.SetBinding(AssociatedObject, dependencyProperty, binding);
		}
		protected override void OnAttached() {
			base.OnAttached();
			UpdateTargetBinding();
		}
		public bool Condition {
			get { return (bool)GetValue(ConditionProperty); }
			set { SetValue(ConditionProperty, value); }
		}
		public string Property { get; set; }
		BindingBase conditionTrueBinding;
		public BindingBase ConditionTrueBinding {
			get { return conditionTrueBinding; }
			set {
				if(conditionTrueBinding == value)
					return;
				conditionTrueBinding = value;
				UpdateTargetBinding();
			}
		}
		BindingBase conditionFalseBinding;
		public BindingBase ConditionFalseBinding {
			get { return conditionFalseBinding; }
			set {
				if(conditionFalseBinding == value)
					return;
				conditionFalseBinding = value;
				UpdateTargetBinding();
			}
		}
	}
}
