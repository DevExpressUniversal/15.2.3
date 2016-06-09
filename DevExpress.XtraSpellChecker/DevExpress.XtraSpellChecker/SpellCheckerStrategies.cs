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
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Forms;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.XtraSpellChecker.Localization;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraSpellChecker.Strategies {
	public class WinSearchStrategy : SearchStrategy {
		public WinSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController) : base(spellChecker, textController) { }
		public WinSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController, ContainerSearchStrategyBase parentStrategy) : base(spellChecker, textController, parentStrategy) { }
		protected override TextOperationsManager CreateOperationsManager() {
			return new WinTextOperationsManager(this);
		}
		protected override bool ShouldClearLists() {
			return false;  
		}
		public virtual new SpellChecker SpellChecker { get { return base.SpellChecker as SpellChecker; } }
		protected override SpellCheckerStateBase CreateSpellCheckerState(StrategyState fState) {
			if(fState == StrategyState.Checking)
				return new WinCheckingSpellCheckerState(this);
			return base.CreateSpellCheckerState(fState);
		}
	}
	public class WinPartTextSearchStrategy : PartTextSearchStrategy {
		public WinPartTextSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController, Position startPosition, Position finishPosition)
			: base(spellChecker, textController, startPosition, finishPosition) { }
		protected override TextOperationsManager CreateOperationsManager() {
			return new WinPartTextOperationsManager(this);
		}
		public virtual new SpellChecker SpellChecker { get { return base.SpellChecker as SpellChecker; } }
		protected override CheckingPartSpellCheckerStateBase CreateCheckingMainPartSpellCheckerState() {
			return new WinCheckingMainPartSpellCheckerState(this);
		}
		protected override CheckingPartSpellCheckerStateBase CreateCheckingBeforeMainPartSpellCheckerState() {
			return new WinCheckingBeforeMainPartSpellCheckerState(this);
		}
		protected override CheckingPartSpellCheckerStateBase CreateCheckingAfterMainPartSpellCheckerState() {
			return new WinCheckingAfterMainPartSpellCheckerState(this);
		}
		protected override bool ShouldClearLists() {
			return false;  
		}
	}
	public class WinPartSilentTextSearchStrategy : PartSimpleTextSilentSearchStrategy {
		public WinPartSilentTextSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController, Position startPosition)
			: base(spellChecker, textController, startPosition) { }
		protected override TextOperationsManager CreateOperationsManager() {
			return new WinPartTextOperationsManager(this);
		}
		public virtual new SpellChecker SpellChecker { get { return base.SpellChecker as SpellChecker; } }
		protected override CheckingPartSpellCheckerStateBase CreateCheckingMainPartSpellCheckerState() {
			return new WinCheckingMainPartSilentSpellCheckerState(this);
		}
		protected override CheckingPartSpellCheckerStateBase CreateCheckingAfterMainPartSpellCheckerState() {
			return new WinCheckingAfterMainPartSpellCheckerState(this);
		}
		protected override CheckingPartSpellCheckerStateBase CreateCheckingBeforeMainPartSpellCheckerState() {
			return new WinCheckingBeforeMainPartSpellCheckerState(this);
		}
		protected override bool ShouldClearLists() {
			return false;  
		}
	}
	public class WinTextOperationsManager : TextOperationsManager {
		public SpellingOptionsForm OptionsForm { get { return SpellChecker.FormsManager.OptionsForm; } }
		protected virtual void HandleOptionsFormResult(DialogResult result) {
			if (result == DialogResult.OK)
				OptionsForm.Apply();
		}
		public WinTextOperationsManager(SearchStrategy searchStrategy) : base(searchStrategy) { }
		public override void ShowOptionsDialog() {
			HandleOptionsFormResult(ShowOptionsForm());
			CurrentPosition = TextController.GetWordStartPosition(WordStartPosition);
			base.DoSpellCheckOperation(SpellCheckOperation.Recheck, string.Empty, SearchStrategy.ActiveError);			
		}
		public override void DoSpellCheckOperation(SpellCheckOperation operation, string suggestion, SpellCheckErrorBase error) {
			base.DoSpellCheckOperation(operation, suggestion, SearchStrategy.ActiveError);
			if(SpellChecker.IsCheckAsYouTypeAndDialogMode && operation != SpellCheckOperation.AddToDictionary && !SpellChecker.IsRichTextEdit(SpellChecker.EditControl))
				SpellChecker.Manager.OperationsManager.DoSpellCheckOperation(operation, suggestion, SearchStrategy.ActiveError);
		}
		public override void IgnoreOnce(Position start, Position finish, string word) {
			base.IgnoreOnce(start, finish, word);
		}
		public virtual SpellChecker SpellChecker {
			get { return SearchStrategy.SpellChecker as SpellChecker; }
		}
		public virtual DialogResult ShowOptionsForm() {
			return SpellChecker.FormsManager.ShowOptionsForm();
		}
	}
	public class WinPartTextOperationsManager : PartTextOperationsManager {
		public SpellingOptionsForm OptionsForm { get { return SpellChecker.FormsManager.OptionsForm; } }
		protected virtual void HandleOptionsFormResult(DialogResult result) {
		}
		public WinPartTextOperationsManager(PartTextSearchStrategy searchStrategy) : base(searchStrategy) { }
		public override void ShowOptionsDialog() {
			HandleOptionsFormResult(ShowOptionsForm());
			CurrentPosition = TextController.GetWordStartPosition(WordStartPosition);
			base.DoSpellCheckOperation(SpellCheckOperation.Recheck, string.Empty, SearchStrategy.ActiveError);
		}
		public override void DoSpellCheckOperation(SpellCheckOperation operation, string suggestion, SpellCheckErrorBase error) {
			base.DoSpellCheckOperation(operation, suggestion, SearchStrategy.ActiveError);
			if (SpellChecker.IsCheckAsYouTypeAndDialogMode && !SpellChecker.IsRichTextEdit(SpellChecker.EditControl))
				SpellChecker.Manager.OperationsManager.DoSpellCheckOperation(operation, suggestion, SearchStrategy.ActiveError);
		}
		public override void IgnoreOnce(Position start, Position finish, string word) {
			base.IgnoreOnce(start, finish, word);
		}
		public virtual SpellChecker SpellChecker {
			get { return SearchStrategy.SpellChecker as SpellChecker; }
		}
		public virtual DialogResult ShowOptionsForm() {
			return SpellChecker.FormsManager.ShowOptionsForm();
		}
	}
	public class WinControlContainerSearchStrategy : ContainerSearchStrategyBase {
		public WinControlContainerSearchStrategy(SpellChecker spellChecker, Control container) : base(spellChecker, container) { }
		public virtual new Control Container { get { return base.Container as Control; } }
		public virtual new SpellChecker SpellChecker { get { return base.SpellChecker as SpellChecker; } }
		protected virtual new Control CurrentControl {
			get { return base.CurrentControl as Control; }
			set { base.CurrentControl = value; }
		}
		protected override bool CanCheckControl(object currentObject) {
			Control control = (Control)currentObject;
			if(control != null && control.IsHandleCreated && control.Visible)
				return CanCheckControl(control);
			return false;
		}
		protected internal override ControlBrowserBase CreateControlBrowser(object control) {
			Control controlInstance = control as Control;
			if(control is IControlIterator)
				return new ControlIterator(controlInstance);
			else
				return new ContainerBrowser(controlInstance);
		}
		protected virtual bool CanCheckControl(Control control) {
			bool result = control != null && SpellCheckTextControllersManager.Default.IsClassRegistered(control.GetType())
				&& SpellChecker.GetCanCheckText(control);
			if(result) {
				ISpellCheckTextControlController controller = SpellCheckTextControllersManager.Default.GetSpellCheckTextControlController(control);
				try {
					result = !controller.IsReadOnly;
				}
				finally {
					controller.Dispose();
				}
			}
			return result;
		}
		public override void FinishControlChecking() { 
			base.FinishControlChecking();
		}
	}
	public abstract class WinControlBrowserBase : ControlBrowserBase {
		protected WinControlBrowserBase(object container) : base(container) {
		}
		public new Control Container { get { return base.Container as Control; } }
		protected override ControlBrowserBase CreateChildControlBrowser(object control) {
			Control controlInstance = control as Control;
			if(control is IControlIterator)
				return new ControlIterator(controlInstance);
			else
				return new ContainerBrowser(controlInstance);
		}
		protected override bool Contains(object control) {
			return Container.Contains((Control)control);
		}
		protected override bool IsObjectContainer(object container) {
			return container is IContainerControl;
		}
	}
	public class ControlIterator : WinControlBrowserBase {
		public ControlIterator(object container) : base(container) { }
		public new IControlIterator Container { get { return base.Container as IControlIterator; } }
		protected override object GetNextObjectCore(object control) {
			return Container.GetNextObject(control) as Control;
		}
		protected override object GetFirstObjectCore() {
			return Container.GetFirstObject() as Control;
		}
		protected override bool IsLastObjectCore(object control) {
			return Container.IsLastObject(control);
		}
		protected override ControlBrowserBase CreateInstance(object container) {
			return new ControlIterator(container);
		}
	}
	public class ContainerBrowser : WinControlBrowserBase {
		public ContainerBrowser(object container) : base(container) { }
		protected override object GetNextObjectCore(object control) {
			return Container.GetNextControl(control as Control, true);
		}
		protected override object GetFirstObjectCore() {
			MethodInfo mi = typeof(Control).GetMethod("GetFirstChildControlInTabOrder", BindingFlags.NonPublic | BindingFlags.Instance);
			return mi.Invoke(Container, new object[] { true }) as Control;
		}
		protected override bool IsLastObjectCore(object control) {
			return GetNextObjectCore(control) == null;
		}
		protected override ControlBrowserBase CreateInstance(object container) {
			return new ContainerBrowser(container);
		}
	}
}
