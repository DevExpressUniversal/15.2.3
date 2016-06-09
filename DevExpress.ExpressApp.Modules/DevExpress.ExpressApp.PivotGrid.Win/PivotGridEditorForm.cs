#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Design.Frames;
using DevExpress.XtraPivotGrid.Frames;
namespace DevExpress.ExpressApp.PivotGrid.Win {
	public class XafPivotGridDesignerForm : BaseDesignerForm {
		public const string XafPivotDesignerSettings = "Software\\Developer Express\\Xaf\\Designer\\PivotGrid\\";
		public XafPivotGridDesignerForm(UserLookAndFeel parentLookAndFeel)
			: base(parentLookAndFeel) {
			Text = "PivotGrid Designer";
			MinimumSize = new Size(690, 400);
			ClientSize = new Size(750, 400);
		}
		protected override string GetDefaultProductInfo() {
			return string.Empty;
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new XafPivotGridDesigner();
		}
		protected override Type ResolveType(string type) {
			Type t = typeof(XafPivotGridDesignerForm).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
		protected override string RegistryStorePath { get { return XafPivotDesignerSettings; } }
	}
	public class XafPivotFieldDesigner : FieldDesigner {
		protected override object CreateNewColumn(string fieldName, int index) {
			PivotGridField newField = base.CreateNewColumn(fieldName, index) as PivotGridField;
			if(string.IsNullOrEmpty(newField.Name)) {
				newField.Name = string.Format("Field_{0}_{1}", index, fieldName);
			}
			return newField;
		}
	}
	public class XafPivotGridDesigner : BaseDesigner {
		static ImageCollection largeImages;
		static ImageCollection smallImages;
		static ImageCollection groupImages;
		static XafPivotGridDesigner() {
			largeImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Design.Images.icons32x32.png", typeof(PivotGridControl).Assembly, new Size(32, 32));
			smallImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Design.Images.icons16x16.png", typeof(PivotGridControl).Assembly, new Size(16, 16));
			groupImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Design.Images.navBarGroupIcon16x16.png", typeof(PivotGridControl).Assembly, new Size(16, 16));
		}
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group = Groups.Add("Main", "Main settings(Fields, Appearances).", GetDefaultGroupImage(0), true);
			group.Add("Fields", "Manage fields.", typeof(XafPivotFieldDesigner), GetDefaultLargeImage(0), GetDefaultSmallImage(0), null);
			group.Add("Groups", "Manage groups.", typeof(GroupsDesigner), GetDefaultLargeImage(0), GetDefaultSmallImage(1), null);
			group.Add("Layout", "Customize the current PivotGrid's layout and preview its data.", typeof(Layouts), GetDefaultLargeImage(1), GetDefaultSmallImage(2), null);
			group = Groups.Add("Appearances", "Adjust the appearance of the current PivotGrid.", GetDefaultGroupImage(1), true);
			group.Add("Appearances", "Manage the appearances for the current PivotGrid.", typeof(AppearancesDesigner), GetDefaultLargeImage(2), GetDefaultSmallImage(3), null);
			group.Add("Format Conditions", "Manage the format conditions for the current PivotGrid.", typeof(FormatConditionFrame), GetDefaultLargeImage(3), GetDefaultSmallImage(4), null);
			group = Groups.Add("Printing", "Printing option management for the current PivotGrid.", GetDefaultGroupImage(2), true);
			group.Add("Print Appearances", "Adjust the print appearances of the current PivotGrid.", typeof(PrintAppearancesDesigner), GetDefaultLargeImage(4), GetDefaultSmallImage(5), null);
			group.Add("Print Settings", "Set up various printing options for the current PivotGrid.", typeof(PivotGridPrinting), GetDefaultLargeImage(5), GetDefaultSmallImage(6), null);
		}
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		protected override object GroupImageList { get { return groupImages; } }
	}
}
