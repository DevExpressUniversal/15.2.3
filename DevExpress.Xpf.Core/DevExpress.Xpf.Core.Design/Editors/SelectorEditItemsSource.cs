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

using System.Collections.Generic;
using DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Design {
	public class SelectorEditDataAccessTechnologyCollectionProvider : IDataAccessTechnologyCollectionProvider {
		private readonly IList<IDataAccessTechnologyInfo> collection;
		public SelectorEditDataAccessTechnologyCollectionProvider() {
			this.collection = new List<IDataAccessTechnologyInfo>() { 
				#if !SILVERLIGHT
				new EntityFrameworkInfo<SelectorEditSourceGenerator>() { IsSupportServerMode = false }, 
				new TypedDataSourceInfo<SelectorEditTypedSourceGenerator> { IsSupportServerMode = false },
				new LinqToSQLDataSourceInfo<SelectorEditSourceGenerator> { IsSupportServerMode = false },
#else
				new RIADataSourceInfo<SelectorEditSourceGenerator> { IsSupportServerMode = false },
#endif
				new WCFDataSourceInfo<SelectorEditSourceGenerator> { IsSupportServerMode = false }
			};
		}
		public IList<IDataAccessTechnologyInfo> DataAccessTechnologyCollection { get { return this.collection; } }
	}
}
