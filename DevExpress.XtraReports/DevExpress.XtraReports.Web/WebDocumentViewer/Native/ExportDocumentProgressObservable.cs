﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native {
	class ExportDocumentProgressObservable : DocumentProgressObservableBase<OperationProgress> {
		public ExportDocumentProgressObservable() { }
		public ExportDocumentProgressObservable(PrintingSystemBase printingSystem) : base(printingSystem) { }
		public override void AssignPrintingSystem(PrintingSystemBase printingSystem) {
			base.AssignPrintingSystem(printingSystem);
			printingSystem.ProgressReflector.PositionChanged += (object sender, EventArgs e) => {
				RaiseProgress(new OperationProgress(printingSystem.ProgressReflector.Position, false, null));
			};
			RaiseProgress(new OperationProgress(0, false, null));
		}
		public override void Complete(Exception optionalException) {
			if(printingSystem.Document == null || optionalException != null) {
				RaiseProgress(new OperationProgress(0, false, optionalException));
			} else {
				RaiseProgress(new OperationProgress(100, true, null));
			}
		}
	}
}
