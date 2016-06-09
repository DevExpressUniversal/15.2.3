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

using DevExpress.XtraPrinting.Export;
using System;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting.NativeBricks {
	public interface IRichTextBoxBrickOwner : IBrickOwner {
		RichTextBox RichTextBox { get; }
	}
	[BrickExporter(typeof(RichTextBoxBrickExporter))]
	public class RichTextBoxBrick : VisualBrick {
		string rtfText = string.Empty;
		bool detectUrls;
		[XtraSerializableProperty]
		public string RtfText {
			get { return rtfText; }
			set {
				if (RtfTags.IsRtfContent(value))
					rtfText = value;
				else
					rtfText = RtfTags.WrapTextInRtf(value);
			}
		}
		public bool DetectUrls { get { return detectUrls; } set { detectUrls = value; } }
		internal IRichTextBoxBrickOwner RichTextBoxBrickContainer { get { return (IRichTextBoxBrickOwner)BrickOwner; } }
		public override string BrickType { get { return BrickTypes.RichText; } }
		public RichTextBoxBrick(IRichTextBoxBrickOwner container)
			: base(container) { 
		}
		protected override float ValidatePageBottomInternal(float pageBottom, RectangleF rect, IPrintingSystemContext context) {
			string text = RichTextBoxBrickContainer.RichTextBox.Rtf;
			try {
				RichTextBoxBrickContainer.RichTextBox.Rtf = RtfText;
				int charFrom;
				RectangleF clientRect = GetClientRectangle(rect, GraphicsDpi.Document);
				pageBottom -= clientRect.Top;
				pageBottom = MathMethods.Scale(pageBottom, 1 / GetScaleFactor(context));
				RectangleF bounds = new RectangleF(clientRect.X, 0.0f, clientRect.Width, pageBottom);
				RectangleF newBounds = RichEditHelper.CorrectRtfLineBounds(GraphicsDpi.Document, RichTextBoxBrickContainer.RichTextBox, bounds, 0, out charFrom);
				if(newBounds.Height > bounds.Height)
					return rect.Top;
				if(charFrom < RichTextBoxBrickContainer.RichTextBox.Text.Length)
					pageBottom = newBounds.Bottom;
				return MathMethods.Scale(pageBottom, GetScaleFactor(context)) + clientRect.Top;
			} finally {
				RichTextBoxBrickContainer.RichTextBox.Rtf = text;
			}
		}
	}
}
