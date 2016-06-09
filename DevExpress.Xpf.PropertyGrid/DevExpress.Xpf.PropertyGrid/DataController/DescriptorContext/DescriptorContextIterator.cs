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

using DevExpress.XtraVerticalGrid.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class DescriptorContextIterator {
		public bool IsMultiSource { get; set; }
		PGridDataModeHelperContextCache ContextCache { get; set; }
		internal DescriptorContextIterator(PGridDataModeHelperContextCache contextCache) {
			if (contextCache == null)
				throw new NullReferenceException("Can't create DataContextIterator instance.");
			ContextCache = contextCache;
		}
		public virtual void DoOperation(DescriptorContextOperation operation) {
			DoLocalOperation(operation, RowHandle.Root);
		}
		public virtual void DoLocalOperation(DescriptorContextOperation operation, IEnumerable<RowHandle> handles) {
			if (handles == null)
				return;
			if (operation == null)
				return;
			try {
				operation.Init();
				if (operation.NeedsFullIteration)
					VisitAllNodes(handles, operation);
				else
					VisitParentNodes(handles, operation);
			}
			finally {
				operation.Release();
			}
		}
		public virtual void DoLocalOperation(DescriptorContextOperation operation, RowHandle handle) {
			if (handle == null)
				return;
			if (operation == null)
				return;
			try {
				operation.Init();
				VisitAllNodes(handle, operation);
			}
			finally {
				operation.Release();
			}
		}
		public virtual void VisitCategories(DescriptorContextOperation operation, IList<RowHandle> handles) {
			foreach (RowHandle row in handles) {
				DescriptorContext context = GetContext(row);
				if (!operation.CanContinueIteration(context))
					return;
				operation.Execute(context);
			}
		}
		DescriptorContext GetContext(RowHandle rowhandle) {
			return ContextCache[IsMultiSource, rowhandle];
		}
		protected virtual void VisitAllNodes(IEnumerable<RowHandle> handles, DescriptorContextOperation operation) {
			foreach (RowHandle row in handles) {
				VisitAllNodes(row, operation);
			}
		}
		protected virtual void VisitAllNodes(RowHandle handle, DescriptorContextOperation operation) {
			DescriptorContext context = GetContext(handle);
			if (!operation.CanContinueIteration(context))
				return;
			operation.Execute(context);
			if (!operation.CanContinueIteration(context))
				return;
			if (HasChildren(context)) {
				if (NeedsVisitChildren(context, operation)) {
					VisitAllNodes(context.AllChildHandles, operation);
					if (handle == RowHandle.Root) {
						VisitCategories(operation, context.CategoryHandles);
					}
				}
			}
		}
		bool HasChildren(DescriptorContext context) {
			return context.Return(x => x.IsLoaded, () => false);
		}
		protected virtual void VisitParentNodes(IEnumerable<RowHandle> handles, DescriptorContextOperation operation) {
			foreach (RowHandle handle in handles) {
				DescriptorContext context = GetContext(handle);
				if (!operation.CanContinueIteration(context))
					return;
				if (HasChildren(context)) {
					operation.Execute(context);
					if (!operation.CanContinueIteration(context))
						return;
					if (NeedsVisitChildren(context, operation))
						VisitParentNodes(context.AllChildHandles, operation);
				}
			}
		}
		bool NeedsVisitChildren(DescriptorContext context, DescriptorContextOperation operation) {
			return operation.NeedsVisitChildren(context) && (context.IsLoaded || (!context.IsLoaded && operation.NeedsVisitUnloadedRows));
		}
	}
}
