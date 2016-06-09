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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.FeatureBrowser;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors.Designer.Utils;
namespace DevExpress.XtraEditors.Design.TasksSolution {
	public class StepItemFrame : IEmbeddedFrameOwner, IStepItemFrame {
		StepItem stepItem;
		object sourceObject;
		IEmbeddedFrame stepFrame;
		ITaskRunner taskRunner;
		public StepItemFrame(StepItem stepItem, object sourceObject, ITaskRunner taskRunner) {
			this.stepItem = stepItem;
			this.sourceObject = GetSourceObject(sourceObject);
			this.taskRunner = taskRunner;
			this.stepFrame = CreateFrame();
			if(StepFrame != null) {
				InitFrameControl();
				StepFrame.Control.Parent = taskRunner.Control;
				StepFrame.Control.Dock = DockStyle.Fill;
				StepFrame.Control.BringToFront();
			}
		}
		public void Dispose() {
			if(StepFrame != null && StepFrame is IDisposable)
				(StepFrame as IDisposable).Dispose();
			this.stepFrame = null;
		}
		public StepItem StepItem { get { return stepItem; } }
		public object SourceObject { get { return sourceObject; } }
		protected IEmbeddedFrame StepFrame { get { return stepFrame; } }
		protected IEmbeddedFrame CreateFrame() {
			return this.taskRunner.CreateFrame(StepItem.Frame);
		}
		protected void InitFrameControl() {
			if(StepFrame == null) return;
			EmbeddedFrameInit init = new EmbeddedFrameInit(this, SourceObject, StepItem.Description);
			init.Properties = StepItem.Properties;
			init.ExpandedPropertiesOnStart = StepItem.ExpandedPropertiesOnStart;
			init.ExpandAllProperties = StepItem.ExpandedPropertiesOnStart == null || StepItem.ExpandedPropertiesOnStart.Length == 0;
			init.DescriptionPanelDock = DockStyle.Top;
			StepFrame.InitEmbeddedFrame(init);
			StepFrame.SetPropertyGridSortMode(PropertySort.Alphabetical);		
			StepFrame.ShowPropertyGridToolBar(false);
			StepFrame.RefreshPropertyGrid();
			if(StepItem.Properties != null && StepItem.Properties.Length > 0)
				StepFrame.SelectProperty(StepItem.Properties[0]);
		}
		object GetSourceObject(object sourceObject) {
			ObjectValueGetter valueGetter = new ObjectValueGetter(sourceObject, StepItem.SourceProperty);
			return valueGetter.GetterObject;
		}
		bool IStepItemFrame.CanMoveNext {
			get {
				return new ObjectCondition(SourceObject, StepItem.ReadyCondition).Run();
			}
		}
		void IEmbeddedFrameOwner.SourceObjectChanged(IEmbeddedFrame frame, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			this.taskRunner.SourceObjectChanged();
		}
	}
}
