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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.NativeBricks {
	[BrickExporter(typeof(ToggleSwitchTextBrickExporter))]
	public class ToggleSwitchTextBrick : ContainerBrickBase {
		HorzAlignment checkBoxAlignment = HorzAlignment.Near;
		internal ToggleSwitchBrick ToggleSwitchBrick { get { return (ToggleSwitchBrick)Bricks[0]; } }
		internal TextBrick TextBrick { get { return (TextBrick)Bricks[1]; } }
		[XtraSerializableProperty]
		[DefaultValue(null)]
		public string CheckText {
			get { return ToggleSwitchBrick.CheckText; }
			set { ToggleSwitchBrick.CheckText = value; }
		}
		internal string CheckStateText {
			get {
				return ToggleSwitchBrick.CheckStateText;
			}
		}
		[XtraSerializableProperty]
		[DefaultValue(false)]
		public bool IsOn { get { return ToggleSwitchBrick.IsOn; } set { ToggleSwitchBrick.IsOn = value; } }
		public System.Collections.ArrayList ImageList { get { return ToggleSwitchBrick.ImageList; } set { ToggleSwitchBrick.ImageList = value; } }
		[
		XtraSerializableProperty,
		DefaultValue(""),
		]
		public override string Text {
			get { return TextBrick.Text; }
			set { TextBrick.Text = value; }
		}
		public override string BrickType { get { return BrickTypes.ToggleSwitchText; } }
		[
		XtraSerializableProperty,
		DefaultValue(null),
		]
		public override object TextValue {
			get {
				return TextBrick.TextValue;
			}
			set {
				TextBrick.TextValue = value;
			}
		}
		[
		XtraSerializableProperty,
		DefaultValue(null),
		]
		public override string TextValueFormatString {
			get {
				return TextBrick.TextValueFormatString;
			}
			set {
				TextBrick.TextValueFormatString = value;
			}
		}
		[
		XtraSerializableProperty,
		DefaultValue(null),
		]
		public override string XlsxFormatString {
			get {
				return TextBrick.XlsxFormatString;
			}
			set {
				TextBrick.XlsxFormatString = value;
			}
		}
		[XtraSerializableProperty,
		DefaultValue(HorzAlignment.Near)]
		public HorzAlignment CheckBoxAlignment {
			get {
				return checkBoxAlignment;
			}
			set {
				if(value != HorzAlignment.Near && value != HorzAlignment.Far)
					throw new NotSupportedException();
				checkBoxAlignment = value;
			}
		}
		public ToggleSwitchTextBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
			Bricks.Add(new ToggleSwitchBrick());
			Bricks.Add(new TextBrick());
		}
		public ToggleSwitchTextBrick()
			: this(NullBrickOwner.Instance) {
		}
		public ToggleSwitchTextBrick(BrickStyle style)
			: base(style) {
			Bricks.Add(new ToggleSwitchBrick());
			Bricks.Add(new TextBrick());
		}
		protected override void OnSetPrintingSystem(bool cacheStyle) {
			base.OnSetPrintingSystem(cacheStyle);
			InitializeInnerBricksStyles();
			RectangleF panelRect = new RectangleF(PointF.Empty, Rect.Size);
			RectangleF clientRect = GetClientRectangle(panelRect, GraphicsDpi.Document);
			clientRect = Padding.Deflate(clientRect, GraphicsDpi.Document);
			SizeF checkBoxSize = ToggleSwitchBrick.CheckSize;
			ToggleSwitchBrick.InitialRect = RectF.Align(new RectangleF(PointF.Empty, checkBoxSize), clientRect, CheckBoxAlignment == HorzAlignment.Near ? BrickAlignment.Near : BrickAlignment.Far, BrickAlignment.Center);
			RectangleF textRect = clientRect;
			float textOffset = checkBoxSize.Width + GraphicsUnitConverter.Convert(2, GraphicsUnit.Pixel, GraphicsUnit.Document);
			if(CheckBoxAlignment == HorzAlignment.Near) {
				textRect.X += textOffset;
			}
			textRect.Width -= textOffset;
			TextBrick.InitialRect = textRect;
		}
		void InitializeInnerBricksStyles() {
			BrickStyle style = new BrickStyle(Style);
			style.Sides = BorderSide.None;
			style.Padding = PaddingInfo.Empty;
			TextBrick.Style = style;
			ToggleSwitchBrick.Style = TextBrick.Style;
		}
	}
}
