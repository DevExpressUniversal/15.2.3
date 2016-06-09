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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.EasyTest;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Win.EasyTest {
	public class WindowControlFinderController : WindowController, IDiagnosticInfoProvider, IControlFinder, IControlsEnumeration {
		private static FrameControlFinder frameControlFinder = new FrameControlFinder();
		SingleChoiceAction diagnosticInfo;
		ChoiceActionItem diagnosticActionItem;
		public WindowControlFinderController() {
			diagnosticActionItem = new ChoiceActionItem("EasyTest Control", null);
			diagnosticActionItem.Active["ControllerActive"] = false;
			diagnosticInfo = new SingleChoiceAction(this, "Diagnostic Info." + diagnosticActionItem.Id, "Diagnostic");
			diagnosticInfo.Active.SetItemValue("Fake action for localization", false);
			diagnosticInfo.Items.Add(diagnosticActionItem);
		}
		protected override void OnActivated() {
			base.OnActivated();
			ControlFinder.Instance.RegisterController(this);
			diagnosticActionItem.Active["ControllerActive"] = true;
		}
		protected override void OnDeactivated() {
			diagnosticActionItem.Active["ControllerActive"] = false;
			ControlFinder.Instance.UnregisterController(this);
			base.OnDeactivated();
		}
		private string FormatXml(XmlNode xmlNode) {
			StringBuilder sb = new StringBuilder();
			using(StringWriter stringWriter = new StringWriter(sb)) {
				using(XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter)) {
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlNode.WriteTo(xmlTextWriter);
				}
			}
			return sb.ToString();
		}
		#region ITestControlsFinder Members
		public object Find(Form activeForm, string contolType, string caption) {
			if(Window.Template == activeForm) {
				return frameControlFinder.Find(Window, contolType, caption);
			}
			return null;
		}
		#endregion
		#region IDiagnosticInfoProvider Members
		public ChoiceActionItem DiagnosticActionItem {
			get {
				return diagnosticActionItem;
			}
		}
		public string GetDiagnosticInfoObjectString() {
			return FormatXml(frameControlFinder.CreateDiagnosticInfoNode(Window));
		}
		#endregion
		#region IControlCollector Members
		public IDictionary<int, string> CollectAllControls() {
			Dictionary<int, string> allControls = new Dictionary<int, string>();
			Add(allControls, frameControlFinder.GetControlCaptions(Frame, ControlFinder.ControlTypeField));
			Add(allControls, frameControlFinder.GetControlCaptions(Frame, ControlFinder.ControlTypeAction));
			return allControls;
		}
		private void Add(IDictionary<int, string> target, IDictionary<int, string> source) {
			foreach(KeyValuePair<int, string> pair in source) {
				target.Add(pair.Key, pair.Value);
			}
		}
		#endregion
	}
}
