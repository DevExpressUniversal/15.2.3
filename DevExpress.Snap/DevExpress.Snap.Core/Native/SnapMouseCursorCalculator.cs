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

using System.Drawing;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Fields;
namespace DevExpress.Snap.Core.Native {
	public class SnapMouseCursorCalculator : MouseCursorCalculator {
		readonly InnerSnapControl innerControl;
		public SnapMouseCursorCalculator(InnerSnapControl innerControl, RichEditView view)
			: base(view) {
			this.innerControl = innerControl;			
		}
		public override RichEditCursor Calculate(RichEditHitTestResult hitTestResult, Point physicalPoint) {
			ILayoutUIElementViewInfo viewInfo = GetActiveUIElementViewInfo(innerControl, hitTestResult);
			if (viewInfo == null)
				return base.Calculate(hitTestResult, physicalPoint);
			else
				return viewInfo.GetCursor();
		}
		protected internal override RichEditCursor CalculateHotZoneCursorCore(HotZone hotZone) {
			if (FieldsHelper.IsNotResizableField(hotZone))
				return RichEditCursors.Default;
			return base.CalculateHotZoneCursorCore(hotZone);
		}
		ILayoutUIElementViewInfo GetActiveUIElementViewInfo(InnerSnapControl innerControl, RichEditHitTestResult hitTestResult) {
			foreach (ILayoutUIElementViewInfo viewInfo in innerControl.ActiveUIElementViewInfos) {
				if (viewInfo.HitTest(hitTestResult))
					return viewInfo;
			}
			return null;
		}
	}
}
