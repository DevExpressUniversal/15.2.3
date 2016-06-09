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
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.Utils.Base;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public class ContentContainerActionCollection : Base.BaseMutableList<IContentContainerAction> {
		ISupportBatchUpdate ownerCore;
		public ContentContainerActionCollection(ISupportBatchUpdate owner) {
			ownerCore = owner;
		}
		protected ISupportBatchUpdate Owner {
			get { return ownerCore; }
		}
		protected override void OnDispose() {
			ownerCore = null;
			base.OnDispose();
		}
		IBatchUpdate batchUpdateRangeOperation;
		protected override void OnBeforeElementRangeAdded() {
			batchUpdateRangeOperation = BatchUpdate.Enter(Owner);
		}
		protected override void OnElementRangeAdded() {
			Ref.Dispose(ref batchUpdateRangeOperation);
		}
		protected override void OnBeforeElementRangeRemoved() {
			batchUpdateRangeOperation = BatchUpdate.Enter(Owner);
		}
		protected override void OnElementRangeRemoved() {
			Ref.Dispose(ref batchUpdateRangeOperation);
		}
		public bool Insert(int index, IContentContainerAction action) {
			return InsertCore(index, action);
		}
	}
	public abstract class ContentContainerAction : IContentContainerAction {
		#region static
		internal static bool CanExecute(Args e) {
			return e.Action.CanExecuteCore(e.Parameter);
		}
		internal static void Execute(WindowsUIView view, Args e) {
			if(!e.Stateless)
				e.Action.ExecuteCore(view, e.Parameter, e.State);
			else
				e.Action.ExecuteCore(view, e.Parameter);
		}
		#endregion static
		protected abstract void ExecuteCore(WindowsUIView view, IContentContainer container);
		protected virtual void ExecuteCore(WindowsUIView view, IContentContainer container, object state) {
			ExecuteCore(view, container);
		}
		protected abstract bool CanExecuteCore(IContentContainer container);
		#region IContentContainerAction Members
		ICommand<IContentContainer> IUIAction<IContentContainer>.Command {
			get { return null; }
		}
		string IUIActionProperties.Caption {
			get { return GetCaption(); }
		}
		Image IUIActionProperties.Image {
			get { return GetImage(); }
		}
		string IUIActionProperties.Description {
			get { return GetDescription(); }
		}
		protected virtual string GetCaption() {
			return DocumentManagerLocalizer.GetString(ID);
		}
		protected virtual Image GetImage() {
			return Resources.ContentContainterActionResourceLoader.GetImage(ID);
		}
		protected virtual string GetDescription() {
			return DocumentManagerLocalizer.GetString(DescriptionID);
		}
		protected abstract DocumentManagerStringId ID { get; }
		protected abstract DocumentManagerStringId DescriptionID { get; }
		#endregion
		#region ActionGroup
		public static IActionLayout GetActionLayout(IContentContainerAction action) {
			return action as IActionLayout ?? GetActionGroupCore(action);
		}
		public static IActionLayout GetActionLayout(IDocumentAction action) {
			return action as IActionLayout ?? GetActionGroupCore(action);
		}
		public static IActionStyle GetActionStyle(IDocumentAction action) {
			return action as IActionStyle ?? GetActionGroupCore(action);
		}
		public static IActionBehavior GetActionBehavior(IContentContainerAction action) {
			IActionBehavior behavior = action as IActionBehavior;
			return (behavior != null && behavior.Behavior != ActionBehavior.Default) ?
				behavior : GetActionGroupCore(action);
		}
		public static IActionBehavior GetActionBehavior(IDocumentAction action) {
			IActionBehavior behavior = action as IActionBehavior;
			return (behavior != null && behavior.Behavior != ActionBehavior.Default) ?
				behavior : GetActionGroupCore(action);
		}
		public static ActionGroupAttribute GetActionGroup(IContentContainerAction action) {
			return GetActionGroupCore(action);
		}
		public static ActionGroupAttribute GetActionGroup(IDocumentAction action) {
			return GetActionGroupCore(action);
		}
		static ActionGroupAttribute GetActionGroupCore(object action) {
			object[] attributes = action.GetType().GetCustomAttributes(typeof(ActionGroupAttribute), true);
			return (attributes.Length == 1) ? (ActionGroupAttribute)attributes[0] : ActionGroupAttribute.Unknown;
		}
		public static int Compare(IContentContainerAction x, IContentContainerAction y) {
			return Compare(x, y, null);
		}
		public class ContentContainerActionComparer : System.Collections.Generic.IComparer<IContentContainerAction> {
			IContentContainer containerCore;
			public ContentContainerActionComparer(IContentContainer container) {
				containerCore = container;
			}
			public int Compare(IContentContainerAction x, IContentContainerAction y) {
				if(x == y) return 0;
				if(containerCore is DocumentGroup.DocumentGroupDetailContainer) {
					DetailAction detailX = x as DetailAction;
					DetailAction detailY = y as DetailAction;
					if(detailX != null && detailY != null)
						return detailY.CompareTo(detailX, containerCore.Parent as DocumentGroup);
				}
				if(containerCore is DocumentGroup) {
					DetailAction detailX = x as DetailAction;
					DetailAction detailY = y as DetailAction;
					if(detailX != null && detailY != null)
						return detailY.CompareTo(detailX, containerCore as DocumentGroup);
				}
				ActionGroupAttribute group1 = GetActionGroup(x);
				ActionGroupAttribute group2 = GetActionGroup(y);
				if(group1.Order == group2.Order)
					return group2.Index.CompareTo(group1.Index);
				else return group2.Order.CompareTo(group1.Order);
			}
		}
		public static int Compare(IContentContainerAction x, IContentContainerAction y, IContentContainer container) {
			return new ContentContainerActionComparer(container).Compare(x, y);
		}
		#endregion ActionGroup
		internal sealed class Args {
			IContentContainer parameterCore;
			ContentContainerAction actionCore;
			public Args(IContentContainerAction action, IContentContainer parameter) {
				ContentContainerAction containerAction = action as ContentContainerAction;
				if(containerAction == null)
					containerAction = GetAction(action);
				this.actionCore = containerAction;
				this.parameterCore = parameter;
			}
			public Args(IDocumentAction action, IContentContainer parameter) {
				Stateless = !(action is IDocumentCheckAction);
				this.actionCore = new CustomDocumentAction(action);
				this.parameterCore = parameter;
			}
			public ContentContainerAction Action { get { return actionCore; } }
			public IContentContainer Parameter { get { return parameterCore; } }
			public bool Stateless { get; private set; }
			public object State { get; set; }
			ContentContainerAction GetAction(IContentContainerAction action) {
				if(action is IContentContainerPopupMenuAction)
					return new ContentContainerPopupMenuAction.CustomContentContainerPopupMenuAction((IContentContainerPopupMenuAction)action);
				if(action is IContentContainerPopupControlAction)
					return new ContentContainerPopupControlAction.CustomContentContainerPopupControlAction((IContentContainerPopupControlAction)action);
				return new CustomContentContainerAction(action);
			}
			sealed class CustomContentContainerAction : ContentContainerAction, IActionBehavior, IActionLayout {
				IContentContainerAction action;
				public CustomContentContainerAction(IContentContainerAction action) {
					this.action = action;
				}
				protected override bool CanExecuteCore(IContentContainer container) {
					return action.Command.CanExecute(container);
				}
				protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
					action.Command.Execute(container);
				}
				protected override string GetCaption() {
					return action.Caption;
				}
				protected override string GetDescription() {
					return action.Description;
				}
				protected override Image GetImage() {
					return action.Image;
				}
				protected override DocumentManagerStringId DescriptionID {
					get { throw new NotSupportedException(); }
				}
				protected override DocumentManagerStringId ID {
					get { throw new NotSupportedException(); }
				}
				ActionBehavior IActionBehavior.Behavior {
					get { return GetActionBehavior(action).Behavior; }
				}
				ActionType IActionLayout.Type {
					get { return GetActionLayout(action).Type; }
				}
				ActionEdge IActionLayout.Edge {
					get { return GetActionLayout(action).Edge; }
				}
			}
			sealed class CustomDocumentAction : ContentContainerAction, IActionStyle {
				IDocumentAction action;
				public CustomDocumentAction(IDocumentAction action) {
					this.action = action;
				}
				static Document GetDocument(IContentContainer container) {
					IDocumentContainer documentContainer = container as IDocumentContainer;
					if(documentContainer != null)
						return documentContainer.Document;
					IDocumentSelector documentSelector = container as IDocumentSelector;
					if(documentSelector != null)
						return documentSelector.SelectedDocument;
					return null;
				}
				protected override bool CanExecuteCore(IContentContainer container) {
					return action.Command.CanExecute(GetDocument(container));
				}
				protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
					action.Command.Execute(GetDocument(container));
				}
				protected override void ExecuteCore(WindowsUIView view, IContentContainer container, object state) {
					ICommand<Document> command = (bool)state ?
						((IUICheckAction<Document>)action).CheckedCommand :
						((IUICheckAction<Document>)action).UncheckedCommand;
					command.Execute(GetDocument(container));
				}
				protected override string GetCaption() {
					return action.Caption;
				}
				protected override string GetDescription() {
					return action.Description;
				}
				protected override Image GetImage() {
					return action.Image;
				}
				protected override DocumentManagerStringId DescriptionID {
					get { throw new NotSupportedException(); }
				}
				protected override DocumentManagerStringId ID {
					get { throw new NotSupportedException(); }
				}
				ActionStyle IActionStyle.Style {
					get { return GetActionStyle(action).Style; }
				}
				object IActionStyle.InitialState {
					get { return GetActionStyle(action).InitialState; }
				}
			}
		}
		internal delegate WindowsUIView GetWindowsUIView();
		internal sealed class Tag {
			WindowsUIView viewCore;
			Args argsCore;
			GetWindowsUIView getWindowsUIViewCore;
			public Tag(GetWindowsUIView getWindowsUIView, Args args) {
				getWindowsUIViewCore = getWindowsUIView;
				argsCore = args;
			}
			public Tag(WindowsUIView view, Args args) {
				viewCore = view;
				argsCore = args;
			}
			public WindowsUIView View {
				get {
					if(viewCore == null)
						viewCore = getWindowsUIViewCore();
					return viewCore;
				}
			}
			public Args Args {
				get { return argsCore; }
			}
		}
		#region Actions
		public static readonly IContentContainerAction Back = new BackAction();
		public static readonly IContentContainerAction Home = new HomeAction();
		public static readonly IContentContainerAction Exit = new ExitAction();
		public static readonly IContentContainerAction Overview = new OverviewAction();
		public static IContentContainerAction CreateDetailAction(Document document) {
			return new DetailAction(document);
		}
		public static IContentContainerAction CreateActivateAction(IContentContainer container) {
			return new ActivateAction(container);
		}
		[ActionGroup("Navigation", ActionType.Navigation, Order = 0, Index = 0, Edge = ActionEdge.Left)]
		class BackAction : ContentContainerAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandBack; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandBackDescription; }
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				return container != null && container.Parent != null;
			}
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				view.Controller.Activate(container.Parent);
			}
		}
		[ActionGroup("Navigation", ActionType.Navigation, Order = 0, Index = 1, Edge = ActionEdge.Left)]
		class HomeAction : ContentContainerAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandHome; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandHomeDescription; }
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				IContentContainer root = BaseContentContainer.GetRoot(container);
				return container != null && container != root && container.Parent != root;
			}
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				view.Controller.Activate(BaseContentContainer.GetRoot(container));
			}
		}
		[ActionGroup("Navigation", ActionType.Navigation, Order = 0, Index = 2)]
		class ActivateAction : ContentContainerAction {
			IContentContainer containerCore;
			public ActivateAction(IContentContainer container) {
				this.containerCore = container;
			}
			protected sealed override DocumentManagerStringId DescriptionID {
				get { throw new NotImplementedException(); }
			}
			protected sealed override DocumentManagerStringId ID {
				get { throw new NotImplementedException(); }
			}
			protected override string GetCaption() {
				return containerCore.Caption;
			}
			protected override Image GetImage() {
				return containerCore.Image ?? Resources.ContentContainterActionResourceLoader.GetImage(containerCore.GetType());
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				return containerCore is IContentContainerInternal;
			}
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				view.Controller.Activate(containerCore);
			}
		}
		[ActionGroup("ContextualZoom", ActionType.Navigation, Order = 1, Index = 0, Edge = ActionEdge.Left)]
		class OverviewAction : ContentContainerAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandOverview; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandOverviewDescription; }
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				IContentContainerInternal contentContainer = container as IContentContainerInternal;
				return (contentContainer != null) && (contentContainer.Count > 1) &&
					(contentContainer.ZoomLevel == ContextualZoomLevel.Normal);
			}
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				view.Controller.Overview(container);
			}
		}
		[ActionGroup("ContextualZoom", ActionType.Navigation, Order = 1, Index = 1, Behavior = ActionBehavior.HideBarOnClick)]
		class DetailAction : ContentContainerAction {
			Document document;
			public DetailAction(Document document) {
				this.document = document;
			}
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandDetail; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandDetailDescription; }
			}
			protected override string GetCaption() {
				return !string.IsNullOrEmpty(document.ActionCaption) ? document.ActionCaption : document.Caption;
			}
			protected override Image GetImage() {
				return (document.GetActualActionImage() ?? document.GetActualImage()) ??
					Resources.ContentContainterActionResourceLoader.GetImage(ID);
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				if(container is IDocumentContainer)
					container = ((IDocumentContainer)container).Parent;
				return (container != null) && (container.Contains(document)) && document.CanActivate();
			}
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				view.Controller.Activate(document);
			}
			internal int CompareTo(DetailAction action, DocumentGroup group) {
				if(document == action.document) return 0;
				int index = group.Items.IndexOf(document);
				int actionIndex = group.Items.IndexOf(action.document);
				if(index == -1 || actionIndex == -1) return 0;
				return index.CompareTo(actionIndex);
			}
		}
		[ActionGroup("Application", ActionType.Navigation, Order = 100, Index = 100)]
		class ExitAction : ContentContainerAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandExit; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandExitDescription; }
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				return container != null;
			}
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				((WindowsUIViewController)view.Controller).Exit();
			}
		}
		#endregion Actions
	}
	[ActionGroup("Skin", ActionType.Default, Behavior = ActionBehavior.HideBarOnClick)]
	public sealed class SetSkinAction : IContentContainerAction, IActionLayout {
		string skinName;
		public SetSkinAction(string skinName)
			: this(skinName, skinName) {
		}
		public SetSkinAction(string skinName, string caption) {
			this.skinName = skinName;
			Edge = ActionEdge.Left;
			Type = ActionType.Context;
			Caption = caption;
			Description = caption;
			Command = new DelegateCommand<IContentContainer>(CanSetSkin, SetSkin);
		}
		bool CanSetSkin(IContentContainer container) {
			return DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveSkinName != skinName;
		}
		void SetSkin(IContentContainer container) {
			DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(skinName);
		}
		public ICommand<IContentContainer> Command { get; private set; }
		public System.Drawing.Image Image { get; set; }
		public string Caption { get; private set; }
		public string Description { get; set; }
		public ActionType Type { get; set; }
		public ActionEdge Edge { get; set; }
	}
	public sealed class AttachedAction : BasePropertiesProvider {
		public static void Execute(Control source) {
			AttachedAction attachedAction = AttachedAction.GetProvider<AttachedAction>(source);
			if(attachedAction != null) {
				IContentContainerAction action;
				IContentContainer target = attachedAction.Target;
				ICommand<IContentContainer> command = attachedAction.Command;
				if(command == null)
					action = ContentContainerAction.CreateActivateAction(target);
				else
					action = new CommandAction(command);
				DocumentManager manager = DocumentManager.FromControl(source);
				if(manager != null) {
					WindowsUIView view = manager.View as WindowsUIView;
					if(view != null) {
						var args = new ContentContainerAction.Args(action, target);
						if(ContentContainerAction.CanExecute(args))
							ContentContainerAction.Execute(view, args);
					}
				}
			}
		}
		#region CommandAction
		sealed class CommandAction : ContentContainerAction {
			ICommand<IContentContainer> command;
			public CommandAction(ICommand<IContentContainer> command) {
				this.command = command;
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				return command.CanExecute(container);
			}
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				command.Execute(container);
			}
			protected override string GetCaption() {
				return null;
			}
			protected override string GetDescription() {
				return null;
			}
			protected override Image GetImage() {
				return null;
			}
			protected override DocumentManagerStringId DescriptionID {
				get { throw new NotSupportedException(); }
			}
			protected override DocumentManagerStringId ID {
				get { throw new NotSupportedException(); }
			}
		}
		#endregion CommandAction
		public IContentContainer Target {
			get { return GetValueCore<IContentContainer>("Target"); }
			set { SetValueCore("Target", value); }
		}
		public ICommand<IContentContainer> Command {
			get { return GetValueCore<ICommand<IContentContainer>>("Command"); }
			set { SetValueCore("Command", value); }
		}
	}
	public enum ActionType { Default, Navigation, Context }
	public enum ActionEdge { Default, Left, Right }
	public enum ActionBehavior { Default, HideBarOnClick, HideFlyoutPanelOnClick, UpdateBarOnClick, UpdateFlyoutPanelOnClick }
	public enum ActionStyle { Default, PushAction, CheckAction }
	public interface IActionLayout {
		ActionType Type { get; }
		ActionEdge Edge { get; }
	}
	public interface IActionBehavior {
		ActionBehavior Behavior { get; }
	}
	public interface IActionStyle {
		ActionStyle Style { get; }
		object InitialState { get; }
	}
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ActionGroupAttribute : Attribute, IActionLayout, IActionBehavior, IActionStyle {
		public ActionGroupAttribute(string name, ActionType type) {
			nameCore = name;
			typeCore = type;
		}
		ActionType typeCore;
		public ActionType Type {
			get { return typeCore; }
		}
		string nameCore;
		public string Name {
			get { return nameCore; }
		}
		int orderCore = -1;
		public int Order {
			get { return orderCore; }
			set { orderCore = value; }
		}
		int indexCore = -1;
		public int Index {
			get { return indexCore; }
			set { indexCore = value; }
		}
		public ActionBehavior Behavior { get; set; }
		public ActionEdge Edge { get; set; }
		public ActionStyle Style { get; set; }
		public object InitialState { get; set; }
		public static readonly ActionGroupAttribute Unknown = new ActionGroupAttribute("Unknown", ActionType.Default);
	}
}
