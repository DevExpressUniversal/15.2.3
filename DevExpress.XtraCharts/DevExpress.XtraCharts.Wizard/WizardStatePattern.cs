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
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraCharts.Wizard;
namespace DevExpress.XtraCharts.Native {
	internal interface ILabelControl {
		string Text { get; }
		Image Image { get; }
		void Highlight();
	}
	internal interface IParentControl {
		Control Control { get; }
		void SetDescription(string description);
		void SetHeader(string header);
	}
	public class WizardState {
		WizardPage page;
		InternalWizardControlBase wizardControl;
		public Control WizardControl { get { return wizardControl; } }
		public WizardState(WizardFormBase wizardForm, WizardPage page) {
			this.page = page;
			wizardControl = CreateControl();
			wizardControl.Initialize(wizardForm, page);
		}
		InternalWizardControlBase CreateControl() {
			InternalWizardControlBase control;
			object handle = Activator.CreateInstance(page.Type);
			if (handle is InternalWizardControlBase)
				control = (InternalWizardControlBase)handle;
			else {
				UserWizardPageControlContainer container = new UserWizardPageControlContainer();
				container.AddControl((WizardControlBase)handle);
				control = container;
			}
			return control;
		}
		public void Execute() {
			page.AddControl(wizardControl);
		}
		public bool PrepareChangeState() {
			return wizardControl.PerformChangeState();
		}
		public bool ValidateContent() {
			return wizardControl.ValidateContent();
		}
		public bool ValidateContentWihoutOnValidateHandler() {
			var userControlContainer = wizardControl as UserWizardPageControlContainer;
			if (userControlContainer!= null)
				return userControlContainer.ValidateContentWithoutCallOnValidationHandler();
			return wizardControl.ValidateContent();
		}
	}
	public class WizardStateController {
		WizardState currentState;
		LinkedStateList stateList;
		WizardFormBase wizardForm;
		public WizardState State { 
			get { return currentState; } 
			private set { currentState = value; } 
		}
		public WizardFormBase WizardForm { get { return wizardForm; } }
		public WizardStateController(WizardFormBase wizardForm, IList<WizardPage> data) {
			this.wizardForm = wizardForm;
			stateList = new LinkedStateList(this, data);
			ChangeState(stateList.Custom(0));
		}
		#region IStateController Members
		public void NextState() {
			if (!stateList.IsLastState && State.PrepareChangeState())
				ChangeState(stateList.Next());
		}
		public void PreviousState() {
			if (!stateList.IsFirstState && State.PrepareChangeState())
				ChangeState(stateList.Previous());
		}
		public bool CustomState(int index) {
			if (stateList.CurrentStateIndex == index)
				return true;
			if (!State.PrepareChangeState())
				return false;
			ChangeState(stateList.Custom(index));
			return true;
		}
		public bool CanChangeState() {
			return State.ValidateContentWihoutOnValidateHandler();
		}
		#endregion
		public virtual void UpdateWizardForm() {
			wizardForm.EnableNextButton(!stateList.IsLastState);
			wizardForm.EnablePreviousButton(!stateList.IsFirstState);
		}
		public void ChangeState(WizardState newState) {
			State = newState;
			State.Execute();
			UpdateWizardForm();
		}
	}
	class LinkedStateList {
		IList<WizardPage> pageData;
		WizardStateController controller;
		int index;
		public int CurrentStateIndex { get { return index; } } 
		public int Count { get { return pageData.Count; } }
		public bool IsFirstState { get { return index <= 0; } }
		public bool IsLastState { get { return index >= pageData.Count - 1; } }
		public WizardPage CurrentPage { get { return pageData[index]; } }
		public LinkedStateList(WizardStateController controller, IList<WizardPage> pageData) {
			this.controller = controller;
			this.pageData = pageData;
		}
		public WizardState Next() {
			if (!IsLastState)
				index++;
			return new WizardState(controller.WizardForm, pageData[index]);
		}
		public WizardState Previous() {
			if (!IsFirstState)
				index--;
			return new WizardState(controller.WizardForm, pageData[index]);
		}
		public WizardState Custom(int index) {
			if (IsCorrectIndex(index))
				this.index = index;
			return new WizardState(controller.WizardForm, pageData[index]);
		}
		bool IsCorrectIndex(int index) {
			return pageData != null && index >= 0 && index < pageData.Count;
		}
	}
}
