function getStations()
{
    var response = "";
    $.ajax({
        type:'GET',
        url:'http://motw.tech/code/GetStations.aspx',
        dataType:'json',
        async: false,
        success: function (responseData)
        {
            response = responseData;
        },
        error:function(errdata)
        {
            alert(errdata.statusText);
        }
    });
    return(response);
}
