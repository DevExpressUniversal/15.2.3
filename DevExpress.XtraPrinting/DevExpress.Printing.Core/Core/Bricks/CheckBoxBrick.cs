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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
#if SL
using DevExpress.Xpf.Windows.Forms;
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
#else
using System.Windows.Forms;
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.XtraPrinting {
	[
	BrickExporter(typeof(CheckBoxBrickExporter))
	]
	public class CheckBoxBrick : VisualBrick, ICheckBoxBrick {
		#region static
		internal static SizeF DefaultCheckSize() {
			float size = GraphicsUnitConverter.Convert(FontSizeHelper.GetSizeInPoints(BrickStyle.DefaultFont), GraphicsDpi.Point, GraphicsDpi.Pixel);
			return new SizeF(size, size);
		}
		#endregion
		string checkText = null;
		CheckState checkState;
		SizeF checkSize;
		float toDpi = GraphicsDpi.Document;
		[
		XtraSerializableProperty,
		DefaultValue(null)
		]
		public string CheckText {
			get { return checkText; }
			set { checkText = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal bool ShouldAlignToBottom {
			get;
			set;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal float ToDpi {
			get { return toDpi; }
			set { toDpi = value; }
		}
		internal string CheckStateText {
			get {
				return CheckText != null ? CheckText :
					CheckState == CheckState.Checked ? "[+]" :
					CheckState == CheckState.Unchecked ? "[-]" :
						"[?]";
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("CheckBoxBrickChecked")]
#endif
		public bool Checked {
			get { return checkState != CheckState.Unchecked; }
			set {
				if (value != Checked)
					CheckState = value ? CheckState.Checked : CheckState.Unchecked;
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("CheckBoxBrickCheckState"),
#endif
		XtraSerializableProperty,
		DefaultValue(CheckState.Unchecked),
		]
		public CheckState CheckState {
			get { return checkState; }
			set { checkState = value; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("CheckBoxBrickCheckSize")]
#endif
		public SizeF CheckSize {
			get { return checkSize; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("CheckBoxBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.CheckBox; } }
		public CheckBoxBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor, Color foreColor)
			: this(sides, borderWidth, borderColor, backColor) {
			Style.ForeColor = foreColor;
		}
		public CheckBoxBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor)
			: base(sides, borderWidth, borderColor, backColor) {
			checkSize = DefaultCheckSize();
		}
		public CheckBoxBrick()
			: this(NullBrickOwner.Instance) {
		}
		public CheckBoxBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
			checkSize = DefaultCheckSize();
		}
		public CheckBoxBrick(BrickStyle style)
			: base(style) {
			checkSize = DefaultCheckSize();
		}
		internal CheckBoxBrick(CheckBoxBrick brick) : base(brick) {
			checkSize = brick.checkSize;
			checkState = brick.checkState;
			checkText = brick.checkText;
		}
		public bool? GetCheckValue() {
			switch(CheckState) {
				case CheckState.Checked:
					return true;
				case CheckState.Unchecked:
					return false;
				default:
					return null;
			}
		}
		#region ICloneable Members
		public override object Clone() {
			return new CheckBoxBrick(this);
		}
		#endregion
	}
}
