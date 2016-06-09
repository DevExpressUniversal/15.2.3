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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	[ToolboxItem(false)] 
	[ParseChildren(true)]
	[Designer("DevExpress.ExpressApp.Web.Design.NavigationTabsActionContainerDesigner, DevExpress.ExpressApp.Web.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.ComponentModel.Design.IDesigner))]
	public class NavigationTabsActionContainer : Panel, IActionContainer, ITestableEx, IShowImages, INamingContainer
#if DebugTest
, ISupportNavigationActionContainerTesting
#endif
 {
		private ASPxPageControlHelper helper;
		private string name;
		private ASPxPageControl pageControl;
		private ActionItemPaintStyle imageTextStyle = ActionItemPaintStyle.Default;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void CreateDesignTimeContent() {
			if(DesignMode) {
				if(!Controls.Contains(pageControl)) {
					Controls.Add(pageControl);
				}
				SingleChoiceAction navigationAction = new SingleChoiceAction(null, "Navigation", "");
				ChoiceActionItem group1 = new ChoiceActionItem("Common", "");
				group1.Items.Add(new ChoiceActionItem("Contacts", ""));
				group1.Items.Add(new ChoiceActionItem("Tasks", ""));
				ChoiceActionItem group2 = new ChoiceActionItem("Mail", "");
				group2.Items.Add(new ChoiceActionItem("Inbox", ""));
				group2.Items.Add(new ChoiceActionItem("Drafts", ""));
				group2.Items.Add(new ChoiceActionItem("Outbox", ""));
				group2.Items.Add(new ChoiceActionItem("Sent Items", ""));
				ChoiceActionItem group3 = new ChoiceActionItem("Calendar", "");
				group3.Items.Add(new ChoiceActionItem("Appointments", ""));
				group3.Items.Add(new ChoiceActionItem("Events", ""));
				group3.Items.Add(new ChoiceActionItem("Meetings", ""));
				navigationAction.Items.AddRange(new ChoiceActionItem[] { group1, group2, group3 });
				Register(navigationAction);
			}
		}
		protected override void OnUnload(EventArgs e) {
			OnControlInitialized(this);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		public override void Dispose() {
			if(helper != null) {
				helper.Dispose();
				helper = null;
			}
			if(pageControl != null) {
				pageControl.Init -= pageControl_Init;
				pageControl = null;
			}
			base.Dispose();
		}
		[DefaultValue(GroupingStyle.GroupItemsIntoTabs)]
		[Category("Layout")]
		public GroupingStyle GroupingStyle {
			get { return helper.GroupingStyle; }
			set { helper.GroupingStyle = value; }
		}
		[Category("Layout")]
		[DefaultValue(false)]
		public bool ShowImages {
			get { return helper.ShowTabsImages; }
			set { helper.ShowTabsImages = value; }
		}
		public ActionItemPaintStyle ImageTextStyle {
			get { return imageTextStyle; }
			set {
				imageTextStyle = value;
				helper.ShowImages = ImageTextStyle == ActionItemPaintStyle.Default || ImageTextStyle == ActionItemPaintStyle.CaptionAndImage || ImageTextStyle == ActionItemPaintStyle.Image;
			}
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public TabPageCollection TabPages {
			get { return pageControl.TabPages; }
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public ASPxPageControl PageControl {
			get { return pageControl; }
		}
		public new string SkinID {
			get { return pageControl.SkinID; }
			set { pageControl.SkinID = value; }
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		new public ControlCollection Controls {
			get { return base.Controls; }
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public int ActiveTabIndex {
			get { return pageControl.ActiveTabIndex; }
			set { pageControl.ActiveTabIndex = value; }
		}
		public NavigationTabsActionContainer() {
			InitializeComponent();
		}
		private void InitializeComponent() {
			pageControl = RenderHelper.CreateASPxPageControl();
			pageControl.ID = "PC";
			pageControl.Width = Unit.Percentage(100);
			pageControl.Init += pageControl_Init;
			this.helper = new ASPxPageControlHelper(pageControl);
			pageControl.SaveStateToCookies = false;
			Controls.Add(pageControl);
		}
		private void pageControl_Init(object sender, EventArgs e) {
			WebActionContainerHelper.TryRegisterActionContainer(this, new IActionContainer[] { this });		
		}
		[TemplateContainer(typeof(TemplateContainerBase))]
		[DefaultValue("")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ITemplate SpaceAfterTabsTemplate {
			set {
				PageControl.SpaceAfterTabsTemplate = value;
			}
			get {
				return PageControl.SpaceAfterTabsTemplate;
			}
		}
		#region IActionContainer Members
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyCollection<ActionBase> Actions {
			get { return new ReadOnlyCollection<ActionBase>(new ActionBase[] { helper.Navigation }); }
		}
		[DefaultValue(null), TypeConverter(typeof(DevExpress.ExpressApp.Core.Design.ContainerIdConverter)), Category("Design")]
		public string ContainerId {
			get { return name; }
			set {
				name = value;
			}
		}
		public void Register(ActionBase action) {
			SingleChoiceAction singleChoiceAction = action as SingleChoiceAction;
			if(singleChoiceAction != null) {
				helper.Navigation = singleChoiceAction;
			}
		}
		#endregion
		#region ITestable Members
		string ITestable.TestCaption {
			get { return helper.Navigation != null ? helper.Navigation.Caption : string.Empty; }
		}
		IJScriptTestControl ITestable.TestControl {
			get { return null; }
		}
		string ITestable.ClientId {
			get { return ClientID; }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Action;
			}
		}
		#endregion
		#region ISupportUpdate Members
		public virtual void BeginUpdate() {
		}
		public virtual void EndUpdate() {
		}
		#endregion
#if DebugTest
		#region ISupportNavigationActionContainerTesting Members
		bool ISupportNavigationActionContainerTesting.IsItemControlVisible(ChoiceActionItem item) {
			return helper.IsItemControlVisible(item);
		}
		int ISupportNavigationActionContainerTesting.GetGroupCount() {
			return TabPages.Count;
		}
		string ISupportNavigationActionContainerTesting.GetGroupControlCaption(ChoiceActionItem item) {
			return helper.GetGroupControlCaption(item);
		}
		int ISupportNavigationActionContainerTesting.GetGroupChildControlCount(ChoiceActionItem item) {
			return helper.GetGroupChildControlCount(item);
		}
		string ISupportNavigationActionContainerTesting.GetChildControlCaption(ChoiceActionItem item) {
			return helper.GetChildControlCaption(item);
		}
		bool ISupportNavigationActionContainerTesting.GetChildControlEnabled(ChoiceActionItem item) {
			return helper.GetChildControlEnabled(item);
		}
		bool ISupportNavigationActionContainerTesting.GetChildControlVisible(ChoiceActionItem item) {
			return helper.GetChildControlVisible(item);
		}
		bool ISupportNavigationActionContainerTesting.IsGroupExpanded(ChoiceActionItem item) {
			return true;
		}
		string ISupportNavigationActionContainerTesting.GetSelectedItemCaption() {
			return "";
		}
		INavigationControlTestable ISupportNavigationActionContainerTesting.NavigationControl {
			get { return (INavigationControlTestable)this; }
		}
		#endregion
#endif
		#region ITestableEx Members
		public Type RegisterControlType {
			get { return GetType(); }
		}
		#endregion
	}
	public enum GroupingStyle { GroupItemsIntoTabs, ShowItemsAsTabs }
	public interface IShowImages {
		bool ShowImages { get; }
	}
}
