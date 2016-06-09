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
using System.Windows.Media;
using System.Windows.Controls;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class UIElementPresenter : psvDecorator {
		#region static
		public static readonly DependencyProperty UIElementProperty;
		static UIElementPresenter() {
			var dProp = new DependencyPropertyRegistrator<UIElementPresenter>();
			dProp.Register("UIElement", ref UIElementProperty, (UIElement)null,
				(dObj, e) => ((UIElementPresenter)dObj).OnUIElementChanged((UIElement)e.NewValue));
		}
		#endregion static
		protected override void OnDispose() {
			ClearValue(ChildProperty);
			ClearValue(DataContextProperty);
			base.OnDispose();
		}
		public UIElement UIElement {
			get { return (UIElement)GetValue(UIElementProperty); }
			set { SetValue(UIElementProperty, value); }
		}
		void OnUIElementChanged(UIElement element) {
			if(element != null) {
				DependencyObject parent = VisualTreeHelper.GetParent(element);
				var parentPresenter = parent as UIElementPresenter;
				if(parentPresenter != null)
					parentPresenter.Child = null;
				var detachedDecorator = parent as DocumentSelectorPreview.DetachedElementDecorator;
				if(detachedDecorator != null) 
					detachedDecorator.Child = null;
			}
			Child = element;
		}
		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
			return new UIElementCollection(this, null);
		}
	}
}
