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
using System.Windows.Media;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.Xpf.PivotGrid.Printing {
	public class PivotPrintHelper {
		PivotGridControl pivotGrid;
		public PivotPrintHelper(PivotGridControl pivotGrid) {
			this.pivotGrid = pivotGrid;
		}
		public PivotGridControl PivotGrid {
			get { return pivotGrid; }
		}
		protected internal virtual IRootDataNode CreateRootNode(PrintSizeArgs sizes) {
			return CreatePrintRoot(sizes);
		}
		public virtual PivotPrintRoot CreatePrintRoot(PrintSizeArgs sizes) {
			PrintLayoutMode mode = PivotGrid.PrintLayoutMode;
			if(mode == PrintLayoutMode.Auto)
				mode = PivotGrid.DesiredPrintLayoutMode;
			if(mode == PrintLayoutMode.SinglePageLayout)
				return new SinglePagePrintRoot(PivotGrid, sizes);
			else
				return new MultiplePagePrintRoot(PivotGrid, sizes);
		}
	}
	public enum PrintLayoutMode {
		SinglePageLayout = 0,
		MultiplePagesLayout = 1,
		Auto = 2
	}
	public struct PrintSizeArgs {
		 readonly Size usablePageSize, reportHeaderSize, reportFooterSize, pageHeaderSize, pageFooterSize;
		public PrintSizeArgs(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			this.usablePageSize = usablePageSize;
			Size nullSize = new Size();
			this.reportHeaderSize = reportHeaderSize.Height < 0 ? nullSize : reportHeaderSize;
			this.reportFooterSize = reportFooterSize.Height < 0 ? nullSize : reportFooterSize;
			this.pageHeaderSize = pageHeaderSize.Height < 0 ? nullSize : pageHeaderSize;
			this.pageFooterSize = pageFooterSize.Height < 0 ? nullSize : pageFooterSize;
		}
		public Size UsablePageSize {get {return usablePageSize;}}
		public Size ReportHeaderSize { get { return reportHeaderSize; } }
		public Size ReportFooterSize { get { return reportFooterSize; } }
		public Size PageHeaderSize { get { return pageHeaderSize; } }
		public Size PageFooterSize { get { return pageFooterSize; } } 
	}
	public class PivotPrintingException : Exception {
		public const string PageTooSmall = "Page size is too small to contain fixed panes and data cells. Please increase the page size or set the PivotGridControl.PrintLayoutMode property to SinglePageLayout.";
		public PivotPrintingException(string message) : base (message) {
		}
	}
	interface IVirtualChildCollection {
		DependencyObject GetChild(int childIndex);
		int GetChildrenCount();
	}
	class PivotVisualTreeWalker : IVisualTreeWalker {
		#region IVisualTreeWalker Members
		public int GetChildrenCount(DependencyObject reference) {
			IVirtualChildCollection helper = reference as IVirtualChildCollection;
			if(helper == null)
				return VisualTreeHelper.GetChildrenCount(reference);
			else
				return helper.GetChildrenCount();
		}
		public DependencyObject GetChild(DependencyObject reference, int childIndex) {
			IVirtualChildCollection helper = reference as IVirtualChildCollection;
			if(helper == null)
				return VisualTreeHelper.GetChild(reference, childIndex);
			else
				return helper.GetChild(childIndex);
		}
		#endregion
	}
}
