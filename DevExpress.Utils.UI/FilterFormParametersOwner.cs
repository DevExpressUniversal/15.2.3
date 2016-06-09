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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using DevExpress.Data;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Utils.UI;
namespace DevExpress.Utils.UI {
	public class FilterFormParametersOwner : IFilterParametersOwner {
		readonly bool canAddParameters;
		readonly IList<IParameter> parameters;
		readonly IParameterCreator parameterCreator;
		public FilterFormParametersOwner(IParameterCreator parameterCreator, IList<IParameter> parameters)
			: this(parameterCreator, parameters, true) { 
		}
		public FilterFormParametersOwner(IParameterCreator parameterCreator, IList<IParameter> parameters, bool canAddParameters) {
			this.parameters = parameters;
			this.parameterCreator = parameterCreator;;
			this.canAddParameters = canAddParameters;
		}
		#region IFilterParametersOwner
		void IFilterParametersOwner.AddParameter(string name, Type type) {
			if(!parameters.Any<IParameter>(f => object.Equals(name, f.Name))) {
				var parameter = parameterCreator.CreateParameter(name, type);
				parameters.Add(parameter);
			}
		}
		bool IFilterParametersOwner.CanAddParameters {
			get { return canAddParameters; }
		}
		IList<IFilterParameter> IFilterParametersOwner.Parameters {
			get { return parameters.ToList<IFilterParameter>(); }
		}
		#endregion
	}
}
