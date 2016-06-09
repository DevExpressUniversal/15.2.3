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

using System.Collections.Generic;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Compatibility.System.ComponentModel;
using System;
namespace DevExpress.DashboardCommon {
	public abstract class ParameterLookUpSettings {
		internal DashboardParameter Parameter { get; set; }
		public ParameterLookUpSettings Clone() {
			ParameterLookUpSettings lookupsettings = CreateInstance();
			lookupsettings.Assign(this);
			return lookupsettings;
		}
		protected virtual void Assign(ParameterLookUpSettings lookupsettings) {
		}
		protected virtual ParameterLookUpSettings CreateInstance() { 
			return null; 
		}
		protected internal abstract bool SettingsEquals(object settings);
		protected internal abstract XElement SaveToXml();
		protected internal abstract void LoadFromXml(XElement element);
		protected internal virtual void DataBind(IEnumerable<IDashboardDataSource> dataSources) {
		}
	}
}
