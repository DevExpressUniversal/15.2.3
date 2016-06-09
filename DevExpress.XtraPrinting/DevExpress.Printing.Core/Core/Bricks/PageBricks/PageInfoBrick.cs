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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraPrinting {
	public class PageInfoBrick : PageInfoTextBrick, IPageBrick {
		protected bool fAutoWidth;
		public PageInfoBrick() {
		}
		public PageInfoBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor, Color foreColor)
			: base(sides, borderWidth, borderColor, backColor, foreColor) {
		}
		#region properties
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoBrickAlignment"),
#endif
 XtraSerializableProperty]
		public BrickAlignment Alignment { get { return AlignmentCore; } set { AlignmentCore = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoBrickLineAlignment"),
#endif
 XtraSerializableProperty]
		public BrickAlignment LineAlignment { get { return LineAlignmentCore; } set { LineAlignmentCore = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoBrickAutoWidth"),
#endif
 XtraSerializableProperty]
		public bool AutoWidth { get { return fAutoWidth; } set { fAutoWidth = value; } }
		protected override bool AutoWidthCore {
			get { return AutoWidth; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PageInfoBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.PageInfo; } }
		#endregion
		protected internal override void PerformLayout(IPrintingSystemContext context) {
			if(Width == 0) AutoWidth = true;
			base.PerformLayout(context);
		}
	}
}
