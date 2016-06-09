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

using System.Linq;
using System.Windows.Controls;
using System;
using System.Windows;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class psvTreeView : TreeView {
		#region static
		public static readonly DependencyProperty LayoutGroupTemplateProperty;
		public static readonly DependencyProperty LayoutItemTemplateProperty;
		static psvTreeView() {
			var dProp = new DependencyPropertyRegistrator<psvTreeView>();
			dProp.Register("LayoutGroupTemplate", ref LayoutGroupTemplateProperty, (DataTemplate)null);
			dProp.Register("LayoutItemTemplate", ref LayoutItemTemplateProperty, (DataTemplate)null);
		}
		#endregion
		public psvTreeView() {
			ItemTemplateSelector = new psvTreeViewItemDataTemplateSelector(this);
		}
		public DataTemplate LayoutGroupTemplate {
			get { return (DataTemplate)GetValue(LayoutGroupTemplateProperty); }
			set { SetValue(LayoutGroupTemplateProperty, value); }
		}
		public DataTemplate LayoutItemTemplate {
			get { return (DataTemplate)GetValue(LayoutItemTemplateProperty); }
			set { SetValue(LayoutItemTemplateProperty, value); }
		}
		class psvTreeViewItemDataTemplateSelector : DataTemplateSelector {
			psvTreeView Owner;
			public psvTreeViewItemDataTemplateSelector(psvTreeView owner) {
				Owner = owner;
			}
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				return item is LayoutGroup ? Owner.LayoutGroupTemplate : Owner.LayoutItemTemplate;
			}
		}
	}
	public class psvTreeViewItem : TreeViewItem {
	}
}
