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
using System.Reflection;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.ObjectBinding;
namespace DevExpress.DataAccess.Wizard.Views {
	public interface IChooseObjectMemberPageView {
		ObjectMember Result { get; set; }
		bool ShowAll { get; }
		void Initialize(IEnumerable<ObjectMember> items, bool staticType, bool showAll);
		event EventHandler Changed;
	}
	public sealed class ParametersViewInfo {
		public ParametersViewInfo(ParameterInfo[] data) { Data = data; }
		public ParameterInfo[] Data { get; private set; }
		public static bool Equals(ParametersViewInfo x, ParametersViewInfo y) {
			if(object.ReferenceEquals(x, y))
				return true;
			if(x == null || y == null)
				return false;
			if(object.ReferenceEquals(x.Data, y.Data))
				return true;
			if(x.Data == null || y.Data == null)
				return false;
			int n = x.Data.Length;
			if(y.Data.Length != n)
				return false;
			for(int i = 0; i < n; i++)
				if(x.Data[i] != y.Data[i])
					return false;
			return true;
		}
		#region Overrides of Object
		public override string ToString() {
			if(Data == null)
				return String.Empty;
			if(Data.Length == 0)
				return DataAccessLocalizer.GetString(DataAccessStringId.ParameterlessConstructor);
			return String.Format("({0})",
				String.Join(", ", Data.Select(pi => String.Format("{0} {1}", TypeNamesHelper.ShortName(pi.ParameterType), pi.Name))));
		}
		#endregion
	}
}
