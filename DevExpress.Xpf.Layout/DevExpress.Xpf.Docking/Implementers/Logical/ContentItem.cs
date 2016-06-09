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
using System.Windows.Controls;
using System.Windows.Markup;
namespace DevExpress.Xpf.Docking {
	[ContentProperty("Content")]
	public abstract class ContentItem : BaseLayoutItem {
		#region static
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty IsDataBoundProperty;
		internal static readonly DependencyPropertyKey IsDataBoundPropertyKey;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty FocusContentOnActivatingProperty;
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		static ContentItem() {
			var dProp = new DependencyPropertyRegistrator<ContentItem>();
			dProp.Register("Content", ref ContentProperty, (object)null,
				(dObj, e) => ((ContentItem)dObj).OnContentChanged(e.NewValue, e.OldValue));
			dProp.RegisterReadonly("IsDataBound", ref IsDataBoundPropertyKey, ref IsDataBoundProperty, false,
							(dObj, e) => ((ContentItem)dObj).OnIsDataBoundChanged((bool)e.NewValue));
			dProp.Register("ContentTemplate", ref ContentTemplateProperty, (DataTemplate)null);
			dProp.Register("ContentTemplateSelector", ref ContentTemplateSelectorProperty, (DataTemplateSelector)null);
			dProp.Register("FocusContentOnActivating", ref FocusContentOnActivatingProperty, true);
		}
		#endregion
		internal override void PrepareContainer(object content) {
			if(this != content) {
				Content = content;
				if(content is FrameworkElement) DataContext = ((FrameworkElement)content).DataContext;
			}
		}
		protected virtual void OnContentChanged(object content, object oldContent) { }
		protected virtual void OnIsDataBoundChanged(bool value) {
			if(value)
				BindingHelper.SetBinding(this, DataContextProperty, this, "Content");
			else
				BindingHelper.ClearBinding(this, DataContextProperty);
		}
		protected virtual DependencyObject GetDataBoundContainer() {
			ContentPresenter container = new ContentPresenter();
			BindingHelper.SetBinding(container, ContentPresenter.ContentProperty, this, ContentProperty);
			BindingHelper.SetBinding(container, ContentPresenter.ContentTemplateProperty, this, ContentTemplateProperty);
			BindingHelper.SetBinding(container, ContentPresenter.ContentTemplateSelectorProperty, this, ContentTemplateSelectorProperty);
			return container;
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("ContentItemContent"),
#endif
 Category("Content")]
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsDataBound {
			get { return (bool)GetValue(IsDataBoundProperty); }
			private set { SetValue(IsDataBoundPropertyKey, value); }
		}
		public bool FocusContentOnActivating {
			get { return (bool)GetValue(FocusContentOnActivatingProperty); }
			set { SetValue(FocusContentOnActivatingProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("ContentItemContentTemplate")]
#endif
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("ContentItemContentTemplateSelector")]
#endif
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
	}
}
