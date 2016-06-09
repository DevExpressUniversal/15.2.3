#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.XtraReports.Web.DocumentViewer {
	public class DocumentViewerReportToolbarStyles : ReportToolbarStyles {
		const string
			ToolbarComboBoxExtraWidthName = "ToolbarComboBoxExtraWidth",
			ToolbarMenuStyleName = "ToolbarMenuStyle",
			AlignmentName = "Alignment";
		const ReportToolbarAlignment DefaultAlignment = ReportToolbarAlignment.Default;
		public DocumentViewerReportToolbarStyles(ISkinOwner owner)
			: base(owner) {
		}
		#region prpoerties
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerReportToolbarStylesToolbarMenuStyle")]
#endif
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ReportToolbarMenuStyle ToolbarMenuStyle {
			get { return (ReportToolbarMenuStyle)GetStyle(ToolbarMenuStyleName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerReportToolbarStylesToolbarComboBoxExtraWidth")]
#endif
		[AutoFormatDisable]
		[Category("Layout")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Unit), "")]
		public Unit ToolbarComboBoxExtraWidth {
			get { return GetUnitProperty(ToolbarComboBoxExtraWidthName, Unit.Empty); }
			set { SetUnitProperty(ToolbarComboBoxExtraWidthName, Unit.Empty, value); }
		}
		[AutoFormatDisable]
		[Category("Layout")]
		[NotifyParentProperty(true)]
		[DefaultValue(DefaultAlignment)]
		public ReportToolbarAlignment Alignment {
			get { return (ReportToolbarAlignment)GetEnumProperty(AlignmentName, DefaultAlignment); }
			set { SetEnumProperty(AlignmentName, DefaultAlignment, value); }
		}
		#endregion
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ToolbarMenuStyle });
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ToolbarMenuStyleName, () => new ReportToolbarMenuStyle()));
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			var src = source as DocumentViewerReportToolbarStyles;
			if(src == null) {
				return;
			}
			ToolbarMenuStyle.CopyFrom(src.ToolbarMenuStyle);
			ToolbarComboBoxExtraWidth = src.ToolbarComboBoxExtraWidth;
			Alignment = src.Alignment;
		}
	}
}
