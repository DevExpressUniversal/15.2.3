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
using System.Web.Mvc;
using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxScheduler;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.ASPxScheduler.Internal;
	using DevExpress.Web.ASPxScheduler.Controls;
	[ToolboxItem(false)]
	public class MVCxTimeZoneEdit: ASPxTimeZoneEdit {
		IMasterControl masterControl;
		protected internal MVCxTimeZoneEdit(IMasterControl masterControl)
			: base() {
			this.masterControl = masterControl;
		}
		protected internal new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		protected internal void PerformOnLoad() {
			OnLoad(EventArgs.Empty);
		}
		protected override RelatedControlDefaultImplementation CreateDefaultRelatedControl() {
			return new MVCxRelatedControlDefaultImplementation(this, (IRelatedControl)this, this.masterControl);
		}
		protected override string GetClientObjectClassName() {
			return "MVCx.ClientTimeZoneEdit";
		}
	}
	[ToolboxItem(false)]
	public class MVCxResourceNavigator: ASPxResourceNavigator {
		protected internal MVCxResourceNavigator(MVCxScheduler ownerControl)
			: base() {
			OwnerControl = ownerControl;
		}
		protected override RelatedControlDefaultImplementation CreateDefaultRelatedControl() {
			return new MVCxRelatedControlDefaultImplementation(this, (IRelatedControl)this, (IMasterControl)OwnerControl);
		}
	}
	[ToolboxItem(false)]
	public class MVCxViewSelector: ASPxViewSelector {
		protected internal MVCxViewSelector(MVCxScheduler ownerControl)
			: base() {
			OwnerControl = ownerControl;
		}
		protected override RelatedControlDefaultImplementation CreateDefaultRelatedControl() {
			return new MVCxRelatedControlDefaultImplementation(this, (IRelatedControl)this, (IMasterControl)OwnerControl);
		}
	}
	[ToolboxItem(false)]
	public class MVCxViewNavigator: ASPxViewNavigator {
		protected internal MVCxViewNavigator(MVCxScheduler ownerControl)
			: base() {
			OwnerControl = ownerControl;
		}
		protected override RelatedControlDefaultImplementation CreateDefaultRelatedControl() {
			return new MVCxRelatedControlDefaultImplementation(this, (IRelatedControl)this, (IMasterControl)OwnerControl);
		}
	}
	[ToolboxItem(false)]
	public class MVCxViewVisibleInterval: ASPxViewVisibleInterval {
		protected internal MVCxViewVisibleInterval(MVCxScheduler ownerControl)
			: base() {
			OwnerControl = ownerControl;
		}
		protected override RelatedControlDefaultImplementation CreateDefaultRelatedControl() {
			return new MVCxRelatedControlDefaultImplementation(this, (IRelatedControl)this, (IMasterControl)OwnerControl);
		}
	}
	[ToolboxItem(false)]
	public class MVCxSchedulerStatusInfoManager: ASPxSchedulerStatusInfoManager {
		protected internal MVCxSchedulerStatusInfoManager(MVCxScheduler ownerControl)
			: base() {
			OwnerControl = ownerControl;
		}
		protected override RelatedControlDefaultImplementation CreateDefaultRelatedControl() {
			return new MVCxRelatedControlDefaultImplementation(this, (IRelatedControl)this, (IMasterControl)OwnerControl);
		}
	}
	[ToolboxItem(false)]
	public class MVCxDateNavigator: ASPxDateNavigator {
		protected internal MVCxDateNavigator(MVCxScheduler ownerControl)
			: base() {
			OwnerControl = ownerControl;
		}
		protected internal void PerformOnLoad() {
			OnLoad(EventArgs.Empty);
		}
		protected override RelatedControlDefaultImplementation CreateDefaultRelatedControl() {
			return new MVCxRelatedControlDefaultImplementation(this, (IRelatedControl)this, (IMasterControl)OwnerControl);
		}
	}
}
