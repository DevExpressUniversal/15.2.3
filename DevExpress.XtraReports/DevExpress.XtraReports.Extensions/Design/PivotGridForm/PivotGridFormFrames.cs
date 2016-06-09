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
using System.Text;
using DevExpress.XtraPivotGrid.Design.Frames;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPivotGrid.Frames;
using DevExpress.XtraPivotGrid;
namespace DevExpress.XtraReports.Design.Frames {
	public class PivotGridFieldDesigner : FieldDesigner {
		protected override string GroupControlColumnsText { get { return DesignSR.PivotGridFrame_Fields_ColumnsText; } }
		protected override string DescriptionText {
			get {
				return  VisibleList ? string.Concat(DesignSR.PivotGridFrame_Fields_DescriptionText1, " ", DesignSR.PivotGridFrame_Fields_DescriptionText2) :
					DesignSR.PivotGridFrame_Fields_DescriptionText1;
			}
		}
	}
	public class PivotGridLayouts : Layouts {
		class XRPivotGridLayoutEditorControl : PivotGridLayoutEditorControl { }
		protected override string DescriptionText { get { return DesignSR.PivotGridFrame_Layouts_DescriptionText; } }
		protected override void SetColumnSelectorText(bool showing) {
			SetColumnSelectorCaption(showing ? DesignSR.PivotGridFrame_Layouts_SelectorCaption1 : DesignSR.PivotGridFrame_Layouts_SelectorCaption2);
		}
		protected override PivotGridControl CreatePivotGridControl() {
			return new XRPivotGridLayoutEditorControl();
		}
		protected override DevExpress.XtraEditors.Frames.DBAdapter CreateDBAdapter() {
			return null;
		}
	}
	public class PivotGridPrintAppearancesDesigner : PrintAppearancesDesigner {
		protected override string DescriptionText { get { return DesignSR.PivotGridFrame_Appearances_DescriptionText; } }
	}
	public class XRPivotGridPrinting : DevExpress.XtraPivotGrid.Frames.PivotGridPrinting {
		protected override void CreateOptionItems() {
			base.CreateOptionItems();
			OptionItems.Remove("OptionsPrint.UsePrintAppearance");
		}
		protected override DevExpress.Utils.ImageCollection CreateImageCollection() {
			return DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Images.PivotGridPrintOptions.png", typeof(DevExpress.XtraPivotGrid.Frames.PivotGridPrinting).Assembly, new System.Drawing.Size(16, 16));
		}
	}
}
