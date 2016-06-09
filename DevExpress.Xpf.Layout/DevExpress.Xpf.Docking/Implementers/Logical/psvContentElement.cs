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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Docking.VisualElements;
namespace DevExpress.Xpf.Docking {
	public class psvContentElement : DependencyObject {
		public void BeginAnimation(DependencyProperty property, System.Windows.Media.Animation.Timeline animation) {
			AnimationHelper.BeginAnimation(this, property, animation);
		}
	}
	public abstract class psvFrameworkElement : Control {
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty WidthInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty HeightInternalProperty;
		static psvFrameworkElement() {
			var dProp = new DependencyPropertyRegistrator<psvFrameworkElement>();
			dProp.Register("WidthInternal", ref WidthInternalProperty, double.NaN, 
				(dObj,e)=>((psvFrameworkElement)dObj).OnWidthInternalChanged((double)e.OldValue,(double)e.NewValue));
			dProp.Register("HeightInternal", ref HeightInternalProperty, double.NaN,
				(dObj, e) => ((psvFrameworkElement)dObj).OnHeightInternalChanged((double)e.OldValue, (double)e.NewValue));
		}
		public psvFrameworkElement() {
			Loaded += new RoutedEventHandler(psvFrameworkElement_Loaded);
			Unloaded += new RoutedEventHandler(psvFrameworkElement_Unloaded);
			this.StartListen(WidthInternalProperty, "Width");
			this.StartListen(HeightInternalProperty, "Height");
		}
		void psvFrameworkElement_Unloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
		}
		void psvFrameworkElement_Loaded(object sender, RoutedEventArgs e) {
			OnLoaded();
		}
		protected virtual void OnLoaded() { }
		protected virtual void OnUnloaded() { }
		protected abstract void OnWidthInternalChanged(double oldValue, double newValue);
		protected abstract void OnHeightInternalChanged(double oldValue, double newValue);
	}
}
