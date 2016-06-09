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

using DevExpress.Utils;
using DevExpress.Xpf.Printing.Native;
using System;
#if !SL
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Printing {
	class ScaleService : IScaleService {
		PrintingSystemPreviewModel model;
		ScaleWindow scaleWindow;
		#region IScaleService Members
#if SL
		public void Scale(PrintingSystemPreviewModel model) {
			Guard.ArgumentNotNull(model, "model");
			this.model = model;
			scaleWindow = new ScaleWindow(model.PrintingSystem.Document.ScaleFactor, model.PrintingSystem.Document.AutoFitToPagesWidth);
			scaleWindow.ViewModel.ScaleApplied += ViewModel_ScaleApplied;
			scaleWindow.ShowDialog();
		}
#else
		public void Scale(PrintingSystemPreviewModel model, System.Windows.Window ownerWindow) {
			Guard.ArgumentNotNull(model, "model");
			this.model = model;
			scaleWindow = new ScaleWindow(model.PrintingSystem.Document.ScaleFactor, model.PrintingSystem.Document.AutoFitToPagesWidth);
			scaleWindow.Owner = ownerWindow;
			if(ownerWindow != null) {
				scaleWindow.FlowDirection = ownerWindow.FlowDirection;
			}
			scaleWindow.ViewModel.ScaleApplied += ViewModel_ScaleApplied;
			scaleWindow.ShowDialog();
		}
#endif
		void ViewModel_ScaleApplied(object sender, ScaleWindowViewModelEventArgs e) {
			scaleWindow.ViewModel.ScaleApplied -= ViewModel_ScaleApplied;
			if(e.ScaleMode == ScaleMode.AdjustToPercent) {
				model.PrintingSystem.Document.ScaleFactor = e.ScaleFactor;
				return;
			}
			if(e.ScaleMode == ScaleMode.FitToPageWidth) {
				model.PrintingSystem.Document.AutoFitToPagesWidth = e.PagesToFit;
				return;
			}
			throw new NotSupportedException("ScaleMode");
		}
		#endregion
	}
}
