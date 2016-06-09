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
using System.Linq;
using System.Threading;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.WcfLinq.Helpers;
namespace DevExpress.Data.WcfLinq {
	[System.Drawing.ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "WcfServerModeSource.bmp")]
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)]
	public class WcfServerModeSource : Component, IListSource, ISupportInitialize, IWcfServerModeFrontEndOwner, IDXCloneable {
		WcfServerModeFrontEnd _List;
		IQueryable query;
		string key = string.Empty; 
		string _DefaultSorting = string.Empty;
		CriteriaOperator _FixedFilter;
		private WcfServerModeCore internalList;
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown;
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected;
		Type elementType;
		readonly ServerModeCoreExtender extender;
		public ServerModeCoreExtender Extender {
			get { return extender; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CriteriaOperator FixedFilterCriteria {
			get { return _FixedFilter; }
			set {
				if (ReferenceEquals(FixedFilterCriteria, value))
					return;
				_FixedFilter = value;
				ForceCatchUp();
			}
		}
		[Browsable(false)]
		[DefaultValue("")]
		public string FixedFilterString {
			get {
				return CriteriaOperator.ToString(FixedFilterCriteria);
			}
			set {
				FixedFilterCriteria = CriteriaOperator.Parse(value);
			}
		}
		#region IDXCloneable Members
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual object DXClone() {
			WcfServerModeSource clone = DXCloneCreate();
			clone._DefaultSorting = this._DefaultSorting;
			clone.elementType = this.elementType;
			clone.key = this.key;
			clone.InconsistencyDetected = this.InconsistencyDetected;
			clone.ExceptionThrown = this.ExceptionThrown;
			clone.Query = this.Query;
			return clone;
		}
		protected virtual WcfServerModeSource DXCloneCreate() {
			return new WcfServerModeSource();
		}
		#endregion
		#region IWcfServerModeFrontEndOwner Members
		public Type ElementType {
			get { return elementType; }
			set {
				if (Query != null) {
					value = Query.ElementType;
				}
				if (ElementType == value)
					return;
				elementType = value;
				FillKeyExpression();
				ForceCatchUp();
			}
		}
		public IQueryable Query {
			get {
				return query;
			}
			set {
				if (internalList != null)
					throw new InvalidOperationException();
				query = value;
				if(query != null) {
					elementType = query.ElementType;
					FillKeyExpression();
				}
				ForceCatchUp();
			}
		}
		bool? _isDesignMode;
		bool IWcfServerModeFrontEndOwner.IsReadyForTakeOff() {
			if (KeyExpression == null || KeyExpression.Length == 0)
				return false;
			if (Query == null)
				return false;
			if (!this.IsInitialized())
				return false;
			if(IsDesignModeHelper.GetIsDesignModeBypassable(this, ref _isDesignMode))
				return false;
			return true;
		}
		public string KeyExpression {
			get {
				return key;
			}
			set {
				if (internalList != null)
					throw new InvalidOperationException();
				key = value;
			}
		}
		[DefaultValue("")]
		public string DefaultSorting {
			get { return _DefaultSorting; }
			set {
				if (value == null)
					value = string.Empty;
				if (DefaultSorting == value)
					return;
				_DefaultSorting = value;
				ForceCatchUp();
			}
		}
		void FillKeyExpression() {
			if (ElementType == null)
				return;
			try {
				if (ElementType.GetProperty(KeyExpression) != null)
					return;
			} catch { }
			KeyExpression = WcfServerModeCore.GuessKeyExpression(ElementType);
		}
		int _initCount = 0;
		bool IsInitialized() {
			return _initCount == 0;
		}
		void ForceCatchUp() {
			if (!IsInitialized())
				return;
			List.CatchUp();
		}
		#endregion        
		WcfServerModeFrontEnd List {
			get {
				if (_List == null) {
					_List = CreateList();
					_List.InconsistencyDetected += new EventHandler<ServerModeInconsistencyDetectedEventArgs>(_List_InconsistencyDetected);
					_List.ExceptionThrown += new EventHandler<ServerModeExceptionThrownEventArgs>(_List_ExceptionThrown);
				}
				return _List;
			}
		}
		protected virtual void OnExceptionThrown(ServerModeExceptionThrownEventArgs e) {
			if (ExceptionThrown != null)
				ExceptionThrown(this, e);
		}
		protected virtual void OnInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs e) {
			if (InconsistencyDetected != null)
				InconsistencyDetected(this, e);
		}
		void _List_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			OnExceptionThrown(e);
		}
		void _List_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			OnInconsistencyDetected(e);
			if (e.Handled)
				return;
			SynchronizationContext context = SynchronizationContext.Current;
			if (IsGoodContext(context)) {
				PostState state = new PostState();
				state.ShouldFailWithException = true;
				context.Post(DoPostponedReload, state);
				state.ShouldFailWithException = false;
			} else {
				FailUnderAspOrAnotherNonPostEnvironment();
			}
		}
		class PostState {
			public bool ShouldFailWithException;
		}
		public void Reload() {
			List.Refresh();
		}
		void DoPostponedReload(object state) {
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
		protected WcfServerModeCore InternalList {
			get {
				if (internalList == null) {
					internalList = CreateInternalList();
				}
				return internalList;
			}
		}
		protected virtual WcfServerModeFrontEnd CreateList() {
			return new WcfServerModeFrontEnd(this, Extender);
		}
		protected virtual WcfServerModeCore CreateInternalList() {
			if (query == null || ReferenceEquals(key, null)) throw new InvalidOperationException();
			WcfServerModeCore core = new WcfServerModeCore(query, key, FixedFilterCriteria, Extender);
			return core;
		}
		public WcfServerModeSource()
			: this(null) {
		}
		public WcfServerModeSource(ServerModeCoreExtender extender) {
			if (extender == null) this.extender = new ServerModeCoreExtender();
			else this.extender = extender;
		}
		IList IListSource.GetList() {
			return List;
		}
		bool IListSource.ContainsListCollection {
			get { return false; }
		}
		void ISupportInitialize.BeginInit() {
			++_initCount;
			ForceCatchUp();
		}
		void ISupportInitialize.EndInit() {
			--_initCount;
			ForceCatchUp();
		}
	}
}
