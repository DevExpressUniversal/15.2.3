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
#region ComplexNumberScanner
public class ComplexNumberScanner  : IDisposable{
	#region Fields
	const char EOL = '\n';
	const int eofSym = 0; 
	const int maxT = 9;
	const int noSym = 9;
	IBuffer buffer; 
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
	int lastDecimalSymbolStart;
	int defaultDecimalSymbolStart;
	#endregion
	void InitStartPoints(){
	start = new int[char.MaxValue + 2];
		for (int i = 48; i <= 57; ++i) start[i] = 16;
		for (int i = 32; i <= 32; ++i) start[i] = 15;
		for (int i = 105; i <= 106; ++i) start[i] = 1;
		start[46] = 2; 
		start[43] = 17; 
		start[45] = 18; 
		start[40] = 19; 
		start[41] = 20;
		start[StreamBuffer.EOF] = -1;
	}
	#region Properties
	public IBuffer Buffer { get { return buffer; } }
	#endregion
	public ComplexNumberScanner () {
		InitStartPoints();
		this.decimalSymbol = '.';
		defaultDecimalSymbolStart = start[this.decimalSymbol];
		lastDecimalSymbolStart = 0;
	}
	public void SetString(string text, char decimalSymbol){
		if (decimalSymbol != this.decimalSymbol) {
			start[this.decimalSymbol] = lastDecimalSymbolStart;
			this.decimalSymbol = decimalSymbol;
			lastDecimalSymbolStart = start[decimalSymbol];
			start[decimalSymbol] = defaultDecimalSymbolStart;
		}
		buffer = new StringBuffer(text);
		Init();
	}
	void Init() {
		pos = -1; line = 1; col = 0; charPos = -1;
		oldEols = 0;
		NextCh();
		if (ch == 0xEF) { 
			NextCh(); int ch1 = ch;
			NextCh(); int ch2 = ch;
			if (ch1 != 0xBB || ch2 != 0xBF) {
				throw new FatalError(String.Format("illegal byte order mark: EF {0,2:X} {1,2:X}", ch1, ch2));
			}
			buffer = new UTF8Buffer(buffer as StreamBuffer); col = 0; charPos = -1;
			NextCh();
		}
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
	}
	void AddCh() {
		if (tlen >= tval.Length) {
			char[] newBuf = new char[2 * tval.Length];
			Array.Copy(tval, 0, newBuf, 0, tval.Length);
			tval = newBuf;
		}
		if (ch != StreamBuffer.EOF) {
			tval[tlen++] = (char) ch;
			NextCh();
		}
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811")]
	void CheckLiteral() {
		switch (t.val) {
			default: break;
		}
	}
	Token NextToken() {
		while (ch == ' ' ||
			ch == 9
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
				{t.kind = 2; break;}
			case 2:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 3;}
				else {goto case 0;}
			case 3:
				recEnd = pos; recKind = 3;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 3;}
				else if (ch == 'E' || ch == 'e') {AddCh(); goto case 4;}
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
				else if (ch == 'E' || ch == 'e') {AddCh(); goto case 9;}
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
				recEnd = pos; recKind = 1;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 16;}
				else if (ch == decimalSymbol) {AddCh(); goto case 7;}
				else if (ch == 'E' || ch == 'e') {AddCh(); goto case 12;}
				else {t.kind = 1; break;}
			case 17:
				{t.kind = 5; break;}
			case 18:
				{t.kind = 6; break;}
			case 19:
				{t.kind = 7; break;}
			case 20:
				{t.kind = 8; break;}
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
