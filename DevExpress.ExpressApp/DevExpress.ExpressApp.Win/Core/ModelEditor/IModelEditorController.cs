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
using System.Collections.Specialized;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[Flags]
	public enum SearchNodeOptions { None = 0, WholeWord = 1, CaseSensitive = 2, SearchUp = 4 };
	public class NodeDeletingEventArgs : EventArgs {
		public NodeDeletingEventArgs(ModelNode node) {
			this.Node = node;
		}
		public ModelNode Node;
	}
	public class CustomLoadModelEventArgs : EventArgs {
		private DevExpress.ExpressApp.Model.IModelApplication modelApplication;
		private ModelDifferenceStore modelDifferenceStore;
		public DevExpress.ExpressApp.Model.IModelApplication ModelApplication {
			get {
				return modelApplication;
			}
			set {
				modelApplication = value;
			}
		}
		public ModelDifferenceStore ModelDifferenceStore {
			get {
				return modelDifferenceStore;
			}
			set {
				modelDifferenceStore = value;
			}
		}
	}
	public interface IModelEditorController : IDisposable {
		bool ShowCulturesManager(string newValue);
		void ShowLocalizationForm();
		void RefreshCurrentAttributeValue();
		void SetCurrentAspectByName(string aspectName);
		void Back();
		void Forward();
		bool TrySetModified();
		bool Save();
		List<string> AspectNames { get; }
		List<string> AllAspectNames { get; }
		string CurrentAspect { get; }
		ModelTreeListNode CurrentModelNode { get; set; }
		bool ReadOnly { get; set; }
		bool DesignMode { get; set; }
		bool IsStandalone { get; set; }
		bool IsModified { get; }
		bool CanBack { get; }
		bool CanForward { get; }
		bool CanShowLocalizationForm { get; }
		bool CanChangeAspect { get; }
		IList<string> LastSavedFiles { get; }
		bool FindAndFocusEntry(bool nextEntry, string text, SearchNodeOptions searchNodeOptions);
		void ReloadModel(bool askConfirmation, bool refreshAdapter);
		void CalculateUnusableModel();
		bool MergeDiffsMode { get; set; }
		void SetModuleDiffStore(List<ModuleDiffStoreInfo> modulesDiffStoreInfo);
		SingleChoiceAction ChooseMergeModuleAction { get; }
		SimpleAction MergeDifferencesAction { get; }
		bool IsDisposing { get; set; }
		event EventHandler CurrentNodeChanged;
		event EventHandler CurrentAspectChanged;
		event EventHandler<CustomLoadModelEventArgs> CustomLoadModel;
		event CancelEventHandler Modifying;
		event EventHandler<FileModelStoreCancelEventArgs> CanSaveBoFiles;
		void SaveSettings();
		void LoadSettings();
		bool PreProcessMessage(Message m);
		DevExpress.ExpressApp.Model.IModelApplication ModelApplication { get; }
	}
}
