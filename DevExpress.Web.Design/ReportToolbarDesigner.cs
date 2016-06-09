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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Web.Design;
using DevExpress.Web.Design.Reports;
using DevExpress.Web.Design.Reports.Toolbar;
namespace DevExpress.XtraReports.Web.Design {
	public class ReportToolbarDesigner : ASPxWebControlDesigner {
		ReportToolbar reportToolbar;
		public override bool AllowResize {
			get { return true; }
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			reportToolbar = (ReportToolbar)component;
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			XtraReportsAssembliesInsurer.EnsureControlReferences(DesignerHost);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Items", "Item");
		}
		public override void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			base.OnComponentChanged(sender, e);
			Tag.SetDirty(true);
			Tag.RemoveAttribute("BorderColor");
			Tag.RemoveAttribute("BorderWidth");
			Tag.RemoveAttribute("BorderStyle");
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ReportToolbarActionList(this);
		}
		public override void ShowAbout() {
			XtraReportAboutDialogHelper.ShowAbout(Component.Site);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new ReportToolbarCommonFormDesigner(reportToolbar, DesignerHost)));
		}
	}
}
