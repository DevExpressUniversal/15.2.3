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
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Model {
	[ImageName("ModelEditor_DetailView")]
	[ModelInterfaceImplementor(typeof(IModelLayoutManagerOptions), "Application.Options.LayoutManagerOptions")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelDetailView")]
#endif
	[ModelNodeValueSource("ImageName", "ModelClass", "DefaultDetailViewImage")]
	public interface IModelDetailView : IModelObjectView, IModelCompositeView, IModelLayoutManagerOptions {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelDetailViewObjectCaptionFormat"),
#endif
 Localizable(true)]
		[ModelValueCalculator("ModelClass", "ObjectCaptionFormat")]
		[Category("Format")]
		String ObjectCaptionFormat { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelDetailViewFreezeLayout")]
#endif
		[Category("Behavior")]
		[ModelBrowsable(typeof(NotNewNodeVisibleCalculator))]
		bool FreezeLayout { get; set; }
	}
	[ImageName("ModelEditor_DetailViewItems")]
	[ModelNodesGenerator(typeof(ModelDetailViewItemsNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelViewItems")]
#endif
	public interface IModelViewItems : IModelNode, IModelList<IModelViewItem> {
	}
	[ModelAbstractClass]
	public interface IModelViewItem : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewItemId"),
#endif
 Required()]
		string Id { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewItemCaption"),
#endif
 Localizable(true)]
		string Caption { get; set; }
	}
	[ModelPersistentName("Layout")]
	[ModelNodesGenerator(typeof(ModelDetailViewLayoutNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelViewLayout")]
#endif
	public interface IModelViewLayout : IModelNode, IModelList<IModelViewLayoutElement> {
	}
	[ModelAbstractClass]
	public interface IModelViewLayoutElement : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewLayoutElementId"),
#endif
 Required()]
		string Id { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewLayoutElementRelativeSize"),
#endif
 Category("Layout")]
		double RelativeSize { get; set; }
	}
	public interface IModelLayoutElementWithCaption : IModelLayoutElementWithCaptionOptions {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutElementWithCaptionCaption"),
#endif
 Localizable(true)]
		string Caption { get; set; }
	}
	public interface IModelLayoutElementWithCaptionOptions : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutElementWithCaptionOptionsShowCaption"),
#endif
 Category("Behavior")]
		bool? ShowCaption { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutElementWithCaptionOptionsCaptionLocation"),
#endif
 Category("Appearance")]
		Locations CaptionLocation { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutElementWithCaptionOptionsCaptionHorizontalAlignment"),
#endif
 Category("Appearance")]
		HorzAlignment CaptionHorizontalAlignment { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutElementWithCaptionOptionsCaptionVerticalAlignment"),
#endif
 Category("Appearance")]
		VertAlignment CaptionVerticalAlignment { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutElementWithCaptionOptionsCaptionWordWrap"),
#endif
 Category("Appearance")]
		WordWrap CaptionWordWrap { get; set; }
	}
	[DomainLogic(typeof(IModelLayoutElementWithCaptionOptions))]
	public static class ModelLayoutElementWithCaptionOptionsLogic {
		private static IModelLayoutManagerOptions GetViewLayoutOptions(IModelLayoutElementWithCaptionOptions elementModel) {
			IModelNode parent = elementModel.Parent;
			IModelDetailView detailViewModel = parent as IModelDetailView;
			while(parent != null && detailViewModel == null) {
				parent = parent.Parent;
				detailViewModel = parent as IModelDetailView;
			}
			return detailViewModel as IModelLayoutManagerOptions;
		}
		public static Locations Get_CaptionLocation(IModelLayoutElementWithCaptionOptions elementModel) {
			IModelLayoutManagerOptions optionsModel = GetViewLayoutOptions(elementModel);
			if(optionsModel != null) {
				return optionsModel.CaptionLocation;
			}
			return Locations.Default;
		}
		public static HorzAlignment Get_CaptionHorizontalAlignment(IModelLayoutElementWithCaptionOptions elementModel) {
			IModelLayoutManagerOptions optionsModel = GetViewLayoutOptions(elementModel);
			if(optionsModel != null) {
				return optionsModel.CaptionHorizontalAlignment;
			}
			return HorzAlignment.Default;
		}
		public static VertAlignment Get_CaptionVerticalAlignment(IModelLayoutElementWithCaptionOptions elementModel) {
			IModelLayoutManagerOptions optionsModel = GetViewLayoutOptions(elementModel);
			if(optionsModel != null) {
				return optionsModel.CaptionVerticalAlignment;
			}
			return VertAlignment.Default;
		}
		public static WordWrap Get_CaptionWordWrap(IModelLayoutElementWithCaptionOptions elementModel) {
			IModelLayoutManagerOptions optionsModel = GetViewLayoutOptions(elementModel);
			if(optionsModel != null) {
				return optionsModel.CaptionWordWrap;
			}
			return WordWrap.Default;
		}
	}
	[DomainLogic(typeof(IModelLayoutGroup))]
	public static class ModelLayoutGroupLogic {
		internal static IEnumerable<T> GetLayoutItems<T>(IEnumerable items) where T : IModelViewLayoutElement {
			Stack<IEnumerable> stack = new Stack<IEnumerable>();
			stack.Push(items);
			while(stack.Count > 0) {
				foreach(IModelViewLayoutElement item in stack.Pop()) {
					if(item is T) {
						yield return (T)item;
					}
					IEnumerable layoutItems = item as IEnumerable;
					if(layoutItems != null) {
						stack.Push(layoutItems);
					}
				}
			}
		}
		public static string Get_Caption(IModelLayoutGroup layoutGroup) {
			if(layoutGroup.Count == 1 && layoutGroup[0] is IModelLayoutViewItem) {
				IModelDetailView detailViewModel = GetParentDetailView(layoutGroup);
				if(detailViewModel != null) {
					IModelViewItem layoutItemModel = ((IModelLayoutViewItem)layoutGroup[0]).ViewItem;
					if(layoutItemModel != null) {
						return layoutItemModel.Caption;
					}
				}
			}
			return layoutGroup.Id;
		}
		internal static IModelDetailView GetParentDetailView(IModelNode elementModel) {
			IModelNode result = elementModel;
			while(!(result is IModelDetailView) && result != null) {
				result = result.Parent;
			}
			return (IModelDetailView)result;
		}
		public static string Get_ImageName(IModelLayoutGroup modelLayoutGroup) {
			string result = "";
			IModelPropertyEditor modelPropertyEditor = GetPropertyEditorForGroup(modelLayoutGroup);
			if(modelPropertyEditor != null) {
				IModelView view = modelPropertyEditor.View;
				if(view != null) {
					result = view.ImageName;
				}
			}
			return result;
		}
		public static string Get_ToolTip(IModelLayoutGroup modelLayoutGroup) {
			IModelToolTip editor = GetPropertyEditorForGroup(modelLayoutGroup);
			if(editor != null) {
				return editor.ToolTip;
			}
			return null;
		}
		public static string Get_ToolTipTitle(IModelLayoutGroup modelLayoutGroup) {
			IModelToolTipOptions editor = GetPropertyEditorForGroup(modelLayoutGroup) as IModelToolTipOptions;
			if(editor != null) {
				return editor.ToolTipTitle;
			}
			return null;
		}
		public static ToolTipIconType Get_ToolTipIconType(IModelLayoutGroup modelLayoutGroup) {
			IModelToolTipOptions editor = GetPropertyEditorForGroup(modelLayoutGroup) as IModelToolTipOptions;
			if(editor != null) {
				return editor.ToolTipIconType;
			}
			return ToolTipIconType.None;
		}
		internal static IModelPropertyEditor GetPropertyEditorForGroup(IModelLayoutGroup modelLayoutGroup) {
			IModelDetailView modelDetailView = GetParentDetailView(modelLayoutGroup);
			IModelPropertyEditor editor = null;
			if(modelDetailView != null) {
				editor = modelDetailView.Items[modelLayoutGroup.Id] as IModelPropertyEditor;
				if((editor == null) && (modelLayoutGroup.Id.Contains(ModelDetailViewLayoutNodesGenerator.LayoutGroupNameSuffix))) {
					editor = modelDetailView.Items[modelLayoutGroup.Id.Substring(0, modelLayoutGroup.Id.Length - ModelDetailViewLayoutNodesGenerator.LayoutGroupNameSuffix.Length)] as IModelPropertyEditor;
				}
			}
			return editor;
		}
#if DebugTest
		public static IEnumerable<T> DebugTest_GetLayoutItems<T>(IEnumerable items) where T : IModelViewLayoutElement {
			return GetLayoutItems<T>(items);
		}
#endif
	}
	[DomainLogic(typeof(IModelLayoutViewItem))]
	public static class ModelLayoutViewItemLogic {
		public static StaticHorizontalAlign Get_HorizontalAlign(IModelLayoutViewItem elementModel) {
			StaticHorizontalAlign horizontalAlign = StaticHorizontalAlign.NotSet;
			if(elementModel.Parent is IModelLayoutGroup) {
				StaticHorizontalAlign groupHorizontalAlign = ((IModelLayoutGroup)elementModel.Parent).HorizontalAlign;
				if(groupHorizontalAlign != StaticHorizontalAlign.NotSet) {
					horizontalAlign = groupHorizontalAlign;
				}
			}
			IModelDetailView detailViewModel = ModelLayoutGroupLogic.GetParentDetailView(elementModel);
			if(detailViewModel != null) {
				IModelViewItem layoutItemModel = elementModel.ViewItem; ;
				if(layoutItemModel != null && layoutItemModel is ISupportControlAlignment) {
					ISupportControlAlignment iSupportControlAlignment = (ISupportControlAlignment)layoutItemModel;
					if(iSupportControlAlignment.HorizontalAlign != StaticHorizontalAlign.NotSet) {
						horizontalAlign = iSupportControlAlignment.HorizontalAlign;
					}
				}
			}
			return horizontalAlign;
		}
		public static StaticVerticalAlign Get_VerticalAlign(IModelLayoutViewItem elementModel) {
			StaticVerticalAlign verticalAlign = StaticVerticalAlign.NotSet;
			if(elementModel.Parent is IModelLayoutGroup) {
				StaticVerticalAlign groupVerticalAlign = ((IModelLayoutGroup)elementModel.Parent).VerticalAlign;
				if(groupVerticalAlign != StaticVerticalAlign.NotSet) {
					verticalAlign = groupVerticalAlign;
				}
			}
			IModelDetailView detailViewModel = ModelLayoutGroupLogic.GetParentDetailView(elementModel);
			if(detailViewModel != null) {
				IModelViewItem layoutItemModel = elementModel.ViewItem;
				if(layoutItemModel != null && layoutItemModel is ISupportControlAlignment) {
					ISupportControlAlignment iSupportControlAlignment = (ISupportControlAlignment)layoutItemModel;
					if(iSupportControlAlignment.VerticalAlign != StaticVerticalAlign.NotSet) {
						verticalAlign = iSupportControlAlignment.VerticalAlign;
					}
				}
			}
			return verticalAlign;
		}
		public static IModelList<IModelViewItem> Get_ViewItems(IModelLayoutViewItem elementModel) {
			IModelCompositeView view = FindCompositeView(elementModel);
			LazyCalculatedModelNodeList<IModelViewItem> result = null;
			if(view != null) {
				result = new LazyCalculatedModelNodeList<IModelViewItem>(GetFilteredItems(GetAllUsedViewItems(view.Layout), view.Items), view.Items);
			}
			return result;
		}
		public static string Get_ToolTip(IModelLayoutViewItem elementModel) {
			IModelDetailView detailViewModel = ModelLayoutGroupLogic.GetParentDetailView(elementModel);
			if(detailViewModel != null) {
				IModelToolTip editor = elementModel.ViewItem as IModelToolTip;
				if(editor != null) {
					return editor.ToolTip;
				}
			}
			return null;
		}
		public static string Get_ToolTipTitle(IModelLayoutViewItem elementModel) {
			IModelDetailView detailViewModel = ModelLayoutGroupLogic.GetParentDetailView(elementModel);
			if(detailViewModel != null) {
				IModelToolTipOptions editor = elementModel.ViewItem as IModelToolTipOptions;
				if(editor != null) {
					return editor.ToolTipTitle;
				}
			}
			return null;
		}
		public static ToolTipIconType Get_ToolTipIconType(IModelLayoutViewItem elementModel) {
			IModelDetailView detailViewModel = ModelLayoutGroupLogic.GetParentDetailView(elementModel);
			if(detailViewModel != null) {
				IModelToolTipOptions editor = elementModel.ViewItem as IModelToolTipOptions;
				if(editor != null) {
					return editor.ToolTipIconType;
				}
			}
			return ToolTipIconType.None;
		}
		private static IEnumerable<IModelViewItem> GetFilteredItems(IEnumerable<IModelViewItem> allUsedItems, IEnumerable<IModelViewItem> items) {
			foreach(IModelViewItem item in items) {
				if(Enumerator.Find<IModelViewItem>(allUsedItems, usedItem => usedItem.Id == item.Id) == null) {
					yield return item;
				}
			}
		}
		private static IModelCompositeView FindCompositeView(IModelLayoutViewItem elementModel) {
			IModelNode parent = elementModel.Parent;
			while(parent != null) {
				if(parent is IModelCompositeView) {
					return (IModelCompositeView)parent;
				}
				parent = parent.Parent;
			}
			return null;
		}
		private static IEnumerable<IModelViewItem> GetAllUsedViewItems(IEnumerable items) {
			foreach(IModelLayoutViewItem item in ModelLayoutGroupLogic.GetLayoutItems<IModelLayoutViewItem>(items)) {
				if(item.ViewItem != null) {
					yield return item.ViewItem;
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static IEnumerable<IModelViewItem> GetUsedViewItems(IModelViewLayout layout) {
			return GetAllUsedViewItems(layout);
		}
	}
	[ModelPersistentName("LayoutGroup")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelLayoutGroup")]
#endif
	public interface IModelLayoutGroup : IModelViewLayoutElement, IModelLayoutElementWithCaption, IModelList<IModelViewLayoutElement>, ISupportControlAlignment, IModelToolTip, IModelToolTipOptions {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutGroupDirection"),
#endif
 Category("Behavior")]
		[DefaultValue(FlowDirection.Vertical)]
		FlowDirection Direction { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutGroupImageName"),
#endif
 Category("Appearance")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string ImageName { get; set; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelTabbedGroup")]
#endif
	public interface IModelTabbedGroup : IModelViewLayoutElement, IModelLayoutElementWithCaption, IModelList<IModelLayoutGroup> {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelTabbedGroupMultiLine"),
#endif
 Category("Appearance")]
		bool MultiLine { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelTabbedGroupDirection"),
#endif
 DefaultValue(FlowDirection.Horizontal)]
		[Category("Behavior")]
		FlowDirection Direction { get; set; }
	}
	[ModelAbstractClass]
	public interface IModelLayoutItem : IModelViewLayoutElement, ISupportControlAlignment {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutItemSizeConstraintsType"),
#endif
 Category("Layout")]
		XafSizeConstraintsType SizeConstraintsType { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutItemMinSize"),
#endif
 Category("Layout")]
		Size MinSize { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutItemMaxSize"),
#endif
 Category("Layout")]
		Size MaxSize { get; set; }
	}
	[ModelPersistentName("LayoutItem")] 
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelLayoutItem")]
#endif
	public interface IModelLayoutViewItem : IModelLayoutItem, IModelLayoutElementWithCaptionOptions, ISupportControlAlignment, IModelToolTip, IModelToolTipOptions {
		[DataSourceProperty("ViewItems")]
		[ Category("Layout")]
		IModelViewItem ViewItem { get; set; }
		[Browsable(false)]
		IModelList<IModelViewItem> ViewItems { get; }
	}
}
