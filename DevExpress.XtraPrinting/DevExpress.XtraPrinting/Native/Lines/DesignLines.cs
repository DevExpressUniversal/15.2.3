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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPrinting.Native.Lines {
	public class EmptyLine : BaseLine {
		public EmptyLine()
			: base() {
			Size = new Size(0, 5);
		}
		protected UserLookAndFeel LookAndFeel {
			get {
				XtraForm xtraForm = FindForm() as XtraForm;
				if (xtraForm != null)
					return xtraForm.LookAndFeel;
				else
					return UserLookAndFeel;
			}
		}
		public override void SetText(string text) {
		}
	}
	public class SeparatorLine : EmptyLine {
		LabelControl line;
		public SeparatorLine()
			: base() {
			Size = new Size(0, 8);
		}
		protected override void Initialize() {
			base.Initialize();
			line = new LabelControl();
			SetParentLookAndFeel(line);
			line.Location = new Point(0, 0);
			line.Size = new Size(ClientSize.Width, 5);
			line.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			line.LineVisible = true;
			line.LineOrientation = LabelLineOrientation.Horizontal;
			line.LineLocation = LineLocation.Center;
			line.AutoSizeMode = LabelAutoSizeMode.None;
			this.Controls.Add(line);
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			line.LineVisible = DrawSkinLine();
			base.OnPaintBackground(e);
			if(!line.LineVisible)
				using(GraphicsCache cache = new GraphicsCache(e)) {
					Color color = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ControlDark);
					using(SolidBrush brush = new SolidBrush(color)) {
						cache.FillRectangle(brush, new Rectangle(0, 4, ClientRectangle.Width, 1));
					}
				}
		}
		bool DrawSkinLine() {
			return LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Flat &&
				LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Style3D &&
				LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.UltraFlat;
		}
	}
}
