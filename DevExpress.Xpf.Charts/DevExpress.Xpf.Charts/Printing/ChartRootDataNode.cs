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
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.Xpf.Charts.Native {
	public class ChartRootDataNode : IRootDataNode {
		readonly WeakReference chartReference;
		readonly ChartVisualDataNode visualChild;
		internal ChartControl Chart { get { return chartReference.IsAlive ? chartReference.Target as ChartControl : null; } }
		public ChartRootDataNode(ChartControl chart, Size usablePageSize, Size pageHeaderSize, Size pageFooterSize) {
			this.chartReference = new WeakReference(chart);
			Size availableSize = GetAvailableSize(usablePageSize, pageHeaderSize, pageFooterSize);
			this.visualChild = new ChartVisualDataNode(this, availableSize);
		}
		#region IRootDataNode implementation
		int IRootDataNode.GetTotalDetailCount() {
			return 1;
		}
		#endregion
		#region IDataNode implementation
		int IDataNode.Index { get { return -1; } }
		bool IDataNode.IsDetailContainer { get { return true; } }
		bool IDataNode.PageBreakAfter { get { return false; } }
		bool IDataNode.PageBreakBefore { get { return false; } }
		IDataNode IDataNode.Parent { get { return null; } }
		bool IDataNode.CanGetChild(int index) {
			return IsValidChildIndex(index);
		}
		IDataNode IDataNode.GetChild(int index) {
			return IsValidChildIndex(index) ?  visualChild : null;
		}
		#endregion
		bool IsValidChildIndex(int childIndex) {
			return childIndex == 0;
		}
		Size GetAvailableSize(Size usablePageSize, Size pageHeaderSize, Size pageFooterSize) {
			double height = usablePageSize.Height - pageHeaderSize.Height - pageFooterSize.Height;
			return new Size(usablePageSize.Width, height);
		}
	}
}
