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
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.XtraReports.Web {
	public class ReportParametersPanelEditorStyles : StylesBase {
		const string
			CaptionCellName = "CaptionCell",
			CaptionName = "Caption",
			EditorRootName = "EditorRoot";
		public ReportParametersPanelEditorStyles(ISkinOwner owner)
			: base(owner) {
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportParametersPanelEditorStylesCaptionCell")]
#endif
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public EditorCaptionCellStyle CaptionCell {
			get { return (EditorCaptionCellStyle)GetStyle(CaptionCellName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportParametersPanelEditorStylesCaption")]
#endif
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public EditorCaptionStyle Caption {
			get { return (EditorCaptionStyle)GetStyle(CaptionName); }
		}
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public EditorRootStyle EditorRoot {
			get { return (EditorRootStyle)GetStyle(EditorRootName); }
		}
		protected override string GetCssClassNamePrefix() {
			return "dxxr";
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(
				base.GetStateManagedObjects(),
				new IStateManager[] {
					CaptionCell,
					Caption,
					EditorRoot
				});
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(CaptionCellName, () => new EditorCaptionCellStyle()));
			list.Add(new StyleInfo(CaptionName, () => new EditorCaptionStyle()));
			list.Add(new StyleInfo(EditorRootName, () => new EditorRootStyle()));
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			var src = source as ReportParametersPanelEditorStyles;
			if(src == null) {
				return;
			}
			CaptionCell.CopyFrom(src.CaptionCell);
			Caption.CopyFrom(src.Caption);
			EditorRoot.CopyFrom(src.EditorRoot);
		}
	}
}
