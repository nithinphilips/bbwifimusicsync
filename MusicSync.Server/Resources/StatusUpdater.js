$(document).ready(onReady);

function onReady()
{
    updateStatus(); // first update.
    setInterval(updateStatus, 2000); 
}

function updateStatus()
{
    var jqxhr = $.getJSON('/getStatus', function(data)
        {
            $("#serverStatus").html(data.Status);
        });

    jqxhr.error(function() { $("#serverStatus").html("Server cannot be reached. It may be down."); })
}
