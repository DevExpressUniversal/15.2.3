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
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	[DomainComponent]
	public class DiagnosticInfoObject {
		private String asText;
		public DiagnosticInfoObject(String asText) {
			this.asText = asText;
		}
		[FieldSize(FieldSizeAttribute.Unlimited)]
		public String AsText {
			get { return asText; }
		}
	}
	public interface IDiagnosticInfoProvider {
		ChoiceActionItem DiagnosticActionItem { get; }
		String GetDiagnosticInfoObjectString();
	}
	public class DiagnosticInfoController : Controller {
		private SingleChoiceAction diagnosticInfo;
		private Dictionary<ChoiceActionItem, IDiagnosticInfoProvider> diagnosticInfoProviderByActionItem = new Dictionary<ChoiceActionItem, IDiagnosticInfoProvider>();
		private void DiagnosticInfo_Execute(Object sender, SingleChoiceActionExecuteEventArgs e) {
			DiagnosticInfoObject diagnosticObject = new DiagnosticInfoObject(diagnosticInfoProviderByActionItem[e.SelectedChoiceActionItem].GetDiagnosticInfoObjectString());
			Tracing.Tracer.LogText(diagnosticObject.AsText);
			e.ShowViewParameters.CreatedView = Application.CreateDetailView(new NonPersistentObjectSpace(Application.TypesInfo), diagnosticObject, null);
			e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
			e.ShowViewParameters.Context = TemplateContext.PopupWindow;
			e.ShowViewParameters.Controllers.Add(Application.CreateController<DialogController>());
		}
		private void Window_TemplateChanged(Object sender, EventArgs e) {
			Window window = Frame as Window;
			bool isChildWindow = window != null && !window.IsMain;
			string childWindowAddition = " " + CaptionHelper.GetLocalizedText("Captions", "DiagnosticChildWindowAddition");
			string basicCaption = diagnosticInfo.Caption.Replace(childWindowAddition, String.Empty);
			diagnosticInfo.Caption = isChildWindow ? basicCaption + childWindowAddition : basicCaption;
		}
		private void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
			ObjectView objectView = Frame.View as ObjectView;
			bool isDiagnosticInfoObject = objectView != null && objectView.ObjectTypeInfo.Type == typeof(DiagnosticInfoObject);
			diagnosticInfo.Active.SetItemValue("ObjectType should not be " + typeof(DiagnosticInfoObject).Name, !isDiagnosticInfoObject);
		}
		private void FillDiagnosticInfoProviderByActionItem() {
			foreach(Controller controller in Frame.Controllers) {
				IDiagnosticInfoProvider diagnosticInfoProvider = controller as IDiagnosticInfoProvider;
				if(diagnosticInfoProvider != null) {
					ChoiceActionItem item = diagnosticInfoProvider.DiagnosticActionItem;
					diagnosticInfo.Items.Add(item);
					diagnosticInfoProviderByActionItem.Add(item, diagnosticInfoProvider);
				}
			}
		}
		private void SubscribeToFrameEvents() {
			Frame.ViewChanged += Frame_ViewChanged;
			Window window = Frame as Window;
			if(window != null) {
				window.TemplateChanged += Window_TemplateChanged;
			}
		}
		private void UnsubscribeFromFrameEvents() {
			Frame.ViewChanged -= Frame_ViewChanged;
			Window window = Frame as Window;
			if(window != null) {
				window.TemplateChanged -= Window_TemplateChanged;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			diagnosticInfo.Active.SetItemValue("Diagnostic enabled in config file", StringHelper.ParseToBool(ConfigurationManager.AppSettings["EnableDiagnosticActions"], false));
			FillDiagnosticInfoProviderByActionItem();
			SubscribeToFrameEvents();
		}
		protected override void OnDeactivated() {
			diagnosticInfoProviderByActionItem.Clear();
			diagnosticInfo.Items.Clear();
			UnsubscribeFromFrameEvents();
			base.OnDeactivated();
		}
		public DiagnosticInfoController() {
			diagnosticInfo = new SingleChoiceAction(this, "Diagnostic Info", "Diagnostic");
			diagnosticInfo.Caption = "Diagnostic";
			diagnosticInfo.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			diagnosticInfo.Execute += new SingleChoiceActionExecuteEventHandler(DiagnosticInfo_Execute);
		}
		public SingleChoiceAction DiagnosticInfo {
			get { return diagnosticInfo; }
		}
	}
	public abstract class DiagnosticInfoProviderBase : Controller, IDiagnosticInfoProvider {
		public static XmlNode CreateBoolListNode(XmlDocument doc, BoolList list, string nodeName) {
			XmlNode result = doc.CreateNode(XmlNodeType.Element, nodeName, "");
			foreach(string key in list.GetKeys()) {
				XmlNode itemNode = doc.CreateNode(XmlNodeType.Element, "Item", "");
				XmlAttribute attr = doc.CreateAttribute("Key");
				attr.Value = key;
				itemNode.Attributes.Append(attr);
				attr = doc.CreateAttribute("Value");
				attr.Value = list[key].ToString();
				itemNode.Attributes.Append(attr);
				result.AppendChild(itemNode);
			}
			return result;
		}
		public static string XmlNodeToString(XmlNode xmlNode) {
			if(xmlNode == null) {
				return "";
			}
			StringBuilder sb = new StringBuilder();
			using(StringWriter stringWriter = new StringWriter(sb)) {
				using(XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter)) {
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlNode.WriteTo(xmlTextWriter);
				}
			}
			return sb.ToString();
		}
		private SingleChoiceAction diagnosticInfo;
		protected XmlDocument doc = new XmlDocument();
		private ChoiceActionItem diagnosticActionItem;
		protected virtual XmlNode CreateBoolListNode(BoolList list, string nodeName) {
			return CreateBoolListNode(doc, list, nodeName);
		}
		protected abstract string GetActionItemCaption();
		public abstract string GetDiagnosticInfoObjectString();
		public ChoiceActionItem DiagnosticActionItem {
			get {
				if(diagnosticActionItem == null) {
					string itemCaption = GetActionItemCaption();
					diagnosticActionItem = new ChoiceActionItem(itemCaption, itemCaption, null);
				}
				return diagnosticActionItem;
			}
		}
		public DiagnosticInfoProviderBase() {
			diagnosticInfo = new SingleChoiceAction(this, "Diagnostic Info." + DiagnosticActionItem.Id, "Diagnostic");
			diagnosticInfo.Active.SetItemValue("Fake action for localization", false);
			diagnosticInfo.Items.Add(DiagnosticActionItem);
		}
		protected string FormatXml(XmlNode xmlNode) {
			return XmlNodeToString(xmlNode);
		}
		protected static XmlAttribute CreateAttribute(XmlDocument doc, string name, string value) {
			XmlAttribute attr = doc.CreateAttribute(name);
			attr.Value = value;
			return attr;
		}
		protected XmlAttribute CreateAttribute(string name, string value) {
			return CreateAttribute(doc, name, value);
		}
	}
	public class ActionsInfoController : DiagnosticInfoProviderBase {
		public static XmlNode CreateActionNode(XmlDocument doc, ActionBase action) {
			XmlNode result = doc.CreateNode(XmlNodeType.Element, "Action", "");
			result.Attributes.Append(CreateAttribute(doc, "ID", action.Id));
			result.Attributes.Append(CreateAttribute(doc, "Caption", action.Caption));
			result.Attributes.Append(CreateAttribute(doc, "TypeName", action.GetType().Name));
			result.Attributes.Append(CreateAttribute(doc, "Category", action.Category));
			result.Attributes.Append(CreateAttribute(doc, "Active", action.Active.ResultValue.ToString()));
			result.Attributes.Append(CreateAttribute(doc, "Enabled", action.Enabled.ResultValue.ToString()));
			result.Attributes.Append(CreateAttribute(doc, "AdditionalInfo", action.DiagnosticInfo));
			BoolList activeList = action.Active;
			if(activeList.Count > 0)
				result.AppendChild(CreateBoolListNode(doc, activeList, "ActiveList"));
			BoolList actionEnabledList = action.Enabled;
			if(actionEnabledList.Count > 0)
				result.AppendChild(CreateBoolListNode(doc, actionEnabledList, "EnabledList"));
			if(action is SingleChoiceAction) {
				string selectedItemCaption = "";
				if(((SingleChoiceAction)action).SelectedItem != null) {
					selectedItemCaption = ((SingleChoiceAction)action).SelectedItem.Caption;
				}
				result.Attributes.Append(CreateAttribute(doc, "SelectedItem", selectedItemCaption));
				ChoiceActionItemCollection itemCollection = ((SingleChoiceAction)action).Items;
				if(itemCollection.Count > 0) {
					result.AppendChild(AddSingleChoiceItems(doc, itemCollection));
				}
			}
			return result;
		}
		protected static XmlNode AddSingleChoiceItems(XmlDocument doc, ChoiceActionItemCollection itemCollection) {
			XmlNode singleChoiceActionsNode = doc.CreateNode(XmlNodeType.Element, "Items", "");
			foreach(ChoiceActionItem item in itemCollection) {
				singleChoiceActionsNode.AppendChild(AddSingleChoiceItem(doc, item));
			}
			return singleChoiceActionsNode;
		}
		protected static XmlNode AddSingleChoiceItem(XmlDocument doc, ChoiceActionItem item) {
			XmlNode result = doc.CreateNode(XmlNodeType.Element, "Item", "");
			result.Attributes.Append(CreateAttribute(doc, "Caption", item.Caption));
			result.Attributes.Append(CreateAttribute(doc, "Active", item.Active.ResultValue.ToString()));
			result.Attributes.Append(CreateAttribute(doc, "Enabled", item.Enabled.ResultValue.ToString()));
			BoolList visibleList = item.Active;
			if(visibleList.Count > 0) {
				result.AppendChild(CreateBoolListNode(doc, visibleList, "ActiveList"));
			}
			BoolList enabledList = item.Enabled;
			if(enabledList.Count > 0) {
				result.AppendChild(CreateBoolListNode(doc, enabledList, "EnabledList"));
			}
			if(item.Items.Count > 0) {
				result.AppendChild(AddSingleChoiceItems(doc, item.Items));
			}
			return result;
		}
		protected virtual XmlNode CreateControllersNode() {
			XmlNode result = doc.CreateNode(XmlNodeType.Element, "Controllers", "");
			foreach(Controller controller in Frame.Controllers) {
				result.AppendChild(CreateControllerNode(controller));
			}
			return result;
		}
		protected virtual XmlNode CreateControllerNode(Controller controller) {
			XmlNode result = doc.CreateNode(XmlNodeType.Element, "Controller", "");
			result.Attributes.Append(CreateAttribute("Name", controller.GetType().Name));
			result.Attributes.Append(CreateAttribute("FullName", controller.GetType().FullName));
			result.Attributes.Append(CreateAttribute("Active", controller.Active.ResultValue.ToString()));
			if(controller.Active.Count > 0)
				result.AppendChild(CreateBoolListNode(controller.Active, "ActiveList"));
			if(controller.Actions.Count > 0) {
				XmlNode actionsNode = doc.CreateNode(XmlNodeType.Element, "Actions", "");
				foreach(ActionBase action in controller.Actions) {
					actionsNode.AppendChild(CreateActionNode(action));
				}
				result.AppendChild(actionsNode);
			}
			return result;
		}
		protected virtual XmlNode CreateActionNode(ActionBase action) {
			return CreateActionNode(doc, action);
		}
		protected virtual XmlNode AddSingleChoiceItems(ChoiceActionItemCollection itemCollection) {
			return AddSingleChoiceItems(doc, itemCollection);
		}
		protected virtual XmlNode AddSingleChoiceItem(ChoiceActionItem item) {
			return AddSingleChoiceItem(doc, item);
		}
		protected virtual XmlNode CreateContainerNode(IActionContainer actionContainer, string name) {
			XmlNode containerNode = doc.CreateNode(XmlNodeType.Element, "Container", "");
			containerNode.Attributes.Append(CreateAttribute("Name", !string.IsNullOrEmpty(name) ? name : actionContainer.ContainerId));
			if(actionContainer.Actions.Count > 0) {
				XmlNode actionsNode = doc.CreateNode(XmlNodeType.Element, "Actions", "");
				foreach(ActionBase action in actionContainer.Actions) {
					XmlNode actionNode = doc.CreateNode(XmlNodeType.Element, "Action", "");
					actionNode.Attributes.Append(CreateAttribute("ID", action.Id));
					actionsNode.AppendChild(actionNode);
				}
				containerNode.AppendChild(actionsNode);
			}
			return containerNode;
		}
		protected virtual XmlNode CreateActionContainersNode(ICollection<IActionContainer> actionContainers) {
			XmlNode result = doc.CreateNode(XmlNodeType.Element, "ActionContainers", "");
			foreach(IActionContainer actionContainer in actionContainers) {
				result.AppendChild(CreateContainerNode(actionContainer, ""));
			}
			return result;
		}
		protected virtual XmlNode CreateTemplateNodeNode(Frame frame) {
			XmlNode result = doc.CreateNode(XmlNodeType.Element, "Template", "");
			if(frame is Window) {
				result.Attributes.Append(CreateAttribute("Name", ((Window)frame).Context));
			}
			else {
				result.Attributes.Append(CreateAttribute("Name", "Cannot determine the name of the template: the frame is not a Window"));
			}
			if(frame.Template != null) {
				result.Attributes.Append(CreateAttribute("TypeName", frame.Template.GetType().FullName));
				if(frame.Template.DefaultContainer != null) {
					XmlNode defaultContainerNode = CreateContainerNode(frame.Template.DefaultContainer, "DefaultActionContainer");
					result.AppendChild(defaultContainerNode);
				}
				if(frame.Template.GetContainers().Count > 0)
					result.AppendChild(CreateActionContainersNode(frame.Template.GetContainers()));
			}
			else {
				result.Attributes.Append(CreateAttribute("TypeName", "Template is null"));
			}
			return result;
		}
		private const string diagnosticInfoItemCaption = "Actions Info";
		protected override string GetActionItemCaption() {
			return diagnosticInfoItemCaption;
		}
		public override string GetDiagnosticInfoObjectString() {
			XmlNode node = doc.CreateNode(XmlNodeType.Element, "Frame", "");
			node.AppendChild(CreateTemplateNodeNode(Frame));
			if(Frame.Controllers.Count > 0) {
				node.AppendChild(CreateControllersNode());
			}
			return FormatXml(node);
		}
	}
	public class ViewInfoController : DiagnosticInfoProviderBase {
		private void AddBoolList(XmlNode parent, BoolList boolList, string nodeName) {
			XmlNode newNode = CreateBoolListNode(boolList, nodeName);
			newNode.Attributes.Append(CreateAttribute("Result", boolList.ResultValue.ToString()));
			parent.AppendChild(newNode);
		}
		private void CreationModifyAttributes(XmlNode parent, View view) {
			AddBoolList(parent, view.AllowNew, "AllowNew");
			AddBoolList(parent, view.AllowEdit, "AllowEdit");
			AddBoolList(parent, view.AllowDelete, "AllowDelete");
		}
		protected virtual XmlNode CreateListViewNode() {
			XmlNode result = doc.CreateNode(XmlNodeType.Element, "ListView", "");
			ListView listView = Frame.View as ListView;
			result.Attributes.Append(CreateAttribute("ID", listView.Id));
			result.Attributes.Append(CreateAttribute("IsRoot", listView.IsRoot.ToString()));
			CreationModifyAttributes(result, listView);
			XmlNode listEditorNode = doc.CreateNode(XmlNodeType.Element, "ListEditor", "");
			listEditorNode.Attributes.Append(CreateAttribute("Type", listView.Editor.GetType().FullName));
			listEditorNode.Attributes.Append(CreateAttribute("Name", listView.Editor.Name));
			result.AppendChild(listEditorNode);
			XmlNode criteriaNode = doc.CreateNode(XmlNodeType.Element, "Criteria", "");
			string criteriaValue = "null";
			if(listView.CollectionSource != null) {
				DevExpress.Data.Filtering.CriteriaOperator criteriaOperator = listView.CollectionSource.GetTotalCriteria();
				if(!object.ReferenceEquals(criteriaOperator, null)) {
					criteriaValue = criteriaOperator.LegacyToString();
				}
			}
			criteriaNode.Attributes.Append(CreateAttribute("Value", criteriaValue));
			result.AppendChild(criteriaNode);
			return result;
		}
		protected virtual XmlNode CreateDetailViewNode() {
			DetailView detailView = Frame.View as DetailView;
			XmlNode result = doc.CreateNode(XmlNodeType.Element, "DetailView", "");
			result.Attributes.Append(CreateAttribute("ID", detailView.Id));
			result.Attributes.Append(CreateAttribute("IsRoot", detailView.IsRoot.ToString()));
			result.Attributes.Append(CreateAttribute("ViewEditMode", detailView.ViewEditMode.ToString()));
			CreationModifyAttributes(result, detailView);
			XmlNode propertyEditorsNode = doc.CreateNode(XmlNodeType.Element, "PropertyEditors", "");
			foreach(PropertyEditor editor in detailView.GetItems<PropertyEditor>()) {
				propertyEditorsNode.AppendChild(CreatePropertyEditorNode(editor));
			}
			result.AppendChild(propertyEditorsNode);
			return result;
		}
		protected virtual XmlNode CreatePropertyEditorNode(PropertyEditor editor) {
			XmlNode result = doc.CreateNode(XmlNodeType.Element, "PropertyEditor", "");
			result.Attributes.Append(CreateAttribute("ID", editor.Id));
			result.Attributes.Append(CreateAttribute("Type", editor.GetType().FullName));
			result.Attributes.Append(CreateAttribute("Caption", editor.Caption));
			result.Attributes.Append(CreateAttribute("PropertyName", editor.PropertyName));
			result.Attributes.Append(CreateAttribute("AllowEdit", editor.AllowEdit.ResultValue.ToString()));
			if(editor.AllowEdit.Count > 0) {
				result.AppendChild(CreateBoolListNode(editor.AllowEdit, "AllowEditList"));
			}
			return result;
		}
		private const string diagnosticInfoItemCaption = "View Info";
		protected override string GetActionItemCaption() {
			return diagnosticInfoItemCaption;
		}
		public override string GetDiagnosticInfoObjectString() {
			XmlNode result = null;
			if(Frame.View is DetailView) {
				result = CreateDetailViewNode();
			}
			else if(Frame.View is ListView) {
				result = CreateListViewNode();
			}
			return FormatXml(result);
		}
	}
}
