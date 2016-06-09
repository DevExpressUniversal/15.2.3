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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing.Native {
	public class DXPrintingDocument : PrintingDocument {
		public DXPrintingDocument(PrintingSystemBase ps)
			: base(ps, null, null) {
		}
		#region PrintingDocument overrides
		protected internal override void Begin() {
			SetRoot(new RootDocumentBand(PrintingSystem));
			base.Begin();
		}
		protected internal override void BeginReport(DocumentBand docBand, System.Drawing.PointF offset) {
			throw new NotImplementedException();
		}
		protected internal override void EndReport() {
			throw new NotImplementedException();
		}
		protected internal override DocumentBand AddReportContainer() {
			throw new NotImplementedException();
		}
		protected internal override void InsertPageBreak(float pos) {
			throw new NotImplementedException();
		}
		protected internal override void InsertPageBreak(float pos, CustomPageData nextPageData) {
			throw new NotImplementedException();
		}
		public override void ShowFromNewPage(DevExpress.XtraPrinting.Brick brick) {
			throw new NotImplementedException();
		}
		public override int AutoFitToPagesWidth {
			get {
				return base.AutoFitToPagesWidth;
			}
			set {
				if(value != 0)
					throw new NotSupportedException();
				base.AutoFitToPagesWidth = value;
			}
		}
		#endregion
	}
}
