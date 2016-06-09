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

using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Mouse;
using System;
using System.Collections.Generic;
#if !SL
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using DevExpress.XtraRichEdit.Fields;
#endif
namespace DevExpress.Snap.Core.Native.MouseHandler {
	public class SnapRectangularObjectResizeMouseHandlerState : RichEditRectangularObjectResizeMouseHandlerState {
		public SnapRectangularObjectResizeMouseHandlerState(SnapMouseHandler mouseHandler, RectangularObjectHotZone hotZone, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, hotZone, initialHitTestResult) {
		}
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		protected override void ApplySizeChangesCore(IRectangularObject rectangularObject) {
			base.ApplySizeChangesCore(rectangularObject);
			UpdateSnapObjects();
		}
		void UpdateSnapObjects() {
			SnapFieldInfo selectedField = FieldsHelper.GetSelectedField(DocumentModel);
			if(selectedField == null)
				return;
			CalculatedFieldBase field = new SnapFieldCalculatorService().ParseField(selectedField);
			if (field is SNImageField)
				((SnapPieceTable)ActivePieceTable).SetSizeInfo(selectedField.Field);
			if(!(field is SNChartField) && !(field is SNSparklineField))
				return;
			ActivePieceTable.FieldUpdater.UpdateFieldAndNestedFields(selectedField.Field);
		}
	}
}
