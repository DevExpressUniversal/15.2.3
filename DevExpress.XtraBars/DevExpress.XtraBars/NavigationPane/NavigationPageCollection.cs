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
using System.ComponentModel;
namespace DevExpress.XtraBars.Navigation {
	[Editor("DevExpress.XtraBars.Design.NavigationPageBaseCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	[RefreshProperties(RefreshProperties.All), ListBindable(false)]
	public class NavigationPageBaseCollection : Docking2010.Base.BaseMutableListEx<NavigationPageBase> {
		INavigationFrame ownerCore;
		public NavigationPageBaseCollection(INavigationFrame owner) {
			this.ownerCore = owner;
		}
		protected INavigationFrame Owner {
			get { return ownerCore; }
		}
		public bool Insert(int index, NavigationPageBase page) {
			return InsertCore(index, page);
		}
		Docking2010.Base.IBatchUpdate batchUpdateRangeOperation;
		protected override void OnBeforeElementRangeAdded() {
			batchUpdateRangeOperation = Docking2010.BatchUpdate.Enter(Owner);
			base.OnBeforeElementRangeAdded();
		}
		protected override void OnElementRangeAdded() {
			base.OnElementRangeAdded();
			Docking2010.Ref.Dispose(ref batchUpdateRangeOperation);
		}
		protected override void OnBeforeElementRangeRemoved() {
			batchUpdateRangeOperation = Docking2010.BatchUpdate.Enter(Owner);
			base.OnBeforeElementRangeRemoved();
		}
		protected override void OnElementRangeRemoved() {
			base.OnElementRangeRemoved();
			Docking2010.Ref.Dispose(ref batchUpdateRangeOperation);
		}
		protected override void OnElementDisposed(object sender, EventArgs ea) {
			using(Docking2010.BatchUpdate.Enter(Owner))
				base.OnElementDisposed(sender, ea);
		}
		public override string ToString() {
			if(Count == 0) return "None";
			return string.Format("Count {0}", Count);
		}
	}
}
