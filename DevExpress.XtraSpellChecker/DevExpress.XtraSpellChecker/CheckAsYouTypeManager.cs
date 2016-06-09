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
using System.Collections.Generic;
using System.Text;
using System.Collections;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.XtraSpellChecker.Parser;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.XtraSpellChecker.Rules;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Threading;
using DevExpress.XtraEditors;
using DevExpress.XtraSpellChecker.Localization;
using System.Security.Permissions;
using System.Security;
using System.Threading.Tasks;
namespace DevExpress.XtraSpellChecker.Native {
	public delegate void AsyncCheckingFinishedCallBack(ArrayList errors);
	public class AsyncChecker : IDisposable {
		#region Fields
		Thread workingThread;
		TaskCompletionSource<ArrayList> currentSource;
		AutoResetEvent startEvent;
		AutoResetEvent completeEvent;
		ManualResetEvent exitEvent;
		bool isWorking;
		SpellChecker spellChecker;
		AsyncSearchStrategy searchStrategy;
		bool isDisposing;
		#endregion
		public AsyncChecker(SpellChecker spellChecker) {
			this.spellChecker = spellChecker;
			this.startEvent = new AutoResetEvent(false);
			this.completeEvent = new AutoResetEvent(true);
			this.exitEvent = new ManualResetEvent(false);
			this.workingThread = new Thread(new ThreadStart(DoBackgroundWork));
			this.workingThread.Priority = ThreadPriority.Lowest;
			this.workingThread.IsBackground = true;
			this.searchStrategy = CreateSearchStrategy();
			this.workingThread.Start();
		}
		#region Properties
		Thread WorkingThread { get { return workingThread; } }
		public ManualResetEvent ExitEvent { get { return exitEvent; } }
		public bool IsWorking { get { return isWorking; } }
		AsyncSearchStrategy SearchStrategy { get { return searchStrategy; } }
		public SpellChecker SpellChecker { get { return spellChecker; } }
		public string Text { get { return SearchStrategy.Text; } }
		#endregion
		public virtual Task<ArrayList> DoWork(string text, OptionsSpellingBase options) {
			if (this.isDisposing)
				return null;
			CheckCurrentThread();
			this.completeEvent.WaitOne();
			TaskCompletionSource<ArrayList> source = new TaskCompletionSource<ArrayList>();
			this.currentSource = source;
			SearchStrategy.OptionsSpelling.Assign(options);
			SearchStrategy.Text = text;
			this.startEvent.Set();
			return source.Task;
		}
		protected virtual AsyncSearchStrategy CreateSearchStrategy() {
			return new AsyncSearchStrategy(SpellChecker);
		}
		protected internal virtual void OnCallBack() {
			if (this.currentSource != null) {
				try {
					if (!this.isDisposing)
						this.currentSource.SetResult((ArrayList)SearchStrategy.ErrorList.Clone());
					else
						this.currentSource.SetCanceled();
				}
				catch (Exception e) {
					this.currentSource.TrySetException(e);
				}
			}
		}
		void DoBackgroundWork() {
			for (; ; ) {
				if (ExitCondition())
					break;
				try {
					this.isWorking = true;
					SearchStrategy.ClearErrorList();
					SearchStrategy.Check();
				}
				catch {
					SearchStrategy.ClearErrorList();
				}
				finally {
					OnCallBack();
					this.completeEvent.Set();
					this.isWorking = false;
				}
			}
		}
		bool ExitCondition() {
			return WaitHandle.WaitAny(new WaitHandle[] { startEvent, exitEvent }) == 1;
		}
		public void Dispose() {
			if (this.isDisposing)
				return;
			this.exitEvent.Set();
			CheckCurrentThread();
			if (this.workingThread != null && this.workingThread.ThreadState != System.Threading.ThreadState.Unstarted) {
				this.workingThread.Join();
				this.workingThread = null;
			}
			this.currentSource = null;
			this.startEvent.Close();
			this.exitEvent.Close();
			this.completeEvent.Close();
			this.isDisposing = true;
		}
		void CheckCurrentThread() {
			if (Thread.CurrentThread == this.workingThread)
				throw new InvalidOperationException();
		}
		protected virtual void DisposeSearchStrategy() {
			if (searchStrategy != null) {
				SearchStrategy.Dispose();
				searchStrategy = null;
			}
		}
		public SpellCheckErrorBase GetErrorByWord(string word) {
			return SearchStrategy.GetErrorByWord(word);
		}
	}
	#region States
	public class WinCheckingSpellCheckerState : CheckingSpellCheckerState {
		public WinCheckingSpellCheckerState(SearchStrategy strategy) : base(strategy) { }
		public virtual new SpellChecker SpellChecker { get { return base.SpellChecker as SpellChecker; } }
		protected CheckAsYouTypeManager Manager { get { return SpellChecker.Manager; } }
		protected override bool ShouldProcessWordWithoutChecking() {
			bool result = base.ShouldProcessWordWithoutChecking();
			if (result)
				return result;
			if (!SpellChecker.ThreadChecking)
				lock (Manager) {
					result = Manager.OperationsManager.IsException(SearchStrategy.WordStartPosition, SearchStrategy.CurrentPosition, SearchStrategy.CheckedWord);
				}
			return result;
		}
	}
	public class WinCheckingAfterMainPartSpellCheckerState : CheckingAfterMainPartSpellCheckerState {
		public WinCheckingAfterMainPartSpellCheckerState(PartTextSearchStrategy strategy) : base(strategy) { }
		public virtual new SpellChecker SpellChecker { get { return base.SpellChecker as SpellChecker; } }
		protected CheckAsYouTypeManager Manager { get { return SpellChecker.Manager; } }
		protected override bool ShouldProcessWordWithoutChecking() {
			bool result = base.ShouldProcessWordWithoutChecking();
			if (result)
				return result;
			lock (Manager) {
				result = Manager.OperationsManager.IsException(SearchStrategy.WordStartPosition, SearchStrategy.CurrentPosition, SearchStrategy.CheckedWord);
			}
			return result;
		}
	}
	public class WinCheckingBeforeMainPartSpellCheckerState : CheckingBeforeMainPartSpellCheckerState {
		public WinCheckingBeforeMainPartSpellCheckerState(PartTextSearchStrategy strategy) : base(strategy) { }
		public virtual new SpellChecker SpellChecker { get { return base.SpellChecker as SpellChecker; } }
		protected CheckAsYouTypeManager Manager { get { return SpellChecker.Manager; } }
		protected override bool ShouldProcessWordWithoutChecking() {
			bool result = base.ShouldProcessWordWithoutChecking();
			if (result)
				return result;
			lock (Manager) {
				result = Manager.OperationsManager.IsException(SearchStrategy.WordStartPosition, SearchStrategy.CurrentPosition, SearchStrategy.CheckedWord);
			}
			return result;
		}
	}
	public class WinCheckingMainPartSpellCheckerState : CheckingMainPartSpellCheckerState {
		public WinCheckingMainPartSpellCheckerState(PartTextSearchStrategy strategy) : base(strategy) { }
		public virtual new SpellChecker SpellChecker { get { return base.SpellChecker as SpellChecker; } }
		protected CheckAsYouTypeManager Manager { get { return SpellChecker.Manager; } }
		protected override bool ShouldProcessWordWithoutChecking() {
			bool result = base.ShouldProcessWordWithoutChecking();
			if (result)
				return result;
			lock (Manager) {
				result = Manager.OperationsManager.IsException(SearchStrategy.WordStartPosition, SearchStrategy.CurrentPosition, SearchStrategy.CheckedWord);
			}
			return result;
		}
		protected override StrategyState GetDefaultFinishCheckingMainPartResult(FinishCheckingMainPartEventArgs e) {
			IWin32Window owner = SpellChecker.FormsManager.DialogOwner;
			string text = SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MsgBoxCheckNotSelectedText);
			DialogResult result = XtraMessageBox.Show(owner, text, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			if (result == DialogResult.Yes)
				return StrategyState.CheckingAfterMainPart;
			return StrategyState.Stop;
		}
	}
	public class WinCheckingMainPartSilentSpellCheckerState : CheckingMainPartSilentSpellCheckerState {
		public WinCheckingMainPartSilentSpellCheckerState(PartTextSearchStrategy strategy) : base(strategy) { }
		public virtual new SpellChecker SpellChecker { get { return base.SpellChecker as SpellChecker; } }
		protected CheckAsYouTypeManager Manager { get { return SpellChecker.Manager; } }
		protected override bool ShouldProcessWordWithoutChecking() {
			bool result = base.ShouldProcessWordWithoutChecking();
			if (result)
				return result;
			lock (Manager) {
				result = Manager.OperationsManager.IsException(SearchStrategy.WordStartPosition, SearchStrategy.CurrentPosition, SearchStrategy.CheckedWord);
			}
			return result;
		}
	}
	public class AsyncCheckingSpellCheckerState : WinCheckingSpellCheckerState {
		public AsyncCheckingSpellCheckerState(AsyncSearchStrategy strategy) : base(strategy) { }
		public virtual new AsyncSearchStrategy SearchStrategy { get { return base.SearchStrategy as AsyncSearchStrategy; } }
		protected virtual bool ShouldStopChecking() {
			return Manager.NeedRecheck || Manager.IsDisposing;
		}
		protected override StrategyState DoOperationCore() {
			if (ShouldStopChecking()) {
				return StrategyState.Stop;
			}
			else
				return base.DoOperationCore();
		}
	}
	#endregion
	public class AsyncSearchStrategy : AsyncSearchStrategyBase {
		public AsyncSearchStrategy(SpellCheckerBase spellChecker)
			: base(spellChecker) {
		}
		protected override SpellCheckerStateBase CreateAsyncCheckingState(StrategyState state) {
			return new AsyncCheckingSpellCheckerState(this);
		}
		protected override SpellCheckerRulesController CreateAsyncRulesController() {
			return new AsyncSpellCheckerRulesController(this, OptionsSpelling);
		}
		protected override OptionsSpellingBase CreateOptionsSpelling() {
			return new OptionsSpelling();
		}
	}
	public class AsyncSpellCheckerRulesController : SpellCheckerRulesController {
		public AsyncSpellCheckerRulesController(AsyncSearchStrategy strategy, OptionsSpellingBase optionsSpelling)
			: base(strategy, optionsSpelling) {
		}
		protected override SpellCheckerErrorClassManager CreateErrorClassManager() {
			return new AsyncSpellCheckerErrorClassManager();
		}
		protected override void OnSpellCheckerOptionsChanged(object sender, EventArgs e) {
			lock (this) {
				base.OnSpellCheckerOptionsChanged(sender, e);
			}
		}
	}
	public interface IEditControlWrapper {
		Point GetPointByPosition(Position pos);
		Point GetPointByLine(int lineNumber);
		Position GetPositionByPoint(Point p);
		int GetLineByPosition(Position pos);
		Graphics GetGraphics();
		int GetWidth();
		int GetHeight();
		Position CurrentPosition { get; }
		Position SelectionLength { get; }
		string Text { get; set; }
		Color GetBackColor();
		int FirstVisibleLine { get; }
		Position FirstVisibleIndex { get; }
		bool CanScrollTextHorizontally { get; }
		bool IsSingleLine { get; }
		void InvalidateRect(Rectangle rectangle);
		string GetVisibleText(); 
		Position GetFirstPositionOfLine(int line);
		Position GetLastPositionOfLine(Position firstPosition);
		bool Enabled { get; }
		bool ReadOnly { get; }
	}
	public class ViewInfoBase : IDisposable {
		string text = string.Empty;
		internal const int BrushHeight = 3;
		IEditControlWrapper editControlWrapper = null;
		bool isViewInfoReady = false;
		ArrayList errorList = new ArrayList();
		RectangleList errorRectangles = new RectangleList();
		EventHandler viewinfoReady = null;
		public ViewInfoBase(IEditControlWrapper editControlWrapper) {
			this.editControlWrapper = editControlWrapper;
		}
		public ArrayList ErrorList { get { return this.errorList; } }
		public RectangleList ErrorRectangles { get { return this.errorRectangles; } }
		protected IEditControlWrapper EditControlWrapper { get { return this.editControlWrapper; } }
		public void Dispose() {
			this.editControlWrapper = null;
		}
		public string Text { get { return text; } set { text = value; } }
		protected virtual void FillErrorList(ArrayList errorList) {
			lock (ErrorList.SyncRoot) {
				ErrorList.Clear();
				lock (errorList.SyncRoot)
					ErrorList.AddRange(errorList);
			}
		}
		public virtual void Calculate(ArrayList errorList) {
			IsViewInfoReady = false;
			FillErrorList(errorList);
			Calculate();
		}
		protected virtual bool CanPaintRectangle(Rectangle errorRectangle) {
			int lineHeight = EditControlWrapper.GetPointByLine(EditControlWrapper.FirstVisibleLine).Y;
			bool result = errorRectangle.Top >= lineHeight;
			result = result && Math.Abs(errorRectangle.Bottom) <= EditControlWrapper.GetHeight() - lineHeight / 2;
			return result;
		}
		public virtual void Calculate() {
			IsViewInfoReady = false;
			ErrorRectangles.Clear();
			for (int i = 0; i < errorList.Count; i++) {
				RectangleList errorRectangles = GetErrorRectangles(errorList[i] as SpellCheckErrorBase);
				ErrorRectangles.AddRange(errorRectangles);
			}
			IsViewInfoReady = true;
		}
		IntPosition GetIntPositionInstance(Position pos) {
			if (pos == null)
				return IntPosition.Null;
			return pos as IntPosition;
		}
		protected virtual bool ShouldAddErrorRectangle(Rectangle rect) {
			return !rect.IsEmpty;
		}
		protected virtual Rectangle GetRectangleFromPoints(Point leftTop, Point rightBottom) {
			return Rectangle.FromLTRB(leftTop.X, leftTop.Y, rightBottom.X, leftTop.Y + BrushHeight);
		}
		protected virtual RectangleList GetErrorRectangles(SpellCheckErrorBase error) {
			RectangleList result = new RectangleList();
			Point startPoint = GetPointByPosition(error.StartPosition);
			Point finishPoint = GetPointByPosition(error.FinishPosition);
			if (!IsPointValid(startPoint) && !IsPointValid(finishPoint))
				return result;
			if (!IsPointValid(startPoint) && IsPointValid(finishPoint)) {
				Position startPos = NormalizeFirstPosition(error.StartPosition);
				startPoint = GetPointByPosition(startPos);
			}
			else if (IsPointValid(startPoint) && !IsPointValid(finishPoint)) {
				Position finishPos = NormalizeSecondPosition(error.FinishPosition);
				finishPoint = GetPointByPosition(finishPos);
			}
			if (startPoint.Y == finishPoint.Y) {
				Rectangle rect = GetRectangleFromPoints(startPoint, finishPoint);
				result.Add(rect);
			}
			else {
				int firstLineIndex = EditControlWrapper.GetLineByPosition(error.StartPosition);
				int lastLineIndex = EditControlWrapper.GetLineByPosition(error.FinishPosition);
				Position startBoxPos = EditControlWrapper.GetFirstPositionOfLine(firstLineIndex);
				Position endBoxPos = EditControlWrapper.GetLastPositionOfLine(startBoxPos);
				result.Add(GetRectangleFromPositions(error.StartPosition, endBoxPos));
				for (int i = firstLineIndex + 1; i < lastLineIndex; i++) {
					startBoxPos = EditControlWrapper.GetFirstPositionOfLine(i);
					endBoxPos = EditControlWrapper.GetLastPositionOfLine(startBoxPos);
					result.Add(GetRectangleFromPositions(startBoxPos, endBoxPos));
				}
				startBoxPos = EditControlWrapper.GetFirstPositionOfLine(lastLineIndex);
				result.Add(GetRectangleFromPositions(startBoxPos, error.FinishPosition));
			}
			return result;
		}
		Rectangle GetRectangleFromPositions(Position start, Position end) {
			return GetRectangleFromPoints(EditControlWrapper.GetPointByPosition(start), EditControlWrapper.GetPointByPosition(end));
		}
		protected virtual Point GetPointByPosition(Position pos) {
			return EditControlWrapper.GetPointByPosition(pos);
		}
		protected virtual bool IsPositionValid(Position pos) {
			Point p = GetPointByPosition(pos);
			return IsPointValid(p);
		}
		protected virtual bool IsPointValid(Point point) {
			return point.X < 65000 && point.X >= 0; 
		}
		protected virtual Position NormalizeFirstPosition(Position pos) {
			Position onePos = new IntPosition(1);
			IntPosition intPos = GetIntPositionInstance(pos);
			while (!IsPositionValid(intPos))
				intPos = Position.Add(intPos, onePos) as IntPosition;
			return intPos;
		}
		protected virtual Position NormalizeSecondPosition(Position pos) {
			Position onePos = new IntPosition(1);
			IntPosition intPos = GetIntPositionInstance(pos);
			while (!IsPositionValid(intPos))
				intPos = Position.Subtract(intPos, onePos) as IntPosition;
			return intPos;
		}
		protected internal virtual void OnViewInfoReady() {
			RaiseOnViewInfoReady();
		}
		protected internal virtual void RaiseOnViewInfoReady() {
			if (viewinfoReady != null)
				viewinfoReady(this, EventArgs.Empty);
		}
		protected bool IsViewInfoReady {
			get { return this.isViewInfoReady; }
			set {
				if (IsViewInfoReady != value)
					isViewInfoReady = value;
				if (IsViewInfoReady) {
					Text = EditControlWrapper.Text;
					OnViewInfoReady();
				}
			}
		}
		protected virtual void UpdateError(ref SpellCheckErrorBase error, Position delta) {
			error.FinishPosition = Position.Add(error.FinishPosition, delta);
		}
		protected virtual void UpdateWholeError(ref SpellCheckErrorBase error, Position delta) {
			error.StartPosition = Position.Add(error.StartPosition, delta);
			error.FinishPosition = Position.Add(error.FinishPosition, delta);
		}
		protected virtual bool UpdateErrorListWhenNoSelection() {
			Position delta = new IntPosition(1);
			bool result = false;
			for (int i = ErrorList.Count - 1; i >= 0; i--) {
				SpellCheckErrorBase error = (SpellCheckErrorBase)ErrorList[i];
				if (Position.IsLessOrEqual(EditControlWrapper.CurrentPosition, error.StartPosition)) {
					UpdateWholeError(ref error, delta);
					result = true;
					continue;
				}
				if (Position.IsLess(error.StartPosition, EditControlWrapper.CurrentPosition) &&
					Position.IsGreaterOrEqual(Position.Add(error.FinishPosition, new IntPosition(1)), EditControlWrapper.CurrentPosition)) {
					UpdateError(ref error, delta);
					result = true;
				}
				else
					break;
			}
			return result;
		}
		protected virtual bool UpdateErrorListWhenSelection() {
			Position delta = CalcDelta();
			Position startSelection = EditControlWrapper.CurrentPosition;
			Position finishSelection = Position.Add(EditControlWrapper.CurrentPosition, EditControlWrapper.SelectionLength);
			for (int i = ErrorList.Count - 1; i >= 0; i--) {
				SpellCheckErrorBase error = (SpellCheckErrorBase)ErrorList[i];
				if (Position.IsGreaterOrEqual(error.StartPosition, startSelection) &&
					Position.IsLessOrEqual(error.FinishPosition, finishSelection)) {
					ErrorList.RemoveAt(i);
					continue;
				}
				if (Position.IsLessOrEqual(EditControlWrapper.CurrentPosition, error.StartPosition)) {
					UpdateWholeError(ref error, delta);
					continue;
				}
			}
			for (int i = 0; i < ErrorList.Count; i++) {
				SpellCheckErrorBase error = (SpellCheckErrorBase)ErrorList[i];
				if (Position.IsLessOrEqual(error.StartPosition, EditControlWrapper.CurrentPosition) &&
					Position.IsGreaterOrEqual(error.FinishPosition, EditControlWrapper.CurrentPosition)) {
					UpdateError(ref error, delta);
					continue;
				}
				if (Position.IsLessOrEqual(EditControlWrapper.CurrentPosition, error.StartPosition)) {
					UpdateWholeError(ref error, delta);
				}
			}
			return true;
		}
		public virtual bool UpdateErrorList() {
			if (Position.IsGreater(EditControlWrapper.SelectionLength, Position.Null))
				return UpdateErrorListWhenSelection();
			else
				return UpdateErrorListWhenNoSelection();
		}
		protected virtual Position CalcDelta() {
			if (Position.IsGreater(EditControlWrapper.SelectionLength, Position.Null))
				return Position.Subtract(EditControlWrapper.SelectionLength, new IntPosition(1));
			else
				return new IntPosition(1);
		}
		public virtual void Assign(ViewInfoBase viewInfo) {
			ErrorRectangles.Clear();
			ErrorRectangles.AddRange(viewInfo.ErrorRectangles);
		}
		public event EventHandler ViewInfoReady { add { viewinfoReady += value; } remove { viewinfoReady -= value; } }
	}
	public class MultiLineScrollableViewInfo : ViewInfoBase {
		public MultiLineScrollableViewInfo(IEditControlWrapper editControlWrapper) : base(editControlWrapper) { }
		public override bool UpdateErrorList() {
			base.UpdateErrorList();
			return true;
		}
		protected override bool CanPaintRectangle(Rectangle errorRectangle) {
			return errorRectangle.Left >= 0 && errorRectangle.Right < EditControlWrapper.GetWidth();
		}
		public override void Calculate() {
			IsViewInfoReady = false;
			ErrorRectangles.Clear();
			for (int i = 0; i < ErrorList.Count; i++) {
				RectangleList errorRectangles = GetErrorRectangles(ErrorList[i] as SpellCheckErrorBase);
				for (int j = 0; j < errorRectangles.Count; j++) {
					Rectangle errorRectangle = errorRectangles[j];
					if (ShouldAddErrorRectangle(errorRectangle) && CanPaintRectangle(errorRectangle))
						ErrorRectangles.Add(errorRectangle);
				}
			}
			IsViewInfoReady = true;
		}
	}
	public class SingleLineViewInfo : MultiLineScrollableViewInfo {
		public SingleLineViewInfo(IEditControlWrapper editControlWrapper) : base(editControlWrapper) { }
		protected override Rectangle GetRectangleFromPoints(Point leftTop, Point rightBottom) {
			return Rectangle.FromLTRB(leftTop.X, leftTop.Y + BrushHeight - 2, rightBottom.X, leftTop.Y + BrushHeight - 1);
		}
	}
	public class ViewInfoManager : IDisposable {
		ViewInfoBase oldViewInfo = null;
		ViewInfoBase viewInfo = null;
		RectangleList clearRectangles = new RectangleList();
		RectangleList errorRectangles = new RectangleList();
		EventHandler differenceReady = null;
		bool isReady = false;
		IEditControlWrapper editControlWrapper = null;
		public ViewInfoManager(IEditControlWrapper editControlWrapper) {
			this.editControlWrapper = editControlWrapper;
		}
		#region Dispose
		public void Dispose() {
			DisposeViewInfo();
			editControlWrapper = null;
		}
		protected virtual void DisposeViewInfo() {
			if (viewInfo != null) {
				viewInfo.ViewInfoReady -= new EventHandler(OnViewInfoReady);
				viewInfo.Dispose();
				viewInfo = null;
			}
			if (oldViewInfo != null) {
				oldViewInfo.Dispose();
				oldViewInfo = null;
			}
		}
		#endregion
		public IEditControlWrapper EditControlWrapper { get { return this.editControlWrapper; } }
		protected ViewInfoBase OldViewInfo {
			get {
				if (oldViewInfo == null)
					oldViewInfo = CreateViewInfo();
				return oldViewInfo;
			}
		}
		protected ViewInfoBase ViewInfo {
			get {
				if (viewInfo == null) {
					viewInfo = CreateViewInfo();
					viewInfo.ViewInfoReady += new EventHandler(OnViewInfoReady);
				}
				return viewInfo;
			}
		}
		public bool IsReady {
			get { return isReady; }
			set {
				if (IsReady != value) {
					isReady = value;
					if (IsReady)
						OnDifferenceReady();
				}
			}
		}
		public RectangleList ClearRectangles {
			get { return clearRectangles; }
		}
		public RectangleList ErrorRectangles {
			get { return errorRectangles; }
		}
		public RectangleList AllErrorRectangles {
			get { return ViewInfo.ErrorRectangles; }
		}
		protected virtual ViewInfoBase CreateViewInfo() {
			if (!EditControlWrapper.IsSingleLine)
				if (!EditControlWrapper.CanScrollTextHorizontally)
					return new ViewInfoBase(EditControlWrapper);
				else
					return new MultiLineScrollableViewInfo(EditControlWrapper);
			else
				return new SingleLineViewInfo(EditControlWrapper);
		}
		#region DifferenceReady
		public event EventHandler DifferenceReady { add { differenceReady += value; } remove { differenceReady -= value; } }
		protected internal virtual void OnDifferenceReady() {
			RaiseOnDifferenceReady();
		}
		protected internal virtual void RaiseOnDifferenceReady() {
			if (differenceReady != null)
				differenceReady(this, EventArgs.Empty);
		}
		#endregion
		protected virtual void OnViewInfoReady(object sender, EventArgs e) {
			CalcDifference();
		}
		protected virtual Rectangle CalcClearRectangle(Rectangle errorRectangle) {
			return errorRectangle;
		}
		protected virtual void CalcDifferenceCore() {
			ErrorRectangles.AddRange(ViewInfo.ErrorRectangles);
			for (int i = 0; i < OldViewInfo.ErrorRectangles.Count; i++)
				if (!ViewInfo.ErrorRectangles.Contains(OldViewInfo.ErrorRectangles[i]))
					ClearRectangles.Add(CalcClearRectangle(OldViewInfo.ErrorRectangles[i]));
		}
		void ClearRectangleLists() {
			ClearRectangles.Clear();
			ErrorRectangles.Clear();
		}
		protected virtual void CalcDifference() {
			if (!IsReady) {
				ClearRectangleLists();
				if (OldViewInfo.ErrorRectangles.Count == 0)
					ErrorRectangles.AddRange(ViewInfo.ErrorRectangles);
				else
					CalcDifferenceCore();
				IsReady = true;
			}
		}
		protected virtual void SaveViewInfo() {
			OldViewInfo.Assign(ViewInfo);
		}
		protected virtual void CalcViewInfo() {
			SaveViewInfo();
			ViewInfo.Calculate();
		}
		protected virtual void CalcViewInfo(ArrayList errors) {
			SaveViewInfo();
			ViewInfo.Calculate(errors);
		}
		protected virtual void RequestDifferenceCore() {
			CalcViewInfo();
		}
		public virtual void RequestDifference(bool forceCalculate) {
			if (!IsReady || forceCalculate) {
				IsReady = false;
				RequestDifferenceCore();
			}
			else
				OnDifferenceReady();
		}
		public virtual void RequestDifference(ArrayList errors) {
			IsReady = false;
			CalcViewInfo(errors);
			CalcDifference();
		}
		public virtual bool UpdateErrorList() {
			return ViewInfo.UpdateErrorList();
		}
		public virtual SpellCheckErrorBase GetErrorByPosition(Position pos) {
			for (int i = 0; i < ViewInfo.ErrorList.Count; i++) {
				SpellCheckErrorBase error = ((SpellCheckErrorBase)ViewInfo.ErrorList[i]);
				if (Position.IsGreaterOrEqual(pos, error.StartPosition) && Position.IsLessOrEqual(pos, error.FinishPosition))
					return error;
			}
			return null;
		}
		public virtual void IgnoreWord(string word) {
			for (int i = this.ViewInfo.ErrorList.Count - 1; i >= 0; i--)
				if (string.Compare(((SpellCheckErrorBase)ViewInfo.ErrorList[i]).WrongWord, word, true) == 0)
					ViewInfo.ErrorList.RemoveAt(i);
			RequestDifference(true);
		}
		public virtual void IgnoreIgnoreItem(IgnoreListItem item) {
			IgnoreIgnoreItem(item.Start, item.End);
		}
		public virtual void IgnoreIgnoreItem(Position start, Position end) {
			for (int i = this.ViewInfo.ErrorList.Count - 1; i >= 0; i--) {
				SpellCheckErrorBase error = ((SpellCheckErrorBase)ViewInfo.ErrorList[i]);
				if (Position.Equals(error.StartPosition, start) && (Position.Equals(error.FinishPosition, end))) {
					ViewInfo.ErrorList.RemoveAt(i);
					break;
				}
			}
			RequestDifference(true);
		}
		public virtual void ResetViewInfo() {
			ViewInfo.ErrorRectangles.Clear();
		}
	}
	public abstract class PainterBase : IDisposable {
		readonly ViewInfoManager viewInfoManager;
		IEditControlWrapper editControlWrapper;
		Brush brush;
		SpellChecker spellChecker;
		protected PainterBase(IEditControlWrapper editControlWrapper, ViewInfoManager viewInfoManager, SpellChecker spellChecker) {
			this.editControlWrapper = editControlWrapper;
			this.viewInfoManager = viewInfoManager;
			this.spellChecker = spellChecker;
		}
		protected virtual SpellChecker SpellChecker { get { return this.spellChecker; } }
		protected abstract Brush CreateNewBrush();
		protected virtual Brush Brush {
			get {
				if (brush == null)
					brush = CreateNewBrush();
				return brush;
			}
		}
		protected virtual Graphics Graphics {
			get {
				return EditControlWrapper.GetGraphics();
			}
		}
		protected virtual ViewInfoManager ViewInfoManager { get { return this.viewInfoManager; } }
		protected virtual void FillErrorRectangle(Rectangle rect) {
			Graphics.FillRectangle(Brush, rect);
		}
		public virtual void ClearBackground(bool isFull) {
			Brush bBrush = new SolidBrush(EditControlWrapper.GetBackColor());
			try {
				ClearBackgroundCore(bBrush, isFull);
				ViewInfoManager.ClearRectangles.Clear();
			}
			finally {
				bBrush.Dispose();
			}
		}
		protected virtual void ClearBackgroundCore(Brush bBrush, bool isFull) {
			RectangleList list = isFull ? ViewInfoManager.ErrorRectangles : ViewInfoManager.ClearRectangles;
			for (int i = 0; i < list.Count; i++) {
				EditControlWrapper.InvalidateRect(list[i]);
			}
#if DEBUGTEST
			if (list.Count > 0)
				Debug.WriteLine("CLEAR");
#endif
		}
		protected virtual void PaintErrorsCore() {
			for (int i = 0; i < ViewInfoManager.ErrorRectangles.Count; i++)
				FillErrorRectangle(ViewInfoManager.ErrorRectangles[i]);
#if DEBUGTEST
			if (ViewInfoManager.ErrorRectangles.Count > 0)
				Debug.WriteLine("ERRORS");
#endif
		}
		protected virtual void PaintCore() {
			if (ViewInfoManager != null) {
				ClearBackground(false);
				PaintErrorsCore();
			}
		}
		protected virtual void UpdateErrorsCore(SpellCheckErrorBase error, Position pos) {
			error.StartPosition = Position.Add(error.StartPosition, pos);
			error.FinishPosition = Position.Add(error.FinishPosition, pos);
		}
		protected virtual void UpdateErrors(ArrayList errors) {
			Position pos = EditControlWrapper.GetFirstPositionOfLine(EditControlWrapper.FirstVisibleLine);
			lock (errors.SyncRoot)
				foreach (SpellCheckErrorBase error in errors)
					UpdateErrorsCore(error, pos);
		}
		public virtual void Paint(ArrayList errors) {
			UpdateErrors(errors);
			ViewInfoManager.RequestDifference(errors);
		}
		public virtual void Paint(bool isTextChanged, bool isLayoutChanged) {
			if (!isTextChanged && !isLayoutChanged)
				Paint();  
			else
				if (isLayoutChanged && !isTextChanged) {  
					ViewInfoManager.ResetViewInfo();
					ViewInfoManager.RequestDifference(true);
				}
				else
					if (isTextChanged && !isLayoutChanged) 
						if (ViewInfoManager.UpdateErrorList())
							ViewInfoManager.RequestDifference(true);
						else
							Paint(); 
					else {  
						ViewInfoManager.UpdateErrorList();
						ViewInfoManager.RequestDifference(true);
					}
		}
		public virtual void Paint() {
			if (EditControlWrapper.Enabled && !EditControlWrapper.ReadOnly)
				PaintCore();
		}
		public virtual IEditControlWrapper EditControlWrapper {
			get {
				return this.editControlWrapper;
			}
			set {
				if (editControlWrapper != value)
					editControlWrapper = value;
			}
		}
		public void Dispose() {
			spellChecker = null;
			editControlWrapper = null;
			if (brush != null) {
				brush.Dispose();
				brush = null;
			}
		}
	}
	public class WavyLinePainter : PainterBase {
		public WavyLinePainter(IEditControlWrapper editControlWrapper, ViewInfoManager viewInfoManager, SpellChecker spellChecker) : base(editControlWrapper, viewInfoManager, spellChecker) { }
		protected override Brush CreateNewBrush() {
			Brush result = null;
			Bitmap image = new Bitmap(2 * ViewInfoBase.BrushHeight, ViewInfoBase.BrushHeight);
			try {
				Graphics gr = Graphics.FromImage(image);
				try {
					Pen pen = new Pen(SpellChecker.CheckAsYouTypeOptions.Color);
					try {
						gr.DrawLines(pen, new Point[] { new Point(0, ViewInfoBase.BrushHeight), new Point(ViewInfoBase.BrushHeight, 0), new Point(2 * ViewInfoBase.BrushHeight, ViewInfoBase.BrushHeight) });
						result = new TextureBrush(image, new Rectangle(1, 0, 2 * ViewInfoBase.BrushHeight - 1, ViewInfoBase.BrushHeight));
						(result as TextureBrush).WrapMode = WrapMode.TileFlipXY;
					}
					finally {
						pen.Dispose();
					}
				}
				finally {
					gr.Dispose();
				}
			}
			finally {
				image.Dispose();
			}
			return result;
		}
		protected override void FillErrorRectangle(Rectangle rect) {
			Brush.ResetTransform();
			Brush.TranslateTransform(rect.Left, rect.Top);
			base.FillErrorRectangle(rect);
		}
		protected virtual new TextureBrush Brush { get { return base.Brush as TextureBrush; } }
	}
	public class LinePainter : PainterBase {
		public LinePainter(IEditControlWrapper editControlWrapper, ViewInfoManager viewInfoManager, SpellChecker spellChecker) : base(editControlWrapper, viewInfoManager, spellChecker) { }
		protected override Brush CreateNewBrush() {
			return new SolidBrush(SpellChecker.CheckAsYouTypeOptions.Color);
		}
	}
	public class CheckAsYouTypeManager : IDisposable {
		Control editControl = null;
		SpellChecker spellChecker = null;
		AsyncChecker checker = null;
		bool needRecheck = false;
		ViewInfoManager viewInfoManager = null;
		IEditControlWrapper editControlWrapper = null;
		StringCollection ignoreAllList = new StringCollection();
		Dictionary<string, string> changeAllList = new Dictionary<string, string>();
		DictionaryHelper dictionaryHelper;
		ISpellCheckTextControlController textController = null;
		PainterBase painter = null;
		bool lockTextChanged = false;
		TextBoxHandler editControlHandler = null;
		CheckAsYouTypeOperationsManager operationsManager;
		bool isDisposing = false;
		public CheckAsYouTypeManager(SpellChecker spellChecker) {
			this.spellChecker = spellChecker;
			if (!SpellChecker.IsDesignMode)
				spellChecker.CheckAsYouTypeOptions.OptionChanged += new EventHandler(OnOptionChanged);
			operationsManager = new CheckAsYouTypeOperationsManager(this);
		}
		public virtual bool IsActive {
			get { return editControl != null; }
		}
		protected virtual void OnOptionChanged(object sender, EventArgs e) {
			if (Painter != null) {
				Painter.ClearBackground(true);
				Painter.Dispose();
				painter = null;
				Painter.Paint();
			}
		}
		public Control EditControl {
			get { return this.editControl; }
			set {
				if (EditControl != value) {
					if (EditControl != null)
						UnsubscribeFromEditControlEvents();
					if (viewInfoManager != null) {
						viewInfoManager.Dispose();
						viewInfoManager = null;
					}
					if (painter != null) {
						painter.Dispose();
						painter = null;
					}
					editControlWrapper = null;
					if (EditControl != null) {
						if (!EditControl.IsDisposed)
							EditControl.Update();
					}
					SetEditControl(value);
					if (EditControl != null)
						SubscribeToEditControlEvents();
				}
			}
		}
		public Dictionary<string, string> ChangeAllList { get { return this.changeAllList; } }
		public IIgnoreList IgnoreAllList { get { return SpellChecker.IgnoreList; } }
		#region Check
		protected virtual bool CanCheck(Control control) {
			TextBoxBase textBox = GetTextBoxInstance(control);
			return textBox != null;
		}
		public virtual void Check(Control fEditControl) {
			if (IsDisposing)
				return;
			if (!CanCheck(fEditControl) || !fEditControl.IsHandleCreated)
				return;
			EditControl = fEditControl;
			if (!Checker.IsWorking) {
				CheckCore();
			}
			else {
				NeedRecheck = true;
			}
		}
		void UnsubscribeFromEditControlEvents() {
			OptionsSpelling options = SpellChecker.GetSpellCheckerOptions(EditControl);
			if (options != null)
				options.OptionChanged -= OnSpellingOptionChanged;
			EditControl.Disposed -= OnEditControlDisposed;
		}
		void SubscribeToEditControlEvents() {
			OptionsSpelling options = SpellChecker.GetSpellCheckerOptions(EditControl);
			if (options != null)
				options.OptionChanged += OnSpellingOptionChanged;
			EditControl.Disposed += OnEditControlDisposed;
		}
		void OnEditControlDisposed(object sender, EventArgs e) {
			EditControl = null;
		}
		void OnSpellingOptionChanged(object sender, EventArgs e) {
			if (!SpellChecker.IsOptionsRestoring)
				NeedRecheck = true;
		}
		public virtual void Check() {
			if (EditControl != null)
				Check(EditControl);
		}
		protected virtual void CheckCore() {
			NeedRecheck = false;  
			OptionsSpelling actualOptions = new OptionsSpelling();
			actualOptions.Assign(SpellChecker.OptionsSpelling);
			actualOptions.CombineOptions(SpellChecker.GetSpellCheckerOptions(EditControl));
			Task<ArrayList> task = Checker.DoWork(EditControlWrapper.GetVisibleText(), actualOptions);
			task.ContinueWith((t) => {
				if (t.IsCompleted && t.Exception == null && EditControl != null && EditControl.IsHandleCreated)
					EditControl.BeginInvoke(new AsyncCheckingFinishedCallBack(AsyncCheckingFinished), t.Result);
			});
		}
		public virtual bool NeedRecheck {
			get { return this.needRecheck; }
			set {
				this.needRecheck = value;
				if (!Checker.IsWorking && NeedRecheck)
					Check(EditControl);
			}
		}
		#endregion
		protected internal bool LockTextChanged { get { return lockTextChanged; } set { lockTextChanged = value; } }
		protected internal DictionaryHelper DictionaryHelper {
			get {
				if (dictionaryHelper == null)
					dictionaryHelper = CreateDictionaryHelper();
				return dictionaryHelper;
			}
		}
		public virtual bool IsDisposing { get { return isDisposing; } }
		public virtual SpellChecker SpellChecker { get { return this.spellChecker; } }
		public CheckAsYouTypeOperationsManager OperationsManager {
			get {
				return operationsManager;
			}
		}
		protected virtual DictionaryHelper CreateDictionaryHelper() {
			return new DictionaryHelper(SpellChecker, SpellChecker.SharedDictionaries);
		}
		public virtual SpellCheckErrorBase GetErrorByClientPosition(Point clientPos) {
			Position pos = EditControlWrapper.GetPositionByPoint(clientPos);
			return ViewInfoManager.GetErrorByPosition(pos);
		}
		public virtual SpellCheckErrorBase GetErrorByCursorPos(int pos) {
			IntPosition position = new IntPosition(pos);
			return GetErrorByCursorPositionCore(position);
		}
		protected virtual TextBoxBase GetTextBoxInstance() {
			return GetTextBoxInstance(EditControl);
		}
		protected virtual TextBoxBase GetTextBoxInstance(Control control) {
			return GetTextBoxInstanceCore(control);
		}
		protected virtual TextBoxBase GetTextBoxInstanceCore(Control control) {
			return SpellCheckTextBoxBaseFinderManager.Default.GetTextBoxInstance(control);
		}
		protected virtual IEditControlWrapper CreateRichTextBoxControlWrapper(RichTextBox textBox) {
			return new RichTextBoxWrapper(textBox, Checker);
		}
		protected virtual IEditControlWrapper CreateTextBoxControlWrapper(TextBoxBase textBox) {
			return new TextBoxWrapper(textBox, Checker);
		}
		protected virtual IEditControlWrapper CreateEditControlWrapper() {
			TextBoxBase textBox = GetTextBoxInstance();
			if (textBox == null)
				return null;
			RichTextBox richTextBox = textBox as RichTextBox;
			if (richTextBox != null)
				return CreateRichTextBoxControlWrapper(richTextBox);
			else
				return CreateTextBoxControlWrapper(textBox);
		}
		protected virtual IEditControlWrapper CreateEditControlWrapper(Control c) {
			TextBoxBase textBox = GetTextBoxInstanceCore(c);
			RichTextBox richTextBox = textBox as RichTextBox;
			if (richTextBox != null)
				return CreateRichTextBoxControlWrapper(richTextBox);
			else
				return CreateTextBoxControlWrapper(textBox);
		}
		protected virtual IEditControlWrapper EditControlWrapper {
			get {
				if (editControlWrapper == null)
					editControlWrapper = CreateEditControlWrapper();
				return editControlWrapper;
			}
		}
		public virtual SpellCheckErrorBase GetErrorByCursorPosition() {
			return GetErrorByCursorPositionCore(EditControlWrapper.CurrentPosition);
		}
		protected virtual SpellCheckErrorBase GetErrorByCursorPositionCore(Position pos) {
			Point clientPos = EditControlWrapper.GetPointByPosition(pos);
			return GetErrorByClientPosition(clientPos);
		}
		#region ViewInfoManager
		protected virtual ViewInfoManager CreateViewInfoManager() {
			return new ViewInfoManager(EditControlWrapper);
		}
		protected internal virtual ViewInfoManager ViewInfoManager {
			get {
				if (viewInfoManager == null) {
					viewInfoManager = CreateViewInfoManager();
					viewInfoManager.DifferenceReady += new EventHandler(OnDifferenceReady);
				}
				return viewInfoManager;
			}
		}
		void OnDifferenceReady(object sender, EventArgs e) {
			Painter.Paint();
		}
		#endregion
		protected virtual AsyncChecker Checker {
			get {
				if (checker == null)
					checker = CreateChecker();
				return checker;
			}
		}
		protected virtual AsyncChecker CreateChecker() {
			return new AsyncChecker(spellChecker);
		}
		protected virtual void SetEditControl(Control value) {
			this.editControl = value;
			if (EditControl != null) {
				DisposeEditControlHandler();
				EditContolHandler.HandleEditControlNotifications();
			}
		}
		protected virtual TextBoxHandler EditContolHandler {
			get {
				if (editControlHandler == null) {
					editControlHandler = CreateEditControlHandler();
					editControlHandler.NeedPaint += new NeedPaintEventHandler(OnNeedPaint);
					editControlHandler.NeedClearErrors += new EventHandler(OnNeedClearErrors);
					editControlHandler.ResetViewInfo += new EventHandler(OnResetViewInfo);
				}
				return editControlHandler;
			}
		}
		void OnResetViewInfo(object sender, EventArgs e) {
			ViewInfoManager.IsReady = false;
		}
		protected virtual void OnNeedClearErrors(object sender, EventArgs e) {
			Painter.ClearBackground(true);
		}
		protected virtual void OnNeedPaint(object sender, NeedPaintEventArgs e) {
			if (ViewInfoManager.IsReady)
				Painter.Paint(e.TextChanged, e.LayoutChanged);
			else
				ViewInfoManager.RequestDifference(true);
		}
		protected virtual bool IsRichTextBoxChecked() {
			TextBoxBase textBox = GetTextBoxInstance();
			return textBox is RichTextBox;
		}
		protected virtual bool IsTextBoxChecked() {
			TextBoxBase textBox = GetTextBoxInstance();
			return textBox is TextBox;
		}
		protected virtual TextBoxHandler CreateEditControlHandler() {
			if (IsRichTextBoxChecked())
				return new RichTextBoxHandler(this, EditControlWrapper, EditControl);
			else
				if (IsTextBoxChecked())
					return new TextBoxHandler(this, EditControlWrapper, EditControl);
				else
					return null;
		}
		void EditControl_Disposed(object sender, EventArgs e) {
			if (SpellChecker != null)
				SpellChecker.DisposeManager();
		}
		void EditControl_VisibleChanged(object sender, EventArgs e) {
			if (!(sender as Control).Visible && SpellChecker != null) {
				SpellChecker.DisposeManager();
			}
		}
		protected virtual ISpellCheckTextControlController CreateTextController() {
			return SpellCheckTextControllersManager.Default.GetSpellCheckTextControlController(EditControl);
		}
		protected internal virtual ISpellCheckTextControlController TextController {
			get {
				if (textController == null)
					textController = CreateTextController();
				return this.textController;
			}
		}
		protected virtual PainterBase CreateWavyLinePainter() {
			return new WavyLinePainter(EditControlWrapper, ViewInfoManager, SpellChecker);
		}
		protected virtual PainterBase CreateLinePainter() {
			return new LinePainter(EditControlWrapper, ViewInfoManager, SpellChecker);
		}
		protected virtual PainterBase CreatePainter() {
			if (EditControlWrapper == null)
				return null;
			if (!EditControlWrapper.IsSingleLine)
				switch (SpellChecker.CheckAsYouTypeOptions.UnderlineStyle) {
					case UnderlineStyle.WavyLine: return CreateWavyLinePainter();
					case UnderlineStyle.Line: return CreateLinePainter();
					default:
						throw new Exception("The " + SpellChecker.CheckAsYouTypeOptions.UnderlineStyle.ToString() + " is not currently supported");
				}
			else
				return CreateLinePainter();
		}
		protected PainterBase Painter {
			get {
				if (painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected virtual bool CanPaintErrors() {
			return !IsDisposing && EditControl != null && !EditControl.IsDisposed;
		}
		protected virtual void PaintErrors(ArrayList errors) {
			if (CanPaintErrors())  
				Painter.Paint(errors);
		}
		delegate void PaintErrorsDelegate(ArrayList errors);
		delegate void CheckCoreDelegate();
		protected virtual void AsyncCheckingFinished(ArrayList errors) {
			if (CanPaintErrors()) { 
				if (!NeedRecheck)
					PaintErrors(errors);
				else
					CheckCore();
			}
		}
		public virtual bool IsTextChanged() {
			return string.Compare(Checker.Text, EditControlWrapper.GetVisibleText(), false, SpellChecker.Culture) != 0;
		}
		protected virtual void OnCheckNeeded(object sender, EventArgs e) {
			if (IsTextChanged())
				Check(editControl);
			else
				if (!EditControlWrapper.CanScrollTextHorizontally)
					Painter.Paint(false, false);
				else
					Check(editControl);  
		}
		protected virtual void DisposeAsyncChecker() {
			if (checker != null) {
				AsyncChecker oldChecker = Checker;
				checker = null;
				oldChecker.Dispose();
			}
		}
		protected virtual void DisposeEditControlHandler() {
			if (editControlHandler != null) {
				editControlHandler.NeedPaint -= new NeedPaintEventHandler(OnNeedPaint);
				editControlHandler.NeedClearErrors -= new EventHandler(OnNeedClearErrors);
				editControlHandler.ResetViewInfo -= new EventHandler(OnResetViewInfo);
				editControlHandler.Dispose();
				editControlHandler = null;
			}
		}
		protected virtual void DisposeViewInfoManager() {
			if (viewInfoManager == null) {
				viewInfoManager.DifferenceReady -= new EventHandler(OnDifferenceReady);
				viewInfoManager = null;
			}
		}
		public void Dispose() {
#if DEBUG
			System.Diagnostics.Debug.WriteLine("Start Disposing");
#endif
			if (isDisposing) return;
			isDisposing = true;
#if DEBUG
			System.Diagnostics.Debug.WriteLine("IsDisposing = true");
#endif
			if (IsActive) {
#if DEBUG
				System.Diagnostics.Debug.WriteLine("IsActive = true");
#endif
#if DEBUG
				System.Diagnostics.Debug.WriteLine("DoAfterCheck");
#endif
			}
			DisposeEditControlHandler();
#if DEBUG
			System.Diagnostics.Debug.WriteLine("DisposeEditControlHandler");
#endif
			DisposeAsyncChecker();
#if DEBUG
			System.Diagnostics.Debug.WriteLine("DisposeAsyncChecker");
#endif
			if (editControl != null) {
				if (!editControl.IsDisposed && editControl.IsHandleCreated)
					editControl.Refresh();
				UnsubscribeFromEditControlEvents();
				editControl = null;
			}
			if (spellChecker != null)
				spellChecker.CheckAsYouTypeOptions.OptionChanged -= new EventHandler(OnOptionChanged);
			spellChecker = null;
			editControlWrapper = null;
		}
		public SpellCheckErrorBase GetErrorByWord(string word) {
			SpellCheckErrorBase result = null;
			lock (Checker) {
				result = Checker.GetErrorByWord(word);
			}
			return result;
		}
	}
	public class CheckAsYouTypeOperationsManager : WinTextOperationsManager {
		CheckAsYouTypeManager manager;
		public CheckAsYouTypeOperationsManager(CheckAsYouTypeManager manager)
			: base(null) {
			this.manager = manager;
		}
		public new ISpellCheckTextControlController TextController { get { return Manager.TextController; } }
		public CheckAsYouTypeManager Manager {
			get {
				return manager;
			}
		}
		public override SpellChecker SpellChecker {
			get {
				return Manager.SpellChecker;
			}
		}
		public virtual void AddToDictionary(Position startPosition, Position finishPosition, string word, System.Globalization.CultureInfo culture) {
			AddToDictionary(word, culture);
		}
		protected override SpellCheckerCommand CreateCommand(SpellCheckOperation operation) {
			if (operation == SpellCheckOperation.Change)
				return new WinSpellCheckerChangeCommand(this, SpellChecker);
			if (operation == SpellCheckOperation.Delete)
				return new WinSpellCheckerDeleteCommand(this, SpellChecker);
			if (operation == SpellCheckOperation.AddToDictionary)
				return new WinSpellCheckerAddToDictionaryCommand(this);
			if (operation == SpellCheckOperation.Ignore)
				return new WinSpellCheckerIgnoreCommand(this);
			if (operation == SpellCheckOperation.IgnoreAll)
				return new WinSpellCheckerIgnoreAllCommand(this);
			if (operation == SpellCheckOperation.None)
				return new WinSpellCheckerNoSuggestionsCommand(this);
			return base.CreateCommand(operation);
		}
		public override IIgnoreList IgnoreList { get { return Manager.IgnoreAllList; } }
		public override Dictionary<string, string> ChangeAllList { get { return Manager.ChangeAllList; } }
		public override void IgnoreAll(string word) {
			IgnoreAllWord(word);
		}
		public override DictionaryHelper DictionaryHelper {
			get {
				return Manager.DictionaryHelper;
			}
		}
		public virtual void ReplaceWord(SpellCheckErrorBase currentError) {
			ReplaceWord(currentError.StartPosition, currentError.FinishPosition, currentError.Suggestion);
		}
		public virtual void ReplaceWord(Position startPosition, Position finishPosition, string suggestion) {
			Manager.LockTextChanged = true;
			try {
				TextController.Text = TextController.EditControlText;
				string word = TextController.GetWord(startPosition, finishPosition);
				TextController.ReplaceWord(startPosition, finishPosition, suggestion);
				Position selectionStart = Position.Add(startPosition, TextController.GetTextLength(suggestion));
				TextController.Select(selectionStart, selectionStart);
				Manager.ViewInfoManager.IgnoreWord(word);
			}
			finally {
				Manager.LockTextChanged = false;
				Manager.NeedRecheck = true;
			}
		}
		public virtual void Delete(SpellCheckErrorBase currentError) {
			Delete(currentError.StartPosition, currentError.FinishPosition);
		}
		public override void Delete(Position startPosition, Position finishPosition) {
			Manager.LockTextChanged = true;
			try {
				TextController.Text = TextController.EditControlText;
				Position start = startPosition;
				Position finish = finishPosition;
				Position pos = TextController.GetPrevPosition(start);
				TextController.DeleteWord(ref start, ref finish);
				TextController.Select(pos, pos);
			}
			finally {
				Manager.LockTextChanged = false;
				Manager.NeedRecheck = true;
			}
		}
		public virtual bool CanAddAddToDictionaryItem(System.Globalization.CultureInfo culture) {
			return DictionaryHelper.GetCustomDictionary(culture) != null;
		}
		protected virtual void RemoveWordFromErrorsList(string word) {
			Manager.ViewInfoManager.IgnoreWord(word);
		}
		public virtual void IgnoreAllWord(string word) {
			IgnoreList.Add(word);
			RemoveWordFromErrorsList(word);
		}
		public override void IgnoreOnce(Position start, Position finish, string word) {
			IgnoreListItem item = new IgnoreListItem();
			item.Start = start;
			item.End = finish;
			item.Word = word;
			IgnoreList.Add(start, finish, word);
			Manager.ViewInfoManager.IgnoreIgnoreItem(item);
		}
		public void AddToDictionary(string word, System.Globalization.CultureInfo culture) {
			DictionaryHelper.AddWord(word, culture);
			SpellChecker.OnWordAdded(word);
			RemoveWordFromErrorsList(word);
		}
		public virtual bool IsException(Position startPosition, Position finishPosition, string word) {
			IIgnoreList ignoreList = SpellChecker.GetIgnoreListCore();
			return ignoreList.Contains(startPosition, finishPosition, word);
		}
		public override void DoSpellCheckOperation(SpellCheckOperation operation, string suggestion, SpellCheckErrorBase error) {
			if (!Manager.IsActive)
				return;
			switch (operation) {
				case SpellCheckOperation.AddToDictionary:
					RemoveWordFromErrorsList(error.WrongWord);
					break;
				case SpellCheckOperation.Change:
				case SpellCheckOperation.SilentChange:
					if (!DictionaryHelper.FindWord(suggestion, error.Culture)) {
						Manager.NeedRecheck = true;
					}
					break;
				case SpellCheckOperation.Undo:
					Manager.NeedRecheck = true;
					break;
				case SpellCheckOperation.ChangeAll:
					if (ChangeAllList.ContainsKey(error.WrongWord))
						ChangeAllList.Remove(error.WrongWord);
					ChangeAllList.Add(error.WrongWord, error.Suggestion);
					break;
				case SpellCheckOperation.IgnoreAll:
					Manager.ViewInfoManager.IgnoreWord(error.WrongWord);
					break;
				case SpellCheckOperation.Ignore:
					Manager.ViewInfoManager.IgnoreIgnoreItem(error.StartPosition, error.FinishPosition);
					break;
				case SpellCheckOperation.Delete:
					Manager.ViewInfoManager.IgnoreWord(error.WrongWord);
					Manager.NeedRecheck = true;
					break;
			}
		}
	}
	public class TextBoxWrapper : IEditControlWrapper {
		const int EM_GETFIRSTVISIBLELINE = 0x00CE;
		const int EM_LINELENGTH = 0x00C1;
		TextBoxBase textBox = null;
		Hashtable indents = new Hashtable();
		AsyncChecker checker;
		public TextBoxWrapper(TextBoxBase textBox, AsyncChecker checker) {
			this.textBox = textBox;
			this.checker = checker;
		}
		public TextBoxBase TextBox { get { return this.textBox; } }
		public bool Enabled { get { return TextBox.Enabled; } }
		public bool ReadOnly { get { return TextBox.ReadOnly; } }
		public Graphics GetGraphics() {
			Graphics graphics = Graphics.FromHwnd(textBox.Handle);
			return graphics;
		}
		protected IntPosition GetIntPositionInstance(Position pos) {
			if (pos == null)
				return IntPosition.Null;
			return pos as IntPosition;
		}
		public Point GetPointByLine(int lineNumber) {
			int index = textBox.GetFirstCharIndexFromLine(lineNumber);
			return GetPointByPosition(new IntPosition(index));
		}
		public int GetLineByPosition(Position pos) {
			return textBox.GetLineFromCharIndex(GetIntPositionInstance(pos).ActualIntPosition);
		}
		public Position CurrentPosition { get { return new IntPosition(TextBox.SelectionStart); } }
		protected virtual int GetIndent(Font font) {
			if (indents[font] != null)
				return Convert.ToInt32(indents[font]);
			else {
				indents[font] = GetIndentCore(font);
				return GetIndent(font);
			}
		}
		protected virtual int GetIndentCore(Font font) {
			Graphics gr = GetGraphics();
			try {
				return UnderlineTextPositionCalculator.CalculateUnderlinePosition(gr, font);
			}
			finally {
				gr.Dispose();
				gr = null;
			}
		}
		protected virtual bool NeedUpdatePointByLastLetter(int pos) {
			return pos == TextBox.Text.Length;
		}
		protected virtual Point GetPointByPositionCore(Position pos) {
			int intPos = GetIntPositionInstance(pos).ActualIntPosition;
			int lastletterWidth = 0;
			if (NeedUpdatePointByLastLetter(intPos)) {
				intPos--;
				if (Text.Length > 0)
					lastletterWidth = DevExpress.Utils.Text.TextUtils.GetStringSize(GetGraphics(), Text[Text.Length - 1].ToString(), TextBox.Font).Width;
				else
					return Point.Empty;
			}
			Point result = textBox.GetPositionFromCharIndex(intPos);
			result.Y += GetIndent(textBox.Font);
			result.X += lastletterWidth;
			return result;
		}
		delegate bool IsLineVisibleInvoker(int i);
		protected bool ThreadSafeIsLineVisible(int i) {
			if (textBox.InvokeRequired) {
				IsLineVisibleInvoker methodInvoker = new IsLineVisibleInvoker(IsLineVisible);
				IAsyncResult asyncResult = textBox.BeginInvoke(methodInvoker, new object[] { i });
				if (WaitHandle.WaitAny(new WaitHandle[] { asyncResult.AsyncWaitHandle, checker.ExitEvent }) == 0)
					return Convert.ToBoolean(textBox.EndInvoke(asyncResult));
				else
					return false;
			}
			return IsLineVisible(i);
		}
		protected bool IsLineVisible(int i) {
			int charIndex = GetFirstCharIndexFromLine(i);
			if (charIndex == -1)
				return false;
			return textBox.GetPositionFromCharIndex(charIndex).Y < GetHeight();
		}
		delegate int GetFirstCharIndexFromLineInvoker(int i);
		protected virtual int GetFirstCharIndexFromLine(int i) {
			return (int)SafeNativeMethods.SendMessage(textBox.Handle, 0xbb, new IntPtr(i), IntPtr.Zero).ToInt64();
		}
		protected int ThreadSafeGetFirstCharIndexFromLine(int i) {
			if (textBox.InvokeRequired) {
				GetFirstCharIndexFromLineInvoker methodInvoker = new GetFirstCharIndexFromLineInvoker(GetFirstCharIndexFromLine);
				IAsyncResult asyncResult = textBox.BeginInvoke(methodInvoker, new object[] { i });
				if (WaitHandle.WaitAny(new WaitHandle[] { asyncResult.AsyncWaitHandle, checker.ExitEvent }) == 0)
					return Convert.ToInt32(textBox.EndInvoke(asyncResult));
				else
					return 0;
			}
			return GetFirstCharIndexFromLine(i);
		}
		public virtual string GetVisibleText() {
			if (IsSingleLine)
				return Text;
			int startLine = FirstVisibleLine;
			int finishLine = startLine;
			for (int i = finishLine; i < int.MaxValue; i++) {
				if (!ThreadSafeIsLineVisible(i)) {
					finishLine = i;
					break;
				}
			}
			int startIndex = ThreadSafeGetFirstCharIndexFromLine(startLine);
			int lastIndex = ThreadSafeGetFirstCharIndexFromLine(finishLine);
			if (lastIndex == -1)
				lastIndex = Text.Length;
			return Text.Substring(startIndex, lastIndex - startIndex);
		}
		public Point GetPointByPosition(Position pos) {
			return GetPointByPositionCore(pos);
		}
		public void InvalidateRect(Rectangle rect) {
			TextBox.Invalidate(rect);
		}
		public Position GetPositionByPoint(System.Drawing.Point p) {
			return new IntPosition(textBox.GetCharIndexFromPosition(p));
		}
		delegate int GetFirstVisibleLineInvoker();
		protected virtual int GetFirstVisibleLine() {
			return (int)SafeNativeMethods.SendMessage(textBox.Handle, EM_GETFIRSTVISIBLELINE, IntPtr.Zero, IntPtr.Zero).ToInt64();
		}
		public int FirstVisibleLine {
			get {
				if (textBox.InvokeRequired) {
					GetFirstVisibleLineInvoker methodInvoker = new GetFirstVisibleLineInvoker(GetFirstVisibleLine);
					IAsyncResult asyncResult = textBox.BeginInvoke(methodInvoker, null);
					if (WaitHandle.WaitAny(new WaitHandle[] { asyncResult.AsyncWaitHandle, checker.ExitEvent }) == 0)
						return Convert.ToInt32(textBox.EndInvoke(asyncResult));
					else
						return 0;
				}
				return SafeNativeMethods.SendMessage(textBox.Handle, EM_GETFIRSTVISIBLELINE, IntPtr.Zero, IntPtr.Zero).ToInt32();
			}
		}
		public Position FirstVisibleIndex {
			get {
				return GetPositionByPoint(new Point(0, 0));
			}
		}
		public int GetWidth() {
			return TextBox.Width;
		}
		public int GetHeight() {
			return TextBox.Height;
		}
		public Position SelectionLength {
			get { return new IntPosition(TextBox.SelectionLength); }
		}
		delegate string TextBoxTextPropertyInvoker();
		public string Text {
			get {
				if (TextBox.InvokeRequired) {
					IAsyncResult asyncResult = textBox.BeginInvoke(new TextBoxTextPropertyInvoker(GetTextBoxText));
					if (WaitHandle.WaitAny(new WaitHandle[] { asyncResult.AsyncWaitHandle, checker.ExitEvent }) == 0)
						return Convert.ToString(textBox.EndInvoke(asyncResult));
					else
						return string.Empty;
				}
				return TextBox.Text;
			}
			set { TextBox.Text = value; }
		}
		private string GetTextBoxText() {
			return TextBox.Text;
		}
		public Color GetBackColor() {
			return TextBox.BackColor;
		}
		public bool CanScrollTextHorizontally {
			get { return !TextBox.Multiline || (TextBox.Multiline && !TextBox.WordWrap); }
		}
		public bool IsSingleLine {
			get { return !TextBox.Multiline; }
		}
		public Position GetFirstPositionOfLine(int line) {
			return new IntPosition(textBox.GetFirstCharIndexFromLine(line));
		}
		public Position GetLastPositionOfLine(Position firstPosition) {
			IntPosition intPosition = firstPosition as IntPosition;
			if (intPosition == null)
				return firstPosition;
			int lineLength = SafeNativeMethods.SendMessage(TextBox.Handle, EM_LINELENGTH, new IntPtr(intPosition.ActualIntPosition), IntPtr.Zero).ToInt32();
			if (lineLength == 0)
				return firstPosition;
			return Position.Add(intPosition, new IntPosition(lineLength - 1));
		}
	}
	internal class UnderlineTextPositionCalculator {
		[DllImport("gdi32.dll", EntryPoint = "GetOutlineTextMetricsA")]
		public static extern uint GetOutlineTextMetrics(IntPtr hdc, uint cbData, IntPtr ptrZero);
		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hGdiObj);
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
		#region LOGFONT
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), ComVisible(false)]
		public class LOGFONT {
			public int lfHeight = 0;
			public int lfWidth = 0;
			public int lfEscapement = 0;
			public int lfOrientation = 0;
			public int lfWeight = 0;
			public byte lfItalic = 0;
			public byte lfUnderline = 0;
			public byte lfStrikeOut = 0;
			public byte lfCharSet = 0;
			public byte lfOutPrecision = 0;
			public byte lfClipPrecision = 0;
			public byte lfQuality = 0;
			public byte lfPitchAndFamily = 0;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string lfFaceName = null;
		}
		#endregion
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateFont(
			int nHeight,
			int nWidth,
			int nEscapement,
			int nOrientation,
			int fnWeight,
			int fdwItalic,
			int fdwUnderline,
			int fdwStrikeOut,
			int fdwCharSet,
			int fdwOutputPrecision,
			int fdwClipPrecision,
			int fdwQuality,
			int fdwPitchAndFamily,
			string lpszFace
		);
		#region POINT
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int X;
			public int Y;
			public POINT(int x, int y) {
				this.X = x;
				this.Y = y;
			}
			public static implicit operator System.Drawing.Point(POINT p) {
				return new Point(p.X, p.Y);
			}
			public static implicit operator POINT(Point p) {
				return new POINT(p.X, p.Y);
			}
		}
		#endregion
		#region RECT
		[Serializable, StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
			public RECT(int left, int top, int right, int bottom) {
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}
			public int Width { get { return Right - Left; } }
			public int Height { get { return Bottom - Top; } }
			public Size Size { get { return new Size(Width, Height); } }
			public Point Location { get { return new Point(Left, Top); } }
			public Rectangle ToRectangle() {
				return Rectangle.FromLTRB(Left, Top, Right, Bottom);
			}
			public static RECT FromRectangle(Rectangle rectangle) {
				return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
			}
			public override int GetHashCode() {
				return Left ^ ((Top << 13) | (Top >> 0x13)) ^
					((Width << 0x1a) | (Width >> 6)) ^
					((Height << 7) | (Height >> 0x19));
			}
			public static implicit operator Rectangle(RECT r) {
				return Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
			}
			public static implicit operator RECT(Rectangle r) {
				return new RECT(r.Left, r.Top, r.Right, r.Bottom);
			}
		}
		#endregion
		#region TEXTMETRIC
		[Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct TEXTMETRIC {
			public int tmHeight;
			public int tmAscent;
			public int tmDescent;
			public int tmInternalLeading;
			public int tmExternalLeading;
			public int tmAveCharWidth;
			public int tmMaxCharWidth;
			public int tmWeight;
			public int tmOverhang;
			public int tmDigitizedAspectX;
			public int tmDigitizedAspectY;
			public byte tmFirstChar;
			public byte tmLastChar;
			public byte tmDefaultChar;
			public byte tmBreakChar;
			public byte tmItalic;
			public byte tmUnderlined;
			public byte tmStruckOut;
			public byte tmPitchAndFamily;
			public byte tmCharSet;
		}
		#endregion
		#region PANOSE
		[StructLayout(LayoutKind.Sequential)]
		public struct PANOSE {
			public byte bFamilyType;
			public byte bSerifStyle;
			public byte bWeight;
			public byte bProportion;
			public byte bContrast;
			public byte bStrokeVariation;
			public byte bArmStyle;
			public byte bLetterform;
			public byte bMidline;
			public byte bXHeight;
		}
		#endregion
		#region OUTLINETEXTMETRIC
		[StructLayout(LayoutKind.Sequential)]
		public struct OUTLINETEXTMETRIC {
			public uint otmSize;
			public TEXTMETRIC otmTextMetrics;
			public byte otmFiller;
			public PANOSE otmPanoseNumber;
			public uint otmfsSelection;
			public uint otmfsType;
			public int otmsCharSlopeRise;
			public int otmsCharSlopeRun;
			public int otmItalicAngle;
			public uint otmEMSquare;
			public int otmAscent;
			public int otmDescent;
			public uint otmLineGap;
			public uint otmsCapEmHeight;
			public uint otmsXHeight;
			public RECT otmrcFontBox;
			public int otmMacAscent;
			public int otmMacDescent;
			public uint otmMacLineGap;
			public uint otmusMinimumPPEM;
			public POINT otmptSubscriptSize;
			public POINT otmptSubscriptOffset;
			public POINT otmptSuperscriptSize;
			public POINT otmptSuperscriptOffset;
			public uint otmsStrikeoutSize;
			public int otmsStrikeoutPosition;
			public int otmsUnderscoreSize;
			public int otmsUnderscorePosition;
			public uint otmpFamilyName;
			public uint otmpFaceName;
			public uint otmpStyleName;
			public uint otmpFullName;
		}
		#endregion
		public static int CalculateUnderlineAndStrikeoutParameters(Graphics gr, Font font) {
			return CalculateUnderlineAndStrikeoutParametersCore(gr, font);
		}
		[System.Security.SecuritySafeCritical]
		static int CalculateUnderlineAndStrikeoutParametersCore(Graphics gr, Font font) {
			IntPtr fontHandle = CreateGdiFont(font);
			try {
				IntPtr hdc = gr.GetHdc();
				try {
					IntPtr oldFontHandle = SelectObject(hdc, fontHandle);
					try {
						return CalculateUnderlineAndStrikeoutParameters(hdc);
					}
					finally {
						SelectObject(hdc, oldFontHandle);
					}
				}
				finally {
					gr.ReleaseHdc(hdc);
				}
			}
			finally {
				DeleteObject(fontHandle);
			}
		}
		[System.Security.SecuritySafeCritical]
		static IntPtr CreateGdiFont(Font font) {
			LOGFONT lf = new LOGFONT();
			font.ToLogFont(lf);
			if (font.Unit != GraphicsUnit.Point)
				lf.lfHeight = (int)-font.Size;
			IntPtr hFont = CreateFont(lf.lfHeight, lf.lfWidth, lf.lfEscapement, lf.lfOrientation, lf.lfWeight, lf.lfItalic, lf.lfUnderline, lf.lfStrikeOut, lf.lfCharSet, lf.lfOutPrecision, lf.lfClipPrecision, lf.lfQuality, lf.lfPitchAndFamily, lf.lfFaceName);
			return hFont;
		}
		[System.Security.SecuritySafeCritical]
		protected static int CalculateUnderlineAndStrikeoutParameters(IntPtr hdc) {
			uint bufferSize = GetOutlineTextMetrics(hdc, 0, IntPtr.Zero);
			if (bufferSize == 0)
				return -1;
			IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
			try {
				if (GetOutlineTextMetrics(hdc, bufferSize, buffer) != 0) {
					OUTLINETEXTMETRIC otm = new OUTLINETEXTMETRIC();
					otm = (OUTLINETEXTMETRIC)Marshal.PtrToStructure(buffer, typeof(OUTLINETEXTMETRIC));
					return CalculateUnderlineAndStrikeoutParametersCore(otm);
				}
				else
					return -1;
			}
			finally {
				Marshal.FreeHGlobal(buffer);
			}
		}
		static int CalculateUnderlineAndStrikeoutParametersCore(OUTLINETEXTMETRIC otm) {
			return -otm.otmsUnderscorePosition;
		}
		public static int CalculateUnderlinePosition(Graphics gr, Font font) {
			int lineSpacing = (int)Math.Ceiling(ConvertToFontUnits(font, GetLineHeight(font)));
			return (int)Math.Ceiling((lineSpacing + CalculateUnderlineAndStrikeoutParameters(gr, font)) * gr.DpiY / 96.0);
		}
		static float ConvertToFontUnits(Font font, float designSize) {
			FontFamily family = font.FontFamily;
			float sizeInUnits = font.Size;
			float emSize = family.GetEmHeight(font.Style);
			return (sizeInUnits / emSize) * designSize;
		}
		static float GetLineHeight(Font font) {
			FontFamily family = font.FontFamily;
			FontStyle style = font.Style;
			return family.GetCellAscent(style) + family.GetCellDescent(style);
		}
	}
	public class TextBoxNativeWindowWrapper : NativeWindow, IDisposable {
		WeakReference editControlRef;
		EventHandler textChanged;
		EventHandler scrolled;
		ScrollingEventHandler scrolling;
		EventHandler checkNeeded;
		PaintEventHandler paint;
		EventHandler charPressed;
		bool lockTextChanged = false;
		[SecuritySafeCritical]
		public TextBoxNativeWindowWrapper(TextBoxBase textBox) {
			this.editControlRef = new WeakReference(textBox);
			if (EditControl.IsHandleCreated)
				AssignHandle(EditControl.Handle);
			SubscribeToEditControlEvents();
			GC.KeepAlive(textBox);
		}
		protected virtual void SubscribeToEditControlEvents() {
			EditControl.TextChanged += OnEditTextChanged;
			EditControl.FontChanged += OnFontChanged;
			if (EditControl is BaseEdit)
				((BaseEdit)EditControl).PropertiesChanged += TextBoxHandler_PropertiesChanged;
			EditControl.HandleCreated += EditControl_HandleCreated; 
			EditControl.HandleDestroyed += EditControl_HandleDestroyed;
		}
		protected virtual void UnsubscribeFromEditControlEvents() {
			EditControl.TextChanged -= OnEditTextChanged;
			EditControl.FontChanged -= OnFontChanged;
			if (EditControl is BaseEdit)
				((BaseEdit)EditControl).PropertiesChanged -= TextBoxHandler_PropertiesChanged;
			EditControl.HandleCreated -= EditControl_HandleCreated; 
			EditControl.HandleDestroyed -= EditControl_HandleDestroyed;
		}
		[SecuritySafeCritical]
		void EditControl_HandleDestroyed(object sender, EventArgs e) {
			ReleaseHandle();
		}
		[SecuritySafeCritical]
		void EditControl_HandleCreated(object sender, EventArgs e) {
			AssignHandle(EditControl.Handle);
		}
		protected internal bool LockTextChanged {
			get { return lockTextChanged; }
			set { lockTextChanged = value; }
		}
		public Control EditControl { get { return editControlRef.Target as Control; } }
		void TextBoxHandler_PropertiesChanged(object sender, EventArgs e) {
			OnPaint(PaintReason.LayoutChanged);
		}
		internal const int WM_CHAR = 0x0102, WM_MOUSEWHEEL = 0x020A, WM_HSCROLL = 0x0114, WM_VSCROLL = 0x0115, WM_PAINT = 0x000F, WM_KEYDOWN = 0x0100, WM_KEYUP = 0x0101, WM_PASTE = 0x302,
			WM_CUT = 0x300, WM_COPY = 0x301, WM_LBUTTONDOWN = 0x0201, WM_LBUTTONUP = 0x0202, WM_LBUTTONDBLCLK = 0x0203, WM_RBUTTONDOWN = 0x0204, WM_MOUSEMOVE = 0x0200, MK_LBUTTON = 0x0001, WM_SIZE = 0x0005,
			EM_UNDO = 0xC7, VK_CONTROL = 0x011;
		[SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		protected override void WndProc(ref Message m) {
			if (m.Msg == WM_CHAR)
				this.lockTextChanged = true;
			base.WndProc(ref m);
			ProcessWindowsMessage(m);
		}
		[SecuritySafeCritical]
		void ProcessWindowsMessage(Message m) {
			switch (m.Msg) {
				case WM_CHAR:
					TranslateCharMessage(m);
					break;
				case WM_MOUSEWHEEL:
					TranslateMouseWheelMessage(m);
					break;
				case WM_HSCROLL:
					TranslateHScrollMessage(m);
					break;
				case WM_VSCROLL:
					TranslateVScrollMessage(m);
					break;
				case WM_SIZE:
					TranslateSizeMessage(m);
					break;
				case WM_PAINT:
					TranslatePaintMessage(m);
					break;
				case WM_KEYDOWN:
					TranslateKeyDownMessage(m);
					break;
				case WM_KEYUP:
					TranslateKeyUpMessage(m);
					break;
				case WM_PASTE:
					TranslatePasteMessage(m);
					break;
				case WM_CUT:
					TranslateCutMessage(m);
					break;
				case EM_UNDO:
					TranslateUndoMessate(m);
					break;
				case WM_LBUTTONUP:
					TranslateLeftButtonUp(m);
					break;
				case WM_LBUTTONDOWN:
					TranslateLeftButtonDown(m);
					break;
				case WM_LBUTTONDBLCLK:
					TranslateDoubleClick(m);
					break;
				case WM_MOUSEMOVE:
					TranslateMouseMove(m);
					break;
			}
		}
		private void TranslateUndoMessate(Message m) {
			OnCheckNeeded();
		}
		private void TranslateDoubleClick(Message m) {
			OnPaint(PaintReason.Unknown);
		}
		private void TranslateLeftButtonDown(Message m) {
			OnPaint(PaintReason.Unknown);
		}
		private void TranslateLeftButtonUp(Message m) {
			OnScrolled(); 
			OnCheckNeeded();
		}
		protected virtual void TranslateSizeMessage(Message m) {
			if (EditControl != null && !String.IsNullOrEmpty(EditControl.Text)) {
				OnCheckNeeded();
				OnPaint(PaintReason.LayoutChanged);
			}
		}
		[SecuritySafeCritical]
		private void TranslateMouseMove(Message m) {
			int btns = m.WParam.ToInt32();
			if ((btns & MK_LBUTTON) != 0) {
				OnScrolling(ScrollReason.Keyboard, ScrollDirection.Undefined); 
				OnPaint(PaintReason.Unknown); 
			}
		}
		private void TranslateCutMessage(Message m) {
			OnTextChanged();
		}
		private void TranslatePasteMessage(Message m) {
			OnTextChanged();
		}
		[SecuritySafeCritical]
		protected virtual void TranslateKeyDownMessage(Message m) {
			int key = (int)m.WParam;
			switch ((Keys)key) {
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
				case Keys.PageDown:
				case Keys.PageUp:
				case Keys.Home:
				case Keys.End:
					OnScrolling(ScrollReason.Keyboard, ScrollDirection.Undefined);
					break;
				case Keys.A:
					if (Control.ModifierKeys == Keys.Control && EditControl != null) {
						EditControl.BeginInvoke(new MethodInvoker(delegate {
							OnScrolling(ScrollReason.Keyboard, ScrollDirection.Undefined);
						}));
					}
					break;
			}
		}
		[SecuritySafeCritical]
		protected virtual void TranslateKeyUpMessage(Message m) {
			int key = (int)m.WParam;
			switch ((Keys)key) {
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
				case Keys.PageDown:
				case Keys.PageUp:
				case Keys.Home:
				case Keys.End:
					OnScrolled();
					OnCheckNeeded();
					break;
				case Keys.A:
					if (Control.ModifierKeys == Keys.Control && EditControl != null) {
						EditControl.BeginInvoke(new MethodInvoker(delegate {
							OnScrolled();
							OnCheckNeeded();
						}));
					}
					break;
				case Keys.Delete:
					OnCheckNeeded();
					break;
			}
		}
		protected virtual void TranslatePaintMessage(Message m) {
			OnPaint(PaintReason.Unknown);
		}
		const int SB_ENDSCROLL = 8;
		[SecuritySafeCritical]
		protected virtual void TranslateVScrollMessage(Message m) {
			if (m.WParam.ToInt32() == SB_ENDSCROLL)
				OnScrolled();
			else
				OnScrolling(ScrollReason.Mouse, ScrollDirection.Undefined);
		}
		[SecuritySafeCritical]
		protected virtual void TranslateHScrollMessage(Message m) {
			if (m.WParam.ToInt32() == SB_ENDSCROLL)
				OnScrolled();
			else
				OnScrolling(ScrollReason.Mouse, ScrollDirection.Undefined);
		}
		[SecuritySafeCritical]
		protected virtual void TranslateMouseWheelMessage(Message m) {
			int delta = (((int)m.WParam.ToInt64()) >> 16);
			OnScrolling(ScrollReason.Keyboard, delta > 0 ? ScrollDirection.Down : ScrollDirection.Up);
			OnScrolled();
		}
		protected virtual bool IsWantedChar(char c) {
			bool result = char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSeparator(c) || char.IsSymbol(c);
			if (!result)
				result = (int)c == (int)Keys.Enter || (int)c == (int)Keys.Back;
			return result;
		}
		[SecuritySafeCritical]
		protected virtual void TranslateCharMessage(Message m) {
			int pressedKey = m.WParam.ToInt32();
			char pressedChar = (char)pressedKey;
			if (IsWantedChar(pressedChar)) {
				if (pressedKey == (int)Keys.Space || pressedKey == (int)Keys.Enter || pressedKey == (int)Keys.Back)
					OnCheckNeeded();
				else
					OnCharPressed();
			}
			else
				OnPaint(PaintReason.Unknown); 
		}
		protected virtual void DisposeCore() {
			if (EditControl != null && !EditControl.IsDisposed) {
				UnsubscribeFromEditControlEvents();
			}
			editControlRef.Target = null;
		}
		[SecuritySafeCritical]
		public void Dispose() {
			ReleaseHandle();
			DisposeCore();
		}
		protected virtual void OnEditTextChanged(object sender, EventArgs e) {
			if (!this.lockTextChanged)
				OnCheckNeeded();
			this.lockTextChanged = false;
		}
		protected virtual void OnFontChanged(object sender, EventArgs e) {
			OnPaint(PaintReason.LayoutChanged);
		}
		#region CheckNeeded
		public event EventHandler CheckNeeded { add { checkNeeded += value; } remove { checkNeeded -= value; } }
		protected internal virtual void OnCheckNeeded() {
			RaiseOnCheckNeeded(EventArgs.Empty);
		}
		protected internal virtual void RaiseOnCheckNeeded(EventArgs e) {
			if (checkNeeded != null)
				checkNeeded(EditControl, e);
		}
		#endregion
		#region Scrolling
		public event ScrollingEventHandler Scrolling { add { scrolling += value; } remove { scrolling -= value; } }
		protected internal virtual void OnScrolling(ScrollReason reason, ScrollDirection direction) {
			RaiseOnScrolling(new ScrollingEventArgs(reason, direction));
		}
		protected internal virtual void RaiseOnScrolling(ScrollingEventArgs e) {
			if (scrolling != null)
				scrolling(EditControl, e);
		}
		#endregion
		#region Scrolled
		public event EventHandler Scrolled { add { scrolled += value; } remove { scrolled -= value; } }
		protected internal virtual void OnScrolled() {
			RaiseOnScrolled(EventArgs.Empty);
		}
		protected internal virtual void RaiseOnScrolled(EventArgs e) {
			if (scrolled != null)
				scrolled(EditControl, e);
		}
		#endregion
		#region TextChanged
		public event EventHandler TextChanged { add { textChanged += value; } remove { textChanged -= value; } }
		protected internal virtual void OnTextChanged() {
			RaiseOnTextChanged(EventArgs.Empty);
		}
		protected internal virtual void RaiseOnTextChanged(EventArgs e) {
			if (textChanged != null)
				textChanged(EditControl, e);
		}
		#endregion
		#region Paint
		public event PaintEventHandler Paint { add { paint += value; } remove { paint -= value; } }
		protected internal virtual void OnPaint(PaintReason paintReason) {
			RaiseOnPaint(new PaintEventArgs(paintReason));
		}
		protected internal virtual void RaiseOnPaint(PaintEventArgs e) {
			if (paint != null)
				paint(EditControl, e);
		}
		#endregion
		#region CharPressed
		public event EventHandler CharPressed { add { charPressed += value; } remove { charPressed -= value; } }
		protected internal virtual void OnCharPressed() {
			RaiseOnCharPressed(EventArgs.Empty);
		}
		protected internal virtual void RaiseOnCharPressed(EventArgs e) {
			if (charPressed != null)
				charPressed(EditControl, e);
		}
		#endregion
	}
	public class TextBoxHandler : IDisposable {
		IEditControlWrapper editControlWrapper = null;
		TextBoxNativeWindowWrapper nativeWindowWrapper = null;
		CheckAsYouTypeManager manager = null;
		NeedPaintEventHandler needPaint = null;
		EventHandler needClearErrors = null;
		EventHandler resetViewInfo = null;
		int topLineIndex = -1;
		Position leftIndex = Position.Null;
		bool isScrolling = false;
		Control editControl = null;
		public TextBoxHandler(CheckAsYouTypeManager manager, IEditControlWrapper editControlWrapper, Control editControl) {
			this.editControlWrapper = editControlWrapper;
			this.manager = manager;
			this.editControl = editControl;
			if (EditControlWrapper != null)
				topLineIndex = EditControlWrapper.FirstVisibleLine;
		}
		protected IEditControlWrapper EditControlWrapper { get { return this.editControlWrapper; } }
		protected TextBoxNativeWindowWrapper NativeWindowWrapper {
			get {
				if (nativeWindowWrapper == null)
					nativeWindowWrapper = CreateNativeWindowWrappper();
				return this.nativeWindowWrapper;
			}
		}
		protected CheckAsYouTypeManager Manager { get { return manager; } }
		protected Control EditControl { get { return editControl; } }
		public virtual void HandleEditControlNotifications() {
			SubscribeToNativeWindowEvents();
		}
		public virtual void StopHandleEditControlNotifications() {
			UnsubscribeFromNativeWindowEvents();
		}
		protected virtual void SubscribeToNativeWindowEvents() {
			NativeWindowWrapper.CharPressed += new EventHandler(OnCharPressed);
			NativeWindowWrapper.CheckNeeded += new EventHandler(OnCheckNeeded);
			NativeWindowWrapper.Scrolled += new EventHandler(OnScrolled);
			NativeWindowWrapper.Scrolling += new ScrollingEventHandler(OnScrolling);
			NativeWindowWrapper.TextChanged += new EventHandler(OnTextChanged);
			NativeWindowWrapper.Paint += new PaintEventHandler(OnPaint);
		}
		protected virtual void UnsubscribeFromNativeWindowEvents() {
			NativeWindowWrapper.CharPressed -= new EventHandler(OnCharPressed);
			NativeWindowWrapper.CheckNeeded -= new EventHandler(OnCheckNeeded);
			NativeWindowWrapper.Scrolled -= new EventHandler(OnScrolled);
			NativeWindowWrapper.Scrolling -= new ScrollingEventHandler(OnScrolling);
			NativeWindowWrapper.TextChanged -= new EventHandler(OnTextChanged);
			NativeWindowWrapper.Paint -= new PaintEventHandler(OnPaint);
		}
		protected virtual TextBoxBase GetTextBoxInstance() {
			return SpellCheckTextBoxBaseFinderManager.Default.GetTextBoxInstance(EditControl);
		}
		[SecuritySafeCritical]
		protected TextBoxNativeWindowWrapper CreateNativeWindowWrappper() {
			TextBoxBase textBox = GetTextBoxInstance();
			if (textBox == null)
				return null;
			if (textBox.IsHandleCreated) {
				TextBoxNativeWindowWrapper result = NativeWindow.FromHandle(textBox.Handle) as TextBoxNativeWindowWrapper;
				if (result != null)
					return result;
			}
			return CreateNativeWindowWrappper(textBox);
		}
		protected virtual TextBoxNativeWindowWrapper CreateNativeWindowWrappper(TextBoxBase textBox) {
			return new TextBoxNativeWindowWrapper(textBox);
		}
		protected virtual void OnTextChanged(object sender, EventArgs e) {
			InitScrollCoordinatesWithoutNotification();
			if (!Manager.LockTextChanged && Manager.IsTextChanged())
				Manager.Check();
		}
		protected virtual void OnPaint(object sender, PaintEventArgs e) {
			if (Manager.IsTextChanged() && e.PaintReason == PaintReason.CharPressed)
				OnNeedPaint(true, false);
			else
				if (e.PaintReason == PaintReason.LayoutChanged)
					OnNeedPaint(false, true);
				else
					if (!IsScrolling)
						OnNeedPaint(false, false);
		}
		#region VScrolling
		protected int TopLineIndex {
			get { return topLineIndex; }
			set {
				if (TopLineIndex != value) {
					topLineIndex = value;
					OnResetViewInfo();
				}
			}
		}
		protected virtual bool IsScrolling {
			get { return isScrolling; }
			set {
				if (isScrolling != value) {
					isScrolling = value;
					InitScrollCoordinates();
					OnIsScrollingChanged();
				}
			}
		}
		private void InitScrollCoordinates() {
			TopLineIndex = EditControlWrapper.FirstVisibleLine;
			LeftIndex = EditControlWrapper.FirstVisibleIndex;
		}
		private void InitScrollCoordinatesWithoutNotification() {
			topLineIndex = EditControlWrapper.FirstVisibleLine; ;
			leftIndex = EditControlWrapper.FirstVisibleIndex;
		}
		protected virtual void OnIsScrollingChanged() {
			if (IsScrolling) {
				OnNeedClearErrors();
			}
			else {
				if (!Manager.LockTextChanged)
					Manager.Check();
			}
		}
		protected virtual void OnScrolling(object sender, ScrollingEventArgs e) {
			if (String.IsNullOrEmpty(EditControl.Text))
				return;
			if (!IsScrolling) {
				if (e.ScrollReason == ScrollReason.Mouse)
					IsScrolling = true;
				else
					if (e.ScrollReason == ScrollReason.Wheel) {
						if (e.ScrollDirection == ScrollDirection.Down) {
							Point point = EditControlWrapper.GetPointByPosition(new IntPosition(EditControlWrapper.Text.Length - 1));
							if (point.Y > EditControlWrapper.GetHeight())
								IsScrolling = true;
						}
						else
							if (e.ScrollDirection == ScrollDirection.Up) {
								Point point = EditControlWrapper.GetPointByPosition(IntPosition.Null);
								if (point.Y < 0)
									IsScrolling = true;
							}
					}
					else
						if (EditControlWrapper.FirstVisibleLine != TopLineIndex || !Position.Equals(EditControlWrapper.FirstVisibleIndex, LeftIndex))
							IsScrolling = true;
			}
		}
		protected virtual void OnScrolled(object sender, EventArgs e) {
			IsScrolling = false;
			InitScrollCoordinates();
		}
		#endregion
		protected Position LeftIndex {
			get { return leftIndex; }
			set {
				if (!Position.Equals(leftIndex, value)) {
					leftIndex = value;
					OnResetViewInfo();
				}
			}
		}
		protected virtual Position GetCurrentLeftIndex() {
			return EditControlWrapper.FirstVisibleIndex;
		}
		protected virtual void OnCheckNeeded(object sender, EventArgs e) {
			if (Manager.IsTextChanged()) {
				if (!Manager.LockTextChanged)
					Manager.Check();
			}
			else
				if (!EditControlWrapper.CanScrollTextHorizontally)
					OnNeedPaint(false, false);
				else {
					Position currentLeftIndex = GetCurrentLeftIndex();
					if (!Position.Equals(currentLeftIndex, LeftIndex)) {
						LeftIndex = currentLeftIndex;
						OnNeedPaint(false, true);
					}
					else
						OnNeedPaint(false, false);
				}
		}
		protected virtual void OnCharPressed(object sender, EventArgs e) {
			int firstVisibleLine = EditControlWrapper.FirstVisibleLine;
			if (TopLineIndex != firstVisibleLine) {
				TopLineIndex = firstVisibleLine;
				OnNeedPaint(true, true);
			}
			else
				OnNeedPaint(true, false);
		}
		#region NeedPaint
		public event NeedPaintEventHandler NeedPaint { add { needPaint += value; } remove { needPaint -= value; } }
		protected internal virtual void OnNeedPaint(bool textChanged, bool layoutChanged) {
			NeedPaintEventArgs args = new NeedPaintEventArgs(textChanged, layoutChanged);
			RaiseOnNeedPaint(args);
		}
		protected internal virtual void RaiseOnNeedPaint(NeedPaintEventArgs args) {
			if (needPaint != null)
				needPaint(EditControlWrapper, args);
		}
		#endregion
		#region NeedClearErrors
		public event EventHandler NeedClearErrors { add { needClearErrors += value; } remove { needClearErrors -= value; } }
		protected internal virtual void OnNeedClearErrors() {
			RaiseOnNeedClearErrors(EventArgs.Empty);
		}
		protected internal virtual void RaiseOnNeedClearErrors(EventArgs args) {
			if (needClearErrors != null)
				needClearErrors(EditControlWrapper, args);
		}
		#endregion
		#region ResetViewInfo
		public event EventHandler ResetViewInfo { add { resetViewInfo += value; } remove { resetViewInfo -= value; } }
		protected internal virtual void OnResetViewInfo() {
			RaiseOnResetViewInfo(EventArgs.Empty);
		}
		protected internal virtual void RaiseOnResetViewInfo(EventArgs args) {
			if (resetViewInfo != null)
				resetViewInfo(EditControlWrapper, args);
		}
		#endregion
		public void Dispose() {
			UnsubscribeFromNativeWindowEvents();
			editControlWrapper = null;
			if (nativeWindowWrapper != null) {
				nativeWindowWrapper = null;
			}
			editControl = null;
		}
	}
	#region RichTextBox
	public class RichTextBoxNativeWindowWrapper : TextBoxNativeWindowWrapper {
		EventHandler fontChanged = null;
		public RichTextBoxNativeWindowWrapper(TextBoxBase textBox) : base(textBox) { }
		internal const int WM_COMMAND = 0x2111, EN_VSCROLL = 0x602;
		[SecuritySafeCritical]
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			switch (m.Msg) {
				case WM_KEYUP: 
					if (m.LParam == new IntPtr(-1071841279) && m.WParam == new IntPtr(VK_CONTROL)) {
						OnTextChanged();
					}
					break;
				case WM_COMMAND:
					int command = (((int)m.WParam.ToInt64()) >> 16);
					if (command == EN_VSCROLL) {
						OnScrolled();
					}
					break;
			}
		}
		protected virtual void OnFontChanged() {
			RaiseOnFontChanged(EventArgs.Empty);
		}
		private void RaiseOnFontChanged(EventArgs e) {
			if (fontChanged != null)
				fontChanged(EditControl, e);
		}
		public event EventHandler FontChanged { add { fontChanged += value; } remove { fontChanged -= value; } }
		const int MK_CONTROL = 0x0008;
		[SecuritySafeCritical]
		protected override void TranslateMouseWheelMessage(Message m) {
			int value = (int)m.WParam.ToInt64();
			int delta = (value >> 16);
			int keys = value & 0xFFFF;
			if ((keys & MK_CONTROL) != 0)
				OnFontChanged();
			else
				OnScrolling(ScrollReason.Wheel, delta < 0 ? ScrollDirection.Down : ScrollDirection.Up);
		}
		[SecuritySafeCritical]
		protected override void TranslateCharMessage(Message m) {
			base.TranslateCharMessage(m);
			int pressedKey = m.WParam.ToInt32();
			if (pressedKey == (int)Keys.Enter && EditControl != null)
				EditControl.Invalidate();
		}
	}
	public class RichTextBoxHandler : TextBoxHandler {
		bool isMouseWheelScrolling = false;
		bool isFontChanged = false;
		public RichTextBoxHandler(CheckAsYouTypeManager manager, IEditControlWrapper editControlWrapper, Control editControl) : base(manager, editControlWrapper, editControl) { }
		protected override TextBoxNativeWindowWrapper CreateNativeWindowWrappper(TextBoxBase textBox) {
			return new RichTextBoxNativeWindowWrapper(textBox);
		}
		protected new RichTextBoxNativeWindowWrapper NativeWindowWrapper { get { return base.NativeWindowWrapper as RichTextBoxNativeWindowWrapper; } }
		protected override void SubscribeToNativeWindowEvents() {
			base.SubscribeToNativeWindowEvents();
			NativeWindowWrapper.FontChanged += new EventHandler(OnFontChanged);
		}
		protected override void UnsubscribeFromNativeWindowEvents() {
			base.UnsubscribeFromNativeWindowEvents();
			NativeWindowWrapper.FontChanged -= new EventHandler(OnFontChanged);
		}
		protected virtual void OnFontChanged(object sender, EventArgs e) {
			IsFontChanged = true;
		}
		protected bool IsFontChanged {
			get { return isFontChanged; }
			set { isFontChanged = value; }
		}
		protected virtual bool IsMouseWheelScrolling {
			get { return isMouseWheelScrolling; }
			set {
				if (isMouseWheelScrolling != value) {
					isMouseWheelScrolling = value;
					OnIsMouseWheelScrollingChanged();
				}
			}
		}
		protected virtual void OnIsMouseWheelScrollingChanged() {
			if (!IsMouseWheelScrolling) {
				OnScrolled(EditControl, EventArgs.Empty);
			}
		}
		protected override void OnScrolling(object sender, ScrollingEventArgs e) {
			if (String.IsNullOrEmpty(EditControl.Text))
				return;
			if (e.ScrollReason == ScrollReason.Wheel)
				IsMouseWheelScrolling = true;
			base.OnScrolling(sender, e);
		}
		protected override void OnPaint(object sender, PaintEventArgs e) {
			if (!IsMouseWheelScrolling) {
				if (IsFontChanged) {
					IsFontChanged = false;
					OnNeedPaint(false, true);
				}
				if (e.PaintReason == PaintReason.LayoutChanged)
					OnNeedClearErrors();
				base.OnPaint(sender, e);
			}
		}
		protected override void OnCheckNeeded(object sender, EventArgs e) {
			if (IsMouseWheelScrolling) {
				IsMouseWheelScrolling = false;
			}
			base.OnCheckNeeded(sender, e);
		}
		protected override void OnIsScrollingChanged() {
			if (!IsScrolling)
				OnNeedClearErrors();
			base.OnIsScrollingChanged();
			if (!IsScrolling)
				this.EditControl.Refresh();
		}
	}
	public class RichTextBoxWrapper : TextBoxWrapper {
		Hashtable indents = new Hashtable();
		public RichTextBoxWrapper(TextBoxBase textBox, AsyncChecker checker) : base(textBox, checker) { }
		protected override Point GetPointByPositionCore(Position pos) {
			int intPos = GetIntPositionInstance(pos).ActualIntPosition;
			int lastletterWidth = 0;
			if (NeedUpdatePointByLastLetter(intPos)) {
				intPos--;
				if (Text.Length > 0)
					lastletterWidth = DevExpress.Utils.Text.TextUtils.GetStringSize(GetGraphics(), Text[Text.Length - 1].ToString(), TextBox.Font).Width;
				else
					return Point.Empty;
			}
			Point result = TextBox.GetPositionFromCharIndex(intPos);
			result.Y += GetIndent(pos) - 1; 
			result.X += lastletterWidth;
			return result;
		}
		protected virtual int GetIndent(Position pos) {
			int line = GetLineByPosition(pos);
			int firstIndex = TextBox.GetFirstCharIndexFromLine(line);
			int lastIndex = GetIntPositionInstance(pos).ActualIntPosition;
			if (firstIndex == lastIndex)
				lastIndex++;
			return RichTextboxLineHeightCalculator.MeasureRtfInPixels(TextBox, firstIndex, lastIndex, TextBox.ClientRectangle, 14, TextBox.ZoomFactor);
		}
		public new RichTextBox TextBox { get { return base.TextBox as RichTextBox; } }
	}
	public class RichTextboxLineHeightCalculator {
		struct STRUCT_CHARRANGE {
			internal int cpMin;
			internal int cpMax;
		}
		struct STRUCT_RECT {
			internal int left;
			internal int top;
			internal int right;
			internal int bottom;
		}
		struct STRUCT_FORMATRANGE {
			public IntPtr hdc;
			public IntPtr hdcTarget;
			public STRUCT_RECT rc;
			public STRUCT_RECT rcPage;
			public STRUCT_CHARRANGE chrg;
		}
		const int WM_USER = 0x0400, EM_FORMATRANGE = WM_USER + 57;
		public static int MeasureRtfInPixels(RichTextBox richTextBox, int charFrom, int charTo, RectangleF bounds, int minHeight, float zoomFactor) {
			if (richTextBox.TextLength == 0) {
				return minHeight;
			}
			else {
				bounds.X = 0;
				bounds.Y = 0;
				int height = 0;
				int fullHeight = 0;
				Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
				try {
					int lastHeight = 0;
					height = FormatRangeInternal(graphics, richTextBox, bounds, charFrom, charTo, out charTo);
					if (height == lastHeight) {
						charFrom = charTo;
						lastHeight = height;
						bounds.Height = height * 2;
					}
					fullHeight += height;
					return Convert.ToInt32(fullHeight * zoomFactor - GetCellDescent(richTextBox.Font) * zoomFactor);
				}
				finally {
					graphics.Dispose();
				}
			}
		}
		private static int GetCellDescent(Font font) {
			FontFamily family = font.FontFamily;
			FontStyle style = font.Style;
			float sizeInUnits = font.Size;
			float emSize = family.GetEmHeight(style);
			float ratio = sizeInUnits / emSize;
			return (int)Math.Ceiling(family.GetCellDescent(style) * ratio);
		}
		[System.Security.SecuritySafeCritical]
		private static int FormatRangeInternal(Graphics graphics, RichTextBox richTextBox, RectangleF bounds, int charFrom, int charTo, out int charEnd) {
			int result = 0;
			float DpiY = graphics.DpiY;
			PointF[] points = { bounds.Location, 
				new PointF(bounds.Location.X + bounds.Size.Width, bounds.Location.Y + bounds.Size.Height) };
			graphics.Transform.TransformPoints(points);
			points[0].X = Convert.ToInt32(points[0].X);
			points[0].Y = Convert.ToInt32(points[0].Y);
			points[1].X = Convert.ToInt32(points[1].X);
			points[1].Y = Convert.ToInt32(points[1].Y);
			Rectangle r = new Rectangle((int)points[0].X, (int)points[0].Y,
				(int)points[1].X - (int)points[0].X - 1, (int)points[1].Y - (int)points[0].Y - 1);
			STRUCT_CHARRANGE cr;
			cr.cpMin = charFrom;
			cr.cpMax = (charTo >= 0 && charTo > charFrom && charTo <= richTextBox.TextLength) ? charTo : richTextBox.TextLength;
			STRUCT_RECT rc;
			rc.top = (int)(r.Top / graphics.DpiY * 1440);
			rc.left = (int)(r.Left / graphics.DpiX * 1440);
			rc.right = (int)(r.Right / graphics.DpiX * 1440);
			rc.bottom = (int)(r.Bottom / graphics.DpiY * 1440);
			STRUCT_RECT rcPage;
			rcPage.top = (int)(r.Top / graphics.DpiY * 1440); ;
			rcPage.left = (int)(r.Left / graphics.DpiX * 1440);
			rcPage.right = (int)(r.Right / graphics.DpiX * 1440);
			rcPage.bottom = (int)(r.Bottom / graphics.DpiY * 1440);
			using (Graphics wndGraphics = Graphics.FromHwnd(IntPtr.Zero)) {
				IntPtr hdc = graphics.GetHdc();
				IntPtr wndDc = wndGraphics.GetHdc();
				try {
					STRUCT_FORMATRANGE fr;
					fr.chrg = cr;
					fr.hdc = hdc;
					fr.hdcTarget = wndDc;
					fr.rc = rc;
					fr.rcPage = rcPage;
					IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr));
					Marshal.StructureToPtr(fr, lParam, false);
					charEnd = SafeNativeMethods.SendMessage(richTextBox.Handle, EM_FORMATRANGE, IntPtr.Zero, lParam).ToInt32();
					fr = (STRUCT_FORMATRANGE)Marshal.PtrToStructure(lParam, typeof(STRUCT_FORMATRANGE));
					result = Convert.ToInt32((fr.rc.bottom - fr.rc.top) * DpiY / 1440);
					lParam = new IntPtr(0);
					SafeNativeMethods.SendMessage(richTextBox.Handle, EM_FORMATRANGE, IntPtr.Zero, lParam);
					Marshal.FreeCoTaskMem(lParam);
				}
				finally {
					graphics.ReleaseHdc(hdc);
					wndGraphics.ReleaseHdc(wndDc);
				}
			}
			return result;
		}
	}
	#endregion
	public static class SafeNativeMethods {
		static class Interop {
			[DllImport("user32.dll")]
			internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		}
		[System.Security.SecuritySafeCritical]
		public static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam) {
			return Interop.SendMessage(hWnd, msg, wParam, lParam);
		}
	}
}
