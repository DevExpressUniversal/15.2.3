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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.Mvvm;
using System.Windows.Input;
using System.Collections;
using DevExpress.Design.Mvvm;
using DevExpress.Entity.Model;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.DataModel;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData {
	public class DocumentInfo {
		public DocumentInfo(string viewName, string caption, IEntitySetInfo entityInfo, EntityRepositoryInfo repositoryInfo) {
			ViewName = viewName;
			Caption = caption;
			EntityInfo = entityInfo;
			RepositoryInfo = repositoryInfo;
		}
		public string Caption { get; private set; }
		public string ViewName { get; private set; }
		public IEntitySetInfo EntityInfo { get; private set; }
		public EntityRepositoryInfo RepositoryInfo { get; private set; }
	}
	public class DocumentManagerViewModelInfo : ViewModelInfoBase {		
		public DocumentManagerViewModelInfo(string assemblyName, bool isSolutionType, string name, string nameSpace, bool isLocalType, string moduleName, DocumentInfo[] tables, DocumentInfo[] views)
			: base(name, nameSpace, assemblyName, isSolutionType, isLocalType) {
			ModuleName = moduleName;
			Tables = tables;
			Views = views;
		}
		public string ModuleName { get; private set; }
		public DocumentInfo[] Tables { get; private set; }
		public DocumentInfo[] Views { get; private set; }
	}
}
