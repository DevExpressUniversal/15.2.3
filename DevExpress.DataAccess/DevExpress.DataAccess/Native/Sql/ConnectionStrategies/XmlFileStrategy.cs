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
using DevExpress.DataAccess.Localization;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	public class XmlFileStrategy : FileDbStrategyBase {
		InMemoryProviderFactory factory;
		#region Overrides of DbStrategyBase
		public override string FileNameFilter {
			get { return FileNameFilterStrings.Xml; }
		}
		#endregion
		#region Overrides of FileDbStrategyBase
		public override ConnectionParameterEdits GetEditsSet(IConnectionParametersControl control) { return ConnectionParameterEdits.FileName; }
		protected override ProviderFactory Factory { get { return this.factory ?? (this.factory = new InMemoryProviderFactory()); } }
		public override DataConnectionParametersBase GetConnectionParameters(IConnectionParametersControl control) { return new XmlFileConnectionParameters(control.FileName); }
		public override void InitializeControl(IConnectionParametersControl control, DataConnectionParametersBase value) {
			XmlFileConnectionParameters p = (XmlFileConnectionParameters)value;
			control.FileName = p.FileName;
		}
		#endregion
	}
}
