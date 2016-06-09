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
using System.IO;
using DevExpress.Utils.StoredObjects;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraPrinting {
	public abstract class TextBrickBase : VisualBrick {
		protected string fText = String.Empty;
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("TextBrickBaseForeColor")]
#endif
		public Color ForeColor { get { return Style.ForeColor; } set { Style = BrickStyleHelper.ChangeForeColor(Style, value); } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("TextBrickBaseText"),
#endif
		XtraSerializableProperty,
		DefaultValue(""),
		]
		public override string Text { get { return fText != null ? fText : String.Empty; } set { fText = value; } }
		protected TextBrickBase()
			: base() {
		}
		protected TextBrickBase(BorderSide sides, float borderWidth, Color borderColor, Color backColor, Color foreColor)
			: base(sides, borderWidth, borderColor, backColor) {
			Style.ForeColor = foreColor;
		}
		protected TextBrickBase(IBrickOwner brickOwner)
			: base(brickOwner) {
		}
		protected TextBrickBase(BrickStyle style)
			: base(style) {
		}
		internal TextBrickBase(TextBrickBase brick)
			: base(brick) {
			fText = brick.Text;
		}
		protected override void StoreValues(BinaryWriter writer, IRepositoryProvider provider) {
			base.StoreValues(writer, provider);
			writer.Write(provider.StoreObject<string>(Text));
		} 
		protected override void RestoreValues(BinaryReader reader, IRepositoryProvider provider) {
			base.RestoreValues(reader, provider);
			Text = provider.RestoreObject<string>(reader.ReadInt64(), string.Empty);
		}
	}
}
