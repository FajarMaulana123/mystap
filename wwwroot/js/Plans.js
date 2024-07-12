$(document).ready(function () {
    $('#Customers').dataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/get_",
            "type": "POST",
            "datatype": "json"
        },

        "columns": [

            { "data": "planDesc", "name": "planDesc", "autowidth": true },


        ]
    });
});