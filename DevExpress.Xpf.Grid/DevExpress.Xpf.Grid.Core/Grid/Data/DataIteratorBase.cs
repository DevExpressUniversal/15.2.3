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
using System.Collections;
using DevExpress.Xpf.Grid;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class DataIteratorBase {
		protected static DataControllerValuesContainer CreateValuesContainer(DataTreeBuilder treeBuilder, int visibleIndex) {
			int rowHandle = treeBuilder.GetRowHandleByVisibleIndexCore(visibleIndex);
			return new DataControllerValuesContainer(new RowHandle(rowHandle), visibleIndex, treeBuilder.GetRowLevelByControllerRow(rowHandle));
		}
		protected internal static DataControllerValuesContainer CreateValuesContainer(DataTreeBuilder treeBuilder, RowHandle rowHandle) {
			return new DataControllerValuesContainer(rowHandle, treeBuilder.GetRowVisibleIndexByHandleCore(rowHandle.Value), treeBuilder.GetRowLevelByControllerRow(rowHandle.Value));
		}
		protected readonly DataViewBase viewBase;
		public DataIteratorBase(DataViewBase viewBase) {
			this.viewBase = viewBase;
		}
		protected internal virtual bool GetHasTop(DataNodeContainer nodeContainer) {
			return nodeContainer.StartScrollIndex == 0;
		}
		protected internal bool GetHasBottom(DataNodeContainer nodeContainer) {
			int lastVisibleIndex = nodeContainer.StartScrollIndex + nodeContainer.Items.Count - 1;
			return lastVisibleIndex + 1 == nodeContainer.TotalVisibleCount || GetHasBottomCore(nodeContainer, lastVisibleIndex);
		}
		protected internal virtual bool GetHasBottomCore(DataNodeContainer nodeContainer, int lastVisibleIndex) {
			return false;
		}
		protected internal abstract RowNode GetRowNodeForCurrentLevel(DataNodeContainer nodeContainer, int index, int startVisibleIndex, ref bool shouldBreak);
		protected internal abstract RowNode GetSummaryNodeForCurrentNode(DataNodeContainer nodeContainer, RowHandle rowHandle, int index);
		protected internal virtual int GetRowParentIndex(DataNodeContainer nodeContainer, int visibleIndex, int level) {
			return viewBase.GetRowParentIndex(visibleIndex, level);
		}
		protected internal virtual bool IsGroupRowsContainer(DataNodeContainer nodeContainer) {
			return false;
		}
		internal DataRowNode GetRowNode(DataTreeBuilder treeBuilder, int startVisibleIndex, DataControllerValuesContainer controllerValues) {
			DataRowNode rowNode = treeBuilder.GetRowNode(values => new DataRowNode(treeBuilder, values), controllerValues);
			rowNode.UpdateDetailInfo(startVisibleIndex);
			return rowNode;
		}
	}
}
