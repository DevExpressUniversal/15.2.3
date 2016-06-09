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
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraPrinting.Links {
	public class LinkPrintTool : PrintTool {
		LinkBase link;
		internal override PreviewFormContainer FormContainer {
			get {
				if(PrintingSystem is PrintingSystem)
					return ((PrintingSystem)PrintingSystem).FormContainer;
				return base.FormContainer;
			}
		}
		public LinkPrintTool(LinkBase link) : base(link.PrintingSystemBase) {
			this.link = link;
		}
		void CreateIfEmpty(bool buildPagesInBackground) {
			if(PrintingSystem != null && (PrintingSystem.ActiveLink != link || (PrintingSystem.Document.IsEmpty && !(PrintingSystem.Document.IsCreating)))) {
				link.ExecuteActivity(PrintingSystemActivity.Preparing, delegate() {
					link.CreateDocument(buildPagesInBackground);
				}); 
			}
		}
		protected override void BeforeShowPreview(XtraForm form, UserLookAndFeel lookAndFeel) {
			CreateIfEmpty(true);
		}
		protected override void BeforePrint() {
			CreateIfEmpty(false);
		}
	}
}
