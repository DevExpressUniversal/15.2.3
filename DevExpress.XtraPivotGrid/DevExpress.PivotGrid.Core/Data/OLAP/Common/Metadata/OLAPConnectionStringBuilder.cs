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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils;
#if !SL
using System.Windows.Forms;
using ConnectionStringBuilderBase = System.Data.Common.DbConnectionStringBuilder;
#else
using DevExpress.Utils;
using ConnectionStringBuilderBase = DevExpress.XtraPivotGrid.Data.ConnectionStringBuilder;
#endif
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.XtraPivotGrid.Data {
	public class OLAPConnectionStringBuilder : IOLAPConnectionSettings {
		Builder builder;
		public static int DefaultQueryTimeout { get { return Builder.DefaultQueryTimeout; } }
		public static int DefaultConnectionTimeout { get { return Builder.DefaultConnectionTimeout; } }
		public static int DefaultLCID { get { return Builder.DefaultLCID; } }
		public static string[] PropertiesOrder = new string[] { "Provider", "ServerName", "CatalogName", "CubeName", "QueryTimeout", 
			"LocaleIdentifier", "ConnectionTimeout", "UserId", "Password", "Roles", "CustomData" };
		public OLAPConnectionStringBuilder() {
			this.builder = new Builder();
		}
		public OLAPConnectionStringBuilder(string fullConnectionString)
			: this() {
			ParseConnectionString(fullConnectionString);
		}
		[Basic, TypeConverter(typeof(ProviderTypeConverter))]
		public string Provider {
			get { return builder.Provider; }
			set { builder.Provider = value; }
		}
		[Basic, TypeConverter(typeof(ServerTypeConverter)), DisplayName("Server Name")]
		public string ServerName {
			get { return builder.ServerName; }
			set { builder.ServerName = value; }
		}
		[Basic, TypeConverter(typeof(CatalogTypeConverter)), DisplayName("Catalog Name")]
		public string CatalogName {
			get { return builder.CatalogName; }
			set { builder.CatalogName = value; }
		}
		[TypeConverter(typeof(LocaleTypeConverter)), DisplayName("Language")]
		public int LocaleIdentifier {
			get { return builder.LocaleIdentifier; }
			set { builder.LocaleIdentifier = value; }
		}
		[DisplayName("Connection Timeout (seconds)")]
		public int ConnectionTimeout {
			get { return builder.ConnectionTimeout; }
			set { builder.ConnectionTimeout = value; }
		}
		public string UserId {
			get { return builder.UserId; }
			set { builder.UserId = value; }
		}
#if !DXPORTABLE
		[PasswordPropertyText(true)]
#endif
		public string Password {
			get { return builder.Password; }
			set { builder.Password = value; }
		}
		[Basic, TypeConverter(typeof(CubeTypeConverter)), DisplayName("Cube Name")]
		public string CubeName {
			get { return builder.CubeName; }
			set { builder.CubeName = value; }
		}
		[DisplayName("Query Timeout (seconds)")]
		public int QueryTimeout {
			get { return builder.QueryTimeout; }
			set { builder.QueryTimeout = value; }
		}
		public string Roles {
			get { return builder.Roles; }
			set { builder.Roles = value; }
		}
		public string CustomData {
			get { return builder.CustomData; }
			set { builder.CustomData = value; }
		}
		[Browsable(false)]
		public string FullConnectionString {
			get { return builder.GetConnectionString(false); }
			set { ParseConnectionString(value, true); }
		}
		[Browsable(false)]
		public string ConnectionString {
			get { return builder.GetConnectionString(true); }
			set { ParseConnectionString(value, false); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsValid {
			get { return !string.IsNullOrEmpty(ConnectionString); }
		}
		void ParseConnectionString(string fullConnectionString) {
			try {
				builder.ConnectionString = fullConnectionString;
			} catch {
				builder.Clear();
			}
		}
		void ParseConnectionString(string value, bool parseMyParameters) {
			string cubeName = "";
			int queryTimeOut = DefaultQueryTimeout;
			if(string.IsNullOrEmpty(value)) {
				builder.Clear();
				return;
			}
			if(!parseMyParameters) {
				cubeName = builder.CubeName;
				queryTimeOut = builder.QueryTimeout;
			}
			ParseConnectionString(value);
			if(!parseMyParameters) {
				if(cubeName != "") builder.CubeName = cubeName;
				if(queryTimeOut != 30) builder.QueryTimeout = queryTimeOut;
			}
		}
		class Builder : ConnectionStringBuilderBase {
#if !SL
#if DOTNET
			const RegexOptions DefaultRegexOptions = RegexOptions.None;
#else
			const RegexOptions DefaultRegexOptions = RegexOptions.Compiled;
#endif
			public static int DefaultLCID { get { return CultureInfo.CurrentCulture.GetLCID(); } }
#else
			const RegexOptions DefaultRegexOptions = RegexOptions.None;
			public static int DefaultLCID { get { return 1033; } }
#endif
			const string ProviderString = "provider";
			const string CubeNameString = "cube name";
			const string CustomDataString = "customdata";
			const string QueryTimeoutString = "query timeout";
			const string LocaleIdentifierString = "locale identifier";
			const string ConnectTimeoutString = "connect timeout";
			const string UserIDString = "user id";
			const string PasswordString = "password";
			const string RolesString = "roles";
			const string DataSourceString = "data source";
			const string InitialCatalogString = "initial catalog";
			public const int DefaultQueryTimeout = 30;
			public const int DefaultConnectionTimeout = 60;			
			int queryTimeout, connectionTimeout,
				localeIdentifier;
			Regex connectionStringValidKeyRegex,
				connectionStringQuoteValueRegex;
			public Builder()
				: base() {
				this.connectionStringValidKeyRegex = new Regex(@"^(?![;\s])[^\p{Cc}]+(?<!\s)$", DefaultRegexOptions);
				this.connectionStringQuoteValueRegex = new Regex("^[^\"'=;\\s\\p{Cc}]*$", DefaultRegexOptions);
				ResetProperties();
			}
			public int QueryTimeout {
				get { return queryTimeout; }
				set {
					if(QueryTimeout == value) return;
					this.queryTimeout = value;
					if(value == DefaultQueryTimeout)
						base.Remove(QueryTimeoutString);
					else
						base[QueryTimeoutString] = value;
				}
			}
			public int ConnectionTimeout {
				get { return connectionTimeout; }
				set {
					if(ConnectionTimeout == value) return;
					this.connectionTimeout = value;
					if(value == DefaultConnectionTimeout)
						base.Remove(ConnectTimeoutString);
					else
						base[ConnectTimeoutString] = value;
				}
			}
			public int LocaleIdentifier {
				get { return localeIdentifier; }
				set {
					if(LocaleIdentifier == value) return;
					this.localeIdentifier = value;
					if(value == DefaultLCID)
						base.Remove(LocaleIdentifierString);
					else
						base[LocaleIdentifierString] = value;
				}
			}
			public string Provider {
				get { return (string)this[ProviderString]; }
				set { this[ProviderString] = value; }
			}
			public string ServerName {
				get { return (string)this[DataSourceString]; }
				set { this[DataSourceString] = value; }
			}
			public string CatalogName {
				get { return (string)this[InitialCatalogString]; }
				set { this[InitialCatalogString] = value; }
			}
			public string UserId {
				get { return (string)this[UserIDString]; }
				set { this[UserIDString] = value; }
			}
			public string Password {
				get { return (string)this[PasswordString]; }
				set { this[PasswordString] = value; }
			}
			public string CubeName {
				get { return (string)this[CubeNameString]; }
				set { this[CubeNameString] = value; }
			}
			public string Roles {
				get { return (string)this[RolesString]; }
				set { this[RolesString] = value; }
			}
			public string CustomData {
				get { return (string)this[CustomDataString]; }
				set { this[CustomDataString] = value; }
			}
			public override object this[string keyword] {
				get {
					if(ContainsKey(keyword))
						return base[keyword];
					else return string.Empty;
				}
				set {
					switch(keyword) {
						case QueryTimeoutString:
							QueryTimeout = ParseInt(value, DefaultQueryTimeout);
							return;
						case ConnectTimeoutString:
							ConnectionTimeout = ParseInt(value, DefaultConnectionTimeout);
							return;
						case LocaleIdentifierString:
							LocaleIdentifier = ParseInt(value, DefaultLCID);
							return;
					}
					base[keyword] = value;
				}
			}
			public override void Clear() {
				ResetProperties();
				base.Clear();
			}
			public override bool Remove(string keyword) {
				if(this[keyword] is int) this[keyword] = "s";
				return base.Remove(keyword);
			}
			int ParseInt(object value, object defaultValue) {
				int newValue;
				if(value is int) return (int)value;
				if(int.TryParse((string)value, out newValue))
					return newValue;
				else
					return (int)defaultValue;
			}
			void ResetProperties() {
				queryTimeout = DefaultQueryTimeout;
				connectionTimeout = DefaultConnectionTimeout;
				localeIdentifier = DefaultLCID;
			}
			bool IsValueValidInternal(string keyvalue) {
				if(keyvalue != null)
					return keyvalue.IndexOf('\0') == -1;
				return false;
			}
			void AppendKeyValuePairBuilder(StringBuilder builder, string keyName, string keyValue) {
				if(string.IsNullOrEmpty(keyName) || !connectionStringValidKeyRegex.IsMatch(keyName))
					return;
				if(!IsValueValidInternal(keyValue)) return;
				if(builder.Length > 0 && builder[builder.Length - 1] != ';')
					builder.Append(";");
				builder.Append(keyName.Replace("=", "==")).Append("=");
				if(keyValue != null) {
					if(connectionStringQuoteValueRegex.IsMatch(keyValue)) {
						builder.Append(keyValue);
					} else if((-1 != keyValue.IndexOf('"')) && (-1 == keyValue.IndexOf('\''))) {
						builder.Append('\'');
						builder.Append(keyValue);
						builder.Append('\'');
					} else {
						builder.Append('"');
						builder.Append(keyValue.Replace("\"", "\"\""));
						builder.Append('"');
					}
				}
			}
			public string GetConnectionString(bool ignoreMyParameters) {
				if(string.IsNullOrEmpty(ServerName) && string.IsNullOrEmpty(CatalogName))
					return string.Empty;
				StringBuilder res = new StringBuilder();
				AppendKeyValuePairBuilder(res, "provider", string.IsNullOrEmpty(Provider) ? "msolap" : Provider);
				foreach(KeyValuePair<string, object> record in this) {
					string key = record.Key;
					if (StringExtensions.CompareInvariantCultureIgnoreCase(key, ProviderString) == 0) continue;
					if (StringExtensions.CompareInvariantCultureIgnoreCase(key, QueryTimeoutString) == 0 &&
						QueryTimeout == DefaultQueryTimeout) continue;
					if(ignoreMyParameters &&
						(StringExtensions.CompareInvariantCultureIgnoreCase(key, QueryTimeoutString) == 0 ||
						 StringExtensions.CompareInvariantCultureIgnoreCase(key, CubeNameString) == 0)) continue;
					AppendKeyValuePairBuilder(res, key, Convert.ToString(record.Value));
				}
				return res.ToString();
			}
		}
	}
	class ConnectionStringBuilder : IEnumerable {
		string connectionString;
		Dictionary<string, object> currentValues;
		internal ConnectionStringBuilder() {
		}
		public virtual object this[string key] {
			get {
				object obj;
				if(string.IsNullOrEmpty(key) || !this.CurrentValues.TryGetValue(key, out obj))
					throw new ArgumentException("Invalid Key");
				return obj;
			}
			set {
				if(value != null) {
					this.CurrentValues[key] = value;
				} else {
					this.Remove(key);
				}
				this.connectionString = null;
			}
		}
		public virtual bool Remove(string key) {
			if(this.CurrentValues.Remove(key)) {
				this.connectionString = null;
				return true;
			}
			return false;
		}
		public virtual void Clear() {
			this.connectionString = string.Empty;
			if(this.currentValues != null)
				this.currentValues.Clear();
		}
		public string ConnectionString {
			get { return this.connectionString; }
			set {
				Parser parser = new Parser();
				string prevConnectionString = this.connectionString;
				Clear();
				try {
					for(ChainElement currectChainElem = parser.GetChain(value); currectChainElem != null; currectChainElem = currectChainElem.Next) {
						if(currectChainElem.Value != null) {
							this[currectChainElem.Name] = currectChainElem.Value;
						} else {
							this.Remove(currectChainElem.Name);
						}
					}
					this.connectionString = null;
				} catch(Exception e) {
					this.connectionString = prevConnectionString;
					throw e;
				}
			}
		}
		public virtual bool ContainsKey(string key) {
			if(this.currentValues == null) return false;
			return this.CurrentValues.ContainsKey(key);
		}
		Dictionary<string, object> CurrentValues {
			get {
				if(this.currentValues == null)
					this.currentValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
				return this.currentValues;
			}
		}
#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return this.CurrentValues.GetEnumerator();
		}
#endregion
		class Parser {
			static bool IsKeyNameValid(string keyname) {
				if(keyname == null) return false;
				return (keyname.Length > 0) && (keyname[0] != ';')
					&& !char.IsWhiteSpace(keyname[0]) && (keyname.IndexOf(KeyValueParser.NullChar) == -1);
			}
			internal Parser() {
			}
			internal ChainElement GetChain(string connectionString) {
				Hashtable parsetable = new Hashtable();
				StringBuilder buffer = new StringBuilder();
				ChainElement elem = null, currentElem = null;
				int num = 0;
				int length = connectionString.Length;
				while(num < length) {
					int currentPosition = num;
					KeyValueParser keyParser = new KeyValueParser(connectionString, currentPosition, buffer);
					num = keyParser.Parse();
					if(string.IsNullOrEmpty(keyParser.KeyName))
						return currentElem;
					string keyname = keyParser.KeyName;
					if(!IsKeyNameValid(keyname))
						throw new ArgumentException("Unsupported Key: " + keyname);
					parsetable[keyname] = keyParser.KeyValue;
					if(elem != null) {
						elem = elem.Next = new ChainElement(keyname, keyParser.KeyValue, num - currentPosition);
					} else {
						currentElem = elem = new ChainElement(keyname, keyParser.KeyValue, num - currentPosition);
					}
				}
				return currentElem;
			}
		}
		class KeyValueParser {
			internal const char NullChar = '\0';
			enum CycleStateEnum {
				Default = 0,
				Continue = 1
			}
			enum StateEnum {
				Default = 1,
				Key = 2,
				KeyEqual = 3,
				KeyEnd = 4,
				UnquotedValue = 5,
				DoubleQuoteValue = 6,
				DoubleQuoteValueQuote = 7,
				SingleQuoteValue = 8,
				SingleQuoteValueQuote = 9,
				BraceQuoteValue = 10,
				BraceQuoteValueQuote = 11,
				QuotedValueEnd = 12,
				NullTermination = 13
			}
			static string GetKeyName(StringBuilder buffer) {
				int length = buffer.Length;
				while((0 < length) && char.IsWhiteSpace(buffer[length - 1])) {
					length--;
				}
				return buffer.ToString(0, length).ToLower(System.Globalization.CultureInfo.InvariantCulture);
			}
			static string GetKeyValue(StringBuilder buffer, bool trimWhitespace) {
				int length = buffer.Length;
				int startIndex = 0;
				if(trimWhitespace) {
					while((startIndex < length) && char.IsWhiteSpace(buffer[startIndex])) {
						startIndex++;
					}
					while((0 < length) && char.IsWhiteSpace(buffer[length - 1])) {
						length--;
					}
				}
				return buffer.ToString(startIndex, length - startIndex);
			}
			static Exception GenerateConnectionStringSyntaxException(int index) {
				return new ArgumentException("ConnectionString Syntax Error", "index=" + index.ToString());
			}
			static Exception GenerateInternalErrorException(string error) {
				return new InvalidOperationException("Internal Error: '" + error + "'");
			}
			string connectionString;
			string keyName;
			string keyValue;
			StringBuilder buffer;
			StateEnum state;
			int currentPosition;
			internal KeyValueParser(string connectionString, int currentPosition, StringBuilder buffer) {
				this.connectionString = connectionString;
				this.currentPosition = currentPosition;
				this.buffer = buffer;
				this.keyName = null;
				this.keyValue = null;
				ChangeState(StateEnum.Default);
			}
			internal string KeyName {
				get { return keyName; }
			}
			internal string KeyValue {
				get { return keyValue; }
			}
			internal int Parse() {
				int index = currentPosition;
				char c = NullChar;
				int length = connectionString.Length;
				bool finished = false;
				buffer.Length = 0;
				while(!finished && currentPosition < length) {
					c = connectionString[currentPosition];
					switch(state) {
						case StateEnum.Default:
							if((c != ';') && !char.IsWhiteSpace(c)) {
								if(c != NullChar) {
									if(char.IsControl(c))
										throw GenerateConnectionStringSyntaxException(index);
									index = currentPosition;
									if(c != '=') {
										Append(c, StateEnum.Key);
										continue;
									}
									Next(StateEnum.KeyEqual);
									continue;
								}
								ChangeState(StateEnum.NullTermination);
							}
							Next();
							continue;
						case StateEnum.Key:
							if(c == '=') {
								Next(StateEnum.KeyEqual);
								continue;
							}
							if(char.IsWhiteSpace(c) || !char.IsControl(c)) {
								Append(c);
								continue;
							}
							throw GenerateConnectionStringSyntaxException(index);
						case StateEnum.KeyEnd:
						case StateEnum.KeyEqual:
							if(state == StateEnum.KeyEqual) {
								if(c == '=') {
									Append(c, StateEnum.Key);
									continue;
								}
								this.keyName = GetKeyName(buffer);
								if(string.IsNullOrEmpty(this.keyName))
									throw GenerateConnectionStringSyntaxException(index);
								buffer.Length = 0;
								ChangeState(StateEnum.KeyEnd);
							}
							if(char.IsWhiteSpace(c)) {
								Next();
								continue;
							}
							if(c == '\'') {
								Next(StateEnum.SingleQuoteValue);
								continue;
							}
							if(c == '"') {
								Next(StateEnum.DoubleQuoteValue);
								continue;
							}
							if((c == ';') || (c == NullChar)) {
								index = currentPosition;
								Append(c, StateEnum.Key);
								continue;
							}
							if(char.IsControl(c))
								throw GenerateConnectionStringSyntaxException(index);
							Append(c, StateEnum.UnquotedValue);
							continue;
						case StateEnum.UnquotedValue:
							if(char.IsWhiteSpace(c) || (!char.IsControl(c) && (c != ';'))) {
								Append(c);
								continue;
							}
							finished = true;
							continue;
						case StateEnum.DoubleQuoteValue:
							if(c == '"') {
								Next(StateEnum.DoubleQuoteValueQuote);
								continue;
							}
							if(c != NullChar) {
								Append(c);
								continue;
							}
							throw GenerateConnectionStringSyntaxException(index);
						case StateEnum.DoubleQuoteValueQuote:
							if(c != '"') {
								CommonInit();
								break;
							}
							Append(c, StateEnum.DoubleQuoteValue);
							continue;
						case StateEnum.SingleQuoteValue:
							if(c == '\'') {
								Next(StateEnum.SingleQuoteValueQuote);
								continue;
							}
							if(c != NullChar) {
								Append(c);
								continue;
							}
							throw GenerateConnectionStringSyntaxException(index);
						case StateEnum.SingleQuoteValueQuote:
							if(c != '\'') {
								CommonInit();
								break;
							}
							Append(c, StateEnum.SingleQuoteValue);
							continue;
						case StateEnum.BraceQuoteValue:
							if(c == '}') {
								Append(c, StateEnum.BraceQuoteValueQuote);
								continue;
							}
							if(c != NullChar) {
								Append(c);
								continue;
							}
							throw GenerateConnectionStringSyntaxException(index);
						case StateEnum.BraceQuoteValueQuote:
							if(c != '}') {
								CommonInit();
								break;
							}
							Append(c, StateEnum.BraceQuoteValue);
							continue;
						case StateEnum.QuotedValueEnd:
							break;
						case StateEnum.NullTermination:
							if((c != NullChar) && !char.IsWhiteSpace(c))
								throw GenerateConnectionStringSyntaxException(currentPosition);
							Next();
							continue;
						default:
							throw GenerateInternalErrorException("Invalid KeyValueParser State");
					}
					if(char.IsWhiteSpace(c)) {
						Next();
						continue;
					}
					if(c == ';')
						break;
					if(c == NullChar) {
						Next(StateEnum.NullTermination);
						continue;
					}
					throw GenerateConnectionStringSyntaxException(index);
				}
				switch(state) {
					case StateEnum.Default:
					case StateEnum.KeyEnd:
					case StateEnum.NullTermination:
						break;
					case StateEnum.Key:
					case StateEnum.DoubleQuoteValue:
					case StateEnum.SingleQuoteValue:
					case StateEnum.BraceQuoteValue:
						throw GenerateConnectionStringSyntaxException(index);
					case StateEnum.KeyEqual:
						this.keyName = GetKeyName(buffer);
						if(string.IsNullOrEmpty(this.keyName))
							throw GenerateConnectionStringSyntaxException(index);
						break;
					case StateEnum.UnquotedValue:
						this.keyValue = GetKeyValue(buffer, true);
						char ch2 = this.keyValue[this.keyValue.Length - 1];
						if((ch2 == '\'') || (ch2 == '"'))
							throw GenerateConnectionStringSyntaxException(index);
						break;
					case StateEnum.DoubleQuoteValueQuote:
					case StateEnum.SingleQuoteValueQuote:
					case StateEnum.BraceQuoteValueQuote:
					case StateEnum.QuotedValueEnd:
						this.keyValue = GetKeyValue(buffer, false);
						break;
					default:
						throw GenerateInternalErrorException("Invalid Parser State");
				}
				if((c == ';') && (currentPosition < connectionString.Length))
					Next();
				return currentPosition;
			}
			void CommonInit() {
				this.keyValue = GetKeyValue(buffer, false);
				ChangeState(StateEnum.QuotedValueEnd);
			}
			void Append(char c, StateEnum newState) {
				ChangeState(newState);
				Append(c);
			}
			void Append(char c) {
				this.buffer.Append(c);
				Next();
			}
			void Next(StateEnum newState) {
				ChangeState(newState);
				Next();
			}
			void Next() {
				this.currentPosition++;
			}
			void ChangeState(StateEnum newState) {
				this.state = newState;
			}
		}
		class ChainElement {
			readonly int length;
			readonly string name;
			readonly string value;
			ChainElement next;
			internal ChainElement(string name, string value, int length) {
				this.name = name;
				this.value = value;
				this.length = length;
			}
			internal int Length { get { return length; } }
			internal string Name { get { return name; } }
			internal ChainElement Next { get { return next; } set { next = value; } }
			internal string Value { get { return value; } }
		}
	}
#region Type Converters
#if !SL
	class BasicAttribute : Attribute {
#if !DXPORTABLE
		public override object TypeId { get { return "Basic"; } }
#endif
	}
	abstract class ConnectionTypeConverter : OLAPTypeConverterBase {
		List<string> fNames;
		bool isLastConnectFailed = true;
		protected bool IsLastConnectFailed {
			get { return isLastConnectFailed; }
			set { isLastConnectFailed = value; }
		}
		long lastConnectTickCount = 0;
		OLAPMetaGetter metaGetter = new OLAPMetaGetter();
		protected OLAPMetaGetter MetaGetter { get { return metaGetter; } }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(GetNames((OLAPConnectionStringBuilder)context.Instance));
		}
		protected void Connect(string providerName, string serverName, string catalogName) {
			if(!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(providerName)) {
				if(IsLastConnectFailed && (Environment.TickCount - lastConnectTickCount < 1000)) return;
				MetaGetter.ConnectionString = GetBaseConnectionString(providerName, serverName);
				if(!MetaGetter.Connected) {
					ShowMessage("Couldn't connect to the server.", "Error");
					lastConnectTickCount = Environment.TickCount;
					return;
				}
				if(!String.IsNullOrEmpty(serverName)) {
					metaGetter.ConnectionString += ";Initial Catalog=" + catalogName;
				}
				if(!metaGetter.Connected) {
					ShowMessage("Couldn't connect to the \"" + catalogName + "\" database.", "Error");
					lastConnectTickCount = Environment.TickCount;
					return;
				}
			}
		}
		public static void ShowMessage(string message, string caption) {
#if DXPORTABLE
			System.Diagnostics.Debug.WriteLine(caption + ": " + message);
#else
			MessageBox.Show(message, caption);
#endif
		}
		public static void ShowMessage(string message) {
#if DXPORTABLE
			System.Diagnostics.Debug.WriteLine(message);
#else
			MessageBox.Show(message);
#endif
		}
		string GetBaseConnectionString(string providerName, string serverName) {
			return "Provider=" + providerName + ";Data Source=" + serverName;
		}
		protected List<string> GetNames(OLAPConnectionStringBuilder options) {
			if(!SkipRefresh(options)) {
				fNames = GetNamesCore(options);
			}
			return fNames;
		}
		protected abstract bool SkipRefresh(OLAPConnectionStringBuilder options);
		protected abstract List<string> GetNamesCore(OLAPConnectionStringBuilder options);
	}
	class OLAPTypeConverterBase : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string)) return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string) return (string)value;
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) return Convert.ToString(value);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	class ProviderTypeConverter : OLAPTypeConverterBase {
		List<string> providers;
		public ProviderTypeConverter() {
			providers = OLAPMetaGetter.GetProviders();
			if(providers.Count == 0) {
				string linkOleDb = " http://www.microsoft.com/downloads/details.aspx?FamilyID=50b97994-8453-4998-8226-fa42ec403d17#ASOLEDB";
				ConnectionTypeConverter.ShowMessage(PivotGridLocalizer.GetString(PivotGridStringId.OLAPNoOleDbProvidersMessage) + linkOleDb);
			}
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(providers);
		}
	}
	class ServerTypeConverter : OLAPTypeConverterBase {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return false;
		}
	}
	class CatalogTypeConverter : ConnectionTypeConverter {
		string oldProvider, oldServerName;
		protected override bool SkipRefresh(OLAPConnectionStringBuilder options) {
			bool skip = oldProvider == options.Provider && oldServerName == options.ServerName && !IsLastConnectFailed;
			if(!skip) {
				oldProvider = options.Provider;
				oldServerName = options.ServerName;
			}
			return skip;
		}
		protected override List<string> GetNamesCore(OLAPConnectionStringBuilder options) {
			Connect(options.Provider, options.ServerName, string.Empty);
			IsLastConnectFailed = !MetaGetter.Connected;
			return MetaGetter.GetCatalogs();
		}
	}
	class CubeTypeConverter : ConnectionTypeConverter {
		string oldProvider, oldServerName, oldCatalogName;
		protected override bool SkipRefresh(OLAPConnectionStringBuilder options) {
			bool skip = oldProvider == options.Provider && oldServerName == options.ServerName && oldCatalogName == options.CatalogName && !IsLastConnectFailed;
			if(!skip) {
				oldProvider = options.Provider;
				oldServerName = options.ServerName;
				oldCatalogName = options.CatalogName;
			}
			return skip;
		}
		protected override List<string> GetNamesCore(OLAPConnectionStringBuilder options) {
			Connect(options.Provider, options.ServerName, options.CatalogName);
			IsLastConnectFailed = !MetaGetter.Connected;
			return MetaGetter.GetCubes(options.CatalogName);
		}
	}
#if DXPORTABLE
	class LocaleTypeConverter : TypeConverter {
		public LocaleTypeConverter() { }
	}
#else
	class LocaleTypeConverter : TypeConverter {
		List<CultureInfo> fCultures;
		List<string> fEnglishNames;
		public LocaleTypeConverter() {
			fCultures = new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures));
			fEnglishNames = new List<string>(fCultures.Count);
			foreach(CultureInfo info in fCultures)
				fEnglishNames.Add(info.EnglishName);
			fEnglishNames.Sort();
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string)) return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string) return GetLCID((string)value);
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(value is string) return (string)value;
			if(destinationType == typeof(string)) return GetName((int)value);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(fEnglishNames);
		}
		string GetName(int LCID) {
			CultureInfo info = FindByLCID(LCID);
			if(info == null)
				return FindByLCID(OLAPConnectionStringBuilder.DefaultLCID).EnglishName;
			return info.EnglishName;
		}
		CultureInfo FindByName(string englishName) {
			foreach(CultureInfo info in fCultures) {
				if(info.EnglishName == englishName)
					return info;
			}
			return null;
		}
		CultureInfo FindByLCID(int LCID) {
			foreach(CultureInfo info in fCultures) {
				if(info.GetLCID() == LCID)
					return info;
			}
			return null;
		}
		int GetLCID(string englishName) {
			return FindByName(englishName).GetLCID();
		}
		class CultureComparer : IComparer<CultureInfo> {
			int IComparer<CultureInfo>.Compare(CultureInfo x, CultureInfo y) {
				return Comparer.Default.Compare(x.EnglishName, y.EnglishName);
			}
		}
	}
#endif
#else
		class BasicAttribute : Attribute {
	}
	class PasswordPropertyTextAttribute : Attribute {
		internal PasswordPropertyTextAttribute(bool password) { }
	}
	class OLAPTypeConverterBase : TypeConverter {
	}
	abstract class ConnectionTypeConverter : OLAPTypeConverterBase {
	}
	class ProviderTypeConverter : OLAPTypeConverterBase {
		public ProviderTypeConverter() { }
	}
	class ServerTypeConverter : OLAPTypeConverterBase {
	}
	class CatalogTypeConverter : ConnectionTypeConverter {
	}
	class CubeTypeConverter : ConnectionTypeConverter {
	}
	class LocaleTypeConverter : TypeConverter {	
		public LocaleTypeConverter() {}
	}
#endif
	#endregion
}
