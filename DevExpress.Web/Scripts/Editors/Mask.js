/// <reference path="..\_references.js"/>
(function() {
var MaskPartBase = ASPx.CreateClass(null, {
    typeCode: 1,
    constructor: function() {
        this.valueInitialized = false;
        this.dateTimeRole = null;
	},
	Grow: function(text) {
	    throw "Not supported";
	},
	GetSize: function() {
		throw "Not supported";
	},
	GetValue: function() {
		this.EnsureValueInitialized();
		return this.GetValueCore();
	},
	EnsureValueInitialized: function() {
		if(this.valueInitialized) return;
		this.InitValue();
		this.valueInitialized = true;
	},
	InitValue: function() {
		throw "Not supported";
	},	
	GetValueCore: function() {
		throw "Not supported";
	},
	Clear: function(startPos, endPos) {
	},
	HandleKey: function(maskInfo, keyInfo, pos) {
		throw "Not supported";
	},
	HandleControlKey: function(maskInfo, keyInfo, pos) {	     
	    switch(keyInfo.keyCode) {
		    case ASPx.Key.Left:
		        if(keyInfo.ctrlState)
		            maskInfo.MoveToPrevNonLiteral();
		        else
		            maskInfo.IncCaretPos(-1);
			    break;
		    case ASPx.Key.Right:
		        if(keyInfo.ctrlState)
		            maskInfo.MoveToNextNonLiteral();
		        else
		            maskInfo.IncCaretPos(1);
			    break;
	    }
	},
	HandleMouseWheel: function(maskInfo, delta, pos) {
	},
	AllowIncreaseSize: function() { 
	    return false; 
	},
	SupportsUpDown: function() { 
	    return false; 
	},
	IsValid: function() {
		return true;
	},
	GetHintHtml: function() {
		return "";
	}
});	

var LiteralMaskPart = ASPx.CreateClass(MaskPartBase, {
    typeCode: 2,
    constructor: function() {
        this.constructor.prototype.constructor.call(this);        
        this.literal = "";
	},
	Grow: function(text) {
	    this.literal += text;
	},	
	GetSize: function() {
		return this.literal.length;
	},
	InitValue: function() {
	},	
	GetValueCore: function() {
		return this.literal;
	},
	HandleKey: function(maskInfo, keyInfo, pos) {
		if(keyInfo.keyCode == 32){
		    maskInfo.IncCaretPos();
		    return true;
		}
		var ch = String.fromCharCode(keyInfo.keyCode).toLowerCase();
		var index = this.GetValue().toLowerCase().indexOf(ch, pos);
		if(index > -1){
		    maskInfo.IncCaretPos(index - pos + 1);
		    return true;
		}
		maskInfo.IncCaretPos();
	    return false;
	},
	HandleControlKey: function(maskInfo, keyInfo, pos) {
		switch(keyInfo.keyCode) {
		    case ASPx.Key.Right:
			case ASPx.Key.Delete:
			    maskInfo.IncCaretPos(this.GetSize() - pos);
			    break;
            case ASPx.Key.Left:			    
			case ASPx.Key.Backspace:
			    maskInfo.IncCaretPos(-pos);
			    break;
			default:
			    MaskPartBase.prototype.HandleControlKey.call(this, maskInfo, keyInfo, pos);
	    }
	}
});	

var EnumMaskPart = ASPx.CreateClass(MaskPartBase, {
    typeCode: 3,
    constructor: function(items) {
        this.constructor.prototype.constructor.call(this);
        this.items = [];
        this.itemIndex = 0;
        this.defaultItemIndex = 0;
        this.PrepareItems(items);
	},
	PrepareItems: function(items){	
	    var hash = {};
	    for(var i = 0; i < items.length; i++){
	        var item = String(items[i]);
	        if(item.length > 0 && !ASPx.IsExists(hash[item])){
	            if(item.charAt(0) == "*"){
	                this.defaultItemIndex = i;
	                item = item.substr(1);
	            }
	            this.items.push(item);
	            hash[item] = 1;
	        } 
	    }
	},
	GetSize: function() {
		return this.GetValue().length;
	},
	InitValue: function() {	    
		this.itemIndex = this.defaultItemIndex;
	},	
	GetValueCore: function() {
		return this.items[this.itemIndex];
	},
	Clear: function(startPos, endPos) {
	    this.ClearInternal(startPos);
	},
	ClearInternal: function(pos) {
        var prefix = this.GetValue().substr(0, pos);
        if(prefix.length < 1) {
            this.itemIndex = this.defaultItemIndex;
		} else {
            this.itemIndex = this.FindItemIndexByPrefix(prefix);
        }
	},
	HandleKey: function(maskInfo, keyInfo, pos) {
	    var ch = String.fromCharCode(keyInfo.keyCode);
	    var prefix = this.GetValue().substr(0, pos) + ch;
	    var index = this.FindItemIndexByPrefix(prefix);
        
        var offset = (index < 0 || this.GetSize() > prefix.length ? this.GetSize() : prefix.length) - pos;
		if(index < 0 && (ch != " " || offset == 0)) {
			maskInfo.SetCaret(maskInfo.caretPos, offset);
			return false;
		}
		if(index > -1)
			this.itemIndex = index;
		maskInfo.SetCaret(1 + maskInfo.caretPos, offset - 1);
	    return true;
	},
	HandleControlKey: function(maskInfo, keyInfo, pos) {
		switch(keyInfo.keyCode) {
			case ASPx.Key.Up:
			    this.ChangeItemIndex(maskInfo, this.dateTimeRole != null ? 1 : -1, pos);
				break;
			case ASPx.Key.Down:
			    this.ChangeItemIndex(maskInfo, this.dateTimeRole != null ? -1 : 1, pos);
				break;				
			case ASPx.Key.Backspace:
			    if(keyInfo.ctrlState){
			        this.itemIndex = this.defaultItemIndex;
			        maskInfo.IncCaretPos(-pos);
			    }
				else {
				    this.ClearInternal(pos - 1);
				    maskInfo.SetCaret(maskInfo.caretPos - 1, 0);
				}
				break;
			case ASPx.Key.Delete:
			    if(keyInfo.ctrlState){
			        this.itemIndex = this.defaultItemIndex;
			        maskInfo.IncCaretPos(this.GetSize() - pos);
				}
				else {
				    this.ClearInternal(pos);
				    maskInfo.SetCaret(maskInfo.caretPos + 1, 0);
				}
				break;				
			default:
			    MaskPartBase.prototype.HandleControlKey.call(this, maskInfo, keyInfo, pos);
	    }
	},
	HandleMouseWheel: function(maskInfo, delta, pos) {
		if(this.dateTimeRole == null)
			delta = -delta;
	    this.ChangeItemIndex(maskInfo, delta, pos);
	},
	ChangeItemIndex: function(maskInfo, delta, pos) {
	    this.itemIndex += delta;
		while(this.itemIndex < 0)
			this.itemIndex += this.items.length;
		while(this.itemIndex > this.items.length - 1)
			this.itemIndex -= this.items.length;
	    maskInfo.SetCaret(maskInfo.caretPos - pos, this.GetSize());
	},
	FindItemIndexByPrefix: function(prefix) {	    
	    prefix = prefix.toLowerCase();
	    for(var i = 0; i < this.items.length; i++) {
	        var item = this.items[i];
	        if(item.toLowerCase().indexOf(prefix) == 0)
	            return i;
	    }
	    return -1;
	},
	SupportsUpDown: function() { 
	    return true; 
	},
	GetHintHtml: function() {
		if(this.dateTimeRole != null)
			return "";
		var list = [];
		for(var i = 0; i < this.items.length; i++) {
			var text = this.items[i];
			if(i == this.itemIndex)
				text = "<strong>" + text + "</strong>";
			list.push(text);
		}
		return list.join(", ");
	},
    AllowIncreaseSize: function() {
        var value = this.GetValue();
        if(value) {
            for (var i = 0; i < this.items.length; i++)
                if(this.items[i].length > value.length)
                    return true;
        }
        return false;
    }
});

var RangeMaskPart = ASPx.CreateClass(MaskPartBase, {
    typeCode: 4,
    constructor: function(minNumber, maxNumber) {
        this.constructor.prototype.constructor.call(this);        
        if(maxNumber < minNumber)
			maxNumber = minNumber;        
        this.minNumber = minNumber;
        this.maxNumber = maxNumber;
        this.defaultNumber = null;
        this.zeroFill = false;
        this.absNumber = 0;
        this.negative = false;
        this.enableGroups = false;
	},
	GetSize: function() {
		return this.GetValue().length;
	},
	InitValue: function() {
	    var number = 0;
	    if(this.defaultNumber != null)
	        number = this.defaultNumber;
		else {
		    if(this.maxNumber < 0)
			    number = this.maxNumber;
		    else if(this.minNumber < 0)
			    number = 0;
		    else
			    number = this.minNumber;
	    }
	    this.SetNumber(number);
	},	
	GetValueCore: function() {
	    var value = String(this.absNumber);	    
	    if(this.zeroFill) {
            var size = Math.max(this.minNumber.toString().length, this.maxNumber.toString().length);
            var incSize = size - value.length;
            for(var i = 0; i < incSize; i++)
                value = "0" + value;
        }
        if(this.enableGroups)
			value = this.AddGroupSeparators(value);
        if(this.negative)
            value = "-" + value;
        return value;
	},
	AddGroupSeparators: function(text) {
		if(text.length < 4)
			return text;
		var temp = [ ];
		var count = Math.ceil(text.length / 3);
		for(var i = 1; i < count; i++)
			temp.unshift(text.substr(text.length - i * 3, 3));
		temp.unshift(text.substr(0, text.length % 3 || 3));
		return temp.join(ASPx.CultureInfo.numGroupSeparator);
	},
	IsGroupSeparatorPos: function(pos) {
		if(!this.enableGroups)
			return false;
		var reversePos = this.GetSize() - pos;
		return reversePos > 0 && reversePos % 4 == 0;
	},	
	GetNumber: function() {
		var result = this.absNumber;
		if(this.negative)
			result = -result;
		return result;
	},
	SetNumber: function(number) {
	    this.negative = (number < 0);
	    this.absNumber = Math.abs(number);
	},
    TextToNumber: function(text) {
	    if(text == "" || text == "-")
            return 0;	    
	    if(this.enableGroups)
			text = text.split(ASPx.CultureInfo.numGroupSeparator).join("");
	    return Number(text);
    },
	SetText: function(text, checkMinNumber) {
		checkMinNumber = checkMinNumber || Math.abs(this.minNumber) < 2;
	    var number = this.TextToNumber(text);
        if(number > this.maxNumber) {
            this.SetNumber(this.maxNumber);
        } else if(checkMinNumber && number < this.minNumber) {
            this.SetNumber(this.minNumber);
        } else {
            this.absNumber = Math.abs(number);
            this.negative = (text.indexOf("-") > -1);
        }	    
	},
	Clear: function(startPos, endPos){
		var newText = ASPx.Str.InsertEx(this.GetValue(), "", startPos, endPos);
		if(newText.length < 1 && this.defaultNumber !== null)
            this.SetNumber(this.defaultNumber);
        else
		    this.SetText(newText, true);
    },
	HandleKey: function(maskInfo, keyInfo, pos) {	
	    var keyCode = keyInfo.keyCode;	    	    
	    var ch = String.fromCharCode(keyCode);
	    if((ch == ASPx.CultureInfo.numGroupSeparator && this.IsGroupSeparatorPos(pos)
			|| keyCode == 32) && pos < this.GetSize()) {
	        maskInfo.IncCaretPos();
	        return true;
	    }
	    var oldNumber = this.GetNumber();
        if(MaskManager.IsSignumCode(keyCode)) {
            if((ch == "-" && this.minNumber < 0)  || (ch == "+" && oldNumber < 0)) {               
                var newNumber = -oldNumber;
                if(this.CheckRange(newNumber)) {
                    this.negative = !this.negative;
                    maskInfo.SetCaret(maskInfo.caretPos - pos + (this.negative ? 1 : 0), 0);
                    return true;
                }
            }
        }
        if(MaskManager.IsDigitCode(keyCode)) {
			if(!this.zeroFill && ch == "0" && oldNumber == 0 && pos > this.GetSize() - 1)
				return false;
            this.TryTypeAtPos(maskInfo, ch, pos, 1);
            return true;
        }
	    return false;
	},
	HandleControlKey: function(maskInfo, keyInfo, pos) {
		switch(keyInfo.keyCode) {
			case ASPx.Key.Up:			    
			    this.ChangeNumber(maskInfo, 1, pos);
				break;
			case ASPx.Key.Down:
			    this.ChangeNumber(maskInfo, -1, pos);
				break;
			case ASPx.Key.Delete:
			    if(keyInfo.ctrlState) {			    
			        var newText = this.GetValue().substr(0, pos);
                    this.SetText(newText, false);
                    maskInfo.IncCaretPos(this.GetSize() - pos);
			    } else {						        
					if(this.IsGroupSeparatorPos(pos)) {
						maskInfo.IncCaretPos();
					} else {						        
			            if(this.zeroFill)
			                this.TryTypeAtPos(maskInfo, "0", pos, 1);
			            else
			                this.TryTypeAtPos(maskInfo, "", pos, 1);
			    }
			    }
			    break;
			case ASPx.Key.Backspace:
			    if(keyInfo.ctrlState) {			    
			        var newText = this.GetValue().substr(pos);
                    this.SetText(newText, false);
                    maskInfo.IncCaretPos(-pos);
                } else {			                
					if(this.IsGroupSeparatorPos(pos - 1)) {
						maskInfo.IncCaretPos(-1);
					} else {
			            if(this.zeroFill)
    			            this.TryTypeAtPos(maskInfo, "0", pos, -1);
			            else
			                this.TryTypeAtPos(maskInfo, "", pos, -1);
			        }
			    }
			    break;
			default:
			    MaskPartBase.prototype.HandleControlKey.call(this, maskInfo, keyInfo, pos);
	    }
	},
    HandleMouseWheel: function(maskInfo, delta, pos) {
        this.ChangeNumber(maskInfo, delta, pos);
    },
    ChangeNumber: function(maskInfo, delta, pos) {
		var number = this.GetNumber();
		if(number < this.minNumber)
			number = this.minNumber;
        var newNumber = number + delta;
		while(newNumber < this.minNumber)
			newNumber += 1 + this.maxNumber - this.minNumber;
		while(newNumber > this.maxNumber)
			newNumber -= 1 + this.maxNumber - this.minNumber;
        this.SetNumber(newNumber);
        maskInfo.SetCaret(maskInfo.caretPos - pos, this.GetSize());    
    },
	CheckRange: function(number){
	    return (this.minNumber <= number && number <= this.maxNumber);
	},
	TryTypeAtPos: function(maskInfo, str, pos, dir) {
		if(dir > 0 && this.IsGroupSeparatorPos(pos)) {
			pos++;
			maskInfo.IncCaretPos();
		}
        var oldSize = this.GetSize();
        var strPos = pos;
        if(dir < 0) strPos -= 1;

        var newText = ASPx.Str.InsertEx(this.GetValue(), str, strPos, strPos + 1);

        // special case when entering a single digit completes the whole date part (B207347)
        if(this.dateTimeRole && dir > 0 && pos == 0 && /\d/.test(str) && !this.CheckRange(this.TextToNumber(newText))) {
            var number = Number(str);
            if(this.negative)
                number = -number;
            if(this.CheckRange(number) && this.IsMaxMagnitude(number)) {
                this.SetNumber(number);
                maskInfo.IncCaretPos(this.GetSize() - pos);
                return; // RETURNS
            }                
        }

        var newPos;
        this.SetText(newText, false);
        if(dir > 0 && oldSize == pos) {
			newPos = this.GetSize();
        } else  {                        
			var diff = 0;
			if(!this.zeroFill) {
				diff = this.GetSize() - oldSize;
				if(dir < 0) diff += 1;
				if(diff > 0) diff = 0;
			}
			newPos = pos + dir + diff;
		}
        if(newPos < 0) newPos = 0;        
        if(newPos > this.GetSize()) newPos = this.GetSize();
        if(this.IsGroupSeparatorPos(newPos))
			newPos++;
        maskInfo.IncCaretPos(newPos - pos);
	},	
    IsMaxMagnitude: function(number) {
	    if(number < 0)
	        return number * 10 < this.minNumber;
        return number * 10 > this.maxNumber;
    },
	AllowIncreaseSize: function() {
        return !this.zeroFill && !this.IsMaxMagnitude(this.GetNumber());
	},
	SupportsUpDown: function() { 
	    return true; 
	},
	GetHintHtml: function() {
		if(this.dateTimeRole != null)
			return "";	
		return this.minNumber + ".." + this.maxNumber;
	}
});	

var PromptMaskPart = ASPx.CreateClass(MaskPartBase, {
    typeCode: 5,
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
        this.required = false;
        this.size = 0;
        this.text = "";
	},
	Grow: function(text) {
	    this.size += text.length;
	},	
	GetSize: function() {
		return this.size;
	},
	InitValue: function() {		
		var size = this.GetSize();
		for(var i = 0; i < size; i++)
			this.text += " ";
	},	
	GetValueCore: function() {
		return this.text;
	},
	Clear: function(startPos, endPos){
	    this.ClearInternal(startPos, endPos - startPos);
    },
    ClearInternal: function(pos, count){
	    for(var  i = 0; i < count; i++)
            this.SetCharInPos(" ", i + pos);                                    
    },
	HandleKey: function(maskInfo, keyInfo, pos) {
		var keyCode = keyInfo.keyCode;		
		if(maskInfo.IsPromptCode(keyCode))
			keyCode = 32;
		if(keyCode != 32 && !this.IsValidCharCode(keyCode, pos))
			return false;
		this.SetCharInPos(String.fromCharCode(keyCode), pos);
		maskInfo.IncCaretPos();
		return true;
	},
	HandleControlKey: function(maskInfo, keyInfo, pos) {
        switch(keyInfo.keyCode) {
			case ASPx.Key.Delete:
			    var count = keyInfo.ctrlState ? this.GetSize() - pos : 1;
			    this.ClearInternal(pos, count);
                maskInfo.IncCaretPos(count);
			    break; 
			case ASPx.Key.Backspace:
			    var count = keyInfo.ctrlState ? pos : 1;
			    this.ClearInternal(pos - count, count);
                maskInfo.IncCaretPos(-count);
			    break;
			default:
			    MaskPartBase.prototype.HandleControlKey.call(this, maskInfo, keyInfo, pos);
	    }
	},	
	SetCharInPos: function(ch, pos) {
	    this.text = ASPx.Str.InsertEx(this.GetValue(), ch, pos, pos + 1);
	},	
	IsValidCharCode: function(code, pos) {
		throw "Not supported";
	},
	IsValid: function() {
		if(!this.required)
			return true;
		return this.GetValue().indexOf(" ") < 0;
	}
});
	
var NumericMaskPart = ASPx.CreateClass(PromptMaskPart, {
    typeCode: 6,
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
        this.acceptsSignum = false;
	},
    IsValidCharCode: function(code, pos) {
		if(MaskManager.IsSignumCode(code)) {
		    if(!this.acceptsSignum) return false;
		    
		    var value = this.GetValue();
		    for(var i = 0; i < pos; i++){
		        var currentCode = value.charCodeAt(i);
		        if(MaskManager.IsDigitCode(currentCode) || MaskManager.IsSignumCode(currentCode))
		            return false;
		    }
		    return true;
	    }
		return MaskManager.IsDigitCode(code);
    }	
});

var CharMaskPart = ASPx.CreateClass(PromptMaskPart, {
    typeCode: 7,
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
        this.caseConvention = 0;
	},
	SetCharInPos: function(ch, pos) {
		if(this.caseConvention < 0)
			ch = ch.toLowerCase();
		if(this.caseConvention > 0)
			ch = ch.toUpperCase();			
		PromptMaskPart.prototype.SetCharInPos.call(this, ch, pos);
	},	
    IsValidCharCode: function(code, pos) {
		return code > 31;
    }
});
	
var AlphaMaskPart = ASPx.CreateClass(CharMaskPart, {
    typeCode: 8,
    IsValidCharCode: function(code, pos) {
		return MaskManager.IsAlphaCode(code);
    }
});	

var AlphaNumericMaskPart = ASPx.CreateClass(CharMaskPart, {
    typeCode: 9,
    IsValidCharCode: function(code, pos) {
		return MaskManager.IsAlphaCode(code) || MaskManager.IsDigitCode(code);
    }    
});	


var MaskParser = {
    Parse: function(mask, dateTimeOnly) {
        this.result = [ ];
        this.currentCaseConvention = 0;        
        this.quoteMode = null;
        this.dateTimeOnly = (dateTimeOnly === true);
        mask.replace(this.GetMasterRegex(), this.ParseCallback);
        return this.result;
    },

    // Regular expressions
    regex: {       
        ranges: "\\<-?\\d+(\\.\\.-?\\d+){1,2}g?\\>",
        enums: "\\<\\*?[^|*<>]*(\\|\\*?[^|*<>]*)+\\>",
        prompts: "[LlAaCc09#,.:/$<>~]",
        dates: "(y{1,4}|M{1,4}|d{1,4}|hh?|HH?|mm?|ss?|F{1,6}|f{1,6}|tt?)"
    },    
    GetMasterRegex: function() {
		if(this.dateTimeOnly) {
			if(!this.__masterDateTimeOnlyRegex)
				this.__masterDateTimeOnlyRegex = this.CreateMasterRegex(true);
			return this.__masterDateTimeOnlyRegex;						
		}
		if(!this.__masterRegex)
			this.__masterRegex = this.CreateMasterRegex(false);
		return this.__masterRegex;
    }, 
    GetRangesRegex: function() {
        if(!this.__rangesRegex) 
            this.__rangesRegex = this.CreateAnchoredRegex(this.regex.ranges);
        return this.__rangesRegex;
    },
    GetEnumsRegex: function() {
        if(!this.__enumsRegex) 
            this.__enumsRegex = this.CreateAnchoredRegex(this.regex.enums);
        return this.__enumsRegex;
    },
    GetDatesRegex: function() {
		if(!this.__datesRegex)
			this.__datesRegex = this.CreateAnchoredRegex(this.regex.dates);
		return this.__datesRegex;
    },
    CreateAnchoredRegex: function(text) {
		return new RegExp("^" + text + "$");
    },
    CreateMasterRegex: function(dateTimeOnly) {
		var list = [ ];
		this.PushConditional(list, "\\\\\\\\", true);
		this.PushConditional(list, "\\\\[\"']", true);
		this.PushConditional(list, "[\"']", true);
		this.PushConditional(list, this.regex.ranges, !dateTimeOnly);
		this.PushConditional(list, this.regex.enums, !dateTimeOnly);
		this.PushConditional(list, "\\\\" + this.regex.dates, true);
		this.PushConditional(list, "\\\\" + this.regex.prompts, !dateTimeOnly);
		this.PushConditional(list, this.regex.dates, true);
		this.PushConditional(list, this.regex.prompts, !dateTimeOnly);
		this.PushConditional(list, ".", true);
		return new RegExp("(" + list.join("|") + ")", "g");
    },
    PushConditional: function(list, item, allow) {
		if(allow)
			list.push(item);	
    },
        
    ParseCallback: function(text) {        
        MaskParser.ParseCore(text, null);
    },
    ParseCore: function(text, dateTimeRole) {
		var acceptRangesEnums = (dateTimeRole != null || !this.dateTimeOnly);
		if(text == "'" || text == '"')
			this.ParseQuote(text);
		else if(this.quoteMode != null)
			this.ParseLiteral(text);
        else if(acceptRangesEnums && this.GetRangesRegex().test(text)) 
            this.ParseRange(text, dateTimeRole);
        else if(acceptRangesEnums && this.GetEnumsRegex().test(text)) 
            this.ParseEnum(text, dateTimeRole);
        else if(this.GetDatesRegex().test(text))
			this.ParseDate(text);
        else
            this.ParseSimple(text);
    },
    ParseRange: function(text, dateTimeRole) {        
		var enableGroups = false;
        text = this.StripBrockets(text);
        if(text.charAt(text.length - 1) == "g") {
			enableGroups = true;
			text = text.substr(0, text.length - 1);
		}
        var data = text.split("..");
        var minNumber, maxNumber = 0;
        var defaultNumber = null;
        if(data.length == 2){
            minNumber = Number(data[0]);
            maxNumber = Number(data[1]);
        }
        else if(data.length == 3){
            minNumber = Number(data[0]);
            maxNumber = Number(data[2]);
            defaultNumber = Number(data[1]);
        }
        
        var part = new RangeMaskPart(minNumber, maxNumber);
        part.defaultNumber = defaultNumber;
        part.zeroFill = (data[0] == "00") || (data[0].length > 1 && data[0].charAt(0) == "0");
        part.dateTimeRole = dateTimeRole;
        part.enableGroups = enableGroups;
        this.result.push(part);
    },
    ParseEnum: function(text, dateTimeRole) {
        text = this.StripBrockets(text);
        var part = new EnumMaskPart(text.split("|"));
        part.dateTimeRole = dateTimeRole;
        this.result.push(part);
    },
    StripBrockets: function(text) {
        return text.substr(1, text.length - 2);
    },   

    ParseSimple: function(text) {  
		switch(text) {
			case ":":
				this.ParseLiteral(ASPx.CultureInfo.ts);
				break;
			case "/":
				this.ParseLiteral(ASPx.CultureInfo.ds);
				break;
			default:
				if(this.dateTimeOnly) {
					this.ParseLiteral(text);					
				} else {
					switch(text) {
						case "L":
						case "l":
							this.ParseChar(text, AlphaMaskPart, text == "L");
							break;
						case "A":
						case "a":
							this.ParseChar(text, AlphaNumericMaskPart, text == "A");   
							break;
						case "C":
						case "c":
							this.ParseChar(text, CharMaskPart, text == "C");
							break;
						case ">":
							 this.currentCaseConvention = 1;
							 break;							
						case "<":
							 this.currentCaseConvention = -1;
							 break;						
						case "~":
							 this.currentCaseConvention = 0;
							 break;							
						case "0":
						case "9":
						case "#":
							this.ParseNumeric(text);   
							break;
						case ".":
							this.ParseLiteral(ASPx.CultureInfo.numDecimalPoint);
							break;
						case ",":
							this.ParseLiteral(ASPx.CultureInfo.numGroupSeparator);
							break;
						case "$":
							this.ParseLiteral(ASPx.CultureInfo.currency);
							break;
						default:
							this.ParseLiteral(text);
							break;													 								 
					}
				}				
				break;
		}
    },
    ParseChar: function(text, ctor, required) {
        var part = this.GetCurrentPart();
        if(part == null || part.typeCode != ctor.prototype.typeCode || part.required != required || part.caseConvention != this.currentCaseConvention) {
            part = new ctor();
            part.required = required;
            part.caseConvention = this.currentCaseConvention;
            this.result.push(part);
        }
        part.Grow(text);
    },
    ParseNumeric: function(text) {
        var required = text == "0";
        var acceptsSignum = text == "#";
        var part = this.GetCurrentPart();
        if(part == null || part.typeCode != NumericMaskPart.prototype.typeCode || part.required != required || part.acceptsSignum != acceptsSignum) {
            part = new NumericMaskPart();
            part.required = required;
            part.acceptsSignum = acceptsSignum;
            this.result.push(part);
        }
        part.Grow(text);
    },
    ParseLiteral: function(text) {
        var part = this.GetCurrentPart();
        if(part == null || part.typeCode != LiteralMaskPart.prototype.typeCode) {
            part = new LiteralMaskPart();
            this.result.push(part);
        }
        if(text.length > 0 && text.charAt(0) == "\\")
			text = text.substr(1);
        part.Grow(text);
    },
    GetCurrentPart: function() {
        var len = this.result.length;
        if(len < 1)
            return null;
        return this.result[len - 1];
    },
    
    ParseDate: function(text) {
		this.ParseCore(this.GetDateSpecifierReplacement(text), this.GetDateTimeRole(text));
    },
    GetDateSpecifierReplacement: function(text) {
        switch(text) {
            case "yyyy":				
                return "<0100..9999>"; 
            case "yyy":				
                return "<100..9999>"; 
            case "yy":				
                return "<00..99>";
            case "y":				
                return "<0..99>";
            case "MMMM":				
                return "<" + ASPx.CultureInfo.genMonthNames.join("|") + ">";
            case "MMM":				
                return "<" + ASPx.CultureInfo.abbrMonthNames.join("|") + ">";
            case "MM":				
                return "<01..12>";
            case "M":				
                return "<1..12>";
            case "dddd":
                return "<" + ASPx.CultureInfo.dayNames.join("|") + ">";
            case "ddd":
                return "<" + ASPx.CultureInfo.abbrDayNames.join("|") + ">";
            case "dd":				
                return "<01..31>";
            case "d":				
                return "<1..31>";
            case "hh":
                return "<01..12..12>";
            case "h":
                return "<1..12..12>";
            case "HH":
                return "<00..23>";
            case "H":
                return "<0..23>";
            case "mm":
                return "<00..59>";
            case "m":
                return "<0..59>";
            case "ss":
                return "<00..59>";
            case "s":
                return "<0..59>";
            case "tt":
            case "t":
				if(ASPx.CultureInfo.am.length < 1)
					return "";
                return "<" + this.GetAmPmArray(text.length < 2).join("|") + ">";
        }
        if(/^f{1,6}$/i.test(text)) {
            if(text.length == 1)
                return "<0..9>";
            if(text.length == 2)
                return "<0..99>";                
            return "<0..999>";
        }
		throw "Not supported";
    },	    
    GetDateTimeRole: function(text) {
		var ch = text.charAt(0);
		if(ch == "y" || ch == "M" || ch =="d"
			|| ch.toLowerCase() == "h" || ch == "m" || ch == "s" 
			|| ch.toLowerCase() == "f" || ch == "t")
			return ch;
		return null;
    },
    GetAmPmArray: function(useFirstChar){
        var result = [ ASPx.CultureInfo.am, ASPx.CultureInfo.pm ];
        if(useFirstChar) {
            for(var i = 0; i < result.length; i++)
                result[i] = result[i].charAt(0);            
        }
        return result;
    },
    
    ParseQuote: function(text) {		
		if(this.quoteMode == null) {
			this.quoteMode = text;
		} else {
			if(text == this.quoteMode)
				this.quoteMode = null;
			else
				this.ParseLiteral(text);
		}
    }
};

var MaskIncludeLiterals = {
    All: 1,
    None: 2,
    DecimalSymbol: 3	
};

var MaskInfo = ASPx.CreateClass(null, {    
    constructor: function() {    		
		this.parts = null;
		this.allowMouseWheel = true;
        this.promptChar = "_";        
        this.includeLiterals = MaskIncludeLiterals.All;        
        this.errorText = "";

		this.caretPos = 0;
		this.selectionLength = 0;	
		
		this.lastEditedPart = null;
    },
    
    GetSize: function() { 
        var size = 0;
        for(var i = 0; i < this.parts.length; i++)
            size += this.parts[i].GetSize();
        return size;
    },
    GetText: function() {
		var result = "";
		for(var i = 0; i < this.parts.length; i++) {
			var part = this.parts[i];
			if(MaskManager.IsLiteralPart(part) || MaskManager.IsEnumPart(part))
				result += part.GetValue();
			else
				result += part.GetValue().split(" ").join(this.promptChar);
		}
		return result;
    },
    GetValue: function() {
        var list = [];
        var hasNonEmptyValue = false;
        var canPartReturnNull = false;
        var canPartReturnNullConstValue = true;
        for(var i = this.parts.length - 1; i >= 0; i--) {
            var part = this.parts[i];
            if(part.typeCode < PromptMaskPart.prototype.typeCode && 
                (this.includeLiterals == MaskIncludeLiterals.All || part.typeCode != LiteralMaskPart.prototype.typeCode))
                    canPartReturnNullConstValue = false;
            canPartReturnNull = part.typeCode >= PromptMaskPart.prototype.typeCode ? canPartReturnNullConstValue : false;
            var partValue = part.GetValue();
            if(MaskManager.IsLiteralPart(part) && MaskManager.IsIgnorableLiteral(partValue, this.includeLiterals))
					continue;
			if(MaskManager.IsRangePart(part) && this.includeLiterals != MaskIncludeLiterals.All)
				partValue = partValue.split(ASPx.CultureInfo.numGroupSeparator).join("");
            var valueToAdd = canPartReturnNull && !hasNonEmptyValue ? ASPx.Str.TrimEnd(partValue) : partValue;
            list.unshift(valueToAdd);
            hasNonEmptyValue = hasNonEmptyValue || !!ASPx.Str.TrimEnd(valueToAdd);
        }
        return list.join("");
    },
    SetText: function(text) {
		this.Clear();
		this.SetCaret(0, 0);
		this.SetValueCore(text, MaskIncludeLiterals.All);
		this.SetCaret(0, 0);
    },
    SetValue: function(value) {
		this.Clear();
		this.SetCaret(0, 0);
		this.SetValueCore(value, this.includeLiterals);
		this.SetCaret(0, 0);
    },
    SetValueCore: function(value, includeLiterals) {
        if(value == null) return;

	    for(var i = 0; i < value.length; i++) {
		    var keyInfo = MaskManager.CreateKeyInfo(value.charCodeAt(i), false, false);
		    MaskManager.HandleKey(this, keyInfo, false, includeLiterals);
	    }
    },
    Clear: function() {
		for(var i = 0; i < this.parts.length; i++) {
			var part = this.parts[i];
			part.Clear(0, part.GetSize());
		}
    },

    ProcessPaste: function(rawText, caretPosAfterPaste) {
        var currentText = this.GetText();

        if (caretPosAfterPaste === rawText.length) { //B215174
            this.Clear();
            this.SetCaret(0, 0);
            this.SetValueCore(rawText, this.includeLiterals);
        } else {
            var leadLength = 0;
            for (var i = 0; i < Math.min(rawText.length, currentText.length); i++) {
                if (rawText.charAt(i) != currentText.charAt(i))
                    break;
                leadLength++;
            }
            var pastedText = rawText.substr(leadLength, caretPosAfterPaste - leadLength);
            this.SetCaret(caretPosAfterPaste - pastedText.length, 0);
            var padLength = 0;
            for (var i = pastedText.length + rawText.length; i < currentText.length; i++) {
                pastedText += " ";
                padLength++;
            }
            this.SetValueCore(pastedText, MaskIncludeLiterals.All);
            this.caretPos -= padLength;
        }
    },
    IsValid: function() {
		for(var i = 0; i < this.parts.length; i++) {
			if(!this.parts[i].IsValid())
				return false;
		}
		return true;
    },
            
    SetCaret: function(caretPos, selectionLength) {
        if(selectionLength < 0) throw "Internal Error";
        
		this.caretPos = caretPos;
		this.selectionLength = selectionLength;
    },
    IncCaretPos: function(delta) {
        if(!ASPx.IsExists(delta))
            delta = 1;
		this.caretPos += delta;
		this.selectionLength = 0;
    },
    MoveToPrevNonLiteral: function() {
		var partPos = 0;
		var resultPos = 0;
		for(var i = 0; i < this.parts.length; i++) {
			if(partPos >= this.caretPos)
				break;
			var part = this.parts[i];
			var nextPartPos = partPos + part.GetSize();
			if(!MaskManager.IsLiteralPart(part))
				resultPos = nextPartPos < this.caretPos ? nextPartPos : partPos;
			partPos = nextPartPos;
		}
		this.SetCaret(resultPos, 0);
    },        
    MoveToNextNonLiteral: function() {
		var partPos = 0;
		for(var i = 0; i < this.parts.length; i++) {
			var part = this.parts[i];
			var nextPartPos = partPos + part.GetSize();			
			if(nextPartPos > this.caretPos && !MaskManager.IsLiteralPart(part)) {
				if(partPos <= this.caretPos)
					partPos = nextPartPos;
				break;
			}
			partPos = nextPartPos;
		}
		this.SetCaret(partPos, 0);
    },    
    
    IsPromptCode: function(code) {
		return (code == 32 || code == this.promptChar.charCodeAt(0));
    },
    
    BeforeChange: function(part) {
        this.ApplyFixes(part);
		this.lastEditedPart = part;
		part.EnsureValueInitialized();
    },
    AfterChange: function(part) {
    },
    ApplyFixes: function(currentPart) {
        var result1 = this.FixLastRangePart(currentPart);
        var result2 = this.FixLastDatePart(currentPart);
        return result1 || result2;
    },
    FixLastRangePart: function(currentPart) {		
		var part = this.lastEditedPart;
		if(!part || part == currentPart || !MaskManager.IsRangePart(part))
			return false;
		var number = part.GetNumber();
		if(number >= part.minNumber) 
			return false;
		var prevSize = part.GetSize();
		part.SetNumber(part.minNumber);
		this.SetCaret(this.caretPos + part.GetSize() - prevSize, 0);
		return true;
    },
    FixLastDatePart: function(currentPart) {
		var part = this.lastEditedPart;
		if(!part || part == currentPart || part.dateTimeRole == null)
			return false;
			
		var bag = MaskDateTimeHelper.GetDateBag(this);
		if(!bag.hasDate)
			return false;
			
		var maxDay = MaskDateTimeHelper.GetMaxDayInMonth(bag.month, bag.year);
		if(bag.day > maxDay) {		    
            if(bag.day == 29 && bag.month == 1) {
                bag.year = MaskDateTimeHelper.GetNextLeapYear(bag.year);
            } else {
                if(part.dateTimeRole == "d")
                    bag.month--;
                else
                    bag.day = maxDay;
            }            
        }
        var prefixSize = this.GetSizeBeforeEditedPart(currentPart);
        MaskDateTimeHelper.SetDate(this, MaskDateTimeHelper.CreateDateFromBag(bag, true));
        this.caretPos += this.GetSizeBeforeEditedPart(currentPart) - prefixSize;
        return true;
    },
    GetSizeBeforeEditedPart: function(currentPart) {
		var pos = 0;
		for(var i = 0; i < this.parts.length; i++) {
			if(this.parts[i] == currentPart)
				break;
			pos += this.parts[i].GetSize();
		}
		return pos;
    }    
});
MaskInfo.Create = function(maskText, dateTimeOnly) {
	var info = new MaskInfo();
	info.parts = MaskParser.Parse(maskText, dateTimeOnly);
	return info;
}

// Consider moving this functionality to mask info class
var MaskManager = {
    
	OnKeyPress: function(maskInfo, keyInfo) {
	    if(maskInfo.selectionLength > 0)
	        this.ClearSelection(maskInfo);
		
	    this.HandleKey(maskInfo, keyInfo, true, MaskIncludeLiterals.All);
        this.savedKeyDownKeyInfo = null;
	},
	OnKeyDown: function(maskInfo, keyInfo) {
	    if(maskInfo.selectionLength > 0 && (keyInfo.keyCode == ASPx.Key.Backspace || keyInfo.keyCode == ASPx.Key.Delete))
	        this.ClearSelection(maskInfo);
	    else
	        this.HandleControlKey(maskInfo, keyInfo);
	},
	OnMouseWheel: function(maskInfo, delta) {
		if(maskInfo.allowMouseWheel)
		    this.HandleMouseWheel(maskInfo, delta);
	},
	
	HandleKey: function(maskInfo, keyInfo, autoSkipLiterals, includeLiterals) {	
		var partStart = 0;
		var caretInfoBeforeSkip = null;		
		for(var i = 0; i < maskInfo.parts.length; i++) {
			var part = maskInfo.parts[i];
		    if(this.IsCaretInPart(maskInfo.caretPos, partStart, part)) {				
				if(!this.IsLiteralPart(part) || !this.IsIgnorableLiteral(part.GetValue(), includeLiterals)) {
				
		            var savedCaretPos = maskInfo.caretPos;
					maskInfo.BeforeChange(part);
				    partStart += maskInfo.caretPos - savedCaretPos;
				    
					if(this.savedKeyDownKeyInfo && this.savedKeyDownKeyInfo.keyCode == ASPx.Key.Decimal)
						keyInfo.keyCode = ASPx.CultureInfo.numDecimalPoint.charCodeAt(0);
					if(part.HandleKey(maskInfo, keyInfo, maskInfo.caretPos - partStart)) {								
						if(autoSkipLiterals)
							this.TrySkipLiteralOnPartEdge(maskInfo, partStart, i);
						maskInfo.AfterChange(part);
						return;
					}
					if(caretInfoBeforeSkip == null)
						caretInfoBeforeSkip = [ maskInfo.caretPos, maskInfo.selectionLength ];
				}									
				maskInfo.SetCaret(partStart + part.GetSize(), 0);
		    }
			partStart += part.GetSize();
		}
		if(caretInfoBeforeSkip != null)
			maskInfo.SetCaret(caretInfoBeforeSkip[0], caretInfoBeforeSkip[1]);
	},
	HandleControlKey: function(maskInfo, keyInfo) {
		maskInfo.selectionLength = 0;
		var partStart = 0;		
	    for(var i = 0; i < maskInfo.parts.length; i++) {
		    var part = maskInfo.parts[i];
		    if(this.IsCaretInPartOnControlKey(maskInfo.caretPos, partStart, part, keyInfo.keyCode)) {
		    
		        var savedCaretPos = maskInfo.caretPos;
				maskInfo.BeforeChange(part);
				partStart += maskInfo.caretPos - savedCaretPos;
				
			    part.HandleControlKey(maskInfo, keyInfo, maskInfo.caretPos - partStart);
				if(keyInfo.keyCode == ASPx.Key.Delete)
				    this.TrySkipLiteralOnPartEdge(maskInfo, partStart, i);
		        maskInfo.AfterChange(part);
			    break;
		    }
		    partStart += part.GetSize();
	    }
	},
	HandleMouseWheel: function(maskInfo, delta) {
		var partStart = 0;		
		for(var i = 0; i < maskInfo.parts.length; i++) {
			var part = maskInfo.parts[i];
			if(this.IsCaretInPartOnMouseWheel(maskInfo.caretPos, partStart, part)) {
			
		        var savedCaretPos = maskInfo.caretPos;
				maskInfo.BeforeChange(part);
				partStart += maskInfo.caretPos - savedCaretPos;

				part.HandleMouseWheel(maskInfo, delta, maskInfo.caretPos - partStart);
				maskInfo.AfterChange(part);
				break;
			}			
			partStart += part.GetSize();
		}
	},
	
	TrySkipLiteralOnPartEdge: function(maskInfo, partStartPos, partIndex) {
	    var part = maskInfo.parts[partIndex];
	    var posInPart = maskInfo.caretPos - partStartPos;
	    var amount = 0;
	    	    
        if(part.AllowIncreaseSize()) return;
        if(partIndex > maskInfo.parts.length - 3 ||  posInPart < part.GetSize()) return;
        var sibling = maskInfo.parts[partIndex + 1];
        if(this.IsLiteralPart(sibling))
            amount = sibling.GetSize();
	    
	    maskInfo.IncCaretPos(amount);
	    maskInfo.ApplyFixes(sibling);
	},
	
	ClearSelection: function(maskInfo){		
		var partStart = 0;
	    for(var i = 0; i < maskInfo.parts.length; i++) {
		    var part = maskInfo.parts[i];
		    var outerLeft = Math.min(partStart, maskInfo.caretPos);
		    var outerRight = Math.max(partStart + part.GetSize(), maskInfo.caretPos + maskInfo.selectionLength);
		    var size = part.GetSize();
		    if(size + maskInfo.selectionLength > outerRight - outerLeft){
		        var innerLeft = Math.max(partStart, maskInfo.caretPos);
		        var innerRight = Math.min(partStart + size, maskInfo.caretPos + maskInfo.selectionLength);		        
		        part.Clear(innerLeft - partStart, innerRight - partStart);
		        maskInfo.selectionLength += part.GetSize() - size;		        
		    }
		    partStart += part.GetSize();
	    }
	    maskInfo.selectionLength = 0;
	},
	
	IsCaretInPart: function(caretPos, partStartPos, part) {
		if(caretPos < partStartPos)
			return false;
		var nextPartPos = partStartPos + part.GetSize();
		if(caretPos > nextPartPos)
			return false;
		if(caretPos == nextPartPos)
			return part.AllowIncreaseSize();
		return true;
	},
	IsCaretInPartOnControlKey: function(caretPos, partStartPos, part, keyCode) {
		if(caretPos == partStartPos) {
			if(keyCode == ASPx.Key.Backspace || keyCode == ASPx.Key.Left)
				return false;
			return true;
		}
		var nextPartPos = partStartPos + part.GetSize();
		if(partStartPos < caretPos && caretPos < nextPartPos) 
			return true;
		if(caretPos == nextPartPos) {
		    if(keyCode == ASPx.Key.Up || keyCode == ASPx.Key.Down)
		        return part.SupportsUpDown();
			if(keyCode == ASPx.Key.Backspace || keyCode == ASPx.Key.Left)
				return true;
			return false;			
		}
		return false;
	},
	IsCaretInPartOnMouseWheel: function(caretPos, partStartPos, part) {
	    if(!part.SupportsUpDown()) 
	        return false;
	    return caretPos >= partStartPos && caretPos <= partStartPos + part.GetSize();
	},
	
    GetHintHtml: function(maskInfo) {
		var pos = 0;
		for(var i = 0; i < maskInfo.parts.length; i++) {
			var part = maskInfo.parts[i];
			if(this.IsCaretInPartOnMouseWheel(maskInfo.caretPos, pos, part))
				return part.GetHintHtml();
			pos += part.GetSize();
		}
		return "";
    },	
		
	CreateKeyInfo: function(keyCode, shiftState, ctrlState) {
		return {
			keyCode: keyCode,
			shiftState: shiftState,
			ctrlState: ctrlState
		};	
	},
	CreateKeyInfoByEvent: function(evt) {
	    return this.CreateKeyInfo(ASPx.Evt.GetKeyCode(evt), evt.shiftKey, evt.ctrlKey);
	},
	
    IsLiteralPart: function(part) {
		return (part.typeCode == LiteralMaskPart.prototype.typeCode);
    },    
    IsEnumPart: function(part) {
		return (part.typeCode == EnumMaskPart.prototype.typeCode);
    },
    IsRangePart: function(part) {
		return (part.typeCode == RangeMaskPart.prototype.typeCode);
    },
	IsAlphaCode: function(code) {		
		return (64 < code && code < 91 || 96 < code && code < 123 || code > 127);
	},
	IsDigitCode: function(code) {		
		return (47 < code && code < 58);
	},
    IsSignumCode: function(code) {		
		return (code == 43 || code == 45);
	},
	
	CanHandleControlKey: function(keyInfo) {
	    if(keyInfo.shiftState)
	        return false;
	    return keyInfo.keyCode == ASPx.Key.Up || keyInfo.keyCode == ASPx.Key.Down
	        || keyInfo.keyCode == ASPx.Key.Left || keyInfo.keyCode == ASPx.Key.Right
	        || keyInfo.keyCode == ASPx.Key.Backspace || keyInfo.keyCode == ASPx.Key.Delete;
	},	
	IsPrintableKeyCode: function(keyInfo) {
	    if(keyInfo.ctrlState) 
	        return false;
	    var code = keyInfo.keyCode;	    
	    return code == 32
	        || (code >= 48 && code <= 57)
	        || (code >= 65 && code <= 90)
	        || (code >= 96 && code <= 107)
	        || (code >= 109 && code <= 111)
	        || (code >= 186 && code <= 192)
	        || (code >= 219 && code <= 222);	    
	},
	
	IsIgnorableLiteral: function(text, mode) {
		if(mode == MaskIncludeLiterals.None)
			return true;
		if(mode == MaskIncludeLiterals.All)
			return false;
		return text != ASPx.CultureInfo.numDecimalPoint;
	}
};
MaskManager.keyCancelled = false;
MaskManager.keyDownHandled = false;
MaskManager.savedKeyDownKeyInfo = null;


var MaskDateTimeHelper = {

	GetDate: function(maskInfo, etalonDate, ignoreHasDateParts) {
		return this.CreateDateFromBag(this.GetDateBag(maskInfo, etalonDate), !ignoreHasDateParts && this.HasDateParts(maskInfo));
	},
	
	GetDateBag: function(maskInfo, etalonDate) {
		var bag = {
			year: 100, 
			month: 0, 
			day: 1,
			hours: 0,
			min: 0,
			sec: 0,
			msec: 0,
			pm: false,
			hasAmPm: false,
			hasDate: false
		};
        if(etalonDate) {
            bag.year = etalonDate.getFullYear();
            bag.month = etalonDate.getMonth();
            bag.day = etalonDate.getDate();
            bag.hours = etalonDate.getHours();
            bag.min = etalonDate.getMinutes();
            bag.sec = etalonDate.getSeconds();
            bag.msec = etalonDate.getMilliseconds();
        }
		for(var i = 0; i < maskInfo.parts.length; i++) {
			var part = maskInfo.parts[i];
			switch(part.dateTimeRole) {
				case "y":					
					bag.year = Number(part.GetValue());
					if(bag.year < 100)
						bag.year = ASPx.DateUtils.ExpandTwoDigitYear(bag.year);					
					bag.hasDate = true;
					break;
				case "M":
					bag.month = MaskManager.IsEnumPart(part) ? part.itemIndex : Number(part.GetValue()) - 1;
					bag.hasDate = true;
					break;
				case "d":
					if(!MaskManager.IsEnumPart(part)) {
						bag.day = Number(part.GetValue());
						bag.hasDate = true;
					}
					break;
				case "H":
				case "h":
					bag.hours = Number(part.GetValue());
					break;
				case "m":
					bag.min = Number(part.GetValue());
					break;
				case "s":
					bag.sec = Number(part.GetValue());
					break;
				case "f":
				case "F":
					bag.msec = Number(part.GetValue());
					break;
				case "t":
					bag.hasAmPm = true;
					bag.pm = MaskManager.IsEnumPart(part) && part.itemIndex > 0;
					break;
			}
		}
		if(bag.hasAmPm) {
			if(!bag.pm && bag.hours == 12)
				bag.hours = 0;
			if(bag.pm && bag.hours < 12)
			    bag.hours += 12;
		}
		return bag;
	},
	CreateDateFromBag: function(bag, allowNull) {
	    if(allowNull) {
		    if(bag.year == 100 && bag.month == 0 && bag.day == 1 
			    && bag.hours + bag.min + bag.sec + bag.msec == 0)
			    return null;
	    }
	    var date = new Date(bag.year, bag.month, bag.day, bag.hours, bag.min, bag.sec, bag.msec);
	    ASPx.DateUtils.FixTimezoneGap(
	        new Date(bag.year, bag.month, date.getDate() > 1 ? bag.day - 1 : bag.day + 1, bag.hours, bag.min, bag.sec, bag.msec),
	        date
	    );
	    return date;
	},
	
    GetMaxDayInMonth: function(month, year) {
		if(month == 1)
			return this.IsLeapYear(year) ? 29 : 28;
		if(month == 3 || month == 5 || month == 8 || month == 10)
			return 30;
		return 31;
    },
    IsLeapYear: function(year) {
		if(year % 4 != 0)
			return false;
		if(year % 100 == 0)		
			return year % 400 == 0;
		return true;
    },
    GetNextLeapYear: function(year) {
		var result = 4 * (1 + Math.floor(year / 4));
		if(!this.IsLeapYear(result))
			result += 4;
		return result;
    },	

    SetDate: function(maskInfo, date) {
		if(date == null)
			date = new Date(100, 0, 1);
		for(var i = 0; i < maskInfo.parts.length; i++) {
			var part = maskInfo.parts[i];
			part.EnsureValueInitialized();			
			switch(part.dateTimeRole) {
				case "y":
					this.SetYear(part, date);
					break;
				case "M":
					this.SetMonth(part, date);
					break;
				case "d":
					this.SetDay(part, date);
					break;
				case "h":
					this.SetHours(part, date, false);
					break;
				case "H":
					this.SetHours(part, date, true);
					break;	
				case "m":
					this.SetMinutes(part, date);
					break;
				case "s":
					this.SetSeconds(part, date);						
					break;
				case "f":
				case "F":
					this.SetMilliseconds(part, date);
					break;
				case "t":
					this.SetAmPm(part, date);						
					break;						
			}			
		}
    },
    
    
    SetYear: function(part, date) {
		if(!MaskManager.IsRangePart(part))
			return;
		var value = date.getFullYear();
		if(part.maxNumber < 100)
			value = value % 100;
		part.SetNumber(value);
    },
    SetMonth: function(part, date) {	
		if(MaskManager.IsRangePart(part))
			part.SetNumber(1 + date.getMonth());
		else if(MaskManager.IsEnumPart(part))
			part.itemIndex = date.getMonth();
    },
    SetDay: function(part, date) {
		if(MaskManager.IsRangePart(part))
			part.SetNumber(date.getDate());
		else if(MaskManager.IsEnumPart(part))
			part.itemIndex = date.getDay();
    },
    SetHours: function(part, date, full) {
		if(!MaskManager.IsRangePart(part))
			return;    
		var value = date.getHours();
		if(!full) {
			value = value % 12;
		    if(value == 0)
			    value = 12;
        }
		part.SetNumber(value);
    },
    SetMinutes: function(part, date) {
		if(!MaskManager.IsRangePart(part))
			return;    
		part.SetNumber(date.getMinutes());
    },
    SetSeconds: function(part, date) {
		if(!MaskManager.IsRangePart(part))
			return;
		part.SetNumber(date.getSeconds());
    },
    SetMilliseconds: function(part, date) {
		if(!MaskManager.IsRangePart(part))
			return;
		var value = date.getMilliseconds();
		while(value > part.maxNumber)
			value = value / 10;
		part.SetNumber(Math.round(value));
    },
    SetAmPm: function(part, date) {
		if(!MaskManager.IsEnumPart(part))
			return;    
		part.itemIndex = date.getHours() < 12 ? 0 : 1;
    },
    
    HasDateParts: function(maskInfo) {
        var list = maskInfo.parts;
        for(var i = 0; i < list.length; i++) {
            var role = list[i].dateTimeRole;
            if(role == "d" || role == "M" || role == "y")
                return true;
        }
        return false;
    }
};

ASPx.MaskPartBase = MaskPartBase;
ASPx.LiteralMaskPart = LiteralMaskPart;
ASPx.EnumMaskPart = EnumMaskPart;
ASPx.RangeMaskPart = RangeMaskPart;
ASPx.PromptMaskPart = PromptMaskPart;
ASPx.NumericMaskPart = NumericMaskPart;
ASPx.CharMaskPart = CharMaskPart;
ASPx.AlphaMaskPart = AlphaMaskPart;
ASPx.AlphaNumericMaskPart = AlphaNumericMaskPart;
ASPx.MaskParser = MaskParser;
ASPx.MaskInfo = MaskInfo;
ASPx.MaskManager = MaskManager;
ASPx.MaskDateTimeHelper = MaskDateTimeHelper;
ASPx.MaskIncludeLiterals = MaskIncludeLiterals;
})();