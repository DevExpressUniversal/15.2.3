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
using System.ComponentModel;
using System.Windows;
using DevExpress.XtraPrinting.DataNodes;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils.Themes;
namespace DevExpress.Xpf.Printing.DataNodes {
	class VisualRootNode : VisualNodeBase, IRootDataNode {
		readonly ICollectionView collectionView;
		public IList<GroupInfo> GroupInfos { get; private set; }
		internal DataTemplate ItemTemplate { get; private set; }
		public VisualRootNode(ICollectionView collectionView, DataTemplate itemTemplate, IList<GroupInfo> groupInfos)
			: base(null, -1)
		{
			this.collectionView = collectionView;
			this.ItemTemplate = itemTemplate;
			this.GroupInfos = groupInfos;
		}
		#region DataNodeBase overrides
		public override bool CanGetChild(int index) {
			if(collectionView.Groups != null)
				return index >= 0 && index < collectionView.Groups.Count;
			return collectionView.MoveCurrentToPosition(index);
		}
		public override bool IsDetailContainer {
			get {
				return collectionView.Groups == null;
			}
		}
		protected override IDataNode CreateChildNode(int index) {
			if(IsDetailContainer) {
				return new VisualNode(this, index, GetData(index));
			}
			return new GroupVisualNode(this, index, (CollectionViewGroup)collectionView.Groups[index]);
		}
		object GetData(int index) {
			if(!collectionView.MoveCurrentToPosition(index))
				throw new ArgumentOutOfRangeException("index");
			return collectionView.CurrentItem;
		}
		protected override int GetLevel() {
			throw new InvalidOperationException();
		}
		#endregion
		#region IRootDataNode Members
		public int GetTotalDetailCount() {
			if(collectionView.SourceCollection is ICollection) {
				return ((ICollection)collectionView.SourceCollection).Count;
			}
			return -1;
		}
		#endregion
	}
}
