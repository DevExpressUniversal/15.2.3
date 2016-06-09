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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.Printing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting {
	public class GdiGraphicsWrapper : GdiGraphicsWrapperBase {
		readonly static Image checkImage;
		readonly static Image uncheckImage;
		readonly static Image checkGrayImage;
		static GdiGraphicsWrapper() {
			lock (typeof(GdiGraphicsWrapper)) {
				checkImage = CheckBoxImageHelper.GetCheckBoxImage(CheckState.Checked);
				uncheckImage = CheckBoxImageHelper.GetCheckBoxImage(CheckState.Unchecked);
				checkGrayImage = CheckBoxImageHelper.GetCheckBoxImage(CheckState.Indeterminate);
			}
		}
		public GdiGraphicsWrapper(Graphics gr)
			: base(gr) {
		}
		public override void DrawCheckBox(System.Drawing.RectangleF rect, System.Windows.Forms.CheckState state) {
			Image image = (state & CheckState.Checked) != 0 ? checkImage :
					(state & CheckState.Indeterminate) != 0 ? checkGrayImage :
					uncheckImage;
			lock (image) {
				Graphics.DrawImageUnscaled(image, Rectangle.Round(rect).Location);
			}
		}
	}
}
