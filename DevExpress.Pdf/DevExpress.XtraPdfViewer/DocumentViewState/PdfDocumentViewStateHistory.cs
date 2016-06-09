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

using System.Collections.Generic;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfDocumentViewStateHistory {
		readonly List<PdfDocumentViewState> viewStates = new List<PdfDocumentViewState>();
		int currentIndex = 0;
		public PdfDocumentViewState CurrentState { get { return viewStates[currentIndex]; } }
		public bool IsTherePreviousState { get { return currentIndex > 0; } }
		public bool IsThereNextState { get { return currentIndex < viewStates.Count - 1; } }
		public PdfDocumentViewStateHistory(PdfDocumentViewState viewState) {
			viewStates.Add(viewState);
		}
		public void Add(PdfDocumentViewState viewState) {
			if (CompareState(viewState, CurrentState)) {
				if (CurrentState.CompareNavigationModesAndTimeStamps(viewState))
					GoToPreviousState();
				currentIndex++;
				int countToRemove = viewStates.Count - currentIndex;
				if (countToRemove > 0)
					viewStates.RemoveRange(currentIndex, countToRemove);
				if (CompareState(viewState, viewStates[currentIndex - 1]))
					viewStates.Add(viewState);
				else
					currentIndex--;
			}
		}
		public void GoToPreviousState() {
			if (IsTherePreviousState)
				currentIndex--;
		}
		public void GoToNextState() {
			if (IsThereNextState)
				currentIndex++;
		}
		bool CompareState(PdfDocumentViewState newState, PdfDocumentViewState currentState) {
			return newState.Zoom != currentState.Zoom || newState.ZoomMode != currentState.ZoomMode || newState.Location != currentState.Location || newState.RotationAngle != currentState.RotationAngle || newState.FilePath != currentState.FilePath;
		}
	}
}
