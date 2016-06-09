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
using System.Linq;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public interface IDataLockingManager {
		Boolean IsActive { get; }
		DataLockingInfo GetDataLockingInfo();
		void MergeData(DataLockingInfo dataLockingInfo);
		void RefreshData(DataLockingInfo dataLockingInfo);
	}
	public sealed class DataLockingInfo {
		public static readonly DataLockingInfo Empty = new DataLockingInfo(new ObjectLockingInfo[0]);
		public DataLockingInfo(ObjectLockingInfo[] objectLockingInfo) {
			Guard.ArgumentNotNull(objectLockingInfo, "objectLockingInfo");
			ObjectLockingInfo = objectLockingInfo;
			IsLocked = objectLockingInfo.Length > 0;
			CanMerge = objectLockingInfo.FirstOrDefault(info => !info.CanMerge) == null;
		}
		public ObjectLockingInfo[] ObjectLockingInfo { get; private set; }
		public Boolean IsLocked { get; private set; }
		public Boolean CanMerge { get; private set; }
	}
	public sealed class ObjectLockingInfo {
		public ObjectLockingInfo(Object lockedObject, Boolean canMerge) {
			Guard.ArgumentNotNull(lockedObject, "lockedObject");
			LockedObject = lockedObject;
			CanMerge = canMerge;
		}
		public Object LockedObject { get; private set; }
		public Boolean CanMerge { get; private set; }
	}
}
