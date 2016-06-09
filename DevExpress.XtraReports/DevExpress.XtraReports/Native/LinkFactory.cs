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
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Native {
	static class LinkFactory {
		public static LinkBase GetLinkOf(Control ctl) {
			if(ctl == null)
				return null;
			IntPtr ignore = ctl.Handle;
			return CreateLink(ctl.GetType());
		}
		static LinkBase CreateLink(Type ctlType) {
			if(typeof(IPrintable).IsAssignableFrom(ctlType))
				return new PrintableComponentLinkBase();
			if(typeof(DataGrid).IsAssignableFrom(ctlType)) {
				DataGridLinkBase dataGridLink = new DataGridLinkBase();
				dataGridLink.UseDataGridView = true;
				return dataGridLink;
			}
			if(typeof(TreeView).IsAssignableFrom(ctlType))
				return new TreeViewLinkBase();
			if(typeof(ListView).IsAssignableFrom(ctlType))
				return new ListViewLinkBase();
			if(typeof(RichTextBox).IsAssignableFrom(ctlType)) {
				RichTextBoxLinkBase richTextBoxLink = new RichTextBoxLinkBase();
				richTextBoxLink.PrintFormat = RichTextPrintFormat.RichTextBoxSize;
				richTextBoxLink.InfiniteFormatHeight = true;
				return richTextBoxLink;
			}
			return null;
		}
	}
}
