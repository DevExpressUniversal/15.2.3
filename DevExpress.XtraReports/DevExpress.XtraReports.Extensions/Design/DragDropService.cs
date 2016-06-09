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
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design
{
	public interface IDragDropService {
		IDragHandler GetDragHandler(IDataObject data);
	}
	public class DragDropService : IDragDropService {
		IDesignerHost designerHost;
		public DragDropService(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		protected internal IDesignerHost DesignerHost { get { return designerHost; } }
		public IDragHandler GetDragHandler(IDataObject data) {
			if (data != null) {
				try {
					if (data.GetDataPresent(typeof(DragDataObject)))
						return CreateControlDragHandler();
				}
				catch {
				}
				try {
					if (data.GetDataPresent(FieldDragHandler.DragType)) {
						IFieldListDragDropService fieldListDragDropService = designerHost.GetService(typeof(IFieldListDragDropService)) as IFieldListDragDropService;
						return fieldListDragDropService != null ? fieldListDragDropService.GetDragHandler() : null;
					}
				}
				catch {
				}
				try {
					if(data.GetDataPresent(ReportExplorerDragHandler.DragType)) {
						IReportExplorerDragDropService reportExplorerDragDropService = designerHost.GetService(typeof(IReportExplorerDragDropService)) as IReportExplorerDragDropService;
						return reportExplorerDragDropService != null ? reportExplorerDragDropService.GetDragHandler() : null;
					}
				}
				catch {
				}
				try {
					if (data.GetDataPresent("CF_DSREF"))
						return new DragHandlerBase(designerHost);
				}
				catch {
				}
				try {
					if (DragHandlerBase.GetToolboxItem(data, designerHost) != null)
						return CreateToolboxItemDragHandler();
				}
				catch {
				}
			}
			return new DragHandlerBase(designerHost);
		}
		protected virtual ControlDragHandler CreateControlDragHandler() {
			return new ControlDragHandler(designerHost);
		}
		protected virtual TbxItemDragHandler CreateToolboxItemDragHandler() {
			return new TbxItemDragHandler(designerHost);
		}
	}
	public interface IFieldListDragDropService {
		IDragHandler GetDragHandler();
	}
	public class FieldListDragDropService : IFieldListDragDropService {
		IDesignerHost designerHost;
		public FieldListDragDropService(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		public IDragHandler GetDragHandler() {
			return new FieldDragHandler(designerHost);
		}
	}
	public interface IReportExplorerDragDropService {
		IDragHandler GetDragHandler();
	}
	public class ReportExplorerDragDropService : IReportExplorerDragDropService {
		IDesignerHost designerHost;
		public ReportExplorerDragDropService(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		public IDragHandler GetDragHandler() {
			return new ReportExplorerDragHandler(designerHost);
		}
	}
}
