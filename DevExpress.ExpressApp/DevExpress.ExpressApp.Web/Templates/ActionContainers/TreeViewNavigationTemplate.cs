#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.ExpressApp.Web.Templates {
	public class TreeViewNavigationTemplate : ITemplate {
		public const string
			SampleExpandButtonID = "SEB",
			SampleCollapseButtonID = "SCB",
			SampleNodeLoadingPanelID = "SNLP",
			CollapseButtonImageName = "tvColBtn",
			ExpandButtonImageName = "tvExpBtn",
			ButtonImageCssClassName = "dxtv-btn",
			NodeLoadingPanelImageName = "tvNodeLoading",
			NodeExpandedCssClassName = "dxtv-ndExp",
			ExpandButtonCssClassName = "dxtv-xafNavBt",
			NodeTableCssClassName = "xav-nav-dxtv-con-tb",
			NodeRowCssClassName = "xav-nav-dxtv-con-row",
			NodeImageCssClassName = "xav-nav-dxtv-ndImg",
			NodeImageCellCssClassName = "dxtv-textImgCell",
			NodeContentCellCssClassName = "dxtv-textConCell",
			NodeButtonCellCssClassName = "dxtv-textButtCell";
		public void InstantiateIn(Control container) {
			TreeViewNodeTemplateContainer nodeContainer = (TreeViewNodeTemplateContainer)container;
			Table table = RenderHelper.CreateTable();
			table.CssClass = NodeTableCssClassName;
			int padding = 8 + NodeDepth(nodeContainer.Node) * 20;
			table.Style.Add("padding-left", padding.ToString() + "px");
			TableRow contentRow = new TableRow();
			contentRow.CssClass = NodeRowCssClassName;
			contentRow.VerticalAlign = VerticalAlign.Middle;
			table.Rows.Add(contentRow);
			TableCell imageCell = CreateImageCell(nodeContainer);
			if(imageCell != null) {
				contentRow.Cells.Add(imageCell);
			}
			contentRow.Cells.Add(CreateTextCell(nodeContainer));
			TableCell buttonCell = CreateExpandButtonCell(nodeContainer);
			if(buttonCell != null) {
				contentRow.Cells.Add(buttonCell);
			}
			nodeContainer.Controls.Add(table);
		}
		protected virtual TableCell CreateImageCell(TreeViewNodeTemplateContainer nodeContainer) {
			if(nodeContainer.Node.Image.IsEmpty) {
				return null;
			}
			TableCell imageCell = new TableCell();
			imageCell.CssClass = NodeImageCellCssClassName;
			Image image = RenderUtils.CreateImage();
			image.CssClass = NodeImageCssClassName;
			nodeContainer.Node.Image.AssignToControl(image, false);
			imageCell.Controls.Add(image);
			return imageCell;
		}
		protected virtual TableCell CreateTextCell(TreeViewNodeTemplateContainer nodeContainer) {
			TableCell itemCell = new TableCell();
			itemCell.CssClass = NodeContentCellCssClassName;
			WebControl textSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			string strNodeText = RenderUtils.ProtectTextWhitespaces(nodeContainer.Node.TreeView.HtmlEncode(string.Format(nodeContainer.Node.TreeView.TextFormatString, nodeContainer.Node.Text)));
			LiteralControl nodeText = RenderUtils.CreateLiteralControl(strNodeText);
			textSpan.Controls.Add(nodeText);
			itemCell.Controls.Add(textSpan);
			return itemCell;
		}
		protected virtual TableCell CreateExpandButtonCell(TreeViewNodeTemplateContainer nodeContainer) {
			TableCell itemCell = null;
			if(nodeContainer.Node.Nodes.GetVisibleItemCount() > 0) {
				itemCell = new TableCell();
				itemCell.CssClass = NodeButtonCellCssClassName;
				WebControl expandButton = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				expandButton.CssClass = ExpandButtonCssClassName;
				Image expandImage = RenderUtils.CreateImage();
				expandImage.CssClass = ButtonImageCssClassName;
				expandButton.Controls.Add(expandImage);
				string imageName = nodeContainer.Node.Expanded ? CollapseButtonImageName : ExpandButtonImageName;
				ImageProperties imageProperties = nodeContainer.Node.TreeView.Images.GetImageProperties(nodeContainer.Page, imageName);
				imageProperties.AssignToControl(expandImage, false);
				itemCell.Controls.Add(expandButton);
			}
			return itemCell;
		}
		public void CustomizeTreeView(ASPxTreeView treeView) {
			CreateSampleImages(treeView);
		}
		protected virtual void CreateSampleImages(ASPxTreeView treeView) {
			Image collapseButtonImage = RenderUtils.CreateImage();
			collapseButtonImage.ID = TreeViewNavigationTemplate.SampleCollapseButtonID;
			Image expandButtonImage = RenderUtils.CreateImage();
			expandButtonImage.ID = TreeViewNavigationTemplate.SampleExpandButtonID;
			PrepareExpandSampleImage(expandButtonImage, TreeViewNavigationTemplate.ExpandButtonImageName, treeView);
			PrepareExpandSampleImage(collapseButtonImage, TreeViewNavigationTemplate.CollapseButtonImageName, treeView);
			Image loadingPanelImage = RenderUtils.CreateImage();
			loadingPanelImage.ID = TreeViewNavigationTemplate.SampleNodeLoadingPanelID;
			PrepareLoadingPanelImage(loadingPanelImage, TreeViewNavigationTemplate.NodeLoadingPanelImageName, treeView);
			treeView.Controls.Add(collapseButtonImage);
			treeView.Controls.Add(expandButtonImage);
			treeView.Controls.Add(loadingPanelImage);
		}
		protected void PrepareExpandSampleImage(Image image, string imageName, ASPxTreeView view) {
			if(image == null)
				return;
			image.CssClass = ButtonImageCssClassName;
			ImageProperties imageProperties = view.Images.GetImageProperties(view.Page, imageName);
			imageProperties.AssignToControl(image, false);
			RenderUtils.SetVisibility(image, false, true);
		}
		protected void PrepareLoadingPanelImage(Image image, string imageName, ASPxTreeView view) {
			if(image == null)
				return;
			ImageProperties imageProperties = view.Images.GetImageProperties(view.Page, imageName);
			imageProperties.AssignToControl(image, false);
			RenderUtils.SetVisibility(image, false, false, true);
			RenderUtils.SetStyleStringAttribute(image, "poision", "absolute");
		}
		protected int NodeDepth(TreeViewNode node) {
			if(node != null)
				return node.Parent == null ? -1 : NodeDepth(node.Parent) + 1;
			return -1;
		}
	}
}
