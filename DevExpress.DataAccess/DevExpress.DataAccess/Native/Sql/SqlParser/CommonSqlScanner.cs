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
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using DevExpress.CodeParser;
namespace DevExpress.DataAccess.Native.Sql.SqlParser {
	[CLSCompliant(false)]
	public partial class CommonSqlScanner : GeneratedScannerBase {
		const int charSetSize = 272;
		const int UnicodeCharIndex = 257;
		const int UnicodeLetterIndex = 258;
		const int maxT = 205;
		const int noSym = 205;
		short[] start = {
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 46, 0, 0, 0, 0, 45, 55, 56, 0, 0, 62, 49, 66, 51,
			65, 64, 64, 64, 64, 64, 64, 64, 64, 64, 0, 63, 60, 61, 59, 0,
			1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 48, 16, 0, 0, 2,
			67, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 57, 0, 58, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 2,
			0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			-1
		};
		public CommonSqlScanner(ISourceReader s) {
			Initialize(s);
		}
		protected override int GetUnicodeLetterIndex() {
			return UnicodeLetterIndex;
		}
		protected override int GetNextState(int input) {
			return start[input];
		}
		protected override void InitializeIgnoreTable() {
			ignore = new BitArray(charSetSize + 1);
			ignore[' '] = true; 
			ignore[9] = true;
			ignore[10] = true;
			ignore[13] = true;
		}
		protected override void NextChCasing() {
		}
		protected override void AddCh() {
			base.AddCh();
			tval[tlen ++] = ch;
			NextCh();
		}
		void CheckLiteral() {
			switch(t.Value) {
				case "count":
					t.Type = 17;
					break;
				case "max":
					t.Type = 18;
					break;
				case "min":
					t.Type = 19;
					break;
				case "avg":
					t.Type = 20;
					break;
				case "sum":
					t.Type = 21;
					break;
				case "add":
					t.Type = 25;
					break;
				case "all":
					t.Type = 26;
					break;
				case "alter":
					t.Type = 27;
					break;
				case "and":
					t.Type = 28;
					break;
				case "any":
					t.Type = 29;
					break;
				case "as":
					t.Type = 30;
					break;
				case "asc":
					t.Type = 31;
					break;
				case "authorization":
					t.Type = 32;
					break;
				case "backup":
					t.Type = 33;
					break;
				case "begin":
					t.Type = 34;
					break;
				case "between":
					t.Type = 35;
					break;
				case "break":
					t.Type = 36;
					break;
				case "browse":
					t.Type = 37;
					break;
				case "bulk":
					t.Type = 38;
					break;
				case "by":
					t.Type = 39;
					break;
				case "cascade":
					t.Type = 40;
					break;
				case "case":
					t.Type = 41;
					break;
				case "check":
					t.Type = 42;
					break;
				case "checkpoint":
					t.Type = 43;
					break;
				case "close":
					t.Type = 44;
					break;
				case "clustered":
					t.Type = 45;
					break;
				case "coalesce":
					t.Type = 46;
					break;
				case "collate":
					t.Type = 47;
					break;
				case "column":
					t.Type = 48;
					break;
				case "commit":
					t.Type = 49;
					break;
				case "compute":
					t.Type = 50;
					break;
				case "constraint":
					t.Type = 51;
					break;
				case "contains":
					t.Type = 52;
					break;
				case "containstable":
					t.Type = 53;
					break;
				case "continue":
					t.Type = 54;
					break;
				case "convert":
					t.Type = 55;
					break;
				case "create":
					t.Type = 56;
					break;
				case "cross":
					t.Type = 57;
					break;
				case "current":
					t.Type = 58;
					break;
				case "current_date":
					t.Type = 59;
					break;
				case "current_time":
					t.Type = 60;
					break;
				case "current_timestamp":
					t.Type = 61;
					break;
				case "current_user":
					t.Type = 62;
					break;
				case "cursor":
					t.Type = 63;
					break;
				case "database":
					t.Type = 64;
					break;
				case "dbcc":
					t.Type = 65;
					break;
				case "deallocate":
					t.Type = 66;
					break;
				case "declare":
					t.Type = 67;
					break;
				case "default":
					t.Type = 68;
					break;
				case "delete":
					t.Type = 69;
					break;
				case "deny":
					t.Type = 70;
					break;
				case "desc":
					t.Type = 71;
					break;
				case "disk":
					t.Type = 72;
					break;
				case "distinct":
					t.Type = 73;
					break;
				case "distributed":
					t.Type = 74;
					break;
				case "double":
					t.Type = 75;
					break;
				case "drop":
					t.Type = 76;
					break;
				case "dump":
					t.Type = 77;
					break;
				case "else":
					t.Type = 78;
					break;
				case "end":
					t.Type = 79;
					break;
				case "errlvl":
					t.Type = 80;
					break;
				case "escape":
					t.Type = 81;
					break;
				case "except":
					t.Type = 82;
					break;
				case "exec":
					t.Type = 83;
					break;
				case "execute":
					t.Type = 84;
					break;
				case "exists":
					t.Type = 85;
					break;
				case "exit":
					t.Type = 86;
					break;
				case "external":
					t.Type = 87;
					break;
				case "fetch":
					t.Type = 88;
					break;
				case "file":
					t.Type = 89;
					break;
				case "fillfactor":
					t.Type = 90;
					break;
				case "for":
					t.Type = 91;
					break;
				case "foreign":
					t.Type = 92;
					break;
				case "freetext":
					t.Type = 93;
					break;
				case "freetexttable":
					t.Type = 94;
					break;
				case "from":
					t.Type = 95;
					break;
				case "full":
					t.Type = 96;
					break;
				case "function":
					t.Type = 97;
					break;
				case "goto":
					t.Type = 98;
					break;
				case "grant":
					t.Type = 99;
					break;
				case "group":
					t.Type = 100;
					break;
				case "having":
					t.Type = 101;
					break;
				case "holdlock":
					t.Type = 102;
					break;
				case "identity":
					t.Type = 103;
					break;
				case "identity_insert":
					t.Type = 104;
					break;
				case "identitycol":
					t.Type = 105;
					break;
				case "if":
					t.Type = 106;
					break;
				case "in":
					t.Type = 107;
					break;
				case "index":
					t.Type = 108;
					break;
				case "inner":
					t.Type = 109;
					break;
				case "insert":
					t.Type = 110;
					break;
				case "intersect":
					t.Type = 111;
					break;
				case "into":
					t.Type = 112;
					break;
				case "is":
					t.Type = 113;
					break;
				case "join":
					t.Type = 114;
					break;
				case "key":
					t.Type = 115;
					break;
				case "kill":
					t.Type = 116;
					break;
				case "left":
					t.Type = 117;
					break;
				case "like":
					t.Type = 118;
					break;
				case "lineno":
					t.Type = 119;
					break;
				case "load":
					t.Type = 120;
					break;
				case "merge":
					t.Type = 121;
					break;
				case "national":
					t.Type = 122;
					break;
				case "nocheck":
					t.Type = 123;
					break;
				case "nonclustered":
					t.Type = 124;
					break;
				case "not":
					t.Type = 125;
					break;
				case "null":
					t.Type = 126;
					break;
				case "nullif":
					t.Type = 127;
					break;
				case "of":
					t.Type = 128;
					break;
				case "off":
					t.Type = 129;
					break;
				case "offsets":
					t.Type = 130;
					break;
				case "on":
					t.Type = 131;
					break;
				case "open":
					t.Type = 132;
					break;
				case "opendatasource":
					t.Type = 133;
					break;
				case "openquery":
					t.Type = 134;
					break;
				case "openrowset":
					t.Type = 135;
					break;
				case "openxml":
					t.Type = 136;
					break;
				case "option":
					t.Type = 137;
					break;
				case "or":
					t.Type = 138;
					break;
				case "order":
					t.Type = 139;
					break;
				case "outer":
					t.Type = 140;
					break;
				case "over":
					t.Type = 141;
					break;
				case "percent":
					t.Type = 142;
					break;
				case "pivot":
					t.Type = 143;
					break;
				case "plan":
					t.Type = 144;
					break;
				case "precision":
					t.Type = 145;
					break;
				case "primary":
					t.Type = 146;
					break;
				case "print":
					t.Type = 147;
					break;
				case "proc":
					t.Type = 148;
					break;
				case "procedure":
					t.Type = 149;
					break;
				case "public":
					t.Type = 150;
					break;
				case "raiserror":
					t.Type = 151;
					break;
				case "read":
					t.Type = 152;
					break;
				case "readtext":
					t.Type = 153;
					break;
				case "reconfigure":
					t.Type = 154;
					break;
				case "references":
					t.Type = 155;
					break;
				case "replication":
					t.Type = 156;
					break;
				case "restore":
					t.Type = 157;
					break;
				case "restrict":
					t.Type = 158;
					break;
				case "return":
					t.Type = 159;
					break;
				case "revert":
					t.Type = 160;
					break;
				case "revoke":
					t.Type = 161;
					break;
				case "right":
					t.Type = 162;
					break;
				case "rollback":
					t.Type = 163;
					break;
				case "rowcount":
					t.Type = 164;
					break;
				case "rowguidcol":
					t.Type = 165;
					break;
				case "rule":
					t.Type = 166;
					break;
				case "save":
					t.Type = 167;
					break;
				case "schema":
					t.Type = 168;
					break;
				case "securityaudit":
					t.Type = 169;
					break;
				case "select":
					t.Type = 170;
					break;
				case "session_user":
					t.Type = 171;
					break;
				case "set":
					t.Type = 172;
					break;
				case "setuser":
					t.Type = 173;
					break;
				case "shutdown":
					t.Type = 174;
					break;
				case "some":
					t.Type = 175;
					break;
				case "statistics":
					t.Type = 176;
					break;
				case "system_user":
					t.Type = 177;
					break;
				case "table":
					t.Type = 178;
					break;
				case "tablesample":
					t.Type = 179;
					break;
				case "textsize":
					t.Type = 180;
					break;
				case "then":
					t.Type = 181;
					break;
				case "to":
					t.Type = 182;
					break;
				case "top":
					t.Type = 183;
					break;
				case "tran":
					t.Type = 184;
					break;
				case "transaction":
					t.Type = 185;
					break;
				case "trigger":
					t.Type = 186;
					break;
				case "truncate":
					t.Type = 187;
					break;
				case "tsequal":
					t.Type = 188;
					break;
				case "union":
					t.Type = 189;
					break;
				case "unique":
					t.Type = 190;
					break;
				case "unpivot":
					t.Type = 191;
					break;
				case "update":
					t.Type = 192;
					break;
				case "updatetext":
					t.Type = 193;
					break;
				case "use":
					t.Type = 194;
					break;
				case "user":
					t.Type = 195;
					break;
				case "values":
					t.Type = 196;
					break;
				case "varying":
					t.Type = 197;
					break;
				case "view":
					t.Type = 198;
					break;
				case "waitfor":
					t.Type = 199;
					break;
				case "when":
					t.Type = 200;
					break;
				case "where":
					t.Type = 201;
					break;
				case "while":
					t.Type = 202;
					break;
				case "with":
					t.Type = 203;
					break;
				case "writetext":
					t.Type = 204;
					break;
				default:
					break;
			}
		}
		protected override void NextTokenComments() {
		}
		protected override void NextTokenScan(int state) {
			AddCh();
			switch(state) {
				case -1: {
					t.Type = eofSym;
					break;
				} 
				case 0: {
					t.Type = noSym;
					break;
				} 
				case 1:
					if((ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 170 || ch == 181 || ch == 186 || ch >= 192 && ch <= 214 || ch >= 216 && ch <= 246 || ch >= 248 && ch <= 255 || ch == 258) || IsUnicodeLetter(ch)) {
						AddCh();
						goto case 2;
					} else if(ch == 92) {
						AddCh();
						goto case 16;
					} else {
						goto case 0;
					}
				case 2:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 170 || ch == 181 || ch == 186 || ch >= 192 && ch <= 214 || ch >= 216 && ch <= 246 || ch >= 248 && ch <= 255 || ch == 258) || IsUnicodeLetter(ch)) {
						AddCh();
						goto case 2;
					} else if(ch == 92) {
						AddCh();
						goto case 3;
					} else {
						t.Type = 1;
						t.Value = new String(tval, 0, tlen);
						CheckLiteral();
						break;
					}
				case 3:
					if(ch == 'u') {
						AddCh();
						goto case 4;
					} else if(ch == 'U') {
						AddCh();
						goto case 8;
					} else {
						goto case 0;
					}
				case 4:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 5;
					} else {
						goto case 0;
					}
				case 5:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 6;
					} else {
						goto case 0;
					}
				case 6:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 7;
					} else {
						goto case 0;
					}
				case 7:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 2;
					} else {
						goto case 0;
					}
				case 8:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 9;
					} else {
						goto case 0;
					}
				case 9:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 10;
					} else {
						goto case 0;
					}
				case 10:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 11;
					} else {
						goto case 0;
					}
				case 11:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 12;
					} else {
						goto case 0;
					}
				case 12:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 13;
					} else {
						goto case 0;
					}
				case 13:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 14;
					} else {
						goto case 0;
					}
				case 14:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 15;
					} else {
						goto case 0;
					}
				case 15:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 2;
					} else {
						goto case 0;
					}
				case 16:
					if(ch == 'u') {
						AddCh();
						goto case 17;
					} else if(ch == 'U') {
						AddCh();
						goto case 21;
					} else {
						goto case 0;
					}
				case 17:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 18;
					} else {
						goto case 0;
					}
				case 18:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 19;
					} else {
						goto case 0;
					}
				case 19:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 20;
					} else {
						goto case 0;
					}
				case 20:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 2;
					} else {
						goto case 0;
					}
				case 21:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 22;
					} else {
						goto case 0;
					}
				case 22:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 23;
					} else {
						goto case 0;
					}
				case 23:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 24;
					} else {
						goto case 0;
					}
				case 24:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 25;
					} else {
						goto case 0;
					}
				case 25:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 26;
					} else {
						goto case 0;
					}
				case 26:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 27;
					} else {
						goto case 0;
					}
				case 27:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 28;
					} else {
						goto case 0;
					}
				case 28:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 2;
					} else {
						goto case 0;
					}
				case 29:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 30;
					} else {
						goto case 0;
					}
				case 30:
					if((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f')) {
						AddCh();
						goto case 30;
					} else if(ch == 'U') {
						AddCh();
						goto case 72;
					} else if(ch == 'u') {
						AddCh();
						goto case 73;
					} else if(ch == 'L') {
						AddCh();
						goto case 74;
					} else if(ch == 'l') {
						AddCh();
						goto case 75;
					} else {
						t.Type = 2;
						break;
					}
				case 31: {
					t.Type = 2;
					break;
				}
				case 32:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 32;
					} else if((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm')) {
						AddCh();
						goto case 44;
					} else if((ch == 'E' || ch == 'e')) {
						AddCh();
						goto case 33;
					} else {
						t.Type = 3;
						break;
					}
				case 33:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 35;
					} else if((ch == '+' || ch == '-')) {
						AddCh();
						goto case 34;
					} else {
						goto case 0;
					}
				case 34:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 35;
					} else {
						goto case 0;
					}
				case 35:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 35;
					} else if((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm')) {
						AddCh();
						goto case 44;
					} else {
						t.Type = 3;
						break;
					}
				case 36:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 37;
					} else {
						goto case 0;
					}
				case 37:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 37;
					} else if((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm')) {
						AddCh();
						goto case 44;
					} else if((ch == 'E' || ch == 'e')) {
						AddCh();
						goto case 38;
					} else {
						t.Type = 3;
						break;
					}
				case 38:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 40;
					} else if((ch == '+' || ch == '-')) {
						AddCh();
						goto case 39;
					} else {
						goto case 0;
					}
				case 39:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 40;
					} else {
						goto case 0;
					}
				case 40:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 40;
					} else if((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm')) {
						AddCh();
						goto case 44;
					} else {
						t.Type = 3;
						break;
					}
				case 41:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 43;
					} else if((ch == '+' || ch == '-')) {
						AddCh();
						goto case 42;
					} else {
						goto case 0;
					}
				case 42:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 43;
					} else {
						goto case 0;
					}
				case 43:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 43;
					} else if((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm')) {
						AddCh();
						goto case 44;
					} else {
						t.Type = 3;
						break;
					}
				case 44: {
					t.Type = 3;
					break;
				}
				case 45:
					if(!(ch == 39) && ch != EOF) {
						AddCh();
						goto case 45;
					} else if(ch == 39) {
						AddCh();
						goto case 76;
					} else {
						goto case 0;
					}
				case 46:
					if(!(ch == '"') && ch != EOF) {
						AddCh();
						goto case 46;
					} else if(ch == '"') {
						AddCh();
						goto case 77;
					} else {
						goto case 0;
					}
				case 47:
					if(!(ch == '`') && ch != EOF) {
						AddCh();
						goto case 47;
					} else if(ch == '`') {
						AddCh();
						goto case 78;
					} else {
						goto case 0;
					}
				case 48:
					if((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 'Z' || ch == 92 || ch >= '^' && ch <= 271) || (ch > 255 && ch != EOF)) {
						AddCh();
						goto case 48;
					} else if(ch == ']') {
						AddCh();
						goto case 79;
					} else if(ch == '[') {
						AddCh();
						goto case 80;
					} else {
						goto case 0;
					}
				case 49:
					if((ch == '-')) {
						AddCh();
						goto case 50;
					} else {
						goto case 0;
					}
				case 50:
					if((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) || (ch > 255 && ch != EOF)) {
						AddCh();
						goto case 50;
					} else {
						t.Type = 8;
						break;
					}
				case 51:
					if(ch == '*') {
						AddCh();
						goto case 52;
					} else {
						goto case 0;
					}
				case 52:
					if((ch <= ')' || ch >= '+' && ch <= '.' || ch >= '0' && ch <= 271) || (ch > 255 && ch != EOF)) {
						AddCh();
						goto case 52;
					} else if(ch == '*') {
						AddCh();
						goto case 53;
					} else {
						goto case 0;
					}
				case 53:
					if(ch == '/') {
						AddCh();
						goto case 54;
					} else {
						goto case 0;
					}
				case 54: {
					t.Type = 8;
					break;
				}
				case 55: {
					t.Type = 9;
					break;
				}
				case 56: {
					t.Type = 10;
					break;
				}
				case 57: {
					t.Type = 11;
					break;
				}
				case 58: {
					t.Type = 12;
					break;
				}
				case 59: {
					t.Type = 15;
					break;
				}
				case 60: {
					t.Type = 16;
					break;
				}
				case 61: {
					t.Type = 22;
					break;
				}
				case 62: {
					t.Type = 23;
					break;
				}
				case 63: {
					t.Type = 24;
					break;
				}
				case 64:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 64;
					} else if(ch == 'U') {
						AddCh();
						goto case 68;
					} else if(ch == 'u') {
						AddCh();
						goto case 69;
					} else if(ch == 'L') {
						AddCh();
						goto case 70;
					} else if(ch == 'l') {
						AddCh();
						goto case 71;
					} else if(ch == '.') {
						AddCh();
						goto case 36;
					} else if((ch == 'E' || ch == 'e')) {
						AddCh();
						goto case 41;
					} else if((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm')) {
						AddCh();
						goto case 44;
					} else {
						t.Type = 2;
						break;
					}
				case 65:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 64;
					} else if(ch == 'U') {
						AddCh();
						goto case 68;
					} else if(ch == 'u') {
						AddCh();
						goto case 69;
					} else if(ch == 'L') {
						AddCh();
						goto case 70;
					} else if(ch == 'l') {
						AddCh();
						goto case 71;
					} else if((ch == 'X' || ch == 'x')) {
						AddCh();
						goto case 29;
					} else if(ch == '.') {
						AddCh();
						goto case 36;
					} else if((ch == 'E' || ch == 'e')) {
						AddCh();
						goto case 41;
					} else if((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm')) {
						AddCh();
						goto case 44;
					} else {
						t.Type = 2;
						break;
					}
				case 66:
					if((ch >= '0' && ch <= '9')) {
						AddCh();
						goto case 32;
					} else {
						t.Type = 13;
						break;
					}
				case 67:
					if(!(ch == '`') && ch != EOF) {
						AddCh();
						goto case 47;
					} else if(ch == '`') {
						AddCh();
						goto case 78;
					} else {
						t.Type = 14;
						break;
					}
				case 68:
					if((ch == 'L' || ch == 'l')) {
						AddCh();
						goto case 31;
					} else {
						t.Type = 2;
						break;
					}
				case 69:
					if((ch == 'L' || ch == 'l')) {
						AddCh();
						goto case 31;
					} else {
						t.Type = 2;
						break;
					}
				case 70:
					if((ch == 'U' || ch == 'u')) {
						AddCh();
						goto case 31;
					} else {
						t.Type = 2;
						break;
					}
				case 71:
					if((ch == 'U' || ch == 'u')) {
						AddCh();
						goto case 31;
					} else {
						t.Type = 2;
						break;
					}
				case 72:
					if((ch == 'L' || ch == 'l')) {
						AddCh();
						goto case 31;
					} else {
						t.Type = 2;
						break;
					}
				case 73:
					if((ch == 'L' || ch == 'l')) {
						AddCh();
						goto case 31;
					} else {
						t.Type = 2;
						break;
					}
				case 74:
					if((ch == 'U' || ch == 'u')) {
						AddCh();
						goto case 31;
					} else {
						t.Type = 2;
						break;
					}
				case 75:
					if((ch == 'U' || ch == 'u')) {
						AddCh();
						goto case 31;
					} else {
						t.Type = 2;
						break;
					}
				case 76:
					if(ch == 39) {
						AddCh();
						goto case 45;
					} else {
						t.Type = 4;
						break;
					}
				case 77:
					if(ch == '"') {
						AddCh();
						goto case 46;
					} else {
						t.Type = 5;
						break;
					}
				case 78:
					if(ch == '`') {
						AddCh();
						goto case 47;
					} else {
						t.Type = 6;
						break;
					}
				case 79:
					if(ch == ']') {
						AddCh();
						goto case 48;
					} else {
						t.Type = 7;
						break;
					}
				case 80:
					if((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 'Z' || ch == 92 || ch >= '^' && ch <= 271) || (ch > 255 && ch != EOF)) {
						AddCh();
						goto case 48;
					} else if(ch == ']') {
						AddCh();
						goto case 79;
					} else if(ch == '[') {
						AddCh();
						goto case 80;
					} else {
						goto case 0;
					}
			}
		}
	} 
}
