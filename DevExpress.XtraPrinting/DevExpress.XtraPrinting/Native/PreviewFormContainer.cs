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
using DevExpress.XtraPrinting.Preview;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting.Native {
	internal class PreviewFormContainer : IDisposable {
		PrintPreviewFormEx previewFormEx;
		PrintPreviewRibbonFormEx previewRibbonFormEx;
		PrintingSystemBase printingSystem;
		public PreviewFormContainer(PrintingSystemBase printingSystem) {
			this.printingSystem = printingSystem;
		}
		public PrintPreviewRibbonFormEx PreviewRibbonForm {
			get {
				if(PSNativeMethods.AspIsRunning)
					return null;
				if(previewRibbonFormEx == null || previewRibbonFormEx.IsDisposed) {
					previewRibbonFormEx = new PrintPreviewRibbonFormEx();
					previewRibbonFormEx.PrintingSystem = printingSystem;
				}
				return previewRibbonFormEx;
			}
		}
		public PrintPreviewFormEx PreviewForm {
			get {
				if(PSNativeMethods.AspIsRunning)
					return null;
				if(previewFormEx == null || previewFormEx.IsDisposed) {
					previewFormEx = new PrintPreviewFormEx();
					previewFormEx.PrintingSystem = printingSystem;
				}
				return previewFormEx;
			}
		}
		public virtual void Dispose() {
			DestroyForm(previewFormEx);
			DestroyForm(previewRibbonFormEx);
			previewFormEx = null;
			previewRibbonFormEx = null;
		}
		static void DestroyForm(Form form) {
			if(form != null && !form.IsDisposed)
				form.Dispose();
		}
	}
}
