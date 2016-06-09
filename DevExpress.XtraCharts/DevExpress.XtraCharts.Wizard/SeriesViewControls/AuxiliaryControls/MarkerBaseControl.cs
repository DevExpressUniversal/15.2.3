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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class MarkerBaseControl : ChartUserControl {
		static ImageCollection kindMarkerList = CreateKindMarkerList();
		static ImageCollection CreateKindMarkerList() {
			return ImageHelper.CreateImageCollectionFromResources(
				ImageResourcesUtils.WizardImagePath + "markers.png", Assembly.GetAssembly(typeof(Chart)), new Size(13, 13));
		}
		MarkerBase marker;
		public MarkerBaseControl() {
			InitializeComponent();
		}
		public void Initialize(MarkerBase marker) {
			this.marker = marker;
			SimpleMarker simpleMarker = marker as SimpleMarker;
			if(simpleMarker != null)
				spnSize.EditValue = simpleMarker.Size;
			else {
				pnlSize.Visible = false;
				sepKind.Visible = false;
			}
			cbKind.Properties.SmallImages = kindMarkerList;
			cbKind.SelectedIndex = (int)marker.Kind;
			borderControl.Initialize(marker);
			polygonFillStyleControl.Initialize(marker.FillStyle);
			UpdateControls();
		}
		void UpdateControls() {
			spnStarPoints.EditValue = marker.StarPointCount;
			spnStarPoints.Enabled = marker.Kind == MarkerKind.Star;
		}
		void spnSize_EditValueChanged(object sender, EventArgs e) {
			SimpleMarker simpleMarker = marker as SimpleMarker;
			if(simpleMarker != null)
				simpleMarker.Size = Convert.ToInt32(spnSize.EditValue);
		}
		void cbKind_EditValueChanged(object sender, EventArgs e) {
			marker.Kind = (MarkerKind)cbKind.SelectedIndex;
			UpdateControls();
		}
		void spnStarPoints_EditValueChanged(object sender, EventArgs e) {
			marker.StarPointCount = Convert.ToInt32(spnStarPoints.EditValue);
		}
	}
}
