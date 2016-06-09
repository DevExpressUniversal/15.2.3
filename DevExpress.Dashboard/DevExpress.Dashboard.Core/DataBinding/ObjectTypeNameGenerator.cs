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

using System;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public abstract class ObjectTypeNameGenerator : ObjectNameGenerator {
		public static bool ContainsWrongCharacters(string name) {
			if(!(Char.IsLetter(name[0]) || name[0] == '_'))
				return true;
			foreach(char ch in name) {
				if(!Char.IsLetterOrDigit(ch) && ch != '_')
					return true;
			}
			return false;
		}
		protected ObjectTypeNameGenerator()
			: base(string.Empty) {
		}
		protected override string GetPrefix(object obj) {
			string typeName = obj.GetType().Name;
			char head = Char.ToLower(typeName[0]);
			string tail = typeName.Length > 1 ? typeName.Substring(1) : string.Empty;
			return string.Format("{0}{1}", head, tail);
		}
		public override void CheckName(string name) {
			base.CheckName(name);
			if(ContainsWrongCharacters(name))
				throw new InvalidNameException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.MessageWrongCharacterItemName), name));
		}
		public override IErrorInfo ValidateName(string name) {
			IErrorInfo errorInfo = base.ValidateName(name);
			if (errorInfo != null)
				return errorInfo;
			return ContainsWrongCharacters(name) ? new ErrorInfo(ErrorType.WrongCharacterName) : null;
		}
	}
}
