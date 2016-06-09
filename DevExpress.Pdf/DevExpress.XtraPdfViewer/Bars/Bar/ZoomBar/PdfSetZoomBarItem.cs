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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraPdfViewer.Commands;
using DevExpress.XtraPdfViewer.Native;
namespace DevExpress.XtraPdfViewer.Bars {
	public class PdfSetZoomBarItem : ControlCommandBarEditItem<PdfViewer, PdfViewerCommandId, float> { 
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.SetZoom; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithText; } }
		protected override ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<float?> result = new DefaultValueBasedCommandUIState<float?>();
			object editValue = EditValue;
			if (editValue == null)
				result.Value = 0;
			else
				try {
					string strEditValue = editValue.ToString();
					int percentIndex = strEditValue.IndexOf("%");
					if (percentIndex >= 0)
						strEditValue = strEditValue.Substring(0, percentIndex);
					result.Value = Convert.ToSingle(strEditValue);			
				}
				catch {		
					PdfDocumentViewer viewer = Control.Viewer;
					result.Value = viewer == null ? 0 : (viewer.Zoom * 100);
				}
			return result;
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemTextEdit edit = new RepositoryItemTextEdit();
			edit.DisplayFormat.FormatType = FormatType.Numeric;
			edit.DisplayFormat.FormatString = "p1";
			edit.EditFormat.FormatType = FormatType.Numeric;
			edit.EditFormat.FormatString = "p1";
			return edit;
		}
	}
}
