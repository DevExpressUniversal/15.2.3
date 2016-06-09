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

using System.Linq;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
namespace DevExpress.Xpf.PdfViewer {
	public class InplaceEditingStrategy {
		readonly PdfPresenterControl presenter;
		public InplaceEditingStrategy(PdfPresenterControl presenter) {
			this.presenter = presenter;
		}
		public void StartEditing(PdfEditorSettings editorSettings, IPdfViewerValueEditingCallBack valueEditing) {
			var owner = new CellEditorOwner(presenter);
			var cellEditor = new CellEditor(owner, valueEditing, new CellEditorColumn(presenter.PdfBehaviorProvider, (PdfPageViewModel)presenter.Document.Pages.ElementAt(editorSettings.DocumentArea.PageIndex), editorSettings));
			owner.CurrentCellEditor = cellEditor;
			var area = editorSettings.DocumentArea;
			PdfPageViewModel page = (PdfPageViewModel)presenter.Document.Pages.ElementAt(area.PageIndex);
			var pageWrapperIndex = presenter.NavigationStrategy.PositionCalculator.GetPageWrapperIndex(area.PageIndex);
			PdfPageWrapper pageWrapper = (PdfPageWrapper)presenter.Pages.ElementAt(pageWrapperIndex);
			presenter.AttachEditorToTree(owner, area.PageIndex, () => CalcCellEditorPosition(page, pageWrapper, area), presenter.PdfBehaviorProvider.RotateAngle);
			cellEditor.ShowEditorIfNotVisible(true);
		}
		Rect CalcCellEditorPosition(PdfPageViewModel page, PdfPageWrapper pageWrapper, PdfDocumentArea area) {
			var pageRect = pageWrapper.GetPageRect(page);
			var topLeft = page.GetPoint(area.Area.TopLeft, presenter.PdfBehaviorProvider.ZoomFactor, presenter.PdfBehaviorProvider.RotateAngle);
			var bottomRight = page.GetPoint(area.Area.BottomRight, presenter.PdfBehaviorProvider.ZoomFactor, presenter.PdfBehaviorProvider.RotateAngle);
			return new Rect(topLeft.X + pageRect.Left, topLeft.Y + pageRect.Top, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
		}
		public void EndEditing() {
			presenter.ActiveEditorOwner.Do(x => x.CurrentCellEditor.CommitEditor(true));
			presenter.DetachEditorFromTree();
			presenter.Focus();
		}
	}
}
