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
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors.Designer.Utils;
namespace DevExpress.XtraEditors.Design.TasksSolution {
	public interface ITaskRunner {
		void SourceObjectChanged();
		Control Control { get; }
		IEmbeddedFrame CreateFrame(string frameName);
	}
	public interface IStepItemFrame : IDisposable {
		bool CanMoveNext {get;}
	}
	[ToolboxItem(false)]
	public class TaskRunner  : System.Windows.Forms.UserControl, ITaskRunner {
		TaskItem taskItem;
		ArrayList hiddenControls;
		ArrayList steps;
		object sourceObject;
		int stepIndex;
		Hashtable stepFrames;
		Panel pnlTop;
		Label lbTop;
		Panel pnlBottom;
		SimpleButton btnPrev, btnNext;
		IStepItemFrame stepItemFrame;
		public TaskRunner(TaskItem taskItem, object sourceObject, Control parent, Hashtable stepFrames) {
			this.stepFrames = stepFrames;
			Dock = DockStyle.Fill;
			Visible = false;
			this.taskItem = taskItem;
			this.sourceObject = sourceObject;
			Parent = parent;
			this.hiddenControls = null;
			this.steps = new ArrayList();
			this.stepIndex = 0;
			this.stepItemFrame = null;
			CreateControls();
		}
		public TaskItem TaskItem { get { return taskItem; } }
		public object SourceObject { get { return sourceObject; } }
		public void Run() {
			PrepareSteps();
			if(StepCount == 0) return;
			ShowForm(true);
			ShowStep();
		}
		protected int StepIndex { get { return stepIndex; } }
		protected int StepCount { get { return steps.Count + (TaskItem.Event != null ? 1 : 0); } }
		protected StepItem GetStepItem(int index) {{ return this.steps[index] as StepItem;  } }
		protected void Finish(bool proccess) {
			ShowForm(false);
			if(proccess && TaskItem.Event != null) {
				if(ParentForm != null)
					ParentForm.Close();
				new EventCreator(SourceObject, TaskItem.Event).Generate();
			} else Dispose();
		}
		void ShowForm(bool show) {
			if(this.hiddenControls == null) {
				this.hiddenControls = new ArrayList();
				for(int i = 0; i < Parent.Controls.Count; i ++)
					if(Parent.Controls[i].Visible)
						this.hiddenControls.Add(Parent.Controls[i]);
			}
			this.Visible = show;
			for(int i = 0; i < this.hiddenControls.Count; i ++)
				(this.hiddenControls[i] as Control).Visible = !show;
		}
		void PrepareSteps() {
			for(int i = 0; i < TaskItem.Count; i ++)
				AddSteps(TaskItem[i]);
		}
		void AddSteps(StepItem stepItem) {
			if(!CheckStartCondition(stepItem)) return;
			for(int i = 0; i < stepItem.Count; i ++)
				AddSteps(stepItem[i]);
			AddStep(stepItem);
		}
		void AddStep(StepItem stepItem) {
			if(stepItem.Count == 0)
				steps.Add(stepItem);
		}
		bool CheckStartCondition(StepItem stepItem) {
			return new ObjectCondition(SourceObject, stepItem.StartCondition).Run();
		}
		void ShowStep(int delta) {
			this.stepIndex += delta;
			ShowStep();
		}
		void ShowStep() {
			UpdateTopText();
			UpdateButtonsText();
			if(this.stepItemFrame != null) {
				this.stepItemFrame.Dispose();
				this.stepItemFrame = null;
			}
			if(StepIndex < steps.Count)
				this.stepItemFrame = new StepItemFrame(GetStepItem(StepIndex), SourceObject, this);
			else this.stepItemFrame = new EventItemFrame(TaskItem.Event, SourceObject, this);
			UpdateNextButtonEnableState();
		}
		void CreateControls() {
			this.pnlTop = new Panel();
			this.pnlTop.Parent = this;
			this.pnlTop.Dock = DockStyle.Top;
			this.pnlTop.Height = 20;
			this.lbTop = new Label();
			this.lbTop.Parent = pnlTop;
			this.lbTop.Font = new Font(this.lbTop.Font, FontStyle.Bold);
			this.lbTop.Dock = DockStyle.Fill;
			this.pnlBottom = new Panel();
			this.pnlBottom.Parent = this;
			this.pnlBottom.Dock = DockStyle.Bottom;
			this.pnlBottom.Height = 30;
			this.btnPrev  = CreatePrevNextButton("Previous");
			this.btnPrev.Click += new EventHandler(OnButtonPrev);
			this.btnNext  = CreatePrevNextButton("Next");
			this.btnNext.Click += new EventHandler(OnButtonNext);
			this.btnNext.Left = pnlBottom.ClientSize.Width - this.btnNext.Width - 2;
			this.btnPrev.Left = this.btnNext.Left - this.btnPrev.Width - 5;
		}
		SimpleButton CreatePrevNextButton(string text) {
			SimpleButton button = new SimpleButton();
			button.Parent = pnlBottom;
			button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			button.Text = text;
			button.Top = (pnlBottom.ClientSize.Height - button.Height) / 2;
			return button;
		}
		void UpdateTopText() {
			string name = StepIndex < steps.Count ? GetStepItem(StepIndex).Name : TaskItem.Event.Name; 
			string text = string.Format("Step {0:##} of {1:##}. ", StepIndex + 1, StepCount) + name;
			this.lbTop.Text = text;
		}
		void UpdateButtonsText() {
			btnNext.Text = StepIndex < StepCount - 1 ? "Next" : "Finish";
		}
		void ITaskRunner.SourceObjectChanged() {
			UpdateNextButtonEnableState();
		}
		Control ITaskRunner.Control { get { return this; } }
		IEmbeddedFrame ITaskRunner.CreateFrame(string frameName) {
			Type frameType = (Type)this.stepFrames[frameName];
			if(frameType == null) return null;
			ConstructorInfo constructorInfoObj = frameType.GetConstructor(Type.EmptyTypes);
			return constructorInfoObj.Invoke(null) as IEmbeddedFrame;
		}
		void OnButtonPrev(object sender, EventArgs e) {
			if(StepIndex == 0)
				Finish(false);
			else {
				ShowStep(-1);
			}
		}
		void UpdateNextButtonEnableState() {
			btnNext.Enabled = stepItemFrame == null || stepItemFrame.CanMoveNext;
		}
		void OnButtonNext(object sender, EventArgs e) {
			if(StepIndex + 1 == StepCount)
				Finish(true);
			else ShowStep(1);
		}
		private void InitializeComponent() {
			this.Name = "TaskRunner";
			this.Size = new System.Drawing.Size(576, 480);
		}
	}
}
