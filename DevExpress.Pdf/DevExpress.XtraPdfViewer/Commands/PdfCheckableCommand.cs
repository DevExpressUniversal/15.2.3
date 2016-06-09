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

using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Commands {
	public abstract class PdfCheckableCommand : PdfViewerCommand {		
		public override PdfViewerCommandId Id { get { return PdfViewerCommandId.None; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return (XtraPdfViewerStringId)(-1); } }
		public override XtraPdfViewerStringId DescriptionStringId { get { return (XtraPdfViewerStringId)(-1); } }
		protected virtual bool IsChecked { get { return false; } }
		protected PdfCheckableCommand(PdfViewer control) : base(control) { 
		}
		public override BarItem CreateContextMenuBarItem(BarManager barManager) {
			BarCheckItem item = new BarCheckItem(barManager);
			item.Caption = MenuCaption + GetCaptionPostfix();
			ICommandUIState state = new DefaultCommandUIState();
			UpdateUIState(state);
			item.Enabled = state.Enabled;
			item.Checked = state.Checked;
			InitializeContextMenuBarItem(item);
			return item;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			if (Viewer == null) {
				state.Enabled = false;
				state.Checked = false;  
			}
			else {
				state.Enabled = true;
				state.Checked = IsChecked;
			}
		}
		protected override void ExecuteInternal() {
		}
	}
}
