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

using DevExpress.XtraPrinting.Native;
using System;
using System.Drawing;
using DevExpress.XtraPrinting.Shape.Native;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.XtraPrinting.Shape {
	public class ShapeBracket : ShapeBase {
		int tipLength = 20;
		protected int fFillet;
		protected int fTailLength;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeBracketTipLength"),
#endif
	   DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Shape.ShapeBracket.TipLength"),
		DefaultValue(20),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public int TipLength {
			get { return tipLength; }
			set { tipLength = ShapeHelper.ValidatePercentageValue(value, "TipLength"); }
		}
		internal override PreviewStringId ShapeStringId {
			get { return PreviewStringId.Shapes_Bracket; }
		}
		protected internal override bool SupportsFillColor {
			get { return false; }
		}
		public ShapeBracket() {
		}
		protected ShapeBracket(ShapeBracket source) {
			tipLength = source.TipLength;
		}
		protected internal override ShapeCommandCollection CreateCommands(RectangleF bounds, int angle) {
			return ShapeHelper.CreateBraceCommands(bounds, fTailLength, TipLength, fFillet);
		}
		protected override ShapeBase CloneShape() {
			return new ShapeBracket(this);
		}
		protected override RectangleF AdjustClientRectangle(IGraphics gr, RectangleF clientBounds, float lineWidth) {
			return RectangleF.Inflate(clientBounds, -lineWidth / 2, -lineWidth / 2);
		}
	}
}
