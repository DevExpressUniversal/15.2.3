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
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Web.Layout {
	public interface ISupportSizeConstraints {
		void ApplyConstraints(Size minSize, Size maxSize);
	}
	public class LayoutItemTemplate : LayoutBaseTemplate {
		protected override void InstantiateInCore(LayoutItemTemplateContainerBase container) {
			LayoutItemTemplateContainer templateContainer = (LayoutItemTemplateContainer)container;
			Control targetControl;
			Control placeholder = CreatePlaceholderControl(templateContainer.Model, templateContainer, out targetControl);
			ViewItem viewItem = templateContainer.ViewItem;
			Control control = CreateViewItemControl(viewItem);
			if(control != null) {
				if(targetControl is WebControl) {
					((WebControl)targetControl).CssClass += " " + GetParentControlCssClass(viewItem);
				}
				targetControl.Controls.Add(control);
			}
			else {
				Panel emptySpacePanel = new Panel();
				emptySpacePanel.CssClass = "EmptySpace";
				targetControl.Controls.Add(emptySpacePanel);
			}
			templateContainer.Controls.Add(placeholder);
		}
		private Control CreatePlaceholderControl(IModelLayoutViewItem layoutItemModel, LayoutItemTemplateContainer templateContainer, out Control targetControl) {
			Panel itemPanel = new Panel();
			SetCssClass(itemPanel, layoutItemModel);
			SetConstraints(itemPanel, layoutItemModel);
			SetConstraints(templateContainer.ViewItem, layoutItemModel);
			targetControl = itemPanel;
			if(templateContainer.ShowCaption) {
				Table layoutItemTable = CreateLayoutItemTable(layoutItemModel, templateContainer, out targetControl);
				itemPanel.Controls.Add(layoutItemTable);
			}
			return itemPanel;
		}
		private void SetCssClass(Panel itemPanel, IModelLayoutViewItem layoutItemModel) {
			itemPanel.CssClass = "Item";
			if(layoutItemModel is IModelViewLayoutElementWeb) {
				itemPanel.CssClass += " " + ((IModelViewLayoutElementWeb)layoutItemModel).CustomCSSClassName;
			}
		}
		private void SetConstraints(Panel itemPanel, IModelLayoutViewItem layoutItemModel) {
			if(layoutItemModel.SizeConstraintsType == XafSizeConstraintsType.Custom) {
				SetStyle(itemPanel, "min-width", layoutItemModel.MinSize.Width);
				SetStyle(itemPanel, "min-height", layoutItemModel.MinSize.Height);
				SetStyle(itemPanel, "max-width", layoutItemModel.MaxSize.Width);
				SetStyle(itemPanel, "max-height", layoutItemModel.MaxSize.Height);
			}
		}
		private void SetStyle(WebControl control, string style, int value) {
			if(value != 0) {
				control.Style[style] = value + "px";
			}
		}
		private void SetConstraints(ViewItem viewItem, IModelLayoutViewItem layoutItemModel) {
			if(layoutItemModel.SizeConstraintsType == XafSizeConstraintsType.Custom && viewItem is ISupportSizeConstraints) {
				((ISupportSizeConstraints)viewItem).ApplyConstraints(layoutItemModel.MinSize, layoutItemModel.MaxSize);
			}
		}
		private Table CreateLayoutItemTable(IModelLayoutViewItem layoutItemModel, LayoutItemTemplateContainer templateContainer, out Control targetControl) {
			Table layoutItemTable = RenderHelper.CreateTable();
			layoutItemTable.ID = WebIdHelper.GetCorrectedLayoutItemId(layoutItemModel, "", "_ctbl");
			layoutItemTable.Width = Unit.Percentage(100);
			TableCell captionCell = CreateCaptionCell(templateContainer);
			templateContainer.CaptionControl = captionCell;
			TableCell controlCell = CreateControlCell(templateContainer, out targetControl);
			templateContainer.LayoutItemControl = controlCell;
			SetupLayoutItemTable(layoutItemTable, captionCell, controlCell, templateContainer.CaptionLocation);
			return layoutItemTable;
		}
		protected virtual TableCell CreateCaptionCell(LayoutItemTemplateContainer templateContainer) {
			TableCell captionCell = new TableCell();
			((ISupportToolTip)this).SetToolTip(captionCell, templateContainer.Model);
			captionCell.CssClass = "Caption " + GetAlignmentCssClasses(templateContainer);
			captionCell.Width = templateContainer.CaptionWidth;
			Control captionControl = CreateCaptionControl(templateContainer);
			captionCell.Controls.Add(captionControl);
			return captionCell;
		}
		protected virtual string GetAlignmentCssClasses(LayoutItemTemplateContainer templateContainer) {
			string horizontalAlignmentClass = "";
			string verticalAlignmentClass = "";
			switch(templateContainer.CaptionVerticalAlignment) {
				case VertAlignment.Top:
					verticalAlignmentClass = "vaTop";
					break;
				case VertAlignment.Center:
					verticalAlignmentClass = "vaCenter";
					break;
				case VertAlignment.Bottom:
					verticalAlignmentClass = "vaBottom";
					break;
			}
			switch(templateContainer.CaptionHorizontalAlignment) {
				case HorzAlignment.Far:
					horizontalAlignmentClass = templateContainer.CaptionLocation == Locations.Right ? "haLeft" : "haRight";
					break;
				case HorzAlignment.Center:
					horizontalAlignmentClass = "haCenter";
					break;
				case HorzAlignment.Near:
					horizontalAlignmentClass = templateContainer.CaptionLocation == Locations.Right ? "haRight" : "haLeft";
					break;
			}
			return horizontalAlignmentClass + " " + verticalAlignmentClass;
		}
		protected virtual Control CreateCaptionControl(LayoutItemTemplateContainer templateContainer) {
			Literal label = new Literal();
			if(WebApplicationStyleManager.IsNewStyle && WebApplicationStyleManager.GroupUpperCase) {
				label.Text = templateContainer.Caption.ToUpper();
			}
			else {
				label.Text = templateContainer.Caption;
			}
			return label;
		}
		protected virtual TableCell CreateControlCell(LayoutItemTemplateContainer templateContainer, out Control targetControl) {
			TableCell result = new TableCell();
			targetControl = result;
			return result;
		}
		private void SetupLayoutItemTable(Table layoutItemTable, TableCell captionCell, TableCell controlCell, Locations captionLocation) {
			switch(captionLocation) {
				case Locations.Left:
				case Locations.Default:
					layoutItemTable.Rows.Add(new TableRow());
					layoutItemTable.Rows[0].Cells.Add(captionCell);
					layoutItemTable.Rows[0].Cells.Add(controlCell);
					break;
				case Locations.Right:
					layoutItemTable.Rows.Add(new TableRow());
					layoutItemTable.Rows[0].Cells.Add(controlCell);
					layoutItemTable.Rows[0].Cells.Add(captionCell);
					break;
				case Locations.Top:
					layoutItemTable.Rows.Add(new TableRow());
					layoutItemTable.Rows[0].Cells.Add(captionCell);
					layoutItemTable.Rows.Add(new TableRow());
					layoutItemTable.Rows[1].Cells.Add(controlCell);
					break;
				case Locations.Bottom:
					layoutItemTable.Rows.Add(new TableRow());
					layoutItemTable.Rows[0].Cells.Add(controlCell);
					layoutItemTable.Rows.Add(new TableRow());
					layoutItemTable.Rows[1].Cells.Add(captionCell);
					break;
			}
		}
		protected Control CreateViewItemControl(ViewItem viewItem) {
			if(viewItem == null) {
				return null;
			}
			CreateCustomControlEventArgs args = new CreateCustomControlEventArgs(viewItem);
			OnCreateCustomControl(args);
			if(args.Handled) {
				return args.Control;
			}
			if(viewItem.Control == null) {
				viewItem.CreateControl();
			}
			return (Control)viewItem.Control;
		}
		protected virtual void OnCreateCustomControl(CreateCustomControlEventArgs args) {
			if(CreateCustomControl != null) {
				CreateCustomControl(this, args);
			}
		}
		protected string GetParentControlCssClass(ViewItem viewItem) {
			object[] attributes = viewItem.Control.GetType().GetCustomAttributes(typeof(ParentControlCssClassAttribute), true);
			if(attributes.Length == 1) {
				return ((ParentControlCssClassAttribute)attributes[0]).ParentCssClass;
			}
			return "";
		}
		public LayoutItemTemplate() : this(new SimpleControlInstantiationStrategy()) { }
		public LayoutItemTemplate(ControlInstantiationStrategyBase controlInstantiationStrategy) : base(controlInstantiationStrategy) { }
		public event EventHandler<CreateCustomControlEventArgs> CreateCustomControl;
	}
	public class CreateCustomControlEventArgs : HandledEventArgs {
		private ViewItem viewItem;
		public CreateCustomControlEventArgs(ViewItem viewItem) {
			this.viewItem = viewItem;
		}
		public ViewItem ViewItem {
			get { return viewItem; }
		}
		public Control Control { get; set; }
	}
	public class ParentControlCssClassAttribute : Attribute {
		private string parentCssClass;
		public ParentControlCssClassAttribute(string parentCssClass) {
			this.parentCssClass = parentCssClass;
		}
		public string ParentCssClass {
			get { return parentCssClass; }
		}
	}
}
