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

using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting {
	[BrickExporter(typeof(ToggleSwitchBrickExporter))]
	public class ToggleSwitchBrick : VisualBrick {		
		string checkText = null;
		bool isOnCore;
		float toDpi = GraphicsDpi.Document;
		[XtraSerializableProperty]
		[DefaultValue(null)]
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
					IsOn ? "On" : "Off";
			}
		}
		public SizeF CheckSize {
			get { return GraphicsUnitConverter.Convert(new Size(50, 19), GraphicsDpi.Pixel, GraphicsDpi.Document); }
		}
		[
		XtraSerializableProperty,
		DefaultValue(false)]
		public bool IsOn {
			get { return isOnCore; }
			set {
				if (value != IsOn)
					isOnCore = value;
			}
		}
		public System.Collections.ArrayList ImageList { get; set; }
		public override string BrickType { get { return BrickTypes.ToggleSwitch; } }
		public ToggleSwitchBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor, Color foreColor)
			: this(sides, borderWidth, borderColor, backColor) {
			Style.ForeColor = foreColor;
		}
		public ToggleSwitchBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor)
			: base(sides, borderWidth, borderColor, backColor) {			
		}
		public ToggleSwitchBrick()
			: this(NullBrickOwner.Instance) {
		}
		public ToggleSwitchBrick(IBrickOwner brickOwner)
			: base(brickOwner) {			
		}
		public ToggleSwitchBrick(BrickStyle style)
			: base(style) {			
		}
	}
}
