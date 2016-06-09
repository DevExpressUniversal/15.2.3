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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraPivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.Design.Frames {
	public class PivotGridEditorForm : BaseDesignerForm {
		public const string XtraReportsSettings = "Software\\Developer Express\\XtraReports\\";
		public PivotGridEditorForm(DevExpress.LookAndFeel.UserLookAndFeel parentLookAndFeel) : base(parentLookAndFeel) {
			MinimumSize = new Size(780, 480);
			ClientSize = new Size(1175, 715);
		}
		protected override string GetDefaultProductInfo() {
			return string.Empty;
		}
		protected override string AssemblyName {
			get {
				return string.Empty;
			}
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new PivotGridDesigner();
		}
		protected override string RegistryStorePath { get { return XtraReportsSettings; } }
		protected override Type ResolveType(string type) {
			Type t = typeof(PivotGridEditorForm).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
	}
	public class PivotGridDesigner : BaseDesigner {
		static DevExpress.Utils.ImageCollection largeImages = null;
		static DevExpress.Utils.ImageCollection smallImages = null;
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		static PivotGridDesigner() {
			largeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Design.Images.icons32x32.png", typeof(PivotGridControl).Assembly, new Size(32, 32));
			smallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Design.Images.icons16x16.png", typeof(PivotGridControl).Assembly, new Size(16, 16));
		}
		public PivotGridDesigner() {
		}
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group = Groups.Add(DesignSR.PivotGridForm_GroupMain_Caption, DesignSR.PivotGridForm_GroupMain_Description, null, true);
			group.Add(DesignSR.PivotGridForm_ItemFields_Caption, DesignSR.PivotGridForm_ItemFields_Description, typeof(PivotGridFieldDesigner), GetDefaultLargeImage(0), GetDefaultSmallImage(0), null);
			group.Add(DesignSR.PivotGridForm_ItemLayout_Caption, DesignSR.PivotGridForm_ItemLayout_Description, typeof(PivotGridLayouts), GetDefaultLargeImage(1), GetDefaultSmallImage(1), null);
			group = Groups.Add(DesignSR.PivotGridForm_GroupPrinting_Caption, DesignSR.PivotGridForm_GroupPrinting_Description, null, true);
			group.Add(DesignSR.PivotGridForm_ItemAppearances_Caption, DesignSR.PivotGridForm_ItemAppearances_Description, typeof(PivotGridPrintAppearancesDesigner), GetDefaultLargeImage(4), GetDefaultSmallImage(4), null);
			group.Add(DesignSR.PivotGridForm_ItemSettings_Caption, DesignSR.PivotGridForm_ItemSettings_Description, typeof(XRPivotGridPrinting), GetDefaultLargeImage(5), GetDefaultSmallImage(5), null); 
		}
	}
}
