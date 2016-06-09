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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	public class DXClickHelper {
		#region static stuf
		[IgnoreDependencyPropertiesConsistencyChecker()]
		public static readonly DependencyProperty HandleMouseLeftButtonProperty;
		static DXClickHelper() {
			HandleMouseLeftButtonProperty = DependencyProperty.RegisterAttached("HandleMouseLeftButton", typeof(bool), typeof(DXClickHelper), new PropertyMetadata(true));
		}
		#endregion
		UIElement root;
		UIElement clickedElem;
		ClickHelperDelegate clickDelegate;
		public DXClickHelper(UIElement root, ClickHelperDelegate clickDelegate) {
			this.root = root;
			this.clickDelegate = clickDelegate;
			Root.MouseDown += OnMouseDown;
			Root.MouseUp += OnMouseUp;
			Root.LostMouseCapture += OnLostMouseCapture;
		}
		public UIElement Root { get { return root; } }
		public ClickHelperDelegate ClickDelegate {
			get { return clickDelegate; }
			set { clickDelegate = value; }
		}
		public bool HandleMouseButton {
			get { return (bool)Root.GetValue(HandleMouseLeftButtonProperty); }
			set { Root.SetValue(HandleMouseLeftButtonProperty, value); }
		}
		void OnMouseDown(object sender, MouseButtonEventArgs e) {
			clickedElem = e.OriginalSource as UIElement;
			if(clickedElem == null) return;
			Root.CaptureMouse();
			e.Handled = HandleMouseButton;
		}
		void OnMouseUp(object sender, MouseButtonEventArgs e) {
			if(clickedElem == null) return;
			UIElement parent = GetCommonParent(clickedElem, GetChildElement(e), Root);
			if(Root.IsMouseCaptured)
				Root.ReleaseMouseCapture();
			e.Handled = RaiseClick(parent, e.ChangedButton);
			clickedElem = null;
		}
		void OnLostMouseCapture(object sender, MouseEventArgs e) {
			clickedElem = null;
		}
		bool RaiseClick(UIElement clickedElem, MouseButton button) {
			if(clickDelegate == null) return HandleMouseButton;
			return clickDelegate(clickedElem, button);
		}
		public static UIElement GetCommonParent(UIElement elem1, UIElement elem2, UIElement root) {
			if(elem1 == null || elem2 == null) return null;
			if(elem1 == elem2) return elem1;
			List<DependencyObject> parents1 = GetParents(elem1, root),
				parents2 = GetParents(elem2, root);
			int count = Math.Min(parents1.Count, parents2.Count);
			for(int i = 0; i < count; i++) {
				if(parents1[i] == parents2[i]) continue;
				if(i == 0) return null;
				return parents1[i - 1] as UIElement;
			}
			return parents1[count - 1] as UIElement;
		}
		static List<DependencyObject> GetParents(UIElement elem, UIElement root) {
			List<DependencyObject> parents = new List<DependencyObject>();
			if(elem != root) {
				DependencyObject parent = VisualTreeHelper.GetParent(elem);
				while(parent != null) {
					parents.Insert(0, parent);
					if(parent == root)
						break;
					parent = VisualTreeHelper.GetParent(parent);
				}
			}
			parents.Add(elem);
			return parents;
		}
		public static UIElement GetChildElement(MouseEventArgs args) {
			UIElement source = args.Source as UIElement;
			return GetChildElementCore(source, args.GetPosition(source));
		}
		internal static UIElement GetChildElementCore(UIElement source, Point pt) {
			if(source == null)
				return null;
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(source, pt);
			if(hitTestResult == null)
				return null;
			UIElement elem = LayoutHelper.FindParentObject<UIElement>(hitTestResult.VisualHit);
			if(!elem.IsVisible)
				return null;
			return elem;
		}
	}
	public delegate bool ClickHelperDelegate(UIElement elem, MouseButton button);
}
