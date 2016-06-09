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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public class ObjectMemberParametersPageView : ParametersPageViewBase, IObjectMemberParametersPageView {
		public ObjectMemberParametersPageView(IServiceProvider propertyGridServices, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider) 
			: base(propertyGridServices, parameterService, repositoryItemsProvider) {
		}
		ObjectMemberParametersPageView() : this(null, null, DefaultRepositoryItemsProvider.Instance) { }
		#region Overrides of ConfigureParametersPageView
		public override string HeaderDescription { get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageObjectMemberParameters); } }
		#endregion
		#region Implementation of IObjectMemberParametersPageView
		void IObjectMemberParametersPageView.Initialize( IEnumerable<IParameter> parameters) { Initialize(parameters); }
		#endregion
	}
}
