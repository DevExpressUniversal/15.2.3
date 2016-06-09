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
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	public class TitleIndexItemEventArgs : EventArgs {
		private TitleIndexItem fItem = null;
		public TitleIndexItem Item {
			get { return fItem; }
		}
		public TitleIndexItemEventArgs(TitleIndexItem item)
			: base() {
			fItem = item;
		}
	}
	public delegate void TitleIndexItemEventHandler(object source, TitleIndexItemEventArgs e);
	public class TitleIndexItemCommandEventArgs : CommandEventArgs {
		private object fCommandSource = null;
		private TitleIndexItem fItem = null;
		public object CommandSource {
			get { return fCommandSource; }
		}
		public TitleIndexItem Item {
			get { return fItem; }
		}
		public TitleIndexItemCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			fCommandSource = commandSource;
		}
		public TitleIndexItemCommandEventArgs(TitleIndexItem item, object commandSource, CommandEventArgs originalArgs)
			: this(commandSource, originalArgs) {
			fItem = item;
		}
	}
	public delegate void TitleIndexItemCommandEventHandler(object source, TitleIndexItemCommandEventArgs e);
	public class GroupHeaderCommandEventArgs : CommandEventArgs {
		private object fCommandSource = null;
		private object fGroupValue = null;
		public object CommandSource {
			get { return fCommandSource; }
		}
		public object GroupValue {
			get { return fGroupValue; }
		}
		public GroupHeaderCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			fCommandSource = commandSource;
		}
		public GroupHeaderCommandEventArgs(object groupValue, object commandSource, CommandEventArgs originalArgs)
			: this(commandSource, originalArgs) {
			fGroupValue = groupValue;
		}
	}
	public delegate void GroupHeaderCommandEventHandler(object source, GroupHeaderCommandEventArgs e);
	public class IndexPanelItemCommandEventArgs : CommandEventArgs {
		private object fCommandSource = null;
		private object fGroupValue = null;
		public object CommandSource {
			get { return fCommandSource; }
		}
		public object GroupValue {
			get { return fGroupValue; }
		}
		public IndexPanelItemCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			fCommandSource = commandSource;
		}
		public IndexPanelItemCommandEventArgs(object groupValue, object commandSource, CommandEventArgs originalArgs)
			: this(commandSource, originalArgs) {
			fGroupValue = groupValue;
		}
	}
	public delegate void IndexPanelItemCommandEventHandler(object source, IndexPanelItemCommandEventArgs e);
}
