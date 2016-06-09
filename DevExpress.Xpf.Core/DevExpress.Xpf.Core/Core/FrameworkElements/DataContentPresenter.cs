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
using System.Windows.Controls;
using System.Collections;
using DevExpress.Xpf.Editors;
#if SL
using ContentPresenter = DevExpress.Xpf.Core.DataContentPresenterBaseClass;
#endif
namespace DevExpress.Xpf.Core {
	public partial class DataContentPresenter : ContentPresenter {
		DataTemplate lastTemplate;
		InnerContentChangedEventHandler<DataContentPresenter> InnerContentChangedEventHandler { get; set; }
	static Action<DataContentPresenter, object, EventArgs> changed = (owner, o, e) => owner.OnInnerContentChanged(o, e);
		public DataContentPresenter() {
			InnerContentChangedEventHandler = new InnerContentChangedEventHandler<DataContentPresenter>(this, changed);
		}
		void OnContentChanged(object oldValue) {
			if(oldValue != null && oldValue is INotifyContentChanged)
				((INotifyContentChanged)oldValue).ContentChanged -= InnerContentChangedEventHandler.Handler;
			if(Content != null && Content is INotifyContentChanged)
				((INotifyContentChanged)Content).ContentChanged += InnerContentChangedEventHandler.Handler;
		}
		protected void OnInnerContentChanged(object sender, EventArgs e) {
			if(CanRefreshContent())
				OnInnerContentChangedCore();
		}
		protected virtual void OnInnerContentChangedCore() {
			if(lastTemplate != base.ChooseTemplate()) {
				object currentValue = Content;
#if !SL
				Content = null;
				Content = currentValue;
#else
				UpdateContentTemplate();
#endif
				OnContentInvalidated();
			}
		}
		protected virtual bool CanRefreshContent() {
			return true;
		}
		protected override DataTemplate ChooseTemplate() {
			lastTemplate = base.ChooseTemplate();
			return lastTemplate;
		}
		protected virtual void OnContentInvalidated() {
		}
#if !SL
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property == ContentProperty) {
				OnContentChanged(e.OldValue);
			}
		}
#endif
	}	
}
