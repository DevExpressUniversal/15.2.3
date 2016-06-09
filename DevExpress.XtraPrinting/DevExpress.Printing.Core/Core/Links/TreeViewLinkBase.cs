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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export.Web;
using System.Collections;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export;
namespace DevExpress.XtraPrintingLinks
{
	[DefaultProperty("TreeView")]
	public class TreeViewLinkBase : DevExpress.XtraPrinting.LinkBase
	{
		#region inner classes
		class BrickFactory {
			const int space = 2;
			TreeNode node;
			TextBrick textBrick;
			CheckBoxBrick checkBrick;
			ImageBrick imageBrick;
			RectangleF textRect = RectangleF.Empty;
			RectangleF checkRect = RectangleF.Empty;
			RectangleF imageRect = RectangleF.Empty;
			bool CanHaveCheckBox { get { return node.TreeView.CheckBoxes; } }
			bool CanHaveImage { get { return node.TreeView.ImageList != null; } }
			bool HaveImage { get { return node.ImageIndex >= 0 && node.ImageIndex < node.TreeView.ImageList.Images.Count; } }
			Size ImageSize { get { return node.TreeView.ImageList.ImageSize; } }
			public static PanelBrick CreateBrick(TreeNode node) {
				return (new BrickFactory()).CreateBrickInternal(node);
			}
			static BrickStringFormat GetStringFormat() {
				StringFormat stringFormat;
				stringFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit);
				stringFormat.LineAlignment = StringAlignment.Center;
				stringFormat.Alignment = StringAlignment.Near;
				return new BrickStringFormat(stringFormat);
			}
			PanelBrick CreateBrickInternal(TreeNode node) {
				PanelBrick panelBrick = new PanelBrick();
				this.node = node;
				textBrick = new TextBrick();
				textBrick.StringFormat = GetStringFormat();
				textBrick.Text = node.Text;
				textBrick.Sides = BorderSide.None;
				textBrick.Font = node.NodeFont != null ? node.NodeFont : node.TreeView.Font;
				panelBrick.Bricks.Add(textBrick);
				if(CanHaveCheckBox) {
					checkBrick = new CheckBoxBrick();
					checkBrick.Checked = node.Checked;
					checkBrick.Sides = BorderSide.None;
					panelBrick.Bricks.Add(checkBrick);
				}
				if(CanHaveImage && HaveImage) {
					imageBrick = new ImageBrick();
					imageBrick.Image = GetImage();
					imageBrick.Sides = BorderSide.None;
					panelBrick.Bricks.Add(imageBrick);
				}
				CalcLayout();
				textBrick.Rect = textRect;
				if(checkBrick != null) checkBrick.Rect = checkRect;
				if(imageBrick != null) imageBrick.Rect = imageRect;
				panelBrick.Rect = GetNodeRect();
				return panelBrick;
			}
			void CalcLayout() {
				textRect = GetNodeRect();
				float x = space + node.TreeView.Indent * node.Level;
				if(CanHaveCheckBox) {
					checkRect.Size = GetCheckSize();
					checkRect.Y = (textRect.Height - checkRect.Height) / 2;
					checkRect.X = x;
					x = checkRect.Right + space;
				}
				if(CanHaveImage) {
					imageRect = new RectangleF(x, 0, ImageSize.Width, ImageSize.Height);
					imageRect.Y = (textRect.Height - imageRect.Height) / 2;
					x = imageRect.Right + space;
				}
				textRect.X = x;
				textRect.Y = 0;
			}
			RectangleF GetNodeRect() {
				RectangleF rect = node.Bounds;
				rect.X = 0;
				rect.Width = node.TreeView.Width;
				return rect;
			}
			SizeF GetCheckSize() {
				System.Diagnostics.Debug.Assert(checkBrick != null);
				return checkBrick.CheckSize;
			}
			Image GetImage() {
				Image img = null;
				if(CanHaveImage && HaveImage)
					img = node.TreeView.ImageList.Images[node.ImageIndex];
				return img;
			}
		}
		#endregion
		private TreeView treeView;
		PointF basePoint = PointF.Empty;
		float offset;
		public override Type PrintableObjectType {
			get { return typeof(TreeView); }
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(null),
		]
		public TreeView TreeView {
			get { return treeView; }
			set { treeView = value; }
		}
		public TreeViewLinkBase() : base() {
		}
		public TreeViewLinkBase(PrintingSystemBase ps) : base(ps) {
		}
		public override void SetDataObject(object data) {
			if(data is TreeView) 
				treeView = data as TreeView; 
		}
		protected override void BeforeCreate() {
			if(TreeView == null)
				throw new NullReferenceException("The TreeView property value must not be null");
			base.BeforeCreate();
			ps.Graph.PageUnit = GraphicsUnit.Pixel;
		}
		protected override void CreateDetail(BrickGraphics gr) {
			if(treeView.Nodes.Count == 0)
				return;
			treeView.ExpandAll();
			if(treeView.Nodes[0].Bounds.Y < 0)
				offset = -treeView.Nodes[0].Bounds.Y;
			CreateNodeBricks(gr, treeView.Nodes);
			treeView.CollapseAll();
		}
		protected virtual void OnNodeBrickDraw(Brick brick, TreeNode node, RectangleF rect) { }
		private void CreateNodeBricks(BrickGraphics gr, TreeNodeCollection nodes) {
			foreach(TreeNode node in nodes) {
				gr.DefaultBrickStyle.BackColor = (node.BackColor == Color.Empty) ? treeView.BackColor : node.BackColor;
				gr.DefaultBrickStyle.ForeColor = (node.ForeColor == Color.Empty) ? treeView.ForeColor : node.ForeColor;
				gr.DefaultBrickStyle.BorderWidth = 0;
				PanelBrick panelBrick = BrickFactory.CreateBrick(node);
				RectangleF rect = panelBrick.Rect;
				rect.Offset(0, offset);
				gr.DrawBrick(panelBrick, rect);
				OnNodeBrickDraw(panelBrick, node, rect);
				CreateNodeBricks(gr, node.Nodes);
			}
		}
	}
}
