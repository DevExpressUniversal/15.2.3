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

using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	public class AmazonRedshiftStrategy : ServerDbStrategyBase {
		DataAccessAmazonRedshiftProviderFactory factory;
		#region Overrides of ServerDbStrategyBase
		public override ConnectionParameterEdits GetEditsSet(IConnectionParametersControl control) { return base.GetEditsSet(control) | ConnectionParameterEdits.Port; }
		protected override ProviderFactory Factory {
			get { return this.factory ?? (this.factory = new DataAccessAmazonRedshiftProviderFactory()); }
		}
		public override string GetDefaultText(ConnectionParameterEdits edit) {
			switch(edit) {
				case ConnectionParameterEdits.ServerName:
					return "***.redshift.amazonaws.com";
				case ConnectionParameterEdits.Port:
					return "5439";
				default:
					return base.GetDefaultText(edit);
			}
		}
		#endregion
		#region Implementation of IConnectionParametersStrategy
		public override DataConnectionParametersBase GetConnectionParameters(IConnectionParametersControl control) {
			return new AmazonRedshiftConnectionParameters(control.ServerName, control.Port, control.Database,
				control.UserName, control.Password);
		}
		public override void InitializeControl(IConnectionParametersControl control, DataConnectionParametersBase value) {
			AmazonRedshiftConnectionParameters p = (AmazonRedshiftConnectionParameters)value;
			control.ServerName = p.ServerName;
			control.Port = p.Port;
			control.Database = p.DatabaseName;
			control.UserName = p.UserName;
			control.Password = p.Password;
		}
		#endregion
	}
}
