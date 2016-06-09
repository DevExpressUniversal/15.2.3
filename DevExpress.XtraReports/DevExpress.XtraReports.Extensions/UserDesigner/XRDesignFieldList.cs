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
using System.ComponentModel.Design;
using System.Collections;
using System.Diagnostics;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Localization;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraReports.Native;
using DevExpress.Data.Browsing;
using System.Collections.Generic;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.XtraReports.UserDesigner {
	[
	ToolboxItem(false),
	]
	public class XRDesignFieldList : FieldListTreeView, IDesignControl, DevExpress.Utils.IToolTipControlClient {
		class FieldListHelper : XRDesignTreeListItemHelper {
			public FieldListHelper(XRDesignFieldList control)
				: base(ReportToolItemKindHelper.FieldList, control) {
			}
			protected XRDesignFieldList FieldList {
				get { return (XRDesignFieldList)control; }
			}
			protected override void SubscribeDesignPanelEvents() {
				base.SubscribeDesignPanelEvents();
				XRDesignPanel.DesignerHostLoaded += new DesignerLoadedEventHandler(OnDesignerHostLoaded);
			}
			protected override void UnSubscribeDesignPanelEvents() {
				base.UnSubscribeDesignPanelEvents();
				XRDesignPanel.DesignerHostLoaded -= new DesignerLoadedEventHandler(OnDesignerHostLoaded);
			}
			void OnDesignerHostLoaded(object sender, DesignerLoadedEventArgs e) {
				FieldList.SynchDataContext(XRDesignPanel);
			}
		}
		XRDesignTreeListItemHelper treeListItemHelper;
		SortOrder sortOrder = DesignTimeDataContextService.DefaultSortOrder;
		ShowComplexProperties showComplexProperties = ShowComplexProperties.First;
		[DefaultValue(ShowComplexProperties.First)]
		public ShowComplexProperties ShowComplexProperties {
			get { return showComplexProperties; }
			set { 
				showComplexProperties = value;
				SynchDataContext(XRDesignPanel);
			}
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignFieldListXRDesignPanel"),
#endif
		DefaultValue(null),
		]
		public XRDesignPanel XRDesignPanel {
			get { return treeListItemHelper.XRDesignPanel; }
			set { treeListItemHelper.XRDesignPanel = value; }
		}
		[DefaultValue(DesignTimeDataContextService.DefaultSortOrder)]
		public SortOrder SortOrder {
			get { return sortOrder; }
			set {
				sortOrder = value;
				SynchDataContext(XRDesignPanel);
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the ShowComplexProperties property instead."),
		]
		public bool ShowComplexNodesFirst {
			get {
				return ShowComplexProperties == ShowComplexProperties.First;
			}
			set {
				ShowComplexProperties = value ? ShowComplexProperties.First : ShowComplexProperties.Default;
			}
		}
		protected DevExpress.XtraReports.UI.XtraReport XtraReport {
			get { return treeListItemHelper.XtraReport; }
		}
		public XRDesignFieldList()
			: base(null) {
			this.treeListItemHelper = new FieldListHelper(this);
		}
		protected override void Dispose(bool disposing) {
			if(treeListItemHelper != null) {
				treeListItemHelper.DoDispose(disposing);
				treeListItemHelper = null;
			}
			base.Dispose(disposing);
		}
		void SynchDataContext(IServiceProvider servProvider) {
			if(servProvider != null) {
				DesignTimeDataContextService serv = servProvider.GetService(typeof(IDataContextService)) as DesignTimeDataContextService;
				if(serv != null) {
					serv.ShowComplexProperties = ShowComplexProperties;
					serv.PropertiesSortOrder = SortOrder;
				}
			}
		}
		bool DevExpress.Utils.IToolTipControlClient.ShowToolTips {
			get { return !this.ContextMenuVisible && this.ShowNodeToolTips && XtraReport != null && XtraReport.DesignerOptions.ShowDesignerHints; }
		}
	}
}
