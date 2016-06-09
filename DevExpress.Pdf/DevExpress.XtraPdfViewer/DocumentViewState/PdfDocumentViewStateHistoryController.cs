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
using System.Drawing;
using DevExpress.DocumentView;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfDocumentViewStateHistoryController {
		readonly PdfViewer viewer;
		PdfDocumentViewStateHistory history;
		int lockCount;
		public bool IsTherePreviousState { get { return history.IsTherePreviousState; } }
		public bool IsThereNextState { get { return history.IsThereNextState; } }
		bool AllowLoad { get { return history.CurrentState.NavigationMode == PdfNavigationMode.ReferencedDocumentOpening; } }
		public PdfDocumentViewStateHistoryController(PdfViewer viewer) {
			this.viewer = viewer;
			history = new PdfDocumentViewStateHistory(GetCurrentDocumentViewState(PdfNavigationMode.None));
		}
		public void RegisterCurrentDocumentViewState(PdfNavigationMode navigationMode) {
			if (lockCount == 0)
				history.Add(GetCurrentDocumentViewState(navigationMode));
		}
		public void GoToPreviousState() {
			bool allowLoad = AllowLoad;
			history.GoToPreviousState();
			RestoreViewerState(allowLoad, GoToNextState);
		}
		public void GoToNextState() {
			history.GoToNextState();
			RestoreViewerState(AllowLoad, GoToPreviousState);
		}
		public void PerformLockedOperation(Action action) {
			lockCount++;
			try {
				action();
			}
			finally {
				lockCount--;
			}
		}
		public void Clear() {
			if (lockCount == 0 || String.IsNullOrWhiteSpace(history.CurrentState.FilePath))
				history = new PdfDocumentViewStateHistory(GetCurrentDocumentViewState(PdfNavigationMode.None));
		}
		void RestoreViewerState(bool allowLoad, Action exceptionAction) {
			try {
				PerformLockedOperation(() => {
					ViewManager viewManager = viewer.Viewer.ViewManager;
					PdfDocumentViewState state = history.CurrentState;
					if (allowLoad)
						viewer.LoadDocument(state.FilePath);
					viewer.RotationAngle = state.RotationAngle;
					viewer.ZoomFactor = state.Zoom;
					viewer.ZoomMode = state.ZoomMode;
					PointF location = state.Location;
					viewManager.SetHorizScroll(location.X);
					viewManager.SetVertScroll(location.Y);
					viewer.HandleScrolling();
				});
			}
			catch {
				exceptionAction();
			}
		}
		PdfDocumentViewState GetCurrentDocumentViewState(PdfNavigationMode navigationMode) {
			return new PdfDocumentViewState(navigationMode, viewer.Viewer.ViewManager.ScrollPos, viewer.ZoomMode, viewer.ZoomFactor, viewer.RotationAngle, viewer.DocumentFilePath, DateTime.Now);
		}
	}
}
