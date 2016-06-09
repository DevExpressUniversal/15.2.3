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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Controls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.Win.Core {
	public class ModelBrowserAdapter : NodeObjectAdapter {
		private bool showCollection = true;
		private Boolean IsListMember(Object nodeObject) {
			if(nodeObject is IMemberInfo) {
				IMemberInfo memberInfo = (IMemberInfo)nodeObject;
				return memberInfo.IsList && !ModelNodesGeneratorBase.IsBinaryImage(memberInfo);
			}
			else {
				return false;
			}
		}
		private bool IsDisplayedMember(IMemberInfo memberInfo) {
			if(memberInfo.Owner.KeyMember == memberInfo) {
				return true;
			}
			bool showCollectionModificator = showCollection ? true : !memberInfo.IsList || ModelNodesGeneratorBase.IsBinaryImage(memberInfo);
			return memberInfo.IsPublic && memberInfo.IsVisible && showCollectionModificator &&
				(memberInfo.MemberTypeInfo.IsDomainComponent || memberInfo.Owner.IsDomainComponent);
		}
		private IEnumerable GetDisplayedMembers(IEnumerable<IMemberInfo> members) {
			foreach(IMemberInfo memberInfo in members) {
				if(IsDisplayedMember(memberInfo)) {
					yield return memberInfo;
				}
			}  
		}
		private IEnumerable<IMemberInfo> GetMembers(object nodeObject) {
			IMemberInfo memberInfo = nodeObject as IMemberInfo;
			if(memberInfo != null) {
				if(!IsListMember(nodeObject)) {
					return memberInfo.MemberTypeInfo.Members;
				}
				else {
					return new List<IMemberInfo>();
				}
			}
			else {
				return ((ITypeInfo)nodeObject).Members;
			}
		}
		public override bool HasChildren(object nodeObject) {
			return GetDisplayedMembers(GetMembers(nodeObject)).GetEnumerator().MoveNext();
		}
		public override object GetParent(object nodeObject) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override IEnumerable GetChildren(object nodeObject) {
			return GetDisplayedMembers(GetMembers(nodeObject));
		}
		public override string GetDisplayPropertyValue(object nodeObject) {
			IMemberInfo memberInfo = nodeObject as IMemberInfo;
			if(memberInfo != null) {
				return CaptionHelper.GetLastMemberPartCaption(memberInfo.Owner.Type, memberInfo.Name);
			}
			else {
				return CaptionHelper.GetClassCaption(((ITypeInfo)nodeObject).FullName);
			}
		}
		public string GetDataMemberName(object node) {
			return ((IMemberInfo)node).Name;
		}
		public override string DisplayPropertyName {
			get { return ""; }
		}
		public bool ShowCollection {
			get { return showCollection; }
			set { showCollection = value; }
		}
	}
	public class ModelBrowser : IDisposable {
		private PopupForm form;
		ModelBrowserAdapter adapter = new ModelBrowserAdapter();
		private ObjectTreeList treeView;
		private IList<string> disabledMemberNames;
		private bool IsRootNode(TreeListNode node) {
			return (treeView.Nodes.Count > 0 && node == treeView.Nodes[0]);
		}
		private bool IsMemberDisabled(TreeListNode node) {
			return (disabledMemberNames != null) && (disabledMemberNames.IndexOf(GetDataMemberPath(node)) >= 0);
		}
		private string GetDataMemberPath(TreeListNode node) {
			if(node.ParentNode == null) {
				return "";
			}
			else {
				if(node.ParentNode.ParentNode == null) {
					return adapter.GetDataMemberName(((ObjectTreeListNode)node).Object);
				}
				else {
					return GetDataMemberPath(node.ParentNode) + "." + adapter.GetDataMemberName(((ObjectTreeListNode)node).Object);
				}
			}
		}
		private void treeView_MouseDoubleClick(object sender, MouseEventArgs e) {
			TreeListHitInfo hitInfo = treeView.CalcHitInfo(new Point(e.X, e.Y));
			if(hitInfo.Node != null && hitInfo.Column != null) {
				if(!IsMemberDisabled(hitInfo.Node) && !IsRootNode(hitInfo.Node)) {
					form.DialogResult = DialogResult.OK;
				}
			}
		}
		private void treeView_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e) {			
			if(IsMemberDisabled(e.Node) && !e.Node.Selected) {
				e.Appearance.ForeColor = DevExpress.LookAndFeel.UserLookAndFeel.Default.UseWindowsXPTheme ?
					SystemColors.GrayText : DevExpress.Skins.CommonSkins.GetSkin(DevExpress.LookAndFeel.UserLookAndFeel.Default).GetSystemColor(SystemColors.GrayText);
			}
		}
		private void form_FormClosing(object sender, FormClosingEventArgs e) {
			if(form.DialogResult == DialogResult.OK) {
				foreach(TreeListNode node in treeView.Selection) {
					if(IsMemberDisabled(node) || IsRootNode(node)) {
						e.Cancel = true;
						return;
					}
				}
			}
		}
		private void treeView_BeforeExpand(object sender, BeforeExpandEventArgs e) {
			treeView.SetFocusedNode(e.Node);
		}
		public ModelBrowser(Type objectType, IList<string> disabledMemberNames, bool canAddListProperty) {
			this.disabledMemberNames = disabledMemberNames;
			form = new PopupForm();
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.AutoShrink = false;
			form.ClientSize = new Size(500, 320);
			SimpleAction okAction = new SimpleAction();
			okAction.ActionMeaning = ActionMeaning.Accept;
			okAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "Add", "Add");
			form.ButtonsContainer.Register(okAction);
			SimpleAction cancelAction = new SimpleAction();
			cancelAction.ActionMeaning = ActionMeaning.Cancel;
			cancelAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "Close", "Close");
			form.ButtonsContainer.Register(cancelAction);
			form.FormClosing += new FormClosingEventHandler(form_FormClosing);
			adapter.ShowCollection = canAddListProperty;
			treeView = new ObjectTreeList(adapter);
			treeView.BuildChildNodesRecursive = false;
			treeView.OptionsBehavior.Editable = false;
			treeView.OptionsView.ShowIndicator = false;
			treeView.OptionsView.ShowHorzLines = false;
			treeView.OptionsView.ShowVertLines = false;
			treeView.OptionsBehavior.AllowIncrementalSearch = true;
			treeView.OptionsBehavior.ExpandNodesOnIncrementalSearch = true;
			treeView.OptionsSelection.EnableAppearanceFocusedCell = false;
			XafTypesInfo.Instance.RefreshInfo(objectType);
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(objectType);
			adapter.RootValue = typeInfo;
			treeView.DataSource = typeInfo;
			treeView.MouseDoubleClick += new MouseEventHandler(treeView_MouseDoubleClick);
			treeView.CustomDrawNodeCell += new CustomDrawNodeCellEventHandler(treeView_CustomDrawNodeCell);
			treeView.BeforeExpand += new BeforeExpandEventHandler(treeView_BeforeExpand);
			form.AddControl(treeView, CaptionHelper.GetLocalizedText("Captions", "ObjectModel", "Object Model"));
		}
		public ModelBrowser(Type objectType, IList<string> disabledMemberNames) : this(objectType, disabledMemberNames, true) { }
		public ModelBrowser(Type objectType) : this(objectType, null) { }
		public void Dispose() {
			if(treeView != null) {
				treeView.CustomDrawNodeCell -= new CustomDrawNodeCellEventHandler(treeView_CustomDrawNodeCell);
				treeView.MouseDoubleClick -= new MouseEventHandler(treeView_MouseDoubleClick);
				treeView.BeforeExpand -= new BeforeExpandEventHandler(treeView_BeforeExpand);
				treeView.Dispose();
				treeView = null;
			}
			if(form != null) {
				form.FormClosing -= new FormClosingEventHandler(form_FormClosing);
				form.Dispose();
				form = null;
			}
		}
		public bool ShowDialog() {
			return ShowDialog(null);
		}
		public bool ShowDialog(IWin32Window owner) {
			return form.ShowDialog(owner) == DialogResult.OK;
		}
		public string SelectedMember {
			get {
				return GetDataMemberPath(treeView.FocusedNode);
			}
		}
		public string SelectedMemberCaption {
			get {
				return adapter.GetDisplayPropertyValue(treeView.FocusedObject);
			}
		}
		public bool MultiSelect {
			get { return treeView.OptionsSelection.MultiSelect; }
			set { treeView.OptionsSelection.MultiSelect = value; }
		}
		public IEnumerable<string> SelectedMembers {
			get {
				foreach(TreeListNode node in treeView.Selection) {
					yield return GetDataMemberPath(node);
				}
			}
		}
		internal ObjectTreeList FieldsTreeView {
			get { return treeView; }
		}
	}
}
