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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.InternalAccess;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrintingLinks;
namespace DevExpress.XtraPrinting.Links {
	class LinkCommandHandler {
		protected LinkBase link;
		IList images;
		public LinkCommandHandler(LinkBase link, IList images) {
			this.link = link;
			this.images = images;
		}
		public virtual void HandleCommand(PrintingSystemCommand command, object[] args, PrintControl printControl, ref bool handled) {
			if(command == PrintingSystemCommand.EditPageHF)
				HandleCommandCore(printControl);
		}
		void HandleCommandCore(PrintControl printControl) {
			using(Form form = PageHeaderFooterAccessor.GetEditorForm((PageHeaderFooter)link.PageHeaderFooter, images)) {
				if(form is ISupportLookAndFeel)
					((ISupportLookAndFeel)form).LookAndFeel.ParentLookAndFeel = printControl.LookAndFeel;
				if(form != null && form.ShowDialog(printControl) != DialogResult.Cancel) {
					int index = printControl.SelectedPageIndex;
					link.CreateDocument();
					printControl.SelectedPageIndex = index;
				}
			}
		}
	}
	class PrintableComponentLinkCommandHandler : LinkCommandHandler {
		PrintableComponentLink PrintableComponentLink {
			get { return (PrintableComponentLink)base.link; }
		}
		public PrintableComponentLinkCommandHandler(PrintableComponentLinkBase link, IList images) : base(link, images) { 
		}
		public override void HandleCommand(PrintingSystemCommand command, object[] args, PrintControl printControl, ref bool handled) {
			if(command != PrintingSystemCommand.Customize || PrintableComponentLink.Component == null) {
				base.HandleCommand(command, args, printControl, ref handled);
				return;
			}
			UserControl propertyEditor = PrintableComponentLink.Component.PropertyEditorControl;
			if(propertyEditor == null)
				return;
			Size size = propertyEditor.Size;
			ComponentEditorForm form = null;
			try {
				form = new ComponentEditorForm(PrintableComponentLink);
				form.LookAndFeel.ParentLookAndFeel = printControl.LookAndFeel;
				form.ShowDialog(printControl);
			} finally {
				propertyEditor.Parent = null;
				propertyEditor.Dock = DockStyle.None;
				propertyEditor.Size = size;
				if(form != null)
					form.Dispose();
			}
		}
	}
}
