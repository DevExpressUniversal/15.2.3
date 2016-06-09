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
	public class DocumentViewerSplitterStyles : SplitterStyles {
		const string SidePaneWidthName = "SidePaneWidth",
			SidePaneMinWidthName = "SidePaneMinWidth",
			ToolbarPaneHeightName = "ToolbarPaneHeight",
			InternalRibbonToolbarPaneHeightName = "InternalRibbonToolbarPaneHeight",
			ParametersPanelControlButtonWidthName = "ParametersPanelControlButtonWidth";
		internal static readonly Unit
			DefaultSidePaneWidth = Unit.Pixel(195),
			DefaultToolbarPaneHeight = Unit.Pixel(30),
			DefaultInternalRibbonToolbarPaneHeight = Unit.Pixel(96);
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerSplitterStylesPane")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public new DocumentViewerSplitterPaneStyle Pane {
			get { return (DocumentViewerSplitterPaneStyle)GetStyle(PaneStyleName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerSplitterStylesSidePaneWidth")]
#endif
		[DefaultValue(typeof(Unit), "195px")]
		[AutoFormatEnable]
		[NotifyParentProperty(true)]
		public Unit SidePaneWidth {
			get { return GetUnitProperty(SidePaneWidthName, DefaultSidePaneWidth); }
			set { SetUnitProperty(SidePaneWidthName, DefaultSidePaneWidth, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerSplitterStylesSidePaneMinWidth")]
#endif
		[DefaultValue(typeof(Unit), "195px")]
		[AutoFormatEnable]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		public Unit SidePaneMinWidth {
			get { return GetUnitProperty(SidePaneMinWidthName, DefaultSidePaneWidth); }
			set { SetUnitProperty(SidePaneMinWidthName, DefaultSidePaneWidth, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerSplitterStylesToolbarPaneHeight")]
#endif
		[DefaultValue(typeof(Unit), "30px")]
		[AutoFormatEnable]
		[NotifyParentProperty(true)]
		public Unit ToolbarPaneHeight {
			get { return GetUnitProperty(ToolbarPaneHeightName, DefaultToolbarPaneHeight); }
			set { SetUnitProperty(ToolbarPaneHeightName, DefaultToolbarPaneHeight, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerSplitterStylesInternalRibbonToolbarPaneHeight")]
#endif
		[DefaultValue(typeof(Unit), "96px")]
		[AutoFormatEnable]
		[NotifyParentProperty(true)]
		public Unit InternalRibbonToolbarPaneHeight {
			get { return GetUnitProperty(InternalRibbonToolbarPaneHeightName, DefaultInternalRibbonToolbarPaneHeight); }
			set { SetUnitProperty(InternalRibbonToolbarPaneHeightName, DefaultInternalRibbonToolbarPaneHeight, value); }
		}
		public DocumentViewerSplitterStyles(ISkinOwner owner)
			: base(owner) {
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			var src = source as DocumentViewerSplitterStyles;
			if(src == null) {
				return;
			}
			SidePaneWidth = src.SidePaneWidth;
			SidePaneMinWidth = src.SidePaneMinWidth;
			ToolbarPaneHeight = src.ToolbarPaneHeight;
			InternalRibbonToolbarPaneHeight = src.InternalRibbonToolbarPaneHeight;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			ReplaceStyle(list, PaneStyleName, () => new DocumentViewerSplitterPaneStyle());
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Pane });
		}
		static void ReplaceStyle(List<StyleInfo> list, string name, CreateStyleHandler createStyle) {
			list.RemoveAll(x => x.Name == name);
			list.Add(new StyleInfo(name, createStyle));
		}
	}
}
