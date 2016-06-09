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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Layout {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class FixedWidthViewItemAttribute : Attribute { }
	public class LayoutGroupTemplate : LayoutBaseTemplate, ILinkedToControl {
		private List<IDisposable> objectsToDispose = new List<IDisposable>();
		protected override void InstantiateInCore(LayoutItemTemplateContainerBase container) {
			LayoutGroupTemplateContainer layoutGroupTemplateContainer = (LayoutGroupTemplateContainer)container;
			if(layoutGroupTemplateContainer.Model.Direction == FlowDirection.Vertical) {
				ProcessGroupItemsVertically(layoutGroupTemplateContainer);
			}
			else {
				ProcessGroupItemsHorizontally(layoutGroupTemplateContainer);
			}
		}
		protected virtual void ProcessGroupItemsVertically(LayoutGroupTemplateContainer templateContainer) {
			List<Control> controlsToLayout = new List<Control>(templateContainer.Items.Values);
			LayoutContentControls(templateContainer, controlsToLayout);
		}
		protected virtual void ProcessGroupItemsHorizontally(LayoutGroupTemplateContainer templateContainer) {
			if(WebApplicationStyleManager.IsNewStyle && ((IModelViewLayoutElementWeb)templateContainer.Model).Adaptivity) {
				ProcessGroupItemsHorizontallyNewStyle(templateContainer);
			}
			else {
				ProcessGroupItemsHorizontallyOldStyle(templateContainer);
			}
		}
		protected virtual void ProcessGroupItemsHorizontallyNewStyle(LayoutGroupTemplateContainer templateContainer) {
			IModelLayoutGroup info = templateContainer.Model;
			if(info.Count < 2) {
				ProcessGroupItemsVertically(templateContainer);
			}
			else {
				Table table = RenderHelper.CreateTable();
				table.ID = WebIdHelper.GetCorrectedLayoutItemId(info);
				table.CssClass = GroupLayoutCSSInfo.GetGroupContentCssClassName(info);
				SetCustomCSSClass(templateContainer.Model, table);
				TableRow row = new TableRow();
				row.VerticalAlign = VerticalAlign.Top;
				table.Rows.Add(row);
				TableCell itemCell = new TableCell();
				table.Rows[0].Cells.Add(itemCell);
				Dictionary<IModelViewLayoutElement, WebControl> layoutControls = new Dictionary<IModelViewLayoutElement, WebControl>();
				foreach(LayoutItemTemplateContainerBase innerItem in templateContainer.Items.Values) {
					ASPxPanel groupPanel = new ASPxPanel();
					layoutControls.Add(innerItem.Model, groupPanel);
					groupPanel.ID = WebIdHelper.GetCorrectedLayoutItemId(innerItem.Model);
					itemCell.Controls.Add(groupPanel);
					groupPanel.Controls.Add(innerItem);
				}
				SetupLayoutControls(info, layoutControls);
				List<Control> controlsToLayout = new List<Control>();
				controlsToLayout.Add(table);
				LayoutContentControls(templateContainer, controlsToLayout);
			}
		}
		private void SetupLayoutControls(IModelLayoutGroup parentGroup, Dictionary<IModelViewLayoutElement, WebControl> layoutControls) {
			SetCss(parentGroup, layoutControls);
			SetWidth(parentGroup, layoutControls);
		}
		private void SetCss(IModelLayoutGroup parentGroup, Dictionary<IModelViewLayoutElement, WebControl> layoutControls) {
			Dictionary<IModelViewLayoutElement, LayoutCSSInfo> result = LayoutCSSCalculator.CalcCss(parentGroup);
			foreach(KeyValuePair<IModelViewLayoutElement, WebControl> item in layoutControls) {
				LayoutCSSInfo layoutCSSInfo;
				if(result.TryGetValue(item.Key, out layoutCSSInfo)) {
					if(layoutCSSInfo.CardItem && layoutCSSInfo.ParentDirection == FlowDirection.Horizontal) {
						item.Value.CssClass = layoutCSSInfo.CardCssClassNameCore;
					}
					else {
						item.Value.CssClass = layoutCSSInfo.EditorContainerCssClassName;
					}
				}
				SetCustomCSSClass(item.Key, item.Value);
			}
		}
		private void SetWidth(IModelLayoutGroup parentGroup, Dictionary<IModelViewLayoutElement, WebControl> layoutControls) {
			Dictionary<IModelViewLayoutElement, Unit> result = LayoutWidthCalculator.CalcWidth(parentGroup);
			foreach(KeyValuePair<IModelViewLayoutElement, WebControl> item in layoutControls) {
				Unit width;
				if(result.TryGetValue(item.Key, out width)) {
					item.Value.Width = width;
				}
			}
		}
		protected virtual void ProcessGroupItemsHorizontallyOldStyle(LayoutGroupTemplateContainer templateContainer) {
			IModelLayoutGroup info = templateContainer.Model;
			if(info.Count < 2) {
				ProcessGroupItemsVertically(templateContainer);
			}
			else {
				double multiplier;
				double zeroReplacer;
				SolveMultiplierAndZeroReplacer(info, out multiplier, out zeroReplacer);
				Table table = RenderHelper.CreateTable();
				table.CssClass = GroupLayoutCSSInfo.GetGroupContentCssClassName(info);
				table.Rows.Add(new TableRow());
				TableRow row = new TableRow();
				row.VerticalAlign = VerticalAlign.Top;
				table.Rows.Add(row);
				Dictionary<IModelViewLayoutElement, WebControl> layoutControls = new Dictionary<IModelViewLayoutElement, WebControl>();
				foreach(LayoutItemTemplateContainerBase innerItem in templateContainer.Items.Values) {
					TableCell itemCell = new TableCell();
					itemCell.CssClass = "HItem";
					if(table.Rows[0].Cells.Count == 0) {
						itemCell.CssClass = "HItem FirstColumn";
					}
					else {
						itemCell.CssClass = "HItem NextColumn";
					}
					if(innerItem.ViewItem != null || innerItem is LayoutGroupTemplateContainerBase) {
						double relativeSize = innerItem.Model.RelativeSize * multiplier;
						if(relativeSize == 0.0) {
							relativeSize = zeroReplacer;
						}
						if(!IsFixedWidthItem(innerItem)) {
							itemCell.Width = Unit.Percentage(relativeSize);
						}
						else {
							itemCell.Width = Unit.Percentage(1);
						}
					}
					table.Rows[0].Cells.Add(itemCell);
					itemCell.Controls.Add(innerItem);
					layoutControls.Add(innerItem.Model, itemCell);
					if(innerItem is LayoutGroupTemplateContainerBase) {
						itemCell.CssClass += " " + LayoutCSSInfo.GroupContentCssClassName;
					}
				}
				if(WebApplicationStyleManager.IsNewStyle) {
					SetupLayoutControls(info, layoutControls);
				}
				List<Control> controlsToLayout = new List<Control>();
				controlsToLayout.Add(table);
				LayoutContentControls(templateContainer, controlsToLayout);
			}
		}
		protected virtual void SolveMultiplierAndZeroReplacer(IModelLayoutGroup layoutingInfo, out double multiplier, out double zeroReplacer) {
			int totalSize = 0;
			int zerosCount = 0;
			foreach(IModelViewLayoutElement itemInfo in layoutingInfo) {
				int relativeSize = (int)itemInfo.RelativeSize;
				if(relativeSize == 0) {
					zerosCount++;
				}
				totalSize += relativeSize;
			}
			multiplier = 1;
			zeroReplacer = 0;
			if(totalSize > 100) {
				multiplier = 100.0 / totalSize;
			}
			else if(zerosCount > 0) {
				zeroReplacer = (100.0 - totalSize) / zerosCount;
			}
		}
		protected virtual bool IsFixedWidthItem(LayoutItemTemplateContainerBase innerItem) {
			bool result = false;
			if(innerItem is LayoutGroupTemplateContainerBase) {
				result = true;
				LayoutGroupTemplateContainerBase layoutGroupTemplateContainerBase = (LayoutGroupTemplateContainerBase)innerItem;
				foreach(LayoutItemTemplateContainerBase childItem in layoutGroupTemplateContainerBase.Items.Values) {
					if(!IsFixedWidthItem(childItem)) {
						result = false;
						break;
					}
				}
			}
			else if(innerItem is LayoutItemTemplateContainer) {
				LayoutItemTemplateContainer layoutItemTemplateContainer = (LayoutItemTemplateContainer)innerItem;
				if(layoutItemTemplateContainer.ViewItem != null) {
					ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(layoutItemTemplateContainer.ViewItem.GetType());
					result = typeInfo.FindAttribute<FixedWidthViewItemAttribute>() != null;
				}
			}
			return result;
		}
		protected virtual void LayoutContentControls(LayoutGroupTemplateContainer templateContainer, IList<Control> controlsToLayout) {
			Control headerContainer = templateContainer;
			Control contentContainer = templateContainer;
			if(WebApplicationStyleManager.IsNewStyle) {
				LayoutCSSInfo layoutCSSInfo = LayoutCSSCalculator.GetLayoutCSSInfo(templateContainer.Model);
				if(layoutCSSInfo != null && layoutCSSInfo.CardItem) {
					if(((IModelLayoutGroupWeb)templateContainer.Model).IsCollapsibleCardGroup) {
						contentContainer = CreateCollapsibleCardGroup(templateContainer, layoutCSSInfo);
					}
					else {
						contentContainer = CreateCardGroup(templateContainer, ref headerContainer, layoutCSSInfo);
					}
				}
				else {
					WebControl header = CreateLayoutContentHeader(templateContainer);
					if(header != null) {
						headerContainer.Controls.Add(header);
					}
				}
			}
			else {
				WebControl header = CreateLayoutContentHeader(templateContainer);
				if(header != null) {
					headerContainer.Controls.Add(header);
				}
			}
			foreach(Control control in controlsToLayout) {
				contentContainer.Controls.Add(control);
			}
		}
		private Control CreateCollapsibleCardGroup(LayoutGroupTemplateContainer templateContainer, LayoutCSSInfo layoutCSSInfo) {
			ASPxRoundPanel cardPanel = new ASPxRoundPanel();
			cardPanel.ID = WebIdHelper.GetCorrectedLayoutItemId(templateContainer.Model, "", "_CardTable");
			cardPanel.BorderWidth = Unit.Empty;
			if(layoutCSSInfo.ParentDirection == FlowDirection.Horizontal) {
				cardPanel.CssClass = layoutCSSInfo.EditorContainerCssClassName;
			}
			else {
				cardPanel.CssClass = layoutCSSInfo.CardCssClassNameCore;
			}
			SetCustomCSSClass(templateContainer.Model, cardPanel);
			WebControl cardGroupContent = new WebControl(HtmlTextWriterTag.Div);
			cardGroupContent.CssClass = ((GroupLayoutCSSInfo)layoutCSSInfo).CardGroupContentCssClass;
			cardPanel.Controls.Add(cardGroupContent);
			templateContainer.Controls.Add(cardPanel);
			if(templateContainer.ShowCaption && (!templateContainer.IsOnTabPage || templateContainer.ParentGroupDirection == FlowDirection.Vertical)) {
				cardPanel.AllowCollapsingByHeaderClick = false;
				cardPanel.ShowCollapseButton = true;
				cardPanel.HeaderStyle.CssClass = "GroupHeader Label";
				cardPanel.HeaderStyle.Width = Unit.Percentage(100);
				if(templateContainer.HasHeaderImage) {
					cardPanel.HeaderImage.AlternateText = templateContainer.Caption;
					ASPxImageHelper.SetImageProperties(cardPanel.HeaderImage, templateContainer.HeaderImageInfo);
				}
				if(WebApplicationStyleManager.GroupUpperCase) {
					cardPanel.HeaderText = templateContainer.Caption.ToUpper();
				}
				else {
					cardPanel.HeaderText = templateContainer.Caption;
				}
				((ISupportToolTip)this).SetToolTip(cardPanel, templateContainer.Model);
			}
			else {
				cardPanel.ShowHeader = false;
			}
			return cardGroupContent;
		}
		private Control CreateCardGroup(LayoutGroupTemplateContainer templateContainer, ref Control headerContainer, LayoutCSSInfo layoutCSSInfo) {
			Table cardTable = RenderHelper.CreateTable();
			cardTable.BorderWidth = Unit.Empty;
			if(layoutCSSInfo.ParentDirection == FlowDirection.Horizontal) {
				cardTable.CssClass = layoutCSSInfo.EditorContainerCssClassName;
			}
			else {
				cardTable.CssClass = layoutCSSInfo.CardCssClassNameCore;
			}
			SetCustomCSSClass(templateContainer.Model, cardTable);
			cardTable.ID = WebIdHelper.GetCorrectedLayoutItemId(templateContainer.Model, "", "_CardTable");
			TableRow headerRow = new TableRow();
			headerRow.VerticalAlign = VerticalAlign.Top;
			cardTable.Rows.Add(headerRow);
			TableRow contentRow = new TableRow();
			contentRow.VerticalAlign = VerticalAlign.Top;
			cardTable.Rows.Add(contentRow);
			TableCell headerCell = new TableCell();
			headerRow.Cells.Add(headerCell);
			TableCell contentCell = new TableCell();
			contentCell.CssClass = ((GroupLayoutCSSInfo)layoutCSSInfo).CardGroupContentCssClass;
			contentRow.Cells.Add(contentCell);
			templateContainer.Controls.Add(cardTable);
			headerContainer = headerCell;
			WebControl header = CreateLayoutContentHeader(templateContainer);
			if(header != null) {
				headerContainer.Controls.Add(header);
			}
			return contentCell;
		}
		private void SetCustomCSSClass(IModelNode model, WebControl control) {
			if(model is IModelViewLayoutElementWeb) {
				string customClass = ((IModelViewLayoutElementWeb)model).CustomCSSClassName;
				if(!string.IsNullOrEmpty(customClass)) {
					control.CssClass += " " + customClass;
				}
			}
		}
		private WebControl CreateLayoutContentHeader(LayoutGroupTemplateContainer templateContainer) {
			WebControl div = null;
			if(templateContainer.ShowCaption && (!templateContainer.IsOnTabPage || templateContainer.ParentGroupDirection == FlowDirection.Vertical)) {
				div = new WebControl(HtmlTextWriterTag.Div);
				div.CssClass = "GroupHeader";
				div.Width = Unit.Percentage(100);
				if(templateContainer.HasHeaderImage) {
					ASPxImage imageHeader = new ASPxImage();
					imageHeader.CssClass = "Image";
					ASPxImageHelper.SetImageProperties(imageHeader, templateContainer.HeaderImageInfo);
					imageHeader.AlternateText = templateContainer.Caption;
					div.Controls.Add(imageHeader);
				}
				Label label = new Label();
				if(WebApplicationStyleManager.IsNewStyle && WebApplicationStyleManager.GroupUpperCase) {
					label.Text = templateContainer.Caption.ToUpper();
				}
				else {
					label.Text = templateContainer.Caption;
				}
				((ISupportToolTip)this).SetToolTip(label, templateContainer.Model);
				templateContainer.CaptionControl = label;
				label.CssClass = "Label";
				div.Controls.Add(label);
				objectsToDispose.Add(new LayoutGroupTemplateContainerVisibilitySynchronizer(templateContainer, div));
			}
			return div;
		}
		public LayoutGroupTemplate() : this(new SimpleControlInstantiationStrategy()) { }
		public LayoutGroupTemplate(ControlInstantiationStrategyBase controlInstantiationStrategy) : base(controlInstantiationStrategy) { }
		#region ILinkedToControl Members
		public void BreakLinksToControl() {
			foreach(IDisposable disposable in objectsToDispose) {
				disposable.Dispose();
			}
			objectsToDispose.Clear();
		}
		#endregion
	}
}
