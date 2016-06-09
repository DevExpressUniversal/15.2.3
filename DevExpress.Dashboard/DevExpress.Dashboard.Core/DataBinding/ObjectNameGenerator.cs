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
using System.Linq;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public interface IObjectNameGenerator {
		string GenerateName(object obj);
		void CheckName(string name);
		IErrorInfo ValidateName(string name);
	}
	public abstract class ObjectNameGenerator : NameGenerator, IObjectNameGenerator {
		protected abstract IEnumerable<string> Names { get; }
		protected ObjectNameGenerator(string separator)
			: base(separator) {
		}
		public virtual void CheckName(string name) {
			if(string.IsNullOrEmpty(name))
				throw new InvalidNameException(DataAccessLocalizer.GetString(DataAccessStringId.MessageInvalidItemName));
			if(Contains(name))
				throw new InvalidNameException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.MessageDuplicateItemName), name));
		}
		public virtual IErrorInfo ValidateName(string name) {
			if (string.IsNullOrEmpty(name))
				return new ErrorInfo(ErrorType.EmptyName);
			if (Contains(name))
				return new ErrorInfo(ErrorType.DuplicateName);
			return null;
		}
		public string GenerateName(object obj) {
			int index = 1;
			return GetNextName(GetPrefix(obj), ref index, Contains);
		}
		public bool IsDefaultName(object obj, string name) {
			return IsMatchToDefaultNamePattern(name, GetPrefix(obj));
		}
		protected abstract string GetPrefix(object obj);
		bool Contains(string name) {
			return Names != null && Names.Contains<string>(name);
		}
	}
}
