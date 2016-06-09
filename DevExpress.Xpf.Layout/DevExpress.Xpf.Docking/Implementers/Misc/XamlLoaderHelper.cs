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
namespace DevExpress.Xpf.Docking {
	static class XamlLoaderHelper {
		static void TryPreventWindowShowing(ContentControl control) {
			Window w = control as Window;
			if(w != null) w.Close();
		}
		public static object LoadContentFromUri(Uri uri) {
			object content = null;
			object component = Application.LoadComponent(uri);
			ContentControl contentControl = component as ContentControl;
			if(contentControl != null) {
				if(!DevExpress.Xpf.Docking.Platform.WindowHelper.IsXBAP)
					TryPreventWindowShowing(contentControl);
				if(contentControl is UserControl) {
					content = contentControl;
				}
				else {
					content = contentControl.Content;
					contentControl.Content = null;
				}
			}
			Page page = component as Page;
			if(page != null) {
				content = page.Content;
				page.Content = null;
			}
			if(content != component && content is DependencyObject && component is DependencyObject)
				NameScope.SetNameScope((DependencyObject)content, NameScope.GetNameScope((DependencyObject)component));
			return content;
		}
	}
}
