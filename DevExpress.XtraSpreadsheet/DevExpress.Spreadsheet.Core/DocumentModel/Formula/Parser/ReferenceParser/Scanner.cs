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
using System.Text;
#if SL
using DevExpress.Utils;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
#region Token
public class Token {
	#region Fields
	int fkind;	
	int fpos;	 
	int fcharPos; 
	int fcol;	 
	int fline;	
	string fval;  
	Token fnext;  
	#endregion
	#region Properties
	public int kind { get { return fkind; } set { fkind = value; } } 
	public int pos { get { return fpos; } set { fpos = value; } } 
	public int charPos { get { return fcharPos; } set { fcharPos = value; } }
	public int col { get { return fcol; } set { fcol = value; } }
	public int line { get { return fline; } set { fline = value; } }
	public string val { get { return fval; } set { fval = value; } }
	public Token next { get { return fnext; } set { fnext = value; } }
	#endregion
}
#endregion
#region ReferenceScanner
public class ReferenceScanner  : IDisposable{
	#region Fields
	const char EOL = '\n';
	const int eofSym = 0; 
	const int maxT = 39;
	const int noSym = 39;
	char valCh;	   
	Buffer buffer; 
	Token t;		  
	int ch;		   
	int pos;		  
	int charPos;	  
	int col;		  
	int line;		 
	int oldEols;	  
	int[] start; 
	Token tokens;	 
	Token pt;		 
	char[] tval = new char[128]; 
	int tlen;		 
	char decimalSymbol;
	char listSeparator;
	int lastDecimalSymbolStart;
	int lastListSeparatorStart;
	int defaultDecimalSymbolStart;
	int defaultListSeparatorStart;
	#endregion
	void InitStartPoints(){
	start = new int[char.MaxValue + 2];
		for (int i = 0; i <= 12; ++i) start[i] = 20;
		for (int i = 14; i <= 31; ++i) start[i] = 20;
		for (int i = 65; i <= 90; ++i) start[i] = 20;
		for (int i = 95; i <= 122; ++i) start[i] = 20;
		for (int i = 127; i <= 65535; ++i) start[i] = 20;
		for (int i = 48; i <= 57; ++i) start[i] = 21;
		for (int i = 32; i <= 32; ++i) start[i] = 15;
		start[46] = 2; 
		start[39] = 46; 
		start[58] = 47; 
		start[60] = 48; 
		start[62] = 49; 
		start[61] = 25; 
		start[38] = 26; 
		start[43] = 27; 
		start[45] = 28; 
		start[42] = 29; 
		start[47] = 30; 
		start[94] = 31; 
		start[37] = 32; 
		start[44] = 33; 
		start[91] = 34; 
		start[124] = 35; 
		start[33] = 36; 
		start[40] = 37; 
		start[41] = 38; 
		start[36] = 39; 
		start[93] = 40; 
		start[64] = 41; 
		start[35] = 42; 
		start[123] = 43; 
		start[125] = 44; 
		start[34] = 45; 
		start[Buffer.EOF] = -1;
	}
	#region Properties
	public Buffer Buffer { get { return buffer; } }
	#endregion
	public ReferenceScanner () {
		InitStartPoints();
		this.decimalSymbol = '.';
		this.listSeparator = ',';
		defaultDecimalSymbolStart = start[this.decimalSymbol];
		defaultListSeparatorStart = start[this.listSeparator];
		lastDecimalSymbolStart = 0;
		lastListSeparatorStart = 0;
	}
	public void SetString(string text, char decimalSymbol, char listSeparator){
		if (decimalSymbol != this.decimalSymbol || listSeparator != this.listSeparator) {
			start[this.decimalSymbol] = lastDecimalSymbolStart;
			start[this.listSeparator] = lastListSeparatorStart;
			this.decimalSymbol = decimalSymbol;
			this.listSeparator = listSeparator;
			lastDecimalSymbolStart = start[decimalSymbol];
			lastListSeparatorStart = start[listSeparator];
			start[decimalSymbol] = defaultDecimalSymbolStart;
			start[listSeparator] = defaultListSeparatorStart;
		}
		buffer = new StringBuffer(text);
		Init();
	}
	void Init() {
		pos = -1; line = 1; col = 0; charPos = -1;
		oldEols = 0;
		NextCh();
		pt = tokens = new Token();  
	}
	void NextCh() {
		if (oldEols > 0) { ch = EOL; oldEols--; } 
		else {
			pos = buffer.Pos;
			ch = buffer.Read(); col++; charPos++;
			if (ch == '\r' && buffer.Peek() != '\n') ch = EOL;
			if (ch == EOL) { line++; col = 0; }
		}
		if (ch != Buffer.EOF) {
			valCh = (char) ch;
			ch = char.ToLower((char) ch);
		}
	}
	void AddCh() {
		if (tlen >= tval.Length) {
			char[] newBuf = new char[2 * tval.Length];
			Array.Copy(tval, 0, newBuf, 0, tval.Length);
			tval = newBuf;
		}
		if (ch != Buffer.EOF) {
			tval[tlen++] = valCh;
			NextCh();
		}
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811")]
	void CheckLiteral() {
		switch (t.val.ToLower()) {
			case "true": t.kind = 9; break;
			case "false": t.kind = 10; break;
			default: break;
		}
	}
	Token NextToken() {
		while (ch == ' ' ||
			ch >= 9 && ch <= 10 || ch == 13
		) NextCh();
		int recKind = noSym;
		int recEnd = pos;
		t = new Token();
		t.pos = pos; t.col = col; t.line = line; t.charPos = charPos;
		int state;
		if (start[ch] != 0) 
			state = start[ch];
		else 
			state = 0;
		tlen = 0; AddCh();
		switch (state) {
			case -1: { t.kind = eofSym; break; } 
			case 0: {
				if (recKind != noSym) {
					tlen = recEnd - t.pos;
					SetScannerBehindT();
				}
				t.kind = recKind; break;
			} 
			case 1:
				recEnd = pos; recKind = 1;
				if (ch <= 12 || ch >= 14 && ch <= 31 || ch == '.' || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch >= '_' && ch <= 'z' || ch >= 127 && ch <= 65535) {AddCh(); goto case 1;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 2:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 3;}
				else {goto case 0;}
			case 3:
				recEnd = pos; recKind = 3;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 3;}
				else if (ch == 'e') {AddCh(); goto case 4;}
				else {t.kind = 3; break;}
			case 4:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 6;}
				else if (ch == '+' || ch == '-') {AddCh(); goto case 5;}
				else {goto case 0;}
			case 5:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 6;}
				else {goto case 0;}
			case 6:
				recEnd = pos; recKind = 3;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 6;}
				else {t.kind = 3; break;}
			case 7:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 8;}
				else {goto case 0;}
			case 8:
				recEnd = pos; recKind = 3;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 8;}
				else if (ch == 'e') {AddCh(); goto case 9;}
				else {t.kind = 3; break;}
			case 9:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 11;}
				else if (ch == '+' || ch == '-') {AddCh(); goto case 10;}
				else {goto case 0;}
			case 10:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 11;}
				else {goto case 0;}
			case 11:
				recEnd = pos; recKind = 3;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 11;}
				else {t.kind = 3; break;}
			case 12:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 14;}
				else if (ch == '+' || ch == '-') {AddCh(); goto case 13;}
				else {goto case 0;}
			case 13:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 14;}
				else {goto case 0;}
			case 14:
				recEnd = pos; recKind = 3;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 14;}
				else {t.kind = 3; break;}
			case 15:
				{t.kind = 4; break;}
			case 16:
				recEnd = pos; recKind = 5;
				if (ch <= 12 || ch >= 14 && ch <= 31 || ch >= '0' && ch <= '9' || ch == '?' || ch >= 'A' && ch <= 'Z' || ch == 92 || ch >= '_' && ch <= 'z' || ch >= 127 && ch <= 65535) {AddCh(); goto case 16;}
				else {t.kind = 5; break;}
			case 17:
				{t.kind = 6; break;}
			case 18:
				{t.kind = 7; break;}
			case 19:
				{t.kind = 8; break;}
			case 20:
				recEnd = pos; recKind = 1;
				if (ch <= 12 || ch >= 14 && ch <= 31 || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch >= '_' && ch <= 'z' || ch >= 127 && ch <= 65535) {AddCh(); goto case 20;}
				else if (ch == '.') {AddCh(); goto case 1;}
				else if (ch == '?' || ch == 92) {AddCh(); goto case 16;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 21:
				recEnd = pos; recKind = 2;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 21;}
				else if (ch == decimalSymbol) { AddCh(); goto case 7; }
				else if (ch == 'e') {AddCh(); goto case 12;}
				else {t.kind = 2; break;}
			case 22:
				{t.kind = 13; break;}
			case 23:
				{t.kind = 14; break;}
			case 24:
				{t.kind = 15; break;}
			case 25:
				{t.kind = 16; break;}
			case 26:
				{t.kind = 17; break;}
			case 27:
				{t.kind = 18; break;}
			case 28:
				{t.kind = 19; break;}
			case 29:
				{t.kind = 20; break;}
			case 30:
				{t.kind = 21; break;}
			case 31:
				{t.kind = 22; break;}
			case 32:
				{t.kind = 23; break;}
			case 33:
				{t.kind = 24; break;}
			case 34:
				{t.kind = 26; break;}
			case 35:
				{t.kind = 27; break;}
			case 36:
				{t.kind = 28; break;}
			case 37:
				{t.kind = 29; break;}
			case 38:
				{t.kind = 30; break;}
			case 39:
				{t.kind = 31; break;}
			case 40:
				{t.kind = 32; break;}
			case 41:
				{t.kind = 34; break;}
			case 42:
				{t.kind = 35; break;}
			case 43:
				{t.kind = 36; break;}
			case 44:
				{t.kind = 37; break;}
			case 45:
				{t.kind = 38; break;}
			case 46:
				recEnd = pos; recKind = 33;
				if (ch == '#' || ch == 39 || ch == '@' || ch == ']') {AddCh(); goto case 18;}
				else if (ch == '[') {AddCh(); goto case 17;}
				else {t.kind = 33; break;}
			case 47:
				recEnd = pos; recKind = 25;
				if (ch == 92) {AddCh(); goto case 19;}
				else {t.kind = 25; break;}
			case 48:
				recEnd = pos; recKind = 11;
				if (ch == '=') {AddCh(); goto case 22;}
				else if (ch == '>') {AddCh(); goto case 24;}
				else {t.kind = 11; break;}
			case 49:
				recEnd = pos; recKind = 12;
				if (ch == '=') {AddCh(); goto case 23;}
				else {t.kind = 12; break;}
		}
		t.val = new String(tval, 0, tlen);
		return t;
	}
	private void SetScannerBehindT() {
		buffer.Pos = t.pos;
		NextCh();
		line = t.line; col = t.col; charPos = t.charPos;
		for (int i = 0; i < tlen; i++) NextCh();
	}
	public Token Scan () {
		if (tokens.next == null) {
			return NextToken();
		} else {
			pt = tokens = tokens.next;
			return tokens;
		}
	}
	public Token Peek () {
		do {
			if (pt.next == null) {
				pt.next = NextToken();
			}
			pt = pt.next;
		} while (pt.kind > maxT); 
		return pt;
	}
	public void ResetPeek () { pt = tokens; }
	#region IDisposable Members
	public void Dispose() {
	}
	#endregion
}
#endregion
}
