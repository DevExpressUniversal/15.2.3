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
using System.Web.UI;
namespace DevExpress.ExpressApp.Web.TestScripts {
	public interface IJScriptTestControl {
		string JScriptClassName { get; }
		TestScriptsDeclarationBase ScriptsDeclaration { get; }
	}
	public interface ISupportAdditionalParametersTestControl {
		ICollection<string> GetAdditionalParameters(object control);
	}
	public class ControlInitializedEventArgs : EventArgs {
		private Control control;
		public ControlInitializedEventArgs(Control control) {
			this.control = control;
		}
		public Control Control {
			get {
				return control;
			}
		}
	}
	public interface ITestable  {
		string TestCaption { get; }
		string ClientId { get; }
		event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		IJScriptTestControl TestControl { get; }
		TestControlType TestControlType { get; }
	}
	public interface ITestableEx : ITestable {
		Type RegisterControlType { get; }
	}
	public interface ITestableContainer {
		ITestable[] GetTestableControls();
		event EventHandler TestableControlsCreated;
	}
	public class TestableContolWrapper : ITestable {
		private string caption;
		private string clientId;
		private IJScriptTestControl testControl;
		private TestControlType testControlType;
		public void OnControlInitialized() {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(null));
			}
		}
		public TestableContolWrapper(string caption, string clientId, IJScriptTestControl testControl, TestControlType testControlType) {
			this.caption = caption;
			this.clientId = clientId;
			this.testControl = testControl;
			this.testControlType = testControlType;
		}
		#region ITestable Members
		public string TestCaption {
			get { return caption; }
		}
		public string ClientId {
			get { return clientId; }
		}
		public IJScriptTestControl TestControl {
			get { return testControl; }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public TestControlType TestControlType {
			get {
				return testControlType;
			}
		}
		#endregion
	}
	public interface IDetailFramesContainer {
		IEnumerable<DetailFrameInfo> GetDetailFramesInfo();
	}
	public class DetailFrameInfo : IDisposable {
		public Frame DetailFrame { get; set; }
		public int FrameIndex { get; set; }
		public void Dispose() {
			DetailFrame.Dispose();
		}
	}
}
