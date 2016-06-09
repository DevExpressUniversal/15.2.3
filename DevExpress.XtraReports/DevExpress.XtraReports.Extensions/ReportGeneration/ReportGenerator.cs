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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.XtraReports.ReportGeneration;
using DevExpress.Utils;
using DevExpress.XtraExport;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.ReportGeneration {
	[Designer("DevExpress.XtraReports.Design.ReportGenerator.ReportGeneratorDesigner," + AssemblyInfo.SRAssemblyReportsDesign, typeof(System.ComponentModel.Design.IDesigner))]
	[Description("Generate XtraReport from GridView")]
	[ToolboxTabName(AssemblyInfo.DXTabComponents)]
	[ToolboxBitmap(typeof(LocalResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "ReportGenerator.bmp")]
	[DXToolboxItem(DXToolboxItemKind.Free)]
	public class ReportGenerator : Component {
		public ReportGenerator() {
		}
		public ReportGenerator(IContainer container) {
			container.Add(this);
		}
		public static XtraReport GenerateReport(GridView view) {
			var report = new XtraReport();
			ReportGenerationExtensions<ColumnImplementer, DataRowImplementer>.Generate(report, new GridViewFactoryImplementer(view));
			return report;
		}
		public static XtraReport GenerateReport(GridView view, ReportGenerationOptions options) {
			var report = new XtraReport();
			ReportGenerationExtensions<ColumnImplementer, DataRowImplementer>.Generate(report, new GridViewFactoryImplementer(view), options);
			return report;
		}
		public Form ContainerForm {
			get {
				if(Container == null) return null;
				foreach(Component temp in Container.Components) {
					Form tempForm = temp as Form;
					if(tempForm != null) return tempForm;
				}
				return null;
			}
		}
	}
}
