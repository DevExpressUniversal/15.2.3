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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.Data;
namespace DevExpress.XtraReports.Native.Printing {
	public class DetailWriter : DetailWriterBase {
		int rowIndex;
		public override int RowIndex {
			get { return rowIndex; }
		}
		public DetailWriter(XtraReportBase report, DocumentBand root)
			: base(report, root) {
		}
		protected override void MoveNextRow() {
			rowIndex++;
			fReport.RepeatIndexOnEmptyDataSource = rowIndex;
		}
		protected override bool EndOfDataCore() {
			return rowIndex >= fReport.ReportPrintOptions.DetailCountOnEmptyDataSource - 1;
		}
		protected override int GetRowCount() {
			return fReport != null ? fReport.ReportPrintOptions.DetailCountOnEmptyDataSource : 0;
		}
		DocumentBand detailContainer;
		protected override DocumentBand GetContainer(int index) {
			if(detailContainer == null) {
				detailContainer = new DocumentBandContainer();
				detailContainer.BandManager = this.root.BandManager;
				root.Bands.Insert(detailContainer, index);
			}
			return detailContainer;
		}
	}
	public class DetailWriterWithDS : DetailWriterBase {
		DocumentBand detailContainer;
		protected override DocumentBand GetContainer(int index) {
			if(detailContainer == null) {
				detailContainer = new DocumentBandContainer();
				detailContainer.BandManager = this.root.BandManager;
				root.Bands.Insert(detailContainer, index);
			}
			return detailContainer;
		}
		public DetailWriterWithDS(XtraReportBase report, DocumentBand root)
			: base(report, root) {
		}
		protected override void MoveNextRow() {
			fReport.MoveNextRow();
		}
		protected override bool EndOfDataCore() {
			return fReport.EndOfData();
		}
		protected override int GetRowCount() {
			return fReport.DisplayableRowCount;
		}
	}
	public class DetailWriterWithGroups : DetailWriterWithDS {
		public DetailWriterWithGroups(XtraReportBase report, DocumentBand root)
			: base(report, root) {
		}
		protected override DocumentBand GetContainer(int index) {
			return this.root;
		}
		protected override int FindNextGroupIndex() {
			if(fReport.EndOfData())
				return Groups.Count - 1;
			IList<GroupRowInfo> groups = fReport.DataController.GetDataGroups(fReport.DataBrowser.Position + 1);
			IList<int> levels = Groups.GetDataGroupLevels();
			int index = -1;
			for(int i = 0; i < Groups.Count; i++) {
				if(groups.Any<GroupRowInfo>(group => group.Level == levels[i] && group.ChildControllerRow == fReport.DataBrowser.Position + 1))
					index = i;
			}
			return index;
		}
	}
}
