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
using System.Globalization;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Utils;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Core.DataSources {
	public class PivotOlapDataSource : DXDesignTimeControl {
		#region static
#if !SL
		public static int DefaultLCID { get { return CultureInfo.CurrentCulture.LCID; } }
#else
		public static int DefaultLCID { get { return 1033; } }
#endif
		public static readonly DependencyProperty ServerProperty;
		public static readonly DependencyProperty CatalogProperty;
		public static readonly DependencyProperty CubeProperty;
		public static readonly DependencyProperty ConnectionTimeoutProperty;
		public static readonly DependencyProperty LocaleIdentifierProperty;
		public static readonly DependencyProperty QueryTimeoutProperty;
		public static readonly DependencyProperty UserIdProperty;
		public static readonly DependencyProperty PasswordProperty;
#if !SL
		public static readonly DependencyProperty ProviderProperty;
#endif
		public static readonly DependencyProperty ConnectionStringProperty;
		static readonly DependencyPropertyKey ConnectionStringPropertyKey;
		static PivotOlapDataSource() {
			Type ownerType = typeof(PivotOlapDataSource);
			ServerProperty = DependencyPropertyManager.Register("Server", typeof(string), ownerType,
				new UIPropertyMetadata(null, (d, e) => ((PivotOlapDataSource)d).UpdateConnectionString()));
			CatalogProperty = DependencyPropertyManager.Register("Catalog", typeof(string), ownerType,
				new UIPropertyMetadata(null, (d, e) => ((PivotOlapDataSource)d).UpdateConnectionString()));
			CubeProperty = DependencyPropertyManager.Register("Cube", typeof(string), ownerType,
				new UIPropertyMetadata(null, (d, e) => ((PivotOlapDataSource)d).UpdateConnectionString()));
			ConnectionTimeoutProperty = DependencyPropertyManager.Register("ConnectionTimeout", typeof(int), ownerType,
				new UIPropertyMetadata(60, (d, e) => ((PivotOlapDataSource)d).UpdateConnectionString(), CoerceTimeout));
			LocaleIdentifierProperty = DependencyPropertyManager.Register("LocaleIdentifier", typeof(int), ownerType,
				new UIPropertyMetadata(DefaultLCID, (d, e) => ((PivotOlapDataSource)d).UpdateConnectionString()));
			QueryTimeoutProperty = DependencyPropertyManager.Register("QueryTimeout", typeof(int), ownerType,
				new UIPropertyMetadata(30, (d, e) => ((PivotOlapDataSource)d).UpdateConnectionString(), CoerceTimeout));
			UserIdProperty = DependencyPropertyManager.Register("UserId", typeof(string), ownerType,
				new UIPropertyMetadata(null, (d, e) => ((PivotOlapDataSource)d).UpdateConnectionString()));
			PasswordProperty = DependencyPropertyManager.Register("Password", typeof(string), ownerType,
				new UIPropertyMetadata(null, (d, e) => ((PivotOlapDataSource)d).UpdateConnectionString()));
#if !SL
			ProviderProperty = DependencyPropertyManager.Register("Provider", typeof(string), ownerType,
				new UIPropertyMetadata(null, (d, e) => ((PivotOlapDataSource)d).UpdateConnectionString()));
#endif
			ConnectionStringPropertyKey = DependencyPropertyManager.RegisterReadOnly("ConnectionString", typeof(string), ownerType,
				new UIPropertyMetadata(null));
			ConnectionStringProperty = ConnectionStringPropertyKey.DependencyProperty;
		}
		#endregion
		public PivotOlapDataSource() { }
		#region Properties
#if !SL
		public string Provider {
			get { return (string)GetValue(ProviderProperty); }
			set { SetValue(ProviderProperty, value); }
		}
#endif
		public string Server {
			get { return (string)GetValue(ServerProperty); }
			set { SetValue(ServerProperty, value); }
		}
		public string Catalog {
			get { return (string)GetValue(CatalogProperty); }
			set { SetValue(CatalogProperty, value); }
		}
		public string Cube {
			get { return (string)GetValue(CubeProperty); }
			set { SetValue(CubeProperty, value); }
		}
		public int ConnectionTimeout {
			get { return (int)GetValue(ConnectionTimeoutProperty); }
			set { SetValue(ConnectionTimeoutProperty, value); }
		}
		public int LocaleIdentifier {
			get { return (int)GetValue(LocaleIdentifierProperty); }
			set { SetValue(LocaleIdentifierProperty, value); }
		}
		public int QueryTimeout {
			get { return (int)GetValue(QueryTimeoutProperty); }
			set { SetValue(QueryTimeoutProperty, value); }
		}
		public string UserId {
			get { return (string)GetValue(UserIdProperty); }
			set { SetValue(UserIdProperty, value); }
		}
		public string Password {
			get { return (string)GetValue(PasswordProperty); }
			set { SetValue(PasswordProperty, value); }
		}
		public string ConnectionString {
			get { return (string)GetValue(ConnectionStringProperty); }
			protected set { this.SetValue(ConnectionStringPropertyKey, value); }
		}
		#endregion Properties
		protected virtual void UpdateConnectionString() {
			StringBuilder builder = new StringBuilder();
			if(string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Catalog) || string.IsNullOrEmpty(Cube)) {
				ConnectionString = string.Empty;
				return;
			}
#if !SL
			if(!string.IsNullOrEmpty(Provider))
				builder.Append("Provider=").Append(Provider).Append(";");
#endif
			builder.Append("Data Source=").Append(Server).Append(";");
			builder.Append("initial catalog=").Append(Catalog).Append(";");
			builder.Append("cube name=").Append(Cube).Append(";");
			if(ConnectionTimeout != 60)
				builder.Append("connect timeout=").Append(ConnectionTimeout).Append(";");
			if(QueryTimeout != 30)
				builder.Append("query timeout=").Append(QueryTimeout).Append(";");
			if(LocaleIdentifier != DefaultLCID) {
				builder.Append("locale identifier=").Append(LocaleIdentifier).Append(";");
			}
			if(!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(Password))
				builder.Append("user id=").Append(UserId).Append(";password=").Append(Password).Append(";");
			ConnectionString = builder.ToString();
		}
		protected override string GetDesignTimeImageName() {
			return string.Empty;
		}
		static object CoerceTimeout(DependencyObject d, object baseValue) {
			int value = (int)baseValue;
			return (value < 1) ? 1 : value;
		}
	}
}
