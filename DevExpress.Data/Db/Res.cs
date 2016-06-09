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

namespace DevExpress.Xpo.DB {
	using System;
	using System.Reflection;
	using System.Resources;
	using System.Globalization;
	sealed class DbRes {
		ResourceManager manager;
		static DbRes res;
		DbRes() {
#if DXPORTABLE
			manager = new ResourceManager("Db.Messages", GetType().GetTypeInfo().Assembly);
#else
			manager = new ResourceManager("DevExpress.Data.Db.Messages", GetType().Assembly);
#endif
		}
		static DbRes GetLoader() {
			if(res == null) {
				lock(typeof(DbRes)) {
					if(res == null) {
						res = new DbRes();
					}
				}
			}
			return res;
		}
		public static string GetString(CultureInfo culture, string name) {
			DbRes r = DbRes.GetLoader();
			if(r == null)
				return null;
			return r.manager.GetString(name, culture);
		}
		public static string GetString(CultureInfo culture, string name, params object[] args) {
			DbRes r = DbRes.GetLoader();
			if(r == null)
				return null;
			string str = r.manager.GetString(name, culture);
			if(args != null && args.Length > 0)
				return String.Format(str, args);
			return str;
		}
		public static string GetString(string name) {
			return GetString(null, name);
		}
		public static string GetString(string name, params object[] args) {
			return GetString(null, name, args);
		}
		public const string ConnectionProvider_TypeMappingMissing = "ConnectionProvider_TypeMappingMissing";
		public const string ConnectionProvider_UnableToCreateDBObject = "ConnectionProvider_UnableToCreateDBObject";
		public const string ConnectionProvider_SqlExecutionError = "ConnectionProvider_SqlExecutionError";
		public const string ConnectionProvider_SchemaCorrectionNeeded = "ConnectionProvider_SchemaCorrectionNeeded";
		public const string ConnectionProvider_AtLeastOneColumnExpected = "ConnectionProvider_AtLeastOneColumnExpected";
		public const string ConnectionProvider_Locking = "ConnectionProvider_Locking";
		public const string ConnectionProvider_UnableToOpenDatabase = "ConnectionProvider_UnableToOpenDatabase";
	}
}
