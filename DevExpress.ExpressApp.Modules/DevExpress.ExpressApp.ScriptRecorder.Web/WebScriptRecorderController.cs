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
using System.Text;
using System.Web;
using System.IO;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Web.SystemModule;
namespace DevExpress.ExpressApp.ScriptRecorder.Web {
	public class WebScriptRecorderController : ScriptRecorderControllerBase {
		private PropertyEditorListenersFactoryBase propertyEditorsFactory = null;
		protected override PropertyEditorListenersFactoryBase PropertyEditorListenersFactory {
			get {
				if (propertyEditorsFactory == null) {
					propertyEditorsFactory = new WebPropertyEditorListenersFactory();
				}
				return propertyEditorsFactory;
			}
		}
		protected override void SaveScript(string script) {
			ResponseWriter.WriteTextFileToResponse("Test.ets", script);
		}
		protected override string ApplicationName {
			get {
				return Frame.Application.ApplicationName + "Web";
			}
		}
		protected override ScriptRecorderActionsListenerBase CreateActionsListener(List<ActionBase> actions) {
			return new WebActionsListener(actions);
		}
	}
	public class ActionNodeGeneratorUpdater : ModelNodesGeneratorUpdater<ModelActionsNodesGenerator> {
		private List<string> actionsId = new List<string>();
		public ActionNodeGeneratorUpdater() {
			Initialize();
		}
		private void Initialize() {
			ScriptRecorderControllerBase controller = new ScriptRecorderControllerBase();
			foreach(ActionBase action in controller.Actions) {
				if(action.Category == ScriptRecorderControllerBase.ActionContainerName) {
					actionsId.Add(action.Id);
				}
			}
		}
		public override void UpdateNode(ModelNode node) {
			foreach (IModelAction action in ((IModelActions)node)) {
				if (actionsId.IndexOf(action.Id) != -1) {
					action.Category = PredefinedCategory.Tools.ToString();
				}
			}
		}
	}
}
