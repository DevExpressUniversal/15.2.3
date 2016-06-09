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
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Templates.Bars.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Ribbon.ActionControls;
using DevExpress.Persistent.Base;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.ScriptRecorder.Win {
	public class WinScriptRecorderController : ScriptRecorderControllerBase {
		private SimpleAction easyTestRecorderAction;
		private PropertyEditorListenersFactoryBase propertyEditorsFactory = null;
		public WinScriptRecorderController() {
			easyTestRecorderAction = new SimpleAction();
			easyTestRecorderAction.ImageName = "ActionGroup_EasyTestRecorder";
			easyTestRecorderAction.Id = "EasyTestRecorder";
			easyTestRecorderAction.Caption = "EasyTest Recorder";
			easyTestRecorderAction.Category = ActionContainerName;
			easyTestRecorderAction.Active.SetItemValue("DummyAction", false);
			RegisterActions(easyTestRecorderAction);
		}
		protected override PropertyEditorListenersFactoryBase PropertyEditorListenersFactory {
			get {
				if(propertyEditorsFactory == null) {
					propertyEditorsFactory = new WinPropertyEditorListenersFactory();
				}
				return propertyEditorsFactory;
			}
		}
		protected override void SaveScript(string script) {
			using(SaveFileDialog dialog = new SaveFileDialog()) {
				dialog.AddExtension = true;
				dialog.ValidateNames = true;
				dialog.RestoreDirectory = true;
				dialog.OverwritePrompt = true;
				dialog.CreatePrompt = false;
				dialog.Filter = "ets files (*.ets)|*.ets";
				if(dialog.ShowDialog(Form.ActiveForm) == DialogResult.OK) {
					WriteScriptToFile(dialog.FileName, script);
				}
			}
		}
		protected override ScriptRecorderActionsListenerBase CreateActionsListener(List<ActionBase> actions) {
			return new WinActionsListener(actions);
		}
		protected override string ApplicationName {
			get {
				return Frame.Application.ApplicationName + "Win";
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Active) { 
				if(Frame != null) {
					CustomizeRecordActionContainer(Frame.Template);
				}
				Frame.Application.CustomizeTemplate -= new EventHandler<CustomizeTemplateEventArgs>(Application_CustomizeTemplate);
				Frame.Application.CustomizeTemplate += new EventHandler<CustomizeTemplateEventArgs>(Application_CustomizeTemplate);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(Frame != null) {
				CustomizeRecordActionContainer(Frame.Template);
			}
			Frame.Application.CustomizeTemplate -= new EventHandler<CustomizeTemplateEventArgs>(Application_CustomizeTemplate);
		}
		protected void CustomizeRecordActionContainer(IFrameTemplate template) {
			if(template == null ||
				((Control)template).Disposing) { 
				return;
			}
			BarLinksHolder container = GetToolsContainer(template);
			if(container != null) {
				BarItemLink easyTestRecorder = GetEasyTestRecorderBarItem(container);
				if(easyTestRecorder == null && Active) {
					CreateRecordActionContainer(container);
				}
				else if(!Active && easyTestRecorder != null) {
					container.RemoveLink(easyTestRecorder);
				}
			}
		}
		protected virtual void CreateRecordActionContainer(BarLinksHolder container) {
			BarSubItem easyTestRecorderGroupItem = new BarSubItem();
			easyTestRecorderGroupItem.Tag = easyTestRecorderAction.Tag;
			easyTestRecorderGroupItem.Manager = container.Manager;
			easyTestRecorderGroupItem.Caption = easyTestRecorderAction.Caption;
			easyTestRecorderGroupItem.Glyph = ImageLoader.Instance.GetImageInfo(easyTestRecorderAction.ImageName).Image;
			easyTestRecorderGroupItem.MergeType = BarMenuMerge.Replace;
			ImageInfo largeImageInfo = ImageLoader.Instance.GetLargeImageInfo(easyTestRecorderAction.ImageName);
			if(!largeImageInfo.IsEmpty) {
				easyTestRecorderGroupItem.LargeGlyph = largeImageInfo.Image;
			}
			container.AddItem(easyTestRecorderGroupItem);
			ActionContainerBarItem newCont = new ActionContainerBarItem();
			newCont.ContainerId = "EasyTestRecorder";
			newCont.Manager = container.Manager;
			newCont.Register(PauseRecordAction);
			newCont.Register(StartRecordAction);
			newCont.Register(ShowScriptAction);
			newCont.Register(SaveScriptAction);
			easyTestRecorderGroupItem.AddItem(newCont);
		}
		protected override List<ActionBase> CollectAllActions {
			get {
				Breach();
				return base.CollectAllActions;
			}
		}
		private void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
			CustomizeRecordActionContainer(e.Template);
		}
		private void WriteScriptToFile(string fileName, string script) {
			using(FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
				StreamWriter sw = new StreamWriter(new MemoryStream());
				sw.WriteLine(script);
				sw.Flush();
				BinaryReader binaryReader = new BinaryReader(sw.BaseStream, System.Text.Encoding.UTF8);
				sw.BaseStream.Position = 0;
				byte[] bytes = binaryReader.ReadBytes(System.Convert.ToInt32(sw.BaseStream.Length));
				stream.Position = stream.Length;
				stream.Write(bytes, 0, bytes.Length);
			}
		}
		#region TODO Minakov B146141
		private static List<int> rootFramesIndex = new List<int>();
		private void Breach() {
			List<ActionBase> actions = new List<ActionBase>();
			if(Frame.Application != null && Frame.Application.MainWindow != null) {
				if(((IModelOptionsWin)Frame.Application.Model.Options).UIType == UIType.StandardMDI ||
					((IModelOptionsWin)Frame.Application.Model.Options).UIType == UIType.TabbedMDI) {
					if(!rootFramesIndex.Contains(Frame.Application.MainWindow.GetHashCode())
						&& Frame != Frame.Application.MainWindow) {
						rootFramesIndex.Add(Frame.Application.MainWindow.GetHashCode());
						foreach(Controller controller in Frame.Application.MainWindow.Controllers) {
							if(!(controller is ScriptRecorderControllerBase)) {
								actions.AddRange(controller.Actions);
							}
						}
						CreateActionsListener(actions);
					}
				}
			}
		}
		#endregion
		private BarLinksHolder GetToolsContainer(IFrameTemplate template) {
			string ToolsCategoryId = PredefinedCategory.Tools.ToString();
			foreach(IActionContainer container in template.GetContainers()) {
				if(container.ContainerId == ToolsCategoryId && container is BarLinksHolder) {
					return (BarLinksHolder)container;
				}
			}
			IActionControlsSite actionControlsSite = template as IActionControlsSite;
			if(actionControlsSite != null) {
				foreach(IActionControlContainer container in actionControlsSite.ActionContainers) {
					if(container.ActionCategory == ToolsCategoryId) {
						if(container is BarLinkActionControlContainer) {
							return ((BarLinkActionControlContainer)container).BarContainerItem;
						}
						if(container is RibbonGroupActionControlContainer) {
							return ((RibbonGroupActionControlContainer)container).RibbonGroup.ItemLinks;
						}
					}
				}
			}
			return null;
		}
		private BarItemLink GetEasyTestRecorderBarItem(BarLinksHolder container) {
			foreach(BarItemLink item in container.ItemLinks) {
				if(item.Caption == easyTestRecorderAction.Caption) {
					return item;
				}
			}
			return null;
		}
#if DebugTest
		public BarLinksHolder DebugTest_GetToolsContainer(IFrameTemplate template) {
			return GetToolsContainer(template);
		}
		public BarItemLink DebugTest_GetEasyTestRecorderBarItem(BarLinksHolder container) {
			return GetEasyTestRecorderBarItem(container);
		}
#endif
	}
}
