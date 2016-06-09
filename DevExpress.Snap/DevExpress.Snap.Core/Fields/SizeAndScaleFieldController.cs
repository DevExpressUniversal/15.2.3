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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Office.Utils;
using System.Drawing;
namespace DevExpress.Snap.Core.Fields {
	public abstract class SizeAndScaleFieldController {
		public static SizeAndScaleFieldController Create(CalculatedFieldBase field, InstructionController controller) {
			if(field is SNImageField) return new SNImageFieldController(controller);
			if(field is SNBarCodeField) return new SNBarCodeFieldController(controller);
			if(field is SNChartField) return new SNChartFieldController(controller);
			if(field is SNSparklineField) return new SNSparklineFieldController(controller);
			return null;
		}
		public abstract bool IsReady { get; }
		public abstract void SetImageSizeInfo();
	}
	public abstract class SizeAndScaleFieldController<TField> : SizeAndScaleFieldController
	where TField : MergefieldField {
		readonly InstructionController controller;
		readonly IRectangularObject rectangularObject;
		protected SizeAndScaleFieldController(InstructionController controller, IRectangularObject rectangularObject) {
			Guard.ArgumentNotNull(controller, "controller");
			this.controller = controller;
			this.rectangularObject = rectangularObject;
		}
		protected InstructionController Controller { get { return controller; } }
		public override bool IsReady { get { return rectangularObject != null; } }
		protected TField GetField() {
			return (TField)controller.ParsedInfo;
		}
		public override void SetImageSizeInfo() {
			controller.SuppressFieldsUpdateAfterUpdateInstruction = true;
			SetImageSizeInfoCore();
			controller.ApplyDeferredActions();
		}
		protected abstract void SetImageSizeInfoCore();
		protected void ClearSize() {
			controller.RemoveSwitch("h");
			controller.RemoveSwitch("w");
		}
		protected void SetSize() {
			controller.SetSwitch("w", rectangularObject.ActualSize.Width.ToString(NumberFormatInfo.InvariantInfo));
			controller.SetSwitch("h", rectangularObject.ActualSize.Height.ToString(NumberFormatInfo.InvariantInfo));
		}
		protected void ClearScale() {
			controller.RemoveSwitch("sx");
			controller.RemoveSwitch("sy");
		}
		protected void SetScale() {
			IRectangularScalableObject scalableObject = rectangularObject as IRectangularScalableObject;
			controller.SetSwitch("sx", Math.Round(scalableObject.ScaleX * 100).ToString(NumberFormatInfo.InvariantInfo));
			controller.SetSwitch("sy", Math.Round(scalableObject.ScaleY * 100).ToString(NumberFormatInfo.InvariantInfo));
		}
	}
	public class RichEditImageWrapper : IRectangularObject {
		readonly OfficeImage image;
		public RichEditImageWrapper(OfficeImage image) {
			this.image = image;
		}
		public Size ActualSize { get { return image.SizeInPixels; } set { throw new NotSupportedException(); } }
	}
}
