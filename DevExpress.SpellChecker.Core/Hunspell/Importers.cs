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
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using DevExpress.Utils;
namespace DevExpress.XtraSpellChecker.Hunspell {
	public delegate void TranslateKeywordHandler(AffixImporter importer, string value);
	public class TranslateKeywordTable : Dictionary<string, TranslateKeywordHandler> { }
	#region HunspellImporter (abstract class)
	public abstract class HunspellImporter {
		readonly HunspellDictionary hunspellDictionary;
		readonly HunspellImportingInfo importingInfo;
		protected HunspellImporter(HunspellDictionary hunspellDictionary, HunspellImportingInfo importingInfo) {
			Guard.ArgumentNotNull(hunspellDictionary, "hunspellDictionary");
			Guard.ArgumentNotNull(importingInfo, "importingInfo");
			this.hunspellDictionary = hunspellDictionary;
			this.importingInfo = importingInfo;
		}
		public HunspellDictionary Dictionary { get { return hunspellDictionary; } }
		protected internal Encoding Encoding { get { return ImportingInfo.Encoding; } }
		protected internal FlagTypes FlagType { get { return ImportingInfo.FlagType; } }
		protected HunspellImportingInfo ImportingInfo { get { return importingInfo; } }
		public virtual void Load(Stream stream) {
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanRead)
				throw new ArgumentException("Stream doesn't supports reading data", "stream");
			if (!stream.CanSeek)
				throw new ArgumentException("Stream doesn't supports seeking");
			stream.Seek(0, SeekOrigin.Begin);
			LoadCore(stream);
		}
		protected abstract void LoadCore(Stream stream);
		protected internal string[] GetTokens(string value) {
			return value.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
		}
		protected internal string GetToken(string value) {
			return GetTokens(value)[0];
		}
		protected internal bool IsLineBreak(char ch) {
			return ch == '\n' || ch == '\r';
		}
		protected internal void SetFlags(IFlagedObject flagedObject, string value) {
			if (ImportingInfo.FlagsSetCollection.Count > 0) {
				int index;
				if (Int32.TryParse(value, out index) && index > 0 && index <= ImportingInfo.FlagsSetCollection.Count) {
					flagedObject.Flags = ImportingInfo.FlagsSetCollection[index - 1];
					return;
				}
			}
			FlagsCollection flags = new FlagsCollection();
			flags.AddRange(ParseFlags(value));
			flags.Sort();
			flagedObject.Flags = flags;
		}
		#region Flags parser
		protected internal HunspellFlag ParseFlag(string flags) {
			List<HunspellFlag> result = ParseFlags(flags);
			return result.Count > 0 ? result[0] : HunspellFlag.Empty;
		}
		protected internal List<HunspellFlag> ParseFlags(string flags) {
			return ParseFlags(flags, FlagType, Encoding);
		}
		protected internal List<HunspellFlag> ParseFlags(string flags, FlagTypes type, Encoding encoding) {
			if (type == FlagTypes.Number)
				return GetIntFlags(flags);
			if (type == FlagTypes.TwoChar)
				return GetTwoCharFlags(flags);
			if (type == FlagTypes.UTF8)
				return GetUTF8Flags(flags, encoding);
			return GetCharFlags(flags);
		}
		protected internal virtual List<HunspellFlag> GetIntFlags(string value) {
			string[] flags = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			int count = flags.Length;
			List<HunspellFlag> result = new List<HunspellFlag>();
			for (int i = 0; i < count; i++)
				result.Add(CreateIntFlag(flags[i]));
			return result;
		}
		protected internal HunspellFlag CreateIntFlag(string value) {
			int flag;
			if (Int32.TryParse(value, out flag))
				return new HunspellFlag(flag);
			throw new Exception("Number flag is incorrect");
		}
		protected List<HunspellFlag> GetUTF8Flags(string flags, Encoding encoding) {
			string result = new string(Encoding.UTF8.GetChars(encoding.GetBytes(flags)));
			return GetCharFlags(result);
		}
		protected List<HunspellFlag> GetTwoCharFlags(string flags) {
			int length = flags.Length;
			List<HunspellFlag> result = new List<HunspellFlag>();
			for (int i = length % 2; i < length; i += 2) {
				int hash = (flags[i] << 16) | flags[i + 1];
				result.Add(new HunspellFlag(hash));
			}
			return result;
		}
		protected List<HunspellFlag> GetCharFlags(string flags) {
			int count = flags.Length;
			List<HunspellFlag> result = new List<HunspellFlag>();
			for (int i = 0; i < count; i++)
				result.Add(new HunspellFlag((int)flags[i]));
			return result;
		}
		#endregion
	}
	#endregion
	#region AffixImporter
	public class AffixImporter : HunspellImporter {
		const bool x = true;
		const bool o = false;
		enum EncodingType {
			UTF_8 = 0,
			UTF_32BE = 1,
			UTF_32LE = 2,
			UTF_16BE = 3,
			UTF_16LE = 4
		}
		class EncodingInfo {
			readonly Encoding encoding;
			readonly bool[] byteOrderMask;
			internal EncodingInfo(Encoding encoding, bool[] byteOrderMask) {
				this.encoding = encoding;
				this.byteOrderMask = byteOrderMask;
			}
			public Encoding Encoding { get { return encoding; } }
			public bool[] ByteOrderMask { get { return byteOrderMask; } }
		}
		delegate T CreateAffix<T>(HunspellFlag key, bool canCombine) where T : AffixEntry;
		delegate bool ProcessKeyValuePair(string key, string value);
		static Dictionary<EncodingType, EncodingInfo> EncodingTable = CreateEncodingTable();
		static Dictionary<EncodingType, EncodingInfo> CreateEncodingTable() {
			Dictionary<EncodingType, EncodingInfo> result = new Dictionary<EncodingType, EncodingInfo>();
			result.Add(EncodingType.UTF_8, new EncodingInfo(new UTF8Encoding(true), new bool[] { x }));
			result.Add(EncodingType.UTF_16LE, new EncodingInfo(new UnicodeEncoding(false, true), new bool[] { x, o }));
			result.Add(EncodingType.UTF_16BE, new EncodingInfo(new UnicodeEncoding(true, true), new bool[] { o, x }));
#if !SL
			result.Add(EncodingType.UTF_32LE, new EncodingInfo(new UTF32Encoding(false, true), new bool[] { x, o, o, o }));
			result.Add(EncodingType.UTF_32BE, new EncodingInfo(new UTF32Encoding(true, true), new bool[] { o, o, o, x }));
#endif
			return result;
		}
		static TranslateKeywordTable keywordTable = CreateKeywordTable();
		static TranslateKeywordTable CreateKeywordTable() {
			TranslateKeywordTable table = new TranslateKeywordTable();
			table.Add("SFX", OnSuffixKeyword);
			table.Add("PFX", OnPreffixKeyword);
			table.Add("KEY", OnKeyKeyword);
			table.Add("TRY", OnTryKeyword);
			table.Add("COMPOUNDFLAG", OnCompoundFlagKeyword);
			table.Add("COMPOUNDMIN", OnCompoundMinKeyword);
			table.Add("COMPOUNDPERMITFLAG", OnCompoundPermitFlagKeyword);
			table.Add("COMPOUNDFORBIDFLAG", OnCompoundForbidFlagKeyword);
			table.Add("COMPOUNDWORDMAX", OnCompoundWordMaxKeyword);
			table.Add("COMPOUNDRULE", OnCompoundRuleKeyword);
			table.Add("ONLYINCOMPOUND", OnOnlyCompoundKeyword);
			table.Add("CHECKCOMPOUNDPATTERN", OnCompoundPatternKeyword);
			table.Add("COMPOUNDBEGIN", OnCompoundBeginKeyword);
			table.Add("COMPOUNDMIDDLE", OnCompoundMiddleKeyword);
			table.Add("COMPOUNDEND", OnCompoundLastKeyword);
			table.Add("CHECKCOMPOUNDTRIPLE", OnCheckCompoundTripleKeyword);
			table.Add("CHECKCOMPOUNDDUP", OnCheckCompoundDupKeyword);
			table.Add("SIMPLIFIEDTRIPLE", OnSimplifiedTripleKeyword);
			table.Add("NEEDAFFIX", OnNeedAffixKeyword);
			table.Add("ICONV", OnInputConversionKeyword);
			table.Add("REP", OnReplacementKeyword);
			table.Add("AF", OnFlagsVectorKeyword);
			table.Add("KEEPCASE", OnKeepCaseKeyword);
			table.Add("FORBIDDENWORD", OnForbiddenWordKeyword);
			return table;
		}
		StreamReader reader;
		public AffixImporter(HunspellDictionary dictionary, HunspellImportingInfo importingInfo)
			: base(dictionary, importingInfo) {
		}
		protected TranslateKeywordTable KeywordTable { get { return keywordTable; } }
		protected StreamReader Reader { get { return reader; } set { reader = value; } }
		protected override void LoadCore(Stream stream) {
			if (!DetectEncoding(stream)) {
				ParseStream(stream, ParseEncoding, true);
				stream.Seek(0, SeekOrigin.Begin);
				if (ImportingInfo.Encoding == EmptyEncoding.Instance)
					ImportingInfo.Encoding = Encoding.ASCII;
			}
			ParseStream(stream, ParseFlag, true);
			stream.Seek(0, SeekOrigin.Begin);
			ParseStream(stream, ProcessKeyword, false);
		}
		void ParseStream(Stream stream, ProcessKeyValuePair handler, bool breakIfHandle) {
			Reader = new StreamReader(stream, Encoding);
			while (!Reader.EndOfStream) {
				string line = Reader.ReadLine().Trim();
				if (String.IsNullOrEmpty(line) || line[0] == '#')
					continue;
				string keyword;
				string value;
				ParseLine(line, out keyword, out value);
				if (handler(keyword, value) && breakIfHandle)
					break;
			}
		}
		bool ProcessKeyword(string key, string value) {
			TranslateKeywordHandler handler;
			if (KeywordTable.TryGetValue(key, out handler)) {
				handler(this, value);
				return true;
			}
			return false;
		}
		bool ParseEncoding(string key, string value) {
			if (String.Equals("SET", key, StringComparison.Ordinal)) {
				string encodingName = value;
				Regex regex = new Regex("^iso-?(\\S+)$");
				Match match = regex.Match(encodingName.ToLowerInvariant());
				if (match.Success)
					encodingName = String.Format("iso-{0}", match.Groups[1]);
				try {
					Encoding encoding = Encoding.GetEncoding(encodingName);
					ImportingInfo.Encoding = encoding;
				}
				catch {
				}
				return true;
			}
			return false;
		}
		bool ParseFlag(string key, string value) {
			if (String.Equals("FLAG", key, StringComparison.Ordinal)) {
				FlagTypes flagType = FlagTypes.OneChar;
				switch (value) {
					case "long":
						flagType = FlagTypes.TwoChar;
						break;
					case "num":
						flagType = FlagTypes.Number;
						break;
					case "UTF-8":
						flagType = FlagTypes.UTF8;
						break;
					default:
						throw new Exception("Unknown flag type.");
				}
				ImportingInfo.FlagType = flagType;
				return true;
			}
			return false;
		}
		void ParseLine(string line, out string keyword, out string value) {
			int index = GetKeywordEndIndex(line);
			if (index < line.Length) {
				keyword = line.Substring(0, index);
				value = line.Substring(index).Trim();
			}
			else {
				keyword = line;
				value = String.Empty;
			}
		}
		int GetKeywordEndIndex(string line) {
			int count = line.Length;
			for (int index = 0; index < count; index++) {
				if (Char.IsWhiteSpace(line[index]))
					return index;
			}
			return count;
		}
		internal string GetNonEmptyLine() {
			while (!Reader.EndOfStream) {
				string line = Reader.ReadLine();
				if (!String.IsNullOrEmpty(line))
					return line;
			}
			throw new Exception("Unexprected end of the stream.");
		}
		protected internal virtual bool DetectEncoding(Stream stream) {
			EncodingInfo encodingInfo = DetectEncodingByByteOrderMarks(stream);
			if (encodingInfo == null)
				encodingInfo = DetectEncodingByFirstChar(stream);
			if (encodingInfo != null) {
				ImportingInfo.Encoding = encodingInfo.Encoding;
				return true;
			}
			return false;
		}
		EncodingInfo DetectEncodingByByteOrderMarks(Stream stream) {
#if !SL
			if (IsByteOrderMarksDefined(EncodingTable[EncodingType.UTF_32LE], stream))
				return EncodingTable[EncodingType.UTF_32LE];
			if (IsByteOrderMarksDefined(EncodingTable[EncodingType.UTF_32BE], stream))
				return EncodingTable[EncodingType.UTF_32BE];
#endif
			if (IsByteOrderMarksDefined(EncodingTable[EncodingType.UTF_16LE], stream))
				return EncodingTable[EncodingType.UTF_16LE];
			if (IsByteOrderMarksDefined(EncodingTable[EncodingType.UTF_16BE], stream))
				return EncodingTable[EncodingType.UTF_16BE];
			if (IsByteOrderMarksDefined(EncodingTable[EncodingType.UTF_8], stream))
				return EncodingTable[EncodingType.UTF_8];
			return null;
		}
		EncodingInfo DetectEncodingByFirstChar(Stream stream) {
#if !SL
			if (IsByteOrderMaskMatch(EncodingTable[EncodingType.UTF_32LE], stream))
				return EncodingTable[EncodingType.UTF_32LE];
			if (IsByteOrderMaskMatch(EncodingTable[EncodingType.UTF_32BE], stream))
				return EncodingTable[EncodingType.UTF_32BE];
#endif
			if (IsByteOrderMaskMatch(EncodingTable[EncodingType.UTF_16LE], stream))
				return EncodingTable[EncodingType.UTF_16LE];
			if (IsByteOrderMaskMatch(EncodingTable[EncodingType.UTF_16BE], stream))
				return EncodingTable[EncodingType.UTF_16BE];
			return null;
		}
		bool IsByteOrderMarksDefined(EncodingInfo info, Stream stream) {
			byte[] preamble = info.Encoding.GetPreamble();
			if (preamble.Length == 0)
				return false;
			byte[] bytes = new byte[preamble.Length];
			stream.Read(bytes, 0, bytes.Length);
			stream.Seek(0, SeekOrigin.Begin);
			if (bytes.Length < preamble.Length)
				return false;
			for (int i = 0; i < preamble.Length; i++) {
				if (preamble[i] != bytes[i])
					return false;
			}
			stream.Seek(preamble.Length, SeekOrigin.Begin);
			return true;
		}
		bool IsByteOrderMaskMatch(EncodingInfo info, Stream stream) {
			bool[] mask = info.ByteOrderMask;
			int bytesCount = mask.Length;
			byte[] bytes = new byte[bytesCount];
			stream.Read(bytes, 0, bytesCount); 
			stream.Seek(0, SeekOrigin.Begin);
			if (bytes.Length < bytesCount)
				return false;
			for (int i = 0; i < bytesCount; i++) {
				if (mask[i] != (bytes[i] > 0))
					return false;
			}
			return true;
		}
		protected static void OnTryKeyword(AffixImporter importer, string value) {
			importer.Dictionary.SetAlphabetChars(value.ToCharArray());
		}
		protected static void OnCompoundFlagKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Flags.Add(HunspellFlags.Compound, importer.ParseFlag(value));
		}
		protected static void OnPreffixKeyword(AffixImporter importer, string value) {
			Prefix[] preffixes = ParseAffix<Prefix>(value, CreatePreffix, importer);
			foreach (Prefix preffix in preffixes)
				importer.Dictionary.Prefixes.Add(preffix);
		}
		protected static void OnSuffixKeyword(AffixImporter importer, string value) {
			Suffix[] suffixes = ParseAffix<Suffix>(value, CreateSuffix, importer);
			foreach (Suffix suffix in suffixes)
				importer.Dictionary.Suffixes.Add(suffix);
		}
		static Prefix CreatePreffix(HunspellFlag key, bool canCombine) {
			return new Prefix(key, canCombine);
		}
		static Suffix CreateSuffix(HunspellFlag key, bool canCombine) {
			return new Suffix(key, canCombine);
		}
		static T[] ParseAffix<T>(string value, CreateAffix<T> createAffix, AffixImporter importer) where T : AffixEntry {
			string[] tokens = importer.GetTokens(value);
			if (tokens.Length < 3)
				throw new Exception("Invalid affix");
			HunspellFlag key = importer.ParseFlag(tokens[0]);
			bool canCombine = String.Equals(tokens[1], "Y", StringComparison.InvariantCulture);
			int entriesCount;
			if (!Int32.TryParse(tokens[2], out entriesCount))
				entriesCount = 0;
			T[] result = new T[entriesCount];
			for (int i = 0; i < entriesCount; i++) {
				T affix = createAffix(key, canCombine);
				ParseAffixEntry(affix, importer);
				result[i] = affix;
			}
			return result;
		}
		static void ParseAffixEntry<T>(T affix, AffixImporter importer) where T : AffixEntry {
			string line = importer.GetNonEmptyLine();
			if (String.IsNullOrEmpty(line))
				throw new Exception("Unexpected end of the stream.");
			string[] tokens = importer.GetTokens(line);
			int count = tokens.Length;
			for (int tokenIndex = 0; tokenIndex < count; tokenIndex++) {
				string token = tokens[tokenIndex];
				switch (tokenIndex) {
					case 0:
						break;
					case 1:
						HunspellFlag identifier = importer.ParseFlag(token);
						if (!identifier.Equals(affix.Identifier))
							throw new Exception("Invalid key");
						break;
					case 2:
						affix.StringToStrip = (token != "0") ? token : String.Empty;
						break;
					case 3:
						ParseStringToAppend(affix, token, importer);
						break;
					case 4:
						if (token != ".")
							ParseConditionCharacters(token, affix);
						break;
				}
			}
		}
		static void ParseStringToAppend<T>(T affix, string token, AffixImporter importer) where T : AffixEntry {
			string[] parts = token.Split('/');
			affix.StringToAppend = parts[0] != "0" ? parts[0] : String.Empty;
			if (parts.Length < 2)
				return;
			string xtraParameter = parts[1];
			importer.SetFlags(affix, xtraParameter);
		}
		static void ParseConditionCharacters(string token, AffixEntry affix) {
			int count = token.Length;
			for (int index = 0; index < count; index++) {
				char ch = token[index];
				if (ch == '[') {
					int startIndex = index + 1;
					if (startIndex >= count)
						break;
					int endIndex = token.IndexOf(']', startIndex);
					if (endIndex < 0)
						endIndex = count - 1;
					index = endIndex;
					if (startIndex == endIndex)
						continue;
					string charsSet = token.Substring(startIndex, endIndex - startIndex);
					if (charsSet.StartsWith("^"))
						affix.SetCondition(charsSet.TrimStart('^').ToCharArray(), false);
					else
						affix.SetCondition(charsSet.ToCharArray(), true);
				}
				else if (ch == '.')
					affix.SetCondition();
				else
					affix.SetCondition(new char[] { ch }, true);
			}
		}
		protected static void OnCompoundMinKeyword(AffixImporter importer, string value) {
			int num;
			if (Int32.TryParse(value, out num))
				importer.Dictionary.CompoundMin = num;
		}
		protected static void OnCompoundPermitFlagKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Flags.Add(HunspellFlags.CompoundPermit, importer.ParseFlag(value));
		}
		protected static void OnCompoundForbidFlagKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Flags.Add(HunspellFlags.CompoundForbid, importer.ParseFlag(value));
		}
		protected static void OnKeepCaseKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Flags.Add(HunspellFlags.KeepCase, importer.ParseFlag(value));
		}
		protected static void OnForbiddenWordKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Flags.Add(HunspellFlags.ForbiddenWord, importer.ParseFlag(value));
		}
		static void OnKeyKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Keys = value;
		}
		protected static void OnCompoundWordMaxKeyword(AffixImporter importer, string value) {
			int num;
			if (Int32.TryParse(value, out num))
				importer.Dictionary.CompoundWordMax = num;
		}
		protected static void OnCompoundRuleKeyword(AffixImporter importer, string value) {
			int entriesCount;
			if (!Int32.TryParse(value, out entriesCount))
				entriesCount = 0;
			for (int i = 0; i < entriesCount; i++) {
				string[] tokens = importer.GetTokens(importer.GetNonEmptyLine());
				if (tokens.Length < 2 || tokens[0] != "COMPOUNDRULE")
					throw new Exception("Incorrect COMPOUNDRULE definition");
				string rule = tokens[1];
				importer.Dictionary.CompoundRules.Add(new CompoundRule(rule, ParseCompoundRuleElement(importer, rule, 0)));
			}
		}
		protected static CompoundRuleElement ParseCompoundRuleElement(AffixImporter importer, string rule, int charIndex) {
			StringBuilder flag = new StringBuilder();
			int charsCount = rule.Length;
			for (int index = charIndex; index < charsCount; index++) {
				if (rule[index] == '*' && flag.Length > 0)
					return new ZeroOrMoreElement(importer.ParseFlag(flag.ToString()), ParseCompoundRuleElement(importer, rule, ++index));
				if (rule[index] == '?' && flag.Length > 0)
					return new ZeroOrOneElement(importer.ParseFlag(flag.ToString()), ParseCompoundRuleElement(importer, rule, ++index));
				if (flag.Length > 0)
					return new SingleElement(importer.ParseFlag(flag.ToString()), ParseCompoundRuleElement(importer, rule, index));
				if (rule[index] == '(') {
					for (++index; index < charsCount; index++) {
						if (rule[index] == ')')
							break;
						flag.Append(rule[index]);
					}
				}
				else
					flag.Append(rule[index]);
			}
			if (flag.Length > 0)
				return new SingleElement(importer.ParseFlag(flag.ToString()), new FinishElement());
			return new FinishElement();
		}
		protected static void OnOnlyCompoundKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Flags.Add(HunspellFlags.OnlyCompound, importer.ParseFlag(value));
		}
		protected static void OnCompoundPatternKeyword(AffixImporter importer, string value) {
			int entriesCount;
			if (!Int32.TryParse(value, out entriesCount))
				return;
			for (int i = 0; i < entriesCount; i++) {
				string line = importer.GetNonEmptyLine();
				string[] tokens = importer.GetTokens(line);
				if (tokens.Length < 3 || tokens[0] != "CHECKCOMPOUNDPATTERN")
					throw new Exception("Incorrect CHECKCOMPOUNDPATTERN definition");
				CompoundPattern pattern = new CompoundPattern(importer.FlagType);
				ParseFlagedObject(importer, pattern.Ending, tokens[1]);
				ParseFlagedObject(importer, pattern.Begining, tokens[2]);
				importer.Dictionary.CompoundValidationRules.Add(pattern);
				if (tokens.Length >= 4)
					pattern.Replacement = tokens[3];
				importer.Dictionary.CompoundPatterns.Add(pattern);
			}
		}
		static void ParseFlagedObject(AffixImporter importer, FlagedObject flagedObject, string token) {
			string[] parts = token.Split('/');
			Debug.Assert(parts.Length > 0);
			flagedObject.Value = parts[0];
			if (parts.Length > 1) {
				FlagsCollection flags = new FlagsCollection();
				flags.AddRange(importer.ParseFlags(parts[1]));
				flagedObject.Flags = flags;
			}
		}
		protected static void OnCompoundBeginKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Flags.Add(HunspellFlags.CompoundStart, importer.ParseFlag(value));
		}
		protected static void OnCompoundMiddleKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Flags.Add(HunspellFlags.CompoundMiddle, importer.ParseFlag(value));
		}
		protected static void OnCompoundLastKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Flags.Add(HunspellFlags.CompoundEnd, importer.ParseFlag(value));
		}
		protected static void OnCheckCompoundTripleKeyword(AffixImporter importer, string value) {
			importer.Dictionary.CompoundValidationRules.Add(new CheckCompoundTripleValidationRule());
		}
		protected static void OnCheckCompoundDupKeyword(AffixImporter importer, string value) {
			importer.Dictionary.CompoundValidationRules.Add(new CheckCompoundDuplicationValidationRule());
		}
		protected static void OnSimplifiedTripleKeyword(AffixImporter importer, string value) {
			importer.Dictionary.SimplifiedTriple = true;
		}
		protected static void OnNeedAffixKeyword(AffixImporter importer, string value) {
			importer.Dictionary.Flags.Add(HunspellFlags.NeedAffix, importer.ParseFlags(value)[0]);
		}
		protected static void OnInputConversionKeyword(AffixImporter importer, string value) {
			int entriesCount;
			if (!Int32.TryParse(value, out entriesCount))
				return;
			for (int i = 0; i < entriesCount; i++) {
				string[] tokens = importer.GetTokens(importer.GetNonEmptyLine());
				if (tokens.Length < 3 || tokens[0] != "ICONV")
					throw new Exception("Incorrect ICONV definition");
				importer.Dictionary.ConversionTable.Add(tokens[1], tokens[2]);
			}
		}
		protected static void OnReplacementKeyword(AffixImporter importer, string value) {
			int entriesCount;
			if (!Int32.TryParse(value, out entriesCount))
				return;
			for (int i = 0; i < entriesCount; i++) {
				string[] tokens = importer.GetTokens(importer.GetNonEmptyLine());
				if (tokens.Length < 3 || tokens[0] != "REP")
					throw new Exception("Incorrect REP definition");
				importer.Dictionary.ReplacementTable.Add(tokens[1], tokens[2]);
			}
		}
		protected static void OnFlagsVectorKeyword(AffixImporter importer, string value) {
			int entriesCount;
			if (!Int32.TryParse(value, out entriesCount))
				return;
			for (int i = 0; i < entriesCount; i++) {
				string[] tokens = importer.GetTokens(importer.GetNonEmptyLine());
				if (tokens.Length < 2 || tokens[0] != "AF")
					throw new Exception("Incorrect AF definition");
				FlagsCollection flags = new FlagsCollection();
				flags.AddRange(importer.ParseFlags(tokens[1], importer.FlagType, importer.Encoding));
				flags.Sort();
				importer.ImportingInfo.FlagsSetCollection.Add(flags);
			}
		}
	}
	#endregion
	#region DictionaryImporter
	public class DictionaryImporter : HunspellImporter {
		public DictionaryImporter(HunspellDictionary dictionary, HunspellImportingInfo importingInfo)
			: base(dictionary, importingInfo) {
		}
		protected override void LoadCore(Stream stream) {
			StreamReader reader = new StreamReader(stream, Encoding);
			int wordsCount;
			string line = reader.ReadLine();
			if (line != null && Int32.TryParse(GetToken(line), out wordsCount)) {
				Dictionary.SetCapacity(wordsCount);
				line = reader.ReadLine();
			}
			for (; line != null; line = reader.ReadLine()) {
				if (String.IsNullOrEmpty(line) || line[0] == '\t')
					continue;
				string wordEntryDefinition = GetTokens(line)[0];
				string[] parts = SplitFlagsDefinition(wordEntryDefinition);
				WordEntry item = new WordEntry(FlagType);
				item.Value = parts[0];
				if (parts.Length > 1)
					SetFlags(item, parts[1]);
				AddWordEntry(item);
			}
		}
		protected internal virtual string[] SplitFlagsDefinition(string wordEntry) {
			int count = wordEntry.Length;
			StringBuilder wordChars = new StringBuilder();
			for (int charIndex = 0; charIndex < count; charIndex++) {
				char character = wordEntry[charIndex];
				if (character == '/') {
					int nextIndex = charIndex + 1;
					if (nextIndex < count)
						return new string[] { wordChars.ToString(), wordEntry.Substring(nextIndex) };
					else
						return new string[] { wordChars.ToString() };
				}
				else if (character == '"' || character == '\'') {
					char subsetStartChar = character;
					int startIndex = charIndex + 1;
					int length = 0;
					bool isSubsetClosed = false;
					for (int index = startIndex; index < count; index++) {
						if (wordEntry[index] == subsetStartChar) {
							isSubsetClosed = true;
							break;
						}
						length++;
					}
					if (isSubsetClosed) {
						wordChars.Append(wordEntry.Substring(startIndex, length));
						charIndex = startIndex + length;
					}
					else
						wordChars.Append(subsetStartChar);
				}
				else if (character == '\\') {
					charIndex++;
					if (charIndex < count)
						wordChars.Append(wordEntry[charIndex]);
					else
						break;
				}
				else
					wordChars.Append(character);
			}
			return new string[] { wordChars.ToString() };
		}
		protected internal virtual void AddWordEntry(WordEntry item) {
			WordEntry entry;
			if (Dictionary.WordEntries.TryGetValue(item.Value, out entry)) {
				WordEntry parent = entry;
				while (parent.NextEntry != null)
					parent = parent.NextEntry;
				parent.NextEntry = item;
			}
			else
				Dictionary.WordEntries.Add(item.Value, item);
		}
	}
	#endregion
}
