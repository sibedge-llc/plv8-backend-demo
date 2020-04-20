$(document).ready(function()
{
    var jsonObj = {};
    var jsonViewer = new JSONViewer();
    document.querySelector("#json").appendChild(jsonViewer.getContainer());

    $("button.load-sql").click(() =>
    {
        $.ajax(
        {
            url: "/Main/sql",
            cache: false,
            traditional: true,
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify({ Content: $("textarea").val() }),
        
            success: function (data, textStatus, XMLHttpRequest)
            {
                console.log(data);
                jsonViewer.showJSON(data);
            },
            error: function(jqXHR, textStatus, errorThrown)
            {
                console.log(textStatus, errorThrown);
            }
        });
    });

    $("button.collapse").click(() => jsonViewer.showJSON(jsonObj, null, 1));
    $("button.maxlvl").click(() => jsonViewer.showJSON(jsonObj, 1));
});
