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
using DevExpress.XtraPrinting.Native;
using System;
using System.Drawing;
using Export = DevExpress.XtraPrinting.Export;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using System.Collections;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Windows.Forms;
#else
using System.Windows.Forms;
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.XtraPrinting.NativeBricks {
#if !SL
	[BrickExporter(typeof(ContainerBrickBaseExporter))]
#endif
	public abstract class ContainerBrickBase : PanelBrick {
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override BrickCollectionBase Bricks { get { return base.Bricks; } }
		internal override IList InnerBrickList { get {  return EmptyBrickCollection.Instance; } }
		protected ContainerBrickBase(IBrickOwner brickOwner) 
			: base(brickOwner) {
		}
		protected ContainerBrickBase(BrickStyle style)
			: base(style) {
		}
	}
#if !SL
	[BrickExporter(typeof(CheckBoxTextBrickExporter))]
#endif
	public class CheckBoxTextBrick : ContainerBrickBase, ICheckBoxBrick {
		HorzAlignment checkBoxAlignment = HorzAlignment.Near;
		TextBrick textBrick;
		CheckBoxBrick checkBoxBrick;
		internal CheckBoxBrick CheckBoxBrick { get { return checkBoxBrick; } }
		internal TextBrick TextBrick { get { return textBrick; } }
		bool IsTextBrickEmpty { get { return string.IsNullOrEmpty(textBrick.Text); } }
		[XtraSerializableProperty,
		DefaultValue(null)]
		public string CheckText {
			get { return CheckBoxBrick.CheckText; }
			set { CheckBoxBrick.CheckText = value; }
		}
		internal string CheckStateText {
			get { return CheckBoxBrick.CheckStateText; }
		}
		[XtraSerializableProperty,
		DefaultValue(CheckState.Unchecked)]
		public CheckState CheckState { get { return CheckBoxBrick.CheckState; } set { CheckBoxBrick.CheckState = value; } }
		[XtraSerializableProperty,
		DefaultValue("")]
		public override string Text { get { return TextBrick.Text; } set { TextBrick.Text = value; } }
		public override string BrickType { get { return BrickTypes.CheckBoxText; } }
		[XtraSerializableProperty,
		DefaultValue(null)]
		public override object TextValue {
			get { return TextBrick.TextValue; }
			set { TextBrick.TextValue = value; }
		}
		[XtraSerializableProperty,
		DefaultValue(null)]
		public override string TextValueFormatString {
			get { return TextBrick.TextValueFormatString; }
			set { TextBrick.TextValueFormatString = value; }
		}
		[XtraSerializableProperty,
		DefaultValue(null)]
		public override string XlsxFormatString {
			get { return TextBrick.XlsxFormatString; }
			set { TextBrick.XlsxFormatString = value; }
		}
		[XtraSerializableProperty,
		DefaultValue(HorzAlignment.Near)]
		public HorzAlignment CheckBoxAlignment {
			get { return checkBoxAlignment; }
			set {
				if(value != HorzAlignment.Near && value != HorzAlignment.Far)
					throw new NotSupportedException();
				checkBoxAlignment = value;
			}
		}
		public CheckBoxTextBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
			textBrick = new TextBrick();
			checkBoxBrick = new CheckBoxBrick();
		}
		public CheckBoxTextBrick()
			: this(NullBrickOwner.Instance) {
		}
		public CheckBoxTextBrick(BrickStyle style)
			: base(style) {
			textBrick = new TextBrick();
			checkBoxBrick = new CheckBoxBrick();
		}
		protected override void OnSetPrintingSystem(bool cacheStyle) {
			Bricks.Clear();
			AddBricks();
			base.OnSetPrintingSystem(cacheStyle);
			InitializeInnerBricksStyles();
			RectangleF panelRect = new RectangleF(PointF.Empty, Rect.Size);
			RectangleF clientRect = GetClientRectangle(panelRect, GraphicsDpi.Document);
			clientRect = Padding.Deflate(clientRect, GraphicsDpi.Document);
			SizeF checkBoxSize = GraphicsUnitConverter.Convert(CheckBoxBrick.DefaultCheckSize(), GraphicsDpi.Pixel, GraphicsDpi.Document);
			CheckBoxBrick.InitialRect = RectF.Align(new RectangleF(PointF.Empty, checkBoxSize), clientRect, CheckBoxAlignment == HorzAlignment.Near ? BrickAlignment.Near : BrickAlignment.Far, BrickAlignment.Center);
			if(!IsTextBrickEmpty) {
				RectangleF textRect = clientRect;
				float textOffset = checkBoxSize.Width + GraphicsUnitConverter.Convert(2, GraphicsUnit.Pixel, GraphicsUnit.Document);
				if(CheckBoxAlignment == HorzAlignment.Near) {
					textRect.X += textOffset;
				}
				textRect.Width -= textOffset;
				TextBrick.InitialRect = textRect; 
			}
		}
		void AddBricks() {
			Bricks.Add(checkBoxBrick);
			if(!IsTextBrickEmpty)
				Bricks.Add(textBrick);
		}
		void InitializeInnerBricksStyles() {
			BrickStyle style = new BrickStyle(Style);
			style.Sides = BorderSide.None;
			style.Padding = PaddingInfo.Empty;
			TextBrick.Style = style;
			CheckBoxBrick.Style = TextBrick.Style;
		}
	} 
}
