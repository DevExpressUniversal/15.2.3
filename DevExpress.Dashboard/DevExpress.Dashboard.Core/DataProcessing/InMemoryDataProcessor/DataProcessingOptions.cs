#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public static class DataProcessingOptions {
		static int dataVectorSize = 1024;
		static int defaultVectorInBuffer = dataVectorSize;
		static int defaultGroupIndexesSize = dataVectorSize * 10;
		static int aggregateWorkerCacheSize = 1000;
		static int projectWorkerCacheSize = 5 * 1000;
		static int defaultGroupStorageCapacity = 1000;
		static int defaultRowProcessingLimit = dataVectorSize;
		static bool useParallelProcessing = true;
		static bool stopProcessingOnError = false;
		public static bool UseParallelProcessing { get { return useParallelProcessing; } set { useParallelProcessing = value; } }
		public static int DataVectorSize { get { return dataVectorSize; } set { dataVectorSize = value; } }
		public static int DefaultBufferSize { get { return defaultVectorInBuffer; } set { defaultVectorInBuffer = value; } }
		public static int DefaultGroupIndexesSize { get { return defaultGroupIndexesSize; } set { defaultGroupIndexesSize = value; } }
		public static int DefaultGroupStorageCapacity { get { return defaultGroupStorageCapacity; } set { defaultGroupStorageCapacity = value; } }
		public static int DefaultRowProcessingLimit { get { return defaultRowProcessingLimit; } set { defaultRowProcessingLimit = value; } }
		public static bool StopProcessingOnError { get { return stopProcessingOnError; } set { stopProcessingOnError = value; } }
		public static int AggregateWorkerCacheSize { get { return aggregateWorkerCacheSize; } set { aggregateWorkerCacheSize = value; } }
		public static int ProjectWorkerCacheSize { get { return projectWorkerCacheSize; } set { projectWorkerCacheSize = value; } }
	}
}
