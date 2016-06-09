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
using System.Reflection;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.Utils;
using DevExpress.Xpf.SpellChecker.Forms;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using System.Windows;
using DevExpress.XtraSpellChecker.Localization;
using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.XtraSpellChecker.Forms;
using DevExpress.Xpf.Core.Native;
using System.Threading.Tasks;
#if SL
using MessageBoxButton = DevExpress.Xpf.Core.DXMessageBoxButton;
#endif
namespace DevExpress.XtraSpellChecker.Strategies {
	public class AgSearchStrategy : SearchStrategy {
		TaskCompletionSource<string> checkTextTaskCompletionSource;
		public AgSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController)
			: base(spellChecker, textController) {
		}
		public AgSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController, ContainerSearchStrategyBase parentStrategy)
			: base(spellChecker, textController, parentStrategy) {
		}
		public virtual new DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker { get { return base.SpellChecker as DevExpress.Xpf.SpellChecker.SpellChecker; } }
		protected override TextOperationsManager CreateOperationsManager() {
			return new AgTextOperationsManager(this);
		}
		protected override SpellCheckerStateBase CreateSpellCheckerState(StrategyState fState) {
			if (fState == StrategyState.Checking)
				return new CheckingSpellCheckerState(this);
			return base.CreateSpellCheckerState(fState);
		}
		protected override bool ShouldRaiseOperationCompletedEvent(SpellCheckOperation operation) {
			if (operation == SpellCheckOperation.Options)  
				return false;
			return base.ShouldRaiseOperationCompletedEvent(operation);
		}
		protected internal override void OnAfterCheck(StopCheckingReason reason) {
			if (TextController == null)
				this.checkTextTaskCompletionSource.SetCanceled();
			else if (this.checkTextTaskCompletionSource != null)
				this.checkTextTaskCompletionSource.SetResult(Text);
			this.checkTextTaskCompletionSource = null;
			base.OnAfterCheck(reason);
		}
		public Task<string> CheckAndGetResult() {
			if (this.checkTextTaskCompletionSource != null)
				this.checkTextTaskCompletionSource.SetCanceled();
			this.checkTextTaskCompletionSource = new TaskCompletionSource<string>();
			Task<string> result = this.checkTextTaskCompletionSource.Task;
			Check();
			return result;
		}
	}
	public class AgPartTextSearchStrategy : PartTextSearchStrategy {
		public AgPartTextSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController, Position startPosition, Position finishPosition)
			: base(spellChecker, textController, startPosition, finishPosition) { }
		protected override TextOperationsManager CreateOperationsManager() {
			return new AgPartTextOperationsManager(this);
		}
		public virtual new DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker { get { return base.SpellChecker as DevExpress.Xpf.SpellChecker.SpellChecker; } }
		protected override CheckingPartSpellCheckerStateBase CreateCheckingMainPartSpellCheckerState() {
			return new AgCheckingMainPartSpellCheckerState(this);
		}
		protected override CheckingPartSpellCheckerStateBase CreateCheckingBeforeMainPartSpellCheckerState() {
			return new CheckingBeforeMainPartSpellCheckerState(this);
		}
		protected override CheckingPartSpellCheckerStateBase CreateCheckingAfterMainPartSpellCheckerState() {
			return new CheckingAfterMainPartSpellCheckerState(this);
		}
		protected override bool ShouldRaiseOperationCompletedEvent(SpellCheckOperation operation) {
			if (operation == SpellCheckOperation.Options)  
				return false;
			return base.ShouldRaiseOperationCompletedEvent(operation);
		}
	}
	public class AgCheckingMainPartSpellCheckerState : CheckingMainPartSpellCheckerState {
		public AgCheckingMainPartSpellCheckerState(PartTextSearchStrategy strategy) : base(strategy) { }
		public new DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker { get { return base.SpellChecker as DevExpress.Xpf.SpellChecker.SpellChecker; } }
		protected override StrategyState GetStrategyStateWhenCannotDoNextStep() {
			if (ShouldRaiseFinishCheckingMainPart) {
				FinishCheckingMainPartEventArgs args = new FinishCheckingMainPartEventArgs(true);
				SpellChecker.SLOnFinishCheckingMainPart(args);
				ShouldRaiseFinishCheckingMainPart = false;
				if (SpellChecker.IsOnFinishCheckingMainPartAssigned())
					if (args.NeedCheckRemainingPart)
						return StrategyState.CheckingAfterMainPart;
					else
						return StrategyState.Stop;
				else {
					return GetDefaultFinishCheckingMainPartResult(args);
				}
			}
			else
				return StrategyState.CheckingAfterMainPart;
		}
		protected override StrategyState GetDefaultFinishCheckingMainPartResult(FinishCheckingMainPartEventArgs e) {
			string title = SpellCheckerLocalizer.GetString(SpellCheckerStringId.Form_Spelling_Caption);
			string text = SpellCheckerLocalizer.GetString(SpellCheckerStringId.Msg_CheckNotSelectedText);
			FrameworkElement owner = SpellChecker.FormsManager.GetWidnowOwner();
#if SL
			DialogClosedDelegate onClosing = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value == true)
					SearchStrategy.SetState(StrategyState.CheckingAfterMainPart);
				else
					SearchStrategy.SetState(StrategyState.Stop);
			};
			DialogHelper.ShowDialog(title, text, owner, onClosing, MessageBoxButton.YesNo);
			return StrategyState.SuspendChecking;
#else
			bool? result = DialogHelper.ShowDialog(title, text, owner, MessageBoxButton.YesNo);
			if (result.HasValue && result.Value == true)
				return StrategyState.CheckingAfterMainPart;
			else
				return StrategyState.Stop;
#endif
		}
		bool ShouldContinueCheck(DXDialog dialog) {
#if SL
			return dialog.DialogResult == DialogResult.OK;
#else
			return dialog.DialogResult == true;
#endif
		}
	}
	public class AgPartSilentTextSearchStrategy : PartSimpleTextSilentSearchStrategy {
		public AgPartSilentTextSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController, Position startPosition)
			: base(spellChecker, textController, startPosition) { }
		protected override TextOperationsManager CreateOperationsManager() {
			return new AgPartTextOperationsManager(this);
		}
		public virtual new DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker { get { return base.SpellChecker as DevExpress.Xpf.SpellChecker.SpellChecker; } }
		protected override CheckingPartSpellCheckerStateBase CreateCheckingMainPartSpellCheckerState() {
			return new CheckingMainPartSilentSpellCheckerState(this);
		}
		protected override bool ShouldRaiseOperationCompletedEvent(SpellCheckOperation operation) {
			if (operation == SpellCheckOperation.Options)
				return false;
			return base.ShouldRaiseOperationCompletedEvent(operation);
		}
	}
	public class AgTextOperationsManager : TextOperationsManager {
		public AgTextOperationsManager(SearchStrategy searchStrategy) : base(searchStrategy) { }
		public override void ShowOptionsDialog() {
			SpellChecker.FormsManager.OptionsFormClosed += new WindowClosedEventHandler(OptionsForm_WindowClosed);
			ShowOptionsForm();
		}
		protected virtual void OptionsForm_WindowClosed(object sender, WindowClosedEventArgs e) {
			SpellChecker.FormsManager.OptionsFormClosed -= new WindowClosedEventHandler(OptionsForm_WindowClosed);
			base.DoSpellCheckOperation(SpellCheckOperation.Recheck, string.Empty, SearchStrategy.ActiveError);
		}
		public override void DoSpellCheckOperation(SpellCheckOperation operation, string suggestion, SpellCheckErrorBase error) {
			base.DoSpellCheckOperation(operation, suggestion, SearchStrategy.ActiveError);
			if (SpellChecker.SpellCheckMode == SpellCheckMode.AsYouType) {
				if (SpellChecker.EditControl == null)
					return;
				DevExpress.Xpf.SpellChecker.Native.CheckAsYouTypeBehavior behavior = DevExpress.Xpf.SpellChecker.Native.BehaviorHelper.TryGetBehavior(SpellChecker.EditControl);
				if (behavior != null) {
					int start = error != null ? error.StartPosition.ToInt() : WordStartPosition.ToInt();
					behavior.OperationManager.DoOperation(operation, start, CurrentPosition.ToInt());
				}
			}
		}
		public virtual DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker {
			get { return SearchStrategy.SpellChecker as DevExpress.Xpf.SpellChecker.SpellChecker; }
		}
		public virtual void ShowOptionsForm() {
			SpellChecker.FormsManager.ShowOptionsForm();
		}
		public override void Recheck() {
			SearchStrategy.RaiseSpellCheckOperationCompleted(EventArgs.Empty);  
		}
		public override void IgnoreOnce(Position start, Position finish, string word) {
			base.IgnoreOnce(start, finish, word);
			if (!SearchStrategy.ThreadChecking) {
				ExceptionPosition pos = new ExceptionPosition();
				pos.StartPosition = start;
				pos.FinishPosition = finish;
			}
		}
	}
	public class AgPartTextOperationsManager : PartTextOperationsManager {
		public AgPartTextOperationsManager(PartTextSearchStrategy searchStrategy) : base(searchStrategy) { }
		public override void ShowOptionsDialog() {
			SpellChecker.FormsManager.OptionsFormClosed += new WindowClosedEventHandler(OptionsForm_WindowClosed);
			ShowOptionsForm();
		}
		protected virtual void OptionsForm_WindowClosed(object sender, WindowClosedEventArgs e) {
			SpellChecker.FormsManager.OptionsFormClosed -= new WindowClosedEventHandler(OptionsForm_WindowClosed);
			base.DoSpellCheckOperation(SpellCheckOperation.Recheck, string.Empty, SearchStrategy.ActiveError);
		}
		public override void DoSpellCheckOperation(SpellCheckOperation operation, string suggestion, SpellCheckErrorBase error) {
			base.DoSpellCheckOperation(operation, suggestion, SearchStrategy.ActiveError);
			if (SpellChecker.SpellCheckMode == SpellCheckMode.AsYouType) {
				if (SpellChecker.EditControl == null)
					return;
				DevExpress.Xpf.SpellChecker.Native.CheckAsYouTypeBehavior behavior = DevExpress.Xpf.SpellChecker.Native.BehaviorHelper.TryGetBehavior(SpellChecker.EditControl);
				if (behavior != null) {
					int start = error != null ? error.StartPosition.ToInt() : WordStartPosition.ToInt();
					behavior.OperationManager.DoOperation(operation, start, CurrentPosition.ToInt());
				}
			}
		}
		public virtual DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker {
			get { return SearchStrategy.SpellChecker as DevExpress.Xpf.SpellChecker.SpellChecker; }
		}
		public virtual void ShowOptionsForm() {
			SpellChecker.FormsManager.ShowOptionsForm();
		}
		public override void Recheck() {
			SearchStrategy.RaiseSpellCheckOperationCompleted(EventArgs.Empty);  
		}
		public override void IgnoreOnce(Position start, Position finish, string word) {
			base.IgnoreOnce(start, finish, word);
			if (!SearchStrategy.ThreadChecking) {
				ExceptionPosition pos = new ExceptionPosition();
				pos.StartPosition = start;
				pos.FinishPosition = finish;
			}
		}
	}
#if WPF
	public class AgControlContainerSearchStrategy : ContainerSearchStrategyBase {
		public AgControlContainerSearchStrategy(DevExpress.Xpf.SpellChecker.SpellChecker spellChecker, Control container) : base(spellChecker, container) { }
		public virtual new Control Container { get { return base.Container as Control; } }
		public virtual new DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker { get { return base.SpellChecker as DevExpress.Xpf.SpellChecker.SpellChecker; } }
		protected virtual new Control CurrentControl {
			get { return base.CurrentControl as Control; }
			set { base.CurrentControl = value; }
		}
		protected override bool CanCheckControl(object currentObject) {
			Control control = (Control)currentObject;
			if (control != null)
				return CanCheckControl(control);
			return false;
		}
		protected internal override ControlBrowserBase CreateControlBrowser(object control) {
			if (control is IControlIterator)
				return new ControlIterator(control as Control);
			return null;
		}
		protected virtual bool CanCheckControl(Control control) {
			bool result = control != null && SpellCheckTextControllersManager.Default.IsClassRegistered(control.GetType());
			if (result) {
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
		protected WinControlBrowserBase(object container) : base(container) { }
		public new UIElement Container { get { return base.Container as UIElement; } }
		protected override ControlBrowserBase CreateChildControlBrowser(object control) {
			if (control is IControlIterator)
				return new ControlIterator(control as Control);
			return null;
		}
		protected override bool Contains(object control) {
			UIElement c = control as UIElement;
			if (c == null)
				return false;
			return c.FindIsInParents(c);
		}
	}
	public interface IControlIterator : IDisposable {
		object GetNextObject(object currentObject);
		bool IsLastObject(object currentObject);
		object GetFirstObject();
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
#endif
	public class XpfControlContainerSearchStrategy : ContainerSearchStrategyBase {
		Queue<Control> controlsQueue;
		ContainerHelper helper;
		public XpfControlContainerSearchStrategy(DevExpress.Xpf.SpellChecker.SpellChecker spellChecker, FrameworkElement container)
			: base(spellChecker, container) {
			this.helper = new ContainerHelper();
		}
		public virtual new FrameworkElement Container { get { return base.Container as FrameworkElement; } }
		public virtual new DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker { get { return base.SpellChecker as DevExpress.Xpf.SpellChecker.SpellChecker; } }
		protected virtual new Control CurrentControl {
			get { return base.CurrentControl as Control; }
			set { base.CurrentControl = value; }
		}
		#region Events
		#region AfterCheck
		EventHandler onAfterCheck;
		public event EventHandler AfterCheck { add { onAfterCheck += value; } remove { onAfterCheck -= value; } }
		protected void RaiseAfterCheckEvent() {
			EventHandler handler = onAfterCheck;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected override bool CanCheckControl(object currentObject) {
			System.Diagnostics.Debug.Assert(false);
			return false;
		}
		public override object GetFirstObject() {
			System.Diagnostics.Debug.Assert(false);
			return null;
		}
		public override object GetNextObject(object currentObject) {
			System.Diagnostics.Debug.Assert(false);
			return null;
		}
		public override bool IsLastObject(object currentObject) {
			return !(controlsQueue.Count > 0);
		}
		public override void FinishControlChecking() {
			CheckNextControl();
		}
		void CheckNextControl() {
			if (!IsStoped && controlsQueue.Count > 0)
				CheckNextElementFromQueue();
			else
				RaiseAfterCheckEvent();
		}
		protected virtual bool CanCheckControl(Control control) {
			return helper.CanCheckControl(control);
		}
		public override void Check() {
			FindControlsForCheck(Container);
			CheckNextControl();
		}
		protected void CheckNextElementFromQueue() {
			CurrentControl = controlsQueue.Dequeue();
			CheckCurrentControl();
		}
		protected virtual void FindControlsForCheck(FrameworkElement element) {
			this.controlsQueue = helper.GetControlsFromContainer(element);
		}
	}
	public class ContainerHelper {
		Queue<Control> controlsQueue;
		public ContainerHelper() {
			this.controlsQueue = new Queue<Control>();
		}
		public bool CanCheckControl(Control control) {
			if (CanCheckControlCore(control)) {
				ISpellCheckTextControlController controller = SpellCheckTextControllersManager.Default.GetSpellCheckTextControlController(control);
				try {
					return !controller.IsReadOnly;
				}
				finally {
					controller.Dispose();
				}
			}
			return false;
		}
		bool CanCheckControlCore(Control control) {
			return SpellCheckTextControllersManager.Default.IsClassRegistered(control.GetType());
		}
		public Queue<Control> GetControlsFromContainer(FrameworkElement container) {
			Control control = container as Control;
			if (control != null && CanCheckControl(control)) {
				controlsQueue.Enqueue(control);
				return this.controlsQueue;
			}
			int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(container);
			for (int i = 0; i < count; i++) {
				FrameworkElement child = VisualTreeHelper.GetChild(container, i) as FrameworkElement;
				if (child != null)
					GetControlsFromContainer(child);
			}
			return this.controlsQueue;
		}
	}
}
