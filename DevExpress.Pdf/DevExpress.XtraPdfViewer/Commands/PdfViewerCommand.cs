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
using DevExpress.Utils.Localization;
using DevExpress.XtraBars;
using DevExpress.XtraPdfViewer.Native;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Commands {
	public abstract class PdfViewerCommand : ControlCommand<PdfViewer, PdfViewerCommandId, XtraPdfViewerStringId> {
		protected PdfDocumentViewer Viewer {
			get {
				PdfViewer pdfViewer = Control;
				return pdfViewer == null ? null : pdfViewer.Viewer;
			}
		}
		protected override XtraLocalizer<XtraPdfViewerStringId> Localizer { get { return XtraPdfViewerLocalizer.Active; } }
		protected override string ImageResourcePrefix { get { return "DevExpress.XtraPdfViewer.Images.Bars"; } }
		protected virtual BarShortcut ItemShortcut { get { return BarShortcut.Empty; } }
		protected PdfViewerCommand(PdfViewer control) : base(control) {
		}
		public virtual BarItem CreateContextMenuBarItem(BarManager barManager) {
			BarButtonItem item = new BarButtonItem(barManager, MenuCaption + GetCaptionPostfix());
			ICommandUIState state = new DefaultCommandUIState();
			UpdateUIState(state);
			item.Enabled = state.Enabled;
			InitializeContextMenuBarItem(item);
			return item;
		}
		public override void ForceExecute(ICommandUIState state) {
			ExecuteInternal();
			Control.RaiseUpdateUI();
		}
		protected virtual string GetCaptionPostfix() {
			return "";
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		protected abstract void ExecuteInternal();
		internal void InitializeContextMenuBarItem(BarItem item) {
			item.Glyph = Image;
			item.Tag = this;
			item.ItemClick += (sender, e) => {
				PdfViewerCommand command = e.Item.Tag as PdfViewerCommand;
				if (command != null)
					command.Execute();
			};
			if (ItemShortcut.IsExist)
				item.ShortcutKeyDisplayString = ItemShortcut.ToString();
		}
	}
}
