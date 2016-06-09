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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.SystemModule;
namespace DevExpress.ExpressApp.Web.Layout {
	internal class RootLayoutCSSInfo : LayoutCSSInfo {
		public const string RootCssClassName = "CardGroupContent";
		public RootLayoutCSSInfo(int containerIndex, bool cardItem, int neighborCount, FlowDirection parentDirection)
			: base(containerIndex, cardItem, neighborCount, parentDirection) {
		}
		protected override string RootCssClass {
			get { return RootCssClassName; }
		}
		protected override string CardCssClass {
			get { return LayoutCSSInfo.CardCssClassName; }
		}
	}
	internal class LayoutItemCSSInfo : LayoutCSSInfo {
		bool adaptivity;
		public LayoutItemCSSInfo(int containerIndex, bool cardItem, int neighborCount, FlowDirection parentDirection, bool adaptivity)
			: base(containerIndex, cardItem, neighborCount, parentDirection) {
			this.adaptivity = adaptivity;
		}
		protected override string RootCssClass {
			get { return GetGroupContentCssClassName(adaptivity); }
		}
		protected override string CardCssClass {
			get { return LayoutCSSInfo.CardCssClassName; }
		}
		protected override string AdaptivityEditorContainerCssClass {
			get {
				return adaptivity ? base.AdaptivityEditorContainerCssClass : FixedEditorContainerCssClassName;
			}
		}
		public static string GetGroupContentCssClassName(IModelLayoutGroup model) {
			return GetGroupContentCssClassName(((IModelViewLayoutElementWeb)model).Adaptivity);
		}
		public static string GetGroupContentCssClassName(bool adaptivity) {
			return adaptivity ? GroupContentCssClassName + " " + AdaptivityCssSelector : GroupContentCssClassName;
		}
	}
	internal class GroupLayoutCSSInfo : LayoutItemCSSInfo {
		public const string CardGroupContentCssClassName = "CardGroupContent";
		public const string NoPaddingsCardGroupContentCssClassName = "CardGroupContent cgc-np";
		bool allowPaddings;
		public GroupLayoutCSSInfo(int containerIndex, bool cardItem, int neighborCount, FlowDirection parentDirection, bool adaptivity, bool allowPaddings)
			: base(containerIndex, cardItem, neighborCount, parentDirection, adaptivity) {
			this.allowPaddings = allowPaddings;
		}
		public string CardGroupContentCssClass {
			get {
				return allowPaddings ? CardGroupContentCssClassName : NoPaddingsCardGroupContentCssClassName;
			}
		}
	}
	internal class TabbedGroupLayoutCSSInfo : LayoutCSSInfo {
		public TabbedGroupLayoutCSSInfo(int containerIndex, bool cardItem, int neighborCount, FlowDirection parentDirection)
			: base(containerIndex, cardItem, neighborCount, parentDirection) {
		}
		protected override string RootCssClass {
			get { return LayoutTabbedGroupContainerCssClassName; }
		}
		protected override string CardCssClass {
			get { return ""; }
		}
	}
	internal abstract class LayoutCSSInfo {
		public const string CardCssClassName = "CardGroupBase ";
		public const string AdaptivityEditorContainerCssClassName = "AdaptivityEditorContainer";
		public const string FixedEditorContainerCssClassName = "FixedEditorContainer";
		public const string GroupContentCssClassName = "GroupContent";
		public const string FirstVerticalCardGroupCssClassName = "firstVerticalCardGroup";
		public const string SecondVerticalCardGroupCssClassName = "secondVerticalCardGroup";
		public const string LastVerticalCardGroupCssClassName = "lastVerticalCardGroup";
		public const string FirstCardGroupCssClassName = "firstCardGroup";
		public const string SecondCardGroupCssClassName = "secondCardGroup";
		public const string LastCardGroupCssClassName = "lastCardGroup";
		public const string FirstEditorContainerCssClassName = "firstEditorContainer";
		public const string SecondEditorContainerCssClassName = "secondEditorContainer";
		public const string LastEditorContainerCssClassName = "lastEditorContainer";
		public const string AdaptivityCssSelector = "Adaptivity";
		public const string SecondHorizontalEditorContainerCssClassName = "secondHorizontalEditorContainer";
		public const string LayoutTabbedGroupContainerCssClassName = "LayoutTabbedGroupContainer";
		private string cardCssClassNameCore = null;
		private string editorContainerCssClassName = null;
		private FlowDirection parentDirection;
		private int containerIndex;
		private int neighborCount;
		private bool cardItem;
		public LayoutCSSInfo(int containerIndex, bool cardItem, int neighborCount, FlowDirection parentDirection) {
			Initialize(containerIndex, cardItem, neighborCount, parentDirection);
		}
		protected abstract string RootCssClass {
			get;
		}
		protected abstract string CardCssClass {
			get;
		}
		protected virtual string AdaptivityEditorContainerCssClass {
			get {
				return AdaptivityEditorContainerCssClassName;
			}
		}
		private void Initialize(int containerIndex, bool cardItem, int neighborCount, ExpressApp.Layout.FlowDirection parentDirection) {
			this.cardItem = cardItem;
			this.containerIndex = containerIndex;
			this.neighborCount = neighborCount;
			this.parentDirection = parentDirection;
		}
		private void CalcCssClassName() {
			editorContainerCssClassName = RootCssClass + " " + AdaptivityEditorContainerCssClass;
			if(cardItem) {
				cardCssClassNameCore = editorContainerCssClassName + " " + CardCssClass;
			}
			if(neighborCount == 1) {
				if(cardItem) {
					cardCssClassNameCore += parentDirection == FlowDirection.Horizontal ? SecondCardGroupCssClassName : SecondVerticalCardGroupCssClassName;
					if(parentDirection == FlowDirection.Horizontal) {
						cardCssClassNameCore += " " + SecondEditorContainerCssClassName;
					}
				}
				else {
					editorContainerCssClassName += " " + SecondEditorContainerCssClassName;
					if(parentDirection == FlowDirection.Horizontal) {
						editorContainerCssClassName += " " + SecondHorizontalEditorContainerCssClassName;
					}
				}
			}
			else {
				if(containerIndex == neighborCount) {
					if(cardItem) {
						cardCssClassNameCore += parentDirection == FlowDirection.Horizontal ? LastCardGroupCssClassName : LastVerticalCardGroupCssClassName;
						if(parentDirection == FlowDirection.Horizontal) {
							cardCssClassNameCore += " " + LastEditorContainerCssClassName;
						}
					}
					else {
						editorContainerCssClassName += " " + LastEditorContainerCssClassName;
					}
				}
				else {
					if(containerIndex == 1) {
						if(cardItem) {
							cardCssClassNameCore += parentDirection == FlowDirection.Horizontal ? FirstCardGroupCssClassName : FirstVerticalCardGroupCssClassName;
							if(parentDirection == FlowDirection.Horizontal) {
								cardCssClassNameCore += " " + FirstEditorContainerCssClassName;
							}
						}
						else {
							editorContainerCssClassName += " " + FirstEditorContainerCssClassName;
						}
					}
					else {
						if(cardItem) {
							cardCssClassNameCore += parentDirection == FlowDirection.Horizontal ? SecondCardGroupCssClassName : SecondVerticalCardGroupCssClassName;
							if(parentDirection == FlowDirection.Horizontal) {
								cardCssClassNameCore += " " + SecondEditorContainerCssClassName;
							}
						}
						else {
							editorContainerCssClassName += " " + SecondEditorContainerCssClassName;
							if(parentDirection == FlowDirection.Horizontal) {
								editorContainerCssClassName += " " + SecondHorizontalEditorContainerCssClassName;
							}
						}
					}
				}
			}
		}
		public string EditorContainerCssClassName {
			get {
				if(editorContainerCssClassName == null) {
					CalcCssClassName();
				}
				return editorContainerCssClassName;
			}
		}
		public string CardCssClassNameCore {
			get {
				if(cardCssClassNameCore == null) {
					CalcCssClassName();
				}
				return cardCssClassNameCore;
			}
		}
		public bool CardItem {
			get {
				return cardItem;
			}
		}
		public FlowDirection ParentDirection {
			get { return parentDirection; }
		}
	}
	internal class LayoutCSSCalculator {
		public static Dictionary<IModelViewLayoutElement, LayoutCSSInfo> CalcCss(IModelViewLayoutElement rootLayoutGroup) {
			return CalcCss(GetModelLayoutNode(rootLayoutGroup));
		}
		public static Dictionary<IModelViewLayoutElement, LayoutCSSInfo> CalcCss(IModelViewLayout layoutModel) {
			Dictionary<IModelViewLayoutElement, LayoutCSSInfo> result = new Dictionary<IModelViewLayoutElement, LayoutCSSInfo>();
			new GroupInfo(layoutModel, result);
			return result;
		}
		public static LayoutCSSInfo GetLayoutCSSInfo(IModelViewLayout layoutModel) {
			bool cardItem = true;
			foreach(IModelNode item in layoutModel) {
				if(item is IModelLayoutGroup || item is IModelTabbedGroup) {
					cardItem = false;
					break;
				}
			}
			return new RootLayoutCSSInfo(0, cardItem, 0, FlowDirection.Vertical);
		}
		public static LayoutCSSInfo GetLayoutCSSInfo(IModelViewLayoutElement model) {
			if(model != null) {
				Dictionary<IModelViewLayoutElement, LayoutCSSInfo> result = CalcCss(model);
				LayoutCSSInfo layoutCSSInfo;
				if(result.TryGetValue(model, out layoutCSSInfo)) {
					return layoutCSSInfo;
				}
			}
			return null;
		}
		internal static IModelViewLayout GetModelLayoutNode(IModelViewLayoutElement rootLayoutGroup) {
			IModelViewLayout result = null;
			IModelNode parent = rootLayoutGroup.Parent;
			do {
				if(parent != null && !(parent is IModelViewLayout)) {
					parent = parent.Parent;
				}
			} while(parent != null && !(parent is IModelViewLayout));
			result = parent as IModelViewLayout;
			return result;
		}
		private class GroupInfo {
			public GroupInfo(IModelViewLayout layoutModel, Dictionary<IModelViewLayoutElement, LayoutCSSInfo> childrenGroupsStyles) {
				if(layoutModel != null) {
					Calc(layoutModel, childrenGroupsStyles);
				}
			}
			private GroupInfo(IEnumerable group, Dictionary<IModelViewLayoutElement, LayoutCSSInfo> childrenGroupsStyles, int groupIndex, int groupsOnLevel, FlowDirection parentDirection) {
				if(group != null) {
					Calc(group, childrenGroupsStyles, groupIndex, groupsOnLevel, parentDirection);
				}
			}
			private void Calc(IModelViewLayout layoutModel, Dictionary<IModelViewLayoutElement, LayoutCSSInfo> childrenGroupsStyles) {
				Calc(layoutModel, childrenGroupsStyles, -1, -1, FlowDirection.Vertical);
			}
			private void Calc(IEnumerable parentModel, Dictionary<IModelViewLayoutElement, LayoutCSSInfo> childrenGroupsStyles, int groupIndex, int groupsOnLevel, FlowDirection parentDirection) {
				List<IModelViewLayoutElement> childrenGroups = new List<IModelViewLayoutElement>();
				foreach(IModelViewLayoutElement item in parentModel) {
					childrenGroups.Add((IModelViewLayoutElement)item);
				}
				if(parentModel is IModelLayoutGroup || parentModel is IModelTabbedGroup) {
					if(parentModel is IModelLayoutGroup) {
						childrenGroupsStyles.Add((IModelViewLayoutElement)parentModel, new GroupLayoutCSSInfo(groupIndex, ((IModelViewLayoutElementWeb)parentModel).IsCardGroup, groupsOnLevel, parentDirection, ((IModelViewLayoutElementWeb)parentModel).Adaptivity, AllowPaddings((IModelLayoutGroup)parentModel)));
					}
					else {
						childrenGroupsStyles.Add((IModelViewLayoutElement)parentModel, new TabbedGroupLayoutCSSInfo(groupIndex, ((IModelViewLayoutElementWeb)parentModel).IsCardGroup, groupsOnLevel, parentDirection));
					}
				}
				CalcChildrenGroups(parentModel, childrenGroupsStyles, childrenGroups);
			}
			private bool AllowPaddings(IModelLayoutGroup model) {
				if(model.NodeCount == 1 && model[0] is IModelLayoutViewItem) {
					IModelPropertyEditor viewItem = ((IModelLayoutViewItem)model[0]).ViewItem as IModelPropertyEditor;
					if(viewItem != null && typeof(ListPropertyEditor).IsAssignableFrom(viewItem.PropertyEditorType)) {
						return false;
					}
				}
				return true;
			}
			private void CalcChildrenGroups(IEnumerable parentModel, Dictionary<IModelViewLayoutElement, LayoutCSSInfo> childrenGroupsStyles, List<IModelViewLayoutElement> childrenGroups) {
				if(childrenGroups.Count > 0) {
					FlowDirection parentDirection = ParetnGroupDirection(parentModel);
					int groupsOnLevel = childrenGroups.Count;
					int containerIndex = 0;
					foreach(IModelViewLayoutElement item in childrenGroups) {
						containerIndex++;
						if(item is IEnumerable) {
							GroupInfo innerGoupInfo = new GroupInfo((IEnumerable)item, childrenGroupsStyles, containerIndex, groupsOnLevel, parentDirection);
						}
						else {
							childrenGroupsStyles.Add(item, new LayoutItemCSSInfo(containerIndex, ((IModelViewLayoutElementWeb)item).IsCardGroup, groupsOnLevel, parentDirection, ((IModelViewLayoutElementWeb)item).Adaptivity));
						}
					}
				}
			}
			private FlowDirection ParetnGroupDirection(IEnumerable parentModel) {
				if(parentModel is IModelLayoutGroup) {
					return ((IModelLayoutGroup)parentModel).Direction;
				}
				else {
					if(parentModel is IModelTabbedGroup) {
						return ((IModelTabbedGroup)parentModel).Direction;
					}
				}
				return FlowDirection.Vertical;
			}
		}
	}
	internal class LayoutWidthCalculator {
		public static Dictionary<IModelViewLayoutElement, Unit> CalcWidth(IModelLayoutGroup rootLayoutGroup) {
			Dictionary<IModelViewLayoutElement, Unit> result = new Dictionary<IModelViewLayoutElement, Unit>();
			new GroupInfo(rootLayoutGroup, result);
			return result;
		}
		interface IGroupInfo {
			double Width { get; }
			int ItemsCount { get; }
			FlowDirection Direction { get; }
			IModelViewLayoutElement Model { get; }
		}
		private class FakeGroupInfo : IGroupInfo {
			private Double maxWidth = 0;
			private IModelViewLayoutElement model;
			public FakeGroupInfo(IModelViewLayoutElement item) {
				model = item;
				maxWidth = item.RelativeSize;
			}
			public double Width {
				get { return maxWidth; }
			}
			public int ItemsCount {
				get { return 1; }
			}
			public FlowDirection Direction {
				get { return FlowDirection.Vertical; }
			}
			public IModelViewLayoutElement Model {
				get { return model; }
			}
		}
		private class GroupInfo : IGroupInfo {
			private int innerGoupItemsCount = 0;
			private double width = 0;
			private IModelLayoutGroup groupModel;
			private FlowDirection direction;
			public GroupInfo(IModelLayoutGroup group, Dictionary<IModelViewLayoutElement, Unit> childrenGroupsWidth) {
				this.groupModel = group;
				this.direction = group.Direction;
				this.width = group.RelativeSize;
				Calc(childrenGroupsWidth);
			}
			private void Calc(Dictionary<IModelViewLayoutElement, Unit> childrenGroupsWidth) {
				List<IGroupInfo> childrenGroup = new List<IGroupInfo>();
				foreach(IModelViewLayoutElement item in groupModel) {
					if(item is IModelLayoutGroup) {
						GroupInfo innerGoupInfo = new GroupInfo((IModelLayoutGroup)item, childrenGroupsWidth);
						innerGoupItemsCount += CalcInnerGoupItemsCount(innerGoupInfo);
						childrenGroup.Add(innerGoupInfo);
					}
					else {
						childrenGroup.Add(new FakeGroupInfo(item));
						innerGoupItemsCount++;
					}
				}
				if(groupModel.Count > 0) {
					CalcWidth(childrenGroupsWidth, childrenGroup);
				}
			}
			private void CalcWidth(Dictionary<IModelViewLayoutElement, Unit> childrenGroupsWidth, List<IGroupInfo> childrenGroup) {
				double totalWidth = 100;
				List<IGroupInfo> groupsForCalculateWidth = new List<IGroupInfo>();
				for(int i = 0; i < childrenGroup.Count; i++) {
					IGroupInfo groupTestItem = childrenGroup[i];
					if(groupTestItem.Width == 0) {
						groupsForCalculateWidth.Add(groupTestItem);
					}
					else {
						double itemWidth = groupTestItem.Width;
						totalWidth -= groupTestItem.Width;
						if(i == childrenGroup.Count - 1 && groupsForCalculateWidth.Count == 0) {
							if(totalWidth > 0) {
								itemWidth += totalWidth;
								totalWidth = 0;
							}
						}
						childrenGroupsWidth.Add(groupTestItem.Model, Unit.Percentage(itemWidth));
						innerGoupItemsCount -= CalcInnerGoupItemsCount(groupTestItem);
					}
				}
				foreach(IGroupInfo groupTestItem in groupsForCalculateWidth) {
					double calculatedWidth = 100;
					if(direction == FlowDirection.Horizontal) {
						calculatedWidth = totalWidth / ItemsCount * CalcInnerGoupItemsCount(groupTestItem);
					}
					childrenGroupsWidth.Add(groupTestItem.Model, Unit.Percentage(calculatedWidth));
				}
			}
			private int CalcInnerGoupItemsCount(IGroupInfo innerGoupInfo) {
				return innerGoupInfo.Direction == FlowDirection.Vertical ? 1 : innerGoupInfo.ItemsCount;
			}
			public double Width {
				get {
					return width;
				}
			}
			public int ItemsCount {
				get {
					if(groupModel.Direction == FlowDirection.Vertical) {
						return 1;
					}
					else {
						int result = innerGoupItemsCount;
						return result != 0 ? result : 1;
					}
				}
			}
			public FlowDirection Direction {
				get {
					return groupModel.Direction;
				}
			}
			public IModelViewLayoutElement Model {
				get {
					return groupModel;
				}
			}
			public override string ToString() {
				return groupModel.Id; ;
			}
		}
	}
}
