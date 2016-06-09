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
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.ReportsV2;
namespace DevExpress.ExpressApp.ReportsV2 {
	internal class ReportObjectSpaceProvider : IReportObjectSpaceProvider {
		private List<IObjectSpace> createdObjectSpaces = new List<IObjectSpace>();
		private XafApplication application;
		private bool isDisposed = false;
		public ReportObjectSpaceProvider(XafApplication application) {
			this.application = application;
		}
		public bool IsDisposed {
			get { return isDisposed; }
		}
		protected IObjectSpace GetObjectSpace(Type objectType) {
			IObjectSpace result = null;
			if(objectType == null) {
				Guard.ArgumentNotNull(objectType, "objectType");
			}
			foreach(IObjectSpace objectSpace in createdObjectSpaces) {
				if(objectSpace.IsKnownType(objectType)) {
					result = objectSpace;
					break;
				}
			}
			if(result == null) {
				result = application.CreateObjectSpace(objectType);
				createdObjectSpaces.Add(result);
			}
			return result;
		}
		#region IReportObjectSpaceProvider Members
		IObjectSpace IReportObjectSpaceProvider.GetObjectSpace(Type type) {
			if(IsDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
			Guard.ArgumentNotNull(application, "Application");
			return GetObjectSpace(type);
		}
		void IReportObjectSpaceProvider.DisposeObjectSpaces() {
			isDisposed = true;
			if(createdObjectSpaces != null) {
				foreach(IObjectSpace os in createdObjectSpaces) {
					os.Dispose();
				}
				createdObjectSpaces.Clear();
			}
			createdObjectSpaces = null;
			application = null;
		}
		#endregion
	}
}
