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
using DevExpress.XtraPdfViewer.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraPdfViewer.Controls;
using DevExpress.Pdf.Drawing;
namespace DevExpress.XtraPdfViewer.Commands {
	public class PdfOutlinesExpandCollapseTopLevelCommand : PdfViewerCommand {
		public override PdfViewerCommandId Id { get { return PdfViewerCommandId.OutlinesExpandCollapseTopLevel; } }
		public override XtraPdfViewerStringId DescriptionStringId { get { return (XtraPdfViewerStringId)(-1); } }
		public override XtraPdfViewerStringId MenuCaptionStringId {
			get {
				PdfViewer viewer = Control;
				PdfOutlineViewerControl outlineViewerControl = viewer == null ? null : viewer.OutlineViewerControl;
				return outlineViewerControl != null && outlineViewerControl.AreTopLevelTreeNodesExpanded
										 ? XtraPdfViewerStringId.CommandOutlinesCollapseTopLevelCaption : XtraPdfViewerStringId.CommandOutlinesExpandTopLevelCaption;
			}
		}
		public PdfOutlinesExpandCollapseTopLevelCommand(PdfViewer control)
			: base(control) {
		}
		public override void UpdateUIState(ICommandUIState state) {
			base.UpdateUIState(state);
			PdfViewer viewer = Control;
			PdfDocumentState documentState = viewer == null ? null : viewer.DocumentState;
			state.Enabled = documentState != null && HasChildOutlineViewerNodes(documentState.OutlineViewerNodes);
		}
		protected override void ExecuteInternal() {
			PdfViewer viewer = Control;
			PdfOutlineViewerControl outlineViewerControl = viewer == null ? null : viewer.OutlineViewerControl;
			if (outlineViewerControl != null)
				outlineViewerControl.ExpandCollapseTopLevelOutlineViewerNodes();
		}
		bool HasChildOutlineViewerNodes(IList<PdfOutlineViewerNode> nodes) {
			bool hasChildNodes = false;
			foreach (PdfOutlineViewerNode node in nodes) {
				if (node.HasChildNodes) {
					hasChildNodes = true;
					break;
				}
			}
			return hasChildNodes;
		}
	}
}
