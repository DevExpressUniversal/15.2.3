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
using System.Drawing;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Native {
	#region ResourcePredicate
	public abstract class ResourcePredicate : SchedulerPredicate<Resource> {
	}
	#endregion
	#region EmptyResourceFilterPredicate
	public class EmptyResourceFilterPredicate : SchedulerEmptyPredicate<Resource> {
	}
	#endregion
	#region CompositeResourcePredicate
	public class CompositeResourcePredicate : SchedulerCompositePredicate<Resource> {
	}
	#endregion
	#region FilterResourceViaStorageEventPredicate
	public class FilterResourceViaStorageEventPredicate : FilterObjectViaStorageEventPredicate<Resource> {
		public FilterResourceViaStorageEventPredicate(ISchedulerStorageBase storage)
			: base(storage) {
		}
		public override bool Calculate(Resource obj) {
			return ((IInternalSchedulerStorageBase)Storage).RaiseFilterResource(obj);
		}
	}
	#endregion
	#region FilterResourcePredicate
	public class FilterResourcePredicate : FilterObjectViaFilterCriteriaPredicate<Resource> {		
		public FilterResourcePredicate(IResourceStorageBase resourceStorage, string filterString, bool caseSensitive)
			: base(resourceStorage, filterString, caseSensitive) {
		}
		public FilterResourcePredicate(IResourceStorageBase resourceStorage, string filterString) 
			: base(resourceStorage, filterString, true) {		
		}		
	}
	#endregion
	#region ResourcesProcessorBase
	public abstract class ResourcesProcessorBase : ProcessorBase<Resource> {
		protected internal override NotificationCollection<Resource> CreateDestinationCollection() {
			return new ResourceBaseCollection();
		}
	}
	#endregion
	#region ResourceFilter
	public class ResourceFilter : SimpleProcessorBase<Resource> {
		public ResourceFilter(IPredicate<Resource> predicate)
			: base(predicate) {
		}
		protected internal override NotificationCollection<Resource> CreateDestinationCollection() {
			return new ResourceBaseCollection();
		}
	}
	#endregion
}
