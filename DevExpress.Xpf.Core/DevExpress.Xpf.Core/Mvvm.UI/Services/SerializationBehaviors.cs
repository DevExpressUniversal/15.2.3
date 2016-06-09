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

using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Serialization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
namespace DevExpress.Mvvm.UI {
	[Obsolete]
	public class ApplicationThemeSerializationBehavior : Behavior<DependencyObject> {
		public ApplicationThemeSerializationBehavior() {
			DXSerializer.SetSerializationID(this, "applicationThemeId");
		}
		[XtraSerializableProperty]
		public string ApplicationThemeName {
			get { return ThemeManager.ApplicationThemeName; }
			set { ThemeManager.ApplicationThemeName = value; }
		}
	}
	public class CurrentWindowSerializationBehavior : Behavior<DependencyObject> {
		List<Tuple<string, Action<Window>>> onceLoaded = new List<Tuple<string, Action<Window>>>();
		Window window = null;
		FrameworkElement fe = null;
		bool initialized = false;
		double normalStateWidth = 0;
		double normalStateHeight = 0;
		static CurrentWindowSerializationBehavior() {
			DXSerializer.SerializationIDDefaultProperty.OverrideMetadata(typeof(CurrentWindowSerializationBehavior), new UIPropertyMetadata("activeWindowId"));
		}
		void TryGetWindow(FrameworkElement fe) {
			window = window
				?? Window.GetWindow(fe)
				?? LayoutTreeHelper.GetVisualParents(AssociatedObject).OfType<Window>().FirstOrDefault()
				?? (Application.Current == null ? null : Application.Current.Windows.OfType<Window>().FirstOrDefault());
		}
		protected override void OnAttached() {
			base.OnAttached();
			fe = AssociatedObject as FrameworkElement;
			if(fe == null)
				return;
			EventHandler handler = null;
			TryGetWindow(fe);
			HandleAssociatedObjectInitialized();
			handler = (s, e) => {
				TryGetWindow(fe);
				HandleAssociatedObjectInitialized();
				window_SizeChanged(null, null);
				fe.Initialized -= handler;
			};
			fe.Initialized += handler;
			RoutedEventHandler loadedHandler = null;
			loadedHandler = (s, e) => {
				TryGetWindow(fe);
				window_SizeChanged(null, null);
				fe.Loaded -= loadedHandler;
			};
			fe.Loaded += loadedHandler;
		}
		void HandleAssociatedObjectInitialized() {
			if(initialized || window == null)
				return;
			initialized = true;
			window.SizeChanged += window_SizeChanged;
			onceLoaded
				.GroupBy(x => x.Item1)
				.Select(x => x.Last().Item2)
				.ForEach(x => x(window));
			onceLoaded = null;
		}
		void window_SizeChanged(object sender, SizeChangedEventArgs e) {
			if(window != null && window.WindowState == System.Windows.WindowState.Normal) {
				normalStateWidth = window.Width;
				normalStateHeight = window.Height;
			}
		}
		void DoOrPostpone(string id, Action<Window> action) {
			if(initialized) {
				if(window != null) {
					action(window);
				}
			} else {
				onceLoaded.Add(Tuple.Create(id, action));
			}
		}
		[XtraSerializableProperty]
		public WindowState WindowState {
			get {
				if(window == null || window.WindowState == System.Windows.WindowState.Minimized)
					return System.Windows.WindowState.Normal;
				return window.WindowState;
			}
			set { DoOrPostpone(BindableBase.GetPropertyName(() => WindowState), w => w.WindowState = value); }
		}
		[XtraSerializableProperty]
		public double Width {
			get { return normalStateWidth; }
			set { DoOrPostpone(BindableBase.GetPropertyName(() => Width), w => w.Width = value); }
		}
		[XtraSerializableProperty]
		public double Height {
			get { return normalStateHeight; }
			set { DoOrPostpone(BindableBase.GetPropertyName(() => Height), w => w.Height = value); }
		}
	}
}
