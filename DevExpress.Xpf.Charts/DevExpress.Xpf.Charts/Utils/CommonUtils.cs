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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts.Native {
	public static class CommonUtils {
		public static Binding CreateBinding(DependencyProperty property, object source) {
			Binding binding = new Binding(property.GetName());
			binding.Source = source;
			return binding;
		}
		public static Binding CreateBinding(string path, object source){
			Binding binding = new Binding(path);
			binding.Source = source;
			return binding;
		}
		public static DoubleCollection CloneDoubleCollection(DoubleCollection collection) {
			DoubleCollection collectionClone = new DoubleCollection();
			if (collection != null)
				foreach (double value in collection)
					collectionClone.Add(value);
			return collectionClone;
		}
		static void InvalidateRecursive(UIElement element, bool invalidateMeasure, bool invalidateArrange) {
			if (element != null) {
				if (invalidateMeasure)
					element.InvalidateMeasure();
				if (invalidateArrange)
					element.InvalidateArrange();
				if (element is IFinishInvalidation)
					return;
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
					InvalidateRecursive(VisualTreeHelper.GetChild(element, i) as UIElement, invalidateMeasure, invalidateArrange);
			}
		}
		public static void InvalidateChildren(UIElement element, Predicate<UIElement> predicate, bool invalidateMeasure, bool invalidateArrange) {
			if (element != null) {
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++) {
					UIElement child = VisualTreeHelper.GetChild(element, i) as UIElement;
					if (child != null) {
						if (predicate(child)) {
							if (invalidateMeasure)
								child.InvalidateMeasure();
							if (invalidateArrange)
								child.InvalidateArrange();
						}
						else
							InvalidateChildren(child, predicate, invalidateMeasure, invalidateArrange);
					}
				}
			}
		}
		public static void InvalidateMeasure(UIElement element) {
			InvalidateRecursive(element, true, false);
		}
		public static void InvalidateArrange(UIElement element) {
			InvalidateRecursive(element, false, true);
		}
		public static void SubscribePropertyChangedWeakEvent(INotifyPropertyChanged oldSource, INotifyPropertyChanged newSource, IWeakEventListener listener) {
			if (listener != null) {
				if (oldSource != null)
					PropertyChangedWeakEventManager.RemoveListener(oldSource, listener);
				if (newSource != null)
					PropertyChangedWeakEventManager.AddListener(newSource, listener);
			}
		}
	}	
}
