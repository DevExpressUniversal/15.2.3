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
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using DevExpress.CodeParser;
namespace DevExpress.DataAccess.Native.Sql.SqlParser
{
	public static class Tokens
	{
		public const int DISTRIBUTED = 74;
		public const int TOP = 183;
		public const int REPLICATION = 156;
		public const int CURRENTUSER = 62;
		public const int COMPUTE = 50;
		public const int BY = 39;
		public const int LIKE = 118;
		public const int TEXTSIZE = 180;
		public const int ERRLVL = 80;
		public const int CASE = 41;
		public const int RBRACE = 12;
		public const int PRIMARY = 146;
		public const int ENDKW = 79;
		public const int PLAN = 144;
		public const int INTERSECT = 111;
		public const int BEGIN = 34;
		public const int SECURITYAUDIT = 169;
		public const int NONCLUSTERED = 124;
		public const int ALL = 26;
		public const int SQUAREBRACKETSTRING = 7;
		public const int LEFT = 117;
		public const int AUTHORIZATION = 32;
		public const int TRAN = 184;
		public const int LBRACE = 11;
		public const int MIN = 19;
		public const int REVOKE = 161;
		public const int CREATE = 56;
		public const int RETURN = 159;
		public const int SELECT = 170;
		public const int REVERT = 160;
		public const int SAVE = 167;
		public const int FREETEXT = 93;
		public const int OUTER = 140;
		public const int UNION = 189;
		public const int CURRENTDATE = 59;
		public const int WHERE = 201;
		public const int PROC = 148;
		public const int FETCH = 88;
		public const int AVG = 20;
		public const int RECONFIGURE = 154;
		public const int VARYING = 197;
		public const int REFERENCES = 155;
		public const int IN = 107;
		public const int IDENT = 1;
		public const int ON = 131;
		public const int GREATER = 15;
		public const int TABLE = 178;
		public const int COMMA = 23;
		public const int EXCEPT = 82;
		public const int BACKUP = 33;
		public const int WHEN = 200;
		public const int HAVING = 101;
		public const int DISTINCT = 73;
		public const int CROSS = 57;
		public const int THEN = 181;
		public const int OPENROWSET = 135;
		public const int CONTAINSTABLE = 53;
		public const int OFFSETS = 130;
		public const int FREETEXTTABLE = 94;
		public const int SCHEMA = 168;
		public const int ROWCOUNT = 164;
		public const int ASC = 31;
		public const int TRIGGER = 186;
		public const int COMMENT = 8;
		public const int WHILE = 202;
		public const int VIEW = 198;
		public const int OFF = 129;
		public const int FUNCTION = 97;
		public const int ESCAPE = 81;
		public const int DUMP = 77;
		public const int ROWGUIDCOL = 165;
		public const int IDENTITYINSERT = 104;
		public const int DELETE = 69;
		public const int CLUSTERED = 45;
		public const int COLUMN = 48;
		public const int WAITFOR = 199;
		public const int GRAVIS = 14;
		public const int CURSOR = 63;
		public const int CONVERT = 55;
		public const int NULL = 126;
		public const int IFKW = 106;
		public const int HOLDLOCK = 102;
		public const int GRAVISSTRING = 6;
		public const int CONSTRAINT = 51;
		public const int WRITETEXT = 204;
		public const int CHECKPOINT = 43;
		public const int CHECK = 42;
		public const int GROUP = 100;
		public const int DESC = 71;
		public const int EXECUTE = 84;
		public const int CURRENTTIMESTAMP = 61;
		public const int FILLFACTOR = 90;
		public const int DOT = 13;
		public const int SUM = 21;
		public const int BREAK = 36;
		public const int USE = 194;
		public const int LINENO = 119;
		public const int USER = 195;
		public const int STATISTICS = 176;
		public const int KILL = 116;
		public const int DOUBLESTRING = 5;
		public const int SYSTEMUSER = 177;
		public const int ANYKW = 29;
		public const int IDENTITYCOL = 105;
		public const int BROWSE = 37;
		public const int WITH = 203;
		public const int NATIONAL = 122;
		public const int NULLIF = 127;
		public const int OPENXML = 136;
		public const int COMMIT = 49;
		public const int TRANSACTION = 185;
		public const int ORDER = 139;
		public const int OR = 138;
		public const int RAISERROR = 151;
		public const int DEFAULT = 68;
		public const int SOME = 175;
		public const int CASCADE = 40;
		public const int PRINT = 147;
		public const int RULE = 166;
		public const int RESTRICT = 158;
		public const int NOCHECK = 123;
		public const int CLOSE = 44;
		public const int IS = 113;
		public const int CONTINUE = 54;
		public const int IDENTITY = 103;
		public const int UPDATETEXT = 193;
		public const int AS = 30;
		public const int COUNT = 17;
		public const int SINGLESTRING = 4;
		public const int DATABASE = 64;
		public const int FULL = 96;
		public const int SEMICOLON = 24;
		public const int SESSIONUSER = 171;
		public const int FOR = 91;
		public const int OF = 128;
		public const int TRUNCATE = 187;
		public const int OVER = 141;
		public const int AND = 28;
		public const int RESTORE = 157;
		public const int DROP = 76;
		public const int VALUES = 196;
		public const int INTCON = 2;
		public const int FROMKW = 95;
		public const int EXTERNAL = 87;
		public const int UPDATE = 192;
		public const int INTO = 112;
		public const int MERGE = 121;
		public const int INDEX = 108;
		public const int SHUTDOWN = 174;
		public const int ROLLBACK = 163;
		public const int TABLESAMPLE = 179;
		public const int INSERT = 110;
		public const int KEY = 115;
		public const int MAX = 18;
		public const int SET = 172;
		public const int ELSE = 78;
		public const int OPTION = 137;
		public const int OPENQUERY = 134;
		public const int GOTO = 98;
		public const int READTEXT = 153;
		public const int BETWEEN = 35;
		public const int DBCC = 65;
		public const int REALCON = 3;
		public const int NOT = 125;
		public const int JOIN = 114;
		public const int FILE = 89;
		public const int OPEN = 132;
		public const int GRANT = 99;
		public const int CURRENT = 58;
		public const int UNIQUE = 190;
		public const int TSEQUAL = 188;
		public const int LOAD = 120;
		public const int ALTER = 27;
		public const int DEALLOCATE = 66;
		public const int DECLARE = 67;
		public const int SETUSER = 173;
		public const int PROCEDURE = 149;
		public const int EXEC = 83;
		public const int PIVOT = 143;
		public const int DOUBLE = 75;
		public const int CURRENTTIME = 60;
		public const int RPAR = 10;
		public const int UNPIVOT = 191;
		public const int LESS = 16;
		public const int COALESCE = 46;
		public const int EOF = 0;
		public const int EXIT = 86;
		public const int PRECISION = 145;
		public const int LPAR = 9;
		public const int PERCENT = 142;
		public const int COLLATE = 47;
		public const int DENY = 70;
		public const int RIGHT = 162;
		public const int BULK = 38;
		public const int PUBLIC = 150;
		public const int CONTAINS = 52;
		public const int FOREIGN = 92;
		public const int OPENDATASOURCE = 133;
		public const int DISK = 72;
		public const int EQUAL = 22;
		public const int INNER = 109;
		public const int EXISTS = 85;
		public const int READ = 152;
		public const int TOKW = 182;
		public const int ADD = 25;
		public const int MaxTokens = 205;
		public static int[] Keywords = {
		};
	}
	#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812")]
#endif
	partial class CommonSqlParser : GeneratedParserBase
	{
	public override string Language
	{
	  get { return ""; }
	}
		protected override void HandlePragmas()
		{
		}		
			void Parser()
	{
		Expect(Tokens.SELECT );
	}
		void Parse()
		{
				Parser();
		}
		protected override bool[,] CreateSetArray()
		{
			bool[,] set =
			{
			{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}
			};
			return set;
		}
	}	
	[CLSCompliant(false)]
	public class CSharpErrors : ParserErrorsBase
	{
		protected override string GetSyntaxErrorText(int n)
		{
			string s;
			switch (n)
			{
				case 0: s = "EOF expected"; break;
			case 1: s = "IDENT expected"; break;
			case 2: s = "INTCON expected"; break;
			case 3: s = "REALCON expected"; break;
			case 4: s = "SINGLESTRING expected"; break;
			case 5: s = "DOUBLESTRING expected"; break;
			case 6: s = "GRAVISSTRING expected"; break;
			case 7: s = "SQUAREBRACKETSTRING expected"; break;
			case 8: s = "COMMENT expected"; break;
			case 9: s = "LPAR expected"; break;
			case 10: s = "RPAR expected"; break;
			case 11: s = "LBRACE expected"; break;
			case 12: s = "RBRACE expected"; break;
			case 13: s = "DOT expected"; break;
			case 14: s = "GRAVIS expected"; break;
			case 15: s = "GREATER expected"; break;
			case 16: s = "LESS expected"; break;
			case 17: s = "COUNT expected"; break;
			case 18: s = "MAX expected"; break;
			case 19: s = "MIN expected"; break;
			case 20: s = "AVG expected"; break;
			case 21: s = "SUM expected"; break;
			case 22: s = "EQUAL expected"; break;
			case 23: s = "COMMA expected"; break;
			case 24: s = "SEMICOLON expected"; break;
			case 25: s = "ADD expected"; break;
			case 26: s = "ALL expected"; break;
			case 27: s = "ALTER expected"; break;
			case 28: s = "AND expected"; break;
			case 29: s = "ANYKW expected"; break;
			case 30: s = "AS expected"; break;
			case 31: s = "ASC expected"; break;
			case 32: s = "AUTHORIZATION expected"; break;
			case 33: s = "BACKUP expected"; break;
			case 34: s = "BEGIN expected"; break;
			case 35: s = "BETWEEN expected"; break;
			case 36: s = "BREAK expected"; break;
			case 37: s = "BROWSE expected"; break;
			case 38: s = "BULK expected"; break;
			case 39: s = "BY expected"; break;
			case 40: s = "CASCADE expected"; break;
			case 41: s = "CASE expected"; break;
			case 42: s = "CHECK expected"; break;
			case 43: s = "CHECKPOINT expected"; break;
			case 44: s = "CLOSE expected"; break;
			case 45: s = "CLUSTERED expected"; break;
			case 46: s = "COALESCE expected"; break;
			case 47: s = "COLLATE expected"; break;
			case 48: s = "COLUMN expected"; break;
			case 49: s = "COMMIT expected"; break;
			case 50: s = "COMPUTE expected"; break;
			case 51: s = "CONSTRAINT expected"; break;
			case 52: s = "CONTAINS expected"; break;
			case 53: s = "CONTAINSTABLE expected"; break;
			case 54: s = "CONTINUE expected"; break;
			case 55: s = "CONVERT expected"; break;
			case 56: s = "CREATE expected"; break;
			case 57: s = "CROSS expected"; break;
			case 58: s = "CURRENT expected"; break;
			case 59: s = "CURRENTDATE expected"; break;
			case 60: s = "CURRENTTIME expected"; break;
			case 61: s = "CURRENTTIMESTAMP expected"; break;
			case 62: s = "CURRENTUSER expected"; break;
			case 63: s = "CURSOR expected"; break;
			case 64: s = "DATABASE expected"; break;
			case 65: s = "DBCC expected"; break;
			case 66: s = "DEALLOCATE expected"; break;
			case 67: s = "DECLARE expected"; break;
			case 68: s = "DEFAULT expected"; break;
			case 69: s = "DELETE expected"; break;
			case 70: s = "DENY expected"; break;
			case 71: s = "DESC expected"; break;
			case 72: s = "DISK expected"; break;
			case 73: s = "DISTINCT expected"; break;
			case 74: s = "DISTRIBUTED expected"; break;
			case 75: s = "DOUBLE expected"; break;
			case 76: s = "DROP expected"; break;
			case 77: s = "DUMP expected"; break;
			case 78: s = "ELSE expected"; break;
			case 79: s = "ENDKW expected"; break;
			case 80: s = "ERRLVL expected"; break;
			case 81: s = "ESCAPE expected"; break;
			case 82: s = "EXCEPT expected"; break;
			case 83: s = "EXEC expected"; break;
			case 84: s = "EXECUTE expected"; break;
			case 85: s = "EXISTS expected"; break;
			case 86: s = "EXIT expected"; break;
			case 87: s = "EXTERNAL expected"; break;
			case 88: s = "FETCH expected"; break;
			case 89: s = "FILE expected"; break;
			case 90: s = "FILLFACTOR expected"; break;
			case 91: s = "FOR expected"; break;
			case 92: s = "FOREIGN expected"; break;
			case 93: s = "FREETEXT expected"; break;
			case 94: s = "FREETEXTTABLE expected"; break;
			case 95: s = "FROMKW expected"; break;
			case 96: s = "FULL expected"; break;
			case 97: s = "FUNCTION expected"; break;
			case 98: s = "GOTO expected"; break;
			case 99: s = "GRANT expected"; break;
			case 100: s = "GROUP expected"; break;
			case 101: s = "HAVING expected"; break;
			case 102: s = "HOLDLOCK expected"; break;
			case 103: s = "IDENTITY expected"; break;
			case 104: s = "IDENTITYINSERT expected"; break;
			case 105: s = "IDENTITYCOL expected"; break;
			case 106: s = "IFKW expected"; break;
			case 107: s = "IN expected"; break;
			case 108: s = "INDEX expected"; break;
			case 109: s = "INNER expected"; break;
			case 110: s = "INSERT expected"; break;
			case 111: s = "INTERSECT expected"; break;
			case 112: s = "INTO expected"; break;
			case 113: s = "IS expected"; break;
			case 114: s = "JOIN expected"; break;
			case 115: s = "KEY expected"; break;
			case 116: s = "KILL expected"; break;
			case 117: s = "LEFT expected"; break;
			case 118: s = "LIKE expected"; break;
			case 119: s = "LINENO expected"; break;
			case 120: s = "LOAD expected"; break;
			case 121: s = "MERGE expected"; break;
			case 122: s = "NATIONAL expected"; break;
			case 123: s = "NOCHECK expected"; break;
			case 124: s = "NONCLUSTERED expected"; break;
			case 125: s = "NOT expected"; break;
			case 126: s = "NULL expected"; break;
			case 127: s = "NULLIF expected"; break;
			case 128: s = "OF expected"; break;
			case 129: s = "OFF expected"; break;
			case 130: s = "OFFSETS expected"; break;
			case 131: s = "ON expected"; break;
			case 132: s = "OPEN expected"; break;
			case 133: s = "OPENDATASOURCE expected"; break;
			case 134: s = "OPENQUERY expected"; break;
			case 135: s = "OPENROWSET expected"; break;
			case 136: s = "OPENXML expected"; break;
			case 137: s = "OPTION expected"; break;
			case 138: s = "OR expected"; break;
			case 139: s = "ORDER expected"; break;
			case 140: s = "OUTER expected"; break;
			case 141: s = "OVER expected"; break;
			case 142: s = "PERCENT expected"; break;
			case 143: s = "PIVOT expected"; break;
			case 144: s = "PLAN expected"; break;
			case 145: s = "PRECISION expected"; break;
			case 146: s = "PRIMARY expected"; break;
			case 147: s = "PRINT expected"; break;
			case 148: s = "PROC expected"; break;
			case 149: s = "PROCEDURE expected"; break;
			case 150: s = "PUBLIC expected"; break;
			case 151: s = "RAISERROR expected"; break;
			case 152: s = "READ expected"; break;
			case 153: s = "READTEXT expected"; break;
			case 154: s = "RECONFIGURE expected"; break;
			case 155: s = "REFERENCES expected"; break;
			case 156: s = "REPLICATION expected"; break;
			case 157: s = "RESTORE expected"; break;
			case 158: s = "RESTRICT expected"; break;
			case 159: s = "RETURN expected"; break;
			case 160: s = "REVERT expected"; break;
			case 161: s = "REVOKE expected"; break;
			case 162: s = "RIGHT expected"; break;
			case 163: s = "ROLLBACK expected"; break;
			case 164: s = "ROWCOUNT expected"; break;
			case 165: s = "ROWGUIDCOL expected"; break;
			case 166: s = "RULE expected"; break;
			case 167: s = "SAVE expected"; break;
			case 168: s = "SCHEMA expected"; break;
			case 169: s = "SECURITYAUDIT expected"; break;
			case 170: s = "SELECT expected"; break;
			case 171: s = "SESSIONUSER expected"; break;
			case 172: s = "SET expected"; break;
			case 173: s = "SETUSER expected"; break;
			case 174: s = "SHUTDOWN expected"; break;
			case 175: s = "SOME expected"; break;
			case 176: s = "STATISTICS expected"; break;
			case 177: s = "SYSTEMUSER expected"; break;
			case 178: s = "TABLE expected"; break;
			case 179: s = "TABLESAMPLE expected"; break;
			case 180: s = "TEXTSIZE expected"; break;
			case 181: s = "THEN expected"; break;
			case 182: s = "TOKW expected"; break;
			case 183: s = "TOP expected"; break;
			case 184: s = "TRAN expected"; break;
			case 185: s = "TRANSACTION expected"; break;
			case 186: s = "TRIGGER expected"; break;
			case 187: s = "TRUNCATE expected"; break;
			case 188: s = "TSEQUAL expected"; break;
			case 189: s = "UNION expected"; break;
			case 190: s = "UNIQUE expected"; break;
			case 191: s = "UNPIVOT expected"; break;
			case 192: s = "UPDATE expected"; break;
			case 193: s = "UPDATETEXT expected"; break;
			case 194: s = "USE expected"; break;
			case 195: s = "USER expected"; break;
			case 196: s = "VALUES expected"; break;
			case 197: s = "VARYING expected"; break;
			case 198: s = "VIEW expected"; break;
			case 199: s = "WAITFOR expected"; break;
			case 200: s = "WHEN expected"; break;
			case 201: s = "WHERE expected"; break;
			case 202: s = "WHILE expected"; break;
			case 203: s = "WITH expected"; break;
			case 204: s = "WRITETEXT expected"; break;
			case 205: s = "??? expected"; break;
				default:
					s = "error " + n;
					break;
			}
			return s;
		}
	}
}
