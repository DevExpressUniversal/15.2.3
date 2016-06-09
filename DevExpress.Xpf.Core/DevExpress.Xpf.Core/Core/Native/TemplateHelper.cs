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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace DevExpress.Xpf.Core.Native {
	public class TemplateHelper {
		public static ContentPresenter CreateBindedContentPresenter(object source, string contentTemplatePropertyName, object dataContext) {
			ContentPresenter presenter = new ContentPresenter();
			presenter.Content = dataContext;
			Binding binding = new Binding(contentTemplatePropertyName) { Source = source };
			presenter.SetBinding(ContentPresenter.ContentTemplateProperty, binding);
			return presenter;
		}
		public static ContentPresenter CreateBindedContentPresenter(object source, string contentTemplatePropertyName) {
			return CreateBindedContentPresenter(source, contentTemplatePropertyName, null);
		}
		public static T LoadFromTemplate<T>(DataTemplate template) where T : class {
			T loadedItem = null;
			object loadedContent = template != null ? template.LoadContent() : null;
			if(loadedContent != null) {
				if(loadedContent is T) {
					loadedItem = (T)loadedContent;
				} else if(loadedContent is ContentControl) {
					loadedItem = (T)((ContentControl)loadedContent).Content;
					((ContentControl)loadedContent).Content = null;
				} else if(loadedContent is ContentPresenter) {
					loadedItem = (T)((ContentPresenter)loadedContent).Content;
					((ContentPresenter)loadedContent).Content = null;
				} else {
					loadedItem = (T)loadedContent; 
				}
			}
			return loadedItem;
		}
	}
}
