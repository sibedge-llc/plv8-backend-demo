var jsonViewer = null;
var activeEditor = "graphql";

function executeScript(script, endpoint)
{
    $.ajax(
    {
        url: "/Main/" + endpoint,
        cache: false,
        traditional: true,
        type: "POST",
        dataType: "json",
        contentType: 'application/json',
        data: JSON.stringify({ Content: script }),

        success: function (data, textStatus, XMLHttpRequest) {
            console.log(data);
            jsonViewer.showJSON(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
        }
    });
}

$(document).ready(function ()
{
    jsonViewer = new JSONViewer();
    document.querySelector("#json").appendChild(jsonViewer.getContainer());

    $("button.load").click(() =>
    {
        executeScript(editAreaLoader.getValue(activeEditor + "Editor"), activeEditor);
    });

    $("#sqlRadio").click(() =>
    {
        activeEditor = "sql";
        $("#graphqlEditorHolder").hide();
        $("#sqlEditorHolder").show();
    });

    $("#graphqlRadio").click(() =>
    {
        activeEditor = "graphql";
        $("#graphqlEditorHolder").show();
        $("#sqlEditorHolder").hide();
    });

    editAreaLoader.init({
        id: "sqlEditor"
        , start_highlight: true
        , allow_resize: "both"
        , allow_toggle: false
        , word_wrap: true
        , language: "en"
        , syntax: "sql"
    });

    editAreaLoader.init({
        id: "graphqlEditor"
        , start_highlight: true
        , allow_resize: "both"
        , allow_toggle: false
        , word_wrap: true
        , language: "en"
        , syntax: "css"
    });

    setTimeout(() => $("#sqlEditorHolder").hide(), 100);
});
