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
using System.Collections.Generic;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Design.Mvvm.EntityFramework;
using DevExpress.Entity.Model;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	public class MvvmConstructorContext {
		public IDbContainerInfo DataSource { get; set; }
		public IEntitySetInfo[] SelectedTables { get; set; }
		public IDataModel SelectedDataModel { get; set; }
		public ViewModelType SelectedViewModelType { get; set; }
		public string SelectedViewModelName { get; set; }
		public IEntitySetInfo SelectedEntity { get; set; }
		public string SelectedViewName { get; set; }
		public ViewType SelectedViewType { get; set; }
		public UIType SelectedUIType { get; set; }
		public TaskType TaskType { get; set; }
		public PlatformType PlatformType { get; set; }
		public bool WithoutDesignTime { get; set; }
		public IViewModelInfo SelectedViewModel {
			get; set;
		}
		public IContainerInfo DbContextCandidate {
			get;
			set;
		}
	}
}
