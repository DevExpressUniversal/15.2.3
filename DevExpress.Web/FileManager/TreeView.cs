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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Web.Internal;
using DevExpress.Web;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class FileManagerFolders : ASPxTreeView {
		const string CallbackArgsSeparator = "|";
		ASPxFileManager fileManager;
		TreeViewDataMediator mediator;
		public FileManagerFolders(ASPxFileManager fileManager, bool isMoving)
			: base(fileManager) {
			this.fileManager = fileManager;
			ClientIDHelper.EnableClientIDGeneration(this);
			IsMoving = isMoving;
			Initialize();
		}
		public ASPxFileManager FileManager { get { return fileManager; } }
		public bool IsMoving { get; private set; }
		public string CreateNodePath { get; set; }
		public string CreateNodeParentName { get; set; }
		public string CallbackTargetFolderPath { get; set; }
		public void RepopulateTree(bool skipSync) {
			if(IsVirtualMode()) {
				FileManager.Helper.Data.ResetFilePathIndexes();
				RefreshVirtualTree();
				if(!IsMoving && !skipSync)
					FileManager.Helper.Data.SyncFolders(this);
			}
			else
				FileManager.Helper.Data.CreateFolders(this, IsMoving);
		}
		protected void Initialize() {
			EnableClientSideAPI = true;
			EnableCallBacks = IsFoldersCallbacksEnabled();
			if(IsFoldersCallbacksEnabled())
				VirtualModeCreateChildren += FileManagerFolders_VirtualModeCreateChildren;
			AccessibilityCompliant = FileManager.AccessibilityCompliant;
			AllowSelectNode = true;
			EnableViewState = false;
			ShowTreeLines = FileManager.SettingsFolders.ShowTreeLines;
			SyncSelectionMode = SyncSelectionMode.None;
			ShowExpandButtons = FileManager.SettingsFolders.ShowExpandButtons;
		}
		void FileManagerFolders_VirtualModeCreateChildren(object source, TreeViewVirtualModeCreateChildrenEventArgs e) {
			FileManager.Helper.Data.PopulateFolders(this, e);
		}
		protected virtual bool IsFoldersCallbacksEnabled() {
			return FileManager.SettingsFolders.EnableCallBacks && !DesignMode;
		}
		protected override void PrepareControlHierarchy() {
			FileManagerFileStyle folderStyle = FileManager.Helper.GetFolderStyle();
			Styles.Node.Assign(folderStyle);
			if(!folderStyle.ForeColor.IsEmpty)
				Styles.NodeText.ForeColor = folderStyle.ForeColor;
			FileManager.Helper.GetFoldersTreeViewStyle().AssignToControl(this);
			base.PrepareControlHierarchy();
		}
		protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
			bool res = base.LoadPostData(postCollection);
			if(FileManager.Helper.Data.NeedResetToInitialFolder)
				FileManager.Helper.Data.SelectFolder(this, new FileManagerFolder(FileManager.FileSystemProvider, FileManager.Settings.InitialFolder), false);
			return res;
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.FileManagerTreeView";
		}
		protected internal object GetCallbackResult(string callbackArgs) {
			int separatorIndex = callbackArgs.IndexOf(CallbackArgsSeparator);
			CallbackTargetFolderPath = callbackArgs.Substring(0, separatorIndex);
			string treeViewArgs = callbackArgs.Substring(separatorIndex + 1);
			RaiseCallbackEvent(treeViewArgs);
			FileManager.Helper.Data.SetVirtualNodesEnabled(CallbackNodes);  
			return GetCallbackResult();
		}
		protected override TreeViewDataMediator DataMediator {
			get {
				if(IsVirtualMode()) {
					if(this.mediator == null)
						this.mediator = new FileManagerVirtualModeTreeViewDataMediator(this);
					return this.mediator;
				}
				return base.DataMediator;
			}
		}
	}
	public class FileManagerVirtualModeTreeViewDataMediator : VirtualModeTreeViewDataMediator {
		public FileManagerVirtualModeTreeViewDataMediator(ASPxTreeView treeView)
			: base(treeView) { }
		protected List<string> LegacyExpandedNodeNames = new List<string>();
		protected string LegacySelectedNodeName = null;
		protected internal override void SyncNodesWithMediator(string parentNodeID, TreeViewNodeCollection nodes) {
			foreach(TreeViewVirtualNode node in nodes) {
				string nodeID = node.GetID();
				if(NodeNames.ContainsValue(node.Name) && (NodeNames[nodeID] as string) != node.Name) {
					ResetDescendantsState(parentNodeID);
					SyncNodesWithMediator(parentNodeID, nodes);
					RestoreLegacyNodesState();
					return;
				}
			}
			base.SyncNodesWithMediator(parentNodeID, nodes);
		}
		void ResetDescendantsState(string parentNodeID) {
			LegacyExpandedNodeNames.Clear();
			LegacySelectedNodeName = null;
			var states = new List<Hashtable>() { NodeNames, ExpandedState, CheckedState };
			var legacyIDs = new List<string>();
			var parentChain = GetNodeIndexChain(parentNodeID);
			foreach(string nodeID in NodeNames.Keys) {
				var nodeChain = GetNodeIndexChain(nodeID);
				if(nodeChain.Length > parentChain.Length && IsParentChain(nodeChain, parentChain))
					legacyIDs.Add(nodeID);
			}
			foreach(string legacyID in legacyIDs) {
				var legacyName = NodeNames[legacyID].ToString();
				if(GetNodeExpanded(legacyID))
					LegacyExpandedNodeNames.Add(legacyName);
				ExpandedState.Remove(legacyID);
				CheckedState.Remove(legacyID);
				NodeNames.Remove(legacyID);
				if(SelectedNodeID == legacyID)
					LegacySelectedNodeName = legacyName;
			}
		}
		void RestoreLegacyNodesState() {
			foreach(DictionaryEntry nodeName in NodeNames) {
				if(LegacyExpandedNodeNames.Contains(nodeName.Value.ToString()))
					SetNodeExpanded(nodeName.Key.ToString(), true);
				if(LegacySelectedNodeName != null && LegacySelectedNodeName == nodeName.Value.ToString())
					SelectedNodeID = nodeName.Key.ToString();
			}
			LegacySelectedNodeName = null;
			LegacyExpandedNodeNames.Clear();
		}
		string[] GetNodeIndexChain(string nodeID) {
			nodeID = nodeID.Substring(TreeViewNode.NodeIDPrefix.Length);
			if(nodeID == "-1")
				return new string[0];
			return nodeID.Split(TreeViewNode.IndexPathSeparator);
		}
		bool IsParentChain(string[] chain, string[] parentChain) {
			for(int i = 0; i < parentChain.Length; i++) {
				if(chain[i] != parentChain[i])
					return false;
			}
			return true;
		}
	}
}
