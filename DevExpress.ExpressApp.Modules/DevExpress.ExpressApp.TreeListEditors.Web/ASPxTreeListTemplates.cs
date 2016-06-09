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

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using DevExpress.Web.ASPxTreeList;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.Web;
using DevExpress.Persistent.Base.General;
using System.Web.UI.WebControls;
namespace DevExpress.ExpressApp.TreeListEditors.Web {
	public class ASPxTreeListDataCellTemplateImageDecorator : ITemplate {
		private ITemplate decoratedTemplate;
		private WebImageCache imageCache;
		public ASPxTreeListDataCellTemplateImageDecorator(ITemplate decoratedTemplate, WebImageCache imageCache) {
			Guard.ArgumentNotNull(decoratedTemplate, "decoratedTemplate");
			this.decoratedTemplate = decoratedTemplate;
			this.imageCache = imageCache;
		}
		public ITemplate DecoratedTemplate {
			get {
				return decoratedTemplate;
			}
		}
		#region ITemplate Members
		public void InstantiateIn(Control container) {
			TreeListDataCellTemplateContainer treeListContainer = (TreeListDataCellTemplateContainer)container;
			decoratedTemplate.InstantiateIn(container);
			TreeListNode node = treeListContainer.TreeList.FindNodeByKeyValue(treeListContainer.NodeKey);
			ITreeNodeImageProvider imageProvider = node[ASPxTreeListEditor.RowObjectColumnName] as ITreeNodeImageProvider;
			if(imageProvider != null) {
				Table table = RenderHelper.CreateTable();
				table.CssClass = "XafTreeNode";
				TableRow tableRow = new TableRow();
				table.Rows.Add(tableRow);
				TableCell imageCell = new TableCell();
				imageCell.CssClass = "ImageCell";
				tableRow.Cells.Add(imageCell);
				imageCell.Visible = false;
				TableCell contentCell = new TableCell();
				contentCell.CssClass = "TextCell";
				tableRow.Cells.Add(contentCell);
				container.Controls.Add(table);
				Control imageCellControl = new LiteralControl(" ");
				string imageName;
				System.Drawing.Image image = imageProvider.GetImage(out imageName);
				if(image != null) {
					ImageInfo imageInfo = !string.IsNullOrEmpty(imageName) ? imageCache.GetImageInfo(image, imageName) : imageCache.GetImageInfo(image);
					ASPxImage imageControl = RenderHelper.CreateASPxImage();
					ASPxImageHelper.SetImageProperties(imageControl, imageInfo);
					imageCellControl = imageControl;
					imageCell.Visible = true;
				}
				imageCell.Controls.Add(imageCellControl);
				foreach(Control control in treeListContainer.Controls) {
					contentCell.Controls.Add(control);
				}
			}
		}
		#endregion
	}
}
