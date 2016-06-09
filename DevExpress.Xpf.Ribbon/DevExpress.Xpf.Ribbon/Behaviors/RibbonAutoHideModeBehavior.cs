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

using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Ribbon.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Ribbon {
	[TargetTypeAttribute(typeof(BarCheckItem))]
	public class RibbonAutoHideModeBehavior : Behavior<BarCheckItem> {
		protected object StyleKey { get; private set; }
		protected RibbonControl Ribbon {
			get { return ribbon; }
			set {
				if(ribbon == value)
					return;
				var oldValue = ribbon;
				ribbon = value;
				OnRibbonChanged(oldValue);
			}
		}
		public RibbonAutoHideModeBehavior() {
			StyleKey = CreateStyleKey();
		}
		void OnRibbonChanged(RibbonControl oldValue) {
			SetRibbonForCheckItem();
		}
		void SetRibbonForCheckItem() {
			if(AssociatedObject != null)
				AssociatedObject.Tag = Ribbon;
		}
		protected override void OnAttached() {
			base.OnAttached();
			SetRibbonForCheckItem();
			AssociatedObject.SetResourceReference(BarCheckItem.StyleProperty, StyleKey);
			AssociatedObject.Loaded += OnAssociatedObjectLoaded;
			if(AssociatedObject.IsLoaded)
				UpdateRibbon();
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
			AssociatedObject.ClearValue(BarCheckItem.StyleProperty);
			AssociatedObject.Tag = null;
			Ribbon = null;
		}
		protected virtual void UpdateRibbon() {
			Ribbon = RibbonControl.GetRibbon(AssociatedObject) ?? LayoutHelper.FindLayoutOrVisualParentObject<RibbonControl>(AssociatedObject, true);
		}
		protected virtual object CreateStyleKey() {
			return new RibbonAutoHideCheckItemBehaviorThemeKeyExtension() { ResourceKey = RibbonAutoHideCheckItemBehaviorThemeKeys.Style, IsThemeIndependent = true };
		}
		protected virtual void OnAssociatedObjectLoaded(object sender, System.Windows.RoutedEventArgs e) {
			UpdateRibbon();
		}
		RibbonControl ribbon;
	}
}
