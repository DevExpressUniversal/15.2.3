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
using System.Linq;
namespace DevExpress.Data.Linq {
	using DevExpress.Data.Linq.Helpers;
	using System.Drawing;
	using System.Threading;
	using System.ComponentModel;
	using DevExpress.Data.Helpers;
	using System.Collections;
	using Compatibility.System.ComponentModel;
	[DXToolboxItem(true)]
	[ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "EntityServerModeSource.bmp")]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)]
	public class EntityServerModeSource : Component, IListSource, ISupportInitialize, ILinqServerModeFrontEndOwner, IDXCloneable {
		bool IListSource.ContainsListCollection {
			get { return false; }
		}
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual object DXClone() {
			EntityServerModeSource clone = DXCloneCreate();
			clone._DefaultSorting = this._DefaultSorting;
			clone._ElementType = this._ElementType;
			clone._KeyExpression = this._KeyExpression;
			clone.InconsistencyDetected = this.InconsistencyDetected;
			clone.ExceptionThrown = this.ExceptionThrown;
			clone._QueryableSource = this._QueryableSource;
			return clone;
		}
		protected virtual EntityServerModeSource DXCloneCreate() {
			return new EntityServerModeSource();
		}
		IList IListSource.GetList() {
			return List;
		}
		EntityServerModeFrontEnd _List;
		EntityServerModeFrontEnd List {
			get {
				if (_List == null) {
					_List = CreateList();
					_List.InconsistencyDetected += new EventHandler<ServerModeInconsistencyDetectedEventArgs>(_List_InconsistencyDetected);
					_List.ExceptionThrown += new EventHandler<ServerModeExceptionThrownEventArgs>(_List_ExceptionThrown);
				}
				return _List;
			}
		}
		public void Reload() {
			List.Refresh();
		}
		protected virtual EntityServerModeFrontEnd CreateList() {
			return new EntityServerModeFrontEnd(this);
		}
		void ForceCatchUp() {
			if (!IsInitialized())
				return;
			List.CatchUp();
		}
		Type _ElementType;
		[
#if !SL
	DevExpressDataLocalizedDescription("EntityServerModeSourceElementType"),
#endif
		RefreshProperties(RefreshProperties.All), DefaultValue(null)]
#if !SL //TODO SL
		[TypeConverter(typeof(LinqServerModeSourceObjectTypeConverter))]
#endif
		public Type ElementType {
			get { return _ElementType; }
			set {
				if (QueryableSource != null) {
					value = QueryableSource.ElementType;
				}
				if (ElementType == value)
					return;
				_ElementType = value;
				FillKeyExpression();
				ForceCatchUp();
			}
		}
		void FillKeyExpression() {
			if (ElementType == null)
				return;
			if (KeyExpression != null && KeyExpression.IndexOfAny(new char[] { ',', ';' }) >= 0)
				return;
			try {
				if (ElementType.GetProperty(KeyExpression) != null)
					return;
			} catch { }
			string guessed = LinqServerModeCore.GuessKeyExpression(ElementType);
			if (!string.IsNullOrEmpty(guessed))
				KeyExpression = guessed;
		}
		string _KeyExpression = string.Empty;
		[
#if !SL
	DevExpressDataLocalizedDescription("EntityServerModeSourceKeyExpression"),
#endif
		DefaultValue("")]
		public string KeyExpression {
			get { return _KeyExpression; }
			set {
				if (KeyExpression == value)
					return;
				_KeyExpression = value;
				ForceCatchUp();
			}
		}
		string _DefaultSorting = string.Empty;
		[DefaultValue("")]
		public string DefaultSorting {
			get { return _DefaultSorting; }
			set {
				if (value == null) value = string.Empty;
				if (DefaultSorting == value)
					return;
				_DefaultSorting = value;
				ForceCatchUp();
			}
		}
		IQueryable _QueryableSource;
		[
#if !SL
	DevExpressDataLocalizedDescription("EntityServerModeSourceQueryableSource"),
#endif
		DefaultValue(null), RefreshProperties(RefreshProperties.All)]
		public IQueryable QueryableSource {
			get { return _QueryableSource; }
			set {
				if (QueryableSource == value)
					return;
				_QueryableSource = value;
				if (QueryableSource != null)
					ElementType = QueryableSource.ElementType;
				ForceCatchUp();
			}
		}
		int _initCount = 0;
		bool IsInitialized() {
			return _initCount == 0;
		}
		void ISupportInitialize.BeginInit() {
			++_initCount;
			ForceCatchUp();
		}
		void ISupportInitialize.EndInit() {
			--_initCount;
			ForceCatchUp();
		}
		class PostState {
			public bool ShouldFailWithException;
		}
		void _List_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			OnExceptionThrown(new LinqServerModeExceptionThrownEventArgs(e.Exception));
		}
		void _List_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			LinqServerModeInconsistencyDetectedEventArgs ee = new LinqServerModeInconsistencyDetectedEventArgs();
			ee.Handled = e.Handled;
			OnInconsistencyDetected(ee);
			e.Handled = ee.Handled;
			if (ee.Handled)
				return;
			SynchronizationContext context = SynchronizationContext.Current;
			if (IsGoodContext(context)) {
				PostState state = new PostState();
				state.ShouldFailWithException = true;
				context.Post(DoPostpronedReload, state);
				state.ShouldFailWithException = false;
			} else {
				FailUnderAspOrAnotherNonPostEnvironment();
			}
		}
		void DoPostpronedReload(object state) {
			PostState postedState = (PostState)state;
			if (postedState.ShouldFailWithException)
				FailUnderAspOrAnotherNonPostEnvironment();
			else
				Reload();
		}
		void FailUnderAspOrAnotherNonPostEnvironment() {
		}
		protected virtual bool IsGoodContext(SynchronizationContext context) {
			if (context == null)
				return false;
			return true;
		}
		public event LinqServerModeExceptionThrownEventHandler ExceptionThrown;
		public event LinqServerModeInconsistencyDetectedEventHandler InconsistencyDetected;
		protected virtual void OnExceptionThrown(LinqServerModeExceptionThrownEventArgs e) {
			if (ExceptionThrown != null)
				ExceptionThrown(this, e);
		}
		protected virtual void OnInconsistencyDetected(LinqServerModeInconsistencyDetectedEventArgs e) {
			if (InconsistencyDetected != null)
				InconsistencyDetected(this, e);
		}
		Type ILinqServerModeFrontEndOwner.ElementType {
			get { return this.ElementType; }
		}
		IQueryable ILinqServerModeFrontEndOwner.QueryableSource {
			get { return this.QueryableSource; }
		}
		bool? _IsDesignMode;
		bool ILinqServerModeFrontEndOwner.IsReadyForTakeOff() {
			if (KeyExpression == null || KeyExpression.Length == 0)
				return false;
			if (QueryableSource == null)
				return false;
			if (!this.IsInitialized())
				return false;
			if (IsDesignModeHelper.GetIsDesignModeBypassable(this, ref _IsDesignMode))
				return false;
			return true;
		}
		string ILinqServerModeFrontEndOwner.KeyExpression {
			get { return this.KeyExpression; }
		}
	}
}
