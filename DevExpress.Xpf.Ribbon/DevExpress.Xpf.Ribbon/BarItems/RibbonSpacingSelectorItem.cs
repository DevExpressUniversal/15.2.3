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

using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonSpacingSelectorItem : BarSubItem {
		static RibbonSpacingSelectorItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(RibbonSpacingSelectorItem), typeof(RibbonSpacingSelectorItemLink), (item) => { return new RibbonSpacingSelectorItemLink(); });
			BarItemLinkControlCreator.Default.RegisterObject(typeof(RibbonSpacingSelectorItemLink), typeof(RibbonSpacingSelectorItemLinkControl), (link) => { return new RibbonSpacingSelectorItemLinkControl(link as RibbonSpacingSelectorItemLink); });
		}
		public RibbonControl Ribbon {
			get { return ribbon; }
			set {
				if (value == ribbon)
					return;
				RibbonControl oldValue = ribbon;
				ribbon = value;
				OnRibbonChanged(oldValue);
			}
		}
		protected override bool CanOpenMenu {
			get { return true; }
		}
		public RibbonSpacingSelectorItem() {
			Glyph = ImageHelper.GetImage("touchMode_16x16.png");
			AllowGlyphTheming = true;
		}
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			ExecuteActionOnLinkControls(x => ((RibbonSpacingSelectorItemLinkControl)x).UpdateActualRibbon());
		}
		RibbonControl ribbon;
	}
}
