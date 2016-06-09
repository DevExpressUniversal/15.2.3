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

using System.Collections.Generic;
using System.Windows;
using System.Linq;
using DevExpress.Mvvm.Native;
#if !FREE && !NETFX_CORE
using DevExpress.Mvvm.UI.Native;
#endif
using System;
#if !FREE
using DevExpress.Xpf.Core.Native;
#endif
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media;
using System.Windows.Media.Media3D;
#endif
namespace DevExpress.Mvvm.UI {
	public static class LayoutTreeHelper {
		static DependencyObject GetParent(DependencyObject element) {
#if !SILVERLIGHT && !NETFX_CORE
			if(element is Visual || element is Visual3D) 
				return VisualTreeHelper.GetParent(element);
			if(element is FrameworkContentElement)
				return LogicalTreeHelper.GetParent(element);
			return null;
#else
			return VisualTreeHelper.GetParent(element);
#endif
		}
		public static IEnumerable<DependencyObject> GetVisualParents(DependencyObject child, DependencyObject stopNode = null) {
			return GetVisualParentsCore(child, stopNode, false);
		}
		public static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject parent) {
			return GetVisualChildrenCore(parent, false);
		}
		internal static IEnumerable<DependencyObject> GetVisualParentsCore(DependencyObject child, DependencyObject stopNode, bool includeStartNode) {
			var result = LinqExtensions.Unfold(child, x => x != stopNode ? GetParent(x) : null, x => x == null);
			return includeStartNode ? result : result.Skip(1);
		}
		internal static IEnumerable<DependencyObject> GetVisualChildrenCore(DependencyObject parent, bool includeStartNode) {
			var result = parent
				.Yield()
				.Flatten(x => Enumerable.Range(0, x != null ? VisualTreeHelper.GetChildrenCount(x) : 0).Select(index => VisualTreeHelper.GetChild(x, index)));
			return includeStartNode ? result : result.Skip(1);
		}
	}
}
