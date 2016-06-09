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

using DevExpress.DataAccess.Native.Sql;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
namespace DevExpress.DataAccess.ConnectionParameters {
	public abstract class DataConnectionParametersBase : IConnectionPage {
		ProviderFactory factory;
		#region IConnectionPage Members
		bool IConnectionPage.AuthType { get; set; }
		bool IConnectionPage.CeConnectionHelper { get; set; }
		string IConnectionPage.CustomConStr {
			get { return string.Empty; }
		}
		string IConnectionPage.CustomConStrTag {
			get { return string.Empty; }
		}
		string IConnectionPage.DatabaseName { get; set; }
		ProviderFactory IConnectionPage.Factory {
			get { return factory; }
		}
		string IConnectionPage.FileName { get; set; }
		bool IConnectionPage.GenerateConnectionHelper {
			get { return false; }
		}
		bool IConnectionPage.IsServerbased {
			get { return IsServerBasedInternal; }
		}
		string IConnectionPage.LastConStr { get; set; }
		string IConnectionPage.Password { get; set; }
		string IConnectionPage.ServerName { get; set; }
		void IConnectionPage.SetProvider(string providerKey) {
			factory = DataConnectionHelper.GetProviderFactory(providerKey);
		}
		string IConnectionPage.UserName { get; set; }
		#endregion
		internal ProviderFactory Factory {
			get { return factory; }
			set { factory = value; }
		}
		protected virtual bool IsServerBasedInternal {
			get { return true; }
		}
		internal abstract void Assign(IConnectionPage source);		   
	}   
}
