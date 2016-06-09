#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using System;
using DevExpress.Utils;
using DevExpress.Data.Utils;
using System.ComponentModel.Design;
namespace DevExpress.DashboardWin.Native {
	public class SelectedContextService {
		readonly List<IEditorContext> editorContextList = new List<IEditorContext>();
		readonly DashboardDesigner designer;
		IDesignerHost designerHost;
		public DashboardDesigner Designer { get { return designer; } }
		public IServiceProvider DashboardServiceProvider { get { return designer.ServiceProvider; } }
		public Dashboard Dashboard { get { return designer.Dashboard; } }
		public DataSourceInfo DataSourceInfo {
			get {
				DataDashboardItem dashboardItem = DashboardItem;
				if (dashboardItem != null && dashboardItem.DataSource != null)
					return new DataSourceInfo(dashboardItem.DataSource, dashboardItem.DataMember);
				return null;
			}
		}
		DataDashboardItem DashboardItem {
			get {
				DataDashboardItem contextDashboardItem = GetContextObject<DataDashboardItem>();
				if (contextDashboardItem != null)
					return contextDashboardItem;
				return designer.SelectedDashboardItem as DataDashboardItem;
			}
		}
		public SelectedContextService(DashboardDesigner designer, IDesignerHost designerHost) {
			Guard.ArgumentNotNull(designer, "designer");
			this.designer = designer;
			this.designerHost = designerHost;
		}
		public ICollection<Measure> GetUniqueMeasures() {
			return DashboardItem.UniqueMeasures;
		}
		public ICollection<Dimension> GetUniqueDimensions() {
			return DashboardItem.UniqueDimensions;
		}
		public void PushContext(IEditorContext editorContext) {
			editorContextList.Add(editorContext);
		}
		public void PopContext() {
			int lastIndex = editorContextList.Count - 1;
			editorContextList.RemoveAt(lastIndex);
		}
		public IDashboardDataSource GetContextDataSource() {
			IDashboardDataSource dataSource = GetContextObject<IDashboardDataSource>();
			if (dataSource == null && designerHost != null) {
				ISelectionService selectionService = designerHost.GetService<ISelectionService>();
				if (selectionService != null)
					dataSource = selectionService.PrimarySelection as IDashboardDataSource;
			}
			return dataSource;
		}
		TContextObject GetContextObject<TContextObject>() where TContextObject : class {
			if (editorContextList.Count > 0) {
				for (int i = editorContextList.Count - 1; i >= 0; i--) {
					TContextObject contextObject = editorContextList[i].EditorContext as TContextObject;
					if (contextObject != null) {
						return contextObject;
					}
				}
			}
			return null;
		}
	}
	public interface IEditorContext {
		object EditorContext { get; }
	}
}
