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
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.PivotChart {
	public abstract class AnalysisViewControllerBase : ViewController {
		protected AnalysisEditorBase analysisEditor;
		private void analysisEditor_IsDataSourceReadyChanged(Object sender, EventArgs e) {
			UpdateActionState();
		}
		private void analysisEditor_ControlValueChanged(Object sender, EventArgs e) {
			ObjectSpace.SetModified(((DetailView)View).CurrentObject);
		}
		private void View_ControlsCreated(Object sender, EventArgs e) {
			OnAnalysisControlCreated();
		}
		protected override void OnActivated() {
			base.OnActivated();
			InitAnalysisEditor();
			if(View.IsControlCreated) {
				OnAnalysisControlCreated();
			}
			else {
				View.ControlsCreated += new EventHandler(View_ControlsCreated);
			}
		}
		protected override void OnDeactivated() {
			if(analysisEditor != null) {
				analysisEditor.IsDataSourceReadyChanged -= new EventHandler<EventArgs>(analysisEditor_IsDataSourceReadyChanged);
				analysisEditor.ControlValueChanged -= new EventHandler(analysisEditor_ControlValueChanged);
			}
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			analysisEditor = null;
			base.OnDeactivated();
		}
		protected virtual void InitAnalysisEditor() {
			analysisEditor = null;
			IList<AnalysisEditorBase> analysisEditors = ((DetailView)View).GetItems<AnalysisEditorBase>();
			if(analysisEditors.Count == 1) {
				analysisEditor = analysisEditors[0];
			}
		}
		protected virtual void OnAnalysisControlCreated() {
			UpdateActionState();
			if(analysisEditor != null) {
				analysisEditor.IsDataSourceReadyChanged += new EventHandler<EventArgs>(analysisEditor_IsDataSourceReadyChanged);
				analysisEditor.ControlValueChanged += new EventHandler(analysisEditor_ControlValueChanged);
			}
		}
		protected virtual void UpdateActionState() { }
		protected bool IsDataSourceReady {
			get {
				return analysisEditor != null && analysisEditor.IsDataSourceReady;
			}
		}
		public AnalysisViewControllerBase()
			: base() {
			this.TargetObjectType = typeof(IAnalysisInfo);
			this.TargetViewType = ViewType.DetailView;
		}
	}
}
