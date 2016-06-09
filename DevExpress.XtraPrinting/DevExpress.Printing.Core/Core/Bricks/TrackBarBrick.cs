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

using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraPrinting {
	[BrickExporter(typeof(TrackBarBrickExporter))]
	public class TrackBarBrick : VisualBrick, ITrackBarBrick {
		public TrackBarBrick() { }
		public TrackBarBrick(int position, int min, int max) {
			this.position = position;
			this.min = min;
			this.max = max;
		}
		int position, min, max;
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
		XtraSerializableProperty,
		DefaultValue(0),
		]
		public int Position { get { return position; } set { position = value; } }
		public int Minimum { get { return min; } }
		public int Maximum { get { return max; } }
		public Color ForeColor { get { return Style.ForeColor; } set { Style = BrickStyleHelper.ChangeForeColor(Style, value); } }
		public override string BrickType { get { return BrickTypes.TrackBar; } }
	}
}
