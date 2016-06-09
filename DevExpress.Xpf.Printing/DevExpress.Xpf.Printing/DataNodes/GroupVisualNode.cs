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
using System.Windows.Data;
using DevExpress.XtraPrinting.DataNodes;
using System.Windows.Controls;
namespace DevExpress.Xpf.Printing.DataNodes {
	class GroupVisualNode : VisualNodeBase, IVisualGroupNode {
		readonly CollectionViewGroup group;
		int level = -1;
		int Level { 
			get {
				if(level == -1)
					level = GetLevel();
				return level;
			}
		}
		public GroupUnion Union {
			get {
				return GroupInfo != null ? GroupInfo.Union : GroupUnion.None;
			}
		}
		public bool RepeatHeaderEveryPage {
			get {
				return GroupInfo != null ? GroupInfo.RepeatHeaderEveryPage : false;
			}
		}
		public override bool PageBreakAfter {
			get {
				return GroupInfo != null ? GroupInfo.PageBreakAfter : false;
			}
		}
		public override bool PageBreakBefore {
			get {
				return GroupInfo != null ? GroupInfo.PageBreakBefore : false;
			}
		}
		internal GroupInfo GroupInfo {
			get {
				if(Root.GroupInfos.Count == 0)
					return null;
				return Root.GroupInfos[Math.Min(Level, Root.GroupInfos.Count - 1)];
			}
		}
		public GroupVisualNode(IDataNode parent, int index, CollectionViewGroup group)
			: base(parent, index) {
			this.group = group;
		}
		#region DataNodeBase overrides
		public override bool CanGetChild(int index) {
			return index >= 0 && index < group.Items.Count;
		}
		protected override IDataNode CreateChildNode(int index) {
			object data = group.Items[index];
			CollectionViewGroup collectionViewGroup = data as CollectionViewGroup;
			return collectionViewGroup != null ? (IDataNode)new GroupVisualNode(this, index, collectionViewGroup) : 
				(IDataNode)new VisualNode(this, index, data);
		}
		public override bool IsDetailContainer {
			get { return !(group.Items[0] is CollectionViewGroup); }
		}
		#endregion
		#region IVisualGroupNode Members
		public RowViewInfo GetHeader(bool allowContentReuse) {
			if(GroupInfo == null || GroupInfo.HeaderTemplate == null)
				return null;
			return new RowViewInfo(GroupInfo.HeaderTemplate, group);
		}
		public RowViewInfo GetFooter(bool allowContentReuse) {
			if(GroupInfo == null || GroupInfo.FooterTemplate == null)
				return null;
			return new RowViewInfo(GroupInfo.FooterTemplate, group);
		}
		#endregion
	}
}
