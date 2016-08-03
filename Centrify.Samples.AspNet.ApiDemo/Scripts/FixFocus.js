var lastFocusedControlId = "";

function focusHandler(e) {
    document.activeElement = e.originalTarget;
}

function appInit() {
    if (typeof (window.addEventListener) !== "undefined") {
        window.addEventListener("focus", focusHandler, true);
    }
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoading(pageLoadingHandler);
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(pageLoadedHandler);
}

function pageLoadingHandler(sender, args) {
    lastFocusedControlId = typeof (document.activeElement) === "undefined"
        ? "" : document.activeElement.id;
}

function focusControl(targetControl) {
    if (Sys.Browser.agent === Sys.Browser.InternetExplorer) {
        var focusTarget = targetControl;
        if (focusTarget && (typeof (focusTarget.contentEditable) !== "undefined")) {
            oldContentEditableSetting = focusTarget.contentEditable;
            focusTarget.contentEditable = false;
        }
        else {
            focusTarget = null;
        }
        targetControl.focus();
        if (focusTarget) {
            focusTarget.contentEditable = oldContentEditableSetting;
        }
    }
    else {
        targetControl.focus();
    }
}

function pageLoadedHandler(sender, args) {
    if (typeof (lastFocusedControlId) !== "undefined" && lastFocusedControlId != "") {
        var newFocused = $get(lastFocusedControlId);
        if (newFocused) {
            focusControl(newFocused);
        }
    }
}

Sys.Application.add_init(appInit);