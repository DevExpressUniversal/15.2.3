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
namespace DevExpress.Xpf.WindowsUI.Base {
	public abstract class veContentSelectorItem : veSelectorItem, IContentSelectorItem {
		protected internal virtual ClickMode SelectionMode { get { return ClickMode.Press; } }
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if(SelectionMode == ClickMode.Press)
				InvokeSelectInParentContainer();
		}
		protected internal void InvokeSelectInParentContainer() {
			Dispatcher.BeginInvoke(new Action(() =>
			{
				SelectInParentContainer();
			}));
		}
		protected virtual bool SelectInParentContainer() {
			var templatedParent = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<veItemsControl>(this);
			ISelector contentSelector = Owner as ISelector ?? templatedParent as ISelector;
			if(contentSelector != null) {
				contentSelector.Select(this);
			}
			return contentSelector != null;
		}
		protected override void OnIsSelectedChanged(bool isSelected) {
			base.OnIsSelectedChanged(isSelected);
			if(isSelected) {
				var templatedParent = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<veItemsControl>(this);
				ISelector contentSelector = Owner as ISelector ?? templatedParent as ISelector;
				if(contentSelector != null)
					contentSelector.Select(this);
			}
		}
		void NotifyOwner() {
			ISelector selector = Owner as ISelector;
			if(selector != null) selector.UpdateSelection();
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			NotifyOwner();
		}
		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate) {
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
			NotifyOwner();
		}
		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector) {
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);
			NotifyOwner();
		}
	}
}
