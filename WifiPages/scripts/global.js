// Delegate Load and Unload events to scripts which are not defined in HTML head
function OnLoad() {
  try {
    DoOnload();
  }
  catch (e) { }
}
function OnUnload() {
  try {
    DoOnunload();
  }
  catch (e) { }
}
window.onload = OnLoad;
window.onunload = OnUnload;

function inputOnFocus(object, val) {
    if (object.value.replace(/^\s+|\s+$/g, "") == val) {
        object.value = "";
    }
}

function inputOnBlur(object, val)
{
    if (object.value.replace(/^\s+|\s+$/g, "") == "")
    {
        object.value = val;
    }
}