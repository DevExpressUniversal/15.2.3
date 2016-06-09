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

namespace DevExpress.XtraCharts.Designer {
	public partial class SeriesPointsGridControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesPointsGridControl));
			this.gcPoints = new DevExpress.XtraGrid.GridControl();
			this.pointsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
			((System.ComponentModel.ISupportInitialize)(this.gcPoints)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pointsGridView)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.gcPoints, "gcPoints");
			this.gcPoints.MainView = this.pointsGridView;
			this.gcPoints.Name = "gcPoints";
			this.gcPoints.ShowOnlyPredefinedDetails = true;
			this.gcPoints.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.pointsGridView});
			this.pointsGridView.ActiveFilterEnabled = false;
			this.pointsGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pointsGridView.GridControl = this.gcPoints;
			this.pointsGridView.Name = "pointsGridView";
			this.pointsGridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
			this.pointsGridView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
			this.pointsGridView.OptionsBehavior.AutoPopulateColumns = false;
			this.pointsGridView.OptionsCustomization.AllowColumnMoving = false;
			this.pointsGridView.OptionsCustomization.AllowFilter = false;
			this.pointsGridView.OptionsCustomization.AllowGroup = false;
			this.pointsGridView.OptionsCustomization.AllowSort = false;
			this.pointsGridView.OptionsFilter.AllowFilterEditor = false;
			this.pointsGridView.OptionsMenu.EnableColumnMenu = false;
			this.pointsGridView.OptionsMenu.EnableFooterMenu = false;
			this.pointsGridView.OptionsMenu.EnableGroupPanelMenu = false;
			this.pointsGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
			this.pointsGridView.OptionsView.ShowGroupPanel = false;
			this.pointsGridView.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.GridViewInitNewRow);
			this.pointsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.GridViewCellValueChanged);
			this.pointsGridView.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.GridViewRowUpdated);
			this.pointsGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.GridViewCustomUnboundColumnData);
			this.pointsGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GridViewKeyDown);
			this.pointsGridView.RowCountChanged += new System.EventHandler(this.GridViewRowCountChanged);
			this.Controls.Add(this.gcPoints);
			this.Name = "SeriesPointsGridControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.gcPoints)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pointsGridView)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraGrid.GridControl gcPoints;
		private XtraGrid.Views.Grid.GridView pointsGridView;
	}
}
