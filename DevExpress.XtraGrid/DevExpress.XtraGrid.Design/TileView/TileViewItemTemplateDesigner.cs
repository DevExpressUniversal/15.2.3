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

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.XtraGrid.Views.Tile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraGrid.Design.Tile {
	class TileViewItemTemplateDesigner : XtraFrame {
		public TileViewItemTemplateDesigner()
			: base(0) {
		}
		public override void DoInitFrame() {
			InitPreview();
		}
		GridControl sampleGrid;
		TileTemplateDesignerControl control;
		public GridControl SampleGrid { 
			get { return sampleGrid; } 
		}
		public TileView SampleView {
			get {
				if(SampleGrid == null) return null;
				return SampleGrid.MainView as TileView;
			}
		}
		protected TileTemplateDesignerControl ItemControl { 
			get { return this.control; } 
		}
		TileView EditingView { 
			get { return EditingObject as TileView; } 
		}
		IComponentChangeService componentChangeServiceCore;
		protected IComponentChangeService ComponentChangeService { 
			get { return componentChangeServiceCore; } 
		}
		protected virtual void OnViewChanged() {
			if(EditingView != null && ComponentChangeService != null) {
				if(ItemControl != null)
					EditingView.TileTemplate.Assign(ItemControl.GetValue());
				ComponentChangeService.OnComponentChanged(EditingView, null, null, null);
				SyncPreview();
			}
		}
		GridControl CreateSampleGrid() {
			return GridAssign.CreateGridControlAssign(EditingView.GridControl, EditingView);
		}
		void InitPreview() {
			this.componentChangeServiceCore = EditingView.GridControl.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			this.sampleGrid = CreateSampleGrid();
			SampleGrid.Dock = DockStyle.Fill;
			SampleGrid.DataSource = EditingView.DataSource;
			SampleView.Assign(EditingView, false);
			SampleView.OptionsTiles.HorizontalContentAlignment = DevExpress.Utils.HorzAlignment.Center;
			SampleView.OptionsTiles.VerticalContentAlignment = DevExpress.Utils.VertAlignment.Center;
			SampleView.SetTemplateEditing(true);
			SplitContainerControl splitContainer = new SplitContainerControl();
			splitContainer.Dock = DockStyle.Fill;
			splitContainer.Panel2.Controls.Add(SampleGrid);
			splitContainer.SplitterPosition = 430;
			pnlMain.Controls.Add(splitContainer);
			control = new TileTemplateDesignerControl();
			control.Dock = DockStyle.Fill;
			control.OnViewChanged = OnViewChanged;
			control.Assign(EditingView.Site, EditingView.TemplateItem);
			control.PopulateColumns(EditingView.Columns);
			splitContainer.Panel1.Controls.Add(ItemControl);
			InitPreviewTile();
		}
		void InitPreviewTile() {
			(SampleGrid.MainView as TileView).AddDesignItem();
		}
		void SyncPreview() {
			SampleView.BeginUpdate();
			try {
				SampleView.Assign(EditingView, false);
				SampleView.OptionsTiles.HorizontalContentAlignment = DevExpress.Utils.HorzAlignment.Center;
				SampleView.OptionsTiles.VerticalContentAlignment = DevExpress.Utils.VertAlignment.Center;
			}
			finally {
				SampleView.EndUpdate();
			}
		}
	}
}
