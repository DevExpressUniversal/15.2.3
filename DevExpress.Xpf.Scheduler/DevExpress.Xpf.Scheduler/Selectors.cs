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
using System.Windows.Controls;
using System.Windows;
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region TimelineColumnHeaderTemplateSelector
	public class TimelineColumnHeaderTemplateSelector : DataTemplateSelector {
		DataTemplate topHeader;
		DataTemplate otherHeader;
		public TimelineColumnHeaderTemplateSelector() {
		}
		public DataTemplate TopHeader {
			get { return topHeader; }
			set { topHeader = value; }
		}
		public DataTemplate OtherHeader {
			get { return otherHeader; }
			set { otherHeader = value; }
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
			if(itemsControl == null)
				return null;
			int index = itemsControl.ItemContainerGenerator.IndexFromContainer(container);
			return index == 0 ? TopHeader : OtherHeader;
		}
	}
	#endregion     
	public class GanttResourceTemplateSelector : DataTemplateSelector {
		public DataTemplate RootNode { get; set; }
		public DataTemplate ChildNode { get; set; }
		public DataTemplate CollapsedNode { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			VisualGanttResource resource = item as VisualGanttResource;
			if (resource == null)
				return null;
			if (!resource.IsExpanded)
				return CollapsedNode;
			return RootNode;
		}
	}
}
