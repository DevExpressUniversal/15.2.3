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
using System.Collections.Generic;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
namespace DevExpress.ExpressApp.Model.NodeWrappers {
	internal class ModelNodeInternal<NodeType> : IModelNode where NodeType : IModelNode {
		private NodeType modelNode;
		private IDictionary<string, object> values;
		public ModelNodeInternal(NodeType modelNode) {
			this.modelNode = modelNode;
			values = new Dictionary<string, object>();
		}
		protected internal NodeType ModelNode { get { return modelNode; } }
		#region IModelNode Members
		public IModelApplication Application { get { return modelNode != null ? modelNode.Application : null; } }
		public IModelNode Parent { get { return modelNode != null ? modelNode.Parent : null; } }
		public IModelNode Root { get { return modelNode != null ? modelNode.Root : null; } }
		public int NodeCount { get { return modelNode != null ? modelNode.NodeCount : 0; } }
		public int? Index {
			get {
				object result = 0;
				if(values.TryGetValue(DevExpress.ExpressApp.Model.Core.ModelValueNames.Index, out result)) {
					return (int?)result;
				}
				return null;
			}
			set {
				SetValue<int?>(DevExpress.ExpressApp.Model.Core.ModelValueNames.Index, value);
			}
		}
		public IModelNode GetNode(int index) {
			return modelNode != null ? modelNode.GetNode(index) : null;
		}
		public IModelNode GetNode(string id) {
			return modelNode != null ? modelNode.GetNode(id) : null;
		}
		public T AddNode<T>() {
			return modelNode != null ? modelNode.AddNode<T>() : default(T);
		}
		public T AddNode<T>(string id) {
			return modelNode != null ? modelNode.AddNode<T>(id) : default(T);
		}
		public void Remove() {
			if(modelNode != null)
				modelNode.Remove();
		}
		public T GetValue<T>(string name) {
			if(string.IsNullOrEmpty(name))
				return default(T);
			object value;
			if(values.TryGetValue(name, out value))
				return (T)value;
			if(modelNode != null)
				return modelNode.GetValue<T>(name);
			return default(T);
		}
		public void SetValue<T>(string name, T value) {
			if(!string.IsNullOrEmpty(name)) {
				values[name] = value;
			}
		}
		public void ClearValue(string name) {
			if(!string.IsNullOrEmpty(name)) {
				values.Remove(name);
			}
		}
		public bool HasValue(string name) {
			if(string.IsNullOrEmpty(name)) {
				bool result = values.ContainsKey(name);
				if(!result && modelNode != null)
					return modelNode.HasValue(name);
			}
			return false;
		}
		#endregion
	}
	internal class ModelActionInternal : ModelNodeInternal<IModelAction>, IModelAction {
		public ModelActionInternal(IModelAction modelAction) : base(modelAction) { }
		public ModelActionInternal(string id)
			: base(null) {
			Id = id;
		}
		#region IModelAction Members
		[Localizable(true)]
		public String Caption {
			get { return GetValue<String>("Caption"); }
			set { SetValue<String>("Caption", value); }
		}
		public String ImageName {
			get { return GetValue<String>("ImageName"); }
			set { SetValue<String>("ImageName", value); }
		}
		public SelectionDependencyType SelectionDependencyType {
			get { return GetValue<SelectionDependencyType>("SelectionDependencyType"); }
			set { SetValue<SelectionDependencyType>("SelectionDependencyType", value); }
		}
		public String Category {
			get { return GetValue<String>("Category"); }
			set { SetValue<String>("Category", value); }
		}
		[Localizable(true)]
		public String ConfirmationMessage {
			get { return GetValue<String>("ConfirmationMessage"); }
			set { SetValue<String>("ConfirmationMessage", value); }
		}
		public String Shortcut {
			get { return GetValue<String>("Shortcut"); }
			set { SetValue<String>("Shortcut", value); }
		}
		[Localizable(true)]
		public String ToolTip {
			get { return GetValue<String>("ToolTip"); }
			set { SetValue<String>("ToolTip", value); }
		}
		public String Id {
			get { return GetValue<String>("Id"); }
			set { SetValue<String>("Id", value); }
		}
		public ViewType TargetViewType {
			get { return GetValue<ViewType>("TargetViewType"); }
			set { SetValue<ViewType>("TargetViewType", value); }
		}
		public Nesting TargetViewNesting {
			get { return GetValue<Nesting>("TargetViewNesting"); }
			set { SetValue<Nesting>("TargetViewNesting", value); }
		}
		public String TargetObjectType {
			get { return GetValue<String>("TargetObjectType"); }
			set { SetValue<String>("TargetObjectType", value); }
		}
		public String TargetViewId {
			get { return GetValue<String>("TargetViewId"); }
			set { SetValue<String>("TargetViewId", value); }
		}
		public String TargetObjectsCriteria {
			get { return GetValue<String>("TargetObjectsCriteria"); }
			set { SetValue<String>("TargetObjectsCriteria", value); }
		}
		public TargetObjectsCriteriaMode TargetObjectsCriteriaMode {
			get { return GetValue<TargetObjectsCriteriaMode>("TargetObjectsCriteriaMode"); }
			set { SetValue<TargetObjectsCriteriaMode>("TargetObjectsCriteriaMode", value); }
		}
		[Localizable(true)]
		public String ShortCaption {
			get { return GetValue<String>("ShortCaption"); }
			set { SetValue<String>("ShortCaption", value); }
		}
		public IModelController Controller {
			get { return GetValue<IModelController>("Controller"); }
			set { SetValue<IModelController>("Controller", value); }
		}
		public ActionItemPaintStyle PaintStyle {
			get { return GetValue<ActionItemPaintStyle>("PaintStyle"); }
			set { SetValue<ActionItemPaintStyle>("PaintStyle", value); }
		}
		public Boolean QuickAccess {
			get { return GetValue<Boolean>("QuickAccess"); }
			set { SetValue<Boolean>("QuickAccess", value); }
		}
		public Boolean ShowItemsOnClick {
			get { return GetValue<Boolean>("ShowItemsOnClick"); }
			set { SetValue<Boolean>("ShowItemsOnClick", value); }
		}
		public String CaptionFormat {
			get { return GetValue<String>("CaptionFormat"); }
			set { SetValue<String>("CaptionFormat", value); }
		}
		public IModelDisableReasons DisableReasons {
			get { return (IModelDisableReasons)GetNode("DisableReasons"); }
			set { SetValue<IModelDisableReasons>("DisableReasons", value); }
		}
		public IModelChoiceActionItems ChoiceActionItems {
			get { return (IModelChoiceActionItems)GetNode("ChoiceActionItems"); }
		}
		public DefaultItemMode DefaultItemMode {
			get { return GetValue<DefaultItemMode>("DefaultItemMode"); }
			set { SetValue<DefaultItemMode>("DefaultItemMode", value); }
		}
		public ImageMode ImageMode {
			get { return GetValue<ImageMode>("ImageMode"); }
			set { SetValue<ImageMode>("ImageMode", value); }
		}
		public String NullValuePrompt {
			get { return GetValue<String>("NullValuePrompt"); }
			set { SetValue<String>("NullValuePrompt", value); }
		}
		#endregion
	}
	internal class ModelChoiceActionItemInternal : ModelNodeInternal<IModelChoiceActionItem>, IModelChoiceActionItem {
		public ModelChoiceActionItemInternal(string id)
			: base(null) {
			Id = id;
		}
		public ModelChoiceActionItemInternal(IModelChoiceActionItem info) : base(info) { }
		#region IModelChoiceActionItem Members
		public string Id {
			get { return GetValue<string>("Id"); }
			set { SetValue<string>("Id", value); }
		}
		[Localizable(true)]
		public string Caption {
			get { return GetValue<string>("Caption"); }
			set { SetValue<string>("Caption", value); }
		}
		public string ImageName {
			get { return GetValue<string>("ImageName"); }
			set { SetValue<string>("ImageName", value); }
		}
		public string Shortcut {
			get { return GetValue<string>("Shortcut"); }
			set { SetValue<string>("Shortcut", value); }
		}
		[Localizable(true)]
		public String ToolTip {
			get { return GetValue<String>("ToolTip"); }
			set { SetValue<String>("ToolTip", value); }
		}
		public IModelChoiceActionItems ChoiceActionItems { get { return (IModelChoiceActionItems)GetNode("ChoiceActionItems"); } }
		#endregion
	}
}
