//TODO Crimp move in CommonFunctions
(function () {
    if (typeof xaf == "undefined") {
        xaf = {};
    }
})();

/*ControllersManager singleton*/
(function () {
    if (typeof xaf.ControllersManager == "undefined") {
        if (typeof window.top.xaf.ControllersManager == "undefined") {
            xaf.ControllersManager = {
                controllers: [],

                GetController: function (id) {
                    return this.controllers[id];
                },
                RegisterController: function (id, controller) {
                    this.controllers[id] = controller;
                },
                RemoveController: function (id) {
                    delete this.controllers[id];
                },
                CreateController: function (constructor, argArray) {
                    if (!argArray) {
                        return new constructor();
                    } else {
                        var args = [null].concat(argArray);
                        var factoryFunction = constructor.bind.apply(constructor, args);
                        return new factoryFunction();
                    }
                },
                ControllerExist: function (id) {
                    if (this.controllers[id] != undefined) {
                        return true;
                    }
                    return false;
                },
            };
        }
        else {
            xaf.ControllersManager = window.top.xaf.ControllersManager;
        }
    }
})();

xaf.FormsManager = {
    isSubmit: false,
    observableForms: [],
    ProcessForm: function () {
        for (var i = 0; i < document.forms.length; i++) {
            var form = document.forms[i];

            if (this.NeedProcessForm(form)) {
                this.ReplaceFormsSubmit(form)
                this.observableForms.push(form);
            }
        }
    },
    ReplaceFormsSubmit: function (form) {
        var originalSubmit = form.submit;
        form.submit = function () {
            xaf.FormsManager.isSubmit = true;
            this.submit = originalSubmit;
            var result = this.submit();
            xaf.FormsManager.isSubmit = false;
            return result;
        }

    },
    NeedProcessForm: function (form) {
        for (var i = 0; i < this.observableForms.length; i++) {
            if (this.observableForms[i] == form) {
                return false;
            }
        }
        return true;
    }
};
xaf.FormsManager.ProcessForm();