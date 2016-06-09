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
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting {
	[BrickExporter(typeof(ProgressBarBrickExporter))]
	public class ProgressBarBrick : VisualBrick, IProgressBarBrick {
		int position;
		object textValue;
		#region hidden properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string TextValueFormatString { get { return base.TextValueFormatString; } set { base.TextValueFormatString = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string XlsxFormatString { get { return base.XlsxFormatString; } set { base.XlsxFormatString = value; } }
		#endregion
		[
		XtraSerializableProperty,
		DefaultValue(null),
		EditorBrowsable(EditorBrowsableState.Always),
		]
		public override object TextValue {
			get { return textValue; }
			set { textValue = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ProgressBarBrickPosition"),
#endif
		XtraSerializableProperty,
		DefaultValue(0),
		]
		public int Position { get { return position; } set { position = value; } }
		internal protected int ValidPosition { get { return Math.Max(0, Math.Min(position, 100)); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ProgressBarBrickForeColor")]
#endif
		public Color ForeColor { get { return Style.ForeColor; } set { Style = BrickStyleHelper.ChangeForeColor(Style, value); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ProgressBarBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.ProgressBar; } }
		public ProgressBarBrick() {
		}
		public ProgressBarBrick(int position) {
			this.position = position;
		}
		public ProgressBarBrick(IBrickOwner brickOwner) 
			: base(brickOwner) {
		}
	}
}
