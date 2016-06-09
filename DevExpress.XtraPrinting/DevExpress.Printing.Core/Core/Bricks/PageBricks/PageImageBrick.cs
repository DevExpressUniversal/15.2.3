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
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
#if SL
using System.Windows.Media;
#else
using System.Drawing;
#endif
namespace DevExpress.XtraPrinting {
	public class PageImageBrick : ImageBrick, IPageBrick {
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageImageBrickAlignment"),
#endif
		XtraSerializableProperty]
		public BrickAlignment Alignment { get { return AlignmentCore; } set { AlignmentCore = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageImageBrickLineAlignment"),
#endif
		XtraSerializableProperty]
		public BrickAlignment LineAlignment { get { return LineAlignmentCore; } set { LineAlignmentCore = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PageImageBrickBrickType")]
#endif
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override ImageAlignment ImageAlignment { get { return ImageAlignment.Default; } set { } }
		public override string BrickType { get { return BrickTypes.PageImage; } }
		public PageImageBrick() {
			Sides = BorderSide.None;
		}
		public PageImageBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor)
			: base(sides, borderWidth, borderColor, backColor) {
			Sides = BorderSide.None;
		}
	}
}
