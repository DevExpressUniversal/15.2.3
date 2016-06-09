﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Windows.Threading;
namespace DevExpress.Xpf.Printing.Native {
	public static class SearchPanelBehavior {
		public static readonly DependencyProperty SearchBoxFocusedProperty =
			DependencyProperty.RegisterAttached(
				"SearchBoxFocused",
				typeof(bool),
				typeof(SearchPanelBehavior),
				new PropertyMetadata(SearchBoxFocusedChanged));
		private static void SearchBoxFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PrintingSearchPanel searchPanel = d as PrintingSearchPanel;
			if(searchPanel == null)
				return;
			if(object.Equals(e.NewValue, true)) {
				Action action = searchPanel.FocusSearchBox;
#if SILVERLIGHT
				Deployment.Current.Dispatcher.BeginInvoke(action);
#else
				Dispatcher.CurrentDispatcher.BeginInvoke(action, DispatcherPriority.Background);
#endif
			}
		}
		public static bool GetSearchBoxFocused(DependencyObject obj) {
			return (bool)obj.GetValue(SearchBoxFocusedProperty);
		}
		public static void SetSearchBoxFocused(DependencyObject obj, bool value) {
			obj.SetValue(SearchBoxFocusedProperty, value);
		}
	}
}
