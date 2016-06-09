(function() {
    var recordSeparator = '|';

    ASPx.HtmlEditorClasses.Managers.ClientStateManager = ASPx.CreateClass(null, {
        constructor: function(htmlEditor) {
            this.htmlEditor = htmlEditor;
            this.clientStateInput = null;
            this.fieldsNameValueCollection = null;
        },
        GetFieldNameValueCollection: function(){
            if(!this.fieldsNameValueCollection)
                this.fieldsNameValueCollection = this.parseStateStr();
            return this.fieldsNameValueCollection;
        },
        setFieldValue: function(name, value, saveToCookie) {
            var collection = this.GetFieldNameValueCollection();
            collection[name] = value;
            this.save(collection, saveToCookie);
        },
        parseStateStr: function() {
            var state = this.htmlEditor.stateObject ? this.htmlEditor.stateObject.clientState : "";
            collection = {};
            var startIndex = 0;
            while(startIndex < state.length)
                startIndex = this.parseFieldRecord(state, startIndex, collection);
            return collection;
        },
        parseFieldRecord: function(state, startIndex, collection) {
            var indexOfFirstSeparator = state.indexOf(recordSeparator, startIndex);
            var fieldName = state.substr(startIndex, indexOfFirstSeparator - startIndex);
            startIndex += fieldName.length + 1;
            var indexOfSecondSeparator = state.indexOf(recordSeparator, startIndex);
            var fieldValueLengthStr = state.substr(startIndex, indexOfSecondSeparator - startIndex);
            startIndex += fieldValueLengthStr.length + 1;
            var fieldValueLength = parseInt(fieldValueLengthStr);
            var fieldValue = state.substr(startIndex, fieldValueLength);
            startIndex += fieldValueLength;
            collection[fieldName] = fieldValue;
            return startIndex;
        },
        save: function(collection, saveToCookie) {
            var result = [];
            for(var fieldName in collection) {
                var value = collection[fieldName];
                if(typeof(value) == "number")
                    value = value.toString();
                if(typeof(value) == "string")
                    result.push(fieldName + recordSeparator + value.length + recordSeparator + value);
            }
            var state = result.join('');
            this.htmlEditor.stateObject.clientState = state;
        
            if(saveToCookie && this.htmlEditor.cookieName != "") {
                ASPx.Cookie.DelCookie(this.htmlEditor.cookieName);
                ASPx.Cookie.SetCookie(this.htmlEditor.cookieName, state);
            }
        }
    });
})();