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

using DevExpress.Data;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.Utils.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
namespace DevExpress.DashboardWin.Native {
	public class ParameterDescriptionProvider : TypeDescriptionProvider {
		public static string GenerateName(IEnumerable items) {
			PrefixNameGenerator nameGenerator = new PrefixNameGenerator(DataAccessUILocalizer.GetString(DataAccessUIStringId.DefaultNameParameter), "", 1);
			return nameGenerator.GenerateNameFromStart(name => items.Cast<IParameter>().Any(p => p.Name == name));
		}
		readonly IParameterCreator parameterCreator;
		public ParameterDescriptionProvider(IParameterCreator parameterCreator) {
			this.parameterCreator = parameterCreator;
		}
		public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args) {
			IItemsContainer itemsContainer = (IItemsContainer)provider.GetService(typeof(IItemsContainer));
			string pName = GenerateName(itemsContainer.Items);
			return parameterCreator.CreateParameter(pName, typeof(string));
		}
	}
}
