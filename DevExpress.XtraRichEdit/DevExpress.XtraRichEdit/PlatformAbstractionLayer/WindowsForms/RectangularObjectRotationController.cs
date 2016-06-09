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
using System.Drawing;
using DevExpress.Services;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Mouse;
namespace DevExpress.XtraRichEdit.Internal {
	public interface IRectangularObjectRotationControllerOwner {
		void CompleteRectangularObjectRotation();
	}
	public class RectangularObjectRotationController {
		bool commitChangesOnNextTimerTick = false;
		IRectangularObjectRotationControllerOwner owner;
		readonly RichEditRectangularObjectRotateByGestureMouseHandlerState state;
		public RectangularObjectRotationController(IRectangularObjectRotationControllerOwner owner, RichEditRectangularObjectRotateByGestureMouseHandlerState state) {
			this.owner = owner;
			this.state = state;
		}
		public IRectangularObjectRotationControllerOwner Owner { get { return owner; } }
		public RichEditRectangularObjectRotateByGestureMouseHandlerState State { get { return state; } }
		public static RichEditRectangularObjectRotateByGestureMouseHandlerState CreateMouseHandlerState(Point center, RichEditControl richEditControl) {
			RichEditView activeView = richEditControl.ActiveView;
			RichEditHitTestResult hitTestResult = activeView.CalculateNearestCharacterHitTest(center, richEditControl.InnerControl.DocumentModel.ActivePieceTable);
			HotZoneCollection hotZones = activeView.SelectionLayout.LastDocumentSelectionLayout.HotZones;
			if (hotZones.Count == 0)
				return null;
			RectangularObjectHotZone rotationHotZone = null;
			for (int i = 0; i < hotZones.Count; i++) {
				rotationHotZone = hotZones[i] as RectangularObjectRotationHotZone;
				if (rotationHotZone != null)
					break;
			}
			RichEditMouseHandlerService svc = richEditControl.GetService<IMouseHandlerService>() as RichEditMouseHandlerService;
			if (svc == null)
				return null;
			RichEditMouseHandler mouseHandler = svc.Handler as RichEditMouseHandler;
			if (mouseHandler == null)
				return null;
			RichEditRectangularObjectRotateByGestureMouseHandlerState state = new RichEditRectangularObjectRotateByGestureMouseHandlerState(mouseHandler, rotationHotZone, hitTestResult);
			return state;
		}
		public void Rotate(Point center, double degreeDelta) {
			State.Rotate((float)degreeDelta);
			commitChangesOnNextTimerTick = false;
		}
		public void OnTimerTick() {
			if (commitChangesOnNextTimerTick) {
				State.Terminate();
				Owner.CompleteRectangularObjectRotation();
				commitChangesOnNextTimerTick = false;
			}
			commitChangesOnNextTimerTick = true;
		}
	}
}
