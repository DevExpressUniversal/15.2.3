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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting {
	public class PageTableBrick : PanelBrick, IPageBrick {
		TableRowCollection rows;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageTableBrickRows"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false),
		]
		public TableRowCollection Rows { get { return rows; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageTableBrickAlignment"),
#endif
		XtraSerializableProperty]
		public BrickAlignment Alignment { get { return AlignmentCore; } set { AlignmentCore = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageTableBrickLineAlignment"),
#endif
		XtraSerializableProperty]
		public BrickAlignment LineAlignment { get { return LineAlignmentCore; } set { LineAlignmentCore = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageTableBrickBricks"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override BrickCollectionBase Bricks { get { return base.Bricks; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PageTableBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.PageTable; } }
		public PageTableBrick() {
			Sides = BorderSide.None;
#if SL
			BackColor = System.Windows.Media.Colors.Transparent;
#else
			BackColor = Color.Transparent;
#endif
			rows = new TableRowCollection();
		}
		public void UpdateSize() {
			SizeF size = SizeF.Empty;
			foreach(TableRow row in rows) {
				SizeF sz = row.CalcSize();
				size.Height += sz.Height;
				size.Width = Math.Max(size.Width, sz.Width);
			}
			Size = size;
		}
		protected override void PerformInnerBricksLayout(IPrintingSystemContext context) {
			UpdateBricks();
			base.PerformInnerBricksLayout(context);
			UpdateSize();
			UpdateLayout();
		}
		protected override void OnSetPrintingSystem(bool cacheStyle) {
			UpdateBricks();
			base.OnSetPrintingSystem(cacheStyle);
		}
		void UpdateBricks() {
			Bricks.Clear();
			foreach(TableRow row in rows) {
				foreach(Brick brick in row.Bricks) {
					Bricks.Add(brick);
				}
			}
		}
		void UpdateLayout() {
			float yOffset = 0;
			foreach(TableRow row in rows) {
				row.Align(Alignment, yOffset, Width);
				yOffset += row.CalcSize().Height;
			}
		}
		protected override void SetIndexCollectionItemCore(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.Rows)
				Rows.Add((TableRow)e.Item.Value);
			base.SetIndexCollectionItemCore(propertyName, e);
		}
		protected override object CreateCollectionItemCore(string propertyName, XtraItemEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.Rows)
				return new TableRow();
			return base.CreateCollectionItemCore(propertyName, e);
		}
	}
}
