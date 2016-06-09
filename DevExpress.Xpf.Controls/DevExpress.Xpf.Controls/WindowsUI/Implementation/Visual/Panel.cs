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
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class vePanel : Panel, IDisposable {
		#region static
#if SILVERLIGHT
		public static readonly DependencyProperty ZIndexProperty;
		public static int GetZIndex(DependencyObject dObj) {
			return (int)dObj.GetValue(ZIndexProperty);
		}
		public static void SetZIndex(DependencyObject dObj, int value) {
			dObj.SetValue(ZIndexProperty, value);
		}
		static void OnZIndexChanged(DependencyObject dObj, SLDependencyPropertyChangedEventArgs ea) {
			if(dObj is UIElement) dObj.SetValue(Canvas.ZIndexProperty, ea.NewValue);
		}
#endif
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ActualSizeProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty DataContextInternalProperty;
		static vePanel() {
			var dProp = new DependencyPropertyRegistrator<vePanel>();
#if SILVERLIGHT
			dProp.RegisterAttached("ZIndex", ref ZIndexProperty, -1, OnZIndexChanged);
#endif
			dProp.Register("ActualSize", ref ActualSizeProperty, Size.Empty,
				(dObj, e) => ((vePanel)dObj).OnActualSizeChanged((Size)e.NewValue));
			dProp.Register("DataContextInternal", ref DataContextInternalProperty, (object)null,
				(dObj, e) => ((vePanel)dObj).OnDataContextChanged(e.NewValue, e.OldValue));
		}
		#endregion static
		public vePanel() {
			Focusable = false;
#if SILVERLIGHT
			SizeChanged += new SizeChangedEventHandler(psvPanel_SizeChanged);
#endif
			SetBinding(DataContextInternalProperty, new System.Windows.Data.Binding());
			Loaded += psvPanel_Loaded;
		}
		void psvPanel_Loaded(object sender, RoutedEventArgs e) {
			OnLoaded();
		}
		public bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				Loaded -= psvPanel_Loaded;
#if SILVERLIGHT
				SizeChanged -= psvPanel_SizeChanged;
#endif
				ClearValue(ActualSizeProperty);
				OnDispose();
			}
			GC.SuppressFinalize(this);
		}
#if !SILVERLIGHT
		protected sealed override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			SetValue(ActualSizeProperty, sizeInfo.NewSize);
			base.OnRenderSizeChanged(sizeInfo);
		}
#else
		public bool Focusable { get; set; }
		protected UIElementCollection InternalChildren { get { return Children; } }
		protected virtual System.Windows.Media.Geometry GetLayoutClip(Size layoutSlotSize) {
			throw new NotImplementedException();
		}
		public void BeginAnimation(DependencyProperty property, Timeline animation) {
			AnimationHelper.BeginAnimation(this, property, animation);
		}
		void psvPanel_SizeChanged(object sender, SizeChangedEventArgs e) {
			OnActualSizeChanged(RenderSize);
		}
#endif
		protected virtual void OnLoaded() { }
		protected virtual void OnDispose() { }
		protected virtual void OnActualSizeChanged(Size value) { }
		protected virtual void OnDataContextChanged(object newValue, object oldValue) { }
	}
}
