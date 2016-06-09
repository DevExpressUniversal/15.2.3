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
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.Web {
	public class DataViewItemCommandEventArgs : CommandEventArgs {
		private object fCommandSource = null;
		private DataViewItem fItem = null;
		public object CommandSource {
			get { return fCommandSource; }
		}
		public DataViewItem Item {
			get { return fItem; }
		}
		public DataViewItemCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			fCommandSource = commandSource;
		}
		public DataViewItemCommandEventArgs(DataViewItem item, object commandSource, CommandEventArgs originalArgs)
			: this(commandSource, originalArgs) {
			fItem = item;
		}
	}
	public delegate void DataViewItemCommandEventHandler(object source, DataViewItemCommandEventArgs e);
	public class DataViewPagerPanelCommandEventArgs: CommandEventArgs {
		private object fCommandSource = null;
		private PagerPanelPosition fPosition = PagerPanelPosition.Top;
		private PagerPanelTemplatePosition fTemplatePosition = PagerPanelTemplatePosition.Left;
		public object CommandSource {
			get { return fCommandSource; }
		}
		public PagerPanelPosition Position {
			get { return fPosition; }
		}
		public PagerPanelTemplatePosition TemplatePosition {
			get { return fTemplatePosition; }
		}
		public DataViewPagerPanelCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			fCommandSource = commandSource;
		}
		public DataViewPagerPanelCommandEventArgs(PagerPanelPosition position, PagerPanelTemplatePosition templatePosition, 
			object commandSource, CommandEventArgs originalArgs)
			: this(commandSource, originalArgs) {
			fPosition = position;
			fTemplatePosition = templatePosition;
		}
	}
	public delegate void DataViewPagerPanelCommandEventHandler(object source, DataViewPagerPanelCommandEventArgs e);
	public class DataViewPageEventArgs: CancelEventArgs {
		private int fNewPageIndex;
		public int NewPageIndex {
			get { return fNewPageIndex; }
			set { fNewPageIndex = value; }
		}
		public DataViewPageEventArgs(int newPageIndex)
			: base() {
			fNewPageIndex = newPageIndex;
		}
	}
	public class DataViewPageSizeEventArgs : CancelEventArgs {
		private int fNewPageSize;
		public int NewPageSize {
			get { return fNewPageSize; }
			set { fNewPageSize = value; }
		}
		public DataViewPageSizeEventArgs(int newPageSize)
			: base() {
			fNewPageSize = newPageSize;
		}
	}
	public delegate void DataViewPageEventHandler(object source, DataViewPageEventArgs e);
	public delegate void DataViewPageSizeEventHandler(object source, DataViewPageSizeEventArgs e);
}
