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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Threading;
using DevExpress.Data.Browsing;
namespace DevExpress.Data {
	[
	ToolboxItem(false),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "EntityBindingSource.bmp"),
	]
	public class EntityBindingSource : Component, IListSource {
		IList list = null;
		object dataSource;
		public virtual object DataSource {
			get { return dataSource; }
			set {
				if(dataSource != value) {
					dataSource = value;
					list = value as IList;
				}
			}
		}
		protected virtual bool CacheList { get { return true; } }
		#region IListSource Members
		bool IListSource.ContainsListCollection {
			get { return false; }
		}
		IList IListSource.GetList() {
			if(list == null || !CacheList)
				list = Site != null ? GetDesigntimeList() : GetRuntimeList();
			return list;
		}
		protected virtual IList GetDesigntimeList() {
			if(dataSource is Type) {
				Type listType = typeof(List<>).MakeGenericType((Type)dataSource);
				return (IList)Activator.CreateInstance(listType);
			}
			return null;
		}
		protected virtual IList GetRuntimeList() {
			return null;
		}
		#endregion
	}
	public class AsyncWorker {
		Thread thread;
		WorkAsyncResult result;
		public Exception Exception { get; private set; }
		public IAsyncResult BeginWork(Action work, CancellationToken token) {
			result = new WorkAsyncResult();
			thread = new Thread(ExecCore) { Name = GetType().Name };
			thread.Start(work);
			token.Register(() => thread.Abort());
			return result;
		}
		void ExecCore(object work) {
			try {
				((Action)work)();
			} catch(ThreadAbortException) {
			} catch(Exception ex) {
				Exception = ex;
			} finally {
				result.IsCompleted = true;
			}
		}
	}
	class WorkAsyncResult : IAsyncResult {
		public object AsyncState {
			get;
			set;
		}
		WaitHandle IAsyncResult.AsyncWaitHandle {
			get { return null; }
		}
		bool IAsyncResult.CompletedSynchronously {
			get { return false; }
		}
		public virtual bool IsCompleted {
			get;
			set;
		}
	}
}
