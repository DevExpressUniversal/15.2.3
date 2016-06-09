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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Interop;
namespace DevExpress.DemoData.Helpers {
	public class UniversalControl : Control, ISupportInitialize {
		#region Dependency Properties
		public static readonly DependencyProperty IsBrowserHostedProperty;
		static UniversalControl() {
			Type ownerType = typeof(UniversalControl);
			IsBrowserHostedProperty = DependencyProperty.Register("IsBrowserHosted", typeof(bool), ownerType, new PropertyMetadata(GetIsBrowserHosted()));
		}
		#endregion
		bool onLoadedInvoked = false;
		Dictionary<object, bool> logicalChildren = new Dictionary<object, bool>();
		public UniversalControl() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			GoToStateCore(false);
		}
		public bool IsBrowserHosted { get { return (bool)GetValue(IsBrowserHostedProperty); } }
		public event EventHandler ActualLoaded;
		public event EventHandler Initialize;
		public static bool GetIsBrowserHosted() {
			return BrowserInteropHelper.IsBrowserHosted;
		}
		public sealed override void BeginInit() {
			base.BeginInit();
			BeginInitCore();
		}
		public sealed override void EndInit() {
			EndInitCore();
			base.EndInit();
		}
		protected virtual void BeginInitCore() { }
		protected virtual void EndInitCore() { }
		public sealed override void OnApplyTemplate() {
			base.OnApplyTemplate();
			OnApplyTemplateOverride();
			if(Initialize != null)
				Initialize(this, EventArgs.Empty);
		}
		protected virtual void OnApplyTemplateOverride() { }
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			GoToStateCore(false);
			if(!onLoadedInvoked) {
				onLoadedInvoked = true;
				RaiseActualLoaded();
			}
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
		}
		protected virtual void RaiseActualLoaded() {
			if(ActualLoaded != null)
				ActualLoaded(this, EventArgs.Empty);
		}
		protected static DependencyProperty RegisterContentProperty(string name, Type propertyType, Type ownerType, object defaultValue, PropertyChangedCallback changedCallback) {
			PropertyChangedCallback callback = (d, e) => {
				UniversalControl control = (UniversalControl)d;
				object oldChild = e.OldValue;
				object newChild = e.NewValue;
				if(oldChild != null) {
					if(control.logicalChildren.ContainsKey(oldChild))
						control.logicalChildren.Remove(oldChild);
					control.RemoveLogicalChild(oldChild);
				}
				if(newChild != null) {
					control.AddLogicalChild(newChild);
					control.logicalChildren.Add(newChild, true);
				}
				if(changedCallback != null)
					changedCallback(d, e);
			};
			return DependencyProperty.Register(name, propertyType, ownerType, new PropertyMetadata(defaultValue, callback));
		}
		protected override IEnumerator LogicalChildren {
			get {
				IEnumerator baseEnumerator = GetBaseLogicalChildren();
				if(baseEnumerator != null) {
					while(baseEnumerator.MoveNext())
						yield return baseEnumerator.Current;
				}
				foreach(object child in logicalChildren.Keys)
					yield return child;
			}
		}
		IEnumerator GetBaseLogicalChildren() { 
			return base.LogicalChildren;
		}
		protected void GoToState(bool useTransitions) {
			if(IsLoaded)
				GoToStateCore(useTransitions);
		}
		protected virtual void GoToStateCore(bool useTransitions) { }
	}
	[ContentProperty("Content")]
	public class UniversalContentControl : UniversalControl {
		#region Dependency Properties
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		static UniversalContentControl() {
			Type ownerType = typeof(UniversalContentControl);
			ContentProperty = DependencyProperty.Register("Content", typeof(object), ownerType, new PropertyMetadata(null));
			ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null));
		}
		#endregion
		public UniversalContentControl() {
			HorizontalContentAlignment = HorizontalAlignment.Left;
			DefaultStyleKey = typeof(UniversalContentControl);
		}
		public object Content { get { return GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }
		public DataTemplate ContentTemplate { get { return (DataTemplate)GetValue(ContentTemplateProperty); } set { SetValue(ContentTemplateProperty, value); } }
	}
	public class UniversalUserControl : UserControl {
		bool onLoadedInvoked = false;
		public UniversalUserControl() {
			Loaded += OnLoaded;
		}
		public event EventHandler ActualLoaded;
		void OnLoaded(object sender, RoutedEventArgs e) {
			if(!onLoadedInvoked) {
				onLoadedInvoked = true;
				RaiseActualLoaded();
			}
		}
		protected virtual void RaiseActualLoaded() {
			if(ActualLoaded != null)
				ActualLoaded(this, EventArgs.Empty);
		}
	}
}
